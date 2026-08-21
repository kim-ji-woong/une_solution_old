using System;
using System.Windows.Forms;
using System.Drawing;

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
            this.PictureBoxTitleImage = global::SDMS.Properties.Resources.App_Icon_Small;


            this.CloseButtonImage = global::SDMS.Properties.Resources.CloseWindow_Normal;
            this.MaxButtonImage = global::SDMS.Properties.Resources.MaxWindow_Normal;
            this.NormalButtonImage = global::SDMS.Properties.Resources.NormalWindow_Normal;
            this.MinButtonImage = global::SDMS.Properties.Resources.HideWindow_Normal;
                        

            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Load += new EventHandler(FormFrame_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);

            nTargetMonitor = nMonitor;
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
                if (sc[i].DeviceName.IndexOf(szNum) != -1)
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
            if (form != null && form.IsDisposed == false)
            {
                form.Dispose();
            }

            if (m_frmMain != null)
            {
                m_frmMain.Visible = false;
                m_frmMain.Close();
            }
        }

        private void FormFrame_Load(object sender, EventArgs e)
        {
            // 대상 모니터의 최대 사이즈와 시작 위치를 Form의 위치로 지정 지정
            SetMonitorForm(this, nTargetMonitor);

            this.ShowMaxButton = true;
            this.ShowMinButton = true;
            this.ShowCloseButton = true;
            this.Text = "CCTV";

            this.m_frmMain.Visible = true;
            this.TitleBarHeight = 30;
            this.Icon = m_frmMain.Icon;

            this.ResizeFrame();

            Button btn = new Button();
            btn.Click += btn_Click;
            btn.Text = "메뉴";
            btn.Size = new Size(100, 24);
            btn.Location = new Point(304, 4);
            btn.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.panelTop.Controls.Add(btn);

            Button btn2 = new Button();
            btn2.Click += btn2_Click;
            btn2.Text = "초기화면";
            btn2.Size = new Size(100, 24);
            btn2.Location = new Point(100, 4);
            btn2.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.panelTop.Controls.Add(btn2);

            Button btn3 = new Button();
            btn3.Click += btn3_Click;
            btn3.Text = "화재탐지 화면";
            btn3.Size = new Size(100, 24);
            btn3.Location = new Point(202, 4);
            btn3.BackColor = Color.FromKnownColor(KnownColor.Control);
            btn3.Enabled = false;
            this.panelTop.Controls.Add(btn3);  
        }

        void btn3_Click(object sender, EventArgs e)
        {
            //if(m_frmMain != null)
            //{
            //    SDMS.Form4CCTV form = SDMS.FormMain.Instance.PageHome.CCTVForm;
            //    if (form != null && form.IsDisposed == false)
            //    {
            //        if (form.LastCCTVList != null)
            //        {
            //            form.SetCCTV(form.LastCCTVList, form.ZoneTarget);
            //            if (form.GetContent(0) != null && form.GetContent(0).GetType() != typeof(PictureBox))
            //                form.SetPanel(0, form.PictureBox1, false);
            //            if (form.GetContent(3) != null && form.GetContent(3).GetType() != typeof(PictureBox))
            //                form.SetPanel(3, form.PictureBox2, false);
            //        }
            //        else
            //            form.SetDefaultCCTV();
            //    }
            //}            
        }

        void btn2_Click(object sender, EventArgs e)
        {
            if(m_frmMain != null)
            {
                SDMS.FormMain.Instance.PageHome.ShowDefaultCCTV();
            }
        }

        private SDMS.FormConfigCCTV form = new SDMS.FormConfigCCTV();

        void btn_Click(object sender, EventArgs e)
        {            
            if (form == null || form.IsDisposed == true)
            {
                form = new SDMS.FormConfigCCTV();
            }


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
                btnMax.BackgroundImage = global::SDMS.Properties.Resources.MaxWindow_Normal;
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
                    if (m_frmMain != null && m_frmMain.IsDisposed == false)
                        m_frmMain.Dispose();
                    break;
            }

            base.WndProc(ref m);
        }

        private void InitializeComponent()
        {
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Location = new System.Drawing.Point(23, 4);
            // 
            // btnMin
            // 
            this.btnMin.Location = new System.Drawing.Point(210, 3);
            // 
            // CCTVFormFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "CCTVFormFrame";
            this.ShowCloseButton = true;
            this.ShowMaxButton = true;
            this.ShowMinButton = true;
            this.ShowPictureBoxTitle = true;
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
            this.ResumeLayout(false);

        }
    }
}