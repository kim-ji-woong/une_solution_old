using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SOPWebClient;
using System.Collections;
using System.Threading;
using DBUtility2;
using System.Collections.Concurrent;

namespace SOPWebAPI.Network
{
    using Models;

    public class NetworkWebManager
    {
        // libSensorProcess2.dll은 참조할 수 없다.
        // Geometrydn이 C++ 버전이기 때문이다.
        public enum ReactionType
        {
            BEGIN_STATUS = 0,              // 상황 시작
            RUN_BROADCAST = 10,            // 사내 방송 실시         
            SEND_SMS = 11,                 // 문자메시지 발송
            MALFUNCTION = 21,              // 오작동 처리
            NOTIFY_SIGNAL = 22,            // 재난 신고
            IGNORE_SIGNAL = 23,            // 재난 탐지신호 무시
                                           //TRAINNING_FIRE = 24,         // 
            RUN_SOP = 30,                  // SOP 발동 
            RUN_N_CANCEL_SOP = 31,         // SOP 실행중 취소
            FINISH_SOP = 32,               // SOP 종료
            IGNORE_SOP = 33,               // SOP 실행 안함
            END_STATUS = 50,               // 상황 종료
                                           //BEGIN_PSM_STATUS = 60,
                                           //IGNORE_PSM_DETECT = 61,
            CHANGE_ALARM_DEPTH = 62,
            //NOTIFY_PSM = 63,
            USER_RESET = 64,
            //END_PSM_STATUS = 70,
            ETC = 100,                     // 기타
            RUN_DETECT_BROADCAST = 101,
            RUN_REPORT_BROADCAST = 102,
            SEND_DETECT_SMS = 111,
            SEND_REPORT_SMS = 112,
            SEND_MALFUNCTION_SMS = 113,
            SEND_REPAIR_SMS = 114,

            /*NOTIFY_EARTHQUAKE = 200,

            NOTIFY_SECURITY = 898,
            BEGIN_S1SVMS_STATUS = 899,
            IGNORE_S1SVMS_STATUS = 919,
            END_S1SVMS_STATUS = 920,


            BEGIN_S1ACCESS_STATUS = 921,
            IGNORE_S1ACCESS_STATUS = 939,
            END_S1ACCESS_STATUS = 940,


            BEGIN_SECOM_STATUS = 961,
            IGNORE_SECOM_STATUS = 969,
            END_SECOM_STATUS = 970,*/

            TIME_OUT = 1000
        }

        // libData2.dll은 참조할 수 없다.
        // Geometrydn이 C++ 버전이기 때문이다.
        public enum FacilityType
        {
            NONE = -1,
            FIRE_SENSOR = 0,        // 화재탐지센서(100번 ~ 199번)
            COOLER_SENSOR = 1,      // 스프링쿨러
            PRESSURE_SENSOR = 2,    // 펌프압력센서
            CCTV = 3,
            FE = 4,                 // 소화기(Fire Extinguisher)
            HD = 5,                 // 소화전(Hydrant)
            FA = 6,                 // 발신기(Fire Alarm)
            FR = 7,                 // 수신반(Fire Receiver)
            PSM_SENSOR = 11,        // 유해화학물질 누출감지 센서
            DISASTER_PREVENTION_EQUIPMENT = 12, // 방재장비
            AIR_QUAILITY = 13,                  // 공기질 센서
            TEMPERATURE_HUMIDITY = 14,          // 온도/습도 센서
            FIREWALL = 15,                      // 방화벽
            DOOR = 16,                          // 출입문
            BLACKOUT = 17,                      // 정전
            STRONG_WIND = 18,                   // 강풍
            SUBMERGENCY = 19,                   // 침수
            TERROR = 20,                        // 테러
            Earthquake = 50,                    // 지진 센서
            FireSensor_TypeA = 101,             // 화재감지기 A
            FireSensor_TypeB = 102,             // 화재감지기 B
            FireSensor_GasEmission = 103,       // 가스 방출신호
            FireSensor_ManualControl = 104,     // 수동조작함 신호
            FireSensor_LightType = 105,         // 광선식
            FireSensor_SiemensType = 106,       // 지멘스 자탐
            FireSensor_Monitoring = 107,        // 감시
            FireSensor_SensingLine = 108,       // 감지선
            FireSensor_AnalogSmokeType = 109,   // 아날로그식 연기
            FireSensor_MonitoringType = 110,     // 감시센서

