namespace SoilMan
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsMenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.프로젝트열기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.프로젝트저장ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.다른이름으로저장ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.수치지도열기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.토지이용계획도열기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.지적도열기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuRemoveSelectedShapes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuRemoveUnselectedShapes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuOption = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuSelectedColor = new System.Windows.Forms.ToolStripMenuItem();
            this.panelLeftToolBar = new System.Windows.Forms.Panel();
            this.rbtnShowPanel = new UnE.GUI.RibbonButton();
            this.rbtnHidePanel = new UnE.GUI.RibbonButton();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerLeft = new System.Windows.Forms.SplitContainer();
            this.splitContainerLeftDown = new System.Windows.Forms.SplitContainer();
            this.tabCtrlSystemConst = new System.Windows.Forms.TabControl();
            this.tabPage계량지표 = new System.Windows.Forms.TabPage();
            this.tabPage화폐화지표 = new System.Windows.Forms.TabPage();
            this.tabPage기능회복율 = new System.Windows.Forms.TabPage();
            this.tabPage기능회복기간 = new System.Windows.Forms.TabPage();
            this.tabPage단가 = new System.Windows.Forms.TabPage();
            this.tabPage지불의사액 = new System.Windows.Forms.TabPage();
            this.tabPage지역별가구수및면적 = new System.Windows.Forms.TabPage();
            this.tabPage비사용가치 = new System.Windows.Forms.TabPage();
            this.panelProjectTitle = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsDXFCoord = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.panelRibbonToolbar = new System.Windows.Forms.Panel();
            this.mUpdataeTmr = new System.Windows.Forms.Timer(this.components);
            this.tabPage스티그마 = new System.Windows.Forms.TabPage();
            this.menuStrip1.SuspendLayout();
            this.panelLeftToolBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).BeginInit();
            this.splitContainerLeft.Panel2.SuspendLayout();
            this.splitContainerLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeftDown)).BeginInit();
            this.splitContainerLeftDown.SuspendLayout();
            this.tabCtrlSystemConst.SuspendLayout();
            this.panelProjectTitle.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuFile,
            this.tsMenuEdit,
            this.tsMenuOption});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(862, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // tsMenuFile
            // 
            this.tsMenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.프로젝트열기ToolStripMenuItem,
            this.프로젝트저장ToolStripMenuItem,
            this.다른이름으로저장ToolStripMenuItem,
            this.수치지도열기ToolStripMenuItem,
            this.토지이용계획도열기ToolStripMenuItem,
            this.지적도열기ToolStripMenuItem});
            this.tsMenuFile.ForeColor = System.Drawing.Color.White;
            this.tsMenuFile.Name = "tsMenuFile";
            this.tsMenuFile.Size = new System.Drawing.Size(57, 20);
            this.tsMenuFile.Text = "파일(&F)";
            // 
            // 프로젝트열기ToolStripMenuItem
            // 
            this.프로젝트열기ToolStripMenuItem.Name = "프로젝트열기ToolStripMenuItem";
            this.프로젝트열기ToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.프로젝트열기ToolStripMenuItem.Text = "프로젝트 열기";
            this.프로젝트열기ToolStripMenuItem.Click += new System.EventHandler(this.프로젝트열기ToolStripMenuItem_Click);
            // 
            // 프로젝트저장ToolStripMenuItem
            // 
            this.프로젝트저장ToolStripMenuItem.Name = "프로젝트저장ToolStripMenuItem";
            this.프로젝트저장ToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.프로젝트저장ToolStripMenuItem.Text = "프로젝트 저장";
            this.프로젝트저장ToolStripMenuItem.Click += new System.EventHandler(this.프로젝트저장ToolStripMenuItem_Click);
            // 
            // 다른이름으로저장ToolStripMenuItem
            // 
            this.다른이름으로저장ToolStripMenuItem.Name = "다른이름으로저장ToolStripMenuItem";
            this.다른이름으로저장ToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.다른이름으로저장ToolStripMenuItem.Text = "다른이름으로 저장";
            this.다른이름으로저장ToolStripMenuItem.Click += new System.EventHandler(this.다른이름으로저장ToolStripMenuItem_Click);
            // 
            // 수치지도열기ToolStripMenuItem
            // 
            this.수치지도열기ToolStripMenuItem.Name = "수치지도열기ToolStripMenuItem";
            this.수치지도열기ToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.수치지도열기ToolStripMenuItem.Text = "수치지도 열기";
            this.수치지도열기ToolStripMenuItem.Click += new System.EventHandler(this.수치지도열기ToolStripMenuItem_Click);
            // 
            // 토지이용계획도열기ToolStripMenuItem
            // 
            this.토지이용계획도열기ToolStripMenuItem.Name = "토지이용계획도열기ToolStripMenuItem";
            this.토지이용계획도열기ToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.토지이용계획도열기ToolStripMenuItem.Text = "토지이용 계획도 열기";
            this.토지이용계획도열기ToolStripMenuItem.Click += new System.EventHandler(this.토지이용계획도열기ToolStripMenuItem_Click);
            // 
            // 지적도열기ToolStripMenuItem
            // 
            this.지적도열기ToolStripMenuItem.Name = "지적도열기ToolStripMenuItem";
            this.지적도열기ToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.지적도열기ToolStripMenuItem.Text = "지적도 열기";
            this.지적도열기ToolStripMenuItem.Click += new System.EventHandler(this.지적도열기ToolStripMenuItem_Click);
            // 
            // tsMenuEdit
            // 
            this.tsMenuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuRemoveSelectedShapes,
            this.tsMenuRemoveUnselectedShapes,
            this.tsMenuSelect});
            this.tsMenuEdit.ForeColor = System.Drawing.Color.White;
            this.tsMenuEdit.Name = "tsMenuEdit";
            this.tsMenuEdit.Size = new System.Drawing.Size(74, 20);
            this.tsMenuEdit.Text = "마우스(&M)";
            // 
            // tsMenuRemoveSelectedShapes
            // 
            this.tsMenuRemoveSelectedShapes.Name = "tsMenuRemoveSelectedShapes";
            this.tsMenuRemoveSelectedShapes.Size = new System.Drawing.Size(284, 22);
            this.tsMenuRemoveSelectedShapes.Text = "선택된 영역의 Shape Data 삭제";
            this.tsMenuRemoveSelectedShapes.Click += new System.EventHandler(this.tsMenuRemoveSelectedShapes_Click);
            // 
            // tsMenuRemoveUnselectedShapes
            // 
            this.tsMenuRemoveUnselectedShapes.Name = "tsMenuRemoveUnselectedShapes";
            this.tsMenuRemoveUnselectedShapes.Size = new System.Drawing.Size(284, 22);
            this.tsMenuRemoveUnselectedShapes.Text = "선택되지 않은 영역의 Shape Data 삭제";
            this.tsMenuRemoveUnselectedShapes.Click += new System.EventHandler(this.tsMenuRemoveUnselectedShapes_Click);
            // 
            // tsMenuSelect
            // 
            this.tsMenuSelect.Checked = true;
            this.tsMenuSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsMenuSelect.Name = "tsMenuSelect";
            this.tsMenuSelect.Size = new System.Drawing.Size(284, 22);
            this.tsMenuSelect.Text = "선택";
            this.tsMenuSelect.Click += new System.EventHandler(this.tsMenuSelect_Click);
            // 
            // tsMenuOption
            // 
            this.tsMenuOption.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuSelectedColor});
            this.tsMenuOption.ForeColor = System.Drawing.Color.White;
            this.tsMenuOption.Name = "tsMenuOption";
            this.tsMenuOption.Size = new System.Drawing.Size(60, 20);
            this.tsMenuOption.Text = "옵션(&O)";
            // 
            // tsMenuSelectedColor
            // 
            this.tsMenuSelectedColor.Name = "tsMenuSelectedColor";
            this.tsMenuSelectedColor.Size = new System.Drawing.Size(150, 22);
            this.tsMenuSelectedColor.Text = "선택영역 색상";
            this.tsMenuSelectedColor.Click += new System.EventHandler(this.tsMenuSelectedColor_Click);
            // 
            // panelLeftToolBar
            // 
            this.panelLeftToolBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.panelLeftToolBar.Controls.Add(this.rbtnShowPanel);
            this.panelLeftToolBar.Controls.Add(this.rbtnHidePanel);
            this.panelLeftToolBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeftToolBar.Location = new System.Drawing.Point(0, 100);
            this.panelLeftToolBar.Name = "panelLeftToolBar";
            this.panelLeftToolBar.Size = new System.Drawing.Size(40, 455);
            this.panelLeftToolBar.TabIndex = 2;
            // 
            // rbtnShowPanel
            // 
            this.rbtnShowPanel.BackColor = System.Drawing.Color.Transparent;
            this.rbtnShowPanel.CheckButton = false;
            this.rbtnShowPanel.CheckedBkgndImage = null;
            this.rbtnShowPanel.CheckedImage = null;
            this.rbtnShowPanel.ClickedBackgroundImage = global::SoilMan.Properties.Resources.clicked_background;
            this.rbtnShowPanel.ClickedImage = global::SoilMan.Properties.Resources.right_arrow_click;
            this.rbtnShowPanel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 40);
            this.rbtnShowPanel.DisabledBkgndImage = null;
            this.rbtnShowPanel.DisabledImage = global::SoilMan.Properties.Resources.right_arrow_disable;
            this.rbtnShowPanel.ID = -1;
            this.rbtnShowPanel.InitButtonWidth = 40;
            this.rbtnShowPanel.IsChecked = false;
            this.rbtnShowPanel.Location = new System.Drawing.Point(0, 61);
            this.rbtnShowPanel.MouseOverBkgndImage = global::SoilMan.Properties.Resources.mouse_over_background;
            this.rbtnShowPanel.MouseOverImage = global::SoilMan.Properties.Resources.right_arrow_over;
            this.rbtnShowPanel.Name = "rbtnShowPanel";
            this.rbtnShowPanel.NormalImage = global::SoilMan.Properties.Resources.right_arrow_normal;
            this.rbtnShowPanel.Owner = null;
            this.rbtnShowPanel.Size = new System.Drawing.Size(40, 40);
            this.rbtnShowPanel.TabIndex = 30;
            this.rbtnShowPanel.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnShowPanel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnShowPanel.ToolTipText = "";
            this.rbtnShowPanel.UseCustomImageRect = true;
            this.rbtnShowPanel.UseTextLocation = false;
            this.rbtnShowPanel.UseVisualStyleBackColor = false;
            this.rbtnShowPanel.Click += new System.EventHandler(this.rbtnShowPanel_Click);
            // 
            // rbtnHidePanel
            // 
            this.rbtnHidePanel.BackColor = System.Drawing.Color.Transparent;
            this.rbtnHidePanel.CheckButton = false;
            this.rbtnHidePanel.CheckedBkgndImage = null;
            this.rbtnHidePanel.CheckedImage = null;
            this.rbtnHidePanel.ClickedBackgroundImage = global::SoilMan.Properties.Resources.clicked_background;
            this.rbtnHidePanel.ClickedImage = global::SoilMan.Properties.Resources.left_arrow_click;
            this.rbtnHidePanel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 40);
            this.rbtnHidePanel.DisabledBkgndImage = null;
            this.rbtnHidePanel.DisabledImage = global::SoilMan.Properties.Resources.left_arrow_disable;
            this.rbtnHidePanel.ID = -1;
            this.rbtnHidePanel.InitButtonWidth = 40;
            this.rbtnHidePanel.IsChecked = false;
            this.rbtnHidePanel.Location = new System.Drawing.Point(0, 1);
            this.rbtnHidePanel.MouseOverBkgndImage = global::SoilMan.Properties.Resources.mouse_over_background;
            this.rbtnHidePanel.MouseOverImage = global::SoilMan.Properties.Resources.left_arrow_over;
            this.rbtnHidePanel.Name = "rbtnHidePanel";
            this.rbtnHidePanel.NormalImage = global::SoilMan.Properties.Resources.left_arrow_normal;
            this.rbtnHidePanel.Owner = null;
            this.rbtnHidePanel.Size = new System.Drawing.Size(40, 40);
            this.rbtnHidePanel.TabIndex = 30;
            this.rbtnHidePanel.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnHidePanel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnHidePanel.ToolTipText = "";
            this.rbtnHidePanel.UseCustomImageRect = true;
            this.rbtnHidePanel.UseTextLocation = false;
            this.rbtnHidePanel.UseVisualStyleBackColor = false;
            this.rbtnHidePanel.Click += new System.EventHandler(this.rbtnHidePanel_Click);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(40, 100);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerLeft);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.splitContainerMain.Panel2.Controls.Add(this.tabCtrlSystemConst);
            this.splitContainerMain.Panel2.Controls.Add(this.panelProjectTitle);
            this.splitContainerMain.Size = new System.Drawing.Size(822, 455);
            this.splitContainerMain.SplitterDistance = 274;
            this.splitContainerMain.TabIndex = 3;
            this.splitContainerMain.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.splitContainerMain_SplitterMoving);
            this.splitContainerMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerMain_SplitterMoved);
            // 
            // splitContainerLeft
            // 
            this.splitContainerLeft.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.splitContainerLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLeft.Name = "splitContainerLeft";
            this.splitContainerLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerLeft.Panel1
            // 
            this.splitContainerLeft.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            // 
            // splitContainerLeft.Panel2
            // 
            this.splitContainerLeft.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.splitContainerLeft.Panel2.Controls.Add(this.splitContainerLeftDown);
            this.splitContainerLeft.Size = new System.Drawing.Size(274, 455);
            this.splitContainerLeft.SplitterDistance = 207;
            this.splitContainerLeft.TabIndex = 0;
            // 
            // splitContainerLeftDown
            // 
            this.splitContainerLeftDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeftDown.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLeftDown.Name = "splitContainerLeftDown";
            this.splitContainerLeftDown.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerLeftDown.Size = new System.Drawing.Size(274, 244);
            this.splitContainerLeftDown.SplitterDistance = 96;
            this.splitContainerLeftDown.TabIndex = 0;
            // 
            // tabCtrlSystemConst
            // 
            this.tabCtrlSystemConst.Controls.Add(this.tabPage계량지표);
            this.tabCtrlSystemConst.Controls.Add(this.tabPage화폐화지표);
            this.tabCtrlSystemConst.Controls.Add(this.tabPage기능회복율);
            this.tabCtrlSystemConst.Controls.Add(this.tabPage기능회복기간);
            this.tabCtrlSystemConst.Controls.Add(this.tabPage단가);
            this.tabCtrlSystemConst.Controls.Add(this.tabPage지불의사액);
            this.tabCtrlSystemConst.Controls.Add(this.tabPage지역별가구수및면적);
            this.tabCtrlSystemConst.Controls.Add(this.tabPage비사용가치);
            this.tabCtrlSystemConst.Controls.Add(this.tabPage스티그마);
            this.tabCtrlSystemConst.Location = new System.Drawing.Point(35, 61);
            this.tabCtrlSystemConst.Name = "tabCtrlSystemConst";
            this.tabCtrlSystemConst.SelectedIndex = 0;
            this.tabCtrlSystemConst.Size = new System.Drawing.Size(465, 316);
            this.tabCtrlSystemConst.TabIndex = 1;
            this.tabCtrlSystemConst.SelectedIndexChanged += new System.EventHandler(this.tabCtrlSystemConst_SelectedIndexChanged);
            // 
            // tabPage계량지표
            // 
            this.tabPage계량지표.Location = new System.Drawing.Point(4, 22);
            this.tabPage계량지표.Name = "tabPage계량지표";
            this.tabPage계량지표.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage계량지표.Size = new System.Drawing.Size(457, 290);
            this.tabPage계량지표.TabIndex = 0;
            this.tabPage계량지표.Text = "기능별 계량지표";
            this.tabPage계량지표.UseVisualStyleBackColor = true;
            // 
            // tabPage화폐화지표
            // 
            this.tabPage화폐화지표.Location = new System.Drawing.Point(4, 22);
            this.tabPage화폐화지표.Name = "tabPage화폐화지표";
            this.tabPage화폐화지표.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage화폐화지표.Size = new System.Drawing.Size(457, 290);
            this.tabPage화폐화지표.TabIndex = 1;
            this.tabPage화폐화지표.Text = "기능별 화폐화지표";
            this.tabPage화폐화지표.UseVisualStyleBackColor = true;
            // 
            // tabPage기능회복율
            // 
            this.tabPage기능회복율.Location = new System.Drawing.Point(4, 22);
            this.tabPage기능회복율.Name = "tabPage기능회복율";
            this.tabPage기능회복율.Size = new System.Drawing.Size(457, 290);
            this.tabPage기능회복율.TabIndex = 2;
            this.tabPage기능회복율.Text = "토양정화기술별 기능회복율";
            this.tabPage기능회복율.UseVisualStyleBackColor = true;
            // 
            // tabPage기능회복기간
            // 
            this.tabPage기능회복기간.Location = new System.Drawing.Point(4, 22);
            this.tabPage기능회복기간.Name = "tabPage기능회복기간";
            this.tabPage기능회복기간.Size = new System.Drawing.Size(457, 290);
            this.tabPage기능회복기간.TabIndex = 3;
            this.tabPage기능회복기간.Text = "토양정화기술별 기능회복기간";
            this.tabPage기능회복기간.UseVisualStyleBackColor = true;
            // 
            // tabPage단가
            // 
            this.tabPage단가.Location = new System.Drawing.Point(4, 22);
            this.tabPage단가.Name = "tabPage단가";
            this.tabPage단가.Size = new System.Drawing.Size(457, 290);
            this.tabPage단가.TabIndex = 4;
            this.tabPage단가.Text = "토양정화기술 단가";
            this.tabPage단가.UseVisualStyleBackColor = true;
            // 
            // tabPage지불의사액
            // 
            this.tabPage지불의사액.Location = new System.Drawing.Point(4, 22);
            this.tabPage지불의사액.Name = "tabPage지불의사액";
            this.tabPage지불의사액.Size = new System.Drawing.Size(457, 290);
            this.tabPage지불의사액.TabIndex = 5;
            this.tabPage지불의사액.Text = "지불의사액";
            this.tabPage지불의사액.UseVisualStyleBackColor = true;
            // 
            // tabPage지역별가구수및면적
            // 
            this.tabPage지역별가구수및면적.Location = new System.Drawing.Point(4, 22);
            this.tabPage지역별가구수및면적.Name = "tabPage지역별가구수및면적";
            this.tabPage지역별가구수및면적.Size = new System.Drawing.Size(457, 290);
            this.tabPage지역별가구수및면적.TabIndex = 6;
            this.tabPage지역별가구수및면적.Text = "지역별가구수 및 면적";
            this.tabPage지역별가구수및면적.UseVisualStyleBackColor = true;
            // 
            // tabPage비사용가치
            // 
            this.tabPage비사용가치.Location = new System.Drawing.Point(4, 22);
            this.tabPage비사용가치.Name = "tabPage비사용가치";
            this.tabPage비사용가치.Size = new System.Drawing.Size(457, 290);
            this.tabPage비사용가치.TabIndex = 7;
            this.tabPage비사용가치.Text = "비사용가치 가중치";
            this.tabPage비사용가치.UseVisualStyleBackColor = true;
            // 
            // panelProjectTitle
            // 
            this.panelProjectTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(96)))));
            this.panelProjectTitle.Controls.Add(this.labelTitle);
            this.panelProjectTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelProjectTitle.Location = new System.Drawing.Point(0, 0);
            this.panelProjectTitle.Name = "panelProjectTitle";
            this.panelProjectTitle.Size = new System.Drawing.Size(544, 33);
            this.panelProjectTitle.TabIndex = 0;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(214, 5);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(112, 21);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "프로젝트 제목";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsDXFCoord,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(40, 533);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(822, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsDXFCoord
            // 
            this.tsDXFCoord.Name = "tsDXFCoord";
            this.tsDXFCoord.Size = new System.Drawing.Size(31, 17);
            this.tsDXFCoord.Text = "좌표";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(473, 17);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.AutoSize = false;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(120, 17);
            this.toolStripStatusLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.BackColor = System.Drawing.Color.White;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 16);
            this.toolStripProgressBar1.Step = 1;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(31, 17);
            this.toolStripStatusLabel1.Text = "U&&E";
            // 
            // panelRibbonToolbar
            // 
            this.panelRibbonToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.panelRibbonToolbar.BackgroundImage = global::SoilMan.Properties.Resources.관리아이콘bg_02;
            this.panelRibbonToolbar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelRibbonToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRibbonToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelRibbonToolbar.Name = "panelRibbonToolbar";
            this.panelRibbonToolbar.Size = new System.Drawing.Size(862, 100);
            this.panelRibbonToolbar.TabIndex = 1;
            // 
            // mUpdataeTmr
            // 
            this.mUpdataeTmr.Tick += new System.EventHandler(this.mUpdataeTmr_Tick);
            // 
            // tabPage스티그마
            // 
            this.tabPage스티그마.Location = new System.Drawing.Point(4, 22);
            this.tabPage스티그마.Name = "tabPage스티그마";
            this.tabPage스티그마.Size = new System.Drawing.Size(457, 290);
            this.tabPage스티그마.TabIndex = 8;
            this.tabPage스티그마.Text = "스티그마 및 회복기간";
            this.tabPage스티그마.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 555);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.panelLeftToolBar);
            this.Controls.Add(this.panelRibbonToolbar);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelLeftToolBar.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).EndInit();
            this.splitContainerLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeftDown)).EndInit();
            this.splitContainerLeftDown.ResumeLayout(false);
            this.tabCtrlSystemConst.ResumeLayout(false);
            this.panelProjectTitle.ResumeLayout(false);
            this.panelProjectTitle.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuFile;
        private System.Windows.Forms.ToolStripMenuItem 프로젝트열기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 프로젝트저장ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 다른이름으로저장ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsMenuEdit;
        private System.Windows.Forms.ToolStripMenuItem tsMenuRemoveSelectedShapes;
        private System.Windows.Forms.ToolStripMenuItem tsMenuRemoveUnselectedShapes;
        private System.Windows.Forms.ToolStripMenuItem tsMenuSelect;
        private System.Windows.Forms.Panel panelLeftToolBar;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private UnE.GUI.RibbonButton rbtnShowPanel;
        private UnE.GUI.RibbonButton rbtnHidePanel;
        private System.Windows.Forms.SplitContainer splitContainerLeft;
        private System.Windows.Forms.ToolStripMenuItem 수치지도열기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 토지이용계획도열기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 지적도열기ToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsDXFCoord;
        private System.Windows.Forms.SplitContainer splitContainerLeftDown;
        private System.Windows.Forms.Panel panelProjectTitle;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOption;
        private System.Windows.Forms.ToolStripMenuItem tsMenuSelectedColor;
        private System.Windows.Forms.TabControl tabCtrlSystemConst;
        private System.Windows.Forms.TabPage tabPage계량지표;
        private System.Windows.Forms.TabPage tabPage화폐화지표;
        private System.Windows.Forms.TabPage tabPage기능회복율;
        private System.Windows.Forms.TabPage tabPage기능회복기간;
        private System.Windows.Forms.TabPage tabPage단가;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.Panel panelRibbonToolbar;
        private System.Windows.Forms.Timer mUpdataeTmr;
        private System.Windows.Forms.TabPage tabPage지불의사액;
        private System.Windows.Forms.TabPage tabPage지역별가구수및면적;
        private System.Windows.Forms.TabPage tabPage비사용가치;
        private System.Windows.Forms.TabPage tabPage스티그마;
    }
}

