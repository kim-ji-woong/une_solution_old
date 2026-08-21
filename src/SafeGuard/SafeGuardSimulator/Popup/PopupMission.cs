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
    public partial class PopupMission : Form
    {
		private PropertiesProcess m_propertiesProcess = null;
		public PropertiesProcess PropertiesProcess
		{
			get { return m_propertiesProcess; }
			set 
			{
				m_propertiesProcess = value;
				if (m_propertiesProcess != null)
					InitText();
			}
		}

		private ArrayList mMissionList = new ArrayList();
		public ArrayList MissionList
		{
			get { return mMissionList; }
		}

		private Sections.SectionProcess m_section = null;
        private Sections.SectionCommander m_commander = null;

        string m_szTitleText = "";
		public string TitleText
		{
            get { return m_szTitleText; }
            set { m_szTitleText  = value; }
		}

        private ArrayList m_TeamList = new ArrayList();
        public ArrayList TeamList
        {
            get { return m_TeamList; }
        }

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public PopupMission()
        {
            InitializeComponent();            
        }

        private DataGridViewRow AddGridRow(Sections.MissionItem data, int nRowIndex = -1)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewComboBoxCell cboCell = new DataGridViewComboBoxCell();
            cboCell.Items.Add("구두");
            cboCell.Items.Add("전화");
            cboCell.Items.Add("무전기");
            cboCell.Items.Add("기타");

            cboCell.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            cboCell.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            gridRow.Cells.Add(cboCell);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            gridRow.Cells.Add(cell2);
            gridRow.Tag = data;

            if (nRowIndex < 0)
                dataGridView.Rows.Add(gridRow);
            else
                dataGridView.Rows.Insert(nRowIndex, gridRow);

            return gridRow;
        }

        private void InitText()
        {
			mMissionList.Clear();		

			m_section = (Sections.SectionProcess)m_propertiesProcess.GetSection();

			m_szTitleText = m_section.Title;
            textBox.Text = m_szTitleText;

            m_TeamList = (ArrayList)m_propertiesProcess.SelectedTeamList.Clone();

            InitSelectTeam();

            Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;
            dataGridView.Rows.Clear();

            m_commander = new Sections.SectionCommander();
            m_commander.DisplayText = sectionData.Commander.DisplayText;
            m_commander.IsTeamMember = sectionData.Commander.IsTeamMember;
            m_commander.Team = sectionData.Commander.Team;
            m_commander.TeamMemberID = sectionData.Commander.TeamMemberID;

            if (m_commander != null && m_commander.DisplayText != null)
                textBoxCommander.Text = m_commander.DisplayText;

            foreach (Sections.MissionItem data in sectionData.MissionItems)
            {
                AddGridRow(data);
            }

            if (sectionData.MissionItems.Count < 1)
            {
                Sections.MissionItem item = new Sections.MissionItem();
                item.Mission = "";
                item.TransmissionType = 2;
                AddGridRow(item);
            }
        }

        public void SaveData()
        {
            bool bChangedData = false;
            Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;

            if (m_propertiesProcess == null)
                return;

            m_propertiesProcess.EnabledSnapshot = false;
            if (m_szTitleText != textBox.Text)
            {
                SaveSnapShot("ProcessSection 변경");
                m_szTitleText = textBox.Text;
                m_propertiesProcess.Text = m_szTitleText;
                bChangedData = true;
            }
           
            dataGridView.EndEdit();
            mMissionList.Clear();
            
            foreach (DataGridViewRow row in dataGridView.Rows)
            {

                DataGridViewComboBoxCell cboCell = (DataGridViewComboBoxCell)row.Cells[0];
                DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[1];
                DataGridViewTextBoxCell cell2 = (DataGridViewTextBoxCell)row.Cells[2];

                if (cell.Value == null || cboCell.Value == null)
                    continue;
                string szTarget = "";
                if (cell2.Value != null)
                {
                    szTarget = cell2.Value.ToString();
                }

                string szValue = cell.Value.ToString();
                if (szValue.Equals(""))
                    continue;

                Sections.MissionItem info = new Sections.MissionItem();

                int nType = 2;
                if (cboCell.Value.ToString() == "구두")
                    nType = 0;
                else if (cboCell.Value.ToString() == "전화")
                    nType = 1;
                else if (cboCell.Value.ToString() == "무전기")
                    nType = 2;
                else if (cboCell.Value.ToString() == "기타")
                    nType = 3;
                info.TransmissionType = nType;

                info.Target = szTarget;
                info.Mission = szValue;
                ArrayList arrCheck = new ArrayList();
                info.ArrCheckItem = sectionData.CheckedItems;

                mMissionList.Add(info);
            }

            if (CompareCommander(sectionData.Commander, m_commander))
            {
                SaveSnapShot("ProcessSection 변경");

                Sections.SectionCommander Commander = new Sections.SectionCommander();
                Commander.DisplayText = m_commander.DisplayText;
                Commander.IsTeamMember = m_commander.IsTeamMember;
                Commander.Team = m_commander.Team;
                Commander.TeamMemberID = m_commander.TeamMemberID;

                m_propertiesProcess.SectionCommander = Commander;
                bChangedData = true;
            }

            ArrayList arrMissionList = sectionData.MissionItems;
            if (CompareMissionList(MissionList, arrMissionList))
			{
                SaveSnapShot("ProcessSection 변경");
                m_propertiesProcess.MissionList = mMissionList;
                bChangedData = true;
			}

            ArrayList arTeams = sectionData.TeamList;
            if (CompareTeamList(arTeams, m_TeamList))
            {
                SaveSnapShot("ProcessSection 변경");
                m_propertiesProcess.SelectedTeamList = m_TeamList;
                bChangedData = true;
            }

            if(bChangedData == true)
            {
                if (m_section.GetParent() != null)
                    m_section.GetParent().Refresh();
            }
            m_propertiesProcess.EnabledSnapshot = true;
        }

        private bool bSaveSnapShot = false;
        private void SaveSnapShot(string szName)
        {
            if (bSaveSnapShot == false)
            {
                UndoRedoManager.Instance.SaveSnapshot(szName);
                bSaveSnapShot = true;
            }            
        }

        private bool CompareCommander(Sections.SectionCommander commander1, Sections.SectionCommander commander2)
        {
            if (commander1 == null)
                return false;

            if (commander2 == null)
                return true;

            if (commander1.Team != null && commander2.Team == null)
            {
                return true;
            }
            if (commander2.Team != null && commander1.Team == null)
            {
                return true;
            }
            if ((commander2.Team == null && commander1.Team == null) && (commander2.DisplayText == commander1.DisplayText))
            {
                return false;
            }

            if (commander1.DisplayText != commander2.DisplayText)
                return true;
            if (commander1.TeamMemberID != commander2.TeamMemberID)
                return true;
            if (commander1.Team.TeamType != commander2.Team.TeamType)
            {
                return true;
            }
            if (commander1.Team.TeamID != commander2.Team.TeamID)
            {
                return true;
            }

            return false;
        }

        private bool CompareMissionList(ArrayList arMission, ArrayList arOrgMission)
        {
            if (arMission == null)
                return false;

            if (arOrgMission == null)
                return true;

            if (arMission.Count != arOrgMission.Count)
                return true;

            for (int i = 0; i < arMission.Count; i++)
            {
                Sections.MissionItem item = (Sections.MissionItem)arMission[i];
                Sections.MissionItem item2 = (Sections.MissionItem)arOrgMission[i];

                if (item.Target != item2.Target)
                    return true;
                if (item.Mission != item2.Mission)
                    return true;
                if (item.TransmissionType != item2.TransmissionType)
                    return true;
            }
            return false;
        }

        private bool CompareTeamList(ArrayList arMission, ArrayList arOrgMission)
        {
            if (arMission == null)
                return false;

            if (arOrgMission == null)
                return true;

            if (arMission.Count != arOrgMission.Count)
                return true;

            for (int i = 0; i < arMission.Count; i++)
            {
                Sections.SOPTeam item = (Sections.SOPTeam)arMission[i];
                Sections.SOPTeam item2 = (Sections.SOPTeam)arOrgMission[i];

                if (item.TeamID != item2.TeamID)
                    return true;
                if (item.TeamName != item2.TeamName)
                    return true;
                if (item.TeamType != item2.TeamType)
                    return true;
            }
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveData();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

             if (dataGridView.Rows[e.RowIndex].Cells[0].Value == null || dataGridView.Rows[e.RowIndex].Cells[1].Value == null)
                return;

            Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;


            //if (e.ColumnIndex == 3)
            //{
            //    PopupCheckItem popupCheckItem = new PopupCheckItem();

            //    Sections.MissionItem info = new Sections.MissionItem();
            //    //info.Transmission = 0;
            //    info.Mission = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();

            //    popupCheckItem.GetCheckItem(m_section, info.Mission);
            //    popupCheckItem.ShowDialog();
            //}
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.Rows[e.RowIndex].Cells[0].Value == null || dataGridView.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string szMission = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
            string szValue = dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();

            string szTarget = "";
            if (dataGridView.Rows[e.RowIndex].Cells[2].Value != null)
            {
                szTarget = dataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
            }

            if (szMission == "" || szValue == "")
                return;    
                   
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewCell cell in dataGridView.SelectedCells)
                {
                    if (cell.Value == null) return;

                    DataGridViewRow row = dataGridView.Rows[cell.RowIndex];

                    if (!row.IsNewRow)
                        dataGridView.Rows.Remove(row);
                    break;
                }
            }
        }

        private void dataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
        }

        private void dataGridView_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
        }

        private void dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {            
            DataGridView grid = (DataGridView)sender;
            if (grid != null)
            {

                for (int i = 0; i < e.RowCount; i++)
                {
                    DataGridViewRow row = grid.Rows[e.RowIndex + i];
                    if (row != null && row.Tag == null)
                    {
                        row.Cells[0].Value = "무전기";
                        row.Cells[1].Value = "";
                        row.Cells[2].Value = "";
                    }
                    if (row != null && row.Tag != null)
                    {
                        Sections.MissionItem data = (Sections.MissionItem)row.Tag;
                        switch (data.TransmissionType)
                        {
                            case 0:
                                row.Cells[0].Value = "구두";
                                break;
                            case 1:
                                row.Cells[0].Value = "전화";
                                break;
                            case 2:
                                row.Cells[0].Value = "무전기";
                                break;
                            case 3:
                                row.Cells[0].Value = "기타";
                                break;

                        }
                        row.Cells[1].Value = data.Mission;
                        row.Cells[2].Value = data.Target;
                    }
                }
               
            }            
        }

        private void PopupMission_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void PopupMission_MouseMove(object sender, MouseEventArgs e)
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

        private void PopupMission_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void label3_MouseDown(object sender, MouseEventArgs e)
        {
            PopupMission_MouseDown(sender, e);
        }

        private void label3_MouseMove(object sender, MouseEventArgs e)
        {
            PopupMission_MouseMove(sender, e);
        }

        private void label3_MouseUp(object sender, MouseEventArgs e)
        {
            PopupMission_MouseUp(sender, e);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            PopupMission_MouseDown(sender, e);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            PopupMission_MouseMove(sender, e);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            PopupMission_MouseUp(sender, e);
        }

        private void RemoveNInsert(DataGridViewRow row, int nRemoveRowIndex, int nInsertIndex)
        {
            try
            {
                DataGridViewComboBoxCell cell1 = (DataGridViewComboBoxCell)row.Cells[0];
                DataGridViewTextBoxCell cell2 = (DataGridViewTextBoxCell)row.Cells[1];
                DataGridViewTextBoxCell cell3 = (DataGridViewTextBoxCell)row.Cells[2];

                object strValue1 = cell1.Value;
                object strValue2 = cell2.Value;
                object strValue3 = cell3.Value;

                dataGridView.Rows.RemoveAt(nRemoveRowIndex);
                dataGridView.Rows.Insert(nInsertIndex, row);

                row.Cells[2].Value = strValue3;
                row.Cells[0].Value = strValue1;
                row.Cells[1].Value = strValue2;
            }catch(Exception)
            { }
            
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int nSelectedColumnIndex;
            int nSelectedRowIndex = GetSelectedRowIndex(out nSelectedColumnIndex);
            int nRowCount = dataGridView.Rows.Count;

            if (nSelectedRowIndex <= 0 || nRowCount <= 1)
                return;

            DataGridViewRow rowSelected = dataGridView.Rows[nSelectedRowIndex];
            RemoveNInsert(rowSelected, nSelectedRowIndex, nSelectedRowIndex - 1);
            
            dataGridView.ClearSelection();
            rowSelected.Cells[nSelectedColumnIndex].Selected = true;
            dataGridView.Refresh();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int nSelectedColumnIndex;
            int nSelectedRowIndex = GetSelectedRowIndex(out nSelectedColumnIndex);
            int nRowCount = dataGridView.Rows.Count;

            if (nSelectedRowIndex >= nRowCount - 2 || nSelectedRowIndex < 0)
                return;

            DataGridViewRow rowSelected = dataGridView.Rows[nSelectedRowIndex];
            RemoveNInsert(rowSelected, nSelectedRowIndex, nSelectedRowIndex + 1);           

            dataGridView.ClearSelection();
            rowSelected.Cells[nSelectedColumnIndex].Selected = true;
            dataGridView.Refresh();
        }

        private int GetSelectedRowIndex(out int nSelectedColumnIndex)
        {
            foreach (DataGridViewCell cell in dataGridView.SelectedCells)
            {
                nSelectedColumnIndex = cell.ColumnIndex;
                return cell.RowIndex;
            }

            nSelectedColumnIndex = -1;
            return -1;
        }

        private void PopupMission_Load(object sender, EventArgs e)
        {
            textBox.Text = m_szTitleText;                 
        }

        private void SaveDataAfter()
        {
            if (PropertiesProcess != null)
            {
                
               
            }
        }

        private void PopupSelectTeam()
        {
        }

        private void InitSelectTeam()
        {
            string strTeamText = "";

            foreach (Sections.SOPTeam item in m_TeamList)
            {
                if (String.IsNullOrWhiteSpace(strTeamText))
                    strTeamText = item.TeamName;
                else
                    strTeamText += String.Format(", {0}", item.TeamName);
            }

            txtSelectTeam.Text = strTeamText;
        }

        private void btnSelectTeam_Click(object sender, EventArgs e)
        {
            PopupSelectTeam();
        }

        private void btnSelectCommander_Click(object sender, EventArgs e)
        {
   
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
    }

    public class Mission
    {
        private string m_strTitle;
        private ArrayList m_arrMission = null;
        
        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        public ArrayList ArrMission
        {
            get { return m_arrMission; }
            set { m_arrMission = value; }
        }
    }

}
