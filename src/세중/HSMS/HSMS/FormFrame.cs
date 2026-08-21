using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HSMS
{
    public partial class FormFrame : UnE.GUI.FormNoFrameSizable 
    {
        private static FormFrame m_instance = null;
        public static FormFrame Instance
        {
            get { return m_instance; }
        }

        // 모니터는 1번부터 시작
        private int nTargetMonitor = 1;

        public FormFrame(Form frmMain)
            : base(frmMain)
        {
            InitializeComponent();

            m_instance = this;
            this.Load += new EventHandler(FormFrame_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);
            
        }

        public FormFrame(Form frmMain, int nMonitor)
            : base(frmMain)
        {
            InitializeComponent();

            m_instance = this;
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

        void FormFrame_Load(object sender, EventArgs e)
        {
            FormLoginMain frmLoginMain = new FormLoginMain();
            frmLoginMain.Size = new Size(600, 329);
            frmLoginMain.StartPosition = FormStartPosition.Manual;

            MoveToScreenCenter(frmLoginMain, nTargetMonitor);

            LoginManager logInMgr = new LoginManager(frmLoginMain);
            if (frmLoginMain.ShowDialog(this) == DialogResult.Cancel)
            {
                this.Close();
                FormFrame.Instance.Close();
                return;
            }
            FormMain formMain = (FormMain)m_frmMain;
            if (formMain != null)
                formMain.DataMgr.ReadDBData();
            
                        
            // 대상 모니터의 최대 사이즈와 시작 위치를 Form의 위치로 지정 지정
            SetMonitorForm(this, nTargetMonitor);

            this.m_frmMain.Visible = true;

            this.TitleBarHeight = 30;
            this.SystemButtonSize = new Size(30, 24);
            
            
            //폰트설정
            this.TitleTextFont = new Font("맑은 고딕", 10, FontStyle.Bold);

            this.ShowPictureBoxTitle = true;
            this.PictureBoxSize = new Size(30, 30);
            this.PictureBoxTitleImage = global::HSMS.Properties.Resources.HSMS_Icon;

            string strID = LoginManager.Instance.LoginUserID;
            //0이면 false, 1이면 true
            int nisAdmin = 0;
            UnE.Utility.Properties.GetProperty("isAdmin", ref nisAdmin);
            string strisAdmin = nisAdmin == 1 ? "(관리자)" : "(일반 사용자)";

            this.Text = strID + strisAdmin;
            //this.Text = strID +"로 로그인 중" + strisAdmin;
            this.TitlePosition = new Point(40,7);

            this.CloseButtonImage = global::HSMS.Properties.Resources.CloseWindow_Normal;
            this.MaxButtonImage = global::HSMS.Properties.Resources.MaxWindow_Normal;
            this.NormalButtonImage = global::HSMS.Properties.Resources.NormalWindow_Normal;
            this.MinButtonImage = global::HSMS.Properties.Resources.HideWindow_Normal;
            this.ResizeFrame();
        }

        private void FormFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(m_frmMain!= null)
            {
                m_frmMain.Close();
            }
        }

        public void TitleBarMouseDown(MouseEventArgs e, Point ptMouse)
        {
            ProcessPanelMouseDown(e, ptMouse);
        }

        public void TitleBarMouseUp(MouseEventArgs e)
        {
            EdgePanelMouseUp(null, e);
        }

        public void TitleBarMouseDrag(MouseEventArgs e, Point ptMouse)
        {
            ProcessPanelMouseMove(null, e, ptMouse);
        }

        public void TitleBarMouseDoubleClick()
        {
            btnMax_Click(null, null);
        }

        protected override void btnMax_Click(object sender, EventArgs e)
        {
            base.btnMax_Click(sender, e);

            
            
        }

        protected override void WndProc(ref Message m)
        {
            FormWindowState previousWindowState = this.WindowState;

            base.WndProc(ref m);

            FormWindowState currentWindowState = this.WindowState;

            if (previousWindowState != currentWindowState)
            {
                FormMain.Instance.Update3DView();
            }
        }
    }
}
