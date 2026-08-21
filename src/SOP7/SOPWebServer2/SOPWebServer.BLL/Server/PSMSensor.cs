using System;
using System.Collections;
using System.Collections.Generic;
using dnsSopID;
using dnsData.Sensor;
using SDMS.Model.Sensor;
using SDMS.Model.History;
using SDMS.Model.Spatial;
using dnsData.Alarm;
using AgentFactory.BLL;

namespace SOPWebServer.BLL.Server
{
    using Response;
    using Models;
    using SOPWebServer.BLL.Process;

    public class PSMSensor : BaseServer
    {
        private MainManager m_mainManager = null;

        public PSMSensor(MainManager mainManager, Factory factory)
            : base(factory)
        {
            m_mainManager = mainManager;
            m_agent = factory.MakeAgent(Factory.AgentType.PSM);
        }

        protected override void OnLoadEvent()
        {
        }

        protected override Result OnReceiveEvent(int header, string strClientInfo, ArrayList arrDatas)
        {
            if (header == Header.SENSOR_DATA)
                return ProcessSensorData(arrDatas, true);
            else if (header == Header.SENSOR_DATA_TEST)
                return ProcessSensorData(arrDatas, false);
            else if (header == Header.SENSOR_USER_RESET)
                return ProcessUserReset(arrDatas, true);
            else if (header == Header.SENSOR_MALFUNCTION)
                return ProcessUserReset(arrDatas, false);
            else if (header == Header.MANUAL_REPORT)
                return ProcessManualReport(arrDatas);
            else if (header == Header.CLEAR_MANUAL_REPORT)
                return ProcessClearManualReport(arrDatas);

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.UNKNOWN_COMMAND));
        }

        private Result ProcessUserReset(ArrayList arrDatas, bool userReset)
        {
            if (arrDatas.Count >= 2 && arrDatas[0] is int && arrDatas[1] is int)
            {
                int nSensorZoneID = (int)arrDatas[0];
                int nSOPGenUserID = (int)arrDatas[1];

                SensorZone sensorZone = m_mainManager.SensorManager.GetSensorZone(nSensorZoneID);
                SensorZoneGroup group = m_mainManager.SensorManager.GetSensorZoneGroup(nSensorZoneID);

                if (sensorZone == null || group == null)
                    return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.UNKNOWN_SENSOR_ID));

                // sensorZone과 연관된 모든 알람을 해제해야 한다.
                // 사용자 복구이므로 하나의 센서만 끄는게 아니라 같은 EquipZone 내의 모든 센서를 끈다.
                // 알람 해제
                AlarmData alarm = group.CurrentAlarm;

                string strErrorMessage;
                int nResult = ErrorMessageType.SUCCESS;

                foreach (KeyValuePair<SensorZone, int> pair in group.GetSensors())
                {
                    PSM sensor = m_mainManager.SDMSDataManager.GetSelectManager().SelectPSMSensor((int)sensorZone.OrgSensorID, out strErrorMessage);

                    if (sensor == null)
                        return new MessageResult(false, strErrorMessage);

                    int result = RemoveAlarm_UserReset(group, pair.Key, sensor, nSOPGenUserID, userReset);

                    if (result != ErrorMessageType.SUCCESS)
                        nResult = result;
                }

                if (alarm != null && group.CurrentAlarm == null)
                {
                    if (userReset)
                        alarm.Status = SensorReactionHistory.ReactionTypes.USER_RESET;
                    else
                        alarm.Status = SensorReactionHistory.ReactionTypes.MALFUNCTION;
                    m_agentFactory.ProcessManager.ClearAlarm(alarm);
                }

                if (nResult == ErrorMessageType.SUCCESS)
                    return new Result(true);

                return new MessageResult(false, ErrorMessageType.ToMessage(nResult));
            }

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.INVALID_MESSAGE));
        }

        // 탐지신호 사용자 복구
        private int RemoveAlarm_UserReset(SensorZoneGroup group, SensorZone sensorZone, PSM sensor, int nSOPGenUserID, bool userReset)
        {
            DateTime timeStamp = DateTime.Now;

            // 신호복구 처리는 SDMS에서 사용자에 의하여 보내기 때문에 특정 센서 뿐만 아니라
            // SensorZoneGroup내에 있는 모든 센서 데이터를 초기화 시킨다.
            if (group.RemoveAllSensorData(m_mainManager.SDMSDataManager) == false)
            {
                WriteLog("RemoveAllSensorData 실패 : " + sensorZone.ID.ToString());
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
            string strLocationName = GetLocationName(sensor, equipZone);

            string strMessage = GetUserResetMessage(strLocationName, alarm.IsReal);
            SensorZoneHistory.DetectionType detectionStatus = alarm.IsReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
            SensorReactionHistory.ReactionTypes reactionType = userReset ? SensorReactionHistory.ReactionTypes.USER_RESET : SensorReactionHistory.ReactionTypes.MALFUNCTION;

            string strParam1 = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
            string strParam2 = sensorZone.ID.ToString();
            string strParam3 = nSOPGenUserID.ToString();

            if (((Process.AlarmManager)m_mainManager.AlarmManager).RemoveAlarm(alarm, timeStamp, (int)reactionType, strMessage, strParam1, strParam2, strParam3, null, null, (int)detectionStatus, m_mainManager.SDMSDataManager))
            {
                alarm.Message = strMessage;
                group.CurrentAlarm = null;
                return ErrorMessageType.SUCCESS;
            }

            WriteLog("RemoveAlarm 실패 : " + sensorZone.ID.ToString());
            return ErrorMessageType.DB_EXCEPTION;
        }

        private Result ProcessSensorData(ArrayList arrDatas, bool isReal, bool clearAlarm = false)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                int nSensorData = (int)arrDatas[3];

                int nAlarmLevel = -1;

                if (arrDatas.Count > 4 && arrDatas[4] is int)
                    nAlarmLevel = (int)arrDatas[4];

                Facility.FacilityType sensorType = Facility.ToFacilityType(nSensorType);
                SensorZoneGroup group = m_mainManager.SensorManager.GetSensorZoneGroup(nSensorZoneID);

                if (group == null)
                    return GetErrorMessageResult(ErrorMessageType.UNKNOWN_SENSOR_ID);

                SensorZone sensorZone = m_mainManager.SensorManager.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                    return GetErrorMessageResult(ErrorMessageType.UNKNOWN_SENSOR_ID);

                string strErrorMessage;
                PSM sensor = m_mainManager.SDMSDataManager.GetSelectManager().SelectPSMSensor((int)sensorZone.OrgSensorID, out strErrorMessage);

                if (sensor == null)
                    return new MessageResult(false, strErrorMessage);

                if (nSensorData > 0)
                {
                    // 알람 신호 받지 않음
                    bool useReceive = m_mainManager.SensorManager.GetUseReceive(nSensorType);
                    if (!useReceive)
                        return new Result(true);

                    if (nSensorData < (int)AlarmData.AlarmType.PSM_ALARM_1)
                        nSensorData = (int)AlarmData.AlarmType.PSM_ALARM_1 - 1 + nSensorData;

                    // 알람 발생
                    AlarmData alarm, prevAlarm;
                    int nResult = AddAlarm(group, nSensorTagID, sensorZone, sensor, nSensorData, isReal, nAlarmLevel, out alarm, out prevAlarm);

                    if (alarm != null && prevAlarm != null)
                    {
                        m_mainManager.ProcessManager.UpdateAlarm(alarm, group.GetAlarmSensorZoneIDs());
                        m_mainManager.ProcessManager.ChangeAlarm(alarm, prevAlarm);
                    }
                    else if (alarm != null)
                    {
                        m_mainManager.ProcessManager.NewAlarm(alarm, group.GetAlarmSensorZoneIDs());
                    }

                    if (nResult == ErrorMessageType.SUCCESS)
                        return new Result(true);

                    return new MessageResult(false, ErrorMessageType.ToMessage(nResult));
                }
                else
                {
                    // 알람 해제
                    AlarmData alarm = group.CurrentAlarm;
                    
                    int nResult = RemoveAlarm(group, sensorZone, isReal);

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

        private int ChangeAlarm(AlarmData currentAlarm, SensorZoneGroup group, SensorZone sensorZone, PSM sensor, int nSensorData, bool isReal, ref AlarmData alarm, ref AlarmData prevAlarm)
        {
            KeyValuePair<SensorZone, int>[] sensorZoneDatas = group.GetSensors();
            int nAlarmDepth;
            SensorZone alarmSensorZone;

            string strAlarmLocationName = GetLocationName(sensor, group.EquipmentZone);

            if (IsChangedAlarmDepth(sensorZoneDatas, group, sensorZone, nSensorData, out nAlarmDepth, out alarmSensorZone))
            {
                // 알람단계가 바뀌었다.
                prevAlarm = currentAlarm;
                alarm = prevAlarm.Clone();

                alarm.TimeStamp = DateTime.Now;
                alarm.AlarmDepth = nAlarmDepth - (int)AlarmData.AlarmType.PSM_ALARM_1 + 1;
                alarm.Status = SensorReactionHistory.ReactionTypes.CHANGE_ALARM_DEPTH;
                alarm.SensorZoneID = alarmSensorZone.ID;
                //alarm.Message = GetChangeAlarmDepthString(alarmSensorZone.SensorType, alarm.AlarmDepth, prevAlarm.AlarmDepth, isReal, strAlarmLocationName);
                alarm.Message = GetChangeAlarmDepthString(sensor.MaterialType, alarm.AlarmDepth, prevAlarm.AlarmDepth, isReal, strAlarmLocationName);

                group.SetSensorData(sensorZone, nAlarmDepth, true, m_mainManager.SDMSDataManager);
                ((Process.AlarmManager)m_mainManager.AlarmManager).AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, m_mainManager.SDMSDataManager);

                string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                string strEquipZoneName = group.EquipmentZone == null ? "" : group.EquipmentZone.ZoneName;
                SensorZoneHistory.DetectionType detectionStatus = isReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;

                string strParam3 = ((int)sensorZone.SensorType).ToString();
                string strParam4 = "1"; // 0: 알람해제로 인한 단계 변경, 1: 알람발생으로 인한 단계 변경
                string strParam5 = alarm.AlarmDepth.ToString();

                if (((Process.AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)alarm.Status, alarm.TimeStamp, alarm.Message, strEquipZoneID, sensorZone.ID.ToString(), strParam3, strParam4, strParam5, m_mainManager.SDMSDataManager))
                {
                    group.CurrentAlarm = alarm;
                    ((Process.AlarmManager)m_mainManager.AlarmManager).SetAlarm(alarm.SensorZoneHistoryID, alarm);
                    return ErrorMessageType.SUCCESS;
                }
                else
                {
                    group.SetSensorData(sensorZone, (int)AlarmData.AlarmType.PSM_ALARM_1 - 1 + prevAlarm.AlarmDepth, true, m_mainManager.SDMSDataManager);
                    WriteLog("AddReactionHistory 실패 : " + alarm.SensorZoneHistoryID.ToString());
                    return ErrorMessageType.DB_EXCEPTION;
                }
            }
            else
            {
                // 알람단계가 바뀌지 않았으므로 Sensor 데이터만 기록하고 종료한다.
                group.SetSensorData(sensorZone, nSensorData, true, m_mainManager.SDMSDataManager);
                ((Process.AlarmManager)m_mainManager.AlarmManager).AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, m_mainManager.SDMSDataManager);
                return ErrorMessageType.SUCCESS;
            }
        }

        private int AddAlarm(SensorZoneGroup group, int nSensorTagID, SensorZone sensorZone, PSM sensor, int nSensorData, bool isReal, int nAlarmLevel, out AlarmData alarm, out AlarmData prevAlarm)
        {
            prevAlarm = alarm = null;

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
                // 알람단계가 바뀌었는지 확인한다.
                return ChangeAlarm(currentAlarm, group, sensorZone, sensor, nSensorData, isReal, ref alarm, ref prevAlarm);
                /*KeyValuePair<SensorZone, int>[] sensorZoneDatas = group.GetSensors();
                int nAlarmDepth;
                SensorZone alarmSensorZone;

                string strAlarmLocationName = GetLocationName(sensor, group.EquipmentZone);

                if (IsChangedAlarmDepth(sensorZoneDatas, group, sensorZone, nSensorData, out nAlarmDepth, out alarmSensorZone))
                {
                    // 알람단계가 바뀌었다.
                    prevAlarm = currentAlarm;
                    alarm = prevAlarm.Clone();

                    alarm.TimeStamp = DateTime.Now;
                    alarm.AlarmDepth = nAlarmDepth - (int)AlarmData.AlarmType.PSM_ALARM_1 + 1;
                    alarm.Status = SensorReactionHistory.ReactionTypes.CHANGE_ALARM_DEPTH;
                    alarm.SensorZoneID = alarmSensorZone.ID;                    
                    //alarm.Message = GetChangeAlarmDepthString(alarmSensorZone.SensorType, alarm.AlarmDepth, prevAlarm.AlarmDepth, isReal, strAlarmLocationName);
                    alarm.Message = GetChangeAlarmDepthString(sensor.MaterialType, alarm.AlarmDepth, prevAlarm.AlarmDepth, isReal, strAlarmLocationName);

                    group.SetSensorData(sensorZone, nAlarmDepth, true, m_mainManager.SDMSDataManager);
                    ((Process.AlarmManager)m_mainManager.AlarmManager).AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, m_mainManager.SDMSDataManager);

                    string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                    string strEquipZoneName = group.EquipmentZone == null ? "" : group.EquipmentZone.ZoneName;
                    SensorZoneHistory.DetectionType detectionStatus = isReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
                    
                    string strParam3 = ((int)sensorZone.SensorType).ToString();
                    string strParam4 = "1"; // 0: 알람해제로 인한 단계 변경, 1: 알람발생으로 인한 단계 변경
                    string strParam5 = alarm.AlarmDepth.ToString();

                    if (((Process.AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)alarm.Status, alarm.TimeStamp, alarm.Message, strEquipZoneID, sensorZone.ID.ToString(), strParam3, strParam4, strParam5, m_mainManager.SDMSDataManager))
                    {
                        group.CurrentAlarm = alarm;
                        ((Process.AlarmManager)m_mainManager.AlarmManager).SetAlarm(alarm.SensorZoneHistoryID, alarm);
                        return ErrorMessageType.SUCCESS;
                    }
                    else
                    {
                        group.SetSensorData(sensorZone, (int)AlarmData.AlarmType.PSM_ALARM_1 - 1 + prevAlarm.AlarmDepth, true, m_mainManager.SDMSDataManager);
                        WriteLog("AddReactionHistory 실패 : " + alarm.SensorZoneHistoryID.ToString());
                        return ErrorMessageType.DB_EXCEPTION;
                    }
                }
                else
                {
                    // 알람단계가 바뀌지 않았으므로 Sensor 데이터만 기록하고 종료한다.
                    group.SetSensorData(sensorZone, nSensorData, true, m_mainManager.SDMSDataManager);
                    ((Process.AlarmManager)m_mainManager.AlarmManager).AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, m_mainManager.SDMSDataManager);
                    return ErrorMessageType.SUCCESS;
                }*/
            }
            else
            {
                group.SetSensorData(sensorZone, nSensorData, true, m_mainManager.SDMSDataManager);

                DateTime timeStamp = DateTime.Now;
                int nZoneID = -1;

                if (group.EquipmentZone != null)
                {
                    if (group.EquipmentZone.LinkedZoneIDs.Count == 1)
                        nZoneID = group.EquipmentZone.LinkedZoneIDs[0];
                    else if (group.EquipmentZone.LinkedZoneIDs.Count > 1)
                    {
                        if (sensorZone.OrgSensorID == null)
                            nZoneID = -1;
                        else
                        {
                            string strErrorMessage = null;
                            PSM psm = m_mainManager.SDMSDataManager.GetSelectManager().SelectPSMSensor((int)sensorZone.OrgSensorID, out strErrorMessage);
                            if (psm == null)
                                nZoneID = -1;
                            else
                                nZoneID = psm.ZoneID;
                        }
                    }
                }
                SensorZoneHistory.DetectionType detectionStatus = isReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;

                alarm = ((Process.AlarmManager)m_mainManager.AlarmManager).AddAlarm(sensorZone.ID, nSensorData, nZoneID, sensorZone.SensorType, (int)detectionStatus, timeStamp, m_mainManager.SDMSDataManager, FacilityManager.DetectTypes.Detect);

                if (alarm != null)
                {
                    // 동기화 문제로 인하여 같은 SensorZoneGroup에 중복된 알람이 발생하지 않았는지 한번더 검사한다.
                    if (((Process.AlarmManager)m_mainManager.AlarmManager).CheckAlarmDuplication(alarm, group, m_mainManager.SensorManager))
                    {
                        // 이미 같은 SensorZoneGroup에 알람이 있기 때문에 해당 알람과 정보를 합친다.
                        ((Process.AlarmManager)m_mainManager.AlarmManager).RemoveCurrentAlarm(alarm.SensorZoneHistoryID);
                        ((Process.AlarmManager)m_mainManager.AlarmManager).RemoveSensorZoneHistory(alarm.SensorZoneHistoryID);
                        group.RemoveSensorData(sensorZone, m_mainManager.SDMSDataManager);

                        currentAlarm = group.CurrentAlarm;

                        if (currentAlarm != null)
                            return ChangeAlarm(currentAlarm, group, sensorZone, sensor, nSensorData, isReal, ref alarm, ref prevAlarm);
                        else
                            return ErrorMessageType.SUCCESS;
                    }

                    string strAlarmLocationName = GetLocationName(sensor, group.EquipmentZone);

                    // 기본 알람 단계 수정 1 >> 2
                    //alarm.AlarmDepth = nSensorData - (int)AlarmData.AlarmType.PSM_ALARM_1 + 1;
                    alarm.AlarmDepth = nSensorData - (int)AlarmData.AlarmType.PSM_ALARM_1 + 2;

                    // 알람 단계 전송시
                    if (nAlarmLevel != -1)
                        alarm.AlarmDepth = nAlarmLevel;

                    group.CurrentAlarm = alarm;

                    //string strMessage = GetDetectPSMMessage(sensorZone.SensorType, group.EquipmentZone, isReal, strAlarmLocationName);
                    string strMessage = GetDetectPSMMessage(sensor.MaterialType, group.EquipmentZone, isReal, strAlarmLocationName);
                    string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                    string strEquipZoneName = group.EquipmentZone == null ? "" : group.EquipmentZone.ZoneName;
                    SensorReactionHistory.ReactionTypes reactionType = SensorReactionHistory.ReactionTypes.BEGIN_STATUS;

                    if (((Process.AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)reactionType, timeStamp, strMessage, strEquipZoneID, sensorZone.ID.ToString(), sensor.ID.ToString(), strEquipZoneName, alarm.AlarmDepth.ToString(), m_mainManager.SDMSDataManager))
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

        private bool IsChangedAlarmDepth(KeyValuePair<SensorZone, int>[] sensorZoneDatas, SensorZoneGroup group, SensorZone sensorZone, int nSensorData, out int nAlarmDepth, out SensorZone alarmSensorZone)
        {
            // sensorZone의 알람값
            int nSensorZoneAlarmDepth = -1;
            // sensorZone을 제외한 나머지 센서들 값 중에서 가장 큰 값
            int nMaxAlarmDepth = -1;
            SensorZone maxSensorZone = null;

            int data;
            bool isAlarmStatus;

            foreach (KeyValuePair<SensorZone, int> pair in sensorZoneDatas)
            {
                SensorZone sensor = pair.Key;

                if (sensor == sensorZone)
                {
                    if (group.GetSensorData(sensor, out data, out isAlarmStatus))
                    {
                        nSensorZoneAlarmDepth = data;
                    }
                }
                else
                {
                    if (group.GetSensorData(sensor, out data, out isAlarmStatus))
                    {
                        if (maxSensorZone == null || nMaxAlarmDepth < data)
                        {
                            maxSensorZone = sensor;
                            nMaxAlarmDepth = data;
                        }
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

        public int RemoveAlarm(SensorZoneGroup group, SensorZone sensorZone, bool isReal)
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
                m_mainManager.ProcessManager.UpdateAlarm(group.CurrentAlarm, group.GetAlarmSensorZoneIDs());
                return ErrorMessageType.SUCCESS;
            }

            AlarmData alarm = group.CurrentAlarm;

            if (alarm == null)
            {
                return ErrorMessageType.SUCCESS;
            }

            EquipmentZone equipZone = m_mainManager.SensorManager.GetEquipmentZone(sensorZone.EquipZoneID);
            string strMessage = GetClearPSMMessage(equipZone, isReal);
            string strEquipZoneID = equipZone == null ? null : sensorZone.EquipZoneID.ToString();

            SensorZoneHistory.DetectionType detectionStatus = isReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
            SensorReactionHistory.ReactionTypes reactionType = isReal ? SensorReactionHistory.ReactionTypes.END_STATUS : SensorReactionHistory.ReactionTypes.USER_RESET;

            if (((Process.AlarmManager)m_mainManager.AlarmManager).RemoveAlarm(alarm, timeStamp, (int)reactionType, strMessage, strEquipZoneID, sensorZone.ID.ToString(), null, null, null, (int)detectionStatus, m_mainManager.SDMSDataManager))
            {
                alarm.Message = strMessage;
                group.CurrentAlarm = null;
                return ErrorMessageType.SUCCESS;
            }

            WriteLog("RemoveAlarm 실패 : " + sensorZone.ID.ToString());
            return ErrorMessageType.DB_EXCEPTION;
        }

        private string GetChangeAlarmDepthString(int? nMaterialType, int nAlarmDepth, int nPrevAlarmDepth, bool isReal, string strLocationName)
        {
            string strMessage = "";
            string strTag = isReal ? "" : "[테스트]";

            if (nMaterialType == null || nMaterialType < 0)
            {
                strMessage = strTag + string.Format("유해화학물질 누출의 알람 단계가 {0}단계에서 {1}단계로 변경되었습니다.", nPrevAlarmDepth, nAlarmDepth);
            }
            else
            {
                string strErrorMessage;
                //FacilityType type = m_mainManager.SDMSDataManager.GetSelectManager().SelectFacilityType(sensorType, out strErrorMessage);
                Material material = m_mainManager.SensorManager.GetMaterial(nMaterialType);

                string strMaterialName = "유해화학물질";
                //if (type != null && type.Description.Length > 0)
                //    strMaterialName = type.Description;
                if (material != null && material.MaterialName.Length > 0)
                    strMaterialName = material.MaterialName;
                strMessage = string.Format("{0}[{1}]에서 탐지된 {2} 누출의 알람 단계가 {3}단계에서 {4}단계로 변경되었습니다", strTag, strLocationName, strMaterialName, nPrevAlarmDepth, nAlarmDepth);
            }
            return strMessage;
        }

        private string GetFireManualReportString(int nZoneID)
        {
            string strMessage = "";

            if (nZoneID < 0)
            {
                strMessage = "누출이 신고되었습니다";
            }
            else
            {
                Zone zone = m_mainManager.SensorManager.GetZone(nZoneID);

                if (zone != null)
                {
                    string szLocationName = zone.DisplayText;
                    strMessage = string.Format("[{0}]에서 누출이 신고되었습니다", szLocationName);
                }
            }

            return strMessage;
        }

        private string GetClearManualFireMessage(AlarmData alarm)
        {
            string strMessage = "신고된 누출 상황이 종료되었습니다";
            int nZoneID;

            if (int.TryParse(alarm.ReactionHistoryParam1, out nZoneID))
            {
                Zone zone = m_mainManager.SensorManager.GetZone(nZoneID);

                if (zone != null)
                {
                    strMessage = string.Format("[{0}]에서 신고된 누출 상황이 종료되었습니다", zone.DisplayText);
                }
            }

            return strMessage;
        }

        //private string GetDetectPSMMessage(int sensorType, EquipmentZone equipZone, bool isReal, string strLocationName)
        private string GetDetectPSMMessage(int? nMaterialType, EquipmentZone equipZone, bool isReal, string strLocationName)
        {
            string strMessage = "";
            string strMaterialName = null;

            //string strMaterialName = Facility.GetNFacilityTypeString(sensorType);
            Material material = m_mainManager.SensorManager.GetMaterial(nMaterialType);
            if (material != null)
            {
                strMaterialName = material.MaterialName;
            }

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                //if (sensorType < 0)
                if (nMaterialType == null || nMaterialType < 0 || strMaterialName == null)
                {
                    strMessage = strTag + "유해화학물질 누출이 탐지되었습니다";
                }
                else
                {
                    strMessage = string.Format("{0}[{1}]에서 {2} 누출이 탐지되었습니다", strTag, strLocationName, strMaterialName);
                }
            }
            else
            {
                //if (sensorType < 0)
                if (nMaterialType == null || nMaterialType < 0 || strMaterialName == null)
                {
                    strMessage = "[테스트]유해화학물질 누출이 탐지되었습니다";
                }
                else
                {
                    strMessage = string.Format("[테스트][{0}]에서 {1} 누출이 탐지되었습니다", strLocationName, strMaterialName);
                }
            }

            return strMessage;
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

        private string GetUserResetMessage(string strLocationName, bool isReal)
        {
            if (isReal)
            {
                string strTag = GetTrainingModeString();
                return string.Format("{0}[{1}]에서 탐지된 누출신호가 시스템 복구되었습니다.", strTag, strLocationName);
            }
            else
            {
                return string.Format("[테스트][{0}]에서 탐지된 누출신호가 시스템 복구되었습니다", strLocationName);
            }
        }

        private string GetLocationName(PSM sensor, EquipmentZone equipZone)
        {
            if (sensor.PositionName != null && sensor.PositionName.Length > 0)
                return sensor.PositionName;

            return equipZone == null ? "" : equipZone.DisplayText;
        }

        private string GetTrainingModeString()
        {
            return m_agentFactory.SMSManager.GetTrainingModeString();
        }

    }
}
