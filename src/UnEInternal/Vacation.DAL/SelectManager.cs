using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using dnsDBUtil;

namespace Vacation.DAL
{
    using IDAL;
    using Vacation.Model;

    public class SelectManager : QueryManager, ISelectManager
    {
        private WebDBManager m_dbMgr = null;

        public SelectManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
        }

        public CompanyMember SelectCompanyMember(int id, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select Name, JobLevelID, StartDate, TeamID, IsTeamLeader, IsAdmin, UserID, UserPW, PasswordCode, PhoneNumber from {0} where ID = {1}", CompanyMember.GetTableName(), id);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 10)
                return null;

            string strName = WebDBManager.GetStringField(arrResult[0]);
            VariousData<int> jobLevelID = WebDBManager.GetIntField(arrResult[1].ToString());
            VariousData<DateTime> dtStart = WebDBManager.GetDateTimeField(arrResult[2]);
            VariousData<int> teamID = WebDBManager.GetIntField(arrResult[3].ToString());
            VariousData<int> isTeamLeader = WebDBManager.GetIntField(arrResult[4].ToString());
            VariousData<int> isAdmin = WebDBManager.GetIntField(arrResult[5].ToString());
            string strUserID = WebDBManager.GetStringField(arrResult[6]);
            string pw = WebDBManager.GetStringField(arrResult[7]);
            string pwCode = WebDBManager.GetStringField(arrResult[8]);
            string strPhoneNumber = WebDBManager.GetStringField(arrResult[9]);

            if (strName == null || dtStart == null || jobLevelID == null ||
                isTeamLeader == null || teamID == null ||
                isAdmin == null || strUserID == null)
                return null;

            CompanyMember member = new CompanyMember();

            member.ID = id;
            member.Name = strName;
            member.JobLevelID = jobLevelID.Data;
            member.StartDate = dtStart.Data;
            member.TeamID = teamID.Data;
            member.IsTeamLeader = isTeamLeader.Data == 1;
            member.IsAdmin = isAdmin.Data == 1;
            member.UserID = strUserID;
            member.Password = pw;
            member.PasswordCode = pwCode;
            member.PhoneNumber = strPhoneNumber;

            return member;
        }

        public List<CompanyMember> SelectCompanyMembers(Dictionary<CompanyMember.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectCompanyMembers(dicConditions, null, out strErrorMessage);
        }

