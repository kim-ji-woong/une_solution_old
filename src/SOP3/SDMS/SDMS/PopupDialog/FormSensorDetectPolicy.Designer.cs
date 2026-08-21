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
			this.mCmbTimeDay = new System.Windows.Forms.ComboBox();
			this.mCmbTimeHour = new System.Windows.Forms.ComboBox();
			this.mCmbTimeMin = new System.Windows.Forms.ComboBox();
			this.mBtnCancel = new System.Windows.Forms.Button();
			this.mBtnSave = new System.Windows.Forms.Button();
			this.mCmbDetectPolicy = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
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
			this.panel2.Controls.Add(this.mCmbTimeDay);
			this.panel2.Controls.Add(this.mCmbTimeHour);
			this.panel2.Controls.Add(this.mCmbTimeMin);
			this.panel2.Controls.Add(this.mBtnCancel);
			this.panel2.Controls.Add(this.mBtnSave);
			this.panel2.Controls.Add(this.mCmbDetectPolicy);
			this.panel2.Controls.Add(this.label3);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Location = new System.Drawing.Point(12, 72);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(492, 276);
			this.panel2.TabIndex = 2;
			// 
			// mCmbTimeDay
			// 
			this.mCmbTimeDay.DropDownWidth = 212;
			this.mCmbTimeDay.Font = new System.Drawing.Font("굴림", 10F);
			this.mCmbTimeDay.FormattingEnabled = true;
			this.mCmbTimeDay.Items.AddRange(new object[] {
            "1일",
            "2일",
            "3일",
            "7일"});
			this.mCmbTimeDay.Location = new System.Drawing.Point(20, 231);
			this.mCmbTimeDay.Name = "mCmbTimeDay";
			this.mCmbTimeDay.Size = new System.Drawing.Size(212, 21);
			this.mCmbTimeDay.TabIndex = 7;
			this.mCmbTimeDay.SelectedIndexChanged += new System.EventHandler(this.CmbTimeDay_SelectedIndexChanged);
			// 
			// mCmbTimeHour
			// 
			this.mCmbTimeHour.DropDownWidth = 212;
			this.mCmbTimeHour.Font = new System.Drawing.Font("굴림", 10F);
			this.mCmbTimeHour.FormattingEnabled = true;
			this.mCmbTimeHour.Items.AddRange(new object[] {
            "1시간",
            "3시간",
            "5시간",
            "8시간"});
			this.mCmbTimeHour.Location = new System.Drawing.Point(20, 198);
			this.mCmbTimeHour.Name = "mCmbTimeHour";
			this.mCmbTimeHour.Size = new System.Drawing.Size(212, 21);
			this.mCmbTimeHour.TabIndex = 6;
			this.mCmbTimeHour.SelectedIndexChanged += new System.EventHandler(this.CmbTimeHour_SelectedIndexChanged);
			// 
			// mCmbTimeMin
			// 
			this.mCmbTimeMin.DropDownWidth = 212;
			this.mCmbTimeMin.Font = new System.Drawing.Font("굴림", 10F);
			this.mCmbTimeMin.FormattingEnabled = true;
			this.mCmbTimeMin.Items.AddRange(new object[] {
            "5분",
            "15분",
            "30분",
            "45분"});
			this.mCmbTimeMin.Location = new System.Drawing.Point(143, 150);
			this.mCmbTimeMin.Name = "mCmbTimeMin";
			this.mCmbTimeMin.Size = new System.Drawing.Size(212, 21);
			this.mCmbTimeMin.TabIndex = 5;
			this.mCmbTimeMin.SelectedIndexChanged += new System.EventHandler(this.CmbTimeMin_SelectedIndexChanged);
			// 
			// mBtnCancel
			// 
			this.mBtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mBtnCancel.Location = new System.Drawing.Point(279, 231);
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
			this.mBtnSave.Location = new System.Drawing.Point(380, 231);
			this.mBtnSave.Name = "mBtnSave";
			this.mBtnSave.Size = new System.Drawing.Size(91, 28);
			this.mBtnSave.TabIndex = 3;
			this.mBtnSave.Text = "수정완료";
			this.mBtnSave.UseVisualStyleBackColor = true;
			this.mBtnSave.Click += new System.EventHandler(this.button1_Click);
			// 
			// mCmbDetectPolicy
			// 
			this.mCmbDetectPolicy.Font = new System.Drawing.Font("굴림", 10F);
			this.mCmbDetectPolicy.FormattingEnabled = true;
			this.mCmbDetectPolicy.Items.AddRange(new object[] {
            "모든 탐지 값을 표시",
            "몇 분 동안 표시하지 않습니다",
            "몇 시간 동안 표시하지 않습니다",
            "몇 일 동안 표시하지 않습니다",
            "완전히 표시하지 않습니다"});
			this.mCmbDetectPolicy.Location = new System.Drawing.Point(143, 112);
			this.mCmbDetectPolicy.Name = "mCmbDetectPolicy";
			this.mCmbDetectPolicy.Size = new System.Drawing.Size(212, 21);
			this.mCmbDetectPolicy.TabIndex = 2;
			this.mCmbDetectPolicy.SelectedIndexChanged += new System.EventHandler(this.CmbDetectPolicySelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label3.Location = new System.Drawing.Point(140, 85);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(209, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "오작동 처리된 센서의 탐지 값을";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label2.Location = new System.Drawing.Point(42, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(392, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "반복적으로 들어오는 오작동 값을 처리하기 위한 기능입니다.";
			// 
			// FormSensorDetectPolicy
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.LightGray;
			this.ClientSize = new System.Drawing.Size(516, 360);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormSensorDetectPolicy";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "FormSensorDetectPolicy";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
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

    }
}