namespace AlarmButtonSimulator
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
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.treeSensorTag = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelSensorTagID = new System.Windows.Forms.Label();
            this.labelSensorTagType = new System.Windows.Forms.Label();
            this.labelSensorName = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxBroadcastSiren = new System.Windows.Forms.CheckBox();
            this.checkBoxBroadcast3 = new System.Windows.Forms.CheckBox();
            this.checkBoxBroadcast2 = new System.Windows.Forms.CheckBox();
            this.checkBoxBroadcast1 = new System.Windows.Forms.CheckBox();
            this.checkBoxSMS3 = new System.Windows.Forms.CheckBox();
            this.checkBoxSMS2 = new System.Windows.Forms.CheckBox();
            this.checkBoxSMS1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxSMSMessage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxBroadcastMessage = new System.Windows.Forms.TextBox();
            this.textBoxSMSReceivers = new System.Windows.Forms.TextBox();
            this.btnSMSReceivers = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbText = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnPortClose = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbStopBits = new System.Windows.Forms.ComboBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbDataBits = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbBRate = new System.Windows.Forms.ComboBox();
            this.cmbPort = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(3, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(283, 21);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(292, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "검색";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // treeSensorTag
            // 
            this.treeSensorTag.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeSensorTag.HideSelection = false;
            this.treeSensorTag.Location = new System.Drawing.Point(3, 29);
            this.treeSensorTag.Name = "treeSensorTag";
            this.treeSensorTag.Size = new System.Drawing.Size(364, 615);
            this.treeSensorTag.TabIndex = 9;
            this.treeSensorTag.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSensorTag_AfterSelect);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelSensorTagID);
            this.groupBox1.Controls.Add(this.labelSensorTagType);
            this.groupBox1.Controls.Add(this.labelSensorName);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Location = new System.Drawing.Point(373, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(265, 104);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "현재 설정된 신호";
            // 
            // labelSensorTagID
            // 
            this.labelSensorTagID.AutoSize = true;
            this.labelSensorTagID.Location = new System.Drawing.Point(6, 25);
            this.labelSensorTagID.Name = "labelSensorTagID";
            this.labelSensorTagID.Size = new System.Drawing.Size(41, 12);
            this.labelSensorTagID.TabIndex = 11;
            this.labelSensorTagID.Text = "번호 : ";
            // 
            // labelSensorTagType
            // 
            this.labelSensorTagType.AutoSize = true;
            this.labelSensorTagType.Location = new System.Drawing.Point(6, 50);
            this.labelSensorTagType.Name = "labelSensorTagType";
            this.labelSensorTagType.Size = new System.Drawing.Size(41, 12);
            this.labelSensorTagType.TabIndex = 12;
            this.labelSensorTagType.Text = "타입 : ";
            // 
            // labelSensorName
            // 
            this.labelSensorName.AutoSize = true;
            this.labelSensorName.Location = new System.Drawing.Point(6, 75);
            this.labelSensorName.Name = "labelSensorName";
            this.labelSensorName.Size = new System.Drawing.Size(41, 12);
            this.labelSensorName.TabIndex = 13;
            this.labelSensorName.Text = "이름 : ";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(174, 20);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(85, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "신호 초기화";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxBroadcastSiren);
            this.groupBox2.Controls.Add(this.checkBoxBroadcast3);
            this.groupBox2.Controls.Add(this.checkBoxBroadcast2);
            this.groupBox2.Controls.Add(this.checkBoxBroadcast1);
            this.groupBox2.Controls.Add(this.checkBoxSMS3);
            this.groupBox2.Controls.Add(this.checkBoxSMS2);
            this.groupBox2.Controls.Add(this.checkBoxSMS1);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(644, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(265, 104);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "신호 옵션";
            // 
            // checkBoxBroadcastSiren
            // 
            this.checkBoxBroadcastSiren.AutoSize = true;
            this.checkBoxBroadcastSiren.Location = new System.Drawing.Point(64, 83);
            this.checkBoxBroadcastSiren.Name = "checkBoxBroadcastSiren";
            this.checkBoxBroadcastSiren.Size = new System.Drawing.Size(128, 16);
            this.checkBoxBroadcastSiren.TabIndex = 15;
            this.checkBoxBroadcastSiren.Text = "방송시 사이렌 사용";
            this.checkBoxBroadcastSiren.UseVisualStyleBackColor = true;
            this.checkBoxBroadcastSiren.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxBroadcast3
            // 
            this.checkBoxBroadcast3.AutoSize = true;
            this.checkBoxBroadcast3.Location = new System.Drawing.Point(154, 62);
            this.checkBoxBroadcast3.Name = "checkBoxBroadcast3";
            this.checkBoxBroadcast3.Size = new System.Drawing.Size(48, 16);
            this.checkBoxBroadcast3.TabIndex = 14;
            this.checkBoxBroadcast3.Text = "방송";
            this.checkBoxBroadcast3.UseVisualStyleBackColor = true;
            this.checkBoxBroadcast3.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxBroadcast2
            // 
            this.checkBoxBroadcast2.AutoSize = true;
            this.checkBoxBroadcast2.Location = new System.Drawing.Point(154, 39);
            this.checkBoxBroadcast2.Name = "checkBoxBroadcast2";
            this.checkBoxBroadcast2.Size = new System.Drawing.Size(48, 16);
            this.checkBoxBroadcast2.TabIndex = 14;
            this.checkBoxBroadcast2.Text = "방송";
            this.checkBoxBroadcast2.UseVisualStyleBackColor = true;
            this.checkBoxBroadcast2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxBroadcast1
            // 
            this.checkBoxBroadcast1.AutoSize = true;
            this.checkBoxBroadcast1.Location = new System.Drawing.Point(154, 16);
            this.checkBoxBroadcast1.Name = "checkBoxBroadcast1";
            this.checkBoxBroadcast1.Size = new System.Drawing.Size(48, 16);
            this.checkBoxBroadcast1.TabIndex = 14;
            this.checkBoxBroadcast1.Text = "방송";
            this.checkBoxBroadcast1.UseVisualStyleBackColor = true;
            this.checkBoxBroadcast1.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxSMS3
            // 
            this.checkBoxSMS3.AutoSize = true;
            this.checkBoxSMS3.Location = new System.Drawing.Point(64, 62);
            this.checkBoxSMS3.Name = "checkBoxSMS3";
            this.checkBoxSMS3.Size = new System.Drawing.Size(84, 16);
            this.checkBoxSMS3.TabIndex = 14;
            this.checkBoxSMS3.Text = "문자메시지";
            this.checkBoxSMS3.UseVisualStyleBackColor = true;
            this.checkBoxSMS3.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxSMS2
            // 
            this.checkBoxSMS2.AutoSize = true;
            this.checkBoxSMS2.Location = new System.Drawing.Point(64, 39);
            this.checkBoxSMS2.Name = "checkBoxSMS2";
            this.checkBoxSMS2.Size = new System.Drawing.Size(84, 16);
            this.checkBoxSMS2.TabIndex = 14;
            this.checkBoxSMS2.Text = "문자메시지";
            this.checkBoxSMS2.UseVisualStyleBackColor = true;
            this.checkBoxSMS2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxSMS1
            // 
            this.checkBoxSMS1.AutoSize = true;
            this.checkBoxSMS1.Location = new System.Drawing.Point(64, 16);
            this.checkBoxSMS1.Name = "checkBoxSMS1";
            this.checkBoxSMS1.Size = new System.Drawing.Size(84, 16);
            this.checkBoxSMS1.TabIndex = 14;
            this.checkBoxSMS1.Text = "문자메시지";
            this.checkBoxSMS1.UseVisualStyleBackColor = true;
            this.checkBoxSMS1.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "버튼1 : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "버튼2 : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "버튼3 : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(373, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "문자메시지";
            // 
            // textBoxSMSMessage
            // 
            this.textBoxSMSMessage.Location = new System.Drawing.Point(373, 153);
            this.textBoxSMSMessage.Multiline = true;
            this.textBoxSMSMessage.Name = "textBoxSMSMessage";
            this.textBoxSMSMessage.Size = new System.Drawing.Size(265, 66);
            this.textBoxSMSMessage.TabIndex = 12;
            this.textBoxSMSMessage.Text = "{location}에서 화재 탐지되었습니다.";
            this.textBoxSMSMessage.TextChanged += new System.EventHandler(this.textBoxMessage_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(644, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "방송메시지";
            // 
            // textBoxBroadcastMessage
            // 
            this.textBoxBroadcastMessage.Location = new System.Drawing.Point(644, 153);
            this.textBoxBroadcastMessage.Multiline = true;
            this.textBoxBroadcastMessage.Name = "textBoxBroadcastMessage";
            this.textBoxBroadcastMessage.Size = new System.Drawing.Size(265, 66);
            this.textBoxBroadcastMessage.TabIndex = 12;
            this.textBoxBroadcastMessage.Text = "안전품질실에서 알려드립니다.\r\n{location}에서 화재가 탐지되었습니다.\r\n소방 담당자들은, 현장 확인하여 주시고, 나머지 직원들은 비상 방송" +
    " 및 무전기를 이용하여, 전파되는 임무메시지에 따라 행동해 주시기 바랍니다.";
            this.textBoxBroadcastMessage.TextChanged += new System.EventHandler(this.textBoxMessage_TextChanged);
            // 
            // textBoxSMSReceivers
            // 
            this.textBoxSMSReceivers.BackColor = System.Drawing.Color.White;
            this.textBoxSMSReceivers.Location = new System.Drawing.Point(375, 4);
            this.textBoxSMSReceivers.Name = "textBoxSMSReceivers";
            this.textBoxSMSReceivers.ReadOnly = true;
            this.textBoxSMSReceivers.Size = new System.Drawing.Size(417, 21);
            this.textBoxSMSReceivers.TabIndex = 3;
            // 
            // btnSMSReceivers
            // 
            this.btnSMSReceivers.Location = new System.Drawing.Point(794, 2);
            this.btnSMSReceivers.Name = "btnSMSReceivers";
            this.btnSMSReceivers.Size = new System.Drawing.Size(115, 23);
            this.btnSMSReceivers.TabIndex = 2;
            this.btnSMSReceivers.Text = "문자메시지 수신자";
            this.btnSMSReceivers.UseVisualStyleBackColor = true;
            this.btnSMSReceivers.Click += new System.EventHandler(this.btnSMSReceivers_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel1.Controls.Add(this.rbText);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.lbStatus);
            this.panel1.Location = new System.Drawing.Point(372, 232);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(536, 391);
            this.panel1.TabIndex = 13;
            // 
            // rbText
            // 
            this.rbText.Location = new System.Drawing.Point(268, 82);
            this.rbText.Name = "rbText";
            this.rbText.ReadOnly = true;
            this.rbText.Size = new System.Drawing.Size(262, 290);
            this.rbText.TabIndex = 15;
            this.rbText.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnPortClose);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.cmbStopBits);
            this.groupBox3.Controls.Add(this.btnOpen);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.cmbParity);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.cmbDataBits);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.cmbBRate);
            this.groupBox3.Controls.Add(this.cmbPort);
            this.groupBox3.Location = new System.Drawing.Point(9, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(229, 373);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // btnPortClose
            // 
            this.btnPortClose.BackColor = System.Drawing.Color.Red;
            this.btnPortClose.ForeColor = System.Drawing.Color.Black;
            this.btnPortClose.Location = new System.Drawing.Point(42, 314);
            this.btnPortClose.Name = "btnPortClose";
            this.btnPortClose.Size = new System.Drawing.Size(171, 27);
            this.btnPortClose.TabIndex = 13;
            this.btnPortClose.Text = "Port Close";
            this.btnPortClose.UseVisualStyleBackColor = false;
            this.btnPortClose.Visible = false;
            this.btnPortClose.Click += new System.EventHandler(this.btnPortClose_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(18, 209);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 14);
            this.label7.TabIndex = 10;
            this.label7.Text = "Stop Bits";
            // 
            // cmbStopBits
            // 
            this.cmbStopBits.FormattingEnabled = true;
            this.cmbStopBits.Items.AddRange(new object[] {
            "NONE",
            "1",
            "1.5",
            "2"});
            this.cmbStopBits.Location = new System.Drawing.Point(42, 226);
            this.cmbStopBits.Name = "cmbStopBits";
            this.cmbStopBits.Size = new System.Drawing.Size(171, 20);
            this.cmbStopBits.TabIndex = 9;
            this.cmbStopBits.SelectedIndex = 1;
            this.cmbStopBits.SelectedIndexChanged += new System.EventHandler(this.cmbStopBits_SelectedIndexChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.BackColor = System.Drawing.Color.Yellow;
            this.btnOpen.ForeColor = System.Drawing.Color.Black;
            this.btnOpen.Location = new System.Drawing.Point(40, 262);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(171, 27);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Port Open";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(18, 166);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 14);
            this.label8.TabIndex = 8;
            this.label8.Text = "Parity";
            // 
            // cmbParity
            // 
            this.cmbParity.FormattingEnabled = true;
            this.cmbParity.Items.AddRange(new object[] {
            "EVEN",
            "MARK",
            "NONE",
            "ODD",
            "SPACE"});
            this.cmbParity.Location = new System.Drawing.Point(42, 182);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(171, 20);
            this.cmbParity.TabIndex = 7;
            this.cmbParity.SelectedIndex = 2;
            this.cmbParity.SelectedIndexChanged += new System.EventHandler(this.cmbParity_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(16, 121);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 14);
            this.label9.TabIndex = 6;
            this.label9.Text = "Data bits";
            // 
            // cmbDataBits
            // 
            this.cmbDataBits.FormattingEnabled = true;
            this.cmbDataBits.Items.AddRange(new object[] {
            "8 bits",
            "7 bits"});
            this.cmbDataBits.Location = new System.Drawing.Point(40, 138);
            this.cmbDataBits.Name = "cmbDataBits";
            this.cmbDataBits.Size = new System.Drawing.Size(171, 20);
            this.cmbDataBits.TabIndex = 5;
            this.cmbDataBits.SelectedIndex = 0;
            this.cmbDataBits.SelectedIndexChanged += new System.EventHandler(this.cmbDataBits_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(16, 77);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 14);
            this.label10.TabIndex = 4;
            this.label10.Text = "Baud Rate";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(16, 30);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 14);
            this.label11.TabIndex = 3;
            this.label11.Text = "COM Port";
            // 
            // cmbBRate
            // 
            this.cmbBRate.FormattingEnabled = true;
            this.cmbBRate.Items.AddRange(new object[] {
            "9600 bps",
            "14400 bps",
            "19200 bps",
            "38400 bps",
            "57600 bps",
            "115200 bps"});
            this.cmbBRate.Location = new System.Drawing.Point(40, 93);
            this.cmbBRate.Name = "cmbBRate";
            this.cmbBRate.Size = new System.Drawing.Size(171, 20);
            this.cmbBRate.TabIndex = 1;
            this.cmbBRate.SelectedIndex = 0;
            this.cmbBRate.SelectedIndexChanged += new System.EventHandler(this.cmbBRate_SelectedIndexChanged);
            // 
            // cmbPort
            // 
            this.cmbPort.FormattingEnabled = true;
            this.cmbPort.Location = new System.Drawing.Point(40, 49);
            this.cmbPort.Name = "cmbPort";
            this.cmbPort.Size = new System.Drawing.Size(171, 20);
            this.cmbPort.TabIndex = 0;
            this.cmbPort.SelectedIndexChanged += new System.EventHandler(this.cmbPort_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(269, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 14);
            this.label6.TabIndex = 11;
            this.label6.Text = "State";
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lbStatus.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStatus.ForeColor = System.Drawing.Color.Yellow;
            this.lbStatus.Location = new System.Drawing.Point(269, 45);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(123, 25);
            this.lbStatus.TabIndex = 12;
            this.lbStatus.Text = "Not Connect";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 650);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBoxBroadcastMessage);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxSMSMessage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.treeSensorTag);
            this.Controls.Add(this.textBoxSMSReceivers);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnSMSReceivers);
            this.Controls.Add(this.btnSearch);
            this.Name = "FormMain";
            this.Text = "알람버튼 신호 수신기";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TreeView treeSensorTag;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelSensorTagID;
        private System.Windows.Forms.Label labelSensorTagType;
        private System.Windows.Forms.Label labelSensorName;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxBroadcast3;
        private System.Windows.Forms.CheckBox checkBoxBroadcast2;
        private System.Windows.Forms.CheckBox checkBoxBroadcast1;
        private System.Windows.Forms.CheckBox checkBoxSMS3;
        private System.Windows.Forms.CheckBox checkBoxSMS2;
        private System.Windows.Forms.CheckBox checkBoxSMS1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxSMSMessage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxBroadcastMessage;
        private System.Windows.Forms.TextBox textBoxSMSReceivers;
        private System.Windows.Forms.Button btnSMSReceivers;
        private System.Windows.Forms.CheckBox checkBoxBroadcastSiren;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnPortClose;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbStopBits;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbDataBits;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbBRate;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.RichTextBox rbText;
    }
}

