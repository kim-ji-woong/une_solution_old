namespace SDMS.WeatherDisplay
{
    partial class FormRain
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
            this.pictureBoxLeft = new SDMS.WeatherDisplay.PictureBoxArrow();
            this.pictureBoxRight = new SDMS.WeatherDisplay.PictureBoxArrow();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelMaxWindSpeed = new System.Windows.Forms.Panel();
            this.panelMaxWindData = new System.Windows.Forms.Panel();
            this.labelMaxWindSpeed = new System.Windows.Forms.Label();
            this.panelAveWindSpeed = new System.Windows.Forms.Panel();
            this.panelAveWindData = new System.Windows.Forms.Panel();
            this.labelAveWindSpeed = new System.Windows.Forms.Label();
            this.panelRainDay = new System.Windows.Forms.Panel();
            this.labelRainDay = new System.Windows.Forms.Label();
            this.panelRainHour = new System.Windows.Forms.Panel();
            this.labelRainHour = new System.Windows.Forms.Label();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelDate = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            this.panelRight.SuspendLayout();
            this.panelMaxWindSpeed.SuspendLayout();
            this.panelMaxWindData.SuspendLayout();
            this.panelAveWindSpeed.SuspendLayout();
            this.panelAveWindData.SuspendLayout();
            this.panelRainDay.SuspendLayout();
            this.panelRainHour.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.SuspendLayout();
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
            this.pictureBoxLeft.TabIndex = 3;
            this.pictureBoxLeft.TabStop = false;
            this.pictureBoxLeft.Click += new System.EventHandler(this.pictureBoxLeft_Click);
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
            this.pictureBoxRight.TabIndex = 4;
            this.pictureBoxRight.TabStop = false;
            this.pictureBoxRight.Click += new System.EventHandler(this.pictureBoxRight_Click);
            // 
            // panelRight
            // 
            this.panelRight.BackgroundImage = global::SDMS.Properties.Resources.right_background;
            this.panelRight.Controls.Add(this.panelMaxWindSpeed);
            this.panelRight.Controls.Add(this.pictureBoxRight);
            this.panelRight.Controls.Add(this.panelAveWindSpeed);
            this.panelRight.Controls.Add(this.panelRainDay);
            this.panelRight.Controls.Add(this.panelRainHour);
            this.panelRight.Location = new System.Drawing.Point(159, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(317, 272);
            this.panelRight.TabIndex = 1;
            // 
            // panelMaxWindSpeed
            // 
            this.panelMaxWindSpeed.BackgroundImage = global::SDMS.Properties.Resources.max_wind_speed;
            this.panelMaxWindSpeed.Controls.Add(this.panelMaxWindData);
            this.panelMaxWindSpeed.Location = new System.Drawing.Point(159, 114);
            this.panelMaxWindSpeed.Name = "panelMaxWindSpeed";
            this.panelMaxWindSpeed.Size = new System.Drawing.Size(130, 134);
            this.panelMaxWindSpeed.TabIndex = 1;
            // 
            // panelMaxWindData
            // 
            this.panelMaxWindData.BackgroundImage = global::SDMS.Properties.Resources.wind_speed_1;
            this.panelMaxWindData.Controls.Add(this.labelMaxWindSpeed);
            this.panelMaxWindData.Location = new System.Drawing.Point(0, 26);
            this.panelMaxWindData.Name = "panelMaxWindData";
            this.panelMaxWindData.Size = new System.Drawing.Size(130, 108);
            this.panelMaxWindData.TabIndex = 0;
            // 
            // labelMaxWindSpeed
            // 
            this.labelMaxWindSpeed.AutoSize = true;
            this.labelMaxWindSpeed.BackColor = System.Drawing.Color.Transparent;
            this.labelMaxWindSpeed.Font = new System.Drawing.Font("Verdana", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMaxWindSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(173)))), ((int)(((byte)(76)))));
            this.labelMaxWindSpeed.Location = new System.Drawing.Point(-6, 68);
            this.labelMaxWindSpeed.Name = "labelMaxWindSpeed";
            this.labelMaxWindSpeed.Size = new System.Drawing.Size(108, 38);
            this.labelMaxWindSpeed.TabIndex = 1;
            this.labelMaxWindSpeed.Text = "300.0";
            // 
            // panelAveWindSpeed
            // 
            this.panelAveWindSpeed.BackgroundImage = global::SDMS.Properties.Resources.ave_wind_speed;
            this.panelAveWindSpeed.Controls.Add(this.panelAveWindData);
            this.panelAveWindSpeed.Location = new System.Drawing.Point(23, 114);
            this.panelAveWindSpeed.Name = "panelAveWindSpeed";
            this.panelAveWindSpeed.Size = new System.Drawing.Size(130, 134);
            this.panelAveWindSpeed.TabIndex = 1;
            // 
            // panelAveWindData
            // 
            this.panelAveWindData.BackgroundImage = global::SDMS.Properties.Resources.wind_speed_1;
            this.panelAveWindData.Controls.Add(this.labelAveWindSpeed);
            this.panelAveWindData.Location = new System.Drawing.Point(0, 26);
            this.panelAveWindData.Name = "panelAveWindData";
            this.panelAveWindData.Size = new System.Drawing.Size(130, 108);
            this.panelAveWindData.TabIndex = 0;
            // 
            // labelAveWindSpeed
            // 
            this.labelAveWindSpeed.AutoSize = true;
            this.labelAveWindSpeed.BackColor = System.Drawing.Color.Transparent;
            this.labelAveWindSpeed.Font = new System.Drawing.Font("Verdana", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAveWindSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(173)))), ((int)(((byte)(76)))));
            this.labelAveWindSpeed.Location = new System.Drawing.Point(30, 67);
            this.labelAveWindSpeed.Name = "labelAveWindSpeed";
            this.labelAveWindSpeed.Size = new System.Drawing.Size(68, 38);
            this.labelAveWindSpeed.TabIndex = 1;
            this.labelAveWindSpeed.Text = "3.0";
            // 
            // panelRainDay
            // 
            this.panelRainDay.BackgroundImage = global::SDMS.Properties.Resources.rain_per_day;
            this.panelRainDay.Controls.Add(this.labelRainDay);
            this.panelRainDay.Location = new System.Drawing.Point(159, 18);
            this.panelRainDay.Name = "panelRainDay";
            this.panelRainDay.Size = new System.Drawing.Size(130, 90);
            this.panelRainDay.TabIndex = 0;
            // 
            // labelRainDay
            // 
            this.labelRainDay.AutoSize = true;
            this.labelRainDay.BackColor = System.Drawing.Color.Transparent;
            this.labelRainDay.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRainDay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(86)))), ((int)(((byte)(12)))));
            this.labelRainDay.Location = new System.Drawing.Point(34, 53);
            this.labelRainDay.Name = "labelRainDay";
            this.labelRainDay.Size = new System.Drawing.Size(67, 29);
            this.labelRainDay.TabIndex = 0;
            this.labelRainDay.Text = "10.0";
            // 
            // panelRainHour
            // 
            this.panelRainHour.BackgroundImage = global::SDMS.Properties.Resources.rain_per_hour;
            this.panelRainHour.Controls.Add(this.labelRainHour);
            this.panelRainHour.Location = new System.Drawing.Point(23, 18);
            this.panelRainHour.Name = "panelRainHour";
            this.panelRainHour.Size = new System.Drawing.Size(130, 90);
            this.panelRainHour.TabIndex = 0;
            // 
            // labelRainHour
            // 
            this.labelRainHour.AutoSize = true;
            this.labelRainHour.BackColor = System.Drawing.Color.Transparent;
            this.labelRainHour.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRainHour.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(86)))), ((int)(((byte)(12)))));
            this.labelRainHour.Location = new System.Drawing.Point(16, 53);
            this.labelRainHour.Name = "labelRainHour";
            this.labelRainHour.Size = new System.Drawing.Size(82, 29);
            this.labelRainHour.TabIndex = 0;
            this.labelRainHour.Text = "100.0";
            // 
            // panelLeft
            // 
            this.panelLeft.BackgroundImage = global::SDMS.Properties.Resources.no_rain;
            this.panelLeft.Controls.Add(this.pictureBoxLeft);
            this.panelLeft.Controls.Add(this.labelTime);
            this.panelLeft.Controls.Add(this.labelDate);
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(159, 272);
            this.panelLeft.TabIndex = 0;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.BackColor = System.Drawing.Color.Transparent;
            this.labelTime.Font = new System.Drawing.Font("돋움체", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(225)))), ((int)(((byte)(253)))));
            this.labelTime.Location = new System.Drawing.Point(10, 230);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(52, 15);
            this.labelTime.TabIndex = 4;
            this.labelTime.Text = "17:31";
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.BackColor = System.Drawing.Color.Transparent;
            this.labelDate.Font = new System.Drawing.Font("돋움체", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(225)))), ((int)(((byte)(253)))));
            this.labelDate.Location = new System.Drawing.Point(10, 213);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(148, 15);
            this.labelDate.TabIndex = 5;
            this.labelDate.Text = "2015년 05월 25일";
            // 
            // FormRain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 272);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRain";
            this.Text = "FormRain";
            this.Load += new System.EventHandler(this.FormRain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.panelMaxWindSpeed.ResumeLayout(false);
            this.panelMaxWindData.ResumeLayout(false);
            this.panelMaxWindData.PerformLayout();
            this.panelAveWindSpeed.ResumeLayout(false);
            this.panelAveWindData.ResumeLayout(false);
            this.panelAveWindData.PerformLayout();
            this.panelRainDay.ResumeLayout(false);
            this.panelRainDay.PerformLayout();
            this.panelRainHour.ResumeLayout(false);
            this.panelRainHour.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelMaxWindSpeed;
        private System.Windows.Forms.Panel panelMaxWindData;
        private System.Windows.Forms.Panel panelAveWindSpeed;
        private System.Windows.Forms.Panel panelAveWindData;
        private System.Windows.Forms.Panel panelRainDay;
        private System.Windows.Forms.Panel panelRainHour;
        private System.Windows.Forms.Label labelRainDay;
        private System.Windows.Forms.Label labelRainHour;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Label labelAveWindSpeed;
        private System.Windows.Forms.Label labelMaxWindSpeed;
        private PictureBoxArrow pictureBoxLeft;
        private PictureBoxArrow pictureBoxRight;
    }
}