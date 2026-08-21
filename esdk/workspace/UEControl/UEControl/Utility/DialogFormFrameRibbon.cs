using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace UnE.GUI
{
    public partial class DialogFormFrameRibbon : UnE.GUI.FormNoFrameSizableRibbon
	{
		// 모니터는 1번부터 시작
		//private int nTargetMonitor = 1;
		private Size DiffSize;
		private bool m_bDiffSize = false;

		public static Color BorderColor = Color.FromArgb(53, 50, 61);
		public static Color TitleBarColor = Color.FromArgb(53, 50, 61);
        public static Color TitleTextForeColor = Color.Black;

        private static double m_WindowRateWidth = 1d;
        public static double WindowRateWidth
        {
            get { return m_WindowRateWidth; }
            set { m_WindowRateWidth = value; }
        }

        private static double m_WindowRateHeight = 1d;
        public static double WindowRateHeight
        {
            get { return m_WindowRateHeight; }
            set { m_WindowRateHeight = value; }
        }
        
		private bool m_bCloseDispose = true;
        public DialogFormFrameRibbon(Form frmMain, bool bCloseDispose = true)
			: base(frmMain)
		{

			m_bCloseDispose = bCloseDispose;
			m_bPreventInnerFormResize = true;

			//this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

			RemoveResizeEventHander(frmMain);

			this.Controls.Remove(frmMain);

			this.WindowState = frmMain.WindowState;
			if (frmMain.WindowState != FormWindowState.Normal)
			{
				frmMain.WindowState = FormWindowState.Normal;
			}

			if (frmMain.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
			{
				m_bDiffSize = true;
				DiffSize = new Size((frmMain.Size.Width - frmMain.ClientSize.Width), frmMain.Size.Height - frmMain.ClientSize.Height);
			}

			frmMain.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			if (m_frmMain != null)
			{
				m_frmMain.Location = new Point(EdgeThick, 30);
			}

			InitializeComponent();

            this.MinimumSize = new Size(frmMain.MinimumSize.Width + EdgeThick * 2, frmMain.MinimumSize.Height + TitleBarHeight + EdgeThick*2);
            this.MaximumSize = frmMain.MaximumSize;
			this.ShowInTaskbar = frmMain.ShowInTaskbar;
			this.TopMost = frmMain.TopMost;
			this.TitleBarHeight = 30;
			this.SystemButtonSize = new Size(27, 24);

			this.ClientSize = new Size(m_frmMain.Size.Width + EdgeThick * 2, m_frmMain.Size.Height + TitleBarHeight + EdgeThick);
			this.TitleBarBackColor = TitleBarColor;
			this.LBEdgeBackColor = BorderColor;
			this.RBEdgeBackColor = BorderColor;
			this.LeftEdgeBackColor = BorderColor;
			this.RightEdgeBackColor = BorderColor;
			this.BottomEdgeBackColor = BorderColor;
			//폰트설정
            this.TitleTextFont = new Font("굴림", 13F, FontStyle.Bold);
            this.labelTitle.ForeColor = TitleTextForeColor;
			this.TitlePosition = new Point(5, 8);
			this.Text = m_frmMain.Text;

            this.ShowPictureBoxTitle = m_frmMain.ShowIcon;
            this.pictureBoxTitle.Visible = m_frmMain.ShowIcon;

            if (m_frmMain.ShowIcon == true)
            {
                this.PictureBoxSize = new Size(30, 30);
                if (m_frmMain.Icon != null)
                {
                    this.PictureBoxTitleImage = m_frmMain.Icon.ToBitmap();
                }
            }
            else
            {
                this.TitlePosition = new Point(5, 8);
            }

			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowMaxButton = false;
			this.ShowMinButton = false;

            this.CloseButtonImage = global::UnE.Properties.Resources.Close_40_40_Default;
            this.CloseButtonOverImage = global::UnE.Properties.Resources.Close_40_40_Click;

			//this.MaxButtonImage = global::SOPManager.Properties.Resources.MaxWindow_Normal;
			//this.NormalButtonImage = global::SOPManager.Properties.Resources.NormalWindow_Normal;
			//this.MinButtonImage = global::SOPManager.Properties.Resources.HideWindow_Normal;

			this.FormClosing += new FormClosingEventHandler(this.DialogFormFrame_FormClosing);
			this.FormClosed += new FormClosedEventHandler(this.DialogFormFrame_FormClosed);
			this.Load += new System.EventHandler(this.DialogFormFrame_Load);
			this.Shown += new System.EventHandler(this.DialogFormFrame_Shown);
			this.VisibleChanged += new System.EventHandler(this.DialogFormFrame_VisibleChanged);

			ResizeFrameOnly();

			this.Sizable = false;

			frmMain.FormClosing += new FormClosingEventHandler(InnerForm_FormClosing);
			frmMain.VisibleChanged += new System.EventHandler(this.InnerForm_VisibleChanged);
			frmMain.SizeChanged += new System.EventHandler(this.InnerForm_SizeChanged);
            this.Controls.Add(frmMain);

            UpdateControlSize();
		}

        public void UpdateControlSize()
        {
            this.TitleTextFont = new Font(TitleTextFont.FontFamily, (float)(TitleTextFont.Size * WindowRateWidth), TitleTextFont.Style);
            //this.TitlePosition = new Point((int)(TitlePosition.X * WindowRateWidth), (int)(TitlePosition.Y * WindowRateHeight));

            this.TitleBarHeight = (int)(TitleBarHeight * WindowRateHeight);
            this.SystemButtonSize = new Size((int)(SystemButtonSize.Width * WindowRateWidth), (int)(SystemButtonSize.Height * WindowRateHeight));

            if (m_frmMain.ShowIcon == true)
            {
                this.PictureBoxSize = new Size((int)(PictureBoxSize.Width * WindowRateWidth), (int)(PictureBoxSize.Height * WindowRateHeight));
            }
            else
            {
                this.TitlePosition = new Point((int)(TitlePosition.X * WindowRateWidth), (int)(TitlePosition.Y * WindowRateHeight));
            }

            this.ClientSize = new Size(m_frmMain.Size.Width + EdgeThick * 2, m_frmMain.Size.Height + TitleBarHeight + EdgeThick);
            if (m_frmMain != null)
            {
                m_frmMain.Location = new Point(EdgeThick, (int)(m_frmMain.Location.Y * WindowRateHeight));
            }
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

		protected void ResizeFrameOnly()
		{
			ResizeFramePanels(this.Size);
			ResizeTitle();
			ResizeSystemButtons();
		}

		private bool m_nInitMode = true;
		protected override void ResizeFrame()
		{
			if (m_nInitMode == true)
				ResizeFrameOnly();
			else
				base.ResizeFrame();
		}

		protected override void CloseButtonClicked()
		{
			if (m_frmMain != null)
			{
				if (m_bCloseDispose == true)
				{
					m_frmMain.DialogResult = DialogResult.Cancel;
				}
			}
			DialogResult = DialogResult.Cancel;
			this.Close();
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

		private MethodInfo mSizeChange = null;
		private object mTargetForm = null;

		private void RemoveResizeEventHander(Form form)
		{
			FieldInfo f1 = typeof(Control).GetField("EventSize", BindingFlags.Static | BindingFlags.NonPublic);
			object obj = f1.GetValue(form);
			PropertyInfo pi = form.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
			EventHandlerList list = (EventHandlerList)pi.GetValue(form, null);

			if (list[obj] != null)
			{
				mSizeChange = list[obj].Method;
				mTargetForm = list[obj].Target;

				list.RemoveHandler(obj, list[obj]);
			}

		}

		private bool m_bPreventInnerFormResize = false;
		private bool m_bPreventFrameResize = false;

		protected override void OnFormResize(object sender, EventArgs e)
		{

			// InnerFormResize가 제한경우
			if (m_bPreventInnerFormResize == true)
				return;


			ResizeFrameOnly();


			// InnerForm Resize로 인한 FrameResize를 제한합니다.
			m_bPreventFrameResize = true;

			// InnerForm의 Resize를 수행합니다.
			// 여기서 수행되는 Resize는 FormFrame에 영향을 미치지 않습니다.
			//m_frmMain.Location = new Point(0, 0);		
			//m_frmMain.Size = new Size(ClientSize.Width, ClientSize.Height);
			m_frmMain.Location = new Point(EdgeThick, TitleBarHeight);

			int width = ClientSize.Width - EdgeThick * 2;
			int height = ClientSize.Height - TitleBarHeight - EdgeThick;
			if (m_bDiffSize == true)
			{
				//width += DiffSize.Width;
				//height += DiffSize.Height;
			}
			m_frmMain.Size = new Size(width, height);

			// InnerForm Resize로 인한 FrameResize제한을 해제합니다.
			m_bPreventFrameResize = false;
		}

		private void InnerForm_SizeChanged(object sender, EventArgs e)
		{
			// 부모 프레임을 리사이즈 하지 않는 경우만 사이즈 변환을 수행
			if (m_bPreventFrameResize == false)
			{
				// 이벤트가 순환되지 않도록 InnerFormResize를 중지합니다.
				m_bPreventInnerFormResize = true;
				// 부모폼의 Size변경에 대한 처리를 추가합니다.
				// 자식폼의 크기를 클라이언트 영역의 사이즈로 지정합니다.				

				m_frmMain.Location = new Point(EdgeThick, TitleBarHeight);


				m_bPreventFrameResize = true;
				if (m_bDiffSize == true)
				{
					Size t = new System.Drawing.Size(m_frmMain.Width, m_frmMain.Height);
					m_frmMain.Size = new Size(t.Width - DiffSize.Width, t.Height - DiffSize.Height);
				}
				m_bPreventFrameResize = false;

				int width = m_frmMain.Size.Width + EdgeThick * 2;
				int height = m_frmMain.Size.Height + TitleBarHeight + EdgeThick;
				if (m_bDiffSize == true)
				{
					//width -= DiffSize.Width;
					//height -= DiffSize.Height;
				}


				this.Size = new Size(width, height);

				ResizeFrameOnly();

				// 이벤트가 순환되지 않도록 InnerFormResize를 중지합니다.
				m_bPreventInnerFormResize = false;


			}

			// InnerForm에 Resize 이벤트가 설정된 경우 전달합니다.
			if (mSizeChange != null)
				mSizeChange.Invoke(mTargetForm, new object[] { mTargetForm, e });
		}


		private bool m_bVisibleChangeFormFrame = false;
		private void InnerForm_VisibleChanged(object sender, EventArgs e)
		{
			if (m_bVisibleChangeFormFrame == true)
			{
				return;
			}

			if (m_frmMain != null)
			{
				if (m_frmMain.Visible != this.Visible)
					this.Visible = m_frmMain.Visible;
			}
		}
		private void DialogFormFrame_VisibleChanged(object sender, EventArgs e)
		{
			m_bVisibleChangeFormFrame = true;
			if (m_frmMain != null)
			{
				if (m_frmMain.Visible != this.Visible)
					m_frmMain.Visible = this.Visible;
			}
			m_bVisibleChangeFormFrame = false;
		}



		private void InnerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.DialogResult = m_frmMain.DialogResult;
			m_frmMain = null;
			this.Close();
		}

		private void DialogFormFrame_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_frmMain != null)
			{
				if (m_bCloseDispose == true)
					m_frmMain.Close();
				else
				{
					m_frmMain.Visible = false;
					e.Cancel = true;
				}
			}
		}

		private void DialogFormFrame_FormClosed(object sender, FormClosedEventArgs e)
		{

		}

		private void DialogFormFrame_Load(object sender, EventArgs e)
		{
			m_bPreventInnerFormResize = false;
			this.m_frmMain.Visible = true;

			ResizeFrameOnly();
			m_nInitMode = false;
		}

		private void DialogFormFrame_Shown(object sender, EventArgs e)
		{

		}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(m_frmMain != null)
            {
                if (keyData == Keys.Escape)
                {
                    m_frmMain.Focus();
                    SendKeys.Send("{ESC}");
                }
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }
	}
}
