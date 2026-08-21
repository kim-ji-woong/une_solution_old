using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
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

        private Point m_ptOrigin = new Point();

        public Point OriginLocation
        {
            get { return m_ptOrigin; }
        }

        private Point m_ptLocation = new Point();
        public new Point Location
        {
            get { return m_ptLocation; }
            set 
            {
                m_ptLocation = value;

               
                base.Location = m_ptLocation;
            }
        }

        public FormFrame(Form frmMain)
            : base(frmMain)
        {
            InitializeComponent();
            this.Text = "SOP Simulator";

            m_instance = this;
            this.Load += new EventHandler(FormFrame_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);
            this.Activated += new EventHandler(FormFrame_Activated);
            this.VisibleChanged += new EventHandler(FormFrame_VisibleChanged);
        }

        private void FormFrame_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && FormSOP.Instance.OnlySDMS && !FormSOP.Instance.ProxyMessenger.SDMSisLoading)
                this.Visible = false;

            if (this.Visible == true)
            {
                this.BringToFront();
            }
                
        }

        void FormFrame_Load(object sender, EventArgs e)
        {
            // Splash 화면이 로딩중이므로 모니터 바깥에 띄우도록 한다.
            this.WindowState = FormWindowState.Normal;
            m_ptOrigin = this.Location;
            this.Location = new System.Drawing.Point(-10000, -10000);

            this.m_frmMain.Visible = true;
            this.TitleBarHeight = 0;
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
            this.Location = m_ptOrigin;
           
        }

        private void FormFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormSOP.Instance.SDMSisClosed)
            {
                e.Cancel = true;
                FormSOP.Instance.btnClose_Click(null, null);
                return;
            }

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
                FormSOP.Instance.Update3DView();
            }
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
            FormSOP.Instance.FormMain_Activated(null, null);
        }

        private void FormFrame_LocationChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("SOPSimulator Location : " + base.Location);
        }
    }
}
