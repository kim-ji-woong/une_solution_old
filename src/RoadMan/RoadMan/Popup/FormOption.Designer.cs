namespace RoadMan
{
    partial class FormOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOption));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxBackupFileCount = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBoxBackColor = new System.Windows.Forms.PictureBox();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBoxShowBackgroundImage = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.ckbObjectZoom = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.radioLengthRatio = new System.Windows.Forms.RadioButton();
            this.radioAreaRatio = new System.Windows.Forms.RadioButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnReduceHeight = new System.Windows.Forms.Button();
            this.btnIncreaseHeight = new System.Windows.Forms.Button();
            this.btnReduceWidth = new System.Windows.Forms.Button();
            this.btnIncreaseWidth = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnDeleteImage = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.editHeight = new System.Windows.Forms.TextBox();
            this.editWidth = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxSelectOnScreen = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnApplayImage = new System.Windows.Forms.Button();
            this.editDivision = new System.Windows.Forms.TextBox();
            this.editOffsetY = new System.Windows.Forms.TextBox();
            this.editOffsetX = new System.Windows.Forms.TextBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.editImagePath = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxRegionName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.btnSetLayer = new System.Windows.Forms.Button();
            this.btnSettingStreets = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackColor)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(629, 446);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "확인";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(722, 446);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxBackupFileCount);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(469, 62);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "파일 입출력";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "백업파일 개수 :";
            // 
            // textBoxBackupFileCount
            // 
            this.textBoxBackupFileCount.Location = new System.Drawing.Point(131, 21);
            this.textBoxBackupFileCount.Name = "textBoxBackupFileCount";
            this.textBoxBackupFileCount.Size = new System.Drawing.Size(41, 23);
            this.textBoxBackupFileCount.TabIndex = 2;
            this.textBoxBackupFileCount.TextChanged += new System.EventHandler(this.textBoxBackupFileCount_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBoxBackColor);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.checkBoxShowBackgroundImage);
            this.groupBox2.Location = new System.Drawing.Point(6, 149);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(469, 77);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "배경";
            // 
            // pictureBoxBackColor
            // 
            this.pictureBoxBackColor.BackColor = System.Drawing.Color.Black;
            this.pictureBoxBackColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxBackColor.Location = new System.Drawing.Point(272, 28);
            this.pictureBoxBackColor.Name = "pictureBoxBackColor";
            this.pictureBoxBackColor.Size = new System.Drawing.Size(28, 24);
            this.pictureBoxBackColor.TabIndex = 12;
            this.pictureBoxBackColor.TabStop = false;
            this.pictureBoxBackColor.Click += new System.EventHandler(this.pictureBoxBackColor_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(223, 32);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(50, 15);
            this.label13.TabIndex = 13;
            this.label13.Text = "배경색 :";
            // 
            // checkBoxShowBackgroundImage
            // 
            this.checkBoxShowBackgroundImage.AutoSize = true;
            this.checkBoxShowBackgroundImage.Checked = true;
            this.checkBoxShowBackgroundImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowBackgroundImage.Location = new System.Drawing.Point(35, 32);
            this.checkBoxShowBackgroundImage.Name = "checkBoxShowBackgroundImage";
            this.checkBoxShowBackgroundImage.Size = new System.Drawing.Size(126, 19);
            this.checkBoxShowBackgroundImage.TabIndex = 11;
            this.checkBoxShowBackgroundImage.Text = "배경이미지 보이기";
            this.checkBoxShowBackgroundImage.UseVisualStyleBackColor = true;
            this.checkBoxShowBackgroundImage.CheckedChanged += new System.EventHandler(this.checkBoxShowBackgroundImage_CheckedChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(815, 438);
            this.tabControl1.TabIndex = 16;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox7);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(807, 410);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "일반";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.ckbObjectZoom);
            this.groupBox7.Location = new System.Drawing.Point(6, 77);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(469, 62);
            this.groupBox7.TabIndex = 5;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "마우스 선택";
            // 
            // ckbObjectZoom
            // 
            this.ckbObjectZoom.AutoSize = true;
            this.ckbObjectZoom.Location = new System.Drawing.Point(35, 26);
            this.ckbObjectZoom.Name = "ckbObjectZoom";
            this.ckbObjectZoom.Size = new System.Drawing.Size(158, 19);
            this.ckbObjectZoom.TabIndex = 0;
            this.ckbObjectZoom.Text = "도로 선택시 포커스 이동";
            this.ckbObjectZoom.UseVisualStyleBackColor = true;
            this.ckbObjectZoom.CheckedChanged += new System.EventHandler(this.ckbObjectZoom_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.radioLengthRatio);
            this.groupBox6.Controls.Add(this.radioAreaRatio);
            this.groupBox6.Location = new System.Drawing.Point(6, 239);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(469, 71);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "개설 비율(%)";
            // 
            // radioLengthRatio
            // 
            this.radioLengthRatio.AutoSize = true;
            this.radioLengthRatio.Location = new System.Drawing.Point(226, 31);
            this.radioLengthRatio.Name = "radioLengthRatio";
            this.radioLengthRatio.Size = new System.Drawing.Size(141, 19);
            this.radioLengthRatio.TabIndex = 0;
            this.radioLengthRatio.TabStop = true;
            this.radioLengthRatio.Text = "총길이 대비 개설길이";
            this.radioLengthRatio.UseVisualStyleBackColor = true;
            this.radioLengthRatio.CheckedChanged += new System.EventHandler(this.radioRatio_CheckedChanged);
            // 
            // radioAreaRatio
            // 
            this.radioAreaRatio.AutoSize = true;
            this.radioAreaRatio.Location = new System.Drawing.Point(35, 31);
            this.radioAreaRatio.Name = "radioAreaRatio";
            this.radioAreaRatio.Size = new System.Drawing.Size(141, 19);
            this.radioAreaRatio.TabIndex = 0;
            this.radioAreaRatio.TabStop = true;
            this.radioAreaRatio.Text = "총면적 대비 개설면적";
            this.radioAreaRatio.UseVisualStyleBackColor = true;
            this.radioAreaRatio.CheckedChanged += new System.EventHandler(this.radioRatio_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(807, 410);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "도면";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.btnReduceHeight);
            this.groupBox3.Controls.Add(this.btnIncreaseHeight);
            this.groupBox3.Controls.Add(this.btnReduceWidth);
            this.groupBox3.Controls.Add(this.btnIncreaseWidth);
            this.groupBox3.Controls.Add(this.btnDown);
            this.groupBox3.Controls.Add(this.btnUp);
            this.groupBox3.Controls.Add(this.btnRight);
            this.groupBox3.Controls.Add(this.btnLeft);
            this.groupBox3.Location = new System.Drawing.Point(532, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(261, 386);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "이미지 조정";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(30, 159);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 15);
            this.label12.TabIndex = 17;
            this.label12.Text = "위치 조정";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(30, 94);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 15);
            this.label11.TabIndex = 16;
            this.label11.Text = "높이 조정";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(30, 34);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 15);
            this.label10.TabIndex = 15;
            this.label10.Text = "너비 조정";
            // 
            // btnReduceHeight
            // 
            this.btnReduceHeight.Location = new System.Drawing.Point(134, 114);
            this.btnReduceHeight.Name = "btnReduceHeight";
            this.btnReduceHeight.Size = new System.Drawing.Size(83, 23);
            this.btnReduceHeight.TabIndex = 14;
            this.btnReduceHeight.Text = "높이 줄이기";
            this.btnReduceHeight.UseVisualStyleBackColor = true;
            this.btnReduceHeight.Click += new System.EventHandler(this.btnReduceHeight_Click);
            // 
            // btnIncreaseHeight
            // 
            this.btnIncreaseHeight.Location = new System.Drawing.Point(42, 114);
            this.btnIncreaseHeight.Name = "btnIncreaseHeight";
            this.btnIncreaseHeight.Size = new System.Drawing.Size(82, 23);
            this.btnIncreaseHeight.TabIndex = 13;
            this.btnIncreaseHeight.Text = "높이 늘이기";
            this.btnIncreaseHeight.UseVisualStyleBackColor = true;
            this.btnIncreaseHeight.Click += new System.EventHandler(this.btnIncreaseHeight_Click);
            // 
            // btnReduceWidth
            // 
            this.btnReduceWidth.Location = new System.Drawing.Point(134, 57);
            this.btnReduceWidth.Name = "btnReduceWidth";
            this.btnReduceWidth.Size = new System.Drawing.Size(83, 23);
            this.btnReduceWidth.TabIndex = 12;
            this.btnReduceWidth.Text = "너비 줄이기";
            this.btnReduceWidth.UseVisualStyleBackColor = true;
            this.btnReduceWidth.Click += new System.EventHandler(this.btnReduceWidth_Click);
            // 
            // btnIncreaseWidth
            // 
            this.btnIncreaseWidth.Location = new System.Drawing.Point(42, 57);
            this.btnIncreaseWidth.Name = "btnIncreaseWidth";
            this.btnIncreaseWidth.Size = new System.Drawing.Size(82, 23);
            this.btnIncreaseWidth.TabIndex = 11;
            this.btnIncreaseWidth.Text = "너비 늘이기";
            this.btnIncreaseWidth.UseVisualStyleBackColor = true;
            this.btnIncreaseWidth.Click += new System.EventHandler(this.btnIncreaseWidth_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(103, 271);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(35, 35);
            this.btnDown.TabIndex = 10;
            this.btnDown.Text = "▼";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(103, 196);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(35, 35);
            this.btnUp.TabIndex = 9;
            this.btnUp.Text = "▲";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(148, 235);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(35, 35);
            this.btnRight.TabIndex = 8;
            this.btnRight.Text = "▶";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(56, 235);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(35, 35);
            this.btnLeft.TabIndex = 7;
            this.btnLeft.Text = "◀";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnDeleteImage);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.editHeight);
            this.groupBox5.Controls.Add(this.editWidth);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.checkBoxSelectOnScreen);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.btnApplayImage);
            this.groupBox5.Controls.Add(this.editDivision);
            this.groupBox5.Controls.Add(this.editOffsetY);
            this.groupBox5.Controls.Add(this.editOffsetX);
            this.groupBox5.Controls.Add(this.btnOpenFile);
            this.groupBox5.Controls.Add(this.editImagePath);
            this.groupBox5.Location = new System.Drawing.Point(6, 168);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(469, 224);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "배경이미지";
            // 
            // btnDeleteImage
            // 
            this.btnDeleteImage.Location = new System.Drawing.Point(281, 181);
            this.btnDeleteImage.Name = "btnDeleteImage";
            this.btnDeleteImage.Size = new System.Drawing.Size(127, 23);
            this.btnDeleteImage.TabIndex = 35;
            this.btnDeleteImage.Text = "배경이미지 삭제";
            this.btnDeleteImage.UseVisualStyleBackColor = true;
            this.btnDeleteImage.Click += new System.EventHandler(this.btnDeleteImage_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 181);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 15);
            this.label8.TabIndex = 34;
            this.label8.Text = "높이 :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(30, 156);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 15);
            this.label9.TabIndex = 33;
            this.label9.Text = "너비 :";
            // 
            // editHeight
            // 
            this.editHeight.Enabled = false;
            this.editHeight.Location = new System.Drawing.Point(85, 178);
            this.editHeight.Name = "editHeight";
            this.editHeight.Size = new System.Drawing.Size(100, 23);
            this.editHeight.TabIndex = 32;
            // 
            // editWidth
            // 
            this.editWidth.Enabled = false;
            this.editWidth.Location = new System.Drawing.Point(85, 153);
            this.editWidth.Name = "editWidth";
            this.editWidth.Size = new System.Drawing.Size(100, 23);
            this.editWidth.TabIndex = 31;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(262, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(179, 15);
            this.label7.TabIndex = 30;
            this.label7.Text = "화살표키 누를 때 이동하는 거리";
            // 
            // checkBoxSelectOnScreen
            // 
            this.checkBoxSelectOnScreen.AutoSize = true;
            this.checkBoxSelectOnScreen.Checked = true;
            this.checkBoxSelectOnScreen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSelectOnScreen.Location = new System.Drawing.Point(32, 73);
            this.checkBoxSelectOnScreen.Name = "checkBoxSelectOnScreen";
            this.checkBoxSelectOnScreen.Size = new System.Drawing.Size(102, 19);
            this.checkBoxSelectOnScreen.TabIndex = 29;
            this.checkBoxSelectOnScreen.Text = "화면에서 선택";
            this.checkBoxSelectOnScreen.UseVisualStyleBackColor = true;
            this.checkBoxSelectOnScreen.CheckedChanged += new System.EventHandler(this.checkBoxSelectOnScreen_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(406, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 15);
            this.label6.TabIndex = 28;
            this.label6.Text = "m";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(262, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 15);
            this.label5.TabIndex = 27;
            this.label5.Text = "이동 눈금";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 15);
            this.label4.TabIndex = 26;
            this.label4.Text = "위치 Y";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 25;
            this.label3.Text = "배경이미지";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 15);
            this.label2.TabIndex = 24;
            this.label2.Text = "위치 X :";
            // 
            // btnApplayImage
            // 
            this.btnApplayImage.Location = new System.Drawing.Point(281, 138);
            this.btnApplayImage.Name = "btnApplayImage";
            this.btnApplayImage.Size = new System.Drawing.Size(127, 23);
            this.btnApplayImage.TabIndex = 23;
            this.btnApplayImage.Text = "위치적용하기";
            this.btnApplayImage.UseVisualStyleBackColor = true;
            this.btnApplayImage.Click += new System.EventHandler(this.btnApplayImage_Click);
            // 
            // editDivision
            // 
            this.editDivision.Location = new System.Drawing.Point(325, 71);
            this.editDivision.Name = "editDivision";
            this.editDivision.Size = new System.Drawing.Size(75, 23);
            this.editDivision.TabIndex = 22;
            this.editDivision.Text = "10.0";
            // 
            // editOffsetY
            // 
            this.editOffsetY.Enabled = false;
            this.editOffsetY.Location = new System.Drawing.Point(85, 126);
            this.editOffsetY.Name = "editOffsetY";
            this.editOffsetY.Size = new System.Drawing.Size(100, 23);
            this.editOffsetY.TabIndex = 21;
            // 
            // editOffsetX
            // 
            this.editOffsetX.Enabled = false;
            this.editOffsetX.Location = new System.Drawing.Point(85, 101);
            this.editOffsetX.Name = "editOffsetX";
            this.editOffsetX.Size = new System.Drawing.Size(100, 23);
            this.editOffsetX.TabIndex = 20;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(414, 31);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(24, 23);
            this.btnOpenFile.TabIndex = 19;
            this.btnOpenFile.Text = "▼";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // editImagePath
            // 
            this.editImagePath.Location = new System.Drawing.Point(92, 33);
            this.editImagePath.Name = "editImagePath";
            this.editImagePath.Size = new System.Drawing.Size(316, 23);
            this.editImagePath.TabIndex = 18;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBoxRegionName);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.btnSetLayer);
            this.groupBox4.Controls.Add(this.btnSettingStreets);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(469, 156);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "설정";
            // 
            // textBoxRegionName
            // 
            this.textBoxRegionName.Location = new System.Drawing.Point(81, 26);
            this.textBoxRegionName.Name = "textBoxRegionName";
            this.textBoxRegionName.Size = new System.Drawing.Size(169, 23);
            this.textBoxRegionName.TabIndex = 2;
            this.textBoxRegionName.TextChanged += new System.EventHandler(this.textBoxRegionName_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(25, 28);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(50, 15);
            this.label14.TabIndex = 1;
            this.label14.Text = "지역명 :";
            // 
            // btnSetLayer
            // 
            this.btnSetLayer.Location = new System.Drawing.Point(140, 64);
            this.btnSetLayer.Name = "btnSetLayer";
            this.btnSetLayer.Size = new System.Drawing.Size(110, 23);
            this.btnSetLayer.TabIndex = 0;
            this.btnSetLayer.Text = "표시 도면층 설정";
            this.btnSetLayer.UseVisualStyleBackColor = true;
            this.btnSetLayer.Click += new System.EventHandler(this.btnSetLayer_Click);
            // 
            // btnSettingStreets
            // 
            this.btnSettingStreets.Location = new System.Drawing.Point(24, 64);
            this.btnSettingStreets.Name = "btnSettingStreets";
            this.btnSettingStreets.Size = new System.Drawing.Size(98, 23);
            this.btnSettingStreets.TabIndex = 0;
            this.btnSettingStreets.Text = "노선구간 설정";
            this.btnSettingStreets.UseVisualStyleBackColor = true;
            this.btnSettingStreets.Click += new System.EventHandler(this.btnSettingStreets_Click);
            // 
            // FormOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 479);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormOption";
            this.Text = "옵션";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOption_FormClosing);
            this.Load += new System.EventHandler(this.FormOption_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackColor)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxBackupFileCount;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSettingStreets;
        private System.Windows.Forms.Button btnSetLayer;
        private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.CheckBox ckbObjectZoom;
        private System.Windows.Forms.CheckBox checkBoxShowBackgroundImage;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton radioLengthRatio;
        private System.Windows.Forms.RadioButton radioAreaRatio;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnReduceHeight;
        private System.Windows.Forms.Button btnIncreaseHeight;
        private System.Windows.Forms.Button btnReduceWidth;
        private System.Windows.Forms.Button btnIncreaseWidth;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnDeleteImage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox editHeight;
        private System.Windows.Forms.TextBox editWidth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxSelectOnScreen;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnApplayImage;
        private System.Windows.Forms.TextBox editDivision;
        private System.Windows.Forms.TextBox editOffsetY;
        private System.Windows.Forms.TextBox editOffsetX;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.TextBox editImagePath;
        private System.Windows.Forms.PictureBox pictureBoxBackColor;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxRegionName;
        private System.Windows.Forms.Label label14;
    }
}