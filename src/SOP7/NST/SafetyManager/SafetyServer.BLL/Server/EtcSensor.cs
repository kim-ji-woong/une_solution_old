using System;
using System.Collections;
using System.Collections.Generic;
using dnsSopID;
using dnsData.Sensor;
using SDMS.Model.Sensor;
using SDMS.Model.History;
using SDMS.Model.Spatial;
using SDMS.Model.CCTV;
using dnsData.Alarm;
using AgentFactory.BLL;
using Common.Model.History;

namespace SafetyServer.BLL.Server
{
    using Data.Response;
    using Data.Models;

    public class EtcSensor : BaseServer
    {
        private MainManager m_mainManager = null;
        private bool m_initialized = false;

        private Dictionary<Facility.FacilityType, SDMS.Model.Sensor.Option.Etc> m_sensorTypeOptions = new Dictionary<Facility.FacilityType, SDMS.Model.Sensor.Option.Etc>();
        // 알람단계별 옵션데이터
        // Dicionary.Key ; AlarmDepth
        private Dictionary<SDMS.Model.Sensor.Option.Etc, Dictionary<int, SDMS.Model.Sensor.Option.EtcData>> m_optionSensorData = new Dictionary<SDMS.Model.Sensor.Option.Etc, Dictionary<int, SDMS.Model.Sensor.Option.EtcData>>();

        public EtcSensor(MainManager mainManager, Factory factory)
            : base(factory)
        {
            m_mainManager = mainManager;
            m_agent = factory.MakeAgent(Factory.AgentType.Etc);
        }

        protected override void OnLoadEvent()
        {
            //ReadPrevAlarmSOP();
            m_initialized = true;
        }

        // 이전에 발생했던 알람에 대한 SOP 실행여부를 확인한다.
        /*private void ReadPrevAlarmSOP()
        {
            ICollection<AlarmData> alarms = ((Process.AlarmManager)m_mainManager.AlarmManager).CurrentAlarms;
            string strSensorZoneHistoryIDs = "";

            foreach (AlarmData alarm in alarms)
            {
                if (Facility.IsETCSensorType(alarm.SensorType))
                {
                    if (strSensorZoneHistoryIDs.Length == 0)
                        strSensorZoneHistoryIDs = alarm.SensorZoneHistoryID.ToString();
                    else
                        strSensorZoneHistoryIDs += ", " + alarm.SensorZoneHistoryID.ToString();
                }
            }

            bool isNullable;
            string strCondition = "";
            if (strSensorZoneHistoryIDs.Length > 0)
                strCondition = string.Format("{0} in ({1})", ActionStepHistory.GetFieldName(ActionStepHistory.Fields.SensorZoneHistoryID, out isNullable), strSensorZoneHistoryIDs);

            string strErrorMessage;
            List<ActionStepHistory> actionStepHistories = m_mainManager.CommonDataManager.GetSelectManager().SelectActionStepHistories(strCondition, out strErrorMessage);

            if (actionStepHistories == null)
            {
                System.Diagnostics.Trace.WriteLine(strErrorMessage);
                return;
            }

            foreach (ActionStepHistory history in actionStepHistories)
            {
                if (history.SensorZoneHistoryID == null)
                    continue;

                AlarmData alarm = ((Process.AlarmManager)m_mainManager.AlarmManager).GetAlarm((int)history.SensorZoneHistoryID);

                if (alarm != null)
                {
                    alarm.SOPProcess = AlarmData.SOPProcessType.Run;
                }
            }
        }*/

