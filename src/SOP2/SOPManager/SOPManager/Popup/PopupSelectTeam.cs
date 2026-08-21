using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPManager
{
    public partial class PopupSelectTeam : Form
    {
        private Sections.SectionProcess m_section;
        // 0(평일), 1(휴일), 2(외부 기관), 3(사용자 정의 조직)
        private int m_nCurrentTeamType = -1;

        PropertiesProcess propertiesProcess = null;

        ArrayList m_arrRemove = new ArrayList();

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public PopupSelectTeam(Sections.SectionProcess section)
        {
            InitializeComponent();

            propertiesProcess = FormMain.Instance.GetPageLevel().GetPropertiesProcess();
            m_section = section;

            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)m_section.GetParent();
            m_nCurrentTeamType = panel.TeamType;
            
            InitTree();
            //InitGrid();
        }

        private void InitGrid()
        {
            if (propertiesProcess.ArrSelected.Count == 0) return;

            foreach (DataGridViewRow row in propertiesProcess.ArrSelected)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = row.Cells[0].Value;
                cell.Tag = row.Cells[0].Tag;
                gridRow.Cells.Add(cell);

                dataGridView.Rows.Add(gridRow);
            }
        }

        private void SetTeamTypeLabel(int nTeamType)
        {
            if (nTeamType == 0)
                labelTeamType.Text = "평일 비상 조직";
            else if (nTeamType == 1)
                labelTeamType.Text = "야간 및 휴일 비상 조직";
            else if (nTeamType == 2)
                labelTeamType.Text = "외부 기관";
            else if (nTeamType == 3)
                labelTeamType.Text = "사용자 정의 조직";
            else if (nTeamType == 4)
                labelTeamType.Text = "정규 조직";
        }

        private void LoadExternalTeamTree()
        {
            ArrayList arrExternalTeam = FormMain.Instance.ExternalTeam;

            foreach (Data_ExternalTeam data in arrExternalTeam)
            {
                TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                node.Tag = data.ID;
                treeViewTeam.ExpandAll();
            }
        }

        private void LoadUserDefinedTeamTree()
        {
            ArrayList arrUserDefinedTeam = FormMain.Instance.UserDefinedTeam;

            foreach (Data_UserDefinedTeam data in arrUserDefinedTeam)
            {
                TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                node.Tag = data.ID;
                treeViewTeam.ExpandAll();
            }
        }

        private void LoadTemporaryNormalTeamTree()
        {
            ArrayList arrRegularTeam = FormMain.Instance.TemporaryNormalTeam;

            foreach (Data_NormalTeam data in arrRegularTeam)
            {
                if (data.ParentTeamID == 0)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                    if (child == null) return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data.ID;
                }
            }

            treeViewTeam.ExpandAll();
        }

        private void LoadTemporaryEmergencyTeamTree()
        {
            ArrayList arrEmergencyTeam = FormMain.Instance.TemporaryEmergencyTeam;

            foreach (Data_EmergencyTeam data in arrEmergencyTeam)
            {
                if (data.ParentTeamID == 0)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                    if (child == null) return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data.ID;
                }
            }

            treeViewTeam.ExpandAll();
        }

        private void LoadRegularTeamTree()
        {
            ArrayList arrRegularTeam = FormMain.Instance.RegularTeam;

            foreach (Data_RegularTeam data in arrRegularTeam)
            {
                if (data.ParentTeamID == 0)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                    if (child == null) return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data.ID;
                }
            }

            treeViewTeam.ExpandAll();
        }

        public void InitTree()
        {
            if (m_section == null)
                return;

            treeViewTeam.Nodes.Clear();

            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)m_section.GetParent();
            int nTeamID = panel.TeamID;
            int nTeamType = m_nCurrentTeamType;//panel.TeamType;

            SetTeamTypeLabel(nTeamType);

            if (nTeamType == 2)         // 외부 조직
                LoadExternalTeamTree();
            else if (nTeamType == 3)    // 사용자 정의 조직
                LoadUserDefinedTeamTree();
            else if (nTeamType == 0)    // 평일 비상 조직
                LoadTemporaryNormalTeamTree();
            else if (nTeamType == 1)    // 야간 및 휴일 비상 조직
                LoadTemporaryEmergencyTeamTree();
            else// if (nTeamType == 4)  // 정규 조직
                LoadRegularTeamTree();
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

        public void GetSection(Sections.SectionProcess section)
        {
            m_section = section;
            
            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)m_section.GetParent();
            m_nCurrentTeamType = panel.TeamType;

            string[] strTeamList = m_section.TextDown.Split(new char[] {','});

            if (strTeamList.Length == 0) return;
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;
            AddRowData(data.TeamList);

        }

        private void AddRowData(ArrayList arrTeamList)
        {
            dataGridView.Rows.Clear();
            foreach (Sections.SOPTeam sopTeam in arrTeamList)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = sopTeam.TeamName;
                cell.Tag = sopTeam.TeamID;
                gridRow.Tag = sopTeam.TeamType;
                gridRow.Cells.Add(cell);

                dataGridView.Rows.Add(gridRow);
            }
        }
        
        private int FindTagInNode(string strValue)
        {
            TreeNode node = FindNode(strValue, treeViewTeam.Nodes);
            if (node != null)
            {
                if (node.Text == strValue)
                    return (int)node.Tag;
            }

            return -1;
        }

		private string GetOriginalTeam()
		{
			string strValue = "";
			int nCount = 0;
			// m_nTeamType : 0(평일 비상조직), 1(휴일 및 야간 비상 조직), 2(외부 기관), 3(사용자 정의 조직)
			Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;
		
			foreach (Sections.SOPTeam team in data.TeamList)
			{
				strValue += team.TeamName;
				if (nCount > 1 && nCount != data.TeamList.Count - 1)
				{
					strValue += ", ";
				}
				nCount++;
			}
			return strValue;
		}

		private void ApplySelectedTeam()
		{
			// m_nTeamType : 0(평일 비상조직), 1(휴일 및 야간 비상 조직), 2(외부 기관), 3(사용자 정의 조직)
			Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;

			// 기존 데이터는 지운다.
			if (data.TeamList.Count != 0)
				data.TeamList.Clear();

			foreach (DataGridViewRow row in dataGridView.Rows)
			{
				Sections.SOPTeam sopTeam = new Sections.SOPTeam();
				sopTeam.TeamID = (int)row.Cells[0].Tag;
				sopTeam.TeamType = (int)row.Tag;//nType;
				sopTeam.TeamName = row.Cells[0].Value.ToString();

				ArrayList arrTeamList = data.TeamList;
				arrTeamList.Add(sopTeam);
			}
		}

        private string GetSelectedTeam()
        {
            string strValue = "";
            int nRow = 0;

            // m_nTeamType : 0(평일 비상조직), 1(휴일 및 야간 비상 조직), 2(외부 기관), 3(사용자 정의 조직)
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {              
                strValue += (string)row.Cells[0].Value;
                if (dataGridView.Rows.Count > 1 && nRow != dataGridView.Rows.Count - 1)
                {
                    strValue += ", ";
                    nRow++;
                }
            }
            return strValue;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
			//RemoveList();
			//if (dataGridView.Rows.Count == 0)
			//{
			//    propertiesProcess.SelectedTeam = "";
			//}
			//else
			//{				
			//    string strOrgValue = GetOriginalTeam();
			//    string strValue = GetSelectedTeam();
			//    if (strOrgValue != strValue)
			//    {
			//        UndoRedoManager.Instance.SaveSnapshot();
			//        ApplySelectedTeam();
			//        propertiesProcess.SelectedTeam = strValue;
			//        propertiesProcess.SetSectionDownText();
			//    }
                
			//}

			RemoveList();
			string strOrgValue = GetOriginalTeam();
			string strValue = GetSelectedTeam();
			if (strOrgValue != strValue)
			{
				UndoRedoManager.Instance.SaveSnapshot();
				ApplySelectedTeam();
				propertiesProcess.SelectedTeam = strValue;
				propertiesProcess.SetSectionDownText();
			}

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (treeViewTeam.SelectedNode == null)
                return;

            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            cell.Value = treeViewTeam.SelectedNode.Text;
            cell.Tag = treeViewTeam.SelectedNode.Tag;
            gridRow.Cells.Add(cell);

            if (!FindTeamName(cell))
            {
                gridRow.Tag = m_nCurrentTeamType;
                dataGridView.Rows.Add(gridRow);
                propertiesProcess.ArrSelected.Add(gridRow);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if(row.Cells[0].Selected)
                {
                    m_arrRemove.Add(row);
                    dataGridView.Rows.Remove(row);
                    //RemoveList();
                }
            }
        }

        private void btnChangeTeam_Click(object sender, EventArgs e)
        {
            PopupSelectTeam2 frm = new PopupSelectTeam2(m_nCurrentTeamType);

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_nCurrentTeamType = frm.SelectedTeamType;

                SetTeamTypeLabel(m_nCurrentTeamType);
                InitTree();
            }
        }

        private void RemoveList()
        {
            foreach (DataGridViewRow row in m_arrRemove)
            {
                foreach (DataGridViewRow row2 in propertiesProcess.ArrSelected)
                {
                    if ((int)row.Cells[0].Tag == (int)row2.Cells[0].Tag)
                    {
                        propertiesProcess.ArrSelected.Remove(row2);
                        break;
                    }
                }
            }
        }

        private bool FindTeamName(DataGridViewCell cell)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if ((row.Cells[0].Value == cell.Value) && ((int)row.Cells[0].Tag == (int)cell.Tag))
                    return true;
            }
            return false;
        }

        private void PopupSelectTeam_MouseDown(object sender, MouseEventArgs e)
        {
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupSelectTeam_MouseMove(object sender, MouseEventArgs e)
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

        private void PopupSelectTeam_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

    }

}
