namespace UnE.SenarioMaker
{
    partial class FormTreeStep
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("SOP");
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.mStepTreeView = new UnE.Controls.TreeViewEx();
            this.senarioTreeToolStripMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.actionStepToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.deleteActionStepStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disasterTreeToolStripMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.typeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.newSubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.senarioTreeToolStripMenu.SuspendLayout();
            this.disasterTreeToolStripMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(282, 21);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(1, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "단계별 시나리오";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panel2.Controls.Add(this.mStepTreeView);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(282, 297);
            this.panel2.TabIndex = 1;
            // 
            // mStepTreeView
            // 
            this.mStepTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mStepTreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.mStepTreeView.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mStepTreeView.FullRowSelect = true;
            this.mStepTreeView.HideSelection = false;
            this.mStepTreeView.LabelEdit = true;
            this.mStepTreeView.Location = new System.Drawing.Point(0, 21);
            this.mStepTreeView.Margin = new System.Windows.Forms.Padding(0);
            this.mStepTreeView.Name = "mStepTreeView";
            treeNode1.Name = "노드0";
            treeNode1.Text = "SOP";
            this.mStepTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.mStepTreeView.ShowNodeToolTips = true;
            this.mStepTreeView.Size = new System.Drawing.Size(282, 276);
            this.mStepTreeView.TabIndex = 1;
            this.mStepTreeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.mStepTreeView_BeforeLabelEdit);
            this.mStepTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.mStepTreeView_AfterLabelEdit);
            this.mStepTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.mStepTreeView_BeforeSelect);
            this.mStepTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.mStepTreeView_AfterSelect);
            this.mStepTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.mStepTreeView_NodeMouseClick);
            this.mStepTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.mStepTreeView_NodeMouseDoubleClick);
            // 
            // senarioTreeToolStripMenu
            // 
            this.senarioTreeToolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.actionStepToolStripSeparator,
            this.deleteActionStepStripMenuItem});
            this.senarioTreeToolStripMenu.Name = "senarioTreeToolStripMenu";
            this.senarioTreeToolStripMenu.Size = new System.Drawing.Size(139, 54);
            this.senarioTreeToolStripMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.senarioTreeToolStripMenu_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem1.Text = "이름 바꾸기";
            // 
            // actionStepToolStripSeparator
            // 
            this.actionStepToolStripSeparator.Name = "actionStepToolStripSeparator";
            this.actionStepToolStripSeparator.Size = new System.Drawing.Size(135, 6);
            // 
            // deleteActionStepStripMenuItem
            // 
            this.deleteActionStepStripMenuItem.Name = "deleteActionStepStripMenuItem";
            this.deleteActionStepStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.deleteActionStepStripMenuItem.Text = "삭제";
            // 
            // disasterTreeToolStripMenu
            // 
            this.disasterTreeToolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.typeToolStripMenuItem,
            this.toolStripSeparator2,
            this.newSubToolStripMenuItem});
            this.disasterTreeToolStripMenu.Name = "senarioTreeToolStripMenu";
            this.disasterTreeToolStripMenu.Size = new System.Drawing.Size(151, 54);
            this.disasterTreeToolStripMenu.Opening += new System.ComponentModel.CancelEventHandler(this.disasterTreeToolStripMenu_Opening);
            this.disasterTreeToolStripMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.disasterTreeToolStripMenu_ItemClicked);
            // 
            // typeToolStripMenuItem
            // 
            this.typeToolStripMenuItem.Name = "typeToolStripMenuItem";
            this.typeToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.typeToolStripMenuItem.Text = "시나리오 타입";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(147, 6);
            this.toolStripSeparator2.Visible = false;
            // 
            // newSubToolStripMenuItem
            // 
            this.newSubToolStripMenuItem.Name = "newSubToolStripMenuItem";
            this.newSubToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.newSubToolStripMenuItem.Text = "새 함수 추가";
            this.newSubToolStripMenuItem.Visible = false;
            // 
            // FormTreeStep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.ClientSize = new System.Drawing.Size(282, 297);
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormTreeStep";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormTreeStep";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.senarioTreeToolStripMenu.ResumeLayout(false);
            this.disasterTreeToolStripMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private UnE.Controls.TreeViewEx mStepTreeView;
        private System.Windows.Forms.ContextMenuStrip senarioTreeToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator actionStepToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem deleteActionStepStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip disasterTreeToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem typeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem newSubToolStripMenuItem;
    }
}