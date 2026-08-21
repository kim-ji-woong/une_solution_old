using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using TcpLib2;
using DBUtility2;
using SOPWebClient;

namespace PSMSensorServer
{
	public class NetworkWebClient
    {
        private class PostMan : IPostMan
        {
            private PostBox m_postBox = null;
            private NetworkWebClient m_owner = null;
            private int m_nClientType = -1;
            private int m_nClientSubType = -1;
            private bool m_isConnected = false;
            private int m_nPort = -1;
            private bool m_isPSM = false;
            private DateTime m_dtLastSendMessage = new DateTime();

            public PostBox PostBox
            {
                get { return m_postBox; }
                set { m_postBox = value; }
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
                set { m_isConnected = value; }
            }

            public int Port
            {
                get { return m_nPort; }
                set { m_nPort = value; }
            }

            public bool PSM
            {
                get { return m_isPSM; }
                set { m_isPSM = value; }
            }

            public DateTime LastSendMessageTime
            {
                get { return m_dtLastSendMessage; }
            }

            public PostMan(NetworkWebClient owner, int nClientType, int nClientSubType)
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
                    //m_postBox = null;
                    m_isConnected = false;

                    //if (m_owner != null)
                    //    m_owner.Connect(this);
                }
                else
                {
                    bool closeConnection;
                    bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                    if (closeConnection)
                    {
                        if (m_owner != null)
                            m_owner.WriteLog(m_postBox.ErrorMessage);

                        //m_postBox = null;
                        m_isConnected = false;

                        //if (m_owner != null)
                        //    m_owner.Connect(this);
                    }
                    else
                        m_dtLastSendMessage = DateTime.Now;

                    return result;
                }

