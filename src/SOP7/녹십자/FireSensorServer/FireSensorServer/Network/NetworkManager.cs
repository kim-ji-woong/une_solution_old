using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using dnsTcpLib2;
using dnsDBUtil;
using SDMS.Model.Sensor;
using System.Threading;

namespace FireSensorServer.Network
{
    public class NetworkManager
    {
        // 전체복구, 신호탐지, 신호복구, 장애발생, 장애복구
        public enum EventType { Unknown = 0, ClearAll, DetectSensor, ClearSensor, DetectError, ClearError };
        public enum ClientType { Siemens = 0, Johnson };

        private Logger m_logger = null;
        // 지멘스 Server 모드시 사용할 데이터
        private TcpServer m_serverSiemens = null;
        private ServerServiceProvider m_siemensServerProvider = null;
        private WebDBManager m_dbMgr = null;
        private Data.SensorManager m_sensorManager = null;

        // 지멘스 Client 모드시 사용할 데이터
        private SiemensClientProvider m_siemensProvider = null;
        private bool m_runThread = false;
        private string m_strSiemensServerIP = "";
        private int m_nSiemensServerPort = 0;

        // 동방 센서 수신반과의 통신을 위한 Manager
        private JohnsonManager m_johnsonManager = null;
        public JohnsonManager JohnsonManager
        {
            get { return m_johnsonManager; }
        }

        // SOPWebServer와의 통신
        private NetworkWebClient m_webClient = null;

        // ClientType별 SensorServerID
        private Dictionary<ClientType, int> m_dicSensorServerIDs = new Dictionary<ClientType, int>();

        private static NetworkManager m_instance = null;

        // Delegate 호출을 위한 Form
        private IFormMain m_frmDelegate = null;

        public IFormMain FormDelegate
        {
            get { return m_frmDelegate; }
            set { m_frmDelegate = value; }
        }

        public static NetworkManager Instance
        {
            get { return m_instance; }
        }

        public NetworkManager(IFormMain frm)
        {
            m_instance = this;
            m_frmDelegate = frm;
            ReadSettings();
        }

        private void ReadSettings()
        {
            int? siteID = ReadInt("siteid");

            if (siteID == null)
                return;

            string strWebServerURL = ReadString("webserverURL");

            if (strWebServerURL == null)
                return;

            string strDBName = ReadString("dbName");

            if (strDBName == null)
                return;

            int? dbType = ReadInt("dbType");

            if (dbType == null)
                return;

            string strLogFolder = ReadString("logFolder");

            if (strLogFolder == null)
                return;

            string strLogFile = ReadString("logFile");

            if (strLogFile == null)
                return;

            int? siemensPort = ReadInt("siemensPort");

            if (siemensPort == null)
                return;

            int? siemensSensorServerID = ReadInt("siemensSensorServer");

            if (siemensSensorServerID == null)
                return;

            int? johnsonSensorServerID = ReadInt("johnsonSensorServer");

            if (johnsonSensorServerID == null)
                return;

            string strJohnsonServerIP = ReadString("muxServerIP");

            if (strJohnsonServerIP == null)
                return;

            int? johnsonServerPort = ReadInt("muxServerPort");

            if (johnsonServerPort == null)
                return;

            int? muxType = ReadInt("NMuxType");

            if (muxType == null)
                return;

            m_dicSensorServerIDs[ClientType.Siemens] = (int)siemensSensorServerID;
            m_dicSensorServerIDs[ClientType.Johnson] = (int)johnsonSensorServerID;

            m_dbMgr = new WebDBManager(strDBName, (int)dbType, (int)siteID, strWebServerURL);
            m_logger = new Logger(strLogFolder, strLogFile);

            m_webClient = new NetworkWebClient();

            m_sensorManager = new Data.SensorManager(m_dbMgr);

            string strSiemensMode = ReadString("siemensMode");

            if (string.Compare(strSiemensMode, "server", true) == 0)
                RunSiemensServer((int)siemensPort);
            else
                RunSiemensClient((int)siemensPort);

            RunJohnsonManager(strJohnsonServerIP, (int)johnsonServerPort, (int)muxType);
        }

