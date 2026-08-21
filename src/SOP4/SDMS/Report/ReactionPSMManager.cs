using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;


namespace SDMS
{
    namespace Report
    {
        public class ReactionPSMManager
        {
            public class RefreshCheckData
            {
                private Dictionary<int, Zone> m_dicPrevZones = new Dictionary<int, Zone>();
                private DateTime m_dtBefore = new DateTime();
                private DateTime m_dtCurrent = new DateTime();
                // 마지막으로 읽은 SensorReactionHistoryID
                private int m_nReadLastSensorReactionHistoryID = -1;
                private int m_nViewCount = 20;

                public Dictionary<int, Zone> DicPrevZones
                {
                    get { return m_dicPrevZones; }
                }

                public DateTime BeforeTime
                {
                    get { return m_dtBefore; }
                    set { m_dtBefore = value; }
                }

                public DateTime CurrentTime
                {
                    get { return m_dtCurrent; }
                    set { m_dtCurrent = value; }
                }

                public int ReadLastSensorReactionHistoryID
                {
                    get { return m_nReadLastSensorReactionHistoryID; }
                    set { m_nReadLastSensorReactionHistoryID = value; }
                }

                public int ViewCount
                {
                    get { return m_nViewCount; }
                    set { m_nViewCount = value; }
                }
            }

            // 누출탐지 리스트
            private ArrayList m_arrDectectList = null;
            public ArrayList DectectList
            {
                get { return m_arrDectectList; }
                set { m_arrDectectList = value; }
            }

            // 오작동 리스트
            private ArrayList m_arrMulFunctionList = null;
            public ArrayList MulFunctionList
            {
                get { return m_arrMulFunctionList; }
                set { m_arrMulFunctionList = value; }
            }

            private ArrayList m_arrReactionHistory = new ArrayList();

            //HistoryID,ReactionLog
            private Dictionary<int, ArrayList> m_dicHistoryLog = new Dictionary<int, ArrayList>();
            //SensorID,HistoryID List
            private Dictionary<int, ArrayList> m_dicSensorHistorys = new Dictionary<int, ArrayList>();

            //HistoryID, Zone
            private Dictionary<int, Zone> m_dicZoneHistorys = new Dictionary<int, Zone>();

            //HistoryID, ReactionType
            private Dictionary<int, int> m_dicHistoryType = new Dictionary<int, int>();

            //MulFunctionPSMLog, SensorReactionLogList
            private Dictionary<MulFunctionPSMLog, ArrayList> m_dicMulFuctionSrLog = new Dictionary<MulFunctionPSMLog, ArrayList>();
            internal Dictionary<MulFunctionPSMLog, ArrayList> DicMulFuctionSrLog
            {
                get { return m_dicMulFuctionSrLog; }
                set { m_dicMulFuctionSrLog = value; }
            }

            private ArrayList arrAllReactionLog = new ArrayList();

            //누출을 신고한곳<MemberID, NicName>
            private Dictionary<string, string> m_dicGenUserIDDNicName = new Dictionary<string, string>();
            public Dictionary<string, string> DicGenUserIDDNicName
            {
                get { return m_dicGenUserIDDNicName; }
                set { m_dicGenUserIDDNicName = value; }
            }

            //HistoryID, Param3(MemberID)
            private Dictionary<int, string> m_dicHistoryMember = new Dictionary<int, string>();

            private int m_nSiteID = 1;
            public ReactionPSMManager()
            {
                m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
                m_arrDectectList = new ArrayList();
            }

            public void DataClear()
            {
                if (m_arrDectectList != null)
                    m_arrDectectList.Clear();

                if (m_arrMulFunctionList != null)
                    m_arrMulFunctionList.Clear();

                if (m_dicHistoryLog != null)
                    m_dicHistoryLog.Clear();

                if (m_dicSensorHistorys != null)
                    m_dicSensorHistorys.Clear();

                if (m_dicZoneHistorys != null)
                    m_dicZoneHistorys.Clear();

                if (m_dicHistoryType != null)
                    m_dicHistoryType.Clear();

                if (arrAllReactionLog != null)
                    arrAllReactionLog.Clear();

                if (m_dicMulFuctionSrLog != null)
                    m_dicMulFuctionSrLog.Clear();

                if (m_arrReactionHistory != null)
                    m_arrReactionHistory.Clear();

            }

            private ArrayList AddReactionHistoryLog(ArrayList arrManualReactionHistory, ArrayList arrReactionList)
            {
                ArrayList arrAllReactionLog = new ArrayList();

                if (arrReactionList != null)
                    arrAllReactionLog.AddRange(arrReactionList);
                if (arrManualReactionHistory != null)
                    arrAllReactionLog.AddRange(arrManualReactionHistory);

                return arrAllReactionLog;
            }

            //탐지, 처리이력
            public void ZoneSubmit(ArrayList arrZoneList, DateTime startDate, DateTime endDate,bool isActionPage = false)// [isActionPage] true : 대응이력 페이지 , false : 탐지/처리 이력 페이지
            {
                LoadSOPGenUser();

                string strNowDate, strBeforeDate;
                GetZoneSumitDate(isActionPage, startDate, endDate, out strBeforeDate, out strNowDate);

                //ZoneID 리스트로 ReactionHistory의 수동신고의 log를 가져온다.
                ArrayList arrManualReactionHistory = GetManualReactionHistory(arrZoneList, strBeforeDate, strNowDate);


                //선택한 ZoneID 리스트로 EquipmentZoneID를 찾는다.
                ArrayList arrEquipmentZoneList = FindEquipZone(arrZoneList);
                //가져온 EquipmentZoneID 리스트로 SensorID를 찾아온다.
                Dictionary<int, ISensor> dicSensorZones = FindSensorZone(arrEquipmentZoneList);
                //ArrayList arrSensorZoneList = FindSensorZone(arrEquipmentZoneList);
                //SensorID리스트로 SensorHistoryID를 찾아옴
                ArrayList arrZoneHistoryList = GetSensorZoneHistoryID(dicSensorZones, strBeforeDate, strNowDate);
                //ArrayList arrZoneHistoryList = GetSensorZoneHistoryID(arrSensorZoneList, strBeforeDate, strNowDate);
                //ReactionLog를 가져옴
                ArrayList arrReactionList = GetReactionHistory(arrZoneHistoryList);

                //수동신고와 누출의 SensorReactionPSMLog를 합친다.
                arrAllReactionLog = new ArrayList();
                arrAllReactionLog = AddReactionHistoryLog(arrManualReactionHistory, arrReactionList);

                //오작동이력 로그 저장
                m_arrMulFunctionList = GetMulFunctionLog(arrZoneList, strBeforeDate, strNowDate);


                //전체 ReactionLog중에 화재 탐지 된 로그만 가져와서 저장함
                //화재신고 된 로그만 가져옴(ReactionType=60 -> 누출탐지 / reactionLog.ReactionType == 63 && reactionLog.Param2 == "0" -> 수동
                m_arrDectectList = GetDetectLog(arrAllReactionLog);
                m_arrDectectList.Sort();
            }

