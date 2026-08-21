using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Command
{
    /// <summary>
    /// 사용자 정의 조직
    /// </summary>
    public class CommandChangeUserDefinedTeamInfo : CommandEx
    {
        public enum InfoType { TeamName = 1, PhoneNumber, FaxNumber, Unknown };

        private InfoType m_infoType = InfoType.Unknown;
        private object m_originData = null;
        private object m_changedData = null;
        private TeamGrid m_grid = null;
        private UserDefinedTeam m_team = null;
        private bool m_isNewTeam = false;
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

        public UserDefinedTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public TeamGrid Grid
        {
            get { return m_grid; }
            set { m_grid = value; }
        }

        public bool IsNewTeam
        {
            get { return m_isNewTeam; }
            set { m_isNewTeam = value; }
        }

        public CommandChangeUserDefinedTeamInfo() { }

        public CommandChangeUserDefinedTeamInfo(TeamGrid grid, UserDefinedTeam team)
        {
            m_grid = grid;
            m_team = team;
        }

        public override void Do()
        {
            if (m_team == null || m_grid == null || m_infoType == InfoType.Unknown)
                return;

            SetMemberData(m_changedData);

            if (m_isNewTeam)
            {
                List<UserDefinedTeam> teams = DataManager.GetUserDefinedTeams();

                if (teams != null && !teams.Contains(m_team))
                    teams.Add(m_team);
            }

            m_grid.SelectTeam(null, true);
        }

        public override void RollBack()
        {
            if (m_team == null || m_grid == null || m_infoType == InfoType.Unknown)
                return;

            SetMemberData(m_originData);

            if (m_isNewTeam)
            {
                List<UserDefinedTeam> teams = DataManager.GetUserDefinedTeams();

                if (teams != null)
                    teams.Remove(m_team);
            }

            m_grid.SelectTeam(null, true);
        }

        public void SetMemberData(object data)
        {
            switch (m_infoType)
            {
                case InfoType.TeamName:
                    if (data == null)
                        m_team.TeamName = null;
                    else
                        m_team.TeamName = data.ToString();

                    break;

                case InfoType.PhoneNumber:
                    if (data == null)
                        m_team.PhoneNumber = null;
                    else
                        m_team.PhoneNumber = data.ToString();

                    break;

                case InfoType.FaxNumber:
                    if (data == null)
                        m_team.FaxNumber = null;
                    else
                        m_team.FaxNumber = data.ToString();

                    break;
            }
        }

        public override void SaveDB(DBUtility.WebDBManager dbMgr, bool dir)
        {
            if (m_team == null || m_infoType == InfoType.Unknown)
                return;

            if (m_isNewTeam)
            {
                if (dir)
                {
                    if (m_team.TeamID < 0)
                    {
                        AddDB(dbMgr);
                    }
                }
                else
                    RemoveDB(dbMgr);
            }
            else
                UpdateDB(dbMgr);

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.USER_DEFINED_TEAM);
        }

        private void AddDB(DBUtility.WebDBManager dbMgr)
        {
            if (m_rollbackSQLs == null)
            {
                if (m_team == null)
                    return;

                if (m_team.TeamName.Length == 0 )
                    return;

                string strSQL = "Select max(ID) from UserDefinedTeam";

                // Batch Job Start - Begin Transaction
                dbMgr.BeginBatch();

                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.BatchRollback();
                    return;
                }

                int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

                strSQL = "Insert into UserDefinedTeam (ID, TeamName, PhoneNumber, FaxNumber, SiteID) values ";
                strSQL += string.Format("({0}, '{1}', {2}, {3}, {4})",
                    nID,
                    m_team.TeamName,
                    m_team.PhoneNumber == null || m_team.PhoneNumber.Length == 0 ? "NULL" : "'" + m_team.PhoneNumber + "'",
                    m_team.FaxNumber == null || m_team.FaxNumber.Length == 0 ? "NULL" : "'" + m_team.FaxNumber + "'",
                    FormMain.Instance.SiteID);

                if (dbMgr.GetBatchData(strSQL) == null)
                {
                    dbMgr.BatchRollback();
                    return;
                }
                
                dbMgr.BatchCommit();
                m_team.TeamID = nID;
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

                    string str = strSQL.ToLower();

                    if (str.Contains("insert into UserDefinedTeam"))
                    {
                        ReadUserDefinedTeamInfo(dbMgr, str);
                    }
                }

                // Batch Job end - Commit
                dbMgr.BatchCommit();

                m_rollbackSQLs.Clear();
            }
        }

        private bool ReadUserDefinedTeamInfo(DBUtility.WebDBManager dbMgr, string strSQLLower)
        {
            string strTrg = "values (";
            int nIndex = strSQLLower.IndexOf(strTrg);

            if (nIndex > 0)
            {
                int nIndex2 = strSQLLower.IndexOf(',', nIndex + 1);

                if (nIndex2 > 0)
                {
                    int nTrgLen = strTrg.Length;
                    string strID = strSQLLower.Substring(nIndex + nTrgLen, nIndex2 - (nIndex + nTrgLen));

                    int nID;

                    if (int.TryParse(strID, out nID))
                        m_team.TeamID = nID;
                    else
                        return false;

                    UserDefinedTeam team = DataManager.LoadUserDefinedTeam(dbMgr, 1, nID);

                    if (team == null)
                        return false;

                    m_team.TeamName = team.TeamName;
                    m_team.PhoneNumber = team.PhoneNumber;
                    m_team.FaxNumber = team.FaxNumber;

                    return true;
                }
            }

            return false;
        }

        private void RemoveDB(DBUtility.WebDBManager dbMgr)
        {
            if (m_team.TeamID < 0)
            {
                m_rollbackSQLs = null;
                return;
            }

            CommandRemoveUserDefinedTeam cmd = new CommandRemoveUserDefinedTeam();

            string strUserDefinedTeamID = "(" + m_team.TeamID.ToString() + ")";
            m_rollbackSQLs = cmd.RemoveDB(dbMgr, strUserDefinedTeamID);

            if (m_rollbackSQLs != null)
            {
                m_team.TeamID = -1;
            }

            // TODO : mwkim 삭제된 데이터를 되돌릴 때에는 IsNewMember가 활성화 되어야 함.
            m_isNewTeam = true;
        }

        private void UpdateDB(DBUtility.WebDBManager dbMgr)
        {
            if (m_team.TeamID < 0)
                return;

            string strSet = "";

            switch (m_infoType)
            {
                case InfoType.TeamName:
                    if (String.IsNullOrWhiteSpace(m_team.TeamName))
                        return;

                    strSet = String.Format("MemberName = '{0}'", m_team.TeamName);
                    break;

                case InfoType.PhoneNumber:
                    strSet = String.Format("PhoneNumber = {0}", (String.IsNullOrWhiteSpace(m_team.PhoneNumber) ? "NULL" : String.Format("'{0}'", m_team.PhoneNumber)));
                    break;

                case InfoType.FaxNumber:
                    strSet = String.Format("FaxNumber = {0}", (String.IsNullOrWhiteSpace(m_team.FaxNumber) ? "NULL" : String.Format("'{0}'", m_team.FaxNumber)));
                    break;

                default:
                    return;
            }

            string strSQL = "Update UserDefinedTeam set " + strSet + " where ID = " + m_team.TeamID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

        }

        public static InfoType ToInfoType(int nType)
        {
            if (nType < (int)InfoType.TeamName || nType >= (int)InfoType.Unknown)
                return InfoType.Unknown;

            return (InfoType)nType;
        }

    }
}
