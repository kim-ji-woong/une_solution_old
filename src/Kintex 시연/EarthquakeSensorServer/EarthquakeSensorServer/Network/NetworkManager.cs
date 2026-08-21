using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using SDMS;
using System.Threading;
//using DBUtility;
using System.Collections;

namespace EarthquakeSensorServer.Network
{
    public class NetworkManager
    {
        private ClientProvider m_provider = null;
        private int m_nPort = -1;
        private string m_strServerAddr = "";
        private bool shutdownThread = false;
        private ConnectionLogEx m_log = new ConnectionLogEx();
        //private WebDBManager m_dbMgr = null;

        public ClientProvider ClientProvier
        {
            get { return m_provider; }
        }

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        private void WriteLog(object str)
        {
            if (m_log.IsOpened)
                m_log.Write(str);
        }

        private void WriteLineLog(object str)
        {
            if (m_log.IsOpened)
                m_log.WriteLine(str);
        }

        private void InitLog()
        {
            m_log.Create();
        }
        
        public bool IsLogOpened
        {
            get { return m_log.IsOpened; }
        }

        public void RecvLog(byte[] bytes)
        {
            if (!IsLogOpened)
                return;

            if (bytes[0] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
            {
                string strLog = string.Format("RecvMessage : Header({0}), Length({1}), EarthquakeSensorServer", (int)bytes[0], (int)bytes.Length);
                string strBytes = "";

                foreach (byte b in bytes)
                {
                    if (strBytes.Length == 0)
                        strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                    else
                        strBytes += string.Format(" {0:X2}", (int)b);
                }

                WriteLineLog(strLog + strBytes);
            }
        }

        public int Send(byte[] bytes, ClientProvider provider)
        {
            if (provider.IsClientDisposed == true)
                return -1;

            if (provider.IsConnected == false)
            {
                Thread.Sleep(1000);
                if (provider.IsConnected == false)
                    return -1;
            }

            int nResult = provider.Send(bytes, 0, bytes.Length);

            if (nResult > 0)
            {
                if (!IsLogOpened)
                    return nResult;

                if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                {
                    string strLog = string.Format("SendMessage : Header({0}), Length({1}), EarthquakeSensorServer", (int)bytes[0], (int)bytes.Length);


                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    WriteLineLog(strLog + strBytes);
                }
            }

            return nResult;
        }

        private int m_nSiteID = 1;
        public NetworkManager(int nSiteID/*, WebDBManager dbMgr*/)
        {
            m_nSiteID = nSiteID;
            //m_dbMgr = dbMgr;
            InitLog();

            //string strPort = FormSOP.Instance.DBManager.LoadIni("sdms_port", "Server Connection Info");

            /*string strServerURL = DBUtility.RegUtil.ReadRegValue("Server Connection Info", "webserver_url", m_nSiteID);

            if (strServerURL == null || strServerURL.Length == 0)
                strServerURL = m_dbMgr.WebServerURL;

            int nIndex1 = strServerURL.IndexOf("http://");
            int nIndex2 = strServerURL.LastIndexOf(':');
            string strURL = strServerURL;

            if (nIndex1 >= 0 && nIndex2 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
            }
            else if (nIndex1 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex);
            }
            else if (nIndex2 >= 0)
            {
                strURL = strServerURL.Substring(0, nIndex2);
            }

            System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);*/

            m_provider = new ClientProvider(this);
            //m_strServerAddr = addr[0].ToString();

            m_strServerAddr = "127.0.0.1";

            Thread t;
            t = new Thread(ConnectionThread);
            t.Name = "ConnectionThread";
            t.Start();
        }
        
        private int GetServerPort()
        {
            return 9908;
            /*string strSQL = "Select Port from SDMSServerPort where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;*/
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            while (!shutdownThread)
            {
                lock (this)
                {
                    if (m_provider.IsConnected)
                    {
                        if (m_provider.PingCount > 5)
                        {
                            m_provider.PingCount = 0;

                            try
                            {
                                m_log.WriteLine("PING COUNT EXCEPTION");
                                m_provider.Close();
                            }
                            catch (System.Exception)
                            {

                            }

                        }
                        // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                        else if (m_provider.IsReadingProcess)
                            m_provider.SendData(TCP_ID.I_AM_HERE);
                        else
                            m_provider.PingCount++;
                    }
                    
                    if (!m_provider.IsConnected)
                    {
                        m_nPort = GetServerPort();
                        try
                        {
                            if (m_nPort > 0)
                            {
                                m_provider.Connect(m_strServerAddr, m_nPort);
                            }
                        }
                        catch (System.Exception)
                        {

                        }

                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void OnDropConnection()
        {
            lock (this)
            {
                m_provider = new ClientProvider(this);
            }
        }

        public bool SendMessage(short header)
        {
            if (m_provider == null)
                return false;

            //lock (this)
            {
                m_provider.SendData(header);
            }
            return true;
        }

        public bool SendMessage(short header, float data)
        {
            if (m_provider != null)
                return false;

            //lock (this)
            {
                byte[] datas = BitConverter.GetBytes(data);
                m_provider.SendData(header, TCP_TYPE.INTEGER, datas);
            }
            return true;
        }

        public bool SendMessage(short header, string data)
        {
            if (m_provider != null)
                return false;

            //lock (this)
            {
                UTF8Encoding enc = new UTF8Encoding();
                byte[] datas = enc.GetBytes(data);
                m_provider.SendData((short)header, TCP_TYPE.STRING, datas);
            }
            return true;
        }

        public bool SendMessage(short header, int data)
        {
            if (m_provider != null)
                return false;

            //lock (this)
            {
                byte[] datas = BitConverter.GetBytes(data);
                m_provider.SendData((short)header, TCP_TYPE.INTEGER, datas);
            }

            return true;
        }

        public void SendEarthquakeSignal(int nSensorID, float fMagnitude, int nIntensity, int nAlarmLevel, string strPosition, DateTime time)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorID);
            arrDatas.Add(fMagnitude);
            arrDatas.Add(nIntensity);
            arrDatas.Add(nAlarmLevel);
            arrDatas.Add(strPosition);
            arrDatas.Add(time.ToBinary());

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.EARTHQUAKE_SENSOR_DETECT, arrDatas);
            Send(bytes, m_provider);

            if (m_nSiteID == 2)
            {
                if (FormMain.Instance.GetOptionSOPSimulatorBoolean("UseSMS"))
                {
                    if (FormMain.Instance.IsAfterSignal() == false)
                    {
                        //SOPSMS.SendSOPSMS(m_nSiteID, m_dbMgr, nIntensity, fMagnitude);
                    }
                    else
                    {
                        //System.Threading.Thread.Sleep(3000);
                        //SOPSMS.SendSecondSMS(m_dbMgr, 582);
                        //SOPSMS.SendSecondBroadcast(m_dbMgr, 584);
                    }
                }
            }
        }

        public void SendSDMSView(string szViewName, DateTime dtTime)
        {
            ArrayList arrDatas = new ArrayList();
            
            arrDatas.Add(SDMSCommandType.SET_VIEW);
            arrDatas.Add(szViewName);

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.SDMS_COMMAND, arrDatas);

            try
            {
                Send(bytes, m_provider);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }            
        }
    }

    public class ConnectionLogEx : ConnectionLog
    {
    }
 }
