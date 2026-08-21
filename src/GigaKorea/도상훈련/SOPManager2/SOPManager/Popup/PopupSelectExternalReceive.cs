using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DBUtility2;

namespace SOPManager
{
    public partial class PopupSelectExternalReceive : Form
    {
        ArrayList m_arrRcvPhone = new ArrayList();
        ArrayList m_arrRcvFax = new ArrayList();
        ArrayList m_arrOriginTeam = new ArrayList();

        // 새로 추가되거나 변경된 것을 포함한 Data_ExternalTeam List
        // Grid Row Index, 행별 Data_ExternalTeam
        private Dictionary<int, Data_ExternalTeam> m_dicExternalTeamList = new Dictionary<int, Data_ExternalTeam>();
        // 삭제될 Data_ExternalTeam List
        private ArrayList m_arrRemoveExternalTeamList = new ArrayList();

        //private int m_nItemID;

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public PopupSelectExternalReceive()
        {
            InitializeComponent();
            InitTree();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

		private ArrayList mSelectedTeam = new ArrayList();
		public ArrayList SelectedTeamList
		{
			get { return mSelectedTeam; }
			set 
			{
				mSelectedTeam.Clear();
				if( value != null)
				{
					mSelectedTeam.AddRange(value);
				}			

				if(mSelectedTeam != null)
				{
					InitGrid();

                    
				}
			}
		}

        private void InitGrid()
        {
            //m_nItemID = nID;

			AddSelectedTeam(mSelectedTeam);

            //foreach (Data_ExternalTeam data in FormMain.Instance.ExternalTeam)
            //{
            //    if (mSelectedTeam.Count == 0)
            //    {
            //        AllExternalTeam(data);
            //    }
            //    else
            //    {
            //        bool isCheck = false;
            //        foreach (DataGridViewRow row in dataGridView.Rows)
            //        {
            //            if ((int)row.Tag == data.ID)
            //            {
            //                isCheck = true;
            //                break;
            //            }
            //        }

            //        if(isCheck)
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            AllExternalTeam(data);
            //        }
            //    }
            //}
        }

        public void InitTree()
        {
            treeViewTeam.Nodes.Clear();
         
            LoadExternalTeamTree();
        
        }

        private void LoadExternalTeamTree()
        {
            ArrayList arrExternalTeam = FormMain.Instance.ExternalTeam;

            foreach (Data_ExternalTeam data in arrExternalTeam)
            {
                if (data.ParentTeam == null)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;

                    AddExternalCompanySubTeamTree(data, node);
                }
            }
            treeViewTeam.ExpandAll();
        }
        
        private void AddExternalCompanySubTeamTree(Data_ExternalTeam teamParent, TreeNode nodeParent)
        {
            foreach (Data_ExternalTeam team in FormMain.Instance.ExternalTeam)
            {
                if (team.ParentTeam != null && team.ParentTeam.ID == teamParent.ID)
                {
                    TreeNode node = nodeParent.Nodes.Add(team.TeamName);
                    node.Tag = team.ID;

                    AddExternalCompanySubTeamTree(team, node);
                }
            }
        }

