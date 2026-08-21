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
        PropertiesProcess m_propertiesProcess = null;
        private Sections.SectionProcess m_section;

        private ArrayList m_arrMission = new ArrayList();

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        private int m_itemID = 0;
        public int ItemID
        {
            get { return m_itemID; }
            set { m_itemID = value; }
        }

        public PopupMission()
        {
            InitializeComponent();

            m_propertiesProcess = FormMain.Instance.GetPageLevel().GetPropertiesProcess();
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

        public void InitText(Sections.SectionProcess section)
        {
            m_section = section;
            if (m_propertiesProcess.Mission == null) return;

            textBox.Text = section.Title;

            Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;
            dataGridView.Rows.Clear();

            foreach (Sections.MissionItem data in sectionData.MissionItems)
            {
                AddGridRow(data);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (textBox.Text == "")
            {
                MessageBox.Show("제목을 입력하여 주십시오.");
                return;
            }

            m_propertiesProcess.Mission.Title = textBox.Text;
            m_propertiesProcess.SetSectionUpText();

            Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;

            sectionData.Title = m_section.Title = textBox.Text;
            sectionData.MissionItems.Clear();

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
                if( szValue.Equals(""))
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

                sectionData.MissionItems.Add(info);
            }

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


            if (e.ColumnIndex == 3)
            {
                PopupCheckItem popupCheckItem = new PopupCheckItem();

                Sections.MissionItem info = new Sections.MissionItem();
                //info.Transmission = 0;
                info.Mission = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();

                popupCheckItem.GetCheckItem(m_section, info.Mission);
                popupCheckItem.ShowDialog();
            }
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
            /*if (dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() == "내부상황전파")
                data.Transmission = 0;
            else
                data.Transmission = 1;*/
                            
			//int nType = 2;
			//if (szValue == "구두")
			//    nType = 0;
			//else if (szValue == "전화")
			//    nType = 1;
			//else if (szValue == "무전기")
			//    nType = 2;
			//else if (szValue == "기타")
			//    nType = 3;

            //Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;
            //Sections.MissionItem data = new Sections.MissionItem();
            //data.TransmissionType = nType;
            //data.Mission = szMission;
            //data.Target = szTarget;
            //sectionData.MissionItems.Add(data);            
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewCell cell in dataGridView.SelectedCells)
                {
                    if (cell.Value == null) return;

                    dataGridView.Rows.Remove(dataGridView.Rows[cell.RowIndex]);
                    break;
                }
            }
        }

        private ArrayList SetMissionInfo(Mission mission)
        {
            Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;
            mission.ArrMission = new ArrayList();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[1].Value == null || row.Cells[2].Value == null)
                    continue;                
                string szValue1 = row.Cells[0].Value.ToString();
                string szValue2 = row.Cells[1].Value.ToString();
                string szValue3 = row.Cells[2].Value.ToString();
                if (szValue1 == "" || szValue2 == "")
                    continue;

                int nType = 2;
                if (szValue1 == "구두")
                    nType = 0;
                else if (szValue1 == "전화")
                    nType = 1;
                else if (szValue1 == "무전기")
                    nType = 2;
                else if (szValue1 == "기타")
                    nType = 3;

                Sections.MissionItem data = new Sections.MissionItem();
                data.TransmissionType = nType;
                data.Mission = szValue2;
                data.Target = szValue3;               
                mission.ArrMission.Add(data);
            }

            return mission.ArrMission;
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
            /*dataGridView.Rows.RemoveAt(nSelectedRowIndex);
            dataGridView.Rows.Insert(nSelectedRowIndex - 1, rowSelected);*/

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
            /*dataGridView.Rows.RemoveAt(nSelectedRowIndex);
            dataGridView.Rows.Insert(nSelectedRowIndex + 1, rowSelected);*/

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
