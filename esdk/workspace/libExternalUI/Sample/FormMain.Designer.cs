namespace Sample
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
            this.btnShowRed = new System.Windows.Forms.Button();
            this.btnShowTwice = new System.Windows.Forms.Button();
            this.btnShowBoth = new System.Windows.Forms.Button();
            this.btnHideRed = new System.Windows.Forms.Button();
            this.btnHideTwice = new System.Windows.Forms.Button();
            this.btnHideBoth = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnShowRed
            // 
            this.btnShowRed.Location = new System.Drawing.Point(46, 118);
            this.btnShowRed.Name = "btnShowRed";
            this.btnShowRed.Size = new System.Drawing.Size(97, 23);
            this.btnShowRed.TabIndex = 0;
            this.btnShowRed.Text = "레드벨벳 보기";
            this.btnShowRed.UseVisualStyleBackColor = true;
            this.btnShowRed.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // btnShowTwice
            // 
            this.btnShowTwice.Location = new System.Drawing.Point(46, 147);
            this.btnShowTwice.Name = "btnShowTwice";
            this.btnShowTwice.Size = new System.Drawing.Size(97, 23);
            this.btnShowTwice.TabIndex = 0;
            this.btnShowTwice.Text = "트와이스 보기";
            this.btnShowTwice.UseVisualStyleBackColor = true;
            this.btnShowTwice.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // btnShowBoth
            // 
            this.btnShowBoth.Location = new System.Drawing.Point(46, 176);
            this.btnShowBoth.Name = "btnShowBoth";
            this.btnShowBoth.Size = new System.Drawing.Size(97, 23);
            this.btnShowBoth.TabIndex = 0;
            this.btnShowBoth.Text = "둘다 보기";
            this.btnShowBoth.UseVisualStyleBackColor = true;
            this.btnShowBoth.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // btnHideRed
            // 
            this.btnHideRed.Location = new System.Drawing.Point(46, 222);
            this.btnHideRed.Name = "btnHideRed";
            this.btnHideRed.Size = new System.Drawing.Size(97, 23);
            this.btnHideRed.TabIndex = 0;
            this.btnHideRed.Text = "레드벨벳 끄기";
            this.btnHideRed.UseVisualStyleBackColor = true;
            this.btnHideRed.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // btnHideTwice
            // 
            this.btnHideTwice.Location = new System.Drawing.Point(46, 251);
            this.btnHideTwice.Name = "btnHideTwice";
            this.btnHideTwice.Size = new System.Drawing.Size(97, 23);
            this.btnHideTwice.TabIndex = 0;
            this.btnHideTwice.Text = "트와이스 끄기";
            this.btnHideTwice.UseVisualStyleBackColor = true;
            this.btnHideTwice.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // btnHideBoth
            // 
            this.btnHideBoth.Location = new System.Drawing.Point(46, 280);
            this.btnHideBoth.Name = "btnHideBoth";
            this.btnHideBoth.Size = new System.Drawing.Size(97, 23);
            this.btnHideBoth.TabIndex = 0;
            this.btnHideBoth.Text = "둘다 끄기";
            this.btnHideBoth.UseVisualStyleBackColor = true;
            this.btnHideBoth.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1512, 884);
            this.Controls.Add(this.btnHideBoth);
            this.Controls.Add(this.btnShowBoth);
            this.Controls.Add(this.btnHideTwice);
            this.Controls.Add(this.btnShowTwice);
            this.Controls.Add(this.btnHideRed);
            this.Controls.Add(this.btnShowRed);
            this.Name = "FormMain";
            this.Text = "libExternalUI 샘플";
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnShowRed;
        private System.Windows.Forms.Button btnShowTwice;
        private System.Windows.Forms.Button btnShowBoth;
        private System.Windows.Forms.Button btnHideRed;
        private System.Windows.Forms.Button btnHideTwice;
        private System.Windows.Forms.Button btnHideBoth;
    }
}

