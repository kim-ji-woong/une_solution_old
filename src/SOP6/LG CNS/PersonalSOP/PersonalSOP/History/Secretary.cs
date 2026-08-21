using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBUtility2;
using System.Collections;

namespace PersonalSOP.History
{
    public class Secretary
    {
        public enum ComponentState
        {
            //대기상태(1), 실행중(2), 완료(3), 입력대기상태(4), 건너뜀 상태(5)
            STANDBY = 1,
            RUN = 2,
            DONE = 3,
            WAIT = 4,
            IGNORE = 5
        }

        private static int m_nLastReadComponentHistoryID = -1;
        private static int m_nLastReadActionStepHistoryID = -1;
        // 시작된지 m_nDayLimit일이 지난 ActionStepHistory는 무시한다.
        private static int m_nDayLimit = 30;
        // Key : 평일, 낮이면 양수
        //       휴일 또는 야간이면 음수
        private static Dictionary<int, string> m_dicSOPGenUserCommanderNames = new Dictionary<int, string>();
        private static int m_nWorkingBeginHour = 9, m_nWorkingBeginMinute = 0;
        private static int m_nWorkingEndHour = 18, m_nWorkingEndMinute = 0;

        private static string m_strCurrentCommanderName = "";

        public static string CurrentCommanderName
        {
            get { return m_strCurrentCommanderName; }
        }

        public static void FindNewActionStepHistory(WebDBManager dbMgr, SOPHistoryManager historyManager)
        {
            string strSQL = "Select ash.ID, ash.ActionStepID, ash.RealMode, ash.BeginTime, ash.DetectTime, ash.Position, ash.LastAccessedUserID, _as.StepName, d.DisasterName, sdc.SubCategoryName, dc.CategoryName";
            strSQL += " from ActionStepHistory as ash, ActionStep as _as, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc";
            strSQL += " where ash.ActionStepID = _as.ID and _as.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and ash.EndTime is NULL and ash.CancelTime is NULL and ash.ID > " + m_nLastReadActionStepHistoryID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            IOManager ioMgr = new IOManager();
            string strCommanderName;
            bool isDayLight = IsDayLight(DateTime.Now);

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> nRealMode = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> dtBeginTime = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                VariousData<DateTime> dtDetectTime = WebDBManager.GetDateTimeField(arrResult[i + 4]);
                string strPosition = WebDBManager.GetStringField(arrResult[i + 5]);
                VariousData<int> nAccessedUserID = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                string strActionStepName = WebDBManager.GetStringField(arrResult[i + 7]);
                string strDisasterName = WebDBManager.GetStringField(arrResult[i + 8]);
                string strSubCategoryName = WebDBManager.GetStringField(arrResult[i + 9]);
                string strCategoryName = WebDBManager.GetStringField(arrResult[i + 10]);

                bool isRealMode = false;

                if (nRealMode != null && nRealMode.Data == 1)
                    isRealMode = true;

                if (nID == null || nActionStepID == null || dtBeginTime == null || nAccessedUserID == null ||
                    strActionStepName == null || strDisasterName == null || strSubCategoryName == null || strCategoryName == null)
                    continue;

                if (strPosition == null)
                    strPosition = "";

                TimeSpan span = DateTime.Now - dtBeginTime.Data;

                // 시작된지 m_nDayLimit일이 지난 ActionStepHistory는 무시한다.
                if (span.TotalDays >= m_nDayLimit)
                    continue;

                ActionStepHistory actionStepHistory = AddActionStepSections(dbMgr, ioMgr, nActionStepID.Data);
                
                if (actionStepHistory.GetSectionStateCount() == 0)
                    continue;

                actionStepHistory.CalcProcessPercentage();

                actionStepHistory.ActionStepPath = strCategoryName + "/" + strSubCategoryName + "/" + strDisasterName + "/" + strActionStepName;
                actionStepHistory.ActionStepID = nActionStepID.Data;
                actionStepHistory.ActionStepHistoryID = nID.Data;
                actionStepHistory.RealMode = isRealMode;
                actionStepHistory.Position = strPosition;
                actionStepHistory.BeginTime = new TimeInfo(dtBeginTime.Data);
                actionStepHistory.DetectTime = new TimeInfo(dtDetectTime == null ? DateTime.Now : dtDetectTime.Data);

                if (!isDayLight)
                    nAccessedUserID.Data = -nAccessedUserID.Data;

                if (m_dicSOPGenUserCommanderNames.TryGetValue(nAccessedUserID.Data, out strCommanderName))
                    actionStepHistory.CommanderName = strCommanderName;

                historyManager.AddActionStepHistory(actionStepHistory);

                if (nID.Data > m_nLastReadActionStepHistoryID)
                    m_nLastReadActionStepHistoryID = nID.Data;
            }

            if (m_nLastReadActionStepHistoryID == -1)
            {
                FindLastActionStepHistory(dbMgr, historyManager);
            }

        }

