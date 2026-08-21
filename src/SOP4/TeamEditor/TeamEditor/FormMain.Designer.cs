namespace TeamEditor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            this.paneRibbonToolBar = new System.Windows.Forms.Panel();
            this.rbtnOption = new UnE.GUI.RibbonButton();
            this.rbtnImportRegular = new UnE.GUI.RibbonButton();
            this.rbtnEmergency = new UnE.GUI.RibbonButton();
            this.rbtnNormal = new UnE.GUI.RibbonButton();
            this.rbtnUserDefined = new UnE.GUI.RibbonButton();
            this.rbtnExternal = new UnE.GUI.RibbonButton();
            this.rbtnRegular = new UnE.GUI.RibbonButton();
            this.rbtnRedo = new UnE.GUI.RibbonButton();
            this.rbtnUndo = new UnE.GUI.RibbonButton();
            this.rbtnEdit = new UnE.GUI.RibbonButton();
            this.rbtnSave = new UnE.GUI.RibbonButton();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.treeExternalCompanyTeam = new TeamEditor.TeamTreeView();
            this.treeRegularTeam = new TeamEditor.TeamTreeView();
            this.panelExternal = new System.Windows.Forms.Panel();
            this.lblExternalServerState = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTeamPathForExternal = new System.Windows.Forms.Label();
            this.gridExternal = new TeamEditor.TeamGrid();
            this.colNo3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExternalName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExternalLevel = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colExternalPosition = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colExternalPhoneNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEtc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelRegular = new System.Windows.Forms.Panel();
            this.lblRegularServerState = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTeamPathForRegular = new System.Windows.Forms.Label();
            this.gridRegularMember = new TeamEditor.TeamGrid();
            this.colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPosition = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colSubPosition = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colLevel = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colSubLevel = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colPhoneNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGroupPosition = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colMemberID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOfficePhoneNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainerEmergency = new System.Windows.Forms.SplitContainer();
            this.treeEmergency = new TeamEditor.TeamTreeView();
            this.treeNormal = new TeamEditor.TeamTreeView();
            this.panelTemporary = new System.Windows.Forms.Panel();
            this.lblTemporaryServerState = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblTeamPathForTemporary = new System.Windows.Forms.Label();
            this.panelBand2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.gridTemporary = new TeamEditor.TeamGrid();
            this.panelBand1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuRegularTeam = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuAddTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuDeleteTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuRenameTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListDrag = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuTemporaryTeam = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuNewGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuAddTempTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuDeleteTempTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuRenameTempTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.colMemberType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.imageListAlwaysTreeIcon = new System.Windows.Forms.ImageList(this.components);
            this.imageListEmergencyTreeIcon = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuExternal = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuNewExternalTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuAddExternalCompanyTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuRemoveExternal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuRenameExternalCompanyTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMain = new System.Windows.Forms.Panel();
            this.gridUserDefinedTeam = new TeamEditor.TeamGrid();
            this.colNo4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUserDefinedTeamName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUserDefinedTeamPhoneNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUserDefinedTeamFaxNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colManager = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colTemporaryMemberName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeamButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colPosition2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colManager2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colManager2Button = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIncludeSubTeams = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.paneRibbonToolBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panelExternal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridExternal)).BeginInit();
            this.panelRegular.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRegularMember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerEmergency)).BeginInit();
            this.splitContainerEmergency.Panel1.SuspendLayout();
            this.splitContainerEmergency.Panel2.SuspendLayout();
            this.splitContainerEmergency.SuspendLayout();
            this.panelTemporary.SuspendLayout();
            this.panelBand2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTemporary)).BeginInit();
            this.panelBand1.SuspendLayout();
            this.contextMenuRegularTeam.SuspendLayout();
            this.contextMenuTemporaryTeam.SuspendLayout();
            this.contextMenuExternal.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUserDefinedTeam)).BeginInit();
            this.SuspendLayout();
            // 
            // paneRibbonToolBar
            // 
            this.paneRibbonToolBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.paneRibbonToolBar.Controls.Add(this.rbtnOption);
            this.paneRibbonToolBar.Controls.Add(this.rbtnImportRegular);
            this.paneRibbonToolBar.Controls.Add(this.rbtnEmergency);
            this.paneRibbonToolBar.Controls.Add(this.rbtnNormal);
            this.paneRibbonToolBar.Controls.Add(this.rbtnUserDefined);
            this.paneRibbonToolBar.Controls.Add(this.rbtnExternal);
            this.paneRibbonToolBar.Controls.Add(this.rbtnRegular);
            this.paneRibbonToolBar.Controls.Add(this.rbtnRedo);
            this.paneRibbonToolBar.Controls.Add(this.rbtnUndo);
            this.paneRibbonToolBar.Controls.Add(this.rbtnEdit);
            this.paneRibbonToolBar.Controls.Add(this.rbtnSave);
            this.paneRibbonToolBar.Controls.Add(this.pictureBox3);
            this.paneRibbonToolBar.Controls.Add(this.pictureBox2);
            this.paneRibbonToolBar.Controls.Add(this.pictureBox1);
            this.paneRibbonToolBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.paneRibbonToolBar.Location = new System.Drawing.Point(0, 0);
            this.paneRibbonToolBar.Name = "paneRibbonToolBar";
            this.paneRibbonToolBar.Size = new System.Drawing.Size(1122, 88);
            this.paneRibbonToolBar.TabIndex = 0;
            // 
            // rbtnOption
            // 
            this.rbtnOption.BackColor = System.Drawing.Color.Transparent;
            this.rbtnOption.CheckButton = false;
            this.rbtnOption.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnOption.CheckedImage = global::TeamEditor.Properties.Resources.Option_checked;
            this.rbtnOption.ClickedBackgroundImage = null;
            this.rbtnOption.ClickedImage = null;
            this.rbtnOption.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnOption.DisabledBkgndImage = null;
            this.rbtnOption.DisabledImage = global::TeamEditor.Properties.Resources.Option_disabled;
            this.rbtnOption.ID = -1;
            this.rbtnOption.InitButtonWidth = 70;
            this.rbtnOption.IsChecked = false;
            this.rbtnOption.Location = new System.Drawing.Point(831, 9);
            this.rbtnOption.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnOption.MouseOverImage = null;
            this.rbtnOption.Name = "rbtnOption";
            this.rbtnOption.NormalImage = global::TeamEditor.Properties.Resources.Option_normal;
            this.rbtnOption.Owner = null;
            this.rbtnOption.Size = new System.Drawing.Size(70, 70);
            this.rbtnOption.TabIndex = 3;
            this.rbtnOption.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnOption.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnOption.ToolTipText = "";
            this.rbtnOption.UseCustomImageRect = true;
            this.rbtnOption.UseTextLocation = false;
            this.rbtnOption.UseVisualStyleBackColor = false;
            this.rbtnOption.Click += new System.EventHandler(this.rbtnOption_Click);
            // 
            // rbtnImportRegular
            // 
            this.rbtnImportRegular.BackColor = System.Drawing.Color.Transparent;
            this.rbtnImportRegular.CheckButton = false;
            this.rbtnImportRegular.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnImportRegular.CheckedImage = global::TeamEditor.Properties.Resources.import_Checked;
            this.rbtnImportRegular.ClickedBackgroundImage = null;
            this.rbtnImportRegular.ClickedImage = null;
            this.rbtnImportRegular.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnImportRegular.DisabledBkgndImage = null;
            this.rbtnImportRegular.DisabledImage = global::TeamEditor.Properties.Resources.import_Disabled;
            this.rbtnImportRegular.ID = -1;
            this.rbtnImportRegular.InitButtonWidth = 70;
            this.rbtnImportRegular.IsChecked = false;
            this.rbtnImportRegular.Location = new System.Drawing.Point(750, 9);
            this.rbtnImportRegular.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnImportRegular.MouseOverImage = null;
            this.rbtnImportRegular.Name = "rbtnImportRegular";
            this.rbtnImportRegular.NormalImage = global::TeamEditor.Properties.Resources.import_normal;
            this.rbtnImportRegular.Owner = null;
            this.rbtnImportRegular.Size = new System.Drawing.Size(70, 70);
            this.rbtnImportRegular.TabIndex = 2;
            this.rbtnImportRegular.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnImportRegular.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnImportRegular.ToolTipText = "";
            this.rbtnImportRegular.UseCustomImageRect = true;
            this.rbtnImportRegular.UseTextLocation = false;
            this.rbtnImportRegular.UseVisualStyleBackColor = false;
            this.rbtnImportRegular.Click += new System.EventHandler(this.rbtnImportRegular_Click);
            // 
            // rbtnEmergency
            // 
            this.rbtnEmergency.BackColor = System.Drawing.Color.Transparent;
            this.rbtnEmergency.CheckButton = false;
            this.rbtnEmergency.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnEmergency.CheckedImage = global::TeamEditor.Properties.Resources.holiday_checked;
            this.rbtnEmergency.ClickedBackgroundImage = null;
            this.rbtnEmergency.ClickedImage = null;
            this.rbtnEmergency.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnEmergency.DisabledBkgndImage = null;
            this.rbtnEmergency.DisabledImage = global::TeamEditor.Properties.Resources.holiday_disabled;
            this.rbtnEmergency.ID = -1;
            this.rbtnEmergency.InitButtonWidth = 70;
            this.rbtnEmergency.IsChecked = false;
            this.rbtnEmergency.Location = new System.Drawing.Point(507, 9);
            this.rbtnEmergency.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnEmergency.MouseOverImage = null;
            this.rbtnEmergency.Name = "rbtnEmergency";
            this.rbtnEmergency.NormalImage = global::TeamEditor.Properties.Resources.holiday_normal;
            this.rbtnEmergency.Owner = null;
            this.rbtnEmergency.Size = new System.Drawing.Size(70, 70);
            this.rbtnEmergency.TabIndex = 1;
            this.rbtnEmergency.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnEmergency.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnEmergency.ToolTipText = "";
            this.rbtnEmergency.UseCustomImageRect = true;
            this.rbtnEmergency.UseTextLocation = false;
            this.rbtnEmergency.UseVisualStyleBackColor = false;
            this.rbtnEmergency.Click += new System.EventHandler(this.rbtnEmergency_Click);
            // 
            // rbtnNormal
            // 
            this.rbtnNormal.BackColor = System.Drawing.Color.Transparent;
            this.rbtnNormal.CheckButton = false;
            this.rbtnNormal.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnNormal.CheckedImage = global::TeamEditor.Properties.Resources.weekday_checked;
            this.rbtnNormal.ClickedBackgroundImage = null;
            this.rbtnNormal.ClickedImage = null;
            this.rbtnNormal.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnNormal.DisabledBkgndImage = null;
            this.rbtnNormal.DisabledImage = global::TeamEditor.Properties.Resources.weekday_disabled;
            this.rbtnNormal.ID = -1;
            this.rbtnNormal.InitButtonWidth = 70;
            this.rbtnNormal.IsChecked = false;
            this.rbtnNormal.Location = new System.Drawing.Point(431, 9);
            this.rbtnNormal.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnNormal.MouseOverImage = null;
            this.rbtnNormal.Name = "rbtnNormal";
            this.rbtnNormal.NormalImage = global::TeamEditor.Properties.Resources.weekday_normal;
            this.rbtnNormal.Owner = null;
            this.rbtnNormal.Size = new System.Drawing.Size(70, 70);
            this.rbtnNormal.TabIndex = 1;
            this.rbtnNormal.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnNormal.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnNormal.ToolTipText = "";
            this.rbtnNormal.UseCustomImageRect = true;
            this.rbtnNormal.UseTextLocation = false;
            this.rbtnNormal.UseVisualStyleBackColor = false;
            this.rbtnNormal.Click += new System.EventHandler(this.rbtnNormal_Click);
            // 
            // rbtnUserDefined
            // 
            this.rbtnUserDefined.BackColor = System.Drawing.Color.Transparent;
            this.rbtnUserDefined.CheckButton = false;
            this.rbtnUserDefined.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnUserDefined.CheckedImage = global::TeamEditor.Properties.Resources.UserDefine_checked;
            this.rbtnUserDefined.ClickedBackgroundImage = null;
            this.rbtnUserDefined.ClickedImage = null;
            this.rbtnUserDefined.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnUserDefined.DisabledBkgndImage = null;
            this.rbtnUserDefined.DisabledImage = global::TeamEditor.Properties.Resources.UserDefine_disabled;
            this.rbtnUserDefined.ID = -1;
            this.rbtnUserDefined.InitButtonWidth = 70;
            this.rbtnUserDefined.IsChecked = false;
            this.rbtnUserDefined.Location = new System.Drawing.Point(669, 9);
            this.rbtnUserDefined.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnUserDefined.MouseOverImage = null;
            this.rbtnUserDefined.Name = "rbtnUserDefined";
            this.rbtnUserDefined.NormalImage = global::TeamEditor.Properties.Resources.UserDefine_normal;
            this.rbtnUserDefined.Owner = null;
            this.rbtnUserDefined.Size = new System.Drawing.Size(70, 70);
            this.rbtnUserDefined.TabIndex = 1;
            this.rbtnUserDefined.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnUserDefined.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnUserDefined.ToolTipText = "";
            this.rbtnUserDefined.UseCustomImageRect = true;
            this.rbtnUserDefined.UseTextLocation = false;
            this.rbtnUserDefined.UseVisualStyleBackColor = false;
            this.rbtnUserDefined.Click += new System.EventHandler(this.rbtnUserDefined_Click);
            // 
            // rbtnExternal
            // 
            this.rbtnExternal.BackColor = System.Drawing.Color.Transparent;
            this.rbtnExternal.CheckButton = false;
            this.rbtnExternal.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnExternal.CheckedImage = global::TeamEditor.Properties.Resources.External_Checked;
            this.rbtnExternal.ClickedBackgroundImage = null;
            this.rbtnExternal.ClickedImage = null;
            this.rbtnExternal.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnExternal.DisabledBkgndImage = null;
            this.rbtnExternal.DisabledImage = global::TeamEditor.Properties.Resources.External_Disabled;
            this.rbtnExternal.ID = -1;
            this.rbtnExternal.InitButtonWidth = 70;
            this.rbtnExternal.IsChecked = false;
            this.rbtnExternal.Location = new System.Drawing.Point(593, 9);
            this.rbtnExternal.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnExternal.MouseOverImage = null;
            this.rbtnExternal.Name = "rbtnExternal";
            this.rbtnExternal.NormalImage = global::TeamEditor.Properties.Resources.External_normal;
            this.rbtnExternal.Owner = null;
            this.rbtnExternal.Size = new System.Drawing.Size(70, 70);
            this.rbtnExternal.TabIndex = 1;
            this.rbtnExternal.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnExternal.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnExternal.ToolTipText = "";
            this.rbtnExternal.UseCustomImageRect = true;
            this.rbtnExternal.UseTextLocation = false;
            this.rbtnExternal.UseVisualStyleBackColor = false;
            this.rbtnExternal.Click += new System.EventHandler(this.rbtnExternal_Click);
            // 
            // rbtnRegular
            // 
            this.rbtnRegular.BackColor = System.Drawing.Color.Transparent;
            this.rbtnRegular.CheckButton = false;
            this.rbtnRegular.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnRegular.CheckedImage = global::TeamEditor.Properties.Resources.always_checked;
            this.rbtnRegular.ClickedBackgroundImage = null;
            this.rbtnRegular.ClickedImage = null;
            this.rbtnRegular.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnRegular.DisabledBkgndImage = null;
            this.rbtnRegular.DisabledImage = global::TeamEditor.Properties.Resources.always_disabled;
            this.rbtnRegular.ID = -1;
            this.rbtnRegular.InitButtonWidth = 70;
            this.rbtnRegular.IsChecked = false;
            this.rbtnRegular.Location = new System.Drawing.Point(355, 9);
            this.rbtnRegular.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnRegular.MouseOverImage = null;
            this.rbtnRegular.Name = "rbtnRegular";
            this.rbtnRegular.NormalImage = global::TeamEditor.Properties.Resources.always_normal;
            this.rbtnRegular.Owner = null;
            this.rbtnRegular.Size = new System.Drawing.Size(70, 70);
            this.rbtnRegular.TabIndex = 1;
            this.rbtnRegular.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnRegular.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnRegular.ToolTipText = "";
            this.rbtnRegular.UseCustomImageRect = true;
            this.rbtnRegular.UseTextLocation = false;
            this.rbtnRegular.UseVisualStyleBackColor = false;
            this.rbtnRegular.Click += new System.EventHandler(this.rbtnRegular_Click);
            // 
            // rbtnRedo
            // 
            this.rbtnRedo.BackColor = System.Drawing.Color.Transparent;
            this.rbtnRedo.CheckButton = false;
            this.rbtnRedo.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnRedo.CheckedImage = null;
            this.rbtnRedo.ClickedBackgroundImage = null;
            this.rbtnRedo.ClickedImage = null;
            this.rbtnRedo.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnRedo.DisabledBkgndImage = null;
            this.rbtnRedo.DisabledImage = global::TeamEditor.Properties.Resources.redo_disabled;
            this.rbtnRedo.Enabled = false;
            this.rbtnRedo.ID = -1;
            this.rbtnRedo.InitButtonWidth = 70;
            this.rbtnRedo.IsChecked = false;
            this.rbtnRedo.Location = new System.Drawing.Point(268, 9);
            this.rbtnRedo.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnRedo.MouseOverImage = null;
            this.rbtnRedo.Name = "rbtnRedo";
            this.rbtnRedo.NormalImage = global::TeamEditor.Properties.Resources.redo_normal;
            this.rbtnRedo.Owner = null;
            this.rbtnRedo.Size = new System.Drawing.Size(70, 70);
            this.rbtnRedo.TabIndex = 1;
            this.rbtnRedo.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnRedo.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnRedo.ToolTipText = "";
            this.rbtnRedo.UseCustomImageRect = true;
            this.rbtnRedo.UseTextLocation = false;
            this.rbtnRedo.UseVisualStyleBackColor = false;
            // 
            // rbtnUndo
            // 
            this.rbtnUndo.BackColor = System.Drawing.Color.Transparent;
            this.rbtnUndo.CheckButton = false;
            this.rbtnUndo.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnUndo.CheckedImage = null;
            this.rbtnUndo.ClickedBackgroundImage = null;
            this.rbtnUndo.ClickedImage = null;
            this.rbtnUndo.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbtnUndo.DisabledBkgndImage = null;
            this.rbtnUndo.DisabledImage = global::TeamEditor.Properties.Resources.undo_disabled;
            this.rbtnUndo.Enabled = false;
            this.rbtnUndo.ID = -1;
            this.rbtnUndo.InitButtonWidth = 70;
            this.rbtnUndo.IsChecked = false;
            this.rbtnUndo.Location = new System.Drawing.Point(192, 9);
            this.rbtnUndo.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnUndo.MouseOverImage = null;
            this.rbtnUndo.Name = "rbtnUndo";
            this.rbtnUndo.NormalImage = global::TeamEditor.Properties.Resources.undo_normal;
            this.rbtnUndo.Owner = null;
            this.rbtnUndo.Size = new System.Drawing.Size(70, 70);
            this.rbtnUndo.TabIndex = 1;
            this.rbtnUndo.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnUndo.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnUndo.ToolTipText = "";
            this.rbtnUndo.UseCustomImageRect = true;
            this.rbtnUndo.UseTextLocation = false;
            this.rbtnUndo.UseVisualStyleBackColor = false;
            // 
            // rbtnEdit
            // 
            this.rbtnEdit.BackColor = System.Drawing.Color.Transparent;
            this.rbtnEdit.CheckButton = false;
            this.rbtnEdit.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnEdit.CheckedImage = global::TeamEditor.Properties.Resources.edit_checked;
            this.rbtnEdit.ClickedBackgroundImage = null;
            this.rbtnEdit.ClickedImage = null;
            this.rbtnEdit.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnEdit.DisabledBkgndImage = null;
            this.rbtnEdit.DisabledImage = global::TeamEditor.Properties.Resources.edit_disabled;
            this.rbtnEdit.ID = -1;
            this.rbtnEdit.InitButtonWidth = 70;
            this.rbtnEdit.IsChecked = false;
            this.rbtnEdit.Location = new System.Drawing.Point(116, 9);
            this.rbtnEdit.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnEdit.MouseOverImage = null;
            this.rbtnEdit.Name = "rbtnEdit";
            this.rbtnEdit.NormalImage = global::TeamEditor.Properties.Resources.edit_normal;
            this.rbtnEdit.Owner = null;
            this.rbtnEdit.Size = new System.Drawing.Size(70, 70);
            this.rbtnEdit.TabIndex = 1;
            this.rbtnEdit.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnEdit.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnEdit.ToolTipText = "";
            this.rbtnEdit.UseCustomImageRect = true;
            this.rbtnEdit.UseTextLocation = false;
            this.rbtnEdit.UseVisualStyleBackColor = false;
            this.rbtnEdit.Click += new System.EventHandler(this.rbtnEdit_Click);
            // 
            // rbtnSave
            // 
            this.rbtnSave.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSave.CheckButton = false;
            this.rbtnSave.CheckedBkgndImage = global::TeamEditor.Properties.Resources.clicked_background;
            this.rbtnSave.CheckedImage = global::TeamEditor.Properties.Resources.save_checked2;
            this.rbtnSave.ClickedBackgroundImage = null;
            this.rbtnSave.ClickedImage = null;
            this.rbtnSave.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnSave.DisabledBkgndImage = null;
            this.rbtnSave.DisabledImage = global::TeamEditor.Properties.Resources.save_disabled2;
            this.rbtnSave.ID = -1;
            this.rbtnSave.InitButtonWidth = 70;
            this.rbtnSave.IsChecked = false;
            this.rbtnSave.Location = new System.Drawing.Point(40, 9);
            this.rbtnSave.MouseOverBkgndImage = global::TeamEditor.Properties.Resources.mouse_over_background;
            this.rbtnSave.MouseOverImage = null;
            this.rbtnSave.Name = "rbtnSave";
            this.rbtnSave.NormalImage = global::TeamEditor.Properties.Resources.save_normal2;
            this.rbtnSave.Owner = null;
            this.rbtnSave.Size = new System.Drawing.Size(70, 70);
            this.rbtnSave.TabIndex = 1;
            this.rbtnSave.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSave.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSave.ToolTipText = "";
            this.rbtnSave.UseCustomImageRect = true;
            this.rbtnSave.UseTextLocation = false;
            this.rbtnSave.UseVisualStyleBackColor = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::TeamEditor.Properties.Resources.skin_line_img;
            this.pictureBox3.Location = new System.Drawing.Point(583, 5);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(2, 75);
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::TeamEditor.Properties.Resources.skin_line_img;
            this.pictureBox2.Location = new System.Drawing.Point(345, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(2, 75);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::TeamEditor.Properties.Resources.skin_line_img;
            this.pictureBox1.Location = new System.Drawing.Point(30, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(2, 75);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.splitContainerMain.Panel1.Controls.Add(this.treeExternalCompanyTeam);
            this.splitContainerMain.Panel1.Controls.Add(this.treeRegularTeam);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.splitContainerMain.Panel2.Controls.Add(this.panelExternal);
            this.splitContainerMain.Panel2.Controls.Add(this.panelRegular);
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerEmergency);
            this.splitContainerMain.Size = new System.Drawing.Size(1122, 475);
            this.splitContainerMain.SplitterDistance = 243;
            this.splitContainerMain.TabIndex = 1;
            // 
            // treeExternalCompanyTeam
            // 
            this.treeExternalCompanyTeam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeExternalCompanyTeam.DropData = null;
            this.treeExternalCompanyTeam.DropType = TeamEditor.TeamTreeView.DropDataType.NONE;
            this.treeExternalCompanyTeam.HideSelection = false;
            this.treeExternalCompanyTeam.Location = new System.Drawing.Point(0, 0);
            this.treeExternalCompanyTeam.Name = "treeExternalCompanyTeam";
            this.treeExternalCompanyTeam.Size = new System.Drawing.Size(243, 475);
            this.treeExternalCompanyTeam.TabIndex = 1;
            this.treeExternalCompanyTeam.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_AfterLabelEdit);
            this.treeExternalCompanyTeam.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeExternalCompanyTeam_KeyDown);
            // 
            // treeRegularTeam
            // 
            this.treeRegularTeam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeRegularTeam.DropData = null;
            this.treeRegularTeam.DropType = TeamEditor.TeamTreeView.DropDataType.NONE;
            this.treeRegularTeam.HideSelection = false;
            this.treeRegularTeam.Location = new System.Drawing.Point(0, 0);
            this.treeRegularTeam.Name = "treeRegularTeam";
            this.treeRegularTeam.Size = new System.Drawing.Size(243, 475);
            this.treeRegularTeam.TabIndex = 0;
            this.treeRegularTeam.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_AfterLabelEdit);
            this.treeRegularTeam.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeRegularTeam_KeyDown);
            // 
            // panelExternal
            // 
            this.panelExternal.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panelExternal.Controls.Add(this.lblExternalServerState);
            this.panelExternal.Controls.Add(this.panel2);
            this.panelExternal.Controls.Add(this.lblTeamPathForExternal);
            this.panelExternal.Controls.Add(this.gridExternal);
            this.panelExternal.Location = new System.Drawing.Point(418, 25);
            this.panelExternal.Name = "panelExternal";
            this.panelExternal.Size = new System.Drawing.Size(397, 100);
            this.panelExternal.TabIndex = 3;
            // 
            // lblExternalServerState
            // 
            this.lblExternalServerState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExternalServerState.Location = new System.Drawing.Point(144, 6);
            this.lblExternalServerState.Name = "lblExternalServerState";
            this.lblExternalServerState.Size = new System.Drawing.Size(250, 12);
            this.lblExternalServerState.TabIndex = 9;
            this.lblExternalServerState.Text = "       ";
            this.lblExternalServerState.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblExternalServerState.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(0, 21);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(397, 1);
            this.panel2.TabIndex = 8;
            // 
            // lblTeamPathForExternal
            // 
            this.lblTeamPathForExternal.AutoSize = true;
            this.lblTeamPathForExternal.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTeamPathForExternal.Location = new System.Drawing.Point(3, 6);
            this.lblTeamPathForExternal.Name = "lblTeamPathForExternal";
            this.lblTeamPathForExternal.Size = new System.Drawing.Size(40, 12);
            this.lblTeamPathForExternal.TabIndex = 7;
            this.lblTeamPathForExternal.Text = "       ";
            // 
            // gridExternal
            // 
            this.gridExternal.AllowUserToAddRows = false;
            this.gridExternal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridExternal.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridExternal.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gridExternal.ColumnHeadersHeight = 32;
            this.gridExternal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridExternal.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo3,
            this.colExternalName,
            this.colExternalLevel,
            this.colExternalPosition,
            this.colExternalPhoneNumber,
            this.colEtc});
            this.gridExternal.CurrentTeam = null;
            this.gridExternal.groupPosition = null;
            this.gridExternal.LinkedTree = null;
            this.gridExternal.Location = new System.Drawing.Point(0, 21);
            this.gridExternal.Name = "gridExternal";
            this.gridExternal.NoSort = false;
            this.gridExternal.RowHeadersVisible = false;
            this.gridExternal.RowTemplate.Height = 23;
            this.gridExternal.Size = new System.Drawing.Size(397, 79);
            this.gridExternal.TabIndex = 0;
            this.gridExternal.Type = TeamEditor.TeamGrid.GridType.ExternalCompanyTeam;
            // 
            // colNo3
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo3.DefaultCellStyle = dataGridViewCellStyle1;
            this.colNo3.HeaderText = "번호";
            this.colNo3.Name = "colNo3";
            this.colNo3.Width = 40;
            // 
            // colExternalName
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colExternalName.DefaultCellStyle = dataGridViewCellStyle2;
            this.colExternalName.HeaderText = "이름";
            this.colExternalName.Name = "colExternalName";
            // 
            // colExternalLevel
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colExternalLevel.DefaultCellStyle = dataGridViewCellStyle3;
            this.colExternalLevel.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colExternalLevel.HeaderText = "직급";
            this.colExternalLevel.Name = "colExternalLevel";
            // 
            // colExternalPosition
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colExternalPosition.DefaultCellStyle = dataGridViewCellStyle4;
            this.colExternalPosition.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colExternalPosition.HeaderText = "직책";
            this.colExternalPosition.Name = "colExternalPosition";
            this.colExternalPosition.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colExternalPosition.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colExternalPhoneNumber
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colExternalPhoneNumber.DefaultCellStyle = dataGridViewCellStyle5;
            this.colExternalPhoneNumber.HeaderText = "전화번호";
            this.colExternalPhoneNumber.Name = "colExternalPhoneNumber";
            // 
            // colEtc
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colEtc.DefaultCellStyle = dataGridViewCellStyle6;
            this.colEtc.HeaderText = "비고";
            this.colEtc.Name = "colEtc";
            this.colEtc.Width = 200;
            // 
            // panelRegular
            // 
            this.panelRegular.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panelRegular.Controls.Add(this.lblRegularServerState);
            this.panelRegular.Controls.Add(this.panel1);
            this.panelRegular.Controls.Add(this.lblTeamPathForRegular);
            this.panelRegular.Controls.Add(this.gridRegularMember);
            this.panelRegular.Location = new System.Drawing.Point(21, 6);
            this.panelRegular.Name = "panelRegular";
            this.panelRegular.Size = new System.Drawing.Size(362, 257);
            this.panelRegular.TabIndex = 2;
            // 
            // lblRegularServerState
            // 
            this.lblRegularServerState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRegularServerState.Location = new System.Drawing.Point(109, 6);
            this.lblRegularServerState.Name = "lblRegularServerState";
            this.lblRegularServerState.Size = new System.Drawing.Size(250, 12);
            this.lblRegularServerState.TabIndex = 8;
            this.lblRegularServerState.Text = "       ";
            this.lblRegularServerState.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRegularServerState.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(0, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(362, 1);
            this.panel1.TabIndex = 7;
            // 
            // lblTeamPathForRegular
            // 
            this.lblTeamPathForRegular.AutoSize = true;
            this.lblTeamPathForRegular.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTeamPathForRegular.Location = new System.Drawing.Point(3, 6);
            this.lblTeamPathForRegular.Name = "lblTeamPathForRegular";
            this.lblTeamPathForRegular.Size = new System.Drawing.Size(40, 12);
            this.lblTeamPathForRegular.TabIndex = 6;
            this.lblTeamPathForRegular.Text = "       ";
            // 
            // gridRegularMember
            // 
            this.gridRegularMember.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridRegularMember.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridRegularMember.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gridRegularMember.ColumnHeadersHeight = 32;
            this.gridRegularMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridRegularMember.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIndex,
            this.colName,
            this.colPosition,
            this.colSubPosition,
            this.colLevel,
            this.colSubLevel,
            this.colPhoneNumber,
            this.colGroupPosition,
            this.colMemberID,
            this.colOfficePhoneNumber});
            this.gridRegularMember.CurrentTeam = null;
            this.gridRegularMember.groupPosition = null;
            this.gridRegularMember.LinkedTree = null;
            this.gridRegularMember.Location = new System.Drawing.Point(0, 21);
            this.gridRegularMember.Name = "gridRegularMember";
            this.gridRegularMember.NoSort = false;
            this.gridRegularMember.RowHeadersVisible = false;
            this.gridRegularMember.RowTemplate.Height = 23;
            this.gridRegularMember.Size = new System.Drawing.Size(362, 236);
            this.gridRegularMember.TabIndex = 1;
            this.gridRegularMember.Type = TeamEditor.TeamGrid.GridType.RegularMember;
            // 
            // colIndex
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colIndex.DefaultCellStyle = dataGridViewCellStyle7;
            this.colIndex.HeaderText = "번호";
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            this.colIndex.Width = 40;
            // 
            // colName
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colName.DefaultCellStyle = dataGridViewCellStyle8;
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            // 
            // colPosition
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colPosition.DefaultCellStyle = dataGridViewCellStyle9;
            this.colPosition.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colPosition.HeaderText = "직위";
            this.colPosition.Name = "colPosition";
            this.colPosition.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colSubPosition
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSubPosition.DefaultCellStyle = dataGridViewCellStyle10;
            this.colSubPosition.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colSubPosition.HeaderText = "직위상세";
            this.colSubPosition.Name = "colSubPosition";
            this.colSubPosition.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSubPosition.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colLevel
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colLevel.DefaultCellStyle = dataGridViewCellStyle11;
            this.colLevel.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colLevel.HeaderText = "직급";
            this.colLevel.Items.AddRange(new object[] {
            "알수없음",
            "1급",
            "2급",
            "3급",
            "4급",
            "5급",
            "6급",
            "7급",
            "8급",
            "9급"});
            this.colLevel.Name = "colLevel";
            this.colLevel.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colLevel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colSubLevel
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSubLevel.DefaultCellStyle = dataGridViewCellStyle12;
            this.colSubLevel.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colSubLevel.HeaderText = "직급상세";
            this.colSubLevel.Name = "colSubLevel";
            // 
            // colPhoneNumber
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colPhoneNumber.DefaultCellStyle = dataGridViewCellStyle13;
            this.colPhoneNumber.HeaderText = "휴대전화번호";
            this.colPhoneNumber.Name = "colPhoneNumber";
            this.colPhoneNumber.Width = 200;
            // 
            // colGroupPosition
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colGroupPosition.DefaultCellStyle = dataGridViewCellStyle14;
            this.colGroupPosition.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colGroupPosition.HeaderText = "직군";
            this.colGroupPosition.Name = "colGroupPosition";
            // 
            // colMemberID
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colMemberID.DefaultCellStyle = dataGridViewCellStyle15;
            this.colMemberID.HeaderText = "사번";
            this.colMemberID.Name = "colMemberID";
            // 
            // colOfficePhoneNumber
            // 
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colOfficePhoneNumber.DefaultCellStyle = dataGridViewCellStyle16;
            this.colOfficePhoneNumber.HeaderText = "근무처 전화번호";
            this.colOfficePhoneNumber.Name = "colOfficePhoneNumber";
            this.colOfficePhoneNumber.Width = 200;
            // 
            // splitContainerEmergency
            // 
            this.splitContainerEmergency.Location = new System.Drawing.Point(108, 269);
            this.splitContainerEmergency.Name = "splitContainerEmergency";
            // 
            // splitContainerEmergency.Panel1
            // 
            this.splitContainerEmergency.Panel1.Controls.Add(this.treeEmergency);
            this.splitContainerEmergency.Panel1.Controls.Add(this.treeNormal);
            // 
            // splitContainerEmergency.Panel2
            // 
            this.splitContainerEmergency.Panel2.Controls.Add(this.panelTemporary);
            this.splitContainerEmergency.Size = new System.Drawing.Size(707, 181);
            this.splitContainerEmergency.SplitterDistance = 182;
            this.splitContainerEmergency.TabIndex = 0;
            // 
            // treeEmergency
            // 
            this.treeEmergency.DropData = null;
            this.treeEmergency.DropType = TeamEditor.TeamTreeView.DropDataType.NONE;
            this.treeEmergency.HideSelection = false;
            this.treeEmergency.Location = new System.Drawing.Point(22, 171);
            this.treeEmergency.Name = "treeEmergency";
            this.treeEmergency.Size = new System.Drawing.Size(135, 97);
            this.treeEmergency.TabIndex = 0;
            this.treeEmergency.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_AfterLabelEdit);
            // 
            // treeNormal
            // 
            this.treeNormal.DropData = null;
            this.treeNormal.DropType = TeamEditor.TeamTreeView.DropDataType.NONE;
            this.treeNormal.HideSelection = false;
            this.treeNormal.Location = new System.Drawing.Point(22, 30);
            this.treeNormal.Name = "treeNormal";
            this.treeNormal.Size = new System.Drawing.Size(135, 97);
            this.treeNormal.TabIndex = 0;
            this.treeNormal.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_AfterLabelEdit);
            // 
            // panelTemporary
            // 
            this.panelTemporary.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panelTemporary.Controls.Add(this.lblTemporaryServerState);
            this.panelTemporary.Controls.Add(this.panel3);
            this.panelTemporary.Controls.Add(this.lblTeamPathForTemporary);
            this.panelTemporary.Controls.Add(this.panelBand2);
            this.panelTemporary.Controls.Add(this.gridTemporary);
            this.panelTemporary.Controls.Add(this.panelBand1);
            this.panelTemporary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTemporary.Location = new System.Drawing.Point(0, 0);
            this.panelTemporary.Name = "panelTemporary";
            this.panelTemporary.Size = new System.Drawing.Size(521, 181);
            this.panelTemporary.TabIndex = 1;
            // 
            // lblTemporaryServerState
            // 
            this.lblTemporaryServerState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTemporaryServerState.Location = new System.Drawing.Point(268, 6);
            this.lblTemporaryServerState.Name = "lblTemporaryServerState";
            this.lblTemporaryServerState.Size = new System.Drawing.Size(250, 12);
            this.lblTemporaryServerState.TabIndex = 9;
            this.lblTemporaryServerState.Text = "       ";
            this.lblTemporaryServerState.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTemporaryServerState.Visible = false;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(0, 21);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(521, 1);
            this.panel3.TabIndex = 8;
            // 
            // lblTeamPathForTemporary
            // 
            this.lblTeamPathForTemporary.AutoSize = true;
            this.lblTeamPathForTemporary.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTeamPathForTemporary.Location = new System.Drawing.Point(3, 6);
            this.lblTeamPathForTemporary.Name = "lblTeamPathForTemporary";
            this.lblTeamPathForTemporary.Size = new System.Drawing.Size(40, 12);
            this.lblTeamPathForTemporary.TabIndex = 5;
            this.lblTeamPathForTemporary.Text = "       ";
            // 
            // panelBand2
            // 
            this.panelBand2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panelBand2.Controls.Add(this.label2);
            this.panelBand2.Location = new System.Drawing.Point(340, 21);
            this.panelBand2.Name = "panelBand2";
            this.panelBand2.Size = new System.Drawing.Size(181, 32);
            this.panelBand2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(1, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "정규조직도";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gridTemporary
            // 
            this.gridTemporary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridTemporary.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridTemporary.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gridTemporary.ColumnHeadersHeight = 32;
            this.gridTemporary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridTemporary.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colManager,
            this.colTemporaryMemberName,
            this.colTeam,
            this.colTeamButton,
            this.colPosition2,
            this.colManager2,
            this.colManager2Button,
            this.colNumber,
            this.colIncludeSubTeams});
            this.gridTemporary.CurrentTeam = null;
            this.gridTemporary.groupPosition = null;
            this.gridTemporary.LinkedTree = null;
            this.gridTemporary.Location = new System.Drawing.Point(0, 52);
            this.gridTemporary.Name = "gridTemporary";
            this.gridTemporary.NoSort = false;
            this.gridTemporary.RowHeadersVisible = false;
            this.gridTemporary.RowTemplate.Height = 23;
            this.gridTemporary.Size = new System.Drawing.Size(521, 127);
            this.gridTemporary.TabIndex = 0;
            this.gridTemporary.Type = TeamEditor.TeamGrid.GridType.RegularMember;
            this.gridTemporary.RowHeadersWidthChanged += new System.EventHandler(this.gridTemporary_RowHeadersWidthChanged);
            this.gridTemporary.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.gridTemporary_ColumnWidthChanged);
            // 
            // panelBand1
            // 
            this.panelBand1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panelBand1.Controls.Add(this.label1);
            this.panelBand1.Location = new System.Drawing.Point(0, 21);
            this.panelBand1.Name = "panelBand1";
            this.panelBand1.Size = new System.Drawing.Size(341, 32);
            this.panelBand1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(1, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(339, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "자위소방대";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // contextMenuRegularTeam
            // 
            this.contextMenuRegularTeam.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuAddTeam,
            this.tsMenuDeleteTeam,
            this.tsMenuRenameTeam});
            this.contextMenuRegularTeam.Name = "contextMenuStrip1";
            this.contextMenuRegularTeam.Size = new System.Drawing.Size(139, 70);
            // 
            // tsMenuAddTeam
            // 
            this.tsMenuAddTeam.Name = "tsMenuAddTeam";
            this.tsMenuAddTeam.Size = new System.Drawing.Size(138, 22);
            this.tsMenuAddTeam.Text = "추가";
            this.tsMenuAddTeam.Click += new System.EventHandler(this.tsMenuAddTeam_Click);
            // 
            // tsMenuDeleteTeam
            // 
            this.tsMenuDeleteTeam.Name = "tsMenuDeleteTeam";
            this.tsMenuDeleteTeam.Size = new System.Drawing.Size(138, 22);
            this.tsMenuDeleteTeam.Text = "삭제";
            this.tsMenuDeleteTeam.Click += new System.EventHandler(this.tsMenuDeleteTeam_Click);
            // 
            // tsMenuRenameTeam
            // 
            this.tsMenuRenameTeam.Name = "tsMenuRenameTeam";
            this.tsMenuRenameTeam.Size = new System.Drawing.Size(138, 22);
            this.tsMenuRenameTeam.Text = "이름 바꾸기";
            this.tsMenuRenameTeam.Click += new System.EventHandler(this.tsMenuRenameTeam_Click);
            // 
            // imageListDrag
            // 
            this.imageListDrag.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListDrag.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListDrag.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // contextMenuTemporaryTeam
            // 
            this.contextMenuTemporaryTeam.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuNewGroup,
            this.tsMenuAddTempTeam,
            this.tsMenuDeleteTempTeam,
            this.tsMenuRenameTempTeam});
            this.contextMenuTemporaryTeam.Name = "contextMenuTemporaryTeam";
            this.contextMenuTemporaryTeam.Size = new System.Drawing.Size(143, 92);
            // 
            // tsMenuNewGroup
            // 
            this.tsMenuNewGroup.Name = "tsMenuNewGroup";
            this.tsMenuNewGroup.Size = new System.Drawing.Size(142, 22);
            this.tsMenuNewGroup.Text = "새 조직 추가";
            this.tsMenuNewGroup.Click += new System.EventHandler(this.tsMenuNewGroup_Click);
            // 
            // tsMenuAddTempTeam
            // 
            this.tsMenuAddTempTeam.Name = "tsMenuAddTempTeam";
            this.tsMenuAddTempTeam.Size = new System.Drawing.Size(142, 22);
            this.tsMenuAddTempTeam.Text = "추가";
            this.tsMenuAddTempTeam.Click += new System.EventHandler(this.tsMenuAddTempTeam_Click);
            // 
            // tsMenuDeleteTempTeam
            // 
            this.tsMenuDeleteTempTeam.Name = "tsMenuDeleteTempTeam";
            this.tsMenuDeleteTempTeam.Size = new System.Drawing.Size(142, 22);
            this.tsMenuDeleteTempTeam.Text = "삭제";
            this.tsMenuDeleteTempTeam.Click += new System.EventHandler(this.tsMenuDeleteTempTeam_Click);
            // 
            // tsMenuRenameTempTeam
            // 
            this.tsMenuRenameTempTeam.Name = "tsMenuRenameTempTeam";
            this.tsMenuRenameTempTeam.Size = new System.Drawing.Size(142, 22);
            this.tsMenuRenameTempTeam.Text = "이름 바꾸기";
            this.tsMenuRenameTempTeam.Click += new System.EventHandler(this.tsMenuRenameTempTeam_Click);
            // 
            // colMemberType
            // 
            this.colMemberType.HeaderText = "타입";
            this.colMemberType.Name = "colMemberType";
            this.colMemberType.ReadOnly = true;
            // 
            // imageListAlwaysTreeIcon
            // 
            this.imageListAlwaysTreeIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListAlwaysTreeIcon.ImageStream")));
            this.imageListAlwaysTreeIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListAlwaysTreeIcon.Images.SetKeyName(0, "left_always_group.png");
            this.imageListAlwaysTreeIcon.Images.SetKeyName(1, "left_always_team.png");
            this.imageListAlwaysTreeIcon.Images.SetKeyName(2, "left_always_employee.png");
            // 
            // imageListEmergencyTreeIcon
            // 
            this.imageListEmergencyTreeIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListEmergencyTreeIcon.ImageStream")));
            this.imageListEmergencyTreeIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListEmergencyTreeIcon.Images.SetKeyName(0, "left_emergency.png");
            this.imageListEmergencyTreeIcon.Images.SetKeyName(1, "left_always_team.png");
            this.imageListEmergencyTreeIcon.Images.SetKeyName(2, "left_always_employee.png");
            // 
            // contextMenuExternal
            // 
            this.contextMenuExternal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuNewExternalTeam,
            this.tsMenuAddExternalCompanyTeam,
            this.tsMenuRemoveExternal,
            this.tsMenuRenameExternalCompanyTeam});
            this.contextMenuExternal.Name = "contextMenuExternal";
            this.contextMenuExternal.Size = new System.Drawing.Size(167, 92);
            // 
            // tsMenuNewExternalTeam
            // 
            this.tsMenuNewExternalTeam.Name = "tsMenuNewExternalTeam";
            this.tsMenuNewExternalTeam.Size = new System.Drawing.Size(166, 22);
            this.tsMenuNewExternalTeam.Text = "새 협력회사 추가";
            this.tsMenuNewExternalTeam.Click += new System.EventHandler(this.tsMenuNewExternalTeam_Click);
            // 
            // tsMenuAddExternalCompanyTeam
            // 
            this.tsMenuAddExternalCompanyTeam.Name = "tsMenuAddExternalCompanyTeam";
            this.tsMenuAddExternalCompanyTeam.Size = new System.Drawing.Size(166, 22);
            this.tsMenuAddExternalCompanyTeam.Text = "추가";
            this.tsMenuAddExternalCompanyTeam.Click += new System.EventHandler(this.tsMenuAddExternalCompanyTeam_Click);
            // 
            // tsMenuRemoveExternal
            // 
            this.tsMenuRemoveExternal.Name = "tsMenuRemoveExternal";
            this.tsMenuRemoveExternal.Size = new System.Drawing.Size(166, 22);
            this.tsMenuRemoveExternal.Text = "삭제";
            this.tsMenuRemoveExternal.Click += new System.EventHandler(this.tsMenuRemoveExternalCompanyTeam_Click);
            // 
            // tsMenuRenameExternalCompanyTeam
            // 
            this.tsMenuRenameExternalCompanyTeam.Name = "tsMenuRenameExternalCompanyTeam";
            this.tsMenuRenameExternalCompanyTeam.Size = new System.Drawing.Size(166, 22);
            this.tsMenuRenameExternalCompanyTeam.Text = "이름 바꾸기";
            this.tsMenuRenameExternalCompanyTeam.Click += new System.EventHandler(this.tsMenuRenameExternalCompanyTeam_Click);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.gridUserDefinedTeam);
            this.panelMain.Controls.Add(this.splitContainerMain);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 88);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1122, 475);
            this.panelMain.TabIndex = 4;
            // 
            // gridUserDefinedTeam
            // 
            this.gridUserDefinedTeam.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gridUserDefinedTeam.ColumnHeadersHeight = 32;
            this.gridUserDefinedTeam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridUserDefinedTeam.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo4,
            this.colUserDefinedTeamName,
            this.colUserDefinedTeamPhoneNumber,
            this.colUserDefinedTeamFaxNumber});
            this.gridUserDefinedTeam.CurrentTeam = null;
            this.gridUserDefinedTeam.groupPosition = null;
            this.gridUserDefinedTeam.LinkedTree = null;
            this.gridUserDefinedTeam.Location = new System.Drawing.Point(10, 113);
            this.gridUserDefinedTeam.Name = "gridUserDefinedTeam";
            this.gridUserDefinedTeam.NoSort = false;
            this.gridUserDefinedTeam.RowHeadersVisible = false;
            this.gridUserDefinedTeam.RowTemplate.Height = 23;
            this.gridUserDefinedTeam.Size = new System.Drawing.Size(244, 223);
            this.gridUserDefinedTeam.TabIndex = 3;
            this.gridUserDefinedTeam.Type = TeamEditor.TeamGrid.GridType.RegularMember;
            // 
            // colNo4
            // 
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo4.DefaultCellStyle = dataGridViewCellStyle23;
            this.colNo4.HeaderText = "번호";
            this.colNo4.Name = "colNo4";
            this.colNo4.ReadOnly = true;
            this.colNo4.Width = 40;
            // 
            // colUserDefinedTeamName
            // 
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.colUserDefinedTeamName.DefaultCellStyle = dataGridViewCellStyle24;
            this.colUserDefinedTeamName.HeaderText = "사용자정의조직 이름";
            this.colUserDefinedTeamName.Name = "colUserDefinedTeamName";
            this.colUserDefinedTeamName.Width = 200;
            // 
            // colUserDefinedTeamPhoneNumber
            // 
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colUserDefinedTeamPhoneNumber.DefaultCellStyle = dataGridViewCellStyle25;
            this.colUserDefinedTeamPhoneNumber.HeaderText = "전화번호";
            this.colUserDefinedTeamPhoneNumber.Name = "colUserDefinedTeamPhoneNumber";
            this.colUserDefinedTeamPhoneNumber.Width = 150;
            // 
            // colUserDefinedTeamFaxNumber
            // 
            dataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colUserDefinedTeamFaxNumber.DefaultCellStyle = dataGridViewCellStyle26;
            this.colUserDefinedTeamFaxNumber.HeaderText = "팩스번호";
            this.colUserDefinedTeamFaxNumber.Name = "colUserDefinedTeamFaxNumber";
            this.colUserDefinedTeamFaxNumber.Width = 150;
            // 
            // colNo
            // 
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle17;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 40;
            // 
            // colManager
            // 
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colManager.DefaultCellStyle = dataGridViewCellStyle18;
            this.colManager.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colManager.HeaderText = "정/부";
            this.colManager.Items.AddRange(new object[] {
            "정",
            "부",
            "반원"});
            this.colManager.Name = "colManager";
            // 
            // colTemporaryMemberName
            // 
            this.colTemporaryMemberName.HeaderText = "SOP 표시이름";
            this.colTemporaryMemberName.Name = "colTemporaryMemberName";
            this.colTemporaryMemberName.Width = 200;
            // 
            // colTeam
            // 
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTeam.DefaultCellStyle = dataGridViewCellStyle19;
            this.colTeam.HeaderText = "부서명";
            this.colTeam.Name = "colTeam";
            this.colTeam.ReadOnly = true;
            // 
            // colTeamButton
            // 
            this.colTeamButton.HeaderText = "";
            this.colTeamButton.Name = "colTeamButton";
            this.colTeamButton.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTeamButton.Text = "편집";
            this.colTeamButton.UseColumnTextForButtonValue = true;
            this.colTeamButton.Width = 45;
            // 
            // colPosition2
            // 
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colPosition2.DefaultCellStyle = dataGridViewCellStyle20;
            this.colPosition2.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colPosition2.HeaderText = "직위";
            this.colPosition2.Items.AddRange(new object[] {
            "팀전체",
            "책임자",
            "팀원"});
            this.colPosition2.Name = "colPosition2";
            // 
            // colManager2
            // 
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colManager2.DefaultCellStyle = dataGridViewCellStyle21;
            this.colManager2.HeaderText = "성명";
            this.colManager2.Name = "colManager2";
            this.colManager2.ReadOnly = true;
            // 
            // colManager2Button
            // 
            this.colManager2Button.HeaderText = "";
            this.colManager2Button.Name = "colManager2Button";
            this.colManager2Button.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colManager2Button.Text = "편집";
            this.colManager2Button.UseColumnTextForButtonValue = true;
            this.colManager2Button.Width = 45;
            // 
            // colNumber
            // 
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNumber.DefaultCellStyle = dataGridViewCellStyle22;
            this.colNumber.HeaderText = "인원수";
            this.colNumber.Name = "colNumber";
            // 
            // colIncludeSubTeams
            // 
            this.colIncludeSubTeams.HeaderText = "하위팀 포함";
            this.colIncludeSubTeams.Name = "colIncludeSubTeams";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1122, 563);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.paneRibbonToolBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.paneRibbonToolBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.panelExternal.ResumeLayout(false);
            this.panelExternal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridExternal)).EndInit();
            this.panelRegular.ResumeLayout(false);
            this.panelRegular.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRegularMember)).EndInit();
            this.splitContainerEmergency.Panel1.ResumeLayout(false);
            this.splitContainerEmergency.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerEmergency)).EndInit();
            this.splitContainerEmergency.ResumeLayout(false);
            this.panelTemporary.ResumeLayout(false);
            this.panelTemporary.PerformLayout();
            this.panelBand2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridTemporary)).EndInit();
            this.panelBand1.ResumeLayout(false);
            this.contextMenuRegularTeam.ResumeLayout(false);
            this.contextMenuTemporaryTeam.ResumeLayout(false);
            this.contextMenuExternal.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridUserDefinedTeam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel paneRibbonToolBar;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerEmergency;
        private System.Windows.Forms.PictureBox pictureBox1;
        private UnE.GUI.RibbonButton rbtnSave;
        private UnE.GUI.RibbonButton rbtnRegular;
        private UnE.GUI.RibbonButton rbtnEdit;
        private System.Windows.Forms.PictureBox pictureBox2;
        private UnE.GUI.RibbonButton rbtnEmergency;
        private UnE.GUI.RibbonButton rbtnNormal;
        private System.Windows.Forms.PictureBox pictureBox3;
        private TeamTreeView treeRegularTeam;
        private TeamGrid gridRegularMember;
        private TeamTreeView treeEmergency;
        private TeamTreeView treeNormal;
        private TeamGrid gridTemporary;
        private UnE.GUI.RibbonButton rbtnRedo;
        private UnE.GUI.RibbonButton rbtnUndo;
        private System.Windows.Forms.ContextMenuStrip contextMenuRegularTeam;
        private System.Windows.Forms.ToolStripMenuItem tsMenuAddTeam;
        private System.Windows.Forms.ToolStripMenuItem tsMenuDeleteTeam;
        private System.Windows.Forms.ToolStripMenuItem tsMenuRenameTeam;
        private System.Windows.Forms.ImageList imageListDrag;
        private TeamTreeView treeExternalCompanyTeam;
        private System.Windows.Forms.ContextMenuStrip contextMenuTemporaryTeam;
        private System.Windows.Forms.ToolStripMenuItem tsMenuNewGroup;
        private System.Windows.Forms.ToolStripMenuItem tsMenuAddTempTeam;
        private System.Windows.Forms.ToolStripMenuItem tsMenuDeleteTempTeam;
        private System.Windows.Forms.ToolStripMenuItem tsMenuRenameTempTeam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMemberType;
        private System.Windows.Forms.ImageList imageListAlwaysTreeIcon;
        private System.Windows.Forms.ImageList imageListEmergencyTreeIcon;
        private UnE.GUI.RibbonButton rbtnUserDefined;
        private UnE.GUI.RibbonButton rbtnExternal;
        private TeamGrid gridExternal;
        private System.Windows.Forms.ContextMenuStrip contextMenuExternal;
        private System.Windows.Forms.ToolStripMenuItem tsMenuNewExternalTeam;
        private System.Windows.Forms.ToolStripMenuItem tsMenuAddExternalCompanyTeam;
        private System.Windows.Forms.ToolStripMenuItem tsMenuRemoveExternal;
        private System.Windows.Forms.ToolStripMenuItem tsMenuRenameExternalCompanyTeam;
        private TeamGrid gridUserDefinedTeam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo3;
        private System.Windows.Forms.DataGridViewTextBoxColumn colExternalName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colExternalLevel;
        private System.Windows.Forms.DataGridViewComboBoxColumn colExternalPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn colExternalPhoneNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEtc;
        private System.Windows.Forms.Panel panelBand2;
        private System.Windows.Forms.Panel panelBand1;
        private System.Windows.Forms.Panel panelTemporary;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTeamPathForTemporary;
        private System.Windows.Forms.Panel panelRegular;
        private System.Windows.Forms.Label lblTeamPathForRegular;
        private System.Windows.Forms.Panel panelExternal;
        private System.Windows.Forms.Label lblTeamPathForExternal;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblExternalServerState;
        private System.Windows.Forms.Label lblRegularServerState;
        private System.Windows.Forms.Label lblTemporaryServerState;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colPosition;
        private System.Windows.Forms.DataGridViewComboBoxColumn colSubPosition;
        private System.Windows.Forms.DataGridViewComboBoxColumn colLevel;
        private System.Windows.Forms.DataGridViewComboBoxColumn colSubLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhoneNumber;
        private System.Windows.Forms.DataGridViewComboBoxColumn colGroupPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMemberID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOfficePhoneNumber;
        private UnE.GUI.RibbonButton rbtnImportRegular;
        private System.Windows.Forms.Panel panelMain;
        private UnE.GUI.RibbonButton rbtnOption;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo4;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUserDefinedTeamName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUserDefinedTeamPhoneNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUserDefinedTeamFaxNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewComboBoxColumn colManager;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTemporaryMemberName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeam;
        private System.Windows.Forms.DataGridViewButtonColumn colTeamButton;
        private System.Windows.Forms.DataGridViewComboBoxColumn colPosition2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colManager2;
        private System.Windows.Forms.DataGridViewButtonColumn colManager2Button;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colIncludeSubTeams;

    }
}

