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
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.tabControl = new UnE.SOP.Sections.SectionTabControl();
            this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
            this.tabLogs = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnEditExternalMembers = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelComponentContentsTitle = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnBroadRunner = new System.Windows.Forms.Button();
            this.btnOpenSOP = new System.Windows.Forms.Button();
            this.btnSendSMS = new System.Windows.Forms.Button();
            this.lblSOPHeader = new System.Windows.Forms.Label();
            this.btnPollution = new System.Windows.Forms.Button();
            this.btnFire = new System.Windows.Forms.Button();
            this.btnTerror = new System.Windows.Forms.Button();
            this.panelScenarioName = new System.Windows.Forms.Panel();
            this.cmbScenario = new System.Windows.Forms.ComboBox();
            this.labelScenarioName = new System.Windows.Forms.Label();
            this.btnHeavySnow = new System.Windows.Forms.Button();
            this.btnSecurity = new System.Windows.Forms.Button();
            this.btnEarthquake = new System.Windows.Forms.Button();
            this.btnSubmergence = new System.Windows.Forms.Button();
            this.btnTyphoon = new System.Windows.Forms.Button();
            this.panelBackImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).BeginInit();
            this.splitContainerVertical.Panel2.SuspendLayout();
            this.splitContainerVertical.SuspendLayout();
            this.tabLogs.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelScenarioName.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerBackgroundImage
            // 
            this.timerBackgroundImage.Interval = 500;
            this.timerBackgroundImage.Tick += new System.EventHandler(this.timerBackGroundImage_Tick);
            // 
            // timerSelectMission
            // 
            this.timerSelectMission.Interval = 500;
            this.timerSelectMission.Tick += new System.EventHandler(this.timerSelectMission_Tick);
            // 
            // panelBackImage
            // 
            this.panelBackImage.BackColor = System.Drawing.Color.Transparent;
            this.panelBackImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelBackImage.backImgHeight = 0;
            this.panelBackImage.backImgWidth = 0;
            this.panelBackImage.Controls.Add(this.splitContainerMain);
            this.panelBackImage.Controls.Add(this.panelTop);
            this.panelBackImage.Location = new System.Drawing.Point(0, 0);
            this.panelBackImage.Name = "panelBackImage";
            this.panelBackImage.Size = new System.Drawing.Size(1093, 525);
            this.panelBackImage.TabIndex = 1;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.Location = new System.Drawing.Point(0, 32);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.tabControl);
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerVertical);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.AutoScroll = true;
            this.splitContainerMain.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.splitContainerMain.Panel2.Controls.Add(this.btnEditExternalMembers);
            this.splitContainerMain.Panel2.Controls.Add(this.labelTitle);
            this.splitContainerMain.Panel2.Controls.Add(this.labelComponentContentsTitle);
            this.splitContainerMain.Panel2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnComponentContentsDoubleClick);
            this.splitContainerMain.Panel2.Resize += new System.EventHandler(this.splitContainerMain_Panel2_Resize);
            this.splitContainerMain.Panel2MinSize = 0;
            this.splitContainerMain.Size = new System.Drawing.Size(1093, 563);
            this.splitContainerMain.SplitterDistance = 832;
            this.splitContainerMain.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl.CloseBtnImage = null;
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("맑은 고딕", 15.75F, System.Drawing.FontStyle.Bold);
            this.tabControl.ItemSize = new System.Drawing.Size(72, 35);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.SelectedTabColor = System.Drawing.Color.DarkGray;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(832, 563);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.tabControl.TabDisabledForeColor = System.Drawing.Color.DarkGray;
            this.tabControl.TabForeColor = System.Drawing.Color.White;
            this.tabControl.TabIndex = 1;
            this.tabControl.UseCloseButton = false;
            this.tabControl.Visible = false;
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
            this.splitContainerVertical.Size = new System.Drawing.Size(832, 563);
            this.splitContainerVertical.SplitterDistance = 438;
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
            this.tabLogs.Size = new System.Drawing.Size(832, 121);
            this.tabLogs.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(824, 95);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "SOP 로그";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(824, 95);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "메시지 로그";
            this.tabPage3.Click += new System.EventHandler(this.tabPage3_Click);
            // 
            // btnEditExternalMembers
            // 
            this.btnEditExternalMembers.Location = new System.Drawing.Point(68, 6);
            this.btnEditExternalMembers.Name = "btnEditExternalMembers";
            this.btnEditExternalMembers.Size = new System.Drawing.Size(140, 23);
            this.btnEditExternalMembers.TabIndex = 2;
            this.btnEditExternalMembers.Text = "담당자 및 연락처 변경";
            this.btnEditExternalMembers.UseVisualStyleBackColor = true;
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
            this.labelTitle.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnComponentContentsDoubleClick);
            // 
            // labelComponentContentsTitle
            // 
            this.labelComponentContentsTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.labelComponentContentsTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelComponentContentsTitle.Font = new System.Drawing.Font("맑은 고딕", 15.25F, System.Drawing.FontStyle.Bold);
            this.labelComponentContentsTitle.ForeColor = System.Drawing.Color.White;
            this.labelComponentContentsTitle.Location = new System.Drawing.Point(0, 0);
            this.labelComponentContentsTitle.Name = "labelComponentContentsTitle";
            this.labelComponentContentsTitle.Size = new System.Drawing.Size(257, 33);
            this.labelComponentContentsTitle.TabIndex = 0;
            this.labelComponentContentsTitle.Text = "임무목록 실행";
            this.labelComponentContentsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelComponentContentsTitle.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnComponentContentsDoubleClick);
            // 
            // panelTop
            // 
            this.panelTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panelTop.Controls.Add(this.btnBroadRunner);
            this.panelTop.Controls.Add(this.btnOpenSOP);
            this.panelTop.Controls.Add(this.btnSendSMS);
            this.panelTop.Controls.Add(this.lblSOPHeader);
            this.panelTop.Controls.Add(this.btnPollution);
            this.panelTop.Controls.Add(this.btnFire);
            this.panelTop.Controls.Add(this.btnTerror);
            this.panelTop.Controls.Add(this.panelScenarioName);
            this.panelTop.Controls.Add(this.btnHeavySnow);
            this.panelTop.Controls.Add(this.btnSecurity);
            this.panelTop.Controls.Add(this.btnEarthquake);
            this.panelTop.Controls.Add(this.btnSubmergence);
            this.panelTop.Controls.Add(this.btnTyphoon);
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1093, 32);
            this.panelTop.TabIndex = 0;
            // 
            // btnBroadRunner
            // 
            this.btnBroadRunner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBroadRunner.Location = new System.Drawing.Point(291, 1);
            this.btnBroadRunner.Name = "btnBroadRunner";
            this.btnBroadRunner.Size = new System.Drawing.Size(119, 30);
            this.btnBroadRunner.TabIndex = 12;
            this.btnBroadRunner.Text = "방송 테스트";
            this.btnBroadRunner.UseVisualStyleBackColor = true;
            this.btnBroadRunner.Click += new System.EventHandler(this.btnBroadRunner_Click);
            // 
            // btnOpenSOP
            // 
            this.btnOpenSOP.Image = global::SOPMonitoringSystem.Properties.Resources.Open_SOP_Normal_small;
            this.btnOpenSOP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenSOP.Location = new System.Drawing.Point(3, 1);
            this.btnOpenSOP.Name = "btnOpenSOP";
            this.btnOpenSOP.Size = new System.Drawing.Size(115, 30);
            this.btnOpenSOP.TabIndex = 11;
            this.btnOpenSOP.Text = "SOP불러오기 ";
            this.btnOpenSOP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpenSOP.UseVisualStyleBackColor = true;
            // 
            // btnSendSMS
            // 
            this.btnSendSMS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendSMS.Location = new System.Drawing.Point(416, 1);
            this.btnSendSMS.Name = "btnSendSMS";
            this.btnSendSMS.Size = new System.Drawing.Size(117, 30);
            this.btnSendSMS.TabIndex = 11;
            this.btnSendSMS.Text = "SMS 전송 테스트";
            this.btnSendSMS.UseVisualStyleBackColor = true;
            this.btnSendSMS.Visible = false;
            this.btnSendSMS.Click += new System.EventHandler(this.btnSendSMS_Click);
            // 
            // lblSOPHeader
            // 
            this.lblSOPHeader.AutoSize = true;
            this.lblSOPHeader.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSOPHeader.Location = new System.Drawing.Point(150, 10);
            this.lblSOPHeader.Name = "lblSOPHeader";
            this.lblSOPHeader.Size = new System.Drawing.Size(100, 12);
            this.lblSOPHeader.TabIndex = 10;
            this.lblSOPHeader.Text = "실행중인 SOP :";
            // 
            // btnPollution
            // 
            this.btnPollution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPollution.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.btnPollution.Location = new System.Drawing.Point(1027, 1);
            this.btnPollution.Name = "btnPollution";
            this.btnPollution.Size = new System.Drawing.Size(64, 30);
            this.btnPollution.TabIndex = 9;
            this.btnPollution.Text = "오염";
            this.btnPollution.UseVisualStyleBackColor = true;
            // 
            // btnFire
            // 
            this.btnFire.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFire.Location = new System.Drawing.Point(579, 1);
            this.btnFire.Name = "btnFire";
            this.btnFire.Size = new System.Drawing.Size(64, 30);
            this.btnFire.TabIndex = 2;
            this.btnFire.Text = "화재";
            this.btnFire.UseVisualStyleBackColor = true;
            // 
            // btnTerror
            // 
            this.btnTerror.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTerror.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.btnTerror.Location = new System.Drawing.Point(963, 1);
            this.btnTerror.Name = "btnTerror";
            this.btnTerror.Size = new System.Drawing.Size(64, 30);
            this.btnTerror.TabIndex = 8;
            this.btnTerror.Text = "테러";
            this.btnTerror.UseVisualStyleBackColor = true;
            // 
            // panelScenarioName
            // 
            this.panelScenarioName.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Background;
            this.panelScenarioName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelScenarioName.Controls.Add(this.cmbScenario);
            this.panelScenarioName.Controls.Add(this.labelScenarioName);
            this.panelScenarioName.Location = new System.Drawing.Point(280, 2);
            this.panelScenarioName.Name = "panelScenarioName";
            this.panelScenarioName.Size = new System.Drawing.Size(400, 29);
            this.panelScenarioName.TabIndex = 1;
            // 
            // cmbScenario
            // 
            this.cmbScenario.BackColor = System.Drawing.Color.White;
            this.cmbScenario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbScenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScenario.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbScenario.ForeColor = System.Drawing.Color.DarkGray;
            this.cmbScenario.FormattingEnabled = true;
            this.cmbScenario.Location = new System.Drawing.Point(0, 0);
            this.cmbScenario.Margin = new System.Windows.Forms.Padding(0);
            this.cmbScenario.Name = "cmbScenario";
            this.cmbScenario.Size = new System.Drawing.Size(400, 28);
            this.cmbScenario.TabIndex = 1;
            this.cmbScenario.SelectedIndexChanged += new System.EventHandler(this.cmbScenario_SelectedIndexChanged);
            // 
            // labelScenarioName
            // 
            this.labelScenarioName.AutoSize = true;
            this.labelScenarioName.BackColor = System.Drawing.Color.Transparent;
            this.labelScenarioName.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelScenarioName.Location = new System.Drawing.Point(8, 5);
            this.labelScenarioName.Name = "labelScenarioName";
            this.labelScenarioName.Size = new System.Drawing.Size(95, 15);
            this.labelScenarioName.TabIndex = 0;
            this.labelScenarioName.Text = "Scenario Name";
            // 
            // btnHeavySnow
            // 
            this.btnHeavySnow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHeavySnow.Location = new System.Drawing.Point(899, 1);
            this.btnHeavySnow.Name = "btnHeavySnow";
            this.btnHeavySnow.Size = new System.Drawing.Size(64, 30);
            this.btnHeavySnow.TabIndex = 7;
            this.btnHeavySnow.Text = "폭설";
            this.btnHeavySnow.UseVisualStyleBackColor = true;
            // 
            // btnSecurity
            // 
            this.btnSecurity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSecurity.Location = new System.Drawing.Point(835, 1);
            this.btnSecurity.Name = "btnSecurity";
            this.btnSecurity.Size = new System.Drawing.Size(64, 30);
            this.btnSecurity.TabIndex = 6;
            this.btnSecurity.Text = "방범";
            this.btnSecurity.UseVisualStyleBackColor = true;
            // 
            // btnEarthquake
            // 
            this.btnEarthquake.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEarthquake.Location = new System.Drawing.Point(643, 1);
            this.btnEarthquake.Name = "btnEarthquake";
            this.btnEarthquake.Size = new System.Drawing.Size(64, 30);
            this.btnEarthquake.TabIndex = 3;
            this.btnEarthquake.Text = "지진";
            this.btnEarthquake.UseVisualStyleBackColor = true;
            // 
            // btnSubmergence
            // 
            this.btnSubmergence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmergence.Location = new System.Drawing.Point(771, 1);
            this.btnSubmergence.Name = "btnSubmergence";
            this.btnSubmergence.Size = new System.Drawing.Size(64, 30);
            this.btnSubmergence.TabIndex = 5;
            this.btnSubmergence.Text = "침수";
            this.btnSubmergence.UseVisualStyleBackColor = true;
            // 
            // btnTyphoon
            // 
            this.btnTyphoon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTyphoon.Location = new System.Drawing.Point(707, 1);
            this.btnTyphoon.Name = "btnTyphoon";
            this.btnTyphoon.Size = new System.Drawing.Size(64, 30);
            this.btnTyphoon.TabIndex = 4;
            this.btnTyphoon.Text = "태풍";
            this.btnTyphoon.UseVisualStyleBackColor = true;
            // 
            // PageBackstageSOP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1093, 595);
            this.Controls.Add(this.panelBackImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageSOP";
            this.Text = "PageBackstageHome";
            this.Load += new System.EventHandler(this.PageBackstageHome_Load);
            this.Shown += new System.EventHandler(this.PageBackstageSOP_Shown);
            this.Resize += new System.EventHandler(this.PageBackstageHome_Resize);
            this.panelBackImage.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerVertical.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).EndInit();
            this.splitContainerVertical.ResumeLayout(false);
            this.tabLogs.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelScenarioName.ResumeLayout(false);
            this.panelScenarioName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelScenarioName;
        private System.Windows.Forms.Label labelScenarioName;
        private PanelSOP panelBackImage;
        public  UnE.SOP.Sections.SectionTabPage tabPage1;
		public UnE.SOP.Sections.SectionTabControl tabControl;
        private System.Windows.Forms.SplitContainer splitContainerVertical;
        private System.Windows.Forms.TabControl tabLogs;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ComboBox cmbScenario;
        private System.Windows.Forms.Timer timerBackgroundImage;
        private System.Windows.Forms.Label labelComponentContentsTitle;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button btnPollution;
        private System.Windows.Forms.Button btnFire;
        private System.Windows.Forms.Button btnTerror;
        private System.Windows.Forms.Button btnHeavySnow;
        private System.Windows.Forms.Button btnSecurity;
        private System.Windows.Forms.Button btnEarthquake;
        private System.Windows.Forms.Button btnSubmergence;
        private System.Windows.Forms.Button btnTyphoon;
        private System.Windows.Forms.Button btnEditExternalMembers;
        private System.Windows.Forms.Label lblSOPHeader;
        private System.Windows.Forms.Button btnOpenSOP;
        private System.Windows.Forms.Button btnSendSMS;
        private System.Windows.Forms.Button btnBroadRunner;
        private System.Windows.Forms.Timer timerSelectMission;
    }
}