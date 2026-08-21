using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnE.Spatial;
using DBUtility;
using UnE.Sensor;
using UnE.PSM;

namespace libSDMSReport
{
    /// <summary>
    /// 누출신호에 대한 이력처리 클래스
    /// SensorReactionLog.Param1 : 탐지된 Sensor가 대표하는(또는 위치하는) EquipZone ID
    /// SensorReactionLog.Param2 : SensorZone ID
    /// SensorReactionLog.Param3 : 이 값은 SensorReactionHistory Table의 ReactionType값에 따라서 각각 다른 의미를 가진다.
    ///                            ReactionType.BEGIN_PSM_STATUS : Origin Sensor ID
    ///                            ReactionType.IGNORE_PSM_DETECT : 사용되지 않음
    ///                            ReactionType.CHANGE_PSM_ALARM_DEPTH : 같은 영역(또는 탱크)에 포함된 다른 센서에서 다른 단계의 알람이 들어올 경우에 해당 센서의 SensorZoneID가 표시된다.
    ///                                                                  예를 들어 하나의 염산탱크에 염산센서1과 염산센서2가 연결되어 있는데 처음에 염산센서1에서 1단계 알람이 발생하고, 뒤이어 염산센서2에서 2단계 알람이 발생하였다.
    ///                                                                  이 경우 Param3는 염산센서2의 SensorZoneID 가 표시된다.
    ///                                                                  만일, 두번째 알람 역시 염산센서1을 통하여 2단계나 3단계로 발생한다면 Param3는 염산센서1의 SensorZoneID가 된다.
    ///                            ReactionType.NOTIFY_PSM : 누출신고한 SOPGenUser의 ID
    ///                            ReactionType.PSM_USER_RESET : 신호복구를 수행한 SOPGenUser의 ID
    /// SensorReactionLog.Param4 : Message
    /// SensorReactionLog.Param5 : 알람 단계
    /// </summary>
    public class ReactionPSMManager : ReactionManager
    {
        private IPSMManager m_psmMgr = null;

        public IPSMManager PSMManager
        {
            get { return m_psmMgr; }
            set { m_psmMgr = value; }
        }