            private void GetZoneSumitDate(bool isActionPage, DateTime startDate, DateTime endDate, out string strBeforeDate, out string strNowDate)
            {
                strNowDate = "";
                strBeforeDate = string.Format("{0} {1}:{2}:{3}", startDate.ToShortDateString(), "00", "00", "00");

                if (isActionPage == true)//대응이력은 시작날과 종료날이 같을경우 시간까지 조절해야하므로 ..
                {
                    if (startDate.ToShortDateString() == endDate.ToShortDateString())
                    {
                        strNowDate = string.Format("{0} {1}:{2}:{3}", endDate.ToShortDateString(), endDate.Hour, endDate.Minute, endDate.Second);
                    }
                    else
                    {
                        strNowDate = string.Format("{0} {1}:{2}:{3}", endDate.ToShortDateString(), 23, 59, 59);
                    }
                }
                else
                {
                    DateTime dtNow = DateTime.Now;

                    //검색에 오늘날짜가 들어가면 현재 시간까지만 검사
                    if (endDate.ToShortDateString() == dtNow.ToShortDateString())
                    {
                        // 서버와 시간차이가 날수도 있으니 Client의 현재 시간보다 1시간 뒤로 설정한다.
                        dtNow = dtNow.AddHours(1.0);
                        strNowDate = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);
                        //strNowDate = string.Format("{0} {1}:{2}:{3}", endDate.ToShortDateString(), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    }
                    else//아니면 23시 59분59분까지 검사
                    {
                        strNowDate = string.Format("{0} {1}:{2}:{3}", endDate.ToShortDateString(), 23, 59, 59);
                    }
                }
            }

            // 이전과 같은 데이터인지 검사한다.
            // Return 값 : true이면 이전 데이터와 다르다.
            //             false이면 Refresh 필요없다.
            public bool NeedRefresh(ArrayList arrZoneList, DateTime startDate, DateTime endDate, RefreshCheckData checkData, bool isActionPage = false)
            {
                string strBeforeDate, strNowDate;
                GetZoneSumitDate(isActionPage, startDate, endDate, out strBeforeDate, out strNowDate);

                int nSensorHistoryID = GetMaxSensorReactionHistoryID();

                DateTime dtBefore, dtCurrent;

                if (!DateTime.TryParse(strBeforeDate, out dtBefore) || !DateTime.TryParse(strNowDate, out dtCurrent))
                    return true;

                bool allZones = ZoneManager.Instance.DicZones.Count == arrZoneList.Count;

                bool isSameZones = false;

                if (CheckPrevZones(allZones, arrZoneList, checkData.DicPrevZones))
                {
                    isSameZones = true;
                }
                else
                {
                    checkData.DicPrevZones.Clear();

                    foreach (Zone zone in arrZoneList)
                    {
                        checkData.DicPrevZones[zone.ID] = zone;
                    }
                }

                if (isSameZones)
                {
                    if (checkData.BeforeTime == dtBefore && checkData.CurrentTime == dtCurrent)
                        return false;
                    else if (checkData.BeforeTime == dtBefore &&
                        checkData.CurrentTime.Year == dtCurrent.Year && checkData.CurrentTime.Month == dtCurrent.Month && checkData.CurrentTime.Day == dtCurrent.Day &&
                        checkData.ReadLastSensorReactionHistoryID == nSensorHistoryID)
                    {
                        // 이전 검색조건과 모두 일치하면서, CurrentTime의 시간만 다를 경우
                        // 이렇게 하는 이유는 EndTime이 현재날짜가 아닐수도 있기 때문
                        return false;
                    }
                }

                checkData.ReadLastSensorReactionHistoryID = nSensorHistoryID;

                checkData.BeforeTime = dtBefore;
                checkData.CurrentTime = dtCurrent;
                return true;
            }

            private int GetMaxSensorReactionHistoryID()
            {
                string strSQL = "Select max(ID) from SensorReactionHistory";
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count == 0)
                    return -1;

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return -1;

                return id.Data;
            }

            // Return 값 : true이면 이전 값과 같다.
            private bool CheckPrevZones(bool allZones, ArrayList arrZones, Dictionary<int, Zone> dicPrevZones)
            {
                if (allZones)
                {
                    // 모든 Zone일 경우 Zone의 개수만 비교하면 된다.
                    if (arrZones.Count == dicPrevZones.Count)
                        return true;
                }
                else
                {
                    if (arrZones.Count != dicPrevZones.Count)
                        return false;

                    foreach (Zone zone in arrZones)
                    {
                        if (!dicPrevZones.ContainsKey(zone.ID))
                            return false;
                    }

                    return true;
                }

                return false;
            }

            public void LoadSOPGenUser()
            {
                WebDBManager webDB = FormMain.Instance.DBManager;

                string strSQL = "select ID, NickName From SOPGenUser WHERE SiteID = " + m_nSiteID.ToString();

                ArrayList arrResult = webDB.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;
                DateTime dt = DateTime.Now;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    int nMemberID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strNicName = WebDBManager.GetStringField(arrResult[i + 1], "");

                    if (!m_dicGenUserIDDNicName.ContainsKey(nMemberID.ToString()))
                        m_dicGenUserIDDNicName[nMemberID.ToString()] = strNicName;
                }
            }

            public ArrayList HistorySubmit(DateTime startDate, DateTime endDate)
            {
                m_arrReactionHistory.Clear();


                endDate = endDate.AddDays(1);
                foreach (KeyValuePair<int, ArrayList> pair in m_dicHistoryLog)
                {
                    int nHistoryID = pair.Key;
                    ArrayList log = pair.Value;
                    int nReactionType = 0;
                    Zone zone = null;
                    string strMemberID = "";



                    if (m_dicZoneHistorys.ContainsKey(nHistoryID))
                        zone = m_dicZoneHistorys[nHistoryID];

                    ReactionPSMLog reactionLog = new ReactionPSMLog();
                    reactionLog.HistoryID = nHistoryID;
                    reactionLog.ArrLogList = log;

                    //누출 ReactionType가져옴
                    if (m_dicHistoryType.ContainsKey(nHistoryID))
                    {
                        nReactionType = m_dicHistoryType[nHistoryID];
                        reactionLog.SensorType = (int)IFacility.FacilityType.PSM_SENSOR;
                    }
                    else
                        reactionLog.SensorType = 0;

                    if (reactionLog.SensorType == 0)
                    {
                        //오작동인지 화재인지, 무시된 신호인지 구분하기위함(수동신고)
                        if (reactionLog.SensorType == 0)
                        {
                            foreach (SensorReactionPSMLog Typelog in log)
                            {
                                if (Typelog.ReactionType == (int)ReactionType.NOTIFY_PSM)
                                {
                                    nReactionType = (int)ReactionType.NOTIFY_PSM;
                                    break;
                                }
                                else if (Typelog.ReactionType == (int)ReactionType.MALFUNCTION)
                                {
                                    nReactionType = (int)ReactionType.MALFUNCTION;
                                    break;
                                }
                                else if (Typelog.ReactionType == (int)ReactionType.IGNORE_PSM_DETECT)
                                {
                                    nReactionType = (int)ReactionType.IGNORE_PSM_DETECT;
                                    break;
                                }
                                nReactionType = Typelog.ReactionType;
                            }
                        }
                    }


                    //가장 맨 처음 발생한 ReactionLog를 Comobox로 보여줘야 하므로 log배열의 가장 첫번째 값을 가져온다
                    SensorReactionPSMLog sensorreactionLog = (SensorReactionPSMLog)log[0];

                    int nLevel = -1;
                    if (int.TryParse(sensorreactionLog.Param5, out nLevel))
                    {
                        reactionLog.Level = nLevel;
                    }

                    if (!(sensorreactionLog.Time >= startDate && sensorreactionLog.Time <= endDate))
                    {
                        continue;
                    }


                    if (m_dicHistoryMember.ContainsKey(nHistoryID))
                        strMemberID = m_dicHistoryMember[nHistoryID];

                    if (m_dicGenUserIDDNicName.ContainsKey(strMemberID))
                        reactionLog.UserName = m_dicGenUserIDDNicName[strMemberID];

                    reactionLog.Time = sensorreactionLog.Time;
                    reactionLog.SensorType = sensorreactionLog.SensorType;
                    reactionLog.Zone = zone;
                    reactionLog.ManagerName = FindManagerName(zone);
                    reactionLog.Type = nReactionType;
                    reactionLog.PSMSensor = sensorreactionLog.PSMSensor;
                    reactionLog.PSMMaterial = sensorreactionLog.PSMMaterial;

                    m_arrReactionHistory.Add(reactionLog);
                }
                m_arrReactionHistory.Sort();
                return m_arrReactionHistory;
            }

