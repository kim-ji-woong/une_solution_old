using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;
using DBUtility2;
using System.Collections;
using AgentFactory;
using UnE.Sensor;
using System.Text;

namespace ServerProcess.Data
{
    public class AlarmManager : IAlarmManager
    {
        private static AlarmManager m_instance = new AlarmManager();

        // Key : SensorZoneHistory ID
        private ConcurrentDictionary<int, AlarmData> m_dicSensorZoneHistoryIDAlarms = new ConcurrentDictionary<int, AlarmData>();

        public static AlarmManager Instance
        {
            get { return m_instance; }
        }

        public List<AlarmData> CurrentAlarms
        {
            get { return m_dicSensorZoneHistoryIDAlarms.Values.ToList(); }
        }

        private AlarmManager()
        {
        }

        public static int GetMaxTableID(DirectDBManager dbMgr, string strTableName, bool transaction)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = null;

            if (transaction)
                arrResult = dbMgr.GetBatchData(strSQL);
            else
                arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return 0;

            return id.Data;
        }

        public static string GetTimeString(DateTime timeStamp)
        {
            return string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);
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

        public bool RemoveAlarm(AlarmData alarm, DateTime timeStamp, int nReactionType, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, VariousData<int> detectionStatus, DirectDBManager dbMgr)
        {
            // Transaction 처리를 위하여 객체를 새로 만든다.
            /*dbMgr = dbMgr.Clone();

            if (dbMgr.BeginBatch() == false)
                return false;*/

            if (AddReactionHistory(alarm, nReactionType, timeStamp, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, detectionStatus, dbMgr, true) == false)
            {
                //dbMgr.BatchRollback();
                return false;
            }

            AlarmData data;

            if (m_dicSensorZoneHistoryIDAlarms.TryRemove(alarm.SensorZoneHistoryID, out data))
            {
                //dbMgr.BatchCommit();
                return true;
            }

            //dbMgr.BatchRollback();
            return false;
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
        public AlarmData GetManualAlarm(int nZoneID, IFacility.FacilityType facility, DirectDBManager dbMgr)
        {
            List<AlarmData> alarms = CurrentAlarms;
            string strSensorZoneHistoryIDs = "";
            Dictionary<int, AlarmData> dicAlarms = new Dictionary<int, AlarmData>();

            foreach (AlarmData alarm in alarms)
            {
                if (alarm.SensorZoneID == 0 && alarm.SensorType == facility && alarm.Status == BaseProcessManager.ReactionType.NOTIFY_SIGNAL)
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
                string strSQL = "Select ID from SensorZoneHistory where ID in (" + strSensorZoneHistoryIDs + ") and Param1 = '" + nZoneID.ToString() + "'";
                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return null;

                AlarmData alarm = null;
                int nResultCount = arrResult.Count;

                for (int i=0;i<nResultCount;i++)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                    if (dicAlarms.TryGetValue(nID, out alarm))
                        return alarm;
                }
            }

            return null;
        }

        public AlarmData AddAlarm(int nSensorZoneID, int nSensorData, string strParam1, string strParam2, string strParam3, DateTime timeStamp, DirectDBManager dbMgr)
        {
            // Transaction 처리를 위하여 객체를 새로 만든다.
            /*dbMgr = dbMgr.Clone();

            if (dbMgr.BeginBatch() == false)
                return null;*/

            int nID = GetMaxTableID(dbMgr, "SensorZoneHistory", true) + 1;
            string strTime = GetTimeString(timeStamp);

            strParam1 = GetParamValue(strParam1);
            strParam2 = GetParamValue(strParam2);

            // Param3에는 이 알람과 관련된 SensorZone ID들을 넣도록 한다.
            if (strParam3 != null)
                strParam3 = GetParamValue(strParam3);
            else
                strParam3 = GetParamValue(nSensorZoneID.ToString());

            string strSQL = "Insert into SensorZoneHistory (ID, SensorID, Connected, Data, Time, param1, param2, param3, SiteID, Description) values (";
            strSQL += string.Format("{0}, {1}, 1, {2}, '{3}', {4}, {5}, {6}, {7}, NULL)",
                nID, nSensorZoneID, nSensorData, strTime, strParam1, strParam2, strParam3, dbMgr.SiteID);

            if (dbMgr.GetBatchData(strSQL) == null)
            {
                //dbMgr.BatchRollback();
                return null;
            }

            /*if (dbMgr.BatchCommit() == false)
                return null;*/

            AlarmData alarm = new AlarmData();

            alarm.SensorZoneID = nSensorZoneID;
            alarm.SensorZoneHistoryID = nID;
            alarm.TimeStamp = timeStamp;

            if (nSensorZoneID > 0)
            {
                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone != null)
                    alarm.SensorType = sensorZone.Type;
            }