        // 현재 진행중인 최신 SOP가 없으면 마지막으로 진행했었던 SOP를 로드함.
        private static void FindLastActionStepHistory(WebDBManager dbMgr, SOPHistoryManager historyManager)
        {
            Dictionary<int, Data_SOPGenUser> dicGenUsers = new Dictionary<int, Data_SOPGenUser>();
            Dictionary<int, Data_SOPGenUser> dicGenUsers2 = new Dictionary<int, Data_SOPGenUser>();

            string strSQL = "Select ID, MemberID, UserID, NickName from SOPGenUser where SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;


            int nResultCount = arrResult.Count;
            string strIDs = "";

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strUserID = WebDBManager.GetStringField(arrResult[i + 2]);
                string strNickName = WebDBManager.GetStringField(arrResult[i + 3]);

                if (nID == null || strUserID == null)
                    continue;

                Data_SOPGenUser user = new Data_SOPGenUser();

                user.ID = nID.Data;
                user.UserID = strUserID;
                user.NickName = strNickName;

                if (strNickName == null && nMemberID != null)
                {
                    if (strIDs.Length == 0)
                        strIDs = nMemberID.Data.ToString();
                    else
                        strIDs += ", " + nMemberID.Data.ToString();

                    user.MemberID = nMemberID.Data;
                    dicGenUsers2[user.MemberID] = user;
                }

                dicGenUsers[user.ID] = user;
            }

