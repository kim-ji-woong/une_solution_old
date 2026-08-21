using System;
using System.Collections.Generic;
using System.Collections;
using dnsDBUtil;

namespace Vacation.DAL
{
    using IDAL;
    using Vacation.Model;

    public class CreateManager : QueryManager, ICreateManager
    {
        private WebDBManager m_dbMgr = null;
        private DataManager m_dataManager = null;

        public CreateManager(WebDBManager dbMgr, DataManager dataManager)
        {
            m_dbMgr = dbMgr;
            m_dataManager = dataManager;
        }

        public static string TimeString(DateTime time)
        {
            return string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                time.Year, time.Month, time.Day,
                time.Hour, time.Minute, time.Second);
        }

        public CompanyMember CreateCompanyMember(string name, int jobLevelID, DateTime startDate, int teamID, bool isTeamLeader, bool isAdmin, string strUserID, string strPassword, string strPasswordCode, string strPhoneNumber)
        {
            name = CheckQueryString(name);
            strUserID = CheckQueryString(strUserID);
            strPhoneNumber = CheckQueryString(strPhoneNumber);

            string strFormat = "Insert into {0} (ID, Name, JobLevelID, StartDate, TeamID, IsTeamLeader, IsAdmin, UserID, UserPW, PasswordCode, PhoneNumber) values (";
            string strSQL = string.Format(strFormat + "IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, '{1}', {2}, '{3}', {4}, {5}, {6}, '{7}', {8}, {9}, {10})",
                CompanyMember.GetTableName(),
                name, jobLevelID, TimeString(startDate),
                teamID,
                isTeamLeader ? 1 : 0,
                isAdmin ? 1 : 0,
                strUserID,
                strPassword == null ? "NULL" : "'" + strPassword + "'",
                strPasswordCode == null ? "NULL" : "'" + strPasswordCode + "'",
                strPhoneNumber == null ? "NULL" : "'" + strPhoneNumber + "'");

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            string strCondition;

            if (strPhoneNumber == null)
                strCondition = string.Format("Name = '{0}' and PhoneNumber is NULL and TeamID = {1}", name, teamID);
            else
                strCondition = string.Format("Name = '{0}' and PhoneNumber = '{1}' and TeamID = {2}", name, strPhoneNumber, teamID);

            strSQL = string.Format("Select ID from {0} where {1}", CompanyMember.GetTableName(), strCondition);
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return null;

            CompanyMember member = new CompanyMember();

            member.ID = id.Data;
            member.IsAdmin = isAdmin;
            member.IsTeamLeader = isTeamLeader;
            member.Name = name;
            member.JobLevelID = jobLevelID;
            member.Password = strPassword;
            member.PasswordCode = strPasswordCode;
            member.PhoneNumber = strPhoneNumber;
            member.StartDate = startDate;
            member.TeamID = teamID;
            member.UserID = strUserID;

            return member;
        }

        public JobLevel CreateJobLevel(string strLevelName)
        {
            strLevelName = CheckQueryString(strLevelName);

            string strFormat = "Insert into {0} (ID, LevelName) values (";
            string strSQL = string.Format(strFormat + "IsNull((SELECT MAX(ID) FROM {0} J), 0) + 1, '{1}'",
                JobLevel.GetTableName(), strLevelName);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            string strCondition = string.Format("LevelName = '{0}'", strLevelName);

            strSQL = string.Format("Select ID from {0} where {1}", JobLevel.GetTableName(), strCondition);
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return null;

            JobLevel level = new JobLevel();
            level.ID = id.Data;
            level.LevelName = strLevelName;
            
            return level;
        }

