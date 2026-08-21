using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using AgentFactory.BLL;
using dnsData.Alarm;
using dnsData.Sensor;
using SDMS.IDAL;
using SDMS.Model.History;
using SDMS.Model.Sensor;
using SDMS.Model.Spatial;
using SDMS.Model.Alarm;
using dnsSopID;
using Common.Model.History;
using TeamEditor.Model.Sop.Team;

namespace SOPWebServer.BLL.Process
{
    using Models;

    public class AlarmManager : IAlarmManager
    {
        // Key : SensorZoneHistory ID
        private ConcurrentDictionary<int, AlarmData> m_dicSensorZoneHistoryIDAlarms = new ConcurrentDictionary<int, AlarmData>();
        private MainManager m_mainManager = null;
        private SensorManager m_sensorManager = null;

        public ICollection<AlarmData> CurrentAlarms
        {
            get { return m_dicSensorZoneHistoryIDAlarms.Values; }
        }

        public AlarmManager(MainManager mainManager, SensorManager sensorManager)
        {
            m_mainManager = mainManager;
            m_sensorManager = sensorManager;

            ReadSensorHistory(m_mainManager.SDMSDataManager);
        }

        private string GetParamValue(string strParam)
        {
            if (strParam == null)
                return "NULL";

            return "'" + strParam + "'";
        }

        // DB 오류 또는 그 밖의 원인으로 인하여 SensorZoneHistory까지 만들어진 이후 SensorReactionHistory 생성에 실패한 Alarm을 삭제시킨다.
        public void RemoveAlarm(AlarmData alarm)
        {
            AlarmData data;
            m_dicSensorZoneHistoryIDAlarms.TryRemove(alarm.SensorZoneHistoryID, out data);
        }

        public bool RemoveAlarm(AlarmData alarm, DateTime timeStamp, int nReactionType, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, int? detectionStatus, IDataManager dataManager)
        {
            if (AddReactionHistory(alarm, nReactionType, timeStamp, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, dataManager) == false)
            {
                return false;
            }

            AlarmData data;

            if (m_dicSensorZoneHistoryIDAlarms.TryRemove(alarm.SensorZoneHistoryID, out data))
            {
                return true;
            }

            return false;
        }

        // CurrentAlarm을 삭제한다.
        // DB에서 CurrentAlarm Table만 지운다.
        public void RemoveCurrentAlarm(int nSensorZoneHistoryID)
        {
            string strErrorMessage;
            Dictionary<CurrentAlarm.Fields, object> dicConditions = new Dictionary<CurrentAlarm.Fields, object>();
            dicConditions[CurrentAlarm.Fields.SensorZoneHistoryID] = nSensorZoneHistoryID;

            if (m_mainManager.SDMSDataManager.GetDeleteManager().DeleteCurrentAlarm(dicConditions, null, out strErrorMessage))
            {
                AlarmData alarm;
                m_dicSensorZoneHistoryIDAlarms.TryRemove(nSensorZoneHistoryID, out alarm);
            }
        }

        // 잘못 생성된 알람일 경우 SensorZoneHistory를 지운다.
        public void RemoveSensorZoneHistory(int nSensorZoneHistoryID)
        {
            string strErrorMessage;
            m_mainManager.SDMSDataManager.GetDeleteManager().DeleteSensorZoneHistory(nSensorZoneHistoryID, out strErrorMessage);
        }

        public void SetAlarm(int nSensorZoneHistoryID, AlarmData alarm)
        {
            m_dicSensorZoneHistoryIDAlarms[nSensorZoneHistoryID] = alarm;
        }

        public AlarmData GetAlarm(int nSensorZoneHistoryID)
        {
            AlarmData alarm;

            if (m_dicSensorZoneHistoryIDAlarms.TryGetValue(nSensorZoneHistoryID, out alarm))
                return alarm;

            return null;
        }

        // 수동신고된 알람 검색
        public AlarmData GetManualAlarm(int nZoneID, Facility.FacilityType facility, IDataManager dataManager)
        {
            ICollection<AlarmData> alarms = CurrentAlarms;
            string strSensorZoneHistoryIDs = "";
            Dictionary<int, AlarmData> dicAlarms = new Dictionary<int, AlarmData>();

            foreach (AlarmData alarm in alarms)
            {
                if (alarm.SensorZoneID == 0 && alarm.SensorType == facility && alarm.Status == SensorReactionHistory.ReactionTypes.NOTIFY_SIGNAL)
                {
                    if (strSensorZoneHistoryIDs.Length == 0)
                        strSensorZoneHistoryIDs = alarm.SensorZoneHistoryID.ToString();
                    else
                        strSensorZoneHistoryIDs += ", " + alarm.SensorZoneHistoryID.ToString();

                    dicAlarms[alarm.SensorZoneHistoryID] = alarm;
                }
            }

            if (strSensorZoneHistoryIDs.Length > 0)
            {
                string strErrorMessage;
                bool isNullable;

                string strConditions = string.Format("{0} in ({1}) and {2} = {3}",
                    SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.ID, out isNullable),
                    strSensorZoneHistoryIDs,
                    SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.ZoneID, out isNullable),
                    nZoneID);

