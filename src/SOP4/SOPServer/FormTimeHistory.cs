using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDMSServer;

namespace SOPServer
{
    public partial class FormTimeHistory : Form
    {
        public FormTimeHistory()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NetworkServer server = FormMain.Instance.Server;
            ServiceProvider sp = server.ServiceProvider;
            List<TimeHistory> arList = new List<TimeHistory>(sp.TimeHistoryList);

            gridTimeHistory.ClearSelection();
            gridTimeHistory.Rows.Clear();


            for(int i = 0 ; i < arList.Count; i++)
            {
                TimeHistory history = arList[i];

                int nTimeHistoryID = history.HistoryID;
                int nHistoryID = history.LastReactionLog.SensorHistoryID;
                int nReactionLogID = history.LastReactionLog.ID;
                string szSensorZoneID = history.LastReactionLog.Param2;
                string szMessage = history.LastReactionLog.Message;

                DataGridViewRow row = new DataGridViewRow();



                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = i;
                row.Cells.Add(cell1);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = history.LastReactionLog.LogTime;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = nHistoryID;
                row.Cells.Add(cell3);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = nTimeHistoryID;
                row.Cells.Add(cell4);

                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                cell5.Value = szSensorZoneID;
                row.Cells.Add(cell5);

                DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                cell6.Value = szMessage;
                row.Cells.Add(cell6);


                gridTimeHistory.Rows.Add(row);

            }
            //sp.AddTimeHistory
        }
    }
}
