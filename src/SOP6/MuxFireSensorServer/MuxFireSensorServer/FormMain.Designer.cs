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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Receiver";
            // 
            // textBoxReceiver
            // 
            this.textBoxReceiver.Location = new System.Drawing.Point(83, 52);
            this.textBoxReceiver.Name = "textBoxReceiver";
            this.textBoxReceiver.Size = new System.Drawing.Size(48, 20);
            this.textBoxReceiver.TabIndex = 1;
            this.textBoxReceiver.Text = "2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Loop";
            // 
            // textBoxLoop
            // 
            this.textBoxLoop.Location = new System.Drawing.Point(83, 78);
            this.textBoxLoop.Name = "textBoxLoop";
            this.textBoxLoop.Size = new System.Drawing.Size(48, 20);
            this.textBoxLoop.TabIndex = 1;
            this.textBoxLoop.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Relay";
            // 
            // textBoxRelay
            // 
            this.textBoxRelay.Location = new System.Drawing.Point(83, 104);
            this.textBoxRelay.Name = "textBoxRelay";
            this.textBoxRelay.Size = new System.Drawing.Size(48, 20);
            this.textBoxRelay.TabIndex = 1;
            this.textBoxRelay.Text = "23";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Tag";
            // 
            // textBoxTag
            // 
            this.textBoxTag.Location = new System.Drawing.Point(83, 130);
            this.textBoxTag.Name = "textBoxTag";
            this.textBoxTag.Size = new System.Drawing.Size(48, 20);
            this.textBoxTag.TabIndex = 1;
            this.textBoxTag.Text = "1";
            // 
            // checkBoxAlarm
            // 
            this.checkBoxAlarm.AutoSize = true;
            this.checkBoxAlarm.Location = new System.Drawing.Point(30, 165);
            this.checkBoxAlarm.Name = "checkBoxAlarm";
            this.checkBoxAlarm.Size = new System.Drawing.Size(52, 17);
            this.checkBoxAlarm.TabIndex = 2;
            this.checkBoxAlarm.Text = "Alarm";
            this.checkBoxAlarm.UseVisualStyleBackColor = true;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(30, 200);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "전송";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 488);
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
    }
}

