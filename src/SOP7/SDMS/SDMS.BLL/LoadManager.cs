using dnsData.Sensor;
using SDMS.BLL.Models.Alarm;
using SDMS.BLL.Models.Response;
using SDMS.BLL.Models.Request;
using SDMS.IDAL;
using SDMS.Model.Alarm;
using SDMS.Model.History;
using SDMS.Model.Sensor;
using SDMS.Model.Spatial;
using SDMS.Model.CCTV;
using SDMS.Model.Facility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.IO;

namespace SDMS.BLL
{
    using Models.Data;
    using Models.Data.Sensor;
    using System.Linq;
    using System.Text;

    public class LoadManager
    {
        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        private static bool m_bTimerRunning = false;     // 이력 조회중인가 ?
        private static int m_nMaxReactionHistoryID = -1; // 새로운 알람 이력만 조회한다

        private static Timer m_timerAlarm = null; // 알람 감시 타이머
        private static List<AlarmData> m_alarmDatas = null;
        /// <summary>
        /// 개수 제한한 알람
        /// </summary>
        public List<AlarmData> AlarmDatas { get { return m_alarmDatas; } }
        private static List<AlarmData> m_allAlarmDatas = null;
        /// <summary>
        /// 발생한 모든 알람
        /// </summary>
        public List<AlarmData> AllAlarmDatas { get { return m_allAlarmDatas; } }

        private static SensorManager m_sensorManager = null;
        private static SpatialManager m_spatialManager = null;
        private static bool m_completeLoading = false;

        private static ArrayList m_lastCurrentAlarms = null;
        private static bool isLoading = false;

        private static DateTime m_dtLastCheckSensorCount = new DateTime();

        /// <summary>
        /// 사이트 별로 사용중인 PSM, ETC 센서 종류
        /// </summary>
        private static Dictionary<int, Material> m_dicMaterials = new Dictionary<int, Material>();
        

        public LoadManager(IDataManager dataManager, ProcessManager processManager)
        {
            this.m_dataManager = dataManager;
            this.m_processManager = processManager;
                        
            if (!isLoading)
            {
                isLoading = true;
                m_sensorManager = new SensorManager();
                m_spatialManager = new SpatialManager();

                LoadSpatial();
                LoadUseFacilityType();
                m_sensorManager.LoadSensorList(dataManager, m_spatialManager);
                LoadMaterial();

                m_completeLoading = true;
            }

            InitTimerSensorReactionHistory();
        }

        // 사용하는 센서 신호
        private void LoadUseFacilityType()
        {
            string strErrorMessage = null;
            List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(
                Common.Model.Option.Options.OptionTarget.SDMS, "UseFacilityType", out strErrorMessage);

            if (options == null || options.Count == 0)
                return;

            List<string> useFacilityType = null;
            
            foreach (Common.Model.Option.Options option in options)
            {
                if (option.PropertyName == "UseFacilityType")
                {
                    useFacilityType = new List<string>();
                    Facility.FacilityType type;
                    Enum.TryParse(option.PropertyValue, out type);
                    Facility.UseFacilityType.Add(type);
                    break;
                }
            }
        }

        private void LoadMaterial()
        {
            string strErrorMessage = null;
            m_dicMaterials.Clear();

            Dictionary<Material.Fields, object> dicConditions = new Dictionary<Material.Fields, object>();
            string strAdditionalConditions = "";
            List<Material> materials = m_dataManager.GetSelectManager().SelectMaterials(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (materials == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadMaterial Error : " + strErrorMessage);
                return;
            }
            else if (materials.Count == 0)
                return;

            foreach (Material material in materials)
            {
                m_dicMaterials[material.ID] = material;
            }
        }
        
        private bool LoadSpatial()
        {
            return m_spatialManager.LoadSpatial(m_dataManager);
        }

        // 센서 히스토리 감시 타이머
        private void InitTimerSensorReactionHistory()
        {
            if (m_timerAlarm == null)
            {
                m_timerAlarm = new Timer();
                m_timerAlarm.Interval = 1000 * 1.5;
                
                if (m_dataManager.SiteID == 11)
                {
                    m_timerAlarm.Elapsed += new ElapsedEventHandler(timerNSTAlarm_Elapsed);                    
                }
                else
                {
                    m_timerAlarm.Elapsed += new ElapsedEventHandler(timerLoadCurrentAlarm_Elapsed);
                }
                m_timerAlarm.Start();
            }
            
        }

        // SensorCount는 1분에 한번씩만 검사한다.
        private bool IsSensorCountTime()
        {
            DateTime dtNow = DateTime.Now;
            TimeSpan span = dtNow - m_dtLastCheckSensorCount;

            if (span.TotalMinutes >= 1.0)
            {
                m_dtLastCheckSensorCount = dtNow;
                return true;
            }

            return false;
        }

        /// <summary>
        /// (솔브레인) 현재 발생중인 알람 감시 타이머
        /// </summary>
        private void timerLoadCurrentAlarm_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_bTimerRunning)
                return;

            m_bTimerRunning = true;

            if (m_sensorManager != null && IsSensorCountTime())
            {
                m_sensorManager.CheckDisabledSensors(m_dataManager);
            }

            DateTime dtNow = DateTime.Now;
            //DateTime dtBefore = dtNow.AddDays(-1); // 최근 하루
            DateTime dtBefore = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0); // 당일
                        
            List<AlarmData> alarmDatas = CopyAlarmDatas();
            List<AlarmData> allAlarmDatas = CopyAllAlarmDatas();

            string strErrorMessage = null;

            int currentAlarmsCount = 0;            
            List<CurrentAlarm> currentAlarms = m_dataManager.GetSelectManager().SelectCurrentAlarms(null, "", out strErrorMessage);
            if (currentAlarms == null)
            {
                m_alarmDatas = alarmDatas;
                m_allAlarmDatas = allAlarmDatas;
                m_bTimerRunning = false;
                return;
            }

            currentAlarmsCount = currentAlarms.Count;

