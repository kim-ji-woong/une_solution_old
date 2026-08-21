namespace KpxPipeMonitoring.Popups
{
    partial class EnvironmentPop
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelCheckSMS = new System.Windows.Forms.Label();
            this.textBoxPublicMessage = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox_alarmOccurSecond = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_alarmIgnoreMinute = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_minTemp = new System.Windows.Forms.TextBox();
            this.textBox_highLevel = new System.Windows.Forms.TextBox();
            this.textBox_maxTemp = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.button_tankInit = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.PictureBox();
            this.pictureCheckBoxSMS = new System.Windows.Forms.PictureBox();
            this.btnManager = new System.Windows.Forms.Button();
            this.btnMemberInfo = new System.Windows.Forms.Button();
            this.pictureBoxTitle = new System.Windows.Forms.PictureBox();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_leakLevel = new System.Windows.Forms.TextBox();
            this.textBox_leakTime = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBox_allTank = new System.Windows.Forms.CheckBox();
            this.textBox_alarmInterval = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_tankStableCTime = new System.Windows.Forms.TextBox();
            this.checkBox_tankStableCTimeUse = new System.Windows.Forms.CheckBox();
            this.textBox_tankStableRatio = new System.Windows.Forms.TextBox();
            this.textBox_tankStableAbsolute = new System.Windows.Forms.TextBox();
            this.radioButton_tankStableRatio = new System.Windows.Forms.RadioButton();
            this.radioButton_tankStableAbsolute = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_alarmIntervalUse = new System.Windows.Forms.CheckBox();
            this.button_tankStableSave = new System.Windows.Forms.Button();
            this.comboBox_tankStable = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_stableBeginWorkM = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_pipeStableCTime = new System.Windows.Forms.TextBox();
            this.checkBox_pipeStableCTimeUse = new System.Windows.Forms.CheckBox();
            this.textBox_pipeStableRatio = new System.Windows.Forms.TextBox();
            this.textBox_pipeStableAbsolute = new System.Windows.Forms.TextBox();
            this.radioButton_pipeStableRatio = new System.Windows.Forms.RadioButton();
            this.radioButton_pipeStableAbsolute = new System.Windows.Forms.RadioButton();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.checkBox_allPipe = new System.Windows.Forms.CheckBox();
            this.button_pipeStableSave = new System.Windows.Forms.Button();
            this.comboBox_pipeStable = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCheckBoxSMS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(181, 3);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(90, 24);
            this.labelTitle.TabIndex = 8;
            this.labelTitle.Text = "환경설정";
            this.labelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseDown);
            this.labelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseMove);
            this.labelTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseUp);
            // 
            // labelCheckSMS
            // 
            this.labelCheckSMS.AutoSize = true;
            this.labelCheckSMS.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCheckSMS.Location = new System.Drawing.Point(27, 18);
            this.labelCheckSMS.Name = "labelCheckSMS";
            this.labelCheckSMS.Size = new System.Drawing.Size(148, 14);
            this.labelCheckSMS.TabIndex = 14;
            this.labelCheckSMS.Text = "알람 발생시 문자메시지 발송";
            this.labelCheckSMS.Click += new System.EventHandler(this.CheckBoxSMS_Click);
            // 
            // textBoxPublicMessage
            // 
            this.textBoxPublicMessage.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxPublicMessage.Location = new System.Drawing.Point(8, 18);
            this.textBoxPublicMessage.Multiline = true;
            this.textBoxPublicMessage.Name = "textBoxPublicMessage";
            this.textBoxPublicMessage.Size = new System.Drawing.Size(242, 78);
            this.textBoxPublicMessage.TabIndex = 22;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Orange;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(16, 16);
            this.panel1.TabIndex = 25;
            // 
            // textBox_alarmOccurSecond
            // 
            this.textBox_alarmOccurSecond.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_alarmOccurSecond.Location = new System.Drawing.Point(99, 44);
            this.textBox_alarmOccurSecond.Name = "textBox_alarmOccurSecond";
            this.textBox_alarmOccurSecond.Size = new System.Drawing.Size(38, 21);
            this.textBox_alarmOccurSecond.TabIndex = 50;
            this.textBox_alarmOccurSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(6, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 14);
            this.label1.TabIndex = 49;
            this.label1.Text = "발생 단위 시간 : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(141, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 14);
            this.label2.TabIndex = 48;
            this.label2.Text = "분";
            // 
            // textBox_alarmIgnoreMinute
            // 
            this.textBox_alarmIgnoreMinute.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_alarmIgnoreMinute.Location = new System.Drawing.Point(99, 20);
            this.textBox_alarmIgnoreMinute.Name = "textBox_alarmIgnoreMinute";
            this.textBox_alarmIgnoreMinute.Size = new System.Drawing.Size(38, 21);
            this.textBox_alarmIgnoreMinute.TabIndex = 47;
            this.textBox_alarmIgnoreMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(6, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 14);
            this.label3.TabIndex = 45;
            this.label3.Text = "수동 조작 시간 : ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.Location = new System.Drawing.Point(141, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(18, 14);
            this.label12.TabIndex = 51;
            this.label12.Text = "초";
            // 
            // textBox_minTemp
            // 
            this.textBox_minTemp.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_minTemp.Location = new System.Drawing.Point(168, 71);
            this.textBox_minTemp.Name = "textBox_minTemp";
            this.textBox_minTemp.Size = new System.Drawing.Size(45, 21);
            this.textBox_minTemp.TabIndex = 61;
            this.textBox_minTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox_highLevel
            // 
            this.textBox_highLevel.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_highLevel.Location = new System.Drawing.Point(109, 47);
            this.textBox_highLevel.Name = "textBox_highLevel";
            this.textBox_highLevel.Size = new System.Drawing.Size(45, 21);
            this.textBox_highLevel.TabIndex = 58;
            this.textBox_highLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox_maxTemp
            // 
            this.textBox_maxTemp.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_maxTemp.Location = new System.Drawing.Point(109, 71);
            this.textBox_maxTemp.Name = "textBox_maxTemp";
            this.textBox_maxTemp.Size = new System.Drawing.Size(45, 21);
            this.textBox_maxTemp.TabIndex = 64;
            this.textBox_maxTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label13.Location = new System.Drawing.Point(7, 50);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(100, 14);
            this.label13.TabIndex = 67;
            this.label13.Text = "레벨 상한 (m)      :";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label14.Location = new System.Drawing.Point(6, 74);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(101, 14);
            this.label14.TabIndex = 68;
            this.label14.Text = "온도 상/하한 (℃) :";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label16.Location = new System.Drawing.Point(156, 74);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(12, 14);
            this.label16.TabIndex = 69;
            this.label16.Text = "/";
            // 
            // button_tankInit
            // 
            this.button_tankInit.BackColor = System.Drawing.Color.Transparent;
            this.button_tankInit.Image = global::KpxPipeMonitoring.Properties.Resources.Initialize;
            this.button_tankInit.Location = new System.Drawing.Point(9, 20);
            this.button_tankInit.Name = "button_tankInit";
            this.button_tankInit.Size = new System.Drawing.Size(48, 26);
            this.button_tankInit.TabIndex = 65;
            this.button_tankInit.UseVisualStyleBackColor = false;
            this.button_tankInit.Click += new System.EventHandler(this.button_tankInit_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.OptionClose_normal;
            this.btnClose.Location = new System.Drawing.Point(436, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(18, 18);
            this.btnClose.TabIndex = 24;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.btnClose_MouseEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.btnClose_MouseLeave);
            // 
            // pictureCheckBoxSMS
            // 
            this.pictureCheckBoxSMS.Image = global::KpxPipeMonitoring.Properties.Resources.UncheckedEdge;
            this.pictureCheckBoxSMS.Location = new System.Drawing.Point(12, 20);
            this.pictureCheckBoxSMS.Name = "pictureCheckBoxSMS";
            this.pictureCheckBoxSMS.Size = new System.Drawing.Size(11, 11);
            this.pictureCheckBoxSMS.TabIndex = 13;
            this.pictureCheckBoxSMS.TabStop = false;
            this.pictureCheckBoxSMS.Click += new System.EventHandler(this.CheckBoxSMS_Click);
            // 
            // btnManager
            // 
            this.btnManager.BackColor = System.Drawing.Color.Transparent;
            this.btnManager.Image = global::KpxPipeMonitoring.Properties.Resources.OptionManager;
            this.btnManager.Location = new System.Drawing.Point(85, 39);
            this.btnManager.Name = "btnManager";
            this.btnManager.Size = new System.Drawing.Size(66, 25);
            this.btnManager.TabIndex = 12;
            this.btnManager.UseVisualStyleBackColor = false;
            this.btnManager.Click += new System.EventHandler(this.btnManager_Click);
            // 
            // btnMemberInfo
            // 
            this.btnMemberInfo.BackColor = System.Drawing.Color.Transparent;
            this.btnMemberInfo.Image = global::KpxPipeMonitoring.Properties.Resources.OptionMemberInfo;
            this.btnMemberInfo.Location = new System.Drawing.Point(13, 40);
            this.btnMemberInfo.Name = "btnMemberInfo";
            this.btnMemberInfo.Size = new System.Drawing.Size(66, 25);
            this.btnMemberInfo.TabIndex = 11;
            this.btnMemberInfo.UseVisualStyleBackColor = false;
            this.btnMemberInfo.Click += new System.EventHandler(this.btnMemberInfo_Click);
            // 
            // pictureBoxTitle
            // 
            this.pictureBoxTitle.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxTitle.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.OptionTop;
            this.pictureBoxTitle.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxTitle.Name = "pictureBoxTitle";
            this.pictureBoxTitle.Size = new System.Drawing.Size(902, 35);
            this.pictureBoxTitle.TabIndex = 7;
            this.pictureBoxTitle.TabStop = false;
            this.pictureBoxTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseDown);
            this.pictureBoxTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseMove);
            this.pictureBoxTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseUp);
            // 
            // button_ok
            // 
            this.button_ok.BackColor = System.Drawing.Color.Transparent;
            this.button_ok.Image = global::KpxPipeMonitoring.Properties.Resources.OptionButtonConfirm;
            this.button_ok.Location = new System.Drawing.Point(355, 560);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(48, 26);
            this.button_ok.TabIndex = 4;
            this.button_ok.UseVisualStyleBackColor = false;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_cancel.Image = global::KpxPipeMonitoring.Properties.Resources.OptionButtonCancel;
            this.button_cancel.Location = new System.Drawing.Point(409, 560);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(48, 26);
            this.button_cancel.TabIndex = 3;
            this.button_cancel.UseVisualStyleBackColor = false;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.textBox_leakLevel);
            this.groupBox3.Controls.Add(this.textBox_leakTime);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.textBox_highLevel);
            this.groupBox3.Controls.Add(this.textBox_minTemp);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.textBox_maxTemp);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.button_tankInit);
            this.groupBox3.Location = new System.Drawing.Point(208, 107);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(228, 107);
            this.groupBox3.TabIndex = 83;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "탱크 옵션";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(5, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 14);
            this.label5.TabIndex = 73;
            this.label5.Text = "누유 시간 (분)       :";
            this.label5.Visible = false;
            // 
            // textBox_leakLevel
            // 
            this.textBox_leakLevel.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_leakLevel.Location = new System.Drawing.Point(108, 111);
            this.textBox_leakLevel.Name = "textBox_leakLevel";
            this.textBox_leakLevel.Size = new System.Drawing.Size(45, 21);
            this.textBox_leakLevel.TabIndex = 70;
            this.textBox_leakLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_leakLevel.Visible = false;
            // 
            // textBox_leakTime
            // 
            this.textBox_leakTime.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_leakTime.Location = new System.Drawing.Point(108, 135);
            this.textBox_leakTime.Name = "textBox_leakTime";
            this.textBox_leakTime.Size = new System.Drawing.Size(45, 21);
            this.textBox_leakTime.TabIndex = 71;
            this.textBox_leakTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_leakTime.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label11.Location = new System.Drawing.Point(6, 114);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(102, 14);
            this.label11.TabIndex = 72;
            this.label11.Text = "누유 레벨차 (m)   :";
            this.label11.Visible = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxPublicMessage);
            this.groupBox5.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox5.Location = new System.Drawing.Point(201, 446);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(256, 104);
            this.groupBox5.TabIndex = 85;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "공지사항";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnManager);
            this.groupBox6.Controls.Add(this.btnMemberInfo);
            this.groupBox6.Controls.Add(this.pictureCheckBoxSMS);
            this.groupBox6.Controls.Add(this.labelCheckSMS);
            this.groupBox6.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox6.Location = new System.Drawing.Point(8, 446);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(187, 104);
            this.groupBox6.TabIndex = 86;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "문자";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.textBox_alarmOccurSecond);
            this.groupBox7.Controls.Add(this.label3);
            this.groupBox7.Controls.Add(this.textBox_alarmIgnoreMinute);
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Location = new System.Drawing.Point(8, 556);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(218, 70);
            this.groupBox7.TabIndex = 87;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "알람 지연";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBox_allTank);
            this.groupBox4.Controls.Add(this.textBox_alarmInterval);
            this.groupBox4.Controls.Add(this.groupBox2);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.groupBox3);
            this.groupBox4.Controls.Add(this.checkBox_alarmIntervalUse);
            this.groupBox4.Controls.Add(this.button_tankStableSave);
            this.groupBox4.Controls.Add(this.comboBox_tankStable);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.textBox_stableBeginWorkM);
            this.groupBox4.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox4.Location = new System.Drawing.Point(11, 42);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(446, 230);
            this.groupBox4.TabIndex = 88;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "탱크 알람 옵션";
            // 
            // checkBox_allTank
            // 
            this.checkBox_allTank.AutoSize = true;
            this.checkBox_allTank.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox_allTank.Location = new System.Drawing.Point(208, 24);
            this.checkBox_allTank.Name = "checkBox_allTank";
            this.checkBox_allTank.Size = new System.Drawing.Size(116, 19);
            this.checkBox_allTank.TabIndex = 93;
            this.checkBox_allTank.Text = "모든 탱크에 적용";
            this.checkBox_allTank.UseVisualStyleBackColor = true;
            // 
            // textBox_alarmInterval
            // 
            this.textBox_alarmInterval.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_alarmInterval.Location = new System.Drawing.Point(177, 78);
            this.textBox_alarmInterval.Name = "textBox_alarmInterval";
            this.textBox_alarmInterval.Size = new System.Drawing.Size(54, 21);
            this.textBox_alarmInterval.TabIndex = 92;
            this.textBox_alarmInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_tankStableCTime);
            this.groupBox2.Controls.Add(this.checkBox_tankStableCTimeUse);
            this.groupBox2.Controls.Add(this.textBox_tankStableRatio);
            this.groupBox2.Controls.Add(this.textBox_tankStableAbsolute);
            this.groupBox2.Controls.Add(this.radioButton_tankStableRatio);
            this.groupBox2.Controls.Add(this.radioButton_tankStableAbsolute);
            this.groupBox2.Location = new System.Drawing.Point(10, 107);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(192, 107);
            this.groupBox2.TabIndex = 84;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "유량";
            // 
            // textBox_tankStableCTime
            // 
            this.textBox_tankStableCTime.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_tankStableCTime.Location = new System.Drawing.Point(120, 47);
            this.textBox_tankStableCTime.Name = "textBox_tankStableCTime";
            this.textBox_tankStableCTime.Size = new System.Drawing.Size(52, 21);
            this.textBox_tankStableCTime.TabIndex = 86;
            this.textBox_tankStableCTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkBox_tankStableCTimeUse
            // 
            this.checkBox_tankStableCTimeUse.AutoSize = true;
            this.checkBox_tankStableCTimeUse.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox_tankStableCTimeUse.Location = new System.Drawing.Point(11, 50);
            this.checkBox_tankStableCTimeUse.Name = "checkBox_tankStableCTimeUse";
            this.checkBox_tankStableCTimeUse.Size = new System.Drawing.Size(104, 18);
            this.checkBox_tankStableCTimeUse.TabIndex = 84;
            this.checkBox_tankStableCTimeUse.Text = "유지시간 (분)  :";
            this.checkBox_tankStableCTimeUse.UseVisualStyleBackColor = true;
            // 
            // textBox_tankStableRatio
            // 
            this.textBox_tankStableRatio.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_tankStableRatio.Location = new System.Drawing.Point(120, 19);
            this.textBox_tankStableRatio.Name = "textBox_tankStableRatio";
            this.textBox_tankStableRatio.Size = new System.Drawing.Size(52, 21);
            this.textBox_tankStableRatio.TabIndex = 18;
            this.textBox_tankStableRatio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox_tankStableAbsolute
            // 
            this.textBox_tankStableAbsolute.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_tankStableAbsolute.Location = new System.Drawing.Point(120, 69);
            this.textBox_tankStableAbsolute.Name = "textBox_tankStableAbsolute";
            this.textBox_tankStableAbsolute.Size = new System.Drawing.Size(52, 21);
            this.textBox_tankStableAbsolute.TabIndex = 78;
            this.textBox_tankStableAbsolute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_tankStableAbsolute.Visible = false;
            // 
            // radioButton_tankStableRatio
            // 
            this.radioButton_tankStableRatio.AutoSize = true;
            this.radioButton_tankStableRatio.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_tankStableRatio.Location = new System.Drawing.Point(11, 20);
            this.radioButton_tankStableRatio.Name = "radioButton_tankStableRatio";
            this.radioButton_tankStableRatio.Size = new System.Drawing.Size(102, 18);
            this.radioButton_tankStableRatio.TabIndex = 76;
            this.radioButton_tankStableRatio.TabStop = true;
            this.radioButton_tankStableRatio.Text = "비율 (%)         :";
            this.radioButton_tankStableRatio.UseVisualStyleBackColor = true;
            // 
            // radioButton_tankStableAbsolute
            // 
            this.radioButton_tankStableAbsolute.AutoSize = true;
            this.radioButton_tankStableAbsolute.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_tankStableAbsolute.Location = new System.Drawing.Point(11, 70);
            this.radioButton_tankStableAbsolute.Name = "radioButton_tankStableAbsolute";
            this.radioButton_tankStableAbsolute.Size = new System.Drawing.Size(99, 18);
            this.radioButton_tankStableAbsolute.TabIndex = 77;
            this.radioButton_tankStableAbsolute.TabStop = true;
            this.radioButton_tankStableAbsolute.Text = "절대값 (kl/h) :";
            this.radioButton_tankStableAbsolute.UseVisualStyleBackColor = true;
            this.radioButton_tankStableAbsolute.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(233, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(162, 14);
            this.label4.TabIndex = 91;
            this.label4.Text = "분 이내에 다시 발생하지 않는다";
            // 
            // checkBox_alarmIntervalUse
            // 
            this.checkBox_alarmIntervalUse.AutoSize = true;
            this.checkBox_alarmIntervalUse.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox_alarmIntervalUse.Location = new System.Drawing.Point(12, 81);
            this.checkBox_alarmIntervalUse.Name = "checkBox_alarmIntervalUse";
            this.checkBox_alarmIntervalUse.Size = new System.Drawing.Size(159, 18);
            this.checkBox_alarmIntervalUse.TabIndex = 90;
            this.checkBox_alarmIntervalUse.Text = "한번 발생한 알람은 종료 후";
            this.checkBox_alarmIntervalUse.UseVisualStyleBackColor = true;
            // 
            // button_tankStableSave
            // 
            this.button_tankStableSave.BackColor = System.Drawing.Color.Transparent;
            this.button_tankStableSave.Image = global::KpxPipeMonitoring.Properties.Resources.Save;
            this.button_tankStableSave.Location = new System.Drawing.Point(145, 18);
            this.button_tankStableSave.Name = "button_tankStableSave";
            this.button_tankStableSave.Size = new System.Drawing.Size(48, 26);
            this.button_tankStableSave.TabIndex = 72;
            this.button_tankStableSave.UseVisualStyleBackColor = false;
            this.button_tankStableSave.Click += new System.EventHandler(this.button_tankStableSave_Click);
            // 
            // comboBox_tankStable
            // 
            this.comboBox_tankStable.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox_tankStable.FormattingEnabled = true;
            this.comboBox_tankStable.Location = new System.Drawing.Point(11, 20);
            this.comboBox_tankStable.Name = "comboBox_tankStable";
            this.comboBox_tankStable.Size = new System.Drawing.Size(126, 22);
            this.comboBox_tankStable.TabIndex = 71;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(141, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 14);
            this.label9.TabIndex = 82;
            this.label9.Text = "분 기준";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(10, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 14);
            this.label6.TabIndex = 81;
            this.label6.Text = "작업 시작 후";
            // 
            // textBox_stableBeginWorkM
            // 
            this.textBox_stableBeginWorkM.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_stableBeginWorkM.Location = new System.Drawing.Point(85, 54);
            this.textBox_stableBeginWorkM.Name = "textBox_stableBeginWorkM";
            this.textBox_stableBeginWorkM.Size = new System.Drawing.Size(52, 21);
            this.textBox_stableBeginWorkM.TabIndex = 83;
            this.textBox_stableBeginWorkM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_pipeStableCTime);
            this.groupBox1.Controls.Add(this.checkBox_pipeStableCTimeUse);
            this.groupBox1.Controls.Add(this.textBox_pipeStableRatio);
            this.groupBox1.Controls.Add(this.textBox_pipeStableAbsolute);
            this.groupBox1.Controls.Add(this.radioButton_pipeStableRatio);
            this.groupBox1.Controls.Add(this.radioButton_pipeStableAbsolute);
            this.groupBox1.Location = new System.Drawing.Point(11, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(191, 81);
            this.groupBox1.TabIndex = 83;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "압력";
            // 
            // textBox_pipeStableCTime
            // 
            this.textBox_pipeStableCTime.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_pipeStableCTime.Location = new System.Drawing.Point(119, 46);
            this.textBox_pipeStableCTime.Name = "textBox_pipeStableCTime";
            this.textBox_pipeStableCTime.Size = new System.Drawing.Size(52, 21);
            this.textBox_pipeStableCTime.TabIndex = 86;
            this.textBox_pipeStableCTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkBox_pipeStableCTimeUse
            // 
            this.checkBox_pipeStableCTimeUse.AutoSize = true;
            this.checkBox_pipeStableCTimeUse.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox_pipeStableCTimeUse.Location = new System.Drawing.Point(11, 49);
            this.checkBox_pipeStableCTimeUse.Name = "checkBox_pipeStableCTimeUse";
            this.checkBox_pipeStableCTimeUse.Size = new System.Drawing.Size(104, 18);
            this.checkBox_pipeStableCTimeUse.TabIndex = 84;
            this.checkBox_pipeStableCTimeUse.Text = "유지시간 (분)  :";
            this.checkBox_pipeStableCTimeUse.UseVisualStyleBackColor = true;
            // 
            // textBox_pipeStableRatio
            // 
            this.textBox_pipeStableRatio.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_pipeStableRatio.Location = new System.Drawing.Point(119, 19);
            this.textBox_pipeStableRatio.Name = "textBox_pipeStableRatio";
            this.textBox_pipeStableRatio.Size = new System.Drawing.Size(52, 21);
            this.textBox_pipeStableRatio.TabIndex = 18;
            this.textBox_pipeStableRatio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox_pipeStableAbsolute
            // 
            this.textBox_pipeStableAbsolute.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_pipeStableAbsolute.Location = new System.Drawing.Point(130, 69);
            this.textBox_pipeStableAbsolute.Name = "textBox_pipeStableAbsolute";
            this.textBox_pipeStableAbsolute.Size = new System.Drawing.Size(52, 21);
            this.textBox_pipeStableAbsolute.TabIndex = 78;
            this.textBox_pipeStableAbsolute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_pipeStableAbsolute.Visible = false;
            // 
            // radioButton_pipeStableRatio
            // 
            this.radioButton_pipeStableRatio.AutoSize = true;
            this.radioButton_pipeStableRatio.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_pipeStableRatio.Location = new System.Drawing.Point(11, 20);
            this.radioButton_pipeStableRatio.Name = "radioButton_pipeStableRatio";
            this.radioButton_pipeStableRatio.Size = new System.Drawing.Size(102, 18);
            this.radioButton_pipeStableRatio.TabIndex = 76;
            this.radioButton_pipeStableRatio.TabStop = true;
            this.radioButton_pipeStableRatio.Text = "비율 (%)         :";
            this.radioButton_pipeStableRatio.UseVisualStyleBackColor = true;
            // 
            // radioButton_pipeStableAbsolute
            // 
            this.radioButton_pipeStableAbsolute.AutoSize = true;
            this.radioButton_pipeStableAbsolute.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_pipeStableAbsolute.Location = new System.Drawing.Point(11, 70);
            this.radioButton_pipeStableAbsolute.Name = "radioButton_pipeStableAbsolute";
            this.radioButton_pipeStableAbsolute.Size = new System.Drawing.Size(118, 18);
            this.radioButton_pipeStableAbsolute.TabIndex = 77;
            this.radioButton_pipeStableAbsolute.TabStop = true;
            this.radioButton_pipeStableAbsolute.Text = "절대값 (kg/cm²) :";
            this.radioButton_pipeStableAbsolute.UseVisualStyleBackColor = true;
            this.radioButton_pipeStableAbsolute.Visible = false;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.checkBox_allPipe);
            this.groupBox8.Controls.Add(this.groupBox1);
            this.groupBox8.Controls.Add(this.button_pipeStableSave);
            this.groupBox8.Controls.Add(this.comboBox_pipeStable);
            this.groupBox8.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox8.Location = new System.Drawing.Point(11, 287);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(446, 150);
            this.groupBox8.TabIndex = 89;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "배관 알람 옵션";
            // 
            // checkBox_allPipe
            // 
            this.checkBox_allPipe.AutoSize = true;
            this.checkBox_allPipe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox_allPipe.Location = new System.Drawing.Point(205, 20);
            this.checkBox_allPipe.Name = "checkBox_allPipe";
            this.checkBox_allPipe.Size = new System.Drawing.Size(116, 19);
            this.checkBox_allPipe.TabIndex = 93;
            this.checkBox_allPipe.Text = "모든 배관에 적용";
            this.checkBox_allPipe.UseVisualStyleBackColor = true;
            // 
            // button_pipeStableSave
            // 
            this.button_pipeStableSave.BackColor = System.Drawing.Color.Transparent;
            this.button_pipeStableSave.Image = global::KpxPipeMonitoring.Properties.Resources.Save;
            this.button_pipeStableSave.Location = new System.Drawing.Point(145, 18);
            this.button_pipeStableSave.Name = "button_pipeStableSave";
            this.button_pipeStableSave.Size = new System.Drawing.Size(48, 26);
            this.button_pipeStableSave.TabIndex = 72;
            this.button_pipeStableSave.UseVisualStyleBackColor = false;
            this.button_pipeStableSave.Click += new System.EventHandler(this.button_pipeStableSave_Click);
            // 
            // comboBox_pipeStable
            // 
            this.comboBox_pipeStable.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox_pipeStable.FormattingEnabled = true;
            this.comboBox_pipeStable.Location = new System.Drawing.Point(11, 20);
            this.comboBox_pipeStable.Name = "comboBox_pipeStable";
            this.comboBox_pipeStable.Size = new System.Drawing.Size(126, 22);
            this.comboBox_pipeStable.TabIndex = 71;
            // 
            // EnvironmentPop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 603);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.pictureBoxTitle);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "EnvironmentPop";
            this.Text = "환경설정";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.Load += new System.EventHandler(this.EnvironmentPop_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCheckBoxSMS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.PictureBox pictureBoxTitle;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button btnMemberInfo;
        private System.Windows.Forms.Button btnManager;
        private System.Windows.Forms.PictureBox pictureCheckBoxSMS;
        private System.Windows.Forms.Label labelCheckSMS;
        private System.Windows.Forms.TextBox textBoxPublicMessage;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.PictureBox btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox_alarmOccurSecond;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_alarmIgnoreMinute;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_minTemp;
        private System.Windows.Forms.TextBox textBox_highLevel;
        private System.Windows.Forms.TextBox textBox_maxTemp;
        private System.Windows.Forms.Button button_tankInit;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_tankStableCTime;
        private System.Windows.Forms.CheckBox checkBox_tankStableCTimeUse;
        private System.Windows.Forms.TextBox textBox_tankStableRatio;
        private System.Windows.Forms.TextBox textBox_tankStableAbsolute;
        private System.Windows.Forms.RadioButton radioButton_tankStableRatio;
        private System.Windows.Forms.RadioButton radioButton_tankStableAbsolute;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_alarmInterval;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox_alarmIntervalUse;
        private System.Windows.Forms.TextBox textBox_pipeStableCTime;
        private System.Windows.Forms.CheckBox checkBox_pipeStableCTimeUse;
        private System.Windows.Forms.TextBox textBox_stableBeginWorkM;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_pipeStableRatio;
        private System.Windows.Forms.TextBox textBox_pipeStableAbsolute;
        private System.Windows.Forms.RadioButton radioButton_pipeStableRatio;
        private System.Windows.Forms.RadioButton radioButton_pipeStableAbsolute;
        private System.Windows.Forms.Button button_tankStableSave;
        private System.Windows.Forms.ComboBox comboBox_tankStable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_leakLevel;
        private System.Windows.Forms.TextBox textBox_leakTime;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkBox_allTank;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox checkBox_allPipe;
        private System.Windows.Forms.Button button_pipeStableSave;
        private System.Windows.Forms.ComboBox comboBox_pipeStable;
    }
}