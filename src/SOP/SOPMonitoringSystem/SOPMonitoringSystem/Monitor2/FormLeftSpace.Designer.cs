namespace SOPDisasterSystem
{
    partial class FormLeftSpace
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
            this.contextSpaceTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.재난위치선택ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.재난위치해지ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.건물정보보기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.실내공간가시화ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeSpace = new System.Windows.Forms.TreeView();
            this.treeViewSearch = new System.Windows.Forms.TreeView();
            this.tsTextSearch = new System.Windows.Forms.ToolStripTextBox();
            this.tsbtnSearch = new System.Windows.Forms.ToolStripButton();
            this.tsbtnTree = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.contextSpaceTree.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextSpaceTree
            // 
            this.contextSpaceTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.재난위치선택ToolStripMenuItem,
            this.재난위치해지ToolStripMenuItem,
            this.건물정보보기ToolStripMenuItem,
            this.실내공간가시화ToolStripMenuItem});
            this.contextSpaceTree.Name = "contextSpaceTree";
            this.contextSpaceTree.Size = new System.Drawing.Size(163, 92);
            // 
            // 재난위치선택ToolStripMenuItem
            // 
            this.재난위치선택ToolStripMenuItem.Name = "재난위치선택ToolStripMenuItem";
            this.재난위치선택ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.재난위치선택ToolStripMenuItem.Text = "재난 위치 선택";
            // 
            // 재난위치해지ToolStripMenuItem
            // 
            this.재난위치해지ToolStripMenuItem.Name = "재난위치해지ToolStripMenuItem";
            this.재난위치해지ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.재난위치해지ToolStripMenuItem.Text = "재난 위치 해지";
            // 
            // 건물정보보기ToolStripMenuItem
            // 
            this.건물정보보기ToolStripMenuItem.Name = "건물정보보기ToolStripMenuItem";
            this.건물정보보기ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.건물정보보기ToolStripMenuItem.Text = "건물 정보 보기";
            this.건물정보보기ToolStripMenuItem.Visible = false;
            // 
            // 실내공간가시화ToolStripMenuItem
            // 
            this.실내공간가시화ToolStripMenuItem.Name = "실내공간가시화ToolStripMenuItem";
            this.실내공간가시화ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.실내공간가시화ToolStripMenuItem.Text = "실내공간 가시화";
            // 
            // treeSpace
            // 
            this.treeSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeSpace.Location = new System.Drawing.Point(6, 34);
            this.treeSpace.Name = "treeSpace";
            this.treeSpace.Size = new System.Drawing.Size(250, 222);
            this.treeSpace.TabIndex = 0;
            this.treeSpace.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeSpace_BeforeSelect);
            this.treeSpace.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSpace_AfterSelect);
            this.treeSpace.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeSpace_MouseDown);
            // 
            // treeViewSearch
            // 
            this.treeViewSearch.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.treeViewSearch.HideSelection = false;
            this.treeViewSearch.Location = new System.Drawing.Point(6, 34);
            this.treeViewSearch.Name = "treeViewSearch";
            this.treeViewSearch.ShowLines = false;
            this.treeViewSearch.ShowPlusMinus = false;
            this.treeViewSearch.ShowRootLines = false;
            this.treeViewSearch.Size = new System.Drawing.Size(236, 222);
            this.treeViewSearch.TabIndex = 3;
            this.treeViewSearch.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewSearch_BeforeSelect);
            this.treeViewSearch.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSearch_AfterSelect);
            // 
            // tsTextSearch
            // 
            this.tsTextSearch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tsTextSearch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tsTextSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tsTextSearch.Margin = new System.Windows.Forms.Padding(6, 4, 1, 0);
            this.tsTextSearch.Name = "tsTextSearch";
            this.tsTextSearch.Size = new System.Drawing.Size(150, 23);
            this.tsTextSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tsTextSearch_KeyDown);
            // 
            // tsbtnSearch
            // 
            this.tsbtnSearch.AutoToolTip = false;
            this.tsbtnSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnSearch.Image = global::SOPMonitoringSystem.Properties.Resources.btn_search;
            this.tsbtnSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnSearch.Name = "tsbtnSearch";
            this.tsbtnSearch.Size = new System.Drawing.Size(28, 28);
            this.tsbtnSearch.Text = "toolStripButton1";
            this.tsbtnSearch.Click += new System.EventHandler(this.tsbtnSearch_Click);
            // 
            // tsbtnTree
            // 
            this.tsbtnTree.AutoToolTip = false;
            this.tsbtnTree.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnTree.Image = global::SOPMonitoringSystem.Properties.Resources.btn_tree;
            this.tsbtnTree.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnTree.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnTree.Name = "tsbtnTree";
            this.tsbtnTree.Size = new System.Drawing.Size(28, 28);
            this.tsbtnTree.Text = "toolStripButton2";
            this.tsbtnTree.Click += new System.EventHandler(this.tsbtnTree_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsTextSearch,
            this.tsbtnSearch,
            this.tsbtnTree});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 3, 1, 0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(262, 34);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // FormLeftSpace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(0, 5);
            this.ClientSize = new System.Drawing.Size(262, 262);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.treeViewSearch);
            this.Controls.Add(this.treeSpace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLeftSpace";
            this.Text = "공간구조";
            this.contextSpaceTree.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextSpaceTree;
        private System.Windows.Forms.ToolStripMenuItem 재난위치선택ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 재난위치해지ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 건물정보보기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 실내공간가시화ToolStripMenuItem;
        private System.Windows.Forms.TreeView treeSpace;
        private System.Windows.Forms.TreeView treeViewSearch;
        private System.Windows.Forms.ToolStripTextBox tsTextSearch;
        private System.Windows.Forms.ToolStripButton tsbtnSearch;
        private System.Windows.Forms.ToolStripButton tsbtnTree;
        private System.Windows.Forms.ToolStrip toolStrip1;
    }
}