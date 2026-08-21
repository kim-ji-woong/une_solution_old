using DBUtility2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SendSensorResult
{
    public partial class FormMain : Form
    {
        private int m_nReal = -1;
        private string m_strEvtID = "";

        public FormMain(int nReal, string strEvtID)
        {
            InitializeComponent();

            m_nReal = nReal;
            m_strEvtID = strEvtID;

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.Hide();
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            WebServiceManager.SendSOPWebAPI(m_strEvtID, m_nReal);
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
