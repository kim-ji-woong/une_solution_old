namespace SecomEventReceiver
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
            this.lblSecomDBStatus = new System.Windows.Forms.Label();
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblSecomDBStatus
            // 
            this.lblSecomDBStatus.AutoSize = true;
            this.lblSecomDBStatus.ForeColor = System.Drawing.Color.Red;
            this.lblSecomDBStatus.Location = new System.Drawing.Point(26, 95);
            this.lblSecomDBStatus.Name = "lblSecomDBStatus";
            this.lblSecomDBStatus.Size = new System.Drawing.Size(209, 12);
            this.lblSecomDBStatus.TabIndex = 0;
            this.lblSecomDBStatus.Text = "Secom DB에 접속되지 못하였습니다.";
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.AutoSize = true;
            this.lblServerStatus.ForeColor = System.Drawing.Color.Red;
            this.lblServerStatus.Location = new System.Drawing.Point(26, 136);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(185, 12);
            this.lblServerStatus.TabIndex = 1;
            this.lblServerStatus.Text = "Server에 접속되지 못하였습니다.";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.lblServerStatus);
            this.Controls.Add(this.lblSecomDBStatus);
            this.Name = "FormMain";
            this.Text = "Secom 이벤트 수신기";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSecomDBStatus;
        private System.Windows.Forms.Label lblServerStatus;
    }
}

