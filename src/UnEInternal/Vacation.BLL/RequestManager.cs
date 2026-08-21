using System;
using System.Collections.Generic;
using System.Collections;
using Vacation.Model;
using Vacation.IDAL;

namespace Vacation.BLL
{
    using Models;
    using Models.Vacation;
    using Models.Account;
    using Rollback;

    public static class RequestManager
    {
        public static RequestSpecialVacationResult CreateSpecialVacationResult(IDataManager dataManager, ApplicationUser requestManager, List<ApplicationUser> responseManagers, List<ApplicationUser> users, float fDays, Response.ResponseType response, string strRequestDescription)
        {
            DateTime dtNow = DateTime.Now;

            List<int> userIDs = GetUserIDs(users);
            List<int> managerIDs = GetUserIDs(responseManagers);

            SpecialVacationRequest svr = dataManager.GetCreateManager().CreateSpecialVacationRequest(fDays, dtNow, requestManager.ID, userIDs, managerIDs, response, strRequestDescription);

            if (svr == null)
                return GetRequestSpecialVacationResult(null, 0, 0, "특별휴가 승인요청이 실패하였습니다.");

            Response.ResponseType responseType = response == Response.ResponseType.Permit ? Response.ResponseType.Permit : Response.ResponseType.None;
            string strResponseDescription = response == Response.ResponseType.Permit ? "자동승인" : null;

            string strRequestManagerName = requestManager.Name + " " + requestManager.Level;
            string strTargetNames = GetUserNames(users);

            foreach (ApplicationUser responseManager in responseManagers)
            {
                SpecialVacationResponse svResponse = dataManager.GetCreateManager().CreateSpecialVacationResponse(svr.ID, responseManager.ID, responseType, (DateTime?)dtNow, strResponseDescription, null);

                if (svResponse == null)
                {
                    return GetRequestSpecialVacationResult(null, 0, 0, "특별휴가 승인요청이 실패하였습니다.");
                }
                else if (responseManager.ID != requestManager.ID)
                {
                    KakaoManager.SendSVMessage(KakaoManager.MessageType.Request, responseManager.PhoneNumber, dtNow, fDays, strRequestManagerName, strTargetNames, strRequestDescription);
                }
            }

            string strMessage = "";

            if (responseManagers.Count == 1 && responseManagers[0].ID == requestManager.ID)
            {
                if (managerIDs.Contains(requestManager.ID) == false)
                    managerIDs.Insert(0, requestManager.ID);

                foreach (ApplicationUser user in users)
                {
                    SpecialVacation sv = dataManager.GetCreateManager().CreateSpecialVacation(user.ID, fDays, dtNow, managerIDs, svr.ID, strRequestDescription);

                    if (sv == null)
                        return GetRequestSpecialVacationResult(null, 0, 0, "특별휴가 생성이 실패하였습니다.");

                    Model.History memberHistory = dataManager.GetSelectManager().SelectHistory(user.ID, dtNow.Year, out strMessage);

                    if (memberHistory == null)
                        return GetRequestSpecialVacationResult(null, 0, 0, "DB로부터 휴가이력을 얻어오는 것이 실패하였습니다.");

                    memberHistory.TotalDays += fDays;

                    if (dataManager.GetUpdateManager().UpdateHistory(memberHistory, out strMessage) == false)
                        return GetRequestSpecialVacationResult(null, 0, 0, "[휴가이력 갱신실패] : " + strMessage);
                    else
                        KakaoManager.SendSVMessage(KakaoManager.MessageType.Notify, user.PhoneNumber, dtNow, fDays, user.Name + " " + user.Level, null, strRequestDescription);
                }

                KakaoManager.SendSVMessage(KakaoManager.MessageType.Permit, requestManager.PhoneNumber, dtNow, fDays, strRequestManagerName, strTargetNames, strRequestDescription);
                strMessage = "특별휴가가 승인되었습니다.";
            }
            else
                strMessage = "특별휴가가 요청되었습니다.";

            return GetRequestSpecialVacationResult(users, fDays, (int)response, strMessage);
        }

        private static string GetUserNames(List<ApplicationUser> users)
        {
            string strUserNames = "";

            foreach (ApplicationUser user in users)
            {
                if (strUserNames.Length == 0)
                    strUserNames = user.Name + " " + user.Level;
                else
                    strUserNames += ", " + user.Name + " " + user.Level;
            }

            return strUserNames;
        }

        private static RequestSpecialVacationResult GetRequestSpecialVacationResult(List<Models.Account.ApplicationUser> users, float fDays, int nResponseType, string strMessage)
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

        private static List<int> GetUserIDs(List<ApplicationUser> users)
        {
            List<int> ids = new List<int>();

            foreach (ApplicationUser user in users)
            {
                ids.Add(user.ID);
            }

            return ids;
        }

