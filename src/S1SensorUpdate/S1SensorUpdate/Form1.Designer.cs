namespace S1SensorUpdate
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_s1Name = new System.Windows.Forms.TextBox();
            this.textBox_s1Ip = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_unePw = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_uneUid = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_uneName = new System.Windows.Forms.TextBox();
            this.textBox_uneIp = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button_run = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "NAME";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_s1Name);
            this.groupBox1.Controls.Add(this.textBox_s1Ip);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 86);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "S1";
            // 
            // textBox_s1Name
            // 
            this.textBox_s1Name.Location = new System.Drawing.Point(58, 48);
            this.textBox_s1Name.Name = "textBox_s1Name";
            this.textBox_s1Name.Size = new System.Drawing.Size(132, 21);
            this.textBox_s1Name.TabIndex = 4;
            this.textBox_s1Name.Text = "S1";
            // 
            // textBox_s1Ip
            // 
            this.textBox_s1Ip.Location = new System.Drawing.Point(58, 24);
            this.textBox_s1Ip.Name = "textBox_s1Ip";
            this.textBox_s1Ip.Size = new System.Drawing.Size(132, 21);
            this.textBox_s1Ip.TabIndex = 3;
            this.textBox_s1Ip.Text = "192.168.0.195";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_unePw);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox_uneUid);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBox_uneName);
            this.groupBox2.Controls.Add(this.textBox_uneIp);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 124);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "UNE";
            // 
            // textBox_unePw
            // 
            this.textBox_unePw.Location = new System.Drawing.Point(58, 95);
            this.textBox_unePw.Name = "textBox_unePw";
            this.textBox_unePw.Size = new System.Drawing.Size(132, 21);
            this.textBox_unePw.TabIndex = 9;
            this.textBox_unePw.Text = "9966";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 12);
            this.label7.TabIndex = 8;
            this.label7.Text = "PW";
            // 
            // textBox_uneUid
            // 
            this.textBox_uneUid.Location = new System.Drawing.Point(58, 69);
            this.textBox_uneUid.Name = "textBox_uneUid";
            this.textBox_uneUid.Size = new System.Drawing.Size(132, 21);
            this.textBox_uneUid.TabIndex = 7;
            this.textBox_uneUid.Text = "root";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "UID";
            // 
            // textBox_uneName
            // 
            this.textBox_uneName.Location = new System.Drawing.Point(58, 45);
            this.textBox_uneName.Name = "textBox_uneName";
            this.textBox_uneName.Size = new System.Drawing.Size(132, 21);
            this.textBox_uneName.TabIndex = 5;
            this.textBox_uneName.Text = "EDU_100";
            // 
            // textBox_uneIp
            // 
            this.textBox_uneIp.Location = new System.Drawing.Point(58, 21);
            this.textBox_uneIp.Name = "textBox_uneIp";
            this.textBox_uneIp.Size = new System.Drawing.Size(132, 21);
            this.textBox_uneIp.TabIndex = 4;
            this.textBox_uneIp.Text = "127.0.0.1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "NAME";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "IP";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 12);
            this.label4.TabIndex = 2;
            // 
            // button_run
            // 
            this.button_run.Location = new System.Drawing.Point(139, 246);
            this.button_run.Name = "button_run";
            this.button_run.Size = new System.Drawing.Size(75, 23);
            this.button_run.TabIndex = 8;
            this.button_run.Text = "RUN";
            this.button_run.UseVisualStyleBackColor = true;
            this.button_run.Click += new System.EventHandler(this.button_run_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 281);
            this.Controls.Add(this.button_run);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_s1Name;
        private System.Windows.Forms.TextBox textBox_s1Ip;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_uneName;
        private System.Windows.Forms.TextBox textBox_uneIp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_run;
        private System.Windows.Forms.TextBox textBox_unePw;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_uneUid;
        private System.Windows.Forms.Label label6;
    }
}