        protected override Result OnReceiveEvent(int header, string strClientInfo, ArrayList arrDatas)
        {
            // 초기화되기 전에는 통신 데이터를 처리하지 않는다.
            if (m_initialized == false)
                return new Result(true);

            if (header == Header.SENSOR_DATA)
                return ProcessSensorData(header, arrDatas, true);
            else if (header == Header.SENSOR_DATA_TEST)
                return ProcessSensorData(header, arrDatas, false);
            else if (header == Header.SENSOR_MALFUNCTION || header == Header.SENSOR_USER_RESET)
                return ProcessSensorData(header, arrDatas, false);
            else if (header == Header.MANUAL_REPORT)
                return ProcessManualReport(arrDatas);
            else if (header == Header.CLEAR_MANUAL_REPORT)
                return ProcessClearManualReport(arrDatas);

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.UNKNOWN_COMMAND));
        }

        private MessageResult ProcessSensorData(int header, ArrayList arrDatas, bool isReal)
        {
            if (arrDatas.Count >= 8 &&
                arrDatas[0] is int &&
                arrDatas[1] is int &&
                arrDatas[2] is int &&
                arrDatas[3] is string &&
                arrDatas[4] is string &&
                arrDatas[5] is DateTime &&
                arrDatas[6] is int &&
                (arrDatas[7] == null || arrDatas[7] is string))
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                string strMemberID = (string)arrDatas[3];
                string strCameraID = (string)arrDatas[4];
                DateTime timeStamp = (DateTime)arrDatas[5];
                int nAlarmLevel = (int)arrDatas[6];
                string strMessage = (string)arrDatas[7];

                // 외부에서 받은 신호인가?
                bool signalFromSystem = false;

                if (arrDatas.Count >= 9 && arrDatas[8] is bool)
                {
                    signalFromSystem = (bool)arrDatas[8];
                }

                string strErrorMessage;

                Facility.FacilityType sensorType = Facility.ToFacilityType(nSensorType);
                SensorZoneGroup group = m_mainManager.SensorManager.GetSensorZoneGroup(nSensorZoneID);

                if (group == null)
                    return GetErrorMessageResult(ErrorMessageType.UNKNOWN_SENSOR_ID);

                if (nAlarmLevel < 0 && strCameraID.Length == 0)
                {
                    GetAlarmTagFromReactionHistory(group.CurrentAlarm, out strErrorMessage);
                    strCameraID = group.CurrentAlarm.Tag.ToString();
                }

                if (UpdateSensorData(nSensorZoneID, strCameraID, strMessage, out strErrorMessage) == false)
                    return new MessageResult(false, strErrorMessage);

                

                SensorZone sensorZone = m_mainManager.SensorManager.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                    return GetErrorMessageResult(ErrorMessageType.UNKNOWN_SENSOR_ID);

                if (nAlarmLevel > 0)
                {
                    // 알람 발생
                    AlarmData alarm;
                    int nResult = AddAlarm(group, nSensorTagID, sensorZone, isReal, nAlarmLevel, strMessage, out alarm);

                    if (alarm != null)
                    {
                        if (strMessage != null && strMessage.Length > 0)
                            alarm.Message = strMessage;

                        alarm.Tag = strCameraID;
                        SetAlarmTagToReactionHistory(alarm, out strErrorMessage);

                        m_mainManager.ProcessManager.NewAlarm(alarm, group.GetAlarmSensorZoneIDs());
                    }

                    if (nResult == ErrorMessageType.SUCCESS)
                        return new MessageResult(true, "");

                    return new MessageResult(false, ErrorMessageType.ToMessage(nResult));
                }
                else
                {
                    // 알람 해제
                    AlarmData alarm = group.CurrentAlarm;

                    if (alarm == null)
                    {
                        return new MessageResult(false, "Alarm is alreay clear");
                    }

                    alarm.Tag = signalFromSystem;
                    AlarmData alarmPrev = alarm != null ? alarm.Clone() : null;

                    int nResult = RemoveAlarm(group, sensorZone, isReal, header);

                    if (alarm != null && group.CurrentAlarm == null)
                    {
                        alarm.Status = SensorReactionHistory.ReactionTypes.END_STATUS;
                        m_agentFactory.ProcessManager.ClearAlarm(alarm);
                    }
                    else if (alarm != null && group.CurrentAlarm != null)
                    {
                        int nAlarmDepth = m_agent.GetAlarmDepth(m_mainManager.AlarmManager, group.GetSensors(), null);
                        alarm.AlarmDepth = nAlarmDepth;

                        ChangeAlarm(group.CurrentAlarm, alarmPrev, group, sensorZone, 0);
                    }

                    if (nResult == ErrorMessageType.SUCCESS)
                        return new MessageResult(true, "");

                    return new MessageResult(false, ErrorMessageType.ToMessage(nResult));
                }
            }

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.INVALID_MESSAGE));
        }

        private bool GetAlarmTagFromReactionHistory(AlarmData alarm, out string strErrorMessage)
        {
            strErrorMessage = null;

            Dictionary<SensorReactionHistory.Fields, object> dicConditions = new Dictionary<SensorReactionHistory.Fields, object>();

            dicConditions[SensorReactionHistory.Fields.SensorZoneHistoryID] = alarm.SensorZoneHistoryID;
            dicConditions[SensorReactionHistory.Fields.ReactionType] = (int)SensorReactionHistory.ReactionTypes.BEGIN_STATUS;

            List<SensorReactionHistory> histories = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorReactionHistories(dicConditions, null, out strErrorMessage);

            if (histories == null)
                return false;

            if (histories.Count > 0)
            {
                alarm.Tag = histories[0].Param4;
            }

            return true;
        }

        private bool SetAlarmTagToReactionHistory(AlarmData alarm, out string strErrorMessage)
        {
            strErrorMessage = null;

            if (alarm.Tag == null)
                return true;

            Dictionary<SensorReactionHistory.Fields, object> dicConditions = new Dictionary<SensorReactionHistory.Fields, object>();
            Dictionary<SensorReactionHistory.Fields, object> dicSets = new Dictionary<SensorReactionHistory.Fields, object>();

            dicConditions[SensorReactionHistory.Fields.ID] = alarm.SensorReactionHistoryID;
            dicSets[SensorReactionHistory.Fields.Param4] = alarm.Tag.ToString();

            return m_mainManager.SDMSDataManager.GetUpdateManager().UpdateSensorReactionHistory(dicSets, dicConditions, null, out strErrorMessage);
        }

        private bool UpdateSensorData(int nSensorZoneID, string strCameraID, string strMessage, out string strErrorMessage)
        {
            SensorZone sensorZone = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorZone(nSensorZoneID, out strErrorMessage);

            if (sensorZone == null)
            {
                if (strErrorMessage == null)
                    strErrorMessage = string.Format("{0}에 해당하는 SensorZone Data를 찾을수 없습니다.", nSensorZoneID);

                return false;
            }

            if (sensorZone.OrgSensorID == null)
            {
                strErrorMessage = "SensorZone에 OrgSensor 정보가 기입되어 있지 않습니다.";
                return false;
            }

            Dictionary<CCTV.Fields, object> dicConditions = new Dictionary<CCTV.Fields, object>();
            dicConditions[CCTV.Fields.UniqueKey] = strCameraID;

            List<CCTV> cctvs = m_mainManager.SDMSDataManager.GetSelectManager().SelectCCTVs(dicConditions, null, out strErrorMessage);

            if (cctvs == null)
                return false;

            if (cctvs.Count == 0)
            {
                strErrorMessage = string.Format("{0}에 해당하는 CCTV를 찾을수 없습니다.", strCameraID);
                return false;
            }

            CCTV cctv = cctvs[0];

            ETC etcSensor = m_mainManager.SDMSDataManager.GetSelectManager().SelectETCSensor((int)sensorZone.OrgSensorID, out strErrorMessage);

            if (etcSensor == null)
            {
                if (strErrorMessage == null)
                    strErrorMessage = string.Format("{0}에 해당하는 EtcSensor를 찾을수 없습니다.", sensorZone.OrgSensorID);

                return false;
            }

            etcSensor.DepartmentPhoneNumber = strMessage;
            etcSensor.Status = cctv.ID.ToString();

            return m_mainManager.SDMSDataManager.GetUpdateManager().UpdateCCTV(cctv, out strErrorMessage);
        }

        private void ChangeAlarm(AlarmData alarmCurrent, AlarmData alarmPrev, SensorZoneGroup group, SensorZone sensorZone, int sensorData)
        {
            m_mainManager.ProcessManager.UpdateAlarm(alarmCurrent, group.GetAlarmSensorZoneIDs());

            if (alarmCurrent.AlarmDepth != alarmPrev.AlarmDepth)
            {
                m_mainManager.ProcessManager.ChangeAlarm(alarmCurrent, alarmPrev);
                string strLocationName = group.EquipmentZone != null ? group.EquipmentZone.DisplayText : "";

                alarmCurrent.TimeStamp = DateTime.Now;
                alarmCurrent.Status = SensorReactionHistory.ReactionTypes.CHANGE_ALARM_DEPTH;
                alarmCurrent.Message = GetChangeAlarmDepthString(Facility.ToFacilityType(sensorZone.SensorType), alarmCurrent.AlarmDepth, alarmPrev.AlarmDepth, alarmCurrent.IsReal, group.EquipmentZone);

                string strParam3 = ((int)sensorZone.SensorType).ToString();
                string strParam4 = sensorData.ToString(); // 0: 알람해제로 인한 단계 변경, 1: 알람발생으로 인한 단계 변경
                string strParam5 = alarmCurrent.AlarmDepth.ToString();
                ((Process.AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarmCurrent, (int)alarmCurrent.Status, alarmCurrent.TimeStamp, alarmCurrent.Message, sensorZone.EquipZoneID.ToString(), sensorZone.ID.ToString(), strParam3, strParam4, strParam5, m_mainManager.SDMSDataManager);
            }
        }

        private Result ProcessManualReport(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 7 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is DateTime
                                    && arrDatas[4] is int && arrDatas[5] is string && arrDatas[6] is string)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nZoneID = (int)arrDatas[2];
                DateTime dtDateTime = (DateTime)arrDatas[3];
                int nAlarmDepth = (int)arrDatas[4];
                string strReportPerson = (string)arrDatas[5];
                string strMemo = (string)arrDatas[6];

                if (nSensorZoneID < Header.ManualReportDefaultID)
                    return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.UNKNOWN_SENSOR_ID));

                SensorZoneGroup group = m_mainManager.SensorManager.GetSensorZoneGroup(nSensorZoneID);
                if (group == null)
                    return GetErrorMessageResult(ErrorMessageType.UNKNOWN_SENSOR_ID);

                SensorZone sensorZone = m_mainManager.SensorManager.GetSensorZone(nSensorZoneID);
                if (sensorZone == null)
                    return GetErrorMessageResult(ErrorMessageType.UNKNOWN_SENSOR_ID);

                AlarmData alarm = ((Process.AlarmManager)m_mainManager.AlarmManager).GetManualAlarm(nZoneID, Facility.FacilityType.FIRE_SENSOR, m_mainManager.SDMSDataManager);
                if (alarm != null)
                    return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.ALREADY_PROCESSED));

                alarm = ((Process.AlarmManager)m_mainManager.AlarmManager).AddAlarm(nSensorZoneID, 1, nZoneID, nSensorType, (int)SensorZoneHistory.DetectionType.Real, dtDateTime, m_mainManager.SDMSDataManager, FacilityManager.DetectTypes.Detect);
                if (alarm == null)
                    return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.DB_EXCEPTION));

                alarm.AlarmDepth = nAlarmDepth;
                alarm.IsManual = true;
                alarm.IsReal = true;
                SensorReactionHistory.ReactionTypes reactionType = SensorReactionHistory.ReactionTypes.BEGIN_STATUS;

                string strMessage = GetFireManualReportString(nZoneID);
                string strParam1 = nZoneID.ToString();
                string strParam2 = nSensorZoneID.ToString();
                string strParam3 = strReportPerson;
                string strParam4 = strMemo;
                string strParam5 = alarm.AlarmDepth.ToString();

                if (((Process.AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)reactionType, dtDateTime, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, m_mainManager.SDMSDataManager))
                {
                    alarm.Message = strMessage;
                    alarm.IsReal = true;
                    alarm.Status = reactionType;

                    group.SetSensorData(sensorZone, 1, true, m_mainManager.SDMSDataManager);

                    m_mainManager.ProcessManager.NewAlarm(alarm, group.GetAlarmSensorZoneIDs());

                    return new Result(true);
                }
            }

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.INVALID_MESSAGE));
        }

        private Result ProcessClearManualReport(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSensorZoneHistoryID = (int)arrDatas[2];
                int nUserID = (int)arrDatas[3];

                AlarmData alarm = ((Process.AlarmManager)m_mainManager.AlarmManager).GetAlarm(nSensorZoneHistoryID);

                SensorZoneGroup group = m_mainManager.SensorManager.GetSensorZoneGroup(nSensorZoneID);

                if (group == null)
                    return GetErrorMessageResult(ErrorMessageType.UNKNOWN_SENSOR_ID);

                SensorZone sensorZone = m_mainManager.SensorManager.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                    return GetErrorMessageResult(ErrorMessageType.UNKNOWN_SENSOR_ID);

                if (alarm.IsManual)
                {
                    AlarmData alarmPrev = alarm != null ? alarm.Clone() : null;

                    int nResult = ErrorMessageType.SUCCESS;

                    if (group.RemoveSensorData(sensorZone, m_mainManager.SDMSDataManager) == false)
                    {
                        WriteLog("RemoveSensorData 실패 : " + sensorZone.ID.ToString());
                        return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.DB_EXCEPTION));
                    }

                    EquipmentZone equipZone = m_mainManager.SensorManager.GetEquipmentZone(sensorZone.EquipZoneID);
                    string strMessage = GetClearManualFireMessage(alarm);
                    string strEquipZoneID = equipZone == null ? null : sensorZone.EquipZoneID.ToString();

                    SensorZoneHistory.DetectionType detectionStatus = SensorZoneHistory.DetectionType.Real;
                    SensorReactionHistory.ReactionTypes reactionType = SensorReactionHistory.ReactionTypes.END_STATUS;

                    if (((Process.AlarmManager)m_mainManager.AlarmManager).RemoveAlarm(alarm, DateTime.Now, (int)reactionType, strMessage, strEquipZoneID, sensorZone.ID.ToString(), null, null, null, (int)detectionStatus, m_mainManager.SDMSDataManager))
                    {
                        alarm.Message = strMessage;
                        group.CurrentAlarm = null;
                        nResult = ErrorMessageType.SUCCESS;
                    }

                    if (alarm != null && group.CurrentAlarm == null)
                    {
                        alarm.Status = SensorReactionHistory.ReactionTypes.END_STATUS;
                        m_agentFactory.ProcessManager.ClearAlarm(alarm);
                    }

                    if (nResult == ErrorMessageType.SUCCESS)
                        return new Result(true);

                    return new MessageResult(false, ErrorMessageType.ToMessage(nResult));
                }
            }

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.INVALID_MESSAGE));
        }

        private int AddAlarm(SensorZoneGroup group, int nSensorTagID, SensorZone sensorZone, bool isReal, int nAlarmLevel, string strMessage, out AlarmData alarm)
        {
            alarm = null;

            // 알람발생 신호에 대해서만 센서 비활성화를 검사한다.
            // 이미 알람이 발생한 센서의 경우 센서가 비활성화 상태이더라도 알람을 해제할 수 있어야 한다.
            if (m_mainManager.SensorManager.IsActiveSensor(nSensorTagID) == false)
            {
                WriteLog("AddAlarm 무시(비활성화된 센서) : " + sensorZone.ID.ToString());
                return ErrorMessageType.SUCCESS;
            }

            AlarmData currentAlarm = group.CurrentAlarm;
            int nSensorDataCount = group.GetSensors().Length;

            if (currentAlarm == null && nSensorDataCount > 0)
            {
                //  논리적인 오류
                group.ClearSensorDatas(m_mainManager.SDMSDataManager);
            }
            else if (currentAlarm != null && nSensorDataCount > 0)
            {
                // 이미 알람이 발생중이다.
                // Sensor 데이터만 기록하고 종료한다.

                int data;
                bool isAlarmStatus;
                AlarmData alarmPrev = currentAlarm.Clone();

                int nAlarmDepth = m_agent.GetAlarmDepth(m_mainManager.AlarmManager, group.GetSensors(), sensorZone);
                currentAlarm.AlarmDepth = nAlarmDepth;

                if ((group.GetSensorData(sensorZone, out data, out isAlarmStatus) == false) || data == 0 || isAlarmStatus == false)
                {
                    group.SetSensorData(sensorZone, 1, true, m_mainManager.SDMSDataManager);
                    ((Process.AlarmManager)m_mainManager.AlarmManager).AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, m_mainManager.SDMSDataManager);

                    ChangeAlarm(currentAlarm, alarmPrev, group, sensorZone, 1);
                }

                return ErrorMessageType.SUCCESS;
            }
            else
            {
                int nAlarmDepth = m_agent.GetAlarmDepth(m_mainManager.AlarmManager, group.GetSensors(), sensorZone);

                // 기본 알람 단계가 주의 이상
                if (nAlarmDepth == 1)
                    nAlarmDepth = 2;

                group.SetSensorData(sensorZone, 1, true, m_mainManager.SDMSDataManager);

                SensorZoneHistory.DetectionType detectionStatus = isReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;

                DateTime timeStamp = DateTime.Now;
                int nZoneID = group.EquipmentZone == null || group.EquipmentZone.LinkedZoneIDs.Count == 0 ? -1 : group.EquipmentZone.LinkedZoneIDs[0];
                alarm = ((Process.AlarmManager)m_mainManager.AlarmManager).AddAlarm(sensorZone.ID, 1, nZoneID, sensorZone.SensorType, (int)detectionStatus, timeStamp, m_mainManager.SDMSDataManager, FacilityManager.DetectTypes.Detect);

                if (alarm != null)
                {
                    alarm.AlarmDepth = nAlarmDepth;
                    //alarm.AlarmDepth = 1;

                    // 알람 단계 전송시
                    if (nAlarmLevel != -1)
                        alarm.AlarmDepth = nAlarmLevel;

                    group.CurrentAlarm = alarm;

                    //string strMessage = GetDetectEtcMessage(Facility.ToFacilityType(sensorZone.SensorType), group.EquipmentZone, isReal);
                    string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                    SensorReactionHistory.ReactionTypes reactionType = SensorReactionHistory.ReactionTypes.BEGIN_STATUS;

                    string strParam3 = sensorZone.SensorType.ToString();
                    string strParam5 = alarm.AlarmDepth.ToString();

                    if (((Process.AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)reactionType, timeStamp, strMessage, strEquipZoneID, sensorZone.ID.ToString(), strParam3, null, strParam5, m_mainManager.SDMSDataManager))
                    {
                        alarm.Message = strMessage;
                        alarm.IsReal = isReal;
                        alarm.Status = reactionType;
                        return ErrorMessageType.SUCCESS;
                    }
                    else
                    {
                        group.RemoveSensorData(sensorZone, m_mainManager.SDMSDataManager);
                        ((Process.AlarmManager)m_mainManager.AlarmManager).RemoveAlarm(alarm);
                        WriteLog("AddReactionHistory 실패 : " + alarm.SensorZoneHistoryID.ToString());
                        alarm = null;
                    }
                }
                else
                {
                    group.RemoveSensorData(sensorZone, m_mainManager.SDMSDataManager);
                    WriteLog("AddAlarm 실패 : " + sensorZone.ID.ToString());
                }
            }

            return ErrorMessageType.DB_EXCEPTION;
        }

        public int RemoveAlarm(SensorZoneGroup group, SensorZone sensorZone, bool isReal, int header)
        {
            DateTime timeStamp = DateTime.Now;

            if (group.RemoveSensorData(sensorZone, m_mainManager.SDMSDataManager) == false)
            {
                WriteLog("RemoveSensorData 실패 : " + sensorZone.ID.ToString());
                return ErrorMessageType.DB_EXCEPTION;
            }

            // sensorZone의 신호는 복구되었지만 같은 영역에 다른 신호가 아직 남아있는 상황
            if (group.GetSensors().Length > 0 && group.CurrentAlarm != null)
            {
                return ErrorMessageType.SUCCESS;
            }

            AlarmData alarm = group.CurrentAlarm;

            if (alarm == null)
            {
                return ErrorMessageType.SUCCESS;
            }

            EquipmentZone equipZone = m_mainManager.SensorManager.GetEquipmentZone(sensorZone.EquipZoneID);
            string strMessage = GetClearEtcMessage(Facility.ToFacilityType(sensorZone.SensorType), equipZone, isReal);
            string strEquipZoneID = equipZone == null ? null : sensorZone.EquipZoneID.ToString();

            SensorZoneHistory.DetectionType detectionStatus = isReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
            SensorReactionHistory.ReactionTypes reactionType = SensorReactionHistory.ReactionTypes.END_STATUS;
            if (header == Header.SENSOR_MALFUNCTION)
                reactionType = SensorReactionHistory.ReactionTypes.MALFUNCTION;
            else if (header == Header.SENSOR_USER_RESET)
                reactionType = SensorReactionHistory.ReactionTypes.USER_RESET;

            if (((Process.AlarmManager)m_mainManager.AlarmManager).RemoveAlarm(alarm, timeStamp, (int)reactionType, strMessage, strEquipZoneID, sensorZone.ID.ToString(), null, null, null, (int)detectionStatus, m_mainManager.SDMSDataManager))
            {
                alarm.Message = strMessage;
                group.CurrentAlarm = null;
                return ErrorMessageType.SUCCESS;
            }

            WriteLog("RemoveAlarm 실패 : " + sensorZone.ID.ToString());
            return ErrorMessageType.DB_EXCEPTION;
        }

        private string GetDetectEtcMessage(Facility.FacilityType sensorType, EquipmentZone equipZone, bool isReal)
        {
            string strEventName = Facility.GetFacilityTypeString(sensorType) + " 신호";

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    return strTag + strEventName + "가 탐지되었습니다";
                else
                    return string.Format("{0}[{1}]에서 {2}가 탐지되었습니다", strTag, equipZone.DisplayText, strEventName);
            }

            if (equipZone == null)
                return string.Format("[테스트]{0}가 탐지되었습니다", strEventName);

            return string.Format("[테스트][{0}]에서 {1}가 탐지되었습니다", equipZone.DisplayText, strEventName);
        }

        private string GetClearEtcMessage(Facility.FacilityType sensorType, EquipmentZone equipZone, bool isReal)
        {
            string strEventName = Facility.GetFacilityTypeString(sensorType) + " 신호";
            string strMessage = "상황해제";

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    strMessage = strTag + strEventName + "가 복구되었습니다";
                else
                    strMessage = string.Format("{0}[{1}]에서 탐지된 {2}가 복구되었습니다", strTag, equipZone.DisplayText, strEventName);
            }
            else
            {
                if (equipZone == null)
                    strMessage = string.Format("[테스트]{0}가 복구되었습니다", strEventName);
                else
                    strMessage = string.Format("[테스트][{0}]에서 탐지된 {1}가 복구되었습니다", equipZone.DisplayText, strEventName);
            }

            return strMessage;
        }

        private string GetChangeAlarmDepthString(Facility.FacilityType sensorType, int nAlarmDepth, int nPrevAlarmDepth, bool isReal, EquipmentZone equipZone)
        {
            string strMessage = "";
            string strTag = isReal ? "" : "[테스트]";
            string strEventName = Facility.GetFacilityTypeString(sensorType) + " 신호";

            if (equipZone != null)
            {
                strMessage = string.Format("{0}[{1}]에서 탐지된 {2}의 알람 단계가 {3}단계에서 {4}단계로 변경되었습니다", strTag, equipZone.DisplayText, strEventName, nPrevAlarmDepth, nAlarmDepth);
            }
            else
            {
                strMessage = strTag + string.Format("탐지된 {0}의 알람 단계가 {1}단계에서 {2}단계로 변경되었습니다.", strEventName, nPrevAlarmDepth, nAlarmDepth);
            }
            return strMessage;
        }

        private string GetFireManualReportString(int nZoneID)
        {
            string strMessage = "";

            if (nZoneID < 0)
            {
                strMessage = "기타 상황이 신고되었습니다";
            }
            else
            {
                Zone zone = m_mainManager.SensorManager.GetZone(nZoneID);

                if (zone != null)
                {
                    string szLocationName = zone.DisplayText;
                    strMessage = string.Format("[{0}]에서 기타 상황이 신고되었습니다", szLocationName);
                }
            }

            return strMessage;
        }

        private string GetClearManualFireMessage(AlarmData alarm)
        {
            string strMessage = "신고된 기타 상황이 종료되었습니다";
            int nZoneID;

            if (int.TryParse(alarm.ReactionHistoryParam1, out nZoneID))
            {
                Zone zone = m_mainManager.SensorManager.GetZone(nZoneID);

                if (zone != null)
                {
                    strMessage = string.Format("[{0}]에서 신고된 기타 상황이 종료되었습니다", zone.DisplayText);
                }
            }

            return strMessage;
        }

        private string GetTrainingModeString()
        {
            return m_agentFactory.SMSManager.GetTrainingModeString();
        }
    }
}
