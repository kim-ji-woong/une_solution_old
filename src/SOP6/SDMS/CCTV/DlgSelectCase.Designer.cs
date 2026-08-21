namespace SDMS
{
	partial class DlgSelectCase
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.labelHeader = new System.Windows.Forms.Label();
            this.labelDetectCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mBtnViewCCTV = new UnE.GUI.ImageButton();
            this.mBtnReportFire = new UnE.GUI.ImageButton();
            this.mBtnReportMalfunction = new UnE.GUI.ImageButton();
            this.btnSound = new UnE.GUI.ImageButton();
            this.btnStartSOP = new UnE.GUI.ImageButton();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mBtnViewCCTV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBtnReportFire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBtnReportMalfunction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStartSOP)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.Location = new System.Drawing.Point(316, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(149, 27);
            this.button1.TabIndex = 3;
            this.button1.Text = "[훈련상황] 화재신고";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelHeader.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelHeader.Location = new System.Drawing.Point(168, 10);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(132, 15);
            this.labelHeader.TabIndex = 4;
            this.labelHeader.Text = "미복구 센서신호 : ";
            this.labelHeader.Visible = false;
            this.labelHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DlgSelectCase_MouseDown);
            this.labelHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DlgSelectCase_MouseUp);
            // 
            // labelDetectCount
            // 
            this.labelDetectCount.AutoSize = true;
            this.labelDetectCount.BackColor = System.Drawing.Color.Transparent;
            this.labelDetectCount.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDetectCount.Location = new System.Drawing.Point(281, 11);
            this.labelDetectCount.Name = "labelDetectCount";
            this.labelDetectCount.Size = new System.Drawing.Size(29, 14);
            this.labelDetectCount.TabIndex = 5;
            this.labelDetectCount.Text = "0개";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(292, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "전화번호";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(69, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "담당 부서  ";
            // 
            // mBtnViewCCTV
            // 
            this.mBtnViewCCTV.BackColor = System.Drawing.Color.Transparent;
            this.mBtnViewCCTV.ButtonText = "";
            this.mBtnViewCCTV.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnViewCCTV.ImageClicked = global::SDMS.Properties.Resources.DlgSelectCase_ViewCCTV_Click;
            this.mBtnViewCCTV.ImageDisabled = null;
            this.mBtnViewCCTV.ImageMouseOver = global::SDMS.Properties.Resources.DlgSelectCase_ViewCCTV_Click;
            this.mBtnViewCCTV.ImageNormal = global::SDMS.Properties.Resources.DlgSelectCase_ViewCCTV_Default;
            this.mBtnViewCCTV.Location = new System.Drawing.Point(14, 42);
            this.mBtnViewCCTV.Name = "mBtnViewCCTV";
            this.mBtnViewCCTV.Owner = null;
            this.mBtnViewCCTV.Size = new System.Drawing.Size(79, 33);
            this.mBtnViewCCTV.TabIndex = 8;
            this.mBtnViewCCTV.TabStop = false;
            this.mBtnViewCCTV.TextColor = System.Drawing.Color.Black;
            this.mBtnViewCCTV.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnViewCCTV.ToolTipText = "";
            this.mBtnViewCCTV.UseToolTip = false;
            this.mBtnViewCCTV.WindowRateWidth = 1F;
            this.mBtnViewCCTV.Click += new System.EventHandler(this.BtnViewCCTV_Click);
            // 
            // mBtnReportFire
            // 
            this.mBtnReportFire.BackColor = System.Drawing.Color.Transparent;
            this.mBtnReportFire.ButtonText = "";
            this.mBtnReportFire.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnReportFire.ImageClicked = global::SDMS.Properties.Resources.DlgSelectCase_ReportFire_Click;
            this.mBtnReportFire.ImageDisabled = null;
            this.mBtnReportFire.ImageMouseOver = global::SDMS.Properties.Resources.DlgSelectCase_ReportFire_Click;
            this.mBtnReportFire.ImageNormal = global::SDMS.Properties.Resources.DlgSelectCase_ReportFire_Default;
            this.mBtnReportFire.Location = new System.Drawing.Point(93, 42);
            this.mBtnReportFire.Name = "mBtnReportFire";
            this.mBtnReportFire.Owner = null;
            this.mBtnReportFire.Size = new System.Drawing.Size(79, 33);
            this.mBtnReportFire.TabIndex = 9;
            this.mBtnReportFire.TabStop = false;
            this.mBtnReportFire.TextColor = System.Drawing.Color.Black;
            this.mBtnReportFire.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnReportFire.ToolTipText = "";
            this.mBtnReportFire.UseToolTip = false;
            this.mBtnReportFire.WindowRateWidth = 1F;
            this.mBtnReportFire.Click += new System.EventHandler(this.BtnReportFire_Click);
            // 
            // mBtnReportMalfunction
            // 
            this.mBtnReportMalfunction.BackColor = System.Drawing.Color.Transparent;
            this.mBtnReportMalfunction.ButtonText = "";
            this.mBtnReportMalfunction.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnReportMalfunction.ImageClicked = global::SDMS.Properties.Resources.DlgSelectCase_Malfunction_Click;
            this.mBtnReportMalfunction.ImageDisabled = null;
            this.mBtnReportMalfunction.ImageMouseOver = global::SDMS.Properties.Resources.DlgSelectCase_Malfunction_Click;
            this.mBtnReportMalfunction.ImageNormal = global::SDMS.Properties.Resources.DlgSelectCase_Malfunction_Default;
            this.mBtnReportMalfunction.Location = new System.Drawing.Point(172, 42);
            this.mBtnReportMalfunction.Name = "mBtnReportMalfunction";
            this.mBtnReportMalfunction.Owner = null;
            this.mBtnReportMalfunction.Size = new System.Drawing.Size(79, 33);
            this.mBtnReportMalfunction.TabIndex = 10;
            this.mBtnReportMalfunction.TabStop = false;
            this.mBtnReportMalfunction.TextColor = System.Drawing.Color.Black;
            this.mBtnReportMalfunction.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnReportMalfunction.ToolTipText = "";
            this.mBtnReportMalfunction.UseToolTip = false;
            this.mBtnReportMalfunction.WindowRateWidth = 1F;
            this.mBtnReportMalfunction.Click += new System.EventHandler(this.BtnReportMalfunction_Click);
            // 
            // btnSound
            // 
            this.btnSound.BackColor = System.Drawing.Color.Transparent;
            this.btnSound.ButtonText = "";
            this.btnSound.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSound.ImageClicked = global::SDMS.Properties.Resources.DlgSelectCase_SoundOn_Click;
            this.btnSound.ImageDisabled = null;
            this.btnSound.ImageMouseOver = global::SDMS.Properties.Resources.DlgSelectCase_SoundOn_Click;
            this.btnSound.ImageNormal = global::SDMS.Properties.Resources.DlgSelectCase_SoundOn_Default;
            this.btnSound.Location = new System.Drawing.Point(251, 42);
            this.btnSound.Name = "btnSound";
            this.btnSound.Owner = null;
            this.btnSound.Size = new System.Drawing.Size(79, 33);
            this.btnSound.TabIndex = 11;
            this.btnSound.TabStop = false;
            this.btnSound.TextColor = System.Drawing.Color.Black;
            this.btnSound.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSound.ToolTipText = "";
            this.btnSound.UseToolTip = false;
            this.btnSound.WindowRateWidth = 1F;
            this.btnSound.Click += new System.EventHandler(this.BtnSoundOnOff_Click);
            // 
            // btnStartSOP
            // 
            this.btnStartSOP.BackColor = System.Drawing.Color.Transparent;
            this.btnStartSOP.ButtonText = "";
            this.btnStartSOP.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnStartSOP.ImageClicked = global::SDMS.Properties.Resources.DlgSelectCase_StartSOP_Click;
            this.btnStartSOP.ImageDisabled = null;
            this.btnStartSOP.ImageMouseOver = global::SDMS.Properties.Resources.DlgSelectCase_StartSOP_Click;
            this.btnStartSOP.ImageNormal = global::SDMS.Properties.Resources.DlgSelectCase_StartSOP_Default;
            this.btnStartSOP.Location = new System.Drawing.Point(330, 42);
            this.btnStartSOP.Name = "btnStartSOP";
            this.btnStartSOP.Owner = null;
            this.btnStartSOP.Size = new System.Drawing.Size(129, 33);
            this.btnStartSOP.TabIndex = 12;
            this.btnStartSOP.TabStop = false;
            this.btnStartSOP.TextColor = System.Drawing.Color.Black;
            this.btnStartSOP.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnStartSOP.ToolTipText = "";
            this.btnStartSOP.UseToolTip = false;
            this.btnStartSOP.WindowRateWidth = 1F;
            this.btnStartSOP.Click += new System.EventHandler(this.btnStartSOP_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(174, 18);
            this.lblTitle.TabIndex = 13;
            this.lblTitle.Text = "누출 탐지신호 수신";
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DlgSelectCase_MouseDown);
            this.lblTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DlgSelectCase_MouseUp);
            // 
            // DlgSelectCase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::SDMS.Properties.Resources.DlgSelectCase_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(467, 116);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnStartSOP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSound);
            this.Controls.Add(this.mBtnReportMalfunction);
            this.Controls.Add(this.mBtnReportFire);
            this.Controls.Add(this.mBtnViewCCTV);
            this.Controls.Add(this.labelDetectCount);
            this.Controls.Add(this.labelHeader);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.Name = "DlgSelectCase";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "화재 탐지신호 수신";
            this.Load += new System.EventHandler(this.DlgSelectCase_Load);
            this.VisibleChanged += new System.EventHandler(this.DlgSelectCase_VisibleChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DlgSelectCase_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DlgSelectCase_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.mBtnViewCCTV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBtnReportFire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBtnReportMalfunction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStartSOP)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.Label labelDetectCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private UnE.GUI.ImageButton mBtnViewCCTV;
        private UnE.GUI.ImageButton mBtnReportFire;
        private UnE.GUI.ImageButton mBtnReportMalfunction;
        private UnE.GUI.ImageButton btnSound;
        private UnE.GUI.ImageButton btnStartSOP;
        private System.Windows.Forms.Label lblTitle;
	}
}