        private TreeNode FindNode(int nTag, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewTeam.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if ((int)node.Tag == nTag)
                    return node;

                TreeNode result = FindNode(nTag, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewTeam.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if (strValue == node.Text)
                    return node;
                TreeNode result = FindNode(strValue, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void treeViewTeam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // 협력업체 회사명을 클릭하였다.

            // 협력업체 회사명도 선택할 수 있도록 변경함. skkim 2015-08-03
            //if (m_currentTeamType == SOPTeam.SOPTeamType.External && e.Node.Parent == null)
            //     return;
            treeViewTeam.SelectedNode = e.Node;
        }
        
        private void AllExternalTeam(Data_ExternalTeam data)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            Data_ExternalTeam team = new Data_ExternalTeam();
            
            cell.Value = data.TeamName;
            gridRow.Cells.Add(cell);
            team.TeamName = data.TeamName;
            
            cell = new DataGridViewTextBoxCell();
            cell.Value = data.PhoneNumber;
            gridRow.Cells.Add(cell);
            team.PhoneNumber = data.PhoneNumber;
            
            cell = new DataGridViewTextBoxCell();
            cell.Value = data.FaxNumber;
            gridRow.Cells.Add(cell);
            team.FaxNumber = data.FaxNumber;
            
            gridRow.Tag = data.ID;
            team.ID = data.ID;

            if (dataGridViewReceive.AllowUserToAddRows)
                m_dicExternalTeamList[dataGridViewReceive.Rows.Count - 1] = team;
            else
                m_dicExternalTeamList[dataGridViewReceive.Rows.Count] = team;

            dataGridViewReceive.Rows.Add(gridRow);
        }

        private void AddSelectedTeam(ArrayList arr)
        {
            Image img = new Bitmap(global::SOPManager.Properties.Resources.call_18);

            foreach (Sections.ExternalTeamData exData in arr)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewImageCell ImageCell = new DataGridViewImageCell();
                ImageCell.Value = img;
                gridRow.Cells.Add(ImageCell);

                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = exData.TeamName;
                gridRow.Cells.Add(cell);


                Sections.SOPTeam newTeam = new Sections.SOPTeam();
                newTeam.TeamID = exData.TeamID;
                newTeam.TeamName = exData.TeamName;
                newTeam.TeamType = Sections.SOPTeam.SOPTeamType.External;

                cell.Tag = newTeam;

                cell = new DataGridViewTextBoxCell();
                cell.Value = exData.PhoneNumber;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = exData.FaxNumber;
                gridRow.Cells.Add(cell);             

                gridRow.Tag = Sections.SOPTeam.SOPTeamType.External;
                dataGridView.Rows.Add(gridRow);

                m_arrOriginTeam.Add(gridRow);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
			GetSelectedReceive(true);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
			GetSelectedReceive(false);
            
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool FindTeamName2(DataGridViewCell cell)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                Sections.SOPTeam team1 = (Sections.SOPTeam)row.Cells[1].Tag;
                Sections.SOPTeam team2 = (Sections.SOPTeam)cell.Tag;
                if ((row.Cells[1].Value.ToString() == cell.Value.ToString()) && (team1.TeamType == team2.TeamType))
                {
                    if (team1.TeamID != -1 && team2.TeamID != -1)
                    {
                        if (team1.TeamID != team2.TeamID)
                            return false;
                    }
                    return true;
                }
                    
            }
            return false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (treeViewTeam.SelectedNode == null)
                return;
            
            Image img = new Bitmap(global::SOPManager.Properties.Resources.call_18);

            Sections.SOPTeam newTeam = new Sections.SOPTeam();
            newTeam.TeamID = (int)treeViewTeam.SelectedNode.Tag;
            newTeam.TeamName = treeViewTeam.SelectedNode.Text;
            newTeam.TeamType = Sections.SOPTeam.SOPTeamType.External;
                        
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewImageCell ImageCell = new DataGridViewImageCell();
            ImageCell.Value = img;
            gridRow.Cells.Add(ImageCell);      

            DataGridViewCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = treeViewTeam.SelectedNode.Text;
            gridRow.Cells.Add(cell1);
            cell1.Tag = newTeam;

            DataGridViewCell cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            gridRow.Cells.Add(cell);

            if (!FindTeamName2(cell1))
            {
                gridRow.Tag = Sections.SOPTeam.SOPTeamType.External;
                dataGridView.Rows.Add(gridRow);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {            
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (row.Cells[0].Selected)
                {
                    dataGridView.Rows.Remove(row);
                }
            }
        } 

        private string GetSelectedReceive(bool isOK)
        {
            int nRow = 0;
            string strValue = "";

            mSelectedTeam.Clear();
            if (dataGridView.RowCount != 0)
            {
                if (isOK)
                {
					
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        strValue += row.Cells[1].Value.ToString();
                        if (nRow != dataGridView.RowCount - 1)
                        {
                            strValue += ", ";
                            nRow++;
                        }

                        Sections.ExternalTeamData data = new Sections.ExternalTeamData();

                        Sections.SOPTeam team = (Sections.SOPTeam)row.Cells[1].Tag;
                        data.TeamID = team.TeamID;
                        data.TeamName = row.Cells[1].Value.ToString();
                        data.PhoneNumber = row.Cells[2].Value.ToString();
                        data.FaxNumber = row.Cells[3].Value.ToString();

						mSelectedTeam.Add(data);
                    }
                }
                else
                {
                     foreach (DataGridViewRow row in m_arrOriginTeam)
                     {
                         strValue += row.Cells[1].Value.ToString();
                         if (nRow != m_arrOriginTeam.Count - 1)
                         {
                             strValue += ", ";
                             nRow++;
                         }

                         Sections.ExternalTeamData data = new Sections.ExternalTeamData();

                         Sections.SOPTeam team = (Sections.SOPTeam)row.Cells[1].Tag;
                         data.TeamID = team.TeamID;
                         data.TeamName = row.Cells[1].Value.ToString();
                         data.PhoneNumber = row.Cells[2].Value.ToString();
                         data.FaxNumber = row.Cells[3].Value.ToString();

                         mSelectedTeam.Add(data);
                     }
                }
            }

            return strValue;
        }

