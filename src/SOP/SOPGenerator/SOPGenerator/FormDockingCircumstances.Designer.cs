namespace SOPGen
{
    partial class FormDockingCircumstances
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelMessageSize = new System.Windows.Forms.Label();
            this.btnStandard = new System.Windows.Forms.Button();
            this.textMessage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textCellPhone3 = new System.Windows.Forms.TextBox();
            this.textCellPhone2 = new System.Windows.Forms.TextBox();
            this.textCellPhone1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textFAXFile = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textFAX3 = new System.Windows.Forms.TextBox();
            this.textFAX2 = new System.Windows.Forms.TextBox();
            this.textFAX1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBroadcast = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelMessageSize);
            this.groupBox1.Controls.Add(this.btnStandard);
            this.groupBox1.Controls.Add(this.textMessage);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textCellPhone3);
            this.groupBox1.Controls.Add(this.textCellPhone2);
            this.groupBox1.Controls.Add(this.textCellPhone1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(317, 139);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SMS";
            // 
            // labelMessageSize
            // 
            this.labelMessageSize.AutoSize = true;
            this.labelMessageSize.Location = new System.Drawing.Point(174, 115);
            this.labelMessageSize.Name = "labelMessageSize";
            this.labelMessageSize.Size = new System.Drawing.Size(122, 12);
            this.labelMessageSize.TabIndex = 9;
            this.labelMessageSize.Text = "메시지 크기 : (0Byte)";
            // 
            // btnStandard
            // 
            this.btnStandard.Location = new System.Drawing.Point(230, 19);
            this.btnStandard.Name = "btnStandard";
            this.btnStandard.Size = new System.Drawing.Size(75, 23);
            this.btnStandard.TabIndex = 6;
            this.btnStandard.Text = "표준문구";
            this.btnStandard.UseVisualStyleBackColor = true;
            this.btnStandard.Click += new System.EventHandler(this.btnStandard_Click);
            // 
            // textMessage
            // 
            this.textMessage.Location = new System.Drawing.Point(60, 46);
            this.textMessage.Multiline = true;
            this.textMessage.Name = "textMessage";
            this.textMessage.Size = new System.Drawing.Size(245, 62);
            this.textMessage.TabIndex = 8;
            this.textMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textMessage_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "휴대폰 :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "메세지 :";
            // 
            // textCellPhone3
            // 
            this.textCellPhone3.Location = new System.Drawing.Point(179, 20);
            this.textCellPhone3.MaxLength = 4;
            this.textCellPhone3.Name = "textCellPhone3";
            this.textCellPhone3.Size = new System.Drawing.Size(45, 21);
            this.textCellPhone3.TabIndex = 5;
            this.textCellPhone3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textCellPhone3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textCellPhone3.Leave += new System.EventHandler(this.textCellPhone3_Leave);
            // 
            // textCellPhone2
            // 
            this.textCellPhone2.Location = new System.Drawing.Point(117, 20);
            this.textCellPhone2.MaxLength = 4;
            this.textCellPhone2.Name = "textCellPhone2";
            this.textCellPhone2.Size = new System.Drawing.Size(45, 21);
            this.textCellPhone2.TabIndex = 3;
            this.textCellPhone2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textCellPhone2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textCellPhone2.Leave += new System.EventHandler(this.textCellPhone2_Leave);
            // 
            // textCellPhone1
            // 
            this.textCellPhone1.Location = new System.Drawing.Point(60, 20);
            this.textCellPhone1.MaxLength = 3;
            this.textCellPhone1.Name = "textCellPhone1";
            this.textCellPhone1.Size = new System.Drawing.Size(40, 21);
            this.textCellPhone1.TabIndex = 1;
            this.textCellPhone1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textCellPhone1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textCellPhone1.Leave += new System.EventHandler(this.textCellPhone1_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(165, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(103, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "-";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textFAXFile);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textFAX3);
            this.groupBox2.Controls.Add(this.textFAX2);
            this.groupBox2.Controls.Add(this.textFAX1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(3, 148);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(317, 79);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // textFAXFile
            // 
            this.textFAXFile.Enabled = false;
            this.textFAXFile.Location = new System.Drawing.Point(60, 47);
            this.textFAXFile.Name = "textFAXFile";
            this.textFAXFile.Size = new System.Drawing.Size(164, 21);
            this.textFAXFile.TabIndex = 6;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(230, 45);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "파일등록";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "FAX :";
            // 
            // textFAX3
            // 
            this.textFAX3.Enabled = false;
            this.textFAX3.Location = new System.Drawing.Point(179, 20);
            this.textFAX3.MaxLength = 4;
            this.textFAX3.Name = "textFAX3";
            this.textFAX3.Size = new System.Drawing.Size(45, 21);
            this.textFAX3.TabIndex = 5;
            this.textFAX3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textFAX3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textFAX3.Leave += new System.EventHandler(this.textFAX3_Leave);
            // 
            // textFAX2
            // 
            this.textFAX2.Enabled = false;
            this.textFAX2.Location = new System.Drawing.Point(117, 20);
            this.textFAX2.MaxLength = 4;
            this.textFAX2.Name = "textFAX2";
            this.textFAX2.Size = new System.Drawing.Size(45, 21);
            this.textFAX2.TabIndex = 3;
            this.textFAX2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textFAX2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textFAX2.Leave += new System.EventHandler(this.textFAX2_Leave);
            // 
            // textFAX1
            // 
            this.textFAX1.Enabled = false;
            this.textFAX1.Location = new System.Drawing.Point(60, 20);
            this.textFAX1.MaxLength = 3;
            this.textFAX1.Name = "textFAX1";
            this.textFAX1.Size = new System.Drawing.Size(40, 21);
            this.textFAX1.TabIndex = 1;
            this.textFAX1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textFAX1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textFAX1.Leave += new System.EventHandler(this.textFAX1_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(165, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "-";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Location = new System.Drawing.Point(103, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "-";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.textBroadcast);
            this.groupBox3.Location = new System.Drawing.Point(3, 233);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(317, 64);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "방송 :";
            // 
            // textBroadcast
            // 
            this.textBroadcast.Enabled = false;
            this.textBroadcast.Location = new System.Drawing.Point(60, 20);
            this.textBroadcast.Multiline = true;
            this.textBroadcast.Name = "textBroadcast";
            this.textBroadcast.Size = new System.Drawing.Size(245, 31);
            this.textBroadcast.TabIndex = 1;
            // 
            // FormDockingCircumstances
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 306);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDockingCircumstances";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormGroup";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStandard;
        private System.Windows.Forms.TextBox textMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textCellPhone3;
        private System.Windows.Forms.TextBox textCellPhone2;
        private System.Windows.Forms.TextBox textCellPhone1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textFAXFile;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textFAX3;
        private System.Windows.Forms.TextBox textFAX2;
        private System.Windows.Forms.TextBox textFAX1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBroadcast;
        private System.Windows.Forms.Label labelMessageSize;

    }
}