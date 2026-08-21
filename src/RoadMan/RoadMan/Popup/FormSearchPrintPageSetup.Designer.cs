namespace UnE.Utility.Print
{
	partial class FormGridPrintPageSetup
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGridPrintPageSetup));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cmbPageList = new System.Windows.Forms.ComboBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.marginTop = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.marginBottom = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.lbUnit2 = new System.Windows.Forms.Label();
			this.lbUnit1 = new System.Windows.Forms.Label();
			this.marginLeft = new System.Windows.Forms.TextBox();
			this.marginRight = new System.Windows.Forms.TextBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.pictureBoxPrintDirection = new System.Windows.Forms.PictureBox();
			this.radioHorzPrint = new System.Windows.Forms.RadioButton();
			this.radioVertPrint = new System.Windows.Forms.RadioButton();
			this.btnPreview = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
			this.ckbShoweader = new System.Windows.Forms.CheckBox();
			this.editHeader = new System.Windows.Forms.TextBox();
			this.ckbShowDate = new System.Windows.Forms.CheckBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrintDirection)).BeginInit();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cmbPageList);
			this.groupBox1.Location = new System.Drawing.Point(13, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(330, 69);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "용지 설정";
			// 
			// cmbPageList
			// 
			this.cmbPageList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPageList.FormattingEnabled = true;
			this.cmbPageList.Location = new System.Drawing.Point(33, 28);
			this.cmbPageList.Name = "cmbPageList";
			this.cmbPageList.Size = new System.Drawing.Size(274, 20);
			this.cmbPageList.TabIndex = 0;
			this.cmbPageList.SelectedIndexChanged += new System.EventHandler(this.cmbPageList_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.marginTop);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.marginBottom);
			this.groupBox3.Controls.Add(this.pictureBox1);
			this.groupBox3.Controls.Add(this.lbUnit2);
			this.groupBox3.Controls.Add(this.lbUnit1);
			this.groupBox3.Controls.Add(this.marginLeft);
			this.groupBox3.Controls.Add(this.marginRight);
			this.groupBox3.Location = new System.Drawing.Point(13, 93);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(330, 201);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "문서 여백";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(182, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(27, 12);
			this.label2.TabIndex = 10;
			this.label2.Text = "mm";
			// 
			// marginTop
			// 
			this.marginTop.Location = new System.Drawing.Point(129, 20);
			this.marginTop.Name = "marginTop";
			this.marginTop.Size = new System.Drawing.Size(49, 21);
			this.marginTop.TabIndex = 9;
			this.marginTop.Text = "20";
			this.marginTop.TextChanged += new System.EventHandler(this.marginTop_TextChanged);
			this.marginTop.Leave += new System.EventHandler(this.marginTop_Leave);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(182, 171);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27, 12);
			this.label1.TabIndex = 8;
			this.label1.Text = "mm";
			// 
			// marginBottom
			// 
			this.marginBottom.Location = new System.Drawing.Point(129, 167);
			this.marginBottom.Name = "marginBottom";
			this.marginBottom.Size = new System.Drawing.Size(49, 21);
			this.marginBottom.TabIndex = 7;
			this.marginBottom.Text = "20";
			this.marginBottom.TextChanged += new System.EventHandler(this.marginBottom_TextChanged);
			this.marginBottom.Leave += new System.EventHandler(this.marginBottom_Leave);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox1.BackgroundImage = global::RoadMan.Properties.Resources._64Vertical_normal;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureBox1.Location = new System.Drawing.Point(102, 49);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(117, 110);
			this.pictureBox1.TabIndex = 6;
			this.pictureBox1.TabStop = false;
			// 
			// lbUnit2
			// 
			this.lbUnit2.AutoSize = true;
			this.lbUnit2.Location = new System.Drawing.Point(72, 98);
			this.lbUnit2.Name = "lbUnit2";
			this.lbUnit2.Size = new System.Drawing.Size(27, 12);
			this.lbUnit2.TabIndex = 5;
			this.lbUnit2.Text = "mm";
			// 
			// lbUnit1
			// 
			this.lbUnit1.AutoSize = true;
			this.lbUnit1.Location = new System.Drawing.Point(286, 98);
			this.lbUnit1.Name = "lbUnit1";
			this.lbUnit1.Size = new System.Drawing.Size(27, 12);
			this.lbUnit1.TabIndex = 4;
			this.lbUnit1.Text = "mm";
			// 
			// marginLeft
			// 
			this.marginLeft.Location = new System.Drawing.Point(14, 95);
			this.marginLeft.Name = "marginLeft";
			this.marginLeft.Size = new System.Drawing.Size(54, 21);
			this.marginLeft.TabIndex = 1;
			this.marginLeft.Text = "20";
			this.marginLeft.TextChanged += new System.EventHandler(this.marginLeft_TextChanged);
			this.marginLeft.Leave += new System.EventHandler(this.marginLeft_Leave);
			// 
			// marginRight
			// 
			this.marginRight.Location = new System.Drawing.Point(232, 95);
			this.marginRight.Name = "marginRight";
			this.marginRight.Size = new System.Drawing.Size(49, 21);
			this.marginRight.TabIndex = 0;
			this.marginRight.Text = "20";
			this.marginRight.TextChanged += new System.EventHandler(this.marginRight_TextChanged);
			this.marginRight.Leave += new System.EventHandler(this.marginRight_Leave);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.pictureBoxPrintDirection);
			this.groupBox4.Controls.Add(this.radioHorzPrint);
			this.groupBox4.Controls.Add(this.radioVertPrint);
			this.groupBox4.Location = new System.Drawing.Point(362, 14);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(200, 90);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "인쇄 방향";
			// 
			// pictureBoxPrintDirection
			// 
			this.pictureBoxPrintDirection.BackColor = System.Drawing.Color.White;
			this.pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Vertical_normal;
			this.pictureBoxPrintDirection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureBoxPrintDirection.Location = new System.Drawing.Point(129, 23);
			this.pictureBoxPrintDirection.Name = "pictureBoxPrintDirection";
			this.pictureBoxPrintDirection.Size = new System.Drawing.Size(51, 51);
			this.pictureBoxPrintDirection.TabIndex = 3;
			this.pictureBoxPrintDirection.TabStop = false;
			// 
			// radioHorzPrint
			// 
			this.radioHorzPrint.AutoSize = true;
			this.radioHorzPrint.Checked = true;
			this.radioHorzPrint.Location = new System.Drawing.Point(18, 53);
			this.radioHorzPrint.Name = "radioHorzPrint";
			this.radioHorzPrint.Size = new System.Drawing.Size(75, 16);
			this.radioHorzPrint.TabIndex = 1;
			this.radioHorzPrint.TabStop = true;
			this.radioHorzPrint.Text = "가로 방향";
			this.radioHorzPrint.UseVisualStyleBackColor = true;
			this.radioHorzPrint.CheckedChanged += new System.EventHandler(this.radioHorzPrint_CheckedChanged);
			// 
			// radioVertPrint
			// 
			this.radioVertPrint.AutoSize = true;
			this.radioVertPrint.Location = new System.Drawing.Point(18, 27);
			this.radioVertPrint.Name = "radioVertPrint";
			this.radioVertPrint.Size = new System.Drawing.Size(75, 16);
			this.radioVertPrint.TabIndex = 0;
			this.radioVertPrint.Text = "세로 방향";
			this.radioVertPrint.UseVisualStyleBackColor = true;
			this.radioVertPrint.CheckedChanged += new System.EventHandler(this.radioVertPrint_CheckedChanged);
			// 
			// btnPreview
			// 
			this.btnPreview.Location = new System.Drawing.Point(21, 312);
			this.btnPreview.Name = "btnPreview";
			this.btnPreview.Size = new System.Drawing.Size(106, 27);
			this.btnPreview.TabIndex = 5;
			this.btnPreview.Text = "인쇄 미리보기";
			this.btnPreview.UseVisualStyleBackColor = true;
			this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(351, 312);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(99, 28);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "확인";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(456, 312);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(99, 28);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "취소";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// ckbShoweader
			// 
			this.ckbShoweader.AutoSize = true;
			this.ckbShoweader.Location = new System.Drawing.Point(18, 34);
			this.ckbShoweader.Name = "ckbShoweader";
			this.ckbShoweader.Size = new System.Drawing.Size(76, 16);
			this.ckbShoweader.TabIndex = 0;
			this.ckbShoweader.Text = "헤더 출력";
			this.ckbShoweader.UseVisualStyleBackColor = true;
			this.ckbShoweader.CheckedChanged += new System.EventHandler(this.ckbShoweader_CheckedChanged);
			// 
			// editHeader
			// 
			this.editHeader.Enabled = false;
			this.editHeader.Location = new System.Drawing.Point(18, 65);
			this.editHeader.Name = "editHeader";
			this.editHeader.Size = new System.Drawing.Size(163, 21);
			this.editHeader.TabIndex = 3;
			this.editHeader.Text = "고령군청";
			this.editHeader.TextChanged += new System.EventHandler(this.editHeader_TextChanged);
			this.editHeader.Leave += new System.EventHandler(this.editHeader_Leave);
			// 
			// ckbShowDate
			// 
			this.ckbShowDate.AutoSize = true;
			this.ckbShowDate.Location = new System.Drawing.Point(18, 119);
			this.ckbShowDate.Name = "ckbShowDate";
			this.ckbShowDate.Size = new System.Drawing.Size(100, 16);
			this.ckbShowDate.TabIndex = 5;
			this.ckbShowDate.Text = "출력일자 표시";
			this.ckbShowDate.UseVisualStyleBackColor = true;
			this.ckbShowDate.CheckedChanged += new System.EventHandler(this.ckbShowDate_CheckedChanged);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.ckbShowDate);
			this.groupBox5.Controls.Add(this.editHeader);
			this.groupBox5.Controls.Add(this.ckbShoweader);
			this.groupBox5.Location = new System.Drawing.Point(362, 117);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(200, 177);
			this.groupBox5.TabIndex = 4;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "헤더 옵션";
			// 
			// FormGridPrintPageSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(579, 357);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnPreview);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormGridPrintPageSetup";
			this.Text = "인쇄 설정";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPrintPageSetup_FormClosing);
			this.Load += new System.EventHandler(this.FormPrintPageSetup_Load);
			this.VisibleChanged += new System.EventHandler(this.FormPrintPageSetup_VisibleChanged);
			this.groupBox1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrintDirection)).EndInit();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox cmbPageList;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox marginRight;
		private System.Windows.Forms.Label lbUnit2;
		private System.Windows.Forms.Label lbUnit1;
		private System.Windows.Forms.TextBox marginLeft;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.PictureBox pictureBoxPrintDirection;
		private System.Windows.Forms.RadioButton radioHorzPrint;
		private System.Windows.Forms.RadioButton radioVertPrint;
		private System.Windows.Forms.Button btnPreview;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox marginTop;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox marginBottom;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.CheckBox ckbShoweader;
		private System.Windows.Forms.TextBox editHeader;
		private System.Windows.Forms.CheckBox ckbShowDate;
		private System.Windows.Forms.GroupBox groupBox5;
	}
}