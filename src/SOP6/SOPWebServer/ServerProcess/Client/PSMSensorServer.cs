using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentFactory;
using DBUtility2;
using System.Collections;
using System.ServiceModel;
using UnE.Sensor;
using UnE.PSM;
using UnE.Spatial;
using System.Collections.Concurrent;

namespace ServerProcess.Client
{
    using ServerProcess.Data;

    public class PSMSensorServer : BaseClient
    {
        private static PSMSensorServer m_instance = null;

        // 센서서버 Client가 수신반의 상태정보를 보내올때 어떤 Client가 어떤 수신반 정보를 가지고 있는지를 기억시킨다.
        // 나중에 해당 Client와의 접속이 끊어지면 Client와 연관된 모든 수신반의 상태정보를 초기화시킨다.
        // Value : 수신반(SensorServerInfo) ID
        private ConcurrentDictionary<ServerProcess.Client.ClientData, Dictionary<int, int>> m_dicClientReceivers = new ConcurrentDictionary<Client.ClientData, Dictionary<int, int>>();

        public static PSMSensorServer Instance
        {
            get { return m_instance; }
        }

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.PSM_SENSOR_SERVER; }
        }

        public PSMSensorServer()
            : base()
        {
            m_instance = this;
        }

        public PSMSensorServer(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.PSM);
        }
        
        protected override void OnLoadEvent()
        {
        }

        protected override int OnReceiveEvent(ServerProcess.Client.ClientData data, OperationContext ctx, int header, byte[] bytes, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.SENSOR_DATA)
                return ProcessSensorData(arrDatas, true);
            else if (header == SOPWebServer.Header.SENSOR_DATA_TEST)
                return ProcessSensorData(arrDatas, false);
            else if (header == SOPWebServer.Header.SENSOR_USER_RESET)
                return _ProcessUserReset(arrDatas);
            else if (header == SOPWebServer.Header.ALL_RECEIVER_STATE)
                return ProcessAllReceiverState(ctx, arrDatas);

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        public int ProcessManualReportPSM(int nSensorZoneID, int nZoneID, int nSOPGenUserID, string strMemo, int nAlarmStep)
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            // nSOPGenUserID에 해당하는 SOP Simulator가 제어권을 가지도록 한다.
            // SendSOPSimulatorControl(nSOPGenUserID) => 구현할 것
            AlarmData alarm = AlarmManager.Instance.GetManualAlarm(nZoneID, IFacility.FacilityType.PSM_SENSOR, dbMgr);

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
            string strParam2 = ((int)IFacility.FacilityType.PSM_SENSOR).ToString();
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

            string strMessage = GetPSMManualReportString(nZoneID);
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

        private string GetPSMManualReportString(int nZoneID)
        {
            string strMessage = "";

            if (nZoneID < 0)
            {
                strMessage = "가스 누출이 신고되었습니다";
            }
            else
            {
                Zone zone = SensorZoneManager.Instance.GetZone(nZoneID);

                if (zone != null)
                {
                    string szLocationName = zone.DisplayText;
                    strMessage = string.Format("[{0}]에서 가스 누출이 신고되었습니다", szLocationName);
                }
            }

            return strMessage;
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

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int)
                {
                    int nReceiverID = (int)arrDatas[i];
                    bool isConnected = (int)arrDatas[i + 1] == 1;

                    Data.Receiver receiver = Data.ReceiverManager.Instance.GetReceiver(nReceiverID);

                    if (receiver == null)
                    {
                        dbMgr.Close();
                        return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
                    }

                    dicReceiverIDs[nReceiverID] = nReceiverID;
                    Data.ReceiverManager.Instance.UpdateState(receiver, isConnected, false, dbMgr);
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

        public int _ProcessUserReset(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is string)
            {
                int nSensorZoneHistoryID = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSOPGenUserID = (int)arrDatas[2];
                string strDescription = (string)arrDatas[3];

                return ProcessUserReset(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        public int ProcessUserReset(int nSensorZoneHistoryID, int nSensorZoneID, int nSOPGenUserID, string strDescription)
        {
            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);
            SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

            if (sensorZone == null || group == null)
                return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

            // 알람 해제
            AlarmData alarm = group.CurrentAlarm;
            int nResult = RemoveAlarm_UserReset(group, sensorZone, nSOPGenUserID);

            if (alarm != null && group.CurrentAlarm == null)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                alarm.Status = BaseProcessManager.ReactionType.USER_RESET;
                m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                dbMgr.Clone();
            }

            return nResult;
        }

        // 탐지신호 사용자 복구
        private int RemoveAlarm_UserReset(SensorZoneGroup group, SensorZone sensorZone, int nSOPGenUserID)
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

            // 신호복구 처리는 SDMS에서 사용자에 의하여 보내기 때문에 특정 센서 뿐만 아니라
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

            string strMessage = GetUserResetMessage(sensorZone.EquipZone, alarm.IsReal);
            ProcessManager.DetectionStatus detectionStatus = alarm.IsReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            string strParam1 = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
            string strParam2 = sensorZone.ID.ToString();
            string strParam3 = nSOPGenUserID.ToString();

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.USER_RESET, strMessage, strParam1, strParam2, strParam3, null, null, status, dbMgr))
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

        public int ProcessReportPSM(AlarmData alarm, int nSensorZoneHistoryID, int nEquipZoneID, int nSensorZoneID, int nSOPGenUserID)
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

            string strMessage = GetPSMReportString(nEquipZoneID, nSensorZoneID);
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
                    WriteLog("ProcessReportPSM 실패 : " + alarm.SensorZoneHistoryID.ToString());
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            dbMgr.BatchRollback();
            dbMgr.Close();
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private string GetPSMReportString(int nEquipZoneID, int nSensorZoneID)
        {
            string strMessage = "";

            if (nEquipZoneID < 0)
            {
                strMessage = "유해화학물질 누출이 신고되었습니다.";
            }
            else
            {
                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone != null)
                {
                    PSMSensor sensor = PSMManager.Instance.GetSensor(sensorZone.LinkedSensorID);

                    if (sensor != null)
                    {
                        PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
                        string strMaterialName = material == null ? "유해화학물질" : material.Name;

                        // update by mwkim 2016-05-11 : Sensor가 연결된 Tank의 LocationName을 EquipmentZone의 DisplayText로 변경
                        string strLocationName = sensorZone.EquipZone != null ? sensorZone.EquipZone.DisplayText : "";
                        strMessage = string.Format("[{0}]에서 {1} 누출이 신고되었습니다", strLocationName, strMaterialName);
                    }
                }
                
                if (strMessage.Length == 0)
                    strMessage = "유해화학물질 누출이 신고되었습니다.";
            }

            return strMessage;
        }

        private string GetClearManualPSMMessage(AlarmData alarm)
        {
            string strMessage = "누출신호가 복구되었습니다";
            int nZoneID;

            if (int.TryParse(alarm.ReactionHistoryParam1, out nZoneID))
            {
                Zone zone = SensorZoneManager.Instance.GetZone(nZoneID);

                if (zone != null)
                {
                    strMessage = string.Format("[{0}]에서 탐지된 누출신호가 복구되었습니다", zone.DisplayText);
                }
            }

            return strMessage;
        }

        private string GetUserResetMessage(EquipmentZone equipZone, bool isReal)
        {
            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    return strTag + "탐지된 누출신호가 시스템 복구되었습니다.";
                else
                    return string.Format("{0}[{1}]에서 탐지된 누출신호가 시스템 복구되었습니다.", strTag, equipZone.DisplayText);
            }
            else
            {
                if (equipZone == null)
                    return "[테스트]탐지된 누출신호가 시스템 복구되었습니다.";
                else
                    return string.Format("[테스트][{0}]에서 탐지된 누출신호가 시스템 복구되었습니다", equipZone.DisplayText);
            }
        }

        private string GetTrainingModeString()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return "";

            string strTag = m_agentFactory.SMSManager.GetTrainingModeString(dbMgr);
            dbMgr.Close();
            return strTag;
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

                PSMSensor sensor = PSMManager.Instance.GetSensor(sensorZone.LinkedSensorID);

                if (sensor == null)
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

                if (nSensorData > 0)
                {
                    if (nSensorData < (int)UnE.Alarm.AlarmType.PSM_ALARM_1)
                        nSensorData = (int)UnE.Alarm.AlarmType.PSM_ALARM_1 - 1 + nSensorData;

                    // 알람 발생
                    AlarmData alarm, prevAlarm;
                    int nResult = AddAlarm(group, nSensorTagID, sensorZone, sensor, nSensorData, isReal, out alarm, out prevAlarm);

                    DirectDBManager dbMgr = m_dbMgr.Clone();

                    if (dbMgr.Connect() == false)
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                    if (alarm != null && prevAlarm != null)
                    {
                        m_agentFactory.ProcessManager.ChangeAlarm(dbMgr, alarm, prevAlarm);
                    }
                    else if (alarm != null)
                    {
                        m_agentFactory.ProcessManager.NewAlarm(dbMgr, alarm);
                    }

                    dbMgr.Close();
                    return nResult;
                }
                else
                {
                    // 알람 해제
                    AlarmData alarm = group.CurrentAlarm;

                    if (alarm == null)
                        return SOPWebServer.ErrorMessageType.SUCCESS;

                    int nResult = RemoveAlarm(group, sensorZone, isReal);

                    if (alarm != null && group.CurrentAlarm == null)
                    {
                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        alarm.Status = BaseProcessManager.ReactionType.END_STATUS;
                        m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                        dbMgr.Close();
                    }

                    return nResult;
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int AddAlarm(SensorZoneGroup group, int nSensorTagID, SensorZone sensorZone, PSMSensor sensor, int nSensorData, bool isReal, out AlarmData alarm, out AlarmData prevAlarm)
        {
            prevAlarm = alarm = null;

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
                // 이미 알람이 발생중이다.
                // 알람단계가 바뀌었는지 확인한다.
                List<KeyValuePair<SensorZone, int>> sensorZoneDatas = group.GetSensorDatas();
                int nAlarmDepth;
                SensorZone alarmSensorZone;

                if (IsChangedAlarmDepth(sensorZoneDatas, sensorZone, nSensorData, out nAlarmDepth, out alarmSensorZone))
                {
                    // 알람단계가 바뀌었다.
                    prevAlarm = currentAlarm;
                    alarm = prevAlarm.Clone();

                    alarm.TimeStamp = DateTime.Now;
                    alarm.AlarmDepth = nAlarmDepth - (int)UnE.Alarm.AlarmType.PSM_ALARM_1 + 1;
                    alarm.Status = BaseProcessManager.ReactionType.CHANGE_ALARM_DEPTH;
                    alarm.SensorZoneID = alarmSensorZone.ID;
                    alarm.Message = GetChangeAlarmDepthString(sensor, group.EquipmentZone, alarm.AlarmDepth, prevAlarm.AlarmDepth, isReal);

                    // Transaction 처리를 위하여 객체를 새로 만든다.
                    DirectDBManager dbMgr = m_dbMgr.Clone();

                    if (dbMgr.Connect() == false)
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                    if (dbMgr.BeginBatch() == false)
                    {
                        dbMgr.Close();
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }

                    group.SetSensorData(sensorZone, nAlarmDepth, dbMgr, true);
                    AlarmManager.Instance.AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, dbMgr);

                    string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                    string strEquipZoneName = group.EquipmentZone == null ? "" : group.EquipmentZone.ZoneName;
                    ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                    VariousData<int> status = new VariousData<int>((int)detectionStatus);

                    string strParam3 = ((int)sensorZone.Type).ToString();

                    if (AlarmManager.Instance.AddReactionHistory(alarm, (int)alarm.Status, alarm.TimeStamp, alarm.Message, strEquipZoneID, sensorZone.ID.ToString(), strParam3, strEquipZoneName, alarm.AlarmDepth.ToString(), status, dbMgr, true))
                    {
                        if (dbMgr.BatchCommit())
                        {
                            dbMgr.Close();
                            group.CurrentAlarm = alarm;
                            AlarmManager.Instance.SetAlarm(alarm.SensorZoneHistoryID, alarm);
                            return SOPWebServer.ErrorMessageType.SUCCESS;
                        }
                        else
                        {
                            dbMgr.BatchRollback();
                            dbMgr.Close();
                            group.SetSensorData(sensorZone, (int)UnE.Alarm.AlarmType.PSM_ALARM_1 - 1 + prevAlarm.AlarmDepth, null, false);
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                        }
                    }
                    else
                    {
                        dbMgr.BatchRollback();
                        dbMgr.Close();
                        group.SetSensorData(sensorZone, (int)UnE.Alarm.AlarmType.PSM_ALARM_1 - 1 + prevAlarm.AlarmDepth, null, false);
                        WriteLog("AddReactionHistory 실패 : " + alarm.SensorZoneHistoryID.ToString());
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }
                }
                else
                {
                    DirectDBManager dbMgr = m_dbMgr.Clone();

                    if (dbMgr.Connect() == false)
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                    // 알람단계가 바뀌지 않았으므로 Sensor 데이터만 기록하고 종료한다.
                    group.SetSensorData(sensorZone, nSensorData, dbMgr, false);
                    AlarmManager.Instance.AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, dbMgr);
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
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

                group.SetSensorData(sensorZone, nSensorData, dbMgr, true);

                DateTime timeStamp = DateTime.Now;
                alarm = AlarmManager.Instance.AddAlarm(sensorZone.ID, nSensorData, null, null, null, timeStamp, dbMgr);

                if (alarm != null)
                {
                    alarm.AlarmDepth = nSensorData - (int)UnE.Alarm.AlarmType.PSM_ALARM_1 + 1;
                    group.CurrentAlarm = alarm;

                    ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                    VariousData<int> status = new VariousData<int>((int)detectionStatus);
                    string strMessage = "";
                    if (m_dbMgr.SiteID == 3)
                        strMessage = GetDetectPSMMessageSiteID3(sensor, group.EquipmentZone, nSensorData, isReal);
                    else
                        strMessage = GetDetectPSMMessage(sensor, group.EquipmentZone, isReal);
                    string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                    string strEquipZoneName = group.EquipmentZone == null ? "" : group.EquipmentZone.ZoneName;
                    ProcessManager.ReactionType reactionType = ProcessManager.ReactionType.BEGIN_STATUS;

                    if (AlarmManager.Instance.AddReactionHistory(alarm, (int)reactionType, timeStamp, strMessage, strEquipZoneID, sensorZone.ID.ToString(), sensor.ID.ToString(), strEquipZoneName, alarm.AlarmDepth.ToString(), status, dbMgr, true))
                    {
                        if (dbMgr.BatchCommit())
                        {
                            dbMgr.Close();
                            alarm.Message = strMessage;
                            alarm.IsReal = isReal;
                            alarm.Status = reactionType;
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

        private bool IsChangedAlarmDepth(List<KeyValuePair<SensorZone, int>> sensorZoneDatas, SensorZone sensorZone, int nSensorData, out int nAlarmDepth, out SensorZone alarmSensorZone)
        {
            // sensorZone의 알람값
            int nSensorZoneAlarmDepth = -1;
            // sensorZone을 제외한 나머지 센서들 값 중에서 가장 큰 값
            int nMaxAlarmDepth = -1;
            SensorZone maxSensorZone = null;

            foreach (KeyValuePair<SensorZone, int> pair in sensorZoneDatas)
            {
                if (pair.Key == sensorZone)
                    nSensorZoneAlarmDepth = pair.Value;
                else
                {
                    if (maxSensorZone == null || nMaxAlarmDepth < pair.Value)
                    {
                        maxSensorZone = pair.Key;
                        nMaxAlarmDepth = pair.Value;
                    }
                }
            }

            alarmSensorZone = sensorZone;
            nAlarmDepth = nSensorData;

            if (nSensorData == nSensorZoneAlarmDepth)
                return false;

            if (nSensorZoneAlarmDepth > nMaxAlarmDepth)
            {
                if (nSensorData > nMaxAlarmDepth)
                    nAlarmDepth = nSensorData;
                else
                {
                    alarmSensorZone = maxSensorZone;
                    nAlarmDepth = nMaxAlarmDepth;
                }

                return true;
            }
            else
            {
                if (nSensorData > nMaxAlarmDepth)
                    return true;
            }

            return false;
        }

        private string GetChangeAlarmDepthString(PSMSensor sensor, EquipmentZone equipZone, int nAlarmDepth, int nPrevAlarmDepth, bool isReal)
        {
            string strMessage = "";
            string strTag = isReal ? "" : "[테스트]";

            if (sensor == null)
            {
                strMessage = strTag + string.Format("유해화학물질 누출의 알람 단계가 {0}단계에서 {1}단계로 변경되었습니다.", nPrevAlarmDepth, nAlarmDepth);
            }
            else
            {
                PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
                string strMaterialName = material == null ? "유해화학물질" : material.Name;
                // update by mwkim 2016-05-11 : Sensor가 연결된 Tank의 LocationName을 EquipmentZone의 DisplayText로 변경
                string strLocationName = equipZone != null ? equipZone.DisplayText : "";

                strMessage = string.Format("{0}[{1}]에서 탐지된 {2} 누출의 알람 단계가 {3}단계에서 {4}단계로 변경되었습니다", strTag, strLocationName, strMaterialName, nPrevAlarmDepth, nAlarmDepth);
            }
            return strMessage;
        }

        // 수동 신고된 신호 복구
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
            string strMessage = GetClearManualPSMMessage(alarm);
            string strEquipZoneID = null;
            ProcessManager.DetectionStatus detectionStatus = ProcessManager.DetectionStatus.REAL;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.END_STATUS, strMessage, strEquipZoneID, alarm.SensorZoneID.ToString(), null, null, null, status, dbMgr))
            {
                if (dbMgr.BatchCommit())
                {
                    alarm.Message = strMessage;
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
            WriteLog("Remove.ManualAlarm 실패 : " + alarm.SensorZoneHistoryID.ToString());
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        // 누출신호 복구
        private int RemoveAlarm(SensorZoneGroup group, SensorZone sensorZone, bool isReal)
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
                dbMgr.BatchRollback();
                dbMgr.Close();
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

            string strMessage = GetClearPSMMessage(sensorZone.EquipZone, isReal);
            string strEquipZoneID = sensorZone.EquipZone == null ? null : sensorZone.EquipZone.ID.ToString();
            ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.END_STATUS, strMessage, strEquipZoneID, sensorZone.ID.ToString(), null, null, null, status, dbMgr))
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

        private string GetClearPSMMessage(EquipmentZone equipZone, bool isReal)
        {
            string strMessage = "상황해제";

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    strMessage = strTag + "탐지된 누출신호가 현장 복구되었습니다";
                else
                    strMessage = string.Format("{0}[{1}]에서 탐지된 누출신호가 현장 복구되었습니다", strTag, equipZone.DisplayText);
            }
            else
            {
                if (equipZone == null)
                    strMessage = "[테스트]탐지된 누출신호가 복구되었습니다";
                else
                    strMessage = string.Format("[테스트][{0}]에서 탐지된 누출신호가 복구되었습니다", equipZone.DisplayText);
            }

            return strMessage;
        }

        private string GetDetectPSMMessage(PSMSensor sensor, EquipmentZone equipZone, bool isReal)
        {
            string strMessage = "";

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (sensor == null)
                {
                    strMessage = strTag + "유해화학물질 누출이 탐지되었습니다";
                }
                else
                {
                    PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
                    string strMaterialName = material == null ? "유해화학물질" : material.Name;

                    // update by mwkim 2016-05-11 : Sensor가 연결된 Tank의 LocationName을 EquipmentZone의 DisplayText로 변경
                    string strLocationName = equipZone != null ? equipZone.DisplayText : "";
                    strMessage = string.Format("{0}[{1}]에서 {2} 누출이 탐지되었습니다", strTag, strLocationName, strMaterialName);
                }
            }
            else
            {
                if (sensor == null)
                {
                    strMessage = "[테스트]유해화학물질 누출이 탐지되었습니다";
                }
                else
                {
                    PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
                    string strMaterialName = material == null ? "유해화학물질" : material.Name;

                    // update by mwkim 2016-05-11 : Sensor가 연결된 Tank의 LocationName을 EquipmentZone의 DisplayText로 변경
                    string strLocationName = equipZone != null ? equipZone.DisplayText : "";
                    strMessage = string.Format("[테스트][{0}]에서 {1} 누출이 탐지되었습니다", strLocationName, strMaterialName);
                }
            }

            return strMessage;
        }

        private string GetDetectPSMMessageSiteID3(PSMSensor sensor, EquipmentZone equipZone, int nSensorData, bool isReal)
        {
            string strMessage = "";

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (sensor == null)
                {
                    strMessage = strTag + "유해화학물질 누출이 탐지되었습니다";
                }
                else
                {
                    // 광교 실내공기질
                    if (m_dbMgr.SiteID == 3 && (sensor.Name == "산소" || sensor.Name == "이산화탄소" || sensor.Name == "일산화탄소" || sensor.Name == "메탄"))
                    {
                        string strLevel = "";
                        if (nSensorData == (int)UnE.Alarm.AlarmType.PSM_ALARM_1)
                            strLevel = "주의단계";
                        else if (nSensorData == (int)UnE.Alarm.AlarmType.PSM_ALARM_2)
                            strLevel = "위험단계";
                        strMessage = string.Format("밀폐공간에서 [{0}] {1} 알람이 발생했습니다.", sensor.Name, strLevel);
                    }
                    else
                    {
                        PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
                        string strMaterialName = material == null ? "유해화학물질" : material.Name;

                        // update by mwkim 2016-05-11 : Sensor가 연결된 Tank의 LocationName을 EquipmentZone의 DisplayText로 변경
                        string strLocationName = equipZone != null ? equipZone.DisplayText : "";
                        strMessage = string.Format("{0}[{1}]에서 {2} 누출이 탐지되었습니다", strTag, strLocationName, strMaterialName);
                    }
                }
            }
            else
            {
                if (sensor == null)
                {
                    strMessage = "[테스트]유해화학물질 누출이 탐지되었습니다";
                }
                else
                {
                    // 광교 실내공기질
                    if (m_dbMgr.SiteID == 3 && (sensor.Name == "산소" || sensor.Name == "이산화탄소" || sensor.Name == "일산화탄소" || sensor.Name == "메탄"))
                    {
                        string strLevel = "";
                        if (nSensorData == (int)UnE.Alarm.AlarmType.PSM_ALARM_1)
                            strLevel = "주의단계";
                        else if (nSensorData == (int)UnE.Alarm.AlarmType.PSM_ALARM_2)
                            strLevel = "위험단계";
                        strMessage = string.Format("밀폐공간에서 [{0}] {1} 알람이 발생했습니다.", sensor.Name, strLevel);
                    }
                    else
                    {
                        PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
                        string strMaterialName = material == null ? "유해화학물질" : material.Name;

                        // update by mwkim 2016-05-11 : Sensor가 연결된 Tank의 LocationName을 EquipmentZone의 DisplayText로 변경
                        string strLocationName = equipZone != null ? equipZone.DisplayText : "";
                        strMessage = string.Format("[테스트][{0}]에서 {1} 누출이 탐지되었습니다", strLocationName, strMaterialName);
                    }
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
