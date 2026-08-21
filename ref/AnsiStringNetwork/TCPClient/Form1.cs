using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TCPClient
{
    public partial class Form1 : Form
    {
        static private Form1 m_instance = null;
        static public Form1 Instance
        {
            get { return m_instance; }
        }

        private ClientProvider m_provider = new ClientProvider();
        private bool m_isConnected = false;

        public Form1()
        {
            m_instance = this;
            InitializeComponent();

            m_provider.LengthAdd = false;
        }

        public void OnReceive()
        {
            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            string strReceived = encEUC_KR.GetString(m_provider.ReceivedData);

            //string strReceived = Encoding.ASCII.GetString(m_provider.ReceivedData, 0, m_provider.ReceivedData.Length);

            Invoke((MethodInvoker)delegate
            {
                AddMessage("Recv : " + strReceived);
            });
        }

        public void OnDropConnection()
        {
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (textBoxIP.Text.Length == 0)
            {
                MessageBox.Show("IP를 입력해 주세요");
            }
            else if (textBoxPortNo.Text.Length == 0)
            {
                MessageBox.Show("포트번호를 입력해 주세요");
            }
            else
            {
                int nPort;
                bool flag = false;

                if (!int.TryParse(textBoxPortNo.Text, out nPort))
                    flag = false;

                if (nPort > 0)
                    flag = true;

                if (!flag)
                {
                    MessageBox.Show("포트번호는 0보다 큰 정수이어야 합니다.");
                    return;
                }

                if (!m_provider.Connect(textBoxIP.Text, nPort))
                {
                    MessageBox.Show(m_provider.ErrorMessage);
                }
                else
                {
                    m_isConnected = true;
                    AddMessage("[" + textBoxPortNo.Text + "]에 접속 성공하였습니다.");
                }
            }
        }

        private void textBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend_Click(null, null);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (textBoxMessage.Text.Length == 0)
                return;

            if (m_isConnected)
            {
                // EUC-KR : 51949
                Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
                byte[] bytes = encEUC_KR.GetBytes(textBoxMessage.Text.ToArray());

                m_provider.Send(bytes, 0, bytes.Length);

                AddMessage("Send : " + textBoxMessage.Text);
            }

            textBoxMessage.Text = "";
        }

        private void AddMessage(string strMessage)
        {
            if (textBoxView.Text.Length == 0)
                textBoxView.Text = strMessage;
            else
                textBoxView.Text += "\r\n" + strMessage;
        }
    }
}
