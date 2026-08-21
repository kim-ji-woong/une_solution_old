using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TeamManagementSystem
{
    public partial class FormRightPerssonnel : Form
    {
        private FormMain m_Main = null;

        public FormRightPerssonnel(FormMain main)
        {
            InitializeComponent();
            m_Main = main;
            InitGrid();
        }

        private void InitGrid()
        {
            string[] strValue = new string[] { "이름", "부서", "직급", "전화번호", "휴대전화" };

            for (int i = 0; i < strValue.Length; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = strValue[i];
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                gridRow.Cells.Add(cell);

                dataGridPerssonnel.Rows.Add(gridRow);
            }
        }

        public void AddRowData(Data_OrganizationHistory data)
        {
            if (data == null)
            {
                dataGridPerssonnel.Rows[0].Cells[1].Value = "";
                dataGridPerssonnel.Rows[1].Cells[1].Value = "";
                dataGridPerssonnel.Rows[2].Cells[1].Value = "";
            }
            else
            {
                dataGridPerssonnel.Rows[0].Cells[1].Value = data.MemberName;
                dataGridPerssonnel.Rows[1].Cells[1].Value = data.TeamName;
                dataGridPerssonnel.Rows[2].Cells[1].Value = data.PositionName;
            }
            dataGridPerssonnel.Rows[3].Cells[1].Value = "";
            dataGridPerssonnel.Rows[4].Cells[1].Value = "";
        }

        public void RemoveData()
        {
            dataGridPerssonnel.Rows.Clear();
            InitGrid();
        }
    }
}
