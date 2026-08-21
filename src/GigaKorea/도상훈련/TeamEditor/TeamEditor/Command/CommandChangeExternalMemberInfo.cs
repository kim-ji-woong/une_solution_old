using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace TeamEditor.Command
{
    public class CommandChangeExternalMemberInfo : CommandEx
    {
        public enum InfoType { TeamName = 1, Name, Level, Position, PhoneNumber, Description, Unknown };

        private InfoType m_infoType = InfoType.Unknown;
        private object m_originData = null;
        private object m_changedData = null;
        private TeamGrid m_grid = null;
        private ExternalTeam m_team = null;
        private ExternalCompanyMember m_member = null;
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

        public ExternalCompanyMember Member
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

        public CommandChangeExternalMemberInfo(TeamGrid grid, ExternalCompanyMember member)
        {
            m_member = member;
            SetGrid(grid);            
        }

        private void SetGrid(TeamGrid grid)
        {
            m_grid = grid;

            if (m_grid != null)
            {
                if (m_member == null || m_member.Team == null)
                {
                    m_team = (ExternalTeam)m_grid.CurrentTeam;
                    m_member.Team = (ExternalTeam)m_grid.CurrentTeam;
                }
                else
                    m_team = m_member.Team;
            }
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
                List<ExternalCompanyMember> members = DataManager.GetExternalCompanyMembers(m_team);

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
                List<ExternalCompanyMember> members = DataManager.GetExternalCompanyMembers(m_team);

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
                case InfoType.TeamName:
                    m_member.Team = (ExternalTeam)data;
                    m_team = m_member.Team;
                    break;
                case InfoType.Name:
                    if (data == null)
                        m_member.Name = null;
                    else
                        m_member.Name = data.ToString();

                    break;

                case InfoType.Level:
                    m_member.ExternalJobLevel = data as ExternalCompanyMember.ExternalJobLevelInfo;
                    break;

                case InfoType.Position:
                    m_member.ExternalJobPosition = data as ExternalCompanyMember.ExternalJobPositionInfo;
                    break;

                case InfoType.PhoneNumber:
                    if (data is TeamGrid.PhoneNumber)
                    {
                        TeamGrid.PhoneNumber phone = (TeamGrid.PhoneNumber)data;
                        m_member.PhoneNumber = phone.Number;

                        DataManager.SetExternalCompanyMemberPhoneNumberChanged(m_member, phone.IsChanged);
                    }
                    else // if(data == null)
                    {
                        m_member.PhoneNumber = null;

                        DataManager.SetExternalCompanyMemberPhoneNumberChanged(m_member, !DataManager.GetExternalCompanyMemberPhoneNumberChanged(m_member));
                    }
                    break;

                case InfoType.Description:
                    if (data == null)
                        m_member.Description = null;
                    else
                        m_member.Description = data.ToString();
                    break;
            }
        }

        public override void SaveDB(DBUtility.WebDBManager dbMgr, bool dir)
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

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER);
        }

        private void UpdateDB(DBUtility.WebDBManager dbMgr)
        {
            if (m_member.ID < 0)
                return;

            string strSet = "";
            string _strSQL = "";

            switch (m_infoType)
            {
                case InfoType.TeamName:
                    string strUpdateTeam = "";
                    if (m_member.Team == null)
                        return;

                    strUpdateTeam = string.Format("Update ExternalMemberList SET ExternalCompanyTeamID = {0} WHERE ExternalCompanyMemberID = {1} ", m_member.Team.TeamID, m_member.ID);
                    dbMgr.GetResultData(strUpdateTeam, 0);
                    break;
                case InfoType.Name:
                    if (String.IsNullOrWhiteSpace(m_member.Name))
                        return;

                    strSet = "Name = '" + m_member.Name + "'";
                    break;

                case InfoType.Level:
                    if (m_member.ExternalJobLevel == null)
                    {
                        _strSQL = string.Format("Update ExternalMemberList set JobLevelID = NULL where ExternalCompanyTeamID = {0} and ExternalCompanyMemberID = {1}",
                            m_team.TeamID, m_member.ID);
                    }
                    else
                    {
                        if (m_member.ExternalJobLevel.ID < 0)
                            m_member.ExternalJobLevel.ID = GetExternalJobLevel(dbMgr, 0, m_member.ExternalJobLevel);

                        _strSQL = string.Format("Update ExternalMemberList set JobLevelID = {0} where ExternalCompanyTeamID = {1} and ExternalCompanyMemberID = {2}",
                            m_member.ExternalJobLevel.ID, m_team.TeamID, m_member.ID);
                    }

                    dbMgr.GetResultData(_strSQL, 0);
                    return;

                case InfoType.Position:
                    if (m_member.ExternalJobPosition == null)
                    {
                        _strSQL = string.Format("Update ExternalMemberList set JobPositionID = NULL where ExternalCompanyTeamID = {0} and ExternalCompanyMemberID = {1}",
                            m_team.TeamID, m_member.ID);
                    }
                    else
                    {
                        if (m_member.ExternalJobPosition.ID < 0)
                            m_member.ExternalJobPosition.ID = GetExternalJobPosition(dbMgr, 0, m_member.ExternalJobPosition);

                        _strSQL = string.Format("Update ExternalMemberList set JobPositionID = {0} where ExternalCompanyTeamID = {1} and ExternalCompanyMemberID = {2}",
                            m_member.ExternalJobPosition.ID, m_team.TeamID, m_member.ID);
                    }

                    dbMgr.GetResultData(_strSQL, 0);
                    return;

                case InfoType.PhoneNumber:
                    if (String.IsNullOrWhiteSpace(m_member.PhoneNumber))
                        strSet = "PhoneNumber = NULL";
                    else
                        strSet = "PhoneNumber = '" + DataManager.EncryptString(m_member.PhoneNumber) + "'";
                    break;

                case InfoType.Description:
                    if (String.IsNullOrWhiteSpace(m_member.Description))
                        strSet = "Description = NULL";
                    else
                        strSet = "Description = '" + m_member.Description + "'";
                    break;

                default:
                    return;
            }

            _strSQL = "Update ExternalCompanyMember set " + strSet + " where ID = " + m_member.ID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(_strSQL, 0);
        }

        private void RemoveDB(DBUtility.WebDBManager dbMgr)
        {
            if (m_member.ID < 0)
            {
                m_rollbackSQLs = null;
                return;
            }

            CommandRemoveExternalCompanyMembers cmd = new CommandRemoveExternalCompanyMembers();

            string strExternalMemberID = "(" + m_member.ID.ToString() + ")";
            m_rollbackSQLs = cmd.RemoveDB(dbMgr, strExternalMemberID);

            if (m_rollbackSQLs != null)
            {
                m_member.ID = -1;
            }

            m_isNewMember = true;
        }

        private string GetJobLevelIDString(DBUtility.WebDBManager dbMgr, string strJobLevel, int nTransaction)
        {
            if (strJobLevel == null || strJobLevel.Length == 0)
                return "NULL";

            string strSQL = "Select ID from ExternalJobLevel where LevelName = '" + strJobLevel + "'";
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return null;

            if (arrResult.Count != 0)
            {
                return DBUtility.WebDBManager.GetStringField(arrResult[0], "");
            }

            strSQL = "Select max(ID) from ExternalJobLevel";
            arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return null;

            int nID = 0;

            if (arrResult.Count == 0)
                nID = 1;
            else
                nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            if (nID <= 0)
                return null;

            strSQL = "Insert into ExternalJobLevel (ID, LevelName, Description) values ";
            strSQL += string.Format("({0}, '{1}', NULL)", nID, strJobLevel);

            if (nTransaction != 0)
            {
                if (dbMgr.GetBatchData(strSQL) == null)
                    return null;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return null;
            }
            //if (dbMgr.GetResultData(strSQL, nTransaction) == null)
            //    return null;

            return nID.ToString();
        }

        private string GetJobPositionIDString(DBUtility.WebDBManager dbMgr, string strJobPosition, int nTransaction)
        {
            if (strJobPosition == null || strJobPosition.Length == 0)
                return "NULL";

            string strSQL = "Select ID from ExternalJobPosition where PositionName = '" + strJobPosition + "'";
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return null;

            if (arrResult.Count != 0)
            {
                return DBUtility.WebDBManager.GetStringField(arrResult[0], "");
            }

            strSQL = "Select max(ID) from ExternalJobPosition";
            arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return null;

            int nID = 0;

            if (arrResult.Count == 0)
                nID = 1;
            else
                nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            if (nID <= 0)
                return null;

            strSQL = "Insert into ExternalJobPosition (ID, PositionName, Description) values ";
            strSQL += string.Format("({0}, '{1}', NULL)", nID, strJobPosition);

            if (nTransaction != 0)
            {
                if (dbMgr.GetBatchData(strSQL) == null)
                    return null;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return null;
            }
            //if (dbMgr.GetResultData(strSQL, nTransaction) == null)
            //    return null;

            return nID.ToString();
        }

        private int GetExternalJobLevel(DBUtility.WebDBManager dbMgr, int nTransaction, ExternalCompanyMember.ExternalJobLevelInfo externalJobLevel)
        {
            if (externalJobLevel == null)
                return -1;

            if (externalJobLevel.ID > 0)
                return externalJobLevel.ID;

            if (externalJobLevel.Name.Length == 0)
                return -1;

            int nID = GetMaxID(dbMgr, "ExternalJobLevel", nTransaction) + 1;

            if (nID == 0)
                return -1;

            string strSQL = string.Format("Insert into ExternalJobLevel (ID, LevelName) values ({0}, '{1}')", nID, externalJobLevel.Name);
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return -1;

            externalJobLevel.ID = nID;
            return nID;
        }

        private int GetExternalJobPosition(DBUtility.WebDBManager dbMgr, int nTransaction, ExternalCompanyMember.ExternalJobPositionInfo externalJobPosition)
        {
            if (externalJobPosition == null)
                return -1;

            if (externalJobPosition.ID > 0)
                return externalJobPosition.ID;

            if (externalJobPosition.Name.Length == 0)
                return -1;

            int nID = GetMaxID(dbMgr, "ExternalJobPosition", nTransaction) + 1;

            if (nID == 0)
                return -1;

            string strSQL = string.Format("Insert into ExternalJobPosition (ID, PositionName) values ({0}, '{1}')", nID, externalJobPosition.Name);
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return -1;

            externalJobPosition.ID = nID;
            return nID;
        }

        public static int GetMaxID(DBUtility.WebDBManager dbMgr, string strTableName, int nTransaction)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            return DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private void AddDB(DBUtility.WebDBManager dbMgr)
        {
            if (m_rollbackSQLs == null)
            {
                if (m_member == null || m_team == null || m_team.TeamID < 0)
                    return;

                if (m_member.Name.Length == 0)
                    return;

                string strSQL = "Select max(ID) from ExternalCompanyMember";

                // Batch Job Start - Begin Transaction
                dbMgr.BeginBatch();

                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.BatchRollback();
                    return;
                }

                int nExternalJobLevelID = GetExternalJobLevel(dbMgr, 1, m_member.ExternalJobLevel);
                int nExternalJobPositionID = GetExternalJobPosition(dbMgr, 1, m_member.ExternalJobPosition);

                int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

                strSQL = "Insert into ExternalCompanyMember (ID, Name, PhoneNumber, Description) values ";
                strSQL += string.Format("({0}, '{1}', {2}, {3})",
                    nID, m_member.Name,
                    m_member.PhoneNumber == null || m_member.PhoneNumber.Length == 0 ? "NULL" : "'" + DataManager.EncryptString(m_member.PhoneNumber) + "'",
                    m_member.Description == null || m_member.Description.Length == 0 ? "NULL" : "'" + m_member.Description + "'");

                arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.BatchRollback();
                    return;
                }

                strSQL = string.Format("Select JobPositionID from ExternalMemberList where ExternalCompanyTeamID = {0} and ExternalCompanyMemberID = {1}",
                    m_team.TeamID, nID);

                arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.BatchRollback();
                    return;
                }

                if (arrResult.Count == 0)
                {
                    strSQL = "Insert into ExternalMemberList (ExternalCompanyTeamID, ExternalCompanyMemberID, JobLevelID, JobPositionID) values ";
                    strSQL += string.Format("({0}, {1}, {2}, {3})",
                        m_team.TeamID, nID,
                        nExternalJobLevelID < 0 ? "NULL" : nExternalJobLevelID.ToString(),
                        nExternalJobPositionID < 0 ? "NULL" : nExternalJobPositionID.ToString());

                    if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        dbMgr.BatchRollback();
                        return;
                    }
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

                    string str = strSQL.ToLower();

                    if (str.Contains("insert into externalcompanymember"))
                    {
                        ReadExternalCompanyMemberInfo(dbMgr, str);
                    }
                }

                // Batch Job end - Commit
                dbMgr.BatchCommit();

                m_rollbackSQLs.Clear();
            }
        }

        private bool ReadExternalCompanyMemberInfo(DBUtility.WebDBManager dbMgr, string strSQLLower)
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
                        m_member.ID = nID;
                    else
                        return false;

                    ExternalCompanyMember member = DataManager.LoadExternalCompanyMember(dbMgr, 1, nID);

                    if (member == null)
                        return false;

                    m_member.Name = member.Name;
                    m_member.ExternalJobLevel = member.ExternalJobLevel;
                    m_member.ExternalJobPosition = member.ExternalJobPosition;
                    m_member.Description = member.Description;
                    
                    return true;
                }
            }

            return false;
        }

        public static InfoType ToInfoType(int nType)
        {
            if (nType < (int)InfoType.TeamName || nType >= (int)InfoType.Unknown)
                return InfoType.Unknown;

            return (InfoType)nType;
        }
    }
}
