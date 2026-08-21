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
            this.mClockTimer = new System.Windows.Forms.Timer(this.components);
            this.m_MainTimer = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.m_CheckReciver = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemBig = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.DatePickerStart = new System.Windows.Forms.DateTimePicker();
            this.DatePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.DatePickerStart2 = new System.Windows.Forms.DateTimePicker();
            this.DatePickerEnd2 = new System.Windows.Forms.DateTimePicker();
            this.DatePickerDetectPSMStart = new System.Windows.Forms.DateTimePicker();
            this.DatePickerDetectPSMEnd = new System.Windows.Forms.DateTimePicker();
            this.DatePickerNotOperationPSMStart = new System.Windows.Forms.DateTimePicker();
            this.DatePickerNotOperationPSMEnd = new System.Windows.Forms.DateTimePicker();
            this.DatePickerActionPSMStart = new System.Windows.Forms.DateTimePicker();
            this.DatePickerActionPSMEnd = new System.Windows.Forms.DateTimePicker();
            this.DatePickerSMSPSMStart = new System.Windows.Forms.DateTimePicker();
            this.DatePickerSMSPSMEnd = new System.Windows.Forms.DateTimePicker();
            this.pnNotOperationPSM = new System.Windows.Forms.Panel();
            this.btnNotOperationPSMEndDate = new System.Windows.Forms.Button();
            this.btnNotOperationPSMStartDate = new System.Windows.Forms.Button();
            this.cboNotOperationPSMLatelyDate = new System.Windows.Forms.ComboBox();
            this.lblNotOperationPSMDate = new System.Windows.Forms.Label();
            this.btnNotOperationPSMSelectZone = new System.Windows.Forms.Button();
            this.cboNotOperationPSMBuilding = new System.Windows.Forms.ComboBox();
            this.lblNotOperationPSMSelectZone = new System.Windows.Forms.Label();
            this.pnDetectPSM = new System.Windows.Forms.Panel();
            this.btnDetectPSMDateFormat = new System.Windows.Forms.Button();
            this.nudDetectPSMSplitUnitDetail = new System.Windows.Forms.NumericUpDown();
            this.lblDetectPSMSplitUnitDetail = new System.Windows.Forms.Label();
            this.lblDetectPSMViewCount = new System.Windows.Forms.Label();
            this.lblDetectPSMSplitUnit = new System.Windows.Forms.Label();
            this.cboDetectPSMViewCount = new System.Windows.Forms.ComboBox();
            this.cboDetectPSMSplitUnit = new System.Windows.Forms.ComboBox();
            this.btnDetectPSMEndDate = new System.Windows.Forms.Button();
            this.btnDetectPSMStartDate = new System.Windows.Forms.Button();
            this.cboDetectPSMLatelyDate = new System.Windows.Forms.ComboBox();
            this.lblDetectPSMDate = new System.Windows.Forms.Label();
            this.btnDetectPSMSelectZone = new System.Windows.Forms.Button();
            this.cboDetectPSMBuilding = new System.Windows.Forms.ComboBox();
            this.lblDetectPSMSelectZone = new System.Windows.Forms.Label();
            this.labelDetectPSMDateFormat = new System.Windows.Forms.Label();
            this.pnSMSPSM = new System.Windows.Forms.Panel();
            this.btnSMSPSMEndDate = new System.Windows.Forms.Button();
            this.btnSMSPSMStartDate = new System.Windows.Forms.Button();
            this.cboSMSPSMLatelyDate = new System.Windows.Forms.ComboBox();
            this.lblSMSPSMDate = new System.Windows.Forms.Label();
            this.btnSMSPSMSelectZone = new System.Windows.Forms.Button();
            this.cboSMSPSMBuilding = new System.Windows.Forms.ComboBox();
            this.lblSMSPSMSelectZone = new System.Windows.Forms.Label();
            this.pnActionPSM = new System.Windows.Forms.Panel();
            this.btnReactionPSMSelectDisaster = new System.Windows.Forms.Button();
            this.cboActionPSMSearchType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboActionPSMSelect = new System.Windows.Forms.ComboBox();
            this.btnActionPSMEndDate = new System.Windows.Forms.Button();
            this.btnActionPSMStartDate = new System.Windows.Forms.Button();
            this.cboActionPSMEndTime = new System.Windows.Forms.ComboBox();
            this.cboActionPSMStartTime = new System.Windows.Forms.ComboBox();
            this.lblActionPSMSelect = new System.Windows.Forms.Label();
            this.lblActionPSMDate = new System.Windows.Forms.Label();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.labelSaveHome = new System.Windows.Forms.Label();
            this.btnLayerCCTVDisconnected = new System.Windows.Forms.Button();
            this.btnSaveHome = new System.Windows.Forms.Button();
            this.btnLayerLowCCTV = new System.Windows.Forms.Button();
            this.labelCCTVDisconnected = new System.Windows.Forms.Label();
            this.labelCCTVLow = new System.Windows.Forms.Label();
            this.labelNotice = new System.Windows.Forms.Label();
            this.labelBuildingText = new System.Windows.Forms.Label();
            this.labelFR = new System.Windows.Forms.Label();
            this.labelFA = new System.Windows.Forms.Label();
            this.labelHD = new System.Windows.Forms.Label();
            this.labelFE = new System.Windows.Forms.Label();
            this.labelCCTV = new System.Windows.Forms.Label();
            this.labelPump = new System.Windows.Forms.Label();
            this.labelCooler = new System.Windows.Forms.Label();
            this.labelFire = new System.Windows.Forms.Label();
            this.btnLayerNotice = new System.Windows.Forms.Button();
            this.btnLayerBuildingText = new System.Windows.Forms.Button();
            this.btnLayerFR = new System.Windows.Forms.Button();
            this.btnLayerFA = new System.Windows.Forms.Button();
            this.btnLayerHD = new System.Windows.Forms.Button();
            this.btnLayerFE = new System.Windows.Forms.Button();
            this.btnLayerCCTV = new System.Windows.Forms.Button();
            this.btnLayerPump = new System.Windows.Forms.Button();
            this.btnLayerSpringCooler = new System.Windows.Forms.Button();
            this.btnLayerFire = new System.Windows.Forms.Button();
            this.panelReactionHistory = new System.Windows.Forms.Panel();
            this.btnReactionSelectDisaster = new System.Windows.Forms.Button();
            this.btnReactionIntrusionSelectDisaster = new System.Windows.Forms.Button();
            this.react_cboSearchType = new System.Windows.Forms.ComboBox();
            this.react_cboSearchTypeIntrusion = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cboFireSelect = new System.Windows.Forms.ComboBox();
            this.cboActionIntrusionSelect = new System.Windows.Forms.ComboBox();
            this.react_btnEndDate = new System.Windows.Forms.Button();
            this.react_btnStartDate = new System.Windows.Forms.Button();
            this.react_cboEndTime = new System.Windows.Forms.ComboBox();
            this.react_cboStartTime = new System.Windows.Forms.ComboBox();
            this.lblFireSelect = new System.Windows.Forms.Label();
            this.lblIntrusionSelect = new System.Windows.Forms.Label();
            this.lblReactionDate = new System.Windows.Forms.Label();
            this.panelProcessHistory = new System.Windows.Forms.Panel();
            this.btnDateFormat = new System.Windows.Forms.Button();
            this.nudSplitUnitDetail = new System.Windows.Forms.NumericUpDown();
            this.lblSplitUnitDetail = new System.Windows.Forms.Label();
            this.lblViewCount = new System.Windows.Forms.Label();
            this.labelDetectDateFormat = new System.Windows.Forms.Label();
            this.lblSplitUnit = new System.Windows.Forms.Label();
            this.proc_cboViewCount = new System.Windows.Forms.ComboBox();
            this.proc_cboSplitUnit = new System.Windows.Forms.ComboBox();
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
            this.btnSimulator = new System.Windows.Forms.Button();
            this.labelSensorMonitor = new System.Windows.Forms.Label();
            this.btnSendMessage = new System.Windows.Forms.Button();
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
            this.btnWeatherInfo = new System.Windows.Forms.Button();
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBox2D = new SDMS.TextPictureBox();
            this.pictureBoxCCTV = new SDMS.TextPictureBox();
            this.btnDefaultCCTV = new UnE.GUI.RibbonButton();
            this.btnMissionStatus = new UnE.GUI.RibbonButton();
            this.btnBulletin = new UnE.GUI.RibbonButton();
            this.btnSOP = new UnE.GUI.RibbonButton();
            this.btnSDMS = new UnE.GUI.RibbonButton();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.panelReportRibbonBarMiddle = new System.Windows.Forms.Panel();
            this.btnSMSIntrusionHistory = new SDMS.RibbonButton();
            this.btnReactionIntrusionHistory = new SDMS.RibbonButton();
            this.btnProcessIntrusionHistory = new SDMS.RibbonButton();
            this.btnDetectIntrusionHistory = new SDMS.RibbonButton();
            this.imgReportSplit2 = new System.Windows.Forms.PictureBox();
            this.btnDetectIntrusionAnalyze = new SDMS.RibbonButton();
            this.btnSMSPSMHistory = new SDMS.RibbonButton();
            this.btnNotOperationPSMHistory = new SDMS.RibbonButton();
            this.btnDetectPSMHistory = new SDMS.RibbonButton();
            this.imgReportSplit = new System.Windows.Forms.PictureBox();
            this.btnActionPSMHistory = new SDMS.RibbonButton();
            this.btnSMSHistory = new SDMS.RibbonButton();
            this.btnReactionHistory = new SDMS.RibbonButton();
            this.btnProcessHistory = new SDMS.RibbonButton();
            this.btnDetectPSMAnalyze = new SDMS.RibbonButton();
            this.btnDetectAnalyze = new SDMS.RibbonButton();
            this.btnDetectHistory = new SDMS.RibbonButton();
            this.labelDate = new System.Windows.Forms.Label();
            this.panelAdminRibbonBarMiddle = new System.Windows.Forms.Panel();
            this.sensorMgrBtn = new SDMS.RibbonButton();
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
            this.btnEarthquake = new SDMS.RibbonButton();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.pnNotOperationPSM.SuspendLayout();
            this.pnDetectPSM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDetectPSMSplitUnitDetail)).BeginInit();
            this.pnSMSPSM.SuspendLayout();
            this.pnActionPSM.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelReactionHistory.SuspendLayout();
            this.panelProcessHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSplitUnitDetail)).BeginInit();
            this.panelMiddle.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2D)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCCTV)).BeginInit();
            this.panelReportRibbonBarMiddle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgReportSplit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgReportSplit)).BeginInit();
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
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.Location = new System.Drawing.Point(46, 214);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1280, 1024);
            this.panelBottom.TabIndex = 1;
            // 
            // mClockTimer
            // 
            this.mClockTimer.Interval = 900;
            this.mClockTimer.Tick += new System.EventHandler(this.OnTimer);
            // 
            // m_MainTimer
            // 
            this.m_MainTimer.Interval = 300;
            this.m_MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
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
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // DatePickerStart
            // 
            this.DatePickerStart.Location = new System.Drawing.Point(43, 419);
            this.DatePickerStart.Name = "DatePickerStart";
            this.DatePickerStart.Size = new System.Drawing.Size(200, 21);
            this.DatePickerStart.TabIndex = 11;
            this.DatePickerStart.ValueChanged += new System.EventHandler(this.DatePickerStart_ValueChanged);
            this.DatePickerStart.Leave += new System.EventHandler(this.DatePickerStart_Leave);
            // 
            // DatePickerEnd
            // 
            this.DatePickerEnd.Location = new System.Drawing.Point(171, 419);
            this.DatePickerEnd.Name = "DatePickerEnd";
            this.DatePickerEnd.Size = new System.Drawing.Size(200, 21);
            this.DatePickerEnd.TabIndex = 12;
            this.DatePickerEnd.ValueChanged += new System.EventHandler(this.DatePickerEnd_ValueChanged);
            this.DatePickerEnd.Leave += new System.EventHandler(this.DatePickerEnd_Leave);
            // 
            // DatePickerStart2
            // 
            this.DatePickerStart2.Location = new System.Drawing.Point(540, 502);
            this.DatePickerStart2.Name = "DatePickerStart2";
            this.DatePickerStart2.Size = new System.Drawing.Size(200, 21);
            this.DatePickerStart2.TabIndex = 13;
            this.DatePickerStart2.ValueChanged += new System.EventHandler(this.DatePickerStart2_ValueChanged);
            this.DatePickerStart2.Leave += new System.EventHandler(this.DatePickerStart2_Leave);
            // 
            // DatePickerEnd2
            // 
            this.DatePickerEnd2.Location = new System.Drawing.Point(605, 469);
            this.DatePickerEnd2.Name = "DatePickerEnd2";
            this.DatePickerEnd2.Size = new System.Drawing.Size(200, 21);
            this.DatePickerEnd2.TabIndex = 14;
            this.DatePickerEnd2.ValueChanged += new System.EventHandler(this.DatePickerEnd2_ValueChanged);
            this.DatePickerEnd2.Leave += new System.EventHandler(this.DatePickerEnd2_Leave);
            // 
            // DatePickerDetectPSMStart
            // 
            this.DatePickerDetectPSMStart.Location = new System.Drawing.Point(856, 376);
            this.DatePickerDetectPSMStart.Name = "DatePickerDetectPSMStart";
            this.DatePickerDetectPSMStart.Size = new System.Drawing.Size(200, 21);
            this.DatePickerDetectPSMStart.TabIndex = 17;
            this.DatePickerDetectPSMStart.ValueChanged += new System.EventHandler(this.DatePickerDetectPSMStart_ValueChanged);
            // 
            // DatePickerDetectPSMEnd
            // 
            this.DatePickerDetectPSMEnd.Location = new System.Drawing.Point(864, 384);
            this.DatePickerDetectPSMEnd.Name = "DatePickerDetectPSMEnd";
            this.DatePickerDetectPSMEnd.Size = new System.Drawing.Size(200, 21);
            this.DatePickerDetectPSMEnd.TabIndex = 18;
            this.DatePickerDetectPSMEnd.ValueChanged += new System.EventHandler(this.DatePickerDetectPSMEnd_ValueChanged);
            // 
            // DatePickerNotOperationPSMStart
            // 
            this.DatePickerNotOperationPSMStart.Location = new System.Drawing.Point(872, 392);
            this.DatePickerNotOperationPSMStart.Name = "DatePickerNotOperationPSMStart";
            this.DatePickerNotOperationPSMStart.Size = new System.Drawing.Size(200, 21);
            this.DatePickerNotOperationPSMStart.TabIndex = 19;
            this.DatePickerNotOperationPSMStart.ValueChanged += new System.EventHandler(this.DatePickerNotOperationPSMStart_ValueChanged);
            // 
            // DatePickerNotOperationPSMEnd
            // 
            this.DatePickerNotOperationPSMEnd.Location = new System.Drawing.Point(880, 400);
            this.DatePickerNotOperationPSMEnd.Name = "DatePickerNotOperationPSMEnd";
            this.DatePickerNotOperationPSMEnd.Size = new System.Drawing.Size(200, 21);
            this.DatePickerNotOperationPSMEnd.TabIndex = 20;
            this.DatePickerNotOperationPSMEnd.ValueChanged += new System.EventHandler(this.DatePickerNotOperationPSMEnd_ValueChanged);
            // 
            // DatePickerActionPSMStart
            // 
            this.DatePickerActionPSMStart.Location = new System.Drawing.Point(888, 408);
            this.DatePickerActionPSMStart.Name = "DatePickerActionPSMStart";
            this.DatePickerActionPSMStart.Size = new System.Drawing.Size(200, 21);
            this.DatePickerActionPSMStart.TabIndex = 21;
            this.DatePickerActionPSMStart.ValueChanged += new System.EventHandler(this.DatePickerActionPSMStart_ValueChanged);
            // 
            // DatePickerActionPSMEnd
            // 
            this.DatePickerActionPSMEnd.Location = new System.Drawing.Point(896, 416);
            this.DatePickerActionPSMEnd.Name = "DatePickerActionPSMEnd";
            this.DatePickerActionPSMEnd.Size = new System.Drawing.Size(200, 21);
            this.DatePickerActionPSMEnd.TabIndex = 22;
            this.DatePickerActionPSMEnd.ValueChanged += new System.EventHandler(this.DatePickerActionPSMEnd_ValueChanged);
            // 
            // DatePickerSMSPSMStart
            // 
            this.DatePickerSMSPSMStart.Location = new System.Drawing.Point(904, 424);
            this.DatePickerSMSPSMStart.Name = "DatePickerSMSPSMStart";
            this.DatePickerSMSPSMStart.Size = new System.Drawing.Size(200, 21);
            this.DatePickerSMSPSMStart.TabIndex = 23;
            this.DatePickerSMSPSMStart.ValueChanged += new System.EventHandler(this.DatePickerSMSPSMStart_ValueChanged);
            // 
            // DatePickerSMSPSMEnd
            // 
            this.DatePickerSMSPSMEnd.Location = new System.Drawing.Point(912, 432);
            this.DatePickerSMSPSMEnd.Name = "DatePickerSMSPSMEnd";
            this.DatePickerSMSPSMEnd.Size = new System.Drawing.Size(200, 21);
            this.DatePickerSMSPSMEnd.TabIndex = 24;
            this.DatePickerSMSPSMEnd.ValueChanged += new System.EventHandler(this.DatePickerSMSPSMEnd_ValueChanged);
            // 
            // pnNotOperationPSM
            // 
            this.pnNotOperationPSM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnNotOperationPSM.BackColor = System.Drawing.Color.Transparent;
            this.pnNotOperationPSM.BackgroundImage = global::SDMS.Properties.Resources.HToolbar_bkgnd;
            this.pnNotOperationPSM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnNotOperationPSM.Controls.Add(this.btnNotOperationPSMEndDate);
            this.pnNotOperationPSM.Controls.Add(this.btnNotOperationPSMStartDate);
            this.pnNotOperationPSM.Controls.Add(this.cboNotOperationPSMLatelyDate);
            this.pnNotOperationPSM.Controls.Add(this.lblNotOperationPSMDate);
            this.pnNotOperationPSM.Controls.Add(this.btnNotOperationPSMSelectZone);
            this.pnNotOperationPSM.Controls.Add(this.cboNotOperationPSMBuilding);
            this.pnNotOperationPSM.Controls.Add(this.lblNotOperationPSMSelectZone);
            this.pnNotOperationPSM.Location = new System.Drawing.Point(2, 545);
            this.pnNotOperationPSM.Name = "pnNotOperationPSM";
            this.pnNotOperationPSM.Size = new System.Drawing.Size(1915, 48);
            this.pnNotOperationPSM.TabIndex = 25;
            // 
            // btnNotOperationPSMEndDate
            // 
            this.btnNotOperationPSMEndDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnNotOperationPSMEndDate.Location = new System.Drawing.Point(237, 6);
            this.btnNotOperationPSMEndDate.Name = "btnNotOperationPSMEndDate";
            this.btnNotOperationPSMEndDate.Size = new System.Drawing.Size(122, 30);
            this.btnNotOperationPSMEndDate.TabIndex = 9;
            this.btnNotOperationPSMEndDate.Text = "끝 일";
            this.btnNotOperationPSMEndDate.UseVisualStyleBackColor = true;
            this.btnNotOperationPSMEndDate.Click += new System.EventHandler(this.btnNotOperationPSMEndDate_Click);
            // 
            // btnNotOperationPSMStartDate
            // 
            this.btnNotOperationPSMStartDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnNotOperationPSMStartDate.Location = new System.Drawing.Point(110, 6);
            this.btnNotOperationPSMStartDate.Name = "btnNotOperationPSMStartDate";
            this.btnNotOperationPSMStartDate.Size = new System.Drawing.Size(121, 30);
            this.btnNotOperationPSMStartDate.TabIndex = 9;
            this.btnNotOperationPSMStartDate.Text = "시작 일";
            this.btnNotOperationPSMStartDate.UseVisualStyleBackColor = true;
            this.btnNotOperationPSMStartDate.Click += new System.EventHandler(this.btnNotOperationPSMStartDate_Click);
            // 
            // cboNotOperationPSMLatelyDate
            // 
            this.cboNotOperationPSMLatelyDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNotOperationPSMLatelyDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboNotOperationPSMLatelyDate.FormattingEnabled = true;
            this.cboNotOperationPSMLatelyDate.Location = new System.Drawing.Point(381, 6);
            this.cboNotOperationPSMLatelyDate.Name = "cboNotOperationPSMLatelyDate";
            this.cboNotOperationPSMLatelyDate.Size = new System.Drawing.Size(115, 29);
            this.cboNotOperationPSMLatelyDate.TabIndex = 8;
            this.cboNotOperationPSMLatelyDate.SelectedIndexChanged += new System.EventHandler(this.cboNotOperationPSMLatelyDate_SelectedIndexChanged);
            // 
            // lblNotOperationPSMDate
            // 
            this.lblNotOperationPSMDate.AutoSize = true;
            this.lblNotOperationPSMDate.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblNotOperationPSMDate.Location = new System.Drawing.Point(19, 10);
            this.lblNotOperationPSMDate.Name = "lblNotOperationPSMDate";
            this.lblNotOperationPSMDate.Size = new System.Drawing.Size(74, 21);
            this.lblNotOperationPSMDate.TabIndex = 5;
            this.lblNotOperationPSMDate.Text = "기간선택";
            // 
            // btnNotOperationPSMSelectZone
            // 
            this.btnNotOperationPSMSelectZone.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnNotOperationPSMSelectZone.Location = new System.Drawing.Point(1847, 6);
            this.btnNotOperationPSMSelectZone.Name = "btnNotOperationPSMSelectZone";
            this.btnNotOperationPSMSelectZone.Size = new System.Drawing.Size(52, 30);
            this.btnNotOperationPSMSelectZone.TabIndex = 4;
            this.btnNotOperationPSMSelectZone.Text = "선택";
            this.btnNotOperationPSMSelectZone.UseVisualStyleBackColor = true;
            this.btnNotOperationPSMSelectZone.Click += new System.EventHandler(this.btnNotOperationPSMSelectZone_Click);
            // 
            // cboNotOperationPSMBuilding
            // 
            this.cboNotOperationPSMBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNotOperationPSMBuilding.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboNotOperationPSMBuilding.FormattingEnabled = true;
            this.cboNotOperationPSMBuilding.Location = new System.Drawing.Point(1504, 6);
            this.cboNotOperationPSMBuilding.Name = "cboNotOperationPSMBuilding";
            this.cboNotOperationPSMBuilding.Size = new System.Drawing.Size(337, 29);
            this.cboNotOperationPSMBuilding.TabIndex = 3;
            this.cboNotOperationPSMBuilding.SelectedIndexChanged += new System.EventHandler(this.cboNotOperationPSMBuildingGroup_SelectedIndexChanged);
            // 
            // lblNotOperationPSMSelectZone
            // 
            this.lblNotOperationPSMSelectZone.AutoSize = true;
            this.lblNotOperationPSMSelectZone.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblNotOperationPSMSelectZone.Location = new System.Drawing.Point(1440, 10);
            this.lblNotOperationPSMSelectZone.Name = "lblNotOperationPSMSelectZone";
            this.lblNotOperationPSMSelectZone.Size = new System.Drawing.Size(58, 21);
            this.lblNotOperationPSMSelectZone.TabIndex = 2;
            this.lblNotOperationPSMSelectZone.Text = "시설명";
            // 
            // pnDetectPSM
            // 
            this.pnDetectPSM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnDetectPSM.BackColor = System.Drawing.Color.Transparent;
            this.pnDetectPSM.BackgroundImage = global::SDMS.Properties.Resources.HToolbar_bkgnd;
            this.pnDetectPSM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnDetectPSM.Controls.Add(this.btnDetectPSMDateFormat);
            this.pnDetectPSM.Controls.Add(this.nudDetectPSMSplitUnitDetail);
            this.pnDetectPSM.Controls.Add(this.lblDetectPSMSplitUnitDetail);
            this.pnDetectPSM.Controls.Add(this.lblDetectPSMViewCount);
            this.pnDetectPSM.Controls.Add(this.lblDetectPSMSplitUnit);
            this.pnDetectPSM.Controls.Add(this.cboDetectPSMViewCount);
            this.pnDetectPSM.Controls.Add(this.cboDetectPSMSplitUnit);
            this.pnDetectPSM.Controls.Add(this.btnDetectPSMEndDate);
            this.pnDetectPSM.Controls.Add(this.btnDetectPSMStartDate);
            this.pnDetectPSM.Controls.Add(this.cboDetectPSMLatelyDate);
            this.pnDetectPSM.Controls.Add(this.lblDetectPSMDate);
            this.pnDetectPSM.Controls.Add(this.btnDetectPSMSelectZone);
            this.pnDetectPSM.Controls.Add(this.cboDetectPSMBuilding);
            this.pnDetectPSM.Controls.Add(this.lblDetectPSMSelectZone);
            this.pnDetectPSM.Controls.Add(this.labelDetectPSMDateFormat);
            this.pnDetectPSM.Location = new System.Drawing.Point(2, 545);
            this.pnDetectPSM.Name = "pnDetectPSM";
            this.pnDetectPSM.Size = new System.Drawing.Size(1915, 48);
            this.pnDetectPSM.TabIndex = 16;
            // 
            // btnDetectPSMDateFormat
            // 
            this.btnDetectPSMDateFormat.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnDetectPSMDateFormat.Location = new System.Drawing.Point(644, -37);
            this.btnDetectPSMDateFormat.Name = "btnDetectPSMDateFormat";
            this.btnDetectPSMDateFormat.Size = new System.Drawing.Size(82, 30);
            this.btnDetectPSMDateFormat.TabIndex = 17;
            this.btnDetectPSMDateFormat.Text = "날짜형식";
            this.btnDetectPSMDateFormat.UseVisualStyleBackColor = true;
            this.btnDetectPSMDateFormat.Click += new System.EventHandler(this.btnDetectPSMDateFormat_Click);
            // 
            // nudDetectPSMSplitUnitDetail
            // 
            this.nudDetectPSMSplitUnitDetail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudDetectPSMSplitUnitDetail.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.nudDetectPSMSplitUnitDetail.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.nudDetectPSMSplitUnitDetail.Location = new System.Drawing.Point(722, 8);
            this.nudDetectPSMSplitUnitDetail.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.nudDetectPSMSplitUnitDetail.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDetectPSMSplitUnitDetail.Name = "nudDetectPSMSplitUnitDetail";
            this.nudDetectPSMSplitUnitDetail.Size = new System.Drawing.Size(46, 29);
            this.nudDetectPSMSplitUnitDetail.TabIndex = 13;
            this.nudDetectPSMSplitUnitDetail.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDetectPSMSplitUnitDetail.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblDetectPSMSplitUnitDetail
            // 
            this.lblDetectPSMSplitUnitDetail.AutoSize = true;
            this.lblDetectPSMSplitUnitDetail.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblDetectPSMSplitUnitDetail.Location = new System.Drawing.Point(774, 10);
            this.lblDetectPSMSplitUnitDetail.Name = "lblDetectPSMSplitUnitDetail";
            this.lblDetectPSMSplitUnitDetail.Size = new System.Drawing.Size(80, 21);
            this.lblDetectPSMSplitUnitDetail.TabIndex = 16;
            this.lblDetectPSMSplitUnitDetail.Text = "단위 마다";
            // 
            // lblDetectPSMViewCount
            // 
            this.lblDetectPSMViewCount.AutoSize = true;
            this.lblDetectPSMViewCount.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblDetectPSMViewCount.Location = new System.Drawing.Point(869, 10);
            this.lblDetectPSMViewCount.Name = "lblDetectPSMViewCount";
            this.lblDetectPSMViewCount.Size = new System.Drawing.Size(80, 21);
            this.lblDetectPSMViewCount.TabIndex = 14;
            this.lblDetectPSMViewCount.Text = "최대 표기";
            // 
            // lblDetectPSMSplitUnit
            // 
            this.lblDetectPSMSplitUnit.AutoSize = true;
            this.lblDetectPSMSplitUnit.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblDetectPSMSplitUnit.Location = new System.Drawing.Point(524, 10);
            this.lblDetectPSMSplitUnit.Name = "lblDetectPSMSplitUnit";
            this.lblDetectPSMSplitUnit.Size = new System.Drawing.Size(42, 21);
            this.lblDetectPSMSplitUnit.TabIndex = 13;
            this.lblDetectPSMSplitUnit.Text = "단위";
            // 
            // cboDetectPSMViewCount
            // 
            this.cboDetectPSMViewCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetectPSMViewCount.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboDetectPSMViewCount.FormattingEnabled = true;
            this.cboDetectPSMViewCount.Location = new System.Drawing.Point(955, 6);
            this.cboDetectPSMViewCount.Name = "cboDetectPSMViewCount";
            this.cboDetectPSMViewCount.Size = new System.Drawing.Size(50, 29);
            this.cboDetectPSMViewCount.TabIndex = 12;
            // 
            // cboDetectPSMSplitUnit
            // 
            this.cboDetectPSMSplitUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetectPSMSplitUnit.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboDetectPSMSplitUnit.FormattingEnabled = true;
            this.cboDetectPSMSplitUnit.Location = new System.Drawing.Point(572, 6);
            this.cboDetectPSMSplitUnit.Name = "cboDetectPSMSplitUnit";
            this.cboDetectPSMSplitUnit.Size = new System.Drawing.Size(56, 29);
            this.cboDetectPSMSplitUnit.TabIndex = 11;
            this.cboDetectPSMSplitUnit.SelectedIndexChanged += new System.EventHandler(this.cboDetectPSMSplitUnit_SelectedIndexChanged);
            // 
            // btnDetectPSMEndDate
            // 
            this.btnDetectPSMEndDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnDetectPSMEndDate.Location = new System.Drawing.Point(237, 6);
            this.btnDetectPSMEndDate.Name = "btnDetectPSMEndDate";
            this.btnDetectPSMEndDate.Size = new System.Drawing.Size(122, 30);
            this.btnDetectPSMEndDate.TabIndex = 9;
            this.btnDetectPSMEndDate.Text = "끝 일";
            this.btnDetectPSMEndDate.UseVisualStyleBackColor = true;
            this.btnDetectPSMEndDate.Click += new System.EventHandler(this.btnDetectPSMEndDate_Click);
            // 
            // btnDetectPSMStartDate
            // 
            this.btnDetectPSMStartDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnDetectPSMStartDate.Location = new System.Drawing.Point(110, 6);
            this.btnDetectPSMStartDate.Name = "btnDetectPSMStartDate";
            this.btnDetectPSMStartDate.Size = new System.Drawing.Size(121, 30);
            this.btnDetectPSMStartDate.TabIndex = 9;
            this.btnDetectPSMStartDate.Text = "시작 일";
            this.btnDetectPSMStartDate.UseVisualStyleBackColor = true;
            this.btnDetectPSMStartDate.Click += new System.EventHandler(this.btnDetectPSMStartDate_Click);
            // 
            // cboDetectPSMLatelyDate
            // 
            this.cboDetectPSMLatelyDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetectPSMLatelyDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboDetectPSMLatelyDate.FormattingEnabled = true;
            this.cboDetectPSMLatelyDate.Location = new System.Drawing.Point(381, 6);
            this.cboDetectPSMLatelyDate.Name = "cboDetectPSMLatelyDate";
            this.cboDetectPSMLatelyDate.Size = new System.Drawing.Size(115, 29);
            this.cboDetectPSMLatelyDate.TabIndex = 8;
            this.cboDetectPSMLatelyDate.SelectedIndexChanged += new System.EventHandler(this.cboDetectPSMLatelyDate_SelectedIndexChanged);
            // 
            // lblDetectPSMDate
            // 
            this.lblDetectPSMDate.AutoSize = true;
            this.lblDetectPSMDate.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDetectPSMDate.Location = new System.Drawing.Point(19, 10);
            this.lblDetectPSMDate.Name = "lblDetectPSMDate";
            this.lblDetectPSMDate.Size = new System.Drawing.Size(74, 21);
            this.lblDetectPSMDate.TabIndex = 5;
            this.lblDetectPSMDate.Text = "기간선택";
            // 
            // btnDetectPSMSelectZone
            // 
            this.btnDetectPSMSelectZone.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnDetectPSMSelectZone.Location = new System.Drawing.Point(1847, 5);
            this.btnDetectPSMSelectZone.Name = "btnDetectPSMSelectZone";
            this.btnDetectPSMSelectZone.Size = new System.Drawing.Size(52, 30);
            this.btnDetectPSMSelectZone.TabIndex = 4;
            this.btnDetectPSMSelectZone.Text = "선택";
            this.btnDetectPSMSelectZone.UseVisualStyleBackColor = true;
            this.btnDetectPSMSelectZone.Click += new System.EventHandler(this.btnDetectPSMSelectZone_Click);
            // 
            // cboDetectPSMBuilding
            // 
            this.cboDetectPSMBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetectPSMBuilding.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboDetectPSMBuilding.FormattingEnabled = true;
            this.cboDetectPSMBuilding.Location = new System.Drawing.Point(1504, 5);
            this.cboDetectPSMBuilding.Name = "cboDetectPSMBuilding";
            this.cboDetectPSMBuilding.Size = new System.Drawing.Size(337, 29);
            this.cboDetectPSMBuilding.TabIndex = 3;
            this.cboDetectPSMBuilding.SelectedIndexChanged += new System.EventHandler(this.cboDetectPSMBuilding_SelectedIndexChanged);
            // 
            // lblDetectPSMSelectZone
            // 
            this.lblDetectPSMSelectZone.AutoSize = true;
            this.lblDetectPSMSelectZone.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblDetectPSMSelectZone.Location = new System.Drawing.Point(1440, 9);
            this.lblDetectPSMSelectZone.Name = "lblDetectPSMSelectZone";
            this.lblDetectPSMSelectZone.Size = new System.Drawing.Size(58, 21);
            this.lblDetectPSMSelectZone.TabIndex = 2;
            this.lblDetectPSMSelectZone.Text = "시설명";
            // 
            // labelDetectPSMDateFormat
            // 
            this.labelDetectPSMDateFormat.AutoSize = true;
            this.labelDetectPSMDateFormat.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.labelDetectPSMDateFormat.Location = new System.Drawing.Point(642, 10);
            this.labelDetectPSMDateFormat.Name = "labelDetectPSMDateFormat";
            this.labelDetectPSMDateFormat.Size = new System.Drawing.Size(74, 21);
            this.labelDetectPSMDateFormat.TabIndex = 13;
            this.labelDetectPSMDateFormat.Text = "날짜형식";
            // 
            // pnSMSPSM
            // 
            this.pnSMSPSM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnSMSPSM.BackColor = System.Drawing.Color.Transparent;
            this.pnSMSPSM.BackgroundImage = global::SDMS.Properties.Resources.HToolbar_bkgnd;
            this.pnSMSPSM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnSMSPSM.Controls.Add(this.btnSMSPSMEndDate);
            this.pnSMSPSM.Controls.Add(this.btnSMSPSMStartDate);
            this.pnSMSPSM.Controls.Add(this.cboSMSPSMLatelyDate);
            this.pnSMSPSM.Controls.Add(this.lblSMSPSMDate);
            this.pnSMSPSM.Controls.Add(this.btnSMSPSMSelectZone);
            this.pnSMSPSM.Controls.Add(this.cboSMSPSMBuilding);
            this.pnSMSPSM.Controls.Add(this.lblSMSPSMSelectZone);
            this.pnSMSPSM.Location = new System.Drawing.Point(2, 545);
            this.pnSMSPSM.Name = "pnSMSPSM";
            this.pnSMSPSM.Size = new System.Drawing.Size(1915, 48);
            this.pnSMSPSM.TabIndex = 15;
            // 
            // btnSMSPSMEndDate
            // 
            this.btnSMSPSMEndDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnSMSPSMEndDate.Location = new System.Drawing.Point(237, 6);
            this.btnSMSPSMEndDate.Name = "btnSMSPSMEndDate";
            this.btnSMSPSMEndDate.Size = new System.Drawing.Size(122, 30);
            this.btnSMSPSMEndDate.TabIndex = 9;
            this.btnSMSPSMEndDate.Text = "끝 일";
            this.btnSMSPSMEndDate.UseVisualStyleBackColor = true;
            this.btnSMSPSMEndDate.Click += new System.EventHandler(this.btnSMSPSMEndDate_Click);
            // 
            // btnSMSPSMStartDate
            // 
            this.btnSMSPSMStartDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnSMSPSMStartDate.Location = new System.Drawing.Point(110, 6);
            this.btnSMSPSMStartDate.Name = "btnSMSPSMStartDate";
            this.btnSMSPSMStartDate.Size = new System.Drawing.Size(121, 30);
            this.btnSMSPSMStartDate.TabIndex = 9;
            this.btnSMSPSMStartDate.Text = "시작 일";
            this.btnSMSPSMStartDate.UseVisualStyleBackColor = true;
            this.btnSMSPSMStartDate.Click += new System.EventHandler(this.btnSMSPSMStartDate_Click);
            // 
            // cboSMSPSMLatelyDate
            // 
            this.cboSMSPSMLatelyDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSMSPSMLatelyDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboSMSPSMLatelyDate.FormattingEnabled = true;
            this.cboSMSPSMLatelyDate.Location = new System.Drawing.Point(381, 6);
            this.cboSMSPSMLatelyDate.Name = "cboSMSPSMLatelyDate";
            this.cboSMSPSMLatelyDate.Size = new System.Drawing.Size(115, 29);
            this.cboSMSPSMLatelyDate.TabIndex = 8;
            this.cboSMSPSMLatelyDate.SelectedIndexChanged += new System.EventHandler(this.cboSMSPSMLatelyDate_SelectedIndexChanged);
            // 
            // lblSMSPSMDate
            // 
            this.lblSMSPSMDate.AutoSize = true;
            this.lblSMSPSMDate.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSMSPSMDate.Location = new System.Drawing.Point(19, 10);
            this.lblSMSPSMDate.Name = "lblSMSPSMDate";
            this.lblSMSPSMDate.Size = new System.Drawing.Size(74, 21);
            this.lblSMSPSMDate.TabIndex = 5;
            this.lblSMSPSMDate.Text = "기간선택";
            // 
            // btnSMSPSMSelectZone
            // 
            this.btnSMSPSMSelectZone.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnSMSPSMSelectZone.Location = new System.Drawing.Point(1847, 6);
            this.btnSMSPSMSelectZone.Name = "btnSMSPSMSelectZone";
            this.btnSMSPSMSelectZone.Size = new System.Drawing.Size(52, 30);
            this.btnSMSPSMSelectZone.TabIndex = 4;
            this.btnSMSPSMSelectZone.Text = "선택";
            this.btnSMSPSMSelectZone.UseVisualStyleBackColor = true;
            this.btnSMSPSMSelectZone.Click += new System.EventHandler(this.btnSMSPSMSelectZone_Click);
            // 
            // cboSMSPSMBuilding
            // 
            this.cboSMSPSMBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSMSPSMBuilding.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboSMSPSMBuilding.FormattingEnabled = true;
            this.cboSMSPSMBuilding.Location = new System.Drawing.Point(1504, 6);
            this.cboSMSPSMBuilding.Name = "cboSMSPSMBuilding";
            this.cboSMSPSMBuilding.Size = new System.Drawing.Size(337, 29);
            this.cboSMSPSMBuilding.TabIndex = 3;
            this.cboSMSPSMBuilding.SelectedIndexChanged += new System.EventHandler(this.cboSMSPSMBuilding_SelectedIndexChanged);
            // 
            // lblSMSPSMSelectZone
            // 
            this.lblSMSPSMSelectZone.AutoSize = true;
            this.lblSMSPSMSelectZone.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblSMSPSMSelectZone.Location = new System.Drawing.Point(1440, 10);
            this.lblSMSPSMSelectZone.Name = "lblSMSPSMSelectZone";
            this.lblSMSPSMSelectZone.Size = new System.Drawing.Size(58, 21);
            this.lblSMSPSMSelectZone.TabIndex = 2;
            this.lblSMSPSMSelectZone.Text = "시설명";
            // 
            // pnActionPSM
            // 
            this.pnActionPSM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnActionPSM.BackColor = System.Drawing.Color.Transparent;
            this.pnActionPSM.BackgroundImage = global::SDMS.Properties.Resources.HToolbar_bkgnd;
            this.pnActionPSM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnActionPSM.Controls.Add(this.btnReactionPSMSelectDisaster);
            this.pnActionPSM.Controls.Add(this.cboActionPSMSearchType);
            this.pnActionPSM.Controls.Add(this.label1);
            this.pnActionPSM.Controls.Add(this.cboActionPSMSelect);
            this.pnActionPSM.Controls.Add(this.btnActionPSMEndDate);
            this.pnActionPSM.Controls.Add(this.btnActionPSMStartDate);
            this.pnActionPSM.Controls.Add(this.cboActionPSMEndTime);
            this.pnActionPSM.Controls.Add(this.cboActionPSMStartTime);
            this.pnActionPSM.Controls.Add(this.lblActionPSMSelect);
            this.pnActionPSM.Controls.Add(this.lblActionPSMDate);
            this.pnActionPSM.Location = new System.Drawing.Point(2, 593);
            this.pnActionPSM.Name = "pnActionPSM";
            this.pnActionPSM.Size = new System.Drawing.Size(1915, 48);
            this.pnActionPSM.TabIndex = 13;
            // 
            // btnReactionPSMSelectDisaster
            // 
            this.btnReactionPSMSelectDisaster.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnReactionPSMSelectDisaster.Location = new System.Drawing.Point(1011, 6);
            this.btnReactionPSMSelectDisaster.Name = "btnReactionPSMSelectDisaster";
            this.btnReactionPSMSelectDisaster.Size = new System.Drawing.Size(52, 30);
            this.btnReactionPSMSelectDisaster.TabIndex = 13;
            this.btnReactionPSMSelectDisaster.Text = "선택";
            this.btnReactionPSMSelectDisaster.UseVisualStyleBackColor = true;
            // 
            // cboActionPSMSearchType
            // 
            this.cboActionPSMSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboActionPSMSearchType.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboActionPSMSearchType.FormattingEnabled = true;
            this.cboActionPSMSearchType.Location = new System.Drawing.Point(796, 6);
            this.cboActionPSMSearchType.Name = "cboActionPSMSearchType";
            this.cboActionPSMSearchType.Size = new System.Drawing.Size(209, 29);
            this.cboActionPSMSearchType.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label1.Location = new System.Drawing.Point(501, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 21);
            this.label1.TabIndex = 11;
            this.label1.Text = "~";
            // 
            // cboActionPSMSelect
            // 
            this.cboActionPSMSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboActionPSMSelect.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboActionPSMSelect.FormattingEnabled = true;
            this.cboActionPSMSelect.Location = new System.Drawing.Point(1319, 6);
            this.cboActionPSMSelect.MaxDropDownItems = 20;
            this.cboActionPSMSelect.Name = "cboActionPSMSelect";
            this.cboActionPSMSelect.Size = new System.Drawing.Size(580, 29);
            this.cboActionPSMSelect.TabIndex = 8;
            // 
            // btnActionPSMEndDate
            // 
            this.btnActionPSMEndDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnActionPSMEndDate.Location = new System.Drawing.Point(237, 6);
            this.btnActionPSMEndDate.Name = "btnActionPSMEndDate";
            this.btnActionPSMEndDate.Size = new System.Drawing.Size(121, 30);
            this.btnActionPSMEndDate.TabIndex = 9;
            this.btnActionPSMEndDate.Text = "끝 일";
            this.btnActionPSMEndDate.UseVisualStyleBackColor = true;
            this.btnActionPSMEndDate.Click += new System.EventHandler(this.btnActionPSMEndDate_Click);
            // 
            // btnActionPSMStartDate
            // 
            this.btnActionPSMStartDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnActionPSMStartDate.Location = new System.Drawing.Point(110, 6);
            this.btnActionPSMStartDate.Name = "btnActionPSMStartDate";
            this.btnActionPSMStartDate.Size = new System.Drawing.Size(121, 30);
            this.btnActionPSMStartDate.TabIndex = 9;
            this.btnActionPSMStartDate.Text = "시작 일";
            this.btnActionPSMStartDate.UseVisualStyleBackColor = true;
            this.btnActionPSMStartDate.Click += new System.EventHandler(this.btnActionPSMStartDate_Click);
            // 
            // cboActionPSMEndTime
            // 
            this.cboActionPSMEndTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboActionPSMEndTime.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboActionPSMEndTime.FormattingEnabled = true;
            this.cboActionPSMEndTime.Location = new System.Drawing.Point(528, 6);
            this.cboActionPSMEndTime.Name = "cboActionPSMEndTime";
            this.cboActionPSMEndTime.Size = new System.Drawing.Size(115, 29);
            this.cboActionPSMEndTime.TabIndex = 8;
            this.cboActionPSMEndTime.SelectedIndexChanged += new System.EventHandler(this.cboActionPSMEndTime_SelectedIndexChanged);
            // 
            // cboActionPSMStartTime
            // 
            this.cboActionPSMStartTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboActionPSMStartTime.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboActionPSMStartTime.FormattingEnabled = true;
            this.cboActionPSMStartTime.Location = new System.Drawing.Point(381, 6);
            this.cboActionPSMStartTime.Name = "cboActionPSMStartTime";
            this.cboActionPSMStartTime.Size = new System.Drawing.Size(115, 29);
            this.cboActionPSMStartTime.TabIndex = 8;
            this.cboActionPSMStartTime.SelectedIndexChanged += new System.EventHandler(this.cboActionPSMStartTime_SelectedIndexChanged);
            // 
            // lblActionPSMSelect
            // 
            this.lblActionPSMSelect.AutoSize = true;
            this.lblActionPSMSelect.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblActionPSMSelect.Location = new System.Drawing.Point(1242, 10);
            this.lblActionPSMSelect.Name = "lblActionPSMSelect";
            this.lblActionPSMSelect.Size = new System.Drawing.Size(74, 21);
            this.lblActionPSMSelect.TabIndex = 5;
            this.lblActionPSMSelect.Text = "누출선택";
            // 
            // lblActionPSMDate
            // 
            this.lblActionPSMDate.AutoSize = true;
            this.lblActionPSMDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblActionPSMDate.Location = new System.Drawing.Point(19, 10);
            this.lblActionPSMDate.Name = "lblActionPSMDate";
            this.lblActionPSMDate.Size = new System.Drawing.Size(74, 21);
            this.lblActionPSMDate.TabIndex = 5;
            this.lblActionPSMDate.Text = "기간선택";
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.Transparent;
            this.panelLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelLeft.BackgroundImage")));
            this.panelLeft.Controls.Add(this.labelSaveHome);
            this.panelLeft.Controls.Add(this.btnLayerCCTVDisconnected);
            this.panelLeft.Controls.Add(this.btnSaveHome);
            this.panelLeft.Controls.Add(this.btnLayerLowCCTV);
            this.panelLeft.Controls.Add(this.labelCCTVDisconnected);
            this.panelLeft.Controls.Add(this.labelCCTVLow);
            this.panelLeft.Controls.Add(this.labelNotice);
            this.panelLeft.Controls.Add(this.labelBuildingText);
            this.panelLeft.Controls.Add(this.labelFR);
            this.panelLeft.Controls.Add(this.labelFA);
            this.panelLeft.Controls.Add(this.labelHD);
            this.panelLeft.Controls.Add(this.labelFE);
            this.panelLeft.Controls.Add(this.labelCCTV);
            this.panelLeft.Controls.Add(this.labelPump);
            this.panelLeft.Controls.Add(this.labelCooler);
            this.panelLeft.Controls.Add(this.labelFire);
            this.panelLeft.Controls.Add(this.btnLayerNotice);
            this.panelLeft.Controls.Add(this.btnLayerBuildingText);
            this.panelLeft.Controls.Add(this.btnLayerFR);
            this.panelLeft.Controls.Add(this.btnLayerFA);
            this.panelLeft.Controls.Add(this.btnLayerHD);
            this.panelLeft.Controls.Add(this.btnLayerFE);
            this.panelLeft.Controls.Add(this.btnLayerCCTV);
            this.panelLeft.Controls.Add(this.btnLayerPump);
            this.panelLeft.Controls.Add(this.btnLayerSpringCooler);
            this.panelLeft.Controls.Add(this.btnLayerFire);
            this.panelLeft.Location = new System.Drawing.Point(2, 648);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(60, 734);
            this.panelLeft.TabIndex = 3;
            // 
            // labelSaveHome
            // 
            this.labelSaveHome.AutoSize = true;
            this.labelSaveHome.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelSaveHome.Location = new System.Drawing.Point(6, 764);
            this.labelSaveHome.Name = "labelSaveHome";
            this.labelSaveHome.Size = new System.Drawing.Size(51, 13);
            this.labelSaveHome.TabIndex = 13;
            this.labelSaveHome.Text = "화면설정";
            this.labelSaveHome.Visible = false;
            // 
            // btnLayerCCTVDisconnected
            // 
            this.btnLayerCCTVDisconnected.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Normal;
            this.btnLayerCCTVDisconnected.Location = new System.Drawing.Point(6, 325);
            this.btnLayerCCTVDisconnected.Name = "btnLayerCCTVDisconnected";
            this.btnLayerCCTVDisconnected.Size = new System.Drawing.Size(48, 48);
            this.btnLayerCCTVDisconnected.TabIndex = 6;
            this.btnLayerCCTVDisconnected.UseVisualStyleBackColor = true;
            this.btnLayerCCTVDisconnected.Visible = false;
            this.btnLayerCCTVDisconnected.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnSaveHome
            // 
            this.btnSaveHome.BackgroundImage = global::SDMS.Properties.Resources.Show_List_Checked;
            this.btnSaveHome.Location = new System.Drawing.Point(6, 720);
            this.btnSaveHome.Name = "btnSaveHome";
            this.btnSaveHome.Size = new System.Drawing.Size(48, 48);
            this.btnSaveHome.TabIndex = 12;
            this.btnSaveHome.UseVisualStyleBackColor = true;
            this.btnSaveHome.Click += new System.EventHandler(this.btnSaveHome_Click);
            this.btnSaveHome.Leave += new System.EventHandler(this.btnSaveHome_Leave);
            // 
            // btnLayerLowCCTV
            // 
            this.btnLayerLowCCTV.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Normal;
            this.btnLayerLowCCTV.Location = new System.Drawing.Point(6, 260);
            this.btnLayerLowCCTV.Name = "btnLayerLowCCTV";
            this.btnLayerLowCCTV.Size = new System.Drawing.Size(48, 48);
            this.btnLayerLowCCTV.TabIndex = 6;
            this.btnLayerLowCCTV.UseVisualStyleBackColor = true;
            this.btnLayerLowCCTV.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // labelCCTVDisconnected
            // 
            this.labelCCTVDisconnected.AutoSize = true;
            this.labelCCTVDisconnected.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelCCTVDisconnected.Location = new System.Drawing.Point(8, 374);
            this.labelCCTVDisconnected.Name = "labelCCTVDisconnected";
            this.labelCCTVDisconnected.Size = new System.Drawing.Size(46, 13);
            this.labelCCTVDisconnected.TabIndex = 5;
            this.labelCCTVDisconnected.Text = "CCTV-X";
            // 
            // labelCCTVLow
            // 
            this.labelCCTVLow.AutoSize = true;
            this.labelCCTVLow.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelCCTVLow.Location = new System.Drawing.Point(9, 309);
            this.labelCCTVLow.Name = "labelCCTVLow";
            this.labelCCTVLow.Size = new System.Drawing.Size(44, 13);
            this.labelCCTVLow.TabIndex = 5;
            this.labelCCTVLow.Text = "CCTV-L";
            // 
            // labelNotice
            // 
            this.labelNotice.AutoSize = true;
            this.labelNotice.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelNotice.Location = new System.Drawing.Point(5, 764);
            this.labelNotice.Name = "labelNotice";
            this.labelNotice.Size = new System.Drawing.Size(49, 13);
            this.labelNotice.TabIndex = 1;
            this.labelNotice.Text = "   알람  ";
            this.labelNotice.Visible = false;
            // 
            // labelBuildingText
            // 
            this.labelBuildingText.AutoSize = true;
            this.labelBuildingText.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelBuildingText.Location = new System.Drawing.Point(5, 699);
            this.labelBuildingText.Name = "labelBuildingText";
            this.labelBuildingText.Size = new System.Drawing.Size(51, 13);
            this.labelBuildingText.TabIndex = 1;
            this.labelBuildingText.Text = "상세정보";
            this.labelBuildingText.Visible = false;
            // 
            // labelFR
            // 
            this.labelFR.AutoSize = true;
            this.labelFR.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelFR.Location = new System.Drawing.Point(9, 634);
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
            this.labelFA.Location = new System.Drawing.Point(9, 569);
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
            this.labelHD.Location = new System.Drawing.Point(9, 504);
            this.labelHD.Name = "labelHD";
            this.labelHD.Size = new System.Drawing.Size(40, 13);
            this.labelHD.TabIndex = 1;
            this.labelHD.Text = "소화전";
            // 
            // labelFE
            // 
            this.labelFE.AutoSize = true;
            this.labelFE.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelFE.Location = new System.Drawing.Point(10, 439);
            this.labelFE.Name = "labelFE";
            this.labelFE.Size = new System.Drawing.Size(40, 13);
            this.labelFE.TabIndex = 1;
            this.labelFE.Text = "소화기";
            // 
            // labelCCTV
            // 
            this.labelCCTV.AutoSize = true;
            this.labelCCTV.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelCCTV.Location = new System.Drawing.Point(12, 244);
            this.labelCCTV.Name = "labelCCTV";
            this.labelCCTV.Size = new System.Drawing.Size(34, 13);
            this.labelCCTV.TabIndex = 1;
            this.labelCCTV.Text = "CCTV";
            // 
            // labelPump
            // 
            this.labelPump.AutoSize = true;
            this.labelPump.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelPump.Location = new System.Drawing.Point(5, 179);
            this.labelPump.Name = "labelPump";
            this.labelPump.Size = new System.Drawing.Size(51, 13);
            this.labelPump.TabIndex = 1;
            this.labelPump.Text = "펌프압력";
            // 
            // labelCooler
            // 
            this.labelCooler.AutoSize = true;
            this.labelCooler.Font = new System.Drawing.Font("맑은 고딕", 7F);
            this.labelCooler.Location = new System.Drawing.Point(2, 114);
            this.labelCooler.Name = "labelCooler";
            this.labelCooler.Size = new System.Drawing.Size(55, 12);
            this.labelCooler.TabIndex = 1;
            this.labelCooler.Text = "스프링쿨러";
            // 
            // labelFire
            // 
            this.labelFire.AutoSize = true;
            this.labelFire.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.labelFire.Location = new System.Drawing.Point(6, 49);
            this.labelFire.Name = "labelFire";
            this.labelFire.Size = new System.Drawing.Size(51, 13);
            this.labelFire.TabIndex = 1;
            this.labelFire.Text = "화재탐지";
            // 
            // btnLayerNotice
            // 
            this.btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Normal;
            this.btnLayerNotice.Location = new System.Drawing.Point(6, 715);
            this.btnLayerNotice.Name = "btnLayerNotice";
            this.btnLayerNotice.Size = new System.Drawing.Size(48, 48);
            this.btnLayerNotice.TabIndex = 0;
            this.btnLayerNotice.UseVisualStyleBackColor = true;
            this.btnLayerNotice.Visible = false;
            this.btnLayerNotice.Click += new System.EventHandler(this.btnLayerNotice_Click);
            // 
            // btnLayerBuildingText
            // 
            this.btnLayerBuildingText.BackgroundImage = global::SDMS.Properties.Resources.Layer_Building_Normal;
            this.btnLayerBuildingText.Location = new System.Drawing.Point(6, 650);
            this.btnLayerBuildingText.Name = "btnLayerBuildingText";
            this.btnLayerBuildingText.Size = new System.Drawing.Size(48, 48);
            this.btnLayerBuildingText.TabIndex = 0;
            this.btnLayerBuildingText.UseVisualStyleBackColor = true;
            this.btnLayerBuildingText.Visible = false;
            this.btnLayerBuildingText.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerFR
            // 
            this.btnLayerFR.BackgroundImage = global::SDMS.Properties.Resources.Layer_FR_Normal;
            this.btnLayerFR.Location = new System.Drawing.Point(6, 585);
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
            this.btnLayerFA.Location = new System.Drawing.Point(6, 520);
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
            this.btnLayerHD.Location = new System.Drawing.Point(6, 455);
            this.btnLayerHD.Name = "btnLayerHD";
            this.btnLayerHD.Size = new System.Drawing.Size(48, 48);
            this.btnLayerHD.TabIndex = 0;
            this.btnLayerHD.UseVisualStyleBackColor = true;
            this.btnLayerHD.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerFE
            // 
            this.btnLayerFE.BackgroundImage = global::SDMS.Properties.Resources.Layer_FE_Normal;
            this.btnLayerFE.Location = new System.Drawing.Point(6, 390);
            this.btnLayerFE.Name = "btnLayerFE";
            this.btnLayerFE.Size = new System.Drawing.Size(48, 48);
            this.btnLayerFE.TabIndex = 0;
            this.btnLayerFE.UseVisualStyleBackColor = true;
            this.btnLayerFE.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerCCTV
            // 
            this.btnLayerCCTV.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Normal;
            this.btnLayerCCTV.Location = new System.Drawing.Point(6, 195);
            this.btnLayerCCTV.Name = "btnLayerCCTV";
            this.btnLayerCCTV.Size = new System.Drawing.Size(48, 48);
            this.btnLayerCCTV.TabIndex = 0;
            this.btnLayerCCTV.UseVisualStyleBackColor = true;
            this.btnLayerCCTV.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerPump
            // 
            this.btnLayerPump.BackgroundImage = global::SDMS.Properties.Resources.Layer_Pump_Normal;
            this.btnLayerPump.Location = new System.Drawing.Point(6, 130);
            this.btnLayerPump.Name = "btnLayerPump";
            this.btnLayerPump.Size = new System.Drawing.Size(48, 48);
            this.btnLayerPump.TabIndex = 0;
            this.btnLayerPump.UseVisualStyleBackColor = true;
            this.btnLayerPump.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerSpringCooler
            // 
            this.btnLayerSpringCooler.BackgroundImage = global::SDMS.Properties.Resources.Layer_SpringCooler_Normal;
            this.btnLayerSpringCooler.Location = new System.Drawing.Point(6, 65);
            this.btnLayerSpringCooler.Name = "btnLayerSpringCooler";
            this.btnLayerSpringCooler.Size = new System.Drawing.Size(48, 48);
            this.btnLayerSpringCooler.TabIndex = 0;
            this.btnLayerSpringCooler.UseVisualStyleBackColor = true;
            this.btnLayerSpringCooler.Click += new System.EventHandler(this.OnClickLayerToolBarButton);
            // 
            // btnLayerFire
            // 
            this.btnLayerFire.BackgroundImage = global::SDMS.Properties.Resources.Layer_Fire_Normal;
            this.btnLayerFire.Location = new System.Drawing.Point(6, 0);
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
            this.panelReactionHistory.Controls.Add(this.btnReactionSelectDisaster);
            this.panelReactionHistory.Controls.Add(this.btnReactionIntrusionSelectDisaster);
            this.panelReactionHistory.Controls.Add(this.react_cboSearchType);
            this.panelReactionHistory.Controls.Add(this.react_cboSearchTypeIntrusion);
            this.panelReactionHistory.Controls.Add(this.label14);
            this.panelReactionHistory.Controls.Add(this.cboFireSelect);
            this.panelReactionHistory.Controls.Add(this.cboActionIntrusionSelect);
            this.panelReactionHistory.Controls.Add(this.react_btnEndDate);
            this.panelReactionHistory.Controls.Add(this.react_btnStartDate);
            this.panelReactionHistory.Controls.Add(this.react_cboEndTime);
            this.panelReactionHistory.Controls.Add(this.react_cboStartTime);
            this.panelReactionHistory.Controls.Add(this.lblFireSelect);
            this.panelReactionHistory.Controls.Add(this.lblIntrusionSelect);
            this.panelReactionHistory.Controls.Add(this.lblReactionDate);
            this.panelReactionHistory.Location = new System.Drawing.Point(2, 545);
            this.panelReactionHistory.Name = "panelReactionHistory";
            this.panelReactionHistory.Size = new System.Drawing.Size(1915, 48);
            this.panelReactionHistory.TabIndex = 2;
            // 
            // btnReactionSelectDisaster
            // 
            this.btnReactionSelectDisaster.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnReactionSelectDisaster.Location = new System.Drawing.Point(1011, 6);
            this.btnReactionSelectDisaster.Name = "btnReactionSelectDisaster";
            this.btnReactionSelectDisaster.Size = new System.Drawing.Size(52, 30);
            this.btnReactionSelectDisaster.TabIndex = 13;
            this.btnReactionSelectDisaster.Text = "선택";
            this.btnReactionSelectDisaster.UseVisualStyleBackColor = true;
            // 
            // btnReactionIntrusionSelectDisaster
            // 
            this.btnReactionIntrusionSelectDisaster.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnReactionIntrusionSelectDisaster.Location = new System.Drawing.Point(1011, 6);
            this.btnReactionIntrusionSelectDisaster.Name = "btnReactionIntrusionSelectDisaster";
            this.btnReactionIntrusionSelectDisaster.Size = new System.Drawing.Size(52, 30);
            this.btnReactionIntrusionSelectDisaster.TabIndex = 13;
            this.btnReactionIntrusionSelectDisaster.Text = "선택";
            this.btnReactionIntrusionSelectDisaster.UseVisualStyleBackColor = true;
            // 
            // react_cboSearchType
            // 
            this.react_cboSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.react_cboSearchType.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.react_cboSearchType.FormattingEnabled = true;
            this.react_cboSearchType.Location = new System.Drawing.Point(796, 6);
            this.react_cboSearchType.Name = "react_cboSearchType";
            this.react_cboSearchType.Size = new System.Drawing.Size(209, 29);
            this.react_cboSearchType.TabIndex = 12;
            // 
            // react_cboSearchTypeIntrusion
            // 
            this.react_cboSearchTypeIntrusion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.react_cboSearchTypeIntrusion.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.react_cboSearchTypeIntrusion.FormattingEnabled = true;
            this.react_cboSearchTypeIntrusion.Location = new System.Drawing.Point(796, 6);
            this.react_cboSearchTypeIntrusion.Name = "react_cboSearchTypeIntrusion";
            this.react_cboSearchTypeIntrusion.Size = new System.Drawing.Size(209, 29);
            this.react_cboSearchTypeIntrusion.TabIndex = 12;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label14.Location = new System.Drawing.Point(501, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(21, 21);
            this.label14.TabIndex = 11;
            this.label14.Text = "~";
            // 
            // cboFireSelect
            // 
            this.cboFireSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFireSelect.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboFireSelect.FormattingEnabled = true;
            this.cboFireSelect.Location = new System.Drawing.Point(1319, 6);
            this.cboFireSelect.MaxDropDownItems = 20;
            this.cboFireSelect.Name = "cboFireSelect";
            this.cboFireSelect.Size = new System.Drawing.Size(580, 29);
            this.cboFireSelect.TabIndex = 8;
            // 
            // cboActionIntrusionSelect
            // 
            this.cboActionIntrusionSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboActionIntrusionSelect.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.cboActionIntrusionSelect.FormattingEnabled = true;
            this.cboActionIntrusionSelect.Location = new System.Drawing.Point(1319, 6);
            this.cboActionIntrusionSelect.MaxDropDownItems = 20;
            this.cboActionIntrusionSelect.Name = "cboActionIntrusionSelect";
            this.cboActionIntrusionSelect.Size = new System.Drawing.Size(580, 29);
            this.cboActionIntrusionSelect.TabIndex = 8;
            // 
            // react_btnEndDate
            // 
            this.react_btnEndDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.react_btnEndDate.Location = new System.Drawing.Point(237, 6);
            this.react_btnEndDate.Name = "react_btnEndDate";
            this.react_btnEndDate.Size = new System.Drawing.Size(121, 30);
            this.react_btnEndDate.TabIndex = 9;
            this.react_btnEndDate.Text = "끝 일";
            this.react_btnEndDate.UseVisualStyleBackColor = true;
            this.react_btnEndDate.Click += new System.EventHandler(this.react_btnEndDate_Click);
            // 
            // react_btnStartDate
            // 
            this.react_btnStartDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.react_btnStartDate.Location = new System.Drawing.Point(110, 6);
            this.react_btnStartDate.Name = "react_btnStartDate";
            this.react_btnStartDate.Size = new System.Drawing.Size(121, 30);
            this.react_btnStartDate.TabIndex = 9;
            this.react_btnStartDate.Text = "시작 일";
            this.react_btnStartDate.UseVisualStyleBackColor = true;
            this.react_btnStartDate.TextChanged += new System.EventHandler(this.react_btnStartDate_TextChanged);
            this.react_btnStartDate.Click += new System.EventHandler(this.react_btnStartDate_Click);
            // 
            // react_cboEndTime
            // 
            this.react_cboEndTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.react_cboEndTime.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.react_cboEndTime.FormattingEnabled = true;
            this.react_cboEndTime.Location = new System.Drawing.Point(528, 6);
            this.react_cboEndTime.Name = "react_cboEndTime";
            this.react_cboEndTime.Size = new System.Drawing.Size(115, 29);
            this.react_cboEndTime.TabIndex = 8;
            this.react_cboEndTime.SelectedIndexChanged += new System.EventHandler(this.react_cboEndTime_SelectedIndexChanged);
            // 
            // react_cboStartTime
            // 
            this.react_cboStartTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.react_cboStartTime.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.react_cboStartTime.FormattingEnabled = true;
            this.react_cboStartTime.Location = new System.Drawing.Point(381, 6);
            this.react_cboStartTime.Name = "react_cboStartTime";
            this.react_cboStartTime.Size = new System.Drawing.Size(115, 29);
            this.react_cboStartTime.TabIndex = 8;
            this.react_cboStartTime.SelectedIndexChanged += new System.EventHandler(this.react_cboStartTime_SelectedIndexChanged);
            // 
            // lblFireSelect
            // 
            this.lblFireSelect.AutoSize = true;
            this.lblFireSelect.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblFireSelect.Location = new System.Drawing.Point(1242, 10);
            this.lblFireSelect.Name = "lblFireSelect";
            this.lblFireSelect.Size = new System.Drawing.Size(74, 21);
            this.lblFireSelect.TabIndex = 5;
            this.lblFireSelect.Text = "화재선택";
            // 
            // lblIntrusionSelect
            // 
            this.lblIntrusionSelect.AutoSize = true;
            this.lblIntrusionSelect.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblIntrusionSelect.Location = new System.Drawing.Point(1242, 10);
            this.lblIntrusionSelect.Name = "lblIntrusionSelect";
            this.lblIntrusionSelect.Size = new System.Drawing.Size(74, 21);
            this.lblIntrusionSelect.TabIndex = 5;
            this.lblIntrusionSelect.Text = "방범선택";
            // 
            // lblReactionDate
            // 
            this.lblReactionDate.AutoSize = true;
            this.lblReactionDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblReactionDate.Location = new System.Drawing.Point(19, 10);
            this.lblReactionDate.Name = "lblReactionDate";
            this.lblReactionDate.Size = new System.Drawing.Size(74, 21);
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
            this.panelProcessHistory.Controls.Add(this.btnDateFormat);
            this.panelProcessHistory.Controls.Add(this.nudSplitUnitDetail);
            this.panelProcessHistory.Controls.Add(this.lblSplitUnitDetail);
            this.panelProcessHistory.Controls.Add(this.lblViewCount);
            this.panelProcessHistory.Controls.Add(this.labelDetectDateFormat);
            this.panelProcessHistory.Controls.Add(this.lblSplitUnit);
            this.panelProcessHistory.Controls.Add(this.proc_cboViewCount);
            this.panelProcessHistory.Controls.Add(this.proc_cboSplitUnit);
            this.panelProcessHistory.Controls.Add(this.proc_btnEndDate);
            this.panelProcessHistory.Controls.Add(this.proc_btnStartDate);
            this.panelProcessHistory.Controls.Add(this.proc_cboLatelyDate);
            this.panelProcessHistory.Controls.Add(this.lblProcessDate);
            this.panelProcessHistory.Controls.Add(this.proc_btnSelectZone);
            this.panelProcessHistory.Controls.Add(this.proc_cboFloor);
            this.panelProcessHistory.Controls.Add(this.proc_cboBuilding);
            this.panelProcessHistory.Controls.Add(this.proc_cboBuildingGroup);
            this.panelProcessHistory.Controls.Add(this.proc_lblSelectZone);
            this.panelProcessHistory.Location = new System.Drawing.Point(2, 545);
            this.panelProcessHistory.Name = "panelProcessHistory";
            this.panelProcessHistory.Size = new System.Drawing.Size(1915, 48);
            this.panelProcessHistory.TabIndex = 2;
            // 
            // btnDateFormat
            // 
            this.btnDateFormat.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.btnDateFormat.Location = new System.Drawing.Point(644, -36);
            this.btnDateFormat.Name = "btnDateFormat";
            this.btnDateFormat.Size = new System.Drawing.Size(82, 30);
            this.btnDateFormat.TabIndex = 17;
            this.btnDateFormat.Text = "날짜형식";
            this.btnDateFormat.UseVisualStyleBackColor = true;
            this.btnDateFormat.Click += new System.EventHandler(this.btnDateFormat_Click);
            // 
            // nudSplitUnitDetail
            // 
            this.nudSplitUnitDetail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudSplitUnitDetail.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.nudSplitUnitDetail.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.nudSplitUnitDetail.Location = new System.Drawing.Point(722, 8);
            this.nudSplitUnitDetail.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.nudSplitUnitDetail.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSplitUnitDetail.Name = "nudSplitUnitDetail";
            this.nudSplitUnitDetail.Size = new System.Drawing.Size(46, 29);
            this.nudSplitUnitDetail.TabIndex = 13;
            this.nudSplitUnitDetail.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudSplitUnitDetail.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblSplitUnitDetail
            // 
            this.lblSplitUnitDetail.AutoSize = true;
            this.lblSplitUnitDetail.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblSplitUnitDetail.Location = new System.Drawing.Point(774, 10);
            this.lblSplitUnitDetail.Name = "lblSplitUnitDetail";
            this.lblSplitUnitDetail.Size = new System.Drawing.Size(80, 21);
            this.lblSplitUnitDetail.TabIndex = 16;
            this.lblSplitUnitDetail.Text = "단위 마다";
            // 
            // lblViewCount
            // 
            this.lblViewCount.AutoSize = true;
            this.lblViewCount.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblViewCount.Location = new System.Drawing.Point(869, 10);
            this.lblViewCount.Name = "lblViewCount";
            this.lblViewCount.Size = new System.Drawing.Size(80, 21);
            this.lblViewCount.TabIndex = 14;
            this.lblViewCount.Text = "최대 표기";
            // 
            // labelDetectDateFormat
            // 
            this.labelDetectDateFormat.AutoSize = true;
            this.labelDetectDateFormat.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.labelDetectDateFormat.Location = new System.Drawing.Point(642, 10);
            this.labelDetectDateFormat.Name = "labelDetectDateFormat";
            this.labelDetectDateFormat.Size = new System.Drawing.Size(74, 21);
            this.labelDetectDateFormat.TabIndex = 13;
            this.labelDetectDateFormat.Text = "날짜형식";
            // 
            // lblSplitUnit
            // 
            this.lblSplitUnit.AutoSize = true;
            this.lblSplitUnit.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblSplitUnit.Location = new System.Drawing.Point(524, 10);
            this.lblSplitUnit.Name = "lblSplitUnit";
            this.lblSplitUnit.Size = new System.Drawing.Size(42, 21);
            this.lblSplitUnit.TabIndex = 13;
            this.lblSplitUnit.Text = "단위";
            // 
            // proc_cboViewCount
            // 
            this.proc_cboViewCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proc_cboViewCount.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.proc_cboViewCount.FormattingEnabled = true;
            this.proc_cboViewCount.Location = new System.Drawing.Point(955, 6);
            this.proc_cboViewCount.Name = "proc_cboViewCount";
            this.proc_cboViewCount.Size = new System.Drawing.Size(50, 29);
            this.proc_cboViewCount.TabIndex = 12;
            // 
            // proc_cboSplitUnit
            // 
            this.proc_cboSplitUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proc_cboSplitUnit.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.proc_cboSplitUnit.FormattingEnabled = true;
            this.proc_cboSplitUnit.Location = new System.Drawing.Point(572, 6);
            this.proc_cboSplitUnit.Name = "proc_cboSplitUnit";
            this.proc_cboSplitUnit.Size = new System.Drawing.Size(56, 29);
            this.proc_cboSplitUnit.TabIndex = 11;
            this.proc_cboSplitUnit.SelectedIndexChanged += new System.EventHandler(this.proc_cboSplitUnit_SelectedIndexChanged);
            // 
            // proc_btnEndDate
            // 
            this.proc_btnEndDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.proc_btnEndDate.Location = new System.Drawing.Point(237, 6);
            this.proc_btnEndDate.Name = "proc_btnEndDate";
            this.proc_btnEndDate.Size = new System.Drawing.Size(122, 30);
            this.proc_btnEndDate.TabIndex = 9;
            this.proc_btnEndDate.Text = "끝 일";
            this.proc_btnEndDate.UseVisualStyleBackColor = true;
            this.proc_btnEndDate.Click += new System.EventHandler(this.proc_btnEndDate_Click);
            // 
            // proc_btnStartDate
            // 
            this.proc_btnStartDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.proc_btnStartDate.Location = new System.Drawing.Point(110, 6);
            this.proc_btnStartDate.Name = "proc_btnStartDate";
            this.proc_btnStartDate.Size = new System.Drawing.Size(121, 30);
            this.proc_btnStartDate.TabIndex = 9;
            this.proc_btnStartDate.Text = "시작 일";
            this.proc_btnStartDate.UseVisualStyleBackColor = true;
            this.proc_btnStartDate.Click += new System.EventHandler(this.proc_btnStartDate_Click);
            // 
            // proc_cboLatelyDate
            // 
            this.proc_cboLatelyDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proc_cboLatelyDate.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.proc_cboLatelyDate.FormattingEnabled = true;
            this.proc_cboLatelyDate.Location = new System.Drawing.Point(381, 6);
            this.proc_cboLatelyDate.Name = "proc_cboLatelyDate";
            this.proc_cboLatelyDate.Size = new System.Drawing.Size(115, 29);
            this.proc_cboLatelyDate.TabIndex = 8;
            this.proc_cboLatelyDate.SelectedIndexChanged += new System.EventHandler(this.proc_cboLatelyDate_SelectedIndexChanged);
            // 
            // lblProcessDate
            // 
            this.lblProcessDate.AutoSize = true;
            this.lblProcessDate.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblProcessDate.Location = new System.Drawing.Point(19, 10);
            this.lblProcessDate.Name = "lblProcessDate";
            this.lblProcessDate.Size = new System.Drawing.Size(74, 21);
            this.lblProcessDate.TabIndex = 5;
            this.lblProcessDate.Text = "기간선택";
            // 
            // proc_btnSelectZone
            // 
            this.proc_btnSelectZone.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.proc_btnSelectZone.Location = new System.Drawing.Point(1847, 6);
            this.proc_btnSelectZone.Name = "proc_btnSelectZone";
            this.proc_btnSelectZone.Size = new System.Drawing.Size(52, 30);
            this.proc_btnSelectZone.TabIndex = 4;
            this.proc_btnSelectZone.Text = "선택";
            this.proc_btnSelectZone.UseVisualStyleBackColor = true;
            this.proc_btnSelectZone.Click += new System.EventHandler(this.proc_btnSelectZone_Click);
            // 
            // proc_cboFloor
            // 
            this.proc_cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proc_cboFloor.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.proc_cboFloor.FormattingEnabled = true;
            this.proc_cboFloor.Location = new System.Drawing.Point(1746, 6);
            this.proc_cboFloor.Name = "proc_cboFloor";
            this.proc_cboFloor.Size = new System.Drawing.Size(95, 29);
            this.proc_cboFloor.TabIndex = 3;
            // 
            // proc_cboBuilding
            // 
            this.proc_cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proc_cboBuilding.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.proc_cboBuilding.FormattingEnabled = true;
            this.proc_cboBuilding.Location = new System.Drawing.Point(1424, 6);
            this.proc_cboBuilding.Name = "proc_cboBuilding";
            this.proc_cboBuilding.Size = new System.Drawing.Size(316, 29);
            this.proc_cboBuilding.TabIndex = 3;
            this.proc_cboBuilding.SelectedIndexChanged += new System.EventHandler(this.proc_cboBuilding_SelectedIndexChanged);
            // 
            // proc_cboBuildingGroup
            // 
            this.proc_cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proc_cboBuildingGroup.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.proc_cboBuildingGroup.FormattingEnabled = true;
            this.proc_cboBuildingGroup.Location = new System.Drawing.Point(1243, 6);
            this.proc_cboBuildingGroup.Name = "proc_cboBuildingGroup";
            this.proc_cboBuildingGroup.Size = new System.Drawing.Size(175, 29);
            this.proc_cboBuildingGroup.TabIndex = 3;
            this.proc_cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.Proc_cboBuildingGroup_SelectedIndexChanged);
            // 
            // proc_lblSelectZone
            // 
            this.proc_lblSelectZone.AutoSize = true;
            this.proc_lblSelectZone.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.proc_lblSelectZone.Location = new System.Drawing.Point(1163, 10);
            this.proc_lblSelectZone.Name = "proc_lblSelectZone";
            this.proc_lblSelectZone.Size = new System.Drawing.Size(74, 21);
            this.proc_lblSelectZone.TabIndex = 2;
            this.proc_lblSelectZone.Text = "위치선택";
            // 
            // panelMiddle
            // 
            this.panelMiddle.BackColor = System.Drawing.Color.Transparent;
            this.panelMiddle.BackgroundImage = global::SDMS.Properties.Resources.HToolbar_bkgnd;
            this.panelMiddle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelMiddle.Controls.Add(this.btnSimulator);
            this.panelMiddle.Controls.Add(this.labelSensorMonitor);
            this.panelMiddle.Controls.Add(this.btnSendMessage);
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
            this.panelMiddle.Controls.Add(this.btnWeatherInfo);
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
            this.panelMiddle.Location = new System.Drawing.Point(0, 455);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Size = new System.Drawing.Size(1920, 48);
            this.panelMiddle.TabIndex = 2;
            this.panelMiddle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMiddle_MouseDown);
            // 
            // btnSimulator
            // 
            this.btnSimulator.BackgroundImage = global::SDMS.Properties.Resources.SensorSimulator_Normal;
            this.btnSimulator.Location = new System.Drawing.Point(414, 32);
            this.btnSimulator.Name = "btnSimulator";
            this.btnSimulator.Size = new System.Drawing.Size(32, 32);
            this.btnSimulator.TabIndex = 14;
            this.btnSimulator.UseVisualStyleBackColor = true;
            this.btnSimulator.Visible = false;
            this.btnSimulator.Click += new System.EventHandler(this.btnSimulator_Click);
            // 
            // labelSensorMonitor
            // 
            this.labelSensorMonitor.AutoSize = true;
            this.labelSensorMonitor.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSensorMonitor.Location = new System.Drawing.Point(460, 11);
            this.labelSensorMonitor.Name = "labelSensorMonitor";
            this.labelSensorMonitor.Size = new System.Drawing.Size(58, 21);
            this.labelSensorMonitor.TabIndex = 11;
            this.labelSensorMonitor.Text = "수신반";
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSendMessage.Location = new System.Drawing.Point(514, 6);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(85, 30);
            this.btnSendMessage.TabIndex = 10;
            this.btnSendMessage.Text = "알림공지";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnSensorMonitor
            // 
            this.btnSensorMonitor.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSensorMonitor.Location = new System.Drawing.Point(393, 6);
            this.btnSensorMonitor.Name = "btnSensorMonitor";
            this.btnSensorMonitor.Size = new System.Drawing.Size(61, 30);
            this.btnSensorMonitor.TabIndex = 10;
            this.btnSensorMonitor.Text = "상세보기";
            this.btnSensorMonitor.UseVisualStyleBackColor = true;
            this.btnSensorMonitor.Click += new System.EventHandler(this.btnSensorMonitor_Click);
            // 
            // btnShowCCTVList
            // 
            this.btnShowCCTVList.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnShowCCTVList.Location = new System.Drawing.Point(207, 6);
            this.btnShowCCTVList.Name = "btnShowCCTVList";
            this.btnShowCCTVList.Size = new System.Drawing.Size(132, 30);
            this.btnShowCCTVList.TabIndex = 9;
            this.btnShowCCTVList.Text = "CCTV List 보기";
            this.btnShowCCTVList.UseVisualStyleBackColor = true;
            this.btnShowCCTVList.Visible = false;
            this.btnShowCCTVList.Click += new System.EventHandler(this.btnShowCCTVList_Click);
            // 
            // cboEquipZone
            // 
            this.cboEquipZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEquipZone.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboEquipZone.FormattingEnabled = true;
            this.cboEquipZone.Location = new System.Drawing.Point(1150, 8);
            this.cboEquipZone.Name = "cboEquipZone";
            this.cboEquipZone.Size = new System.Drawing.Size(42, 29);
            this.cboEquipZone.TabIndex = 8;
            this.cboEquipZone.Visible = false;
            this.cboEquipZone.SelectedIndexChanged += new System.EventHandler(this.cboEquipZone_SelectedIndexChanged);
            // 
            // checkBoxEquipZoneCCTV
            // 
            this.checkBoxEquipZoneCCTV.AutoSize = true;
            this.checkBoxEquipZoneCCTV.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxEquipZoneCCTV.Location = new System.Drawing.Point(10, 8);
            this.checkBoxEquipZoneCCTV.Name = "checkBoxEquipZoneCCTV";
            this.checkBoxEquipZoneCCTV.Size = new System.Drawing.Size(192, 25);
            this.checkBoxEquipZoneCCTV.TabIndex = 7;
            this.checkBoxEquipZoneCCTV.Text = "영역별 CCTV 설정하기";
            this.checkBoxEquipZoneCCTV.UseVisualStyleBackColor = true;
            this.checkBoxEquipZoneCCTV.Visible = false;
            this.checkBoxEquipZoneCCTV.CheckedChanged += new System.EventHandler(this.checkBoxEquipZoneCCTV_CheckedChanged);
            // 
            // labelFireDetect
            // 
            this.labelFireDetect.AutoSize = true;
            this.labelFireDetect.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFireDetect.Location = new System.Drawing.Point(584, 11);
            this.labelFireDetect.Name = "labelFireDetect";
            this.labelFireDetect.Size = new System.Drawing.Size(80, 21);
            this.labelFireDetect.TabIndex = 6;
            this.labelFireDetect.Text = "재난 발생";
            // 
            // cmbFireDetect
            // 
            this.cmbFireDetect.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbFireDetect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFireDetect.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbFireDetect.FormattingEnabled = true;
            this.cmbFireDetect.Location = new System.Drawing.Point(670, 8);
            this.cmbFireDetect.MaxDropDownItems = 30;
            this.cmbFireDetect.Name = "cmbFireDetect";
            this.cmbFireDetect.Size = new System.Drawing.Size(448, 30);
            this.cmbFireDetect.TabIndex = 5;
            this.cmbFireDetect.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbFireDetect_DrawItem);
            this.cmbFireDetect.DropDown += new System.EventHandler(this.cmbFireDetect_DropDown);
            this.cmbFireDetect.SelectedIndexChanged += new System.EventHandler(this.cmbFireDetect_SelectedIndexChanged);
            this.cmbFireDetect.SelectionChangeCommitted += new System.EventHandler(this.cmbFireDetect_SelectionChangeCommitted);
            this.cmbFireDetect.DropDownClosed += new System.EventHandler(this.cmbFireDetect_DropDownClosed);
            // 
            // btnSelectZone
            // 
            this.btnSelectZone.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectZone.Location = new System.Drawing.Point(1859, 6);
            this.btnSelectZone.Name = "btnSelectZone";
            this.btnSelectZone.Size = new System.Drawing.Size(52, 30);
            this.btnSelectZone.TabIndex = 4;
            this.btnSelectZone.Text = "선택";
            this.btnSelectZone.UseVisualStyleBackColor = true;
            this.btnSelectZone.Visible = false;
            this.btnSelectZone.Click += new System.EventHandler(this.btnSelectZone_Click);
            // 
            // cboFloor
            // 
            this.cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloor.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboFloor.FormattingEnabled = true;
            this.cboFloor.Location = new System.Drawing.Point(1776, 8);
            this.cboFloor.Name = "cboFloor";
            this.cboFloor.Size = new System.Drawing.Size(71, 29);
            this.cboFloor.TabIndex = 3;
            this.cboFloor.Visible = false;
            this.cboFloor.SelectedIndexChanged += new System.EventHandler(this.cboFloor_SelectedIndexChanged);
            this.cboFloor.SelectedValueChanged += new System.EventHandler(this.cboFloor_SelectedIndexChanged);
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.Location = new System.Drawing.Point(1433, 8);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Size = new System.Drawing.Size(337, 29);
            this.cboBuilding.TabIndex = 3;
            this.cboBuilding.Visible = false;
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.Location = new System.Drawing.Point(1288, 8);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Size = new System.Drawing.Size(139, 29);
            this.cboBuildingGroup.TabIndex = 3;
            this.cboBuildingGroup.Visible = false;
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // labelSelectZone
            // 
            this.labelSelectZone.AutoSize = true;
            this.labelSelectZone.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSelectZone.Location = new System.Drawing.Point(1208, 11);
            this.labelSelectZone.Name = "labelSelectZone";
            this.labelSelectZone.Size = new System.Drawing.Size(74, 21);
            this.labelSelectZone.TabIndex = 2;
            this.labelSelectZone.Text = "위치선택";
            this.labelSelectZone.Visible = false;
            // 
            // btnMultiCCTV
            // 
            this.btnMultiCCTV.BackgroundImage = global::SDMS.Properties.Resources.CCTV_Normal;
            this.btnMultiCCTV.Location = new System.Drawing.Point(320, 32);
            this.btnMultiCCTV.Name = "btnMultiCCTV";
            this.btnMultiCCTV.Size = new System.Drawing.Size(32, 32);
            this.btnMultiCCTV.TabIndex = 0;
            this.btnMultiCCTV.UseVisualStyleBackColor = true;
            this.btnMultiCCTV.Visible = false;
            this.btnMultiCCTV.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnWeatherInfo
            // 
            this.btnWeatherInfo.BackgroundImage = global::SDMS.Properties.Resources.Weather_Normal;
            this.btnWeatherInfo.Location = new System.Drawing.Point(383, 32);
            this.btnWeatherInfo.Name = "btnWeatherInfo";
            this.btnWeatherInfo.Size = new System.Drawing.Size(32, 32);
            this.btnWeatherInfo.TabIndex = 0;
            this.btnWeatherInfo.UseVisualStyleBackColor = true;
            this.btnWeatherInfo.Visible = false;
            this.btnWeatherInfo.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnScreenShot
            // 
            this.btnScreenShot.BackgroundImage = global::SDMS.Properties.Resources.ScreenShot_Normal;
            this.btnScreenShot.Location = new System.Drawing.Point(351, 32);
            this.btnScreenShot.Name = "btnScreenShot";
            this.btnScreenShot.Size = new System.Drawing.Size(32, 32);
            this.btnScreenShot.TabIndex = 0;
            this.btnScreenShot.UseVisualStyleBackColor = true;
            this.btnScreenShot.Visible = false;
            this.btnScreenShot.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnOutside
            // 
            this.btnOutside.BackgroundImage = global::SDMS.Properties.Resources._3d_normal;
            this.btnOutside.Location = new System.Drawing.Point(227, 32);
            this.btnOutside.Name = "btnOutside";
            this.btnOutside.Size = new System.Drawing.Size(32, 32);
            this.btnOutside.TabIndex = 0;
            this.btnOutside.UseVisualStyleBackColor = true;
            this.btnOutside.Visible = false;
            this.btnOutside.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnBoth
            // 
            this.btnBoth.BackgroundImage = global::SDMS.Properties.Resources.Both_Normal;
            this.btnBoth.Location = new System.Drawing.Point(258, 32);
            this.btnBoth.Name = "btnBoth";
            this.btnBoth.Size = new System.Drawing.Size(32, 32);
            this.btnBoth.TabIndex = 0;
            this.btnBoth.UseVisualStyleBackColor = true;
            this.btnBoth.Visible = false;
            this.btnBoth.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnInside
            // 
            this.btnInside.BackgroundImage = global::SDMS.Properties.Resources._2d_normal;
            this.btnInside.Location = new System.Drawing.Point(289, 32);
            this.btnInside.Name = "btnInside";
            this.btnInside.Size = new System.Drawing.Size(32, 32);
            this.btnInside.TabIndex = 0;
            this.btnInside.UseVisualStyleBackColor = true;
            this.btnInside.Visible = false;
            this.btnInside.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.BackgroundImage = global::SDMS.Properties.Resources.ZoomOut_Normal;
            this.btnZoomOut.Location = new System.Drawing.Point(196, 32);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(32, 32);
            this.btnZoomOut.TabIndex = 0;
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Visible = false;
            this.btnZoomOut.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.BackgroundImage = global::SDMS.Properties.Resources.ZoomIn_Normal;
            this.btnZoomIn.Location = new System.Drawing.Point(165, 32);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(32, 32);
            this.btnZoomIn.TabIndex = 0;
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Visible = false;
            this.btnZoomIn.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnOrbit
            // 
            this.btnOrbit.BackgroundImage = global::SDMS.Properties.Resources.Orbit_Normal;
            this.btnOrbit.Location = new System.Drawing.Point(134, 32);
            this.btnOrbit.Name = "btnOrbit";
            this.btnOrbit.Size = new System.Drawing.Size(32, 32);
            this.btnOrbit.TabIndex = 0;
            this.btnOrbit.UseVisualStyleBackColor = true;
            this.btnOrbit.Visible = false;
            this.btnOrbit.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnPanning
            // 
            this.btnPanning.BackgroundImage = global::SDMS.Properties.Resources.Panning_Normal;
            this.btnPanning.Location = new System.Drawing.Point(102, 32);
            this.btnPanning.Name = "btnPanning";
            this.btnPanning.Size = new System.Drawing.Size(32, 32);
            this.btnPanning.TabIndex = 0;
            this.btnPanning.UseVisualStyleBackColor = true;
            this.btnPanning.Visible = false;
            this.btnPanning.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnPick
            // 
            this.btnPick.BackgroundImage = global::SDMS.Properties.Resources.Pick_Normal;
            this.btnPick.Location = new System.Drawing.Point(71, 32);
            this.btnPick.Name = "btnPick";
            this.btnPick.Size = new System.Drawing.Size(32, 32);
            this.btnPick.TabIndex = 0;
            this.btnPick.UseVisualStyleBackColor = true;
            this.btnPick.Visible = false;
            this.btnPick.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnFullScreen
            // 
            this.btnFullScreen.BackgroundImage = global::SDMS.Properties.Resources.FullScreen_Normal;
            this.btnFullScreen.Location = new System.Drawing.Point(40, 32);
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.Size = new System.Drawing.Size(32, 32);
            this.btnFullScreen.TabIndex = 0;
            this.btnFullScreen.UseVisualStyleBackColor = true;
            this.btnFullScreen.Visible = false;
            this.btnFullScreen.Click += new System.EventHandler(this.OnClickToolBarButton);
            // 
            // btnHome
            // 
            this.btnHome.BackgroundImage = global::SDMS.Properties.Resources.Home_Normal;
            this.btnHome.Location = new System.Drawing.Point(9, 32);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(32, 32);
            this.btnHome.TabIndex = 0;
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Visible = false;
            this.btnHome.Click += new System.EventHandler(this.OnClickToolBarButton);
            this.btnHome.Leave += new System.EventHandler(this.btnHome_Leave);
            // 
            // panelTop
            // 
            this.panelTop.BackgroundImage = global::SDMS.Properties.Resources.ToolbarBkgnd;
            this.panelTop.Controls.Add(this.pictureBox2D);
            this.panelTop.Controls.Add(this.pictureBoxCCTV);
            this.panelTop.Controls.Add(this.btnDefaultCCTV);
            this.panelTop.Controls.Add(this.btnMissionStatus);
            this.panelTop.Controls.Add(this.btnBulletin);
            this.panelTop.Controls.Add(this.btnSOP);
            this.panelTop.Controls.Add(this.btnSDMS);
            this.panelTop.Controls.Add(this.labelTitle);
            this.panelTop.Controls.Add(this.labelTime);
            this.panelTop.Controls.Add(this.panelReportRibbonBarMiddle);
            this.panelTop.Controls.Add(this.labelDate);
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
            this.panelTop.Size = new System.Drawing.Size(1913, 441);
            this.panelTop.TabIndex = 0;
            // 
            // pictureBox2D
            // 
            this.pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            this.pictureBox2D.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2D.Location = new System.Drawing.Point(100, 29);
            this.pictureBox2D.Name = "pictureBox2D";
            this.pictureBox2D.Size = new System.Drawing.Size(98, 35);
            this.pictureBox2D.TabIndex = 14;
            this.pictureBox2D.TabStop = false;
            // 
            // pictureBoxCCTV
            // 
            this.pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            this.pictureBoxCCTV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxCCTV.Location = new System.Drawing.Point(199, 29);
            this.pictureBoxCCTV.Name = "pictureBoxCCTV";
            this.pictureBoxCCTV.Size = new System.Drawing.Size(98, 35);
            this.pictureBoxCCTV.TabIndex = 13;
            this.pictureBoxCCTV.TabStop = false;
            // 
            // btnDefaultCCTV
            // 
            this.btnDefaultCCTV.BackColor = System.Drawing.Color.Transparent;
            this.btnDefaultCCTV.CheckButton = false;
            this.btnDefaultCCTV.CheckedBkgndImage = null;
            this.btnDefaultCCTV.CheckedImage = global::SDMS.Properties.Resources.SOPcctv_checked;
            this.btnDefaultCCTV.ClickedBackgroundImage = null;
            this.btnDefaultCCTV.ClickedImage = global::SDMS.Properties.Resources.SOPcctv_checked;
            this.btnDefaultCCTV.CustomImageRect = new System.Drawing.Rectangle(0, 0, 89, 87);
            this.btnDefaultCCTV.DisabledBkgndImage = null;
            this.btnDefaultCCTV.DisabledImage = global::SDMS.Properties.Resources.base_disable;
            this.btnDefaultCCTV.ID = -1;
            this.btnDefaultCCTV.InitButtonWidth = 87;
            this.btnDefaultCCTV.IsChecked = false;
            this.btnDefaultCCTV.Location = new System.Drawing.Point(358, 67);
            this.btnDefaultCCTV.MouseOverBkgndImage = null;
            this.btnDefaultCCTV.MouseOverImage = global::SDMS.Properties.Resources.over_03;
            this.btnDefaultCCTV.Name = "btnDefaultCCTV";
            this.btnDefaultCCTV.NormalImage = global::SDMS.Properties.Resources.SOPcctv_normal;
            this.btnDefaultCCTV.Owner = null;
            this.btnDefaultCCTV.Size = new System.Drawing.Size(87, 87);
            this.btnDefaultCCTV.TabIndex = 12;
            this.btnDefaultCCTV.TextLocation = new System.Drawing.Point(0, 0);
            this.btnDefaultCCTV.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDefaultCCTV.ToolTipText = "";
            this.btnDefaultCCTV.UseCustomImageRect = true;
            this.btnDefaultCCTV.UseTextLocation = false;
            this.btnDefaultCCTV.UseVisualStyleBackColor = false;
            this.btnDefaultCCTV.Click += new System.EventHandler(this.btnDefaultCCTV_Click);
            // 
            // btnMissionStatus
            // 
            this.btnMissionStatus.BackColor = System.Drawing.Color.Transparent;
            this.btnMissionStatus.CheckButton = false;
            this.btnMissionStatus.CheckedBkgndImage = null;
            this.btnMissionStatus.CheckedImage = global::SDMS.Properties.Resources.임무_checked_04;
            this.btnMissionStatus.ClickedBackgroundImage = null;
            this.btnMissionStatus.ClickedImage = global::SDMS.Properties.Resources.임무_checked_04;
            this.btnMissionStatus.CustomImageRect = new System.Drawing.Rectangle(0, 0, 89, 87);
            this.btnMissionStatus.DisabledBkgndImage = null;
            this.btnMissionStatus.DisabledImage = global::SDMS.Properties.Resources.임무_disable_04;
            this.btnMissionStatus.ID = -1;
            this.btnMissionStatus.InitButtonWidth = 87;
            this.btnMissionStatus.IsChecked = false;
            this.btnMissionStatus.Location = new System.Drawing.Point(270, 67);
            this.btnMissionStatus.MouseOverBkgndImage = null;
            this.btnMissionStatus.MouseOverImage = global::SDMS.Properties.Resources.over_03;
            this.btnMissionStatus.Name = "btnMissionStatus";
            this.btnMissionStatus.NormalImage = global::SDMS.Properties.Resources.임무_normal_04;
            this.btnMissionStatus.Owner = null;
            this.btnMissionStatus.Size = new System.Drawing.Size(87, 87);
            this.btnMissionStatus.TabIndex = 11;
            this.btnMissionStatus.Text = "현황판";
            this.btnMissionStatus.TextLocation = new System.Drawing.Point(0, 0);
            this.btnMissionStatus.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnMissionStatus.ToolTipText = "현황판";
            this.btnMissionStatus.UseCustomImageRect = true;
            this.btnMissionStatus.UseTextLocation = false;
            this.btnMissionStatus.UseVisualStyleBackColor = false;
            this.btnMissionStatus.Click += new System.EventHandler(this.btnMissionStatus_Click);
            // 
            // btnBulletin
            // 
            this.btnBulletin.BackColor = System.Drawing.Color.Transparent;
            this.btnBulletin.CheckButton = false;
            this.btnBulletin.CheckedBkgndImage = null;
            this.btnBulletin.CheckedImage = global::SDMS.Properties.Resources.상황판_checked_03;
            this.btnBulletin.ClickedBackgroundImage = null;
            this.btnBulletin.ClickedImage = global::SDMS.Properties.Resources.상황판_checked_03;
            this.btnBulletin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 89, 87);
            this.btnBulletin.DisabledBkgndImage = null;
            this.btnBulletin.DisabledImage = global::SDMS.Properties.Resources.상황판_disable_03;
            this.btnBulletin.ID = -1;
            this.btnBulletin.InitButtonWidth = 87;
            this.btnBulletin.IsChecked = false;
            this.btnBulletin.Location = new System.Drawing.Point(181, 67);
            this.btnBulletin.MouseOverBkgndImage = null;
            this.btnBulletin.MouseOverImage = global::SDMS.Properties.Resources.over_03;
            this.btnBulletin.Name = "btnBulletin";
            this.btnBulletin.NormalImage = global::SDMS.Properties.Resources.상황판_normal_03;
            this.btnBulletin.Owner = null;
            this.btnBulletin.Size = new System.Drawing.Size(87, 87);
            this.btnBulletin.TabIndex = 11;
            this.btnBulletin.Text = "상황판";
            this.btnBulletin.TextLocation = new System.Drawing.Point(0, 0);
            this.btnBulletin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnBulletin.ToolTipText = "상황판";
            this.btnBulletin.UseCustomImageRect = true;
            this.btnBulletin.UseTextLocation = false;
            this.btnBulletin.UseVisualStyleBackColor = false;
            this.btnBulletin.Click += new System.EventHandler(this.btnBulletin_Click);
            // 
            // btnSOP
            // 
            this.btnSOP.BackColor = System.Drawing.Color.Transparent;
            this.btnSOP.CheckButton = false;
            this.btnSOP.CheckedBkgndImage = null;
            this.btnSOP.CheckedImage = global::SDMS.Properties.Resources.sop_checked_03;
            this.btnSOP.ClickedBackgroundImage = null;
            this.btnSOP.ClickedImage = global::SDMS.Properties.Resources.sop_checked_03;
            this.btnSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 89, 87);
            this.btnSOP.DisabledBkgndImage = null;
            this.btnSOP.DisabledImage = global::SDMS.Properties.Resources.sop_disable_03;
            this.btnSOP.ID = -1;
            this.btnSOP.InitButtonWidth = 89;
            this.btnSOP.IsChecked = false;
            this.btnSOP.Location = new System.Drawing.Point(91, 67);
            this.btnSOP.MouseOverBkgndImage = null;
            this.btnSOP.MouseOverImage = global::SDMS.Properties.Resources.over_03;
            this.btnSOP.Name = "btnSOP";
            this.btnSOP.NormalImage = global::SDMS.Properties.Resources.sop_normal_03;
            this.btnSOP.Owner = null;
            this.btnSOP.Size = new System.Drawing.Size(89, 87);
            this.btnSOP.TabIndex = 11;
            this.btnSOP.Text = "SOP시스템";
            this.btnSOP.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSOP.ToolTipText = "SOP시스템";
            this.btnSOP.UseCustomImageRect = true;
            this.btnSOP.UseTextLocation = false;
            this.btnSOP.UseVisualStyleBackColor = false;
            this.btnSOP.Click += new System.EventHandler(this.btnSOP_Click);
            // 
            // btnSDMS
            // 
            this.btnSDMS.BackColor = System.Drawing.Color.Transparent;
            this.btnSDMS.CheckButton = false;
            this.btnSDMS.CheckedBkgndImage = null;
            this.btnSDMS.CheckedImage = global::SDMS.Properties.Resources.화재_checked_03;
            this.btnSDMS.ClickedBackgroundImage = null;
            this.btnSDMS.ClickedImage = global::SDMS.Properties.Resources.화재_checked_03;
            this.btnSDMS.CustomImageRect = new System.Drawing.Rectangle(0, 0, 89, 87);
            this.btnSDMS.DisabledBkgndImage = null;
            this.btnSDMS.DisabledImage = global::SDMS.Properties.Resources.화재_disable_03;
            this.btnSDMS.Enabled = false;
            this.btnSDMS.ID = -1;
            this.btnSDMS.InitButtonWidth = 87;
            this.btnSDMS.IsChecked = false;
            this.btnSDMS.Location = new System.Drawing.Point(3, 67);
            this.btnSDMS.MouseOverBkgndImage = null;
            this.btnSDMS.MouseOverImage = global::SDMS.Properties.Resources.over_03;
            this.btnSDMS.Name = "btnSDMS";
            this.btnSDMS.NormalImage = global::SDMS.Properties.Resources.화재_normal_03;
            this.btnSDMS.Owner = null;
            this.btnSDMS.Size = new System.Drawing.Size(87, 87);
            this.btnSDMS.TabIndex = 11;
            this.btnSDMS.Text = "재난탐지";
            this.btnSDMS.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSDMS.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSDMS.ToolTipText = "재난탐지";
            this.btnSDMS.UseCustomImageRect = true;
            this.btnSDMS.UseTextLocation = false;
            this.btnSDMS.UseVisualStyleBackColor = false;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(30, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(36, 17);
            this.labelTitle.TabIndex = 10;
            this.labelTitle.Text = "Title";
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
            this.labelTime.TabIndex = 1;
            this.labelTime.Text = "00:00:00";
            // 
            // panelReportRibbonBarMiddle
            // 
            this.panelReportRibbonBarMiddle.BackColor = System.Drawing.Color.Transparent;
            this.panelReportRibbonBarMiddle.BackgroundImage = global::SDMS.Properties.Resources.RibbonBar_Middle;
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnSMSIntrusionHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnReactionIntrusionHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnProcessIntrusionHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnDetectIntrusionHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.imgReportSplit2);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnDetectIntrusionAnalyze);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnSMSPSMHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnNotOperationPSMHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnDetectPSMHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.imgReportSplit);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnActionPSMHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnSMSHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnReactionHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnProcessHistory);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnDetectPSMAnalyze);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnDetectAnalyze);
            this.panelReportRibbonBarMiddle.Controls.Add(this.btnDetectHistory);
            this.panelReportRibbonBarMiddle.Location = new System.Drawing.Point(140, 314);
            this.panelReportRibbonBarMiddle.Name = "panelReportRibbonBarMiddle";
            this.panelReportRibbonBarMiddle.Size = new System.Drawing.Size(1517, 87);
            this.panelReportRibbonBarMiddle.TabIndex = 9;
            // 
            // btnSMSIntrusionHistory
            // 
            this.btnSMSIntrusionHistory.CheckedBkgndImage = null;
            this.btnSMSIntrusionHistory.CheckedImage = null;
            this.btnSMSIntrusionHistory.IsChecked = false;
            this.btnSMSIntrusionHistory.Location = new System.Drawing.Point(1061, 1);
            this.btnSMSIntrusionHistory.MouseOverBkgndImage = null;
            this.btnSMSIntrusionHistory.Name = "btnSMSIntrusionHistory";
            this.btnSMSIntrusionHistory.NormalImage = null;
            this.btnSMSIntrusionHistory.Owner = null;
            this.btnSMSIntrusionHistory.Size = new System.Drawing.Size(81, 85);
            this.btnSMSIntrusionHistory.TabIndex = 19;
            this.btnSMSIntrusionHistory.Title = "방범문자이력";
            this.btnSMSIntrusionHistory.UseVisualStyleBackColor = true;
            // 
            // btnReactionIntrusionHistory
            // 
            this.btnReactionIntrusionHistory.CheckedBkgndImage = null;
            this.btnReactionIntrusionHistory.CheckedImage = null;
            this.btnReactionIntrusionHistory.IsChecked = false;
            this.btnReactionIntrusionHistory.Location = new System.Drawing.Point(982, 1);
            this.btnReactionIntrusionHistory.MouseOverBkgndImage = null;
            this.btnReactionIntrusionHistory.Name = "btnReactionIntrusionHistory";
            this.btnReactionIntrusionHistory.NormalImage = null;
            this.btnReactionIntrusionHistory.Owner = null;
            this.btnReactionIntrusionHistory.Size = new System.Drawing.Size(81, 85);
            this.btnReactionIntrusionHistory.TabIndex = 18;
            this.btnReactionIntrusionHistory.Title = "방범대응이력";
            this.btnReactionIntrusionHistory.UseVisualStyleBackColor = true;
            // 
            // btnProcessIntrusionHistory
            // 
            this.btnProcessIntrusionHistory.CheckedBkgndImage = null;
            this.btnProcessIntrusionHistory.CheckedImage = null;
            this.btnProcessIntrusionHistory.IsChecked = false;
            this.btnProcessIntrusionHistory.Location = new System.Drawing.Point(904, 1);
            this.btnProcessIntrusionHistory.MouseOverBkgndImage = null;
            this.btnProcessIntrusionHistory.Name = "btnProcessIntrusionHistory";
            this.btnProcessIntrusionHistory.NormalImage = null;
            this.btnProcessIntrusionHistory.Owner = null;
            this.btnProcessIntrusionHistory.Size = new System.Drawing.Size(81, 85);
            this.btnProcessIntrusionHistory.TabIndex = 17;
            this.btnProcessIntrusionHistory.Title = "방범처리이력";
            this.btnProcessIntrusionHistory.UseVisualStyleBackColor = true;
            // 
            // btnDetectIntrusionHistory
            // 
            this.btnDetectIntrusionHistory.CheckedBkgndImage = null;
            this.btnDetectIntrusionHistory.CheckedImage = null;
            this.btnDetectIntrusionHistory.IsChecked = false;
            this.btnDetectIntrusionHistory.Location = new System.Drawing.Point(828, 1);
            this.btnDetectIntrusionHistory.MouseOverBkgndImage = null;
            this.btnDetectIntrusionHistory.Name = "btnDetectIntrusionHistory";
            this.btnDetectIntrusionHistory.NormalImage = null;
            this.btnDetectIntrusionHistory.Owner = null;
            this.btnDetectIntrusionHistory.Size = new System.Drawing.Size(81, 85);
            this.btnDetectIntrusionHistory.TabIndex = 16;
            this.btnDetectIntrusionHistory.Title = "방범탐지이력";
            this.btnDetectIntrusionHistory.UseVisualStyleBackColor = true;
            // 
            // imgReportSplit2
            // 
            this.imgReportSplit2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgReportSplit2.Image = global::SDMS.Properties.Resources.Separator;
            this.imgReportSplit2.Location = new System.Drawing.Point(725, 8);
            this.imgReportSplit2.Name = "imgReportSplit2";
            this.imgReportSplit2.Size = new System.Drawing.Size(13, 76);
            this.imgReportSplit2.TabIndex = 15;
            this.imgReportSplit2.TabStop = false;
            // 
            // btnDetectIntrusionAnalyze
            // 
            this.btnDetectIntrusionAnalyze.CheckedBkgndImage = null;
            this.btnDetectIntrusionAnalyze.CheckedImage = null;
            this.btnDetectIntrusionAnalyze.IsChecked = false;
            this.btnDetectIntrusionAnalyze.Location = new System.Drawing.Point(744, 1);
            this.btnDetectIntrusionAnalyze.MouseOverBkgndImage = null;
            this.btnDetectIntrusionAnalyze.Name = "btnDetectIntrusionAnalyze";
            this.btnDetectIntrusionAnalyze.NormalImage = null;
            this.btnDetectIntrusionAnalyze.Owner = null;
            this.btnDetectIntrusionAnalyze.Size = new System.Drawing.Size(81, 85);
            this.btnDetectIntrusionAnalyze.TabIndex = 14;
            this.btnDetectIntrusionAnalyze.Title = "방범탐지분석";
            this.btnDetectIntrusionAnalyze.UseVisualStyleBackColor = true;
            // 
            // btnSMSPSMHistory
            // 
            this.btnSMSPSMHistory.CheckedBkgndImage = null;
            this.btnSMSPSMHistory.CheckedImage = null;
            this.btnSMSPSMHistory.IsChecked = false;
            this.btnSMSPSMHistory.Location = new System.Drawing.Point(640, 1);
            this.btnSMSPSMHistory.MouseOverBkgndImage = null;
            this.btnSMSPSMHistory.Name = "btnSMSPSMHistory";
            this.btnSMSPSMHistory.NormalImage = null;
            this.btnSMSPSMHistory.Owner = null;
            this.btnSMSPSMHistory.Size = new System.Drawing.Size(81, 85);
            this.btnSMSPSMHistory.TabIndex = 13;
            this.btnSMSPSMHistory.Title = "누출문자이력";
            this.btnSMSPSMHistory.UseVisualStyleBackColor = true;
            // 
            // btnNotOperationPSMHistory
            // 
            this.btnNotOperationPSMHistory.CheckedBkgndImage = null;
            this.btnNotOperationPSMHistory.CheckedImage = null;
            this.btnNotOperationPSMHistory.IsChecked = false;
            this.btnNotOperationPSMHistory.Location = new System.Drawing.Point(486, 1);
            this.btnNotOperationPSMHistory.MouseOverBkgndImage = null;
            this.btnNotOperationPSMHistory.Name = "btnNotOperationPSMHistory";
            this.btnNotOperationPSMHistory.NormalImage = null;
            this.btnNotOperationPSMHistory.Owner = null;
            this.btnNotOperationPSMHistory.Size = new System.Drawing.Size(81, 85);
            this.btnNotOperationPSMHistory.TabIndex = 12;
            this.btnNotOperationPSMHistory.Title = "누출처리이력";
            this.btnNotOperationPSMHistory.UseVisualStyleBackColor = true;
            // 
            // btnDetectPSMHistory
            // 
            this.btnDetectPSMHistory.CheckedBkgndImage = null;
            this.btnDetectPSMHistory.CheckedImage = null;
            this.btnDetectPSMHistory.IsChecked = false;
            this.btnDetectPSMHistory.Location = new System.Drawing.Point(564, 1);
            this.btnDetectPSMHistory.MouseOverBkgndImage = null;
            this.btnDetectPSMHistory.Name = "btnDetectPSMHistory";
            this.btnDetectPSMHistory.NormalImage = null;
            this.btnDetectPSMHistory.Owner = null;
            this.btnDetectPSMHistory.Size = new System.Drawing.Size(81, 85);
            this.btnDetectPSMHistory.TabIndex = 11;
            this.btnDetectPSMHistory.Title = "누출탐지이력";
            this.btnDetectPSMHistory.UseVisualStyleBackColor = true;
            // 
            // imgReportSplit
            // 
            this.imgReportSplit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgReportSplit.Image = global::SDMS.Properties.Resources.Separator;
            this.imgReportSplit.Location = new System.Drawing.Point(313, 8);
            this.imgReportSplit.Name = "imgReportSplit";
            this.imgReportSplit.Size = new System.Drawing.Size(13, 76);
            this.imgReportSplit.TabIndex = 10;
            this.imgReportSplit.TabStop = false;
            // 
            // btnActionPSMHistory
            // 
            this.btnActionPSMHistory.CheckedBkgndImage = null;
            this.btnActionPSMHistory.CheckedImage = null;
            this.btnActionPSMHistory.IsChecked = false;
            this.btnActionPSMHistory.Location = new System.Drawing.Point(405, 1);
            this.btnActionPSMHistory.MouseOverBkgndImage = null;
            this.btnActionPSMHistory.Name = "btnActionPSMHistory";
            this.btnActionPSMHistory.NormalImage = null;
            this.btnActionPSMHistory.Owner = null;
            this.btnActionPSMHistory.Size = new System.Drawing.Size(81, 85);
            this.btnActionPSMHistory.TabIndex = 2;
            this.btnActionPSMHistory.Title = "누출대응이력";
            this.btnActionPSMHistory.UseVisualStyleBackColor = true;
            // 
            // btnSMSHistory
            // 
            this.btnSMSHistory.CheckedBkgndImage = null;
            this.btnSMSHistory.CheckedImage = null;
            this.btnSMSHistory.IsChecked = false;
            this.btnSMSHistory.Location = new System.Drawing.Point(247, 1);
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
            this.btnReactionHistory.Location = new System.Drawing.Point(191, 1);
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
            this.btnProcessHistory.Location = new System.Drawing.Point(133, 1);
            this.btnProcessHistory.MouseOverBkgndImage = null;
            this.btnProcessHistory.Name = "btnProcessHistory";
            this.btnProcessHistory.NormalImage = null;
            this.btnProcessHistory.Owner = null;
            this.btnProcessHistory.Size = new System.Drawing.Size(60, 85);
            this.btnProcessHistory.TabIndex = 0;
            this.btnProcessHistory.Title = "처리이력";
            this.btnProcessHistory.UseVisualStyleBackColor = true;
            // 
            // btnDetectPSMAnalyze
            // 
            this.btnDetectPSMAnalyze.CheckedBkgndImage = null;
            this.btnDetectPSMAnalyze.CheckedImage = null;
            this.btnDetectPSMAnalyze.IsChecked = false;
            this.btnDetectPSMAnalyze.Location = new System.Drawing.Point(327, 1);
            this.btnDetectPSMAnalyze.MouseOverBkgndImage = null;
            this.btnDetectPSMAnalyze.Name = "btnDetectPSMAnalyze";
            this.btnDetectPSMAnalyze.NormalImage = null;
            this.btnDetectPSMAnalyze.Owner = null;
            this.btnDetectPSMAnalyze.Size = new System.Drawing.Size(81, 85);
            this.btnDetectPSMAnalyze.TabIndex = 0;
            this.btnDetectPSMAnalyze.Title = "누출탐지분석";
            this.btnDetectPSMAnalyze.UseVisualStyleBackColor = true;
            // 
            // btnDetectAnalyze
            // 
            this.btnDetectAnalyze.CheckedBkgndImage = null;
            this.btnDetectAnalyze.CheckedImage = null;
            this.btnDetectAnalyze.IsChecked = false;
            this.btnDetectAnalyze.Location = new System.Drawing.Point(1, 1);
            this.btnDetectAnalyze.MouseOverBkgndImage = null;
            this.btnDetectAnalyze.Name = "btnDetectAnalyze";
            this.btnDetectAnalyze.NormalImage = null;
            this.btnDetectAnalyze.Owner = null;
            this.btnDetectAnalyze.Size = new System.Drawing.Size(60, 85);
            this.btnDetectAnalyze.TabIndex = 0;
            this.btnDetectAnalyze.Title = "탐지분석";
            this.btnDetectAnalyze.UseVisualStyleBackColor = true;
            // 
            // btnDetectHistory
            // 
            this.btnDetectHistory.CheckedBkgndImage = null;
            this.btnDetectHistory.CheckedImage = null;
            this.btnDetectHistory.IsChecked = false;
            this.btnDetectHistory.Location = new System.Drawing.Point(77, 1);
            this.btnDetectHistory.MouseOverBkgndImage = null;
            this.btnDetectHistory.Name = "btnDetectHistory";
            this.btnDetectHistory.NormalImage = null;
            this.btnDetectHistory.Owner = null;
            this.btnDetectHistory.Size = new System.Drawing.Size(60, 85);
            this.btnDetectHistory.TabIndex = 0;
            this.btnDetectHistory.Title = "탐지이력";
            this.btnDetectHistory.UseVisualStyleBackColor = true;
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
            this.labelDate.TabIndex = 0;
            this.labelDate.Text = "2013년 7월 1일";
            // 
            // panelAdminRibbonBarMiddle
            // 
            this.panelAdminRibbonBarMiddle.BackColor = System.Drawing.Color.Transparent;
            this.panelAdminRibbonBarMiddle.BackgroundImage = global::SDMS.Properties.Resources.RibbonBar_Middle;
            this.panelAdminRibbonBarMiddle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelAdminRibbonBarMiddle.Controls.Add(this.sensorMgrBtn);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnShowList);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnDelete);
            this.panelAdminRibbonBarMiddle.Controls.Add(this.btnEarthquake);
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
            // sensorMgrBtn
            // 
            this.sensorMgrBtn.CheckedBkgndImage = null;
            this.sensorMgrBtn.CheckedImage = null;
            this.sensorMgrBtn.IsChecked = false;
            this.sensorMgrBtn.Location = new System.Drawing.Point(275, 3);
            this.sensorMgrBtn.MouseOverBkgndImage = null;
            this.sensorMgrBtn.Name = "sensorMgrBtn";
            this.sensorMgrBtn.NormalImage = null;
            this.sensorMgrBtn.Owner = null;
            this.sensorMgrBtn.Size = new System.Drawing.Size(81, 85);
            this.sensorMgrBtn.TabIndex = 7;
            this.sensorMgrBtn.Title = "센서동작관리";
            this.sensorMgrBtn.UseVisualStyleBackColor = true;
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
            this.btnShowList.Size = new System.Drawing.Size(81, 85);
            this.btnShowList.TabIndex = 0;
            this.btnShowList.Title = "설비목록보기";
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
            this.btnSave.Location = new System.Drawing.Point(793, 1);
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
            this.btnManageFacility.Enabled = false;
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
            this.btnManageFacility.Visible = false;
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
            this.btnManagePrint.Enabled = false;
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
            this.btnManagePrint.Visible = false;
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
            this.pictureBoxReport.Location = new System.Drawing.Point(298, 29);
            this.pictureBoxReport.Name = "pictureBoxReport";
            this.pictureBoxReport.Size = new System.Drawing.Size(98, 35);
            this.pictureBoxReport.TabIndex = 5;
            this.pictureBoxReport.TabStop = false;
            // 
            // pictureBoxAdmin
            // 
            this.pictureBoxAdmin.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            this.pictureBoxAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxAdmin.Location = new System.Drawing.Point(397, 29);
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
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
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
            this.btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar_Blue;
            this.btnFire.ExtraImage = null;
            this.btnFire.Font = new System.Drawing.Font("맑은 고딕", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFire.ForeColor = System.Drawing.Color.White;
            this.btnFire.Location = new System.Drawing.Point(1674, 67);
            this.btnFire.Name = "btnFire";
            this.btnFire.Size = new System.Drawing.Size(246, 87);
            this.btnFire.TabIndex = 1;
            this.btnFire.Text = "재난신고";
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
            this.panelLog.BufferLength = 54;
            this.panelLog.Controls.Add(this.label5);
            this.panelLog.Controls.Add(this.pictureBoxLog);
            this.panelLog.Controls.Add(this.mLabelLog);
            this.panelLog.DisplayFont = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold);
            this.panelLog.DisplayLength = 34;
            this.panelLog.Location = new System.Drawing.Point(896, 67);
            this.panelLog.Name = "panelLog";
            this.panelLog.RealTimeInfo = null;
            this.panelLog.Size = new System.Drawing.Size(770, 87);
            this.panelLog.TabIndex = 0;
            this.panelLog.Text = "FormRealTimeInfo";
            this.panelLog.TextColor = System.Drawing.Color.White;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(5, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "재난대응현황";
            // 
            // pictureBoxLog
            // 
            this.pictureBoxLog.BackgroundImage = global::SDMS.Properties.Resources.Log_icon;
            this.pictureBoxLog.Location = new System.Drawing.Point(16, 5);
            this.pictureBoxLog.Name = "pictureBoxLog";
            this.pictureBoxLog.Size = new System.Drawing.Size(55, 57);
            this.pictureBoxLog.TabIndex = 6;
            this.pictureBoxLog.TabStop = false;
            this.pictureBoxLog.Visible = false;
            // 
            // mLabelLog
            // 
            this.mLabelLog.AutoSize = true;
            this.mLabelLog.BackColor = System.Drawing.Color.Transparent;
            this.mLabelLog.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mLabelLog.ForeColor = System.Drawing.Color.White;
            this.mLabelLog.Location = new System.Drawing.Point(98, 24);
            this.mLabelLog.Name = "mLabelLog";
            this.mLabelLog.Size = new System.Drawing.Size(0, 32);
            this.mLabelLog.TabIndex = 1;
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.Color.Transparent;
            this.panelStatus.BackgroundImage = global::SDMS.Properties.Resources.Log_Bar;
            this.panelStatus.Controls.Add(this.mLabelZone);
            this.panelStatus.Controls.Add(this.label4);
            this.panelStatus.Controls.Add(this.pictureBoxStatus);
            this.panelStatus.Controls.Add(this.mLabelStatus);
            this.panelStatus.Location = new System.Drawing.Point(450, 67);
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
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(14, 7);
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
            this.pictureBoxStatus.Visible = false;
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
            this.mLabelStatus.Text = "탐지 신호 없음";
            this.mLabelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelClock
            // 
            this.panelClock.BackColor = System.Drawing.Color.Transparent;
            this.panelClock.BackgroundImage = global::SDMS.Properties.Resources.Clock_Bar;
            this.panelClock.Controls.Add(this.label3);
            this.panelClock.Controls.Add(this.pictureBoxClock);
            this.panelClock.Location = new System.Drawing.Point(0, 67);
            this.panelClock.Name = "panelClock";
            this.panelClock.Size = new System.Drawing.Size(358, 87);
            this.panelClock.TabIndex = 0;
            this.panelClock.Visible = false;
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
            // btnEarthquake
            // 
            this.btnEarthquake.CheckedBkgndImage = null;
            this.btnEarthquake.CheckedImage = null;
            this.btnEarthquake.IsChecked = false;
            this.btnEarthquake.Location = new System.Drawing.Point(715, 1);
            this.btnEarthquake.MouseOverBkgndImage = null;
            this.btnEarthquake.Name = "btnEarthquake";
            this.btnEarthquake.NormalImage = null;
            this.btnEarthquake.Owner = null;
            this.btnEarthquake.Size = new System.Drawing.Size(60, 85);
            this.btnEarthquake.TabIndex = 0;
            this.btnEarthquake.Title = "지진관리";
            this.btnEarthquake.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1913, 772);
            this.Controls.Add(this.pnActionPSM);
            this.Controls.Add(this.pnSMSPSM);
            this.Controls.Add(this.pnNotOperationPSM);
            this.Controls.Add(this.pnDetectPSM);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelReactionHistory);
            this.Controls.Add(this.panelProcessHistory);
            this.Controls.Add(this.panelMiddle);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.DatePickerEnd2);
            this.Controls.Add(this.DatePickerStart2);
            this.Controls.Add(this.DatePickerEnd);
            this.Controls.Add(this.DatePickerStart);
            this.Controls.Add(this.DatePickerSMSPSMEnd);
            this.Controls.Add(this.DatePickerActionPSMEnd);
            this.Controls.Add(this.DatePickerSMSPSMStart);
            this.Controls.Add(this.DatePickerNotOperationPSMEnd);
            this.Controls.Add(this.DatePickerActionPSMStart);
            this.Controls.Add(this.DatePickerDetectPSMStart);
            this.Controls.Add(this.DatePickerNotOperationPSMStart);
            this.Controls.Add(this.DatePickerDetectPSMEnd);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.pnNotOperationPSM.ResumeLayout(false);
            this.pnNotOperationPSM.PerformLayout();
            this.pnDetectPSM.ResumeLayout(false);
            this.pnDetectPSM.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDetectPSMSplitUnitDetail)).EndInit();
            this.pnSMSPSM.ResumeLayout(false);
            this.pnSMSPSM.PerformLayout();
            this.pnActionPSM.ResumeLayout(false);
            this.pnActionPSM.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelReactionHistory.ResumeLayout(false);
            this.panelReactionHistory.PerformLayout();
            this.panelProcessHistory.ResumeLayout(false);
            this.panelProcessHistory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSplitUnitDetail)).EndInit();
            this.panelMiddle.ResumeLayout(false);
            this.panelMiddle.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2D)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCCTV)).EndInit();
            this.panelReportRibbonBarMiddle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgReportSplit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgReportSplit)).EndInit();
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
        private SDMS.TextPictureBox pictureBox2D;
        private SDMS.TextPictureBox pictureBoxCCTV;

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
        private System.Windows.Forms.Label lblIntrusionSelect;
        private System.Windows.Forms.Label lblReactionDate;
        private System.Windows.Forms.Panel panelMiddle;
        private System.Windows.Forms.Button proc_btnStartDate;
        private System.Windows.Forms.Button proc_btnEndDate;
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
        private System.Windows.Forms.ComboBox react_cboSearchTypeIntrusion;
        private System.Windows.Forms.Button react_btnEndDate;
        private System.Windows.Forms.Button react_btnStartDate;
        private System.Windows.Forms.ComboBox cboFireSelect;
        private System.Windows.Forms.ComboBox cboActionIntrusionSelect;
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
        private System.Windows.Forms.Label labelTitle;
        private RibbonButton btnSMSHistory;
        private System.Windows.Forms.Button btnWeatherInfo;
        private System.Windows.Forms.Button btnSimulator;
        private System.Windows.Forms.Label labelNotice;
        private System.Windows.Forms.Label labelBuildingText;
        private System.Windows.Forms.Button btnLayerNotice;
        private System.Windows.Forms.Button btnLayerBuildingText;        
        private System.Windows.Forms.Button btnLayerCCTVDisconnected;
        private System.Windows.Forms.Label labelCCTVDisconnected;
        private UnE.GUI.RibbonButton btnSDMS;
        private UnE.GUI.RibbonButton btnMissionStatus;
        private UnE.GUI.RibbonButton btnBulletin;
        private UnE.GUI.RibbonButton btnSOP;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.Label labelSaveHome;
        private UnE.GUI.RibbonButton btnDefaultCCTV;
        private System.Windows.Forms.ComboBox proc_cboViewCount;
        private System.Windows.Forms.ComboBox proc_cboSplitUnit;
        private System.Windows.Forms.Label lblViewCount;
        private System.Windows.Forms.Label lblSplitUnit;
        private System.Windows.Forms.NumericUpDown nudSplitUnitDetail;
        private System.Windows.Forms.Label lblSplitUnitDetail;
        private System.Windows.Forms.Button btnDateFormat;
        private System.Windows.Forms.Panel pnActionPSM;
        private System.Windows.Forms.ComboBox cboActionPSMSearchType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboActionPSMSelect;
        private System.Windows.Forms.Button btnActionPSMEndDate;
        private System.Windows.Forms.Button btnActionPSMStartDate;
        private System.Windows.Forms.ComboBox cboActionPSMEndTime;
        private System.Windows.Forms.ComboBox cboActionPSMStartTime;
        private System.Windows.Forms.Label lblActionPSMSelect;
        private System.Windows.Forms.Label lblActionPSMDate;
        private RibbonButton btnActionPSMHistory;
        private System.Windows.Forms.PictureBox imgReportSplit;
        private RibbonButton btnSMSPSMHistory;
        private RibbonButton btnNotOperationPSMHistory;
        private RibbonButton btnDetectPSMHistory;
        private System.Windows.Forms.Panel pnSMSPSM;
        private System.Windows.Forms.Button btnSMSPSMEndDate;
        private System.Windows.Forms.Button btnSMSPSMStartDate;
        private System.Windows.Forms.ComboBox cboSMSPSMLatelyDate;
        private System.Windows.Forms.Label lblSMSPSMDate;
        private System.Windows.Forms.Button btnSMSPSMSelectZone;
        private System.Windows.Forms.ComboBox cboSMSPSMBuilding;
        private System.Windows.Forms.Label lblSMSPSMSelectZone;
        private System.Windows.Forms.Panel pnDetectPSM;
        private System.Windows.Forms.Button btnDetectPSMDateFormat;
        private System.Windows.Forms.NumericUpDown nudDetectPSMSplitUnitDetail;
        private System.Windows.Forms.Label lblDetectPSMSplitUnitDetail;
        private System.Windows.Forms.Label lblDetectPSMViewCount;
        private System.Windows.Forms.Label lblDetectPSMSplitUnit;
        private System.Windows.Forms.ComboBox cboDetectPSMViewCount;
        private System.Windows.Forms.ComboBox cboDetectPSMSplitUnit;
        private System.Windows.Forms.Button btnDetectPSMEndDate;
        private System.Windows.Forms.Button btnDetectPSMStartDate;
        private System.Windows.Forms.ComboBox cboDetectPSMLatelyDate;
        private System.Windows.Forms.Label lblDetectPSMDate;
        private System.Windows.Forms.Button btnDetectPSMSelectZone;
        private System.Windows.Forms.ComboBox cboDetectPSMBuilding;
        private System.Windows.Forms.Label lblDetectPSMSelectZone;
        private System.Windows.Forms.DateTimePicker DatePickerEnd;
        private System.Windows.Forms.DateTimePicker DatePickerStart;
        private System.Windows.Forms.DateTimePicker DatePickerEnd2;
        private System.Windows.Forms.DateTimePicker DatePickerStart2;
        private System.Windows.Forms.DateTimePicker DatePickerSMSPSMEnd;
        private System.Windows.Forms.DateTimePicker DatePickerActionPSMEnd;
        private System.Windows.Forms.DateTimePicker DatePickerSMSPSMStart;
        private System.Windows.Forms.DateTimePicker DatePickerNotOperationPSMEnd;
        private System.Windows.Forms.DateTimePicker DatePickerActionPSMStart;
        private System.Windows.Forms.DateTimePicker DatePickerDetectPSMStart;
        private System.Windows.Forms.DateTimePicker DatePickerNotOperationPSMStart;
        private System.Windows.Forms.DateTimePicker DatePickerDetectPSMEnd;
        private System.Windows.Forms.Panel pnNotOperationPSM;
        private System.Windows.Forms.Button btnNotOperationPSMEndDate;
        private System.Windows.Forms.Button btnNotOperationPSMStartDate;
        private System.Windows.Forms.ComboBox cboNotOperationPSMLatelyDate;
        private System.Windows.Forms.Label lblNotOperationPSMDate;
        private System.Windows.Forms.Button btnNotOperationPSMSelectZone;
        private System.Windows.Forms.ComboBox cboNotOperationPSMBuilding;
        private System.Windows.Forms.Label lblNotOperationPSMSelectZone;
        private System.Windows.Forms.Button btnReactionPSMSelectDisaster;
        private System.Windows.Forms.Button btnReactionSelectDisaster;
        private System.Windows.Forms.Button btnReactionIntrusionSelectDisaster;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Label labelDetectDateFormat;
        private System.Windows.Forms.Label labelDetectPSMDateFormat;
        private RibbonButton btnDetectAnalyze;
        private RibbonButton btnDetectPSMAnalyze;
        private System.Windows.Forms.PictureBox imgReportSplit2;
        private RibbonButton btnDetectIntrusionAnalyze;
        private RibbonButton btnSMSIntrusionHistory;
        private RibbonButton btnReactionIntrusionHistory;
        private RibbonButton btnProcessIntrusionHistory;
        private RibbonButton btnDetectIntrusionHistory;
        private RibbonButton sensorMgrBtn;
        private RibbonButton btnEarthquake;

    }
}