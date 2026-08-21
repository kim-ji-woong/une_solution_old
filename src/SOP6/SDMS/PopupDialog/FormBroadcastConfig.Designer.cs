namespace SDMS
{
    partial class FormBroadcastConfig
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
            this.checkBoxUseBroadcastSecurityReport = new System.Windows.Forms.CheckBox();
            this.checkBoxUseBroadcastSecurityDetect = new System.Windows.Forms.CheckBox();
            this.checkBoxUseBroadcastPSMReport = new System.Windows.Forms.CheckBox();
            this.checkBoxUseBroadcastPSMDetect = new System.Windows.Forms.CheckBox();
            this.labelReportSecurity = new System.Windows.Forms.Label();
            this.labelReportPSM = new System.Windows.Forms.Label();
            this.richTextBoxSecurityReport = new System.Windows.Forms.RichTextBox();
            this.richTextBoxPSMReport = new System.Windows.Forms.RichTextBox();
            this.labelDetectSecurity = new System.Windows.Forms.Label();
            this.labelDetectPSM = new System.Windows.Forms.Label();
            this.richTextBoxSecurityDetect = new System.Windows.Forms.RichTextBox();
            this.richTextBoxPSMDetect = new System.Windows.Forms.RichTextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbConfirm2 = new System.Windows.Forms.RadioButton();
            this.rbAlways2 = new System.Windows.Forms.RadioButton();
            this.rbNone2 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbConfirm1 = new System.Windows.Forms.RadioButton();
            this.rbAlways1 = new System.Windows.Forms.RadioButton();
            this.rbNone1 = new System.Windows.Forms.RadioButton();
            this.checkBoxUseBroadcastFireReport = new System.Windows.Forms.CheckBox();
            this.checkBoxUseBroadcastFireDetect = new System.Windows.Forms.CheckBox();
            this.checkBoxUseSiren = new System.Windows.Forms.CheckBox();
            this.radioRepeatTwice = new System.Windows.Forms.RadioButton();
            this.radioRepeatOnce = new System.Windows.Forms.RadioButton();
            this.radioNoRepeat = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.richTextBoxFireReport = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBoxFireDetect = new System.Windows.Forms.RichTextBox();
            this.btnOK = new UnE.GUI.ImageButton();
            this.btnSpecialMessage = new UnE.GUI.ImageButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSpecialMessage)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxUseBroadcastSecurityReport
            // 
            this.checkBoxUseBroadcastSecurityReport.AutoSize = true;
            this.checkBoxUseBroadcastSecurityReport.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUseBroadcastSecurityReport.Font = new System.Drawing.Font("굴림", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxUseBroadcastSecurityReport.ForeColor = System.Drawing.Color.White;
            this.checkBoxUseBroadcastSecurityReport.Location = new System.Drawing.Point(153, 406);
            this.checkBoxUseBroadcastSecurityReport.Name = "checkBoxUseBroadcastSecurityReport";
            this.checkBoxUseBroadcastSecurityReport.Size = new System.Drawing.Size(181, 17);
            this.checkBoxUseBroadcastSecurityReport.TabIndex = 3;
            this.checkBoxUseBroadcastSecurityReport.Text = "방범 신고시 사내방송 실시";
            this.checkBoxUseBroadcastSecurityReport.UseVisualStyleBackColor = false;
            this.checkBoxUseBroadcastSecurityReport.Visible = false;
            // 
            // checkBoxUseBroadcastSecurityDetect
            // 
            this.checkBoxUseBroadcastSecurityDetect.AutoSize = true;
            this.checkBoxUseBroadcastSecurityDetect.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUseBroadcastSecurityDetect.Font = new System.Drawing.Font("굴림", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxUseBroadcastSecurityDetect.ForeColor = System.Drawing.Color.White;
            this.checkBoxUseBroadcastSecurityDetect.Location = new System.Drawing.Point(153, 383);
            this.checkBoxUseBroadcastSecurityDetect.Name = "checkBoxUseBroadcastSecurityDetect";
            this.checkBoxUseBroadcastSecurityDetect.Size = new System.Drawing.Size(181, 17);
            this.checkBoxUseBroadcastSecurityDetect.TabIndex = 4;
            this.checkBoxUseBroadcastSecurityDetect.Text = "방범 탐지시 사내방송 실시";
            this.checkBoxUseBroadcastSecurityDetect.UseVisualStyleBackColor = false;
            this.checkBoxUseBroadcastSecurityDetect.Visible = false;
            this.checkBoxUseBroadcastSecurityDetect.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBoxUseBroadcastPSMReport
            // 
            this.checkBoxUseBroadcastPSMReport.AutoSize = true;
            this.checkBoxUseBroadcastPSMReport.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUseBroadcastPSMReport.Font = new System.Drawing.Font("굴림", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxUseBroadcastPSMReport.ForeColor = System.Drawing.Color.White;
            this.checkBoxUseBroadcastPSMReport.Location = new System.Drawing.Point(34, 224);
            this.checkBoxUseBroadcastPSMReport.Name = "checkBoxUseBroadcastPSMReport";
            this.checkBoxUseBroadcastPSMReport.Size = new System.Drawing.Size(181, 17);
            this.checkBoxUseBroadcastPSMReport.TabIndex = 3;
            this.checkBoxUseBroadcastPSMReport.Text = "누출 신고시 사내방송 실시";
            this.checkBoxUseBroadcastPSMReport.UseVisualStyleBackColor = false;
            // 
            // checkBoxUseBroadcastPSMDetect
            // 
            this.checkBoxUseBroadcastPSMDetect.AutoSize = true;
            this.checkBoxUseBroadcastPSMDetect.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUseBroadcastPSMDetect.Font = new System.Drawing.Font("굴림", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxUseBroadcastPSMDetect.ForeColor = System.Drawing.Color.White;
            this.checkBoxUseBroadcastPSMDetect.Location = new System.Drawing.Point(34, 201);
            this.checkBoxUseBroadcastPSMDetect.Name = "checkBoxUseBroadcastPSMDetect";
            this.checkBoxUseBroadcastPSMDetect.Size = new System.Drawing.Size(181, 17);
            this.checkBoxUseBroadcastPSMDetect.TabIndex = 4;
            this.checkBoxUseBroadcastPSMDetect.Text = "누출 탐지시 사내방송 실시";
            this.checkBoxUseBroadcastPSMDetect.UseVisualStyleBackColor = false;
            this.checkBoxUseBroadcastPSMDetect.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // labelReportSecurity
            // 
            this.labelReportSecurity.AutoSize = true;
            this.labelReportSecurity.BackColor = System.Drawing.Color.Transparent;
            this.labelReportSecurity.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelReportSecurity.ForeColor = System.Drawing.Color.White;
            this.labelReportSecurity.Location = new System.Drawing.Point(624, 262);
            this.labelReportSecurity.Name = "labelReportSecurity";
            this.labelReportSecurity.Size = new System.Drawing.Size(129, 18);
            this.labelReportSecurity.TabIndex = 15;
            this.labelReportSecurity.Text = "방범신고 방송";
            this.labelReportSecurity.Visible = false;
            // 
            // labelReportPSM
            // 
            this.labelReportPSM.AutoSize = true;
            this.labelReportPSM.BackColor = System.Drawing.Color.Transparent;
            this.labelReportPSM.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelReportPSM.ForeColor = System.Drawing.Color.White;
            this.labelReportPSM.Location = new System.Drawing.Point(547, 262);
            this.labelReportPSM.Name = "labelReportPSM";
            this.labelReportPSM.Size = new System.Drawing.Size(129, 18);
            this.labelReportPSM.TabIndex = 15;
            this.labelReportPSM.Text = "누출신고 방송";
            // 
            // richTextBoxSecurityReport
            // 
            this.richTextBoxSecurityReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxSecurityReport.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.richTextBoxSecurityReport.Location = new System.Drawing.Point(551, 286);
            this.richTextBoxSecurityReport.Name = "richTextBoxSecurityReport";
            this.richTextBoxSecurityReport.Size = new System.Drawing.Size(300, 160);
            this.richTextBoxSecurityReport.TabIndex = 14;
            this.richTextBoxSecurityReport.Text = "{location}에서 방범 상황이 발생하였습니다.\n상황실 근무자들은 현장 확인하여 주시고, 나머지 직원들은 비상 방송 및 무전기를 이용하여 전파" +
    "되는 임무메시지에 따라 행동해 주시기 바랍니다.";
            this.richTextBoxSecurityReport.Visible = false;
            // 
            // richTextBoxPSMReport
            // 
            this.richTextBoxPSMReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxPSMReport.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.richTextBoxPSMReport.Location = new System.Drawing.Point(551, 285);
            this.richTextBoxPSMReport.Name = "richTextBoxPSMReport";
            this.richTextBoxPSMReport.Size = new System.Drawing.Size(300, 160);
            this.richTextBoxPSMReport.TabIndex = 14;
            this.richTextBoxPSMReport.Text = "<<안전품질실에서 알려드립니다.>>\n{location}에서 {PSMMaterial} 누출이 발생하였습니다.\n설비 담당자들은 현장 확인하여 주시고," +
    " 나머지 직원들은 비상 방송 및 무전기를 이용하여 전파되는 임무메시지에 따라 행동해 주시기 바랍니다.";
            // 
            // labelDetectSecurity
            // 
            this.labelDetectSecurity.AutoSize = true;
            this.labelDetectSecurity.BackColor = System.Drawing.Color.Transparent;
            this.labelDetectSecurity.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDetectSecurity.ForeColor = System.Drawing.Color.White;
            this.labelDetectSecurity.Location = new System.Drawing.Point(624, 56);
            this.labelDetectSecurity.Name = "labelDetectSecurity";
            this.labelDetectSecurity.Size = new System.Drawing.Size(129, 18);
            this.labelDetectSecurity.TabIndex = 13;
            this.labelDetectSecurity.Text = "방범탐지 방송";
            this.labelDetectSecurity.Visible = false;
            // 
            // labelDetectPSM
            // 
            this.labelDetectPSM.AutoSize = true;
            this.labelDetectPSM.BackColor = System.Drawing.Color.Transparent;
            this.labelDetectPSM.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDetectPSM.ForeColor = System.Drawing.Color.White;
            this.labelDetectPSM.Location = new System.Drawing.Point(547, 56);
            this.labelDetectPSM.Name = "labelDetectPSM";
            this.labelDetectPSM.Size = new System.Drawing.Size(129, 18);
            this.labelDetectPSM.TabIndex = 13;
            this.labelDetectPSM.Text = "누출탐지 방송";
            // 
            // richTextBoxSecurityDetect
            // 
            this.richTextBoxSecurityDetect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxSecurityDetect.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.richTextBoxSecurityDetect.Location = new System.Drawing.Point(551, 84);
            this.richTextBoxSecurityDetect.Name = "richTextBoxSecurityDetect";
            this.richTextBoxSecurityDetect.Size = new System.Drawing.Size(300, 160);
            this.richTextBoxSecurityDetect.TabIndex = 12;
            this.richTextBoxSecurityDetect.Text = "";
            this.richTextBoxSecurityDetect.Visible = false;
            // 
            // richTextBoxPSMDetect
            // 
            this.richTextBoxPSMDetect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxPSMDetect.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.richTextBoxPSMDetect.Location = new System.Drawing.Point(551, 84);
            this.richTextBoxPSMDetect.Name = "richTextBoxPSMDetect";
            this.richTextBoxPSMDetect.Size = new System.Drawing.Size(300, 160);
            this.richTextBoxPSMDetect.TabIndex = 12;
            this.richTextBoxPSMDetect.Text = "<<안전품질실에서 알려드립니다.>>\n{location}에서 {PSMMaterial} 누출이 탐지되었습니다.\n설비 담당자들은 현장 확인하여 주시고," +
    " 나머지 직원들은 비상 방송 및 무전기를 이용하여 전파되는 임무메시지에 따라 행동해 주시기 바랍니다.";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbConfirm2);
            this.groupBox4.Controls.Add(this.rbAlways2);
            this.groupBox4.Controls.Add(this.rbNone2);
            this.groupBox4.Location = new System.Drawing.Point(481, 473);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(136, 118);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "누출2단계 방송 옵션";
            this.groupBox4.Visible = false;
            // 
            // rbConfirm2
            // 
            this.rbConfirm2.AutoSize = true;
            this.rbConfirm2.Location = new System.Drawing.Point(21, 83);
            this.rbConfirm2.Name = "rbConfirm2";
            this.rbConfirm2.Size = new System.Drawing.Size(87, 16);
            this.rbConfirm2.TabIndex = 7;
            this.rbConfirm2.TabStop = true;
            this.rbConfirm2.Text = "사용자 확인";
            this.rbConfirm2.UseVisualStyleBackColor = true;
            // 
            // rbAlways2
            // 
            this.rbAlways2.AutoSize = true;
            this.rbAlways2.Location = new System.Drawing.Point(21, 58);
            this.rbAlways2.Name = "rbAlways2";
            this.rbAlways2.Size = new System.Drawing.Size(75, 16);
            this.rbAlways2.TabIndex = 6;
            this.rbAlways2.TabStop = true;
            this.rbAlways2.Text = "방송 실시";
            this.rbAlways2.UseVisualStyleBackColor = true;
            // 
            // rbNone2
            // 
            this.rbNone2.AutoSize = true;
            this.rbNone2.Location = new System.Drawing.Point(21, 33);
            this.rbNone2.Name = "rbNone2";
            this.rbNone2.Size = new System.Drawing.Size(75, 16);
            this.rbNone2.TabIndex = 5;
            this.rbNone2.TabStop = true;
            this.rbNone2.Text = "방송 안함";
            this.rbNone2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbConfirm1);
            this.groupBox3.Controls.Add(this.rbAlways1);
            this.groupBox3.Controls.Add(this.rbNone1);
            this.groupBox3.Location = new System.Drawing.Point(339, 473);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(136, 118);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "누출1단계  방송 옵션";
            this.groupBox3.Visible = false;
            // 
            // rbConfirm1
            // 
            this.rbConfirm1.AutoSize = true;
            this.rbConfirm1.Location = new System.Drawing.Point(21, 83);
            this.rbConfirm1.Name = "rbConfirm1";
            this.rbConfirm1.Size = new System.Drawing.Size(87, 16);
            this.rbConfirm1.TabIndex = 7;
            this.rbConfirm1.TabStop = true;
            this.rbConfirm1.Text = "사용자 확인";
            this.rbConfirm1.UseVisualStyleBackColor = true;
            // 
            // rbAlways1
            // 
            this.rbAlways1.AutoSize = true;
            this.rbAlways1.Location = new System.Drawing.Point(21, 58);
            this.rbAlways1.Name = "rbAlways1";
            this.rbAlways1.Size = new System.Drawing.Size(75, 16);
            this.rbAlways1.TabIndex = 6;
            this.rbAlways1.TabStop = true;
            this.rbAlways1.Text = "방송 실시";
            this.rbAlways1.UseVisualStyleBackColor = true;
            // 
            // rbNone1
            // 
            this.rbNone1.AutoSize = true;
            this.rbNone1.Location = new System.Drawing.Point(21, 33);
            this.rbNone1.Name = "rbNone1";
            this.rbNone1.Size = new System.Drawing.Size(75, 16);
            this.rbNone1.TabIndex = 5;
            this.rbNone1.TabStop = true;
            this.rbNone1.Text = "방송 안함";
            this.rbNone1.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseBroadcastFireReport
            // 
            this.checkBoxUseBroadcastFireReport.AutoSize = true;
            this.checkBoxUseBroadcastFireReport.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUseBroadcastFireReport.Font = new System.Drawing.Font("굴림", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxUseBroadcastFireReport.ForeColor = System.Drawing.Color.White;
            this.checkBoxUseBroadcastFireReport.Location = new System.Drawing.Point(34, 118);
            this.checkBoxUseBroadcastFireReport.Name = "checkBoxUseBroadcastFireReport";
            this.checkBoxUseBroadcastFireReport.Size = new System.Drawing.Size(181, 17);
            this.checkBoxUseBroadcastFireReport.TabIndex = 3;
            this.checkBoxUseBroadcastFireReport.Text = "화재 신고시 사내방송 실시";
            this.checkBoxUseBroadcastFireReport.UseVisualStyleBackColor = false;
            // 
            // checkBoxUseBroadcastFireDetect
            // 
            this.checkBoxUseBroadcastFireDetect.AutoSize = true;
            this.checkBoxUseBroadcastFireDetect.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUseBroadcastFireDetect.Font = new System.Drawing.Font("굴림", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxUseBroadcastFireDetect.ForeColor = System.Drawing.Color.White;
            this.checkBoxUseBroadcastFireDetect.Location = new System.Drawing.Point(34, 95);
            this.checkBoxUseBroadcastFireDetect.Name = "checkBoxUseBroadcastFireDetect";
            this.checkBoxUseBroadcastFireDetect.Size = new System.Drawing.Size(181, 17);
            this.checkBoxUseBroadcastFireDetect.TabIndex = 4;
            this.checkBoxUseBroadcastFireDetect.Text = "화재 탐지시 사내방송 실시";
            this.checkBoxUseBroadcastFireDetect.UseVisualStyleBackColor = false;
            // 
            // checkBoxUseSiren
            // 
            this.checkBoxUseSiren.AutoSize = true;
            this.checkBoxUseSiren.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUseSiren.Font = new System.Drawing.Font("굴림", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxUseSiren.ForeColor = System.Drawing.Color.White;
            this.checkBoxUseSiren.Location = new System.Drawing.Point(34, 374);
            this.checkBoxUseSiren.Name = "checkBoxUseSiren";
            this.checkBoxUseSiren.Size = new System.Drawing.Size(168, 17);
            this.checkBoxUseSiren.TabIndex = 2;
            this.checkBoxUseSiren.Text = "방송 시작시 사이렌 사용";
            this.checkBoxUseSiren.UseVisualStyleBackColor = false;
            this.checkBoxUseSiren.CheckedChanged += new System.EventHandler(this.checkBoxUseSiren_CheckedChanged);
            // 
            // radioRepeatTwice
            // 
            this.radioRepeatTwice.AutoSize = true;
            this.radioRepeatTwice.BackColor = System.Drawing.Color.Transparent;
            this.radioRepeatTwice.Font = new System.Drawing.Font("굴림", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioRepeatTwice.ForeColor = System.Drawing.Color.White;
            this.radioRepeatTwice.Location = new System.Drawing.Point(34, 351);
            this.radioRepeatTwice.Name = "radioRepeatTwice";
            this.radioRepeatTwice.Size = new System.Drawing.Size(75, 17);
            this.radioRepeatTwice.TabIndex = 3;
            this.radioRepeatTwice.TabStop = true;
            this.radioRepeatTwice.Text = "2회 반복";
            this.radioRepeatTwice.UseVisualStyleBackColor = false;
            // 
            // radioRepeatOnce
            // 
            this.radioRepeatOnce.AutoSize = true;
            this.radioRepeatOnce.BackColor = System.Drawing.Color.Transparent;
            this.radioRepeatOnce.Font = new System.Drawing.Font("굴림", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioRepeatOnce.ForeColor = System.Drawing.Color.White;
            this.radioRepeatOnce.Location = new System.Drawing.Point(35, 328);
            this.radioRepeatOnce.Name = "radioRepeatOnce";
            this.radioRepeatOnce.Size = new System.Drawing.Size(75, 17);
            this.radioRepeatOnce.TabIndex = 4;
            this.radioRepeatOnce.TabStop = true;
            this.radioRepeatOnce.Text = "1회 반복";
            this.radioRepeatOnce.UseVisualStyleBackColor = false;
            // 
            // radioNoRepeat
            // 
            this.radioNoRepeat.AutoSize = true;
            this.radioNoRepeat.BackColor = System.Drawing.Color.Transparent;
            this.radioNoRepeat.Font = new System.Drawing.Font("굴림", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioNoRepeat.ForeColor = System.Drawing.Color.White;
            this.radioNoRepeat.Location = new System.Drawing.Point(34, 305);
            this.radioNoRepeat.Name = "radioNoRepeat";
            this.radioNoRepeat.Size = new System.Drawing.Size(77, 17);
            this.radioNoRepeat.TabIndex = 5;
            this.radioNoRepeat.TabStop = true;
            this.radioNoRepeat.Text = "반복없음";
            this.radioNoRepeat.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(228, 262);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 18);
            this.label5.TabIndex = 7;
            this.label5.Text = "화재신고 방송";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(228, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 18);
            this.label4.TabIndex = 6;
            this.label4.Text = "화재탐지 방송";
            // 
            // richTextBoxFireReport
            // 
            this.richTextBoxFireReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxFireReport.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.richTextBoxFireReport.Location = new System.Drawing.Point(232, 286);
            this.richTextBoxFireReport.Name = "richTextBoxFireReport";
            this.richTextBoxFireReport.Size = new System.Drawing.Size(300, 160);
            this.richTextBoxFireReport.TabIndex = 5;
            this.richTextBoxFireReport.Text = "<<안전품질실에서 알려드립니다.>>\n{location}에서 화재가 발생하였습니다.\n소방 담당자들은 현장 확인하여 주시고, 나머지 직원들은 비상 방" +
    "송 및 무전기를 이용하여 전파되는 임무메시지에 따라 행동해 주시기 바랍니다.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(31, 473);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(240, 14);
            this.label3.TabIndex = 3;
            this.label3.Text = "<< >> 내의 메시지는 반복되지 않음";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(16, 455);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(505, 14);
            this.label2.TabIndex = 3;
            this.label2.Text = "● 상황에 따라 내용이 정해지는 것들은 특수문자 버튼을 클릭하여 확인하세요.";
            // 
            // richTextBoxFireDetect
            // 
            this.richTextBoxFireDetect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxFireDetect.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.richTextBoxFireDetect.Location = new System.Drawing.Point(232, 84);
            this.richTextBoxFireDetect.Name = "richTextBoxFireDetect";
            this.richTextBoxFireDetect.Size = new System.Drawing.Size(300, 160);
            this.richTextBoxFireDetect.TabIndex = 0;
            this.richTextBoxFireDetect.Text = "<<안전품질실에서 알려드립니다.>>\n{location}에서 화재가 탐지되었습니다.\n소방 담당자들은 현장 확인하여 주시고, 나머지 직원들은 비상 방" +
    "송 및 무전기를 이용하여 전파되는 임무메시지에 따라 행동해 주시기 바랍니다.";
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.ButtonText = "";
            this.btnOK.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ImageClicked = global::SDMS.Properties.Resources.Ok_101_57_Click;
            this.btnOK.ImageDisabled = null;
            this.btnOK.ImageMouseOver = global::SDMS.Properties.Resources.Ok_101_57_Click;
            this.btnOK.ImageNormal = global::SDMS.Properties.Resources.Ok_101_57_Default;
            this.btnOK.Location = new System.Drawing.Point(800, 455);
            this.btnOK.Name = "btnOK";
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(51, 29);
            this.btnOK.TabIndex = 5;
            this.btnOK.TabStop = false;
            this.btnOK.TextColor = System.Drawing.Color.Black;
            this.btnOK.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ToolTipText = "";
            this.btnOK.UseToolTip = false;
            this.btnOK.WindowRateWidth = 1F;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnSpecialMessage
            // 
            this.btnSpecialMessage.BackColor = System.Drawing.Color.Transparent;
            this.btnSpecialMessage.ButtonText = "";
            this.btnSpecialMessage.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSpecialMessage.ImageClicked = global::SDMS.Properties.Resources.BroadcastConfig_SpecialLetter_Click;
            this.btnSpecialMessage.ImageDisabled = null;
            this.btnSpecialMessage.ImageMouseOver = global::SDMS.Properties.Resources.BroadcastConfig_SpecialLetter_Click;
            this.btnSpecialMessage.ImageNormal = global::SDMS.Properties.Resources.BroadcastConfig_SpecialLetter_Default;
            this.btnSpecialMessage.Location = new System.Drawing.Point(712, 455);
            this.btnSpecialMessage.Name = "btnSpecialMessage";
            this.btnSpecialMessage.Owner = null;
            this.btnSpecialMessage.Size = new System.Drawing.Size(82, 29);
            this.btnSpecialMessage.TabIndex = 7;
            this.btnSpecialMessage.TabStop = false;
            this.btnSpecialMessage.TextColor = System.Drawing.Color.Black;
            this.btnSpecialMessage.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSpecialMessage.ToolTipText = "";
            this.btnSpecialMessage.UseToolTip = false;
            this.btnSpecialMessage.WindowRateWidth = 1F;
            this.btnSpecialMessage.Click += new System.EventHandler(this.btnSpecialMessage_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(15, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 18);
            this.label1.TabIndex = 16;
            this.label1.Text = "화재 방송 옵션";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(15, 169);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 18);
            this.label6.TabIndex = 17;
            this.label6.Text = "누출 방송 옵션";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(132, 351);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(136, 18);
            this.label7.TabIndex = 18;
            this.label7.Text = "방범 방송 옵션";
            this.label7.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(15, 273);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(136, 18);
            this.label8.TabIndex = 19;
            this.label8.Text = "방송 송출 옵션";
            // 
            // FormBroadcastConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BackgroundImage = global::SDMS.Properties.Resources.BroadcastConfig_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(875, 502);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.checkBoxUseSiren);
            this.Controls.Add(this.checkBoxUseBroadcastSecurityReport);
            this.Controls.Add(this.radioRepeatTwice);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.radioRepeatOnce);
            this.Controls.Add(this.checkBoxUseBroadcastSecurityDetect);
            this.Controls.Add(this.radioNoRepeat);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBoxUseBroadcastPSMReport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxUseBroadcastPSMDetect);
            this.Controls.Add(this.checkBoxUseBroadcastFireReport);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.checkBoxUseBroadcastFireDetect);
            this.Controls.Add(this.btnSpecialMessage);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelReportSecurity);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.richTextBoxFireReport);
            this.Controls.Add(this.labelReportPSM);
            this.Controls.Add(this.richTextBoxSecurityDetect);
            this.Controls.Add(this.richTextBoxFireDetect);
            this.Controls.Add(this.richTextBoxSecurityReport);
            this.Controls.Add(this.richTextBoxPSMReport);
            this.Controls.Add(this.richTextBoxPSMDetect);
            this.Controls.Add(this.labelDetectPSM);
            this.Controls.Add(this.labelDetectSecurity);
            this.Name = "FormBroadcastConfig";
            this.Text = "FormBroadcastConfig";
            this.Load += new System.EventHandler(this.FormBroadcastConfig_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSpecialMessage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBoxFireDetect;
        private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RichTextBox richTextBoxFireReport;
        private System.Windows.Forms.Label labelReportPSM;
        private System.Windows.Forms.RichTextBox richTextBoxPSMReport;
        private System.Windows.Forms.Label labelDetectPSM;
        private System.Windows.Forms.RichTextBox richTextBoxPSMDetect;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbConfirm2;
        private System.Windows.Forms.RadioButton rbAlways2;
        private System.Windows.Forms.RadioButton rbNone2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbConfirm1;
        private System.Windows.Forms.RadioButton rbAlways1;
        private System.Windows.Forms.RadioButton rbNone1;
        private System.Windows.Forms.CheckBox checkBoxUseBroadcastFireReport;
        private System.Windows.Forms.CheckBox checkBoxUseBroadcastFireDetect;
        private System.Windows.Forms.CheckBox checkBoxUseSiren;
        private System.Windows.Forms.RadioButton radioRepeatTwice;
        private System.Windows.Forms.RadioButton radioRepeatOnce;
        private System.Windows.Forms.RadioButton radioNoRepeat;
        private System.Windows.Forms.CheckBox checkBoxUseBroadcastPSMReport;
        private System.Windows.Forms.CheckBox checkBoxUseBroadcastPSMDetect;
        private System.Windows.Forms.Label labelReportSecurity;
        private System.Windows.Forms.RichTextBox richTextBoxSecurityReport;
        private System.Windows.Forms.Label labelDetectSecurity;
        private System.Windows.Forms.RichTextBox richTextBoxSecurityDetect;
        private System.Windows.Forms.CheckBox checkBoxUseBroadcastSecurityReport;
        private System.Windows.Forms.CheckBox checkBoxUseBroadcastSecurityDetect;
        private UnE.GUI.ImageButton btnOK;
        private UnE.GUI.ImageButton btnSpecialMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}