            Security_Sensor = 899,              // 방범센서
            // 서울대학교 e재난 시스템 - S1시스템 통합으로 추가됨
            // skkim     2017-03-14
            Intrusion_S1 = 900,                    // SVMS 침입
            Loiter_S1 = 901,                       // SVMS 배회
            Collapse_S1 = 902,                     // SVMS 쓰러짐
            Theft_S1 = 903,                        // SVMS 도난
            Neglect_S1 = 904,                      // SVMS 방치
            VirtualFence_S1 = 905,                 // SVMS 가상펜스
            Fire_S1 = 906,                         // SVMS 화재
            EmergencyBell_S1 = 907,                // SVMS 비상벨
            GeneralIntrusionT1_S1 = 1001,          // S1Access 일반침입1
            GeneralIntrusionT2_S1 = 1002,          // S1Access 일반 침입2
            InternalIntrusionT3_S1 = 1003,         // S1Access 내부침입
            VaultIntrusionT4_S1 = 1004,            // S1Access 금고침입
            FireF1_S1 = 2000,                      // S1Access 화재
            CustomerEmergencyC1_S1 = 2100,         // S1Access 고객비상
            CustomerEmergencyC2_S1 = 2110,         // S1Access 고객 비상
            RescueQQ_S1 = 2200,                    // S1Access 구급
            GasG1_S1 = 2300,                       // S1Access 가스
            BlackoutAbnormalityU1_S1 = 3000,       // S1Access 정전이상
            LeakAbnormalityU4_S1 = 3004,           // S1Access 누수이상
            SynthesisAlertAbnormalityU8_S1 = 3008, // S1Access 종합경보반 이상
            ExternalAlarmBell = 4000,              // 외부 비상벨

            SecomFire = 5000,                       // SECOM 화재
            SecomExternalAlarmBell = 5001,          // SECOM 외부 비상벨
            SecomWomenAlarmBell = 5002              // SECOM 여자화장실 비상벨
        };

        private class PostMan : IPostMan
        {
            private PostBox m_postBox = null;
            private NetworkWebManager m_owner = null;
            private int m_nClientType = -1;
            private int m_nClientSubType = -1;
            private bool m_isConnected = false;
            private int m_nPort = -1;
            private DateTime m_dtLastSendMessage = new DateTime();

            public PostBox PostBox
            {
                get { return m_postBox; }
                set
                {
                    m_postBox = value;
                }
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
                set
                {
                    if (m_isConnected != value)
                    {
                        m_isConnected = value;
                    }
                }
            }

            public int Port
            {
                get { return m_nPort; }
                set { m_nPort = value; }
            }

            public DateTime LastSendMessageTime
            {
                get { return m_dtLastSendMessage; }
            }

            public PostMan(NetworkWebManager owner, int nClientType, int nClientSubType)
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
                    m_isConnected = false;
                }
                else
                {
                    bool closeConnection;
                    bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                    if (closeConnection)
                    {
                        if (m_owner != null)
                            m_owner.WriteLog(m_postBox.ErrorMessage);

                        m_isConnected = false;
                    }
                    else
                        m_dtLastSendMessage = DateTime.Now;

                    return result;
                }

