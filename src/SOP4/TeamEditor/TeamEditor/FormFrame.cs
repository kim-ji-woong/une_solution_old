using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor
{
    public partial class FormFrame : UnE.GUI.FormNoFrameSizable
    {
        private static FormFrame m_instance = null;
        public static FormFrame Instance
        {
            get { return m_instance; }
        }

        private string m_strAppName = "조직관리툴 v2.0";
        public string AppName
        {
            get { return m_strAppName; }
        }

        public FormFrame(Form frmMain)
            : base(frmMain)
        {
            m_instance = this;
            InitializeComponent();
        }

        private void FormFrame_Load(object sender, EventArgs e)
        {
            this.m_frmMain.Visible = true;

            this.TitleBarHeight = 30;
            this.SystemButtonSize = new Size(30, 24);


            this.LBEdgeBackColor = Color.FromArgb(75, 71, 86);
            this.RBEdgeBackColor = Color.FromArgb(75, 71, 86);
            this.LeftEdgeBackColor = Color.FromArgb(75, 71, 86);
            this.RightEdgeBackColor = Color.FromArgb(75, 71, 86);
            this.BottomEdgeBackColor = Color.FromArgb(75, 71, 86);
            //폰트설정
            this.TitleTextFont = new Font("맑은 고딕", 9, FontStyle.Bold);
            this.TitlePosition = new Point(35, 8);
            this.Text = m_strAppName;

            this.ShowPictureBoxTitle = true;
            this.PictureBoxSize = new Size(30, 30);
            //this.PictureBoxTitleImage = global::RoadMan.Properties.Resources.RoadMan_Icon;

            this.pictureBoxTitle.Image = global::TeamEditor.Properties.Resources.teamEdit_logo_small;
            this.CloseButtonImage = global::TeamEditor.Properties.Resources.CloseWindow_Normal;
            this.MaxButtonImage = global::TeamEditor.Properties.Resources.MaxWindow_Normal;
            this.NormalButtonImage = global::TeamEditor.Properties.Resources.NormalWindow_Normal;

            this.MinButtonImage = global::TeamEditor.Properties.Resources.HideWindow_Normal;

            this.WindowState = FormWindowState.Maximized;
            this.ResizeFrame();
        }

        protected override void OnFormResize(object sender, EventArgs e)
        {
            base.OnFormResize(sender, e);
        }

        private void FormFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_frmMain != null)
            {
                m_frmMain.Close();

                if (m_frmMain.DialogResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private const int WM_CLOSE = 0x0010;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CLOSE:
                    FormMain.Instance.CloseApplication = true;
                    break;
            }

            base.WndProc(ref m);
        }
    }
}
