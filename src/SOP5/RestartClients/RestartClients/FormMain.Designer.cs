namespace RestartClients
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
            this.labelStatus = new System.Windows.Forms.Label();
            this.btnSendRestart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(42, 96);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(53, 12);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "상태정보";
            // 
            // btnSendRestart
            // 
            this.btnSendRestart.Enabled = false;
            this.btnSendRestart.Location = new System.Drawing.Point(72, 151);
            this.btnSendRestart.Name = "btnSendRestart";
            this.btnSendRestart.Size = new System.Drawing.Size(129, 23);
            this.btnSendRestart.TabIndex = 1;
            this.btnSendRestart.Text = "모든 Client 재시작";
            this.btnSendRestart.UseVisualStyleBackColor = true;
            this.btnSendRestart.Click += new System.EventHandler(this.btnSendRestart_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnSendRestart);
            this.Controls.Add(this.labelStatus);
            this.Name = "FormMain";
            this.Text = "재접속 요청";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button btnSendRestart;
    }
}

