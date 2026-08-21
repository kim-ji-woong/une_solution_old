using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TcpLib2;
using UnE.Sensor;

namespace BroadcastServer
{
    partial class BroadcastServerService : ServiceBase
    {
        private TcpServer server;
        private ServiceProvider provider;

        private static BroadcastServerService m_instance;
        public static BroadcastServerService Instance
        {
            get { return m_instance; }
        }

        private bool m_runThread = false;

        private List<TcpLib2.ConnectionState> m_connectionStates = new List<TcpLib2.ConnectionState>();
        private WebDBManager m_dbManager = null;

        //private StreamWriter m_sw = new StreamWriter(@"C:\UNE\Log\BroadcastServer.log");

        private int m_nRecentSec = 30;
        public BroadcastServerService()
        {
            m_instance = this;
            InitializeComponent();

            string strSiteID = System.Configuration.ConfigurationManager.AppSettings.Get("siteid");
            
            if (strSiteID != null && strSiteID.Length > 0)
            {
                int nSiteID;

                if (int.TryParse(strSiteID.Trim(), out nSiteID))
                {
                    m_dbManager = new WebDBManager(nSiteID);
                    
                    m_dbManager.DatabaseName = System.Configuration.ConfigurationManager.AppSettings.Get("dbName");
                    m_dbManager.DatabaseTypeName = System.Configuration.ConfigurationManager.AppSettings.Get("dbType");
                    m_dbManager.WebServerURL = System.Configuration.ConfigurationManager.AppSettings.Get("WebServerURL");
                }
            }
            else
                return;

            string strPort = System.Configuration.ConfigurationManager.AppSettings.Get("port");
            if (strPort != null && strPort.Length > 0)
            {
                int nPort;

                if (int.TryParse(strPort.Trim(), out nPort))
                {
                    provider = new ServiceProvider();
                    server = new TcpServer(provider, nPort);
                    server.Start();
                }
            }
            else
                return;

            string strRecentSec = System.Configuration.ConfigurationManager.AppSettings.Get("nRecentSec");
            if (strRecentSec != null && strRecentSec.Length > 0)
            {
                int nRecentSec;
                if (int.TryParse(strRecentSec.Trim(), out nRecentSec))
                {
                    m_nRecentSec = nRecentSec;
                }
            }
        }

        //private void WriteLog(string content)
        //{
        //    m_sw.WriteLine(content);
        //    m_sw.Flush();
        //}

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            Thread t = new Thread(new ThreadStart(DisplayCommand));
            t.Start();

            Logger.Instance.Write("OnStart");
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.

            Logger.Instance.Write("OnStop");
            m_runThread = false;
        }

        private void DisplayCommand()
        {            
            m_runThread = true;
            while (m_runThread)
            {
                ArrayList arrResult = m_dbManager.GetResultData("Select ID, TimeStamp, FacilityType, IsBegin From BroadcastCommand");                
                if (arrResult != null && arrResult.Count > 0)
                {
                    for (int i = 0; i < arrResult.Count; i+=4)
                    {
                        VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                        VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                        VariousData<int> nFacilityType = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                        VariousData<int> nIsBegin = WebDBManager.GetIntField(arrResult[i + 3].ToString());

                        if (nID == null || timeStamp == null || nFacilityType == null || nIsBegin == null)
                            continue;

                        bool bTimeover = false;

                        DateTime now = DateTime.Now.AddSeconds(-m_nRecentSec);
                        if (timeStamp.Data >= now)
                        {
                            string sendMessage = "U";
                            if (nFacilityType.Data == 0) //facilityType:2byte
                                sendMessage += "00";
                            else
                                sendMessage += nFacilityType.Data.ToString();

                            if (nIsBegin.Data == 1)
                                sendMessage += ":S";
                            else
                                sendMessage += ":E";

                            byte[] bytes = Encoding.ASCII.GetBytes(sendMessage);
                            foreach (TcpLib2.ConnectionState state in m_connectionStates)
                            {
                                state.LengthAdd = false;
                                state.Write(bytes, 0, bytes.Length);

                                Logger.Instance.Write(sendMessage);
                            }
                        }
                        else
                        {
                            bTimeover = true;
                        }

                        string deleteQuery = string.Format("Delete from BroadcastCommand Where ID = {0} And FacilityType = {1}", nID.Data, nFacilityType.Data);
                        m_dbManager.GetResultData(deleteQuery);

                        IFacility.FacilityType facilityType = IFacility.ToFacilityType(nFacilityType.Data);
                        string strMsg = IFacility.GetFacilityTypeString(facilityType).Replace(" ", "").Replace("센서", "");
                        if (bTimeover)
                            strMsg += "시간 초과";
                        else
                            strMsg += (nIsBegin.Data == 0) ? "중지" : "실행";

                        StringBuilder historyQuery = new StringBuilder();
                        historyQuery.Append("Insert into BroadcastHistory(Text, UseSiren, PlayOption, RepeatCount, HostInfo, AddTime, SiteID) ");
                        historyQuery.AppendFormat("Values('{0}', 0,0,0,'',getDate(), 205)", strMsg);

                        m_dbManager.GetResultData(historyQuery.ToString());
                    }
                }

                Thread.Sleep(500);
            }
        }

        public void OnAccept(TcpLib2.ConnectionState state)
        {
            //System.Net.IPEndPoint endPoint = (System.Net.IPEndPoint)state.RemoteEndPoint;
            //string strIP = endPoint.Address.ToString();
            //int nPort = endPoint.Port;
            
            if (m_connectionStates.Contains(state))
                m_connectionStates.Remove(state);
            else
                m_connectionStates.Add(state);
        }

        public void OnDropConnection(TcpLib2.ConnectionState state)
        {
            //System.Net.IPEndPoint endPoint = (System.Net.IPEndPoint)state.RemoteEndPoint;
            //string strIP = endPoint.Address.ToString();
            //int nPort = endPoint.Port;

            m_connectionStates.Remove(state);
        }

        public class State
        {
            private string m_strIP = "";
            private int m_nPort = -1;

            public string IP
            {
                get { return m_strIP; }
                set { m_strIP = value; }
            }

            public int Port
            {
                get { return m_nPort; }
                set { m_nPort = value; }
            }
        }
    }
}
