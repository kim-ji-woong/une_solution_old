using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDMS;
using System.Collections;
using System.Threading;

namespace SDMSServer
{
    public class ClientDataSensorSimulator : ClientData
    {
        private class QueueData
        {
            private TcpLib2.ConnectionState m_state = null;
            private byte[] m_bytes = null;
            private int m_nHeader = 0;
            private ArrayList m_arrDatas = null;

            public TcpLib2.ConnectionState State
            {
                get { return m_state; }
            }

            public byte[] Bytes
            {
                get { return m_bytes; }
            }

            public int Header
            {
                get { return m_nHeader; }
            }

            public ArrayList Datas
            {
                get { return m_arrDatas; }
            }

            public QueueData()
            {
            }

            public QueueData(TcpLib2.ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
            {
                m_state = state;
                m_bytes = bytes;
                m_nHeader = nHeader;
                m_arrDatas = arrDatas;
            }
        }

        private static ArrayList m_arrMessageQueue = new ArrayList();
        private static bool m_runThread = false;

        public static bool RunThread
        {
            get { return m_runThread; }
            set
            {
                if (m_runThread != value)
                {
                    m_runThread = value;

                    if (m_runThread)
                    {
                        Thread t = new Thread(ReceiveThread);
                        t.Start();
                    }
                }
            }
        }

        public ClientDataSensorSimulator(ServiceProvider provider)
        {
            m_provider = provider;
            Type = ClientType.SENSOR_SIMULATOR;
        }

        protected override bool OnReceive(TcpLib2.ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            RunThread = true;

            QueueData data = new QueueData(state, bytes, nHeader, arrDatas);
            m_arrMessageQueue.Add(data);

            return true;
        }

        protected bool _OnReceive(TcpLib2.ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.SENSOR_DATA)
            {
                int nSensorID, data, nPrevSensorHistoryID = -1;
                bool connected = false;
                int nHistoryID = NetworkServer.Instance.SensorManager.ProcessSensorData(bytes, out nSensorID, out data, out connected, ref nPrevSensorHistoryID);
                PostProcessSensorData(nHistoryID, nPrevSensorHistoryID, nSensorID, data, connected);
            }

            return true;
        }

        static bool bClient = true;
        private void PostProcessSensorData(int nHistoryID, int nPrevSensorHistoryID, int nSensorID, int nData, bool bConnected)
        {
            // comment by skkim : AbnormalSensorManager에서 대행
            // 임시로 무시된 Sensor List에서 해제할 것이 있는지 검사
            if (nSensorID > 0 && nData == 0)
            {
                m_provider.RemoveTempIgnoreSensor(nSensorID);
            }

            // Connection만 변경되는 경우 리턴값이 -2임
            if (nHistoryID == -2)
            {
                m_provider.SendSensorZoneData(nData, nSensorID, ClientData.ClientType.SDMS_CLIENT);
            }

            if (nData == 1 && nHistoryID != -1)
            {
                if (!m_provider.CheckSituation(nHistoryID))
                {
                    TimeHistory hs = new TimeHistory(nHistoryID, DateTime.Now);
                    m_provider.AddTimeHistory(hs);
                    PingCount = 0;
                    m_provider.SendSensorZoneData(nData, nSensorID, ClientData.ClientType.SDMS_CLIENT);
                    PingCount = 0;
                    SensorReactionLog log = CreateFireDetect(nHistoryID, nSensorID);
                    m_provider.AddReactionLog(log);

                    // 사내방송 실시 - 신호에 대해 현장확인 후 방송 보내도록 함(삼천포:김명수대리요청)
                    // 2013-12-18
                    ClientDataSDMS.RunBroadcast(log, m_provider, BroadcastManager.SituationType.DETECT_FIRE);
                    m_provider.SendSMS(log);

                    // Send Reaction Log
                    m_provider.SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT_SECOND);

                    hs.LastReactionLog = log;

                    m_provider.MonitorDetectFireProcess(log);
                }
            }
            else if (nData == 0 && nHistoryID != -1)
            {
                if (nPrevSensorHistoryID > 0)
                {
                    TimeHistory history = m_provider.FindTimeHistory(nPrevSensorHistoryID);

                    if (history != null && history.LastReactionLog != null/* && history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS*/)
                    {
                        ClientData.ClientType nClientType = (bClient == true ? ClientData.ClientType.SDMS_CLIENT : ClientData.ClientType.SDMS_CLIENT_SECOND);
                        bClient = !bClient;
                        Thread.Sleep(5);
                        PingCount = 0;

                        m_provider.SendSensorZoneData(nData, nSensorID, nClientType);

                        PingCount = 0;
                        // 화재 상황 종료
                        Thread.Sleep(5);
                        nClientType = (bClient == true ? ClientData.ClientType.SDMS_CLIENT : ClientData.ClientType.SDMS_CLIENT_SECOND);
                        bClient = !bClient;

                        m_provider.SendClearDetectReport(nPrevSensorHistoryID, nClientType);

                        Thread.Sleep(5);
                        m_provider.RemoveTimeHistory(history);
                        m_provider.RemoveSituation(nHistoryID, false);

                        SensorManager.Instance.RemoveSensorHistory(nPrevSensorHistoryID);
                        SensorManager.Instance.RemoveSensorHistory(nHistoryID);

                        if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS)
                        {
                            SensorReactionLog log = new SensorReactionLog();

                            log.LogTime = DateTime.Now;
                            log.Message = "화재 신호가 무시되었습니다.";
                            log.SensorHistoryID = nPrevSensorHistoryID;
                            log.Type = SensorReactionLog.ReactionType.IGNORE_FIRE;

                            m_provider.AddReactionLog(log);
                        }
                    }
                }
            }
        }

        private SensorReactionLog CreateFireDetect(int nHistoryID, int nSensorID)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;
            log.Type = SensorReactionLog.ReactionType.BEGIN_STATUS;

            int nEquipZoneID = SensorManager.Instance.GetSensorZone(nSensorID);
            log.Message = GetFireDetectString(nEquipZoneID);

            if (nEquipZoneID >= 0)
                log.Param1 = nEquipZoneID.ToString();

            log.Param2 = nSensorID.ToString();

            return log;
        }

        public static string GetFireDetectString(int nEquipZoneID)
        {
            if (nEquipZoneID == -1)
            {
                return "화재가 탐지 되었습니다";
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szZoneName = equipZone.BroadcastName;
                    return string.Format("[{0}]에서 화재가 탐지 되었습니다", szZoneName);
                }
            }

            return "";
        }

        static int m_nThreadCount = 0;
        protected static void ReceiveThread()
        {
            System.Diagnostics.Trace.WriteLine(string.Format("ReceiveThread Count : {0}", ++m_nThreadCount));

            while (m_runThread && !NetworkServer.Instance.FinishProcess)
            {
                while (m_arrMessageQueue.Count > 0)
                {
                    QueueData data = (QueueData)m_arrMessageQueue[0];

                    if (data.State != null && data.State.Tag != null)
                    {
                        ClientDataSensorSimulator client = (ClientDataSensorSimulator)data.State.Tag;
                        client._OnReceive(data.State, data.Bytes, data.Header, data.Datas);
                    }

                    m_arrMessageQueue.RemoveAt(0);

                    if (!m_runThread || NetworkServer.Instance.FinishProcess)
                        return;
                }

                Thread.Sleep(50);
            }
        }
    }
}
