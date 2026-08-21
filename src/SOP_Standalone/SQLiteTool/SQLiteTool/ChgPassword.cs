using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteTool
{
    public partial class ChgPassword : Form
    {
        private DbManager m_dbMgr = null;
        public string strChgPw = "";

        public ChgPassword(DbManager dbMgr)
        {
            InitializeComponent();

            this.m_dbMgr = dbMgr;
        }

        private void btnChg_Click(object sender, EventArgs e)
        {
            bool suc = m_dbMgr.ChgPw(textBox1.Text);
            if (suc)
            {
                strChgPw = textBox1.Text;
                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.No;
            }
        }
    }
}
