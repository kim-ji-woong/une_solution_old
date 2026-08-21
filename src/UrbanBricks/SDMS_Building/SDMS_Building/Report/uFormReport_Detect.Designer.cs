namespace SDMS_Building.Report
{
    partial class uFormReport_Detect
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.lblLoaction = new System.Windows.Forms.Label();
            this.lblLevel = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.eleType = new System.Windows.Forms.Integration.ElementHost();
            this.lblDateStart = new System.Windows.Forms.Label();
            this.lblDateEnd = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.eleFloor = new System.Windows.Forms.Integration.ElementHost();
            this.eleBuilding = new System.Windows.Forms.Integration.ElementHost();
            this.eleLevel = new System.Windows.Forms.Integration.ElementHost();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.panelChart = new System.Windows.Forms.Panel();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.lblGridViewTitle = new System.Windows.Forms.Label();
            this.lblTotalPage = new System.Windows.Forms.Label();
            this.eleUnit = new System.Windows.Forms.Integration.ElementHost();
            this.lblUnit = new System.Windows.Forms.Label();
            this.lblUnitDetail = new System.Windows.Forms.Label();
            this.pnUnitDetail = new System.Windows.Forms.Panel();
            this.btnUnitDetailDown = new UnE.GUI.ImageButton();
            this.btnUnitDetailUp = new UnE.GUI.ImageButton();
            this.txtUnitDetail = new System.Windows.Forms.TextBox();
            this.btnSaveFile = new UnE.GUI.ImageButton();
            this.btnPageBefore = new UnE.GUI.ImageButton();
            this.btnPageNext = new UnE.GUI.ImageButton();
            this.btnSearch = new UnE.GUI.ImageButton();
            this.btnDateEnd = new System.Windows.Forms.PictureBox();
            this.btnDateStart = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.pnUnitDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnUnitDetailDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUnitDetailUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPageBefore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPageNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDateEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDateStart)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(50, 30);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1329, 1);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(101)))), ((int)(((byte)(150)))));
            this.label1.Location = new System.Drawing.Point(50, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(792, 22);
            this.label1.TabIndex = 2;
            this.label1.Text = "각 센서들이 탐지한 재난빈도를 표시합니다. 센서 오류 및 특정 상황에 의한 오작동을 포함한 빈도 입니다.";
            // 
            // lblDate
            // 
            this.lblDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblDate.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDate.ForeColor = System.Drawing.Color.White;
            this.lblDate.Location = new System.Drawing.Point(326, 61);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(75, 48);
            this.lblDate.TabIndex = 3;
            this.lblDate.Text = "기간";
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblType
            // 
            this.lblType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblType.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblType.ForeColor = System.Drawing.Color.White;
            this.lblType.Location = new System.Drawing.Point(50, 61);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(75, 48);
            this.lblType.TabIndex = 5;
            this.lblType.Text = "유형";
            this.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLoaction
            // 
            this.lblLoaction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblLoaction.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLoaction.ForeColor = System.Drawing.Color.White;
            this.lblLoaction.Location = new System.Drawing.Point(812, 61);
            this.lblLoaction.Name = "lblLoaction";
            this.lblLoaction.Size = new System.Drawing.Size(75, 48);
            this.lblLoaction.TabIndex = 6;
            this.lblLoaction.Text = "위치";
            this.lblLoaction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLevel
            // 
            this.lblLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblLevel.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLevel.ForeColor = System.Drawing.Color.White;
            this.lblLevel.Location = new System.Drawing.Point(1236, 61);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(75, 48);
            this.lblLevel.TabIndex = 7;
            this.lblLevel.Text = "단계";
            this.lblLevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLevel.Visible = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeight = 50;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.dataGridView1.Location = new System.Drawing.Point(46, 357);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.RowTemplate.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1330, 189);
            this.dataGridView1.TabIndex = 47;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellMouseEnter);
            this.dataGridView1.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellMouseLeave);
            // 
            // eleType
            // 
            this.eleType.Location = new System.Drawing.Point(150, 60);
            this.eleType.Name = "eleType";
            this.eleType.Size = new System.Drawing.Size(150, 50);
            this.eleType.TabIndex = 48;
            this.eleType.Text = "elementHost1";
            this.eleType.Child = null;
            // 
            // lblDateStart
            // 
            this.lblDateStart.BackColor = System.Drawing.Color.White;
            this.lblDateStart.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDateStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblDateStart.Location = new System.Drawing.Point(437, 60);
            this.lblDateStart.Name = "lblDateStart";
            this.lblDateStart.Size = new System.Drawing.Size(163, 50);
            this.lblDateStart.TabIndex = 49;
            this.lblDateStart.Text = "2020-01-01";
            this.lblDateStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDateEnd
            // 
            this.lblDateEnd.BackColor = System.Drawing.Color.White;
            this.lblDateEnd.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDateEnd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblDateEnd.Location = new System.Drawing.Point(629, 60);
            this.lblDateEnd.Name = "lblDateEnd";
            this.lblDateEnd.Size = new System.Drawing.Size(163, 50);
            this.lblDateEnd.TabIndex = 51;
            this.lblDateEnd.Text = "2020-01-01";
            this.lblDateEnd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.label8.Location = new System.Drawing.Point(605, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 50);
            this.label8.TabIndex = 53;
            this.label8.Text = "~";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // eleFloor
            // 
            this.eleFloor.Location = new System.Drawing.Point(1096, 60);
            this.eleFloor.Name = "eleFloor";
            this.eleFloor.Size = new System.Drawing.Size(130, 50);
            this.eleFloor.TabIndex = 56;
            this.eleFloor.Text = "elementHost2";
            this.eleFloor.Child = null;
            // 
            // eleBuilding
            // 
            this.eleBuilding.Location = new System.Drawing.Point(911, 60);
            this.eleBuilding.Name = "eleBuilding";
            this.eleBuilding.Size = new System.Drawing.Size(160, 50);
            this.eleBuilding.TabIndex = 55;
            this.eleBuilding.Text = "elementHost1";
            this.eleBuilding.Child = null;
            // 
            // eleLevel
            // 
            this.eleLevel.Location = new System.Drawing.Point(1336, 60);
            this.eleLevel.Name = "eleLevel";
            this.eleLevel.Size = new System.Drawing.Size(128, 50);
            this.eleLevel.TabIndex = 57;
            this.eleLevel.Text = "elementHost2";
            this.eleLevel.Visible = false;
            this.eleLevel.Child = null;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(50, 113);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(163, 21);
            this.dateTimePicker1.TabIndex = 59;
            this.dateTimePicker1.Visible = false;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // panelChart
            // 
            this.panelChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelChart.BackColor = System.Drawing.Color.White;
            this.panelChart.Location = new System.Drawing.Point(50, 200);
            this.panelChart.Name = "panelChart";
            this.panelChart.Size = new System.Drawing.Size(1329, 87);
            this.panelChart.TabIndex = 60;
            this.panelChart.Paint += new System.Windows.Forms.PaintEventHandler(this.panelChart_Paint);
            this.panelChart.Resize += new System.EventHandler(this.panelChart_Resize);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(256, 113);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(163, 21);
            this.dateTimePicker2.TabIndex = 61;
            this.dateTimePicker2.Visible = false;
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // lblGridViewTitle
            // 
            this.lblGridViewTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGridViewTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblGridViewTitle.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblGridViewTitle.ForeColor = System.Drawing.Color.White;
            this.lblGridViewTitle.Location = new System.Drawing.Point(46, 309);
            this.lblGridViewTitle.Name = "lblGridViewTitle";
            this.lblGridViewTitle.Size = new System.Drawing.Size(1330, 48);
            this.lblGridViewTitle.TabIndex = 65;
            this.lblGridViewTitle.Text = "탐지 이력 리스트";
            this.lblGridViewTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalPage
            // 
            this.lblTotalPage.AutoSize = true;
            this.lblTotalPage.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTotalPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(101)))), ((int)(((byte)(150)))));
            this.lblTotalPage.Location = new System.Drawing.Point(1286, 168);
            this.lblTotalPage.Name = "lblTotalPage";
            this.lblTotalPage.Size = new System.Drawing.Size(48, 22);
            this.lblTotalPage.TabIndex = 66;
            this.lblTotalPage.Text = "1 / 1";
            // 
            // eleUnit
            // 
            this.eleUnit.Location = new System.Drawing.Point(911, 140);
            this.eleUnit.Name = "eleUnit";
            this.eleUnit.Size = new System.Drawing.Size(120, 50);
            this.eleUnit.TabIndex = 70;
            this.eleUnit.Text = "elementHost1";
            this.eleUnit.Child = null;
            // 
            // lblUnit
            // 
            this.lblUnit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.lblUnit.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblUnit.ForeColor = System.Drawing.Color.White;
            this.lblUnit.Location = new System.Drawing.Point(836, 141);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(75, 48);
            this.lblUnit.TabIndex = 69;
            this.lblUnit.Text = "단위";
            this.lblUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUnitDetail
            // 
            this.lblUnitDetail.AutoSize = true;
            this.lblUnitDetail.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblUnitDetail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(101)))), ((int)(((byte)(150)))));
            this.lblUnitDetail.Location = new System.Drawing.Point(1119, 154);
            this.lblUnitDetail.Name = "lblUnitDetail";
            this.lblUnitDetail.Size = new System.Drawing.Size(65, 22);
            this.lblUnitDetail.TabIndex = 73;
            this.lblUnitDetail.Text = "일 마다";
            // 
            // pnUnitDetail
            // 
            this.pnUnitDetail.BackColor = System.Drawing.Color.White;
            this.pnUnitDetail.Controls.Add(this.btnUnitDetailDown);
            this.pnUnitDetail.Controls.Add(this.btnUnitDetailUp);
            this.pnUnitDetail.Controls.Add(this.txtUnitDetail);
            this.pnUnitDetail.Location = new System.Drawing.Point(1033, 141);
            this.pnUnitDetail.Name = "pnUnitDetail";
            this.pnUnitDetail.Size = new System.Drawing.Size(72, 48);
            this.pnUnitDetail.TabIndex = 74;
            // 
            // btnUnitDetailDown
            // 
            this.btnUnitDetailDown.ButtonText = "";
            this.btnUnitDetailDown.ImageClicked = global::SDMS_Building.Properties.Resources.down_click;
            this.btnUnitDetailDown.ImageDisabled = null;
            this.btnUnitDetailDown.ImageMouseOver = global::SDMS_Building.Properties.Resources.down_click;
            this.btnUnitDetailDown.ImageNormal = global::SDMS_Building.Properties.Resources.down_normal;
            this.btnUnitDetailDown.Location = new System.Drawing.Point(46, 27);
            this.btnUnitDetailDown.Name = "btnUnitDetailDown";
            this.btnUnitDetailDown.Owner = null;
            this.btnUnitDetailDown.Size = new System.Drawing.Size(20, 12);
            this.btnUnitDetailDown.TabIndex = 76;
            this.btnUnitDetailDown.TabStop = false;
            this.btnUnitDetailDown.TextColor = System.Drawing.Color.Black;
            this.btnUnitDetailDown.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnUnitDetailDown.ToolTipText = "";
            this.btnUnitDetailDown.UseToolTip = false;
            this.btnUnitDetailDown.WindowRateWidth = 1F;
            this.btnUnitDetailDown.Click += new System.EventHandler(this.btnUnitDetailDown_Click);
            // 
            // btnUnitDetailUp
            // 
            this.btnUnitDetailUp.ButtonText = "";
            this.btnUnitDetailUp.ImageClicked = global::SDMS_Building.Properties.Resources.up_click;
            this.btnUnitDetailUp.ImageDisabled = null;
            this.btnUnitDetailUp.ImageMouseOver = global::SDMS_Building.Properties.Resources.up_click;
            this.btnUnitDetailUp.ImageNormal = global::SDMS_Building.Properties.Resources.up_normal;
            this.btnUnitDetailUp.Location = new System.Drawing.Point(46, 9);
            this.btnUnitDetailUp.Name = "btnUnitDetailUp";
            this.btnUnitDetailUp.Owner = null;
            this.btnUnitDetailUp.Size = new System.Drawing.Size(20, 12);
            this.btnUnitDetailUp.TabIndex = 75;
            this.btnUnitDetailUp.TabStop = false;
            this.btnUnitDetailUp.TextColor = System.Drawing.Color.Black;
            this.btnUnitDetailUp.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnUnitDetailUp.ToolTipText = "";
            this.btnUnitDetailUp.UseToolTip = false;
            this.btnUnitDetailUp.WindowRateWidth = 1F;
            this.btnUnitDetailUp.Click += new System.EventHandler(this.btnUnitDetailUp_Click);
            // 
            // txtUnitDetail
            // 
            this.txtUnitDetail.BackColor = System.Drawing.Color.White;
            this.txtUnitDetail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUnitDetail.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtUnitDetail.ForeColor = System.Drawing.Color.Black;
            this.txtUnitDetail.Location = new System.Drawing.Point(3, 12);
            this.txtUnitDetail.Name = "txtUnitDetail";
            this.txtUnitDetail.Size = new System.Drawing.Size(47, 22);
            this.txtUnitDetail.TabIndex = 4;
            this.txtUnitDetail.Text = "0";
            this.txtUnitDetail.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtUnitDetail.TextChanged += new System.EventHandler(this.txtUnitDetail_TextChanged);
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
            this.btnSaveFile.TabIndex = 75;
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
            this.btnPageBefore.TabIndex = 67;
            this.btnPageBefore.TabStop = false;
            this.btnPageBefore.TextColor = System.Drawing.Color.Black;
            this.btnPageBefore.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPageBefore.ToolTipText = "";
            this.btnPageBefore.UseToolTip = false;
            this.btnPageBefore.WindowRateWidth = 1F;
            this.btnPageBefore.Click += new System.EventHandler(this.btnPageBefore_Click);
            // 
            // btnPageNext
            // 
            this.btnPageNext.ButtonText = "";
            this.btnPageNext.ImageClicked = global::SDMS_Building.Properties.Resources.arrowRightReport_click;
            this.btnPageNext.ImageDisabled = null;
            this.btnPageNext.ImageMouseOver = global::SDMS_Building.Properties.Resources.arrowRightReport_click;
            this.btnPageNext.ImageNormal = global::SDMS_Building.Properties.Resources.arrowRightReport_normal;
            this.btnPageNext.Location = new System.Drawing.Point(1349, 164);
            this.btnPageNext.Name = "btnPageNext";
            this.btnPageNext.Owner = null;
            this.btnPageNext.Size = new System.Drawing.Size(30, 30);
            this.btnPageNext.TabIndex = 68;
            this.btnPageNext.TabStop = false;
            this.btnPageNext.TextColor = System.Drawing.Color.Black;
            this.btnPageNext.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPageNext.ToolTipText = "";
            this.btnPageNext.UseToolTip = false;
            this.btnPageNext.WindowRateWidth = 1F;
            this.btnPageNext.Click += new System.EventHandler(this.btnPageNext_Click);
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
            this.btnSearch.TabIndex = 58;
            this.btnSearch.TabStop = false;
            this.btnSearch.TextColor = System.Drawing.Color.Black;
            this.btnSearch.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.ToolTipText = "";
            this.btnSearch.UseToolTip = false;
            this.btnSearch.WindowRateWidth = 1F;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
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
            this.btnDateEnd.TabIndex = 52;
            this.btnDateEnd.TabStop = false;
            this.btnDateEnd.Click += new System.EventHandler(this.btnDateAfter_Click);
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
            this.btnDateStart.TabIndex = 50;
            this.btnDateStart.TabStop = false;
            this.btnDateStart.Click += new System.EventHandler(this.btnDateBefore_Click);
            // 
            // uFormReport_Detect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(231)))), ((int)(((byte)(243)))));
            this.Controls.Add(this.btnSaveFile);
            this.Controls.Add(this.pnUnitDetail);
            this.Controls.Add(this.lblUnitDetail);
            this.Controls.Add(this.eleUnit);
            this.Controls.Add(this.lblUnit);
            this.Controls.Add(this.btnPageBefore);
            this.Controls.Add(this.lblTotalPage);
            this.Controls.Add(this.btnPageNext);
            this.Controls.Add(this.lblGridViewTitle);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.panelChart);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.eleLevel);
            this.Controls.Add(this.eleFloor);
            this.Controls.Add(this.eleBuilding);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnDateEnd);
            this.Controls.Add(this.lblDateEnd);
            this.Controls.Add(this.btnDateStart);
            this.Controls.Add(this.lblDateStart);
            this.Controls.Add(this.eleType);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lblLevel);
            this.Controls.Add(this.lblLoaction);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "uFormReport_Detect";
            this.Size = new System.Drawing.Size(1429, 576);
            this.Load += new System.EventHandler(this.uFormReport_Detect_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.uFormReport_Detect_Paint);
            this.Resize += new System.EventHandler(this.uFormReport_Detect_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.pnUnitDetail.ResumeLayout(false);
            this.pnUnitDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnUnitDetailDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUnitDetailUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPageBefore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPageNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDateEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDateStart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblLoaction;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Integration.ElementHost eleType;
        private System.Windows.Forms.Label lblDateStart;
        private System.Windows.Forms.PictureBox btnDateStart;
        private System.Windows.Forms.PictureBox btnDateEnd;
        private System.Windows.Forms.Label lblDateEnd;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Integration.ElementHost eleFloor;
        private System.Windows.Forms.Integration.ElementHost eleBuilding;
        private System.Windows.Forms.Integration.ElementHost eleLevel;
        private UnE.GUI.ImageButton btnSearch;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Panel panelChart;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label lblGridViewTitle;
        private UnE.GUI.ImageButton btnPageBefore;
        private System.Windows.Forms.Label lblTotalPage;
        private UnE.GUI.ImageButton btnPageNext;
        private System.Windows.Forms.Integration.ElementHost eleUnit;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.Label lblUnitDetail;
        private System.Windows.Forms.Panel pnUnitDetail;
        private System.Windows.Forms.TextBox txtUnitDetail;
        private UnE.GUI.ImageButton btnUnitDetailDown;
        private UnE.GUI.ImageButton btnUnitDetailUp;
        private UnE.GUI.ImageButton btnSaveFile;
    }
}
