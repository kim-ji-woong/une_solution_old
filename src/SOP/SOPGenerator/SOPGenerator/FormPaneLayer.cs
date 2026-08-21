using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;

namespace SOPGen
{
    public partial class FormPaneLayer : Form
    {
        private FormMain m_Main = null;
        //private string m_strValue = "";
        private string m_strNodePath = "";
        private TeamData m_selectedTeamData = null;
        private bool m_isDragDrop = false;
        //private bool m_isAdd = false;

        // 상시 조직의 팀과 팀원들 리스트
        // int : 팀 또는 팀원의 ID, 0보다 큰 값이면 팀, 음수이면 팀원의 ID를 의미한다.
        Dictionary<int, TeamData> m_dicRegularMember = new Dictionary<int,TeamData>();
        // 비상 조직의 팀과 팀원들 리스트
        // int : 팀 또는 팀원의 ID, 0보다 큰 값이면 팀, 음수이면 팀원의 ID를 의미한다.
        Dictionary<int, TeamData> m_dicTemporaryMember = new Dictionary<int, TeamData>();

        public bool IsDragDrop
        {
            get { return m_isDragDrop; }
            set { m_isDragDrop = value; }
        }

        public string NodePath
        {
            get { return m_strNodePath; }
            set { m_strNodePath = value; }
        }

        public FormPaneLayer(FormMain main)
        {
            InitializeComponent();

            m_Main = main;
            GetDisasterCategoryInfo();
        }

        //////////////////////////////////////////////////////////////////////////
        //TreeView        
        public void GetDisasterCategoryInfo()
        {
            ArrayList arrList = new ArrayList();
            arrList = m_Main.m_dbMgr.GetDisasterCategoryName();
            if (arrList.Count == 0) return;

            for (int i = 0; i < arrList.Count; i++)
            {
                Data_DispasterCategory data = (Data_DispasterCategory)arrList[i];
                treeViewSOP.Nodes.Add(data.CategoryName.TrimEnd());
            }
        }

        private void btnSOPAdd_Click(object sender, EventArgs e)
        {
            if (treeViewSOP.SelectedNode == null) return;

//             int nParent = 0;
//             if (treeViewSOP.SelectedNode.Level != 0)
//                 nParent = treeViewSOP.SelectedNode.Parent.Index;
//             else
//                 nParent = treeViewSOP.SelectedNode.Index;
// 
//             TreeNode node = treeViewSOP.Nodes[nParent].Nodes.Add(" ");
//             int nIndex = node.Index;
// 
//             treeViewSOP.Nodes[nParent].Nodes[nIndex].Nodes.Add("예방");
//             treeViewSOP.Nodes[nParent].Nodes[nIndex].Nodes.Add("대비");
//             treeViewSOP.Nodes[nParent].Nodes[nIndex].Nodes.Add("대응");
//             treeViewSOP.Nodes[nParent].Nodes[nIndex].Nodes.Add("복구");
//             treeViewSOP.ExpandAll();
// 
//             treeViewSOP.SelectedNode = treeViewSOP.Nodes[nParent];
//             treeViewSOP.Select();
// 
//             treeViewSOP.Nodes[nParent].Nodes[nIndex].BeginEdit();
            //m_isAdd = true;
            if (treeViewSOP.SelectedNode.Level == 0)
            {
                treeViewSOP.LabelEdit = true;

                int nParent = treeViewSOP.SelectedNode.Index;
                
                TreeNode node = treeViewSOP.Nodes[nParent].Nodes.Add(" ");
                int nIndex = node.Index;

                treeViewSOP.Nodes[nParent].Nodes[nIndex].Nodes.Add("예방");
                treeViewSOP.Nodes[nParent].Nodes[nIndex].Nodes.Add("대비");
                treeViewSOP.Nodes[nParent].Nodes[nIndex].Nodes.Add("대응");
                treeViewSOP.Nodes[nParent].Nodes[nIndex].Nodes.Add("복구");
                treeViewSOP.ExpandAll();

//                 treeViewSOP.SelectedNode = treeViewSOP.Nodes[nParent];
//                 treeViewSOP.Select();

                treeViewSOP.Nodes[nParent].Nodes[nIndex].BeginEdit();
            }
            else if (treeViewSOP.SelectedNode.Level == 1)
            {
                int nNode1 = treeViewSOP.SelectedNode.Parent.Index;
                int nNode2 = treeViewSOP.SelectedNode.Index;

                TreeNode node = treeViewSOP.Nodes[nNode1].Nodes[nNode2].Nodes.Add(" ");
                int nIndex = node.Index;

                treeViewSOP.ExpandAll();

//                 treeViewSOP.SelectedNode = treeViewSOP.Nodes[nNode2];
//                 treeViewSOP.Select();

                treeViewSOP.LabelEdit = true;

                treeViewSOP.Nodes[nNode1].Nodes[nNode2].Nodes[nIndex].BeginEdit();
            }
        }

