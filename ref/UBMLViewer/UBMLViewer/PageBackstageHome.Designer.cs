namespace UBMLViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageBackstageHome));
            this.m_axDockingPane = new AxXtremeDockingPane.AxDockingPane();
            this.m_ToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.m_ContentPanel = new System.Windows.Forms.Panel();
            this.m_ToolStripOperator = new System.Windows.Forms.ToolStrip();
            this.m_ToolStrip3DView = new System.Windows.Forms.ToolStrip();
            ((System.ComponentModel.ISupportInitialize)(this.m_axDockingPane)).BeginInit();
            this.m_ToolStripContainer.ContentPanel.SuspendLayout();
            this.m_ToolStripContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_axDockingPane
            // 
            this.m_axDockingPane.Enabled = true;
            this.m_axDockingPane.Location = new System.Drawing.Point(13, 13);
            this.m_axDockingPane.Name = "m_axDockingPane";
            this.m_axDockingPane.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_axDockingPane.OcxState")));
            this.m_axDockingPane.Size = new System.Drawing.Size(24, 24);
            this.m_axDockingPane.TabIndex = 0;
            this.m_axDockingPane.AttachPaneEvent += new AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEventHandler(this.DockingPane_AttachPaneEvent);
            this.m_axDockingPane.ResizeEvent += new System.EventHandler(this.DockingPane_ResizeEvent);
            // 
            // m_ToolStripContainer
            // 
            // 
            // m_ToolStripContainer.BottomToolStripPanel
            // 
            this.m_ToolStripContainer.BottomToolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // m_ToolStripContainer.ContentPanel
            // 
            this.m_ToolStripContainer.ContentPanel.Controls.Add(this.m_ContentPanel);
            this.m_ToolStripContainer.ContentPanel.Size = new System.Drawing.Size(791, 405);
            // 
            // m_ToolStripContainer.LeftToolStripPanel
            // 
            this.m_ToolStripContainer.LeftToolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.m_ToolStripContainer.Location = new System.Drawing.Point(13, 132);
            this.m_ToolStripContainer.Name = "m_ToolStripContainer";
            // 
            // m_ToolStripContainer.RightToolStripPanel
            // 
            this.m_ToolStripContainer.RightToolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.m_ToolStripContainer.Size = new System.Drawing.Size(791, 430);
            this.m_ToolStripContainer.TabIndex = 2;
            this.m_ToolStripContainer.Text = "toolStripContainer1";
            // 
            // m_ToolStripContainer.TopToolStripPanel
            // 
            this.m_ToolStripContainer.TopToolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // m_ContentPanel
            // 
            this.m_ContentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.m_ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ContentPanel.Location = new System.Drawing.Point(0, 0);
            this.m_ContentPanel.Name = "m_ContentPanel";
            this.m_ContentPanel.Size = new System.Drawing.Size(791, 405);
            this.m_ContentPanel.TabIndex = 2;
            // 
            // m_ToolStripOperator
            // 
            this.m_ToolStripOperator.Dock = System.Windows.Forms.DockStyle.None;
            this.m_ToolStripOperator.Location = new System.Drawing.Point(3, 0);
            this.m_ToolStripOperator.Name = "m_ToolStripOperator";
            this.m_ToolStripOperator.Size = new System.Drawing.Size(111, 25);
            this.m_ToolStripOperator.TabIndex = 0;
            // 
            // m_ToolStrip3DView
            // 
            this.m_ToolStrip3DView.Dock = System.Windows.Forms.DockStyle.None;
            this.m_ToolStrip3DView.Location = new System.Drawing.Point(0, 3);
            this.m_ToolStrip3DView.Name = "m_ToolStrip3DView";
            this.m_ToolStrip3DView.Size = new System.Drawing.Size(26, 111);
            this.m_ToolStrip3DView.TabIndex = 0;
            // 
            // PageBackstageHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 900);
            this.Controls.Add(this.m_ToolStripContainer);
            this.Controls.Add(this.m_axDockingPane);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageHome";
            this.Text = "PageHomeView";
            ((System.ComponentModel.ISupportInitialize)(this.m_axDockingPane)).EndInit();
            this.m_ToolStripContainer.ContentPanel.ResumeLayout(false);
            this.m_ToolStripContainer.ResumeLayout(false);
            this.m_ToolStripContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremeDockingPane.AxDockingPane m_axDockingPane;
        private System.Windows.Forms.ToolStripContainer m_ToolStripContainer;
        private System.Windows.Forms.Panel m_ContentPanel;
        private System.Windows.Forms.ToolStrip m_ToolStripOperator;
        private System.Windows.Forms.ToolStrip m_ToolStrip3DView;
    }
}