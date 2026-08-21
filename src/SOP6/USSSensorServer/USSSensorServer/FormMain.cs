using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USSFireSensorServer
{
    using Network;

    public partial class FormMain : Form, IServiceOwner, IUIOwner
    {
        private NetworkManager m_netMgr = null;
        
        public FormMain()
        {
            InitializeComponent();

            labelStatus.Text = "";
            labelStatusSOP.Text = "";
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            labelStatus.Text = string.Format("USS Server와 접속을 시도하고 있습니다.");

            m_netMgr = new NetworkManager(this, this);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netMgr.Close();
        }

        public void OnConnect(string strIP, bool ussServer)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (ussServer)
                {
                    labelStatus.ForeColor = Color.Green;

                    if (strIP.Length > 0)
                        labelStatus.Text = string.Format("USS Server({0})와의 접속이 성공하였습니다.", strIP);
                    else
                        labelStatus.Text = string.Format("USS Server와의 접속이 성공하였습니다.");
                }
                else
                {
                    labelStatusSOP.ForeColor = Color.Green;
                    labelStatusSOP.Text = string.Format("SOP Server와의 접속이 성공하였습니다.");
                }
            });
        }

        public void OnDropConnection(string strIP, bool ussServer)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (ussServer)
                {
                    labelStatus.ForeColor = Color.Red;

                    if (strIP.Length > 0)
                        labelStatus.Text = string.Format("USS Server({0})와의 접속이 끊어졌습니다.", strIP);
                    else
                        labelStatus.Text = string.Format("USS Server와의 접속이 끊어졌습니다.");
                }
                else
                {
                    labelStatusSOP.ForeColor = Color.Red;
                    labelStatusSOP.Text = string.Format("SOP Server와의 접속이 끊어졌습니다.");
                }
            });
        }

        public void OnAddClient(TcpLib2.ConnectionState state)
        {
            this.Invoke((MethodInvoker)delegate
            {
                int nRowIndex = gridClients.Rows.Add();

                if (nRowIndex >= 0)
                {
                    DataGridViewRow row = gridClients.Rows[nRowIndex];
                    row.Cells[0].Value = nRowIndex + 1;
                    row.Cells[1].Value = state.IPAddress + ":" + state.PortNo.ToString();
                    row.Tag = state;
                }
            });
        }

        public void OnRemoveClient(TcpLib2.ConnectionState state)
        {
            int nRowCount = gridClients.Rows.Count;

            for (int i=0;i<nRowCount;i++)
            {
                DataGridViewRow row = gridClients.Rows[i];

                if (row.Tag == (object)state)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        gridClients.Rows.RemoveAt(i);

                        for (int j = i + 1; j < nRowCount - 1; j++)
                        {
                            row = gridClients.Rows[j];
                            row.Cells[0].Value = j - 2;
                        }
                    });

                    break;
                }
            }
        }

        public void SetClientInfo(TcpLib2.ConnectionState state, List<byte> eventTypes)
        {
            int nRowCount = gridClients.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = gridClients.Rows[i];

                if (row.Tag == (object)state)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        row.Cells[2].Value = GetEventTypeString(eventTypes);
                    });

                    break;
                }
            }
        }

        private string GetEventTypeString(List<byte> eventTypes)
        {
            string strTypes = "", strType = "";

            foreach (byte eType in eventTypes)
            {
                if (eType == libUSS.EventType.Fire)
                    strType = "화재";
                else if (eType == libUSS.EventType.PowerOff)
                    strType = "정전";
                else if (eType == libUSS.EventType.Earthquake)
                    strType = "지진";
                else if (eType == libUSS.EventType.Wind)
                    strType = "강풍";
                else
                    return "";

                if (strTypes.Length == 0)
                    strTypes = strType;
                else
                    strTypes += ";" + strType;
            }

            return strTypes;
        }

        private void btnSendEarthquake_Click(object sender, EventArgs e)
        {
            string strIntensity = textBoxIntensity.Text.Trim();

            if (strIntensity.Length == 0)
            {
                textBoxIntensity.Focus();
                MessageBox.Show("진도값을 입력하세요.");
                return;
            }

            int nIntensity;

            if (int.TryParse(strIntensity, out nIntensity) == false || nIntensity < 0)
            {
                textBoxIntensity.Focus();
                MessageBox.Show("진도값은 0보다 크거나 같은 정수 형태의 값이어야 합니다.");
                return;
            }

            m_netMgr.SendSimulationEarthquake(nIntensity);
        }

        private void btnSendWind_Click(object sender, EventArgs e)
        {
            string strWindSpeed = textBoxWindSpeed.Text.Trim();

            if (strWindSpeed.Length == 0)
            {
                textBoxWindSpeed.Focus();
                MessageBox.Show("풍속을 입력하세요.");
                return;
            }

            float fWindSpeed;

            if (float.TryParse(strWindSpeed, out fWindSpeed) == false || fWindSpeed < 0)
            {
                textBoxWindSpeed.Focus();
                MessageBox.Show("풍속은 0보다 크거나 같은 실수 형태의 값이어야 합니다.");
                return;
            }

            string strOfficeA = System.Configuration.ConfigurationManager.AppSettings["officeA"].ToString().Trim();
            string strOfficeB = System.Configuration.ConfigurationManager.AppSettings["officeB"].ToString().Trim();

            int nSensorID;

            if (radioOfficeA.Checked)
            {
                if (int.TryParse(strOfficeA.Trim(), out nSensorID) == false)
                    return;
            }
            else
            {
                if (int.TryParse(strOfficeB.Trim(), out nSensorID) == false)
                    return;
            }

            m_netMgr.SendSimulationWindSpeed(nSensorID, fWindSpeed);
        }

        private void checkBoxStopReadEvent_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxStopReadEvent.Checked)
                m_netMgr.StopReadEvent();
            else
                m_netMgr.StartReadEvent();
        }
    }
}
