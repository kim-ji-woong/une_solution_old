using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BroadcastSimulator
{
    public partial class FormMain : Form, IServiceOwner
    {
        private BroadcastManager m_broadcastManager = null;// new BroadcastManager(this, 13000);
        private ClientProvider m_provider = null;
        private bool m_isConnected = false;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_broadcastManager = new BroadcastManager(this, 13000);

            radio1.Checked = true;
            cboChannel.SelectedIndex = 0;
            cboMode.SelectedIndex = 0;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (radio1.Checked)
                Send(1, textBoxIP1, textBoxPort1);
            else if (radio2.Checked)
                Send(2, textBoxIP2, textBoxPort2);
            else if (radio3.Checked)
                Send(3, textBoxIP3, textBoxPort3);
            else if (radio4.Checked)
                Send(4, textBoxIP4, textBoxPort4);
            else if (radio5.Checked)
                Send(5, textBoxIP5, textBoxPort5);
        }

        private void Send(int nEquipID, TextBox textBoxIP, TextBox textBoxPort)
        {
            /*string strIP = textBoxIP.Text.Trim();

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

            if (int.TryParse(strPort, out nPort) == false)
            {
                textBoxPort.Focus();
                MessageBox.Show("Port는 0보다 큰 정수만 입력 가능합니다.");
                return;
            }*/

            if (m_isConnected == false)
                return;

            int nChannel = cboChannel.SelectedIndex + 1;
            bool onOff = cboMode.SelectedIndex == 0;

            m_broadcastManager.SendMessage(nEquipID, nChannel, onOff);

            /*if (Connect(strIP, nPort))
            {
                int nChannel = cboChannel.SelectedIndex + 1;
                bool onOff = cboMode.SelectedIndex == 0;

                SendData(nEquipID, nChannel, onOff);
                m_provider.Close();
                m_provider = null;
            }*/

            btnSend.Enabled = true;
        }

        private void SendData(int nEquipID, int nChannel, bool onOff)
        {
            byte[] bytes = new byte[12];

            bytes[0] = 0x02;
            bytes[1] = (byte)((int)'0' + (nEquipID / 10));
            bytes[2] = (byte)((int)'0' + (nEquipID % 10));
            bytes[3] = (byte)((int)'-');
            bytes[4] = (byte)((int)'0' + (nChannel / 10));
            bytes[5] = (byte)((int)'0' + (nChannel % 10));
            bytes[6] = (byte)((int)'-');
            bytes[7] = (byte)((int)'0');
            bytes[8] = (byte)((int)'0');
            bytes[9] = onOff ? (byte)((int)'1') : (byte)((int)'0');
            bytes[10] = bytes[0];
            bytes[11] = 0x03;

            for (int i = 1; i < 10; i++)
            {
                bytes[10] = (byte)(bytes[10] ^ bytes[i]);
            }

            m_provider.Send(bytes, 0, bytes.Length);
        }

        private bool Connect(string strIP, int nPort)
        {
            btnSend.Enabled = false;

            ClientProvider provider = new ClientProvider();
            provider.LengthAdd = false;

            if (!provider.Connect(strIP, nPort))
            {
                MessageBox.Show(provider.ErrorMessage);
                return false;
            }

            m_provider = provider;
            return true;
        }

        public void OnAccept(string strConnectionInfo)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.labelConnection.Text = "클라이언트 접속중 : " + strConnectionInfo;
            });

            m_isConnected = true;
            //Logger.Instance.Write("클라이언트 접속(" + strConnectionInfo + ")");
        }

        public void OnDropConnection(string strConnectionInfo)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.labelConnection.Text = "접속된 클라이언트 없음";
            });

            m_isConnected = false;
            //Logger.Instance.Write("클라이언트 접속종료(" + strConnectionInfo + ")");
        }
    }
}