            public ArrayList GetReactionLog(int nHistoryID)
            {
                ArrayList arrReactLog = new ArrayList();

                foreach (KeyValuePair<int, ArrayList> pair in m_dicHistoryLog)
                {
                    int nSensorHistoryID = pair.Key;
                    ArrayList log = pair.Value;
                    string strMemberID = "";

                    if (nSensorHistoryID == nHistoryID)
                    {
                        Zone zone = null;
                        if (m_dicZoneHistorys.ContainsKey(nHistoryID))
                            zone = m_dicZoneHistorys[nHistoryID];

                        foreach (SensorReactionPSMLog srLog in log)
                        {
                            ReactionPSMLog reactionLog = new ReactionPSMLog();
                            reactionLog.HistoryID = nHistoryID;
                            reactionLog.ArrLogList = log;

                            if (m_dicHistoryMember.ContainsKey(nHistoryID))
                                strMemberID = m_dicHistoryMember[nHistoryID];

                            if (m_dicGenUserIDDNicName.ContainsKey(strMemberID))
                                reactionLog.UserName = m_dicGenUserIDDNicName[strMemberID];


                            //자탐은 param1
                            reactionLog.equipZone = ZoneManager.Instance.GetEquipZone(srLog.Param1);

                            reactionLog.Time = srLog.Time;
                            reactionLog.SensorType = srLog.SensorType;
                            reactionLog.Zone = zone;
                            reactionLog.ManagerName = FindManagerName(zone);
                            reactionLog.Type = srLog.ReactionType;
                            reactionLog.PSMSensor = srLog.PSMSensor;
                            reactionLog.PSMMaterial = srLog.PSMMaterial;

                            int nLevel = -1;
                            if (int.TryParse(srLog.Param5, out nLevel))
                            {
                                reactionLog.Level = nLevel;
                            }

                            arrReactLog.Add(reactionLog);
                        }
                        break;
                    }
                }

                // arrReactLog.Sort();
                return arrReactLog;
            }

            private ArrayList GetDetectLog(ArrayList arrAllLog)
            {
                ArrayList arrDetectLog = new ArrayList();
                ArrayList arrComboBoxDate = new ArrayList();
                //ArrayList arrReactionLog = new ArrayList();

                foreach (SensorReactionPSMLog reactionLog in arrAllLog)
                {
                    if (reactionLog.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS ||
                        (reactionLog.ReactionType == (int)ReactionType.NOTIFY_PSM && reactionLog.Param2 == "0"))
                    {
                        DetectPSMLog detect = new DetectPSMLog();

                        detect.SensorReactionHistoryID = reactionLog.ID;
                        detect.HistoryID = reactionLog.SensorHistoryID;
                        detect.Time = reactionLog.Time;
                        detect.PSMMaterial = reactionLog.PSMMaterial;
                        detect.PSMSensor = reactionLog.PSMSensor;
                        detect.Memo = reactionLog.Memo;

                        foreach (SensorReactionPSMLog item in from items in arrAllLog.Cast<SensorReactionPSMLog>()
                                                              where items.SensorHistoryID == detect.HistoryID
                                                              orderby items.Time ascending
                                                              select items
                                                              )
                        {
                            detect.DetectStartDate = item.Time.AddMinutes(-10);
                            break;
                        }

                        foreach (SensorReactionPSMLog item in from items in arrAllLog.Cast<SensorReactionPSMLog>()
                                                              where items.SensorHistoryID == detect.HistoryID
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
                            zone = ZoneManager.Instance.GetZone(reactionLog.Param1);
                            detect.zoneID = reactionLog.Param1;
                        }
                        else
                        {
                            if (m_dicZoneHistorys.ContainsKey(reactionLog.SensorHistoryID))
                                zone = m_dicZoneHistorys[reactionLog.SensorHistoryID];

                            detect.zoneID = zone.ID;
                        }

                        string szBuildingName = zone.Building != null ? zone.Building.BuildingName : "";
                        string szGroupName = szBuildingName != "" ? zone.Building.BuildingGroup.BuildingGroupName : "";
                        string strFloorIndex = zone.Floor != null ? zone.Floor.ToString() : "";


                        //EquipZone표시는 자탐일때만.. 수동신고일때는 알 수 없다.
                        if (reactionLog.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS)
                        {
                            //EquipZone구하기
                            detect.EquipZone = ZoneManager.Instance.GetEquipZone(reactionLog.Param1);
                        }

                        if (reactionLog.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS ||
                            reactionLog.ReactionType == (int)ReactionType.CHANGE_PSM_ALARM_DEPTH)
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

                        string strManagerName = FindManagerName(zone);
                        detect.ManagerName = strManagerName;

                        //누출
                        detect.DetectType = GetReactionString(7);
                        /*switch (reactionLog.ReactionType)
                        {
                            case (int)ReactionType.BEGIN_PSM_STATUS:
                            case (int)ReactionType.IGNORE_PSM_DETECT:
                            case (int)ReactionType.NOTIFY_PSM:
                            case (int)ReactionType.CHANGE_PSM_ALARM_DEPTH:
                            case (int)ReactionType.END_PSM_STATUS:
                                detect.DetectType = GetReactionString(7);
                                break;
                            default:
                                detect.DetectType = GetReactionString(1);
                                break;
                        }*/

                        detect.DetectionStatusName = GetDetectionStatusName(reactionLog.DetectionStatus);


                        arrDetectLog.Add(detect);
                    }
                }
                //
                //arrDetectLog.Sort();
                return arrDetectLog;
            }

