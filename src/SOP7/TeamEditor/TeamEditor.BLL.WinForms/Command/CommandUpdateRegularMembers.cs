using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;

namespace TeamEditor.BLL.WinForms.Command
{
    // 전체 직원정보를 갱신하는 것이 아니라 m_dicRegularMembers에 있는 팀원들에 대해서만 변화를 준다.
    public class CommandUpdateRegularMembers : CommandEx
    {
        // 팀별 직원들
        private Dictionary<RegularTeam, List<CompanyMember>> m_dicRegularMembers = null;
        // 삭제될 이전 직원들의 목록
        private Dictionary<RegularTeam, List<CompanyMember>> m_dicRemovingOldCompanyMembers = null;
        private Dictionary<RegularTeam, List<CompanyMember>> m_dicNewCompanyMembers = new Dictionary<RegularTeam, List<CompanyMember>>();
        private Dictionary<RegularTeam, List<CompanyMember>> m_dicOldRegularMembers = null;

        private List<Team> m_newTemporaryNormalTeams = null;
        private List<Team> m_newTemporaryEmergencyTeams = null;
        private List<Team> m_removingOldTemporaryNormalTeams = null;
        private List<Team> m_removingOldTemporaryEmergencyTeams = null;
        // 추가될 주간 및 평일 자위소방대원 목록
        private Dictionary<Team, List<TemporaryMember>> m_dicNewTemporaryNormalMembers = null;
        // 추가될 야간 및 휴일 자위소방대원 목록
        private Dictionary<Team, List<TemporaryMember>> m_dicNewTemporaryEmergencyMembers = null;
        // 삭제될 주간 및 평일 자위소방대원 목록
        private Dictionary<Team, List<TemporaryMember>> m_dicRemovingOldTemporaryNormalMembers = null;
        // 삭제될 야간 및 휴일 자위소방대원 목록
        private Dictionary<Team, List<TemporaryMember>> m_dicRemovingOldTemporaryEmergencyMembers = null;

        private List<CompanyMember> m_changedCompanyMembers = new List<CompanyMember>();
        private List<CompanyMember> m_addedCompanyMembers = new List<CompanyMember>();

        private List<string> m_rollbackQueries = new List<string>();

        public Dictionary<RegularTeam, List<CompanyMember>> RegularMembers
        {
            get { return m_dicRegularMembers; }
            set { m_dicRegularMembers = value; }
        }

        public List<Team> NewTemporaryNormalTeams
        {
            set { m_newTemporaryNormalTeams = value; }
        }

        public List<Team> NewTemporaryEmergencyTeams
        {
            set { m_newTemporaryEmergencyTeams = value; }
        }

        public List<Team> RemovingOldTemporaryNormalTeams
        {
            set { m_removingOldTemporaryNormalTeams = value; }
        }

        public List<Team> RemovingOldTemporaryEmergencyTeams
        {
            set { m_removingOldTemporaryEmergencyTeams = value; }
        }

        // 추가될 주간 및 평일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> NewTemporaryNormalMembers
        {
            set { m_dicNewTemporaryNormalMembers = value; }
        }

        // 추가될 야간 및 휴일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> NewTemporaryEmergencyMembers
        {
            set { m_dicNewTemporaryEmergencyMembers = value; }
        }

        // 삭제될 주간 및 평일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> RemovingOldTemporaryNormalMembers
        {
            set { m_dicRemovingOldTemporaryNormalMembers = value; }
        }

        // 삭제될 야간 및 휴일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> RemovingOldTemporaryEmergencyMembers
        {
            set { m_dicRemovingOldTemporaryEmergencyMembers = value; }
        }

        public override void Do()
        {
            RemoveOldTemporaryMembers();
            AddNewTemporaryMembers();

            SaveOldMembers();
            DeleteOldMembers();

            LoadNewMembers();
        }

        public override void RollBack()
        {
            RemoveNewMembers();

            LoadOldMembers();

            RemoveNewTemporaryMembers();
            AddOldTemporaryMembers();
        }

        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            //dbMgr.BeginBatch();

            if (dir)
            {
                m_rollbackQueries.Clear();

                if (RemoveOldTemporaryMembers(dbMgr) == false)
                    goto ROLLBACK;

                if (AddNewTemporaryMembers(dbMgr) == false)
                    goto ROLLBACK;

                if (DeleteOldMembers(dbMgr) == false)
                    goto ROLLBACK;

                if (LoadNewMembers(dbMgr) == false)
                    goto ROLLBACK;
            }
            else
            {
                Rollback(dbMgr);

                RemoveCompanyMemberID(m_addedCompanyMembers);
                RollbackCompanyMember(m_changedCompanyMembers);
            }

