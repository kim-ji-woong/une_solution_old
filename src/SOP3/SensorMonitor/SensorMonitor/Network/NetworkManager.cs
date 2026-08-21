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
		//private bool m_isConnected = false;
		private bool shutdownThread = false;
		private DBUtility.WebDBManager m_dbMgr = null;

		// Ping은 로그에 남기지 않는다.
		private bool m_exceptPingLog = true;

        private MessageQueue m_msgQueue = new MessageQueue();

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
                    //foreach (byte b in bytes)
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
			/*if (nResult > 0)
			{
				if (!ConnectionLog.Instance.IsOpened)
					return nResult;

				if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
				{
					string strLog = string.Format("SendMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
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

                provider.PingCount = 0;
			}

			return nResult;*/
		}

        public int Send_NoLengthByte(byte[] bytes, ClientProvider provider)
        {
            int nResult = provider.Send_NoLengthByte(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 4);
        }


        private int m_nSiteID = 1;

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

		public NetworkManager(DBUtility.WebDBManager dbMgr, string strServerAddr, int nSiteID)
		{
            m_nSiteID = nSiteID;

			InitLog();  

			m_dbMgr = dbMgr;

            if (strServerAddr == null)
            {
                string strPort = m_dbMgr.LoadIni("sdms_port", "Server Connection Info");
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

		private int GetServerPort()
		{
            string strSQL = "Select Port from SDMSServerPort";// where SiteID = " + m_nSiteID.ToString();
			ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null || arrResult.Count == 0)
				return -1;

			int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			return nPort;
		}


		private bool shutdownSensorThread = false;

		public bool ShutdownSensorThread
		{
			get { return shutdownSensorThread; }
			set { shutdownSensorThread = value; }
		}
		public void ReleaseThread()
		{
			shutdownThread = true;
			shutdownSensorThread = true;
            //CloseAllReciverProvider();
		}

        public void CloseAllReciverProvider()
        {         

            foreach (ReciverClientProvider provider in m_arReicverProvider)
            {
                if (provider != null)
                    provider.ExitClose();
            }
           
        }

		private ArrayList m_arReicverProvider = new ArrayList();
		public void CreateReciverProvider()
		{
			m_arReicverProvider.Clear();
			
			shutdownSensorThread = false;
			// Get Reciver List
			ArrayList arReciverList = SOPMonitor.Instance.IoMgr.GetReciverList();
			if (arReciverList != null)
			{
                //Reciver reciver = (Reciver)arReciverList[0];
                //reciver.Address = "192.168.0.223";
                arReciverList.Reverse();

                int nStart = 0;
                int nCount = 13;

                string szPath = System.Reflection.Assembly.GetEntryAssembly().FullName;

                if (szPath.Contains("SensorMonitor2"))
                {
                    nStart = 13;
                    nCount = 25;
                }

                for (int i = nStart; i < nCount; i++)
                {
                    //if (reciver.Address == "172.18.101.122")
                    {
                        Reciver reciver = (Reciver)arReciverList[i];
                        ReciverClientProvider provider = new ReciverClientProvider(this, reciver);
                        m_arReicverProvider.Add(provider);

                        Thread t2 = new Thread(SensorReciveThread);
                        t2.Name = "Sensor[" + reciver.Address + "]Thread";
                        t2.Start(provider);

                        Thread.Sleep(100);
                    }
                }
                   				
			}

			//Thread tt = new Thread(ReciverCheckThread);
            //tt.Name = "ReciverStateChecker";
			//tt.Start(this);
		}

		private Dictionary<int, ReciverState> m_dicStateList = new Dictionary<int, ReciverState>();
		private void ReciverCheckThread(object p)
		{
			NetworkManager manager = (NetworkManager)p;
			
			m_dicStateList.Clear();

			ArrayList arReciverList = SOPMonitor.Instance.IoMgr.GetReciverList();
            arReciverList.Reverse();
            int nStart = 0;
            int nCount = 13;

            string szPath = System.Reflection.Assembly.GetEntryAssembly().FullName;

            if (szPath.Contains("SensorMonitor2"))
            {
                nStart = 13;
                nCount = 25;
            }

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
                   	
            //foreach (Reciver reciver in arReciverList)
            //{
            //    ReciverState state = new ReciverState();
            //    state.ID = reciver.ID;
            //    state.TargetReciver = reciver;
            //    state.LastAccess = DateTime.Now;
            //    state.Connected = reciver.IsConnected;
            //    m_dicStateList.Add(state.ID, state);
            //}

			DateTime lastTime = DateTime.Now;

			while (!shutdownThread)
			{                
			   // bool bModifyData = false;
				if (manager != null && m_dicStateList.Count > 0)
				{
					if (!shutdownThread)
					{
						foreach (KeyValuePair<int, ReciverState> pair in m_dicStateList)
						{
							ReciverState state = pair.Value;

							if (state.Connected != state.TargetReciver.IsConnected)
							{
								//bModifyData = true;
								state.Connected = state.TargetReciver.IsConnected;

#if WIN
                                // nothing
#else
                                //manager.SendReciverState(state.ID, state.Connected);
#endif

                            }
						}

						//if (bModifyData == true)
						//{
						//    lastTime = DateTime.Now;
						//}

						DateTime dtNow = DateTime.Now;
						TimeSpan span = dtNow - lastTime;
						if (span.TotalMinutes > 30.0)
						{
#if WIN
                            // nothing
#else
                            manager.SendAllReciverState();
#endif

                            lastTime = DateTime.Now;
						}

						for (int i = 0; i < 100; i++)
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

		private void SensorReciveThread(object p)
		{
			ReciverClientProvider provider = (ReciverClientProvider)p;
			while (!shutdownSensorThread)
			{
				if (provider != null)
				{
					if (provider.IsConnected)
					{
						if (provider.PingCount > 200)
						{
							provider.PingCount = 0;
							provider.Close();
						}
						else
						{
                            provider.PingCount++;
							if(!provider.OnReceiveData())
                            {                                
                                provider.PingCount = 0;
                                provider.Close();

                                Thread.Sleep(100);
                            }
						}                        
					}
					if (!provider.IsConnected)
					{
						provider.Connect();
						provider.SendNACK();
					}
				}			
				Thread.Sleep(900);
			}

            if (provider.IsConnected)
            {
                provider.Close();
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
						if (m_provider.PingCount > 20)
						{
							m_provider.PingCount = 0;
							m_provider.Close();
						}
						// IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                        else if (m_provider.IsReadingProcess)
                        {
                            m_provider.SendData(TCP_ID.I_AM_HERE);
                        }
                        else
                            m_provider.PingCount++;
					}

					if (!m_provider.IsConnected)
					{
						m_nPort = GetServerPort();

						if (m_nPort > 0)
							m_provider.Connect(m_strServerAddr, m_nPort);

						if (m_provider.IsConnected)
						{
							Thread.Sleep(100);
#if WIN
                            // nothing
#else
                            SendAllReciverState();
#endif

                        }
					}
				}

                if (m_provider.IsConnected)
                {
                    // 큐에 데이터가 있으면 내보낸다.
                    m_msgQueue.Send(this);
                }

				Thread.Sleep(1000);
			}
		}

		public void OnDropConnection()
		{
            m_msgQueue.AbleToSend = false;
			//lock (this)
			//{
				//m_isConnected = false;
				//m_provider = new ClientProvider(this);
			//}
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

		public void SendReciverState(int nReciver, bool bConnected)
		{
			if (!m_provider.IsConnected)
				return;
			
			byte[] bytes = new byte[24];

			byte[] nReciverIDBytes = ClientProvider.MakeBytes(nReciver);
			byte[] nConnectedBytes = ClientProvider.MakeBytes(bConnected == true ? 1 : 0);

			short nHeader = bConnected == true ? TCP_ID.RECIVER_CONNECT : TCP_ID.RECIVER_DISCONNECT;
			byte[] byteHeader = BitConverter.GetBytes(nHeader);
			bytes[0] = byteHeader[0];
			bytes[1] = byteHeader[1];

			// SET DATA COUNT
			byte[] nCount = BitConverter.GetBytes(2);
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			int nIndex = 6;

			CopyBytes(bytes, ref nIndex, nReciverIDBytes);
			CopyBytes(bytes, ref nIndex, nConnectedBytes);

            // 바로 보내지 않고 Queue에 쌓아둔다.
            // Queue에 쌓인 데이터는 ConnectionThread에서 한꺼번에 보낸다.
            m_msgQueue.Add(new QueueData_ReceiverConnection(bytes, nReciver));
			//Send(bytes, m_provider);

			//m_provider.PingCount = 0;
		}

		public void SendAllReciverState()
		{
			if (!m_provider.IsConnected)
				return;

			ArrayList arReciverList = SOPMonitor.Instance.IoMgr.GetReciverList();
			if (arReciverList == null)
				return;
            
            int nStart = 0;
            int dddd = 13;
            int ddCount = 13;
            string szPath = System.Reflection.Assembly.GetEntryAssembly().FullName;

            if (szPath.Contains("SensorMonitor2"))
            {
                nStart = 13;
                dddd = 25;
                ddCount = 12;
            }

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
                
                for (int i = nStart; i < dddd; i++)
                {
                    Reciver reciver = (Reciver)arReciverList[i];
                    //foreach (Reciver reciver in arReciverList)
                    //{
                    byte[] nReciverIDBytes = ClientProvider.MakeBytes(reciver.ID);
                    byte[] nConnectedBytes = ClientProvider.MakeBytes(reciver.IsConnected == true ? 1 : 0);

                    CopyBytes(bytes, ref nIndex, nReciverIDBytes);
                    CopyBytes(bytes, ref nIndex, nConnectedBytes);
                }
			}

            // 바로 보내지 않고 Queue에 쌓아둔다.
            // Queue에 쌓인 데이터는 ConnectionThread에서 한꺼번에 보낸다.
            m_msgQueue.Add(new QueueData_AllReceiverState(bytes));
			//Send(bytes, m_provider);

			//m_provider.PingCount = 0;
		}

		public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, string szBuilding, string szTag)
		{
			// SOP서버로 연결된 Provider로 전송
			if (!m_provider.IsConnected)
				return false;

			

			//if (nData == 2)
			//{
			//	nData = 0;
			//}

			byte[] bytes = new byte[42];
			//1(화재탐지 센서), 2(소화 센서), 3(압력 센서), 4(발신기)
			
			// 들어오는 신호
			//화재 센서(0), 화재감지기 A(1), 화재감지기 B(2), 가스 방출신호(3), 수동조작함 신호(4)

			int nSensor = -1;
			switch(nSensorType)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					nSensor = 1;
					break;
				case 4:
					nSensor = 1;
					break;
			};

			if (nSensor == -1)
				return false;

			if (nSensorZoneID < 0)
				return false;

			byte[] sensorTypeBytes = ClientProvider.MakeBytes(nSensor);
            byte[] sensorTagInfoIDBytes = ClientProvider.MakeBytes(nSensorTagInfoID);
			byte[] zoneIDBytes = ClientProvider.MakeBytes(nSensorZoneID);
			byte[] dataBytes = ClientProvider.MakeBytes((int)nData);


			byte[] nHeader = BitConverter.GetBytes((short)TCP_ID.SENSOR_DATA);
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

			//return Send(bytes, m_provider) > 0;

			//m_provider.PingCount = 0;
		}

        //public bool SendSensorData(EquipmentZone zone, int nSensorType, byte data)
        //{
			
        //    //if (!m_isConnected)
        //    if (!m_provider.IsConnected)
        //        return false;

			

        //    byte[] bytes = new byte[33];

        //    byte[] sensorTypeBytes = ClientProvider.MakeBytes(nSensorType);
        //    byte[] zoneIDBytes = ClientProvider.MakeBytes(zone.ID);
        //    byte[] dataBytes = ClientProvider.MakeBytes((int)data);


        //    byte[] nHeader = BitConverter.GetBytes((short)TCP_ID.SENSOR_DATA);
        //    bytes[0] = nHeader[0];
        //    bytes[1] = nHeader[1];

        //    // SET DATA COUNT
        //    byte[] nCount = BitConverter.GetBytes(3);
        //    bytes[2] = nCount[0];
        //    bytes[3] = nCount[1];
        //    bytes[4] = nCount[2];
        //    bytes[5] = nCount[3];

        //    int nIndex = 6;

        //    CopyBytes(bytes, ref nIndex, sensorTypeBytes);
        //    CopyBytes(bytes, ref nIndex, zoneIDBytes);
        //    CopyBytes(bytes, ref nIndex, dataBytes);

        //    // 바로 보내지 않고 Queue에 쌓아둔다.
        //    // Queue에 쌓인 데이터는 ConnectionThread에서 한꺼번에 보낸다.
        //    m_msgQueue.Add(new QueueData_SensorData(bytes, zone.ID, nSensorType));
        //    return true;
        //    /*bool bResult = Send(bytes, m_provider) > 0;

        //    if (bResult == true)
        //        m_provider.PingCount = 0;

        //    return bResult;*/
        //}

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
