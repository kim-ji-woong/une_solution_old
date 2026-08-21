namespace SOPGen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tabCtrlMain = new System.Windows.Forms.TabControl();
            this.tabPageProcess = new System.Windows.Forms.TabPage();
            this.axSkinFramework1 = new AxXtremeSkinFramework.AxSkinFramework();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.axDocking = new AxXtremeDockingPane.AxDockingPane();
            this.panelProcess = new System.Windows.Forms.Panel();
            this.tsMenu = new System.Windows.Forms.ToolStrip();
            this.tsBtnNewSOP = new System.Windows.Forms.ToolStripButton();
            this.tsBtnSave = new System.Windows.Forms.ToolStripButton();
            this.tsBtnLoad = new System.Windows.Forms.ToolStripButton();
            this.tsBtnProccessAdd = new System.Windows.Forms.ToolStripButton();
            this.tsBtnProccessDel = new System.Windows.Forms.ToolStripButton();
            this.tsBtnGroupAdd = new System.Windows.Forms.ToolStripButton();
            this.tsBtnGroupDel = new System.Windows.Forms.ToolStripButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabCtrlMain.SuspendLayout();
            this.tabPageProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework1)).BeginInit();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axDocking)).BeginInit();
            this.tsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabCtrlMain
            // 
            this.tabCtrlMain.Controls.Add(this.tabPageProcess);
            this.tabCtrlMain.Controls.Add(this.tabPage2);
            this.tabCtrlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrlMain.Location = new System.Drawing.Point(0, 0);
            this.tabCtrlMain.Name = "tabCtrlMain";
            this.tabCtrlMain.Padding = new System.Drawing.Point(0, 0);
            this.tabCtrlMain.SelectedIndex = 0;
            this.tabCtrlMain.Size = new System.Drawing.Size(1134, 726);
            this.tabCtrlMain.TabIndex = 0;
            // 
            // tabPageProcess
            // 
            this.tabPageProcess.Controls.Add(this.axSkinFramework1);
            this.tabPageProcess.Controls.Add(this.toolStripContainer1);
            this.tabPageProcess.Location = new System.Drawing.Point(4, 22);
            this.tabPageProcess.Name = "tabPageProcess";
            this.tabPageProcess.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProcess.Size = new System.Drawing.Size(1126, 700);
            this.tabPageProcess.TabIndex = 0;
            this.tabPageProcess.Text = "프로세스관리";
            this.tabPageProcess.UseVisualStyleBackColor = true;
            // 
            // axSkinFramework1
            // 
            this.axSkinFramework1.Enabled = true;
            this.axSkinFramework1.Location = new System.Drawing.Point(8, 8);
            this.axSkinFramework1.Name = "axSkinFramework1";
            this.axSkinFramework1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSkinFramework1.OcxState")));
            this.axSkinFramework1.Size = new System.Drawing.Size(24, 24);
            this.axSkinFramework1.TabIndex = 1;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.axDocking);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panelProcess);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1120, 669);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(3, 3);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1120, 694);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsMenu);
            // 
            // axDocking
            // 
            this.axDocking.Dock = System.Windows.Forms.DockStyle.Right;
            this.axDocking.Enabled = true;
            this.axDocking.Location = new System.Drawing.Point(1096, 0);
            this.axDocking.Name = "axDocking";
            this.axDocking.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axDocking.OcxState")));
            this.axDocking.Size = new System.Drawing.Size(24, 24);
            this.axDocking.TabIndex = 4;
            this.axDocking.AttachPaneEvent += new AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEventHandler(this.axDocking_AttachPaneEvent);
            this.axDocking.Action += new AxXtremeDockingPane._DDockingPaneEvents_ActionEventHandler(this.axDocking_Action);
            // 
            // panelProcess
            // 
            this.panelProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelProcess.Location = new System.Drawing.Point(0, 0);
            this.panelProcess.Name = "panelProcess";
            this.panelProcess.Size = new System.Drawing.Size(1120, 644);
            this.panelProcess.TabIndex = 0;
            // 
            // tsMenu
            // 
            this.tsMenu.Dock = System.Windows.Forms.DockStyle.None;
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtnNewSOP,
            this.tsBtnSave,
            this.tsBtnLoad,
            this.tsBtnProccessAdd,
            this.tsBtnProccessDel,
            this.tsBtnGroupAdd,
            this.tsBtnGroupDel});
            this.tsMenu.Location = new System.Drawing.Point(3, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(591, 25);
            this.tsMenu.TabIndex = 0;
            // 
            // tsBtnNewSOP
            // 
            this.tsBtnNewSOP.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnNewSOP.Image")));
            this.tsBtnNewSOP.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnNewSOP.Name = "tsBtnNewSOP";
            this.tsBtnNewSOP.Size = new System.Drawing.Size(66, 22);
            this.tsBtnNewSOP.Text = "새 SOP";
            this.tsBtnNewSOP.Click += new System.EventHandler(this.tsBtnNewSOP_Click);
            // 
            // tsBtnSave
            // 
            this.tsBtnSave.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnSave.Image")));
            this.tsBtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnSave.Name = "tsBtnSave";
            this.tsBtnSave.Size = new System.Drawing.Size(51, 22);
            this.tsBtnSave.Text = "저장";
            this.tsBtnSave.Click += new System.EventHandler(this.tsBtnSave_Click);
            // 
            // tsBtnLoad
            // 
            this.tsBtnLoad.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnLoad.Image")));
            this.tsBtnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnLoad.Name = "tsBtnLoad";
            this.tsBtnLoad.Size = new System.Drawing.Size(75, 22);
            this.tsBtnLoad.Text = "불러오기";
            this.tsBtnLoad.Click += new System.EventHandler(this.tsBtnLoad_Click);
            // 
            // tsBtnProccessAdd
            // 
            this.tsBtnProccessAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnProccessAdd.Image")));
            this.tsBtnProccessAdd.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsBtnProccessAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnProccessAdd.Name = "tsBtnProccessAdd";
            this.tsBtnProccessAdd.Size = new System.Drawing.Size(103, 22);
            this.tsBtnProccessAdd.Text = "프로세스 추가";
            this.tsBtnProccessAdd.Click += new System.EventHandler(this.OnBtnProcessAdd);
            // 
            // tsBtnProccessDel
            // 
            this.tsBtnProccessDel.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnProccessDel.Image")));
            this.tsBtnProccessDel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsBtnProccessDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnProccessDel.Name = "tsBtnProccessDel";
            this.tsBtnProccessDel.Size = new System.Drawing.Size(103, 22);
            this.tsBtnProccessDel.Text = "프로세스 삭제";
            this.tsBtnProccessDel.Click += new System.EventHandler(this.tsBtnProccessDel_Click);
            // 
            // tsBtnGroupAdd
            // 
            this.tsBtnGroupAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnGroupAdd.Image")));
            this.tsBtnGroupAdd.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsBtnGroupAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnGroupAdd.Name = "tsBtnGroupAdd";
            this.tsBtnGroupAdd.Size = new System.Drawing.Size(75, 22);
            this.tsBtnGroupAdd.Text = "조직등록";
            this.tsBtnGroupAdd.Click += new System.EventHandler(this.OnBtnGroupAdd);
            // 
            // tsBtnGroupDel
            // 
            this.tsBtnGroupDel.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnGroupDel.Image")));
            this.tsBtnGroupDel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsBtnGroupDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnGroupDel.Name = "tsBtnGroupDel";
            this.tsBtnGroupDel.Size = new System.Drawing.Size(75, 22);
            this.tsBtnGroupDel.Text = "조직삭제";
            this.tsBtnGroupDel.Click += new System.EventHandler(this.tsBtnGroupDel_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1126, 700);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 726);
            this.Controls.Add(this.tabCtrlMain);
            this.MinimumSize = new System.Drawing.Size(1070, 764);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "선진통합방재시스템";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabCtrlMain.ResumeLayout(false);
            this.tabPageProcess.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework1)).EndInit();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axDocking)).EndInit();
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabCtrlMain;
        private System.Windows.Forms.TabPage tabPageProcess;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Panel panelProcess;
        private AxXtremeDockingPane.AxDockingPane axDocking;
        private System.Windows.Forms.ToolStrip tsMenu;
        private System.Windows.Forms.ToolStripButton tsBtnSave;
        private System.Windows.Forms.ToolStripButton tsBtnLoad;
        private System.Windows.Forms.ToolStripButton tsBtnProccessAdd;
        private System.Windows.Forms.ToolStripButton tsBtnProccessDel;
        private System.Windows.Forms.ToolStripButton tsBtnGroupAdd;
        private System.Windows.Forms.ToolStripButton tsBtnGroupDel;
        private System.Windows.Forms.ToolStripButton tsBtnNewSOP;
        private AxXtremeSkinFramework.AxSkinFramework axSkinFramework1;
    }
}