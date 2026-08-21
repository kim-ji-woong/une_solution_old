using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnE.Spatial;
using DBUtility2;
using System.Collections;
using System.Collections.Concurrent;

namespace AutoUpdater.Data
{
    public static class AlarmManager
    {
        public const string FireOn = "3";
        public const string FireOff = "0";

        private static bool m_closeSystem = false;
        private static int m_nLastReadID =  0;

        // Key : dvcCd
        private static ConcurrentDictionary<string, FireAlarm> m_dicCurrentAlarms = new ConcurrentDictionary<string, FireAlarm>();

        public static void RunAlarmMonitoring(WebDBManagerEx dbMgr)
        {
            Thread t = new Thread(new ParameterizedThreadStart(DoAlarmMonitoring));
            t.Start(dbMgr);
        }

        private static void DoAlarmMonitoring(object arg)
        {
            WebDBManagerEx dbMgr = (WebDBManagerEx)arg;

            // AutoUpdater가 실행되기 전에 발생했던 알람들을 읽어온다.
            ReadPrevAlarms(dbMgr);

            while (m_closeSystem == false)
            {
                ReadClearAlarm(dbMgr);
                ReadNewAlarm(dbMgr);

                Thread.Sleep(1000);
            }
        }

        // 현재 Alarm이 발생중인 SensorReactionLog에 대한 Query 조건문
        private static string GetAlarmReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.BEGIN_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.NOTIFY_SIGNAL).ToString();

