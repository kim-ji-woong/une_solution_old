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
            this.mBtnViewCCTV = new System.Windows.Forms.Button();
            this.mBtnReportMalfunction = new System.Windows.Forms.Button();
            this.mBtnReportFire = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.btnSound = new System.Windows.Forms.Button();
            this.labelHeader = new System.Windows.Forms.Label();
            this.labelDetectCount = new System.Windows.Forms.Label();
            this.btnStartSOP = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mBtnViewCCTV
            // 
            this.mBtnViewCCTV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mBtnViewCCTV.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnViewCCTV.Location = new System.Drawing.Point(13, 1);
            this.mBtnViewCCTV.Name = "mBtnViewCCTV";
            this.mBtnViewCCTV.Size = new System.Drawing.Size(109, 29);
            this.mBtnViewCCTV.TabIndex = 0;
            this.mBtnViewCCTV.Text = "CCTV 보기";
            this.mBtnViewCCTV.UseVisualStyleBackColor = true;
            this.mBtnViewCCTV.Click += new System.EventHandler(this.BtnViewCCTV_Click);
            // 
            // mBtnReportMalfunction
            // 
            this.mBtnReportMalfunction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.mBtnReportMalfunction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mBtnReportMalfunction.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnReportMalfunction.Location = new System.Drawing.Point(243, 1);
            this.mBtnReportMalfunction.Name = "mBtnReportMalfunction";
            this.mBtnReportMalfunction.Size = new System.Drawing.Size(109, 29);
            this.mBtnReportMalfunction.TabIndex = 1;
            this.mBtnReportMalfunction.Text = "오작동/복구";
            this.mBtnReportMalfunction.UseVisualStyleBackColor = false;
            this.mBtnReportMalfunction.Click += new System.EventHandler(this.BtnReportMalfunction_Click);
            // 
            // mBtnReportFire
            // 
            this.mBtnReportFire.BackColor = System.Drawing.Color.Red;
            this.mBtnReportFire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mBtnReportFire.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnReportFire.Location = new System.Drawing.Point(128, 1);
            this.mBtnReportFire.Name = "mBtnReportFire";
            this.mBtnReportFire.Size = new System.Drawing.Size(109, 29);
            this.mBtnReportFire.TabIndex = 2;
            this.mBtnReportFire.Text = "화재 전파";
            this.mBtnReportFire.UseVisualStyleBackColor = false;
            this.mBtnReportFire.Click += new System.EventHandler(this.BtnReportFire_Click);
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
            this.button1.Location = new System.Drawing.Point(630, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 29);
            this.button1.TabIndex = 3;
            this.button1.Text = "[훈련상황] 화재신고";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSound
            // 
            this.btnSound.BackColor = System.Drawing.Color.White;
            this.btnSound.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSound.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSound.Location = new System.Drawing.Point(358, 1);
            this.btnSound.Name = "btnSound";
            this.btnSound.Size = new System.Drawing.Size(109, 29);
            this.btnSound.TabIndex = 2;
            this.btnSound.Text = "소리 켜짐";
            this.btnSound.UseVisualStyleBackColor = false;
            this.btnSound.Click += new System.EventHandler(this.BtnSoundOnOff_Click);
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.Location = new System.Drawing.Point(787, 9);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(105, 12);
            this.labelHeader.TabIndex = 4;
            this.labelHeader.Text = "미복구 센서신호 : ";
            this.labelHeader.Visible = false;
            // 
            // labelDetectCount
            // 
            this.labelDetectCount.AutoSize = true;
            this.labelDetectCount.Location = new System.Drawing.Point(893, 9);
            this.labelDetectCount.Name = "labelDetectCount";
            this.labelDetectCount.Size = new System.Drawing.Size(23, 12);
            this.labelDetectCount.TabIndex = 5;
            this.labelDetectCount.Text = "0개";
            this.labelDetectCount.Visible = false;
            // 
            // btnStartSOP
            // 
            this.btnStartSOP.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnStartSOP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartSOP.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnStartSOP.Location = new System.Drawing.Point(473, 1);
            this.btnStartSOP.Name = "btnStartSOP";
            this.btnStartSOP.Size = new System.Drawing.Size(151, 29);
            this.btnStartSOP.TabIndex = 6;
            this.btnStartSOP.Text = "경보에 따른 SOP가동";
            this.btnStartSOP.UseVisualStyleBackColor = false;
            this.btnStartSOP.Click += new System.EventHandler(this.btnStartSOP_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(13, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(897, 24);
            this.panel1.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(316, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "전화번호";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(253, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "전화번호";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(88, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "담당 부서  ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "담당 부서  ";
            // 
            // DlgSelectCase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(925, 72);
            this.ControlBox = false;
            this.Controls.Add(this.btnStartSOP);
            this.Controls.Add(this.labelDetectCount);
            this.Controls.Add(this.labelHeader);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSound);
            this.Controls.Add(this.mBtnReportFire);
            this.Controls.Add(this.mBtnReportMalfunction);
            this.Controls.Add(this.mBtnViewCCTV);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "DlgSelectCase";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "화재 탐지신호 수신";
            this.Load += new System.EventHandler(this.DlgSelectCase_Load);
            this.VisibleChanged += new System.EventHandler(this.DlgSelectCase_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button mBtnViewCCTV;
		private System.Windows.Forms.Button mBtnReportMalfunction;
        private System.Windows.Forms.Button mBtnReportFire;
        private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSound;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.Label labelDetectCount;
        private System.Windows.Forms.Button btnStartSOP;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
	}
}