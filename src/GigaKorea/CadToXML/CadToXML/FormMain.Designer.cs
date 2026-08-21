namespace CadToXML
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
            UnE.Geometry.Vertex2D vertex2D1 = new UnE.Geometry.Vertex2D();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDXFPath = new System.Windows.Forms.TextBox();
            this.btnOpenDXF = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDoorHeight = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxDoorElevation = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxWindowHeight = new System.Windows.Forms.TextBox();
            this.textBoxWindowElevation = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboXMLUnit = new System.Windows.Forms.ComboBox();
            this.cboDXFUnit = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.cmbDoorPrtyYN = new System.Windows.Forms.ComboBox();
            this.labelUnitWindowHeight = new System.Windows.Forms.Label();
            this.cmbColumnMaterial = new System.Windows.Forms.ComboBox();
            this.labelUnitWindowElevation = new System.Windows.Forms.Label();
            this.labelUnitDoorElevation = new System.Windows.Forms.Label();
            this.labelUnitDoorHeight = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxXMLPath = new System.Windows.Forms.TextBox();
            this.btnSaveXML = new System.Windows.Forms.Button();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.textBoxProjectName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxAuthor = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBoxRemember = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbSwallPrtyFinMaterial = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cmbSwallPrtyMaterial = new System.Windows.Forms.ComboBox();
            this.panelStructureColor = new System.Windows.Forms.Panel();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.labelUnitWallSThick = new System.Windows.Forms.Label();
            this.labelUnitWallSHeight = new System.Windows.Forms.Label();
            this.textBoxWallSThick = new System.Windows.Forms.TextBox();
            this.textBoxWallSHeight = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbFwallPrtyFinMaterial = new System.Windows.Forms.ComboBox();
            this.panelFakeColor = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.cmbFwallPrtyMaterial = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.textBoxWallFThick = new System.Windows.Forms.TextBox();
            this.labelUnitWallFThick = new System.Windows.Forms.Label();
            this.labelUnitWallFHeight = new System.Windows.Forms.Label();
            this.textBoxWallFHeight = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.panelPartitionColor = new System.Windows.Forms.Panel();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.textBoxWallPThick = new System.Windows.Forms.TextBox();
            this.labelUnitWallPThick = new System.Windows.Forms.Label();
            this.labelUnitWallPHeight = new System.Windows.Forms.Label();
            this.textBoxWallPHeight = new System.Windows.Forms.TextBox();
            this.textBoxWallPMaterial = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.dxfControl = new DXFViewer.DXFControl();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.labelFloorIndex = new System.Windows.Forms.Label();
            this.checkBoxDXFNameToProjectName = new System.Windows.Forms.CheckBox();
            this.labelCoord = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cmbHwallMaterial = new System.Windows.Forms.ComboBox();
            this.panelHandrailColor = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxWallHThick = new System.Windows.Forms.TextBox();
            this.labelUnitWallHThick = new System.Windows.Forms.Label();
            this.labelUnitWallHHeight = new System.Windows.Forms.Label();
            this.textBoxWallHHeight = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.grpColumn = new System.Windows.Forms.GroupBox();
            this.grpDoor = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnExportIndoorGML = new System.Windows.Forms.Button();
            this.grpLevelHeight = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.labelUnitFloorHeight = new System.Windows.Forms.Label();
            this.txtLevelHeight = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.cboGlobalUnit = new System.Windows.Forms.ComboBox();
            this.checkBoxUseAnchorNode = new System.Windows.Forms.CheckBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.textBoxLocal = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxAngle = new System.Windows.Forms.TextBox();
            this.textBoxGlobal = new System.Windows.Forms.TextBox();
            this.checkBoxWallCenterLine = new System.Windows.Forms.CheckBox();
            this.checkBoxSpace = new System.Windows.Forms.CheckBox();
            this.checkBoxWallBoundary = new System.Windows.Forms.CheckBox();
            this.btnWallBoundaryColor = new System.Windows.Forms.Button();
            this.btnSpaceColor = new System.Windows.Forms.Button();
            this.btnChangeXML = new System.Windows.Forms.Button();
            this.checkBoxTopologyNode = new System.Windows.Forms.CheckBox();
            this.btnTopologyNodeColor = new System.Windows.Forms.Button();
            this.checkBoxTopologyLink = new System.Windows.Forms.CheckBox();
            this.btnTopologyLinkColor = new System.Windows.Forms.Button();
            this.btnAlertAreaColor = new System.Windows.Forms.Button();
            this.checkBoxAlertArea = new System.Windows.Forms.CheckBox();
            this.btnTransfer2nd = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.grpColumn.SuspendLayout();
            this.grpDoor.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.grpLevelHeight.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "DXF 경로 :";
            // 
            // textBoxDXFPath
            // 
            this.textBoxDXFPath.Location = new System.Drawing.Point(98, 40);
            this.textBoxDXFPath.Name = "textBoxDXFPath";
            this.textBoxDXFPath.Size = new System.Drawing.Size(342, 21);
            this.textBoxDXFPath.TabIndex = 0;
            // 
            // btnOpenDXF
            // 
            this.btnOpenDXF.Location = new System.Drawing.Point(446, 39);
            this.btnOpenDXF.Name = "btnOpenDXF";
            this.btnOpenDXF.Size = new System.Drawing.Size(33, 23);
            this.btnOpenDXF.TabIndex = 2;
            this.btnOpenDXF.Text = "...";
            this.btnOpenDXF.UseVisualStyleBackColor = true;
            this.btnOpenDXF.Click += new System.EventHandler(this.btnOpenDXF_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(135, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "문 높이 :";
            // 
            // textBoxDoorHeight
            // 
            this.textBoxDoorHeight.Location = new System.Drawing.Point(197, 14);
            this.textBoxDoorHeight.Name = "textBoxDoorHeight";
            this.textBoxDoorHeight.Size = new System.Drawing.Size(37, 21);
            this.textBoxDoorHeight.TabIndex = 3;
            this.textBoxDoorHeight.Text = "2000";
            this.textBoxDoorHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(165, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "벽체하단으로부터의 문 위치 :";
            // 
            // textBoxDoorElevation
            // 
            this.textBoxDoorElevation.Location = new System.Drawing.Point(196, 46);
            this.textBoxDoorElevation.Name = "textBoxDoorElevation";
            this.textBoxDoorElevation.Size = new System.Drawing.Size(37, 21);
            this.textBoxDoorElevation.TabIndex = 5;
            this.textBoxDoorElevation.Text = "0";
            this.textBoxDoorElevation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(123, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "창문 높이 :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(177, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "벽체하단으로부터의 창문 위치 :";
            // 
            // textBoxWindowHeight
            // 
            this.textBoxWindowHeight.Location = new System.Drawing.Point(202, 21);
            this.textBoxWindowHeight.Name = "textBoxWindowHeight";
            this.textBoxWindowHeight.Size = new System.Drawing.Size(32, 21);
            this.textBoxWindowHeight.TabIndex = 7;
            this.textBoxWindowHeight.Text = "1000";
            this.textBoxWindowHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxWindowElevation
            // 
            this.textBoxWindowElevation.Location = new System.Drawing.Point(197, 51);
            this.textBoxWindowElevation.Name = "textBoxWindowElevation";
            this.textBoxWindowElevation.Size = new System.Drawing.Size(37, 21);
            this.textBoxWindowElevation.TabIndex = 8;
            this.textBoxWindowElevation.Text = "1500";
            this.textBoxWindowElevation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboXMLUnit);
            this.groupBox1.Controls.Add(this.cboDXFUnit);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(11, 151);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(196, 86);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "단위계";
            // 
            // cboXMLUnit
            // 
            this.cboXMLUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboXMLUnit.FormattingEnabled = true;
            this.cboXMLUnit.Items.AddRange(new object[] {
            "mm",
            "cm",
            "미터"});
            this.cboXMLUnit.Location = new System.Drawing.Point(107, 54);
            this.cboXMLUnit.Name = "cboXMLUnit";
            this.cboXMLUnit.Size = new System.Drawing.Size(55, 20);
            this.cboXMLUnit.TabIndex = 7;
            this.cboXMLUnit.SelectedIndexChanged += new System.EventHandler(this.cboXMLUnit_SelectedIndexChanged);
            // 
            // cboDXFUnit
            // 
            this.cboDXFUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDXFUnit.FormattingEnabled = true;
            this.cboDXFUnit.Items.AddRange(new object[] {
            "mm",
            "cm",
            "미터"});
            this.cboDXFUnit.Location = new System.Drawing.Point(107, 21);
            this.cboDXFUnit.Name = "cboDXFUnit";
            this.cboDXFUnit.Size = new System.Drawing.Size(55, 20);
            this.cboDXFUnit.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(31, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "xml 단위계 :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "dxf 단위계 :";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(27, 26);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(37, 12);
            this.label28.TabIndex = 16;
            this.label28.Text = "재질 :";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(60, 79);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(93, 12);
            this.label27.TabIndex = 15;
            this.label27.Text = "방화문유무속성:";
            // 
            // cmbDoorPrtyYN
            // 
            this.cmbDoorPrtyYN.FormattingEnabled = true;
            this.cmbDoorPrtyYN.Items.AddRange(new object[] {
            "아니오",
            "예"});
            this.cmbDoorPrtyYN.Location = new System.Drawing.Point(157, 77);
            this.cmbDoorPrtyYN.Name = "cmbDoorPrtyYN";
            this.cmbDoorPrtyYN.Size = new System.Drawing.Size(76, 20);
            this.cmbDoorPrtyYN.TabIndex = 14;
            // 
            // labelUnitWindowHeight
            // 
            this.labelUnitWindowHeight.AutoSize = true;
            this.labelUnitWindowHeight.Location = new System.Drawing.Point(235, 24);
            this.labelUnitWindowHeight.Name = "labelUnitWindowHeight";
            this.labelUnitWindowHeight.Size = new System.Drawing.Size(23, 12);
            this.labelUnitWindowHeight.TabIndex = 7;
            this.labelUnitWindowHeight.Text = "cm";
            // 
            // cmbColumnMaterial
            // 
            this.cmbColumnMaterial.FormattingEnabled = true;
            this.cmbColumnMaterial.Items.AddRange(new object[] {
            "콘크리트",
            "철근",
            "목재"});
            this.cmbColumnMaterial.Location = new System.Drawing.Point(70, 20);
            this.cmbColumnMaterial.Name = "cmbColumnMaterial";
            this.cmbColumnMaterial.Size = new System.Drawing.Size(97, 20);
            this.cmbColumnMaterial.TabIndex = 13;
            // 
            // labelUnitWindowElevation
            // 
            this.labelUnitWindowElevation.AutoSize = true;
            this.labelUnitWindowElevation.Location = new System.Drawing.Point(235, 54);
            this.labelUnitWindowElevation.Name = "labelUnitWindowElevation";
            this.labelUnitWindowElevation.Size = new System.Drawing.Size(23, 12);
            this.labelUnitWindowElevation.TabIndex = 7;
            this.labelUnitWindowElevation.Text = "cm";
            // 
            // labelUnitDoorElevation
            // 
            this.labelUnitDoorElevation.AutoSize = true;
            this.labelUnitDoorElevation.Location = new System.Drawing.Point(235, 49);
            this.labelUnitDoorElevation.Name = "labelUnitDoorElevation";
            this.labelUnitDoorElevation.Size = new System.Drawing.Size(23, 12);
            this.labelUnitDoorElevation.TabIndex = 7;
            this.labelUnitDoorElevation.Text = "cm";
            // 
            // labelUnitDoorHeight
            // 
            this.labelUnitDoorHeight.AutoSize = true;
            this.labelUnitDoorHeight.Location = new System.Drawing.Point(235, 18);
            this.labelUnitDoorHeight.Name = "labelUnitDoorHeight";
            this.labelUnitDoorHeight.Size = new System.Drawing.Size(23, 12);
            this.labelUnitDoorHeight.TabIndex = 7;
            this.labelUnitDoorHeight.Text = "cm";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "저장 파일 :";
            // 
            // textBoxXMLPath
            // 
            this.textBoxXMLPath.Location = new System.Drawing.Point(98, 71);
            this.textBoxXMLPath.Name = "textBoxXMLPath";
            this.textBoxXMLPath.Size = new System.Drawing.Size(283, 21);
            this.textBoxXMLPath.TabIndex = 1;
            // 
            // btnSaveXML
            // 
            this.btnSaveXML.Location = new System.Drawing.Point(446, 70);
            this.btnSaveXML.Name = "btnSaveXML";
            this.btnSaveXML.Size = new System.Drawing.Size(33, 23);
            this.btnSaveXML.TabIndex = 2;
            this.btnSaveXML.Text = "...";
            this.btnSaveXML.UseVisualStyleBackColor = true;
            this.btnSaveXML.Click += new System.EventHandler(this.btnSaveXML_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Location = new System.Drawing.Point(396, 440);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(75, 23);
            this.btnTransfer.TabIndex = 5;
            this.btnTransfer.Text = "변환";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // textBoxProjectName
            // 
            this.textBoxProjectName.Location = new System.Drawing.Point(123, 102);
            this.textBoxProjectName.Name = "textBoxProjectName";
            this.textBoxProjectName.Size = new System.Drawing.Size(103, 21);
            this.textBoxProjectName.TabIndex = 2;
            this.textBoxProjectName.Text = "테스트 프로젝트";
            this.textBoxProjectName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(28, 105);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(89, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "프로젝트 이름 :";
            // 
            // textBoxAuthor
            // 
            this.textBoxAuthor.Location = new System.Drawing.Point(297, 102);
            this.textBoxAuthor.Name = "textBoxAuthor";
            this.textBoxAuthor.Size = new System.Drawing.Size(84, 21);
            this.textBoxAuthor.TabIndex = 3;
            this.textBoxAuthor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(242, 105);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(49, 12);
            this.label13.TabIndex = 0;
            this.label13.Text = "작성자 :";
            // 
            // checkBoxRemember
            // 
            this.checkBoxRemember.AutoSize = true;
            this.checkBoxRemember.Location = new System.Drawing.Point(390, 74);
            this.checkBoxRemember.Name = "checkBoxRemember";
            this.checkBoxRemember.Size = new System.Drawing.Size(48, 16);
            this.checkBoxRemember.TabIndex = 6;
            this.checkBoxRemember.Text = "기억";
            this.checkBoxRemember.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbSwallPrtyFinMaterial);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.cmbSwallPrtyMaterial);
            this.groupBox2.Controls.Add(this.panelStructureColor);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Controls.Add(this.labelUnitWallSThick);
            this.groupBox2.Controls.Add(this.labelUnitWallSHeight);
            this.groupBox2.Controls.Add(this.textBoxWallSThick);
            this.groupBox2.Controls.Add(this.textBoxWallSHeight);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Location = new System.Drawing.Point(485, 41);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 103);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "구조벽";
            // 
            // cmbSwallPrtyFinMaterial
            // 
            this.cmbSwallPrtyFinMaterial.FormattingEnabled = true;
            this.cmbSwallPrtyFinMaterial.Items.AddRange(new object[] {
            "페인트"});
            this.cmbSwallPrtyFinMaterial.Location = new System.Drawing.Point(73, 70);
            this.cmbSwallPrtyFinMaterial.Name = "cmbSwallPrtyFinMaterial";
            this.cmbSwallPrtyFinMaterial.Size = new System.Drawing.Size(75, 20);
            this.cmbSwallPrtyFinMaterial.TabIndex = 12;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(20, 78);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(49, 12);
            this.label14.TabIndex = 11;
            this.label14.Text = "마감재 :";
            // 
            // cmbSwallPrtyMaterial
            // 
            this.cmbSwallPrtyMaterial.FormattingEnabled = true;
            this.cmbSwallPrtyMaterial.Items.AddRange(new object[] {
            "콘크리트",
            "철근",
            "목재"});
            this.cmbSwallPrtyMaterial.Location = new System.Drawing.Point(73, 46);
            this.cmbSwallPrtyMaterial.Name = "cmbSwallPrtyMaterial";
            this.cmbSwallPrtyMaterial.Size = new System.Drawing.Size(75, 20);
            this.cmbSwallPrtyMaterial.TabIndex = 10;
            // 
            // panelStructureColor
            // 
            this.panelStructureColor.BackColor = System.Drawing.Color.Yellow;
            this.panelStructureColor.Location = new System.Drawing.Point(221, 52);
            this.panelStructureColor.Name = "panelStructureColor";
            this.panelStructureColor.Size = new System.Drawing.Size(37, 21);
            this.panelStructureColor.TabIndex = 8;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(178, 24);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(37, 12);
            this.label23.TabIndex = 0;
            this.label23.Text = "두께 :";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(20, 24);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(37, 12);
            this.label24.TabIndex = 0;
            this.label24.Text = "높이 :";
            // 
            // labelUnitWallSThick
            // 
            this.labelUnitWallSThick.AutoSize = true;
            this.labelUnitWallSThick.Location = new System.Drawing.Point(258, 24);
            this.labelUnitWallSThick.Name = "labelUnitWallSThick";
            this.labelUnitWallSThick.Size = new System.Drawing.Size(23, 12);
            this.labelUnitWallSThick.TabIndex = 7;
            this.labelUnitWallSThick.Text = "cm";
            // 
            // labelUnitWallSHeight
            // 
            this.labelUnitWallSHeight.AutoSize = true;
            this.labelUnitWallSHeight.Location = new System.Drawing.Point(148, 24);
            this.labelUnitWallSHeight.Name = "labelUnitWallSHeight";
            this.labelUnitWallSHeight.Size = new System.Drawing.Size(23, 12);
            this.labelUnitWallSHeight.TabIndex = 7;
            this.labelUnitWallSHeight.Text = "cm";
            // 
            // textBoxWallSThick
            // 
            this.textBoxWallSThick.Location = new System.Drawing.Point(221, 21);
            this.textBoxWallSThick.Name = "textBoxWallSThick";
            this.textBoxWallSThick.Size = new System.Drawing.Size(37, 21);
            this.textBoxWallSThick.TabIndex = 1;
            this.textBoxWallSThick.Text = "250";
            this.textBoxWallSThick.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxWallSHeight
            // 
            this.textBoxWallSHeight.Location = new System.Drawing.Point(111, 21);
            this.textBoxWallSHeight.Name = "textBoxWallSHeight";
            this.textBoxWallSHeight.Size = new System.Drawing.Size(37, 21);
            this.textBoxWallSHeight.TabIndex = 0;
            this.textBoxWallSHeight.Text = "2600";
            this.textBoxWallSHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(20, 50);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(37, 12);
            this.label17.TabIndex = 0;
            this.label17.Text = "재질 :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbFwallPrtyFinMaterial);
            this.groupBox3.Controls.Add(this.panelFakeColor);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.cmbFwallPrtyMaterial);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.textBoxWallFThick);
            this.groupBox3.Controls.Add(this.labelUnitWallFThick);
            this.groupBox3.Controls.Add(this.labelUnitWallFHeight);
            this.groupBox3.Controls.Add(this.textBoxWallFHeight);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Location = new System.Drawing.Point(485, 151);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(300, 103);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "가벽";
            // 
            // cmbFwallPrtyFinMaterial
            // 
            this.cmbFwallPrtyFinMaterial.FormattingEnabled = true;
            this.cmbFwallPrtyFinMaterial.Items.AddRange(new object[] {
            "페인트"});
            this.cmbFwallPrtyFinMaterial.Location = new System.Drawing.Point(71, 73);
            this.cmbFwallPrtyFinMaterial.Name = "cmbFwallPrtyFinMaterial";
            this.cmbFwallPrtyFinMaterial.Size = new System.Drawing.Size(75, 20);
            this.cmbFwallPrtyFinMaterial.TabIndex = 16;
            // 
            // panelFakeColor
            // 
            this.panelFakeColor.BackColor = System.Drawing.Color.Lime;
            this.panelFakeColor.Location = new System.Drawing.Point(221, 51);
            this.panelFakeColor.Name = "panelFakeColor";
            this.panelFakeColor.Size = new System.Drawing.Size(37, 21);
            this.panelFakeColor.TabIndex = 8;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(22, 76);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(49, 12);
            this.label16.TabIndex = 15;
            this.label16.Text = "마감재 :";
            // 
            // cmbFwallPrtyMaterial
            // 
            this.cmbFwallPrtyMaterial.FormattingEnabled = true;
            this.cmbFwallPrtyMaterial.Items.AddRange(new object[] {
            "콘크리트",
            "철근",
            "목재",
            "벽돌조"});
            this.cmbFwallPrtyMaterial.Location = new System.Drawing.Point(71, 48);
            this.cmbFwallPrtyMaterial.Name = "cmbFwallPrtyMaterial";
            this.cmbFwallPrtyMaterial.Size = new System.Drawing.Size(75, 20);
            this.cmbFwallPrtyMaterial.TabIndex = 14;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(178, 27);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(37, 12);
            this.label18.TabIndex = 0;
            this.label18.Text = "두께 :";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(22, 27);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(37, 12);
            this.label19.TabIndex = 0;
            this.label19.Text = "높이 :";
            // 
            // textBoxWallFThick
            // 
            this.textBoxWallFThick.Location = new System.Drawing.Point(221, 20);
            this.textBoxWallFThick.Name = "textBoxWallFThick";
            this.textBoxWallFThick.Size = new System.Drawing.Size(37, 21);
            this.textBoxWallFThick.TabIndex = 1;
            this.textBoxWallFThick.Text = "150";
            this.textBoxWallFThick.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelUnitWallFThick
            // 
            this.labelUnitWallFThick.AutoSize = true;
            this.labelUnitWallFThick.Location = new System.Drawing.Point(258, 22);
            this.labelUnitWallFThick.Name = "labelUnitWallFThick";
            this.labelUnitWallFThick.Size = new System.Drawing.Size(23, 12);
            this.labelUnitWallFThick.TabIndex = 7;
            this.labelUnitWallFThick.Text = "cm";
            // 
            // labelUnitWallFHeight
            // 
            this.labelUnitWallFHeight.AutoSize = true;
            this.labelUnitWallFHeight.Location = new System.Drawing.Point(148, 27);
            this.labelUnitWallFHeight.Name = "labelUnitWallFHeight";
            this.labelUnitWallFHeight.Size = new System.Drawing.Size(23, 12);
            this.labelUnitWallFHeight.TabIndex = 7;
            this.labelUnitWallFHeight.Text = "cm";
            // 
            // textBoxWallFHeight
            // 
            this.textBoxWallFHeight.Location = new System.Drawing.Point(109, 23);
            this.textBoxWallFHeight.Name = "textBoxWallFHeight";
            this.textBoxWallFHeight.Size = new System.Drawing.Size(37, 21);
            this.textBoxWallFHeight.TabIndex = 0;
            this.textBoxWallFHeight.Text = "2600";
            this.textBoxWallFHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(22, 51);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(37, 12);
            this.label20.TabIndex = 0;
            this.label20.Text = "재질 :";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.panelPartitionColor);
            this.groupBox4.Controls.Add(this.label21);
            this.groupBox4.Controls.Add(this.label22);
            this.groupBox4.Controls.Add(this.textBoxWallPThick);
            this.groupBox4.Controls.Add(this.labelUnitWallPThick);
            this.groupBox4.Controls.Add(this.labelUnitWallPHeight);
            this.groupBox4.Controls.Add(this.textBoxWallPHeight);
            this.groupBox4.Controls.Add(this.textBoxWallPMaterial);
            this.groupBox4.Controls.Add(this.label25);
            this.groupBox4.Location = new System.Drawing.Point(485, 349);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(300, 86);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "파티션";
            // 
            // panelPartitionColor
            // 
            this.panelPartitionColor.BackColor = System.Drawing.Color.Silver;
            this.panelPartitionColor.Location = new System.Drawing.Point(221, 48);
            this.panelPartitionColor.Name = "panelPartitionColor";
            this.panelPartitionColor.Size = new System.Drawing.Size(37, 21);
            this.panelPartitionColor.TabIndex = 8;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(22, 54);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(37, 12);
            this.label21.TabIndex = 0;
            this.label21.Text = "두께 :";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(22, 27);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(37, 12);
            this.label22.TabIndex = 0;
            this.label22.Text = "높이 :";
            // 
            // textBoxWallPThick
            // 
            this.textBoxWallPThick.Location = new System.Drawing.Point(109, 51);
            this.textBoxWallPThick.Name = "textBoxWallPThick";
            this.textBoxWallPThick.ReadOnly = true;
            this.textBoxWallPThick.Size = new System.Drawing.Size(37, 21);
            this.textBoxWallPThick.TabIndex = 1;
            this.textBoxWallPThick.Text = "0";
            this.textBoxWallPThick.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelUnitWallPThick
            // 
            this.labelUnitWallPThick.AutoSize = true;
            this.labelUnitWallPThick.Location = new System.Drawing.Point(148, 57);
            this.labelUnitWallPThick.Name = "labelUnitWallPThick";
            this.labelUnitWallPThick.Size = new System.Drawing.Size(23, 12);
            this.labelUnitWallPThick.TabIndex = 7;
            this.labelUnitWallPThick.Text = "cm";
            // 
            // labelUnitWallPHeight
            // 
            this.labelUnitWallPHeight.AutoSize = true;
            this.labelUnitWallPHeight.Location = new System.Drawing.Point(148, 26);
            this.labelUnitWallPHeight.Name = "labelUnitWallPHeight";
            this.labelUnitWallPHeight.Size = new System.Drawing.Size(23, 12);
            this.labelUnitWallPHeight.TabIndex = 7;
            this.labelUnitWallPHeight.Text = "cm";
            // 
            // textBoxWallPHeight
            // 
            this.textBoxWallPHeight.Location = new System.Drawing.Point(109, 21);
            this.textBoxWallPHeight.Name = "textBoxWallPHeight";
            this.textBoxWallPHeight.ReadOnly = true;
            this.textBoxWallPHeight.Size = new System.Drawing.Size(37, 21);
            this.textBoxWallPHeight.TabIndex = 0;
            this.textBoxWallPHeight.Text = "0";
            this.textBoxWallPHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxWallPMaterial
            // 
            this.textBoxWallPMaterial.Location = new System.Drawing.Point(73, 114);
            this.textBoxWallPMaterial.Name = "textBoxWallPMaterial";
            this.textBoxWallPMaterial.Size = new System.Drawing.Size(103, 21);
            this.textBoxWallPMaterial.TabIndex = 2;
            this.textBoxWallPMaterial.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 115);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(65, 12);
            this.label25.TabIndex = 0;
            this.label25.Text = "벽체 재질 :";
            // 
            // dxfControl
            // 
            this.dxfControl.AntiAliasing = true;
            this.dxfControl.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dxfControl.DrawHatchFirst = true;
            this.dxfControl.ExternalPainter = null;
            this.dxfControl.GroupItemDistance = 30;
            this.dxfControl.GroupItemMinCount = 3;
            this.dxfControl.Location = new System.Drawing.Point(791, 43);
            this.dxfControl.MinimumSize = new System.Drawing.Size(100, 100);
            this.dxfControl.MovedVertex = vertex2D1;
            this.dxfControl.Name = "dxfControl";
            this.dxfControl.ObjectBR = null;
            this.dxfControl.ObjectTL = null;
            this.dxfControl.OpenNRefresh = true;
            this.dxfControl.Panning = false;
            this.dxfControl.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.dxfControl.PrintDocument = null;
            this.dxfControl.Renderer = DXFViewer.IPainter.RendererType.GDI_PLUS;
            this.dxfControl.Size = new System.Drawing.Size(584, 397);
            this.dxfControl.TabIndex = 7;
            this.dxfControl.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.dxfControl.UseGroupItem = false;
            this.dxfControl.UseLastViewport = false;
            this.dxfControl.UseMouseWheel = true;
            this.dxfControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dxfControl_MouseDown);
            this.dxfControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dxfControl_MouseMove);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(75, 440);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(34, 23);
            this.btnPrev.TabIndex = 9;
            this.btnPrev.Text = "<-";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(139, 440);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(34, 23);
            this.btnNext.TabIndex = 9;
            this.btnNext.Text = "->";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // labelFloorIndex
            // 
            this.labelFloorIndex.AutoSize = true;
            this.labelFloorIndex.Location = new System.Drawing.Point(114, 445);
            this.labelFloorIndex.Name = "labelFloorIndex";
            this.labelFloorIndex.Size = new System.Drawing.Size(17, 12);
            this.labelFloorIndex.TabIndex = 10;
            this.labelFloorIndex.Text = "층";
            // 
            // checkBoxDXFNameToProjectName
            // 
            this.checkBoxDXFNameToProjectName.AutoSize = true;
            this.checkBoxDXFNameToProjectName.Location = new System.Drawing.Point(124, 130);
            this.checkBoxDXFNameToProjectName.Name = "checkBoxDXFNameToProjectName";
            this.checkBoxDXFNameToProjectName.Size = new System.Drawing.Size(231, 16);
            this.checkBoxDXFNameToProjectName.TabIndex = 11;
            this.checkBoxDXFNameToProjectName.Text = "DXF 파일명을 프로젝트 이름으로 사용";
            this.checkBoxDXFNameToProjectName.UseVisualStyleBackColor = true;
            // 
            // labelCoord
            // 
            this.labelCoord.AutoSize = true;
            this.labelCoord.Location = new System.Drawing.Point(791, 451);
            this.labelCoord.Name = "labelCoord";
            this.labelCoord.Size = new System.Drawing.Size(56, 12);
            this.labelCoord.TabIndex = 12;
            this.labelCoord.Text = "DXF 좌표";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cmbHwallMaterial);
            this.groupBox5.Controls.Add(this.panelHandrailColor);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.textBoxWallHThick);
            this.groupBox5.Controls.Add(this.labelUnitWallHThick);
            this.groupBox5.Controls.Add(this.labelUnitWallHHeight);
            this.groupBox5.Controls.Add(this.textBoxWallHHeight);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Location = new System.Drawing.Point(485, 259);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(300, 86);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "난간";
            // 
            // cmbHwallMaterial
            // 
            this.cmbHwallMaterial.FormattingEnabled = true;
            this.cmbHwallMaterial.Items.AddRange(new object[] {
            "철재"});
            this.cmbHwallMaterial.Location = new System.Drawing.Point(71, 56);
            this.cmbHwallMaterial.Name = "cmbHwallMaterial";
            this.cmbHwallMaterial.Size = new System.Drawing.Size(75, 20);
            this.cmbHwallMaterial.TabIndex = 17;
            // 
            // panelHandrailColor
            // 
            this.panelHandrailColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(221)))));
            this.panelHandrailColor.Location = new System.Drawing.Point(221, 55);
            this.panelHandrailColor.Name = "panelHandrailColor";
            this.panelHandrailColor.Size = new System.Drawing.Size(37, 21);
            this.panelHandrailColor.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(178, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "두께 :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(22, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "높이 :";
            // 
            // textBoxWallHThick
            // 
            this.textBoxWallHThick.Location = new System.Drawing.Point(221, 29);
            this.textBoxWallHThick.Name = "textBoxWallHThick";
            this.textBoxWallHThick.Size = new System.Drawing.Size(37, 21);
            this.textBoxWallHThick.TabIndex = 1;
            this.textBoxWallHThick.Text = "150";
            this.textBoxWallHThick.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelUnitWallHThick
            // 
            this.labelUnitWallHThick.AutoSize = true;
            this.labelUnitWallHThick.Location = new System.Drawing.Point(258, 31);
            this.labelUnitWallHThick.Name = "labelUnitWallHThick";
            this.labelUnitWallHThick.Size = new System.Drawing.Size(23, 12);
            this.labelUnitWallHThick.TabIndex = 7;
            this.labelUnitWallHThick.Text = "cm";
            // 
            // labelUnitWallHHeight
            // 
            this.labelUnitWallHHeight.AutoSize = true;
            this.labelUnitWallHHeight.Location = new System.Drawing.Point(148, 29);
            this.labelUnitWallHHeight.Name = "labelUnitWallHHeight";
            this.labelUnitWallHHeight.Size = new System.Drawing.Size(23, 12);
            this.labelUnitWallHHeight.TabIndex = 7;
            this.labelUnitWallHHeight.Text = "cm";
            // 
            // textBoxWallHHeight
            // 
            this.textBoxWallHHeight.Location = new System.Drawing.Point(109, 27);
            this.textBoxWallHHeight.Name = "textBoxWallHHeight";
            this.textBoxWallHHeight.Size = new System.Drawing.Size(37, 21);
            this.textBoxWallHHeight.TabIndex = 0;
            this.textBoxWallHHeight.Text = "1200";
            this.textBoxWallHHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(22, 59);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(37, 12);
            this.label15.TabIndex = 0;
            this.label15.Text = "재질 :";
            // 
            // grpColumn
            // 
            this.grpColumn.Controls.Add(this.label28);
            this.grpColumn.Controls.Add(this.cmbColumnMaterial);
            this.grpColumn.Location = new System.Drawing.Point(12, 299);
            this.grpColumn.Name = "grpColumn";
            this.grpColumn.Size = new System.Drawing.Size(195, 57);
            this.grpColumn.TabIndex = 13;
            this.grpColumn.TabStop = false;
            this.grpColumn.Text = "기둥";
            // 
            // grpDoor
            // 
            this.grpDoor.Controls.Add(this.label27);
            this.grpDoor.Controls.Add(this.cmbDoorPrtyYN);
            this.grpDoor.Controls.Add(this.textBoxDoorHeight);
            this.grpDoor.Controls.Add(this.label4);
            this.grpDoor.Controls.Add(this.textBoxDoorElevation);
            this.grpDoor.Controls.Add(this.labelUnitDoorElevation);
            this.grpDoor.Controls.Add(this.label3);
            this.grpDoor.Controls.Add(this.labelUnitDoorHeight);
            this.grpDoor.Location = new System.Drawing.Point(213, 151);
            this.grpDoor.Name = "grpDoor";
            this.grpDoor.Size = new System.Drawing.Size(266, 103);
            this.grpDoor.TabIndex = 14;
            this.grpDoor.TabStop = false;
            this.grpDoor.Text = "문";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.labelUnitWindowElevation);
            this.groupBox6.Controls.Add(this.labelUnitWindowHeight);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.textBoxWindowHeight);
            this.groupBox6.Controls.Add(this.textBoxWindowElevation);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Location = new System.Drawing.Point(213, 259);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(266, 97);
            this.groupBox6.TabIndex = 15;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "창문";
            // 
            // btnExportIndoorGML
            // 
            this.btnExportIndoorGML.Location = new System.Drawing.Point(315, 440);
            this.btnExportIndoorGML.Name = "btnExportIndoorGML";
            this.btnExportIndoorGML.Size = new System.Drawing.Size(75, 23);
            this.btnExportIndoorGML.TabIndex = 5;
            this.btnExportIndoorGML.Text = "IndoorGML";
            this.btnExportIndoorGML.UseVisualStyleBackColor = true;
            this.btnExportIndoorGML.Click += new System.EventHandler(this.btnExportIndoorGML_Click);
            // 
            // grpLevelHeight
            // 
            this.grpLevelHeight.Controls.Add(this.label26);
            this.grpLevelHeight.Controls.Add(this.labelUnitFloorHeight);
            this.grpLevelHeight.Controls.Add(this.txtLevelHeight);
            this.grpLevelHeight.Location = new System.Drawing.Point(11, 242);
            this.grpLevelHeight.Name = "grpLevelHeight";
            this.grpLevelHeight.Size = new System.Drawing.Size(196, 50);
            this.grpLevelHeight.TabIndex = 16;
            this.grpLevelHeight.TabStop = false;
            this.grpLevelHeight.Text = "층높이";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(26, 26);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(49, 12);
            this.label26.TabIndex = 8;
            this.label26.Text = "층높이 :";
            // 
            // labelUnitFloorHeight
            // 
            this.labelUnitFloorHeight.AutoSize = true;
            this.labelUnitFloorHeight.Location = new System.Drawing.Point(164, 25);
            this.labelUnitFloorHeight.Name = "labelUnitFloorHeight";
            this.labelUnitFloorHeight.Size = new System.Drawing.Size(23, 12);
            this.labelUnitFloorHeight.TabIndex = 16;
            this.labelUnitFloorHeight.Text = "cm";
            // 
            // txtLevelHeight
            // 
            this.txtLevelHeight.Location = new System.Drawing.Point(107, 20);
            this.txtLevelHeight.Name = "txtLevelHeight";
            this.txtLevelHeight.Size = new System.Drawing.Size(55, 21);
            this.txtLevelHeight.TabIndex = 16;
            this.txtLevelHeight.Text = "2600";
            this.txtLevelHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.cboGlobalUnit);
            this.groupBox7.Controls.Add(this.checkBoxUseAnchorNode);
            this.groupBox7.Controls.Add(this.label32);
            this.groupBox7.Controls.Add(this.label29);
            this.groupBox7.Controls.Add(this.textBoxLocal);
            this.groupBox7.Controls.Add(this.label31);
            this.groupBox7.Controls.Add(this.label30);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.textBoxAngle);
            this.groupBox7.Controls.Add(this.textBoxGlobal);
            this.groupBox7.Location = new System.Drawing.Point(12, 361);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(467, 74);
            this.groupBox7.TabIndex = 16;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "앵커노드";
            // 
            // cboGlobalUnit
            // 
            this.cboGlobalUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGlobalUnit.FormattingEnabled = true;
            this.cboGlobalUnit.Items.AddRange(new object[] {
            "mm",
            "cm",
            "미터"});
            this.cboGlobalUnit.Location = new System.Drawing.Point(394, 24);
            this.cboGlobalUnit.Name = "cboGlobalUnit";
            this.cboGlobalUnit.Size = new System.Drawing.Size(55, 20);
            this.cboGlobalUnit.TabIndex = 7;
            this.cboGlobalUnit.SelectedIndexChanged += new System.EventHandler(this.cboXMLUnit_SelectedIndexChanged);
            // 
            // checkBoxUseAnchorNode
            // 
            this.checkBoxUseAnchorNode.AutoSize = true;
            this.checkBoxUseAnchorNode.Location = new System.Drawing.Point(14, 22);
            this.checkBoxUseAnchorNode.Name = "checkBoxUseAnchorNode";
            this.checkBoxUseAnchorNode.Size = new System.Drawing.Size(72, 16);
            this.checkBoxUseAnchorNode.TabIndex = 17;
            this.checkBoxUseAnchorNode.Text = "사용여부";
            this.checkBoxUseAnchorNode.UseVisualStyleBackColor = true;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(304, 27);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(87, 12);
            this.label32.TabIndex = 0;
            this.label32.Text = "global 단위계 :";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(148, 48);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(70, 12);
            this.label29.TabIndex = 8;
            this.label29.Text = "dxf 기준점 :";
            // 
            // textBoxLocal
            // 
            this.textBoxLocal.Location = new System.Drawing.Point(232, 45);
            this.textBoxLocal.Name = "textBoxLocal";
            this.textBoxLocal.Size = new System.Drawing.Size(55, 21);
            this.textBoxLocal.TabIndex = 16;
            this.textBoxLocal.Text = "0,0";
            this.textBoxLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(102, 46);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(9, 12);
            this.label31.TabIndex = 8;
            this.label31.Text = "°";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(16, 49);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(49, 12);
            this.label30.TabIndex = 8;
            this.label30.Text = "방위각 :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(136, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(87, 12);
            this.label11.TabIndex = 8;
            this.label11.Text = "global 기준점 :";
            // 
            // textBoxAngle
            // 
            this.textBoxAngle.Location = new System.Drawing.Point(70, 44);
            this.textBoxAngle.Name = "textBoxAngle";
            this.textBoxAngle.Size = new System.Drawing.Size(30, 21);
            this.textBoxAngle.TabIndex = 16;
            this.textBoxAngle.Text = "0";
            this.textBoxAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxGlobal
            // 
            this.textBoxGlobal.Location = new System.Drawing.Point(232, 22);
            this.textBoxGlobal.Name = "textBoxGlobal";
            this.textBoxGlobal.Size = new System.Drawing.Size(55, 21);
            this.textBoxGlobal.TabIndex = 16;
            this.textBoxGlobal.Text = "0,0";
            this.textBoxGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkBoxWallCenterLine
            // 
            this.checkBoxWallCenterLine.AutoSize = true;
            this.checkBoxWallCenterLine.Checked = true;
            this.checkBoxWallCenterLine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWallCenterLine.Location = new System.Drawing.Point(729, 12);
            this.checkBoxWallCenterLine.Name = "checkBoxWallCenterLine";
            this.checkBoxWallCenterLine.Size = new System.Drawing.Size(88, 16);
            this.checkBoxWallCenterLine.TabIndex = 19;
            this.checkBoxWallCenterLine.Text = "벽체 중심선";
            this.checkBoxWallCenterLine.UseVisualStyleBackColor = true;
            this.checkBoxWallCenterLine.CheckedChanged += new System.EventHandler(this.checkBoxLayer_CheckedChanged);
            // 
            // checkBoxSpace
            // 
            this.checkBoxSpace.AutoSize = true;
            this.checkBoxSpace.Location = new System.Drawing.Point(948, 12);
            this.checkBoxSpace.Name = "checkBoxSpace";
            this.checkBoxSpace.Size = new System.Drawing.Size(48, 16);
            this.checkBoxSpace.TabIndex = 20;
            this.checkBoxSpace.Text = "공간";
            this.checkBoxSpace.UseVisualStyleBackColor = true;
            this.checkBoxSpace.CheckedChanged += new System.EventHandler(this.checkBoxLayer_CheckedChanged);
            // 
            // checkBoxWallBoundary
            // 
            this.checkBoxWallBoundary.AutoSize = true;
            this.checkBoxWallBoundary.Checked = true;
            this.checkBoxWallBoundary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWallBoundary.Location = new System.Drawing.Point(825, 12);
            this.checkBoxWallBoundary.Name = "checkBoxWallBoundary";
            this.checkBoxWallBoundary.Size = new System.Drawing.Size(88, 16);
            this.checkBoxWallBoundary.TabIndex = 21;
            this.checkBoxWallBoundary.Text = "벽체 외곽선";
            this.checkBoxWallBoundary.UseVisualStyleBackColor = true;
            this.checkBoxWallBoundary.CheckedChanged += new System.EventHandler(this.checkBoxLayer_CheckedChanged);
            // 
            // btnWallBoundaryColor
            // 
            this.btnWallBoundaryColor.BackColor = System.Drawing.Color.Yellow;
            this.btnWallBoundaryColor.Location = new System.Drawing.Point(910, 9);
            this.btnWallBoundaryColor.Name = "btnWallBoundaryColor";
            this.btnWallBoundaryColor.Size = new System.Drawing.Size(20, 20);
            this.btnWallBoundaryColor.TabIndex = 0;
            this.btnWallBoundaryColor.UseVisualStyleBackColor = false;
            this.btnWallBoundaryColor.Click += new System.EventHandler(this.btnLayerColor_Click);
            // 
            // btnSpaceColor
            // 
            this.btnSpaceColor.BackColor = System.Drawing.Color.Red;
            this.btnSpaceColor.Location = new System.Drawing.Point(993, 9);
            this.btnSpaceColor.Name = "btnSpaceColor";
            this.btnSpaceColor.Size = new System.Drawing.Size(20, 20);
            this.btnSpaceColor.TabIndex = 0;
            this.btnSpaceColor.UseVisualStyleBackColor = false;
            this.btnSpaceColor.Click += new System.EventHandler(this.btnLayerColor_Click);
            // 
            // btnChangeXML
            // 
            this.btnChangeXML.Location = new System.Drawing.Point(635, 440);
            this.btnChangeXML.Name = "btnChangeXML";
            this.btnChangeXML.Size = new System.Drawing.Size(150, 23);
            this.btnChangeXML.TabIndex = 22;
            this.btnChangeXML.Text = "v1.5 이전의 XML 변환";
            this.btnChangeXML.UseVisualStyleBackColor = true;
            this.btnChangeXML.Click += new System.EventHandler(this.btnChangeXML_Click);
            // 
            // checkBoxTopologyNode
            // 
            this.checkBoxTopologyNode.AutoSize = true;
            this.checkBoxTopologyNode.Location = new System.Drawing.Point(1032, 12);
            this.checkBoxTopologyNode.Name = "checkBoxTopologyNode";
            this.checkBoxTopologyNode.Size = new System.Drawing.Size(100, 16);
            this.checkBoxTopologyNode.TabIndex = 20;
            this.checkBoxTopologyNode.Text = "토폴로지 노드";
            this.checkBoxTopologyNode.UseVisualStyleBackColor = true;
            this.checkBoxTopologyNode.CheckedChanged += new System.EventHandler(this.checkBoxLayer_CheckedChanged);
            // 
            // btnTopologyNodeColor
            // 
            this.btnTopologyNodeColor.BackColor = System.Drawing.Color.Cyan;
            this.btnTopologyNodeColor.Location = new System.Drawing.Point(1129, 9);
            this.btnTopologyNodeColor.Name = "btnTopologyNodeColor";
            this.btnTopologyNodeColor.Size = new System.Drawing.Size(20, 20);
            this.btnTopologyNodeColor.TabIndex = 0;
            this.btnTopologyNodeColor.UseVisualStyleBackColor = false;
            this.btnTopologyNodeColor.Click += new System.EventHandler(this.btnLayerColor_Click);
            // 
            // checkBoxTopologyLink
            // 
            this.checkBoxTopologyLink.AutoSize = true;
            this.checkBoxTopologyLink.Location = new System.Drawing.Point(1166, 12);
            this.checkBoxTopologyLink.Name = "checkBoxTopologyLink";
            this.checkBoxTopologyLink.Size = new System.Drawing.Size(100, 16);
            this.checkBoxTopologyLink.TabIndex = 20;
            this.checkBoxTopologyLink.Text = "토폴로지 링크";
            this.checkBoxTopologyLink.UseVisualStyleBackColor = true;
            this.checkBoxTopologyLink.CheckedChanged += new System.EventHandler(this.checkBoxLayer_CheckedChanged);
            // 
            // btnTopologyLinkColor
            // 
            this.btnTopologyLinkColor.BackColor = System.Drawing.Color.Cyan;
            this.btnTopologyLinkColor.Location = new System.Drawing.Point(1264, 9);
            this.btnTopologyLinkColor.Name = "btnTopologyLinkColor";
            this.btnTopologyLinkColor.Size = new System.Drawing.Size(20, 20);
            this.btnTopologyLinkColor.TabIndex = 0;
            this.btnTopologyLinkColor.UseVisualStyleBackColor = false;
            this.btnTopologyLinkColor.Click += new System.EventHandler(this.btnLayerColor_Click);
            // 
            // btnAlertAreaColor
            // 
            this.btnAlertAreaColor.BackColor = System.Drawing.Color.Fuchsia;
            this.btnAlertAreaColor.Location = new System.Drawing.Point(1365, 10);
            this.btnAlertAreaColor.Name = "btnAlertAreaColor";
            this.btnAlertAreaColor.Size = new System.Drawing.Size(20, 20);
            this.btnAlertAreaColor.TabIndex = 23;
            this.btnAlertAreaColor.UseVisualStyleBackColor = false;
            // 
            // checkBoxAlertArea
            // 
            this.checkBoxAlertArea.AutoSize = true;
            this.checkBoxAlertArea.Location = new System.Drawing.Point(1298, 13);
            this.checkBoxAlertArea.Name = "checkBoxAlertArea";
            this.checkBoxAlertArea.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAlertArea.TabIndex = 24;
            this.checkBoxAlertArea.Text = "경계구역";
            this.checkBoxAlertArea.UseVisualStyleBackColor = true;
            this.checkBoxAlertArea.CheckedChanged += new System.EventHandler(this.checkBoxLayer_CheckedChanged);
            // 
            // btnTransfer2nd
            // 
            this.btnTransfer2nd.Location = new System.Drawing.Point(485, 440);
            this.btnTransfer2nd.Name = "btnTransfer2nd";
            this.btnTransfer2nd.Size = new System.Drawing.Size(144, 23);
            this.btnTransfer2nd.TabIndex = 25;
            this.btnTransfer2nd.Text = "2차년도 변환";
            this.btnTransfer2nd.UseVisualStyleBackColor = true;
            this.btnTransfer2nd.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1391, 477);
            this.Controls.Add(this.btnTransfer2nd);
            this.Controls.Add(this.btnAlertAreaColor);
            this.Controls.Add(this.checkBoxAlertArea);
            this.Controls.Add(this.btnChangeXML);
            this.Controls.Add(this.btnTopologyLinkColor);
            this.Controls.Add(this.btnTopologyNodeColor);
            this.Controls.Add(this.btnSpaceColor);
            this.Controls.Add(this.btnWallBoundaryColor);
            this.Controls.Add(this.checkBoxWallBoundary);
            this.Controls.Add(this.checkBoxTopologyLink);
            this.Controls.Add(this.checkBoxTopologyNode);
            this.Controls.Add(this.checkBoxSpace);
            this.Controls.Add(this.checkBoxWallCenterLine);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.grpLevelHeight);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.grpDoor);
            this.Controls.Add(this.grpColumn);
            this.Controls.Add(this.labelCoord);
            this.Controls.Add(this.checkBoxDXFNameToProjectName);
            this.Controls.Add(this.labelFloorIndex);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.dxfControl);
            this.Controls.Add(this.checkBoxRemember);
            this.Controls.Add(this.btnExportIndoorGML);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnSaveXML);
            this.Controls.Add(this.btnOpenDXF);
            this.Controls.Add(this.textBoxXMLPath);
            this.Controls.Add(this.textBoxDXFPath);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxAuthor);
            this.Controls.Add(this.textBoxProjectName);
            this.Name = "FormMain";
            this.Text = "Cad 2 XML";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.grpColumn.ResumeLayout(false);
            this.grpColumn.PerformLayout();
            this.grpDoor.ResumeLayout(false);
            this.grpDoor.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.grpLevelHeight.ResumeLayout(false);
            this.grpLevelHeight.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDXFPath;
        private System.Windows.Forms.Button btnOpenDXF;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDoorHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxDoorElevation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxWindowHeight;
        private System.Windows.Forms.TextBox textBoxWindowElevation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxXMLPath;
        private System.Windows.Forms.Button btnSaveXML;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.TextBox textBoxProjectName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxAuthor;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBoxRemember;
        private System.Windows.Forms.ComboBox cboXMLUnit;
        private System.Windows.Forms.ComboBox cboDXFUnit;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox textBoxWallSThick;
        private System.Windows.Forms.TextBox textBoxWallSHeight;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBoxWallFThick;
        private System.Windows.Forms.TextBox textBoxWallFHeight;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox textBoxWallPThick;
        private System.Windows.Forms.TextBox textBoxWallPHeight;
        private System.Windows.Forms.TextBox textBoxWallPMaterial;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label labelUnitDoorHeight;
        private System.Windows.Forms.Label labelUnitWindowHeight;
        private System.Windows.Forms.Label labelUnitWindowElevation;
        private System.Windows.Forms.Label labelUnitDoorElevation;
        private System.Windows.Forms.Label labelUnitWallSThick;
        private System.Windows.Forms.Label labelUnitWallSHeight;
        private System.Windows.Forms.Label labelUnitWallFThick;
        private System.Windows.Forms.Label labelUnitWallFHeight;
        private System.Windows.Forms.Label labelUnitWallPThick;
        private System.Windows.Forms.Label labelUnitWallPHeight;
        private DXFViewer.DXFControl dxfControl;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label labelFloorIndex;
        private System.Windows.Forms.CheckBox checkBoxDXFNameToProjectName;
        private System.Windows.Forms.Label labelCoord;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxWallHThick;
        private System.Windows.Forms.Label labelUnitWallHThick;
        private System.Windows.Forms.Label labelUnitWallHHeight;
        private System.Windows.Forms.TextBox textBoxWallHHeight;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panelStructureColor;
        private System.Windows.Forms.Panel panelFakeColor;
        private System.Windows.Forms.Panel panelPartitionColor;
        private System.Windows.Forms.Panel panelHandrailColor;
        private System.Windows.Forms.ComboBox cmbColumnMaterial;
        private System.Windows.Forms.ComboBox cmbDoorPrtyYN;
        private System.Windows.Forms.ComboBox cmbSwallPrtyFinMaterial;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cmbSwallPrtyMaterial;
        private System.Windows.Forms.ComboBox cmbFwallPrtyFinMaterial;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbFwallPrtyMaterial;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.GroupBox grpColumn;
        private System.Windows.Forms.GroupBox grpDoor;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ComboBox cmbHwallMaterial;
        private System.Windows.Forms.Button btnExportIndoorGML;
        private System.Windows.Forms.GroupBox grpLevelHeight;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label labelUnitFloorHeight;
        private System.Windows.Forms.TextBox txtLevelHeight;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.ComboBox cboGlobalUnit;
        private System.Windows.Forms.CheckBox checkBoxUseAnchorNode;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox textBoxLocal;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxAngle;
        private System.Windows.Forms.TextBox textBoxGlobal;
        private System.Windows.Forms.CheckBox checkBoxWallCenterLine;
        private System.Windows.Forms.CheckBox checkBoxSpace;
        private System.Windows.Forms.CheckBox checkBoxWallBoundary;
        private System.Windows.Forms.Button btnWallBoundaryColor;
        private System.Windows.Forms.Button btnSpaceColor;
        private System.Windows.Forms.Button btnChangeXML;
        private System.Windows.Forms.CheckBox checkBoxTopologyNode;
        private System.Windows.Forms.Button btnTopologyNodeColor;
        private System.Windows.Forms.CheckBox checkBoxTopologyLink;
        private System.Windows.Forms.Button btnTopologyLinkColor;
        private System.Windows.Forms.Button btnAlertAreaColor;
        private System.Windows.Forms.CheckBox checkBoxAlertArea;
        private System.Windows.Forms.Button btnTransfer2nd;
    }
}

