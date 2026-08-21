using System;
using System.Drawing;
using System.Windows.Forms;

namespace UnE.Utility
{
    internal partial class UMessageBoxFrameRibbon : UnE.GUI.FormNoFrameSizableRibbon
    {
        private static UMessageBoxFrameRibbon m_instance = null;
        public static UMessageBoxFrameRibbon Instance
        {
            get { return m_instance; }
        }

        // 모니터는 1번부터 시작
		private int nTargetMonitor = 1;
		public int TargetMonitor
		{
			get { return nTargetMonitor; }
			set { nTargetMonitor = value; }
		}

        private double m_WindowRateWidth = 1d;
        public double WindowRateWidth
        {
            get { return m_WindowRateWidth; }
            set { m_WindowRateWidth = value; }
        }

        private double m_WindowRateHeight = 1d;
        public double WindowRateHeight
        {
            get { return m_WindowRateHeight; }
            set { m_WindowRateHeight = value; }
        }

        public UMessageBoxFrameRibbon(Form frmMain)
            : base(frmMain)
        {
            InitializeComponent();

            m_instance = this;
            this.Load += new EventHandler(FormFrame_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);
            frmMain.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);

            this.StartPosition = FormStartPosition.CenterParent;         
        }

        private Color m_FrameColor = Color.FromArgb(60, 56, 71);
        public void SetFrameColor(Color color)
        {
            m_FrameColor = color;
            this.LBEdgeBackColor = color;
            this.RBEdgeBackColor = color;
            this.LeftEdgeBackColor = color;
            this.RightEdgeBackColor = color;
            this.BottomEdgeBackColor = color;
        }

        private Color m_TitleColor = Color.FromArgb(60, 56, 71);
        public void SetTitleColor(Color color)
        {
            m_TitleColor = color;
            this.TitleBarBackColor = color;
        }

        void FormFrame_Load(object sender, EventArgs e)
        {
            m_frmMain.Visible = true;
            
            // 대상 모니터의 최대 사이즈와 시작 위치를 Form의 위치로 지정 지정
            //SetMonitorForm(this, nTargetMonitor);

            this.m_frmMain.Visible = true;

            SetFrameColor(m_FrameColor);
            SetTitleColor(m_TitleColor);            
             
            this.Text = m_frmMain.Text;

            this.ShowPictureBoxTitle = false;

            this.ShowMaxButton = false;
            this.ShowMinButton = false;
            this.ShowCloseButton = true;

            this.ResizeFrame();
        }

        public void UpdateControlSize()
        {
            this.TitleBarHeight = 30;
            this.SystemButtonSize = new Size(24, 24);
            this.TitleTextFont = new Font("굴림", 13F, FontStyle.Bold);
            this.TitlePosition = new Point(5, 8);
            this.PictureBoxSize = new Size(24, 24);

            this.TitleBarHeight = (int)(TitleBarHeight * WindowRateHeight);

            this.SystemButtonSize = new Size((int)(SystemButtonSize.Width * WindowRateWidth), (int)(SystemButtonSize.Height * WindowRateHeight));

            this.PictureBoxSize = new Size((int)(PictureBoxSize.Width * WindowRateWidth), (int)(PictureBoxSize.Height * WindowRateHeight));

            this.TitlePosition = new Point((int)(TitlePosition.X * WindowRateWidth), (int)(TitlePosition.Y * WindowRateHeight));
            this.TitleTextFont = new Font(TitleTextFont.FontFamily, (float)(TitleTextFont.Size * WindowRateWidth), TitleTextFont.Style);

            base.OnFormResize(null, null);
        }

        protected override void OnFormResize(object sender, EventArgs e)
        {
            base.OnFormResize(sender, e);
        }

        private void FormFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_frmMain != null && sender == this)
            {
                m_frmMain.Close();
            }
            if (m_frmMain != null && sender != this)
            {
                DialogResult = m_frmMain.DialogResult;
                m_frmMain = null;                
                Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (m_frmMain == null)
                return;

            m_frmMain.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }

}