            if (strIDs.Length != 0)
            {
                strSQL = "Select ID, MemberName from CompanyMember where ID in (" + strIDs + ")";
                arrResult = dbMgr.GetResultData(strSQL);

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strMemberName = WebDBManager.GetStringField(arrResult[i + 1]);

                    if (nID == null || strMemberName == null)
                        continue;

                    Data_SOPGenUser user;

                    if (dicGenUsers2.TryGetValue(nID.Data, out user))
                    {
                        user.UserName = strMemberName;
                    }
                }

            }
            dicGenUsers2.Clear();

            if (dicGenUsers == null)
                return;

            strSQL = "Select ash.ID, ash.ActionStepID, ash.RealMode, ash.BeginTime, ash.DetectTime, ash.Position, ash.LastAccessedUserID, v.isNormal, ash.EndTime, ash.CancelTime, dc.CategoryName, sdc.SubCategoryName, d.DisasterName, _as.StepName ";
            strSQL += "from ActionStepHistory as ash, ActionStep as _as, Disaster as d, Version as v, DisasterCategory as dc, SubDisasterCategory as sdc ";
            strSQL += "where ash.ActionStepID = _as.ID and _as.DisasterID = d.ID and d.VersionID = v.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and (ash.EndTime is not NULL or ash.CancelTime is not NULL) order by ash.BeginTime desc";


            arrResult = dbMgr.GetResultData(strSQL, 1);

            if (arrResult == null)
                return;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 13; i += 14)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> nRealMode = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> beginTime = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                VariousData<DateTime> detectTime = WebDBManager.GetDateTimeField(arrResult[i + 4]);
                string strPosition = WebDBManager.GetStringField(arrResult[i + 5]);
                VariousData<int> nAccessedUserID = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<int> nNormal = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                VariousData<DateTime> endTime = WebDBManager.GetDateTimeField(arrResult[i + 8]);
                VariousData<DateTime> cancelTime = WebDBManager.GetDateTimeField(arrResult[i + 9]);
                string strCategoryName = WebDBManager.GetStringField(arrResult[i + 10]);
                string strSubCategoryName = WebDBManager.GetStringField(arrResult[i + 11]);
                string strDisasterName = WebDBManager.GetStringField(arrResult[i + 12]);
                string strActionStepName = WebDBManager.GetStringField(arrResult[i + 13]);

                if (nID == null || nActionStepID == null || nRealMode == null || beginTime == null || nNormal == null || nAccessedUserID == null ||
                    strCategoryName == null || strSubCategoryName == null || strDisasterName == null || strActionStepName == null)
                    continue;

                if (endTime == null && cancelTime == null)
                    continue;

                Data_SOPGenUser user;

                if (!dicGenUsers.TryGetValue(nAccessedUserID.Data, out user))
                    continue;

                string strCommanderName = "";

                if (user.NickName != null && user.NickName != "")
                    strCommanderName = user.NickName;
                else if (user.UserName != null && user.UserName != "")
                    strCommanderName = user.UserName;

                if (strPosition == null)
                    strPosition = "";

                ActionStepHistory actionStepHistory = new ActionStepHistory();

                actionStepHistory.ActionStepHistoryID = nID.Data;
                actionStepHistory.ActionStepID = nActionStepID.Data;
                actionStepHistory.ActionStepPath = strCategoryName + "/" + strSubCategoryName + "/" + strDisasterName + "/" + strActionStepName;
                actionStepHistory.BeginTime = new TimeInfo(beginTime.Data);

                if (endTime != null)
                    actionStepHistory.EndTime = new TimeInfo(endTime.Data);
                else
                    actionStepHistory.CancelTime = new TimeInfo(cancelTime.Data);

                actionStepHistory.DetectTime = detectTime == null ? null : new TimeInfo(detectTime.Data);
                actionStepHistory.CommanderName = strCommanderName;
                actionStepHistory.Position = strPosition;
                actionStepHistory.RealMode = nRealMode.Data == 1;
                actionStepHistory.IsNormal = nNormal.Data == 1;

                historyManager.LoadActionStepHistory(actionStepHistory);

                if (actionStepHistory.ActionStepHistoryID > m_nLastReadActionStepHistoryID)
                    m_nLastReadActionStepHistoryID = actionStepHistory.ActionStepHistoryID;
            }
        }

        public static ActionStepHistory AddActionStepSections(WebDBManager dbMgr, IOManager ioMgr, int nActionStepID, ActionStepHistory actionStepHistory = null)
        {
            List<int> stepMemberIDs = GetStepMemberIDs(dbMgr, nActionStepID);

            if (stepMemberIDs == null)
                return null;

            if (actionStepHistory == null)
                actionStepHistory = new ActionStepHistory();

            foreach (int nStepMemberID in stepMemberIDs)
            {
                Dictionary<int, Sections.Section> dicSections = new Dictionary<int, Sections.Section>();

                if (!ioMgr.LoadSections(dbMgr, nStepMemberID, dicSections, nActionStepID))
                    continue;

                foreach (KeyValuePair<int, Sections.Section> pair in dicSections)
                {
                    int nComponentType = pair.Key >> 24;
                    int nComponentID = pair.Key & 0x00ffffff;

                    actionStepHistory.AddSectionState(nComponentID, nComponentType, pair.Value);
                }
            }

            return actionStepHistory;
        }

        private static List<int> GetStepMemberIDs(WebDBManager dbMgr, int nActionStepID)
        {
            string strSQL = "Select ID from StepMember where ActionStepID = " + nActionStepID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            List<int> stepMemberIDs = new List<int>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());

                if (nID != null)
                    stepMemberIDs.Add(nID.Data);
            }

            return stepMemberIDs;
        }

        public static void LoadCurrentActionStepHistoryList(WebDBManager dbMgr, SOPHistoryManager historyManager)
        {
            List<ActionStepHistory> completeActionStepHistories = new List<ActionStepHistory>();
            List<ActionStepHistory> cancelActionStepHistories = new List<ActionStepHistory>();

            LoadWorkingTime(dbMgr);

            List<ActionStepHistory> arrActionStepHistories = historyManager.ActionStepHistories;

            string strActionStepHistoryIDs = "";
            Dictionary<int, ActionStepHistory> dicActionStepHistories = new Dictionary<int, ActionStepHistory>();

            foreach (ActionStepHistory actionStepHistory in arrActionStepHistories)
            {
                if (strActionStepHistoryIDs.Length == 0)
                    strActionStepHistoryIDs = actionStepHistory.ActionStepHistoryID.ToString();
                else
                    strActionStepHistoryIDs += ", " + actionStepHistory.ActionStepHistoryID.ToString();

                dicActionStepHistories[actionStepHistory.ActionStepHistoryID] = actionStepHistory;
            }

            LoadCommander(dbMgr, strActionStepHistoryIDs, dicActionStepHistories);
            LoadComponentHistory(historyManager, dbMgr, strActionStepHistoryIDs, dicActionStepHistories);

            CheckCompleteOrCancel(dbMgr, strActionStepHistoryIDs, dicActionStepHistories, completeActionStepHistories, cancelActionStepHistories);
            dicActionStepHistories.Clear();

            int newActionStepHistoryCount = arrActionStepHistories.Count - completeActionStepHistories.Count - cancelActionStepHistories.Count;

            foreach (ActionStepHistory actionStepHistory in completeActionStepHistories)
            {
                historyManager.CompleteActionStepHistory(actionStepHistory);

                if (newActionStepHistoryCount > 0)
                    historyManager.RemoveActionStepHistory(actionStepHistory);
            }

            foreach (ActionStepHistory actionStepHistory in cancelActionStepHistories)
            {
                historyManager.CancelActionStepHistory(actionStepHistory);

                if (newActionStepHistoryCount > 0)
                    historyManager.RemoveActionStepHistory(actionStepHistory);
            }

            historyManager.UpdateData();
        }

        private static void LoadWorkingTime(WebDBManager dbMgr)
        {
            string strBeginTimeTag = "WorkingBeginHour";
            string strEndTimeTag = "WorkingEndHour";

            string strSQL = string.Format("Select PropertyName, PropertyValue from OptionSOPSimulator where (PropertyName = '{0}' or PropertyName = '{1}') and SiteID = {2}",
                strBeginTimeTag, strEndTimeTag, dbMgr.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nHour = 0, nMinute = 0;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyValue == null || strPropertyName == null)
                    continue;

                if (!ParseHourNMinute(strPropertyValue, ref nHour, ref nMinute))
                    continue;

                if (strPropertyName == strBeginTimeTag)
                {
                    m_nWorkingBeginHour = nHour;
                    m_nWorkingBeginMinute = nMinute;
                }
                else if (strPropertyName == strEndTimeTag)
                {
                    m_nWorkingEndHour = nHour;
                    m_nWorkingEndMinute = nMinute;
                }
            }
        }

        private static bool ParseHourNMinute(string strValue, ref int nHour, ref int nMinute)
        {
            string[] arrTokens = strValue.Split(':');

            if (arrTokens.Count() != 2)
                return false;

            if (!int.TryParse(arrTokens[0].Trim(), out nHour))
                return false;

            if (!int.TryParse(arrTokens[1].Trim(), out nMinute))
                return false;

            return true;
        }

        private static bool IsDayLight(DateTime time)
        {
            if (time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday)
                return false;

            if (time.Hour > m_nWorkingBeginHour && time.Hour < m_nWorkingEndHour)
                return true;
            else if (time.Hour == m_nWorkingBeginHour && time.Minute >= m_nWorkingBeginMinute)
                return true;
            else if (time.Hour == m_nWorkingEndHour && time.Minute <= m_nWorkingEndMinute)
                return true;

            return false;
        }

        // Return 값 : SOPGenUserID별 진행총괄자 이름
        //             Key : 평일, 낮이면 양수
        //                   휴일 또는 야간이면 음수
        private static void LoadCommander(WebDBManager dbMgr, string strActionStepHistoryIDs, Dictionary<int, ActionStepHistory> dicActionStepHistories)
        {
            string strSQL = "Select ID, NickName from SOPGenUser where SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            string strGenUserIDs = "";
            Dictionary<int, string> dicCommanders = m_dicSOPGenUserCommanderNames;
            List<int> ids = new List<int>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strNickName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (nID == null)
                    continue;

                if (strNickName == null)
                    strNickName = "";

                dicCommanders[nID.Data] = strNickName;
                dicCommanders[-nID.Data] = strNickName;
                ids.Add(nID.Data);

                if (strGenUserIDs.Length == 0)
                    strGenUserIDs = nID.Data.ToString();
                else
                    strGenUserIDs += ", " + nID.Data.ToString();
            }

            if (strGenUserIDs.Length == 0)
                return;

            strSQL = "Select SOPGenUserID, DayLight, DisplayText from SOPGenUserCommander where SOPGenUserID in (" + strGenUserIDs + ")";
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            nResultCount = arrResult.Count;

            Dictionary<int, string> dicDayLight = new Dictionary<int, string>();
            Dictionary<int, string> dicNight = new Dictionary<int, string>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nDayLight = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 2]);

                if (nDayLight == null || nID == null)
                    continue;

                bool dayLight = nDayLight.Data == 1;

                if (strDisplayText == null)
                    strDisplayText = "";

                if (dayLight)
                    dicDayLight[nID.Data] = strDisplayText;
                else
                    dicNight[nID.Data] = strDisplayText;
            }

            string strDayLightCommander, strNightCommander;

            foreach (int nSOPGenUserID in ids)
            {
                bool dayLight = dicDayLight.TryGetValue(nSOPGenUserID, out strDayLightCommander);
                bool night = dicNight.TryGetValue(nSOPGenUserID, out strNightCommander);

                if (dayLight && night)
                {
                    dicCommanders[nSOPGenUserID] = strDayLightCommander;
                    dicCommanders[-nSOPGenUserID] = strNightCommander;
                }
                else if (dayLight)
                {
                    dicCommanders[nSOPGenUserID] = strDayLightCommander;
                    dicCommanders[-nSOPGenUserID] = strDayLightCommander;
                }
                else if (night)
                {
                    dicCommanders[nSOPGenUserID] = strNightCommander;
                    dicCommanders[-nSOPGenUserID] = strNightCommander;
                }
            }

            ids.Clear();
            dicDayLight.Clear();
            dicNight.Clear();

            if (strActionStepHistoryIDs.Length == 0)
                return;

            strSQL = "Select ID, LastAccessedUserID from ActionStepHistory where ID in (" + strActionStepHistoryIDs + ")";
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            nResultCount = arrResult.Count;
            bool isDayLight = IsDayLight(DateTime.Now);

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nAccessedUserID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (nID == null || nAccessedUserID == null)
                    continue;

                ActionStepHistory actionStepHistory;
                string strCommanderName;

                if (!dicActionStepHistories.TryGetValue(nID.Data, out actionStepHistory))
                    continue;

                if (!isDayLight)
                    nAccessedUserID.Data = -nAccessedUserID.Data;

                if (dicCommanders.TryGetValue(nAccessedUserID.Data, out strCommanderName))
                {
                    actionStepHistory.CommanderName = strCommanderName;
                }
            }

            strSQL = "Select UserID from ControlUser where SiteID = " + dbMgr.SiteID.ToString();
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
            {
                m_strCurrentCommanderName = "";
            }
            else
            {
                VariousData<int> nUserID = WebDBManager.GetIntField(arrResult[0].ToString());

                if (nUserID == null)
                    m_strCurrentCommanderName = "";
                else
                {
                    int nID = isDayLight ? nUserID.Data : -nUserID.Data;

                    string strCommanderName;

                    if (dicCommanders.TryGetValue(nID, out strCommanderName))
                        m_strCurrentCommanderName = strCommanderName;
                    else if (dicCommanders.TryGetValue(-nID, out strCommanderName))
                        m_strCurrentCommanderName = strCommanderName;
                    else
                        m_strCurrentCommanderName = "";
                }
            }
        }

        private static void CheckCompleteOrCancel(WebDBManager dbMgr, string strActionStepHistoryIDs, Dictionary<int, ActionStepHistory> dicActionStepHistories, List<ActionStepHistory> completeActionStepHistories, List<ActionStepHistory> cancelActionStepHistories)
        {
            if (strActionStepHistoryIDs.Length == 0)
                return;

            string strSQL = "Select ID, EndTime, CancelTime from ActionStepHistory where (EndTime is not NULL or CancelTime is not NULL) and ID in (" + strActionStepHistoryIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            ActionStepHistory actionStepHistory;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> nActionStepHistoryID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> timeEnd = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                VariousData<DateTime> timeCancel = WebDBManager.GetDateTimeField(arrResult[i + 2]);

                if (nActionStepHistoryID == null)
                    continue;

                if (!dicActionStepHistories.TryGetValue(nActionStepHistoryID.Data, out actionStepHistory))
                    continue;

                if (timeEnd != null)
                {
                    actionStepHistory.EndTime = new TimeInfo(timeEnd.Data);
                    completeActionStepHistories.Add(actionStepHistory);
                }
                else if (timeCancel != null)
                {
                    actionStepHistory.CancelTime = new TimeInfo(timeCancel.Data);
                    cancelActionStepHistories.Add(actionStepHistory);
                }

                if (m_nLastReadActionStepHistoryID < nActionStepHistoryID.Data)
                    m_nLastReadActionStepHistoryID = nActionStepHistoryID.Data;
            }
        }

        public static void LoadComponentHistory(SOPHistoryManager historyManager, WebDBManager dbMgr, string strActionStepHistoryIDs, Dictionary<int, ActionStepHistory> dicActionStepHistories, bool ignoreLastLog = false)
        {
            if (strActionStepHistoryIDs.Length == 0)
                return;

            string strFormat = "Select ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, CompleteCount";
            strFormat += ", ShowBoard, AccessedUserID from ComponentHistory where ActionStepHistoryID in ({0}) and ID > {1}";

            int nLastReadComponentHistoryID = ignoreLastLog ? -1 : m_nLastReadComponentHistoryID;

            string strSQL = string.Format(strFormat, strActionStepHistoryIDs, nLastReadComponentHistoryID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            string strComponentHistoryIDs = "";
            Dictionary<int, ComponentHistory> dicComponentHistories = new Dictionary<int, ComponentHistory>();
            List<ComponentHistory> componentHistories = new List<ComponentHistory>();

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nActionStepHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> nComponentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> nComponentType = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 4]);
                VariousData<int> nStatus = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strTask = WebDBManager.GetStringField(arrResult[i + 6].ToString());
                VariousData<int> nCompleteCount = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                VariousData<int> nShowBoard = WebDBManager.GetIntField(arrResult[i + 8].ToString());
                VariousData<int> nSOPGenUserID = WebDBManager.GetIntField(arrResult[i + 9].ToString());

                if (nID == null || nActionStepHistoryID == null || nComponentID == null || nComponentType == null ||
                    time == null || nStatus == null || nSOPGenUserID == null)
                    continue;

                if (strTask == null)
                    strTask = "";

                if (nCompleteCount == null)
                    nCompleteCount = new VariousData<int>(0);

                //bool showBoard = true;

                //if (nShowBoard != null && nShowBoard.Data == 0)
                //    showBoard = false;

                ActionStepHistory actionStepHistory;

                if (!dicActionStepHistories.TryGetValue(nActionStepHistoryID.Data, out actionStepHistory))
                    continue;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nSectionKey = (nComponentType.Data << 24) | nComponentID.Data;

                SectionState sectionState = actionStepHistory.GetSectionState(nComponentID.Data, nComponentType.Data);

                if (sectionState == null)
                    continue;

                int nDirection = nStatus.Data >> 16;
                int nStatus2 = nStatus.Data & 0x0000ffff;

                sectionState.SetState(nStatus2);

                string strStatus = "";
                if (!GetStatusString(sectionState.GetState(), ref strStatus))
                    continue;

                ComponentHistory.HistoryType historyType = ToHistoryType(nStatus2);

                if (historyType == ComponentHistory.HistoryType.NONE)
                    continue;

                ComponentHistory componentHistory = new ComponentHistory();

                componentHistory.ActionStepHistory = actionStepHistory;
                componentHistory.ComponentHistoryID = nID.Data;
                componentHistory.Time = time.Data;
                componentHistory.SectionState = sectionState;
                componentHistory.AccessedUserID = nSOPGenUserID.Data;
                componentHistory.Task = GetDetailTask(sectionState.Section.Data);
                componentHistory.Type = historyType;
                componentHistory.Commander = "";//GetUserCommanderName(nSOPGenUserID.Data);
                //componentHistory.Commander = GetCommanderName(sectionState.Section);
                componentHistory.Receiver = GetReceiverName(sectionState.Section);

                SetEndpointTime(componentHistory);

                if (sectionState.Section is Sections.SectionDecision && strTask.Length > 0)
                    componentHistory.Task += " => " + strTask;

                dicComponentHistories[nID.Data] = componentHistory;
                componentHistories.Add(componentHistory);

                //realTime.AddComponentHistory(actionStepHistory, componentHistory);
                m_nLastReadComponentHistoryID = nID.Data;

                if (strComponentHistoryIDs.Length == 0)
                    strComponentHistoryIDs = nID.Data.ToString();
                else
                    strComponentHistoryIDs += ", " + nID.Data.ToString();
            }

            // Value : Detail History
            Dictionary<ComponentHistory, List<ComponentHistory>> dicComponentHistoryDetails = LoadComponentHistoryDetail(dbMgr, strComponentHistoryIDs, dicComponentHistories);
            //List<ComponentHistory> componentHistories = LoadComponentHistoryDetail(dbMgr, strComponentHistoryIDs, dicComponentHistories);

            List<ComponentHistory> details = null;

            if (componentHistories != null)
            {
                ComponentHistory beginHistory = GetBeginHistory(componentHistories);

                if (beginHistory != null)
                {
                    // 시작 컴포넌트의 History를 제일 위쪽에 둔다.
                    componentHistories.Remove(beginHistory);
                    componentHistories.Insert(0, beginHistory);
                }

                foreach (ComponentHistory history in componentHistories)
                {
                    historyManager.AddComponentHistory(history.ActionStepHistory, history);

                    if (dicComponentHistoryDetails.TryGetValue(history, out details))
                    {
                        foreach (ComponentHistory detail in details)
                        {
                            historyManager.AddComponentHistory(history.ActionStepHistory, detail);
                        }
                    }
                }
            }
        }

        private static void SetEndpointTime(ComponentHistory componentHistory)
        {
            if (componentHistory.SectionState.Section.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)
            {
                if (((Sections.SectionDataEndPoint)componentHistory.SectionState.Section.Data).IsBegin)
                {
                    componentHistory.Time = componentHistory.ActionStepHistory.BeginTime.m_time;
                }
                else
                {
                    if (componentHistory.ActionStepHistory.EndTime != null)
                        componentHistory.Time = componentHistory.ActionStepHistory.EndTime.m_time;
                    else if (componentHistory.ActionStepHistory.CancelTime != null)
                        componentHistory.Time = componentHistory.ActionStepHistory.CancelTime.m_time;
                }
            }
        }

        private static ComponentHistory GetBeginHistory(List<ComponentHistory> histories)
        {
            foreach (ComponentHistory history in histories)
            {
                if (history.SectionState.Section is Sections.SectionEndPoint)
                {
                    Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)history.SectionState.Section.Data;

                    if (data.IsBegin)
                        return history;
                }
            }

            return null;
        }

        private static string GetReceiverName(Sections.Section section)
        {
            if (section == null)
                return "";

            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
            {
                return ((Sections.SectionProcess)section).TextDown;
            }
            else if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
                return data.TeamList.Count == 0 ? "" : data.TeamList[data.TeamList.Count - 1].ToString();
            }

            return "";
        }

        private static ComponentHistory FindComponentHistory(ActionStepHistory actionStepHistory, SectionState sectionState)
        {
            foreach (ComponentHistory componentHistory in actionStepHistory.ComponentHistories)
            {
                if (componentHistory.SectionState == sectionState)
                    return componentHistory;
            }

            return null;
        }

        private static ComponentHistory.HistoryType ToHistoryType(int nComponentState)
        {
            if (nComponentState == (int)ComponentState.WAIT || nComponentState == (int)ComponentState.STANDBY)
                return ComponentHistory.HistoryType.WAIT;
            else if (nComponentState == (int)ComponentState.RUN)
                return ComponentHistory.HistoryType.CONFIRM_MISSION;
            else if (nComponentState == (int)ComponentState.DONE)
                return ComponentHistory.HistoryType.COMPLETE_MISSION;

            return ComponentHistory.HistoryType.NONE;
        }

        private static Dictionary<ComponentHistory, List<ComponentHistory>> LoadComponentHistoryDetail(WebDBManager dbMgr, string strComponentHistoryIDs, Dictionary<int, ComponentHistory> dicComponentHistories)
        {
            if (strComponentHistoryIDs.Length == 0)
                return null;

            string strSQL = "Select ComponentHistoryID, DataIndex, Datai, Dataf, Datas, Time from ComponentHistoryDetail where ComponentHistoryID in (" + strComponentHistoryIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            Dictionary<ComponentHistory, List<ComponentHistory>> dicComponentHistoryDetails = new Dictionary<ComponentHistory, List<ComponentHistory>>();
            List<ComponentHistory> componentHistories = null;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> nComponentHistoryID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nDataIndex = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> nData = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<float> fData = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                string strData = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 5]);

                if (nComponentHistoryID == null || nDataIndex == null)
                    continue;

                DateTime historyTime = time == null ? DateTime.Now : time.Data;

                ComponentHistory componentHistory;

                if (!dicComponentHistories.TryGetValue(nComponentHistoryID.Data, out componentHistory))
                    continue;

                if (componentHistory == null)
                    continue;

                if (componentHistory.SectionState == null || componentHistory.SectionState.Section == null)
                    continue;

                if (dicComponentHistoryDetails.TryGetValue(componentHistory, out componentHistories) == false)
                {
                    componentHistories = new List<ComponentHistory>();
                    dicComponentHistoryDetails[componentHistory] = componentHistories;
                }

                Sections.Section section = componentHistory.SectionState.Section;
                Sections.Section.ComponentType sectionType = section.GetComponentType();

                bool addDetail = false;

                if (nDataIndex.Data >= 0 && sectionType == Sections.Section.ComponentType.PROCESS)
                    addDetail = AddProcessDetail(componentHistories, nDataIndex.Data, nData, fData, strData, historyTime, componentHistory);
                else if (nDataIndex.Data < 0 && sectionType == Sections.Section.ComponentType.INTERNAL)
                    addDetail = AddInternalDetail(componentHistories, nDataIndex.Data, nData, fData, strData, historyTime, componentHistory);

                if (addDetail)
                {
                    // Section이 Normal이나 Input 상태인데, 세부임무가 수행될 경우 "임무확인"으로 바꿔준다.
                    if (componentHistory.Type == ComponentHistory.HistoryType.WAIT)
                        componentHistory.Type = ComponentHistory.HistoryType.CONFIRM_MISSION;
                }
            }

            return dicComponentHistoryDetails;
            /*foreach (KeyValuePair<int, ComponentHistory> pair in dicComponentHistories)
            {
                componentHistories.Add(pair.Value);
            }

            componentHistories.Sort();

            return componentHistories;*/
        }

        private static bool AddInternalDetail(List<ComponentHistory> componentHistories, int nDataIndex, VariousData<int> nData, VariousData<float> fData, string strData, DateTime time, ComponentHistory parentHistory)
        {
            ComponentHistory.HistoryType historyType = ComponentHistory.GetComponentHistoryType(nDataIndex, nData, fData, strData);

            // 문자전송이나 방송실행, 완료체크를 제외하곤 모두 무시한다.
            if (historyType != ComponentHistory.HistoryType.SEND_BROADCAST &&
                historyType != ComponentHistory.HistoryType.SEND_SMS &&
                historyType != ComponentHistory.HistoryType.CHECK_MISSION)
                return false;

            Sections.Section section = parentHistory.SectionState.Section;
            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

            ComponentHistory history = new ComponentHistory();

            history.ActionStepHistory = parentHistory.ActionStepHistory;
            history.ComponentHistoryID = parentHistory.ComponentHistoryID;
            history.AccessedUserID = parentHistory.AccessedUserID;
            history.Commander = parentHistory.Commander;
            history.Receiver = parentHistory.Receiver;
            history.SectionState = parentHistory.SectionState;
            history.Task = GetInternalDetailTask(data, nDataIndex, nData, fData, strData, history.ComponentHistoryID, componentHistories);
            history.Time = time;
            history.Type = historyType;
            history.IsDetailLog = true;

            componentHistories.Add(history);
            return true;
        }

        private static bool AddProcessDetail(List<ComponentHistory> componentHistories, int nDataIndex, VariousData<int> nData, VariousData<float> fData, string strData, DateTime time, ComponentHistory parentHistory)
        {
            ComponentHistory.HistoryType historyType = ComponentHistory.GetComponentHistoryType(nDataIndex, nData, fData, strData);

            // 문자전송이나 완료체크를 제외하곤 모두 무시한다.
            if (historyType != ComponentHistory.HistoryType.CHECK_MISSION &&
                historyType != ComponentHistory.HistoryType.SEND_UNIT_SMS)
                return false;

            Sections.SectionProcess section = (Sections.SectionProcess)parentHistory.SectionState.Section;
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

            if (nDataIndex >= data.MissionItems.Count)
                return false;

            Sections.MissionItem item = (Sections.MissionItem)data.MissionItems[nDataIndex];

            ComponentHistory history = new ComponentHistory();

            history.ActionStepHistory = parentHistory.ActionStepHistory;
            history.ComponentHistoryID = parentHistory.ComponentHistoryID;
            history.AccessedUserID = parentHistory.AccessedUserID;
            history.Commander = parentHistory.Commander;
            history.Receiver = parentHistory.Receiver;
            history.SectionState = parentHistory.SectionState;
            history.Task = GetProcessDetailTask(data, item);
            history.Time = time;
            history.Type = historyType;
            history.IsDetailLog = true;

            if (history.Commander == null)
            {
                bool isDayLight = IsDayLight(time);
                int nAccessedUserID = isDayLight ? history.AccessedUserID : -history.AccessedUserID;

                string strCommanderName;

                if (m_dicSOPGenUserCommanderNames.TryGetValue(nAccessedUserID, out strCommanderName))
                    history.Commander = strCommanderName;
            }

            componentHistories.Add(history);
            return true;
        }

        public static string GetDetailTask(Models.SectionData data)
        {
            if (data is Sections.SectionDataEndPoint)
            {
                Sections.SectionDataEndPoint dataEndPoint = (Sections.SectionDataEndPoint)data;

                if (dataEndPoint.IsBegin)
                    return "SOP 시작";
                else
                    return "SOP 종료";
            }

            string strTask = data.SectionNumber > 0 ? "[" + data.SectionNumber.ToString() + "] " : "";
            strTask += data.Title;

            return strTask;
        }

        private static string GetInternalDetailTask(Models.SectionData data, int nDataIndex, VariousData<int> nData, VariousData<float> fData, string strData, int nComponentHistoryID, List<ComponentHistory> componentHistories)
        {
            if (strData == null)
            {
                int nHistoryCount = componentHistories.Count;

                for (int i = nHistoryCount - 1; i >= 0; i--)
                {
                    ComponentHistory history = componentHistories[i];

                    // CheckBox가 눌려질 경우는 로그에 전파내용을 따로 기록하지 않으므로,
                    // 이전에
                    if (history.ComponentHistoryID == nComponentHistoryID)
                        return history.Task;
                }

                return "";
            }

            if (nDataIndex == ComponentHistory.RUN_BROADCAST_INTERNAL)
                return GetBroadcastInternalMessage(strData);
            if (nDataIndex == ComponentHistory.RUN_SMS_INTERNAL)
                return GetSMSInternalMessage(strData);

            return "";
        }

        private static string GetSMSInternalMessage(string strData)
        {
            int nIndex1 = strData.IndexOf(']');

            if (nIndex1 < 0)
                return "";

            int nIndex2 = strData.IndexOf(']', nIndex1 + 1);

            if (nIndex2 < 0)
                return "";

            int nIndex3 = strData.IndexOf(',', nIndex2 + 1);

            if (nIndex3 < 0)
                return "";

            string strMessage = strData.Substring(nIndex3 + 1);
            return strMessage.Trim();
        }

        private static string GetBroadcastInternalMessage(string strData)
        {
            int nIndex1 = strData.IndexOf(',');

            if (nIndex1 < 0)
                return "";

            int nIndex2 = strData.IndexOf(',', nIndex1 + 1);

            if (nIndex2 < 0)
                return "";

            string strMessage = strData.Substring(nIndex2 + 1);
            return strMessage.Trim();
        }

        private static string GetProcessDetailTask(Models.SectionData data, Sections.MissionItem item)
        {
            string strTask = "";
            int nIndex = ((Sections.SectionDataProcess)data).MissionItems.IndexOf(item);

            if (data.SectionNumber > 0)
            {
                if (nIndex >= 0)
                    strTask = string.Format("({0}-{1}) ", data.SectionNumber, nIndex + 1);
                else
                    strTask = string.Format("({0}) ", data.SectionNumber);
            }

            //string strTask = data.SectionNumber > 0 ? "[" + data.SectionNumber.ToString() + "] " : "";
            //strTask += data.Title;

            if (item.Mission.Length > 0)
            {
                strTask += /*"\r\n" + */item.Mission;
            }

            return strTask;
        }

        private static bool GetStatusString(SectionState.State state, ref string strStatus)
        {
            if (state == SectionState.State.NORMAL)        // 대기
                strStatus = "대기";
            else if (state == SectionState.State.INPUT)    // 입력 대기
            {
                strStatus = "대기";
                //strStatus = "입력대기";
                // 입력 대기는 로그를 기록하지 않는다.
                //return false;
            }
            else if (state == SectionState.State.RUN)      // 실행중
                strStatus = "실행중";
            else if (state == SectionState.State.SKIP)     // 건너뛰기
            {
                strStatus = "건너뛰기";
                // 건너뛰기는 로그를 기록하지 않는다.
                return false;
            }
            else if (state == SectionState.State.DONE)     // 실행 완료
                strStatus = "완료";
            else
                return false;

            return true;
        }

        public static SectionState MakeEndPointSectionState(bool isBegin)
        {
            Sections.SectionEndPoint section = new Sections.SectionEndPoint();
            ((Sections.SectionDataEndPoint)section.Data).IsBegin = isBegin;

            SectionState sectionState = new SectionState();
            sectionState.Section = section;
            return sectionState;
        }

        public static bool GetCurrentActionStepID(WebDBManager dbMgr, out int nActionStepID, out bool isRealMode)
        {
            nActionStepID = -1;
            isRealMode = false;

            string strSQL = "Select ActionStepID, RealMode from ActionStepHistory where ID in ";
            strSQL += "(Select ActionStepHistoryID from ComponentHistory where ID = ";
            strSQL += "(Select max(ID) from ComponentHistory where ActionStepHistoryID in ";
            strSQL += "(Select id from ActionStepHistory where EndTime is null and CancelTime is null))) order by ID desc";
            //string strSQL = "Select ActionStepID, RealMode from CurrentActionStep where SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
                return false;

            VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> realMode = WebDBManager.GetIntField(arrResult[1].ToString());

            if (actionStepID == null || realMode == null)
                return false;

            nActionStepID = actionStepID.Data;
            isRealMode = realMode.Data == 1;
            return true;
        }
    }
}
