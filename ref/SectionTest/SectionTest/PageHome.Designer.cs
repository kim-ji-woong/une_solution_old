namespace section
{
    partial class PageHome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageHome));
            this.m_TabControl = new System.Windows.Forms.TabControl();
            
            this.axDockingPane1 = new AxXtremeDockingPane.AxDockingPane();
            this.m_TabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane1)).BeginInit();
            this.SuspendLayout();
            // 
            // m_TabControl
            // 
            //this.m_TabControl.Controls.Add(this.tabPage1);
            this.m_TabControl.Location = new System.Drawing.Point(172, 62);
            this.m_TabControl.Name = "m_TabControl";
            this.m_TabControl.SelectedIndex = 0;
            this.m_TabControl.Size = new System.Drawing.Size(643, 416);
            this.m_TabControl.TabIndex = 15;
            // 
            // tabPage1
            // 
          
            // 
            // axDockingPane1
            // 
            this.axDockingPane1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.axDockingPane1.Enabled = true;
            this.axDockingPane1.Location = new System.Drawing.Point(46, 11);
            this.axDockingPane1.Name = "axDockingPane1";
            this.axDockingPane1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axDockingPane1.OcxState")));
            this.axDockingPane1.Size = new System.Drawing.Size(24, 24);
            this.axDockingPane1.TabIndex = 16;
            this.axDockingPane1.AttachPaneEvent += new AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEventHandler(this.DockingPane_AttachPaneEvent);
            this.axDockingPane1.ResizeEvent += new System.EventHandler(this.DockingPaneManager_ResizeEvent);
            // 
            // PageHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 588);
            this.Controls.Add(this.axDockingPane1);
            this.Controls.Add(this.m_TabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageHome";
            this.Text = "PageHome";
            this.Load += new System.EventHandler(this.PageHome_Load);
            this.Resize += new System.EventHandler(this.PageHome_Resize);
            this.m_TabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl m_TabControl;
       
        private AxXtremeDockingPane.AxDockingPane axDockingPane1;
    }
}