namespace SDMS
{
    partial class WeatherInfoPanel
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
            this.m_TextScrollTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // m_TextScrollTimer
            // 
            this.m_TextScrollTimer.Interval = 500;
            this.m_TextScrollTimer.Tick += new System.EventHandler(this.TextScrollTimer_Tick);
            // 
            // FormRealTimeInfo
            // 
           
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(450, 87);        
            this.Name = "FormRealTimeInfo";
            this.Text = "FormRealTimeInfo";
            this.SizeChanged += new System.EventHandler(this.FormRealTimeInfo_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormRealTimeInfo_Paint);
            this.Resize += new System.EventHandler(this.FormRealTimeInfo_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer m_TextScrollTimer;
    }
}