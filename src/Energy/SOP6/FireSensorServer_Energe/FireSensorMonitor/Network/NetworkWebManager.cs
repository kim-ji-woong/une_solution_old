using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using TcpLib2;
using DBUtility2;
using SOPWebClient;

namespace SensorMonitor
{
	public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private bool m_isConnected = false;
        private DateTime m_dtLastSendMessage = new DateTime();

        private int m_nClientType = SOPWebServer.ClientType.FIRE_SENSOR_SERVER;
        private int m_nClientSubType = SOPWebServer.ClientSubType.JOHNSON_CONTROLS;

        private bool m_shutdownThread = false;
		private WebDBManager m_dbMgr = null;
        
        // 전체 FireReciverProvider
        private ArrayList m_arReicverProvider = new ArrayList();

        // 각 FireReciver에 대한 State정보
        //private Dictionary<int, ReciverState> m_dicStateList = new Dictionary<int, ReciverState>();
        
        private bool shutdownSensorThread = false;
        public bool ShutdownSensorThread
        {
            get { return shutdownSensorThread; }
            set { shutdownSensorThread = value; }
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
        
        public NetworkWebManager(WebDBManager dbMgr)
		{
			InitLog();  

			m_dbMgr = dbMgr;

            int nPort = ReadServerPort(m_dbMgr);
            SetPostBox(nPort);

            Thread t = new Thread(ConnectionThread);
            t.Name = "Server Connection Thread";
			t.Start();			
		}
        private void SetPostBox(int nPort)
        {
            m_postBox = new PostBox();
            m_postBox.WebServerURL = m_dbMgr.WebServerURL;
            m_postBox.Port = nPort;
            m_postBox.PostMan = this;
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

        public void ReleaseThread()
		{
			m_shutdownThread = true;
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

            /*Thread tt = new Thread(ReciverCheckThread);
            tt.Name = "ReciverStateChecker";
            tt.Start(this);*/
		}


        /*private int m_bChangedCount = 0;
		private void ReciverCheckThread(object p)
		{
            NetworkWebManager manager = (NetworkWebManager)p;
			
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

			while (!m_shutdownThread)
			{                
				if (manager != null && m_dicStateList.Count > 0)
				{
					if (!m_shutdownThread)
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
                            //if (m_bChangedCount == 3)
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
							if (!m_shutdownThread)
								Thread.Sleep(100);
							else
								break;
						}
					}
				}
			}
		}*/

		// 서버와의 접속이 끊어지면 다시 연결시킨다.
		private void ConnectionThread()
		{
            m_shutdownThread = false;

            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort(m_dbMgr);

                    if (m_postBox != null && m_postBox.Port != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(m_nClientType, m_nClientSubType))
                        {
                            m_isConnected = true;
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
                        SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                    }
                }

                if (m_isConnected)
                    CheckReceiverStatus();

                Thread.Sleep(3000);
            }
        }

        public bool SendMessage(int header, byte[] messages)
        {
            if (m_postBox == null || m_isConnected == false)
            {
                m_isConnected = false;
            }
            else
            {
                bool closeConnection;
                bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                if (closeConnection)
                {
                    WriteLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else if (result == true)
                {
                    m_dtLastSendMessage = DateTime.Now;
                    WriteSendLog(header, messages);
                }

                return result;
            }

            return false;
        }

        private void WriteSendLog(int header, byte[] bytes)
        {
            if (header == SOPWebServer.Header.ARE_YOU_THERE)
                return;

            string strLog = string.Format("SendMessage : Header({0}), Length({1})", header, (int)bytes.Length);
            string strBytes = "";

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];

                if (strBytes.Length == 0)
                    strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                else
                    strBytes += string.Format(" {0:X2}", (int)b);
            }

            WriteLog(strLog + strBytes);
        }

        private Dictionary<Reciver, int> m_dicReceiverStatus = new Dictionary<Reciver, int>();

        private void CheckReceiverStatus()
        {
            ArrayList arReciverList = (ArrayList)SOPMonitor.Instance.IoMgr.GetReciverList().Clone();
            if (arReciverList == null)
                return;

            if (arReciverList != null)
            {
                int nStatus;
                bool changed = false;

                for (int i = 0; i < arReciverList.Count; i++)
                {
                    Reciver reciver = (Reciver)arReciverList[i];

                    int nCon = reciver.IsConnected == true ? 1 : 0;
                    int nPol = reciver.IsConnected == true ? 10 : 0;
                    //int nPol = reciver.RecivedPoll == true ? 10 : 0;

                    nCon += nPol;

                    if (m_dicReceiverStatus.TryGetValue(reciver, out nStatus) == false)
                    {
                        changed = true;
                        break;
                    }

                    if (nStatus != nCon)
                    {
                        changed = true;
                        break;
                    }
                }

                if (changed)
                    SendAllReciverState();
            }
        }

        public void SendAllReciverState()
		{
			if (!m_isConnected)
				return;

			ArrayList arReciverList = (ArrayList)SOPMonitor.Instance.IoMgr.GetReciverList().Clone();
			if (arReciverList == null)
				return;            
            
            ArrayList arrDatas = new ArrayList();

            if (arReciverList != null)
			{
                arReciverList.Reverse();
                
                for (int i = 0; i < arReciverList.Count; i++)
                {
                    Reciver reciver = (Reciver)arReciverList[i];

                    int nCon = reciver.IsConnected == true ? 1 : 0;
                    int nPol = reciver.IsConnected == true ? 10 : 0;
                    //int nPol = reciver.RecivedPoll == true ? 10 : 0;

                    nCon += nPol;

                    arrDatas.Add(reciver.ID);
                    arrDatas.Add(nCon);

                    m_dicReceiverStatus[reciver] = nCon;
                }

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(SOPWebServer.Header.ALL_RECEIVER_STATE, bytes);
            }
		}

        public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, string szBuilding, string szTag, bool bPSM = false)
        {
            if (!m_isConnected)
                return false;

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

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensor);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
        }

		private void CopyBytes(byte[] bytesDest, ref int nDestOffset, byte[] bytesSrc)
		{
			int nLength = bytesSrc.Length;
			System.Buffer.BlockCopy(bytesSrc, 0, bytesDest, nDestOffset, nLength);
			nDestOffset += nLength;
		}

        public void OnMessage(int header, byte[] messages)
        {
            
        }

        public void SendConnectionState(int nReceiverID, bool isConnected)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nReceiverID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            int cmd = isConnected ? SOPWebServer.Header.RECEIVER_CONNECT : SOPWebServer.Header.RECEIVER_DISCONNECT;
            SendMessage(cmd, bytes);
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