        public History CreateHistory(int memberID, int year, float totalDays, float usedDays, float waitingDays, List<int> requestIDs, DateTime nextVacationDay)
        {
            string strFormat = "Insert into {0} (MemberID, Year, TotalDays, UsedDays, WaitingDays, RequestIDs, NextVacationDay) values (";
            string strSQL = string.Format(strFormat + "{1}, {2}, {3}, {4}, {5}, '{6}', '{7}')",
                History.GetTableName(),
                memberID,
                year,
                totalDays,
                usedDays,
                waitingDays,
                ListToString<int>(requestIDs),
                TimeString(nextVacationDay));

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            History history = new History();

            history.MemberID = memberID;
            history.Year = year;
            history.TotalDays = totalDays;
            history.UsedDays = usedDays;
            history.WaitingDays = waitingDays;
            history.RequestIDs.AddRange(requestIDs);
            history.NextVacationDay = nextVacationDay;

            return history;
        }

        public RegularTeam CreateRegularTeam(string name, int? parentID)
        {
            name = CheckQueryString(name);

            string strFormat = "Insert into {0} (ID, Name, ParentID) values (";
            string strSQL = string.Format(strFormat + "IsNull((SELECT MAX(ID) FROM {0} R), 0) + 1, '{1}', {2})",
                RegularTeam.GetTableName(), name, parentID == null ? "NULL" : parentID.ToString());

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            string strCondition;

            if (parentID == null)
                strCondition = string.Format("Name = '{0}' and ParentID is NULL", name);
            else
                strCondition = string.Format("Name = '{0}' and ParentID = {1}", name, parentID);

            strSQL = string.Format("Select ID from {0} where {1}", RegularTeam.GetTableName(), strCondition);
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return null;

            RegularTeam team = new RegularTeam();

            team.ID = id.Data;
            team.Name = name;
            team.ParentTeamID = parentID;

            return team;
        }

        public Request CreateRequest(DateTime requestTime, int memberID, List<Date> days, List<int> managerIDs, Response.ResponseType response, string strRequestDescription, int year, int? year2, DateTime? mailSendTime)
        {
            strRequestDescription = CheckQueryString(strRequestDescription);

            string strDays = Date.DateListToString(days);
            string strManagerIDs = ListToString<int>(managerIDs);

            string strFormat = "Insert into {0} (ID, RequestTime, MemberID, Days, ManagerIDs, Response, RequestDescription, Year, Year2, MailSendTime) values (";
            string strSQL = string.Format(strFormat + "IsNull((SELECT MAX(ID) FROM {0} R), 0) + 1, '{1}', {2}, '{3}', '{4}', {5}, {6}, {7}, {8}, {9})",
                Request.GetTableName(),
                TimeString(requestTime),
                memberID,
                strDays,
                strManagerIDs,
                response == Response.ResponseType.None ? "NULL" : ((int)response).ToString(),
                strRequestDescription == null ? "NULL" : "'" + strRequestDescription + "'",
                year,
                year2 == null ? "NULL" : year2.ToString(),
                mailSendTime == null ? "NULL" : "'" + TimeString((DateTime)mailSendTime) + "'");

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            string strCondition = string.Format("MemberID = {0} and RequestTime = '{1}'", memberID, TimeString(requestTime));

            strSQL = string.Format("Select ID from {0} where {1}", Request.GetTableName(), strCondition);
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return null;

            Request request = new Request();

            request.ID = id.Data;
            request.RequestTime = requestTime;
            request.MemberID = memberID;
            request.Days.AddRange(days);
            request.ManagerIDs.AddRange(managerIDs);
            request.Response = response;
            request.RequestDescription = strRequestDescription;
            request.Year = year;
            request.Year2 = year2;
            request.MailSendTime = mailSendTime;

            return request;
        }

