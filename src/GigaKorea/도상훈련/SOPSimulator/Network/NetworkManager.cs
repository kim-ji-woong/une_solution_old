using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using SOPMonitoringSystem;
using TcpLib2;
using FireSimulator;
using DBUtility;

namespace SOPMonitoringSystem
{
    public class NetworkManager
    {
        private ClientProvider m_provider = null;
        private ClientProviderInternal m_providerInternal = null;

        private int m_nPort = 6000;
        private string m_strServerAddr = "127.0.0.1";
        //private bool m_isConnected = false;
        private bool shutdownThread = false;

        // FireDetectSignal Array
        private ArrayList m_arrDetectSignals = new ArrayList();

		private static NetworkManager m_manager = null;
		public static NetworkManager Instance
		{
			get
			{
				if (m_manager == null)
					m_manager = new NetworkManager();
				return m_manager;
			}
		}

        public ClientProvider ClientProvier
        {
            get { return m_provider; }
        }

        public ClientProviderInternal ClientProviderInternal
        {
            get { return m_providerInternal; }
        }

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

		private void WriteLog(object str)
		{
		}

		private void WriteLineLog(object str)
		{
		}

		private void InitLog()
		{
		}

		private bool m_bIsLogOpened = false;
		public bool IsLogOpened
		{
			get { return m_bIsLogOpened; }
			set { m_bIsLogOpened = value; }
		}

        public void RecvLog(byte[] bytes)
        {
			if (!IsLogOpened)
                return;

            if (bytes[0] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
            {
                string strLog = string.Format("RecvMessage : Header({0}), Length({1}), SOPSimulator", (int)bytes[0], (int)bytes.Length);
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

        public int Send(byte[] bytes, ClientServiceProvider provider, string strTag = "")
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
                    string strLog = string.Format("SendMessage : Header({0}), Length({1}), SOPSimulator", (int)bytes[0], (int)bytes.Length);

                    if (strTag.Length > 0)
                        strLog += " " + strTag;


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
        protected NetworkManager()
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            InitLog();

            Utility util = new Utility();
            string strServerURL = util.getinivalue("Server Connection Info", "server_addr").Trim();

            if (strServerURL.Length > 0)
            {
                int nIndex = strServerURL.LastIndexOf(':');

                if (nIndex > 0)
                {
                    string strPort = strServerURL.Substring(nIndex + 1).Trim();
                    int.TryParse(strPort, out m_nPort);
                    m_strServerAddr = strServerURL.Substring(0, nIndex).Trim();
                }
                else
                    m_strServerAddr = strServerURL;
            }

            m_provider = new ClientProvider(this);
            m_providerInternal = new ClientProviderInternal(this);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
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
                                System.Diagnostics.Trace.WriteLine("PING COUNT EXCEPTION");
                                m_provider.Close();
                            }
                            catch (System.Exception)
                            {
                            }
                        }
                        else
                        {
                            m_provider.SendData(TCP_ID.ARE_YOU_THERE);
                            m_provider.PingCount++;
                        }
                    }

                    if (!m_provider.IsConnected)
                    {
                        // m_strServerAddr가 ""이면 LockConnection()이 호출된 상태다.
                        if (m_strServerAddr.Length > 0)
                        {
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

        public void SendRunSOP(int nSensorHistoryID, int nActionStepHistoryID)
        {
        }

        public void SendChangedWorkingMemberData()
        {
        }

        public void SendIgnoreSOP(int nSensorHistoryID)
        {
        }

        public void AddDetectSignal(FireDetectSignal signal)
        {
            if (!m_arrDetectSignals.Contains(signal))
                m_arrDetectSignals.Add(signal);
        }

        public void RemoveDetectSignal(FireDetectSignal signal)
        {
            m_arrDetectSignals.Remove(signal);
        }

        public FireDetectSignal FindDetectSignal(int nSensorHistoryID)
        {
            foreach (FireDetectSignal signal in m_arrDetectSignals)
            {
                if (signal.SensorHistoryID == nSensorHistoryID)
                    return signal;
            }

            return null;
        }

        public void RemoveSensorHistory(int nSensorHistoryID)
        {
            foreach (FireDetectSignal signal in m_arrDetectSignals)
            {
                if (signal.SensorHistoryID == nSensorHistoryID)
                {
                    m_arrDetectSignals.Remove(signal);
                    break;
                }
            }
        }

        // 현재 진행중인 화재 상황에 대하여 SOP List를 팝업시킨다.
        public void ShowDetectSignal()
        {
            if (m_arrDetectSignals == null || m_arrDetectSignals.Count == 0)
                return;

            if (FormSOP.Instance.UsePopupSensorOn)
            {
                FireDetectSignal signal = (FireDetectSignal)m_arrDetectSignals[0];
                SOPMonitoringSystem.Popup.PopupSensorOn.PopUpForm(FormSOP.Instance.DBManager, signal, FormSOP.Instance.HasControl);
            }
        }

        public void SendControl(string strUserID, string strUserName, string strUserIP)
        {
        }

        public void SendRejectRequestControl(string strUserID, string strUserName, string strUserIP)
        {
        }

        public void SendSelectMission(int nActionStepHistory, int nRealMode, int nCompHistoryID, string strRowIndex)
		{
		}

        public void SendChangedConfig(byte byteClientType, string strPropertyName, string strPropertyValue)
        {
        }

        public void ReleaseConnection()
        {
        }
    }

    public class FireDetectSignal
    {
        // SensorZone이 아닌 개별 Sensor의 ID
        private int m_nSensorID = -1;
        private int m_nSensorHistoryID = -1;
        private int m_nEquipZoneID = -1;
        private DateTime m_detectTime;
        private float x = 0.0f;
        private float y = 0.0f;
        private float z = 0.0f;
        private int m_nActionStepHistoryID = -1;
        private int m_nActionStepID = -1;
        private string m_szPositionName = "";


		private bool m_bRealMode = true;
		public bool RealMode
		{
			get { return m_bRealMode; }
			set { m_bRealMode = value; }
		}
        public int SensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }

        public int SensorHistoryID
        {
            get { return m_nSensorHistoryID; }
            set { m_nSensorHistoryID = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public DateTime DetectTime
        {
            get { return m_detectTime; }
            set { m_detectTime = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }
        
        public string PositionName
        {
            get { return m_szPositionName; }
            set { m_szPositionName = value; }
        }
        
        public FireDetectSignal()
        {
        }

        public FireDetectSignal(int nSensorID, int nSensorHistoryID, int nEquipZoneID, DateTime detectTime, float x, float y, float z)
        {
            m_nSensorID = nSensorID;
            m_nSensorHistoryID = nSensorHistoryID;
            m_nEquipZoneID = nEquipZoneID;
            m_detectTime = detectTime;            
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}

//namespace SDMS
//{
//    public class ConnectionLogEx : ConnectionLog
//    {
//        private log4net.ILog logger = null;

//        public static bool MakeInstance()
//        {
//            if (m_instance == null)
//                m_instance = new ConnectionLogEx();

//            ConnectionLogEx instance = (ConnectionLogEx)m_instance;
//            instance.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
//            instance.m_isOpened = true;
//            return instance.m_isOpened;
//        }

//        public override bool Write(object str, bool writeTime = true)
//        {
//            if (logger != null)
//                logger.DebugFormat("{0}", str);

//            return true;
//        }

//        public override bool WriteLine(object str, bool writeTime = true)
//        {
//            if (logger != null)
//                logger.Debug(str);

//            return true;
//        }
//    }
//}
