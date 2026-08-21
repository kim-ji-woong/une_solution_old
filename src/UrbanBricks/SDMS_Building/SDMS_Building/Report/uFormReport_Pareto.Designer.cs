namespace SDMS_Building.Report
{
    partial class uFormReport_Pareto
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSaveFile = new UnE.GUI.ImageButton();
            this.btnPageBefore = new UnE.GUI.ImageButton();
            this.lblTotalPage = new System.Windows.Forms.Label();
            this.btnPageNext = new UnE.GUI.ImageButton();
            this.lblGridViewTitle = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.eleLevel = new System.Windows.Forms.Integration.ElementHost();
            this.eleFloor = new System.Windows.Forms.Integration.ElementHost();
            this.eleBuilding = new System.Windows.Forms.Integration.ElementHost();
            this.btnDateEnd = new System.Windows.Forms.PictureBox();
            this.btnSearch = new UnE.GUI.ImageButton();
            this.label8 = new System.Windows.Forms.Label();
            this.lblDateEnd = new System.Windows.Forms.Label();
            this.btnDateStart = new System.Windows.Forms.PictureBox();
            this.lblDateStart = new System.Windows.Forms.Label();
            this.eleType = new System.Windows.Forms.Integration.ElementHost();
            this.dataGridViewSensor = new System.Windows.Forms.DataGridView();
            this.lblLevel = new System.Windows.Forms.Label();
            this.lblLoaction = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.eleReportType = new System.Windows.Forms.Integration.ElementHost();
            this.winChartViewerEquipZone = new ChartDirector.WinChartViewer();
            this.dataGridViewEquipZone = new System.Windows.Forms.DataGridView();
            this.winChartViewerSensor = new ChartDirector.WinChartViewer();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPageBefore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPageNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDateEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDateStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSensor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerEquipZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEquipZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerSensor)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.ButtonText = "";
            this.btnSaveFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveFile.ImageClicked = global::SDMS_Building.Properties.Resources.download_click;
            this.btnSaveFile.ImageDisabled = null;
            this.btnSaveFile.ImageMouseOver = global::SDMS_Building.Properties.Resources.download_hover;
            this.btnSaveFile.ImageNormal = global::SDMS_Building.Properties.Resources.download_normal;
            this.btnSaveFile.Location = new System.Drawing.Point(1197, 164);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Owner = null;
            this.btnSaveFile.Size = new System.Drawing.Size(30, 30);
            this.btnSaveFile.TabIndex = 103;
            this.btnSaveFile.TabStop = false;
            this.btnSaveFile.TextColor = System.Drawing.Color.Black;
            this.btnSaveFile.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveFile.ToolTipText = "";
            this.btnSaveFile.UseToolTip = false;
            this.btnSaveFile.WindowRateWidth = 1F;
            this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // btnPageBefore
            // 
            this.btnPageBefore.ButtonText = "";
            this.btnPageBefore.ImageClicked = global::SDMS_Building.Properties.Resources.arrowLeftReport_click;
            this.btnPageBefore.ImageDisabled = null;
            this.btnPageBefore.ImageMouseOver = global::SDMS_Building.Properties.Resources.arrowLeftReport_click;
            this.btnPageBefore.ImageNormal = global::SDMS_Building.Properties.Resources.arrowLeftReport_normal;
            this.btnPageBefore.Location = new System.Drawing.Point(1240, 164);
            this.btnPageBefore.Name = "btnPageBefore";
            this.btnPageBefore.Owner = null;
            this.btnPageBefore.Size = new System.Drawing.Size(30, 30);
            this.btnPageBefore.TabIndex = 97;
            this.btnPageBefore.TabStop = false;
            this.btnPageBefore.TextColor = System.Drawing.Color.Black;
            this.btnPageBefore.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPageBefore.ToolTipText = "";
            this.btnPageBefore.UseToolTip = false;
            this.btnPageBefore.WindowRateWidth = 1F;
            this.btnPageBefore.Click += new System.EventHandler(this.btnPageBefore_Click);
            // 
            // lblTotalPage
            // 
            this.lblTotalPage.AutoSize = true;
            this.lblTotalPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTotalPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(101)))), ((int)(((byte)(150)))));
            this.lblTotalPage.Location = new System.Drawing.Point(1333, 168);
            this.lblTotalPage.Name = "lblTotalPage";
            this.lblTotalPage.Size = new System.Drawing.Size(45, 24);
            this.lblTotalPage.TabIndex = 96;
            this.lblTotalPage.Text = "1 / 1";
            // 
            // btnPageNext
            // 
            this.btnPageNext.ButtonText = "";
            this.btnPageNext.ImageClicked = global::SDMS_Building.Properties.Resources.arrowRightReport_click;
            this.btnPageNext.ImageDisabled = null;
            this.btnPageNext.ImageMouseOver = global::SDMS_Building.Properties.Resources.arrowRightReport_click;
            this.btnPageNext.ImageNormal = global::SDMS_Building.Properties.Resources.arrowRightReport_normal;
            this.btnPageNext.Location = new System.Drawing.Point(1396, 164);
            this.btnPageNext.Name = "btnPageNext";
            this.btnPageNext.Owner = null;
            this.btnPageNext.Size = new System.Drawing.Size(30, 30);
            this.btnPageNext.TabIndex = 98;
            this.btnPageNext.TabStop = false;
            this.btnPageNext.TextColor = System.Drawing.Color.Black;
            this.btnPageNext.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPageNext.ToolTipText = "";
            this.btnPageNext.UseToolTip = false;
            this.btnPageNext.WindowRateWidth = 1F;
            this.btnPageNext.Click += new System.EventHandler(this.btnPageNext_Click);
            // 
            // lblGridViewTitle
            // 
            this.lblGridViewTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblGridViewTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblGridViewTitle.ForeColor = System.Drawing.Color.White;
            this.lblGridViewTitle.Location = new System.Drawing.Point(46, 309);
            this.lblGridViewTitle.Name = "lblGridViewTitle";
            this.lblGridViewTitle.Size = new System.Drawing.Size(1477, 48);
            this.lblGridViewTitle.TabIndex = 95;
            this.lblGridViewTitle.Text = "탐지 분석 리스트";
            this.lblGridViewTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(256, 113);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(163, 21);
            this.dateTimePicker2.TabIndex = 94;
            this.dateTimePicker2.Visible = false;
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(50, 113);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(163, 21);
            this.dateTimePicker1.TabIndex = 93;
            this.dateTimePicker1.Visible = false;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // eleLevel
            // 
            this.eleLevel.Location = new System.Drawing.Point(1336, 60);
            this.eleLevel.Name = "eleLevel";
            this.eleLevel.Size = new System.Drawing.Size(128, 50);
            this.eleLevel.TabIndex = 91;
            this.eleLevel.Text = "elementHost2";
            this.eleLevel.Visible = false;
            this.eleLevel.Child = null;
            // 
            // eleFloor
            // 
            this.eleFloor.Location = new System.Drawing.Point(1096, 60);
            this.eleFloor.Name = "eleFloor";
            this.eleFloor.Size = new System.Drawing.Size(155, 50);
            this.eleFloor.TabIndex = 90;
            this.eleFloor.Text = "elementHost2";
            this.eleFloor.Child = null;
            // 
            // eleBuilding
            // 
            this.eleBuilding.Location = new System.Drawing.Point(911, 60);
            this.eleBuilding.Name = "eleBuilding";
            this.eleBuilding.Size = new System.Drawing.Size(195, 50);
            this.eleBuilding.TabIndex = 89;
            this.eleBuilding.Text = "elementHost1";
            this.eleBuilding.Child = null;
            // 
            // btnDateEnd
            // 
            this.btnDateEnd.BackColor = System.Drawing.Color.White;
            this.btnDateEnd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDateEnd.Image = global::SDMS_Building.Properties.Resources.calendar;
            this.btnDateEnd.Location = new System.Drawing.Point(756, 69);
            this.btnDateEnd.Name = "btnDateEnd";
            this.btnDateEnd.Size = new System.Drawing.Size(30, 30);
            this.btnDateEnd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnDateEnd.TabIndex = 87;
            this.btnDateEnd.TabStop = false;
            this.btnDateEnd.Click += new System.EventHandler(this.btnDateAfter_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.ButtonText = "";
            this.btnSearch.ImageClicked = global::SDMS_Building.Properties.Resources.search_click;
            this.btnSearch.ImageDisabled = null;
            this.btnSearch.ImageMouseOver = global::SDMS_Building.Properties.Resources.search_click;
            this.btnSearch.ImageNormal = global::SDMS_Building.Properties.Resources.search_normal;
            this.btnSearch.Location = new System.Drawing.Point(1326, 113);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Owner = null;
            this.btnSearch.Size = new System.Drawing.Size(100, 50);
            this.btnSearch.TabIndex = 92;
            this.btnSearch.TabStop = false;
            this.btnSearch.TextColor = System.Drawing.Color.Black;
            this.btnSearch.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.ToolTipText = "";
            this.btnSearch.UseToolTip = false;
            this.btnSearch.WindowRateWidth = 1F;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.label8.Location = new System.Drawing.Point(605, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 50);
            this.label8.TabIndex = 88;
            this.label8.Text = "~";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDateEnd
            // 
            this.lblDateEnd.BackColor = System.Drawing.Color.White;
            this.lblDateEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDateEnd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblDateEnd.Location = new System.Drawing.Point(629, 60);
            this.lblDateEnd.Name = "lblDateEnd";
            this.lblDateEnd.Size = new System.Drawing.Size(163, 50);
            this.lblDateEnd.TabIndex = 86;
            this.lblDateEnd.Text = "2020-01-01";
            this.lblDateEnd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnDateStart
            // 
            this.btnDateStart.BackColor = System.Drawing.Color.White;
            this.btnDateStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDateStart.Image = global::SDMS_Building.Properties.Resources.calendar;
            this.btnDateStart.Location = new System.Drawing.Point(564, 69);
            this.btnDateStart.Name = "btnDateStart";
            this.btnDateStart.Size = new System.Drawing.Size(30, 30);
            this.btnDateStart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnDateStart.TabIndex = 85;
            this.btnDateStart.TabStop = false;
            this.btnDateStart.Click += new System.EventHandler(this.btnDateBefore_Click);
            // 
            // lblDateStart
            // 
            this.lblDateStart.BackColor = System.Drawing.Color.White;
            this.lblDateStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDateStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblDateStart.Location = new System.Drawing.Point(437, 60);
            this.lblDateStart.Name = "lblDateStart";
            this.lblDateStart.Size = new System.Drawing.Size(163, 50);
            this.lblDateStart.TabIndex = 84;
            this.lblDateStart.Text = "2020-01-01";
            this.lblDateStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // eleType
            // 
            this.eleType.Location = new System.Drawing.Point(150, 60);
            this.eleType.Name = "eleType";
            this.eleType.Size = new System.Drawing.Size(160, 50);
            this.eleType.TabIndex = 83;
            this.eleType.Text = "elementHost1";
            this.eleType.Child = null;
            // 
            // dataGridViewSensor
            // 
            this.dataGridViewSensor.AllowUserToAddRows = false;
            this.dataGridViewSensor.AllowUserToDeleteRows = false;
            this.dataGridViewSensor.AllowUserToResizeRows = false;
            this.dataGridViewSensor.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewSensor.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewSensor.ColumnHeadersHeight = 50;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewSensor.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewSensor.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.dataGridViewSensor.Location = new System.Drawing.Point(46, 357);
            this.dataGridViewSensor.MultiSelect = false;
            this.dataGridViewSensor.Name = "dataGridViewSensor";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewSensor.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewSensor.RowHeadersVisible = false;
            this.dataGridViewSensor.RowTemplate.Height = 30;
            this.dataGridViewSensor.RowTemplate.ReadOnly = true;
            this.dataGridViewSensor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSensor.Size = new System.Drawing.Size(1477, 189);
            this.dataGridViewSensor.TabIndex = 82;
            // 
            // lblLevel
            // 
            this.lblLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLevel.ForeColor = System.Drawing.Color.White;
            this.lblLevel.Location = new System.Drawing.Point(1236, 61);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(75, 48);
            this.lblLevel.TabIndex = 81;
            this.lblLevel.Text = "단계";
            this.lblLevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLevel.Visible = false;
            // 
            // lblLoaction
            // 
            this.lblLoaction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblLoaction.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLoaction.ForeColor = System.Drawing.Color.White;
            this.lblLoaction.Location = new System.Drawing.Point(812, 61);
            this.lblLoaction.Name = "lblLoaction";
            this.lblLoaction.Size = new System.Drawing.Size(75, 48);
            this.lblLoaction.TabIndex = 80;
            this.lblLoaction.Text = "위치";
            this.lblLoaction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblType
            // 
            this.lblType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblType.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblType.ForeColor = System.Drawing.Color.White;
            this.lblType.Location = new System.Drawing.Point(50, 61);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(75, 48);
            this.lblType.TabIndex = 79;
            this.lblType.Text = "유형";
            this.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDate
            // 
            this.lblDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDate.ForeColor = System.Drawing.Color.White;
            this.lblDate.Location = new System.Drawing.Point(326, 61);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(75, 48);
            this.lblDate.TabIndex = 78;
            this.lblDate.Text = "기간";
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(101)))), ((int)(((byte)(150)))));
            this.label1.Location = new System.Drawing.Point(50, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(290, 24);
            this.label1.TabIndex = 77;
            this.label1.Text = "작동 빈도가 높은 센서들부터 표시합니다.";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(50, 30);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1476, 1);
            this.flowLayoutPanel1.TabIndex = 76;
            // 
            // eleReportType
            // 
            this.eleReportType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.eleReportType.Location = new System.Drawing.Point(1031, 164);
            this.eleReportType.Name = "eleReportType";
            this.eleReportType.Size = new System.Drawing.Size(160, 30);
            this.eleReportType.TabIndex = 105;
            this.eleReportType.Text = "elementHost1";
            this.eleReportType.Child = null;
            // 
            // winChartViewerEquipZone
            // 
            this.winChartViewerEquipZone.Location = new System.Drawing.Point(50, 200);
            this.winChartViewerEquipZone.Name = "winChartViewerEquipZone";
            this.winChartViewerEquipZone.Size = new System.Drawing.Size(1477, 87);
            this.winChartViewerEquipZone.TabIndex = 106;
            this.winChartViewerEquipZone.TabStop = false;
            this.winChartViewerEquipZone.Visible = false;
            // 
            // dataGridViewEquipZone
            // 
            this.dataGridViewEquipZone.AllowUserToAddRows = false;
            this.dataGridViewEquipZone.AllowUserToDeleteRows = false;
            this.dataGridViewEquipZone.AllowUserToResizeRows = false;
            this.dataGridViewEquipZone.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewEquipZone.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewEquipZone.ColumnHeadersHeight = 50;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewEquipZone.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewEquipZone.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.dataGridViewEquipZone.Location = new System.Drawing.Point(46, 357);
            this.dataGridViewEquipZone.MultiSelect = false;
            this.dataGridViewEquipZone.Name = "dataGridViewEquipZone";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewEquipZone.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewEquipZone.RowHeadersVisible = false;
            this.dataGridViewEquipZone.RowTemplate.Height = 30;
            this.dataGridViewEquipZone.RowTemplate.ReadOnly = true;
            this.dataGridViewEquipZone.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewEquipZone.Size = new System.Drawing.Size(1477, 189);
            this.dataGridViewEquipZone.TabIndex = 107;
            // 
            // winChartViewerSensor
            // 
            this.winChartViewerSensor.Location = new System.Drawing.Point(50, 200);
            this.winChartViewerSensor.Name = "winChartViewerSensor";
            this.winChartViewerSensor.Size = new System.Drawing.Size(1477, 87);
            this.winChartViewerSensor.TabIndex = 104;
            this.winChartViewerSensor.TabStop = false;
            this.winChartViewerSensor.Visible = false;
            // 
            // uFormReport_Pareto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(231)))), ((int)(((byte)(243)))));
            this.Controls.Add(this.dataGridViewSensor);
            this.Controls.Add(this.dataGridViewEquipZone);
            this.Controls.Add(this.winChartViewerSensor);
            this.Controls.Add(this.winChartViewerEquipZone);
            this.Controls.Add(this.eleReportType);
            this.Controls.Add(this.btnSaveFile);
            this.Controls.Add(this.btnPageBefore);
            this.Controls.Add(this.lblTotalPage);
            this.Controls.Add(this.btnPageNext);
            this.Controls.Add(this.lblGridViewTitle);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.eleLevel);
            this.Controls.Add(this.eleFloor);
            this.Controls.Add(this.eleBuilding);
            this.Controls.Add(this.btnDateEnd);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblDateEnd);
            this.Controls.Add(this.btnDateStart);
            this.Controls.Add(this.lblDateStart);
            this.Controls.Add(this.eleType);
            this.Controls.Add(this.lblLevel);
            this.Controls.Add(this.lblLoaction);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "uFormReport_Pareto";
            this.Size = new System.Drawing.Size(1576, 576);
            this.Load += new System.EventHandler(this.uFormReport_Pareto_Load);
            this.Resize += new System.EventHandler(this.uFormReport_Pareto_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPageBefore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPageNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDateEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDateStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSensor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerEquipZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEquipZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerSensor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UnE.GUI.ImageButton btnSaveFile;
        private UnE.GUI.ImageButton btnPageBefore;
        private System.Windows.Forms.Label lblTotalPage;
        private UnE.GUI.ImageButton btnPageNext;
        private System.Windows.Forms.Label lblGridViewTitle;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Integration.ElementHost eleLevel;
        private System.Windows.Forms.Integration.ElementHost eleFloor;
        private System.Windows.Forms.Integration.ElementHost eleBuilding;
        private System.Windows.Forms.PictureBox btnDateEnd;
        private UnE.GUI.ImageButton btnSearch;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblDateEnd;
        private System.Windows.Forms.PictureBox btnDateStart;
        private System.Windows.Forms.Label lblDateStart;
        private System.Windows.Forms.Integration.ElementHost eleType;
        private System.Windows.Forms.DataGridView dataGridViewSensor;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.Label lblLoaction;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Integration.ElementHost eleReportType;
        private ChartDirector.WinChartViewer winChartViewerEquipZone;
        private System.Windows.Forms.DataGridView dataGridViewEquipZone;
        private ChartDirector.WinChartViewer winChartViewerSensor;
    }
}
