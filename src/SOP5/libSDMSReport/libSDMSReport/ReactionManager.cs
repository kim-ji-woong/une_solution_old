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
    public abstract class ReactionManager
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

        // 화재센서, 소화센서, 압력센서, 수동신고
        public enum DetectType { UNKNOWN = 0, FIRE = 1, COOLER, PRESSURE, MANUAL, PSM = 7 };

        protected IZoneManager m_zoneManager = null;
        protected ISensorManager m_sensorManager = null;
        protected WebDBManager m_dbMgr = null;
        protected int m_nSiteID = 1;
        protected IReportOwner m_owner = null;

        protected List<DetectLog> m_detectList = new List<DetectLog>();
        protected List<Statistics> m_statisticsList = new List<Statistics>();
        // Key : SensorZoneHistory ID
        protected Dictionary<int, List<SensorReactionLog>> m_dicHistorySensorReactionLog = new Dictionary<int, List<SensorReactionLog>>();
        // Key : SensorZone ID
        // Value : SensorZoneHistory ID List
        protected Dictionary<int, List<int>> m_dicSensorHistories = new Dictionary<int, List<int>>();
        // Key : SensorZoneHistory ID
        protected Dictionary<int, Zone> m_dicZoneHistories = new Dictionary<int, Zone>();
        // Key : SensorZoneHistory ID
        // Value : libSensorProcess.ReactionType
        protected Dictionary<int, int> m_dicHistoryType = new Dictionary<int, int>();
        protected List<SensorReactionLog> m_allReactionLogs = new List<SensorReactionLog>();
        protected Dictionary<Statistics, List<SensorReactionLog>> m_dicStatisticsReactionLog = new Dictionary<Statistics, List<SensorReactionLog>>();
        protected List<ReactionLog> m_reactionHistories = new List<ReactionLog>();
        // Key : SOPGenUser ID
        // Value : NickName
        protected Dictionary<string, string> m_dicGenUserIDDNicName = new Dictionary<string, string>();
        // SensorZoneHistory ID, SOPGenUser ID
        protected Dictionary<int, string> m_dicHistoryMember = new Dictionary<int, string>();

        public IZoneManager ZoneManager
        {
            get { return m_zoneManager; }
            set { m_zoneManager = value; }
        }

        public ISensorManager SensorManager
        {
            get { return m_sensorManager; }
            set { m_sensorManager = value; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public IReportOwner Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        // 이전과 같은 데이터인지 검사한다.
        // isActionPage : 대응이력(true), 탐지이력(false)
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

            if (ZoneManager == null)
                return true;

            bool allZones = ZoneManager.GetZoneCount() == arrZoneList.Count;

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

        private void GetZoneSumitDate(bool isActionPage, DateTime startDate, DateTime endDate, out string strBeforeDate, out string strNowDate)
        {
            strNowDate = "";
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", startDate.ToShortDateString(), "00", "00", "00");

            if (isActionPage)//대응이력은 시작날과 종료날이 같을경우 시간까지 조절해야하므로 ..
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

        private int GetMaxSensorReactionHistoryID()
        {
            if (DBManager == null)
                return -1;

            string strSQL = "Select max(ID) from SensorReactionHistory";
            ArrayList arrResult = DBManager.GetResultData(strSQL, 0);

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

        public void ClearData()
        {
            if (m_detectList != null)
                m_detectList.Clear();
            if (m_statisticsList != null)
                m_statisticsList.Clear();
            if (m_dicHistorySensorReactionLog != null)
                m_dicHistorySensorReactionLog.Clear();
            if (m_dicSensorHistories != null)
                m_dicSensorHistories.Clear();
            if (m_dicZoneHistories != null)
                m_dicZoneHistories.Clear();
            if (m_dicHistoryType != null)
                m_dicHistoryType.Clear();
            if (m_allReactionLogs != null)
                m_allReactionLogs.Clear();
            if (m_dicStatisticsReactionLog != null)
                m_dicStatisticsReactionLog.Clear();
            if (m_reactionHistories != null)
                m_reactionHistories.Clear();
        }

        //탐지, 처리이력
        // isActionPage : 대응이력(true), 탐지이력(false)
        // Return 값 : true이면 데이터가 변경되었음.
        //             false이면 데이터 변경없음.
        public void ZoneSubmit(ArrayList arrZoneList, DateTime startDate, DateTime endDate, bool isActionPage = false)
        {
            LoadSOPGenUser();

            string strNowDate, strBeforeDate;
            GetZoneSumitDate(isActionPage, startDate, endDate, out strBeforeDate, out strNowDate);

            //ZoneID 리스트로 ReactionHistory의 수동신고의 log를 가져온다.
            List<SensorReactionLog> arrExternalReactionHistory = GetExternalReactionHistory(arrZoneList, strBeforeDate, strNowDate);

            //선택한 ZoneID 리스트로 EquipmentZoneID를 찾는다.
            List<EquipmentZone> arrEquipmentZoneList = FindEquipZone(arrZoneList);
            //가져온 EquipmentZoneID 리스트로 SensorID를 찾아온다.
            Dictionary<int, ISensor> dicSensorZones = FindSensorZone(arrEquipmentZoneList);
            //ArrayList arrSensorZoneList = FindSensorZone(arrEquipmentZoneList);
            //SensorID리스트로 SensorHistoryID를 찾아옴
            List<int> arrSensorZoneHistoryIDs = GetSensorZoneHistoryID(dicSensorZones, strBeforeDate, strNowDate);
            //ArrayList arrZoneHistoryList = GetSensorZoneHistoryID(arrSensorZoneList, strBeforeDate, strNowDate);
            //ReactionLog를 가져옴
            List<SensorReactionLog> arrReactionList = GetReactionHistory(arrSensorZoneHistoryIDs);

            //수동신고와 자탐의 SensorReactionLog를 합친다.
            m_allReactionLogs = AddReactionHistoryLog(arrExternalReactionHistory, arrReactionList);

            // 통계정보(오작동률, 실제 재난 비율...) 저장
            m_statisticsList = GetStatisticsLog(arrZoneList, strBeforeDate, strNowDate);

            //전체 ReactionLog중에 화재 탐지 된 로그만 가져와서 저장함
            //화재신고 된 로그만 가져옴(ReactionType=0 -> 자탐 / reactionLog.ReactionType == 22 && reactionLog.Param2 == "0" -> 수동
            m_detectList = GetDetectLog(m_allReactionLogs);
            m_detectList.Sort();
        }

        protected List<int> FindHistoryID(int nZoneID)
        {
            List<int> arrHistoryIDList = new List<int>();

            if (ZoneManager == null)
                return arrHistoryIDList;

            Zone zone = ZoneManager.GetZone(nZoneID);
            //자탐Log
            List<EquipmentZone> arrEquipmentZoneList = ZoneManager != null ? ZoneManager.GetEquipmentZoneList(zone) : null;
            Dictionary<int, ISensor> dicSensorZones = FindSensorZone(arrEquipmentZoneList);

            if (dicSensorZones == null)
                return arrHistoryIDList;

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

        private List<SensorReactionLog> AddReactionHistoryLog(List<SensorReactionLog> arrExternalReactionHistory, List<SensorReactionLog> arrReactionList)
        {
            List<SensorReactionLog> arrAllReactionLog = new List<SensorReactionLog>();

            if (arrReactionList != null)
                arrAllReactionLog.AddRange(arrReactionList);

            if (arrExternalReactionHistory != null)
                arrAllReactionLog.AddRange(arrExternalReactionHistory);

            return arrAllReactionLog;
        }

        private List<SensorReactionLog> GetReactionHistory(List<int> arrSensorZoneHistoryID)
        {
            if (arrSensorZoneHistoryID == null)
                return null;

            if (arrSensorZoneHistoryID.Count == 0)
                return null;

            string strSensorList = "";
            int nCount = 1;
            foreach (int nHistoryID in arrSensorZoneHistoryID)
            {
                strSensorList += nHistoryID.ToString();
                if (nCount != arrSensorZoneHistoryID.Count)
                    strSensorList += ",";

                nCount++;
            }

            List<SensorReactionLog> arrReactionLog = new List<SensorReactionLog>();
            if (strSensorList == "")
                return arrReactionLog;

            if (DBManager == null)
                return arrReactionLog;

            Dictionary<int, SensorReactionLog> dicReactionLogs = new Dictionary<int, SensorReactionLog>();
            int nMinReactionLogID = -1, nMaxReactionLogID = -1;

            if (_GetReactionHistory(arrReactionLog, strSensorList, dicReactionLogs, ref nMinReactionLogID, ref nMaxReactionLogID) == false)
                return null;

            // SensorReactionHistoryDescription 읽어오기
            ReadReactionLogMemo(nMinReactionLogID, nMaxReactionLogID, dicReactionLogs);

            return arrReactionLog;
        }

        //SensorID로 SensorHistoryID를 찾아옴
        // startDate와 endDate 사이의 모든 History들을 DB로부터 가져온다.
        // 가져온 DB 데이터들 가운데 dicSensorZoneIDs에 속하는 것들만 따로 추려낸다.
        private List<int> GetSensorZoneHistoryID(Dictionary<int, ISensor> dicSensorZones, string startDate, string endDate)
        {
            if (DBManager == null)
                return null;

            string strAlarmCondition = GetAlarmTypeQueryString("Data");

            string strSQL = "select id,SensorID from SensorZoneHistory where Time Between '" + startDate + "' and '" + endDate + "' and (" + strAlarmCondition + ")";

            ArrayList arrResult = DBManager.GetResultData(strSQL, 0);
            if (arrResult == null)
                return null;

            List<int> arrSensorZoneHistoryID = new List<int>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nSensorID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                if (dicSensorZones.ContainsKey(nSensorID) == false)
                    continue;

                arrSensorZoneHistoryID.Add(nID);

                List<int> arrLogs = null;

                if (m_dicSensorHistories.ContainsKey(nSensorID))
                    arrLogs = m_dicSensorHistories[nSensorID];
                else
                {
                    arrLogs = new List<int>();
                    m_dicSensorHistories[nSensorID] = arrLogs;
                }
                arrLogs.Add(nID);
            }

            return arrSensorZoneHistoryID;
        }

        private void LoadSOPGenUser()
        {
            if (DBManager == null)
                return;

            string strSQL = "select ID, NickName From SOPGenUser WHERE SiteID = " + m_nSiteID.ToString();

            ArrayList arrResult = DBManager.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            DateTime dt = DateTime.Now;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nMemberID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strNicName = WebDBManager.GetStringField(arrResult[i + 1], "");

                m_dicGenUserIDDNicName[nMemberID.ToString()] = strNicName;
            }
        }

        protected void ReadReactionLogMemo(int nMinReactionLogID, int nMaxReactionLogID, Dictionary<int, SensorReactionLog> dicReactionLogs)
        {
            if (DBManager == null)
                return;

            if (dicReactionLogs.Count == 0)
                return;

            string strNotIncludeIDs = GetNotIncludeIDs<SensorReactionLog>(DBManager, "SensorReactionHistory", nMinReactionLogID, nMaxReactionLogID, dicReactionLogs);
            string strCondition = MakeConditionWithNotIncludeIDs("SensorReactionHistoryID", nMinReactionLogID, nMaxReactionLogID, strNotIncludeIDs);

            string strSQL = "Select SensorReactionHistoryID, Description ";
            strSQL += "from SensorReactionHistoryDescription as memo, SensorReactionHistoryDescriptionText as memoText ";
            strSQL += "where memo.DescriptionID = memoText.ID and " + strCondition;

            ArrayList arrResult = DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            SensorReactionLog log = null;

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

                dicSensorZoneHistoryMemo[log.SensorZoneHistoryID] = strMemo;
            }

            // Memo는 특정 SensorReactionHistory에 속해있는 것이지만 Report에서 사용할 때에는 어차피
            // SensorZoneHistory별로 정렬되기 때문에 같은 SensorZoneHistory ID를 가지는 모든 SensorReactionHistory에 Memo를 공유한다.
            foreach (KeyValuePair<int, SensorReactionLog> pair in dicReactionLogs)
            {
                string strMemo = null;

                if (dicSensorZoneHistoryMemo.TryGetValue(pair.Value.SensorZoneHistoryID, out strMemo))
                {
                    pair.Value.Memo = strMemo;
                }
            }
        }

        //선택한 ZoneID로 EquipmentZoneID를 찾는다
        private List<EquipmentZone> FindEquipZone(ArrayList arrZoneList)
        {
            List<EquipmentZone> arrEquipZoneList = new List<EquipmentZone>();
            if (arrZoneList == null)
                return null;

            if (ZoneManager != null)
            {
                foreach (Zone zone in arrZoneList)
                {
                    if (ZoneManager.GetEquipmentZoneList(zone) == null)
                        continue;

                    arrEquipZoneList.AddRange(ZoneManager.GetEquipmentZoneList(zone));
                }
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

        public List<ReactionLog> GetReactionLog(int nSensorZoneHistoryID)
        {
            List<ReactionLog> arrReactLog = new List<ReactionLog>();

            foreach (KeyValuePair<int, List<SensorReactionLog>> pair in m_dicHistorySensorReactionLog)
            {
                int nSensorHistoryID = pair.Key;
                List<SensorReactionLog> log = pair.Value;
                //string strMemberID = "";

                if (nSensorHistoryID == nSensorZoneHistoryID)
                {
                    Zone zone = null;
                    if (m_dicZoneHistories.ContainsKey(nSensorZoneHistoryID))
                        zone = m_dicZoneHistories[nSensorZoneHistoryID];

                    foreach (SensorReactionLog srLog in log)
                    {
                        ReactionLog reactionLog = _GetReactionLog(srLog, log, nSensorZoneHistoryID, zone);
                        
                        if (reactionLog != null)
                            arrReactLog.Add(reactionLog);
                    }
                    break;
                }
            }

            // arrReactLog.Sort();
            return arrReactLog;
        }

        public List<ReactionLog> HistorySubmit(DateTime startDate, DateTime endDate)
        {
            m_reactionHistories.Clear();

            endDate = endDate.AddDays(1);
            foreach (KeyValuePair<int, List<SensorReactionLog>> pair in m_dicHistorySensorReactionLog)
            {
                int nHistoryID = pair.Key;
                List<SensorReactionLog> log = pair.Value;
                Zone zone = null;

                if (log.Count == 0)
                    continue;

                //가장 맨 처음 발생한 ReactionLog를 Comobox로 보여줘야 하므로 log배열의 가장 첫번째 값을 가져온다
                SensorReactionLog sensorreactionLog = (SensorReactionLog)log[0];

                if (!(sensorreactionLog.Time >= startDate && sensorreactionLog.Time <= endDate))
                    continue;

                if (m_dicZoneHistories.ContainsKey(nHistoryID))
                    zone = m_dicZoneHistories[nHistoryID];

                ReactionLog reactionLog = _HistorySubmit(sensorreactionLog, log, nHistoryID, zone);

                if (reactionLog != null)
                    m_reactionHistories.Add(reactionLog);
            }
            m_reactionHistories.Sort();
            return m_reactionHistories;
        }

        public void UpdateStatusForSensorReactionHistory(int nSensorReactionHistoryID, int nSensorReactionHistorySensorHistoryID, string strDetectionStatusName)
        {
            List<SensorReactionLog> sensorReactionLogs = null;

            if (m_dicHistorySensorReactionLog.TryGetValue(nSensorReactionHistorySensorHistoryID, out sensorReactionLogs))
            {
                SensorReactionLog.SignalResult nDetectionStatus = GetReverseDetectionStatus(strDetectionStatusName);

                foreach (SensorReactionLog reactionHistory in sensorReactionLogs)
                {
                    reactionHistory.DetectionResult = nDetectionStatus;

                    if (reactionHistory.ID == nSensorReactionHistoryID)
                    {
                        if (DBManager != null)
                        {
                            string strUpdateQuery = String.Format("UPDATE SensorReactionHistory SET DetectionStatus = {0} WHERE SensorHistoryID = {1}", (int)nDetectionStatus, reactionHistory.SensorZoneHistoryID);

                            ArrayList arrResult = DBManager.GetResultData(strUpdateQuery, 0);

                            if (arrResult == null)
                                return;
                        }
                    }
                }
            }
        }

        // nMinID와 nMaxID 사이에 있는 값중에 dicIDs에 포함되지 않는 리스트를 얻어온다.
        protected static string GetNotIncludeIDs<LogType>(WebDBManager dbMgr, string strTableName, int nMinID, int nMaxID, Dictionary<int, LogType> dicIDs)
        {
            string strSQL = "Select ID from " + strTableName + " where ID >= " + nMinID.ToString() + " and ID <= " + nMaxID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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

        protected static string MakeConditionWithNotIncludeIDs(string strFieldName, int nMinID, int nMaxID, string strNotIncludeIDs)
        {
            string strCondition = "";

            if (strNotIncludeIDs.Length > 0)
                strCondition = string.Format("{0} >= {1} and {0} <= {2} and {0} not in ({3})", strFieldName, nMinID, nMaxID, strNotIncludeIDs);
            else
                strCondition = string.Format("{0} >= {1} and {0} <= {2}", strFieldName, nMinID, nMaxID);

            return strCondition;
        }

        // 센서탐지 이외의 경로를 통해서 처리된 신호에 대한 Log를 가져온다.
        // arrZoneList : Zone List
        protected virtual List<SensorReactionLog> GetExternalReactionHistory(ArrayList arrZoneList, string startDate, string endDate)
        {
            return null;
        }

        // 특정 재난에 대한 센서(신호) 타입
        protected virtual string GetReactionString(int nType)
        {
            string strType = "";
            switch (nType)
            {
                case (int)DetectType.FIRE:
                    strType = "화재 센서";
                    //strType = "자탐 센서";
                    break;
                case (int)DetectType.COOLER:
                    strType = "소화 센서";
                    break;
                case (int)DetectType.PRESSURE:
                    strType = "압력 센서";
                    break;
                case (int)DetectType.MANUAL:
                    strType = "수동 신고";
                    break;
                case (int)DetectType.PSM:
                    strType = "누출 센서";
                    break;
                default:
                    break;
            }

            return strType;
        }

        protected virtual string GetDetectionStatusName(SensorReactionLog.SignalResult status)
        {
            switch (status)
            {
                case SensorReactionLog.SignalResult.REAL:
                    return "실제";
                case SensorReactionLog.SignalResult.USER_RESET:
                    return "오동작";
                case SensorReactionLog.SignalResult.TEST:
                default:
                    return "테스트";
            }
        }

        protected virtual SensorReactionLog.SignalResult GetReverseDetectionStatus(string strDetectionStatusName)
        {
            switch (strDetectionStatusName)
            {
                case "실제":
                    return SensorReactionLog.SignalResult.REAL;
                case "오동작":
                    return SensorReactionLog.SignalResult.USER_RESET;
                case "테스트":
                default:
                    return SensorReactionLog.SignalResult.TEST;
            }
        }

        // 각 재난별 발생할 수 있는 알람 Type들을 얻어온다.
        // strVariableName : DB Field 이름
        protected virtual string GetAlarmTypeQueryString(string strVariableName)
        {
            return strVariableName + " = " + ((int)UnE.Alarm.AlarmType.ALARM).ToString();
        }

        // SensorReactionHistory DB Table로부터 읽은 Parameter들을 사용하여 SensorReactionLog 객체의 값을 채운다.
        protected abstract void SetReactionLogParam(SensorReactionLog reactionLog, int nReactionType, int nParam1, string strParam2, string strParam3, string strParam4, string strParam5);
        // strStartDate와 strEndDate 사이에서 arrZoneList내에 존재하는 모든 재난(특정 재난)신호에 대한 통계자료를 작성한다.
        protected abstract List<Statistics> GetStatisticsLog(ArrayList arrZoneList, string strStartDate, string strEndDate);
        // arrAllLog 중에서 특정 재난탐지 신호만을 추려낸다.
        protected abstract List<DetectLog> GetDetectLog(List<SensorReactionLog> arrAllLog);
        // EquipmentZoneID로 SensorID를 찾아온다
        // 빠른 검색을 위하여 Dictionary 형태로 리턴한다.
        // Key : SensorZone ID
        protected abstract Dictionary<int, ISensor> FindSensorZone(List<EquipmentZone> arrEquipZoneList);
        protected abstract ReactionLog _GetReactionLog(SensorReactionLog sensorReactionLog, List<SensorReactionLog> sensorReactionLogList, int nSensorZoneHistoryID, Zone zone);
        protected abstract ReactionLog _HistorySubmit(SensorReactionLog sensorReactionLog, List<SensorReactionLog> sensorReactionLogList, int nSensorZoneHistoryID, Zone zone);
        protected abstract bool _GetReactionHistory(List<SensorReactionLog> arrReactionLog, string strSensorZoneHistoryIDs, Dictionary<int, SensorReactionLog> dicReactionLogs, ref int nMinReactionLogID, ref int nMaxReactionLogID);
    }

    public class SensorReactionLog
    {
        // 탐지된 신호의 처리결과
        public enum SignalResult
        {
            UNKNOWN = -1,
            REAL = 1,           // 실제 재난
            USER_RESET,         // 오동작 또는 신호복구
            TEST
        }

        private int nID = -1;
        private int nSensorZoneHistoryID = -1;
        private int nReactionType = -1;
        private DateTime time;
        private int param1 = -1;
        private string strMessage = "";
        private string param2 = "";
        private string param3 = "";
        private string param4 = "";
        private string param5 = "";
        private SignalResult nDetectionResult = SignalResult.UNKNOWN;
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

        // SensorReactionHistory ID
        public int ID
        {
            get { return nID; }
            set { nID = value; }
        }

        public int SensorZoneHistoryID
        {
            get { return nSensorZoneHistoryID; }
            set { nSensorZoneHistoryID = value; }
        }

        // libSensorProcess.ReactionType
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

        public SignalResult DetectionResult
        {
            get { return nDetectionResult; }
            set { nDetectionResult = value; }
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

        public void SetDetectionResult(int nResult)
        {
            if (nResult == (int)SignalResult.REAL)
                nDetectionResult = SignalResult.REAL;
            else if (nResult == (int)SignalResult.USER_RESET)
                nDetectionResult = SignalResult.USER_RESET;
            else if (nResult == (int)SignalResult.TEST)
                nDetectionResult = SignalResult.TEST;
            else
                nDetectionResult = SignalResult.UNKNOWN;
        }
    }

    public class DetectLog : IComparable
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

        public DetectLog()
        {
        }

        public int CompareTo(object b)
        {
            return _CompareTo(b);
        }

        protected virtual int _CompareTo(object b)
        {
            DetectLog data = this;
            DetectLog data2 = (DetectLog)b;

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

    public class Statistics
    {
        private List<int> nSensorZoneHistoryIDs = new List<int>();
        public List<int> SensorZoneHistoryIDList
        {
            get { return nSensorZoneHistoryIDs; }
            set { nSensorZoneHistoryIDs = value; }
        }
        
        // 신호타입
        // 센서의 종류 또는 탐지된 신호의 종류를 나타낸다.
        private string strDetectType = "";
        public string DetectType
        {
            get { return strDetectType; }
            set { strDetectType = value; }
        }

        // 탐지 횟수
        private int nDetectCount = 0;
        public int DetectCount
        {
            get { return nDetectCount; }
            set { nDetectCount = value; }
        }

        // 오작동 혹은 신호복구 횟수
        private int nUserResetCount = 0;
        public int UserResetCount
        {
            get { return nUserResetCount; }
            set { nUserResetCount = value; }
        }

        // 재난신고 횟수
        private int nReportCount = 0;
        public int ReportCount
        {
            get { return nReportCount; }
            set { nReportCount = value; }
        }

        // 처리되지 않은 신호의 횟수
        private int nIgnoreCount = 0;
        public int IgnoreCount
        {
            get { return nIgnoreCount; }
            set { nIgnoreCount = value; }
        }

        // 오작동(혹은 신호복구) 비율(%)
        private double percentUserReset = 0.0;
        public double PercentUserReset
        {
            get { return percentUserReset; }
            set { percentUserReset = value; }
        }

        // 현재 탐지상태인 신호의 개수
        private int nCurrentDetectCount = 0;
        public int CurrentDetectCount
        {
            get { return nCurrentDetectCount; }
            set { nCurrentDetectCount = value; }
        }

        // 건물그룹
        private string strBuildingGroupName = "";
        public string BuildingGroupName
        {
            get { return strBuildingGroupName; }
            set { strBuildingGroupName = value; }
        }

        // 건물이름
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

        // 담당자 이름
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

    public abstract class ReactionLog : IComparable
    {
        private int nHistoryID = -1;
        public int SensorZoneHistoryID
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

        // UnE.Sensor.IFacility.FacilityType
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

        // libSensorProcess.ReactionType
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

        private List<SensorReactionLog> arrLogList = new List<SensorReactionLog>();
        public List<SensorReactionLog> SensorReactionLogList
        {
            get { return arrLogList; }
            set { arrLogList = value; }
        }
        // 재난신고, 오작동 등 사용자 선택이 있을 경우 이 선택을 수행한 담당자 또는 수행 장소
        private string strUserName = "";
        public string UserName
        {
            get { return strUserName; }
            set { strUserName = value; }
        }

        // IFacility.FacilityType
        private string facilityType = "";
        public string FacilityType
        {
            get { return facilityType; }
            set { facilityType = value; }
        }

        public int CompareTo(object obj)
        {
            return _CompareTo(obj);
        }

        protected virtual int _CompareTo(object obj)
        {
            ReactionLog data = (ReactionLog)obj;

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

        public override string ToString()
        {
            return _ToString();
        }

        protected abstract string _ToString();
    }
}
