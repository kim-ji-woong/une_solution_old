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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tsSetting = new System.Windows.Forms.ToolStrip();
            this.tsbtnSetting = new System.Windows.Forms.ToolStripButton();
            this.tsbtnAutoNavi = new System.Windows.Forms.ToolStripButton();
            this.tsFloor = new System.Windows.Forms.ToolStrip();
            this.tsbtnLeft = new System.Windows.Forms.ToolStripButton();
            this.tsbtnFloor1 = new System.Windows.Forms.ToolStripButton();
            this.tsbtnFloor2 = new System.Windows.Forms.ToolStripButton();
            this.tsbtnFloor3 = new System.Windows.Forms.ToolStripButton();
            this.tsbtnFloor4 = new System.Windows.Forms.ToolStripButton();
            this.tsbtnFloor5 = new System.Windows.Forms.ToolStripButton();
            this.tsbtnRight = new System.Windows.Forms.ToolStripButton();
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
            this.tabEquipment = new System.Windows.Forms.TabPage();
            this.tabSensor = new System.Windows.Forms.TabPage();
            this.tabCCTV = new System.Windows.Forms.TabPage();
            this.imageListViewCtrl = new System.Windows.Forms.ImageList(this.components);
            this.tabCtrlMonitoring.SuspendLayout();
            this.tabDisaster.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).BeginInit();
            this.panelVirtool.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tsSetting.SuspendLayout();
            this.tsFloor.SuspendLayout();
            this.tsViewCtrl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabCtrlMonitoring
            // 
            this.tabCtrlMonitoring.Controls.Add(this.tabDisaster);
            this.tabCtrlMonitoring.Controls.Add(this.tabEquipment);
            this.tabCtrlMonitoring.Controls.Add(this.tabSensor);
            this.tabCtrlMonitoring.Controls.Add(this.tabCCTV);
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
            this.panelVirtool.Controls.Add(this.splitContainer);
            this.panelVirtool.Location = new System.Drawing.Point(240, 0);
            this.panelVirtool.Name = "panelVirtool";
            this.panelVirtool.Size = new System.Drawing.Size(620, 416);
            this.panelVirtool.TabIndex = 1;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.splitContainer.Panel1MinSize = 32;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer.Panel2.Controls.Add(this.panel1);
            this.splitContainer.Panel2MinSize = 32;
            this.splitContainer.Size = new System.Drawing.Size(620, 416);
            this.splitContainer.SplitterDistance = 383;
            this.splitContainer.SplitterWidth = 1;
            this.splitContainer.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panel1.Controls.Add(this.tsSetting);
            this.panel1.Controls.Add(this.tsFloor);
            this.panel1.Controls.Add(this.tsViewCtrl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(620, 33);
            this.panel1.TabIndex = 1;
            // 
            // tsSetting
            // 
            this.tsSetting.Dock = System.Windows.Forms.DockStyle.None;
            this.tsSetting.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnSetting,
            this.tsbtnAutoNavi});
            this.tsSetting.Location = new System.Drawing.Point(549, 2);
            this.tsSetting.Name = "tsSetting";
            this.tsSetting.Size = new System.Drawing.Size(99, 31);
            this.tsSetting.TabIndex = 3;
            this.tsSetting.Text = "toolStrip2";
            // 
            // tsbtnSetting
            // 
            this.tsbtnSetting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnSetting.Image = global::SOPMonitoringSystem.Properties.Resources.btn_setting;
            this.tsbtnSetting.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnSetting.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnSetting.Name = "tsbtnSetting";
            this.tsbtnSetting.Size = new System.Drawing.Size(28, 28);
            this.tsbtnSetting.Text = "환경설정";
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
            this.tsbtnAutoNavi.Click += new System.EventHandler(this.tsbtnAutoNavi_Click);
            //this.tsbtnAutoNavi.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tsbtnAutoNavi_MouseUp);
            // 
            // tsFloor
            // 
            this.tsFloor.Dock = System.Windows.Forms.DockStyle.None;
            this.tsFloor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnLeft,
            this.tsbtnFloor1,
            this.tsbtnFloor2,
            this.tsbtnFloor3,
            this.tsbtnFloor4,
            this.tsbtnFloor5,
            this.tsbtnRight});
            this.tsFloor.Location = new System.Drawing.Point(326, 2);
            this.tsFloor.Name = "tsFloor";
            this.tsFloor.Size = new System.Drawing.Size(223, 31);
            this.tsFloor.TabIndex = 2;
            this.tsFloor.Text = "toolStrip1";
            // 
            // tsbtnLeft
            // 
            this.tsbtnLeft.AutoToolTip = false;
            this.tsbtnLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnLeft.Image = global::SOPMonitoringSystem.Properties.Resources.btn_MoveLeft;
            this.tsbtnLeft.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnLeft.Name = "tsbtnLeft";
            this.tsbtnLeft.Size = new System.Drawing.Size(28, 28);
            this.tsbtnLeft.Click += new System.EventHandler(this.tsbtnLeft_Click);
            // 
            // tsbtnFloor1
            // 
            this.tsbtnFloor1.AutoSize = false;
            this.tsbtnFloor1.AutoToolTip = false;
            this.tsbtnFloor1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnFloor1.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnFloor1.Image")));
            this.tsbtnFloor1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnFloor1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnFloor1.Name = "tsbtnFloor1";
            this.tsbtnFloor1.Size = new System.Drawing.Size(31, 28);
            this.tsbtnFloor1.Text = "1F";
            this.tsbtnFloor1.Click += new System.EventHandler(this.tsbtnFloor1_Click);
            // 
            // tsbtnFloor2
            // 
            this.tsbtnFloor2.AutoSize = false;
            this.tsbtnFloor2.AutoToolTip = false;
            this.tsbtnFloor2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnFloor2.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnFloor2.Image")));
            this.tsbtnFloor2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnFloor2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnFloor2.Name = "tsbtnFloor2";
            this.tsbtnFloor2.Size = new System.Drawing.Size(31, 28);
            this.tsbtnFloor2.Text = "2F";
            this.tsbtnFloor2.Click += new System.EventHandler(this.tsbtnFloor2_Click);
            // 
            // tsbtnFloor3
            // 
            this.tsbtnFloor3.AutoSize = false;
            this.tsbtnFloor3.AutoToolTip = false;
            this.tsbtnFloor3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnFloor3.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnFloor3.Image")));
            this.tsbtnFloor3.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnFloor3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnFloor3.Name = "tsbtnFloor3";
            this.tsbtnFloor3.Size = new System.Drawing.Size(31, 28);
            this.tsbtnFloor3.Text = "3F";
            this.tsbtnFloor3.Click += new System.EventHandler(this.tsbtnFloor3_Click);
            // 
            // tsbtnFloor4
            // 
            this.tsbtnFloor4.AutoSize = false;
            this.tsbtnFloor4.AutoToolTip = false;
            this.tsbtnFloor4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnFloor4.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnFloor4.Image")));
            this.tsbtnFloor4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnFloor4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnFloor4.Name = "tsbtnFloor4";
            this.tsbtnFloor4.Size = new System.Drawing.Size(31, 28);
            this.tsbtnFloor4.Text = "4F";
            this.tsbtnFloor4.Click += new System.EventHandler(this.tsbtnFloor4_Click);
            // 
            // tsbtnFloor5
            // 
            this.tsbtnFloor5.AutoSize = false;
            this.tsbtnFloor5.AutoToolTip = false;
            this.tsbtnFloor5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnFloor5.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnFloor5.Image")));
            this.tsbtnFloor5.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnFloor5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnFloor5.Name = "tsbtnFloor5";
            this.tsbtnFloor5.Size = new System.Drawing.Size(31, 28);
            this.tsbtnFloor5.Text = "5F";
            this.tsbtnFloor5.Click += new System.EventHandler(this.tsbtnFloor5_Click);
            // 
            // tsbtnRight
            // 
            this.tsbtnRight.AutoToolTip = false;
            this.tsbtnRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnRight.Image = global::SOPMonitoringSystem.Properties.Resources.btn_MoveRight;
            this.tsbtnRight.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnRight.Name = "tsbtnRight";
            this.tsbtnRight.Size = new System.Drawing.Size(28, 28);
            this.tsbtnRight.Click += new System.EventHandler(this.tsbtnRight_Click);
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
            this.tsViewCtrl.Size = new System.Drawing.Size(271, 25);
            this.tsViewCtrl.TabIndex = 1;
            this.tsViewCtrl.Text = "toolStrip1";
            // 
            // tsbtnHomeView
            // 
            this.tsbtnHomeView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnHomeView.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnHomeView.Image")));
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
            this.tsbtnFullScreen.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnFullScreen.Image")));
            this.tsbtnFullScreen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnFullScreen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnFullScreen.Name = "tsbtnFullScreen";
            this.tsbtnFullScreen.Size = new System.Drawing.Size(23, 22);
            this.tsbtnFullScreen.Text = "전체화면";
            this.tsbtnFullScreen.Click += new System.EventHandler(this.tsbtnFullScreen_Click);
            // 
            // tsbtnZoomin
            // 
            this.tsbtnZoomin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnZoomin.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnZoomin.Image")));
            this.tsbtnZoomin.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnZoomin.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnZoomin.Name = "tsbtnZoomin";
            this.tsbtnZoomin.Size = new System.Drawing.Size(23, 22);
            this.tsbtnZoomin.Text = "확대";
            this.tsbtnZoomin.Click += new System.EventHandler(this.tsbtnZoomin_Click);
            this.tsbtnZoomin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsbtnZoomin_MouseDown);
            this.tsbtnZoomin.MouseHover += new System.EventHandler(this.tsbtnZoomin_MouseHover);
            this.tsbtnZoomin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tsbtnZoomin_MouseUp);
            // 
            // tsbtnZoomout
            // 
            this.tsbtnZoomout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnZoomout.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnZoomout.Image")));
            this.tsbtnZoomout.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnZoomout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnZoomout.Name = "tsbtnZoomout";
            this.tsbtnZoomout.Size = new System.Drawing.Size(23, 22);
            this.tsbtnZoomout.Text = "축소";
            this.tsbtnZoomout.Click += new System.EventHandler(this.tsbtnZoomout_Click);
            this.tsbtnZoomout.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsbtnZoomout_MouseDown);
            this.tsbtnZoomout.MouseHover += new System.EventHandler(this.tsbtnZoomout_MouseHover);
            this.tsbtnZoomout.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tsbtnZoomout_MouseUp);
            // 
            // tsbtnMove
            // 
            this.tsbtnMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnMove.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnMove.Image")));
            this.tsbtnMove.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnMove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnMove.Name = "tsbtnMove";
            this.tsbtnMove.Size = new System.Drawing.Size(23, 22);
            this.tsbtnMove.Text = "이동";
            // 
            // tsbtnPick
            // 
            this.tsbtnPick.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnPick.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnPick.Image")));
            this.tsbtnPick.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnPick.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnPick.Name = "tsbtnPick";
            this.tsbtnPick.Size = new System.Drawing.Size(23, 22);
            this.tsbtnPick.Text = "선택";
            // 
            // tsbtnOrbit
            // 
            this.tsbtnOrbit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnOrbit.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnOrbit.Image")));
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
            this.tsbtnLayout1.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnLayout1.Image")));
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
            this.tsbtnLayout2.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnLayout2.Image")));
            this.tsbtnLayout2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnLayout2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnLayout2.Name = "tsbtnLayout2";
            this.tsbtnLayout2.Size = new System.Drawing.Size(23, 22);
            this.tsbtnLayout2.Text = "Layout2";
            this.tsbtnLayout2.Click += new System.EventHandler(this.tsbtnLayout2_Click);
            // 
            // tsbtnLayout3
            // 
            this.tsbtnLayout3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnLayout3.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnLayout3.Image")));
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
            this.tsbtnLayout4.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnLayout4.Image")));
            this.tsbtnLayout4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnLayout4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnLayout4.Name = "tsbtnLayout4";
            this.tsbtnLayout4.Size = new System.Drawing.Size(23, 22);
            this.tsbtnLayout4.Text = "Layout4";
            this.tsbtnLayout4.Click += new System.EventHandler(this.tsbtnLayout4_Click);
            // 
            // tabEquipment
            // 
            this.tabEquipment.Location = new System.Drawing.Point(4, 22);
            this.tabEquipment.Name = "tabEquipment";
            this.tabEquipment.Padding = new System.Windows.Forms.Padding(3);
            this.tabEquipment.Size = new System.Drawing.Size(1126, 700);
            this.tabEquipment.TabIndex = 1;
            this.tabEquipment.Text = "소방설비";
            this.tabEquipment.UseVisualStyleBackColor = true;
            // 
            // tabSensor
            // 
            this.tabSensor.Location = new System.Drawing.Point(4, 22);
            this.tabSensor.Name = "tabSensor";
            this.tabSensor.Padding = new System.Windows.Forms.Padding(3);
            this.tabSensor.Size = new System.Drawing.Size(1126, 700);
            this.tabSensor.TabIndex = 2;
            this.tabSensor.Text = "화재감지";
            this.tabSensor.UseVisualStyleBackColor = true;
            // 
            // tabCCTV
            // 
            this.tabCCTV.Location = new System.Drawing.Point(4, 22);
            this.tabCCTV.Name = "tabCCTV";
            this.tabCCTV.Padding = new System.Windows.Forms.Padding(3);
            this.tabCCTV.Size = new System.Drawing.Size(1126, 700);
            this.tabCCTV.TabIndex = 3;
            this.tabCCTV.Text = "CCTV";
            this.tabCCTV.UseVisualStyleBackColor = true;
            // 
            // imageListViewCtrl
            // 
            this.imageListViewCtrl.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListViewCtrl.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListViewCtrl.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 726);
            this.Controls.Add(this.tabCtrlMonitoring);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "재난관리시스템";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain2_Resize);
            this.tabCtrlMonitoring.ResumeLayout(false);
            this.tabDisaster.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).EndInit();
            this.panelVirtool.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tsSetting.ResumeLayout(false);
            this.tsSetting.PerformLayout();
            this.tsFloor.ResumeLayout(false);
            this.tsFloor.PerformLayout();
            this.tsViewCtrl.ResumeLayout(false);
            this.tsViewCtrl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabCtrlMonitoring;
        private System.Windows.Forms.TabPage tabDisaster;
        private System.Windows.Forms.TabPage tabEquipment;
        private System.Windows.Forms.TabPage tabSensor;
        private System.Windows.Forms.TabPage tabCCTV;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelVirtool;
        private AxXtremeDockingPane.AxDockingPane axDockingPane;
        private System.Windows.Forms.ImageList imageListViewCtrl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip tsSetting;
        private System.Windows.Forms.ToolStripButton tsbtnSetting;
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
        private AxXtremeSkinFramework.AxSkinFramework axSkinFramework1;
        private System.Windows.Forms.ToolStripButton tsbtnAutoNavi;
    }
}