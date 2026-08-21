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
    public partial class FormFrame : UnE.GUI.FormNoFrameSizableRibbon
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
            this.SystemButtonSize = new Size(20, 20);

            this.LBEdgeBackColor = Color.FromArgb(43, 43, 43);
            this.RBEdgeBackColor = Color.FromArgb(43, 43, 43);
            this.LeftEdgeBackColor = Color.FromArgb(43, 43, 43);
            this.RightEdgeBackColor = Color.FromArgb(43, 43, 43);
            this.BottomEdgeBackColor = Color.FromArgb(43, 43, 43);
            //폰트설정
            this.TitleTextFont = new Font("나눔스퀘어", 10f, FontStyle.Bold);
            this.TitlePosition = new Point(30, 5);
            this.Text = m_strAppName;

            this.ShowPictureBoxTitle = true;
            this.PictureBoxSize = new Size(30, 30);
            //this.PictureBoxTitleImage = global::RoadMan.Properties.Resources.RoadMan_Icon;

            this.pictureBoxTitle.Image = global::TeamEditor.Properties.Resources.teamEdit_logo_small;

            this.CloseButtonImage = global::TeamEditor.Properties.Resources.WindowClose;
            this.CloseButtonOverImage = global::TeamEditor.Properties.Resources.WindowClose_Click;

            this.MaxButtonImage = global::TeamEditor.Properties.Resources.WindowNormal;
            this.MaxButtonOverImage = global::TeamEditor.Properties.Resources.WindowNormal_Click;

            this.MinButtonImage = global::TeamEditor.Properties.Resources.WindowHide;
            this.MinButtonOverImage = global::TeamEditor.Properties.Resources.WindowHide_Click;

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
