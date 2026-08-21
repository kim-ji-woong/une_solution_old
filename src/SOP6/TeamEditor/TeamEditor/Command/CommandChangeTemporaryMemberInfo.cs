using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using DBUtility2;

namespace TeamEditor.Command
{
    public class CommandChangeTemporaryMemberInfo : CommandEx
    {
        public enum InfoType { ManagerType = 1, DisplayName = 2, Position = 5, Member = 6, MemberCount = 8, IncludeChildTeams = 9, IncludeChildTeams2 = 10, Unknown = 11 };

        private InfoType m_infoType = InfoType.Unknown;
        private object m_originData = null;
        private object m_changedData = null;
        private TeamGrid m_grid = null;
        private Team m_team = null;
        private TemporaryMember m_member = null;
        private bool m_isNewMember = false;
        private List<string> m_rollbackSQLs = null;

        public InfoType DataType
        {
            get { return m_infoType; }
            set { m_infoType = value; }
        }

        public object Origin
        {
            get { return m_originData; }
            set { m_originData = value; }
        }

        public object Changed
        {
            get { return m_changedData; }
            set { m_changedData = value; }
        }

        public TemporaryMember Member
        {
            get { return m_member; }
            set { m_member = value; }
        }

        public TeamGrid Grid
        {
            get { return m_grid; }
            set { SetGrid(value); }
        }

        public bool IsNewMember
        {
            get { return m_isNewMember; }
            set { m_isNewMember = value; }
        }

        public CommandChangeTemporaryMemberInfo()
        {
        }

        public CommandChangeTemporaryMemberInfo(TeamGrid grid, TemporaryMember member)
        {
            SetGrid(grid);
            m_member = member;
        }

        private void SetGrid(TeamGrid grid)
        {
            m_grid = grid;

            if (m_grid != null)
                m_team = m_grid.CurrentTeam;
            else
                m_team = null;
        }

        public override void Do()
        {
            if (m_member == null || m_team == null || m_grid == null || m_infoType == InfoType.Unknown)
                return;

            SetMemberData(m_changedData);

            if (m_isNewMember)
            {
                List<TemporaryMember> members = null;

                if (m_team is TemporaryNormalTeam)
                {
                    members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)m_team);

                    if (members == null)
                    {
                        members = new List<TemporaryMember>();
                        DataManager.SetTemporaryNormalMembers((TemporaryNormalTeam)m_team, members);
                    }
                }
                else if (m_team is TemporaryEmergencyTeam)
                {
                    members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_team);

