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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageBackstageHome));
            this.axSkinFramework = new AxXtremeSkinFramework.AxSkinFramework();
            this.m_ContentPanel = new System.Windows.Forms.Panel();
            this.m_axDockingPane = new AxXtremeDockingPane.AxDockingPane();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_axDockingPane)).BeginInit();
            this.SuspendLayout();
            // 
            // axSkinFramework
            // 
            this.axSkinFramework.Enabled = true;
            this.axSkinFramework.Location = new System.Drawing.Point(12, 12);
            this.axSkinFramework.Name = "axSkinFramework";
            this.axSkinFramework.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSkinFramework.OcxState")));
            this.axSkinFramework.Size = new System.Drawing.Size(24, 24);
            this.axSkinFramework.TabIndex = 0;
            // 
            // m_ContentPanel
            // 
            this.m_ContentPanel.BackColor = System.Drawing.SystemColors.Control;
            this.m_ContentPanel.Location = new System.Drawing.Point(82, 159);
            this.m_ContentPanel.Name = "m_ContentPanel";
            this.m_ContentPanel.Size = new System.Drawing.Size(1280, 800);
            this.m_ContentPanel.TabIndex = 2;
            // 
            // m_axDockingPane
            // 
            this.m_axDockingPane.Enabled = true;
            this.m_axDockingPane.Location = new System.Drawing.Point(52, 12);
            this.m_axDockingPane.Name = "m_axDockingPane";
            this.m_axDockingPane.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_axDockingPane.OcxState")));
            this.m_axDockingPane.Size = new System.Drawing.Size(24, 24);
            this.m_axDockingPane.TabIndex = 1;
            this.m_axDockingPane.AttachPaneEvent += new AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEventHandler(this.DockingPane_AttachPaneEvent);
            this.m_axDockingPane.ResizeEvent += new System.EventHandler(this.DockingPane_ResizeEvent);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PageBackstageHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 900);
            this.Controls.Add(this.m_ContentPanel);
            this.Controls.Add(this.m_axDockingPane);
            this.Controls.Add(this.axSkinFramework);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageHome";
            this.Text = "PageHomeView";
            this.Load += new System.EventHandler(this.PageBackstageHome_Load);
            this.Resize += new System.EventHandler(this.PageBackstageHome_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_axDockingPane)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremeSkinFramework.AxSkinFramework axSkinFramework;
        private System.Windows.Forms.Panel m_ContentPanel;
        private AxXtremeDockingPane.AxDockingPane m_axDockingPane;
        private System.Windows.Forms.Timer timer1;
    }
}