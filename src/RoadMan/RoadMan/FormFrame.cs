using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
    public partial class FormFrame : UnE.GUI.FormNoFrameSizable
    {
        private static FormFrame m_instance = null;
        public static FormFrame Instance
        {
            get { return m_instance; }
        }

        private string m_strAppName = "로드맨 v1.01";
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
            this.PictureBoxTitleImage = global::RoadMan.Properties.Resources.RoadMan_Icon;


            this.CloseButtonImage = global::RoadMan.Properties.Resources.CloseWindow_Normal;
            this.MaxButtonImage = global::RoadMan.Properties.Resources.MaxWindow_Normal;
            this.NormalButtonImage = global::RoadMan.Properties.Resources.NormalWindow_Normal;

            this.MinButtonImage = global::RoadMan.Properties.Resources.HideWindow_Normal;

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

		private void FormFrame_LocationChanged(object sender, EventArgs e)
		{
			if( m_frmMain != null)
			{
				FormMain.Instance.FormMain_LocationChanged(m_frmMain, e);
			}
		}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == WindowMessage.WM_KEYDOWN ||
                msg.Msg == WindowMessage.WM_CHAR ||
                msg.Msg == WindowMessage.WM_SYSKEYDOWN)
            {
                if (keyData == Keys.F1)
                {
                    FormMain.Instance.ShowHelp();
                    return true;
                }
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
