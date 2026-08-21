using System;
using System.Drawing;
using System.Windows.Forms;

namespace SOPManager
{
    public partial class FormFrameDialog : UnE.GUI.FormNoFrameSizableRibbon
	{
		public FormFrameDialog(Form frmHelp)
			: base(frmHelp)
		{
			InitializeComponent();

			this.Load += new EventHandler(FormFrameHelp_Load);
			this.FormClosing += new FormClosingEventHandler(FormFrameHelp_FormClosing);
		}

		protected override void CloseButtonClicked()
		{
			this.Close();
			//this.Visible = false;
		}

		private void FormFrameHelp_Load(object sender, EventArgs e)
		{
			this.TitleBarHeight = 30;
			this.SystemButtonSize = new Size(30, 24);

			Color boraderColor = Color.FromArgb(43, 43, 43);
			this.TitleBarBackColor = boraderColor;
			this.LBEdgeBackColor = boraderColor;
			this.RBEdgeBackColor = boraderColor;
			this.LeftEdgeBackColor = boraderColor;
			this.RightEdgeBackColor = boraderColor;
			this.BottomEdgeBackColor = boraderColor;
			this.PictureBoxTitle.BackColor = boraderColor;
			//폰트설정
			this.TitleTextFont = new Font("나눔스퀘어", 9, FontStyle.Bold);
			this.TitlePosition = new Point(5, 8);
			this.TitleTextColor = Color.White;

			this.PictureBoxTitle.Location = new Point(5, 5);
			this.ShowPictureBoxTitle = true;
			this.ShowCloseButton = true;
			this.ShowMaxButton = false;
			this.ShowMinButton = false;

            this.CloseButtonImage = global::SOPManager.Properties.Resources.Close_40_40_Default;
            this.CloseButtonOverImage = global::SOPManager.Properties.Resources.Close_40_40_Click;

			this.ResizeFrame();

			this.m_frmMain.Visible = true;
		}

		private void FormFrameHelp_FormClosing(object sender, FormClosingEventArgs e)
		{
			//e.Cancel = true;
			//return;
		}
	}
}