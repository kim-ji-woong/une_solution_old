using System;
using System.Windows.Forms;

namespace SDMS
{
	public class FormFrame : UnE.GUI.FormNoFrameSizable
	{
		private static FormFrame m_instance = null;

		public static FormFrame Instance
		{
			get { return m_instance; }
		}

        private System.Drawing.Point m_ptOrigin = new System.Drawing.Point();

        public System.Drawing.Point OriginLocation
        {
            get { return m_ptOrigin; }
            set { m_ptOrigin = value; }
        }

        private FormMain m_FormMain = null;
		public FormFrame(Form frmMain)
			: base(frmMain)
		{

            m_FormMain = (FormMain)frmMain;

            this.DoubleBuffered = true;

			m_instance = this;

			this.Load += new EventHandler(FormFrame_Load);
			this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);
            this.Resize += FormFrame_Resize;
           
		}

        void FormFrame_Resize(object sender, EventArgs e)
        {
            //m_FormMain.Size = this.Size;
        }


		private void FormFrame_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_frmMain != null)
			{
				m_frmMain.Visible = false;
				m_frmMain.Close();
			}

            Application.Exit();
		}

		private void FormFrame_Load(object sender, EventArgs e)
		{
            // Splash 화면이 로딩중이므로 모니터 바깥에 띄우도록 한다.
            this.WindowState = FormWindowState.Normal;
            m_ptOrigin = this.Location;
            this.Location = new System.Drawing.Point(-10000, -10000);

			this.ShowMaxButton = false;
			this.ShowMinButton = false;
			this.ShowCloseButton = false;
			this.Text = "";

			this.m_frmMain.Visible = true;

			this.TitleBarHeight = 0;
			this.Icon = m_frmMain.Icon;

			this.ResizeFrame();

			this.ShowMaxButton = true;
			this.ShowMinButton = true;
			this.ShowCloseButton = true;
			this.Text = "SDMS";
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

		private void InitializeComponent()
		{
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Size = new System.Drawing.Size(1151, 20);
            // 
            // panelLeft
            // 
            this.panelLeft.Size = new System.Drawing.Size(5, 616);
            // 
            // panelRight
            // 
            this.panelRight.Location = new System.Drawing.Point(1146, 20);
            this.panelRight.Size = new System.Drawing.Size(5, 616);
            // 
            // panelBottom
            // 
            this.panelBottom.Location = new System.Drawing.Point(5, 636);
            this.panelBottom.Size = new System.Drawing.Size(1141, 5);
            // 
            // panelLB
            // 
            this.panelLB.Location = new System.Drawing.Point(0, 636);
            // 
            // panelRB
            // 
            this.panelRB.Location = new System.Drawing.Point(1146, 636);
            // 
            // labelTitle
            // 
            this.labelTitle.Location = new System.Drawing.Point(23, 10);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(1129, 2);
            // 
            // btnMax
            // 
            this.btnMax.Location = new System.Drawing.Point(1111, 2);
            // 
            // btnMin
            // 
            this.btnMin.Location = new System.Drawing.Point(1093, 2);
            // 
            // FormFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.ClientSize = new System.Drawing.Size(1151, 641);
            this.Name = "FormFrame";
            this.ShowCloseButton = true;
            this.ShowMaxButton = true;
            this.ShowMinButton = true;
            this.ShowPictureBoxTitle = true;
            this.TitleTextWidth = 122;
            this.Activated += new System.EventHandler(this.FormFrame_Activated);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
            this.ResumeLayout(false);

		}

        private const int WM_CLOSE = 0x0010;
        FormWindowState org = FormWindowState.Normal;
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CLOSE:
                    FormMain.Instance.CloseApplication = true;
                    break;
            }
            
            if (this.WindowState != org)
            {

                this.OnFormWindowStateChanged(EventArgs.Empty);
            }
            org = this.WindowState;
            base.WndProc(ref m);
           
        }

        protected virtual void OnFormWindowStateChanged(EventArgs e)
        {
            if (this.m_FormMain != null)
            {
                m_FormMain.ClearSelectDlg();
            }
        }

        private void FormFrame_Activated(object sender, EventArgs e)
        {
            if (this.m_frmMain != null)
            {
                m_frmMain.Activate();
            }
        }
	}
}