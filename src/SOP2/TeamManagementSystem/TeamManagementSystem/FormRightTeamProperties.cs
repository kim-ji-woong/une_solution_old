using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace TeamManagementSystem
{
    public partial class FormRightTeamProperties : Form
    {
        private FormMain m_Main = null;

        public FormRightTeamProperties(FormMain main)
        {
            InitializeComponent();
            m_Main = main;
            InitGrid();
            dataGridTeamMember.ReadOnly = true;
        }

        private void InitGrid()
        {
            string[] strValue = new string[] { "팀명", "-", "그룹명", "-", "책임자", "-", "부책임자", "-"};

            for (int i = 0; i < strValue.Length; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = strValue[i];
                gridRow.Cells.Add(cell);

                dataGridTeamProperties.Rows.Add(gridRow);
            }
        }

        // 팀 속성 출력
        public void SetGridData(string strTeamName, DataGridView dataGrid, int nRegularTeamID)
        {
            if (m_Main.TeamMode == 0) // 상시조직도
                groupBox1.Text = "소속팀원";
            else
                groupBox1.Text = "소속팀";

            string strLeader = m_Main.FindTeamLeader(nRegularTeamID);

            for (int i = 0; i < dataGridTeamProperties.RowCount; i++)
            {
                if (i == 1) // 팀명
                    dataGridTeamProperties.Rows[i].Cells[0].Value = strTeamName;

                if (m_Main.TeamMode == 0) // 상시조직도
                {
                    if (i == 3) // 그룹명
                        dataGridTeamProperties.Rows[i].Cells[0].Value = "-";
                    else if (i == 5) // 책임자
                        dataGridTeamProperties.Rows[i].Cells[0].Value = strLeader;
                    else if (i == 7) // 부책임자
                        dataGridTeamProperties.Rows[i].Cells[0].Value = "-";
                }
                else if (m_Main.TeamMode == 1) //평일비상조직도
                {
                    ArrayList arrNormal = m_Main.FindTeam(m_Main.TeamMode, strTeamName);
                    if (arrNormal == null) return;
                    Data_NormalHistory data = (Data_NormalHistory)arrNormal[arrNormal.Count-1];

                    if (i == 3)
                    {
                        if (data.GroupName != "null")
                            dataGridTeamProperties.Rows[i].Cells[0].Value = data.GroupName;
                        else
                            dataGridTeamProperties.Rows[i].Cells[0].Value = "-";
                    }
                    else if (i == 5)
                        dataGridTeamProperties.Rows[i].Cells[0].Value = "-";
                    else if (i == 7)
                        dataGridTeamProperties.Rows[i].Cells[0].Value = "-";
                }
                else if (m_Main.TeamMode == 2) //휴일비상조직도
                {
                    ArrayList arrEmergency = m_Main.FindTeam(m_Main.TeamMode, strTeamName);
                    if (arrEmergency == null) return;
                    Data_EmergencyHistory data = (Data_EmergencyHistory)arrEmergency[arrEmergency.Count - 1];
                    
                    if (i == 3)
                    {
                        if (data.GroupName != "null")
                            dataGridTeamProperties.Rows[i].Cells[0].Value = data.GroupName;
                        else
                            dataGridTeamProperties.Rows[i].Cells[0].Value = "-";
                    }
                    else if (i == 5)
                        dataGridTeamProperties.Rows[i].Cells[0].Value = "-";
                    else if (i == 7)
                        dataGridTeamProperties.Rows[i].Cells[0].Value = "-";
                }
            }

            dataGridTeamMember.Rows.Clear();

            int nRowCount = 0;
            if (m_Main.EditMode)
            {
                nRowCount = dataGrid.RowCount - 1;
            }
            else
            {
                nRowCount = dataGrid.RowCount;
            }

            for (int i = 0; i < nRowCount; i++)
            {
                if (dataGrid.RowCount == 0) return;

                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = dataGrid.Rows[i].Cells[0].Value;
                cell.Tag = dataGrid.Rows[i].Cells[0].Tag;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                gridRow.Cells.Add(cell);

                dataGridTeamMember.Rows.Add(gridRow);
            }

            m_Main.GetPerssonnel().RemoveData();
            foreach (DataGridViewCell cell in dataGridTeamMember.SelectedCells)
            {
                if (cell.Selected)
                {
                    FindMember(cell.RowIndex);
                }
            }
            
        }

        public void RemoveData()
        {
            dataGridTeamProperties.Rows.Clear();
            dataGridTeamMember.Rows.Clear();
            InitGrid();
        }

        public void ReadOnlyData(bool isReadOnly)
        {
            dataGridTeamMember.ReadOnly = isReadOnly;
        }

        private void dataGridTeamMember_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridTeamMember_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FindMember(e.RowIndex);
        }

        private void FindMember(int nRowIndex)
        {
            if (dataGridTeamMember.Rows[nRowIndex].Cells[0].Value == null) return;

            int nID = (int)dataGridTeamMember.Rows[nRowIndex].Cells[0].Tag;

            //개인속성검색
            Data_OrganizationHistory data = m_Main.FindMember(nID);
            m_Main.GetPerssonnel().AddRowData(data);
        }
    }
}
