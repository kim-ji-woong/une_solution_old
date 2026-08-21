using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using DBUtility2;
using SDMS;
using System.Threading;
using System.Collections;
using SOPWebClient;

namespace ExternalFireSensorDBWatcher
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private bool m_shutdownThread = false;
        private bool m_isConnected = false;
        private DateTime m_dtLastSendMessage = new DateTime();

        private int m_nClientType = SOPWebServer.ClientType.FIRE_SENSOR_SERVER;
        private int m_nClientSubType = SOPWebServer.ClientSubType.ASIN;

        private int m_nSiteID = 1;
        private WebDBManager m_dbMgr = null;
        
        private const string APP_NAME = "아신EventReceiver";

        private static NetworkWebManager m_manager = null;
        public static NetworkWebManager Instance
        {
            get
            {
                if (m_manager == null)
                    m_manager = new NetworkWebManager();
                return m_manager;
            }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        private void WriteLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.Write(str);
        }

        private void WriteLineLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str);
        }

        private void InitLog()
        {
            if (ConnectionLogEx.MakeInstance())
                m_bIsLogOpened = true;
            else
                m_bIsLogOpened = false;
        }

        private bool m_bIsLogOpened = false;
        public bool IsLogOpened
        {
            get { return m_bIsLogOpened; }
            set { m_bIsLogOpened = value; }
        }
        
        protected NetworkWebManager()
        {
            InitLog();

            if (ReadSiteID())
            {
                m_dbMgr = new WebDBManager(m_nSiteID);
            }

            int nPort = ReadServerPort(m_dbMgr);
            SetPostBox(nPort);

            Thread t;
            t = new Thread(ConnectionThread);
            t.Name = "ConnectionThread";
            t.Start();
        }

        private void SetPostBox(int nPort)
        {
            m_postBox = new PostBox();
            m_postBox.WebServerURL = m_dbMgr.WebServerURL;
            m_postBox.Port = nPort;
            m_postBox.PostMan = this;
        }

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            DateTime dtLog = new DateTime(nYear, nMonth, nDay);
            TimeSpan span = dtNow - dtLog;
            return span.TotalDays > 30.0;
        }

        private bool ReadSiteID()
        {
            Utility util = new Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                ConnectionLogEx.Instance.WriteLine("Site ID가 지정되지 않았습니다. ini파일을 확인하세요");
                //UnE.Utility.UMessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int nSiteId = 1;

            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                ConnectionLogEx.Instance.WriteLine("잘못된 Site ID입니다. ini파일을 확인하세요");
                //UnE.Utility.UMessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private int ReadServerPort(WebDBManager dbMgr)
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            while (!m_shutdownThread)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort(m_dbMgr);

                    if (m_postBox != null && m_postBox.Port != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(m_nClientType, m_nClientSubType))
                        {
                            m_isConnected = true;
                        }
                    }
                }
                else
                {
                    TimeSpan span = DateTime.Now - m_dtLastSendMessage;

                    // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                    if (span.TotalSeconds > 3.0)
                    {
                        // 접속이 유지되고 있는지 확인한다.
                        SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                    }
                }

                Thread.Sleep(1000);
            }
        }
        
        // 알람발생
        public bool SendAlarm(int nSensorZoneID, int nSensorTagInfoID)
        {
            int nSensorType = (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;
            int nData = 1;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorType);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
            return true;
        }

        // 알람해제
        public bool SendAlarmClear(int nSensorZoneID, int nSensorTagInfoID)
        {
            int nSensorType = (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;
            int nData = 0;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorType);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
            return true;
        }

        private void WriteSendLog(int header, byte[] bytes)
        {
            if (header == SOPWebServer.Header.ARE_YOU_THERE)
                return;

            string strLog = string.Format("SendMessage : Header({0}), Length({1})", header, (int)bytes.Length);
            string strBytes = "";

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];

                if (strBytes.Length == 0)
                    strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                else
                    strBytes += string.Format(" {0:X2}", (int)b);
            }

            WriteLog(strLog + strBytes);
        }

        public bool SendMessage(int header, byte[] messages)
        {
            if (m_postBox == null || m_isConnected == false)
            {
                m_isConnected = false;
            }
            else
            {
                bool closeConnection;
                bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                if (closeConnection)
                {
                    WriteLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else if (result == true)
                {
                    m_dtLastSendMessage = DateTime.Now;
                    WriteSendLog(header, messages);
                }

                return result;
            }

            return false;
        }

        public void OnMessage(int header, byte[] messages)
        {
            
        }
    }

    public class ConnectionLogEx : ConnectionLog
    {
        private log4net.ILog logger = null;

        public static ConnectionLogEx Instance
        {
            get
            {
                return (ConnectionLogEx)m_instance;
            }
        }

        public static bool MakeInstance()
        {
            if (m_instance == null)
                m_instance = new ConnectionLogEx();

            ConnectionLogEx instance = (ConnectionLogEx)m_instance;
            instance.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            instance.m_isOpened = true;
            return instance.m_isOpened;
        }

        public override bool Write(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.DebugFormat("{0}", str);

            return true;
        }

        public override bool WriteLine(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.Debug(str);

            return true;
        }
    }
}
