namespace ServerCommander
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
            m_netMgr.ReleaseThread();

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
            this.labelNetStatus = new System.Windows.Forms.Label();
            this.btnStartSDMS = new System.Windows.Forms.Button();
            this.btnUpdateSystem = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelNetStatus
            // 
            this.labelNetStatus.AutoSize = true;
            this.labelNetStatus.Location = new System.Drawing.Point(29, 41);
            this.labelNetStatus.Name = "labelNetStatus";
            this.labelNetStatus.Size = new System.Drawing.Size(97, 12);
            this.labelNetStatus.TabIndex = 0;
            this.labelNetStatus.Text = "Server 접속 상태";
            // 
            // btnStartSDMS
            // 
            this.btnStartSDMS.Enabled = false;
            this.btnStartSDMS.Location = new System.Drawing.Point(31, 83);
            this.btnStartSDMS.Name = "btnStartSDMS";
            this.btnStartSDMS.Size = new System.Drawing.Size(95, 23);
            this.btnStartSDMS.TabIndex = 1;
            this.btnStartSDMS.Text = "SDMS 실행";
            this.btnStartSDMS.UseVisualStyleBackColor = true;
            this.btnStartSDMS.Click += new System.EventHandler(this.btnStartSDMS_Click);
            // 
            // btnUpdateSystem
            // 
            this.btnUpdateSystem.Enabled = false;
            this.btnUpdateSystem.Location = new System.Drawing.Point(31, 133);
            this.btnUpdateSystem.Name = "btnUpdateSystem";
            this.btnUpdateSystem.Size = new System.Drawing.Size(120, 23);
            this.btnUpdateSystem.TabIndex = 2;
            this.btnUpdateSystem.Text = "통합관리자 재시작";
            this.btnUpdateSystem.UseVisualStyleBackColor = true;
            this.btnUpdateSystem.Click += new System.EventHandler(this.btnUpdateSystem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 384);
            this.Controls.Add(this.btnUpdateSystem);
            this.Controls.Add(this.btnStartSDMS);
            this.Controls.Add(this.labelNetStatus);
            this.Name = "FormMain";
            this.Text = "ServerCommander";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelNetStatus;
        private System.Windows.Forms.Button btnStartSDMS;
        private System.Windows.Forms.Button btnUpdateSystem;
    }
}