            if (allAlarmDatas != null && allAlarmDatas.Count > 0)
            {
                // SopStatus Update
                int listAlarmDataCount = alarmDatas.Count;
                for (int i = 0; i < currentAlarmsCount; i++)
                {
                    for (int j = 0; j < listAlarmDataCount; j++)
                    {
                        if (currentAlarms[i].SensorZoneHistoryID == alarmDatas[j].SensorZoneHistoryID)
                        {
                            alarmDatas[j].SopStatus = currentAlarms[i].SopStatus;
                            break;
                        }
                    }
                }

                // 당일 지난 알람 삭제
                // 시간 역순으로 저장되어 있음
                int allAlarmDataCount = allAlarmDatas.Count;
                List<AlarmData> deleteData = new List<AlarmData>();
                for (int i = allAlarmDataCount - 1; i >= 0; i--)
                {
                    if (allAlarmDatas[i].dtTime < dtBefore)
                    {
                        if (allAlarmDatas[i].IsAlarm)
                        {
                            // 아직도 알람 진행중인지 체크
                            if (currentAlarmsCount > 0)
                            {
                                int matchAlarm = currentAlarms.Where(p => p.SensorZoneHistoryID == allAlarmDatas[i].SensorZoneHistoryID).Count();
                                if (matchAlarm > 0)
                                    continue;
                            }
                        }

                        deleteData.Add(allAlarmDatas[i]);
                    }
                    else
                        break;
                }

                foreach (AlarmData item in deleteData)
                {
                    for (int i = alarmDatas.Count - 1; i >= 0; i--)
                    {
                        if (item.SensorZoneHistoryID == alarmDatas[i].SensorZoneHistoryID)
                        {
                            alarmDatas.RemoveAt(i);
                            break;
                        }
                    }

                    for (int i = allAlarmDatas.Count - 1; i >= 0; i--)
                    {
                        if (item.SensorZoneHistoryID == allAlarmDatas[i].SensorZoneHistoryID)
                        {
                            allAlarmDatas.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            // 현재 발생중인 알람
            // 하루가 지난 알람도 발생중이면 조회해야 함
            string currentSensorZoneHistoryIDs = "";
            if (currentAlarms != null && currentAlarmsCount > 0)
            {
                currentSensorZoneHistoryIDs = string.Join(", ", currentAlarms.Select(p => p.SensorZoneHistoryID));
            }

            StringBuilder sbCondition = new StringBuilder();
            if (currentSensorZoneHistoryIDs.Length > 0)
            {                
                sbCondition.AppendFormat("({0}.{1} >= '{2}' And {0}.{1} <= '{3}' OR {4}.{5} in ({6})) "
                    , SensorReactionHistory.TableName, SensorReactionHistory.Fields.Time
                    , dtBefore.ToString("yyyy-MM-dd HH:mm:ss"), dtNow.ToString("yyyy-MM-dd HH:mm:ss")
                    , SensorZoneHistory.TableName, SensorZoneHistory.Fields.ID, currentSensorZoneHistoryIDs);
            }
            else
            {
                sbCondition.AppendFormat("{0}.{1} >= '{2}' And {0}.{1} <= '{3}' "
                    , SensorReactionHistory.TableName, SensorReactionHistory.Fields.Time
                    , dtBefore.ToString("yyyy-MM-dd HH:mm:ss"), dtNow.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            sbCondition.AppendFormat("And {0}.{1} in (0,21,50,62,64) ", SensorReactionHistory.TableName, SensorReactionHistory.Fields.ReactionType);
            sbCondition.AppendFormat("And {0}.{1} > {2} ", SensorReactionHistory.TableName, SensorReactionHistory.Fields.ID, m_nMaxReactionHistoryID);
            sbCondition.AppendFormat("ORDER BY {0}.{1} DESC", SensorReactionHistory.TableName, SensorReactionHistory.Fields.Time);

            ArrayList arrResult = m_dataManager.GetSelectManager().JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory(null, null, null, null, sbCondition.ToString(), out strErrorMessage);
            if (arrResult == null)
            {
                m_alarmDatas = alarmDatas;
                m_allAlarmDatas = allAlarmDatas;
                m_bTimerRunning = false;
                return;
            }

            int nResultCount = arrResult.Count;
            if (arrResult.Count == 0)
            {
                m_alarmDatas = alarmDatas;
                m_allAlarmDatas = allAlarmDatas;
                m_bTimerRunning = false;
                return;
            }

            if (allAlarmDatas == null)
                allAlarmDatas = new List<AlarmData>();

            // 각 알람이 어떻게 종료되었는지, 진행중인지 판단 (50:상황종료/21:오작동/64:user reset)
            List<SensorZoneKey> endTypes = new List<SensorZoneKey>();

            for (int i = 0; i < nResultCount; i += 5)
            {
                if (arrResult[i] is EquipmentZone &&
                    arrResult[i + 1] is SensorReactionHistory &&
                    arrResult[i + 2] is SensorZone &&
                    arrResult[i + 3] is SensorZoneHistory &&
                    //arrResult[i + 4] is Building &&
                    arrResult[i + 4] is Zone)
                {
                    AlarmData alarmData = new AlarmData();

                    EquipmentZone eq = arrResult[i] as EquipmentZone;
                    SensorReactionHistory srh = arrResult[i + 1] as SensorReactionHistory;
                    SensorZone sz = arrResult[i + 2] as SensorZone;
                    SensorZoneHistory szh = arrResult[i + 3] as SensorZoneHistory;
                    //Building b = arrResult[i + 4] as Building;
                    Zone z = arrResult[i + 4] as Zone;

                    //allSensorZoneIDs.AddRange(szh.AllSensorZoneIDs);

                    int sensorZoneID;
                    int.TryParse(srh.Param2, out sensorZoneID);
                    int isAlarm;
                    int.TryParse(srh.Param4, out isAlarm); // ReactionType이 62(CHANGE_ALARM_DEPTH) 일 때 

                    // 알람 해제
                    if (srh.ReactionType == SensorReactionHistory.ReactionTypes.MALFUNCTION ||
                        srh.ReactionType == SensorReactionHistory.ReactionTypes.END_STATUS ||
                        srh.ReactionType == SensorReactionHistory.ReactionTypes.USER_RESET)
                    {
                        // 같은 EquipZone에 있는 다른 알람도 해제한다
                        foreach (int item in szh.AllSensorZoneIDs)
                        {
                            SensorZoneKey key = new SensorZoneKey();
                            key.SensorZoneHistoryID = srh.SensorZoneHistoryID;
                            key.SensorZoneID = item;
                            key.ReactionType = srh.ReactionType;
                            endTypes.Add(key); 
                        }
                    }
                    // 알람 단계 변경
                    else if (srh.ReactionType == SensorReactionHistory.ReactionTypes.CHANGE_ALARM_DEPTH)
                    {                        
                        for (int j = 0; j < allAlarmDatas.Count; j++)
                        {
                            if (allAlarmDatas[j].SensorZoneHistoryID == szh.ID)
                            {
                                int alarmDepth;
                                if (int.TryParse(srh.Param5, out alarmDepth))
                                    allAlarmDatas[j].AlarmDepth = alarmDepth;
                            }

                            for (int c = 0; c < currentAlarmsCount; c++)
                            {
                                if (allAlarmDatas[j].SensorZoneHistoryID == currentAlarms[c].SensorZoneHistoryID)
                                {
                                    allAlarmDatas[j].AlarmSensorZoneIDs = currentAlarms[c].AlarmSensorZoneIDs;
                                    break;
                                }
                            }
                        }
                    }
                    else // 추가                     
                    {
                        alarmData.dtTime = srh.Time;
                        alarmData.StrDateTime = srh.Time.ToString("yyyy-MM-dd HH:mm:ss");
                        alarmData.OrgSensorID = sz.OrgSensorID;
                        alarmData.SensorZoneID = sensorZoneID;
                        alarmData.SensorZoneHistoryID = szh.ID;
                        if (z.BuildingID != null)
                        {
                            Building b = m_dataManager.GetSelectManager().SelectBuilding((int)z.BuildingID, out strErrorMessage);
                            if (b != null)
                                alarmData.BuildingName = b.DisplayText;
                        }
                        alarmData.ZoneName = z.DisplayText;
                        alarmData.PositionName = eq.DisplayText;
                        alarmData.ZoneID = z.ID;
                        alarmData.EquipZoneID = eq.ID;
                        alarmData.FacilityType = Facility.ToFacilityType(szh.SensorType);
                        if (sz.ID >= dnsSopID.Header.ManualReportDefaultID)
                            alarmData.FacilityTypeString = "수동신고";
                        else
                        {
                            alarmData.FacilityTypeString = Facility.GetNFacilityTypeString(szh.SensorType);

                            Material material = getSensorMaterialType(alarmData.FacilityType, (int)alarmData.OrgSensorID, out strErrorMessage);
                            if (material != null)
                            {
                                alarmData.MaterialType = material.ID;
                                alarmData.MaterialTypeString = material.MaterialName;
                            }
                        }
                        alarmData.Message = srh.Message;

                        int alarmDepth;
                        if (int.TryParse(srh.Param5, out alarmDepth))
                            alarmData.AlarmDepth = alarmDepth;

                        alarmData.AlarmSensorZoneIDs = szh.AllSensorZoneIDs;
                        for (int j = 0; j < currentAlarmsCount; j++)
                        {
                            if (currentAlarms[j].SensorZoneHistoryID == szh.ID)
                            {
                                alarmData.SopStatus = currentAlarms[j].SopStatus;
                                alarmData.AlarmSensorZoneIDs = currentAlarms[j].AlarmSensorZoneIDs;
                                break;
                            }
                        }

                        // 수동신고
                        if (sz.ID >= dnsSopID.Header.ManualReportDefaultID)
                        {
                            alarmData.ReportPerson = srh.Param3;
                            alarmData.Memo = srh.Param4;
                        }

                        allAlarmDatas.Add(alarmData);
                    }                  

                    m_nMaxReactionHistoryID = Math.Max(m_nMaxReactionHistoryID, srh.ID);
                }
            }

            #region 알람 종료 방식 지정
            int alarmDataCount = allAlarmDatas.Count;
            foreach (SensorZoneKey key in endTypes)
            {
                for (int i = 0; i < alarmDataCount; i++)
                {
                    if (key.SensorZoneHistoryID == allAlarmDatas[i].SensorZoneHistoryID && allAlarmDatas[i].AlarmSensorZoneIDs.Contains(key.SensorZoneID))
                    {
                        if (allAlarmDatas[i].IsAlarm && allAlarmDatas[i].ReleaseInfo.Length == 0)
                        {
                            allAlarmDatas[i].IsAlarm = false;
                            switch (key.ReactionType)
                            {
                                case SensorReactionHistory.ReactionTypes.END_STATUS:
                                    allAlarmDatas[i].ReleaseInfo = "현장 종료";
                                    break;
                                case SensorReactionHistory.ReactionTypes.MALFUNCTION:
                                    allAlarmDatas[i].ReleaseInfo = "오작동 처리";
                                    break;
                                case SensorReactionHistory.ReactionTypes.USER_RESET:
                                    allAlarmDatas[i].ReleaseInfo = "사용자 종료";
                                    break;
                            }
                            break; 
                        }
                    }
                }
            }
            #endregion

            allAlarmDatas = allAlarmDatas.OrderByDescending(p => p.IsAlarm == true).ThenByDescending(p => p.dtTime).ThenByDescending(p => p.SensorZoneHistoryID).ToList();

            int svmsEventCount = 0;
            List<AlarmData> deleteAlarms = new List<AlarmData>();
            for (int i = 0; i < alarmDataCount; i++)
            {
                if (Facility.IsSVMSSensorType(allAlarmDatas[i].FacilityType))
                {
                    if (svmsEventCount > 29) // SVMS 이벤트는 30개까지만 보여줌
                    {
                        deleteAlarms.Add(allAlarmDatas[i]);
                    }
                    else
                    {
                        svmsEventCount++;
                    }
                }                    
            }

            m_allAlarmDatas = allAlarmDatas;
            List<AlarmData> splitAlarmDatas = new List<AlarmData>(allAlarmDatas);

            foreach (AlarmData item in deleteAlarms)
            {
                splitAlarmDatas.Remove(item);
            }            
            
            int totalCount = splitAlarmDatas.Count;
            if (totalCount > 100) //100개까지만 표현함
            {
                splitAlarmDatas.RemoveRange(100, totalCount - 100);
            }

            m_alarmDatas = splitAlarmDatas;

            m_bTimerRunning = false;
        }

        /// <summary>
        /// (NST) 현재 발생중인 알람 감시 타이머
        /// </summary>
        private void timerNSTAlarm_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_bTimerRunning)
                return;

            m_bTimerRunning = true;

            DateTime dtNow = DateTime.Now;
            DateTime dtBefore2 = dtNow.AddDays(-1); // 최근 하루
            DateTime dtBefore = new DateTime(dtBefore2.Year, dtBefore2.Month, dtBefore2.Day, 0, 0, 0);
            //DateTime dtBefore = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0); // 당일

            List<AlarmData> allAlarmDatas = CopyAllAlarmDatas();

            string strErrorMessage = null;

            int currentAlarmsCount = 0;
            List<CurrentAlarm> currentAlarms = m_dataManager.GetSelectManager().SelectCurrentAlarms(null, "", out strErrorMessage);
            if (currentAlarms == null)
            {
                m_allAlarmDatas = allAlarmDatas;
                m_bTimerRunning = false;
                return;
            }

            currentAlarmsCount = currentAlarms.Count;

            if (allAlarmDatas != null && allAlarmDatas.Count > 0)
            {
                // SopStatus Update
                int listAlarmDataCount = allAlarmDatas.Count;
                for (int i = 0; i < currentAlarmsCount; i++)
                {
                    for (int j = 0; j < listAlarmDataCount; j++)
                    {
                        if (currentAlarms[i].SensorZoneHistoryID == allAlarmDatas[j].SensorZoneHistoryID)
                        {
                            allAlarmDatas[j].SopStatus = currentAlarms[i].SopStatus;
                            break;
                        }
                    }
                }

                // 당일 지난 알람 삭제
                // 시간 역순으로 저장되어 있음
                List<AlarmData> deleteData = new List<AlarmData>();
                for (int i = listAlarmDataCount - 1; i >= 0; i--)
                {
                    if (allAlarmDatas[i].dtTime < dtBefore)
                    {
                        if (allAlarmDatas[i].IsAlarm)
                        {
                            // 아직도 알람 진행중인지 체크
                            if (currentAlarmsCount > 0)
                            {
                                int matchAlarm = currentAlarms.Where(p => p.SensorZoneHistoryID == allAlarmDatas[i].SensorZoneHistoryID).Count();
                                if (matchAlarm > 0)
                                    continue;
                            }
                        }

                        deleteData.Add(allAlarmDatas[i]);
                    }
                    else
                        break;
                }

                foreach (AlarmData item in deleteData)
                {
                    allAlarmDatas.Remove(item);
                }
            }

            // 현재 발생중인 알람
            // 하루가 지난 알람도 발생중이면 조회해야 함
            string currentSensorZoneHistoryIDs = "";
            if (currentAlarms != null && currentAlarmsCount > 0)
            {
                currentSensorZoneHistoryIDs = string.Join(", ", currentAlarms.Select(p => p.SensorZoneHistoryID));
            }

            StringBuilder sbCondition = new StringBuilder();
            if (currentSensorZoneHistoryIDs.Length > 0)
            {
                sbCondition.AppendFormat("({0}.{1} >= '{2}' And {0}.{1} <= '{3}' OR {4}.{5} in ({6})) "
                    , SensorReactionHistory.TableName, SensorReactionHistory.Fields.Time
                    , dtBefore.ToString("yyyy-MM-dd HH:mm:ss"), dtNow.ToString("yyyy-MM-dd HH:mm:ss")
                    , SensorZoneHistory.TableName, SensorZoneHistory.Fields.ID, currentSensorZoneHistoryIDs);
            }
            else
            {
                sbCondition.AppendFormat("{0}.{1} >= '{2}' And {0}.{1} <= '{3}' "
                    , SensorReactionHistory.TableName, SensorReactionHistory.Fields.Time
                    , dtBefore.ToString("yyyy-MM-dd HH:mm:ss"), dtNow.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            sbCondition.AppendFormat("And {0}.{1} in (0,21,50,62,64) ", SensorReactionHistory.TableName, SensorReactionHistory.Fields.ReactionType);
            sbCondition.AppendFormat("And {0}.{1} > {2} ", SensorReactionHistory.TableName, SensorReactionHistory.Fields.ID, m_nMaxReactionHistoryID);
            sbCondition.AppendFormat("ORDER BY {0}.{1} DESC", SensorReactionHistory.TableName, SensorReactionHistory.Fields.Time);

            ArrayList arrResult = m_dataManager.GetSelectManager().JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory2(null, null, null, null, sbCondition.ToString(), out strErrorMessage);
            if (arrResult == null)
            {
                m_allAlarmDatas = allAlarmDatas;
                m_bTimerRunning = false;
                return;
            }

            int nResultCount = arrResult.Count;
            if (arrResult.Count == 0)
            {
                m_allAlarmDatas = allAlarmDatas;
                m_bTimerRunning = false;
                return;
            }

            if (allAlarmDatas == null)
                allAlarmDatas = new List<AlarmData>();

            // 각 알람이 어떻게 종료되었는지, 진행중인지 판단 (50:상황종료/21:오작동/64:user reset)
            List<SensorZoneKey> endTypes = new List<SensorZoneKey>();

            for (int i = 0; i < nResultCount; i += 5)
            {
                if (arrResult[i] is EquipmentZone &&
                    arrResult[i + 1] is SensorReactionHistory &&
                    arrResult[i + 2] is SensorZone &&
                    arrResult[i + 3] is SensorZoneHistory &&
                    arrResult[i + 4] is Zone)
                {
                    AlarmData alarmData = new AlarmData();

                    EquipmentZone eq = arrResult[i] as EquipmentZone;
                    SensorReactionHistory srh = arrResult[i + 1] as SensorReactionHistory;
                    SensorZone sz = arrResult[i + 2] as SensorZone;
                    SensorZoneHistory szh = arrResult[i + 3] as SensorZoneHistory;
                    Zone z = arrResult[i + 4] as Zone;

                    //allSensorZoneIDs.AddRange(szh.AllSensorZoneIDs);

                    int sensorZoneID;
                    int.TryParse(srh.Param2, out sensorZoneID);
                    int isAlarm;
                    int.TryParse(srh.Param4, out isAlarm); // ReactionType이 62(CHANGE_ALARM_DEPTH) 일 때 

                    // 알람 해제
                    if (srh.ReactionType == SensorReactionHistory.ReactionTypes.MALFUNCTION ||
                        srh.ReactionType == SensorReactionHistory.ReactionTypes.END_STATUS ||
                        srh.ReactionType == SensorReactionHistory.ReactionTypes.USER_RESET)
                    {
                        SensorZoneKey key = new SensorZoneKey();
                        key.SensorZoneHistoryID = srh.SensorZoneHistoryID;
                        key.SensorZoneID = sensorZoneID;

                        if (srh.ReactionType == SensorReactionHistory.ReactionTypes.CHANGE_ALARM_DEPTH)
                            key.ReactionType = SensorReactionHistory.ReactionTypes.END_STATUS;
                        else
                            key.ReactionType = srh.ReactionType;

                        endTypes.Add(key);
                    }
                    // 알람 단계 변경
                    else if (srh.ReactionType == SensorReactionHistory.ReactionTypes.CHANGE_ALARM_DEPTH)
                    {
                        for (int j = 0; j < allAlarmDatas.Count; j++)
                        {
                            if (allAlarmDatas[j].SensorZoneHistoryID == szh.ID)
                            {
                                int alarmDepth;
                                if (int.TryParse(srh.Param5, out alarmDepth))
                                    allAlarmDatas[j].AlarmDepth = alarmDepth;
                            }

                            for (int c = 0; c < currentAlarmsCount; c++)
                            {
                                if (allAlarmDatas[j].SensorZoneHistoryID == currentAlarms[c].SensorZoneHistoryID)
                                {
                                    allAlarmDatas[j].AlarmSensorZoneIDs = currentAlarms[c].AlarmSensorZoneIDs;
                                    break;
                                }
                            }
                        }
                    }
                    else // 추가                     
                    {
                        alarmData.dtTime = srh.Time;
                        alarmData.StrDateTime = srh.Time.ToString("yyyy-MM-dd HH:mm:ss");
                        alarmData.OrgSensorID = sz.OrgSensorID;
                        alarmData.SensorZoneID = sensorZoneID;
                        alarmData.SensorZoneHistoryID = szh.ID;
                        //alarmData.BuildingName = b.DisplayText;
                        alarmData.ZoneName = z.DisplayText;
                        alarmData.PositionName = eq.DisplayText;
                        alarmData.ZoneID = z.ID;
                        alarmData.EquipZoneID = eq.ID;
                        alarmData.FacilityType = Facility.ToFacilityType(szh.SensorType);
                        if (sz.ID >= dnsSopID.Header.ManualReportDefaultID)
                            alarmData.FacilityTypeString = "수동신고";
                        else
                        {
                            alarmData.FacilityTypeString = Facility.GetNFacilityTypeString(szh.SensorType);

                            Material material = getSensorMaterialType(alarmData.FacilityType, (int)alarmData.OrgSensorID, out strErrorMessage);
                            if (material != null)
                            {
                                alarmData.MaterialType = material.ID;
                                alarmData.MaterialTypeString = material.MaterialName;
                            }
                        }                            
                        alarmData.Message = srh.Message;

                        

                        int alarmDepth;
                        if (int.TryParse(srh.Param5, out alarmDepth))
                            alarmData.AlarmDepth = alarmDepth;

                        alarmData.AlarmSensorZoneIDs = szh.AllSensorZoneIDs;
                        for (int j = 0; j < currentAlarmsCount; j++)
                        {
                            if (currentAlarms[j].SensorZoneHistoryID == szh.ID)
                            {
                                alarmData.SopStatus = currentAlarms[j].SopStatus;
                                alarmData.AlarmSensorZoneIDs = currentAlarms[j].AlarmSensorZoneIDs;
                                break;
                            }
                        }

                        // 수동신고
                        if (sz.ID >= dnsSopID.Header.ManualReportDefaultID)
                        {
                            alarmData.ReportPerson = srh.Param3;
                            alarmData.Memo = srh.Param4;
                        }

                        allAlarmDatas.Add(alarmData);
                    }

                    m_nMaxReactionHistoryID = Math.Max(m_nMaxReactionHistoryID, srh.ID);
                }
            }

            #region 알람 종료 방식 지정
            int alarmDataCount = allAlarmDatas.Count;
            foreach (SensorZoneKey key in endTypes)
            {
                for (int i = 0; i < alarmDataCount; i++)
                {
                    if (key.SensorZoneHistoryID == allAlarmDatas[i].SensorZoneHistoryID && allAlarmDatas[i].AlarmSensorZoneIDs.Contains(key.SensorZoneID))
                    {
                        allAlarmDatas[i].IsAlarm = false;
                        switch (key.ReactionType)
                        {
                            case SensorReactionHistory.ReactionTypes.END_STATUS:
                                allAlarmDatas[i].ReleaseInfo = "현장 종료";
                                break;
                            case SensorReactionHistory.ReactionTypes.MALFUNCTION:
                                allAlarmDatas[i].ReleaseInfo = "오작동 처리";
                                break;
                            case SensorReactionHistory.ReactionTypes.USER_RESET:
                                allAlarmDatas[i].ReleaseInfo = "사용자 종료";
                                break;
                        }
                        break;
                    }
                }
            }
            #endregion

            m_allAlarmDatas = allAlarmDatas.OrderByDescending(p => p.IsAlarm == true).ThenByDescending(p => p.dtTime).ThenByDescending(p => p.SensorZoneHistoryID).ToList();

            m_bTimerRunning = false;
        }

        /// <summary>
        /// 해당 센서의 MaterialType
        /// </summary>
        private Material getSensorMaterialType(Facility.FacilityType type, int nOrgSensorID, out string strErrorMessage)
        {
            Material materialType = null;
            strErrorMessage = "";

            if (Facility.IsPSMSensorType(type))
            {
                Dictionary<PSM.Fields, object> dicConditions = new Dictionary<PSM.Fields, object>();
                dicConditions[PSM.Fields.ID] = nOrgSensorID;

                string strAdditionalConditions = "";

                List<PSM> psmSensors = m_dataManager.GetSelectManager().SelectPSMSensors(dicConditions, strAdditionalConditions, out strErrorMessage);

                if (psmSensors == null || psmSensors.Count == 0)
                    return materialType;

                PSM psm = psmSensors[0];

                if (psm.MaterialType != null && m_dicMaterials.ContainsKey((int)psm.MaterialType))
                    materialType = m_dicMaterials[(int)psm.MaterialType];
            } 
            else if (Facility.IsETCSensorType(type))
            {
                Dictionary<ETC.Fields, object> dicConditions = new Dictionary<ETC.Fields, object>();
                dicConditions[ETC.Fields.ID] = nOrgSensorID;

                string strAdditionalConditions = "";

                List<ETC> etcSensors = m_dataManager.GetSelectManager().SelectETCSensors(dicConditions, strAdditionalConditions, out strErrorMessage);

                if (etcSensors == null || etcSensors.Count == 0)
                    return materialType;

                ETC etc = etcSensors[0];

                if (etc.MaterialType != null && m_dicMaterials.ContainsKey((int)etc.MaterialType))
                    materialType = m_dicMaterials[(int)etc.MaterialType];
            }

            return materialType;
        }

        /// <summary>
        /// 현재 발생중인 알람 감시 타이머
        /// </summary>
        private void timerLoadCurrentAlarm_Elapsed2(object sender, ElapsedEventArgs e)
        {
            //if (m_bTimerRunning)
            //    return;

            //m_bTimerRunning = true;

            //string strErrorMessage = null;
            //List<AlarmData> listAlarmData = new List<AlarmData>();

            //// CurrentAlarm 감시
            //ArrayList arrResult = m_dataManager.GetSelectManager().JoinCurrentAlarmHistory(null, out strErrorMessage);
            //if (arrResult == null)
            //{
            //    m_bTimerRunning = false;
            //    return;
            //}

            //bool checkChanged = CheckAlarmChanged(arrResult);
            //if (!checkChanged)
            //{
            //    m_bTimerRunning = false;
            //    return;
            //}

            //m_lastCurrentAlarms = arrResult;

            //System.Diagnostics.Trace.WriteLine("[" + DateTime.Now.ToString("yyyy -MM-dd HH:mm:ss") + "] AlarmData 변경");

            //int alarmCount = arrResult.Count;
            //for (int i = 0; i < alarmCount; i += 3)
            //{
            //    if (arrResult[i] is CurrentAlarm && arrResult[i + 1] is SensorZoneHistory && arrResult[i + 2] is SensorReactionHistory)
            //    {
            //        CurrentAlarm currentAlarm = (CurrentAlarm)arrResult[i];
            //        SensorZoneHistory sensorZoneHistory = (SensorZoneHistory)arrResult[i + 1];
            //        SensorReactionHistory reactionHistory = (SensorReactionHistory)arrResult[i + 2];

            //        AlarmData alarmData = new AlarmData();
            //        alarmData.dtTime = currentAlarm.TimeStamp;
            //        alarmData.StrDateTime = currentAlarm.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
            //        alarmData.FacilityType = (Facility.FacilityType)currentAlarm.SensorType;
            //        alarmData.FacilityTypeString = Facility.GetFacilityTypeString(alarmData.FacilityType);
            //        alarmData.AlarmDepth = currentAlarm.AlarmDepth;
            //        alarmData.SopStatus = currentAlarm.SopStatus;
            //        alarmData.Message = reactionHistory.Message;

            //        //alarmData.OrgSensorID = sensorZone.OrgSensorID;
            //        //alarmData.SensorZoneID = sensorZone.ID;
            //        //alarmData.PositionName = equipmentZone.ZoneName;
            //        //alarmData.EquipZoneID = equipmentZone.ID;
            //        //alarmData.BuildingName = building.DisplayText;
            //        //alarmData.ZoneID = zone.ID;
            //        //alarmData.ZoneName = zone.ZoneName;
            //        //alarmData.SopStatus = alarm.SopStatus;
            //        //alarmData.SensorZoneHistoryID = alarm.SensorZoneHistoryID;
            //        //alarmData.AlarmDepth = alarm.AlarmDepth;
            //        //alarmData.AlarmSensorZoneIDs = alarm.AlarmSensorZoneIDs;

            //        listAlarmData.Add(alarmData);
            //    }
            //}

            //m_allAlarmDatas = listAlarmData;

            return;
        }

        /// <summary>
        /// 오늘 발생한 알람 정보
        /// </summary>
        public ResponseTodayAlarmData GetTodayAlarmData()
        {
            ResponseTodayAlarmData result = new ResponseTodayAlarmData();
            List<AlarmData> alarmDatas = new List<AlarmData>();

            string strErrorMessage = null;

            DateTime now = DateTime.Now;
            string strToday = now.ToString("yyyy-MM-dd") + " 00:00:00";
            string strCondition = string.Format("{0}.{1} >= '{2}' and {0}.{3} < {4}"
                , SensorZoneHistory.TableName, SensorZoneHistory.Fields.Time, strToday, SensorZoneHistory.Fields.SensorZoneID, dnsSopID.Header.ManualReportDefaultID);//SensorZoneID

            ArrayList arrResult = m_dataManager.GetSelectManager().JoinSensorZoneSensorZoneHistory(strCondition, out strErrorMessage);
            
            if (arrResult == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
            }

            int nResultCount = arrResult.Count;

            if (arrResult.Count == 0)
            {
                result.AlarmDatas = alarmDatas;
                result.Success = true;
                return result;
            }

            for (int i = 0; i < nResultCount; i += 2)
            {
                SensorZone sensorZone = arrResult[i] as SensorZone;
                SensorZoneHistory sensorZoneHistory = arrResult[i + 1] as SensorZoneHistory;

                AlarmData alarmData = new AlarmData();
                alarmData.dtTime = sensorZoneHistory.Time;
                alarmData.SensorZoneID = sensorZone.ID;
                alarmData.FacilityType = (Facility.FacilityType)sensorZoneHistory.SensorType;
                alarmData.OrgSensorID = sensorZone.OrgSensorID;
                alarmData.ZoneID = sensorZoneHistory.ZoneID;
                alarmData.EquipZoneID = sensorZone.EquipZoneID;

                alarmDatas.Add(alarmData);
            }

            result.AlarmDatas = alarmDatas;
            result.Success = true;
            return result;
        }

        private List<AlarmData> CopyAlarmDatas()
        {
            if (m_alarmDatas == null)
                return null;

            List<AlarmData> datas = new List<AlarmData>();
            foreach (AlarmData item in m_alarmDatas)
            {
                datas.Add((AlarmData)item.Clone());
            }

            return datas;
        }
        private List<AlarmData> CopyAllAlarmDatas()
        {
            if (m_allAlarmDatas == null)
                return null;

            List<AlarmData> datas = new List<AlarmData>();
            foreach (AlarmData item in m_allAlarmDatas)
            {
                datas.Add((AlarmData)item.Clone());
            }

            return datas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentAlarms"></param>
        /// <returns>true: 바뀜, false: 안바뀜</returns>
        private bool CheckAlarmChanged(ArrayList arrResult)
        {
            if (m_lastCurrentAlarms == null) // 최초 조회
                return true;

            if (m_lastCurrentAlarms.Count != arrResult.Count) // 갯수 체크
                return true;

            int alarmCount = arrResult.Count;
            int lastAlarmCount = m_lastCurrentAlarms.Count;
            for (int i = 0; i < alarmCount; i+=3)
            {
                if (arrResult[i] is CurrentAlarm)
                {
                    for (int j = 0; j < lastAlarmCount; j+=3)
                    {
                        CurrentAlarm currentAlarm = (CurrentAlarm)arrResult[i];
                        CurrentAlarm lastAlarm = (CurrentAlarm)m_lastCurrentAlarms[j];
                        if (currentAlarm.SensorZoneHistoryID == lastAlarm.SensorZoneHistoryID)
                        {
                            if (currentAlarm.SensorType != lastAlarm.SensorType ||
                                currentAlarm.AlarmType != lastAlarm.AlarmType ||
                                currentAlarm.TimeStamp != lastAlarm.TimeStamp ||
                                currentAlarm.SopStatus != lastAlarm.SopStatus ||
                                currentAlarm.AlarmDepth != lastAlarm.AlarmDepth)
                                return true;
                        } 
                    }
                }
            }
            
            return false;
        }

        public ResponseBuildingGroupList RequestBuildingGroupList(List<int> siteIDs)
        {
            // 데이터 로딩이 끝날때까지 최대 5초간 대기한다.
            for (int i=0;i<5 && m_completeLoading == false;i++)
            {
                System.Threading.Thread.Sleep(1000);
            }

            ResponseBuildingGroupList response = new ResponseBuildingGroupList();
            response.BuildingGroups = new List<BuildingGroupData>();

            foreach (BuildingGroupData bg in m_spatialManager.BuildingGroups)
            {
                if (siteIDs != null)
                {
                    if (siteIDs.Contains(bg.SiteID) == false)
                        continue;
                }

                response.BuildingGroups.Add(bg);
            }

            List<ZoneData> outdoorZones = m_spatialManager.GetOutdoorZones();

            foreach (ZoneData zone in outdoorZones)
            {
                if (siteIDs != null)
                {
                    if (siteIDs.Contains(zone.SiteID) == false)
                        continue;
                }

                response.OutdoorZones.Add(zone);
            }

            response.Success = true;
            return response;
        }

        public ResponseBuildingGroupList RequestOuterDatas()
        {
            string strErrorMessage;
            ResponseBuildingGroupList response = new ResponseBuildingGroupList();

            if (m_spatialManager.ReloadOuters(m_dataManager, out strErrorMessage) == false)
            {
                response.Success = false;
                response.Message = strErrorMessage == null ? "" : strErrorMessage;
            }
            else
            {
                response.BuildingGroups = new List<BuildingGroupData>();

                foreach (BuildingGroupData bg in m_spatialManager.BuildingGroups)
                {
                    response.BuildingGroups.Add(bg);
                }

                List<ZoneData> outdoorZones = m_spatialManager.GetOutdoorZones();

                foreach (ZoneData zone in outdoorZones)
                {
                    response.OutdoorZones.Add(zone);

                    List<FireSensor> fireSensors = null;
                    List<PSMSensor> psmSensors = null;
                    List<EtcSensor> etcSensors = null;
                    List<CCTVSensor> cctvSensors = null;

                    if (m_sensorManager.ReloadSensors(m_dataManager, zone.ID, out fireSensors, out psmSensors, out etcSensors, out cctvSensors, out strErrorMessage))
                    {
                        zone.Sensors = new ZoneSensors(fireSensors, psmSensors, etcSensors, cctvSensors);
                    }
                }

                response.Success = true;
            }

            return response;
        }

        public ResponseIndoorDatas RequestIndoorDatas(int nZoneID)
        {
            string strErrorMessage;
            ResponseIndoorDatas response = new ResponseIndoorDatas();

            response.ZoneID = nZoneID;

            if (m_spatialManager.ReloadIndoorZone(m_dataManager, nZoneID, out strErrorMessage) == false)
            {
                response.Success = false;
                response.Message = strErrorMessage == null ? "" : strErrorMessage;
            }
            else
            {
                ZoneData zoneData = m_spatialManager.GetZone(nZoneID);

                if (zoneData == null)
                {
                    response.Success = false;
                    response.Message = string.Format("ID {0}에 해당하는 Zone 정보를 찾을수 없습니다.", nZoneID);
                }
                else
                {
                    foreach (EquipmentZone equipZone in zoneData.EquipmentZoneDatas)
                    {
                        response.EquipZones.Add(equipZone);
                    }

                    List<FireSensor> fireSensors = null;
                    List<PSMSensor> psmSensors = null;
                    List<EtcSensor> etcSensors = null;
                    List<CCTVSensor> cctvSensors = null;

                    if (m_sensorManager.ReloadSensors(m_dataManager, nZoneID, out fireSensors, out psmSensors, out etcSensors, out cctvSensors, out strErrorMessage) == false)
                    {
                        response.Success = false;
                        response.Message = strErrorMessage;
                    }
                    else
                    {
                        response.Success = true;
                        response.FireSensors = fireSensors;
                        response.PSMSensors = psmSensors;
                        response.EtcSensors = etcSensors;
                        response.Cctvs = cctvSensors;
                    }
                }
            }

            return response;
        }

        /*public ResponseGltfDataList RequestGltfModelList(int nUserID)
        {
            string strErrorMessage;
            ICollection<GltfModel> models = GltfManager.LoadGltfModels(m_dataManager, out strErrorMessage);

            if (models == null)
                return MakeResponseGltfDataList(null, null, strErrorMessage);

            // 계정에 따른 고,저용량 3D 모델 옵션 구하기
            string str3DHighVer = "true";

            Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object> dicConditions = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            dicConditions[SOPManager.Model.Sop.Account.Option.Fields.UserID] = nUserID;
            dicConditions[SOPManager.Model.Sop.Account.Option.Fields.Category] = "SDMS";
            dicConditions[SOPManager.Model.Sop.Account.Option.Fields.SubCategory] = "3DHighVer";

            List<SOPManager.Model.Sop.Account.Option> accountOptions = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicConditions, out strErrorMessage);

            if (accountOptions != null && accountOptions.Count > 0)
            {
                str3DHighVer = accountOptions[0].PropertyValue1;
            }

            GltfOption option = GltfManager.LoadGltfOption(m_processManager.CommonDataManager, str3DHighVer, out strErrorMessage);

            if (option == null)
                return MakeResponseGltfDataList(models, null, strErrorMessage);

            return MakeResponseGltfDataList(models, option, "");
        }*/

        public ResponseGltfDataList RequestGltfModelList(int nUserID, List<int> siteIDs/*, string strRootPath*/)
        {
            string strErrorMessage;
            ICollection<GltfModel> models = GltfManager.LoadGltfModels(m_dataManager, siteIDs, out strErrorMessage);

            if (models == null)
                return MakeResponseGltfDataList(null, null/*, strRootPath*/, strErrorMessage);

            // 계정에 따른 고,저용량 3D 모델 옵션 구하기
            string str3DHighVer = "true";

            Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object> dicConditions = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            dicConditions[SOPManager.Model.Sop.Account.Option.Fields.UserID] = nUserID;
            dicConditions[SOPManager.Model.Sop.Account.Option.Fields.Category] = "SDMS";
            dicConditions[SOPManager.Model.Sop.Account.Option.Fields.SubCategory] = "3DHighVer";

            List<SOPManager.Model.Sop.Account.Option> accountOptions = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicConditions, out strErrorMessage);

            if (accountOptions != null && accountOptions.Count > 0)
            {
                str3DHighVer = accountOptions[0].PropertyValue1;
            }

            GltfOption option = GltfManager.LoadGltfOption(m_processManager.CommonDataManager, str3DHighVer, out strErrorMessage);

            if (option == null)
                return MakeResponseGltfDataList(models, null/*, strRootPath*/, strErrorMessage);

            return MakeResponseGltfDataList(models, option/*, strRootPath*/, "");
        }

        private ResponseGltfDataList MakeResponseGltfDataList(ICollection<GltfModel> models, GltfOption option/*, string strRootPath*/, string strMessage)
        {
            ResponseGltfDataList response = new ResponseGltfDataList();

            if (models == null || option == null)
            {
                response.Success = false;
            }
            else
            {
                response.Success = true;

                //string strRootResourceFolder = GetRootResourcePath(option, strRootPath);
                response.Models = new List<GltfModel>();

                foreach (GltfModel model in models)
                {
                    if (model.ParentID == null)
                        response.Models.Add(model);

                    /*foreach (Model.GLTF.ModelData modelData in model.ModelDatas)
                    {
                        CheckModelFileName(modelData, strRootResourceFolder);
                    }*/
                }

                response.GltfOption = option;
            }

            response.Message = strMessage;
            return response;
        }

        private string GetRootResourcePath(GltfOption option, string strRootPath)
        {
            string strRootResourceFolder = Directory.GetCurrentDirectory();

            if (strRootResourceFolder.EndsWith("\\"))
            {
                if (strRootPath.StartsWith("\\"))
                    strRootResourceFolder += strRootPath.Substring(1).Trim();
                else
                    strRootResourceFolder += strRootPath.Trim();
            }
            else
            {
                if (strRootPath.StartsWith("\\"))
                    strRootResourceFolder += strRootPath.Trim();
                else
                    strRootResourceFolder += "\\" + strRootPath.Trim();
            }

            string str3DModelBaseURL = option._3DModelBaseURL.Trim().Replace('/', '\\');

            if (str3DModelBaseURL.Length > 0)
            {
                if (strRootResourceFolder.EndsWith("\\"))
                {
                    if (str3DModelBaseURL.StartsWith("\\"))
                        strRootResourceFolder += str3DModelBaseURL.Substring(1).Trim();
                    else
                        strRootResourceFolder += str3DModelBaseURL.Trim();
                }
                else
                {
                    if (str3DModelBaseURL.StartsWith("\\"))
                        strRootResourceFolder += str3DModelBaseURL.Trim();
                    else
                        strRootResourceFolder += "\\" + str3DModelBaseURL.Trim();
                }
            }

            return strRootResourceFolder;
        }

        // Model이 하나의 파일이 아니라 다수의 파일로 구성되어 있는지 확인한다.
        private void CheckModelFileName(Model.GLTF.ModelData modelData, string strRootResourceFolder)
        {
            if (modelData.ModelFile == null || modelData.ModelFile.Length == 0)
                return;

            int nIndex = modelData.ModelFile.IndexOf('*');

            if (nIndex >= 0)
            {
                string strModelFile = modelData.ModelFile.Trim().Replace('/', '\\');
                string strFilePath = strModelFile.StartsWith("\\") ? strRootResourceFolder + strModelFile : strRootResourceFolder + "\\" + strModelFile;

                int nIndex2 = strFilePath.LastIndexOf('\\');

                if (nIndex2 < 0)
                    return;

                string strFolder = strFilePath.Substring(0, nIndex2);
                string strPattern = strFilePath.Substring(nIndex2 + 1);

                string[] files = Directory.GetFiles(strFolder, strPattern);

                if (files.Count() > 0)
                {
                    int nBeginIndex = strFolder.Length + 1;

                    int nIndex3 = modelData.ModelFile.LastIndexOf('/');
                    string strFileTag = nIndex3 < 0 ? "" : modelData.ModelFile.Substring(0, nIndex3 + 1);

                    string strFileList = "";

                    foreach (string strFile in files)
                    {
                        string strFileName = strFile.Substring(nBeginIndex);

                        if (strFileList.Length == 0)
                            strFileList = strFileTag + strFileName;
                        else
                            strFileList += ";" + strFileTag + strFileName;
                    }

                    if (strFileList.Length > 0)
                        modelData.ModelFile = strFileList;
                }
            }
        }

        public ResponseSensorList GetSensorList(RequestSensorList request)
        {
            /*if (CheckCompleteLoading() == false)
                return MakeResponseSensorList(request, null, null, null, null, "센서정보를 읽어올수 없습니다.");*/
            SensorManager sensorManager = new SensorManager();
            
            if (sensorManager.LoadSensorList(m_dataManager, m_spatialManager) == false)
                return MakeResponseSensorList(request, null, null, null, null, "센서정보를 읽어올수 없습니다.");

            return MakeResponseSensorList(request, sensorManager.FireSensors, sensorManager.PSMSensors, sensorManager.EtcSensors, sensorManager.CCTVs, "");
        }

        private ResponseSensorList MakeResponseSensorList(RequestSensorList request, ICollection<FireSensor> fireSensors, ICollection<PSMSensor> psmSensors, ICollection<EtcSensor> etcSensors, ICollection<CCTVSensor> cctvs, string strMessage)
        {
            ResponseSensorList response = new ResponseSensorList();

            response.Success = strMessage == null || strMessage.Length == 0;
            response.Message = strMessage;

            if (request.RequestFireSensors)
                response.FireSensors = MakeList<FireSensor>(fireSensors);

            if (request.RequestPSMSensors)
                response.PSMSensors = MakeList<PSMSensor>(psmSensors);

            if (request.RequestEtcSensors)
                response.EtcSensors = MakeList<EtcSensor>(etcSensors);

            if (request.RequestCCTVs)
                response.Cctvs = MakeList<CCTVSensor>(cctvs);

            return response;
        }

        private List<DataType> MakeList<DataType>(ICollection<DataType> datas)
        {
            if (datas == null)
                return null;

            List<DataType> dataList = new List<DataType>();

            foreach (DataType data in datas)
            {
                dataList.Add(data);
            }

            return dataList;
        }

        private bool CheckCompleteLoading()
        {
            if (m_completeLoading)
                return true;

            for (int i=0;i<5;i++)
            {
                System.Threading.Thread.Sleep(1000);

                if (m_completeLoading)
                    return true;
            }

            return false;
        }

        public MessageResult MoveSensor(RequestMoveSensor request)
        {
            string strErrorMessage;
            bool result = m_sensorManager.MoveSensor(m_dataManager, request.SensorType, request.SensorID, request.X, request.Z, out strErrorMessage);

            MessageResult response = new MessageResult(result, strErrorMessage);
            return response;
        }

        public ResponseEquipZoneCCTV GetEquipZoneCCTV(int nEquipZoneID)
        {
            ResponseEquipZoneCCTV response = new ResponseEquipZoneCCTV();
            string strErrorMessage;

            Dictionary<EquipZoneCCTV.Fields, object> dicCondition = new Dictionary<EquipZoneCCTV.Fields, object>();
            dicCondition.Add(EquipZoneCCTV.Fields.EquipZoneID, nEquipZoneID);

            List<EquipZoneCCTV> equipZoneCCTVs = m_dataManager.GetSelectManager().SelectEquipZoneCCTVs(dicCondition, "", out strErrorMessage);
            if (equipZoneCCTVs != null && equipZoneCCTVs.Count > 0)
            {
                response.EquipZoneCCTV = equipZoneCCTVs[0];

                response.Success = true;
                response.Message = strErrorMessage;
            }
            else
            {
                response.Success = false;
                response.Message = "EquipZoneCCTV 조회를 할 수 없습니다.";
            }
            return response;
        }

        public ResponseEquipZoneCCTVFromSensor GetEquipZoneCCTV(string strSensorType, int nSensorID)
        {
            ResponseEquipZoneCCTVFromSensor response = new ResponseEquipZoneCCTVFromSensor();

            string strErrorMessage;
            SensorZone sensorZone = GetSensorZoneFromSensor(strSensorType, nSensorID, out strErrorMessage);

            if (sensorZone == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            ArrayList arrDatas = m_dataManager.GetSelectManager().JoinEquipmentZoneEquipZoneCCTV(sensorZone.EquipZoneID, null, out strErrorMessage);

            if (arrDatas != null)
            {
                if (arrDatas.Count == 2 && arrDatas[0] is EquipmentZone && arrDatas[1] is EquipZoneCCTV)
                {
                    response.EquipZoneID = sensorZone.EquipZoneID;
                    response.EquipZoneCCTV = (EquipZoneCCTV)arrDatas[1];
                    response.EquipZoneDisplayName = ((EquipmentZone)arrDatas[0]).DisplayText;
                }
                else
                {
                    EquipmentZone equipZone = m_dataManager.GetSelectManager().SelectEquipmentZone(sensorZone.EquipZoneID, out strErrorMessage);

                    if (equipZone == null)
                    {
                        response.Success = false;
                        response.Message = "해당 센서에 대한 구역설정이 되어있지 않습니다.";
                        return response;
                    }

                    response.EquipZoneID = sensorZone.EquipZoneID;
                    response.EquipZoneDisplayName = equipZone.DisplayText;
                }

                response.Success = true;
                response.Message = "";
            }
            else
            {
                response.Success = false;
                response.Message = strErrorMessage;
            }

            return response;
        }

        private SensorZone GetSensorZoneFromSensor(string strSensorType, int nSensorID, out string strErrorMessage)
        {
            strErrorMessage = null;

            int nSensorType = -1;

            if (SensorManager.IsCCTVType(strSensorType))
                nSensorType = (int)dnsData.Sensor.Facility.FacilityType.CCTV;
            else if (SensorManager.IsFireSensor(strSensorType))
                nSensorType = (int)dnsData.Sensor.Facility.FacilityType.FIRE_SENSOR;
            else if (SensorManager.IsPSMSensor(strSensorType))
                nSensorType = (int)dnsData.Sensor.Facility.FacilityType.PSM_SENSOR;
            else if (SensorManager.IsEtcSensor(strSensorType))
                nSensorType = (int)dnsData.Sensor.Facility.FacilityType.ETC;
            else
            {
                strErrorMessage = "알수없는 형식의 SensorType입니다. : " + strSensorType;
                return null;
            }

            string strAdditionalConditions = null;

            Dictionary<SensorZone.Fields, object> dicConditions = new Dictionary<SensorZone.Fields, object>();
            dicConditions[SensorZone.Fields.OrgSensorID] = nSensorID;

            if (nSensorType == (int)dnsData.Sensor.Facility.FacilityType.PSM_SENSOR)
            {
                strAdditionalConditions = string.Format("{0} in ({1})", SensorZone.Fields.SensorType, string.Join(",", Facility.GetPSMTypeAllNumberToList()));
            }
            else if (nSensorType == (int)dnsData.Sensor.Facility.FacilityType.ETC)
            {
                strAdditionalConditions = string.Format("{0} in ({1})", SensorZone.Fields.SensorType, string.Join(",", Facility.GetETCTypeAllNumberToList()));
            }
            else if (nSensorType == (int)dnsData.Sensor.Facility.FacilityType.CCTV)
            {
                List<int> ids = Facility.GetSVMSTypeAllNumberToList();
                ids.Add(nSensorType);
                strAdditionalConditions = string.Format("{0} in ({1})", SensorZone.Fields.SensorType, string.Join(",", ids));
            }
            else
                dicConditions[SensorZone.Fields.SensorType] = nSensorType;


            List<SensorZone> sensorZones = m_dataManager.GetSelectManager().SelectSensorZones(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (sensorZones == null)
                return null;

            if (sensorZones.Count == 0)
            {
                strErrorMessage = "해당 센서에 대한 구역설정이 되어있지 않습니다.";
                return null;
            }

            return sensorZones[0];
        }

        public ArrayList GetOrgSensorID(int sensorZoneID)
        {
            string strErrorMessage = null;
            SensorZone sensorZone = m_dataManager.GetSelectManager().SelectSensorZone(sensorZoneID, out strErrorMessage);
            if (sensorZone == null)
                return null;

            ArrayList arrResult = new ArrayList();
            arrResult.Add(sensorZone.OrgSensorID);
            arrResult.Add(sensorZone.IsAlarmStatus);

            return arrResult;
        }

        public ResponseSensorCount GetSensorCount()
        {
            ResponseSensorCount response = new ResponseSensorCount();

            response.FireSensorCount = m_sensorManager.FireSensors.Count;
            response.DisabledFireSensorCount = m_sensorManager.DisabledFireSensors.Count;
            response.PsmSensorCount = m_sensorManager.PSMSensors.Count;
            response.DisabledPSMSensorCount = m_sensorManager.DisabledPSMSensors.Count;
            response.EtcSensorCount = m_sensorManager.EtcSensors.Count;
            response.DisabledEtcSensorCount = m_sensorManager.DisabledEtcSensors.Count;
            response.CctvCount = m_sensorManager.CCTVs.Count;
            response.DisabledCCTVCount = m_sensorManager.DisabledCCTVs.Count;

            response.Success = true;
            return response;
        }

        public ResponseFacilityType GetFacilityType(int nFacilityTypeID)
        {
            ResponseFacilityType response = new ResponseFacilityType();
            string strErrorMessage;

            Dictionary<FacilityType.Fields, object> dicCondition = new Dictionary<FacilityType.Fields, object>();
            dicCondition.Add(FacilityType.Fields.ID, nFacilityTypeID);

            FacilityType facilityType = m_dataManager.GetSelectManager().SelectFacilityType(nFacilityTypeID, out strErrorMessage);
            if (facilityType != null)
            {
                response.FacilityType = facilityType;

                response.Success = true;
                response.Message = strErrorMessage;
            }
            else
            {
                response.Success = false;
                response.Message = "facilityType 조회를 할 수 없습니다.";
            }
            return response;
        }

        public ResponseFacilityInfoData GetFacilityInfoDatas(string strModelName)
        {
            ResponseFacilityInfoData response = new ResponseFacilityInfoData();
            string strErrorMessage;

            Info info = m_dataManager.GetSelectManager().SelectFacilityInfo(strModelName, out strErrorMessage);

            if (strErrorMessage != null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }
            else if (info == null)
            {
                response.Success = false;
                response.Message = string.Format("{0}에 해당하는 설비 데이터가 존재하지 않습니다.", strModelName);
                return response;
            }

            Dictionary<InfoData.Fields, object> dicCondition = new Dictionary<InfoData.Fields, object>();
            dicCondition[InfoData.Fields.FacilityInfoID] = info.ID;

            List<InfoData> datas = m_dataManager.GetSelectManager().SelectFacilityInfoDatas(dicCondition, null, out strErrorMessage);

            if (datas == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            datas.Sort();

            response.Success = true;
            response.ModelName = strModelName;
            response.FacilityName = info.FacilityName;
            response.Datas.AddRange(datas);

            return response;
        }

        public ResponseAllFacilityInfo GetAllFacilityInfos()
        {
            ResponseAllFacilityInfo response = new ResponseAllFacilityInfo();
            string strErrorMessage;

            List<Info> infos = m_dataManager.GetSelectManager().SelectFacilityInfos(null, null, out strErrorMessage);

            if (strErrorMessage != null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }
            else if (infos == null)
            {
                response.Success = false;
                response.Message = string.Format("설비 데이터가 존재하지 않습니다.");
                return response;
            }

            response.Success = true;
            response.Infos = infos;

            return response;
        }

        public ResponseBuildingData GetBuildingDatas(string strBuildingName)
        {
            ResponseBuildingData response = new ResponseBuildingData();
            string strErrorMessage;

            Dictionary<Building.Fields, object> dicConditions = new Dictionary<Building.Fields, object>();
            dicConditions[Building.Fields.BuildingName] = strBuildingName;

            List<Building> buildings = m_dataManager.GetSelectManager().SelectBuildings(dicConditions, null, out strErrorMessage);

            if (strErrorMessage != null || buildings == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }
            else if (buildings.Count == 0)
            {
                response.Success = false;
                response.Message = string.Format("{0}에 해당하는 건물 정보가 존재하지 않습니다.", strBuildingName);
                return response;
            }

            Building building = buildings[0];

            Dictionary<Model.Spatial.BuildingData.Fields, object> dicCondition2 = new Dictionary<Model.Spatial.BuildingData.Fields, object>();
            dicCondition2[Model.Spatial.BuildingData.Fields.BuildingID] = building.ID;

            List<Model.Spatial.BuildingData> datas = m_dataManager.GetSelectManager().SelectBuildingDatas(dicCondition2, null, out strErrorMessage);

            if (datas == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            datas.Sort();

            response.Success = true;
            response.DisplayText = building.DisplayText;
            response.Datas.AddRange(datas);

            return response;
        }

        public ResponseBuildingGroupData GetBuildingGroupDatas(int nBuildingGroupID)
        {
            ResponseBuildingGroupData response = new ResponseBuildingGroupData();
            string strErrorMessage;

            BuildingGroup buildingGroup = m_dataManager.GetSelectManager().SelectBuildingGroup(nBuildingGroupID, out strErrorMessage);

            if (strErrorMessage != null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }
            else if (buildingGroup == null)
            {
                response.Success = false;
                response.Message = string.Format("ID {0}에 해당하는 건물그룹 정보가 존재하지 않습니다.", nBuildingGroupID);
                return response;
            }

            Dictionary<Model.Spatial.BuildingGroupData.Fields, object> dicConditions = new Dictionary<Model.Spatial.BuildingGroupData.Fields, object>();
            dicConditions[Model.Spatial.BuildingGroupData.Fields.BuildingGroupID] = nBuildingGroupID;

            List<Model.Spatial.BuildingGroupData> datas = m_dataManager.GetSelectManager().SelectBuildingGroupDatas(dicConditions, null, out strErrorMessage);

            if (datas == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            datas.Sort();

            response.Success = true;
            response.DisplayText = buildingGroup.DisplayText;
            response.Datas.AddRange(datas);

            return response;
        }

        public ResponseFakeWalls GetFakeWalls(int nZoneID)
        {
            return FakeWallManager.GetFakeWalls(m_processManager, m_dataManager, nZoneID);
        }

        public ResponseUpdateFakeWall UpdateFakeWall(RequestUpdateFakeWall request)
        {
            return FakeWallManager.UpdateFakeWall(m_processManager, m_dataManager, request);
        }

        public ResponseUpdateFakeWalls UpdateFakeWalls(RequestUpdateFakeWalls request)
        {
            return FakeWallManager.UpdateFakeWalls(m_processManager, m_dataManager, request);
        }

        //GetSpatialSensorCount
        public ResponseUseSensor GetUseSensor()
        {
            ResponseUseSensor response = new ResponseUseSensor();

            ICollection<FireSensor> fireSensors = m_sensorManager.FireSensors;
            ICollection<Fire> disabledFireSensors = m_sensorManager.DisabledFireSensors;
            ICollection<PSMSensor> psmSensors = m_sensorManager.PSMSensors;
            ICollection<PSM> disabledPSMSensors = m_sensorManager.DisabledPSMSensors;
            ICollection<EtcSensor> etcSensors = m_sensorManager.EtcSensors;
            ICollection<ETC> disabledEtcSensors = m_sensorManager.DisabledEtcSensors;
            ICollection<CCTVSensor> cctvSensors = m_sensorManager.CCTVs;
            ICollection<CCTV> disabledCCTVs = m_sensorManager.DisabledCCTVs;

            response.FireSensors = fireSensors;
            response.DisabledFireSensors = disabledFireSensors;
            response.PsmSensors = psmSensors;
            response.DisabledPSMSensors = disabledPSMSensors;
            response.EtcSensors = etcSensors;
            response.DisabledEtcSensors = disabledEtcSensors;
            response.CCTVs = cctvSensors;
            response.DisabledCCTVs = disabledCCTVs;

            response.Success = true;
            return response;
        }

        public ResponseNewCCTVList GetNewCCTVList()
        {
            string strErrorMessage;
            List<CCTV> cctvList = CCTVManager.GetNewCCTVList(m_dataManager, out strErrorMessage);

            if (cctvList == null)
                return new ResponseNewCCTVList(false, strErrorMessage);

            ResponseNewCCTVList response = new ResponseNewCCTVList(true, "");
            response.CCTVs = cctvList;
            return response;
        }

        public ResponseWeeklyStatus GetWeeklyStatus()
        {
            ResponseWeeklyStatus response = new ResponseWeeklyStatus();
            string strErrorMessage;

            //Dictionary<SensorZoneHistory.Fields, object> dicCondition = new Dictionary<SensorZoneHistory.Fields, object>();

            DateTime now = DateTime.Now;
            string strStart = now.AddDays(-7).ToString("yyyy-MM-dd") + " 00:00:00";
            string strEnd = now.ToString("yyyy-MM-dd") + " 00:00:00";
            //string strAdditionalConditions = "Time >= '" + strStart + "' AND Time <= '" + strEnd + "'";
            string strAdditionalConditions = string.Format("{0}.{1} >= '{2}' AND {0}.{1} <= '{3}'", SensorZoneHistory.TableName, SensorZoneHistory.Fields.Time, strStart, strEnd);

            //List<SensorZoneHistory> sensorZoneHistorys = m_dataManager.GetSelectManager().SelectSensorZoneHistories(dicCondition, strAdditionalConditions, out strErrorMessage);

            //if (sensorZoneHistorys != null && sensorZoneHistorys.Count > 0)
            //{
            //    response.SensorZoneHistorys = sensorZoneHistorys;
            //    response.Success = true;
            //    response.Message = strErrorMessage;
            //}
            //else
            //{
            //    response.Success = false;
            //    response.Message = "SensorZoneHistory 조회를 할 수 없습니다.";
            //}

            ArrayList arrResult = m_dataManager.GetSelectManager().JoinSensorZoneHistorySensorZoneZoneBuildingBuildingGroup(strAdditionalConditions, out strErrorMessage);


            if (arrResult == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            int nResultCount = arrResult.Count;
            List<AlarmInfo> alarmInfos = new List<AlarmInfo>();

            if (arrResult.Count == 0)
            {
                response.Success = true;
                response.AlarmInfos = alarmInfos;
                response.Message = "";
            }

            for (int i = 0; arrResult.Count - 4 > i; i += 5)
            {
                SensorZoneHistory sensorZoneHistory = arrResult[i] as SensorZoneHistory;
                SensorZone sensorZone = arrResult[i + 1] as SensorZone;
                Zone zone = arrResult[i + 2] as Zone;
                Building building = arrResult[i + 3] as Building;
                BuildingGroup buildingGroup = arrResult[i + 4] as BuildingGroup;

                AlarmInfo alarmInfo = new AlarmInfo();
                alarmInfo.Time = sensorZoneHistory.Time;
                alarmInfo.OrgSensorID = sensorZone.OrgSensorID;
                alarmInfo.FacilityType = sensorZoneHistory.SensorType;
                alarmInfo.SensorZoneID = sensorZone.ID;
                alarmInfo.ZoneID = zone.ID;
                alarmInfo.BuildingID = building.ID;
                alarmInfo.BuildingGroupID = buildingGroup.ID;

                Material material = GetMaterialType(alarmInfo.OrgSensorID, alarmInfo.FacilityType, out strErrorMessage);
                if (material != null)
                    alarmInfo.MaterialType = material.ID;

                alarmInfos.Add(alarmInfo);
            }

            response.Success = true;
            response.AlarmInfos = alarmInfos;
            return response;
        }

        private Material GetMaterialType(int? nOrgSensorID, int nFacilityType, out string strErrorMessage)
        {
            Material materialType = null;
            ArrayList arrResult = null;
            strErrorMessage = "";

            if (nOrgSensorID == null)
                return materialType;

            if (Facility.IsPSMSensorType(Facility.ToFacilityType(nFacilityType))) {
                string strAdditionalConditions = string.Format("{0}.{1} = {2}", PSM.TableName, PSM.Fields.ID, nOrgSensorID);
                arrResult = m_dataManager.GetSelectManager().JoinPSMSensorMaterial(strAdditionalConditions, out strErrorMessage);

                if (arrResult != null && arrResult.Count != 0)
                {
                    PSM psm = arrResult[0] as PSM;
                    Material material = arrResult[1] as Material;

                    materialType = material;
                }
            } 
            else if (Facility.IsETCSensorType(Facility.ToFacilityType(nFacilityType)))
            {
                string strAdditionalConditions = string.Format("{0}.{1} = {2}", ETC.TableName, ETC.Fields.ID, nOrgSensorID);
                arrResult = m_dataManager.GetSelectManager().JoinETCSensorMaterial(strAdditionalConditions, out strErrorMessage);

                if (arrResult != null && arrResult.Count != 0)
                {
                    ETC etc = arrResult[0] as ETC;
                    Material material = arrResult[1] as Material;

                    materialType = material;
                }
            }

            return materialType;
        }

        public ResponseSpreadMessage GetSpreadMessage()
        {
            ResponseSpreadMessage result = new ResponseSpreadMessage();
            string strErrorMessage = "";
            string strAdditionalConditions = null;

            Dictionary<SDMS.Model.Config.SpreadMessage.Fields, object> dicCondition = new Dictionary<SDMS.Model.Config.SpreadMessage.Fields, object>();

            List<SDMS.Model.Config.SpreadMessage> spreadMessages = m_processManager.SdmsDataManager.GetSelectManager().SelectSpreadMessages(dicCondition, strAdditionalConditions, out strErrorMessage);
            if (spreadMessages == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            result.SpreadMessages = spreadMessages;
            result.Success = true;
            return result;
        }

        public ResponseEquipZoneSensorList GetEquipZoneSensorList(string strSensorType, int nSensorID)
        {
            ResponseEquipZoneSensorList response = new ResponseEquipZoneSensorList();

            string strErrorMessage;
            SensorZone sensorZone = GetSensorZoneFromSensor(strSensorType, nSensorID, out strErrorMessage);

            if (sensorZone == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            Dictionary<SensorZone.Fields, object> dicConditions = new Dictionary<SensorZone.Fields, object>();
            dicConditions[SensorZone.Fields.SensorType] = sensorZone.SensorType;
            dicConditions[SensorZone.Fields.EquipZoneID] = sensorZone.EquipZoneID;

            List<SensorZone> sensorZones = m_dataManager.GetSelectManager().SelectSensorZones(dicConditions, null, out strErrorMessage);

            if (sensorZones == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            EquipmentZone equipZone = m_dataManager.GetSelectManager().SelectEquipmentZone(sensorZone.EquipZoneID, out strErrorMessage);

            if (equipZone == null)
            {
                response.Success = false;
                response.Message = "센서가 위치한 곳의 구역이름이 설정되어 있지 않습니다.";
                return response;
            }

            response.Success = true;
            response.EquipZoneID = equipZone.ID;
            response.EquipZoneName = equipZone.ZoneName;
            response.SensorType = strSensorType;

            foreach (SensorZone sz in sensorZones)
            {
                if (sz.OrgSensorID != null)
                    response.SensorIDs.Add((int)sz.OrgSensorID);
            }

            return response;
        }

        public class SensorZoneKey
        {
            private int m_nSensorZoneHistoryID = -1;
            private int m_nSensorZoneID = -1;
            private int m_nSensorType = -1;
            private int m_nZoneID = -1;
            private SensorReactionHistory.ReactionTypes m_reactionType = SensorReactionHistory.ReactionTypes.NONE;
            private List<int> m_AlarmSensorZoneIDs = new List<int>();
            
            public int SensorZoneHistoryID
            {
                get { return m_nSensorZoneHistoryID; }
                set { m_nSensorZoneHistoryID = value; }
            }

            public int SensorZoneID
            {
                get { return m_nSensorZoneID; }
                set { m_nSensorZoneID = value; }
            }

            public int SensorType
            {
                get { return m_nSensorType; }
                set { m_nSensorType = value; }
            }

            public int ZoneID
            {
                get { return m_nZoneID; }
                set { m_nZoneID = value; }
            }

            public SensorReactionHistory.ReactionTypes ReactionType
            {
                get { return m_reactionType; }
                set { m_reactionType = value; }
            }
            public List<int> AlarmSensorZoneIDs
            {
                get { return m_AlarmSensorZoneIDs; }
                set { m_AlarmSensorZoneIDs = value; }
            }
        }

        public ResponseMaterials GetMaterials()
        {

            ResponseMaterials response = new ResponseMaterials();

            List<Material> materials = new List<Material>(m_dicMaterials.Values);

            if (materials == null)
            {
                response.Success = false;
                response.Message = "Material 불러오기를 실패하였습니다.";
            }
            else
            {
                response.Success = true;
                response.Materials = materials;
            }

            return response;
        }
    }
}
