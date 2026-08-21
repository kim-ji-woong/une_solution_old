using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using TcpLib2;
using SOPWebClient;
using DBUtility2;

namespace SVMSEventReciver
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
        private PostMan m_postManSecurity = null;

        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
            set { m_isConnected = value; }
        }
        private DateTime m_dtLastSendMessage = new DateTime();

        private bool m_shutdownThread = false;
        public bool ShutdownThread
        {
            get { return m_shutdownThread; }
            set { m_shutdownThread = value; }
        }
        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = -1;
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
		public NetworkWebClient(WebDBManager dbMgr, string strServerAddr, int nSiteID)
		{
            m_instance = this;
            this.m_nSiteID = nSiteID;
            m_dbMgr = dbMgr;

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			InitLog();

            m_postManFire = new PostMan(this, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.S1_SVMS);
            m_postManSecurity = new PostMan(this, SOPWebServer.ClientType.SECURITY_SENSOR_SERVER, SOPWebServer.ClientSubType.S1_SVMS);

            int nPort = ReadServerPort();
            SetPostBox(m_postManFire, nPort);
            SetPostBox(m_postManSecurity, nPort);
            
			Thread t = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t.Name = "Server Connection Thread";
			t.Start(m_postManFire);

            Thread t2 = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t2.Name = "Server Connection Thread";
            t2.Start(m_postManSecurity);
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

        private int ReadServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_nSiteID;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }
        
        public void OnMessage(int header, byte[] messages, IPostMan postMan)
        {
            PostMan _postMan = (PostMan)postMan;
            System.Diagnostics.Trace.WriteLine("OnMessage : " + header.ToString());

            ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

            if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                _postMan.IsConnected = false;
            }
            else if (header == SOPWebServer.Header.EDIT_SENSOR_ZONE)
                ProcessEditSensorZone(arrDatas);
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread(object arg)
        {
#if WIN
            FormMain.Instance.SetServerConnection(m_strServerAddr, false);
#endif
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

                if (m_postManFire.IsConnected && m_postManSecurity.IsConnected)
                    m_isConnected = true;
                else
                    m_isConnected = false;
            }
		}
        
        public void SendSensorData(Circuit circuit, int nData, int nTagHistoryID)
        {
            if (circuit != null)
            {
                int nCurcuit = circuit.TagNum;

                logger.Debug("[SOP서버로 회로 이름 " + circuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");

                //int nEquipzoneID = curcuit.TargetZoneID;
                int nSensorZoneID = circuit.SensorZone == null ? -1 : circuit.SensorZone.ID;

                int nTagNum = circuit.TagNum;
                int nSensorType = circuit.SensorType;
                //if(nSensorType == 6 && nSensorType == 9)
                {

                    SendSensorData(nSensorZoneID, circuit.ID, nSensorType, nData, "", nTagNum.ToString(), nTagHistoryID);
                }
                logger.Debug("[SensorType]" + nSensorType);
            }            
        }    

		public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, string szBuilding, string szTag, int nTagID)
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

                case Facility.FacilityType.Fire_S1:
                case Facility.FacilityType.Intrusion_S1:
                case Facility.FacilityType.Loiter_S1:
                case Facility.FacilityType.Slip_S1:
                case Facility.FacilityType.Steal_S1:
                case Facility.FacilityType.Abandoned_S1:
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
            }
            
            if (nSensor == -1)
                return false;

            if (nSensorZoneID < 0)
                return false;

            PostMan postMan = null;
            switch (sensorType)
            {
                case Facility.FacilityType.FIRE_SENSOR:
                case Facility.FacilityType.FireSensor_TypeA:
                case Facility.FacilityType.FireSensor_TypeB:
                case Facility.FacilityType.FireSensor_GasEmission:
                case Facility.FacilityType.FireSensor_ManualControl:
                case Facility.FacilityType.Fire_S1:
                case Facility.FacilityType.FireF1_S1:
                case Facility.FacilityType.FireSensor_SiemensType:
                case Facility.FacilityType.FireSensor_AnalogSmokeType:
                    postMan = m_postManFire;
                    break;
                    
                case Facility.FacilityType.Intrusion_S1:
                case Facility.FacilityType.Loiter_S1:
                case Facility.FacilityType.Slip_S1:
                case Facility.FacilityType.Steal_S1:
                case Facility.FacilityType.Abandoned_S1:
                case Facility.FacilityType.VirtualFence_S1:
                case Facility.FacilityType.EmergencyBell_S1:
                case Facility.FacilityType.GeneralIntrusionT1_S1:
                case Facility.FacilityType.GeneralIntrusionT2_S1:
                case Facility.FacilityType.InternalIntrusionT3_S1:
                case Facility.FacilityType.VaultIntrusionT4_S1:                
                case Facility.FacilityType.CustomerEmergencyC1_S1:
                case Facility.FacilityType.CustomerEmergencyC2_S1:
                case Facility.FacilityType.RescueQQ_S1:
                case Facility.FacilityType.GasG1_S1:
                case Facility.FacilityType.BlackoutAbnormalityU1_S1:
                case Facility.FacilityType.LeakAbnormalityU4_S1:
                case Facility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    postMan = m_postManSecurity;
                    break;
            }

            if (postMan == null)
                return false;

            if (!postMan.IsConnected)
                return false;
            
            logger.Debug("[SendSensorType]" + nSensorType);

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensor);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);
            arrDatas.Add(nTagID);
            
            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);            
            postMan.SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
            return true;
		}

        private void ProcessEditSensorZone(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount % 4 != 0)
                return;

            for (int i = 0; i < nDataCount; i += 4)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int && arrDatas[i + 2] is int && arrDatas[i + 3] is int)
                {
                    int nSensorZoneID = (int)arrDatas[i];
                    int nOriginEquipZoneID = (int)arrDatas[i + 1];
                    int nChangedEquipZoneID = (int)arrDatas[i + 2];
                    int nZoneID = (int)arrDatas[i + 3];

                    SensorZone sensorZone = SVMSEventReciver.Instance.IOManager.GetSensorZone(nSensorZoneID);

                    if (sensorZone == null)
                        continue;

                    EquipmentZone equipZoneOrigin = SVMSEventReciver.Instance.IOManager.GetEquipmentZone(nOriginEquipZoneID);
                    EquipmentZone equipZoneChanged = SVMSEventReciver.Instance.IOManager.GetEquipmentZone(nChangedEquipZoneID);

                    if (equipZoneOrigin != null)
                    {
                        ArrayList arrSensorZones;

                        if (SVMSEventReciver.Instance.IOManager.D_EquipZoneSensor.TryGetValue(equipZoneOrigin, out arrSensorZones))
                        {
                            arrSensorZones.Remove(sensorZone);
                        }
                    }

                    if (equipZoneChanged != null)
                    {
                        ArrayList arrSensorZones;

                        if (!SVMSEventReciver.Instance.IOManager.D_EquipZoneSensor.TryGetValue(equipZoneChanged, out arrSensorZones))
                        {
                            arrSensorZones = new ArrayList();
                            SVMSEventReciver.Instance.IOManager.D_EquipZoneSensor[equipZoneChanged] = arrSensorZones;
                        }

                        if (!arrSensorZones.Contains(sensorZone))
                            arrSensorZones.Add(sensorZone);
                    }

                    sensorZone.EquipZone = equipZoneChanged; 
                }
            }
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
