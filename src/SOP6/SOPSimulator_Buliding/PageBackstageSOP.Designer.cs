namespace SOPMonitoringSystem
{
    partial class PageBackstageSOP
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
            this.timerBackgroundImage = new System.Windows.Forms.Timer(this.components);
            this.timerSelectMission = new System.Windows.Forms.Timer(this.components);
            this.panelBackImage = new SOPMonitoringSystem.PanelSOP();
            this.panelScenarioTab = new System.Windows.Forms.Panel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.rbtnSMS = new UnE.GUI.RibbonButton();
            this.labelActionStep1stName = new System.Windows.Forms.Label();
            this.labelActionStep2ndName = new System.Windows.Forms.Label();
            this.labelActionStep3rdName = new System.Windows.Forms.Label();
            this.labelActionStep4thName = new System.Windows.Forms.Label();
            this.rbtnActionStep3rd = new UnE.GUI.RibbonButton();
            this.rbtnActionStep2nd = new UnE.GUI.RibbonButton();
            this.rbtnActionStep1st = new UnE.GUI.RibbonButton();
            this.rbtnBroadcast = new UnE.GUI.RibbonButton();
            this.rbtnActionStep4th = new UnE.GUI.RibbonButton();
            this.pictureBoxVerticalLine = new System.Windows.Forms.PictureBox();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.panelTabPage = new System.Windows.Forms.Panel();
            this.tabControl = new UnE.SOP.Sections.SectionTabControl();
            this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
            this.tabLogs = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panelComponentContentsTitle = new System.Windows.Forms.Panel();
            this.labelComponentContentsTitle = new System.Windows.Forms.Label();
            this.panelMark = new System.Windows.Forms.Panel();
            this.btnEditExternalMembers = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelBackImage.SuspendLayout();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVerticalLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panelTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).BeginInit();
            this.splitContainerVertical.Panel2.SuspendLayout();
            this.splitContainerVertical.SuspendLayout();
            this.tabLogs.SuspendLayout();
            this.panelComponentContentsTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerBackgroundImage
            // 
            this.timerBackgroundImage.Interval = 100;
            this.timerBackgroundImage.Tick += new System.EventHandler(this.timerBackGroundImage_Tick);
            // 
            // timerSelectMission
            // 
            this.timerSelectMission.Interval = 500;
            this.timerSelectMission.Tick += new System.EventHandler(this.timerSelectMission_Tick);
            // 
            // panelBackImage
            // 
            this.panelBackImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.panelBackImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelBackImage.backImgHeight = 0;
            this.panelBackImage.backImgWidth = 0;
            this.panelBackImage.Controls.Add(this.panelScenarioTab);
            this.panelBackImage.Controls.Add(this.panelLeft);
            this.panelBackImage.Controls.Add(this.splitContainerMain);
            this.panelBackImage.Location = new System.Drawing.Point(0, 0);
            this.panelBackImage.Name = "panelBackImage";
            this.panelBackImage.Size = new System.Drawing.Size(1093, 751);
            this.panelBackImage.TabIndex = 1;
            this.panelBackImage.UseColorFilter = false;
            // 
            // panelScenarioTab
            // 
            this.panelScenarioTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(198)))), ((int)(((byte)(215)))));
            this.panelScenarioTab.Location = new System.Drawing.Point(260, 0);
            this.panelScenarioTab.Name = "panelScenarioTab";
            this.panelScenarioTab.Size = new System.Drawing.Size(833, 70);
            this.panelScenarioTab.TabIndex = 2;
            this.panelScenarioTab.Visible = false;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(72)))), ((int)(((byte)(108)))));
            this.panelLeft.Controls.Add(this.rbtnSMS);
            this.panelLeft.Controls.Add(this.labelActionStep1stName);
            this.panelLeft.Controls.Add(this.labelActionStep2ndName);
            this.panelLeft.Controls.Add(this.labelActionStep3rdName);
            this.panelLeft.Controls.Add(this.labelActionStep4thName);
            this.panelLeft.Controls.Add(this.rbtnActionStep3rd);
            this.panelLeft.Controls.Add(this.rbtnActionStep2nd);
            this.panelLeft.Controls.Add(this.rbtnActionStep1st);
            this.panelLeft.Controls.Add(this.rbtnBroadcast);
            this.panelLeft.Controls.Add(this.rbtnActionStep4th);
            this.panelLeft.Controls.Add(this.pictureBoxVerticalLine);
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(260, 742);
            this.panelLeft.TabIndex = 1;
            this.panelLeft.Visible = false;
            // 
            // rbtnSMS
            // 
            this.rbtnSMS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbtnSMS.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSMS.CheckButton = false;
            this.rbtnSMS.CheckedBkgndImage = null;
            this.rbtnSMS.CheckedImage = null;
            this.rbtnSMS.CheckedMouseOver = null;
            this.rbtnSMS.ClickedBackgroundImage = null;
            this.rbtnSMS.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.Button_MouseOver1;
            this.rbtnSMS.CustomImageRect = new System.Drawing.Rectangle(0, 0, 200, 52);
            this.rbtnSMS.DisabledBkgndImage = null;
            this.rbtnSMS.DisabledImage = null;
            this.rbtnSMS.ForeColor = System.Drawing.Color.White;
            this.rbtnSMS.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnSMS.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnSMS.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnSMS.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnSMS.ForeColorsByTypeUse = false;
            this.rbtnSMS.ID = -1;
            this.rbtnSMS.InitButtonWidth = 200;
            this.rbtnSMS.IsChecked = false;
            this.rbtnSMS.Location = new System.Drawing.Point(30, 619);
            this.rbtnSMS.MouseOverBkgndImage = null;
            this.rbtnSMS.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.Button_MouseOver1;
            this.rbtnSMS.Name = "rbtnSMS";
            this.rbtnSMS.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Button_Normal;
            this.rbtnSMS.Owner = null;
            this.rbtnSMS.Size = new System.Drawing.Size(233, 52);
            this.rbtnSMS.TabIndex = 13;
            this.rbtnSMS.Text = "SMS 전송 테스트";
            this.rbtnSMS.TextLocation = new System.Drawing.Point(0, 14);
            this.rbtnSMS.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSMS.ToolTipText = "SMS 전송 테스트";
            this.rbtnSMS.UseCustomImageRect = true;
            this.rbtnSMS.UseTextLocation = true;
            this.rbtnSMS.UseVisualStyleBackColor = false;
            this.rbtnSMS.Visible = false;
            this.rbtnSMS.Click += new System.EventHandler(this.rbtnSMS_Click);
            // 
            // labelActionStep1stName
            // 
            this.labelActionStep1stName.AutoSize = true;
            this.labelActionStep1stName.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelActionStep1stName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.labelActionStep1stName.Location = new System.Drawing.Point(143, 101);
            this.labelActionStep1stName.Name = "labelActionStep1stName";
            this.labelActionStep1stName.Size = new System.Drawing.Size(57, 33);
            this.labelActionStep1stName.TabIndex = 12;
            this.labelActionStep1stName.Text = "관심";
            // 
            // labelActionStep2ndName
            // 
            this.labelActionStep2ndName.AutoSize = true;
            this.labelActionStep2ndName.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelActionStep2ndName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.labelActionStep2ndName.Location = new System.Drawing.Point(143, 226);
            this.labelActionStep2ndName.Name = "labelActionStep2ndName";
            this.labelActionStep2ndName.Size = new System.Drawing.Size(57, 33);
            this.labelActionStep2ndName.TabIndex = 12;
            this.labelActionStep2ndName.Text = "주의";
            // 
            // labelActionStep3rdName
            // 
            this.labelActionStep3rdName.AutoSize = true;
            this.labelActionStep3rdName.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelActionStep3rdName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.labelActionStep3rdName.Location = new System.Drawing.Point(143, 351);
            this.labelActionStep3rdName.Name = "labelActionStep3rdName";
            this.labelActionStep3rdName.Size = new System.Drawing.Size(57, 33);
            this.labelActionStep3rdName.TabIndex = 12;
            this.labelActionStep3rdName.Text = "경계";
            // 
            // labelActionStep4thName
            // 
            this.labelActionStep4thName.AutoSize = true;
            this.labelActionStep4thName.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelActionStep4thName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(154)))), ((int)(((byte)(214)))));
            this.labelActionStep4thName.Location = new System.Drawing.Point(143, 477);
            this.labelActionStep4thName.Name = "labelActionStep4thName";
            this.labelActionStep4thName.Size = new System.Drawing.Size(57, 33);
            this.labelActionStep4thName.TabIndex = 12;
            this.labelActionStep4thName.Text = "심각";
            // 
            // rbtnActionStep3rd
            // 
            this.rbtnActionStep3rd.BackColor = System.Drawing.Color.Transparent;
            this.rbtnActionStep3rd.CheckButton = false;
            this.rbtnActionStep3rd.CheckedBkgndImage = null;
            this.rbtnActionStep3rd.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_3rd_Selected;
            this.rbtnActionStep3rd.CheckedMouseOver = null;
            this.rbtnActionStep3rd.ClickedBackgroundImage = null;
            this.rbtnActionStep3rd.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_3rd_Selected;
            this.rbtnActionStep3rd.CustomImageRect = new System.Drawing.Rectangle(0, 0, 60, 60);
            this.rbtnActionStep3rd.DisabledBkgndImage = null;
            this.rbtnActionStep3rd.DisabledImage = null;
            this.rbtnActionStep3rd.Enabled = false;
            this.rbtnActionStep3rd.ForeColor = System.Drawing.Color.White;
            this.rbtnActionStep3rd.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnActionStep3rd.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnActionStep3rd.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnActionStep3rd.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnActionStep3rd.ForeColorsByTypeUse = false;
            this.rbtnActionStep3rd.ID = -1;
            this.rbtnActionStep3rd.InitButtonWidth = 60;
            this.rbtnActionStep3rd.IsChecked = false;
            this.rbtnActionStep3rd.Location = new System.Drawing.Point(51, 336);
            this.rbtnActionStep3rd.MouseOverBkgndImage = null;
            this.rbtnActionStep3rd.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_3rd_MouseOver;
            this.rbtnActionStep3rd.Name = "rbtnActionStep3rd";
            this.rbtnActionStep3rd.NormalImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_Normal;
            this.rbtnActionStep3rd.Owner = null;
            this.rbtnActionStep3rd.Size = new System.Drawing.Size(70, 60);
            this.rbtnActionStep3rd.TabIndex = 1;
            this.rbtnActionStep3rd.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnActionStep3rd.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnActionStep3rd.ToolTipText = "";
            this.rbtnActionStep3rd.UseCustomImageRect = true;
            this.rbtnActionStep3rd.UseTextLocation = false;
            this.rbtnActionStep3rd.UseVisualStyleBackColor = false;
            this.rbtnActionStep3rd.Click += new System.EventHandler(this.rbtnActionStep_Click);
            // 
            // rbtnActionStep2nd
            // 
            this.rbtnActionStep2nd.BackColor = System.Drawing.Color.Transparent;
            this.rbtnActionStep2nd.CheckButton = false;
            this.rbtnActionStep2nd.CheckedBkgndImage = null;
            this.rbtnActionStep2nd.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_2nd_Selected;
            this.rbtnActionStep2nd.CheckedMouseOver = null;
            this.rbtnActionStep2nd.ClickedBackgroundImage = null;
            this.rbtnActionStep2nd.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_2nd_Selected;
            this.rbtnActionStep2nd.CustomImageRect = new System.Drawing.Rectangle(0, 0, 60, 60);
            this.rbtnActionStep2nd.DisabledBkgndImage = null;
            this.rbtnActionStep2nd.DisabledImage = null;
            this.rbtnActionStep2nd.Enabled = false;
            this.rbtnActionStep2nd.ForeColor = System.Drawing.Color.White;
            this.rbtnActionStep2nd.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnActionStep2nd.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnActionStep2nd.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnActionStep2nd.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnActionStep2nd.ForeColorsByTypeUse = false;
            this.rbtnActionStep2nd.ID = -1;
            this.rbtnActionStep2nd.InitButtonWidth = 60;
            this.rbtnActionStep2nd.IsChecked = false;
            this.rbtnActionStep2nd.Location = new System.Drawing.Point(51, 211);
            this.rbtnActionStep2nd.MouseOverBkgndImage = null;
            this.rbtnActionStep2nd.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_2nd_MouseOver;
            this.rbtnActionStep2nd.Name = "rbtnActionStep2nd";
            this.rbtnActionStep2nd.NormalImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_Normal;
            this.rbtnActionStep2nd.Owner = null;
            this.rbtnActionStep2nd.Size = new System.Drawing.Size(70, 60);
            this.rbtnActionStep2nd.TabIndex = 2;
            this.rbtnActionStep2nd.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnActionStep2nd.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnActionStep2nd.ToolTipText = "";
            this.rbtnActionStep2nd.UseCustomImageRect = true;
            this.rbtnActionStep2nd.UseTextLocation = false;
            this.rbtnActionStep2nd.UseVisualStyleBackColor = false;
            this.rbtnActionStep2nd.Click += new System.EventHandler(this.rbtnActionStep_Click);
            // 
            // rbtnActionStep1st
            // 
            this.rbtnActionStep1st.BackColor = System.Drawing.Color.Transparent;
            this.rbtnActionStep1st.CheckButton = false;
            this.rbtnActionStep1st.CheckedBkgndImage = null;
            this.rbtnActionStep1st.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_1st_Selected;
            this.rbtnActionStep1st.CheckedMouseOver = null;
            this.rbtnActionStep1st.ClickedBackgroundImage = null;
            this.rbtnActionStep1st.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_1st_Selected;
            this.rbtnActionStep1st.CustomImageRect = new System.Drawing.Rectangle(0, 0, 60, 60);
            this.rbtnActionStep1st.DisabledBkgndImage = null;
            this.rbtnActionStep1st.DisabledImage = null;
            this.rbtnActionStep1st.Enabled = false;
            this.rbtnActionStep1st.ForeColor = System.Drawing.Color.White;
            this.rbtnActionStep1st.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnActionStep1st.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnActionStep1st.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnActionStep1st.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnActionStep1st.ForeColorsByTypeUse = false;
            this.rbtnActionStep1st.ID = -1;
            this.rbtnActionStep1st.InitButtonWidth = 60;
            this.rbtnActionStep1st.IsChecked = false;
            this.rbtnActionStep1st.Location = new System.Drawing.Point(51, 86);
            this.rbtnActionStep1st.MouseOverBkgndImage = null;
            this.rbtnActionStep1st.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_1st_MouseOver;
            this.rbtnActionStep1st.Name = "rbtnActionStep1st";
            this.rbtnActionStep1st.NormalImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_Normal;
            this.rbtnActionStep1st.Owner = null;
            this.rbtnActionStep1st.Size = new System.Drawing.Size(70, 60);
            this.rbtnActionStep1st.TabIndex = 3;
            this.rbtnActionStep1st.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnActionStep1st.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnActionStep1st.ToolTipText = "";
            this.rbtnActionStep1st.UseCustomImageRect = true;
            this.rbtnActionStep1st.UseTextLocation = false;
            this.rbtnActionStep1st.UseVisualStyleBackColor = false;
            this.rbtnActionStep1st.Click += new System.EventHandler(this.rbtnActionStep_Click);
            // 
            // rbtnBroadcast
            // 
            this.rbtnBroadcast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbtnBroadcast.BackColor = System.Drawing.Color.Transparent;
            this.rbtnBroadcast.CheckButton = false;
            this.rbtnBroadcast.CheckedBkgndImage = null;
            this.rbtnBroadcast.CheckedImage = null;
            this.rbtnBroadcast.CheckedMouseOver = null;
            this.rbtnBroadcast.ClickedBackgroundImage = null;
            this.rbtnBroadcast.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.Button_MouseOver1;
            this.rbtnBroadcast.CustomImageRect = new System.Drawing.Rectangle(0, 0, 200, 52);
            this.rbtnBroadcast.DisabledBkgndImage = null;
            this.rbtnBroadcast.DisabledImage = null;
            this.rbtnBroadcast.ForeColor = System.Drawing.Color.White;
            this.rbtnBroadcast.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnBroadcast.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnBroadcast.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnBroadcast.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnBroadcast.ForeColorsByTypeUse = false;
            this.rbtnBroadcast.ID = -1;
            this.rbtnBroadcast.InitButtonWidth = 200;
            this.rbtnBroadcast.IsChecked = false;
            this.rbtnBroadcast.Location = new System.Drawing.Point(30, 553);
            this.rbtnBroadcast.MouseOverBkgndImage = null;
            this.rbtnBroadcast.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.Button_MouseOver1;
            this.rbtnBroadcast.Name = "rbtnBroadcast";
            this.rbtnBroadcast.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Button_Normal;
            this.rbtnBroadcast.Owner = null;
            this.rbtnBroadcast.Size = new System.Drawing.Size(233, 52);
            this.rbtnBroadcast.TabIndex = 0;
            this.rbtnBroadcast.Text = "방송 테스트";
            this.rbtnBroadcast.TextLocation = new System.Drawing.Point(0, 14);
            this.rbtnBroadcast.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnBroadcast.ToolTipText = "방송 테스트";
            this.rbtnBroadcast.UseCustomImageRect = true;
            this.rbtnBroadcast.UseTextLocation = true;
            this.rbtnBroadcast.UseVisualStyleBackColor = false;
            this.rbtnBroadcast.Visible = false;
            this.rbtnBroadcast.Click += new System.EventHandler(this.rbtnBroadcast_Click);
            // 
            // rbtnActionStep4th
            // 
            this.rbtnActionStep4th.BackColor = System.Drawing.Color.Transparent;
            this.rbtnActionStep4th.CheckButton = false;
            this.rbtnActionStep4th.CheckedBkgndImage = null;
            this.rbtnActionStep4th.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_4th_Selected;
            this.rbtnActionStep4th.CheckedMouseOver = null;
            this.rbtnActionStep4th.ClickedBackgroundImage = null;
            this.rbtnActionStep4th.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_4th_Selected;
            this.rbtnActionStep4th.CustomImageRect = new System.Drawing.Rectangle(0, 0, 60, 60);
            this.rbtnActionStep4th.DisabledBkgndImage = null;
            this.rbtnActionStep4th.DisabledImage = null;
            this.rbtnActionStep4th.Enabled = false;
            this.rbtnActionStep4th.ForeColor = System.Drawing.Color.White;
            this.rbtnActionStep4th.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnActionStep4th.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnActionStep4th.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnActionStep4th.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnActionStep4th.ForeColorsByTypeUse = false;
            this.rbtnActionStep4th.ID = -1;
            this.rbtnActionStep4th.InitButtonWidth = 60;
            this.rbtnActionStep4th.IsChecked = false;
            this.rbtnActionStep4th.Location = new System.Drawing.Point(51, 462);
            this.rbtnActionStep4th.MouseOverBkgndImage = null;
            this.rbtnActionStep4th.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_4th_MouseOver;
            this.rbtnActionStep4th.Name = "rbtnActionStep4th";
            this.rbtnActionStep4th.NormalImage = global::SOPMonitoringSystem.Properties.Resources.ActionStep_Normal;
            this.rbtnActionStep4th.Owner = null;
            this.rbtnActionStep4th.Size = new System.Drawing.Size(70, 60);
            this.rbtnActionStep4th.TabIndex = 0;
            this.rbtnActionStep4th.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnActionStep4th.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnActionStep4th.ToolTipText = "";
            this.rbtnActionStep4th.UseCustomImageRect = true;
            this.rbtnActionStep4th.UseTextLocation = false;
            this.rbtnActionStep4th.UseVisualStyleBackColor = false;
            this.rbtnActionStep4th.Click += new System.EventHandler(this.rbtnActionStep_Click);
            // 
            // pictureBoxVerticalLine
            // 
            this.pictureBoxVerticalLine.Image = global::SOPMonitoringSystem.Properties.Resources.ActionStep_VerticalLine;
            this.pictureBoxVerticalLine.Location = new System.Drawing.Point(76, 116);
            this.pictureBoxVerticalLine.Name = "pictureBoxVerticalLine";
            this.pictureBoxVerticalLine.Size = new System.Drawing.Size(10, 378);
            this.pictureBoxVerticalLine.TabIndex = 11;
            this.pictureBoxVerticalLine.TabStop = false;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.Location = new System.Drawing.Point(260, 70);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.panelTabPage);
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerVertical);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.AutoScroll = true;
            this.splitContainerMain.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.splitContainerMain.Panel2.Controls.Add(this.panelComponentContentsTitle);
            this.splitContainerMain.Panel2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnComponentContentsDoubleClick);
            this.splitContainerMain.Panel2.Resize += new System.EventHandler(this.splitContainerMain_Panel2_Resize);
            this.splitContainerMain.Panel2MinSize = 0;
            this.splitContainerMain.Size = new System.Drawing.Size(833, 751);
            this.splitContainerMain.SplitterDistance = 633;
            this.splitContainerMain.SplitterWidth = 3;
            this.splitContainerMain.TabIndex = 0;
            // 
            // panelTabPage
            // 
            this.panelTabPage.Controls.Add(this.tabControl);
            this.panelTabPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTabPage.Location = new System.Drawing.Point(0, 0);
            this.panelTabPage.Name = "panelTabPage";
            this.panelTabPage.Size = new System.Drawing.Size(633, 751);
            this.panelTabPage.TabIndex = 0;
            this.panelTabPage.Visible = false;
            // 
            // tabControl
            // 
            this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl.CloseBtnImage = null;
            this.tabControl.Font = new System.Drawing.Font("맑은 고딕", 15.75F, System.Drawing.FontStyle.Bold);
            this.tabControl.ItemSize = new System.Drawing.Size(72, 35);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.SelectedTabColor = System.Drawing.Color.DarkGray;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(635, 751);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabBackColor = System.Drawing.Color.White;
            this.tabControl.TabDisabledForeColor = System.Drawing.Color.DarkGray;
            this.tabControl.TabForeColor = System.Drawing.Color.White;
            this.tabControl.TabIndex = 1;
            this.tabControl.UseCloseButton = false;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // splitContainerVertical
            // 
            this.splitContainerVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerVertical.Location = new System.Drawing.Point(0, 0);
            this.splitContainerVertical.Name = "splitContainerVertical";
            this.splitContainerVertical.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerVertical.Panel1
            // 
            this.splitContainerVertical.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            // 
            // splitContainerVertical.Panel2
            // 
            this.splitContainerVertical.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.splitContainerVertical.Panel2.Controls.Add(this.tabLogs);
            this.splitContainerVertical.Size = new System.Drawing.Size(633, 751);
            this.splitContainerVertical.SplitterDistance = 581;
            this.splitContainerVertical.TabIndex = 1;
            // 
            // tabLogs
            // 
            this.tabLogs.Controls.Add(this.tabPage2);
            this.tabLogs.Controls.Add(this.tabPage3);
            this.tabLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabLogs.Location = new System.Drawing.Point(0, 0);
            this.tabLogs.Margin = new System.Windows.Forms.Padding(0);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.Padding = new System.Drawing.Point(20, 3);
            this.tabLogs.SelectedIndex = 0;
            this.tabLogs.Size = new System.Drawing.Size(633, 166);
            this.tabLogs.TabIndex = 0;
            this.tabLogs.Visible = false;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(625, 140);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "SOP 로그";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(625, 140);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "메시지 로그";
            this.tabPage3.Click += new System.EventHandler(this.tabPage3_Click);
            // 
            // panelComponentContentsTitle
            // 
            this.panelComponentContentsTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.panelComponentContentsTitle.Controls.Add(this.labelComponentContentsTitle);
            this.panelComponentContentsTitle.Controls.Add(this.panelMark);
            this.panelComponentContentsTitle.Controls.Add(this.btnEditExternalMembers);
            this.panelComponentContentsTitle.Controls.Add(this.labelTitle);
            this.panelComponentContentsTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelComponentContentsTitle.Location = new System.Drawing.Point(0, 0);
            this.panelComponentContentsTitle.Name = "panelComponentContentsTitle";
            this.panelComponentContentsTitle.Size = new System.Drawing.Size(197, 80);
            this.panelComponentContentsTitle.TabIndex = 0;
            // 
            // labelComponentContentsTitle
            // 
            this.labelComponentContentsTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.labelComponentContentsTitle.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelComponentContentsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelComponentContentsTitle.Location = new System.Drawing.Point(30, 23);
            this.labelComponentContentsTitle.Name = "labelComponentContentsTitle";
            this.labelComponentContentsTitle.Size = new System.Drawing.Size(178, 33);
            this.labelComponentContentsTitle.TabIndex = 0;
            this.labelComponentContentsTitle.Text = "임무목록 실행";
            this.labelComponentContentsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelComponentContentsTitle.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnComponentContentsDoubleClick);
            // 
            // panelMark
            // 
            this.panelMark.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.panelMark.Location = new System.Drawing.Point(16, 37);
            this.panelMark.Name = "panelMark";
            this.panelMark.Size = new System.Drawing.Size(7, 7);
            this.panelMark.TabIndex = 3;
            // 
            // btnEditExternalMembers
            // 
            this.btnEditExternalMembers.Location = new System.Drawing.Point(68, 6);
            this.btnEditExternalMembers.Name = "btnEditExternalMembers";
            this.btnEditExternalMembers.Size = new System.Drawing.Size(140, 23);
            this.btnEditExternalMembers.TabIndex = 2;
            this.btnEditExternalMembers.Text = "담당자 및 연락처 변경";
            this.btnEditExternalMembers.UseVisualStyleBackColor = true;
            this.btnEditExternalMembers.Visible = false;
            this.btnEditExternalMembers.Click += new System.EventHandler(this.btnEditExternalMembers_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(3, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(59, 15);
            this.labelTitle.TabIndex = 1;
            this.labelTitle.Text = "임무 목록";
            this.labelTitle.Visible = false;
            this.labelTitle.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnComponentContentsDoubleClick);
            // 
            // PageBackstageSOP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1093, 754);
            this.Controls.Add(this.panelBackImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageSOP";
            this.Text = "PageBackstageHome";
            this.Load += new System.EventHandler(this.PageBackstageHome_Load);
            this.Shown += new System.EventHandler(this.PageBackstageSOP_Shown);
            this.Resize += new System.EventHandler(this.PageBackstageHome_Resize);
            this.panelBackImage.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVerticalLine)).EndInit();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.panelTabPage.ResumeLayout(false);
            this.splitContainerVertical.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).EndInit();
            this.splitContainerVertical.ResumeLayout(false);
            this.tabLogs.ResumeLayout(false);
            this.panelComponentContentsTitle.ResumeLayout(false);
            this.panelComponentContentsTitle.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private PanelSOP panelBackImage;
        public  UnE.SOP.Sections.SectionTabPage tabPage1;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.SplitContainer splitContainerVertical;
        private System.Windows.Forms.TabControl tabLogs;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Timer timerBackgroundImage;
        private System.Windows.Forms.Label labelComponentContentsTitle;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button btnEditExternalMembers;
        private System.Windows.Forms.Timer timerSelectMission;
        private UnE.GUI.RibbonButton rbtnActionStep4th;
        private UnE.GUI.RibbonButton rbtnActionStep3rd;
        private UnE.GUI.RibbonButton rbtnActionStep2nd;
        private UnE.GUI.RibbonButton rbtnActionStep1st;
        private System.Windows.Forms.PictureBox pictureBoxVerticalLine;
        private System.Windows.Forms.Label labelActionStep1stName;
        private System.Windows.Forms.Label labelActionStep2ndName;
        private System.Windows.Forms.Label labelActionStep3rdName;
        private System.Windows.Forms.Label labelActionStep4thName;
        private System.Windows.Forms.Panel panelTabPage;
        public UnE.SOP.Sections.SectionTabControl tabControl;
        private System.Windows.Forms.Panel panelScenarioTab;
        private System.Windows.Forms.Panel panelComponentContentsTitle;
        private System.Windows.Forms.Panel panelMark;
        private UnE.GUI.RibbonButton rbtnBroadcast;
        private UnE.GUI.RibbonButton rbtnSMS;
    }
}