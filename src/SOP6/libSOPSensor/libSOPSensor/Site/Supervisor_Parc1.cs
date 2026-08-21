using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Sensor;
using System.Collections.Concurrent;
using System.Collections;
using DBUtility2;
using libSensorProcess;
using UnE.SOP.Workstate;

namespace UnE.SOP.Site
{
    internal class Supervisor_Parc1 : Supervisor
    {
        private class AlarmData
        {
            private int m_nSensorZoneID = -1;
            private int m_nSensorZoneHistoryID = -1;
            private int m_nBuildingID = -1;
            private int m_nZoneID = -1;
            private int m_nFloorIndex = -1000;
            private int m_nEquipZoneID = -1;
            private IFacility.FacilityType m_sensorType = IFacility.FacilityType.NONE;
            private int m_nActionStepID = -1;
            private string m_strLinkedSOP = "";

            public int SensorZoneID
            {
                get { return m_nSensorZoneID; }
                set { m_nSensorZoneID = value; }
            }

            public int SensorZoneHistoryID
            {
                get { return m_nSensorZoneHistoryID; }
                set { m_nSensorZoneHistoryID = value; }
            }

            public int ActionStepID
            {
                get { return m_nActionStepID; }
                set { m_nActionStepID = value; }
            }

            public int BuildingID
            {
                get { return m_nBuildingID; }
                set { m_nBuildingID = value; }
            }

            public int ZoneID
            {
                get { return m_nZoneID; }
                set { m_nZoneID = value; }
            }

            // 0이면 1층, 1이면 2층
            // 지하는 음수로 표시(지하1층은 -1)
            public int FloorIndex
            {
                get { return m_nFloorIndex; }
                set { m_nFloorIndex = value; }
            }

            public bool HasFloor
            {
                get { return m_nFloorIndex > -1000; }
            }

            public int EquipZoneID
            {
                get { return m_nEquipZoneID; }
                set { m_nEquipZoneID = value; }
            }

            public IFacility.FacilityType SensorType
            {
                get { return m_sensorType; }
                set { m_sensorType = value; }
            }

            public string LinkedSOP
            {
                get { return m_strLinkedSOP; }
                set { m_strLinkedSOP = value; }
            }
        }

        // SOPData별로 가장 마지막에 읽은 SensorReactionHistoryID
        // Value : SensorReactionHistory ID
        private ConcurrentDictionary<SOPCheckData, int> m_dicSOPReactionHistoryIDs = new ConcurrentDictionary<SOPCheckData, int>();
        private bool m_processAlarmDepth = false;

        public Supervisor_Parc1()
        {
        }

        public Supervisor_Parc1(DirectDBManager dbMgr)
            : base(dbMgr)
        {
        }

        protected override void CheckSensorReactionHistory(SOPCheckData data)
        {
            if (IFacility.IsFireSensorType(data.SensorType))
            {
            }
            else if (IFacility.IsEarthquakeSensorType(data.SensorType))
            {
                CheckAlarmDepthFromSensorReactionHistory(data);
            }
            else if (IFacility.IsETCSensorType(data.SensorType))
            {
                CheckAlarmDepthFromSensorReactionHistory(data);
            }
        }

        protected override void OnAddSOP(SOPCheckData data)
        {
            m_dicSOPReactionHistoryIDs[data] = -1;
        }

        protected override void OnRemoveSOP(SOPCheckData data)
        {
            int nReactionHistoryID;
            m_dicSOPReactionHistoryIDs.TryRemove(data, out nReactionHistoryID);
        }

