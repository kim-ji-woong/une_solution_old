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
            this.SuspendLayout();
            // 
            // mBtnViewCCTV
            // 
            this.mBtnViewCCTV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mBtnViewCCTV.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnViewCCTV.Location = new System.Drawing.Point(13, 12);
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
            this.mBtnReportMalfunction.Location = new System.Drawing.Point(129, 12);
            this.mBtnReportMalfunction.Name = "mBtnReportMalfunction";
            this.mBtnReportMalfunction.Size = new System.Drawing.Size(109, 29);
            this.mBtnReportMalfunction.TabIndex = 1;
            this.mBtnReportMalfunction.Text = "오작동 신고";
            this.mBtnReportMalfunction.UseVisualStyleBackColor = false;
            this.mBtnReportMalfunction.Click += new System.EventHandler(this.BtnReportMalfunction_Click);
            // 
            // mBtnReportFire
            // 
            this.mBtnReportFire.BackColor = System.Drawing.Color.Red;
            this.mBtnReportFire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mBtnReportFire.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnReportFire.Location = new System.Drawing.Point(246, 12);
            this.mBtnReportFire.Name = "mBtnReportFire";
            this.mBtnReportFire.Size = new System.Drawing.Size(109, 29);
            this.mBtnReportFire.TabIndex = 2;
            this.mBtnReportFire.Text = "화재 신고";
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
            this.button1.Location = new System.Drawing.Point(476, 12);
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
            this.btnSound.Location = new System.Drawing.Point(361, 12);
            this.btnSound.Name = "btnSound";
            this.btnSound.Size = new System.Drawing.Size(109, 29);
            this.btnSound.TabIndex = 2;
            this.btnSound.Text = "소리 켜짐";
            this.btnSound.UseVisualStyleBackColor = false;
            this.btnSound.Click += new System.EventHandler(this.BtnSoundOnOff_Click);
            // 
            // DlgSelectCase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 66);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSound);
            this.Controls.Add(this.mBtnReportFire);
            this.Controls.Add(this.mBtnReportMalfunction);
            this.Controls.Add(this.mBtnViewCCTV);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "DlgSelectCase";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "화재 탐지신호 수신";
            this.Load += new System.EventHandler(this.DlgSelectCase_Load);            
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button mBtnViewCCTV;
		private System.Windows.Forms.Button mBtnReportMalfunction;
        private System.Windows.Forms.Button mBtnReportFire;
        private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSound;
	}
}