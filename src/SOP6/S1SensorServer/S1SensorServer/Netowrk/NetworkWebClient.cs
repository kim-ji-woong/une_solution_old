using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using SDMS;
using TcpLib2;
using DBUtility2;
using SOPWebClient;
using UnE.Sensor;
using UnE.Spatial;

namespace S1SensorServer
{
    using Data;

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
        private PostMan m_postManSecurity = null;
        private PostMan m_postManPSM = null;
        
        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
        }
        private DateTime m_dtLastSendMessage = new DateTime();
        
        private bool m_shutdownThread = false;
        public bool ShutdownThread
        {
            get { return m_shutdownThread; }
            set { m_shutdownThread = value; }
        }
        private DirectDBManagerEx m_dbMgr = null;
        
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
        
        private int m_nSiteID = -1;
        private static log4net.ILog logger = null;
		public NetworkWebClient(DirectDBManagerEx dbMgr, string strServerAddr, int nSiteID)
		{
            m_instance = this;            
            m_nSiteID = nSiteID;
            m_dbMgr = dbMgr;

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			InitLog();

            m_postManFire = new PostMan(this, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.SIMULATOR);            
            m_postManSecurity = new PostMan(this, SOPWebServer.ClientType.SECURITY_SENSOR_SERVER, SOPWebServer.ClientSubType.SIMULATOR);            
            m_postManPSM = new PostMan(this, SOPWebServer.ClientType.PSM_SENSOR_SERVER, SOPWebServer.ClientSubType.SIMULATOR);

            int nPort = ReadServerPort();
            SetPostBox(m_postManFire, nPort);
            SetPostBox(m_postManSecurity, nPort);
            SetPostBox(m_postManPSM, nPort);
            
            Thread t = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t.Start(m_postManFire);

            Thread t2 = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t2.Start(m_postManSecurity);

            Thread t7 = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t7.Start(m_postManPSM);
        }

        private void SetPostBox(PostMan postMan, int nPort)
        {
            if (nPort > 0)
            {
                string strWebServerURL;
                WebDBManager.DBType dbType;
                string strDBName;

                S1NetworkServer.Instance.LoadSiteID(out strWebServerURL, out dbType, out strDBName);
                PostBox postBox = new PostBox();
                postBox.WebServerURL = strWebServerURL;//m_dbMgr.WebServerURL;
                postBox.PostMan = postMan;
                postMan.PostBox = postBox;

                postMan.Port = nPort;
                postBox.Port = nPort;
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
        
		public void ReleaseThread()
		{
			m_shutdownThread = true;
		}

		//private Dictionary<int, ReciverState> m_dicStateList = new Dictionary<int, ReciverState>();
		
		// 서버와의 접속이 끊어지면 다시 연결시킨다.
		private void ConnectionThread(object arg)
        {
            PostMan postMan = (PostMan)arg;
            while (!m_shutdownThread)
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

                            if (postMan.PSM)
                                SendAllReciverState();
                        }
                    }
                }
                else
                {
                    TimeSpan span = DateTime.Now - m_dtLastSendMessage;

                    // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                    if (span.TotalSeconds > 3.0)
                    {
                        // 접속이 유지되고 있는지 확인한다.
                        postMan.SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                    }
                }
				Thread.Sleep(1000);

                if (m_postManFire.IsConnected && m_postManSecurity.IsConnected && m_postManPSM.IsConnected)
                    m_isConnected = true;
                else
                    m_isConnected = false;
            }
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
			if (!m_isConnected)
				return;

            ArrayList arReciverList = S1NetworkServer.Instance.IOManager.GetPSMReciverList();
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
                    arrDatas.Add(0);
                    //arrDatas.Add(receiver.IsConnected == true ? 1 : 0);
                }
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            m_postManPSM.SendMessage(SOPWebServer.Header.ALL_RECEIVER_STATE, bytes);
        }

        public void SendSensorData(int nReciver, int nCircuit, int nChannel, int nData, bool bPSM)
        {
            int nReciverType = 1;
            if (bPSM == true)
            {
                nReciverType = 2;
            }
            Reciver reciver = S1NetworkServer.Instance.IOManager.FindReciverForUnitID(nReciver, nReciverType);
            if (reciver != null)
            {
                Circuit2 curcuit = null;
                if (reciver.Circuits.ContainsKey(nCircuit))
                {
                    curcuit = (Circuit2)reciver.Circuits[nCircuit];
                }

                if (curcuit != null)
                {
                    int nCurcuit = curcuit.TagNum;

                    logger.Debug("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");

                    //int nEquipzoneID = curcuit.TargetZoneID;
                    int nSensorZoneID = curcuit.SensorZone == null ? -1 : curcuit.SensorZone.ID;

                    int nTagNum = curcuit.TagNum;
                    int nSensorType = (int)curcuit.SensorType;
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
            int nSensor = -1;
            IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);

            switch (sensorType)
            {
                case IFacility.FacilityType.FIRE_SENSOR:
                case IFacility.FacilityType.FireSensor_TypeA:
                case IFacility.FacilityType.FireSensor_TypeB:
                case IFacility.FacilityType.FireSensor_GasEmission:
                case IFacility.FacilityType.FireSensor_ManualControl:
                case IFacility.FacilityType.SecomFire:
                    nSensor = (int)IFacility.FacilityType.FIRE_SENSOR;
                    break;
                case IFacility.FacilityType.FireSensor_SiemensType:
                case IFacility.FacilityType.FireSensor_AnalogSmokeType:
                case IFacility.FacilityType.PSM_SENSOR:
                case IFacility.FacilityType.Fire_S1:
                case IFacility.FacilityType.Intrusion_S1:
                case IFacility.FacilityType.Loiter_S1:
                case IFacility.FacilityType.Collapse_S1:
                case IFacility.FacilityType.Theft_S1:
                case IFacility.FacilityType.Neglect_S1:
                case IFacility.FacilityType.VirtualFence_S1:
                case IFacility.FacilityType.EmergencyBell_S1:
                case IFacility.FacilityType.GeneralIntrusionT1_S1:
                case IFacility.FacilityType.GeneralIntrusionT2_S1:
                case IFacility.FacilityType.InternalIntrusionT3_S1:
                case IFacility.FacilityType.VaultIntrusionT4_S1:
                case IFacility.FacilityType.FireF1_S1:
                case IFacility.FacilityType.CustomerEmergencyC1_S1:
                case IFacility.FacilityType.CustomerEmergencyC2_S1:
                case IFacility.FacilityType.RescueQQ_S1:
                case IFacility.FacilityType.GasG1_S1:
                case IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                case IFacility.FacilityType.LeakAbnormalityU4_S1:
                case IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                case IFacility.FacilityType.ExternalAlarmBell:
                case IFacility.FacilityType.SecomExternalAlarmBell:
                case IFacility.FacilityType.SecomWomenAlarmBell:
                    nSensor = (int)sensorType;
                    break;
            }

            if (nSensor == -1)
                return false;

            if (nSensorZoneID < 0)
                return false;
            
            PostMan postMan = null;
            if (IFacility.IsFireSensorType(sensorType))
                postMan = m_postManFire;
            else if (IFacility.IsPSMSensorType(sensorType))
                postMan = m_postManPSM;
            else if (IFacility.IsSecurityType(sensorType))
                postMan = m_postManSecurity;
            
            if (postMan == null)
                return false;

            if (!postMan.IsConnected)
                return false;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensor);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            if (bTest)
                postMan.SendMessage(SOPWebServer.Header.SENSOR_DATA_TEST, bytes);
            else
                postMan.SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);

            return true;
		}
        
        public void OnMessage(int header, byte[] messages, IPostMan postMan)
        {
            if (header == TCP_ID.ARE_YOU_THERE)
            {
                //SendData(TCP_ID.I_AM_HERE);
            }
            else if (header == TCP_ID.WHO_ARE_YOU)
            {
                //SendWhoIam();
            }
        }

        public void Close()
        {
            // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
            // 실패하더라도 상관없다.
            if (m_postManFire.IsConnected)
            {
                bool closeConnection;
                m_postManFire.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_postManFire.IsConnected = false;
            }

            if (m_postManPSM.IsConnected)
            {
                bool closeConnection;
                m_postManPSM.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_postManPSM.IsConnected = false;
            }

            if (m_postManSecurity.IsConnected)
            {
                bool closeConnection;
                m_postManSecurity.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_postManSecurity.IsConnected = false;
            }

            m_shutdownThread = true;
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