            return "(" + strCondition + ")";
        }

        //현재 Alarm이 꺼진 SensorReactionLog에 대한 Query조건문
        private static string GetAlarmOffReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.MALFUNCTION).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SIGNAL).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SOP).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.END_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.USER_RESET).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.TIME_OUT).ToString();

            return "(" + strCondition + ")";
        }

        // AutoUpdater가 실행되기 전에 발생했던 알람들을 읽어온다.
        private static void ReadPrevAlarms(WebDBManagerEx dbMgr)
        {
            string strSQL = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, szh.SensorID FROM SensorReactionHistory as srh ";
            strSQL += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            strSQL += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + " ) ";
            strSQL += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + " ) ";
            strSQL += " AND szh.SiteID = " + dbMgr.SiteID.ToString();
            strSQL += " ORDER BY srh.Time, szh.SensorID";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, dbMgr.LocalDBName);

            if (arrResult == null)
                return;

            string strSensorZoneHistoryIDs = "";
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                
                if (nID < 0 || nHistoryID < 0)
                    continue;

                int nSensorID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                int nSensorType = -1;
                if (DataManager.Instance.GetSensorZoneType(nSensorID, out nSensorType) == false)
                    continue;

                // 화재센서만 취급한다.
                if (nReactionType == (int)libSensorProcess.ReactionType.BEGIN_STATUS && nSensorType == (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
                {
                    if (DataManager.Instance.BaseBuildingGroupID < 0)
                    {
                        // 일반 건물
                        if (strSensorZoneHistoryIDs.Length == 0)
                            strSensorZoneHistoryIDs = "'" + FireAlarm.MakeSensorZoneHistoryIDString(nHistoryID) + "'";
                        else
                            strSensorZoneHistoryIDs += ", '" + FireAlarm.MakeSensorZoneHistoryIDString(nHistoryID) + "'";
                    }
                    else
                    {
                        // 지자체
                        if (strSensorZoneHistoryIDs.Length == 0)
                            strSensorZoneHistoryIDs = nHistoryID.ToString();
                        else
                            strSensorZoneHistoryIDs += ", " + nHistoryID.ToString();
                    }
                }
            }

            if (strSensorZoneHistoryIDs.Length == 0)
                return;

            if (DataManager.Instance.BaseBuildingGroupID < 0)
            {
                // 일반 건물
                strSQL = "Select ID, RecvTime, dvcCd, evtId, evtType, mapCd, floorId, SensorZoneHistoryID from WebFireAlarmHistory where SensorZoneHistoryID in (" + strSensorZoneHistoryIDs + ")";
                arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return;

                // 같은 SensorZoneHistoryID를 가진 알람이 여러번 생성되는 것을 막는다.
                Dictionary<string, FireAlarm> dicSensorZoneHistoryAlarms = new Dictionary<string, FireAlarm>();

                nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 7; i += 8)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                    string strDvcCd = WebDBManager.GetStringField(arrResult[i + 2]);
                    string strEventID = WebDBManager.GetStringField(arrResult[i + 3]);
                    string strEventType = WebDBManager.GetStringField(arrResult[i + 4]);
                    string strMapCode = WebDBManager.GetStringField(arrResult[i + 5]);
                    string strFloorID = WebDBManager.GetStringField(arrResult[i + 6]);
                    string strSensorZoneHistoryID = WebDBManager.GetStringField(arrResult[i + 7]);

                    if (id == null || recvTime == null || strDvcCd == null || strEventID == null ||
                        strEventType == null || strMapCode == null || strFloorID == null || strSensorZoneHistoryID == null)
                        continue;

                    if (m_nLastReadID < id.Data)
                        m_nLastReadID = id.Data;

                    if (DataManager.Instance.SiteMapCodeList.Contains("'" + strMapCode + "'") == false)
                        continue;

                    FireAlarm alarm = MakeAlarm(id.Data, strDvcCd, FireOn, strEventID, strEventType, strMapCode, strFloorID, false);

                    if (alarm != null)
                    {
                        alarm.TimeStamp = recvTime.Data;
                        alarm.SetSensorZoneHistoryID(strSensorZoneHistoryID);
                        dicSensorZoneHistoryAlarms[strSensorZoneHistoryID] = alarm;
                        //m_dicCurrentAlarms[alarm.EquipCode] = alarm;
                    }
                }

                foreach (KeyValuePair<string, FireAlarm> pair in dicSensorZoneHistoryAlarms)
                {
                    m_dicCurrentAlarms[pair.Value.EquipCode] = pair.Value;
                    // SensorZoneHistoryID가 있기 때문에 굳이 SOPWebServer에게 알릴 필요가 없다.
                    //Network.NetworkWebManager.Instance.AddAlarm(alarm);
                }
            }
            else
            {
                // 지자체
                strSQL = "Select WebFireAlarmHistoryID, SensorZoneHistoryID from WebFireAlarmSensorZoneHistory where SensorZoneHistoryID in (" + strSensorZoneHistoryIDs + ")";
                arrResult = dbMgr.GetResultData(strSQL, dbMgr.LocalDBName);

                if (arrResult == null)
                    return;

                string strWebHistoryIDs = "";
                Dictionary<int, int> dicWebSensorZoneHistoryID = new Dictionary<int, int>();
                nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                    if (id == null || sensorZoneHistoryID == null)
                        continue;

                    dicWebSensorZoneHistoryID[id.Data] = sensorZoneHistoryID.Data;

                    if (strWebHistoryIDs.Length == 0)
                        strWebHistoryIDs = id.Data.ToString();
                    else
                        strWebHistoryIDs += ", " + id.Data.ToString();
                }

                if (strWebHistoryIDs.Length == 0)
                    return;

                strSQL = "Select ID, RecvTime, dvcCd, evtId, evtType, mapCd, floorId from WebFireAlarmHistory where ID in (" + strWebHistoryIDs + ")";
                arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return;

                nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 6; i += 7)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                    string strDvcCd = WebDBManager.GetStringField(arrResult[i + 2]);
                    string strEventID = WebDBManager.GetStringField(arrResult[i + 3]);
                    string strEventType = WebDBManager.GetStringField(arrResult[i + 4]);
                    string strMapCode = WebDBManager.GetStringField(arrResult[i + 5]);
                    string strFloorID = WebDBManager.GetStringField(arrResult[i + 6]);

                    if (id == null || recvTime == null || strDvcCd == null || strEventID == null ||
                        strEventType == null || strMapCode == null || strFloorID == null)
                        continue;

                    if (m_nLastReadID < id.Data)
                        m_nLastReadID = id.Data;

                    if (DataManager.Instance.SiteMapCodeList.Contains("'" + strMapCode + "'") == false)
                        continue;

                    FireAlarm alarm = MakeAlarm(id.Data, strDvcCd, FireOn, strEventID, strEventType, strMapCode, strFloorID, false);

                    if (alarm != null)
                    {
                        int nSensorZoneHistoryID;

                        if (dicWebSensorZoneHistoryID.TryGetValue(id.Data, out nSensorZoneHistoryID))
                            alarm.SensorZoneHistoryID = nSensorZoneHistoryID;
                        
                        alarm.TimeStamp = recvTime.Data;
                        m_dicCurrentAlarms[alarm.EquipCode] = alarm;

                        // SensorZoneHistoryID가 있기 때문에 굳이 SOPWebServer에게 알릴 필요가 없다.
                        //Network.NetworkWebManager.Instance.AddAlarm(alarm);
                    }
                }
            }
        }

        private static void ReadNewAlarm(WebDBManagerEx dbMgr)
        {
            int nLastEventID, nEventID;
            Dictionary<string, int> dicEquipCodeEventIDs = GetClearAlarmEquipCodes(dbMgr, out nLastEventID);

            string strSQL = "";

            if (DataManager.Instance.BaseBuildingGroupID < 0)
            {
                strSQL = "Select ID, RecvTime, dvcCd, evtId, evtType, mapCd, floorId from WebFireAlarmHistory ";
                strSQL += "where ID > " + m_nLastReadID.ToString() + " and dvcStatus = '" + FireOn + "' and SensorZoneHistoryID is NULL";
            }
            else
            {
                // 이미 처리된 알람이면 무시한다.
                string strIgnoreIDs = GetIgnoreWebFireAlarmHistoryIDs(dbMgr);

                strSQL = "Select ID, RecvTime, dvcCd, evtId, evtType, mapCd, floorId from WebFireAlarmHistory ";
                strSQL += "where ID > " + m_nLastReadID.ToString() + " and dvcStatus = '" + FireOn + "'";

                if (strIgnoreIDs.Length > 0)
                {
                    strSQL += " and ID not in (" + strIgnoreIDs + ")";
                }
            }

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-6;i+=7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                string strDvcCd = WebDBManager.GetStringField(arrResult[i + 2]);
                string strEventID = WebDBManager.GetStringField(arrResult[i + 3]);
                string strEventType = WebDBManager.GetStringField(arrResult[i + 4]);
                string strMapCode = WebDBManager.GetStringField(arrResult[i + 5]);
                string strFloorID = WebDBManager.GetStringField(arrResult[i + 6]);

                if (id == null || recvTime == null || strDvcCd == null || strEventID == null ||
                    strEventType == null || strMapCode == null || strFloorID == null)
                    continue;

                if (m_nLastReadID < id.Data)
                    m_nLastReadID = id.Data;

                if (DataManager.Instance.SiteMapCodeList.Contains("'" + strMapCode + "'") == false)
                    continue;

                if (dicEquipCodeEventIDs.TryGetValue(strDvcCd, out nEventID))
                {
                    if (nEventID > id.Data)
                    {
                        // 이미 처리된 알람이다.
                        continue;
                    }
                }

                FireAlarm alarm = MakeAlarm(id.Data, strDvcCd, FireOn, strEventID, strEventType, strMapCode, strFloorID, true);

                if (alarm != null)
                {
                    alarm.TimeStamp = recvTime.Data;
                    m_dicCurrentAlarms[alarm.EquipCode] = alarm;
                    Network.NetworkWebManager.Instance.AddAlarm(alarm);
                }
            }

            if (m_nLastReadID < nLastEventID)
                m_nLastReadID = nLastEventID;
        }

        private static string GetIgnoreWebFireAlarmHistoryIDs(WebDBManagerEx dbMgr)
        {
            string strSQL = "Select WebFireAlarmHistoryID from WebFireAlarmSensorZoneHistory where WebFireAlarmHistoryID > " + m_nLastReadID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, dbMgr.LocalDBName);

            if (arrResult == null)
                return "";

            string strIgnoreIDs = "";
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                if (strIgnoreIDs.Length == 0)
                    strIgnoreIDs = id.Data.ToString();
                else
                    strIgnoreIDs += ", " + id.Data.ToString();
            }

            return strIgnoreIDs;
        }

        // Key : dvcCd
        // Value : EventID
        private static Dictionary<string, int> GetClearAlarmEquipCodes(WebDBManagerEx dbMgr, out int nLastEventID)
        {
            nLastEventID = m_nLastReadID;

            string strSQL = "Select ID, dvcCd from WebFireAlarmHistory ";
            strSQL += "where ID > " + m_nLastReadID.ToString() + " and dvcStatus = '" + FireOff + "'";

            // Key : dvcCd
            // Value : EventID
            Dictionary<string, int> dicEquipCodeIDs = new Dictionary<string, int>();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return dicEquipCodeIDs;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strDvcCd = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strDvcCd == null)
                    continue;

                dicEquipCodeIDs[strDvcCd] = id.Data;

                if (nLastEventID < id.Data)
                    nLastEventID = id.Data;
            }

            return dicEquipCodeIDs;
        }

        private static void ReadClearAlarm(WebDBManagerEx dbMgr)
        {
            List<FireAlarm> alarms = m_dicCurrentAlarms.Values.ToList();
            string strDvcCodes = "";

            int nMinID = -1;

            foreach (FireAlarm alarm in alarms)
            {
                if (nMinID < 0)
                    nMinID = alarm.WebHistoryID;
                else if (nMinID > alarm.WebHistoryID)
                    nMinID = alarm.WebHistoryID;

                if (strDvcCodes.Length == 0)
                    strDvcCodes = "'" + alarm.EquipCode + "'";
                else
                    strDvcCodes += ", '" + alarm.EquipCode + "'";
            }

            if (strDvcCodes.Length == 0)
                return;

            string strSQL = "Select ID, RecvTime, dvcCd, evtId, evtType, mapCd, floorId from WebFireAlarmHistory ";
            strSQL += string.Format("where dvcStatus = '{0}' and dvcCd in ({1}) and ID > {2}", FireOff, strDvcCodes, nMinID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                string strDvcCd = WebDBManager.GetStringField(arrResult[i + 2]);
                string strEventID = WebDBManager.GetStringField(arrResult[i + 3]);
                string strEventType = WebDBManager.GetStringField(arrResult[i + 4]);
                string strMapCode = WebDBManager.GetStringField(arrResult[i + 5]);
                string strFloorID = WebDBManager.GetStringField(arrResult[i + 6]);

                if (id == null || recvTime == null || strDvcCd == null || strEventID == null ||
                    strEventType == null || strMapCode == null || strFloorID == null)
                    continue;

                FireAlarm temp;

                if (m_dicCurrentAlarms.TryGetValue(strDvcCd, out temp) == false)
                    continue;

                if (temp.WebHistoryID > id.Data)
                {
                    // 알람발생 이전의 기록은 무시한다.
                    continue;
                }

                FireAlarm alarm = MakeAlarm(id.Data, strDvcCd, FireOff, strEventID, strEventType, strMapCode, strFloorID, true);

                if (alarm != null)
                {
                    alarm.TimeStamp = recvTime.Data;
                    m_dicCurrentAlarms.TryRemove(alarm.EquipCode, out temp);
                    Network.NetworkWebManager.Instance.AddAlarm(alarm);
                }
            }
        }

        public static void Close()
        {
            m_closeSystem = true;
        }

        public static FireAlarm MakeAlarm(int nWebHistoryID, string dvcCd, string dvcStatus, string evtId, string evtType, string mapCd, string floorId, bool checkValidation = false)
        {
            Building building = DataManager.Instance.GetBuilding(mapCd);

            if (building == null)
            {
                /*if (result != null)
                    result.ErrorMessage = string.Format("{0}는 알수 없는 Map Code입니다.", mapCd);*/
                return null;
            }

            Zone zone = DataManager.Instance.GetZone(building, floorId);

            if (zone == null)
            {
                /*if (result != null)
                    result.ErrorMessage = string.Format("{0}로부터 층정보를 알아낼수 없습니다.", floorId);*/
                return null;
            }

            FireAlarm alarm = null;

            if (dvcStatus == FireOn)
            {
                if (checkValidation)
                {
                    alarm = Network.NetworkWebManager.Instance.GetFireAlarm(dvcCd, evtId);

                    if (alarm != null)
                    {
                        /*if (result != null)
                            result.ErrorMessage = string.Format("{0}는 현재 진행중인 알람에 대한 이벤트 ID입니다.", evtId);*/
                        return null;
                    }
                }

                int nSensorTagID, nSensorZoneID;

                if (Network.NetworkWebManager.Instance.GetSensorInfo(zone, out nSensorTagID, out nSensorZoneID) == false)
                {
                    /*if (result != null)
                        result.ErrorMessage = string.Format("DB로부터 [{0}]에 대한 센서정보를 읽어오지 못하였습니다.", zone.Name);*/
                    return null;
                }

                if (checkValidation)
                {
                    alarm = Network.NetworkWebManager.Instance.GetFireAlarm(nSensorZoneID);

                    if (alarm != null)
                    {
                        /*if (result != null)
                            result.ErrorMessage = string.Format("[{0}]에 대한 알람이 이미 발생되어 있습니다.", zone.Name);*/
                        return null;
                    }
                }

                alarm = new FireAlarm();

                alarm.SensorTagID = nSensorTagID;
                alarm.SensorZoneID = nSensorZoneID;
                alarm.IsAlarmOn = true;
            }
            else if (dvcStatus == FireOff)
            {
                if (checkValidation)
                {
                    if (m_dicCurrentAlarms.TryGetValue(dvcCd, out alarm))
                    {
                        alarm.IsAlarmOn = false;
                        return alarm;
                    }

                    alarm = Network.NetworkWebManager.Instance.GetFireAlarm(zone);

                    if (alarm == null)
                    {
                        /*if (result != null)
                            result.ErrorMessage = string.Format("[{0}]에 대한 알람정보를 찾을수 없습니다.", zone.Name);*/
                        return null;
                    }
                }
                else
                {
                    int nSensorTagID, nSensorZoneID;

                    if (Network.NetworkWebManager.Instance.GetSensorInfo(zone, out nSensorTagID, out nSensorZoneID) == false)
                    {
                        return null;
                    }

                    alarm = new FireAlarm();
                    alarm.SensorTagID = nSensorTagID;
                    alarm.SensorZoneID = nSensorZoneID;
                }

                alarm.IsAlarmOn = false;
            }
            else
            {
                //result.ErrorMessage = string.Format("[{0}]는 알수 없는 상태값입니다.", dvcStatus);
                return null;
            }

            alarm.WebHistoryID = nWebHistoryID;
            alarm.EquipCode = dvcCd;
            alarm.EquipStatus = dvcStatus;
            alarm.EventID = evtId;
            alarm.EventType = evtType;
            alarm.Zone = zone;

            return alarm;
        }
    }
}
