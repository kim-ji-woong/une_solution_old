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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBoxSensorSignal = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ckbPSMSignalOn = new System.Windows.Forms.CheckBox();
            this.ckbFireSignalOn = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mCmbTimeMin = new System.Windows.Forms.ComboBox();
            this.mCmbDetectPolicy = new System.Windows.Forms.ComboBox();
            this.mCmbTimeDay = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mCmbTimeHour = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mBtnCancel = new System.Windows.Forms.Button();
            this.mBtnSave = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBoxSensorSignal.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(492, 47);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(20, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "탐지 관리";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.groupBoxSensorSignal);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.mBtnCancel);
            this.panel2.Controls.Add(this.mBtnSave);
            this.panel2.Location = new System.Drawing.Point(12, 72);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(492, 459);
            this.panel2.TabIndex = 2;
            // 
            // groupBoxSensorSignal
            // 
            this.groupBoxSensorSignal.Controls.Add(this.label6);
            this.groupBoxSensorSignal.Controls.Add(this.label5);
            this.groupBoxSensorSignal.Controls.Add(this.label4);
            this.groupBoxSensorSignal.Controls.Add(this.ckbPSMSignalOn);
            this.groupBoxSensorSignal.Controls.Add(this.ckbFireSignalOn);
            this.groupBoxSensorSignal.Location = new System.Drawing.Point(20, 205);
            this.groupBoxSensorSignal.Name = "groupBoxSensorSignal";
            this.groupBoxSensorSignal.Size = new System.Drawing.Size(451, 175);
            this.groupBoxSensorSignal.TabIndex = 9;
            this.groupBoxSensorSignal.TabStop = false;
            this.groupBoxSensorSignal.Text = "센서 신호 수신 설정";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(22, 134);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(401, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "이는 수신거부일뿐이며 신호의 발생여부와는 관련이 없습니다.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(22, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(383, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "해당 신호가 체크 되지 않는경우 신호가 수신되지 않습니다.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(22, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(369, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "센서 종류에 따라 신호 처리 여부를 결정하는 기능입니다.";
            // 
            // ckbPSMSignalOn
            // 
            this.ckbPSMSignalOn.AutoSize = true;
            this.ckbPSMSignalOn.Location = new System.Drawing.Point(248, 64);
            this.ckbPSMSignalOn.Name = "ckbPSMSignalOn";
            this.ckbPSMSignalOn.Size = new System.Drawing.Size(152, 16);
            this.ckbPSMSignalOn.TabIndex = 1;
            this.ckbPSMSignalOn.Text = "위험물질 누출신호 수신";
            this.ckbPSMSignalOn.UseVisualStyleBackColor = true;
            this.ckbPSMSignalOn.CheckedChanged += new System.EventHandler(this.ckbPSMSignalOn_CheckedChanged);
            // 
            // ckbFireSignalOn
            // 
            this.ckbFireSignalOn.AutoSize = true;
            this.ckbFireSignalOn.Location = new System.Drawing.Point(73, 64);
            this.ckbFireSignalOn.Name = "ckbFireSignalOn";
            this.ckbFireSignalOn.Size = new System.Drawing.Size(100, 16);
            this.ckbFireSignalOn.TabIndex = 0;
            this.ckbFireSignalOn.Text = "화재신호 수신";
            this.ckbFireSignalOn.UseVisualStyleBackColor = true;
            this.ckbFireSignalOn.CheckedChanged += new System.EventHandler(this.ckbFireSignalOn_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mCmbTimeMin);
            this.groupBox1.Controls.Add(this.mCmbDetectPolicy);
            this.groupBox1.Controls.Add(this.mCmbTimeDay);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.mCmbTimeHour);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(20, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(451, 168);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "화재센서 탐지";
            // 
            // mCmbTimeMin
            // 
            this.mCmbTimeMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCmbTimeMin.DropDownWidth = 212;
            this.mCmbTimeMin.Font = new System.Drawing.Font("굴림", 10F);
            this.mCmbTimeMin.FormattingEnabled = true;
            this.mCmbTimeMin.Items.AddRange(new object[] {
            "5분",
            "15분",
            "30분",
            "45분"});
            this.mCmbTimeMin.Location = new System.Drawing.Point(122, 125);
            this.mCmbTimeMin.Name = "mCmbTimeMin";
            this.mCmbTimeMin.Size = new System.Drawing.Size(212, 21);
            this.mCmbTimeMin.TabIndex = 5;
            this.mCmbTimeMin.SelectedIndexChanged += new System.EventHandler(this.CmbTimeMin_SelectedIndexChanged);
            // 
            // mCmbDetectPolicy
            // 
            this.mCmbDetectPolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCmbDetectPolicy.Font = new System.Drawing.Font("굴림", 10F);
            this.mCmbDetectPolicy.FormattingEnabled = true;
            this.mCmbDetectPolicy.Items.AddRange(new object[] {
            "모든 탐지 값을 표시",
            "몇 분 동안 표시하지 않습니다",
            "몇 시간 동안 표시하지 않습니다",
            "몇 일 동안 표시하지 않습니다",
            "완전히 표시하지 않습니다"});
            this.mCmbDetectPolicy.Location = new System.Drawing.Point(122, 87);
            this.mCmbDetectPolicy.Name = "mCmbDetectPolicy";
            this.mCmbDetectPolicy.Size = new System.Drawing.Size(212, 21);
            this.mCmbDetectPolicy.TabIndex = 2;
            this.mCmbDetectPolicy.SelectedIndexChanged += new System.EventHandler(this.CmbDetectPolicySelectedIndexChanged);
            // 
            // mCmbTimeDay
            // 
            this.mCmbTimeDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCmbTimeDay.DropDownWidth = 212;
            this.mCmbTimeDay.Font = new System.Drawing.Font("굴림", 10F);
            this.mCmbTimeDay.FormattingEnabled = true;
            this.mCmbTimeDay.Items.AddRange(new object[] {
            "1일",
            "2일",
            "3일",
            "5일",
            "7일",
            "10일",
            "15일",
            "30일"});
            this.mCmbTimeDay.Location = new System.Drawing.Point(222, 137);
            this.mCmbTimeDay.Name = "mCmbTimeDay";
            this.mCmbTimeDay.Size = new System.Drawing.Size(212, 21);
            this.mCmbTimeDay.TabIndex = 7;
            this.mCmbTimeDay.SelectedIndexChanged += new System.EventHandler(this.CmbTimeDay_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(119, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(209, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "오작동 처리된 센서의 탐지 값을";
            // 
            // mCmbTimeHour
            // 
            this.mCmbTimeHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCmbTimeHour.DropDownWidth = 212;
            this.mCmbTimeHour.Font = new System.Drawing.Font("굴림", 10F);
            this.mCmbTimeHour.FormattingEnabled = true;
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
            this.mCmbTimeHour.Location = new System.Drawing.Point(25, 137);
            this.mCmbTimeHour.Name = "mCmbTimeHour";
            this.mCmbTimeHour.Size = new System.Drawing.Size(212, 21);
            this.mCmbTimeHour.TabIndex = 6;
            this.mCmbTimeHour.SelectedIndexChanged += new System.EventHandler(this.CmbTimeHour_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(22, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(392, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "반복적으로 들어오는 오작동 값을 처리하기 위한 기능입니다.";
            // 
            // mBtnCancel
            // 
            this.mBtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mBtnCancel.Location = new System.Drawing.Point(279, 402);
            this.mBtnCancel.Name = "mBtnCancel";
            this.mBtnCancel.Size = new System.Drawing.Size(91, 28);
            this.mBtnCancel.TabIndex = 4;
            this.mBtnCancel.Text = "취소";
            this.mBtnCancel.UseVisualStyleBackColor = true;
            this.mBtnCancel.Click += new System.EventHandler(this.mBtnCancel_Click);
            // 
            // mBtnSave
            // 
            this.mBtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mBtnSave.Location = new System.Drawing.Point(380, 402);
            this.mBtnSave.Name = "mBtnSave";
            this.mBtnSave.Size = new System.Drawing.Size(91, 28);
            this.mBtnSave.TabIndex = 3;
            this.mBtnSave.Text = "수정완료";
            this.mBtnSave.UseVisualStyleBackColor = true;
            this.mBtnSave.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormSensorDetectPolicy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(516, 543);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSensorDetectPolicy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormSensorDetectPolicy";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBoxSensorSignal.ResumeLayout(false);
            this.groupBoxSensorSignal.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox mCmbDetectPolicy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox mCmbTimeMin;
        private System.Windows.Forms.Button mBtnCancel;
        private System.Windows.Forms.Button mBtnSave;
        private System.Windows.Forms.ComboBox mCmbTimeDay;
        private System.Windows.Forms.ComboBox mCmbTimeHour;
        private System.Windows.Forms.GroupBox groupBoxSensorSignal;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox ckbPSMSignalOn;
        private System.Windows.Forms.CheckBox ckbFireSignalOn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;

    }
}