        public Request CreateRequest(int id, DateTime requestTime, int memberID, List<Date> days, List<int> managerIDs, Response.ResponseType response, string strRequestDescription, int year, int? year2, DateTime? mailSendTime)
        {
            strRequestDescription = CheckQueryString(strRequestDescription);

            string strDays = Date.DateListToString(days);
            string strManagerIDs = ListToString<int>(managerIDs);

            string strFormat = "Insert into {0} (ID, RequestTime, MemberID, Days, ManagerIDs, Response, RequestDescription, Year, Year2, MailSendTime) values (";
            string strSQL = string.Format(strFormat + "{10}, '{1}', {2}, '{3}', '{4}', {5}, {6}, {7}, {8}, {9})",
                Request.GetTableName(),
                TimeString(requestTime),
                memberID,
                strDays,
                strManagerIDs,
                response == Response.ResponseType.None ? "NULL" : ((int)response).ToString(),
                strRequestDescription == null ? "NULL" : "'" + strRequestDescription + "'",
                year,
                year2 == null ? "NULL" : year2.ToString(),
                mailSendTime == null ? "NULL" : "'" + TimeString((DateTime)mailSendTime) + "'",
                id);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            Request request = new Request();

            request.ID = id;
            request.RequestTime = requestTime;
            request.MemberID = memberID;
            request.Days.AddRange(days);
            request.ManagerIDs.AddRange(managerIDs);
            request.Response = response;
            request.RequestDescription = strRequestDescription;
            request.Year = year;
            request.Year2 = year2;
            request.MailSendTime = mailSendTime;

            return request;
        }

        public Response CreateResponse(int requestID, int managerID, Response.ResponseType response, DateTime? responseTime, string strResponseDescription, int? prevResponseID)
        {
            strResponseDescription = CheckQueryString(strResponseDescription);

            string strFormat = "Insert into {0} (ID, RequestID, ManagerID, Response, ResponseTime, ResponseDescription, PrevResponseID) values (";
            string strSQL = string.Format(strFormat + "IsNull((SELECT MAX(ID) FROM {0} R), 0) + 1, {1}, {2}, {3}, {4}, {5}, {6})",
                Response.GetTableName(),
                requestID,
                managerID,
                response == Response.ResponseType.None ? "NULL" : ((int)response).ToString(),
                responseTime == null ? "NULL" : "'" + TimeString((DateTime)responseTime) + "'",
                strResponseDescription == null ? "NULL" : "'" + strResponseDescription + "'",
                prevResponseID == null ? "NULL" : prevResponseID.ToString());

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            string strCondition = string.Format("RequestID = {0} and ManagerID = {1}", requestID, managerID);

            strSQL = string.Format("Select ID from {0} where {1}", Response.GetTableName(), strCondition);
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return null;

            Response _response = new Response();

            _response.ID = id.Data;
            _response.RequestID = requestID;
            _response.ManagerID = managerID;
            _response.Result = response;
            _response.ResponseTime = responseTime;
            _response.Description = strResponseDescription;
            _response.PrevResponseID = prevResponseID;

            return _response;
        }

        public Response CreateResponse(int id, int requestID, int managerID, Response.ResponseType response, DateTime? responseTime, string strResponseDescription, int? prevResponseID)
        {
            strResponseDescription = CheckQueryString(strResponseDescription);

            string strFormat = "Insert into {0} (ID, RequestID, ManagerID, Response, ResponseTime, ResponseDescription, PrevResponseID) values (";
            string strSQL = string.Format(strFormat + "{7}, {1}, {2}, {3}, {4}, {5}, {6})",
                Response.GetTableName(),
                requestID,
                managerID,
                response == Response.ResponseType.None ? "NULL" : ((int)response).ToString(),
                responseTime == null ? "NULL" : "'" + TimeString((DateTime)responseTime) + "'",
                strResponseDescription == null ? "NULL" : "'" + strResponseDescription + "'",
                prevResponseID == null ? "NULL" : prevResponseID.ToString(),
                id);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            Response _response = new Response();

            _response.ID = id;
            _response.RequestID = requestID;
            _response.ManagerID = managerID;
            _response.Result = response;
            _response.ResponseTime = responseTime;
            _response.Description = strResponseDescription;
            _response.PrevResponseID = prevResponseID;

            return _response;
        }

