using System;
using System.Configuration;
using System.Windows.Forms;

namespace SVMSServer
{
    public partial class FormLogin : Form
    {
        public string UserID { get; private set; }
        public string Password { get; private set; }
        public string ServerIP { get; private set; }

        public int ServerPort { get; private set; }

        public FormLogin(string strIP, int nPort, string strID, string strPW)
        {
            InitializeComponent();

            ServerIP = strIP;
            ServerPort = nPort;
            UserID = strID;
            Password = strPW;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            textBoxIP.Text = ServerIP;
            textBoxPort.Text = ServerPort.ToString();
            textBoxID.Text = UserID;
            textBoxPassword.Text = Password;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string strServerIP = textBoxIP.Text.Trim();

            if (strServerIP.Length == 0)
            {
                textBoxIP.Focus();
                MessageBox.Show("Server IP를 입력하세요.");
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
                MessageBox.Show("Port는 0보다 큰 숫자만 입력 가능합니다.");
                return;
            }

            string strID = textBoxID.Text.Trim();

            if (strID.Length == 0)
            {
                textBoxID.Focus();
                MessageBox.Show("ID를 입력하세요.");
                return;
            }

            string strPW = textBoxPassword.Text.Trim();

            if (strPW.Length == 0)
            {
                textBoxPassword.Focus();
                MessageBox.Show("비밀번호를 입력하세요.");
                return;
            }

            ServerIP = strServerIP;
            ServerPort = nPort;
            UserID = strID;
            Password = strPW;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
