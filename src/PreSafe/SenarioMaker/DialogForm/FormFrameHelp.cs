using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.SenarioMaker
{
    public partial class FormFrameHelp : UnE.GUI.FormNoFrameSizable
    {
        public enum HelpType
        {
            Default = 0,
            System = 1,
            User = 2,
            Enum = 3
        }

        private static FormFrameHelp m_instance = null;
        public static FormFrameHelp Instance
        {
            set { m_instance = value; }
        }

        public FormFrameHelp(Form frmHelp)
            :base(frmHelp)
        {
            InitializeComponent();

            m_instance = this;
            this.Load += new EventHandler(FormFrameHelp_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrameHelp_FormClosing);

        }

        protected override void CloseButtonClicked()
        {
            this.Visible = false;
        }


        private void FormFrameHelp_Load(object sender, EventArgs e)
        {
            this.TitleBarHeight = 30;
            this.SystemButtonSize = new Size(30, 24);

            this.TitleBarBackColor = Color.FromArgb(60, 56, 71);
            this.LBEdgeBackColor = Color.FromArgb(60, 56, 71);
            this.RBEdgeBackColor = Color.FromArgb(60, 56, 71);
            this.LeftEdgeBackColor = Color.FromArgb(60, 56, 71);
            this.RightEdgeBackColor = Color.FromArgb(60, 56, 71);
            this.BottomEdgeBackColor = Color.FromArgb(60, 56, 71);

            //폰트설정
            this.TitleTextFont = new Font("맑은 고딕", 9, FontStyle.Bold);
            this.TitlePosition = new Point(10, 10);
            this.TitleTextColor = Color.White;


            this.ShowPictureBoxTitle = false;
            this.ShowCloseButton = true;
            this.ShowMaxButton = false;
            this.ShowMinButton = false;

            this.CloseButtonImage = global::UnE.SenarioMaker.Properties.Resources.CloseWindow_Normal;

            this.ResizeFrame();            

            this.m_frmMain.Visible = true;            
        }

        private void FormFrameHelp_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