        private void btnSOPEdit_Click(object sender, EventArgs e)
        {
            //m_isAdd = false;
            if (treeViewSOP.SelectedNode == null || treeViewSOP.SelectedNode.Parent == null) return;

            int nSelect = treeViewSOP.SelectedNode.Index;

            if (treeViewSOP.SelectedNode.Level == 1)
            {
                int nParent = treeViewSOP.SelectedNode.Parent.Index;
                treeViewSOP.Nodes[nParent].Nodes[nSelect].BeginEdit();
            }
            else if (treeViewSOP.SelectedNode.Level == 2)
            {
                int nNode1 = treeViewSOP.SelectedNode.Parent.Parent.Index;
                int nNode2 = treeViewSOP.SelectedNode.Parent.Index;
                treeViewSOP.Nodes[nNode1].Nodes[nNode2].Nodes[nSelect].BeginEdit();
            }                
        }

        private void btnSOPDel_Click(object sender, EventArgs e)
        {
            DeleteNode();
        }
        
        // node의 Full Text
        public int GetNodeText(TreeNode node, out string strFullPath)
        {
            strFullPath = "";

            if (node == null)
                return 0;

            strFullPath = node.Text;
            int nDepth = 1;

            while (node.Parent != null)
            {
                node = node.Parent;
                strFullPath = node.Text + "/" + strFullPath;
                nDepth++;
            }

            return nDepth;
        }

        public void treeViewSOP_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewSOP.SelectedNode.Level == 0)
            {
                treeViewSOP.LabelEdit = false;
            }
            else
            {
                treeViewSOP.LabelEdit = true;
            }

            TreeNode node = treeViewSOP.SelectedNode;
            if (node == null) return;