                return false;
            }
        }

        private PostMan m_postManFire = null;
        private PostMan m_postManSDMS = null;
        private List<PostMan> m_postManList = new List<PostMan>();
        private bool m_shutdownThread = false;
        private WebDBManager m_dbMgr = null;

        // Key : SensorZone ID
        private ConcurrentDictionary<int, FireAlarm> m_dicSensorZoneFireAlarms = new ConcurrentDictionary<int, FireAlarm>();
        // Key : SensorZoneHistory ID
        private ConcurrentDictionary<int, FireAlarm> m_dicSensorZoneHistoryFireAlarms = new ConcurrentDictionary<int, FireAlarm>();
        private ConcurrentQueue<FireAlarm> m_queueAlarms = new ConcurrentQueue<FireAlarm>();

        private static NetworkWebManager m_instance = null;

        public static NetworkWebManager Instance
        {
            get { return m_instance; }
        }

        private NetworkWebManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;

            // SOPWebAPI를 통한 SOPWebServer 통신 기능은 AutoUpdater로 이관한다.
            /*int nPort = ReadServerPort();

            m_postManFire = new PostMan(this, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.SKT_DT);
            m_postManSDMS = new PostMan(this, SOPWebServer.ClientType.SDMS, SOPWebServer.ClientSubType.SKT_DT);

            SetPostBox(m_postManFire, nPort);
            SetPostBox(m_postManSDMS, nPort);

            m_postManList.Add(m_postManFire);
            m_postManList.Add(m_postManSDMS);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();*/
        }

        public static void InitInstance()
        {
            m_instance = new NetworkWebManager(DataManager.Instance.DBManager);
        }

        public void OnMessage(int header, byte[] messages, object postMan)
        {
            // SOPWebServer와 더 이상 통신하지 않는다.
            /*if (postMan != null && postMan is PostMan)
            {
                ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

                RecvLog(header, messages);

                if (header == SOPWebServer.Header.CLOSE_CONNECTION)
                {
                    ((PostMan)postMan).IsConnected = false;
                }
                else if (header == SOPWebServer.Header.ARE_YOU_THERE)
                {
                    ((PostMan)postMan).SendMessage(SOPWebServer.Header.I_AM_HERE, null);
                }
                else if (header == SOPWebServer.Header.SENSOR_REACTION_HISTORY_DATA)
                {
                    ProcessSensorReactionSensorHistoryData(arrDatas);
                }
                else if (header == SOPWebServer.Header.SENSOR_REACTION_HISTORY_DATA_LIST)
                    ProcessReactionHistoryLogList(arrDatas);
            }*/
        }

        //private void ConnectionThread()
        //{
        //    List<FireAlarm> unprocessedAlarms = new List<FireAlarm>();

        //    while (m_shutdownThread == false)
        //    {
        //        foreach (PostMan postMan in m_postManList)
        //        {
        //            if (postMan.IsConnected == false)
        //            {
        //                int nPort = ReadServerPort();

        //                if (postMan.Port != nPort)
        //                    SetPostBox(postMan, nPort);

        //                if (postMan.PostBox != null)
        //                {
        //                    if (postMan.PostBox.Connect(postMan.ClientType, postMan.ClientSubType))
        //                    {
        //                        postMan.IsConnected = true;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                TimeSpan span = DateTime.Now - postMan.LastSendMessageTime;

        //                // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
        //                if (span.TotalSeconds > 3.0)
        //                {
        //                    // 접속이 유지되고 있는지 확인한다.
        //                    postMan.SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
        //                }
        //            }
        //        }

        //        while (m_queueAlarms.Count > 0)
        //        {
        //            FireAlarm alarm;

        //            if (m_queueAlarms.TryDequeue(out alarm) == false)
        //                break;

        //            if (m_postManFire.IsConnected)
        //                SendFireSensorEvent(alarm);
        //            else
        //                unprocessedAlarms.Add(alarm);
        //        }

        //        foreach (FireAlarm alarm in unprocessedAlarms)
        //        {
        //            m_queueAlarms.Enqueue(alarm);
        //        }

        //        unprocessedAlarms.Clear();
        //        Thread.Sleep(1000);
        //    }
        //}

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

        private bool SendMessage(int header, byte[] messages, PostMan postMan)
        {
            if (postMan.IsConnected)
            {
                SendLog(header, messages);
                return postMan.SendMessage(header, messages);
            }

            return false;
        }

        public void RecvLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "RecvMessage");
        }

        private void SendLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "SendMessage");
        }

        private void MessageLog(int header, byte[] bytes, string strMessageTag)
        {
            if (header != SOPWebServer.Header.ARE_YOU_THERE &&
                header != SOPWebServer.Header.I_AM_HERE)
            {
                string strLog = "";

                if (bytes == null)
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length(0)", header);
                }
                else
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length({1})", header, bytes.Length);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    strLog += strBytes;
                }

                WriteLog(strLog);
            }
        }

        public void WriteLog(string strLog)
        {
            //if (m_logger != null)
            //    m_logger.Write(strLog);
        }

        public void Close()
        {
            foreach (PostMan postMan in m_postManList)
            {
                if (postMan.IsConnected)
                {
                    // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                    // 실패하더라도 상관없다.
                    bool closeConnection;
                    postMan.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                    postMan.IsConnected = false;
                }
            }

            m_shutdownThread = true;
        }

        //private bool SendFireSensorEvent(FireAlarm alarm)
        //{
        //    int nSensorZoneID = alarm.SensorZoneID, nSensorTagID = alarm.SensorTagID;

        //    if (nSensorZoneID < 0 || nSensorTagID < 0)
        //    {
        //        if (GetSensorInfo(alarm.Zone, out nSensorTagID, out nSensorZoneID))
        //        {
        //            alarm.SensorTagID = nSensorTagID;
        //            alarm.SensorZoneID = nSensorZoneID;
        //        }
        //        else
        //            return false;
        //    }

        //    ArrayList arrDatas = new ArrayList();

        //    arrDatas.Add((int)FacilityType.FIRE_SENSOR);
        //    arrDatas.Add(nSensorTagID);
        //    arrDatas.Add(nSensorZoneID);
        //    arrDatas.Add(alarm.IsAlarmOn ? 1 : 0);

        //    alarm.SensorTagID = nSensorTagID;
        //    alarm.SensorZoneID = nSensorZoneID;

        //    if (alarm.IsAlarmOn)
        //        m_dicSensorZoneFireAlarms[nSensorZoneID] = alarm;
        //    else
        //    {
        //        if (alarm.SensorZoneHistoryID > 0)
        //        {
        //            // SensorZoneHistoryID가 생성되어 있는 경우
        //            FireAlarm temp;
        //            m_dicSensorZoneHistoryFireAlarms.TryRemove(alarm.SensorZoneHistoryID, out temp);
        //        }
        //        else
        //        {
        //            // 아직 SOPWebServer에서 SensorZoneHistoryID를 생성하지 않은 경우
        //        }
        //    }

        //    byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
        //    return m_postManFire.SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
        //}

        // isReal : true이면 실제화재, false이면 오작동
        public bool SendFireAlarmResult(FireAlarm alarm, bool isReal, string strDescription)
        {
            ArrayList arrDatas = new ArrayList();
            bool result;

            if (isReal)
            {
                // 실제화재
                arrDatas.Add(alarm.SensorZoneHistoryID);
                // EquipZone ID를 사용해야 하지만, Zone ID와 EquipZone ID가 동일하기 때문에 상관없다.
                arrDatas.Add(alarm.Zone.ID);
                arrDatas.Add(alarm.SensorZoneID);
                // SOPGenUserID를 넣어야 하는데, 1번 유저는 있기 마련이므로 1번을 사용한다.
                arrDatas.Add(1);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                result = m_postManSDMS.SendMessage(SOPWebServer.Header.NOTIFY_DISASTER, bytes);
            }
            else
            {
                // 오작동
                arrDatas.Add(alarm.SensorZoneHistoryID);
                arrDatas.Add(alarm.SensorZoneID);
                // SOPGenUserID를 넣어야 하는데, 1번 유저는 있기 마련이므로 1번을 사용한다.
                arrDatas.Add(1);
                arrDatas.Add(strDescription);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                result = m_postManSDMS.SendMessage(SOPWebServer.Header.SENSOR_MALFUNCTION, bytes);

                // 알람이 종료되었으니 삭제한다.
                FireAlarm temp;
                m_dicSensorZoneHistoryFireAlarms.TryRemove(alarm.SensorZoneHistoryID, out temp);
            }

            return result;
        }

        public bool GetSensorInfo(Zone zone, out int nSensorTagID, out int nSensorZoneID)
        {
            nSensorTagID = nSensorZoneID = 0;

            string strSQL = "Select sz.ID, sti.ID ";
            strSQL += "from SensorZone as sz, SensorTagInfo as sti ";
            strSQL += "where sti.SensorZoneID = sz.ID and sz.Zone = " + zone.ID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorTagInfoID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (sensorZoneID == null || sensorTagInfoID == null)
                    continue;

                nSensorZoneID = sensorZoneID.Data;
                nSensorTagID = sensorTagInfoID.Data;
                return true;
            }

            return false;
        }

        /*private void ProcessSensorReactionSensorHistoryData(ArrayList arrDatas)
        {
            ReadReactionHistoryLog(arrDatas);
        }*/

        /*private bool ReadReactionHistoryLog(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is long)
            {
                int nSensorReactionHistoryID = (int)arrDatas[0];
                int nSensorZoneHistoryID = (int)arrDatas[1];
                int nReactionType = (int)arrDatas[2];
                long time = (long)arrDatas[3];

                DateTime timeStamp = DateTime.FromBinary(time);

                ReactionType reactionType = (ReactionType)nReactionType;

                if (reactionType == ReactionType.BEGIN_STATUS || reactionType == ReactionType.NOTIFY_SIGNAL)
                {
                    // 외부센서를 통한 화재발생 신호
                    int nSensorZoneID = GetSensorZoneID(nSensorZoneHistoryID);

                    if (nSensorZoneID < 0)
                        return false;

                    FireAlarm alarm = null, temp;

                    m_dicSensorZoneFireAlarms.TryGetValue(nSensorZoneID, out alarm);

                    if (alarm == null)
                    {
                        // IIS가 꺼져있는 동안 발생했던 알람정보를 받는 경우
                        alarm = ReadAlarmFromSensorZoneHistory(nSensorZoneHistoryID, timeStamp);
                    }

                    if (alarm != null)
                    {
                        alarm.SensorZoneHistoryID = nSensorZoneHistoryID;

                        if (alarm.IsAlarmOn == false)
                        {
                            // 이미 알람이 종료된 경우
                            // m_postManFire를 사용하여 알람종료 신호를 보낸다.
                            // m_postManSDMS를 사용하면, Block될 우려가 있다.
                            m_dicSensorZoneFireAlarms.TryRemove(nSensorZoneID, out temp);
                            AddAlarm(alarm);
                            //SendFireSensorEvent(alarm);
                            return true;
                        }

                        // WebFireAlarmHistory Table에 SensorZoneHistoryID를 넣어준다.
                        if (alarm.WebHistoryID > 0)
                            UpdateWebHistory(alarm, nSensorZoneHistoryID);

                        m_dicSensorZoneHistoryFireAlarms[nSensorZoneHistoryID] = alarm;
                        m_dicSensorZoneFireAlarms.TryRemove(nSensorZoneID, out temp);
                        return true;
                    }
                }
                else if (reactionType == ReactionType.END_STATUS)
                {
                    // 외부센서를 통한 화재꺼짐 신호
                    FireAlarm temp;
                    m_dicSensorZoneHistoryFireAlarms.TryRemove(nSensorZoneHistoryID, out temp);
                }

                return true;
            }

            return false;
        }*/

        /*private int GetSensorZoneID(int nSensorZoneHistoryID)
        {
            string strSQL = "Select SensorID from SensorZoneHistory where ID = " + nSensorZoneHistoryID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (sensorZoneID == null)
                return -1;

            return sensorZoneID.Data;
        }*/

        /*private void ProcessReactionHistoryLogList(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            ArrayList arrReactionLog = new ArrayList();

            int nDataCount = arrDatas.Count;
            int nLogChunkSize = 10;
            for (int i = 0; i < nDataCount - (nLogChunkSize - 1); i += nLogChunkSize)
            {
                ArrayList arrDatas2 = new ArrayList();
                for (int j = i; j < i + nLogChunkSize; j++)
                {
                    arrDatas2.Add(arrDatas[j]);
                }

                ReadReactionHistoryLog(arrDatas2);
            }
        }*/

        public FireAlarm GetFireAlarm(string strEventID)
        {
            /*List<FireAlarm> alarms = m_dicSensorZoneHistoryFireAlarms.Values.ToList();

            foreach (FireAlarm alarm in alarms)
            {
                if (alarm.EventID == strEventID)
                    return alarm;
            }*/

            return GetFireAlarmFromDB(strEventID);
        }

        private FireAlarm GetFireAlarmFromDB(string strEventID)
        {
            if (m_dbMgr == null)
                return null;

            string strSQL = "Select ID, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId, SensorZoneHistoryID from WebFireAlarmHistory where evtId = '" + strEventID + "' and SensorZoneHistoryID is not null order by ID desc";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            FireAlarm alarm = ToFireAlarm(arrResult);

            if (alarm == null)
                return null;

            if (alarm.SensorZoneHistoryID < 0)
                return null;

            string strSiteID = DataManager.Instance.GetSiteID(alarm.SiteID);

            strSQL = "Select SensorID from SensorZoneHistory where ID = " + alarm.SensorZoneHistoryID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL, DataManager.GetDBName(strSiteID));

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (sensorZoneID == null)
                return null;

            alarm.SensorZoneID = sensorZoneID.Data; 

            // 알람의 종료여부를 검사하여 이미 종료되었으면 null 리턴
            strSQL = string.Format("Select ID from SensorReactionHistory where SensorHistoryID = {0} and (ReactionType = {1} or ReactionType = {2})",
                alarm.SensorZoneHistoryID, (int)ReactionType.END_STATUS, (int)ReactionType.TIME_OUT);

            arrResult = m_dbMgr.GetResultData(strSQL, DataManager.GetDBName(strSiteID));

            if (arrResult == null)
                return null;

            // 이미 종료된 알람
            if (arrResult.Count > 0)
                return null;

            return alarm;
        }

        private FireAlarm GetFireAlarmFromDB(Zone zone)
        {
            if (m_dbMgr == null)
                return null;

            if (zone.Building == null)
                return null;

            string strSQL = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID, szh.Param3 FROM SensorReactionHistory as srh ";
            strSQL += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            strSQL += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + " ) ";
            strSQL += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + " ) ";
            strSQL += " AND szh.SiteID = " + zone.Building.SiteID;
            strSQL += " ORDER BY srh.Time, szh.SensorID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, DataManager.GetDBName(zone.Building.SiteID));

            if (arrResult == null || arrResult.Count == 0)
                return null;

            int nEquipZoneID, nSensorZoneHistoryID = -1;
            int nResultCount = arrResult.Count;
            VariousData<DateTime> alarmTime = null;

            for (int i=0;i<nResultCount-11;i+=12)
            {
                VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> reactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                string strParam1 = WebDBManager.GetStringField(arrResult[i + 5]);

                if (sensorZoneHistoryID != null && strParam1 != null && reactionType != null && timeStamp != null)
                {
                    if (reactionType.Data == (int)ReactionType.BEGIN_STATUS && int.TryParse(strParam1.Trim(), out nEquipZoneID))
                    {
                        // Param1은 EquipZone ID인데, SK DigitalTwin에서는 EquipZone ID와 Zone ID가 동일하다.
                        if (zone.ID == nEquipZoneID)
                        {
                            nSensorZoneHistoryID = sensorZoneHistoryID.Data;
                            
                            if (alarmTime == null)
                            {
                                alarmTime = timeStamp;
                                nSensorZoneHistoryID = sensorZoneHistoryID.Data;
                            }
                            else if (alarmTime.Data < timeStamp.Data)
                            {
                                // 가장 나중에 발생한 알람을 선택한다.
                                alarmTime = timeStamp;
                                nSensorZoneHistoryID = sensorZoneHistoryID.Data;
                            }
                        }
                    }
                }
            }

            return GetFireAlarmFromDB(nSensorZoneHistoryID, alarmTime, zone.Building.SiteID);
        }

        private FireAlarm GetFireAlarmFromDB(int nSensorZoneID, Zone zone)
        {
            if (m_dbMgr == null)
                return null;

            if (zone == null || zone.Building == null)
                return null;

            string strSQL = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID, szh.Param3 FROM SensorReactionHistory as srh ";
            strSQL += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            strSQL += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + " ) ";
            strSQL += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + " ) ";
            strSQL += " AND szh.SiteID = " + zone.Building.SiteID;
            strSQL += " ORDER BY srh.Time, szh.SensorID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, DataManager.GetDBName(zone.Building.SiteID));

            if (arrResult == null || arrResult.Count == 0)
                return null;

            int sensorZoneID, nSensorZoneHistoryID = -1;
            int nResultCount = arrResult.Count;
            VariousData<DateTime> alarmTime = null;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> reactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                string strParam2 = WebDBManager.GetStringField(arrResult[i + 6]);

                if (sensorZoneHistoryID != null && strParam2 != null && reactionType != null && timeStamp != null)
                {
                    if (reactionType.Data == (int)ReactionType.BEGIN_STATUS && int.TryParse(strParam2.Trim(), out sensorZoneID))
                    {
                        if (nSensorZoneID == sensorZoneID)
                        {
                            nSensorZoneHistoryID = sensorZoneHistoryID.Data;

                            if (alarmTime == null)
                            {
                                alarmTime = timeStamp;
                                nSensorZoneHistoryID = sensorZoneHistoryID.Data;
                            }
                            else if (alarmTime.Data < timeStamp.Data)
                            {
                                // 가장 나중에 발생한 알람을 선택한다.
                                alarmTime = timeStamp;
                                nSensorZoneHistoryID = sensorZoneHistoryID.Data;
                            }
                        }
                    }
                }
            }

            return GetFireAlarmFromDB(nSensorZoneHistoryID, alarmTime, zone.Building.SiteID);
        }

        private FireAlarm GetFireAlarmFromDB(int nSensorZoneHistoryID, VariousData<DateTime> alarmTime, string strSiteID)
        {
            if (nSensorZoneHistoryID < 0 || alarmTime == null)
                return null;

            string strSensorZoneHistoryID = FireAlarm.MakeSensorZoneHistoryIDString(strSiteID, nSensorZoneHistoryID);

            string strSQL = "Select ID, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId, SensorZoneHistoryID, RecvTime from WebFireAlarmHistory where SensorZoneHistoryID = '" + strSensorZoneHistoryID + "' order by ID desc";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            FireAlarm alarm = null;

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[i + 9]);

                if (recvTime == null)
                    continue;

                // recvTime : WebAPI가 호출된 시간
                // alarmTime : SensorReactionHistory가 작성된 시간
                TimeSpan span = recvTime.Data - alarmTime.Data;

                // API 호출뒤 DB 작성까지 3초가 넘게 걸릴리 없다.
                if (span.TotalSeconds > 3.0)
                    continue;

                alarm = ToFireAlarm(arrResult, i);
                break;
            }

            return alarm;
        }

        // 현재 Alarm이 발생중인 SensorReactionLog에 대한 Query 조건문
        private string GetAlarmReactionHistoryQueryString()
        {
            string strCondition = ((int)ReactionType.BEGIN_STATUS).ToString();
            strCondition += ", " + ((int)ReactionType.NOTIFY_SIGNAL).ToString();

            return "(" + strCondition + ")";
        }

        //현재 Alarm이 꺼진 SensorReactionLog에 대한 Query조건문
        private string GetAlarmOffReactionHistoryQueryString()
        {
            string strCondition = ((int)ReactionType.MALFUNCTION).ToString();
            strCondition += ", " + ((int)ReactionType.IGNORE_SIGNAL).ToString();
            strCondition += ", " + ((int)ReactionType.IGNORE_SOP).ToString();
            strCondition += ", " + ((int)ReactionType.END_STATUS).ToString();
            strCondition += ", " + ((int)ReactionType.USER_RESET).ToString();
            strCondition += ", " + ((int)ReactionType.TIME_OUT).ToString();

            return "(" + strCondition + ")";
        }

        private FireAlarm ToFireAlarm(ArrayList arrResult, int nIndex = 0)
        {
            if (arrResult == null || arrResult.Count < 9)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[nIndex].ToString());
            string strEquipCode = WebDBManager.GetStringField(arrResult[nIndex + 1]);
            string strEquipStatus = WebDBManager.GetStringField(arrResult[nIndex + 2]);
            string strEventID = WebDBManager.GetStringField(arrResult[nIndex + 3]);
            string strEventTime = WebDBManager.GetStringField(arrResult[nIndex + 4]);
            string strEventType = WebDBManager.GetStringField(arrResult[nIndex + 5]);
            string strMapCode = WebDBManager.GetStringField(arrResult[nIndex + 6]);
            string strFloorID = WebDBManager.GetStringField(arrResult[nIndex + 7]);
            string strSensorZoneHistoryID = WebDBManager.GetStringField(arrResult[nIndex + 8]);

            if (id == null || strEquipCode == null || strEventID == null || strEventTime == null || strEventType == null || strMapCode == null || strFloorID == null)
                return null;

            Building building = DataManager.Instance.GetBuilding(strMapCode);

            if (building == null)
            {
                return null;
            }

            Zone zone = DataManager.Instance.GetZone(building, strFloorID);

            if (zone == null)
                return null;

            FireAlarm alarm = new FireAlarm();

            alarm.WebHistoryID = id.Data;
            alarm.EquipCode = strEquipCode;
            alarm.EquipStatus = strEquipStatus;
            alarm.EventID = strEventID;

            DateTime timeStamp;

            if (DateTime.TryParse(strEventTime, out timeStamp))
                alarm.TimeStamp = timeStamp;

            alarm.EventType = strEventType;
            alarm.Zone = zone;

            if (strSensorZoneHistoryID != null)
            {
                alarm.SetSensorZoneHistoryID(strSensorZoneHistoryID);
                //alarm.SensorZoneHistoryID = sensorZoneHistoryID.Data;
            }

            return alarm;
        }

        public FireAlarm GetFireAlarm(int nSensorZoneID, Zone zone)
        {
            /*List<FireAlarm> alarms = m_dicSensorZoneHistoryFireAlarms.Values.ToList();

            foreach (FireAlarm alarm in alarms)
            {
                if (alarm.SensorZoneID > 0 && alarm.SensorZoneID == nSensorZoneID)
                    return alarm;
            }*/

            return GetFireAlarmFromDB(nSensorZoneID, zone);
        }

        public FireAlarm GetFireAlarm(Zone zone)
        {
            /*List<FireAlarm> alarms = m_dicSensorZoneHistoryFireAlarms.Values.ToList();

            foreach (FireAlarm alarm in alarms)
            {
                if (alarm.Zone != null && alarm.Zone == zone)
                    return alarm;
            }*/

            return GetFireAlarmFromDB(zone);
        }

        private void UpdateWebHistory(FireAlarm alarm, int nSensorZoneHistoryID)
        {
            string strSensorZoneHistoryID = FireAlarm.MakeSensorZoneHistoryIDString(alarm.SiteID, nSensorZoneHistoryID);
            string strSQL = string.Format("Update {0} set SensorZoneHistoryID = '{1}' where ID = {2}",
                DataManager.FireAlarmHistoryTable, strSensorZoneHistoryID, alarm.WebHistoryID);

            m_dbMgr.GetResultData(strSQL);
        }

        //private FireAlarm ReadAlarmFromSensorZoneHistory(int nSensorZoneHistoryID, DateTime timeStamp)
        //{
        //    string strSQL = "Select ID, RecvTime, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId from ";
        //    strSQL += DataManager.FireAlarmHistoryTable + " where SensorZoneHistoryID = " + nSensorZoneHistoryID.ToString();

        //    ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

        //    if (arrResult == null)
        //        return null;

        //    if (arrResult.Count == 9)
        //    {
        //        VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
        //        VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[1]);
        //        string dvcCd = WebDBManager.GetStringField(arrResult[2]);
        //        string dvcStatus = WebDBManager.GetStringField(arrResult[3]);
        //        string evtId = WebDBManager.GetStringField(arrResult[4]);
        //        string evtTime = WebDBManager.GetStringField(arrResult[5]);
        //        string evtType = WebDBManager.GetStringField(arrResult[6]);
        //        string mapCd = WebDBManager.GetStringField(arrResult[7]);
        //        string floorId = WebDBManager.GetStringField(arrResult[8]);

        //        if (id == null || recvTime == null || dvcCd == null || dvcStatus == null ||
        //            evtId == null || evtTime == null || evtType == null || mapCd == null ||
        //            floorId == null)
        //            return null;

        //        FireAlarm alarm = Controllers.AlarmEventController.MakeAlarm(id.Data, dvcCd, dvcStatus, evtId, evtType, mapCd, floorId);

        //        if (alarm != null)
        //        {
        //            alarm.SensorZoneHistoryID = nSensorZoneHistoryID;
        //            return alarm;
        //        }
        //        else
        //            return null;
        //    }

        //    // SensorZoneHistory가 생성된 시간의 앞뒤 5초 이내에서 가장 시간이 가까운 데이터를 고른다.
        //    DateTime prevTime = timeStamp.AddSeconds(-5);
        //    DateTime postTime = timeStamp.AddSeconds(5);
        //    string strPrevTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", prevTime.Year, prevTime.Month, prevTime.Day, prevTime.Hour, prevTime.Minute, prevTime.Second);
        //    string strPostTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", postTime.Year, postTime.Month, postTime.Day, postTime.Hour, postTime.Minute, postTime.Second);

        //    strSQL = "Select ID, RecvTime, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId from ";
        //    strSQL += string.Format("{0} where RecvTime >= {1} and RecvTime <= {2} and SensorZoneHistoryID is null",
        //        DataManager.FireAlarmHistoryTable, strPrevTime, strPostTime);

        //    arrResult = m_dbMgr.GetResultData(strSQL);

        //    if (arrResult == null)
        //        return null;

        //    double dMinSeconds = 0.0;
        //    int nIndex = -1;
        //    int nResultCount = arrResult.Count;

        //    for (int i=0;i<nResultCount-8;i+=9)
        //    {
        //        VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[i + 1]);

        //        if (recvTime == null)
        //            continue;

        //        TimeSpan span = timeStamp - recvTime.Data;

        //        if (nIndex < 0)
        //        {
        //            dMinSeconds = span.TotalSeconds;
        //            nIndex = i;

        //            if (dMinSeconds < 0.0)
        //                dMinSeconds = -dMinSeconds;
        //        }
        //        else
        //        {
        //            double diff = span.TotalSeconds;

        //            if (diff < 0.0)
        //                diff = -diff;

        //            if (diff < dMinSeconds)
        //            {
        //                dMinSeconds = diff;
        //                nIndex = i;
        //            }
        //        }
        //    }

        //    if (nIndex >= 0)
        //    {
        //        VariousData<int> id = WebDBManager.GetIntField(arrResult[nIndex].ToString());
        //        VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[nIndex + 1]);
        //        string dvcCd = WebDBManager.GetStringField(arrResult[nIndex + 2]);
        //        string dvcStatus = WebDBManager.GetStringField(arrResult[nIndex + 3]);
        //        string evtId = WebDBManager.GetStringField(arrResult[nIndex + 4]);
        //        string evtTime = WebDBManager.GetStringField(arrResult[nIndex + 5]);
        //        string evtType = WebDBManager.GetStringField(arrResult[nIndex + 6]);
        //        string mapCd = WebDBManager.GetStringField(arrResult[nIndex + 7]);
        //        string floorId = WebDBManager.GetStringField(arrResult[nIndex + 8]);

        //        if (id == null || recvTime == null || dvcCd == null || dvcStatus == null ||
        //            evtId == null || evtTime == null || evtType == null || mapCd == null ||
        //            floorId == null)
        //            return null;

        //        FireAlarm alarm = Controllers.AlarmEventController.MakeAlarm(id.Data, dvcCd, dvcStatus, evtId, evtType, mapCd, floorId);

        //        if (alarm != null)
        //        {
        //            alarm.SensorZoneHistoryID = nSensorZoneHistoryID;
        //            return alarm;
        //        }
        //    }

        //    return null;
        //}

        public void AddAlarm(FireAlarm alarm)
        {
            // SOPWebAPI를 통한 SOPWebServer 통신 기능은 AutoUpdater로 이관한다.
            //m_queueAlarms.Enqueue(alarm);
        }
    }
}
