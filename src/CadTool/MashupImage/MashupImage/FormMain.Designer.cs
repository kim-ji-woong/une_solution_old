namespace MashupImage
{
    partial class FormMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.cboLOD = new System.Windows.Forms.ComboBox();
            this.btnAddLOD = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFolderPath = new System.Windows.Forms.TextBox();
            this.btnFolderPath = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblImageHeight = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblImageWidth = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblImageVCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblImageHCount = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnRemoveLOD = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblImageTotalHeight = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblImageTotalWidth = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cboAddPixel = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnArrangeLOD = new System.Windows.Forms.Button();
            this.splitContainerBody = new System.Windows.Forms.SplitContainer();
            this.splitContainerLeft = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.cboShapeLOD = new System.Windows.Forms.ComboBox();
            this.textBoxShapeY = new System.Windows.Forms.TextBox();
            this.textBoxShapeX = new System.Windows.Forms.TextBox();
            this.textBoxShapeFilePath = new System.Windows.Forms.TextBox();
            this.btnShapeFilePath = new System.Windows.Forms.Button();
            this.checkBoxAllLod = new System.Windows.Forms.CheckBox();
            this.cboShapes = new System.Windows.Forms.ComboBox();
            this.textBoxShapeName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnRemoveShape = new System.Windows.Forms.Button();
            this.btnAddShape = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.btnApplyShape = new System.Windows.Forms.Button();
            this.btnRenameShape = new System.Windows.Forms.Button();
            this.panelImage = new MashupImage.ImagePanel();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBody)).BeginInit();
            this.splitContainerBody.Panel1.SuspendLayout();
            this.splitContainerBody.Panel2.SuspendLayout();
            this.splitContainerBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).BeginInit();
            this.splitContainerLeft.Panel1.SuspendLayout();
            this.splitContainerLeft.Panel2.SuspendLayout();
            this.splitContainerLeft.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1199, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuNewProject,
            this.tsMenuOpenProject,
            this.tsMenuSave});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.fileToolStripMenuItem.Text = "파일";
            // 
            // tsMenuNewProject
            // 
            this.tsMenuNewProject.Name = "tsMenuNewProject";
            this.tsMenuNewProject.Size = new System.Drawing.Size(180, 22);
            this.tsMenuNewProject.Text = "새로 만들기";
            this.tsMenuNewProject.Click += new System.EventHandler(this.tsMenuNewProject_Click);
            // 
            // tsMenuOpenProject
            // 
            this.tsMenuOpenProject.Name = "tsMenuOpenProject";
            this.tsMenuOpenProject.Size = new System.Drawing.Size(180, 22);
            this.tsMenuOpenProject.Text = "열기";
            this.tsMenuOpenProject.Click += new System.EventHandler(this.tsMenuOpenProject_Click);
            // 
            // tsMenuSave
            // 
            this.tsMenuSave.Enabled = false;
            this.tsMenuSave.Name = "tsMenuSave";
            this.tsMenuSave.Size = new System.Drawing.Size(180, 22);
            this.tsMenuSave.Text = "저장하기";
            this.tsMenuSave.Click += new System.EventHandler(this.tsMenuSave_Click);
            // 
            // cboLOD
            // 
            this.cboLOD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLOD.Enabled = false;
            this.cboLOD.FormattingEnabled = true;
            this.cboLOD.Location = new System.Drawing.Point(275, 73);
            this.cboLOD.Name = "cboLOD";
            this.cboLOD.Size = new System.Drawing.Size(97, 20);
            this.cboLOD.TabIndex = 2;
            this.cboLOD.SelectedIndexChanged += new System.EventHandler(this.cboLOD_SelectedIndexChanged);
            // 
            // btnAddLOD
            // 
            this.btnAddLOD.Enabled = false;
            this.btnAddLOD.Location = new System.Drawing.Point(162, 109);
            this.btnAddLOD.Name = "btnAddLOD";
            this.btnAddLOD.Size = new System.Drawing.Size(66, 23);
            this.btnAddLOD.TabIndex = 3;
            this.btnAddLOD.Text = "LOD 추가";
            this.btnAddLOD.UseVisualStyleBackColor = true;
            this.btnAddLOD.Click += new System.EventHandler(this.btnAddLOD_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Map 이미지 폴더 :";
            // 
            // textBoxFolderPath
            // 
            this.textBoxFolderPath.Enabled = false;
            this.textBoxFolderPath.Location = new System.Drawing.Point(128, 28);
            this.textBoxFolderPath.Name = "textBoxFolderPath";
            this.textBoxFolderPath.Size = new System.Drawing.Size(206, 21);
            this.textBoxFolderPath.TabIndex = 5;
            // 
            // btnFolderPath
            // 
            this.btnFolderPath.Enabled = false;
            this.btnFolderPath.Location = new System.Drawing.Point(340, 27);
            this.btnFolderPath.Name = "btnFolderPath";
            this.btnFolderPath.Size = new System.Drawing.Size(32, 23);
            this.btnFolderPath.TabIndex = 3;
            this.btnFolderPath.Text = "...";
            this.btnFolderPath.UseVisualStyleBackColor = true;
            this.btnFolderPath.Click += new System.EventHandler(this.btnFolderPath_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblImageHeight);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblImageWidth);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(18, 175);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(106, 76);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "이미지 크기";
            // 
            // lblImageHeight
            // 
            this.lblImageHeight.AutoSize = true;
            this.lblImageHeight.Location = new System.Drawing.Point(49, 49);
            this.lblImageHeight.Name = "lblImageHeight";
            this.lblImageHeight.Size = new System.Drawing.Size(37, 12);
            this.lblImageHeight.TabIndex = 0;
            this.lblImageHeight.Text = "100px";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "세로 :";
            // 
            // lblImageWidth
            // 
            this.lblImageWidth.AutoSize = true;
            this.lblImageWidth.Location = new System.Drawing.Point(49, 27);
            this.lblImageWidth.Name = "lblImageWidth";
            this.lblImageWidth.Size = new System.Drawing.Size(37, 12);
            this.lblImageWidth.TabIndex = 0;
            this.lblImageWidth.Text = "100px";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "가로 :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblImageVCount);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.lblImageHCount);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(143, 175);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(106, 76);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "이미지 개수";
            // 
            // lblImageVCount
            // 
            this.lblImageVCount.AutoSize = true;
            this.lblImageVCount.Location = new System.Drawing.Point(49, 49);
            this.lblImageVCount.Name = "lblImageVCount";
            this.lblImageVCount.Size = new System.Drawing.Size(29, 12);
            this.lblImageVCount.TabIndex = 0;
            this.lblImageVCount.Text = "10개";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "세로 :";
            // 
            // lblImageHCount
            // 
            this.lblImageHCount.AutoSize = true;
            this.lblImageHCount.Location = new System.Drawing.Point(49, 26);
            this.lblImageHCount.Name = "lblImageHCount";
            this.lblImageHCount.Size = new System.Drawing.Size(29, 12);
            this.lblImageHCount.TabIndex = 0;
            this.lblImageHCount.Text = "10개";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "가로 :";
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(306, 257);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(66, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnRemoveLOD
            // 
            this.btnRemoveLOD.Enabled = false;
            this.btnRemoveLOD.Location = new System.Drawing.Point(234, 109);
            this.btnRemoveLOD.Name = "btnRemoveLOD";
            this.btnRemoveLOD.Size = new System.Drawing.Size(66, 23);
            this.btnRemoveLOD.TabIndex = 3;
            this.btnRemoveLOD.Text = "LOD 삭제";
            this.btnRemoveLOD.UseVisualStyleBackColor = true;
            this.btnRemoveLOD.Click += new System.EventHandler(this.btnRemoveLOD_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblImageTotalHeight);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.lblImageTotalWidth);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(266, 175);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(106, 76);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "전체 이미지";
            // 
            // lblImageTotalHeight
            // 
            this.lblImageTotalHeight.AutoSize = true;
            this.lblImageTotalHeight.Location = new System.Drawing.Point(49, 49);
            this.lblImageTotalHeight.Name = "lblImageTotalHeight";
            this.lblImageTotalHeight.Size = new System.Drawing.Size(43, 12);
            this.lblImageTotalHeight.TabIndex = 0;
            this.lblImageTotalHeight.Text = "1000px";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "세로 :";
            // 
            // lblImageTotalWidth
            // 
            this.lblImageTotalWidth.AutoSize = true;
            this.lblImageTotalWidth.Location = new System.Drawing.Point(49, 26);
            this.lblImageTotalWidth.Name = "lblImageTotalWidth";
            this.lblImageTotalWidth.Size = new System.Drawing.Size(43, 12);
            this.lblImageTotalWidth.TabIndex = 0;
            this.lblImageTotalWidth.Text = "1000px";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "가로 :";
            // 
            // cboAddPixel
            // 
            this.cboAddPixel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAddPixel.Enabled = false;
            this.cboAddPixel.FormattingEnabled = true;
            this.cboAddPixel.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cboAddPixel.Location = new System.Drawing.Point(85, 73);
            this.cboAddPixel.Name = "cboAddPixel";
            this.cboAddPixel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboAddPixel.Size = new System.Drawing.Size(52, 20);
            this.cboAddPixel.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "여유픽셀 :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(141, 76);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 12);
            this.label8.TabIndex = 8;
            this.label8.Text = "px";
            // 
            // btnArrangeLOD
            // 
            this.btnArrangeLOD.Enabled = false;
            this.btnArrangeLOD.Location = new System.Drawing.Point(306, 109);
            this.btnArrangeLOD.Name = "btnArrangeLOD";
            this.btnArrangeLOD.Size = new System.Drawing.Size(66, 23);
            this.btnArrangeLOD.TabIndex = 3;
            this.btnArrangeLOD.Text = "LOD 정렬";
            this.btnArrangeLOD.UseVisualStyleBackColor = true;
            this.btnArrangeLOD.Click += new System.EventHandler(this.btnArrangeLOD_Click);
            // 
            // splitContainerBody
            // 
            this.splitContainerBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBody.Location = new System.Drawing.Point(0, 24);
            this.splitContainerBody.Name = "splitContainerBody";
            // 
            // splitContainerBody.Panel1
            // 
            this.splitContainerBody.Panel1.Controls.Add(this.splitContainerLeft);
            // 
            // splitContainerBody.Panel2
            // 
            this.splitContainerBody.Panel2.Controls.Add(this.panelImage);
            this.splitContainerBody.Size = new System.Drawing.Size(1199, 530);
            this.splitContainerBody.SplitterDistance = 379;
            this.splitContainerBody.TabIndex = 10;
            // 
            // splitContainerLeft
            // 
            this.splitContainerLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLeft.Name = "splitContainerLeft";
            this.splitContainerLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerLeft.Panel1
            // 
            this.splitContainerLeft.Panel1.Controls.Add(this.panel1);
            this.splitContainerLeft.Panel1.Controls.Add(this.label1);
            this.splitContainerLeft.Panel1.Controls.Add(this.textBoxFolderPath);
            this.splitContainerLeft.Panel1.Controls.Add(this.label6);
            this.splitContainerLeft.Panel1.Controls.Add(this.groupBox1);
            this.splitContainerLeft.Panel1.Controls.Add(this.btnApply);
            this.splitContainerLeft.Panel1.Controls.Add(this.btnArrangeLOD);
            this.splitContainerLeft.Panel1.Controls.Add(this.cboLOD);
            this.splitContainerLeft.Panel1.Controls.Add(this.btnFolderPath);
            this.splitContainerLeft.Panel1.Controls.Add(this.groupBox3);
            this.splitContainerLeft.Panel1.Controls.Add(this.btnRemoveLOD);
            this.splitContainerLeft.Panel1.Controls.Add(this.btnAddLOD);
            this.splitContainerLeft.Panel1.Controls.Add(this.groupBox2);
            this.splitContainerLeft.Panel1.Controls.Add(this.cboAddPixel);
            this.splitContainerLeft.Panel1.Controls.Add(this.label8);
            // 
            // splitContainerLeft.Panel2
            // 
            this.splitContainerLeft.Panel2.Controls.Add(this.btnApplyShape);
            this.splitContainerLeft.Panel2.Controls.Add(this.label15);
            this.splitContainerLeft.Panel2.Controls.Add(this.label14);
            this.splitContainerLeft.Panel2.Controls.Add(this.label13);
            this.splitContainerLeft.Panel2.Controls.Add(this.cboShapeLOD);
            this.splitContainerLeft.Panel2.Controls.Add(this.textBoxShapeY);
            this.splitContainerLeft.Panel2.Controls.Add(this.textBoxShapeX);
            this.splitContainerLeft.Panel2.Controls.Add(this.textBoxShapeFilePath);
            this.splitContainerLeft.Panel2.Controls.Add(this.btnShapeFilePath);
            this.splitContainerLeft.Panel2.Controls.Add(this.checkBoxAllLod);
            this.splitContainerLeft.Panel2.Controls.Add(this.cboShapes);
            this.splitContainerLeft.Panel2.Controls.Add(this.textBoxShapeName);
            this.splitContainerLeft.Panel2.Controls.Add(this.label12);
            this.splitContainerLeft.Panel2.Controls.Add(this.btnRenameShape);
            this.splitContainerLeft.Panel2.Controls.Add(this.btnRemoveShape);
            this.splitContainerLeft.Panel2.Controls.Add(this.btnAddShape);
            this.splitContainerLeft.Panel2.Controls.Add(this.panel2);
            this.splitContainerLeft.Size = new System.Drawing.Size(379, 530);
            this.splitContainerLeft.SplitterDistance = 300;
            this.splitContainerLeft.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(96)))), ((int)(((byte)(130)))));
            this.panel1.Controls.Add(this.label10);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(379, 25);
            this.panel1.TabIndex = 10;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(7, 7);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "2D Map 설정";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(92, 169);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(21, 12);
            this.label15.TabIndex = 12;
            this.label15.Text = "Y :";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(21, 169);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(21, 12);
            this.label14.TabIndex = 12;
            this.label14.Text = "X :";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 140);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(77, 12);
            this.label13.TabIndex = 12;
            this.label13.Text = "이미지 파일 :";
            // 
            // cboShapeLOD
            // 
            this.cboShapeLOD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShapeLOD.Enabled = false;
            this.cboShapeLOD.FormattingEnabled = true;
            this.cboShapeLOD.Location = new System.Drawing.Point(18, 106);
            this.cboShapeLOD.Name = "cboShapeLOD";
            this.cboShapeLOD.Size = new System.Drawing.Size(97, 20);
            this.cboShapeLOD.TabIndex = 11;
            this.cboShapeLOD.Visible = false;
            // 
            // textBoxShapeY
            // 
            this.textBoxShapeY.Enabled = false;
            this.textBoxShapeY.Location = new System.Drawing.Point(116, 166);
            this.textBoxShapeY.Name = "textBoxShapeY";
            this.textBoxShapeY.Size = new System.Drawing.Size(36, 21);
            this.textBoxShapeY.TabIndex = 13;
            // 
            // textBoxShapeX
            // 
            this.textBoxShapeX.Enabled = false;
            this.textBoxShapeX.Location = new System.Drawing.Point(45, 166);
            this.textBoxShapeX.Name = "textBoxShapeX";
            this.textBoxShapeX.Size = new System.Drawing.Size(36, 21);
            this.textBoxShapeX.TabIndex = 13;
            // 
            // textBoxShapeFilePath
            // 
            this.textBoxShapeFilePath.Enabled = false;
            this.textBoxShapeFilePath.Location = new System.Drawing.Point(101, 136);
            this.textBoxShapeFilePath.Name = "textBoxShapeFilePath";
            this.textBoxShapeFilePath.Size = new System.Drawing.Size(233, 21);
            this.textBoxShapeFilePath.TabIndex = 13;
            // 
            // btnShapeFilePath
            // 
            this.btnShapeFilePath.Enabled = false;
            this.btnShapeFilePath.Location = new System.Drawing.Point(340, 135);
            this.btnShapeFilePath.Name = "btnShapeFilePath";
            this.btnShapeFilePath.Size = new System.Drawing.Size(32, 23);
            this.btnShapeFilePath.TabIndex = 11;
            this.btnShapeFilePath.Text = "...";
            this.btnShapeFilePath.UseVisualStyleBackColor = true;
            this.btnShapeFilePath.Click += new System.EventHandler(this.btnShapeFilePath_Click);
            // 
            // checkBoxAllLod
            // 
            this.checkBoxAllLod.AutoSize = true;
            this.checkBoxAllLod.Checked = true;
            this.checkBoxAllLod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAllLod.Enabled = false;
            this.checkBoxAllLod.Location = new System.Drawing.Point(256, 106);
            this.checkBoxAllLod.Name = "checkBoxAllLod";
            this.checkBoxAllLod.Size = new System.Drawing.Size(116, 16);
            this.checkBoxAllLod.TabIndex = 15;
            this.checkBoxAllLod.Text = "모든 LOD에 적용";
            this.checkBoxAllLod.UseVisualStyleBackColor = true;
            this.checkBoxAllLod.Visible = false;
            // 
            // cboShapes
            // 
            this.cboShapes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShapes.FormattingEnabled = true;
            this.cboShapes.Location = new System.Drawing.Point(215, 38);
            this.cboShapes.Name = "cboShapes";
            this.cboShapes.Size = new System.Drawing.Size(157, 20);
            this.cboShapes.TabIndex = 14;
            this.cboShapes.SelectedIndexChanged += new System.EventHandler(this.cboShapes_SelectedIndexChanged);
            // 
            // textBoxShapeName
            // 
            this.textBoxShapeName.Location = new System.Drawing.Point(58, 38);
            this.textBoxShapeName.Name = "textBoxShapeName";
            this.textBoxShapeName.Size = new System.Drawing.Size(151, 21);
            this.textBoxShapeName.TabIndex = 13;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 42);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(37, 12);
            this.label12.TabIndex = 12;
            this.label12.Text = "이름 :";
            // 
            // btnRemoveShape
            // 
            this.btnRemoveShape.Enabled = false;
            this.btnRemoveShape.Location = new System.Drawing.Point(116, 74);
            this.btnRemoveShape.Name = "btnRemoveShape";
            this.btnRemoveShape.Size = new System.Drawing.Size(86, 23);
            this.btnRemoveShape.TabIndex = 11;
            this.btnRemoveShape.Text = "Shape 삭제";
            this.btnRemoveShape.UseVisualStyleBackColor = true;
            this.btnRemoveShape.Click += new System.EventHandler(this.btnRemoveShape_Click);
            // 
            // btnAddShape
            // 
            this.btnAddShape.Location = new System.Drawing.Point(18, 74);
            this.btnAddShape.Name = "btnAddShape";
            this.btnAddShape.Size = new System.Drawing.Size(86, 23);
            this.btnAddShape.TabIndex = 11;
            this.btnAddShape.Text = "Shape 추가";
            this.btnAddShape.UseVisualStyleBackColor = true;
            this.btnAddShape.Click += new System.EventHandler(this.btnAddShape_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(96)))), ((int)(((byte)(130)))));
            this.panel2.Controls.Add(this.label11);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(379, 25);
            this.panel2.TabIndex = 10;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(7, 7);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 12);
            this.label11.TabIndex = 11;
            this.label11.Text = "Shape 설정";
            // 
            // btnApplyShape
            // 
            this.btnApplyShape.Enabled = false;
            this.btnApplyShape.Location = new System.Drawing.Point(306, 169);
            this.btnApplyShape.Name = "btnApplyShape";
            this.btnApplyShape.Size = new System.Drawing.Size(66, 23);
            this.btnApplyShape.TabIndex = 11;
            this.btnApplyShape.Text = "적용";
            this.btnApplyShape.UseVisualStyleBackColor = true;
            this.btnApplyShape.Click += new System.EventHandler(this.btnApplyShape_Click);
            // 
            // btnRenameShape
            // 
            this.btnRenameShape.Enabled = false;
            this.btnRenameShape.Location = new System.Drawing.Point(215, 74);
            this.btnRenameShape.Name = "btnRenameShape";
            this.btnRenameShape.Size = new System.Drawing.Size(86, 23);
            this.btnRenameShape.TabIndex = 11;
            this.btnRenameShape.Text = "이름 변경";
            this.btnRenameShape.UseVisualStyleBackColor = true;
            this.btnRenameShape.Click += new System.EventHandler(this.btnRenameShape_Click);
            // 
            // panelImage
            // 
            this.panelImage.BackColor = System.Drawing.Color.Black;
            this.panelImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImage.Location = new System.Drawing.Point(0, 0);
            this.panelImage.Name = "panelImage";
            this.panelImage.Owner = null;
            this.panelImage.Size = new System.Drawing.Size(816, 530);
            this.panelImage.TabIndex = 1;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1199, 554);
            this.Controls.Add(this.splitContainerBody);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Mashup Image";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.ResizeBegin += new System.EventHandler(this.FormMain_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.FormMain_ResizeEnd);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.splitContainerBody.Panel1.ResumeLayout(false);
            this.splitContainerBody.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBody)).EndInit();
            this.splitContainerBody.ResumeLayout(false);
            this.splitContainerLeft.Panel1.ResumeLayout(false);
            this.splitContainerLeft.Panel1.PerformLayout();
            this.splitContainerLeft.Panel2.ResumeLayout(false);
            this.splitContainerLeft.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).EndInit();
            this.splitContainerLeft.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsMenuNewProject;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOpenProject;
        private ImagePanel panelImage;
        private System.Windows.Forms.ComboBox cboLOD;
        private System.Windows.Forms.Button btnAddLOD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFolderPath;
        private System.Windows.Forms.Button btnFolderPath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblImageHeight;
        private System.Windows.Forms.Label lblImageWidth;
        private System.Windows.Forms.Label lblImageVCount;
        private System.Windows.Forms.Label lblImageHCount;
        private System.Windows.Forms.ToolStripMenuItem tsMenuSave;
        private System.Windows.Forms.Button btnRemoveLOD;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblImageTotalHeight;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblImageTotalWidth;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cboAddPixel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnArrangeLOD;
        private System.Windows.Forms.SplitContainer splitContainerBody;
        private System.Windows.Forms.SplitContainer splitContainerLeft;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cboShapeLOD;
        private System.Windows.Forms.TextBox textBoxShapeFilePath;
        private System.Windows.Forms.Button btnShapeFilePath;
        private System.Windows.Forms.CheckBox checkBoxAllLod;
        private System.Windows.Forms.ComboBox cboShapes;
        private System.Windows.Forms.TextBox textBoxShapeName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnAddShape;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxShapeY;
        private System.Windows.Forms.TextBox textBoxShapeX;
        private System.Windows.Forms.Button btnRemoveShape;
        private System.Windows.Forms.Button btnApplyShape;
        private System.Windows.Forms.Button btnRenameShape;
    }
}