                List<SensorZoneHistory> sensorZoneHistories = dataManager.GetSelectManager().SelectSensorZoneHistories(null, strConditions, out strErrorMessage);

                if (sensorZoneHistories == null)
                    return null;

                AlarmData alarm = null;

                foreach (SensorZoneHistory szh in sensorZoneHistories)
                {
                    if (dicAlarms.TryGetValue(szh.ID, out alarm))
                        return alarm;
                }
            }

            return null;
        }

        public AlarmData AddAlarm(int nSensorZoneID, int nSensorData, int nZoneID, int nSensorType, int? detectionStatus, DateTime timeStamp, IDataManager dataManager, FacilityManager.DetectTypes detectType)
        {
            List<int> allSensorZoneIDs = new List<int>();
            allSensorZoneIDs.Add(nSensorZoneID);
            SensorZoneHistory szh = dataManager.GetCreateManager().CreateSensorZoneHistory(nSensorZoneID, nSensorData.ToString(), timeStamp, nZoneID, nSensorType, detectionStatus, dataManager.SiteID, null, allSensorZoneIDs);
            
            if (szh == null)
            {
                return null;
            }

            AlarmData alarm = new AlarmData();

            alarm.SensorZoneID = nSensorZoneID;
            alarm.SensorZoneHistoryID = szh.ID;
            alarm.TimeStamp = timeStamp;

            if (nSensorZoneID > 0)
            {
                SensorZone sensorZone = m_sensorManager.GetSensorZone(nSensorZoneID);

                if (sensorZone != null)
                    alarm.SensorType = (Facility.FacilityType)sensorZone.SensorType;
            }

            m_dicSensorZoneHistoryIDAlarms[szh.ID] = alarm;
            return alarm;
        }

        // 이미 존재하는 알람에 특정 센서가 추가되었을 경우 DB 정보를 갱신해준다.
        public void AddAlarmSensor(KeyValuePair<SensorZone, int>[] sensors, int nSensorZoneHistoryID, IDataManager dataManager)
        {
            List<int> allSensorZoneIDs = null;

            foreach (KeyValuePair<SensorZone, int> pair in sensors)
            {
                SensorZone sensor = pair.Key;

                if (allSensorZoneIDs == null)
                    allSensorZoneIDs = new List<int>();

                allSensorZoneIDs.Add(sensor.ID);
            }

            string strErrorMessage;
            SensorZoneHistory szh = dataManager.GetSelectManager().SelectSensorZoneHistory(nSensorZoneHistoryID, out strErrorMessage);

            if (szh == null)
                return;

            if (szh.AllSensorZoneIDs != null)
            {
                foreach (int id in szh.AllSensorZoneIDs)
                {
                    if (allSensorZoneIDs == null)
                        allSensorZoneIDs = new List<int>();

                    if (allSensorZoneIDs.Contains(id) == false)
                        allSensorZoneIDs.Add(id);
                }
            }

            Dictionary<SensorZoneHistory.Fields, object> dicSets = new Dictionary<SensorZoneHistory.Fields, object>();
            Dictionary<SensorZoneHistory.Fields, object> dicConditions = new Dictionary<SensorZoneHistory.Fields, object>();
            dicConditions[SensorZoneHistory.Fields.ID] = nSensorZoneHistoryID;
            dicSets[SensorZoneHistory.Fields.AllSensorZoneIDs] = allSensorZoneIDs;

            dataManager.GetUpdateManager().UpdateSensorZoneHistory(dicSets, dicConditions, null, out strErrorMessage);
        }

        public bool AddReactionHistory(AlarmData alarm, int nReactionType, DateTime timeStamp, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, IDataManager dataManager)
        {
            SensorReactionHistory srh = dataManager.GetCreateManager().CreateSensorReactionHistory(alarm.SensorZoneHistoryID, nReactionType, timeStamp, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5);

            if (srh != null)
            {
                alarm.SensorReactionHistoryID = srh.ID;
                alarm.ReactionHistoryParam1 = strParam1 == null ? "" : strParam1;
                alarm.ReactionHistoryParam2 = strParam2 == null ? "" : strParam2;
                alarm.ReactionHistoryParam3 = strParam3 == null ? "" : strParam3;
                alarm.ReactionHistoryParam4 = strParam4 == null ? "" : strParam4;
                alarm.ReactionHistoryParam5 = strParam5 == null ? "" : strParam5;

                //// 센서 신호로 인해 실행된 SOP 이력에 센서 신호 종료 시간을 입력해준다
                //string strErrorMessage;
                //Dictionary<ActionStepHistory.Fields, object> dicCondition = new Dictionary<ActionStepHistory.Fields, object>();
                //dicCondition.Add(ActionStepHistory.Fields.SensorZoneHistoryID, srh.SensorZoneHistoryID);

                //List<ActionStepHistory> actionStepHistory = m_mainManager.CommonDataManager.GetSelectManager().SelectActionStepHistories(dicCondition, "EndTime Is null", out strErrorMessage);
                //if (actionStepHistory != null && actionStepHistory.Count > 0)
                //{
                //    actionStepHistory[0].DetectEndTime = timeStamp;
                //    if (m_mainManager.CommonDataManager.GetUpdateManager().UpdateActionStepHistory(actionStepHistory[0], out strErrorMessage))
                //        return true;
                //    else
                //        return false;
                //}

                return true;
            }

            return false;
        }

        public bool AddReactionHistoryDescription(int sensorReactionHistoryID, int sensorZoneHistoryID, string strMemo, IDataManager dataManager)
        {
            Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicConditions = new Dictionary<SensorReactionHistoryDescriptionText.Fields, object>();
            dicConditions[SensorReactionHistoryDescriptionText.Fields.Description] = strMemo;

            string strErrorMessage;

            List<SensorReactionHistoryDescriptionText> texts = dataManager.GetSelectManager().SelectSensorReactionHistoryDescriptionTexts(dicConditions, "", out strErrorMessage);

            if (texts == null)
                return false;

            SensorReactionHistoryDescriptionText text = null;

            if (texts.Count == 0)
            {
                text = dataManager.GetCreateManager().CreateSensorReactionHistoryDescriptionText(1, strMemo);
            }
            else
            {
                text = texts[0];
            }

            if (text == null)
                return false;

            SensorReactionHistoryDescription description = dataManager.GetCreateManager().CreateSensorReactionHistoryDescription(sensorReactionHistoryID, text.ID, sensorZoneHistoryID);
            return description != null;
        }

        // SensorZoneData가 알람상태인지 검사한다.
        private bool IsAlarmSensorZoneData(int nSensorZoneID, string strSensorData)
        {
            if (nSensorZoneID == 0)
            {
                // nSensorZoneID가 0인 경우는 수동화재신고 이므로 SensorData를 신경쓰지 않는다.
                return true;
            }

            SensorZone sensorZone = m_sensorManager.GetSensorZone(nSensorZoneID);

            if (sensorZone == null)
                return false;

            // 지진센서의 경우 SensorData에 알람단계가 아니라 진도 또는 규모가 기록되어 있다.
            if (BroadcastManager.IsEarthquakeSensor((Facility.FacilityType)sensorZone.SensorType))
                return true;

            if (strSensorData == null)
                return false;

            int nSensorData;

            if (int.TryParse(strSensorData, out nSensorData) == false)
                return false;

            if (BroadcastManager.IsETCSensor((Facility.FacilityType)sensorZone.SensorType))
            {
                return nSensorData > 0;
            }

            if (nSensorData == (int)AlarmData.AlarmType.ALARM ||
                nSensorData == (int)AlarmData.AlarmType.PSM_ALARM_1 ||
                nSensorData == (int)AlarmData.AlarmType.PSM_ALARM_2 ||
                nSensorData == (int)AlarmData.AlarmType.PSM_ALARM_3)
                return true;

            return false;
        }

        // sz가 알람상태인지 검사한다.
        private bool IsAlarmSensorZoneData(SensorZone sz)
        {
            if (sz == null || sz.Data == null)
            {
                return false;
            }

            int nSensorData = (int)sz.Data;

            if (BroadcastManager.IsETCSensor((Facility.FacilityType)sz.SensorType))
            {
                return nSensorData > 0;
            }

            if (nSensorData == (int)AlarmData.AlarmType.ALARM ||
                nSensorData == (int)AlarmData.AlarmType.PSM_ALARM_1 ||
                nSensorData == (int)AlarmData.AlarmType.PSM_ALARM_2 ||
                nSensorData == (int)AlarmData.AlarmType.PSM_ALARM_3)
                return true;

            return false;
        }

        // 현재 Alarm이 발생중인 SensorZone에 대한 Query 조건문
        private string GetAlarmSensorZoneQueryString()
        {
            string strCondition = ((int)AlarmData.AlarmType.ALARM).ToString();

            strCondition += ", " + ((int)AlarmData.AlarmType.PSM_ALARM_1).ToString();
            strCondition += ", " + ((int)AlarmData.AlarmType.PSM_ALARM_2).ToString();
            strCondition += ", " + ((int)AlarmData.AlarmType.PSM_ALARM_3).ToString();
            /*strCondition += ", " + ((int)PSMManager.HistoryDataType.PSM_ALARM_1).ToString();
            strCondition += ", " + ((int)PSMManager.HistoryDataType.PSM_ALARM_2).ToString();
            strCondition += ", " + ((int)PSMManager.HistoryDataType.PSM_ALARM_3).ToString();*/

            return "(" + strCondition + ")";
        }

        // 현재 Alarm이 발생중인 SensorReactionLog에 대한 Query 조건문
        private string GetAlarmReactionHistoryQueryString()
        {
            string strCondition = ((int)SensorReactionHistory.ReactionTypes.BEGIN_STATUS).ToString();
            strCondition += ", " + ((int)SensorReactionHistory.ReactionTypes.NOTIFY_SIGNAL).ToString();

            return strCondition;
        }

        //현재 Alarm이 꺼진 SensorReactionLog에 대한 Query조건문
        private string GetAlarmOffReactionHistoryQueryString()
        {
            string strCondition = ((int)SensorReactionHistory.ReactionTypes.MALFUNCTION).ToString();
            strCondition += ", " + ((int)SensorReactionHistory.ReactionTypes.IGNORE_SIGNAL).ToString();
            strCondition += ", " + ((int)SensorReactionHistory.ReactionTypes.IGNORE_SOP).ToString();
            strCondition += ", " + ((int)SensorReactionHistory.ReactionTypes.END_STATUS).ToString();
            strCondition += ", " + ((int)SensorReactionHistory.ReactionTypes.USER_RESET).ToString();
            strCondition += ", " + ((int)SensorReactionHistory.ReactionTypes.TIME_OUT).ToString();

            // strCondition += ", " + ((int)SensorReactionLog.ReactionType.END_S1SVMS_STATUS).ToString();

            return strCondition;
        }

        // Server가 켜지기 전에 이미 발생해있던 알람들을 읽어온다.
        public void ReadSensorHistory(IDataManager dataManager)
        {
            string strErrorMessage;
            ArrayList arrDatas = dataManager.GetSelectManager().SelectCurrentAlarmHistories(GetAlarmReactionHistoryQueryString(), GetAlarmOffReactionHistoryQueryString(), out strErrorMessage);

            if (arrDatas == null)
                return;

            DateTime dtNow = DateTime.Now;
            DateTime dtYesterday = dtNow.AddDays(-1.0);
            int nDataCount = arrDatas.Count;

            for (int i=0;i<nDataCount-4;i+=5)
            {
                if (arrDatas[i] is EquipmentZone &&
                    arrDatas[i + 1] is SensorReactionHistory &&
                    arrDatas[i + 2] is SensorZone &&
                    arrDatas[i + 3] is SensorZoneHistory &&
                    arrDatas[i + 4] is Zone)
                {
                    EquipmentZone equipZone = (EquipmentZone)arrDatas[i];
                    SensorReactionHistory srh = (SensorReactionHistory)arrDatas[i + 1];
                    SensorZone sz = (SensorZone)arrDatas[i + 2];
                    SensorZoneHistory szh = (SensorZoneHistory)arrDatas[i + 3];
                    Zone z = (Zone)arrDatas[i + 4];

                    // 하루가 경과된 알람들은 종료처리한다.
                    if (srh.Time < dtYesterday)
                    {
                        TimeoutAlarm(dataManager, szh.ID, dtNow);
                        continue;
                    }

                    if (szh.AllSensorZoneIDs == null || szh.AllSensorZoneIDs.Count == 0)
                    {
                        if (IsAlarmSensorZoneData(sz))
                            CheckAlarmSensorZone(sz.ID, (Facility.FacilityType)sz.SensorType, (int)srh.ReactionType, (int)szh.DetectionStatus, szh.ID, srh.ID, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, dataManager);
                    }
                    else
                    {
                        // SensorZoneHistory의 Param3에는 현재 발생한 알람과 연관된 센서중 작동한 SensorZone ID들이 담겨있다.
                        // [2019/10/31] 김지웅
                        Dictionary<int, int> dicSensorZoneDatas = GetSensorZoneDatas(szh.AllSensorZoneIDs, dataManager);

                        int nSensorZoneData;

                        foreach (int nSensorZoneID in szh.AllSensorZoneIDs)
                        {
                            if (Header.ManualReportDefaultID <= nSensorZoneID)
                            {
                                // 수동신고일 경우
                                CheckAlarmSensorZone(nSensorZoneID, (Facility.FacilityType)sz.SensorType, (int)srh.ReactionType, (int)szh.DetectionStatus, szh.ID, srh.ID, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, dataManager);
                            }
                            else if (dicSensorZoneDatas.TryGetValue(nSensorZoneID, out nSensorZoneData))
                            {
                                // 센서신호일 경우
                                if (IsAlarmSensorZoneData(sz))
                                    CheckAlarmSensorZone(nSensorZoneID, (Facility.FacilityType)sz.SensorType, (int)srh.ReactionType, (int)szh.DetectionStatus, szh.ID, srh.ID, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, dataManager);
                            }
                        }
                    }
                }
            }

            // 서버가 켜지기 이전에 발생한 알람들에 대한 SOP 실행상태를 검사한다.
            CheckAlarmSOPState(dataManager);
        }

        // 서버가 켜지기 이전에 발생한 알람들에 대한 SOP 실행상태를 검사한다.
        private void CheckAlarmSOPState(IDataManager dataManager)
        {
            ReadCurrentAlarms();

            string strSensorZoneHistoryIDs = "";
            ICollection<AlarmData> alarms = CurrentAlarms;

            foreach (AlarmData alarm in alarms)
            {
                if (alarm.SensorZoneHistoryID > 0)
                {
                    if (strSensorZoneHistoryIDs.Length == 0)
                        strSensorZoneHistoryIDs = alarm.SensorZoneHistoryID.ToString();
                    else
                        strSensorZoneHistoryIDs += ", " + alarm.SensorZoneHistoryID.ToString();
                }
            }

            if (strSensorZoneHistoryIDs.Length == 0)
                return;

            bool isNullable;
            string strErrorMessage;
            string strCondition = string.Format("{0} in ({1})", ActionStepHistory.GetFieldName(ActionStepHistory.Fields.SensorZoneHistoryID, out isNullable), strSensorZoneHistoryIDs);
            List<ActionStepHistory> actionStepHistories = m_mainManager.CommonDataManager.GetSelectManager().SelectActionStepHistories(strCondition, out strErrorMessage);

            if (actionStepHistories == null)
                return;

            foreach (ActionStepHistory ash in actionStepHistories)
            {
                if (ash.SensorZoneHistoryID == null)
                    continue;

                AlarmData alarm = GetAlarm((int)ash.SensorZoneHistoryID);

                if (alarm != null)
                {
                    alarm.SOPProcess = AlarmData.SOPProcessType.Run;
                }
            }
        }

        private void ReadCurrentAlarms()
        {
            string strErrorMessage;
            List<CurrentAlarm> alarms = m_mainManager.SDMSDataManager.GetSelectManager().SelectCurrentAlarms(null, null, out strErrorMessage);

            if (alarms == null)
                return;

            string strSensorZoneHistoryIDs = "";

            foreach (CurrentAlarm alarm in alarms)
            {
                if (strSensorZoneHistoryIDs.Length == 0)
                    strSensorZoneHistoryIDs = alarm.SensorZoneHistoryID.ToString();
                else
                    strSensorZoneHistoryIDs += ", " + alarm.SensorZoneHistoryID.ToString();
            }

            if (strSensorZoneHistoryIDs.Length == 0)
                return;

            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.SensorZoneHistoryID, out isNullable), strSensorZoneHistoryIDs);

            List<SensorReactionHistory> sensorReactionHistories = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorReactionHistories(null, strCondition, out strErrorMessage);

            if (sensorReactionHistories == null)
                return;

            strCondition = string.Format("{0} in ({1})", SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.ID, out isNullable), strSensorZoneHistoryIDs);
            List<SensorZoneHistory> sensorZoneHistories = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorZoneHistories(null, strCondition, out strErrorMessage);

            if (sensorZoneHistories == null)
                return;

            Dictionary<int, int> dicSensorZoneIDs = new Dictionary<int, int>();
            Dictionary<int, SensorZoneHistory> dicSensorZoneHistories = new Dictionary<int, SensorZoneHistory>();

            foreach (SensorZoneHistory szh in sensorZoneHistories)
            {
                dicSensorZoneHistories[szh.ID] = szh;

                if (szh.AllSensorZoneIDs != null)
                {
                    foreach (int nSensorZoneID in szh.AllSensorZoneIDs)
                    {
                        dicSensorZoneIDs[nSensorZoneID] = nSensorZoneID;
                    }
                }
            }

            // 알람 리스트를 받았으니, 알람에 연관된 SensorZone들의 상태정보도 검사한다.
            if (dicSensorZoneIDs.Count > 0)
            {
                string strIDs = "";

                foreach (KeyValuePair<int, int> pair in dicSensorZoneIDs)
                {
                    if (strIDs.Length == 0)
                        strIDs = pair.Value.ToString();
                    else
                        strIDs += ", " + pair.Value.ToString();
                }

                strCondition = string.Format("{0} in ({1})", SensorZone.GetFieldName(SensorZone.Fields.ID, out isNullable), strIDs);
                List<SensorZone> sensorZones = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorZones(null, strCondition, out strErrorMessage);

                if (sensorZones != null)
                {
                    foreach (SensorZone sensorZone in sensorZones)
                    {
                        SensorZone sz = m_mainManager.SensorManager.GetSensorZone(sensorZone.ID);

                        if (sz != null)
                        {
                            sz.Data = sensorZone.Data;
                            sz.IsAlarmStatus = sensorZone.IsAlarmStatus;
                        }
                        else
                        {
                            m_mainManager.SensorManager.AddSensorZone(sensorZone);
                        }
                    }
                }
            }

            foreach (SensorReactionHistory srh in sensorReactionHistories)
            {
                AlarmData alarm;

                if (m_dicSensorZoneHistoryIDAlarms.TryGetValue(srh.SensorZoneHistoryID, out alarm) == false)
                {
                    SensorZoneHistory szh;

                    if (dicSensorZoneHistories.TryGetValue(srh.SensorZoneHistoryID, out szh) == false)
                        continue;

                    alarm = new AlarmData();

                    alarm.SensorZoneHistoryID = srh.SensorZoneHistoryID;
                    alarm.SensorReactionHistoryID = srh.ID;
                    alarm.TimeStamp = srh.Time;
                    alarm.Message = srh.Message;
                    alarm.SensorZoneID = szh.SensorZoneID;
                    alarm.Status = srh.ReactionType;
                    alarm.IsReal = szh.DetectionStatus == SensorZoneHistory.DetectionType.Real;

                    int nAlarmDepth;

                    if (srh.Param5 != null && int.TryParse(srh.Param5.Trim(), out nAlarmDepth))
                        alarm.AlarmDepth = nAlarmDepth;

                    m_dicSensorZoneHistoryIDAlarms[szh.ID] = alarm;

                    SensorZoneGroup group = m_mainManager.SensorManager.GetSensorZoneGroup(szh.SensorZoneID);

                    if (group != null)
                    {
                        group.CurrentAlarm = alarm;

                        if (szh.AllSensorZoneIDs != null)
                        {
                            foreach (int nSensorZoneID in szh.AllSensorZoneIDs)
                            {
                                SensorZone sensorZone = m_mainManager.SensorManager.GetSensorZone(nSensorZoneID);

                                if (sensorZone != null && sensorZone.Data != null && sensorZone.Data > 0 && sensorZone.IsAlarmStatus)
                                {
                                    group.SetSensorData(sensorZone, (int)sensorZone.Data, sensorZone.IsAlarmStatus, null, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        private string ListToString(List<int> datas)
        {
            string strDatas = "";

            foreach (int data in datas)
            {
                if (strDatas.Length == 0)
                    strDatas = data.ToString();
                else
                    strDatas += ", " + data.ToString();
            }

            return strDatas;
        }

        private Dictionary<int, int> GetSensorZoneDatas(List<int> sensorZoneIDs, IDataManager dataManager)
        {
            Dictionary<int, int> dicSensorZoneDatas = new Dictionary<int, int>();
            string strCondition = string.Format("ID in ({0})", ListToString(sensorZoneIDs));

            string strErrorMessage;
            List<SensorZone> sensorZones = dataManager.GetSelectManager().SelectSensorZones(null, strCondition, out strErrorMessage);

            if (sensorZones == null)
                return dicSensorZoneDatas;

            foreach (SensorZone sz in sensorZones)
            {
                if (sz.Data != null)
                    dicSensorZoneDatas[sz.ID] = (int)sz.Data;
            }

            return dicSensorZoneDatas;
        }

        private void CheckAlarmSensorZone(int nSensorZoneID, Facility.FacilityType sensorType, int nReactionType, int nDetectionStatus, int nSensorZoneHistoryID, int nSensorReactionHistoryID, DateTime time, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, IDataManager dataManager)
        {
            if (sensorType == Facility.FacilityType.NONE)
            {
                SensorZone sensorZone = m_sensorManager.GetSensorZone(nSensorZoneID);

                if (sensorZone != null)
                    sensorType = (Facility.FacilityType)sensorZone.SensorType;
            }

            // SensorType을 알수 없으면 재난 타입을 알수 없다.
            if (sensorType == Facility.FacilityType.NONE)
                return;

            SensorReactionHistory.ReactionTypes type = SensorReactionHistory.ToReactionType(nReactionType);
            SensorZoneHistory.DetectionType status = SensorZoneHistory.ToDetectionType(nDetectionStatus);

            if (type == SensorReactionHistory.ReactionTypes.ETC || status == SensorZoneHistory.DetectionType.None)
                return;

            if (type == SensorReactionHistory.ReactionTypes.BEGIN_STATUS || type == SensorReactionHistory.ReactionTypes.NOTIFY_SIGNAL || type == SensorReactionHistory.ReactionTypes.CHANGE_ALARM_DEPTH)
            {
                if (BaseBroadcastManager.IsPSMSensor(sensorType))
                    AddPSMAlarm(dataManager, type, nSensorZoneHistoryID, nSensorReactionHistoryID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, nSensorZoneID, status);
                else
                    AddSensorAlarm(dataManager, type, nSensorZoneHistoryID, nSensorReactionHistoryID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, nSensorZoneID, status);
            }
        }

        private void TimeoutAlarm(IDataManager dataManager, int nSensorZoneHistoryID, DateTime time)
        {
            dataManager.GetCreateManager().CreateSensorReactionHistory(nSensorZoneHistoryID, (int)SensorReactionHistory.ReactionTypes.TIME_OUT, time, "알람발생후 만 하루가 경과하여 알람을 초기화합니다.", null, null, null, null, null);
        }

        private AlarmData AddAlarm(IDataManager dataManager, SensorReactionHistory.ReactionTypes type, int nSensorZoneHistoryID, int nSensorReactionHistoryID, DateTime timeStamp, string strMessage, int nSensorZoneID, SensorZoneHistory.DetectionType detectionStatus)
        {
            if (detectionStatus == SensorZoneHistory.DetectionType.Malfunction)
                return null;

            SensorZone sensorZone = m_sensorManager.GetSensorZone(nSensorZoneID);

            if (sensorZone == null)
                return null;

            SensorZoneGroup group = m_sensorManager.GetSensorZoneGroup(nSensorZoneID);

            if (group == null)
                return null;

            AlarmData alarm = null;

            if (group.GetSensors().Length > 0 && group.CurrentAlarm != null)
            {
                alarm = group.CurrentAlarm;
            }
            else
            {
                alarm = new AlarmData();

                alarm.SensorZoneHistoryID = nSensorZoneHistoryID;
                alarm.SensorReactionHistoryID = nSensorReactionHistoryID;
                alarm.TimeStamp = timeStamp;
                alarm.Message = strMessage;
                alarm.SensorZoneID = nSensorZoneID;
                alarm.Status = type;
                alarm.SensorType = (Facility.FacilityType)sensorZone.SensorType;
                alarm.IsReal = detectionStatus == SensorZoneHistory.DetectionType.Real;

                Dictionary<SMSHistory.Fields, object> dicCondition = new Dictionary<SMSHistory.Fields, object>();
                dicCondition[SMSHistory.Fields.SensorZoneHistoryID] = nSensorZoneHistoryID;
                dicCondition[SMSHistory.Fields.SensorReactionHistoryID] = nSensorReactionHistoryID;

                string strErrorMessage;
                List<SMSHistory> histories = dataManager.GetSelectManager().SelectSMSHistories(dicCondition, "", out strErrorMessage);

                if (histories != null)
                {
                    foreach (SMSHistory history in histories)
                    {
                        if (history.RegularMemberIDList != null)
                        {
                            SetMemberIDs(alarm.RegularMemberIDs, history.RegularMemberIDList);
                            SetRegularPhoneNumbers(alarm);
                        }
                    }
                }

                m_dicSensorZoneHistoryIDAlarms[nSensorZoneHistoryID] = alarm;
            }

            int nSensorZoneData = GetSensorZoneData(dataManager, nSensorZoneID);
            group.SetSensorData(sensorZone, nSensorZoneData, alarm != null, null);

            group.CurrentAlarm = alarm;

            if (alarm != null)
                alarm.AlarmDepth = nSensorZoneData;

            return alarm;
        }

        private int GetSensorZoneData(IDataManager dataManager, int nSensorZoneID)
        {
            string strErrorMessage;
            SensorZone sz = dataManager.GetSelectManager().SelectSensorZone(nSensorZoneID, out strErrorMessage);

            if (sz == null || sz.Data == null)
                return -1;

            return (int)sz.Data;
        }

        private void AddSensorAlarm(IDataManager dataManager, SensorReactionHistory.ReactionTypes type, int nSensorZoneHistoryID, int nSensorReactionHistoryID, DateTime timeStamp, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, int nSensorZoneID, SensorZoneHistory.DetectionType detectionStatus)
        {
            AlarmData alarm = AddAlarm(dataManager, type, nSensorZoneHistoryID, nSensorReactionHistoryID, timeStamp, strMessage, nSensorZoneID, detectionStatus);

            if (alarm != null)
            {
                alarm.ReactionHistoryParam1 = strParam1;
                alarm.ReactionHistoryParam2 = strParam2;
                alarm.ReactionHistoryParam3 = strParam3;
                alarm.ReactionHistoryParam4 = strParam4;
                alarm.ReactionHistoryParam5 = strParam5;
            }
        }

        private void AddPSMAlarm(IDataManager dataManager, SensorReactionHistory.ReactionTypes type, int nSensorZoneHistoryID, int nSensorReactionHistoryID, DateTime timeStamp, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, int nSensorZoneID, SensorZoneHistory.DetectionType detectionStatus)
        {
            AlarmData alarm = null;

            if (type == SensorReactionHistory.ReactionTypes.BEGIN_STATUS || type == SensorReactionHistory.ReactionTypes.NOTIFY_SIGNAL)
            {
                alarm = AddAlarm(dataManager, type, nSensorZoneHistoryID, nSensorReactionHistoryID, timeStamp, strMessage, nSensorZoneID, detectionStatus);

                if (alarm != null)
                    alarm.AlarmDepth = alarm.AlarmDepth - (int)AlarmData.AlarmType.PSM_ALARM_1 + 1;
            }
            else if (type == SensorReactionHistory.ReactionTypes.CHANGE_ALARM_DEPTH)
            {
                if (m_dicSensorZoneHistoryIDAlarms.TryGetValue(nSensorZoneHistoryID, out alarm) == false)
                    return;
            }

            if (alarm == null)
                return;

            alarm.SensorReactionHistoryID = nSensorReactionHistoryID;
            alarm.Message = strMessage;
            alarm.TimeStamp = timeStamp;
            alarm.Status = type;
            alarm.ReactionHistoryParam1 = strParam1;
            alarm.ReactionHistoryParam2 = strParam2;
            alarm.ReactionHistoryParam3 = strParam3;
            alarm.ReactionHistoryParam4 = strParam4;
            alarm.ReactionHistoryParam5 = strParam5;

            SensorZone sensorZone = m_sensorManager.GetSensorZone(nSensorZoneID);

            if (sensorZone != null)
            {
                SensorZoneGroup group = m_sensorManager.GetSensorZoneGroup(nSensorZoneID);

                if (group != null)
                    group.SetSensorData(sensorZone, alarm.AlarmDepth + (int)AlarmData.AlarmType.PSM_ALARM_1 - 1, true, null);
            }
        }

        private void SetRegularPhoneNumbers(AlarmData alarm)
        {
            string strIDs = "";

            foreach (KeyValuePair<int, int> pair in alarm.RegularMemberIDs)
            {
                if (strIDs.Length == 0)
                    strIDs = pair.Value.ToString();
                else
                    strIDs += ", " + pair.Value.ToString();
            }

            if (strIDs.Length == 0)
                return;

            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", RegularMember.GetFieldName(RegularMember.Fields.ID, out isNullable));

            string strErrorMessage;
            List<RegularMember> members = m_mainManager.TeamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);

            if (members == null)
                return;

            foreach (RegularMember member in members)
            {
                if (member.PhoneNumber.Length > 0)
                {
                    alarm.PhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                }
            }
        }

        private void SetMemberIDs(Dictionary<int, int> dicMemberIDs, string strMemberIDs)
        {
            int nID;
            string[] strIDs = strMemberIDs.Split(',');

            foreach (string strID in strIDs)
            {
                if (int.TryParse(strID.Trim(), out nID))
                    dicMemberIDs[nID] = nID;
            }
        }

        private void SetMemberIDs(Dictionary<int, int> dicMemberIDs, List<int> memberIDs)
        {
            foreach (int id in memberIDs)
            {
                dicMemberIDs[id] = id;
            }
        }

        // group에 alarm 이외에 다른 alarm이 이미 존재하는지 확인한다.
        public bool CheckAlarmDuplication(AlarmData alarm, SensorZoneGroup group, SensorManager sensorManager)
        {
            ICollection<AlarmData> currentAlarms = this.CurrentAlarms;

            foreach (AlarmData _alarm in currentAlarms)
            {
                if (alarm == _alarm)
                    continue;

                SensorZoneGroup _group = sensorManager.GetSensorZoneGroup(_alarm.SensorZoneID);

                if (_group == group)
                    return true;
            }

            return false;
        }
    }
}
