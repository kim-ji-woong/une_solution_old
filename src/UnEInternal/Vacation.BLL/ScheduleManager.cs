using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Vacation.BLL
{
    using IDAL;
    using Model;
    using Models.Account;

    public class ScheduleManager
    {
        public enum UsingTypes { Half = 0, Quater };

        private static ScheduleManager m_instance = null;
        private static string m_strSystemMail = "", m_strNoticeMail = "", m_strSystemCode = "", m_strURL = "";

        private static string m_strPrevEmail = "", m_strPrevSubject = "", m_strPrevMessage = "";
        private static DateTime m_prevSendTime = new DateTime();

        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;
        private int m_nPrevYear = 0, m_nPrevMonth = 0, m_nPrevDay = 0;
        private bool m_closeThread = true;

        private static UsingTypes m_usingType = UsingTypes.Half;

        public static string SiteURL
        {
            get { return m_strURL; }
        }

        public static UsingTypes UsingType
        {
            get { return m_usingType; }
        }

        public static ScheduleManager Instance
        {
            get { return m_instance; }
        }

        public static void InitInstance(IDataManager dataManager, ProcessManager processManager)
        {
            if (m_instance == null)
                m_instance = new ScheduleManager(dataManager, processManager);
            else
                m_instance.SetDataManager(dataManager, processManager);
        }

        public static void SetSystemInfo(string strSystemMail, string strNoticeMail, string strSystemCode, string strURL, string strUsingType)
        {
            m_strSystemMail = strSystemMail;
            m_strNoticeMail = strNoticeMail;
            m_strSystemCode = strSystemCode;
            m_strURL = strURL;

            if (strUsingType != null)
            {
                if (strUsingType.ToLower() == "half")
                    m_usingType = UsingTypes.Half;
                else if (strUsingType.ToLower() == "quater")
                    m_usingType = UsingTypes.Quater;
            }
        }

        private ScheduleManager(IDataManager dataManager, ProcessManager processManager)
        {
            SetDataManager(dataManager, processManager);

            Thread t = new Thread(new ThreadStart(SchedulingThread));
            t.Start();
        }

        public void SetDataManager(IDataManager dataManager, ProcessManager processManager)
        {
            m_dataManager = dataManager;
            m_processManager = processManager;
        }

        private void SchedulingThread()
        {
            m_closeThread = false;

            while (m_closeThread == false)
            {
                DateTime dtNow = DateTime.Now;
                
                if (dtNow.Year != m_nPrevYear || dtNow.Month != m_nPrevMonth || dtNow.Day != m_nPrevDay)
                {
                    if (dtNow.Year != m_nPrevYear)
                    {
                        CheckNewYear(dtNow);
                    }

                    if (CheckSchedule(dtNow))
                    {
                        m_nPrevYear = dtNow.Year;
                        m_nPrevMonth = dtNow.Month;
                        m_nPrevDay = dtNow.Day;
                    }
                }

                // 1분에 한번씩 검사
                Thread.Sleep(60000);
            }
        }

        private bool CheckSchedule(DateTime dtNow)
        {
            if (CheckVacationNotice(dtNow) == false)
                return false;

            if (CheckTodayVacationNotice(dtNow) == false)
                return false;

            if (CheckNextVacation(dtNow) == false)
                return false;

            // 마감시한까지 결재가 이루어지지 않은 경우
            if (CheckIgnoreRequest(dtNow) == false)
                return false;

            // 마감시한은 되지 않았으나 휴가개시일까지 결재가 이루어지지 않은 경우
            if (CheckTimeoutRequest(dtNow) == false)
                return false;

            // 마감시한까지 결재가 이루어지지 않은 경우(특별휴가)
            if (CheckIgnoreSVRequest(dtNow) == false)
                return false;

            return true;
        }

        // 일정시간동안 결재하지 않으면 Timeout 처리한다.
        private bool CheckIgnoreSVRequest(DateTime dtNow)
        {
            int nRequestTimeoutDay = m_processManager.GetVacationManager().RequestTimeoutDay;
            DateTime limitDay = dtNow.AddDays(-nRequestTimeoutDay);
            string strTime = string.Format("{0}-{1:00}-{2:00} 00:00:00", limitDay.Year, limitDay.Month, limitDay.Day);

            bool isNullable;
            string strCondition = string.Format("{0} < '{1}' and ({2} = {3} or {2} = {4})",
                SpecialVacationRequest.GetFieldName(SpecialVacationRequest.Fields.RequestTime, out isNullable),
                strTime,
                SpecialVacationRequest.GetFieldName(SpecialVacationRequest.Fields.Response, out isNullable),
                (int)Response.ResponseType.Processing,
                (int)Response.ResponseType.None);

            string strErrorMessage;
            List<SpecialVacationRequest> requests = m_dataManager.GetSelectManager().SelectSpecialVacationRequests(null, strCondition, out strErrorMessage);

            if (requests == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckIgnoreSVRequest Request Error : " + strErrorMessage);
                return false;
            }

            Dictionary<int, int> dicMemberIDs = new Dictionary<int, int>();
            List<int> requestIDs = new List<int>();

            foreach (SpecialVacationRequest request in requests)
            {
                requestIDs.Add(request.ID);
                dicMemberIDs[request.RequestManagerID] = request.RequestManagerID;
            }

            if (requestIDs.Count == 0)
                return true;

            Dictionary<int, CompanyMember> requestMembers = GetMembers(dicMemberIDs.Keys);

            if (requestMembers == null)
                return false;

            List<SpecialVacationResponse> responses = m_dataManager.GetSelectManager().SelectSpecialVacationResponse(requestIDs, out strErrorMessage);

            if (responses == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckIgnoreSVRequest Response Error : " + strErrorMessage);
                return false;
            }

            Dictionary<int, ApplicationUser> dicManagers = new Dictionary<int, ApplicationUser>();
            Dictionary<SpecialVacationRequest, List<SpecialVacationResponse>> dicRequestResponses = GetSVRequestResponses(requests, responses);

            foreach (KeyValuePair<SpecialVacationRequest, List<SpecialVacationResponse>> pair in dicRequestResponses)
            {
                if (CheckIgnoreSVRequest(pair.Key, pair.Value, requestMembers, dicManagers, dtNow) == false)
                    return false;
            }

            return true;
        }

        private bool CheckIgnoreSVRequest(SpecialVacationRequest request, List<SpecialVacationResponse> responses, Dictionary<int, CompanyMember> dicRequestMembers, Dictionary<int, ApplicationUser> dicManagers, DateTime dtNow)
        {
            if (responses == null)
            {
                if (TimeoutSVRequest(request, dicRequestMembers) == false)
                    return false;
            }

            string strErrorMessage;
            ApplicationUser manager, permitTopManager = null;
            bool topManagerIgnore = false;

            foreach (SpecialVacationResponse response in responses)
            {
                if (response.Result == Response.ResponseType.Processing ||
                    response.Result == Response.ResponseType.None)
                {
                    if (dicManagers.TryGetValue(response.ManagerID, out manager) == false)
                    {
                        manager = RequestManager.GetApplicationUser(m_dataManager, response.ManagerID, null, out strErrorMessage);

                        if (manager == null || strErrorMessage != null)
                        {
                            System.Diagnostics.Trace.WriteLine("CheckIgnoreSVRequest Error : " + strErrorMessage);
                            return false;
                        }

                        dicManagers[response.ManagerID] = manager;
                    }

                    if (manager.IsTopManager)
                    {
                        topManagerIgnore = true;
                    }
                    else
                    {
                        if (TimeoutSVResponse(response, Response.ResponseType.Timeout, dtNow) == false)
                            return false;

                        return TimeoutSVRequest(request, dicRequestMembers);
                    }
                }
                else if (response.Result == Response.ResponseType.Permit)
                {
                    if (dicManagers.TryGetValue(response.ManagerID, out manager) == false)
                    {
                        manager = RequestManager.GetApplicationUser(m_dataManager, response.ManagerID, null, out strErrorMessage);

                        if (manager == null || strErrorMessage != null)
                        {
                            System.Diagnostics.Trace.WriteLine("CheckSVIgnoreRequest Error : " + strErrorMessage);
                            return false;
                        }

                        dicManagers[response.ManagerID] = manager;
                    }

                    if (manager.IsTopManager)
                    {
                        permitTopManager = manager;
                    }
                }
            }

            if (topManagerIgnore)
            {
                if (permitTopManager != null)
                    return TimeoutSVPermitRequest(request, responses, dicRequestMembers, permitTopManager, dtNow);
                else
                    return TimeoutSVRequest(request, responses, dicRequestMembers, dtNow);
            }

            return true;
        }

        // Request에 해당하는 Response들을 시간순으로 정렬하여 리턴한다.
        private Dictionary<SpecialVacationRequest, List<SpecialVacationResponse>> GetSVRequestResponses(List<SpecialVacationRequest> requests, List<SpecialVacationResponse> responses)
        {
            List<SpecialVacationResponse> responseList;
            Dictionary<int, List<SpecialVacationResponse>> dicResponses = new Dictionary<int, List<SpecialVacationResponse>>();

            foreach (SpecialVacationResponse response in responses)
            {
                if (dicResponses.TryGetValue(response.RequestID, out responseList) == false)
                {
                    responseList = new List<SpecialVacationResponse>();
                    dicResponses[response.RequestID] = responseList;
                }

                responseList.Add(response);
            }

            Dictionary<SpecialVacationRequest, List<SpecialVacationResponse>> dicRequestResponse = new Dictionary<SpecialVacationRequest, List<SpecialVacationResponse>>();

            foreach (SpecialVacationRequest request in requests)
            {
                if (dicResponses.TryGetValue(request.ID, out responseList))
                {
                    dicRequestResponse[request] = responseList;
                }
                else
                {
                    dicRequestResponse[request] = null;
                }
            }

            return dicRequestResponse;
        }

        private bool TimeoutSVRequest(SpecialVacationRequest request, List<SpecialVacationResponse> responses, Dictionary<int, CompanyMember> dicRequestMembers, DateTime dtNow)
        {
            foreach (SpecialVacationResponse response in responses)
            {
                if (response.Result == Response.ResponseType.Processing ||
                    response.Result == Response.ResponseType.None)
                {
                    if (TimeoutSVResponse(response, Response.ResponseType.Timeout, dtNow) == false)
                        return false;
                }
            }

            return TimeoutSVRequest(request, dicRequestMembers);
        }

        private bool TimeoutSVResponse(SpecialVacationResponse response, Response.ResponseType result, DateTime dtNow)
        {
            response.Result = result;
            response.ResponseTime = dtNow;

            string strErrorMessage;

            if (m_dataManager.GetUpdateManager().UpdateSpecialVacationResponse(response, out strErrorMessage) == false)
            {
                System.Diagnostics.Trace.WriteLine("TimeoutSVResponse Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private bool TimeoutSVPermitRequest(SpecialVacationRequest request, List<SpecialVacationResponse> responses, Dictionary<int, CompanyMember> dicRequestMembers, ApplicationUser lastManager, DateTime dtNow)
        {
            string strErrorMessage;

            foreach (SpecialVacationResponse response in responses)
            {
                if (response.Result == Response.ResponseType.Processing ||
                    response.Result == Response.ResponseType.None)
                {
                    if (TimeoutSVResponse(response, Response.ResponseType.Timeout, dtNow) == false)
                        return false;
                }
            }

            return RequestManager.PostFinalSpecialVacationRequest(m_dataManager, request, lastManager, true, out strErrorMessage);
        }

        private bool TimeoutSVRequest(SpecialVacationRequest request, Dictionary<int, CompanyMember> dicRequestMembers)
        {
            string strErrorMessage;
            request.Response = Response.ResponseType.Timeout;

            // 취소처리
            if (m_dataManager.GetUpdateManager().UpdateSpecialVacationRequest(request, out strErrorMessage) == false)
            {
                System.Diagnostics.Trace.WriteLine("TimeoutSVRequest Error : " + strErrorMessage);
                return false;
            }

            CompanyMember member;

            // 취소 통보(특별휴가를 요청한 팀장에게 메시지를 보낸다.)
            if (dicRequestMembers.TryGetValue(request.RequestManagerID, out member))
            {
                KakaoManager.SendSVMessage(KakaoManager.MessageType.Timeout, member.PhoneNumber, request.RequestTime, request.Days);
            }

            return true;
        }

        // 마감시한은 되지 않았으나 휴가개시일까지 결재가 이루어지지 않은 경우
        private bool CheckTimeoutRequest(DateTime dtNow)
        {
            bool isNullable;
            string strCondition = string.Format("{0} = {1} or {0} = {2}",
                Request.GetFieldName(Request.Fields.Response, out isNullable),
                (int)Response.ResponseType.Processing,
                (int)Response.ResponseType.None);

            string strErrorMessage;
            List<Request> requests = m_dataManager.GetSelectManager().SelectRequests(null, strCondition, out strErrorMessage);

            if (requests == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckTimeoutRequest Request Error : " + strErrorMessage);
                return false;
            }

            Dictionary<int, int> dicMemberIDs = new Dictionary<int, int>();
            List<int> requestIDs = new List<int>();

            int today = dtNow.Year * 10000 + dtNow.Month * 100 + dtNow.Day;

            foreach (Request request in requests)
            {
                Date firstDate = GetFirstDate(request);

                if (firstDate == null)
                    continue;

                int dateNumber = firstDate.Year * 10000 + firstDate.Month * 100 + firstDate.Day;

                if (dateNumber > today)
                    continue;
                else if (dateNumber == today)
                {
                    if (Date.BeforeNoon(firstDate.DateType) && dtNow.Hour >= 12)
                    {
                        requestIDs.Add(request.ID);
                        dicMemberIDs[request.MemberID] = request.MemberID;
                    }

                    continue;
                }
                
                requestIDs.Add(request.ID);
                dicMemberIDs[request.MemberID] = request.MemberID;
            }

            if (requestIDs.Count == 0)
                return true;

            Dictionary<int, CompanyMember> requestMembers = GetMembers(dicMemberIDs.Keys);

            if (requestMembers == null)
                return false;

            List<Response> responses = m_dataManager.GetSelectManager().SelectResponse(requestIDs, out strErrorMessage);

            if (responses == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckTimeoutRequest Response Error : " + strErrorMessage);
                return false;
            }

            Dictionary<int, ApplicationUser> dicManagers = new Dictionary<int, ApplicationUser>();
            Dictionary<Request, List<Response>> dicRequestResponses = GetRequestResponses(requests, responses);

            foreach (KeyValuePair<Request, List<Response>> pair in dicRequestResponses)
            {
                if (pair.Value == null)
                    continue;

                if (TimeoutRequest(pair.Key, pair.Value, requestMembers, dtNow) == false)
                    return false;
            }

            return true;
        }

        private Date GetFirstDate(Request request)
        {
            foreach (Date date in request.Days)
            {
                return date;
            }

            return null;
        }

        // 일정시간동안 결재하지 않으면 Timeout 처리한다.
        private bool CheckIgnoreRequest(DateTime dtNow)
        {
            int nRequestTimeoutDay = m_processManager.GetVacationManager().RequestTimeoutDay;
            DateTime limitDay = dtNow.AddDays(-nRequestTimeoutDay);
            string strTime = string.Format("{0}-{1:00}-{2:00} 00:00:00", limitDay.Year, limitDay.Month, limitDay.Day);

            bool isNullable;
            string strCondition = string.Format("{0} < '{1}' and ({2} = {3} or {2} = {4})",
                Request.GetFieldName(Request.Fields.RequestTime, out isNullable),
                strTime,
                Request.GetFieldName(Request.Fields.Response, out isNullable),
                (int)Response.ResponseType.Processing,
                (int)Response.ResponseType.None);

            string strErrorMessage;
            List<Request> requests = m_dataManager.GetSelectManager().SelectRequests(null, strCondition, out strErrorMessage);

            if (requests == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckIgnoreRequest Request Error : " + strErrorMessage);
                return false;
            }

            Dictionary<int, int> dicMemberIDs = new Dictionary<int, int>();
            List<int> requestIDs = new List<int>();

            foreach (Request request in requests)
            {
                requestIDs.Add(request.ID);
                dicMemberIDs[request.MemberID] = request.MemberID;
            }

            if (requestIDs.Count == 0)
                return true;

            Dictionary<int, CompanyMember> requestMembers = GetMembers(dicMemberIDs.Keys);

            if (requestMembers == null)
                return false;

            List<Response> responses = m_dataManager.GetSelectManager().SelectResponse(requestIDs, out strErrorMessage);

            if (responses == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckIgnoreRequest Response Error : " + strErrorMessage);
                return false;
            }

            Dictionary<int, ApplicationUser> dicManagers = new Dictionary<int, ApplicationUser>();
            Dictionary<Request, List<Response>> dicRequestResponses = GetRequestResponses(requests, responses);

            foreach (KeyValuePair<Request, List<Response>> pair in dicRequestResponses)
            {
                if (CheckIgnoreRequest(pair.Key, pair.Value, requestMembers, dicManagers, dtNow) == false)
                    return false;
            }

            return true;
        }

        private Dictionary<int, CompanyMember> GetMembers(IEnumerable<int> memberIDs)
        {
            Dictionary<int, CompanyMember> dicMembers = new Dictionary<int, CompanyMember>();

            string strIDs = "";

            foreach (int id in memberIDs)
            {
                if (strIDs.Length == 0)
                    strIDs = id.ToString();
                else
                    strIDs += ", " + id.ToString();
            }

            if (strIDs.Length == 0)
                return dicMembers;

            string strConditions = "ID in (" + strIDs + ")";
            string strErrorMessage;
            List<CompanyMember> members = m_dataManager.GetSelectManager().SelectCompanyMembers(null, strConditions, out strErrorMessage);

            if (members == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("GetMembers Error : " + strErrorMessage);
                return null;
            }

            foreach (CompanyMember member in members)
            {
                dicMembers[member.ID] = member;
            }

            return dicMembers;
        }

        private bool CheckIgnoreRequest(Request request, List<Response> responses, Dictionary<int, CompanyMember> dicRequestMembers, Dictionary<int, ApplicationUser> dicManagers, DateTime dtNow)
        {
            if (responses == null)
            {
                if (TimeoutRequest(request, dicRequestMembers) == false)
                    return false;
            }

            string strErrorMessage;
            ApplicationUser manager, permitTopManager = null;
            bool topManagerIgnore = false;
            
            foreach (Response response in responses)
            {
                if (response.Result == Response.ResponseType.Processing ||
                    response.Result == Response.ResponseType.None)
                {
                    if (dicManagers.TryGetValue(response.ManagerID, out manager) == false)
                    {
                        manager = RequestManager.GetApplicationUser(m_dataManager, response.ManagerID, null, out strErrorMessage);

                        if (manager == null || strErrorMessage != null)
                        {
                            System.Diagnostics.Trace.WriteLine("CheckIgnoreRequest Error : " + strErrorMessage);
                            return false;
                        }

                        dicManagers[response.ManagerID] = manager;
                    }

                    if (manager.IsTopManager)
                    {
                        topManagerIgnore = true;
                    }
                    else
                    {
                        if (TimeoutResponse(response, Response.ResponseType.Timeout, dtNow) == false)
                            return false;

                        return TimeoutRequest(request, dicRequestMembers);
                    }
                }
                else if (response.Result == Response.ResponseType.Permit)
                {
                    if (dicManagers.TryGetValue(response.ManagerID, out manager) == false)
                    {
                        manager = RequestManager.GetApplicationUser(m_dataManager, response.ManagerID, null, out strErrorMessage);

                        if (manager == null || strErrorMessage != null)
                        {
                            System.Diagnostics.Trace.WriteLine("CheckIgnoreRequest Error : " + strErrorMessage);
                            return false;
                        }

                        dicManagers[response.ManagerID] = manager;
                    }

                    if (manager.IsTopManager)
                    {
                        permitTopManager = manager;
                    }
                }
            }

            if (topManagerIgnore)
            {
                if (permitTopManager != null)
                    return TimeoutPermitRequest(request, responses, dicRequestMembers, permitTopManager, dtNow);
                else
                    return TimeoutRequest(request, responses, dicRequestMembers, dtNow);
            }

            return true;
        }

        private bool TimeoutResponse(Response response, Response.ResponseType result, DateTime dtNow)
        {
            response.Result = result;
            response.ResponseTime = dtNow;

            string strErrorMessage;

            if (m_dataManager.GetUpdateManager().UpdateResponse(response, out strErrorMessage) == false)
            {
                System.Diagnostics.Trace.WriteLine("TimeoutResponse Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private bool TimeoutPermitRequest(Request request, List<Response> responses, Dictionary<int, CompanyMember> dicRequestMembers, ApplicationUser lastManager, DateTime dtNow)
        {
            string strErrorMessage;
            
            foreach (Response response in responses)
            {
                if (response.Result == Response.ResponseType.Processing ||
                    response.Result == Response.ResponseType.None)
                {
                    if (TimeoutResponse(response, Response.ResponseType.Timeout, dtNow) == false)
                        return false;
                }
            }

            return RequestManager.PostFinalRequest(m_dataManager, request, lastManager, true, out strErrorMessage);
        }

        private bool TimeoutRequest(Request request, List<Response> responses, Dictionary<int, CompanyMember> dicRequestMembers, DateTime dtNow)
        {
            foreach (Response response in responses)
            {
                if (response.Result == Response.ResponseType.Processing ||
                    response.Result == Response.ResponseType.None)
                {
                    if (TimeoutResponse(response, Response.ResponseType.Timeout, dtNow) == false)
                        return false;
                }
            }

            return TimeoutRequest(request, dicRequestMembers);
        }

        private bool TimeoutRequest(Request request, Dictionary<int, CompanyMember> dicRequestMembers)
        {
            string strErrorMessage;
            request.Response = Response.ResponseType.Timeout;

            // 취소처리
            if (m_dataManager.GetUpdateManager().UpdateRequest(request, out strErrorMessage) == false)
            {
                System.Diagnostics.Trace.WriteLine("TimeoutRequest Error : " + strErrorMessage);
                return false;
            }

            CompanyMember member;

            // 취소 통보
            if (dicRequestMembers.TryGetValue(request.MemberID, out member))
            {
                float fTotalDays;
                string strPeriod;
                RequestManager.GetYearDays(request.Days, out fTotalDays, out strPeriod);
                KakaoManager.SendMessage(KakaoManager.MessageType.Timeout, member.PhoneNumber, request.RequestTime, fTotalDays, strPeriod);
            }

            return true;
        }

        // Request에 해당하는 Response들을 시간순으로 정렬하여 리턴한다.
        private Dictionary<Request, List<Response>> GetRequestResponses(List<Request> requests, List<Response> responses)
        {
            List<Response> responseList;
            Dictionary<int, List<Response>> dicResponses = new Dictionary<int, List<Response>>();

            foreach (Response response in responses)
            {
                if (dicResponses.TryGetValue(response.RequestID, out responseList) == false)
                {
                    responseList = new List<Response>();
                    dicResponses[response.RequestID] = responseList;
                }

                responseList.Add(response);
            }

            Dictionary<Request, List<Response>> dicRequestResponse = new Dictionary<Request, List<Response>>();

            foreach (Request request in requests)
            {
                if (dicResponses.TryGetValue(request.ID, out responseList))
                {
                    dicRequestResponse[request] = responseList;
                }
                else
                {
                    dicRequestResponse[request] = null;
                }
            }

            return dicRequestResponse;
        }

        private bool CheckNewYear(DateTime dtNow)
        {
            if (m_dataManager == null)
                return false;

            string strErrorMessage;
            List<CompanyMember> members = m_dataManager.GetSelectManager().SelectCompanyMembers(null, out strErrorMessage);

            if (members == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckNewYear Error : " + strErrorMessage);
                return false;
            }

            foreach (CompanyMember member in members)
            {
                History history = m_dataManager.GetSelectManager().SelectHistory(member.ID, dtNow.Year, out strErrorMessage);

                if (strErrorMessage != null)
                {
                    System.Diagnostics.Trace.WriteLine("CheckNewYear Error : " + strErrorMessage);
                    return false;
                }
                else if (history != null)
                    continue;

                /*float fUnusedDays;
                float fPrevYearDays = GetPrevYearDays(member, dtNow.Year - 1, out fUnusedDays);*/

                DateTime nextVacationDay;
                float day = m_processManager.GetVacationManager().GetVacationDay(member.StartDate, dtNow, out nextVacationDay/*, fUnusedDays*/) - m_processManager.GetVacationManager().GetMinusDay(member.UserID, dtNow.Year - 1);

                if (m_dataManager.GetCreateManager().CreateHistory(member.ID, dtNow.Year, day /*+ fPrevYearDays*/, 0, 0, new List<int>(), nextVacationDay) == null)
                    return false;
            }

            return true;
        }

        // 전년도에 초과한 휴가일수가 있는지 확인한다.
        private float GetPrevYearDays(CompanyMember member, int year, out float fUnusedDays)
        {
            fUnusedDays = 0;

            string strErrorMessage;
            History history = m_dataManager.GetSelectManager().SelectHistory(member.ID, year, out strErrorMessage);

            if (history == null)
                return 0;

            fUnusedDays = history.TotalDays - history.UsedDays;
            return fUnusedDays < 0 ? fUnusedDays : 0;
        }

        private bool CheckNextVacation(DateTime dtNow)
        {
            string strErrorMessage;

            // 1월 1일에는 작년의 History를 검사한다.
            int year = dtNow.Month == 1 && dtNow.Day == 1 ? dtNow.Year - 1 : dtNow.Year;

            Dictionary<History.Fields, object> dicConditions = new Dictionary<History.Fields, object>();
            dicConditions[History.Fields.Year] = year;

            ArrayList arrDatas = m_dataManager.GetSelectManager().SelectCompanyMemberHistories(null, dicConditions, out strErrorMessage);

            if (arrDatas == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckNextVacation : " + strErrorMessage);
                return false;
            }

            int nDataCount = arrDatas.Count;

            for (int i=0;i<nDataCount-1;i+=2)
            {
                if (arrDatas[i] is CompanyMember &&
                    arrDatas[i + 1] is History)
                {
                    CompanyMember member = (CompanyMember)arrDatas[i];
                    History history = (History)arrDatas[i + 1];

                    if (history.NextVacationDay <= dtNow)
                    {
                        UpdateNextVacation(member, history, dtNow);
                    }
                }
            }

            return true;
        }

        private void UpdateNextVacation(CompanyMember member, History history, DateTime dtNow)
        {
            if (history.NextVacationDay.Month == 1 && history.NextVacationDay.Day == 1)
            {
                if (member.StartDate.Year + 2 <= history.NextVacationDay.Year)
                    UpdateYearVacation(member, history);
                else if (member.StartDate.Year + 1 == history.NextVacationDay.Year &&
                    member.StartDate.Month == 1 && member.StartDate.Day == 1)
                    UpdateYearVacation(member, history);
                else
                    UpdateMonthVacation(member, history, history.NextVacationDay);
            }
            else
                UpdateMonthVacation(member, history, history.NextVacationDay);
        }

        private void UpdateMonthVacation(CompanyMember member, History history, DateTime dtNow)
        {
            string strErrorMessage;
            DateTime nextVacationDay;
            float day = m_processManager.GetVacationManager().GetVacationDay(member.StartDate, dtNow, out nextVacationDay);

            if (dtNow.Month == 1 && dtNow.Year == 1)
            {
                float fUnusedDays;
                float prevYearDays = GetPrevYearDays(member, dtNow.Year - 1, out fUnusedDays);
                history = m_dataManager.GetSelectManager().SelectHistory(member.ID, dtNow.Year, out strErrorMessage);

                if (history == null)
                    return;

                history.TotalDays = day + prevYearDays;
            }
            else
            {
                history.TotalDays = history.TotalDays + day;
            }

            history.NextVacationDay = nextVacationDay;
            m_dataManager.GetUpdateManager().UpdateHistory(history, out strErrorMessage);
        }

        private void UpdateYearVacation(CompanyMember member, History history)
        {
            // CheckNewYear()에서 이미 처리되었다.
        }

        // 휴가알림 공지메일을 보낼것이 있는지 확인해서 보낸다.
        // 휴가개시일 (option)일전에 메일을 보낸다.
        private bool CheckVacationNotice(DateTime dtNow)
        {
            if (m_strSystemMail.Length == 0 || m_strNoticeMail.Length == 0 || m_strSystemCode.Length == 0)
                return false;

            if (m_dataManager == null)
                return false;

            string strErrorMessage;
            int nNoticeDays;

            if (ReadOptions(out nNoticeDays, out strErrorMessage) == false)
            {
                System.Diagnostics.Trace.WriteLine("CheckScheduleError : " + strErrorMessage);
                return false;
            }

            Dictionary<Request.Fields, object> dicConditions = new Dictionary<Request.Fields, object>();
            dicConditions[Request.Fields.Response] = (int)Response.ResponseType.Permit;
            dicConditions[Request.Fields.MailSendTime] = null;

            List<Request> requests = m_dataManager.GetSelectManager().SelectRequests(dicConditions, out strErrorMessage);

            if (requests == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckScheduleError : " + strErrorMessage);
                return false;
            }
            else
            {
                RegularTeam rootTeam = GetRootTeam();

                foreach (Request request in requests)
                {
                    string strDays;
                    DateTime? date = GetFirstVacationDay(request, out strDays);

                    if (date == null)
                        continue;

                    TimeSpan span = (DateTime)date - dtNow;

                    if (span.TotalDays <= nNoticeDays)
                    {
                        if (SendNoticeEmail(request, strDays, rootTeam))
                        {
                            request.MailSendTime = DateTime.Now;
                            m_dataManager.GetUpdateManager().UpdateRequest(request, out strErrorMessage);
                        }
                    }
                }
            }

            return true;
        }

        // 휴가알림 공지메일을 보낼것이 있는지 확인해서 보낸다.
        // 휴가개시일 당일에 메일을 보낸다.
        private bool CheckTodayVacationNotice(DateTime dtNow)
        {
            if (m_strSystemMail.Length == 0 || m_strNoticeMail.Length == 0 || m_strSystemCode.Length == 0)
                return false;

            if (m_dataManager == null)
                return false;

            string strErrorMessage;
            int nNoticeDays;

            if (ReadOptions(out nNoticeDays, out strErrorMessage) == false)
            {
                System.Diagnostics.Trace.WriteLine("CheckScheduleError : " + strErrorMessage);
                return false;
            }

            Dictionary<Request.Fields, object> dicConditions = new Dictionary<Request.Fields, object>();
            dicConditions[Request.Fields.Response] = (int)Response.ResponseType.Permit;

            string strAdditionalCondition = string.Format("Days like '{1}{2:00}%' and Year = {0} and MailSendTime < '{0:00}-{1:00}-{2:00} 00:00:00'", dtNow.Year, dtNow.Month, dtNow.Day);

            List<Request> requests = m_dataManager.GetSelectManager().SelectRequests(dicConditions, strAdditionalCondition, out strErrorMessage);

            if (requests == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckScheduleError : " + strErrorMessage);
                return false;
            }
            else
            {
                RegularTeam rootTeam = GetRootTeam();

                foreach (Request request in requests)
                {
                    string strDays;
                    DateTime? date = GetFirstVacationDay(request, out strDays);

                    if (date == null)
                        continue;

                    // 쿼리에서 조건을 걸었지만 한번더 확인한다.
                    if (((DateTime)date).Year == dtNow.Year && ((DateTime)date).Month == dtNow.Month && ((DateTime)date).Day == dtNow.Day)
                    {
                        if (SendNoticeEmail(request, strDays, rootTeam))
                        {
                            request.MailSendTime = dtNow;
                            m_dataManager.GetUpdateManager().UpdateRequest(request, out strErrorMessage);
                        }
                    }
                }
            }

            return true;
        }

        private RegularTeam GetRootTeam()
        {
            Dictionary<RegularTeam.Fields, object> dicConditions = new Dictionary<RegularTeam.Fields, object>();
            dicConditions[RegularTeam.Fields.ParentID] = null;

            string strErrorMessage;
            List<RegularTeam> teams = m_dataManager.GetSelectManager().SelectRegularTeams(dicConditions, out strErrorMessage);

            if (teams == null || teams.Count == 0)
                return null;

            return teams[0];
        }

        private bool SendNoticeEmail(Request request, string strDays, RegularTeam rootTeam)
        {
            string strErrorMessage;
            ApplicationUser user = RequestManager.GetApplicationUser(m_dataManager, request.MemberID, null, out strErrorMessage);

            if (user == null)
            {
                System.Diagnostics.Trace.WriteLine("SendNoticeEmail Error : " + strErrorMessage);
                return false;
            }

            string strName = user.Name;

            if (user.Level == "사원")
                strName += "님";
            else
                strName += " " + user.Level + "님";

            string strTeamName = rootTeam != null && user.TeamName != rootTeam.Name ? user.TeamName + " " : "";

            string strSubject = string.Format("[휴가공지] {0}{1} {2}", strTeamName, user.Name, user.Level);
            string strMessage = strTeamName + strName + "의 휴가사용을 알려드립니다.\r\n\r\n";
            strMessage += "일시 : " + strDays + "\r\n";
            
            if (request.RequestDescription != null && request.RequestDescription.Length > 0)
            {
                strMessage += "사유 : " + request.RequestDescription + "\r\n";
            }

            strMessage += "\r\n" + m_strURL;
            return SendEmail(m_strSystemMail, m_strSystemCode, m_strNoticeMail, strSubject, strMessage, "휴가공지", ref strErrorMessage);
        }

        public bool SendCancelEmail(Request request, string strDays)
        {
            string strErrorMessage;
            ApplicationUser user = RequestManager.GetApplicationUser(m_dataManager, request.MemberID, null, out strErrorMessage);

            if (user == null)
            {
                System.Diagnostics.Trace.WriteLine("SendCancelEmail Error : " + strErrorMessage);
                return false;
            }

            string strName = user.Name;

            if (user.Level == "사원")
                strName += "님";
            else
                strName += " " + user.Level + "님";

            string strSubject = string.Format("[휴가 취소공지] {0}{1} {2}", user.TeamName, user.Name, user.Level);
            string strMessage = user.TeamName + " " + strName + "의 휴가가 취소되었음을 알려드립니다.\r\n\r\n";
            strMessage += "일시 : " + strDays + "\r\n";

            strMessage += "\r\n" + m_strURL;

            return SendEmail(m_strSystemMail, m_strSystemCode, m_strNoticeMail, strSubject, strMessage, "휴가 취소공지", ref strErrorMessage);
        }

        public bool SendChangeEmail(Request request, string strChangedVacation)
        {
            string strErrorMessage;
            ApplicationUser user = RequestManager.GetApplicationUser(m_dataManager, request.MemberID, null, out strErrorMessage);

            if (user == null)
            {
                System.Diagnostics.Trace.WriteLine("SendChangeEmail Error : " + strErrorMessage);
                return false;
            }

            string strName = user.Name;

            if (user.Level == "사원")
                strName += "님";
            else
                strName += " " + user.Level + "님";

            string strSubject = string.Format("[휴가 변경공지] {0}{1} {2}", user.TeamName, user.Name, user.Level);
            string strMessage = strChangedVacation;
            strMessage += "\r\n" + m_strURL;

            return SendEmail(m_strSystemMail, m_strSystemCode, m_strNoticeMail, strSubject, strMessage, "휴가 변경공지", ref strErrorMessage);
        }

        // 휴가일로부터 3일 이내로 남았으면 공지메일을 보낸다.
        public bool CheckSchedule(Request request)
        {
            string strDays;
            DateTime? date = GetFirstVacationDay(request, out strDays);

            if (date == null)
                return false;

            string strErrorMessage;
            int nNoticeDays;

            if (ReadOptions(out nNoticeDays, out strErrorMessage) == false)
            {
                System.Diagnostics.Trace.WriteLine("CheckScheduleError : " + strErrorMessage);
                return false;
            }

            TimeSpan span = (DateTime)date - DateTime.Now;

            if (span.TotalDays <= nNoticeDays)
            {
                RegularTeam rootTeam = GetRootTeam();
                
                if (SendNoticeEmail(request, strDays, rootTeam))
                {
                    request.MailSendTime = DateTime.Now;
                    m_dataManager.GetUpdateManager().UpdateRequest(request, out strErrorMessage);
                }
            }

            return true;
        }

        private bool SendEmail(string strSystemMail, string strSystemCode, string strEmail, string strSubject, string strMessage, string strEmailTitle, ref string strErrorMessage)
        {
            // 같은 메일이 여러번 반복되어 발송되는 것 방지
            if (strEmail == m_strPrevEmail && strSubject == m_strPrevSubject && strMessage == m_strPrevMessage)
            {
                TimeSpan span = DateTime.Now - m_prevSendTime;

                if (span.TotalSeconds < 60)
                    return true;
            }

            m_strPrevEmail = strEmail;
            m_strPrevSubject = strSubject;
            m_strPrevMessage = strMessage;
            m_prevSendTime = DateTime.Now;

            try
            {
                // Credentials
                var credentials = new NetworkCredential(strSystemMail, strSystemCode);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(strSystemMail),
                    Subject = strSubject,
                    Body = strMessage
                };

                mail.To.Add(new MailAddress(strEmail));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                strErrorMessage = "Error in sending email: " + ex.Message;
                return false;
            }

            return true;
        }

        private DateTime? GetFirstVacationDay(Request request, out string strDays)
        {
            strDays = "";

            Date dateMin = null;
            float fDays = 0.0f;
            string strBeginDate = "", strEndDate = "";
            string strCurrentBeginDate = "", strCurrentDays = "";

            List<string> dayList = new List<string>();
            Date prevDate = null;

            foreach (Date date in request.Days)
            {
                if (dateMin == null)
                    dateMin = date;
                else if (dateMin.CompareTo(date) > 0)
                    dateMin = date;

                string strDate = string.Format("{0}월 {1}일", date.Month, date.Day);
                strDate += Date.GetDateTypeString(date.DateType);

                fDays += Date.GetDateCount(date.DateType);

                if (strBeginDate.Length == 0)
                {
                    strBeginDate = strDate;
                    strCurrentBeginDate = strDate;
                    strCurrentDays = strDate;
                }
                else
                {
                    if (IsContinuous(prevDate, date))
                    {
                        strCurrentDays = SetContinuousDays(strCurrentBeginDate, strDate);
                    }
                    else
                    {
                        if (strCurrentDays.Length > 0)
                            dayList.Add(strCurrentDays);

                        strCurrentDays = strDate;
                        strCurrentBeginDate = strDate;
                    }
                }

                prevDate = date;
                strEndDate = strDate;
            }

            if (strBeginDate == strEndDate)
            {
                strDays = strBeginDate;
            }
            else
            {
                if (strCurrentDays.Length > 0)
                    dayList.Add(strCurrentDays);

                foreach (string str in dayList)
                {
                    if (strDays.Length == 0)
                        strDays = str;
                    else
                        strDays += "," + str;
                }

                strDays += string.Format("({0:F1}일)", fDays);
                //strDays = string.Format("{0} ~ {1}({2:F1}일)", strBeginDate, strEndDate, fDays);
            }

            if (dateMin == null)
                return null;

            DateTime time = new DateTime(dateMin.Year, dateMin.Month, dateMin.Day);
            return (DateTime?)time;
        }

        public static string SetContinuousDays(string strBeginDate, string strEndDate)
        {
            if (strBeginDate == strEndDate)
                return strBeginDate;

            return strBeginDate + "~" + strEndDate;
        }

        public static bool IsContinuous(Date date1, Date date2)
        {
            DateTime dt1 = new DateTime(date1.Year, date1.Month, date1.Day);
            DateTime dt2 = new DateTime(date2.Year, date2.Month, date2.Day);

            int diff = (int)((dt2 - dt1).TotalDays + 0.1);

            if (diff == 1)
            {
                if ((Date.IsFullDay(date1.DateType) || date1.DateType == (int)Date.DateTypes.PM || (date1.DateType & Date.Quater4th) == Date.Quater4th) && Date.IsContinuous(date1.DateType))
                {
                    if ((Date.IsFullDay(date2.DateType) || date2.DateType == (int)Date.DateTypes.AM || (date2.DateType & Date.Quater1st) == Date.Quater1st) && Date.IsContinuous(date2.DateType))
                        return true;
                }

                /*if (date1.Type == Date.DateType.Normal)
                {
                    if (date2.Type == Date.DateType.Normal || date2.Type == Date.DateType.AM)
                        return true;
                }
                else if (date1.Type == Date.DateType.PM)
                {
                    if (date2.Type == Date.DateType.Normal || date2.Type == Date.DateType.AM)
                        return true;
                }*/
            }
            else if (diff == 0)
            {
                if (Date.IsContinuous(date1.DateType, date2.DateType))
                    return true;
                /*if (date1.Type == Date.DateType.AM && date2.Type == Date.DateType.PM)
                    return true;*/
            }

            return false;
        }

        private bool ReadOptions(out int nNoticeDays, out string strErrorMessage)
        {
            strErrorMessage = null;
            nNoticeDays = 0;
            
            string strNoticeDays = "NoticeDays";

            List<string> propertyNames = new List<string>();
            propertyNames.Add(strNoticeDays);

            List<VacationOption> options = m_dataManager.GetSelectManager().SelectOptions(propertyNames, out strErrorMessage);

            if (options == null || strErrorMessage != null)
            {
                System.Diagnostics.Trace.WriteLine("CheckScheduleError : " + strErrorMessage);
                return false;
            }

            foreach (VacationOption option in options)
            {
                if (string.Compare(option.PropertyName, strNoticeDays, true) == 0)
                {
                    if (int.TryParse(option.PropertyValue, out nNoticeDays) == false)
                    {
                        strErrorMessage = option.PropertyName + "에 잘못된 데이터가 들어 있습니다. => " + option.PropertyValue;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
