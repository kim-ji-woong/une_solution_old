using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using DBUtility2;
using System.Collections;
using System.Threading;

namespace SensorMonitor
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private bool m_isConnected = false;
        private DateTime m_dtLastSendMessage = new DateTime();

        private int m_nClientType = SOPWebServer.ClientType.FIRE_SENSOR_SERVER;
        private int m_nClientSubType = SOPWebServer.ClientSubType.TH;

        private bool m_shutdownThread = true;
        private WebDBManager m_dbMgr = null;

        private bool m_shutdownSensorThread = true;
        private int m_nSensorThreadCount = 0;
        private int m_bChangedCount = 0;

        private log4net.ILog logger = null;

        // 전체 FireReciverProvider
        private List<FireReciverProvider> m_arReicverProvider = new List<FireReciverProvider>();

        // 각 FireReciver에 대한 State정보
        //private Dictionary<int, ReciverState> m_dicStateList = new Dictionary<int, ReciverState>();

        public PostBox PostBox
        {
            get { return m_postBox; }
        }

        private void WriteLog(string strLog)
        {
            logger.Debug(strLog);
        }

        public NetworkWebManager(WebDBManager dbMgr)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_dbMgr = dbMgr;
            int nPort = ReadServerPort(m_dbMgr);

            SetPostBox(nPort);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
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

        public void OnMessage(int header, byte[] messages)
        {
        }

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
                            InitReceiverState();
                            m_isConnected = true;
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

                Thread.Sleep(1000);
            }
        }

        // 서버에게 수신반 상태를 아무것도 보내지 않은 상태로 만든다.
        private void InitReceiverState()
        {
            ArrayList arReciverList = (ArrayList)SOPMonitor.Instance.IoMgr.GetReciverList().Clone();
            if (arReciverList == null)
                return;

            foreach (Reciver receiver in arReciverList)
            {
                receiver.PrevConnected = -1;
                receiver.PrevPoll = -1;
            }
        }

        private void ReceiverCheckThread()
        {
            // 먼저 실행중인 쓰레드가 있으면 종료시킨다.
            while (m_nSensorThreadCount > 0)
            {
                m_shutdownSensorThread = true;
                Thread.Sleep(500);
            }

            ArrayList arReciverList = SOPMonitor.Instance.IoMgr.GetReciverList();
            arReciverList.Reverse();

            if (arReciverList.Count == 0)
                return;

            m_shutdownSensorThread = false;
            m_nSensorThreadCount++;

            List<Reciver> changedReceivers = new List<Reciver>();

            while (!m_shutdownSensorThread)
            {
                changedReceivers.Clear();

                if (m_isConnected)
                {
                    foreach (Reciver receiver in arReciverList)
                    {
                        int nCon = receiver.IsConnected ? 1 : 0;
                        int nPol = receiver.RecivedPoll ? 1 : 0;

                        if (receiver.PrevConnected < 0 || receiver.PrevPoll < 0 || nCon != receiver.PrevConnected || nPol != receiver.PrevPoll)
                        {
                            changedReceivers.Add(receiver);
#if !SERVICE
                            if (receiver.IsConnected)
                                FormMain.Instance.OnConnectReciver(receiver.ID);
                            else
                                FormMain.Instance.OnDisconnectReciver(receiver.ID);
#endif
                        }
                    }

                    if (changedReceivers.Count > 0)
                        SendAllReceiverState(changedReceivers);
                }

                Thread.Sleep(1000);
            }

            m_nSensorThreadCount--;
        }

        /*private void ReciverCheckThread()
        {
            // 먼저 실행중인 쓰레드가 있으면 종료시킨다.
            while (m_nSensorThreadCount > 0)
            {
                m_shutdownSensorThread = true;
                Thread.Sleep(500);
            }

            m_dicStateList.Clear();
            m_shutdownSensorThread = false;
            m_nSensorThreadCount++;

            ArrayList arReciverList = SOPMonitor.Instance.IoMgr.GetReciverList();
            arReciverList.Reverse();

            int nStart = 0;
            int nCount = arReciverList.Count;

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

            while (!m_shutdownSensorThread)
            {
                if (m_dicStateList.Count > 0)
                {
                    bool isChangedData = false;
                    foreach (KeyValuePair<int, ReciverState> pair in m_dicStateList)
                    {
                        ReciverState state = pair.Value;

                        if (state.Connected != state.TargetReciver.IsConnected)
                        {
                            state.Connected = state.TargetReciver.IsConnected;
                            isChangedData = true;
                        }
#if !SERVICE
                        if (state.Connected == true)
                            FormMain.Instance.OnConnectReciver(state.ID);
                        else
                            FormMain.Instance.OnDisconnectReciver(state.ID);
#endif
                    }

                    bool sendReceiverState = false;

                    if (isChangedData == true)
                    {
                        m_bChangedCount++;

                        if (m_bChangedCount == 3)
                        {
                            m_bChangedCount = 0;
                            SendAllReciverState();
                            sendReceiverState = true;
                        }
                    }

                    DateTime dtNow = DateTime.Now;
                    TimeSpan span = dtNow - lastTime;
                    if (span.TotalMinutes > 3.0)
                    {
                        if (sendReceiverState == false)
                            SendAllReciverState();

                        lastTime = DateTime.Now;
                    }

                    for (int i = 0; i < 300; i++)
                    {
                        if (!m_shutdownSensorThread)
                            Thread.Sleep(100);
                        else
                            break;
                    }
                }
            }

            m_nSensorThreadCount--;
        }*/

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

        public void Close()
        {
            if (m_isConnected)
            {
                // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                // 실패하더라도 상관없다.
                bool closeConnection;
                m_postBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_isConnected = false;
            }

            m_shutdownThread = true;
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
            m_shutdownSensorThread = true;
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

            // Get Reciver List
            ArrayList arReciverList = SOPMonitor.Instance.IoMgr.GetReciverList();
            if (arReciverList != null)
            {
                string szPath = System.Reflection.Assembly.GetEntryAssembly().FullName;
                for (int i = 0; i < arReciverList.Count; i++)
                {
                    Reciver reciver = (Reciver)arReciverList[i];
                    FireReciverProvider provider = new FireReciverProvider(this, reciver);

                    try
                    {
                        provider.BeginServer();
                    }
                    catch (Exception)
                    {

                    }

                    m_arReicverProvider.Add(provider);
                }
            }

            Thread tt = new Thread(new ThreadStart(ReceiverCheckThread));
            tt.Name = "ReciverStateChecker";
            tt.Start();
        }

        // 수신반 상태가 바뀐 것들만 전송한다.
        public void SendAllReceiverState(List<Reciver> changedReceivers)
        {
            if (!m_isConnected)
                return;

            ArrayList arrDatas = new ArrayList();
            
            foreach (Reciver receiver in changedReceivers)
            {
                int nCon = receiver.IsConnected == true ? 1 : 0;
                int nPol = receiver.IsConnected == true ? 10 : 0;
                //int nPol = receiver.RecivedPoll == true ? 10 : 0;

                arrDatas.Add(receiver.ID);
                //arrDatas.Add(nCon);
                //arrDatas.Add(nPol);
                arrDatas.Add(nPol + nCon);

                string strLog = string.Format("Receiver[{0}] : {1}", receiver.Address, nPol + nCon);
                System.Diagnostics.Trace.WriteLine(strLog);
            }

            if (arrDatas.Count > 0)
            {
                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

                if (SendMessage(SOPWebServer.Header.ALL_RECEIVER_STATE, bytes))
                {
                    // 전송에 성공하면 Prev 상태를 갱신해준다.
                    foreach (Reciver receiver in changedReceivers)
                    {
                        receiver.PrevConnected = receiver.IsConnected ? 1 : 0;
                        receiver.PrevPoll = receiver.RecivedPoll ? 1 : 0;
                    }
                }
            }
        }

        // 모든 수신반 상태를 전송한다.
        /*public void SendAllReciverState()
        {
            if (!m_isConnected)
                return;

            ArrayList arReciverList = (ArrayList)SOPMonitor.Instance.IoMgr.GetReciverList().Clone();
            if (arReciverList == null)
                return;

            int nReceiverCount = arReciverList.Count;

            arReciverList.Reverse();
            ArrayList arrDatas = new ArrayList();

            for (int i = 0; i < nReceiverCount; i++)
            {
                Reciver receiver = (Reciver)arReciverList[i];

                int nCon = receiver.IsConnected == true ? 1 : 0;
                int nPol = receiver.RecivedPoll == true ? 10 : 0;

                arrDatas.Add(receiver.ID);
                arrDatas.Add(nCon);
                arrDatas.Add(nPol);
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.ALL_RECEIVER_STATE, bytes);
        }*/

        public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, string szBuilding, string szTag)
        {
            // SOP서버로 연결된 Provider로 전송
            if (m_isConnected == false)
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
    }
}
