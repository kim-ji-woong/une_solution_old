using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlackoutSensorServer.Data;
using DBUtility2;
using SOPWebClient;
using UnE.Sensor;

namespace BlackoutSensorServer.Network
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private bool m_isConnected = false;
        private DateTime m_dtLastSendMessage = new DateTime();

        private int m_nClientType = SOPWebServer.ClientType.ETC;
        private int m_nClientSubType = SOPWebServer.ClientSubType.OFFICE_BUILDING;

        private WebDBManager m_dbMgr = null;

        private Logger m_logger = null;

        private bool m_shutdownThread = true;

        private Dictionary<int, SensorTag> m_dicSensors = null;
        
        public NetworkWebManager()
        {
            if (Init())
            {
                int nPort = ReadServerPort();
                SetPostBox(nPort);

                Thread t = new Thread(new ThreadStart(ConnectionThread));
                t.Start(); 
            }
        }

        private bool Init()
        {
            int nSiteID;
            
            if (ReadConfig("siteid", out nSiteID) == false)
                return false;

            string strWebServerURL, strIP;

            if (ReadURL(out strWebServerURL, out strIP) == false)
                return false;

            string strDBName = System.Configuration.ConfigurationManager.AppSettings["dbname"].ToString().Trim();

            if (strDBName.Length == 0)
                return false;

            m_dbMgr = new WebDBManager(strDBName, nSiteID);
            m_dbMgr.WebServerURL = strWebServerURL;
            m_dbMgr.DatabaseType = (int)WebDBManager.DBType.sqlserver;
            
            m_dicSensors = Data.SensorTag.ReadSensors(m_dbMgr);

            string strServerIP = System.Configuration.ConfigurationManager.AppSettings["rabbitmqServerIP"].ToString().Trim();

            //int nPort;
            //if (ReadConfig("bacnetPort", out nPort) == false)
            //    return false;

            string strLogFolder = System.Configuration.ConfigurationManager.AppSettings["logFolder"].ToString().Trim();
            string strLifeTime = System.Configuration.ConfigurationManager.AppSettings["logLifeTime"].ToString().Trim();
            string strFileName = System.Configuration.ConfigurationManager.AppSettings["logFileTag"].ToString().Trim();

            if (strLogFolder.Length > 0 && strLifeTime.Length > 0 && strFileName.Length > 0)
            {
                int nLifeTime;

                if (int.TryParse(strLifeTime, out nLifeTime) && nLifeTime > 0)
                    m_logger = new Logger(strLogFolder, strFileName, nLifeTime);
            }

            return true;
        }

        private bool ReadConfig(string strName, out int value)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strName].ToString().Trim();
            return int.TryParse(strValue, out value);
        }

        private bool ReadURL(out string strWebServerURL, out string strIP)
        {
            strIP = "";
            strWebServerURL = System.Configuration.ConfigurationManager.AppSettings["webserver"].ToString().Trim();

            if (strWebServerURL.Length == 0)
                return false;

            string str = strWebServerURL.ToLower();
            string strURL = "";

            if (str.StartsWith("http://") == false)
            {
                strURL = strWebServerURL;
                strWebServerURL = "http://" + strWebServerURL;
            }
            else
            {
                strURL = str.Replace("http://", "");
            }

            int nIndex = strURL.IndexOf(':');

            if (nIndex < 0)
                strIP = strURL;
            else
                strIP = strURL.Substring(0, nIndex);

            return true;
        }

        private void SetPostBox(int nPort)
        {
            m_postBox = new PostBox();
            m_postBox.WebServerURL = m_dbMgr.WebServerURL;
            m_postBox.Port = nPort;
            m_postBox.PostMan = this;
        }

        private void ConnectionThread()
        {
            m_shutdownThread = false;

            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_postBox != null && m_postBox.Port != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(m_nClientType, m_nClientSubType))
                            m_isConnected = true;
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

        public void OnMessage(int header, byte[] messages)
        {
            RecvLog(header, messages);

            if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                m_isConnected = false;
            }
        }

        private int ReadServerPort()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select Port from SensorServerPort ");
            sb.AppendFormat("Where Name='{0}' And SiteID={1} ", SOPWebServer.ServerPort.SOP_WEB_SERVER, m_dbMgr.SiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());
            if (port == null)
                return -1;

            return port.Data;
        }

        public void OnBlackoutSignal(string toState, string objectName)
        {
            WriteLog(toState + " / " + objectName);
            foreach (KeyValuePair<int, SensorTag> sensor in m_dicSensors)
            {
                SensorTag tag = sensor.Value;
                if (tag.Codes.Contains(objectName))
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add((int)IFacility.FacilityType.BLACKOUT);
                    arrDatas.Add(tag.ID);
                    arrDatas.Add(tag. SensorZoneID);
                    if (toState == "OFFNORMAL") // 정전 발생
                        arrDatas.Add(1);
                    else if (toState == "NORMAL")
                        arrDatas.Add(-1);

                    byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

                    SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
                }
            }
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
                    m_isConnected = false;
                }
                else
                    m_dtLastSendMessage = DateTime.Now;

                return result;
            }

            return false;
        }

        public void RecvLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "RecvMessage");
        }

        private void MessageLog(int header, byte[] bytes, string strMessageTag)
        {
            if (header != SOPWebServer.Header.ARE_YOU_THERE &&
                header != SOPWebServer.Header.I_AM_HERE)
            {
                string strLog = "";

                if (bytes == null)
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length(0)", header);
                }
                else
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length({1})", header, bytes.Length);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    strLog += strBytes;
                }

                WriteLog(strLog);
            }
        }

        private void WriteLog(string strLog)
        {
            if (m_logger != null)
                m_logger.Write(strLog);
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        public void Close()
        {
            if (this.m_isConnected)
            {
                // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                // 실패하더라도 상관없다.
                bool closeConnection;
                m_postBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                this.m_isConnected = false;
            }

            m_shutdownThread = true;
        }
    }
}
