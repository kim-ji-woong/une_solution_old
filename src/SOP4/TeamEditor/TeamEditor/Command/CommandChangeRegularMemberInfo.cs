using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace TeamEditor.Command
{
    // 사번과 전화번호 이외의 정보 변경
    public class CommandChangeRegularMemberInfo : CommandEx
    {
        public enum InfoType { Name = 1, Position, SubPosition, Level, SubLevel, PhoneNumber, GroupPosition, MemberID, OfficePhoneNumber, Unknown };

        private InfoType m_infoType = InfoType.Unknown;
        private object m_originData = null;
        private object m_changedData = null;
        private TeamGrid m_grid = null;
        private RegularTeam m_team = null;
        private CompanyMember m_member = null;
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

        public CompanyMember Member
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

        public CommandChangeRegularMemberInfo()
        {
        }

        public CommandChangeRegularMemberInfo(TeamGrid grid, CompanyMember member)
        {
            SetGrid(grid);
            m_member = member;
        }

        private void SetGrid(TeamGrid grid)
        {
            m_grid = grid;

            if (m_grid != null)
                m_team = (RegularTeam)m_grid.CurrentTeam;
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
                List<CompanyMember> members = DataManager.GetRegularMembers(m_team);

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
                List<CompanyMember> members = DataManager.GetRegularMembers(m_team);

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
                case InfoType.Name:
                    if (data == null)
                        m_member.Name = string.Empty;
                    else
                        m_member.Name = data.ToString();

                    break;

                case InfoType.Level:
                    if (data == null)
                        m_member.LevelID = -1;
                    else
                        m_member.LevelID = m_grid.GetLevelID(data.ToString());

                    break;

                case InfoType.SubLevel:
                    m_member.SubJobLevel = (CompanyMember.JobLevelSubInfo)data;
                    break;

                case InfoType.Position:
                    {
                        int nPositionID = -100;

                        if (data == null)
                            m_member.PositionID = nPositionID;
                        else if (DataManager.GetJobPositionID(data.ToString(), out nPositionID))
                            m_member.PositionID = nPositionID;
                        else
                            m_member.PositionID = nPositionID;
                    }
                    break;

                case InfoType.SubPosition:
                    m_member.SubJobPosition = (CompanyMember.JobPositionSubInfo)data;
                    break;

                case InfoType.GroupPosition:
                    m_member.GroupPosition = (CompanyMember.JobGroupPosition)data;
                    break;

                case InfoType.MemberID:
                    {
                        if (data is TeamGrid.MemberID)
                        {
                            TeamGrid.MemberID id = (TeamGrid.MemberID)data;
                            m_member.MemberID = id.ID;

                            DataManager.SetCompanyMemberMemberIDChanged(m_member, id.IsChanged);
                        }
                    }
                    break;

                case InfoType.OfficePhoneNumber:
                    if (data == null)
                        m_member.OfficePhoneNumber = null;
                    else
                        m_member.OfficePhoneNumber = data.ToString();

                    break;

                case InfoType.PhoneNumber:
                    {
                        if (data is TeamGrid.PhoneNumber)
                        {
                            TeamGrid.PhoneNumber phone = (TeamGrid.PhoneNumber)data;
                            m_member.PhoneNumber = phone.Number;

                            DataManager.SetCompanyMemberPhoneNumberChanged(m_member, phone.IsChanged);
                        }
                        else // if(data == null)
                        {
                            m_member.PhoneNumber = null;

                            DataManager.SetCompanyMemberPhoneNumberChanged(m_member, !DataManager.GetCompanyMemberPhoneNumberChanged(m_member));
                        }
                    }
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

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.COMPANY_MEMBER);
        }

        private void AddDB(DBUtility.WebDBManager dbMgr)
        {
            if (m_rollbackSQLs == null)
            {
                if (m_member == null || m_team == null || m_team.TeamID < 0)
                    return;

                if (DataManager.GetJobPositionName(m_member.PositionID) == null)
                {
                    m_member.PositionID = 0;
                }

                if (m_member.Name.Length == 0)
                {
                    MessageBox.Show("멤버 이름을 입력해야 합니다.");
                    return;
                }

                if (m_member.LevelID < 0)
                {
                    m_member.LevelID = 0;
                    //MessageBox.Show("입력된 멤버의 직급이 설정되지 않았습니다.");
                    //return;
                }

                //if (m_member.Name.Length == 0 || m_member.LevelID < 0)
                //    return;

                string strSQL = "Select max(ID) from CompanyMember";

                // Batch Job Start - Begin Transaction
                dbMgr.BeginBatch();

                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.BatchRollback();
                    return;
                }

                int nSubLevelID = DataManager.GetJobSubLevel(dbMgr, 1, m_member.SubJobLevel);
                int nSubPositionID = DataManager.GetJobSubPosition(dbMgr, 1, m_member.SubJobPosition);
                int nGroupPositionID = DataManager.GetGroupPosition(dbMgr, 1, m_member.GroupPosition);

                int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

                strSQL = "Insert into CompanyMember (ID, MemberName, LevelID, SubLevelID, MemberID, OfficePhoneNumber, PhoneNumber) values ";
                strSQL += string.Format("({0}, '{1}', {2}, {3}, {4}, {5}, {6})",
                    nID, m_member.Name, m_member.LevelID,
                    nSubLevelID < 0 ? "NULL" : nSubLevelID.ToString(),
                    m_member.MemberID == null || m_member.MemberID.Length == 0 ? "NULL" : "'" + m_member.MemberID + "'",
                    m_member.OfficePhoneNumber == null || m_member.OfficePhoneNumber.Length == 0 ? "NULL" : "'" + m_member.OfficePhoneNumber + "'",
                    m_member.PhoneNumber == null || m_member.PhoneNumber.Length == 0 ? "NULL" : "'" + DataManager.EncryptString(m_member.PhoneNumber) + "'");

                if (dbMgr.GetBatchData(strSQL) == null)
                {
                    dbMgr.BatchRollback();
                    return;
                }

                strSQL = string.Format("Select PositionID from RegularMemberList where RegularTeamID = {0} and CompanyMemberID = {1}",
                    m_team.TeamID, nID);

                arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.BatchRollback();
                    return;
                }

                if (arrResult.Count == 0)
                {
                    // 팀이 파트 팀인 경우에는 파트장이 실제DB에는 팀장으로 데이터가 입력되어 있으므로
                    // 파트장으로 되어있는 데이터를 다시 팀장으로 변환
                    int nSavePositionID = m_member.PositionID;

                    if (m_team.IsPartTeam)
                    {
                        if (String.Equals(DataManager.GetJobPositionName(m_member.PositionID), "파트장"))
                        {
                            if (DataManager.GetJobPositionID("팀장", out nSavePositionID) == false)
                                nSavePositionID = m_member.PositionID;
                        }
                    }

                    strSQL = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values ";
                    strSQL += string.Format("({0}, {1}, {2}, {3}, {4})",
                        m_team.TeamID, nID,
                        nSavePositionID,
                        nSubPositionID < 0 ? "NULL" : nSubPositionID.ToString(),
                        nGroupPositionID < 0 ? "NULL" : nGroupPositionID.ToString());

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

                    if (str.Contains("insert into companymember"))
                    {
                        ReadCompanyMemberInfo(dbMgr, str);
                    }
                }

                // Batch Job end - Commit
                dbMgr.BatchCommit();

                m_rollbackSQLs.Clear();
            }
        }

        private bool ReadCompanyMemberInfo(DBUtility.WebDBManager dbMgr, string strSQLLower)
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

                    CompanyMember member = DataManager.LoadCompanyMember(dbMgr, 1, nID);

                    if (member == null)
                        return false;

                    m_member.LevelID = member.LevelID;
                    m_member.SubJobLevel = member.SubJobLevel;
                    m_member.MemberID = member.MemberID;
                    m_member.Name = member.Name;
                    m_member.OfficePhoneNumber = member.OfficePhoneNumber;
                    m_member.PhoneNumber = member.PhoneNumber;
                    m_member.PositionID = member.PositionID;
                    m_member.SubJobPosition = member.SubJobPosition;
                    m_member.GroupPosition = member.GroupPosition;

                    return true;
                }
            }

            return false;
        }

        private void RemoveDB(DBUtility.WebDBManager dbMgr)
        {
            if (m_member.ID < 0)
            {
                m_rollbackSQLs = null;
                return;
            }

            CommandRemoveRegularMembers cmd = new CommandRemoveRegularMembers();

            string strCompanyMemberID = "(" + m_member.ID.ToString() + ")";
            m_rollbackSQLs = cmd.RemoveDB(dbMgr, strCompanyMemberID);

            if (m_rollbackSQLs != null)
            {
                m_member.ID = -1;
            }

            // TODO : mwkim 삭제된 데이터를 되돌릴 때에는 IsNewMember가 활성화 되어야 함.
            m_isNewMember = true;
        }

        private void UpdateDB(DBUtility.WebDBManager dbMgr)
        {
            if (m_member.ID < 0)
                return;

            string strSet = "";

            switch (m_infoType)
            {
                case InfoType.Name:
                    if (m_member.Name.Length < 0)
                        return;

                    strSet = "MemberName = '" + m_member.Name + "'";
                    break;

                case InfoType.Level:
                    if (m_member.LevelID < 0)
                        return;

                    strSet = "LevelID = " + m_member.LevelID.ToString();
                    break;

                case InfoType.SubLevel:
                    if (m_member.SubJobLevel == null)
                        strSet = "SubLevelID = NULL";
                    else
                    {
                        if (m_member.SubJobLevel.ID < 0)
                            m_member.SubJobLevel.ID = DataManager.GetJobSubLevel(dbMgr, 0, m_member.SubJobLevel);

                        strSet = "SubLevelID = " + m_member.SubJobLevel.ID.ToString();
                    }
                    break;

                case InfoType.Position:
                    {
                        if (DataManager.GetJobPositionName(m_member.PositionID) == null)
                            return;

                        int nNewPositionID = m_member.PositionID;
                        if (m_member.PositionID < 0 && m_member.PositionID > -100)
                        {
                            if (DataManager.GetJobPositionID("팀장", out nNewPositionID) == false)
                            {
                                nNewPositionID = m_member.PositionID;
                            }
                        }
                        
                        // 팀이 파트 팀인 경우에는 파트장이 실제DB에는 팀장으로 데이터가 입력되어 있으므로
                        // 파트장으로 되어있는 데이터를 다시 팀장으로 변환
                        int nSavePositionID = m_member.PositionID;

                        if (m_team.IsPartTeam)
                        {
                            if (String.Equals(DataManager.GetJobPositionName(m_member.PositionID), "파트장"))
                            {
                                if (DataManager.GetJobPositionID("팀장", out nSavePositionID))
                                    nNewPositionID = nSavePositionID;
                            }
                        }

                        string _strSQL = string.Format("Update RegularMemberList set PositionID = {0} where RegularTeamID = {1} and CompanyMemberID = {2}",
                            nNewPositionID,
                            m_team.TeamID,
                            m_member.ID);

                        dbMgr.GetResultData(_strSQL, 0);
                    }
                    return;

                case InfoType.SubPosition:
                    {
                        string _strSQL = "";

                        if (m_member.SubJobPosition == null)
                        {
                            _strSQL = string.Format("Update RegularMemberList set SubPositionID = NULL where RegularTeamID = {0} and CompanyMemberID = {1}",
                                m_team.TeamID, m_member.ID);
                        }
                        else
                        {
                            if (m_member.SubJobPosition.ID < 0)
                                m_member.SubJobPosition.ID = DataManager.GetJobSubPosition(dbMgr, 0, m_member.SubJobPosition);

                            _strSQL = string.Format("Update RegularMemberList set SubPositionID = {0} where RegularTeamID = {1} and CompanyMemberID = {2}",
                                m_member.SubJobPosition.ID, m_team.TeamID, m_member.ID);
                        }

                        dbMgr.GetResultData(_strSQL, 0);
                    }
                    return;

                case InfoType.GroupPosition:
                    {
                        string _strSQL = "";

                        if (m_member.GroupPosition == null)
                        {
                            _strSQL = string.Format("Update RegularMemberList set GroupPositionID = NULL where RegularTeamID = {0} and CompanyMemberID = {1}",
                                m_team.TeamID, m_member.ID);
                        }
                        else
                        {
                            if (m_member.GroupPosition.ID < 0)
                                m_member.GroupPosition.ID = DataManager.GetGroupPosition(dbMgr, 0, m_member.GroupPosition);

                            _strSQL = string.Format("Update RegularMemberList set GroupPositionID = {0} where RegularTeamID = {1} and CompanyMemberID = {2}",
                                m_member.GroupPosition.ID, m_team.TeamID, m_member.ID);
                        }

                        dbMgr.GetResultData(_strSQL, 0);
                    }
                    return;

                case InfoType.MemberID:
                    strSet = "MemberID = '" + m_member.MemberID + "'";
                    break;

                case InfoType.OfficePhoneNumber:
                    strSet = "OfficePhoneNumber = '" + m_member.OfficePhoneNumber + "'";
                    break;

                case InfoType.PhoneNumber:
                    if (m_member.PhoneNumber == null)
                        strSet = "PhoneNumber = ''";
                    else
                        strSet = "PhoneNumber = '" + DataManager.EncryptString(m_member.PhoneNumber) + "'";
                    break;

                default:
                    return;
            }

            string strSQL = "Update CompanyMember set " + strSet + " where ID = " + m_member.ID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            /*// DB 저장이 완료되면 보안이 필요한 데이터는 감춘다.
            if (arrResult != null)
            {
                if (m_infoType == InfoType.MemberID)
                {
                    if (m_cell.Value != null && m_cell.Value is TeamGrid.MemberID)
                    {
                        TeamGrid.MemberID id = (TeamGrid.MemberID)m_cell.Value;

                        m_cell.Tag = new TeamGrid.MemberID(id.ID, id.IsChanged);
                        id.IsChanged = false;
                    }
                }
                else if (m_infoType == InfoType.PhoneNumber)
                {
                    if (m_cell.Value != null && m_cell.Value is TeamGrid.PhoneNumber)
                    {
                        TeamGrid.PhoneNumber phone = (TeamGrid.PhoneNumber)m_cell.Value;

                        m_cell.Tag = new TeamGrid.PhoneNumber(phone.Number, phone.IsChanged);
                        phone.IsChanged = false;
                    }
                }
            }*/
        }

        public static InfoType ToInfoType(int nType)
        {
            if (nType < (int)InfoType.Name || nType >= (int)InfoType.Unknown)
                return InfoType.Unknown;

            return (InfoType)nType;
        }
    }
}
