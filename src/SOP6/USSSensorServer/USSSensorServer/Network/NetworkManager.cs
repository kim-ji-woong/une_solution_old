using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using SOPWebClient;
using System.Collections;
using System.Threading;
using UnE.Sensor;
using USSSensorServer.Data;

namespace USSFireSensorServer.Network
{
    public class NetworkManager : IUSSServiceOwner
    {
        private class PostMan : IPostMan
        {
            private PostBox m_postBox = null;
            private NetworkManager m_owner = null;
            private int m_nClientType = -1;
            private int m_nClientSubType = -1;
            private bool m_isConnected = false;
            private int m_nPort = -1;
            private DateTime m_dtLastSendMessage = new DateTime();
            private int m_nPrevIntensity = -1;
            // Key : SensorID
            private Dictionary<int, float> m_dicPrevDataf = new Dictionary<int, float>();

            public PostBox PostBox
            {
                get { return m_postBox; }
                set
                {
                    m_postBox = value;
                    m_nPrevIntensity = -1;
                    m_dicPrevDataf.Clear();
                }
            }

            public int ClientType
            {
                get { return m_nClientType; }
            }

            public int ClientSubType
            {
                get { return m_nClientSubType; }
            }

            public bool IsConnected
            {
                get { return m_isConnected; }
                set
                {
                    if (m_isConnected != value)
                    {
                        m_isConnected = value;
                        m_nPrevIntensity = -1;
                        m_dicPrevDataf.Clear();
                    }
                }
            }

            public int Port
            {
                get { return m_nPort; }
                set { m_nPort = value; }
            }

            public DateTime LastSendMessageTime
            {
                get { return m_dtLastSendMessage; }
            }

            public int PrevIntensity
            {
                get { return m_nPrevIntensity; }
                set { m_nPrevIntensity = value; }
            }

            public PostMan(NetworkManager owner, int nClientType, int nClientSubType)
            {
                m_owner = owner;
                m_nClientType = nClientType;
                m_nClientSubType = nClientSubType;
            }

            public void OnMessage(int header, byte[] messages)
            {
                if (m_owner != null)
                    m_owner.OnMessage(header, messages, this);
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
                        if (m_owner != null)
                            m_owner.WriteLog(m_postBox.ErrorMessage);

                        m_isConnected = false;
                    }
                    else
                        m_dtLastSendMessage = DateTime.Now;

                    return result;
                }

                return false;
            }

            public float GetPrevDataf(int nSensorID)
            {
                float fData;

                if (m_dicPrevDataf.TryGetValue(nSensorID, out fData))
                    return fData;

                return -1.0f;
            }

