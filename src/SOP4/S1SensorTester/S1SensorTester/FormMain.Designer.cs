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
            this.btnSend = new System.Windows.Forms.Button();
            this.btnRecovery = new System.Windows.Forms.Button();
            this.btnRecoverAll = new System.Windows.Forms.Button();
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
            this.grpSensorInfo = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnOff = new System.Windows.Forms.Button();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridCurrent)).BeginInit();
            this.grpSearchZone.SuspendLayout();
            this.grpSearchSensor.SuspendLayout();
            this.grpSensorInfo.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(827, 260);
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
            this.textBox4.Location = new System.Drawing.Point(651, 262);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(170, 21);
            this.textBox4.TabIndex = 7;
            this.textBox4.Text = "127.0.0.1";
            this.textBox4.Visible = false;
            // 
            // treeSensorTag
            // 
            this.treeSensorTag.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeSensorTag.HideSelection = false;
            this.treeSensorTag.Location = new System.Drawing.Point(255, 32);
            this.treeSensorTag.Name = "treeSensorTag";
            this.treeSensorTag.Size = new System.Drawing.Size(364, 190);
            this.treeSensorTag.TabIndex = 8;
            this.treeSensorTag.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSensorTag_AfterSelect);
            // 
            // labelSensorTagID
            // 
            this.labelSensorTagID.AutoSize = true;
            this.labelSensorTagID.Location = new System.Drawing.Point(6, 25);
            this.labelSensorTagID.Name = "labelSensorTagID";
            this.labelSensorTagID.Size = new System.Drawing.Size(41, 12);
            this.labelSensorTagID.TabIndex = 9;
            this.labelSensorTagID.Text = "번호 : ";
            // 
            // labelSensorTagType
            // 
            this.labelSensorTagType.AutoSize = true;
            this.labelSensorTagType.Location = new System.Drawing.Point(6, 50);
            this.labelSensorTagType.Name = "labelSensorTagType";
            this.labelSensorTagType.Size = new System.Drawing.Size(41, 12);
            this.labelSensorTagType.TabIndex = 9;
            this.labelSensorTagType.Text = "타입 : ";
            // 
            // btnSend
            // 
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(6, 100);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "신호전송";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnRecovery
            // 
            this.btnRecovery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRecovery.Enabled = false;
            this.btnRecovery.Location = new System.Drawing.Point(3, 199);
            this.btnRecovery.Name = "btnRecovery";
            this.btnRecovery.Size = new System.Drawing.Size(120, 23);
            this.btnRecovery.TabIndex = 10;
            this.btnRecovery.Text = "선택한 센서복구";
            this.btnRecovery.UseVisualStyleBackColor = true;
            this.btnRecovery.Click += new System.EventHandler(this.btnRecovery_Click);
            // 
            // btnRecoverAll
            // 
            this.btnRecoverAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRecoverAll.Enabled = false;
            this.btnRecoverAll.Location = new System.Drawing.Point(129, 199);
            this.btnRecoverAll.Name = "btnRecoverAll";
            this.btnRecoverAll.Size = new System.Drawing.Size(120, 23);
            this.btnRecoverAll.TabIndex = 10;
            this.btnRecoverAll.Text = "모든 센서복구";
            this.btnRecoverAll.UseVisualStyleBackColor = true;
            this.btnRecoverAll.Click += new System.EventHandler(this.btnRecoverAll_Click);
            // 
            // labelSensorName
            // 
            this.labelSensorName.AutoSize = true;
            this.labelSensorName.Location = new System.Drawing.Point(6, 75);
            this.labelSensorName.Name = "labelSensorName";
            this.labelSensorName.Size = new System.Drawing.Size(41, 12);
            this.labelSensorName.TabIndex = 9;
            this.labelSensorName.Text = "이름 : ";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(651, 289);
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
            this.gridCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridCurrent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCurrent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCurrentDetect});
            this.gridCurrent.Location = new System.Drawing.Point(3, 3);
            this.gridCurrent.MultiSelect = false;
            this.gridCurrent.Name = "gridCurrent";
            this.gridCurrent.ReadOnly = true;
            this.gridCurrent.RowHeadersVisible = false;
            this.gridCurrent.RowTemplate.Height = 23;
            this.gridCurrent.Size = new System.Drawing.Size(246, 190);
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
            this.grpSearchZone.Location = new System.Drawing.Point(933, 12);
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
            this.grpSearchSensor.Location = new System.Drawing.Point(927, 68);
            this.grpSearchSensor.Name = "grpSearchSensor";
            this.grpSearchSensor.Size = new System.Drawing.Size(282, 50);
            this.grpSearchSensor.TabIndex = 18;
            this.grpSearchSensor.TabStop = false;
            this.grpSearchSensor.Text = "센서 검색";
            this.grpSearchSensor.Visible = false;
            // 
            // grpSensorInfo
            // 
            this.grpSensorInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSensorInfo.Controls.Add(this.button4);
            this.grpSensorInfo.Controls.Add(this.button3);
            this.grpSensorInfo.Controls.Add(this.button2);
            this.grpSensorInfo.Controls.Add(this.button1);
            this.grpSensorInfo.Controls.Add(this.labelSensorTagID);
            this.grpSensorInfo.Controls.Add(this.btnOff);
            this.grpSensorInfo.Controls.Add(this.btnSend);
            this.grpSensorInfo.Controls.Add(this.labelSensorTagType);
            this.grpSensorInfo.Controls.Add(this.labelSensorName);
            this.grpSensorInfo.Location = new System.Drawing.Point(625, 59);
            this.grpSensorInfo.Name = "grpSensorInfo";
            this.grpSensorInfo.Size = new System.Drawing.Size(282, 163);
            this.grpSensorInfo.TabIndex = 19;
            this.grpSensorInfo.TabStop = false;
            this.grpSensorInfo.Text = "센서 상세 정보";
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(87, 129);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 13;
            this.button4.Text = "장애복구";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(6, 129);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 12;
            this.button3.Text = "장애신호";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(168, 100);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "3단계알람";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(87, 100);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "2단계알람";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnOff
            // 
            this.btnOff.Location = new System.Drawing.Point(168, 129);
            this.btnOff.Name = "btnOff";
            this.btnOff.Size = new System.Drawing.Size(75, 23);
            this.btnOff.TabIndex = 5;
            this.btnOff.Text = "신호복구";
            this.btnOff.UseVisualStyleBackColor = true;
            this.btnOff.Click += new System.EventHandler(this.btnOff_Click);
            // 
            // grpSearch
            // 
            this.grpSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSearch.Controls.Add(this.txtSearch);
            this.grpSearch.Controls.Add(this.btnSearch);
            this.grpSearch.Location = new System.Drawing.Point(625, 3);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(282, 50);
            this.grpSearch.TabIndex = 20;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "검색";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(8, 20);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(187, 21);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown_1);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(201, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "검색";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(265, 10);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(72, 16);
            this.checkBox1.TabIndex = 21;
            this.checkBox1.Text = "화재센서";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(343, 10);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(85, 16);
            this.checkBox2.TabIndex = 22;
            this.checkBox2.Text = "S1 Access";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Location = new System.Drawing.Point(445, 10);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(59, 16);
            this.checkBox3.TabIndex = 23;
            this.checkBox3.Text = "SVMS";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.Location = new System.Drawing.Point(525, 10);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(74, 16);
            this.checkBox4.TabIndex = 24;
            this.checkBox4.Text = "EMPOLL";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 225);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.grpSearch);
            this.Controls.Add(this.grpSensorInfo);
            this.Controls.Add(this.grpSearchSensor);
            this.Controls.Add(this.grpSearchZone);
            this.Controls.Add(this.gridCurrent);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.btnRecoverAll);
            this.Controls.Add(this.btnRecovery);
            this.Controls.Add(this.treeSensorTag);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.btnConnect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "S1 센서 테스트";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridCurrent)).EndInit();
            this.grpSearchZone.ResumeLayout(false);
            this.grpSearchZone.PerformLayout();
            this.grpSearchSensor.ResumeLayout(false);
            this.grpSearchSensor.PerformLayout();
            this.grpSensorInfo.ResumeLayout(false);
            this.grpSensorInfo.PerformLayout();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TreeView treeSensorTag;
        private System.Windows.Forms.Label labelSensorTagID;
        private System.Windows.Forms.Label labelSensorTagType;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnRecovery;
        private System.Windows.Forms.Button btnRecoverAll;
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
        private System.Windows.Forms.GroupBox grpSensorInfo;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Button btnOff;


    }
}

