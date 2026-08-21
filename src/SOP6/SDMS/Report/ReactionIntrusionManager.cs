using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility2;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using libSensorProcess;

namespace SDMS
{
    namespace Report
    {
        public class ReactionIntrusionManager
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

            private ArrayList m_arrDectectList = null;
            public ArrayList DectectList
            {
                get { return m_arrDectectList; }
                set { m_arrDectectList = value; }
            }
            private ArrayList m_arrMulFunctionList = null;
            public ArrayList MulFunctionList
            {
                get { return m_arrMulFunctionList; }
                set { m_arrMulFunctionList = value; }
            }

            private ArrayList m_arrReactionHistory = new ArrayList();

            //HistoryID,ReactionLog
            private Dictionary<int, ArrayList> m_dicHistoryLog = new Dictionary<int, ArrayList>();
            //SensorZone ID,HistoryID List
            private Dictionary<int, ArrayList> m_dicSensorHistorys = new Dictionary<int, ArrayList>();

            //HistoryID, Zone
            private Dictionary<int, Zone> m_dicZoneHistorys = new Dictionary<int, Zone>();

            //HistoryID, ReactionType
            private Dictionary<int, int> m_dicHistoryType = new Dictionary<int, int>();

            //MulFunctionLog, SensorReactionLogList
            private Dictionary<MulFunctionIntrusionLog, ArrayList> m_dicMulFuctionSrLog = new Dictionary<MulFunctionIntrusionLog, ArrayList>();
            internal Dictionary<MulFunctionIntrusionLog, ArrayList> DicMulFuctionSrLog
            {
                get { return m_dicMulFuctionSrLog; }
                set { m_dicMulFuctionSrLog = value; }
            }

            private ArrayList arrAllReactionLog = new ArrayList();

            public ArrayList AllReactionLog
            {
                get { return arrAllReactionLog; }
            }

            //화재를 신고한곳<MemberID, NicName>
            private Dictionary<string, string> m_dicGenUserIDDNicName = new Dictionary<string, string>();
            public Dictionary<string, string> DicGenUserIDDNicName
            {
                get { return m_dicGenUserIDDNicName; }
                set { m_dicGenUserIDDNicName = value; }
            }

            //HistoryID, Param3(MemberID)
            private Dictionary<int, string> m_dicHistoryMember = new Dictionary<int, string>();

            private int m_nSiteID = 1;
            public ReactionIntrusionManager()
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
            // Return 값 : true이면 데이터가 변경되었음.
            //             false이면 데이터 변경없음.
            public void ZoneSubmit(ArrayList arrZoneList, DateTime startDate, DateTime endDate, int pageType = 1)//pageType이 1이면 탐지/처리 2이면 대응이력
            {
                LoadSOPGenUser();

                string strNowDate, strBeforeDate;
                GetZoneSumitDate(pageType, startDate, endDate, out strBeforeDate, out strNowDate);

                //ZoneID 리스트로 ReactionHistory의 수동신고의 log를 가져온다. 
                //SELECT * FROM SensorReactionHistory WHERE ReactionType = 898 AND Param2 = 0;
                ArrayList arrManualReactionHistory = GetManualReactionHistory(arrZoneList, strBeforeDate, strNowDate);

                //선택한 ZoneID 리스트로 EquipmentZoneID를 찾는다.
                List<EquipmentZone> arrEquipmentZoneList = FindEquipZone(arrZoneList);
                //가져온 EquipmentZoneID 리스트로 SensorID를 찾아온다.
                Dictionary<int, ISensor> dicSensorZones = FindSensorZone(arrEquipmentZoneList);
                //ArrayList arrSensorZoneList = FindSensorZone(arrEquipmentZoneList);
                //SensorID리스트로 SensorHistoryID를 찾아옴
                ArrayList arrZoneHistoryList = GetSensorZoneHistoryID(dicSensorZones, strBeforeDate, strNowDate);
                //ArrayList arrZoneHistoryList = GetSensorZoneHistoryID(arrSensorZoneList, strBeforeDate, strNowDate);

                //ReactionLog를 가져옴
                //SELECT * FROM SensorReactionHistory WHERE SensorHistoryID IN (arrZoneHistoryList);
                ArrayList arrReactionList = GetReactionHistory(arrZoneHistoryList);

                //수동신고와 자탐의 SensorReactionLog를 합친다.
                arrAllReactionLog = new ArrayList();
                arrAllReactionLog = AddReactionHistoryLog(arrManualReactionHistory, arrReactionList);

                //오작동이력 로그 저장
                m_arrMulFunctionList = GetMulFunctionLog(arrZoneList, strBeforeDate, strNowDate);


                //전체 ReactionLog중에 화재 탐지 된 로그만 가져와서 저장함
                //화재신고 된 로그만 가져옴(ReactionType=0 -> 자탐 / reactionLog.ReactionType == 22 && reactionLog.Param2 == "0" -> 수동
                m_arrDectectList = GetDetectLog(arrAllReactionLog);
                m_arrDectectList.Sort();
            }

