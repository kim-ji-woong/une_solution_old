namespace PreSafe
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mStatusWork = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStatusClock = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStatsCaps = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStatusNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStatusHanguel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStatusProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.mMainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.mViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMainPanel = new System.Windows.Forms.Panel();
            this.mSplitContent = new System.Windows.Forms.SplitContainer();
            this.mPaneRibbonToolBar = new System.Windows.Forms.Panel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.mRBtnUndo = new UnE.GUI.RibbonButton();
            this.mRBtnRedo = new UnE.GUI.RibbonButton();
            this.mRBtnSystemVar = new UnE.GUI.RibbonButton();
            this.mRBtnUserVar = new UnE.GUI.RibbonButton();
            this.mRBtnEnum = new UnE.GUI.RibbonButton();
            this.mRBtnHelp = new UnE.GUI.RibbonButton();
            this.statusClockTimer = new System.Windows.Forms.Timer(this.components);
            this.mFileToolStripMenuItem = new PreSafe.IDToolStripMenuItem();
            this.mOpenSennarioMenuItem = new PreSafe.IDToolStripMenuItem();
            this.mSaveSenarioMenuItem = new PreSafe.IDToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mOpenUserVarMenuItem = new PreSafe.IDToolStripMenuItem();
            this.mSaveUserVarMenuItem = new PreSafe.IDToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mOpenEnumMenuItem = new PreSafe.IDToolStripMenuItem();
            this.saveEnumToolstripMenuItem = new PreSafe.IDToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new PreSafe.IDToolStripMenuItem();
            this.viewExprToolStripMenuItem = new PreSafe.IDToolStripMenuItem();
            this.viewTextToolStripMenuItem = new PreSafe.IDToolStripMenuItem();
            this.compOptionToolStripMenuItem = new PreSafe.IDToolStripMenuItem();
            this.leftPaneToolStripMenuItem = new PreSafe.IDToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.mMainMenuStrip.SuspendLayout();
            this.mMainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mSplitContent)).BeginInit();
            this.mSplitContent.SuspendLayout();
            this.mPaneRibbonToolBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mStatusWork,
            this.mStatusClock,
            this.mStatsCaps,
            this.mStatusNum,
            this.mStatusHanguel,
            this.toolStripStatusLabel4,
            this.mStatusProgress,
            this.toolStripStatusLabel5});
            this.statusStrip1.Location = new System.Drawing.Point(0, 706);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1350, 24);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // mStatusWork
            // 
            this.mStatusWork.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.mStatusWork.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.mStatusWork.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mStatusWork.Name = "mStatusWork";
            this.mStatusWork.Size = new System.Drawing.Size(807, 19);
            this.mStatusWork.Spring = true;
            this.mStatusWork.Text = "현재 작업을 표시합니다.";
            this.mStatusWork.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mStatusWork.ToolTipText = "현재 작업을 표시합니다.";
            // 
            // mStatusClock
            // 
            this.mStatusClock.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.mStatusClock.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.mStatusClock.Name = "mStatusClock";
            this.mStatusClock.Size = new System.Drawing.Size(187, 19);
            this.mStatusClock.Text = "현재시간                                ";
            // 
            // mStatsCaps
            // 
            this.mStatsCaps.AutoSize = false;
            this.mStatsCaps.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.mStatsCaps.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.mStatsCaps.Name = "mStatsCaps";
            this.mStatsCaps.Size = new System.Drawing.Size(41, 19);
            this.mStatsCaps.Text = "CAPS";
            // 
            // mStatusNum
            // 
            this.mStatusNum.AutoSize = false;
            this.mStatusNum.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.mStatusNum.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.mStatusNum.Name = "mStatusNum";
            this.mStatusNum.Size = new System.Drawing.Size(39, 19);
            this.mStatusNum.Text = "NUM";
            // 
            // mStatusHanguel
            // 
            this.mStatusHanguel.AutoSize = false;
            this.mStatusHanguel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.mStatusHanguel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.mStatusHanguel.Name = "mStatusHanguel";
            this.mStatusHanguel.Size = new System.Drawing.Size(40, 19);
            this.mStatusHanguel.Text = "한/영";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.AutoSize = false;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(55, 19);
            this.toolStripStatusLabel4.Text = "진행사항";
            // 
            // mStatusProgress
            // 
            this.mStatusProgress.AutoSize = false;
            this.mStatusProgress.Name = "mStatusProgress";
            this.mStatusProgress.Size = new System.Drawing.Size(100, 18);
            this.mStatusProgress.ToolTipText = "I/O 작업의 진행율을 표시합니다.";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.AutoSize = false;
            this.toolStripStatusLabel5.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabel5.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(64, 19);
            this.toolStripStatusLabel5.Text = " Pre-Safe ";
            // 
            // mMainMenuStrip
            // 
            this.mMainMenuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.mMainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFileToolStripMenuItem,
            this.mViewToolStripMenuItem});
            this.mMainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mMainMenuStrip.Name = "mMainMenuStrip";
            this.mMainMenuStrip.Size = new System.Drawing.Size(1350, 24);
            this.mMainMenuStrip.TabIndex = 1;
            this.mMainMenuStrip.Text = "menuStrip1";
            // 
            // mViewToolStripMenuItem
            // 
            this.mViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewExprToolStripMenuItem,
            this.viewTextToolStripMenuItem,
            this.compOptionToolStripMenuItem,
            this.leftPaneToolStripMenuItem});
            this.mViewToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.mViewToolStripMenuItem.Name = "mViewToolStripMenuItem";
            this.mViewToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.mViewToolStripMenuItem.Text = "보기 (&V)";
            this.mViewToolStripMenuItem.ToolTipText = "창 및 내용 보기";
            // 
            // mMainPanel
            // 
            this.mMainPanel.BackColor = System.Drawing.Color.Transparent;
            this.mMainPanel.Controls.Add(this.mSplitContent);
            this.mMainPanel.Controls.Add(this.mPaneRibbonToolBar);
            this.mMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMainPanel.Location = new System.Drawing.Point(0, 24);
            this.mMainPanel.Name = "mMainPanel";
            this.mMainPanel.Size = new System.Drawing.Size(1350, 682);
            this.mMainPanel.TabIndex = 2;
            // 
            // mSplitContent
            // 
            this.mSplitContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.mSplitContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSplitContent.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mSplitContent.Location = new System.Drawing.Point(0, 100);
            this.mSplitContent.Name = "mSplitContent";
            // 
            // mSplitContent.Panel1
            // 
            this.mSplitContent.Panel1.BackColor = System.Drawing.Color.White;
            this.mSplitContent.Panel1Collapsed = true;
            this.mSplitContent.Panel1MinSize = 120;
            // 
            // mSplitContent.Panel2
            // 
            this.mSplitContent.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.mSplitContent.Panel2MinSize = 120;
            this.mSplitContent.Size = new System.Drawing.Size(1350, 582);
            this.mSplitContent.SplitterDistance = 300;
            this.mSplitContent.SplitterWidth = 2;
            this.mSplitContent.TabIndex = 1;
            // 
            // mPaneRibbonToolBar
            // 
            this.mPaneRibbonToolBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.mPaneRibbonToolBar.BackgroundImage = global::UnE.Properties.Resources.관리아이콘bg_02;
            this.mPaneRibbonToolBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mPaneRibbonToolBar.Controls.Add(this.pictureBox3);
            this.mPaneRibbonToolBar.Controls.Add(this.pictureBox2);
            this.mPaneRibbonToolBar.Controls.Add(this.pictureBox1);
            this.mPaneRibbonToolBar.Controls.Add(this.mRBtnUndo);
            this.mPaneRibbonToolBar.Controls.Add(this.mRBtnRedo);
            this.mPaneRibbonToolBar.Controls.Add(this.mRBtnSystemVar);
            this.mPaneRibbonToolBar.Controls.Add(this.mRBtnUserVar);
            this.mPaneRibbonToolBar.Controls.Add(this.mRBtnEnum);
            this.mPaneRibbonToolBar.Controls.Add(this.mRBtnHelp);
            this.mPaneRibbonToolBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.mPaneRibbonToolBar.Location = new System.Drawing.Point(0, 0);
            this.mPaneRibbonToolBar.Margin = new System.Windows.Forms.Padding(0);
            this.mPaneRibbonToolBar.Name = "mPaneRibbonToolBar";
            this.mPaneRibbonToolBar.Size = new System.Drawing.Size(1350, 100);
            this.mPaneRibbonToolBar.TabIndex = 0;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::UnE.Properties.Resources.skin_line_img;
            this.pictureBox3.Location = new System.Drawing.Point(30, 5);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(2, 90);
            this.pictureBox3.TabIndex = 20;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::UnE.Properties.Resources.skin_line_img;
            this.pictureBox2.Location = new System.Drawing.Point(394, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(2, 90);
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::UnE.Properties.Resources.skin_line_img;
            this.pictureBox1.Location = new System.Drawing.Point(175, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(2, 90);
            this.pictureBox1.TabIndex = 18;
            this.pictureBox1.TabStop = false;
            // 
            // mRBtnUndo
            // 
            this.mRBtnUndo.BackColor = System.Drawing.Color.Transparent;
            this.mRBtnUndo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mRBtnUndo.CheckedBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnUndo.CheckedImage = global::UnE.Properties.Resources.undo_checked;
            this.mRBtnUndo.ClickedBackgroundImage = null;
            this.mRBtnUndo.ClickedImage = global::UnE.Properties.Resources.undo_checked;
            this.mRBtnUndo.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.mRBtnUndo.DisabledBkgndImage = null;
            this.mRBtnUndo.DisabledImage = global::UnE.Properties.Resources.undo_disabled;
            this.mRBtnUndo.ID = -1;
            this.mRBtnUndo.InitButtonWidth = 70;
            this.mRBtnUndo.IsChecked = false;
            this.mRBtnUndo.Location = new System.Drawing.Point(33, 9);
            this.mRBtnUndo.MouseOverBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnUndo.MouseOverImage = global::UnE.Properties.Resources.undo_normal;
            this.mRBtnUndo.Name = "mRBtnUndo";
            this.mRBtnUndo.NormalImage = global::UnE.Properties.Resources.undo_normal;
            this.mRBtnUndo.Owner = null;
            this.mRBtnUndo.Size = new System.Drawing.Size(70, 80);
            this.mRBtnUndo.TabIndex = 12;
            this.mRBtnUndo.Text = "되돌리기";
            this.mRBtnUndo.TextLocation = new System.Drawing.Point(0, 0);
            this.mRBtnUndo.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.mRBtnUndo.ToolTipText = "이전상태로 되돌립니다.";
            this.mRBtnUndo.UseCustomImageRect = true;
            this.mRBtnUndo.UseTextLocation = false;
            this.mRBtnUndo.UseVisualStyleBackColor = false;
            // 
            // mRBtnRedo
            // 
            this.mRBtnRedo.BackColor = System.Drawing.Color.Transparent;
            this.mRBtnRedo.CheckedBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnRedo.CheckedImage = global::UnE.Properties.Resources.redo_checked;
            this.mRBtnRedo.ClickedBackgroundImage = null;
            this.mRBtnRedo.ClickedImage = global::UnE.Properties.Resources.redo_checked;
            this.mRBtnRedo.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.mRBtnRedo.DisabledBkgndImage = null;
            this.mRBtnRedo.DisabledImage = global::UnE.Properties.Resources.redo_disabled;
            this.mRBtnRedo.ID = -1;
            this.mRBtnRedo.InitButtonWidth = 70;
            this.mRBtnRedo.IsChecked = false;
            this.mRBtnRedo.Location = new System.Drawing.Point(104, 9);
            this.mRBtnRedo.MouseOverBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnRedo.MouseOverImage = global::UnE.Properties.Resources.redo_normal;
            this.mRBtnRedo.Name = "mRBtnRedo";
            this.mRBtnRedo.NormalImage = global::UnE.Properties.Resources.redo_normal;
            this.mRBtnRedo.Owner = null;
            this.mRBtnRedo.Size = new System.Drawing.Size(70, 80);
            this.mRBtnRedo.TabIndex = 15;
            this.mRBtnRedo.Text = "다시실행";
            this.mRBtnRedo.TextLocation = new System.Drawing.Point(0, 0);
            this.mRBtnRedo.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.mRBtnRedo.ToolTipText = "다시 실행합니다.";
            this.mRBtnRedo.UseCustomImageRect = true;
            this.mRBtnRedo.UseTextLocation = false;
            this.mRBtnRedo.UseVisualStyleBackColor = false;
            // 
            // mRBtnSystemVar
            // 
            this.mRBtnSystemVar.BackColor = System.Drawing.Color.Transparent;
            this.mRBtnSystemVar.CheckedBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnSystemVar.CheckedImage = null;
            this.mRBtnSystemVar.ClickedBackgroundImage = null;
            this.mRBtnSystemVar.ClickedImage = null;
            this.mRBtnSystemVar.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.mRBtnSystemVar.DisabledBkgndImage = null;
            this.mRBtnSystemVar.DisabledImage = null;
            this.mRBtnSystemVar.ID = -1;
            this.mRBtnSystemVar.InitButtonWidth = 70;
            this.mRBtnSystemVar.IsChecked = false;
            this.mRBtnSystemVar.Location = new System.Drawing.Point(178, 9);
            this.mRBtnSystemVar.MouseOverBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnSystemVar.MouseOverImage = null;
            this.mRBtnSystemVar.Name = "mRBtnSystemVar";
            this.mRBtnSystemVar.NormalImage = null;
            this.mRBtnSystemVar.Owner = null;
            this.mRBtnSystemVar.Size = new System.Drawing.Size(70, 80);
            this.mRBtnSystemVar.TabIndex = 13;
            this.mRBtnSystemVar.Text = "시스템변수";
            this.mRBtnSystemVar.TextLocation = new System.Drawing.Point(0, 0);
            this.mRBtnSystemVar.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.mRBtnSystemVar.ToolTipText = "시스템 변수를 확인합니다.";
            this.mRBtnSystemVar.UseCustomImageRect = true;
            this.mRBtnSystemVar.UseTextLocation = false;
            this.mRBtnSystemVar.UseVisualStyleBackColor = false;
            // 
            // mRBtnUserVar
            // 
            this.mRBtnUserVar.BackColor = System.Drawing.Color.Transparent;
            this.mRBtnUserVar.CheckedBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnUserVar.CheckedImage = null;
            this.mRBtnUserVar.ClickedBackgroundImage = null;
            this.mRBtnUserVar.ClickedImage = null;
            this.mRBtnUserVar.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.mRBtnUserVar.DisabledBkgndImage = null;
            this.mRBtnUserVar.DisabledImage = null;
            this.mRBtnUserVar.ID = -1;
            this.mRBtnUserVar.InitButtonWidth = 70;
            this.mRBtnUserVar.IsChecked = false;
            this.mRBtnUserVar.Location = new System.Drawing.Point(249, 9);
            this.mRBtnUserVar.MouseOverBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnUserVar.MouseOverImage = null;
            this.mRBtnUserVar.Name = "mRBtnUserVar";
            this.mRBtnUserVar.NormalImage = null;
            this.mRBtnUserVar.Owner = null;
            this.mRBtnUserVar.Size = new System.Drawing.Size(70, 80);
            this.mRBtnUserVar.TabIndex = 16;
            this.mRBtnUserVar.Text = "사용자변수";
            this.mRBtnUserVar.TextLocation = new System.Drawing.Point(0, 0);
            this.mRBtnUserVar.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.mRBtnUserVar.ToolTipText = "사용자 변수를 편집합니다.";
            this.mRBtnUserVar.UseCustomImageRect = true;
            this.mRBtnUserVar.UseTextLocation = false;
            this.mRBtnUserVar.UseVisualStyleBackColor = false;
            // 
            // mRBtnEnum
            // 
            this.mRBtnEnum.BackColor = System.Drawing.Color.Transparent;
            this.mRBtnEnum.CheckedBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnEnum.CheckedImage = null;
            this.mRBtnEnum.ClickedBackgroundImage = null;
            this.mRBtnEnum.ClickedImage = null;
            this.mRBtnEnum.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.mRBtnEnum.DisabledBkgndImage = null;
            this.mRBtnEnum.DisabledImage = null;
            this.mRBtnEnum.ID = -1;
            this.mRBtnEnum.InitButtonWidth = 70;
            this.mRBtnEnum.IsChecked = false;
            this.mRBtnEnum.Location = new System.Drawing.Point(320, 9);
            this.mRBtnEnum.MouseOverBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnEnum.MouseOverImage = null;
            this.mRBtnEnum.Name = "mRBtnEnum";
            this.mRBtnEnum.NormalImage = null;
            this.mRBtnEnum.Owner = null;
            this.mRBtnEnum.Size = new System.Drawing.Size(73, 80);
            this.mRBtnEnum.TabIndex = 17;
            this.mRBtnEnum.Text = "열거형 변수";
            this.mRBtnEnum.TextLocation = new System.Drawing.Point(0, 0);
            this.mRBtnEnum.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.mRBtnEnum.ToolTipText = "열거형 변수를 편집합니다.";
            this.mRBtnEnum.UseCustomImageRect = true;
            this.mRBtnEnum.UseTextLocation = false;
            this.mRBtnEnum.UseVisualStyleBackColor = false;
            // 
            // mRBtnHelp
            // 
            this.mRBtnHelp.BackColor = System.Drawing.Color.Transparent;
            this.mRBtnHelp.CheckedBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnHelp.CheckedImage = null;
            this.mRBtnHelp.ClickedBackgroundImage = null;
            this.mRBtnHelp.ClickedImage = null;
            this.mRBtnHelp.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.mRBtnHelp.DisabledBkgndImage = null;
            this.mRBtnHelp.DisabledImage = null;
            this.mRBtnHelp.ID = -1;
            this.mRBtnHelp.InitButtonWidth = 70;
            this.mRBtnHelp.IsChecked = false;
            this.mRBtnHelp.Location = new System.Drawing.Point(398, 9);
            this.mRBtnHelp.MouseOverBkgndImage = global::UnE.Properties.Resources.Ribon_mouse_over_background;
            this.mRBtnHelp.MouseOverImage = null;
            this.mRBtnHelp.Name = "mRBtnHelp";
            this.mRBtnHelp.NormalImage = null;
            this.mRBtnHelp.Owner = null;
            this.mRBtnHelp.Size = new System.Drawing.Size(73, 80);
            this.mRBtnHelp.TabIndex = 14;
            this.mRBtnHelp.Text = "수식 도움말";
            this.mRBtnHelp.TextLocation = new System.Drawing.Point(0, 0);
            this.mRBtnHelp.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.mRBtnHelp.ToolTipText = "수식 도움말";
            this.mRBtnHelp.UseCustomImageRect = true;
            this.mRBtnHelp.UseTextLocation = false;
            this.mRBtnHelp.UseVisualStyleBackColor = false;
            // 
            // statusClockTimer
            // 
            this.statusClockTimer.Interval = 1000;
            this.statusClockTimer.Tick += new System.EventHandler(this.statusClockTimer_Tick);
            // 
            // mFileToolStripMenuItem
            // 
            this.mFileToolStripMenuItem.CommandID = -1;
            this.mFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mOpenSennarioMenuItem,
            this.mSaveSenarioMenuItem,
            this.toolStripSeparator1,
            this.mOpenUserVarMenuItem,
            this.mSaveUserVarMenuItem,
            this.toolStripSeparator2,
            this.mOpenEnumMenuItem,
            this.saveEnumToolstripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.mFileToolStripMenuItem.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.mFileToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
            this.mFileToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.mFileToolStripMenuItem.Text = "파일 (&F)";
            this.mFileToolStripMenuItem.ToolTipText = "파일 열기 및 저장";
            // 
            // mOpenSennarioMenuItem
            // 
            this.mOpenSennarioMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.mOpenSennarioMenuItem.CommandID = -1;
            this.mOpenSennarioMenuItem.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.mOpenSennarioMenuItem.ForeColor = System.Drawing.Color.White;
            this.mOpenSennarioMenuItem.Name = "mOpenSennarioMenuItem";
            this.mOpenSennarioMenuItem.Size = new System.Drawing.Size(214, 22);
            this.mOpenSennarioMenuItem.Text = "시나리오 열기 (&O)";
            this.mOpenSennarioMenuItem.ToolTipText = "시나리오파일을 엽니다.";
            // 
            // mSaveSenarioMenuItem
            // 
            this.mSaveSenarioMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.mSaveSenarioMenuItem.CommandID = -1;
            this.mSaveSenarioMenuItem.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.mSaveSenarioMenuItem.ForeColor = System.Drawing.Color.White;
            this.mSaveSenarioMenuItem.Name = "mSaveSenarioMenuItem";
            this.mSaveSenarioMenuItem.Size = new System.Drawing.Size(214, 22);
            this.mSaveSenarioMenuItem.Text = "시나리오 저장 (&S)";
            this.mSaveSenarioMenuItem.ToolTipText = "현재 시나리오를 저장합니다.";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.toolStripSeparator1.ForeColor = System.Drawing.Color.LightGray;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(211, 6);
            // 
            // mOpenUserVarMenuItem
            // 
            this.mOpenUserVarMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.mOpenUserVarMenuItem.CommandID = -1;
            this.mOpenUserVarMenuItem.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.mOpenUserVarMenuItem.ForeColor = System.Drawing.Color.White;
            this.mOpenUserVarMenuItem.Name = "mOpenUserVarMenuItem";
            this.mOpenUserVarMenuItem.Size = new System.Drawing.Size(214, 22);
            this.mOpenUserVarMenuItem.Text = "사용자 정의 변수 열기 (&U)";
            this.mOpenUserVarMenuItem.ToolTipText = "사용자 정의 변수를 엽니다.";
            // 
            // mSaveUserVarMenuItem
            // 
            this.mSaveUserVarMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.mSaveUserVarMenuItem.CommandID = -1;
            this.mSaveUserVarMenuItem.ForeColor = System.Drawing.Color.White;
            this.mSaveUserVarMenuItem.Name = "mSaveUserVarMenuItem";
            this.mSaveUserVarMenuItem.Size = new System.Drawing.Size(214, 22);
            this.mSaveUserVarMenuItem.Text = "사용자 정의 변수 저장 (&I)";
            this.mSaveUserVarMenuItem.ToolTipText = "사용자 정의 변수를 저장합니다.";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(211, 6);
            // 
            // mOpenEnumMenuItem
            // 
            this.mOpenEnumMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.mOpenEnumMenuItem.CommandID = -1;
            this.mOpenEnumMenuItem.ForeColor = System.Drawing.Color.White;
            this.mOpenEnumMenuItem.Name = "mOpenEnumMenuItem";
            this.mOpenEnumMenuItem.Size = new System.Drawing.Size(214, 22);
            this.mOpenEnumMenuItem.Text = "Enumeration 열기 (&E)";
            this.mOpenEnumMenuItem.ToolTipText = "상수로 사용되는 변수목록을 엽니다.";
            // 
            // saveEnumToolstripMenuItem
            // 
            this.saveEnumToolstripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.saveEnumToolstripMenuItem.CommandID = -1;
            this.saveEnumToolstripMenuItem.ForeColor = System.Drawing.Color.White;
            this.saveEnumToolstripMenuItem.Name = "saveEnumToolstripMenuItem";
            this.saveEnumToolstripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.saveEnumToolstripMenuItem.Text = "Enumeration 저장 (&W)";
            this.saveEnumToolstripMenuItem.ToolTipText = "상수로 사용되는 변수 목록을 저장합니다.";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(211, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.exitToolStripMenuItem.CommandID = -1;
            this.exitToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.exitToolStripMenuItem.Text = "종료 (&X)";
            this.exitToolStripMenuItem.ToolTipText = "프로그램을 종료합니다.";
            // 
            // viewExprToolStripMenuItem
            // 
            this.viewExprToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.viewExprToolStripMenuItem.CommandID = -1;
            this.viewExprToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.viewExprToolStripMenuItem.Name = "viewExprToolStripMenuItem";
            this.viewExprToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.viewExprToolStripMenuItem.Text = "수식 보기 (&E)";
            this.viewExprToolStripMenuItem.ToolTipText = "수식을 보여줍니다.";
            // 
            // viewTextToolStripMenuItem
            // 
            this.viewTextToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.viewTextToolStripMenuItem.CommandID = -1;
            this.viewTextToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.viewTextToolStripMenuItem.Name = "viewTextToolStripMenuItem";
            this.viewTextToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.viewTextToolStripMenuItem.Text = "텍스트 보기 (&T)";
            this.viewTextToolStripMenuItem.ToolTipText = "텍스트로 보여줍니다.";
            // 
            // compOptionToolStripMenuItem
            // 
            this.compOptionToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.compOptionToolStripMenuItem.CommandID = -1;
            this.compOptionToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.compOptionToolStripMenuItem.Name = "compOptionToolStripMenuItem";
            this.compOptionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.compOptionToolStripMenuItem.Text = "Componet 옵션 (&O)";
            this.compOptionToolStripMenuItem.ToolTipText = "Component의 옵션을 수정합니다.";
            // 
            // leftPaneToolStripMenuItem
            // 
            this.leftPaneToolStripMenuItem.CommandID = -1;
            this.leftPaneToolStripMenuItem.Name = "leftPaneToolStripMenuItem";
            this.leftPaneToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.leftPaneToolStripMenuItem.Text = "작업창 보이기";
            this.leftPaneToolStripMenuItem.ToolTipText = "작업 창을 보입니다.";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1350, 730);
            this.Controls.Add(this.mMainPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mMainMenuStrip);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mMainMenuStrip;
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mMainMenuStrip.ResumeLayout(false);
            this.mMainMenuStrip.PerformLayout();
            this.mMainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mSplitContent)).EndInit();
            this.mSplitContent.ResumeLayout(false);
            this.mPaneRibbonToolBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip mMainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mViewToolStripMenuItem;
        private IDToolStripMenuItem mOpenSennarioMenuItem;
        private IDToolStripMenuItem mSaveSenarioMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private IDToolStripMenuItem mOpenUserVarMenuItem;        
        private IDToolStripMenuItem mSaveUserVarMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private IDToolStripMenuItem mOpenEnumMenuItem;
        private IDToolStripMenuItem viewExprToolStripMenuItem;
        private IDToolStripMenuItem viewTextToolStripMenuItem;
        private IDToolStripMenuItem compOptionToolStripMenuItem;
        private IDToolStripMenuItem saveEnumToolstripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private IDToolStripMenuItem exitToolStripMenuItem;
        private IDToolStripMenuItem leftPaneToolStripMenuItem;
        internal System.Windows.Forms.Panel mMainPanel;
        private System.Windows.Forms.Panel mPaneRibbonToolBar;
        internal System.Windows.Forms.SplitContainer mSplitContent;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripProgressBar mStatusProgress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        internal System.Windows.Forms.ToolStripStatusLabel mStatusWork;
        internal System.Windows.Forms.ToolStripStatusLabel mStatsCaps;
        internal System.Windows.Forms.ToolStripStatusLabel mStatusNum;
        internal System.Windows.Forms.ToolStripStatusLabel mStatusHanguel;
        private IDToolStripMenuItem mFileToolStripMenuItem;
        private UnE.GUI.RibbonButton mRBtnUndo;
        private UnE.GUI.RibbonButton mRBtnRedo;
        private UnE.GUI.RibbonButton mRBtnSystemVar;
        private UnE.GUI.RibbonButton mRBtnUserVar;
        private UnE.GUI.RibbonButton mRBtnEnum;
        private UnE.GUI.RibbonButton mRBtnHelp;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.ToolStripStatusLabel mStatusClock;
        private System.Windows.Forms.Timer statusClockTimer;
    }
}

