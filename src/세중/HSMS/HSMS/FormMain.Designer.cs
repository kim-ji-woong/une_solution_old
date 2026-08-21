namespace HSMS
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
            if (m_netMgr != null)
                 m_netMgr.ReleaseThread();

            //if (m_safetyChecker!= null)
            //    m_safetyChecker.ReleaseThread();

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
            this.ClockTimer = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.ribbonButton21 = new UnE.GUI.RibbonButton();
            this.panelMiddle = new HSMS.PanelEx();
            this.panelAdminBar = new HSMS.PanelEx();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.btnZoomIn = new UnE.GUI.RibbonButton();
            this.btnPanning = new UnE.GUI.RibbonButton();
            this.btnFullScreen = new UnE.GUI.RibbonButton();
            this.btnSaveHome = new UnE.GUI.RibbonButton();
            this.label4 = new System.Windows.Forms.Label();
            this.cboAlarmList = new System.Windows.Forms.ComboBox();
            this.btnZoomOut = new UnE.GUI.RibbonButton();
            this.btnOrbit = new UnE.GUI.RibbonButton();
            this.btnPick = new UnE.GUI.RibbonButton();
            this.btnHome = new UnE.GUI.RibbonButton();
            this.panelReportBar = new HSMS.PanelEx();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnReportDateEnd = new System.Windows.Forms.Button();
            this.btnReportDateStart = new System.Windows.Forms.Button();
            this.cboReportLatelyDate = new System.Windows.Forms.ComboBox();
            this.cboAlarmStep = new System.Windows.Forms.ComboBox();
            this.panelLeft = new HSMS.PanelEx();
            this.btnDangerZoneLayer = new UnE.GUI.RibbonButton();
            this.btnVehicleLayer = new UnE.GUI.RibbonButton();
            this.btnDangerFacilityLayer = new UnE.GUI.RibbonButton();
            this.btnWorkerLayer = new UnE.GUI.RibbonButton();
            this.panelTop = new HSMS.PanelEx();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.panelReportTab = new HSMS.PanelEx();
            this.btnReportHistory = new UnE.GUI.RibbonButton();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.panelAdminTab = new HSMS.PanelEx();
            this.btnOption = new UnE.GUI.RibbonButton();
            this.btnListAdmin = new UnE.GUI.RibbonButton();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.btnDetectAdmin = new UnE.GUI.RibbonButton();
            this.btnAlarmAdmin = new UnE.GUI.RibbonButton();
            this.btnMessageAdmin = new UnE.GUI.RibbonButton();
            this.btnManagerAdmin = new UnE.GUI.RibbonButton();
            this.btnDeleteAdmin = new UnE.GUI.RibbonButton();
            this.btnSaveAdmin = new UnE.GUI.RibbonButton();
            this.btnDangerZoneAdmin = new UnE.GUI.RibbonButton();
            this.btnFacilityAdmin = new UnE.GUI.RibbonButton();
            this.btnVehicleAdmin = new UnE.GUI.RibbonButton();
            this.btnWorkerAdmin = new UnE.GUI.RibbonButton();
            this.panelMonitoringTab = new HSMS.PanelEx();
            this.btnStatusEnd = new UnE.GUI.RibbonButton();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.panelClock = new HSMS.PanelEx();
            this.labelTime = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelDate = new System.Windows.Forms.Label();
            this.panelStatus = new HSMS.PanelEx();
            this.lblDangerDetail = new System.Windows.Forms.Label();
            this.lblDangerLevel = new System.Windows.Forms.Label();
            this.pictureBoxbell = new System.Windows.Forms.PictureBox();
            this.panelLog = new HSMS.RealTimeInfoPane();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBoxReport = new UnE.GUI.TextPictureBox();
            this.pictureBoxAdmin = new UnE.GUI.TextPictureBox();
            this.pictureBoxMonitoring = new UnE.GUI.TextPictureBox();
            this.panelBottom = new HSMS.PanelEx();
            this.monthCalendar2 = new System.Windows.Forms.MonthCalendar();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.lbCount = new System.Windows.Forms.Label();
            this.panelMiddle.SuspendLayout();
            this.panelAdminBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.panelReportBar.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            this.panelReportTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            this.panelAdminTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            this.panelMonitoringTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.panelClock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxbell)).BeginInit();
            this.panelLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMonitoring)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // ClockTimer
            // 
            this.ClockTimer.Interval = 1000;
            this.ClockTimer.Tick += new System.EventHandler(this.ClockTimer_Tick);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // ribbonButton21
            // 
            this.ribbonButton21.CheckButton = false;
            this.ribbonButton21.CheckedBkgndImage = null;
            this.ribbonButton21.CheckedImage = null;
            this.ribbonButton21.ClickedBackgroundImage = null;
            this.ribbonButton21.ClickedImage = null;
            this.ribbonButton21.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.ribbonButton21.DisabledBkgndImage = null;
            this.ribbonButton21.DisabledImage = null;
            this.ribbonButton21.ID = -1;
            this.ribbonButton21.InitButtonWidth = 60;
            this.ribbonButton21.IsChecked = false;
            this.ribbonButton21.Location = new System.Drawing.Point(1911, 524);
            this.ribbonButton21.MouseOverBkgndImage = null;
            this.ribbonButton21.MouseOverImage = null;
            this.ribbonButton21.Name = "ribbonButton21";
            this.ribbonButton21.NormalImage = null;
            this.ribbonButton21.Owner = null;
            this.ribbonButton21.Size = new System.Drawing.Size(100, 23);
            this.ribbonButton21.TabIndex = 6;
            this.ribbonButton21.Text = "ribbonButton21";
            this.ribbonButton21.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton21.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton21.ToolTipText = "ribbonButton21";
            this.ribbonButton21.UseCustomImageRect = false;
            this.ribbonButton21.UseTextLocation = false;
            this.ribbonButton21.UseVisualStyleBackColor = true;
            // 
            // panelMiddle
            // 
            this.panelMiddle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMiddle.Controls.Add(this.panelAdminBar);
            this.panelMiddle.Controls.Add(this.panelReportBar);
            this.panelMiddle.Location = new System.Drawing.Point(0, 323);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Size = new System.Drawing.Size(1366, 99);
            this.panelMiddle.TabIndex = 46;
            // 
            // panelAdminBar
            // 
            this.panelAdminBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelAdminBar.BackColor = System.Drawing.Color.Transparent;
            this.panelAdminBar.BackgroundImage = global::HSMS.Properties.Resources.toolBar_skin;
            this.panelAdminBar.Controls.Add(this.lbCount);
            this.panelAdminBar.Controls.Add(this.pictureBox7);
            this.panelAdminBar.Controls.Add(this.pictureBox6);
            this.panelAdminBar.Controls.Add(this.btnZoomIn);
            this.panelAdminBar.Controls.Add(this.btnPanning);
            this.panelAdminBar.Controls.Add(this.btnFullScreen);
            this.panelAdminBar.Controls.Add(this.btnSaveHome);
            this.panelAdminBar.Controls.Add(this.label4);
            this.panelAdminBar.Controls.Add(this.cboAlarmList);
            this.panelAdminBar.Controls.Add(this.btnZoomOut);
            this.panelAdminBar.Controls.Add(this.btnOrbit);
            this.panelAdminBar.Controls.Add(this.btnPick);
            this.panelAdminBar.Controls.Add(this.btnHome);
            this.panelAdminBar.Location = new System.Drawing.Point(0, 3);
            this.panelAdminBar.Name = "panelAdminBar";
            this.panelAdminBar.Size = new System.Drawing.Size(1364, 45);
            this.panelAdminBar.TabIndex = 0;
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox7.BackgroundImage = global::HSMS.Properties.Resources.toolBar_Line;
            this.pictureBox7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox7.Location = new System.Drawing.Point(257, 7);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(3, 33);
            this.pictureBox7.TabIndex = 0;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox6.BackgroundImage = global::HSMS.Properties.Resources.toolBar_Line;
            this.pictureBox6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox6.Location = new System.Drawing.Point(131, 7);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(3, 33);
            this.pictureBox6.TabIndex = 0;
            this.pictureBox6.TabStop = false;
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.BackColor = System.Drawing.Color.Transparent;
            this.btnZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnZoomIn.CheckButton = false;
            this.btnZoomIn.CheckedBkgndImage = global::HSMS.Properties.Resources.toolBar_click_BG;
            this.btnZoomIn.CheckedImage = null;
            this.btnZoomIn.ClickedBackgroundImage = null;
            this.btnZoomIn.ClickedImage = null;
            this.btnZoomIn.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 32);
            this.btnZoomIn.DisabledBkgndImage = global::HSMS.Properties.Resources.@__disable1;
            this.btnZoomIn.DisabledImage = null;
            this.btnZoomIn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnZoomIn.ID = -1;
            this.btnZoomIn.InitButtonWidth = 40;
            this.btnZoomIn.IsChecked = false;
            this.btnZoomIn.Location = new System.Drawing.Point(270, 5);
            this.btnZoomIn.MouseOverBkgndImage = global::HSMS.Properties.Resources.toolBar_mouseover_BG;
            this.btnZoomIn.MouseOverImage = null;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.NormalImage = global::HSMS.Properties.Resources._1;
            this.btnZoomIn.Owner = null;
            this.btnZoomIn.Size = new System.Drawing.Size(40, 35);
            this.btnZoomIn.TabIndex = 2;
            this.btnZoomIn.TextLocation = new System.Drawing.Point(0, 0);
            this.btnZoomIn.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnZoomIn.ToolTipText = "";
            this.btnZoomIn.UseCustomImageRect = true;
            this.btnZoomIn.UseTextLocation = false;
            this.btnZoomIn.UseVisualStyleBackColor = false;
            // 
            // btnPanning
            // 
            this.btnPanning.BackColor = System.Drawing.Color.Transparent;
            this.btnPanning.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPanning.CheckButton = false;
            this.btnPanning.CheckedBkgndImage = global::HSMS.Properties.Resources.toolBar_click_BG;
            this.btnPanning.CheckedImage = null;
            this.btnPanning.ClickedBackgroundImage = null;
            this.btnPanning.ClickedImage = null;
            this.btnPanning.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 32);
            this.btnPanning.DisabledBkgndImage = global::HSMS.Properties.Resources.Hand_disable;
            this.btnPanning.DisabledImage = null;
            this.btnPanning.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnPanning.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPanning.ID = -1;
            this.btnPanning.InitButtonWidth = 40;
            this.btnPanning.IsChecked = false;
            this.btnPanning.Location = new System.Drawing.Point(180, 5);
            this.btnPanning.MouseOverBkgndImage = global::HSMS.Properties.Resources.toolBar_mouseover_BG;
            this.btnPanning.MouseOverImage = null;
            this.btnPanning.Name = "btnPanning";
            this.btnPanning.NormalImage = global::HSMS.Properties.Resources.Hand_img;
            this.btnPanning.Owner = null;
            this.btnPanning.Size = new System.Drawing.Size(40, 35);
            this.btnPanning.TabIndex = 2;
            this.btnPanning.TextLocation = new System.Drawing.Point(0, 0);
            this.btnPanning.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnPanning.ToolTipText = "";
            this.btnPanning.UseCustomImageRect = true;
            this.btnPanning.UseTextLocation = false;
            this.btnPanning.UseVisualStyleBackColor = false;
            // 
            // btnFullScreen
            // 
            this.btnFullScreen.BackColor = System.Drawing.Color.Transparent;
            this.btnFullScreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnFullScreen.CheckButton = false;
            this.btnFullScreen.CheckedBkgndImage = global::HSMS.Properties.Resources.toolBar_click_BG;
            this.btnFullScreen.CheckedImage = null;
            this.btnFullScreen.ClickedBackgroundImage = null;
            this.btnFullScreen.ClickedImage = null;
            this.btnFullScreen.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 32);
            this.btnFullScreen.DisabledBkgndImage = global::HSMS.Properties.Resources.Monitor_disable;
            this.btnFullScreen.DisabledImage = null;
            this.btnFullScreen.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnFullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFullScreen.ID = -1;
            this.btnFullScreen.InitButtonWidth = 40;
            this.btnFullScreen.IsChecked = false;
            this.btnFullScreen.Location = new System.Drawing.Point(92, 5);
            this.btnFullScreen.MouseOverBkgndImage = global::HSMS.Properties.Resources.toolBar_mouseover_BG;
            this.btnFullScreen.MouseOverImage = null;
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.NormalImage = global::HSMS.Properties.Resources.Monitor;
            this.btnFullScreen.Owner = null;
            this.btnFullScreen.Size = new System.Drawing.Size(40, 35);
            this.btnFullScreen.TabIndex = 2;
            this.btnFullScreen.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFullScreen.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFullScreen.ToolTipText = "";
            this.btnFullScreen.UseCustomImageRect = true;
            this.btnFullScreen.UseTextLocation = false;
            this.btnFullScreen.UseVisualStyleBackColor = false;
            // 
            // btnSaveHome
            // 
            this.btnSaveHome.BackColor = System.Drawing.Color.Transparent;
            this.btnSaveHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSaveHome.CheckButton = false;
            this.btnSaveHome.CheckedBkgndImage = global::HSMS.Properties.Resources.toolBar_click_BG;
            this.btnSaveHome.CheckedImage = null;
            this.btnSaveHome.ClickedBackgroundImage = null;
            this.btnSaveHome.ClickedImage = null;
            this.btnSaveHome.CustomImageRect = new System.Drawing.Rectangle(0, 0, 38, 32);
            this.btnSaveHome.DisabledBkgndImage = global::HSMS.Properties.Resources.home_set_disable1;
            this.btnSaveHome.DisabledImage = null;
            this.btnSaveHome.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnSaveHome.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSaveHome.ID = -1;
            this.btnSaveHome.InitButtonWidth = 40;
            this.btnSaveHome.IsChecked = false;
            this.btnSaveHome.Location = new System.Drawing.Point(10, 5);
            this.btnSaveHome.MouseOverBkgndImage = null;
            this.btnSaveHome.MouseOverImage = null;
            this.btnSaveHome.Name = "btnSaveHome";
            this.btnSaveHome.NormalImage = global::HSMS.Properties.Resources.home_set_nomal;
            this.btnSaveHome.Owner = null;
            this.btnSaveHome.Size = new System.Drawing.Size(40, 35);
            this.btnSaveHome.TabIndex = 2;
            this.btnSaveHome.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSaveHome.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSaveHome.ToolTipText = "";
            this.btnSaveHome.UseCustomImageRect = true;
            this.btnSaveHome.UseTextLocation = false;
            this.btnSaveHome.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(1248, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 12);
            this.label4.TabIndex = 44;
            this.label4.Text = "상황 선택";
            // 
            // cboAlarmList
            // 
            this.cboAlarmList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlarmList.FormattingEnabled = true;
            this.cboAlarmList.Location = new System.Drawing.Point(1318, 11);
            this.cboAlarmList.Name = "cboAlarmList";
            this.cboAlarmList.Size = new System.Drawing.Size(193, 20);
            this.cboAlarmList.TabIndex = 43;
            this.cboAlarmList.SelectedIndexChanged += new System.EventHandler(this.cboAlarmList_SelectedIndexChanged);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.BackColor = System.Drawing.Color.Transparent;
            this.btnZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnZoomOut.CheckButton = false;
            this.btnZoomOut.CheckedBkgndImage = global::HSMS.Properties.Resources.toolBar_click_BG;
            this.btnZoomOut.CheckedImage = null;
            this.btnZoomOut.ClickedBackgroundImage = null;
            this.btnZoomOut.ClickedImage = null;
            this.btnZoomOut.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 32);
            this.btnZoomOut.DisabledBkgndImage = global::HSMS.Properties.Resources.@__disable;
            this.btnZoomOut.DisabledImage = null;
            this.btnZoomOut.ID = -1;
            this.btnZoomOut.InitButtonWidth = 40;
            this.btnZoomOut.IsChecked = false;
            this.btnZoomOut.Location = new System.Drawing.Point(314, 5);
            this.btnZoomOut.MouseOverBkgndImage = global::HSMS.Properties.Resources.toolBar_mouseover_BG;
            this.btnZoomOut.MouseOverImage = null;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.NormalImage = global::HSMS.Properties.Resources._;
            this.btnZoomOut.Owner = null;
            this.btnZoomOut.Size = new System.Drawing.Size(40, 35);
            this.btnZoomOut.TabIndex = 2;
            this.btnZoomOut.TextLocation = new System.Drawing.Point(0, 0);
            this.btnZoomOut.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnZoomOut.ToolTipText = "";
            this.btnZoomOut.UseCustomImageRect = true;
            this.btnZoomOut.UseTextLocation = false;
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnOrbit
            // 
            this.btnOrbit.BackColor = System.Drawing.Color.Transparent;
            this.btnOrbit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOrbit.CheckButton = false;
            this.btnOrbit.CheckedBkgndImage = global::HSMS.Properties.Resources.toolBar_click_BG;
            this.btnOrbit.CheckedImage = null;
            this.btnOrbit.ClickedBackgroundImage = null;
            this.btnOrbit.ClickedImage = null;
            this.btnOrbit.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 32);
            this.btnOrbit.DisabledBkgndImage = global::HSMS.Properties.Resources.rotate_disable;
            this.btnOrbit.DisabledImage = null;
            this.btnOrbit.ID = -1;
            this.btnOrbit.InitButtonWidth = 40;
            this.btnOrbit.IsChecked = false;
            this.btnOrbit.Location = new System.Drawing.Point(220, 5);
            this.btnOrbit.MouseOverBkgndImage = global::HSMS.Properties.Resources.toolBar_mouseover_BG;
            this.btnOrbit.MouseOverImage = null;
            this.btnOrbit.Name = "btnOrbit";
            this.btnOrbit.NormalImage = global::HSMS.Properties.Resources.rotate_img;
            this.btnOrbit.Owner = null;
            this.btnOrbit.Size = new System.Drawing.Size(40, 35);
            this.btnOrbit.TabIndex = 2;
            this.btnOrbit.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOrbit.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOrbit.ToolTipText = "";
            this.btnOrbit.UseCustomImageRect = true;
            this.btnOrbit.UseTextLocation = false;
            this.btnOrbit.UseVisualStyleBackColor = true;
            // 
            // btnPick
            // 
            this.btnPick.BackColor = System.Drawing.Color.Transparent;
            this.btnPick.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPick.CheckButton = false;
            this.btnPick.CheckedBkgndImage = global::HSMS.Properties.Resources.toolBar_click_BG;
            this.btnPick.CheckedImage = null;
            this.btnPick.ClickedBackgroundImage = null;
            this.btnPick.ClickedImage = null;
            this.btnPick.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 32);
            this.btnPick.DisabledBkgndImage = global::HSMS.Properties.Resources.arrow_disable;
            this.btnPick.DisabledImage = null;
            this.btnPick.ID = -1;
            this.btnPick.InitButtonWidth = 40;
            this.btnPick.IsChecked = false;
            this.btnPick.Location = new System.Drawing.Point(139, 5);
            this.btnPick.MouseOverBkgndImage = global::HSMS.Properties.Resources.toolBar_mouseover_BG;
            this.btnPick.MouseOverImage = null;
            this.btnPick.Name = "btnPick";
            this.btnPick.NormalImage = global::HSMS.Properties.Resources.arrow_img;
            this.btnPick.Owner = null;
            this.btnPick.Size = new System.Drawing.Size(40, 35);
            this.btnPick.TabIndex = 2;
            this.btnPick.TextLocation = new System.Drawing.Point(0, 0);
            this.btnPick.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnPick.ToolTipText = "";
            this.btnPick.UseCustomImageRect = true;
            this.btnPick.UseTextLocation = false;
            this.btnPick.UseVisualStyleBackColor = true;
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.Transparent;
            this.btnHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHome.CheckButton = false;
            this.btnHome.CheckedBkgndImage = global::HSMS.Properties.Resources.toolBar_click_BG;
            this.btnHome.CheckedImage = null;
            this.btnHome.ClickedBackgroundImage = null;
            this.btnHome.ClickedImage = null;
            this.btnHome.CustomImageRect = new System.Drawing.Rectangle(0, 0, 40, 32);
            this.btnHome.DisabledBkgndImage = global::HSMS.Properties.Resources.Home_disable;
            this.btnHome.DisabledImage = null;
            this.btnHome.ID = -1;
            this.btnHome.InitButtonWidth = 40;
            this.btnHome.IsChecked = false;
            this.btnHome.Location = new System.Drawing.Point(50, 5);
            this.btnHome.MouseOverBkgndImage = global::HSMS.Properties.Resources.toolBar_mouseover_BG;
            this.btnHome.MouseOverImage = null;
            this.btnHome.Name = "btnHome";
            this.btnHome.NormalImage = global::HSMS.Properties.Resources.Home_img;
            this.btnHome.Owner = null;
            this.btnHome.Size = new System.Drawing.Size(40, 35);
            this.btnHome.TabIndex = 2;
            this.btnHome.TextLocation = new System.Drawing.Point(0, 0);
            this.btnHome.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnHome.ToolTipText = "";
            this.btnHome.UseCustomImageRect = true;
            this.btnHome.UseTextLocation = false;
            this.btnHome.UseVisualStyleBackColor = true;
            // 
            // panelReportBar
            // 
            this.panelReportBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelReportBar.BackColor = System.Drawing.Color.Transparent;
            this.panelReportBar.BackgroundImage = global::HSMS.Properties.Resources.toolBar_skin;
            this.panelReportBar.Controls.Add(this.btnSearch);
            this.panelReportBar.Controls.Add(this.btnReportDateEnd);
            this.panelReportBar.Controls.Add(this.btnReportDateStart);
            this.panelReportBar.Controls.Add(this.cboReportLatelyDate);
            this.panelReportBar.Controls.Add(this.cboAlarmStep);
            this.panelReportBar.Location = new System.Drawing.Point(0, 51);
            this.panelReportBar.Name = "panelReportBar";
            this.panelReportBar.Size = new System.Drawing.Size(1364, 45);
            this.panelReportBar.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(980, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "조회";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnReportDateEnd
            // 
            this.btnReportDateEnd.Location = new System.Drawing.Point(306, 11);
            this.btnReportDateEnd.Name = "btnReportDateEnd";
            this.btnReportDateEnd.Size = new System.Drawing.Size(120, 23);
            this.btnReportDateEnd.TabIndex = 8;
            this.btnReportDateEnd.UseVisualStyleBackColor = true;
            this.btnReportDateEnd.Click += new System.EventHandler(this.btnReportDateEnd_Click);
            // 
            // btnReportDateStart
            // 
            this.btnReportDateStart.Location = new System.Drawing.Point(173, 11);
            this.btnReportDateStart.Name = "btnReportDateStart";
            this.btnReportDateStart.Size = new System.Drawing.Size(120, 23);
            this.btnReportDateStart.TabIndex = 7;
            this.btnReportDateStart.UseVisualStyleBackColor = true;
            this.btnReportDateStart.Click += new System.EventHandler(this.button1_Click);
            // 
            // cboReportLatelyDate
            // 
            this.cboReportLatelyDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReportLatelyDate.FormattingEnabled = true;
            this.cboReportLatelyDate.Location = new System.Drawing.Point(437, 13);
            this.cboReportLatelyDate.Name = "cboReportLatelyDate";
            this.cboReportLatelyDate.Size = new System.Drawing.Size(121, 20);
            this.cboReportLatelyDate.TabIndex = 3;
            this.cboReportLatelyDate.SelectedIndexChanged += new System.EventHandler(this.cboReportLatelyDate_SelectedIndexChanged);
            // 
            // cboAlarmStep
            // 
            this.cboAlarmStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlarmStep.FormattingEnabled = true;
            this.cboAlarmStep.Location = new System.Drawing.Point(835, 13);
            this.cboAlarmStep.Name = "cboAlarmStep";
            this.cboAlarmStep.Size = new System.Drawing.Size(121, 20);
            this.cboAlarmStep.TabIndex = 6;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.panelLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLeft.Controls.Add(this.btnDangerZoneLayer);
            this.panelLeft.Controls.Add(this.btnVehicleLayer);
            this.panelLeft.Controls.Add(this.btnDangerFacilityLayer);
            this.panelLeft.Controls.Add(this.btnWorkerLayer);
            this.panelLeft.Location = new System.Drawing.Point(8, 483);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(65, 445);
            this.panelLeft.TabIndex = 5;
            // 
            // btnDangerZoneLayer
            // 
            this.btnDangerZoneLayer.BackColor = System.Drawing.Color.Transparent;
            this.btnDangerZoneLayer.BackgroundImage = global::HSMS.Properties.Resources.leftBar_dangerZone_nomal;
            this.btnDangerZoneLayer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDangerZoneLayer.CheckButton = false;
            this.btnDangerZoneLayer.CheckedBkgndImage = null;
            this.btnDangerZoneLayer.CheckedImage = null;
            this.btnDangerZoneLayer.ClickedBackgroundImage = null;
            this.btnDangerZoneLayer.ClickedImage = null;
            this.btnDangerZoneLayer.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnDangerZoneLayer.DisabledBkgndImage = null;
            this.btnDangerZoneLayer.DisabledImage = null;
            this.btnDangerZoneLayer.ID = -1;
            this.btnDangerZoneLayer.InitButtonWidth = 60;
            this.btnDangerZoneLayer.IsChecked = false;
            this.btnDangerZoneLayer.Location = new System.Drawing.Point(0, 266);
            this.btnDangerZoneLayer.MouseOverBkgndImage = null;
            this.btnDangerZoneLayer.MouseOverImage = null;
            this.btnDangerZoneLayer.Name = "btnDangerZoneLayer";
            this.btnDangerZoneLayer.NormalImage = null;
            this.btnDangerZoneLayer.Owner = null;
            this.btnDangerZoneLayer.Size = new System.Drawing.Size(60, 80);
            this.btnDangerZoneLayer.TabIndex = 2;
            this.btnDangerZoneLayer.TextLocation = new System.Drawing.Point(0, 0);
            this.btnDangerZoneLayer.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDangerZoneLayer.ToolTipText = "";
            this.btnDangerZoneLayer.UseCustomImageRect = false;
            this.btnDangerZoneLayer.UseTextLocation = false;
            this.btnDangerZoneLayer.UseVisualStyleBackColor = false;
            // 
            // btnVehicleLayer
            // 
            this.btnVehicleLayer.BackColor = System.Drawing.Color.Transparent;
            this.btnVehicleLayer.BackgroundImage = global::HSMS.Properties.Resources.leftBar_vehicle_nomal;
            this.btnVehicleLayer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnVehicleLayer.CheckButton = false;
            this.btnVehicleLayer.CheckedBkgndImage = null;
            this.btnVehicleLayer.CheckedImage = null;
            this.btnVehicleLayer.ClickedBackgroundImage = null;
            this.btnVehicleLayer.ClickedImage = null;
            this.btnVehicleLayer.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnVehicleLayer.DisabledBkgndImage = null;
            this.btnVehicleLayer.DisabledImage = null;
            this.btnVehicleLayer.ForeColor = System.Drawing.Color.DarkGray;
            this.btnVehicleLayer.ID = -1;
            this.btnVehicleLayer.InitButtonWidth = 60;
            this.btnVehicleLayer.IsChecked = false;
            this.btnVehicleLayer.Location = new System.Drawing.Point(0, 94);
            this.btnVehicleLayer.MouseOverBkgndImage = null;
            this.btnVehicleLayer.MouseOverImage = null;
            this.btnVehicleLayer.Name = "btnVehicleLayer";
            this.btnVehicleLayer.NormalImage = null;
            this.btnVehicleLayer.Owner = null;
            this.btnVehicleLayer.Size = new System.Drawing.Size(60, 80);
            this.btnVehicleLayer.TabIndex = 2;
            this.btnVehicleLayer.Text = "차량";
            this.btnVehicleLayer.TextLocation = new System.Drawing.Point(0, 0);
            this.btnVehicleLayer.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnVehicleLayer.ToolTipText = "차량";
            this.btnVehicleLayer.UseCustomImageRect = false;
            this.btnVehicleLayer.UseTextLocation = false;
            this.btnVehicleLayer.UseVisualStyleBackColor = false;
            // 
            // btnDangerFacilityLayer
            // 
            this.btnDangerFacilityLayer.BackColor = System.Drawing.Color.Transparent;
            this.btnDangerFacilityLayer.BackgroundImage = global::HSMS.Properties.Resources.leftBar_dangerfacility_nomal;
            this.btnDangerFacilityLayer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDangerFacilityLayer.CheckButton = false;
            this.btnDangerFacilityLayer.CheckedBkgndImage = null;
            this.btnDangerFacilityLayer.CheckedImage = null;
            this.btnDangerFacilityLayer.ClickedBackgroundImage = null;
            this.btnDangerFacilityLayer.ClickedImage = null;
            this.btnDangerFacilityLayer.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnDangerFacilityLayer.DisabledBkgndImage = null;
            this.btnDangerFacilityLayer.DisabledImage = null;
            this.btnDangerFacilityLayer.ForeColor = System.Drawing.Color.DarkGray;
            this.btnDangerFacilityLayer.ID = -1;
            this.btnDangerFacilityLayer.InitButtonWidth = 60;
            this.btnDangerFacilityLayer.IsChecked = false;
            this.btnDangerFacilityLayer.Location = new System.Drawing.Point(0, 180);
            this.btnDangerFacilityLayer.MouseOverBkgndImage = null;
            this.btnDangerFacilityLayer.MouseOverImage = null;
            this.btnDangerFacilityLayer.Name = "btnDangerFacilityLayer";
            this.btnDangerFacilityLayer.NormalImage = null;
            this.btnDangerFacilityLayer.Owner = null;
            this.btnDangerFacilityLayer.Size = new System.Drawing.Size(60, 80);
            this.btnDangerFacilityLayer.TabIndex = 2;
            this.btnDangerFacilityLayer.Text = "시설물";
            this.btnDangerFacilityLayer.TextLocation = new System.Drawing.Point(0, 0);
            this.btnDangerFacilityLayer.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDangerFacilityLayer.ToolTipText = "시설물";
            this.btnDangerFacilityLayer.UseCustomImageRect = false;
            this.btnDangerFacilityLayer.UseTextLocation = false;
            this.btnDangerFacilityLayer.UseVisualStyleBackColor = false;
            // 
            // btnWorkerLayer
            // 
            this.btnWorkerLayer.BackColor = System.Drawing.Color.Transparent;
            this.btnWorkerLayer.BackgroundImage = global::HSMS.Properties.Resources.leftBar_worker_nomal;
            this.btnWorkerLayer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnWorkerLayer.CheckButton = false;
            this.btnWorkerLayer.CheckedBkgndImage = null;
            this.btnWorkerLayer.CheckedImage = null;
            this.btnWorkerLayer.ClickedBackgroundImage = null;
            this.btnWorkerLayer.ClickedImage = null;
            this.btnWorkerLayer.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnWorkerLayer.DisabledBkgndImage = null;
            this.btnWorkerLayer.DisabledImage = null;
            this.btnWorkerLayer.ForeColor = System.Drawing.Color.DarkGray;
            this.btnWorkerLayer.ID = -1;
            this.btnWorkerLayer.InitButtonWidth = 60;
            this.btnWorkerLayer.IsChecked = false;
            this.btnWorkerLayer.Location = new System.Drawing.Point(0, 8);
            this.btnWorkerLayer.MouseOverBkgndImage = null;
            this.btnWorkerLayer.MouseOverImage = null;
            this.btnWorkerLayer.Name = "btnWorkerLayer";
            this.btnWorkerLayer.NormalImage = null;
            this.btnWorkerLayer.Owner = null;
            this.btnWorkerLayer.Size = new System.Drawing.Size(60, 80);
            this.btnWorkerLayer.TabIndex = 2;
            this.btnWorkerLayer.Text = "작업자";
            this.btnWorkerLayer.TextLocation = new System.Drawing.Point(0, 0);
            this.btnWorkerLayer.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnWorkerLayer.ToolTipText = "작업자";
            this.btnWorkerLayer.UseCustomImageRect = false;
            this.btnWorkerLayer.UseTextLocation = false;
            this.btnWorkerLayer.UseVisualStyleBackColor = false;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.Transparent;
            this.panelTop.BackgroundImage = global::HSMS.Properties.Resources.PanelTop_skin;
            this.panelTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelTop.Controls.Add(this.pictureBox9);
            this.panelTop.Controls.Add(this.panelReportTab);
            this.panelTop.Controls.Add(this.pictureBox8);
            this.panelTop.Controls.Add(this.panelAdminTab);
            this.panelTop.Controls.Add(this.panelMonitoringTab);
            this.panelTop.Controls.Add(this.pictureBoxReport);
            this.panelTop.Controls.Add(this.pictureBoxAdmin);
            this.panelTop.Controls.Add(this.pictureBoxMonitoring);
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1914, 324);
            this.panelTop.TabIndex = 0;
            this.panelTop.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDoubleClick);
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            this.panelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseUp);
            // 
            // pictureBox9
            // 
            this.pictureBox9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox9.Location = new System.Drawing.Point(199, 1);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(2, 41);
            this.pictureBox9.TabIndex = 0;
            this.pictureBox9.TabStop = false;
            // 
            // panelReportTab
            // 
            this.panelReportTab.BackColor = System.Drawing.Color.Transparent;
            this.panelReportTab.Controls.Add(this.btnReportHistory);
            this.panelReportTab.Location = new System.Drawing.Point(6, 228);
            this.panelReportTab.Name = "panelReportTab";
            this.panelReportTab.Size = new System.Drawing.Size(1905, 87);
            this.panelReportTab.TabIndex = 42;
            // 
            // btnReportHistory
            // 
            this.btnReportHistory.BackgroundImage = global::HSMS.Properties.Resources.btnhistoryCheck_nomal;
            this.btnReportHistory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReportHistory.CheckButton = false;
            this.btnReportHistory.CheckedBkgndImage = global::HSMS.Properties.Resources.btnhistoryCheck_over;
            this.btnReportHistory.CheckedImage = null;
            this.btnReportHistory.ClickedBackgroundImage = null;
            this.btnReportHistory.ClickedImage = null;
            this.btnReportHistory.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnReportHistory.DisabledBkgndImage = global::HSMS.Properties.Resources.btnhistoryCheck_disable;
            this.btnReportHistory.DisabledImage = null;
            this.btnReportHistory.ID = -1;
            this.btnReportHistory.InitButtonWidth = 60;
            this.btnReportHistory.IsChecked = false;
            this.btnReportHistory.Location = new System.Drawing.Point(14, 3);
            this.btnReportHistory.MouseOverBkgndImage = global::HSMS.Properties.Resources.btnhistoryCheck_over;
            this.btnReportHistory.MouseOverImage = null;
            this.btnReportHistory.Name = "btnReportHistory";
            this.btnReportHistory.NormalImage = null;
            this.btnReportHistory.Owner = null;
            this.btnReportHistory.Size = new System.Drawing.Size(60, 81);
            this.btnReportHistory.TabIndex = 2;
            this.btnReportHistory.Text = "이력조회";
            this.btnReportHistory.TextLocation = new System.Drawing.Point(0, 0);
            this.btnReportHistory.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnReportHistory.ToolTipText = "이력조회";
            this.btnReportHistory.UseCustomImageRect = false;
            this.btnReportHistory.UseTextLocation = false;
            this.btnReportHistory.UseVisualStyleBackColor = true;
            // 
            // pictureBox8
            // 
            this.pictureBox8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox8.Location = new System.Drawing.Point(97, 1);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(2, 41);
            this.pictureBox8.TabIndex = 0;
            this.pictureBox8.TabStop = false;
            // 
            // panelAdminTab
            // 
            this.panelAdminTab.BackColor = System.Drawing.Color.Transparent;
            this.panelAdminTab.Controls.Add(this.btnOption);
            this.panelAdminTab.Controls.Add(this.btnListAdmin);
            this.panelAdminTab.Controls.Add(this.pictureBox12);
            this.panelAdminTab.Controls.Add(this.pictureBox11);
            this.panelAdminTab.Controls.Add(this.pictureBox10);
            this.panelAdminTab.Controls.Add(this.btnDetectAdmin);
            this.panelAdminTab.Controls.Add(this.btnAlarmAdmin);
            this.panelAdminTab.Controls.Add(this.btnMessageAdmin);
            this.panelAdminTab.Controls.Add(this.btnManagerAdmin);
            this.panelAdminTab.Controls.Add(this.btnDeleteAdmin);
            this.panelAdminTab.Controls.Add(this.btnSaveAdmin);
            this.panelAdminTab.Controls.Add(this.btnDangerZoneAdmin);
            this.panelAdminTab.Controls.Add(this.btnFacilityAdmin);
            this.panelAdminTab.Controls.Add(this.btnVehicleAdmin);
            this.panelAdminTab.Controls.Add(this.btnWorkerAdmin);
            this.panelAdminTab.Location = new System.Drawing.Point(5, 136);
            this.panelAdminTab.Name = "panelAdminTab";
            this.panelAdminTab.Size = new System.Drawing.Size(1905, 87);
            this.panelAdminTab.TabIndex = 42;
            this.panelAdminTab.Paint += new System.Windows.Forms.PaintEventHandler(this.panelAdminTab_Paint);
            // 
            // btnOption
            // 
            this.btnOption.BackgroundImage = global::HSMS.Properties.Resources.option_normal;
            this.btnOption.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOption.CheckButton = false;
            this.btnOption.CheckedBkgndImage = global::HSMS.Properties.Resources.option_over;
            this.btnOption.CheckedImage = null;
            this.btnOption.ClickedBackgroundImage = null;
            this.btnOption.ClickedImage = null;
            this.btnOption.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnOption.DisabledBkgndImage = global::HSMS.Properties.Resources.option_disable;
            this.btnOption.DisabledImage = null;
            this.btnOption.ID = -1;
            this.btnOption.InitButtonWidth = 60;
            this.btnOption.IsChecked = false;
            this.btnOption.Location = new System.Drawing.Point(1041, 5);
            this.btnOption.MouseOverBkgndImage = global::HSMS.Properties.Resources.option_over;
            this.btnOption.MouseOverImage = null;
            this.btnOption.Name = "btnOption";
            this.btnOption.NormalImage = null;
            this.btnOption.Owner = null;
            this.btnOption.Size = new System.Drawing.Size(60, 81);
            this.btnOption.TabIndex = 2;
            this.btnOption.Text = "옵션";
            this.btnOption.TextLocation = new System.Drawing.Point(0, 58);
            this.btnOption.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnOption.ToolTipText = "옵션";
            this.btnOption.UseCustomImageRect = false;
            this.btnOption.UseTextLocation = true;
            this.btnOption.UseVisualStyleBackColor = true;
            // 
            // btnListAdmin
            // 
            this.btnListAdmin.BackgroundImage = global::HSMS.Properties.Resources.btnList_nomal;
            this.btnListAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnListAdmin.CheckButton = false;
            this.btnListAdmin.CheckedBkgndImage = global::HSMS.Properties.Resources.btnList_over;
            this.btnListAdmin.CheckedImage = null;
            this.btnListAdmin.ClickedBackgroundImage = null;
            this.btnListAdmin.ClickedImage = null;
            this.btnListAdmin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnListAdmin.DisabledBkgndImage = global::HSMS.Properties.Resources.btnList_disable;
            this.btnListAdmin.DisabledImage = null;
            this.btnListAdmin.ID = -1;
            this.btnListAdmin.InitButtonWidth = 60;
            this.btnListAdmin.IsChecked = false;
            this.btnListAdmin.Location = new System.Drawing.Point(975, 5);
            this.btnListAdmin.MouseOverBkgndImage = global::HSMS.Properties.Resources.btnList_over;
            this.btnListAdmin.MouseOverImage = null;
            this.btnListAdmin.Name = "btnListAdmin";
            this.btnListAdmin.NormalImage = null;
            this.btnListAdmin.Owner = null;
            this.btnListAdmin.Size = new System.Drawing.Size(60, 81);
            this.btnListAdmin.TabIndex = 2;
            this.btnListAdmin.Text = "리스트";
            this.btnListAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnListAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnListAdmin.ToolTipText = "리스트";
            this.btnListAdmin.UseCustomImageRect = false;
            this.btnListAdmin.UseTextLocation = true;
            this.btnListAdmin.UseVisualStyleBackColor = true;
            // 
            // pictureBox12
            // 
            this.pictureBox12.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox12.BackgroundImage = global::HSMS.Properties.Resources.skin_line_img;
            this.pictureBox12.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox12.Location = new System.Drawing.Point(947, 4);
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.Size = new System.Drawing.Size(4, 80);
            this.pictureBox12.TabIndex = 0;
            this.pictureBox12.TabStop = false;
            // 
            // pictureBox11
            // 
            this.pictureBox11.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox11.BackgroundImage = global::HSMS.Properties.Resources.skin_line_img;
            this.pictureBox11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox11.Location = new System.Drawing.Point(593, 4);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(3, 80);
            this.pictureBox11.TabIndex = 0;
            this.pictureBox11.TabStop = false;
            // 
            // pictureBox10
            // 
            this.pictureBox10.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox10.BackgroundImage = global::HSMS.Properties.Resources.skin_line_img;
            this.pictureBox10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox10.Location = new System.Drawing.Point(338, 4);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(3, 80);
            this.pictureBox10.TabIndex = 0;
            this.pictureBox10.TabStop = false;
            // 
            // btnDetectAdmin
            // 
            this.btnDetectAdmin.BackgroundImage = global::HSMS.Properties.Resources.btnDetect_nomal;
            this.btnDetectAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDetectAdmin.CheckButton = false;
            this.btnDetectAdmin.CheckedBkgndImage = global::HSMS.Properties.Resources.btnDetect_over;
            this.btnDetectAdmin.CheckedImage = null;
            this.btnDetectAdmin.ClickedBackgroundImage = null;
            this.btnDetectAdmin.ClickedImage = null;
            this.btnDetectAdmin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnDetectAdmin.DisabledBkgndImage = global::HSMS.Properties.Resources.btnDetect_disable;
            this.btnDetectAdmin.DisabledImage = null;
            this.btnDetectAdmin.ID = -1;
            this.btnDetectAdmin.InitButtonWidth = 60;
            this.btnDetectAdmin.IsChecked = false;
            this.btnDetectAdmin.Location = new System.Drawing.Point(867, 5);
            this.btnDetectAdmin.MouseOverBkgndImage = global::HSMS.Properties.Resources.btnDetect_over;
            this.btnDetectAdmin.MouseOverImage = null;
            this.btnDetectAdmin.Name = "btnDetectAdmin";
            this.btnDetectAdmin.NormalImage = null;
            this.btnDetectAdmin.Owner = null;
            this.btnDetectAdmin.Size = new System.Drawing.Size(60, 81);
            this.btnDetectAdmin.TabIndex = 2;
            this.btnDetectAdmin.Text = "탐지관리";
            this.btnDetectAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnDetectAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnDetectAdmin.ToolTipText = "탐지관리";
            this.btnDetectAdmin.UseCustomImageRect = false;
            this.btnDetectAdmin.UseTextLocation = true;
            this.btnDetectAdmin.UseVisualStyleBackColor = true;
            // 
            // btnAlarmAdmin
            // 
            this.btnAlarmAdmin.BackgroundImage = global::HSMS.Properties.Resources.btnAlarmDistancenomal;
            this.btnAlarmAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAlarmAdmin.CheckButton = false;
            this.btnAlarmAdmin.CheckedBkgndImage = global::HSMS.Properties.Resources.btnAlarmDistance_over;
            this.btnAlarmAdmin.CheckedImage = null;
            this.btnAlarmAdmin.ClickedBackgroundImage = null;
            this.btnAlarmAdmin.ClickedImage = null;
            this.btnAlarmAdmin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnAlarmAdmin.DisabledBkgndImage = global::HSMS.Properties.Resources.btnAlarmDistance_disable;
            this.btnAlarmAdmin.DisabledImage = null;
            this.btnAlarmAdmin.ID = -1;
            this.btnAlarmAdmin.InitButtonWidth = 60;
            this.btnAlarmAdmin.IsChecked = false;
            this.btnAlarmAdmin.Location = new System.Drawing.Point(801, 5);
            this.btnAlarmAdmin.MouseOverBkgndImage = global::HSMS.Properties.Resources.btnAlarmDistance_over;
            this.btnAlarmAdmin.MouseOverImage = null;
            this.btnAlarmAdmin.Name = "btnAlarmAdmin";
            this.btnAlarmAdmin.NormalImage = null;
            this.btnAlarmAdmin.Owner = null;
            this.btnAlarmAdmin.Size = new System.Drawing.Size(60, 81);
            this.btnAlarmAdmin.TabIndex = 2;
            this.btnAlarmAdmin.Text = "알람거리";
            this.btnAlarmAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnAlarmAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnAlarmAdmin.ToolTipText = "알람거리";
            this.btnAlarmAdmin.UseCustomImageRect = false;
            this.btnAlarmAdmin.UseTextLocation = true;
            this.btnAlarmAdmin.UseVisualStyleBackColor = true;
            // 
            // btnMessageAdmin
            // 
            this.btnMessageAdmin.BackgroundImage = global::HSMS.Properties.Resources.btnMessage_nomal;
            this.btnMessageAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMessageAdmin.CheckButton = false;
            this.btnMessageAdmin.CheckedBkgndImage = global::HSMS.Properties.Resources.btnMessage_over;
            this.btnMessageAdmin.CheckedImage = null;
            this.btnMessageAdmin.ClickedBackgroundImage = null;
            this.btnMessageAdmin.ClickedImage = null;
            this.btnMessageAdmin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnMessageAdmin.DisabledBkgndImage = global::HSMS.Properties.Resources.btnMessage_disable;
            this.btnMessageAdmin.DisabledImage = null;
            this.btnMessageAdmin.ID = -1;
            this.btnMessageAdmin.InitButtonWidth = 60;
            this.btnMessageAdmin.IsChecked = false;
            this.btnMessageAdmin.Location = new System.Drawing.Point(735, 5);
            this.btnMessageAdmin.MouseOverBkgndImage = global::HSMS.Properties.Resources.btnMessage_over;
            this.btnMessageAdmin.MouseOverImage = null;
            this.btnMessageAdmin.Name = "btnMessageAdmin";
            this.btnMessageAdmin.NormalImage = null;
            this.btnMessageAdmin.Owner = null;
            this.btnMessageAdmin.Size = new System.Drawing.Size(68, 81);
            this.btnMessageAdmin.TabIndex = 2;
            this.btnMessageAdmin.Text = "메세지관리";
            this.btnMessageAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnMessageAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnMessageAdmin.ToolTipText = "메세지관리";
            this.btnMessageAdmin.UseCustomImageRect = false;
            this.btnMessageAdmin.UseTextLocation = true;
            this.btnMessageAdmin.UseVisualStyleBackColor = true;
            // 
            // btnManagerAdmin
            // 
            this.btnManagerAdmin.BackgroundImage = global::HSMS.Properties.Resources.btnManager_nomal;
            this.btnManagerAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnManagerAdmin.CheckButton = false;
            this.btnManagerAdmin.CheckedBkgndImage = global::HSMS.Properties.Resources.btnManager_over;
            this.btnManagerAdmin.CheckedImage = null;
            this.btnManagerAdmin.ClickedBackgroundImage = null;
            this.btnManagerAdmin.ClickedImage = null;
            this.btnManagerAdmin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnManagerAdmin.DisabledBkgndImage = global::HSMS.Properties.Resources.btnManager_disable;
            this.btnManagerAdmin.DisabledImage = null;
            this.btnManagerAdmin.ID = -1;
            this.btnManagerAdmin.InitButtonWidth = 60;
            this.btnManagerAdmin.IsChecked = false;
            this.btnManagerAdmin.Location = new System.Drawing.Point(656, 5);
            this.btnManagerAdmin.MouseOverBkgndImage = global::HSMS.Properties.Resources.btnManager_over;
            this.btnManagerAdmin.MouseOverImage = null;
            this.btnManagerAdmin.Name = "btnManagerAdmin";
            this.btnManagerAdmin.NormalImage = null;
            this.btnManagerAdmin.Owner = null;
            this.btnManagerAdmin.Size = new System.Drawing.Size(68, 81);
            this.btnManagerAdmin.TabIndex = 2;
            this.btnManagerAdmin.Text = "담당자관리";
            this.btnManagerAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnManagerAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnManagerAdmin.ToolTipText = "담당자관리";
            this.btnManagerAdmin.UseCustomImageRect = false;
            this.btnManagerAdmin.UseTextLocation = true;
            this.btnManagerAdmin.UseVisualStyleBackColor = true;
            // 
            // btnDeleteAdmin
            // 
            this.btnDeleteAdmin.CheckButton = false;
            this.btnDeleteAdmin.CheckedBkgndImage = null;
            this.btnDeleteAdmin.CheckedImage = null;
            this.btnDeleteAdmin.ClickedBackgroundImage = null;
            this.btnDeleteAdmin.ClickedImage = null;
            this.btnDeleteAdmin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnDeleteAdmin.DisabledBkgndImage = null;
            this.btnDeleteAdmin.DisabledImage = null;
            this.btnDeleteAdmin.ID = -1;
            this.btnDeleteAdmin.InitButtonWidth = 60;
            this.btnDeleteAdmin.IsChecked = false;
            this.btnDeleteAdmin.Location = new System.Drawing.Point(470, 5);
            this.btnDeleteAdmin.MouseOverBkgndImage = null;
            this.btnDeleteAdmin.MouseOverImage = null;
            this.btnDeleteAdmin.Name = "btnDeleteAdmin";
            this.btnDeleteAdmin.NormalImage = null;
            this.btnDeleteAdmin.Owner = null;
            this.btnDeleteAdmin.Size = new System.Drawing.Size(60, 81);
            this.btnDeleteAdmin.TabIndex = 2;
            this.btnDeleteAdmin.Text = "저장취소";
            this.btnDeleteAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnDeleteAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDeleteAdmin.ToolTipText = "저장취소";
            this.btnDeleteAdmin.UseCustomImageRect = false;
            this.btnDeleteAdmin.UseTextLocation = true;
            this.btnDeleteAdmin.UseVisualStyleBackColor = true;
            // 
            // btnSaveAdmin
            // 
            this.btnSaveAdmin.CheckButton = false;
            this.btnSaveAdmin.CheckedBkgndImage = global::HSMS.Properties.Resources.btnSave_over;
            this.btnSaveAdmin.CheckedImage = null;
            this.btnSaveAdmin.ClickedBackgroundImage = null;
            this.btnSaveAdmin.ClickedImage = null;
            this.btnSaveAdmin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnSaveAdmin.DisabledBkgndImage = null;
            this.btnSaveAdmin.DisabledImage = null;
            this.btnSaveAdmin.ID = -1;
            this.btnSaveAdmin.InitButtonWidth = 60;
            this.btnSaveAdmin.IsChecked = false;
            this.btnSaveAdmin.Location = new System.Drawing.Point(390, 5);
            this.btnSaveAdmin.MouseOverBkgndImage = null;
            this.btnSaveAdmin.MouseOverImage = null;
            this.btnSaveAdmin.Name = "btnSaveAdmin";
            this.btnSaveAdmin.NormalImage = null;
            this.btnSaveAdmin.Owner = null;
            this.btnSaveAdmin.Size = new System.Drawing.Size(60, 81);
            this.btnSaveAdmin.TabIndex = 2;
            this.btnSaveAdmin.Text = "저장";
            this.btnSaveAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnSaveAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSaveAdmin.ToolTipText = "저장";
            this.btnSaveAdmin.UseCustomImageRect = false;
            this.btnSaveAdmin.UseTextLocation = true;
            this.btnSaveAdmin.UseVisualStyleBackColor = true;
            // 
            // btnDangerZoneAdmin
            // 
            this.btnDangerZoneAdmin.BackColor = System.Drawing.Color.Transparent;
            this.btnDangerZoneAdmin.BackgroundImage = global::HSMS.Properties.Resources.btndangerZone_nomal;
            this.btnDangerZoneAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDangerZoneAdmin.CheckButton = false;
            this.btnDangerZoneAdmin.CheckedBkgndImage = global::HSMS.Properties.Resources.btndangerZone_over;
            this.btnDangerZoneAdmin.CheckedImage = null;
            this.btnDangerZoneAdmin.ClickedBackgroundImage = null;
            this.btnDangerZoneAdmin.ClickedImage = null;
            this.btnDangerZoneAdmin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnDangerZoneAdmin.DisabledBkgndImage = global::HSMS.Properties.Resources.btndangerZone_disable;
            this.btnDangerZoneAdmin.DisabledImage = null;
            this.btnDangerZoneAdmin.ID = -1;
            this.btnDangerZoneAdmin.InitButtonWidth = 60;
            this.btnDangerZoneAdmin.IsChecked = false;
            this.btnDangerZoneAdmin.Location = new System.Drawing.Point(234, 5);
            this.btnDangerZoneAdmin.MouseOverBkgndImage = global::HSMS.Properties.Resources.btndangerZone_over;
            this.btnDangerZoneAdmin.MouseOverImage = null;
            this.btnDangerZoneAdmin.Name = "btnDangerZoneAdmin";
            this.btnDangerZoneAdmin.NormalImage = null;
            this.btnDangerZoneAdmin.Owner = null;
            this.btnDangerZoneAdmin.Size = new System.Drawing.Size(60, 81);
            this.btnDangerZoneAdmin.TabIndex = 2;
            this.btnDangerZoneAdmin.Text = "위험영역";
            this.btnDangerZoneAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnDangerZoneAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDangerZoneAdmin.ToolTipText = "위험영역";
            this.btnDangerZoneAdmin.UseCustomImageRect = false;
            this.btnDangerZoneAdmin.UseTextLocation = true;
            this.btnDangerZoneAdmin.UseVisualStyleBackColor = false;
            // 
            // btnFacilityAdmin
            // 
            this.btnFacilityAdmin.BackgroundImage = global::HSMS.Properties.Resources.btndangerFacility_nomal;
            this.btnFacilityAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFacilityAdmin.CheckButton = false;
            this.btnFacilityAdmin.CheckedBkgndImage = global::HSMS.Properties.Resources.btndangerFacility_over;
            this.btnFacilityAdmin.CheckedImage = null;
            this.btnFacilityAdmin.ClickedBackgroundImage = null;
            this.btnFacilityAdmin.ClickedImage = null;
            this.btnFacilityAdmin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFacilityAdmin.DisabledBkgndImage = global::HSMS.Properties.Resources.btndangerFacility_disable;
            this.btnFacilityAdmin.DisabledImage = null;
            this.btnFacilityAdmin.ID = -1;
            this.btnFacilityAdmin.InitButtonWidth = 60;
            this.btnFacilityAdmin.IsChecked = false;
            this.btnFacilityAdmin.Location = new System.Drawing.Point(168, 5);
            this.btnFacilityAdmin.MouseOverBkgndImage = global::HSMS.Properties.Resources.btndangerFacility_over;
            this.btnFacilityAdmin.MouseOverImage = null;
            this.btnFacilityAdmin.Name = "btnFacilityAdmin";
            this.btnFacilityAdmin.NormalImage = null;
            this.btnFacilityAdmin.Owner = null;
            this.btnFacilityAdmin.Size = new System.Drawing.Size(60, 81);
            this.btnFacilityAdmin.TabIndex = 2;
            this.btnFacilityAdmin.Text = "시설물";
            this.btnFacilityAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnFacilityAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFacilityAdmin.ToolTipText = "시설물";
            this.btnFacilityAdmin.UseCustomImageRect = false;
            this.btnFacilityAdmin.UseTextLocation = true;
            this.btnFacilityAdmin.UseVisualStyleBackColor = true;
            // 
            // btnVehicleAdmin
            // 
            this.btnVehicleAdmin.BackgroundImage = global::HSMS.Properties.Resources.btnVehicle_nomal;
            this.btnVehicleAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVehicleAdmin.CheckButton = false;
            this.btnVehicleAdmin.CheckedBkgndImage = global::HSMS.Properties.Resources.btnVehicle_over;
            this.btnVehicleAdmin.CheckedImage = null;
            this.btnVehicleAdmin.ClickedBackgroundImage = null;
            this.btnVehicleAdmin.ClickedImage = null;
            this.btnVehicleAdmin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnVehicleAdmin.DisabledBkgndImage = global::HSMS.Properties.Resources.btnVehicle_disable;
            this.btnVehicleAdmin.DisabledImage = null;
            this.btnVehicleAdmin.ID = -1;
            this.btnVehicleAdmin.InitButtonWidth = 60;
            this.btnVehicleAdmin.IsChecked = false;
            this.btnVehicleAdmin.Location = new System.Drawing.Point(91, 5);
            this.btnVehicleAdmin.MouseOverBkgndImage = global::HSMS.Properties.Resources.btnVehicle_over;
            this.btnVehicleAdmin.MouseOverImage = null;
            this.btnVehicleAdmin.Name = "btnVehicleAdmin";
            this.btnVehicleAdmin.NormalImage = null;
            this.btnVehicleAdmin.Owner = null;
            this.btnVehicleAdmin.Size = new System.Drawing.Size(60, 81);
            this.btnVehicleAdmin.TabIndex = 2;
            this.btnVehicleAdmin.Text = "차량";
            this.btnVehicleAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnVehicleAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnVehicleAdmin.ToolTipText = "차량";
            this.btnVehicleAdmin.UseCustomImageRect = false;
            this.btnVehicleAdmin.UseTextLocation = true;
            this.btnVehicleAdmin.UseVisualStyleBackColor = true;
            // 
            // btnWorkerAdmin
            // 
            this.btnWorkerAdmin.BackgroundImage = global::HSMS.Properties.Resources.btnWorker_nomal;
            this.btnWorkerAdmin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnWorkerAdmin.CheckButton = false;
            this.btnWorkerAdmin.CheckedBkgndImage = global::HSMS.Properties.Resources.btnWorker_over;
            this.btnWorkerAdmin.CheckedImage = null;
            this.btnWorkerAdmin.ClickedBackgroundImage = null;
            this.btnWorkerAdmin.ClickedImage = null;
            this.btnWorkerAdmin.CustomImageRect = new System.Drawing.Rectangle(2, 0, 55, 70);
            this.btnWorkerAdmin.DisabledBkgndImage = global::HSMS.Properties.Resources.btnWorker_disable;
            this.btnWorkerAdmin.DisabledImage = null;
            this.btnWorkerAdmin.ID = -1;
            this.btnWorkerAdmin.InitButtonWidth = 60;
            this.btnWorkerAdmin.IsChecked = false;
            this.btnWorkerAdmin.Location = new System.Drawing.Point(14, 5);
            this.btnWorkerAdmin.MouseOverBkgndImage = global::HSMS.Properties.Resources.btnWorker_over;
            this.btnWorkerAdmin.MouseOverImage = null;
            this.btnWorkerAdmin.Name = "btnWorkerAdmin";
            this.btnWorkerAdmin.NormalImage = null;
            this.btnWorkerAdmin.Owner = null;
            this.btnWorkerAdmin.Size = new System.Drawing.Size(60, 81);
            this.btnWorkerAdmin.TabIndex = 2;
            this.btnWorkerAdmin.Text = "작업자";
            this.btnWorkerAdmin.TextLocation = new System.Drawing.Point(0, 58);
            this.btnWorkerAdmin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnWorkerAdmin.ToolTipText = "작업자";
            this.btnWorkerAdmin.UseCustomImageRect = true;
            this.btnWorkerAdmin.UseTextLocation = true;
            this.btnWorkerAdmin.UseVisualStyleBackColor = true;
            // 
            // panelMonitoringTab
            // 
            this.panelMonitoringTab.Controls.Add(this.btnStatusEnd);
            this.panelMonitoringTab.Controls.Add(this.pictureBox3);
            this.panelMonitoringTab.Controls.Add(this.panelClock);
            this.panelMonitoringTab.Controls.Add(this.panelStatus);
            this.panelMonitoringTab.Controls.Add(this.panelLog);
            this.panelMonitoringTab.Location = new System.Drawing.Point(3, 43);
            this.panelMonitoringTab.Name = "panelMonitoringTab";
            this.panelMonitoringTab.Size = new System.Drawing.Size(1908, 87);
            this.panelMonitoringTab.TabIndex = 41;
            // 
            // btnStatusEnd
            // 
            this.btnStatusEnd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStatusEnd.CheckButton = false;
            this.btnStatusEnd.CheckedBkgndImage = global::HSMS.Properties.Resources.status_end_click;
            this.btnStatusEnd.CheckedImage = null;
            this.btnStatusEnd.ClickedBackgroundImage = null;
            this.btnStatusEnd.ClickedImage = null;
            this.btnStatusEnd.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnStatusEnd.DisabledBkgndImage = global::HSMS.Properties.Resources.status_end_disabled;
            this.btnStatusEnd.DisabledImage = null;
            this.btnStatusEnd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(167)))), ((int)(((byte)(164)))));
            this.btnStatusEnd.ID = -1;
            this.btnStatusEnd.InitButtonWidth = 245;
            this.btnStatusEnd.IsChecked = false;
            this.btnStatusEnd.Location = new System.Drawing.Point(1660, 11);
            this.btnStatusEnd.MouseOverBkgndImage = global::HSMS.Properties.Resources.status_end_click;
            this.btnStatusEnd.MouseOverImage = null;
            this.btnStatusEnd.Name = "btnStatusEnd";
            this.btnStatusEnd.NormalImage = null;
            this.btnStatusEnd.Owner = null;
            this.btnStatusEnd.Size = new System.Drawing.Size(245, 66);
            this.btnStatusEnd.TabIndex = 3;
            this.btnStatusEnd.Text = "상황 종료";
            this.btnStatusEnd.TextLocation = new System.Drawing.Point(10, 15);
            this.btnStatusEnd.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnStatusEnd.ToolTipText = "상황 종료";
            this.btnStatusEnd.UseCustomImageRect = false;
            this.btnStatusEnd.UseTextLocation = true;
            this.btnStatusEnd.UseVisualStyleBackColor = true;
            this.btnStatusEnd.Click += new System.EventHandler(this.btnStatusEnd_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.BackgroundImage = global::HSMS.Properties.Resources.skin_line_img;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox3.Location = new System.Drawing.Point(292, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(10, 80);
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            // 
            // panelClock
            // 
            this.panelClock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelClock.Controls.Add(this.labelTime);
            this.panelClock.Controls.Add(this.pictureBox1);
            this.panelClock.Controls.Add(this.labelDate);
            this.panelClock.Location = new System.Drawing.Point(-1, 0);
            this.panelClock.Name = "panelClock";
            this.panelClock.Size = new System.Drawing.Size(298, 87);
            this.panelClock.TabIndex = 38;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Font = new System.Drawing.Font("맑은 고딕", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTime.ForeColor = System.Drawing.Color.White;
            this.labelTime.Location = new System.Drawing.Point(99, 37);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(127, 38);
            this.labelTime.TabIndex = 1;
            this.labelTime.Text = "00:00:00";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::HSMS.Properties.Resources.img_Clock;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Location = new System.Drawing.Point(18, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(67, 79);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDate.ForeColor = System.Drawing.Color.White;
            this.labelDate.Location = new System.Drawing.Point(105, 15);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(105, 17);
            this.labelDate.TabIndex = 0;
            this.labelDate.Text = "2013년 7월 1일";
            // 
            // panelStatus
            // 
            this.panelStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelStatus.Controls.Add(this.lblDangerDetail);
            this.panelStatus.Controls.Add(this.lblDangerLevel);
            this.panelStatus.Controls.Add(this.pictureBoxbell);
            this.panelStatus.Location = new System.Drawing.Point(303, 0);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(489, 87);
            this.panelStatus.TabIndex = 0;
            // 
            // lblDangerDetail
            // 
            this.lblDangerDetail.AutoSize = true;
            this.lblDangerDetail.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDangerDetail.ForeColor = System.Drawing.Color.White;
            this.lblDangerDetail.Location = new System.Drawing.Point(100, 11);
            this.lblDangerDetail.Name = "lblDangerDetail";
            this.lblDangerDetail.Size = new System.Drawing.Size(46, 17);
            this.lblDangerDetail.TabIndex = 0;
            this.lblDangerDetail.Text = "label1";
            // 
            // lblDangerLevel
            // 
            this.lblDangerLevel.AutoSize = true;
            this.lblDangerLevel.Font = new System.Drawing.Font("맑은 고딕", 17.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDangerLevel.ForeColor = System.Drawing.Color.White;
            this.lblDangerLevel.Location = new System.Drawing.Point(96, 25);
            this.lblDangerLevel.Name = "lblDangerLevel";
            this.lblDangerLevel.Size = new System.Drawing.Size(168, 31);
            this.lblDangerLevel.TabIndex = 1;
            this.lblDangerLevel.Text = "위험 상황 없음";
            // 
            // pictureBoxbell
            // 
            this.pictureBoxbell.BackgroundImage = global::HSMS.Properties.Resources.state_img;
            this.pictureBoxbell.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxbell.Location = new System.Drawing.Point(12, 4);
            this.pictureBoxbell.Name = "pictureBoxbell";
            this.pictureBoxbell.Size = new System.Drawing.Size(78, 79);
            this.pictureBoxbell.TabIndex = 43;
            this.pictureBoxbell.TabStop = false;
            // 
            // panelLog
            // 
            this.panelLog.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLog.Controls.Add(this.pictureBox5);
            this.panelLog.Controls.Add(this.pictureBox4);
            this.panelLog.Location = new System.Drawing.Point(795, 0);
            this.panelLog.Name = "panelLog";
            this.panelLog.RealTimeInfo = null;
            this.panelLog.Size = new System.Drawing.Size(862, 87);
            this.panelLog.TabIndex = 39;
            this.panelLog.Text = "FormRealTimeInfo";
            this.panelLog.TextColor = System.Drawing.Color.White;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackgroundImage = global::HSMS.Properties.Resources.detail_info_img;
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox5.Location = new System.Drawing.Point(12, 3);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(89, 79);
            this.pictureBox5.TabIndex = 43;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.BackgroundImage = global::HSMS.Properties.Resources.skin_line_img;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox4.Location = new System.Drawing.Point(-4, 4);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(10, 80);
            this.pictureBox4.TabIndex = 0;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBoxReport
            // 
            this.pictureBoxReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pictureBoxReport.BackgroundImage = global::HSMS.Properties.Resources.Tab_off;
            this.pictureBoxReport.Location = new System.Drawing.Point(200, 0);
            this.pictureBoxReport.Name = "pictureBoxReport";
            this.pictureBoxReport.Owner = null;
            this.pictureBoxReport.PictureBoxText = "리포트";
            this.pictureBoxReport.Size = new System.Drawing.Size(100, 40);
            this.pictureBoxReport.TabIndex = 2;
            this.pictureBoxReport.TabStop = false;
            this.pictureBoxReport.Text = "리포트";
            this.pictureBoxReport.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxAdmin
            // 
            this.pictureBoxAdmin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pictureBoxAdmin.BackgroundImage = global::HSMS.Properties.Resources.Tab_off;
            this.pictureBoxAdmin.Location = new System.Drawing.Point(100, 0);
            this.pictureBoxAdmin.Name = "pictureBoxAdmin";
            this.pictureBoxAdmin.Owner = null;
            this.pictureBoxAdmin.PictureBoxText = "관리";
            this.pictureBoxAdmin.Size = new System.Drawing.Size(100, 40);
            this.pictureBoxAdmin.TabIndex = 1;
            this.pictureBoxAdmin.TabStop = false;
            this.pictureBoxAdmin.Text = "관리";
            this.pictureBoxAdmin.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxMonitoring
            // 
            this.pictureBoxMonitoring.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pictureBoxMonitoring.BackgroundImage = global::HSMS.Properties.Resources.Tab_on;
            this.pictureBoxMonitoring.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxMonitoring.Name = "pictureBoxMonitoring";
            this.pictureBoxMonitoring.Owner = null;
            this.pictureBoxMonitoring.PictureBoxText = "모니터링";
            this.pictureBoxMonitoring.Size = new System.Drawing.Size(100, 40);
            this.pictureBoxMonitoring.TabIndex = 0;
            this.pictureBoxMonitoring.TabStop = false;
            this.pictureBoxMonitoring.Text = "모니터링";
            this.pictureBoxMonitoring.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // panelBottom
            // 
            this.panelBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panelBottom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelBottom.Controls.Add(this.monthCalendar2);
            this.panelBottom.Controls.Add(this.monthCalendar1);
            this.panelBottom.Location = new System.Drawing.Point(0, 363);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1362, 609);
            this.panelBottom.TabIndex = 1;
            // 
            // monthCalendar2
            // 
            this.monthCalendar2.Location = new System.Drawing.Point(395, 57);
            this.monthCalendar2.Name = "monthCalendar2";
            this.monthCalendar2.TabIndex = 9;
            this.monthCalendar2.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar2_DateChanged);
            this.monthCalendar2.Leave += new System.EventHandler(this.monthCalendar2_Leave);
            this.monthCalendar2.MouseEnter += new System.EventHandler(this.monthCalendar2_MouseEnter);
            this.monthCalendar2.MouseLeave += new System.EventHandler(this.monthCalendar2_MouseLeave);
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(173, 57);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 0;
            this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateChanged);
            this.monthCalendar1.Enter += new System.EventHandler(this.monthCalendar1_Enter);
            this.monthCalendar1.Leave += new System.EventHandler(this.monthCalendar1_Leave);
            this.monthCalendar1.MouseCaptureChanged += new System.EventHandler(this.monthCalendar1_MouseCaptureChanged);
            this.monthCalendar1.MouseEnter += new System.EventHandler(this.monthCalendar1_MouseEnter);
            this.monthCalendar1.MouseLeave += new System.EventHandler(this.monthCalendar1_MouseLeave);
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.ForeColor = System.Drawing.Color.White;
            this.lbCount.Location = new System.Drawing.Point(1114, 15);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(69, 12);
            this.lbCount.TabIndex = 45;
            this.lbCount.Text = "현재 상항 : ";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1370, 772);
            this.Controls.Add(this.panelMiddle);
            this.Controls.Add(this.ribbonButton21);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyUp);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.panelMiddle.ResumeLayout(false);
            this.panelAdminBar.ResumeLayout(false);
            this.panelAdminBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.panelReportBar.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            this.panelReportTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            this.panelAdminTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            this.panelMonitoringTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.panelClock.ResumeLayout(false);
            this.panelClock.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxbell)).EndInit();
            this.panelLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMonitoring)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.TextPictureBox pictureBoxReport;
        private UnE.GUI.TextPictureBox pictureBoxAdmin;
        private UnE.GUI.TextPictureBox pictureBoxMonitoring;
        private RealTimeInfoPane panelLog;
        private UnE.GUI.RibbonButton btnListAdmin;
        private UnE.GUI.RibbonButton btnDetectAdmin;
        private UnE.GUI.RibbonButton btnAlarmAdmin;
        private UnE.GUI.RibbonButton btnMessageAdmin;
        private UnE.GUI.RibbonButton btnManagerAdmin;
        private UnE.GUI.RibbonButton btnDeleteAdmin;
        private UnE.GUI.RibbonButton btnSaveAdmin;
        private UnE.GUI.RibbonButton btnDangerZoneAdmin;
        private UnE.GUI.RibbonButton btnFacilityAdmin;
        private UnE.GUI.RibbonButton btnVehicleAdmin;
        private UnE.GUI.RibbonButton btnWorkerAdmin;
        private UnE.GUI.RibbonButton btnReportHistory;
        private UnE.GUI.RibbonButton btnSaveHome;
        private UnE.GUI.RibbonButton btnHome;
        private UnE.GUI.RibbonButton btnDangerZoneLayer;
        private UnE.GUI.RibbonButton btnVehicleLayer;
        private UnE.GUI.RibbonButton btnDangerFacilityLayer;
        private UnE.GUI.RibbonButton btnWorkerLayer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboAlarmList;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBoxbell;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Label lblDangerLevel;
        private System.Windows.Forms.Timer ClockTimer;
        private System.Windows.Forms.Label lblDangerDetail;
        private UnE.GUI.RibbonButton ribbonButton21;
        private UnE.GUI.RibbonButton btnZoomIn;
        private UnE.GUI.RibbonButton btnPanning;
        private UnE.GUI.RibbonButton btnFullScreen;
        private UnE.GUI.RibbonButton btnZoomOut;
        private UnE.GUI.RibbonButton btnOrbit;
        private UnE.GUI.RibbonButton btnPick;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox12;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.PictureBox pictureBox10;
        private PanelEx panelTop;
        private PanelEx panelBottom;
        private PanelEx panelStatus;
        private PanelEx panelClock;
        private PanelEx panelAdminBar;
        private PanelEx panelMonitoringTab;
        private PanelEx panelAdminTab;
        private PanelEx panelReportTab;
        private PanelEx panelLeft;
        private PanelEx panelReportBar;
        private PanelEx panelMiddle;
        private UnE.GUI.RibbonButton btnStatusEnd;
        private UnE.GUI.RibbonButton btnOption;
        private System.Windows.Forms.ComboBox cboReportLatelyDate;
        private System.Windows.Forms.ComboBox cboAlarmStep;
        private System.Windows.Forms.Button btnReportDateEnd;
        private System.Windows.Forms.Button btnReportDateStart;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.MonthCalendar monthCalendar2;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Label lbCount;
    }
}