            private void GetZoneSumitDate(int pageType, DateTime startDate, DateTime endDate, out string strBeforeDate, out string strNowDate)
            {
                strNowDate = "";
                strBeforeDate = string.Format("{0} {1}:{2}:{3}", startDate.ToShortDateString(), "00", "00", "00");

                if (pageType == 2)//대응이력은 시작날과 종료날이 같을경우 시간까지 조절해야하므로 ..
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
            public bool NeedRefresh(ArrayList arrZoneList, DateTime startDate, DateTime endDate, RefreshCheckData checkData, int pageType = 1)
            {
                string strBeforeDate, strNowDate;
                GetZoneSumitDate(pageType, startDate, endDate, out strBeforeDate, out strNowDate);

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
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

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

                ArrayList arrResult = webDB.GetResultData(strSQL);
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

                    ReactionIntrusionLog reactionLog = new ReactionIntrusionLog();
                    reactionLog.HistoryID = nHistoryID;
                    reactionLog.ArrLogList = log;

                    //자탐 ReactionType가져옴
                    if (m_dicHistoryType.ContainsKey(nHistoryID))
                    {
                        nReactionType = m_dicHistoryType[nHistoryID];
                        //reactionLog.SensorType = 1;
                    }
                    /*else
                        reactionLog.SensorType = 0;*/
                     
                    //오작동인지 화재인지, 무시된 신호인지 구분하기위함(수동신고)
                    //if (reactionLog.SensorType == 0)
                    //{
                    //    foreach (SensorReactionLog Typelog in log)
                    //    {
                            //if (Typelog.ReactionType == (int)ReactionType.NOTIFY_SECURITY)
                    //        {
                    //            nReactionType = (int)ReactionType.NOTIFY_SECURITY;
                    //            break;
                    //        }
                    //        else if (Typelog.ReactionType == (int)ReactionType.MALFUNCTION)
                    //        {
                    //            nReactionType = (int)ReactionType.MALFUNCTION;
                    //            break;
                    //        }
                    //        else if (Typelog.ReactionType == (int)ReactionType.IGNORE_S1ACCESS_STATUS)
                    //        {
                    //            nReactionType = (int)ReactionType.IGNORE_S1ACCESS_STATUS;
                    //            break;
                    //        }
                    //        else if (Typelog.ReactionType == (int)ReactionType.IGNORE_S1SVMS_STATUS)
                    //        {
                    //            nReactionType = (int)ReactionType.IGNORE_S1SVMS_STATUS;
                    //            break;
                    //        } 
                    //        nReactionType = Typelog.ReactionType;
                    //    }
                    //} 

                    //가장 맨 처음 발생한 ReactionLog를 Comobox로 보여줘야 하므로 log배열의 가장 첫번째 값을 가져온다
                    SensorReactionIntrusionLog sensorreactionLog = (SensorReactionIntrusionLog)log[0];

                    if (!(sensorreactionLog.Time >= startDate && sensorreactionLog.Time <= endDate))
                        continue;


                    if (m_dicHistoryMember.ContainsKey(nHistoryID))
                        strMemberID = m_dicHistoryMember[nHistoryID];

                    if (m_dicGenUserIDDNicName.ContainsKey(strMemberID))
                        reactionLog.UserName = m_dicGenUserIDDNicName[strMemberID];

                    reactionLog.Time = sensorreactionLog.Time;
                    reactionLog.SensorType = sensorreactionLog.SensorType;
                    
                    reactionLog.Zone = zone;
                    reactionLog.ManagerName = FindManagerName(zone);
                    reactionLog.Type = nReactionType;
                    reactionLog.FacilityType = sensorreactionLog.Param3;
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

                        foreach (SensorReactionIntrusionLog srLog in log)
                        {
                            ReactionIntrusionLog reactionLog = new ReactionIntrusionLog();
                            reactionLog.HistoryID = nHistoryID;
                            reactionLog.ArrLogList = log;

                            if (m_dicHistoryMember.ContainsKey(nHistoryID))
                                strMemberID = m_dicHistoryMember[nHistoryID];

                            if (m_dicGenUserIDDNicName.ContainsKey(strMemberID))
                                reactionLog.UserName = m_dicGenUserIDDNicName[strMemberID];


                            //자탐은 param1
                            reactionLog.equipZone = ZoneManager.Instance.GetEquipZone(srLog.Param1);

                            if (srLog.ReactionType == (int)ReactionType.MALFUNCTION || srLog.ReactionType == (int)ReactionType.USER_RESET || srLog.ReactionType == (int)ReactionType.IGNORE_SIGNAL)
                            {
                                int nCommanderID = -1;
                                if (int.TryParse(srLog.Param3, out nCommanderID))
                                {
                                    if (m_dicGenUserIDDNicName.ContainsKey(nCommanderID.ToString()))
                                        reactionLog.ManagerName = m_dicGenUserIDDNicName[nCommanderID.ToString()];
                                }
                            } 

                            reactionLog.Time = srLog.Time; 
                            reactionLog.SensorType = srLog.SensorType;
                            reactionLog.Zone = zone;
                            //reactionLog.ManagerName = FindManagerName(zone);
                            reactionLog.Type = srLog.ReactionType; 

                            arrReactLog.Add(reactionLog);
                        }
                        break;
                    }
                }

                // arrReactLog.Sort();
                return arrReactLog;
            }


            private Zone GetZone(int nHistoryID)
            {
                if (m_dicZoneHistorys.ContainsKey(nHistoryID))
                    return m_dicZoneHistorys[nHistoryID];

                return null;
            }


