using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnE.Spatial;
using DBUtility;
using UnE.Sensor;

namespace libSDMSReport
{
    /// <summary>
    /// 화재신호에 대한 이력처리 클래스
    /// SensorReactionLog.Param1 : Sensor 탐지된 신호일 경우 EquipZone ID, 수동신고일 경우 Zone ID
    /// SensorReactionLog.Param2 : SensorZone ID
    /// SensorReactionLog.Param3 : 이 값은 SensorReactionHistory Table의 ReactionType값에 따라서 각각 다른 의미를 가진다.
    ///                            ReactionType.BEGIN_STATUS : 일반적인 화재센서(지멘스, 동방...) 로부터의 신호일 경우 Param3는 비어있다.
    ///                                                        화재신호가 화재센서가 아닌 외부 시스템으로부터 비롯된 것이라면 이 시스템이 어느 것인지 알려주는 값이 Param3에 정의되어 있다.
    ///                                                        이때의 Param3는 IFacility.FacilityType을 의미한다.
    ///                            ReactionType.IGNORE_FIRE : 현장에서 화재센서가 신호복구 신호를 보내와서 상황이 종료되었을 경우는 빈 문자열,
    ///                                                       수동신고된 화재신호를 특정 사용자가 종료시켰다면, 해당 사용자의 SOPGenUser ID
    ///                            ReactionType.NOTIFY_FIRE : 화재신고를 수행한 사용자의 SOPGenUser ID
    ///                            ReactionType.MALFUNCTION : 오동작 처리한 사용자의 SOPGenUser ID
    /// SensorReactionLog.Param4 : Message(사용 안함)
    /// SensorReactionLog.Param5 : 사용 안함
    /// </summary>
    public class ReactionFireManager : ReactionManager
    {
        // 수동신고된 신호에 대한 Log를 가져온다.
        // arrZoneList : Zone List
        protected override List<SensorReactionLog> GetExternalReactionHistory(ArrayList arrZoneList, string startDate, string endDate)
        {
            if (DBManager == null)
                return null;

            //수동신고 목록을 저장 할 배열
            List<SensorReactionLog> arrManualReactionLog = new List<SensorReactionLog>();

            string strZoneList = "";
            int nCount = 1;
            foreach (Zone zone in arrZoneList)
            {
                strZoneList += zone.ID.ToString();
                if (nCount != arrZoneList.Count)
                    strZoneList += ",";

                nCount++;
            }

            int notify = (int)libSensorProcess.ReactionType.NOTIFY_FIRE;

            // 수동신고는 센서로부터의 신호가 아니기 때문에 SensorZone ID가 존재하지 않는다.
            // 따라서, Param2를 0으로 설정한다.
            string strSQL = "select ID,SensorHistoryID,ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5, DetectionStatus From SensorReactionHistory where SensorHistoryID in "
                     + "(select SensorHistoryID from SensorReactionHistory where param1 in(" + strZoneList + ") And ReactionType = " + notify.ToString() + " And Param2 = 0 And Time Between '" + startDate + "' and '" + endDate + "')";

            ArrayList arrResult = DBManager.GetResultData(strSQL, 0);
            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            DateTime dt = DateTime.Now;

            Dictionary<int, SensorReactionLog> dicReactionLogs = new Dictionary<int, SensorReactionLog>();
            int nMinReactionLogID = -1, nMaxReactionLogID = -1;

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
                reactionLog.Param1 = Param1;
                reactionLog.Message = strMessage;
                reactionLog.Param2 = Param2;
                reactionLog.Param3 = Param3;
                reactionLog.Param4 = Param4;
                reactionLog.Param5 = Param5;
                reactionLog.SetDetectionResult(nDetectionStatus);
                reactionLog.SensorType = (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;

                if (nReactionType == (int)libSensorProcess.ReactionType.MALFUNCTION ||
                     nReactionType == (int)libSensorProcess.ReactionType.NOTIFY_FIRE ||
                     nReactionType == (int)libSensorProcess.ReactionType.IGNORE_FIRE)
                {
                    if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                        m_dicHistoryMember.Add(nSensorHistoryID, Param3);
                }
                
                //사내방송실시, 메시지(탐지/신고 여부)
                //사내방송실시(탐지)
                if (nReactionType == (int)libSensorProcess.ReactionType.RUN_BROADCAST && Param3 == "")
                {
                    reactionLog.ReactionType = (int)libSensorProcess.ReactionType.RUN_DETECT_BROADCAST;
                }
                else if (nReactionType == (int)libSensorProcess.ReactionType.RUN_BROADCAST && Param3 != "") //사내방송실시(신고)
                {
                    reactionLog.ReactionType = (int)libSensorProcess.ReactionType.RUN_REPORT_BROADCAST;
                }

                //문자메시지(탐지)
                if (nReactionType == (int)libSensorProcess.ReactionType.SEND_SMS && strMessage.Contains("탐지"))
                {
                    reactionLog.ReactionType = (int)libSensorProcess.ReactionType.SEND_DETECT_SMS;
                }
                else if (nReactionType == (int)libSensorProcess.ReactionType.SEND_SMS && strMessage.Contains("신고"))
                {
                    reactionLog.ReactionType = (int)libSensorProcess.ReactionType.SEND_REPORT_SMS;
                }

                Zone zone = ZoneManager != null ? ZoneManager.GetZone(Param1) : null;

                if (zone != null)
                {
                    if (!m_dicZoneHistories.ContainsKey(nSensorHistoryID))
                        m_dicZoneHistories.Add(nSensorHistoryID, zone);
                }

                reactionLog.SensorType = (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;

                arrManualReactionLog.Add(reactionLog);

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

            // SensorReactionHistoryDescription 읽어오기
            ReadReactionLogMemo(nMinReactionLogID, nMaxReactionLogID, dicReactionLogs);

            return arrManualReactionLog;
        }

        // SensorReactionHistory DB Table로부터 읽은 Parameter들을 사용하여 SensorReactionLog 객체의 값을 채운다.
        protected override void SetReactionLogParam(SensorReactionLog reactionLog, int nReactionType, int nParam1, string strParam2, string strParam3, string strParam4, string strParam5)
        {
            reactionLog.SensorType = (int)IFacility.FacilityType.FIRE_SENSOR;

            if (nReactionType == (int)libSensorProcess.ReactionType.MALFUNCTION || nReactionType == (int)libSensorProcess.ReactionType.NOTIFY_FIRE || nReactionType == (int)libSensorProcess.ReactionType.IGNORE_FIRE)
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

        // strStartDate와 strEndDate 사이에서 arrZoneList내에 존재하는 모든 화재신호에 대한 통계자료를 작성한다.
        protected override List<Statistics> GetStatisticsLog(ArrayList arrZoneList, string strStartDate, string strEndDate)
        {
            List<Statistics> arrStatistics = new List<Statistics>();
            List<int> liAddedReactionHistoryIDs = new List<int>();

            //Zone별로 Log에서 탐지,화재,오작동,처리되지않은신호의 갯수, 오작동률 등을 구함
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

                int nFireCount = 0;
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
                        if (log.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_STATUS)
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

                        if (log.ReactionType == (int)libSensorProcess.ReactionType.NOTIFY_FIRE)
                        {
                            nFireCount++;
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
                        else if (log.ReactionType == (int)libSensorProcess.ReactionType.IGNORE_FIRE)
                        {
                            nType = log.ReactionType;

                            break;
                        }
                    }

                    if (nType == 0)
                        nCurrentDetectCount++;

                    if (!m_dicHistoryType.ContainsKey(nHistoryID))
                    {
                        m_dicHistoryType.Add(nHistoryID, nType);
                    }
                }

                stat.DetectCount = arrHistoryList.Count;

                //처리되지 않음
                nIgnoreCount = arrHistoryList.Count - (nFireCount + nMalFunctionCount) - nCurrentDetectCount;

                double percentMalFunction = (nMalFunctionCount * 100) / arrHistoryList.Count;

                stat.SensorZoneHistoryIDList = arrHistoryList;
                stat.DetectType = GetReactionString((int)DetectType.FIRE);

                stat.ReportCount = nFireCount;
                stat.UserResetCount = nMalFunctionCount;
                stat.Zone = zone;

                if (m_owner != null)
                    stat.ManagerName = m_owner.FindManagerName(zone, IFacility.FacilityType.FIRE_SENSOR);

                stat.IgnoreCount = nIgnoreCount;
                stat.PercentUserReset = percentMalFunction;
                stat.CurrentDetectCount = nCurrentDetectCount;

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
            //오작동 이력 로그들을 배열에 저장
            return arrStatistics;
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
                    List<ISensor> arSensors = SensorManager.FindZoneInSensor(equip.ID, IFacility.FacilityType.FIRE_SENSOR);

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

        // arrAllLog 중에서 화재탐지 신호만을 추려낸다.
        protected override List<DetectLog> GetDetectLog(List<SensorReactionLog> arrAllLog)
        {
            List<DetectLog> arrDetectLog = new List<DetectLog>();
            ArrayList arrComboBoxDate = new ArrayList();
            //ArrayList arrReactionLog = new ArrayList();

            foreach (SensorReactionLog reactionLog in arrAllLog)
            {
                // 자탐신호이거나 수동신고이거나 S1Access, SVMS 화재인 경우 추출
                if (reactionLog.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_STATUS || (reactionLog.ReactionType == (int)libSensorProcess.ReactionType.NOTIFY_FIRE && reactionLog.Param2 == "0")
                    || IsS1AccessFire(reactionLog) || IsSVMSFire(reactionLog))
                {
                    DetectLog detect = new DetectLog();

                    detect.SensorReactionHistoryID = reactionLog.ID;
                    detect.HistoryID = reactionLog.SensorZoneHistoryID;
                    detect.Time = reactionLog.Time;
                    detect.Memo = reactionLog.Memo;

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

                    //EquipZone표시는 자탐일때만.. 수동신고일때는 알 수 없다.
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
                    string strManagerName = m_owner != null ? m_owner.FindManagerName(zone, IFacility.FacilityType.FIRE_SENSOR) : "";
                    detect.ManagerName = strManagerName;

                    //자탐
                    if (reactionLog.ReactionType == 0)
                    {
                        int nDetectType;

                        // 일반적인 화재센서(지멘스, 동방...) 로부터의 신호가 아니라 외부 시스템에서 알려주는 화재신호일 경우
                        // 이 화재신호가 어느 시스템으로부터 비롯된 것인지 알려주는 값이 Param3에 정의되어 있다.
                        // Param3 : IFacility.FacilityType
                        if (int.TryParse(reactionLog.Param3, out nDetectType))
                        {
                            detect.DetectType = SOPServer.EventTypeString.GetEventTypeDetectString(nDetectType/*Convert.ToInt32(reactionLog.Param3)*/);
                        }
                        else
                            detect.DetectType = GetReactionString((int)DetectType.FIRE);
                    }
                    else//수동신고
                        detect.DetectType = GetReactionString((int)DetectType.MANUAL);

                    detect.DetectionStatusName = GetDetectionStatusName(reactionLog.DetectionResult);

                    arrDetectLog.Add(detect);
                }
            }
            //
            //arrDetectLog.Sort();
            return arrDetectLog;
        }

        protected override ReactionLog _GetReactionLog(SensorReactionLog sensorReactionLog, List<SensorReactionLog> sensorReactionLogList, int nSensorZoneHistoryID, Zone zone)
        {
            ReactionFireLog reactionLog = new ReactionFireLog();
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
                || sensorReactionLog.ReactionType == (int)libSensorProcess.ReactionType.NOTIFY_FIRE
                || sensorReactionLog.ReactionType == (int)libSensorProcess.ReactionType.IGNORE_FIRE)
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

        protected override ReactionLog _HistorySubmit(SensorReactionLog sensorReactionLog, List<SensorReactionLog> sensorReactionLogList, int nSensorZoneHistoryID, Zone zone)
        {
            ReactionFireLog reactionLog = new ReactionFireLog();
            reactionLog.SensorZoneHistoryID = nSensorZoneHistoryID;
            reactionLog.SensorReactionLogList = sensorReactionLogList;

            int nReactionType = 0;

            //자탐 ReactionType가져옴
            if (m_dicHistoryType.ContainsKey(nSensorZoneHistoryID))
            {
                nReactionType = m_dicHistoryType[nSensorZoneHistoryID];
            }

            reactionLog.SensorType = (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;

            //오작동인지 화재인지, 무시된 신호인지 구분하기위함(수동신고)
            //if (reactionLog.SensorType == (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
            {
                foreach (SensorReactionLog Typelog in sensorReactionLogList)
                {
                    if (Typelog.ReactionType == (int)libSensorProcess.ReactionType.NOTIFY_FIRE ||
                        Typelog.ReactionType == (int)libSensorProcess.ReactionType.MALFUNCTION ||
                        Typelog.ReactionType == (int)libSensorProcess.ReactionType.IGNORE_FIRE)
                    {
                        nReactionType = Typelog.ReactionType;
                        break;
                    }
                    nReactionType = Typelog.ReactionType;
                }
            }

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
                reactionLog.ManagerName = m_owner.FindManagerName(zone, IFacility.FacilityType.FIRE_SENSOR);

            return reactionLog;
        }

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

        private bool IsS1AccessFire(SensorReactionLog log)
        {
            if (log.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS)
            {
                if (log.Message.Contains("화재"))
                    return true;
            }

            return false;
        }

        private bool IsSVMSFire(SensorReactionLog log)
        {
            if (log.ReactionType == (int)libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS)
            {
                if (log.Message.Contains("화재"))
                    return true;
            }

            return false;
        }
    }

    public class ReactionFireLog : ReactionLog
    {
        protected override string _ToString()
        {
            string strReactionType = string.Empty;
            if (Type == (int)libSensorProcess.ReactionType.NOTIFY_FIRE)
                strReactionType = "화재 발생";
            else if (Type == (int)libSensorProcess.ReactionType.MALFUNCTION)
                strReactionType = "오작동 처리";
            else if (Type == (int)libSensorProcess.ReactionType.IGNORE_FIRE)
                strReactionType = "화재탐지 후 상황해제";
            else if (Type == (int)libSensorProcess.ReactionType.BEGIN_STATUS)
                strReactionType = "화재 탐지";
            else if (Type == (int)libSensorProcess.ReactionType.NOTIFY_SECURITY)
                strReactionType = "방범 센서 탐지";
            else if (Type == (int)libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS)
                strReactionType = "SVMS 탐지 후 상황해제";
            else if (Type == (int)libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS)
                strReactionType = "ACCESS 탐지 후 상황해제";
            else if (Type == (int)libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS)
                strReactionType = "SVMS 센서 탐지";
            else if (Type == (int)libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS)
                strReactionType = "ACCESS 센서 탐지";

            if (SensorType == (int)IFacility.FacilityType.FIRE_SENSOR)
            {
                int nfacilityType;

                if (int.TryParse(FacilityType, out nfacilityType) && (nfacilityType == (int)IFacility.FacilityType.Fire_S1 || nfacilityType == (int)IFacility.FacilityType.FireF1_S1))
                {
                    //화재뿐이라 화재라는 단어가 필요없음
                    return String.Format("{0:0000}-{1:00}-{2:00} {3} {4:00}:{5:00}", Time.Year, Time.Month, Time.Day, Time.Hour < 12 ? "오전" : "오후", Time.Hour > 12 ? Time.Hour - 12 : Time.Hour, Time.Minute)
                        + "   [ " + SOPServer.EventTypeString.GetEventTypeDetectString(nfacilityType).Replace("화재", "") + " 수동 신고 ] " + strReactionType;
                }
                else
                {
                    return String.Format("{0:0000}-{1:00}-{2:00} {3} {4:00}:{5:00}", Time.Year, Time.Month, Time.Day, Time.Hour < 12 ? "오전" : "오후", Time.Hour > 12 ? Time.Hour - 12 : Time.Hour, Time.Minute)
                        + "   [ 수동 신고 ] " + strReactionType;
                }
            }
            else
            {
                int nfacilityType;

                if (int.TryParse(FacilityType, out nfacilityType) && (nfacilityType == (int)IFacility.FacilityType.Fire_S1 || nfacilityType == (int)IFacility.FacilityType.FireF1_S1))
                {
                    //화재뿐이라 화재라는 단어가 필요없음
                    return String.Format("{0:0000}-{1:00}-{2:00} {3} {4:00}:{5:00}", Time.Year, Time.Month, Time.Day, Time.Hour < 12 ? "오전" : "오후", Time.Hour > 12 ? Time.Hour - 12 : Time.Hour, Time.Minute)
                        + "   [ " + SOPServer.EventTypeString.GetEventTypeDetectString(nfacilityType).Replace("화재", "") + " ] " + strReactionType;
                }
                else
                {
                    return String.Format("{0:0000}-{1:00}-{2:00} {3} {4:00}:{5:00}", Time.Year, Time.Month, Time.Day, Time.Hour < 12 ? "오전" : "오후", Time.Hour > 12 ? Time.Hour - 12 : Time.Hour, Time.Minute)
                        + "   [ 자 탐 ] " + strReactionType;
                }
            }
        }
    }
}