        public SpecialVacationRequest CreateSpecialVacationRequest(float days, DateTime requestTime, int requestManagerID, List<int> memberIDs, List<int> responseManagerIDs, Response.ResponseType response, string strRequestDescription)
        {
            strRequestDescription = CheckQueryString(strRequestDescription);

            string strFormat = "Insert into {0} (ID, Days, RequestTime, RequestManagerID, MemberIDs, ResponseManagerIDs, Response, RequestDescription) values (";
            string strSQL = string.Format(strFormat + "IsNull((SELECT MAX(ID) FROM {0} R), 0) + 1, {1}, '{2}', {3}, '{4}', '{5}', {6}, {7})",
                SpecialVacationRequest.GetTableName(),
                days,
                TimeString(requestTime),
                requestManagerID,
                ListToString<int>(memberIDs),
                ListToString<int>(responseManagerIDs),
                response == Response.ResponseType.None ? "NULL" : ((int)response).ToString(),
                strRequestDescription == null ? "NULL" : "'" + strRequestDescription + "'");

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            string strCondition = string.Format("RequestManagerID = {0} and RequestTime = '{1}'", requestManagerID, TimeString(requestTime));

            strSQL = string.Format("Select ID from {0} where {1}", SpecialVacationRequest.GetTableName(), strCondition);
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return null;

            SpecialVacationRequest request = new SpecialVacationRequest();

            request.ID = id.Data;
            request.Days = days;
            request.RequestTime = requestTime;
            request.RequestManagerID = requestManagerID;
            request.MemberIDs.AddRange(memberIDs);
            request.ResponseManagerIDs.AddRange(responseManagerIDs);
            request.Response = response;
            request.RequestDescription = strRequestDescription;

            return request;
        }

        public SpecialVacationResponse CreateSpecialVacationResponse(int requestID, int managerID, Response.ResponseType response, DateTime? responseTime, string strResponseDescription, int? prevResponseID)
        {
            strResponseDescription = CheckQueryString(strResponseDescription);

            string strFormat = "Insert into {0} (ID, RequestID, ManagerID, Response, ResponseTime, ResponseDescription, PrevResponseID) values (";
            string strSQL = string.Format(strFormat + "IsNull((SELECT MAX(ID) FROM {0} R), 0) + 1, {1}, {2}, {3}, {4}, {5}, {6})",
                SpecialVacationResponse.GetTableName(),
                requestID,
                managerID,
                response == Response.ResponseType.None ? "NULL" : ((int)response).ToString(),
                responseTime == null ? "NULL" : "'" + TimeString((DateTime)responseTime) + "'",
                strResponseDescription == null ? "NULL" : "'" + strResponseDescription + "'",
                prevResponseID == null ? "NULL" : prevResponseID.ToString());

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            string strCondition = string.Format("RequestID = {0} and ManagerID = {1}", requestID, managerID);

            strSQL = string.Format("Select ID from {0} where {1}", SpecialVacationResponse.GetTableName(), strCondition);
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return null;

            SpecialVacationResponse _response = new SpecialVacationResponse();

            _response.ID = id.Data;
            _response.RequestID = requestID;
            _response.ManagerID = managerID;
            _response.Result = response;
            _response.ResponseTime = responseTime;
            _response.Description = strResponseDescription;
            _response.PrevResponseID = prevResponseID;

            return _response;
        }

        public Reservation CreateReservation(int memberID, int requestID, List<Date> days, int year, int? year2)
        {
            string strDays = Date.DateListToString(days);

            string strFormat = "Insert into {0} (ID, MemberID, RequestID, Days, Year, Year2) values (";
            string strSQL = string.Format(strFormat + "IsNull((SELECT MAX(ID) FROM {0} R), 0) + 1, {1}, {2}, '{3}', {4}, {5})",
                Reservation.GetTableName(),
                memberID,
                requestID,
                strDays,
                year,
                year2 == null ? "NULL" : year2.ToString());

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            string strCondition = string.Format("MemberID = {0} and Days = '{1}'", memberID, strDays);

            strSQL = string.Format("Select ID from {0} where {1}", Reservation.GetTableName(), strCondition);
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return null;

            Reservation reservation = new Reservation();

            reservation.ID = id.Data;
            reservation.MemberID = memberID;
            reservation.RequestID = requestID;
            reservation.Days.AddRange(days);
            reservation.Year = year;
            reservation.Year2 = year2;

            return reservation;
        }

