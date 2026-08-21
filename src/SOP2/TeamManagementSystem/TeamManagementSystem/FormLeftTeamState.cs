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
    public partial class FormLeftTeamState : Form
    {
        private FormMain m_Main = null;

        public FormLeftTeamState(FormMain main)
        {
            InitializeComponent();
            m_Main = main;

            InitGrid();
            InitData();
        }

        private void InitGrid()
        {
            string[] strValue = new string[] { "버전", "최종 수정일", "최종 수정자", "부서수", "인원" };

            for (int i = 0; i < strValue.Length; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = strValue[i];
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                gridRow.Cells.Add(cell);

                dataGridVersion.Rows.Add(gridRow);
            }
        }

        public void InitData()
        {
            foreach (Data_TeamVersion data in m_Main.TeamVersion)
            {
                if(m_Main.VersionName == data.VersionName)
                {
                    dataGridVersion.Rows[0].Cells[1].Value = data.VersionName;
                    dataGridVersion.Rows[1].Cells[1].Value = data.CreateTime;
                    dataGridVersion.Rows[2].Cells[1].Value = data.UserName;
                    dataGridVersion.Rows[3].Cells[1].Value = "0"; // 부서수
                    dataGridVersion.Rows[4].Cells[1].Value = "0"; // 인원
                }
            }
        }

        public void RemoveData()
        {
            dataGridVersion.Rows.Clear();
            InitGrid();
        }
    }
}
