using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using TcpLib2;

namespace SensorTester
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
            while (!shutdownThread)
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
						m_nPort = GetServerPort();

						if (m_nPort > 0)
							m_provider.Connect(m_strServerAddr, m_nPort);

						if (m_provider.IsConnected)
						{
							Thread.Sleep(10);
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
	
		public bool SendSensorData(int nSensorZoneID, int nSensorType, int nData, bool bPSM = true, bool bTest = true)
		{
			// SOP서버로 연결된 Provider로 전송
			if (!m_provider.IsConnected)
				return false;

	
			byte[] bytes = new byte[33];

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

            byte[] sensorTypeBytes = SOPClientProvider.MakeBytes(nSensor);
            byte[] zoneIDBytes = SOPClientProvider.MakeBytes(nSensorZoneID);
            byte[] dataBytes = SOPClientProvider.MakeBytes((int)nData);


            short sHeader = (short)TCP_ID.SENSOR_DATA;
            if( bTest == true)
            {
                sHeader = (short)TCP_ID.TEST_SENSOR_DATA;
                if (bPSM == true)
                {
                    sHeader = (short)TCP_ID.TEST_PSM_SENSOR_DATA;
                }
            }
            else
            {
                sHeader = (short)TCP_ID.SENSOR_DATA;
                if (bPSM == true)
                {
                    sHeader = (short)TCP_ID.PSM_SENSOR_DATA;
                }
            }
           

            byte[] nHeader = BitConverter.GetBytes(sHeader);
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

   

        public class TCP_ID
        {
            public const byte ARE_YOU_THERE = 1;
            public const byte I_AM_HERE = 2;

            public const byte SENSOR_DATA = 3;
            public const byte SENSOR_ZONE_DATA = 4;
            //public const byte SENSOR_CONNECTION_DATA = 4;

            public const byte FIRE_DETECT_REPORT = 5;   // 화재 탐지
            public const byte SENSOR_FAIL_REPORT = 6;
            public const byte MALFUNCTION_REPORT = 7;   // 오동작
            public const byte IGNORE_DETECT_REPORT = 8; // 화재신호 꺼짐
            public const byte CLEAR_DETECT_REPORT = 9;  // 상황 해제

            public const byte FIRE_DETECT_TRAINNING = 10;

            public const byte ALL_SENSOR_DATA_IN_RECIVER = 11;

            public const byte PSM_SENSOR_DATA = 12;
            public const byte PSM_DETECT_REPORT = 13;
            public const byte PSM_SENSOR_RESET = 14;
            public const byte PSM_BUZZER_STOP = 15;

            public const byte PSM_DETECT_BROADCAST = 16;
            public const byte PSM_REPORT_BROADCAST = 17;

            public const byte TEST_SENSOR_DATA = 18;
            public const byte TEST_PSM_SENSOR_DATA = 19;

            public const byte EDIT_SENSOR_ZONE = 21;

            //public const byte SENSOR_HISTORY_ID = 31;
            public const byte SENSOR_REACTION_HISTORY_DATA = 32;

            public const byte SENSOR_REACTION_HISTORY_DATA_LIST = 33;
            public const byte REQUEST_SENSOR_REACTION_HISTORY_DATA_LIST = 34;

            //public const byte SENSOR_HISTORY_ID_LIST = 33;

            public const byte RUN_SOP = 41;
            public const byte RUN_N_CANCEL_SOP = 42;
            public const byte FINISH_SOP = 43;
            public const byte IGNORE_SOP = 44;

            public const byte FIRE_SENSOR_SIGNAL = 50;

            public const byte RECIVER_CONNECT = 52;
            public const byte RECIVER_DISCONNECT = 53;
            public const byte ALL_RECIVER_STATE = 54;

            //public const byte CHANGE_FACILITY_MANAGER = 60; // 관리자 정보 변경
            //public const byte CHANGE_EQUIPZONE_CCTV = 61;   // CCTV 정보 변경

            public const byte REQUEST_RESTORE = 70;   // 복원 요청
            public const byte REJECT_RESTORE = 71;    // 복원 요청 거절
            public const byte ACCEPT_RESTORE = 72;	  // 복원 요청 승인
            public const byte BEGEIN_RESTORE = 73;    // 복원 작업 시작
            public const byte END_RESTORE = 74;       // 복원 작업 종료	, 모두 재시작

            public const byte LOGIN_USER = 81;          // 사용자 로그인
            public const byte ACCEPT_LOGIN = 82;		// 로그인 성공
            public const byte REJECT_LOGIN = 83;		// 로그인 실패
            public const byte CHECK_LOGIN = 84;		    // 로그인 상태 체크
            public const byte LOGOUT_USER = 85;			// 사용자 로그아웃
            public const byte JOIN_USER = 86;			// 사용자 등록
            public const byte CHNAGE_PASSWORD = 87;	    // 로그인된 사용자 비번 변경
            public const byte SET_PASSWORD = 88;		// 사용자 이름과 사번으로 사용자 비번 변경
            public const byte CHANGE_NICKNAME = 89;	    // 로그인된 사용자 별명 변경

            public const byte REQUEST_CONTROL = 91;     // 제어권 요청
            public const byte RETURN_CONTROL = 92;      // 제어권 반납
            public const byte GIVE_CONTROL = 93;        // 제어권 부여
            public const byte CONFIRM_GIVE_CONTROL = 94;// 제어권 취득 확인
            public const byte TAKE_CONTROL = 95;        // 제어권 상실
            public const byte CONFIRM_TAKE_CONTROL = 96;// 제어권 상실 확인
            public const byte REJECT_REQUEST_CONTROL = 97;  // 제어권 요청 거부
            public const byte STEAL_CONTROL = 98;       // 제어권 뺏기
            public const byte GIVE_CONTROL_KEY = 99;    // 특정 사용자에게만 제어권 부여

            public const byte WHO_ARE_YOU = 100;
            public const byte WHO_I_AM = 101;

            public const byte CHANGE_CONFIG = 110;       // 설정 변경

            public const byte WEATHER_INFO = 120;        //  기후 정보
            public const byte EARTHQUAKE_SENSOR_DETECT = 121;   // 지진 정보


            public const byte SENSOR_DATA_WITH_TAG = 124;
            public const byte SECURITY_DETECT_REPORT = 125;

            public const byte SOP_SELECT_MISSION = 200;  // SOP 미션 선택 전송
            public const byte SOP_CURRENT_SELECT_MISSION = 201; // SOP 현재 미션 선택 전송

            public const byte CHAGNE_WORK_MEMBER = 210;  // 근무조 변경

            public const byte SOP_SIMULATOR_COMMAND = 220;  // SOPSimulatorCommandType과 조합
            public const byte SDMS_COMMAND = 221;           // SDMSCommandType과 조합

            public const byte START_SERVER_FROM_MONITOR = 238;
            public const byte STOP_SERVER_FROM_MONITOR = 239;

            public const byte CHECK_ALL_SERVER = 240;
            public const byte SERVER_STATE = 241;

            public const byte START_SOP_SERVER = 242;
            public const byte STOP_SOP_SERVER = 243;

            public const byte START_TTS_SERVER = 244;
            public const byte STOP_TTS_SERVER = 245;

            public const byte START_SENSOR_MONITOR = 246;
            public const byte STOP_SENSOR_MONITOR = 247;

            public const byte START_BACKUP_LOG = 248;
            public const byte GET_BACKUP_LOG = 249;

            public const byte SERVER_COMMAND = 250;             // ServerCommandType과 조합
            public const byte INTERNAL_MESSAGE = 251;           // 통합관리자와 로컬 PC 내부간 통신
            public const byte TRAINING_SIMULATOR_COMMAND = 252; // TrainingSimulatorCommandType과 조합
        }

        public class TCP_CLIENT
        {
            public const byte SDMS_CLIENT = 1;
            public const byte SOP_SIMULATOR = 2;
            public const byte SENSOR_SIMULATOR = 3;
            public const byte SENSOR_MONITOR = 4;
            public const byte SOP_RESTORE = 5;
            public const byte INTEGRATE_MANAGE = 6;
            public const byte SDMS_CLIENT_SECOND = 7;
            public const byte SERVER_MONITOR = 8;
            public const byte SENSOR_MONITOR2 = 9;
            public const byte SERVER_COMMANDER = 10;
            public const byte TRAINING_SIMULATOR = 11;  // 연습용 모드
            public const byte SOP_WEATHER = 12;
            public const byte PSM_SENSOR_SERVER = 13;
            public const byte PSM_LEVEL_SERVER = 14;
            public const byte EARTHQUAKE_SENSOR_SERVER = 15;

            public const byte SVMS_EVENT_RECIVER = 16;
            public const byte ACCESS_EVENT_RECIVER = 17;
            public const byte SAINTOP_EVENT_RECIVER = 18;
            public const byte ASIN_EVENT_RECIVER = 19;
            public const byte S1_TEST_SENSOR_SERVER = 20;
        }

        public class ServerCommandType
        {
            public const byte RUN_SDMS = 1;
            public const byte UPDATE_SYSTEM = 2;
            public const byte REQUEST_PSM_SENSOR_ALARM = 3;
            public const byte REQUEST_PSM_SENSOR_RESET = 4;
            public const byte REQUEST_PSM_BUZZER = 5;
            public const byte DELETE_SENSOR_TAG_HISTORY = 6;
            public const byte EQUIPMENTZONE_CHANGE_NAME = 7;
        }

        public class SOPSimulatorCommandType
        {
            public const byte RESET_USER_DEFINED_TEAM_NAMES = 1;
        }

        public class TrainingSimulatorCommandType
        {
            public const byte SEND_SDMS_SMS = 1;
        }

        public class SDMSCommandType
        {
            public const byte CHANGE_PSM_SENSOR_STATUS = 1;
            public const byte PSM_SENSOR_DATA = 2;
            public const byte REFRESH_PSM_SENSOR_LIFE_TIME = 3;
            public const byte SDMS_PUBLIC_MESSAGE = 4;
            public const byte SDMS_PUBLIC_MESSAGE_ID = 5;
            public const byte PSM_SENSOR_ALARM_LEVEL = 6;


        }


}
