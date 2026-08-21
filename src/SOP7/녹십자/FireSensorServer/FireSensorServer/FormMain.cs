using System;
using System.Windows.Forms;
using dnsTcpLib2;

namespace FireSensorServer
{
    using Network;

    public partial class FormMain : Form, IFormMain
    {
        private NetworkManager m_netManager = null;
        private Timer m_timer = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_netManager = new NetworkManager(this);

            m_timer = new Timer();
            m_timer.Tick += M_timer_Tick;
            m_timer.Interval = 1000;
            m_timer.Start();
        }

        private void M_timer_Tick(object sender, EventArgs e)
        {
            if (m_netManager.JohnsonManager != null)
            {
                if (m_netManager.JohnsonManager.IsConnected)
                    label1.Text = "연결됨";
                else
                    label1.Text = "연결 안됨";
            }
            else
            {
                label1.Text = "null";
            }
        }

        public void AddClient(ConnectionState state, string strClientType)
        {
            int nRowIndex = gridClients.Rows.Add();

            if (nRowIndex < 0)
                return;

            DataGridViewRow row = gridClients.Rows[nRowIndex];

            row.Cells[0].Value = row.Index + 1;
            row.Cells[1].Value = state.IPAddress + ":" + state.PortNo.ToString();
            row.Cells[2].Value = strClientType;

            row.Tag = state;
        }

        public Control GetControl()
        {
            return this;
        }

        public void RemoveClient(ConnectionState state)
        {
            int nRowCount = gridClients.RowCount;

            for (int i=0;i<nRowCount;i++)
            {
                DataGridViewRow row = gridClients.Rows[i];

                if (row.Tag == state)
                {
                    gridClients.Rows.Remove(row);

                    for (int j=i;j<nRowCount-1;j++)
                    {
                        row = gridClients.Rows[j];
                        row.Cells[0].Value = j + 1;
                    }

                    break;
                }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netManager.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Data.SensorManager.Instance.GetTagInfo(2, 1010200041);
            //Data.SensorManager.Instance.GetTagInfo(2, 1010011);
        }
    }
}
