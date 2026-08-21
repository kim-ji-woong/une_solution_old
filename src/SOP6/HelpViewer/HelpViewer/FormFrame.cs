using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HelpViewer
{
    public partial class FormFrame : UnE.GUI.FormNoFrameSizable
    {
        private static FormFrame m_instance = null;
        public static FormFrame Instance
        {
            get { return m_instance; }
        }

        private bool m_firstHidden = false;
        public bool FirstHidden
        {
            get { return m_firstHidden; }
            set { m_firstHidden = value; }
        }

        public FormFrame(Form frmMain)
            : base(frmMain)
        {
            InitializeComponent();
            this.Text = frmMain.Text;

            m_instance = this;
            this.Load += new EventHandler(FormFrame_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);
            this.Activated += new EventHandler(FormFrame_Activated);
            this.VisibleChanged += new EventHandler(FormFrame_VisibleChanged);
        }

        private void FormFrame_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                this.BringToFront();
            }
        }

        void FormFrame_Load(object sender, EventArgs e)
        {
            Point ptPrevLocation = new Point();
            Size sizePrev = new Size();
            int nPrevSplitDistance = ((FormMain)m_frmMain).PrevSplitDistance;
            bool isPrevMaximize = false;

            if (FormMain.ReadPrevLocationNSize(ref ptPrevLocation, ref sizePrev, ref nPrevSplitDistance, ref isPrevMaximize))
            {
                FormMain.SetPrevLocationNSize(this, ptPrevLocation, isPrevMaximize, sizePrev);
                ((FormMain)m_frmMain).PrevSplitDistance = nPrevSplitDistance;
            }

            this.m_frmMain.Visible = true;
            //this.TitleBarHeight = 40;
            /*this.SystemButtonSize = new Size(30, 24);
            
            
            //폰트설정
            this.TitleTextFont = new Font("맑은 고딕", 10, FontStyle.Bold);

            this.ShowPictureBoxTitle = true;
            this.PictureBoxSize = new Size(30, 30);
            this.PictureBoxTitleImage = global::SOPMonitoringSystem.Properties.Resources.Monitoring_32;




            this.Text = "";
            //this.Text = strID +"로 로그인 중" + strisAdmin;
            this.TitlePosition = new Point(40,7);

            this.CloseButtonImage = global::SOPMonitoringSystem.Properties.Resources.CloseWindow_Normal;
            this.MaxButtonImage = global::SOPMonitoringSystem.Properties.Resources.MaxWindow_Normal;
            this.MinButtonImage = global::SOPMonitoringSystem.Properties.Resources.HideWindow_Normal;
            this.NormalButtonImage = global::SOPMonitoringSystem.Properties.Resources.NormalWindow_Normal;*/
            this.ResizeFrame();
        }

        private void FormFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormMain frmMain = (FormMain)this.m_frmMain;

            /*if (frmMain.CloseApplication == false)
            {
                e.Cancel = true;
                return;
            }*/

            if (this.WindowState != FormWindowState.Minimized)
                FormMain.WriteSizeNLocation(this, frmMain.CurrentSplitDistance);

            if (m_frmMain != null)
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

        public void ToNormalWindow()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.MaxButtonClicked();
            }
        }

        private void FormFrame_Activated(object sender, EventArgs e)
        {
            //FormSOP.Instance.FormMain_Activated(null, null);
        }
    }
}
