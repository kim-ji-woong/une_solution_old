using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnE.Utility.Print
{
    public partial class FormPrintFrame : UnE.GUI.FormNoFrameSizable 
    {
        private static FormPrintFrame m_instance = null;
        public static FormPrintFrame Instance
        {
            get { return m_instance; }
        }

        // 모니터는 1번부터 시작
        private int nTargetMonitor = 1;
		private bool m_bMaximized = true;

		public bool FrameMaximized
		{
			get { return m_bMaximized; }
			set { m_bMaximized = value; }
		}

        public FormPrintFrame(Form frmMain)
            : base(frmMain)
        {
            InitializeComponent();
			frmMain.TopLevel = false;
            m_instance = this;
            this.Load += new EventHandler(FormFrame_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);
			this.Activated += new EventHandler(FormFrame_Activated);

			//nTargetMonitor = GetMonitorForm(frmMain.Location);
			
        }

        public FormPrintFrame(Form frmMain, int nMonitor)
            : base(frmMain)
        {
            InitializeComponent();

            m_instance = this;
			frmMain.TopLevel = false;
            this.Load += new EventHandler(FormFrame_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);
			this.Activated += new EventHandler(FormFrame_Activated);
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

		public int GetMonitorForm(Point pt)
		{
			Screen[] sc;
			sc = Screen.AllScreens;

			if (sc.Length == 0)
			{
				return 1;
			}

			Rectangle rect = new Rectangle();
			rect.Location = pt;
			rect.Width = 1;
			rect.Height = 1;
			int nIdx = -1;
			for (int i = 0; i < sc.Length; i++)
			{
				if (sc[i].Bounds.IntersectsWith(rect))
				{
					nIdx = i;
					break;
				}
			}

			if (nIdx == -1)
				nIdx = 0;


			return (nIdx + 1);
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
				if (m_bMaximized)
					form.WindowState = FormWindowState.Maximized;
            }
            else
            {
				if (m_bMaximized)
					form.WindowState = FormWindowState.Maximized;
            }

            return true;
        }       

        void FormFrame_Load(object sender, EventArgs e)
        {
			Form form = this.Owner;
			if (form != null)
			{
				nTargetMonitor = GetMonitorForm(form.Location);
			}
			
            
            // 대상 모니터의 최대 사이즈와 시작 위치를 Form의 위치로 지정 지정
            SetMonitorForm(this, nTargetMonitor);

            this.m_frmMain.Visible = true;

            this.TitleBarHeight = 30;
            this.SystemButtonSize = new Size(30, 24);


			Color borderColor = Color.FromArgb(53, 50, 61);
			this.LBEdgeBackColor = borderColor;
			this.RBEdgeBackColor = borderColor;
			this.LeftEdgeBackColor = borderColor;
			this.RightEdgeBackColor = borderColor;
			this.BottomEdgeBackColor = borderColor;
			//this.panelTop.BackColor = borderColor;
            //폰트설정
            this.TitleTextFont = new Font("맑은 고딕", 9, FontStyle.Bold);
            this.TitlePosition = new Point( 35, 8);
           

            this.ShowPictureBoxTitle = true;
            this.PictureBoxSize = new Size(30, 30);
			this.PictureBoxTitleImage = global::RoadMan.Properties.Resources.RoadMan_Icon;


			this.CloseButtonImage = global::RoadMan.Properties.Resources.CloseWindow_Normal;
			this.MaxButtonImage = global::RoadMan.Properties.Resources.MaxWindow_Normal;
			this.NormalButtonImage = global::RoadMan.Properties.Resources.NormalWindow_Normal;

			this.MinButtonImage = global::RoadMan.Properties.Resources.HideWindow_Normal;
			this.MinimizeBox = false;
			this.ShowMinButton = false;
			
            this.ResizeFrame();
        }

        protected override void OnFormResize(object sender, EventArgs e)
        {
            base.OnFormResize(sender, e);
        }


		protected override void CloseButtonClicked()
		{
			if (m_frmMain != null)
			{
				m_frmMain.DialogResult = DialogResult.Cancel;				
			}

			DialogResult = DialogResult.Cancel;
			this.Visible = false;

		}

        private void FormFrame_FormClosing(object sender, FormClosingEventArgs e)
        {			
            if(m_frmMain!= null)
            {
				m_frmMain.Visible = false;                      
            }
        }


		private bool m_isFirst = false;
		private void FormFrame_Activated(object sender, EventArgs e)
		{
			if (!m_isFirst)
			{
				m_frmMain.Activate();
				m_isFirst = true;
			}
		}
    }

	
}
