namespace SensorTester
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btnConnect = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.treeSensorTag = new System.Windows.Forms.TreeView();
            this.labelSensorTagID = new System.Windows.Forms.Label();
            this.labelSensorTagType = new System.Windows.Forms.Label();
            this.labelSensorName = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.gridCurrent = new System.Windows.Forms.DataGridView();
            this.colCurrentDetect = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtZoneSearch = new System.Windows.Forms.TextBox();
            this.btnZoneSearch = new System.Windows.Forms.Button();
            this.btnSensorSearch = new System.Windows.Forms.Button();
            this.txtSensorSearch = new System.Windows.Forms.TextBox();
            this.grpSearchZone = new System.Windows.Forms.GroupBox();
            this.grpSearchSensor = new System.Windows.Forms.GroupBox();
            this.btnSend = new UnE.GUI.ImageButton();
            this.button2 = new UnE.GUI.ImageButton();
            this.button4 = new UnE.GUI.ImageButton();
            this.button3 = new UnE.GUI.ImageButton();
            this.button1 = new UnE.GUI.ImageButton();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.btnSearch = new UnE.GUI.ImageButton();
            this.btnRecovery = new UnE.GUI.ImageButton();
            this.btnRecoverAll = new UnE.GUI.ImageButton();
            this.btnOff = new UnE.GUI.ImageButton();
            this.btnMinimize = new UnE.GUI.ImageButton();
            this.btnClose = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridCurrent)).BeginInit();
            this.grpSearchZone.SuspendLayout();
            this.grpSearchSensor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.button2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.button4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.button3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.button1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRecovery)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRecoverAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(754, 271);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "접속";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Visible = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.Location = new System.Drawing.Point(578, 273);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(170, 21);
            this.textBox4.TabIndex = 7;
            this.textBox4.Text = "127.0.0.1";
            this.textBox4.Visible = false;
            // 
            // treeSensorTag
            // 
            this.treeSensorTag.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeSensorTag.HideSelection = false;
            this.treeSensorTag.Location = new System.Drawing.Point(231, 67);
            this.treeSensorTag.Name = "treeSensorTag";
            this.treeSensorTag.Size = new System.Drawing.Size(333, 197);
            this.treeSensorTag.TabIndex = 8;
            this.treeSensorTag.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSensorTag_AfterSelect);
            // 
            // labelSensorTagID
            // 
            this.labelSensorTagID.AutoSize = true;
            this.labelSensorTagID.BackColor = System.Drawing.Color.Transparent;
            this.labelSensorTagID.Font = new System.Drawing.Font("나눔스퀘어", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSensorTagID.ForeColor = System.Drawing.Color.White;
            this.labelSensorTagID.Location = new System.Drawing.Point(570, 76);
            this.labelSensorTagID.Name = "labelSensorTagID";
            this.labelSensorTagID.Size = new System.Drawing.Size(56, 20);
            this.labelSensorTagID.TabIndex = 9;
            this.labelSensorTagID.Text = "번호 : ";
            // 
            // labelSensorTagType
            // 
            this.labelSensorTagType.AutoSize = true;
            this.labelSensorTagType.BackColor = System.Drawing.Color.Transparent;
            this.labelSensorTagType.Font = new System.Drawing.Font("나눔스퀘어", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSensorTagType.ForeColor = System.Drawing.Color.White;
            this.labelSensorTagType.Location = new System.Drawing.Point(570, 101);
            this.labelSensorTagType.Name = "labelSensorTagType";
            this.labelSensorTagType.Size = new System.Drawing.Size(56, 20);
            this.labelSensorTagType.TabIndex = 9;
            this.labelSensorTagType.Text = "타입 : ";
            // 
            // labelSensorName
            // 
            this.labelSensorName.AutoSize = true;
            this.labelSensorName.BackColor = System.Drawing.Color.Transparent;
            this.labelSensorName.Font = new System.Drawing.Font("나눔스퀘어", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSensorName.ForeColor = System.Drawing.Color.White;
            this.labelSensorName.Location = new System.Drawing.Point(570, 126);
            this.labelSensorName.Name = "labelSensorName";
            this.labelSensorName.Size = new System.Drawing.Size(56, 20);
            this.labelSensorName.TabIndex = 9;
            this.labelSensorName.Text = "이름 : ";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(578, 300);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(170, 20);
            this.comboBox1.TabIndex = 11;
            this.comboBox1.Visible = false;
            this.comboBox1.Leave += new System.EventHandler(this.comboBox1_Leave);
            // 
            // gridCurrent
            // 
            this.gridCurrent.AllowUserToAddRows = false;
            this.gridCurrent.AllowUserToDeleteRows = false;
            this.gridCurrent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCurrent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCurrentDetect});
            this.gridCurrent.Location = new System.Drawing.Point(3, 41);
            this.gridCurrent.MultiSelect = false;
            this.gridCurrent.Name = "gridCurrent";
            this.gridCurrent.ReadOnly = true;
            this.gridCurrent.RowHeadersVisible = false;
            this.gridCurrent.RowTemplate.Height = 23;
            this.gridCurrent.Size = new System.Drawing.Size(222, 190);
            this.gridCurrent.TabIndex = 12;
            this.gridCurrent.SelectionChanged += new System.EventHandler(this.gridCurrent_SelectionChanged);
            // 
            // colCurrentDetect
            // 
            this.colCurrentDetect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCurrentDetect.HeaderText = "현재 신호";
            this.colCurrentDetect.Name = "colCurrentDetect";
            this.colCurrentDetect.ReadOnly = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 2500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtZoneSearch
            // 
            this.txtZoneSearch.Location = new System.Drawing.Point(6, 20);
            this.txtZoneSearch.Name = "txtZoneSearch";
            this.txtZoneSearch.Size = new System.Drawing.Size(189, 21);
            this.txtZoneSearch.TabIndex = 13;
            this.txtZoneSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // btnZoneSearch
            // 
            this.btnZoneSearch.Location = new System.Drawing.Point(201, 18);
            this.btnZoneSearch.Name = "btnZoneSearch";
            this.btnZoneSearch.Size = new System.Drawing.Size(75, 23);
            this.btnZoneSearch.TabIndex = 14;
            this.btnZoneSearch.Text = "검색";
            this.btnZoneSearch.UseVisualStyleBackColor = true;
            this.btnZoneSearch.Click += new System.EventHandler(this.btnZoneSearch_Click);
            // 
            // btnSensorSearch
            // 
            this.btnSensorSearch.Location = new System.Drawing.Point(201, 18);
            this.btnSensorSearch.Name = "btnSensorSearch";
            this.btnSensorSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSensorSearch.TabIndex = 16;
            this.btnSensorSearch.Text = "검색";
            this.btnSensorSearch.UseVisualStyleBackColor = true;
            this.btnSensorSearch.Click += new System.EventHandler(this.btnSensorSearch_Click);
            // 
            // txtSensorSearch
            // 
            this.txtSensorSearch.Location = new System.Drawing.Point(6, 20);
            this.txtSensorSearch.Name = "txtSensorSearch";
            this.txtSensorSearch.Size = new System.Drawing.Size(189, 21);
            this.txtSensorSearch.TabIndex = 15;
            this.txtSensorSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSensorSearch_KeyDown);
            // 
            // grpSearchZone
            // 
            this.grpSearchZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSearchZone.Controls.Add(this.txtZoneSearch);
            this.grpSearchZone.Controls.Add(this.btnZoneSearch);
            this.grpSearchZone.Location = new System.Drawing.Point(860, 12);
            this.grpSearchZone.Name = "grpSearchZone";
            this.grpSearchZone.Size = new System.Drawing.Size(282, 50);
            this.grpSearchZone.TabIndex = 17;
            this.grpSearchZone.TabStop = false;
            this.grpSearchZone.Text = "위치 검색";
            this.grpSearchZone.Visible = false;
            // 
            // grpSearchSensor
            // 
            this.grpSearchSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSearchSensor.Controls.Add(this.txtSensorSearch);
            this.grpSearchSensor.Controls.Add(this.btnSensorSearch);
            this.grpSearchSensor.Location = new System.Drawing.Point(854, 68);
            this.grpSearchSensor.Name = "grpSearchSensor";
            this.grpSearchSensor.Size = new System.Drawing.Size(282, 50);
            this.grpSearchSensor.TabIndex = 18;
            this.grpSearchSensor.TabStop = false;
            this.grpSearchSensor.Text = "센서 검색";
            this.grpSearchSensor.Visible = false;
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.Transparent;
            this.btnSend.ButtonText = "";
            this.btnSend.Enabled = false;
            this.btnSend.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSend.ImageClicked = global::SensorTester.Properties.Resources.BtnSend_Click;
            this.btnSend.ImageDisabled = global::SensorTester.Properties.Resources.BtnSend_Disable;
            this.btnSend.ImageMouseOver = global::SensorTester.Properties.Resources.BtnSend_Click;
            this.btnSend.ImageNormal = global::SensorTester.Properties.Resources.BtnSend_Default;
            this.btnSend.Location = new System.Drawing.Point(570, 201);
            this.btnSend.Name = "btnSend";
            this.btnSend.Owner = null;
            this.btnSend.Size = new System.Drawing.Size(87, 30);
            this.btnSend.TabIndex = 32;
            this.btnSend.TabStop = false;
            this.btnSend.TextColor = System.Drawing.Color.Black;
            this.btnSend.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSend.ToolTipText = "";
            this.btnSend.UseToolTip = false;
            this.btnSend.WindowRateWidth = 1F;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.ButtonText = "";
            this.button2.Enabled = false;
            this.button2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button2.ImageClicked = global::SensorTester.Properties.Resources.BtnStep3_Click;
            this.button2.ImageDisabled = global::SensorTester.Properties.Resources.BtnStep3_Disabled;
            this.button2.ImageMouseOver = global::SensorTester.Properties.Resources.BtnStep3_Click;
            this.button2.ImageNormal = global::SensorTester.Properties.Resources.BtnStep3_Default;
            this.button2.Location = new System.Drawing.Point(748, 201);
            this.button2.Name = "button2";
            this.button2.Owner = null;
            this.button2.Size = new System.Drawing.Size(87, 30);
            this.button2.TabIndex = 31;
            this.button2.TabStop = false;
            this.button2.TextColor = System.Drawing.Color.Black;
            this.button2.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button2.ToolTipText = "";
            this.button2.UseToolTip = false;
            this.button2.WindowRateWidth = 1F;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Transparent;
            this.button4.ButtonText = "";
            this.button4.Enabled = false;
            this.button4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button4.ImageClicked = global::SensorTester.Properties.Resources.BtnErrorRecovery_Click;
            this.button4.ImageDisabled = global::SensorTester.Properties.Resources.BtnErrorRecovery_Disabled;
            this.button4.ImageMouseOver = global::SensorTester.Properties.Resources.BtnErrorRecovery_Click;
            this.button4.ImageNormal = global::SensorTester.Properties.Resources.BtnErrorRecovery_Default;
            this.button4.Location = new System.Drawing.Point(659, 234);
            this.button4.Name = "button4";
            this.button4.Owner = null;
            this.button4.Size = new System.Drawing.Size(87, 30);
            this.button4.TabIndex = 28;
            this.button4.TabStop = false;
            this.button4.TextColor = System.Drawing.Color.Black;
            this.button4.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button4.ToolTipText = "";
            this.button4.UseToolTip = false;
            this.button4.WindowRateWidth = 1F;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Transparent;
            this.button3.ButtonText = "";
            this.button3.Enabled = false;
            this.button3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button3.ImageClicked = global::SensorTester.Properties.Resources.BtnErrorSignal_Click;
            this.button3.ImageDisabled = global::SensorTester.Properties.Resources.BtnErrorSignal_Disabled;
            this.button3.ImageMouseOver = global::SensorTester.Properties.Resources.BtnErrorSignal_Click;
            this.button3.ImageNormal = global::SensorTester.Properties.Resources.BtnErrorSignal_Default;
            this.button3.Location = new System.Drawing.Point(570, 234);
            this.button3.Name = "button3";
            this.button3.Owner = null;
            this.button3.Size = new System.Drawing.Size(87, 30);
            this.button3.TabIndex = 29;
            this.button3.TabStop = false;
            this.button3.TextColor = System.Drawing.Color.Black;
            this.button3.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button3.ToolTipText = "";
            this.button3.UseToolTip = false;
            this.button3.WindowRateWidth = 1F;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.ButtonText = "";
            this.button1.Enabled = false;
            this.button1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.ImageClicked = global::SensorTester.Properties.Resources.BtnStep2_Click;
            this.button1.ImageDisabled = global::SensorTester.Properties.Resources.BtnStep2_Disabled;
            this.button1.ImageMouseOver = global::SensorTester.Properties.Resources.BtnStep2_Click;
            this.button1.ImageNormal = global::SensorTester.Properties.Resources.BtnStep2_Default;
            this.button1.Location = new System.Drawing.Point(659, 201);
            this.button1.Name = "button1";
            this.button1.Owner = null;
            this.button1.Size = new System.Drawing.Size(87, 30);
            this.button1.TabIndex = 30;
            this.button1.TabStop = false;
            this.button1.TextColor = System.Drawing.Color.Black;
            this.button1.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.ToolTipText = "";
            this.button1.UseToolTip = false;
            this.button1.WindowRateWidth = 1F;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("나눔스퀘어", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtSearch.Location = new System.Drawing.Point(570, 41);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(235, 24);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown_1);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Font = new System.Drawing.Font("나눔스퀘어", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox1.ForeColor = System.Drawing.Color.White;
            this.checkBox1.Location = new System.Drawing.Point(231, 45);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(79, 20);
            this.checkBox1.TabIndex = 21;
            this.checkBox1.Text = "화재센서";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.BackColor = System.Drawing.Color.Transparent;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Font = new System.Drawing.Font("나눔스퀘어", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox2.ForeColor = System.Drawing.Color.White;
            this.checkBox2.Location = new System.Drawing.Point(310, 45);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(92, 20);
            this.checkBox2.TabIndex = 22;
            this.checkBox2.Text = "S1 Access";
            this.checkBox2.UseVisualStyleBackColor = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.BackColor = System.Drawing.Color.Transparent;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Font = new System.Drawing.Font("나눔스퀘어", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox3.ForeColor = System.Drawing.Color.White;
            this.checkBox3.Location = new System.Drawing.Point(404, 45);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(64, 20);
            this.checkBox3.TabIndex = 23;
            this.checkBox3.Text = "SVMS";
            this.checkBox3.UseVisualStyleBackColor = false;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.BackColor = System.Drawing.Color.Transparent;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.Font = new System.Drawing.Font("나눔스퀘어", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox4.ForeColor = System.Drawing.Color.White;
            this.checkBox4.Location = new System.Drawing.Point(472, 45);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(79, 20);
            this.checkBox4.TabIndex = 24;
            this.checkBox4.Text = "EMPOLL";
            this.checkBox4.UseVisualStyleBackColor = false;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnSearch.ButtonText = "";
            this.btnSearch.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.ImageClicked = global::SensorTester.Properties.Resources.BtnSearch_Click;
            this.btnSearch.ImageDisabled = null;
            this.btnSearch.ImageMouseOver = global::SensorTester.Properties.Resources.BtnSearch_Click;
            this.btnSearch.ImageNormal = global::SensorTester.Properties.Resources.BtnSearch_Default;
            this.btnSearch.Location = new System.Drawing.Point(804, 41);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Owner = null;
            this.btnSearch.Size = new System.Drawing.Size(23, 23);
            this.btnSearch.TabIndex = 25;
            this.btnSearch.TabStop = false;
            this.btnSearch.TextColor = System.Drawing.Color.Black;
            this.btnSearch.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.ToolTipText = "";
            this.btnSearch.UseToolTip = false;
            this.btnSearch.WindowRateWidth = 1F;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnRecovery
            // 
            this.btnRecovery.ButtonText = "";
            this.btnRecovery.Enabled = false;
            this.btnRecovery.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecovery.ImageClicked = global::SensorTester.Properties.Resources.BtnRecovery_Click;
            this.btnRecovery.ImageDisabled = global::SensorTester.Properties.Resources.BtnRecovery_Disabled;
            this.btnRecovery.ImageMouseOver = global::SensorTester.Properties.Resources.BtnRecovery_Click;
            this.btnRecovery.ImageNormal = global::SensorTester.Properties.Resources.BtnRecovery_Default;
            this.btnRecovery.Location = new System.Drawing.Point(3, 234);
            this.btnRecovery.Name = "btnRecovery";
            this.btnRecovery.Owner = null;
            this.btnRecovery.Size = new System.Drawing.Size(108, 30);
            this.btnRecovery.TabIndex = 26;
            this.btnRecovery.TabStop = false;
            this.btnRecovery.TextColor = System.Drawing.Color.Black;
            this.btnRecovery.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecovery.ToolTipText = "";
            this.btnRecovery.UseToolTip = false;
            this.btnRecovery.WindowRateWidth = 1F;
            this.btnRecovery.Click += new System.EventHandler(this.btnRecovery_Click);
            // 
            // btnRecoverAll
            // 
            this.btnRecoverAll.ButtonText = "";
            this.btnRecoverAll.Enabled = false;
            this.btnRecoverAll.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecoverAll.ImageClicked = global::SensorTester.Properties.Resources.BtnRecoveryAll_Click;
            this.btnRecoverAll.ImageDisabled = global::SensorTester.Properties.Resources.BtnRecoveryAll_Disabled;
            this.btnRecoverAll.ImageMouseOver = global::SensorTester.Properties.Resources.BtnRecoveryAll_Click;
            this.btnRecoverAll.ImageNormal = global::SensorTester.Properties.Resources.BtnRecoveryAll_Default;
            this.btnRecoverAll.Location = new System.Drawing.Point(117, 234);
            this.btnRecoverAll.Name = "btnRecoverAll";
            this.btnRecoverAll.Owner = null;
            this.btnRecoverAll.Size = new System.Drawing.Size(108, 30);
            this.btnRecoverAll.TabIndex = 27;
            this.btnRecoverAll.TabStop = false;
            this.btnRecoverAll.TextColor = System.Drawing.Color.Black;
            this.btnRecoverAll.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecoverAll.ToolTipText = "";
            this.btnRecoverAll.UseToolTip = false;
            this.btnRecoverAll.WindowRateWidth = 1F;
            this.btnRecoverAll.Click += new System.EventHandler(this.btnRecoverAll_Click);
            // 
            // btnOff
            // 
            this.btnOff.BackColor = System.Drawing.Color.Transparent;
            this.btnOff.ButtonText = "";
            this.btnOff.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOff.ImageClicked = global::SensorTester.Properties.Resources.BtnSignalRecovery_Click;
            this.btnOff.ImageDisabled = global::SensorTester.Properties.Resources.BtnSignalRecovery_Disabled;
            this.btnOff.ImageMouseOver = global::SensorTester.Properties.Resources.BtnSignalRecovery_Click;
            this.btnOff.ImageNormal = global::SensorTester.Properties.Resources.BtnSignalRecovery_Default;
            this.btnOff.Location = new System.Drawing.Point(748, 234);
            this.btnOff.Name = "btnOff";
            this.btnOff.Owner = null;
            this.btnOff.Size = new System.Drawing.Size(87, 30);
            this.btnOff.TabIndex = 33;
            this.btnOff.TabStop = false;
            this.btnOff.TextColor = System.Drawing.Color.Black;
            this.btnOff.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOff.ToolTipText = "";
            this.btnOff.UseToolTip = false;
            this.btnOff.WindowRateWidth = 1F;
            this.btnOff.Click += new System.EventHandler(this.btnOff_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.btnMinimize.ButtonText = "";
            this.btnMinimize.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMinimize.ImageClicked = global::SensorTester.Properties.Resources.BtnMinimize_Click;
            this.btnMinimize.ImageDisabled = null;
            this.btnMinimize.ImageMouseOver = global::SensorTester.Properties.Resources.BtnMinimize_Click;
            this.btnMinimize.ImageNormal = global::SensorTester.Properties.Resources.BtnMinimize_Default;
            this.btnMinimize.Location = new System.Drawing.Point(785, 9);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Owner = null;
            this.btnMinimize.Size = new System.Drawing.Size(20, 20);
            this.btnMinimize.TabIndex = 34;
            this.btnMinimize.TabStop = false;
            this.btnMinimize.TextColor = System.Drawing.Color.Black;
            this.btnMinimize.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMinimize.ToolTipText = "";
            this.btnMinimize.UseToolTip = false;
            this.btnMinimize.WindowRateWidth = 1F;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ButtonText = "";
            this.btnClose.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ImageClicked = global::SensorTester.Properties.Resources.BtnClose_Click;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SensorTester.Properties.Resources.BtnClose_Click;
            this.btnClose.ImageNormal = global::SensorTester.Properties.Resources.BtnClose_Default;
            this.btnClose.Location = new System.Drawing.Point(811, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(20, 20);
            this.btnClose.TabIndex = 35;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SensorTester.Properties.Resources.SensorTesterBackground;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(841, 272);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.btnOff);
            this.Controls.Add(this.btnRecoverAll);
            this.Controls.Add(this.labelSensorTagID);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnRecovery);
            this.Controls.Add(this.labelSensorTagType);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.labelSensorName);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.grpSearchSensor);
            this.Controls.Add(this.grpSearchZone);
            this.Controls.Add(this.gridCurrent);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.treeSensorTag);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.btnConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "S1 센서 테스트";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.LocationChanged += new System.EventHandler(this.FormMain_LocationChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMain_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormMain_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormMain_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.gridCurrent)).EndInit();
            this.grpSearchZone.ResumeLayout(false);
            this.grpSearchZone.PerformLayout();
            this.grpSearchSensor.ResumeLayout(false);
            this.grpSearchSensor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.button2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.button4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.button3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.button1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRecovery)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRecoverAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TreeView treeSensorTag;
        private System.Windows.Forms.Label labelSensorTagID;
        private System.Windows.Forms.Label labelSensorTagType;
        private System.Windows.Forms.Label labelSensorName;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DataGridView gridCurrent;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurrentDetect;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txtZoneSearch;
        private System.Windows.Forms.Button btnZoneSearch;
        private System.Windows.Forms.Button btnSensorSearch;
        private System.Windows.Forms.TextBox txtSensorSearch;
        private System.Windows.Forms.GroupBox grpSearchZone;
        private System.Windows.Forms.GroupBox grpSearchSensor;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private UnE.GUI.ImageButton btnSearch;
        private UnE.GUI.ImageButton btnRecovery;
        private UnE.GUI.ImageButton btnRecoverAll;
        private UnE.GUI.ImageButton button4;
        private UnE.GUI.ImageButton button3;
        private UnE.GUI.ImageButton button1;
        private UnE.GUI.ImageButton button2;
        private UnE.GUI.ImageButton btnSend;
        private UnE.GUI.ImageButton btnOff;
        private UnE.GUI.ImageButton btnMinimize;
        private UnE.GUI.ImageButton btnClose;


    }
}

