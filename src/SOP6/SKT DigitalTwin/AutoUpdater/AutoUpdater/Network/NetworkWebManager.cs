using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using System.Collections;
using System.Threading;
using DBUtility2;
using System.Collections.Concurrent;
using UnE.Spatial;
using UnE.Sensor;
using libSensorProcess;

namespace AutoUpdater.Network
{
    using Data;

    public class NetworkWebManager
    {
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
        private WebDBManagerEx m_dbMgr = null;

        // Key : SensorZone ID
        private ConcurrentDictionary<int, FireAlarm> m_dicSensorZoneFireAlarms = new ConcurrentDictionary<int, FireAlarm>();
        // Key : SensorZoneHistory ID
        private ConcurrentDictionary<int, FireAlarm> m_dicSensorZoneHistoryFireAlarms = new ConcurrentDictionary<int, FireAlarm>();
        private ConcurrentQueue<FireAlarm> m_queueAlarms = new ConcurrentQueue<FireAlarm>();

        private bool m_saveSensorZoneHistory = true;

        private static NetworkWebManager m_instance = null;

        public static NetworkWebManager Instance
        {
            get { return m_instance; }
        }

        private NetworkWebManager(WebDBManagerEx dbMgr)
        {
            m_dbMgr = dbMgr;

            int nPort = ReadServerPort();

            if (DataManager.Instance.BaseBuildingGroupID > 0)
                m_saveSensorZoneHistory = false;

            m_postManFire = new PostMan(this, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.SKT_DT);
            m_postManSDMS = new PostMan(this, SOPWebServer.ClientType.SDMS, SOPWebServer.ClientSubType.SKT_DT);

            SetPostBox(m_postManFire, nPort);
            SetPostBox(m_postManSDMS, nPort);

            m_postManList.Add(m_postManFire);
            m_postManList.Add(m_postManSDMS);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
        }

        public static void InitInstance()
        {
            m_instance = new NetworkWebManager(DataManager.Instance.DBManager);
        }

        public void OnMessage(int header, byte[] messages, object postMan)
        {
            if (postMan != null && postMan is PostMan)
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
            }
        }

