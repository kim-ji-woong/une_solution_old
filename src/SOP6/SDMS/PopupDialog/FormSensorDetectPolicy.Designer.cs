namespace SDMS
{
    partial class FormSensorDetectPolicy
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
            this.lblSecuritySignalOn = new System.Windows.Forms.Label();
            this.btnSecuritySignalOn = new UnE.GUI.ImageButton();
            this.lblPSMSignalOn = new System.Windows.Forms.Label();
            this.btnPSMSignalOn = new UnE.GUI.ImageButton();
            this.lblFireSignalOn = new System.Windows.Forms.Label();
            this.btnFireSignalOn = new UnE.GUI.ImageButton();
            this.mCmbTimeDay = new UnE.GUI.ImageComboBox();
            this.mCmbTimeHour = new UnE.GUI.ImageComboBox();
            this.mCmbTimeMin = new UnE.GUI.ImageComboBox();
            this.mCmbDetectPolicy = new UnE.GUI.ImageComboBox();
            this.lbl_DetectSensorName = new System.Windows.Forms.Label();
            this.mBtnSave = new UnE.GUI.ImageButton();
            this.lbl_CfgSignalName = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.btnSecuritySignalOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPSMSignalOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFireSignalOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBtnSave)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSecuritySignalOn
            // 
            this.lblSecuritySignalOn.AutoSize = true;
            this.lblSecuritySignalOn.BackColor = System.Drawing.Color.Transparent;
            this.lblSecuritySignalOn.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSecuritySignalOn.ForeColor = System.Drawing.Color.White;
            this.lblSecuritySignalOn.Location = new System.Drawing.Point(281, 461);
            this.lblSecuritySignalOn.Name = "lblSecuritySignalOn";
            this.lblSecuritySignalOn.Size = new System.Drawing.Size(128, 18);
            this.lblSecuritySignalOn.TabIndex = 30;
            this.lblSecuritySignalOn.Text = "방범 신호 수신";
            this.lblSecuritySignalOn.Visible = false;
            // 
            // btnSecuritySignalOn
            // 
            this.btnSecuritySignalOn.BackColor = System.Drawing.Color.Transparent;
            this.btnSecuritySignalOn.ButtonText = "";
            this.btnSecuritySignalOn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSecuritySignalOn.ImageClicked = global::SDMS.Properties.Resources.CheckBox_Click;
            this.btnSecuritySignalOn.ImageDisabled = null;
            this.btnSecuritySignalOn.ImageMouseOver = global::SDMS.Properties.Resources.CheckBox_Default;
            this.btnSecuritySignalOn.ImageNormal = global::SDMS.Properties.Resources.CheckBox_Default;
            this.btnSecuritySignalOn.Location = new System.Drawing.Point(262, 463);
            this.btnSecuritySignalOn.Name = "btnSecuritySignalOn";
            this.btnSecuritySignalOn.Owner = null;
            this.btnSecuritySignalOn.Size = new System.Drawing.Size(16, 16);
            this.btnSecuritySignalOn.TabIndex = 29;
            this.btnSecuritySignalOn.TabStop = false;
            this.btnSecuritySignalOn.TextColor = System.Drawing.Color.Black;
            this.btnSecuritySignalOn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSecuritySignalOn.ToolTipText = "";
            this.btnSecuritySignalOn.UseToolTip = false;
            this.btnSecuritySignalOn.Visible = false;
            this.btnSecuritySignalOn.WindowRateWidth = 1F;
            this.btnSecuritySignalOn.Click += new System.EventHandler(this.btnSecuritySignalOn_Click);
            // 
            // lblPSMSignalOn
            // 
            this.lblPSMSignalOn.AutoSize = true;
            this.lblPSMSignalOn.BackColor = System.Drawing.Color.Transparent;
            this.lblPSMSignalOn.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblPSMSignalOn.ForeColor = System.Drawing.Color.White;
            this.lblPSMSignalOn.Location = new System.Drawing.Point(281, 356);
            this.lblPSMSignalOn.Name = "lblPSMSignalOn";
            this.lblPSMSignalOn.Size = new System.Drawing.Size(200, 18);
            this.lblPSMSignalOn.TabIndex = 28;
            this.lblPSMSignalOn.Text = "위험물질 누출신호 수신";
            // 
            // btnPSMSignalOn
            // 
            this.btnPSMSignalOn.BackColor = System.Drawing.Color.Transparent;
            this.btnPSMSignalOn.ButtonText = "";
            this.btnPSMSignalOn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPSMSignalOn.ImageClicked = global::SDMS.Properties.Resources.CheckBox_Click;
            this.btnPSMSignalOn.ImageDisabled = null;
            this.btnPSMSignalOn.ImageMouseOver = global::SDMS.Properties.Resources.CheckBox_Default;
            this.btnPSMSignalOn.ImageNormal = global::SDMS.Properties.Resources.CheckBox_Default;
            this.btnPSMSignalOn.Location = new System.Drawing.Point(262, 357);
            this.btnPSMSignalOn.Name = "btnPSMSignalOn";
            this.btnPSMSignalOn.Owner = null;
            this.btnPSMSignalOn.Size = new System.Drawing.Size(16, 16);
            this.btnPSMSignalOn.TabIndex = 27;
            this.btnPSMSignalOn.TabStop = false;
            this.btnPSMSignalOn.TextColor = System.Drawing.Color.Black;
            this.btnPSMSignalOn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPSMSignalOn.ToolTipText = "";
            this.btnPSMSignalOn.UseToolTip = false;
            this.btnPSMSignalOn.WindowRateWidth = 1F;
            this.btnPSMSignalOn.Click += new System.EventHandler(this.btnPSMSignalOn_Click);
            // 
            // lblFireSignalOn
            // 
            this.lblFireSignalOn.AutoSize = true;
            this.lblFireSignalOn.BackColor = System.Drawing.Color.Transparent;
            this.lblFireSignalOn.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblFireSignalOn.ForeColor = System.Drawing.Color.White;
            this.lblFireSignalOn.Location = new System.Drawing.Point(112, 356);
            this.lblFireSignalOn.Name = "lblFireSignalOn";
            this.lblFireSignalOn.Size = new System.Drawing.Size(122, 18);
            this.lblFireSignalOn.TabIndex = 26;
            this.lblFireSignalOn.Text = "화재신호 수신";
            // 
            // btnFireSignalOn
            // 
            this.btnFireSignalOn.BackColor = System.Drawing.Color.Transparent;
            this.btnFireSignalOn.ButtonText = "";
            this.btnFireSignalOn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFireSignalOn.ImageClicked = global::SDMS.Properties.Resources.CheckBox_Click;
            this.btnFireSignalOn.ImageDisabled = null;
            this.btnFireSignalOn.ImageMouseOver = global::SDMS.Properties.Resources.CheckBox_Default;
            this.btnFireSignalOn.ImageNormal = global::SDMS.Properties.Resources.CheckBox_Default;
            this.btnFireSignalOn.Location = new System.Drawing.Point(93, 357);
            this.btnFireSignalOn.Name = "btnFireSignalOn";
            this.btnFireSignalOn.Owner = null;
            this.btnFireSignalOn.Size = new System.Drawing.Size(16, 16);
            this.btnFireSignalOn.TabIndex = 25;
            this.btnFireSignalOn.TabStop = false;
            this.btnFireSignalOn.TextColor = System.Drawing.Color.Black;
            this.btnFireSignalOn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFireSignalOn.ToolTipText = "";
            this.btnFireSignalOn.UseToolTip = false;
            this.btnFireSignalOn.WindowRateWidth = 1F;
            this.btnFireSignalOn.Click += new System.EventHandler(this.btnFireSignalOn_Click);
            // 
            // mCmbTimeDay
            // 
            this.mCmbTimeDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCmbTimeDay.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mCmbTimeDay.FormattingEnabled = true;
            this.mCmbTimeDay.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.mCmbTimeDay.ImageDisabled = null;
            this.mCmbTimeDay.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.mCmbTimeDay.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.mCmbTimeDay.Items.AddRange(new object[] {
            "1일",
            "2일",
            "3일",
            "5일",
            "7일",
            "10일",
            "15일",
            "30일"});
            this.mCmbTimeDay.Location = new System.Drawing.Point(141, 211);
            this.mCmbTimeDay.Name = "mCmbTimeDay";
            this.mCmbTimeDay.Owner = null;
            this.mCmbTimeDay.Size = new System.Drawing.Size(258, 25);
            this.mCmbTimeDay.TabIndex = 15;
            this.mCmbTimeDay.TextColor = System.Drawing.Color.Black;
            this.mCmbTimeDay.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mCmbTimeDay.SelectedIndexChanged += new System.EventHandler(this.CmbTimeDay_SelectedIndexChanged);
            // 
            // mCmbTimeHour
            // 
            this.mCmbTimeHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCmbTimeHour.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mCmbTimeHour.FormattingEnabled = true;
            this.mCmbTimeHour.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.mCmbTimeHour.ImageDisabled = null;
            this.mCmbTimeHour.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.mCmbTimeHour.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.mCmbTimeHour.Items.AddRange(new object[] {
            "1시간",
            "2시간",
            "3시간",
            "4시간",
            "5시간",
            "6시간",
            "8시간",
            "10시간",
            "12시간"});
            this.mCmbTimeHour.Location = new System.Drawing.Point(141, 210);
            this.mCmbTimeHour.Name = "mCmbTimeHour";
            this.mCmbTimeHour.Owner = null;
            this.mCmbTimeHour.Size = new System.Drawing.Size(258, 25);
            this.mCmbTimeHour.TabIndex = 14;
            this.mCmbTimeHour.TextColor = System.Drawing.Color.Black;
            this.mCmbTimeHour.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mCmbTimeHour.SelectedIndexChanged += new System.EventHandler(this.CmbTimeHour_SelectedIndexChanged);
            // 
            // mCmbTimeMin
            // 
            this.mCmbTimeMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCmbTimeMin.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mCmbTimeMin.FormattingEnabled = true;
            this.mCmbTimeMin.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.mCmbTimeMin.ImageDisabled = null;
            this.mCmbTimeMin.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.mCmbTimeMin.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.mCmbTimeMin.Items.AddRange(new object[] {
            "5분",
            "15분",
            "30분",
            "45분"});
            this.mCmbTimeMin.Location = new System.Drawing.Point(141, 210);
            this.mCmbTimeMin.Name = "mCmbTimeMin";
            this.mCmbTimeMin.Owner = null;
            this.mCmbTimeMin.Size = new System.Drawing.Size(258, 25);
            this.mCmbTimeMin.TabIndex = 13;
            this.mCmbTimeMin.TextColor = System.Drawing.Color.Black;
            this.mCmbTimeMin.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mCmbTimeMin.SelectedIndexChanged += new System.EventHandler(this.CmbTimeMin_SelectedIndexChanged);
            // 
            // mCmbDetectPolicy
            // 
            this.mCmbDetectPolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCmbDetectPolicy.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mCmbDetectPolicy.FormattingEnabled = true;
            this.mCmbDetectPolicy.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.mCmbDetectPolicy.ImageDisabled = null;
            this.mCmbDetectPolicy.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.mCmbDetectPolicy.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.mCmbDetectPolicy.Items.AddRange(new object[] {
            "모든 탐지 값을 표시",
            "몇 분 동안 표시하지 않습니다",
            "몇 시간 동안 표시하지 않습니다",
            "몇 일 동안 표시하지 않습니다",
            "완전히 표시하지 않습니다"});
            this.mCmbDetectPolicy.Location = new System.Drawing.Point(141, 171);
            this.mCmbDetectPolicy.Name = "mCmbDetectPolicy";
            this.mCmbDetectPolicy.Owner = null;
            this.mCmbDetectPolicy.Size = new System.Drawing.Size(258, 25);
            this.mCmbDetectPolicy.TabIndex = 12;
            this.mCmbDetectPolicy.TextColor = System.Drawing.Color.Black;
            this.mCmbDetectPolicy.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mCmbDetectPolicy.SelectedIndexChanged += new System.EventHandler(this.CmbDetectPolicySelectedIndexChanged);
            // 
            // lbl_DetectSensorName
            // 
            this.lbl_DetectSensorName.AutoSize = true;
            this.lbl_DetectSensorName.BackColor = System.Drawing.Color.Transparent;
            this.lbl_DetectSensorName.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbl_DetectSensorName.ForeColor = System.Drawing.Color.White;
            this.lbl_DetectSensorName.Location = new System.Drawing.Point(21, 68);
            this.lbl_DetectSensorName.Name = "lbl_DetectSensorName";
            this.lbl_DetectSensorName.Size = new System.Drawing.Size(156, 22);
            this.lbl_DetectSensorName.TabIndex = 11;
            this.lbl_DetectSensorName.Text = "화재센서 탐지";
            // 
            // mBtnSave
            // 
            this.mBtnSave.BackColor = System.Drawing.Color.Transparent;
            this.mBtnSave.ButtonText = "";
            this.mBtnSave.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnSave.ImageClicked = global::SDMS.Properties.Resources.Ok_101_57_Click;
            this.mBtnSave.ImageDisabled = null;
            this.mBtnSave.ImageMouseOver = global::SDMS.Properties.Resources.Ok_101_57_Click;
            this.mBtnSave.ImageNormal = global::SDMS.Properties.Resources.Ok_101_57_Default;
            this.mBtnSave.Location = new System.Drawing.Point(455, 472);
            this.mBtnSave.Name = "mBtnSave";
            this.mBtnSave.Owner = null;
            this.mBtnSave.Size = new System.Drawing.Size(50, 28);
            this.mBtnSave.TabIndex = 10;
            this.mBtnSave.TabStop = false;
            this.mBtnSave.TextColor = System.Drawing.Color.Black;
            this.mBtnSave.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mBtnSave.ToolTipText = "";
            this.mBtnSave.UseToolTip = false;
            this.mBtnSave.WindowRateWidth = 1F;
            this.mBtnSave.Click += new System.EventHandler(this.mBtnSave_Click);
            // 
            // lbl_CfgSignalName
            // 
            this.lbl_CfgSignalName.AutoSize = true;
            this.lbl_CfgSignalName.BackColor = System.Drawing.Color.Transparent;
            this.lbl_CfgSignalName.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbl_CfgSignalName.ForeColor = System.Drawing.Color.White;
            this.lbl_CfgSignalName.Location = new System.Drawing.Point(21, 279);
            this.lbl_CfgSignalName.Name = "lbl_CfgSignalName";
            this.lbl_CfgSignalName.Size = new System.Drawing.Size(218, 22);
            this.lbl_CfgSignalName.TabIndex = 9;
            this.lbl_CfgSignalName.Text = "센서 신호 수신 설정";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(130, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(283, 18);
            this.label3.TabIndex = 1;
            this.label3.Text = "오작동 처리된 센서의 탐지 값을";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.label6.Location = new System.Drawing.Point(23, 428);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(486, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "이는 수신거부일뿐이며 신호의 발생여부와는 관련이 없습니다.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(44, 316);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(439, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "센서 종류에 따라 신호 처리 여부를 결정하는 기능입니다.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(35, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(468, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "반복적으로 들어오는 오작동 값을 처리하기 위한 기능입니다.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.label5.Location = new System.Drawing.Point(23, 400);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(464, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "해당 신호가 체크 되지 않는경우 신호가 수신되지 않습니다.";
            // 
            // FormSensorDetectPolicy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BackgroundImage = global::SDMS.Properties.Resources.SensorDetectPolicy_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(523, 512);
            this.Controls.Add(this.lblSecuritySignalOn);
            this.Controls.Add(this.btnSecuritySignalOn);
            this.Controls.Add(this.lblPSMSignalOn);
            this.Controls.Add(this.btnPSMSignalOn);
            this.Controls.Add(this.lblFireSignalOn);
            this.Controls.Add(this.btnFireSignalOn);
            this.Controls.Add(this.mCmbTimeDay);
            this.Controls.Add(this.mCmbTimeHour);
            this.Controls.Add(this.mCmbTimeMin);
            this.Controls.Add(this.mCmbDetectPolicy);
            this.Controls.Add(this.lbl_DetectSensorName);
            this.Controls.Add(this.mBtnSave);
            this.Controls.Add(this.lbl_CfgSignalName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Name = "FormSensorDetectPolicy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormSensorDetectPolicy";
            ((System.ComponentModel.ISupportInitialize)(this.btnSecuritySignalOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPSMSignalOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFireSignalOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBtnSave)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbl_CfgSignalName;
        private UnE.GUI.ImageButton mBtnSave;
        private System.Windows.Forms.Label lbl_DetectSensorName;
        private UnE.GUI.ImageComboBox mCmbDetectPolicy;
        private UnE.GUI.ImageComboBox mCmbTimeMin;
        private UnE.GUI.ImageComboBox mCmbTimeHour;
        private UnE.GUI.ImageComboBox mCmbTimeDay;
        private UnE.GUI.ImageButton btnFireSignalOn;
        private System.Windows.Forms.Label lblFireSignalOn;
        private System.Windows.Forms.Label lblPSMSignalOn;
        private UnE.GUI.ImageButton btnPSMSignalOn;
        private System.Windows.Forms.Label lblSecuritySignalOn;
        private UnE.GUI.ImageButton btnSecuritySignalOn;

    }
}