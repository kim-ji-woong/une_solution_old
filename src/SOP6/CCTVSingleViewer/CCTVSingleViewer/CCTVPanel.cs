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

namespace CCTVSingleViewer
{
    public partial class CCTVPanel : UserControl, ICCTVCtrlOwner
    {
        private CCTVCtrl m_cctvCtrl = null;
        private bool m_isSelected = false;
        private bool m_isInit = false;
        private CCTV m_cctv = null;

        //private static CCTV.CCTVType DefaultType = CCTV.CCTVType.RTSP;
        private static CCTV.CCTVType DefaultType = CCTV.CCTVType.Divisys;

        public bool IsSelected
        {
            get { return m_isSelected; }
            set { SetSelected(value); }
        }

        public CCTV CCTV
        {
            get { return m_cctv; }
        }

        public CCTVPanel()
        {
            InitializeComponent();
            CreateCCTVControl(DefaultType);
        }

        private void CreateCCTVControl(CCTV.CCTVType cctvType)
        {
            if (m_cctvCtrl != null && m_cctvCtrl.IsDisposed == false)
                return;

            if (cctvType == CCTV.CCTVType.Divisys)
                m_cctvCtrl = new CCTVCtrl(CCTVTypes.Divisys);
            else if (cctvType == CCTV.CCTVType.RTSP)
                m_cctvCtrl = new CCTVCtrl(CCTVTypes.RTSP);
            else// if (cctvType == CCTV.CCTVType.WESP)
                m_cctvCtrl = new CCTVCtrl(CCTVTypes.WESP);

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

        private static bool IsSameType(CCTVTypes type1, CCTV.CCTVType type2)
        {
            if (type1 == CCTVTypes.Divisys && type2 == CCTV.CCTVType.Divisys)
                return true;
            else if (type1 == CCTVTypes.RTSP && type2 == CCTV.CCTVType.RTSP)
                return true;

            return false;
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

                    Point ptBtn = btnExpand.Location;
                    this.Controls.Remove(btnExpand);

                    this.Controls.Add(this.m_cctvCtrl);

                    this.m_cctvCtrl.Controls.Add(lbTitle);
                    this.m_cctvCtrl.Controls.Add(btnExpand);
                    lbTitle.Location = ptTitle;
                    btnExpand.Location = ptBtn;

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
                        CreateCCTVControl(m_cctv.Type);
                        this.Controls.Add(this.m_cctvCtrl);
                    }
                    else if (IsSameType(m_cctvCtrl.CCTVType, m_cctv.Type) == false)
                    {
                        if (m_cctvCtrl.Controls.Contains(lbTitle))
                        {
                            m_cctvCtrl.Controls.Remove(lbTitle);
                            m_cctvCtrl.Controls.Remove(btnExpand);
                        }
                        else if (this.Controls.Contains(lbTitle))
                        {
                            this.Controls.Remove(lbTitle);
                            this.Controls.Remove(btnExpand);
                        }

                        this.Controls.Remove(this.m_cctvCtrl);
                        this.m_cctvCtrl.Dispose();
                        this.m_cctvCtrl = null;

                        CreateCCTVControl(m_cctv.Type);
                        this.Controls.Add(this.m_cctvCtrl);
                    }

                    if (m_cctvCtrl.Controls.Contains(lbTitle) == false)
                    {
                        Point ptTitle = lbTitle.Location;
                        this.Controls.Remove(lbTitle);

                        Point ptBtn = btnExpand.Location;
                        this.Controls.Remove(btnExpand);

                        lbTitle.Location = ptTitle;
                        btnExpand.Location = ptBtn;
                        this.m_cctvCtrl.Controls.Add(lbTitle);
                        this.m_cctvCtrl.Controls.Add(btnExpand);
                    }

                    if (m_cctv.Type == CCTV.CCTVType.Divisys)
                    {
                        m_cctvCtrl.Properties["IPAddress"] = m_cctv.ChannelNormalURL;
                        //m_cctvCtrl.Properties["UserName"] = "guest";
                        //m_cctvCtrl.Properties["Password"] = "guest";
                        m_cctvCtrl.Properties["UserName"] = m_cctv.UserID.Length == 0 ? "admin" : m_cctv.UserID;
                        m_cctvCtrl.Properties["Password"] = m_cctv.Password.Length == 0 ? "1234" : m_cctv.Password;
                        m_cctvCtrl.Properties["Port"] = "8081";
                        m_cctvCtrl.Properties["Channel"] = m_cctv.ChannelBigURL;

                        /*if (m_cctv.ChannelSmallURL.Length > 0)
                        {
                            int nIndex = m_cctv.ChannelSmallURL.IndexOf('_');

                            if (nIndex > 0)
                            {
                                string strUserName = m_cctv.ChannelSmallURL.Substring(0, nIndex);
                                string strPW = m_cctv.ChannelSmallURL.Substring(nIndex + 1);
                                m_cctvCtrl.Properties["UserName"] = strUserName;
                                m_cctvCtrl.Properties["Password"] = strPW;
                            }
                        }*/
                    }
                    else if (m_cctv.Type == CCTV.CCTVType.RTSP)
                    {
                        string strURL = GetURL(normal);

                        if (strURL != null && strURL.Length > 0)
                        {
                            m_cctvCtrl.Properties["FullURL"] = strURL;
                            m_cctvCtrl.Properties["URL"] = strURL;
                            m_cctvCtrl.Properties["Port"] = "554";
                        }
                    }
                    else if (m_cctv.Type == CCTV.CCTVType.WESP)
                    {
                        m_cctvCtrl.Properties["IPAddress"] = m_cctv.ChannelNormalURL;
                        m_cctvCtrl.Properties["UserName"] = m_cctv.UserID.Length == 0 ? "admin" : m_cctv.UserID;
                        m_cctvCtrl.Properties["Password"] = m_cctv.Password.Length == 0 ? "12345" : m_cctv.Password;
                        m_cctvCtrl.Properties["Port"] = "80";
                        m_cctvCtrl.Properties["Channel"] = m_cctv.ChannelBigURL;
                    }

                    lbTitle.Text = string.Format("{0}. {1}", m_cctv.ID, m_cctv.CameraName);
                    m_cctvCtrl.Connect();
                    m_cctvCtrl.Show();
                }
            }
        }

        private string GetURL(bool normal)
        {
            if (m_cctv == null)
                return "";

            if (normal)
            {
                if (m_cctv.ChannelNormalURL.Length > 0)
                    return m_cctv.ChannelNormalURL;
                else if (m_cctv.ChannelSmallURL.Length > 0)
                    return m_cctv.ChannelSmallURL;
                else if (m_cctv.ChannelBigURL.Length > 0)
                    return m_cctv.ChannelBigURL;
            }
            else
            {
                if (m_cctv.ChannelBigURL.Length > 0)
                    return m_cctv.ChannelBigURL;
                else if (m_cctv.ChannelNormalURL.Length > 0)
                    return m_cctv.ChannelNormalURL;
                else if (m_cctv.ChannelSmallURL.Length > 0)
                    return m_cctv.ChannelSmallURL;
            }

            return "";
        }

        public void Disconnect()
        {
            if (m_cctvCtrl != null)
            {
                if (m_cctvCtrl.Controls.Contains(lbTitle))
                {
                    Point ptTitle = lbTitle.Location;
                    this.m_cctvCtrl.Controls.Remove(lbTitle);

                    Point ptBtn = btnExpand.Location;
                    this.m_cctvCtrl.Controls.Remove(btnExpand);
                    this.Controls.Remove(m_cctvCtrl);

                    m_cctvCtrl.Dispose();

                    lbTitle.Location = ptTitle;
                    this.Controls.Add(lbTitle);

                    btnExpand.Location = ptBtn;
                    this.Controls.Add(lbTitle);
                    this.Controls.Add(btnExpand);
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
                //this.lbTitle.ForeColor = Color.White;
                this.lbTitle.ForeColor = Color.Black;
            }
        }

        public void OnMouseLButtonClick()
        {
            IsSelected = !IsSelected;
            FormMain.Instance.SelectCCTV(this, IsSelected);
            FormMain.Instance.panelCCTV_MouseDown(this, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
        }

        public void OnMouseLButtonDoubleClick()
        {
            bool isBig;

            if (FormMain.Instance.OnLButtonDoubleClick(this, out isBig))
                SetExpandButton(isBig);
        }

        private void lbTitle_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseLButtonClick();
        }

        private void lbTitle_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseLButtonDoubleClick();
        }

        private void CCTVPanel_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseLButtonClick();
        }

        private void CCTVPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseLButtonDoubleClick();
        }

        private void CCTVPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (m_cctvCtrl == null || m_cctvCtrl.IsDisposed || m_cctvCtrl.IsConnected == false)
                    tsMenuDisconnect.Enabled = false;
                else
                    tsMenuDisconnect.Enabled = true;

                this.contextMenuStrip1.Show(this, e.X, e.Y);
            }
        }

        private void tsMenuDisconnect_Click(object sender, EventArgs e)
        {
            ClearCCTV();
        }

        public void ClearCCTV()
        {
            FormMain.Instance.OnChangeCCTV(this, m_cctv, null);
            Disconnect();
            m_cctv = null;
        }

        public void OnRMouseUp(int x, int y)
        {
            if (m_cctvCtrl == null || m_cctvCtrl.IsDisposed || m_cctvCtrl.IsConnected == false)
                tsMenuDisconnect.Enabled = false;
            else
                tsMenuDisconnect.Enabled = true;

            this.contextMenuStrip1.Show(this, x, y);
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
                    FormMain.Instance.OnChangeCCTV(this, m_cctv, cctv);
                    Connect(cctv);

                    FormMain.Instance.SetGroupInfo();
                    FormMain.Instance.EnableSetHome();
                    FormMain.Instance.EnableSetSaveGroup(); ;
                }
            }
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            bool isBig;

            if (FormMain.Instance.OnLButtonDoubleClick(this, out isBig))
                SetExpandButton(isBig);
        }

        private void SetExpandButton(bool isBig)
        {
            if (isBig)
            {
                this.btnExpand.ImageClicked = global::CCTVSingleViewer.Properties.Resources.re_click;
                this.btnExpand.ImageMouseOver = global::CCTVSingleViewer.Properties.Resources.re_hover;
                this.btnExpand.ImageNormal = global::CCTVSingleViewer.Properties.Resources.re_normal;
            }
            else
            {
                this.btnExpand.ImageClicked = global::CCTVSingleViewer.Properties.Resources.ext_click;
                this.btnExpand.ImageMouseOver = global::CCTVSingleViewer.Properties.Resources.ext_hover;
                this.btnExpand.ImageNormal = global::CCTVSingleViewer.Properties.Resources.ext_normal;
            }
        }
    }

    public class MouseUpMessageFilter : IMessageFilter
    {
        private const int WM_RBUTTONUP = 0x0205;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_RBUTTONUP)
                return FormMain.Instance.OnRButtonUp(m);

            return false;
        }
    }
}
