namespace EarthquakeSensorClient
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxStation = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAlarmLevel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxHPGA = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTPGA = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPortNo = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnKeepGoing = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.timerSend = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "위치";
            // 
            // textBoxStation
            // 
            this.textBoxStation.Location = new System.Drawing.Point(72, 15);
            this.textBoxStation.Name = "textBoxStation";
            this.textBoxStation.Size = new System.Drawing.Size(75, 21);
            this.textBoxStation.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "알람레벨";
            // 
            // textBoxAlarmLevel
            // 
            this.textBoxAlarmLevel.Location = new System.Drawing.Point(72, 42);
            this.textBoxAlarmLevel.Name = "textBoxAlarmLevel";
            this.textBoxAlarmLevel.Size = new System.Drawing.Size(75, 21);
            this.textBoxAlarmLevel.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "HPGA";
            // 
            // textBoxHPGA
            // 
            this.textBoxHPGA.Location = new System.Drawing.Point(72, 69);
            this.textBoxHPGA.Name = "textBoxHPGA";
            this.textBoxHPGA.Size = new System.Drawing.Size(75, 21);
            this.textBoxHPGA.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "TPGA";
            // 
            // textBoxTPGA
            // 
            this.textBoxTPGA.Location = new System.Drawing.Point(72, 96);
            this.textBoxTPGA.Name = "textBoxTPGA";
            this.textBoxTPGA.Size = new System.Drawing.Size(75, 21);
            this.textBoxTPGA.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(158, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Port 번호";
            // 
            // textBoxPortNo
            // 
            this.textBoxPortNo.Location = new System.Drawing.Point(219, 15);
            this.textBoxPortNo.Name = "textBoxPortNo";
            this.textBoxPortNo.Size = new System.Drawing.Size(53, 21);
            this.textBoxPortNo.TabIndex = 4;
            this.textBoxPortNo.Text = "20000";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(197, 144);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "보내기";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnKeepGoing
            // 
            this.btnKeepGoing.Location = new System.Drawing.Point(14, 144);
            this.btnKeepGoing.Name = "btnKeepGoing";
            this.btnKeepGoing.Size = new System.Drawing.Size(51, 23);
            this.btnKeepGoing.TabIndex = 6;
            this.btnKeepGoing.Text = "계속";
            this.btnKeepGoing.UseVisualStyleBackColor = true;
            this.btnKeepGoing.Click += new System.EventHandler(this.btnKeepGoing_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(72, 144);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(51, 23);
            this.btnStop.TabIndex = 7;
            this.btnStop.Text = "중단";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // timerSend
            // 
            this.timerSend.Tick += new System.EventHandler(this.timerSend_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 179);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnKeepGoing);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.textBoxTPGA);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxHPGA);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxAlarmLevel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPortNo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxStation);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "지진센서 클라이언트";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxStation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxAlarmLevel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxHPGA;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTPGA;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxPortNo;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnKeepGoing;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Timer timerSend;
    }
}

