using System;
using System.Collections.Generic;
using System.Text;
using Vacation.Model;

namespace Vacation.BLL.Models.Vacation
{
    using Models.Account;

    public class MemberVacationHistory : MessageResult
    {
        private MemberTeam m_rootTeam = null;
        // 올해의 휴가이력
        // Key : ApplicationUser ID
        private Dictionary<int, Models.Vacation.History> m_dicMemberHistories = new Dictionary<int, History>();
        // 내년의 휴가이력
        private Dictionary<int, Models.Vacation.History> m_dicMemberHistoriesNextYear = new Dictionary<int, History>();
        // 작년의 휴가이력
        private Dictionary<int, Models.Vacation.History> m_dicMemberHistoriesLastYear = new Dictionary<int, History>();
        private int m_nMinimumYear = 0;

        public MemberTeam RootTeam
        {
            get { return m_rootTeam; }
            set { m_rootTeam = value; }
        }

        public List<int> MemberIDs
        {
            get
            {
                List<int> userIDs = new List<int>();

                foreach (KeyValuePair<int, Models.Vacation.History> pair in m_dicMemberHistories)
                {
                    userIDs.Add(pair.Key);
                }

                return userIDs;
            }
        }

        public List<Models.Vacation.History> MemberHistories
        {
            get
            {
                List<Models.Vacation.History> histories = new List<Models.Vacation.History>();

                foreach (KeyValuePair<int, Models.Vacation.History> pair in m_dicMemberHistories)
                {
                    histories.Add(pair.Value);
                }

                return histories;
            }
        }

        // 내년의 휴가이력
        public List<Models.Vacation.History> MemberHistoriesNextYear
        {
            get
            {
                List<Models.Vacation.History> histories = new List<Models.Vacation.History>();

                foreach (KeyValuePair<int, Models.Vacation.History> pair in m_dicMemberHistoriesNextYear)
                {
                    histories.Add(pair.Value);
                }

                return histories;
            }
        }

        // 작년의 휴가이력
        public List<Models.Vacation.History> MemberHistoriesLastYear
        {
            get
            {
                List<Models.Vacation.History> histories = new List<Models.Vacation.History>();

                foreach (KeyValuePair<int, Models.Vacation.History> pair in m_dicMemberHistoriesLastYear)
                {
                    histories.Add(pair.Value);
                }

                return histories;
            }
        }

        public int MinimumYear
        {
            get { return m_nMinimumYear; }
            set { m_nMinimumYear = value; }
        }

        /*public Dictionary<int, Models.Vacation.History> MemberHistories
        {
            get { return m_dicMemberHistories; }
        }*/

        // 올해의 휴가이력
        public void AddUserHistoryThisYear(int nUserID, Models.Vacation.History history)
        {
            m_dicMemberHistories[nUserID] = history;
        }

        // 내년의 휴가이력
        public void AddUserHistoryNextYear(int nUserID, Models.Vacation.History history)
        {
            m_dicMemberHistoriesNextYear[nUserID] = history;
        }

        // 작년의 휴가이력
        public void AddUserHistoryLastYear(int nUserID, Models.Vacation.History history)
        {
            m_dicMemberHistoriesLastYear[nUserID] = history;
        }

        public void RemoveUserHistory(int nUserID)
        {
            m_dicMemberHistories.Remove(nUserID);
            m_dicMemberHistoriesNextYear.Remove(nUserID);
            m_dicMemberHistoriesLastYear.Remove(nUserID);
        }
    }

    public class MemberTeam
    {
        private int m_nID = -1;
        private string m_strTeamName = "";
        private List<MemberTeam> m_childTeams = new List<MemberTeam>();
        private List<ApplicationUser> m_members = new List<ApplicationUser>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public List<MemberTeam> ChildTeams
        {
            get { return m_childTeams; }
        }

        public List<ApplicationUser> Members
        {
            get { return m_members; }
        }
    }
}
