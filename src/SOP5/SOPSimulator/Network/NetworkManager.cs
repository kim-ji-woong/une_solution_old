using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using SOPMonitoringSystem;
using TcpLib2;
using SDMS;

namespace SOPMonitoringSystem
{
    public class NetworkManager
    {
        private ClientProvider m_provider = null;
        private ClientProviderInternal m_providerInternal = null;

        private int m_nPort = -1;
        private string m_strServerAddr = "";
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

            //string strPort = FormSOP.Instance.DBManager.LoadIni("sdms_port", "Server Connection Info");
           
            string strServerURL = DBUtility.RegUtil.ReadRegValue("Server Connection Info", "webserver_url", m_nSiteID);
            if( strServerURL == null || strServerURL == "")
                strServerURL = FormSOP.Instance.DBManager.WebServerURL;

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

            System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);

            m_provider = new ClientProvider(this);
            m_providerInternal = new ClientProviderInternal(this);

            if (FormSOP.Instance.SimulationMode)
                m_strServerAddr = "127.0.0.1";
            else
                m_strServerAddr = addr[0].ToString();

            //m_strServerAddr = "127.0.0.1";

            // UI에서 모든 준비가 끝나면 접속 시도하도록 한다.
            LockConnection();

            Thread t;
            t = new Thread(ConnectionThread);
            t.Name = "ConnectionThread";
            t.Start();

