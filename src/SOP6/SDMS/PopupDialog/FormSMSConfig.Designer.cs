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
            this.btnOK = new UnE.GUI.ImageButton();
            this.checkBoxSendSMSToDuty = new System.Windows.Forms.CheckBox();
            this.checkBoxReportFire = new System.Windows.Forms.CheckBox();
            this.checkBoxFacilityFault = new System.Windows.Forms.CheckBox();
            this.checkBoxDetectFire = new System.Windows.Forms.CheckBox();
            this.ckbRunSimulator = new System.Windows.Forms.CheckBox();
            this.txt_msgHeader = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxActivateTrainingMode = new System.Windows.Forms.CheckBox();
            this.ckbReportReset = new System.Windows.Forms.CheckBox();
            this.ckbReportSpill = new System.Windows.Forms.CheckBox();
            this.ckbDetectSpill = new System.Windows.Forms.CheckBox();
            this.checkBoxResetSecurity = new System.Windows.Forms.CheckBox();
            this.checkBoxReportSecurity = new System.Windows.Forms.CheckBox();
            this.checkBoxDetectSecurity = new System.Windows.Forms.CheckBox();
            this.labelFire = new System.Windows.Forms.Label();
            this.labelMode = new System.Windows.Forms.Label();
            this.labelPsm = new System.Windows.Forms.Label();
            this.labelSecurity = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            this.SuspendLayout();
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
            this.btnOK.Location = new System.Drawing.Point(430, 501);
            this.btnOK.Name = "btnOK";
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(52, 29);
            this.btnOK.TabIndex = 17;
            this.btnOK.TabStop = false;
            this.btnOK.TextColor = System.Drawing.Color.Black;
            this.btnOK.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ToolTipText = "";
            this.btnOK.UseToolTip = false;
            this.btnOK.WindowRateWidth = 1F;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // checkBoxSendSMSToDuty
            // 
            this.checkBoxSendSMSToDuty.AutoSize = true;
            this.checkBoxSendSMSToDuty.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxSendSMSToDuty.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxSendSMSToDuty.ForeColor = System.Drawing.Color.White;
            this.checkBoxSendSMSToDuty.Location = new System.Drawing.Point(51, 507);
            this.checkBoxSendSMSToDuty.Name = "checkBoxSendSMSToDuty";
            this.checkBoxSendSMSToDuty.Size = new System.Drawing.Size(261, 22);
            this.checkBoxSendSMSToDuty.TabIndex = 22;
            this.checkBoxSendSMSToDuty.Text = "당직자에게 문자 메시지 전송";
            this.checkBoxSendSMSToDuty.UseVisualStyleBackColor = false;
            this.checkBoxSendSMSToDuty.Visible = false;
            // 
            // checkBoxReportFire
            // 
            this.checkBoxReportFire.AutoSize = true;
            this.checkBoxReportFire.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxReportFire.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxReportFire.ForeColor = System.Drawing.Color.White;
            this.checkBoxReportFire.Location = new System.Drawing.Point(51, 148);
            this.checkBoxReportFire.Name = "checkBoxReportFire";
            this.checkBoxReportFire.Size = new System.Drawing.Size(331, 21);
            this.checkBoxReportFire.TabIndex = 19;
            this.checkBoxReportFire.Text = "화재신고시 담당자에게 문자메시지 발송";
            this.checkBoxReportFire.UseVisualStyleBackColor = false;
            // 
            // checkBoxFacilityFault
            // 
            this.checkBoxFacilityFault.AutoSize = true;
            this.checkBoxFacilityFault.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxFacilityFault.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxFacilityFault.ForeColor = System.Drawing.Color.White;
            this.checkBoxFacilityFault.Location = new System.Drawing.Point(51, 178);
            this.checkBoxFacilityFault.Name = "checkBoxFacilityFault";
            this.checkBoxFacilityFault.Size = new System.Drawing.Size(436, 21);
            this.checkBoxFacilityFault.TabIndex = 20;
            this.checkBoxFacilityFault.Text = "오작동 신고, 신호 복구시 담당자에게 문자메시지 발송";
            this.checkBoxFacilityFault.UseVisualStyleBackColor = false;
            // 
            // checkBoxDetectFire
            // 
            this.checkBoxDetectFire.AutoSize = true;
            this.checkBoxDetectFire.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDetectFire.Checked = true;
            this.checkBoxDetectFire.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDetectFire.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxDetectFire.ForeColor = System.Drawing.Color.White;
            this.checkBoxDetectFire.Location = new System.Drawing.Point(51, 118);
            this.checkBoxDetectFire.Name = "checkBoxDetectFire";
            this.checkBoxDetectFire.Size = new System.Drawing.Size(331, 21);
            this.checkBoxDetectFire.TabIndex = 21;
            this.checkBoxDetectFire.Text = "화재탐지시 담당자에게 문자메시지 발송";
            this.checkBoxDetectFire.UseVisualStyleBackColor = false;
            // 
            // ckbRunSimulator
            // 
            this.ckbRunSimulator.AutoSize = true;
            this.ckbRunSimulator.BackColor = System.Drawing.Color.Transparent;
            this.ckbRunSimulator.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ckbRunSimulator.ForeColor = System.Drawing.Color.White;
            this.ckbRunSimulator.Location = new System.Drawing.Point(50, 486);
            this.ckbRunSimulator.Name = "ckbRunSimulator";
            this.ckbRunSimulator.Size = new System.Drawing.Size(290, 22);
            this.ckbRunSimulator.TabIndex = 26;
            this.ckbRunSimulator.Text = "화재신고시 SOP시뮬레이터 기동";
            this.ckbRunSimulator.UseVisualStyleBackColor = false;
            this.ckbRunSimulator.Visible = false;
            // 
            // txt_msgHeader
            // 
            this.txt_msgHeader.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_msgHeader.Location = new System.Drawing.Point(224, 287);
            this.txt_msgHeader.Name = "txt_msgHeader";
            this.txt_msgHeader.Size = new System.Drawing.Size(240, 27);
            this.txt_msgHeader.TabIndex = 25;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(65, 290);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 17);
            this.label1.TabIndex = 24;
            this.label1.Text = "메시지 앞머리 문구 : ";
            // 
            // checkBoxActivateTrainingMode
            // 
            this.checkBoxActivateTrainingMode.AutoSize = true;
            this.checkBoxActivateTrainingMode.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxActivateTrainingMode.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxActivateTrainingMode.ForeColor = System.Drawing.Color.White;
            this.checkBoxActivateTrainingMode.ImageIndex = 0;
            this.checkBoxActivateTrainingMode.Location = new System.Drawing.Point(50, 263);
            this.checkBoxActivateTrainingMode.Name = "checkBoxActivateTrainingMode";
            this.checkBoxActivateTrainingMode.Size = new System.Drawing.Size(151, 21);
            this.checkBoxActivateTrainingMode.TabIndex = 23;
            this.checkBoxActivateTrainingMode.Text = "훈련모드 활성화";
            this.checkBoxActivateTrainingMode.UseVisualStyleBackColor = false;
            this.checkBoxActivateTrainingMode.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ckbReportReset
            // 
            this.ckbReportReset.AutoSize = true;
            this.ckbReportReset.BackColor = System.Drawing.Color.Transparent;
            this.ckbReportReset.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ckbReportReset.ForeColor = System.Drawing.Color.White;
            this.ckbReportReset.Location = new System.Drawing.Point(50, 432);
            this.ckbReportReset.Name = "ckbReportReset";
            this.ckbReportReset.Size = new System.Drawing.Size(336, 21);
            this.ckbReportReset.TabIndex = 29;
            this.ckbReportReset.Text = "신호복구시 담당자에게 문자 메시지 발송";
            this.ckbReportReset.UseVisualStyleBackColor = false;
            // 
            // ckbReportSpill
            // 
            this.ckbReportSpill.AutoSize = true;
            this.ckbReportSpill.BackColor = System.Drawing.Color.Transparent;
            this.ckbReportSpill.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ckbReportSpill.ForeColor = System.Drawing.Color.White;
            this.ckbReportSpill.Location = new System.Drawing.Point(50, 402);
            this.ckbReportSpill.Name = "ckbReportSpill";
            this.ckbReportSpill.Size = new System.Drawing.Size(336, 21);
            this.ckbReportSpill.TabIndex = 28;
            this.ckbReportSpill.Text = "누출전파시 담당자에게 문자 메시지 발송";
            this.ckbReportSpill.UseVisualStyleBackColor = false;
            // 
            // ckbDetectSpill
            // 
            this.ckbDetectSpill.AutoSize = true;
            this.ckbDetectSpill.BackColor = System.Drawing.Color.Transparent;
            this.ckbDetectSpill.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ckbDetectSpill.ForeColor = System.Drawing.Color.White;
            this.ckbDetectSpill.Location = new System.Drawing.Point(50, 372);
            this.ckbDetectSpill.Name = "ckbDetectSpill";
            this.ckbDetectSpill.Size = new System.Drawing.Size(336, 21);
            this.ckbDetectSpill.TabIndex = 27;
            this.ckbDetectSpill.Text = "누출탐지시 담당자에게 문자 메시지 발송";
            this.ckbDetectSpill.UseVisualStyleBackColor = false;
            // 
            // checkBoxResetSecurity
            // 
            this.checkBoxResetSecurity.AutoSize = true;
            this.checkBoxResetSecurity.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxResetSecurity.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxResetSecurity.ForeColor = System.Drawing.Color.White;
            this.checkBoxResetSecurity.Location = new System.Drawing.Point(405, 432);
            this.checkBoxResetSecurity.Name = "checkBoxResetSecurity";
            this.checkBoxResetSecurity.Size = new System.Drawing.Size(336, 21);
            this.checkBoxResetSecurity.TabIndex = 32;
            this.checkBoxResetSecurity.Text = "신호복구시 담당자에게 문자 메시지 발송";
            this.checkBoxResetSecurity.UseVisualStyleBackColor = false;
            this.checkBoxResetSecurity.Visible = false;
            // 
            // checkBoxReportSecurity
            // 
            this.checkBoxReportSecurity.AutoSize = true;
            this.checkBoxReportSecurity.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxReportSecurity.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxReportSecurity.ForeColor = System.Drawing.Color.White;
            this.checkBoxReportSecurity.Location = new System.Drawing.Point(405, 402);
            this.checkBoxReportSecurity.Name = "checkBoxReportSecurity";
            this.checkBoxReportSecurity.Size = new System.Drawing.Size(336, 21);
            this.checkBoxReportSecurity.TabIndex = 31;
            this.checkBoxReportSecurity.Text = "방범신고시 담당자에게 문자 메시지 발송";
            this.checkBoxReportSecurity.UseVisualStyleBackColor = false;
            this.checkBoxReportSecurity.Visible = false;
            // 
            // checkBoxDetectSecurity
            // 
            this.checkBoxDetectSecurity.AutoSize = true;
            this.checkBoxDetectSecurity.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDetectSecurity.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxDetectSecurity.ForeColor = System.Drawing.Color.White;
            this.checkBoxDetectSecurity.Location = new System.Drawing.Point(405, 372);
            this.checkBoxDetectSecurity.Name = "checkBoxDetectSecurity";
            this.checkBoxDetectSecurity.Size = new System.Drawing.Size(336, 21);
            this.checkBoxDetectSecurity.TabIndex = 30;
            this.checkBoxDetectSecurity.Text = "방범탐지시 담당자에게 문자 메시지 발송";
            this.checkBoxDetectSecurity.UseVisualStyleBackColor = false;
            this.checkBoxDetectSecurity.Visible = false;
            // 
            // labelFire
            // 
            this.labelFire.AutoSize = true;
            this.labelFire.BackColor = System.Drawing.Color.Transparent;
            this.labelFire.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFire.ForeColor = System.Drawing.Color.White;
            this.labelFire.Location = new System.Drawing.Point(28, 90);
            this.labelFire.Name = "labelFire";
            this.labelFire.Size = new System.Drawing.Size(101, 20);
            this.labelFire.TabIndex = 33;
            this.labelFire.Text = "화재 신호";
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.BackColor = System.Drawing.Color.Transparent;
            this.labelMode.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMode.ForeColor = System.Drawing.Color.White;
            this.labelMode.Location = new System.Drawing.Point(27, 236);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(145, 20);
            this.labelMode.TabIndex = 34;
            this.labelMode.Text = "훈련/실제모드";
            // 
            // labelPsm
            // 
            this.labelPsm.AutoSize = true;
            this.labelPsm.BackColor = System.Drawing.Color.Transparent;
            this.labelPsm.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelPsm.ForeColor = System.Drawing.Color.White;
            this.labelPsm.Location = new System.Drawing.Point(27, 340);
            this.labelPsm.Name = "labelPsm";
            this.labelPsm.Size = new System.Drawing.Size(143, 20);
            this.labelPsm.TabIndex = 35;
            this.labelPsm.Text = "위험물질 신호";
            // 
            // labelSecurity
            // 
            this.labelSecurity.AutoSize = true;
            this.labelSecurity.BackColor = System.Drawing.Color.Transparent;
            this.labelSecurity.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSecurity.ForeColor = System.Drawing.Color.White;
            this.labelSecurity.Location = new System.Drawing.Point(377, 340);
            this.labelSecurity.Name = "labelSecurity";
            this.labelSecurity.Size = new System.Drawing.Size(101, 20);
            this.labelSecurity.TabIndex = 36;
            this.labelSecurity.Text = "방범 신호";
            this.labelSecurity.Visible = false;
            // 
            // FormSMSConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BackgroundImage = global::SDMS.Properties.Resources.SMSConfig_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(500, 542);
            this.Controls.Add(this.labelSecurity);
            this.Controls.Add(this.labelPsm);
            this.Controls.Add(this.labelMode);
            this.Controls.Add(this.labelFire);
            this.Controls.Add(this.checkBoxResetSecurity);
            this.Controls.Add(this.checkBoxReportSecurity);
            this.Controls.Add(this.checkBoxDetectSecurity);
            this.Controls.Add(this.ckbReportReset);
            this.Controls.Add(this.ckbReportSpill);
            this.Controls.Add(this.ckbDetectSpill);
            this.Controls.Add(this.ckbRunSimulator);
            this.Controls.Add(this.txt_msgHeader);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxActivateTrainingMode);
            this.Controls.Add(this.checkBoxSendSMSToDuty);
            this.Controls.Add(this.checkBoxReportFire);
            this.Controls.Add(this.checkBoxFacilityFault);
            this.Controls.Add(this.checkBoxDetectFire);
            this.Controls.Add(this.btnOK);
            this.Name = "FormSMSConfig";
            this.ShowInTaskbar = false;
            this.Text = "FormSMSConfig";
            this.Load += new System.EventHandler(this.FormSMSConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UnE.GUI.ImageButton btnOK;
        private System.Windows.Forms.CheckBox checkBoxSendSMSToDuty;
        private System.Windows.Forms.CheckBox checkBoxReportFire;
        private System.Windows.Forms.CheckBox checkBoxFacilityFault;
        private System.Windows.Forms.CheckBox checkBoxDetectFire;
        private System.Windows.Forms.CheckBox ckbRunSimulator;
        private System.Windows.Forms.TextBox txt_msgHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxActivateTrainingMode;
        private System.Windows.Forms.CheckBox ckbReportReset;
        private System.Windows.Forms.CheckBox ckbReportSpill;
        private System.Windows.Forms.CheckBox ckbDetectSpill;
        private System.Windows.Forms.CheckBox checkBoxResetSecurity;
        private System.Windows.Forms.CheckBox checkBoxReportSecurity;
        private System.Windows.Forms.CheckBox checkBoxDetectSecurity;
        private System.Windows.Forms.Label labelFire;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.Label labelPsm;
        private System.Windows.Forms.Label labelSecurity;
    }
}