            //dbMgr.BatchCommit();

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.COMPANY_MEMBER);
            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.REGULAR_TEAM);
            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER);

            return;

            ROLLBACK:
            Rollback(dbMgr);
            //dbMgr.BatchRollback();
        }

        private void RollbackCompanyMember(List<CompanyMember> members)
        {
            foreach (CompanyMember member in members)
            {
                CompanyMember _member = DataManager.GetCompanyMember(member.ID);

                if (_member != null)
                    _member.CopyFrom(member);
            }

            members.Clear();
        }

        private void RemoveCompanyMemberID(List<CompanyMember> members)
        {
            foreach (CompanyMember member in members)
            {
                member.ID = -1;
            }

            members.Clear();
        }

        private void Rollback(WebDBManager dbMgr)
        {
            int nRollbackCount = m_rollbackQueries.Count;

            for (int i = nRollbackCount - 1; i >= 0; i--)
            {
                string strSQL = m_rollbackQueries[i];

                if (dbMgr.GetResultData(strSQL) == null)
                {
                    MessageBox.Show("DB와의 접속이 끊어졌습니다.");
                    break;
                }
            }
        }

        private void SaveOldMembers()
        {
            if (m_dicOldRegularMembers != null)
                return;

            m_dicOldRegularMembers = new Dictionary<RegularTeam, List<CompanyMember>>();

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicRegularMembers)
            {
                RegularTeam team = DataManager.GetRegularTeam(pair.Key.TeamID);

                if (team != null)
                {
                    List<CompanyMember> members = DataManager.GetRegularMembers(team);
                    List<CompanyMember> copyMembers = new List<CompanyMember>();

                    foreach (CompanyMember member in members)
                    {
                        CompanyMember copyMember = new CompanyMember();
                        copyMember.CopyFrom(member);
                        copyMembers.Add(copyMember);
                    }

                    if (members != null)
                        m_dicOldRegularMembers[team] = copyMembers;
                }
            }
        }

        private void RollbackOldMembers()
        {
            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicRegularMembers)
            {
                RegularTeam team = DataManager.GetRegularTeam(pair.Key.TeamID);

                if (team != null)
                {
                    List<CompanyMember> members = DataManager.GetRegularMembers(team);
                    members.AddRange(pair.Value);
                }
            }

            m_dicRegularMembers.Clear();
            m_dicRegularMembers = null;
        }

        private bool DeleteOldMembers(WebDBManager dbMgr)
        {
            string strIDs = "";

            int nPrevIndex = m_rollbackQueries.Count;

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicRemovingOldCompanyMembers)
            {
                foreach (CompanyMember member in pair.Value)
                {
                    string strMemberID = member.MemberID == null || member.MemberID.Length == 0 ? "NULL" : "'" + member.MemberID + "'";
                    string strOfficePhoneNumber = member.OfficePhoneNumber == null || member.OfficePhoneNumber.Length == 0 ? "NULL" : "'" + member.OfficePhoneNumber + "'";
                    string strPhoneNumber = member.PhoneNumber == null || member.PhoneNumber.Length == 0 ? "NULL" : "'" + member.PhoneNumber + "'";
                    string strSubLevelID = member.SubJobLevel == null || member.SubJobLevel.ID < 0 ? "NULL" : member.SubJobLevel.ID.ToString();

                    if (strPhoneNumber != "NULL" && strPhoneNumber.Length > 0)
                        strPhoneNumber = "'" + DataManager.EncryptString(strPhoneNumber) + "'";

                    string strRollbackQuery = "Insert into CompanyMember (ID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber, SubLevelID) values (";
                    strRollbackQuery += string.Format("{0}, '{1}', {2}, {3}, {4}, {5}, {6})",
                        member.ID, member.Name, member.LevelID, strMemberID, strOfficePhoneNumber, strPhoneNumber, strSubLevelID);

                    string strSubPositionID = member.SubJobPosition == null || member.SubJobPosition.ID < 0 ? "NULL" : member.SubJobPosition.ID.ToString();
                    string strGroupID = member.GroupPosition == null || member.GroupPosition.ID < 0 ? "NULL" : member.GroupPosition.ID.ToString();

                    string strRollbackQuery2 = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values (";
                    strRollbackQuery2 += string.Format("{0}, {1}, {2}, {3}, {4})",
                        pair.Key.TeamID, member.ID, member.PositionID, strSubPositionID, strGroupID);

                    AddRollbackQuery(strRollbackQuery2);
                    AddRollbackQuery(strRollbackQuery);

                    if (strIDs.Length == 0)
                        strIDs = member.ID.ToString();
                    else
                        strIDs += ", " + member.ID.ToString();
                }
            }

            if (strIDs.Length > 0)
            {
                string strSQL = "Delete from RegularMemberList where CompanyMemberID in (" + strIDs + ")";

                if (dbMgr.GetResultData(strSQL) == null)
                {
                    BackToPrev(nPrevIndex);
                    return false;
                }

                string strSQL2 = "Delete from CompanyMember where ID in (" + strIDs + ")";

                if (dbMgr.GetResultData(strSQL2) == null)
                {
                    BackToPrev(nPrevIndex);
                    return false;
                }
            }

            return true;
        }

        private void DeleteOldMembers()
        {
            m_dicRemovingOldCompanyMembers = new Dictionary<RegularTeam, List<CompanyMember>>();

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicRegularMembers)
            {
                RegularTeam team = DataManager.GetRegularTeam(pair.Key.TeamID);

                if (team != null)
                {
                    List<CompanyMember> members = DataManager.GetRegularMembers(team);
                    int nMemberCount = members.Count;

                    for (int i=nMemberCount-1;i>=0;i--)
                    {
                        CompanyMember member = members[i];

                        if (GetCompanyMember(member.ID, pair.Value) == null)
                        {
                            List<CompanyMember> oldMembers = null;

                            if (m_dicRemovingOldCompanyMembers.TryGetValue(team, out oldMembers) == false)
                            {
                                oldMembers = new List<CompanyMember>();
                                m_dicRemovingOldCompanyMembers[team] = oldMembers;
                            }

                            oldMembers.Add(member);
                            members.RemoveAt(i);
                            DataManager.RemoveCompanyMember(member);
                        }
                    }
                }
            }
        }

        private CompanyMember GetCompanyMember(int nMemberID, List<CompanyMember> members)
        {
            foreach (CompanyMember member in members)
            {
                if (member.ID == nMemberID)
                    return member;
            }

            return null;
        }

        private bool LoadNewMembers(WebDBManager dbMgr)
        {
            string strSQL = "Select max(ID) from CompanyMember";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nID = 0;

            if (arrResult.Count == 0)
                nID = 1;
            else
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return false;

                nID = id.Data + 1;
            }

            string strIDs = "";

            foreach (CompanyMember member in m_addedCompanyMembers)
            {
                string strMemberID = member.MemberID == null || member.MemberID.Length == 0 ? "NULL" : "'" + member.MemberID + "'";
                string strOfficePhoneNumber = member.OfficePhoneNumber == null || member.OfficePhoneNumber.Length == 0 ? "NULL" : "'" + member.OfficePhoneNumber + "'";
                string strPhoneNumber = member.PhoneNumber == null || member.PhoneNumber.Length == 0 ? "NULL" : "'" + member.PhoneNumber + "'";
                string strSubLevelID = member.SubJobLevel == null || member.SubJobLevel.ID < 0 ? "NULL" : member.SubJobLevel.ID.ToString();

                if (strPhoneNumber != "NULL" && strPhoneNumber.Length > 0)
                    strPhoneNumber = "'" + DataManager.EncryptString(strPhoneNumber) + "'";

                string strInsert = "Insert into CompanyMember (ID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber, SubLevelID) values (";
                strInsert += string.Format("{0}, '{1}', {2}, {3}, {4}, {5}, {6})",
                    nID, member.Name, member.LevelID, strMemberID, strOfficePhoneNumber, strPhoneNumber, strSubLevelID);

                if (dbMgr.GetResultData(strInsert) == null)
                    return false;
                else
                {
                    string strRollbackQuery = "Delete from CompanyMember where ID = " + nID.ToString();
                    AddRollbackQuery(strRollbackQuery);
                }

                string strSubPositionID = member.SubJobPosition == null || member.SubJobPosition.ID < 0 ? "NULL" : member.SubJobPosition.ID.ToString();
                string strGroupID = member.GroupPosition == null || member.GroupPosition.ID < 0 ? "NULL" : member.GroupPosition.ID.ToString();

                string strInsert2 = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values (";
                strInsert2 += string.Format("{0}, {1}, {2}, {3}, {4})",
                    member.Team.TeamID, nID, member.PositionID, strSubPositionID, strGroupID);

                if (dbMgr.GetResultData(strInsert2) == null)
                    return false;
                else
                {
                    string strRollbackQuery = "Delete from RegularMemberList where CompanyMemberID = " + nID.ToString();
                    AddRollbackQuery(strRollbackQuery);
                }

                member.ID = nID++;

                if (strIDs.Length == 0)
                    strIDs = member.ID.ToString();
                else
                    strIDs += ", " + member.ID.ToString();
            }

            if (strIDs.Length > 0)
            {
                /*string strRollbackQuery = "Delete from RegularMemberList where CompanyMemberID in (" + strIDs + ")";
                AddRollbackQuery(strRollbackQuery);

                string strRollbackQuery2 = "Delete from CompanyMember where ID in (" + strIDs + ")";
                AddRollbackQuery(strRollbackQuery2);*/
            }

            foreach (CompanyMember member in m_changedCompanyMembers)
            {
                CompanyMember changedMember = DataManager.GetCompanyMember(member.ID);

                if (changedMember != null)
                {
                    string strUpdate = MakeUpdateQuery(changedMember);

                    if (dbMgr.GetResultData(strUpdate) == null)
                        return false;

                    string strRollbackQuery = MakeUpdateQuery(member);
                    AddRollbackQuery(strRollbackQuery);
                }
            }

            return true;
        }

        private string MakeUpdateQuery(CompanyMember member)
        {
            string strMemberID = member.MemberID == null || member.MemberID.Length == 0 ? "is NULL" : "= '" + member.MemberID + "'";
            string strOfficePhoneNumber = member.OfficePhoneNumber == null || member.OfficePhoneNumber.Length == 0 ? "is NULL" : "= '" + member.OfficePhoneNumber + "'";
            string strPhoneNumber = member.PhoneNumber == null || member.PhoneNumber.Length == 0 ? "is NULL" : "= '" + member.PhoneNumber + "'";
            string strSubLevelID = member.SubJobLevel == null ? "is NULL" : "= " + member.SubJobLevel.ID.ToString();

            string strUpdate = string.Format("Update CompanyMember Set MemberName = '{1}', LevelID = {2}, MemberID {3}, OfficePhoneNumber {4}, PhoneNumber {5}, SubLevelID {6} where ID = {0}",
                member.ID, member.Name, member.LevelID, strMemberID, strOfficePhoneNumber, strPhoneNumber, strSubLevelID);

            return strUpdate;
        }

        private void LoadNewMembers()
        {
            m_dicNewCompanyMembers.Clear();
            m_changedCompanyMembers.Clear();
            m_addedCompanyMembers.Clear();

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicRegularMembers)
            {
                List<CompanyMember> members = DataManager.GetRegularMembers(pair.Key);

                foreach (CompanyMember member in pair.Value)
                {
                    if (member.ID < 0)
                    {
                        // ID가 할당되지 않은 상태에서는 전체 직원정보에 포함시키지 않는다.
                        //DataManager.AddCompanyMember(member);
                        members.Add(member);

                        List<CompanyMember> newMembers = null;

                        if (m_dicNewCompanyMembers.TryGetValue(pair.Key, out newMembers) == false)
                        {
                            newMembers = new List<CompanyMember>();
                            m_dicNewCompanyMembers[pair.Key] = newMembers;
                        }

                        newMembers.Add(member);
                        m_addedCompanyMembers.Add(member);
                    }
                    else
                    {
                        // 기존에 존재하는 직원의 정보를 member의 정보로 바꾼다.
                        CompanyMember oldMember = DataManager.GetCompanyMember(member.ID);

                        if (oldMember != null)
                        {
                            CompanyMember _oldMember = new CompanyMember();
                            _oldMember.CopyFrom(oldMember);
                            m_changedCompanyMembers.Add(_oldMember);

                            oldMember.CopyFrom(member);
                        }
                    }
                }
            }

            RefreshCurrentTeam();
        }

        private void SelectFirstNode(TreeView tree)
        {
            foreach (TreeNode node in tree.Nodes)
            {
                tree.SelectedNode = node;
                break;
            }
        }

        private void RemoveNewMembers()
        {
            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicNewCompanyMembers)
            {
                RegularTeam team = DataManager.GetRegularTeam(pair.Key.TeamID);

                if (team != null)
                {
                    List<CompanyMember> members = DataManager.GetRegularMembers(team);

                    foreach (CompanyMember newMember in pair.Value)
                    {
                        members.Remove(newMember);

                        if (newMember.ID > 0)
                            DataManager.RemoveCompanyMember(newMember);
                    }
                }
            }

            m_dicNewCompanyMembers.Clear();
        }

        private void LoadOldMembers()
        {
            if (m_dicRemovingOldCompanyMembers == null)
                return;

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicRemovingOldCompanyMembers)
            {
                RegularTeam team = DataManager.GetRegularTeam(pair.Key.TeamID);

                if (team != null)
                {
                    List<CompanyMember> members = DataManager.GetRegularMembers(team);

                    foreach (CompanyMember oldMember in pair.Value)
                    {
                        members.Add(oldMember);
                        DataManager.AddCompanyMember(oldMember);
                    }
                }
            }

            m_dicRemovingOldCompanyMembers.Clear();
            m_dicRemovingOldCompanyMembers = null;

            RefreshCurrentTeam();
        }

        private void RefreshCurrentTeam()
        {
            TreeNode node = FormMain.Instance.RegularTeamTree.SelectedNode;

            if (node == null)
                return;

            if (node.Tag != null && node.Tag is RegularTeam)
            {
                RegularTeam team = (RegularTeam)node.Tag;
                FormMain.Instance.SelectRegularTeam(team, true);
            }
        }

        private void RemoveOldTemporaryMembers()
        {
            RemoveTemporaryMembers(m_dicRemovingOldTemporaryNormalMembers, true);
            RemoveTemporaryMembers(m_dicRemovingOldTemporaryEmergencyMembers, false);

            RemoveTemporaryTeams(m_removingOldTemporaryNormalTeams, true);
            RemoveTemporaryTeams(m_removingOldTemporaryEmergencyTeams, false);
        }

        private bool RemoveOldTemporaryMembers(WebDBManager dbMgr)
        {
            if (RemoveTemporaryMembers(m_dicRemovingOldTemporaryNormalMembers, true, dbMgr) == false)
                return false;
            if (RemoveTemporaryMembers(m_dicRemovingOldTemporaryEmergencyMembers, false, dbMgr) == false)
                return false;

            if (RemoveTemporaryTeams(m_removingOldTemporaryNormalTeams, true, dbMgr) == false)
                return false;
            if (RemoveTemporaryTeams(m_removingOldTemporaryEmergencyTeams, false, dbMgr) == false)
                return false;

            return true;
        }

        private bool AddNewTemporaryMembers(WebDBManager dbMgr)
        {
            if (AddTemporaryTeams(m_newTemporaryNormalTeams, true, dbMgr) == false)
                return false;
            if (AddTemporaryTeams(m_newTemporaryEmergencyTeams, false, dbMgr) == false)
                return false;

            if (AddTemporaryMembers(m_dicNewTemporaryNormalMembers, true, dbMgr) == false)
                return false;
            if (AddTemporaryMembers(m_dicNewTemporaryEmergencyMembers, false, dbMgr) == false)
                return false;

            return true;
        }

        private void AddNewTemporaryMembers()
        {
            AddTemporaryTeams(m_newTemporaryNormalTeams, true);
            AddTemporaryTeams(m_newTemporaryEmergencyTeams, false);

            AddTemporaryMembers(m_dicNewTemporaryNormalMembers, true);
            AddTemporaryMembers(m_dicNewTemporaryEmergencyMembers, false);
        }

        private void RemoveNewTemporaryMembers()
        {
            RemoveTemporaryMembers(m_dicNewTemporaryNormalMembers, true);
            RemoveTemporaryMembers(m_dicNewTemporaryEmergencyMembers, false);

            RemoveTemporaryTeams(m_newTemporaryNormalTeams, true);
            RemoveTemporaryTeams(m_newTemporaryEmergencyTeams, false);
        }

        private void AddOldTemporaryMembers()
        {
            AddTemporaryTeams(m_removingOldTemporaryNormalTeams, true);
            AddTemporaryTeams(m_removingOldTemporaryEmergencyTeams, false);

            AddTemporaryMembers(m_dicRemovingOldTemporaryNormalMembers, true);
            AddTemporaryMembers(m_dicRemovingOldTemporaryEmergencyMembers, false);
        }

        private bool AddTemporaryMembers(Dictionary<Team, List<TemporaryMember>> dicTemporaryMembers, bool isNormal, WebDBManager dbMgr)
        {
            if (dicTemporaryMembers == null)
                return true;

            string strSQL = "Select max(ID) from TemporaryMemberList";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nID = 0;

            if (arrResult.Count == 0)
                nID = 1;
            else
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return false;

                nID = id.Data + 1;
            }

            string strIDs = "";

            foreach (KeyValuePair<Team, List<TemporaryMember>> pair in dicTemporaryMembers)
            {
                foreach (TemporaryMember member in pair.Value)
                {
                    string strInsert = "Insert into TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) values (";
                    strInsert += string.Format("{0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        nID, member.DisplayName, pair.Key.TeamID, ToInt(isNormal), member.MemberID, ToInt(member.IsTeamLeader),
                        (int)member.TemporaryMemberType, member.MemberCount, (int)member.TemporaryManagerType);

                    if (dbMgr.GetResultData(strInsert) == null)
                        return false;
                    else
                    {
                        AddRollbackQuery("Delete from TemporaryMemberList where ID = " + nID.ToString());
                    }

                    member.ID = nID++;

                    if (strIDs.Length == 0)
                        strIDs = member.ID.ToString();
                    else
                        strIDs += ", " + member.ID.ToString();
                }
            }

            //if (strIDs.Length > 0)
            //    AddRollbackQuery("Delete from TemporaryMemberList where ID in (" + strIDs + ")");

            return true;
        }

        private void AddTemporaryMembers(Dictionary<Team, List<TemporaryMember>> dicTemporaryMembers, bool isNormal)
        {
            if (dicTemporaryMembers == null)
                return;

            if (isNormal)
            {
                foreach (KeyValuePair<Team, List<TemporaryMember>> pair in dicTemporaryMembers)
                {
                    List<TemporaryMember> members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)pair.Key);

                    if (members == null)
                    {
                        members = new List<TemporaryMember>();
                        DataManager.SetTemporaryNormalMembers((TemporaryNormalTeam)pair.Key, members);
                    }

                    members.AddRange(pair.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<Team, List<TemporaryMember>> pair in dicTemporaryMembers)
                {
                    List<TemporaryMember> members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)pair.Key);

                    if (members == null)
                    {
                        members = new List<TemporaryMember>();
                        DataManager.SetTemporaryEmergencyMembers((TemporaryEmergencyTeam)pair.Key, members);
                    }

                    members.AddRange(pair.Value);
                }
            }
        }

        private bool AddTemporaryTeams(List<Team> newTemporaryTeams, bool isNormal, WebDBManager dbMgr)
        {
            if (newTemporaryTeams == null)
                return true;

            string strTableName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strSQL = "Select max(ID) from " + strTableName;

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nID = 0;

            if (arrResult.Count == 0)
                nID = 1;
            else
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return false;

                nID = id.Data + 1;
            }

            string strIDs = "";

            foreach (Team team in newTemporaryTeams)
            {
                string strParentTeamID = "";

                if (isNormal)
                {
                    TemporaryNormalTeam normalTeam = (TemporaryNormalTeam)team;
                    strParentTeamID = normalTeam.ParentTeam == null ? "NULL" : normalTeam.ParentTeam.TeamID.ToString();
                }
                else
                {
                    TemporaryEmergencyTeam emergencyTeam = (TemporaryEmergencyTeam)team;
                    strParentTeamID = emergencyTeam.ParentTeam == null ? "NULL" : emergencyTeam.ParentTeam.TeamID.ToString();
                }

                string strGroupName = "NULL";
                string strLevelNo = "NULL";
                string strDescription = "NULL";
                string strRegularTeamLink = "NULL";

                string strInsertQuery = "Insert into " + strTableName + " (ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink, SiteID) values (";
                strInsertQuery += string.Format("{0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7})",
                    nID, team.TeamName, strParentTeamID, strGroupName, strLevelNo, strDescription, strRegularTeamLink, dbMgr.SiteID);

                if (dbMgr.GetResultData(strInsertQuery) == null)
                    return false;
                else
                {
                    string strRollbackQuery = "Delete from " + strTableName + " where ID = " + nID.ToString();
                    AddRollbackQuery(strRollbackQuery);
                }

                team.TeamID = nID++;

                if (strIDs.Length == 0)
                    strIDs = team.TeamID.ToString();
                else
                    strIDs += ", " + team.TeamID.ToString();
            }

            if (strIDs.Length == 0)
                return true;

            //string strRollbackQuery = "Delete from " + strTableName + " where ID in (" + strIDs + ")";
            //AddRollbackQuery(strRollbackQuery);

            return true;
        }

        private void AddTemporaryTeams(List<Team> newTemporaryTeams, bool isNormal)
        {
            if (newTemporaryTeams == null)
                return;

            TreeView teamTreeView = isNormal ? FormMain.Instance.TemporaryNormalTeamTree : FormMain.Instance.TemporaryEmergencyTeamTree;
            Dictionary<Team, Team> dicAddedTeams = new Dictionary<Team, Team>();

            int nAddedCount = 0;

            while (nAddedCount < newTemporaryTeams.Count)
            {
                int nCount = 0;

                foreach (Team team in newTemporaryTeams)
                {
                    if (dicAddedTeams.ContainsKey(team))
                        continue;

                    if (AddTemporaryTeam(team, isNormal, teamTreeView) != null)
                    {
                        dicAddedTeams[team] = team;
                        nCount++;
                    }

                    //DataManager.AddTeam(team);
                }

                // 실패
                if (nCount == 0)
                    return;
                else
                    nAddedCount += nCount;
            }
        }

        private TreeNode AddTemporaryTeam(Team team, bool isNormal, TreeView tree)
        {
            List<Team> teamList = new List<Team>();

            if (isNormal)
            {
                TemporaryNormalTeam _team = (TemporaryNormalTeam)team;
                teamList.Add(_team);

                while (_team.ParentTeam != null)
                {
                    _team = _team.ParentTeam;
                    teamList.Add(_team);
                }
            }
            else
            {
                TemporaryEmergencyTeam _team = (TemporaryEmergencyTeam)team;
                teamList.Add(_team);

                while (_team.ParentTeam != null)
                {
                    _team = _team.ParentTeam;
                    teamList.Add(_team);
                }
            }

            TreeNodeCollection nodes = tree.Nodes;
            int nTeamCount = teamList.Count;

            for (int i=nTeamCount-1;i>=0;i--)
            {
                Team _team = teamList[i];

                if (i == 0)
                {
                    TreeNode node = nodes.Add(_team.TeamName);
                    node.Tag = _team;
                    return node;
                }
                else
                {
                    TreeNode findNode = null;

                    foreach (TreeNode node in nodes)
                    {
                        if (node.Text == _team.TeamName)
                        {
                            findNode = node;
                            nodes = node.Nodes;
                            break;
                        }
                    }

                    if (findNode == null)
                        return null;
                }
            }

            return null;
        }

        private bool RemoveTemporaryTeams(List<Team> removingTemporaryTeams, bool isNormal, WebDBManager dbMgr)
        {
            if (removingTemporaryTeams == null)
                return true;

            string strTableName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strIDs = "";

            foreach (Team team in removingTemporaryTeams)
            {
                if (strIDs.Length == 0)
                    strIDs = team.TeamID.ToString();
                else
                    strIDs += ", " + team.TeamID.ToString();
            }

            if (strIDs.Length == 0)
                return true;

            string strSQL = "Select ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink, SiteID from " + strTableName + " where ID in (" + strIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nPrevIndex = m_rollbackQueries.Count;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-7;i+=8)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> parentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strGroupName = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<int> levelNo = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                string strDescription = WebDBManager.GetStringField(arrResult[i + 5]);
                string strRegularTeamLink = WebDBManager.GetStringField(arrResult[i + 6]);
                VariousData<int> siteID = WebDBManager.GetIntField(arrResult[i + 7].ToString());

                if (id == null || strTeamName == null || siteID == null)
                    continue;

                string strParentTeamID = parentTeamID == null ? "NULL" : parentTeamID.Data.ToString();
                strGroupName = strGroupName == null ? "NULL" : "'" + strGroupName + "'";
                string strLevelNo = levelNo == null ? "NULL" : levelNo.Data.ToString();
                strDescription = strDescription == null ? "NULL" : "'" + strDescription + "'";
                strRegularTeamLink = strRegularTeamLink == null ? "NULL" : "'" + strRegularTeamLink + "'";

                string strRollbackQuery = "Insert into " + strTableName + " (ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink, SiteID) values (";
                strRollbackQuery += string.Format("{0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7})",
                    id.Data, strTeamName, strParentTeamID, strGroupName, strLevelNo, strDescription, strRegularTeamLink, siteID.Data);

                AddRollbackQuery(strRollbackQuery);
            }

            strSQL = "Delete from " + strTableName + " where ID in (" + strIDs + ")";

            if (dbMgr.GetResultData(strSQL) == null)
            {
                BackToPrev(nPrevIndex);
                return false;
            }

            return true;
        }

        private void RemoveTemporaryTeams(List<Team> removingTemporaryTeams, bool isNormal)
        {
            if (removingTemporaryTeams == null)
                return;

            if (isNormal)
            {
                foreach (Team team in removingTemporaryTeams)
                {
                    DataManager.RemoveTemporaryNormalTeam(team.TeamID);
                    RemoveTemporaryTeam(team, isNormal, FormMain.Instance.TemporaryNormalTeamTree);
                }
            }
            else
            {
                foreach (Team team in removingTemporaryTeams)
                {
                    DataManager.RemoveTemporaryEmergencyTeam(team.TeamID);
                    RemoveTemporaryTeam(team, isNormal, FormMain.Instance.TemporaryEmergencyTeamTree);
                }
            }
        }

        private bool RemoveTemporaryTeam(Team team, bool isNormal, TreeView tree)
        {
            List<Team> teamList = new List<Team>();

            if (isNormal)
            {
                TemporaryNormalTeam _team = (TemporaryNormalTeam)team;
                teamList.Add(_team);

                while (_team.ParentTeam != null)
                {
                    _team = _team.ParentTeam;
                    teamList.Add(_team);
                }
            }
            else
            {
                TemporaryEmergencyTeam _team = (TemporaryEmergencyTeam)team;
                teamList.Add(_team);

                while (_team.ParentTeam != null)
                {
                    _team = _team.ParentTeam;
                    teamList.Add(_team);
                }
            }

            TreeNodeCollection nodes = tree.Nodes;
            int nTeamCount = teamList.Count;
            TreeNode findNode = null;

            for (int i = nTeamCount - 1; i >= 0; i--)
            {
                Team _team = teamList[i];

                //if (i == 0)
                {
                    //TreeNode node = nodes.Add(_team.TeamName);
                    //node.Tag = _team;
                    //return node;
                }
                //else
                {
                    findNode = null;

                    foreach (TreeNode node in nodes)
                    {
                        if (node.Text == _team.TeamName)
                        {
                            findNode = node;
                            nodes = node.Nodes;
                            break;
                        }
                    }

                    if (findNode == null)
                        return false;
                }
            }

            if (findNode == null)
                return false;

            findNode.Parent.Nodes.Remove(findNode);
            return true;
        }

        private void RemoveTemporaryMembers(Dictionary<Team, List<TemporaryMember>> dicRemovingOldTemporaryMembers, bool isNormal)
        {
            if (dicRemovingOldTemporaryMembers == null)
                return;

            foreach (KeyValuePair<Team, List<TemporaryMember>> pair in dicRemovingOldTemporaryMembers)
            {
                List<TemporaryMember> members = null;

                if (isNormal)
                {
                    members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)pair.Key);
                }
                else
                {
                    members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)pair.Key);
                }

                if (members != null)
                {
                    foreach (TemporaryMember member in pair.Value)
                    {
                        members.Remove(member);
                    }
                }
            }
        }

        private bool RemoveTemporaryMembers(Dictionary<Team, List<TemporaryMember>> dicRemovingOldTemporaryMembers, bool isNormal, WebDBManager dbMgr)
        {
            if (dicRemovingOldTemporaryMembers == null)
                return true;

            int nPrevIndex = m_rollbackQueries.Count;

            foreach (KeyValuePair<Team, List<TemporaryMember>> pair in dicRemovingOldTemporaryMembers)
            {
                string strIDs = "";

                foreach (TemporaryMember member in pair.Value)
                {
                    string strRollbackQuery = MakeTemporaryMemberInsertQuery(member, isNormal);
                    AddRollbackQuery(strRollbackQuery);

                    if (strIDs.Length == 0)
                        strIDs = member.ID.ToString();
                    else
                        strIDs += ", " + member.ID.ToString();
                }

                if (strIDs.Length == 0)
                    continue;

                string strSQL = "Delete from TemporaryMemberList where ID in (" + strIDs + ")";

                if (dbMgr.GetResultData(strSQL) == null)
                {
                    BackToPrev(nPrevIndex);
                    return false;
                }
            }

            return true;
        }

        private string MakeTemporaryMemberInsertQuery(TemporaryMember member, bool isNormal)
        {
            int nTemporaryTeamID = -1;

            if (isNormal)
            {
                TemporaryNormalMember normalMember = (TemporaryNormalMember)member;
                nTemporaryTeamID = normalMember.TemporaryTeam.TeamID;
            }
            else
            {
                TemporaryEmergencyMember emergencyMember = (TemporaryEmergencyMember)member;
                nTemporaryTeamID = emergencyMember.TemporaryTeam.TeamID;
            }

            string strMemberCount = member.MemberCount < 0 ? "NULL" : member.MemberCount.ToString();

            string strSQL = "Insert into TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) values (";
            strSQL += string.Format("{0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                member.ID, member.DisplayName, nTemporaryTeamID, ToInt(isNormal), member.MemberID, ToInt(member.IsTeamLeader),
                (int)member.TemporaryMemberType, strMemberCount, (int)member.TemporaryManagerType);

            return strSQL;
        }

        private int ToInt(bool flag)
        {
            return flag ? 1 : 0;
        }

        private void AddRollbackQuery(string strSQL)
        {
            m_rollbackQueries.Add(strSQL);
        }

        private void BackToPrev(int nPrevIndex)
        {
            // Rollback 해야 하므로 이전 상태까지만 복원시킨다.
            int nRemoveCount = m_rollbackQueries.Count - nPrevIndex;

            for (int i = nPrevIndex; i < nPrevIndex + nRemoveCount; i++)
            {
                m_rollbackQueries.RemoveAt(nPrevIndex);
            }
        }
    }
}