        // 휴가 요청
        // year1과 year2가 같으면 year2는 null 처리
        public static RequestVacationResult CreateRequest(IDataManager dataManager, VacationManager vacationManager, CompanyMember member, JobLevel memberLevel, List<ApplicationUser> managers, Dictionary<ApplicationUser, CompanyMember> dicManagers, List<Date> requestDays, List<int> managerIDs, string strRequestDescription, int year1, int year2)
        {
            DateTime dtNow = DateTime.Now;
            Request request = dataManager.GetCreateManager().CreateRequest(dtNow, member.ID, requestDays, managerIDs, Response.ResponseType.Processing, strRequestDescription, year1, year1 == year2 ? null : (int?)year2, null);

            string strErrorMessage, strMessage = "";

            if (request == null)
            {
                strErrorMessage = "휴가 승인요청이 실패하였습니다.";
                return GetRequestVacationResult(requestDays, null, null, strErrorMessage);
            }
            else
            {
                Vacation.Model.History history = dataManager.GetSelectManager().SelectHistory(member.ID, dtNow.Year, out strErrorMessage);

                if (history == null)
                {
                    return GetRequestVacationResult(requestDays, null, null, "휴가이력을 조회할 수 없습니다.");
                }
                else
                {
                    history.RequestIDs.Add(request.ID);

                    if (dataManager.GetUpdateManager().UpdateHistory(history, out strErrorMessage) == false)
                        return GetRequestVacationResult(requestDays, null, null, strErrorMessage);
                }
            }

            Models.Account.ApplicationUser firstManager = managers[0];
            CompanyMember _firstManager = dicManagers[firstManager];

            Dictionary<Models.Account.ApplicationUser, Comment> dicManagerComment = new Dictionary<Models.Account.ApplicationUser, Comment>();

            string strPeriod;
            float fTotalVacationDays;
            Dictionary<int, float> dicRequestDays = GetYearDays(requestDays, out fTotalVacationDays, out strPeriod);

            if (member.ID == _firstManager.ID)
            {
                Response response = dataManager.GetCreateManager().CreateResponse(request.ID, _firstManager.ID, Response.ResponseType.Permit, dtNow, "자동승인", null);

                if (response == null)
                {
                    strErrorMessage = "휴가 승인이 실패하였습니다.";
                    return GetRequestVacationResult(requestDays, null, null, strErrorMessage);
                }

                if (managers.Count == 1)
                {
                    request.Response = Response.ResponseType.Permit;

                    if (dataManager.GetUpdateManager().UpdateRequest(request, out strErrorMessage) == false)
                    {
                        strErrorMessage = "휴가 승인이 실패하였습니다.";
                        return GetRequestVacationResult(requestDays, null, null, strErrorMessage);
                    }

                    if (UpdateHistory(dataManager, vacationManager, member, dicRequestDays, request.ID, true, out strErrorMessage) == false)
                    {
                        return GetRequestVacationResult(requestDays, null, null, strErrorMessage);
                    }

                    KakaoManager.SendMessage(KakaoManager.MessageType.Permit, member.PhoneNumber, request.RequestTime, fTotalVacationDays, strPeriod);
                    dicManagerComment[firstManager] = MakeComment((DateTime)response.ResponseTime, (int)response.Result, response.Description);
                    strMessage = "휴가 처리가 완료되었습니다.";
                    ScheduleManager.Instance.CheckSchedule(request);
                }
                else
                {
                    string strManagerNames = "";
                    List<string> managerPhoneNumbers = new List<string>();

                    for (int i = 1; i < managers.Count; i++)
                    {
                        Models.Account.ApplicationUser nextManager = managers[i];
                        CompanyMember _nextManager = dicManagers[nextManager];

                        Response _response = dataManager.GetCreateManager().CreateResponse(request.ID, _nextManager.ID, Response.ResponseType.None, dtNow, null, response.ID);

                        if (_response == null)
                        {
                            strMessage = "휴가 승인이 실패하였습니다.";
                            return GetRequestVacationResult(requestDays, null, null, strMessage);
                        }

                        dicManagerComment[nextManager] = MakeComment((DateTime)_response.ResponseTime, (int)_response.Result, _response.Description);

                        if (strManagerNames.Length == 0)
                            strManagerNames = nextManager.Name + " " + nextManager.Level;
                        else
                            strManagerNames = ", " + nextManager.Name + " " + nextManager.Level;

                        managerPhoneNumbers.Add(_nextManager.PhoneNumber);

                        if (nextManager.IsTopManager == false)
                            break;
                    }

                    if (UpdateHistory(dataManager, vacationManager, member, dicRequestDays, request.ID, false, out strErrorMessage) == false)
                    {
                        strMessage = strErrorMessage;
                        return GetRequestVacationResult(requestDays, null, null, strMessage);
                    }

                    strMessage = strManagerNames + "님에게 휴가 승인요청이 전달되었습니다.";

                    foreach (string strPhoneNumber in managerPhoneNumbers)
                    {
                        KakaoManager.SendMessage(KakaoManager.MessageType.Request, strPhoneNumber, request.RequestTime, fTotalVacationDays, strPeriod, member.Name + " " + memberLevel.LevelName);
                    }
                }
            }
            else
            {
                Response response = dataManager.GetCreateManager().CreateResponse(request.ID, _firstManager.ID, Response.ResponseType.None, dtNow, null, null);

                if (response == null)
                {
                    strMessage = "휴가 승인요청이 실패하였습니다.";
                    return GetRequestVacationResult(requestDays, null, null, strMessage);
                }

                string strManagerNames = _firstManager.Name + " " + firstManager.Level;

                if (UpdateHistory(dataManager, vacationManager, member, dicRequestDays, request.ID, false, out strErrorMessage) == false)
                {
                    strMessage = strErrorMessage;
                    return GetRequestVacationResult(requestDays, null, null, strMessage);
                }

                dicManagerComment[firstManager] = MakeComment((DateTime)response.ResponseTime, (int)response.Result, response.Description);
                strMessage = strManagerNames + "님에게 휴가 승인요청이 전달되었습니다.";

                KakaoManager.SendMessage(KakaoManager.MessageType.Request, _firstManager.PhoneNumber, request.RequestTime, fTotalVacationDays, strPeriod, member.Name + " " + memberLevel.LevelName);
            }

            return GetRequestVacationResult(requestDays, managers, dicManagerComment, strMessage);
        }

        public static bool PostSpecialVacationRequest(IDataManager dataManager, SpecialVacationRequest request, ApplicationUser prevManager, bool permit, out string strErrorMessage)
        {
            strErrorMessage = null;

            if (permit == false)
                return PostFinalSpecialVacationRequest(dataManager, request, prevManager, permit, out strErrorMessage);

            List<ApplicationUser> managers = GetApplicationUsers(dataManager, request.ResponseManagerIDs, out strErrorMessage);

            if (managers == null)
                return false;

            if (prevManager.IsTopManager)
            {
                bool isFinish = true;

                foreach (ApplicationUser manager in managers)
                {
                    if (manager.ID != prevManager.ID && manager.IsTopManager)
                    {
                        SpecialVacationResponse response = GetSpecialVacationResponse(dataManager, request.ID, manager.ID, out strErrorMessage);

                        if (response == null)
                            return false;

                        if (response.Result != Response.ResponseType.Permit && response.Result != Response.ResponseType.Timeout)
                        {
                            isFinish = false;
                            break;
                        }
                    }
                }

                if (isFinish == false)
                    return true;
                else
                    return PostFinalSpecialVacationRequest(dataManager, request, prevManager, permit, out strErrorMessage);
            }
            else
            {
                int nManagerCount = managers.Count;

                for (int i = 0; i < nManagerCount; i++)
                {
                    ApplicationUser manager = managers[i];

                    if (manager.ID == prevManager.ID)
                    {
                        if (i == nManagerCount - 1)
                            return PostFinalSpecialVacationRequest(dataManager, request, prevManager, permit, out strErrorMessage);
                        else
                        {
                            SpecialVacationResponse response = GetSpecialVacationResponse(dataManager, request.ID, prevManager.ID, out strErrorMessage);

                            if (response == null)
                                return false;

                            ApplicationUser nextManager = managers[i + 1];

                            if (nextManager.IsTopManager)
                            {
                                for (int j = i + 1; j < nManagerCount; j++)
                                {
                                    if (MakeNextSpecialVacationResponse(dataManager, request, response, managers[j], out strErrorMessage) == false)
                                        return false;
                                }
                            }
                            else
                                return MakeNextSpecialVacationResponse(dataManager, request, response, nextManager, out strErrorMessage);
                        }

                        break;
                    }
                }
            }

            return true;
        }

