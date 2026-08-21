using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;

namespace RestartClients
{
    public partial class FormMain : Form
    {
        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private External.NetworkManager m_netMgr = null;
        private WebDBManager m_dbMgr = null;
        private bool m_isConnected = false;

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
            int nSiteID = ReadSiteID();

            if (nSiteID > 0)
            {
                labelStatus.Text = "";
                m_dbMgr = new WebDBManager(nSiteID);
                m_netMgr = new External.NetworkManager(m_dbMgr, nSiteID);
            }
            else
            {
                labelStatus.Text = "서버와 접속할 수 없습니다.";
            }
        }

        private int ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return -1;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId) == false)
            {
                MessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return -1;
            }

            return nSiteId;
        }

        public void SetConnected()
        {
            if (m_isConnected)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                labelStatus.Text = "서버와 접속되었습니다.";
                btnSendRestart.Enabled = true;
            });

            m_isConnected = true;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_netMgr != null)
                m_netMgr.ReleaseThread();
        }

        private void btnSendRestart_Click(object sender, EventArgs e)
        {
            m_netMgr.ClientProvider.SendData(SDMS.TCP_ID.END_RESTORE);
        }
    }
}
