using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using SDMS;
using TcpLib2;

namespace S1SensorServer
{
	public class NetworkClient
	{
		private SOPClientProvider m_provider = null;
		private int m_nPort = -1;
		private string m_strServerAddr = "";

		private bool shutdownThread = false;
		private DBUtility.WebDBManager m_dbMgr = null;

		// Ping은 로그에 남기지 않는다.
		private bool m_exceptPingLog = true;

        private MessageQueue m_msgQueue = new MessageQueue();

        private static NetworkClient m_instance = null;
        public static NetworkClient Instance
        {
            get { return m_instance; }
        }

        public SOPClientProvider ClientProvider
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

        private int WriteSendLog(int nResult, byte[] bytes, SOPClientProvider provider, int nOffset)
        {
            if (nResult > 0)
            {
                provider.PingCount = 0;

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
            }

            return nResult;
        }

		public int Send(byte[] bytes, SOPClientProvider provider)
		{
			int nResult = provider.Send(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 0);		
		}

        public int Send_NoLengthByte(byte[] bytes, SOPClientProvider provider)
        {
            int nResult = provider.Send_NoLengthByte(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 4);
        }
        
        private int m_nSiteID = 2;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }
        private static log4net.ILog logger = null;
		public NetworkClient(DBUtility.WebDBManager dbMgr, string strServerAddr, int nSiteID)
		{
            m_instance = this;
            m_nSiteID = nSiteID;

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
           
			m_provider = new SOPClientProvider(this);
			m_strServerAddr = strServerAddr;
            //m_strServerAddr = "127.0.0.1";
            
			Thread t = new Thread(ConnectionThread);
            t.Name = "Server Connection Thread";
			t.Start();

		}
        