        public static bool PostRequest(IDataManager dataManager, Request request, ApplicationUser prevManager, bool permit, out string strErrorMessage)
        {
            strErrorMessage = null;

            if (permit == false)
                return PostFinalRequest(dataManager, request, prevManager, permit, out strErrorMessage);

            List<ApplicationUser> managers = GetApplicationUsers(dataManager, request.ManagerIDs, out strErrorMessage);

            if (managers == null)
                return false;

            if (prevManager.IsTopManager)
            {
                bool isFinish = true;

                foreach (ApplicationUser manager in managers)
                {
                    if (manager.ID != prevManager.ID && manager.IsTopManager)
                    {
                        Response response = GetResponse(dataManager, request.ID, manager.ID, out strErrorMessage);

                        if (response == null)
                            return false;

                        if (response.Result != Response.ResponseType.Permit && response.Result != Response.ResponseType.Timeout)
                        {
                            isFinish = false;
                            break;
                        }
                    }
                }

                if (isFinish == false)
                    return true;
                else
                    return PostFinalRequest(dataManager, request, prevManager, permit, out strErrorMessage);
            }
            else
            {
                int nManagerCount = managers.Count;

                for (int i=0;i<nManagerCount;i++)
                {
                    ApplicationUser manager = managers[i];

                    if (manager.ID == prevManager.ID)
                    {
                        if (i == nManagerCount - 1)
                            return PostFinalRequest(dataManager, request, prevManager, permit, out strErrorMessage);
                        else
                        {
                            Response response = GetResponse(dataManager, request.ID, prevManager.ID, out strErrorMessage);

                            if (response == null)
                                return false;

                            ApplicationUser nextManager = managers[i + 1];

                            if (nextManager.IsTopManager)
                            {
                                for (int j = i + 1; j < nManagerCount; j++)
                                {
                                    if (MakeNextResponse(dataManager, request, response, managers[j], out strErrorMessage) == false)
                                        return false;
                                }
                            }
                            else
                                return MakeNextResponse(dataManager, request, response, nextManager, out strErrorMessage);
                        }

                        break;
                    }
                }
            }

            return true;
        }

        private static bool MakeNextSpecialVacationResponse(IDataManager dataManager, SpecialVacationRequest request, SpecialVacationResponse prevResponse, ApplicationUser manager, out string strErrorMessage)
        {
            strErrorMessage = null;

            SpecialVacationResponse response = dataManager.GetCreateManager().CreateSpecialVacationResponse(request.ID, manager.ID, Response.ResponseType.None, null, null, (int?)prevResponse.ID);

            if (response == null)
            {
                strErrorMessage = "특별휴가 결재이력을 생성할 수 없습니다.";
                return false;
            }

            ApplicationUser requestManager = GetApplicationUser(dataManager, request.RequestManagerID, null, out strErrorMessage);

            if (requestManager == null)
            {
                strErrorMessage = "특별휴가 요청자를 찾을 수 없습니다.";
                return false;
            }

            string strRequestManagerName = requestManager.Name + " " + requestManager.Level;

            List<ApplicationUser> users = GetApplicationUsers(dataManager, request.MemberIDs, out strErrorMessage);

            if (users != null)
            {
                strErrorMessage = "특별휴가 대상자를 찾을 수 없습니다.";
                return false;
            }

            string strTargetNames = GetUserNames(users);

            KakaoManager.SendSVMessage(KakaoManager.MessageType.Request, manager.PhoneNumber, DateTime.Now, request.Days, strRequestManagerName, strTargetNames, request.RequestDescription);
            return true;
        }

        private static bool MakeNextResponse(IDataManager dataManager, Request request, Response prevResponse, ApplicationUser manager, out string strErrorMessage)
        {
            strErrorMessage = null;

            Response response = dataManager.GetCreateManager().CreateResponse(request.ID, manager.ID, Response.ResponseType.None, null, null, (int?)prevResponse.ID);

            if (response == null)
            {
                strErrorMessage = "휴가 결재이력을 생성할 수 없습니다.";
                return false;
            }

            string strPeriod;
            float fTotalVacationDays;
            Dictionary<int, float> dicRequestDays = GetYearDays(request.Days, out fTotalVacationDays, out strPeriod);

            ApplicationUser user = GetApplicationUser(dataManager, request.MemberID, null, out strErrorMessage);

            if (user == null)
                return false;

            KakaoManager.SendMessage(KakaoManager.MessageType.Request, manager.PhoneNumber, DateTime.Now, fTotalVacationDays, strPeriod, user.Name + "" + user.Level);
            return true;
        }

        private static SpecialVacationResponse GetSpecialVacationResponse(IDataManager dataManager, int nRequestID, int nManagerID, out string strErrorMessage)
        {
            Dictionary<SpecialVacationResponse.Fields, object> dicConditions = new Dictionary<SpecialVacationResponse.Fields, object>();
            dicConditions[SpecialVacationResponse.Fields.RequestID] = nRequestID;
            dicConditions[SpecialVacationResponse.Fields.ManagerID] = nManagerID;

            List<SpecialVacationResponse> responses = dataManager.GetSelectManager().SelectSpecialVacationResponse(dicConditions, out strErrorMessage);

            if (responses == null)
                return null;

            if (responses.Count == 0)
            {
                strErrorMessage = string.Format("결재이력을 찾을수 없습니다.(RequestID : {0}, ManagerID : {1})", nRequestID, nManagerID);
                return null;
            }

            return responses[0];
        }

        private static Response GetResponse(IDataManager dataManager, int nRequestID, int nManagerID, out string strErrorMessage)
        {
            Dictionary<Response.Fields, object> dicConditions = new Dictionary<Response.Fields, object>();
            dicConditions[Response.Fields.RequestID] = nRequestID;
            dicConditions[Response.Fields.ManagerID] = nManagerID;

            List<Response> responses = dataManager.GetSelectManager().SelectResponse(dicConditions, out strErrorMessage);

            if (responses == null)
                return null;

            if (responses.Count == 0)
            {
                strErrorMessage = string.Format("결재이력을 찾을수 없습니다.(RequestID : {0}, ManagerID : {1})", nRequestID, nManagerID);
                return null;
            }

            return responses[0];
        }

