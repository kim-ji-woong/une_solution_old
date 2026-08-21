using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class PopupMission : Form
    {
        //PropertiesProcess m_propertiesProcess = null;
        private Sections.SectionProcess m_section;

        private ArrayList m_arrMission = new ArrayList();

        private int m_itemID = 0;
        public int ItemID
        {
            get { return m_itemID; }
            set { m_itemID = value; }
        }

        public PopupMission()
        {
            InitializeComponent();

            //m_propertiesProcess = FormMain.Instance.GetPageLevel().GetPropertiesProcess();
        }

        public void InitText(Sections.SectionProcess section)
        {
            m_section = section;

            textBox.Text = section.Title;

            Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;
            dataGridView.Rows.Clear();

            foreach (Sections.MissionItem data in sectionData.MissionItems)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewComboBoxCell cboCell = new DataGridViewComboBoxCell();
                cboCell.Items.Add("내부상황전파");
                cboCell.Items.Add("외부상황전파");

                //cboCell.Value = cboCell.Items[data.Transmission];
                gridRow.Cells.Add(cboCell);

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = data.Mission;
                gridRow.Cells.Add(cell);

                dataGridView.Rows.Add(gridRow);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //if (textBox.Text == "")
            //{
            //    MessageBox.Show("제목을 입력하여 주십시오.");
            //    return;
            //}

            //m_propertiesProcess.Mission.Title = textBox.Text;
            //m_propertiesProcess.SetSectionUpText();

            //Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;

            //sectionData.Title = m_section.Title = textBox.Text;
            //sectionData.MissionItems.Clear();

            //foreach (DataGridViewRow row in dataGridView.Rows)
            //{
            //    Sections.MissionItem info = new Sections.MissionItem();
            //    //DataGridViewComboBoxCell cboCell = (DataGridViewComboBoxCell)row.Cells[0];

            //    //if (cboCell.Value.ToString() == "내부상황전파")
            //    //    info.Transmission = 0;
            //    //else
            //    //    info.Transmission = 1;
            //    if (row.Cells[1].Value == null) break;

            //    DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[1];
            //    info.Mission = cell.Value.ToString();

            //    ArrayList arrCheck = new ArrayList();
            //    info.ArrCheckItem = sectionData.CheckedItems;

            //    sectionData.MissionItems.Add(info);
            //}

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
            // if (dataGridView.Rows[e.RowIndex].Cells[0].Value == null || dataGridView.Rows[e.RowIndex].Cells[1].Value == null)
            //    return;

            //Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;

            //if (e.ColumnIndex == 2)
            //{
            //    PopupCheckItem popupCheckItem = new PopupCheckItem();

            //    Sections.MissionItem info = new Sections.MissionItem();
            //    info.Transmission = 0;
            //    info.Mission = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();

            //    popupCheckItem.GetCheckItem(m_section, info.Mission);
            //    popupCheckItem.ShowDialog();
            //}
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //if (dataGridView.Rows[e.RowIndex].Cells[0].Value == null || dataGridView.Rows[e.RowIndex].Cells[1].Value == null)
            //    return;

            //Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;

            //Sections.MissionItem data = new Sections.MissionItem();
            //if (dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() == "내부상황전파")
            //    data.Transmission = 0;
            //else
            //    data.Transmission = 1;

            //data.Mission = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();

            //sectionData.MissionItems.Add(data);
            
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Delete)
            //{
            //    foreach (DataGridViewCell cell in dataGridView.SelectedCells)
            //    {
            //        if (cell.Value == null) return;

            //        dataGridView.Rows.Remove(dataGridView.Rows[cell.RowIndex]);
            //        break;
            //    }
            //}
        }

        private ArrayList SetMissionInfo(Mission mission)
        {
            //Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)m_section.Data;
            //mission.ArrMission = new ArrayList();

            //foreach (DataGridViewRow row in dataGridView.Rows)
            //{
            //    if (row.Cells[0].Value == null || row.Cells[1].Value == null) continue;

            //    Sections.MissionItem data = new Sections.MissionItem();
            //    if (row.Cells[0].Value.ToString() == "내부상황전파")
            //        data.Transmission = 0;
            //    else
            //        data.Transmission = 1;

            //    data.Mission = row.Cells[1].Value.ToString();

            //    mission.ArrMission.Add(data);
            //}

            return mission.ArrMission;
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
