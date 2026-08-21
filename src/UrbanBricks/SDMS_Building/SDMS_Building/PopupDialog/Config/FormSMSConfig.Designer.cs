namespace SDMS_Building.PopupDialog.Config
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
            this.eleType = new System.Windows.Forms.Integration.ElementHost();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.txtTraning = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMalfunction = new UnE.GUI.RibbonButton();
            this.btnReport = new UnE.GUI.RibbonButton();
            this.btnDetect = new UnE.GUI.RibbonButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnTrainingMode = new UnE.GUI.RibbonButton();
            this.panel2.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // eleType
            // 
            this.eleType.Location = new System.Drawing.Point(239, 35);
            this.eleType.Name = "eleType";
            this.eleType.Size = new System.Drawing.Size(386, 50);
            this.eleType.TabIndex = 22;
            this.eleType.Text = "elementHost1";
            this.eleType.Child = null;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label3.Location = new System.Drawing.Point(279, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(231, 19);
            this.label3.TabIndex = 33;
            this.label3.Text = "탐지시 담당자에게 문자메시지 발송";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label2.Location = new System.Drawing.Point(279, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(231, 19);
            this.label2.TabIndex = 35;
            this.label2.Text = "신고시 담당자에게 문자메시지 발송";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label4.Location = new System.Drawing.Point(279, 188);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(346, 19);
            this.label4.TabIndex = 37;
            this.label4.Text = "오작동 신고, 신호 복구시 담당자에게 문자메시지 발송";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label6.Location = new System.Drawing.Point(221, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 19);
            this.label6.TabIndex = 40;
            this.label6.Text = "훈련모드 활성화";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BackgroundImage = global::SDMS_Building.Properties.Resources.pnBox;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Controls.Add(this.btnTrainingMode);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.panel7);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Location = new System.Drawing.Point(20, 320);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(920, 143);
            this.panel2.TabIndex = 42;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label9.Location = new System.Drawing.Point(177, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(129, 19);
            this.label9.TabIndex = 44;
            this.label9.Text = "메시지 앞머리 문구";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.txtTraning);
            this.panel7.Location = new System.Drawing.Point(323, 71);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(443, 38);
            this.panel7.TabIndex = 43;
            // 
            // txtTraning
            // 
            this.txtTraning.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTraning.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtTraning.Location = new System.Drawing.Point(6, 7);
            this.txtTraning.Name = "txtTraning";
            this.txtTraning.Size = new System.Drawing.Size(429, 23);
            this.txtTraning.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("나눔바른고딕", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(18, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 54);
            this.label7.TabIndex = 25;
            this.label7.Text = "훈련/실제\r\n모드";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::SDMS_Building.Properties.Resources.pnBox;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.btnMalfunction);
            this.panel1.Controls.Add(this.btnReport);
            this.panel1.Controls.Add(this.btnDetect);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.eleType);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(20, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(920, 247);
            this.panel1.TabIndex = 43;
            // 
            // btnMalfunction
            // 
            this.btnMalfunction.CheckButton = false;
            this.btnMalfunction.CheckedBkgndImage = null;
            this.btnMalfunction.CheckedImage = global::SDMS_Building.Properties.Resources.check_Checked;
            this.btnMalfunction.CheckedMouseOver = global::SDMS_Building.Properties.Resources.check_Checked;
            this.btnMalfunction.ClickedBackgroundImage = null;
            this.btnMalfunction.ClickedImage = global::SDMS_Building.Properties.Resources.check_Hover;
            this.btnMalfunction.CustomImageRect = new System.Drawing.Rectangle(0, 0, 30, 30);
            this.btnMalfunction.DisabledBkgndImage = null;
            this.btnMalfunction.DisabledImage = null;
            this.btnMalfunction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnMalfunction.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnMalfunction.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnMalfunction.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnMalfunction.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnMalfunction.ForeColorsByTypeUse = true;
            this.btnMalfunction.ID = -1;
            this.btnMalfunction.InitButtonWidth = 30;
            this.btnMalfunction.IsChecked = false;
            this.btnMalfunction.Location = new System.Drawing.Point(239, 183);
            this.btnMalfunction.MouseOverBkgndImage = null;
            this.btnMalfunction.MouseOverImage = global::SDMS_Building.Properties.Resources.check_UnChecked;
            this.btnMalfunction.Name = "btnMalfunction";
            this.btnMalfunction.NormalImage = global::SDMS_Building.Properties.Resources.check_UnChecked;
            this.btnMalfunction.Owner = null;
            this.btnMalfunction.Size = new System.Drawing.Size(30, 30);
            this.btnMalfunction.TabIndex = 43;
            this.btnMalfunction.TextLocation = new System.Drawing.Point(0, 13);
            this.btnMalfunction.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnMalfunction.ToolTipText = "";
            this.btnMalfunction.UseCustomImageRect = true;
            this.btnMalfunction.UseTextLocation = true;
            this.btnMalfunction.UseVisualStyleBackColor = true;
            this.btnMalfunction.Click += new System.EventHandler(this.btnMalfunction_Click);
            // 
            // btnReport
            // 
            this.btnReport.CheckButton = false;
            this.btnReport.CheckedBkgndImage = null;
            this.btnReport.CheckedImage = global::SDMS_Building.Properties.Resources.check_Checked;
            this.btnReport.CheckedMouseOver = global::SDMS_Building.Properties.Resources.check_Checked;
            this.btnReport.ClickedBackgroundImage = null;
            this.btnReport.ClickedImage = global::SDMS_Building.Properties.Resources.check_Hover;
            this.btnReport.CustomImageRect = new System.Drawing.Rectangle(0, 0, 30, 30);
            this.btnReport.DisabledBkgndImage = null;
            this.btnReport.DisabledImage = null;
            this.btnReport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnReport.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnReport.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnReport.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnReport.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnReport.ForeColorsByTypeUse = true;
            this.btnReport.ID = -1;
            this.btnReport.InitButtonWidth = 30;
            this.btnReport.IsChecked = false;
            this.btnReport.Location = new System.Drawing.Point(239, 147);
            this.btnReport.MouseOverBkgndImage = null;
            this.btnReport.MouseOverImage = global::SDMS_Building.Properties.Resources.check_UnChecked;
            this.btnReport.Name = "btnReport";
            this.btnReport.NormalImage = global::SDMS_Building.Properties.Resources.check_UnChecked;
            this.btnReport.Owner = null;
            this.btnReport.Size = new System.Drawing.Size(30, 30);
            this.btnReport.TabIndex = 42;
            this.btnReport.TextLocation = new System.Drawing.Point(0, 13);
            this.btnReport.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnReport.ToolTipText = "";
            this.btnReport.UseCustomImageRect = true;
            this.btnReport.UseTextLocation = true;
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // btnDetect
            // 
            this.btnDetect.CheckButton = false;
            this.btnDetect.CheckedBkgndImage = null;
            this.btnDetect.CheckedImage = global::SDMS_Building.Properties.Resources.check_Checked;
            this.btnDetect.CheckedMouseOver = global::SDMS_Building.Properties.Resources.check_Checked;
            this.btnDetect.ClickedBackgroundImage = null;
            this.btnDetect.ClickedImage = global::SDMS_Building.Properties.Resources.check_Hover;
            this.btnDetect.CustomImageRect = new System.Drawing.Rectangle(0, 0, 30, 30);
            this.btnDetect.DisabledBkgndImage = null;
            this.btnDetect.DisabledImage = null;
            this.btnDetect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnDetect.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnDetect.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnDetect.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnDetect.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnDetect.ForeColorsByTypeUse = true;
            this.btnDetect.ID = -1;
            this.btnDetect.InitButtonWidth = 30;
            this.btnDetect.IsChecked = false;
            this.btnDetect.Location = new System.Drawing.Point(239, 111);
            this.btnDetect.MouseOverBkgndImage = null;
            this.btnDetect.MouseOverImage = global::SDMS_Building.Properties.Resources.check_UnChecked;
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.NormalImage = global::SDMS_Building.Properties.Resources.check_UnChecked;
            this.btnDetect.Owner = null;
            this.btnDetect.Size = new System.Drawing.Size(30, 30);
            this.btnDetect.TabIndex = 41;
            this.btnDetect.TextLocation = new System.Drawing.Point(0, 13);
            this.btnDetect.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDetect.ToolTipText = "";
            this.btnDetect.UseCustomImageRect = true;
            this.btnDetect.UseTextLocation = true;
            this.btnDetect.UseVisualStyleBackColor = true;
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label8.Location = new System.Drawing.Point(177, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 23);
            this.label8.TabIndex = 40;
            this.label8.Text = "유형";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(169, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 27);
            this.label1.TabIndex = 39;
            this.label1.Text = "신호";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("나눔바른고딕", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(37, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 27);
            this.label5.TabIndex = 25;
            this.label5.Text = "신  호";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnTrainingMode
            // 
            this.btnTrainingMode.CheckButton = false;
            this.btnTrainingMode.CheckedBkgndImage = null;
            this.btnTrainingMode.CheckedImage = global::SDMS_Building.Properties.Resources.check_Checked;
            this.btnTrainingMode.CheckedMouseOver = global::SDMS_Building.Properties.Resources.check_Checked;
            this.btnTrainingMode.ClickedBackgroundImage = null;
            this.btnTrainingMode.ClickedImage = global::SDMS_Building.Properties.Resources.check_Hover;
            this.btnTrainingMode.CustomImageRect = new System.Drawing.Rectangle(0, 0, 30, 30);
            this.btnTrainingMode.DisabledBkgndImage = null;
            this.btnTrainingMode.DisabledImage = null;
            this.btnTrainingMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnTrainingMode.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnTrainingMode.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnTrainingMode.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnTrainingMode.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnTrainingMode.ForeColorsByTypeUse = true;
            this.btnTrainingMode.ID = -1;
            this.btnTrainingMode.InitButtonWidth = 30;
            this.btnTrainingMode.IsChecked = false;
            this.btnTrainingMode.Location = new System.Drawing.Point(181, 34);
            this.btnTrainingMode.MouseOverBkgndImage = null;
            this.btnTrainingMode.MouseOverImage = global::SDMS_Building.Properties.Resources.check_UnChecked;
            this.btnTrainingMode.Name = "btnTrainingMode";
            this.btnTrainingMode.NormalImage = global::SDMS_Building.Properties.Resources.check_UnChecked;
            this.btnTrainingMode.Owner = null;
            this.btnTrainingMode.Size = new System.Drawing.Size(30, 30);
            this.btnTrainingMode.TabIndex = 44;
            this.btnTrainingMode.TextLocation = new System.Drawing.Point(0, 13);
            this.btnTrainingMode.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnTrainingMode.ToolTipText = "";
            this.btnTrainingMode.UseCustomImageRect = true;
            this.btnTrainingMode.UseTextLocation = true;
            this.btnTrainingMode.UseVisualStyleBackColor = true;
            this.btnTrainingMode.Click += new System.EventHandler(this.btnTrainingMode_Click);
            // 
            // FormSMSConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(960, 500);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSMSConfig";
            this.Text = "FormSMSConfig";
            this.Load += new System.EventHandler(this.FormSMSConfig_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost eleType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.TextBox txtTraning;
        private UnE.GUI.RibbonButton btnDetect;
        private UnE.GUI.RibbonButton btnMalfunction;
        private UnE.GUI.RibbonButton btnReport;
        private UnE.GUI.RibbonButton btnTrainingMode;
    }
}