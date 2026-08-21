namespace SOPMonitoringSystem
{
    partial class FormSOP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSOP));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnBulletin = new UnE.Controls.ColorLabel();
            this.btnSOPManager2 = new UnE.Controls.ColorLabel();
            this.btnTeamEditor2 = new UnE.Controls.ColorLabel();
            this.picUser = new System.Windows.Forms.PictureBox();
            this.panelSOPMode = new System.Windows.Forms.Panel();
            this.labelVirtualMode = new System.Windows.Forms.Label();
            this.rbtnCheckVirtualMode = new UnE.GUI.RibbonButton();
            this.labelRealMode = new System.Windows.Forms.Label();
            this.rbtnCheckRealMode = new UnE.GUI.RibbonButton();
            this.rbtnRealMode = new UnE.GUI.RibbonButton();
            this.pictureBoxSecond = new System.Windows.Forms.PictureBox();
            this.pictureBoxFirst = new System.Windows.Forms.PictureBox();
            this.labelUserName = new System.Windows.Forms.Label();
            this.rbtnControlAction = new UnE.GUI.RibbonButton();
            this.rbtnConfig = new UnE.GUI.RibbonButton();
            this.rbtnCancelSOP = new UnE.GUI.RibbonButton();
            this.rbtnStartSOP = new UnE.GUI.RibbonButton();
            this.rbtnControlStatus = new UnE.GUI.RibbonButton();
            this.rbtnLoadSOP = new UnE.GUI.RibbonButton();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.contextMenuStripLogout = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).BeginInit();
            this.panelSOPMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSecond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFirst)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.contextMenuStripLogout.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.panelMain.Location = new System.Drawing.Point(2, 159);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1920, 719);
            this.panelMain.TabIndex = 1;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.panelTop.Controls.Add(this.btnBulletin);
            this.panelTop.Controls.Add(this.btnSOPManager2);
            this.panelTop.Controls.Add(this.btnTeamEditor2);
            this.panelTop.Controls.Add(this.picUser);
            this.panelTop.Controls.Add(this.panelSOPMode);
            this.panelTop.Controls.Add(this.pictureBoxSecond);
            this.panelTop.Controls.Add(this.pictureBoxFirst);
            this.panelTop.Controls.Add(this.labelUserName);
            this.panelTop.Controls.Add(this.rbtnControlAction);
            this.panelTop.Controls.Add(this.rbtnConfig);
            this.panelTop.Controls.Add(this.rbtnCancelSOP);
            this.panelTop.Controls.Add(this.rbtnStartSOP);
            this.panelTop.Controls.Add(this.rbtnControlStatus);
            this.panelTop.Controls.Add(this.rbtnLoadSOP);
            this.panelTop.Controls.Add(this.pictureBoxLogo);
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1920, 70);
            this.panelTop.TabIndex = 0;
            // 
            // btnBulletin
            // 
            this.btnBulletin.AutoSize = true;
            this.btnBulletin.ColorClicked = System.Drawing.Color.White;
            this.btnBulletin.ColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.btnBulletin.ColorNomal = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnBulletin.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnBulletin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnBulletin.Location = new System.Drawing.Point(1336, 6);
            this.btnBulletin.Name = "btnBulletin";
            this.btnBulletin.Size = new System.Drawing.Size(51, 19);
            this.btnBulletin.TabIndex = 19;
            this.btnBulletin.Text = "상황판";
            this.btnBulletin.Click += new System.EventHandler(this.btnBulletin_Click);
            // 
            // btnSOPManager2
            // 
            this.btnSOPManager2.AutoSize = true;
            this.btnSOPManager2.ColorClicked = System.Drawing.Color.White;
            this.btnSOPManager2.ColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.btnSOPManager2.ColorNomal = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnSOPManager2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSOPManager2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnSOPManager2.Location = new System.Drawing.Point(1336, 44);
            this.btnSOPManager2.Name = "btnSOPManager2";
            this.btnSOPManager2.Size = new System.Drawing.Size(81, 19);
            this.btnSOPManager2.TabIndex = 18;
            this.btnSOPManager2.Text = "SOP생성기";
            this.btnSOPManager2.Click += new System.EventHandler(this.btnSOPManager_Click);
            // 
            // btnTeamEditor2
            // 
            this.btnTeamEditor2.AutoSize = true;
            this.btnTeamEditor2.ColorClicked = System.Drawing.Color.White;
            this.btnTeamEditor2.ColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.btnTeamEditor2.ColorNomal = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnTeamEditor2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnTeamEditor2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.btnTeamEditor2.Location = new System.Drawing.Point(1336, 25);
            this.btnTeamEditor2.Name = "btnTeamEditor2";
            this.btnTeamEditor2.Size = new System.Drawing.Size(79, 19);
            this.btnTeamEditor2.TabIndex = 17;
            this.btnTeamEditor2.Text = "조직관리툴";
            this.btnTeamEditor2.Click += new System.EventHandler(this.btnTeamEditor_Click);
            // 
            // picUser
            // 
            this.picUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picUser.Image = global::SOPMonitoringSystem.Properties.Resources.userLevel1;
            this.picUser.Location = new System.Drawing.Point(1677, 23);
            this.picUser.Name = "picUser";
            this.picUser.Size = new System.Drawing.Size(25, 25);
            this.picUser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picUser.TabIndex = 16;
            this.picUser.TabStop = false;
            // 
            // panelSOPMode
            // 
            this.panelSOPMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(33)))), ((int)(((byte)(60)))));
            this.panelSOPMode.Controls.Add(this.labelVirtualMode);
            this.panelSOPMode.Controls.Add(this.rbtnCheckVirtualMode);
            this.panelSOPMode.Controls.Add(this.labelRealMode);
            this.panelSOPMode.Controls.Add(this.rbtnCheckRealMode);
            this.panelSOPMode.Controls.Add(this.rbtnRealMode);
            this.panelSOPMode.Location = new System.Drawing.Point(1012, 0);
            this.panelSOPMode.Name = "panelSOPMode";
            this.panelSOPMode.Size = new System.Drawing.Size(260, 70);
            this.panelSOPMode.TabIndex = 12;
            // 
            // labelVirtualMode
            // 
            this.labelVirtualMode.AutoSize = true;
            this.labelVirtualMode.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelVirtualMode.ForeColor = System.Drawing.Color.White;
            this.labelVirtualMode.Location = new System.Drawing.Point(49, 39);
            this.labelVirtualMode.Name = "labelVirtualMode";
            this.labelVirtualMode.Size = new System.Drawing.Size(55, 15);
            this.labelVirtualMode.TabIndex = 10;
            this.labelVirtualMode.Text = "훈련모드";
            this.labelVirtualMode.Click += new System.EventHandler(this.rbtnCheckVirtualMode_Click);
            // 
            // rbtnCheckVirtualMode
            // 
            this.rbtnCheckVirtualMode.BackColor = System.Drawing.Color.Transparent;
            this.rbtnCheckVirtualMode.CheckButton = false;
            this.rbtnCheckVirtualMode.CheckedBkgndImage = null;
            this.rbtnCheckVirtualMode.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Radio_Checked;
            this.rbtnCheckVirtualMode.CheckedMouseOver = global::SOPMonitoringSystem.Properties.Resources.Radio_Checked_MouseOver;
            this.rbtnCheckVirtualMode.ClickedBackgroundImage = null;
            this.rbtnCheckVirtualMode.ClickedImage = null;
            this.rbtnCheckVirtualMode.CustomImageRect = new System.Drawing.Rectangle(0, 0, 18, 18);
            this.rbtnCheckVirtualMode.DisabledBkgndImage = null;
            this.rbtnCheckVirtualMode.DisabledImage = null;
            this.rbtnCheckVirtualMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(168)))), ((int)(((byte)(94)))));
            this.rbtnCheckVirtualMode.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnCheckVirtualMode.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnCheckVirtualMode.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnCheckVirtualMode.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnCheckVirtualMode.ForeColorsByTypeUse = true;
            this.rbtnCheckVirtualMode.ID = -1;
            this.rbtnCheckVirtualMode.InitButtonWidth = 18;
            this.rbtnCheckVirtualMode.IsChecked = true;
            this.rbtnCheckVirtualMode.Location = new System.Drawing.Point(24, 37);
            this.rbtnCheckVirtualMode.MouseOverBkgndImage = null;
            this.rbtnCheckVirtualMode.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.Radio_Unchecked_MouseOver;
            this.rbtnCheckVirtualMode.Name = "rbtnCheckVirtualMode";
            this.rbtnCheckVirtualMode.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Radio_Unchecked;
            this.rbtnCheckVirtualMode.Owner = null;
            this.rbtnCheckVirtualMode.Size = new System.Drawing.Size(18, 18);
            this.rbtnCheckVirtualMode.TabIndex = 9;
            this.rbtnCheckVirtualMode.TextLocation = new System.Drawing.Point(0, 15);
            this.rbtnCheckVirtualMode.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCheckVirtualMode.ToolTipText = "";
            this.rbtnCheckVirtualMode.UseCustomImageRect = true;
            this.rbtnCheckVirtualMode.UseTextLocation = true;
            this.rbtnCheckVirtualMode.UseVisualStyleBackColor = false;
            this.rbtnCheckVirtualMode.EnabledChanged += new System.EventHandler(this.rbtnCheckMode_EnabledChanged);
            this.rbtnCheckVirtualMode.Click += new System.EventHandler(this.rbtnCheckVirtualMode_Click);
            // 
            // labelRealMode
            // 
            this.labelRealMode.AutoSize = true;
            this.labelRealMode.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelRealMode.ForeColor = System.Drawing.Color.White;
            this.labelRealMode.Location = new System.Drawing.Point(49, 14);
            this.labelRealMode.Name = "labelRealMode";
            this.labelRealMode.Size = new System.Drawing.Size(55, 15);
            this.labelRealMode.TabIndex = 10;
            this.labelRealMode.Text = "실제모드";
            this.labelRealMode.Click += new System.EventHandler(this.rbtnCheckRealMode_Click);
            // 
            // rbtnCheckRealMode
            // 
            this.rbtnCheckRealMode.BackColor = System.Drawing.Color.Transparent;
            this.rbtnCheckRealMode.CheckButton = false;
            this.rbtnCheckRealMode.CheckedBkgndImage = null;
            this.rbtnCheckRealMode.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Radio_Checked;
            this.rbtnCheckRealMode.CheckedMouseOver = global::SOPMonitoringSystem.Properties.Resources.Radio_Checked_MouseOver;
            this.rbtnCheckRealMode.ClickedBackgroundImage = null;
            this.rbtnCheckRealMode.ClickedImage = null;
            this.rbtnCheckRealMode.CustomImageRect = new System.Drawing.Rectangle(0, 0, 18, 18);
            this.rbtnCheckRealMode.DisabledBkgndImage = null;
            this.rbtnCheckRealMode.DisabledImage = null;
            this.rbtnCheckRealMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(168)))), ((int)(((byte)(94)))));
            this.rbtnCheckRealMode.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnCheckRealMode.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnCheckRealMode.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnCheckRealMode.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnCheckRealMode.ForeColorsByTypeUse = true;
            this.rbtnCheckRealMode.ID = -1;
            this.rbtnCheckRealMode.InitButtonWidth = 18;
            this.rbtnCheckRealMode.IsChecked = false;
            this.rbtnCheckRealMode.Location = new System.Drawing.Point(24, 12);
            this.rbtnCheckRealMode.MouseOverBkgndImage = null;
            this.rbtnCheckRealMode.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.Radio_Unchecked_MouseOver;
            this.rbtnCheckRealMode.Name = "rbtnCheckRealMode";
            this.rbtnCheckRealMode.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Radio_Unchecked;
            this.rbtnCheckRealMode.Owner = null;
            this.rbtnCheckRealMode.Size = new System.Drawing.Size(18, 18);
            this.rbtnCheckRealMode.TabIndex = 9;
            this.rbtnCheckRealMode.TextLocation = new System.Drawing.Point(0, 15);
            this.rbtnCheckRealMode.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCheckRealMode.ToolTipText = "";
            this.rbtnCheckRealMode.UseCustomImageRect = true;
            this.rbtnCheckRealMode.UseTextLocation = true;
            this.rbtnCheckRealMode.UseVisualStyleBackColor = false;
            this.rbtnCheckRealMode.EnabledChanged += new System.EventHandler(this.rbtnCheckMode_EnabledChanged);
            this.rbtnCheckRealMode.Click += new System.EventHandler(this.rbtnCheckRealMode_Click);
            // 
            // rbtnRealMode
            // 
            this.rbtnRealMode.BackColor = System.Drawing.Color.Transparent;
            this.rbtnRealMode.CheckButton = false;
            this.rbtnRealMode.CheckedBkgndImage = null;
            this.rbtnRealMode.CheckedImage = null;
            this.rbtnRealMode.CheckedMouseOver = null;
            this.rbtnRealMode.ClickedBackgroundImage = null;
            this.rbtnRealMode.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Orange_Selected;
            this.rbtnRealMode.CustomImageRect = new System.Drawing.Rectangle(0, 0, 120, 50);
            this.rbtnRealMode.DisabledBkgndImage = null;
            this.rbtnRealMode.DisabledImage = null;
            this.rbtnRealMode.Enabled = false;
            this.rbtnRealMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(168)))), ((int)(((byte)(94)))));
            this.rbtnRealMode.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnRealMode.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnRealMode.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(168)))), ((int)(((byte)(94)))));
            this.rbtnRealMode.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnRealMode.ForeColorsByTypeUse = true;
            this.rbtnRealMode.ID = -1;
            this.rbtnRealMode.InitButtonWidth = 120;
            this.rbtnRealMode.IsChecked = false;
            this.rbtnRealMode.Location = new System.Drawing.Point(126, 9);
            this.rbtnRealMode.MouseOverBkgndImage = null;
            this.rbtnRealMode.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Orange_MouseOver;
            this.rbtnRealMode.Name = "rbtnRealMode";
            this.rbtnRealMode.NormalImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Orange_Normal;
            this.rbtnRealMode.Owner = null;
            this.rbtnRealMode.Size = new System.Drawing.Size(120, 50);
            this.rbtnRealMode.TabIndex = 9;
            this.rbtnRealMode.Text = "훈련모드";
            this.rbtnRealMode.TextLocation = new System.Drawing.Point(0, 15);
            this.rbtnRealMode.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnRealMode.ToolTipText = "훈련모드";
            this.rbtnRealMode.UseCustomImageRect = true;
            this.rbtnRealMode.UseTextLocation = true;
            this.rbtnRealMode.UseVisualStyleBackColor = false;
            // 
            // pictureBoxSecond
            // 
            this.pictureBoxSecond.Image = global::SOPMonitoringSystem.Properties.Resources.Separator_Small;
            this.pictureBoxSecond.Location = new System.Drawing.Point(709, 22);
            this.pictureBoxSecond.Name = "pictureBoxSecond";
            this.pictureBoxSecond.Size = new System.Drawing.Size(3, 20);
            this.pictureBoxSecond.TabIndex = 11;
            this.pictureBoxSecond.TabStop = false;
            // 
            // pictureBoxFirst
            // 
            this.pictureBoxFirst.Image = global::SOPMonitoringSystem.Properties.Resources.Separator_Small;
            this.pictureBoxFirst.Location = new System.Drawing.Point(428, 22);
            this.pictureBoxFirst.Name = "pictureBoxFirst";
            this.pictureBoxFirst.Size = new System.Drawing.Size(3, 20);
            this.pictureBoxFirst.TabIndex = 11;
            this.pictureBoxFirst.TabStop = false;
            // 
            // labelUserName
            // 
            this.labelUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUserName.AutoSize = true;
            this.labelUserName.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.labelUserName.Location = new System.Drawing.Point(1708, 24);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(83, 19);
            this.labelUserName.TabIndex = 10;
            this.labelUserName.Text = "사용자 이름";
            // 
            // rbtnControlAction
            // 
            this.rbtnControlAction.BackColor = System.Drawing.Color.Transparent;
            this.rbtnControlAction.CheckButton = false;
            this.rbtnControlAction.CheckedBkgndImage = null;
            this.rbtnControlAction.CheckedImage = null;
            this.rbtnControlAction.CheckedMouseOver = null;
            this.rbtnControlAction.ClickedBackgroundImage = null;
            this.rbtnControlAction.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Selected;
            this.rbtnControlAction.CustomImageRect = new System.Drawing.Rectangle(0, 0, 120, 50);
            this.rbtnControlAction.DisabledBkgndImage = null;
            this.rbtnControlAction.DisabledImage = null;
            this.rbtnControlAction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnControlAction.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnControlAction.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnControlAction.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnControlAction.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnControlAction.ForeColorsByTypeUse = true;
            this.rbtnControlAction.ID = -1;
            this.rbtnControlAction.InitButtonWidth = 120;
            this.rbtnControlAction.IsChecked = false;
            this.rbtnControlAction.Location = new System.Drawing.Point(574, 10);
            this.rbtnControlAction.MouseOverBkgndImage = null;
            this.rbtnControlAction.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_MouseOver;
            this.rbtnControlAction.Name = "rbtnControlAction";
            this.rbtnControlAction.NormalImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Normal;
            this.rbtnControlAction.Owner = null;
            this.rbtnControlAction.Size = new System.Drawing.Size(120, 50);
            this.rbtnControlAction.TabIndex = 9;
            this.rbtnControlAction.Text = "제어권 반납";
            this.rbtnControlAction.TextLocation = new System.Drawing.Point(0, 15);
            this.rbtnControlAction.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnControlAction.ToolTipText = "제어권 반납";
            this.rbtnControlAction.UseCustomImageRect = true;
            this.rbtnControlAction.UseTextLocation = true;
            this.rbtnControlAction.UseVisualStyleBackColor = false;
            // 
            // rbtnConfig
            // 
            this.rbtnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtnConfig.BackColor = System.Drawing.Color.Transparent;
            this.rbtnConfig.CheckButton = false;
            this.rbtnConfig.CheckedBkgndImage = null;
            this.rbtnConfig.CheckedImage = null;
            this.rbtnConfig.CheckedMouseOver = null;
            this.rbtnConfig.ClickedBackgroundImage = null;
            this.rbtnConfig.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.Config_Click;
            this.rbtnConfig.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 33);
            this.rbtnConfig.DisabledBkgndImage = null;
            this.rbtnConfig.DisabledImage = null;
            this.rbtnConfig.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnConfig.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnConfig.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnConfig.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnConfig.ForeColorsByTypeUse = false;
            this.rbtnConfig.ID = -1;
            this.rbtnConfig.InitButtonWidth = 32;
            this.rbtnConfig.IsChecked = false;
            this.rbtnConfig.Location = new System.Drawing.Point(1858, 18);
            this.rbtnConfig.MouseOverBkgndImage = null;
            this.rbtnConfig.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.Config_Click;
            this.rbtnConfig.Name = "rbtnConfig";
            this.rbtnConfig.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Config_Normal;
            this.rbtnConfig.Owner = null;
            this.rbtnConfig.Size = new System.Drawing.Size(32, 33);
            this.rbtnConfig.TabIndex = 9;
            this.rbtnConfig.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnConfig.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnConfig.ToolTipText = "";
            this.rbtnConfig.UseCustomImageRect = true;
            this.rbtnConfig.UseTextLocation = false;
            this.rbtnConfig.UseVisualStyleBackColor = false;
            this.rbtnConfig.Click += new System.EventHandler(this.rbtnConfig_Click);
            // 
            // rbtnCancelSOP
            // 
            this.rbtnCancelSOP.BackColor = System.Drawing.Color.Transparent;
            this.rbtnCancelSOP.CheckButton = false;
            this.rbtnCancelSOP.CheckedBkgndImage = null;
            this.rbtnCancelSOP.CheckedImage = null;
            this.rbtnCancelSOP.CheckedMouseOver = null;
            this.rbtnCancelSOP.ClickedBackgroundImage = null;
            this.rbtnCancelSOP.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Selected;
            this.rbtnCancelSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 120, 50);
            this.rbtnCancelSOP.DisabledBkgndImage = null;
            this.rbtnCancelSOP.DisabledImage = null;
            this.rbtnCancelSOP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnCancelSOP.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnCancelSOP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnCancelSOP.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnCancelSOP.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnCancelSOP.ForeColorsByTypeUse = true;
            this.rbtnCancelSOP.ID = -1;
            this.rbtnCancelSOP.InitButtonWidth = 120;
            this.rbtnCancelSOP.IsChecked = false;
            this.rbtnCancelSOP.Location = new System.Drawing.Point(856, 10);
            this.rbtnCancelSOP.MouseOverBkgndImage = null;
            this.rbtnCancelSOP.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_MouseOver;
            this.rbtnCancelSOP.Name = "rbtnCancelSOP";
            this.rbtnCancelSOP.NormalImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Normal;
            this.rbtnCancelSOP.Owner = null;
            this.rbtnCancelSOP.Size = new System.Drawing.Size(120, 50);
            this.rbtnCancelSOP.TabIndex = 9;
            this.rbtnCancelSOP.Text = "취소";
            this.rbtnCancelSOP.TextLocation = new System.Drawing.Point(0, 15);
            this.rbtnCancelSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCancelSOP.ToolTipText = "취소";
            this.rbtnCancelSOP.UseCustomImageRect = true;
            this.rbtnCancelSOP.UseTextLocation = true;
            this.rbtnCancelSOP.UseVisualStyleBackColor = false;
            // 
            // rbtnStartSOP
            // 
            this.rbtnStartSOP.BackColor = System.Drawing.Color.Transparent;
            this.rbtnStartSOP.CheckButton = false;
            this.rbtnStartSOP.CheckedBkgndImage = null;
            this.rbtnStartSOP.CheckedImage = null;
            this.rbtnStartSOP.CheckedMouseOver = null;
            this.rbtnStartSOP.ClickedBackgroundImage = null;
            this.rbtnStartSOP.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Selected;
            this.rbtnStartSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 120, 50);
            this.rbtnStartSOP.DisabledBkgndImage = null;
            this.rbtnStartSOP.DisabledImage = null;
            this.rbtnStartSOP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnStartSOP.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnStartSOP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnStartSOP.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnStartSOP.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnStartSOP.ForeColorsByTypeUse = true;
            this.rbtnStartSOP.ID = -1;
            this.rbtnStartSOP.InitButtonWidth = 120;
            this.rbtnStartSOP.IsChecked = false;
            this.rbtnStartSOP.Location = new System.Drawing.Point(730, 10);
            this.rbtnStartSOP.MouseOverBkgndImage = null;
            this.rbtnStartSOP.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_MouseOver;
            this.rbtnStartSOP.Name = "rbtnStartSOP";
            this.rbtnStartSOP.NormalImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Normal;
            this.rbtnStartSOP.Owner = null;
            this.rbtnStartSOP.Size = new System.Drawing.Size(120, 50);
            this.rbtnStartSOP.TabIndex = 9;
            this.rbtnStartSOP.Text = "실행";
            this.rbtnStartSOP.TextLocation = new System.Drawing.Point(0, 15);
            this.rbtnStartSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnStartSOP.ToolTipText = "실행";
            this.rbtnStartSOP.UseCustomImageRect = true;
            this.rbtnStartSOP.UseTextLocation = true;
            this.rbtnStartSOP.UseVisualStyleBackColor = false;
            // 
            // rbtnControlStatus
            // 
            this.rbtnControlStatus.BackColor = System.Drawing.Color.Transparent;
            this.rbtnControlStatus.CheckButton = false;
            this.rbtnControlStatus.CheckedBkgndImage = null;
            this.rbtnControlStatus.CheckedImage = null;
            this.rbtnControlStatus.CheckedMouseOver = null;
            this.rbtnControlStatus.ClickedBackgroundImage = null;
            this.rbtnControlStatus.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Selected;
            this.rbtnControlStatus.CustomImageRect = new System.Drawing.Rectangle(0, 0, 120, 50);
            this.rbtnControlStatus.DisabledBkgndImage = null;
            this.rbtnControlStatus.DisabledImage = null;
            this.rbtnControlStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnControlStatus.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnControlStatus.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnControlStatus.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnControlStatus.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnControlStatus.ForeColorsByTypeUse = true;
            this.rbtnControlStatus.ID = -1;
            this.rbtnControlStatus.InitButtonWidth = 120;
            this.rbtnControlStatus.IsChecked = false;
            this.rbtnControlStatus.Location = new System.Drawing.Point(448, 10);
            this.rbtnControlStatus.MouseOverBkgndImage = null;
            this.rbtnControlStatus.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_MouseOver;
            this.rbtnControlStatus.Name = "rbtnControlStatus";
            this.rbtnControlStatus.NormalImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Normal;
            this.rbtnControlStatus.Owner = null;
            this.rbtnControlStatus.Size = new System.Drawing.Size(120, 50);
            this.rbtnControlStatus.TabIndex = 9;
            this.rbtnControlStatus.Text = "제어";
            this.rbtnControlStatus.TextLocation = new System.Drawing.Point(0, 15);
            this.rbtnControlStatus.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnControlStatus.ToolTipText = "제어";
            this.rbtnControlStatus.UseCustomImageRect = true;
            this.rbtnControlStatus.UseTextLocation = true;
            this.rbtnControlStatus.UseVisualStyleBackColor = false;
            // 
            // rbtnLoadSOP
            // 
            this.rbtnLoadSOP.BackColor = System.Drawing.Color.Transparent;
            this.rbtnLoadSOP.CheckButton = false;
            this.rbtnLoadSOP.CheckedBkgndImage = null;
            this.rbtnLoadSOP.CheckedImage = null;
            this.rbtnLoadSOP.CheckedMouseOver = null;
            this.rbtnLoadSOP.ClickedBackgroundImage = null;
            this.rbtnLoadSOP.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Selected;
            this.rbtnLoadSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 120, 50);
            this.rbtnLoadSOP.DisabledBkgndImage = null;
            this.rbtnLoadSOP.DisabledImage = null;
            this.rbtnLoadSOP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnLoadSOP.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnLoadSOP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnLoadSOP.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnLoadSOP.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnLoadSOP.ForeColorsByTypeUse = true;
            this.rbtnLoadSOP.ID = -1;
            this.rbtnLoadSOP.InitButtonWidth = 120;
            this.rbtnLoadSOP.IsChecked = false;
            this.rbtnLoadSOP.Location = new System.Drawing.Point(292, 10);
            this.rbtnLoadSOP.MouseOverBkgndImage = null;
            this.rbtnLoadSOP.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_MouseOver;
            this.rbtnLoadSOP.Name = "rbtnLoadSOP";
            this.rbtnLoadSOP.NormalImage = global::SOPMonitoringSystem.Properties.Resources.MenuButton_Normal;
            this.rbtnLoadSOP.Owner = null;
            this.rbtnLoadSOP.Size = new System.Drawing.Size(120, 50);
            this.rbtnLoadSOP.TabIndex = 9;
            this.rbtnLoadSOP.Text = "SOP 불러오기";
            this.rbtnLoadSOP.TextLocation = new System.Drawing.Point(0, 15);
            this.rbtnLoadSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnLoadSOP.ToolTipText = "SOP 불러오기";
            this.rbtnLoadSOP.UseCustomImageRect = true;
            this.rbtnLoadSOP.UseTextLocation = true;
            this.rbtnLoadSOP.UseVisualStyleBackColor = false;
            this.rbtnLoadSOP.Click += new System.EventHandler(this.rbtnLoadSOP_Click);
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLogo.Location = new System.Drawing.Point(23, 18);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(69, 29);
            this.pictureBoxLogo.TabIndex = 8;
            this.pictureBoxLogo.TabStop = false;
            // 
            // contextMenuStripLogout
            // 
            this.contextMenuStripLogout.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuLogout});
            this.contextMenuStripLogout.Name = "contextMenuStripLogout";
            this.contextMenuStripLogout.Size = new System.Drawing.Size(181, 48);
            // 
            // tsMenuLogout
            // 
            this.tsMenuLogout.Name = "tsMenuLogout";
            this.tsMenuLogout.Size = new System.Drawing.Size(180, 22);
            this.tsMenuLogout.Text = "로그아웃";
            this.tsMenuLogout.Click += new System.EventHandler(this.tsMenuLogout_Click);
            // 
            // FormSOP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 719);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSOP";
            this.Text = "SOP System";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).EndInit();
            this.panelSOPMode.ResumeLayout(false);
            this.panelSOPMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSecond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFirst)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.contextMenuStripLogout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private UnE.GUI.RibbonButton rbtnLoadSOP;
        private UnE.GUI.RibbonButton rbtnControlAction;
        private UnE.GUI.RibbonButton rbtnRealMode;
        private UnE.GUI.RibbonButton rbtnCancelSOP;
        private UnE.GUI.RibbonButton rbtnStartSOP;
        private UnE.GUI.RibbonButton rbtnControlStatus;
        private System.Windows.Forms.Label labelUserName;
        private UnE.GUI.RibbonButton rbtnConfig;
        private System.Windows.Forms.PictureBox pictureBoxSecond;
        private System.Windows.Forms.PictureBox pictureBoxFirst;
        private System.Windows.Forms.Panel panelSOPMode;
        private System.Windows.Forms.Label labelVirtualMode;
        private UnE.GUI.RibbonButton rbtnCheckVirtualMode;
        private System.Windows.Forms.Label labelRealMode;
        private UnE.GUI.RibbonButton rbtnCheckRealMode;
        private System.Windows.Forms.PictureBox picUser;
        private UnE.Controls.ColorLabel btnTeamEditor2;
        private UnE.Controls.ColorLabel btnSOPManager2;
        private UnE.Controls.ColorLabel btnBulletin;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripLogout;
        private System.Windows.Forms.ToolStripMenuItem tsMenuLogout;
    }
}