using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;

namespace PSMSensorSimulator
{
    public partial class Form1 : Form
    {
        WebDBManager m_dbMgr = new WebDBManager(1);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitGrid();
        }

        private void InitGrid()
        {
            string strSQL = "Select ID, SensorName from PSMSensor";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            int nResultCount = arrResult.Count;
            int nIndex = 1;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);

                DataGridViewRow row = new DataGridViewRow();
                row.Tag = nID;

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = nIndex++;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = 0;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = 0;
                row.Cells.Add(cell);

                DataGridViewButtonCell cell2 = new DataGridViewButtonCell();
                cell2.Value = "전송";
                row.Cells.Add(cell2);

                dataGridView1.Rows.Add(row);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                int nID = (int)dataGridView1.Rows[e.RowIndex].Tag;
                double dSensorData;
                int nAlarm;

                if (double.TryParse(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString(), out dSensorData) &&
                    int.TryParse(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString(), out nAlarm))
                {
                    string strSQL = string.Format("Update PSMSensor set CurrentLevel = {0}, CurrentData = {1} where ID = {2}",
                        dSensorData, nAlarm, nID);

                    m_dbMgr.GetResultData(strSQL, 0);
                }
            }
        }
    }
}
