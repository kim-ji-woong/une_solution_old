namespace SVMSEventReciver
{
#if !SERVICE
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbSOPServer = new System.Windows.Forms.Label();
            this.lbSVMS = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.ckbFire = new System.Windows.Forms.CheckBox();
            this.ckbIntr = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "접속";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbSOPServer);
            this.groupBox1.Controls.Add(this.lbSVMS);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(14, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 112);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "접속정보";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // lbSOPServer
            // 
            this.lbSOPServer.AutoSize = true;
            this.lbSOPServer.Location = new System.Drawing.Point(138, 73);
            this.lbSOPServer.Name = "lbSOPServer";
            this.lbSOPServer.Size = new System.Drawing.Size(41, 12);
            this.lbSOPServer.TabIndex = 24;
            this.lbSOPServer.Text = "접속중";
            // 
            // lbSVMS
            // 
            this.lbSVMS.AutoSize = true;
            this.lbSVMS.Location = new System.Drawing.Point(138, 39);
            this.lbSVMS.Name = "lbSVMS";
            this.lbSVMS.Size = new System.Drawing.Size(41, 12);
            this.lbSVMS.TabIndex = 23;
            this.lbSVMS.Text = "접속중";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 12);
            this.label6.TabIndex = 20;
            this.label6.Text = "SVMS 연결 상태";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 12);
            this.label5.TabIndex = 19;
            this.label5.Text = "SOP서버 연결 상태";
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(270, 104);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 20;
            this.button2.Text = "신호전송";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(270, 77);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(75, 21);
            this.textBox1.TabIndex = 21;
            this.textBox1.Text = "20001";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(270, 131);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 22;
            this.button3.Text = "신호리셋";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ckbFire
            // 
            this.ckbFire.AutoSize = true;
            this.ckbFire.Checked = true;
            this.ckbFire.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbFire.Location = new System.Drawing.Point(154, 19);
            this.ckbFire.Name = "ckbFire";
            this.ckbFire.Size = new System.Drawing.Size(72, 16);
            this.ckbFire.TabIndex = 23;
            this.ckbFire.Text = "화재신호";
            this.ckbFire.UseVisualStyleBackColor = true;
            this.ckbFire.CheckedChanged += new System.EventHandler(this.ckbFire_CheckedChanged);
            // 
            // ckbIntr
            // 
            this.ckbIntr.AutoSize = true;
            this.ckbIntr.Checked = true;
            this.ckbIntr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbIntr.Location = new System.Drawing.Point(246, 19);
            this.ckbIntr.Name = "ckbIntr";
            this.ckbIntr.Size = new System.Drawing.Size(72, 16);
            this.ckbIntr.TabIndex = 24;
            this.ckbIntr.Text = "칩입펜스";
            this.ckbIntr.UseVisualStyleBackColor = true;
            this.ckbIntr.CheckedChanged += new System.EventHandler(this.ckbIntr_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 181);
            this.Controls.Add(this.ckbIntr);
            this.Controls.Add(this.ckbFire);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "SVMS 이벤트 수신자";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbSOPServer;
        private System.Windows.Forms.Label lbSVMS;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox ckbFire;
        private System.Windows.Forms.CheckBox ckbIntr;
        //private AxRTSPLiveScreenLib.AxRTSPLiveScreen axRTSPLiveScreen1;
        //private AxRTSPLiveScreenLib.AxRTSPLiveScreen axRTSPLiveScreen1;
    }

#endif
}