namespace SDMS.PopupDialog
{
    partial class FormConfirmSMS
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
            this.textBoxMsg = new System.Windows.Forms.TextBox();
            this.btnSendSMS = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRunBroadcast = new System.Windows.Forms.Button();
            this.btnRunBoth = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxMsg
            // 
            this.textBoxMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxMsg.Location = new System.Drawing.Point(12, 12);
            this.textBoxMsg.Multiline = true;
            this.textBoxMsg.Name = "textBoxMsg";
            this.textBoxMsg.ReadOnly = true;
            this.textBoxMsg.Size = new System.Drawing.Size(327, 103);
            this.textBoxMsg.TabIndex = 0;
            // 
            // btnSendSMS
            // 
            this.btnSendSMS.Location = new System.Drawing.Point(180, 121);
            this.btnSendSMS.Name = "btnSendSMS";
            this.btnSendSMS.Size = new System.Drawing.Size(75, 23);
            this.btnSendSMS.TabIndex = 1;
            this.btnSendSMS.Text = "문자 발송";
            this.btnSendSMS.UseVisualStyleBackColor = true;
            this.btnSendSMS.Click += new System.EventHandler(this.btnSendSMS_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(264, 121);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRunBroadcast
            // 
            this.btnRunBroadcast.Location = new System.Drawing.Point(96, 121);
            this.btnRunBroadcast.Name = "btnRunBroadcast";
            this.btnRunBroadcast.Size = new System.Drawing.Size(75, 23);
            this.btnRunBroadcast.TabIndex = 1;
            this.btnRunBroadcast.Text = "방송 실행";
            this.btnRunBroadcast.UseVisualStyleBackColor = true;
            this.btnRunBroadcast.Click += new System.EventHandler(this.btnRunBroadcast_Click);
            // 
            // btnRunBoth
            // 
            this.btnRunBoth.Location = new System.Drawing.Point(12, 121);
            this.btnRunBoth.Name = "btnRunBoth";
            this.btnRunBoth.Size = new System.Drawing.Size(75, 23);
            this.btnRunBoth.TabIndex = 1;
            this.btnRunBoth.Text = "모두 실행";
            this.btnRunBoth.UseVisualStyleBackColor = true;
            this.btnRunBoth.Click += new System.EventHandler(this.btnRunBoth_Click);
            // 
            // FormConfirmSMS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 150);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRunBoth);
            this.Controls.Add(this.btnRunBroadcast);
            this.Controls.Add(this.btnSendSMS);
            this.Controls.Add(this.textBoxMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormConfirmSMS";
            this.Text = "문자메시지 확인";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMsg;
        private System.Windows.Forms.Button btnSendSMS;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRunBroadcast;
        private System.Windows.Forms.Button btnRunBoth;
    }
}