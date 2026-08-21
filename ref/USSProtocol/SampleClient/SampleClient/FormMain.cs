using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace SampleClient
{
    using Network;

    public partial class FormMain : Form, IServiceOwner
    {
        private NetworkManager m_netManager = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string strIP = textBoxIP.Text.Trim();

            if (strIP.Length == 0)
            {
                textBoxIP.Focus();
                MessageBox.Show("IP를 입력하세요.");
                return;
            }

            string strPort = textBoxPort.Text.Trim();

            if (strPort.Length == 0)
            {
                textBoxPort.Focus();
                MessageBox.Show("Port를 입력하세요.");
                return;
            }

            int nPort;

            if (int.TryParse(strPort, out nPort) == false || nPort <= 0)
            {
                textBoxPort.Focus();
                MessageBox.Show("Port는 0보다 큰 정수만 입력 가능합니다.");
                return;
            }

            m_netManager = new NetworkManager(strIP, nPort, this);
        }

        public void OnConnect()
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnConnect.Enabled = false;
                checkBoxFire.Enabled = checkBoxPowerOff.Enabled = checkBoxEarthquake.Enabled = checkBoxWind.Enabled = false;

                byte[] eventTypes = GetEventArray();

                if (eventTypes != null)
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add((short)eventTypes.Length);
                    arrDatas.Add(eventTypes);

                    byte[] bytes = libUSS.BinaryHelper.MakeBytes(libUSS.Header.REQUEST_SELECT_EVENT_TYPE, arrDatas);
                    m_netManager.Send(bytes);
                }
            });
        }

        private byte[] GetEventArray()
        {
            List<byte> eventList = new List<byte>();

            if (checkBoxFire.Checked)
                eventList.Add(libUSS.EventType.Fire);

            if (checkBoxPowerOff.Checked)
                eventList.Add(libUSS.EventType.PowerOff);

            if (checkBoxEarthquake.Checked)
                eventList.Add(libUSS.EventType.Earthquake);

            if (checkBoxWind.Checked)
                eventList.Add(libUSS.EventType.Wind);

            int nEventCount = eventList.Count;

            if (nEventCount == 0)
                return null;

            byte[] bytes = new byte[nEventCount];

            for (int i=0;i<nEventCount;i++)
            {
                bytes[i] = eventList[i];
            }

            return bytes;
        }

        public void OnDropConnection()
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnConnect.Enabled = true;
                checkBoxFire.Enabled = checkBoxPowerOff.Enabled = checkBoxEarthquake.Enabled = checkBoxWind.Enabled = true;
            });
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_netManager != null)
                m_netManager.ReleaseThread();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
