using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using libSensorProcess;
using System.Collections;

namespace SDMS
{
    namespace Report
    {
        // 온도/습도 리포트 매니저
        public class ReactionTHManager
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

            private List<DetectTHLog> m_arrDectectList = null;
            public List<DetectTHLog> DectectList
            {
                get { return m_arrDectectList; }
                set { m_arrDectectList = value; }
            }

            private List<MulFunctionTHLog> m_arrMulfunctionList = null;
            public List<MulFunctionTHLog> MulfunctionList
            {
                get { return m_arrMulfunctionList; }
                set { m_arrMulfunctionList = value; }
            }

            private List<ReactionTHLog> m_arrReactionHistory = new List<ReactionTHLog>();

            // Key ; HistoryID
            private Dictionary<int, List<SensorReactionTHLog>> m_dicHistoryLog = new Dictionary<int, List<SensorReactionTHLog>>();
            //SensorZone ID,HistoryID List
            private Dictionary<int, List<int>> m_dicSensorHistorys = new Dictionary<int, List<int>>();

            //HistoryID, Zone
            private Dictionary<int, Zone> m_dicZoneHistorys = new Dictionary<int, Zone>();

            //HistoryID, ReactionType
            private Dictionary<int, int> m_dicHistoryType = new Dictionary<int, int>();

            private Dictionary<MulFunctionTHLog, List<SensorReactionTHLog>> m_dicMulFuctionSrLog = new Dictionary<MulFunctionTHLog, List<SensorReactionTHLog>>();
            internal Dictionary<MulFunctionTHLog, List<SensorReactionTHLog>> DicMulFuctionSrLog
            {
                get { return m_dicMulFuctionSrLog; }
                set { m_dicMulFuctionSrLog = value; }
            }

            private List<SensorReactionTHLog> arrAllReactionLog = new List<SensorReactionTHLog>();

            public List<SensorReactionTHLog> AllReactionLog
            {
                get { return arrAllReactionLog; }
            }

            private Dictionary<string, string> m_dicGenUserIDDNicName = new Dictionary<string, string>();
            //HistoryID, Param3(MemberID)
            private Dictionary<int, string> m_dicHistoryMember = new Dictionary<int, string>();

            private int m_nSiteID = 1;
            public ReactionTHManager()
            {
                m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

                m_arrDectectList = new List<DetectTHLog>();
            }