        // 최종 결재처리를 끝내고 난뒤의 처리
        // Response Table까지 Update 한 상황
        public static bool PostFinalSpecialVacationRequest(IDataManager dataManager, SpecialVacationRequest request, ApplicationUser lastManager, bool permit, out string strErrorMessage)
        {
            DateTime dtNow = DateTime.Now;

            List<int> managerIDs = new List<int>();
            managerIDs.Add(request.RequestManagerID);
            managerIDs.AddRange(request.ResponseManagerIDs);

            foreach (int memberID in request.MemberIDs)
            {
                Vacation.Model.History history = dataManager.GetSelectManager().SelectHistory(memberID, dtNow.Year, out strErrorMessage);

                if (history == null)
                    return false;

                CompanyMember member = dataManager.GetSelectManager().SelectCompanyMember(memberID, out strErrorMessage);

                if (member == null)
                    return false;

                if (permit)
                {
                    SpecialVacation sv = dataManager.GetCreateManager().CreateSpecialVacation(member.ID, request.Days, dtNow, managerIDs, request.ID, request.RequestDescription);

                    if (sv == null)
                        return false;

                    history.TotalDays += request.Days;

                    if (dataManager.GetUpdateManager().UpdateHistory(history, out strErrorMessage) == false)
                        return false;

                    request.Response = Response.ResponseType.Permit;

                    // 특별휴가 부과 대상자에게 보낸다.
                    KakaoManager.SendSVMessage(KakaoManager.MessageType.Notify, member.PhoneNumber, dtNow, request.Days, member.Name, null, request.RequestDescription);
                }
                else
                {
                    request.Response = Response.ResponseType.Deny;
                }
            }

            if (dataManager.GetUpdateManager().UpdateSpecialVacationRequest(request, out strErrorMessage) == false)
                return false;

            // 특별휴가를 신청한 팀장에게 보낸다.
            ApplicationUser manager = GetApplicationUser(dataManager, request.RequestManagerID, null, out strErrorMessage);

            if (manager != null)
            {
                KakaoManager.MessageType messageType = KakaoManager.MessageType.Permit;

                if (request.Response == Response.ResponseType.Deny)
                    messageType = KakaoManager.MessageType.Deny;

                KakaoManager.SendSVMessage(messageType, manager.PhoneNumber, request.RequestTime, request.Days, null, null, null);
            }

            return true;
        }

        // 최종 결재처리를 끝내고 난뒤의 처리
        // Response Table까지 Update 한 상황
        public static bool PostFinalRequest(IDataManager dataManager, Request request, ApplicationUser lastManager, bool permit, out string strErrorMessage)
        {
            Vacation.Model.History history = dataManager.GetSelectManager().SelectHistory(request.MemberID, request.Year, out strErrorMessage);

            if (history == null)
                return false;

            CompanyMember member = dataManager.GetSelectManager().SelectCompanyMember(request.MemberID, out strErrorMessage);

            if (member == null)
                return false;

            string strPeriod;
            float fTotalVacationDays;
            Dictionary<int, float> dicRequestDays = GetYearDays(request.Days, out fTotalVacationDays, out strPeriod);

            if (permit)
            {
                history.WaitingDays -= fTotalVacationDays;
                history.UsedDays += fTotalVacationDays;

                if (history.WaitingDays < 0)
                    history.WaitingDays = 0;

                if (dataManager.GetUpdateManager().UpdateHistory(history, out strErrorMessage) == false)
                    return false;

                request.Response = Response.ResponseType.Permit;

                if (dataManager.GetUpdateManager().UpdateRequest(request, out strErrorMessage) == false)
                    return false;

                KakaoManager.SendMessage(KakaoManager.MessageType.Permit, member.PhoneNumber, request.RequestTime, fTotalVacationDays, strPeriod, lastManager.Name + "" + lastManager.Level);
                return ScheduleManager.Instance.CheckSchedule(request);
            }
            else
            {
                history.WaitingDays -= fTotalVacationDays;

                if (history.WaitingDays < 0)
                    history.WaitingDays = 0;

                if (dataManager.GetUpdateManager().UpdateHistory(history, out strErrorMessage) == false)
                    return false;

                request.Response = Response.ResponseType.Deny;

                if (dataManager.GetUpdateManager().UpdateRequest(request, out strErrorMessage) == false)
                    return false;

                KakaoManager.SendMessage(KakaoManager.MessageType.Deny, member.PhoneNumber, request.RequestTime, fTotalVacationDays, strPeriod, lastManager.Name + "" + lastManager.Level);
            }

            return true;
        }

        public static float GetTotalDays(List<Date> dates)
        {
            float fDays = 0;

            foreach (Date date in dates)
            {
                fDays += Date.GetDateCount(date.DateType);
            }

            return fDays;
        }

        public static RequestVacationResult GetRequestVacationResult(List<Date> dates, List<Models.Account.ApplicationUser> managers, Dictionary<Models.Account.ApplicationUser, Comment> dicManagerComment, string strMessage)
        {
            RequestVacationResult result = new RequestVacationResult();

            if (dates == null || managers == null || dicManagerComment == null)
            {
                result.Message = strMessage;
                result.Success = false;
            }
            else
            {
                VacationDetail detail = new VacationDetail();

                detail.Dates.AddRange(dates);

                foreach (Models.Account.ApplicationUser manager in managers)
                {
                    Comment comment;

                    if (dicManagerComment.TryGetValue(manager, out comment))
                    {
                        comment = dicManagerComment[manager];
                        detail.Managers.Add(new KeyValuePair<Models.Account.ApplicationUser, Comment>(manager, comment));
                    }
                    else
                    {
                        detail.Managers.Add(new KeyValuePair<Models.Account.ApplicationUser, Comment>(manager, null));
                    }
                }

                detail.Calc();

                result.VacationDetail = detail;
                result.Message = strMessage;
                result.Success = true;
            }

            return result;
        }

