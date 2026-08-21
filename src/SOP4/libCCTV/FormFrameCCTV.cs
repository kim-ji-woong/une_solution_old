using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace UnE.CCTV
{
    public class CCTVFormFrame : UnE.GUI.FormNoFrameSizable
    {
        private static CCTVFormFrame m_instance = null;

        public static CCTVFormFrame Instance
        {
            get { return m_instance; }
        }

        // CCTV Form의 기본 모니터는 3번임
        // ProxySOP의 CCTVMonitor 확인할것
        private int nTargetMonitor = 3;

        public CCTVFormFrame(Form frmMain, int nMonitor)
            : base(frmMain)
        {
            frmMain.TopLevel = false;

            m_instance = this;

            this.TitleBarHeight = 30;
            this.SystemButtonSize = new System.Drawing.Size(30, 24);

            this.TitleTextFont = new Font("맑은 고딕", 9, FontStyle.Bold);
            this.TitlePosition = new Point(35, 8);
            this.Text = "CCTV";

            this.ShowPictureBoxTitle = true;
            this.PictureBoxSize = new Size(24, 24);
            this.PictureBoxTitleImage = global::UnE.CCTV.Properties.Resources.App_Icon_Small;


            this.CloseButtonImage = global::UnE.CCTV.Properties.Resources.CloseWindow_Normal;
            this.MaxButtonImage = global::UnE.CCTV.Properties.Resources.MaxWindow_Normal;
            this.NormalButtonImage = global::UnE.CCTV.Properties.Resources.NormalWindow_Normal;
            this.MinButtonImage = global::UnE.CCTV.Properties.Resources.HideWindow_Normal;


            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Load += new EventHandler(FormFrame_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.CCTVFormFrame_VisibleChanged);

            SystemButtonSize = new System.Drawing.Size(30, 24);
            btnMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            //btnMin.Margin = new Padding(15);
            this.btnMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            //btnMax.Margin = new Padding(15);
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            //btnClose.Margin = new System.Windows.Forms.Padding(15);
          
            nTargetMonitor = nMonitor;

            //int nCCTV = 1;           
            //string szCCTVForm = DBUtility.RegUtil.ReadRegValue("Monitor Info", "CCTV", UnE.SOP.ProxySOP.Instance.SiteID);
            //int.TryParse(szCCTVForm, out nCCTV);
            SetMonitorForm(this, nTargetMonitor);
        
        }

        /// <summary>
        /// Target Form을 대상 모니터 중앙으로 이동
        /// </summary>
        /// <param name="target">이동할 Form</param>
        /// <param name="nMontior">대상 모니터 번호, 1부터 시작</param>
        private void MoveToScreenCenter(Form target, int nMontior)
        {
            Size size = GetMonitorSize(nMontior);
            Point p = GetMonitorPosition(nMontior);
            int x = p.X + (size.Width / 2) - (target.Size.Width / 2);
            int y = p.Y + (size.Height / 2) - (target.Size.Height / 2);
            target.Location = new Point(x, y);
        }


        /// <summary>
        /// 특정 모니터의 해상도를 구하기
        /// </summary>
        /// <param name="nMonitor">대상 모니터 번호, 1부터 시작</param>
        /// <returns>대상 모니터의 해상도</returns>
        public Size GetMonitorSize(int nMonitor)
        {
            Screen[] sc;
            sc = Screen.AllScreens;

            if (sc.Length == 0)
            {
                return new Size(10, 10);
            }

            string szNum = nMonitor.ToString();
            int nIdx = -1;
            for (int i = 0; i < sc.Length; i++)
            {
                if (sc[i].DeviceName.IndexOf(szNum) != -1)
                {
                    nIdx = i;
                    break;
                }
            }

            if (nIdx == -1)
                nIdx = 0;

            if (sc.Length >= nMonitor)
            {
                return sc[nIdx].Bounds.Size;
            }
            return new Size(10, 10);
        }

        /// <summary>
        /// 특정 모니터의 시작위치 구하기
        /// </summary>
        /// <param name="nMonitor">대상 모니터 번호, 1부터 시작</param>
        /// <returns>대상 모니터의 시작위치</returns>
        public Point GetMonitorPosition(int nMonitor)
        {
            Screen[] sc;
            sc = Screen.AllScreens;

            if (sc.Length == 0)
            {
                return new Point(0, 0);
            }

            string szNum = nMonitor.ToString();
            int nIdx = -1;
            for (int i = 0; i < sc.Length; i++)
            {
                if (sc[i].DeviceName.IndexOf(szNum) != -1)
                {
                    nIdx = i;
                    break;
                }
            }

            if (nIdx == -1)
                nIdx = 0;

            if (sc.Length >= nIdx)
            {
                return sc[nIdx].Bounds.Location;
            }
            return new Point(0, 0);
        }


        /// <summary>
        /// 특정 모니터 전체를 Form이 사용하도록 설정
        /// </summary>
        /// <param name="form">대상 Form</param>
        /// <param name="nDisplay">대상 모니터</param>
        /// <returns>true면 완료/false면 1번모니터로 설정</returns>
        private bool SetMonitorForm(Form form, int nDisplay)
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            if (form == null)
                return false;


            if (sc.Length == 0)
            {
                return false;
            }

            string szNum = nDisplay.ToString();
            int nIdx = -1;
            for (int i = 0; i < sc.Length; i++)
            {
                if (i == (nDisplay - 1))
                //if (sc[i].DeviceName.IndexOf(szNum) != -1)
                {
                    nIdx = i;
                    break;
                }
            }

            if (nIdx == -1)
                nIdx = 0;

            if (sc.Length > nIdx)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = sc[nIdx].Bounds.Location;
                form.Size = new Size(sc[nIdx].Bounds.Width, sc[nIdx].Bounds.Height);

                form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                form.WindowState = FormWindowState.Maximized;
            }

            return true;
        }

        private void FormFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (form != null && form.IsDisposed == false)
            //{
            //    form.Dispose();
            //}
            this.Visible = false;

            if (m_frmMain != null)
            {
                FormMain.Instance.SetVisible(false);
            }

            e.Cancel = true;
        }

        private void FormFrame_Load(object sender, EventArgs e)
        {            
            // 대상 모니터의 최대 사이즈와 시작 위치를 Form의 위치로 지정 지정
            //SetMonitorForm(this, nTargetMonitor);

            this.ShowMaxButton = true;
            this.ShowMinButton = true;
            this.ShowCloseButton = true;
            this.Text = "CCTV";

            this.m_frmMain.Visible = true;
            this.TitleBarHeight = 30;
            this.Icon = m_frmMain.Icon;

            this.ResizeFrame();

            Button btnDefaultScreen = new Button();
            btnDefaultScreen.Click += btnDefaultScreen_Click;
            btnDefaultScreen.Text = "초기화면";
            btnDefaultScreen.Size = new Size(100, 24);
            btnDefaultScreen.Location = new Point(100, 4);
            btnDefaultScreen.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.panelTop.Controls.Add(btnDefaultScreen);

            Button btnDisasterScreen = new Button();
            btnDisasterScreen.Click += btnDisasterScreen_Click;
            btnDisasterScreen.Text = "재난탐지 화면";
            btnDisasterScreen.Size = new Size(100, 24);
            btnDisasterScreen.Location = new Point(202, 4);
            btnDisasterScreen.BackColor = Color.FromKnownColor(KnownColor.Control);
            btnDisasterScreen.Enabled = true;
            this.panelTop.Controls.Add(btnDisasterScreen);

            Button btnCCTVSetting = new Button();
            btnCCTVSetting.Click += btnCCTVSetting_Click;
            btnCCTVSetting.Text = "CCTV 설정";
            btnCCTVSetting.Size = new Size(100, 24);
            btnCCTVSetting.Location = new Point(304, 4);
            btnCCTVSetting.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.panelTop.Controls.Add(btnCCTVSetting);

            Button btnAllCCTV = new Button();
            btnAllCCTV.Click += btnAllCCTV_Click;
            btnAllCCTV.Text = "전체 CCTV";
            btnAllCCTV.Size = new Size(100, 24);
            btnAllCCTV.Location = new Point(406, 4);
            btnAllCCTV.BackColor = Color.FromKnownColor(KnownColor.Control);
            btnAllCCTV.Enabled = true;
            this.panelTop.Controls.Add(btnAllCCTV);

            m_DetectPosition.Text = "";
            m_DetectPosition.AutoSize = true;
            m_DetectPosition.Location = new Point(508, 4);
            m_DetectPosition.Font = new Font("맑은 고딕", 11.0f, FontStyle.Bold);
            m_DetectPosition.ForeColor = Color.White;
            m_DetectPosition.Visible = false;
            this.panelTop.Controls.Add(m_DetectPosition);         

        }

        private Label m_DetectPosition = new Label();
        public Label DetectPosition
        {
            get { return m_DetectPosition; }
            set { m_DetectPosition = value; }
        }

        private FormCCTVList m_frmCCTVList = null;
        private bool m_bLoadingList = false;
        public void PopAllCCTV()
        {
            if (m_frmCCTVList != null && m_frmCCTVList.IsHandleCreated == true && m_frmCCTVList.Visible == true)
                return;

            if (m_bLoadingList == false)
            {
                m_bLoadingList = true;

                m_frmCCTVList = new FormCCTVList();
                m_frmCCTVList.StartPosition = FormStartPosition.Manual;
                m_frmCCTVList.Location = new Point(this.Location.X + ((this.Size.Width - m_frmCCTVList.Size.Width) / 2), this.Location.Y + ((this.Size.Height - m_frmCCTVList.Size.Height) / 2));
                m_frmCCTVList.Text = "CCTV 보기";
                m_frmCCTVList.Show();
            }

            m_bLoadingList = false;
        }

        void btnAllCCTV_Click(object sender, EventArgs e)
        {
            PopAllCCTV();
        }

        void btnDisasterScreen_Click(object sender, EventArgs e)
        {
            if (m_frmMain != null)
            {
                if( FormMain.Instance.TargetZone != null)
                {

                    FormMain.Instance.ShowSituationCCTV();
                }
                    
            }
        }

        void btnDefaultScreen_Click(object sender, EventArgs e)
        {
            if (m_frmMain != null)
            {
                FormMain.Instance.ShowDefaultCCTV();
            }
        }

        private FormConfigCCTV form = null;

        void btnCCTVSetting_Click(object sender, EventArgs e)
        {
            if (form == null || form.IsDisposed == true)
            {
                form = new FormConfigCCTV();
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(this.Location.X + ((this.Size.Width - form.Size.Width) / 2), this.Location.Y + ((this.Size.Height - form.Size.Height) / 2));
            }

            FormMain.Instance.SaveLastState();

            //form.StartPosition = FormStartPosition.CenterParent;
            if (form.Visible == false)
                form.Show();
            else
                form.Visible = false;
        }

        protected override void EdgePanelMouseUp(object sender, MouseEventArgs e)
        {
            base.EdgePanelMouseUp(sender, e);

            if (m_frmMain != null)
                m_frmMain.Refresh();
        }

        public void ToNormalWindow()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                btnMax.BackgroundImage = global::UnE.CCTV.Properties.Resources.MaxWindow_Normal;
                this.MaxButtonClicked();
            }
        }

        public void ToMaxWindow()
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;

                if (NormalButtonImage != null)
                    this.btnMax.BackgroundImage = NormalButtonImage;
            }
        }

        public void ToMinWindow()
        {
            this.MinButtonClicked();
        }

        private const int WM_CLOSE = 0x0010;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CLOSE:
                    FormMain.Instance.SetVisible(false);
                    this.Visible = false;
                    return;
            }

            if (this.WindowState != org)
            {

                this.OnFormWindowStateChanged(EventArgs.Empty);
            }
            org = this.WindowState;
            base.WndProc(ref m);
        }
      
        FormWindowState org = FormWindowState.Normal;      
        protected virtual void OnFormWindowStateChanged(EventArgs e)
        {
            this.Refresh();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CCTVFormFrame));
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.VisibleChanged += new System.EventHandler(this.panelTop_VisibleChanged);
            // 
            // labelTitle
            // 
            this.labelTitle.Location = new System.Drawing.Point(23, 4);
            this.labelTitle.Size = new System.Drawing.Size(37, 15);
            this.labelTitle.Text = "CCTV";
            // 
            // btnMin
            // 
            this.btnMin.Location = new System.Drawing.Point(210, 3);
            // 
            // CCTVFormFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CCTVFormFrame";
            this.ShowCloseButton = true;
            this.ShowMaxButton = true;
            this.ShowMinButton = true;
            this.ShowPictureBoxTitle = true;
            this.Text = "CCTV";
            this.Activated += new System.EventHandler(this.CCTVFormFrame_Activated);
            this.VisibleChanged += new System.EventHandler(this.CCTVFormFrame_VisibleChanged);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
            this.ResumeLayout(false);

        }

        private void panelTop_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void CCTVFormFrame_VisibleChanged(object sender, EventArgs e)
        {
            if( this.Visible == true)
            {               
                this.ToMaxWindow();
                BringFront(this);
            }
        }

        private void CCTVFormFrame_Activated(object sender, EventArgs e)
        {
            BringFront(this);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(
                int hWnd, // window handle
                int hWndInsertAfter, // placement-order handle
                int X, // horizontal position
                int Y, // vertical position
                int cx, // width
                int cy, // height
                uint uFlags); // window positioning flags

        const uint SWP_NOSIZE = 0x1;
        const uint SWP_NOMOVE = 0x2;
        const uint SWP_SHOWWINDOW = 0x40;
        const uint SWP_NOACTIVATE = 0x10;
        const int HWND_TOPMOST = -1;
        const int HWND_NOTOPMOST = -2;
        
        private static void BringFront(Form form)
        {
            SetWindowPos((int)form.Handle, HWND_TOPMOST, 0, 0, 0, 0, 
                                          SWP_NOMOVE|SWP_NOSIZE|SWP_SHOWWINDOW); 
            // 최상위 윈도우 속성을 제거한다. 하지만 윈도우는 다른 윈도우보다 앞에 존재한다. 
            SetWindowPos((int)form.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, 
                                          SWP_NOMOVE|SWP_NOSIZE|SWP_SHOWWINDOW); 
        }
    }    
}