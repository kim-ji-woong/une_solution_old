namespace SDMS
{
    partial class FormSMSConfig
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBoxSecurity = new System.Windows.Forms.GroupBox();
            this.checkBoxResetSecurity = new System.Windows.Forms.CheckBox();
            this.checkBoxReportSecurity = new System.Windows.Forms.CheckBox();
            this.checkBoxDetectSecurity = new System.Windows.Forms.CheckBox();
            this.groupBoxPSM = new System.Windows.Forms.GroupBox();
            this.ckbReportReset = new System.Windows.Forms.CheckBox();
            this.ckbReportSpill = new System.Windows.Forms.CheckBox();
            this.ckbDetectSpill = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ckbRunSimulator = new System.Windows.Forms.CheckBox();
            this.txt_msgHeader = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxActivateTrainingMode = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxSendSMSToDuty = new System.Windows.Forms.CheckBox();
            this.checkBoxReportFire = new System.Windows.Forms.CheckBox();
            this.checkBoxFacilityFault = new System.Windows.Forms.CheckBox();
            this.checkBoxDetectFire = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBoxSecurity.SuspendLayout();
            this.groupBoxPSM.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(221, 379);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(302, 379);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(10, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(395, 47);
            this.panel1.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(20, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "메시지 관리";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.groupBoxSecurity);
            this.panel2.Controls.Add(this.groupBoxPSM);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Location = new System.Drawing.Point(10, 70);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(395, 420);
            this.panel2.TabIndex = 3;
            // 
            // groupBoxSecurity
            // 
            this.groupBoxSecurity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSecurity.Controls.Add(this.checkBoxResetSecurity);
            this.groupBoxSecurity.Controls.Add(this.checkBoxReportSecurity);
            this.groupBoxSecurity.Controls.Add(this.checkBoxDetectSecurity);
            this.groupBoxSecurity.Location = new System.Drawing.Point(339, 246);
            this.groupBoxSecurity.Name = "groupBoxSecurity";
            this.groupBoxSecurity.Size = new System.Drawing.Size(352, 106);
            this.groupBoxSecurity.TabIndex = 10;
            this.groupBoxSecurity.TabStop = false;
            this.groupBoxSecurity.Text = "방범 신호";
            this.groupBoxSecurity.Visible = false;
            // 
            // checkBoxResetSecurity
            // 
            this.checkBoxResetSecurity.AutoSize = true;
            this.checkBoxResetSecurity.Location = new System.Drawing.Point(17, 77);
            this.checkBoxResetSecurity.Name = "checkBoxResetSecurity";
            this.checkBoxResetSecurity.Size = new System.Drawing.Size(244, 16);
            this.checkBoxResetSecurity.TabIndex = 2;
            this.checkBoxResetSecurity.Text = "신호복구시 담당자에게 문자 메시지 발송";
            this.checkBoxResetSecurity.UseVisualStyleBackColor = true;
            this.checkBoxResetSecurity.CheckedChanged += new System.EventHandler(this.ckbReportReset_CheckedChanged);
            // 
            // checkBoxReportSecurity
            // 
            this.checkBoxReportSecurity.AutoSize = true;
            this.checkBoxReportSecurity.Location = new System.Drawing.Point(17, 53);
            this.checkBoxReportSecurity.Name = "checkBoxReportSecurity";
            this.checkBoxReportSecurity.Size = new System.Drawing.Size(244, 16);
            this.checkBoxReportSecurity.TabIndex = 1;
            this.checkBoxReportSecurity.Text = "방범신고시 담당자에게 문자 메시지 발송";
            this.checkBoxReportSecurity.UseVisualStyleBackColor = true;
            this.checkBoxReportSecurity.CheckedChanged += new System.EventHandler(this.ckbReportSpill_CheckedChanged);
            // 
            // checkBoxDetectSecurity
            // 
            this.checkBoxDetectSecurity.AutoSize = true;
            this.checkBoxDetectSecurity.Location = new System.Drawing.Point(17, 29);
            this.checkBoxDetectSecurity.Name = "checkBoxDetectSecurity";
            this.checkBoxDetectSecurity.Size = new System.Drawing.Size(244, 16);
            this.checkBoxDetectSecurity.TabIndex = 0;
            this.checkBoxDetectSecurity.Text = "방범탐지시 담당자에게 문자 메시지 발송";
            this.checkBoxDetectSecurity.UseVisualStyleBackColor = true;
            this.checkBoxDetectSecurity.CheckedChanged += new System.EventHandler(this.ckbDetectSpill_CheckedChanged);
            // 
            // groupBoxPSM
            // 
            this.groupBoxPSM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPSM.Controls.Add(this.ckbReportReset);
            this.groupBoxPSM.Controls.Add(this.ckbReportSpill);
            this.groupBoxPSM.Controls.Add(this.ckbDetectSpill);
            this.groupBoxPSM.Location = new System.Drawing.Point(23, 246);
            this.groupBoxPSM.Name = "groupBoxPSM";
            this.groupBoxPSM.Size = new System.Drawing.Size(352, 106);
            this.groupBoxPSM.TabIndex = 10;
            this.groupBoxPSM.TabStop = false;
            this.groupBoxPSM.Text = "위험물질 신호";
            // 
            // ckbReportReset
            // 
            this.ckbReportReset.AutoSize = true;
            this.ckbReportReset.Location = new System.Drawing.Point(17, 77);
            this.ckbReportReset.Name = "ckbReportReset";
            this.ckbReportReset.Size = new System.Drawing.Size(244, 16);
            this.ckbReportReset.TabIndex = 2;
            this.ckbReportReset.Text = "신호복구시 담당자에게 문자 메시지 발송";
            this.ckbReportReset.UseVisualStyleBackColor = true;
            this.ckbReportReset.CheckedChanged += new System.EventHandler(this.ckbReportReset_CheckedChanged);
            // 
            // ckbReportSpill
            // 
            this.ckbReportSpill.AutoSize = true;
            this.ckbReportSpill.Location = new System.Drawing.Point(17, 53);
            this.ckbReportSpill.Name = "ckbReportSpill";
            this.ckbReportSpill.Size = new System.Drawing.Size(244, 16);
            this.ckbReportSpill.TabIndex = 1;
            this.ckbReportSpill.Text = "누출전파시 담당자에게 문자 메시지 발송";
            this.ckbReportSpill.UseVisualStyleBackColor = true;
            this.ckbReportSpill.CheckedChanged += new System.EventHandler(this.ckbReportSpill_CheckedChanged);
            // 
            // ckbDetectSpill
            // 
            this.ckbDetectSpill.AutoSize = true;
            this.ckbDetectSpill.Location = new System.Drawing.Point(17, 29);
            this.ckbDetectSpill.Name = "ckbDetectSpill";
            this.ckbDetectSpill.Size = new System.Drawing.Size(244, 16);
            this.ckbDetectSpill.TabIndex = 0;
            this.ckbDetectSpill.Text = "누출탐지시 담당자에게 문자 메시지 발송";
            this.ckbDetectSpill.UseVisualStyleBackColor = true;
            this.ckbDetectSpill.CheckedChanged += new System.EventHandler(this.ckbDetectSpill_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.ckbRunSimulator);
            this.groupBox2.Controls.Add(this.txt_msgHeader);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.checkBoxActivateTrainingMode);
            this.groupBox2.Location = new System.Drawing.Point(23, 154);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(353, 83);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "훈련 / 실제모드";
            // 
            // ckbRunSimulator
            // 
            this.ckbRunSimulator.AutoSize = true;
            this.ckbRunSimulator.Location = new System.Drawing.Point(255, 48);
            this.ckbRunSimulator.Name = "ckbRunSimulator";
            this.ckbRunSimulator.Size = new System.Drawing.Size(201, 16);
            this.ckbRunSimulator.TabIndex = 7;
            this.ckbRunSimulator.Text = "화재신고시 SOP시뮬레이터 기동";
            this.ckbRunSimulator.UseVisualStyleBackColor = true;
            this.ckbRunSimulator.Visible = false;
            this.ckbRunSimulator.CheckedChanged += new System.EventHandler(this.ckbRunSimulator_CheckedChanged);
            // 
            // txt_msgHeader
            // 
            this.txt_msgHeader.Location = new System.Drawing.Point(157, 47);
            this.txt_msgHeader.Name = "txt_msgHeader";
            this.txt_msgHeader.Size = new System.Drawing.Size(83, 21);
            this.txt_msgHeader.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "메시지 앞머리 문구 : ";
            // 
            // checkBoxActivateTrainingMode
            // 
            this.checkBoxActivateTrainingMode.AutoSize = true;
            this.checkBoxActivateTrainingMode.Location = new System.Drawing.Point(17, 28);
            this.checkBoxActivateTrainingMode.Name = "checkBoxActivateTrainingMode";
            this.checkBoxActivateTrainingMode.Size = new System.Drawing.Size(112, 16);
            this.checkBoxActivateTrainingMode.TabIndex = 4;
            this.checkBoxActivateTrainingMode.Text = "훈련모드 활성화";
            this.checkBoxActivateTrainingMode.UseVisualStyleBackColor = true;
            this.checkBoxActivateTrainingMode.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBoxSendSMSToDuty);
            this.groupBox1.Controls.Add(this.checkBoxReportFire);
            this.groupBox1.Controls.Add(this.checkBoxFacilityFault);
            this.groupBox1.Controls.Add(this.checkBoxDetectFire);
            this.groupBox1.Location = new System.Drawing.Point(23, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(353, 126);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "화재신호";
            // 
            // checkBoxSendSMSToDuty
            // 
            this.checkBoxSendSMSToDuty.AutoSize = true;
            this.checkBoxSendSMSToDuty.Location = new System.Drawing.Point(17, 101);
            this.checkBoxSendSMSToDuty.Name = "checkBoxSendSMSToDuty";
            this.checkBoxSendSMSToDuty.Size = new System.Drawing.Size(180, 16);
            this.checkBoxSendSMSToDuty.TabIndex = 11;
            this.checkBoxSendSMSToDuty.Text = "당직자에게 문자 메시지 전송";
            this.checkBoxSendSMSToDuty.UseVisualStyleBackColor = true;
            this.checkBoxSendSMSToDuty.Visible = false;
            // 
            // checkBoxReportFire
            // 
            this.checkBoxReportFire.AutoSize = true;
            this.checkBoxReportFire.Location = new System.Drawing.Point(17, 53);
            this.checkBoxReportFire.Name = "checkBoxReportFire";
            this.checkBoxReportFire.Size = new System.Drawing.Size(240, 16);
            this.checkBoxReportFire.TabIndex = 7;
            this.checkBoxReportFire.Text = "화재신고시 담당자에게 문자메시지 발송";
            this.checkBoxReportFire.UseVisualStyleBackColor = true;
            // 
            // checkBoxFacilityFault
            // 
            this.checkBoxFacilityFault.AutoSize = true;
            this.checkBoxFacilityFault.Location = new System.Drawing.Point(17, 77);
            this.checkBoxFacilityFault.Name = "checkBoxFacilityFault";
            this.checkBoxFacilityFault.Size = new System.Drawing.Size(316, 16);
            this.checkBoxFacilityFault.TabIndex = 8;
            this.checkBoxFacilityFault.Text = "오작동 신고, 신호 복구시 담당자에게 문자메시지 발송";
            this.checkBoxFacilityFault.UseVisualStyleBackColor = true;
            // 
            // checkBoxDetectFire
            // 
            this.checkBoxDetectFire.AutoSize = true;
            this.checkBoxDetectFire.Checked = true;
            this.checkBoxDetectFire.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDetectFire.Location = new System.Drawing.Point(17, 29);
            this.checkBoxDetectFire.Name = "checkBoxDetectFire";
            this.checkBoxDetectFire.Size = new System.Drawing.Size(240, 16);
            this.checkBoxDetectFire.TabIndex = 9;
            this.checkBoxDetectFire.Text = "화재탐지시 담당자에게 문자메시지 발송";
            this.checkBoxDetectFire.UseVisualStyleBackColor = true;
            // 
            // FormSMSConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(422, 502);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSMSConfig";
            this.ShowInTaskbar = false;
            this.Text = "FormSMSConfig";
            this.Load += new System.EventHandler(this.FormSMSConfig_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBoxSecurity.ResumeLayout(false);
            this.groupBoxSecurity.PerformLayout();
            this.groupBoxPSM.ResumeLayout(false);
            this.groupBoxPSM.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox checkBoxActivateTrainingMode;
        private System.Windows.Forms.CheckBox ckbRunSimulator;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxSendSMSToDuty;
        private System.Windows.Forms.CheckBox checkBoxReportFire;
        private System.Windows.Forms.CheckBox checkBoxFacilityFault;
        private System.Windows.Forms.CheckBox checkBoxDetectFire;
        private System.Windows.Forms.GroupBox groupBoxPSM;
        private System.Windows.Forms.CheckBox ckbDetectSpill;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox ckbReportReset;
        private System.Windows.Forms.CheckBox ckbReportSpill;
        private System.Windows.Forms.GroupBox groupBoxSecurity;
        private System.Windows.Forms.CheckBox checkBoxResetSecurity;
        private System.Windows.Forms.CheckBox checkBoxReportSecurity;
        private System.Windows.Forms.CheckBox checkBoxDetectSecurity;
        private System.Windows.Forms.TextBox txt_msgHeader;
        private System.Windows.Forms.Label label1;
    }
}