            // 시간이 경과한 로그 삭제
            t = new Thread(DeleteLog);
            t.Name = "LogDelete";
            t.Start();
        }

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            DateTime dtLog = new DateTime(nYear, nMonth, nDay);
            TimeSpan span = dtNow - dtLog;
            return span.TotalDays > 30.0;           
        }

        // 1달이 경과한 통신로그 삭제
        private void DeleteLog()
        {
            try
            {
                string strPath = System.Windows.Forms.Application.ExecutablePath;
                string szParentPath = System.IO.Path.GetDirectoryName(strPath);

                string[] arrFiles = System.IO.Directory.GetFiles(szParentPath + "\\logs");

                string strKey = "SOPMonitoringSystem.log-";
                int len = strKey.Length;

                DateTime dtNow = DateTime.Now;
                int nYear, nMonth, nDay;

                foreach (string strFile in arrFiles)
                {
                    int nIndex = strFile.IndexOf(strKey);

                    if (nIndex < 0)
                        continue;

                    string strDate = strFile.Substring(nIndex + len);

                    int nIndex1 = strDate.IndexOf('-');
                    int nIndex2 = strDate.LastIndexOf('-');

                    if (nIndex1 < 0 || nIndex2 < 0 || nIndex1 == nIndex2)
                        continue;

                    string strYear = strDate.Substring(0, nIndex1);
                    string strMonth = strDate.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                    string strDay = strDate.Substring(nIndex2 + 1);

                    if (!int.TryParse(strYear, out nYear))
                        continue;
                    if (!int.TryParse(strMonth, out nMonth))
                        continue;
                    if (!int.TryParse(strDay, out nDay))
                        continue;

                    if (IsPassedTime(dtNow, nYear, nMonth, nDay))
                        System.IO.File.Delete(strFile);
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {
            }
        }

        private int GetServerPort()
        {
            WebDBManager dbMgr = FormSOP.Instance.DBManager;

            string strSQL = "Select Port from SDMSServerPort where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            DateTime dtPrev = DateTime.Now;

            while (!shutdownThread)
            {
                lock (this)
                {
                    //if (m_isConnected)
                    if (m_provider.IsConnected)
                    {
                        if (m_provider.PingCount > 5)
                        {
                            //m_isConnected = false;
                            m_provider.PingCount = 0;

							try
							{
                                ConnectionLogEx.Instance.WriteLine("PING COUNT EXCEPTION");
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

                        // SOPSImulator가 Hide 상태일때는 통신을 끊는다.
                        if (FormSOP.Instance.OnlySDMS)
                            m_provider.Close();
                    }

                    //if (!m_isConnected)
                    if (!m_provider.IsConnected)
                    {
                        // m_strServerAddr가 ""이면 LockConnection()이 호출된 상태다.
                        if (m_strServerAddr.Length > 0)
                        {
                            m_nPort = GetServerPort();
                            try
                            {
                                if (m_nPort > 0 && !FormSOP.Instance.OnlySDMS)
                                {
                                    /*m_isConnected = */
                                    m_provider.Connect(m_strServerAddr, m_nPort);
                                }
                            }
                            catch (System.Exception)
                            {

                            }
                        }
                    }

                    Thread.Sleep(500);

                    if (m_providerInternal.IsConnected)
                    {
                        if (m_providerInternal.PingCount > 5)
                        {
                            m_providerInternal.PingCount = 0;

                            try
                            {
                                ConnectionLogEx.Instance.WriteLine("PING COUNT EXCEPTION");
                                m_providerInternal.Close();
                            }
                            catch (System.Exception)
                            {

                            }

                        }
                        // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                        else if (m_providerInternal.IsReadingProcess)
                            m_providerInternal.SendData(TCP_ID.I_AM_HERE);
                        else
                            m_providerInternal.PingCount++;
                    }

                    if (!m_providerInternal.IsConnected)
                    {
                        int nPort = IntegratedManagement3.InternalMessage.GetInternalServerPort(FormSOP.Instance.DBManager, m_nSiteID);

                        try
                        {
                            if (nPort > 0)
                            {
                                m_providerInternal.Connect("127.0.0.1", nPort);
                            }
                        }
                        catch (System.Exception)
                        {

                        }
                    }
                }
                Thread.Sleep(500);

                // 날짜가 경과하면 한달이 지난 로그를 삭제한다.
                if (DateTime.Now.Day != dtPrev.Day)
                    DeleteLog();

                dtPrev = DateTime.Now;
            }
        }

        public void OnDropConnection()
        {
            lock (this)
            {
                //m_isConnected = false;
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
            byte[] actionStepHistoryBytes = ClientProvider.MakeBytes(nActionStepHistoryID.ToString());
            byte[] sensorHistoryBytes = ClientProvider.MakeBytes(nSensorHistoryID);

            byte[] bytes = new byte[6 + actionStepHistoryBytes.Length + sensorHistoryBytes.Length];

			// SET HEADER
			byte[] nHeader = BitConverter.GetBytes((short)TCP_ID.RUN_SOP);
			bytes[0] = nHeader[0];
			bytes[1] = nHeader[1];

			// SET DATA COUNT
			byte[] nCount = BitConverter.GetBytes(2);
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];
			
            System.Buffer.BlockCopy(sensorHistoryBytes, 0, bytes, 6 , sensorHistoryBytes.Length);
            System.Buffer.BlockCopy(actionStepHistoryBytes, 0, bytes, 6 + sensorHistoryBytes.Length, actionStepHistoryBytes.Length);
						
            Send(bytes, m_provider);   
        }

        public void SendChangedWorkingMemberData()
        {
            m_provider.SendData(TCP_ID.CHAGNE_WORK_MEMBER);
        }

        public void SendIgnoreSOP(int nSensorHistoryID)
        {

            byte[] sensorHistoryBytes = ClientProvider.MakeBytes(nSensorHistoryID);


			int nLength = 6 + sensorHistoryBytes.Length;// +sensorIdBytes.Length + zoneIdBytes.Length;
			byte[] bytes = new byte[nLength];

			// SET HEADER
			byte[] nHeader = BitConverter.GetBytes((short)TCP_ID.IGNORE_SOP);
			bytes[0] = nHeader[0];
			bytes[1] = nHeader[1];

			// SET DATA COUNT
			byte[] nCount = BitConverter.GetBytes(1);
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			int nOffset = 6;
			System.Buffer.BlockCopy(sensorHistoryBytes, 0, bytes, nOffset, sensorHistoryBytes.Length);

			Send(bytes, m_provider);
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
            byte[] userIDBytes = ClientProvider.MakeBytes(strUserID);
            byte[] userNameBytes = ClientProvider.MakeBytes(strUserName);
            byte[] ipBytes = ClientProvider.MakeBytes(strUserIP);

            int nChunkCount = 3;
            byte[] chunkCountBytes = BitConverter.GetBytes(nChunkCount);

            int nLen = chunkCountBytes.Length + userIDBytes.Length + userNameBytes.Length + ipBytes.Length + 2;

            byte[] bytes = new byte[nLen];

            bytes[0] = TCP_ID.GIVE_CONTROL;
            bytes[1] = 0;

            int nIndex = 2;

            ClientProvider.CopyBlock(chunkCountBytes, 0, bytes, ref nIndex, chunkCountBytes.Length);
            ClientProvider.CopyBlock(userIDBytes, 0, bytes, ref nIndex, userIDBytes.Length);
            ClientProvider.CopyBlock(userNameBytes, 0, bytes, ref nIndex, userNameBytes.Length);
            ClientProvider.CopyBlock(ipBytes, 0, bytes, ref nIndex, ipBytes.Length);

            Send(bytes, m_provider);
        }

        public void SendRejectRequestControl(string strUserID, string strUserName, string strUserIP)
        {
            byte[] userIDBytes = ClientProvider.MakeBytes(strUserID);
            byte[] userNameBytes = ClientProvider.MakeBytes(strUserName);
            byte[] ipBytes = ClientProvider.MakeBytes(strUserIP);

            int nChunkCount = 3;
            byte[] chunkCountBytes = BitConverter.GetBytes(nChunkCount);

            int nLen = chunkCountBytes.Length + userIDBytes.Length + userNameBytes.Length + ipBytes.Length + 2;

            byte[] bytes = new byte[nLen];

            bytes[0] = TCP_ID.REJECT_REQUEST_CONTROL;
            bytes[1] = 0;

            int nIndex = 2;

            ClientProvider.CopyBlock(chunkCountBytes, 0, bytes, ref nIndex, chunkCountBytes.Length);
            ClientProvider.CopyBlock(userIDBytes, 0, bytes, ref nIndex, userIDBytes.Length);
            ClientProvider.CopyBlock(userNameBytes, 0, bytes, ref nIndex, userNameBytes.Length);
            ClientProvider.CopyBlock(ipBytes, 0, bytes, ref nIndex, ipBytes.Length);

            Send(bytes, m_provider);
        }

        public void SendSelectMission(int nActionStepHistory, int nRealMode, int nCompHistoryID, string strRowIndex)
		{
			byte[] userIDBytes = ClientProvider.MakeBytes(nActionStepHistory);
			byte[] realMode = ClientProvider.MakeBytes(nRealMode);
			byte[] userNameBytes = ClientProvider.MakeBytes(nCompHistoryID);
            byte[] ipBytes = ClientProvider.MakeBytes(strRowIndex);

			int nChunkCount = 4;
			byte[] chunkCountBytes = BitConverter.GetBytes(nChunkCount);

			int nLen = chunkCountBytes.Length + realMode.Length + userIDBytes.Length + userNameBytes.Length + ipBytes.Length + 2;

			byte[] bytes = new byte[nLen];

			bytes[0] = TCP_ID.SOP_SELECT_MISSION;
			bytes[1] = 0;

			int nIndex = 2;

			ClientProvider.CopyBlock(chunkCountBytes, 0, bytes, ref nIndex, chunkCountBytes.Length);
			ClientProvider.CopyBlock(userIDBytes, 0, bytes, ref nIndex, userIDBytes.Length);
			ClientProvider.CopyBlock(realMode, 0, bytes, ref nIndex, realMode.Length);
			ClientProvider.CopyBlock(userNameBytes, 0, bytes, ref nIndex, userNameBytes.Length);
			ClientProvider.CopyBlock(ipBytes, 0, bytes, ref nIndex, ipBytes.Length);

			Send(bytes, m_provider);

            // Remember Selected Mission
            byte[] bytesRemember = new byte[nLen];

            bytesRemember[0] = TCP_ID.SOP_CURRENT_SELECT_MISSION;
            bytesRemember[1] = 0;

            int nIndexRemember = 2;

            ClientProvider.CopyBlock(chunkCountBytes, 0, bytesRemember, ref nIndexRemember, chunkCountBytes.Length);
            ClientProvider.CopyBlock(userIDBytes, 0, bytesRemember, ref nIndexRemember, userIDBytes.Length);
            ClientProvider.CopyBlock(realMode, 0, bytesRemember, ref nIndexRemember, realMode.Length);
            ClientProvider.CopyBlock(userNameBytes, 0, bytesRemember, ref nIndexRemember, userNameBytes.Length);
            ClientProvider.CopyBlock(ipBytes, 0, bytesRemember, ref nIndexRemember, ipBytes.Length);

            Send(bytesRemember, m_provider);

		}

        public void SendChangedConfig(byte byteClientType, string strPropertyName, string strPropertyValue)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(byteClientType);
            arrDatas.Add(strPropertyName);
            arrDatas.Add(strPropertyValue);

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_CONFIG, arrDatas);

            Send(bytes, m_provider);
        }

        private string m_strTempServerAddr = "";
        public void LockConnection()
        {
            m_strTempServerAddr = m_strServerAddr;
            m_strServerAddr = "";
        }

        public void ReleaseConnection()
        {
            m_strServerAddr = m_strTempServerAddr;
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
