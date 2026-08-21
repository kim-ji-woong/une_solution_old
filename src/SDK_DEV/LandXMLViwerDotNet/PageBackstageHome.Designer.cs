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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageBackstageHome));
            this.m_axDockingPane = new AxXtremeDockingPane.AxDockingPane();
            this.m_3DView = new System.Windows.Forms.Label();
            this.m_popupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.textureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hiddenLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_ContentPane = new System.Windows.Forms.Panel();
            this.m_SelectNodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.m_axDockingPane)).BeginInit();
            this.m_popupMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_axDockingPane
            // 
            this.m_axDockingPane.Enabled = true;
            this.m_axDockingPane.Location = new System.Drawing.Point(21, 97);
            this.m_axDockingPane.Name = "m_axDockingPane";
            this.m_axDockingPane.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_axDockingPane.OcxState")));
            this.m_axDockingPane.Size = new System.Drawing.Size(24, 24);
            this.m_axDockingPane.TabIndex = 0;
            this.m_axDockingPane.AttachPaneEvent += new AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEventHandler(this.DockingPane_AttachPaneEvent);
            this.m_axDockingPane.ResizeEvent += new System.EventHandler(this.DockingPane_ResizeEvent);
            // 
            // m_3DView
            // 
            this.m_3DView.Location = new System.Drawing.Point(0, 0);
            this.m_3DView.Name = "m_3DView";
            this.m_3DView.Size = new System.Drawing.Size(100, 23);
            this.m_3DView.TabIndex = 0;
            // 
            // m_popupMenu
            // 
            this.m_popupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectToolStripMenuItem,
            this.m_SelectNodeMenuItem,
            this.clearSelectToolStripMenuItem,
            this.toolStripSeparator1,
            this.textureToolStripMenuItem,
            this.hiddenLineToolStripMenuItem,
            this.wireFrameToolStripMenuItem,
            this.shadingToolStripMenuItem});
            this.m_popupMenu.Name = "m_popupMenu";
            this.m_popupMenu.Size = new System.Drawing.Size(153, 186);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.selectToolStripMenuItem.Text = "Select";
            this.selectToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // clearSelectToolStripMenuItem
            // 
            this.clearSelectToolStripMenuItem.Name = "clearSelectToolStripMenuItem";
            this.clearSelectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clearSelectToolStripMenuItem.Text = "Clear Select";
            this.clearSelectToolStripMenuItem.Click += new System.EventHandler(this.clearSelectToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // textureToolStripMenuItem
            // 
            this.textureToolStripMenuItem.Name = "textureToolStripMenuItem";
            this.textureToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.textureToolStripMenuItem.Text = "Texture";
            this.textureToolStripMenuItem.Click += new System.EventHandler(this.textureToolStripMenuItem_Click);
            // 
            // hiddenLineToolStripMenuItem
            // 
            this.hiddenLineToolStripMenuItem.Name = "hiddenLineToolStripMenuItem";
            this.hiddenLineToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hiddenLineToolStripMenuItem.Text = "Hidden Line";
            this.hiddenLineToolStripMenuItem.Click += new System.EventHandler(this.hiddenLineToolStripMenuItem_Click);
            // 
            // wireFrameToolStripMenuItem
            // 
            this.wireFrameToolStripMenuItem.Name = "wireFrameToolStripMenuItem";
            this.wireFrameToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.wireFrameToolStripMenuItem.Text = "Wire Frame";
            this.wireFrameToolStripMenuItem.Click += new System.EventHandler(this.wireFrameToolStripMenuItem_Click);
            // 
            // shadingToolStripMenuItem
            // 
            this.shadingToolStripMenuItem.Name = "shadingToolStripMenuItem";
            this.shadingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.shadingToolStripMenuItem.Text = "Shading";
            this.shadingToolStripMenuItem.Click += new System.EventHandler(this.shadingToolStripMenuItem_Click);
            // 
            // m_ContentPane
            // 
            this.m_ContentPane.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.m_ContentPane.Location = new System.Drawing.Point(147, 173);
            this.m_ContentPane.Name = "m_ContentPane";
            this.m_ContentPane.Size = new System.Drawing.Size(542, 270);
            this.m_ContentPane.TabIndex = 1;
            // 
            // m_SelectNodeMenuItem
            // 
            this.m_SelectNodeMenuItem.Name = "m_SelectNodeMenuItem";
            this.m_SelectNodeMenuItem.Size = new System.Drawing.Size(152, 22);
            this.m_SelectNodeMenuItem.Text = "Select Node";
            this.m_SelectNodeMenuItem.Click += new System.EventHandler(this.selectNodeMenuItem_Click);
            // 
            // PageBackstageHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 900);
            this.Controls.Add(this.m_ContentPane);
            this.Controls.Add(this.m_axDockingPane);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageHome";
            this.Text = "PageHomeView";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PageBackstageHome_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.m_axDockingPane)).EndInit();
            this.m_popupMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremeDockingPane.AxDockingPane m_axDockingPane;
        private System.Windows.Forms.ContextMenuStrip m_popupMenu;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSelectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem textureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hiddenLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wireFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shadingToolStripMenuItem;
        private System.Windows.Forms.Panel m_ContentPane;
        private System.Windows.Forms.ToolStripMenuItem m_SelectNodeMenuItem;

        

    }
}