            private ArrayList GetDetectLog(ArrayList arrAllLog)
            {
                ArrayList arrDetectLog = new ArrayList();
                ArrayList arrComboBoxDate = new ArrayList();
                //ArrayList arrReactionLog = new ArrayList();

                foreach (SensorReactionIntrusionLog reactionLog in arrAllLog)
                {
                    // BEGIN_STATUS만 있으면 되는데 이전 로그를 읽어들일수 있도록 하기 위하여
                    // BEGIN_~ 다른 것들도 검사한다.
                    // SOPWebServer 도입 이후에는 BEGIN_STATUS만 생성된다.
                    if (reactionLog.ReactionType == (int)ReactionType.BEGIN_STATUS
                     /*|| reactionLog.ReactionType == (int)ReactionType.BEGIN_S1SVMS_STATUS  
                     || reactionLog.ReactionType == (int)ReactionType.BEGIN_S1ACCESS_STATUS
                     || reactionLog.ReactionType == (int)ReactionType.BEGIN_SECOM_STATUS*/
                     || (reactionLog.ReactionType == (int)ReactionType.NOTIFY_SIGNAL && reactionLog.Param2 == "0"))
                    {
                        DetectIntrusionLog detect = new DetectIntrusionLog();

                        detect.SensorReactionHistoryID = reactionLog.ID;
                        detect.HistoryID = reactionLog.SensorHistoryID;
                        detect.Time = reactionLog.Time;
                        detect.Memo = reactionLog.Memo;

                        Zone zone = null;

                        //if (reactionLog.Param2 == "0")
                        //{
                        //    zone = ZoneManager.Instance.GetZone(reactionLog.Param1);
                        //    detect.zoneID = reactionLog.Param1;
                        //}
                        //else
                        //{
                            if (m_dicZoneHistorys.ContainsKey(reactionLog.SensorHistoryID))
                                zone = m_dicZoneHistorys[reactionLog.SensorHistoryID];

                            detect.zoneID = zone.ID;
                        //}

                        string szBuildingName = zone.Building != null ? zone.Building.BuildingName : "";
                        string szGroupName = szBuildingName != "" ? zone.Building.BuildingGroup.BuildingGroupName : "";
                        string strFloorIndex = zone.Floor != null ? zone.Floor.ToString() : "";


                        //EquipZone표시는 자탐일때만.. 수동신고일때는 알 수 없다.
                        if (reactionLog.ReactionType == (int)ReactionType.NOTIFY_SIGNAL)
                        {
                            //EquipZone구하기
                            detect.EquipZone = ZoneManager.Instance.GetEquipZone(reactionLog.Param1);
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
                        detect.DetectType = GetReactionString(Convert.ToInt32(reactionLog.Param3)); 
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

                int beginDetectType = 0;
                //Zone별로 Log에서 탐지,화재,오작동,처리되지않은신호의 갯수, 오작동률 등을 구함
                foreach (Zone zone in arrZoneList)
                {
                    ArrayList arrHistoryList = FindHistoryID(zone.ID);
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
                    MulFunctionIntrusionLog mulfuction = new MulFunctionIntrusionLog();

                    int nFireCount = 0;
                    int nMulFunctionCount = 0;
                    int nNotprocessCount = 0;
                    int nOnlyDetectCount = 0;

                    foreach (int nHistoryID in arrHistoryList)
                    {
                        ArrayList arrLog = new ArrayList();

                        if (m_dicHistoryLog.ContainsKey(nHistoryID))
                            arrLog = m_dicHistoryLog[nHistoryID];

                        int nType = 0;

                        foreach (SensorReactionIntrusionLog log in arrLog)
                        {
                            if (log.ReactionType != (int)ReactionType.NOTIFY_SIGNAL &&
                                log.ReactionType != (int)ReactionType.BEGIN_STATUS &&
                                log.ReactionType != (int)ReactionType.IGNORE_SIGNAL &&
                                log.ReactionType != (int)ReactionType.END_STATUS &&
                                log.ReactionType != (int)ReactionType.MALFUNCTION &&
                                log.ReactionType != (int)ReactionType.USER_RESET)
                                continue;
                            /*if (log.ReactionType != (int)ReactionType.NOTIFY_SECURITY &&
                                log.ReactionType != (int)ReactionType.BEGIN_S1SVMS_STATUS &&
                                log.ReactionType != (int)ReactionType.IGNORE_S1SVMS_STATUS &&
                                log.ReactionType != (int)ReactionType.END_S1SVMS_STATUS &&
                                log.ReactionType != (int)ReactionType.BEGIN_S1ACCESS_STATUS &&
                                log.ReactionType != (int)ReactionType.IGNORE_S1ACCESS_STATUS &&
                                log.ReactionType != (int)ReactionType.END_S1ACCESS_STATUS &&
                                log.ReactionType != (int)ReactionType.BEGIN_SECOM_STATUS &&
                                log.ReactionType != (int)ReactionType.IGNORE_SECOM_STATUS &&
                                log.ReactionType != (int)ReactionType.END_SECOM_STATUS &&
                                log.ReactionType != (int)ReactionType.MALFUNCTION) continue;*/

                            // BEGIN_STATUS만 있으면 되는데 이전 로그를 읽어들일수 있도록 하기 위하여
                            // BEGIN_~ 다른 것들도 검사한다.
                            // SOPWebServer 도입 이후에는 BEGIN_STATUS만 생성된다.
                            if ((log.ReactionType == (int)ReactionType.BEGIN_STATUS
                                /*|| log.ReactionType == (int)ReactionType.BEGIN_S1SVMS_STATUS
                                || log.ReactionType == (int)ReactionType.BEGIN_S1ACCESS_STATUS
                                || log.ReactionType == (int)ReactionType.BEGIN_SECOM_STATUS*/) && log.Param3 != "null")
                                beginDetectType = Convert.ToInt32(log.Param3);

                            if (log.ReactionType == (int)ReactionType.BEGIN_STATUS
                                /*|| log.ReactionType == (int)ReactionType.BEGIN_S1SVMS_STATUS
                                || log.ReactionType == (int)ReactionType.BEGIN_S1ACCESS_STATUS
                                || log.ReactionType == (int)ReactionType.BEGIN_SECOM_STATUS*/)
                            {
                                ArrayList arrSensorLog = null;


                                //<MulFunctionLog, SensorReactionLog> Dictionary에 값 추가
                                if (m_dicMulFuctionSrLog.ContainsKey(mulfuction))
                                    arrSensorLog = m_dicMulFuctionSrLog[mulfuction];
                                else
                                {
                                    arrSensorLog = new ArrayList();
                                    m_dicMulFuctionSrLog[mulfuction] = arrSensorLog;
                                }
                                arrSensorLog.Add(log);
                            }
                            if (log.ReactionType == (int)ReactionType.BEGIN_STATUS)
                            {
                                nFireCount++;
                                nType = (int)ReactionType.BEGIN_STATUS;

                                break;
                            }
                            else if (log.ReactionType == (int)ReactionType.NOTIFY_SIGNAL)
                            {
                                nFireCount++;
                                nType = (int)ReactionType.NOTIFY_SIGNAL;

                                break;
                            }
                            else if (log.ReactionType == (int)ReactionType.MALFUNCTION)
                            {
                                nMulFunctionCount++;
                                nType = (int)ReactionType.MALFUNCTION;

                                break;
                            }
                            // 처리되지 않음.
                            else if (log.ReactionType == (int)ReactionType.IGNORE_SIGNAL || log.ReactionType == (int)ReactionType.END_STATUS) // 상황종료(처리되지 않음, 현장복구)
                            {
                                nType = (int)ReactionType.IGNORE_SIGNAL;

                                break;
                            }
                        }

                        if (nType == 0)
                            nOnlyDetectCount++;

                        if (!m_dicHistoryType.ContainsKey(nHistoryID))
                        {
                            m_dicHistoryType.Add(nHistoryID, nType);
                        } 
                    }

                    if (beginDetectType < (int)IFacility.FacilityType.Intrusion_S1 ||
                        beginDetectType > (int)IFacility.FacilityType.SecomWomenAlarmBell ||
                        beginDetectType == (int)IFacility.FacilityType.Fire_S1 || 
                        beginDetectType == (int)IFacility.FacilityType.SecomFire ||
                        beginDetectType == (int)IFacility.FacilityType.FireF1_S1) continue;                        

                    mulfuction.ReactionCount = arrHistoryList.Count;

                    //처리되지 않음
                    nNotprocessCount = arrHistoryList.Count - (nFireCount + nMulFunctionCount) - nOnlyDetectCount;

                    double PercentMulFunction = (nMulFunctionCount * 100) / arrHistoryList.Count;

                    mulfuction.HistoryIDList = arrHistoryList;
                    mulfuction.DetectType = GetReactionString(beginDetectType);

                    mulfuction.FireCount = nFireCount;
                    mulfuction.MulFunctionCount = nMulFunctionCount;
                    mulfuction.Zone = zone;
                    mulfuction.ManagerName = FindManagerName(zone);
                    mulfuction.Notprocess = nNotprocessCount;
                    mulfuction.PercentMulFunction = PercentMulFunction;
                    mulfuction.OnlyDetectCount = nOnlyDetectCount;

                    string szBuildingName = zone.Building != null ? zone.Building.DisplayText : "";
                    string szGroupName = szBuildingName != "" ? zone.Building.BuildingGroup.BuildingGroupName : "";
                    string strFloorIndex = zone.Floor != null ? zone.Floor.ToString() : "";

                    if (szGroupName == "" || szGroupName == "null")
                        mulfuction.GroupName = "외부 영역";
                    else
                        mulfuction.GroupName = szGroupName;

                    if (szBuildingName == "" || szBuildingName == "null")
                        mulfuction.BuildingName = zone.DisplayText;
                    else
                        mulfuction.BuildingName = szBuildingName;

                    mulfuction.FloorName = strFloorIndex;

                    arrMulFunction.Add(mulfuction);
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

                int nSensorZoneID = SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.Security_Sensor;

                string strSQL = "select srh.ID, SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, srh.DetectionStatus, sz.Type ";
                strSQL += "From SensorReactionHistory as srh, SensorZoneHistory as szh, SensorZone as sz ";
                strSQL += "where srh.SensorHistoryID = szh.ID and szh.SensorID = sz.ID and SensorHistoryID in "
                         + "(select SensorHistoryID from SensorReactionHistory where param1 in(" + strZoneList + ") And srh.ReactionType = " + (int)ReactionType.NOTIFY_SIGNAL + " AND Param2 = " + nSensorZoneID + " And Time Between '" + startDate + "' and '" + endDate + "') ";
                strSQL += "order by srh.ID";

                ArrayList arrResult = webDB.GetResultData(strSQL);
                if (arrResult == null)
                    return null;

                int nResultCount = arrResult.Count;
                DateTime dt = DateTime.Now;

                Dictionary<int, SensorReactionIntrusionLog> dicReactionLogs = new Dictionary<int, SensorReactionIntrusionLog>();
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
                    int nSensorType = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);

                    SensorReactionIntrusionLog reactionLog = new SensorReactionIntrusionLog();
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
                    reactionLog.SensorType = nSensorType;
                    //reactionLog.SensorType = 0;

                    if (nReactionType == (int)ReactionType.MALFUNCTION ||
                        nReactionType == (int)ReactionType.USER_RESET ||
                        nReactionType == (int)ReactionType.NOTIFY_SIGNAL ||
                        nReactionType == (int)ReactionType.IGNORE_SIGNAL
                        /*nReactionType == (int)ReactionType.NOTIFY_SECURITY ||
                        nReactionType == (int)ReactionType.IGNORE_S1ACCESS_STATUS ||
                        nReactionType == (int)ReactionType.IGNORE_S1SVMS_STATUS ||
                        nReactionType == (int)libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS*/)
                    {
                        if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                            m_dicHistoryMember.Add(nSensorHistoryID, Param3);
                    }


                    //사내방송실시, 메시지(탐지/신고 여부)
                    //사내방송실시(탐지)
                    if (nReactionType == (int)ReactionType.RUN_BROADCAST && Param3 == "")
                    {
                        reactionLog.ReactionType = (int)ReactionType.RUN_DETECT_BROADCAST;
                    }
                    else if (nReactionType == (int)ReactionType.RUN_BROADCAST && Param3 != "") //사내방송실시(신고)
                    {
                        reactionLog.ReactionType = (int)ReactionType.RUN_REPORT_BROADCAST;
                    }

                    //문자메시지(탐지)
                    if (nReactionType == (int)ReactionType.SEND_SMS && strMessage.Contains("탐지"))
                    {
                        reactionLog.ReactionType = (int)ReactionType.SEND_DETECT_SMS;
                    }
                    else if (nReactionType == (int)ReactionType.SEND_SMS && strMessage.Contains("신고"))
                    {
                        reactionLog.ReactionType = (int)ReactionType.SEND_REPORT_SMS;
                    }

                    Zone zone = ZoneManager.Instance.GetZone(Param1);

                    if (zone != null)
                    {
                        if (!m_dicZoneHistorys.ContainsKey(nSensorHistoryID))
                            m_dicZoneHistorys.Add(nSensorHistoryID, zone);
                    }

                    //reactionLog.SensorType = 0;

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

                string strSQL = "select srh.id, SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, srh.DetectionStatus, sz.Type ";
                strSQL += "From SensorReactionHistory as srh, SensorZone as sz, SensorZoneHistory as szh ";
                strSQL += "where srh.SensorHistoryID = szh.ID and szh.SensorID = sz.ID order by srh.ID and SensorHistoryID in (" + strSensorList + ") order by srh.id";

                ArrayList arrResult = webDB.GetResultData(strSQL);
                if (arrResult == null)
                    return null;

                int nResultCount = arrResult.Count;
                DateTime dt = DateTime.Now;

                Dictionary<int, SensorReactionIntrusionLog> dicReactionLogs = new Dictionary<int, SensorReactionIntrusionLog>();
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
                    int nSensorType = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);

                    if (Param1 == 308 || Param1 == 310)
                    {

                    }

                    SensorReactionIntrusionLog reactionLog = new SensorReactionIntrusionLog();
                    reactionLog.ID = nID;
                    reactionLog.SensorHistoryID = nSensorHistoryID;
                    reactionLog.ReactionType = nReactionType;
                    reactionLog.Time = time;
                    reactionLog.DetectionStatus = nDetectionStatus;
                    reactionLog.SensorType = nSensorType;

                    if (nReactionType == (int)libSensorProcess.ReactionType.MALFUNCTION ||
                        nReactionType == (int)libSensorProcess.ReactionType.USER_RESET ||
                        nReactionType == (int)libSensorProcess.ReactionType.NOTIFY_SIGNAL ||
                        nReactionType == (int)libSensorProcess.ReactionType.IGNORE_SIGNAL
                        /*nReactionType == (int)libSensorProcess.ReactionType.NOTIFY_SECURITY ||
                        nReactionType == (int)libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS ||
                        nReactionType == (int)libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS ||
                        nReactionType == (int)libSensorProcess.ReactionType.IGNORE_SECOM_STATUS*/)
                    {
                        if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                            m_dicHistoryMember.Add(nSensorHistoryID, Param3);
                    }

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
                    if (nReactionType == (int)libSensorProcess.ReactionType.RUN_BROADCAST && Param3 == "")
                    {
                        reactionLog.ReactionType = (int)libSensorProcess.ReactionType.RUN_DETECT_BROADCAST;
                    }
                    else if (nReactionType == (int)libSensorProcess.ReactionType.RUN_BROADCAST && Param3 != "") //사내방송실시(신고)
                    {
                        reactionLog.ReactionType = (int)libSensorProcess.ReactionType.RUN_REPORT_BROADCAST;
                    }

                    //문자메시지
                    if (nReactionType == (int)libSensorProcess.ReactionType.SEND_SMS)
                    {
                        if (strMessage.Contains("복구"))
                        {
                            reactionLog.ReactionType = (int)libSensorProcess.ReactionType.SEND_REPAIR_SMS;
                        }
                        else if (strMessage.Contains("오작동"))
                        {
                            reactionLog.ReactionType = (int)libSensorProcess.ReactionType.SEND_MALFUNCTION_SMS;
                        }
                        else if (strMessage.Contains("탐지"))
                        {
                            reactionLog.ReactionType = (int)libSensorProcess.ReactionType.SEND_DETECT_SMS;
                        }
                        else if (strMessage.Contains("신고"))
                        {
                            reactionLog.ReactionType = (int)libSensorProcess.ReactionType.SEND_REPORT_SMS;
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

            private void ReadReactionLogMemo(int nMinReactionLogID, int nMaxReactionLogID, Dictionary<int, SensorReactionIntrusionLog> dicReactionLogs)
            {
                if (dicReactionLogs.Count == 0)
                    return;

                string strNotIncludeIDs = ReactionManager.GetNotIncludeIDs<SensorReactionIntrusionLog>("SensorReactionHistory", nMinReactionLogID, nMaxReactionLogID, dicReactionLogs);
                string strCondition = ReactionManager.MakeConditionWithNotIncludeIDs("SensorReactionHistoryID", nMinReactionLogID, nMaxReactionLogID, strNotIncludeIDs);

                string strSQL = "Select SensorReactionHistoryID, Description ";
                strSQL += "from SensorReactionHistoryDescription as memo, SensorReactionHistoryDescriptionText as memoText ";
                strSQL += "where memo.DescriptionID = memoText.ID and " + strCondition;

                WebDBManager webDB = FormMain.Instance.DBManager;
                ArrayList arrResult = webDB.GetResultData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;
                SensorReactionIntrusionLog log = null;

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
                foreach (KeyValuePair<int, SensorReactionIntrusionLog> pair in dicReactionLogs)
                {
                    string strMemo = null;

                    if (dicSensorZoneHistoryMemo.TryGetValue(pair.Value.SensorHistoryID, out strMemo))
                    {
                        pair.Value.Memo = strMemo;
                    }
                }
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

                string strSQL = "select id,SensorID from SensorZoneHistory where Time Between '" + startDate + "' and '" + endDate + "' and (Data =1)";

                ArrayList arrResult = webDB.GetResultData(strSQL);
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

                string strSQL = "select id,SensorID from SensorZoneHistory where SensorID in (" + strSensorList + ") And Time Between '" + startDate + "' and '" + endDate + "' and ( Data =1)";

                ArrayList arrResult = webDB.GetResultData(strSQL);
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

            private ArrayList FindHistoryID(int nZoneID)
            {
                ArrayList arrHistoryIDList = new ArrayList();
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                //자탐Log
                List<EquipmentZone> arrEquipmentZoneList = ZoneManager.Instance.GetEquipmentZoneList(zone);
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
            private List<EquipmentZone> FindEquipZone(ArrayList arrZoneList)
            {
                List<EquipmentZone> arrEquipZoneList = new List<EquipmentZone>();
                if (arrZoneList == null)
                    return null;

                foreach (Zone zone in arrZoneList)
                {
                    if (ZoneManager.Instance.GetEquipmentZoneList(zone) == null)
                        continue;

                    arrEquipZoneList.AddRange(ZoneManager.Instance.GetEquipmentZoneList(zone));
                }

                //중복제거
                List<EquipmentZone> arTemp = new List<EquipmentZone>();
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
            private Dictionary<int, ISensor> FindSensorZone(List<EquipmentZone> arrEquipZoneList)
            {
                Dictionary<int, ISensor> dicSensorZones = new Dictionary<int, ISensor>();
                if (arrEquipZoneList == null)
                    return null;

                foreach (EquipmentZone equip in arrEquipZoneList)
                {
                    List<ISensor> arSensors = SensorManager.Instance.FindZoneInSensorIntrusion(equip.ID);

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
                    ArrayList arSensors = SensorManager.Instance.FindZoneInSensorIntrusion(equip.ID); 

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

                List<EquipmentZone> arrEquipZoneList = null;
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
                    ManagerGroup = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(IFacility.FacilityType.Intrusion_S1, equipZone, true);
                }
                if (ManagerGroup == null)
                {
                    //EquipmentZone으로 담당자를 못찾으면 Building으로 찾음
                    ManagerGroup = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(IFacility.FacilityType.Intrusion_S1, buildingFind, true);
                }
                if (ManagerGroup == null)
                {
                    ManagerGroup = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.Intrusion_S1, true);
                }

                string strPhoneNumber = "";
                string strManagerName = FormMain.Instance.DataManager.GetFacilityManagerName(ManagerGroup, ref strPhoneNumber);

                return strManagerName;
            }

            private string GetReactionString(int facilityType)
            {
                string strType = "";
                switch (facilityType)
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
                    case (int)IFacility.FacilityType.SecomExternalAlarmBell:
                        strType = "외부비상벨";
                        break;
                    case (int)IFacility.FacilityType.SecomWomenAlarmBell:
                        strType = "여자화장실 비상벨";
                        break;
                    default:
                        strType = "방범센서";
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
                foreach (SensorReactionIntrusionLog reactionHistory in m_dicHistoryLog[nSensorReactionHistorySensorHistoryID])
                {
                    int nDetectionStatus = GetReverseDetectionStatus(strDetectionStatusName);

                    reactionHistory.DetectionStatus = nDetectionStatus;

                    if (reactionHistory.ID == nSensorReactionHistoryID)
                    {

                        WebDBManager webDB = FormMain.Instance.DBManager;

                        string strUpdateQuery = String.Format("UPDATE SensorReactionHistory SET DetectionStatus = {0} WHERE SensorHistoryID = {1}", nDetectionStatus, reactionHistory.SensorHistoryID);

                        ArrayList arrResult = webDB.GetResultData(strUpdateQuery);

                        if (arrResult == null)
                            return;
                    }

                }
            }

            public static bool IsS1SVMS(int nSensorType)
            {
                if (nSensorType >= (int)IFacility.FacilityType.Intrusion_S1 && nSensorType <= (int)IFacility.FacilityType.EmergencyBell_S1)
                    return true;

                return false;
            }

            public static bool IsS1Access(int nSensorType)
            {
                if (nSensorType >= (int)IFacility.FacilityType.GeneralIntrusionT1_S1 && nSensorType <= (int)IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1)
                    return true;

                return false;
            }

            public static bool IsEmpoll(int nSensorType)
            {
                return nSensorType == (int)IFacility.FacilityType.ExternalAlarmBell;
            }

            public static bool IsSecom(int nSensorType)
            {
                if (nSensorType >= (int)IFacility.FacilityType.SecomFire && nSensorType <= (int)IFacility.FacilityType.SecomWomenAlarmBell)
                    return true;

                return false;
            }
        }

        public class SensorReactionIntrusionLog
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
            private string memo = "";

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

            public int Param1
            {
                get { return param1; }
                set { param1 = value; }
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

            public string Memo
            {
                get { return memo; }
                set { memo = value; }
            }
        }

        class DetectIntrusionLog : IComparable
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
            private string strMemo = "";

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

            public string Memo
            {
                get { return strMemo; }
                set { strMemo = value; }
            }

            public DetectIntrusionLog()
            {

            }


            public int CompareTo(object b)
            {
                DetectIntrusionLog data = this;
                DetectIntrusionLog data2 = (DetectIntrusionLog)b;

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

        class MulFunctionIntrusionLog
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
            //화재신고 횟수
            private int nFireCount = 0;

            public int FireCount
            {
                get { return nFireCount; }
                set { nFireCount = value; }
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

            private string strGroupName = "";

            public string GroupName
            {
                get { return strGroupName; }
                set { strGroupName = value; }
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
            private string strManagerName = "";

            public string ManagerName
            {
                get { return strManagerName; }
                set { strManagerName = value; }
            }
            private Zone zone = null;

            public Zone Zone
            {
                get { return zone; }
                set { zone = value; }
            }


        }

        public class ReactionIntrusionLog : IComparable
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

            private ArrayList arrLogList = new ArrayList();
            public ArrayList ArrLogList
            {
                get { return arrLogList; }
                set { arrLogList = value; }
            }
            //화재신고위치
            private string strUserName = "";
            public string UserName
            {
                get { return strUserName; }
                set { strUserName = value; }
            }

            private string facilityType = "";
            public string FacilityType
            {
                get { return facilityType; }
                set { facilityType = value; }
            }

            public override string ToString()
            {
                int nfacilityType;

                string strReactionType = "";

                if (nReactionType == (int)ReactionType.NOTIFY_SIGNAL)
                {
                    strReactionType = "방범 신고";
                }
                else if (nReactionType == (int)ReactionType.MALFUNCTION)
                    strReactionType = "오작동 처리";
                else if (nReactionType == (int)ReactionType.IGNORE_SIGNAL)
                {
                    if (ReactionIntrusionManager.IsS1SVMS(SensorType))
                        strReactionType = "S1SVMS 무시";
                    else if (ReactionIntrusionManager.IsS1Access(SensorType))
                        strReactionType = "S1ACCESS 무시";
                    else if (ReactionIntrusionManager.IsEmpoll(SensorType))
                        strReactionType = "외부비상벨 무시";
                    else if (ReactionIntrusionManager.IsSecom(SensorType))
                    {
                        if (SensorType == (int)IFacility.FacilityType.SecomExternalAlarmBell)
                            strReactionType = "외부비상벨 시작";
                        else if (SensorType == (int)IFacility.FacilityType.SecomWomenAlarmBell)
                            strReactionType = "여자화장실 비상벨 시작";
                        else
                            strReactionType = "Secom 시작";
                    }
                    else
                        strReactionType = "방범탐지 후 상황해제";
                }
                else if (nReactionType == (int)ReactionType.BEGIN_STATUS)
                {
                    if (ReactionIntrusionManager.IsS1SVMS(SensorType))
                        strReactionType = "S1SVMS 시작";
                    else if (ReactionIntrusionManager.IsS1Access(SensorType))
                        strReactionType = "S1ACCESS 시작";
                    else if (ReactionIntrusionManager.IsEmpoll(SensorType))
                        strReactionType = "외부비상벨 시작";
                    else if (ReactionIntrusionManager.IsSecom(SensorType))
                    {
                        if (SensorType == (int)IFacility.FacilityType.SecomExternalAlarmBell)
                            strReactionType = "외부비상벨 무시";
                        else if (SensorType == (int)IFacility.FacilityType.SecomWomenAlarmBell)
                            strReactionType = "여자화장실 비상벨 무시";
                        else
                            strReactionType = "Secom 무시";
                    }
                    else
                        strReactionType = "방범신호 무시";
                }
                else if (nReactionType == (int)ReactionType.END_STATUS)
                {
                    if (ReactionIntrusionManager.IsS1SVMS(SensorType))
                        strReactionType = "S1SVMS 종료";
                    else if (ReactionIntrusionManager.IsS1Access(SensorType))
                        strReactionType = "S1ACCESS 종료";
                    else if (ReactionIntrusionManager.IsEmpoll(SensorType))
                        strReactionType = "외부비상벨 종료";
                    else if (ReactionIntrusionManager.IsSecom(SensorType))
                    {
                        if (SensorType == (int)IFacility.FacilityType.SecomExternalAlarmBell)
                            strReactionType = "외부비상벨 종료";
                        else if (SensorType == (int)IFacility.FacilityType.SecomWomenAlarmBell)
                            strReactionType = "여자화장실 비상벨 종료";
                        else
                            strReactionType = "Secom 종료";
                    }
                    else
                        strReactionType = "방범신호 종료";
                }
                /*else if (nReactionType == (int)ReactionType.NOTIFY_SECURITY)
                    strReactionType = "방범 신고";
                else if (nReactionType == (int)ReactionType.BEGIN_S1SVMS_STATUS)
                    strReactionType = "S1SVMS 시작";
                else if (nReactionType == (int)ReactionType.IGNORE_S1SVMS_STATUS)
                    strReactionType = "S1SVMS 무시";
                else if (nReactionType == (int)ReactionType.END_S1SVMS_STATUS)
                    strReactionType = "S1SVMS 종료";
                else if (nReactionType == (int)ReactionType.BEGIN_S1ACCESS_STATUS)
                {
                    if (int.TryParse(facilityType, out nfacilityType) && (int)IFacility.FacilityType.ExternalAlarmBell == Convert.ToInt32(facilityType))
                        strReactionType = "외부비상벨 시작";
                    else
                        strReactionType = "S1ACCESS 시작";
                }
                else if (int.TryParse(facilityType, out nfacilityType) && nReactionType == (int)ReactionType.IGNORE_S1ACCESS_STATUS)
                {
                    if ((int)IFacility.FacilityType.ExternalAlarmBell == Convert.ToInt32(facilityType))
                        strReactionType = "외부비상벨 무시";
                    else
                        strReactionType = "S1ACCESS 무시";
                }
                else if (int.TryParse(facilityType, out nfacilityType) && nReactionType == (int)ReactionType.END_S1ACCESS_STATUS)
                {
                    if ((int)IFacility.FacilityType.ExternalAlarmBell == Convert.ToInt32(facilityType))
                        strReactionType = "외부비상벨 종료";
                    else
                        strReactionType = "S1ACCESS 종료";
                }

                else if (nReactionType == (int)ReactionType.BEGIN_SECOM_STATUS)
                {
                    if (int.TryParse(facilityType, out nfacilityType) && (int)IFacility.FacilityType.SecomExternalAlarmBell == Convert.ToInt32(facilityType))
                        strReactionType = "외부비상벨 시작";
                    else if (int.TryParse(facilityType, out nfacilityType) && (int)IFacility.FacilityType.SecomWomenAlarmBell == Convert.ToInt32(facilityType))
                        strReactionType = "여자화장실 비상벨 시작";
                    else
                        strReactionType = "Secom 시작";
                }
                else if (int.TryParse(facilityType, out nfacilityType) && nReactionType == (int)ReactionType.IGNORE_SECOM_STATUS)
                {
                    if ((int)IFacility.FacilityType.ExternalAlarmBell == Convert.ToInt32(facilityType))
                        strReactionType = "외부비상벨 무시";
                    else if ((int)IFacility.FacilityType.SecomWomenAlarmBell == Convert.ToInt32(facilityType))
                        strReactionType = "여자화장실 비상벨 무시";
                    else
                        strReactionType = "Secom 무시";
                }
                else if (int.TryParse(facilityType, out nfacilityType) && nReactionType == (int)ReactionType.END_SECOM_STATUS)
                {
                    if ((int)IFacility.FacilityType.ExternalAlarmBell == Convert.ToInt32(facilityType))
                        strReactionType = "외부비상벨 종료";
                    else if ((int)IFacility.FacilityType.SecomWomenAlarmBell == Convert.ToInt32(facilityType))
                        strReactionType = "여자화장실 비상벨 종료";
                    else
                        strReactionType = "Secom 종료";
                }*/

                if (nReactionType == (int)ReactionType.NOTIFY_SIGNAL)
                //if (nSensorType == 0)
                {  
                    return String.Format("{0:0000}-{1:00}-{2:00} {3} {4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour < 12 ? "오전" : "오후", time.Hour > 12 ? time.Hour - 12 : time.Hour, time.Minute)
                        + "   [ " + SOPServer.EventTypeString.GetEventTypeDetectString(Convert.ToInt32(facilityType)) + " 신고 ] " + strReactionType; 
                }
                else
                { 
                    return String.Format("{0:0000}-{1:00}-{2:00} {3} {4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour < 12 ? "오전" : "오후", time.Hour > 12 ? time.Hour - 12 : time.Hour, time.Minute)
                        + "   [ " + SOPServer.EventTypeString.GetEventTypeDetectString(Convert.ToInt32(facilityType)) + " 탐지 ] " + strReactionType;
                }
            }

            public int CompareTo(object obj)
            {
                ReactionIntrusionLog data = (ReactionIntrusionLog)obj;

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