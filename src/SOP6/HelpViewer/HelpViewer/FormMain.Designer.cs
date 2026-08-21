namespace HelpViewer
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelRed = new System.Windows.Forms.Panel();
            this.labelSystemName = new System.Windows.Forms.Label();
            this.panelSearchArea = new System.Windows.Forms.Panel();
            this.panelSearchLabel = new System.Windows.Forms.Panel();
            this.labelSearchLabel = new System.Windows.Forms.Label();
            this.rbtnSearch = new HelpViewer.RibbonButton();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.rbtnPrint = new HelpViewer.RibbonButton();
            this.rbtnRedo = new HelpViewer.RibbonButton();
            this.rbtnUndo = new HelpViewer.RibbonButton();
            this.splitContainerBody = new System.Windows.Forms.SplitContainer();
            this.tabControlHeader = new HelpViewer.TabControlHeader();
            this.tabPageIndexHeader = new System.Windows.Forms.TabPage();
            this.tabPageSearchResultHeader = new System.Windows.Forms.TabPage();
            this.tabControlBody = new HelpViewer.TabControlBody();
            this.tabPageIndex = new System.Windows.Forms.TabPage();
            this.treeViewAdv1 = new Aga.Controls.Tree.TreeViewAdv();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeStateIcon1 = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
            this.tabPageSearchResult = new System.Windows.Forms.TabPage();
            this.panelSearchResult = new System.Windows.Forms.Panel();
            this.labelSearchResult = new System.Windows.Forms.Label();
            this.pictureBoxSearchResultIcon = new System.Windows.Forms.PictureBox();
            this.pictureBoxSearchResultLine = new System.Windows.Forms.PictureBox();
            this.labelNoResult = new System.Windows.Forms.Label();
            this.webViewer = new System.Windows.Forms.WebBrowser();
            this.printMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuPrintPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPrintAll = new System.Windows.Forms.ToolStripMenuItem();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.panelTop.SuspendLayout();
            this.panelRed.SuspendLayout();
            this.panelSearchArea.SuspendLayout();
            this.panelSearchLabel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBody)).BeginInit();
            this.splitContainerBody.Panel1.SuspendLayout();
            this.splitContainerBody.Panel2.SuspendLayout();
            this.splitContainerBody.SuspendLayout();
            this.tabControlHeader.SuspendLayout();
            this.tabControlBody.SuspendLayout();
            this.tabPageIndex.SuspendLayout();
            this.tabPageSearchResult.SuspendLayout();
            this.panelSearchResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearchResultIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearchResultLine)).BeginInit();
            this.printMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.panelRed);
            this.panelTop.Controls.Add(this.panelSearchArea);
            this.panelTop.Controls.Add(this.rbtnPrint);
            this.panelTop.Controls.Add(this.rbtnRedo);
            this.panelTop.Controls.Add(this.rbtnUndo);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(880, 46);
            this.panelTop.TabIndex = 0;
            // 
            // panelRed
            // 
            this.panelRed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(80)))), ((int)(((byte)(71)))));
            this.panelRed.Controls.Add(this.labelSystemName);
            this.panelRed.Location = new System.Drawing.Point(0, 0);
            this.panelRed.Name = "panelRed";
            this.panelRed.Size = new System.Drawing.Size(293, 58);
            this.panelRed.TabIndex = 4;
            // 
            // labelSystemName
            // 
            this.labelSystemName.AutoSize = true;
            this.labelSystemName.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSystemName.ForeColor = System.Drawing.Color.White;
            this.labelSystemName.Location = new System.Drawing.Point(16, 18);
            this.labelSystemName.Name = "labelSystemName";
            this.labelSystemName.Size = new System.Drawing.Size(182, 21);
            this.labelSystemName.TabIndex = 0;
            this.labelSystemName.Text = "스마트 재난관리 시스템";
            // 
            // panelSearchArea
            // 
            this.panelSearchArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSearchArea.BackColor = System.Drawing.Color.White;
            this.panelSearchArea.BackgroundImage = global::HelpViewer.Properties.Resources.search;
            this.panelSearchArea.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelSearchArea.Controls.Add(this.panelSearchLabel);
            this.panelSearchArea.Controls.Add(this.rbtnSearch);
            this.panelSearchArea.Controls.Add(this.textBoxSearch);
            this.panelSearchArea.Location = new System.Drawing.Point(645, 12);
            this.panelSearchArea.Name = "panelSearchArea";
            this.panelSearchArea.Size = new System.Drawing.Size(220, 36);
            this.panelSearchArea.TabIndex = 3;
            // 
            // panelSearchLabel
            // 
            this.panelSearchLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(80)))), ((int)(((byte)(71)))));
            this.panelSearchLabel.Controls.Add(this.labelSearchLabel);
            this.panelSearchLabel.Location = new System.Drawing.Point(18, 7);
            this.panelSearchLabel.Name = "panelSearchLabel";
            this.panelSearchLabel.Size = new System.Drawing.Size(162, 22);
            this.panelSearchLabel.TabIndex = 2;
            // 
            // labelSearchLabel
            // 
            this.labelSearchLabel.AutoSize = true;
            this.labelSearchLabel.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSearchLabel.ForeColor = System.Drawing.Color.White;
            this.labelSearchLabel.Location = new System.Drawing.Point(2, 0);
            this.labelSearchLabel.Name = "labelSearchLabel";
            this.labelSearchLabel.Size = new System.Drawing.Size(160, 21);
            this.labelSearchLabel.TabIndex = 0;
            this.labelSearchLabel.Text = "검색어를 입력하세요";
            this.labelSearchLabel.Click += new System.EventHandler(this.labelSearchLabel_Click);
            this.labelSearchLabel.MouseEnter += new System.EventHandler(this.labelSearchLabel_MouseEnter);
            this.labelSearchLabel.MouseLeave += new System.EventHandler(this.labelSearchLabel_MouseLeave);
            // 
            // rbtnSearch
            // 
            this.rbtnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(80)))), ((int)(((byte)(71)))));
            this.rbtnSearch.CheckButton = false;
            this.rbtnSearch.CheckedBkgndImage = null;
            this.rbtnSearch.CheckedImage = ((System.Drawing.Image)(resources.GetObject("rbtnSearch.CheckedImage")));
            this.rbtnSearch.ClickedBackgroundImage = null;
            this.rbtnSearch.ClickedImage = ((System.Drawing.Image)(resources.GetObject("rbtnSearch.ClickedImage")));
            this.rbtnSearch.CustomImageRect = new System.Drawing.Rectangle(0, 0, 22, 21);
            this.rbtnSearch.DisabledBkgndImage = null;
            this.rbtnSearch.DisabledImage = global::HelpViewer.Properties.Resources.search_disable;
            this.rbtnSearch.ID = -1;
            this.rbtnSearch.InitButtonWidth = 22;
            this.rbtnSearch.IsChecked = false;
            this.rbtnSearch.Location = new System.Drawing.Point(186, 8);
            this.rbtnSearch.MouseOverBkgndImage = null;
            this.rbtnSearch.MouseOverImage = ((System.Drawing.Image)(resources.GetObject("rbtnSearch.MouseOverImage")));
            this.rbtnSearch.Name = "rbtnSearch";
            this.rbtnSearch.NormalImage = ((System.Drawing.Image)(resources.GetObject("rbtnSearch.NormalImage")));
            this.rbtnSearch.Owner = null;
            this.rbtnSearch.Size = new System.Drawing.Size(22, 21);
            this.rbtnSearch.TabIndex = 0;
            this.rbtnSearch.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSearch.TextPos = HelpViewer.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSearch.ToolTipText = "검색";
            this.rbtnSearch.UseCustomImageRect = true;
            this.rbtnSearch.UseTextLocation = false;
            this.rbtnSearch.UseVisualStyleBackColor = false;
            this.rbtnSearch.Click += new System.EventHandler(this.rbtnSearch_Click);
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxSearch.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxSearch.Location = new System.Drawing.Point(18, 7);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(162, 22);
            this.textBoxSearch.TabIndex = 1;
            this.textBoxSearch.Visible = false;
            this.textBoxSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSearch_KeyUp);
            // 
            // rbtnPrint
            // 
            this.rbtnPrint.CheckButton = false;
            this.rbtnPrint.CheckedBkgndImage = null;
            this.rbtnPrint.CheckedImage = global::HelpViewer.Properties.Resources.print_clicked;
            this.rbtnPrint.ClickedBackgroundImage = null;
            this.rbtnPrint.ClickedImage = global::HelpViewer.Properties.Resources.print_clicked;
            this.rbtnPrint.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnPrint.DisabledBkgndImage = null;
            this.rbtnPrint.DisabledImage = global::HelpViewer.Properties.Resources.print_disable;
            this.rbtnPrint.ID = -1;
            this.rbtnPrint.InitButtonWidth = 60;
            this.rbtnPrint.IsChecked = false;
            this.rbtnPrint.Location = new System.Drawing.Point(393, 12);
            this.rbtnPrint.MouseOverBkgndImage = null;
            this.rbtnPrint.MouseOverImage = global::HelpViewer.Properties.Resources.print_over;
            this.rbtnPrint.Name = "rbtnPrint";
            this.rbtnPrint.NormalImage = global::HelpViewer.Properties.Resources.print_normal;
            this.rbtnPrint.Owner = null;
            this.rbtnPrint.Size = new System.Drawing.Size(60, 32);
            this.rbtnPrint.TabIndex = 0;
            this.rbtnPrint.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnPrint.TextPos = HelpViewer.RibbonButton.TextPosition.BOTTOM;
            this.rbtnPrint.ToolTipText = "인쇄";
            this.rbtnPrint.UseCustomImageRect = true;
            this.rbtnPrint.UseTextLocation = false;
            this.rbtnPrint.UseVisualStyleBackColor = true;
            this.rbtnPrint.Click += new System.EventHandler(this.rbtnPrint_Click);
            // 
            // rbtnRedo
            // 
            this.rbtnRedo.CheckButton = false;
            this.rbtnRedo.CheckedBkgndImage = null;
            this.rbtnRedo.CheckedImage = global::HelpViewer.Properties.Resources.redo_checked;
            this.rbtnRedo.ClickedBackgroundImage = null;
            this.rbtnRedo.ClickedImage = global::HelpViewer.Properties.Resources.redo_checked;
            this.rbtnRedo.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnRedo.DisabledBkgndImage = null;
            this.rbtnRedo.DisabledImage = global::HelpViewer.Properties.Resources.redo_disabled;
            this.rbtnRedo.Enabled = false;
            this.rbtnRedo.ID = -1;
            this.rbtnRedo.InitButtonWidth = 60;
            this.rbtnRedo.IsChecked = false;
            this.rbtnRedo.Location = new System.Drawing.Point(353, 12);
            this.rbtnRedo.MouseOverBkgndImage = null;
            this.rbtnRedo.MouseOverImage = global::HelpViewer.Properties.Resources.redo_mouseover;
            this.rbtnRedo.Name = "rbtnRedo";
            this.rbtnRedo.NormalImage = global::HelpViewer.Properties.Resources.redo_normal;
            this.rbtnRedo.Owner = null;
            this.rbtnRedo.Size = new System.Drawing.Size(60, 32);
            this.rbtnRedo.TabIndex = 0;
            this.rbtnRedo.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnRedo.TextPos = HelpViewer.RibbonButton.TextPosition.BOTTOM;
            this.rbtnRedo.ToolTipText = "앞으로";
            this.rbtnRedo.UseCustomImageRect = true;
            this.rbtnRedo.UseTextLocation = false;
            this.rbtnRedo.UseVisualStyleBackColor = true;
            this.rbtnRedo.Click += new System.EventHandler(this.rbtnRedo_Click);
            // 
            // rbtnUndo
            // 
            this.rbtnUndo.CheckButton = false;
            this.rbtnUndo.CheckedBkgndImage = null;
            this.rbtnUndo.CheckedImage = global::HelpViewer.Properties.Resources.undo_checked;
            this.rbtnUndo.ClickedBackgroundImage = null;
            this.rbtnUndo.ClickedImage = global::HelpViewer.Properties.Resources.undo_checked;
            this.rbtnUndo.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnUndo.DisabledBkgndImage = null;
            this.rbtnUndo.DisabledImage = global::HelpViewer.Properties.Resources.undo_disabled;
            this.rbtnUndo.Enabled = false;
            this.rbtnUndo.ID = -1;
            this.rbtnUndo.InitButtonWidth = 60;
            this.rbtnUndo.IsChecked = false;
            this.rbtnUndo.Location = new System.Drawing.Point(313, 12);
            this.rbtnUndo.MouseOverBkgndImage = null;
            this.rbtnUndo.MouseOverImage = global::HelpViewer.Properties.Resources.undo_mouseover;
            this.rbtnUndo.Name = "rbtnUndo";
            this.rbtnUndo.NormalImage = global::HelpViewer.Properties.Resources.undo_normal;
            this.rbtnUndo.Owner = null;
            this.rbtnUndo.Size = new System.Drawing.Size(60, 32);
            this.rbtnUndo.TabIndex = 0;
            this.rbtnUndo.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnUndo.TextPos = HelpViewer.RibbonButton.TextPosition.BOTTOM;
            this.rbtnUndo.ToolTipText = "뒤로";
            this.rbtnUndo.UseCustomImageRect = true;
            this.rbtnUndo.UseTextLocation = false;
            this.rbtnUndo.UseVisualStyleBackColor = true;
            this.rbtnUndo.Click += new System.EventHandler(this.rbtnUndo_Click);
            // 
            // splitContainerBody
            // 
            this.splitContainerBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(218)))), ((int)(((byte)(228)))));
            this.splitContainerBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBody.Location = new System.Drawing.Point(0, 46);
            this.splitContainerBody.Name = "splitContainerBody";
            // 
            // splitContainerBody.Panel1
            // 
            this.splitContainerBody.Panel1.Controls.Add(this.tabControlHeader);
            this.splitContainerBody.Panel1.Controls.Add(this.tabControlBody);
            // 
            // splitContainerBody.Panel2
            // 
            this.splitContainerBody.Panel2.Controls.Add(this.webViewer);
            this.splitContainerBody.Size = new System.Drawing.Size(880, 450);
            this.splitContainerBody.SplitterDistance = 292;
            this.splitContainerBody.TabIndex = 1;
            this.splitContainerBody.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerBody_SplitterMoved);
            // 
            // tabControlHeader
            // 
            this.tabControlHeader.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControlHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlHeader.Controls.Add(this.tabPageIndexHeader);
            this.tabControlHeader.Controls.Add(this.tabPageSearchResultHeader);
            this.tabControlHeader.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlHeader.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tabControlHeader.ItemSize = new System.Drawing.Size(120, 30);
            this.tabControlHeader.Location = new System.Drawing.Point(5, 415);
            this.tabControlHeader.Name = "tabControlHeader";
            this.tabControlHeader.SelectedIndex = 0;
            this.tabControlHeader.Size = new System.Drawing.Size(287, 30);
            this.tabControlHeader.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlHeader.TabIndex = 2;
            this.tabControlHeader.VerticalMode = false;
            this.tabControlHeader.SelectedIndexChanged += new System.EventHandler(this.tabControlHeader_SelectedIndexChanged);
            // 
            // tabPageIndexHeader
            // 
            this.tabPageIndexHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.tabPageIndexHeader.Location = new System.Drawing.Point(4, 4);
            this.tabPageIndexHeader.Name = "tabPageIndexHeader";
            this.tabPageIndexHeader.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIndexHeader.Size = new System.Drawing.Size(279, 0);
            this.tabPageIndexHeader.TabIndex = 0;
            this.tabPageIndexHeader.Text = "목록";
            // 
            // tabPageSearchResultHeader
            // 
            this.tabPageSearchResultHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.tabPageSearchResultHeader.Location = new System.Drawing.Point(4, 4);
            this.tabPageSearchResultHeader.Name = "tabPageSearchResultHeader";
            this.tabPageSearchResultHeader.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSearchResultHeader.Size = new System.Drawing.Size(279, 0);
            this.tabPageSearchResultHeader.TabIndex = 1;
            this.tabPageSearchResultHeader.Text = "찾기결과";
            // 
            // tabControlBody
            // 
            this.tabControlBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlBody.Controls.Add(this.tabPageIndex);
            this.tabControlBody.Controls.Add(this.tabPageSearchResult);
            this.tabControlBody.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlBody.ItemSize = new System.Drawing.Size(120, 30);
            this.tabControlBody.Location = new System.Drawing.Point(5, 5);
            this.tabControlBody.Name = "tabControlBody";
            this.tabControlBody.SelectedIndex = 0;
            this.tabControlBody.Size = new System.Drawing.Size(287, 410);
            this.tabControlBody.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlBody.TabIndex = 1;
            // 
            // tabPageIndex
            // 
            this.tabPageIndex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.tabPageIndex.Controls.Add(this.treeViewAdv1);
            this.tabPageIndex.Location = new System.Drawing.Point(0, 0);
            this.tabPageIndex.Name = "tabPageIndex";
            this.tabPageIndex.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIndex.Size = new System.Drawing.Size(287, 410);
            this.tabPageIndex.TabIndex = 0;
            this.tabPageIndex.Text = "목차";
            // 
            // treeViewAdv1
            // 
            this.treeViewAdv1.BackColor = System.Drawing.Color.Black;
            this.treeViewAdv1.CurrentFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewAdv1.CurrentTextColor = System.Drawing.Color.White;
            this.treeViewAdv1.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeViewAdv1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewAdv1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdv1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewAdv1.ForeColor = System.Drawing.Color.White;
            this.treeViewAdv1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdv1.Location = new System.Drawing.Point(3, 3);
            this.treeViewAdv1.Model = null;
            this.treeViewAdv1.Name = "treeViewAdv1";
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBox1);
            this.treeViewAdv1.NodeControls.Add(this.nodeStateIcon1);
            this.treeViewAdv1.RowHeight = 40;
            this.treeViewAdv1.SelectedChildColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(33)))), ((int)(((byte)(44)))));
            this.treeViewAdv1.SelectedChildFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewAdv1.SelectedChildTextColor = System.Drawing.Color.White;
            this.treeViewAdv1.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(52)))), ((int)(((byte)(68)))));
            this.treeViewAdv1.SelectedFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewAdv1.SelectedNode = null;
            this.treeViewAdv1.SelectedTextColor = System.Drawing.Color.White;
            this.treeViewAdv1.ShowLines = false;
            this.treeViewAdv1.Size = new System.Drawing.Size(281, 404);
            this.treeViewAdv1.TabIndex = 0;
            this.treeViewAdv1.Text = "treeViewAdv1";
            this.treeViewAdv1.TextColor = System.Drawing.Color.White;
            this.treeViewAdv1.SelectionChanged += new System.EventHandler(this.treeViewAdv1_SelectionChanged);
            // 
            // tabPageSearchResult
            // 
            this.tabPageSearchResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.tabPageSearchResult.Controls.Add(this.panelSearchResult);
            this.tabPageSearchResult.Location = new System.Drawing.Point(0, 0);
            this.tabPageSearchResult.Name = "tabPageSearchResult";
            this.tabPageSearchResult.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSearchResult.Size = new System.Drawing.Size(287, 410);
            this.tabPageSearchResult.TabIndex = 1;
            this.tabPageSearchResult.Text = "찾기결과";
            // 
            // panelSearchResult
            // 
            this.panelSearchResult.AutoScroll = true;
            this.panelSearchResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(54)))));
            this.panelSearchResult.Controls.Add(this.labelSearchResult);
            this.panelSearchResult.Controls.Add(this.pictureBoxSearchResultIcon);
            this.panelSearchResult.Controls.Add(this.pictureBoxSearchResultLine);
            this.panelSearchResult.Controls.Add(this.labelNoResult);
            this.panelSearchResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSearchResult.Location = new System.Drawing.Point(3, 3);
            this.panelSearchResult.Name = "panelSearchResult";
            this.panelSearchResult.Size = new System.Drawing.Size(281, 404);
            this.panelSearchResult.TabIndex = 0;
            // 
            // labelSearchResult
            // 
            this.labelSearchResult.AutoSize = true;
            this.labelSearchResult.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSearchResult.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(118)))), ((int)(((byte)(118)))), ((int)(((byte)(118)))));
            this.labelSearchResult.Location = new System.Drawing.Point(24, 16);
            this.labelSearchResult.Name = "labelSearchResult";
            this.labelSearchResult.Size = new System.Drawing.Size(65, 17);
            this.labelSearchResult.TabIndex = 3;
            this.labelSearchResult.Text = "찾기 결과";
            // 
            // pictureBoxSearchResultIcon
            // 
            this.pictureBoxSearchResultIcon.BackgroundImage = global::HelpViewer.Properties.Resources.left_search;
            this.pictureBoxSearchResultIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxSearchResultIcon.Location = new System.Drawing.Point(12, 18);
            this.pictureBoxSearchResultIcon.Name = "pictureBoxSearchResultIcon";
            this.pictureBoxSearchResultIcon.Size = new System.Drawing.Size(14, 13);
            this.pictureBoxSearchResultIcon.TabIndex = 2;
            this.pictureBoxSearchResultIcon.TabStop = false;
            // 
            // pictureBoxSearchResultLine
            // 
            this.pictureBoxSearchResultLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxSearchResultLine.BackgroundImage = global::HelpViewer.Properties.Resources.SearchResultLine;
            this.pictureBoxSearchResultLine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxSearchResultLine.Location = new System.Drawing.Point(14, 37);
            this.pictureBoxSearchResultLine.Name = "pictureBoxSearchResultLine";
            this.pictureBoxSearchResultLine.Size = new System.Drawing.Size(253, 1);
            this.pictureBoxSearchResultLine.TabIndex = 1;
            this.pictureBoxSearchResultLine.TabStop = false;
            // 
            // labelNoResult
            // 
            this.labelNoResult.AutoSize = true;
            this.labelNoResult.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNoResult.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(112)))), ((int)(((byte)(159)))));
            this.labelNoResult.Location = new System.Drawing.Point(19, 45);
            this.labelNoResult.Name = "labelNoResult";
            this.labelNoResult.Size = new System.Drawing.Size(170, 21);
            this.labelNoResult.TabIndex = 0;
            this.labelNoResult.Text = "찾는 결과가 없습니다.";
            // 
            // webViewer
            // 
            this.webViewer.AllowWebBrowserDrop = false;
            this.webViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webViewer.IsWebBrowserContextMenuEnabled = false;
            this.webViewer.Location = new System.Drawing.Point(0, 5);
            this.webViewer.MinimumSize = new System.Drawing.Size(20, 20);
            this.webViewer.Name = "webViewer";
            this.webViewer.Size = new System.Drawing.Size(579, 440);
            this.webViewer.TabIndex = 0;
            this.webViewer.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webViewer_DocumentCompleted);
            this.webViewer.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webViewer_Navigated);
            // 
            // printMenu
            // 
            this.printMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.printMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuPrint,
            this.tsMenuPrintPreview,
            this.tsPrintAll});
            this.printMenu.Name = "printMenu";
            this.printMenu.Size = new System.Drawing.Size(160, 70);
            // 
            // tsMenuPrint
            // 
            this.tsMenuPrint.Name = "tsMenuPrint";
            this.tsMenuPrint.Size = new System.Drawing.Size(159, 22);
            this.tsMenuPrint.Text = "인쇄...";
            this.tsMenuPrint.Click += new System.EventHandler(this.tsMenuPrint_Click);
            // 
            // tsMenuPrintPreview
            // 
            this.tsMenuPrintPreview.Name = "tsMenuPrintPreview";
            this.tsMenuPrintPreview.Size = new System.Drawing.Size(159, 22);
            this.tsMenuPrintPreview.Text = "인쇄 미리보기...";
            this.tsMenuPrintPreview.Click += new System.EventHandler(this.tsMenuPrintPreview_Click);
            // 
            // tsPrintAll
            // 
            this.tsPrintAll.Name = "tsPrintAll";
            this.tsPrintAll.Size = new System.Drawing.Size(159, 22);
            this.tsPrintAll.Text = "전체 인쇄";
            this.tsPrintAll.Visible = false;
            this.tsPrintAll.Click += new System.EventHandler(this.tsPrintAll_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 496);
            this.Controls.Add(this.splitContainerBody);
            this.Controls.Add(this.panelTop);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "유엔이 도움말 뷰어 V2.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResizeBegin += new System.EventHandler(this.FormMain_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.FormMain_ResizeEnd);
            this.panelTop.ResumeLayout(false);
            this.panelRed.ResumeLayout(false);
            this.panelRed.PerformLayout();
            this.panelSearchArea.ResumeLayout(false);
            this.panelSearchArea.PerformLayout();
            this.panelSearchLabel.ResumeLayout(false);
            this.panelSearchLabel.PerformLayout();
            this.splitContainerBody.Panel1.ResumeLayout(false);
            this.splitContainerBody.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBody)).EndInit();
            this.splitContainerBody.ResumeLayout(false);
            this.tabControlHeader.ResumeLayout(false);
            this.tabControlBody.ResumeLayout(false);
            this.tabPageIndex.ResumeLayout(false);
            this.tabPageSearchResult.ResumeLayout(false);
            this.panelSearchResult.ResumeLayout(false);
            this.panelSearchResult.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearchResultIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearchResultLine)).EndInit();
            this.printMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.SplitContainer splitContainerBody;
        private System.Windows.Forms.WebBrowser webViewer;
        private HelpViewer.RibbonButton rbtnUndo;
        private RibbonButton rbtnRedo;
        private RibbonButton rbtnPrint;
        private System.Windows.Forms.ContextMenuStrip printMenu;
        private System.Windows.Forms.ToolStripMenuItem tsMenuPrint;
        private System.Windows.Forms.ToolStripMenuItem tsMenuPrintPreview;
        private System.Windows.Forms.TextBox textBoxSearch;
        private RibbonButton rbtnSearch;
        private System.Windows.Forms.Panel panelSearchArea;
        private TabControlBody tabControlBody;
        private System.Windows.Forms.TabPage tabPageIndex;
        private System.Windows.Forms.TabPage tabPageSearchResult;
        private TabControlHeader tabControlHeader;
        private System.Windows.Forms.TabPage tabPageIndexHeader;
        private System.Windows.Forms.TabPage tabPageSearchResultHeader;
        private System.Windows.Forms.Panel panelSearchResult;
        private System.Windows.Forms.Label labelNoResult;
        private System.Windows.Forms.Panel panelRed;
        private System.Windows.Forms.Label labelSystemName;
        private System.Windows.Forms.Panel panelSearchLabel;
        private System.Windows.Forms.Label labelSearchLabel;
        private Aga.Controls.Tree.TreeViewAdv treeViewAdv1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.NodeControls.NodeStateIcon nodeStateIcon1;
        private System.Windows.Forms.Label labelSearchResult;
        private System.Windows.Forms.PictureBox pictureBoxSearchResultIcon;
        private System.Windows.Forms.PictureBox pictureBoxSearchResultLine;
        private System.Windows.Forms.ToolStripMenuItem tsPrintAll;
        private System.Drawing.Printing.PrintDocument printDocument1;
    }
}

