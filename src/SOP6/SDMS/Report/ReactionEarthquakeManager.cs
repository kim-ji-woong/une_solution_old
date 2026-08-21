using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnE.Sensor;
using libSensorProcess;
using DBUtility2;

namespace SDMS.Report
{
    public class ReactionEarthquakeManager
    {
        public class RefreshCheckData
        {
            private DateTime m_dtBefore = new DateTime();
            private DateTime m_dtCurrent = new DateTime();
            // 마지막으로 읽은 SensorReactionHistoryID
            private int m_nReadLastSensorReactionHistoryID = -1;
            private int m_nViewCount = 20;

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
        private ArrayList m_arrUserResetList = null;
        public ArrayList MulFunctionList
        {
            get { return m_arrUserResetList; }
            set { m_arrUserResetList = value; }
        }

        private ArrayList m_arrReactionHistory = new ArrayList();

        //HistoryID,ReactionLog
        private Dictionary<int, ArrayList> m_dicHistoryLog = new Dictionary<int, ArrayList>();
        //SensorZone ID,HistoryID List
        private Dictionary<int, ArrayList> m_dicSensorHistorys = new Dictionary<int, ArrayList>();
        //HistoryID, ReactionType
        private Dictionary<int, int> m_dicHistoryType = new Dictionary<int, int>();

        //UserResetEarthquakeLog, SensorReactionLogList
        private Dictionary<UserResetEarthquakeLog, ArrayList> m_dicUserResetLog = new Dictionary<UserResetEarthquakeLog, ArrayList>();
        internal Dictionary<UserResetEarthquakeLog, ArrayList> DicUserResetLog
        {
            get { return m_dicUserResetLog; }
            set { m_dicUserResetLog = value; }
        }

        private ArrayList arrAllReactionLog = new ArrayList();

        public ArrayList AllReactionLog
        {
            get { return arrAllReactionLog; }
        }

        private int m_nSiteID = 1;

        public ReactionEarthquakeManager()
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
            m_arrDectectList = new ArrayList();
        }

        public void DataClear()
        {
            if (m_arrDectectList != null)
                m_arrDectectList.Clear();
            if (m_arrUserResetList != null)
                m_arrUserResetList.Clear();
            if (m_dicHistoryLog != null)
                m_dicHistoryLog.Clear();
            if (m_dicSensorHistorys != null)
                m_dicSensorHistorys.Clear();
            if (m_dicHistoryType != null)
                m_dicHistoryType.Clear();
            if (arrAllReactionLog != null)
                arrAllReactionLog.Clear();
            if (m_dicUserResetLog != null)
                m_dicUserResetLog.Clear();
            if (m_arrReactionHistory != null)
                m_arrReactionHistory.Clear();
        }

        //탐지, 처리이력
        // Return 값 : true이면 데이터가 변경되었음.
        //             false이면 데이터 변경없음.
        public void Submit(DateTime startDate, DateTime endDate, int pageType = 1)//pageType이 1이면 탐지/처리 2이면 대응이력
        {
            string strNowDate, strBeforeDate;
            GetSumitDate(pageType, startDate, endDate, out strBeforeDate, out strNowDate);

            Dictionary<int, ISensor> dicSensorZones = GetEarthquakeSensorZone();
            //SensorID리스트로 SensorHistoryID를 찾아옴
            ArrayList arrZoneHistoryList = GetSensorZoneHistoryID(dicSensorZones, strBeforeDate, strNowDate);
            //ReactionLog를 가져옴
            ArrayList arrReactionList = GetReactionHistory(arrZoneHistoryList);

            arrAllReactionLog.Clear();
            if (arrReactionList != null)
                arrAllReactionLog.AddRange(arrReactionList);

            //오작동이력 로그 저장
            m_arrUserResetList = GetUserResetLog(strBeforeDate, strNowDate, dicSensorZones);

            m_arrDectectList = GetDetectLog(arrAllReactionLog);
            m_arrDectectList.Sort();
        }

        private Dictionary<int, ISensor> GetEarthquakeSensorZone()
        {
            Dictionary<int, ISensor> dicSensors = new Dictionary<int, ISensor>();
            List<ISensor> sensors = SensorManager.Instance.FindSensorZoneFromType(IFacility.FacilityType.Earthquake);

            if (sensors == null)
                return dicSensors;

            foreach (ISensor sensor in sensors)
            {
                dicSensors[sensor.ID] = sensor;
            }

            return dicSensors;
        }

