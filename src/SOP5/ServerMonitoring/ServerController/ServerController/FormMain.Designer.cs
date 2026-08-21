namespace ServerController
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
            this.components = new System.ComponentModel.Container();
            this.labelServiceNameSample = new System.Windows.Forms.Label();
            this.labelServiceStatusSample = new System.Windows.Forms.Label();
            this.cboStatusSample = new System.Windows.Forms.ComboBox();
            this.btnSendSample = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // labelServiceNameSample
            // 
            this.labelServiceNameSample.AutoSize = true;
            this.labelServiceNameSample.Location = new System.Drawing.Point(35, 30);
            this.labelServiceNameSample.Name = "labelServiceNameSample";
            this.labelServiceNameSample.Size = new System.Drawing.Size(69, 12);
            this.labelServiceNameSample.TabIndex = 0;
            this.labelServiceNameSample.Text = "서비스 이름";
            this.labelServiceNameSample.Visible = false;
            // 
            // labelServiceStatusSample
            // 
            this.labelServiceStatusSample.AutoSize = true;
            this.labelServiceStatusSample.Location = new System.Drawing.Point(253, 30);
            this.labelServiceStatusSample.Name = "labelServiceStatusSample";
            this.labelServiceStatusSample.Size = new System.Drawing.Size(53, 12);
            this.labelServiceStatusSample.TabIndex = 1;
            this.labelServiceStatusSample.Text = "실행상태";
            this.labelServiceStatusSample.Visible = false;
            // 
            // cboStatusSample
            // 
            this.cboStatusSample.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatusSample.FormattingEnabled = true;
            this.cboStatusSample.Items.AddRange(new object[] {
            "중지",
            "실행",
            "재실행"});
            this.cboStatusSample.Location = new System.Drawing.Point(328, 27);
            this.cboStatusSample.Name = "cboStatusSample";
            this.cboStatusSample.Size = new System.Drawing.Size(67, 20);
            this.cboStatusSample.TabIndex = 2;
            this.cboStatusSample.Visible = false;
            // 
            // btnSendSample
            // 
            this.btnSendSample.Location = new System.Drawing.Point(404, 25);
            this.btnSendSample.Name = "btnSendSample";
            this.btnSendSample.Size = new System.Drawing.Size(46, 23);
            this.btnSendSample.TabIndex = 3;
            this.btnSendSample.Text = "전송";
            this.btnSendSample.UseVisualStyleBackColor = true;
            this.btnSendSample.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 260);
            this.Controls.Add(this.btnSendSample);
            this.Controls.Add(this.cboStatusSample);
            this.Controls.Add(this.labelServiceStatusSample);
            this.Controls.Add(this.labelServiceNameSample);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "서버 원격 제어";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelServiceNameSample;
        private System.Windows.Forms.Label labelServiceStatusSample;
        private System.Windows.Forms.ComboBox cboStatusSample;
        private System.Windows.Forms.Button btnSendSample;
        private System.Windows.Forms.Timer timer1;
    }
}