            public void DataClear()
            {
                if (m_arrDectectList != null)
                    m_arrDectectList.Clear();
                if (m_arrMulfunctionList != null)
                    m_arrMulfunctionList.Clear();
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

            //탐지, 처리이력
            // Return 값 : true이면 데이터가 변경되었음.
            //             false이면 데이터 변경없음.
            public void ZoneSubmit(ArrayList arrZoneList, DateTime startDate, DateTime endDate, int pageType = 1)//pageType이 1이면 탐지/처리 2이면 대응이력
            {
                LoadSOPGenUser();

                string strNowDate, strBeforeDate;
                GetZoneSumitDate(pageType, startDate, endDate, out strBeforeDate, out strNowDate);

                //선택한 ZoneID 리스트로 EquipmentZoneID를 찾는다.
                List<EquipmentZone> arrEquipmentZoneList = FindEquipZone(arrZoneList);
                //가져온 EquipmentZoneID 리스트로 SensorID를 찾아온다.
                Dictionary<int, ISensor> dicSensorZones = FindSensorZone(arrEquipmentZoneList);
                //ArrayList arrSensorZoneList = FindSensorZone(arrEquipmentZoneList);
                //SensorID리스트로 SensorHistoryID를 찾아옴
                List<int> arrZoneHistoryList = GetSensorZoneHistoryID(dicSensorZones, strBeforeDate, strNowDate);
                //ArrayList arrZoneHistoryList = GetSensorZoneHistoryID(arrSensorZoneList, strBeforeDate, strNowDate);
                //ReactionLog를 가져옴
                List<SensorReactionTHLog> arrReactionList = GetReactionHistory(arrZoneHistoryList);

                if (arrReactionList != null)
                    arrAllReactionLog = arrReactionList;
                else
                    arrAllReactionLog = new List<SensorReactionTHLog>();

                //오작동이력 로그 저장
                m_arrMulfunctionList = GetMulFunctionLog(arrZoneList, strBeforeDate, strNowDate);


                //전체 ReactionLog중에 온도/습도 알람 탐지된 로그만 가져와서 저장함
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

            public List<ReactionTHLog> HistorySubmit(DateTime startDate, DateTime endDate)
            {
                m_arrReactionHistory.Clear();
                endDate = endDate.AddDays(1);

                foreach (KeyValuePair<int, List<SensorReactionTHLog>> pair in m_dicHistoryLog)
                {
                    int nHistoryID = pair.Key;
                    List<SensorReactionTHLog> log = pair.Value;
                    int nReactionType = 0;
                    Zone zone = null;
                    string strMemberID = "";

                    if (m_dicZoneHistorys.ContainsKey(nHistoryID))
                        zone = m_dicZoneHistorys[nHistoryID];

                    ReactionTHLog reactionLog = new ReactionTHLog();
                    reactionLog.HistoryID = nHistoryID;
                    reactionLog.ArrLogList = log;

                    //자탐 ReactionType가져옴
                    if (m_dicHistoryType.ContainsKey(nHistoryID))
                    {
                        nReactionType = m_dicHistoryType[nHistoryID];
                    }

                    //가장 맨 처음 발생한 ReactionLog를 Comobox로 보여줘야 하므로 log배열의 가장 첫번째 값을 가져온다
                    SensorReactionTHLog sensorreactionLog = log[0];

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
                    reactionLog.FacilityType = sensorreactionLog.Param3;
                    reactionLog.Type = nReactionType;

                    m_arrReactionHistory.Add(reactionLog);
                }
                m_arrReactionHistory.Sort();
                return m_arrReactionHistory;
            }

            public List<ReactionTHLog> GetReactionLog(int nHistoryID)
            {
                List<ReactionTHLog> arrReactLog = new List<ReactionTHLog>();

                foreach (KeyValuePair<int, List<SensorReactionTHLog>> pair in m_dicHistoryLog)
                {
                    int nSensorHistoryID = pair.Key;
                    List<SensorReactionTHLog> log = pair.Value;
                    string strMemberID = "";

                    if (nSensorHistoryID == nHistoryID)
                    {
                        Zone zone = null;
                        if (m_dicZoneHistorys.ContainsKey(nHistoryID))
                            zone = m_dicZoneHistorys[nHistoryID];

                        foreach (SensorReactionTHLog srLog in log)
                        {
                            ReactionTHLog reactionLog = new ReactionTHLog();
                            reactionLog.HistoryID = nHistoryID;
                            reactionLog.ArrLogList = log;

                            if (m_dicHistoryMember.ContainsKey(nHistoryID))
                                strMemberID = m_dicHistoryMember[nHistoryID];

                            if (m_dicGenUserIDDNicName.ContainsKey(strMemberID))
                                reactionLog.UserName = m_dicGenUserIDDNicName[strMemberID];

                            reactionLog.equipZone = ZoneManager.Instance.GetEquipZone(srLog.Param1);

                            if (srLog.ReactionType == (int)ReactionType.MALFUNCTION
                                || srLog.ReactionType == (int)ReactionType.NOTIFY_SIGNAL
                                || srLog.ReactionType == (int)ReactionType.IGNORE_SIGNAL)
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

            private List<DetectTHLog> GetDetectLog(List<SensorReactionTHLog> arrAllLog)
            {
                List< DetectTHLog> arrDetectLog = new List<DetectTHLog>();
                ArrayList arrComboBoxDate = new ArrayList();
                //ArrayList arrReactionLog = new ArrayList();

                // Key : SensorZoneHistory ID
                Dictionary<int, List<DetectTHLog>> dicDetectLog = new Dictionary<int, List<DetectTHLog>>();
                List<DetectTHLog> logs = null;
                string strSensorZoneHistoryIDs = "";

                foreach (SensorReactionTHLog reactionLog in arrAllLog)
                {
                    if (reactionLog.ReactionType == (int)ReactionType.BEGIN_STATUS || (reactionLog.ReactionType == (int)ReactionType.NOTIFY_SIGNAL && reactionLog.Param2 == "0"))
                    {
                        DetectTHLog detect = new DetectTHLog();

                        detect.SensorReactionHistoryID = reactionLog.ID;
                        detect.HistoryID = reactionLog.SensorHistoryID;
                        detect.Time = reactionLog.Time;
                        detect.Memo = reactionLog.Memo;

                        if (dicDetectLog.TryGetValue(detect.HistoryID, out logs) == false)
                        {
                            logs = new List<DetectTHLog>();
                            dicDetectLog[detect.HistoryID] = logs;

                            if (strSensorZoneHistoryIDs.Length == 0)
                                strSensorZoneHistoryIDs = detect.HistoryID.ToString();
                            else
                                strSensorZoneHistoryIDs += "," + detect.HistoryID.ToString();
                        }

                        logs.Add(detect);

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

                        //EquipZone구하기
                        detect.EquipZone = ZoneManager.Instance.GetEquipZone(reactionLog.Param1);

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

                        int nDetectType;

                        if (int.TryParse(reactionLog.Param3, out nDetectType))
                        {
                            detect.DetectType = SOPServer.EventTypeString.GetEventTypeDetectString(nDetectType/*Convert.ToInt32(reactionLog.Param3)*/);
                        }
                        else
                            detect.DetectType = GetReactionString();
                        
                        detect.DetectionStatusName = GetDetectionStatusName(reactionLog.DetectionStatus);

                        arrDetectLog.Add(detect);
                    }
                }

                SetAlarmTypeData(dicDetectLog, strSensorZoneHistoryIDs);

                //
                //arrDetectLog.Sort();
                return arrDetectLog;
            }

            private void SetAlarmTypeData(Dictionary<int, List<DetectTHLog>> dicDetectLog, string strSensorZoneHistoryIDs)
            {
                if (strSensorZoneHistoryIDs.Length == 0)
                    return;

                Dictionary<int, string> dicAlarmType = GetTHAlarmType();

                if (dicAlarmType == null)
                    return;

                string strSQL = "Select ID, Data from SensorZoneHistory where ID in (" + strSensorZoneHistoryIDs + ")";
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

                if (arrResult == null)
                    return;

                List<DetectTHLog> logs = null;
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<int> sensorData = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                    if (id == null || sensorData == null)
                        continue;

                    if (dicDetectLog.TryGetValue(id.Data, out logs) == false)
                        continue;

                    string strAlarmName = GetTHAlarmName(sensorData.Data, dicAlarmType);

                    foreach (DetectTHLog log in logs)
                    {
                        log.AlarmTypeData = sensorData.Data;
                        log.AlarmType = strAlarmName;
                    }
                }
            }

            private string GetTHAlarmName(int nSensorData, Dictionary<int, string> dicAlarmType)
            {
                string strAlarmName = "";

                foreach (KeyValuePair<int, string> pair in dicAlarmType)
                {
                    int nFlag = 1 << (pair.Key - 1);

                    if ((nSensorData & nFlag) == nFlag)
                    {
                        if (strAlarmName.Length == 0)
                            strAlarmName = pair.Value;
                        else
                            strAlarmName += ", " + pair.Value;
                    }
                }

                return strAlarmName;
            }

            private Dictionary<int, string> GetTHAlarmType()
            {
                string strSQL = "Select ID, AlarmName from THAlarmType";
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

                if (arrResult == null)
                    return null;

                Dictionary<int, string> dicAlarmType = new Dictionary<int, string>();
                int nResultCount = arrResult.Count;

                for (int i=0;i<nResultCount-1;i+=2)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strAlarmName = WebDBManager.GetStringField(arrResult[i + 1]);

                    if (id == null || strAlarmName == null)
                        continue;

                    dicAlarmType[id.Data] = strAlarmName;
                }

                return dicAlarmType;
            }

            private List<MulFunctionTHLog> GetMulFunctionLog(ArrayList arrZoneList, string strStartDate, string strEndDate)
            {
                List< MulFunctionTHLog> arrMulFunction = new List<MulFunctionTHLog>();
                List<int> liAddedReactionHistoryIDs = new List<int>();

                //Zone별로 Log에서 탐지,신고,오작동,처리되지않은신호의 갯수, 오작동률 등을 구함
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

                    //오작동이력 클래스 생성
                    MulFunctionTHLog mulfuction = new MulFunctionTHLog();

                    int nReportCount = 0;
                    int nMulFunctionCount = 0;
                    int nNotprocessCount = 0;
                    int nOnlyDetectCount = 0;

                    foreach (int nHistoryID in arrHistoryList)
                    {
                        List<SensorReactionTHLog> arrLog = new List<SensorReactionTHLog>();

                        if (m_dicHistoryLog.ContainsKey(nHistoryID))
                            arrLog = m_dicHistoryLog[nHistoryID];

                        int nType = 0;

                        foreach (SensorReactionTHLog log in arrLog)
                        {
                            if (log.ReactionType == 0)
                            {
                                List< SensorReactionTHLog> arrSensorLog = null;

                                //<MulFunctionLog, SensorReactionLog> Dictionary에 값 추가
                                if (m_dicMulFuctionSrLog.ContainsKey(mulfuction))
                                    arrSensorLog = m_dicMulFuctionSrLog[mulfuction];
                                else
                                {
                                    arrSensorLog = new List<SensorReactionTHLog>();
                                    m_dicMulFuctionSrLog[mulfuction] = arrSensorLog;
                                }
                                arrSensorLog.Add(log);
                            }

                            if (log.ReactionType == (int)ReactionType.NOTIFY_SIGNAL)
                            {
                                nReportCount++;
                                nType = log.ReactionType;

                                break;
                            }
                            else if (log.ReactionType == (int)ReactionType.MALFUNCTION)
                            {
                                nMulFunctionCount++;
                                nType = log.ReactionType;

                                break;
                            }
                            // 처리되지 않음.
                            else if (log.ReactionType == (int)ReactionType.IGNORE_SIGNAL)
                            {
                                nType = log.ReactionType;

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

                    mulfuction.ReactionCount = arrHistoryList.Count;

                    //처리되지 않음
                    nNotprocessCount = arrHistoryList.Count - (nReportCount + nMulFunctionCount) - nOnlyDetectCount;

                    double PercentMulFunction = (nMulFunctionCount * 100) / arrHistoryList.Count;

                    mulfuction.HistoryIDList = arrHistoryList;
                    mulfuction.DetectType = GetReactionString();

                    mulfuction.ReportCount = nReportCount;
                    mulfuction.UserResetCount = nMulFunctionCount;
                    mulfuction.Zone = zone;
                    mulfuction.ManagerName = FindManagerName(zone);
                    mulfuction.Notprocess = nNotprocessCount;
                    mulfuction.PercentMulFunction = PercentMulFunction;
                    mulfuction.OnlyDetectCount = nOnlyDetectCount;

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
                //오작동이력로그들을 배열에 저장
                return arrMulFunction;
            }

            private void ReadReactionLogMemo(int nMinReactionLogID, int nMaxReactionLogID, Dictionary<int, SensorReactionTHLog> dicReactionLogs)
            {
                if (dicReactionLogs.Count == 0)
                    return;

                string strNotIncludeIDs = GetNotIncludeIDs<SensorReactionTHLog>("SensorReactionHistory", nMinReactionLogID, nMaxReactionLogID, dicReactionLogs);
                string strCondition = MakeConditionWithNotIncludeIDs("SensorReactionHistoryID", nMinReactionLogID, nMaxReactionLogID, strNotIncludeIDs);

                string strSQL = "Select SensorReactionHistoryID, Description ";
                strSQL += "from SensorReactionHistoryDescription as memo, SensorReactionHistoryDescriptionText as memoText ";
                strSQL += "where memo.DescriptionID = memoText.ID and " + strCondition;

                WebDBManager webDB = FormMain.Instance.DBManager;
                ArrayList arrResult = webDB.GetResultData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;
                SensorReactionTHLog log = null;

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
                foreach (KeyValuePair<int, SensorReactionTHLog> pair in dicReactionLogs)
                {
                    string strMemo = null;

                    if (dicSensorZoneHistoryMemo.TryGetValue(pair.Value.SensorHistoryID, out strMemo))
                    {
                        pair.Value.Memo = strMemo;
                    }
                }
            }

            public static string MakeConditionWithNotIncludeIDs(string strFieldName, int nMinID, int nMaxID, string strNotIncludeIDs)
            {
                string strCondition = "";

                if (strNotIncludeIDs.Length > 0)
                    strCondition = string.Format("{0} >= {1} and {0} <= {2} and {0} not in ({3})", strFieldName, nMinID, nMaxID, strNotIncludeIDs);
                else
                    strCondition = string.Format("{0} >= {1} and {0} <= {2}", strFieldName, nMinID, nMaxID);

                return strCondition;
            }

            // nMinID와 nMaxID 사이에 있는 값중에 dicIDs에 포함되지 않는 리스트를 얻어온다.
            public static string GetNotIncludeIDs<LogType>(string strTableName, int nMinID, int nMaxID, Dictionary<int, LogType> dicIDs)
            {
                string strSQL = "Select ID from " + strTableName + " where ID >= " + nMinID.ToString() + " and ID <= " + nMaxID.ToString();
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

                if (arrResult == null)
                    return "";

                string strNotIncludeIDs = "";
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount; i++)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                    if (id == null)
                        continue;

                    if (dicIDs.ContainsKey(id.Data) == false)
                    {
                        if (strNotIncludeIDs.Length == 0)
                            strNotIncludeIDs = id.Data.ToString();
                        else
                            strNotIncludeIDs += ", " + id.Data.ToString();
                    }
                }

                return strNotIncludeIDs;
            }

            private List<SensorReactionTHLog> GetReactionHistory(List<int> arrSensorHistoryID)
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

                List<SensorReactionTHLog> arrReactionLog = new List<SensorReactionTHLog>();
                if (strSensorList == "")
                    return arrReactionLog;

                WebDBManager webDB = FormMain.Instance.DBManager;

                string strSQL = "select srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, srh.DetectionStatus, sz.Type ";
                strSQL += "from SensorReactionHistory as srh, SensorZoneHistory as szh, SensorZone as sz ";
                strSQL += "where SensorHistoryID in (" + strSensorList + ") and srh.SensorHistoryID = szh.ID and szh.SensorID = sz.ID";

                ArrayList arrResult = webDB.GetResultData(strSQL);
                if (arrResult == null)
                    return null;

                int nResultCount = arrResult.Count;
                DateTime dt = DateTime.Now;

                Dictionary<int, SensorReactionTHLog> dicReactionLogs = new Dictionary<int, SensorReactionTHLog>();
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

                    SensorReactionTHLog reactionLog = new SensorReactionTHLog();
                    reactionLog.ID = nID;
                    reactionLog.SensorHistoryID = nSensorHistoryID;
                    reactionLog.ReactionType = nReactionType;
                    reactionLog.Time = time;
                    reactionLog.DetectionStatus = nDetectionStatus;

                    reactionLog.SensorType = nSensorType;

                    if (nReactionType == (int)libSensorProcess.ReactionType.MALFUNCTION || nReactionType == (int)libSensorProcess.ReactionType.USER_RESET ||
                        nReactionType == (int)libSensorProcess.ReactionType.NOTIFY_SIGNAL || nReactionType == (int)libSensorProcess.ReactionType.IGNORE_SIGNAL)
                    {
                        if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                            m_dicHistoryMember.Add(nSensorHistoryID, Param3);
                    }

                    reactionLog.Param1 = Param1;
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

                    List<SensorReactionTHLog> arrLogs = null;

                    //
                    if (m_dicHistoryLog.ContainsKey(nSensorHistoryID))
                        arrLogs = m_dicHistoryLog[nSensorHistoryID];
                    else
                    {
                        arrLogs = new List<SensorReactionTHLog>();
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
            private List<int> GetSensorZoneHistoryID(Dictionary<int, ISensor> dicSensorZones, string startDate, string endDate)
            {
                List<int> arrSensorZoneHistoryID = new List<int>();

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

                string strSQL = "select id,SensorID from SensorZoneHistory where Time Between '" + startDate + "' and '" + endDate + "' and ( Data > 0)";

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

                    List<int> arrLogs = null;

                    if (m_dicSensorHistorys.ContainsKey(nSensorID))
                        arrLogs = m_dicSensorHistorys[nSensorID];
                    else
                    {
                        arrLogs = new List<int>();
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

            private List<int> FindHistoryID(int nZoneID)
            {
                List<int> arrHistoryIDList = new List<int>();
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                //자탐Log
                List<EquipmentZone> arrEquipmentZoneList = ZoneManager.Instance.GetEquipmentZoneList(zone);
                Dictionary<int, ISensor> dicSensorZones = FindSensorZone(arrEquipmentZoneList);

                if (dicSensorZones == null)
                    return null;

                List<int> histories = null;

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
                    List<ISensor> arSensors = SensorManager.Instance.FindZoneInSensor(equip.ID, IFacility.FacilityType.TEMPERATURE_HUMIDITY);

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
                    ArrayList arSensors = SensorManager.Instance.FindZoneInSensor(equip.ID, IFacility.FacilityType.FIRE_SENSOR);

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

            private string GetReactionString()
            {
                string strType = "온도/습도 센서";
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
                foreach (SensorReactionTHLog reactionHistory in m_dicHistoryLog[nSensorReactionHistorySensorHistoryID])
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
        }

        public class SensorReactionTHLog
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

            // UnE.Sensor.IFacility.FacilityType
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

        public class DetectTHLog : IComparable
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
            private int m_nAlarmTypeData = 0;
            private string m_strAlarmType = "";

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

            public int AlarmTypeData
            {
                get { return m_nAlarmTypeData; }
                set { m_nAlarmTypeData = value; }
            }

            public string AlarmType
            {
                get { return m_strAlarmType; }
                set { m_strAlarmType = value; }
            }

            public DetectTHLog()
            {

            }


            public int CompareTo(object b)
            {
                DetectTHLog data = this;
                DetectTHLog data2 = (DetectTHLog)b;

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

        public class MulFunctionTHLog
        {
            private List<int> nHistoryIDList = new List<int>();

            public List<int> HistoryIDList
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
            // 신호복구 횟수
            private int m_nUserResetCount = 0;

            public int UserResetCount
            {
                get { return m_nUserResetCount; }
                set { m_nUserResetCount = value; }
            }

            // 신고 횟수
            private int m_nReportCount = 0;
            public int ReportCount
            {
                get { return m_nReportCount; }
                set { m_nReportCount = value; }
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

        public class ReactionTHLog : IComparable
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

            private List<SensorReactionTHLog> arrLogList = new List<SensorReactionTHLog>();
            public List<SensorReactionTHLog> ArrLogList
            {
                get { return arrLogList; }
                set { arrLogList = value; }
            }

            // 신고위치
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
                string strReactionType = string.Empty;
                if (nReactionType == (int)ReactionType.NOTIFY_SIGNAL)
                    strReactionType = "알람 발생";
                else if (nReactionType == (int)ReactionType.MALFUNCTION)
                    strReactionType = "오작동 처리";
                else if (nReactionType == (int)ReactionType.USER_RESET)
                    strReactionType = "시스템 복구";
                else if (nReactionType == (int)ReactionType.IGNORE_SIGNAL)
                    strReactionType = "알람탐지 후 상황해제";
                else if (nReactionType == (int)ReactionType.BEGIN_STATUS)
                    strReactionType = "알람 탐지";

                return String.Format("{0:0000}-{1:00}-{2:00} {3} {4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour < 12 ? "오전" : "오후", time.Hour > 12 ? time.Hour - 12 : time.Hour, time.Minute)
                    + "   [ " + SOPServer.EventTypeString.GetEventTypeDetectString(Convert.ToInt32(facilityType)) + " ] " + strReactionType;
            }

            public int CompareTo(object obj)
            {
                ReactionTHLog data = (ReactionTHLog)obj;

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
