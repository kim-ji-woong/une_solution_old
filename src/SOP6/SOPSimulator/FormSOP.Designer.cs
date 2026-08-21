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
            this.labelTime = new System.Windows.Forms.Label();
            this.labelDate = new System.Windows.Forms.Label();
            this.pictureBoxCCTV = new UnE.GUI.TextPictureBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.btnMin = new System.Windows.Forms.Button();
            this.btnMax = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelViewRibbonBarMiddle = new System.Windows.Forms.Panel();
            this.btnDefaultCCTV = new UnE.GUI.RibbonButton();
            this.btnMissionStatus = new UnE.GUI.RibbonButton();
            this.btnBulletin = new UnE.GUI.RibbonButton();
            this.btnSOP = new UnE.GUI.RibbonButton();
            this.btnSDMS = new UnE.GUI.RibbonButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.btnOption = new UnE.GUI.RibbonButton();
            this.panelMode = new System.Windows.Forms.Panel();
            this.labelSelectMode = new System.Windows.Forms.Label();
            this.panelRealMode = new System.Windows.Forms.Panel();
            this.labelVirtual = new System.Windows.Forms.Label();
            this.labelReal = new System.Windows.Forms.Label();
            this.radioVirtualMode = new System.Windows.Forms.RadioButton();
            this.radioRealMode = new System.Windows.Forms.RadioButton();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.panelNormalMode = new System.Windows.Forms.Panel();
            this.radioHoliday = new System.Windows.Forms.RadioButton();
            this.labelHoliday = new System.Windows.Forms.Label();
            this.radioNormal = new System.Windows.Forms.RadioButton();
            this.labelNormal = new System.Windows.Forms.Label();
            this.btnWork = new UnE.GUI.RibbonButton();
            this.panelRealTimeInfo = new UnE.Utility.RealTimeInfoPane();
            this.label2 = new System.Windows.Forms.Label();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelMode = new System.Windows.Forms.Label();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnReturnControl = new UnE.GUI.RibbonButton();
            this.btnCancelSOP = new UnE.GUI.RibbonButton();
            this.btnFitToScale = new UnE.GUI.RibbonButton();
            this.btnFitToCurrentComponent = new UnE.GUI.RibbonButton();
            this.btnStartSOP = new UnE.GUI.RibbonButton();
            this.btnControl = new UnE.GUI.RibbonButton();
            this.panelViewRibbonBarRight = new System.Windows.Forms.Panel();
            this.panelViewRibbonBarLeft = new System.Windows.Forms.Panel();
            this.pictureBoxView = new UnE.GUI.TextPictureBox();
            this.pictureBoxMessage = new UnE.GUI.TextPictureBox();
            this.pictureBoxOpt = new UnE.GUI.TextPictureBox();
            this.pictureBoxMainIcon = new System.Windows.Forms.PictureBox();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCCTV)).BeginInit();
            this.panelViewRibbonBarMiddle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            this.panelMode.SuspendLayout();
            this.panelRealMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.panelNormalMode.SuspendLayout();
            this.panelRealTimeInfo.SuspendLayout();
            this.panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMessage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOpt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMainIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
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
            this.panelTop.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.ToolbarBkgnd;
            this.panelTop.Controls.Add(this.labelTime);
            this.panelTop.Controls.Add(this.labelDate);
            this.panelTop.Controls.Add(this.pictureBoxCCTV);
            this.panelTop.Controls.Add(this.labelTitle);
            this.panelTop.Controls.Add(this.btnMin);
            this.panelTop.Controls.Add(this.btnMax);
            this.panelTop.Controls.Add(this.btnClose);
            this.panelTop.Controls.Add(this.panelViewRibbonBarMiddle);
            this.panelTop.Controls.Add(this.panelViewRibbonBarRight);
            this.panelTop.Controls.Add(this.panelViewRibbonBarLeft);
            this.panelTop.Controls.Add(this.pictureBoxView);
            this.panelTop.Controls.Add(this.pictureBoxMessage);
            this.panelTop.Controls.Add(this.pictureBoxOpt);
            this.panelTop.Controls.Add(this.pictureBoxMainIcon);
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1920, 157);
            this.panelTop.TabIndex = 0;
            this.panelTop.DoubleClick += new System.EventHandler(this.panelTop_DoubleClick);
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            this.panelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseUp);
            // 
            // labelTime
            // 
            this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTime.AutoSize = true;
            this.labelTime.BackColor = System.Drawing.Color.Transparent;
            this.labelTime.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTime.ForeColor = System.Drawing.Color.White;
            this.labelTime.Location = new System.Drawing.Point(1838, 32);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(72, 21);
            this.labelTime.TabIndex = 7;
            this.labelTime.Text = "00:00:00";
            // 
            // labelDate
            // 
            this.labelDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDate.AutoSize = true;
            this.labelDate.BackColor = System.Drawing.Color.Transparent;
            this.labelDate.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDate.ForeColor = System.Drawing.Color.White;
            this.labelDate.Location = new System.Drawing.Point(1698, 32);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(124, 21);
            this.labelDate.TabIndex = 6;
            this.labelDate.Text = "2013년 7월 1일";
            // 
            // pictureBoxCCTV
            // 
            this.pictureBoxCCTV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pictureBoxCCTV.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            this.pictureBoxCCTV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxCCTV.Location = new System.Drawing.Point(197, 29);
            this.pictureBoxCCTV.Name = "pictureBoxCCTV";
            this.pictureBoxCCTV.Owner = null;
            this.pictureBoxCCTV.PictureBoxText = "CCTV";
            this.pictureBoxCCTV.Size = new System.Drawing.Size(98, 35);
            this.pictureBoxCCTV.TabIndex = 5;
            this.pictureBoxCCTV.TabStop = false;
            this.pictureBoxCCTV.Text = "CCTV";
            this.pictureBoxCCTV.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(30, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(70, 15);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "SOP 시스템";
            // 
            // btnMin
            // 
            this.btnMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMin.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.HideWindow_Normal;
            this.btnMin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMin.Location = new System.Drawing.Point(1829, 3);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(32, 24);
            this.btnMin.TabIndex = 3;
            this.btnMin.UseVisualStyleBackColor = true;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // btnMax
            // 
            this.btnMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMax.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.NormalWindow_Normal;
            this.btnMax.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMax.Location = new System.Drawing.Point(1859, 3);
            this.btnMax.Name = "btnMax";
            this.btnMax.Size = new System.Drawing.Size(32, 24);
            this.btnMax.TabIndex = 3;
            this.btnMax.UseVisualStyleBackColor = true;
            this.btnMax.Click += new System.EventHandler(this.btnMax_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.CloseWindow_Normal;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.Location = new System.Drawing.Point(1888, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(32, 24);
            this.btnClose.TabIndex = 3;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelViewRibbonBarMiddle
            // 
            this.panelViewRibbonBarMiddle.BackColor = System.Drawing.Color.Transparent;
            this.panelViewRibbonBarMiddle.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.RibbonBar_Middle;
            this.panelViewRibbonBarMiddle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnDefaultCCTV);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnMissionStatus);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnBulletin);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnSOP);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnSDMS);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox1);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox8);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnOption);
            this.panelViewRibbonBarMiddle.Controls.Add(this.panelMode);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnWork);
            this.panelViewRibbonBarMiddle.Controls.Add(this.panelRealTimeInfo);
            this.panelViewRibbonBarMiddle.Controls.Add(this.panelStatus);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox6);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox7);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox5);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox4);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox2);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnReturnControl);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnCancelSOP);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnFitToScale);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnFitToCurrentComponent);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnStartSOP);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnControl);
            this.panelViewRibbonBarMiddle.Location = new System.Drawing.Point(49, 67);
            this.panelViewRibbonBarMiddle.Name = "panelViewRibbonBarMiddle";
            this.panelViewRibbonBarMiddle.Size = new System.Drawing.Size(1775, 87);
            this.panelViewRibbonBarMiddle.TabIndex = 0;
            // 
            // btnDefaultCCTV
            // 
            this.btnDefaultCCTV.CheckButton = false;
            this.btnDefaultCCTV.CheckedBkgndImage = null;
            this.btnDefaultCCTV.CheckedImage = null;
            this.btnDefaultCCTV.CheckedMouseOver = null;
            this.btnDefaultCCTV.ClickedBackgroundImage = null;
            this.btnDefaultCCTV.ClickedImage = null;
            this.btnDefaultCCTV.CustomImageRect = new System.Drawing.Rectangle(0, 0, 89, 87);
            this.btnDefaultCCTV.DisabledBkgndImage = null;
            this.btnDefaultCCTV.DisabledImage = null;
            this.btnDefaultCCTV.ForeColorChecked = System.Drawing.Color.White;
            this.btnDefaultCCTV.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnDefaultCCTV.ForeColorDisabled = System.Drawing.Color.White;
            this.btnDefaultCCTV.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnDefaultCCTV.ForeColorsByTypeUse = false;
            this.btnDefaultCCTV.ID = -1;
            this.btnDefaultCCTV.InitButtonWidth = 87;
            this.btnDefaultCCTV.IsChecked = false;
            this.btnDefaultCCTV.Location = new System.Drawing.Point(1015, 0);
            this.btnDefaultCCTV.MouseOverBkgndImage = null;
            this.btnDefaultCCTV.MouseOverImage = null;
            this.btnDefaultCCTV.Name = "btnDefaultCCTV";
            this.btnDefaultCCTV.NormalImage = null;
            this.btnDefaultCCTV.Owner = null;
            this.btnDefaultCCTV.Size = new System.Drawing.Size(87, 87);
            this.btnDefaultCCTV.TabIndex = 6;
            this.btnDefaultCCTV.TextLocation = new System.Drawing.Point(0, 0);
            this.btnDefaultCCTV.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDefaultCCTV.ToolTipText = "";
            this.btnDefaultCCTV.UseCustomImageRect = true;
            this.btnDefaultCCTV.UseTextLocation = false;
            this.btnDefaultCCTV.UseVisualStyleBackColor = true;
            this.btnDefaultCCTV.Click += new System.EventHandler(this.btnDefaultCCTV_Click);
            // 
            // btnMissionStatus
            // 
            this.btnMissionStatus.CheckButton = false;
            this.btnMissionStatus.CheckedBkgndImage = null;
            this.btnMissionStatus.CheckedImage = null;
            this.btnMissionStatus.CheckedMouseOver = null;
            this.btnMissionStatus.ClickedBackgroundImage = null;
            this.btnMissionStatus.ClickedImage = null;
            this.btnMissionStatus.CustomImageRect = new System.Drawing.Rectangle(0, 0, 89, 87);
            this.btnMissionStatus.DisabledBkgndImage = null;
            this.btnMissionStatus.DisabledImage = null;
            this.btnMissionStatus.ForeColorChecked = System.Drawing.Color.White;
            this.btnMissionStatus.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnMissionStatus.ForeColorDisabled = System.Drawing.Color.White;
            this.btnMissionStatus.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnMissionStatus.ForeColorsByTypeUse = false;
            this.btnMissionStatus.ID = -1;
            this.btnMissionStatus.InitButtonWidth = 87;
            this.btnMissionStatus.IsChecked = false;
            this.btnMissionStatus.Location = new System.Drawing.Point(922, 0);
            this.btnMissionStatus.MouseOverBkgndImage = null;
            this.btnMissionStatus.MouseOverImage = null;
            this.btnMissionStatus.Name = "btnMissionStatus";
            this.btnMissionStatus.NormalImage = null;
            this.btnMissionStatus.Owner = null;
            this.btnMissionStatus.Size = new System.Drawing.Size(87, 87);
            this.btnMissionStatus.TabIndex = 5;
            this.btnMissionStatus.Text = "현황판";
            this.btnMissionStatus.TextLocation = new System.Drawing.Point(0, 0);
            this.btnMissionStatus.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnMissionStatus.ToolTipText = "현황판";
            this.btnMissionStatus.UseCustomImageRect = true;
            this.btnMissionStatus.UseTextLocation = false;
            this.btnMissionStatus.UseVisualStyleBackColor = true;
            this.btnMissionStatus.Click += new System.EventHandler(this.btnMissionStatus_Click);
            // 
            // btnBulletin
            // 
            this.btnBulletin.CheckButton = false;
            this.btnBulletin.CheckedBkgndImage = null;
            this.btnBulletin.CheckedImage = null;
            this.btnBulletin.CheckedMouseOver = null;
            this.btnBulletin.ClickedBackgroundImage = null;
            this.btnBulletin.ClickedImage = null;
            this.btnBulletin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 89, 87);
            this.btnBulletin.DisabledBkgndImage = null;
            this.btnBulletin.DisabledImage = null;
            this.btnBulletin.ForeColorChecked = System.Drawing.Color.White;
            this.btnBulletin.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnBulletin.ForeColorDisabled = System.Drawing.Color.White;
            this.btnBulletin.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnBulletin.ForeColorsByTypeUse = false;
            this.btnBulletin.ID = -1;
            this.btnBulletin.InitButtonWidth = 87;
            this.btnBulletin.IsChecked = false;
            this.btnBulletin.Location = new System.Drawing.Point(829, 0);
            this.btnBulletin.MouseOverBkgndImage = null;
            this.btnBulletin.MouseOverImage = null;
            this.btnBulletin.Name = "btnBulletin";
            this.btnBulletin.NormalImage = null;
            this.btnBulletin.Owner = null;
            this.btnBulletin.Size = new System.Drawing.Size(87, 87);
            this.btnBulletin.TabIndex = 5;
            this.btnBulletin.Text = "상황판";
            this.btnBulletin.TextLocation = new System.Drawing.Point(0, 0);
            this.btnBulletin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnBulletin.ToolTipText = "상황판";
            this.btnBulletin.UseCustomImageRect = true;
            this.btnBulletin.UseTextLocation = false;
            this.btnBulletin.UseVisualStyleBackColor = true;
            this.btnBulletin.Click += new System.EventHandler(this.btnBulletin_Click);
            // 
            // btnSOP
            // 
            this.btnSOP.CheckButton = false;
            this.btnSOP.CheckedBkgndImage = null;
            this.btnSOP.CheckedImage = null;
            this.btnSOP.CheckedMouseOver = null;
            this.btnSOP.ClickedBackgroundImage = null;
            this.btnSOP.ClickedImage = null;
            this.btnSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 89, 87);
            this.btnSOP.DisabledBkgndImage = null;
            this.btnSOP.DisabledImage = null;
            this.btnSOP.Enabled = false;
            this.btnSOP.ForeColorChecked = System.Drawing.Color.White;
            this.btnSOP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSOP.ForeColorDisabled = System.Drawing.Color.White;
            this.btnSOP.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnSOP.ForeColorsByTypeUse = false;
            this.btnSOP.ID = -1;
            this.btnSOP.InitButtonWidth = 87;
            this.btnSOP.IsChecked = false;
            this.btnSOP.Location = new System.Drawing.Point(736, 0);
            this.btnSOP.MouseOverBkgndImage = null;
            this.btnSOP.MouseOverImage = null;
            this.btnSOP.Name = "btnSOP";
            this.btnSOP.NormalImage = null;
            this.btnSOP.Owner = null;
            this.btnSOP.Size = new System.Drawing.Size(87, 87);
            this.btnSOP.TabIndex = 5;
            this.btnSOP.Text = "SOP시스템";
            this.btnSOP.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSOP.ToolTipText = "SOP시스템";
            this.btnSOP.UseCustomImageRect = true;
            this.btnSOP.UseTextLocation = false;
            this.btnSOP.UseVisualStyleBackColor = true;
            // 
            // btnSDMS
            // 
            this.btnSDMS.CheckButton = false;
            this.btnSDMS.CheckedBkgndImage = null;
            this.btnSDMS.CheckedImage = null;
            this.btnSDMS.CheckedMouseOver = null;
            this.btnSDMS.ClickedBackgroundImage = null;
            this.btnSDMS.ClickedImage = null;
            this.btnSDMS.CustomImageRect = new System.Drawing.Rectangle(0, 0, 89, 87);
            this.btnSDMS.DisabledBkgndImage = null;
            this.btnSDMS.DisabledImage = null;
            this.btnSDMS.ForeColorChecked = System.Drawing.Color.White;
            this.btnSDMS.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSDMS.ForeColorDisabled = System.Drawing.Color.White;
            this.btnSDMS.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnSDMS.ForeColorsByTypeUse = false;
            this.btnSDMS.ID = -1;
            this.btnSDMS.InitButtonWidth = 87;
            this.btnSDMS.IsChecked = false;
            this.btnSDMS.Location = new System.Drawing.Point(643, 0);
            this.btnSDMS.MouseOverBkgndImage = null;
            this.btnSDMS.MouseOverImage = null;
            this.btnSDMS.Name = "btnSDMS";
            this.btnSDMS.NormalImage = null;
            this.btnSDMS.Owner = null;
            this.btnSDMS.Size = new System.Drawing.Size(87, 87);
            this.btnSDMS.TabIndex = 5;
            this.btnSDMS.Text = "재난탐지";
            this.btnSDMS.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSDMS.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSDMS.ToolTipText = "재난탐지";
            this.btnSDMS.UseCustomImageRect = true;
            this.btnSDMS.UseTextLocation = false;
            this.btnSDMS.UseVisualStyleBackColor = true;
            this.btnSDMS.Click += new System.EventHandler(this.btnSDMS_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(490, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(13, 85);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox8
            // 
            this.pictureBox8.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox8.Location = new System.Drawing.Point(135, 3);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(13, 85);
            this.pictureBox8.TabIndex = 10;
            this.pictureBox8.TabStop = false;
            // 
            // btnOption
            // 
            this.btnOption.CheckButton = false;
            this.btnOption.CheckedBkgndImage = null;
            this.btnOption.CheckedImage = null;
            this.btnOption.CheckedMouseOver = null;
            this.btnOption.ClickedBackgroundImage = null;
            this.btnOption.ClickedImage = null;
            this.btnOption.CustomImageRect = new System.Drawing.Rectangle(14, 5, 32, 50);
            this.btnOption.DisabledBkgndImage = null;
            this.btnOption.DisabledImage = null;
            this.btnOption.ForeColorChecked = System.Drawing.Color.White;
            this.btnOption.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnOption.ForeColorDisabled = System.Drawing.Color.White;
            this.btnOption.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnOption.ForeColorsByTypeUse = false;
            this.btnOption.ID = -1;
            this.btnOption.InitButtonWidth = 60;
            this.btnOption.IsChecked = false;
            this.btnOption.Location = new System.Drawing.Point(69, 1);
            this.btnOption.MouseOverBkgndImage = null;
            this.btnOption.MouseOverImage = null;
            this.btnOption.Name = "btnOption";
            this.btnOption.NormalImage = null;
            this.btnOption.Owner = null;
            this.btnOption.Size = new System.Drawing.Size(60, 85);
            this.btnOption.TabIndex = 9;
            this.btnOption.Text = "실행설정";
            this.btnOption.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOption.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOption.ToolTipText = "실행설정";
            this.btnOption.UseCustomImageRect = true;
            this.btnOption.UseTextLocation = false;
            this.btnOption.UseVisualStyleBackColor = true;
            // 
            // panelMode
            // 
            this.panelMode.Controls.Add(this.labelSelectMode);
            this.panelMode.Controls.Add(this.panelRealMode);
            this.panelMode.Controls.Add(this.pictureBox3);
            this.panelMode.Controls.Add(this.panelNormalMode);
            this.panelMode.Location = new System.Drawing.Point(292, 1);
            this.panelMode.Name = "panelMode";
            this.panelMode.Size = new System.Drawing.Size(207, 85);
            this.panelMode.TabIndex = 8;
            // 
            // labelSelectMode
            // 
            this.labelSelectMode.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSelectMode.ForeColor = System.Drawing.Color.White;
            this.labelSelectMode.Location = new System.Drawing.Point(8, 1);
            this.labelSelectMode.Name = "labelSelectMode";
            this.labelSelectMode.Size = new System.Drawing.Size(198, 15);
            this.labelSelectMode.TabIndex = 2;
            this.labelSelectMode.Text = "SOP 실행환경";
            // 
            // panelRealMode
            // 
            this.panelRealMode.Controls.Add(this.labelVirtual);
            this.panelRealMode.Controls.Add(this.labelReal);
            this.panelRealMode.Controls.Add(this.radioVirtualMode);
            this.panelRealMode.Controls.Add(this.radioRealMode);
            this.panelRealMode.Location = new System.Drawing.Point(0, 26);
            this.panelRealMode.Name = "panelRealMode";
            this.panelRealMode.Size = new System.Drawing.Size(86, 59);
            this.panelRealMode.TabIndex = 2;
            // 
            // labelVirtual
            // 
            this.labelVirtual.AutoSize = true;
            this.labelVirtual.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelVirtual.ForeColor = System.Drawing.Color.White;
            this.labelVirtual.Location = new System.Drawing.Point(26, 35);
            this.labelVirtual.Name = "labelVirtual";
            this.labelVirtual.Size = new System.Drawing.Size(55, 15);
            this.labelVirtual.TabIndex = 1;
            this.labelVirtual.Text = "훈련모드";
            this.labelVirtual.Click += new System.EventHandler(this.labelRadio_Click);
            // 
            // labelReal
            // 
            this.labelReal.AutoSize = true;
            this.labelReal.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelReal.ForeColor = System.Drawing.Color.White;
            this.labelReal.Location = new System.Drawing.Point(26, 5);
            this.labelReal.Name = "labelReal";
            this.labelReal.Size = new System.Drawing.Size(55, 15);
            this.labelReal.TabIndex = 1;
            this.labelReal.Text = "실제모드";
            this.labelReal.Click += new System.EventHandler(this.labelRadio_Click);
            // 
            // radioVirtualMode
            // 
            this.radioVirtualMode.AutoSize = true;
            this.radioVirtualMode.Checked = true;
            this.radioVirtualMode.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.radioVirtualMode.ForeColor = System.Drawing.Color.White;
            this.radioVirtualMode.Location = new System.Drawing.Point(10, 37);
            this.radioVirtualMode.Name = "radioVirtualMode";
            this.radioVirtualMode.Size = new System.Drawing.Size(14, 13);
            this.radioVirtualMode.TabIndex = 0;
            this.radioVirtualMode.TabStop = true;
            this.radioVirtualMode.UseVisualStyleBackColor = true;
            this.radioVirtualMode.CheckedChanged += new System.EventHandler(this.radioRealMode_CheckedChanged);
            // 
            // radioRealMode
            // 
            this.radioRealMode.AutoSize = true;
            this.radioRealMode.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioRealMode.ForeColor = System.Drawing.Color.White;
            this.radioRealMode.Location = new System.Drawing.Point(10, 6);
            this.radioRealMode.Name = "radioRealMode";
            this.radioRealMode.Size = new System.Drawing.Size(14, 13);
            this.radioRealMode.TabIndex = 0;
            this.radioRealMode.UseVisualStyleBackColor = true;
            this.radioRealMode.CheckedChanged += new System.EventHandler(this.radioRealMode_CheckedChanged);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox3.Location = new System.Drawing.Point(82, 27);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(13, 60);
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Visible = false;
            // 
            // panelNormalMode
            // 
            this.panelNormalMode.Controls.Add(this.radioHoliday);
            this.panelNormalMode.Controls.Add(this.labelHoliday);
            this.panelNormalMode.Controls.Add(this.radioNormal);
            this.panelNormalMode.Controls.Add(this.labelNormal);
            this.panelNormalMode.Location = new System.Drawing.Point(93, 26);
            this.panelNormalMode.Name = "panelNormalMode";
            this.panelNormalMode.Size = new System.Drawing.Size(108, 59);
            this.panelNormalMode.TabIndex = 2;
            // 
            // radioHoliday
            // 
            this.radioHoliday.AutoSize = true;
            this.radioHoliday.Enabled = false;
            this.radioHoliday.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.radioHoliday.ForeColor = System.Drawing.Color.White;
            this.radioHoliday.Location = new System.Drawing.Point(10, 37);
            this.radioHoliday.Name = "radioHoliday";
            this.radioHoliday.Size = new System.Drawing.Size(14, 13);
            this.radioHoliday.TabIndex = 0;
            this.radioHoliday.UseVisualStyleBackColor = true;
            this.radioHoliday.CheckedChanged += new System.EventHandler(this.radioNormalMode_CheckedChanged);
            // 
            // labelHoliday
            // 
            this.labelHoliday.AutoSize = true;
            this.labelHoliday.Enabled = false;
            this.labelHoliday.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelHoliday.ForeColor = System.Drawing.Color.White;
            this.labelHoliday.Location = new System.Drawing.Point(26, 35);
            this.labelHoliday.Name = "labelHoliday";
            this.labelHoliday.Size = new System.Drawing.Size(75, 15);
            this.labelHoliday.TabIndex = 1;
            this.labelHoliday.Text = "야간 및 휴일";
            this.labelHoliday.Click += new System.EventHandler(this.labelRadio_Click);
            // 
            // radioNormal
            // 
            this.radioNormal.AutoSize = true;
            this.radioNormal.Checked = true;
            this.radioNormal.Enabled = false;
            this.radioNormal.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioNormal.ForeColor = System.Drawing.Color.White;
            this.radioNormal.Location = new System.Drawing.Point(10, 6);
            this.radioNormal.Name = "radioNormal";
            this.radioNormal.Size = new System.Drawing.Size(14, 13);
            this.radioNormal.TabIndex = 0;
            this.radioNormal.TabStop = true;
            this.radioNormal.UseVisualStyleBackColor = true;
            this.radioNormal.CheckedChanged += new System.EventHandler(this.radioNormalMode_CheckedChanged);
            // 
            // labelNormal
            // 
            this.labelNormal.AutoSize = true;
            this.labelNormal.Enabled = false;
            this.labelNormal.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNormal.ForeColor = System.Drawing.Color.White;
            this.labelNormal.Location = new System.Drawing.Point(26, 5);
            this.labelNormal.Name = "labelNormal";
            this.labelNormal.Size = new System.Drawing.Size(31, 15);
            this.labelNormal.TabIndex = 1;
            this.labelNormal.Text = "평일";
            this.labelNormal.Click += new System.EventHandler(this.labelRadio_Click);
            // 
            // btnWork
            // 
            this.btnWork.CheckButton = false;
            this.btnWork.CheckedBkgndImage = null;
            this.btnWork.CheckedImage = null;
            this.btnWork.CheckedMouseOver = null;
            this.btnWork.ClickedBackgroundImage = null;
            this.btnWork.ClickedImage = null;
            this.btnWork.CustomImageRect = new System.Drawing.Rectangle(14, 2, 32, 54);
            this.btnWork.DisabledBkgndImage = null;
            this.btnWork.DisabledImage = null;
            this.btnWork.ForeColorChecked = System.Drawing.Color.White;
            this.btnWork.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnWork.ForeColorDisabled = System.Drawing.Color.White;
            this.btnWork.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnWork.ForeColorsByTypeUse = false;
            this.btnWork.ID = -1;
            this.btnWork.InitButtonWidth = 60;
            this.btnWork.IsChecked = false;
            this.btnWork.Location = new System.Drawing.Point(3, 1);
            this.btnWork.MouseOverBkgndImage = null;
            this.btnWork.MouseOverImage = null;
            this.btnWork.Name = "btnWork";
            this.btnWork.NormalImage = null;
            this.btnWork.Owner = null;
            this.btnWork.Size = new System.Drawing.Size(60, 85);
            this.btnWork.TabIndex = 7;
            this.btnWork.Text = "근무표";
            this.btnWork.TextLocation = new System.Drawing.Point(0, 0);
            this.btnWork.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnWork.ToolTipText = "근무표";
            this.btnWork.UseCustomImageRect = true;
            this.btnWork.UseTextLocation = false;
            this.btnWork.UseVisualStyleBackColor = true;
            // 
            // panelRealTimeInfo
            // 
            this.panelRealTimeInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelRealTimeInfo.Controls.Add(this.label2);
            this.panelRealTimeInfo.DisplayBeginPosition = new System.Drawing.Point(30, 27);
            this.panelRealTimeInfo.Location = new System.Drawing.Point(1487, 3);
            this.panelRealTimeInfo.Name = "panelRealTimeInfo";
            this.panelRealTimeInfo.RealTimeInfo = null;
            this.panelRealTimeInfo.Size = new System.Drawing.Size(200, 83);
            this.panelRealTimeInfo.TabIndex = 4;
            this.panelRealTimeInfo.Text = "FormRealTimeInfo";
            this.panelRealTimeInfo.TextColor = System.Drawing.Color.White;
            this.panelRealTimeInfo.UseMovingText = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(8, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "메시지";
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.label1);
            this.panelStatus.Controls.Add(this.pictureBoxStatus);
            this.panelStatus.Controls.Add(this.labelStatus);
            this.panelStatus.Controls.Add(this.labelMode);
            this.panelStatus.Location = new System.Drawing.Point(1247, 3);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(227, 83);
            this.panelStatus.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(8, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "현재 모드";
            // 
            // pictureBoxStatus
            // 
            this.pictureBoxStatus.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxStatus.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Wait_Status;
            this.pictureBoxStatus.Location = new System.Drawing.Point(151, 36);
            this.pictureBoxStatus.Name = "pictureBoxStatus";
            this.pictureBoxStatus.Size = new System.Drawing.Size(12, 12);
            this.pictureBoxStatus.TabIndex = 3;
            this.pictureBoxStatus.TabStop = false;
            this.pictureBoxStatus.Visible = false;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.BackColor = System.Drawing.Color.Transparent;
            this.labelStatus.Font = new System.Drawing.Font("맑은 고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelStatus.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labelStatus.Location = new System.Drawing.Point(166, 24);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(55, 30);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "대기";
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.BackColor = System.Drawing.Color.Transparent;
            this.labelMode.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMode.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labelMode.Location = new System.Drawing.Point(4, 18);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(133, 40);
            this.labelMode.TabIndex = 1;
            this.labelMode.Text = "훈련모드";
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox6.Location = new System.Drawing.Point(1236, 3);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(13, 85);
            this.pictureBox6.TabIndex = 1;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox7.Location = new System.Drawing.Point(1474, 3);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(13, 85);
            this.pictureBox7.TabIndex = 1;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox5.Location = new System.Drawing.Point(1102, 3);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(13, 85);
            this.pictureBox5.TabIndex = 1;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox4.Location = new System.Drawing.Point(629, 3);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(13, 85);
            this.pictureBox4.TabIndex = 1;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(280, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(13, 85);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // btnReturnControl
            // 
            this.btnReturnControl.CheckButton = false;
            this.btnReturnControl.CheckedBkgndImage = null;
            this.btnReturnControl.CheckedImage = null;
            this.btnReturnControl.CheckedMouseOver = null;
            this.btnReturnControl.ClickedBackgroundImage = null;
            this.btnReturnControl.ClickedImage = null;
            this.btnReturnControl.CustomImageRect = new System.Drawing.Rectangle(20, 5, 32, 50);
            this.btnReturnControl.DisabledBkgndImage = null;
            this.btnReturnControl.DisabledImage = null;
            this.btnReturnControl.ForeColorChecked = System.Drawing.Color.White;
            this.btnReturnControl.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnReturnControl.ForeColorDisabled = System.Drawing.Color.White;
            this.btnReturnControl.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnReturnControl.ForeColorsByTypeUse = false;
            this.btnReturnControl.ID = -1;
            this.btnReturnControl.InitButtonWidth = 60;
            this.btnReturnControl.IsChecked = false;
            this.btnReturnControl.Location = new System.Drawing.Point(201, 1);
            this.btnReturnControl.MouseOverBkgndImage = null;
            this.btnReturnControl.MouseOverImage = null;
            this.btnReturnControl.Name = "btnReturnControl";
            this.btnReturnControl.NormalImage = null;
            this.btnReturnControl.Owner = null;
            this.btnReturnControl.Size = new System.Drawing.Size(73, 85);
            this.btnReturnControl.TabIndex = 0;
            this.btnReturnControl.Text = "제어권 반납";
            this.btnReturnControl.TextLocation = new System.Drawing.Point(0, 0);
            this.btnReturnControl.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnReturnControl.ToolTipText = "제어권 반납";
            this.btnReturnControl.UseCustomImageRect = true;
            this.btnReturnControl.UseTextLocation = false;
            this.btnReturnControl.UseVisualStyleBackColor = true;
            // 
            // btnCancelSOP
            // 
            this.btnCancelSOP.CheckButton = false;
            this.btnCancelSOP.CheckedBkgndImage = null;
            this.btnCancelSOP.CheckedImage = null;
            this.btnCancelSOP.CheckedMouseOver = null;
            this.btnCancelSOP.ClickedBackgroundImage = null;
            this.btnCancelSOP.ClickedImage = null;
            this.btnCancelSOP.CustomImageRect = new System.Drawing.Rectangle(15, 20, 32, 32);
            this.btnCancelSOP.DisabledBkgndImage = null;
            this.btnCancelSOP.DisabledImage = null;
            this.btnCancelSOP.ForeColorChecked = System.Drawing.Color.White;
            this.btnCancelSOP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnCancelSOP.ForeColorDisabled = System.Drawing.Color.White;
            this.btnCancelSOP.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnCancelSOP.ForeColorsByTypeUse = false;
            this.btnCancelSOP.ID = -1;
            this.btnCancelSOP.InitButtonWidth = 60;
            this.btnCancelSOP.IsChecked = false;
            this.btnCancelSOP.Location = new System.Drawing.Point(571, 1);
            this.btnCancelSOP.MouseOverBkgndImage = null;
            this.btnCancelSOP.MouseOverImage = null;
            this.btnCancelSOP.Name = "btnCancelSOP";
            this.btnCancelSOP.NormalImage = null;
            this.btnCancelSOP.Owner = null;
            this.btnCancelSOP.Size = new System.Drawing.Size(60, 85);
            this.btnCancelSOP.TabIndex = 0;
            this.btnCancelSOP.Text = "실행취소";
            this.btnCancelSOP.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancelSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancelSOP.ToolTipText = "실행취소";
            this.btnCancelSOP.UseCustomImageRect = true;
            this.btnCancelSOP.UseTextLocation = false;
            this.btnCancelSOP.UseVisualStyleBackColor = true;
            // 
            // btnFitToScale
            // 
            this.btnFitToScale.CheckButton = false;
            this.btnFitToScale.CheckedBkgndImage = null;
            this.btnFitToScale.CheckedImage = null;
            this.btnFitToScale.CheckedMouseOver = null;
            this.btnFitToScale.ClickedBackgroundImage = null;
            this.btnFitToScale.ClickedImage = null;
            this.btnFitToScale.CustomImageRect = new System.Drawing.Rectangle(15, 20, 32, 32);
            this.btnFitToScale.DisabledBkgndImage = null;
            this.btnFitToScale.DisabledImage = null;
            this.btnFitToScale.ForeColorChecked = System.Drawing.Color.White;
            this.btnFitToScale.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnFitToScale.ForeColorDisabled = System.Drawing.Color.White;
            this.btnFitToScale.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnFitToScale.ForeColorsByTypeUse = false;
            this.btnFitToScale.ID = -1;
            this.btnFitToScale.InitButtonWidth = 60;
            this.btnFitToScale.IsChecked = false;
            this.btnFitToScale.Location = new System.Drawing.Point(1177, 1);
            this.btnFitToScale.MouseOverBkgndImage = null;
            this.btnFitToScale.MouseOverImage = null;
            this.btnFitToScale.Name = "btnFitToScale";
            this.btnFitToScale.NormalImage = null;
            this.btnFitToScale.Owner = null;
            this.btnFitToScale.Size = new System.Drawing.Size(60, 85);
            this.btnFitToScale.TabIndex = 0;
            this.btnFitToScale.Text = "전체확대";
            this.btnFitToScale.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFitToScale.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFitToScale.ToolTipText = "전체확대";
            this.btnFitToScale.UseCustomImageRect = true;
            this.btnFitToScale.UseTextLocation = false;
            this.btnFitToScale.UseVisualStyleBackColor = true;
            // 
            // btnFitToCurrentComponent
            // 
            this.btnFitToCurrentComponent.CheckButton = false;
            this.btnFitToCurrentComponent.CheckedBkgndImage = null;
            this.btnFitToCurrentComponent.CheckedImage = null;
            this.btnFitToCurrentComponent.CheckedMouseOver = null;
            this.btnFitToCurrentComponent.ClickedBackgroundImage = null;
            this.btnFitToCurrentComponent.ClickedImage = null;
            this.btnFitToCurrentComponent.CustomImageRect = new System.Drawing.Rectangle(15, 20, 32, 32);
            this.btnFitToCurrentComponent.DisabledBkgndImage = null;
            this.btnFitToCurrentComponent.DisabledImage = null;
            this.btnFitToCurrentComponent.ForeColorChecked = System.Drawing.Color.White;
            this.btnFitToCurrentComponent.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnFitToCurrentComponent.ForeColorDisabled = System.Drawing.Color.White;
            this.btnFitToCurrentComponent.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnFitToCurrentComponent.ForeColorsByTypeUse = false;
            this.btnFitToCurrentComponent.ID = -1;
            this.btnFitToCurrentComponent.InitButtonWidth = 60;
            this.btnFitToCurrentComponent.IsChecked = false;
            this.btnFitToCurrentComponent.Location = new System.Drawing.Point(1117, 1);
            this.btnFitToCurrentComponent.MouseOverBkgndImage = null;
            this.btnFitToCurrentComponent.MouseOverImage = null;
            this.btnFitToCurrentComponent.Name = "btnFitToCurrentComponent";
            this.btnFitToCurrentComponent.NormalImage = null;
            this.btnFitToCurrentComponent.Owner = null;
            this.btnFitToCurrentComponent.Size = new System.Drawing.Size(60, 85);
            this.btnFitToCurrentComponent.TabIndex = 0;
            this.btnFitToCurrentComponent.Text = "부분확대";
            this.btnFitToCurrentComponent.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFitToCurrentComponent.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFitToCurrentComponent.ToolTipText = "부분확대";
            this.btnFitToCurrentComponent.UseCustomImageRect = true;
            this.btnFitToCurrentComponent.UseTextLocation = false;
            this.btnFitToCurrentComponent.UseVisualStyleBackColor = true;
            // 
            // btnStartSOP
            // 
            this.btnStartSOP.CheckButton = false;
            this.btnStartSOP.CheckedBkgndImage = null;
            this.btnStartSOP.CheckedImage = null;
            this.btnStartSOP.CheckedMouseOver = null;
            this.btnStartSOP.ClickedBackgroundImage = null;
            this.btnStartSOP.ClickedImage = null;
            this.btnStartSOP.CustomImageRect = new System.Drawing.Rectangle(15, 20, 32, 32);
            this.btnStartSOP.DisabledBkgndImage = null;
            this.btnStartSOP.DisabledImage = null;
            this.btnStartSOP.ForeColorChecked = System.Drawing.Color.White;
            this.btnStartSOP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnStartSOP.ForeColorDisabled = System.Drawing.Color.White;
            this.btnStartSOP.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnStartSOP.ForeColorsByTypeUse = false;
            this.btnStartSOP.ID = -1;
            this.btnStartSOP.InitButtonWidth = 60;
            this.btnStartSOP.IsChecked = false;
            this.btnStartSOP.Location = new System.Drawing.Point(505, 1);
            this.btnStartSOP.MouseOverBkgndImage = null;
            this.btnStartSOP.MouseOverImage = null;
            this.btnStartSOP.Name = "btnStartSOP";
            this.btnStartSOP.NormalImage = null;
            this.btnStartSOP.Owner = null;
            this.btnStartSOP.Size = new System.Drawing.Size(60, 85);
            this.btnStartSOP.TabIndex = 0;
            this.btnStartSOP.Text = "시작";
            this.btnStartSOP.TextLocation = new System.Drawing.Point(0, 0);
            this.btnStartSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnStartSOP.ToolTipText = "시작";
            this.btnStartSOP.UseCustomImageRect = true;
            this.btnStartSOP.UseTextLocation = false;
            this.btnStartSOP.UseVisualStyleBackColor = true;
            // 
            // btnControl
            // 
            this.btnControl.CheckButton = false;
            this.btnControl.CheckedBkgndImage = null;
            this.btnControl.CheckedImage = null;
            this.btnControl.CheckedMouseOver = null;
            this.btnControl.ClickedBackgroundImage = null;
            this.btnControl.ClickedImage = null;
            this.btnControl.CustomImageRect = new System.Drawing.Rectangle(14, 5, 32, 50);
            this.btnControl.DisabledBkgndImage = null;
            this.btnControl.DisabledImage = null;
            this.btnControl.ForeColorChecked = System.Drawing.Color.White;
            this.btnControl.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnControl.ForeColorDisabled = System.Drawing.Color.White;
            this.btnControl.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnControl.ForeColorsByTypeUse = false;
            this.btnControl.ID = -1;
            this.btnControl.InitButtonWidth = 60;
            this.btnControl.IsChecked = false;
            this.btnControl.Location = new System.Drawing.Point(150, 1);
            this.btnControl.MouseOverBkgndImage = null;
            this.btnControl.MouseOverImage = null;
            this.btnControl.Name = "btnControl";
            this.btnControl.NormalImage = null;
            this.btnControl.Owner = null;
            this.btnControl.Size = new System.Drawing.Size(60, 85);
            this.btnControl.TabIndex = 0;
            this.btnControl.Text = "모니터링";
            this.btnControl.TextLocation = new System.Drawing.Point(0, 0);
            this.btnControl.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnControl.ToolTipText = "모니터링";
            this.btnControl.UseCustomImageRect = true;
            this.btnControl.UseTextLocation = false;
            this.btnControl.UseVisualStyleBackColor = true;
            // 
            // panelViewRibbonBarRight
            // 
            this.panelViewRibbonBarRight.BackColor = System.Drawing.Color.Transparent;
            this.panelViewRibbonBarRight.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.RibbonBar_Right;
            this.panelViewRibbonBarRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelViewRibbonBarRight.Location = new System.Drawing.Point(1830, 67);
            this.panelViewRibbonBarRight.Name = "panelViewRibbonBarRight";
            this.panelViewRibbonBarRight.Size = new System.Drawing.Size(90, 87);
            this.panelViewRibbonBarRight.TabIndex = 1;
            // 
            // panelViewRibbonBarLeft
            // 
            this.panelViewRibbonBarLeft.BackColor = System.Drawing.Color.Transparent;
            this.panelViewRibbonBarLeft.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.RibbonBar_Left;
            this.panelViewRibbonBarLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelViewRibbonBarLeft.Location = new System.Drawing.Point(2, 67);
            this.panelViewRibbonBarLeft.Name = "panelViewRibbonBarLeft";
            this.panelViewRibbonBarLeft.Size = new System.Drawing.Size(41, 87);
            this.panelViewRibbonBarLeft.TabIndex = 2;
            // 
            // pictureBoxView
            // 
            this.pictureBoxView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pictureBoxView.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            this.pictureBoxView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxView.Location = new System.Drawing.Point(99, 29);
            this.pictureBoxView.Name = "pictureBoxView";
            this.pictureBoxView.Owner = null;
            this.pictureBoxView.PictureBoxText = "실행";
            this.pictureBoxView.Size = new System.Drawing.Size(98, 35);
            this.pictureBoxView.TabIndex = 1;
            this.pictureBoxView.TabStop = false;
            this.pictureBoxView.Text = "실행";
            this.pictureBoxView.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxMessage
            // 
            this.pictureBoxMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pictureBoxMessage.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            this.pictureBoxMessage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxMessage.Location = new System.Drawing.Point(295, 29);
            this.pictureBoxMessage.Name = "pictureBoxMessage";
            this.pictureBoxMessage.Owner = null;
            this.pictureBoxMessage.PictureBoxText = "메시지 관리";
            this.pictureBoxMessage.Size = new System.Drawing.Size(98, 35);
            this.pictureBoxMessage.TabIndex = 1;
            this.pictureBoxMessage.TabStop = false;
            this.pictureBoxMessage.Text = "메시지 관리";
            this.pictureBoxMessage.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxOpt
            // 
            this.pictureBoxOpt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pictureBoxOpt.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            this.pictureBoxOpt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxOpt.Location = new System.Drawing.Point(1, 29);
            this.pictureBoxOpt.Name = "pictureBoxOpt";
            this.pictureBoxOpt.Owner = null;
            this.pictureBoxOpt.PictureBoxText = "실행준비";
            this.pictureBoxOpt.Size = new System.Drawing.Size(98, 35);
            this.pictureBoxOpt.TabIndex = 1;
            this.pictureBoxOpt.TabStop = false;
            this.pictureBoxOpt.Text = "실행준비";
            this.pictureBoxOpt.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxMainIcon
            // 
            this.pictureBoxMainIcon.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Monitoring_32;
            this.pictureBoxMainIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxMainIcon.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxMainIcon.Name = "pictureBoxMainIcon";
            this.pictureBoxMainIcon.Size = new System.Drawing.Size(24, 24);
            this.pictureBoxMainIcon.TabIndex = 0;
            this.pictureBoxMainIcon.TabStop = false;
            this.pictureBoxMainIcon.DoubleClick += new System.EventHandler(this.pictureBoxMainIcon_DoubleClick);
            // 
            // FormSOP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 719);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSOP";
            this.Text = "SOP Monitoring System";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCCTV)).EndInit();
            this.panelViewRibbonBarMiddle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            this.panelMode.ResumeLayout(false);
            this.panelRealMode.ResumeLayout(false);
            this.panelRealMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.panelNormalMode.ResumeLayout(false);
            this.panelNormalMode.PerformLayout();
            this.panelRealTimeInfo.ResumeLayout(false);
            this.panelRealTimeInfo.PerformLayout();
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMessage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOpt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMainIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox pictureBoxMainIcon;
        private UnE.GUI.TextPictureBox pictureBoxView;
        private System.Windows.Forms.Panel panelViewRibbonBarRight;
        private System.Windows.Forms.Panel panelViewRibbonBarMiddle;
        private System.Windows.Forms.Panel panelViewRibbonBarLeft;
        private UnE.GUI.RibbonButton btnControl;
        private UnE.GUI.RibbonButton btnReturnControl;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panelRealMode;
        private System.Windows.Forms.RadioButton radioVirtualMode;
        private System.Windows.Forms.RadioButton radioRealMode;
        private UnE.GUI.RibbonButton btnCancelSOP;
        private UnE.GUI.RibbonButton btnStartSOP;
        private System.Windows.Forms.Panel panelNormalMode;
        private System.Windows.Forms.RadioButton radioHoliday;
        private System.Windows.Forms.RadioButton radioNormal;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox7;
        private UnE.GUI.RibbonButton btnFitToScale;
        private UnE.GUI.RibbonButton btnFitToCurrentComponent;
        private System.Windows.Forms.Button btnMin;
        private System.Windows.Forms.Button btnMax;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelHoliday;
        private System.Windows.Forms.Label labelNormal;
        private System.Windows.Forms.Label labelVirtual;
        private System.Windows.Forms.Label labelReal;
        private System.Windows.Forms.Panel panelStatus;
        private UnE.Utility.RealTimeInfoPane panelRealTimeInfo;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.PictureBox pictureBoxStatus;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Timer timer1;
        private UnE.GUI.TextPictureBox pictureBoxMessage;
        private UnE.GUI.RibbonButton btnMissionStatus;
        private UnE.GUI.RibbonButton btnBulletin;
        private UnE.GUI.RibbonButton btnSOP;
        private UnE.GUI.RibbonButton btnSDMS;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private UnE.GUI.RibbonButton btnDefaultCCTV;
        private UnE.GUI.TextPictureBox pictureBoxCCTV;
        private UnE.GUI.RibbonButton btnWork;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Panel panelMode;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelSelectMode;
        private UnE.GUI.RibbonButton btnOption;
        private System.Windows.Forms.PictureBox pictureBox8;
        private UnE.GUI.TextPictureBox pictureBoxOpt;
    }
}