            private ArrayList GetMulFunctionLog(ArrayList arrZoneList, string strStartDate, string strEndDate)
            {
                ArrayList arrMulFunction = new ArrayList();
                List<int> liAddedReactionHistoryIDs = new List<int>();

                //Zone별로 Log에서 누출,오작동,처리되지않은신호의 갯수, 오작동률 등을 구함
                foreach (Zone zone in arrZoneList)
                {
                    if (ZoneManager.Instance.GetEquipmentZoneList(zone) == null)
                        continue;

                    foreach (EquipmentZone equipZone in ZoneManager.Instance.GetEquipmentZoneList(zone))
                    //foreach (EquipmentZone equipZone in ZoneManager.Instance.GetEquipmentZoneList(zone).ToArray(typeof(EquipmentZone)))
                    {
                        ArrayList arrHistoryList = FindHistoryID(zone, equipZone);
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
                        MulFunctionPSMLog mulfuction = new MulFunctionPSMLog();

                        int nPSMCount = 0;
                        int nMulFunctionCount = 0;
                        int nNotprocessCount = 0;
                        int nOnlyDetectCount = 0;

                        foreach (int nHistoryID in arrHistoryList)
                        {
                            ArrayList arrLog = new ArrayList();

                            if (m_dicHistoryLog.ContainsKey(nHistoryID))
                                arrLog = m_dicHistoryLog[nHistoryID];

                            int nType = (int)ReactionType.BEGIN_PSM_STATUS;

                            foreach (SensorReactionPSMLog log in arrLog)
                            {
                                mulfuction.PSMMaterial = log.PSMMaterial;

                                if (log.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS)
                                {
                                    ArrayList arrSensorLog = null;


                                    //<MulFunctionPSMLog, SensorReactionPSMLog> Dictionary에 값 추가
                                    if (m_dicMulFuctionSrLog.ContainsKey(mulfuction))
                                        arrSensorLog = m_dicMulFuctionSrLog[mulfuction];
                                    else
                                    {
                                        arrSensorLog = new ArrayList();
                                        m_dicMulFuctionSrLog[mulfuction] = arrSensorLog;
                                    }
                                    arrSensorLog.Add(log);
                                }

                                if (log.ReactionType == (int)ReactionType.NOTIFY_PSM)
                                {
                                    nPSMCount++;
                                    nType = (int)ReactionType.NOTIFY_PSM;

                                    break;
                                }
                                else if (log.ReactionType == (int)ReactionType.MALFUNCTION || log.ReactionType == (int)ReactionType.PSM_USER_RESET)
                                {
                                    nMulFunctionCount++;
                                    nType = (int)ReactionType.PSM_USER_RESET;

                                    break;
                                }
                                else if (log.ReactionType == (int)ReactionType.IGNORE_PSM_DETECT || log.ReactionType == (int)ReactionType.END_STATUS)
                                {
                                    nType = (int)ReactionType.IGNORE_PSM_DETECT;

                                    break;
                                }
                            }

                            if (nType == (int)ReactionType.BEGIN_PSM_STATUS)
                            {
                                nOnlyDetectCount++;
                            }

                            if (!m_dicHistoryType.ContainsKey(nHistoryID))
                            {
                                m_dicHistoryType.Add(nHistoryID, nType);
                            }

                        }

                        mulfuction.ReactionCount = arrHistoryList.Count;

                        //처리되지 않음
                        nNotprocessCount = arrHistoryList.Count - (nPSMCount + nMulFunctionCount) - nOnlyDetectCount;

                        double PercentMulFunction = (nMulFunctionCount * 100) / arrHistoryList.Count;

                        mulfuction.HistoryIDList = arrHistoryList;
                        mulfuction.DetectType = GetReactionString(7);

                        mulfuction.NotifyCount = nPSMCount;
                        mulfuction.MulFunctionCount = nMulFunctionCount;
                        mulfuction.Zone = zone;
                        mulfuction.EquipmentZone = equipZone;


                        mulfuction.ManagerName = FindManagerName(zone);
                        mulfuction.Notprocess = nNotprocessCount;
                        mulfuction.OnlyDetectCount = nOnlyDetectCount;
                        mulfuction.PercentMulFunction = PercentMulFunction;

                        string szBuildingName = zone.Building != null ? zone.Building.DisplayText : "";
                        string szGroupName = szBuildingName != "" ? zone.Building.BuildingGroup.BuildingGroupName : "";
                        string strFloorIndex = zone.Floor != null ? zone.Floor.ToString() : "";

                        if (szGroupName == "")
                            mulfuction.GroupName = "외부 영역";
                        else
                            mulfuction.GroupName = szGroupName;

                        if (szBuildingName == "")
                            mulfuction.BuildingName = zone.DisplayText;
                        else
                            mulfuction.BuildingName = szBuildingName;

                        mulfuction.FloorName = strFloorIndex;

                        arrMulFunction.Add(mulfuction);
                    }
                }
                //오작동이력로그들을 배열에 저장
                return arrMulFunction;
            }

