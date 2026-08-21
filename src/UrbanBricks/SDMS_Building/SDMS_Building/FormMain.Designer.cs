namespace SDMS_Building
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
            this.pnTop = new System.Windows.Forms.Panel();
            this.btnTester2 = new UnE.Controls.ColorLabel();
            this.btnTeamEditor2 = new UnE.Controls.ColorLabel();
            this.btnTester = new UnE.GUI.RibbonButton();
            this.btnTeamEditor = new UnE.GUI.RibbonButton();
            this.picUser = new System.Windows.Forms.PictureBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.btnExit = new UnE.GUI.ImageButton();
            this.lblDisasterInfo = new System.Windows.Forms.Label();
            this.pnTabSelect = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnTabReport = new UnE.GUI.ImageButton();
            this.btnTabEdit = new UnE.GUI.ImageButton();
            this.btnTabMonitoring = new UnE.GUI.ImageButton();
            this.picSensorMonitor = new System.Windows.Forms.PictureBox();
            this.btnManagement = new UnE.GUI.ImageButton();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.pnLeft = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.lblZoneName = new UnE.Controls.ColorLabel();
            this.picTreeViewRefresh = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.radioOffSensor = new UnE.GUI.RibbonButton();
            this.label6 = new System.Windows.Forms.Label();
            this.radioAllSensor = new UnE.GUI.RibbonButton();
            this.treeViewAdv1 = new Aga.Controls.Tree.TreeViewAdv();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeStateIcon1 = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
            this.nodeIcon1 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.btnOutdoor = new UnE.GUI.RibbonButton();
            this.pnRight = new System.Windows.Forms.Panel();
            this.dgvOpenDoor = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblDetectCount = new System.Windows.Forms.Label();
            this.picDoorInfo = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnDisaster = new System.Windows.Forms.Panel();
            this.picAlarmLevel = new System.Windows.Forms.PictureBox();
            this.btnMalfunction = new UnE.GUI.RibbonButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnReport = new UnE.GUI.ImageButton();
            this.picDisasterLogo = new System.Windows.Forms.PictureBox();
            this.btnSound = new UnE.GUI.ImageButton();
            this.lblDisasterType = new System.Windows.Forms.Label();
            this.lblDisasterDate = new System.Windows.Forms.Label();
            this.lblDisasterLocation = new System.Windows.Forms.Label();
            this.dgvDetectList = new System.Windows.Forms.DataGridView();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelBody = new System.Windows.Forms.Panel();
            this.pnTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTabReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTabEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTabMonitoring)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSensorMonitor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnManagement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.pnLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTreeViewRefresh)).BeginInit();
            this.pnRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOpenDoor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDoorInfo)).BeginInit();
            this.pnDisaster.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAlarmLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDisasterLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetectList)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.pnTop.Controls.Add(this.btnTester2);
            this.pnTop.Controls.Add(this.btnTeamEditor2);
            this.pnTop.Controls.Add(this.btnTester);
            this.pnTop.Controls.Add(this.btnTeamEditor);
            this.pnTop.Controls.Add(this.picUser);
            this.pnTop.Controls.Add(this.lblUserName);
            this.pnTop.Controls.Add(this.btnExit);
            this.pnTop.Controls.Add(this.lblDisasterInfo);
            this.pnTop.Controls.Add(this.pnTabSelect);
            this.pnTop.Controls.Add(this.panel1);
            this.pnTop.Controls.Add(this.panel2);
            this.pnTop.Controls.Add(this.btnTabReport);
            this.pnTop.Controls.Add(this.btnTabEdit);
            this.pnTop.Controls.Add(this.btnTabMonitoring);
            this.pnTop.Controls.Add(this.picSensorMonitor);
            this.pnTop.Controls.Add(this.btnManagement);
            this.pnTop.Controls.Add(this.picLogo);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1539, 70);
            this.pnTop.TabIndex = 0;
            this.pnTop.Paint += new System.Windows.Forms.PaintEventHandler(this.pnTop_Paint);
            this.pnTop.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseDoubleClick);
            this.pnTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseDown);
            this.pnTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseMove);
            this.pnTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseUp);
            // 
            // btnTester2
            // 
            this.btnTester2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTester2.AutoSize = true;
            this.btnTester2.ColorClicked = System.Drawing.Color.White;
            this.btnTester2.ColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.btnTester2.ColorNomal = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnTester2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTester2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnTester2.Location = new System.Drawing.Point(254, 35);
            this.btnTester2.Name = "btnTester2";
            this.btnTester2.Size = new System.Drawing.Size(51, 19);
            this.btnTester2.TabIndex = 32;
            this.btnTester2.Text = "테스트";
            this.btnTester2.Click += new System.EventHandler(this.btnTester_Click);
            // 
            // btnTeamEditor2
            // 
            this.btnTeamEditor2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTeamEditor2.AutoSize = true;
            this.btnTeamEditor2.ColorClicked = System.Drawing.Color.White;
            this.btnTeamEditor2.ColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.btnTeamEditor2.ColorNomal = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnTeamEditor2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTeamEditor2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnTeamEditor2.Location = new System.Drawing.Point(254, 16);
            this.btnTeamEditor2.Name = "btnTeamEditor2";
            this.btnTeamEditor2.Size = new System.Drawing.Size(79, 19);
            this.btnTeamEditor2.TabIndex = 31;
            this.btnTeamEditor2.Text = "조직관리툴";
            this.btnTeamEditor2.Click += new System.EventHandler(this.btnTeamEditor_Click);
            // 
            // btnTester
            // 
            this.btnTester.CheckButton = false;
            this.btnTester.CheckedBkgndImage = null;
            this.btnTester.CheckedImage = null;
            this.btnTester.CheckedMouseOver = null;
            this.btnTester.ClickedBackgroundImage = null;
            this.btnTester.ClickedImage = global::SDMS_Building.Properties.Resources.tester_click;
            this.btnTester.CustomImageRect = new System.Drawing.Rectangle(0, 0, 26, 35);
            this.btnTester.DisabledBkgndImage = null;
            this.btnTester.DisabledImage = null;
            this.btnTester.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnTester.ForeColorChecked = System.Drawing.Color.White;
            this.btnTester.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnTester.ForeColorDisabled = System.Drawing.Color.White;
            this.btnTester.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.btnTester.ForeColorsByTypeUse = true;
            this.btnTester.ID = -1;
            this.btnTester.InitButtonWidth = 35;
            this.btnTester.IsChecked = false;
            this.btnTester.Location = new System.Drawing.Point(171, 20);
            this.btnTester.MouseOverBkgndImage = null;
            this.btnTester.MouseOverImage = global::SDMS_Building.Properties.Resources.tester_hover;
            this.btnTester.Name = "btnTester";
            this.btnTester.NormalImage = global::SDMS_Building.Properties.Resources.tester_normal;
            this.btnTester.Owner = null;
            this.btnTester.Size = new System.Drawing.Size(44, 31);
            this.btnTester.TabIndex = 30;
            this.btnTester.Text = "테스트";
            this.btnTester.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTester.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnTester.TextLocation = new System.Drawing.Point(30, 10);
            this.btnTester.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnTester.ToolTipText = "테스트";
            this.btnTester.UseCustomImageRect = true;
            this.btnTester.UseTextLocation = true;
            this.btnTester.UseVisualStyleBackColor = true;
            this.btnTester.Visible = false;
            this.btnTester.Click += new System.EventHandler(this.btnTester_Click);
            // 
            // btnTeamEditor
            // 
            this.btnTeamEditor.CheckButton = false;
            this.btnTeamEditor.CheckedBkgndImage = null;
            this.btnTeamEditor.CheckedImage = null;
            this.btnTeamEditor.CheckedMouseOver = null;
            this.btnTeamEditor.ClickedBackgroundImage = null;
            this.btnTeamEditor.ClickedImage = global::SDMS_Building.Properties.Resources.teamEditor_click;
            this.btnTeamEditor.CustomImageRect = new System.Drawing.Rectangle(0, 0, 30, 31);
            this.btnTeamEditor.DisabledBkgndImage = null;
            this.btnTeamEditor.DisabledImage = null;
            this.btnTeamEditor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnTeamEditor.ForeColorChecked = System.Drawing.Color.White;
            this.btnTeamEditor.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnTeamEditor.ForeColorDisabled = System.Drawing.Color.White;
            this.btnTeamEditor.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.btnTeamEditor.ForeColorsByTypeUse = true;
            this.btnTeamEditor.ID = -1;
            this.btnTeamEditor.InitButtonWidth = 38;
            this.btnTeamEditor.IsChecked = false;
            this.btnTeamEditor.Location = new System.Drawing.Point(116, 20);
            this.btnTeamEditor.MouseOverBkgndImage = null;
            this.btnTeamEditor.MouseOverImage = global::SDMS_Building.Properties.Resources.teamEditor_hover;
            this.btnTeamEditor.Name = "btnTeamEditor";
            this.btnTeamEditor.NormalImage = global::SDMS_Building.Properties.Resources.teamEditor_normal;
            this.btnTeamEditor.Owner = null;
            this.btnTeamEditor.Size = new System.Drawing.Size(68, 31);
            this.btnTeamEditor.TabIndex = 13;
            this.btnTeamEditor.Text = "조직관리툴";
            this.btnTeamEditor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTeamEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnTeamEditor.TextLocation = new System.Drawing.Point(36, 10);
            this.btnTeamEditor.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnTeamEditor.ToolTipText = "조직관리툴";
            this.btnTeamEditor.UseCustomImageRect = true;
            this.btnTeamEditor.UseTextLocation = true;
            this.btnTeamEditor.UseVisualStyleBackColor = true;
            this.btnTeamEditor.Visible = false;
            this.btnTeamEditor.Click += new System.EventHandler(this.btnTeamEditor_Click);
            // 
            // picUser
            // 
            this.picUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picUser.Image = global::SDMS_Building.Properties.Resources.userIcon;
            this.picUser.Location = new System.Drawing.Point(1387, 22);
            this.picUser.Name = "picUser";
            this.picUser.Size = new System.Drawing.Size(25, 25);
            this.picUser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picUser.TabIndex = 27;
            this.picUser.TabStop = false;
            // 
            // lblUserName
            // 
            this.lblUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.lblUserName.Location = new System.Drawing.Point(1424, 29);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(18, 19);
            this.lblUserName.TabIndex = 26;
            this.lblUserName.Text = "-";
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.ButtonText = "";
            this.btnExit.ImageClicked = global::SDMS_Building.Properties.Resources.close_Click;
            this.btnExit.ImageDisabled = null;
            this.btnExit.ImageMouseOver = global::SDMS_Building.Properties.Resources.close_Normal;
            this.btnExit.ImageNormal = global::SDMS_Building.Properties.Resources.close_Hover;
            this.btnExit.Location = new System.Drawing.Point(1498, 20);
            this.btnExit.Name = "btnExit";
            this.btnExit.Owner = null;
            this.btnExit.Size = new System.Drawing.Size(28, 28);
            this.btnExit.TabIndex = 23;
            this.btnExit.TabStop = false;
            this.btnExit.TextColor = System.Drawing.Color.Black;
            this.btnExit.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnExit.ToolTipText = "";
            this.btnExit.UseToolTip = false;
            this.btnExit.Visible = false;
            this.btnExit.WindowRateWidth = 1F;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblDisasterInfo
            // 
            this.lblDisasterInfo.AutoSize = true;
            this.lblDisasterInfo.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisasterInfo.ForeColor = System.Drawing.Color.White;
            this.lblDisasterInfo.Location = new System.Drawing.Point(955, 25);
            this.lblDisasterInfo.Name = "lblDisasterInfo";
            this.lblDisasterInfo.Size = new System.Drawing.Size(53, 19);
            this.lblDisasterInfo.TabIndex = 22;
            this.lblDisasterInfo.Text = "label5";
            // 
            // pnTabSelect
            // 
            this.pnTabSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.pnTabSelect.Location = new System.Drawing.Point(345, 65);
            this.pnTabSelect.Name = "pnTabSelect";
            this.pnTabSelect.Size = new System.Drawing.Size(200, 5);
            this.pnTabSelect.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.panel1.Location = new System.Drawing.Point(747, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 20);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.panel2.Location = new System.Drawing.Point(545, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1, 20);
            this.panel2.TabIndex = 1;
            // 
            // btnTabReport
            // 
            this.btnTabReport.ButtonText = "";
            this.btnTabReport.ImageClicked = global::SDMS_Building.Properties.Resources.pnTop_Report_Normal;
            this.btnTabReport.ImageDisabled = null;
            this.btnTabReport.ImageMouseOver = global::SDMS_Building.Properties.Resources.pnTop_Report_Hover;
            this.btnTabReport.ImageNormal = global::SDMS_Building.Properties.Resources.pnTop_Report_Normal;
            this.btnTabReport.Location = new System.Drawing.Point(547, 0);
            this.btnTabReport.Name = "btnTabReport";
            this.btnTabReport.Owner = null;
            this.btnTabReport.Size = new System.Drawing.Size(200, 70);
            this.btnTabReport.TabIndex = 7;
            this.btnTabReport.TabStop = false;
            this.btnTabReport.TextColor = System.Drawing.Color.Black;
            this.btnTabReport.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnTabReport.ToolTipText = "";
            this.btnTabReport.UseToolTip = false;
            this.btnTabReport.WindowRateWidth = 1F;
            this.btnTabReport.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // btnTabEdit
            // 
            this.btnTabEdit.ButtonText = "";
            this.btnTabEdit.ImageClicked = global::SDMS_Building.Properties.Resources.pnTop_Edit_Normal;
            this.btnTabEdit.ImageDisabled = null;
            this.btnTabEdit.ImageMouseOver = global::SDMS_Building.Properties.Resources.pnTop_Edit_Hover;
            this.btnTabEdit.ImageNormal = global::SDMS_Building.Properties.Resources.pnTop_Edit_Normal;
            this.btnTabEdit.Location = new System.Drawing.Point(749, 0);
            this.btnTabEdit.Name = "btnTabEdit";
            this.btnTabEdit.Owner = null;
            this.btnTabEdit.Size = new System.Drawing.Size(200, 70);
            this.btnTabEdit.TabIndex = 6;
            this.btnTabEdit.TabStop = false;
            this.btnTabEdit.TextColor = System.Drawing.Color.Black;
            this.btnTabEdit.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnTabEdit.ToolTipText = "";
            this.btnTabEdit.UseToolTip = false;
            this.btnTabEdit.WindowRateWidth = 1F;
            this.btnTabEdit.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // btnTabMonitoring
            // 
            this.btnTabMonitoring.ButtonText = "";
            this.btnTabMonitoring.ImageClicked = global::SDMS_Building.Properties.Resources.pnTop_Monitoring_Click;
            this.btnTabMonitoring.ImageDisabled = null;
            this.btnTabMonitoring.ImageMouseOver = global::SDMS_Building.Properties.Resources.pnTop_Monitoring_Click;
            this.btnTabMonitoring.ImageNormal = global::SDMS_Building.Properties.Resources.pnTop_Monitoring_Click;
            this.btnTabMonitoring.Location = new System.Drawing.Point(345, 0);
            this.btnTabMonitoring.Name = "btnTabMonitoring";
            this.btnTabMonitoring.Owner = null;
            this.btnTabMonitoring.Size = new System.Drawing.Size(200, 70);
            this.btnTabMonitoring.TabIndex = 5;
            this.btnTabMonitoring.TabStop = false;
            this.btnTabMonitoring.TextColor = System.Drawing.Color.Black;
            this.btnTabMonitoring.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnTabMonitoring.ToolTipText = "";
            this.btnTabMonitoring.UseToolTip = false;
            this.btnTabMonitoring.WindowRateWidth = 1F;
            this.btnTabMonitoring.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // picSensorMonitor
            // 
            this.picSensorMonitor.Image = global::SDMS_Building.Properties.Resources.pnTop_SensorMonitor;
            this.picSensorMonitor.Location = new System.Drawing.Point(818, 14);
            this.picSensorMonitor.Name = "picSensorMonitor";
            this.picSensorMonitor.Size = new System.Drawing.Size(500, 40);
            this.picSensorMonitor.TabIndex = 2;
            this.picSensorMonitor.TabStop = false;
            this.picSensorMonitor.Visible = false;
            // 
            // btnManagement
            // 
            this.btnManagement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManagement.ButtonText = "";
            this.btnManagement.ImageClicked = global::SDMS_Building.Properties.Resources.Setting_Click;
            this.btnManagement.ImageDisabled = null;
            this.btnManagement.ImageMouseOver = global::SDMS_Building.Properties.Resources.Setting_Click;
            this.btnManagement.ImageNormal = global::SDMS_Building.Properties.Resources.Setting_Normal;
            this.btnManagement.Location = new System.Drawing.Point(1498, 20);
            this.btnManagement.Name = "btnManagement";
            this.btnManagement.Owner = null;
            this.btnManagement.Size = new System.Drawing.Size(32, 33);
            this.btnManagement.TabIndex = 1;
            this.btnManagement.TabStop = false;
            this.btnManagement.TextColor = System.Drawing.Color.Black;
            this.btnManagement.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnManagement.ToolTipText = "";
            this.btnManagement.UseToolTip = false;
            this.btnManagement.WindowRateWidth = 1F;
            this.btnManagement.Click += new System.EventHandler(this.btnManagement_Click);
            // 
            // picLogo
            // 
            this.picLogo.Image = global::SDMS_Building.Properties.Resources.logo;
            this.picLogo.Location = new System.Drawing.Point(23, 18);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(69, 29);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // pnLeft
            // 
            this.pnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(64)))), ((int)(((byte)(100)))));
            this.pnLeft.Controls.Add(this.button1);
            this.pnLeft.Controls.Add(this.lblZoneName);
            this.pnLeft.Controls.Add(this.picTreeViewRefresh);
            this.pnLeft.Controls.Add(this.label7);
            this.pnLeft.Controls.Add(this.radioOffSensor);
            this.pnLeft.Controls.Add(this.label6);
            this.pnLeft.Controls.Add(this.radioAllSensor);
            this.pnLeft.Controls.Add(this.treeViewAdv1);
            this.pnLeft.Controls.Add(this.btnOutdoor);
            this.pnLeft.Location = new System.Drawing.Point(0, 70);
            this.pnLeft.Name = "pnLeft";
            this.pnLeft.Size = new System.Drawing.Size(345, 878);
            this.pnLeft.TabIndex = 1;
            this.pnLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.pnLeft_Paint);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(208, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 23);
            this.button1.TabIndex = 53;
            this.button1.Text = "SetEquipmentZone";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // lblZoneName
            // 
            this.lblZoneName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblZoneName.ColorClicked = System.Drawing.Color.White;
            this.lblZoneName.ColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.lblZoneName.ColorNomal = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.lblZoneName.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZoneName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.lblZoneName.Location = new System.Drawing.Point(138, 129);
            this.lblZoneName.Name = "lblZoneName";
            this.lblZoneName.Size = new System.Drawing.Size(162, 19);
            this.lblZoneName.TabIndex = 33;
            this.lblZoneName.Text = "외부";
            this.lblZoneName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picTreeViewRefresh
            // 
            this.picTreeViewRefresh.Image = global::SDMS_Building.Properties.Resources.refresh_button;
            this.picTreeViewRefresh.Location = new System.Drawing.Point(306, 125);
            this.picTreeViewRefresh.Name = "picTreeViewRefresh";
            this.picTreeViewRefresh.Size = new System.Drawing.Size(27, 26);
            this.picTreeViewRefresh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picTreeViewRefresh.TabIndex = 19;
            this.picTreeViewRefresh.TabStop = false;
            this.picTreeViewRefresh.Click += new System.EventHandler(this.picTreeViewRefresh_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(101, 130);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 19);
            this.label7.TabIndex = 52;
            this.label7.Text = "Off";
            // 
            // radioOffSensor
            // 
            this.radioOffSensor.CheckButton = false;
            this.radioOffSensor.CheckedBkgndImage = null;
            this.radioOffSensor.CheckedImage = global::SDMS_Building.Properties.Resources.Radio_Checked;
            this.radioOffSensor.CheckedMouseOver = global::SDMS_Building.Properties.Resources.Radio_Checked;
            this.radioOffSensor.ClickedBackgroundImage = null;
            this.radioOffSensor.ClickedImage = global::SDMS_Building.Properties.Resources.Radio_Checked_MouseOver;
            this.radioOffSensor.CustomImageRect = new System.Drawing.Rectangle(0, 0, 20, 20);
            this.radioOffSensor.DisabledBkgndImage = null;
            this.radioOffSensor.DisabledImage = null;
            this.radioOffSensor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.radioOffSensor.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.radioOffSensor.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.radioOffSensor.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.radioOffSensor.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.radioOffSensor.ForeColorsByTypeUse = true;
            this.radioOffSensor.ID = -1;
            this.radioOffSensor.InitButtonWidth = 20;
            this.radioOffSensor.IsChecked = false;
            this.radioOffSensor.Location = new System.Drawing.Point(75, 129);
            this.radioOffSensor.MouseOverBkgndImage = null;
            this.radioOffSensor.MouseOverImage = global::SDMS_Building.Properties.Resources.Radio_Unchecked;
            this.radioOffSensor.Name = "radioOffSensor";
            this.radioOffSensor.NormalImage = global::SDMS_Building.Properties.Resources.Radio_Unchecked;
            this.radioOffSensor.Owner = null;
            this.radioOffSensor.Size = new System.Drawing.Size(20, 20);
            this.radioOffSensor.TabIndex = 51;
            this.radioOffSensor.TextLocation = new System.Drawing.Point(0, 13);
            this.radioOffSensor.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.radioOffSensor.ToolTipText = "";
            this.radioOffSensor.UseCustomImageRect = true;
            this.radioOffSensor.UseTextLocation = true;
            this.radioOffSensor.UseVisualStyleBackColor = true;
            this.radioOffSensor.Click += new System.EventHandler(this.radioSensor_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(40, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 19);
            this.label6.TabIndex = 50;
            this.label6.Text = "All";
            // 
            // radioAllSensor
            // 
            this.radioAllSensor.CheckButton = false;
            this.radioAllSensor.CheckedBkgndImage = null;
            this.radioAllSensor.CheckedImage = global::SDMS_Building.Properties.Resources.Radio_Checked;
            this.radioAllSensor.CheckedMouseOver = global::SDMS_Building.Properties.Resources.Radio_Checked;
            this.radioAllSensor.ClickedBackgroundImage = null;
            this.radioAllSensor.ClickedImage = global::SDMS_Building.Properties.Resources.Radio_Checked_MouseOver;
            this.radioAllSensor.CustomImageRect = new System.Drawing.Rectangle(0, 0, 20, 20);
            this.radioAllSensor.DisabledBkgndImage = null;
            this.radioAllSensor.DisabledImage = null;
            this.radioAllSensor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.radioAllSensor.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.radioAllSensor.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.radioAllSensor.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.radioAllSensor.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.radioAllSensor.ForeColorsByTypeUse = true;
            this.radioAllSensor.ID = -1;
            this.radioAllSensor.InitButtonWidth = 20;
            this.radioAllSensor.IsChecked = true;
            this.radioAllSensor.Location = new System.Drawing.Point(14, 129);
            this.radioAllSensor.MouseOverBkgndImage = null;
            this.radioAllSensor.MouseOverImage = global::SDMS_Building.Properties.Resources.Radio_Unchecked;
            this.radioAllSensor.Name = "radioAllSensor";
            this.radioAllSensor.NormalImage = global::SDMS_Building.Properties.Resources.Radio_Unchecked;
            this.radioAllSensor.Owner = null;
            this.radioAllSensor.Size = new System.Drawing.Size(20, 20);
            this.radioAllSensor.TabIndex = 49;
            this.radioAllSensor.TextLocation = new System.Drawing.Point(0, 13);
            this.radioAllSensor.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.radioAllSensor.ToolTipText = "";
            this.radioAllSensor.UseCustomImageRect = true;
            this.radioAllSensor.UseTextLocation = true;
            this.radioAllSensor.UseVisualStyleBackColor = true;
            this.radioAllSensor.Click += new System.EventHandler(this.radioSensor_Click);
            // 
            // treeViewAdv1
            // 
            this.treeViewAdv1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.treeViewAdv1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewAdv1.CurrentFont = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewAdv1.CurrentTextColor = System.Drawing.Color.Red;
            this.treeViewAdv1.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeViewAdv1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdv1.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewAdv1.ForeColor = System.Drawing.Color.White;
            this.treeViewAdv1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdv1.Location = new System.Drawing.Point(10, 155);
            this.treeViewAdv1.MinimumSize = new System.Drawing.Size(323, 2);
            this.treeViewAdv1.Model = null;
            this.treeViewAdv1.Name = "treeViewAdv1";
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBox1);
            this.treeViewAdv1.NodeControls.Add(this.nodeStateIcon1);
            this.treeViewAdv1.NodeControls.Add(this.nodeIcon1);
            this.treeViewAdv1.RowHeight = 20;
            this.treeViewAdv1.SelectedChildColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.treeViewAdv1.SelectedChildFont = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewAdv1.SelectedChildTextColor = System.Drawing.Color.White;
            this.treeViewAdv1.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(101)))), ((int)(((byte)(150)))));
            this.treeViewAdv1.SelectedFont = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewAdv1.SelectedNode = null;
            this.treeViewAdv1.SelectedTextColor = System.Drawing.Color.White;
            this.treeViewAdv1.ShowNodeToolTips = true;
            this.treeViewAdv1.Size = new System.Drawing.Size(323, 775);
            this.treeViewAdv1.TabIndex = 13;
            this.treeViewAdv1.Text = "treeViewAdv1";
            this.treeViewAdv1.TextColor = System.Drawing.Color.White;
            this.treeViewAdv1.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.treeViewAdv1_NodeMouseDoubleClick);
            this.treeViewAdv1.SelectionChanged += new System.EventHandler(this.treeViewAdv1_SelectionChanged);
            // 
            // btnOutdoor
            // 
            this.btnOutdoor.CheckButton = false;
            this.btnOutdoor.CheckedBkgndImage = null;
            this.btnOutdoor.CheckedImage = null;
            this.btnOutdoor.CheckedMouseOver = null;
            this.btnOutdoor.ClickedBackgroundImage = null;
            this.btnOutdoor.ClickedImage = global::SDMS_Building.Properties.Resources.home_Click;
            this.btnOutdoor.CustomImageRect = new System.Drawing.Rectangle(0, 0, 325, 40);
            this.btnOutdoor.DisabledBkgndImage = null;
            this.btnOutdoor.DisabledImage = null;
            this.btnOutdoor.ForeColorChecked = System.Drawing.Color.White;
            this.btnOutdoor.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnOutdoor.ForeColorDisabled = System.Drawing.Color.White;
            this.btnOutdoor.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnOutdoor.ForeColorsByTypeUse = false;
            this.btnOutdoor.ID = -1;
            this.btnOutdoor.InitButtonWidth = 325;
            this.btnOutdoor.IsChecked = false;
            this.btnOutdoor.Location = new System.Drawing.Point(10, 71);
            this.btnOutdoor.MouseOverBkgndImage = null;
            this.btnOutdoor.MouseOverImage = global::SDMS_Building.Properties.Resources.home_Hover;
            this.btnOutdoor.Name = "btnOutdoor";
            this.btnOutdoor.NormalImage = global::SDMS_Building.Properties.Resources.home_Normal;
            this.btnOutdoor.Owner = null;
            this.btnOutdoor.Size = new System.Drawing.Size(325, 40);
            this.btnOutdoor.TabIndex = 12;
            this.btnOutdoor.Text = "외부 화면";
            this.btnOutdoor.TextLocation = new System.Drawing.Point(0, 10);
            this.btnOutdoor.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOutdoor.ToolTipText = "외부 화면";
            this.btnOutdoor.UseCustomImageRect = true;
            this.btnOutdoor.UseTextLocation = true;
            this.btnOutdoor.UseVisualStyleBackColor = true;
            this.btnOutdoor.Click += new System.EventHandler(this.btnOutdoor_Click);
            // 
            // pnRight
            // 
            this.pnRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(79)))));
            this.pnRight.Controls.Add(this.dgvOpenDoor);
            this.pnRight.Controls.Add(this.lblDetectCount);
            this.pnRight.Controls.Add(this.picDoorInfo);
            this.pnRight.Controls.Add(this.label5);
            this.pnRight.Controls.Add(this.label4);
            this.pnRight.Controls.Add(this.label3);
            this.pnRight.Controls.Add(this.label2);
            this.pnRight.Controls.Add(this.label1);
            this.pnRight.Controls.Add(this.pnDisaster);
            this.pnRight.Controls.Add(this.dgvDetectList);
            this.pnRight.Location = new System.Drawing.Point(1185, 70);
            this.pnRight.Name = "pnRight";
            this.pnRight.Size = new System.Drawing.Size(345, 878);
            this.pnRight.TabIndex = 10;
            this.pnRight.Paint += new System.Windows.Forms.PaintEventHandler(this.pnRight_Paint);
            // 
            // dgvOpenDoor
            // 
            this.dgvOpenDoor.AllowUserToResizeColumns = false;
            this.dgvOpenDoor.AllowUserToResizeRows = false;
            this.dgvOpenDoor.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvOpenDoor.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(79)))));
            this.dgvOpenDoor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvOpenDoor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOpenDoor.ColumnHeadersVisible = false;
            this.dgvOpenDoor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn4});
            this.dgvOpenDoor.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.dgvOpenDoor.Location = new System.Drawing.Point(5, 400);
            this.dgvOpenDoor.MultiSelect = false;
            this.dgvOpenDoor.Name = "dgvOpenDoor";
            this.dgvOpenDoor.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvOpenDoor.RowHeadersVisible = false;
            this.dgvOpenDoor.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(79)))));
            this.dgvOpenDoor.RowTemplate.Height = 35;
            this.dgvOpenDoor.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.dgvOpenDoor.Size = new System.Drawing.Size(330, 182);
            this.dgvOpenDoor.TabIndex = 24;
            this.dgvOpenDoor.Visible = false;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "현황";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 320;
            // 
            // lblDetectCount
            // 
            this.lblDetectCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(79)))));
            this.lblDetectCount.Font = new System.Drawing.Font("나눔바른고딕", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetectCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.lblDetectCount.Location = new System.Drawing.Point(95, 703);
            this.lblDetectCount.Name = "lblDetectCount";
            this.lblDetectCount.Size = new System.Drawing.Size(47, 22);
            this.lblDetectCount.TabIndex = 23;
            this.lblDetectCount.Text = "[ 99 ]";
            this.lblDetectCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picDoorInfo
            // 
            this.picDoorInfo.BackColor = System.Drawing.Color.Transparent;
            this.picDoorInfo.Image = global::SDMS_Building.Properties.Resources.pnRight_DoorInfo;
            this.picDoorInfo.Location = new System.Drawing.Point(11, 297);
            this.picDoorInfo.Name = "picDoorInfo";
            this.picDoorInfo.Size = new System.Drawing.Size(337, 22);
            this.picDoorInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDoorInfo.TabIndex = 19;
            this.picDoorInfo.TabStop = false;
            this.picDoorInfo.Visible = false;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(10, 336);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(323, 61);
            this.label5.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(79)))));
            this.label4.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.label4.Location = new System.Drawing.Point(278, 737);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 29);
            this.label4.TabIndex = 21;
            this.label4.Text = "현황";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(79)))));
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.label3.Location = new System.Drawing.Point(154, 737);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 29);
            this.label3.TabIndex = 20;
            this.label3.Text = "장소";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(79)))));
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.label2.Location = new System.Drawing.Point(95, 737);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 29);
            this.label2.TabIndex = 19;
            this.label2.Text = "종류";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(79)))));
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.label1.Location = new System.Drawing.Point(12, 737);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 29);
            this.label1.TabIndex = 18;
            this.label1.Text = "일시";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnDisaster
            // 
            this.pnDisaster.Controls.Add(this.picAlarmLevel);
            this.pnDisaster.Controls.Add(this.btnMalfunction);
            this.pnDisaster.Controls.Add(this.panel3);
            this.pnDisaster.Controls.Add(this.btnReport);
            this.pnDisaster.Controls.Add(this.picDisasterLogo);
            this.pnDisaster.Controls.Add(this.btnSound);
            this.pnDisaster.Controls.Add(this.lblDisasterType);
            this.pnDisaster.Controls.Add(this.lblDisasterDate);
            this.pnDisaster.Controls.Add(this.lblDisasterLocation);
            this.pnDisaster.Location = new System.Drawing.Point(11, 71);
            this.pnDisaster.Name = "pnDisaster";
            this.pnDisaster.Size = new System.Drawing.Size(324, 220);
            this.pnDisaster.TabIndex = 17;
            // 
            // picAlarmLevel
            // 
            this.picAlarmLevel.Image = global::SDMS_Building.Properties.Resources.alarmLevel2_normal;
            this.picAlarmLevel.Location = new System.Drawing.Point(15, 108);
            this.picAlarmLevel.Name = "picAlarmLevel";
            this.picAlarmLevel.Size = new System.Drawing.Size(295, 50);
            this.picAlarmLevel.TabIndex = 13;
            this.picAlarmLevel.TabStop = false;
            this.picAlarmLevel.Tag = "주의";
            // 
            // btnMalfunction
            // 
            this.btnMalfunction.CheckButton = false;
            this.btnMalfunction.CheckedBkgndImage = null;
            this.btnMalfunction.CheckedImage = null;
            this.btnMalfunction.CheckedMouseOver = null;
            this.btnMalfunction.ClickedBackgroundImage = null;
            this.btnMalfunction.ClickedImage = global::SDMS_Building.Properties.Resources.confirm_Normal;
            this.btnMalfunction.CustomImageRect = new System.Drawing.Rectangle(0, 0, 325, 40);
            this.btnMalfunction.DisabledBkgndImage = null;
            this.btnMalfunction.DisabledImage = null;
            this.btnMalfunction.ForeColorChecked = System.Drawing.Color.White;
            this.btnMalfunction.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnMalfunction.ForeColorDisabled = System.Drawing.Color.White;
            this.btnMalfunction.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnMalfunction.ForeColorsByTypeUse = false;
            this.btnMalfunction.ID = -1;
            this.btnMalfunction.InitButtonWidth = 140;
            this.btnMalfunction.IsChecked = false;
            this.btnMalfunction.Location = new System.Drawing.Point(170, 164);
            this.btnMalfunction.MouseOverBkgndImage = null;
            this.btnMalfunction.MouseOverImage = global::SDMS_Building.Properties.Resources.confirm_Hover;
            this.btnMalfunction.Name = "btnMalfunction";
            this.btnMalfunction.NormalImage = global::SDMS_Building.Properties.Resources.confirm_Click;
            this.btnMalfunction.Owner = null;
            this.btnMalfunction.Size = new System.Drawing.Size(140, 38);
            this.btnMalfunction.TabIndex = 13;
            this.btnMalfunction.Text = "오작동";
            this.btnMalfunction.TextLocation = new System.Drawing.Point(0, 10);
            this.btnMalfunction.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnMalfunction.ToolTipText = "오작동";
            this.btnMalfunction.UseCustomImageRect = true;
            this.btnMalfunction.UseTextLocation = true;
            this.btnMalfunction.UseVisualStyleBackColor = true;
            this.btnMalfunction.Click += new System.EventHandler(this.btnMalfunction_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(101)))), ((int)(((byte)(150)))));
            this.panel3.Location = new System.Drawing.Point(108, 51);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 1);
            this.panel3.TabIndex = 18;
            // 
            // btnReport
            // 
            this.btnReport.BackColor = System.Drawing.Color.Transparent;
            this.btnReport.ButtonText = "";
            this.btnReport.Enabled = false;
            this.btnReport.ImageClicked = global::SDMS_Building.Properties.Resources.alarmLevel2_click;
            this.btnReport.ImageDisabled = null;
            this.btnReport.ImageMouseOver = global::SDMS_Building.Properties.Resources.alarmLevel2_hover;
            this.btnReport.ImageNormal = global::SDMS_Building.Properties.Resources.alarmLevel2_normal;
            this.btnReport.Location = new System.Drawing.Point(15, 108);
            this.btnReport.Name = "btnReport";
            this.btnReport.Owner = null;
            this.btnReport.Size = new System.Drawing.Size(295, 50);
            this.btnReport.TabIndex = 12;
            this.btnReport.TabStop = false;
            this.btnReport.TextColor = System.Drawing.Color.Black;
            this.btnReport.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnReport.ToolTipText = "";
            this.btnReport.UseToolTip = false;
            this.btnReport.Visible = false;
            this.btnReport.WindowRateWidth = 1F;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // picDisasterLogo
            // 
            this.picDisasterLogo.Image = global::SDMS_Building.Properties.Resources.poi_fire_detect;
            this.picDisasterLogo.Location = new System.Drawing.Point(15, 22);
            this.picDisasterLogo.Name = "picDisasterLogo";
            this.picDisasterLogo.Size = new System.Drawing.Size(65, 65);
            this.picDisasterLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDisasterLogo.TabIndex = 11;
            this.picDisasterLogo.TabStop = false;
            // 
            // btnSound
            // 
            this.btnSound.BackColor = System.Drawing.Color.Transparent;
            this.btnSound.ButtonText = "";
            this.btnSound.ImageClicked = global::SDMS_Building.Properties.Resources.soundoff_click;
            this.btnSound.ImageDisabled = null;
            this.btnSound.ImageMouseOver = global::SDMS_Building.Properties.Resources.soundoff_hover;
            this.btnSound.ImageNormal = global::SDMS_Building.Properties.Resources.soundoff_normal;
            this.btnSound.Location = new System.Drawing.Point(15, 164);
            this.btnSound.Name = "btnSound";
            this.btnSound.Owner = null;
            this.btnSound.Size = new System.Drawing.Size(140, 40);
            this.btnSound.TabIndex = 15;
            this.btnSound.TabStop = false;
            this.btnSound.TextColor = System.Drawing.Color.Black;
            this.btnSound.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSound.ToolTipText = "";
            this.btnSound.UseToolTip = false;
            this.btnSound.WindowRateWidth = 1F;
            this.btnSound.Click += new System.EventHandler(this.btnSound_Click);
            // 
            // lblDisasterType
            // 
            this.lblDisasterType.AutoSize = true;
            this.lblDisasterType.Font = new System.Drawing.Font("나눔바른고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisasterType.ForeColor = System.Drawing.Color.White;
            this.lblDisasterType.Location = new System.Drawing.Point(108, 16);
            this.lblDisasterType.Name = "lblDisasterType";
            this.lblDisasterType.Size = new System.Drawing.Size(48, 24);
            this.lblDisasterType.TabIndex = 12;
            this.lblDisasterType.Text = "화재";
            // 
            // lblDisasterDate
            // 
            this.lblDisasterDate.AutoSize = true;
            this.lblDisasterDate.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisasterDate.ForeColor = System.Drawing.Color.White;
            this.lblDisasterDate.Location = new System.Drawing.Point(235, 25);
            this.lblDisasterDate.Name = "lblDisasterDate";
            this.lblDisasterDate.Size = new System.Drawing.Size(73, 17);
            this.lblDisasterDate.TabIndex = 13;
            this.lblDisasterDate.Text = "12:03 PM";
            // 
            // lblDisasterLocation
            // 
            this.lblDisasterLocation.AutoSize = true;
            this.lblDisasterLocation.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisasterLocation.ForeColor = System.Drawing.Color.White;
            this.lblDisasterLocation.Location = new System.Drawing.Point(109, 66);
            this.lblDisasterLocation.Name = "lblDisasterLocation";
            this.lblDisasterLocation.Size = new System.Drawing.Size(201, 22);
            this.lblDisasterLocation.TabIndex = 14;
            this.lblDisasterLocation.Text = "오피스동 25층 OOO구역";
            // 
            // dgvDetectList
            // 
            this.dgvDetectList.AllowUserToResizeColumns = false;
            this.dgvDetectList.AllowUserToResizeRows = false;
            this.dgvDetectList.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(79)))));
            this.dgvDetectList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetectList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetectList.ColumnHeadersVisible = false;
            this.dgvDetectList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDate,
            this.colType,
            this.colLocation,
            this.colStatus});
            this.dgvDetectList.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.dgvDetectList.Location = new System.Drawing.Point(11, 767);
            this.dgvDetectList.MultiSelect = false;
            this.dgvDetectList.Name = "dgvDetectList";
            this.dgvDetectList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvDetectList.RowHeadersVisible = false;
            this.dgvDetectList.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(79)))));
            this.dgvDetectList.RowTemplate.Height = 35;
            this.dgvDetectList.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.dgvDetectList.Size = new System.Drawing.Size(330, 378);
            this.dgvDetectList.TabIndex = 10;
            this.dgvDetectList.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvDetectList_CellMouseDoubleClick);
            this.dgvDetectList.SelectionChanged += new System.EventHandler(this.dgvDetectList_SelectionChanged);
            // 
            // colDate
            // 
            this.colDate.HeaderText = "일시";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            this.colDate.Width = 80;
            // 
            // colType
            // 
            this.colType.HeaderText = "종류";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Width = 63;
            // 
            // colLocation
            // 
            this.colLocation.HeaderText = "장소";
            this.colLocation.Name = "colLocation";
            this.colLocation.ReadOnly = true;
            this.colLocation.Width = 130;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "현황";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 60;
            // 
            // panelBody
            // 
            this.panelBody.BackColor = System.Drawing.Color.Transparent;
            this.panelBody.Location = new System.Drawing.Point(345, 200);
            this.panelBody.Name = "panelBody";
            this.panelBody.Size = new System.Drawing.Size(726, 500);
            this.panelBody.TabIndex = 12;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1539, 948);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.pnRight);
            this.Controls.Add(this.pnLeft);
            this.Controls.Add(this.pnTop);
            this.Name = "FormMain";
            this.Text = "재난탐지 시스템";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTabReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTabEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTabMonitoring)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSensorMonitor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnManagement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnLeft.ResumeLayout(false);
            this.pnLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTreeViewRefresh)).EndInit();
            this.pnRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOpenDoor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDoorInfo)).EndInit();
            this.pnDisaster.ResumeLayout(false);
            this.pnDisaster.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAlarmLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDisasterLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetectList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.PictureBox picLogo;
        private UnE.GUI.ImageButton btnManagement;
        private System.Windows.Forms.PictureBox picSensorMonitor;
        private UnE.GUI.ImageButton btnTabMonitoring;
        private UnE.GUI.ImageButton btnTabReport;
        private UnE.GUI.ImageButton btnTabEdit;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnTabSelect;
        private System.Windows.Forms.Panel pnLeft;
        private System.Windows.Forms.Panel pnRight;
        private System.Windows.Forms.Panel panelBody;
        private System.Windows.Forms.DataGridView dgvDetectList;
        private System.Windows.Forms.PictureBox picDisasterLogo;
        private System.Windows.Forms.Label lblDisasterType;
        private UnE.GUI.ImageButton btnSound;
        private UnE.GUI.ImageButton btnReport;
        private System.Windows.Forms.Label lblDisasterLocation;
        private System.Windows.Forms.Label lblDisasterDate;
        private System.Windows.Forms.Panel pnDisaster;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDisasterInfo;
        private UnE.GUI.ImageButton btnExit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.PictureBox picUser;
        private UnE.GUI.RibbonButton btnOutdoor;
        private UnE.GUI.RibbonButton btnTeamEditor;
        private UnE.GUI.RibbonButton btnTester;
        private UnE.Controls.ColorLabel btnTester2;
        private UnE.Controls.ColorLabel btnTeamEditor2;
        private System.Windows.Forms.PictureBox picDoorInfo;
        private UnE.GUI.RibbonButton btnMalfunction;
        private System.Windows.Forms.PictureBox picAlarmLevel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDetectCount;
        private System.Windows.Forms.DataGridView dgvOpenDoor;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private Aga.Controls.Tree.TreeViewAdv treeViewAdv1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.NodeControls.NodeStateIcon nodeStateIcon1;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon1;
        private System.Windows.Forms.Label label7;
        private UnE.GUI.RibbonButton radioOffSensor;
        private System.Windows.Forms.Label label6;
        private UnE.GUI.RibbonButton radioAllSensor;
        private System.Windows.Forms.PictureBox picTreeViewRefresh;
        private UnE.Controls.ColorLabel lblZoneName;
        private System.Windows.Forms.Button button1;
    }
}

