namespace UnE.Utility.Print
{
	partial class FormPrintPageSetup
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrintPageSetup));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cmbPageList = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.mPreviewPane = new System.Windows.Forms.Panel();
			this.btnSelectWnd = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.cmbPrintArea = new System.Windows.Forms.ComboBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.ckbPageCenter = new System.Windows.Forms.CheckBox();
			this.lbUnit2 = new System.Windows.Forms.Label();
			this.lbUnit1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.editOffsetY = new System.Windows.Forms.TextBox();
			this.editOffsetX = new System.Windows.Forms.TextBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.pictureBoxPrintDirection = new System.Windows.Forms.PictureBox();
			this.ckbUpsideDown = new System.Windows.Forms.CheckBox();
			this.radioHorzPrint = new System.Windows.Forms.RadioButton();
			this.radioVertPrint = new System.Windows.Forms.RadioButton();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.cmbUnit = new System.Windows.Forms.ComboBox();
			this.editUnit = new System.Windows.Forms.TextBox();
			this.editLength = new System.Windows.Forms.TextBox();
			this.cmbScale = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.ckbFitPage = new System.Windows.Forms.CheckBox();
			this.btnPreview = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
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
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.mPreviewPane);
			this.groupBox2.Controls.Add(this.btnSelectWnd);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.cmbPrintArea);
			this.groupBox2.Location = new System.Drawing.Point(13, 93);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(330, 82);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "인쇄 영역";
			// 
			// mPreviewPane
			// 
			this.mPreviewPane.BackColor = System.Drawing.Color.Black;
			this.mPreviewPane.Location = new System.Drawing.Point(251, 24);
			this.mPreviewPane.Name = "mPreviewPane";
			this.mPreviewPane.Size = new System.Drawing.Size(66, 45);
			this.mPreviewPane.TabIndex = 3;
			this.mPreviewPane.Visible = false;
			this.mPreviewPane.Paint += new System.Windows.Forms.PaintEventHandler(this.mPreviewPane_Paint);
			// 
			// btnSelectWnd
			// 
			this.btnSelectWnd.Location = new System.Drawing.Point(154, 42);
			this.btnSelectWnd.Name = "btnSelectWnd";
			this.btnSelectWnd.Size = new System.Drawing.Size(77, 20);
			this.btnSelectWnd.TabIndex = 2;
			this.btnSelectWnd.Text = "< 화면선택";
			this.btnSelectWnd.UseVisualStyleBackColor = true;
			this.btnSelectWnd.Visible = false;
			this.btnSelectWnd.Click += new System.EventHandler(this.btnSelectWnd_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(31, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "인쇄 할 영역: ";
			// 
			// cmbPrintArea
			// 
			this.cmbPrintArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPrintArea.FormattingEnabled = true;
			this.cmbPrintArea.Items.AddRange(new object[] {
            "현재 화면",
            "윈도우 선택"});
			this.cmbPrintArea.Location = new System.Drawing.Point(36, 42);
			this.cmbPrintArea.Name = "cmbPrintArea";
			this.cmbPrintArea.Size = new System.Drawing.Size(106, 20);
			this.cmbPrintArea.TabIndex = 0;
			this.cmbPrintArea.SelectedIndexChanged += new System.EventHandler(this.cmbPrintArea_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.ckbPageCenter);
			this.groupBox3.Controls.Add(this.lbUnit2);
			this.groupBox3.Controls.Add(this.lbUnit1);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.editOffsetY);
			this.groupBox3.Controls.Add(this.editOffsetX);
			this.groupBox3.Location = new System.Drawing.Point(13, 190);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(330, 104);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "문서 오프셋";
			// 
			// ckbPageCenter
			// 
			this.ckbPageCenter.AutoSize = true;
			this.ckbPageCenter.Location = new System.Drawing.Point(215, 39);
			this.ckbPageCenter.Name = "ckbPageCenter";
			this.ckbPageCenter.Size = new System.Drawing.Size(88, 16);
			this.ckbPageCenter.TabIndex = 6;
			this.ckbPageCenter.Text = "용지 가운데";
			this.ckbPageCenter.UseVisualStyleBackColor = true;
			this.ckbPageCenter.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// lbUnit2
			// 
			this.lbUnit2.AutoSize = true;
			this.lbUnit2.Location = new System.Drawing.Point(165, 67);
			this.lbUnit2.Name = "lbUnit2";
			this.lbUnit2.Size = new System.Drawing.Size(27, 12);
			this.lbUnit2.TabIndex = 5;
			this.lbUnit2.Text = "mm";
			// 
			// lbUnit1
			// 
			this.lbUnit1.AutoSize = true;
			this.lbUnit1.Location = new System.Drawing.Point(165, 39);
			this.lbUnit1.Name = "lbUnit1";
			this.lbUnit1.Size = new System.Drawing.Size(27, 12);
			this.lbUnit1.TabIndex = 4;
			this.lbUnit1.Text = "mm";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(37, 67);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(17, 12);
			this.label3.TabIndex = 3;
			this.label3.Text = "Y:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(35, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "X:";
			// 
			// editOffsetY
			// 
			this.editOffsetY.Location = new System.Drawing.Point(58, 64);
			this.editOffsetY.Name = "editOffsetY";
			this.editOffsetY.Size = new System.Drawing.Size(100, 21);
			this.editOffsetY.TabIndex = 1;
			this.editOffsetY.TextChanged += new System.EventHandler(this.editOffsetY_TextChanged);
			this.editOffsetY.Leave += new System.EventHandler(this.editOffsetY_Leave);
			// 
			// editOffsetX
			// 
			this.editOffsetX.Location = new System.Drawing.Point(58, 36);
			this.editOffsetX.Name = "editOffsetX";
			this.editOffsetX.Size = new System.Drawing.Size(100, 21);
			this.editOffsetX.TabIndex = 0;
			this.editOffsetX.TextChanged += new System.EventHandler(this.editOffsetX_TextChanged);
			this.editOffsetX.Leave += new System.EventHandler(this.editOffsetX_Leave);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.pictureBoxPrintDirection);
			this.groupBox4.Controls.Add(this.ckbUpsideDown);
			this.groupBox4.Controls.Add(this.radioHorzPrint);
			this.groupBox4.Controls.Add(this.radioVertPrint);
			this.groupBox4.Location = new System.Drawing.Point(362, 14);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(200, 109);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "인쇄 방향";
			// 
			// pictureBoxPrintDirection
			// 
			this.pictureBoxPrintDirection.BackColor = System.Drawing.Color.White;
			this.pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Vertical_normal;
			this.pictureBoxPrintDirection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureBoxPrintDirection.Location = new System.Drawing.Point(123, 35);
			this.pictureBoxPrintDirection.Name = "pictureBoxPrintDirection";
			this.pictureBoxPrintDirection.Size = new System.Drawing.Size(51, 51);
			this.pictureBoxPrintDirection.TabIndex = 3;
			this.pictureBoxPrintDirection.TabStop = false;
			// 
			// ckbUpsideDown
			// 
			this.ckbUpsideDown.AutoSize = true;
			this.ckbUpsideDown.Location = new System.Drawing.Point(18, 79);
			this.ckbUpsideDown.Name = "ckbUpsideDown";
			this.ckbUpsideDown.Size = new System.Drawing.Size(82, 16);
			this.ckbUpsideDown.TabIndex = 2;
			this.ckbUpsideDown.Text = "상-하 반전";
			this.ckbUpsideDown.UseVisualStyleBackColor = true;
			this.ckbUpsideDown.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
			// 
			// radioHorzPrint
			// 
			this.radioHorzPrint.AutoSize = true;
			this.radioHorzPrint.Location = new System.Drawing.Point(18, 51);
			this.radioHorzPrint.Name = "radioHorzPrint";
			this.radioHorzPrint.Size = new System.Drawing.Size(75, 16);
			this.radioHorzPrint.TabIndex = 1;
			this.radioHorzPrint.Text = "가로 방향";
			this.radioHorzPrint.UseVisualStyleBackColor = true;
			this.radioHorzPrint.CheckedChanged += new System.EventHandler(this.radioHorzPrint_CheckedChanged);
			// 
			// radioVertPrint
			// 
			this.radioVertPrint.AutoSize = true;
			this.radioVertPrint.Checked = true;
			this.radioVertPrint.Location = new System.Drawing.Point(18, 27);
			this.radioVertPrint.Name = "radioVertPrint";
			this.radioVertPrint.Size = new System.Drawing.Size(75, 16);
			this.radioVertPrint.TabIndex = 0;
			this.radioVertPrint.TabStop = true;
			this.radioVertPrint.Text = "세로 방향";
			this.radioVertPrint.UseVisualStyleBackColor = true;
			this.radioVertPrint.CheckedChanged += new System.EventHandler(this.radioVertPrint_CheckedChanged);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.label4);
			this.groupBox5.Controls.Add(this.label7);
			this.groupBox5.Controls.Add(this.cmbUnit);
			this.groupBox5.Controls.Add(this.editUnit);
			this.groupBox5.Controls.Add(this.editLength);
			this.groupBox5.Controls.Add(this.cmbScale);
			this.groupBox5.Controls.Add(this.label6);
			this.groupBox5.Controls.Add(this.ckbFitPage);
			this.groupBox5.Location = new System.Drawing.Point(362, 138);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(200, 156);
			this.groupBox5.TabIndex = 4;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "출력 스케일";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(166, 93);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(11, 12);
			this.label4.TabIndex = 7;
			this.label4.Text = "=";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(102, 122);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(26, 12);
			this.label7.TabIndex = 6;
			this.label7.Text = "Unit";
			// 
			// cmbUnit
			// 
			this.cmbUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbUnit.FormattingEnabled = true;
			this.cmbUnit.Items.AddRange(new object[] {
            "mm",
            "inch"});
			this.cmbUnit.Location = new System.Drawing.Point(99, 90);
			this.cmbUnit.Name = "cmbUnit";
			this.cmbUnit.Size = new System.Drawing.Size(56, 20);
			this.cmbUnit.TabIndex = 5;
			this.cmbUnit.SelectedIndexChanged += new System.EventHandler(this.cmbUnit_SelectedIndexChanged);
			// 
			// editUnit
			// 
			this.editUnit.Location = new System.Drawing.Point(18, 116);
			this.editUnit.Name = "editUnit";
			this.editUnit.Size = new System.Drawing.Size(75, 21);
			this.editUnit.TabIndex = 4;
			this.editUnit.TextChanged += new System.EventHandler(this.editUnit_TextChanged);
			this.editUnit.Leave += new System.EventHandler(this.editUnit_Leave);
			// 
			// editLength
			// 
			this.editLength.Location = new System.Drawing.Point(18, 89);
			this.editLength.Name = "editLength";
			this.editLength.Size = new System.Drawing.Size(75, 21);
			this.editLength.TabIndex = 3;
			this.editLength.TextChanged += new System.EventHandler(this.editLength_TextChanged);
			this.editLength.Leave += new System.EventHandler(this.editLength_Leave);
			// 
			// cmbScale
			// 
			this.cmbScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbScale.FormattingEnabled = true;
			this.cmbScale.Items.AddRange(new object[] {
            "CUSTOM",
            "1:1",
            "1:2",
            "1:4",
            "1:5",
            "1:8",
            "1:10",
            "1:16",
            "1:20",
            "1:30",
            "1:40",
            "1:50",
            "1:100",
            "2:1",
            "4:1",
            "8:1",
            "10:1",
            "100:1"});
			this.cmbScale.Location = new System.Drawing.Point(63, 56);
			this.cmbScale.Name = "cmbScale";
			this.cmbScale.Size = new System.Drawing.Size(121, 20);
			this.cmbScale.TabIndex = 2;
			this.cmbScale.SelectedIndexChanged += new System.EventHandler(this.cmbScale_SelectedIndexChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(16, 59);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(41, 12);
			this.label6.TabIndex = 1;
			this.label6.Text = "스케일";
			// 
			// ckbFitPage
			// 
			this.ckbFitPage.AutoSize = true;
			this.ckbFitPage.Location = new System.Drawing.Point(16, 28);
			this.ckbFitPage.Name = "ckbFitPage";
			this.ckbFitPage.Size = new System.Drawing.Size(88, 16);
			this.ckbFitPage.TabIndex = 0;
			this.ckbFitPage.Text = "용지에 맞게";
			this.ckbFitPage.UseVisualStyleBackColor = true;
			this.ckbFitPage.CheckedChanged += new System.EventHandler(this.ckbFitPage_CheckedChanged);
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
			this.btnOK.Text = "인쇄하기";
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
			// FormPrintPageSetup
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
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPrintPageSetup";
			this.Text = "인쇄 설정";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPrintPageSetup_FormClosing);
			this.Load += new System.EventHandler(this.FormPrintPageSetup_Load);
			this.VisibleChanged += new System.EventHandler(this.FormPrintPageSetup_VisibleChanged);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
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
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cmbPrintArea;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox editOffsetX;
		private System.Windows.Forms.CheckBox ckbPageCenter;
		private System.Windows.Forms.Label lbUnit2;
		private System.Windows.Forms.Label lbUnit1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox editOffsetY;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.PictureBox pictureBoxPrintDirection;
		private System.Windows.Forms.CheckBox ckbUpsideDown;
		private System.Windows.Forms.RadioButton radioHorzPrint;
		private System.Windows.Forms.RadioButton radioVertPrint;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox cmbUnit;
		private System.Windows.Forms.TextBox editUnit;
		private System.Windows.Forms.TextBox editLength;
		private System.Windows.Forms.ComboBox cmbScale;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox ckbFitPage;
		private System.Windows.Forms.Button btnPreview;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
		private System.Windows.Forms.Button btnSelectWnd;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel mPreviewPane;
	}
}