        private void CheckAlarmDepthFromSensorReactionHistory(SOPCheckData data)
        {
            if (m_processAlarmDepth)
                return;

            m_processAlarmDepth = true;

            int nReactionHistoryID = -1;

            if (m_dicSOPReactionHistoryIDs.TryGetValue(data, out nReactionHistoryID) == false)
            {
                nReactionHistoryID = -1;
            }

            string strSQL = "Select ID, SensorHistoryID, ReactionType, Time, Param1, Param2, Param3, Param4, Param5 from SensorReactionHistory";

            if (nReactionHistoryID < 0)
                strSQL += " where SensorHistoryID = " + data.SensorZoneHistoryID.ToString();
            else
                strSQL += " where ID > " + nReactionHistoryID.ToString();

            object dbMgr = GetDBManager();
            ArrayList arrResult = GetResultData(strSQL, dbMgr);

            if (arrResult == null)
            {
                m_processAlarmDepth = false;
                return;
            }

            int nResultCount = arrResult.Count;

            double value;
            string strValue;
            int nActionStepIndex;
            int nSensorType, nSensorZoneID, nEquipZoneID;

            // 마지막에 발생한 이벤트가 제일 중요하기 때문에 뒤에서부터 읽는다.
            for (int i=nResultCount-9;i>=0;i-=9)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> reactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                string strParam1 = WebDBManager.GetStringField(arrResult[i + 4]);
                string strParam2 = WebDBManager.GetStringField(arrResult[i + 5]);
                string strParam3 = WebDBManager.GetStringField(arrResult[i + 6]);
                string strParam4 = WebDBManager.GetStringField(arrResult[i + 7]);
                string strParam5 = WebDBManager.GetStringField(arrResult[i + 8]);

                if (id == null || sensorZoneHistoryID == null || reactionType == null || time == null)
                    continue;

                if (reactionType.Data == (int)ReactionType.END_STATUS ||
                    reactionType.Data == (int)ReactionType.MALFUNCTION ||
                    reactionType.Data == (int)ReactionType.USER_RESET ||
                    reactionType.Data == (int)ReactionType.IGNORE_SIGNAL ||
                    reactionType.Data == (int)ReactionType.IGNORE_SOP ||
                    reactionType.Data == (int)ReactionType.TIME_OUT)
                {
                    // 이미 종료된 SOP
                    break;
                }
                else if (reactionType.Data == (int)ReactionType.CHANGE_ALARM_DEPTH)
                {
                    if (strParam1 == null || strParam2 == null || strParam3 == null || strParam4 == null || strParam5 == null)
                        continue;

                    if (ReadLastDouble(strParam4.Trim(), out value, out strValue) && int.TryParse(strParam5.Trim(), out nActionStepIndex))
                    {
                        if (nActionStepIndex <= data.MaxActionStepIndex)
                            break;

                        if (int.TryParse(strParam1.Trim(), out nEquipZoneID) && int.TryParse(strParam2.Trim(), out nSensorZoneID) && int.TryParse(strParam3.Trim(), out nSensorType))
                        {
                            data.MaxActionStepIndex = nActionStepIndex;
                            m_sopOwner.LoadSOP(nSensorType, nEquipZoneID, time.Data, nSensorZoneID, sensorZoneHistoryID.Data, data.ActionStepHistoryID, nActionStepIndex, strValue, true);
                        }
                    }

                    break;
                }
            }

            m_processAlarmDepth = false;
        }

        // 이미 strSOPFullPath에 해당하는 SOP가 실행중이다.
        // 이 상태에서 새로운 알람 신호가 들어왔는데, 위험단계를 바꿔가며 또다른 SOP를 로딩해야 하는지를 확인한다.
        // strSOPPath : 마지막 ActionStep을 제외한 [대분류/중분류/소분류] 3단계로만 되어있다.
        // Return 값 : strSOPPath가 바뀌었는가?
        public override bool CheckOpenSOP(List<SOPScenario> currentScenarios, ref string strSOPPath, int nSensorZoneID, int nSensorZoneHistoryID, int nSensorType)
        {
            IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);

            if (IFacility.IsFireSensorType(sensorType))
            {
                return CheckOpenFireSOP(currentScenarios, ref strSOPPath, nSensorZoneID, nSensorZoneHistoryID);
            }

