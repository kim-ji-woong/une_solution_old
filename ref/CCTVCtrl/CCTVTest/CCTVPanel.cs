using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Control;

namespace UnECCTV
{
    public partial class CCTVPanel : UserControl, ICCTVCtrlOwner
    {
        private CCTVCtrl m_cctvCtrl = null;
        private bool m_isSelected = false;
        private bool m_isInit = false;

        public bool IsSelected
        {
            get { return m_isSelected; }
            set { SetSelected(value); }
        }

        public CCTVPanel()
        {
            InitializeComponent();
            CreateCCTVControl();
        }

        private void CreateCCTVControl()
        {
            if (m_cctvCtrl != null && m_cctvCtrl.IsDisposed == false)
                return;

#if _SVMS_
            m_cctvCtrl = new CCTVCtrl(CCTVTypes.SVMS);
#else
            m_cctvCtrl = new CCTVCtrl(CCTVTypes.RTSP);
#endif

            this.m_cctvCtrl.BackColor = System.Drawing.Color.Black;
            this.m_cctvCtrl.CCTVID = 0;
            this.m_cctvCtrl.CCTVOwner = this;
            this.m_cctvCtrl.Location = new System.Drawing.Point(0, 0);
            this.m_cctvCtrl.Name = "m_cctvCtrl";
            this.m_cctvCtrl.PositionIndex = -1;
            this.m_cctvCtrl.Size = new System.Drawing.Size(316, 240);
            this.m_cctvCtrl.Dock = DockStyle.Fill;
            this.m_cctvCtrl.TabIndex = 4;
        }

        public void Connect(string strURL, string strCameraName)
        {
            if (m_isInit == false)
            {
                Point ptTitle = lbTitle.Location;
                this.Controls.Remove(lbTitle);

                this.Controls.Add(this.m_cctvCtrl);

                this.m_cctvCtrl.Controls.Add(lbTitle);
                lbTitle.Location = ptTitle;

                m_isInit = true;
            }

            if (m_cctvCtrl != null)
            {
                if (m_cctvCtrl.IsDisposed == false && m_cctvCtrl.IsConnected)
                    Disconnect();

                if (m_cctvCtrl.IsDisposed)
                {
                    CreateCCTVControl();
                    this.Controls.Add(this.m_cctvCtrl);
                }

                if (m_cctvCtrl.Controls.Contains(lbTitle) == false)
                {
                    Point ptTitle = lbTitle.Location;
                    this.Controls.Remove(lbTitle);

                    lbTitle.Location = ptTitle;
                    this.m_cctvCtrl.Controls.Add(lbTitle);
                }

                if (strURL != null && strURL.Length > 0)
                {
#if _SVMS_
                    m_cctvCtrl.AddProperty("URL", strURL);
                    m_cctvCtrl.AddProperty("Port", "554");
#else
                    m_cctvCtrl.Properties["FullURL"] = strURL;
#endif
                }

                lbTitle.Text = strCameraName;
                m_cctvCtrl.Connect();
                m_cctvCtrl.Show();
            }
        }

        public void Disconnect()
        {
            if (m_cctvCtrl != null)
            {
                if (m_cctvCtrl.Controls.Contains(lbTitle))
                {
                    Point ptTitle = lbTitle.Location;
                    this.m_cctvCtrl.Controls.Remove(lbTitle);
                    this.Controls.Remove(m_cctvCtrl);

                    m_cctvCtrl.Dispose();

                    lbTitle.Location = ptTitle;
                    this.Controls.Add(lbTitle);
                }
                else
                    m_cctvCtrl.Dispose();
            }

            lbTitle.Text = "CCTV정보 없음";
        }

        public void OnMouseLButtonClick()
        {
            IsSelected = !IsSelected;
            FormMain.Instance.SelectCCTV(this, IsSelected);
        }

        public void OnMouseLButtonDoubleClick()
        {

        }

        private void lbTitle_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                OnMouseLButtonClick();
        }

        private void lbTitle_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                OnMouseLButtonDoubleClick();
        }

        private void SetSelected(bool isSelected)
        {
            m_isSelected = isSelected;

            if (m_isSelected)
            {
                this.lbTitle.ForeColor = Color.Orange;
            }
            else
            {
                this.lbTitle.ForeColor = Color.White;
            }
        }

        private void CCTVPanel_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseLButtonClick();
        }

        private void CCTVPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseLButtonDoubleClick();
        }
    }
}