        // 기존에 존재하던 외부팀 데이터인가 여부.
        // 만일 기존에 존재하던 팀이라면 데이터가 바뀌었는지 여부
        // Return 값 : 0(기존에 존재하던 팀이며 아무것도 바뀌지 않음)
        //             1(기존에 존재하던 팀이며, 데이터가 바뀌었음)
        //            -1(새로운 팀)
        //            -1(잘못된 데이터)
        private int CheckExternalTeam(Data_ExternalTeam team)
        {
            if (team.TeamName.Length == 0)
                return -2;

            foreach (Data_ExternalTeam data in FormMain.Instance.ExternalTeam)
            {
                if (data.TeamName == team.TeamName)
                {
                    team.ID = data.ID;

                    if (team.PhoneNumber.Length == 0)
                        return -2;

                    if (team.PhoneNumber == data.PhoneNumber &&
                        team.FaxNumber == data.FaxNumber)
                        return 0;
                    else
                        return 1;
                }
            }

            team.ID = -1;
            return -1;
        }

        private void SaveReceiveList()
        {
            // Row Index, ExternalTeam
            Dictionary<int, Data_ExternalTeam> dicNewTeam = new Dictionary<int, Data_ExternalTeam>();
            ArrayList arrUpdateTeam = new ArrayList();

            foreach (KeyValuePair<int, Data_ExternalTeam> pair in m_dicExternalTeamList)
            {
                int nResult = CheckExternalTeam(pair.Value);

                if (nResult == 1)
                    arrUpdateTeam.Add(pair.Value);
                else if (nResult == -1)
                {
                    dicNewTeam[pair.Key] = pair.Value;
                }
            }

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strRemoveIDs = "", strSQL;

            ////////////////////////////////////////////////////////////////////
            // 데이터 삭제
            foreach (Data_ExternalTeam team in m_arrRemoveExternalTeamList)
            {
                if (strRemoveIDs.Length == 0)
                    strRemoveIDs = team.ID.ToString();
                else
                    strRemoveIDs += ", " + team.ID.ToString();
            }

            if (strRemoveIDs.Length > 0)
            {
                if (IOManager.DeleteActionStepUsingTeam(dbMgr, strRemoveIDs, 2) == false)
                    return;

                strSQL = string.Format("Delete from ExternalTeam where id in ({0})", strRemoveIDs);
                if (dbMgr.GetResultData(strSQL) == null)
                    return;
            }

            m_arrRemoveExternalTeamList.Clear();
            ////////////////////////////////////////////////////////////////////

            strSQL = "select max(id) from ExternalTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            int nTeamID;

            if (arrResult == null || arrResult.Count == 0)
                nTeamID = 0;
            else
				nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            //foreach (Data_ExternalTeam team in arrNewTeam)
            foreach (KeyValuePair<int, Data_ExternalTeam> pair in dicNewTeam)
            {
                int nRowIndex = pair.Key;
                Data_ExternalTeam team = pair.Value;

                string strFaxNumber = team.FaxNumber.Length > 0 ? "'" + team.FaxNumber + "'" : "NULL";

                strSQL = string.Format("Insert into ExternalTeam (ID, TeamName, PhoneNumber, FaxNumber, SiteID) values ({0}, '{1}', '{2}', {3}, {4})",
                    ++nTeamID, team.TeamName, team.PhoneNumber, strFaxNumber, FormMain.Instance.SiteID);

                if (dbMgr.GetResultData(strSQL) == null)
                    return;

                SetNewExternalTeamID(nRowIndex, nTeamID);
            }

            foreach (Data_ExternalTeam team in arrUpdateTeam)
            {
                string strFaxNumber = team.FaxNumber.Length > 0 ? "'" + team.FaxNumber + "'" : "NULL";

                strSQL = string.Format("Update ExternalTeam set PhoneNumber = '{0}', FaxNumber = {1} where id = {2}", 
                    team.PhoneNumber, strFaxNumber, team.ID);

                if (dbMgr.GetResultData(strSQL) == null)
                    return;
            }
        }

