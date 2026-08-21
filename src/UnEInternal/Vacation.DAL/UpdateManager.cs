using System;
using System.Collections.Generic;
using dnsDBUtil;

namespace Vacation.DAL
{
    using IDAL;
    using Vacation.Model;

    public class UpdateManager : QueryManager, IUpdateManager
    {
        private WebDBManager m_dbMgr = null;

        public UpdateManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
        }

        public bool UpdateCompanyMember(CompanyMember member, out string strErrorMessage)
        {
            string strFormat = "Update {0} set Name = '{1}', JobLevelID = {2}, StartDate = '{3}', TeamID = {4}, ";
            strFormat += "IsTeamLeader = {5}, IsAdmin = {6}, UserID = '{7}', UserPW = {8}, PasswordCode = {9}, PhoneNumber = {10}";
            strFormat += " where ID = {11}";

            string strSQL = string.Format(strFormat,
                CompanyMember.GetTableName(),
                CheckQueryString(member.Name), member.JobLevelID,
                CreateManager.TimeString(member.StartDate),
                member.TeamID,
                member.IsTeamLeader ? 1 : 0,
                member.IsAdmin ? 1 : 0,
                CheckQueryString(member.UserID),
                member.Password == null ? "NULL" : "'" + member.Password + "'",
                member.PasswordCode == null ? "NULL" : "'" + member.PasswordCode + "'",
                member.PhoneNumber == null ? "NULL" : "'" + CheckQueryString(member.PhoneNumber) + "'",
                member.ID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateCompanyMember(Dictionary<CompanyMember.Fields, object> dicSets, Dictionary<CompanyMember.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<CompanyMember.Fields>(ref strSets, dicSets, CompanyMember.GetFieldName, CompanyMember.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<CompanyMember.Fields>(ref strCondition, dicConditions, CompanyMember.GetFieldName, CompanyMember.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + CompanyMember.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateJobLevel(JobLevel jobLevel, out string strErrorMessage)
        {
            string strFormat = "Update {0} set LevelName = '{1}'";
            strFormat += " where ID = {2}";

            string strSQL = string.Format(strFormat,
                JobLevel.GetTableName(),
                CheckQueryString(jobLevel.LevelName),
                jobLevel.ID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateJobLevel(Dictionary<JobLevel.Fields, object> dicSets, Dictionary<JobLevel.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<JobLevel.Fields>(ref strSets, dicSets, JobLevel.GetFieldName, JobLevel.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<JobLevel.Fields>(ref strCondition, dicConditions, JobLevel.GetFieldName, JobLevel.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + JobLevel.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateHistory(History history, out string strErrorMessage)
        {
            string strFormat = "Update {0} set TotalDays = {1}, UsedDays = {2}, WaitingDays = {3}, ";
            strFormat += "RequestIDs = '{4}', NextVacationDay = '{5}'";
            strFormat += " where MemberID = {6} And Year = {7}";

            string strSQL = string.Format(strFormat,
                History.GetTableName(),
                history.TotalDays, history.UsedDays, history.WaitingDays,
                ListToString<int>(history.RequestIDs),
                CreateManager.TimeString(history.NextVacationDay),
                history.MemberID, history.Year);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateHistory(Dictionary<History.Fields, object> dicSets, Dictionary<History.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<History.Fields>(ref strSets, dicSets, History.GetFieldName, History.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<History.Fields>(ref strCondition, dicConditions, History.GetFieldName, History.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + History.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateRegularTeam(RegularTeam team, out string strErrorMessage)
        {
            string strFormat = "Update {0} set Name = '{1}', ParentID = {2} where ID = {3}";

            string strSQL = string.Format(strFormat,
                RegularTeam.GetTableName(),
                CheckQueryString(team.Name),
                team.ParentTeamID == null ? "NULL" : team.ParentTeamID.ToString(),
                team.ID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateRegularTeam(Dictionary<RegularTeam.Fields, object> dicSets, Dictionary<RegularTeam.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<RegularTeam.Fields>(ref strSets, dicSets, RegularTeam.GetFieldName, RegularTeam.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<RegularTeam.Fields>(ref strCondition, dicConditions, RegularTeam.GetFieldName, RegularTeam.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + RegularTeam.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateRequest(Request request, out string strErrorMessage)
        {
            string strFormat = "Update {0} set RequestTime = '{1}', MemberID = {2}, Days = '{3}', Response = {4}, ";
            strFormat += "RequestDescription = {5}, ManagerIDs = '{6}', Year = {7}, Year2 = {8}, MailSendTime = {9} ";
            strFormat += " where ID = {10}";

            string strSQL = string.Format(strFormat,
                Request.GetTableName(),
                CreateManager.TimeString(request.RequestTime),
                request.MemberID,
                Date.DateListToString(request.Days),
                request.Response == Response.ResponseType.None ? "NULL" : ((int)request.Response).ToString(),
                request.RequestDescription == null ? "NULL" : "'" + CheckQueryString(request.RequestDescription) + "'",
                ListToString(request.ManagerIDs),
                request.Year,
                request.Year2 == null ? "NULL" : request.Year2.ToString(),
                request.MailSendTime == null ? "NULL" : "'" + CreateManager.TimeString((DateTime)request.MailSendTime) + "'",
                request.ID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateRequest(Dictionary<Request.Fields, object> dicSets, Dictionary<Request.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<Request.Fields>(ref strSets, dicSets, Request.GetFieldName, Request.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<Request.Fields>(ref strCondition, dicConditions, Request.GetFieldName, Request.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + Request.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateResponse(Response response, out string strErrorMessage)
        {
            string strFormat = "Update {0} set RequestID = {1}, ManagerID = {2}, Response = {3}, ";
            strFormat += "ResponseTime = {4}, ResponseDescription = {5}, PrevResponseID = {6} ";
            strFormat += "where ID = {7}";

            string strSQL = string.Format(strFormat,
                Response.GetTableName(),
                response.RequestID,
                response.ManagerID,
                response.Result == Response.ResponseType.None ? "NULL" : ((int)response.Result).ToString(),
                response.ResponseTime == null ? "NULL" : "'" + CreateManager.TimeString((DateTime)response.ResponseTime) + "'",
                response.Description == null ? "NULL" : "'" + CheckQueryString(response.Description) + "'",
                response.PrevResponseID == null ? "NULL" : response.PrevResponseID.ToString(),
                response.ID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateResponse(Dictionary<Response.Fields, object> dicSets, Dictionary<Response.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<Response.Fields>(ref strSets, dicSets, Response.GetFieldName, Response.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<Response.Fields>(ref strCondition, dicConditions, Response.GetFieldName, Response.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + Response.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSpecialVacationRequest(SpecialVacationRequest request, out string strErrorMessage)
        {
            string strFormat = "Update {0} set Days = {1}, RequestTime = '{2}', RequestManagerID = {3}, MemberIDs = '{4}', ResponseManagerIDs = '{5}', ";
            strFormat += "Response = {6}, RequestDescription = {7}";
            strFormat += " where ID = {8}";

            string strSQL = string.Format(strFormat,
                SpecialVacationRequest.GetTableName(),
                request.Days,
                CreateManager.TimeString(request.RequestTime),
                request.RequestManagerID,
                ListToString<int>(request.MemberIDs),
                ListToString<int>(request.ResponseManagerIDs),
                request.Response == Response.ResponseType.None ? "NULL" : ((int)request.Response).ToString(),
                request.RequestDescription == null ? "NULL" : "'" + CheckQueryString(request.RequestDescription) + "'",
                request.ID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateSpecialVacationRequest(Dictionary<SpecialVacationRequest.Fields, object> dicSets, Dictionary<SpecialVacationRequest.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<SpecialVacationRequest.Fields>(ref strSets, dicSets, SpecialVacationRequest.GetFieldName, SpecialVacationRequest.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<SpecialVacationRequest.Fields>(ref strCondition, dicConditions, SpecialVacationRequest.GetFieldName, SpecialVacationRequest.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + SpecialVacationRequest.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSpecialVacationResponse(SpecialVacationResponse response, out string strErrorMessage)
        {
            string strFormat = "Update {0} set RequestID = {1}, ManagerID = {2}, Response = {3}, ";
            strFormat += "ResponseTime = {4}, ResponseDescription = {5}, PrevResponseID = {6}";
            strFormat += " where ID = {7}";

            string strSQL = string.Format(strFormat,
                SpecialVacationResponse.GetTableName(),
                response.RequestID,
                response.ManagerID,
                response.Result == Response.ResponseType.None ? "NULL" : ((int)response.Result).ToString(),
                response.ResponseTime == null ? "NULL" : "'" + CreateManager.TimeString((DateTime)response.ResponseTime) + "'",
                response.Description == null ? "NULL" : "'" + CheckQueryString(response.Description) + "'",
                response.PrevResponseID == null ? "NULL" : response.PrevResponseID.ToString(),
                response.ID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateSpecialVacationResponse(Dictionary<SpecialVacationResponse.Fields, object> dicSets, Dictionary<SpecialVacationResponse.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<SpecialVacationResponse.Fields>(ref strSets, dicSets, SpecialVacationResponse.GetFieldName, SpecialVacationResponse.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<SpecialVacationResponse.Fields>(ref strCondition, dicConditions, SpecialVacationResponse.GetFieldName, SpecialVacationResponse.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + SpecialVacationResponse.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateReservation(Reservation reservation, out string strErrorMessage)
        {
            string strFormat = "Update {0} set MemberID = {1}, RequestID = {2}, Days = '{3}', ";
            strFormat += "Year = {4}, Year2 = {5}";
            strFormat += " where ID = {6}";

            string strSQL = string.Format(strFormat,
                Reservation.GetTableName(),
                reservation.MemberID,
                reservation.RequestID,
                reservation.Days,
                reservation.Year,
                reservation.Year2 == null ? "NULL" : reservation.Year2.ToString(),
                reservation.ID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateReservation(Dictionary<Reservation.Fields, object> dicSets, Dictionary<Reservation.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<Reservation.Fields>(ref strSets, dicSets, Reservation.GetFieldName, Reservation.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<Reservation.Fields>(ref strCondition, dicConditions, Reservation.GetFieldName, Reservation.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + Reservation.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSpecialVacation(SpecialVacation vacation, out string strErrorMessage)
        {
            string strFormat = "Update {0} set MemberID = {1}, Days = {2}, CreateTime = '{3}', ";
            strFormat += "ManagerIDs = '{4}', RequestID = {5}, Description = '{6}'";
            strFormat += " where ID = {7}";

            string strSQL = string.Format(strFormat,
                SpecialVacation.GetTableName(),
                vacation.MemberID,
                vacation.Days,
                CreateManager.TimeString(vacation.CreateTime),
                ListToString<int>(vacation.ManagerIDs),
                vacation.RequestID,
                vacation.Description,
                vacation.ID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateSpecialVacation(Dictionary<SpecialVacation.Fields, object> dicSets, Dictionary<SpecialVacation.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<SpecialVacation.Fields>(ref strSets, dicSets, SpecialVacation.GetFieldName, SpecialVacation.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<SpecialVacation.Fields>(ref strCondition, dicConditions, SpecialVacation.GetFieldName, SpecialVacation.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + SpecialVacation.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateOption(VacationOption option, out string strErrorMessage)
        {
            string strFormat = "Update {0} set PropertyName = '{1}', PropertyValue = '{2}', Description = {3} ";
            strFormat += " where ID = {4}";

            string strSQL = string.Format(strFormat,
                VacationOption.GetTableName(),
                CheckQueryString(option.PropertyName),
                CheckQueryString(option.PropertyValue),
                option.Description == null ? "NULL" : "'" + CheckQueryString(option.Description) + "'",
                option.ID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool UpdateOption(Dictionary<VacationOption.Fields, object> dicSets, Dictionary<VacationOption.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<VacationOption.Fields>(ref strSets, dicSets, VacationOption.GetFieldName, VacationOption.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<VacationOption.Fields>(ref strCondition, dicConditions, VacationOption.GetFieldName, VacationOption.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + VacationOption.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateExternalLogin(ExternalLogin login, out string strErrorMessage)
        {
            Dictionary<ExternalLogin.Fields, object> dicSets = new Dictionary<ExternalLogin.Fields, object>();
            Dictionary<ExternalLogin.Fields, object> dicConditions = new Dictionary<ExternalLogin.Fields, object>();

            dicSets[ExternalLogin.Fields.LoginKey] = login.LoginKey;
            dicSets[ExternalLogin.Fields.LoginTime] = login.LoginTime;
            dicSets[ExternalLogin.Fields.Enabled] = login.Enabled;
            dicConditions[ExternalLogin.Fields.UserID] = login.UserID;

            return UpdateExternalLogin(dicSets, dicConditions, out strErrorMessage);
        }

        public bool UpdateExternalLogin(Dictionary<ExternalLogin.Fields, object> dicSets, Dictionary<ExternalLogin.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<ExternalLogin.Fields>(ref strSets, dicSets, ExternalLogin.GetFieldName, ExternalLogin.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<ExternalLogin.Fields>(ref strCondition, dicConditions, ExternalLogin.GetFieldName, ExternalLogin.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + ExternalLogin.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }
    }
}