        private static bool UpdateHistory(IDataManager dataManager, VacationManager vacationManager, CompanyMember member, Dictionary<int, float> dicYearUsedDays, int nRequestID, bool isPermitted, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (KeyValuePair<int, float> pair in dicYearUsedDays)
            {
                float usedDays = isPermitted ? pair.Value : 0;
                float waitingDays = isPermitted ? 0 : pair.Value;

                if (UpdateHistory(dataManager, vacationManager, member, pair.Key, usedDays, waitingDays, nRequestID, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private static bool UpdateHistory(IDataManager dataManager, VacationManager vacationManager, CompanyMember member, int year, float fUsedDays, float fWaitingDays, int nRequestID, out string strErrorMessage)
        {
            Model.History history = dataManager.GetSelectManager().SelectHistory(member.ID, year, out strErrorMessage);
            bool success = false;

            if (history == null)
            {
                if (strErrorMessage != null)
                    return false;

                DateTime time;

                if (year == DateTime.Now.Year)
                    time = DateTime.Now;
                else if (year < member.StartDate.Year)
                {
                    strErrorMessage = "입사일 이전의 휴가이력은 작성할 수 없습니다.";
                    return false;
                }
                else if (year == member.StartDate.Year)
                {
                    time = new DateTime(year, 12, 31);
                }
                else
                {
                    time = new DateTime(year, 1, 1);
                }

                DateTime nextVacationDay;
                float fDays = vacationManager.GetVacationDay(member.StartDate, time, out nextVacationDay) - vacationManager.GetMinusDay(member.UserID, time.Year - 1);

                List<int> requestIDs = new List<int>();
                requestIDs.Add(nRequestID);

                success = dataManager.GetCreateManager().CreateHistory(member.ID, year, fDays, fUsedDays, fWaitingDays, requestIDs, nextVacationDay) != null;
            }
            else
            {
                if (fUsedDays > 0 && fWaitingDays == 0)
                {
                    if (history.RequestIDs.Contains(nRequestID))
                    {
                        // Waiting 시간을 줄여준다.
                        if (history.WaitingDays >= fUsedDays - 0.001f)
                        {
                            fWaitingDays = -fUsedDays;
                        }
                    }
                }

                Dictionary<Model.History.Fields, object> dicSets = new Dictionary<Model.History.Fields, object>();
                Dictionary<Model.History.Fields, object> dicConditions = new Dictionary<Model.History.Fields, object>();

                if (history.RequestIDs.Contains(nRequestID) == false)
                    history.RequestIDs.Add(nRequestID);

                dicSets[Model.History.Fields.UsedDays] = history.UsedDays + fUsedDays;
                dicSets[Model.History.Fields.WaitingDays] = history.WaitingDays + fWaitingDays;
                dicSets[Model.History.Fields.RequestIDs] = ListToString<int>(history.RequestIDs);

                dicConditions[Model.History.Fields.MemberID] = member.ID;
                dicConditions[Model.History.Fields.Year] = year;

                success = dataManager.GetUpdateManager().UpdateHistory(dicSets, dicConditions, out strErrorMessage);
            }

            return success;
        }

        private static Comment MakeComment(DateTime time, int nResponseType, string strDescription)
        {
            Comment comment = new Comment();

            comment.TimeStamp = time;
            comment.ResponseType = nResponseType;
            comment.Description = strDescription;

            return comment;
        }

        public static string ListToString<DataType>(List<DataType> datas)
        {
            string str = "";

            foreach (DataType data in datas)
            {
                if (str.Length == 0)
                    str += data.ToString();
                else
                    str += ", " + data.ToString();
            }

            return str;
        }

        public static CompanyMember GetTopManager(IDataManager dataManager, out string strErrorMessage)
        {
            Dictionary<CompanyMember.Fields, object> dicConditions1 = new Dictionary<CompanyMember.Fields, object>();
            dicConditions1[CompanyMember.Fields.IsTeamLeader] = true;

            Dictionary<RegularTeam.Fields, object> dicConditions3 = new Dictionary<RegularTeam.Fields, object>();
            dicConditions3[RegularTeam.Fields.ParentID] = null;

            ArrayList arrDatas = dataManager.GetSelectManager().SelectCompanyMemberJobLevelRegularTeam(dicConditions1, null, dicConditions3, out strErrorMessage);

            if (arrDatas == null || arrDatas.Count < 3)
            {
                return null;
            }

            return (CompanyMember)arrDatas[0];
        }

        public static CompanyMember GetCompanyMember(IDataManager dataManager, string strUserID, out string strErrorMessage)
        {
            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.UserID] = strUserID;

            List<CompanyMember> members = dataManager.GetSelectManager().SelectCompanyMembers(dicConditions, out strErrorMessage);

            if (members == null)
            {
                return null;
            }

            if (members.Count == 0)
            {
                strErrorMessage = string.Format("{0}에 해당하는 직원정보를 찾을수 없습니다.", strUserID);
                return null;
            }

            return members[0];
        }

        // days를 읽어서 각 연도별 총 휴가일수를 계산한다.
        // Key : 연도
        // Value : 해당 연도의 총 휴가일수
        public static Dictionary<int, float> GetYearDays(List<Date> days, out float fTotalDays, out string strPeriod)
        {
            fTotalDays = 0;
            string strBeginDate = "", strEndDate = "";
            string strCurrentBeginDate = "", strCurrentDays = "";
            strPeriod = "";

            List<string> dayList = new List<string>();
            Date prevDate = null;

            float fDays = 0;
            Dictionary<int, float> dicDays = new Dictionary<int, float>();

            foreach (Date date in days)
            {
                if (dicDays.TryGetValue(date.Year, out fDays) == false)
                {
                    fDays = 0;
                }

                string strDate = string.Format("{0}월 {1}일", date.Month, date.Day);
                strDate += Date.GetDateTypeString(date.DateType);

                float fCount = Date.GetDateCount(date.DateType);
                fDays += fCount;
                fTotalDays += fCount;

                /*if (date.Type == Date.DateType.AM || date.Type == Date.DateType.PM)
                {
                    fDays += 0.5f;
                    fTotalDays += 0.5f;

                    if (date.Type == Date.DateType.AM)
                        strDate += "(오전)";
                    else
                        strDate += "(오후)";
                }
                else
                {
                    fDays += 1;
                    fTotalDays += 1;
                }*/

                if (strBeginDate.Length == 0)
                {
                    strBeginDate = strDate;
                    strCurrentBeginDate = strDate;
                    strCurrentDays = strDate;
                }
                else
                {
                    if (ScheduleManager.IsContinuous(prevDate, date))
                    {
                        strCurrentDays = ScheduleManager.SetContinuousDays(strCurrentBeginDate, strDate);
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

                dicDays[date.Year] = fDays;
            }

            if (strBeginDate == strEndDate)
                strPeriod = strBeginDate;
            else
            {
                if (strCurrentDays.Length > 0)
                    dayList.Add(strCurrentDays);

                foreach (string str in dayList)
                {
                    if (strPeriod.Length == 0)
                        strPeriod = str;
                    else
                        strPeriod += "," + str;
                }

                //strPeriod = strBeginDate + " ~ " + strEndDate;
            }

            return dicDays;
        }

        public static ApplicationUser GetApplicationUser(IDataManager dataManager, int nMemberID, ArrayList userDatas, out string strErrorMessage)
        {
            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.ID] = nMemberID;

            return GetApplicationUser(dataManager, dicConditions, userDatas, out strErrorMessage);
        }

        public static ApplicationUser GetApplicationUser(IDataManager dataManager, string strUserID, ArrayList userDatas, out string strErrorMessage)
        {
            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.UserID] = strUserID;

            return GetApplicationUser(dataManager, dicConditions, userDatas, out strErrorMessage);
        }

        private static ApplicationUser GetApplicationUser(IDataManager dataManager, Dictionary<CompanyMember.Fields, object> dicConditions, ArrayList userDatas, out string strErrorMessage)
        {
            ArrayList arrResult = dataManager.GetSelectManager().SelectCompanyMemberJobLevelRegularTeam(dicConditions, null, null, out strErrorMessage);

            if (arrResult == null)
                return null;

            if (arrResult.Count >= 3 &&
                arrResult[0] is CompanyMember &&
                arrResult[1] is JobLevel &&
                arrResult[2] is RegularTeam)
            {
                CompanyMember member = (CompanyMember)arrResult[0];
                JobLevel level = (JobLevel)arrResult[1];
                RegularTeam team = (RegularTeam)arrResult[2];

                if (userDatas != null)
                {
                    userDatas.Add(member);
                    userDatas.Add(level);
                    userDatas.Add(team);
                }

                Models.Account.ApplicationUser user = new Models.Account.ApplicationUser();

                user.ID = member.ID;
                user.UserID = member.UserID;
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

                return user;
            }

            string strConditions = "";
            bool isNullable;

            foreach (KeyValuePair<CompanyMember.Fields, object> pair in dicConditions)
            {
                string str = string.Format("{0} {1}", CompanyMember.GetFieldName(pair.Key, out isNullable), pair.Value.ToString());

                if (strConditions.Length == 0)
                    strConditions = str;
                else
                    strConditions += ", " + str;
            }

            strErrorMessage = strConditions + "에 해당하는 직원정보를 찾을수 없습니다.";
            return null;
        }

        public static List<Models.Account.ApplicationUser> GetApplicationUsers(IDataManager dataManager, List<int> memberIDs, out string strErrorMessage)
        {
            string strCondition = memberIDs.Count > 0 ? string.Format("{0}.ID in ({1})", CompanyMember.GetTableName(), RequestManager.ListToString<int>(memberIDs)) : null;

            List<ApplicationUser> users = GetApplicationUsers(dataManager, strCondition, out strErrorMessage);

            if (users == null)
                return null;

            List<ApplicationUser> sortedUsers = new List<ApplicationUser>();

            foreach (int memberID in memberIDs)
            {
                ApplicationUser user = GetApplicationUser(memberID, users);

                if (user == null)
                {
                    strErrorMessage = string.Format("{0}에 해당하는 직원정보를 찾을수 없습니다.", memberID);
                    return null;
                }
                else
                    sortedUsers.Add(user);
            }

            return sortedUsers;
        }

        public static List<Models.Account.ApplicationUser> GetApplicationUsers(IDataManager dataManager, List<string> memberUserIDs, out string strErrorMessage)
        {
            string strCondition = null;

            foreach (string userID in memberUserIDs)
            {
                if (strCondition == null)
                    strCondition = "'" + userID + "'";
                else
                    strCondition += ", '" + userID + "'";
            }

            if (strCondition != null)
                strCondition = string.Format("{0}.UserID in ({1})", CompanyMember.GetTableName(), strCondition);

            List<ApplicationUser> users = GetApplicationUsers(dataManager, strCondition, out strErrorMessage);

            if (users == null)
                return null;

            List<ApplicationUser> sortedUsers = new List<ApplicationUser>();

            foreach (string strUserID in memberUserIDs)
            {
                ApplicationUser user = GetApplicationUser(strUserID, users);

                if (user == null)
                {
                    strErrorMessage = string.Format("{0}에 해당하는 직원정보를 찾을수 없습니다.", strUserID);
                    return null;
                }
                else
                    sortedUsers.Add(user);
            }

            return sortedUsers;
        }

        private static List<Models.Account.ApplicationUser> GetApplicationUsers(IDataManager dataManager, string strCondition, out string strErrorMessage)
        {
            ArrayList arrResult = dataManager.GetSelectManager().SelectCompanyMemberJobLevelRegularTeam(null, null, null, strCondition, out strErrorMessage);

            if (arrResult == null)
                return null;

            List<ApplicationUser> users = new List<ApplicationUser>();
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

                    Models.Account.ApplicationUser user = new Models.Account.ApplicationUser();

                    user.ID = member.ID;
                    user.UserID = member.UserID;
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

                    users.Add(user);
                }
            }

            return users;
        }

        private static ApplicationUser GetApplicationUser(int id, List<ApplicationUser> users)
        {
            foreach (ApplicationUser user in users)
            {
                if (user.ID == id)
                    return user;
            }

            return null;
        }

        private static ApplicationUser GetApplicationUser(string strUserID, List<ApplicationUser> users)
        {
            foreach (ApplicationUser user in users)
            {
                if (user.UserID == strUserID)
                    return user;
            }

            return null;
        }

        public static ResponseCancelVacations CancelVacations(IDataManager dataManager, List<int> requestIDs, VacationManager vacationManager)
        {
            string strIDs = "";

            foreach (int requestID in requestIDs)
            {
                if (strIDs.Length == 0)
                    strIDs = requestID.ToString();
                else
                    strIDs += "," + requestID.ToString();
            }

            if (strIDs.Length == 0)
                return new ResponseCancelVacations(true, "");

            string strErrorMessage;
            string strConditions = string.Format("ID in ({0})", strIDs);
            List<Request> requests = dataManager.GetSelectManager().SelectRequests(null, strConditions, out strErrorMessage);

            if (requests == null)
            {
                if (strErrorMessage != null)
                    return new ResponseCancelVacations(false, strErrorMessage);
                else
                    return new ResponseCancelVacations(false, "휴가취소에 실패하였습니다.\r\n데이터를 조회할 수 없습니다.");
            }

            if (requests.Count == 0)
                return new ResponseCancelVacations(true, "");

            // Key : 휴가년도
            Dictionary<int, Vacation.Model.History> dicHistories = new Dictionary<int, Vacation.Model.History>();
            ApplicationUser user = GetApplicationUser(dataManager, requests[0].MemberID, new ArrayList(), out strErrorMessage);

            List<Response> responses = dataManager.GetSelectManager().SelectResponse(requestIDs, out strErrorMessage);

            if (strErrorMessage != null)
                return new ResponseCancelVacations(false, strErrorMessage);

            Dictionary<int, List<Response>> dicResponses = MakeRequestResponses(requests, responses);
            RollbackManager rollback = new RollbackManager();
            Dictionary<Request, string> dicChangedVacationStrings = new Dictionary<Request, string>();

            foreach (Request request in requests)
            {
                string strChangedVacation = "";

                if (CancelVacation(dataManager, user, request, dicResponses, dicHistories, rollback, ref strChangedVacation, out strErrorMessage) == false)
                {
                    rollback.Rollback(dataManager);
                    return new ResponseCancelVacations(false, strErrorMessage);
                }
                else
                    dicChangedVacationStrings[request] = strChangedVacation;
            }

            foreach (KeyValuePair<Request, string> pair in dicChangedVacationStrings)
            {
                // 휴가공지 메일을 보내지 않았으면 취소(또는 변경) 메일도 보내지 않는다.
                if (pair.Key.MailSendTime == null)
                    continue;

                if (pair.Value.Length == 0)
                {
                    float fTotalDays;
                    string strPeriod;
                    GetYearDays(pair.Key.Days, out fTotalDays, out strPeriod);
                    ScheduleManager.Instance.SendCancelEmail(pair.Key, strPeriod);
                }
                else
                    ScheduleManager.Instance.SendChangeEmail(pair.Key, pair.Value);
            }

            DateTime dtNow = DateTime.Now;
            History history = vacationManager.GetVacationHistory(user.UserID, dtNow.Year, dtNow.Month, dtNow.Day);
            History historyNextYear = vacationManager.GetVacationHistory(user.UserID, dtNow.Year + 1, 1, 1);
            return new ResponseCancelVacations(true, "", history, historyNextYear);
        }

        private static Dictionary<int, List<Response>> MakeRequestResponses(List<Request> requests, List<Response> responses)
        {
            Dictionary<int, List<Response>> dicRequestResponses = new Dictionary<int, List<Response>>();

            foreach (Request request in requests)
            {
                dicRequestResponses[request.ID] = new List<Response>();
            }

            List<Response> responseList;

            foreach (Response response in responses)
            {
                if (dicRequestResponses.TryGetValue(response.RequestID, out responseList) == false)
                {
                    // 에러상황인데, 무시한다.
                    continue;
                }

                responseList.Add(response);
            }

            return dicRequestResponses;
        }

        private static bool CancelVacation(IDataManager dataManager, ApplicationUser user, Request request, Dictionary<int, List<Response>> dicResponses, Dictionary<int, Vacation.Model.History> dicHistories, RollbackManager rollback, ref string strChangedVacation, out string strErrorMessage)
        {
            strErrorMessage = null;
            List<Response> responses;

            if (dicResponses.TryGetValue(request.ID, out responses) == false)
                return RemoveRequest(dataManager, request, rollback, out strErrorMessage);

            if (responses.Count == 0)
                return true;

            responses.Sort();
            
            Response response = responses[responses.Count - 1];

            if (response.Result == Response.ResponseType.Processing || response.Result == Response.ResponseType.None)
            {
                // 마지막에 결제한 것부터 삭제해야 한다.
                for (int i=responses.Count - 1; i >= 0;i--)
                {
                    Response _response = responses[i];

                    if (RemoveResponse(dataManager, _response, rollback, out strErrorMessage) == false)
                        return false;
                }

                if (RemoveRequest(dataManager, request, rollback, out strErrorMessage) == false)
                    return false;
            }
            else if (response.Result == Response.ResponseType.Permit)
            {
                if (ProcessPertmitResponse(dataManager, user, response, request, dicHistories, rollback, ref strChangedVacation, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private static bool ProcessPertmitResponse(IDataManager dataManager, ApplicationUser user, Response response, Request request, Dictionary<int, Vacation.Model.History> dicHistories, RollbackManager rollback, ref string strChangedVacation, out string strErrorMessage)
        {
            strErrorMessage = null;

            VacationInfo info = new VacationInfo();
            info.Days.AddRange(request.Days);
            
            VacationInfo.StatusType status = info.ToStatus(response.Result);

            if (status == VacationInfo.StatusType.Confirm)
            {
                Dictionary<Response.Fields, object> dicConditions = new Dictionary<Response.Fields, object>();
                Dictionary<Response.Fields, object> dicSets = new Dictionary<Response.Fields, object>();

                dicConditions[Response.Fields.ID] = response.ID;
                dicSets[Response.Fields.Response] = (int)Response.ResponseType.Cancel;

                if (dataManager.GetUpdateManager().UpdateResponse(dicSets, dicConditions, out strErrorMessage) == false)
                    return false;
                else
                {
                    UpdateRollbackData rollbackData = new UpdateRollbackData();
                    rollback.AddData(rollbackData);
                    rollbackData.AddUpdateResponse(response);
                }

                Dictionary<Request.Fields, object> dicConditions2 = new Dictionary<Request.Fields, object>();
                Dictionary<Request.Fields, object> dicSets2 = new Dictionary<Request.Fields, object>();

                dicConditions2[Request.Fields.ID] = request.ID;
                dicSets2[Request.Fields.Response] = (int)Response.ResponseType.Cancel;

                if (dataManager.GetUpdateManager().UpdateRequest(dicSets2, dicConditions2, out strErrorMessage) == false)
                    return false;
                else
                {
                    UpdateRollbackData rollbackData = new UpdateRollbackData();
                    rollback.AddData(rollbackData);
                    rollbackData.AddUpdateRequest(request);
                }

                Vacation.Model.History history1, history2;

                if (dicHistories.TryGetValue(request.Year, out history1) == false)
                {
                    history1 = RemoveRequestFromHistory(dataManager, user, request.Year, request, rollback, out strErrorMessage);

                    if (history1 == null)
                        return false;
                    else
                        dicHistories[request.Year] = history1;
                }

                if (request.Year2 != null && dicHistories.TryGetValue((int)request.Year2, out history2) == false)
                {
                    history2 = RemoveRequestFromHistory(dataManager, user, (int)request.Year2, request, rollback, out strErrorMessage);

                    if (history2 == null)
                        return false;
                    else
                        dicHistories[(int)request.Year2] = history2;
                }
            }
            else if (status == VacationInfo.StatusType.InProgress)
            {
                // 이미 사용한 휴가일수를 제외하고 나머지는 제거한다.
                if (UpdateVacationDays(dataManager, user, request, rollback, ref strChangedVacation, out strErrorMessage) == false)
                    return false;
            }
            
            return true;
        }

        private static Vacation.Model.History RemoveRequestFromHistory(IDataManager dataManager, ApplicationUser user, int year, Request request, RollbackManager rollback, out string strErrorMessage)
        {
            Vacation.Model.History history = dataManager.GetSelectManager().SelectHistory(user.ID, year, out strErrorMessage);

            if (history.RequestIDs.Contains(request.ID) == false)
                return history;

            List<int> requestIDs = new List<int>();
            
            foreach (int nRequestID in history.RequestIDs)
            {
                if (nRequestID == request.ID)
                    continue;
                else
                    requestIDs.Add(nRequestID);
            }

            Dictionary<Vacation.Model.History.Fields, object> dicConditions = new Dictionary<Model.History.Fields, object>();
            dicConditions[Vacation.Model.History.Fields.MemberID] = history.MemberID;
            dicConditions[Vacation.Model.History.Fields.Year] = year;

            Dictionary<Vacation.Model.History.Fields, object> dicSets = new Dictionary<Model.History.Fields, object>();
            dicSets[Vacation.Model.History.Fields.RequestIDs] = ListToString<int>(requestIDs);

            if (dataManager.GetUpdateManager().UpdateHistory(dicSets, dicConditions, out strErrorMessage) == false)
                return null;

            UpdateRollbackData rollbackData = new UpdateRollbackData();
            rollback.AddData(rollbackData);
            rollbackData.AddUpdateHistory(history);

            return history;
        }

        private static bool UpdateVacationDays(IDataManager dataManager, ApplicationUser user, Request request, RollbackManager rollback, ref string strChangedVacation, out string strErrorMessage)
        {
            DateTime dtNow = DateTime.Now;
            int today = dtNow.Year * 10000 + dtNow.Month * 100 + dtNow.Day;

            List<Date> days = new List<Date>();

            foreach (Date day in request.Days)
            {
                days.Add(new Date(day));
            }

            int nDayCount = days.Count;
            int nRemoveIndex = -1;

            for (int i=0;i<nDayCount;i++)
            {
                Date date = days[i];
                int nDay = date.Year * 10000 + date.Month * 100 + date.Day;

                if (nDay < today)
                    nRemoveIndex = i;
                else if (nDay == today)
                {
                    if (Date.BeforeNoon(date.DateType))
                    {
                        if (dtNow.Hour > VacationManager.BeginWorkHour || (dtNow.Hour == VacationManager.BeginWorkHour && dtNow.Minute >= VacationManager.BeginWorkMinute))
                            nRemoveIndex = i;
                    }
                    else if (date.DateType == (int)Date.DateTypes.PM || (date.DateType & Date.Quater3rd) == Date.Quater3rd || (date.DateType & Date.Quater4th) == Date.Quater4th)
                    {
                        if (dtNow.Hour >= 12)
                            nRemoveIndex = i;
                    }
                    else// if (Date.IsFullDay(date.DateType))
                    {
                        if (dtNow.Hour > VacationManager.BeginWorkHour || (dtNow.Hour == VacationManager.BeginWorkHour && dtNow.Minute >= VacationManager.BeginWorkMinute))
                        {
                            if (ScheduleManager.UsingType == ScheduleManager.UsingTypes.Half)
                            {
                                if (dtNow.Hour < 12)
                                {
                                    // 오전까지만 사용했으니 날짜를 분리하여
                                    // 오후부터의 휴가만 삭제할 수 있도록 한다.
                                    Date am = new Date(date);
                                    am.DateType = (int)Date.DateTypes.AM;
                                    days.Insert(i, am);
                                    nDayCount++;

                                    date.DateType = (int)Date.DateTypes.PM;
                                }
                            }
                            else
                            {
                                if (dtNow.Hour < 10)
                                {
                                    // 1Quater가 지났으니 2Quater 이후의 휴가만 삭제할 수 있도록 한다.
                                    Date date1 = new Date(date);
                                    date1.DateType = Date.Quater1st;
                                    days.Insert(i, date1);
                                    nDayCount++;

                                    date.DateType = Date.Quater2nd | Date.Quater3rd | Date.Quater4th;
                                }
                                else if (dtNow.Hour < 12)
                                {
                                    // 2Quater가 지났으니 3Quater 이후의 휴가만 삭제할 수 있도록 한다.
                                    Date date1 = new Date(date);
                                    date1.DateType = Date.Quater1st | Date.Quater2nd;
                                    days.Insert(i, date1);
                                    nDayCount++;

                                    date.DateType = Date.Quater3rd | Date.Quater4th;
                                }
                                else if (dtNow.Hour < 15)
                                {
                                    // 3Quater가 지났으니 4Quater 이후의 휴가만 삭제할 수 있도록 한다.
                                    Date date1 = new Date(date);
                                    date1.DateType = Date.Quater1st | Date.Quater2nd | Date.Quater3rd;
                                    days.Insert(i, date1);
                                    nDayCount++;

                                    date.DateType = Date.Quater4th;
                                }
                            }

                            nRemoveIndex = i;
                        }
                    }
                }
                else
                    break;
            }

            for (int i=nRemoveIndex+1;i<nDayCount;i++)
            {
                days.RemoveAt(nRemoveIndex+1);
            }

            float fTotalDaysOrigin, fTotalDaysChanged;
            string strPeriodOrigin, strPeriodChanged;
            Dictionary<int, float> dicDaysOrigin = GetYearDays(request.Days, out fTotalDaysOrigin, out strPeriodOrigin);
            Dictionary<int, float> dicDaysChanged = GetYearDays(days, out fTotalDaysChanged, out strPeriodChanged);

            string strOrigin = VacationManager.GetDaysDescription(request.Days.Count, fTotalDaysOrigin, strPeriodOrigin);
            string strChanged = VacationManager.GetDaysDescription(days.Count, fTotalDaysChanged, strPeriodChanged);

            if (strOrigin == strChanged)
            {
                strErrorMessage = null;
                return true;
            }

            string strUserName = user.Level == "사원" ? user.Name + "님" : user.Name + " " + user.Level + "님";
            strChangedVacation = string.Format("{0}의 휴가가 변경되었습니다.\r\n원래 일정 : {1}\r\n변경 일정 : {2}", strUserName, strOrigin, strChanged);

            Dictionary<Request.Fields, object> dicConditions = new Dictionary<Request.Fields, object>();
            dicConditions[Request.Fields.ID] = request.ID;

            Dictionary<Request.Fields, object> dicSets = new Dictionary<Request.Fields, object>();
            dicSets[Request.Fields.Days] = Date.DateListToString(days);

            if (dataManager.GetUpdateManager().UpdateRequest(dicSets, dicConditions, out strErrorMessage) == false)
                return false;

            UpdateRollbackData rollbackData = new UpdateRollbackData();
            rollback.AddData(rollbackData);

            rollbackData.AddUpdateRequest(request);
            return true;
        }

        private static bool RemoveResponse(IDataManager dataManager, Response response, RollbackManager rollback, out string strErrorMessage)
        {
            if (dataManager.GetDeleteManager().DeleteResponse(response.ID, out strErrorMessage) == false)
                return false;

            InsertRollbackData rollbackData = new InsertRollbackData();
            rollback.AddData(rollbackData);

            rollbackData.AddInsertResponse(response);
            return true;
        }

        private static bool RemoveRequest(IDataManager dataManager, Request request, RollbackManager rollback, out string strErrorMessage)
        {
            if (dataManager.GetDeleteManager().DeleteRequest(request.ID, out strErrorMessage) == false)
                return false;

            InsertRollbackData rollbackData = new InsertRollbackData();
            rollback.AddData(rollbackData);

            rollbackData.AddInsertRequest(request);
            return true;
        }
    }
}
