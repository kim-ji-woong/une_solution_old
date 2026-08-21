using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServerCommander
{
    public partial class FormMain : Form
    {
        static private FormMain m_instance = null;

        static public FormMain Instance
        {
            get { return m_instance; }
        }

        private NetworkManager m_netMgr = null;

        private bool m_isConnected = false;

        public void SetConnection(bool isConnected, string strServerAddr, string strPort)
        {
            if (m_isConnected == isConnected)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                this._SetConnection(isConnected, strServerAddr, strPort);
            });
        }

        private void _SetConnection(bool isConnected, string strServerAddr, string strPort)
        {
            m_isConnected = isConnected;

            if (m_isConnected)
            {
                labelNetStatus.Text = "(" + strServerAddr + ":" + strPort + ")Server와 접속되었음";
                btnUpdateSystem.Enabled = btnStartSDMS.Enabled = true;
            }
            else
            {
                labelNetStatus.Text = "(" + strServerAddr + ":" + strPort + ")Server와 연결되지 않음";
                btnUpdateSystem.Enabled = btnStartSDMS.Enabled = false;
            }
        }

        public FormMain()
        {
            m_instance = this;

            InitializeComponent();

            m_netMgr = new NetworkManager();
        }

        private void btnStartSDMS_Click(object sender, EventArgs e)
        {
            m_netMgr.SendRunSDMS();
        }

        private void btnUpdateSystem_Click(object sender, EventArgs e)
        {
            m_netMgr.UpdateSystem();
        }
    }
}
