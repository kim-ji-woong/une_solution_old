namespace SOPManager
{
	partial class FormMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelForm = new System.Windows.Forms.Panel();
            this.panelSection = new System.Windows.Forms.Panel();
            this.panelSectionContent = new System.Windows.Forms.Panel();
            this.m_tmrCmdUpdate = new System.Windows.Forms.Timer(this.components);
            this.panelStatus = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mStatusWork = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStatusClock = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStatsCaps = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStatusNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStatusHanguel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStatusProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelContent.SuspendLayout();
            this.panelSection.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panelContent.Controls.Add(this.panelForm);
            this.panelContent.Location = new System.Drawing.Point(51, 148);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(694, 603);
            this.panelContent.TabIndex = 12;
            // 
            // panelForm
            // 
            this.panelForm.BackColor = System.Drawing.Color.White;
            this.panelForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelForm.Location = new System.Drawing.Point(0, 0);
            this.panelForm.Name = "panelForm";
            this.panelForm.Size = new System.Drawing.Size(694, 603);
            this.panelForm.TabIndex = 13;
            // 
            // panelSection
            // 
            this.panelSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panelSection.Controls.Add(this.panelSectionContent);
            this.panelSection.Location = new System.Drawing.Point(1007, 100);
            this.panelSection.Name = "panelSection";
            this.panelSection.Size = new System.Drawing.Size(379, 458);
            this.panelSection.TabIndex = 13;
            // 
            // panelSectionContent
            // 
            this.panelSectionContent.BackColor = System.Drawing.SystemColors.Control;
            this.panelSectionContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSectionContent.Location = new System.Drawing.Point(0, 0);
            this.panelSectionContent.Name = "panelSectionContent";
            this.panelSectionContent.Size = new System.Drawing.Size(379, 458);
            this.panelSectionContent.TabIndex = 1;
            // 
            // m_tmrCmdUpdate
            // 
            this.m_tmrCmdUpdate.Interval = 300;
            this.m_tmrCmdUpdate.Tick += new System.EventHandler(this.m_tmrCmdUpdate_Tick);
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panelStatus.BackgroundImage = global::SOPManager.Properties.Resources.Background;
            this.panelStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelStatus.Controls.Add(this.statusStrip1);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatus.Location = new System.Drawing.Point(0, 761);
            this.panelStatus.Margin = new System.Windows.Forms.Padding(0);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(1386, 27);
            this.panelStatus.TabIndex = 10;
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.BackColor = System.Drawing.Color.Transparent;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mStatusWork,
            this.mStatusClock,
            this.mStatsCaps,
            this.mStatusNum,
            this.mStatusHanguel,
            this.toolStripStatusLabel4,
            this.mStatusProgress,
            this.toolStripStatusLabel5});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1386, 27);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // mStatusWork
            // 
            this.mStatusWork.BackColor = System.Drawing.Color.Transparent;
            this.mStatusWork.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.mStatusWork.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mStatusWork.ForeColor = System.Drawing.Color.White;
            this.mStatusWork.Name = "mStatusWork";
            this.mStatusWork.Size = new System.Drawing.Size(843, 22);
            this.mStatusWork.Spring = true;
            this.mStatusWork.Text = "현재 작업을 표시합니다.";
            this.mStatusWork.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mStatusWork.ToolTipText = "현재 작업을 표시합니다.";
            // 
            // mStatusClock
            // 
            this.mStatusClock.BackColor = System.Drawing.Color.Transparent;
            this.mStatusClock.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.mStatusClock.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.mStatusClock.ForeColor = System.Drawing.Color.White;
            this.mStatusClock.Name = "mStatusClock";
            this.mStatusClock.Size = new System.Drawing.Size(187, 22);
            this.mStatusClock.Text = "현재시간                                ";
            // 
            // mStatsCaps
            // 
            this.mStatsCaps.AutoSize = false;
            this.mStatsCaps.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.mStatsCaps.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.mStatsCaps.ForeColor = System.Drawing.Color.White;
            this.mStatsCaps.Name = "mStatsCaps";
            this.mStatsCaps.Size = new System.Drawing.Size(41, 22);
            this.mStatsCaps.Text = "CAPS";
            // 
            // mStatusNum
            // 
            this.mStatusNum.AutoSize = false;
            this.mStatusNum.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.mStatusNum.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.mStatusNum.ForeColor = System.Drawing.Color.White;
            this.mStatusNum.Name = "mStatusNum";
            this.mStatusNum.Size = new System.Drawing.Size(39, 22);
            this.mStatusNum.Text = "NUM";
            // 
            // mStatusHanguel
            // 
            this.mStatusHanguel.AutoSize = false;
            this.mStatusHanguel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.mStatusHanguel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.mStatusHanguel.ForeColor = System.Drawing.Color.White;
            this.mStatusHanguel.Name = "mStatusHanguel";
            this.mStatusHanguel.Size = new System.Drawing.Size(40, 22);
            this.mStatusHanguel.Text = "한/영";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.AutoSize = false;
            this.toolStripStatusLabel4.ForeColor = System.Drawing.Color.White;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(55, 22);
            this.toolStripStatusLabel4.Text = "진행사항";
            // 
            // mStatusProgress
            // 
            this.mStatusProgress.AutoSize = false;
            this.mStatusProgress.Name = "mStatusProgress";
            this.mStatusProgress.Size = new System.Drawing.Size(100, 21);
            this.mStatusProgress.ToolTipText = "I/O 작업의 진행율을 표시합니다.";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.AutoSize = false;
            this.toolStripStatusLabel5.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabel5.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripStatusLabel5.ForeColor = System.Drawing.Color.White;
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(64, 22);
            this.toolStripStatusLabel5.Text = "U&&E";
            // 
            // panelTop
            // 
            this.panelTop.BackgroundImage = global::SOPManager.Properties.Resources.TitleBar_background;
            this.panelTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1386, 115);
            this.panelTop.TabIndex = 8;
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(1386, 788);
            this.ControlBox = false;
            this.Controls.Add(this.panelSection);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.panelTop);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "시나리오 생성기   v 2.0";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
            this.panelContent.ResumeLayout(false);
            this.panelSection.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelStatus;
		private System.Windows.Forms.Panel panelContent;
		private System.Windows.Forms.Panel panelSection;
		private System.Windows.Forms.Panel panelForm;
		private System.Windows.Forms.Panel panelSectionContent;
		private System.Windows.Forms.Timer m_tmrCmdUpdate;
		private System.Windows.Forms.StatusStrip statusStrip1;
		internal System.Windows.Forms.ToolStripStatusLabel mStatusWork;
		private System.Windows.Forms.ToolStripStatusLabel mStatusClock;
		internal System.Windows.Forms.ToolStripStatusLabel mStatsCaps;
		internal System.Windows.Forms.ToolStripStatusLabel mStatusNum;
		internal System.Windows.Forms.ToolStripStatusLabel mStatusHanguel;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
		private System.Windows.Forms.ToolStripProgressBar mStatusProgress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
	}
}