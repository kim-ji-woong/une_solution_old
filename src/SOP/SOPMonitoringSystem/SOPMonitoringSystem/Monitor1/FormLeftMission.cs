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
    public partial class FormLeftMission : Form
    {
        private FormMain m_Main = null;

        public FormLeftMission(FormMain main)
        {
            InitializeComponent();

            m_Main = main;
            
            dataGridProMission.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridProCircum.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridMission.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridCheck.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public void ResizeForm()
        {
            this.Size = new System.Drawing.Size(284, 262);
            this.Refresh();
        }

        public void VisiblePanel(int nSection)
        {
            if (nSection == 1)
            {
                //panelProcess.Visible = true;
                //panelGroup.Visible = false;
                panelProcess.Show();
                panelGroup.Hide();
            }
            else if (nSection == 2)
            {
                //panelProcess.Visible = false;
                //panelGroup.Visible = true;
                panelGroup.Show();                
                panelProcess.Hide();

                panelGroup.Location = new Point(10, 9);
                this.Refresh();
            }
        }

        private void AddCheckItem(MemberofSection.MissionofSection mission)
        {
            ArrayList arrCheckItems = mission.CheckItems;

            foreach (MemberofSection.CheckofMission item in arrCheckItems)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = item.Category;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();

                cell.Value = item.SubCategory;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();

                cell.Value = item.TaskName;
                row.Cells.Add(cell);
                dataGridCheck.Rows.Add(row);
            }
        }

        // 임무관리
        public void SetMissionData()
        {
            dataGridProMission.Rows.Clear();
            dataGridMission.Rows.Clear();
            dataGridCheck.Rows.Clear();

            MemberofSection memSection = m_Main.GetProcess().GetMissionData();
            if (memSection == null) return;

            foreach (MemberofSection.MissionofSection mission in memSection.Missions)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = mission.Division;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = mission.TaskName;
                gridRow.Cells.Add(cell);

                dataGridMission.Rows.Add(gridRow);

                AddCheckItem(mission);
            }

        }

        private string GetTeamNames(ArrayList arrSections)
        {
            string strTeamName = "";

            int nSectionCount = arrSections.Count;
            if (nSectionCount == 0) return strTeamName;

            SectionEx section = (SectionEx)arrSections[0];
            strTeamName = section.GetTextBox().Text;

            for (int i = 1; i < nSectionCount; i++)
            {
                section = (SectionEx)arrSections[i];
                strTeamName += ", " + section.GetTextBox().Text;
            }

            return strTeamName;
        }

        public void SetCurrentProcess(SectionEx sectionProcess)
        {
            dataGridProMission.Rows.Clear();

            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();

            cell.Value = sectionProcess.GetTextBox().Text;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = GetTeamNames(sectionProcess.GetChildSections());
            gridRow.Cells.Add(cell);

            dataGridProMission.Rows.Add(gridRow);            
        }

        // 상황전파
        public void SetCircumstancesData()
        {
            ;
        }


    }
}