                    if (members == null)
                    {
                        members = new List<TemporaryMember>();
                        DataManager.SetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_team, members);
                    }
                }

                if (members == null)
                    return;

                if (members != null && !members.Contains(m_member))
                    members.Add(m_member);
            }

            if (m_grid.CurrentTeam == m_team)
                m_grid.SelectTeam(m_team, true);
        }

        public override void RollBack()
        {
            if (m_member == null || m_team == null || m_grid == null || m_infoType == InfoType.Unknown)
                return;

            SetMemberData(m_originData);

            if (m_isNewMember)
            {
                List<TemporaryMember> members = null;

                if (m_team is TemporaryNormalTeam)
                    members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)m_team);
                else if (m_team is TemporaryEmergencyTeam)
                    members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_team);

                if (members != null)
                    members.Remove(m_member);
            }

            if (m_grid.CurrentTeam == m_team)
                m_grid.SelectTeam(m_team, true);
        }

        public void SetMemberData(object data)
        {
            switch (m_infoType)
            {
                case InfoType.ManagerType:
                    if (data == null)
                        m_member.TemporaryManagerType = TemporaryMember.ManagerType.NONE;
                    else
                    {
                        string strManagerType = data.ToString();
                        m_member.TemporaryManagerType = TemporaryMember.ToManagerType(strManagerType);
                    }
                    break;

                case InfoType.DisplayName:
                    m_member.DisplayName = data == null ? "" : data.ToString();
                    break;

                case InfoType.Member:
                    SetTemporaryMember(m_member, data);
                    break;

                case InfoType.Position:
                    if (data == null)
                    {
                        m_member.IsTeamLeader = false;
                        m_member.MemberCount = -1;
                    }
                    else
                    {
                        // 직위 정보는 IsTeamLeader와 MemberCount의 조합으로 이루어진다.
                        TemporaryMember member = (TemporaryMember)data;
                        m_member.IsTeamLeader = member.IsTeamLeader;
                        m_member.MemberCount = member.MemberCount;
                    }
                    break;

                case InfoType.MemberCount:
                    if (data == null)
                        m_member.MemberCount = -1;
                    else
                        m_member.MemberCount = (int)data;
                    break;

                case InfoType.IncludeChildTeams:
                    if (data == null)
                        m_member.IncludeChildTeam = false;
                    else
                    {
                        bool isIncludeChildTeams = (bool)data;
                        m_member.IncludeChildTeam = isIncludeChildTeams;
                    }
                    break;
            }
        }

        private bool SetTemporaryMember(TemporaryMember member, object data)
        {
            if (data != null)
            {
                object[] objArr = data as object[];

                if (objArr[0] is RegularTeam)
                {
                    member.Team = (RegularTeam)objArr[0];

                    if (objArr[1] is CompanyMember)
                    {
                        member.TemporaryMemberType = TeamEditor.TemporaryMember.MemberType.CompanyMember;
                        member.CompanyMember = (CompanyMember)objArr[1];
                    }
                    else
                    {
                        member.TemporaryMemberType = TeamEditor.TemporaryMember.MemberType.RegularTeam;
                    }
                }
                else if (objArr[0] is ExternalTeam)
                {
                    member.Team = (ExternalTeam)objArr[0];

                    if (objArr[1] is ExternalCompanyMember)
                    {
                        member.TemporaryMemberType = TeamEditor.TemporaryMember.MemberType.ExternalCompanyMember;
                        member.ExternalCompanyMember = (ExternalCompanyMember)objArr[1];
                    }
                    else
                    {
                        member.TemporaryMemberType = TeamEditor.TemporaryMember.MemberType.ExternalTeam;
                    }
                }
                else if (objArr[0] is UserDefinedTeam)
                {
                    member.TemporaryMemberType = TeamEditor.TemporaryMember.MemberType.UserDefinedTeam;
                    member.Team = (UserDefinedTeam)objArr[0];
                }
                else if (objArr[1] is int)
                {
                    member.TemporaryMemberType = TeamEditor.TemporaryMember.MemberType.LevelID;
                    member.LevelID = (int)objArr[1];
                }
                else
                    return false;


                member.IsTeamLeader = Convert.ToBoolean(objArr[2]);
                member.MemberCount = Convert.ToInt32(objArr[3]);
            }
            else
                return false;

            return true;
        }

        public void SetChangedData(TemporaryMember memberOrigin, object changedData)
        {
            if (memberOrigin == null)
                return;

            switch (m_infoType)
            {
                case InfoType.ManagerType:
                    m_originData = TemporaryMember.GetManagerTypeString(memberOrigin.TemporaryManagerType);
                    break;

                case InfoType.DisplayName:
                    m_originData = memberOrigin.DisplayName;
                    break;

                case InfoType.Member:
                    m_originData = new object[] { 
                        memberOrigin.Team,
                        memberOrigin.Member,
                        memberOrigin.IsTeamLeader,
                        memberOrigin.MemberCount
                    };
                    break;

                case InfoType.Position:
                    {
                        TemporaryMember member = new TemporaryMember();
                        member.IsTeamLeader = memberOrigin.IsTeamLeader;
                        member.MemberCount = memberOrigin.MemberCount;

                        m_originData = member;
                    }
                    break;

                case InfoType.MemberCount:
                    m_member.MemberCount = memberOrigin.MemberCount;

                    if (memberOrigin.MemberCount < 0)
                        m_originData = null;
                    else
                        m_originData = memberOrigin.MemberCount;
                    break;

                case InfoType.IncludeChildTeams:
                    m_originData = memberOrigin.IncludeChildTeam;
                    break;

                default:
                    return;
            }

            m_changedData = changedData;
        }

        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            if (m_member == null || m_team == null || m_infoType == InfoType.Unknown)
                return;

            if (m_isNewMember)
            {
                if (dir)
                {
                    if (m_member.ID == -1)
                    {
                        AddDB(dbMgr);
                    }
                }
                else
                    RemoveDB(dbMgr);
            }
            else
                UpdateDB(dbMgr);

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER);
        }

        private void UpdateDB(WebDBManager dbMgr)
        {
            if (m_member.ID < 0)
                return;

            string strSet = "";

            switch (m_infoType)
            {
                case InfoType.DisplayName:
                    strSet = "MemberName = '" + m_member.DisplayName + "'";
                    break;

                case InfoType.IncludeChildTeams:
                    strSet = "MemberID = " + m_member.MemberID.ToString();
                    break;

                case InfoType.ManagerType:
                    if (m_member.TemporaryManagerType == TemporaryMember.ManagerType.NONE)
                        strSet = "Role = NULL";
                    else
                        strSet = "Role = " + ((int)m_member.TemporaryManagerType).ToString();
                    break;

                case InfoType.Member:
                    {
                        string strMemberCount;
                        string strTeamLeader = GetTeamLeaderDBString(m_member, out strMemberCount);
                        
                        strSet = "MemberType = " + ((int)m_member.TemporaryMemberType).ToString() + ", MemberID = " + m_member.MemberID.ToString() + ", IsTeamLeader = " + strTeamLeader + ", MemberCount = " + strMemberCount;
                    }
                    break;

                case InfoType.MemberCount:
                case InfoType.Position:
                    {
                        string strMemberCount;
                        string strTeamLeader = GetTeamLeaderDBString(m_member, out strMemberCount);
                        
                        strSet = "IsTeamLeader = " + strTeamLeader + ", MemberCount = " + strMemberCount;
                    }
                    break;

                default:
                    return;
            }

            string strSQL = "Update TemporaryMemberList set " + strSet + " where ID = " + m_member.ID.ToString();
            dbMgr.GetResultData(strSQL);
        }

        private void RemoveDB(WebDBManager dbMgr)
        {
            if (m_member.ID < 0)
            {
                m_rollbackSQLs = null;
                return;
            }

            CommandRemoveTemporaryMembers cmd = new CommandRemoveTemporaryMembers();

            string strTemporaryMemberID = "(" + m_member.ID.ToString() + ")";
            m_rollbackSQLs = cmd.RemoveDB(dbMgr, strTemporaryMemberID);

            if (m_rollbackSQLs != null)
                m_member.ID = -1;
        }

        private string GetTeamLeaderDBString(TemporaryMember member, out string strMemberCount)
        {
            strMemberCount = "NULL";

            if (member.TemporaryMemberType == TemporaryMember.MemberType.RegularTeam ||
                member.TemporaryMemberType == TemporaryMember.MemberType.ExternalTeam ||
                member.TemporaryMemberType == TemporaryMember.MemberType.UserDefinedTeam)
            {
                if (member.IsTeamLeader)
                    return "1";
                else
                {
                    strMemberCount = member.MemberCount < 0 ? "NULL" : member.MemberCount.ToString();
                    return "0";
                }
            }

            return "NULL";
        }

        private void AddDB(WebDBManager dbMgr)
        {
            if (m_rollbackSQLs == null)
            {
                if (m_member == null || m_team == null || m_team.TeamID < 0)
                    return;

                string strMemberName = m_member.DisplayName.Length > 0 ? m_member.DisplayName : (m_member.GetMemberRealName().Length > 0 ? m_member.GetMemberRealName() : m_member.GetTeamRealName());
                int nTeamID = m_team.TeamID;
                bool isNormal = m_team is TemporaryNormalTeam;
                int nMemberID = m_member.MemberID;
                TemporaryMember.MemberType memberType = m_member.TemporaryMemberType;
                TemporaryMember.ManagerType managerType = m_member.TemporaryManagerType;

                string strMemberCount;
                string strTeamLeader = GetTeamLeaderDBString(m_member, out strMemberCount);

                if (strMemberName.Length == 0 || nTeamID < 0 || memberType == TemporaryMember.MemberType.None)
                    return;

                // Batch Job Start - Begin Transaction
                dbMgr.BeginBatch();

                int nID = DataManager.GetMaxID(dbMgr, "TemporaryMemberList", 1) + 1;

                if (nID <= 0)
                {
                    dbMgr.BatchRollback();
                    return;
                }

                string strFormat = "Insert into TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) ";
                strFormat += "values ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})";

                string strSQL = string.Format(strFormat, nID, strMemberName, nTeamID,
                    isNormal ? 1 : 0,
                    nMemberID, strTeamLeader, (int)memberType, strMemberCount,
                    managerType == TemporaryMember.ManagerType.NONE ? "NULL" : ((int)managerType).ToString());

                if (dbMgr.GetBatchData(strSQL) == null)
                {
                    dbMgr.BatchRollback();
                    return;
                }

                dbMgr.BatchCommit();
                m_member.ID = nID;
            }
            else
            {
                // Batch Job Start - Begin Transaction
                dbMgr.BeginBatch();

                foreach (string strSQL in m_rollbackSQLs)
                {
                    ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                    if (arrResult == null)
                    {
                        // Rollback
                        dbMgr.BatchRollback();
                        return;
                    }
                }

                // Batch Job end - Commit
                dbMgr.BatchCommit();

                m_rollbackSQLs.Clear();
            }
        }

        public static InfoType ToInfoType(int nType)
        {
            if (nType < (int)InfoType.ManagerType || nType >= (int)InfoType.Unknown)
                return InfoType.Unknown;

            return (InfoType)nType;
        }
    }
}