                return false;
            }
        }

        private PostMan m_postManFire = null;
        private PostMan m_postManPSM = null;

        private bool m_shutdownThread = false;
		private WebDBManager m_dbMgr = null;
        
        private static NetworkWebClient m_instance = null;
        public static NetworkWebClient Instance
        {
            get { return m_instance; }
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
        
        private static log4net.ILog logger = null;
		public NetworkWebClient(WebDBManager dbMgr)
		{
            m_instance = this;

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			InitLog();  

			m_dbMgr = dbMgr;
            int nPort = ReadServerPort(m_dbMgr);

            m_postManFire = new PostMan(this, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.SIMULATOR);
            m_postManPSM = new PostMan(this, SOPWebServer.ClientType.PSM_SENSOR_SERVER, SOPWebServer.ClientSubType.JUBIX);
            m_postManPSM.PSM = true;

            SetPostBox(m_postManFire, nPort);
            SetPostBox(m_postManPSM, nPort);
            //Connect(m_postManFire, nPort);

            Thread t = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t.Start(m_postManFire);

            Thread t2 = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t2.Start(m_postManPSM);
        }
        private int ReadServerPort(WebDBManager dbMgr)
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

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

        private bool shutdownSensorThread = false;

		public bool ShutdownSensorThread
		{
			get { return shutdownSensorThread; }
			set { shutdownSensorThread = value; }
		}

		public void ReleaseThread()
		{
			m_shutdownThread = true;
			shutdownSensorThread = true;
            //CloseAllReciverProvider();
		}

		//private Dictionary<int, ReciverState> m_dicStateList = new Dictionary<int, ReciverState>();
		
		// 서버와의 접속이 끊어지면 다시 연결시킨다.
		private void ConnectionThread(object arg)
        {
            PostMan postMan = (PostMan)arg;

            while (m_shutdownThread == false)
            {
                if (postMan.IsConnected == false)
                {
                    int nPort = ReadServerPort(m_dbMgr);

                    if (postMan.Port != nPort)
                        SetPostBox(postMan, nPort);

                    if (postMan.PostBox != null)
                    {
                        if (postMan.PostBox.Connect(postMan.ClientType, postMan.ClientSubType))
                        {
                            postMan.IsConnected = true;

                            if (postMan.PSM)
                                SendAllReciverState();
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

                Thread.Sleep(1000);
            }
        }

        public void Close()
        {
            if (m_postManFire.IsConnected)
            {
                // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                // 실패하더라도 상관없다.
                bool closeConnection;
                m_postManFire.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_postManFire.IsConnected = false;
            }

            if (m_postManPSM.IsConnected)
            {
                // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                // 실패하더라도 상관없다.
                bool closeConnection;
                m_postManPSM.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_postManPSM.IsConnected = false;
            }

            m_shutdownThread = true;
        }

        public void SendReciverState(int nReciver, bool bConnected)
		{
			if (!m_postManPSM.IsConnected)
				return;

            int nHeader = bConnected == true ? SOPWebServer.Header.RECEIVER_CONNECT : SOPWebServer.Header.RECEIVER_DISCONNECT;
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nReciver);
            arrDatas.Add(bConnected == true ? 1 : 0);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            m_postManPSM.SendMessage(nHeader, bytes);
        }

		public void SendAllReciverState()
		{
            if (!m_postManPSM.IsConnected)
                return;

            ArrayList arReciverList = PSMNetworkServer.Instance.IOManager.GetPSMReciverList();
            if (arReciverList == null)
                return;

            int nDataCount = arReciverList.Count * 2;
            ArrayList arrDatas = new ArrayList();

            //arrDatas.Add(nDataCount);

            if (arReciverList != null)
            {
                foreach (Reciver receiver in arReciverList)
                {
                    arrDatas.Add(receiver.ID);
                    arrDatas.Add(receiver.IsConnected == true ? 1 : 0);
                }
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            m_postManPSM.SendMessage(SOPWebServer.Header.ALL_RECEIVER_STATE, bytes);
        }

        public void SendSensorData(int nReciver, int nCircuit, int nChannel, int nData, bool bPSM, int windDirection = -1, int windSpeed = -1)
        {
            int nReciverType = 1;
            if( bPSM == true)
            {
                nReciverType = 2;
            }
            Reciver reciver = PSMNetworkServer.Instance.IOManager.FindReciverForUnitID(nReciver, nReciverType);
            if (reciver != null)
            {
                Circuit curcuit = null;
                if (reciver.Curcuits.ContainsKey(nCircuit))
                {
                    curcuit = reciver.Curcuits[nCircuit];
                }

                if (curcuit != null)
                {
                    int nCurcuit = curcuit.ID;

                    logger.Debug("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");

                    //int nEquipzoneID = curcuit.TargetZoneID;
                    int nSensorZoneID = curcuit.SensorZone == null ? -1 : curcuit.SensorZone.ID;

                    int nTagNum = curcuit.TagNum;
                    int nSensorType = curcuit.SensorType;
                    //if(nSensorType == 6 && nSensorType == 9)
                    {
                        SendSensorData(nSensorZoneID, curcuit.ID, nSensorType, nData, "", nTagNum.ToString(), bPSM, false, windDirection, windSpeed);
                    }
                    logger.Debug("[SensorType]" + nSensorType);
                }
            }
        }

		public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, string szBuilding, string szTag, bool bPSM = false, bool bTest = false, int windDirection = -1, int windSpeed = -1)
		{
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

            PostMan postMan = bPSM ? m_postManPSM : m_postManFire;

            if (!postMan.IsConnected)
                return false;

            int nHeader = 0;

            if (bTest)
                nHeader = SOPWebServer.Header.SENSOR_DATA_TEST;
            else
                nHeader = SOPWebServer.Header.SENSOR_DATA;
            
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensor);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add((int)nData);
            arrDatas.Add((int)windDirection); //wind direcction ==> 0 : N, 1 : NE, E : 2, 3 : SE, 4 : S, 5 : SW, 6 : W, 7 : NW //암모니아 확산 모델링을 위한 바이트 전송, -1이면 암모니아 아님
            arrDatas.Add((int)windSpeed);     //windspeed ===> 1 : 2.7미만, 2 : 2.7이상 //상등

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return postMan.SendMessage(nHeader, bytes);
        }

        public bool SendFireSensorData(int sensorType, int sensorTagID, int sensorZoneID)
        {
            if (!m_postManFire.IsConnected)
                return false;

            int nData = 1;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(sensorType);
            arrDatas.Add(sensorTagID);
            arrDatas.Add(sensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return m_postManFire.SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
        }

        public void OnMessage(int header, byte[] messages, IPostMan postMan)
        {
            if (messages == null)
                return;

            PostMan _postMan = (PostMan)postMan;
            System.Diagnostics.Trace.WriteLine("OnMessage : " + header.ToString());

            ArrayList arrDatas = SOPWebServer.BinaryHelper.ReadBytes(messages);

            if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                _postMan.IsConnected = false;
            }
            else if (header == SOPWebServer.Header.EDIT_SENSOR_ZONE)
            {
                ProcessEditSensorZone(arrDatas);
            }
            else if (header == SOPWebServer.Header.SERVER_COMMAND)
            {
                ProcessServerCommand(arrDatas);
            }
        }

        private void ProcessEditSensorZone(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount % 4 != 0)
                return;

            for (int i = 0; i < nDataCount; i += 4)
            {
                int nSensorZoneID = (int)arrDatas[i];
                int nOriginEquipZoneID = (int)arrDatas[i + 1];
                int nChangedEquipZoneID = (int)arrDatas[i + 2];
                int nZoneID = (int)arrDatas[i + 3];

                SensorZone sensorZone = PSMNetworkServer.Instance.IOManager.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                    continue;

                EquipmentZone equipZoneOrigin = PSMNetworkServer.Instance.IOManager.GetEquipmentZone(nOriginEquipZoneID);
                EquipmentZone equipZoneChanged = PSMNetworkServer.Instance.IOManager.GetEquipmentZone(nChangedEquipZoneID);

                if (equipZoneOrigin != null)
                {
                    ArrayList arrSensorZones;

                    if (PSMNetworkServer.Instance.IOManager.D_EquipZoneSensor.TryGetValue(equipZoneOrigin, out arrSensorZones))
                    {
                        arrSensorZones.Remove(sensorZone);
                    }
                }

                if (equipZoneChanged != null)
                {
                    ArrayList arrSensorZones;

                    if (!PSMNetworkServer.Instance.IOManager.D_EquipZoneSensor.TryGetValue(equipZoneChanged, out arrSensorZones))
                    {
                        arrSensorZones = new ArrayList();
                        PSMNetworkServer.Instance.IOManager.D_EquipZoneSensor[equipZoneChanged] = arrSensorZones;
                    }

                    if (!arrSensorZones.Contains(sensorZone))
                        arrSensorZones.Add(sensorZone);
                }

                sensorZone.EquipZone = equipZoneChanged;
            }
        }

        public void ProcessServerCommand(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;
            byte nHeader = (byte)arrDatas[0];
            if (nHeader == SOPWebServer.ServerCommandType.REQUEST_PSM_SENSOR_ALARM)
            {
                int nSensorID = (int)arrDatas[1];
                ProcessRequestSensorAlarm(nSensorID);
            }
            else if (nHeader == SOPWebServer.ServerCommandType.REQUEST_PSM_SENSOR_RESET)
            {
                int nSensorID = (int)arrDatas[1];
                ProcessRequestSensorReset(nSensorID);

            }
            else if (nHeader == SOPWebServer.ServerCommandType.REQUEST_PSM_BUZZER)
            {
                int nSensorID = (int)arrDatas[1];
                int nOnOff = (int)arrDatas[2];
                ProcessRequestBuzzer(nSensorID, nOnOff);
            }
            else if (nHeader == SOPWebServer.ServerCommandType.REQUEST_PSM_TEST_ALARM)
            {
                int nSensorID = (int)arrDatas[1];
                ProcessRequestTestAlamr(nSensorID);
            }
        }

        private void ProcessRequestSensorAlarm(int nSensorID)
        {
            PSMSensorManager.Instance.RequestAlarm(nSensorID);

            string szMsg = string.Format("Request SOPServer : {0}번 센서 신호 요청", nSensorID);
            logger.Info(szMsg);
        }

        private void ProcessRequestSensorReset(int nSensorID)
        {
            PSMSensorManager.Instance.RequestReset(nSensorID);

            string szMsg = string.Format("Request SOPServer : {0}번 센서 신호 리셋", nSensorID);
            logger.Info(szMsg);
        }

        private void ProcessRequestBuzzer(int nSensorID, int nOnOff)
        {
            PSMSensorManager.Instance.BuzzerSet(nSensorID, nOnOff);

            if (nOnOff == 1)
            {
                string szMsg = string.Format("Request SOPServer : {0}번 센서 부저 켜기", nSensorID);
                logger.Info(szMsg);
            }
            else
            {
                string szMsg = string.Format("Request SOPServer : {0}번 센서 부저 정지", nSensorID);
                logger.Info(szMsg);
            }
        }

        private void ProcessRequestTestAlamr(int nSensorID)
        {
            PSMSensorManager.Instance.RequestTestAlarm(nSensorID);

            string szMsg = string.Format("Request SOPServer : {0}번 센서 테스트값 입력", nSensorID);
            logger.Info(szMsg);
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
