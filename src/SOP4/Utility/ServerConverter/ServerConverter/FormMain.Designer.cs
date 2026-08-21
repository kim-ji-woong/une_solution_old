namespace ServerConverter
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
            this.radioLocal = new System.Windows.Forms.RadioButton();
            this.radioRemote = new System.Windows.Forms.RadioButton();
            this.btnConfig = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // radioLocal
            // 
            this.radioLocal.AutoSize = true;
            this.radioLocal.Location = new System.Drawing.Point(12, 12);
            this.radioLocal.Name = "radioLocal";
            this.radioLocal.Size = new System.Drawing.Size(71, 16);
            this.radioLocal.TabIndex = 0;
            this.radioLocal.TabStop = true;
            this.radioLocal.Text = "로컬서버";
            this.radioLocal.UseVisualStyleBackColor = true;
            this.radioLocal.CheckedChanged += new System.EventHandler(this.radioLocal_CheckedChanged);
            // 
            // radioRemote
            // 
            this.radioRemote.AutoSize = true;
            this.radioRemote.Location = new System.Drawing.Point(12, 34);
            this.radioRemote.Name = "radioRemote";
            this.radioRemote.Size = new System.Drawing.Size(75, 16);
            this.radioRemote.TabIndex = 0;
            this.radioRemote.TabStop = true;
            this.radioRemote.Text = "원격 서버";
            this.radioRemote.UseVisualStyleBackColor = true;
            this.radioRemote.CheckedChanged += new System.EventHandler(this.radioRemote_CheckedChanged);
            // 
            // btnConfig
            // 
            this.btnConfig.Location = new System.Drawing.Point(121, 27);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(75, 23);
            this.btnConfig.TabIndex = 1;
            this.btnConfig.Text = "설정";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 69);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.radioRemote);
            this.Controls.Add(this.radioLocal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "Server 교환기";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioLocal;
        private System.Windows.Forms.RadioButton radioRemote;
        private System.Windows.Forms.Button btnConfig;
    }
}