            //ZoneID로 ReactionHistory의 수동신고Log를 가져온다
            private ArrayList GetManualReactionHistory(ArrayList arrZoneList, string startDate, string endDate)
            {
                //수동신고 목록을 저장 할 배열
                ArrayList arrManualReactionLog = new ArrayList();

                string strZoneList = "";
                int nCount = 1;
                foreach (Zone zone in arrZoneList)
                {
                    strZoneList += zone.ID.ToString();
                    if (nCount != arrZoneList.Count)
                        strZoneList += ",";

                    nCount++;
                }

                WebDBManager webDB = FormMain.Instance.DBManager;

                string strSQL = "select ID,SensorHistoryID,ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5, DetectionStatus From SensorReactionHistory where SensorHistoryID in "
                         + "(select SensorHistoryID from SensorReactionHistory where param1 in(" + strZoneList + ") And ReactionType = 63 And Param2 = 0 And Time Between '" + startDate + "' and '" + endDate + "')";

                ArrayList arrResult = webDB.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return null;

                int nResultCount = arrResult.Count;
                DateTime dt = DateTime.Now;

                Dictionary<int, SensorReactionPSMLog> dicReactionLogs = new Dictionary<int, SensorReactionPSMLog>();
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

                    SensorReactionPSMLog reactionLog = new SensorReactionPSMLog();
                    reactionLog.ID = nID;
                    reactionLog.SensorHistoryID = nSensorHistoryID;
                    reactionLog.ReactionType = nReactionType;
                    reactionLog.Time = time;
                    reactionLog.Param1 = Param1;
                    reactionLog.Message = strMessage;
                    reactionLog.Param2 = Param2;
                    reactionLog.Param3 = Param3;
                    reactionLog.Param4 = Param4;
                    reactionLog.Param5 = Param5;
                    reactionLog.DetectionStatus = nDetectionStatus;
                    reactionLog.SensorType = 11;

                    if (nReactionType == (int)ReactionType.IGNORE_PSM_DETECT || nReactionType == (int)ReactionType.NOTIFY_PSM || nReactionType == (int)ReactionType.PSM_USER_RESET)
                    {
                        if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                            m_dicHistoryMember[nSensorHistoryID] = Param3;
                    }
                    /*if (nReactionType == (int)ReactionType.NOTIFY_PSM ||
                        nReactionType == (int)ReactionType.MALFUNCTION ||
                        nReactionType == (int)ReactionType.IGNORE_PSM_DETECT ||
                        nReactionType == (int)ReactionType.CHANGE_PSM_ALARM_DEPTH)
                    {
                        if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                            m_dicHistoryMember.Add(nSensorHistoryID, Param3);
                    }*/


                    //사내방송실시, 메시지(탐지/신고 여부)
                    //사내방송실시(탐지)
                    if (nReactionType == (int)SDMS.ReactionType.RUN_BROADCAST && Param3 == "")
                    {
                        reactionLog.ReactionType = (int)SDMS.ReactionType.RUN_DETECT_BROADCAST;
                    }
                    else if (nReactionType == (int)SDMS.ReactionType.RUN_BROADCAST && Param3 != "") //사내방송실시(신고)
                    {
                        reactionLog.ReactionType = (int)SDMS.ReactionType.RUN_REPORT_BROADCAST;
                    }

                    //문자메시지(탐지)
                    if (nReactionType == (int)ReactionType.SEND_SMS)
                    {
                        // 복수 메시지에 탐지란 단어가 포함되어 있으므로 복구문자인지 부터 확인
                        if (strMessage.Contains("복구"))
                        {
                            reactionLog.ReactionType = (int)SDMS.ReactionType.SEND_REPAIR_SMS;
                        }
                        else if (strMessage.Contains("탐지"))
                        {
                            reactionLog.ReactionType = (int)SDMS.ReactionType.SEND_DETECT_SMS;
                        }
                        else if (strMessage.Contains("신고"))
                        {
                            reactionLog.ReactionType = (int)SDMS.ReactionType.SEND_REPORT_SMS;
                        }
                    }

                    Zone zone = ZoneManager.Instance.GetZone(Param1);

                    if (zone != null)
                    {
                        if (!m_dicZoneHistorys.ContainsKey(nSensorHistoryID))
                            m_dicZoneHistorys.Add(nSensorHistoryID, zone);
                    }

                    reactionLog.SensorType = 11;

                    arrManualReactionLog.Add(reactionLog);

                    ArrayList arrLogs = null;

                    if (m_dicHistoryLog.ContainsKey(nSensorHistoryID))
                        arrLogs = m_dicHistoryLog[nSensorHistoryID];
                    else
                    {
                        arrLogs = new ArrayList();
                        m_dicHistoryLog[nSensorHistoryID] = arrLogs;
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

            private void ReadReactionLogMemo(int nMinReactionLogID, int nMaxReactionLogID, Dictionary<int, SensorReactionPSMLog> dicReactionLogs)
            {
                if (dicReactionLogs.Count == 0)
                    return;

                string strNotIncludeIDs = ReactionManager.GetNotIncludeIDs<SensorReactionPSMLog>("SensorReactionHistory", nMinReactionLogID, nMaxReactionLogID, dicReactionLogs);
                string strCondition = ReactionManager.MakeConditionWithNotIncludeIDs("SensorReactionHistoryID", nMinReactionLogID, nMaxReactionLogID, strNotIncludeIDs);

                string strSQL = "Select SensorReactionHistoryID, Description ";
                strSQL += "from SensorReactionHistoryDescription as memo, SensorReactionHistoryDescriptionText as memoText ";
                strSQL += "where memo.DescriptionID = memoText.ID and " + strCondition;

                WebDBManager webDB = FormMain.Instance.DBManager;
                ArrayList arrResult = webDB.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;
                SensorReactionPSMLog log = null;

                // Key : SensorZoneHistory ID
                // Value : Memo
                Dictionary<int, string> dicSensorZoneHistoryMemo = new Dictionary<int, string>();

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strMemo = WebDBManager.GetStringField(arrResult[i + 1]);

                    if (id == null || strMemo == null)
                        continue;

                    if (dicReactionLogs.TryGetValue(id.Data, out log) == false)
                        continue;

                    log.Memo = strMemo;

                    dicSensorZoneHistoryMemo[log.SensorHistoryID] = strMemo;
                }

                // Memo는 특정 SensorReactionHistory에 속해있는 것이지만 Report에서 사용할 때에는 어차피
                // SensorZoneHistory별로 정렬되기 때문에 같은 SensorZoneHistory ID를 가지는 모든 SensorReactionHistory에 Memo를 공유한다.
                foreach (KeyValuePair<int, SensorReactionPSMLog> pair in dicReactionLogs)
                {
                    string strMemo = null;

                    if (dicSensorZoneHistoryMemo.TryGetValue(pair.Value.SensorHistoryID, out strMemo))
                    {
                        pair.Value.Memo = strMemo;
                    }
                }
            }

            private ArrayList GetReactionHistory(ArrayList arrSensorHistoryID)
            {
                if (arrSensorHistoryID == null)
                    return null;

                if (arrSensorHistoryID.Count == 0)
                    return null;

                string strSensorList = "";
                int nCount = 1;
                foreach (int nHistoryID in arrSensorHistoryID)
                {
                    strSensorList += nHistoryID.ToString();
                    if (nCount != arrSensorHistoryID.Count)
                        strSensorList += ",";

                    nCount++;
                }

                ArrayList arrReactionLog = new ArrayList();
                if (strSensorList == "")
                    return arrReactionLog;

                WebDBManager webDB = FormMain.Instance.DBManager;

                //string strSQL = "select id, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5, DetectionStatus from SensorReactionHistory ";
                //strSQL += "where SensorHistoryID in (" + strSensorList + ")";

                string strSQL = "select srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, srh.DetectionStatus, sz.OrgSensorID ";
                strSQL += "from SensorReactionHistory as srh, ";
                strSQL += "SensorZoneHistory as szh, ";
                strSQL += "SensorZone as sz ";
                strSQL += "where srh.SensorHistoryID = szh.id ";
                strSQL += "and (szh.SensorID = sz.ID and sz.Type = 11) ";
                strSQL += "and srh.SensorHistoryID in (" + strSensorList + ")";

                ArrayList arrResult = webDB.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return null;

                int nResultCount = arrResult.Count;
                DateTime dt = DateTime.Now;

                Dictionary<int, SensorReactionPSMLog> dicReactionLogs = new Dictionary<int, SensorReactionPSMLog>();
                int nMinReactionLogID = -1, nMaxReactionLogID = -1;

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
                    reactionLog.SensorHistoryID = nSensorHistoryID;
                    reactionLog.ReactionType = nReactionType;
                    reactionLog.Time = time;
                    reactionLog.DetectionStatus = nDetectionStatus;

                    reactionLog.PSMSensor = PSMManager.Instance.GetSensor(nPSMSensorID);
                    if (reactionLog.PSMSensor != null)
                    {
                        foreach (UnE.PSM.PSMTank psmTank in reactionLog.PSMSensor.LinkedTankList)
                        {
                            reactionLog.PSMMaterial = psmTank.Material;
                            break;
                        }
                    }


                    reactionLog.SensorType = 11;

                    if (nReactionType == (int)ReactionType.IGNORE_PSM_DETECT || nReactionType == (int)ReactionType.NOTIFY_PSM || nReactionType == (int)ReactionType.PSM_USER_RESET)
                    {
                        if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                            m_dicHistoryMember[nSensorHistoryID] = Param3;
                    }
                    /*if (nReactionType == (int)ReactionType.NOTIFY_PSM ||
                        nReactionType == (int)ReactionType.MALFUNCTION ||
                        nReactionType == (int)ReactionType.IGNORE_PSM_DETECT ||
                        nReactionType == (int)ReactionType.CHANGE_PSM_ALARM_DEPTH)
                    {
                        if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                            m_dicHistoryMember.Add(nSensorHistoryID, Param3);
                    }*/

                    reactionLog.Param1 = Param1;

                    //자탐은 Param1이 EquipZoneID임


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
                    if (nReactionType == (int)ReactionType.SEND_SMS)
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

                    ArrayList arrLogs = null;

                    //
                    if (m_dicHistoryLog.ContainsKey(nSensorHistoryID))
                        arrLogs = m_dicHistoryLog[nSensorHistoryID];
                    else
                    {
                        arrLogs = new ArrayList();
                        m_dicHistoryLog[nSensorHistoryID] = arrLogs;
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

                return arrReactionLog;
            }

            //SensorID로 SensorHistoryID를 찾아옴
            // startDate와 endDate 사이의 모든 History들을 DB로부터 가져온다.
            // 가져온 DB 데이터들 가운데 dicSensorZoneIDs에 속하는 것들만 따로 추려낸다.
            private ArrayList GetSensorZoneHistoryID(Dictionary<int, ISensor> dicSensorZones, string startDate, string endDate)
            {
                ArrayList arrSensorZoneHistoryID = new ArrayList();

                /*string strSensorList = "";
                int nCount = 1;
                foreach (ISensor z in arrSensorZoneID)
                {
                    int sensorID = z.ID;
                    strSensorList += sensorID.ToString();
                    if (nCount != arrSensorZoneID.Count)
                        strSensorList += ",";

                    nCount++;
                }*/

                WebDBManager webDB = FormMain.Instance.DBManager;

                string strSQL = "select id,SensorID from SensorZoneHistory where Time Between '" + startDate + "' and '" + endDate + "' and ( Data IN (21, 22, 23))";

                ArrayList arrResult = webDB.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return null;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nSensorID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                    if (dicSensorZones.ContainsKey(nSensorID) == false)
                        continue;

                    arrSensorZoneHistoryID.Add(nID);

                    ArrayList arrLogs = null;

                    if (m_dicSensorHistorys.ContainsKey(nSensorID))
                        arrLogs = m_dicSensorHistorys[nSensorID];
                    else
                    {
                        arrLogs = new ArrayList();
                        m_dicSensorHistorys[nSensorID] = arrLogs;
                    }
                    arrLogs.Add(nID);
                }
                return arrSensorZoneHistoryID;
            }

            //SensorID로 SensorHistoryID를 찾아옴
            // startDate와 endDate 사이의 History 가운데 arrSensorZoneID에 있는 것들만 가져온다.
            // arrSensorZoneID의 개수가 많아질수록 DB Query 부하가 늘어난다.
            /*private ArrayList GetSensorZoneHistoryID(ArrayList arrSensorZoneID, string startDate, string endDate)
            {
                ArrayList arrSensorZoneHistoryID = new ArrayList();

                string strSensorList = "";
                int nCount = 1;
                foreach (ISensor z in arrSensorZoneID)
                {
                    int sensorID = z.ID;
                    strSensorList += sensorID.ToString();
                    if (nCount != arrSensorZoneID.Count)
                        strSensorList += ",";

                    nCount++;
                }

                WebDBManager webDB = FormMain.Instance.DBManager;

                string strSQL = "select id,SensorID from SensorZoneHistory where SensorID in (" + strSensorList + ") And Time Between '" + startDate + "' and '" + endDate + "' and ( Data IN (21, 22, 23))";

                ArrayList arrResult = webDB.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return null;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nSensorID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    arrSensorZoneHistoryID.Add(nID);

                    ArrayList arrLogs = null;

                    if (m_dicSensorHistorys.ContainsKey(nSensorID))
                        arrLogs = m_dicSensorHistorys[nSensorID];
                    else
                    {
                        arrLogs = new ArrayList();
                        m_dicSensorHistorys[nSensorID] = arrLogs;
                    }
                    arrLogs.Add(nID);
                }
                return arrSensorZoneHistoryID;
            }*/

            private ArrayList FindHistoryID(Zone zone, EquipmentZone equipZone)
            {
                if (equipZone == null)
                    return null;

                ArrayList arrHistoryIDList = new ArrayList();
                ArrayList arrEquipmentZoneList = new ArrayList();
                arrEquipmentZoneList.Add(equipZone);

                Dictionary<int, ISensor> dicSensorZones = FindSensorZone(arrEquipmentZoneList);

                if (dicSensorZones == null)
                    return null;

                ArrayList histories = null;

                foreach (KeyValuePair<int, ISensor> pair in dicSensorZones)
                {
                    int nSensorID = pair.Key;
                    if (m_dicSensorHistorys.TryGetValue(nSensorID, out histories))
                        arrHistoryIDList.AddRange(histories);
                }

                /*ArrayList arrSensorZoneList = FindSensorZone(arrEquipmentZoneList);

                if (arrSensorZoneList == null)
                    return null;

                foreach (ISensor z in arrSensorZoneList)
                {
                    int nSensorID = z.ID;
                    if (m_dicSensorHistorys.ContainsKey(nSensorID))
                        arrHistoryIDList.AddRange(m_dicSensorHistorys[nSensorID]);
                }*/

                foreach (int nHistoryID in arrHistoryIDList)
                {
                    if (!m_dicZoneHistorys.ContainsKey(nHistoryID))
                        m_dicZoneHistorys.Add(nHistoryID, zone);
                }

                return arrHistoryIDList;
            }

            //선택한 ZoneID로 EquipmentZoneID를 찾는다
            private ArrayList FindEquipZone(ArrayList arrZoneList)
            {
                ArrayList arrEquipZoneList = new ArrayList();
                if (arrZoneList == null)
                    return null;

                foreach (Zone zone in arrZoneList)
                {
                    if (ZoneManager.Instance.GetEquipmentZoneList(zone) == null)
                        continue;

                    arrEquipZoneList.AddRange(ZoneManager.Instance.GetEquipmentZoneList(zone));
                }

                //중복제거
                ArrayList arTemp = new ArrayList();
                foreach (EquipmentZone equipZone in arrEquipZoneList)
                {
                    if (!arTemp.Contains(equipZone))
                    {
                        arTemp.Add(equipZone);
                    }
                }
                arrEquipZoneList = arTemp;

                return arrEquipZoneList;

            }

            // EquipmentZoneID로 SensorID를 찾아온다
            // 빠른 검색을 위하여 Dictionary 형태로 리턴한다.
            // Key : SensorZone ID
            private Dictionary<int, ISensor> FindSensorZone(ArrayList arrEquipZoneList)
            {
                Dictionary<int, ISensor> dicSensorZones = new Dictionary<int, ISensor>();
                if (arrEquipZoneList == null)
                    return null;

                foreach (EquipmentZone equip in arrEquipZoneList)
                {
                    List<ISensor> arSensors = SensorManager.Instance.FindZoneInSensor(equip.ID, IFacility.FacilityType.PSM_SENSOR);

                    if (arSensors == null)
                        continue;

                    //SensorZoneID 구함
                    foreach (ISensor sensor in arSensors)
                    {
                        dicSensorZones[sensor.ID] = sensor;
                    }

                }
                return dicSensorZones;
            }

            //EquipmentZoneID로 SensorID를 찾아온다
            /*private ArrayList FindSensorZone(ArrayList arrEquipZoneList)
            {
                ArrayList arrSensorZoneList = new ArrayList();
                if (arrEquipZoneList == null)
                    return null;

                foreach (EquipmentZone equip in arrEquipZoneList)
                {
                    ArrayList arSensors = SensorManager.Instance.FindZoneInSensor(equip.ID, IFacility.FacilityType.PSM_SENSOR);

                    if (arSensors == null)
                        continue;

                    //SensorZoneID 구함
                    arrSensorZoneList.AddRange(arSensors);

                }
                return arrSensorZoneList;
            }*/

            //담당자 찾아옴
            private string FindManagerName(Zone zone)
            {
                EquipmentZone equipZone = null;

                List<EquipmentZone> arrEquipZoneList = null;//new ArrayList();
                if (ZoneManager.Instance.GetEquipmentZoneList(zone) == null)
                    return null;

                arrEquipZoneList = ZoneManager.Instance.GetEquipmentZoneList(zone);
                if (arrEquipZoneList != null && arrEquipZoneList.Count > 0)
                {
                    equipZone = (EquipmentZone)arrEquipZoneList[0];
                }

                FacilityManagerGroup ManagerGroup = null;
                Building buildingFind = zone.Building;

                if (equipZone != null)
                {
                    ManagerGroup = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(IFacility.FacilityType.FIRE_SENSOR, equipZone, true);
                }
                if (ManagerGroup == null)
                {
                    //EquipmentZone으로 담당자를 못찾으면 Building으로 찾음
                    ManagerGroup = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(IFacility.FacilityType.FIRE_SENSOR, buildingFind, true);
                }
                if (ManagerGroup == null)
                {
                    ManagerGroup = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.FIRE_SENSOR, true);
                }

                string strPhoneNumber = "";
                string strManagerName = FormMain.Instance.DataManager.GetFacilityManagerName(ManagerGroup, ref strPhoneNumber);

                return strManagerName;
            }

