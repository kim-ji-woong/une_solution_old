namespace DidViewer.Composition
{
    partial class uBottom
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.picClock = new System.Windows.Forms.PictureBox();
            this.picCalendar = new System.Windows.Forms.PictureBox();
            this.picText = new System.Windows.Forms.PictureBox();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picClock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCalendar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // picClock
            // 
            this.picClock.BackColor = System.Drawing.Color.Transparent;
            this.picClock.Image = global::DidViewer.Properties.Resources.Clock;
            this.picClock.Location = new System.Drawing.Point(401, 28);
            this.picClock.Name = "picClock";
            this.picClock.Size = new System.Drawing.Size(30, 30);
            this.picClock.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picClock.TabIndex = 9;
            this.picClock.TabStop = false;
            // 
            // picCalendar
            // 
            this.picCalendar.BackColor = System.Drawing.Color.Transparent;
            this.picCalendar.Image = global::DidViewer.Properties.Resources.Calendar;
            this.picCalendar.Location = new System.Drawing.Point(40, 27);
            this.picCalendar.Name = "picCalendar";
            this.picCalendar.Size = new System.Drawing.Size(30, 33);
            this.picCalendar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picCalendar.TabIndex = 8;
            this.picCalendar.TabStop = false;
            // 
            // picText
            // 
            this.picText.BackColor = System.Drawing.Color.Transparent;
            this.picText.Image = global::DidViewer.Properties.Resources.invalid_name;
            this.picText.Location = new System.Drawing.Point(677, 51);
            this.picText.Name = "picText";
            this.picText.Size = new System.Drawing.Size(94, 22);
            this.picText.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picText.TabIndex = 7;
            this.picText.TabStop = false;
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.Image = global::DidViewer.Properties.Resources.Logo;
            this.picLogo.Location = new System.Drawing.Point(339, 51);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(287, 46);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 6;
            this.picLogo.TabStop = false;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("나눔바른고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDate.ForeColor = System.Drawing.Color.White;
            this.lblDate.Location = new System.Drawing.Point(92, 31);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(77, 27);
            this.lblDate.TabIndex = 10;
            this.lblDate.Text = "label1";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("나눔바른고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTime.ForeColor = System.Drawing.Color.White;
            this.lblTime.Location = new System.Drawing.Point(445, 31);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(77, 27);
            this.lblTime.TabIndex = 11;
            this.lblTime.Text = "label1";
            // 
            // uBottom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImage = global::DidViewer.Properties.Resources.bottom_bg;
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.picClock);
            this.Controls.Add(this.picCalendar);
            this.Controls.Add(this.picText);
            this.Controls.Add(this.picLogo);
            this.Name = "uBottom";
            this.Size = new System.Drawing.Size(793, 150);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.uBottom_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.picClock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCalendar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.PictureBox picText;
        private System.Windows.Forms.PictureBox picCalendar;
        private System.Windows.Forms.PictureBox picClock;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblTime;
    }
}