            return false;
        }

        public override void SortDisasterActionSteps(DisasterInfo disaster)
        {
            if (disaster.ActionSteps.Count <= 1)
                return;

            if (disaster.DisasterCategoryName == "화재")
                SortFireActionSteps(disaster);
            else if (disaster.DisasterCategoryName == "지진" || disaster.SubDisasterCategoryName == "지진")
                SortEarthquakeActionSteps(disaster);
            else if (disaster.DisasterCategoryName == "강풍" || disaster.SubDisasterCategoryName == "강풍")
                SortStrongWindActionSteps(disaster);
            else
                base.SortDisasterActionSteps(disaster);
        }

        private void SortFireActionSteps(DisasterInfo disaster)
        {
            SortActionSteps(disaster, 1, 2, 3, 0);
        }

        private void SortEarthquakeActionSteps(DisasterInfo disaster)
        {
            SortActionSteps(disaster, 0, 1, 2, 3);
        }

        private void SortStrongWindActionSteps(DisasterInfo disaster)
        {
            SortActionSteps(disaster, 0, 1, 2, 3);
        }

        private void SortActionSteps(DisasterInfo disaster, int nActionStepIndex1, int nActionStepIndex2, int nActionStepIndex3, int nActionStepIndex4)
        {
            string strActionStepName1 = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[nActionStepIndex1];
            string strActionStepName2 = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[nActionStepIndex2];
            string strActionStepName3 = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[nActionStepIndex3];
            string strActionStepName4 = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[nActionStepIndex4];

            ActionStepComparer comparer = new ActionStepComparer();
            comparer.SetActionStepPriority(strActionStepName1, 1);
            comparer.SetActionStepPriority(strActionStepName2, 2);
            comparer.SetActionStepPriority(strActionStepName3, 3);
            comparer.SetActionStepPriority(strActionStepName4, 4);

            disaster.ActionSteps.Sort(comparer);
        }

        private bool CheckOpenFireSOP(List<SOPScenario> currentScenarios, ref string strSOPPath, int nSensorZoneID, int nSensorZoneHistoryID)
        {
            // 위기경보 4단계에 대한 SOP 실행여부
            bool[] actionStepIndex = new bool[] { false, false, false, false };

            string strGivenActionStepName;
            string strGivenSOP = GetSOPPathExceptActionStep(strSOPPath, out strGivenActionStepName);

            foreach (SOPScenario scenario in currentScenarios)
            {
                string strScenarioActionStepName;
                string strScenarioSOP = GetSOPPathExceptActionStep(scenario.ActionStepFullPath, out strScenarioActionStepName);

                if (strScenarioSOP != strGivenSOP || strScenarioActionStepName == null)
                    continue;

                int nActionStepIndex = GetActionStepIndex(strScenarioActionStepName);

                if (nActionStepIndex > 0)
                    actionStepIndex[nActionStepIndex - 1] = true;
            }

            int nGivenActionStepIndex = GetActionStepIndex(strGivenActionStepName);
            
            if (nGivenActionStepIndex > 0)
            {
                /*if (actionStepIndex[nGivenActionStepIndex - 1] == false)
                    return false;
                else*/
                {
                    for (int i=nGivenActionStepIndex;i<4;i++)
                    {
                        if (actionStepIndex[i] == false)
                        {
                            // 주어진 ActionStep 보다 더 높은 단계의 SOP 가운데 아직 실행중이지 않은것이 있는지 검토
                            // 센서신호가 추가로 입력된 경우이므로 주어진 것보다 하위의 단계는 고려하지 않는다.
                            int nResultActionStepIndex = ReadSensorHistory(GetDBManager(), IFacility.FacilityType.FIRE_SENSOR, nSensorZoneID);

                            if (nResultActionStepIndex < 0)
                                break;

                            if (actionStepIndex[nResultActionStepIndex - 1] == false)
                            {
                                string strActionStepName = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[nResultActionStepIndex - 1];
                                strSOPPath = strGivenSOP + "/" + strActionStepName;
                                return true;
                            }
                        }
                    }
                }
            }
            
            return false;
        }

        private string GetSOPPathExceptActionStep(string strSOPPath, out string strStepName)
        {
            strStepName = null;
            int nIndex = strSOPPath.LastIndexOf('/');

            if (nIndex > 0)
            {
                strStepName = strSOPPath.Substring(nIndex + 1).Trim();
                return strSOPPath.Substring(0, nIndex);
            }

            nIndex = strSOPPath.LastIndexOf('\\');

            if (nIndex > 0)
            {
                strStepName = strSOPPath.Substring(nIndex + 1).Trim();
                return strSOPPath.Replace('\\', '/').Substring(0, nIndex);
            }

            nIndex = strSOPPath.LastIndexOf((char)0x06);

            if (nIndex > 0)
            {
                strStepName = strSOPPath.Substring(nIndex + 1).Trim();
                return strSOPPath.Replace((char)0x06, '/').Substring(0, nIndex);
            }

            return strSOPPath;
        }

        // 우선순위에 따라 위기경보단계 이름들을 정렬한다.
        protected override Dictionary<int, string> SortStandardActionStepNames(int nSensorType)
        {
            IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);

            if (IFacility.IsFireSensorType(sensorType))
            {
                Dictionary<int, string> dicActionSteps = new Dictionary<int, string>();

                // 우선순위에 따라 위기경보단계 이름들을 정렬한다.
                dicActionSteps[0] = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[1];
                dicActionSteps[1] = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[2];
                dicActionSteps[2] = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[3];
                dicActionSteps[3] = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[0];

                return dicActionSteps;
            }
            else if (IFacility.IsETCSensorType(sensorType))
            {
                IFacility.FacilityType facilityType = (IFacility.FacilityType)sensorType;

                if (facilityType == IFacility.FacilityType.BLACKOUT)
                {
                    Dictionary<int, string> dicActionSteps = base.SortStandardActionStepNames(nSensorType);

                    int nIndex = -1;
                    string strTarget = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[3];

                    foreach (KeyValuePair<int, string> pair in dicActionSteps)
                    {
                        if (pair.Value == strTarget)
                        {
                            nIndex = pair.Key;
                            dicActionSteps.Remove(pair.Key);
                            break;
                        }
                    }

                    if (nIndex >= 0)
                    {
                        string strActionStepName;

                        for (int i = nIndex - 1; i >= 0; i--)
                        {
                            if (dicActionSteps.TryGetValue(i, out strActionStepName) == false)
                            {
                                return base.SortStandardActionStepNames(nSensorType);
                            }

                            dicActionSteps[i + 1] = strActionStepName;
                        }

                        dicActionSteps[0] = strTarget;
                    }

                    return dicActionSteps;
                }
            }

            return base.SortStandardActionStepNames(nSensorType);
        }

        // 현재 발생한 알람상황에 맞는 ActionStep Index를 리턴한다.
        private int ReadSensorHistory(object dbMgr, IFacility.FacilityType facilityType, int nCurrentSensorZoneID)
        {
            string strQueryField = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID, srh.DetectionStatus, szh.Param2, szh.Param3 ";

            // SensorZone ID가 존재하는 SensorZoneHistory(센서로부터 발생한 신호) 검색
            string szText = strQueryField + ", sz.Data, sz.Type ";
            szText += "FROM SensorReactionHistory as srh, SensorZoneHistory as szh, SensorZone as sz, EquipmentZone as ez ";
            szText += "WHERE srh.ID in (";
            szText += "Select max(ID) from SensorReactionHistory where SensorHistoryID in (SELECT SensorHistoryID FROM SensorReactionHistory WHERE ReactionType in " + GetAlarmReactionHistoryQueryString() + ") and ";
            szText += "SensorHistoryID not in (SELECT SensorHistoryID FROM SensorReactionHistory WHERE ReactionType in " + GetAlarmOffReactionHistoryQueryString() + ") group by SensorHistoryID, ReactionType)";
            szText += "     AND srh.SensorHistoryID = szh.ID ";
            szText += "     AND szh.SensorID = sz.ID ";
            szText += "     AND sz.EquipZoneID = ez.ID ";
            szText += "     AND ez.SiteID = {0} ";
            szText += "     ORDER BY srh.Time, szh.SensorID";

            DateTime dtNow = DateTime.Now;
            string szNowTime = WebDBManager.MakeDateTimeString(DateTime.Now);
            DateTime dtPrev = dtNow.AddDays(-1.0);
            string szPrevTime = WebDBManager.MakeDateTimeString(dtPrev);
            // SensorZone ID가 존재하는 SensorZoneHistory(센서로부터 발생한 신호) 검색
            string strSQL = string.Format(szText, GetSiteID(), szPrevTime, szNowTime);

            ArrayList arrResult = GetResultData(strSQL, dbMgr);
            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;

            DateTime dtDefault = new DateTime();

            // Key : SensorZone ID
            // Value : FacilityType, ReactionType, SensorZoneHistoryID
            Dictionary<int, ArrayList> dicNoZoneInfoSensors = new Dictionary<int, ArrayList>();
            string strNoZoneSensorIDs = "";

            // Key : SensorZone ID
            Dictionary<int, AlarmData> dicSensorZoneAlarms = new Dictionary<int, AlarmData>();

            for (int i = 0; i < nResultCount - 15; i += 16)
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
                //string strSensorType = WebDBManager.GetStringField(arrResult[i + 12], "");
                string strSensorZoneIDs = WebDBManager.GetStringField(arrResult[i + 13]);
                string strSensorData = WebDBManager.GetStringField(arrResult[i + 14]);
                VariousData<int> sensorZoneType = WebDBManager.GetIntField(arrResult[i + 15].ToString());

                if (nID < 0 || nHistoryID < 0 || sensorZoneType == null)
                    continue;

                // 하루가 경과된 알람들은 무시한다.
                if (time < dtPrev)
                    continue;

                IFacility.FacilityType sensorType = IFacility.ToFacilityType(sensorZoneType.Data);

                if (sensorType != facilityType)
                    continue;

                if (strSensorZoneIDs == null || strSensorZoneIDs.Length == 0)
                {
                    if (strNoZoneSensorIDs.Length == 0)
                        strNoZoneSensorIDs = nSensorID.ToString();
                    else
                        strNoZoneSensorIDs += ", " + nSensorID.ToString();

                    ArrayList arrSensorZoneInfos = new ArrayList();
                    arrSensorZoneInfos.Add(sensorType);
                    arrSensorZoneInfos.Add(nReactionType);
                    arrSensorZoneInfos.Add(nHistoryID);

                    dicNoZoneInfoSensors[nSensorID] = arrSensorZoneInfos;
                    //CheckAlarmSensorZone(nSensorID, sensorType, nReactionType, nHistoryID, nID, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, dbMgr);
                }
                else
                {
                    int nSensorZoneID;
                    string[] ids = strSensorZoneIDs.Split(',');

                    foreach (string strID in ids)
                    {
                        if (int.TryParse(strID.Trim(), out nSensorZoneID))
                        {
                            if (SOPWebServer.Header.ManualReportDefaultID <= nSensorZoneID)
                            {
                                int nZoneID;

                                if (int.TryParse(strParam1.Trim(), out nZoneID) == false)
                                    nZoneID = -1;

                                // 수동신고일 경우
                                CheckAlarmSensorZone(dicSensorZoneAlarms, nSensorZoneID, sensorType, nReactionType, nHistoryID, -1, nZoneID);
                            }
                            else
                            {
                                int nEquipZoneID;

                                if (int.TryParse(strParam1.Trim(), out nEquipZoneID) == false)
                                    nEquipZoneID = -1;

                                // 센서신호일 경우
                                CheckAlarmSensorZone(dicSensorZoneAlarms, nSensorZoneID, sensorType, nReactionType, nHistoryID, nEquipZoneID, -1);
                            }
                        }
                    }
                }
            }

            if (strNoZoneSensorIDs.Length > 0)
            {
                strSQL = "Select ID, EquipZoneID, Zone from SensorZone where ID in (" + strNoZoneSensorIDs + ")";
                arrResult = GetResultData(strSQL, dbMgr);

                if (arrResult == null)
                    return -1;

                nResultCount = arrResult.Count;
                ArrayList arrSensorZoneInfos = null;

                for (int i=0;i<nResultCount-2;i+=3)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                    VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                    if (id == null)
                        continue;

                    if (dicNoZoneInfoSensors.TryGetValue(id.Data, out arrSensorZoneInfos) == false)
                        continue;

                    IFacility.FacilityType sensorType = (IFacility.FacilityType)arrSensorZoneInfos[0];
                    int nReactionType = (int)arrSensorZoneInfos[1];
                    int nSensorZoneHistoryID = (int)arrSensorZoneInfos[2];

                    if (equipZoneID != null)
                    {
                        CheckAlarmSensorZone(dicSensorZoneAlarms, id.Data, sensorType, nReactionType, nSensorZoneHistoryID, equipZoneID.Data, -1);
                    }
                    else if (zoneID != null)
                    {
                        CheckAlarmSensorZone(dicSensorZoneAlarms, id.Data, sensorType, nReactionType, nSensorZoneHistoryID, -1, zoneID.Data);
                    }
                }
            }

            AlarmData currentAlarm;
            int nCurrentBuildingID = -1;

            if (dicSensorZoneAlarms.TryGetValue(nCurrentSensorZoneID, out currentAlarm))
            {
                nCurrentBuildingID = currentAlarm.BuildingID;
            }

            List<AlarmData> alarms = dicSensorZoneAlarms.Values.ToList();
            return GetFireActionStepIndex(alarms, nCurrentBuildingID);
        }

        // Return 값 : -1이면 원래 주어진 SOP를 그대로 사용하면 된다.
        private int GetFireActionStepIndex(List<AlarmData> alarms, int nBuildingID)
        {
            if (alarms.Count == 0)
                return -1;

            // Key : Floor Index
            Dictionary<int, List<AlarmData>> dicFloorAlarms = new Dictionary<int, List<AlarmData>>();
            List<AlarmData> floorAlarms = null;

            bool oneMoreAlarm = false;

            foreach (AlarmData alarm in alarms)
            {
                if (alarm.HasFloor && alarm.BuildingID == nBuildingID)
                {
                    if (dicFloorAlarms.TryGetValue(alarm.FloorIndex, out floorAlarms) == false)
                    {
                        floorAlarms = new List<AlarmData>();
                        dicFloorAlarms[alarm.FloorIndex] = floorAlarms;
                    }

                    floorAlarms.Add(alarm);

                    if (floorAlarms.Count >= 2)
                        oneMoreAlarm = true;
                }
            }

            int nAlarmCount = dicFloorAlarms.Count;

            if (nAlarmCount == 0)
                return -1;

            List<int> floorIndex = dicFloorAlarms.Keys.ToList();
            floorIndex.Sort();

            int nPrevFloorIndex = floorIndex[0];

            for (int i=1;i<nAlarmCount;i++)
            {
                int nFloorIndex = floorIndex[i];

                // 인접한 층인가?
                if (nPrevFloorIndex == nFloorIndex + 1 || nPrevFloorIndex == nFloorIndex - 1)
                {
                    // 인접한 2개 층 이상에서 동시에 화재신호가 탐지되면 심각 단계 발동
                    return 4;
                }
            }

            // 1개층 2개 이상의 구역에서 화재신호가 탐지되면 경계 단계 발동
            if (oneMoreAlarm)
                return 3;

            // 기타의 경우는 주의 단계
            return 2;
        }

        private void CheckAlarmSensorZone(Dictionary<int, AlarmData> dicSensorZoneAlarms, int nSensorZoneID, IFacility.FacilityType sensorType, int nReactionType, int nSensorZoneHistoryID, int nEquipZoneID, int nZoneID)
        {
            // SensorType을 알수 없으면 재난 타입을 알수 없다.
            if (sensorType == IFacility.FacilityType.NONE)
                return;

            libSensorProcess.ReactionType type = (libSensorProcess.ReactionType)nReactionType;
            
            if (type == libSensorProcess.ReactionType.BEGIN_STATUS || type == libSensorProcess.ReactionType.NOTIFY_SIGNAL || type == libSensorProcess.ReactionType.CHANGE_ALARM_DEPTH)
            {
                AlarmData alarm = AddSensorAlarm(nSensorZoneID, nSensorZoneHistoryID, sensorType, nEquipZoneID, nZoneID);

                if (alarm != null)
                    dicSensorZoneAlarms[nSensorZoneID] = alarm;
            }
        }

        private AlarmData AddSensorAlarm(int nSensorZoneID, int nSensorZoneHistoryID, IFacility.FacilityType sensorType, int nEquipZoneID, int nZoneID)
        {
            if (m_sopOwner == null)
                return null;

            AlarmData alarm = new AlarmData();

            if (nEquipZoneID > 0)
            {
                string strEquipZoneName;
                int nBuildingID, nFloorIndex;

                if (m_sopOwner.GetEquipmentZoneInfo(nEquipZoneID, out strEquipZoneName, out nZoneID, out nFloorIndex, out nBuildingID))
                {
                    alarm.BuildingID = nBuildingID;
                    alarm.ZoneID = nZoneID;
                    alarm.EquipZoneID = nEquipZoneID;
                    alarm.FloorIndex = nFloorIndex;
                }
            }
            else if (nZoneID > 0)
            {
                string strZoneName;
                int nBuildingID, nFloorIndex;

                if (m_sopOwner.GetZoneInfo(nZoneID, out strZoneName, out nFloorIndex, out nBuildingID))
                {
                    alarm.BuildingID = nBuildingID;
                    alarm.ZoneID = nZoneID;
                    alarm.FloorIndex = nFloorIndex;
                }
            }

            //alarm.ActionStepID
            //alarm.LinkedSOP
            alarm.SensorType = sensorType;
            alarm.SensorZoneHistoryID = nSensorZoneHistoryID;
            alarm.SensorZoneID = nSensorZoneID;

            return alarm;
        }

        // 현재 Alarm이 발생중인 SensorReactionLog에 대한 Query 조건문
        private string GetAlarmReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.BEGIN_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.NOTIFY_SIGNAL).ToString();

            return "(" + strCondition + ")";
        }

        //현재 Alarm이 꺼진 SensorReactionLog에 대한 Query조건문
        private string GetAlarmOffReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.MALFUNCTION).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SIGNAL).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SOP).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.END_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.USER_RESET).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.TIME_OUT).ToString();

            return "(" + strCondition + ")";
        }
    }
}
