using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerConverter
{
    public partial class FormMain : Form
    {
        public enum Option { Local = 0, Remote, None };

        private FormConfig m_frmConfig = new FormConfig();
        private bool m_isFirst = true;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Option opt = m_frmConfig.Option;

            if (opt == Option.Local)
                radioLocal.Checked = true;
            else if (opt == Option.Remote)
                radioRemote.Checked = true;

            m_isFirst = false;
            radioLocal.Enabled = radioRemote.Enabled = false;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            radioLocal.Enabled = radioRemote.Enabled = true;
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            m_frmConfig.ShowDialog();
        }

        private void radioLocal_CheckedChanged(object sender, EventArgs e)
        {
            if (m_isFirst)
                return;

            if (radioLocal.Checked)
                m_frmConfig.SetLocal();
        }

        private void radioRemote_CheckedChanged(object sender, EventArgs e)
        {
            if (m_isFirst)
                return;

            if (radioRemote.Checked)
                m_frmConfig.SetRemote();
        }
    }
}
