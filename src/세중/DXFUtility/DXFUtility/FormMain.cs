using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBUtility;

namespace DXFUtility
{
    public partial class FormMain : Form
    {
        private static FormMain m_instance = null;
        private WebDBManager m_dbMgr = new WebDBManager();

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
        }

        private void btnReadZone_Click(object sender, EventArgs e)
        {
            ZoneBoundaryLoader loader = new ZoneBoundaryLoader();
            loader.Run();
        }
    }
}
