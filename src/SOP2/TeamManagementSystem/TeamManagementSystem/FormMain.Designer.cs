namespace TeamManagementSystem
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.axDockingPane = new AxXtremeDockingPane.AxDockingPane();
            this.panelOrganizational = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.btnScroll = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnScroll2 = new System.Windows.Forms.Button();
            this.btnScroll3 = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbtnRegular = new System.Windows.Forms.ToolStripButton();
            this.tsbtnEmergency = new System.Windows.Forms.ToolStripButton();
            this.tsbtnEmergency1 = new System.Windows.Forms.ToolStripButton();
            this.tsbtnBoth = new System.Windows.Forms.ToolStripButton();
            this.tsbtnTest = new System.Windows.Forms.ToolStripButton();
            this.tsbtn1 = new System.Windows.Forms.ToolStripButton();
            this.contextMenuNormal = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuTeamAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuTeamDel = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).BeginInit();
            this.panelOrganizational.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuNormal.SuspendLayout();
            this.SuspendLayout();
            // 
            // axDockingPane
            // 
            this.axDockingPane.Enabled = true;
            this.axDockingPane.Location = new System.Drawing.Point(0, 0);
            this.axDockingPane.Name = "axDockingPane";
            this.axDockingPane.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axDockingPane.OcxState")));
            this.axDockingPane.Size = new System.Drawing.Size(24, 24);
            this.axDockingPane.TabIndex = 0;
            this.axDockingPane.AttachPaneEvent += new AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEventHandler(this.axDockingPane_AttachPaneEvent);
            this.axDockingPane.ResizeEvent += new System.EventHandler(this.axDockingPane_ResizeEvent);
            // 
            // panelOrganizational
            // 
            this.panelOrganizational.BackColor = System.Drawing.SystemColors.Info;
            this.panelOrganizational.Controls.Add(this.splitContainer);
            this.panelOrganizational.Controls.Add(this.toolStrip1);
            this.panelOrganizational.Location = new System.Drawing.Point(203, 12);
            this.panelOrganizational.Name = "panelOrganizational";
            this.panelOrganizational.Size = new System.Drawing.Size(600, 300);
            this.panelOrganizational.TabIndex = 1;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 25);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.AllowDrop = true;
            this.splitContainer.Panel1.AutoScroll = true;
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.splitContainer.Panel1.Controls.Add(this.btnScroll);
            this.splitContainer.Panel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.splitContainer_Panel1_Scroll);
            this.splitContainer.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer_Panel1_Paint);
            this.splitContainer.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_Panel1_MouseDown);
            this.splitContainer.Panel1MinSize = 0;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.AutoScroll = true;
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.splitContainer.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer.Panel2MinSize = 0;
            this.splitContainer.Size = new System.Drawing.Size(600, 275);
            this.splitContainer.SplitterDistance = 297;
            this.splitContainer.SplitterWidth = 1;
            this.splitContainer.TabIndex = 5;
            // 
            // btnScroll
            // 
            this.btnScroll.Location = new System.Drawing.Point(-100, 0);
            this.btnScroll.Name = "btnScroll";
            this.btnScroll.Size = new System.Drawing.Size(28, 23);
            this.btnScroll.TabIndex = 0;
            this.btnScroll.Text = "NotUse";
            this.btnScroll.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AllowDrop = true;
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.splitContainer1.Panel1.Controls.Add(this.btnScroll2);
            this.splitContainer1.Panel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.splitContainer1_Panel1_Scroll);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            this.splitContainer1.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel1_MouseDown);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AllowDrop = true;
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.splitContainer1.Panel2.Controls.Add(this.btnScroll3);
            this.splitContainer1.Panel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.splitContainer1_Panel2_Scroll);
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.Panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel2_MouseDown);
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Size = new System.Drawing.Size(302, 275);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 2;
            // 
            // btnScroll2
            // 
            this.btnScroll2.Location = new System.Drawing.Point(-100, 0);
            this.btnScroll2.Name = "btnScroll2";
            this.btnScroll2.Size = new System.Drawing.Size(28, 23);
            this.btnScroll2.TabIndex = 0;
            this.btnScroll2.Text = "NotUse";
            this.btnScroll2.UseVisualStyleBackColor = true;
            // 
            // btnScroll3
            // 
            this.btnScroll3.Location = new System.Drawing.Point(-100, 0);
            this.btnScroll3.Name = "btnScroll3";
            this.btnScroll3.Size = new System.Drawing.Size(28, 23);
            this.btnScroll3.TabIndex = 0;
            this.btnScroll3.Text = "NotUse";
            this.btnScroll3.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnRegular,
            this.tsbtnEmergency,
            this.tsbtnEmergency1,
            this.tsbtnBoth,
            this.tsbtnTest,
            this.tsbtn1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(600, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbtnRegular
            // 
            this.tsbtnRegular.Checked = true;
            this.tsbtnRegular.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbtnRegular.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnRegular.Image")));
            this.tsbtnRegular.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnRegular.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnRegular.Name = "tsbtnRegular";
            this.tsbtnRegular.Size = new System.Drawing.Size(87, 22);
            this.tsbtnRegular.Text = "상시조직도";
            this.tsbtnRegular.Click += new System.EventHandler(this.tsbtnRegular_Click);
            // 
            // tsbtnEmergency
            // 
            this.tsbtnEmergency.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnEmergency.Image")));
            this.tsbtnEmergency.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnEmergency.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnEmergency.Name = "tsbtnEmergency";
            this.tsbtnEmergency.Size = new System.Drawing.Size(111, 22);
            this.tsbtnEmergency.Text = "평일비상조직도";
            this.tsbtnEmergency.Click += new System.EventHandler(this.tsbtnEmergency_Click);
            // 
            // tsbtnEmergency1
            // 
            this.tsbtnEmergency1.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnEmergency1.Image")));
            this.tsbtnEmergency1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnEmergency1.Name = "tsbtnEmergency1";
            this.tsbtnEmergency1.Size = new System.Drawing.Size(111, 22);
            this.tsbtnEmergency1.Text = "휴일비상조직도";
            this.tsbtnEmergency1.Click += new System.EventHandler(this.tsbtnEmergency1_Click);
            // 
            // tsbtnBoth
            // 
            this.tsbtnBoth.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnBoth.Image")));
            this.tsbtnBoth.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnBoth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnBoth.Name = "tsbtnBoth";
            this.tsbtnBoth.Size = new System.Drawing.Size(121, 22);
            this.tsbtnBoth.Text = "상시&&비상조직도";
            this.tsbtnBoth.Click += new System.EventHandler(this.tsbtnBoth_Click);
            // 
            // tsbtnTest
            // 
            this.tsbtnTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnTest.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnTest.Image")));
            this.tsbtnTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnTest.Name = "tsbtnTest";
            this.tsbtnTest.Size = new System.Drawing.Size(23, 22);
            this.tsbtnTest.Text = "tsbtnTest";
            this.tsbtnTest.Click += new System.EventHandler(this.tsbtnTest_Click);
            // 
            // tsbtn1
            // 
            this.tsbtn1.Image = ((System.Drawing.Image)(resources.GetObject("tsbtn1.Image")));
            this.tsbtn1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtn1.Name = "tsbtn1";
            this.tsbtn1.Size = new System.Drawing.Size(145, 20);
            this.tsbtn1.Text = "평일&&휴일비상조직도";
            this.tsbtn1.Click += new System.EventHandler(this.tsbtn1_Click);
            // 
            // contextMenuNormal
            // 
            this.contextMenuNormal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuTeamAdd,
            this.tsMenuTeamDel});
            this.contextMenuNormal.Name = "contextMenuNormal";
            this.contextMenuNormal.Size = new System.Drawing.Size(127, 48);
            // 
            // tsMenuTeamAdd
            // 
            this.tsMenuTeamAdd.Name = "tsMenuTeamAdd";
            this.tsMenuTeamAdd.Size = new System.Drawing.Size(126, 22);
            this.tsMenuTeamAdd.Text = "조직 추가";
            this.tsMenuTeamAdd.Click += new System.EventHandler(this.tsMenuAdd_Click);
            // 
            // tsMenuTeamDel
            // 
            this.tsMenuTeamDel.Name = "tsMenuTeamDel";
            this.tsMenuTeamDel.Size = new System.Drawing.Size(126, 22);
            this.tsMenuTeamDel.Text = "조직 삭제";
            this.tsMenuTeamDel.Click += new System.EventHandler(this.tsMenuTeamDel_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 730);
            this.Controls.Add(this.panelOrganizational);
            this.Controls.Add(this.axDockingPane);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.Text = "조직관리 시스템";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).EndInit();
            this.panelOrganizational.ResumeLayout(false);
            this.panelOrganizational.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuNormal.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremeDockingPane.AxDockingPane axDockingPane;
        private System.Windows.Forms.Panel panelOrganizational;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ContextMenuStrip contextMenuNormal;
        private System.Windows.Forms.ToolStripMenuItem tsMenuTeamAdd;
        private System.Windows.Forms.ToolStripMenuItem tsMenuTeamDel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbtnRegular;
        private System.Windows.Forms.ToolStripButton tsbtnEmergency;
        private System.Windows.Forms.ToolStripButton tsbtnBoth;
        private System.Windows.Forms.ToolStripButton tsbtnTest;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton tsbtnEmergency1;
        private System.Windows.Forms.ToolStripButton tsbtn1;
        private System.Windows.Forms.Button btnScroll;
        private System.Windows.Forms.Button btnScroll2;
        private System.Windows.Forms.Button btnScroll3;
    }
}