using System;
using System.Collections.Generic;
using System.Text;
using Vacation.BLL.Models.Teams;
using Vacation.IDAL;
using Vacation.Model;

namespace Vacation.BLL
{
    public class TeamManager
    {
        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        private static List<RegularTeam> m_regularTeam = null;
        /// <summary>
        /// 팀 리스트
        /// </summary>
        public static List<RegularTeam> RegularTeam
        {
            get { return m_regularTeam; }
            set { m_regularTeam = value; }
        }

        private static List<JobLevel> m_jobLevel = null;
        /// <summary>
        /// 직급 리스트
        /// </summary>
        public static List<JobLevel> JobLevel
        {
            get { return m_jobLevel; }
            set { m_jobLevel = value; }
        }

        public TeamManager(IDataManager dataManager, ProcessManager processManager)
        {
            m_dataManager = dataManager;
            m_processManager = processManager;
        }

        public List<CompanyMemberData> LoadCompanyMember(int nTeamID = -1)
        {
            List<CompanyMemberData> datas = new List<CompanyMemberData>();
            List<CompanyMember> regularMembers;

            string strErrorMessage;
            if (nTeamID == -1)
                regularMembers = m_dataManager.GetSelectManager().SelectCompanyMembers(null, out strErrorMessage);
            else
            {
                Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
                dicConditions.Add(CompanyMember.Fields.TeamID, nTeamID);

                regularMembers = m_dataManager.GetSelectManager().SelectCompanyMembers(dicConditions, out strErrorMessage);
            }

            foreach (CompanyMember member in regularMembers)
            {
                CompanyMemberData data = new CompanyMemberData();
                data.CompanyMember = member;
                data.StartDate = member.StartDate.ToString("yyyy-MM-dd");

                // 팀
                foreach (RegularTeam team in m_regularTeam)
                {
                    if (member.TeamID == team.ID)
                    {
                        data.RegularTeam = team;
                        break;
                    }
                }

                // 직급
                if (m_jobLevel == null)
                    m_jobLevel = m_dataManager.GetSelectManager().SelectJobLevels(null, out strErrorMessage);
                
                foreach (JobLevel jobLevel in m_jobLevel)
                {
                    if (member.JobLevelID == jobLevel.ID)
                    {
                        data.JobLevel = jobLevel;
                        break;
                    }
                }

                datas.Add(data);
            }

            return datas;
        }

        public bool SaveMember(List<CompanyMemberData> data)
        {
            Dictionary<CompanyMember.Fields, object> dicUpdateColumn = new Dictionary<CompanyMember.Fields, object>();
            Dictionary<CompanyMember.Fields, object> dicCondition = new Dictionary<CompanyMember.Fields, object>();

            foreach (CompanyMemberData item in data)
            {
                if (item.CompanyMember.ID > 0)
                {
                    dicUpdateColumn.Clear();
                    dicUpdateColumn.Add(CompanyMember.Fields.Name, item.CompanyMember.Name);
                    dicUpdateColumn.Add(CompanyMember.Fields.PhoneNumber, item.CompanyMember.PhoneNumber);
                    //dicUpdateColumn.Add(CompanyMember.Fields.StartDate, item.CompanyMember.StartDate);
                    dicUpdateColumn.Add(CompanyMember.Fields.JobLevelID, item.JobLevel.ID);
                    dicUpdateColumn.Add(CompanyMember.Fields.IsTeamLeader, item.CompanyMember.IsTeamLeader);
                    dicUpdateColumn.Add(CompanyMember.Fields.IsAdmin, item.CompanyMember.IsAdmin);
                    //dicUpdateColumn.Add(CompanyMember.Fields.UserID, item.CompanyMember.UserID);

                    dicCondition.Clear();
                    dicCondition.Add(CompanyMember.Fields.ID, item.CompanyMember.ID);

                    string strErrorMessage;
                    m_dataManager.GetUpdateManager().UpdateCompanyMember(dicUpdateColumn, dicCondition, out strErrorMessage);
                }
                else // 신규 직원 추가
                {
                    m_dataManager.GetCreateManager().CreateCompanyMember(
                        item.CompanyMember.Name,
                        item.JobLevel.ID,
                        Convert.ToDateTime(item.StartDate),
                        item.RegularTeam.ID,
                        item.CompanyMember.IsTeamLeader,
                        item.CompanyMember.IsAdmin,
                        item.CompanyMember.UserID,
                        "",
                        "",
                        item.CompanyMember.PhoneNumber);
                }
            }

            return true;
        }

