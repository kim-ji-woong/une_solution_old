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

namespace RtspUrlEditor
{
    public partial class CCTVPanel : UserControl, ICCTVCtrlOwner
    {
        private CCTVCtrl m_cctvCtrl = null;
        private bool m_isSelected = false;
        private bool m_isInit = false;

        private CCTV m_cctv = null;

        public CCTV CCTV
        {
            get { return m_cctv; }
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

            m_cctvCtrl = new CCTVCtrl(CCTVTypes.RTSP);

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

        public void Connect(CCTV cctv, bool normal = true)
        {
            if (m_cctv == cctv)
                return;

            bool needClose = m_cctv != null;
            m_cctv = cctv;

            if (m_cctv == null)
                Disconnect();
            else
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

                if (needClose)
                    Disconnect();

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

                    string strURL = GetURL(normal);

                    if (strURL != null && strURL.Length > 0)
                    {
                        m_cctvCtrl.Properties["FullURL"] = strURL;
                        m_cctvCtrl.Properties["URL"] = strURL;
                        m_cctvCtrl.Properties["Port"] = "554";
                    }

                    lbTitle.Text = string.Format("{0}. {1}", m_cctv.ID, m_cctv.CCTVName);
                    m_cctvCtrl.Connect();
                    m_cctvCtrl.Show();
                }
            }
        }

        private string GetURL(bool normal)
        {
            if (m_cctv == null)
                return "";

            return m_cctv.URL;
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

                    this.Controls.Add(lbTitle);
                }
                else
                    m_cctvCtrl.Dispose();
            }

            lbTitle.Text = "CCTV정보 없음";
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
                this.lbTitle.ForeColor = Color.Black;
            }
        }

        public void OnMouseLButtonClick()
        {
        }

        public void OnMouseLButtonDoubleClick()
        {
        }

        public void ClearCCTV()
        {
            Disconnect();
            m_cctv = null;
        }

        private void CCTVPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CCTV)))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }

            e.Effect = DragDropEffects.None;
        }

        private void CCTVPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CCTV)))
            {
                CCTV cctv = (CCTV)e.Data.GetData(typeof(CCTV));

                if (m_cctv != cctv)
                {
                    Connect(cctv);
                }
            }
        }
    }
}
