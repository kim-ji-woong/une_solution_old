namespace OrclDBTest
{
    partial class FormSMS
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPhoneNumber1 = new System.Windows.Forms.TextBox();
            this.textBoxPhoneNumber2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSendPhoneNumber = new System.Windows.Forms.TextBox();
            this.labelMsgSize = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(296, 185);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(360, 12);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(95, 31);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "보내기";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "받는사람";
            // 
            // textBoxPhoneNumber1
            // 
            this.textBoxPhoneNumber1.Location = new System.Drawing.Point(17, 237);
            this.textBoxPhoneNumber1.Name = "textBoxPhoneNumber1";
            this.textBoxPhoneNumber1.Size = new System.Drawing.Size(124, 21);
            this.textBoxPhoneNumber1.TabIndex = 3;
            // 
            // textBoxPhoneNumber2
            // 
            this.textBoxPhoneNumber2.Location = new System.Drawing.Point(17, 264);
            this.textBoxPhoneNumber2.Name = "textBoxPhoneNumber2";
            this.textBoxPhoneNumber2.Size = new System.Drawing.Size(124, 21);
            this.textBoxPhoneNumber2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(180, 214);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "보내는사람";
            // 
            // textBoxSendPhoneNumber
            // 
            this.textBoxSendPhoneNumber.Location = new System.Drawing.Point(182, 237);
            this.textBoxSendPhoneNumber.Name = "textBoxSendPhoneNumber";
            this.textBoxSendPhoneNumber.Size = new System.Drawing.Size(124, 21);
            this.textBoxSendPhoneNumber.TabIndex = 3;
            // 
            // labelMsgSize
            // 
            this.labelMsgSize.AutoSize = true;
            this.labelMsgSize.Location = new System.Drawing.Point(358, 65);
            this.labelMsgSize.Name = "labelMsgSize";
            this.labelMsgSize.Size = new System.Drawing.Size(40, 12);
            this.labelMsgSize.TabIndex = 4;
            this.labelMsgSize.Text = "0 Byte";
            // 
            // FormSMS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 298);
            this.Controls.Add(this.labelMsgSize);
            this.Controls.Add(this.textBoxPhoneNumber2);
            this.Controls.Add(this.textBoxSendPhoneNumber);
            this.Controls.Add(this.textBoxPhoneNumber1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.textBox1);
            this.Name = "FormSMS";
            this.Text = "FormSMS";
            this.Load += new System.EventHandler(this.FormSMS_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPhoneNumber1;
        private System.Windows.Forms.TextBox textBoxPhoneNumber2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSendPhoneNumber;
        private System.Windows.Forms.Label labelMsgSize;
    }
}