        private void SetNewExternalTeamID(int nRowIndex, int nNewID)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if ((int)row.Tag == -nRowIndex)
                {
                    row.Tag = nNewID;
                    return;
                }
            }
        }

        private void dataGridViewReceive_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            object value = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            if (value == null)
                return;

            string strValue = value.ToString();
            Data_ExternalTeam team = m_dicExternalTeamList.ContainsKey(e.RowIndex) ? m_dicExternalTeamList[e.RowIndex] : null;

            if (e.ColumnIndex == 0)
            {
                if (team != null && !CheckDuplicate(grid, e.RowIndex, strValue))
                {
                    value = team.TeamName;
                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_ExternalTeam();
                        m_dicExternalTeamList[e.RowIndex] = team;
                    }

                    // 새로 추가된 TeamName 이므로 ID를 -1로 둔다.(DB에 존재하지 않음)
                    team.TeamName = strValue;
                    team.ID = -1;
                }
            }
            else if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                bool isCheck = FormMain.Instance.GetPageLevel().numericCheck(strValue);

                if (!isCheck)
                {
					UnE.Utility.UMessageBoxRibbon.Show("숫자 입력만 가능합니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    if (team == null)
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = e.ColumnIndex == 1 ? team.PhoneNumber : team.FaxNumber;
                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_ExternalTeam();
                        m_dicExternalTeamList[e.RowIndex] = team;
                    }

                    if (e.ColumnIndex == 1)
                        team.PhoneNumber = strValue;
                    else
                        team.FaxNumber = strValue;
                }
            }
        }

        // nRowIndex의 첫번째 Cell의 텍스트가 다른 행에 이미 존재하는지 여부를 확인한다.
        // 이미 존재하면 false, 존재하지 않으면 true를 리턴한다.
        private bool CheckDuplicate(DataGridView grid, int nRowIndex, string strValue)
        {
            int nRowCount = grid.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                if (i == nRowIndex)
                    continue;

                if (grid.Rows[i].Cells[0].Value.ToString() == strValue)
                    return false;
            }

            return true;
        }

        private void dataGridViewReceive_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (sender != dataGridViewReceive)
                    return;

                if (dataGridViewReceive.SelectedRows == null || dataGridViewReceive.SelectedRows.Count == 0)
                    return;

                int nRowCount = dataGridViewReceive.Rows.Count;
                if (dataGridViewReceive.AllowUserToAddRows)
                    nRowCount--;

                int nRowIndex = dataGridViewReceive.SelectedRows[0].Index;
                if (nRowIndex >= nRowCount)
                    return;

                if (!m_dicExternalTeamList.ContainsKey(nRowIndex))
                {
                    if (dataGridViewReceive.SelectedRows[0].Tag != null)
                    {
                        int nExternalTeamID = (int)dataGridViewReceive.SelectedRows[0].Tag;

                        if (nExternalTeamID > 0)
                        {
                            Data_ExternalTeam team = new Data_ExternalTeam();
                            team.ID = nExternalTeamID;
                            m_arrRemoveExternalTeamList.Add(team);
                        }
                    }

                    dataGridViewReceive.Rows.RemoveAt(nRowIndex);
                    return;
                }

                dataGridViewReceive.Rows.RemoveAt(nRowIndex);

                Data_ExternalTeam selectedTeam = m_dicExternalTeamList[nRowIndex];
                if (selectedTeam.ID > 0)
                    m_arrRemoveExternalTeamList.Add(selectedTeam);

                /////////////////////////////////////////////////////////////////
                // dictionary의 데이터를 삭제된 행을 기준으로 하나씩 아래로 내린다.
                for (int i = nRowIndex + 1; i < nRowCount; i++)
                {
                    m_dicExternalTeamList[i - 1] = m_dicExternalTeamList[i];
                }

                m_dicExternalTeamList.Remove(nRowCount - 1);
                /////////////////////////////////////////////////////////////////
            }
        }

        private void PopupSelectReceive_MouseDown(object sender, MouseEventArgs e)
        {
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupSelectReceive_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void PopupSelectReceive_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }
        
    }
}
