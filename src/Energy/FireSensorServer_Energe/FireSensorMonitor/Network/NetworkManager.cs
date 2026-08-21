using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using SDMS;
using TcpLib2;

namespace SensorMonitor
{
	public class NetworkManager
	{
		private ClientProvider m_provider = null;

		private int m_nPort = -1;

		private string m_strServerAddr = "";

		private bool shutdownThread = false;

		private DBUtility.WebDBManager m_dbMgr = null;

		// Ping은 로그에 남기지 않는다.
		private bool m_exceptPingLog = true;

        private MessageQueue m_msgQueue = new MessageQueue();

        // 전체 FireReciverProvider
        private ArrayList m_arReicverProvider = new ArrayList();

        // 각 FireReciver에 대한 State정보
        private Dictionary<int, ReciverState> m_dicStateList = new Dictionary<int, ReciverState>();
        
        // Site ID
        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private bool shutdownSensorThread = false;
        public bool ShutdownSensorThread
        {
            get { return shutdownSensorThread; }
            set { shutdownSensorThread = value; }
        }

        public SensorMonitor.ClientProvider ClientProvider
        {
            get { return m_provider; }
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
			ConnectionLogEx.MakeInstance();		
		}

		public void RecvLog(byte[] bytes)
		{
            if (!ConnectionLogEx.Instance.IsOpened)
				return;

			if (bytes[0] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
			{
				string strLog = string.Format("RecvMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
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

		public void SensorRecvLog(byte[] bytes)
		{
            if (!ConnectionLogEx.Instance.IsOpened)
				return;

			if (bytes[0] != SERIAL_ID.POLL || !m_exceptPingLog)
			{
				string strLog = string.Format("RecvSensorMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
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

        private int WriteSendLog(int nResult, byte[] bytes, ClientProvider provider, int nOffset)
        {
            if (nResult > 0)
            {
                if (!ConnectionLogEx.Instance.IsOpened)
                    return nResult;

                if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                {
                    string strLog = string.Format("SendMessage : Header({0}), Length({1})", (int)bytes[nOffset], (int)bytes.Length);
                    string strBytes = "";

                    for (int i=nOffset;i<bytes.Length;i++)
                    {
                        byte b = bytes[i];

                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }
                    WriteLineLog(strLog + strBytes);
                }
                provider.PingCount = 0;
            }
            return nResult;
        }

		public int Send(byte[] bytes, ClientProvider provider)
		{
			int nResult = provider.Send(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 0);			
		}

        public int Send_NoLengthByte(byte[] bytes, ClientProvider provider)
        {
            int nResult = provider.Send_NoLengthByte(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 4);
        }
        
        public NetworkManager(DBUtility.WebDBManager dbMgr, string strServerAddr, int nSiteID)
		{
            m_nSiteID = nSiteID;

			InitLog();  

			m_dbMgr = dbMgr;

            if (strServerAddr == null)
            {
                //string strPort = DBUtility.RegUtil.ReadRegValue("sdms_port", "Server Connection Info", nSiteID);
                string strServerURL = m_dbMgr.WebServerURL;

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
                strServerAddr = addr[0].ToString();
            }
           
			m_provider = new ClientProvider(this);
			m_strServerAddr = strServerAddr;

			Thread t = new Thread(ConnectionThread);
            t.Name = "Server Connection Thread";
			t.Start();			
		}

        private int GetSensorServerPort()
        {
            string strSQL = "Select Port from SensorServerPort";// where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        }

		private int GetServerPort()
		{
            string strSQL = "Select    Port from SDMSServerPort  Where SiteID = " + m_nSiteID.ToString();
			ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null || arrResult.Count == 0)
				return -1;

			int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			return nPort;
		}

		public void ReleaseThread()
		{
			shutdownThread = true;
			shutdownSensorThread = true;
            CloseAllReciverProvider();
		}

        public void CloseAllReciverProvider()
        { 
            foreach (FireReciverProvider provider in m_arReicverProvider)
            {
                if (provider != null)
                    provider.StopServer();
            }
        }
		
		public void CreateReciverProvider()
		{
			m_arReicverProvider.Clear();
			
			shutdownSensorThread = false;
			
            // Get Reciver List
			ArrayList arReciverList = SOPMonitor.Instance.IoMgr.GetReciverList();
			if (arReciverList != null)
			{
                //arReciverList.Reverse();

                string szPath = System.Reflection.Assembly.GetEntryAssembly().FullName;
                for (int i = 0; i < arReciverList.Count; i++)
                {
                    Reciver reciver = (Reciver)arReciverList[i];
                    FireReciverProvider provider = new FireReciverProvider(this, reciver);

                    try
                    {
                        provider.BeginServer();
                    }
                    catch(Exception)
                    {

                    }
                    
                    m_arReicverProvider.Add(provider);
                }                   				
			}

            Thread tt = new Thread(ReciverCheckThread);
            tt.Name = "ReciverStateChecker";
            tt.Start(this);
		}


        private int m_bChangedCount = 0;
		private void ReciverCheckThread(object p)
		{
			NetworkManager manager = (NetworkManager)p;
			
			m_dicStateList.Clear();

			ArrayList arReciverList = SOPMonitor.Instance.IoMgr.GetReciverList();
            arReciverList.Reverse();
            int nStart = 0;
            int nCount = arReciverList.Count;

            string szPath = System.Reflection.Assembly.GetEntryAssembly().FullName;

            for (int i = nStart; i < nCount; i++)
            {
                Reciver reciver = (Reciver)arReciverList[i];
                ReciverState state = new ReciverState();
                state.ID = reciver.ID;
                state.TargetReciver = reciver;
                state.LastAccess = DateTime.Now;
                state.Connected = reciver.IsConnected;
                m_dicStateList.Add(state.ID, state);
            }
      
			DateTime lastTime = DateTime.Now;

			while (!shutdownThread)
			{                
				if (manager != null && m_dicStateList.Count > 0)
				{
					if (!shutdownThread)
					{

                        bool m_bChangedData = false;
						foreach (KeyValuePair<int, ReciverState> pair in m_dicStateList)
						{
							ReciverState state = pair.Value;

							if (state.Connected != state.TargetReciver.IsConnected)
							{								
								state.Connected = state.TargetReciver.IsConnected;
                                m_bChangedData = true;
                            }
#if !SERVICE
                            if (state.Connected == true)
                                FormMain.Instance.OnConnectReciver(state.ID);
                            else
                                FormMain.Instance.OnDisconnectReciver(state.ID);
#endif
						}


                        if (m_bChangedData == true)
                        {

                            m_bChangedCount++;
                            if (m_bChangedCount == 3)
                            {
                                m_bChangedCount = 0;
                                manager.SendAllReciverState();
                            }
                        }

						DateTime dtNow = DateTime.Now;
						TimeSpan span = dtNow - lastTime;
						if (span.TotalMinutes > 3.0)
						{
                            manager.SendAllReciverState();
                            lastTime = DateTime.Now;
						}



						for (int i = 0; i < 300; i++)
						{
							if (!shutdownThread)
								Thread.Sleep(100);
							else
								break;
						}
					}
				}
			}
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
						if (m_provider.PingCount > 30)
						{
							m_provider.PingCount = 0;
							m_provider.Close();
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
                        //m_strServerAddr = "127.0.0.1";
						if (m_nPort > 0)
							m_provider.Connect(m_strServerAddr, m_nPort);

						if (m_provider.IsConnected)
						{
							Thread.Sleep(10);

                            SendAllReciverState();

                        }
					}
				}

                if (m_provider.IsConnected)
                {
                    // 큐에 데이터가 있으면 내보낸다.
                    m_msgQueue.Send(this);
                }

				Thread.Sleep(3000);
			}
		}

		public void OnDropConnection()
		{
            m_msgQueue.AbleToSend = false;
		}

        public void MessageQueueReady()
        {
            // 1초 후에 MessageQueue에서 Send할 수 있도록 바꾼다.
            Thread t = new Thread(MessageQueueReadyThread);
            t.Name = "MessageQueueThread";
            t.Start();
        }

        // 1초 후에 MessageQueue에서 Send할 수 있도록 바꾼다.
        private void MessageQueueReadyThread()
        {
            Thread.Sleep(1000);
            m_msgQueue.AbleToSend = true;
        }

        //public void SendReciverState(int nReciver, bool bConnected)
        //{
        //    if (!m_provider.IsConnected)
        //        return;
			
        //    byte[] bytes = new byte[24];

        //    byte[] nReciverIDBytes = ClientProvider.MakeBytes(nReciver);
        //    byte[] nConnectedBytes = ClientProvider.MakeBytes(bConnected == true ? 1 : 0);

        //    short nHeader = bConnected == true ? TCP_ID.RECIVER_CONNECT : TCP_ID.RECIVER_DISCONNECT;
        //    byte[] byteHeader = BitConverter.GetBytes(nHeader);
        //    bytes[0] = byteHeader[0];
        //    bytes[1] = byteHeader[1];

        //    // SET DATA COUNT
        //    byte[] nCount = BitConverter.GetBytes(2);
        //    bytes[2] = nCount[0];
        //    bytes[3] = nCount[1];
        //    bytes[4] = nCount[2];
        //    bytes[5] = nCount[3];

        //    int nIndex = 6;

        //    CopyBytes(bytes, ref nIndex, nReciverIDBytes);
        //    CopyBytes(bytes, ref nIndex, nConnectedBytes);

        //    // 바로 보내지 않고 Queue에 쌓아둔다.
        //    // Queue에 쌓인 데이터는 ConnectionThread에서 한꺼번에 보낸다.
        //    m_msgQueue.Add(new QueueData_ReceiverConnection(bytes, nReciver));
        //}

		public void SendAllReciverState()
		{
			if (!m_provider.IsConnected)
				return;

			ArrayList arReciverList = (ArrayList)SOPMonitor.Instance.IoMgr.GetReciverList().Clone();
			if (arReciverList == null)
				return;
            

            int dddd = arReciverList.Count;
            int ddCount = arReciverList.Count;
            string szPath = System.Reflection.Assembly.GetEntryAssembly().FullName;

            int nDataCount = ddCount * 2;
			int nSize = 6 + (nDataCount * 9);
			byte[] bytes = new byte[nSize];
					  
			byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.ALL_RECIVER_STATE);
			bytes[0] = byteHeader[0];
			bytes[1] = byteHeader[1];

			// SET DATA COUNT
			byte[] nCount = BitConverter.GetBytes(nDataCount);
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			int nIndex = 6;
			
			if (arReciverList != null)
			{
                arReciverList.Reverse();

                for (int i = 0; i < arReciverList.Count; i++)
                {
                    Reciver reciver = (Reciver)arReciverList[i];

                    int nCon = reciver.IsConnected == true ? 1 : 0;

                    int nPol = reciver.RecivedPoll == true ? 10 : 0;

                    nCon += nPol;

                    if (nCon > 0 && nPol == 0)
                    {
                        int sss = 0;
                        sss++;
                    }

                    byte[] nReciverIDBytes = ClientProvider.MakeBytes(reciver.ID);
                    byte[] nConnectedBytes = ClientProvider.MakeBytes(nCon);
                    

                    CopyBytes(bytes, ref nIndex, nReciverIDBytes);
                    CopyBytes(bytes, ref nIndex, nConnectedBytes);
                }
			}

            // 바로 보내지 않고 Queue에 쌓아둔다.
            // Queue에 쌓인 데이터는 ConnectionThread에서 한꺼번에 보낸다.
            m_msgQueue.Add(new QueueData_AllReceiverState(bytes));
		}

        public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, string szBuilding, string szTag, bool bPSM = false)
        {
            // SOP서버로 연결된 Provider로 전송
            if (!m_provider.IsConnected)
                return false;

            byte[] bytes = new byte[42];

            int nSensor = -1;
            Facility.FacilityType sensorType = Facility.ToFacilityType(nSensorType);

            switch (sensorType)
            {
                case Facility.FacilityType.FIRE_SENSOR:
                case Facility.FacilityType.FireSensor_TypeA:
                case Facility.FacilityType.FireSensor_TypeB:
                case Facility.FacilityType.FireSensor_GasEmission:
                case Facility.FacilityType.FireSensor_ManualControl:
                    nSensor = (int)Facility.FacilityType.FIRE_SENSOR;
                    break;

                case Facility.FacilityType.FireSensor_SiemensType:
                case Facility.FacilityType.FireSensor_AnalogSmokeType:
                case Facility.FacilityType.PSM_SENSOR:
                    nSensor = (int)sensorType;
                    break;
            }

            if (nSensor == -1)
                return false;

            if (nSensorZoneID < 0)
                return false;

            byte[] sensorTypeBytes = ClientProvider.MakeBytes(nSensor);
            byte[] sensorTagInfoIDBytes = ClientProvider.MakeBytes(nSensorTagInfoID);
            byte[] zoneIDBytes = ClientProvider.MakeBytes(nSensorZoneID);
            byte[] dataBytes = ClientProvider.MakeBytes((int)nData);


            short sHeader = (short)TCP_ID.SENSOR_DATA;
            if (bPSM == true)
            {
                sHeader = (short)TCP_ID.PSM_SENSOR_DATA;
            }

            byte[] nHeader = BitConverter.GetBytes(sHeader);
            bytes[0] = nHeader[0];
            bytes[1] = nHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(4);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            int nIndex = 6;

            CopyBytes(bytes, ref nIndex, sensorTypeBytes);
            CopyBytes(bytes, ref nIndex, sensorTagInfoIDBytes);
            CopyBytes(bytes, ref nIndex, zoneIDBytes);
            CopyBytes(bytes, ref nIndex, dataBytes);

            // 바로 보내지 않고 Queue에 쌓아둔다.
            // Queue에 쌓인 데이터는 ConnectionThread에서 한꺼번에 보낸다.
            m_msgQueue.Add(new QueueData_SensorData(bytes, nSensorZoneID, nSensor));
            return true;
        }

   
		/*public bool SendSensorData(EquipmentZone zone, int nSensorType, byte data)
		{
			if (!m_provider.IsConnected)
				return false;			

			byte[] bytes = new byte[33];

			byte[] sensorTypeBytes = ClientProvider.MakeBytes(nSensorType);
			byte[] zoneIDBytes = ClientProvider.MakeBytes(zone.ID);
			byte[] dataBytes = ClientProvider.MakeBytes((int)data);


			byte[] nHeader = BitConverter.GetBytes((short)TCP_ID.SENSOR_DATA);
			bytes[0] = nHeader[0];
			bytes[1] = nHeader[1];

			// SET DATA COUNT
			byte[] nCount = BitConverter.GetBytes(3);
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			int nIndex = 6;

			CopyBytes(bytes, ref nIndex, sensorTypeBytes);
			CopyBytes(bytes, ref nIndex, zoneIDBytes);
			CopyBytes(bytes, ref nIndex, dataBytes);

            // 바로 보내지 않고 Queue에 쌓아둔다.
            // Queue에 쌓인 데이터는 ConnectionThread에서 한꺼번에 보낸다.
            m_msgQueue.Add(new QueueData_SensorData(bytes, zone.ID, nSensorType));
            return true;
		}*/

		private void CopyBytes(byte[] bytesDest, ref int nDestOffset, byte[] bytesSrc)
		{
			int nLength = bytesSrc.Length;
			System.Buffer.BlockCopy(bytesSrc, 0, bytesDest, nDestOffset, nLength);
			nDestOffset += nLength;
		}
	}

	public class ConnectionLogEx : ConnectionLog
	{
		private log4net.ILog logger = null;

        public static ConnectionLogEx Instance
        {
            get { return (ConnectionLogEx)m_instance; }
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

		public override bool Write(object obj, bool writeTime = true)
		{
			if(obj.GetType() == typeof(Exception))
			{
				Exception e = (Exception)obj;
				if (logger != null)
					logger.Debug(e.Message, e);
			}
			else
			{
				if (logger != null)
					logger.DebugFormat("{0}", obj.ToString());
			}
			return true;
		}

		public override bool WriteLine(object obj, bool writeTime = true)
		{
			if(obj.GetType() == typeof(Exception))
			{
				Exception e = (Exception)obj;
				if (logger != null)
					logger.Debug(e.Message, e);
			}
			else
			{
				if (logger != null)
					logger.Debug(obj.ToString());
			}
			return true;
		}
	}
}