            private string GetReactionString(int nType)
            {
                string strType = "";
                switch (nType)
                {
                    case 1:
                        strType = "화재 센서";
                        //strType = "자탐 센서";
                        break;
                    case 2: strType = "소화 센서";
                        break;
                    case 3: strType = "압력 센서";
                        break;
                    case 4: strType = "수동 신고";
                        break;
                    case 6:
                        strType = "화재 센서";
                        //strType = "자탐 센서";
                        break;
                    case 7: strType = "누출 센서";
                        break;
                    case 9:
                        strType = "화재 센서";
                        //strType = "자탐 센서";
                        break;
                    default:
                        break;
                }

                return strType;
            }

            private string GetDetectionStatusName(int nDetectionStatus)
            {
                switch (nDetectionStatus)
                {
                    case 1:
                        return "실제";
                    case 2:
                        return "오동작";
                    case 3:
                    default:
                        return "테스트";
                }
            }

            private int GetReverseDetectionStatus(string strDetectionStatusName)
            {
                switch (strDetectionStatusName)
                {
                    case "실제":
                        return 1;
                    case "오동작":
                        return 2;
                    case "테스트":
                    default:
                        return 3;
                }
            }

            public void UpdateStatusForSensorReactionHistory(int nSensorReactionHistoryID, int nSensorReactionHistorySensorHistoryID, string strDetectionStatusName)
            {
                foreach (SensorReactionPSMLog reactionHistory in m_dicHistoryLog[nSensorReactionHistorySensorHistoryID])
                {
                    int nDetectionStatus = GetReverseDetectionStatus(strDetectionStatusName);

                    reactionHistory.DetectionStatus = nDetectionStatus;

                    if (reactionHistory.ID == nSensorReactionHistoryID)
                    {
                        WebDBManager webDB = FormMain.Instance.DBManager;

                        string strUpdateQuery = String.Format("UPDATE SensorReactionHistory SET DetectionStatus = {0} WHERE SensorHistoryID = {1}", nDetectionStatus, reactionHistory.SensorHistoryID);

                        ArrayList arrResult = webDB.GetResultData(strUpdateQuery, 0);
                        if (arrResult == null)
                            return;

                    }
                }


            }

        }

        public class SensorReactionPSMLog
        {
            private int nID = -1;
            private int nSensorHistoryID = -1;
            private int nReactionType = -1;
            private DateTime time;
            private int param1 = -1;
            private string strMessage = "";
            private string param2 = "";
            private string param3 = "";
            private string param4 = "";
            private string param5 = "";
            private int nDetectionStatus = -1;
            private int nSensorType = -1;
            private UnE.PSM.PSMSensor obSensor = null;
            private UnE.PSM.PSMMaterial obMaterial = null;
            private string memo = "";

            public int Param1
            {
                get { return param1; }
                set { param1 = value; }
            }

            public string Param2
            {
                get { return param2; }
                set { param2 = value; }
            }

            public string Param3
            {
                get { return param3; }
                set { param3 = value; }
            }

            public string Param4
            {
                get { return param4; }
                set { param4 = value; }
            }

            public string Param5
            {
                get { return param5; }
                set { param5 = value; }
            }

            public int ID
            {
                get { return nID; }
                set { nID = value; }
            }

            public int SensorHistoryID
            {
                get { return nSensorHistoryID; }
                set { nSensorHistoryID = value; }
            }

            public int ReactionType
            {
                get { return nReactionType; }
                set { nReactionType = value; }
            }

            public DateTime Time
            {
                get { return time; }
                set { time = value; }
            }

            public int DetectionStatus
            {
                get { return nDetectionStatus; }
                set { nDetectionStatus = value; }
            }

            public string Message
            {
                get { return strMessage; }
                set { strMessage = value; }
            }

            public int SensorType
            {
                get { return nSensorType; }
                set { nSensorType = value; }
            }

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

            public string Memo
            {
                get { return memo; }
                set { memo = value; }
            }
        }

        class DetectPSMLog : IComparable
        {
            private int nHistoryID = -1;
            private DateTime time;
            private string strDetectType = "";
            private string strManagerName = "";
            private int nZoneID = -1;
            private string strBuildingGroup = "";
            private string strBuildingName = "";
            private string strFloor = "";
            private EquipmentZone equipZone = null;
            private string strDetectionStatusName = "";
            private int nSensorReactionHistoryID = -1;
            private UnE.PSM.PSMMaterial obMaterial = null;
            private UnE.PSM.PSMSensor obSensor = null;
            private string strMemo = "";

            private int nAlarmLevel = 0;

            private DateTime dtDetectStart;
            private DateTime dtDetectEnd;


            public int HistoryID
            {
                get { return nHistoryID; }
                set { nHistoryID = value; }
            }

            public DateTime Time
            {
                get { return time; }
                set { time = value; }
            }

            public string DetectType
            {
                get { return strDetectType; }
                set { strDetectType = value; }
            }

            public string ManagerName
            {
                get { return strManagerName; }
                set { strManagerName = value; }
            }

            public int zoneID
            {
                get { return nZoneID; }
                set { nZoneID = value; }
            }

            public string BuildingGroup
            {
                get { return strBuildingGroup; }
                set { strBuildingGroup = value; }
            }

            public string BuildingName
            {
                get { return strBuildingName; }
                set { strBuildingName = value; }
            }

            public string FloorName
            {
                get { return strFloor; }
                set { strFloor = value; }
            }

            public EquipmentZone EquipZone
            {
                get { return equipZone; }
                set { equipZone = value; }
            }

            public string DetectionStatusName
            {
                get { return strDetectionStatusName; }
                set { strDetectionStatusName = value; }
            }

            public int SensorReactionHistoryID
            {
                get { return nSensorReactionHistoryID; }
                set { nSensorReactionHistoryID = value; }
            }

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
                set{ nAlarmLevel = value; }
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

            public string Memo
            {
                get { return strMemo; }
                set { strMemo = value; }
            }

            public int CompareTo(object b)
            {
                DetectPSMLog data = this;
                DetectPSMLog data2 = (DetectPSMLog)b;

                if (data.time > data2.time)
                    return 1;
                else if (data.time < data2.time)
                    return -1;
                else
                {
                    if (data.nHistoryID < data2.nHistoryID)
                        return -1;
                    else if (data.nHistoryID > data2.nHistoryID)
                        return 1;
                }

                return 0;
            }
        }

        class MulFunctionPSMLog
        {
            private ArrayList nHistoryIDList = new ArrayList();
            public ArrayList HistoryIDList
            {
                get { return nHistoryIDList; }
                set { nHistoryIDList = value; }
            }

            private string strDetectType = "";
            public string DetectType
            {
                get { return strDetectType; }
                set { strDetectType = value; }
            }

            //탐지 횟수
            private int nReactionCount = 0;
            public int ReactionCount
            {
                get { return nReactionCount; }
                set { nReactionCount = value; }
            }
            
            //오작동 횟수
            private int nMulFunctionCount = 0;
            public int MulFunctionCount
            {
                get { return nMulFunctionCount; }
                set { nMulFunctionCount = value; }
            }
            
            //누출신고 횟수
            private int nNotifyCount = 0;
            public int NotifyCount
            {
                get { return nNotifyCount; }
                set { nNotifyCount = value; }
            }
            
            //처리되지 않음
            private int nNotprocess = 0;
            public int Notprocess
            {
                get { return nNotprocess; }
                set { nNotprocess = value; }
            }
            
            //오작동률
            private double nPercentMulFunction = 0;
            public double PercentMulFunction
            {
                get { return nPercentMulFunction; }
                set { nPercentMulFunction = value; }
            }

            //현재 탐지되어 잇는 상태의 신호
            private int nOnlyDetectCount = 0;
            public int OnlyDetectCount
            {
                get { return nOnlyDetectCount; }
                set { nOnlyDetectCount = value; }
            }

            // BuildingGroup
            private string strGroupName = "";
            public string GroupName
            {
                get { return strGroupName; }
                set { strGroupName = value; }
            }
            
            // Building
            private string strBuildingName = "";
            public string BuildingName
            {
                get { return strBuildingName; }
                set { strBuildingName = value; }
            }
            
            // Floor
            private string strFloorName = "";
            public string FloorName
            {
                get { return strFloorName; }
                set { strFloorName = value; }
            }
            
            // 관리자
            private string strManagerName = "";
            public string ManagerName
            {
                get { return strManagerName; }
                set { strManagerName = value; }
            }
            
            // Zone
            private Zone zone = null;
            public Zone Zone
            {
                get { return zone; }
                set { zone = value; }
            }

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

        public class ReactionPSMLog : IComparable
        {
            private int nHistoryID = -1;
            public int HistoryID
            {
                get { return nHistoryID; }
                set { nHistoryID = value; }
            }

            private DateTime time;
            public DateTime Time
            {
                get { return time; }
                set { time = value; }
            }

            private string strManagerName = "";
            public string ManagerName
            {
                get { return strManagerName; }
                set { strManagerName = value; }
            }

            private int nSensorType = -1;
            public int SensorType
            {
                get { return nSensorType; }
                set { nSensorType = value; }
            }

            private string strBuildingName = "";
            public string BuildingName
            {
                get { return strBuildingName; }
                set { strBuildingName = value; }
            }

            private string strFloorName = "";
            public string FloorName
            {
                get { return strFloorName; }
                set { strFloorName = value; }
            }

            private int nReactionType = -1;
            public int Type
            {
                get { return nReactionType; }
                set { nReactionType = value; }
            }

            private Zone zone;
            public Zone Zone
            {
                get { return zone; }
                set { zone = value; }
            }

            public EquipmentZone equipZone;
            public EquipmentZone EquipZone
            {
                get { return equipZone; }
                set { equipZone = value; }
            }

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

            private ArrayList arrLogList = new ArrayList();
            public ArrayList ArrLogList
            {
                get { return arrLogList; }
                set { arrLogList = value; }
            }

            //누출신고위치
            private string strUserName = "";
            public string UserName
            {
                get { return strUserName; }
                set { strUserName = value; }
            }

            public override string ToString()
            {
                string strReactionType = "";
                if (nReactionType == (int)ReactionType.NOTIFY_PSM)
                    strReactionType = "누출 발생";
                else if (nReactionType == (int)ReactionType.MALFUNCTION || nReactionType == (int)ReactionType.PSM_USER_RESET)
                    strReactionType = "시스템 복구 처리";
                else if (nReactionType == (int)ReactionType.IGNORE_PSM_DETECT || nReactionType == (int)ReactionType.END_PSM_STATUS || nReactionType == (int)ReactionType.END_STATUS)
                    strReactionType = "누출탐지 후 상황해제";
                else if (nReactionType == (int)ReactionType.BEGIN_PSM_STATUS)
                    strReactionType = "누출 탐지";

                if (nSensorType == 0)
                {
                    return String.Format("{0}   [ 수동 신고 ] {1} - {2}", String.Format("{0:0000}-{1:00}-{2:00} {3} {4}:{5}", time.Year, time.Month, time.Day, time.Hour < 12 ? "오전" : "오후", time.Hour > 12 ? time.Hour - 12 : time.Hour, time.Minute)
                        , obMaterial.Name, strReactionType);
                }
                else
                {
                    return String.Format("{0}   [ 누 출 ] {1} - {2}", String.Format("{0:0000}-{1:00}-{2:00} {3} {4}:{5}", time.Year, time.Month, time.Day, time.Hour < 12 ? "오전" : "오후", time.Hour > 12 ? time.Hour - 12 : time.Hour, time.Minute)
                        , obMaterial.Name, strReactionType);
                }
            }

            //누출 레벨
            private int nLevel = 0;
            public int Level
            {
                get { return nLevel; }
                set { nLevel = value; }
            }

            public int CompareTo(object obj)
            {
                ReactionPSMLog data = (ReactionPSMLog)obj;

                if (this.time > data.time)
                    return 1;
                else if (this.time < data.time)
                    return -1;
                else
                {
                    if (this.nHistoryID < data.nHistoryID)
                        return -1;
                    else if (this.nHistoryID > data.nHistoryID)
                        return 1;
                }

                return 0;
            }

        }

    }
}