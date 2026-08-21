using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.ServiceModel;
using System.Collections;
using AgentFactory;
using UnE.Sensor;
using UnE.Spatial;
using System.Collections.Concurrent;

namespace ServerProcess.Client
{
    using ServerProcess.Data;
    public class FireSensorServer : BaseClient
    {
        public class ClientData : ServerProcess.Client.ClientData
        {
            private bool m_allReceiverState = false;

            public bool AllReceiverState
            {
                get { return m_allReceiverState; }
                set { m_allReceiverState = value; }
            }

            public ClientData()
                : base()
            {
            }

            public ClientData(string strSessionID, IPostMan postMan)
                : base(strSessionID, postMan)
            {
            }

            public ClientData(string strSessionID, IPostMan postMan, int nClientType, int nClientSubType)
                : base(strSessionID, postMan, nClientType, nClientSubType)
            {
            }
        }

        private static FireSensorServer m_instance = null;

        // 화재센서 Client가 수신반의 상태정보를 보내올때 어떤 Client가 어떤 수신반 정보를 가지고 있는지를 기억시킨다.
        // 나중에 해당 Client와의 접속이 끊어지면 Client와 연관된 모든 수신반의 상태정보를 초기화시킨다.
        // Value : 수신반(SensorServerInfo) ID
        private ConcurrentDictionary<ServerProcess.Client.ClientData, Dictionary<int, int>> m_dicClientReceivers = new ConcurrentDictionary<Client.ClientData, Dictionary<int, int>>();

