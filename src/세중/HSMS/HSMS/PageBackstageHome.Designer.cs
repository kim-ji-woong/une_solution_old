namespace HSMS
{
    partial class PageBackstageHome
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.m_ContentPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // m_ContentPanel
            // 
            this.m_ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ContentPanel.Location = new System.Drawing.Point(0, 0);
            this.m_ContentPanel.Margin = new System.Windows.Forms.Padding(0);
            this.m_ContentPanel.Name = "m_ContentPanel";
            this.m_ContentPanel.Size = new System.Drawing.Size(998, 582);
            this.m_ContentPanel.TabIndex = 0;
            // 
            // PageBackstageHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(998, 582);
            this.Controls.Add(this.m_ContentPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageHome";
            this.Text = "PageBackstageHome";
            this.Load += new System.EventHandler(this.PageBackstageHome_Load);
            this.Resize += new System.EventHandler(this.PageBackstageHome_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel m_ContentPanel;
    }
}