namespace SDMS
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
            this.panelBottom = new System.Windows.Forms.Panel();
            this.DatePickerEnd = new AxXtremeCalendarControl.AxDatePicker();
            this.DatePickerStart = new AxXtremeCalendarControl.AxDatePicker();
            this.mClockTimer = new System.Windows.Forms.Timer(this.components);
            this.m_MainTimer = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.btnLayerLowCCTV = new System.Windows.Forms.Button();
            this.labelCCTVLow = new System.Windows.Forms.Label();
            this.labelFR = new System.Windows.Forms.Label();
            this.labelFA = new System.Windows.Forms.Label();
            this.labelHD = new System.Windows.Forms.Label();
            this.labelFE = new System.Windows.Forms.Label();
            this.labelCCTV = new System.Windows.Forms.Label();
            this.labelPump = new System.Windows.Forms.Label();
            this.labelCooler = new System.Windows.Forms.Label();
            this.labelFire = new System.Windows.Forms.Label();
            this.btnLayerFR = new System.Windows.Forms.Button();
            this.btnLayerFA = new System.Windows.Forms.Button();
            this.btnLayerHD = new System.Windows.Forms.Button();
            this.btnLayerFE = new System.Windows.Forms.Button();
            this.btnLayerCCTV = new System.Windows.Forms.Button();
            this.btnLayerPump = new System.Windows.Forms.Button();
            this.btnLayerSpringCooler = new System.Windows.Forms.Button();
            this.btnLayerFire = new System.Windows.Forms.Button();
            this.panelReactionHistory = new System.Windows.Forms.Panel();
            this.react_cboSearchType = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.btnSaveHWP = new System.Windows.Forms.Button();
            this.cboFireSelect = new System.Windows.Forms.ComboBox();
            this.react_btnEndDate = new System.Windows.Forms.Button();
            this.react_btnStartDate = new System.Windows.Forms.Button();
            this.react_cboEndTime = new System.Windows.Forms.ComboBox();
            this.react_cboStartTime = new System.Windows.Forms.ComboBox();
            this.lblFireSelect = new System.Windows.Forms.Label();
            this.lblReactionDate = new System.Windows.Forms.Label();
            this.panelProcessHistory = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.proc_btnEndDate = new System.Windows.Forms.Button();
            this.proc_btnStartDate = new System.Windows.Forms.Button();
            this.proc_cboLatelyDate = new System.Windows.Forms.ComboBox();
            this.lblProcessDate = new System.Windows.Forms.Label();
            this.proc_btnSelectZone = new System.Windows.Forms.Button();
            this.proc_cboFloor = new System.Windows.Forms.ComboBox();
            this.proc_cboBuilding = new System.Windows.Forms.ComboBox();
            this.proc_cboBuildingGroup = new System.Windows.Forms.ComboBox();
            this.proc_lblSelectZone = new System.Windows.Forms.Label();
            this.panelMiddle = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.btnSaveHome = new System.Windows.Forms.Button();
            this.labelSensorMonitor = new System.Windows.Forms.Label();
            this.btnSensorMonitor = new System.Windows.Forms.Button();
            this.btnShowCCTVList = new System.Windows.Forms.Button();
            this.cboEquipZone = new System.Windows.Forms.ComboBox();
            this.checkBoxEquipZoneCCTV = new System.Windows.Forms.CheckBox();
            this.labelFireDetect = new System.Windows.Forms.Label();
            this.cmbFireDetect = new System.Windows.Forms.ComboBox();
            this.btnSelectZone = new System.Windows.Forms.Button();
            this.cboFloor = new System.Windows.Forms.ComboBox();
            this.cboBuilding = new System.Windows.Forms.ComboBox();
            this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
            this.labelSelectZone = new System.Windows.Forms.Label();
            this.btnMultiCCTV = new System.Windows.Forms.Button();
            this.btnScreenShot = new System.Windows.Forms.Button();
            this.btnOutside = new System.Windows.Forms.Button();
            this.btnBoth = new System.Windows.Forms.Button();
            this.btnInside = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnOrbit = new System.Windows.Forms.Button();
            this.btnPanning = new System.Windows.Forms.Button();
            this.btnPick = new System.Windows.Forms.Button();
            this.btnFullScreen = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape4 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape3 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape2 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelReportRibbonBarMiddle = new System.Windows.Forms.Panel();
            this.btnSMSHistory = new SDMS.RibbonButton();
            this.btnReactionHistory = new SDMS.RibbonButton();
            this.btnProcessHistory = new SDMS.RibbonButton();
            this.btnDetectHistory = new SDMS.RibbonButton();
            this.panelAdminRibbonBarMiddle = new System.Windows.Forms.Panel();
            this.btnShowList = new SDMS.RibbonButton();
            this.btnDelete = new SDMS.RibbonButton();
            this.btnSave = new SDMS.RibbonButton();
            this.btnManageFacility = new SDMS.RibbonButton();
            this.btnManageBroadcast = new SDMS.RibbonButton();
            this.btnCreateCCTV = new SDMS.RibbonButton();
            this.btnManageManager = new SDMS.RibbonButton();
            this.btnCreatePump = new SDMS.RibbonButton();
            this.btnBackupDB = new SDMS.RibbonButton();
            this.btnManageDetect = new SDMS.RibbonButton();
            this.pictureBoxAdminRibbon3 = new System.Windows.Forms.PictureBox();
            this.btnManagePrint = new SDMS.RibbonButton();
            this.btnEditFacilityZone = new SDMS.RibbonButton();
            this.btnCreateSpringCooler = new SDMS.RibbonButton();
            this.pictureBoxAdminRibbon2 = new System.Windows.Forms.PictureBox();
            this.btnManageSMS = new SDMS.RibbonButton();
            this.btnCreateFire = new SDMS.RibbonButton();
            this.pictureBoxAdminRibbon1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxAdminRibbon4 = new System.Windows.Forms.PictureBox();
            this.panelReportRibbonBarLeft = new System.Windows.Forms.Panel();
            this.panelAdminRibbonBarLeft = new System.Windows.Forms.Panel();
            this.panelReportRibbonBarRight = new System.Windows.Forms.Panel();
            this.panelAdminRibbonBarRight = new System.Windows.Forms.Panel();
            this.pictureBoxReport = new SDMS.TextPictureBox();
            this.pictureBoxAdmin = new SDMS.TextPictureBox();
            this.pictureBoxMonitoring = new SDMS.TextPictureBox();
            this.btnMin = new System.Windows.Forms.Button();
            this.btnMax = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnFire = new SDMS.ButtonEx();
            this.panelLog = new SDMS.RealTimeInfoPane();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBoxLog = new System.Windows.Forms.PictureBox();
            this.mLabelLog = new System.Windows.Forms.Label();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.mLabelZone = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
            this.mLabelStatus = new System.Windows.Forms.Label();
            this.panelClock = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBoxClock = new System.Windows.Forms.PictureBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelDate = new System.Windows.Forms.Label();
            this.m_CheckReciver = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemBig = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DatePickerEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatePickerStart)).BeginInit();
            this.panelLeft.SuspendLayout();
            this.panelReactionHistory.SuspendLayout();
            this.panelProcessHistory.SuspendLayout();
            this.panelMiddle.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelReportRibbonBarMiddle.SuspendLayout();
            this.panelAdminRibbonBarMiddle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdminRibbon3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdminRibbon2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdminRibbon1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdminRibbon4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMonitoring)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLog)).BeginInit();
            this.panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
            this.panelClock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClock)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.DatePickerEnd);
            this.panelBottom.Controls.Add(this.DatePickerStart);
            this.panelBottom.Location = new System.Drawing.Point(46, 214);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1280, 1024);
            this.panelBottom.TabIndex = 1;
            // 
            // DatePickerEnd
            // 
            this.DatePickerEnd.Location = new System.Drawing.Point(172, 418);
            this.DatePickerEnd.Name = "DatePickerEnd";
            this.DatePickerEnd.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("DatePickerEnd.OcxState")));
            this.DatePickerEnd.Size = new System.Drawing.Size(24, 28);
            this.DatePickerEnd.TabIndex = 10;
            // 
            // DatePickerStart
            // 
            this.DatePickerStart.Location = new System.Drawing.Point(44, 420);
            this.DatePickerStart.Name = "DatePickerStart";
            this.DatePickerStart.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("DatePickerStart.OcxState")));
            this.DatePickerStart.Size = new System.Drawing.Size(25, 26);
            this.DatePickerStart.TabIndex = 9;
            // 
            // mClockTimer
            // 
            this.mClockTimer.Interval = 1000;
            this.mClockTimer.Tick += new System.EventHandler(this.OnTimer);
            // 
            // m_MainTimer
            // 
            this.m_MainTimer.Interval = 300;
            this.m_MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.Transparent;
            this.panelLeft.BackgroundImage = global::SDMS.Properties.Resources.VToolbar_bkgnd;
            this.panelLeft.Controls.Add(this.btnLayerLowCCTV);
            this.panelLeft.Controls.Add(this.labelCCTVLow);
            this.panelLeft.Controls.Add(this.labelFR);
            this.panelLeft.Controls.Add(this.labelFA);
            this.panelLeft.Controls.Add(this.labelHD);
            this.panelLeft.Controls.Add(this.labelFE);
            this.panelLeft.Controls.Add(this.labelCCTV);
            this.panelLeft.Controls.Add(this.labelPump);
            this.panelLeft.Controls.Add(this.labelCooler);
            this.panelLeft.Controls.Add(this.labelFire);
            this.panelLeft.Controls.Add(this.btnLayerFR);
            this.panelLeft.Controls.Add(this.btnLayerFA);
            this.panelLeft.Controls.Add(this.btnLayerHD);
            this.panelLeft.Controls.Add(this.btnLayerFE);
            this.panelLeft.Controls.Add(this.btnLayerCCTV);
            this.panelLeft.Controls.Add(this.btnLayerPump);
            this.panelLeft.Controls.Add(this.btnLayerSpringCooler);
            this.panelLeft.Controls.Add(this.btnLayerFire);
            this.panelLeft.Location = new System.Drawing.Point(473, 306);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(53, 598);
            this.panelLeft.TabIndex = 3;
            // 
            // btnLayerLowCCTV
            // 
            this.btnLayerLowCCTV.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Normal;
            this.btnLayerLowCCTV.Location = new System.Drawing.Point(2, 265);
            this.btnLayerLowCCTV.Name = "btnLayerLowCCTV";
            this.btnLayerLowCCTV.Size = new System.Drawing.Size(48, 48);
            this.btnLayerLowCCTV.TabIndex = 6;
            this.btnLayerLowCCTV.UseVisualStyleBackColor = true;
            this.btnLayerLowCCTV.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // labelCCTVLow
            // 
            this.labelCCTVLow.AutoSize = true;
            this.labelCCTVLow.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelCCTVLow.Location = new System.Drawing.Point(4, 316);
            this.labelCCTVLow.Name = "labelCCTVLow";
            this.labelCCTVLow.Size = new System.Drawing.Size(46, 13);
            this.labelCCTVLow.TabIndex = 5;
            this.labelCCTVLow.Text = "CCTV-X";
            // 
            // labelFR
            // 
            this.labelFR.AutoSize = true;
            this.labelFR.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelFR.Location = new System.Drawing.Point(4, 581);
            this.labelFR.Name = "labelFR";
            this.labelFR.Size = new System.Drawing.Size(40, 13);
            this.labelFR.TabIndex = 1;
            this.labelFR.Text = "수신기";
            this.labelFR.Visible = false;
            // 
            // labelFA
            // 
            this.labelFA.AutoSize = true;
            this.labelFA.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelFA.Location = new System.Drawing.Point(4, 516);
            this.labelFA.Name = "labelFA";
            this.labelFA.Size = new System.Drawing.Size(40, 13);
            this.labelFA.TabIndex = 1;
            this.labelFA.Text = "발신기";
            this.labelFA.Visible = false;
            // 
            // labelHD
            // 
            this.labelHD.AutoSize = true;
            this.labelHD.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelHD.Location = new System.Drawing.Point(4, 451);
            this.labelHD.Name = "labelHD";
            this.labelHD.Size = new System.Drawing.Size(40, 13);
            this.labelHD.TabIndex = 1;
            this.labelHD.Text = "소화전";
            // 
            // labelFE
            // 
            this.labelFE.AutoSize = true;
            this.labelFE.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelFE.Location = new System.Drawing.Point(5, 386);
            this.labelFE.Name = "labelFE";
            this.labelFE.Size = new System.Drawing.Size(40, 13);
            this.labelFE.TabIndex = 1;
            this.labelFE.Text = "소화기";
            // 
            // labelCCTV
            // 
            this.labelCCTV.AutoSize = true;
            this.labelCCTV.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelCCTV.Location = new System.Drawing.Point(7, 244);
            this.labelCCTV.Name = "labelCCTV";
            this.labelCCTV.Size = new System.Drawing.Size(34, 13);
            this.labelCCTV.TabIndex = 1;
            this.labelCCTV.Text = "CCTV";
            // 
            // labelPump
            // 
            this.labelPump.AutoSize = true;
            this.labelPump.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelPump.Location = new System.Drawing.Point(0, 179);
            this.labelPump.Name = "labelPump";
            this.labelPump.Size = new System.Drawing.Size(51, 13);
            this.labelPump.TabIndex = 1;
            this.labelPump.Text = "펌프압력";
            // 
            // labelCooler
            // 
            this.labelCooler.AutoSize = true;
            this.labelCooler.Font = new System.Drawing.Font("맑은 고딕", 7F);
            this.labelCooler.Location = new System.Drawing.Point(-3, 114);
            this.labelCooler.Name = "labelCooler";
            this.labelCooler.Size = new System.Drawing.Size(55, 12);
            this.labelCooler.TabIndex = 1;
            this.labelCooler.Text = "스프링쿨러";
            // 
            // labelFire
            // 
            this.labelFire.AutoSize = true;
            this.labelFire.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelFire.Location = new System.Drawing.Point(1, 49);
            this.labelFire.Name = "labelFire";
            this.labelFire.Size = new System.Drawing.Size(51, 13);
            this.labelFire.TabIndex = 1;
            this.labelFire.Text = "화재탐지";
            // 
            // btnLayerFR
            // 
            this.btnLayerFR.BackgroundImage = global::SDMS.Properties.Resources.Layer_FR_Normal;
            this.btnLayerFR.Location = new System.Drawing.Point(1, 532);
            this.btnLayerFR.Name = "btnLayerFR";
            this.btnLayerFR.Size = new System.Drawing.Size(48, 48);
            this.btnLayerFR.TabIndex = 0;
            this.btnLayerFR.UseVisualStyleBackColor = true;
            this.btnLayerFR.Visible = false;
            this.btnLayerFR.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerFA
            // 
            this.btnLayerFA.BackgroundImage = global::SDMS.Properties.Resources.Layer_FA_Normal;
            this.btnLayerFA.Location = new System.Drawing.Point(1, 467);
            this.btnLayerFA.Name = "btnLayerFA";
            this.btnLayerFA.Size = new System.Drawing.Size(48, 48);
            this.btnLayerFA.TabIndex = 0;
            this.btnLayerFA.UseVisualStyleBackColor = true;
            this.btnLayerFA.Visible = false;
            this.btnLayerFA.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerHD
            // 
            this.btnLayerHD.BackgroundImage = global::SDMS.Properties.Resources.Layer_HD_Normal;
            this.btnLayerHD.Location = new System.Drawing.Point(1, 402);
            this.btnLayerHD.Name = "btnLayerHD";
            this.btnLayerHD.Size = new System.Drawing.Size(48, 48);
            this.btnLayerHD.TabIndex = 0;
            this.btnLayerHD.UseVisualStyleBackColor = true;
            this.btnLayerHD.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerFE
            // 
            this.btnLayerFE.BackgroundImage = global::SDMS.Properties.Resources.Layer_FE_Normal;
            this.btnLayerFE.Location = new System.Drawing.Point(1, 337);
            this.btnLayerFE.Name = "btnLayerFE";
            this.btnLayerFE.Size = new System.Drawing.Size(48, 48);
            this.btnLayerFE.TabIndex = 0;
            this.btnLayerFE.UseVisualStyleBackColor = true;
            this.btnLayerFE.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerCCTV
            // 
            this.btnLayerCCTV.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Normal;
            this.btnLayerCCTV.Location = new System.Drawing.Point(1, 195);
            this.btnLayerCCTV.Name = "btnLayerCCTV";
            this.btnLayerCCTV.Size = new System.Drawing.Size(48, 48);
            this.btnLayerCCTV.TabIndex = 0;
            this.btnLayerCCTV.UseVisualStyleBackColor = true;
            this.btnLayerCCTV.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerPump
            // 
            this.btnLayerPump.BackgroundImage = global::SDMS.Properties.Resources.Layer_Pump_Normal;
            this.btnLayerPump.Location = new System.Drawing.Point(1, 130);
            this.btnLayerPump.Name = "btnLayerPump";
            this.btnLayerPump.Size = new System.Drawing.Size(48, 48);
            this.btnLayerPump.TabIndex = 0;
            this.btnLayerPump.UseVisualStyleBackColor = true;
            this.btnLayerPump.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerSpringCooler
            // 
            this.btnLayerSpringCooler.BackgroundImage = global::SDMS.Properties.Resources.Layer_SpringCooler_Normal;
            this.btnLayerSpringCooler.Location = new System.Drawing.Point(1, 65);
            this.btnLayerSpringCooler.Name = "btnLayerSpringCooler";
            this.btnLayerSpringCooler.Size = new System.Drawing.Size(48, 48);
            this.btnLayerSpringCooler.TabIndex = 0;
            this.btnLayerSpringCooler.UseVisualStyleBackColor = true;
            this.btnLayerSpringCooler.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerFire
            // 
            this.btnLayerFire.BackgroundImage = global::SDMS.Properties.Resources.Layer_Fire_Normal;
            this.btnLayerFire.Location = new System.Drawing.Point(1, 0);
            this.btnLayerFire.Name = "btnLayerFire";
            this.btnLayerFire.Size = new System.Drawing.Size(48, 48);
            this.btnLayerFire.TabIndex = 0;
            this.btnLayerFire.UseVisualStyleBackColor = true;
            this.btnLayerFire.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // panelReactionHistory
            // 
            this.panelReactionHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelReactionHistory.BackColor = System.Drawing.Color.Transparent;
            this.panelReactionHistory.BackgroundImage = global::SDMS.Properties.Resources.HToolbar_bkgnd;
            this.panelReactionHistory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelReactionHistory.Controls.Add(this.react_cboSearchType);
            this.panelReactionHistory.Controls.Add(this.label14);
            this.panelReactionHistory.Controls.Add(this.btnSaveHWP);
            this.panelReactionHistory.Controls.Add(this.cboFireSelect);
            this.panelReactionHistory.Controls.Add(this.react_btnEndDate);
            this.panelReactionHistory.Controls.Add(this.react_btnStartDate);
            this.panelReactionHistory.Controls.Add(this.react_cboEndTime);
            this.panelReactionHistory.Controls.Add(this.react_cboStartTime);
            this.panelReactionHistory.Controls.Add(this.lblFireSelect);
            this.panelReactionHistory.Controls.Add(this.lblReactionDate);
            this.panelReactionHistory.Location = new System.Drawing.Point(2, 597);
            this.panelReactionHistory.Name = "panelReactionHistory";
            this.panelReactionHistory.Size = new System.Drawing.Size(1386, 39);
            this.panelReactionHistory.TabIndex = 2;
            // 
            // react_cboSearchType
            // 
            this.react_cboSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.react_cboSearchType.FormattingEnabled = true;
            this.react_cboSearchType.Location = new System.Drawing.Point(546, 9);
            this.react_cboSearchType.Name = "react_cboSearchType";
            this.react_cboSearchType.Size = new System.Drawing.Size(180, 20);
            this.react_cboSearchType.TabIndex = 12;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label14.Location = new System.Drawing.Point(424, 13);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(19, 15);
            this.label14.TabIndex = 11;
            this.label14.Text = "~";
            // 
            // btnSaveHWP
            // 
            this.btnSaveHWP.Location = new System.Drawing.Point(1784, 8);
            this.btnSaveHWP.Name = "btnSaveHWP";
            this.btnSaveHWP.Size = new System.Drawing.Size(131, 23);
            this.btnSaveHWP.TabIndex = 10;
            this.btnSaveHWP.Text = "한글파일 저장";
            this.btnSaveHWP.UseVisualStyleBackColor = true;
            this.btnSaveHWP.Click += new System.EventHandler(this.button2_Click);
            // 
            // cboFireSelect
            // 
            this.cboFireSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFireSelect.FormattingEnabled = true;
            this.cboFireSelect.Location = new System.Drawing.Point(794, 9);
            this.cboFireSelect.MaxDropDownItems = 20;
            this.cboFireSelect.Name = "cboFireSelect";
            this.cboFireSelect.Size = new System.Drawing.Size(333, 20);
            this.cboFireSelect.TabIndex = 8;
            // 
            // react_btnEndDate
            // 
            this.react_btnEndDate.Location = new System.Drawing.Point(216, 7);
            this.react_btnEndDate.Name = "react_btnEndDate";
            this.react_btnEndDate.Size = new System.Drawing.Size(121, 23);
            this.react_btnEndDate.TabIndex = 9;
            this.react_btnEndDate.Text = "끝 일";
            this.react_btnEndDate.UseVisualStyleBackColor = true;
            this.react_btnEndDate.Click += new System.EventHandler(this.react_btnEndDate_Click);
            // 
            // react_btnStartDate
            // 
            this.react_btnStartDate.Location = new System.Drawing.Point(87, 7);
            this.react_btnStartDate.Name = "react_btnStartDate";
            this.react_btnStartDate.Size = new System.Drawing.Size(121, 23);
            this.react_btnStartDate.TabIndex = 9;
            this.react_btnStartDate.Text = "시작 일";
            this.react_btnStartDate.UseVisualStyleBackColor = true;
            this.react_btnStartDate.Click += new System.EventHandler(this.react_btnStartDate_Click);
            // 
            // react_cboEndTime
            // 
            this.react_cboEndTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.react_cboEndTime.FormattingEnabled = true;
            this.react_cboEndTime.Location = new System.Drawing.Point(447, 9);
            this.react_cboEndTime.Name = "react_cboEndTime";
            this.react_cboEndTime.Size = new System.Drawing.Size(77, 20);
            this.react_cboEndTime.TabIndex = 8;
            // 
            // react_cboStartTime
            // 
            this.react_cboStartTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.react_cboStartTime.FormattingEnabled = true;
            this.react_cboStartTime.Location = new System.Drawing.Point(343, 8);
            this.react_cboStartTime.Name = "react_cboStartTime";
            this.react_cboStartTime.Size = new System.Drawing.Size(77, 20);
            this.react_cboStartTime.TabIndex = 8;
            // 
            // lblFireSelect
            // 
            this.lblFireSelect.AutoSize = true;
            this.lblFireSelect.Location = new System.Drawing.Point(736, 13);
            this.lblFireSelect.Name = "lblFireSelect";
            this.lblFireSelect.Size = new System.Drawing.Size(53, 12);
            this.lblFireSelect.TabIndex = 5;
            this.lblFireSelect.Text = "화재선택";
            // 
            // lblReactionDate
            // 
            this.lblReactionDate.AutoSize = true;
            this.lblReactionDate.Location = new System.Drawing.Point(28, 13);
            this.lblReactionDate.Name = "lblReactionDate";
            this.lblReactionDate.Size = new System.Drawing.Size(53, 12);
            this.lblReactionDate.TabIndex = 5;
            this.lblReactionDate.Text = "기간선택";
            // 
            // panelProcessHistory
            // 
            this.panelProcessHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelProcessHistory.BackColor = System.Drawing.Color.Transparent;
            this.panelProcessHistory.BackgroundImage = global::SDMS.Properties.Resources.HToolbar_bkgnd;
            this.panelProcessHistory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelProcessHistory.Controls.Add(this.button1);
            this.panelProcessHistory.Controls.Add(this.proc_btnEndDate);
            this.panelProcessHistory.Controls.Add(this.proc_btnStartDate);
            this.panelProcessHistory.Controls.Add(this.proc_cboLatelyDate);
            this.panelProcessHistory.Controls.Add(this.lblProcessDate);
            this.panelProcessHistory.Controls.Add(this.proc_btnSelectZone);
            this.panelProcessHistory.Controls.Add(this.proc_cboFloor);
            this.panelProcessHistory.Controls.Add(this.proc_cboBuilding);
            this.panelProcessHistory.Controls.Add(this.proc_cboBuildingGroup);
            this.panelProcessHistory.Controls.Add(this.proc_lblSelectZone);
            this.panelProcessHistory.Location = new System.Drawing.Point(2, 532);
            this.panelProcessHistory.Name = "panelProcessHistory";
            this.panelProcessHistory.Size = new System.Drawing.Size(1386, 39);
            this.panelProcessHistory.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1784, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "한글파일 저장";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // proc_btnEndDate
            // 
            this.proc_btnEndDate.Location = new System.Drawing.Point(215, 7);
            this.proc_btnEndDate.Name = "proc_btnEndDate";
            this.proc_btnEndDate.Size = new System.Drawing.Size(122, 23);
            this.proc_btnEndDate.TabIndex = 9;
            this.proc_btnEndDate.Text = "끝 일";
            this.proc_btnEndDate.UseVisualStyleBackColor = true;
            this.proc_btnEndDate.Click += new System.EventHandler(this.proc_btnEndDate_Click);
            // 
            // proc_btnStartDate
            // 
            this.proc_btnStartDate.Location = new System.Drawing.Point(88, 7);
            this.proc_btnStartDate.Name = "proc_btnStartDate";
            this.proc_btnStartDate.Size = new System.Drawing.Size(121, 23);
            this.proc_btnStartDate.TabIndex = 9;
            this.proc_btnStartDate.Text = "시작 일";
            this.proc_btnStartDate.UseVisualStyleBackColor = true;
            this.proc_btnStartDate.Click += new System.EventHandler(this.proc_btnStartDate_Click);
            // 
            // proc_cboLatelyDate
            // 
            this.proc_cboLatelyDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proc_cboLatelyDate.FormattingEnabled = true;
            this.proc_cboLatelyDate.Location = new System.Drawing.Point(343, 9);
            this.proc_cboLatelyDate.Name = "proc_cboLatelyDate";
            this.proc_cboLatelyDate.Size = new System.Drawing.Size(121, 20);
            this.proc_cboLatelyDate.TabIndex = 8;
            this.proc_cboLatelyDate.SelectedIndexChanged += new System.EventHandler(this.proc_cboLatelyDate_SelectedIndexChanged);
            // 
            // lblProcessDate
            // 
            this.lblProcessDate.AutoSize = true;
            this.lblProcessDate.Location = new System.Drawing.Point(29, 13);
            this.lblProcessDate.Name = "lblProcessDate";
            this.lblProcessDate.Size = new System.Drawing.Size(53, 12);
            this.lblProcessDate.TabIndex = 5;
            this.lblProcessDate.Text = "기간선택";
            // 
            // proc_btnSelectZone
            // 
            this.proc_btnSelectZone.Location = new System.Drawing.Point(1054, 8);
            this.proc_btnSelectZone.Name = "proc_btnSelectZone";
            this.proc_btnSelectZone.Size = new System.Drawing.Size(38, 23);
            this.proc_btnSelectZone.TabIndex = 4;
            this.proc_btnSelectZone.Text = "선택";
            this.proc_btnSelectZone.UseVisualStyleBackColor = true;
            this.proc_btnSelectZone.Click += new System.EventHandler(this.proc_btnSelectZone_Click);
            // 
            // proc_cboFloor
            // 
            this.proc_cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proc_cboFloor.FormattingEnabled = true;
            this.proc_cboFloor.Location = new System.Drawing.Point(953, 10);
            this.proc_cboFloor.Name = "proc_cboFloor";
            this.proc_cboFloor.Size = new System.Drawing.Size(95, 20);
            this.proc_cboFloor.TabIndex = 3;
            // 
            // proc_cboBuilding
            // 
            this.proc_cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proc_cboBuilding.FormattingEnabled = true;
            this.proc_cboBuilding.Location = new System.Drawing.Point(699, 10);
            this.proc_cboBuilding.Name = "proc_cboBuilding";
            this.proc_cboBuilding.Size = new System.Drawing.Size(248, 20);
            this.proc_cboBuilding.TabIndex = 3;
            this.proc_cboBuilding.SelectedIndexChanged += new System.EventHandler(this.proc_cboBuilding_SelectedIndexChanged);
            // 
            // proc_cboBuildingGroup
            // 
            this.proc_cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proc_cboBuildingGroup.FormattingEnabled = true;
            this.proc_cboBuildingGroup.Location = new System.Drawing.Point(554, 10);
            this.proc_cboBuildingGroup.Name = "proc_cboBuildingGroup";
            this.proc_cboBuildingGroup.Size = new System.Drawing.Size(139, 20);
            this.proc_cboBuildingGroup.TabIndex = 3;
            this.proc_cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.Proc_cboBuildingGroup_SelectedIndexChanged);
            // 
            // proc_lblSelectZone
            // 
            this.proc_lblSelectZone.AutoSize = true;
            this.proc_lblSelectZone.Location = new System.Drawing.Point(495, 13);
            this.proc_lblSelectZone.Name = "proc_lblSelectZone";
            this.proc_lblSelectZone.Size = new System.Drawing.Size(53, 12);
            this.proc_lblSelectZone.TabIndex = 2;
            this.proc_lblSelectZone.Text = "위치선택";
            // 
            // panelMiddle
            // 
            this.panelMiddle.BackColor = System.Drawing.Color.Transparent;
            this.panelMiddle.BackgroundImage = global::SDMS.Properties.Resources.HToolbar_bkgnd;
            this.panelMiddle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelMiddle.Controls.Add(this.button3);
            this.panelMiddle.Controls.Add(this.btnSaveHome);
            this.panelMiddle.Controls.Add(this.labelSensorMonitor);
            this.panelMiddle.Controls.Add(this.btnSensorMonitor);
            this.panelMiddle.Controls.Add(this.btnShowCCTVList);
            this.panelMiddle.Controls.Add(this.cboEquipZone);
            this.panelMiddle.Controls.Add(this.checkBoxEquipZoneCCTV);
            this.panelMiddle.Controls.Add(this.labelFireDetect);
            this.panelMiddle.Controls.Add(this.cmbFireDetect);
            this.panelMiddle.Controls.Add(this.btnSelectZone);
            this.panelMiddle.Controls.Add(this.cboFloor);
            this.panelMiddle.Controls.Add(this.cboBuilding);
            this.panelMiddle.Controls.Add(this.cboBuildingGroup);
            this.panelMiddle.Controls.Add(this.labelSelectZone);
            this.panelMiddle.Controls.Add(this.btnMultiCCTV);
            this.panelMiddle.Controls.Add(this.btnScreenShot);
            this.panelMiddle.Controls.Add(this.btnOutside);
            this.panelMiddle.Controls.Add(this.btnBoth);
            this.panelMiddle.Controls.Add(this.btnInside);
            this.panelMiddle.Controls.Add(this.btnZoomOut);
            this.panelMiddle.Controls.Add(this.btnZoomIn);
            this.panelMiddle.Controls.Add(this.btnOrbit);
            this.panelMiddle.Controls.Add(this.btnPanning);
            this.panelMiddle.Controls.Add(this.btnPick);
            this.panelMiddle.Controls.Add(this.btnFullScreen);
            this.panelMiddle.Controls.Add(this.btnHome);
            this.panelMiddle.Controls.Add(this.shapeContainer1);
            this.panelMiddle.Location = new System.Drawing.Point(0, 458);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Size = new System.Drawing.Size(1920, 39);
            this.panelMiddle.TabIndex = 2;
            this.panelMiddle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMiddle_MouseDown);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(627, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 13;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnSaveHome
            // 
            this.btnSaveHome.BackgroundImage = global::SDMS.Properties.Resources.Show_List_Checked;
            this.btnSaveHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSaveHome.Location = new System.Drawing.Point(12, 4);
            this.btnSaveHome.Name = "btnSaveHome";
            this.btnSaveHome.Size = new System.Drawing.Size(32, 32);
            this.btnSaveHome.TabIndex = 12;
            this.btnSaveHome.UseVisualStyleBackColor = true;
            this.btnSaveHome.Click += new System.EventHandler(this.btnSaveHome_Click);
            // 
            // labelSensorMonitor
            // 
            this.labelSensorMonitor.AutoSize = true;
            this.labelSensorMonitor.Location = new System.Drawing.Point(823, 14);
            this.labelSensorMonitor.Name = "labelSensorMonitor";
            this.labelSensorMonitor.Size = new System.Drawing.Size(41, 12);
            this.labelSensorMonitor.TabIndex = 11;
            this.labelSensorMonitor.Text = "수신반";
            // 
            // btnSensorMonitor
            // 
            this.btnSensorMonitor.Location = new System.Drawing.Point(756, 8);
            this.btnSensorMonitor.Name = "btnSensorMonitor";
            this.btnSensorMonitor.Size = new System.Drawing.Size(61, 23);
            this.btnSensorMonitor.TabIndex = 10;
            this.btnSensorMonitor.Text = "상세보기";
            this.btnSensorMonitor.UseVisualStyleBackColor = true;
            this.btnSensorMonitor.Click += new System.EventHandler(this.btnSensorMonitor_Click);
            // 
            // btnShowCCTVList
            // 
            this.btnShowCCTVList.Location = new System.Drawing.Point(638, 7);
            this.btnShowCCTVList.Name = "btnShowCCTVList";
            this.btnShowCCTVList.Size = new System.Drawing.Size(112, 23);
            this.btnShowCCTVList.TabIndex = 9;
            this.btnShowCCTVList.Text = "CCTV List 보기";
            this.btnShowCCTVList.UseVisualStyleBackColor = true;
            this.btnShowCCTVList.Visible = false;
            this.btnShowCCTVList.Click += new System.EventHandler(this.btnShowCCTVList_Click);
            // 
            // cboEquipZone
            // 
            this.cboEquipZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEquipZone.FormattingEnabled = true;
            this.cboEquipZone.Location = new System.Drawing.Point(1297, 10);
            this.cboEquipZone.Name = "cboEquipZone";
            this.cboEquipZone.Size = new System.Drawing.Size(42, 20);
            this.cboEquipZone.TabIndex = 8;
            this.cboEquipZone.Visible = false;
            this.cboEquipZone.SelectedIndexChanged += new System.EventHandler(this.cboEquipZone_SelectedIndexChanged);
            // 
            // checkBoxEquipZoneCCTV
            // 
            this.checkBoxEquipZoneCCTV.AutoSize = true;
            this.checkBoxEquipZoneCCTV.Location = new System.Drawing.Point(448, 11);
            this.checkBoxEquipZoneCCTV.Name = "checkBoxEquipZoneCCTV";
            this.checkBoxEquipZoneCCTV.Size = new System.Drawing.Size(150, 16);
            this.checkBoxEquipZoneCCTV.TabIndex = 7;
            this.checkBoxEquipZoneCCTV.Text = "영역별 CCTV 설정하기";
            this.checkBoxEquipZoneCCTV.UseVisualStyleBackColor = true;
            this.checkBoxEquipZoneCCTV.Visible = false;
            this.checkBoxEquipZoneCCTV.CheckedChanged += new System.EventHandler(this.checkBoxEquipZoneCCTV_CheckedChanged);
            // 
            // labelFireDetect
            // 
            this.labelFireDetect.AutoSize = true;
            this.labelFireDetect.Location = new System.Drawing.Point(867, 12);
            this.labelFireDetect.Name = "labelFireDetect";
            this.labelFireDetect.Size = new System.Drawing.Size(57, 12);
            this.labelFireDetect.TabIndex = 6;
            this.labelFireDetect.Text = "화재 발생";
            // 
            // cmbFireDetect
            // 
            this.cmbFireDetect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFireDetect.FormattingEnabled = true;
            this.cmbFireDetect.Location = new System.Drawing.Point(933, 8);
            this.cmbFireDetect.MaxDropDownItems = 30;
            this.cmbFireDetect.Name = "cmbFireDetect";
            this.cmbFireDetect.Size = new System.Drawing.Size(333, 20);
            this.cmbFireDetect.TabIndex = 5;
            this.cmbFireDetect.SelectedIndexChanged += new System.EventHandler(this.cmbFireDetect_SelectedIndexChanged);
            this.cmbFireDetect.SelectionChangeCommitted += new System.EventHandler(this.cmbFireDetect_SelectionChangeCommitted);
            // 
            // btnSelectZone
            // 
            this.btnSelectZone.Location = new System.Drawing.Point(1873, 8);
            this.btnSelectZone.Name = "btnSelectZone";
            this.btnSelectZone.Size = new System.Drawing.Size(38, 23);
            this.btnSelectZone.TabIndex = 4;
            this.btnSelectZone.Text = "선택";
            this.btnSelectZone.UseVisualStyleBackColor = true;
            this.btnSelectZone.Click += new System.EventHandler(this.btnSelectZone_Click);
            // 
            // cboFloor
            // 
            this.cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloor.FormattingEnabled = true;
            this.cboFloor.Location = new System.Drawing.Point(1811, 10);
            this.cboFloor.Name = "cboFloor";
            this.cboFloor.Size = new System.Drawing.Size(56, 20);
            this.cboFloor.TabIndex = 3;
            this.cboFloor.SelectedIndexChanged += new System.EventHandler(this.cboFloor_SelectedIndexChanged);
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.Location = new System.Drawing.Point(1557, 10);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Size = new System.Drawing.Size(248, 20);
            this.cboBuilding.TabIndex = 3;
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.Location = new System.Drawing.Point(1412, 10);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Size = new System.Drawing.Size(139, 20);
            this.cboBuildingGroup.TabIndex = 3;
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // labelSelectZone
            // 
            this.labelSelectZone.AutoSize = true;
            this.labelSelectZone.Location = new System.Drawing.Point(1353, 13);
            this.labelSelectZone.Name = "labelSelectZone";
            this.labelSelectZone.Size = new System.Drawing.Size(53, 12);
            this.labelSelectZone.TabIndex = 2;
            this.labelSelectZone.Text = "위치선택";
            // 
            // btnMultiCCTV
            // 
            this.btnMultiCCTV.BackgroundImage = global::SDMS.Properties.Resources.CCTV_Normal;
            this.btnMultiCCTV.Location = new System.Drawing.Point(372, 3);
            this.btnMultiCCTV.Name = "btnMultiCCTV";
            this.btnMultiCCTV.Size = new System.Drawing.Size(32, 32);
            this.btnMultiCCTV.TabIndex = 0;
            this.btnMultiCCTV.UseVisualStyleBackColor = true;
            this.btnMultiCCTV.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnScreenShot
            // 
            this.btnScreenShot.BackgroundImage = global::SDMS.Properties.Resources.ScreenShot_Normal;
            this.btnScreenShot.Location = new System.Drawing.Point(403, 3);
            this.btnScreenShot.Name = "btnScreenShot";
            this.btnScreenShot.Size = new System.Drawing.Size(32, 32);
            this.btnScreenShot.TabIndex = 0;
            this.btnScreenShot.UseVisualStyleBackColor = true;
            this.btnScreenShot.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnOutside
            // 
            this.btnOutside.BackgroundImage = global::SDMS.Properties.Resources.Outside_Normal;
            this.btnOutside.Location = new System.Drawing.Point(275, 3);
            this.btnOutside.Name = "btnOutside";
            this.btnOutside.Size = new System.Drawing.Size(32, 32);
            this.btnOutside.TabIndex = 0;
            this.btnOutside.UseVisualStyleBackColor = true;
            this.btnOutside.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnBoth
            // 
            this.btnBoth.BackgroundImage = global::SDMS.Properties.Resources.Both_Normal;
            this.btnBoth.Location = new System.Drawing.Point(306, 3);
            this.btnBoth.Name = "btnBoth";
            this.btnBoth.Size = new System.Drawing.Size(32, 32);
            this.btnBoth.TabIndex = 0;
            this.btnBoth.UseVisualStyleBackColor = true;
            this.btnBoth.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnInside
            // 
            this.btnInside.BackgroundImage = global::SDMS.Properties.Resources.Inside_Normal;
            this.btnInside.Location = new System.Drawing.Point(337, 3);
            this.btnInside.Name = "btnInside";
            this.btnInside.Size = new System.Drawing.Size(32, 32);
            this.btnInside.TabIndex = 0;
            this.btnInside.UseVisualStyleBackColor = true;
            this.btnInside.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.BackgroundImage = global::SDMS.Properties.Resources.ZoomOut_Normal;
            this.btnZoomOut.Location = new System.Drawing.Point(240, 3);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(32, 32);
            this.btnZoomOut.TabIndex = 0;
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.BackgroundImage = global::SDMS.Properties.Resources.ZoomIn_Normal;
            this.btnZoomIn.Location = new System.Drawing.Point(209, 3);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(32, 32);
            this.btnZoomIn.TabIndex = 0;
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnOrbit
            // 
            this.btnOrbit.BackgroundImage = global::SDMS.Properties.Resources.Orbit_Normal;
            this.btnOrbit.Location = new System.Drawing.Point(174, 3);
            this.btnOrbit.Name = "btnOrbit";
            this.btnOrbit.Size = new System.Drawing.Size(32, 32);
            this.btnOrbit.TabIndex = 0;
            this.btnOrbit.UseVisualStyleBackColor = true;
            this.btnOrbit.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnPanning
            // 
            this.btnPanning.BackgroundImage = global::SDMS.Properties.Resources.Panning_Normal;
            this.btnPanning.Location = new System.Drawing.Point(143, 3);
            this.btnPanning.Name = "btnPanning";
            this.btnPanning.Size = new System.Drawing.Size(32, 32);
            this.btnPanning.TabIndex = 0;
            this.btnPanning.UseVisualStyleBackColor = true;
            this.btnPanning.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnPick
            // 
            this.btnPick.BackgroundImage = global::SDMS.Properties.Resources.Pick_Normal;
            this.btnPick.Location = new System.Drawing.Point(112, 3);
            this.btnPick.Name = "btnPick";
            this.btnPick.Size = new System.Drawing.Size(32, 32);
            this.btnPick.TabIndex = 0;
            this.btnPick.UseVisualStyleBackColor = true;
            this.btnPick.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnFullScreen
            // 
            this.btnFullScreen.BackgroundImage = global::SDMS.Properties.Resources.FullScreen_Normal;
            this.btnFullScreen.Location = new System.Drawing.Point(77, 3);
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.Size = new System.Drawing.Size(32, 32);
            this.btnFullScreen.TabIndex = 0;
            this.btnFullScreen.UseVisualStyleBackColor = true;
            this.btnFullScreen.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnHome
            // 
            this.btnHome.BackgroundImage = global::SDMS.Properties.Resources.Home_Normal;
            this.btnHome.Location = new System.Drawing.Point(46, 3);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(32, 32);
            this.btnHome.TabIndex = 0;
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape4,
            this.lineShape3,
            this.lineShape2,
            this.lineShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(1920, 39);
            this.shapeContainer1.TabIndex = 1;
            this.shapeContainer1.TabStop = false;
            // 
            // lineShape4
            // 
            this.lineShape4.Name = "lineShape4";
            this.lineShape4.X1 = 370;
            this.lineShape4.X2 = 370;
            this.lineShape4.Y1 = 4;
            this.lineShape4.Y2 = 32;
            // 
            // lineShape3
            // 
            this.lineShape3.Name = "lineShape3";
            this.lineShape3.X1 = 273;
            this.lineShape3.X2 = 273;
            this.lineShape3.Y1 = 4;
            this.lineShape3.Y2 = 32;
            // 
            // lineShape2
            // 
            this.lineShape2.Name = "lineShape2";
            this.lineShape2.X1 = 207;
            this.lineShape2.X2 = 207;
            this.lineShape2.Y1 = 4;
            this.lineShape2.Y2 = 32;
            // 
            // lineShape1
            // 
            this.lineShape1.Name = "lineShape1";
            this.lineShape1.X1 = 110;
            this.lineShape1.X2 = 110;
            this.lineShape1.Y1 = 4;
            this.lineShape1.Y2 = 32;
            // 
            // panelTop
            // 
            this.panelTop.BackgroundImage = global::SDMS.Properties.Resources.ToolbarBkgnd;
            this.panelTop.Controls.Add(this.panelReportRibbonBarMiddle);
            this.panelTop.Controls.Add(this.panelAdminRibbonBarMiddle);
            this.panelTop.Controls.Add(this.panelReportRibbonBarLeft);
            this.panelTop.Controls.Add(this.panelAdminRibbonBarLeft);
            this.panelTop.Controls.Add(this.panelReportRibbonBarRight);
            this.panelTop.Controls.Add(this.panelAdminRibbonBarRight);
            this.panelTop.Controls.Add(this.pictureBoxReport);
            this.panelTop.Controls.Add(this.pictureBoxAdmin);
            this.panelTop.Controls.Add(this.pictureBoxMonitoring);
            this.panelTop.Controls.Add(this.btnMin);
            this.panelTop.Controls.Add(this.btnMax);
            this.panelTop.Controls.Add(this.btnClose);
            this.panelTop.Controls.Add(this.pictureBox1);
            this.panelTop.Controls.Add(this.btnFire);
            this.panelTop.Controls.Add(this.panelLog);
            this.panelTop.Controls.Add(this.panelStatus);
            this.panelTop.Controls.Add(this.panelClock);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1386, 441);
            this.panelTop.TabIndex = 0;
            this.panelTop.DoubleClick += new System.EventHandler(this.panelTop_DoubleClick);
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            this.panelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseUp);
            // 
            // panelReportRibbonBarMiddle
            // 
            this.panelReportRibbonBarMiddle.BackColor = System.Drawing.Color.Transparent;
            this.panelReportRibbonBarMiddle.BackgroundImage = global::SDMS.Properties.Resources.RibbonBar_Middle;
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnSMSHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnReactionHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnProcessHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnDetectHistory);
            this.panelReportRibbonBarMiddle.Location = new System.Drawing.Point(140, 314);
            this.panelReportRibbonBarMiddle.Name = "panelReportRibbonBarMiddle";
            this.panelReportRibbonBarMiddle.Size = new System.Drawing.Size(1517, 87);
            this.panelReportRibbonBarMiddle.TabIndex = 9;
            // 
            // btnSMSHistory
            // 
            this.btnSMSHistory.CheckedBkgndImage = null;
            this.btnSMSHistory.CheckedImage = null;
            this.btnSMSHistory.IsChecked = false;
            this.btnSMSHistory.Location = new System.Drawing.Point(158, 1);
            this.btnSMSHistory.MouseOverBkgndImage = null;
            this.btnSMSHistory.Name = "btnSMSHistory";
            this.btnSMSHistory.NormalImage = null;
            this.btnSMSHistory.Owner = null;
            this.btnSMSHistory.Size = new System.Drawing.Size(60, 85);
            this.btnSMSHistory.TabIndex = 1;
            this.btnSMSHistory.Title = "문자이력";
            this.btnSMSHistory.UseVisualStyleBackColor = true;
            // 
            // btnReactionHistory
            // 
            this.btnReactionHistory.CheckedBkgndImage = null;
            this.btnReactionHistory.CheckedImage = null;
            this.btnReactionHistory.IsChecked = false;
            this.btnReactionHistory.Location = new System.Drawing.Point(97, 1);
            this.btnReactionHistory.MouseOverBkgndImage = null;
            this.btnReactionHistory.Name = "btnReactionHistory";
            this.btnReactionHistory.NormalImage = null;
            this.btnReactionHistory.Owner = null;
            this.btnReactionHistory.Size = new System.Drawing.Size(60, 85);
            this.btnReactionHistory.TabIndex = 0;
            this.btnReactionHistory.Title = "대응이력";
            this.btnReactionHistory.UseVisualStyleBackColor = true;
            // 
            // btnProcessHistory
            // 
            this.btnProcessHistory.CheckedBkgndImage = null;
            this.btnProcessHistory.CheckedImage = null;
            this.btnProcessHistory.IsChecked = false;
            this.btnProcessHistory.Location = new System.Drawing.Point(49, 1);
            this.btnProcessHistory.MouseOverBkgndImage = null;
            this.btnProcessHistory.Name = "btnProcessHistory";
            this.btnProcessHistory.NormalImage = null;
            this.btnProcessHistory.Owner = null;
            this.btnProcessHistory.Size = new System.Drawing.Size(60, 85);
            this.btnProcessHistory.TabIndex = 0;
            this.btnProcessHistory.Title = "처리이력";
            this.btnProcessHistory.UseVisualStyleBackColor = true;
            // 
            // btnDetectHistory
            // 
            this.btnDetectHistory.CheckedBkgndImage = null;
            this.btnDetectHistory.CheckedImage = null;
            this.btnDetectHistory.IsChecked = false;
            this.btnDetectHistory.Location = new System.Drawing.Point(1, 1);
            this.btnDetectHistory.MouseOverBkgndImage = null;
            this.btnDetectHistory.Name = "btnDetectHistory";
            this.btnDetectHistory.NormalImage = null;
            this.btnDetectHistory.Owner = null;
            this.btnDetectHistory.Size = new System.Drawing.Size(60, 85);
            this.btnDetectHistory.TabIndex = 0;
            this.btnDetectHistory.Title = "탐지이력";
            this.btnDetectHistory.UseVisualStyleBackColor = true;
            // 
            // panelAdminRibbonBarMiddle
            // 
            this.panelAdminRibbonBarMiddle.BackColor = System.Drawing.Color.Transparent;
            this.panelAdminRibbonBarMiddle.BackgroundImage = global::SDMS.Properties.Resources.RibbonBar_Middle;
            this.panelAdminRibbonBarMiddle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnShowList);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnDelete);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnSave);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnManageFacility);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnManageBroadcast);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnCreateCCTV);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnManageManager);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnCreatePump);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnBackupDB);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnManageDetect);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.pictureBoxAdminRibbon3);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnManagePrint);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnEditFacilityZone);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnCreateSpringCooler);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.pictureBoxAdminRibbon2);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnManageSMS);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnCreateFire);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.pictureBoxAdminRibbon1);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.pictureBoxAdminRibbon4);
            this.panelAdminRibbonBarMiddle.Location = new System.Drawing.Point(142, 198);
            this.panelAdminRibbonBarMiddle.Name = "panelAdminRibbonBarMiddle";
            this.panelAdminRibbonBarMiddle.Size = new System.Drawing.Size(1517, 87);
            this.panelAdminRibbonBarMiddle.TabIndex = 9;
            // 
            // btnShowList
            // 
            this.btnShowList.CheckedBkgndImage = null;
            this.btnShowList.CheckedImage = null;
            this.btnShowList.IsChecked = false;
            this.btnShowList.Location = new System.Drawing.Point(297, 1);
            this.btnShowList.MouseOverBkgndImage = null;
            this.btnShowList.Name = "btnShowList";
            this.btnShowList.NormalImage = null;
            this.btnShowList.Owner = null;
            this.btnShowList.Size = new System.Drawing.Size(68, 85);
            this.btnShowList.TabIndex = 0;
            this.btnShowList.Title = "리스트보기";
            this.btnShowList.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.CheckedBkgndImage = null;
            this.btnDelete.CheckedImage = null;
            this.btnDelete.IsChecked = false;
            this.btnDelete.Location = new System.Drawing.Point(242, 1);
            this.btnDelete.MouseOverBkgndImage = null;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.NormalImage = null;
            this.btnDelete.Owner = null;
            this.btnDelete.Size = new System.Drawing.Size(60, 85);
            this.btnDelete.TabIndex = 0;
            this.btnDelete.Title = "삭제";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.CheckedBkgndImage = null;
            this.btnSave.CheckedImage = null;
            this.btnSave.IsChecked = false;
            this.btnSave.Location = new System.Drawing.Point(687, 1);
            this.btnSave.MouseOverBkgndImage = null;
            this.btnSave.Name = "btnSave";
            this.btnSave.NormalImage = null;
            this.btnSave.Owner = null;
            this.btnSave.Size = new System.Drawing.Size(60, 85);
            this.btnSave.TabIndex = 0;
            this.btnSave.Title = "저장";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnManageFacility
            // 
            this.btnManageFacility.CheckedBkgndImage = null;
            this.btnManageFacility.CheckedImage = null;
            this.btnManageFacility.IsChecked = false;
            this.btnManageFacility.Location = new System.Drawing.Point(544, 1);
            this.btnManageFacility.MouseOverBkgndImage = null;
            this.btnManageFacility.Name = "btnManageFacility";
            this.btnManageFacility.NormalImage = null;
            this.btnManageFacility.Owner = null;
            this.btnManageFacility.Size = new System.Drawing.Size(60, 85);
            this.btnManageFacility.TabIndex = 0;
            this.btnManageFacility.Title = "장비현황";
            this.btnManageFacility.UseVisualStyleBackColor = true;
            // 
            // btnManageBroadcast
            // 
            this.btnManageBroadcast.CheckedBkgndImage = null;
            this.btnManageBroadcast.CheckedImage = null;
            this.btnManageBroadcast.IsChecked = false;
            this.btnManageBroadcast.Location = new System.Drawing.Point(448, 1);
            this.btnManageBroadcast.MouseOverBkgndImage = null;
            this.btnManageBroadcast.Name = "btnManageBroadcast";
            this.btnManageBroadcast.NormalImage = null;
            this.btnManageBroadcast.Owner = null;
            this.btnManageBroadcast.Size = new System.Drawing.Size(60, 85);
            this.btnManageBroadcast.TabIndex = 0;
            this.btnManageBroadcast.Title = "방송관리";
            this.btnManageBroadcast.UseVisualStyleBackColor = true;
            // 
            // btnCreateCCTV
            // 
            this.btnCreateCCTV.CheckedBkgndImage = null;
            this.btnCreateCCTV.CheckedImage = null;
            this.btnCreateCCTV.IsChecked = false;
            this.btnCreateCCTV.Location = new System.Drawing.Point(187, 1);
            this.btnCreateCCTV.MouseOverBkgndImage = null;
            this.btnCreateCCTV.Name = "btnCreateCCTV";
            this.btnCreateCCTV.NormalImage = null;
            this.btnCreateCCTV.Owner = null;
            this.btnCreateCCTV.Size = new System.Drawing.Size(60, 85);
            this.btnCreateCCTV.TabIndex = 0;
            this.btnCreateCCTV.Title = "CCTV";
            this.btnCreateCCTV.UseVisualStyleBackColor = true;
            // 
            // btnManageManager
            // 
            this.btnManageManager.CheckedBkgndImage = null;
            this.btnManageManager.CheckedImage = null;
            this.btnManageManager.IsChecked = false;
            this.btnManageManager.Location = new System.Drawing.Point(352, 1);
            this.btnManageManager.MouseOverBkgndImage = null;
            this.btnManageManager.Name = "btnManageManager";
            this.btnManageManager.NormalImage = null;
            this.btnManageManager.Owner = null;
            this.btnManageManager.Size = new System.Drawing.Size(68, 85);
            this.btnManageManager.TabIndex = 0;
            this.btnManageManager.Title = "담당자관리";
            this.btnManageManager.UseVisualStyleBackColor = true;
            // 
            // btnCreatePump
            // 
            this.btnCreatePump.CheckedBkgndImage = null;
            this.btnCreatePump.CheckedImage = null;
            this.btnCreatePump.IsChecked = false;
            this.btnCreatePump.Location = new System.Drawing.Point(139, 1);
            this.btnCreatePump.MouseOverBkgndImage = null;
            this.btnCreatePump.Name = "btnCreatePump";
            this.btnCreatePump.NormalImage = null;
            this.btnCreatePump.Owner = null;
            this.btnCreatePump.Size = new System.Drawing.Size(60, 85);
            this.btnCreatePump.TabIndex = 0;
            this.btnCreatePump.Title = "펌프압력";
            this.btnCreatePump.UseVisualStyleBackColor = true;
            // 
            // btnBackupDB
            // 
            this.btnBackupDB.CheckedBkgndImage = null;
            this.btnBackupDB.CheckedImage = null;
            this.btnBackupDB.IsChecked = false;
            this.btnBackupDB.Location = new System.Drawing.Point(643, 1);
            this.btnBackupDB.MouseOverBkgndImage = null;
            this.btnBackupDB.Name = "btnBackupDB";
            this.btnBackupDB.NormalImage = null;
            this.btnBackupDB.Owner = null;
            this.btnBackupDB.Size = new System.Drawing.Size(66, 85);
            this.btnBackupDB.TabIndex = 0;
            this.btnBackupDB.Title = "백업//복원";
            this.btnBackupDB.UseVisualStyleBackColor = true;
            // 
            // btnManageDetect
            // 
            this.btnManageDetect.CheckedBkgndImage = null;
            this.btnManageDetect.CheckedImage = null;
            this.btnManageDetect.IsChecked = false;
            this.btnManageDetect.Location = new System.Drawing.Point(592, 1);
            this.btnManageDetect.MouseOverBkgndImage = null;
            this.btnManageDetect.Name = "btnManageDetect";
            this.btnManageDetect.NormalImage = null;
            this.btnManageDetect.Owner = null;
            this.btnManageDetect.Size = new System.Drawing.Size(60, 85);
            this.btnManageDetect.TabIndex = 0;
            this.btnManageDetect.Title = "탐지관리";
            this.btnManageDetect.UseVisualStyleBackColor = true;
            // 
            // pictureBoxAdminRibbon3
            // 
            this.pictureBoxAdminRibbon3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxAdminRibbon3.BackgroundImage = global::SDMS.Properties.Resources.Separator;
            this.pictureBoxAdminRibbon3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxAdminRibbon3.Location = new System.Drawing.Point(300, 3);
            this.pictureBoxAdminRibbon3.Name = "pictureBoxAdminRibbon3";
            this.pictureBoxAdminRibbon3.Size = new System.Drawing.Size(13, 85);
            this.pictureBoxAdminRibbon3.TabIndex = 6;
            this.pictureBoxAdminRibbon3.TabStop = false;
            // 
            // btnManagePrint
            // 
            this.btnManagePrint.CheckedBkgndImage = null;
            this.btnManagePrint.CheckedImage = null;
            this.btnManagePrint.IsChecked = false;
            this.btnManagePrint.Location = new System.Drawing.Point(496, 1);
            this.btnManagePrint.MouseOverBkgndImage = null;
            this.btnManagePrint.Name = "btnManagePrint";
            this.btnManagePrint.NormalImage = null;
            this.btnManagePrint.Owner = null;
            this.btnManagePrint.Size = new System.Drawing.Size(60, 85);
            this.btnManagePrint.TabIndex = 0;
            this.btnManagePrint.Title = "도면관리";
            this.btnManagePrint.UseVisualStyleBackColor = true;
            // 
            // btnEditFacilityZone
            // 
            this.btnEditFacilityZone.CheckedBkgndImage = null;
            this.btnEditFacilityZone.CheckedImage = null;
            this.btnEditFacilityZone.IsChecked = false;
            this.btnEditFacilityZone.Location = new System.Drawing.Point(76, 1);
            this.btnEditFacilityZone.MouseOverBkgndImage = null;
            this.btnEditFacilityZone.Name = "btnEditFacilityZone";
            this.btnEditFacilityZone.NormalImage = null;
            this.btnEditFacilityZone.Owner = null;
            this.btnEditFacilityZone.Size = new System.Drawing.Size(60, 85);
            this.btnEditFacilityZone.TabIndex = 0;
            this.btnEditFacilityZone.Title = "설비영역";
            this.btnEditFacilityZone.UseVisualStyleBackColor = true;
            // 
            // btnCreateSpringCooler
            // 
            this.btnCreateSpringCooler.CheckedBkgndImage = null;
            this.btnCreateSpringCooler.CheckedImage = null;
            this.btnCreateSpringCooler.IsChecked = false;
            this.btnCreateSpringCooler.Location = new System.Drawing.Point(49, 1);
            this.btnCreateSpringCooler.MouseOverBkgndImage = null;
            this.btnCreateSpringCooler.Name = "btnCreateSpringCooler";
            this.btnCreateSpringCooler.NormalImage = null;
            this.btnCreateSpringCooler.Owner = null;
            this.btnCreateSpringCooler.Size = new System.Drawing.Size(68, 85);
            this.btnCreateSpringCooler.TabIndex = 0;
            this.btnCreateSpringCooler.Title = "스프링쿨러";
            this.btnCreateSpringCooler.UseVisualStyleBackColor = true;
            // 
            // pictureBoxAdminRibbon2
            // 
            this.pictureBoxAdminRibbon2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxAdminRibbon2.BackgroundImage = global::SDMS.Properties.Resources.Separator;
            this.pictureBoxAdminRibbon2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxAdminRibbon2.Location = new System.Drawing.Point(245, 3);
            this.pictureBoxAdminRibbon2.Name = "pictureBoxAdminRibbon2";
            this.pictureBoxAdminRibbon2.Size = new System.Drawing.Size(13, 85);
            this.pictureBoxAdminRibbon2.TabIndex = 6;
            this.pictureBoxAdminRibbon2.TabStop = false;
            // 
            // btnManageSMS
            // 
            this.btnManageSMS.CheckedBkgndImage = null;
            this.btnManageSMS.CheckedImage = null;
            this.btnManageSMS.IsChecked = false;
            this.btnManageSMS.Location = new System.Drawing.Point(358, 1);
            this.btnManageSMS.MouseOverBkgndImage = null;
            this.btnManageSMS.Name = "btnManageSMS";
            this.btnManageSMS.NormalImage = null;
            this.btnManageSMS.Owner = null;
            this.btnManageSMS.Size = new System.Drawing.Size(68, 85);
            this.btnManageSMS.TabIndex = 0;
            this.btnManageSMS.Title = "메시지관리";
            this.btnManageSMS.UseVisualStyleBackColor = true;
            // 
            // btnCreateFire
            // 
            this.btnCreateFire.CheckedBkgndImage = null;
            this.btnCreateFire.CheckedImage = null;
            this.btnCreateFire.IsChecked = false;
            this.btnCreateFire.Location = new System.Drawing.Point(1, 1);
            this.btnCreateFire.MouseOverBkgndImage = null;
            this.btnCreateFire.Name = "btnCreateFire";
            this.btnCreateFire.NormalImage = null;
            this.btnCreateFire.Owner = null;
            this.btnCreateFire.Size = new System.Drawing.Size(60, 85);
            this.btnCreateFire.TabIndex = 0;
            this.btnCreateFire.Title = "화재탐지";
            this.btnCreateFire.UseVisualStyleBackColor = true;
            // 
            // pictureBoxAdminRibbon1
            // 
            this.pictureBoxAdminRibbon1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxAdminRibbon1.BackgroundImage = global::SDMS.Properties.Resources.Separator;
            this.pictureBoxAdminRibbon1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxAdminRibbon1.Location = new System.Drawing.Point(190, 3);
            this.pictureBoxAdminRibbon1.Name = "pictureBoxAdminRibbon1";
            this.pictureBoxAdminRibbon1.Size = new System.Drawing.Size(13, 85);
            this.pictureBoxAdminRibbon1.TabIndex = 6;
            this.pictureBoxAdminRibbon1.TabStop = false;
            // 
            // pictureBoxAdminRibbon4
            // 
            this.pictureBoxAdminRibbon4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxAdminRibbon4.BackgroundImage = global::SDMS.Properties.Resources.Separator;
            this.pictureBoxAdminRibbon4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxAdminRibbon4.Location = new System.Drawing.Point(595, 3);
            this.pictureBoxAdminRibbon4.Name = "pictureBoxAdminRibbon4";
            this.pictureBoxAdminRibbon4.Size = new System.Drawing.Size(13, 85);
            this.pictureBoxAdminRibbon4.TabIndex = 6;
            this.pictureBoxAdminRibbon4.TabStop = false;
            // 
            // panelReportRibbonBarLeft
            // 
            this.panelReportRibbonBarLeft.BackColor = System.Drawing.Color.Transparent;
            this.panelReportRibbonBarLeft.BackgroundImage = global::SDMS.Properties.Resources.RibbonBar_Left;
            this.panelReportRibbonBarLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelReportRibbonBarLeft.Location = new System.Drawing.Point(0, 314);
            this.panelReportRibbonBarLeft.Name = "panelReportRibbonBarLeft";
            this.panelReportRibbonBarLeft.Size = new System.Drawing.Size(134, 87);
            this.panelReportRibbonBarLeft.TabIndex = 8;
            // 
            // panelAdminRibbonBarLeft
            // 
            this.panelAdminRibbonBarLeft.BackColor = System.Drawing.Color.Transparent;
            this.panelAdminRibbonBarLeft.BackgroundImage = global::SDMS.Properties.Resources.RibbonBar_Left;
            this.panelAdminRibbonBarLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelAdminRibbonBarLeft.Location = new System.Drawing.Point(2, 198);
            this.panelAdminRibbonBarLeft.Name = "panelAdminRibbonBarLeft";
            this.panelAdminRibbonBarLeft.Size = new System.Drawing.Size(134, 87);
            this.panelAdminRibbonBarLeft.TabIndex = 8;
            // 
            // panelReportRibbonBarRight
            // 
            this.panelReportRibbonBarRight.BackColor = System.Drawing.Color.Transparent;
            this.panelReportRibbonBarRight.BackgroundImage = global::SDMS.Properties.Resources.RibbonBar_Right;
            this.panelReportRibbonBarRight.Location = new System.Drawing.Point(1663, 315);
            this.panelReportRibbonBarRight.Name = "panelReportRibbonBarRight";
            this.panelReportRibbonBarRight.Size = new System.Drawing.Size(254, 87);
            this.panelReportRibbonBarRight.TabIndex = 7;
            // 
            // panelAdminRibbonBarRight
            // 
            this.panelAdminRibbonBarRight.BackColor = System.Drawing.Color.Transparent;
            this.panelAdminRibbonBarRight.BackgroundImage = global::SDMS.Properties.Resources.RibbonBar_Right;
            this.panelAdminRibbonBarRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelAdminRibbonBarRight.Location = new System.Drawing.Point(1665, 198);
            this.panelAdminRibbonBarRight.Name = "panelAdminRibbonBarRight";
            this.panelAdminRibbonBarRight.Size = new System.Drawing.Size(254, 87);
            this.panelAdminRibbonBarRight.TabIndex = 7;
            // 
            // pictureBoxReport
            // 
            this.pictureBoxReport.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            this.pictureBoxReport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxReport.Location = new System.Drawing.Point(199, 29);
            this.pictureBoxReport.Name = "pictureBoxReport";
            this.pictureBoxReport.Size = new System.Drawing.Size(98, 35);
            this.pictureBoxReport.TabIndex = 5;
            this.pictureBoxReport.TabStop = false;
            // 
            // pictureBoxAdmin
            // 
            this.pictureBoxAdmin.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            this.pictureBoxAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxAdmin.Location = new System.Drawing.Point(100, 29);
            this.pictureBoxAdmin.Name = "pictureBoxAdmin";
            this.pictureBoxAdmin.Size = new System.Drawing.Size(98, 35);
            this.pictureBoxAdmin.TabIndex = 5;
            this.pictureBoxAdmin.TabStop = false;
            // 
            // pictureBoxMonitoring
            // 
            this.pictureBoxMonitoring.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            this.pictureBoxMonitoring.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxMonitoring.Location = new System.Drawing.Point(1, 29);
            this.pictureBoxMonitoring.Name = "pictureBoxMonitoring";
            this.pictureBoxMonitoring.Size = new System.Drawing.Size(98, 35);
            this.pictureBoxMonitoring.TabIndex = 5;
            this.pictureBoxMonitoring.TabStop = false;
            // 
            // btnMin
            // 
            this.btnMin.BackgroundImage = global::SDMS.Properties.Resources.HideWindow_Normal;
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
            this.btnMax.BackgroundImage = global::SDMS.Properties.Resources.NormalWindow_Normal;
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
            this.btnClose.BackgroundImage = global::SDMS.Properties.Resources.CloseWindow_Normal;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.Location = new System.Drawing.Point(1888, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(32, 24);
            this.btnClose.TabIndex = 3;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SDMS.Properties.Resources.App_Icon_Small;
            this.pictureBox1.Location = new System.Drawing.Point(0, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // btnFire
            // 
            this.btnFire.BackColor = System.Drawing.Color.Transparent;
            this.btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar;
            this.btnFire.ExtraImage = null;
            this.btnFire.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFire.ForeColor = System.Drawing.Color.White;
            this.btnFire.Location = new System.Drawing.Point(1674, 67);
            this.btnFire.Name = "btnFire";
            this.btnFire.Size = new System.Drawing.Size(246, 87);
            this.btnFire.TabIndex = 1;
            this.btnFire.Text = "화재신고";
            this.btnFire.TextData = null;
            this.btnFire.UseVisualStyleBackColor = false;
            this.btnFire.X = 0;
            this.btnFire.Y = 0;
            this.btnFire.Click += new System.EventHandler(this.btnFire_Click);
            this.btnFire.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnFire_KeyDown);
            // 
            // panelLog
            // 
            this.panelLog.BackColor = System.Drawing.Color.Transparent;
            this.panelLog.BackgroundImage = global::SDMS.Properties.Resources.Status_Bar;
            this.panelLog.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLog.Controls.Add(this.label5);
            this.panelLog.Controls.Add(this.pictureBoxLog);
            this.panelLog.Controls.Add(this.mLabelLog);
            this.panelLog.Location = new System.Drawing.Point(813, 67);
            this.panelLog.Name = "panelLog";
            this.panelLog.RealTimeInfo = null;
            this.panelLog.Size = new System.Drawing.Size(853, 87);
            this.panelLog.TabIndex = 0;
            this.panelLog.Text = "FormRealTimeInfo";
            this.panelLog.TextColor = System.Drawing.Color.White;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(5, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "화재대응현황";
            // 
            // pictureBoxLog
            // 
            this.pictureBoxLog.BackgroundImage = global::SDMS.Properties.Resources.Log_icon;
            this.pictureBoxLog.Location = new System.Drawing.Point(16, 5);
            this.pictureBoxLog.Name = "pictureBoxLog";
            this.pictureBoxLog.Size = new System.Drawing.Size(55, 57);
            this.pictureBoxLog.TabIndex = 6;
            this.pictureBoxLog.TabStop = false;
            // 
            // mLabelLog
            // 
            this.mLabelLog.AutoSize = true;
            this.mLabelLog.BackColor = System.Drawing.Color.Transparent;
            this.mLabelLog.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mLabelLog.ForeColor = System.Drawing.Color.White;
            this.mLabelLog.Location = new System.Drawing.Point(98, 24);
            this.mLabelLog.Name = "mLabelLog";
            this.mLabelLog.Size = new System.Drawing.Size(175, 32);
            this.mLabelLog.TabIndex = 1;
            this.mLabelLog.Text = "화재 대응 로그";
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.Color.Transparent;
            this.panelStatus.BackgroundImage = global::SDMS.Properties.Resources.Log_Bar;
            this.panelStatus.Controls.Add(this.mLabelZone);
            this.panelStatus.Controls.Add(this.label4);
            this.panelStatus.Controls.Add(this.pictureBoxStatus);
            this.panelStatus.Controls.Add(this.mLabelStatus);
            this.panelStatus.Location = new System.Drawing.Point(366, 67);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(441, 87);
            this.panelStatus.TabIndex = 0;
            // 
            // mLabelZone
            // 
            this.mLabelZone.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mLabelZone.ForeColor = System.Drawing.Color.White;
            this.mLabelZone.Location = new System.Drawing.Point(86, 9);
            this.mLabelZone.Name = "mLabelZone";
            this.mLabelZone.Size = new System.Drawing.Size(332, 22);
            this.mLabelZone.TabIndex = 8;
            this.mLabelZone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(14, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "상황정보";
            // 
            // pictureBoxStatus
            // 
            this.pictureBoxStatus.BackgroundImage = global::SDMS.Properties.Resources.Status_Icon;
            this.pictureBoxStatus.Location = new System.Drawing.Point(12, 6);
            this.pictureBoxStatus.Name = "pictureBoxStatus";
            this.pictureBoxStatus.Size = new System.Drawing.Size(58, 60);
            this.pictureBoxStatus.TabIndex = 6;
            this.pictureBoxStatus.TabStop = false;
            // 
            // mLabelStatus
            // 
            this.mLabelStatus.BackColor = System.Drawing.Color.Transparent;
            this.mLabelStatus.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mLabelStatus.ForeColor = System.Drawing.Color.White;
            this.mLabelStatus.Location = new System.Drawing.Point(89, 31);
            this.mLabelStatus.Name = "mLabelStatus";
            this.mLabelStatus.Size = new System.Drawing.Size(339, 32);
            this.mLabelStatus.TabIndex = 1;
            this.mLabelStatus.Text = "화재 탐지 없음";
            this.mLabelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelClock
            // 
            this.panelClock.BackColor = System.Drawing.Color.Transparent;
            this.panelClock.BackgroundImage = global::SDMS.Properties.Resources.Clock_Bar;
            this.panelClock.Controls.Add(this.label3);
            this.panelClock.Controls.Add(this.pictureBoxClock);
            this.panelClock.Controls.Add(this.labelTime);
            this.panelClock.Controls.Add(this.labelDate);
            this.panelClock.Location = new System.Drawing.Point(0, 67);
            this.panelClock.Name = "panelClock";
            this.panelClock.Size = new System.Drawing.Size(358, 87);
            this.panelClock.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(17, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "현재시간";
            // 
            // pictureBoxClock
            // 
            this.pictureBoxClock.BackgroundImage = global::SDMS.Properties.Resources.Clock_Icon;
            this.pictureBoxClock.Location = new System.Drawing.Point(12, 4);
            this.pictureBoxClock.Name = "pictureBoxClock";
            this.pictureBoxClock.Size = new System.Drawing.Size(62, 61);
            this.pictureBoxClock.TabIndex = 6;
            this.pictureBoxClock.TabStop = false;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.BackColor = System.Drawing.Color.Transparent;
            this.labelTime.Font = new System.Drawing.Font("맑은 고딕", 21F, System.Drawing.FontStyle.Bold);
            this.labelTime.ForeColor = System.Drawing.Color.White;
            this.labelTime.Location = new System.Drawing.Point(138, 33);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(127, 38);
            this.labelTime.TabIndex = 1;
            this.labelTime.Text = "00:00:00";
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.BackColor = System.Drawing.Color.Transparent;
            this.labelDate.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDate.ForeColor = System.Drawing.Color.White;
            this.labelDate.Location = new System.Drawing.Point(146, 14);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(105, 17);
            this.labelDate.TabIndex = 0;
            this.labelDate.Text = "2013년 7월 1일";
            // 
            // m_CheckReciver
            // 
            this.m_CheckReciver.Interval = 1000;
            this.m_CheckReciver.Tick += new System.EventHandler(this.m_CheckReciver_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemBig,
            this.toolStripMenuItemNormal});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(145, 48);
            // 
            // toolStripMenuItemBig
            // 
            this.toolStripMenuItemBig.Name = "toolStripMenuItemBig";
            this.toolStripMenuItemBig.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemBig.Text = "CCTV 최대화";
            this.toolStripMenuItemBig.Click += new System.EventHandler(this.toolStripMenuItemBig_Click);
            // 
            // toolStripMenuItemNormal
            // 
            this.toolStripMenuItemNormal.Name = "toolStripMenuItemNormal";
            this.toolStripMenuItemNormal.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemNormal.Text = "일반 모드";
            this.toolStripMenuItemNormal.Click += new System.EventHandler(this.toolStripMenuItemNormal_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1386, 788);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelReactionHistory);
            this.Controls.Add(this.panelProcessHistory);
            this.Controls.Add(this.panelMiddle);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DatePickerEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatePickerStart)).EndInit();
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelReactionHistory.ResumeLayout(false);
            this.panelReactionHistory.PerformLayout();
            this.panelProcessHistory.ResumeLayout(false);
            this.panelProcessHistory.PerformLayout();
            this.panelMiddle.ResumeLayout(false);
            this.panelMiddle.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelReportRibbonBarMiddle.ResumeLayout(false);
            this.panelAdminRibbonBarMiddle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdminRibbon3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdminRibbon2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdminRibbon1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdminRibbon4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMonitoring)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelLog.ResumeLayout(false);
            this.panelLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLog)).EndInit();
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
            this.panelClock.ResumeLayout(false);
            this.panelClock.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClock)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panelClock;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Timer mClockTimer;
		private ButtonEx btnFire;
        private System.Windows.Forms.Label mLabelLog;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label mLabelStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnMin;
        private System.Windows.Forms.Button btnMax;
        private System.Windows.Forms.Button btnClose;
        private SDMS.TextPictureBox pictureBoxMonitoring;
        private SDMS.TextPictureBox pictureBoxAdmin;
        private SDMS.TextPictureBox pictureBoxReport;
        private System.Windows.Forms.Timer m_MainTimer;
        private System.Windows.Forms.PictureBox pictureBoxClock;
        private System.Windows.Forms.PictureBox pictureBoxLog;
        private System.Windows.Forms.PictureBox pictureBoxStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Button btnMultiCCTV;
        private System.Windows.Forms.Button btnScreenShot;
        private System.Windows.Forms.Button btnOutside;
        private System.Windows.Forms.Button btnBoth;
        private System.Windows.Forms.Button btnInside;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnOrbit;
        private System.Windows.Forms.Button btnPanning;
        private System.Windows.Forms.Button btnPick;
        private System.Windows.Forms.Button btnFullScreen;
        private System.Windows.Forms.Button btnHome;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape4;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape3;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private System.Windows.Forms.Button btnSelectZone;
        private System.Windows.Forms.ComboBox cboFloor;
        private System.Windows.Forms.ComboBox cboBuilding;
        private System.Windows.Forms.ComboBox cboBuildingGroup;
        private System.Windows.Forms.Label labelSelectZone;
        private System.Windows.Forms.Button btnLayerFire;
        private System.Windows.Forms.Label labelFR;
        private System.Windows.Forms.Label labelFA;
        private System.Windows.Forms.Label labelHD;
        private System.Windows.Forms.Label labelFE;
        private System.Windows.Forms.Label labelCCTV;
        private System.Windows.Forms.Label labelPump;
        private System.Windows.Forms.Label labelCooler;
        private System.Windows.Forms.Label labelFire;
        private System.Windows.Forms.Button btnLayerFR;
        private System.Windows.Forms.Button btnLayerFA;
        private System.Windows.Forms.Button btnLayerHD;
        private System.Windows.Forms.Button btnLayerFE;
        private System.Windows.Forms.Button btnLayerCCTV;
        private System.Windows.Forms.Button btnLayerPump;
        private System.Windows.Forms.Button btnLayerSpringCooler;
        private System.Windows.Forms.Panel panelAdminRibbonBarRight;
        private System.Windows.Forms.PictureBox pictureBoxAdminRibbon4;
        private System.Windows.Forms.PictureBox pictureBoxAdminRibbon3;
        private System.Windows.Forms.PictureBox pictureBoxAdminRibbon2;
        private System.Windows.Forms.PictureBox pictureBoxAdminRibbon1;
        private System.Windows.Forms.Panel panelAdminRibbonBarLeft;
        private System.Windows.Forms.Panel panelAdminRibbonBarMiddle;
        private SDMS.RibbonButton btnShowList;
		private SDMS.RibbonButton btnDelete;
		private SDMS.RibbonButton btnCreateCCTV;
		private SDMS.RibbonButton btnCreatePump;
		private SDMS.RibbonButton btnCreateSpringCooler;
		private SDMS.RibbonButton btnCreateFire;
		private SDMS.RibbonButton btnManageBroadcast;
		private SDMS.RibbonButton btnManageManager;
		private SDMS.RibbonButton btnManagePrint;
		private SDMS.RibbonButton btnManageSMS;
		private SDMS.RibbonButton btnManageFacility;
		private SDMS.RibbonButton btnManageDetect;
		private SDMS.RibbonButton btnSave;
        private System.Windows.Forms.Panel panelReportRibbonBarMiddle;
		private SDMS.RibbonButton btnReactionHistory;
		private SDMS.RibbonButton btnProcessHistory;
		private SDMS.RibbonButton btnDetectHistory;
        private System.Windows.Forms.Panel panelReportRibbonBarLeft;
        private System.Windows.Forms.Panel panelReportRibbonBarRight;
        private System.Windows.Forms.Panel panelProcessHistory;
        private System.Windows.Forms.Button proc_btnSelectZone;
        private System.Windows.Forms.ComboBox proc_cboFloor;
        private System.Windows.Forms.ComboBox proc_cboBuilding;
        private System.Windows.Forms.ComboBox proc_cboBuildingGroup;
        private System.Windows.Forms.Label proc_lblSelectZone;
        private System.Windows.Forms.Panel panelReactionHistory;
        private System.Windows.Forms.ComboBox proc_cboLatelyDate;
        private System.Windows.Forms.Label lblProcessDate;
        private System.Windows.Forms.Label lblFireSelect;
        private System.Windows.Forms.Label lblReactionDate;
        private System.Windows.Forms.Panel panelMiddle;
        private AxXtremeCalendarControl.AxDatePicker DatePickerEnd;
        private AxXtremeCalendarControl.AxDatePicker DatePickerStart;
        private System.Windows.Forms.Button proc_btnStartDate;
        private System.Windows.Forms.Button proc_btnEndDate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSaveHWP;
        private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label mLabelZone;
		private RealTimeInfoPane panelLog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Label labelFireDetect;
		private System.Windows.Forms.ComboBox cmbFireDetect;
        private RibbonButton btnEditFacilityZone;
        private System.Windows.Forms.Label labelCCTVLow;
        private System.Windows.Forms.Button btnLayerLowCCTV;
        private RibbonButton btnBackupDB;
        private System.Windows.Forms.CheckBox checkBoxEquipZoneCCTV;
        private System.Windows.Forms.ComboBox cboEquipZone;
        
        private System.Windows.Forms.ComboBox react_cboSearchType;
        private System.Windows.Forms.Button react_btnEndDate;
        private System.Windows.Forms.Button react_btnStartDate;
                private System.Windows.Forms.ComboBox cboFireSelect;
        private System.Windows.Forms.ComboBox react_cboEndTime;
        private System.Windows.Forms.ComboBox react_cboStartTime;
        private System.Windows.Forms.Button btnShowCCTVList;
        private System.Windows.Forms.Label labelSensorMonitor;
        private System.Windows.Forms.Button btnSensorMonitor;
		private System.Windows.Forms.Timer m_CheckReciver;
		private System.Windows.Forms.Button btnSaveHome;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBig;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNormal;
        private System.Windows.Forms.Button button3;
        private RibbonButton btnSMSHistory;

    }
}