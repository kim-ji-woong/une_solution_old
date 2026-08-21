namespace SDMS
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
            SaveDockingPane(mCurrentTab);

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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.m_ContentPanel = new System.Windows.Forms.Panel();
            this.m_PanelLeft = new System.Windows.Forms.Panel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.m_PanelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.m_ContentPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.m_PanelLeft);
            this.splitContainer1.Size = new System.Drawing.Size(1280, 900);
            this.splitContainer1.SplitterDistance = 1000;
            this.splitContainer1.TabIndex = 4;
            // 
            // m_ContentPanel
            // 
            this.m_ContentPanel.BackColor = System.Drawing.SystemColors.Control;
            this.m_ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ContentPanel.Location = new System.Drawing.Point(0, 0);
            this.m_ContentPanel.Name = "m_ContentPanel";
            this.m_ContentPanel.Size = new System.Drawing.Size(1000, 900);
            this.m_ContentPanel.TabIndex = 3;
            // 
            // m_PanelLeft
            // 
            this.m_PanelLeft.Controls.Add(this.splitContainer2);
            this.m_PanelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_PanelLeft.Location = new System.Drawing.Point(0, 0);
            this.m_PanelLeft.Name = "m_PanelLeft";
            this.m_PanelLeft.Size = new System.Drawing.Size(276, 900);
            this.m_PanelLeft.TabIndex = 4;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer2.Size = new System.Drawing.Size(276, 900);
            this.splitContainer2.SplitterDistance = 200;
            this.splitContainer2.TabIndex = 0;
            // 
            // PageBackstageHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 900);
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageHome";
            this.Text = "PageHomeView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PageBackstageHome_FormClosing);
            this.Load += new System.EventHandler(this.PageBackstageHome_Load);
            this.Resize += new System.EventHandler(this.PageBackstageHome_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.m_PanelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel m_ContentPanel;
		private System.Windows.Forms.Panel m_PanelLeft;
		private System.Windows.Forms.SplitContainer splitContainer2;
    }
}