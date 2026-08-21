namespace VolumeMasterSample
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
            this.radioUseSound = new System.Windows.Forms.RadioButton();
            this.radioMute = new System.Windows.Forms.RadioButton();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // radioUseSound
            // 
            this.radioUseSound.AutoSize = true;
            this.radioUseSound.Location = new System.Drawing.Point(12, 12);
            this.radioUseSound.Name = "radioUseSound";
            this.radioUseSound.Size = new System.Drawing.Size(75, 16);
            this.radioUseSound.TabIndex = 0;
            this.radioUseSound.TabStop = true;
            this.radioUseSound.Text = "소리 사용";
            this.radioUseSound.UseVisualStyleBackColor = true;
            // 
            // radioMute
            // 
            this.radioMute.AutoSize = true;
            this.radioMute.Location = new System.Drawing.Point(12, 34);
            this.radioMute.Name = "radioMute";
            this.radioMute.Size = new System.Drawing.Size(59, 16);
            this.radioMute.TabIndex = 0;
            this.radioMute.TabStop = true;
            this.radioMute.Text = "음소거";
            this.radioMute.UseVisualStyleBackColor = true;
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.Location = new System.Drawing.Point(10, 96);
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(104, 45);
            this.trackBarVolume.TabIndex = 1;
            this.trackBarVolume.ValueChanged += new System.EventHandler(this.trackBarVolume_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "볼륨 :";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(143, 118);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 154);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBarVolume);
            this.Controls.Add(this.radioMute);
            this.Controls.Add(this.radioUseSound);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioUseSound;
        private System.Windows.Forms.RadioButton radioMute;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApply;
    }
}

