using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;

namespace PSMExternalServer
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = new WebDBManager(1);
        private Network.Server m_server = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_server = new Network.Server(m_dbMgr);
            m_server.BeginServer();

            cboCommandType.SelectedIndex = 0;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_server.StopServer();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string strFirst = CheckContents(textBoxFirst);

            if (strFirst == null)
                return;

            string strMiddle = CheckContents(textBoxMiddle);

            if (strMiddle == null)
                return;

            string strLast = CheckContents(textBoxLast);

            if (strLast == null)
                return;

            if (m_server.Provider != null)
            {
                Network.ServerServiceProvider.CommandType cmd = (Network.ServerServiceProvider.CommandType)cboCommandType.SelectedIndex;
                string strContents = cmd == Network.ServerServiceProvider.CommandType.RestoreAll ? "000000" : strFirst + strMiddle + strLast;
                m_server.Provider.SendCommand(strContents, cmd);
            }
        }

        private string CheckContents(TextBox textBox)
        {
            string str = textBox.Text.Trim();

            if (str.Length != 2)
            {
                textBox.Focus();
                MessageBox.Show("2글자의 아스키 코드값이 입력되어야만 합니다.");
                return null;
            }

            return str;
        }
    }
}