            public void SetPrevDataf(int nSensorID, float fData)
            {
                m_dicPrevDataf[nSensorID] = fData;
            }
        }

        private PostMan m_postManFire = null;
        private PostMan m_postManEarthquake = null;
        private PostMan m_postManEtc = null;
        private List<PostMan> m_postManList = new List<PostMan>();
        private Logger m_logger = null;
        private WebDBManager m_dbMgr = null;
        private bool m_shutdownThread = false;

        private IServiceOwner m_owner = null;
        private IUIOwner m_uiOwner = null;

        // Key : SensorTagID
        private Dictionary<int, Data.SensorTag> m_dicSensorTags = null;

        private ClientProvider m_provider = null;
        private USSServer m_ussServer = null;

        private AccessControl m_accessCtrl = null;

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public NetworkManager(IServiceOwner owner, IUIOwner uiOwner)
        {
            m_owner = owner;
            m_uiOwner = uiOwner;

            if (Init())
            {
                m_dicSensorTags = Data.SensorTag.ReadFireSensors(m_dbMgr);

                int nPort = ReadServerPort();

                m_postManFire = new PostMan(this, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.Parc1);
                m_postManEarthquake = new PostMan(this, SOPWebServer.ClientType.EARTHQUAKE_SENSOR_SERVER, SOPWebServer.ClientSubType.Parc1);
                m_postManEtc = new PostMan(this, SOPWebServer.ClientType.ETC, SOPWebServer.ClientSubType.Parc1);

                SetPostBox(m_postManFire, nPort);
                SetPostBox(m_postManEarthquake, nPort);
                SetPostBox(m_postManEtc, nPort);

                m_postManList.Add(m_postManFire);
                m_postManList.Add(m_postManEarthquake);
                m_postManList.Add(m_postManEtc);

                m_accessCtrl = new AccessControl(m_dbMgr);

                Thread t = new Thread(new ThreadStart(ConnectionThread));
                t.Start();
                Thread t2 = new Thread(new ThreadStart(ConnectionThreadUSS));
                t2.Start();

                SetUSSServer();
            }
        }

        private void SetUSSServer()
        {
            int nPort;
            
            if (ReadConfig("ussServerPort", out nPort) == false)
                return;

            m_ussServer = new USSServer(nPort, this, m_logger, m_dbMgr);
            m_ussServer.BeginServer();
        }

        private bool Init()
        {
            int nSiteID, nDBType;

            if (ReadConfig("siteid", out nSiteID) == false)
                return false;

            if (ReadConfig("dbtype", out nDBType) == false)
                return false;

            string strDBName = System.Configuration.ConfigurationManager.AppSettings["dbname"].ToString().Trim();

            if (strDBName.Length == 0)
                return false;

            string strWebServerURL, strIP;

            if (ReadURL(out strWebServerURL, out strIP) == false)
                return false;

            m_dbMgr = new WebDBManager(strDBName, nSiteID);
            m_dbMgr.WebServerURL = strWebServerURL;
            m_dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;

            int nPort;
            string strServerIP = System.Configuration.ConfigurationManager.AppSettings["fireServerIP"].ToString().Trim();

            if (ReadConfig("fireServerPort", out nPort) == false)
                return false;

            m_provider = new ClientProvider(this, strServerIP, nPort);

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

        public void OnMessage(int header, byte[] messages, object postMan)
        {
            if (postMan != null && postMan is PostMan)
            {
                ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

                RecvLog(header, messages);

                if (header == SOPWebServer.Header.CLOSE_CONNECTION)
                {
                    ((PostMan)postMan).IsConnected = false;
                }
            }
        }

        private void ConnectionThread()
        {
            while (m_shutdownThread == false)
            {
                #region SOPWebServer
                foreach (PostMan postMan in m_postManList)
                {
                    if (postMan.IsConnected == false)
                    {
                        int nPort = ReadServerPort();

                        if (postMan.Port != nPort)
                            SetPostBox(postMan, nPort);

                        if (postMan.PostBox != null)
                        {
                            if (postMan.PostBox.Connect(postMan.ClientType, postMan.ClientSubType))
                            {
                                postMan.IsConnected = true;
                            }
                        }
                    }
                    else
                    {
                        TimeSpan span = DateTime.Now - postMan.LastSendMessageTime;

                        // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                        if (span.TotalSeconds > 3.0)
                        {
                            // 접속이 유지되고 있는지 확인한다.
                            postMan.SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                        }
                    }
                }

                Thread.Sleep(500);
                #endregion
            }
        }

        private void ConnectionThreadUSS()
        {
            while (m_shutdownThread == false)
            {               
                #region USS
                if (m_provider.IsConnected)
                {
                    if (m_provider.PingCount > 5)
                    {
                        m_provider.PingCount = 0;
                        m_provider.Close();
                    }
                    // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                    else if (m_provider.IsReadingProcess)
                        m_provider.SendData(libUSS.Header.I_AM_HERE);
                    else
                        m_provider.PingCount++;
                }

                if (!m_provider.IsConnected)
                {
                    m_provider.Connect();

                    if (m_provider.IsConnected)
                    {
                        if (m_owner != null)
                            m_owner.OnConnect(m_provider.ServerIP, true);

                        SendEventType();
                    }
                }

                Thread.Sleep(500);
                #endregion
            }
        }

        private int ReadServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        private void SetPostBox(PostMan postMan, int nPort)
        {
            if (nPort > 0)
            {
                PostBox postBox = new PostBox();
                postBox.WebServerURL = m_dbMgr.WebServerURL;
                postBox.PostMan = postMan;
                postMan.PostBox = postBox;

                postMan.Port = nPort;
                postBox.Port = nPort;
            }
        }

        private bool SendMessage(int header, byte[] messages, PostMan postMan)
        {
            if (postMan.IsConnected)
            {
                SendLog(header, messages);
                return postMan.SendMessage(header, messages);
            }

            return false;
        }

        public void RecvLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "RecvMessage");
        }

        private void SendLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "SendMessage");
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

        public void WriteLog(string strLog)
        {
            if (m_logger != null)
                m_logger.Write(strLog);
        }

        public void Close()
        {
            foreach (PostMan postMan in m_postManList)
            {
                if (postMan.IsConnected)
                {
                    // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                    // 실패하더라도 상관없다.
                    bool closeConnection;
                    postMan.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                    postMan.IsConnected = false;
                }
            }
            
            m_shutdownThread = true;

            if (m_ussServer != null)
                m_ussServer.StopServer();

            if (m_accessCtrl != null)
                m_accessCtrl.Close();
        }

        public void SendSimulationWindSpeed(int nSensorID, float fWindSpeed)
        {
            if (m_ussServer != null)
                m_ussServer.SendSimulationWindSpeed(nSensorID, fWindSpeed);
        }

        public void SendSimulationEarthquake(int nIntensity)
        {
            if (m_ussServer != null)
                m_ussServer.SendSimulationEarthquake(nIntensity);
        }

        public void StartReadEvent()
        {
            if (m_ussServer != null)
                m_ussServer.StartReadEvent();
        }

        public void StopReadEvent()
        {
            if (m_ussServer != null)
                m_ussServer.StopReadEvent();
        }

        #region USS통신
        public void OnDropConnection(string strServerIP)
        {
            if (m_owner != null)
                m_owner.OnDropConnection(strServerIP, true);
        }

        public int SendUSS(byte[] bytes)
        {
            int nResult = m_provider.Send(bytes, 0, bytes.Length);

            if (nResult > 0)
            {
                if (bytes[0] != libUSS.Header.I_AM_HERE && bytes[0] != libUSS.Header.ARE_YOU_THERE)
                {
                    string strLog = string.Format("SendMessage[USS] : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    WriteLog(strLog + strBytes);
                }
            }
            return nResult;
        }

        public void RecvUSSLog(byte[] bytes)
        {
            if (bytes != null && bytes.Count() > 0)
            {
                if (bytes[0] != libUSS.Header.I_AM_HERE && bytes[0] != libUSS.Header.ARE_YOU_THERE)
                {
                    string strLog = string.Format("RecvMessage[USS] : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    WriteLog(strLog + strBytes);
                }
            }
        }

        private void SendEventType()
        {
            byte[] eventTypes = GetEventArray();

            if (eventTypes != null)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add((short)eventTypes.Length);
                arrDatas.Add(eventTypes);

                byte[] bytes = libUSS.BinaryHelper.MakeBytes(libUSS.Header.REQUEST_SELECT_EVENT_TYPE, arrDatas);
                SendUSS(bytes);
            }
        }

        private byte[] GetEventArray()
        {
            List<byte> eventList = new List<byte>();
            eventList.Add(libUSS.EventType.Fire);

            int nEventCount = eventList.Count;

            if (nEventCount == 0)
                return null;

            byte[] bytes = new byte[nEventCount];

            for (int i = 0; i < nEventCount; i++)
            {
                bytes[i] = eventList[i];
            }

            return bytes;
        }

        public void OnFireSignal(bool on, int nSensorTagID, DateTime timeStamp)
        {
            Data.SensorTag sensorTag;

            if (m_dicSensorTags.TryGetValue(nSensorTagID, out sensorTag))
            {
                ArrayList arrDatas = new ArrayList();
                int nSensorData = on ? 1 : 0;

                arrDatas.Add((int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR);
                arrDatas.Add(sensorTag.ID);
                arrDatas.Add(sensorTag.SensorZoneID);
                arrDatas.Add(nSensorData);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes, m_postManFire);
            }
        }

        public void OnEarthquakeSignal(int nIntensity, int nSensorZoneID, DateTime timeStamp)
        {
            // 이전에 전송한 값과 같으면 다시 보내지 않는다.
            //if (m_postManEarthquake.PrevIntensity == nIntensity)
            //    return;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(-1.0f);
            arrDatas.Add(nIntensity);
            arrDatas.Add("");
            arrDatas.Add(timeStamp.ToBinary());
            arrDatas.Add(true);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            if (SendMessage(SOPWebServer.Header.EARTHQUAKE_SENSOR_DETECT, bytes, m_postManEarthquake))
                m_postManEarthquake.PrevIntensity = nIntensity;
        }

        public void OnStrongWindSignal(float fWindSpeed, int nSensorZoneID, DateTime timeStamp)
        {
            // 이전에 전송한 값과 같으면 다시 보내지 않는다.
            //if (m_postManEtc.GetPrevDataf(nSensorZoneID) == fWindSpeed)
            //    return;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add((int)IFacility.FacilityType.STRONG_WIND);
            arrDatas.Add(-1);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(fWindSpeed);
            arrDatas.Add(timeStamp.ToBinary());

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            if (SendMessage(SOPWebServer.Header.ETC_SENSOR_DETECT, bytes, m_postManEtc))
                m_postManEtc.SetPrevDataf(nSensorZoneID, fWindSpeed);

        }

        public void OnAccept(TcpLib2.ConnectionState state)
        {
            if (m_uiOwner != null)
                m_uiOwner.OnAddClient(state);
        }

        public void OnDropConnection(TcpLib2.ConnectionState state)
        {
            if (m_uiOwner != null)
                m_uiOwner.OnRemoveClient(state);
        }

        public void SetClientInfo(TcpLib2.ConnectionState state, List<byte> eventTypes)
        {
            if (m_uiOwner != null)
                m_uiOwner.SetClientInfo(state, eventTypes);
        }
        #endregion


    }

    public interface IUIOwner
    {
        void OnAddClient(TcpLib2.ConnectionState state);
        void OnRemoveClient(TcpLib2.ConnectionState state);
        void SetClientInfo(TcpLib2.ConnectionState state, List<byte> eventTypes);
    }
}
