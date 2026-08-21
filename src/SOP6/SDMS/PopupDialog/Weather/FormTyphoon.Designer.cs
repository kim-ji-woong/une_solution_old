namespace SDMS.WeatherDisplay
{
    partial class FormTyphoon
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelLeft = new System.Windows.Forms.Panel();
            this.pictureBoxLeft = new SDMS.WeatherDisplay.PictureBoxArrow();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelDate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxWindDirection = new System.Windows.Forms.PictureBox();
            this.panelRight = new System.Windows.Forms.Panel();
            this.pictureBoxRight = new SDMS.WeatherDisplay.PictureBoxArrow();
            this.panelCenterPressure = new System.Windows.Forms.Panel();
            this.labelCenterPressure = new System.Windows.Forms.Label();
            this.panelMovingSpeed = new System.Windows.Forms.Panel();
            this.labelMovingSpeed = new System.Windows.Forms.Label();
            this.panelWindRadius = new System.Windows.Forms.Panel();
            this.labelWindRadius = new System.Windows.Forms.Label();
            this.panelWindSpeed = new System.Windows.Forms.Panel();
            this.labelWindSpeed = new System.Windows.Forms.Label();
            this.pictureBoxWindRadiusTitle = new System.Windows.Forms.PictureBox();
            this.pictureBoxMaxWindSpeedTitle = new System.Windows.Forms.PictureBox();
            this.panelLocation = new SDMS.WeatherInfoPanel();
            this.labelCenterPosition = new System.Windows.Forms.Label();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWindDirection)).BeginInit();
            this.panelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            this.panelCenterPressure.SuspendLayout();
            this.panelMovingSpeed.SuspendLayout();
            this.panelWindRadius.SuspendLayout();
            this.panelWindSpeed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWindRadiusTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMaxWindSpeedTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.BackgroundImage = global::SDMS.Properties.Resources.typhoon_left_background;
            this.panelLeft.Controls.Add(this.panelLocation);
            this.panelLeft.Controls.Add(this.pictureBoxLeft);
            this.panelLeft.Controls.Add(this.labelTime);
            this.panelLeft.Controls.Add(this.labelDate);
            this.panelLeft.Controls.Add(this.labelCenterPosition);
            this.panelLeft.Controls.Add(this.label1);
            this.panelLeft.Controls.Add(this.pictureBoxWindDirection);
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(159, 272);
            this.panelLeft.TabIndex = 3;
            // 
            // pictureBoxLeft
            // 
            this.pictureBoxLeft.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLeft.DisabledImage = null;
            this.pictureBoxLeft.EnabledImage = null;
            this.pictureBoxLeft.Image = global::SDMS.Properties.Resources.left_arrow_normal;
            this.pictureBoxLeft.Location = new System.Drawing.Point(12, 107);
            this.pictureBoxLeft.Name = "pictureBoxLeft";
            this.pictureBoxLeft.Size = new System.Drawing.Size(17, 48);
            this.pictureBoxLeft.TabIndex = 8;
            this.pictureBoxLeft.TabStop = false;
            this.pictureBoxLeft.Click += new System.EventHandler(this.pictureBoxLeft_Click);
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.BackColor = System.Drawing.Color.Transparent;
            this.labelTime.Font = new System.Drawing.Font("돋움체", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(225)))), ((int)(((byte)(253)))));
            this.labelTime.Location = new System.Drawing.Point(10, 224);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(52, 15);
            this.labelTime.TabIndex = 3;
            this.labelTime.Text = "17:31";
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.BackColor = System.Drawing.Color.Transparent;
            this.labelDate.Font = new System.Drawing.Font("돋움체", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(225)))), ((int)(((byte)(253)))));
            this.labelDate.Location = new System.Drawing.Point(10, 207);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(148, 15);
            this.labelDate.TabIndex = 3;
            this.labelDate.Text = "2015년 05월 25일";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(51, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "중심위치";
            // 
            // pictureBoxWindDirection
            // 
            this.pictureBoxWindDirection.Image = global::SDMS.Properties.Resources.dir_n;
            this.pictureBoxWindDirection.Location = new System.Drawing.Point(19, 64);
            this.pictureBoxWindDirection.Name = "pictureBoxWindDirection";
            this.pictureBoxWindDirection.Size = new System.Drawing.Size(129, 126);
            this.pictureBoxWindDirection.TabIndex = 0;
            this.pictureBoxWindDirection.TabStop = false;
            // 
            // panelRight
            // 
            this.panelRight.BackgroundImage = global::SDMS.Properties.Resources.right_background;
            this.panelRight.Controls.Add(this.pictureBoxRight);
            this.panelRight.Controls.Add(this.panelCenterPressure);
            this.panelRight.Controls.Add(this.panelMovingSpeed);
            this.panelRight.Controls.Add(this.panelWindRadius);
            this.panelRight.Controls.Add(this.panelWindSpeed);
            this.panelRight.Controls.Add(this.pictureBoxWindRadiusTitle);
            this.panelRight.Controls.Add(this.pictureBoxMaxWindSpeedTitle);
            this.panelRight.Location = new System.Drawing.Point(159, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(317, 272);
            this.panelRight.TabIndex = 2;
            // 
            // pictureBoxRight
            // 
            this.pictureBoxRight.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxRight.DisabledImage = null;
            this.pictureBoxRight.EnabledImage = null;
            this.pictureBoxRight.Image = global::SDMS.Properties.Resources.right_arrow_normal;
            this.pictureBoxRight.Location = new System.Drawing.Point(288, 107);
            this.pictureBoxRight.Name = "pictureBoxRight";
            this.pictureBoxRight.Size = new System.Drawing.Size(17, 48);
            this.pictureBoxRight.TabIndex = 8;
            this.pictureBoxRight.TabStop = false;
            this.pictureBoxRight.Click += new System.EventHandler(this.pictureBoxRight_Click);
            // 
            // panelCenterPressure
            // 
            this.panelCenterPressure.BackgroundImage = global::SDMS.Properties.Resources.center_pressure;
            this.panelCenterPressure.Controls.Add(this.labelCenterPressure);
            this.panelCenterPressure.Location = new System.Drawing.Point(23, 207);
            this.panelCenterPressure.Name = "panelCenterPressure";
            this.panelCenterPressure.Size = new System.Drawing.Size(268, 44);
            this.panelCenterPressure.TabIndex = 7;
            // 
            // labelCenterPressure
            // 
            this.labelCenterPressure.AutoSize = true;
            this.labelCenterPressure.BackColor = System.Drawing.Color.Transparent;
            this.labelCenterPressure.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCenterPressure.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(99)))), ((int)(((byte)(162)))));
            this.labelCenterPressure.Location = new System.Drawing.Point(186, 16);
            this.labelCenterPressure.Name = "labelCenterPressure";
            this.labelCenterPressure.Size = new System.Drawing.Size(46, 25);
            this.labelCenterPressure.TabIndex = 0;
            this.labelCenterPressure.Text = "8.0";
            // 
            // panelMovingSpeed
            // 
            this.panelMovingSpeed.BackgroundImage = global::SDMS.Properties.Resources.moving_speed;
            this.panelMovingSpeed.Controls.Add(this.labelMovingSpeed);
            this.panelMovingSpeed.Location = new System.Drawing.Point(23, 157);
            this.panelMovingSpeed.Name = "panelMovingSpeed";
            this.panelMovingSpeed.Size = new System.Drawing.Size(268, 50);
            this.panelMovingSpeed.TabIndex = 6;
            // 
            // labelMovingSpeed
            // 
            this.labelMovingSpeed.AutoSize = true;
            this.labelMovingSpeed.BackColor = System.Drawing.Color.Transparent;
            this.labelMovingSpeed.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMovingSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(99)))), ((int)(((byte)(162)))));
            this.labelMovingSpeed.Location = new System.Drawing.Point(95, 19);
            this.labelMovingSpeed.Name = "labelMovingSpeed";
            this.labelMovingSpeed.Size = new System.Drawing.Size(32, 32);
            this.labelMovingSpeed.TabIndex = 0;
            this.labelMovingSpeed.Text = "5";
            // 
            // panelWindRadius
            // 
            this.panelWindRadius.BackgroundImage = global::SDMS.Properties.Resources.wind_radius;
            this.panelWindRadius.Controls.Add(this.labelWindRadius);
            this.panelWindRadius.Location = new System.Drawing.Point(159, 42);
            this.panelWindRadius.Name = "panelWindRadius";
            this.panelWindRadius.Size = new System.Drawing.Size(130, 108);
            this.panelWindRadius.TabIndex = 5;
            // 
            // labelWindRadius
            // 
            this.labelWindRadius.AutoSize = true;
            this.labelWindRadius.BackColor = System.Drawing.Color.Transparent;
            this.labelWindRadius.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWindRadius.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(173)))), ((int)(((byte)(76)))));
            this.labelWindRadius.Location = new System.Drawing.Point(48, 36);
            this.labelWindRadius.Name = "labelWindRadius";
            this.labelWindRadius.Size = new System.Drawing.Size(32, 32);
            this.labelWindRadius.TabIndex = 0;
            this.labelWindRadius.Text = "5";
            // 
            // panelWindSpeed
            // 
            this.panelWindSpeed.BackgroundImage = global::SDMS.Properties.Resources.wind_speed_1;
            this.panelWindSpeed.Controls.Add(this.labelWindSpeed);
            this.panelWindSpeed.Location = new System.Drawing.Point(23, 42);
            this.panelWindSpeed.Name = "panelWindSpeed";
            this.panelWindSpeed.Size = new System.Drawing.Size(130, 108);
            this.panelWindSpeed.TabIndex = 4;
            // 
            // labelWindSpeed
            // 
            this.labelWindSpeed.AutoSize = true;
            this.labelWindSpeed.BackColor = System.Drawing.Color.Transparent;
            this.labelWindSpeed.Font = new System.Drawing.Font("Verdana", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWindSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(173)))), ((int)(((byte)(76)))));
            this.labelWindSpeed.Location = new System.Drawing.Point(29, 70);
            this.labelWindSpeed.Name = "labelWindSpeed";
            this.labelWindSpeed.Size = new System.Drawing.Size(68, 38);
            this.labelWindSpeed.TabIndex = 0;
            this.labelWindSpeed.Text = "3.0";
            // 
            // pictureBoxWindRadiusTitle
            // 
            this.pictureBoxWindRadiusTitle.Image = global::SDMS.Properties.Resources.wind_radius_tit;
            this.pictureBoxWindRadiusTitle.Location = new System.Drawing.Point(159, 16);
            this.pictureBoxWindRadiusTitle.Name = "pictureBoxWindRadiusTitle";
            this.pictureBoxWindRadiusTitle.Size = new System.Drawing.Size(130, 26);
            this.pictureBoxWindRadiusTitle.TabIndex = 1;
            this.pictureBoxWindRadiusTitle.TabStop = false;
            // 
            // pictureBoxMaxWindSpeedTitle
            // 
            this.pictureBoxMaxWindSpeedTitle.Image = global::SDMS.Properties.Resources.max_wind_speed_tit;
            this.pictureBoxMaxWindSpeedTitle.Location = new System.Drawing.Point(23, 16);
            this.pictureBoxMaxWindSpeedTitle.Name = "pictureBoxMaxWindSpeedTitle";
            this.pictureBoxMaxWindSpeedTitle.Size = new System.Drawing.Size(130, 26);
            this.pictureBoxMaxWindSpeedTitle.TabIndex = 1;
            this.pictureBoxMaxWindSpeedTitle.TabStop = false;
            // 
            // panelLocation
            // 
            this.panelLocation.BackColor = System.Drawing.Color.Transparent;
            this.panelLocation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLocation.BufferLength = 34;
            this.panelLocation.DisplayFont = new System.Drawing.Font("맑은 고딕", 23.5F);
            this.panelLocation.DisplayLength = 34;
            this.panelLocation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panelLocation.Location = new System.Drawing.Point(14, 32);
            this.panelLocation.Name = "panelLocation";
            this.panelLocation.RealTimeInfo = null;
            this.panelLocation.Size = new System.Drawing.Size(145, 35);
            this.panelLocation.TabIndex = 1;
            this.panelLocation.Text = "FormRealTimeInfo";
            this.panelLocation.TextColor = System.Drawing.Color.White;
            // 
            // labelCenterPosition
            // 
            this.labelCenterPosition.AutoSize = true;
            this.labelCenterPosition.BackColor = System.Drawing.Color.Transparent;
            this.labelCenterPosition.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCenterPosition.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
            this.labelCenterPosition.Location = new System.Drawing.Point(18, 32);
            this.labelCenterPosition.Name = "labelCenterPosition";
            this.labelCenterPosition.Size = new System.Drawing.Size(134, 21);
            this.labelCenterPosition.TabIndex = 2;
            this.labelCenterPosition.Text = "태풍의 중심 위치";
            this.labelCenterPosition.Visible = false;
            // 
            // FormTyphoon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 272);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelRight);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormTyphoon";
            this.Text = "FormRain";
            this.Load += new System.EventHandler(this.FormTyphoon_Load);
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWindDirection)).EndInit();
            this.panelRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            this.panelCenterPressure.ResumeLayout(false);
            this.panelCenterPressure.PerformLayout();
            this.panelMovingSpeed.ResumeLayout(false);
            this.panelMovingSpeed.PerformLayout();
            this.panelWindRadius.ResumeLayout(false);
            this.panelWindRadius.PerformLayout();
            this.panelWindSpeed.ResumeLayout(false);
            this.panelWindSpeed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWindRadiusTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMaxWindSpeedTitle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxMaxWindSpeedTitle;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.PictureBox pictureBoxWindRadiusTitle;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.PictureBox pictureBoxWindDirection;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelCenterPressure;
        private System.Windows.Forms.Panel panelMovingSpeed;
        private System.Windows.Forms.Panel panelWindRadius;
        private System.Windows.Forms.Label labelWindRadius;
        private System.Windows.Forms.Panel panelWindSpeed;
        private System.Windows.Forms.Label labelWindSpeed;
        private System.Windows.Forms.Label labelCenterPressure;
        private System.Windows.Forms.Label labelMovingSpeed;
        private PictureBoxArrow pictureBoxLeft;
        private PictureBoxArrow pictureBoxRight;
        private WeatherInfoPanel panelLocation;
        private System.Windows.Forms.Label labelCenterPosition;
    }
}