            string strFullPath;
            int nDepth = GetNodeText(node, out strFullPath);
            m_Main.OnSelectedSOP(nDepth, strFullPath, node);
        }

        private void treeViewSOP_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (treeViewSOP.SelectedNode == null)
                return;

            if (treeViewSOP.SelectedNode.Level == 0)
                treeViewSOP.LabelEdit = false;

            TreeNode node = treeViewSOP.SelectedNode;
            if (node == null) return;

            string strFullPath;
            int nDepth = GetNodeText(node, out strFullPath);
            m_Main.OnChangedSOP(nDepth, strFullPath, node);

            CancelEdit(e);
            //if (treeViewSOP.SelectedNode.Level == 0)
            //{
            //    CancelEdit(e);
            //}
            //else if (treeViewSOP.SelectedNode.Level == 1)
            //{
            //    if (m_isAdd)
            //    {
            //        CancelEdit(e);
            //    }
            //    else
            //    {
            //        CancelEdit2(e);
            //    }
            //}
            //else if (treeViewSOP.SelectedNode.Level == 2)
            //{
            //    CancelEdit2(e);
            //}
        }

        private void CancelEdit(NodeLabelEditEventArgs e)
        {
            if (e.Node == null)
                return;

            TreeNode node = e.Node.Parent;
            if (node == null)
                return;

            //foreach (TreeNode child in treeViewSOP.SelectedNode.Nodes)
            foreach (TreeNode child in node.Nodes)
            {
                if (child == e.Node) continue;

                if (child.Text == e.Label)
                {
                    treeViewSOP.LabelEdit = true;
                    e.CancelEdit = true;
                    e.Node.BeginEdit();
                    break;
                }
            }
        }

        //private void CancelEdit2(NodeLabelEditEventArgs e)
        //{
        //    TreeNode node = e.Node.Parent;

        //    //foreach (TreeNode child in treeViewSOP.SelectedNode.Parent.Nodes)
        //    foreach (TreeNode child in node.Nodes)
        //    {
        //        if (child == e.Node) continue;

        //        if (child.Text == e.Label)
        //        {
        //            treeViewSOP.LabelEdit = true;
        //            e.CancelEdit = true;
        //            e.Node.BeginEdit();
        //            break;
        //        }
        //    }
        //}

        private void DeleteNode()
        {
            if (treeViewSOP.SelectedNode.Level != 0)
            {
                if (DialogResult.Yes == MessageBox.Show("작업한 데이터가 같이 삭제됩니다.\r\n정말 삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    if (treeViewSOP.SelectedNode == null) return;

                    if (treeViewSOP.SelectedNode.Level != 0)
                    {
                        m_Main.OnRemovedSOP(treeViewSOP.SelectedNode);
                        treeViewSOP.SelectedNode.Remove();
                        treeViewSOP.SelectedNode = null;
                    }
                    treeViewSOP.ExpandAll();
                }
            }
        }

        private void treeViewSOP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteNode();
            }
        }

        public ArrayList ValidCheck()
        {
            ArrayList arrNodePath = new ArrayList();
            string strTemp;
            int nIndex = 1;
            foreach (TreeNode rt in treeViewSOP.Nodes)
            {
                foreach (TreeNode first in rt.Nodes)
                {
                    strTemp = first.Text.Trim();
                    if (strTemp == "")
                    {
                        string strPath = nIndex.ToString() + ". " + rt.Text;
                        arrNodePath.Add(strPath);
                        nIndex++;
                    }
                    foreach (TreeNode second in first.Nodes)
                    {
                        strTemp = second.Text.Trim();
                        if (strTemp == "")
                        {
                            string strPath = nIndex.ToString() + ". " + rt.Text + "/" + first.Text;
                            arrNodePath.Add(strPath);
                            nIndex++;
                        }
                    }
                }
            }
            return arrNodePath;
        }
        
        //////////////////////////////////////////////////////////////////////////
        // ListView
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (rdoBtnTeam.Checked)
            {
                GetRegularTeam(textSearch.Text);
            }
            else
            {
                GetCompanyMember(textSearch.Text);
            }
        }

        private void textSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(sender, e);
            }
        }

