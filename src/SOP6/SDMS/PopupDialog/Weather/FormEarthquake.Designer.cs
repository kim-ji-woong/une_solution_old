namespace SDMS.WeatherDisplay
{
    partial class FormEarthquake
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
            this.labelLocation = new System.Windows.Forms.Label();
            this.labelStrength = new System.Windows.Forms.Label();
            this.labelHeight = new System.Windows.Forms.Label();
            this.pictureBoxLeft = new SDMS.WeatherDisplay.PictureBoxArrow();
            this.pictureBoxRight = new SDMS.WeatherDisplay.PictureBoxArrow();
            this.panelLocation = new SDMS.WeatherInfoPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            this.SuspendLayout();
            // 
            // labelLocation
            // 
            this.labelLocation.AutoSize = true;
            this.labelLocation.BackColor = System.Drawing.Color.Transparent;
            this.labelLocation.Font = new System.Drawing.Font("맑은 고딕", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLocation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(206)))), ((int)(((byte)(240)))));
            this.labelLocation.Location = new System.Drawing.Point(24, 182);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(122, 42);
            this.labelLocation.TabIndex = 0;
            this.labelLocation.Text = "먼 나라";
            this.labelLocation.Visible = false;
            // 
            // labelStrength
            // 
            this.labelStrength.AutoSize = true;
            this.labelStrength.BackColor = System.Drawing.Color.Transparent;
            this.labelStrength.Font = new System.Drawing.Font("맑은 고딕", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelStrength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(206)))), ((int)(((byte)(240)))));
            this.labelStrength.Location = new System.Drawing.Point(210, 101);
            this.labelStrength.Name = "labelStrength";
            this.labelStrength.Size = new System.Drawing.Size(59, 42);
            this.labelStrength.TabIndex = 0;
            this.labelStrength.Text = "7.0";
            // 
            // labelHeight
            // 
            this.labelHeight.AutoSize = true;
            this.labelHeight.BackColor = System.Drawing.Color.Transparent;
            this.labelHeight.Font = new System.Drawing.Font("맑은 고딕", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelHeight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(206)))), ((int)(((byte)(240)))));
            this.labelHeight.Location = new System.Drawing.Point(356, 101);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(59, 42);
            this.labelHeight.TabIndex = 0;
            this.labelHeight.Text = "9.0";
            // 
            // pictureBoxLeft
            // 
            this.pictureBoxLeft.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLeft.DisabledImage = null;
            this.pictureBoxLeft.EnabledImage = null;
            this.pictureBoxLeft.Image = global::SDMS.Properties.Resources.left_arrow_normal;
            this.pictureBoxLeft.Location = new System.Drawing.Point(12, 182);
            this.pictureBoxLeft.Name = "pictureBoxLeft";
            this.pictureBoxLeft.Size = new System.Drawing.Size(17, 48);
            this.pictureBoxLeft.TabIndex = 2;
            this.pictureBoxLeft.TabStop = false;
            this.pictureBoxLeft.Click += new System.EventHandler(this.pictureBoxLeft_Click);
            // 
            // pictureBoxRight
            // 
            this.pictureBoxRight.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxRight.DisabledImage = null;
            this.pictureBoxRight.EnabledImage = null;
            this.pictureBoxRight.Image = global::SDMS.Properties.Resources.right_arrow_normal;
            this.pictureBoxRight.Location = new System.Drawing.Point(447, 107);
            this.pictureBoxRight.Name = "pictureBoxRight";
            this.pictureBoxRight.Size = new System.Drawing.Size(17, 48);
            this.pictureBoxRight.TabIndex = 2;
            this.pictureBoxRight.TabStop = false;
            this.pictureBoxRight.Click += new System.EventHandler(this.pictureBoxRight_Click);
            // 
            // panelLocation
            // 
            this.panelLocation.BackColor = System.Drawing.Color.Transparent;
            this.panelLocation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLocation.BufferLength = 34;
            this.panelLocation.DisplayFont = new System.Drawing.Font("맑은 고딕", 23.5F);
            this.panelLocation.DisplayLength = 34;
            this.panelLocation.Location = new System.Drawing.Point(17, 114);
            this.panelLocation.MovingAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelLocation.Name = "panelLocation";
            this.panelLocation.NotMovingAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelLocation.RealTimeInfo = null;
            this.panelLocation.Size = new System.Drawing.Size(145, 50);
            this.panelLocation.TabIndex = 1;
            this.panelLocation.Text = "FormRealTimeInfo";
            this.panelLocation.TextColor = System.Drawing.Color.White;
            // 
            // FormEarthquake
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SDMS.Properties.Resources.earthquake_background;
            this.ClientSize = new System.Drawing.Size(476, 272);
            this.Controls.Add(this.pictureBoxLeft);
            this.Controls.Add(this.pictureBoxRight);
            this.Controls.Add(this.panelLocation);
            this.Controls.Add(this.labelLocation);
            this.Controls.Add(this.labelHeight);
            this.Controls.Add(this.labelStrength);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormEarthquake";
            this.Text = "FormEarthquake";
            this.Load += new System.EventHandler(this.FormEarthquake_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelLocation;
        private System.Windows.Forms.Label labelStrength;
        private System.Windows.Forms.Label labelHeight;
        private WeatherInfoPanel panelLocation;
        private PictureBoxArrow pictureBoxRight;
        private PictureBoxArrow pictureBoxLeft;
    }
}