            m_dicSensorZoneHistoryIDAlarms[nID] = alarm;
            return alarm;
        }

        // 이미 존재하는 알람에 특정 센서가 추가되었을 경우 DB 정보를 갱신해준다.
        public void AddAlarmSensor(List<SensorZone> sensors, int nSensorZoneHistoryID, DirectDBManager dbMgr)
        {
            string strParam3 = "";

            foreach (SensorZone sensor in sensors)
            {
                if (strParam3.Length == 0)
                    strParam3 = sensor.ID.ToString();
                else
                    strParam3 += ", " + sensor.ID.ToString();
            }

            if (strParam3.Length == 0)
                strParam3 = "NULL";
            else
                strParam3 = "'" + strParam3 + "'";

            string strSQL = string.Format("Update SensorZoneHistory set Param3 = {0} where ID = {1}", strParam3, nSensorZoneHistoryID);
            dbMgr.GetResultData(strSQL);
        }

        public bool AddReactionHistory(AlarmData alarm, int nReactionType, DateTime timeStamp, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, VariousData<int> detectionStatus, DirectDBManager dbMgr, bool transaction)
        {
            string strTime = GetTimeString(timeStamp);
            string strStatus = detectionStatus == null ? "NULL" : detectionStatus.Data.ToString();

            string param1 = strParam1 == null ? "" : strParam1;
            string param2 = strParam2 == null ? "" : strParam2;
            string param3 = strParam3 == null ? "" : strParam3;
            string param4 = strParam4 == null ? "" : strParam4;
            string param5 = strParam5 == null ? "" : strParam5;

            strParam1 = GetParamValue(strParam1);
            strParam2 = GetParamValue(strParam2);
            strParam3 = GetParamValue(strParam3);
            strParam4 = GetParamValue(strParam4);
            strParam5 = GetParamValue(strParam5);

            int nID = GetMaxTableID(dbMgr, "SensorReactionHistory", transaction) + 1;

            string strSQL = "Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5, DetectionStatus) ";
            strSQL += string.Format("values ({0}, {1}, {2}, '{3}', '{4}', {5}, {6}, {7}, {8}, {9}, {10})",
                nID,
                alarm.SensorZoneHistoryID,
                nReactionType,
                strTime,
                strMessage,
                strParam1,
                strParam2,
                strParam3,
                strParam4,
                strParam5,
                strStatus);

            if (transaction)
            {
                if (dbMgr.GetBatchData(strSQL) != null)
                {
                    alarm.SensorReactionHistoryID = nID;
                    alarm.ReactionHistoryParam1 = param1;
                    alarm.ReactionHistoryParam2 = param2;
                    alarm.ReactionHistoryParam3 = param3;
                    alarm.ReactionHistoryParam4 = param4;
                    alarm.ReactionHistoryParam5 = param5;
                    return true;
                }
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) != null)
                {
                    alarm.SensorReactionHistoryID = nID;
                    alarm.ReactionHistoryParam1 = param1;
                    alarm.ReactionHistoryParam2 = param2;
                    alarm.ReactionHistoryParam3 = param3;
                    alarm.ReactionHistoryParam4 = param4;
                    alarm.ReactionHistoryParam5 = param5;
                    return true;
                }
            }

            return false;
        }

        public bool AddReactionHistoryDescription(int sensorReactionHistoryID, int sensorZoneHistoryID, string strMemo, DirectDBManager dbMgr, bool transaction)
        {
            int nID = GetMaxTableID(dbMgr, "SensorReactionHistoryDescription", transaction) + 1;

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO SensorReactionHistoryDescription(ID, SensorReactionHistoryID, SensorZoneHistoryID, DescriptionID, DescriptionText) ");
            sb.AppendFormat(" VALUES({0}, {1}, {3}, -1, '{2}')", nID, sensorReactionHistoryID, strMemo, sensorZoneHistoryID);

            if (transaction)
            {
                if (dbMgr.GetBatchData(sb.ToString()) != null)
                    return true;
            }
            else
            {
                if (dbMgr.GetResultData(sb.ToString()) != null)
                    return true;
            }

            return false;
        }

        // SensorZoneData가 알람상태인지 검사한다.
        private bool IsAlarmSensorZoneData(int nSensorZoneID, string strSensorData)
        {
            if (nSensorZoneID == 0)
            {
                // nSensorZoneID가 0인 경우는 수동화재신고 이므로 SensorData를 신경쓰지 않는다.
                return true;
            }

            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

            if (sensorZone == null)
                return false;

            // 지진센서의 경우 SensorData에 알람단계가 아니라 진도 또는 규모가 기록되어 있다.
            if (BroadcastManager.IsEarthquakeSensor(sensorZone.Type))
                return true;

            if (strSensorData == null)
                return false;

            int nSensorData;

            if (int.TryParse(strSensorData, out nSensorData) == false)
                return false;

            if (BroadcastManager.IsETCSensor(sensorZone.Type))
            {
                return nSensorData > 0;
            }

            if (nSensorData == (int)UnE.Alarm.AlarmType.ALARM ||
                nSensorData == (int)UnE.Alarm.AlarmType.PSM_ALARM_1 ||
                nSensorData == (int)UnE.Alarm.AlarmType.PSM_ALARM_2 ||
                nSensorData == (int)UnE.Alarm.AlarmType.PSM_ALARM_3)
                return true;

            return false;
        }

        // 현재 Alarm이 발생중인 SensorZone에 대한 Query 조건문
        private string GetAlarmSensorZoneQueryString()
        {
            string strCondition = ((int)UnE.Alarm.AlarmType.ALARM).ToString();

            strCondition += ", " + ((int)UnE.Alarm.AlarmType.PSM_ALARM_1).ToString();
            strCondition += ", " + ((int)UnE.Alarm.AlarmType.PSM_ALARM_2).ToString();
            strCondition += ", " + ((int)UnE.Alarm.AlarmType.PSM_ALARM_3).ToString();
            /*strCondition += ", " + ((int)PSMManager.HistoryDataType.PSM_ALARM_1).ToString();
            strCondition += ", " + ((int)PSMManager.HistoryDataType.PSM_ALARM_2).ToString();
            strCondition += ", " + ((int)PSMManager.HistoryDataType.PSM_ALARM_3).ToString();*/

            return "(" + strCondition + ")";
        }

        // 현재 Alarm이 발생중인 SensorReactionLog에 대한 Query 조건문
        private string GetAlarmReactionHistoryQueryString()
        {
            string strCondition = ((int)BaseProcessManager.ReactionType.BEGIN_STATUS).ToString();
            strCondition += ", " + ((int)BaseProcessManager.ReactionType.NOTIFY_SIGNAL).ToString();

            return "(" + strCondition + ")";
        }

        //현재 Alarm이 꺼진 SensorReactionLog에 대한 Query조건문
        private string GetAlarmOffReactionHistoryQueryString()
        {
            string strCondition = ((int)BaseProcessManager.ReactionType.MALFUNCTION).ToString();
            strCondition += ", " + ((int)BaseProcessManager.ReactionType.IGNORE_SIGNAL).ToString();
            strCondition += ", " + ((int)BaseProcessManager.ReactionType.IGNORE_SOP).ToString();
            strCondition += ", " + ((int)BaseProcessManager.ReactionType.END_STATUS).ToString();
            strCondition += ", " + ((int)BaseProcessManager.ReactionType.USER_RESET).ToString();
            strCondition += ", " + ((int)BaseProcessManager.ReactionType.TIME_OUT).ToString();

            // strCondition += ", " + ((int)SensorReactionLog.ReactionType.END_S1SVMS_STATUS).ToString();

            return "(" + strCondition + ")";
        }

        // Server가 켜지기 전에 이미 발생해있던 알람들을 읽어온다.
        public void ReadSensorHistory(DirectDBManager dbMgr)
        {
            string strQueryField = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID, srh.DetectionStatus, szh.Param2, szh.Param3 ";

            // SensorZone ID가 존재하는 SensorZoneHistory(센서로부터 발생한 신호) 검색
            string szText = strQueryField + ", sz.Data ";
            szText += "FROM SensorReactionHistory as srh, SensorZoneHistory as szh, SensorZone as sz, EquipmentZone as ez ";
            szText += "WHERE srh.ID in (";
            szText += "Select max(ID) from SensorReactionHistory where SensorHistoryID in (SELECT SensorHistoryID FROM SensorReactionHistory WHERE ReactionType in " + GetAlarmReactionHistoryQueryString() + ") and ";
            szText += "SensorHistoryID not in (SELECT SensorHistoryID FROM SensorReactionHistory WHERE ReactionType in " + GetAlarmOffReactionHistoryQueryString() + ") group by SensorHistoryID, ReactionType)";
            /*szText += "WHERE SensorHistoryID in (";
            szText += "         SELECT srh2.SensorHistoryID ";
            szText += "         FROM SensorReactionHistory as srh2, SensorZoneHistory as szh2 ";
            szText += "         WHERE szh2.Id = srh2.SensorHistoryID and srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + ") ";
            szText += "     AND SensorHistoryID not in (";
            szText += "         SELECT srh3.SensorHistoryID ";
            szText += "         FROM SensorReactionHistory as srh3, SensorZoneHistory as szh3 ";
            szText += "         WHERE szh3.Id = srh3.SensorHistoryID and srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + ") ";*/
            szText += "     AND srh.SensorHistoryID = szh.ID ";
            szText += "     AND szh.SensorID = sz.ID ";
            szText += "     AND sz.EquipZoneID = ez.ID ";
            szText += "     AND ez.SiteID = {0} ";
            szText += "     ORDER BY srh.Time, szh.SensorID";

            // SensorZone ID가 0인 SensorZoneHistory(수동화재신고) 검색
            // 수동화재신고 관련 Query에는 SensorZone Table을 사용하지 않기 때문에
            // SensorZone.Data를 Select 할수 없기 때문에 강제로 값을 1로 넣어준다.
            /*string szText2 = strQueryField + ", 1 ";
            szText2 += "FROM SensorReactionHistory as srh, SensorZoneHistory as szh ";
            szText2 += "WHERE SensorHistoryID in (";
            szText2 += "         SELECT srh2.SensorHistoryID ";
            szText2 += "         FROM SensorReactionHistory as srh2, SensorZoneHistory as szh2 ";
            szText2 += "         WHERE szh2.Id = srh2.SensorHistoryID and srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + ") ";
            szText2 += "     AND SensorHistoryID not in (";
            szText2 += "         SELECT srh3.SensorHistoryID ";
            szText2 += "         FROM SensorReactionHistory as srh3, SensorZoneHistory as szh3 ";
            szText2 += "         WHERE szh3.Id = srh3.SensorHistoryID and srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + ") ";
            szText2 += "     AND srh.SensorHistoryID = szh.ID ";
            szText2 += "     AND szh.SensorID = 0 ";
            szText2 += "     AND szh.SiteID = {0} ";
            //szText2 += "     AND ( srh.Time between '{1}' and '{2}') ";
            szText2 += "     ORDER BY srh.Time, szh.SensorID";*/

            DateTime dtNow = DateTime.Now;
            string szNowTime = WebDBManager.MakeDateTimeString(DateTime.Now);
            DateTime dtPrev = dtNow.AddDays(-1.0);
            string szPrevTime = WebDBManager.MakeDateTimeString(dtPrev);
            // SensorZone ID가 존재하는 SensorZoneHistory(센서로부터 발생한 신호) 검색
            string strSQL = string.Format(szText, dbMgr.SiteID, szPrevTime, szNowTime);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null)
                return;

            // SensorZone ID가 0인 SensorZoneHistory(수동화재신고) 검색
            //string strSQL2 = string.Format(szText2, dbMgr.SiteID, szPrevTime, szNowTime);

            //ArrayList arrResult2 = dbMgr.GetResultData(strSQL2);
            //if (arrResult2 == null)
            //    return;

            // 두 Query 결과를 하나로 통합
            //arrResult.AddRange(arrResult2);

            int nResultCount = arrResult.Count;

            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nResultCount - 14; i += 15)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strParam1 = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strParam2 = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strParam3 = WebDBManager.GetStringField(arrResult[i + 7], "");
                string strParam4 = WebDBManager.GetStringField(arrResult[i + 8], "");
                string strParam5 = WebDBManager.GetStringField(arrResult[i + 9], "");
                int nSensorID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                int nStatus = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 3);
                string strSensorType = WebDBManager.GetStringField(arrResult[i + 12], "");
                string strSensorZoneIDs = WebDBManager.GetStringField(arrResult[i + 13]);
                string strSensorData = WebDBManager.GetStringField(arrResult[i + 14]);

                if (nID < 0 || nHistoryID < 0)
                    continue;

                // 하루가 경과된 알람들은 DB에서 종료처리한다.
                if (time < dtPrev)
                {
                    TimeoutAlarm(dbMgr, nHistoryID, nStatus, szNowTime);
                    continue;
                }

                IFacility.FacilityType sensorType = IFacility.FacilityType.NONE;

                if (strSensorType.Length > 0)
                {
                    int nSensorType;

                    if (int.TryParse(strSensorType, out nSensorType))
                    {
                        sensorType = IFacility.ToFacilityType(nSensorType);
                    }
                }

                if (strSensorZoneIDs == null || strSensorZoneIDs.Length == 0)
                {
                    if (IsAlarmSensorZoneData(nSensorID, strSensorData))
                        CheckAlarmSensorZone(nSensorID, sensorType, nReactionType, nStatus, nHistoryID, nID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, dbMgr);
                }
                else
                {
                    // SensorZoneHistory의 Param3에는 현재 발생한 알람과 연관된 센서중 작동한 SensorZone ID들이 담겨있다.
                    // [2019/10/31] 김지웅
                    Dictionary<int, int> dicSensorZoneDatas = GetSensorZoneDatas(strSensorZoneIDs, dbMgr);

                    int nSensorZoneID, nSensorZoneData;
                    string[] ids = strSensorZoneIDs.Split(',');

                    foreach (string strID in ids)
                    {
                        if (int.TryParse(strID.Trim(), out nSensorZoneID))
                        {
                            if (SOPWebServer.Header.ManualReportDefaultID <= nSensorZoneID)
                            {
                                // 수동신고일 경우
                                CheckAlarmSensorZone(nSensorZoneID, sensorType, nReactionType, nStatus, nHistoryID, nID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, dbMgr);
                            }
                            else if (dicSensorZoneDatas.TryGetValue(nSensorZoneID, out nSensorZoneData))
                            {
                                // 센서신호일 경우
                                if (IsAlarmSensorZoneData(nSensorID, nSensorZoneData.ToString()))
                                    CheckAlarmSensorZone(nSensorZoneID, sensorType, nReactionType, nStatus, nHistoryID, nID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, dbMgr);
                            }
                        }
                    }
                }

                /*if (sensorType == IFacility.FacilityType.NONE)
                {
                    SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorID);

                    if (sensorZone != null)
                        sensorType = sensorZone.Type;
                }

                // SensorType을 알수 없으면 재난 타입을 알수 없다.
                if (sensorType == IFacility.FacilityType.NONE)
                    continue;

                BaseProcessManager.ReactionType type = BaseProcessManager.ToReactionType(nReactionType);
                BaseProcessManager.DetectionStatus status = BaseProcessManager.ToDetectionStatus(nStatus);

                if (type == BaseProcessManager.ReactionType.ETC || status == BaseProcessManager.DetectionStatus.Unknown)
                    continue;

                if (type == BaseProcessManager.ReactionType.BEGIN_STATUS || type == BaseProcessManager.ReactionType.NOTIFY_SIGNAL || type == BaseProcessManager.ReactionType.CHANGE_ALARM_DEPTH)
                {
                    if (BaseBroadcastManager.IsFireSensor(sensorType))
                        AddFireAlarm(dbMgr, type, nHistoryID, nID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, nSensorID, status);
                    else if (BaseBroadcastManager.IsPSMSensor(sensorType))
                        AddPSMAlarm(dbMgr, type, nHistoryID, nID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, nSensorID, status);
                    else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
                        AddSecurityAlarm(dbMgr, type, nHistoryID, nID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, nSensorID, status);
                }*/
            }

            // 서버가 켜지기 이전에 발생한 알람들에 대한 SOP 실행상태를 검사한다.
            CheckAlarmSOPState(dbMgr);
        }

        // 서버가 켜지기 이전에 발생한 알람들에 대한 SOP 실행상태를 검사한다.
        private void CheckAlarmSOPState(DirectDBManager dbMgr)
        {
            string strSensorZoneHistoryIDs = "";
            List<AlarmData> alarms = CurrentAlarms;

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

            string strSQL = "Select ID, SensorZoneHistoryID from ActionStepHistory where SensorZoneHistoryID in (" + strSensorZoneHistoryIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> actionStepHistoryID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (actionStepHistoryID == null || sensorZoneHistoryID == null)
                    continue;

                AlarmData alarm = GetAlarm(sensorZoneHistoryID.Data);

                if (alarm != null)
                {
                    alarm.SOPProcess = AlarmData.SOPProcessType.Run;
                }
            }
        }

        private Dictionary<int, int> GetSensorZoneDatas(string strSensorZoneIDs, DirectDBManager dbMgr)
        {
            Dictionary<int, int> dicSensorZoneDatas = new Dictionary<int, int>();

            string strSQL = "Select ID, Data from SensorZone where ID in (" + strSensorZoneIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return dicSensorZoneDatas;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorData = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (sensorZoneID != null && sensorData != null)
                    dicSensorZoneDatas[sensorZoneID.Data] = sensorData.Data;
            }

            return dicSensorZoneDatas;
        }

        private void CheckAlarmSensorZone(int nSensorZoneID, IFacility.FacilityType sensorType, int nReactionType, int nStatus, int nSensorZoneHistoryID, int nSensorReactionHistoryID, DateTime time, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5,  DirectDBManager dbMgr)
        {
            if (sensorType == IFacility.FacilityType.NONE)
            {
                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone != null)
                    sensorType = sensorZone.Type;
            }

            // SensorType을 알수 없으면 재난 타입을 알수 없다.
            if (sensorType == IFacility.FacilityType.NONE)
                return;

            BaseProcessManager.ReactionType type = BaseProcessManager.ToReactionType(nReactionType);
            BaseProcessManager.DetectionStatus status = BaseProcessManager.ToDetectionStatus(nStatus);

            if (type == BaseProcessManager.ReactionType.ETC || status == BaseProcessManager.DetectionStatus.Unknown)
                return;

            if (type == BaseProcessManager.ReactionType.BEGIN_STATUS || type == BaseProcessManager.ReactionType.NOTIFY_SIGNAL || type == BaseProcessManager.ReactionType.CHANGE_ALARM_DEPTH)
            {
                if (BaseBroadcastManager.IsPSMSensor(sensorType))
                    AddPSMAlarm(dbMgr, type, nSensorZoneHistoryID, nSensorReactionHistoryID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, nSensorZoneID, status);
                else
                    AddSensorAlarm(dbMgr, type, nSensorZoneHistoryID, nSensorReactionHistoryID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, nSensorZoneID, status);
                /*if (BaseBroadcastManager.IsFireSensor(sensorType))
                    AddFireAlarm(dbMgr, type, nSensorZoneHistoryID, nSensorReactionHistoryID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, nSensorZoneID, status);
                else if (BaseBroadcastManager.IsPSMSensor(sensorType))
                    AddPSMAlarm(dbMgr, type, nSensorZoneHistoryID, nSensorReactionHistoryID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, nSensorZoneID, status);
                else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
                    AddSecurityAlarm(dbMgr, type, nSensorZoneHistoryID, nSensorReactionHistoryID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, nSensorZoneID, status);*/
            }
        }

        private void TimeoutAlarm(DirectDBManager dbMgr, int nSensorZoneHistoryID, int nDetectionStatus, string strTime)
        {
            string strSQL = "Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5, DetectionStatus) ";
            strSQL += string.Format("Select ISNULL(max(ID) + 1, 1), {0}, {1}, '{2}', '알람발생후 만 하루가 경과하여 알람을 초기화합니다.', NULL, NULL, NULL, NULL, NULL, {3} from SensorReactionHistory",
                nSensorZoneHistoryID,
                (int)BaseProcessManager.ReactionType.TIME_OUT,
                strTime,
                nDetectionStatus);

            dbMgr.GetResultData(strSQL);
        }

        private AlarmData AddAlarm(DirectDBManager dbMgr, BaseProcessManager.ReactionType type, int nSensorZoneHistoryID, int nSensorReactionHistoryID, DateTime timeStamp, string strMessage, int nSensorZoneID, BaseProcessManager.DetectionStatus status)
        {
            if (status == BaseProcessManager.DetectionStatus.MALFUNCTION)
                return null;

            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

            if (sensorZone == null)
                return null;

            SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

            if (group == null)
                return null;

            AlarmData alarm = null;

            if (group.GetSensorDatas().Count > 0 && group.CurrentAlarm != null)
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
                alarm.SensorType = sensorZone.Type;
                alarm.IsReal = status == BaseProcessManager.DetectionStatus.REAL;

                // 수동신고
                if (nSensorZoneID >= 1000000)
                    alarm.IsManual = true;

                string strSQL = string.Format("Select CompanyMemberIDList, ExternalCompanyMemberIDList from SDMSSMSHistory where SensorHistoryID = {0} and ReactionHistoryID = {1}",
                nSensorZoneHistoryID, nSensorReactionHistoryID);

                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult != null && arrResult.Count == 2)
                {
                    string strRegularMemberIDs = WebDBManager.GetStringField(arrResult[0]);
                    string strExternalMemberIDs = WebDBManager.GetStringField(arrResult[1]);

                    if (strRegularMemberIDs != null)
                    {
                        SetMemberIDs(alarm.RegularMemberIDs, strRegularMemberIDs);
                        SetRegularPhoneNumbers(alarm);
                    }

                    if (strExternalMemberIDs != null)
                    {
                        SetMemberIDs(alarm.ExternalMemberIDs, strExternalMemberIDs);
                        SetExternalPhoneNumbers(alarm);
                    }
                }

                m_dicSensorZoneHistoryIDAlarms[nSensorZoneHistoryID] = alarm;
            }

            int nSensorZoneData = GetSensorZoneData(dbMgr, nSensorZoneID, false);
            group.SetSensorData(sensorZone, nSensorZoneData, null, false);
            group.CurrentAlarm = alarm;

            if (alarm != null)
                alarm.AlarmDepth = nSensorZoneData;

            return alarm;
        }

        private int GetSensorZoneData(DirectDBManager dbMgr, int nSensorZoneID, bool transaction)
        {
            string strSQL = "Select Data from SensorZone where ID = " + nSensorZoneID.ToString();
            ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nData = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nData;
        }

        private void AddSensorAlarm(DirectDBManager dbMgr, BaseProcessManager.ReactionType type, int nSensorZoneHistoryID, int nSensorReactionHistoryID, DateTime timeStamp, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, int nSensorZoneID, BaseProcessManager.DetectionStatus status)
        {
            AlarmData alarm = AddAlarm(dbMgr, type, nSensorZoneHistoryID, nSensorReactionHistoryID, timeStamp, strMessage, nSensorZoneID, status);

            if (alarm != null)
            {
                alarm.ReactionHistoryParam1 = strParam1;
                alarm.ReactionHistoryParam2 = strParam2;
                alarm.ReactionHistoryParam3 = strParam3;
                alarm.ReactionHistoryParam4 = strParam4;
                alarm.ReactionHistoryParam5 = strParam5;
                
                if (alarm.IsManual && strParam5 != null)
                {
                    int nAlarmDepth;

                    if (int.TryParse(strParam5, out nAlarmDepth))
                    {
                        alarm.AlarmDepth = nAlarmDepth;
                    }
                }
            }
        }

        /*private void AddFireAlarm(DirectDBManager dbMgr, BaseProcessManager.ReactionType type, int nSensorZoneHistoryID, int nSensorReactionHistoryID, DateTime timeStamp, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, int nSensorZoneID, BaseProcessManager.DetectionStatus status)
        {
            AlarmData alarm = AddAlarm(dbMgr, type, nSensorZoneHistoryID, nSensorReactionHistoryID, timeStamp, strMessage, nSensorZoneID, status);

            if (alarm != null)
            {
                alarm.ReactionHistoryParam1 = strParam1;
                alarm.ReactionHistoryParam2 = strParam2;
                alarm.ReactionHistoryParam3 = strParam3;
                alarm.ReactionHistoryParam4 = strParam4;
                alarm.ReactionHistoryParam5 = strParam5;
            }
        }*/

        private void AddPSMAlarm(DirectDBManager dbMgr, BaseProcessManager.ReactionType type, int nSensorZoneHistoryID, int nSensorReactionHistoryID, DateTime timeStamp, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, int nSensorZoneID, BaseProcessManager.DetectionStatus status)
        {
            //int nAlarmDepth = 1;
            AlarmData alarm = null;

            //if (strParam5 != null)
            //    int.TryParse(strParam5, out nAlarmDepth);

            if (type == BaseProcessManager.ReactionType.BEGIN_STATUS || type == BaseProcessManager.ReactionType.NOTIFY_SIGNAL)
            {
                alarm = AddAlarm(dbMgr, type, nSensorZoneHistoryID, nSensorReactionHistoryID, timeStamp, strMessage, nSensorZoneID, status);

                if (alarm != null)
                    alarm.AlarmDepth = alarm.AlarmDepth - (int)UnE.Alarm.AlarmType.PSM_ALARM_1 + 1;
            }
            else if (type == BaseProcessManager.ReactionType.CHANGE_ALARM_DEPTH)
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
            //alarm.AlarmDepth = nAlarmDepth;
            alarm.ReactionHistoryParam1 = strParam1;
            alarm.ReactionHistoryParam2 = strParam2;
            alarm.ReactionHistoryParam3 = strParam3;
            alarm.ReactionHistoryParam4 = strParam4;
            alarm.ReactionHistoryParam5 = strParam5;

            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

            if (sensorZone != null)
            {
                SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

                if (group != null)
                    group.SetSensorData(sensorZone, alarm.AlarmDepth + (int)UnE.Alarm.AlarmType.PSM_ALARM_1 - 1, null, false);
            }
        }

        /*private void AddSecurityAlarm(DirectDBManager dbMgr, BaseProcessManager.ReactionType type, int nSensorZoneHistoryID, int nSensorReactionHistoryID, DateTime timeStamp, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, int nSensorZoneID, BaseProcessManager.DetectionStatus status)
        {
            AlarmData alarm = AddAlarm(dbMgr, type, nSensorZoneHistoryID, nSensorReactionHistoryID, timeStamp, strMessage, nSensorZoneID, status);

            if (alarm != null)
            {
                alarm.ReactionHistoryParam1 = strParam1;
                alarm.ReactionHistoryParam2 = strParam2;
                alarm.ReactionHistoryParam3 = strParam3;
                alarm.ReactionHistoryParam4 = strParam4;
                alarm.ReactionHistoryParam5 = strParam5;
            }
        }*/

        private void SetRegularPhoneNumbers(AlarmData alarm)
        {
            foreach (KeyValuePair<int, int> pair in alarm.RegularMemberIDs)
            {
                DataCompanyMember member = MemberManager.Instance.GetRegularMember(pair.Value);

                if (member != null && member.PhoneNumber.Length > 0)
                    alarm.PhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
            }
        }

        private void SetExternalPhoneNumbers(AlarmData alarm)
        {
            foreach (KeyValuePair<int, int> pair in alarm.ExternalMemberIDs)
            {
                DataExternalMember member = MemberManager.Instance.GetExternalMember(pair.Value);

                if (member != null && member.PhoneNumber.Length > 0)
                    alarm.PhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
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

        /*public void AddAlarm(AlarmData alarm)
        {
            SensorZone sensorZone = IOManager.Instance.GetSensorZone(alarm.SensorZoneID);

            if (sensorZone != null)
            {
                sensorZone.Alarm = alarm;
                m_dicSensorZoneHistoryIDAlarms[alarm.SensorZoneHistoryID] = alarm;
            }
        }

        public void RemoveAlarm(AlarmData alarm)
        {
            SensorZone sensorZone = IOManager.Instance.GetSensorZone(alarm.SensorZoneID);

            if (sensorZone != null)
            {
                AlarmData _alarm;

                if (m_dicSensorZoneHistoryIDAlarms.TryRemove(alarm.SensorZoneHistoryID, out _alarm))
                {
                    sensorZone.Alarm = null;
                }
            }
        }

        public AlarmData FindAlarm(int nSensorZoneHistoryID)
        {
            AlarmData alarm;

            if (m_dicSensorZoneHistoryIDAlarms.TryGetValue(nSensorZoneHistoryID, out alarm))
                return alarm;

            return null;
        }

        public bool CheckAlarmSensor(int nSensorZoneID)
        {
            SensorZone sensorZone = IOManager.Instance.GetSensorZone(nSensorZoneID);

            if (sensorZone == null)
                return false;

            if (sensorZone.Alarm != null)
                return true;

            // 같은 SensorZone Gropu에 속해있는 다른 Sensor들에 관한 알람이 존재하는지 확인한다.
            SensorZoneGroup group = IOManager.Instance.GetSensorZoneGroup(nSensorZoneID);

            if (group != null)
            {
                foreach (KeyValuePair<SensorZone, object> pair in group.SensorDatas)
                {
                    if (pair.Key.ID == nSensorZoneID)
                        continue;

                    if (pair.Key.Alarm != null)
                        return true;
                }
            }

            return false;
        }*/
    }
}