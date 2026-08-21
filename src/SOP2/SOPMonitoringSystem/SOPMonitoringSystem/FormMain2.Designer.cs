namespace SOPMonitoringSystem
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.btnMin = new System.Windows.Forms.Button();
            this.btnMax = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelViewRibbonBarMiddle = new System.Windows.Forms.Panel();
            this.panelRealTimeInfo = new UnE.Utility.RealTimeInfoPane();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelMode = new System.Windows.Forms.Label();
            this.panelNormalMode = new System.Windows.Forms.Panel();
            this.radioHoliday = new System.Windows.Forms.RadioButton();
            this.labelHoliday = new System.Windows.Forms.Label();
            this.radioNormal = new System.Windows.Forms.RadioButton();
            this.labelNormal = new System.Windows.Forms.Label();
            this.panelRegistMode = new System.Windows.Forms.Panel();
            this.radioNonRegistMode = new System.Windows.Forms.RadioButton();
            this.labelNonRegular = new System.Windows.Forms.Label();
            this.labelRegular = new System.Windows.Forms.Label();
            this.radioRegistMode = new System.Windows.Forms.RadioButton();
            this.panelRealMode = new System.Windows.Forms.Panel();
            this.labelVirtual = new System.Windows.Forms.Label();
            this.labelReal = new System.Windows.Forms.Label();
            this.radioVirtualMode = new System.Windows.Forms.RadioButton();
            this.radioRealMode = new System.Windows.Forms.RadioButton();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnReturnControl = new UnE.GUI.RibbonButton();
            this.btnCancelSOP = new UnE.GUI.RibbonButton();
            this.btnRepeatBroadcast = new UnE.GUI.RibbonButton();
            this.btnStopBroadcast = new UnE.GUI.RibbonButton();
            this.btnPauseBroadcast = new UnE.GUI.RibbonButton();
            this.btnFitToScale = new UnE.GUI.RibbonButton();
            this.btnFitToCurrentComponent = new UnE.GUI.RibbonButton();
            this.btnStartBroadcast = new UnE.GUI.RibbonButton();
            this.btnStartSOP = new UnE.GUI.RibbonButton();
            this.btnControl = new UnE.GUI.RibbonButton();
            this.panelViewRibbonBarRight = new System.Windows.Forms.Panel();
            this.panelViewRibbonBarLeft = new System.Windows.Forms.Panel();
            this.pictureBoxView = new UnE.GUI.TextPictureBox();
            this.pictureBoxMessage = new UnE.GUI.TextPictureBox();
            this.pictureBoxOpt = new UnE.GUI.TextPictureBox();
            this.pictureBoxMainIcon = new System.Windows.Forms.PictureBox();
            this.panelMain = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelTop.SuspendLayout();
            this.panelViewRibbonBarMiddle.SuspendLayout();
            this.panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
            this.panelNormalMode.SuspendLayout();
            this.panelRegistMode.SuspendLayout();
            this.panelRealMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMessage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOpt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMainIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.ToolbarBkgnd;
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
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(30, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(89, 15);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "SOP Simulator";
            // 
            // btnMin
            // 
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
            this.panelViewRibbonBarMiddle.Controls.Add(this.panelRealTimeInfo);
            this.panelViewRibbonBarMiddle.Controls.Add(this.panelStatus);
            this.panelViewRibbonBarMiddle.Controls.Add(this.panelNormalMode);
            this.panelViewRibbonBarMiddle.Controls.Add(this.panelRegistMode);
            this.panelViewRibbonBarMiddle.Controls.Add(this.panelRealMode);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox6);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox7);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox5);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox4);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox3);
            this.panelViewRibbonBarMiddle.Controls.Add(this.pictureBox2);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnReturnControl);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnCancelSOP);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnRepeatBroadcast);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnStopBroadcast);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnPauseBroadcast);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnFitToScale);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnFitToCurrentComponent);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnStartBroadcast);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnStartSOP);
            this.panelViewRibbonBarMiddle.Controls.Add(this.btnControl);
            this.panelViewRibbonBarMiddle.Location = new System.Drawing.Point(142, 67);
            this.panelViewRibbonBarMiddle.Name = "panelViewRibbonBarMiddle";
            this.panelViewRibbonBarMiddle.Size = new System.Drawing.Size(1518, 87);
            this.panelViewRibbonBarMiddle.TabIndex = 0;
            // 
            // panelRealTimeInfo
            // 
            this.panelRealTimeInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelRealTimeInfo.DisplayBeginPosition = new System.Drawing.Point(30, 27);
            this.panelRealTimeInfo.Location = new System.Drawing.Point(1238, 3);
            this.panelRealTimeInfo.Name = "panelRealTimeInfo";
            this.panelRealTimeInfo.RealTimeInfo = null;
            this.panelRealTimeInfo.Size = new System.Drawing.Size(200, 83);
            this.panelRealTimeInfo.TabIndex = 4;
            this.panelRealTimeInfo.Text = "FormRealTimeInfo";
            this.panelRealTimeInfo.TextColor = System.Drawing.Color.White;
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.pictureBoxStatus);
            this.panelStatus.Controls.Add(this.labelStatus);
            this.panelStatus.Controls.Add(this.labelMode);
            this.panelStatus.Location = new System.Drawing.Point(1001, 3);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(227, 83);
            this.panelStatus.TabIndex = 3;
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
            // panelNormalMode
            // 
            this.panelNormalMode.Controls.Add(this.radioHoliday);
            this.panelNormalMode.Controls.Add(this.labelHoliday);
            this.panelNormalMode.Controls.Add(this.radioNormal);
            this.panelNormalMode.Controls.Add(this.labelNormal);
            this.panelNormalMode.Location = new System.Drawing.Point(482, 1);
            this.panelNormalMode.Name = "panelNormalMode";
            this.panelNormalMode.Size = new System.Drawing.Size(108, 85);
            this.panelNormalMode.TabIndex = 2;
            // 
            // radioHoliday
            // 
            this.radioHoliday.AutoSize = true;
            this.radioHoliday.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.radioHoliday.ForeColor = System.Drawing.Color.White;
            this.radioHoliday.Location = new System.Drawing.Point(10, 47);
            this.radioHoliday.Name = "radioHoliday";
            this.radioHoliday.Size = new System.Drawing.Size(14, 13);
            this.radioHoliday.TabIndex = 0;
            this.radioHoliday.UseVisualStyleBackColor = true;
            this.radioHoliday.CheckedChanged += new System.EventHandler(this.radioNormalMode_CheckedChanged);
            // 
            // labelHoliday
            // 
            this.labelHoliday.AutoSize = true;
            this.labelHoliday.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelHoliday.ForeColor = System.Drawing.Color.White;
            this.labelHoliday.Location = new System.Drawing.Point(26, 45);
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
            this.radioNormal.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioNormal.ForeColor = System.Drawing.Color.White;
            this.radioNormal.Location = new System.Drawing.Point(10, 16);
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
            this.labelNormal.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNormal.ForeColor = System.Drawing.Color.White;
            this.labelNormal.Location = new System.Drawing.Point(26, 15);
            this.labelNormal.Name = "labelNormal";
            this.labelNormal.Size = new System.Drawing.Size(31, 15);
            this.labelNormal.TabIndex = 1;
            this.labelNormal.Text = "평일";
            this.labelNormal.Click += new System.EventHandler(this.labelRadio_Click);
            // 
            // panelRegistMode
            // 
            this.panelRegistMode.Controls.Add(this.radioNonRegistMode);
            this.panelRegistMode.Controls.Add(this.labelNonRegular);
            this.panelRegistMode.Controls.Add(this.labelRegular);
            this.panelRegistMode.Controls.Add(this.radioRegistMode);
            this.panelRegistMode.Location = new System.Drawing.Point(383, 1);
            this.panelRegistMode.Name = "panelRegistMode";
            this.panelRegistMode.Size = new System.Drawing.Size(99, 85);
            this.panelRegistMode.TabIndex = 2;
            // 
            // radioNonRegistMode
            // 
            this.radioNonRegistMode.AutoSize = true;
            this.radioNonRegistMode.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.radioNonRegistMode.ForeColor = System.Drawing.Color.White;
            this.radioNonRegistMode.Location = new System.Drawing.Point(10, 47);
            this.radioNonRegistMode.Name = "radioNonRegistMode";
            this.radioNonRegistMode.Size = new System.Drawing.Size(14, 13);
            this.radioNonRegistMode.TabIndex = 0;
            this.radioNonRegistMode.UseVisualStyleBackColor = true;
            this.radioNonRegistMode.CheckedChanged += new System.EventHandler(this.radioRegistMode_CheckedChanged);
            // 
            // labelNonRegular
            // 
            this.labelNonRegular.AutoSize = true;
            this.labelNonRegular.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNonRegular.ForeColor = System.Drawing.Color.White;
            this.labelNonRegular.Location = new System.Drawing.Point(26, 45);
            this.labelNonRegular.Name = "labelNonRegular";
            this.labelNonRegular.Size = new System.Drawing.Size(67, 15);
            this.labelNonRegular.TabIndex = 1;
            this.labelNonRegular.Text = "미등록모드";
            this.labelNonRegular.Click += new System.EventHandler(this.labelRadio_Click);
            // 
            // labelRegular
            // 
            this.labelRegular.AutoSize = true;
            this.labelRegular.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelRegular.ForeColor = System.Drawing.Color.White;
            this.labelRegular.Location = new System.Drawing.Point(26, 15);
            this.labelRegular.Name = "labelRegular";
            this.labelRegular.Size = new System.Drawing.Size(55, 15);
            this.labelRegular.TabIndex = 1;
            this.labelRegular.Text = "등록모드";
            this.labelRegular.Click += new System.EventHandler(this.labelRadio_Click);
            // 
            // radioRegistMode
            // 
            this.radioRegistMode.AutoSize = true;
            this.radioRegistMode.Checked = true;
            this.radioRegistMode.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioRegistMode.ForeColor = System.Drawing.Color.White;
            this.radioRegistMode.Location = new System.Drawing.Point(10, 16);
            this.radioRegistMode.Name = "radioRegistMode";
            this.radioRegistMode.Size = new System.Drawing.Size(14, 13);
            this.radioRegistMode.TabIndex = 0;
            this.radioRegistMode.TabStop = true;
            this.radioRegistMode.UseVisualStyleBackColor = true;
            this.radioRegistMode.CheckedChanged += new System.EventHandler(this.radioRegistMode_CheckedChanged);
            // 
            // panelRealMode
            // 
            this.panelRealMode.Controls.Add(this.labelVirtual);
            this.panelRealMode.Controls.Add(this.labelReal);
            this.panelRealMode.Controls.Add(this.radioVirtualMode);
            this.panelRealMode.Controls.Add(this.radioRealMode);
            this.panelRealMode.Location = new System.Drawing.Point(290, 1);
            this.panelRealMode.Name = "panelRealMode";
            this.panelRealMode.Size = new System.Drawing.Size(86, 85);
            this.panelRealMode.TabIndex = 2;
            // 
            // labelVirtual
            // 
            this.labelVirtual.AutoSize = true;
            this.labelVirtual.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelVirtual.ForeColor = System.Drawing.Color.White;
            this.labelVirtual.Location = new System.Drawing.Point(26, 45);
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
            this.labelReal.Location = new System.Drawing.Point(26, 15);
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
            this.radioVirtualMode.Location = new System.Drawing.Point(10, 47);
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
            this.radioRealMode.Location = new System.Drawing.Point(10, 16);
            this.radioRealMode.Name = "radioRealMode";
            this.radioRealMode.Size = new System.Drawing.Size(14, 13);
            this.radioRealMode.TabIndex = 0;
            this.radioRealMode.UseVisualStyleBackColor = true;
            this.radioRealMode.CheckedChanged += new System.EventHandler(this.radioRealMode_CheckedChanged);
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox6.Location = new System.Drawing.Point(990, 3);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(13, 85);
            this.pictureBox6.TabIndex = 1;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox7.Location = new System.Drawing.Point(1226, 3);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(13, 85);
            this.pictureBox7.TabIndex = 1;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox5.Location = new System.Drawing.Point(854, 3);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(13, 85);
            this.pictureBox5.TabIndex = 1;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox4.Location = new System.Drawing.Point(589, 3);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(13, 85);
            this.pictureBox4.TabIndex = 1;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox3.Location = new System.Drawing.Point(372, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(13, 85);
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Separator;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(146, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(13, 85);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // btnReturnControl
            // 
            this.btnReturnControl.CheckedBkgndImage = null;
            this.btnReturnControl.CheckedImage = null;
            this.btnReturnControl.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnReturnControl.DisabledBkgndImage = null;
            this.btnReturnControl.DisabledImage = null;
            this.btnReturnControl.ID = -1;
            this.btnReturnControl.InitButtonWidth = 60;
            this.btnReturnControl.IsChecked = false;
            this.btnReturnControl.Location = new System.Drawing.Point(67, 1);
            this.btnReturnControl.MouseOverBkgndImage = null;
            this.btnReturnControl.Name = "btnReturnControl";
            this.btnReturnControl.NormalImage = null;
            this.btnReturnControl.Owner = null;
            this.btnReturnControl.Size = new System.Drawing.Size(73, 85);
            this.btnReturnControl.TabIndex = 0;
            this.btnReturnControl.Text = "제어권 반납";
            this.btnReturnControl.TextLocation = new System.Drawing.Point(0, 0);
            this.btnReturnControl.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnReturnControl.UseCustomImageRect = false;
            this.btnReturnControl.UseTextLocation = false;
            this.btnReturnControl.UseVisualStyleBackColor = true;
            // 
            // btnCancelSOP
            // 
            this.btnCancelSOP.CheckedBkgndImage = null;
            this.btnCancelSOP.CheckedImage = null;
            this.btnCancelSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnCancelSOP.DisabledBkgndImage = null;
            this.btnCancelSOP.DisabledImage = null;
            this.btnCancelSOP.ID = -1;
            this.btnCancelSOP.InitButtonWidth = 60;
            this.btnCancelSOP.IsChecked = false;
            this.btnCancelSOP.Location = new System.Drawing.Point(226, 1);
            this.btnCancelSOP.MouseOverBkgndImage = null;
            this.btnCancelSOP.Name = "btnCancelSOP";
            this.btnCancelSOP.NormalImage = null;
            this.btnCancelSOP.Owner = null;
            this.btnCancelSOP.Size = new System.Drawing.Size(60, 85);
            this.btnCancelSOP.TabIndex = 0;
            this.btnCancelSOP.Text = "실행취소";
            this.btnCancelSOP.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancelSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancelSOP.UseCustomImageRect = false;
            this.btnCancelSOP.UseTextLocation = false;
            this.btnCancelSOP.UseVisualStyleBackColor = true;
            // 
            // btnRepeatBroadcast
            // 
            this.btnRepeatBroadcast.CheckedBkgndImage = null;
            this.btnRepeatBroadcast.CheckedImage = null;
            this.btnRepeatBroadcast.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnRepeatBroadcast.DisabledBkgndImage = null;
            this.btnRepeatBroadcast.DisabledImage = null;
            this.btnRepeatBroadcast.ID = -1;
            this.btnRepeatBroadcast.InitButtonWidth = 60;
            this.btnRepeatBroadcast.IsChecked = false;
            this.btnRepeatBroadcast.Location = new System.Drawing.Point(796, 1);
            this.btnRepeatBroadcast.MouseOverBkgndImage = null;
            this.btnRepeatBroadcast.Name = "btnRepeatBroadcast";
            this.btnRepeatBroadcast.NormalImage = null;
            this.btnRepeatBroadcast.Owner = null;
            this.btnRepeatBroadcast.Size = new System.Drawing.Size(60, 85);
            this.btnRepeatBroadcast.TabIndex = 0;
            this.btnRepeatBroadcast.Text = "-";
            this.btnRepeatBroadcast.TextLocation = new System.Drawing.Point(0, 0);
            this.btnRepeatBroadcast.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnRepeatBroadcast.UseCustomImageRect = false;
            this.btnRepeatBroadcast.UseTextLocation = false;
            this.btnRepeatBroadcast.UseVisualStyleBackColor = true;
            // 
            // btnStopBroadcast
            // 
            this.btnStopBroadcast.CheckedBkgndImage = null;
            this.btnStopBroadcast.CheckedImage = null;
            this.btnStopBroadcast.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnStopBroadcast.DisabledBkgndImage = null;
            this.btnStopBroadcast.DisabledImage = null;
            this.btnStopBroadcast.ID = -1;
            this.btnStopBroadcast.InitButtonWidth = 60;
            this.btnStopBroadcast.IsChecked = false;
            this.btnStopBroadcast.Location = new System.Drawing.Point(730, 1);
            this.btnStopBroadcast.MouseOverBkgndImage = null;
            this.btnStopBroadcast.Name = "btnStopBroadcast";
            this.btnStopBroadcast.NormalImage = null;
            this.btnStopBroadcast.Owner = null;
            this.btnStopBroadcast.Size = new System.Drawing.Size(60, 85);
            this.btnStopBroadcast.TabIndex = 0;
            this.btnStopBroadcast.Text = "정지";
            this.btnStopBroadcast.TextLocation = new System.Drawing.Point(0, 0);
            this.btnStopBroadcast.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnStopBroadcast.UseCustomImageRect = false;
            this.btnStopBroadcast.UseTextLocation = false;
            this.btnStopBroadcast.UseVisualStyleBackColor = true;
            // 
            // btnPauseBroadcast
            // 
            this.btnPauseBroadcast.CheckedBkgndImage = null;
            this.btnPauseBroadcast.CheckedImage = null;
            this.btnPauseBroadcast.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnPauseBroadcast.DisabledBkgndImage = null;
            this.btnPauseBroadcast.DisabledImage = null;
            this.btnPauseBroadcast.ID = -1;
            this.btnPauseBroadcast.InitButtonWidth = 60;
            this.btnPauseBroadcast.IsChecked = false;
            this.btnPauseBroadcast.Location = new System.Drawing.Point(664, 1);
            this.btnPauseBroadcast.MouseOverBkgndImage = null;
            this.btnPauseBroadcast.Name = "btnPauseBroadcast";
            this.btnPauseBroadcast.NormalImage = null;
            this.btnPauseBroadcast.Owner = null;
            this.btnPauseBroadcast.Size = new System.Drawing.Size(60, 85);
            this.btnPauseBroadcast.TabIndex = 0;
            this.btnPauseBroadcast.Text = "일시정지";
            this.btnPauseBroadcast.TextLocation = new System.Drawing.Point(0, 0);
            this.btnPauseBroadcast.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnPauseBroadcast.UseCustomImageRect = false;
            this.btnPauseBroadcast.UseTextLocation = false;
            this.btnPauseBroadcast.UseVisualStyleBackColor = true;
            // 
            // btnFitToScale
            // 
            this.btnFitToScale.CheckedBkgndImage = null;
            this.btnFitToScale.CheckedImage = null;
            this.btnFitToScale.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFitToScale.DisabledBkgndImage = null;
            this.btnFitToScale.DisabledImage = null;
            this.btnFitToScale.ID = -1;
            this.btnFitToScale.InitButtonWidth = 60;
            this.btnFitToScale.IsChecked = false;
            this.btnFitToScale.Location = new System.Drawing.Point(928, 1);
            this.btnFitToScale.MouseOverBkgndImage = null;
            this.btnFitToScale.Name = "btnFitToScale";
            this.btnFitToScale.NormalImage = null;
            this.btnFitToScale.Owner = null;
            this.btnFitToScale.Size = new System.Drawing.Size(60, 85);
            this.btnFitToScale.TabIndex = 0;
            this.btnFitToScale.Text = "전체확대";
            this.btnFitToScale.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFitToScale.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFitToScale.UseCustomImageRect = false;
            this.btnFitToScale.UseTextLocation = false;
            this.btnFitToScale.UseVisualStyleBackColor = true;
            // 
            // btnFitToCurrentComponent
            // 
            this.btnFitToCurrentComponent.CheckedBkgndImage = null;
            this.btnFitToCurrentComponent.CheckedImage = null;
            this.btnFitToCurrentComponent.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFitToCurrentComponent.DisabledBkgndImage = null;
            this.btnFitToCurrentComponent.DisabledImage = null;
            this.btnFitToCurrentComponent.ID = -1;
            this.btnFitToCurrentComponent.InitButtonWidth = 60;
            this.btnFitToCurrentComponent.IsChecked = false;
            this.btnFitToCurrentComponent.Location = new System.Drawing.Point(862, 1);
            this.btnFitToCurrentComponent.MouseOverBkgndImage = null;
            this.btnFitToCurrentComponent.Name = "btnFitToCurrentComponent";
            this.btnFitToCurrentComponent.NormalImage = null;
            this.btnFitToCurrentComponent.Owner = null;
            this.btnFitToCurrentComponent.Size = new System.Drawing.Size(60, 85);
            this.btnFitToCurrentComponent.TabIndex = 0;
            this.btnFitToCurrentComponent.Text = "부분확대";
            this.btnFitToCurrentComponent.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFitToCurrentComponent.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFitToCurrentComponent.UseCustomImageRect = false;
            this.btnFitToCurrentComponent.UseTextLocation = false;
            this.btnFitToCurrentComponent.UseVisualStyleBackColor = true;
            // 
            // btnStartBroadcast
            // 
            this.btnStartBroadcast.CheckedBkgndImage = null;
            this.btnStartBroadcast.CheckedImage = null;
            this.btnStartBroadcast.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnStartBroadcast.DisabledBkgndImage = null;
            this.btnStartBroadcast.DisabledImage = null;
            this.btnStartBroadcast.ID = -1;
            this.btnStartBroadcast.InitButtonWidth = 60;
            this.btnStartBroadcast.IsChecked = false;
            this.btnStartBroadcast.Location = new System.Drawing.Point(598, 1);
            this.btnStartBroadcast.MouseOverBkgndImage = null;
            this.btnStartBroadcast.Name = "btnStartBroadcast";
            this.btnStartBroadcast.NormalImage = null;
            this.btnStartBroadcast.Owner = null;
            this.btnStartBroadcast.Size = new System.Drawing.Size(60, 85);
            this.btnStartBroadcast.TabIndex = 0;
            this.btnStartBroadcast.Text = "시작";
            this.btnStartBroadcast.TextLocation = new System.Drawing.Point(0, 0);
            this.btnStartBroadcast.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnStartBroadcast.UseCustomImageRect = false;
            this.btnStartBroadcast.UseTextLocation = false;
            this.btnStartBroadcast.UseVisualStyleBackColor = true;
            // 
            // btnStartSOP
            // 
            this.btnStartSOP.CheckedBkgndImage = null;
            this.btnStartSOP.CheckedImage = null;
            this.btnStartSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnStartSOP.DisabledBkgndImage = null;
            this.btnStartSOP.DisabledImage = null;
            this.btnStartSOP.ID = -1;
            this.btnStartSOP.InitButtonWidth = 60;
            this.btnStartSOP.IsChecked = false;
            this.btnStartSOP.Location = new System.Drawing.Point(160, 1);
            this.btnStartSOP.MouseOverBkgndImage = null;
            this.btnStartSOP.Name = "btnStartSOP";
            this.btnStartSOP.NormalImage = null;
            this.btnStartSOP.Owner = null;
            this.btnStartSOP.Size = new System.Drawing.Size(60, 85);
            this.btnStartSOP.TabIndex = 0;
            this.btnStartSOP.Text = "시작";
            this.btnStartSOP.TextLocation = new System.Drawing.Point(0, 0);
            this.btnStartSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnStartSOP.UseCustomImageRect = false;
            this.btnStartSOP.UseTextLocation = false;
            this.btnStartSOP.UseVisualStyleBackColor = true;
            // 
            // btnControl
            // 
            this.btnControl.CheckedBkgndImage = null;
            this.btnControl.CheckedImage = null;
            this.btnControl.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnControl.DisabledBkgndImage = null;
            this.btnControl.DisabledImage = null;
            this.btnControl.ID = -1;
            this.btnControl.InitButtonWidth = 60;
            this.btnControl.IsChecked = false;
            this.btnControl.Location = new System.Drawing.Point(1, 1);
            this.btnControl.MouseOverBkgndImage = null;
            this.btnControl.Name = "btnControl";
            this.btnControl.NormalImage = null;
            this.btnControl.Owner = null;
            this.btnControl.Size = new System.Drawing.Size(60, 85);
            this.btnControl.TabIndex = 0;
            this.btnControl.Text = "제어";
            this.btnControl.TextLocation = new System.Drawing.Point(0, 0);
            this.btnControl.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnControl.UseCustomImageRect = false;
            this.btnControl.UseTextLocation = false;
            this.btnControl.UseVisualStyleBackColor = true;
            // 
            // panelViewRibbonBarRight
            // 
            this.panelViewRibbonBarRight.BackColor = System.Drawing.Color.Transparent;
            this.panelViewRibbonBarRight.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.RibbonBar_Right;
            this.panelViewRibbonBarRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelViewRibbonBarRight.Location = new System.Drawing.Point(1666, 67);
            this.panelViewRibbonBarRight.Name = "panelViewRibbonBarRight";
            this.panelViewRibbonBarRight.Size = new System.Drawing.Size(254, 87);
            this.panelViewRibbonBarRight.TabIndex = 1;
            // 
            // panelViewRibbonBarLeft
            // 
            this.panelViewRibbonBarLeft.BackColor = System.Drawing.Color.Transparent;
            this.panelViewRibbonBarLeft.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.RibbonBar_Left;
            this.panelViewRibbonBarLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelViewRibbonBarLeft.Location = new System.Drawing.Point(2, 67);
            this.panelViewRibbonBarLeft.Name = "panelViewRibbonBarLeft";
            this.panelViewRibbonBarLeft.Size = new System.Drawing.Size(134, 87);
            this.panelViewRibbonBarLeft.TabIndex = 2;
            // 
            // pictureBoxView
            // 
            this.pictureBoxView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pictureBoxView.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            this.pictureBoxView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxView.Location = new System.Drawing.Point(199, 29);
            this.pictureBoxView.Name = "pictureBoxView";
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
            this.pictureBoxMessage.Location = new System.Drawing.Point(100, 29);
            this.pictureBoxMessage.Name = "pictureBoxMessage";
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
            this.pictureBoxOpt.Size = new System.Drawing.Size(98, 35);
            this.pictureBoxOpt.TabIndex = 1;
            this.pictureBoxOpt.TabStop = false;
            this.pictureBoxOpt.Text = "옵션";
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
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.panelMain.Location = new System.Drawing.Point(93, 218);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(464, 263);
            this.panelMain.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 719);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "SOP Monitoring System";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelViewRibbonBarMiddle.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
            this.panelNormalMode.ResumeLayout(false);
            this.panelNormalMode.PerformLayout();
            this.panelRegistMode.ResumeLayout(false);
            this.panelRegistMode.PerformLayout();
            this.panelRealMode.ResumeLayout(false);
            this.panelRealMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMessage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOpt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMainIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private UnE.GUI.TextPictureBox pictureBoxOpt;
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
        private System.Windows.Forms.Panel panelRegistMode;
        private System.Windows.Forms.RadioButton radioNonRegistMode;
        private System.Windows.Forms.RadioButton radioRegistMode;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private UnE.GUI.RibbonButton btnRepeatBroadcast;
        private UnE.GUI.RibbonButton btnStopBroadcast;
        private UnE.GUI.RibbonButton btnPauseBroadcast;
        private UnE.GUI.RibbonButton btnStartBroadcast;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox7;
        private UnE.GUI.RibbonButton btnFitToScale;
        private UnE.GUI.RibbonButton btnFitToCurrentComponent;
        private System.Windows.Forms.Button btnMin;
        private System.Windows.Forms.Button btnMax;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label labelHoliday;
        private System.Windows.Forms.Label labelNormal;
        private System.Windows.Forms.Label labelNonRegular;
        private System.Windows.Forms.Label labelRegular;
        private System.Windows.Forms.Label labelVirtual;
        private System.Windows.Forms.Label labelReal;
        private System.Windows.Forms.Panel panelStatus;
        private UnE.Utility.RealTimeInfoPane panelRealTimeInfo;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.PictureBox pictureBoxStatus;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Timer timer1;
        private UnE.GUI.TextPictureBox pictureBoxMessage;
    }
}