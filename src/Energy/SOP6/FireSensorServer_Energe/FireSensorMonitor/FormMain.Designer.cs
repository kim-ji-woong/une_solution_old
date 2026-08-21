namespace SensorMonitor
{
    partial class FormMain
    {
#if !SERVICE
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
            this.cmbReciver = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCircuit = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbData = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.chkBox1 = new System.Windows.Forms.CheckBox();
            this.chkBox4 = new System.Windows.Forms.CheckBox();
            this.chkBox3 = new System.Windows.Forms.CheckBox();
            this.chkBox2 = new System.Windows.Forms.CheckBox();
            this.chkBox5 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBox26 = new System.Windows.Forms.CheckBox();
            this.chkBox25 = new System.Windows.Forms.CheckBox();
            this.chkBox22 = new System.Windows.Forms.CheckBox();
            this.chkBox23 = new System.Windows.Forms.CheckBox();
            this.chkBox24 = new System.Windows.Forms.CheckBox();
            this.chkBox21 = new System.Windows.Forms.CheckBox();
            this.chkBox20 = new System.Windows.Forms.CheckBox();
            this.chkBox17 = new System.Windows.Forms.CheckBox();
            this.chkBox18 = new System.Windows.Forms.CheckBox();
            this.chkBox19 = new System.Windows.Forms.CheckBox();
            this.chkBox16 = new System.Windows.Forms.CheckBox();
            this.chkBox15 = new System.Windows.Forms.CheckBox();
            this.chkBox12 = new System.Windows.Forms.CheckBox();
            this.chkBox13 = new System.Windows.Forms.CheckBox();
            this.chkBox14 = new System.Windows.Forms.CheckBox();
            this.chkBox11 = new System.Windows.Forms.CheckBox();
            this.chkBox10 = new System.Windows.Forms.CheckBox();
            this.chkBox7 = new System.Windows.Forms.CheckBox();
            this.chkBox8 = new System.Windows.Forms.CheckBox();
            this.chkBox9 = new System.Windows.Forms.CheckBox();
            this.chkBox6 = new System.Windows.Forms.CheckBox();
            this.chkBox27 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbReciver
            // 
            this.cmbReciver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReciver.FormattingEnabled = true;
            this.cmbReciver.Location = new System.Drawing.Point(12, 24);
            this.cmbReciver.Name = "cmbReciver";
            this.cmbReciver.Size = new System.Drawing.Size(312, 20);
            this.cmbReciver.TabIndex = 0;
            this.cmbReciver.SelectionChangeCommitted += new System.EventHandler(this.cmbReciver_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 365);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "상태";
            // 
            // cmbCircuit
            // 
            this.cmbCircuit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCircuit.FormattingEnabled = true;
            this.cmbCircuit.Location = new System.Drawing.Point(12, 62);
            this.cmbCircuit.Name = "cmbCircuit";
            this.cmbCircuit.Size = new System.Drawing.Size(312, 20);
            this.cmbCircuit.TabIndex = 2;
            this.cmbCircuit.SelectionChangeCommitted += new System.EventHandler(this.cmbCircuit_SelectionChangeCommitted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "데이터 값";
            // 
            // cmbData
            // 
            this.cmbData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbData.FormattingEnabled = true;
            this.cmbData.Items.AddRange(new object[] {
            "OFF",
            "ON",
            "연결됨",
            "연결끊김"});
            this.cmbData.Location = new System.Drawing.Point(12, 110);
            this.cmbData.Name = "cmbData";
            this.cmbData.Size = new System.Drawing.Size(194, 20);
            this.cmbData.TabIndex = 5;
            this.cmbData.SelectedIndexChanged += new System.EventHandler(this.cmbData_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "수신기";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "회로 번호";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(213, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 24);
            this.button1.TabIndex = 9;
            this.button1.Text = "전송";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 386);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(623, 39);
            this.textBox1.TabIndex = 10;
            // 
            // checkBox1
            // 
            this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 161);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(107, 22);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "수신기 통신 연결";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(125, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 17);
            this.label2.TabIndex = 12;
            this.label2.Text = "연결 안됨";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(330, 22);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(79, 23);
            this.button2.TabIndex = 13;
            this.button2.Text = "리셋";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // chkBox1
            // 
            this.chkBox1.AutoSize = true;
            this.chkBox1.Location = new System.Drawing.Point(13, 27);
            this.chkBox1.Name = "chkBox1";
            this.chkBox1.Size = new System.Drawing.Size(30, 16);
            this.chkBox1.TabIndex = 14;
            this.chkBox1.Text = "1";
            this.chkBox1.UseVisualStyleBackColor = true;
            // 
            // chkBox4
            // 
            this.chkBox4.AutoSize = true;
            this.chkBox4.Location = new System.Drawing.Point(13, 102);
            this.chkBox4.Name = "chkBox4";
            this.chkBox4.Size = new System.Drawing.Size(30, 16);
            this.chkBox4.TabIndex = 15;
            this.chkBox4.Text = "4";
            this.chkBox4.UseVisualStyleBackColor = true;
            // 
            // chkBox3
            // 
            this.chkBox3.AutoSize = true;
            this.chkBox3.Location = new System.Drawing.Point(13, 77);
            this.chkBox3.Name = "chkBox3";
            this.chkBox3.Size = new System.Drawing.Size(30, 16);
            this.chkBox3.TabIndex = 16;
            this.chkBox3.Text = "3";
            this.chkBox3.UseVisualStyleBackColor = true;
            // 
            // chkBox2
            // 
            this.chkBox2.AutoSize = true;
            this.chkBox2.Location = new System.Drawing.Point(13, 52);
            this.chkBox2.Name = "chkBox2";
            this.chkBox2.Size = new System.Drawing.Size(30, 16);
            this.chkBox2.TabIndex = 17;
            this.chkBox2.Text = "2";
            this.chkBox2.UseVisualStyleBackColor = true;
            // 
            // chkBox5
            // 
            this.chkBox5.AutoSize = true;
            this.chkBox5.Location = new System.Drawing.Point(13, 127);
            this.chkBox5.Name = "chkBox5";
            this.chkBox5.Size = new System.Drawing.Size(30, 16);
            this.chkBox5.TabIndex = 18;
            this.chkBox5.Text = "5";
            this.chkBox5.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkBox27);
            this.groupBox1.Controls.Add(this.chkBox26);
            this.groupBox1.Controls.Add(this.chkBox25);
            this.groupBox1.Controls.Add(this.chkBox22);
            this.groupBox1.Controls.Add(this.chkBox23);
            this.groupBox1.Controls.Add(this.chkBox24);
            this.groupBox1.Controls.Add(this.chkBox21);
            this.groupBox1.Controls.Add(this.chkBox20);
            this.groupBox1.Controls.Add(this.chkBox17);
            this.groupBox1.Controls.Add(this.chkBox18);
            this.groupBox1.Controls.Add(this.chkBox19);
            this.groupBox1.Controls.Add(this.chkBox16);
            this.groupBox1.Controls.Add(this.chkBox15);
            this.groupBox1.Controls.Add(this.chkBox12);
            this.groupBox1.Controls.Add(this.chkBox13);
            this.groupBox1.Controls.Add(this.chkBox14);
            this.groupBox1.Controls.Add(this.chkBox11);
            this.groupBox1.Controls.Add(this.chkBox10);
            this.groupBox1.Controls.Add(this.chkBox7);
            this.groupBox1.Controls.Add(this.chkBox8);
            this.groupBox1.Controls.Add(this.chkBox9);
            this.groupBox1.Controls.Add(this.chkBox6);
            this.groupBox1.Controls.Add(this.chkBox5);
            this.groupBox1.Controls.Add(this.chkBox2);
            this.groupBox1.Controls.Add(this.chkBox3);
            this.groupBox1.Controls.Add(this.chkBox4);
            this.groupBox1.Controls.Add(this.chkBox1);
            this.groupBox1.Location = new System.Drawing.Point(14, 189);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(626, 173);
            this.groupBox1.TabIndex = 39;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "연결상태";
            // 
            // chkBox26
            // 
            this.chkBox26.AutoSize = true;
            this.chkBox26.Location = new System.Drawing.Point(13, 151);
            this.chkBox26.Name = "chkBox26";
            this.chkBox26.Size = new System.Drawing.Size(30, 16);
            this.chkBox26.TabIndex = 39;
            this.chkBox26.Text = "5";
            this.chkBox26.UseVisualStyleBackColor = true;
            // 
            // chkBox25
            // 
            this.chkBox25.AutoSize = true;
            this.chkBox25.Location = new System.Drawing.Point(533, 127);
            this.chkBox25.Name = "chkBox25";
            this.chkBox25.Size = new System.Drawing.Size(30, 16);
            this.chkBox25.TabIndex = 38;
            this.chkBox25.Text = "5";
            this.chkBox25.UseVisualStyleBackColor = true;
            // 
            // chkBox22
            // 
            this.chkBox22.AutoSize = true;
            this.chkBox22.Location = new System.Drawing.Point(533, 52);
            this.chkBox22.Name = "chkBox22";
            this.chkBox22.Size = new System.Drawing.Size(30, 16);
            this.chkBox22.TabIndex = 37;
            this.chkBox22.Text = "2";
            this.chkBox22.UseVisualStyleBackColor = true;
            // 
            // chkBox23
            // 
            this.chkBox23.AutoSize = true;
            this.chkBox23.Location = new System.Drawing.Point(533, 77);
            this.chkBox23.Name = "chkBox23";
            this.chkBox23.Size = new System.Drawing.Size(30, 16);
            this.chkBox23.TabIndex = 36;
            this.chkBox23.Text = "3";
            this.chkBox23.UseVisualStyleBackColor = true;
            // 
            // chkBox24
            // 
            this.chkBox24.AutoSize = true;
            this.chkBox24.Location = new System.Drawing.Point(533, 102);
            this.chkBox24.Name = "chkBox24";
            this.chkBox24.Size = new System.Drawing.Size(30, 16);
            this.chkBox24.TabIndex = 35;
            this.chkBox24.Text = "3";
            this.chkBox24.UseVisualStyleBackColor = true;
            // 
            // chkBox21
            // 
            this.chkBox21.AutoSize = true;
            this.chkBox21.Location = new System.Drawing.Point(533, 27);
            this.chkBox21.Name = "chkBox21";
            this.chkBox21.Size = new System.Drawing.Size(30, 16);
            this.chkBox21.TabIndex = 34;
            this.chkBox21.Text = "1";
            this.chkBox21.UseVisualStyleBackColor = true;
            // 
            // chkBox20
            // 
            this.chkBox20.AutoSize = true;
            this.chkBox20.Location = new System.Drawing.Point(403, 126);
            this.chkBox20.Name = "chkBox20";
            this.chkBox20.Size = new System.Drawing.Size(30, 16);
            this.chkBox20.TabIndex = 33;
            this.chkBox20.Text = "5";
            this.chkBox20.UseVisualStyleBackColor = true;
            // 
            // chkBox17
            // 
            this.chkBox17.AutoSize = true;
            this.chkBox17.Location = new System.Drawing.Point(403, 52);
            this.chkBox17.Name = "chkBox17";
            this.chkBox17.Size = new System.Drawing.Size(30, 16);
            this.chkBox17.TabIndex = 32;
            this.chkBox17.Text = "2";
            this.chkBox17.UseVisualStyleBackColor = true;
            // 
            // chkBox18
            // 
            this.chkBox18.AutoSize = true;
            this.chkBox18.Location = new System.Drawing.Point(403, 77);
            this.chkBox18.Name = "chkBox18";
            this.chkBox18.Size = new System.Drawing.Size(30, 16);
            this.chkBox18.TabIndex = 31;
            this.chkBox18.Text = "3";
            this.chkBox18.UseVisualStyleBackColor = true;
            // 
            // chkBox19
            // 
            this.chkBox19.AutoSize = true;
            this.chkBox19.Location = new System.Drawing.Point(403, 101);
            this.chkBox19.Name = "chkBox19";
            this.chkBox19.Size = new System.Drawing.Size(30, 16);
            this.chkBox19.TabIndex = 30;
            this.chkBox19.Text = "3";
            this.chkBox19.UseVisualStyleBackColor = true;
            // 
            // chkBox16
            // 
            this.chkBox16.AutoSize = true;
            this.chkBox16.Location = new System.Drawing.Point(403, 27);
            this.chkBox16.Name = "chkBox16";
            this.chkBox16.Size = new System.Drawing.Size(30, 16);
            this.chkBox16.TabIndex = 29;
            this.chkBox16.Text = "1";
            this.chkBox16.UseVisualStyleBackColor = true;
            // 
            // chkBox15
            // 
            this.chkBox15.AutoSize = true;
            this.chkBox15.Location = new System.Drawing.Point(273, 127);
            this.chkBox15.Name = "chkBox15";
            this.chkBox15.Size = new System.Drawing.Size(30, 16);
            this.chkBox15.TabIndex = 28;
            this.chkBox15.Text = "5";
            this.chkBox15.UseVisualStyleBackColor = true;
            // 
            // chkBox12
            // 
            this.chkBox12.AutoSize = true;
            this.chkBox12.Location = new System.Drawing.Point(273, 52);
            this.chkBox12.Name = "chkBox12";
            this.chkBox12.Size = new System.Drawing.Size(30, 16);
            this.chkBox12.TabIndex = 27;
            this.chkBox12.Text = "2";
            this.chkBox12.UseVisualStyleBackColor = true;
            // 
            // chkBox13
            // 
            this.chkBox13.AutoSize = true;
            this.chkBox13.Location = new System.Drawing.Point(273, 77);
            this.chkBox13.Name = "chkBox13";
            this.chkBox13.Size = new System.Drawing.Size(30, 16);
            this.chkBox13.TabIndex = 26;
            this.chkBox13.Text = "3";
            this.chkBox13.UseVisualStyleBackColor = true;
            // 
            // chkBox14
            // 
            this.chkBox14.AutoSize = true;
            this.chkBox14.Location = new System.Drawing.Point(273, 102);
            this.chkBox14.Name = "chkBox14";
            this.chkBox14.Size = new System.Drawing.Size(30, 16);
            this.chkBox14.TabIndex = 25;
            this.chkBox14.Text = "3";
            this.chkBox14.UseVisualStyleBackColor = true;
            // 
            // chkBox11
            // 
            this.chkBox11.AutoSize = true;
            this.chkBox11.Location = new System.Drawing.Point(273, 27);
            this.chkBox11.Name = "chkBox11";
            this.chkBox11.Size = new System.Drawing.Size(30, 16);
            this.chkBox11.TabIndex = 24;
            this.chkBox11.Text = "1";
            this.chkBox11.UseVisualStyleBackColor = true;
            // 
            // chkBox10
            // 
            this.chkBox10.AutoSize = true;
            this.chkBox10.Location = new System.Drawing.Point(143, 127);
            this.chkBox10.Name = "chkBox10";
            this.chkBox10.Size = new System.Drawing.Size(30, 16);
            this.chkBox10.TabIndex = 23;
            this.chkBox10.Text = "5";
            this.chkBox10.UseVisualStyleBackColor = true;
            // 
            // chkBox7
            // 
            this.chkBox7.AutoSize = true;
            this.chkBox7.Location = new System.Drawing.Point(143, 52);
            this.chkBox7.Name = "chkBox7";
            this.chkBox7.Size = new System.Drawing.Size(30, 16);
            this.chkBox7.TabIndex = 22;
            this.chkBox7.Text = "2";
            this.chkBox7.UseVisualStyleBackColor = true;
            // 
            // chkBox8
            // 
            this.chkBox8.AutoSize = true;
            this.chkBox8.Location = new System.Drawing.Point(143, 77);
            this.chkBox8.Name = "chkBox8";
            this.chkBox8.Size = new System.Drawing.Size(30, 16);
            this.chkBox8.TabIndex = 21;
            this.chkBox8.Text = "3";
            this.chkBox8.UseVisualStyleBackColor = true;
            // 
            // chkBox9
            // 
            this.chkBox9.AutoSize = true;
            this.chkBox9.Location = new System.Drawing.Point(143, 102);
            this.chkBox9.Name = "chkBox9";
            this.chkBox9.Size = new System.Drawing.Size(30, 16);
            this.chkBox9.TabIndex = 20;
            this.chkBox9.Text = "3";
            this.chkBox9.UseVisualStyleBackColor = true;
            // 
            // chkBox6
            // 
            this.chkBox6.AutoSize = true;
            this.chkBox6.Location = new System.Drawing.Point(143, 27);
            this.chkBox6.Name = "chkBox6";
            this.chkBox6.Size = new System.Drawing.Size(30, 16);
            this.chkBox6.TabIndex = 19;
            this.chkBox6.Text = "1";
            this.chkBox6.UseVisualStyleBackColor = true;
            // 
            // chkBox27
            // 
            this.chkBox27.AutoSize = true;
            this.chkBox27.Location = new System.Drawing.Point(143, 151);
            this.chkBox27.Name = "chkBox27";
            this.chkBox27.Size = new System.Drawing.Size(30, 16);
            this.chkBox27.TabIndex = 40;
            this.chkBox27.Text = "5";
            this.chkBox27.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 434);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbData);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbCircuit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbReciver);
            this.Name = "FormMain";
            this.Text = "수신기 모니터";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbReciver;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCircuit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbData;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.CheckBox chkBox1;
		private System.Windows.Forms.CheckBox chkBox4;
		private System.Windows.Forms.CheckBox chkBox3;
		private System.Windows.Forms.CheckBox chkBox2;
		private System.Windows.Forms.CheckBox chkBox5;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkBox10;
		private System.Windows.Forms.CheckBox chkBox7;
		private System.Windows.Forms.CheckBox chkBox8;
		private System.Windows.Forms.CheckBox chkBox9;
		private System.Windows.Forms.CheckBox chkBox6;
		private System.Windows.Forms.CheckBox chkBox25;
		private System.Windows.Forms.CheckBox chkBox22;
		private System.Windows.Forms.CheckBox chkBox23;
		private System.Windows.Forms.CheckBox chkBox24;
		private System.Windows.Forms.CheckBox chkBox21;
		private System.Windows.Forms.CheckBox chkBox20;
		private System.Windows.Forms.CheckBox chkBox17;
		private System.Windows.Forms.CheckBox chkBox18;
		private System.Windows.Forms.CheckBox chkBox19;
		private System.Windows.Forms.CheckBox chkBox16;
		private System.Windows.Forms.CheckBox chkBox15;
		private System.Windows.Forms.CheckBox chkBox12;
		private System.Windows.Forms.CheckBox chkBox13;
		private System.Windows.Forms.CheckBox chkBox14;
		private System.Windows.Forms.CheckBox chkBox11;
        private System.Windows.Forms.CheckBox chkBox26;
        private System.Windows.Forms.CheckBox chkBox27;


#endif

    }
}

