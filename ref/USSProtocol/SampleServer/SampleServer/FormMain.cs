using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TcpLib2;

namespace SampleServer
{
    public partial class FormMain : Form, IServiceOwner
    {
        private USSServer m_ussServer = null;
        private bool m_fireOn = false;
        private bool m_powerOff = false;
        private int m_nIntensity = 0;

        public FormMain()
        {
            InitializeComponent();

            int nPort;
            string strPort = System.Configuration.ConfigurationManager.AppSettings.Get("Port");

            if (strPort.Length > 0 && int.TryParse(strPort, out nPort))
            {
                m_ussServer = new USSServer(nPort, this);
                m_ussServer.BeginServer();
            }
        }

        public void OnAccept(TcpLib2.ConnectionState state)
        {
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    int nRowIndex = gridClients.Rows.Add();

                    if (nRowIndex < 0)
                        return;

                    string strIP = state.RemoteEndPoint.ToString();

                    DataGridViewRow row = gridClients.Rows[nRowIndex];

                    row.Cells[0].Value = nRowIndex + 1;
                    row.Cells[1].Value = strIP;
                    row.Tag = state;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("FormMain.OnAccept Error : " + e.Message);
                }
            });
        }

        public void OnDropConnection(TcpLib2.ConnectionState state)
        {
            this.Invoke((MethodInvoker)delegate
            {
                int nRowCount = gridClients.Rows.Count;
                int nIndex = -1;

                try
                {
                    for (int i = 0; i < nRowCount; i++)
                    {
                        DataGridViewRow row = gridClients.Rows[i];

                        if (row.Tag == state)
                        {
                            gridClients.Rows.RemoveAt(i);
                            nIndex = i;
                            nRowCount--;
                        }
                    }

                    for (int i = nIndex; i < nRowCount; i++)
                    {
                        DataGridViewRow row = gridClients.Rows[i];
                        row.Cells[0].Value = i + 1;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("FormMain.OnDropConnection Error : " + e.Message);
                }
            });
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_ussServer.StopServer();
        }

        int m_nWindSensorID = 3;

        private void btnSendSignal_Click(object sender, EventArgs e)
        {
            if (m_ussServer == null)
                return;

            if (sender == btnSendFire)
            {
                m_fireOn = !m_fireOn;
                m_ussServer.SendFireSignal(m_fireOn, 10, DateTime.Now);
            }
            else if (sender == btnSendPowerOff)
            {
                m_powerOff = !m_powerOff;
                m_ussServer.SendPowerOffSignal(m_powerOff, 1, -1, DateTime.Now);
            }
            else if (sender == btnSendEarthquake)
            {
                m_nIntensity = m_nIntensity > 0 ? 0 : 5;
                m_ussServer.SendEarthquakeSignal(m_nIntensity, DateTime.Now);
            }
            else if (sender == btnSendWind)
            {
                m_nWindSensorID = m_nWindSensorID == 3 ? 4 : 3;
                m_ussServer.SendWindSignal(m_nWindSensorID, 10.0f, DateTime.Now);
            }
        }
    }
}
