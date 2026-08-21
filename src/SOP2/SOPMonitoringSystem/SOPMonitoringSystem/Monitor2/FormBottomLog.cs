using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPDisasterSystem
{
    public partial class FormBottomLog : Form
    {
        public FormBottomLog()
        {
            InitializeComponent();

            tabCtrlBottom.Controls.Remove(tabGraph);

            InitGrid();
            SetColumnColor();
        }

        private void InitGrid()
        {
            // Header 가운데 정렬
            dataGridViewLog.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            string[] strValue = new string[] {"[예시]재난 신고 발생.", "[예시] 위치 지정 완료.", "[예시] 소방설비 가시화." };

            for (int i = 0; i < strValue.Length; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = "00(시):00(분):00(초)";
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue[i];
                gridRow.Cells.Add(cell);

                dataGridViewLog.Rows.Add(gridRow);
            }
        }

        private void SetColumnColor()
        {
            dataGridViewLog.DefaultCellStyle.BackColor = Color.GhostWhite;
        }

    }
}