        private void GetSumitDate(int pageType, DateTime startDate, DateTime endDate, out string strBeforeDate, out string strNowDate)
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
        public bool NeedRefresh(DateTime startDate, DateTime endDate, RefreshCheckData checkData, int pageType = 1)
        {
            string strBeforeDate, strNowDate;
            GetSumitDate(pageType, startDate, endDate, out strBeforeDate, out strNowDate);

            int nSensorHistoryID = GetMaxSensorReactionHistoryID();

            DateTime dtBefore, dtCurrent;

            if (!DateTime.TryParse(strBeforeDate, out dtBefore) || !DateTime.TryParse(strNowDate, out dtCurrent))
                return true;

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

        public ArrayList HistorySubmit(DateTime startDate, DateTime endDate)
        {
            m_arrReactionHistory.Clear();
            endDate = endDate.AddDays(1);

            foreach (KeyValuePair<int, ArrayList> pair in m_dicHistoryLog)
            {
                int nHistoryID = pair.Key;
                ArrayList log = pair.Value;
                int nReactionType = 0;

                ReactionEarthquakeLog reactionLog = new ReactionEarthquakeLog();
                reactionLog.HistoryID = nHistoryID;
                reactionLog.ArrLogList = log;

                // ReactionType가져옴
                if (m_dicHistoryType.ContainsKey(nHistoryID))
                {
                    nReactionType = m_dicHistoryType[nHistoryID];
                }
                
                //가장 맨 처음 발생한 ReactionLog를 Comobox로 보여줘야 하므로 log배열의 가장 첫번째 값을 가져온다
                SensorReactionLog sensorreactionLog = (SensorReactionLog)log[0];

                if (!(sensorreactionLog.Time >= startDate && sensorreactionLog.Time <= endDate))
                    continue;

                reactionLog.Time = sensorreactionLog.Time;
                reactionLog.SensorType = sensorreactionLog.SensorType;
                reactionLog.FacilityType = sensorreactionLog.Param3;
                reactionLog.Type = nReactionType;

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

                if (nSensorHistoryID == nHistoryID)
                {
                    foreach (SensorReactionLog srLog in log)
                    {
                        ReactionEarthquakeLog reactionLog = new ReactionEarthquakeLog();
                        reactionLog.HistoryID = nHistoryID;
                        reactionLog.ArrLogList = log;

                        reactionLog.Time = srLog.Time;
                        reactionLog.SensorType = srLog.SensorType;
                        reactionLog.Type = srLog.ReactionType;

                        arrReactLog.Add(reactionLog);
                    }
                    break;
                }
            }

            return arrReactLog;
        }

        private ArrayList GetDetectLog(ArrayList arrAllLog)
        {
            ArrayList arrDetectLog = new ArrayList();
            ArrayList arrComboBoxDate = new ArrayList();
            
            foreach (SensorReactionLog reactionLog in arrAllLog)
            {
                if (reactionLog.ReactionType == (int)ReactionType.BEGIN_STATUS || reactionLog.ReactionType == (int)ReactionType.NOTIFY_SIGNAL)
                {
                    DetectEarthquakeLog detect = new DetectEarthquakeLog();

                    detect.SensorReactionHistoryID = reactionLog.ID;
                    detect.HistoryID = reactionLog.SensorHistoryID;
                    detect.Time = reactionLog.Time;
                    detect.Memo = reactionLog.Memo;
                    detect.DetectType = GetDetectTypeString();
                    detect.DetectionStatusName = GetDetectionStatusName(reactionLog.DetectionStatus);
                    detect.SensorData = reactionLog.Param4;
                    detect.AlarmDepth = reactionLog.Param5;

                    arrDetectLog.Add(detect);
                }
            }

            return arrDetectLog;
        }

        private ArrayList GetUserResetLog(string strStartDate, string strEndDate, Dictionary<int, ISensor> dicSensorZones)
        {
            ArrayList arrUserReset = new ArrayList();
            List<int> liAddedReactionHistoryIDs = new List<int>();

            ArrayList arrHistoryList = FindHistoryID(dicSensorZones);
            if (arrHistoryList == null)
                return arrUserReset;

            if (arrHistoryList.Count == 0)
                return arrUserReset;

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
                return arrUserReset;

            // 사용자복구 이력 클래스 생성
            UserResetEarthquakeLog userReset = new UserResetEarthquakeLog();

            int nNotifyCount = 0;
            int nUserResetCount = 0;
            int nNotprocessCount = 0;
            int nOnlyDetectCount = 0;

            foreach (int nHistoryID in arrHistoryList)
            {
                ArrayList arrLog = new ArrayList();

                if (m_dicHistoryLog.ContainsKey(nHistoryID))
                    arrLog = m_dicHistoryLog[nHistoryID];

                int nType = 0;

                foreach (SensorReactionLog log in arrLog)
                {
                    if (log.ReactionType == (int)ReactionType.BEGIN_STATUS)
                    {
                        ArrayList arrSensorLog = null;

                        if (m_dicUserResetLog.ContainsKey(userReset))
                            arrSensorLog = m_dicUserResetLog[userReset];
                        else
                        {
                            arrSensorLog = new ArrayList();
                            m_dicUserResetLog[userReset] = arrSensorLog;
                        }
                        arrSensorLog.Add(log);
                    }

                    if (log.ReactionType == (int)ReactionType.NOTIFY_SIGNAL)
                    {
                        nNotifyCount++;
                        nType = log.ReactionType;

                        break;
                    }
                    else if (log.ReactionType == (int)ReactionType.USER_RESET)
                    {
                        nUserResetCount++;
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

            userReset.DetectCount = arrHistoryList.Count;

            //처리되지 않음
            nNotprocessCount = arrHistoryList.Count - (nNotifyCount + nUserResetCount) - nOnlyDetectCount;

            double PercentMulFunction = (nUserResetCount * 100) / arrHistoryList.Count;

            userReset.HistoryIDList = arrHistoryList;
            userReset.DetectType = GetDetectTypeString();

            userReset.NotifyCount = nNotifyCount;
            userReset.UserResetCount = nUserResetCount;
            userReset.Notprocess = nNotprocessCount;
            userReset.OnlyDetectCount = nOnlyDetectCount;

            arrUserReset.Add(userReset);

            // 신호복구 이력로그들을 배열에 저장
            return arrUserReset;
        }

        private string GetDetectTypeString()
        {
            return "지진탐지";
        }

        private void ReadReactionLogMemo(int nMinReactionLogID, int nMaxReactionLogID, Dictionary<int, SensorReactionLog> dicReactionLogs)
        {
            if (dicReactionLogs.Count == 0)
                return;

            string strNotIncludeIDs = GetNotIncludeIDs<SensorReactionLog>("SensorReactionHistory", nMinReactionLogID, nMaxReactionLogID, dicReactionLogs);
            string strCondition = MakeConditionWithNotIncludeIDs("SensorReactionHistoryID", nMinReactionLogID, nMaxReactionLogID, strNotIncludeIDs);

            string strSQL = "Select SensorReactionHistoryID, Description ";
            strSQL += "from SensorReactionHistoryDescription as memo, SensorReactionHistoryDescriptionText as memoText ";
            strSQL += "where memo.DescriptionID = memoText.ID and " + strCondition;

            WebDBManager webDB = FormMain.Instance.DBManager;
            ArrayList arrResult = webDB.GetResultData(strSQL);

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

                dicSensorZoneHistoryMemo[log.SensorHistoryID] = strMemo;
            }

            // Memo는 특정 SensorReactionHistory에 속해있는 것이지만 Report에서 사용할 때에는 어차피
            // SensorZoneHistory별로 정렬되기 때문에 같은 SensorZoneHistory ID를 가지는 모든 SensorReactionHistory에 Memo를 공유한다.
            foreach (KeyValuePair<int, SensorReactionLog> pair in dicReactionLogs)
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

            string strSQL = "select id, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5, DetectionStatus from SensorReactionHistory ";
            strSQL += "where SensorHistoryID in (" + strSensorList + ")";

            ArrayList arrResult = webDB.GetResultData(strSQL);
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
                reactionLog.SensorHistoryID = nSensorHistoryID;
                reactionLog.ReactionType = nReactionType;
                reactionLog.Time = time;
                reactionLog.DetectionStatus = nDetectionStatus;

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

        private string GetSensorZoneIDs(Dictionary<int, ISensor> dicSensorZones)
        {
            string strIDs = "";

            foreach (KeyValuePair<int, ISensor> pair in dicSensorZones)
            {
                if (strIDs.Length == 0)
                    strIDs = pair.Value.ID.ToString();
                else
                    strIDs += ", " + pair.Value.ID.ToString();
            }

            return strIDs;
        }

        //SensorID로 SensorHistoryID를 찾아옴
        // startDate와 endDate 사이의 모든 History들을 DB로부터 가져온다.
        // 가져온 DB 데이터들 가운데 dicSensorZoneIDs에 속하는 것들만 따로 추려낸다.
        private ArrayList GetSensorZoneHistoryID(Dictionary<int, ISensor> dicSensorZones, string startDate, string endDate)
        {
            ArrayList arrSensorZoneHistoryID = new ArrayList();

            WebDBManager webDB = FormMain.Instance.DBManager;

            string strSensorZoneIDs = GetSensorZoneIDs(dicSensorZones);

            if (strSensorZoneIDs.Length == 0)
                return null;

            string strSQL = "select id,SensorID from SensorZoneHistory where Time Between '" + startDate + "' and '" + endDate + "' and Data > 0 and SensorID in (" + strSensorZoneIDs + ")";

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

        private ArrayList FindHistoryID(Dictionary<int, ISensor> dicSensorZones)
        {
            ArrayList arrHistoryIDList = new ArrayList();
            
            if (dicSensorZones == null)
                return null;

            ArrayList histories = null;

            foreach (KeyValuePair<int, ISensor> pair in dicSensorZones)
            {
                int nSensorID = pair.Key;
                if (m_dicSensorHistorys.TryGetValue(nSensorID, out histories))
                    arrHistoryIDList.AddRange(histories);
            }
            
            return arrHistoryIDList;
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
            foreach (SensorReactionLog reactionHistory in m_dicHistoryLog[nSensorReactionHistorySensorHistoryID])
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

    class DetectEarthquakeLog : IComparable
    {
        private int nHistoryID = -1;
        private DateTime time;
        private string m_strSensorData = "";
        private string m_strAlarmDepth = "";
        private string strDetectType = "";
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

        public string SensorData
        {
            get { return m_strSensorData; }
            set { m_strSensorData = value; }
        }

        public string AlarmDepth
        {
            get { return m_strAlarmDepth; }
            set { m_strAlarmDepth = value; }
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

        public int CompareTo(object b)
        {
            DetectEarthquakeLog data = this;
            DetectEarthquakeLog data2 = (DetectEarthquakeLog)b;

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

    class UserResetEarthquakeLog
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
        private int nDetectCount = 0;

        public int DetectCount
        {
            get { return nDetectCount; }
            set { nDetectCount = value; }
        }

        private int nNotifyCount = 0;
        public int NotifyCount
        {
            get { return nNotifyCount; }
            set { nNotifyCount = value; }
        }

        //사용자복구 횟수
        private int nUserResetCount = 0;

        public int UserResetCount
        {
            get { return nUserResetCount; }
            set { nUserResetCount = value; }
        }

        //처리되지 않음
        private int nNotprocess = 0;

        public int Notprocess
        {
            get { return nNotprocess; }
            set { nNotprocess = value; }
        }

        //현재 탐지되어 잇는 상태의 신호
        private int nOnlyDetectCount = 0;

        public int OnlyDetectCount
        {
            get { return nOnlyDetectCount; }
            set { nOnlyDetectCount = value; }
        }
    }

    class ReactionEarthquakeLog : IComparable
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
        
        private int nReactionType = -1;

        public int Type
        {
            get { return nReactionType; }
            set { nReactionType = value; }
        }
        
        private ArrayList arrLogList = new ArrayList();
        public ArrayList ArrLogList
        {
            get { return arrLogList; }
            set { arrLogList = value; }
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
            if (nReactionType == 22)
                strReactionType = "지진 발생";            
            else if (nReactionType == 23)
                strReactionType = "방범탐지 후 상황해제";
            else if (nReactionType == 0)
                strReactionType = "지진 탐지";
            
            if (nSensorType == 0)
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
            ReactionEarthquakeLog data = (ReactionEarthquakeLog)obj;

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
