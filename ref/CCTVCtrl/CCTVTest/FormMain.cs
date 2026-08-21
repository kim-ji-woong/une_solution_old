using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnECCTV
{
    public partial class FormMain : Form
    {
        private int m_nCCTVTop = -1, m_nCCTVBottom;
        private int m_nCCTVLeft, m_nCCTVRight;
        private int m_nCCTVMiddleHorz, m_nCCTVMiddleVert;

        private FormCCTVList m_frmList = null;
        private CCTV m_cctv = null;

        private CCTVPanel m_selectedCCTV = null;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();

            InitCCTVSize();

            SetCCTV(null);
        }

        private void btnShowCCTVList_Click(object sender, EventArgs e)
        {
            if (m_frmList == null || m_frmList.IsDisposed)
            {
                m_frmList = new FormCCTVList();
                m_frmList.Show();
            }
            else
            {
                m_frmList.Focus();
            }
        }

        private void InitCCTVSize()
        {
            m_nCCTVTop = panelCCTV1.Location.Y;
            m_nCCTVLeft = panelCCTV1.Location.X;
            m_nCCTVRight = panelRight.Location.X - (panelCCTV3.Location.X + panelCCTV3.Size.Width);
            m_nCCTVMiddleHorz = panelCCTV2.Location.X - (panelCCTV1.Location.X + panelCCTV1.Size.Width);
            m_nCCTVMiddleVert = panelCCTV4.Location.Y - (panelCCTV1.Location.Y + panelCCTV1.Size.Height);
            m_nCCTVBottom = panelRight.Size.Height - (panelCCTV4.Location.Y + panelCCTV4.Size.Height);
        }

        private void btnConnectChannel_Click(object sender, EventArgs e)
        {
            if (m_cctv == null || m_selectedCCTV == null)
                return;

            if (sender == btnConnectChannel1)
                m_selectedCCTV.Connect(m_cctv.Channel1URL, string.Format("{0}. {1}", m_cctv.ID, m_cctv.CameraName));
            else if (sender == btnConnectChannel2)
                m_selectedCCTV.Connect(m_cctv.Channel2URL, string.Format("{0}. {1}", m_cctv.ID, m_cctv.CameraName));
            else if (sender == btnConnectChannel3)
                m_selectedCCTV.Connect(m_cctv.Channel3URL, string.Format("{0}. {1}", m_cctv.ID, m_cctv.CameraName));
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (m_cctv == null || m_selectedCCTV == null)
                return;

            m_selectedCCTV.Disconnect();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (m_nCCTVTop < 0)
                return;

            int nAreaWidth = this.ClientRectangle.Width - panelRight.Size.Width;
            int nAreaHeight = panelRight.Size.Height;

            int nCCTVWidth = (nAreaWidth - m_nCCTVLeft - m_nCCTVMiddleHorz * 2 - m_nCCTVRight) / 3;
            int nCCTVHeight = (nAreaHeight - m_nCCTVTop - m_nCCTVMiddleVert - m_nCCTVBottom) / 2;

            panelCCTV1.Size = new Size(nCCTVWidth, nCCTVHeight);

            panelCCTV2.Location = new Point(m_nCCTVLeft + nCCTVWidth + m_nCCTVMiddleHorz, panelCCTV2.Location.Y);
            panelCCTV2.Size = panelCCTV1.Size;

            panelCCTV3.Location = new Point(panelCCTV2.Location.X + nCCTVWidth + m_nCCTVMiddleHorz, panelCCTV3.Location.Y);
            panelCCTV3.Size = panelCCTV1.Size;

            panelCCTV4.Location = new Point(panelCCTV1.Location.X, panelCCTV1.Location.Y + nCCTVHeight + m_nCCTVMiddleVert);
            panelCCTV4.Size = panelCCTV1.Size;

            panelCCTV5.Location = new Point(panelCCTV2.Location.X, panelCCTV4.Location.Y);
            panelCCTV5.Size = panelCCTV1.Size;

            panelCCTV6.Location = new Point(panelCCTV3.Location.X, panelCCTV4.Location.Y);
            panelCCTV6.Size = panelCCTV1.Size;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*panelCCTV1.Disconnect();
            panelCCTV2.Disconnect();
            panelCCTV3.Disconnect();
            panelCCTV4.Disconnect();
            panelCCTV5.Disconnect();
            panelCCTV6.Disconnect();*/
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }

        public void SetCCTV(CCTV cctv)
        {
            m_cctv = cctv;

            if (m_cctv == null)
            {
                labelCCTVID.Text = "";
                labelCCTVName.Text = "";

                btnConnectChannel1.Enabled = btnConnectChannel2.Enabled = btnConnectChannel3.Enabled = false;
            }
            else
            {
                labelCCTVID.Text = m_cctv.ID.ToString();
                labelCCTVName.Text = m_cctv.CameraName;

                btnConnectChannel1.Enabled = m_cctv.Channel1URL.Length > 0;
                btnConnectChannel2.Enabled = m_cctv.Channel2URL.Length > 0;
                btnConnectChannel3.Enabled = m_cctv.Channel3URL.Length > 0;
            }
        }

        public void SelectCCTV(CCTVPanel cctv, bool isSelected)
        {
            if (m_selectedCCTV != cctv)
            {
                if (m_selectedCCTV != null)
                    m_selectedCCTV.IsSelected = false;

                m_selectedCCTV = cctv;
            }
        }
    }
}
