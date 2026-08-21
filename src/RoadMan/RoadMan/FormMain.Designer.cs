namespace RoadMan
{
    partial class FormMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.파일FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveAsProject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.보기VToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProcessLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProcessSchedule = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProcessResult = new System.Windows.Forms.ToolStripMenuItem();
            this.편집ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.도구EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMemo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuScreenCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOption = new System.Windows.Forms.ToolStripMenuItem();
            this.도움말HToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerLeft = new System.Windows.Forms.SplitContainer();
            this.splitContainerLeftDown = new System.Windows.Forms.SplitContainer();
            this.panelLeftToolBar = new System.Windows.Forms.Panel();
            this.rbtnHidePanel = new UnE.GUI.RibbonButton();
            this.rbtnShowPanel = new UnE.GUI.RibbonButton();
            this.tabControlEx1 = new UnE.Controls.TabControlEx();
            this.paneRibbonToolBar = new System.Windows.Forms.Panel();
            this.rbtnUndo = new UnE.GUI.RibbonButton();
            this.rbtnRedo = new UnE.GUI.RibbonButton();
            this.rbtnSearch = new UnE.GUI.RibbonButton();
            this.rbtnProcessLayer = new UnE.GUI.RibbonButton();
            this.rbtnReport = new UnE.GUI.RibbonButton();
            this.rbtnCloseScreenCapture = new UnE.GUI.RibbonButton();
            this.rbtnSaveScreenCaptureImage = new UnE.GUI.RibbonButton();
            this.rbtnSelectFullScreen = new UnE.GUI.RibbonButton();
            this.rbtnMemo = new UnE.GUI.RibbonButton();
            this.rbtnPrint = new UnE.GUI.RibbonButton();
            this.rbtnOptions = new UnE.GUI.RibbonButton();
            this.rbtnScreenShot = new UnE.GUI.RibbonButton();
            this.rbtnLayer = new UnE.GUI.RibbonButton();
            this.rbtnProcessSchedule = new UnE.GUI.RibbonButton();
            this.rbtnProcessResult = new UnE.GUI.RibbonButton();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsLabelStatusWork = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLabelCoord = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLabelClock = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLabelCaps = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLabelNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLabelHangul = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLabelProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tsLabelCompany = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusClockTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddTabToLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddTabToRight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteTab = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMoveToLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMoveToRight = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.updateCmdTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).BeginInit();
            this.splitContainerLeft.Panel2.SuspendLayout();
            this.splitContainerLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeftDown)).BeginInit();
            this.splitContainerLeftDown.SuspendLayout();
            this.panelLeftToolBar.SuspendLayout();
            this.paneRibbonToolBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.파일FToolStripMenuItem,
            this.보기VToolStripMenuItem,
            this.편집ToolStripMenuItem,
            this.도구EToolStripMenuItem,
            this.도움말HToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1223, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 파일FToolStripMenuItem
            // 
            this.파일FToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNewProject,
            this.menuOpenProject,
            this.menuSaveProject,
            this.menuSaveAsProject,
            this.toolStripSeparator1,
            this.menuPrint});
            this.파일FToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.파일FToolStripMenuItem.Name = "파일FToolStripMenuItem";
            this.파일FToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.파일FToolStripMenuItem.Text = "파일(&F)";
            // 
            // menuNewProject
            // 
            this.menuNewProject.ForeColor = System.Drawing.Color.Black;
            this.menuNewProject.Name = "menuNewProject";
            this.menuNewProject.Size = new System.Drawing.Size(230, 22);
            this.menuNewProject.Text = "새 프로젝트";
            this.menuNewProject.Click += new System.EventHandler(this.menu_Click);
            // 
            // menuOpenProject
            // 
            this.menuOpenProject.Name = "menuOpenProject";
            this.menuOpenProject.Size = new System.Drawing.Size(230, 22);
            this.menuOpenProject.Text = "프로젝트 열기";
            this.menuOpenProject.Click += new System.EventHandler(this.menu_Click);
            // 
            // menuSaveProject
            // 
            this.menuSaveProject.Name = "menuSaveProject";
            this.menuSaveProject.Size = new System.Drawing.Size(230, 22);
            this.menuSaveProject.Text = "프로젝트 저장";
            this.menuSaveProject.Click += new System.EventHandler(this.menu_Click);
            // 
            // menuSaveAsProject
            // 
            this.menuSaveAsProject.Name = "menuSaveAsProject";
            this.menuSaveAsProject.Size = new System.Drawing.Size(230, 22);
            this.menuSaveAsProject.Text = "다른 이름으로 프로젝트 저장";
            this.menuSaveAsProject.Click += new System.EventHandler(this.menu_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(227, 6);
            // 
            // menuPrint
            // 
            this.menuPrint.Name = "menuPrint";
            this.menuPrint.Size = new System.Drawing.Size(230, 22);
            this.menuPrint.Text = "인쇄";
            // 
            // 보기VToolStripMenuItem
            // 
            this.보기VToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLayer,
            this.menuProcessLayer,
            this.menuProcessSchedule,
            this.menuProcessResult});
            this.보기VToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.보기VToolStripMenuItem.Name = "보기VToolStripMenuItem";
            this.보기VToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.보기VToolStripMenuItem.Text = "보기(&V)";
            // 
            // menuLayer
            // 
            this.menuLayer.Name = "menuLayer";
            this.menuLayer.Size = new System.Drawing.Size(146, 22);
            this.menuLayer.Text = "도면층";
            // 
            // menuProcessLayer
            // 
            this.menuProcessLayer.Name = "menuProcessLayer";
            this.menuProcessLayer.Size = new System.Drawing.Size(146, 22);
            this.menuProcessLayer.Text = "집행도면층";
            // 
            // menuProcessSchedule
            // 
            this.menuProcessSchedule.Name = "menuProcessSchedule";
            this.menuProcessSchedule.Size = new System.Drawing.Size(146, 22);
            this.menuProcessSchedule.Text = "집행계획";
            // 
            // menuProcessResult
            // 
            this.menuProcessResult.Name = "menuProcessResult";
            this.menuProcessResult.Size = new System.Drawing.Size(146, 22);
            this.menuProcessResult.Text = "집행진행상황";
            // 
            // 편집ToolStripMenuItem
            // 
            this.편집ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuUndo,
            this.menuRedo});
            this.편집ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.편집ToolStripMenuItem.Name = "편집ToolStripMenuItem";
            this.편집ToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.편집ToolStripMenuItem.Text = "편집(&E)";
            // 
            // menuUndo
            // 
            this.menuUndo.Name = "menuUndo";
            this.menuUndo.Size = new System.Drawing.Size(122, 22);
            this.menuUndo.Text = "되돌리기";
            // 
            // menuRedo
            // 
            this.menuRedo.Name = "menuRedo";
            this.menuRedo.Size = new System.Drawing.Size(122, 22);
            this.menuRedo.Text = "다시실행";
            // 
            // 도구EToolStripMenuItem
            // 
            this.도구EToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSearch,
            this.menuMemo,
            this.menuReport,
            this.menuScreenCapture,
            this.menuOption});
            this.도구EToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.도구EToolStripMenuItem.Name = "도구EToolStripMenuItem";
            this.도구EToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.도구EToolStripMenuItem.Text = "도구(&T)";
            // 
            // menuSearch
            // 
            this.menuSearch.Name = "menuSearch";
            this.menuSearch.Size = new System.Drawing.Size(122, 22);
            this.menuSearch.Text = "검색";
            this.menuSearch.Click += new System.EventHandler(this.menuSearch_Click);
            // 
            // menuMemo
            // 
            this.menuMemo.Name = "menuMemo";
            this.menuMemo.Size = new System.Drawing.Size(122, 22);
            this.menuMemo.Text = "메모";
            // 
            // menuReport
            // 
            this.menuReport.Name = "menuReport";
            this.menuReport.Size = new System.Drawing.Size(122, 22);
            this.menuReport.Text = "보고서";
            // 
            // menuScreenCapture
            // 
            this.menuScreenCapture.Name = "menuScreenCapture";
            this.menuScreenCapture.Size = new System.Drawing.Size(122, 22);
            this.menuScreenCapture.Text = "화면캡쳐";
            // 
            // menuOption
            // 
            this.menuOption.Name = "menuOption";
            this.menuOption.Size = new System.Drawing.Size(122, 22);
            this.menuOption.Text = "설정";
            // 
            // 도움말HToolStripMenuItem
            // 
            this.도움말HToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelp});
            this.도움말HToolStripMenuItem.Name = "도움말HToolStripMenuItem";
            this.도움말HToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.도움말HToolStripMenuItem.Text = "도움말(&H)";
            // 
            // menuHelp
            // 
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(159, 22);
            this.menuHelp.Text = "도움말 보기(F1)";
            this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.splitContainerMain.Location = new System.Drawing.Point(0, 124);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerLeft);
            this.splitContainerMain.Panel1.Controls.Add(this.panelLeftToolBar);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.tabControlEx1);
            this.splitContainerMain.Size = new System.Drawing.Size(1223, 620);
            this.splitContainerMain.SplitterDistance = 400;
            this.splitContainerMain.TabIndex = 2;
            this.splitContainerMain.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.splitContainerMain_SplitterMoving);
            this.splitContainerMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerMain_SplitterMoved);
            // 
            // splitContainerLeft
            // 
            this.splitContainerLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.splitContainerLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeft.Location = new System.Drawing.Point(40, 0);
            this.splitContainerLeft.Name = "splitContainerLeft";
            this.splitContainerLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerLeft.Panel2
            // 
            this.splitContainerLeft.Panel2.Controls.Add(this.splitContainerLeftDown);
            this.splitContainerLeft.Size = new System.Drawing.Size(360, 620);
            this.splitContainerLeft.SplitterDistance = 286;
            this.splitContainerLeft.TabIndex = 0;
            // 
            // splitContainerLeftDown
            // 
            this.splitContainerLeftDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeftDown.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLeftDown.Name = "splitContainerLeftDown";
            this.splitContainerLeftDown.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerLeftDown.Size = new System.Drawing.Size(360, 330);
            this.splitContainerLeftDown.SplitterDistance = 151;
            this.splitContainerLeftDown.TabIndex = 0;
            // 
            // panelLeftToolBar
            // 
            this.panelLeftToolBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.panelLeftToolBar.Controls.Add(this.rbtnHidePanel);
            this.panelLeftToolBar.Controls.Add(this.rbtnShowPanel);
            this.panelLeftToolBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeftToolBar.Location = new System.Drawing.Point(0, 0);
            this.panelLeftToolBar.Name = "panelLeftToolBar";
            this.panelLeftToolBar.Size = new System.Drawing.Size(40, 620);
            this.panelLeftToolBar.TabIndex = 0;
            // 
            // rbtnHidePanel
            // 
            this.rbtnHidePanel.BackColor = System.Drawing.Color.Transparent;
            this.rbtnHidePanel.CheckButton = false;
            this.rbtnHidePanel.CheckedBkgndImage = null;
            this.rbtnHidePanel.CheckedImage = null;
            this.rbtnHidePanel.ClickedBackgroundImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnHidePanel.ClickedImage = global::RoadMan.Properties.Resources.left_arrow_click;
            this.rbtnHidePanel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 40);
            this.rbtnHidePanel.DisabledBkgndImage = null;
            this.rbtnHidePanel.DisabledImage = global::RoadMan.Properties.Resources.left_arrow_disable;
            this.rbtnHidePanel.ID = -1;
            this.rbtnHidePanel.InitButtonWidth = 40;
            this.rbtnHidePanel.IsChecked = false;
            this.rbtnHidePanel.Location = new System.Drawing.Point(0, 3);
            this.rbtnHidePanel.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnHidePanel.MouseOverImage = global::RoadMan.Properties.Resources.left_arrow_over;
            this.rbtnHidePanel.Name = "rbtnHidePanel";
            this.rbtnHidePanel.NormalImage = global::RoadMan.Properties.Resources.left_arrow_normal;
            this.rbtnHidePanel.Owner = null;
            this.rbtnHidePanel.Size = new System.Drawing.Size(40, 40);
            this.rbtnHidePanel.TabIndex = 29;
            this.rbtnHidePanel.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnHidePanel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnHidePanel.ToolTipText = "";
            this.rbtnHidePanel.UseCustomImageRect = true;
            this.rbtnHidePanel.UseTextLocation = false;
            this.rbtnHidePanel.UseVisualStyleBackColor = false;
            this.rbtnHidePanel.Click += new System.EventHandler(this.rbtnHidePanel_Click);
            // 
            // rbtnShowPanel
            // 
            this.rbtnShowPanel.BackColor = System.Drawing.Color.Transparent;
            this.rbtnShowPanel.CheckButton = false;
            this.rbtnShowPanel.CheckedBkgndImage = null;
            this.rbtnShowPanel.CheckedImage = null;
            this.rbtnShowPanel.ClickedBackgroundImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnShowPanel.ClickedImage = global::RoadMan.Properties.Resources.right_arrow_click;
            this.rbtnShowPanel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 40);
            this.rbtnShowPanel.DisabledBkgndImage = null;
            this.rbtnShowPanel.DisabledImage = global::RoadMan.Properties.Resources.right_arrow_disable;
            this.rbtnShowPanel.ID = -1;
            this.rbtnShowPanel.InitButtonWidth = 40;
            this.rbtnShowPanel.IsChecked = false;
            this.rbtnShowPanel.Location = new System.Drawing.Point(0, 71);
            this.rbtnShowPanel.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnShowPanel.MouseOverImage = global::RoadMan.Properties.Resources.right_arrow_over;
            this.rbtnShowPanel.Name = "rbtnShowPanel";
            this.rbtnShowPanel.NormalImage = global::RoadMan.Properties.Resources.right_arrow_normal;
            this.rbtnShowPanel.Owner = null;
            this.rbtnShowPanel.Size = new System.Drawing.Size(40, 40);
            this.rbtnShowPanel.TabIndex = 28;
            this.rbtnShowPanel.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnShowPanel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnShowPanel.ToolTipText = "";
            this.rbtnShowPanel.UseCustomImageRect = true;
            this.rbtnShowPanel.UseTextLocation = false;
            this.rbtnShowPanel.UseVisualStyleBackColor = false;
            this.rbtnShowPanel.Visible = false;
            this.rbtnShowPanel.Click += new System.EventHandler(this.rbtnShowPanel_Click);
            // 
            // tabControlEx1
            // 
            this.tabControlEx1.CloseBtnImage = ((System.Drawing.Image)(resources.GetObject("tabControlEx1.CloseBtnImage")));
            this.tabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlEx1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlEx1.HotTrack = true;
            this.tabControlEx1.ItemSize = new System.Drawing.Size(150, 25);
            this.tabControlEx1.Location = new System.Drawing.Point(0, 0);
            this.tabControlEx1.Name = "tabControlEx1";
            this.tabControlEx1.SelectedIndex = 0;
            this.tabControlEx1.SelectedTabColor = System.Drawing.Color.DarkGray;
            this.tabControlEx1.ShowToolTips = true;
            this.tabControlEx1.Size = new System.Drawing.Size(819, 620);
            this.tabControlEx1.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.tabControlEx1.TabForeColor = System.Drawing.Color.White;
            this.tabControlEx1.TabIndex = 1;
            this.tabControlEx1.UseCloseButton = true;
            this.tabControlEx1.OnTabMouseUp += new UnE.Controls.TabMouseUp(this.tabControlEx1_OnTabMouseUp);
            this.tabControlEx1.SelectedIndexChanged += new System.EventHandler(this.tabControlEx1_SelectedIndexChanged);
            this.tabControlEx1.SizeChanged += new System.EventHandler(this.tabControlEx1_SizeChanged);
            // 
            // paneRibbonToolBar
            // 
            this.paneRibbonToolBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.paneRibbonToolBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.paneRibbonToolBar.Controls.Add(this.rbtnUndo);
            this.paneRibbonToolBar.Controls.Add(this.rbtnRedo);
            this.paneRibbonToolBar.Controls.Add(this.rbtnSearch);
            this.paneRibbonToolBar.Controls.Add(this.rbtnProcessLayer);
            this.paneRibbonToolBar.Controls.Add(this.rbtnReport);
            this.paneRibbonToolBar.Controls.Add(this.rbtnCloseScreenCapture);
            this.paneRibbonToolBar.Controls.Add(this.rbtnSaveScreenCaptureImage);
            this.paneRibbonToolBar.Controls.Add(this.rbtnSelectFullScreen);
            this.paneRibbonToolBar.Controls.Add(this.rbtnMemo);
            this.paneRibbonToolBar.Controls.Add(this.rbtnPrint);
            this.paneRibbonToolBar.Controls.Add(this.rbtnOptions);
            this.paneRibbonToolBar.Controls.Add(this.rbtnScreenShot);
            this.paneRibbonToolBar.Controls.Add(this.rbtnLayer);
            this.paneRibbonToolBar.Controls.Add(this.rbtnProcessSchedule);
            this.paneRibbonToolBar.Controls.Add(this.rbtnProcessResult);
            this.paneRibbonToolBar.Controls.Add(this.pictureBox4);
            this.paneRibbonToolBar.Controls.Add(this.pictureBox2);
            this.paneRibbonToolBar.Controls.Add(this.pictureBox1);
            this.paneRibbonToolBar.Controls.Add(this.pictureBox3);
            this.paneRibbonToolBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.paneRibbonToolBar.Location = new System.Drawing.Point(0, 24);
            this.paneRibbonToolBar.Name = "paneRibbonToolBar";
            this.paneRibbonToolBar.Size = new System.Drawing.Size(1223, 100);
            this.paneRibbonToolBar.TabIndex = 1;
            // 
            // rbtnUndo
            // 
            this.rbtnUndo.BackColor = System.Drawing.Color.Transparent;
            this.rbtnUndo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbtnUndo.CheckButton = false;
            this.rbtnUndo.CheckedBkgndImage = null;
            this.rbtnUndo.CheckedImage = null;
            this.rbtnUndo.ClickedBackgroundImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnUndo.ClickedImage = null;
            this.rbtnUndo.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnUndo.DisabledBkgndImage = null;
            this.rbtnUndo.DisabledImage = global::RoadMan.Properties.Resources.undo_disabled;
            this.rbtnUndo.Enabled = false;
            this.rbtnUndo.ID = -1;
            this.rbtnUndo.InitButtonWidth = 70;
            this.rbtnUndo.IsChecked = false;
            this.rbtnUndo.Location = new System.Drawing.Point(582, 9);
            this.rbtnUndo.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnUndo.MouseOverImage = null;
            this.rbtnUndo.Name = "rbtnUndo";
            this.rbtnUndo.NormalImage = global::RoadMan.Properties.Resources.undo_normal;
            this.rbtnUndo.Owner = null;
            this.rbtnUndo.Size = new System.Drawing.Size(70, 80);
            this.rbtnUndo.TabIndex = 22;
            this.rbtnUndo.Text = "되돌리기";
            this.rbtnUndo.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnUndo.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnUndo.ToolTipText = "이전상태로 되돌립니다.";
            this.rbtnUndo.UseCustomImageRect = true;
            this.rbtnUndo.UseTextLocation = false;
            this.rbtnUndo.UseVisualStyleBackColor = false;
            this.rbtnUndo.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnRedo
            // 
            this.rbtnRedo.BackColor = System.Drawing.Color.Transparent;
            this.rbtnRedo.CheckButton = false;
            this.rbtnRedo.CheckedBkgndImage = null;
            this.rbtnRedo.CheckedImage = null;
            this.rbtnRedo.ClickedBackgroundImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnRedo.ClickedImage = null;
            this.rbtnRedo.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnRedo.DisabledBkgndImage = null;
            this.rbtnRedo.DisabledImage = global::RoadMan.Properties.Resources.redo_disabled;
            this.rbtnRedo.Enabled = false;
            this.rbtnRedo.ID = -1;
            this.rbtnRedo.InitButtonWidth = 70;
            this.rbtnRedo.IsChecked = false;
            this.rbtnRedo.Location = new System.Drawing.Point(653, 9);
            this.rbtnRedo.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnRedo.MouseOverImage = null;
            this.rbtnRedo.Name = "rbtnRedo";
            this.rbtnRedo.NormalImage = global::RoadMan.Properties.Resources.redo_normal;
            this.rbtnRedo.Owner = null;
            this.rbtnRedo.Size = new System.Drawing.Size(70, 80);
            this.rbtnRedo.TabIndex = 23;
            this.rbtnRedo.Text = "다시실행";
            this.rbtnRedo.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnRedo.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnRedo.ToolTipText = "다시 실행합니다.";
            this.rbtnRedo.UseCustomImageRect = true;
            this.rbtnRedo.UseTextLocation = false;
            this.rbtnRedo.UseVisualStyleBackColor = false;
            this.rbtnRedo.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnSearch
            // 
            this.rbtnSearch.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSearch.CheckButton = true;
            this.rbtnSearch.CheckedBkgndImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnSearch.CheckedImage = global::RoadMan.Properties.Resources.검색_normal;
            this.rbtnSearch.ClickedBackgroundImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnSearch.ClickedImage = null;
            this.rbtnSearch.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnSearch.DisabledBkgndImage = null;
            this.rbtnSearch.DisabledImage = global::RoadMan.Properties.Resources.검색_disable;
            this.rbtnSearch.Enabled = false;
            this.rbtnSearch.ID = -1;
            this.rbtnSearch.InitButtonWidth = 70;
            this.rbtnSearch.IsChecked = false;
            this.rbtnSearch.Location = new System.Drawing.Point(40, 9);
            this.rbtnSearch.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnSearch.MouseOverImage = null;
            this.rbtnSearch.Name = "rbtnSearch";
            this.rbtnSearch.NormalImage = global::RoadMan.Properties.Resources.검색_normal;
            this.rbtnSearch.Owner = null;
            this.rbtnSearch.Size = new System.Drawing.Size(70, 80);
            this.rbtnSearch.TabIndex = 27;
            this.rbtnSearch.Text = "검색";
            this.rbtnSearch.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSearch.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSearch.ToolTipText = "검색";
            this.rbtnSearch.UseCustomImageRect = true;
            this.rbtnSearch.UseTextLocation = false;
            this.rbtnSearch.UseVisualStyleBackColor = false;
            this.rbtnSearch.Click += new System.EventHandler(this.ribbonButton1_Click);
            // 
            // rbtnProcessLayer
            // 
            this.rbtnProcessLayer.BackColor = System.Drawing.Color.Transparent;
            this.rbtnProcessLayer.CheckButton = false;
            this.rbtnProcessLayer.CheckedBkgndImage = null;
            this.rbtnProcessLayer.CheckedImage = null;
            this.rbtnProcessLayer.ClickedBackgroundImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnProcessLayer.ClickedImage = null;
            this.rbtnProcessLayer.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnProcessLayer.DisabledBkgndImage = null;
            this.rbtnProcessLayer.DisabledImage = global::RoadMan.Properties.Resources.process_layer_disable;
            this.rbtnProcessLayer.Enabled = false;
            this.rbtnProcessLayer.ID = -1;
            this.rbtnProcessLayer.InitButtonWidth = 70;
            this.rbtnProcessLayer.IsChecked = false;
            this.rbtnProcessLayer.Location = new System.Drawing.Point(342, 9);
            this.rbtnProcessLayer.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnProcessLayer.MouseOverImage = null;
            this.rbtnProcessLayer.Name = "rbtnProcessLayer";
            this.rbtnProcessLayer.NormalImage = global::RoadMan.Properties.Resources.process_layer_normal;
            this.rbtnProcessLayer.Owner = null;
            this.rbtnProcessLayer.Size = new System.Drawing.Size(73, 80);
            this.rbtnProcessLayer.TabIndex = 24;
            this.rbtnProcessLayer.Text = "집행 도면층";
            this.rbtnProcessLayer.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnProcessLayer.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnProcessLayer.ToolTipText = "집행 도면층";
            this.rbtnProcessLayer.UseCustomImageRect = true;
            this.rbtnProcessLayer.UseTextLocation = false;
            this.rbtnProcessLayer.UseVisualStyleBackColor = false;
            this.rbtnProcessLayer.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnReport
            // 
            this.rbtnReport.BackColor = System.Drawing.Color.Transparent;
            this.rbtnReport.CheckButton = false;
            this.rbtnReport.CheckedBkgndImage = null;
            this.rbtnReport.CheckedImage = null;
            this.rbtnReport.ClickedBackgroundImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnReport.ClickedImage = null;
            this.rbtnReport.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnReport.DisabledBkgndImage = null;
            this.rbtnReport.DisabledImage = global::RoadMan.Properties.Resources.report_disable;
            this.rbtnReport.Enabled = false;
            this.rbtnReport.ID = -1;
            this.rbtnReport.InitButtonWidth = 70;
            this.rbtnReport.IsChecked = false;
            this.rbtnReport.Location = new System.Drawing.Point(192, 9);
            this.rbtnReport.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnReport.MouseOverImage = null;
            this.rbtnReport.Name = "rbtnReport";
            this.rbtnReport.NormalImage = global::RoadMan.Properties.Resources.report_normal;
            this.rbtnReport.Owner = null;
            this.rbtnReport.Size = new System.Drawing.Size(70, 80);
            this.rbtnReport.TabIndex = 24;
            this.rbtnReport.Text = "보고서";
            this.rbtnReport.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnReport.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnReport.ToolTipText = "보고서";
            this.rbtnReport.UseCustomImageRect = true;
            this.rbtnReport.UseTextLocation = false;
            this.rbtnReport.UseVisualStyleBackColor = false;
            this.rbtnReport.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnCloseScreenCapture
            // 
            this.rbtnCloseScreenCapture.BackColor = System.Drawing.Color.Transparent;
            this.rbtnCloseScreenCapture.CheckButton = false;
            this.rbtnCloseScreenCapture.CheckedBkgndImage = null;
            this.rbtnCloseScreenCapture.CheckedImage = global::RoadMan.Properties.Resources.닫기_normal;
            this.rbtnCloseScreenCapture.ClickedBackgroundImage = null;
            this.rbtnCloseScreenCapture.ClickedImage = null;
            this.rbtnCloseScreenCapture.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnCloseScreenCapture.DisabledBkgndImage = null;
            this.rbtnCloseScreenCapture.DisabledImage = global::RoadMan.Properties.Resources.닫기_disable;
            this.rbtnCloseScreenCapture.ID = -1;
            this.rbtnCloseScreenCapture.InitButtonWidth = 70;
            this.rbtnCloseScreenCapture.IsChecked = false;
            this.rbtnCloseScreenCapture.Location = new System.Drawing.Point(1136, 9);
            this.rbtnCloseScreenCapture.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnCloseScreenCapture.MouseOverImage = null;
            this.rbtnCloseScreenCapture.Name = "rbtnCloseScreenCapture";
            this.rbtnCloseScreenCapture.NormalImage = global::RoadMan.Properties.Resources.닫기_normal;
            this.rbtnCloseScreenCapture.Owner = null;
            this.rbtnCloseScreenCapture.Size = new System.Drawing.Size(70, 80);
            this.rbtnCloseScreenCapture.TabIndex = 24;
            this.rbtnCloseScreenCapture.Text = "닫기";
            this.rbtnCloseScreenCapture.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnCloseScreenCapture.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCloseScreenCapture.ToolTipText = "닫기";
            this.rbtnCloseScreenCapture.UseCustomImageRect = true;
            this.rbtnCloseScreenCapture.UseTextLocation = false;
            this.rbtnCloseScreenCapture.UseVisualStyleBackColor = false;
            this.rbtnCloseScreenCapture.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnSaveScreenCaptureImage
            // 
            this.rbtnSaveScreenCaptureImage.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSaveScreenCaptureImage.CheckButton = false;
            this.rbtnSaveScreenCaptureImage.CheckedBkgndImage = null;
            this.rbtnSaveScreenCaptureImage.CheckedImage = global::RoadMan.Properties.Resources.저장_normal;
            this.rbtnSaveScreenCaptureImage.ClickedBackgroundImage = null;
            this.rbtnSaveScreenCaptureImage.ClickedImage = null;
            this.rbtnSaveScreenCaptureImage.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnSaveScreenCaptureImage.DisabledBkgndImage = null;
            this.rbtnSaveScreenCaptureImage.DisabledImage = global::RoadMan.Properties.Resources.저장_disable;
            this.rbtnSaveScreenCaptureImage.ID = -1;
            this.rbtnSaveScreenCaptureImage.InitButtonWidth = 70;
            this.rbtnSaveScreenCaptureImage.IsChecked = false;
            this.rbtnSaveScreenCaptureImage.Location = new System.Drawing.Point(1060, 9);
            this.rbtnSaveScreenCaptureImage.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnSaveScreenCaptureImage.MouseOverImage = null;
            this.rbtnSaveScreenCaptureImage.Name = "rbtnSaveScreenCaptureImage";
            this.rbtnSaveScreenCaptureImage.NormalImage = global::RoadMan.Properties.Resources.저장_normal;
            this.rbtnSaveScreenCaptureImage.Owner = null;
            this.rbtnSaveScreenCaptureImage.Size = new System.Drawing.Size(70, 80);
            this.rbtnSaveScreenCaptureImage.TabIndex = 24;
            this.rbtnSaveScreenCaptureImage.Text = "파일저장";
            this.rbtnSaveScreenCaptureImage.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSaveScreenCaptureImage.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSaveScreenCaptureImage.ToolTipText = "파일저장";
            this.rbtnSaveScreenCaptureImage.UseCustomImageRect = true;
            this.rbtnSaveScreenCaptureImage.UseTextLocation = false;
            this.rbtnSaveScreenCaptureImage.UseVisualStyleBackColor = false;
            this.rbtnSaveScreenCaptureImage.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnSelectFullScreen
            // 
            this.rbtnSelectFullScreen.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSelectFullScreen.CheckButton = false;
            this.rbtnSelectFullScreen.CheckedBkgndImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnSelectFullScreen.CheckedImage = global::RoadMan.Properties.Resources.전체선택_normal;
            this.rbtnSelectFullScreen.ClickedBackgroundImage = null;
            this.rbtnSelectFullScreen.ClickedImage = null;
            this.rbtnSelectFullScreen.CustomImageRect = new System.Drawing.Rectangle(20, 8, 40, 40);
            this.rbtnSelectFullScreen.DisabledBkgndImage = null;
            this.rbtnSelectFullScreen.DisabledImage = global::RoadMan.Properties.Resources.전체선택_disable;
            this.rbtnSelectFullScreen.ID = -1;
            this.rbtnSelectFullScreen.InitButtonWidth = 70;
            this.rbtnSelectFullScreen.IsChecked = false;
            this.rbtnSelectFullScreen.Location = new System.Drawing.Point(978, 9);
            this.rbtnSelectFullScreen.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnSelectFullScreen.MouseOverImage = null;
            this.rbtnSelectFullScreen.Name = "rbtnSelectFullScreen";
            this.rbtnSelectFullScreen.NormalImage = global::RoadMan.Properties.Resources.전체선택_normal;
            this.rbtnSelectFullScreen.Owner = null;
            this.rbtnSelectFullScreen.Size = new System.Drawing.Size(85, 80);
            this.rbtnSelectFullScreen.TabIndex = 24;
            this.rbtnSelectFullScreen.Text = "전체화면 선택";
            this.rbtnSelectFullScreen.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSelectFullScreen.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSelectFullScreen.ToolTipText = "전체화면 선택";
            this.rbtnSelectFullScreen.UseCustomImageRect = true;
            this.rbtnSelectFullScreen.UseTextLocation = false;
            this.rbtnSelectFullScreen.UseVisualStyleBackColor = false;
            this.rbtnSelectFullScreen.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnMemo
            // 
            this.rbtnMemo.BackColor = System.Drawing.Color.Transparent;
            this.rbtnMemo.CheckButton = false;
            this.rbtnMemo.CheckedBkgndImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnMemo.CheckedImage = null;
            this.rbtnMemo.ClickedBackgroundImage = null;
            this.rbtnMemo.ClickedImage = null;
            this.rbtnMemo.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnMemo.DisabledBkgndImage = null;
            this.rbtnMemo.DisabledImage = global::RoadMan.Properties.Resources.memo_disable;
            this.rbtnMemo.Enabled = false;
            this.rbtnMemo.ID = -1;
            this.rbtnMemo.InitButtonWidth = 70;
            this.rbtnMemo.IsChecked = false;
            this.rbtnMemo.Location = new System.Drawing.Point(116, 9);
            this.rbtnMemo.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnMemo.MouseOverImage = null;
            this.rbtnMemo.Name = "rbtnMemo";
            this.rbtnMemo.NormalImage = global::RoadMan.Properties.Resources.memo_normal;
            this.rbtnMemo.Owner = null;
            this.rbtnMemo.Size = new System.Drawing.Size(70, 80);
            this.rbtnMemo.TabIndex = 24;
            this.rbtnMemo.Text = "메모";
            this.rbtnMemo.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnMemo.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnMemo.ToolTipText = "메모";
            this.rbtnMemo.UseCustomImageRect = true;
            this.rbtnMemo.UseTextLocation = false;
            this.rbtnMemo.UseVisualStyleBackColor = false;
            this.rbtnMemo.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnPrint
            // 
            this.rbtnPrint.BackColor = System.Drawing.Color.Transparent;
            this.rbtnPrint.CheckButton = false;
            this.rbtnPrint.CheckedBkgndImage = null;
            this.rbtnPrint.CheckedImage = null;
            this.rbtnPrint.ClickedBackgroundImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnPrint.ClickedImage = null;
            this.rbtnPrint.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnPrint.DisabledBkgndImage = null;
            this.rbtnPrint.DisabledImage = global::RoadMan.Properties.Resources.print_disable;
            this.rbtnPrint.Enabled = false;
            this.rbtnPrint.ID = -1;
            this.rbtnPrint.InitButtonWidth = 70;
            this.rbtnPrint.IsChecked = false;
            this.rbtnPrint.Location = new System.Drawing.Point(882, 9);
            this.rbtnPrint.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnPrint.MouseOverImage = null;
            this.rbtnPrint.Name = "rbtnPrint";
            this.rbtnPrint.NormalImage = global::RoadMan.Properties.Resources.print_normal;
            this.rbtnPrint.Owner = null;
            this.rbtnPrint.Size = new System.Drawing.Size(70, 80);
            this.rbtnPrint.TabIndex = 24;
            this.rbtnPrint.Text = "인쇄";
            this.rbtnPrint.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnPrint.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnPrint.ToolTipText = "인쇄";
            this.rbtnPrint.UseCustomImageRect = true;
            this.rbtnPrint.UseTextLocation = false;
            this.rbtnPrint.UseVisualStyleBackColor = false;
            this.rbtnPrint.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnOptions
            // 
            this.rbtnOptions.BackColor = System.Drawing.Color.Transparent;
            this.rbtnOptions.CheckButton = false;
            this.rbtnOptions.CheckedBkgndImage = null;
            this.rbtnOptions.CheckedImage = null;
            this.rbtnOptions.ClickedBackgroundImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnOptions.ClickedImage = null;
            this.rbtnOptions.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnOptions.DisabledBkgndImage = null;
            this.rbtnOptions.DisabledImage = global::RoadMan.Properties.Resources.option_disable;
            this.rbtnOptions.Enabled = false;
            this.rbtnOptions.ID = -1;
            this.rbtnOptions.InitButtonWidth = 70;
            this.rbtnOptions.IsChecked = false;
            this.rbtnOptions.Location = new System.Drawing.Point(811, 9);
            this.rbtnOptions.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnOptions.MouseOverImage = null;
            this.rbtnOptions.Name = "rbtnOptions";
            this.rbtnOptions.NormalImage = global::RoadMan.Properties.Resources.option_normal;
            this.rbtnOptions.Owner = null;
            this.rbtnOptions.Size = new System.Drawing.Size(70, 80);
            this.rbtnOptions.TabIndex = 24;
            this.rbtnOptions.Text = "설정";
            this.rbtnOptions.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnOptions.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnOptions.ToolTipText = "설정";
            this.rbtnOptions.UseCustomImageRect = true;
            this.rbtnOptions.UseTextLocation = false;
            this.rbtnOptions.UseVisualStyleBackColor = false;
            this.rbtnOptions.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnScreenShot
            // 
            this.rbtnScreenShot.BackColor = System.Drawing.Color.Transparent;
            this.rbtnScreenShot.CheckButton = false;
            this.rbtnScreenShot.CheckedBkgndImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnScreenShot.CheckedImage = null;
            this.rbtnScreenShot.ClickedBackgroundImage = null;
            this.rbtnScreenShot.ClickedImage = null;
            this.rbtnScreenShot.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnScreenShot.DisabledBkgndImage = null;
            this.rbtnScreenShot.DisabledImage = global::RoadMan.Properties.Resources.screenshot_disable;
            this.rbtnScreenShot.Enabled = false;
            this.rbtnScreenShot.ID = -1;
            this.rbtnScreenShot.InitButtonWidth = 70;
            this.rbtnScreenShot.IsChecked = false;
            this.rbtnScreenShot.Location = new System.Drawing.Point(740, 9);
            this.rbtnScreenShot.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnScreenShot.MouseOverImage = null;
            this.rbtnScreenShot.Name = "rbtnScreenShot";
            this.rbtnScreenShot.NormalImage = global::RoadMan.Properties.Resources.screenshot_normal;
            this.rbtnScreenShot.Owner = null;
            this.rbtnScreenShot.Size = new System.Drawing.Size(70, 80);
            this.rbtnScreenShot.TabIndex = 24;
            this.rbtnScreenShot.Text = "화면캡쳐";
            this.rbtnScreenShot.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnScreenShot.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnScreenShot.ToolTipText = "화면캡쳐";
            this.rbtnScreenShot.UseCustomImageRect = true;
            this.rbtnScreenShot.UseTextLocation = false;
            this.rbtnScreenShot.UseVisualStyleBackColor = false;
            this.rbtnScreenShot.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnLayer
            // 
            this.rbtnLayer.BackColor = System.Drawing.Color.Transparent;
            this.rbtnLayer.CheckButton = false;
            this.rbtnLayer.CheckedBkgndImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnLayer.CheckedImage = null;
            this.rbtnLayer.ClickedBackgroundImage = null;
            this.rbtnLayer.ClickedImage = null;
            this.rbtnLayer.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnLayer.DisabledBkgndImage = null;
            this.rbtnLayer.DisabledImage = global::RoadMan.Properties.Resources.layer_disable;
            this.rbtnLayer.Enabled = false;
            this.rbtnLayer.ID = -1;
            this.rbtnLayer.InitButtonWidth = 70;
            this.rbtnLayer.IsChecked = false;
            this.rbtnLayer.Location = new System.Drawing.Point(271, 9);
            this.rbtnLayer.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnLayer.MouseOverImage = null;
            this.rbtnLayer.Name = "rbtnLayer";
            this.rbtnLayer.NormalImage = global::RoadMan.Properties.Resources.layer_normal;
            this.rbtnLayer.Owner = null;
            this.rbtnLayer.Size = new System.Drawing.Size(70, 80);
            this.rbtnLayer.TabIndex = 24;
            this.rbtnLayer.Text = "도면층";
            this.rbtnLayer.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnLayer.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnLayer.ToolTipText = "도면층";
            this.rbtnLayer.UseCustomImageRect = true;
            this.rbtnLayer.UseTextLocation = false;
            this.rbtnLayer.UseVisualStyleBackColor = false;
            this.rbtnLayer.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnProcessSchedule
            // 
            this.rbtnProcessSchedule.BackColor = System.Drawing.Color.Transparent;
            this.rbtnProcessSchedule.CheckButton = false;
            this.rbtnProcessSchedule.CheckedBkgndImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnProcessSchedule.CheckedImage = null;
            this.rbtnProcessSchedule.ClickedBackgroundImage = null;
            this.rbtnProcessSchedule.ClickedImage = null;
            this.rbtnProcessSchedule.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnProcessSchedule.DisabledBkgndImage = null;
            this.rbtnProcessSchedule.DisabledImage = global::RoadMan.Properties.Resources.process_schedule_disable;
            this.rbtnProcessSchedule.Enabled = false;
            this.rbtnProcessSchedule.ID = -1;
            this.rbtnProcessSchedule.InitButtonWidth = 70;
            this.rbtnProcessSchedule.IsChecked = false;
            this.rbtnProcessSchedule.Location = new System.Drawing.Point(416, 9);
            this.rbtnProcessSchedule.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnProcessSchedule.MouseOverImage = null;
            this.rbtnProcessSchedule.Name = "rbtnProcessSchedule";
            this.rbtnProcessSchedule.NormalImage = global::RoadMan.Properties.Resources.process_schedule_normal;
            this.rbtnProcessSchedule.Owner = null;
            this.rbtnProcessSchedule.Size = new System.Drawing.Size(70, 80);
            this.rbtnProcessSchedule.TabIndex = 25;
            this.rbtnProcessSchedule.Text = "집행계획";
            this.rbtnProcessSchedule.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnProcessSchedule.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnProcessSchedule.ToolTipText = "집행계획";
            this.rbtnProcessSchedule.UseCustomImageRect = true;
            this.rbtnProcessSchedule.UseTextLocation = false;
            this.rbtnProcessSchedule.UseVisualStyleBackColor = false;
            this.rbtnProcessSchedule.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // rbtnProcessResult
            // 
            this.rbtnProcessResult.BackColor = System.Drawing.Color.Transparent;
            this.rbtnProcessResult.CheckButton = false;
            this.rbtnProcessResult.CheckedBkgndImage = global::RoadMan.Properties.Resources.clicked_background;
            this.rbtnProcessResult.CheckedImage = null;
            this.rbtnProcessResult.ClickedBackgroundImage = null;
            this.rbtnProcessResult.ClickedImage = null;
            this.rbtnProcessResult.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnProcessResult.DisabledBkgndImage = null;
            this.rbtnProcessResult.DisabledImage = global::RoadMan.Properties.Resources.process_result_disable;
            this.rbtnProcessResult.Enabled = false;
            this.rbtnProcessResult.ID = -1;
            this.rbtnProcessResult.InitButtonWidth = 70;
            this.rbtnProcessResult.IsChecked = false;
            this.rbtnProcessResult.Location = new System.Drawing.Point(487, 9);
            this.rbtnProcessResult.MouseOverBkgndImage = global::RoadMan.Properties.Resources.mouse_over_background;
            this.rbtnProcessResult.MouseOverImage = null;
            this.rbtnProcessResult.Name = "rbtnProcessResult";
            this.rbtnProcessResult.NormalImage = global::RoadMan.Properties.Resources.process_result_normal;
            this.rbtnProcessResult.Owner = null;
            this.rbtnProcessResult.Size = new System.Drawing.Size(81, 80);
            this.rbtnProcessResult.TabIndex = 26;
            this.rbtnProcessResult.Text = "집행진행상황";
            this.rbtnProcessResult.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnProcessResult.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnProcessResult.ToolTipText = "집행진행상황";
            this.rbtnProcessResult.UseCustomImageRect = true;
            this.rbtnProcessResult.UseTextLocation = false;
            this.rbtnProcessResult.UseVisualStyleBackColor = false;
            this.rbtnProcessResult.Click += new System.EventHandler(this.toolBarButton_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::RoadMan.Properties.Resources.skin_line_img;
            this.pictureBox4.Location = new System.Drawing.Point(732, 5);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(2, 90);
            this.pictureBox4.TabIndex = 21;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::RoadMan.Properties.Resources.skin_line_img;
            this.pictureBox2.Location = new System.Drawing.Point(574, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(2, 90);
            this.pictureBox2.TabIndex = 21;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadMan.Properties.Resources.skin_line_img;
            this.pictureBox1.Location = new System.Drawing.Point(268, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(2, 90);
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::RoadMan.Properties.Resources.skin_line_img;
            this.pictureBox3.Location = new System.Drawing.Point(30, 5);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(2, 90);
            this.pictureBox3.TabIndex = 21;
            this.pictureBox3.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLabelStatusWork,
            this.tsLabelCoord,
            this.tsLabelClock,
            this.tsLabelCaps,
            this.tsLabelNum,
            this.tsLabelHangul,
            this.tsLabelProgress,
            this.tsProgressBar,
            this.tsLabelCompany});
            this.statusStrip1.Location = new System.Drawing.Point(0, 744);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1223, 24);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsLabelStatusWork
            // 
            this.tsLabelStatusWork.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsLabelStatusWork.ForeColor = System.Drawing.Color.White;
            this.tsLabelStatusWork.Name = "tsLabelStatusWork";
            this.tsLabelStatusWork.Size = new System.Drawing.Size(733, 19);
            this.tsLabelStatusWork.Spring = true;
            this.tsLabelStatusWork.Text = "현재 작업을 표시합니다.";
            this.tsLabelStatusWork.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsLabelCoord
            // 
            this.tsLabelCoord.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsLabelCoord.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsLabelCoord.ForeColor = System.Drawing.Color.White;
            this.tsLabelCoord.Name = "tsLabelCoord";
            this.tsLabelCoord.Size = new System.Drawing.Size(4, 19);
            // 
            // tsLabelClock
            // 
            this.tsLabelClock.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsLabelClock.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsLabelClock.ForeColor = System.Drawing.Color.White;
            this.tsLabelClock.Name = "tsLabelClock";
            this.tsLabelClock.Size = new System.Drawing.Size(59, 19);
            this.tsLabelClock.Text = "현재시간";
            this.tsLabelClock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsLabelCaps
            // 
            this.tsLabelCaps.AutoSize = false;
            this.tsLabelCaps.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsLabelCaps.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsLabelCaps.ForeColor = System.Drawing.Color.White;
            this.tsLabelCaps.Name = "tsLabelCaps";
            this.tsLabelCaps.Size = new System.Drawing.Size(41, 19);
            this.tsLabelCaps.Text = "CAPS";
            // 
            // tsLabelNum
            // 
            this.tsLabelNum.AutoSize = false;
            this.tsLabelNum.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsLabelNum.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsLabelNum.ForeColor = System.Drawing.Color.White;
            this.tsLabelNum.Name = "tsLabelNum";
            this.tsLabelNum.Size = new System.Drawing.Size(39, 19);
            this.tsLabelNum.Text = "NUM";
            // 
            // tsLabelHangul
            // 
            this.tsLabelHangul.AutoSize = false;
            this.tsLabelHangul.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsLabelHangul.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsLabelHangul.ForeColor = System.Drawing.Color.White;
            this.tsLabelHangul.Name = "tsLabelHangul";
            this.tsLabelHangul.Size = new System.Drawing.Size(40, 19);
            this.tsLabelHangul.Text = "한/영";
            // 
            // tsLabelProgress
            // 
            this.tsLabelProgress.AutoSize = false;
            this.tsLabelProgress.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsLabelProgress.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsLabelProgress.ForeColor = System.Drawing.Color.White;
            this.tsLabelProgress.Name = "tsLabelProgress";
            this.tsLabelProgress.Size = new System.Drawing.Size(59, 19);
            this.tsLabelProgress.Text = "진행상황";
            // 
            // tsProgressBar
            // 
            this.tsProgressBar.AutoSize = false;
            this.tsProgressBar.Name = "tsProgressBar";
            this.tsProgressBar.Size = new System.Drawing.Size(100, 18);
            // 
            // tsLabelCompany
            // 
            this.tsLabelCompany.AutoSize = false;
            this.tsLabelCompany.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsLabelCompany.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsLabelCompany.ForeColor = System.Drawing.Color.White;
            this.tsLabelCompany.Name = "tsLabelCompany";
            this.tsLabelCompany.Size = new System.Drawing.Size(100, 19);
            this.tsLabelCompany.Text = "  UPlan.Co.Ltd.";
            // 
            // statusClockTimer
            // 
            this.statusClockTimer.Interval = 1000;
            this.statusClockTimer.Tick += new System.EventHandler(this.statusClockTimer_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddTabToLeft,
            this.menuAddTabToRight,
            this.menuDeleteTab,
            this.menuMoveToLeft,
            this.menuMoveToRight});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(179, 114);
            // 
            // menuAddTabToLeft
            // 
            this.menuAddTabToLeft.Name = "menuAddTabToLeft";
            this.menuAddTabToLeft.Size = new System.Drawing.Size(178, 22);
            this.menuAddTabToLeft.Text = "왼쪽에 탭 추가";
            this.menuAddTabToLeft.Click += new System.EventHandler(this.menuAddTabToLeft_Click);
            // 
            // menuAddTabToRight
            // 
            this.menuAddTabToRight.Name = "menuAddTabToRight";
            this.menuAddTabToRight.Size = new System.Drawing.Size(178, 22);
            this.menuAddTabToRight.Text = "오른쪽에 탭 추가";
            this.menuAddTabToRight.Click += new System.EventHandler(this.menuAddTabToRight_Click);
            // 
            // menuDeleteTab
            // 
            this.menuDeleteTab.Name = "menuDeleteTab";
            this.menuDeleteTab.Size = new System.Drawing.Size(178, 22);
            this.menuDeleteTab.Text = "탭 삭제";
            this.menuDeleteTab.Click += new System.EventHandler(this.menuDeleteTab_Click);
            // 
            // menuMoveToLeft
            // 
            this.menuMoveToLeft.Name = "menuMoveToLeft";
            this.menuMoveToLeft.Size = new System.Drawing.Size(178, 22);
            this.menuMoveToLeft.Text = "왼쪽으로 탭 이동";
            this.menuMoveToLeft.Click += new System.EventHandler(this.menuMoveToLeft_Click);
            // 
            // menuMoveToRight
            // 
            this.menuMoveToRight.Name = "menuMoveToRight";
            this.menuMoveToRight.Size = new System.Drawing.Size(178, 22);
            this.menuMoveToRight.Text = "오른쪽으로 탭 이동";
            this.menuMoveToRight.Click += new System.EventHandler(this.menuMoveToRight_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // updateCmdTimer
            // 
            this.updateCmdTimer.Interval = 300;
            this.updateCmdTimer.Tick += new System.EventHandler(this.updateCmdTimer_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1223, 768);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.paneRibbonToolBar);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.LocationChanged += new System.EventHandler(this.FormMain_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).EndInit();
            this.splitContainerLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeftDown)).EndInit();
            this.splitContainerLeftDown.ResumeLayout(false);
            this.panelLeftToolBar.ResumeLayout(false);
            this.paneRibbonToolBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 파일FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 보기VToolStripMenuItem;
        private System.Windows.Forms.Panel paneRibbonToolBar;
        private System.Windows.Forms.ToolStripMenuItem menuNewProject;
        private System.Windows.Forms.ToolStripMenuItem menuOpenProject;
        private System.Windows.Forms.ToolStripMenuItem menuSaveProject;
        private System.Windows.Forms.ToolStripMenuItem menuSaveAsProject;
        private System.Windows.Forms.ToolStripMenuItem menuLayer;
        private System.Windows.Forms.ToolStripMenuItem 도구EToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerLeft;
        private System.Windows.Forms.SplitContainer splitContainerLeftDown;
        private System.Windows.Forms.PictureBox pictureBox3;
        private UnE.GUI.RibbonButton rbtnUndo;
        private UnE.GUI.RibbonButton rbtnRedo;
        private UnE.GUI.RibbonButton rbtnLayer;
        private UnE.GUI.RibbonButton rbtnProcessSchedule;
        private UnE.GUI.RibbonButton rbtnProcessResult;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuPrint;
        private System.Windows.Forms.ToolStripMenuItem menuProcessSchedule;
        private System.Windows.Forms.ToolStripMenuItem menuProcessResult;
        private System.Windows.Forms.ToolStripMenuItem 편집ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuUndo;
        private System.Windows.Forms.ToolStripMenuItem menuRedo;
        private UnE.GUI.RibbonButton rbtnPrint;
        private UnE.GUI.RibbonButton rbtnOptions;
        private UnE.GUI.RibbonButton rbtnScreenShot;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsLabelStatusWork;
        private System.Windows.Forms.ToolStripStatusLabel tsLabelClock;
        private System.Windows.Forms.ToolStripStatusLabel tsLabelCaps;
        private System.Windows.Forms.ToolStripStatusLabel tsLabelNum;
        private System.Windows.Forms.ToolStripStatusLabel tsLabelHangul;
        private System.Windows.Forms.ToolStripStatusLabel tsLabelProgress;
        private System.Windows.Forms.ToolStripProgressBar tsProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel tsLabelCompany;
        private System.Windows.Forms.ToolStripStatusLabel tsLabelCoord;
        private System.Windows.Forms.Timer statusClockTimer;
        private UnE.GUI.RibbonButton rbtnProcessLayer;
        private UnE.GUI.RibbonButton rbtnReport;
        private UnE.GUI.RibbonButton rbtnMemo;
        private System.Windows.Forms.PictureBox pictureBox4;
        private UnE.Controls.TabControlEx tabControlEx1;
        private UnE.GUI.RibbonButton rbtnCloseScreenCapture;
        private UnE.GUI.RibbonButton rbtnSaveScreenCaptureImage;
        private UnE.GUI.RibbonButton rbtnSelectFullScreen;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuAddTabToLeft;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteTab;
        private System.Windows.Forms.ToolStripMenuItem menuMoveToLeft;
        private System.Windows.Forms.ToolStripMenuItem menuMoveToRight;
        private System.Windows.Forms.ToolStripMenuItem menuAddTabToRight;
		private UnE.GUI.RibbonButton rbtnSearch;
		private System.Windows.Forms.Panel panelLeftToolBar;
		private UnE.GUI.RibbonButton rbtnShowPanel;
		private UnE.GUI.RibbonButton rbtnHidePanel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem menuProcessLayer;
        private System.Windows.Forms.ToolStripMenuItem menuMemo;
        private System.Windows.Forms.ToolStripMenuItem menuReport;
        private System.Windows.Forms.ToolStripMenuItem menuScreenCapture;
        private System.Windows.Forms.ToolStripMenuItem menuOption;
        private System.Windows.Forms.ToolStripMenuItem 도움말HToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuSearch;
		private System.Windows.Forms.Timer updateCmdTimer;
    }
}

