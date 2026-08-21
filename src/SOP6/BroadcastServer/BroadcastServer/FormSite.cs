using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BroadcastServer
{
    public partial class FormSite : Form
    {
        private int m_nSiteID = 0;

        public int SiteID
        {
            get { return m_nSiteID; }
        }

        public FormSite(int nSiteID = 0)
        {
            InitializeComponent();

            if (nSiteID > 0)
                textBoxSiteID.Text = nSiteID.ToString();

            m_nSiteID = nSiteID;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string strSiteID = textBoxSiteID.Text.Trim();

            if (strSiteID.Length == 0)
            {
                DialogResult result = MessageBox.Show(this, "Site ID가 지정되지 않으면 방송 서버 기능이 동작하지 않습니다.\r\n이대로 계속 진행하시겠습니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    m_nSiteID = 0;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    textBoxSiteID.Focus();
                }
            }
            else
            {
                int nSiteID;

                if (int.TryParse(strSiteID, out nSiteID) == false || nSiteID <= 0)
                {
                    textBoxSiteID.Focus();
                    MessageBox.Show("Site ID는 0보다 큰 정수 형태로 입력되어야만 합니다.");
                }
                else
                {
                    m_nSiteID = nSiteID;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
