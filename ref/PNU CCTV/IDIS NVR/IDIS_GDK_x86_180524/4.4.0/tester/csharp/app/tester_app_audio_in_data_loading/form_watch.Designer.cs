namespace GDK_tester
{
    partial class form_watch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_watch));
            this.BTN_FILE_PATH = new System.Windows.Forms.Button();
            this.BTN_SAVE = new System.Windows.Forms.Button();
            this.LBL_PATH = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TXT_DURATION = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BTN_FILE_PATH
            // 
            this.BTN_FILE_PATH.Location = new System.Drawing.Point(237, 11);
            this.BTN_FILE_PATH.Name = "BTN_FILE_PATH";
            this.BTN_FILE_PATH.Size = new System.Drawing.Size(39, 27);
            this.BTN_FILE_PATH.TabIndex = 0;
            this.BTN_FILE_PATH.Text = "...";
            this.BTN_FILE_PATH.UseVisualStyleBackColor = true;
            this.BTN_FILE_PATH.Click += new System.EventHandler(this.BTN_FILE_PATH_Click);
            // 
            // BTN_SAVE
            // 
            this.BTN_SAVE.Location = new System.Drawing.Point(164, 45);
            this.BTN_SAVE.Name = "BTN_SAVE";
            this.BTN_SAVE.Size = new System.Drawing.Size(112, 27);
            this.BTN_SAVE.TabIndex = 1;
            this.BTN_SAVE.Text = "Audio File Save";
            this.BTN_SAVE.UseVisualStyleBackColor = true;
            this.BTN_SAVE.Click += new System.EventHandler(this.BTN_SAVE_Click);
            // 
            // LBL_PATH
            // 
            this.LBL_PATH.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LBL_PATH.Location = new System.Drawing.Point(66, 12);
            this.LBL_PATH.Name = "LBL_PATH";
            this.LBL_PATH.Size = new System.Drawing.Size(165, 25);
            this.LBL_PATH.TabIndex = 2;
            this.LBL_PATH.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Duration : ";
            // 
            // TXT_DURATION
            // 
            this.TXT_DURATION.Location = new System.Drawing.Point(66, 49);
            this.TXT_DURATION.Name = "TXT_DURATION";
            this.TXT_DURATION.Size = new System.Drawing.Size(65, 20);
            this.TXT_DURATION.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(135, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "sec";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Path :";
            // 
            // form_watch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 79);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TXT_DURATION);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LBL_PATH);
            this.Controls.Add(this.BTN_SAVE);
            this.Controls.Add(this.BTN_FILE_PATH);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "form_watch";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Audio Tester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_watch_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BTN_FILE_PATH;
        private System.Windows.Forms.Button BTN_SAVE;
        private System.Windows.Forms.Label LBL_PATH;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TXT_DURATION;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}