        private void ConnectionThread()
        {
            List<FireAlarm> unprocessedAlarms = new List<FireAlarm>();

            while (m_shutdownThread == false)
            {
                foreach (PostMan postMan in m_postManList)
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
                }

                while (m_queueAlarms.Count > 0)
                {
                    FireAlarm alarm;

                    if (m_queueAlarms.TryDequeue(out alarm) == false)
                        break;

                    if (m_postManFire.IsConnected)
                        SendFireSensorEvent(alarm);
                    else
                        unprocessedAlarms.Add(alarm);
                }

                foreach (FireAlarm alarm in unprocessedAlarms)
                {
                    m_queueAlarms.Enqueue(alarm);
                }

                unprocessedAlarms.Clear();
                Thread.Sleep(1000);
            }
        }

        private int ReadServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, m_dbMgr.LocalDBName);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManagerEx.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        private string GetSOPWebServerURL()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'SOPWebServerURL' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, m_dbMgr.LocalDBName);

            if (arrResult == null || arrResult.Count == 0)
                return m_dbMgr.WebServerURL;

            string strWebServerURL = WebDBManager.GetStringField(arrResult[0]);

            if (strWebServerURL == null)
                return m_dbMgr.WebServerURL;

            return strWebServerURL;
        }

        private void SetPostBox(PostMan postMan, int nPort)
        {
            if (nPort > 0)
            {
                PostBox postBox = new PostBox();
                postBox.WebServerURL = GetSOPWebServerURL();
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

        private bool SendFireSensorEvent(FireAlarm alarm)
        {
            int nSensorZoneID = alarm.SensorZoneID, nSensorTagID = alarm.SensorTagID;

            if (nSensorZoneID < 0 || nSensorTagID < 0)
            {
                if (GetSensorInfo(alarm.Zone, out nSensorTagID, out nSensorZoneID))
                {
                    alarm.SensorTagID = nSensorTagID;
                    alarm.SensorZoneID = nSensorZoneID;
                }
                else
                    return false;
            }

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add((int)IFacility.FacilityType.FIRE_SENSOR);
            arrDatas.Add(nSensorTagID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(alarm.IsAlarmOn ? 1 : 0);

            alarm.SensorTagID = nSensorTagID;
            alarm.SensorZoneID = nSensorZoneID;

            if (alarm.IsAlarmOn)
                m_dicSensorZoneFireAlarms[nSensorZoneID] = alarm;
            else
            {
                if (alarm.SensorZoneHistoryID > 0)
                {
                    // SensorZoneHistoryID가 생성되어 있는 경우
                    FireAlarm temp;
                    m_dicSensorZoneHistoryFireAlarms.TryRemove(alarm.SensorZoneHistoryID, out temp);
                }
                else
                {
                    // 아직 SOPWebServer에서 SensorZoneHistoryID를 생성하지 않은 경우
                }
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return m_postManFire.SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
        }

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

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> sensorZoneID = WebDBManagerEx.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorTagInfoID = WebDBManagerEx.GetIntField(arrResult[i + 1].ToString());

                if (sensorZoneID == null || sensorTagInfoID == null)
                    continue;

                nSensorZoneID = sensorZoneID.Data;
                nSensorTagID = sensorTagInfoID.Data;
                return true;
            }

            return false;
        }

        private void ProcessSensorReactionSensorHistoryData(ArrayList arrDatas)
        {
            ReadReactionHistoryLog(arrDatas);
        }

        private bool ReadReactionHistoryLog(ArrayList arrDatas)
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
        }

        private int GetSensorZoneID(int nSensorZoneHistoryID)
        {
            string strSQL = "Select SensorID from SensorZoneHistory where ID = " + nSensorZoneHistoryID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, m_dbMgr.LocalDBName);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> sensorZoneID = WebDBManagerEx.GetIntField(arrResult[0].ToString());

            if (sensorZoneID == null)
                return -1;

            return sensorZoneID.Data;
        }

        private void ProcessReactionHistoryLogList(ArrayList arrDatas)
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
        }

        public FireAlarm GetFireAlarm(string dvcCd, string strEventID)
        {
            /*List<FireAlarm> alarms = m_dicSensorZoneHistoryFireAlarms.Values.ToList();

            foreach (FireAlarm alarm in alarms)
            {
                if (alarm.EventID == strEventID)
                    return alarm;
            }*/

            return GetFireAlarmFromDB(dvcCd, strEventID);
        }

        private FireAlarm GetFireAlarmFromDB(string dvcCd, string strEventID)
        {
            if (m_dbMgr == null)
                return null;

            string strSQL = string.Format("Select ID, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId, SensorZoneHistoryID from WebFireAlarmHistory where evtId = '{0}' and dvcCd = '{1}' and SensorZoneHistoryID is not null order by ID desc",
                strEventID, dvcCd);
            //string strSQL = string.Format("Select ID, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId, SensorZoneHistoryID from WebFireAlarmHistory where evtId = '" + strEventID + "' and SensorZoneHistoryID is not null order by ID desc";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            FireAlarm alarm = ToFireAlarm(arrResult);

            if (alarm == null)
                return null;

            if (alarm.SensorZoneHistoryID < 0)
                return null;

            strSQL = "Select SensorID from SensorZoneHistory where ID = " + alarm.SensorZoneHistoryID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL, m_dbMgr.LocalDBName);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> sensorZoneID = WebDBManagerEx.GetIntField(arrResult[0].ToString());

            if (sensorZoneID == null)
                return null;

            alarm.SensorZoneID = sensorZoneID.Data;

            // 알람의 종료여부를 검사하여 이미 종료되었으면 null 리턴
            strSQL = string.Format("Select ID from SensorReactionHistory where SensorHistoryID = {0} and (ReactionType = {1} or ReactionType = {2})",
                alarm.SensorZoneHistoryID, (int)ReactionType.END_STATUS, (int)ReactionType.TIME_OUT);

            arrResult = m_dbMgr.GetResultData(strSQL, m_dbMgr.LocalDBName);

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

            string strSQL = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID, szh.Param3 FROM SensorReactionHistory as srh ";
            strSQL += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            strSQL += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + " ) ";
            strSQL += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + " ) ";
            strSQL += " AND szh.SiteID = " + m_dbMgr.SiteID.ToString();
            strSQL += " ORDER BY srh.Time, szh.SensorID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, m_dbMgr.LocalDBName);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            int nEquipZoneID, nSensorZoneHistoryID = -1;
            int nResultCount = arrResult.Count;
            VariousData<DateTime> alarmTime = null;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                VariousData<int> sensorZoneHistoryID = WebDBManagerEx.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> reactionType = WebDBManagerEx.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> timeStamp = WebDBManagerEx.GetDateTimeField(arrResult[i + 3]);
                string strParam1 = WebDBManagerEx.GetStringField(arrResult[i + 5]);

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

            return GetFireAlarmFromDB(nSensorZoneHistoryID, alarmTime);
        }

        private FireAlarm GetFireAlarmFromDB(int nSensorZoneID)
        {
            if (m_dbMgr == null)
                return null;

            string strSQL = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID, szh.Param3 FROM SensorReactionHistory as srh ";
            strSQL += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            strSQL += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + " ) ";
            strSQL += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + " ) ";
            strSQL += " AND szh.SiteID = " + m_dbMgr.SiteID.ToString();
            strSQL += " ORDER BY srh.Time, szh.SensorID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, m_dbMgr.LocalDBName);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            int sensorZoneID, nSensorZoneHistoryID = -1;
            int nResultCount = arrResult.Count;
            VariousData<DateTime> alarmTime = null;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                VariousData<int> sensorZoneHistoryID = WebDBManagerEx.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> reactionType = WebDBManagerEx.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> timeStamp = WebDBManagerEx.GetDateTimeField(arrResult[i + 3]);
                string strParam2 = WebDBManagerEx.GetStringField(arrResult[i + 6]);

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

            return GetFireAlarmFromDB(nSensorZoneHistoryID, alarmTime);
        }

        private FireAlarm GetFireAlarmFromDB(int nSensorZoneHistoryID, VariousData<DateTime> alarmTime)
        {
            if (nSensorZoneHistoryID < 0 || alarmTime == null)
                return null;

            string strSensorZoneHistoryID = FireAlarm.MakeSensorZoneHistoryIDString(nSensorZoneHistoryID);
            string strSQL = "Select ID, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId, SensorZoneHistoryID, RecvTime from WebFireAlarmHistory where SensorZoneHistoryID = '" + strSensorZoneHistoryID + "' order by ID desc";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            FireAlarm alarm = null;

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                VariousData<DateTime> recvTime = WebDBManagerEx.GetDateTimeField(arrResult[i + 9]);

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

            VariousData<int> id = WebDBManagerEx.GetIntField(arrResult[nIndex].ToString());
            string strEquipCode = WebDBManagerEx.GetStringField(arrResult[nIndex + 1]);
            string strEquipStatus = WebDBManagerEx.GetStringField(arrResult[nIndex + 2]);
            string strEventID = WebDBManagerEx.GetStringField(arrResult[nIndex + 3]);
            string strEventTime = WebDBManagerEx.GetStringField(arrResult[nIndex + 4]);
            string strEventType = WebDBManagerEx.GetStringField(arrResult[nIndex + 5]);
            string strMapCode = WebDBManagerEx.GetStringField(arrResult[nIndex + 6]);
            string strFloorID = WebDBManagerEx.GetStringField(arrResult[nIndex + 7]);
            string strSensorZoneHistoryID = WebDBManagerEx.GetStringField(arrResult[nIndex + 8]);

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
                if (alarm.SetSensorZoneHistoryID(strSensorZoneHistoryID) == false)
                    return null;
            }

            return alarm;
        }

        public FireAlarm GetFireAlarm(int nSensorZoneID)
        {
            /*List<FireAlarm> alarms = m_dicSensorZoneHistoryFireAlarms.Values.ToList();

            foreach (FireAlarm alarm in alarms)
            {
                if (alarm.SensorZoneID > 0 && alarm.SensorZoneID == nSensorZoneID)
                    return alarm;
            }*/

            return GetFireAlarmFromDB(nSensorZoneID);
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
            if (m_saveSensorZoneHistory)
            {
                string strSensorZoneHistoryID = FireAlarm.MakeSensorZoneHistoryIDString(nSensorZoneHistoryID);
                string strSQL = string.Format("Update {0} set SensorZoneHistoryID = '{1}' where ID = {2}",
                    DataManager.FireAlarmHistoryTable, strSensorZoneHistoryID, alarm.WebHistoryID);

                m_dbMgr.GetResultData(strSQL);
            }
            else
            {
                string strSQL = string.Format("Select WebFireAlarmHistoryID from {0} where WebFireAlarmHistoryID = {1} and SensorZoneHistoryID = {2}",
                    DataManager.FireAlarmSensorZoneHistoryTable, alarm.WebHistoryID, nSensorZoneHistoryID);

                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, m_dbMgr.LocalDBName);

                if (arrResult == null)
                    return;

                if (arrResult.Count == 0)
                {
                    strSQL = string.Format("Insert into {0} (WebFireAlarmHistoryID, SensorZoneHistoryID)", DataManager.FireAlarmSensorZoneHistoryTable);
                    strSQL += string.Format(" values ({0}, {1})", alarm.WebHistoryID, nSensorZoneHistoryID);

                    m_dbMgr.GetResultData(strSQL, m_dbMgr.LocalDBName);
                }

                /*string strSQL = string.Format("Update {0} set SensorZoneHistoryID = {1} where WebFireAlarmHistoryID = {2}",
                    DataManager.FireAlarmSensorZoneHistoryTable, nSensorZoneHistoryID, alarm.WebHistoryID);

                m_dbMgr.GetResultData(strSQL);*/
            }
        }

        private FireAlarm ReadAlarmFromSensorZoneHistory(int nSensorZoneHistoryID, DateTime timeStamp)
        {
            string strSensorZoneHistoryID = FireAlarm.MakeSensorZoneHistoryIDString(nSensorZoneHistoryID);
            string strSQL = "Select ID, RecvTime, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId from ";
            strSQL += DataManager.FireAlarmHistoryTable + " where SensorZoneHistoryID = '" + strSensorZoneHistoryID + "'";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            if (arrResult.Count == 9)
            {
                VariousData<int> id = WebDBManagerEx.GetIntField(arrResult[0].ToString());
                VariousData<DateTime> recvTime = WebDBManagerEx.GetDateTimeField(arrResult[1]);
                string dvcCd = WebDBManagerEx.GetStringField(arrResult[2]);
                string dvcStatus = WebDBManagerEx.GetStringField(arrResult[3]);
                string evtId = WebDBManagerEx.GetStringField(arrResult[4]);
                string evtTime = WebDBManagerEx.GetStringField(arrResult[5]);
                string evtType = WebDBManagerEx.GetStringField(arrResult[6]);
                string mapCd = WebDBManagerEx.GetStringField(arrResult[7]);
                string floorId = WebDBManagerEx.GetStringField(arrResult[8]);

                if (id == null || recvTime == null || dvcCd == null || dvcStatus == null ||
                    evtId == null || evtTime == null || evtType == null || mapCd == null ||
                    floorId == null)
                    return null;

                FireAlarm alarm = AlarmManager.MakeAlarm(id.Data, dvcCd, dvcStatus, evtId, evtType, mapCd, floorId);

                if (alarm != null)
                {
                    alarm.SensorZoneHistoryID = nSensorZoneHistoryID;
                    return alarm;
                }
                else
                    return null;
            }

            // SensorZoneHistory가 생성된 시간의 앞뒤 5초 이내에서 가장 시간이 가까운 데이터를 고른다.
            DateTime prevTime = timeStamp.AddSeconds(-5);
            DateTime postTime = timeStamp.AddSeconds(5);
            string strPrevTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", prevTime.Year, prevTime.Month, prevTime.Day, prevTime.Hour, prevTime.Minute, prevTime.Second);
            string strPostTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", postTime.Year, postTime.Month, postTime.Day, postTime.Hour, postTime.Minute, postTime.Second);

            strSQL = "Select ID, RecvTime, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId from ";
            strSQL += string.Format("{0} where RecvTime >= {1} and RecvTime <= {2} and SensorZoneHistoryID is null",
                DataManager.FireAlarmHistoryTable, strPrevTime, strPostTime);

            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            double dMinSeconds = 0.0;
            int nIndex = -1;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                VariousData<DateTime> recvTime = WebDBManagerEx.GetDateTimeField(arrResult[i + 1]);

                if (recvTime == null)
                    continue;

                TimeSpan span = timeStamp - recvTime.Data;

                if (nIndex < 0)
                {
                    dMinSeconds = span.TotalSeconds;
                    nIndex = i;

                    if (dMinSeconds < 0.0)
                        dMinSeconds = -dMinSeconds;
                }
                else
                {
                    double diff = span.TotalSeconds;

                    if (diff < 0.0)
                        diff = -diff;

                    if (diff < dMinSeconds)
                    {
                        dMinSeconds = diff;
                        nIndex = i;
                    }
                }
            }

            if (nIndex >= 0)
            {
                VariousData<int> id = WebDBManagerEx.GetIntField(arrResult[nIndex].ToString());
                VariousData<DateTime> recvTime = WebDBManagerEx.GetDateTimeField(arrResult[nIndex + 1]);
                string dvcCd = WebDBManagerEx.GetStringField(arrResult[nIndex + 2]);
                string dvcStatus = WebDBManagerEx.GetStringField(arrResult[nIndex + 3]);
                string evtId = WebDBManagerEx.GetStringField(arrResult[nIndex + 4]);
                string evtTime = WebDBManagerEx.GetStringField(arrResult[nIndex + 5]);
                string evtType = WebDBManagerEx.GetStringField(arrResult[nIndex + 6]);
                string mapCd = WebDBManagerEx.GetStringField(arrResult[nIndex + 7]);
                string floorId = WebDBManagerEx.GetStringField(arrResult[nIndex + 8]);

                if (id == null || recvTime == null || dvcCd == null || dvcStatus == null ||
                    evtId == null || evtTime == null || evtType == null || mapCd == null ||
                    floorId == null)
                    return null;

                FireAlarm alarm = AlarmManager.MakeAlarm(id.Data, dvcCd, dvcStatus, evtId, evtType, mapCd, floorId);

                if (alarm != null)
                {
                    alarm.SensorZoneHistoryID = nSensorZoneHistoryID;
                    return alarm;
                }
            }

            return null;
        }

        public void AddAlarm(FireAlarm alarm)
        {
            m_queueAlarms.Enqueue(alarm);
        }
    }
}
