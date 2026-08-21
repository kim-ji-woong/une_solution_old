namespace MuxFireSensorServer
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxReceiver = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLoop = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxRelay = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTag = new System.Windows.Forms.TextBox();
            this.checkBoxAlarm = new System.Windows.Forms.CheckBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnSensorTagNoAlarm = new System.Windows.Forms.Button();
            this.txtSensorTagNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbSensorTagNoAlarm = new System.Windows.Forms.CheckBox();
            this.textBoxRelayTeam = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Receiver";
            // 
            // textBoxReceiver
            // 
            this.textBoxReceiver.Location = new System.Drawing.Point(97, 23);
            this.textBoxReceiver.Name = "textBoxReceiver";
            this.textBoxReceiver.Size = new System.Drawing.Size(55, 21);
            this.textBoxReceiver.TabIndex = 1;
            this.textBoxReceiver.Text = "1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Loop";
            // 
            // textBoxLoop
            // 
            this.textBoxLoop.Location = new System.Drawing.Point(97, 72);
            this.textBoxLoop.Name = "textBoxLoop";
            this.textBoxLoop.Size = new System.Drawing.Size(55, 21);
            this.textBoxLoop.TabIndex = 1;
            this.textBoxLoop.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Relay";
            // 
            // textBoxRelay
            // 
            this.textBoxRelay.Location = new System.Drawing.Point(97, 96);
            this.textBoxRelay.Name = "textBoxRelay";
            this.textBoxRelay.Size = new System.Drawing.Size(55, 21);
            this.textBoxRelay.TabIndex = 1;
            this.textBoxRelay.Text = "12";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Tag";
            // 
            // textBoxTag
            // 
            this.textBoxTag.Location = new System.Drawing.Point(97, 120);
            this.textBoxTag.Name = "textBoxTag";
            this.textBoxTag.Size = new System.Drawing.Size(55, 21);
            this.textBoxTag.TabIndex = 1;
            this.textBoxTag.Text = "1";
            // 
            // checkBoxAlarm
            // 
            this.checkBoxAlarm.AutoSize = true;
            this.checkBoxAlarm.Location = new System.Drawing.Point(35, 152);
            this.checkBoxAlarm.Name = "checkBoxAlarm";
            this.checkBoxAlarm.Size = new System.Drawing.Size(57, 16);
            this.checkBoxAlarm.TabIndex = 2;
            this.checkBoxAlarm.Text = "Alarm";
            this.checkBoxAlarm.UseVisualStyleBackColor = true;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(35, 185);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(87, 21);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "전송";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnSensorTagNoAlarm
            // 
            this.btnSensorTagNoAlarm.Location = new System.Drawing.Point(223, 185);
            this.btnSensorTagNoAlarm.Name = "btnSensorTagNoAlarm";
            this.btnSensorTagNoAlarm.Size = new System.Drawing.Size(75, 23);
            this.btnSensorTagNoAlarm.TabIndex = 4;
            this.btnSensorTagNoAlarm.Text = "전송";
            this.btnSensorTagNoAlarm.UseVisualStyleBackColor = true;
            this.btnSensorTagNoAlarm.Click += new System.EventHandler(this.btnSensorTagNoAlarm_Click);
            // 
            // txtSensorTagNo
            // 
            this.txtSensorTagNo.Location = new System.Drawing.Point(223, 158);
            this.txtSensorTagNo.Name = "txtSensorTagNo";
            this.txtSensorTagNo.Size = new System.Drawing.Size(138, 21);
            this.txtSensorTagNo.TabIndex = 5;
            this.txtSensorTagNo.Text = "100441";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(221, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(163, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "SensorTagNo로 이벤트 전송";
            // 
            // cbSensorTagNoAlarm
            // 
            this.cbSensorTagNoAlarm.AutoSize = true;
            this.cbSensorTagNoAlarm.Location = new System.Drawing.Point(367, 160);
            this.cbSensorTagNoAlarm.Name = "cbSensorTagNoAlarm";
            this.cbSensorTagNoAlarm.Size = new System.Drawing.Size(57, 16);
            this.cbSensorTagNoAlarm.TabIndex = 7;
            this.cbSensorTagNoAlarm.Text = "Alarm";
            this.cbSensorTagNoAlarm.UseVisualStyleBackColor = true;
            // 
            // textBoxRelayTeam
            // 
            this.textBoxRelayTeam.Location = new System.Drawing.Point(97, 48);
            this.textBoxRelayTeam.Name = "textBoxRelayTeam";
            this.textBoxRelayTeam.Size = new System.Drawing.Size(55, 21);
            this.textBoxRelayTeam.TabIndex = 9;
            this.textBoxRelayTeam.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(31, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "RelayTeam";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 240);
            this.Controls.Add(this.textBoxRelayTeam);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbSensorTagNoAlarm);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtSensorTagNo);
            this.Controls.Add(this.btnSensorTagNoAlarm);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.checkBoxAlarm);
            this.Controls.Add(this.textBoxTag);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxRelay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxLoop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxReceiver);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxReceiver;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxLoop;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxRelay;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTag;
        private System.Windows.Forms.CheckBox checkBoxAlarm;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnSensorTagNoAlarm;
        private System.Windows.Forms.TextBox txtSensorTagNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbSensorTagNoAlarm;
        private System.Windows.Forms.TextBox textBoxRelayTeam;
        private System.Windows.Forms.Label label6;
    }
}

