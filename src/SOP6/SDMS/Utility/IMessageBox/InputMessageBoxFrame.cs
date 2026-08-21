using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS.IMessageBox
{
    public partial class InputMessageBoxFrame : UnE.GUI.FormNoFrameSizable
    {
        private static InputMessageBoxFrame m_instance = null;
        public static InputMessageBoxFrame Instance
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

        public InputMessageBoxFrame(Form frmMain)
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
            this.TitleBarBackColor = color;
        }

        void FormFrame_Load(object sender, EventArgs e)
        {

            m_frmMain.Visible = true;

            // 대상 모니터의 최대 사이즈와 시작 위치를 Form의 위치로 지정 지정
            //SetMonitorForm(this, nTargetMonitor);

            this.m_frmMain.Visible = true;

            this.TitleBarHeight = 30;
            this.SystemButtonSize = new Size(30, 24);

            SetFrameColor(m_FrameColor);
            //폰트설정
            this.TitleTextFont = new Font("맑은 고딕", 9, FontStyle.Bold);
            this.TitlePosition = new Point(15, 8);
            this.Text = m_frmMain.Text;

            this.ShowPictureBoxTitle = false;
            this.PictureBoxSize = new Size(24, 24);

            this.ShowPictureBoxTitle = false;

            this.ShowMaxButton = false;
            this.ShowMinButton = false;
            this.ShowCloseButton = true;




            this.ResizeFrame();
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
            if (m_frmMain != null)
                m_frmMain.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
