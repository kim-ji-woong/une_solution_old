using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using DBUtility;
using UnE.Sensor;
using UnE.Spatial;

namespace libSDMSReport
{
    public class ReactionSecurityManager : ReactionManager
    {
        protected override bool _GetReactionHistory(List<SensorReactionLog> arrReactionLog, string strSensorZoneHistoryIDs, Dictionary<int, SensorReactionLog> dicReactionLogs, ref int nMinReactionLogID, ref int nMaxReactionLogID)
        {
            string strSQL = "select id, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5, DetectionStatus from SensorReactionHistory ";
            strSQL += "where SensorHistoryID in (" + strSensorZoneHistoryIDs + ")";

            ArrayList arrResult = DBManager.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            DateTime dt = DateTime.Now;

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nSensorHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dt);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 4], "");
                int Param1 = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string Param2 = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");
                string Param3 = WebDBManager.GetStringField(arrResult[i + 7].ToString(), "");
                string Param4 = WebDBManager.GetStringField(arrResult[i + 8].ToString(), "");
                string Param5 = WebDBManager.GetStringField(arrResult[i + 9].ToString(), "");
                int nDetectionStatus = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                SensorReactionLog reactionLog = new SensorReactionLog();
                reactionLog.ID = nID;
                reactionLog.SensorZoneHistoryID = nSensorHistoryID;
                reactionLog.ReactionType = nReactionType;
                reactionLog.Time = time;
                reactionLog.SetDetectionResult(nDetectionStatus);
                reactionLog.Message = strMessage;

                SetReactionLogParam(reactionLog, nReactionType, Param1, Param2, Param3, Param4, Param5);

                arrReactionLog.Add(reactionLog);

                List<SensorReactionLog> arrLogs = null;

                if (m_dicHistorySensorReactionLog.ContainsKey(nSensorHistoryID))
                    arrLogs = m_dicHistorySensorReactionLog[nSensorHistoryID];
                else
                {
                    arrLogs = new List<SensorReactionLog>();
                    m_dicHistorySensorReactionLog[nSensorHistoryID] = arrLogs;
                }
                arrLogs.Add(reactionLog);

                dicReactionLogs[reactionLog.ID] = reactionLog;

                if (nMinReactionLogID < 0)
                    nMinReactionLogID = reactionLog.ID;
                else if (nMinReactionLogID > reactionLog.ID)
                    nMinReactionLogID = reactionLog.ID;

                if (nMaxReactionLogID < reactionLog.ID)
                    nMaxReactionLogID = reactionLog.ID;
            }

            return true;
        }

        protected override ReactionLog _GetReactionLog(SensorReactionLog sensorReactionLog, List<SensorReactionLog> sensorReactionLogList, int nSensorZoneHistoryID, UnE.Spatial.Zone zone)
        {
            ReactionSecurityLog reactionLog = new ReactionSecurityLog();
            reactionLog.SensorZoneHistoryID = nSensorZoneHistoryID;
            reactionLog.SensorReactionLogList = sensorReactionLogList;

            string strMemberID = "";

            if (m_dicHistoryMember.ContainsKey(nSensorZoneHistoryID))
                strMemberID = m_dicHistoryMember[nSensorZoneHistoryID];

            if (m_dicGenUserIDDNicName.ContainsKey(strMemberID))
                reactionLog.UserName = m_dicGenUserIDDNicName[strMemberID];


            //자탐은 param1
            if (ZoneManager != null)
                reactionLog.equipZone = ZoneManager.GetEquipZone(sensorReactionLog.Param1);

            if (sensorReactionLog.ReactionType == (int)libSensorProcess.ReactionType.MALFUNCTION
                || sensorReactionLog.ReactionType == (int)libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS
                || sensorReactionLog.ReactionType == (int)libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS)
            {
                int nCommanderID = -1;
                if (int.TryParse(sensorReactionLog.Param3, out nCommanderID))
                {
                    if (m_dicGenUserIDDNicName.ContainsKey(nCommanderID.ToString()))
                        reactionLog.ManagerName = m_dicGenUserIDDNicName[nCommanderID.ToString()];
                }
            }

            reactionLog.Time = sensorReactionLog.Time;
            reactionLog.SensorType = sensorReactionLog.SensorType;
            //reactionLog.ManagerName = FindManagerName(zone, IFacility.FacilityType.FIRE_SENSOR);
            reactionLog.Type = sensorReactionLog.ReactionType;
            reactionLog.Zone = zone;

            return reactionLog;
        }

        protected override ReactionLog _HistorySubmit(SensorReactionLog sensorReactionLog, List<SensorReactionLog> sensorReactionLogList, int nSensorZoneHistoryID, UnE.Spatial.Zone zone)
        {
            ReactionSecurityLog reactionLog = new ReactionSecurityLog();
            reactionLog.SensorZoneHistoryID = nSensorZoneHistoryID;
            reactionLog.SensorReactionLogList = sensorReactionLogList;

            int nReactionType = 0;

            //자탐 ReactionType가져옴
            if (m_dicHistoryType.ContainsKey(nSensorZoneHistoryID))
            {
                nReactionType = m_dicHistoryType[nSensorZoneHistoryID];
            }

            reactionLog.SensorType = (int)UnE.Sensor.IFacility.FacilityType.Security_Sensor;

            string strMemberID = "";

            if (m_dicHistoryMember.ContainsKey(nSensorZoneHistoryID))
                strMemberID = m_dicHistoryMember[nSensorZoneHistoryID];

            if (m_dicGenUserIDDNicName.ContainsKey(strMemberID))
                reactionLog.UserName = m_dicGenUserIDDNicName[strMemberID];

            reactionLog.Time = sensorReactionLog.Time;
            reactionLog.SensorType = sensorReactionLog.SensorType;
            reactionLog.Zone = zone;
            reactionLog.FacilityType = sensorReactionLog.Param3;
            reactionLog.Type = nReactionType;

            if (m_owner != null)
                reactionLog.ManagerName = m_owner.FindManagerName(zone, IFacility.FacilityType.Security_Sensor);

            return reactionLog;
        }

        // EquipmentZoneID로 SensorID를 찾아온다
        // 빠른 검색을 위하여 Dictionary 형태로 리턴한다.
        // Key : SensorZone ID
        protected override Dictionary<int, UnE.Sensor.ISensor> FindSensorZone(List<UnE.Spatial.EquipmentZone> arrEquipZoneList)
        {
            Dictionary<int, ISensor> dicSensorZones = new Dictionary<int, ISensor>();
            if (arrEquipZoneList == null)
                return null;

            if (SensorManager != null)
            {
                List<IFacility.FacilityType> sensorTypes = new List<IFacility.FacilityType>();

                sensorTypes.Add(IFacility.FacilityType.Intrusion_S1);
                sensorTypes.Add(IFacility.FacilityType.Loiter_S1);
                sensorTypes.Add(IFacility.FacilityType.Collapse_S1);
                sensorTypes.Add(IFacility.FacilityType.Theft_S1);
                sensorTypes.Add(IFacility.FacilityType.Neglect_S1);
                sensorTypes.Add(IFacility.FacilityType.VirtualFence_S1);
                sensorTypes.Add(IFacility.FacilityType.EmergencyBell_S1);
                sensorTypes.Add(IFacility.FacilityType.GeneralIntrusionT1_S1);
                sensorTypes.Add(IFacility.FacilityType.GeneralIntrusionT2_S1);
                sensorTypes.Add(IFacility.FacilityType.InternalIntrusionT3_S1);
                sensorTypes.Add(IFacility.FacilityType.VaultIntrusionT4_S1);
                sensorTypes.Add(IFacility.FacilityType.CustomerEmergencyC1_S1);
                sensorTypes.Add(IFacility.FacilityType.CustomerEmergencyC2_S1);
                sensorTypes.Add(IFacility.FacilityType.RescueQQ_S1);
                sensorTypes.Add(IFacility.FacilityType.GasG1_S1);
                sensorTypes.Add(IFacility.FacilityType.BlackoutAbnormalityU1_S1);
                sensorTypes.Add(IFacility.FacilityType.LeakAbnormalityU4_S1);
                sensorTypes.Add(IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1);
                sensorTypes.Add(IFacility.FacilityType.ExternalAlarmBell);

                foreach (EquipmentZone equip in arrEquipZoneList)
                {
                    List<ISensor> arSensors = SensorManager.FindZoneInSensor(equip.ID, sensorTypes);

                    if (arSensors == null)
                        continue;

                    //SensorZoneID 구함
                    foreach (ISensor sensor in arSensors)
                    {
                        dicSensorZones[sensor.ID] = sensor;
                    }
                }
            }

            return dicSensorZones;
        }

        // arrAllLog 중에서 방범탐지 신호만을 추려낸다.
        protected override List<DetectLog> GetDetectLog(List<SensorReactionLog> arrAllLog)
        {
            List<DetectLog> arrDetectLog = new List<DetectLog>();
            ArrayList arrComboBoxDate = new ArrayList();
            //ArrayList arrReactionLog = new ArrayList();

            foreach (SensorReactionLog reactionLog in arrAllLog)
            {
                // 센서신호이거나 수동신고인 경우 추출
                if (reactionLog.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS
                     || reactionLog.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS
                     /*|| (reactionLog.ReactionType == (int)libSensorProcess.ReactionType.NOTIFY_SECURITY && reactionLog.Param2 == "0")*/)
                {
                    DetectLog detect = new DetectLog();

                    detect.SensorReactionHistoryID = reactionLog.ID;
                    detect.HistoryID = reactionLog.SensorZoneHistoryID;
                    detect.Time = reactionLog.Time;
                    detect.Memo = reactionLog.Memo;

                    Zone zone = null;

                    /*if (reactionLog.Param2 == "0")
                    {
                        if (ZoneManager != null)
                            zone = ZoneManager.GetZone(reactionLog.Param1);

                        detect.zoneID = reactionLog.Param1;
                    }
                    else*/
                    {
                        if (m_dicZoneHistories.ContainsKey(reactionLog.SensorZoneHistoryID))
                            zone = m_dicZoneHistories[reactionLog.SensorZoneHistoryID];

                        detect.zoneID = zone.ID;
                    }

                    string szBuildingName = zone.Building != null ? zone.Building.BuildingName : "";
                    string szGroupName = szBuildingName != "" ? zone.Building.BuildingGroup.BuildingGroupName : "";
                    string strFloorIndex = zone.Floor != null ? zone.Floor.ToString() : "";

                    //EquipZone표시는 센서신호일때만.. 수동신고일때는 알 수 없다.
                    if (reactionLog.ReactionType == 0)
                    {
                        if (ZoneManager != null)
                        {
                            //EquipZone구하기
                            detect.EquipZone = ZoneManager.GetEquipZone(reactionLog.Param1);
                        }
                    }

                    if (szGroupName == "")
                        detect.BuildingGroup = "외부 영역";
                    else
                        detect.BuildingGroup = szBuildingName;

                    if (szBuildingName == "")
                        detect.BuildingName = zone.ZoneName;
                    else
                        detect.BuildingName = szBuildingName;

                    detect.FloorName = strFloorIndex;
                    string strManagerName = m_owner != null ? m_owner.FindManagerName(zone, IFacility.FacilityType.Security_Sensor) : "";
                    detect.ManagerName = strManagerName;
                    detect.DetectType = GetReactionString(Convert.ToInt32(reactionLog.Param3));
                    detect.DetectionStatusName = GetDetectionStatusName(reactionLog.DetectionResult);

                    arrDetectLog.Add(detect);
                }
            }
            //
            //arrDetectLog.Sort();
            return arrDetectLog;
        }

        protected override string GetReactionString(int nType)
        {
            string strType = "";
            switch (nType)
            {
                case (int)IFacility.FacilityType.Intrusion_S1:
                case (int)IFacility.FacilityType.Loiter_S1:
                case (int)IFacility.FacilityType.Collapse_S1:
                case (int)IFacility.FacilityType.Theft_S1:
                case (int)IFacility.FacilityType.Neglect_S1:
                case (int)IFacility.FacilityType.VirtualFence_S1:
                case (int)IFacility.FacilityType.Fire_S1:
                case (int)IFacility.FacilityType.EmergencyBell_S1:
                    strType = "SVMS센서";
                    break;
                case (int)IFacility.FacilityType.GeneralIntrusionT1_S1:
                case (int)IFacility.FacilityType.GeneralIntrusionT2_S1:
                case (int)IFacility.FacilityType.InternalIntrusionT3_S1:
                case (int)IFacility.FacilityType.VaultIntrusionT4_S1:
                case (int)IFacility.FacilityType.FireF1_S1:
                case (int)IFacility.FacilityType.CustomerEmergencyC1_S1:
                case (int)IFacility.FacilityType.CustomerEmergencyC2_S1:
                case (int)IFacility.FacilityType.RescueQQ_S1:
                case (int)IFacility.FacilityType.GasG1_S1:
                case (int)IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                case (int)IFacility.FacilityType.LeakAbnormalityU4_S1:
                case (int)IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    strType = "Access센서";
                    break;
                case (int)IFacility.FacilityType.ExternalAlarmBell:
                    strType = "외부비상벨";
                    break;
                default:
                    strType = "방범센서";
                    break;
            }

            return strType;
        }

        protected override List<Statistics> GetStatisticsLog(System.Collections.ArrayList arrZoneList, string strStartDate, string strEndDate)
        {
            List<Statistics> arrStatistics = new List<Statistics>();
            List<int> liAddedReactionHistoryIDs = new List<int>();

            int beginDetectType = 0;

            //Zone별로 Log에서 탐지,실제사고,오작동,처리되지않은신호의 갯수, 오작동률 등을 구함
            foreach (Zone zone in arrZoneList)
            {
                List<int> arrHistoryList = FindHistoryID(zone.ID);
                if (arrHistoryList == null)
                    continue;

                if (arrHistoryList.Count == 0)
                    continue;

                // 이미 추가한 로그인지 확인
                bool isGoTo = false;
                foreach (int nReactionHistoryID in arrHistoryList)
                {
                    if (liAddedReactionHistoryIDs.Contains(nReactionHistoryID))
                        isGoTo = true;
                    else
                        liAddedReactionHistoryIDs.Add(nReactionHistoryID);
                }

                if (isGoTo == true)
                    continue;

                Statistics stat = new Statistics();

                int nReportCount = 0;
                int nMalFunctionCount = 0;
                int nIgnoreCount = 0;
                int nCurrentDetectCount = 0;

                foreach (int nHistoryID in arrHistoryList)
                {
                    List<SensorReactionLog> arrLog = new List<SensorReactionLog>();

                    if (m_dicHistorySensorReactionLog.ContainsKey(nHistoryID))
                        arrLog = m_dicHistorySensorReactionLog[nHistoryID];

                    int nType = 0;

                    foreach (SensorReactionLog log in arrLog)
                    {
                        if (log.ReactionType != (int)libSensorProcess.ReactionType.NOTIFY_SECURITY &&
                            log.ReactionType != (int)libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS &&
                            log.ReactionType != (int)libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS &&
                            log.ReactionType != (int)libSensorProcess.ReactionType.END_S1SVMS_STATUS &&
                            log.ReactionType != (int)libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS &&
                            log.ReactionType != (int)libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS &&
                            log.ReactionType != (int)libSensorProcess.ReactionType.END_S1ACCESS_STATUS &&
                            log.ReactionType != (int)libSensorProcess.ReactionType.MALFUNCTION)
                            continue;

                        if ((log.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS ||
                            log.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS) && log.Param3 != "null")
                            beginDetectType = Convert.ToInt32(log.Param3);

                        if (log.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS || log.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS)
                        {
                            List<SensorReactionLog> arrSensorLog = null;

                            //<MulFunctionLog, SensorReactionLog> Dictionary에 값 추가
                            if (m_dicStatisticsReactionLog.ContainsKey(stat))
                                arrSensorLog = m_dicStatisticsReactionLog[stat];
                            else
                            {
                                arrSensorLog = new List<SensorReactionLog>();
                                m_dicStatisticsReactionLog[stat] = arrSensorLog;
                            }
                            arrSensorLog.Add(log);
                        }

                        if (log.ReactionType == (int)libSensorProcess.ReactionType.NOTIFY_SECURITY)
                        {
                            nReportCount++;
                            nType = log.ReactionType;

                            break;
                        }
                        else if (log.ReactionType == (int)libSensorProcess.ReactionType.MALFUNCTION)
                        {
                            nMalFunctionCount++;
                            nType = log.ReactionType;

                            break;
                        }
                        // 처리되지 않음.
                        else if (log.ReactionType == (int)libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS
                            || log.ReactionType == (int)libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS)
                        {
                            nType = log.ReactionType;

                            break;
                        }
                    }

                    if (nType == (int)libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS ||
                        nType == (int)libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS)
                        nCurrentDetectCount++;

                    if (!m_dicHistoryType.ContainsKey(nHistoryID))
                    {
                        m_dicHistoryType.Add(nHistoryID, nType);
                    }
                }

                if (beginDetectType < (int)IFacility.FacilityType.Intrusion_S1 ||
                    beginDetectType > (int)IFacility.FacilityType.ExternalAlarmBell ||
                    beginDetectType == (int)IFacility.FacilityType.Fire_S1 ||
                    beginDetectType == (int)IFacility.FacilityType.FireF1_S1)
                    continue; 

                stat.DetectCount = arrHistoryList.Count;

                //처리되지 않음
                nIgnoreCount = arrHistoryList.Count - (nReportCount + nMalFunctionCount) - nCurrentDetectCount;

                double percentMalFunction = (nMalFunctionCount * 100) / arrHistoryList.Count;

                stat.SensorZoneHistoryIDList = arrHistoryList;
                stat.DetectType = GetReactionString(beginDetectType);

                stat.ReportCount = nReportCount;
                stat.UserResetCount = nMalFunctionCount;
                stat.Zone = zone;

                if (m_owner != null)
                    stat.ManagerName = m_owner.FindManagerName(zone, IFacility.FacilityType.Security_Sensor);

                stat.IgnoreCount = nIgnoreCount;
                stat.PercentUserReset = percentMalFunction;
                stat.CurrentDetectCount = nCurrentDetectCount;

                string szBuildingName = zone.Building != null ? zone.Building.DisplayText : "";
                string szGroupName = szBuildingName != "" ? zone.Building.BuildingGroup.BuildingGroupName : "";
                string strFloorIndex = zone.Floor != null ? zone.Floor.ToString() : "";

                if (szGroupName == "" || szGroupName == "null")
                    stat.BuildingGroupName = "외부 영역";
                else
                    stat.BuildingGroupName = szGroupName;

                if (szBuildingName == "" || szBuildingName == "null")
                    stat.BuildingName = zone.DisplayText;
                else
                    stat.BuildingName = szBuildingName;

                stat.FloorName = strFloorIndex;

                arrStatistics.Add(stat);
            }
            //오작동 이력 로그들을 배열에 저장
            return arrStatistics;
        }

        // SensorReactionHistory DB Table로부터 읽은 Parameter들을 사용하여 SensorReactionLog 객체의 값을 채운다.
        protected override void SetReactionLogParam(SensorReactionLog reactionLog, int nReactionType, int nParam1, string strParam2, string strParam3, string strParam4, string strParam5)
        {
            reactionLog.SensorType = (int)IFacility.FacilityType.Security_Sensor;

            if (nReactionType == (int)libSensorProcess.ReactionType.MALFUNCTION
                || nReactionType == (int)libSensorProcess.ReactionType.NOTIFY_SECURITY
                || nReactionType == (int)libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS
                || nReactionType == (int)libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS)
            {
                if (!m_dicHistoryMember.ContainsKey(reactionLog.SensorZoneHistoryID))
                    m_dicHistoryMember.Add(reactionLog.SensorZoneHistoryID, strParam3);
            }

            reactionLog.Param1 = nParam1;
            reactionLog.Param2 = strParam2;
            reactionLog.Param3 = strParam3;
            reactionLog.Param4 = strParam4;
            reactionLog.Param5 = strParam5;

            //사내방송실시, 메시지(탐지/신고 여부)
            //사내방송실시(탐지)
            if (nReactionType == (int)libSensorProcess.ReactionType.RUN_BROADCAST)
            {
                if (strParam3 == "")
                {
                    reactionLog.ReactionType = (int)libSensorProcess.ReactionType.RUN_DETECT_BROADCAST;
                }
                else if (strParam3 != "") //사내방송실시(신고)
                {
                    reactionLog.ReactionType = (int)libSensorProcess.ReactionType.RUN_REPORT_BROADCAST;
                }
            }

            //문자메시지
            if (nReactionType == (int)libSensorProcess.ReactionType.SEND_SMS)
            {
                if (reactionLog.Message.Contains("복구"))
                {
                    reactionLog.ReactionType = (int)libSensorProcess.ReactionType.SEND_REPAIR_SMS;
                }
                else if (reactionLog.Message.Contains("오작동"))
                {
                    reactionLog.ReactionType = (int)libSensorProcess.ReactionType.SEND_MALFUNCTION_SMS;
                }
                else if (reactionLog.Message.Contains("탐지"))
                {
                    reactionLog.ReactionType = (int)libSensorProcess.ReactionType.SEND_DETECT_SMS;
                }
                else if (reactionLog.Message.Contains("신고"))
                {
                    reactionLog.ReactionType = (int)libSensorProcess.ReactionType.SEND_REPORT_SMS;
                }
            }
        }
    }

    public class ReactionSecurityLog : ReactionLog
    {
        protected override string _ToString()
        {
            string strReactionType = string.Empty;
            int nfacilityType;

            if (Type == (int)libSensorProcess.ReactionType.NOTIFY_SECURITY)
                strReactionType = "방범 발생";
            else if (Type == (int)libSensorProcess.ReactionType.MALFUNCTION)
                strReactionType = "오작동 처리";
            else if (Type == (int)libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS)
                strReactionType = "S1SVMS 시작";
            else if (Type == (int)libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS)
                strReactionType = "S1SVMS 무시";
            else if (Type == (int)libSensorProcess.ReactionType.END_S1SVMS_STATUS)
                strReactionType = "S1SVMS 종료";
            else if (Type == (int)libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS)
            {
                if (int.TryParse(FacilityType, out nfacilityType) && (int)IFacility.FacilityType.ExternalAlarmBell == nfacilityType)
                    strReactionType = "외부비상벨 시작";
                else
                    strReactionType = "S1ACCESS 시작";
            }
            else if (Type == (int)libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS)
            {
                if (int.TryParse(FacilityType, out nfacilityType) && (int)IFacility.FacilityType.ExternalAlarmBell == nfacilityType)
                    strReactionType = "외부비상벨 무시";
                else
                    strReactionType = "S1ACCESS 무시";
            }
            else if (Type == (int)libSensorProcess.ReactionType.END_S1ACCESS_STATUS)
            {
                if (int.TryParse(FacilityType, out nfacilityType) && (int)IFacility.FacilityType.ExternalAlarmBell == nfacilityType)
                    strReactionType = "외부비상벨 종료";
                else
                    strReactionType = "S1ACCESS 종료";
            }

            if (SensorType == (int)IFacility.FacilityType.FIRE_SENSOR)
            {
                return String.Format("{0:0000}-{1:00}-{2:00} {3} {4:00}:{5:00}", Time.Year, Time.Month, Time.Day, Time.Hour < 12 ? "오전" : "오후", Time.Hour > 12 ? Time.Hour - 12 : Time.Hour, Time.Minute)
                    + "   [ " + SOPServer.EventTypeString.GetEventTypeDetectString(Convert.ToInt32(FacilityType)) + " 신고 ] " + strReactionType;
            }
            else
            {
                return String.Format("{0:0000}-{1:00}-{2:00} {3} {4:00}:{5:00}", Time.Year, Time.Month, Time.Day, Time.Hour < 12 ? "오전" : "오후", Time.Hour > 12 ? Time.Hour - 12 : Time.Hour, Time.Minute)
                    + "   [ " + SOPServer.EventTypeString.GetEventTypeDetectString(Convert.ToInt32(FacilityType)) + " 탐지 ] " + strReactionType;
            }
        }
    }
}
