using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor.Command
{
    public class CommandPasteRegularMember : CommandEx
    {
        private TeamTreeView m_tree = null;
        private TeamGrid m_grid = null;
        private string[] m_strPastedRows = null;
        private int m_nRowMinIdx = -1;
        private int m_nColMinIdx = -1;
        private List<CompanyMember> newMembers = new List<CompanyMember>();
        //private List<CompanyMember> chgMember = new List<CompanyMember>();
        private Dictionary<int, List<ChangeMemberInfo>> chgMembers = new Dictionary<int, List<ChangeMemberInfo>>();
        private Dictionary<int, TreeNode> newTeams = new Dictionary<int, TreeNode>();
        /// <summary>
        /// 팀 저장할때 변경된 TeamID 기록 (ex: -1 -> 7)
        /// </summary>
        private Dictionary<int, int> saveTeamID = new Dictionary<int, int>();
        private Dictionary<int, int> saveMemberID = new Dictionary<int, int>();
         
        /// <summary>
        /// 붙여넣기 작업이 성공했는지 판단
        /// </summary>
        public bool IsPasted
        {
            get { return m_IsPasted; }
        }
        private bool m_IsPasted = false;
         
        /// <summary>
        /// 첫 붙여넣기 작업인지 판단 Undo X
        /// </summary>
        public bool IsFirst
        {
            get { return m_IsFirst; }
        }
        private bool m_IsFirst = true;

        private List<string> m_rollbackSQLs = new List<string>();

        /// <summary>
        /// Key -> MemberID
        /// </summary>
        private Dictionary<int, RegularTeam> test = new Dictionary<int, RegularTeam>();

        public CommandPasteRegularMember(TeamTreeView tree, TeamGrid grid, string[] strPastedRows, int rowMinIdx, int colMinIdx)
        {
            this.m_tree = tree;
            this.m_grid = grid;
            this.m_strPastedRows = strPastedRows;
            this.m_nRowMinIdx = rowMinIdx;
            this.m_nColMinIdx = colMinIdx;
        }

        public override void Do()
        { 
            if (m_IsFirst)
            { 
                int rowIndex = m_nRowMinIdx;
                bool isSuc = false;

                for (int row = 0; row < m_strPastedRows.Length; row++)
                {
                    string[] pastedRowCells = m_strPastedRows[row].Split(new char[] { '\t' });
                    string pastedRowLength = string.Join("", pastedRowCells);
                    if (pastedRowLength.Length == 0)
                        continue;

                    int columnIndex = m_nColMinIdx;

                    //if (rowIndex + 1 >= m_grid.Rows.Count && m_strPastedRows.Length > 1) // 클립보드에 한줄만 있을 경우에는 추가 안해도됨
                    //    m_grid.Rows.Add();
                    if (m_grid.Rows.Count - 1 <= rowIndex)
                        m_grid.Rows.Add();
                    DataGridViewRow curRow = m_grid.Rows[rowIndex];

                    bool isNewMember = false;
                    CompanyMember member = null;
                    if (curRow.Tag == null)
                    {
                        isNewMember = true;
                        member = new CompanyMember();
                    }
                    else
                    {
                        CompanyMember old = (CompanyMember)curRow.Tag;
                        member = new CompanyMember();
                        member.CopyFrom(old);
                        //DataManager.RemoveCompanyMember(member);
                    }

                    List<ChangeMemberInfo> chgMemberInfos = new List<ChangeMemberInfo>();

                    for (int cell = 0; cell < pastedRowCells.Length; cell++)
                    {
                        if (curRow.Cells.Count <= columnIndex)
                            break;

                        DataGridViewCell curCell = curRow.Cells[columnIndex];
                        object curOrgTag = curCell.Tag;

                        string pastedValue = pastedRowCells[cell].Trim();

                        // 기존 Member 변경된 항목 저장 변수
                        ChangeMemberInfo chgMemberInfo = new ChangeMemberInfo();

                        if (DoPasted(columnIndex, pastedValue, curCell, member, curOrgTag, chgMemberInfo))
                        {
                            isSuc = true;
                            chgMemberInfos.Add(chgMemberInfo);
                            chgMemberInfo.Member = member;
                        }
                        columnIndex++;
                    }

                    // 직원 임시ID 부여 - 붙여넣기한 순서대로 정렬하기 위해서
                    if (isNewMember)
                    {
                        int memberID = DataManager.NoSaveMemberID();
                        member.ID = memberID;
                        curRow.Tag = member;
                        newMembers.Add(member);
                    }
                    else
                    {
                        if (chgMemberInfos.Count > 0)
                            chgMembers[member.ID] = chgMemberInfos;
                    }

                    if (isSuc)
                    {
                        m_grid.NotifyCurrentCellDirty(true);
                        m_IsPasted = true;
                    }
                    rowIndex++; 
                } 
            }
            else
            { 
                //foreach (KeyValuePair<int, List<ChangeMemberInfo>> chgMember in chgMembers)
                //{
                //    CompanyMember member = DataManager.GetCompanyMember(chgMember.Key);
                //    if (member == null)
                //    { 
                //        if (DataManager.DicSaveRegularMemberIDs != null && DataManager.DicSaveRegularMemberIDs.ContainsKey(chgMember.Key))
                //        {
                //            member = DataManager.GetCompanyMember(DataManager.DicSaveRegularMemberIDs[chgMember.Key]);
                //        }

                //        if (member == null)
                //            continue;
                //    }

                //    foreach (ChangeMemberInfo chgInfo in chgMember.Value)
                //    {
                //        if (chgInfo.Member == null)
                //            continue;

                //        ChgMemberRollback(chgInfo.infoType, member, chgInfo.ChangedData, true);
                //    }
                //}
                 
                if (newTeams.Count > 0)
                {
                    asb();
                }
            } 

            if (newTeams.Count > 0)
            {
                foreach (KeyValuePair<int, TreeNode> node in newTeams)
                {
                    RegularTeam team = node.Value.Tag as RegularTeam;
                    DataManager.SetRegularTeam(team.TeamID, team);
                    DataManager.SetRegularMembers(team);
                }
                 
                FormMain.Instance.SetRegularTeamComboItems();
            }
             
            foreach (CompanyMember member in newMembers)
            {
                if (member.Team == null) 
                    member.Team = (RegularTeam)m_grid.CurrentTeam; 
                
                RegularTeam team = DataManager.GetRegularTeam(member.Team.TeamID);
                
                CompanyMember newMember2 = new CompanyMember();
                newMember2.CopyFrom(member);
                DataManager.SetRegularMember(team, newMember2);

                test[member.ID] = member.Team;
            }             
            foreach (KeyValuePair<int, List<ChangeMemberInfo>> chgMember in chgMembers)
            {
                // 팀 정보 변경됐을 때 팀 정보 Update
                int chgMemberID = chgMember.Key;
                if (chgMemberID < 0 && DataManager.DicSaveRegularMemberIDs != null && DataManager.DicSaveRegularMemberIDs.ContainsKey(chgMember.Key))
                {
                    chgMemberID = DataManager.DicSaveRegularMemberIDs[chgMember.Key];
                }

                CompanyMember orgMember = DataManager.GetCompanyMember(chgMemberID);

                RegularTeam chgTeam = null;
                foreach (ChangeMemberInfo chgInfo in chgMember.Value)
                {
                    //if (chgInfo.infoType == (int)ChangeMemberInfo.InfoType.TeamName)
                    //{
                    //    chgTeam = chgInfo.ChangedData as RegularTeam;

                    //    DataManager.RemoveCompanyMember(chgMemberID);
                    //    DataManager.SetRegularMember(chgTeam, chgInfo.Member);
                    //    break;
                    //}
                    //else
                        ChgMemberRollback(chgInfo.infoType, orgMember, chgInfo.ChangedData, true); 
                }

                if (chgTeam == null)
                    chgTeam = chgMember.Value[chgMember.Value.Count - 1].Member.Team;

                test[chgMember.Key] = chgTeam;                 
            }

            if (!m_IsFirst)
                m_grid.SelectTeam((RegularTeam)m_grid.CurrentTeam, true);

            if (m_IsFirst)
                m_IsFirst = false;
        } 

        private bool DoPasted(int columnIndex, string pastedValue, DataGridViewCell curCell, CompanyMember member, object curOrgTag, ChangeMemberInfo chgMemberInfo)
        {
            chgMemberInfo.infoType = columnIndex;
            if (columnIndex == m_grid.TeamNameIndex2) // 소속팀
            {
                RegularTeam team = MakeRegularTeam(pastedValue);
                RegularTeam orgTeam = curOrgTag as RegularTeam;

                if (team == orgTeam)
                {
                    return false;
                } 

                curCell.Value = team.TeamID;
                curCell.Tag = member.Team = team;
                 
                chgMemberInfo.OriginData = orgTeam;
                chgMemberInfo.ChangedData = curCell.Tag;


            }
            else if (columnIndex == m_grid.NameIndex2) // 이름
            {
                if ((curCell.Value != null && pastedValue == curCell.Value.ToString()) || pastedValue.Length <= 0)
                { 
                    return false;
                }

                curCell.Value = curCell.Tag = member.Name = pastedValue;

                chgMemberInfo.OriginData = curOrgTag;
                chgMemberInfo.ChangedData = curCell.Tag;
            }
            else if (columnIndex == m_grid.MemberIDIndex2) // 사번
            {
                if (curCell.Value != null && pastedValue == curCell.Value.ToString())
                { 
                    return false;
                }

                TeamEditor.TeamGrid.MemberID orgMemberID = curOrgTag as TeamEditor.TeamGrid.MemberID;
                if (orgMemberID == null || orgMemberID.ID.Length <= 0)
                {

                    curCell.Value = new TeamEditor.TeamGrid.MemberID(pastedValue, true);
                    curCell.Tag = curCell.Value;

                    chgMemberInfo.ChangedData = curCell.Tag;
                }
                else
                {
                    TeamEditor.TeamGrid.MemberID id = (TeamEditor.TeamGrid.MemberID)curOrgTag;
                    id.IsChanged = DataManager.GetCompanyMemberMemberIDChanged(member);

                    if (curCell.Value != null && id.ID == pastedValue)
                    {
                        curCell.Value = curCell.Tag;
                         
                        return false;
                    }

                    curCell.Value = new TeamEditor.TeamGrid.MemberID(curCell.Value == null ? "" : pastedValue, true);
                    curCell.Tag = curCell.Value;

                    chgMemberInfo.OriginData = id;
                    chgMemberInfo.ChangedData = curCell.Value;
                }

                DataManager.SetCompanyMemberMemberIDChanged(member, true);

                member.MemberID = pastedValue;

                chgMemberInfo.OriginData = curOrgTag;
                chgMemberInfo.ChangedData = curCell.Tag;
            }
            else if (columnIndex == m_grid.PositionIndex2) // 직위
            {
                if (curCell.Value != null && pastedValue == curCell.Value.ToString())
                { 
                    return false;
                }

                int nJobPositionID = -1;
                if (DataManager.GetJobPositionID(pastedValue, out nJobPositionID))
                {
                    curCell.Value = curCell.Tag = pastedValue;
                    member.PositionID = nJobPositionID;
                }
                else
                {
                    curCell.Value = curCell.Tag = null;
                    member.PositionID = -1;
                }

                chgMemberInfo.OriginData = curOrgTag;
                chgMemberInfo.ChangedData = curCell.Tag;
            }
            else if (columnIndex == m_grid.SubPositionIndex2) // 직위 상세
            {
                DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn)m_grid.Columns[columnIndex];
                CompanyMember.JobPositionSubInfo subPosition = CompanyMember.JobPositionSubInfo.GetSubPosition(pastedValue);

                if (subPosition == null && pastedValue.Length > 0)
                {
                    subPosition = new CompanyMember.JobPositionSubInfo();
                    subPosition.Name = pastedValue;
                    column.Items.Add(subPosition);
                }
                else if (subPosition != null && !column.Items.Contains(subPosition))
                    column.Items.Add(subPosition);

                curCell.Value = curCell.Tag = member.SubJobPosition = subPosition;

                chgMemberInfo.OriginData = curOrgTag;
                chgMemberInfo.ChangedData = curCell.Tag;
            }
            else if (columnIndex == m_grid.ColumnLevelIndex2) // 직급
            { 
                DataGridViewComboBoxColumn columnLevel = (DataGridViewComboBoxColumn)m_grid.Columns[columnIndex];
                if (pastedValue.Length <= 0)
                {
                    //if (columnLevel.Items[0] != null)
                    //{
                    //    curCell.Value = curCell.Tag = columnLevel.Items[0].ToString();
                    //    member.LevelID = m_grid.GetLevelID(columnLevel.Items[0].ToString());
                    //} 
                    curCell.Value = curCell.Tag = null;
                    member.LevelID = -1;
                }
                else
                {
                    for (int i = 0; i < columnLevel.Items.Count; i++)
                    {
                        if (columnLevel.Items[i].ToString() == pastedValue)
                        {
                            curCell.Value = curCell.Tag = pastedValue;
                            member.LevelID = m_grid.GetLevelID(pastedValue);
                            break;
                        }
                    } 
                }

                chgMemberInfo.OriginData = curOrgTag;
                chgMemberInfo.ChangedData = curCell.Tag;
            }
            else if (columnIndex == m_grid.SubLevelIndex2) // 직급 상세
            {
                
                
                //if (pastedValue.Length <= 0)
                //{
                //    if (columnSubLevel.Items[0] != null)
                //    {
                //        curCell.Value = curCell.Tag = columnSubLevel.Items[0].ToString();
                //        member.LevelID = m_grid.GetLevelID(columnSubLevel.Items[0].ToString());
                //    }
                //}
                //else
                {
                    DataGridViewComboBoxColumn columnSubLevel = (DataGridViewComboBoxColumn)m_grid.Columns[columnIndex];
                    CompanyMember.JobLevelSubInfo subLevel = CompanyMember.JobLevelSubInfo.GetJobSubLevel(pastedValue);

                    if (subLevel == null && pastedValue.Length > 0)
                    {
                        subLevel = new CompanyMember.JobLevelSubInfo();
                        subLevel.Name = pastedValue;
                        columnSubLevel.Items.Add(subLevel);
                    }
                    else if (subLevel != null && !columnSubLevel.Items.Contains(subLevel))
                        columnSubLevel.Items.Add(subLevel);

                    curCell.Value = curCell.Tag = member.SubJobLevel = subLevel;
                } 

                chgMemberInfo.OriginData = curOrgTag;
                chgMemberInfo.ChangedData = curCell.Tag;
            }
            else if (columnIndex == m_grid.PhoneNumberIndex2) // 휴대폰
            {
                if (curCell.Value != null && pastedValue == curCell.Value.ToString()) 
                    return false; 

                TeamEditor.TeamGrid.PhoneNumber phoneNumber = new TeamEditor.TeamGrid.PhoneNumber(pastedValue, true);
                if (phoneNumber.IsValid)
                {
                    curCell.Value = curCell.Tag = phoneNumber;
                    member.PhoneNumber = phoneNumber.Number;
                }
                else 
                    curCell.Value = curCell.Tag = member.PhoneNumber = null;  

                chgMemberInfo.OriginData = curOrgTag;
                chgMemberInfo.ChangedData = phoneNumber;
            }
            else if (columnIndex == m_grid.OfficePhoneNumberIndex2) // 근무처 전화번호
            {
                if (curCell.Value != null && pastedValue == curCell.Value.ToString()) 
                    return false; 

                TeamEditor.TeamGrid.OfficePhoneNumber officePhoneNumber = new TeamEditor.TeamGrid.OfficePhoneNumber(pastedValue, true);
                if (officePhoneNumber.IsValid)
                {
                    curCell.Value = curCell.Tag = officePhoneNumber;
                    member.OfficePhoneNumber = officePhoneNumber.Number;
                }
                else 
                    curCell.Value = curCell.Tag = member.OfficePhoneNumber = null; 

                chgMemberInfo.OriginData = curOrgTag;
                chgMemberInfo.ChangedData = officePhoneNumber;
            }
            else if (columnIndex == m_grid.GroupPositionIndex2) // 직군
            {
                DataGridViewComboBoxColumn columnGroupPosition = (DataGridViewComboBoxColumn)m_grid.Columns[columnIndex];
                CompanyMember.JobGroupPosition groupPosition = CompanyMember.JobGroupPosition.GetJobGroupPosition(pastedValue);

                if (groupPosition == null && pastedValue.Length > 0)
                {
                    groupPosition = new CompanyMember.JobGroupPosition();
                    groupPosition.Name = pastedValue;
                    columnGroupPosition.Items.Add(groupPosition);
                }
                else if (groupPosition != null && !columnGroupPosition.Items.Contains(groupPosition))
                    columnGroupPosition.Items.Add(groupPosition);

                curCell.Value = curCell.Tag = member.GroupPosition = groupPosition;

                chgMemberInfo.OriginData = curOrgTag;
                chgMemberInfo.ChangedData = curCell.Tag;
            }

            return true;
        } 

        #region 소속팀 관련
        private RegularTeam MakeRegularTeam(string pastedValue)
        {
            RegularTeam team = null;

            // Path가 있을때
            if (pastedValue.Contains("/") || pastedValue.Contains("\\"))
            {
                string strTeamPath = pastedValue.Replace("\\", "/");
                string strTeamPath2 = "";
                string[] temp2 = strTeamPath.Split('/');
                for (int i = 0; i < temp2.Length; i++)
                {
                    if (temp2[i].ToString().Length > 0)
                    {
                        strTeamPath2 += temp2[i].ToString();
                        if (i + 1 < temp2.Length)
                            strTeamPath2 += "/";
                    }
                }

                string pathTopName = strTeamPath2.Substring(0, strTeamPath2.IndexOf("/"));
                TreeNode node = null;
                if (m_grid.LinkedTree.TopNode.Text == pathTopName)
                    node = FindTeam(m_grid.LinkedTree.TopNode, strTeamPath2);


                if (node == null || node.Tag == null)
                    team = (RegularTeam)m_grid.CurrentTeam;
                else
                    team = node.Tag as RegularTeam;
            }
            else
            {
                if (m_grid.LinkedTree != null && m_grid.LinkedTree.Nodes.Count > 0)
                {
                    if (team == null)
                        team = m_grid.LinkedTree.TopNode.Tag as RegularTeam;

                    if (team.TeamName == pastedValue) // 최상위그룹일때
                        team = this.m_grid.LinkedTree.TopNode.Tag as RegularTeam;
                    else // 최상위그룹 아닐때 현재 Tree 선택된 팀으로 입력                                     
                        team = (RegularTeam)m_grid.CurrentTeam;
                }
            }

            return team;
        }

        private TreeNode FindTeam(TreeNode node, string strTeamPath)
        {
            TreeNode rtnNode = null;
            string strDeptName = strTeamPath.Split('/')[0];

            if (node.Text == strDeptName && node.Parent == null)
            {
                rtnNode = node;
            }
            else
            {
                foreach (TreeNode childNode in node.Nodes)
                {
                    if (childNode.Text == strDeptName)
                    {
                        rtnNode = childNode;
                        break;
                    }
                }
            }

            if (rtnNode == null)
            {  
                int teamID = DataManager.NoSaveTeamID();
                int nIndex = node.Nodes.Count;
                rtnNode = node.Nodes.Insert(nIndex, strDeptName);
                rtnNode.Tag = RegistRegularTeam(strDeptName, (node.Tag as RegularTeam), teamID);                
                ((RegularTeam)rtnNode.Tag).TeamID = teamID;

                newTeams.Add(teamID, rtnNode); 
               
                node.Expand();
            }

            if (strTeamPath.Split('/').Length > 1)
            {
                return FindTeam(rtnNode, strTeamPath.Substring(strDeptName.Length + 1));
            }

            return rtnNode;
        } 

        private void asb()
        {
            foreach (KeyValuePair<int, TreeNode> node in newTeams)
            {
                // 1. node의 tag 구하기 - RegularTeam
                RegularTeam nodeTag = node.Value.Tag as RegularTeam;
                if (nodeTag == null)
                    continue;
                // 2. 구한 tag의 Parent 확인
                RegularTeam parentTeam = nodeTag.ParentTeam;
                // 3. tree에서 Parent와 일치하는 node 찾기
                TreeNode parentNode = FindNode(parentTeam);
                // 4. 찾은 node에 add
                TreeNode rtnNode = null;
                int nIndex = parentNode.Nodes.Count;
                rtnNode = parentNode.Nodes.Insert(nIndex, node.Value.Text);
                rtnNode.Tag = nodeTag;

                parentNode.Expand();
            }
        }
        private TreeNode FindNode(RegularTeam team, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
            {
                if (m_tree == null)
                    return null;

                nodes = m_tree.Nodes;
            } 

            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null)
                {
                    if (team == node.Tag)
                        return node;

                    TreeNode findNode = FindNode(team, node.Nodes);
                    if (findNode != null)
                        return findNode;
                }
            }

            return null;
        }

        private RegularTeam RegistRegularTeam(string strDeptName, RegularTeam parentTeam, int nTeamID)
        {
            RegularTeam team = new RegularTeam();
            team.TeamName = strDeptName;
            team.TeamID = nTeamID; 
            team.ParentTeam = parentTeam;

            return team;
        }  
        #endregion

        public override void RollBack()
        { 
            foreach (KeyValuePair<int, List<ChangeMemberInfo>> chgMember in chgMembers)
            {
                CompanyMember member = DataManager.GetCompanyMember(chgMember.Key);
                if (member == null)
                {
                    if (DataManager.DicSaveRegularMemberIDs != null && DataManager.DicSaveRegularMemberIDs.ContainsKey(chgMember.Key))
                    {
                        member = DataManager.GetCompanyMember(DataManager.DicSaveRegularMemberIDs[chgMember.Key]); 
                    }
                    
                    if (member == null)
                        continue;
                }

                foreach (ChangeMemberInfo chgInfo in chgMember.Value)
                {
                    if (chgInfo.Member == null)
                        continue;

                    //CompanyMember member = DataManager.GetCompanyMember(chgInfo.OrgMember.ID);
                    //if (member == null)
                    //    continue;

                    ChgMemberRollback(chgInfo.infoType, member, chgInfo.OriginData);
                }
            }

            foreach (CompanyMember newMember in newMembers)
            {
                DataManager.RemoveCompanyMember(newMember);
            }

            foreach (KeyValuePair<int, TreeNode> item in newTeams)
            {
                DataManager.RemoveRegularTeam((RegularTeam)item.Value.Tag);

                // 1. TreeNode의 Tag 구하기 - RegularTeam
                RegularTeam team = item.Value.Tag as RegularTeam;
                if (team == null)
                    continue;
                // 2. 구한 Tag의 ParentTeam 구하기
                RegularTeam parentTeam = team.ParentTeam;
                if (parentTeam == null)
                    continue;
                // 3. Tree의 Node에서 ParentTeam 찾기
                TreeNode node = FindNode(parentTeam);
                if (node == null)
                    continue;
                // 4. 찾은 ParentTeam Node에서 Remove
                node.Nodes.Remove(node.Nodes[item.Value.Index]); 
            }

            m_grid.SelectTeam(m_grid.CurrentTeam, true);
        } 

        private void ChgMemberRollback(int infoType, CompanyMember member, object originData, bool isDo = false)
        {
            if (infoType == (int)TeamEditor.Command.ChangeMemberInfo.InfoType.TeamName)
            { 
                DataManager.RemoveCompanyMember(member); // 기존 Member정보 삭제
                member.Team = (RegularTeam)originData;

                //if (!isDo)
                    DataManager.SetRegularMember(member.Team, member);
            }
            else if (infoType == (int)TeamEditor.Command.ChangeMemberInfo.InfoType.Name)
            {
                if (originData == null)
                    member.Name = "";
                else
                    member.Name = originData.ToString();
            }
            else if (infoType == (int)TeamEditor.Command.ChangeMemberInfo.InfoType.MemberID)
            {
                if (originData == null)
                    member.MemberID = "";
                else if (originData is TeamGrid.MemberID)
                {
                    TeamGrid.MemberID id = (TeamGrid.MemberID)originData;
                    member.MemberID = id.ID;

                    DataManager.SetCompanyMemberMemberIDChanged(member, id.IsChanged);
                }
            }
            else if (infoType == (int)TeamEditor.Command.ChangeMemberInfo.InfoType.Position)
            {
                int nPositionID = -100;
                DataManager.GetJobPositionID((originData == null) ? "" : originData.ToString(), out nPositionID);
                member.PositionID = nPositionID;
            }
            else if (infoType == (int)TeamEditor.Command.ChangeMemberInfo.InfoType.SubPosition)
            {
                member.SubJobPosition = (CompanyMember.JobPositionSubInfo)originData;
            }
            else if (infoType == (int)TeamEditor.Command.ChangeMemberInfo.InfoType.Level)
            {
                if (originData == null)
                    member.LevelID = -1;
                else
                    member.LevelID = m_grid.GetLevelID(originData.ToString());
            }
            else if (infoType == (int)TeamEditor.Command.ChangeMemberInfo.InfoType.SubLevel)
            {
                member.SubJobLevel = (CompanyMember.JobLevelSubInfo)originData;
            }
            else if (infoType == (int)TeamEditor.Command.ChangeMemberInfo.InfoType.PhoneNumber)
            {
                if (originData is TeamGrid.PhoneNumber)
                {
                    TeamGrid.PhoneNumber phone = (TeamGrid.PhoneNumber)originData;
                    member.PhoneNumber = phone.Number;

                    DataManager.SetCompanyMemberPhoneNumberChanged(member, phone.IsChanged);
                }
                else if(originData != null)
                {
                    TeamEditor.TeamGrid.PhoneNumber phoneNumber = new TeamEditor.TeamGrid.PhoneNumber(originData.ToString(), true);
                    if (phoneNumber.IsValid)
                    {
                        member.PhoneNumber = phoneNumber.Number;
                    }                    
                    else
                    {
                        member.PhoneNumber = null;
                        DataManager.SetCompanyMemberPhoneNumberChanged(member, !DataManager.GetCompanyMemberPhoneNumberChanged(member));
                    }
                }
                else
                {
                    member.PhoneNumber = null;
                    DataManager.SetCompanyMemberPhoneNumberChanged(member, !DataManager.GetCompanyMemberPhoneNumberChanged(member));
                }
            }
            else if (infoType == (int)TeamEditor.Command.ChangeMemberInfo.InfoType.OfficePhoneNumber)
            {
                if (originData is TeamGrid.OfficePhoneNumber)
                {
                    TeamGrid.OfficePhoneNumber phone = (TeamGrid.OfficePhoneNumber)originData;
                    member.OfficePhoneNumber = phone.Number;

                    DataManager.SetCompanyMemberOfficePhoneNumberChanged(member, phone.IsChanged);
                }
                else if (originData != null)
                {
                    TeamEditor.TeamGrid.OfficePhoneNumber officePhoneNumber = new TeamEditor.TeamGrid.OfficePhoneNumber(originData.ToString(), true);
                    if (officePhoneNumber.IsValid)
                    {
                        member.OfficePhoneNumber = officePhoneNumber.Number;
                    }
                    else
                    {
                        member.OfficePhoneNumber = null;
                        DataManager.SetCompanyMemberOfficePhoneNumberChanged(member, !DataManager.GetCompanyMemberOfficePhoneNumberChanged(member));
                    }
                }
                else
                {
                    member.OfficePhoneNumber = null;
                    DataManager.SetCompanyMemberOfficePhoneNumberChanged(member, !DataManager.GetCompanyMemberOfficePhoneNumberChanged(member));
                } 
            }
            else if (infoType == (int)TeamEditor.Command.ChangeMemberInfo.InfoType.GroupPosition)
            {
                member.GroupPosition = (CompanyMember.JobGroupPosition)originData;
            }
        }
        
        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            if (dir)
            {
                RedoDB(dbMgr);
            }
            else
            {
                UndoDB(dbMgr);
            }
        }

        private bool CheckTeamNAdd(WebDBManager dbMgr, int teamID)
        {
            string strSQL = "Select id from RegularTeam where ID = " + teamID;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;
             
            return true;
        }

        private bool CheckMemberNAdd(WebDBManager dbMgr, int memberID)
        {
            string strSQL = "Select id from CompanyMember where ID = " + memberID;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;
             
            return true;
        }

        private bool RedoDB(WebDBManager dbMgr)
        {
            dbMgr.BeginBatch();

            // 팀 추가
            string strSQL = "Select max(ID) from RegularTeam";
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            int nTeamID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
            
            foreach (KeyValuePair<int, TreeNode> node in newTeams)
            {
                RegularTeam team = node.Value.Tag as RegularTeam;                
                if (team == null)
                    continue;

                if (!CheckTeamNAdd(dbMgr, team.TeamID))
                { 
                    int nParentTeamID = team.ParentTeam.TeamID;
                    if (saveTeamID.ContainsKey(team.ParentTeam.TeamID))
                        nParentTeamID = saveTeamID[team.ParentTeam.TeamID];

                    // 이미 한번 저장이 됐던 Team은 TeamID를 가지고 있으므로 가지고있는 TeamID를 쓴다
                    bool bTeamIDUse = false;
                    int nSaveTeamID = 0;
                    if (team.TeamID > 0)
                        nSaveTeamID = team.TeamID;
                    else
                    {
                        bTeamIDUse = true;
                        nSaveTeamID = nTeamID;
                    }
                    strSQL = string.Format("Insert into RegularTeam (ID, TeamName, ParentTeamID) values ({0}, '{1}', {2})",
                    nSaveTeamID, node.Value.Text, nParentTeamID);

                    if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        dbMgr.BatchRollback();
                        return false;
                    }

                    DataManager.SetRegularTeamMemberInfo(team.TeamID, nSaveTeamID, team);

                    saveTeamID[team.TeamID] = nSaveTeamID;
                    if (DataManager.DicSaveRegularTeamIDs == null)
                        DataManager.DicSaveRegularTeamIDs = new Dictionary<int, int>();
                    DataManager.DicSaveRegularTeamIDs[team.TeamID] = nSaveTeamID;

                    TreeNode targetNode = FindNode(team);
                    if (targetNode != null)
                    {
                        RegularTeam nodeTeam = targetNode.Tag as RegularTeam;
                        nodeTeam.TeamID = nSaveTeamID;
                        nodeTeam.ParentTeam.TeamID = nParentTeamID; 
                    }

                    // newTeam Node Tag정보 업데이트
                    team.TeamID = nSaveTeamID;                    

                    if (bTeamIDUse)
                        nTeamID++;
                }
            }

            // 멤버 추가
            strSQL = "Select max(ID) from CompanyMember";
            arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            int nMemberID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            foreach (CompanyMember member in newMembers)
            {
                if (CheckMemberNAdd(dbMgr, member.ID)) continue;

                int nSubLevelID = DataManager.GetJobSubLevel(dbMgr, 1, member.SubJobLevel);
                int nSubPositionID = DataManager.GetJobSubPosition(dbMgr, 1, member.SubJobPosition);
                int nGroupPositionID = DataManager.GetGroupPosition(dbMgr, 1, member.GroupPosition);

                if (DataManager.GetJobPositionName(member.PositionID) == null)
                    member.PositionID = 0;
                if (member.LevelID < 0)
                    member.LevelID = 0;  

                strSQL = "Insert into CompanyMember (ID, MemberName, LevelID, SubLevelID, MemberID, OfficePhoneNumber, PhoneNumber) values ";
                strSQL += string.Format("({0}, '{1}', {2}, {3}, {4}, {5}, {6})",
                    nMemberID, member.Name, member.LevelID,
                    nSubLevelID < 0 ? "NULL" : nSubLevelID.ToString(),
                    member.MemberID == null || member.MemberID.Length == 0 ? "NULL" : "'" + member.MemberID + "'",
                    member.OfficePhoneNumber == null || member.OfficePhoneNumber.Length == 0 ? "NULL" : "'" + member.OfficePhoneNumber + "'",
                    member.PhoneNumber == null || member.PhoneNumber.Length == 0 ? "NULL" : "'" + DataManager.EncryptString(member.PhoneNumber) + "'");

                if (dbMgr.GetBatchData(strSQL) == null)
                {
                    dbMgr.BatchRollback();
                    return false;
                }

                strSQL = string.Format("Select PositionID from RegularMemberList where RegularTeamID = {0} and CompanyMemberID = {1}",
                    member.Team.TeamID, nMemberID);

                arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.BatchRollback();
                    return false;
                } 

                if (arrResult.Count == 0)
                {
                    // 팀이 파트 팀인 경우에는 파트장이 실제DB에는 팀장으로 데이터가 입력되어 있으므로
                    // 파트장으로 되어있는 데이터를 다시 팀장으로 변환
                    int nSavePositionID = member.PositionID;

                    if (member.Team.IsPartTeam)
                    {
                        if (String.Equals(DataManager.GetJobPositionName(member.PositionID), "파트장"))
                        {
                            if (DataManager.GetJobPositionID("팀장", out nSavePositionID) == false)
                                nSavePositionID = member.PositionID;
                        }
                    }

                    int regularTeamID = member.Team.TeamID;
                    if (saveTeamID.ContainsKey(member.Team.TeamID))
                        regularTeamID = saveTeamID[member.Team.TeamID];
                    else if (test.ContainsKey(member.ID))
                    {
                        regularTeamID = test[member.ID].TeamID;
                    } 

                    strSQL = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values ";
                    strSQL += string.Format("({0}, {1}, {2}, {3}, {4})",
                        regularTeamID, nMemberID,
                        nSavePositionID,
                        nSubPositionID < 0 ? "NULL" : nSubPositionID.ToString(),
                        nGroupPositionID < 0 ? "NULL" : nGroupPositionID.ToString());

                    if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        dbMgr.BatchRollback();
                        return false;
                    }
                }

                saveMemberID[member.ID] = nMemberID;
                if (DataManager.DicSaveRegularMemberIDs == null)
                    DataManager.DicSaveRegularMemberIDs = new Dictionary<int, int>();
                DataManager.DicSaveRegularMemberIDs[member.ID] = nMemberID;

                DataManager.SetRegularMemberID(member, nMemberID);
                nMemberID++;
            }

            // 멤버 수정
            foreach (KeyValuePair<int, List<ChangeMemberInfo>> chgMember in chgMembers)
            {
                nMemberID = chgMember.Key;
                if (nMemberID < 0)
                {
                    if (DataManager.DicSaveRegularMemberIDs != null && DataManager.DicSaveRegularMemberIDs.ContainsKey(nMemberID))
                        nMemberID = DataManager.DicSaveRegularMemberIDs[nMemberID];

                    if (nMemberID < 0)
                        continue;
                }

                string strSet = "";

                for (int i = 0; i < chgMember.Value.Count; i++)
                {
                    ChangeMemberInfo chgInfo = chgMember.Value[i];
                    if (chgInfo.OriginData == chgInfo.ChangedData) 
                        continue; 

                    bool isSuc = true;
                    string strSet2 = UpdateDB(dbMgr, nMemberID, (ChangeMemberInfo.InfoType)chgInfo.infoType, chgInfo.ChangedData, ref isSuc);
                    if (!isSuc)
                    {
                        dbMgr.BatchRollback();
                        return false;
                    }

                    if (strSet2.Length > 0)
                        strSet += strSet2;
                    if (strSet2.Length > 0 && i + 1 < chgMember.Value.Count)
                        strSet += ",";
                }

                // 마지막에 ,로 끝났으면 지우기
                int lastIndex = strSet.LastIndexOf(",");
                if (lastIndex == strSet.Length - 1 && lastIndex > 0) 
                    strSet = strSet.Remove(lastIndex, 1); 

                if (strSet.Length > 0)
                {
                    strSQL = "Update CompanyMember set " + strSet + " where ID = " + nMemberID;
                    if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        dbMgr.BatchRollback();
                        return false;
                    }
                }
            }

            dbMgr.BatchCommit();

            // 저장된 Member ID 부여
            foreach (KeyValuePair<int, int> item in saveMemberID)
            {
                foreach (CompanyMember item2 in newMembers)
	            {
		            if (item.Key == item2.ID)
                    {
                        item2.ID = item.Value;
                        break;
                    }
	            }
            }

            return true;
        }

        private string UpdateDB(WebDBManager dbMgr, int memberID, ChangeMemberInfo.InfoType infoType, object changedData, ref bool suc)
        {
            string strSet = "";

            switch (infoType)
            {
                case ChangeMemberInfo.InfoType.TeamName:
                    string strUpdateTeam = "";
                    if (changedData == null)
                        return strSet;

                    RegularTeam chgTeam = changedData as RegularTeam;
                    if (chgTeam == null)
                        return strSet;

                    if (chgTeam.TeamID < 0)
                        break;

                    strUpdateTeam = string.Format("Update RegularMemberList SET RegularTeamID = {0} WHERE CompanyMemberID = {1} ", chgTeam.TeamID, memberID);                    
                    if (dbMgr.GetBatchData(strUpdateTeam) == null)
                    {
                        suc = false;
                        break;
                    }
                    break;
                case ChangeMemberInfo.InfoType.Name:
                    if (changedData == null || changedData.ToString().Length <= 0)
                        return strSet;

                    strSet = "MemberName = '" + changedData.ToString() + "'";
                    break;

                case ChangeMemberInfo.InfoType.Level:
                    if (changedData == null)
                        strSet = "LevelID = -1";
                    else
                    { 
                        int nLevelID = m_grid.GetLevelID(changedData.ToString());
                        strSet = "LevelID = " + nLevelID;
                    }
                    break;

                case ChangeMemberInfo.InfoType.SubLevel:
                    if (changedData == null)
                        strSet = "SubLevelID = NULL";
                    else
                    {
                        CompanyMember.JobLevelSubInfo subLevel = changedData as CompanyMember.JobLevelSubInfo;

                        if (subLevel.ID < 0)
                            subLevel.ID = DataManager.GetJobSubLevel(dbMgr, 0, subLevel);

                        strSet = "SubLevelID = " + subLevel.ID.ToString();
                    }
                    break;

                case ChangeMemberInfo.InfoType.Position:
                    {
                        int nJobPositionID = -1;
                        if (changedData != null)
                            DataManager.GetJobPositionID(changedData.ToString(), out nJobPositionID);

                        if (nJobPositionID < 0)
                            nJobPositionID = 0;

                        int nNewPositionID = nJobPositionID;
                        if (nJobPositionID < 0 && nJobPositionID > -100)
                        {
                            if (DataManager.GetJobPositionID("팀장", out nNewPositionID) == false)
                            {
                                nNewPositionID = nJobPositionID;
                            }
                        }

                        // 팀이 파트 팀인 경우에는 파트장이 실제DB에는 팀장으로 데이터가 입력되어 있으므로
                        // 파트장으로 되어있는 데이터를 다시 팀장으로 변환
                        int nSavePositionID = nJobPositionID;
                        RegularTeam team = DataManager.GetRegularTeamByCompanyMember(DataManager.GetCompanyMember(memberID));
                        if (team == null)
                            return strSet;

                        if (team.IsPartTeam)
                        {
                            if (String.Equals(DataManager.GetJobPositionName(nJobPositionID), "파트장"))
                            {
                                if (DataManager.GetJobPositionID("팀장", out nSavePositionID))
                                    nNewPositionID = nSavePositionID;
                            }
                        }

                        string _strSQL = string.Format("Update RegularMemberList set PositionID = {0} where RegularTeamID = {1} and CompanyMemberID = {2}",
                            nNewPositionID,
                            team.TeamID,
                            memberID);
                         
                        if (dbMgr.GetBatchData(_strSQL) == null)
                        {
                            suc = false;
                            break;
                        }
                    }
                    return strSet;

                case ChangeMemberInfo.InfoType.SubPosition:
                    {
                        string _strSQL = "";

                        RegularTeam team = DataManager.GetRegularTeamByCompanyMember(DataManager.GetCompanyMember(memberID));
                        if (team == null)
                            return strSet;

                        if (changedData == null)
                        {
                            _strSQL = string.Format("Update RegularMemberList set SubPositionID = NULL where RegularTeamID = {0} and CompanyMemberID = {1}",
                                team.TeamID, memberID);
                        }
                        else
                        {
                            CompanyMember.JobPositionSubInfo subPosition = changedData as CompanyMember.JobPositionSubInfo;

                            if (subPosition.ID < 0)
                                subPosition.ID = DataManager.GetJobSubPosition(dbMgr, 0, subPosition);

                            _strSQL = string.Format("Update RegularMemberList set SubPositionID = {0} where RegularTeamID = {1} and CompanyMemberID = {2}",
                                subPosition.ID, team.TeamID, memberID); 
                        }
                         
                        if (dbMgr.GetBatchData(_strSQL) == null)
                        {
                            suc = false;
                            break;
                        }
                    }
                    return strSet;

                case ChangeMemberInfo.InfoType.GroupPosition:
                    {
                        string _strSQL = "";
                        RegularTeam team = DataManager.GetRegularTeamByCompanyMember(DataManager.GetCompanyMember(memberID));
                        if (team == null)
                            return strSet;

                        if (changedData == null)
                        {

                            _strSQL = string.Format("Update RegularMemberList set GroupPositionID = NULL where RegularTeamID = {0} and CompanyMemberID = {1}",
                                team.TeamID, memberID); 
                        }
                        else
                        {
                            CompanyMember.JobGroupPosition groupPosition = changedData as CompanyMember.JobGroupPosition;

                            if (groupPosition.ID < 0)
                                groupPosition.ID = DataManager.GetGroupPosition(dbMgr, 0, groupPosition);

                            _strSQL = string.Format("Update RegularMemberList set GroupPositionID = {0} where RegularTeamID = {1} and CompanyMemberID = {2}",
                                groupPosition.ID, team.TeamID, memberID); 
                        }
                         
                        if (dbMgr.GetBatchData(_strSQL) == null)
                        {
                            suc = false;
                            break;
                        }
                    }
                    return strSet;

                case ChangeMemberInfo.InfoType.MemberID:
                    TeamEditor.TeamGrid.MemberID id = changedData as TeamEditor.TeamGrid.MemberID;

                    if (changedData == null || id == null || id.ID.Length <= 0)
                        strSet = "MemberID = NULL";
                    else
                        strSet = "MemberID = '" + changedData.ToString() + "'";
                    break;

                case ChangeMemberInfo.InfoType.OfficePhoneNumber:
                    TeamGrid.OfficePhoneNumber officePhoneNumber = changedData as TeamGrid.OfficePhoneNumber;
                    if (changedData == null || officePhoneNumber == null || officePhoneNumber.Number.Length <= 0)
                        strSet = "OfficePhoneNumber = NULL";
                    else
                        strSet = "OfficePhoneNumber = '" + changedData.ToString() + "'";
                    break;

                case ChangeMemberInfo.InfoType.PhoneNumber:
                    TeamGrid.PhoneNumber phoneNumber = changedData as TeamGrid.PhoneNumber;
                    if (changedData == null || phoneNumber == null || phoneNumber.Number.Length <= 0)
                        strSet = "PhoneNumber = ''";
                    else
                    {    
                        strSet = "PhoneNumber = '" + DataManager.EncryptString(phoneNumber.Number) + "'";
                    }
                    break; 
            }

            return strSet;
        }

        private List<string> UndoDB(WebDBManager dbMgr)
        {
            m_rollbackSQLs.Clear();
             
            dbMgr.BeginBatch();

            foreach (KeyValuePair<int, List<ChangeMemberInfo>> chgMember in chgMembers)
            {
                int nMemberID = chgMember.Key;
                if (nMemberID < 0)
                {
                    if (DataManager.DicSaveRegularMemberIDs != null && DataManager.DicSaveRegularMemberIDs.ContainsKey(nMemberID))
                    {
                        nMemberID = DataManager.DicSaveRegularMemberIDs[nMemberID];
                    }

                    if (nMemberID < 0)
                        continue;
                }

                string strSet = ""; 
                for (int i = 0; i < chgMember.Value.Count; i++)
                {
                    ChangeMemberInfo chgInfo = chgMember.Value[i];
                    if (chgInfo.ChangedData == chgInfo.OriginData)
                        continue;

                    bool isSuc = true;
                    string strSet2 = UpdateDB(dbMgr, nMemberID, (ChangeMemberInfo.InfoType)chgInfo.infoType, chgInfo.OriginData, ref isSuc);
                    if (!isSuc)
                    {
                        dbMgr.BatchRollback();
                        return null;
                    }

                    if (strSet2.Length > 0)
                        strSet += strSet2;
                    if (strSet2.Length > 0 && i + 1 < chgMember.Value.Count)
                        strSet += ",";
                }

                // 마지막에 ,로 끝났으면 지우기
                int lastIndex = strSet.LastIndexOf(",");
                if (lastIndex == strSet.Length - 1 && lastIndex > 0)
                    strSet = strSet.Remove(lastIndex, 1);

                if (strSet.Length > 0)
                {
                    string strSQL = "Update CompanyMember set " + strSet + " where ID = " + nMemberID; 
                    if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        dbMgr.BatchRollback();
                        return null;
                    }
                }
            }
             
            #region Member 삭제
            List<int> deleteMemberIDs = new List<int>();
            foreach (CompanyMember item in newMembers)
            {
                deleteMemberIDs.Add(item.ID);
            }

            List<string> insertRegularMemberList = null;
            List<string> insertFacilityManagerList = null;
            List<string> insertTemporaryMemberList = null;
            List<string> updateSOPGenUserList = null;
            List<string> insertCompanyMemberList = null;

            string strCompanyMemberIDs = string.Join(", ", deleteMemberIDs);
            if (strCompanyMemberIDs.Length > 0)
            {
                strCompanyMemberIDs = "(" + strCompanyMemberIDs + ")";
                insertRegularMemberList = RemoveRegularMemberList(dbMgr, strCompanyMemberIDs);
                if (insertRegularMemberList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return null;
                }

                insertFacilityManagerList = RemoveFacilityManagers(dbMgr, strCompanyMemberIDs);

                if (insertFacilityManagerList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return null;
                }

                insertTemporaryMemberList = RemoveTemporaryMembers(dbMgr, strCompanyMemberIDs);

                if (insertTemporaryMemberList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return null;
                }

                updateSOPGenUserList = CommandRemoveRegularTeam.UpdateSOPGenUsers(dbMgr, strCompanyMemberIDs);

                if (updateSOPGenUserList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return null;
                }

                insertCompanyMemberList = CommandRemoveRegularTeam.RemoveCompanyMembers(dbMgr, strCompanyMemberIDs);

                if (insertCompanyMemberList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return null;
                }

                m_rollbackSQLs.AddRange(insertCompanyMemberList);
                m_rollbackSQLs.AddRange(updateSOPGenUserList);
                //m_rollbackSQLs.AddRange(insertDutyList);
                m_rollbackSQLs.AddRange(insertTemporaryMemberList);
                m_rollbackSQLs.AddRange(insertFacilityManagerList);
                m_rollbackSQLs.AddRange(insertRegularMemberList);
            }  
            #endregion

            List<string> deleteTeamIDs = new List<string>();
            foreach (KeyValuePair<int, TreeNode> node in newTeams)
            {
                RegularTeam team = node.Value.Tag as RegularTeam;
                if (team == null)
                    continue;

                deleteTeamIDs.Add(team.TeamID.ToString()); 
            }

            if (deleteTeamIDs.Count > 0)
            {
                string strSQLDeleteRegularTeam = "DELETE FROM RegularTeam WHERE ID IN (" + string.Join(", ", deleteTeamIDs) + ")";

                if (dbMgr.GetBatchData(strSQLDeleteRegularTeam) == null)
                {
                    dbMgr.BatchRollback();
                    return null;
                }
            }

            dbMgr.BatchCommit();
            return m_rollbackSQLs;
        }

        #region Member 삭제
        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveRegularMemberList(WebDBManager dbMgr, string strCompanyMemberIDs)
        {
            string strSQL = "Select RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID from RegularMemberList where CompanyMemberID in " + strCompanyMemberIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            List<string> insertList = new List<string>();

            string strInsertFormat = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values ({0}, {1}, {2}, {3}, {4})";
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nRegularTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nCompanyMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nPositionID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nSubPositionID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nGroupPositionID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                if (nRegularTeamID < 0 || nCompanyMemberID < 0 || nPositionID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nRegularTeamID, nCompanyMemberID, nPositionID,
                    nSubPositionID < 0 ? "NULL" : nSubPositionID.ToString(),
                    nGroupPositionID < 0 ? "NULL" : nGroupPositionID.ToString());
                insertList.Add(strInsert);
            }

            strSQL = "Delete from RegularMemberList where CompanyMemberID in " + strCompanyMemberIDs;

            if (dbMgr.GetBatchData(strSQL) == null)
                return null;

            return insertList;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveFacilityManagers(WebDBManager dbMgr, string strCompanyMemberIDs)
        {
            List<string> insertList = new List<string>();

            if (!RemoveFacilityManagers(dbMgr, strCompanyMemberIDs, insertList))
                return null;

            if (!RemoveBuildingFacilityManagers(dbMgr, strCompanyMemberIDs, insertList))
                return null;

            if (!RemoveEquipZoneFacilityManagers(dbMgr, strCompanyMemberIDs, insertList))
                return null;

            return insertList;
        }

        private bool RemoveFacilityManagers(WebDBManager dbMgr, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit, SiteID ";
            strSQL += "from FacilityManager where ";

            if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 0 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from FacilityManager where MemberType = 0 and MemberID in " + strCompanyMemberIDs;
            }
            else
                return true;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into FacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit, SiteID) ";
            strInsertFormat += "values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})";

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 5], null);
                int nUpperLimit = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nSiteID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);

                if (nID < 0 || nMemberID < 0 || nMemberType < 0 || nFacilityType < 0 || nSiteID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, nMemberID, nMemberType, nFacilityType,
                    nLevelLimit < 0 ? "NULL" : nLevelLimit.ToString(),
                    strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                    nUpperLimit, nSiteID);

                insertList.Add(strInsert);
            }

            return dbMgr.GetBatchData(strDeleteSQL) != null;
        }

        private bool RemoveEquipZoneFacilityManagers(WebDBManager dbMgr, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberID, MemberType, SiteID, FacilityType, LevelLimit, EquipZoneID, Description, UpperLimit ";
            strSQL += "from EquipZoneFacilityManager where ";

            if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 0 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from EquipZoneFacilityManager where MemberType = 0 and MemberID in " + strCompanyMemberIDs;
            }
            else
                return true;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into EquipZoneFacilityManager (ID, MemberID, MemberType, SiteID, FacilityType, LevelLimit, EquipZoneID, Description, UpperLimit) ";
            strInsertFormat += "values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nSiteID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 7], null);
                int nUpperLimit = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                if (nID < 0 || nMemberID < 0 || nMemberType < 0 || nFacilityType < 0 || nEquipZoneID < 0 || nSiteID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, nMemberID, nMemberType, nSiteID, nFacilityType,
                    nLevelLimit < 0 ? "NULL" : nLevelLimit.ToString(),
                    nEquipZoneID,
                    strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                    nUpperLimit);

                insertList.Add(strInsert);
            }

            return dbMgr.GetBatchData(strDeleteSQL) != null;
        }

        private bool RemoveBuildingFacilityManagers(WebDBManager dbMgr, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit, SiteID ";
            strSQL += "from BuildingFacilityManager where ";

            if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 0 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from BuildingFacilityManager where MemberType = 0 and MemberID in " + strCompanyMemberIDs;
            }
            else
                return true;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into BuildingFacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit, SiteID) ";
            strInsertFormat += "values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 6], null);
                int nUpperLimit = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                int nSiteID = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                if (nID < 0 || nMemberID < 0 || nMemberType < 0 || nFacilityType < 0 || nBuildingID < 0 || nSiteID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, nMemberID, nMemberType, nFacilityType,
                    nLevelLimit < 0 ? "NULL" : nLevelLimit.ToString(),
                    nBuildingID,
                    strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                    nUpperLimit, nSiteID);

                insertList.Add(strInsert);
            }

            return dbMgr.GetBatchData(strDeleteSQL) != null;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveTemporaryMembers(WebDBManager dbMgr, string strCompanyMemberIDs)
        {
            List<string> insertList = new List<string>();

            if (!RemoveTemporaryMemberList(dbMgr, strCompanyMemberIDs, insertList))
                return null;

            return insertList;
        }

        private bool RemoveTemporaryMemberList(WebDBManager dbMgr, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role ";
            strSQL += "from TemporaryMemberList where ";

            if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 1 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from TemporaryMemberList where MemberType = 1 and MemberID in " + strCompanyMemberIDs;
            }
            else
                return true;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) ";
            strInsertFormat += "values ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nTemporaryTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                bool isNormal = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nTeamLeader = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nMemberCount = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                int nRole = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                if (nID < 0 || nTemporaryTeamID < 0 || nMemberType < 0 || nRole < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, strMemberName, nTemporaryTeamID,
                    isNormal ? 1 : 0,
                    nMemberID < 0 ? "NULL" : nMemberID.ToString(),
                    nTeamLeader < 0 ? "NULL" : nTeamLeader.ToString(),
                    nMemberType,
                    nMemberCount < 0 ? "NULL" : nMemberCount.ToString(),
                    nRole);

                insertList.Add(strInsert);
            }

            return dbMgr.GetBatchData(strDeleteSQL) != null;
        } 
        #endregion
    }

    public class ChangeMemberInfo
    {
        public enum InfoType { TeamName = 1, Name, Position, SubPosition, Level, SubLevel, PhoneNumber, GroupPosition, MemberID, OfficePhoneNumber, Unknown };

        private InfoType m_infoType = InfoType.Unknown;
        public int infoType
        {
            get { return (int)m_infoType; }
            set { m_infoType = (InfoType)value; }
        }
        private object m_originData = null;
        public object OriginData
        {
            get { return m_originData; }
            set { m_originData = value; }
        }
        private object m_changedData = null;
        public object ChangedData
        {
            get { return m_changedData; }
            set { m_changedData = value; }
        }
        private CompanyMember m_member = null;
        public CompanyMember Member
        {
            get { return m_member; }
            set { m_member = value; }
        }
        //private CompanyMember m_chgMember = null;
        //public CompanyMember ChgMember
        //{
        //    get { return m_chgMember; }
        //    set { m_chgMember = value; }
        //}
    }
}