        // SensorReactionHistory DB Table로부터 읽은 Parameter들을 사용하여 SensorReactionLog 객체의 값을 채운다.
        protected override void SetReactionLogParam(SensorReactionLog reactionLog, int nReactionType, int nParam1, string strParam2, string strParam3, string strParam4, string strParam5)
        {
            reactionLog.SensorType = (int)IFacility.FacilityType.PSM_SENSOR;

            if (nReactionType == (int)libSensorProcess.ReactionType.IGNORE_PSM_DETECT || nReactionType == (int)libSensorProcess.ReactionType.NOTIFY_PSM || nReactionType == (int)libSensorProcess.ReactionType.PSM_USER_RESET)
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

        // strStartDate와 strEndDate 사이에서 arrZoneList내에 존재하는 모든 누출신호에 대한 통계자료를 작성한다.
        protected override List<Statistics> GetStatisticsLog(ArrayList arrZoneList, string strStartDate, string strEndDate)
        {
            List<Statistics> arrStatistics = new List<Statistics>();

            if (ZoneManager == null)
                return arrStatistics;

            List<int> liAddedReactionHistoryIDs = new List<int>();

            //Zone별로 Log에서 누출,신호복구,처리되지않은신호의 갯수, 오작동률 등을 구함
            foreach (Zone zone in arrZoneList)
            {
                List<EquipmentZone> equipZones = ZoneManager.GetEquipmentZoneList(zone);

                if (equipZones == null)
                    continue;

                foreach (EquipmentZone equipZone in equipZones)
                {
                    List<int> arrHistoryList = FindHistoryID(zone, equipZone);
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

                    //오작동이력 클래스 생성
                    StatisticsPSM stat = new StatisticsPSM();

                    int nPSMCount = 0;
                    int nUserResetCount = 0;
                    int nIgnoreCount = 0;
                    int nCurrentDetectCount = 0;

                    foreach (int nHistoryID in arrHistoryList)
                    {
                        List<SensorReactionLog> arrLog = new List<SensorReactionLog>();

                        if (m_dicHistorySensorReactionLog.ContainsKey(nHistoryID))
                            arrLog = m_dicHistorySensorReactionLog[nHistoryID];

                        int nType = (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS;

                        foreach (SensorReactionPSMLog log in arrLog)
                        {
                            stat.PSMMaterial = log.PSMMaterial;

                            if (log.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS)
                            {
                                List<SensorReactionLog> arrSensorLog = null;


                                //<MulFunctionPSMLog, SensorReactionPSMLog> Dictionary에 값 추가
                                if (m_dicStatisticsReactionLog.ContainsKey(stat))
                                    arrSensorLog = m_dicStatisticsReactionLog[stat];
                                else
                                {
                                    arrSensorLog = new List<SensorReactionLog>();
                                    m_dicStatisticsReactionLog[stat] = arrSensorLog;
                                }
                                arrSensorLog.Add(log);
                            }

                            if (log.ReactionType == (int)libSensorProcess.ReactionType.NOTIFY_PSM)
                            {
                                nPSMCount++;
                                nType = (int)libSensorProcess.ReactionType.NOTIFY_PSM;

                                break;
                            }
                            else if (log.ReactionType == (int)libSensorProcess.ReactionType.MALFUNCTION || log.ReactionType == (int)libSensorProcess.ReactionType.PSM_USER_RESET)
                            {
                                nUserResetCount++;
                                nType = (int)libSensorProcess.ReactionType.PSM_USER_RESET;

                                break;
                            }
                            else if (log.ReactionType == (int)libSensorProcess.ReactionType.IGNORE_PSM_DETECT
                                || log.ReactionType == (int)libSensorProcess.ReactionType.END_STATUS
                                || log.ReactionType == (int)libSensorProcess.ReactionType.END_PSM_STATUS)
                            {
                                nType = (int)libSensorProcess.ReactionType.IGNORE_PSM_DETECT;

                                break;
                            }
                        }

                        if (nType == (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS)
                        {
                            nCurrentDetectCount++;
                        }

                        if (!m_dicHistoryType.ContainsKey(nHistoryID))
                        {
                            m_dicHistoryType.Add(nHistoryID, nType);
                        }

                    }

                    stat.DetectCount = arrHistoryList.Count;

                    //처리되지 않음
                    nIgnoreCount = arrHistoryList.Count - (nPSMCount + nUserResetCount) - nCurrentDetectCount;

                    double PercentMulFunction = (nUserResetCount * 100) / arrHistoryList.Count;

                    stat.SensorZoneHistoryIDList = arrHistoryList;
                    stat.DetectType = GetReactionString((int)DetectType.PSM);

                    stat.ReportCount = nPSMCount;
                    stat.UserResetCount = nUserResetCount;
                    stat.Zone = zone;
                    stat.EquipmentZone = equipZone;

                    if (m_owner != null)
                        stat.ManagerName = m_owner.FindManagerName(zone, IFacility.FacilityType.PSM_SENSOR);

                    stat.IgnoreCount = nIgnoreCount;
                    stat.CurrentDetectCount = nCurrentDetectCount;
                    stat.PercentUserReset = PercentMulFunction;

                    string szBuildingName = zone.Building != null ? zone.Building.DisplayText : "";
                    string szGroupName = szBuildingName != "" ? zone.Building.BuildingGroup.BuildingGroupName : "";
                    string strFloorIndex = zone.Floor != null ? zone.Floor.ToString() : "";

                    if (szGroupName == "")
                        stat.BuildingGroupName = "외부 영역";
                    else
                        stat.BuildingGroupName = szGroupName;

                    if (szBuildingName == "")
                        stat.BuildingName = zone.DisplayText;
                    else
                        stat.BuildingName = szBuildingName;

                    stat.FloorName = strFloorIndex;

                    arrStatistics.Add(stat);
                }
            }

            //오작동 이력 로그들을 배열에 저장
            return arrStatistics;
        }

        // arrAllLog 중에서 누출탐지 신호만을 추려낸다.
        protected override List<DetectLog> GetDetectLog(List<SensorReactionLog> arrAllLog)
        {
            List<DetectLog> arrDetectLog = new List<DetectLog>();
            ArrayList arrComboBoxDate = new ArrayList();
            //ArrayList arrReactionLog = new ArrayList();

            foreach (SensorReactionPSMLog reactionLog in arrAllLog)
            {
                // 누출신호인 경우
                if (reactionLog.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS)
                {
                    DetectPSMLog detect = new DetectPSMLog();

                    detect.SensorReactionHistoryID = reactionLog.ID;
                    detect.HistoryID = reactionLog.SensorZoneHistoryID;
                    detect.Time = reactionLog.Time;
                    detect.PSMMaterial = reactionLog.PSMMaterial;
                    detect.PSMSensor = reactionLog.PSMSensor;
                    detect.Memo = reactionLog.Memo;

                    foreach (SensorReactionPSMLog item in from items in arrAllLog.Cast<SensorReactionPSMLog>()
                                                          where items.SensorZoneHistoryID == detect.HistoryID
                                                          orderby items.Time ascending
                                                          select items
                                                              )
                    {
                        detect.DetectStartDate = item.Time.AddMinutes(-10);
                        break;
                    }

                    foreach (SensorReactionPSMLog item in from items in arrAllLog.Cast<SensorReactionPSMLog>()
                                                          where items.SensorZoneHistoryID == detect.HistoryID
                                                          orderby items.Time descending
                                                          select items
                                                          )
                    {
                        detect.DetectEndDate = item.Time.AddMinutes(10);
                        break;
                    }

                    Zone zone = null;

                    if (reactionLog.Param2 == "0")
                    {
                        if (ZoneManager != null)
                            zone = ZoneManager.GetZone(reactionLog.Param1);

                        detect.zoneID = reactionLog.Param1;
                    }
                    else
                    {
                        if (m_dicZoneHistories.ContainsKey(reactionLog.SensorZoneHistoryID))
                            zone = m_dicZoneHistories[reactionLog.SensorZoneHistoryID];

                        detect.zoneID = zone.ID;
                    }

                    string szBuildingName = zone.Building != null ? zone.Building.BuildingName : "";
                    string szGroupName = szBuildingName != "" ? zone.Building.BuildingGroup.BuildingGroupName : "";
                    string strFloorIndex = zone.Floor != null ? zone.Floor.ToString() : "";

                    //EquipZone표시는 누출센서로부터 받은 것만.. 수동신고일때는 알 수 없다.
                    if (reactionLog.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS)
                    {
                        if (ZoneManager != null)
                        {
                            //EquipZone구하기
                            detect.EquipZone = ZoneManager.GetEquipZone(reactionLog.Param1);
                        }
                    }

                    if (reactionLog.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS ||
                        reactionLog.ReactionType == (int)libSensorProcess.ReactionType.CHANGE_PSM_ALARM_DEPTH)
                    {
                        int nAlarmLevel = 0;
                        if (int.TryParse(reactionLog.Param5, out nAlarmLevel))
                        {
                            detect.AlarmLevel = nAlarmLevel;
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
                    string strManagerName = m_owner != null ? m_owner.FindManagerName(zone, IFacility.FacilityType.PSM_SENSOR) : "";
                    detect.ManagerName = strManagerName;

                    // 누출
                    detect.DetectType = GetReactionString((int)DetectType.PSM);
                    /*switch (reactionLog.ReactionType)
                    {
                        case (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS:
                        case (int)libSensorProcess.ReactionType.IGNORE_PSM_DETECT:
                        case (int)libSensorProcess.ReactionType.NOTIFY_PSM:
                        case (int)libSensorProcess.ReactionType.CHANGE_PSM_ALARM_DEPTH:
                        case (int)libSensorProcess.ReactionType.END_PSM_STATUS:
                            detect.DetectType = GetReactionString((int)DetectType.PSM);
                            break;
                        default:
                            detect.DetectType = GetReactionString(1);
                            break;
                    }*/

                    detect.DetectionStatusName = GetDetectionStatusName(reactionLog.DetectionResult);

                    arrDetectLog.Add(detect);
                }
            }
            //
            //arrDetectLog.Sort();
            return arrDetectLog;
        }

        // EquipmentZoneID로 SensorID를 찾아온다
        // 빠른 검색을 위하여 Dictionary 형태로 리턴한다.
        // Key : SensorZone ID
        protected override Dictionary<int, ISensor> FindSensorZone(List<EquipmentZone> arrEquipZoneList)
        {
            Dictionary<int, ISensor> dicSensorZones = new Dictionary<int, ISensor>();
            if (arrEquipZoneList == null)
                return dicSensorZones;

            if (SensorManager != null)
            {
                foreach (EquipmentZone equip in arrEquipZoneList)
                {
                    List<ISensor> arSensors = SensorManager.FindZoneInSensor(equip.ID, IFacility.FacilityType.PSM_SENSOR);

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

        protected override string GetDetectionStatusName(SensorReactionLog.SignalResult status)
        {
            switch (status)
            {
                case SensorReactionLog.SignalResult.REAL:
                    return "실제";
                case SensorReactionLog.SignalResult.USER_RESET:
                    return "신호복구";
                case SensorReactionLog.SignalResult.TEST:
                default:
                    return "테스트";
            }
        }

        protected override SensorReactionLog.SignalResult GetReverseDetectionStatus(string strDetectionStatusName)
        {
            switch (strDetectionStatusName)
            {
                case "실제":
                    return SensorReactionLog.SignalResult.REAL;
                case "신호복구":
                    return SensorReactionLog.SignalResult.USER_RESET;
                case "테스트":
                default:
                    return SensorReactionLog.SignalResult.TEST;
            }
        }

        protected override ReactionLog _HistorySubmit(SensorReactionLog sensorReactionLog, List<SensorReactionLog> sensorReactionLogList, int nSensorZoneHistoryID, Zone zone)
        {
            ReactionPSMLog reactionLog = new ReactionPSMLog();
            reactionLog.SensorZoneHistoryID = nSensorZoneHistoryID;
            reactionLog.SensorReactionLogList = sensorReactionLogList;

            int nReactionType = (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS;

            //누출 ReactionType가져옴
            if (m_dicHistoryType.ContainsKey(nSensorZoneHistoryID))
            {
                nReactionType = m_dicHistoryType[nSensorZoneHistoryID];
            }
            
            reactionLog.SensorType = (int)IFacility.FacilityType.PSM_SENSOR;

            // 신호복구인지 실제 누출인지, 무시된 신호인지등을 구분하기 위함
            //if (reactionLog.SensorType == (int)IFacility.FacilityType.PSM_SENSOR)
            {
                foreach (SensorReactionPSMLog Typelog in sensorReactionLogList)
                {
                    if (Typelog.ReactionType == (int)libSensorProcess.ReactionType.NOTIFY_PSM ||
                        Typelog.ReactionType == (int)libSensorProcess.ReactionType.PSM_USER_RESET ||
                        Typelog.ReactionType == (int)libSensorProcess.ReactionType.IGNORE_PSM_DETECT)
                    {
                        nReactionType = Typelog.ReactionType;
                        break;
                    }
                    
                    // PSM_USER_RESET으로 기록되어야 하나 MALFUNCTION(오작동)으로 잘못 표기된 경우(이전 버전)
                    if (Typelog.ReactionType == (int)libSensorProcess.ReactionType.MALFUNCTION)
                    {
                        nReactionType = (int)libSensorProcess.ReactionType.PSM_USER_RESET;
                        break;
                    }
                    
                    nReactionType = Typelog.ReactionType;
                }
            }
            
            int nLevel = -1;
            if (int.TryParse(sensorReactionLog.Param5, out nLevel))
            {
                reactionLog.Level = nLevel;
            }

            string strMemberID = "";
            SensorReactionPSMLog psmLog = (SensorReactionPSMLog)sensorReactionLog;

            if (m_dicHistoryMember.ContainsKey(nSensorZoneHistoryID))
                strMemberID = m_dicHistoryMember[nSensorZoneHistoryID];

            if (m_dicGenUserIDDNicName.ContainsKey(strMemberID))
                reactionLog.UserName = m_dicGenUserIDDNicName[strMemberID];

            reactionLog.Time = psmLog.Time;
            reactionLog.SensorType = psmLog.SensorType;
            reactionLog.Zone = zone;
            reactionLog.Type = nReactionType;
            reactionLog.PSMSensor = psmLog.PSMSensor;
            reactionLog.PSMMaterial = psmLog.PSMMaterial;

            if (m_owner != null)
                reactionLog.ManagerName = m_owner.FindManagerName(zone, IFacility.FacilityType.PSM_SENSOR);

            return reactionLog;
        }

        protected override ReactionLog _GetReactionLog(SensorReactionLog sensorReactionLog, List<SensorReactionLog> sensorReactionLogList, int nSensorZoneHistoryID, Zone zone)
        {
            ReactionPSMLog reactionLog = new ReactionPSMLog();
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


            SensorReactionPSMLog psmLog = (SensorReactionPSMLog)sensorReactionLog;

            reactionLog.Time = sensorReactionLog.Time;
            reactionLog.SensorType = sensorReactionLog.SensorType;
            reactionLog.Zone = zone;
            reactionLog.Type = sensorReactionLog.ReactionType;
            reactionLog.PSMSensor = psmLog.PSMSensor;
            reactionLog.PSMMaterial = psmLog.PSMMaterial;

            if (m_owner != null)
                reactionLog.ManagerName = m_owner.FindManagerName(zone, IFacility.FacilityType.PSM_SENSOR);

            int nLevel = -1;
            if (int.TryParse(sensorReactionLog.Param5, out nLevel))
            {
                reactionLog.Level = nLevel;
            }

            return reactionLog;
        }

        protected override bool _GetReactionHistory(List<SensorReactionLog> arrReactionLog, string strSensorZoneHistoryIDs, Dictionary<int, SensorReactionLog> dicReactionLogs, ref int nMinReactionLogID, ref int nMaxReactionLogID)
        {
            string strSQL = "select srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, srh.DetectionStatus, sz.OrgSensorID ";
            strSQL += "from SensorReactionHistory as srh, ";
            strSQL += "SensorZoneHistory as szh, ";
            strSQL += "SensorZone as sz ";
            strSQL += "where srh.SensorHistoryID = szh.id ";
            strSQL += "and (szh.SensorID = sz.ID and sz.Type = 11) ";
            strSQL += "and srh.SensorHistoryID in (" + strSensorZoneHistoryIDs + ")";

            ArrayList arrResult = DBManager.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            DateTime dt = DateTime.Now;

            for (int i = 0; i < nResultCount - 11; i += 12)
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
                int nPSMSensorID = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);

                SensorReactionPSMLog reactionLog = new SensorReactionPSMLog();
                reactionLog.ID = nID;
                reactionLog.SensorZoneHistoryID = nSensorHistoryID;
                reactionLog.ReactionType = nReactionType;
                reactionLog.Time = time;
                reactionLog.SetDetectionResult(nDetectionStatus);

                if (PSMManager != null)
                    reactionLog.PSMSensor = PSMManager.GetSensor(nPSMSensorID);
                
                if (reactionLog.PSMSensor != null)
                {
                    foreach (UnE.PSM.PSMTank psmTank in reactionLog.PSMSensor.LinkedTankList)
                    {
                        reactionLog.PSMMaterial = psmTank.Material;
                        break;
                    }
                }

                reactionLog.SensorType = (int)IFacility.FacilityType.PSM_SENSOR;

                if (nReactionType == (int)libSensorProcess.ReactionType.IGNORE_PSM_DETECT
                    || nReactionType == (int)libSensorProcess.ReactionType.NOTIFY_PSM
                    || nReactionType == (int)libSensorProcess.ReactionType.PSM_USER_RESET)
                {
                    if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                        m_dicHistoryMember[nSensorHistoryID] = Param3;
                }
                
                reactionLog.Param1 = Param1;

                //Message에서 
                reactionLog.Message = strMessage;
                reactionLog.Param2 = Param2;
                reactionLog.Param3 = Param3;
                reactionLog.Param4 = Param4;
                reactionLog.Param5 = Param5;
                
                //사내방송실시, 메시지(탐지/신고 여부)
                //사내방송실시(탐지)
                if (nReactionType == 10 && Param3 == "")
                {
                    reactionLog.ReactionType = 101;
                }
                else if (nReactionType == 10 && Param3 != "") //사내방송실시(신고)
                {
                    reactionLog.ReactionType = 102;
                }

                //문자메시지(탐지)
                if (nReactionType == (int)libSensorProcess.ReactionType.SEND_SMS)
                {
                    // 복수 메시지에 탐지란 단어가 포함되어 있으므로 복구문자인지 부터 확인
                    if (strMessage.Contains("복구"))
                    {
                        reactionLog.ReactionType = 113;
                    }
                    else if (strMessage.Contains("탐지"))
                    {
                        reactionLog.ReactionType = 111;
                    }
                    else if (strMessage.Contains("신고"))
                    {
                        reactionLog.ReactionType = 112;
                    }
                }

                arrReactionLog.Add(reactionLog);

                List<SensorReactionLog> arrLogs = null;

                //
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

        // 유해화학물질 누출시 발생할 수 있는 알람 Type들을 얻어온다.
        // strVariableName : DB Field 이름
        protected override string GetAlarmTypeQueryString(string strVariableName)
        {
            string strAlarmType =  ((int)UnE.Alarm.AlarmType.PSM_ALARM_1).ToString();
            strAlarmType += "," + ((int)UnE.Alarm.AlarmType.PSM_ALARM_2).ToString();
            strAlarmType += "," + ((int)UnE.Alarm.AlarmType.PSM_ALARM_3).ToString();

            return strVariableName + " in (" + strAlarmType + ")";
        }

        private List<int> FindHistoryID(Zone zone, EquipmentZone equipZone)
        {
            if (equipZone == null)
                return null;

            List<int> arrHistoryIDList = new List<int>();
            List<EquipmentZone> arrEquipmentZoneList = new List<EquipmentZone>();
            arrEquipmentZoneList.Add(equipZone);

            Dictionary<int, ISensor> dicSensorZones = FindSensorZone(arrEquipmentZoneList);

            if (dicSensorZones == null)
                return null;

            List<int> histories = null;

            foreach (KeyValuePair<int, ISensor> pair in dicSensorZones)
            {
                int nSensorID = pair.Key;
                if (m_dicSensorHistories.TryGetValue(nSensorID, out histories))
                    arrHistoryIDList.AddRange(histories);
            }

            foreach (int nHistoryID in arrHistoryIDList)
            {
                if (!m_dicZoneHistories.ContainsKey(nHistoryID))
                    m_dicZoneHistories.Add(nHistoryID, zone);
            }

            return arrHistoryIDList;
        }
    }

    public class StatisticsPSM : Statistics
    {
        // EquipmentZone
        private EquipmentZone equipmentZone = null;
        public EquipmentZone EquipmentZone
        {
            get { return equipmentZone; }
            set { equipmentZone = value; }
        }

        // 물질
        private UnE.PSM.PSMMaterial obMaterial = null;
        public UnE.PSM.PSMMaterial PSMMaterial
        {
            get { return obMaterial; }
            set { obMaterial = value; }
        }
    }

    public class SensorReactionPSMLog : SensorReactionLog
    {
        private UnE.PSM.PSMSensor obSensor = null;
        private UnE.PSM.PSMMaterial obMaterial = null;

        public UnE.PSM.PSMSensor PSMSensor
        {
            get { return obSensor; }
            set { obSensor = value; }
        }

        public UnE.PSM.PSMMaterial PSMMaterial
        {
            get { return obMaterial; }
            set { obMaterial = value; }
        }
    }

    public class DetectPSMLog : DetectLog
    {
        private UnE.PSM.PSMMaterial obMaterial = null;
        private UnE.PSM.PSMSensor obSensor = null;

        private int nAlarmLevel = 0;

        private DateTime dtDetectStart;
        private DateTime dtDetectEnd;

        public UnE.PSM.PSMMaterial PSMMaterial
        {
            get { return obMaterial; }
            set { obMaterial = value; }
        }

        public UnE.PSM.PSMSensor PSMSensor
        {
            get { return obSensor; }
            set { obSensor = value; }
        }

        public int AlarmLevel
        {
            get { return nAlarmLevel; }
            set { nAlarmLevel = value; }
        }

        public DateTime DetectStartDate
        {
            get { return dtDetectStart; }
            set { dtDetectStart = value; }
        }

        public DateTime DetectEndDate
        {
            get { return dtDetectEnd; }
            set { dtDetectEnd = value; }
        }

        protected override int _CompareTo(object b)
        {
            DetectPSMLog data = this;
            DetectPSMLog data2 = (DetectPSMLog)b;

            if (data.Time > data2.Time)
                return 1;
            else if (data.Time < data2.Time)
                return -1;
            else
            {
                if (data.HistoryID < data2.HistoryID)
                    return -1;
                else if (data.HistoryID > data2.HistoryID)
                    return 1;
            }

            return 0;
        }
    }

    public class ReactionPSMLog : ReactionLog
    {
        private UnE.PSM.PSMSensor obSensor;
        public UnE.PSM.PSMSensor PSMSensor
        {
            get { return obSensor; }
            set { obSensor = value; }
        }

        private UnE.PSM.PSMMaterial obMaterial;
        public UnE.PSM.PSMMaterial PSMMaterial
        {
            get { return obMaterial; }
            set { obMaterial = value; }
        }

        //누출 레벨
        private int nLevel = 0;
        public int Level
        {
            get { return nLevel; }
            set { nLevel = value; }
        }

        protected override string _ToString()
        {
            string strReactionType = "";
            if (Type == (int)libSensorProcess.ReactionType.NOTIFY_PSM)
                strReactionType = "누출 발생";
            else if (Type == (int)libSensorProcess.ReactionType.MALFUNCTION || Type == (int)libSensorProcess.ReactionType.PSM_USER_RESET)
                strReactionType = "시스템 복구 처리";
            else if (Type == (int)libSensorProcess.ReactionType.IGNORE_PSM_DETECT || Type == (int)libSensorProcess.ReactionType.END_PSM_STATUS || Type == (int)libSensorProcess.ReactionType.END_STATUS)
                strReactionType = "누출탐지 후 상황해제";
            else if (Type == (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS)
                strReactionType = "누출 탐지";

            return String.Format("{0}   [ 누 출 ] {1} - {2}", String.Format("{0:0000}-{1:00}-{2:00} {3} {4}:{5}", Time.Year, Time.Month, Time.Day, Time.Hour < 12 ? "오전" : "오후", Time.Hour > 12 ? Time.Hour - 12 : Time.Hour, Time.Minute)
                , obMaterial.Name, strReactionType);
        }
    }
}
