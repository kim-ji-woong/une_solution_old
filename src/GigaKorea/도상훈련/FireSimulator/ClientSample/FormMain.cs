using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientSample
{
    public partial class FormMain : Form
    {
        private ClientProvider m_provider = new ClientProvider();
        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string strIP = textBoxIP.Text;
            int nPort = 1470;

            if (m_provider.Connect(strIP, nPort))
                SetState(true);
            else
                SetState(false);
        }

        public void SetState(bool connected)
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnConnect.Enabled = !connected;

                if (connected)
                {
                    labelConnectionState.Text = "접속중";
                    labelConnectionState.ForeColor = Color.Green;
                }
                else
                {
                    labelConnectionState.Text = "접속안됨";
                    labelConnectionState.ForeColor = Color.Red;
                    m_provider = new ClientProvider();
                }
            });
        }

        public void ReportFire(string strLevelID, string strSpaceID, DateTime time)
        {
            this.Invoke((MethodInvoker)delegate
            {
                foreach (DataGridViewRow _row in gridFire.Rows)
                {
                    if ((string)_row.Cells[0].Tag == strLevelID && (string)_row.Cells[1].Tag == strSpaceID)
                    {
                        // 이미 존재하는 알람이면 무시한다.
                        return;
                    }
                }

                string strTime = string.Format("{0:00}:{1:00}:{2:00}", time.Hour, time.Minute, time.Second);
                string strLocation = strLevelID + " " + strSpaceID;

                int nRowIndex = gridFire.Rows.Add();

                if (nRowIndex < 0)
                    return;

                DataGridViewRow row = gridFire.Rows[nRowIndex];

                row.Cells[0].Value = nRowIndex + 1;
                row.Cells[0].Tag = strLevelID;
                row.Cells[1].Value = strTime;
                row.Cells[1].Tag = strSpaceID;
                row.Cells[2].Value = strLocation;
                row.Cells[2].Tag = time;
            });
        }

        public void RemoveFire(string strLevelID, string strSpaceID, DateTime time)
        {
            this.Invoke((MethodInvoker)delegate
            {
                foreach (DataGridViewRow row in gridFire.Rows)
                {
                    if ((string)row.Cells[0].Tag == strLevelID && (string)row.Cells[1].Tag == strSpaceID)
                    {
                        gridFire.Rows.Remove(row);
                        Reorder();
                        return;
                    }
                }
            });
        }

        private void Reorder()
        {
            foreach (DataGridViewRow row in gridFire.Rows)
            {
                row.Cells[0].Value = row.Index + 1;
            }
        }
    }
}