        public SpecialVacation CreateSpecialVacation(int memberID, float days, DateTime createTime, List<int> managerIDs, int requestID, string strDescription)
        {
            strDescription = CheckQueryString(strDescription);

            string strCreateTime = TimeString(createTime);

            string strFormat = "Insert into {0} (ID, MemberID, Days, CreateTime, ManagerIDs, RequestID, Description) values (";
            string strSQL = string.Format(strFormat + "IsNull((SELECT MAX(ID) FROM {0} S), 0) + 1, {1}, {2}, '{3}', '{4}', {5}, '{6}')",
                SpecialVacation.GetTableName(),
                memberID,
                days,
                strCreateTime,
                ListToString<int>(managerIDs),
                requestID,
                strDescription);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            string strCondition = string.Format("MemberID = {0} and RequestID = {1}", memberID, requestID);

            strSQL = string.Format("Select ID from {0} where {1}", SpecialVacation.GetTableName(), strCondition);
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return null;

            SpecialVacation sv = new SpecialVacation();

            sv.ID = id.Data;
            sv.MemberID = memberID;
            sv.Days = days;
            sv.CreateTime = createTime;
            sv.ManagerIDs.AddRange(managerIDs);
            sv.RequestID = requestID;
            sv.Description = strDescription;

            return sv;
        }

        public VacationOption CreateOption(int id, string strPropertyName, string strPropertyValue, string strDescription)
        {
            strPropertyName = CheckQueryString(strPropertyName);
            strPropertyValue = CheckQueryString(strPropertyValue);
            strDescription = CheckQueryString(strDescription);

            string strFormat = "Insert into {0} (ID, PropertyName, PropertyValue, Description) values (";
            string strSQL = string.Format(strFormat + "IsNull((SELECT MAX(ID) FROM {0} S), 0) + 1, '{1}', '{2}', {3})",
                VacationOption.GetTableName(),
                strPropertyName,
                strPropertyValue,
                strDescription == null ? "NULL" : "'" + strDescription + "'");

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            string strCondition = string.Format("PropertyName = '{0}'", strPropertyName);

            strSQL = string.Format("Select ID from {0} where {1}", VacationOption.GetTableName(), strCondition);
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (nResultCount == 0)
                return null;

            if (nResultCount > 1)
            {
                // 이미 존재하는 옵션이므로 새로 추가한 것을 다시 삭제시킨다.
                for (int i=1;i<nResultCount;i++)
                {
                    VariousData<int> optionID = WebDBManager.GetIntField(arrResult[i].ToString());

                    if (optionID == null)
                        continue;

                    string strErrorMessage;
                    m_dataManager.GetDeleteManager().DeleteOption(optionID.Data, out strErrorMessage);
                }

                return null;
            }

            VariousData<int> _id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (_id == null)
                return null;

            VacationOption option = new VacationOption();

            option.ID = _id.Data;
            option.PropertyName = strPropertyName;
            option.PropertyValue = strPropertyValue;
            option.Description = strDescription;

            return option;
        }

        public ExternalLogin CreateExternalLogin(string userID, long loginKey, DateTime loginTime, bool enabled, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool isNullable;

            string strSQL = string.Format("Insert into {0} ({1}, {2}, {3}, {4}) values ('{5}', {6}, '{7}', {8})",
                ExternalLogin.GetTableName(),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.UserID, out isNullable),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.LoginKey, out isNullable),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.LoginTime, out isNullable),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.Enabled, out isNullable),
                userID,
                loginKey,
                TimeString(loginTime),
                enabled ? 1 : 0);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            ExternalLogin data = new ExternalLogin();

            data.UserID = userID;
            data.LoginKey = loginKey;
            data.LoginTime = loginTime;
            data.Enabled = enabled;

            return data;
        }
    }
}