        public bool DeleteMember(List<CompanyMemberData> data)
        {
            foreach (CompanyMemberData item in data)
            {
                int nCompanyMemberID = item.CompanyMember.ID;
                if (nCompanyMemberID <= 0)
                    continue;

                string strErrorMessage;

                // FK 삭제
                // History
                Dictionary<History.Fields, object> dicManagerID = new Dictionary<History.Fields, object>();
                dicManagerID.Add(Vacation.Model.History.Fields.MemberID, nCompanyMemberID);
                m_dataManager.GetDeleteManager().DeleteHistory(dicManagerID, out strErrorMessage);

                // Response, Request
                Dictionary<Request.Fields, object> dicManagerID4 = new Dictionary<Request.Fields, object>();
                dicManagerID4.Add(Vacation.Model.Request.Fields.MemberID, nCompanyMemberID);
                List<Request> deleteRequests = m_dataManager.GetSelectManager().SelectRequests(dicManagerID4, out strErrorMessage);
                foreach (Request req in deleteRequests)
                {
                    Dictionary<Response.Fields, object> dicRequestID = new Dictionary<Response.Fields, object>();
                    dicRequestID.Add(Vacation.Model.Response.Fields.RequestID, req.ID);
                    m_dataManager.GetDeleteManager().DeleteResponse(dicRequestID, out strErrorMessage);
                }
                m_dataManager.GetDeleteManager().DeleteRequest(dicManagerID4, out strErrorMessage);

                Dictionary<Response.Fields, object> dicManagerID1 = new Dictionary<Response.Fields, object>();
                dicManagerID1.Add(Vacation.Model.Response.Fields.ManagerID, nCompanyMemberID);
                m_dataManager.GetDeleteManager().DeleteResponse(dicManagerID1, out strErrorMessage);

                // SpecialVacation
                Dictionary<SpecialVacation.Fields, object> dicManagerID2 = new Dictionary<SpecialVacation.Fields, object>();
                dicManagerID2.Add(Vacation.Model.SpecialVacation.Fields.MemberID, nCompanyMemberID);
                m_dataManager.GetDeleteManager().DeleteSpecialVacation(dicManagerID2, out strErrorMessage);

                Dictionary<SpecialVacationResponse.Fields, object> dicManagerID3 = new Dictionary<SpecialVacationResponse.Fields, object>();
                dicManagerID3.Add(Vacation.Model.SpecialVacationResponse.Fields.ManagerID, nCompanyMemberID);
                m_dataManager.GetDeleteManager().DeleteSpecialVacationResponse(dicManagerID3, out strErrorMessage);

                m_dataManager.GetDeleteManager().DeleteCompanyMember(nCompanyMemberID, out strErrorMessage);
            }

            return true;
        }

        public bool DeleteTeam(List<RegularTeam> data)
        {
            List<int> teamIDs = new List<int>();
            foreach (RegularTeam item in data)
                teamIDs.Add(item.ID);

            teamIDs.Reverse(); // 역순

            string strErrorMessage;
            // FK 삭제
            foreach (int teamID in teamIDs)
            {
                Dictionary<CompanyMember.Fields, object> dicCompanyMemberConditions = new Dictionary<CompanyMember.Fields, object>();
                dicCompanyMemberConditions.Add(CompanyMember.Fields.TeamID, teamID);

                List<CompanyMember> members = m_dataManager.GetSelectManager().SelectCompanyMembers(dicCompanyMemberConditions, out strErrorMessage);
                if (members != null)
                {
                    foreach (CompanyMember member in members)
                    {
                        Dictionary<Response.Fields, object> dicManagerID = new Dictionary<Response.Fields, object>();
                        dicManagerID.Add(Vacation.Model.Response.Fields.ManagerID, member.ID);
                        m_dataManager.GetDeleteManager().DeleteResponse(dicManagerID, out strErrorMessage);

                        m_dataManager.GetDeleteManager().DeleteCompanyMember(member.ID, out strErrorMessage);
                    }
                }

                m_dataManager.GetDeleteManager().DeleteRegularTeam(teamID, out strErrorMessage);
            }

            return true;
        }


        public bool UpdateRegularTeam(RegularTeam data)
        {
            string strErrorMessage;
            m_dataManager.GetUpdateManager().UpdateRegularTeam(data, out strErrorMessage);

            foreach (RegularTeam item in m_regularTeam)
            {
                if (item.ID == data.ID)
                {
                    item.Name = data.Name;
                    break;
                }
            }

            return true;
        }
    }
}