        public static FireSensorServer Instance
        {
            get { return m_instance; }
        }

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.FIRE_SENSOR_SERVER; }
        }

        public FireSensorServer()
            : base()
        {
            m_instance = this;
            //m_agent = m_agentFactory.MakeAgent(Factory.AgentType.Fire);
        }

        public FireSensorServer(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.Fire);
        }

        protected override void OnLoadEvent()
        {
        }

        protected override ServerProcess.Client.ClientData MakeClientData(int nClientType, int nClientSubType, OperationContext ctx, string strIP, int nPort)
        {
            if (m_postOffice != null)
            {
                IPostMan postMan = m_postOffice.GetPostMan(ctx);
                FireSensorServer.ClientData data = new FireSensorServer.ClientData(ctx.SessionId, postMan, nClientType, nClientSubType);
                data.IP = strIP;
                data.Port = nPort;
                postMan.ClientData = data;

                return data;
            }

            return null;
        }

        protected override int OnReceiveEvent(ServerProcess.Client.ClientData data, OperationContext ctx, int header, byte[] bytes, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.SENSOR_DATA)
                return ProcessSensorData(arrDatas, true);
            else if (header == SOPWebServer.Header.SENSOR_DATA_TEST)
                return ProcessSensorData(arrDatas, false);
            else if (header == SOPWebServer.Header.SENSOR_MALFUNCTION)
                return _ProcessMalfunction(arrDatas);
            else if (header == SOPWebServer.Header.RECEIVER_CONNECT)
                return ProcessReceiverState(ctx, arrDatas, true);
            else if (header == SOPWebServer.Header.RECEIVER_DISCONNECT)
                return ProcessReceiverState(ctx, arrDatas, false);
            else if (header == SOPWebServer.Header.ALL_RECEIVER_STATE)
                return ProcessAllReceiverState(ctx, arrDatas);
            else if (header == SOPWebServer.Header.CLEAR_DETECT_ALL)
                return ProcessAllClear();
            else if (header == SOPWebServer.Header.RESET_MAX_ACTIONSTEP_HISTORY_ID)
                return ResetMaxActionStepHistoryID(header, bytes, arrDatas);

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        private int ResetMaxActionStepHistoryID(int header, byte[] bytes, ArrayList arrDatas)
        {
            return this.m_postOffice.SendMessageToClient(SOPWebServer.ClientType.SOP_SIMULATOR, header, bytes, arrDatas);
        }

        private int _ProcessMalfunction(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is string)
            {
                int nSensorZoneHistoryID = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSOPGenUserID = (int)arrDatas[2];
                string strDescription = (string)arrDatas[3];

                return ProcessMalfunction(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        public int ProcessMalfunction(int nSensorZoneHistoryID, int nSensorZoneID, int nSOPGenUserID, string strDescription)
        {
            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);
            SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

            if (sensorZone == null || group == null)
                return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

            // 알람 해제
            AlarmData alarm = group.CurrentAlarm;
            int nResult = RemoveAlarm_Malfunction(group, sensorZone, nSOPGenUserID);

            if (alarm != null && group.CurrentAlarm == null)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();
                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                alarm.Status = BaseProcessManager.ReactionType.MALFUNCTION;
                m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                dbMgr.Clone();
            }

            return nResult;
        }

        private int ProcessSensorData(ArrayList arrDatas, bool isReal)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                int nSensorData = (int)arrDatas[3];

                IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);
                SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

                if (group == null)
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

                if (nSensorData > 0)
                {
                    // 알람 발생
                    AlarmData alarm;
                    int nResult = AddAlarm(group, nSensorTagID, sensorZone, isReal, out alarm);

                    if (alarm != null)
                    {
                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        m_agentFactory.ProcessManager.NewAlarm(dbMgr, alarm);
                        dbMgr.Close();
                    }

                    return nResult;
                }
                else
                {
                    // 알람 해제
                    AlarmData alarm = group.CurrentAlarm;
                    int nResult = RemoveAlarm(group, sensorZone, isReal);

                    if (alarm != null && group.CurrentAlarm == null)
                    {
                        alarm.Status = BaseProcessManager.ReactionType.END_STATUS;

                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                        dbMgr.Close();
                    }

                    return nResult;
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessAllClear()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            List<AlarmData> alarms = AlarmManager.Instance.CurrentAlarms;

            foreach (AlarmData alarm in alarms)
            {
                SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(alarm.SensorZoneID);
                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(alarm.SensorZoneID);

                if (group != null)
                {
                    int nResult = RemoveAlarm(group, sensorZone, alarm.IsReal);

                    if (nResult != SOPWebServer.ErrorMessageType.SUCCESS)
                    {
                        dbMgr.Close();
                        return nResult;
                    }

                    alarm.Status = BaseProcessManager.ReactionType.END_STATUS;
                    m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);                    
                }
            }

            dbMgr.Close();
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        // 오작동 처리
        private int RemoveAlarm_Malfunction(SensorZoneGroup group, SensorZone sensorZone, int nSOPGenUserID)
        {
            // Transaction 처리를 위하여 객체를 새로 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime timeStamp = DateTime.Now;

            // 오작동 처리는 SDMS에서 사용자에 의하여 보내기 때문에 특정 센서 뿐만 아니라
            // SensorZoneGroup내에 있는 모든 센서 데이터를 초기화 시킨다.
            if (group.RemoveAllSensorData(dbMgr) == false)
            //if (group.RemoveSensorData(sensorZone, dbMgr) == false)
            {
                dbMgr.Close();
                WriteLog("RemoveAllSensorData 실패 : " + sensorZone.ID.ToString());
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            // sensorZone의 신호는 복구되었지만 같은 영역에 다른 신호가 아직 남아있는 상황
            if (group.GetSensorDatas().Count > 0 && group.CurrentAlarm != null)
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            AlarmData alarm = group.CurrentAlarm;

            if (alarm == null)
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            string strMessage = GetMalfunctionMessage(sensorZone.EquipZone, alarm.IsReal);
            string strEquipZoneID = sensorZone.EquipZone == null ? null : sensorZone.EquipZone.ID.ToString();
            ProcessManager.DetectionStatus detectionStatus = alarm.IsReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.MALFUNCTION, strMessage, strEquipZoneID, sensorZone.ID.ToString(), nSOPGenUserID.ToString(), null, null, status, dbMgr))
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    alarm.Message = strMessage;
                    group.CurrentAlarm = null;
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            dbMgr.BatchRollback();
            dbMgr.Close();
            WriteLog("RemoveAlarm 실패 : " + sensorZone.ID.ToString());
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        // 수동 신고된 화재신호 복구
        public int RemoveManualAlarm(AlarmData alarm)
        {
            // Transaction 처리를 위하여 별도의 객체를 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime timeStamp = DateTime.Now;
            string strMessage = GetClearManualFireMessage(alarm);
            string strEquipZoneID = null;
            ProcessManager.DetectionStatus detectionStatus = ProcessManager.DetectionStatus.REAL;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.END_STATUS, strMessage, strEquipZoneID, alarm.SensorZoneID.ToString(), null, null, null, status, dbMgr))
            {
                if (dbMgr.BatchCommit())
                {
                    alarm.Message = strMessage;
                    dbMgr.Close();

                    if (dbMgr.Connect() == false)
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            dbMgr.BatchRollback();
            dbMgr.Close();
            WriteLog("Remove.ManualAlarm 실패 : " + alarm.SensorZoneHistoryID.ToString());
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        // 화재신호 복구
        public int RemoveAlarm(SensorZoneGroup group, SensorZone sensorZone, bool isReal)
        {
            // Transaction 처리를 위하여 별도의 객체를 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime timeStamp = DateTime.Now;

            if (group.RemoveSensorData(sensorZone, dbMgr) == false)
            {
                bool rollback = dbMgr.BatchRollback();
                dbMgr.Close();
                System.Diagnostics.Trace.WriteLine("Rollback : " + rollback.ToString());
                WriteLog("RemoveSensorData 실패 : " + sensorZone.ID.ToString());
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            // sensorZone의 신호는 복구되었지만 같은 영역에 다른 신호가 아직 남아있는 상황
            if (group.GetSensorDatas().Count > 0 && group.CurrentAlarm != null)
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            AlarmData alarm = group.CurrentAlarm;

            if (alarm == null)
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            string strMessage = GetClearFireMessage(sensorZone.EquipZone, isReal);
            string strEquipZoneID = sensorZone.EquipZone == null ? null : sensorZone.EquipZone.ID.ToString();
            ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.END_STATUS, strMessage, strEquipZoneID, sensorZone.ID.ToString(), null, null, null, status, dbMgr))
            {
                if (dbMgr.BatchCommit())
                {
                    alarm.Message = strMessage;
                    group.CurrentAlarm = null;
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            dbMgr.BatchRollback();
            dbMgr.Close();
            WriteLog("RemoveAlarm 실패 : " + sensorZone.ID.ToString());
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        private int AddAlarm(SensorZoneGroup group, int nSensorTagID, SensorZone sensorZone, bool isReal, out AlarmData alarm)
        {
            alarm = null;

            // 알람발생 신호에 대해서만 센서 비활성화를 검사한다.
            // 이미 알람이 발생한 센서의 경우 센서가 비활성화 상태이더라도 알람을 해제할 수 있어야 한다.
            if (SensorZoneManager.Instance.IsActiveSensor(nSensorTagID) == false)
            {
                WriteLog("AddAlarm 무시(비활성화된 센서) : " + sensorZone.ID.ToString());
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            AlarmData currentAlarm = group.CurrentAlarm;
            int nSensorDataCount = group.GetSensorDatas().Count;

            if (currentAlarm == null && nSensorDataCount > 0)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();
                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                //  논리적인 오류
                group.ClearSensorDatas(dbMgr);
                dbMgr.Close();
            }
            else if (currentAlarm != null && nSensorDataCount > 0)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();
                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                // 이미 알람이 발생중이다.
                // Sensor 데이터만 기록하고 종료한다.
                group.SetSensorData(sensorZone, 1, dbMgr, false);
                AlarmManager.Instance.AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, dbMgr);
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }
            else
            {
                // group 영역에 대하여 발생한 알람이 없다.
                // Transaction 처리를 위하여 객체를 새로 만든다.
                DirectDBManager dbMgr = m_dbMgr.Clone();
                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                if (dbMgr.BeginBatch() == false)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                group.SetSensorData(sensorZone, 1, dbMgr, true);

                DateTime timeStamp = DateTime.Now;
                alarm = AlarmManager.Instance.AddAlarm(sensorZone.ID, 1, null, null, null, timeStamp, dbMgr);

                if (alarm != null)
                {
                    alarm.AlarmDepth = 1;
                    group.CurrentAlarm = alarm;

                    ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                    VariousData<int> status = new VariousData<int>((int)detectionStatus);
                    string strMessage = GetDetectFireMessage(group.EquipmentZone, isReal);
                    string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                    ProcessManager.ReactionType reactionType = ProcessManager.ReactionType.BEGIN_STATUS;

                    string strParam3 = ((int)sensorZone.Type).ToString();

                    if (AlarmManager.Instance.AddReactionHistory(alarm, (int)reactionType, timeStamp, strMessage, strEquipZoneID, sensorZone.ID.ToString(), strParam3, null, null, status, dbMgr, true))
                    {
                        if (dbMgr.BatchCommit())
                        {
                            alarm.Message = strMessage;
                            alarm.IsReal = isReal;
                            alarm.Status = reactionType;
                            dbMgr.Close();
                            return SOPWebServer.ErrorMessageType.SUCCESS;
                        }
                        else
                        {
                            group.RemoveSensorData(sensorZone, null);
                            group.CurrentAlarm = null;
                        }
                    }
                    else
                    {
                        group.RemoveSensorData(sensorZone, dbMgr);
                        AlarmManager.Instance.RemoveAlarm(alarm);
                        WriteLog("AddReactionHistory 실패 : " + alarm.SensorZoneHistoryID.ToString());
                        alarm = null;
                        dbMgr.BatchRollback();
                    }
                }
                else
                {
                    group.RemoveSensorData(sensorZone, dbMgr);
                    WriteLog("AddAlarm 실패 : " + sensorZone.ID.ToString());
                    dbMgr.BatchRollback();
                }

                dbMgr.Close();
            }

            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        private string GetDetectFireMessage(EquipmentZone equipZone, bool isReal)
        {
            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    return strTag + "화재가 탐지되었습니다";
                else
                    return string.Format("{0}[{1}]에서 화재가 탐지되었습니다", strTag, equipZone.DisplayText);
            }

            if (equipZone == null)
                return "[테스트]화재가 탐지되었습니다";

            return string.Format("[테스트][{0}]에서 화재가 탐지되었습니다", equipZone.DisplayText);
        }

        private string GetClearManualFireMessage(AlarmData alarm)
        {
            string strMessage = "신고된 화재 상황이 종료되었습니다";
            int nZoneID;

            if (int.TryParse(alarm.ReactionHistoryParam1, out nZoneID))
            {
                Zone zone = SensorZoneManager.Instance.GetZone(nZoneID);

                if (zone != null)
                {
                    strMessage = string.Format("[{0}]에서 신고된 화재 상황이 종료되었습니다", zone.DisplayText);
                }
            }

            return strMessage;
        }

        private string GetClearFireMessage(EquipmentZone equipZone, bool isReal)
        {
            string strMessage = "상황해제";

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    strMessage = strTag + "화재신호가 복구되었습니다";
                else
                    strMessage = string.Format("{0}[{1}]에서 탐지된 화재신호가 복구되었습니다", strTag, equipZone.DisplayText);
            }
            else
            {
                if (equipZone == null)
                    strMessage = "[테스트]화재신호가 복구되었습니다";
                else
                    strMessage = string.Format("[테스트][{0}]에서 탐지된 화재신호가 복구되었습니다", equipZone.DisplayText);
            }

            return strMessage;
        }

        private string GetMalfunctionMessage(EquipmentZone equipZone, bool isReal)
        {
            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    return strTag + "탐지된 화재신호가 오작동으로 신고되었습니다.";
                else
                    return string.Format("{0}[{1}]에서 탐지된 화재신호가 오작동으로 신고되었습니다", strTag, equipZone.DisplayText);
            }
            else
            {
                if (equipZone == null)
                    return "[테스트]화재신호가 오작동으로 신고되었습니다";
                else
                    return string.Format("[테스트][{0}]에서 탐지된 화재신호가 오작동으로 신고되었습니다", equipZone.DisplayText);
            }
        }

        private string GetTrainingModeString()
        {
            string strTag = "";

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect())
            {
                strTag = m_agentFactory.SMSManager.GetTrainingModeString(dbMgr);
                dbMgr.Close();
            }

            return strTag;
        }

        private int ProcessReceiverState(OperationContext ctx, ArrayList arrDatas, bool isConnected)
        {
            ClientData client = (ClientData)GetClientData(ctx);

            if (client == null)
                return SOPWebServer.ErrorMessageType.UNKNOWN_CLIENT;

            if (arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                Dictionary<int, int> dicReceiverIDs = null;

                if (m_dicClientReceivers.TryGetValue(client, out dicReceiverIDs) == false)
                {
                    dicReceiverIDs = new Dictionary<int, int>();
                    m_dicClientReceivers[client] = dicReceiverIDs;
                }

                int nReceiverID = (int)arrDatas[0];
                Data.ReceiverManager.Instance.UpdateState(nReceiverID, isConnected, dbMgr);
                dbMgr.Close();

                dicReceiverIDs[nReceiverID] = nReceiverID;
            }

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessAllReceiverState(OperationContext ctx, ArrayList arrDatas)
        {
            ClientData client = (ClientData)GetClientData(ctx);

            if (client == null)
                return SOPWebServer.ErrorMessageType.UNKNOWN_CLIENT;

            int nDataCount = arrDatas.Count;

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            Dictionary<int, int> dicReceiverIDs = null;

            if (m_dicClientReceivers.TryGetValue(client, out dicReceiverIDs) == false)
            {
                dicReceiverIDs = new Dictionary<int, int>();
                m_dicClientReceivers[client] = dicReceiverIDs;
            }

            for (int i = 0; i < nDataCount; i += 2)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int )
                {
                    int nReceiverID = (int)arrDatas[i];
                    //bool isConnected = (int)arrDatas[i + 1] > 0;
                    //bool isReceivedPoll = (int)arrDatas[i + 2] >= 10;
                    int arg = (int)arrDatas[i + 1];

                    bool isConnected = false;
                    bool isReceivedPoll = false;

                    if (arg == 1)
                        isConnected = true;
                    else if (arg == 11)
                        isConnected = isReceivedPoll = true;
                    else if (arg == 10)
                        isReceivedPoll = true;

                    Data.Receiver receiver = Data.ReceiverManager.Instance.GetReceiver(nReceiverID);

                    if (receiver == null)
                    {
                        dbMgr.Close();
                        return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
                    }

                    dicReceiverIDs[nReceiverID] = nReceiverID;

                    if (client.AllReceiverState == false)
                    {
                        client.AllReceiverState = true;
                        Data.ReceiverManager.Instance.UpdateState(receiver, isConnected, isReceivedPoll, dbMgr);
                    }
                    else
                    {
                        if (receiver.State == Data.Receiver.ConnectionState.NotConnected)
                        {
                            if (isConnected == true || isReceivedPoll == true)
                            {
                                Data.ReceiverManager.Instance.UpdateState(receiver, isConnected, isReceivedPoll, dbMgr);
                            }
                        }
                        else if (receiver.State == Data.Receiver.ConnectionState.Connected)
                        {
                            if (isConnected == false || isReceivedPoll == true)
                            {
                                Data.ReceiverManager.Instance.UpdateState(receiver, isConnected, isReceivedPoll, dbMgr);
                            }
                        }
                        /*else if (receiver.State == 11)
                        {
                            if (isConnected == false || isReceivedPoll == false)
                            {
                                ReceiverManager.Instance.UpdateState(receiver, isConnected, isReceivedPoll, m_dbMgr);
                            }
                        }*/
                    }
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
                }
            }

            dbMgr.Close();
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        public int ProcessManualReportFire(int nSensorZoneID, int nZoneID, int nSOPGenUserID, string strMemo, int nAlarmStep)
        {
            //AlarmManager.Instance.GetManualAlarm()

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            // nSOPGenUserID에 해당하는 SOP Simulator가 제어권을 가지도록 한다.
            // SendSOPSimulatorControl(nSOPGenUserID) => 구현할 것
            AlarmData alarm = AlarmManager.Instance.GetManualAlarm(nZoneID, IFacility.FacilityType.FIRE_SENSOR, dbMgr);

            if (alarm != null)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.ALREADY_PROCESSED;
            }

            // Transaction 처리
            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime dtNow = DateTime.Now;
            string strParam2 = ((int)IFacility.FacilityType.FIRE_SENSOR).ToString();
            alarm = AlarmManager.Instance.AddAlarm(nSensorZoneID, 1, nZoneID.ToString(), strParam2, null, dtNow, dbMgr);

            if (alarm == null)
            {
                dbMgr.BatchRollback();
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }
            else
            {
                alarm.IsManual = true;
                alarm.AlarmDepth = nAlarmStep;
            }
            

            string strMessage = GetFireManualReportString(nZoneID);
            string strParam1 = nZoneID.ToString();
            strParam2 = nSensorZoneID.ToString();
            string strParam3 = nSOPGenUserID.ToString();
            string strParam5 = nAlarmStep.ToString();

            ProcessManager.DetectionStatus detectionStatus = ProcessManager.DetectionStatus.REAL;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);
            BaseProcessManager.ReactionType reactionType = BaseProcessManager.ReactionType.NOTIFY_SIGNAL;

            if (AlarmManager.Instance.AddReactionHistory(alarm, (int)reactionType, dtNow, strMessage, strParam1, strParam2, strParam3, null, strParam5, status, dbMgr, true))
            {
                if (strMemo.Length == 0 || AlarmManager.Instance.AddReactionHistoryDescription(alarm.SensorReactionHistoryID, alarm.SensorZoneHistoryID, strMemo, dbMgr, true))
                {
                    if (dbMgr.BatchCommit())
                    {
                        alarm.Message = strMessage;
                        alarm.IsReal = true;
                        alarm.Status = reactionType;

                        dbMgr.Close();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        m_agentFactory.ProcessManager.ReportAlarm(dbMgr, alarm);
                        dbMgr.Close();
                        return SOPWebServer.ErrorMessageType.SUCCESS;
                    }
                    else
                    {
                        dbMgr.Close();
                        WriteLog("ProcessManualReportFire 실패 : " + alarm.SensorZoneHistoryID.ToString());
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }
                }
                else
                {
                    AlarmManager.Instance.RemoveAlarm(alarm);
                    WriteLog("AddReactionHistoryDescription 실패 : " + alarm.SensorZoneHistoryID.ToString() + " / ErrorMessage : " + dbMgr.ErrorMessage);
                    dbMgr.Close();                    
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            // SensorZoneHistory
            // Param1 : Zone ID
            // Param2 : Sensor Type(FacilityType)

            // SensorReactionHistory
            // Param1 : Zone ID
            // Param2 : SennsorZone ID(없으니까 당연히 0)
            // Param3 : SOPGenUserID

            dbMgr.Close();
            //DateTime timeStamp = DateTime.Now;

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        public int ProcessReportFire(AlarmData alarm, int nSensorZoneHistoryID, int nEquipZoneID, int nSensorZoneID, int nSOPGenUserID)
        {
            // nSOPGenUserID에 해당하는 SOP Simulator가 제어권을 가지도록 한다.
            // SendSOPSimulatorControl(nSOPGenUserID) => 구현할 것

            // Transaction 처리를 위하여 객체를 새로 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime dtNow = DateTime.Now;
            
            string strMessage = GetFireReportString(nEquipZoneID);
            string strParam1 = nEquipZoneID.ToString();
            string strParam2 = nSensorZoneID.ToString();
            string strParam3 = nSOPGenUserID.ToString();

            ProcessManager.DetectionStatus detectionStatus = alarm.IsReal ? BaseProcessManager.DetectionStatus.REAL : BaseProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);
            BaseProcessManager.ReactionType reactionType = BaseProcessManager.ReactionType.NOTIFY_SIGNAL;

            if (AlarmManager.Instance.AddReactionHistory(alarm, (int)reactionType, dtNow, strMessage, strParam1, strParam2, strParam3, null, null, status, dbMgr, true))
            {
                if (dbMgr.BatchCommit())
                {
                    alarm.Message = strMessage;
                    alarm.Status = reactionType;
                    dbMgr.Close();

                    if (dbMgr.Connect() == false)
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                    m_agentFactory.ProcessManager.ReportAlarm(dbMgr, alarm);
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    WriteLog("ProcessReportFire 실패 : " + alarm.SensorZoneHistoryID.ToString());
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            dbMgr.Close();
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private string GetFireManualReportString(int nZoneID)
        {
            string strMessage = "";

            if (nZoneID < 0)
            {
                strMessage = "화재가 신고되었습니다";
            }
            else
            {
                Zone zone = SensorZoneManager.Instance.GetZone(nZoneID);

                if (zone != null)
                {
                    string szLocationName = zone.DisplayText;
                    strMessage = string.Format("[{0}]에서 화재가 신고되었습니다", szLocationName);
                }
            }

            return strMessage;
        }

        private string GetFireReportString(int nEquipZoneID)
        {
            string strMessage = "";

            if (nEquipZoneID < 0)
            {
                strMessage = "화재가 신고되었습니다";
            }
            else
            {
                EquipmentZone equipZone = SensorZoneManager.Instance.GetEquipmentZone(nEquipZoneID);

                if (equipZone != null)
                {
                    string szLocationName = equipZone.DisplayText;
                    strMessage = string.Format("[{0}]에서 화재가 신고되었습니다", szLocationName);
                }
            }

            return strMessage;
        }

        // 센서서버와 접속이 끊어지면 해당 서버와 연결된 수신반의 상태정보를 초기화 시킨다.
        protected override void OnRemoveClient(ServerProcess.Client.ClientData data)
        {
            Dictionary<int, int> dicReceiverIDs = null;

            if (m_dicClientReceivers.TryGetValue(data, out dicReceiverIDs))
            {
                if (dicReceiverIDs.Count > 0)
                {
                    DirectDBManager dbMgr = m_dbMgr.Clone();

                    if (dbMgr.Connect() == false)
                        return;

                    ReceiverManager.Instance.InitReciverState(dbMgr, dicReceiverIDs.Values.ToList());
                    dbMgr.Close();
                }

                m_dicClientReceivers.TryRemove(data, out dicReceiverIDs);
            }
        }
    }
}