//         private void GetRegularTeamInfo(string strTeam)
//         {
//             ArrayList arrRegularTeam = new ArrayList();
//             ArrayList arrList = new ArrayList();
//             arrList = m_Main.m_dbMgr.GetRegularTeamName();
//             if (arrList.Count == 0) return;
// 
//             for (int i = 0; i < arrList.Count; i++)
//             {
//                 Data_RegularTeam data = (Data_RegularTeam)arrList[i];
//                
//                 int nIndex = data.TeamName.IndexOf(strTeam);
//                 if (nIndex != -1)
//                 {
//                     arrRegularTeam.Add(data);
//                 }
//             }
//             AddGridRow(arrRegularTeam);
//         }

        // 부서 및 담당자 조회
        private void GetRegularTeam(string strTeam)
        {
            ArrayList arrRegularTeam = new ArrayList();

            ArrayList arrFields = new ArrayList();
            ArrayList arrValues = new ArrayList();
            arrFields.Add("@teamName");
            arrFields.Add("@isRegular");

            //arrValues.Add("");
            //arrValues.Add("1"); //0이면 비상조직
            arrValues.Add("''");
            arrValues.Add("'1'"); //0이면 비상조직

            //System.Data.SqlClient.SqlDataReader reader;
            ArrayList arrResult;
            m_Main.m_dbMgr.RunStoredProcedure("sp_TeamList", arrFields, arrValues, 0, out arrResult);

            //while (reader.Read())
            for (int i = 0; i < arrResult.Count - 2; i =i+3)
            {
                Data_RegularTeam data = new Data_RegularTeam();

                data.ID = m_Main.m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                data.TeamName = m_Main.m_dbMgr.GetStringField(arrResult[i+1].ToString(), "");
                data.ParentTeamID = m_Main.m_dbMgr.GetIntField(arrResult[i+2].ToString(), 0);

                int nIndex = data.TeamName.IndexOf(strTeam);
                if (nIndex != -1)
                {
                    arrRegularTeam.Add(data);
                }
            }

            AddGridRowTeam(arrRegularTeam);
        }
        
        private void GetCompanyMember(string strMember)
        {
            ArrayList arrCompanyMember = new ArrayList();
            ArrayList arrList = new ArrayList();
            arrList = m_Main.m_dbMgr.GetCompanyMemberName();
            if (arrList.Count == 0) return;

            for (int i = 0; i < arrList.Count; i++)
            {
                Data_SearchMember data = (Data_SearchMember)arrList[i];

                int nIndex = data.MemberName.IndexOf(strMember);
                if (nIndex != -1)
                {
                    arrCompanyMember.Add(data);
                }
            }
            AddGridRowMember(arrCompanyMember);
        }

        private void AddGridRowTeam(ArrayList arrList)
        {
            TeamData teamData = null;
            dataGridViewSearch.Rows.Clear();

            for (int i = 0; i < arrList.Count; i++ )
            {
                //Data_RegularTeam data = (Data_RegularTeam)arrList[i];
                //dataGridView.Rows.Add(data.TeamName);
                DataGridViewRowEx<TeamData> gridRow = new DataGridViewRowEx<TeamData>();
                DataGridViewCell cell = new DataGridViewImageCell();

                Data_RegularTeam data = (Data_RegularTeam)arrList[i];

                if (m_dicRegularMember.ContainsKey(data.ID))
                    teamData = m_dicRegularMember[data.ID];
                else
                {
                    if (!FormTeam.Instance(true).FindItem(data.ID, false, out teamData))
                        continue;
                    m_dicRegularMember[data.ID] = teamData;
                }

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.TeamName;
                gridRow.Cells.Add(cell);
                gridRow.Data = teamData;
                dataGridViewSearch.Rows.Add(gridRow);
            }
        }

        private void AddGridRowMember(ArrayList arrList)
        {
            TeamData teamData = null;
            dataGridViewSearch.Rows.Clear();

            for (int i = 0; i < arrList.Count; i++)
            {
                Data_SearchMember data = (Data_SearchMember)arrList[i];

                // 팀원의 ID가 팀 ID와 겹칠수 있으므로, 팀원의 ID는 음수로 저장
                if (m_dicRegularMember.ContainsKey(-data.MemberID))
                    teamData = m_dicRegularMember[-data.MemberID];
                else
                {
                    if (!FormTeam.Instance(true).FindItem(data.MemberID, true, out teamData))
                        continue;
                    m_dicRegularMember[-data.MemberID] = teamData;
                }

                //DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewRowEx<TeamData> gridRow = new DataGridViewRowEx<TeamData>();
                DataGridViewCell cell = new DataGridViewImageCell();

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.TeamName;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.MemberName;
                gridRow.Cells.Add(cell);

                gridRow.Data = teamData;
                dataGridViewSearch.Rows.Add(gridRow);
            }
        }

        private void dataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || dataGridViewSearch.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                return;
            }

            /*m_strValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            ArrayList arrTeamData = new ArrayList();
            int nDataCount = FormTeam.Instance(true).FindItem(m_strValue, ref arrTeamData);
            m_Main.GetProcess().ArrTeamData = arrTeamData;*/

            //DataGridViewRow row = dataGridView.Rows[e.RowIndex];
            DataGridViewRowEx<TeamData> row = (DataGridViewRowEx<TeamData>)dataGridViewSearch.Rows[e.RowIndex];
            m_selectedTeamData = row.Data;

            //dataGridView.DoDragDrop(dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), DragDropEffects.Copy);
            m_Main.GetProcess().m_label = new Label();
            
            //label 투명배경
            m_Main.GetProcess().m_label.Parent = m_Main.GetProcess();
            m_Main.GetProcess().m_label.BackColor = Color.Transparent;
            m_Main.GetProcess().m_label.AutoSize = true;

            m_Main.GetProcess().m_label.Text = dataGridViewSearch.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