		private int GetServerPort()
		{
            string strSQL = "Select Port from SDMSServerPort where SiteID = " + m_nSiteID.ToString();
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

		//private Dictionary<int, ReciverState> m_dicStateList = new Dictionary<int, ReciverState>();
		
		// 서버와의 접속이 끊어지면 다시 연결시킨다.
		private void ConnectionThread()
        {
#if WIN
            FormMain.Instance.SetServerConnection(m_strServerAddr, false);
#endif

            while (!shutdownSensorThread)
			{
				lock (this)
				{
					if (m_provider.IsConnected)
					{
						if (m_provider.PingCount > 10)
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
#if WIN
                        FormMain.Instance.SetServerConnection(m_strServerAddr, false);
#endif
						m_nPort = GetServerPort();

						if (m_nPort > 0)
							m_provider.Connect(m_strServerAddr, m_nPort);

						if (m_provider.IsConnected)
						{
							Thread.Sleep(10);
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
#if WIN
                        FormMain.Instance.SetServerConnection(m_strServerAddr, true);
#endif
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

			byte[] nReciverIDBytes = SOPClientProvider.MakeBytes(nReciver);
            byte[] nConnectedBytes = SOPClientProvider.MakeBytes(bConnected == true ? 1 : 0);

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

            ArrayList arReciverList = S1NetworkServer.Instance.IOManager.GetPSMReciverList();
			if (arReciverList == null)
				return;

			int nDataCount = arReciverList.Count * 2;
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
				foreach (Reciver reciver in arReciverList)
				{
                    byte[] nReciverIDBytes = SOPClientProvider.MakeBytes(reciver.ID);
                    byte[] nConnectedBytes = SOPClientProvider.MakeBytes(reciver.IsConnected == true ? 1 : 0);            

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

        public void SendSensorData(int nReciver, int nCircuit, int nChannel, int nData, bool bPSM)
        {
            int nReciverType = 1;
            if( bPSM == true)
            {
                nReciverType = 2;
            }
            Reciver reciver = S1NetworkServer.Instance.IOManager.FindReciverForUnitID(nReciver, nReciverType);
            if (reciver != null)
            {
                Circuit curcuit = null;
                if (reciver.Curcuits.ContainsKey(nCircuit))
                {
                    curcuit = reciver.Curcuits[nCircuit];
                }

                if (curcuit != null)
                {
                    int nCurcuit = curcuit.TagNum;

                    logger.Debug("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");

                    //int nEquipzoneID = curcuit.TargetZoneID;
                    int nSensorZoneID = curcuit.SensorZone == null ? -1 : curcuit.SensorZone.ID;

                    int nTagNum = curcuit.TagNum;
                    int nSensorType = curcuit.SensorType;
                    //if(nSensorType == 6 && nSensorType == 9)
                    {
                        
                        SendSensorData(nSensorZoneID, curcuit.ID, nSensorType, nData, "", nTagNum.ToString(), bPSM);
                    }
                    logger.Debug("[SensorType]" + nSensorType);
                }
            }
        }

		public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, string szBuilding, string szTag, bool bPSM = false, bool bTest = false)
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

                case Facility.FacilityType.Fire_S1:
                case Facility.FacilityType.Intrusion_S1:
                case Facility.FacilityType.Loiter_S1:
                case Facility.FacilityType.Slip_S1:
                case Facility.FacilityType.Steal_S1:
                case Facility.FacilityType.Abandoned_S1 :
                case Facility.FacilityType.VirtualFence_S1:
                case Facility.FacilityType.EmergencyBell_S1:
                case Facility.FacilityType.GeneralIntrusionT1_S1:
                case Facility.FacilityType.GeneralIntrusionT2_S1:
                case Facility.FacilityType.InternalIntrusionT3_S1:
                case Facility.FacilityType.VaultIntrusionT4_S1:
                case Facility.FacilityType.FireF1_S1:
                case Facility.FacilityType.CustomerEmergencyC1_S1:
                case Facility.FacilityType.CustomerEmergencyC2_S1:
                case Facility.FacilityType.RescueQQ_S1:
                case Facility.FacilityType.GasG1_S1:
                case Facility.FacilityType.BlackoutAbnormalityU1_S1:
                case Facility.FacilityType.LeakAbnormalityU4_S1:
                case Facility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    nSensor = (int)sensorType;
                    break;

                case Facility.FacilityType.ExternalAlarmBell:
                    nSensor = (int)sensorType;
                    break;
            }
	
			if (nSensor == -1)
				return false;

			if (nSensorZoneID < 0)
				return false;

            byte[] sensorTypeBytes = SOPClientProvider.MakeBytes(nSensor);
            byte[] sensorTagIDBytes = SOPClientProvider.MakeBytes(nSensorTagInfoID);
            byte[] zoneIDBytes = SOPClientProvider.MakeBytes(nSensorZoneID);
            byte[] dataBytes = SOPClientProvider.MakeBytes((int)nData);



            short sHeader = (short)TCP_ID.SENSOR_DATA;

            DateTime dtNow = DateTime.Now;

#if SAFE_KOREA_YH_2017
            /*DateTime dtTarget = new DateTime(2017, 11, 3);

            // 안전한국 훈련 기간 인 경우 테스트 코드틑 뺀다
            if( dtNow < dtTarget)*/
            {
                sHeader = (short)TCP_ID.SENSOR_DATA;
            }
            //else
#else
            {
                if (bTest == true)
                {
                    sHeader = (short)TCP_ID.TEST_SENSOR_DATA;
                    if (bPSM == true)
                    {
                        //sHeader = (short)TCP_ID.TEST_PSM_SENSOR_DATA;
                    }
                }
                else
                {
                    sHeader = (short)TCP_ID.SENSOR_DATA;
                    if (bPSM == true)
                    {
                        //sHeader = (short)TCP_ID.PSM_SENSOR_DATA;
                    }
                }
            }
#endif      

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
            CopyBytes(bytes, ref nIndex, sensorTagIDBytes);
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

        //    byte[] sensorTypeBytes = SOPClientProvider.MakeBytes(nSensorType);
        //    byte[] zoneIDBytes = SOPClientProvider.MakeBytes(zone.ID);
        //    byte[] dataBytes = SOPClientProvider.MakeBytes((int)data);


        //    byte[] nHeader = BitConverter.GetBytes((short)TCP_ID.TEST_SENSOR_DATA);
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
            if (obj.GetType() == typeof(Exception))
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
            if (obj.GetType() == typeof(Exception))
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
