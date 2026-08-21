namespace RoadMan
{
	partial class FormMemo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMemo));
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbFontList = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnOnOff = new System.Windows.Forms.CheckBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.btnStrong = new System.Windows.Forms.CheckBox();
            this.btnItalic = new System.Windows.Forms.CheckBox();
            this.btnUnderline = new System.Windows.Forms.CheckBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBoxTextColor = new System.Windows.Forms.PictureBox();
            this.pictureBoxLineColor = new System.Windows.Forms.PictureBox();
            this.btnText = new System.Windows.Forms.RadioButton();
            this.btnEllipse = new System.Windows.Forms.RadioButton();
            this.btnRect = new System.Windows.Forms.RadioButton();
            this.btnFreeDraw = new System.Windows.Forms.RadioButton();
            this.btnLine = new System.Windows.Forms.RadioButton();
            this.btnSelect = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTextColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLineColor)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1px",
            "3px",
            "5px",
            "8px"});
            this.comboBox1.Location = new System.Drawing.Point(385, 17);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(66, 23);
            this.comboBox1.TabIndex = 13;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(521, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 15);
            this.label2.TabIndex = 24;
            this.label2.Text = "문자";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(479, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 15);
            this.label1.TabIndex = 22;
            this.label1.Text = "선";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(82, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 15);
            this.label3.TabIndex = 30;
            this.label3.Text = "선택 편집";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.Gray;
            this.label4.Location = new System.Drawing.Point(262, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 15);
            this.label4.TabIndex = 31;
            this.label4.Text = "도구";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.Gray;
            this.label5.Location = new System.Drawing.Point(497, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 15);
            this.label5.TabIndex = 33;
            this.label5.Text = "색상";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.Gray;
            this.label6.Location = new System.Drawing.Point(403, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 15);
            this.label6.TabIndex = 35;
            this.label6.Text = "크기";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.Gray;
            this.label7.Location = new System.Drawing.Point(625, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 15);
            this.label7.TabIndex = 40;
            this.label7.Text = "글꼴";
            // 
            // cmbFontList
            // 
            this.cmbFontList.BackColor = System.Drawing.Color.White;
            this.cmbFontList.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbFontList.FormattingEnabled = true;
            this.cmbFontList.Location = new System.Drawing.Point(570, 11);
            this.cmbFontList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbFontList.Name = "cmbFontList";
            this.cmbFontList.Size = new System.Drawing.Size(142, 23);
            this.cmbFontList.TabIndex = 41;
            this.cmbFontList.SelectedIndexChanged += new System.EventHandler(this.cmbFontList_SelectedIndexChanged);
            this.cmbFontList.TextChanged += new System.EventHandler(this.cmbFontList_TextChanged);
            this.cmbFontList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbFontList_KeyDown);
            this.cmbFontList.Leave += new System.EventHandler(this.cmbFontList_Leave);
            // 
            // comboBox3
            // 
            this.comboBox3.BackColor = System.Drawing.Color.White;
            this.comboBox3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28"});
            this.comboBox3.Location = new System.Drawing.Point(570, 43);
            this.comboBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(61, 23);
            this.comboBox3.TabIndex = 42;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            this.comboBox3.Leave += new System.EventHandler(this.comboBox3_Leave);
            this.comboBox3.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.comboBox3_PreviewKeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.Gray;
            this.label8.Location = new System.Drawing.Point(12, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 15);
            this.label8.TabIndex = 45;
            this.label8.Text = "보기";
            // 
            // btnOnOff
            // 
            this.btnOnOff.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnOnOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnOnOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOnOff.ForeColor = System.Drawing.Color.White;
            this.btnOnOff.Image = global::RoadMan.Properties.Resources.보기_norma;
            this.btnOnOff.Location = new System.Drawing.Point(7, 16);
            this.btnOnOff.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOnOff.Name = "btnOnOff";
            this.btnOnOff.Size = new System.Drawing.Size(38, 38);
            this.btnOnOff.TabIndex = 46;
            this.btnOnOff.UseVisualStyleBackColor = false;
            this.btnOnOff.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = global::RoadMan.Properties.Resources.skin_line_img;
            this.pictureBox7.Location = new System.Drawing.Point(48, 12);
            this.pictureBox7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(2, 78);
            this.pictureBox7.TabIndex = 43;
            this.pictureBox7.TabStop = false;
            // 
            // btnStrong
            // 
            this.btnStrong.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnStrong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnStrong.BackgroundImage = global::RoadMan.Properties.Resources.굵게_norma;
            this.btnStrong.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStrong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStrong.ForeColor = System.Drawing.Color.White;
            this.btnStrong.Location = new System.Drawing.Point(688, 42);
            this.btnStrong.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStrong.Name = "btnStrong";
            this.btnStrong.Size = new System.Drawing.Size(24, 30);
            this.btnStrong.TabIndex = 37;
            this.btnStrong.UseVisualStyleBackColor = false;
            this.btnStrong.CheckedChanged += new System.EventHandler(this.btnStrong_CheckedChanged);
            // 
            // btnItalic
            // 
            this.btnItalic.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnItalic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnItalic.BackgroundImage = global::RoadMan.Properties.Resources.이탤릭_norma;
            this.btnItalic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnItalic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnItalic.ForeColor = System.Drawing.Color.White;
            this.btnItalic.Location = new System.Drawing.Point(662, 42);
            this.btnItalic.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnItalic.Name = "btnItalic";
            this.btnItalic.Size = new System.Drawing.Size(24, 30);
            this.btnItalic.TabIndex = 38;
            this.btnItalic.UseVisualStyleBackColor = false;
            this.btnItalic.CheckedChanged += new System.EventHandler(this.btnItalic_CheckedChanged);
            // 
            // btnUnderline
            // 
            this.btnUnderline.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnUnderline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnUnderline.BackgroundImage = global::RoadMan.Properties.Resources.밑줄_norma;
            this.btnUnderline.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnUnderline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnderline.ForeColor = System.Drawing.Color.White;
            this.btnUnderline.Location = new System.Drawing.Point(637, 42);
            this.btnUnderline.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnUnderline.Name = "btnUnderline";
            this.btnUnderline.Size = new System.Drawing.Size(24, 30);
            this.btnUnderline.TabIndex = 39;
            this.btnUnderline.UseVisualStyleBackColor = false;
            this.btnUnderline.CheckedChanged += new System.EventHandler(this.btnUnderline_CheckedChanged);
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::RoadMan.Properties.Resources.skin_line_img;
            this.pictureBox5.Location = new System.Drawing.Point(562, 12);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(2, 78);
            this.pictureBox5.TabIndex = 36;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::RoadMan.Properties.Resources.skin_line_img;
            this.pictureBox4.Location = new System.Drawing.Point(457, 12);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(2, 78);
            this.pictureBox4.TabIndex = 34;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = global::RoadMan.Properties.Resources.skin_line_img;
            this.pictureBox6.Location = new System.Drawing.Point(379, 12);
            this.pictureBox6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(2, 78);
            this.pictureBox6.TabIndex = 32;
            this.pictureBox6.TabStop = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Image = global::RoadMan.Properties.Resources.선택삭제;
            this.btnDelete.Location = new System.Drawing.Point(93, 16);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(38, 38);
            this.btnDelete.TabIndex = 29;
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnDeleteAll.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnDeleteAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteAll.ForeColor = System.Drawing.Color.White;
            this.btnDeleteAll.Image = global::RoadMan.Properties.Resources.삭제;
            this.btnDeleteAll.Location = new System.Drawing.Point(53, 16);
            this.btnDeleteAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(38, 38);
            this.btnDeleteAll.TabIndex = 27;
            this.btnDeleteAll.UseVisualStyleBackColor = false;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::RoadMan.Properties.Resources.skin_line_img;
            this.pictureBox3.Location = new System.Drawing.Point(175, 12);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(2, 78);
            this.pictureBox3.TabIndex = 26;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBoxTextColor
            // 
            this.pictureBoxTextColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxTextColor.Location = new System.Drawing.Point(519, 15);
            this.pictureBoxTextColor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBoxTextColor.Name = "pictureBoxTextColor";
            this.pictureBoxTextColor.Size = new System.Drawing.Size(32, 33);
            this.pictureBoxTextColor.TabIndex = 23;
            this.pictureBoxTextColor.TabStop = false;
            this.pictureBoxTextColor.Click += new System.EventHandler(this.pictureBoxTextColor_Click);
            // 
            // pictureBoxLineColor
            // 
            this.pictureBoxLineColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxLineColor.Location = new System.Drawing.Point(472, 15);
            this.pictureBoxLineColor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBoxLineColor.Name = "pictureBoxLineColor";
            this.pictureBoxLineColor.Size = new System.Drawing.Size(32, 33);
            this.pictureBoxLineColor.TabIndex = 21;
            this.pictureBoxLineColor.TabStop = false;
            this.pictureBoxLineColor.Click += new System.EventHandler(this.pictureBoxLineColor_Click);
            // 
            // btnText
            // 
            this.btnText.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnText.ForeColor = System.Drawing.Color.White;
            this.btnText.Image = global::RoadMan.Properties.Resources.텍스트그리기_normal;
            this.btnText.Location = new System.Drawing.Point(337, 16);
            this.btnText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnText.Name = "btnText";
            this.btnText.Size = new System.Drawing.Size(38, 38);
            this.btnText.TabIndex = 6;
            this.btnText.UseVisualStyleBackColor = false;
            this.btnText.CheckedChanged += new System.EventHandler(this.btnText_CheckedChanged);
            // 
            // btnEllipse
            // 
            this.btnEllipse.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnEllipse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnEllipse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEllipse.ForeColor = System.Drawing.Color.White;
            this.btnEllipse.Image = global::RoadMan.Properties.Resources.원_normal;
            this.btnEllipse.Location = new System.Drawing.Point(298, 16);
            this.btnEllipse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEllipse.Name = "btnEllipse";
            this.btnEllipse.Size = new System.Drawing.Size(38, 38);
            this.btnEllipse.TabIndex = 5;
            this.btnEllipse.UseVisualStyleBackColor = false;
            this.btnEllipse.CheckedChanged += new System.EventHandler(this.btnEllipse_CheckedChanged);
            // 
            // btnRect
            // 
            this.btnRect.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnRect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnRect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRect.ForeColor = System.Drawing.Color.White;
            this.btnRect.Image = global::RoadMan.Properties.Resources.사각_normal;
            this.btnRect.Location = new System.Drawing.Point(259, 16);
            this.btnRect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRect.Name = "btnRect";
            this.btnRect.Size = new System.Drawing.Size(38, 38);
            this.btnRect.TabIndex = 4;
            this.btnRect.UseVisualStyleBackColor = false;
            this.btnRect.CheckedChanged += new System.EventHandler(this.btnRect_CheckedChanged);
            // 
            // btnFreeDraw
            // 
            this.btnFreeDraw.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnFreeDraw.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnFreeDraw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFreeDraw.ForeColor = System.Drawing.Color.White;
            this.btnFreeDraw.Image = global::RoadMan.Properties.Resources.곡선_normal;
            this.btnFreeDraw.Location = new System.Drawing.Point(220, 16);
            this.btnFreeDraw.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnFreeDraw.Name = "btnFreeDraw";
            this.btnFreeDraw.Size = new System.Drawing.Size(38, 38);
            this.btnFreeDraw.TabIndex = 3;
            this.btnFreeDraw.UseVisualStyleBackColor = false;
            this.btnFreeDraw.CheckedChanged += new System.EventHandler(this.btnFreeDraw_CheckedChanged);
            // 
            // btnLine
            // 
            this.btnLine.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLine.ForeColor = System.Drawing.Color.White;
            this.btnLine.Image = global::RoadMan.Properties.Resources.직선_normal;
            this.btnLine.Location = new System.Drawing.Point(181, 16);
            this.btnLine.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnLine.Name = "btnLine";
            this.btnLine.Size = new System.Drawing.Size(38, 38);
            this.btnLine.TabIndex = 2;
            this.btnLine.UseVisualStyleBackColor = false;
            this.btnLine.CheckedChanged += new System.EventHandler(this.btnLine_CheckedChanged);
            // 
            // btnSelect
            // 
            this.btnSelect.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.ForeColor = System.Drawing.Color.White;
            this.btnSelect.Image = global::RoadMan.Properties.Resources.선택;
            this.btnSelect.Location = new System.Drawing.Point(133, 16);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(38, 38);
            this.btnSelect.TabIndex = 47;
            this.btnSelect.UseVisualStyleBackColor = false;
            this.btnSelect.CheckedChanged += new System.EventHandler(this.btnSelect_CheckedChanged);
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // FormMemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.ClientSize = new System.Drawing.Size(725, 108);
            this.ControlBox = false;
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnOnOff);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.cmbFontList);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnStrong);
            this.Controls.Add(this.btnItalic);
            this.Controls.Add(this.btnUnderline);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pictureBox6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxTextColor);
            this.Controls.Add(this.pictureBoxLineColor);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.btnText);
            this.Controls.Add(this.btnEllipse);
            this.Controls.Add(this.btnRect);
            this.Controls.Add(this.btnFreeDraw);
            this.Controls.Add(this.btnLine);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMemo";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormMemo";
            this.Load += new System.EventHandler(this.FormMemo_Load);
            this.Enter += new System.EventHandler(this.FormMemo_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTextColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLineColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.FontDialog fontDialog1;
		private System.Windows.Forms.RadioButton btnLine;
		private System.Windows.Forms.RadioButton btnFreeDraw;
		private System.Windows.Forms.RadioButton btnRect;
		private System.Windows.Forms.RadioButton btnEllipse;
		private System.Windows.Forms.RadioButton btnText;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBoxTextColor;
		private System.Windows.Forms.PictureBox pictureBoxLineColor;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnDeleteAll;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.PictureBox pictureBox6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.PictureBox pictureBox4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.PictureBox pictureBox5;
		private System.Windows.Forms.CheckBox btnStrong;
		private System.Windows.Forms.CheckBox btnItalic;
		private System.Windows.Forms.CheckBox btnUnderline;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox cmbFontList;
		private System.Windows.Forms.ComboBox comboBox3;
		private System.Windows.Forms.PictureBox pictureBox7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox btnOnOff;
		private System.Windows.Forms.RadioButton btnSelect;
	}
}