//             m_Main.GetProcess().m_label.Location = new System.Drawing.Point(0, 0);
//             this.Controls.Add(m_Main.GetProcess().m_label);

            IsDragDrop = true;

        }

        public DataGridView GetGridView()
        {
            return dataGridViewSearch;
        }

        public TeamData GetSelectedValue()
        {
            //return m_strValue;
            return m_selectedTeamData;
        }

        private void dataGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            IsDragDrop = false;
        }

        private void dataGridView_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDragDrop)
            {
                Size sz = this.Size;
                Point pt = this.Location;

                Size szForm = m_Main.GetProcess().Size;
                Point ptForm = m_Main.GetProcess().Location;

                Point ptGroup = groupBox2.Location;
                Point ptGrid = dataGridViewSearch.Location;


                int x = e.X + pt.X + ptGroup.X + ptGrid.X;
                int y = e.Y + pt.Y + ptGroup.Y + ptGrid.Y;
                //MouseEventArgs newEvent = new MouseEventArgs(MouseButtons.Left, 1, e.X + pt.X + ptGroup.X + ptGrid.X, e.Y + pt.Y + ptGroup.Y + ptGrid.Y, 0);

                
                m_Main.GetProcess().m_label.Location = new System.Drawing.Point(x, y);
                m_Main.GetProcess().Controls.Add(m_Main.GetProcess().m_label);

                // Focus를 Grid에서 Form으로 넘기기 위하여 WM_MBUTTONDOWN 이벤트를 강제로 발생시킴
                // WM_MBUTTONDOWM
                ZBobb.win32.SendMessage(m_Main.GetProcess().Handle, 0x0207, (IntPtr)0, (IntPtr)0);
            }
        }

        public bool SaveSubDisasterCategory(VersionData versionData, bool isNewVersion, int nOwnerID, int transaction, out int nVersionID, out Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster)
        {
            dicSubDisaster = null;
            nVersionID = -1;

            // 작업한 내용이 있는가?
            bool isWorked = false;

            foreach (TreeNode node in treeViewSOP.Nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    isWorked = true;
                    break;
                }
            }

            if (!isWorked)
            {
                MessageBox.Show("작업한 내역이 없습니다.\r\n버전을 저장하지 않습니다.", "저장 실패");
                return false;
            }

            string strSQL;

            if (isNewVersion)
            {
                string strCreateTime = versionData.CreateTime.ToShortDateString() + string.Format(" {0:00}:{1:00}:{2:00}", versionData.CreateTime.Hour, versionData.CreateTime.Minute, versionData.CreateTime.Second);
                string strLastAccessTime = versionData.LastAccessTime.ToShortDateString() + string.Format(" {0:00}:{1:00}:{2:00}", versionData.LastAccessTime.Hour, versionData.LastAccessTime.Minute, versionData.LastAccessTime.Second);

                // 새로운 버전 추가
                strSQL = string.Format("Insert into Version (VersionName, OwnerID, CreateTime, LastAccessTime, Description) values ('{0}', {1}, '{2}', '{3}', '{4}')",
                    versionData.VersionName, nOwnerID, strCreateTime, strLastAccessTime, versionData.Description);
                //m_Main.m_dbMgr.Execute(strSQL, transaction);
                m_Main.m_dbMgr.GetResultData(strSQL, transaction);
            }

            //SqlDataReader reader;
            // 대소문자 구분 : collate Korean_Wansung_CS_AS
            strSQL = "select id from Version where VersionName collate Korean_Wansung_CS_AS = '" + versionData.VersionName + "'";
            //m_Main.m_dbMgr.ReadDB(strSQL, transaction, out reader);
            ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, transaction);

            if (arrResult != null)
            {
                nVersionID = m_Main.m_dbMgr.GetIntField(arrResult[0].ToString(), 0);
            }
            else
            {
                return false;
            }

            Dictionary<string, int> dicDisaster = new Dictionary<string, int>();
            dicSubDisaster = new Dictionary<TreeNode, SubDisasterCategoryData>();

            arrResult.Clear();
            strSQL = "select * from DisasterCategory";
            //m_Main.m_dbMgr.ReadDB(strSQL, transaction, out reader);
            arrResult = m_Main.m_dbMgr.GetResultData(strSQL, transaction);

            for (int i = 0; i < arrResult.Count - 1; i = i + 2)
            {
                int nID = m_Main.m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                string strName = m_Main.m_dbMgr.GetStringField(arrResult[i+1].ToString(), "");
                dicDisaster[strName] = nID;
            }

            arrResult.Clear();
            strSQL = "select Max(id) from SubDisasterCategory";
            //m_Main.m_dbMgr.ReadDB(strSQL, transaction, out reader);
            arrResult = m_Main.m_dbMgr.GetResultData(strSQL, transaction);

            int nLastSubCategoryID = 0;

            // 가장 나중에 생성된 SubDisasterCategory ID 얻어오기
            if (arrResult != null)
            {
                nLastSubCategoryID = m_Main.m_dbMgr.GetIntField(arrResult[0].ToString(), 0);
            }
            
            foreach (TreeNode node in treeViewSOP.Nodes)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    InsertSubDisasterCategory(dicDisaster, dicSubDisaster, nVersionID, transaction, ref nLastSubCategoryID, child);
                }
            }

            return true;
        }

        private void InsertSubDisasterCategory(Dictionary<string, int> dicDisaster, Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster, int nVersionID, int transaction, ref int nLastSubCategoryID, TreeNode node)
        {
            if (node.Parent == null)
                return;

            if (node.Parent.Parent == null)
            {
                // Disaster 바로 아래의 노드
                if (!dicDisaster.ContainsKey(node.Parent.Text))
                    return;

                int nDisasterID = dicDisaster[node.Parent.Text];

                string strSQL = string.Format("Insert into SubDisasterCategory (id, DisasterID, SubCategoryName, ParentSubCategoryID, VersionID) values ({0}, {1}, '{2}', NULL, '{3}')",
                        ++nLastSubCategoryID, nDisasterID, node.Text, nVersionID);
                //m_Main.m_dbMgr.Execute(strSQL, transaction);
                m_Main.m_dbMgr.GetResultData(strSQL, transaction);

                SubDisasterCategoryData data = new SubDisasterCategoryData(nLastSubCategoryID, nDisasterID, node.Text, -1);
                dicSubDisaster[node] = data;
            }
            else
            {
                if (!dicSubDisaster.ContainsKey(node.Parent))
                    return;

                SubDisasterCategoryData parentData = dicSubDisaster[node.Parent];

                string strSQL = string.Format("Insert into SubDisasterCategory (id, DisasterID, SubCategoryName, ParentSubCategoryID, VersionID) values ({0}, {1}, '{2}', {3}, '{4}')",
                        ++nLastSubCategoryID, parentData.DisasterID, node.Text, parentData.ID, nVersionID);
                //m_Main.m_dbMgr.Execute(strSQL, transaction);
                m_Main.m_dbMgr.GetResultData(strSQL, transaction);

                SubDisasterCategoryData data = new SubDisasterCategoryData(nLastSubCategoryID, parentData.DisasterID, parentData.SubCategoryName, parentData.ID);
                dicSubDisaster[node] = data;
            }

            foreach (TreeNode child in node.Nodes)
            {
                InsertSubDisasterCategory(dicDisaster, dicSubDisaster, nVersionID, transaction, ref nLastSubCategoryID, child);
            }
        }

        private TreeNode FindTreeItem(TreeNodeCollection nodes, string strNodeText)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text == strNodeText)
                    return node;
            }

            return null;
        }

        public bool LoadSubDisaster(int nVersionID, out Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster, out Dictionary<int, TreeNode> dicSubNode)
        {
            string strSQL = "select * from DisasterCategory";

            //SqlDataReader reader;
            //m_Main.m_dbMgr.ReadDB(strSQL, null, out reader);
            ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

            Dictionary<int, string> dicDisaster = new Dictionary<int, string>();

            for (int i = 0; i < arrResult.Count - 1; i = i + 2)
            {
                int nID = m_Main.m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                string strDisaster = m_Main.m_dbMgr.GetStringField(arrResult[i+1].ToString(), "");
                dicDisaster[nID] = strDisaster;
            }

            arrResult.Clear();
            strSQL = "select * from SubDisasterCategory where VersionID = " + nVersionID.ToString();
            //m_Main.m_dbMgr.ReadDB(strSQL, null, out reader);
            arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

            dicSubDisaster = new Dictionary<TreeNode, SubDisasterCategoryData>();
            dicSubNode = new Dictionary<int, TreeNode>();

            for (int i = 0; i < arrResult.Count - 4; i = i + 5)
            {
                int nID = m_Main.m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                int nDisasterID = m_Main.m_dbMgr.GetIntField(arrResult[i+1].ToString(), 0);
                string strSubDisaster = m_Main.m_dbMgr.GetStringField(arrResult[i+2].ToString(), "");
                int nParentID = m_Main.m_dbMgr.GetIntField(arrResult[i+3].ToString(), -1);

                if (!dicDisaster.ContainsKey(nDisasterID))
                {
                    return false;
                }

                TreeNode node = FindTreeItem(treeViewSOP.Nodes, dicDisaster[nDisasterID]);
                if (node == null)
                {
                    return false;
                }

                if (nParentID < 0)
                {
                    node = node.Nodes.Add(strSubDisaster);
                    if (node == null)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!dicSubNode.ContainsKey(nParentID))
                    {
                        return false;
                    }

                    node = dicSubNode[nParentID];
                    node = node.Nodes.Add(strSubDisaster);

                    if (node == null)
                    {
                        return false;
                    }
                }

                SubDisasterCategoryData data = new SubDisasterCategoryData(nID, nDisasterID, strSubDisaster, nParentID);
                dicSubDisaster[node] = data;
                dicSubNode[nID] = node;
            }

            return true;
        }

        public void ExpandAllTreeView()
        {
            treeViewSOP.ExpandAll();
        }

        public void SelectItem(TreeNode node)
        {
            treeViewSOP.SelectedNode = node;
        }

        public void NewSOP()
        {
            // 첫번째 단계의 노드들만 남기고 모두 없앤다.
            foreach (TreeNode node in treeViewSOP.Nodes)
            {
                node.Nodes.Clear();
            }
        }
    }

    class DataGridViewRowEx<T> : DataGridViewRow
    {
        private T m_data;

        public T Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
    }

    public class SubDisasterCategoryData
    {
        private int m_nID = -1;
        private int m_nDisasterID = -1;
        private string m_strSubCategoryName = "";
        private int m_nParentSubCategoryID = -1;

        public SubDisasterCategoryData()
        {
        }

        public SubDisasterCategoryData(int nID, int nDisasterID, string strSubCategoryName, int nParentSubCategoryID)
        {
            m_nID = nID;
            m_nDisasterID = nDisasterID;
            m_strSubCategoryName = strSubCategoryName;
            m_nParentSubCategoryID = nParentSubCategoryID;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }

        public string SubCategoryName
        {
            get { return m_strSubCategoryName; }
            set { m_strSubCategoryName = value; }
        }

        public int ParentSubCategoryID
        {
            get { return m_nParentSubCategoryID; }
            set { m_nParentSubCategoryID = value; }
        }
    }
}