        private void RunSiemensServer(int nPort)
        {
            if (nPort > 0)
            {
                string strSiemensIP = ReadString("siemensIP");

                if (strSiemensIP == null)
                    return;

                m_siemensServerProvider = new ServerServiceProvider(m_logger);
                m_serverSiemens = new TcpServer(m_siemensServerProvider, nPort);
                m_serverSiemens.Start();
            }
        }

        private void RunSiemensClient(int nPort)
        {
            string strSiemensIP = ReadString("siemensIP");

            if (strSiemensIP == null)
                return;

            m_strSiemensServerIP = strSiemensIP;
            m_nSiemensServerPort = nPort;
            m_siemensProvider = new SiemensClientProvider(this, m_logger);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
        }

        private void RunJohnsonManager(string strIP, int nPort, int muxType)
        {
            if (nPort > 0)
            {
                m_johnsonManager = new JohnsonManager(strIP, nPort.ToString(), muxType.ToString(), m_logger);
                m_johnsonManager.Start();
            }
        }

        private void ConnectionThread()
        {
            m_runThread = true;
            byte[] pingBytes = new byte[] { 0x00 };

            while (m_runThread)
            {
                try
                {
                    if (m_siemensProvider.IsConnected)
                    {
                        // 10초 이상 아무 신호를 못받으면 접속이 끊어진 것으로 간주한다.
                        if (m_siemensProvider.PingCount > 10)
                        {
                            // 아무 신호나 보내본다.
                            int nResult = m_siemensProvider.Send(pingBytes, 0, 1);

                            if (nResult < 0)
                            {
                                lock (m_siemensProvider)
                                {
                                    m_siemensProvider.PingCount = 0;
                                    m_siemensProvider.Close();

                                    if (m_siemensProvider.Client.Client != null)
                                    {
                                        if (m_siemensProvider.Client.Connected)
                                            m_siemensProvider.Client.Close();

                                        System.Diagnostics.Trace.WriteLine("Close Provider : " + !m_siemensProvider.Client.Connected);
                                    }
                                }
                            }
                        }
                        else
                            m_siemensProvider.PingCount++;
                    }

                    if (!m_siemensProvider.IsConnected)
                    {
                        lock (m_siemensProvider)
                        {
                            if (m_nSiemensServerPort > 0)
                            {
                                m_siemensProvider.Connect(m_strSiemensServerIP, m_nSiemensServerPort);

                                if (m_siemensProvider.IsConnected)
                                    m_logger.Write("[Success connect to " + NetworkManager.GetClientTypeString(NetworkManager.ClientType.Siemens) + " Server] " + m_strSiemensServerIP + ":" + m_nSiemensServerPort);
                            }
                        }
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    m_logger.Write("ConnectionThread Error : " + e.Message);
                    System.Diagnostics.Trace.WriteLine("ConnectionThread Error : " + e.Message);
                }
            }
        }

        private int? ReadInt(string strSection)
        {
            string strValue = ConfigurationManager.AppSettings.Get(strSection).Trim();
            int data;

            if (int.TryParse(strValue, out data))
                return data;

            return null;
        }

        private string ReadString(string strSection)
        {
            string strValue = ConfigurationManager.AppSettings.Get(strSection).Trim();
            return strValue;
        }

        public void AddClient(ConnectionState state, ClientType clientType)
        {
            if (m_frmDelegate == null)
                return;
            else
            {
                m_frmDelegate.GetControl().Invoke((MethodInvoker)delegate
                {
                    m_frmDelegate.AddClient(state, GetClientTypeString(clientType));
                });
            }
        }

        public void RemoveClient(ConnectionState state)
        {
            Client.ClientData data = (Client.ClientData)state.Tag;

            if (m_frmDelegate == null)
            {
                return;
            }
            else
            {
                m_frmDelegate.GetControl().Invoke((MethodInvoker)delegate
                {
                    m_frmDelegate.RemoveClient(state);
                });
            }
        }

        public void SendSensorData(int tagNo, EventType eventType, ClientType clientType, DateTime dtEvent, string strPosition, string strMessage, string strTagNo)
        {
            int nSensorServerID;

            if (m_dicSensorServerIDs.TryGetValue(clientType, out nSensorServerID) == false)
                return;

            if (eventType == EventType.ClearAll)
            {
                m_webClient.SendAllClearAsync();
                TraceLog(null, eventType, GetClientTypeString(clientType), dtEvent, strPosition, strMessage, strTagNo);
                return;
            }

            TagInfo tag = m_sensorManager.GetTagInfo(nSensorServerID, tagNo);

            if (tag != null)
            {
                /*if (eventType == EventType.ClearAll)
                    m_webClient.SendAllClearAsync();
                else */if (eventType == EventType.DetectSensor)
                    m_webClient.SendSensorDataAsync(tag.ID, tag.SensorZoneID, 1);
                else if (eventType == EventType.ClearSensor)
                    m_webClient.SendSensorDataAsync(tag.ID, tag.SensorZoneID, 0);

                TraceLog(tag, eventType, GetClientTypeString(clientType), dtEvent, strPosition, strMessage, strTagNo);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("[" + GetClientTypeString(clientType) + "] Unknown TagNo : " + strTagNo);
            }
        }

        private void TraceLog(TagInfo tag, EventType eventType, string strClientType, DateTime dtEvent, string strPosition, string strMessage, string strTagNo)
        {
            string strLog = "";

            if (eventType == EventType.ClearAll)
            {
                strLog = string.Format("[{0}] ClearAll {1} : {2}", strClientType, strPosition, strMessage);
            }
            else if (eventType == EventType.DetectSensor)
            {
                strLog = string.Format("[{0}] DetectSensor {1}({2}), {3} : {4}", strClientType, tag.ID, strTagNo, strPosition, strMessage);
            }
            else if (eventType == EventType.ClearSensor)
            {
                strLog = string.Format("[{0}] ClearSensor {1}({2}), {3} : {4}", strClientType, tag.ID, strTagNo, strPosition, strMessage);
            }
            else if (eventType == EventType.DetectError)
            {
                strLog = string.Format("[{0}] DetectError {1}({2}), {3} : {4}", strClientType, tag.ID, strTagNo, strPosition, strMessage);
            }
            else if (eventType == EventType.ClearError)
            {
                strLog = string.Format("[{0}] ClearError {1}({2}), {3} : {4}", strClientType, tag.ID, strTagNo, strPosition, strMessage);
            }
            else
            {
                strLog = string.Format("[{0}] Unknown Data {1}({2}), {3} : {4}", strClientType, tag.ID, strTagNo, strPosition, strMessage);
            }

            System.Diagnostics.Trace.WriteLine(strLog);

            if (m_logger != null)
                m_logger.Write(strLog);
        }

        public static string GetClientTypeString(ClientType type)
        {
            if (type == ClientType.Siemens)
                return "Siemens";
            else if (type == ClientType.Johnson)
                return "Johnson";

            return "";
        }

        public int GetSensorServerID(ClientType type)
        {
            int id;

            if (m_dicSensorServerIDs.TryGetValue(type, out id))
                return id;

            return -1;
        }

        public void Close()
        {
            m_runThread = false;

            if (m_johnsonManager != null)
                m_johnsonManager.Stop();
        }
    }

    public interface IFormMain
    {
        void AddClient(ConnectionState state, string strClientType);
        void RemoveClient(ConnectionState state);
        Control GetControl();
    }
}
