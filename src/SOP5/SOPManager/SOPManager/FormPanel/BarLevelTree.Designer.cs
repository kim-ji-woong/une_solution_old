namespace SOPManager
{
    partial class BarLevelTree
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
            this.treeView = new System.Windows.Forms.TreeView();
            this.panelTop = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.treeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.subCategoryContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.disasterContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeDisasterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addLevelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.levelContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeLevelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leveMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.leveMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.leveMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.leveMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteLevelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTop.SuspendLayout();
            this.treeContextMenu.SuspendLayout();
            this.subCategoryContextMenu.SuspendLayout();
            this.disasterContextMenu.SuspendLayout();
            this.levelContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.BackColor = System.Drawing.Color.White;
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeView.Location = new System.Drawing.Point(0, 24);
            this.treeView.Margin = new System.Windows.Forms.Padding(0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(284, 238);
            this.treeView.TabIndex = 0;
            this.treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeSelect);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseUp);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panelTop.BackgroundImage = global::SOPManager.Properties.Resources.panelTitle;
            this.panelTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTop.Controls.Add(this.label2);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(284, 24);
            this.panelTop.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font(Program.prgFont, 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(196, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 17);
            this.label2.TabIndex = 1;
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            this.label2.DoubleClick += new System.EventHandler(this.label2_DoubleClick);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "  SOP 단계";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // treeContextMenu
            // 
            this.treeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.treeContextMenu.Name = "treeContextMenu";
            this.treeContextMenu.Size = new System.Drawing.Size(151, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(150, 22);
            this.toolStripMenuItem1.Text = "재난종류 변경";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // subCategoryContextMenu
            // 
            this.subCategoryContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
            this.subCategoryContextMenu.Name = "treeContextMenu";
            this.subCategoryContextMenu.Size = new System.Drawing.Size(151, 26);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(150, 22);
            this.toolStripMenuItem2.Text = "재난종류 변경";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // disasterContextMenu
            // 
            this.disasterContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeDisasterMenuItem,
            this.toolStripSeparator2,
            this.addLevelMenuItem});
            this.disasterContextMenu.Name = "disasterContextMenu";
            this.disasterContextMenu.Size = new System.Drawing.Size(151, 54);
            // 
            // changeDisasterMenuItem
            // 
            this.changeDisasterMenuItem.Name = "changeDisasterMenuItem";
            this.changeDisasterMenuItem.Size = new System.Drawing.Size(150, 22);
            this.changeDisasterMenuItem.Text = "재난종류 변경";
            this.changeDisasterMenuItem.Click += new System.EventHandler(this.changeDisasterMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(147, 6);
            // 
            // addLevelMenuItem
            // 
            this.addLevelMenuItem.Name = "addLevelMenuItem";
            this.addLevelMenuItem.Size = new System.Drawing.Size(150, 22);
            this.addLevelMenuItem.Text = "단계 추가";
            this.addLevelMenuItem.Click += new System.EventHandler(this.addLevelMenuItem_Click);
            // 
            // levelContextMenu
            // 
            this.levelContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeLevelMenuItem,
            this.toolStripSeparator1,
            this.deleteLevelMenuItem});
            this.levelContextMenu.Name = "contextMenuStrip1";
            this.levelContextMenu.Size = new System.Drawing.Size(127, 54);
            this.levelContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.levelContextMenu_Opening);
            // 
            // changeLevelMenuItem
            // 
            this.changeLevelMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.leveMenuItem1,
            this.leveMenuItem2,
            this.leveMenuItem3,
            this.leveMenuItem4});
            this.changeLevelMenuItem.Name = "changeLevelMenuItem";
            this.changeLevelMenuItem.Size = new System.Drawing.Size(126, 22);
            this.changeLevelMenuItem.Text = "단계 변경";
            this.changeLevelMenuItem.Visible = false;
            // 
            // leveMenuItem1
            // 
            this.leveMenuItem1.Name = "leveMenuItem1";
            this.leveMenuItem1.Size = new System.Drawing.Size(98, 22);
            this.leveMenuItem1.Text = "예방";
            // 
            // leveMenuItem2
            // 
            this.leveMenuItem2.Name = "leveMenuItem2";
            this.leveMenuItem2.Size = new System.Drawing.Size(98, 22);
            this.leveMenuItem2.Text = "대비";
            // 
            // leveMenuItem3
            // 
            this.leveMenuItem3.Name = "leveMenuItem3";
            this.leveMenuItem3.Size = new System.Drawing.Size(98, 22);
            this.leveMenuItem3.Text = "대응";
            // 
            // leveMenuItem4
            // 
            this.leveMenuItem4.Name = "leveMenuItem4";
            this.leveMenuItem4.Size = new System.Drawing.Size(98, 22);
            this.leveMenuItem4.Text = "복구";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(123, 6);
            this.toolStripSeparator1.Visible = false;
            // 
            // deleteLevelMenuItem
            // 
            this.deleteLevelMenuItem.Name = "deleteLevelMenuItem";
            this.deleteLevelMenuItem.Size = new System.Drawing.Size(126, 22);
            this.deleteLevelMenuItem.Text = "단계 삭제";
            this.deleteLevelMenuItem.Click += new System.EventHandler(this.deleteLevelMenuItem_Click);
            // 
            // BarLevelTree
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.panelTop);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BarLevelTree";
            this.Text = "단계 Tree";
            this.panelTop.ResumeLayout(false);
            this.treeContextMenu.ResumeLayout(false);
            this.subCategoryContextMenu.ResumeLayout(false);
            this.disasterContextMenu.ResumeLayout(false);
            this.levelContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ContextMenuStrip treeContextMenu;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ContextMenuStrip subCategoryContextMenu;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ContextMenuStrip disasterContextMenu;
		private System.Windows.Forms.ToolStripMenuItem changeDisasterMenuItem;
		private System.Windows.Forms.ContextMenuStrip levelContextMenu;
		private System.Windows.Forms.ToolStripMenuItem changeLevelMenuItem;
		private System.Windows.Forms.ToolStripMenuItem leveMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem leveMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem leveMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem leveMenuItem4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem deleteLevelMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem addLevelMenuItem;
        private System.Windows.Forms.Label label2;
    }
}