        public List<CompanyMember> SelectCompanyMembers(Dictionary<CompanyMember.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, Name, JobLevelID, StartDate, TeamID, IsTeamLeader, IsAdmin, UserID, UserPW, PasswordCode, PhoneNumber from " + CompanyMember.GetTableName();

            string strCondition = "";

            if (SetCondition<CompanyMember.Fields>(ref strCondition, dicConditions, CompanyMember.GetFieldName, CompanyMember.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<CompanyMember> members = new List<CompanyMember>();

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                CompanyMember member = ReadCompanyMember(arrResult, i, out strErrorMessage);

                if (member == null)
                    continue;
                /*VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> jobLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> dtStart = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                VariousData<int> teamID = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> isTeamLeader = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> isAdmin = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                string strUserID = WebDBManager.GetStringField(arrResult[i + 7]);
                string pw = WebDBManager.GetStringField(arrResult[i + 8]);
                string pwCode = WebDBManager.GetStringField(arrResult[i + 9]);
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 10]);

                if (id == null || strName == null || dtStart == null ||
                    jobLevelID == null || isTeamLeader == null ||
                    teamID == null || isAdmin == null || strUserID == null)
                    continue;

                CompanyMember member = new CompanyMember();

                member.ID = id.Data;
                member.Name = strName;
                member.JobLevelID = jobLevelID.Data;
                member.StartDate = dtStart.Data;
                member.TeamID = teamID.Data;
                member.IsTeamLeader = isTeamLeader.Data == 1;
                member.IsAdmin = isAdmin.Data == 1;
                member.UserID = strUserID;
                member.Password = pw;
                member.PasswordCode = pwCode;
                member.PhoneNumber = strPhoneNumber;*/

                members.Add(member);
            }

            return members;
        }

        private CompanyMember ReadCompanyMember(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            CompanyMember model = new CompanyMember();
            bool isNullable;

            foreach (CompanyMember.Fields field in CompanyMember.Fields.GetValues(typeof(CompanyMember.Fields)))
            {
                string strFieldName = CompanyMember.GetFieldName(field, out isNullable);

                if (field == CompanyMember.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ID = data.Data;
                    }
                }
                else if (field == CompanyMember.Fields.Name)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }

                    model.Name = data;
                }
                else if (field == CompanyMember.Fields.JobLevelID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.JobLevelID = data.Data;
                    }
                }
                else if (field == CompanyMember.Fields.StartDate)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.StartDate = data.Data;
                    }
                }
                else if (field == CompanyMember.Fields.TeamID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.TeamID = data.Data;
                    }
                }
                else if (field == CompanyMember.Fields.IsTeamLeader)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.IsTeamLeader = data.Data == 1;
                    }
                }
                else if (field == CompanyMember.Fields.IsAdmin)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.IsAdmin = data.Data == 1;
                    }
                }
                else if (field == CompanyMember.Fields.UserID)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }

                    model.UserID = data;
                }
                else if (field == CompanyMember.Fields.UserPW)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }

                    model.Password = data;
                }
                else if (field == CompanyMember.Fields.PhoneNumber)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }

                    model.PhoneNumber = data;
                }
                else if (field == CompanyMember.Fields.PasswordCode)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }

                    model.PasswordCode = data;
                }

                index++;
            }

            return model;
        }

        public JobLevel SelectJobLevel(int id, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select LevelName from {0} where ID = {1}", JobLevel.GetTableName(), id);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 1)
                return null;

            string strLevelName = WebDBManager.GetStringField(arrResult[0]);

            if (strLevelName == null)
                return null;

            JobLevel level = new JobLevel();
            level.ID = id;
            level.LevelName = strLevelName;

            return level;
        }

        public List<JobLevel> SelectJobLevels(Dictionary<JobLevel.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, LevelName from " + JobLevel.GetTableName();

            string strCondition = "";

            if (SetCondition<JobLevel.Fields>(ref strCondition, dicConditions, JobLevel.GetFieldName, JobLevel.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<JobLevel> levels = new List<JobLevel>();

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strLevelName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strLevelName == null)
                    continue;

                JobLevel level = new JobLevel();
                level.ID = id.Data;
                level.LevelName = strLevelName;

                levels.Add(level);
            }

            return levels;
        }

        public ArrayList SelectCompanyMemberHistories(Dictionary<CompanyMember.Fields, object> dicConditions1, Dictionary<History.Fields, object> dicConditions2, out string strErrorMessage)
        {
            return SelectCompanyMemberHistories(dicConditions1, dicConditions2, null, out strErrorMessage);
        }

        public ArrayList SelectCompanyMemberHistories(Dictionary<CompanyMember.Fields, object> dicConditions1, Dictionary<History.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string cm = CompanyMember.GetTableName();
            string h = History.GetTableName();

            string strSQL = string.Format("Select {0}.ID, {0}.Name, {0}.JobLevelID, {0}.StartDate, {0}.TeamID, {0}.IsTeamLeader, {0}.IsAdmin, {0}.UserID, {0}.PhoneNumber, ", cm);
            strSQL += string.Format("{0}.Year, {0}.TotalDays, {0}.UsedDays, {0}.WaitingDays, {0}.RequestIDs, {0}.NextVacationDay ", h);
            strSQL += string.Format("from {0}, {1} ", cm, h);
            strSQL += string.Format("where {0}.ID = {1}.MemberID", cm, h);

            string strCondition = "";

            if (SetCondition<CompanyMember.Fields>(ref strCondition, dicConditions1, CompanyMember.GetFieldName, cm, ref strErrorMessage) == false)
                return null;

            if (SetCondition<History.Fields>(ref strCondition, dicConditions2, History.GetFieldName, h, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                strSQL += " and " + strCondition;
            }

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-14;i+=15)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> jobLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> dtStart = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                VariousData<int> teamID = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> isTeamLeader = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> isAdmin = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                string strUserID = WebDBManager.GetStringField(arrResult[i + 7]);
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 8]);
                VariousData<int> year = WebDBManager.GetIntField(arrResult[i + 9].ToString());
                VariousData<float> totalDays = WebDBManager.GetFloatField(arrResult[i + 10].ToString());
                VariousData<float> usedDays = WebDBManager.GetFloatField(arrResult[i + 11].ToString());
                VariousData<float> waitingDays = WebDBManager.GetFloatField(arrResult[i + 12].ToString());
                string strRequestIDs = WebDBManager.GetStringField(arrResult[i + 13]);
                VariousData<DateTime> nextVacationDay = WebDBManager.GetDateTimeField(arrResult[i + 14]);

                if (id == null || strName == null || dtStart == null ||
                    jobLevelID == null || isTeamLeader == null ||
                    teamID == null || isAdmin == null || strUserID == null)
                    continue;

                if (year == null || totalDays == null || usedDays == null ||
                    strRequestIDs == null || nextVacationDay == null || waitingDays == null)
                    continue;

                CompanyMember member = new CompanyMember();

                member.ID = id.Data;
                member.Name = strName;
                member.JobLevelID = jobLevelID.Data;
                member.StartDate = dtStart.Data;
                member.TeamID = teamID.Data;
                member.IsTeamLeader = isTeamLeader.Data == 1;
                member.IsAdmin = isAdmin.Data == 1;
                member.UserID = strUserID;
                member.PhoneNumber = strPhoneNumber;

                History history = new History();

                history.MemberID = id.Data;
                history.Year = year.Data;
                history.TotalDays = totalDays.Data;
                history.UsedDays = usedDays.Data;
                history.WaitingDays = waitingDays.Data;
                history.RequestIDs.AddRange(StringToIntList(strRequestIDs));
                history.NextVacationDay = nextVacationDay.Data;

                arrDatas.Add(member);
                arrDatas.Add(history);
            }

            return arrDatas;
        }

        public History SelectHistory(int memberID, int year, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select TotalDays, UsedDays, WaitingDays, RequestIDs, NextVacationDay from {0} where memberID = {1} and Year = {2}", History.GetTableName(), memberID, year);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 5)
                return null;

            VariousData<float> totalDays = WebDBManager.GetFloatField(arrResult[0].ToString());
            VariousData<float> usedDays = WebDBManager.GetFloatField(arrResult[1].ToString());
            VariousData<float> waitingDays = WebDBManager.GetFloatField(arrResult[2].ToString());
            string strRequestIDs = WebDBManager.GetStringField(arrResult[3]);
            VariousData<DateTime> nextVacationDay = WebDBManager.GetDateTimeField(arrResult[4]);

            if (totalDays == null || usedDays == null || waitingDays == null ||
                strRequestIDs == null || nextVacationDay == null)
                return null;

            History history = new History();

            history.MemberID = memberID;
            history.Year = year;
            history.TotalDays = totalDays.Data;
            history.UsedDays = usedDays.Data;
            history.WaitingDays = waitingDays.Data;
            history.RequestIDs.AddRange(StringToIntList(strRequestIDs));
            history.NextVacationDay = nextVacationDay.Data;

            return history;
        }

        public List<History> SelectHistories(Dictionary<History.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectHistories(dicConditions, null, out strErrorMessage);
        }

        public List<History> SelectHistories(Dictionary<History.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select MemberID, Year, TotalDays, UsedDays, WaitingDays, RequestIDs, NextVacationDay from " + History.GetTableName();

            string strCondition = "";

            if (SetCondition<History.Fields>(ref strCondition, dicConditions, History.GetFieldName, History.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length == 0)
                    strCondition = strAdditionalConditions;
                else
                    strCondition += " and " + strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<History> histories = new List<History>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> year = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<float> totalDays = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<float> usedDays = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                VariousData<float> waitingDays = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                string strRequestIDs = WebDBManager.GetStringField(arrResult[i + 5]);
                VariousData<DateTime> nextVacationDay = WebDBManager.GetDateTimeField(arrResult[i + 6]);

                if (memberID == null || year == null || totalDays == null || usedDays == null ||
                    strRequestIDs == null || nextVacationDay == null || waitingDays == null)
                    continue;

                History history = new History();

                history.MemberID = memberID.Data;
                history.Year = year.Data;
                history.TotalDays = totalDays.Data;
                history.UsedDays = usedDays.Data;
                history.WaitingDays = waitingDays.Data;
                history.RequestIDs.AddRange(StringToIntList(strRequestIDs));
                history.NextVacationDay = nextVacationDay.Data;

                histories.Add(history);
            }

            return histories;
        }

        public int GetMinimumHistoryYear(List<int> memberIDs, out string strErrorMessage)
        {
            strErrorMessage = null;

            if (memberIDs.Count == 0)
                return 0;

            string strIDs = "";

            foreach (int memberID in memberIDs)
            {
                if (strIDs.Length == 0)
                    strIDs = memberID.ToString();
                else
                    strIDs += ", " + memberID.ToString();
            }

            string strSQL = string.Format("Select min(Year) from {0} where MemberID in ({1})", History.GetTableName(), strIDs);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return 0;
            }

            if (arrResult.Count == 0)
                return 0;

            VariousData<int> year = WebDBManager.GetIntField(arrResult[0].ToString());

            if (year != null)
                return year.Data;

            return 0;
        }

        public Request SelectRequest(int id, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select ID, RequestTime, MemberID, Days, ManagerIDs, Response, RequestDescription, Year, Year2, MailSendTime from {0} where ID = {1}", Request.GetTableName(), id);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 10)
                return null;

            Request request = ReadRequest(arrResult, 0, out strErrorMessage);
            /*VariousData<DateTime> requestTime = WebDBManager.GetDateTimeField(arrResult[0]);
            VariousData<int> memberID = WebDBManager.GetIntField(arrResult[1].ToString());
            string days = WebDBManager.GetStringField(arrResult[2]);
            string managerIDs = WebDBManager.GetStringField(arrResult[3]);
            VariousData<int> response = WebDBManager.GetIntField(arrResult[4].ToString());
            string strRequestDescription = WebDBManager.GetStringField(arrResult[5]);
            VariousData<int> year = WebDBManager.GetIntField(arrResult[6].ToString());
            VariousData<int> year2 = WebDBManager.GetIntField(arrResult[7].ToString());
            VariousData<DateTime> mailSendTime = WebDBManager.GetDateTimeField(arrResult[8]);

            if (requestTime == null || memberID == null ||
                days == null ||
                managerIDs == null || year == null)
                return null;

            Request request = new Request();

            request.ID = id;
            request.RequestTime = requestTime.Data;
            request.MemberID = memberID.Data;
            request.Days.AddRange(Date.StringToDateList(days, year.Data));
            request.ManagerIDs.AddRange(StringToIntList(managerIDs));
            request.Response = response == null ? Response.ResponseType.None : (Response.ResponseType)response.Data;
            request.RequestDescription = strRequestDescription;
            request.Year = year.Data;
            request.Year2 = year2 == null ? null : (int?)year2.Data;
            request.MailSendTime = mailSendTime == null ? null : (DateTime?)mailSendTime.Data;*/

            return request;
        }

        private Request ReadRequest(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Request model = new Request();
            bool isNullable;

            VariousData<int> year = null;
            string strDays = null;

            foreach (Request.Fields field in Request.Fields.GetValues(typeof(Request.Fields)))
            {
                string strFieldName = Request.GetFieldName(field, out isNullable);

                if (field == Request.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ID = data.Data;
                    }
                }
                else if (field == Request.Fields.RequestTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }

                    model.RequestTime = data.Data;
                }
                else if (field == Request.Fields.MemberID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.MemberID = data.Data;
                    }
                }
                else if (field == Request.Fields.Days)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        if (year != null)
                            model.Days.AddRange(Date.StringToDateList(data, year.Data));
                        else
                            strDays = data;
                    }
                }
                else if (field == Request.Fields.ManagerIDs)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.ManagerIDs.AddRange(StringToIntList(data));
                    }
                }
                else if (field == Request.Fields.Response)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.Response = (Response.ResponseType)data.Data;
                    }
                }
                else if (field == Request.Fields.RequestDescription)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }

                    model.RequestDescription = data;
                }
                else if (field == Request.Fields.Year)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.Year = data.Data;

                        if (strDays != null)
                            model.Days.AddRange(Date.StringToDateList(strDays, data.Data));
                    }
                }
                else if (field == Request.Fields.Year2)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                        else
                            model.Year2 = null;
                    }
                    else
                    {
                        model.Year2 = data.Data;
                    }
                }
                else if (field == Request.Fields.MailSendTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                        else
                            model.MailSendTime = null;
                    }
                    else
                    {
                        model.MailSendTime = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public List<Request> SelectRequests(Dictionary<Request.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectRequests(dicConditions, null, out strErrorMessage);
        }

        public List<Request> SelectRequests(Dictionary<Request.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, RequestTime, MemberID, Days, ManagerIDs, Response, RequestDescription, Year, Year2, MailSendTime from " + Request.GetTableName();

            string strCondition = "";

            if (SetCondition<Request.Fields>(ref strCondition, dicConditions, Request.GetFieldName, Request.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length == 0)
                    strCondition = strAdditionalConditions;
                else
                    strCondition += " and " + strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<Request> requests = new List<Request>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> requestTime = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string days = WebDBManager.GetStringField(arrResult[i + 3]);
                string managerIDs = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> response = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strRequestDescription = WebDBManager.GetStringField(arrResult[i + 6]);
                VariousData<int> year = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                VariousData<int> year2 = WebDBManager.GetIntField(arrResult[i + 8].ToString());
                VariousData<DateTime> mailSendTime = WebDBManager.GetDateTimeField(arrResult[i + 9]);

                if (id == null || requestTime == null ||
                    memberID == null || days == null ||
                    managerIDs == null || year == null)
                    continue;

                Request request = new Request();

                request.ID = id.Data;
                request.RequestTime = requestTime.Data;
                request.MemberID = memberID.Data;
                request.Days.AddRange(Date.StringToDateList(days, year.Data));
                request.ManagerIDs.AddRange(StringToIntList(managerIDs));
                request.Response = response == null ? Response.ResponseType.None : (Response.ResponseType)response.Data;
                request.RequestDescription = strRequestDescription;
                request.Year = year.Data;
                request.Year2 = year2 == null ? null : (int?)year2.Data;
                request.MailSendTime = mailSendTime == null ? null : (DateTime?)mailSendTime.Data;

                requests.Add(request);
            }

            return requests;
        }

        public Response SelectResponse(int id, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select ID, RequestID, ManagerID, Response, ResponseTime, ResponseDescription, PrevResponseID from {0} where ID = {1}", Response.GetTableName(), id);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 6)
                return null;

            VariousData<int> requestID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> managerID = WebDBManager.GetIntField(arrResult[1].ToString());
            VariousData<int> response = WebDBManager.GetIntField(arrResult[2].ToString());
            VariousData<DateTime> responseTime = WebDBManager.GetDateTimeField(arrResult[3]);
            string strResponseDescription = WebDBManager.GetStringField(arrResult[4]);
            VariousData<int> prevResponseID = WebDBManager.GetIntField(arrResult[5].ToString());

            if (requestID == null || managerID == null)
                return null;

            Response _response = new Response();

            _response.ID = id;
            _response.RequestID = requestID.Data;
            _response.ManagerID = managerID.Data;
            _response.Result = response == null ? Response.ResponseType.None : (Response.ResponseType)response.Data;
            _response.ResponseTime = responseTime == null ? null : (DateTime?)responseTime.Data;
            _response.Description = strResponseDescription;
            _response.PrevResponseID = prevResponseID == null ? null : (int?)prevResponseID.Data;

            return _response;
        }

        public List<Response> SelectResponse(Dictionary<Response.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectResponse(dicConditions, null, out strErrorMessage);
        }

        public List<Response> SelectResponse(Dictionary<Response.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, RequestID, ManagerID, Response, ResponseTime, ResponseDescription, PrevResponseID from " + Response.GetTableName();

            string strCondition = "";

            if (SetCondition<Response.Fields>(ref strCondition, dicConditions, Response.GetFieldName, Response.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length == 0)
                    strCondition = strAdditionalConditions;
                else
                    strCondition += " and " + strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<Response> responses = new List<Response>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> requestID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> managerID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> response = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<DateTime> responseTime = WebDBManager.GetDateTimeField(arrResult[i + 4]);
                string strResponseDescription = WebDBManager.GetStringField(arrResult[i + 5]);
                VariousData<int> prevResponseID = WebDBManager.GetIntField(arrResult[i + 6].ToString());

                if (id == null || requestID == null || managerID == null)
                    continue;

                Response _response = new Response();

                _response.ID = id.Data;
                _response.RequestID = requestID.Data;
                _response.ManagerID = managerID.Data;
                _response.Result = response == null ? Response.ResponseType.None : (Response.ResponseType)response.Data;
                _response.ResponseTime = responseTime == null ? null : (DateTime?)responseTime.Data;
                _response.Description = strResponseDescription;
                _response.PrevResponseID = prevResponseID == null ? null : (int?)prevResponseID.Data;

                responses.Add(_response);
            }

            return responses;
        }

        public List<Response> SelectResponse(List<int> requestIDs, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, RequestID, ManagerID, Response, ResponseTime, ResponseDescription, PrevResponseID from " + Response.GetTableName();

            string strCondition = "";

            if (requestIDs != null)
            {
                foreach (int requestID in requestIDs)
                {
                    if (strCondition.Length == 0)
                        strCondition = "RequestID in (" + requestID.ToString();
                    else
                        strCondition += ", " + requestID.ToString();
                }

                if (strCondition.Length > 0)
                    strCondition += ")";
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<Response> responses = new List<Response>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> requestID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> managerID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> response = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<DateTime> responseTime = WebDBManager.GetDateTimeField(arrResult[i + 4]);
                string strResponseDescription = WebDBManager.GetStringField(arrResult[i + 5]);
                VariousData<int> prevResponseID = WebDBManager.GetIntField(arrResult[i + 6].ToString());

                if (id == null || requestID == null || managerID == null)
                    continue;

                Response _response = new Response();

                _response.ID = id.Data;
                _response.RequestID = requestID.Data;
                _response.ManagerID = managerID.Data;
                _response.Result = response == null ? Response.ResponseType.None : (Response.ResponseType)response.Data;
                _response.ResponseTime = responseTime == null ? null : (DateTime?)responseTime.Data;
                _response.Description = strResponseDescription;
                _response.PrevResponseID = prevResponseID == null ? null : (int?)prevResponseID.Data;

                responses.Add(_response);
            }

            return responses;
        }

        public SpecialVacationRequest SelectSpecialVacationRequest(int id, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select Days, RequestTime, RequestManagerID, MemberIDs, ResponseManagerIDs, Response, RequestDescription from {0} where ID = {1}", SpecialVacationRequest.GetTableName(), id);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 7)
                return null;

            VariousData<float> days = WebDBManager.GetFloatField(arrResult[0].ToString());
            VariousData<DateTime> requestTime = WebDBManager.GetDateTimeField(arrResult[1]);
            VariousData<int> requestManagerID = WebDBManager.GetIntField(arrResult[2].ToString());
            string memberIDs = WebDBManager.GetStringField(arrResult[3]);
            string responseManagerIDs = WebDBManager.GetStringField(arrResult[4]);
            VariousData<int> response = WebDBManager.GetIntField(arrResult[5].ToString());
            string strRequestDescription = WebDBManager.GetStringField(arrResult[6]);

            if (days == null || requestTime == null ||
                requestManagerID == null ||
                memberIDs == null || responseManagerIDs == null)
                return null;

            SpecialVacationRequest request = new SpecialVacationRequest();

            request.ID = id;
            request.Days = days.Data;
            request.RequestTime = requestTime.Data;
            request.RequestManagerID = requestManagerID.Data;
            request.MemberIDs.AddRange(StringToIntList(memberIDs));
            request.ResponseManagerIDs.AddRange(StringToIntList(responseManagerIDs));
            request.Response = response == null ? Response.ResponseType.None : (Response.ResponseType)response.Data;
            request.RequestDescription = strRequestDescription;

            return request;
        }

        public List<SpecialVacationRequest> SelectSpecialVacationRequests(Dictionary<SpecialVacationRequest.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectSpecialVacationRequests(dicConditions, null, out strErrorMessage);
        }

        public List<SpecialVacationRequest> SelectSpecialVacationRequests(Dictionary<SpecialVacationRequest.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, Days, RequestTime, RequestManagerID, MemberIDs, ResponseManagerIDs, Response, RequestDescription from " + SpecialVacationRequest.GetTableName();

            string strCondition = "";

            if (SetCondition<SpecialVacationRequest.Fields>(ref strCondition, dicConditions, SpecialVacationRequest.GetFieldName, SpecialVacationRequest.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length == 0)
                    strCondition = strAdditionalConditions;
                else
                    strCondition += " and " + strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<SpecialVacationRequest> requests = new List<SpecialVacationRequest>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<float> days = WebDBManager.GetFloatField(arrResult[i + 1].ToString());
                VariousData<DateTime> requestTime = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                VariousData<int> requestManagerID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string memberIDs = WebDBManager.GetStringField(arrResult[i + 4]);
                string responseManagerIDs = WebDBManager.GetStringField(arrResult[i + 5]);
                VariousData<int> response = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                string strRequestDescription = WebDBManager.GetStringField(arrResult[i + 7]);

                if (days == null || requestTime == null ||
                    requestManagerID == null || id == null ||
                    memberIDs == null || responseManagerIDs == null)
                    return null;

                SpecialVacationRequest request = new SpecialVacationRequest();

                request.ID = id.Data;
                request.Days = days.Data;
                request.RequestTime = requestTime.Data;
                request.RequestManagerID = requestManagerID.Data;
                request.MemberIDs.AddRange(StringToIntList(memberIDs));
                request.ResponseManagerIDs.AddRange(StringToIntList(responseManagerIDs));
                request.Response = response == null ? Response.ResponseType.None : (Response.ResponseType)response.Data;
                request.RequestDescription = strRequestDescription;

                requests.Add(request);
            }

            return requests;
        }

        public SpecialVacationResponse SelectSpecialVacationResponse(int id, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select ID, RequestID, ManagerID, Response, ResponseTime, ResponseDescription, PrevResponseID from {0} where ID = {1}", SpecialVacationResponse.GetTableName(), id);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 6)
                return null;

            VariousData<int> requestID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> managerID = WebDBManager.GetIntField(arrResult[1].ToString());
            VariousData<int> response = WebDBManager.GetIntField(arrResult[2].ToString());
            VariousData<DateTime> responseTime = WebDBManager.GetDateTimeField(arrResult[3]);
            string strResponseDescription = WebDBManager.GetStringField(arrResult[4]);
            VariousData<int> prevResponseID = WebDBManager.GetIntField(arrResult[5].ToString());

            if (requestID == null || managerID == null)
                return null;

            SpecialVacationResponse _response = new SpecialVacationResponse();

            _response.ID = id;
            _response.RequestID = requestID.Data;
            _response.ManagerID = managerID.Data;
            _response.Result = response == null ? Response.ResponseType.None : (Response.ResponseType)response.Data;
            _response.ResponseTime = responseTime == null ? null : (DateTime?)responseTime.Data;
            _response.Description = strResponseDescription;
            _response.PrevResponseID = prevResponseID == null ? null : (int?)prevResponseID.Data;

            return _response;
        }

        public List<SpecialVacationResponse> SelectSpecialVacationResponse(Dictionary<SpecialVacationResponse.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectSpecialVacationResponse(dicConditions, null, out strErrorMessage);
        }

        public List<SpecialVacationResponse> SelectSpecialVacationResponse(Dictionary<SpecialVacationResponse.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, RequestID, ManagerID, Response, ResponseTime, ResponseDescription, PrevResponseID from " + SpecialVacationResponse.GetTableName();

            string strCondition = "";

            if (SetCondition<SpecialVacationResponse.Fields>(ref strCondition, dicConditions, SpecialVacationResponse.GetFieldName, SpecialVacationResponse.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<SpecialVacationResponse> responses = new List<SpecialVacationResponse>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> requestID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> managerID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> response = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<DateTime> responseTime = WebDBManager.GetDateTimeField(arrResult[i + 4]);
                string strResponseDescription = WebDBManager.GetStringField(arrResult[i + 5]);
                VariousData<int> prevResponseID = WebDBManager.GetIntField(arrResult[i + 6].ToString());

                if (id == null || requestID == null || managerID == null)
                    continue;

                SpecialVacationResponse _response = new SpecialVacationResponse();

                _response.ID = id.Data;
                _response.RequestID = requestID.Data;
                _response.ManagerID = managerID.Data;
                _response.Result = response == null ? Response.ResponseType.None : (Response.ResponseType)response.Data;
                _response.ResponseTime = responseTime == null ? null : (DateTime?)responseTime.Data;
                _response.Description = strResponseDescription;
                _response.PrevResponseID = prevResponseID == null ? null : (int?)prevResponseID.Data;

                responses.Add(_response);
            }

            return responses;
        }

        public List<SpecialVacationResponse> SelectSpecialVacationResponse(List<int> requestIDs, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, RequestID, ManagerID, Response, ResponseTime, ResponseDescription, PrevResponseID from " + SpecialVacationResponse.GetTableName();

            string strCondition = "";

            if (requestIDs != null)
            {
                foreach (int requestID in requestIDs)
                {
                    if (strCondition.Length == 0)
                        strCondition = "RequestID in (" + requestID.ToString();
                    else
                        strCondition += ", " + requestID.ToString();
                }

                if (strCondition.Length > 0)
                    strCondition += ")";
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<SpecialVacationResponse> responses = new List<SpecialVacationResponse>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> requestID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> managerID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> response = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<DateTime> responseTime = WebDBManager.GetDateTimeField(arrResult[i + 4]);
                string strResponseDescription = WebDBManager.GetStringField(arrResult[i + 5]);
                VariousData<int> prevResponseID = WebDBManager.GetIntField(arrResult[i + 6].ToString());

                if (id == null || requestID == null || managerID == null)
                    continue;

                SpecialVacationResponse _response = new SpecialVacationResponse();

                _response.ID = id.Data;
                _response.RequestID = requestID.Data;
                _response.ManagerID = managerID.Data;
                _response.Result = response == null ? Response.ResponseType.None : (Response.ResponseType)response.Data;
                _response.ResponseTime = responseTime == null ? null : (DateTime?)responseTime.Data;
                _response.Description = strResponseDescription;
                _response.PrevResponseID = prevResponseID == null ? null : (int?)prevResponseID.Data;

                responses.Add(_response);
            }

            return responses;
        }

        public RegularTeam SelectReqularTeam(int id, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select Name, ParentID from {0} where ID = {1}", RegularTeam.GetTableName(), id);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 2)
                return null;

            string strName = WebDBManager.GetStringField(arrResult[0]);
            VariousData<int> parentID = WebDBManager.GetIntField(arrResult[1].ToString());

            if (strName == null)
                return null;

            RegularTeam team = new RegularTeam();

            team.ID = id;
            team.Name = strName;
            team.ParentTeamID = parentID == null ? null : (int?)parentID.Data;

            return team;
        }

        public List<RegularTeam> SelectRegularTeams(Dictionary<RegularTeam.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, Name, ParentID from " + RegularTeam.GetTableName();

            string strCondition = "";

            if (SetCondition<RegularTeam.Fields>(ref strCondition, dicConditions, RegularTeam.GetFieldName, RegularTeam.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<RegularTeam> teams = new List<RegularTeam>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (id == null || strName == null)
                    continue;

                RegularTeam team = new RegularTeam();

                team.ID = id.Data;
                team.Name = strName;
                team.ParentTeamID = parentID == null ? null : (int?)parentID.Data;

                teams.Add(team);
            }

            return teams;
        }

        public Reservation SelectReservation(int id, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select MemberID, RequestID, Days, Year, Year2 from {0} where ID = {1}", Reservation.GetTableName(), id);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 5)
                return null;

            VariousData<int> memberID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> requestID = WebDBManager.GetIntField(arrResult[1].ToString());
            string strDays = WebDBManager.GetStringField(arrResult[2]);
            VariousData<int> year = WebDBManager.GetIntField(arrResult[3].ToString());
            VariousData<int> year2 = WebDBManager.GetIntField(arrResult[4].ToString());

            if (memberID == null || requestID == null ||
                strDays == null || year == null)
                return null;

            Reservation reservation = new Reservation();

            reservation.ID = id;
            reservation.MemberID = memberID.Data;
            reservation.RequestID = requestID.Data;
            reservation.Days.AddRange(Date.StringToDateList(strDays, year.Data));
            reservation.Year = year.Data;
            reservation.Year2 = year2 == null ? null : (int?)year2.Data;

            return reservation;
        }

        public List<Reservation> SelectReservations(Dictionary<Reservation.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, MemberID, RequestID, Days, Year, Year2 from " + Reservation.GetTableName();

            string strCondition = "";

            if (SetCondition<Reservation.Fields>(ref strCondition, dicConditions, Reservation.GetFieldName, Reservation.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<Reservation> reservations = new List<Reservation>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> requestID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strDays = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<int> year = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> year2 = WebDBManager.GetIntField(arrResult[i + 5].ToString());

                if (id == null || memberID == null || requestID == null ||
                    strDays == null || year == null)
                    continue;

                Reservation reservation = new Reservation();

                reservation.ID = id.Data;
                reservation.MemberID = memberID.Data;
                reservation.RequestID = requestID.Data;
                reservation.Days.AddRange(Date.StringToDateList(strDays, year.Data));
                reservation.Year = year.Data;
                reservation.Year2 = year2 == null ? null : (int?)year2.Data;

                reservations.Add(reservation);
            }

            return reservations;
        }

        public SpecialVacation SelectSpecialVacation(int id, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select MemberID, Days, CreateTime, ManagerIDs, RequestID, Description from {0} where ID = {1}", SpecialVacation.GetTableName(), id);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 6)
                return null;

            VariousData<int> memberID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<float> days = WebDBManager.GetFloatField(arrResult[1].ToString());
            VariousData<DateTime> createTime = WebDBManager.GetDateTimeField(arrResult[2]);
            string managerIDs = WebDBManager.GetStringField(arrResult[3]);
            VariousData<int> requestID = WebDBManager.GetIntField(arrResult[4].ToString());
            string strDescription = WebDBManager.GetStringField(arrResult[5]);

            if (memberID == null || days == null ||
                createTime == null || managerIDs == null ||
                requestID == null || strDescription == null)
                return null;

            SpecialVacation sv = new SpecialVacation();
            List<int> managerIDList = StringToIntList(managerIDs);

            sv.ID = id;
            sv.MemberID = memberID.Data;
            sv.Days = days.Data;
            sv.CreateTime = createTime.Data;
            sv.ManagerIDs.AddRange(managerIDList);
            sv.RequestID = requestID.Data;
            sv.Description = strDescription;

            return sv;
        }

        public List<SpecialVacation> SelectSpecialVacations(Dictionary<SpecialVacation.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectSpecialVacations(dicConditions, null, out strErrorMessage);
        }

        public List<SpecialVacation> SelectSpecialVacations(Dictionary<SpecialVacation.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, MemberID, Days, CreateTime, ManagerIDs, RequestID, Description from " + SpecialVacation.GetTableName();

            string strCondition = "";

            if (SetCondition<SpecialVacation.Fields>(ref strCondition, dicConditions, SpecialVacation.GetFieldName, SpecialVacation.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<SpecialVacation> vacations = new List<SpecialVacation>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<float> days = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<DateTime> createTime = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                string managerIDs = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> requestID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strDescription = WebDBManager.GetStringField(arrResult[i + 6]);

                if (id == null || memberID == null || days == null ||
                    createTime == null || managerIDs == null ||
                    requestID == null || strDescription == null)
                    continue;

                SpecialVacation sv = new SpecialVacation();
                List<int> managerIDList = StringToIntList(managerIDs);

                sv.ID = id.Data;
                sv.MemberID = memberID.Data;
                sv.Days = days.Data;
                sv.CreateTime = createTime.Data;
                sv.ManagerIDs.AddRange(managerIDList);
                sv.RequestID = requestID.Data;
                sv.Description = strDescription;

                vacations.Add(sv);
            }

            return vacations;
        }

        public List<SpecialVacation> SelectSpecialVacations(int year, out string strErrorMessage)
        {
            string strBeginTime = string.Format("'{0}-01-01 00:00:00'", year);
            string strEndTime = string.Format("'{0}-12-31 23:59:59'", year);
            string strCondition = string.Format("CreateTime >= {0} and CreateTime <= {1}", strBeginTime, strEndTime);
            return SelectSpecialVacations(strCondition, out strErrorMessage);
        }

        public List<SpecialVacation> SelectSpecialVacations(int memberID, int year, out string strErrorMessage)
        {
            string strBeginTime = string.Format("'{0}-01-01 00:00:00'", year);
            string strEndTime = string.Format("'{0}-12-31 23:59:59'", year);
            string strCondition = string.Format("CreateTime >= {0} and CreateTime <= {1} and MemberID = {2}", strBeginTime, strEndTime, memberID);
            return SelectSpecialVacations(strCondition, out strErrorMessage);
        }

        private List<SpecialVacation> SelectSpecialVacations(string strCondition, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, MemberID, Days, CreateTime, ManagerIDs, RequestID, Description from " + SpecialVacation.GetTableName();

            if (strCondition != null && strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<SpecialVacation> vacations = new List<SpecialVacation>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<float> days = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<DateTime> createTime = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                string managerIDs = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> requestID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strDescription = WebDBManager.GetStringField(arrResult[i + 6]);

                if (id == null || memberID == null || days == null ||
                    createTime == null || managerIDs == null ||
                    strDescription == null || requestID == null)
                    continue;

                SpecialVacation sv = new SpecialVacation();
                List<int> managerIDList = StringToIntList(managerIDs);

                sv.ID = id.Data;
                sv.MemberID = memberID.Data;
                sv.Days = days.Data;
                sv.CreateTime = createTime.Data;
                sv.ManagerIDs.AddRange(managerIDList);
                sv.RequestID = requestID.Data;
                sv.Description = strDescription;

                vacations.Add(sv);
            }

            return vacations;
        }

        public ArrayList SelectCompanyMemberJobLevelRegularTeam(Dictionary<CompanyMember.Fields, object> dicConditions1, Dictionary<JobLevel.Fields, object> dicConditions2, Dictionary<RegularTeam.Fields, object> dicConditions3, out string strErrorMessage)
        {
            return SelectCompanyMemberJobLevelRegularTeam(dicConditions1, dicConditions2, dicConditions3, null, out strErrorMessage);
        }

        public ArrayList SelectCompanyMemberJobLevelRegularTeam(Dictionary<CompanyMember.Fields, object> dicConditions1, Dictionary<JobLevel.Fields, object> dicConditions2, Dictionary<RegularTeam.Fields, object> dicConditions3, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string cm = CompanyMember.GetTableName();
            string level = JobLevel.GetTableName();
            string team = RegularTeam.GetTableName();

            string strSQL = string.Format("Select {0}.ID, {0}.Name, {0}.JobLevelID, {0}.StartDate, {0}.TeamID, {0}.IsTeamLeader, {0}.IsAdmin, {0}.UserID, {0}.UserPW, {0}.PasswordCode, {0}.PhoneNumber, ", cm);
            strSQL += string.Format("{0}.LevelName, {1}.Name, {1}.ParentID ", level, team);
            strSQL += string.Format("from {0}, {1}, {2} ", cm, level, team);
            strSQL += string.Format("where {0}.JobLevelID = {1}.ID and {0}.TeamID = {2}.ID", cm, level, team);

            string strCondition = "";

            if (SetCondition<CompanyMember.Fields>(ref strCondition, dicConditions1, CompanyMember.GetFieldName, cm, ref strErrorMessage) == false)
                return null;

            if (SetCondition<JobLevel.Fields>(ref strCondition, dicConditions2, JobLevel.GetFieldName, level, ref strErrorMessage) == false)
                return null;

            if (SetCondition<RegularTeam.Fields>(ref strCondition, dicConditions3, RegularTeam.GetFieldName, team, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                strSQL += " and " + strCondition;
            }

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 13; i += 14)
            {
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> levelID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> startDate = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                VariousData<int> teamID = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> isTeamLeader = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> isAdmin = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                string strUserID = WebDBManager.GetStringField(arrResult[i + 7]);
                string strPassword = WebDBManager.GetStringField(arrResult[i + 8]);
                string strPasswordCode = WebDBManager.GetStringField(arrResult[i + 9]);
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 10]);
                string strLevelName = WebDBManager.GetStringField(arrResult[i + 11]);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 12]);
                VariousData<int> parentTeamID = WebDBManager.GetIntField(arrResult[i + 13].ToString());

                if (memberID == null || strMemberName == null ||
                    levelID == null || startDate == null ||
                    teamID == null || isTeamLeader == null || isAdmin == null ||
                    strUserID == null || strLevelName == null || strTeamName == null)
                {
                    continue;
                }

                CompanyMember member = new CompanyMember();
                member.ID = memberID.Data;
                member.Name = strMemberName;
                member.JobLevelID = levelID.Data;
                member.StartDate = startDate.Data;
                member.TeamID = teamID.Data;
                member.IsTeamLeader = isTeamLeader.Data == 1;
                member.IsAdmin = isAdmin.Data == 1;
                member.UserID = strUserID;
                member.Password = strPassword;
                member.PasswordCode = strPasswordCode;
                member.PhoneNumber = strPhoneNumber;
                arrDatas.Add(member);

                JobLevel _level = new JobLevel();
                _level.ID = member.JobLevelID;
                _level.LevelName = strLevelName;
                arrDatas.Add(_level);

                RegularTeam _team = new RegularTeam();
                _team.ID = member.TeamID;
                _team.Name = strTeamName;

                if (parentTeamID != null)
                    _team.ParentTeamID = parentTeamID.Data;

                arrDatas.Add(_team);
            }

            return arrDatas;
        }

        public VacationOption SelectOption(int id, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select PropertyName, PropertyValue, Description from {0} where ID = {1}", VacationOption.GetTableName(), id);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            if (arrResult.Count != 3)
                return null;

            string strPropertyName = WebDBManager.GetStringField(arrResult[0]);
            string strPropertyValue = WebDBManager.GetStringField(arrResult[1]);
            string strDescription = WebDBManager.GetStringField(arrResult[2]);

            if (strPropertyName == null || strPropertyValue == null)
                return null;

            VacationOption option = new VacationOption();

            option.ID = id;
            option.PropertyName = strPropertyName;
            option.PropertyValue = strPropertyValue;
            option.Description = strDescription;

            return option;
        }

        public List<VacationOption> SelectOptions(List<string> propertyNames, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = "Select ID, PropertyName, PropertyValue, Description from " + VacationOption.GetTableName();

            string strCondition = "";

            if (propertyNames != null)
            {
                foreach (string strPropertyName in propertyNames)
                {
                    if (strCondition.Length == 0)
                        strCondition = "PropertyName = '" + CheckQueryString(strPropertyName) + "'";
                    else
                        strCondition += " or PropertyName = '" + CheckQueryString(strPropertyName) + "'";
                }
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<VacationOption> options = new List<VacationOption>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strPropertyName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 2]);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 3]);

                if (id == null || strPropertyName == null || strPropertyValue == null)
                    continue;

                VacationOption option = new VacationOption();

                option.ID = id.Data;
                option.PropertyName = strPropertyName;
                option.PropertyValue = strPropertyValue;
                option.Description = strDescription;

                options.Add(option);
            }

            return options;
        }

        public OptionKakaoInfo SelectOptionKakaoInfo(out string strErrorMessage)
        {
            strErrorMessage = null;
            OptionKakaoInfo option = null;

            string strSQL = string.Format("Select ID, CountryCode, SenderKey, BsID, BsPasswd from {0}", OptionKakaoInfo.GetTableName());
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            if (nResultCount != 5)
            {
                strErrorMessage = "Kakao 정보 부족함";
                return null;
            }
            option = new OptionKakaoInfo();
            option.ID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            option.CountryCode = WebDBManager.GetIntField(arrResult[1].ToString(), -1);
            option.SenderKey = WebDBManager.GetStringField(arrResult[2].ToString(), "");
            option.BsID = WebDBManager.GetStringField(arrResult[3].ToString(), "");
            option.BsPasswd = WebDBManager.GetStringField(arrResult[4].ToString(), "");

            return option;
        }

        public int SelectAdminLength(int teamID, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strQuery = string.Format("Select Count(*) as cnt From CompanyMember Where IsAdmin = 1 And TeamID <> {0}", teamID);
            ArrayList arrResult = m_dbMgr.GetResultData(strQuery);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return -1;
            }
            if (arrResult.Count == 0)
                return 0;

            int length = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            return length;
        }

        public ArrayList JoinCompanyMemberRequest(string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCompanyMemberTableName = CompanyMember.GetTableName();
            string strRequestTableName = Request.GetTableName();

            int nCompanyMemberFieldCount, nRequestFieldCount;

            string strCompanyMemberFields = GetFieldNames<CompanyMember.Fields>(strCompanyMemberTableName, out nCompanyMemberFieldCount);
            string strRequestFields = GetFieldNames<Request.Fields>(strRequestTableName, out nRequestFieldCount);

            int nFieldsCount = nCompanyMemberFieldCount + nRequestFieldCount;
            bool isNullable;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strCompanyMemberFields, strRequestFields);
            sb.AppendFormat("  From {0}, {1} ", strCompanyMemberTableName, strRequestTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strCompanyMemberTableName, CompanyMember.GetFieldName(CompanyMember.Fields.ID, out isNullable), strRequestTableName, Request.GetFieldName(Request.Fields.MemberID, out isNullable));
            
            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" And {0}", strAdditionalConditions);
            }

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                CompanyMember member = ReadCompanyMember(arrResult, i, out strErrorMessage);

                if (member == null)
                    return null;
                else
                    arrDatas.Add(member);

                Request request = ReadRequest(arrResult, i + nCompanyMemberFieldCount, out strErrorMessage);

                if (request == null)
                    return null;
                else
                    arrDatas.Add(request);
            }

            return arrDatas;
        }

        public ExternalLogin SelectExternalLogin(string userID, out string strErrorMessage)
        {
            strErrorMessage = null;
            ExternalLogin data = null;
            bool isNullable;

            string strSQL = string.Format("Select {1}, {2}, {3} from {0} where {4} = '{5}'",
                ExternalLogin.GetTableName(),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.LoginKey, out isNullable),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.LoginTime, out isNullable),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.Enabled, out isNullable),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.UserID, out isNullable),
                userID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;

            if (nResultCount == 0)
                return null;

            if (nResultCount != 3)
            {
                strErrorMessage = "ExternalLogin 정보 부족함";
                return null;
            }

            string strLoginKey = WebDBManager.GetStringField(arrResult[0]);
            VariousData<DateTime> loginTime = WebDBManager.GetDateTimeField(arrResult[1]);
            VariousData<int> enabled = WebDBManager.GetIntField(arrResult[2].ToString());

            if (strLoginKey == null)
            {
                strErrorMessage = "LoginKey는 null이 될수 없습니다.";
                return null;
            }

            if (loginTime == null)
            {
                strErrorMessage = "LoginTime은 null이 될수 없습니다.";
                return null;
            }

            if (enabled == null)
            {
                strErrorMessage = "Enabled는 null이 될수 없습니다.";
                return null;
            }

            long loginKey;

            if (long.TryParse(strLoginKey.Trim(), out loginKey) == false)
            {
                strErrorMessage = "LoginKey가 형식에 맞지 않습니다.";
                return null;
            }

            data = new ExternalLogin();
            data.UserID = userID;
            data.LoginKey = loginKey;
            data.LoginTime = loginTime.Data;
            data.Enabled = enabled.Data == 1;

            return data;
        }

        public List<ExternalLogin> SelectExternalLogins(Dictionary<ExternalLogin.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool isNullable;

            string strSQL = string.Format("Select {0}, {1}, {2}, {3} from {4}",
                ExternalLogin.GetFieldName(ExternalLogin.Fields.UserID, out isNullable),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.LoginKey, out isNullable),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.LoginTime, out isNullable),
                ExternalLogin.GetFieldName(ExternalLogin.Fields.Enabled, out isNullable),
                ExternalLogin.GetTableName());

            string strCondition = "";

            if (SetCondition<ExternalLogin.Fields>(ref strCondition, dicConditions, ExternalLogin.GetFieldName, ExternalLogin.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length == 0)
                    strCondition = strAdditionalConditions;
                else
                    strCondition += " and " + strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbMgr.GetResultData(strSQL) : m_dbMgr.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return null;
            }

            List<ExternalLogin> datas = new List<ExternalLogin>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                string strUserID = WebDBManager.GetStringField(arrResult[i]);
                string strLoginKey = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<DateTime> loginTime = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                VariousData<int> enabled = WebDBManager.GetIntField(arrResult[i + 3].ToString());

                if (strUserID == null || strLoginKey == null || loginTime == null || enabled == null)
                    continue;

                long loginKey;

                if (long.TryParse(strLoginKey.Trim(), out loginKey) == false)
                    continue;

                ExternalLogin data = new ExternalLogin();

                data.UserID = strUserID;
                data.LoginKey = loginKey;
                data.LoginTime = loginTime.Data;
                data.Enabled = enabled.Data == 1;

                datas.Add(data);
            }

            return datas;
        }
    }
}
