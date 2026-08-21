namespace UBMLViewer
{
    partial class Splash
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
            this.components = new System.ComponentModel.Container();
            this.m_SplashPictureBox = new System.Windows.Forms.PictureBox();
            this.m_SplashTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.m_SplashPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // m_SplashPictureBox
            // 
            this.m_SplashPictureBox.BackColor = System.Drawing.Color.White;
            this.m_SplashPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_SplashPictureBox.Location = new System.Drawing.Point(0, 0);
            this.m_SplashPictureBox.Name = "m_SplashPictureBox";
            this.m_SplashPictureBox.Size = new System.Drawing.Size(630, 348);
            this.m_SplashPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.m_SplashPictureBox.TabIndex = 0;
            this.m_SplashPictureBox.TabStop = false;
            // 
            // m_SplashTimer
            // 
            this.m_SplashTimer.Interval = 3000;
            this.m_SplashTimer.Tick += new System.EventHandler(this.m_SplashTimer_Tick);
            // 
            // Splash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 348);
            this.ControlBox = false;
            this.Controls.Add(this.m_SplashPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Splash";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Splash";
            this.Load += new System.EventHandler(this.Splash_Load);
            ((System.ComponentModel.ISupportInitialize)(this.m_SplashPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox m_SplashPictureBox;
        private System.Windows.Forms.Timer m_SplashTimer;
    }
}