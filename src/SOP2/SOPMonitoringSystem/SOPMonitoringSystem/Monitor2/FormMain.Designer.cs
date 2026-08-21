namespace SOPDisasterSystem
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
			this.tabCtrlMonitoring = new System.Windows.Forms.TabControl();
			this.tabDisaster = new System.Windows.Forms.TabPage();
			this.panelMain = new System.Windows.Forms.Panel();
			this.axSkinFramework1 = new AxXtremeSkinFramework.AxSkinFramework();
			this.axDockingPane = new AxXtremeDockingPane.AxDockingPane();
			this.panelVirtool = new System.Windows.Forms.Panel();
			this.imageListViewCtrl = new System.Windows.Forms.ImageList(this.components);
			this.tsViewCtrl = new System.Windows.Forms.ToolStrip();
			this.tsbtnHomeView = new System.Windows.Forms.ToolStripButton();
			this.tsbtnFullScreen = new System.Windows.Forms.ToolStripButton();
			this.tsbtnZoomin = new System.Windows.Forms.ToolStripButton();
			this.tsbtnZoomout = new System.Windows.Forms.ToolStripButton();
			this.tsbtnMove = new System.Windows.Forms.ToolStripButton();
			this.tsbtnPick = new System.Windows.Forms.ToolStripButton();
			this.tsbtnOrbit = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsbtnLayout1 = new System.Windows.Forms.ToolStripButton();
			this.tsbtnLayout2 = new System.Windows.Forms.ToolStripButton();
			this.tsbtnLayout3 = new System.Windows.Forms.ToolStripButton();
			this.tsbtnLayout4 = new System.Windows.Forms.ToolStripButton();
			this.tsFloor = new System.Windows.Forms.ToolStrip();
			this.mCmbFloor = new System.Windows.Forms.ToolStripComboBox();
			this.tsbtnLeft = new System.Windows.Forms.ToolStripButton();
			this.tsbtnFloor1 = new System.Windows.Forms.ToolStripButton();
			this.tsbtnFloor2 = new System.Windows.Forms.ToolStripButton();
			this.tsbtnFloor3 = new System.Windows.Forms.ToolStripButton();
			this.tsbtnFloor4 = new System.Windows.Forms.ToolStripButton();
			this.tsbtnFloor5 = new System.Windows.Forms.ToolStripButton();
			this.tsbtnRight = new System.Windows.Forms.ToolStripButton();
			this.tsSetting = new System.Windows.Forms.ToolStrip();
			this.tsbtnSetting = new System.Windows.Forms.ToolStripButton();
			this.tsbtnAutoNavi = new System.Windows.Forms.ToolStripButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tabCtrlMonitoring.SuspendLayout();
			this.tabDisaster.SuspendLayout();
			this.panelMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.axSkinFramework1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).BeginInit();
			this.tsViewCtrl.SuspendLayout();
			this.tsFloor.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabCtrlMonitoring
			// 
			this.tabCtrlMonitoring.Controls.Add(this.tabDisaster);
			this.tabCtrlMonitoring.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabCtrlMonitoring.Location = new System.Drawing.Point(0, 0);
			this.tabCtrlMonitoring.Name = "tabCtrlMonitoring";
			this.tabCtrlMonitoring.SelectedIndex = 0;
			this.tabCtrlMonitoring.Size = new System.Drawing.Size(1134, 726);
			this.tabCtrlMonitoring.TabIndex = 0;
			this.tabCtrlMonitoring.SelectedIndexChanged += new System.EventHandler(this.tabCtrlMonitoring_SelectedIndexChanged);
			// 
			// tabDisaster
			// 
			this.tabDisaster.Controls.Add(this.panelMain);
			this.tabDisaster.Location = new System.Drawing.Point(4, 22);
			this.tabDisaster.Name = "tabDisaster";
			this.tabDisaster.Padding = new System.Windows.Forms.Padding(3);
			this.tabDisaster.Size = new System.Drawing.Size(1126, 700);
			this.tabDisaster.TabIndex = 0;
			this.tabDisaster.Text = "통합재난관리";
			this.tabDisaster.UseVisualStyleBackColor = true;
			// 
			// panelMain
			// 
			this.panelMain.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.panelMain.Controls.Add(this.axSkinFramework1);
			this.panelMain.Controls.Add(this.axDockingPane);
			this.panelMain.Controls.Add(this.panelVirtool);
			this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMain.Location = new System.Drawing.Point(3, 3);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(1120, 694);
			this.panelMain.TabIndex = 0;
			// 
			// axSkinFramework1
			// 
			this.axSkinFramework1.Enabled = true;
			this.axSkinFramework1.Location = new System.Drawing.Point(5, 30);
			this.axSkinFramework1.Name = "axSkinFramework1";
			this.axSkinFramework1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSkinFramework1.OcxState")));
			this.axSkinFramework1.Size = new System.Drawing.Size(24, 24);
			this.axSkinFramework1.TabIndex = 2;
			// 
			// axDockingPane
			// 
			this.axDockingPane.Dock = System.Windows.Forms.DockStyle.Left;
			this.axDockingPane.Enabled = true;
			this.axDockingPane.Location = new System.Drawing.Point(0, 0);
			this.axDockingPane.Name = "axDockingPane";
			this.axDockingPane.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axDockingPane.OcxState")));
			this.axDockingPane.Size = new System.Drawing.Size(24, 24);
			this.axDockingPane.TabIndex = 0;
			this.axDockingPane.AttachPaneEvent += new AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEventHandler(this.axDockingPane_AttachPaneEvent);
			this.axDockingPane.ResizeEvent += new System.EventHandler(this.axDockingPane_ResizeEvent);
			// 
			// panelVirtool
			// 
			this.panelVirtool.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelVirtool.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.panelVirtool.Location = new System.Drawing.Point(240, 0);
			this.panelVirtool.Name = "panelVirtool";
			this.panelVirtool.Size = new System.Drawing.Size(620, 416);
			this.panelVirtool.TabIndex = 1;
			// 
			// imageListViewCtrl
			// 
			this.imageListViewCtrl.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListViewCtrl.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListViewCtrl.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tsViewCtrl
			// 
			this.tsViewCtrl.Dock = System.Windows.Forms.DockStyle.None;
			this.tsViewCtrl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnHomeView,
            this.tsbtnFullScreen,
            this.tsbtnZoomin,
            this.tsbtnZoomout,
            this.tsbtnMove,
            this.tsbtnPick,
            this.tsbtnOrbit,
            this.toolStripSeparator1,
            this.tsbtnLayout1,
            this.tsbtnLayout2,
            this.tsbtnLayout3,
            this.tsbtnLayout4});
			this.tsViewCtrl.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.tsViewCtrl.Location = new System.Drawing.Point(0, 2);
			this.tsViewCtrl.Name = "tsViewCtrl";
			this.tsViewCtrl.Size = new System.Drawing.Size(225, 25);
			this.tsViewCtrl.TabIndex = 1;
			this.tsViewCtrl.Text = "toolStrip1";
			// 
			// tsbtnHomeView
			// 
			this.tsbtnHomeView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnHomeView.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnHomeView.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnHomeView.Name = "tsbtnHomeView";
			this.tsbtnHomeView.Size = new System.Drawing.Size(23, 22);
			this.tsbtnHomeView.Text = "홈뷰";
			this.tsbtnHomeView.Click += new System.EventHandler(this.tsbtnHomeView_Click);
			// 
			// tsbtnFullScreen
			// 
			this.tsbtnFullScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnFullScreen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnFullScreen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnFullScreen.Name = "tsbtnFullScreen";
			this.tsbtnFullScreen.Size = new System.Drawing.Size(23, 22);
			this.tsbtnFullScreen.Text = "전체화면";
			// 
			// tsbtnZoomin
			// 
			this.tsbtnZoomin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnZoomin.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnZoomin.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnZoomin.Name = "tsbtnZoomin";
			this.tsbtnZoomin.Size = new System.Drawing.Size(23, 22);
			this.tsbtnZoomin.Text = "확대";
			this.tsbtnZoomin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsbtnZoomin_MouseDown);
			// 
			// tsbtnZoomout
			// 
			this.tsbtnZoomout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnZoomout.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnZoomout.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnZoomout.Name = "tsbtnZoomout";
			this.tsbtnZoomout.Size = new System.Drawing.Size(23, 22);
			this.tsbtnZoomout.Text = "축소";
			this.tsbtnZoomout.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsbtnZoomout_MouseDown);
			// 
			// tsbtnMove
			// 
			this.tsbtnMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnMove.Enabled = false;
			this.tsbtnMove.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnMove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnMove.Name = "tsbtnMove";
			this.tsbtnMove.Size = new System.Drawing.Size(23, 22);
			this.tsbtnMove.Text = "이동";
			// 
			// tsbtnPick
			// 
			this.tsbtnPick.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnPick.Enabled = false;
			this.tsbtnPick.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnPick.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnPick.Name = "tsbtnPick";
			this.tsbtnPick.Size = new System.Drawing.Size(23, 22);
			this.tsbtnPick.Text = "선택";
			// 
			// tsbtnOrbit
			// 
			this.tsbtnOrbit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnOrbit.Enabled = false;
			this.tsbtnOrbit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnOrbit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnOrbit.Name = "tsbtnOrbit";
			this.tsbtnOrbit.Size = new System.Drawing.Size(23, 22);
			this.tsbtnOrbit.Text = "회전";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// tsbtnLayout1
			// 
			this.tsbtnLayout1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnLayout1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnLayout1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnLayout1.Name = "tsbtnLayout1";
			this.tsbtnLayout1.Size = new System.Drawing.Size(23, 22);
			this.tsbtnLayout1.Text = "Layout1";
			this.tsbtnLayout1.Click += new System.EventHandler(this.tsbtnLayout1_Click);
			// 
			// tsbtnLayout2
			// 
			this.tsbtnLayout2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnLayout2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnLayout2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnLayout2.Name = "tsbtnLayout2";
			this.tsbtnLayout2.Size = new System.Drawing.Size(23, 4);
			this.tsbtnLayout2.Text = "Layout2";
			this.tsbtnLayout2.Visible = false;
			// 
			// tsbtnLayout3
			// 
			this.tsbtnLayout3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnLayout3.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnLayout3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnLayout3.Name = "tsbtnLayout3";
			this.tsbtnLayout3.Size = new System.Drawing.Size(23, 22);
			this.tsbtnLayout3.Text = "Layout3";
			this.tsbtnLayout3.Click += new System.EventHandler(this.tsbtnLayout3_Click);
			// 
			// tsbtnLayout4
			// 
			this.tsbtnLayout4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnLayout4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnLayout4.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnLayout4.Name = "tsbtnLayout4";
			this.tsbtnLayout4.Size = new System.Drawing.Size(23, 4);
			this.tsbtnLayout4.Text = "Layout4";
			this.tsbtnLayout4.Visible = false;
			// 
			// tsFloor
			// 
			this.tsFloor.AutoSize = false;
			this.tsFloor.Dock = System.Windows.Forms.DockStyle.None;
			this.tsFloor.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.tsFloor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mCmbFloor});
			this.tsFloor.Location = new System.Drawing.Point(268, 2);
			this.tsFloor.Name = "tsFloor";
			this.tsFloor.Size = new System.Drawing.Size(168, 32);
			this.tsFloor.TabIndex = 2;
			this.tsFloor.Text = "toolStrip1";
			// 
			// mCmbFloor
			// 
			this.mCmbFloor.AutoSize = false;
			this.mCmbFloor.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.mCmbFloor.Name = "mCmbFloor";
			this.mCmbFloor.Size = new System.Drawing.Size(121, 28);
			// 
			// tsbtnLeft
			// 
			this.tsbtnLeft.Name = "tsbtnLeft";
			this.tsbtnLeft.Size = new System.Drawing.Size(23, 23);
			// 
			// tsbtnFloor1
			// 
			this.tsbtnFloor1.Name = "tsbtnFloor1";
			this.tsbtnFloor1.Size = new System.Drawing.Size(23, 23);
			// 
			// tsbtnFloor2
			// 
			this.tsbtnFloor2.Name = "tsbtnFloor2";
			this.tsbtnFloor2.Size = new System.Drawing.Size(23, 23);
			// 
			// tsbtnFloor3
			// 
			this.tsbtnFloor3.Name = "tsbtnFloor3";
			this.tsbtnFloor3.Size = new System.Drawing.Size(23, 23);
			// 
			// tsbtnFloor4
			// 
			this.tsbtnFloor4.Name = "tsbtnFloor4";
			this.tsbtnFloor4.Size = new System.Drawing.Size(23, 23);
			// 
			// tsbtnFloor5
			// 
			this.tsbtnFloor5.Name = "tsbtnFloor5";
			this.tsbtnFloor5.Size = new System.Drawing.Size(23, 23);
			// 
			// tsbtnRight
			// 
			this.tsbtnRight.Name = "tsbtnRight";
			this.tsbtnRight.Size = new System.Drawing.Size(23, 23);
			// 
			// tsSetting
			// 
			this.tsSetting.Location = new System.Drawing.Point(0, 0);
			this.tsSetting.Name = "tsSetting";
			this.tsSetting.Size = new System.Drawing.Size(100, 25);
			this.tsSetting.TabIndex = 0;
			// 
			// tsbtnSetting
			// 
			this.tsbtnSetting.Name = "tsbtnSetting";
			this.tsbtnSetting.Size = new System.Drawing.Size(23, 23);
			// 
			// tsbtnAutoNavi
			// 
			this.tsbtnAutoNavi.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnAutoNavi.Image = global::SOPMonitoringSystem.Properties.Resources.btn_AutoNavi;
			this.tsbtnAutoNavi.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsbtnAutoNavi.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnAutoNavi.Name = "tsbtnAutoNavi";
			this.tsbtnAutoNavi.Size = new System.Drawing.Size(28, 28);
			this.tsbtnAutoNavi.Text = "네비게이션";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.panel1.Controls.Add(this.tsFloor);
			this.panel1.Controls.Add(this.tsViewCtrl);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1134, 33);
			this.panel1.TabIndex = 1;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1134, 726);
			this.Controls.Add(this.tabCtrlMonitoring);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(1000, 600);
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Hazard Management System";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.Resize += new System.EventHandler(this.FormMain2_Resize);
			this.tabCtrlMonitoring.ResumeLayout(false);
			this.tabDisaster.ResumeLayout(false);
			this.panelMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.axSkinFramework1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).EndInit();
			this.tsViewCtrl.ResumeLayout(false);
			this.tsViewCtrl.PerformLayout();
			this.tsFloor.ResumeLayout(false);
			this.tsFloor.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabCtrlMonitoring;
        private System.Windows.Forms.TabPage tabDisaster;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelVirtool;
        private AxXtremeDockingPane.AxDockingPane axDockingPane;
        private System.Windows.Forms.ImageList imageListViewCtrl;
        private AxXtremeSkinFramework.AxSkinFramework axSkinFramework1;
       // private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip tsSetting;
        private System.Windows.Forms.ToolStripButton tsbtnSetting;
        private System.Windows.Forms.ToolStripButton tsbtnAutoNavi;
        private System.Windows.Forms.ToolStrip tsFloor;
        private System.Windows.Forms.ToolStripButton tsbtnLeft;
        private System.Windows.Forms.ToolStripButton tsbtnFloor1;
        private System.Windows.Forms.ToolStripButton tsbtnFloor2;
        private System.Windows.Forms.ToolStripButton tsbtnFloor3;
        private System.Windows.Forms.ToolStripButton tsbtnFloor4;
        private System.Windows.Forms.ToolStripButton tsbtnFloor5;
        private System.Windows.Forms.ToolStripButton tsbtnRight;
        private System.Windows.Forms.ToolStrip tsViewCtrl;
        private System.Windows.Forms.ToolStripButton tsbtnHomeView;
        private System.Windows.Forms.ToolStripButton tsbtnFullScreen;
        private System.Windows.Forms.ToolStripButton tsbtnZoomin;
        private System.Windows.Forms.ToolStripButton tsbtnZoomout;
        private System.Windows.Forms.ToolStripButton tsbtnMove;
        private System.Windows.Forms.ToolStripButton tsbtnPick;
        private System.Windows.Forms.ToolStripButton tsbtnOrbit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbtnLayout1;
        private System.Windows.Forms.ToolStripButton tsbtnLayout2;
        private System.Windows.Forms.ToolStripButton tsbtnLayout3;
        private System.Windows.Forms.ToolStripButton tsbtnLayout4;
		private System.Windows.Forms.ToolStripComboBox mCmbFloor;
    }
}