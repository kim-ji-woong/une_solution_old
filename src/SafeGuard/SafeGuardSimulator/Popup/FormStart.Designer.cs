namespace SOPManager
{
	partial class FormStart
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStart));
			this.panelMain = new System.Windows.Forms.Panel();
			this.panelContent = new System.Windows.Forms.Panel();
			this.btnConSetting = new System.Windows.Forms.Button();
			this.btnCanel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbOpenXML = new System.Windows.Forms.RadioButton();
			this.rbOpenSOP = new System.Windows.Forms.RadioButton();
			this.rbNewSOP = new System.Windows.Forms.RadioButton();
			this.panelMain.SuspendLayout();
			this.panelContent.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.Controls.Add(this.panelContent);
			this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMain.Location = new System.Drawing.Point(0, 0);
			this.panelMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(554, 281);
			this.panelMain.TabIndex = 0;
			// 
			// panelContent
			// 
			this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.panelContent.Controls.Add(this.btnConSetting);
			this.panelContent.Controls.Add(this.btnCanel);
			this.panelContent.Controls.Add(this.btnOK);
			this.panelContent.Controls.Add(this.groupBox1);
			this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelContent.Location = new System.Drawing.Point(0, 0);
			this.panelContent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panelContent.Name = "panelContent";
			this.panelContent.Size = new System.Drawing.Size(554, 281);
			this.panelContent.TabIndex = 1;
			// 
			// btnConSetting
			// 
			this.btnConSetting.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.btnConSetting.Location = new System.Drawing.Point(20, 234);
			this.btnConSetting.Name = "btnConSetting";
			this.btnConSetting.Size = new System.Drawing.Size(101, 28);
			this.btnConSetting.TabIndex = 3;
			this.btnConSetting.Text = "연결 설정";
			this.btnConSetting.UseVisualStyleBackColor = true;
			this.btnConSetting.Visible = false;
			this.btnConSetting.Click += new System.EventHandler(this.btnConSetting_Click);
			// 
			// btnCanel
			// 
			this.btnCanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
			this.btnCanel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCanel.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnCanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.btnCanel.Location = new System.Drawing.Point(429, 231);
			this.btnCanel.Name = "btnCanel";
			this.btnCanel.Size = new System.Drawing.Size(101, 31);
			this.btnCanel.TabIndex = 2;
			this.btnCanel.Text = "닫기";
			this.btnCanel.UseVisualStyleBackColor = false;
			this.btnCanel.Click += new System.EventHandler(this.btnCanel_Click);
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.btnOK.Location = new System.Drawing.Point(322, 231);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(101, 31);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "확인";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rbOpenXML);
			this.groupBox1.Controls.Add(this.rbOpenSOP);
			this.groupBox1.Controls.Add(this.rbNewSOP);
			this.groupBox1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.groupBox1.Location = new System.Drawing.Point(20, 19);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(510, 170);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "시작하기";
			// 
			// rbOpenXML
			// 
			this.rbOpenXML.AutoSize = true;
			this.rbOpenXML.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.rbOpenXML.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.rbOpenXML.Location = new System.Drawing.Point(39, 117);
			this.rbOpenXML.Name = "rbOpenXML";
			this.rbOpenXML.Size = new System.Drawing.Size(335, 21);
			this.rbOpenXML.TabIndex = 2;
			this.rbOpenXML.Text = "XML 불러오기 - SOP XML문서를 열기로 시작합니다.";
			this.rbOpenXML.UseVisualStyleBackColor = true;
			this.rbOpenXML.CheckedChanged += new System.EventHandler(this.rbOpenXML_CheckedChanged);
			// 
			// rbOpenSOP
			// 
			this.rbOpenSOP.AutoSize = true;
			this.rbOpenSOP.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.rbOpenSOP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.rbOpenSOP.Location = new System.Drawing.Point(39, 80);
			this.rbOpenSOP.Name = "rbOpenSOP";
			this.rbOpenSOP.Size = new System.Drawing.Size(386, 21);
			this.rbOpenSOP.TabIndex = 1;
			this.rbOpenSOP.Text = "기존 SOP 불러오기 - 이미 작성된 SOP를 DB에서 불러옵니다.";
			this.rbOpenSOP.UseVisualStyleBackColor = true;
			this.rbOpenSOP.CheckedChanged += new System.EventHandler(this.rbOpenSOP_CheckedChanged);
			// 
			// rbNewSOP
			// 
			this.rbNewSOP.AutoSize = true;
			this.rbNewSOP.Checked = true;
			this.rbNewSOP.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.rbNewSOP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.rbNewSOP.Location = new System.Drawing.Point(39, 43);
			this.rbNewSOP.Name = "rbNewSOP";
			this.rbNewSOP.Size = new System.Drawing.Size(299, 21);
			this.rbNewSOP.TabIndex = 0;
			this.rbNewSOP.TabStop = true;
			this.rbNewSOP.Text = "새 SOP 시작하기 -  새로운 SOP를 생성합니다.";
			this.rbNewSOP.UseVisualStyleBackColor = true;
			this.rbNewSOP.CheckedChanged += new System.EventHandler(this.rbNewSOP_CheckedChanged);
			// 
			// FormStart
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(554, 281);
			this.ControlBox = false;
			this.Controls.Add(this.panelMain);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormStart";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "새로운 시작";
			this.TopMost = true;
			this.panelMain.ResumeLayout(false);
			this.panelContent.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.Panel panelContent;
		private System.Windows.Forms.Button btnConSetting;
		private System.Windows.Forms.Button btnCanel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rbOpenXML;
		private System.Windows.Forms.RadioButton rbOpenSOP;
		private System.Windows.Forms.RadioButton rbNewSOP;
	}
}