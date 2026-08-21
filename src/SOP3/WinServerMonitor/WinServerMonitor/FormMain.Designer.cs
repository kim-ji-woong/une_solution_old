namespace ServerMonitor
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.stateMonitor = new System.Windows.Forms.Label();
            this.stateTTS = new System.Windows.Forms.Label();
            this.stateSOP = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLogFolder = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.stateBackup = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnStopSOP = new System.Windows.Forms.Button();
            this.btnStartSOP = new System.Windows.Forms.Button();
            this.btnStopTTS = new System.Windows.Forms.Button();
            this.btnStartTTS = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.m_CheckTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnUpdateImmediately = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stateMonitor
            // 
            this.stateMonitor.BackColor = System.Drawing.Color.Red;
            this.stateMonitor.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.stateMonitor.ForeColor = System.Drawing.Color.White;
            this.stateMonitor.Location = new System.Drawing.Point(194, 26);
            this.stateMonitor.Name = "stateMonitor";
            this.stateMonitor.Size = new System.Drawing.Size(106, 26);
            this.stateMonitor.TabIndex = 0;
            this.stateMonitor.Text = "연결안됨";
            this.stateMonitor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // stateTTS
            // 
            this.stateTTS.BackColor = System.Drawing.Color.Red;
            this.stateTTS.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.stateTTS.ForeColor = System.Drawing.Color.White;
            this.stateTTS.Location = new System.Drawing.Point(194, 72);
            this.stateTTS.Name = "stateTTS";
            this.stateTTS.Size = new System.Drawing.Size(106, 26);
            this.stateTTS.TabIndex = 1;
            this.stateTTS.Text = "연결안됨";
            this.stateTTS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // stateSOP
            // 
            this.stateSOP.BackColor = System.Drawing.Color.Red;
            this.stateSOP.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.stateSOP.ForeColor = System.Drawing.Color.White;
            this.stateSOP.Location = new System.Drawing.Point(194, 118);
            this.stateSOP.Name = "stateSOP";
            this.stateSOP.Size = new System.Drawing.Size(106, 26);
            this.stateSOP.TabIndex = 2;
            this.stateSOP.Text = "연결안됨";
            this.stateSOP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnLogFolder);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.stateBackup);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnStopSOP);
            this.groupBox1.Controls.Add(this.btnStartSOP);
            this.groupBox1.Controls.Add(this.btnStopTTS);
            this.groupBox1.Controls.Add(this.btnStartTTS);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.stateSOP);
            this.groupBox1.Controls.Add(this.stateTTS);
            this.groupBox1.Controls.Add(this.stateMonitor);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(319, 287);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "서버상태";
            // 
            // btnLogFolder
            // 
            this.btnLogFolder.Location = new System.Drawing.Point(101, 244);
            this.btnLogFolder.Name = "btnLogFolder";
            this.btnLogFolder.Size = new System.Drawing.Size(108, 26);
            this.btnLogFolder.TabIndex = 17;
            this.btnLogFolder.Text = "백업 폴더 지정";
            this.btnLogFolder.UseVisualStyleBackColor = true;
            this.btnLogFolder.Click += new System.EventHandler(this.btnLogFolder_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(101, 208);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(108, 26);
            this.button6.TabIndex = 17;
            this.button6.Text = "로그 백업 받기";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 251);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "로그경로";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "서버로그";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 171);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 15;
            this.label7.Text = "수신반서버";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(147, 164);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(44, 26);
            this.button4.TabIndex = 14;
            this.button4.Text = "종료";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(101, 164);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(44, 26);
            this.button5.TabIndex = 13;
            this.button5.Text = "시작";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // stateBackup
            // 
            this.stateBackup.BackColor = System.Drawing.Color.Red;
            this.stateBackup.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.stateBackup.ForeColor = System.Drawing.Color.White;
            this.stateBackup.Location = new System.Drawing.Point(194, 164);
            this.stateBackup.Name = "stateBackup";
            this.stateBackup.Size = new System.Drawing.Size(106, 26);
            this.stateBackup.TabIndex = 12;
            this.stateBackup.Text = "연결안됨";
            this.stateBackup.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "SOP서버";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "방송서버";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "모니터링";
            // 
            // btnStopSOP
            // 
            this.btnStopSOP.Location = new System.Drawing.Point(147, 118);
            this.btnStopSOP.Name = "btnStopSOP";
            this.btnStopSOP.Size = new System.Drawing.Size(44, 26);
            this.btnStopSOP.TabIndex = 8;
            this.btnStopSOP.Text = "종료";
            this.btnStopSOP.UseVisualStyleBackColor = true;
            this.btnStopSOP.Click += new System.EventHandler(this.btnStopSOP_Click);
            // 
            // btnStartSOP
            // 
            this.btnStartSOP.Location = new System.Drawing.Point(101, 118);
            this.btnStartSOP.Name = "btnStartSOP";
            this.btnStartSOP.Size = new System.Drawing.Size(44, 26);
            this.btnStartSOP.TabIndex = 7;
            this.btnStartSOP.Text = "시작";
            this.btnStartSOP.UseVisualStyleBackColor = true;
            this.btnStartSOP.Click += new System.EventHandler(this.btnStartSOP_Click);
            // 
            // btnStopTTS
            // 
            this.btnStopTTS.Location = new System.Drawing.Point(147, 72);
            this.btnStopTTS.Name = "btnStopTTS";
            this.btnStopTTS.Size = new System.Drawing.Size(44, 26);
            this.btnStopTTS.TabIndex = 6;
            this.btnStopTTS.Text = "종료";
            this.btnStopTTS.UseVisualStyleBackColor = true;
            this.btnStopTTS.Click += new System.EventHandler(this.btnStopTTS_Click);
            // 
            // btnStartTTS
            // 
            this.btnStartTTS.Location = new System.Drawing.Point(101, 72);
            this.btnStartTTS.Name = "btnStartTTS";
            this.btnStartTTS.Size = new System.Drawing.Size(44, 26);
            this.btnStartTTS.TabIndex = 5;
            this.btnStartTTS.Text = "시작";
            this.btnStartTTS.UseVisualStyleBackColor = true;
            this.btnStartTTS.Click += new System.EventHandler(this.btnStartTTS_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(147, 26);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(44, 26);
            this.button3.TabIndex = 4;
            this.button3.Text = "종료";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(101, 26);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(44, 26);
            this.button2.TabIndex = 3;
            this.button2.Text = "시작";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(246, 377);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(68, 26);
            this.button1.TabIndex = 4;
            this.button1.Text = "종료";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(52, 377);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(68, 26);
            this.button8.TabIndex = 5;
            this.button8.Text = "숨기기";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // m_CheckTimer
            // 
            this.m_CheckTimer.Interval = 1000;
            this.m_CheckTimer.Tick += new System.EventHandler(this.m_CheckTimer_Tick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnUpdateImmediately);
            this.groupBox2.Location = new System.Drawing.Point(13, 314);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(319, 56);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "System Update";
            // 
            // btnUpdateImmediately
            // 
            this.btnUpdateImmediately.Location = new System.Drawing.Point(101, 20);
            this.btnUpdateImmediately.Name = "btnUpdateImmediately";
            this.btnUpdateImmediately.Size = new System.Drawing.Size(90, 23);
            this.btnUpdateImmediately.TabIndex = 0;
            this.btnUpdateImmediately.Text = "즉시 업데이트";
            this.btnUpdateImmediately.UseVisualStyleBackColor = true;
            this.btnUpdateImmediately.Click += new System.EventHandler(this.btnUpdateImmediately_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 412);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "서버 모니터";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label stateMonitor;
        private System.Windows.Forms.Label stateTTS;
        private System.Windows.Forms.Label stateSOP;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStopSOP;
        private System.Windows.Forms.Button btnStartSOP;
        private System.Windows.Forms.Button btnStopTTS;
        private System.Windows.Forms.Button btnStartTTS;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label stateBackup;
        private System.Windows.Forms.Timer m_CheckTimer;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLogFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnUpdateImmediately;


    }
}

