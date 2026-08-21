using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Vacation.BLL
{
    using IDAL;
    using Model;
    using Models.Vacation;
    using Models.Account;

    public class VacationManager
    {
        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;
        private int m_nBaseVacationDay = 0;
        private int m_nReservationMonth = 0;
        private int m_nRequestTimeoutDay = 0;

        private static int m_nBeginWorkHour = 9;
        private static int m_nBeginWorkMinute = 0;
        private static int m_nEndWorkHour = 18;
        private static int m_nEndWorkMinute = 0;

        // 기본 휴가일수
        public int BaseVacationDay
        {
            get { return m_nBaseVacationDay; }
        }

        // 현재 날짜로부터 몇개월 앞까지 휴가 예약이 가능한가?
        public int ReservationMonth
        {
            get { return m_nReservationMonth; }
        }

        // 처리되지 않은 결재는 몇일이 지나면 폐기되는가?
        public int RequestTimeoutDay
        {
            get { return m_nRequestTimeoutDay; }
        }

        public static int BeginWorkHour
        {
            get { return m_nBeginWorkHour; }
        }

        public static int BeginWorkMinute
        {
            get { return m_nBeginWorkMinute; }
        }

        public static int EndWorkHour
        {
            get { return m_nEndWorkHour; }
        }

        public static int EndWorkMinute
        {
            get { return m_nEndWorkMinute; }
        }

        public VacationManager(IDataManager dataManager, ProcessManager processManager)
        {
            m_dataManager = dataManager;
            m_processManager = processManager;
            ReadOptions();
        }

        private void ReadOptions()
        {
            string strBaseVacationDays = "BaseVacationDays";
            string strReservationMonth = "ReservationMonth";
            string strRequestTimeout = "RequestIgnoreDays";

            List<string> propertyNames = new List<string>();
            propertyNames.Add(strBaseVacationDays);
            propertyNames.Add(strReservationMonth);
            propertyNames.Add(strRequestTimeout);

            string strErrorMessage;

            List<VacationOption> options = m_dataManager.GetSelectManager().SelectOptions(propertyNames, out strErrorMessage);

            if (options != null && strErrorMessage == null)
            {
                foreach (VacationOption option in options)
                {
                    if (string.Compare(option.PropertyName, strBaseVacationDays, true) == 0)
                    {
                        int nDay;

                        if (int.TryParse(option.PropertyValue, out nDay))
                            m_nBaseVacationDay = nDay;
                    }
                    else if (string.Compare(option.PropertyName, strReservationMonth, true) == 0)
                    {
                        int nMonth;

                        if (int.TryParse(option.PropertyValue, out nMonth))
                            m_nReservationMonth = nMonth;
                    }
                    else if (string.Compare(option.PropertyName, strRequestTimeout, true) == 0)
                    {
                        int nDay;

                        if (int.TryParse(option.PropertyValue, out nDay))
                            m_nRequestTimeoutDay = nDay;
                    }
                }
            }
        }

        public static void SetWorkTimes(int nBeginWorkHour, int nBeginWorkMinute, int nEndWorkHour, int nEndWorkMinute)
        {
            m_nBeginWorkHour = nBeginWorkHour;
            m_nBeginWorkMinute = nBeginWorkMinute;
            m_nEndWorkHour = nEndWorkHour;
            m_nEndWorkMinute = nEndWorkMinute;
        }

        public RequestManagerResult GetSpecialVacationManager(string strUserID)
        {
            string strErrorMessage;
            Models.Account.ApplicationUser user = RequestManager.GetApplicationUser(m_dataManager, strUserID, null, out strErrorMessage);

            if (user == null)
                return GetRequestSpecialVacationManagerResult(null, strErrorMessage);

            List<Models.Account.ApplicationUser> managers = new List<Models.Account.ApplicationUser>();

            if (user.IsAdmin || user.IsTopManager)
            {
                managers.Add(user);
                return GetRequestSpecialVacationManagerResult(managers, "");
            }
            else if (user.IsTeamLeader == false)
                return GetRequestSpecialVacationManagerResult(null, "특별휴가 신청은 팀장 또는 시스템 관리자만 가능합니다.");
            else
            {
                if (AddTopManagers(managers, out strErrorMessage) == false)
                {
                    return GetRequestSpecialVacationManagerResult(null, strErrorMessage);
                }
            }

            return GetRequestSpecialVacationManagerResult(managers, "");
        }

        private RequestManagerResult GetRequestSpecialVacationManagerResult(List<Models.Account.ApplicationUser> managers, string strMessage)
        {
            RequestManagerResult result = new RequestManagerResult();

            if (managers == null)
            {
                result.Success = false;
                result.Message = strMessage;
            }
            else
            {
                foreach (Models.Account.ApplicationUser manager in managers)
                {
                    result.Managers.Add(manager);
                }

                result.Success = result.Managers.Count > 0;

                if (result.Success == false)
                {
                    if (strMessage == null || strMessage.Length == 0)
                        result.Message = "팀장 정보를 얻어올 수 없습니다.";
                    else
                        result.Message = strMessage;
                }
            }

            return result;
        }

        /// <summary>
        /// 휴가요청시 승인해줄 담당자를 얻어온다.
        /// </summary>
        /// <param name="strUserID">휴가 요청자의 ID</param>
        /// <param name="requestDays">휴가 요청일수</param>
        /// <param name="isOverRequest">부여된 휴가일수를 초과한 상황인가?</param>
        /// <param name="strMessage"></param>
        /// <returns>CompanyMember, JobLevel, RegularTeam 순으로 들어있다.</returns>
        public RequestManagerResult GetManager(string strUserID, List<Date> requestDays)
        {
            bool isOverRequest = false;
            string strMessage = "";
            ISelectManager selectManager = m_dataManager.GetSelectManager();

            string strErrorMessage;
            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.UserID] = strUserID;

            ArrayList arrMembers = selectManager.SelectCompanyMemberJobLevelRegularTeam(dicConditions, null, null, out strErrorMessage);

            if (arrMembers == null || arrMembers.Count != 3 ||
                (arrMembers[0] is CompanyMember) == false ||
                (arrMembers[1] is JobLevel) == false ||
                (arrMembers[2] is RegularTeam) == false)
            {
                if (strErrorMessage == null)
                    strErrorMessage = string.Format("{0}에 해당하는 직원정보를 찾을수 없습니다.", strUserID);
                return GetRequestManagerResult(null, isOverRequest, strErrorMessage);
            }

            CompanyMember member = (CompanyMember)arrMembers[0];
            JobLevel memberLevel = (JobLevel)arrMembers[1];
            RegularTeam memberTeam = (RegularTeam)arrMembers[2];
            bool isTopManager = member.IsTeamLeader && memberTeam.ParentTeamID == null;

            /*Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.UserID] = strUserID;

            List<CompanyMember> members = selectManager.SelectCompanyMembers(dicConditions, out strMessage);

            if (members == null || strMessage != null)
                return GetRequestManagerResult(null, isOverRequest, strMessage);

            if (members.Count == 0)
            {
                strMessage = string.Format("{0}에 해당하는 직원정보를 찾을수 없습니다.", strUserID);
                return GetRequestManagerResult(null, isOverRequest, strMessage);
            }*/

            Dictionary<int, Model.History> dicHistories = new Dictionary<int, Model.History>();
            Dictionary<int, float> dicVacationDays = new Dictionary<int, float>();
            //CompanyMember member = members[0];

            isOverRequest = false;
            DateTime dtNow = DateTime.Now;

            foreach (Date date in requestDays)
            {
                Model.History history;

                if (dicHistories.TryGetValue(date.Year, out history) == false)
                {
                    history = selectManager.SelectHistory(member.ID, date.Year, out strMessage);

                    if (history == null)
                    {
                        if (strMessage != null)
                            return GetRequestManagerResult(null, isOverRequest, strMessage);

                        DateTime nextVacationDay;
                        float fVacationDay = GetVacationDay(member.StartDate, new DateTime(date.Year, date.Month, date.Day), out nextVacationDay) - GetMinusDay(member.UserID, date.Year - 1);

                        history = m_dataManager.GetCreateManager().CreateHistory(member.ID, date.Year, fVacationDay, 0, 0, new List<int>(), nextVacationDay);

                        if (history == null)
                        {
                            strMessage = "직원의 휴가이력을 생성할 수 없습니다.";
                            return GetRequestManagerResult(null, isOverRequest, strMessage);
                        }
                    }

                    GetHistoryDays(history);
                    dicHistories[date.Year] = history;
                    dicVacationDays[date.Year] = history.TotalDays - history.UsedDays;
                }

                float fVacationDays = dicVacationDays[date.Year];
                float fDay = Date.GetDateCount(date.DateType);//date.Type == Date.DateType.AM || date.Type == Date.DateType.PM ? 0.5f : 1;

                if (fVacationDays - fDay + 0.001 < 0)
                {
                    isOverRequest = true;
                    strMessage = "부여된 휴가일수를 초과하여 경영진 승인이 필요합니다.";
                }

                dicVacationDays[date.Year] = fVacationDays - fDay;
            }

            if (isOverRequest == false && isTopManager)
            {
                // 경영진은 본인이 직접 결재한다.
                // 필요없는 데이터는 지운다.
                member.Password = null;
                return GetRequestManagerResult(arrMembers, isOverRequest, strMessage);
            }

            Dictionary<CompanyMember.Fields, object> dicConditions1 = new Dictionary<CompanyMember.Fields, object>();
            dicConditions1[CompanyMember.Fields.IsTeamLeader] = true;

            Dictionary<RegularTeam.Fields, object> dicConditions3 = new Dictionary<RegularTeam.Fields, object>();
            dicConditions3[RegularTeam.Fields.ID] = member.TeamID;

            ArrayList arrResult = selectManager.SelectCompanyMemberJobLevelRegularTeam(dicConditions1, null, dicConditions3, out strErrorMessage);

            if (arrResult == null)
            {
                strMessage = strErrorMessage;
                return GetRequestManagerResult(null, isOverRequest, strMessage);
            }

            CompanyMember manager = null;
            RegularTeam managerTeam = null;
            JobLevel managerLevel = null;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                if (arrResult[i] is CompanyMember &&
                    arrResult[i + 1] is JobLevel &&
                    arrResult[i + 2] is RegularTeam)
                {
                    if (manager == null)
                    {
                        manager = (CompanyMember)arrResult[i];
                        managerLevel = (JobLevel)arrResult[i + 1];
                        managerTeam = (RegularTeam)arrResult[i + 2];
                    }
                    else
                    {
                        CompanyMember _manager = (CompanyMember)arrResult[i];

                        // 팀장이 둘 이상일 경우 직급이 높은 사람을 선택한다.
                        if (manager.JobLevelID < _manager.JobLevelID)
                        {
                            manager = _manager;
                            managerLevel = (JobLevel)arrResult[i + 1];
                            managerTeam = (RegularTeam)arrResult[i + 2];
                        }
                        // 직급이 같은 팀장이 둘 이상일 경우 입사일자가 빠른 사람을 선택한다.
                        else if (manager.JobLevelID == _manager.JobLevelID)
                        {
                            if (manager.StartDate > _manager.StartDate)
                            {
                                manager = _manager;
                                managerLevel = (JobLevel)arrResult[i + 1];
                                managerTeam = (RegularTeam)arrResult[i + 2];
                            }
                        }
                    }
                }
            }

            if (manager == null)
            {
                strMessage = "팀장 정보를 찾을수 없습니다.";
                return GetRequestManagerResult(null, isOverRequest, strMessage);
            }

            // 필요없는 데이터는 지운다.
            manager.Password = null;

            ArrayList managers = new ArrayList();
            managers.Add(manager);
            managers.Add(managerLevel);
            managers.Add(managerTeam);

            if (isOverRequest)
            {
                // 휴가일수를 초과할 경우 경영진 추가
                if (AddTopManagers(managers, out strErrorMessage) == false)
                {
                    strMessage = strErrorMessage;
                    return GetRequestManagerResult(null, isOverRequest, strMessage);
                }
            }

            return GetRequestManagerResult(managers, isOverRequest, strMessage);
        }

        private bool GetHistoryDays(Model.History history)
        {
            string strIDs = "";

            foreach (int nRequestID in history.RequestIDs)
            {
                if (strIDs.Length == 0)
                    strIDs = nRequestID.ToString();
                else
                    strIDs += "," + nRequestID.ToString();
            }

            if (strIDs.Length == 0)
            {
                history.UsedDays = 0;
                history.WaitingDays = 0;
            }
            else
            {
                Dictionary<Request.Fields, object> dicConditions = new Dictionary<Request.Fields, object>();
                dicConditions[Request.Fields.MemberID] = history.MemberID;
                dicConditions[Request.Fields.Year] = history.Year;

                string strErrorMessage;
                List<Request> requests = m_dataManager.GetSelectManager().SelectRequests(dicConditions, out strErrorMessage);

                if (requests == null)
                {
                    System.Diagnostics.Trace.WriteLine("GetHistoryDays Error : " + strErrorMessage);
                    return false;
                }

                float usedDays = 0;
                float waitingDays = 0;

                foreach (Request request in requests)
                {
                    if (request.Response == Response.ResponseType.Permit)
                    {
                        foreach (Date date in request.Days)
                        {
                            usedDays += Date.GetDateCount(date.DateType);
                            /*if (date.Type == Date.DateType.Normal)
                                usedDays += 1;
                            else if (date.Type == Date.DateType.AM || date.Type == Date.DateType.PM)
                                usedDays += 0.5f;*/
                        }
                    }
                    else if (request.Response == Response.ResponseType.Processing)
                    {
                        foreach (Date date in request.Days)
                        {
                            waitingDays += Date.GetDateCount(date.DateType);
                            /*if (date.Type == Date.DateType.Normal)
                                waitingDays += 1;
                            else if (date.Type == Date.DateType.AM || date.Type == Date.DateType.PM)
                                waitingDays += 0.5f;*/
                        }
                    }
                }

                history.UsedDays = usedDays;
                history.WaitingDays = waitingDays;
            }

            return true;
        }

        private RequestManagerResult GetRequestManagerResult(ArrayList arrResult, bool isOverRequest, string strMessage)
        {
            RequestManagerResult result = new RequestManagerResult();

            if (arrResult == null)
            {
                result.Success = false;
                result.Message = strMessage;
            }
            else
            {
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 2; i += 3)
                {
                    if (arrResult[i] is CompanyMember &&
                        arrResult[i + 1] is JobLevel &&
                        arrResult[i + 2] is RegularTeam)
                    {
                        CompanyMember member = (CompanyMember)arrResult[i];
                        JobLevel level = (JobLevel)arrResult[i + 1];
                        RegularTeam team = (RegularTeam)arrResult[i + 2];

                        Models.Account.ApplicationUser manager = new Models.Account.ApplicationUser();

                        manager.ID = member.ID;
                        manager.UserID = member.UserID;
                        manager.Name = member.Name;
                        manager.TeamName = team.Name;
                        manager.TeamID = team.ID;
                        manager.PhoneNumber = member.PhoneNumber;
                        manager.Level = level.LevelName;
                        manager.IsTeamLeader = member.IsTeamLeader;
                        manager.IsAdmin = member.IsAdmin;
                        manager.IsTopManager = Models.Account.ApplicationUser.CheckTopManager(member, team, level.LevelName);

                        result.Managers.Add(manager);
                    }
                }

                result.IsOverRequest = isOverRequest;
                result.Success = result.Managers.Count > 0;

                if (result.Success == false)
                {
                    if (strMessage == null || strMessage.Length == 0)
                        result.Message = "팀장 정보를 얻어올 수 없습니다.";
                    else
                        result.Message = strMessage;
                }
                else if (result.IsOverRequest)
                {
                    if (strMessage == null || strMessage.Length == 0)
                        result.Message = "부여된 휴가일수를 초과하여 경영진 승인이 필요합니다.";
                    else
                        result.Message = strMessage;
                }
            }

            return result;
        }

        // 경영진 추가
        private bool AddTopManagers(ArrayList managers, out string strErrorMessage)
        {
            Dictionary<CompanyMember.Fields, object> dicConditions1 = new Dictionary<CompanyMember.Fields, object>();
            dicConditions1[CompanyMember.Fields.IsTeamLeader] = true;

            Dictionary<RegularTeam.Fields, object> dicConditions3 = new Dictionary<RegularTeam.Fields, object>();
            dicConditions3[RegularTeam.Fields.ParentID] = null;

            ArrayList arrResult = m_dataManager.GetSelectManager().SelectCompanyMemberJobLevelRegularTeam(dicConditions1, null, dicConditions3, out strErrorMessage);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            // 직급에 따라 정렬하기 위한 container
            Dictionary<CompanyMember, int> dicManagers = new Dictionary<CompanyMember, int>();
            List<CompanyMember> managerList = new List<CompanyMember>();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                if (arrResult[i] is CompanyMember &&
                    arrResult[i + 1] is JobLevel &&
                    arrResult[i + 2] is RegularTeam)
                {
                    // 필요없는 데이터는 지운다.
                    CompanyMember manager = (CompanyMember)arrResult[i];
                    manager.Password = null;

                    JobLevel level = (JobLevel)arrResult[i + 1];

                    if (Models.Account.ApplicationUser.CheckTopManager(manager, (RegularTeam)arrResult[i + 2], level.LevelName))
                    {
                        dicManagers[manager] = i;
                        managerList.Add(manager);
                    }
                }
            }

            managerList.Sort();

            for (int i=managerList.Count-1;i>=0;i--)
            {
                CompanyMember manager = managerList[i];
                int nIndex = dicManagers[manager];

                managers.Add(arrResult[nIndex]);
                managers.Add(arrResult[nIndex + 1]);
                managers.Add(arrResult[nIndex + 2]);
            }

            return true;
        }

        // 경영진 추가
        private bool AddTopManagers(List<Models.Account.ApplicationUser> managers, out string strErrorMessage)
        {
            ArrayList arrDatas = new ArrayList();

            if (AddTopManagers(arrDatas, out strErrorMessage) == false)
                return false;

            int nDataCount = arrDatas.Count;

            for (int i=0;i<nDataCount-2;i+=3)
            {
                CompanyMember member = (CompanyMember)arrDatas[i];
                JobLevel level = (JobLevel)arrDatas[i + 1];
                RegularTeam team = (RegularTeam)arrDatas[i + 2];

                Models.Account.ApplicationUser manager = Models.Account.ApplicationUser.MakeUser(member, level, team);
                managers.Add(manager);
            }

            return true;
        }

        /// <summary>
        /// 입사일 이후 year-month-day 날짜가 되었을때 발생하는 휴가일수를 알려준다.
        /// </summary>
        /// <param name="startDate">입사일자</param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="nextVacationDay">year-month-day 이후 다음 휴가 발생일</param>
        /// <param name="prevYearUnusedDays">아직 연차가 발생하지 않은 직원이 전년도 휴가를 모두 소진하지 못하였을때 다음해 1월에 이를 반영하기 위한 자료</param>
        /// <returns>휴가일수</returns>
        public float GetVacationDay(DateTime startDate, DateTime dtNow, out DateTime nextVacationDay, float? prevYearUnusedDays = null)
        {
            float fVacationDay = 0;
            int nYearCount = dtNow.Year - startDate.Year;
            int nextYear = 0, nextMonth = 0, nextDay = 0;

            // 월차 사용자의 경우 한달에 한번씩 휴가를 발생시킬 것인가?
            bool onceInMonth = false;

            if (nYearCount == 0)
            {
                if (onceInMonth)
                {
                    int nMonthCount = dtNow.Month - startDate.Month;

                    if (startDate.Day > dtNow.Day)
                        nMonthCount--;

                    if (nMonthCount > 0)
                        fVacationDay = nMonthCount;

                    nextDay = startDate.Day;

                    if (dtNow.Month == startDate.Month)
                    {
                        nextMonth = startDate.Month + 1;

                        if (nextMonth > 12)
                        {
                            nextYear = startDate.Year + 1;
                            nextMonth = 1;
                        }
                        else
                            nextYear = startDate.Year;
                    }
                    // 입사일 이전날짜로 휴가 계산은 하지 않는다.
                    else// if (month > startDate.Month)
                    {
                        if (dtNow.Day < startDate.Day)
                        {
                            nextYear = dtNow.Year;
                            nextMonth = dtNow.Month;
                        }
                        else
                        {
                            nextMonth = dtNow.Month + 1;

                            if (nextMonth > 12)
                            {
                                nextYear = dtNow.Year + 1;
                                nextMonth = 1;
                            }
                            else
                                nextYear = dtNow.Year;
                        }
                    }
                }
                else
                {
                    int nMonthCount = 12 - startDate.Month;

                    if (startDate.Day == 1)
                        nMonthCount++;

                    if (nMonthCount > 0)
                        fVacationDay = nMonthCount;

                    nextDay = 1;
                    nextMonth = 1;
                    nextYear = dtNow.Year + 1;
                }
            }
            else if (nYearCount == 1)
            {
                // 1월 1일에 입사한 직원은 다음해 1월 1일에 연차가 발생한다.
                // 그 이후에 입사한 직원은 그 다음해 1월 1일에 연차가 발생한다.
                if (startDate.Month == 1 && startDate.Day == 1)
                {
                    fVacationDay = m_nBaseVacationDay;
                    nextYear = dtNow.Year + 1;
                    nextMonth = 1;
                    nextDay = 1;
                }
                else
                {
                    if (onceInMonth)
                    {
                        if (prevYearUnusedDays != null && prevYearUnusedDays > 0)
                        {
                            // 휴가는 해당년도만 계산한다.
                            // 전년도 휴가가 남아있으니 하루만 추가한다.
                            fVacationDay = dtNow.Month;

                            nextYear = dtNow.Year;
                            // 전년도 휴가를 하루 추가했으니 새로운 휴가 개시일은 1월 1일이다.
                            nextDay = 1;
                            nextMonth = dtNow.Month + 1;

                            if (nextMonth > 12)
                            {
                                nextMonth = 1;
                                nextYear = dtNow.Year + 1;
                                nextDay = 1;
                            }
                        }
                        else
                        {
                            // 휴가는 해당년도만 계산한다.
                            fVacationDay = dtNow.Month;

                            nextYear = dtNow.Year;
                            nextDay = startDate.Day;

                            if (startDate.Day > dtNow.Day)
                            {
                                fVacationDay--;
                                nextMonth = dtNow.Month;
                            }
                            else
                            {
                                nextMonth = dtNow.Month + 1;

                                if (nextMonth > 12)
                                {
                                    nextMonth = 1;
                                    nextYear = dtNow.Year + 1;
                                    nextDay = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        fVacationDay = 12;

                        nextYear = dtNow.Year + 1;
                        nextMonth = 1;
                        nextDay = 1;

                        if (prevYearUnusedDays != null && prevYearUnusedDays > 0)
                        {
                            fVacationDay += (float)prevYearUnusedDays;
                        }
                    }
                }
            }
            else
            {
                fVacationDay = m_nBaseVacationDay;

                if (startDate.Month > 1 || startDate.Day > 1)
                    nYearCount--;

                // 3년마다 하루씩 연차가 늘어난다.
                fVacationDay += (nYearCount / 3);

                nextYear = dtNow.Year + 1;
                nextMonth = 1;
                nextDay = 1;
            }

            nextVacationDay = new DateTime(nextYear, nextMonth, nextDay);
            return fVacationDay;
        }

        // year년도에 추가 사용한 휴가일수를 얻어온다.
        public float GetMinusDay(string userID, int year)
        {
            Models.Vacation.History lastYearHistory = GetVacationHistory(userID, year, 12, 31, false);
            // 작년에 초과 사용한 연차일수
            float fMinusDay = 0;

            if (lastYearHistory != null)
            {
                if (lastYearHistory.TotalDays - lastYearHistory.UsedDays < 0)
                {
                    fMinusDay = lastYearHistory.UsedDays - lastYearHistory.TotalDays;
                }
            }

            return fMinusDay;
        }

        public Models.Vacation.History GetVacationHistory(string userID, int year, int month, int day, bool createIfNotExist = true)
        {
            return GetVacationHistory(userID, null, null, null, null, null, null, null, null, null, null, null, year, month, day, createIfNotExist);
            /*float fTotalDays = 0;
            List<SpecialVacation> specialVacations = new List<SpecialVacation>();
            List<Request> requests = new List<Request>();
            Dictionary<Request, List<Response>> dicResponses = new Dictionary<Request, List<Response>>();
            List<SpecialVacationRequest> specialVacationRequests = new List<SpecialVacationRequest>();
            Dictionary<SpecialVacationRequest, List<SpecialVacationResponse>> dicSpecialVacationResponses = new Dictionary<SpecialVacationRequest, List<SpecialVacationResponse>>();
            Dictionary<int, CompanyMember> dicManagers = new Dictionary<int, CompanyMember>();
            Dictionary<CompanyMember, JobLevel> dicManagerLevels = new Dictionary<CompanyMember, JobLevel>();
            Dictionary<CompanyMember, RegularTeam> dicManagerTeams = new Dictionary<CompanyMember, RegularTeam>();

            string strErrorMessage;
            ISelectManager selectManager = m_dataManager.GetSelectManager();

            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.UserID] = userID;

            List<CompanyMember> members = selectManager.SelectCompanyMembers(dicConditions, out strErrorMessage);

            if (members == null)
                return GetFailVacationHistory(strErrorMessage);

            if (members.Count == 0)
            {
                strErrorMessage = string.Format("{0}에 해당하는 직원정보를 찾을수 없습니다.", userID);
                return GetFailVacationHistory(strErrorMessage);
            }

            CompanyMember member = members[0];
            Model.History history = selectManager.SelectHistory(member.ID, year, out strErrorMessage);

            if (history == null)
            {
                if (createIfNotExist == false)
                    return null;

                DateTime nextVacationDay;
                float fVacationDay = GetVacationDay(member.StartDate, new DateTime(year, month, day), out nextVacationDay) - GetMinusDay(member.UserID, year -1);

                history = m_dataManager.GetCreateManager().CreateHistory(member.ID, year, fVacationDay, 0, 0, new List<int>(), nextVacationDay);

                if (history == null)
                {
                    strErrorMessage = "직원의 휴가이력을 생성할 수 없습니다.";
                    return GetFailVacationHistory(strErrorMessage);
                }
            }

            fTotalDays = history.TotalDays;
            List<SpecialVacation> svs = selectManager.SelectSpecialVacations(member.ID, year, out strErrorMessage);

            if (svs == null)
                return GetFailVacationHistory(strErrorMessage);

            Dictionary<int, SpecialVacationRequest> dicSvRequestIDs = new Dictionary<int, SpecialVacationRequest>();
            List<int> svRequestIDs = new List<int>();

            foreach (SpecialVacation vacation in svs)
            {
                SpecialVacationRequest request = selectManager.SelectSpecialVacationRequest(vacation.RequestID, out strErrorMessage);

                if (request == null)
                    return GetFailVacationHistory(strErrorMessage);

                specialVacations.Add(vacation);
                specialVacationRequests.Add(request);
                dicSvRequestIDs[request.ID] = request;
                svRequestIDs.Add(request.ID);

                if (SetManager(dicManagers, dicManagerLevels, dicManagerTeams, request.RequestManagerID, selectManager, ref strErrorMessage) == false)
                    return GetFailVacationHistory(strErrorMessage);
                if (SetManager(dicManagers, dicManagerLevels, dicManagerTeams, request.ResponseManagerIDs, selectManager, ref strErrorMessage) == false)
                    return GetFailVacationHistory(strErrorMessage);
            }

            List<SpecialVacationResponse> svResponses = selectManager.SelectSpecialVacationResponse(svRequestIDs, out strErrorMessage);

            if (svResponses == null)
                return GetFailVacationHistory(strErrorMessage);

            foreach (SpecialVacationResponse response in svResponses)
            {
                SpecialVacationRequest request;
                List<SpecialVacationResponse> _responses;
                
                if (dicSvRequestIDs.TryGetValue(response.RequestID, out request))
                {
                    if (dicSpecialVacationResponses.TryGetValue(request, out _responses) == false)
                    {
                        _responses = new List<SpecialVacationResponse>();
                        dicSpecialVacationResponses[request] = _responses;
                    }

                    _responses.Add(response);
                }
            }

            Dictionary<Request.Fields, object> dicConditions2 = new Dictionary<Request.Fields, object>();
            dicConditions2[Request.Fields.MemberID] = member.ID;
            dicConditions2[Request.Fields.Year] = year;

            List<Request> requestList = selectManager.SelectRequests(dicConditions2, out strErrorMessage);

            if (requestList == null)
                return GetFailVacationHistory(strErrorMessage);

            Dictionary<int, Request> dicRequestIDs = new Dictionary<int, Request>();
            List<int> requestIDs = new List<int>();

            foreach (Request request in requestList)
            {
                requests.Add(request);
                requestIDs.Add(request.ID);
                dicRequestIDs[request.ID] = request;

                if (SetManager(dicManagers, dicManagerLevels, dicManagerTeams, request.ManagerIDs, selectManager, ref strErrorMessage) == false)
                    return GetFailVacationHistory(strErrorMessage);
            }

            List<Response> responses = selectManager.SelectResponse(requestIDs, out strErrorMessage);

            if (responses == null)
                return GetFailVacationHistory(strErrorMessage);

            List<Response> responseList;

            foreach (Response response in responses)
            {
                Request request;

                if (dicRequestIDs.TryGetValue(response.RequestID, out request) == false)
                    continue;

                if (dicResponses.TryGetValue(request, out responseList) == false)
                {
                    responseList = new List<Response>();
                    dicResponses[request] = responseList;
                }

                responseList.Add(response);
            }

            return GetVacationHistory(userID, year, month, day, fTotalDays, specialVacations, requests, dicResponses, specialVacationRequests, dicSpecialVacationResponses, dicManagers, dicManagerLevels, dicManagerTeams, out strErrorMessage);*/
        }

        private Models.Vacation.History GetVacationHistory(string userID, CompanyMember member, Dictionary<int, Model.History> dicMemberHistories, Dictionary<int, List<SpecialVacation>> dicMemberSpecialVacations, Dictionary<int, SpecialVacationRequest> dicSpecialVacationRequests, List<SpecialVacationResponse> svResponses, Dictionary<int, List<Request>> dicLastYearMemberRequests, Dictionary<int, List<Request>> dicMemberRequests, List<Response> responses, Dictionary<int, CompanyMember> allMembers, Dictionary<int, RegularTeam> allTeams, Dictionary<int, JobLevel> allLevels, int year, int month, int day, bool createIfNotExist = true)
        {
            float fTotalDays = 0;
            List<SpecialVacation> specialVacations = new List<SpecialVacation>();
            List<Request> requests = new List<Request>();
            Dictionary<Request, List<Response>> dicResponses = new Dictionary<Request, List<Response>>();
            List<SpecialVacationRequest> specialVacationRequests = new List<SpecialVacationRequest>();
            Dictionary<SpecialVacationRequest, List<SpecialVacationResponse>> dicSpecialVacationResponses = new Dictionary<SpecialVacationRequest, List<SpecialVacationResponse>>();
            Dictionary<int, CompanyMember> dicManagers = new Dictionary<int, CompanyMember>();
            Dictionary<CompanyMember, JobLevel> dicManagerLevels = new Dictionary<CompanyMember, JobLevel>();
            Dictionary<CompanyMember, RegularTeam> dicManagerTeams = new Dictionary<CompanyMember, RegularTeam>();

            string strErrorMessage = null;
            ISelectManager selectManager = m_dataManager.GetSelectManager();

            if (member == null)
            {
                Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
                dicConditions[CompanyMember.Fields.UserID] = userID;

                List<CompanyMember> members = selectManager.SelectCompanyMembers(dicConditions, out strErrorMessage);

                if (members == null)
                    return GetFailVacationHistory(strErrorMessage);

                if (members.Count == 0)
                {
                    strErrorMessage = string.Format("{0}에 해당하는 직원정보를 찾을수 없습니다.", userID);
                    return GetFailVacationHistory(strErrorMessage);
                }

                member = members[0];
            }

            Model.History history = null;

            if (dicMemberHistories != null)
            {
                dicMemberHistories.TryGetValue(member.ID, out history);
            }
            else
            {
                history = selectManager.SelectHistory(member.ID, year, out strErrorMessage);
            }

            if (history == null)
            {
                if (createIfNotExist == false)
                    return null;

                DateTime nextVacationDay;
                float fVacationDay = GetVacationDay(member.StartDate, new DateTime(year, month, day), out nextVacationDay) - GetMinusDay(member.UserID, year - 1);

                history = m_dataManager.GetCreateManager().CreateHistory(member.ID, year, fVacationDay, 0, 0, new List<int>(), nextVacationDay);

                if (history == null)
                {
                    strErrorMessage = "직원의 휴가이력을 생성할 수 없습니다.";
                    return GetFailVacationHistory(strErrorMessage);
                }
                else if (dicMemberHistories != null)
                    dicMemberHistories[member.ID] = history;
            }

            fTotalDays = history.TotalDays;

            List<SpecialVacation> svs = null;

            if (dicMemberSpecialVacations != null)
            {
                if (dicMemberSpecialVacations.TryGetValue(member.ID, out svs) == false)
                    svs = new List<SpecialVacation>();
            }
            else
            {
                svs = selectManager.SelectSpecialVacations(member.ID, year, out strErrorMessage);

                if (svs == null)
                    return GetFailVacationHistory(strErrorMessage);
            }

            Dictionary<int, SpecialVacationRequest> dicSvRequestIDs = new Dictionary<int, SpecialVacationRequest>();
            List<int> svRequestIDs = new List<int>();

            foreach (SpecialVacation vacation in svs)
            {
                SpecialVacationRequest request = null;

                if (dicSpecialVacationRequests != null)
                {
                    if (dicSpecialVacationRequests.TryGetValue(vacation.RequestID, out request) == false)
                        continue;
                }
                else
                {
                    request = selectManager.SelectSpecialVacationRequest(vacation.RequestID, out strErrorMessage);

                    if (request == null)
                        return GetFailVacationHistory(strErrorMessage);
                }

                specialVacations.Add(vacation);
                specialVacationRequests.Add(request);
                dicSvRequestIDs[request.ID] = request;
                svRequestIDs.Add(request.ID);

                if (SetManager(dicManagers, dicManagerLevels, dicManagerTeams, request.RequestManagerID, allMembers, allTeams, allLevels, selectManager, ref strErrorMessage) == false)
                    return GetFailVacationHistory(strErrorMessage);
                if (SetManager(dicManagers, dicManagerLevels, dicManagerTeams, request.ResponseManagerIDs, allMembers, allTeams, allLevels, selectManager, ref strErrorMessage) == false)
                    return GetFailVacationHistory(strErrorMessage);
            }

            if (svResponses == null)
                svResponses = selectManager.SelectSpecialVacationResponse(svRequestIDs, out strErrorMessage);

            if (svResponses == null)
                return GetFailVacationHistory(strErrorMessage);

            foreach (SpecialVacationResponse response in svResponses)
            {
                SpecialVacationRequest request;
                List<SpecialVacationResponse> _responses;

                if (dicSvRequestIDs.TryGetValue(response.RequestID, out request))
                {
                    if (dicSpecialVacationResponses.TryGetValue(request, out _responses) == false)
                    {
                        _responses = new List<SpecialVacationResponse>();
                        dicSpecialVacationResponses[request] = _responses;
                    }

                    _responses.Add(response);
                }
            }

            List<Request> requestList = null;

            if (dicMemberRequests != null)
            {
                if (dicMemberRequests.TryGetValue(member.ID, out requestList) == false)
                {
                    requestList = new List<Request>();
                    dicMemberRequests[member.ID] = requestList;
                }
                //strErrorMessage = "휴가요청 정보를 얻어올수 없습니다.";
            }
            else
            {
                Dictionary<Request.Fields, object> dicConditions2 = new Dictionary<Request.Fields, object>();
                dicConditions2[Request.Fields.MemberID] = member.ID;
                //dicConditions2[Request.Fields.Year] = year;

                bool isNullable;
                string strAdditionalConditions = string.Format("({0} = {1} or {2} = {1})",
                    Request.GetFieldName(Request.Fields.Year, out isNullable),
                    year,
                    Request.GetFieldName(Request.Fields.Year2, out isNullable));

                requestList = selectManager.SelectRequests(dicConditions2, strAdditionalConditions, out strErrorMessage);
            }

            if (requestList == null)
                return GetFailVacationHistory(strErrorMessage);

            // 작년에 사용한 연차중에서 연말에 두 년도에 결처있는 Request가 있는지 확인
            AddRequestFromLastYear(requestList, member.ID, dicLastYearMemberRequests);

            Dictionary<int, Request> dicRequestIDs = new Dictionary<int, Request>();
            List<int> requestIDs = new List<int>();

            foreach (Request request in requestList)
            {
                requests.Add(request);
                requestIDs.Add(request.ID);
                dicRequestIDs[request.ID] = request;

                if (SetManager(dicManagers, dicManagerLevels, dicManagerTeams, request.ManagerIDs, allMembers, allTeams, allLevels, selectManager, ref strErrorMessage) == false)
                    return GetFailVacationHistory(strErrorMessage);
            }

            if (responses == null)
                responses = selectManager.SelectResponse(requestIDs, out strErrorMessage);

            if (responses == null)
                return GetFailVacationHistory(strErrorMessage);

            List<Response> responseList;

            foreach (Response response in responses)
            {
                Request request;

                if (dicRequestIDs.TryGetValue(response.RequestID, out request) == false)
                    continue;

                if (dicResponses.TryGetValue(request, out responseList) == false)
                {
                    responseList = new List<Response>();
                    dicResponses[request] = responseList;
                }

                responseList.Add(response);
            }

            return GetVacationHistory(userID, year, month, day, fTotalDays, specialVacations, requests, dicResponses, specialVacationRequests, dicSpecialVacationResponses, dicManagers, dicManagerLevels, dicManagerTeams, out strErrorMessage);
        }

        // 작년에 사용한 연차중에서 연말에 두 년도에 결처있는 Request가 있는지 확인
        private void AddRequestFromLastYear(List<Request> requestList, int memberID, Dictionary<int, List<Request>> dicLastYearMemberRequests)
        {
            if (dicLastYearMemberRequests != null)
            {
                List<Request> prevRequestList;

                if (dicLastYearMemberRequests.TryGetValue(memberID, out prevRequestList))
                {
                    foreach (Request request in prevRequestList)
                    {
                        if (request.Year2 != null)
                            requestList.Add(request);
                    }
                }
            }
        }

        private Models.Vacation.History GetFailVacationHistory(string strErrorMessage)
        {
            Models.Vacation.History history = new Models.Vacation.History();
            history.Success = false;
            history.Message = strErrorMessage;
            return history;
        }

        private Models.Vacation.History GetVacationHistory(string userID, int year, int month, int day, float fTotalDays, List<SpecialVacation> specialVacations, List<Request> requests, Dictionary<Request, List<Response>> dicResponses, List<SpecialVacationRequest> specialVacationRequests, Dictionary<SpecialVacationRequest, List<SpecialVacationResponse>> dicSpecialVacationResponses, Dictionary<int, CompanyMember> dicManagers, Dictionary<CompanyMember, JobLevel> dicManagerLevels, Dictionary<CompanyMember, RegularTeam> dicManagerTeams, out string strErrorMessage)
        {
            strErrorMessage = null;
            Models.Vacation.History history = new Models.Vacation.History();

            history.Year = year;
            history.Month = month;
            history.Day = day;

            float fSpecialDays = 0;
            List<SpecialVacationData> svDatas = new List<SpecialVacationData>();

            foreach (SpecialVacation sv in specialVacations)
            {
                SpecialVacationRequest request = GetRequest(sv.RequestID, specialVacationRequests);

                if (request == null)
                    continue;

                List<SpecialVacationResponse> responses;

                if (dicSpecialVacationResponses.TryGetValue(request, out responses) == false)
                    continue;

                SpecialVacationData svData = new SpecialVacationData();

                svData.CreateTime = sv.CreateTime;
                svData.Days = sv.Days;
                svData.Description = sv.Description;

                foreach (SpecialVacationResponse response in responses)
                {
                    Models.Account.ApplicationUser user;
                    Comment comment;

                    if (ParseResponse(response, dicManagers, dicManagerLevels, dicManagerTeams, out user, out comment) == false)
                        continue;

                    svData.ManagerHistories.Add(new KeyValuePair<Models.Account.ApplicationUser, Comment>(user, comment));
                }

                fSpecialDays += svData.Days;
                svDatas.Add(svData);
            }

            AnnualVacation annual = new AnnualVacation();
            annual.BaseDays = fTotalDays - fSpecialDays;
            annual.SpecialVacations.AddRange(svDatas);

            List<VacationDetail> details = ParseVacationDetails(requests, dicResponses, dicManagers, dicManagerLevels, dicManagerTeams);

            if (details == null)
            {
                history.Success = false;
                history.Message = "사용된 휴가이력을 받아올 수 없습니다.";
            }
            else
            {
                history.AnnualVacation = annual;
                history.UsedVacations.AddRange(details);
                history.Success = true;
            }

            history.Calc();
            return history;
        }

        private List<VacationDetail> ParseVacationDetails(List<Request> requests, Dictionary<Request, List<Response>> dicResponses, Dictionary<int, CompanyMember> dicManagers, Dictionary<CompanyMember, JobLevel> dicManagerLevels, Dictionary<CompanyMember, RegularTeam> dicManagerTeams)
        {
            List<VacationDetail> details = new List<VacationDetail>();

            foreach (Request request in requests)
            {
                if (request.Response == Response.ResponseType.Deny ||
                    request.Response == Response.ResponseType.Cancel ||
                    request.Response == Response.ResponseType.Timeout)
                    continue;

                List<Response> responses;

                if (dicResponses.TryGetValue(request, out responses))
                {
                    VacationDetail detail = new VacationDetail();
                    detail.Dates.AddRange(request.Days);

                    Models.Account.ApplicationUser user;
                    Comment comment;

                    foreach (Response response in responses)
                    {
                        if (ParseResponse(response, dicManagers, dicManagerLevels, dicManagerTeams, out user, out comment) == false)
                            continue;

                        detail.Managers.Add(new KeyValuePair<Models.Account.ApplicationUser, Comment>(user, comment));
                    }

                    // 아직 결재순서가 되지 않아서 Response가 없는 Manager들은 null로 Comment를 채운다.
                    foreach (int nManagerID in request.ManagerIDs)
                    {
                        user = GetManager(nManagerID, detail.Managers);

                        if (user == null)
                        {
                            CompanyMember manager;

                            if (dicManagers.TryGetValue(nManagerID, out manager))
                            {
                                JobLevel level;
                                RegularTeam team;

                                if (dicManagerLevels.TryGetValue(manager, out level) && dicManagerTeams.TryGetValue(manager, out team))
                                {
                                    user = Models.Account.ApplicationUser.MakeUser(manager, level, team);
                                    user.ReservationMonth = this.ReservationMonth;
                                    detail.Managers.Add(new KeyValuePair<Models.Account.ApplicationUser, Comment>(user, null));
                                }
                            }
                        }
                    }
                    /*foreach (KeyValuePair<int, CompanyMember> pair in dicManagers)
                    {
                        user = GetManager(pair.Key, detail.Managers);

                        if (user == null)
                        {
                            JobLevel level;
                            RegularTeam team;

                            if (dicManagerLevels.TryGetValue(pair.Value, out level) && dicManagerTeams.TryGetValue(pair.Value, out team))
                            {
                                user = Models.Account.ApplicationUser.MakeUser(pair.Value, level, team);
                                user.ReservationMonth = this.ReservationMonth;
                                detail.Managers.Add(new KeyValuePair<Models.Account.ApplicationUser, Comment>(user, null));
                            }
                        }
                    }*/

                    detail.Calc();
                    details.Add(detail);
                }
            }

            return details;
        }

        private Models.Account.ApplicationUser GetManager(int nManagerID, List<KeyValuePair<Models.Account.ApplicationUser, Comment>> managerComments)
        {
            foreach (KeyValuePair<Models.Account.ApplicationUser, Comment> pair in managerComments)
            {
                if (pair.Key.ID == nManagerID)
                    return pair.Key;
            }

            return null;
        }

        private bool ParseResponse(Response response, Dictionary<int, CompanyMember> dicManagers, Dictionary<CompanyMember, JobLevel> dicManagerLevels, Dictionary<CompanyMember, RegularTeam> dicManagerTeams, out Models.Account.ApplicationUser user, out Comment comment)
        {
            user = null;
            comment = null;

            //if (response.ResponseTime == null)
            //    return true;

            CompanyMember manager;
            JobLevel level;
            RegularTeam team;

            if (dicManagers.TryGetValue(response.ManagerID, out manager) == false)
                return false;

            if (dicManagerLevels.TryGetValue(manager, out level) == false)
                return false;

            if (dicManagerTeams.TryGetValue(manager, out team) == false)
                return false;

            if (response.ResponseTime != null)
            {
                comment = new Comment();
                comment.ResponseType = (int)response.Result;
                comment.Description = response.Description == null ? "" : response.Description;
                comment.TimeStamp = (DateTime)response.ResponseTime;
            }

            user = new Models.Account.ApplicationUser();

            user.ID = manager.ID;
            user.UserID = manager.UserID;
            user.Name = manager.Name;
            user.PhoneNumber = manager.PhoneNumber;
            user.IsAdmin = manager.IsAdmin;
            user.IsTeamLeader = manager.IsTeamLeader;
            user.IsTopManager = Models.Account.ApplicationUser.CheckTopManager(manager, team, level.LevelName);
            user.Level = level.LevelName;
            user.TeamName = team.Name;
            user.TeamID = team.ID;

            return true;
        }

        private bool ParseResponse(SpecialVacationResponse response, Dictionary<int, CompanyMember> dicManagers, Dictionary<CompanyMember, JobLevel> dicManagerLevels, Dictionary<CompanyMember, RegularTeam> dicManagerTeams, out Models.Account.ApplicationUser user, out Comment comment)
        {
            user = null;
            comment = null;

            if (response.ResponseTime == null)
                return true;

            CompanyMember manager;
            JobLevel level;
            RegularTeam team;

            if (dicManagers.TryGetValue(response.ManagerID, out manager) == false)
                return false;

            if (dicManagerLevels.TryGetValue(manager, out level) == false)
                return false;

            if (dicManagerTeams.TryGetValue(manager, out team) == false)
                return false;

            comment = new Comment();
            comment.ResponseType = (int)response.Result;
            comment.Description = response.Description == null ? "" : response.Description;
            comment.TimeStamp = (DateTime)response.ResponseTime;

            user = new Models.Account.ApplicationUser();

            user.ID = manager.ID;
            user.UserID = manager.UserID;
            user.Name = manager.Name;
            user.PhoneNumber = manager.PhoneNumber;
            user.IsAdmin = manager.IsAdmin;
            user.IsTeamLeader = manager.IsTeamLeader;
            user.IsTopManager = Models.Account.ApplicationUser.CheckTopManager(manager, team, level.LevelName);
            user.Level = level.LevelName;
            user.TeamName = team.Name;
            user.TeamID = team.ID;

            return true;
        }

        private SpecialVacationRequest GetRequest(int nRequestID, List<SpecialVacationRequest> requests)
        {
            foreach (SpecialVacationRequest request in requests)
            {
                if (request.ID == nRequestID)
                    return request;
            }

            return null;
        }

        private ArrayList SelectCompanyMemberJobLevelRegularTeam(int memberID, Dictionary<int, CompanyMember> allMembers, Dictionary<int, RegularTeam> allTeams, Dictionary<int, JobLevel> allLevels, ISelectManager selectManager, out string strErrorMessage)
        {
            CompanyMember member = null;
            RegularTeam team = null;
            JobLevel level = null;

            if (allMembers != null && allTeams != null && allLevels != null)
            {
                if (allMembers.TryGetValue(memberID, out member))
                {
                    if (allTeams.TryGetValue(member.TeamID, out team))
                    {
                        allLevels.TryGetValue(member.JobLevelID, out level);
                    }
                }
            }

            strErrorMessage = null;
            ArrayList arrDatas = new ArrayList();

            if (member != null && team != null && level != null)
            {
                arrDatas.Add(member);
                arrDatas.Add(level);
                arrDatas.Add(team);
            }
            else
            {
                Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
                dicConditions[CompanyMember.Fields.ID] = memberID;

                arrDatas = selectManager.SelectCompanyMemberJobLevelRegularTeam(dicConditions, null, null, out strErrorMessage);
            }

            return arrDatas;
        }

        private bool SetManager(Dictionary<int, CompanyMember> dicManagers, Dictionary<CompanyMember, JobLevel> dicManagerLevels, Dictionary<CompanyMember, RegularTeam> dicManagerTeams, int nManagerID, Dictionary<int, CompanyMember> allMembers, Dictionary<int, RegularTeam> allTeams, Dictionary<int, JobLevel> allLevels, ISelectManager selectManager, ref string strErrorMessage)
        {
            CompanyMember member;

            if (dicManagers.TryGetValue(nManagerID, out member) == false)
            {
                ArrayList arrResult = SelectCompanyMemberJobLevelRegularTeam(nManagerID, allMembers, allTeams, allLevels, selectManager, out strErrorMessage);

                /*Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
                dicConditions[CompanyMember.Fields.ID] = nManagerID;

                ArrayList arrResult = selectManager.SelectCompanyMemberJobLevelRegularTeam(dicConditions, null, null, out strErrorMessage);*/

                if (arrResult == null)
                    return false;

                if (arrResult.Count < 3)
                {
                    strErrorMessage = "승인 담당자의 정보를 찾을수 없습니다.(ID : " + nManagerID.ToString() + ")";
                    return false;
                }

                member = (CompanyMember)arrResult[0];
                JobLevel level = (JobLevel)arrResult[1];
                RegularTeam team = (RegularTeam)arrResult[2];

                //member = selectManager.SelectCompanyMember(nManagerID, out strErrorMessage);

                if (member == null)
                    return false;

                dicManagers[nManagerID] = member;
                dicManagerLevels[member] = level;
                dicManagerTeams[member] = team;
            }
            
            return true;
        }

        private bool SetManager(Dictionary<int, CompanyMember> dicManagers, Dictionary<CompanyMember, JobLevel> dicManagerLevels, Dictionary<CompanyMember, RegularTeam> dicManagerTeams, List<int> managerIDs, Dictionary<int, CompanyMember> allMembers, Dictionary<int, RegularTeam> allTeams, Dictionary<int, JobLevel> allLevels, ISelectManager selectManager, ref string strErrorMessage)
        {
            foreach (int nManagerID in managerIDs)
            {
                if (SetManager(dicManagers, dicManagerLevels, dicManagerTeams, nManagerID, allMembers, allTeams, allLevels, selectManager, ref strErrorMessage) == false)
                    return false;
            }
            
            return true;
        }

        public RequestSpecialVacationResult RequestSpecialVacation(string strRequestManagerID, List<string> userIDs, float fDays, string strReason)
        {
            if (strReason == null || strReason.Length == 0)
                return GetRequestSpecialVacationResult(null, 0, 0, "특별휴가는 사유가 있어야 합니다.");

            if (fDays > -0.1f && fDays < 0.1f)
                return GetRequestSpecialVacationResult(null, 0, 0, "부여할 특별휴가의 일수가 지정되지 않았습니다.");

            string strErrorMessage;
            List<Models.Account.ApplicationUser> users = RequestManager.GetApplicationUsers(m_dataManager, userIDs, out strErrorMessage);

            if (users == null)
                return GetRequestSpecialVacationResult(null, 0, 0, strErrorMessage);

            Models.Account.ApplicationUser manager = RequestManager.GetApplicationUser(m_dataManager, strRequestManagerID, null, out strErrorMessage);

            if (manager == null)
                return GetRequestSpecialVacationResult(null, 0, 0, strErrorMessage);

            if (manager.IsTeamLeader == false && manager.IsAdmin == false && manager.IsTopManager == false)
                return GetRequestSpecialVacationResult(null, 0, 0, "특별휴가 신청은 팀장 또는 시스템 관리자만 가능합니다.");

            List<Models.Account.ApplicationUser> responseManagers = new List<Models.Account.ApplicationUser>();
            Response.ResponseType response = Response.ResponseType.Processing;

            if (manager.IsAdmin || manager.IsTopManager)
            {
                responseManagers.Add(manager);
                response = Response.ResponseType.Permit;
            }
            else
            {
                if (AddTopManagers(responseManagers, out strErrorMessage) == false)
                    return GetRequestSpecialVacationResult(null, 0, 0, strErrorMessage);
            }

            return RequestManager.CreateSpecialVacationResult(m_dataManager, manager, responseManagers, users, fDays, response, strReason);
        }

        private RequestSpecialVacationResult GetRequestSpecialVacationResult(List<Models.Account.ApplicationUser> users, float fDays, int nResponseType, string strMessage)
        {
            RequestSpecialVacationResult result = new RequestSpecialVacationResult();

            if (users == null)
            {
                result.Success = false;
                result.Message = strMessage;
            }
            else
            {
                result.Users.AddRange(users);
                result.Days = fDays;
                result.ResponseType = nResponseType;
                result.Success = true;

                if (nResponseType == (int)Response.ResponseType.Permit)
                    result.Message = "특별휴가 신청이 승인되었습니다.";
                else
                    result.Message = "특별휴가를 신청하였습니다.";
            }

            return result;
        }

        /// <summary>
        /// 휴가요청
        /// </summary>
        /// <param name="strUserID">휴가 요청자의 ID</param>
        /// <param name="requestDays">휴가 요청일수</param>
        /// <param name="strRequestDescription">휴가 요청자의 의견</param>
        /// <returns></returns>
        public RequestVacationResult RequestVacation(string strUserID, List<Date> requestDays, string strRequestDescription)
        {
            string strMessage = "";

            if (requestDays == null || requestDays.Count == 0)
            {
                strMessage = "요청할 휴가정보가 없습니다.";
                return RequestManager.GetRequestVacationResult(requestDays, null, null, strMessage);
            }

            string strErrorMessage;
            CompanyMember member = RequestManager.GetCompanyMember(m_dataManager, strUserID, out strErrorMessage);

            if (member == null)
            {
                strMessage = strErrorMessage;
                return RequestManager.GetRequestVacationResult(requestDays, null, null, strMessage);
            }

            JobLevel memberLevel = m_dataManager.GetSelectManager().SelectJobLevel(member.JobLevelID, out strErrorMessage);

            if (memberLevel == null)
            {
                strMessage = strErrorMessage;
                return RequestManager.GetRequestVacationResult(requestDays, null, null, strMessage);
            }

            RequestManagerResult managerResult = GetManager(strUserID, requestDays);

            if (managerResult == null)
                return RequestManager.GetRequestVacationResult(requestDays, null, null, strMessage);

            List<Models.Account.ApplicationUser> managers = new List<Models.Account.ApplicationUser>();
            Dictionary<Models.Account.ApplicationUser, Comment> dicManagerComment = new Dictionary<Models.Account.ApplicationUser, Comment>();

            List<int> managerIDs = new List<int>();
            Dictionary<Models.Account.ApplicationUser, CompanyMember> dicManagers = new Dictionary<Models.Account.ApplicationUser, CompanyMember>();

            foreach (Models.Account.ApplicationUser manager in managerResult.Managers)
            {
                CompanyMember _manager = RequestManager.GetCompanyMember(m_dataManager, manager.UserID, out strErrorMessage);

                if (_manager == null)
                {
                    strMessage = strErrorMessage;
                    return RequestManager.GetRequestVacationResult(requestDays, null, null, strMessage);
                }

                managers.Add(manager);
                managerIDs.Add(_manager.ID);
                dicManagers[manager] = _manager;
            }

            if (managerIDs.Count == 0)
            {
                strMessage = "휴가를 승인할 담당자를 찾을수 없습니다.";
                return RequestManager.GetRequestVacationResult(requestDays, null, null, strMessage);
            }

            int year1 = requestDays[0].Year;
            int year2 = requestDays[requestDays.Count - 1].Year;

            return RequestManager.CreateRequest(m_dataManager, this, member, memberLevel, managerResult.Managers, dicManagers, requestDays, managerIDs, strRequestDescription, year1, year2);
        }

        private void CheckManagerRequests(List<Request> requests, int nManagerID)
        {
            int nRequestID = requests.Count;

            for (int i=nRequestID-1;i>=0;i--)
            {
                Request request = requests[i];

                if (request.ManagerIDs.Contains(nManagerID) == false)
                    requests.RemoveAt(i);
            }
        }

        public ManagerRequestData RequestManagerData(string strManagerUserID, int year)
        {
            string strErrorMessage;
            CompanyMember manager = RequestManager.GetCompanyMember(m_dataManager, strManagerUserID, out strErrorMessage);

            if (manager == null)
                return GetManagerRequestData(null, null, null, null, strErrorMessage);
            
            ISelectManager selectManager = m_dataManager.GetSelectManager();

            /*Dictionary<Request.Fields, object> dicConditions = new Dictionary<Request.Fields, object>();
            dicConditions[Request.Fields.Year] = year;*/

            bool isNullable;
            string strAdditionalConditions = string.Format("{0} like '%{1}%' and {2} >= {3}",
                Request.GetFieldName(Request.Fields.ManagerIDs, out isNullable),
                manager.ID,
                Request.GetFieldName(Request.Fields.Year, out isNullable),
                year);
            List<Request> requests = selectManager.SelectRequests(null/*dicConditions*/, strAdditionalConditions, out strErrorMessage);

            if (requests == null)
                return GetManagerRequestData(null, null, null, null, strErrorMessage);

            // manager에 해당된 것들이 아니면 삭제한다.
            CheckManagerRequests(requests, manager.ID);

            List<Response> responses = null;
            Dictionary<int, Models.Account.ApplicationUser> dicUsers = new Dictionary<int, Models.Account.ApplicationUser>();

            List<WaitingRequest> waitings = GetWaitingRequest(manager.ID, requests, ref responses, dicUsers, out strErrorMessage);

            if (waitings == null)
                return GetManagerRequestData(null, null, null, null, strErrorMessage);

            List<CompletedRequest> datas = GetCompletedRequest(strManagerUserID, requests, ref responses, dicUsers, out strErrorMessage);

            if (datas == null)
                return GetManagerRequestData(null, null, null, null, strErrorMessage);

            List<CompletedRequestSpecialVacation> svCompletes = new List<CompletedRequestSpecialVacation>();
            List<WaitingRequestSpecialVacation> svWaitings = GetRequestSpecialVacations(manager, svCompletes, out strErrorMessage);

            if (svWaitings == null)
                return GetManagerRequestData(null, null, null, null, strErrorMessage);

            return GetManagerRequestData(waitings, datas, svWaitings, svCompletes, "");
        }

        private List<WaitingRequestSpecialVacation> GetRequestSpecialVacations(CompanyMember manager, List<CompletedRequestSpecialVacation> completes, out string strErrorMessage)
        {
            string strResponseTypes = "";
            strResponseTypes = ((int)Response.ResponseType.Deny).ToString();
            strResponseTypes += ", " + ((int)Response.ResponseType.Permit).ToString();
            strResponseTypes += ", " + ((int)Response.ResponseType.Processing).ToString();

            bool isNullable;
            string strAdditionalConditions = string.Format("{0} like '%{1}%' and Response in ({2})",
                SpecialVacationRequest.GetFieldName(SpecialVacationRequest.Fields.ResponseManagerIDs, out isNullable),
                manager.ID,
                strResponseTypes);

            List<SpecialVacationRequest> requests = m_dataManager.GetSelectManager().SelectSpecialVacationRequests(null, strAdditionalConditions, out strErrorMessage);

            if (requests == null)
                return null;

            int nRequestCount = requests.Count;

            for (int i=nRequestCount-1;i>=0;i--)
            {
                SpecialVacationRequest request = requests[i];

                if (request.ResponseManagerIDs.Contains(manager.ID) == false)
                {
                    // manager에 해당된 것들이 아니면 삭제한다.
                    requests.RemoveAt(i);
                }
            }

            List<SpecialVacationResponse> responses = null;
            Dictionary<int, Models.Account.ApplicationUser> dicUsers = new Dictionary<int, Models.Account.ApplicationUser>();

            List<WaitingRequestSpecialVacation> waitings = GetWaitingRequestSpecialVacations(manager.ID, requests, ref responses, dicUsers, out strErrorMessage);

            if (waitings == null)
                return null;

            List<CompletedRequestSpecialVacation> datas = GetCompletedRequestSpecialVacation(manager.UserID, requests, ref responses, dicUsers, out strErrorMessage);

            if (datas == null)
                return null;

            completes.AddRange(datas);
            return waitings;
        }

        private List<CompletedRequest> GetCompletedRequest(string strManagerUserID, List<Request> requests, ref List<Response> responses, Dictionary<int, Models.Account.ApplicationUser> dicUsers, out string strErrorMessage)
        {
            strErrorMessage = null;
            List<CompletedRequest> datas = new List<CompletedRequest>();

            Models.Account.ApplicationUser user;
            List<int> requestIDs = new List<int>();
            Dictionary<int, Request> dicRequest = new Dictionary<int, Request>();

            foreach (Request request in requests)
            {
                requestIDs.Add(request.ID);
                dicRequest[request.ID] = request;
            }

            if (responses == null)
            {
                responses = m_dataManager.GetSelectManager().SelectResponse(requestIDs, out strErrorMessage);

                if (responses == null)
                    return null;
            }

            foreach (Response response in responses)
            {
                if (response.Result != Response.ResponseType.None && response.Result != Response.ResponseType.Timeout)
                {
                    Request request;

                    if (dicRequest.TryGetValue(response.RequestID, out request) == false)
                        continue;

                    if (request.Response == Response.ResponseType.None)
                        continue;

                    CompletedRequest data = new CompletedRequest();

                    if (SetWaitingRequestData(data, request, responses, dicUsers, out strErrorMessage) == false)
                        return null;

                    data.Response = response.Result;
                    data.ResponseTime = (DateTime)response.ResponseTime;

                    foreach (KeyValuePair<Models.Account.ApplicationUser, Comment> pair in data.PrevHistories)
                    {
                        if (pair.Key.UserID == strManagerUserID)
                        {
                            data.MyComment = pair.Value;
                            break;
                        }
                    }

                    datas.Add(data);
                }
            }

            return datas;
        }

        private List<CompletedRequestSpecialVacation> GetCompletedRequestSpecialVacation(string strManagerUserID, List<SpecialVacationRequest> requests, ref List<SpecialVacationResponse> responses, Dictionary<int, Models.Account.ApplicationUser> dicUsers, out string strErrorMessage)
        {
            strErrorMessage = null;
            List<CompletedRequestSpecialVacation> datas = new List<CompletedRequestSpecialVacation>();

            List<int> requestIDs = new List<int>();
            Dictionary<int, SpecialVacationRequest> dicRequest = new Dictionary<int, SpecialVacationRequest>();

            foreach (SpecialVacationRequest request in requests)
            {
                requestIDs.Add(request.ID);
                dicRequest[request.ID] = request;
            }

            if (responses == null)
            {
                responses = m_dataManager.GetSelectManager().SelectSpecialVacationResponse(requestIDs, out strErrorMessage);

                if (responses == null)
                    return null;
            }

            foreach (SpecialVacationResponse response in responses)
            {
                if (response.Result == Response.ResponseType.Deny || response.Result == Response.ResponseType.Permit)
                {
                    SpecialVacationRequest request;

                    if (dicRequest.TryGetValue(response.RequestID, out request) == false)
                        continue;

                    CompletedRequestSpecialVacation data = new CompletedRequestSpecialVacation();

                    if (SetWaitingRequestSpecialVacationData(data, request, responses, dicUsers) == false)
                    {
                        //return null;
                        continue;
                    }

                    data.Response = response.Result;
                    data.ResponseTime = (DateTime)response.ResponseTime;
                    datas.Add(data);
                }
            }

            return datas;
        }

        private List<WaitingRequestSpecialVacation> GetWaitingRequestSpecialVacations(int nManagerID, List<SpecialVacationRequest> requests, ref List<SpecialVacationResponse> responses, Dictionary<int, Models.Account.ApplicationUser> dicUsers, out string strErrorMessage)
        {
            strErrorMessage = null;
            List<WaitingRequestSpecialVacation> waitings = new List<WaitingRequestSpecialVacation>();

            //Models.Account.ApplicationUser user;
            List<int> requestIDs = new List<int>();
            Dictionary<int, SpecialVacationRequest> dicRequest = new Dictionary<int, SpecialVacationRequest>();

            foreach (SpecialVacationRequest request in requests)
            {
                requestIDs.Add(request.ID);
                dicRequest[request.ID] = request;
            }

            if (responses == null)
            {
                responses = m_dataManager.GetSelectManager().SelectSpecialVacationResponse(requestIDs, out strErrorMessage);

                if (responses == null)
                    return null;
            }

            foreach (SpecialVacationResponse response in responses)
            {
                if (response.ManagerID != nManagerID)
                    continue;

                if (response.Result == Response.ResponseType.None)
                {
                    SpecialVacationRequest request;

                    if (dicRequest.TryGetValue(response.RequestID, out request) == false)
                        continue;

                    if (request.Response != Response.ResponseType.Processing)
                        continue;

                    WaitingRequestSpecialVacation waiting = new WaitingRequestSpecialVacation();

                    if (SetWaitingRequestSpecialVacationData(waiting, request, responses, dicUsers) == false)
                        return null;

                    waitings.Add(waiting);
                }
            }

            return waitings;
        }

        private bool SetWaitingRequestSpecialVacationData(WaitingRequestSpecialVacation data, SpecialVacationRequest request, List<SpecialVacationResponse> responses, Dictionary<int, Models.Account.ApplicationUser> dicUsers)
        {
            string strErrorMessage;
            Models.Account.ApplicationUser manager;

            if (dicUsers.TryGetValue(request.RequestManagerID, out manager) == false)
            {
                manager = RequestManager.GetApplicationUser(m_dataManager, request.RequestManagerID, null, out strErrorMessage);

                if (manager == null)
                    return false;

                dicUsers[request.RequestManagerID] = manager;
            }

            data.RequestID = request.ID;
            data.RequestTime = request.RequestTime;
            data.Days = request.Days;
            data.RequestManager = manager;
            data.RequestDescription = request.RequestDescription == null ? "" : request.RequestDescription;

            Models.Account.ApplicationUser member;

            foreach (int memberID in request.MemberIDs)
            {
                if (dicUsers.TryGetValue(memberID, out member))
                    data.TargetMembers.Add(member);
                else
                {
                    member = RequestManager.GetApplicationUser(m_dataManager, memberID, null, out strErrorMessage);

                    if (member == null)
                        return false;
                    else
                    {
                        dicUsers[memberID] = member;
                        data.TargetMembers.Add(member);
                    }
                }
            }

            return true;
        }

        private List<WaitingRequest> GetWaitingRequest(int nManagerID, List<Request> requests, ref List<Response> responses, Dictionary<int, Models.Account.ApplicationUser> dicUsers, out string strErrorMessage)
        {
            strErrorMessage = null;
            List<WaitingRequest> waitings = new List<WaitingRequest>();

            //Models.Account.ApplicationUser user;
            List<int> requestIDs = new List<int>();
            Dictionary<int, Request> dicRequest = new Dictionary<int, Request>();

            foreach (Request request in requests)
            {
                requestIDs.Add(request.ID);
                dicRequest[request.ID] = request;
            }

            if (responses == null)
            {
                responses = m_dataManager.GetSelectManager().SelectResponse(requestIDs, out strErrorMessage);

                if (responses == null)
                    return null;
            }

            foreach (Response response in responses)
            {
                if (response.ManagerID != nManagerID)
                    continue;

                if (response.Result == Response.ResponseType.None)
                {
                    Request request;

                    if (dicRequest.TryGetValue(response.RequestID, out request) == false)
                        continue;

                    if (request.Response != Response.ResponseType.Processing)
                        continue;

                    WaitingRequest waiting = new WaitingRequest();

                    if (SetWaitingRequestData(waiting, request, responses, dicUsers, out strErrorMessage) == false)
                        return null;

                    waitings.Add(waiting);
                }
            }

            return waitings;
        }

        private bool SetWaitingRequestData(WaitingRequest data, Request request, List<Response> responses, Dictionary<int, Models.Account.ApplicationUser> dicUsers, out string strErrorMessage)
        {
            Models.Account.ApplicationUser user;

            if (dicUsers.TryGetValue(request.MemberID, out user) == false)
            {
                user = RequestManager.GetApplicationUser(m_dataManager, request.MemberID, null, out strErrorMessage);

                if (user == null)
                    return false;

                dicUsers[request.MemberID] = user;
            }

            List<KeyValuePair<Models.Account.ApplicationUser, Comment>> prevHistories = GetManagerComments(request, responses, dicUsers, out strErrorMessage);

            if (prevHistories == null)
                return false;

            data.RequestID = request.ID;
            data.RequestTime = request.RequestTime;
            data.RequestDays.AddRange(request.Days);
            data.RequestMember = user;
            data.PrevHistories.AddRange(prevHistories);
            data.RequestDescription = request.RequestDescription == null ? "" : request.RequestDescription;
            data.Calc();

            return true;
        }

        private List<KeyValuePair<Models.Account.ApplicationUser, Comment>> GetManagerComments(Request request, List<Response> responses, Dictionary<int, Models.Account.ApplicationUser> dicUsers, out string strErrorMessage)
        {
            strErrorMessage = null;
            Models.Account.ApplicationUser user;

            List<KeyValuePair<Models.Account.ApplicationUser, Comment>> result = new List<KeyValuePair<Models.Account.ApplicationUser, Comment>>();

            foreach (Response response in responses)
            {
                if (response.RequestID != request.ID)
                    continue;

                if (response.ResponseTime == null)
                    continue;

                if (dicUsers.TryGetValue(response.ManagerID, out user) == false)
                {
                    user = RequestManager.GetApplicationUser(m_dataManager, request.MemberID, null, out strErrorMessage);

                    if (user == null)
                        return null;

                    dicUsers[request.MemberID] = user;
                }

                Comment comment = new Comment();

                comment.Description = response.Description;
                comment.ResponseType = (int)response.Result;
                comment.TimeStamp = (DateTime)response.ResponseTime;

                result.Add(new KeyValuePair<Models.Account.ApplicationUser, Comment>(user, comment));
            }

            return result;
        }

        private ManagerRequestData GetManagerRequestData(List<WaitingRequest> waitingRequests, List<CompletedRequest> completedRequests, List<WaitingRequestSpecialVacation> svWaitingRequests, List<CompletedRequestSpecialVacation> svCompletedRequests, string strMessage)
        {
            ManagerRequestData result = new ManagerRequestData();

            if (waitingRequests == null || completedRequests == null)
            {
                result.Success = false;
                result.Message = strMessage;
            }
            else
            {
                result.Success = true;
                result.Message = strMessage;
            }

            if (waitingRequests != null)
                result.WaitingRequests.AddRange(waitingRequests);

            if (completedRequests != null)
                result.CompletedRequests.AddRange(completedRequests);

            if (svWaitingRequests != null)
                result.WaitingRequestSpecialVacations.AddRange(svWaitingRequests);

            if (svCompletedRequests != null)
                result.CompletedRequestSpecialVacations.AddRange(svCompletedRequests);

            return result;
        }

        public ProcessRequestResult ProcessRequest(int nRequestID, bool permit, string strManagerUserID, string strManagerDescription, bool isNormal)
        {
            if (isNormal)
                return ProcessRequest(nRequestID, permit, strManagerUserID, strManagerDescription);

            return ProcessSpecialVacationRequest(nRequestID, permit, strManagerUserID, strManagerDescription);
        }

        private ProcessRequestResult ProcessSpecialVacationRequest(int nRequestID, bool permit, string strManagerUserID, string strManagerDescription)
        {
            ISelectManager selectManager = m_dataManager.GetSelectManager();

            string strErrorMessage;
            SpecialVacationRequest request = selectManager.SelectSpecialVacationRequest(nRequestID, out strErrorMessage);

            if (request == null)
                return GetProcessRequestResult(null, null, strErrorMessage);

            CompanyMember manager = RequestManager.GetCompanyMember(m_dataManager, strManagerUserID, out strErrorMessage);

            if (manager == null)
                return GetProcessRequestResult(null, null, strErrorMessage);

            Dictionary<SpecialVacationResponse.Fields, object> dicConditions = new Dictionary<SpecialVacationResponse.Fields, object>();

            dicConditions[SpecialVacationResponse.Fields.RequestID] = nRequestID;
            dicConditions[SpecialVacationResponse.Fields.ManagerID] = manager.ID;

            List<SpecialVacationResponse> responses = selectManager.SelectSpecialVacationResponse(dicConditions, out strErrorMessage);

            if (responses == null)
                return GetProcessRequestResult(null, null, strErrorMessage);

            if (responses.Count == 0)
                return GetProcessRequestResult(null, null, "결재처리 내역을 찾을수 없습니다.");

            SpecialVacationResponse response = responses[0];
            response.Result = permit ? Response.ResponseType.Permit : Response.ResponseType.Deny;
            response.Description = strManagerDescription;
            response.ResponseTime = DateTime.Now;

            ProcessRequestResult result = null;

            if (m_dataManager.GetUpdateManager().UpdateSpecialVacationResponse(response, out strErrorMessage) == false)
                result = GetProcessRequestResult(null, null, strErrorMessage);
            else
            {
                Models.Account.ApplicationUser _manager = RequestManager.GetApplicationUser(m_dataManager, manager.ID, null, out strErrorMessage);

                if (_manager == null)
                    return GetProcessRequestResult(null, null, strErrorMessage);

                if (RequestManager.PostSpecialVacationRequest(m_dataManager, request, _manager, permit, out strErrorMessage) == false)
                    result = GetProcessRequestResult(null, null, strErrorMessage);
                else
                    result = GetProcessRequestResult((int)request.ID, (bool)permit, "");
            }

            return result;
        }

        private ProcessRequestResult ProcessRequest(int nRequestID, bool permit, string strManagerUserID, string strManagerDescription)
        {
            ISelectManager selectManager = m_dataManager.GetSelectManager();

            string strErrorMessage;
            Request request = selectManager.SelectRequest(nRequestID, out strErrorMessage);

            if (request == null)
                return GetProcessRequestResult(null, null, strErrorMessage);

            CompanyMember manager = RequestManager.GetCompanyMember(m_dataManager, strManagerUserID, out strErrorMessage);

            if (manager == null)
                return GetProcessRequestResult(null, null, strErrorMessage);

            Dictionary<Response.Fields, object> dicConditions = new Dictionary<Response.Fields, object>();

            dicConditions[Response.Fields.RequestID] = nRequestID;
            dicConditions[Response.Fields.ManagerID] = manager.ID;

            List<Response> responses = selectManager.SelectResponse(dicConditions, out strErrorMessage);

            if (responses == null)
                return GetProcessRequestResult(null, null, strErrorMessage);

            if (responses.Count == 0)
                return GetProcessRequestResult(null, null, "결재처리 내역을 찾을수 없습니다.");

            Response response = responses[0];
            response.Result = permit ? Response.ResponseType.Permit : Response.ResponseType.Deny;
            response.Description = strManagerDescription;
            response.ResponseTime = DateTime.Now;

            ProcessRequestResult result = null;

            if (m_dataManager.GetUpdateManager().UpdateResponse(response, out strErrorMessage) == false)
                result = GetProcessRequestResult(null, null, strErrorMessage);
            else
            {
                Models.Account.ApplicationUser _manager = RequestManager.GetApplicationUser(m_dataManager, manager.ID, null, out strErrorMessage);

                if (_manager == null)
                    return GetProcessRequestResult(null, null, strErrorMessage);

                if (RequestManager.PostRequest(m_dataManager, request, _manager, permit, out strErrorMessage) == false)
                    result = GetProcessRequestResult(null, null, strErrorMessage);
                else
                    result = GetProcessRequestResult((int)request.ID, (bool)permit, "");
            }

            return result;
        }

        private static bool IsLast<DataType>(List<DataType> datas, DataType data)
        {
            if (datas.Count == 0)
                return false;

            if (datas[datas.Count - 1].Equals(data))
                return true;

            return false;
        }

        private ProcessRequestResult GetProcessRequestResult(int? requestID, bool? permit, string strMessage)
        {
            ProcessRequestResult result = new ProcessRequestResult();

            if (requestID == null || permit == null)
            {
                result.Success = false;
                result.Message = strMessage;
            }
            else
            {
                result.Success = true;
                result.Message = strMessage;

                result.RequestID = (int)requestID;
                result.IsPermit = (bool)permit;
            }

            return result;
        }

        public MemberVacationHistory GetMemberVacationHistory(string strManagerUserID)
        {
            string strErrorMessage;
            // strManagerUserID에 상관없이 모든 직원정보를 얻어오는 방식
            CompanyMember _manager = RequestManager.GetTopManager(m_dataManager, out strErrorMessage);
            // strManagerUserID인 팀장 휘하의 직원들 정보만 얻어오는 방식
            //CompanyMember _manager = RequestManager.GetCompanyMember(m_dataManager, strManagerUserID, out strErrorMessage);

            DateTime dtNow = DateTime.Now;

            if (_manager == null)
                return GetMemberVacationHistory(null, null, strErrorMessage, dtNow);

            ArrayList managerDatas = new ArrayList();
            Models.Account.ApplicationUser manager = RequestManager.GetApplicationUser(m_dataManager, _manager.ID, managerDatas, out strErrorMessage);

            if (manager == null)
                return GetMemberVacationHistory(null, null, strErrorMessage, dtNow);

            if (managerDatas.Count < 3 || !(managerDatas[2] is RegularTeam))
                return GetMemberVacationHistory(null, null, strErrorMessage, dtNow);

            // Key : User ID
            // Value : 작년과 올해와, 내년 3년동안의 History
            Dictionary<int, List<Models.Vacation.History>> dicMemberHistory = new Dictionary<int, List<Models.Vacation.History>>();

            MemberTeam rootTeam = GetMembersInManager(manager, (RegularTeam)managerDatas[2], dicMemberHistory, dtNow, out strErrorMessage);

            if (rootTeam == null)
                return GetMemberVacationHistory(null, null, strErrorMessage, dtNow);

            return GetMemberVacationHistory(rootTeam, dicMemberHistory, "", dtNow);
        }

        // Key : User ID
        private MemberTeam GetMembersInManager(Models.Account.ApplicationUser manager, RegularTeam team, Dictionary<int, List<Models.Vacation.History>> dicMemberHistory, DateTime timestamp, out string strErrorMessage)
        {
            List<CompanyMember> members = m_dataManager.GetSelectManager().SelectCompanyMembers(null, out strErrorMessage);

            if (members == null)
                return null;

            List<RegularTeam> teams = m_dataManager.GetSelectManager().SelectRegularTeams(null, out strErrorMessage);

            if (teams == null)
                return null;

            List<JobLevel> levels = m_dataManager.GetSelectManager().SelectJobLevels(null, out strErrorMessage);

            if (levels == null)
                return null;

            Dictionary<int, CompanyMember> dicCompanyMembers = new Dictionary<int, CompanyMember>();
            Dictionary<int, List<CompanyMember>> dicTeamCompanyMembers = new Dictionary<int, List<CompanyMember>>();
            Dictionary<int, RegularTeam> dicTeams = new Dictionary<int, RegularTeam>();
            Dictionary<int, JobLevel> dicLevels = new Dictionary<int, JobLevel>();

            List<CompanyMember> teamMembers = null;

            foreach (CompanyMember member in members)
            {
                dicCompanyMembers[member.ID] = member;

                if (dicTeamCompanyMembers.TryGetValue(member.TeamID, out teamMembers) == false)
                {
                    teamMembers = new List<CompanyMember>();
                    dicTeamCompanyMembers[member.TeamID] = teamMembers;
                }

                teamMembers.Add(member);
            }

            foreach (RegularTeam _team in teams)
            {
                dicTeams[_team.ID] = _team;
            }

            foreach (JobLevel _level in levels)
            {
                dicLevels[_level.ID] = _level;
            }

            MemberTeam rootTeam = new MemberTeam();
            rootTeam.TeamName = team.Name;
            rootTeam.ID = team.ID;

            List<Models.Account.ApplicationUser> users = GetTeamMembers(team, dicMemberHistory, dicCompanyMembers, dicTeamCompanyMembers, dicTeams, dicLevels, timestamp, out strErrorMessage);
            //List<Models.Account.ApplicationUser> users = GetTeamMembers(team, dicMemberHistory, timestamp, out strErrorMessage);

            if (users == null)
                return null;

            /*foreach (Models.Account.ApplicationUser user in users)
            {
                if (user.ID == manager.ID)
                {
                    // manager 자신은 포함시키지 않는다.
                    users.Remove(user);
                    break;
                }
            }*/

            rootTeam.Members.AddRange(users);

            if (manager.IsTopManager)
            {
                if (AddTeamMembers(rootTeam, team.ID, dicMemberHistory, dicCompanyMembers, dicTeamCompanyMembers, dicTeams, dicLevels, timestamp, out strErrorMessage) == false)
                    return null;
            }

            return rootTeam;
        }

        private bool AddTeamMembers(MemberTeam parentTeam, int nTeamID, Dictionary<int, List<Models.Vacation.History>> dicMemberHistory, Dictionary<int, CompanyMember> dicCompanyMembers, Dictionary<int, List<CompanyMember>> dicTeamCompanyMembers, Dictionary<int, RegularTeam> dicTeams, Dictionary<int, JobLevel> dicLevels, DateTime timestamp, out string strErrorMessage)
        {
            Dictionary<RegularTeam.Fields, object> dicConditions = new Dictionary<RegularTeam.Fields, object>();
            dicConditions[RegularTeam.Fields.ParentID] = nTeamID;

            List<RegularTeam> teams = m_dataManager.GetSelectManager().SelectRegularTeams(dicConditions, out strErrorMessage);

            if (teams == null)
                return false;

            foreach (RegularTeam team in teams)
            {
                List<Models.Account.ApplicationUser> users = GetTeamMembers(team, dicMemberHistory, dicCompanyMembers, dicTeamCompanyMembers, dicTeams, dicLevels, timestamp, out strErrorMessage);

                if (users == null)
                    return false;

                MemberTeam childTeam = new MemberTeam();

                childTeam.ID = team.ID;
                childTeam.TeamName = team.Name;
                childTeam.Members.AddRange(users);
                parentTeam.ChildTeams.Add(childTeam);

                if (AddTeamMembers(childTeam, team.ID, dicMemberHistory, dicCompanyMembers, dicTeamCompanyMembers, dicTeams, dicLevels, timestamp, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private bool GetAllCompanyMembers(Dictionary<int, CompanyMember> dicCompanyMembers, Dictionary<int, List<CompanyMember>> dicTeamCompanyMembers, Dictionary<int, JobLevel> dicJobLevels, out string strErrorMessage)
        {
            List<CompanyMember> members = m_dataManager.GetSelectManager().SelectCompanyMembers(null, out strErrorMessage);

            if (members == null)
                return false;

            List<JobLevel> levels = m_dataManager.GetSelectManager().SelectJobLevels(null, out strErrorMessage);

            if (levels == null)
                return false;

            List<CompanyMember> teamMembers = null;

            foreach (CompanyMember member in members)
            {
                dicCompanyMembers[member.ID] = member;

                if (dicTeamCompanyMembers.TryGetValue(member.TeamID, out teamMembers) == false)
                {
                    teamMembers = new List<CompanyMember>();
                    dicTeamCompanyMembers[member.TeamID] = teamMembers;
                }

                teamMembers.Add(member);
            }

            foreach (JobLevel level in levels)
            {
                dicJobLevels[level.ID] = level;
            }

            return true;
        }

        // 전직원에 대하여 작년, 올해, 내년의 History들을 얻어온다.
        private bool GetAllMemberVacationHistories(int lastYear, Dictionary<int, Model.History> dicLastYearHistories, Dictionary<int, Model.History> dicThisYearHistories, Dictionary<int, Model.History> dicNextYearHistories, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} >= {1}", Model.History.GetFieldName(Model.History.Fields.Year, out isNullable), lastYear);
            List<Model.History> histories = m_dataManager.GetSelectManager().SelectHistories(null, strCondition, out strErrorMessage);

            if (histories == null)
                return false;

            Dictionary<int, Model.History> dicMemberHistories = null;

            foreach (Model.History history in histories)
            {
                if (history.Year == lastYear)
                    dicMemberHistories = dicLastYearHistories;
                else if (history.Year == lastYear + 1)
                    dicMemberHistories = dicThisYearHistories;
                else if (history.Year == lastYear + 2)
                    dicMemberHistories = dicNextYearHistories;
                else
                    continue;

                dicMemberHistories[history.MemberID] = history;
            }

            return true;
        }

        private bool GetAllMemberSpecialVacations(int lastYear, Dictionary<int, List<SpecialVacation>> dicLastYearSpecialVacations, Dictionary<int, List<SpecialVacation>> dicThisYearSpecialVacations, Dictionary<int, List<SpecialVacation>> dicNextYearSpecialVacations, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} >= '{1}-01-01 00:00:00'", SpecialVacation.GetFieldName(SpecialVacation.Fields.CreateTime, out isNullable), lastYear);
            List<SpecialVacation> specialVacations = m_dataManager.GetSelectManager().SelectSpecialVacations(null, strCondition, out strErrorMessage);

            if (specialVacations == null)
                return false;

            Dictionary<int, List<SpecialVacation>> dicSpecialVacations = null;
            List<SpecialVacation> vacations = null;

            foreach (SpecialVacation vacation in specialVacations)
            {
                if (vacation.CreateTime.Year == lastYear)
                    dicSpecialVacations = dicLastYearSpecialVacations;
                else if (vacation.CreateTime.Year == lastYear + 1)
                    dicSpecialVacations = dicThisYearSpecialVacations;
                else if (vacation.CreateTime.Year == lastYear + 2)
                    dicSpecialVacations = dicNextYearSpecialVacations;
                else
                    continue;

                if (dicSpecialVacations.TryGetValue(vacation.MemberID, out vacations) == false)
                {
                    vacations = new List<SpecialVacation>();
                    dicSpecialVacations[vacation.MemberID] = vacations;
                }

                vacations.Add(vacation);
            }

            return true;
        }

        private bool GetAllSpecialVacationRequestNResponse(int year, Dictionary<int, SpecialVacationRequest> dicSpecialVacationRequests, List<SpecialVacationResponse> specialVacationResponses, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} >= '{1}-01-01 00:00:00'", SpecialVacationRequest.GetFieldName(SpecialVacationRequest.Fields.RequestTime, out isNullable), year);
            List<SpecialVacationRequest> requests = m_dataManager.GetSelectManager().SelectSpecialVacationRequests(null, strCondition, out strErrorMessage);

            if (requests == null)
                return false;

            int minID = -1;

            foreach (SpecialVacationRequest request in requests)
            {
                dicSpecialVacationRequests[request.ID] = request;

                if (minID < 0 || minID > request.ID)
                    minID = request.ID;
            }

            strCondition = string.Format("{0} >= {1}", SpecialVacationResponse.GetFieldName(SpecialVacationResponse.Fields.RequestID, out isNullable), minID);
            List<SpecialVacationResponse> responses = m_dataManager.GetSelectManager().SelectSpecialVacationResponse(null, strCondition, out strErrorMessage);

            if (responses == null)
                return false;

            specialVacationResponses.AddRange(responses);
            return true;
        }

        private bool GetAllRequests(int lastYear, Dictionary<int, List<Request>> dicLastYearMemberRequests, Dictionary<int, List<Request>> dicThisYearMemberRequests, Dictionary<int, List<Request>> dicNextYearMemberRequests, ref int minRequestID, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} >= {1}", Request.GetFieldName(Request.Fields.Year, out isNullable), lastYear);
            List<Request> requests = m_dataManager.GetSelectManager().SelectRequests(null, strCondition, out strErrorMessage);

            if (requests == null)
                return false;

            Dictionary<int, List<Request>> dicMemberRequests = null;
            List<Request> requestList = null;

            foreach (Request request in requests)
            {
                if (request.Year == lastYear)
                    dicMemberRequests = dicLastYearMemberRequests;
                else if (request.Year == lastYear + 1)
                    dicMemberRequests = dicThisYearMemberRequests;
                else if (request.Year == lastYear + 2)
                    dicMemberRequests = dicNextYearMemberRequests;
                else
                    continue;

                if (dicMemberRequests.TryGetValue(request.MemberID, out requestList) == false)
                {
                    requestList = new List<Request>();
                    dicMemberRequests[request.MemberID] = requestList;
                }

                requestList.Add(request);

                if (minRequestID < 0 || minRequestID > request.ID)
                    minRequestID = request.ID;
            }

            return true;
        }

        // Key : User ID
        private List<Models.Account.ApplicationUser> GetTeamMembers(RegularTeam team, Dictionary<int, List<Models.Vacation.History>> dicMemberHistory, Dictionary<int, CompanyMember> dicCompanyMembers, Dictionary<int, List<CompanyMember>> dicTeamCompanyMembers, Dictionary<int, RegularTeam> dicTeams, Dictionary<int, JobLevel> dicJobLevels, DateTime timestamp, out string strErrorMessage)
        {
            strErrorMessage = null;

            List<Models.Account.ApplicationUser> users = new List<Models.Account.ApplicationUser>();
            List<CompanyMember> teamMembers = null;

            if (dicTeamCompanyMembers.TryGetValue(team.ID, out teamMembers) == false)
                return users;

            Dictionary<int, Model.History> dicLastYearHistories = new Dictionary<int, Model.History>();
            Dictionary<int, Model.History> dicThisYearHistories = new Dictionary<int, Model.History>();
            Dictionary<int, Model.History> dicNextYearHistories = new Dictionary<int, Model.History>();

            // 전직원에 대하여 작년, 올해, 내년의 History들을 얻어온다.
            if (GetAllMemberVacationHistories(timestamp.Year - 1, dicLastYearHistories, dicThisYearHistories, dicNextYearHistories, out strErrorMessage) == false)
                return null;

            Dictionary<int, List<SpecialVacation>> dicLastYearSpecialVacations = new Dictionary<int, List<SpecialVacation>>();
            Dictionary<int, List<SpecialVacation>> dicThisYearSpecialVacations = new Dictionary<int, List<SpecialVacation>>();
            Dictionary<int, List<SpecialVacation>> dicNextYearSpecialVacations = new Dictionary<int, List<SpecialVacation>>();

            // 전직원에 대하여 작년, 올해, 내년의 특별휴가들을 얻어온다.
            if (GetAllMemberSpecialVacations(timestamp.Year - 1, dicLastYearSpecialVacations, dicThisYearSpecialVacations, dicNextYearSpecialVacations, out strErrorMessage) == false)
                return null;

            Dictionary<int, SpecialVacationRequest> dicSpecialVacationRequests = new Dictionary<int, SpecialVacationRequest>();
            List<SpecialVacationResponse> specialVacationResponses = new List<SpecialVacationResponse>();

            if (GetAllSpecialVacationRequestNResponse(timestamp.Year - 1, dicSpecialVacationRequests, specialVacationResponses, out strErrorMessage) == false)
                return null;

            int minRequestID = -1;
            Dictionary<int, List<Request>> dicLastYearMemberRequests = new Dictionary<int, List<Request>>();
            Dictionary<int, List<Request>> dicThisYearMemberRequests = new Dictionary<int, List<Request>>();
            Dictionary<int, List<Request>> dicNextYearMemberRequests = new Dictionary<int, List<Request>>();

            if (GetAllRequests(timestamp.Year - 1, dicLastYearMemberRequests, dicThisYearMemberRequests, dicNextYearMemberRequests, ref minRequestID, out strErrorMessage) == false)
                return null;

            bool isNullable;
            string strCondition = string.Format("{0} >= {1}", Response.GetFieldName(Response.Fields.RequestID, out isNullable), minRequestID);
            List<Response> responses = m_dataManager.GetSelectManager().SelectResponse(null, strCondition, out strErrorMessage);

            if (responses == null)
                return null;

            JobLevel level = null;

            foreach (CompanyMember member in teamMembers)
            {
                if (dicJobLevels.TryGetValue(member.JobLevelID, out level))
                {
                    Models.Account.ApplicationUser user = new Models.Account.ApplicationUser();

                    user.ID = member.ID;
                    user.IsAdmin = member.IsAdmin;
                    user.IsTeamLeader = member.IsTeamLeader;
                    user.IsTopManager = Models.Account.ApplicationUser.CheckTopManager(member, team, level.LevelName);
                    user.Level = level.LevelName;
                    user.Name = member.Name;
                    user.PhoneNumber = member.PhoneNumber;
                    user.StartYear = member.StartDate.Year;
                    user.StartMonth = member.StartDate.Month;
                    user.TeamName = team.Name;
                    user.TeamID = team.ID;
                    user.UserID = member.UserID;

                    users.Add(user);

                    Models.Vacation.History historyLastYear = GetVacationHistory(member.UserID, member, dicLastYearHistories, dicLastYearSpecialVacations, dicSpecialVacationRequests, specialVacationResponses, null, dicLastYearMemberRequests, responses, dicCompanyMembers, dicTeams, dicJobLevels, timestamp.Year - 1, 12, 31, false);
                    Models.Vacation.History historyThisYear = GetVacationHistory(member.UserID, member, dicThisYearHistories, dicThisYearSpecialVacations, dicSpecialVacationRequests, specialVacationResponses, dicLastYearMemberRequests, dicThisYearMemberRequests, responses, dicCompanyMembers, dicTeams, dicJobLevels, timestamp.Year, timestamp.Month, timestamp.Day);
                    Models.Vacation.History historyNextYear = GetVacationHistory(member.UserID, member, dicNextYearHistories, dicNextYearSpecialVacations, dicSpecialVacationRequests, specialVacationResponses, dicThisYearMemberRequests, dicNextYearMemberRequests, responses, dicCompanyMembers, dicTeams, dicJobLevels, timestamp.Year + 1, 1, 1);
                    /*Models.Vacation.History historyLastYear = GetVacationHistory(member.UserID, timestamp.Year - 1, 12, 31, false);
                    Models.Vacation.History historyThisYear = GetVacationHistory(member.UserID, timestamp.Year, timestamp.Month, timestamp.Day);
                    Models.Vacation.History historyNextYear = GetVacationHistory(member.UserID, timestamp.Year + 1, 1, 1);*/

                    if (historyThisYear == null || historyNextYear == null)
                    {
                        strErrorMessage = string.Format("{0}님의 휴가이력을 얻어올 수 없습니다.", member.Name);
                        return null;
                    }

                    List<Models.Vacation.History> histories = new List<Models.Vacation.History>();
                    histories.Add(historyLastYear);
                    histories.Add(historyThisYear);
                    histories.Add(historyNextYear);

                    float fRemainDays = historyThisYear.TotalDays - historyThisYear.UsedDays;

                    if (fRemainDays + 0.1f < 0)
                        historyNextYear.SetMinusDays(-fRemainDays);

                    dicMemberHistory[user.ID] = histories;
                }
            }

            return users;
        }

        // Key : User ID
        private List<Models.Account.ApplicationUser> GetTeamMembers(RegularTeam team, Dictionary<int, List<Models.Vacation.History>> dicMemberHistory, DateTime timestamp, out string strErrorMessage)
        {
            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.TeamID] = team.ID;

            ArrayList arrResult = m_dataManager.GetSelectManager().SelectCompanyMemberJobLevelRegularTeam(dicConditions, null, null, out strErrorMessage);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<Models.Account.ApplicationUser> users = new List<Models.Account.ApplicationUser>();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                if (arrResult[i] is CompanyMember &&
                    arrResult[i + 1] is JobLevel &&
                    arrResult[i + 2] is RegularTeam)
                {
                    CompanyMember member = (CompanyMember)arrResult[i];
                    JobLevel level = (JobLevel)arrResult[i + 1];
                    RegularTeam _team = (RegularTeam)arrResult[i + 2];

                    Models.Account.ApplicationUser user = new Models.Account.ApplicationUser();

                    user.ID = member.ID;
                    user.IsAdmin = member.IsAdmin;
                    user.IsTeamLeader = member.IsTeamLeader;
                    user.IsTopManager = Models.Account.ApplicationUser.CheckTopManager(member, team, level.LevelName);
                    user.Level = level.LevelName;
                    user.Name = member.Name;
                    user.PhoneNumber = member.PhoneNumber;
                    user.StartYear = member.StartDate.Year;
                    user.StartMonth = member.StartDate.Month;
                    user.TeamName = team.Name;
                    user.TeamID = team.ID;
                    user.UserID = member.UserID;

                    users.Add(user);

                    Models.Vacation.History historyLastYear = GetVacationHistory(member.UserID, timestamp.Year - 1, 12, 31, false);
                    Models.Vacation.History historyThisYear = GetVacationHistory(member.UserID, timestamp.Year, timestamp.Month, timestamp.Day);
                    Models.Vacation.History historyNextYear = GetVacationHistory(member.UserID, timestamp.Year + 1, 1, 1);

                    if (historyThisYear == null || historyNextYear == null)
                    {
                        strErrorMessage = string.Format("{0}님의 휴가이력을 얻어올 수 없습니다.", member.Name);
                        return null;
                    }

                    List<Models.Vacation.History> histories = new List<Models.Vacation.History>();
                    histories.Add(historyLastYear);
                    histories.Add(historyThisYear);
                    histories.Add(historyNextYear);

                    float fRemainDays = historyThisYear.TotalDays - historyThisYear.UsedDays;

                    if (fRemainDays + 0.1f < 0)
                        historyNextYear.SetMinusDays(-fRemainDays);

                    dicMemberHistory[user.ID] = histories;
                }
            }

            return users;
        }

        // dicMemberHistoris.Key : ApplicationUser ID
        private MemberVacationHistory GetMemberVacationHistory(MemberTeam rootTeam, Dictionary<int, List<Models.Vacation.History>> dicMemberHistories, string strMessage, DateTime dtNow)
        {
            MemberVacationHistory history = new MemberVacationHistory();

            if (rootTeam == null || dicMemberHistories == null)
            {
                history.Success = false;
                history.Message = strMessage;
            }
            else
            {
                history.Success = true;
                history.Message = strMessage;
                history.RootTeam = rootTeam;

                SetReservationMonth(rootTeam);

                List<int> memberIDs = new List<int>();

                foreach (KeyValuePair<int, List<Models.Vacation.History>> pair in dicMemberHistories)
                {
                    if (pair.Value.Count != 3)
                        continue;

                    for (int i=0;i<3;i++)
                    {
                        Models.Vacation.History _history = pair.Value[i];

                        if (_history == null)
                            continue;

                        if (i == 0)
                            history.AddUserHistoryLastYear(pair.Key, _history);
                        else if (i == 1)
                            history.AddUserHistoryThisYear(pair.Key, _history);
                        else
                            history.AddUserHistoryNextYear(pair.Key, _history);
                    }
                    /*foreach (Models.Vacation.History _history in pair.Value)
                    {
                        if (_history.Year == dtNow.Year)
                            history.AddUserHistory1(pair.Key, _history);
                        else
                            history.AddUserHistory2(pair.Key, _history);
                    }*/

                    memberIDs.Add(pair.Key);
                }

                string strErrorMessage;
                int minYear = m_dataManager.GetSelectManager().GetMinimumHistoryYear(memberIDs, out strErrorMessage);

                if (minYear == 0)
                {
                    if (strErrorMessage != null)
                    {
                        history.Success = false;
                        history.Message = strErrorMessage;
                    }
                }
                else
                {
                    history.MinimumYear = minYear;
                }
            }

            return history;
        }

        private void SetReservationMonth(MemberTeam team)
        {
            if (team == null)
                return;

            foreach (Models.Account.ApplicationUser user in team.Members)
            {
                user.ReservationMonth = this.ReservationMonth;
            }

            foreach (MemberTeam childTeam in team.ChildTeams)
            {
                SetReservationMonth(childTeam);
            }
        }

        public ResponseVacationList GetVacationList(string strUserID, int year)
        {
            bool isNullable;

            string strAdditionalCondition = string.Format("{0}.{1} >= {2} and {3}.{4} = '{5}'",
                Request.GetTableName(),
                Request.GetFieldName(Request.Fields.Year, out isNullable),
                year - 1,
                CompanyMember.GetTableName(),
                CompanyMember.GetFieldName(CompanyMember.Fields.UserID, out isNullable),
                strUserID);

            string strErrorMessage;
            ArrayList arrDatas = m_dataManager.GetSelectManager().JoinCompanyMemberRequest(strAdditionalCondition, out strErrorMessage);

            if (arrDatas == null)
                return new ResponseVacationList(false, strErrorMessage);

            int nDataCount = arrDatas.Count;

            if (nDataCount < 2)
                return new ResponseVacationList(true, "");

            Dictionary<int, Request> dicRequests = new Dictionary<int, Request>();
            // 결재 승인권자가 1명이 아닐수도 있다.
            Dictionary<int, List<Response>> dicResponses = new Dictionary<int, List<Response>>();
            List<int> requestIDs = new List<int>();

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                Request request = (Request)arrDatas[i + 1];

                if (request.Response == Response.ResponseType.Cancel ||
                    request.Response == Response.ResponseType.Deny ||
                    request.Response == Response.ResponseType.None ||
                    request.Response == Response.ResponseType.Timeout)
                    continue;

                requestIDs.Add(request.ID);

                dicRequests[request.ID] = request;
                dicResponses[request.ID] = new List<Response>();
            }

            if (dicRequests.Count == 0)
                return new ResponseVacationList(true, "");

            if (GetResponses(requestIDs, dicResponses, out strErrorMessage) == false)
                return new ResponseVacationList(false, strErrorMessage);

            Dictionary<int, ApplicationUser> dicManagers = new Dictionary<int, ApplicationUser>();
            ResponseVacationList result = new ResponseVacationList(true, "");

            foreach (KeyValuePair<int, Request> pair in dicRequests)
            {
                List<Response> responseList;

                if (dicResponses.TryGetValue(pair.Key, out responseList) == false)
                    continue;

                VacationInfo vacation = GetVacationInfo(pair.Value, responseList, dicManagers);

                if (vacation != null)
                    result.Vacations.Add(vacation);
            }

            return result;
        }

        private VacationInfo GetVacationInfo(Request request, List<Response> responses, Dictionary<int, ApplicationUser> dicManagers)
        {
            if (request.Days.Count == 0)
                return null;

            VacationInfo vacation = new VacationInfo();
            Response response = null;

            if (responses.Count > 0)
            {
                responses.Sort();
                response = responses[responses.Count - 1];

                if (response.Result == Response.ResponseType.Permit)
                    vacation.ConfirmTime = response.ResponseTime;

                ApplicationUser manager;

                if (dicManagers.TryGetValue(response.ManagerID, out manager))
                    vacation.LastManager = manager;
                else
                {
                    Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
                    dicConditions[CompanyMember.Fields.ID] = response.ManagerID;

                    string strErrorMessage = null;
                    ArrayList arrDatas = m_dataManager.GetSelectManager().SelectCompanyMemberJobLevelRegularTeam(dicConditions, null, null, out strErrorMessage);

                    if (arrDatas != null &&
                        arrDatas.Count == 3 &&
                        arrDatas[0] is CompanyMember &&
                        arrDatas[1] is JobLevel &&
                        arrDatas[2] is RegularTeam)
                    {
                        CompanyMember member = (CompanyMember)arrDatas[0];
                        JobLevel level = (JobLevel)arrDatas[1];
                        RegularTeam team = (RegularTeam)arrDatas[2];

                        manager = Models.Account.ApplicationUser.MakeUser(member, level, team);

                        if (manager != null)
                        {
                            vacation.LastManager = manager;
                            dicManagers[response.ManagerID] = manager;
                        }
                    }
                }
            }

            float fTotalDays;
            string strPeriod;
            Dictionary<int, float> dicDays = RequestManager.GetYearDays(request.Days, out fTotalDays, out strPeriod);

            vacation.Days.AddRange(request.Days);
            vacation.DaysDescription = GetDaysDescription(request.Days.Count, fTotalDays, strPeriod);
            vacation.RequestID = request.ID;
            vacation.Status = response == null ? (int)VacationInfo.StatusType.Wait : (int)vacation.ToStatus(response.Result);

            if (vacation.Status == (int)VacationInfo.StatusType.None || vacation.Status == (int)VacationInfo.StatusType.Finish)
                return null;

            return vacation;
        }

        public static string GetDaysDescription(int nDayCount, float fTotalDays, string strPeriod)
        {
            if (nDayCount <= 1)
                return strPeriod;

            if ((int)(fTotalDays + 0.51f) > (int)fTotalDays)
                return string.Format("{0}(총 {1:F1}일)", strPeriod, fTotalDays);

            return string.Format("{0}(총 {1:F0}일)", strPeriod, fTotalDays);
        }

        private bool GetResponses(List<int> requestIDs, Dictionary<int, List<Response>> dicResponse, out string strErrorMessage)
        {
            List<Response> responses = m_dataManager.GetSelectManager().SelectResponse(requestIDs, out strErrorMessage);

            if (strErrorMessage != null)
                return false;

            List<Response> responseList;

            foreach (Response response in responses)
            {
                if (dicResponse.TryGetValue(response.RequestID, out responseList) == false)
                    return false;

                responseList.Add(response);
            }

            return true;
        }
    }
}
