namespace SDMS
{
    partial class NotOperationPSMPage
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.winChartViewer1 = new ChartDirector.WinChartViewer();
            this.gvMain = new System.Windows.Forms.DataGridView();
            this.noDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.materialDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.locationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.detectDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reportDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.systemRecoveryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fieldRecoveryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notOperationPSMPageGridDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cboChart = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblMinDate = new System.Windows.Forms.Label();
            this.lblMaxDate = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblBuilding = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveHWP = new UnE.GUI.ImageButton();
            this.panelChart = new System.Windows.Forms.Panel();
            this.cboPageIndex = new System.Windows.Forms.ComboBox();
            this.lblTotalPage = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnNextIndex = new UnE.GUI.ImageButton();
            this.btnPreviousIndex = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.notOperationPSMPageGridDataBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveHWP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(38, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(170, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "누출 처리 이력";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(39, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(548, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "누출 센서로 탐지된 모든 신호의 처리 결과를 백분률로 표시합니다.";
            this.label3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // winChartViewer1
            // 
            this.winChartViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.winChartViewer1.Location = new System.Drawing.Point(152, 101);
            this.winChartViewer1.Name = "winChartViewer1";
            this.winChartViewer1.Size = new System.Drawing.Size(1657, 688);
            this.winChartViewer1.TabIndex = 9;
            this.winChartViewer1.TabStop = false;
            // 
            // gvMain
            // 
            this.gvMain.AllowUserToAddRows = false;
            this.gvMain.AllowUserToDeleteRows = false;
            this.gvMain.AllowUserToResizeRows = false;
            this.gvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gvMain.AutoGenerateColumns = false;
            this.gvMain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvMain.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvMain.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvMain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.noDataGridViewTextBoxColumn,
            this.materialDataGridViewTextBoxColumn,
            this.buildingDataGridViewTextBoxColumn,
            this.locationDataGridViewTextBoxColumn,
            this.detectDataGridViewTextBoxColumn,
            this.reportDataGridViewTextBoxColumn,
            this.systemRecoveryDataGridViewTextBoxColumn,
            this.fieldRecoveryDataGridViewTextBoxColumn});
            this.gvMain.DataSource = this.notOperationPSMPageGridDataBindingSource;
            this.gvMain.Location = new System.Drawing.Point(0, 478);
            this.gvMain.Name = "gvMain";
            this.gvMain.ReadOnly = true;
            this.gvMain.RowHeadersVisible = false;
            this.gvMain.RowTemplate.Height = 30;
            this.gvMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvMain.Size = new System.Drawing.Size(1834, 527);
            this.gvMain.TabIndex = 10;
            this.gvMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // noDataGridViewTextBoxColumn
            // 
            this.noDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.noDataGridViewTextBoxColumn.DataPropertyName = "No";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.noDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.noDataGridViewTextBoxColumn.HeaderText = "No";
            this.noDataGridViewTextBoxColumn.Name = "noDataGridViewTextBoxColumn";
            this.noDataGridViewTextBoxColumn.ReadOnly = true;
            this.noDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.noDataGridViewTextBoxColumn.Width = 107;
            // 
            // materialDataGridViewTextBoxColumn
            // 
            this.materialDataGridViewTextBoxColumn.DataPropertyName = "Material";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.materialDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.materialDataGridViewTextBoxColumn.HeaderText = "물질";
            this.materialDataGridViewTextBoxColumn.Name = "materialDataGridViewTextBoxColumn";
            this.materialDataGridViewTextBoxColumn.ReadOnly = true;
            this.materialDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // buildingDataGridViewTextBoxColumn
            // 
            this.buildingDataGridViewTextBoxColumn.DataPropertyName = "Building";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.buildingDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.buildingDataGridViewTextBoxColumn.HeaderText = "건물";
            this.buildingDataGridViewTextBoxColumn.Name = "buildingDataGridViewTextBoxColumn";
            this.buildingDataGridViewTextBoxColumn.ReadOnly = true;
            this.buildingDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // locationDataGridViewTextBoxColumn
            // 
            this.locationDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.locationDataGridViewTextBoxColumn.DataPropertyName = "Location";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.locationDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.locationDataGridViewTextBoxColumn.HeaderText = "누출 발생장소";
            this.locationDataGridViewTextBoxColumn.Name = "locationDataGridViewTextBoxColumn";
            this.locationDataGridViewTextBoxColumn.ReadOnly = true;
            this.locationDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // detectDataGridViewTextBoxColumn
            // 
            this.detectDataGridViewTextBoxColumn.DataPropertyName = "Detect";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.detectDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.detectDataGridViewTextBoxColumn.HeaderText = "탐지";
            this.detectDataGridViewTextBoxColumn.Name = "detectDataGridViewTextBoxColumn";
            this.detectDataGridViewTextBoxColumn.ReadOnly = true;
            this.detectDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // reportDataGridViewTextBoxColumn
            // 
            this.reportDataGridViewTextBoxColumn.DataPropertyName = "Report";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.reportDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.reportDataGridViewTextBoxColumn.HeaderText = "누출신고";
            this.reportDataGridViewTextBoxColumn.Name = "reportDataGridViewTextBoxColumn";
            this.reportDataGridViewTextBoxColumn.ReadOnly = true;
            this.reportDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // systemRecoveryDataGridViewTextBoxColumn
            // 
            this.systemRecoveryDataGridViewTextBoxColumn.DataPropertyName = "SystemRecovery";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.systemRecoveryDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle8;
            this.systemRecoveryDataGridViewTextBoxColumn.HeaderText = "시스템복구";
            this.systemRecoveryDataGridViewTextBoxColumn.Name = "systemRecoveryDataGridViewTextBoxColumn";
            this.systemRecoveryDataGridViewTextBoxColumn.ReadOnly = true;
            this.systemRecoveryDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // fieldRecoveryDataGridViewTextBoxColumn
            // 
            this.fieldRecoveryDataGridViewTextBoxColumn.DataPropertyName = "FieldRecovery";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.fieldRecoveryDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle9;
            this.fieldRecoveryDataGridViewTextBoxColumn.HeaderText = "현장복구";
            this.fieldRecoveryDataGridViewTextBoxColumn.Name = "fieldRecoveryDataGridViewTextBoxColumn";
            this.fieldRecoveryDataGridViewTextBoxColumn.ReadOnly = true;
            this.fieldRecoveryDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // notOperationPSMPageGridDataBindingSource
            // 
            this.notOperationPSMPageGridDataBindingSource.DataSource = typeof(SDMS.Report.NotOperationPSMPageGridData);
            // 
            // cboChart
            // 
            this.cboChart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChart.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboChart.FormattingEnabled = true;
            this.cboChart.Location = new System.Drawing.Point(1580, 59);
            this.cboChart.Name = "cboChart";
            this.cboChart.Size = new System.Drawing.Size(121, 25);
            this.cboChart.TabIndex = 11;
            this.cboChart.SelectedIndexChanged += new System.EventHandler(this.cboChart_SelectedIndexChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(209, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 18);
            this.label1.TabIndex = 13;
            this.label1.Text = "조회 기간";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(293, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 18);
            this.label4.TabIndex = 14;
            this.label4.Text = "l";
            this.label4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblMinDate
            // 
            this.lblMinDate.AutoSize = true;
            this.lblMinDate.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMinDate.ForeColor = System.Drawing.Color.White;
            this.lblMinDate.Location = new System.Drawing.Point(305, 30);
            this.lblMinDate.Name = "lblMinDate";
            this.lblMinDate.Size = new System.Drawing.Size(72, 18);
            this.lblMinDate.TabIndex = 15;
            this.lblMinDate.Text = "MinDate";
            this.lblMinDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblMaxDate
            // 
            this.lblMaxDate.AutoSize = true;
            this.lblMaxDate.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMaxDate.ForeColor = System.Drawing.Color.White;
            this.lblMaxDate.Location = new System.Drawing.Point(398, 30);
            this.lblMaxDate.Name = "lblMaxDate";
            this.lblMaxDate.Size = new System.Drawing.Size(79, 18);
            this.lblMaxDate.TabIndex = 16;
            this.lblMaxDate.Text = "MaxDate";
            this.lblMaxDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(486, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 18);
            this.label7.TabIndex = 17;
            this.label7.Text = "조회 범위";
            this.label7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(566, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 18);
            this.label8.TabIndex = 18;
            this.label8.Text = "l";
            this.label8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblBuilding
            // 
            this.lblBuilding.AutoSize = true;
            this.lblBuilding.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblBuilding.ForeColor = System.Drawing.Color.White;
            this.lblBuilding.Location = new System.Drawing.Point(574, 30);
            this.lblBuilding.Name = "lblBuilding";
            this.lblBuilding.Size = new System.Drawing.Size(86, 18);
            this.lblBuilding.TabIndex = 19;
            this.lblBuilding.Text = "모든 건물";
            this.lblBuilding.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(144)))), ((int)(((byte)(139)))));
            this.panel1.Location = new System.Drawing.Point(43, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1750, 3);
            this.panel1.TabIndex = 36;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // btnSaveHWP
            // 
            this.btnSaveHWP.ButtonText = "";
            this.btnSaveHWP.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveHWP.ImageClicked = global::SDMS.Properties.Resources.BtnSaveHWP_Click;
            this.btnSaveHWP.ImageDisabled = null;
            this.btnSaveHWP.ImageMouseOver = global::SDMS.Properties.Resources.BtnSaveHWP_Click;
            this.btnSaveHWP.ImageNormal = global::SDMS.Properties.Resources.BtnSaveHWP_Default;
            this.btnSaveHWP.Location = new System.Drawing.Point(1710, 17);
            this.btnSaveHWP.Name = "btnSaveHWP";
            this.btnSaveHWP.Owner = null;
            this.btnSaveHWP.Size = new System.Drawing.Size(83, 29);
            this.btnSaveHWP.TabIndex = 61;
            this.btnSaveHWP.TabStop = false;
            this.btnSaveHWP.TextColor = System.Drawing.Color.Black;
            this.btnSaveHWP.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveHWP.ToolTipText = "";
            this.btnSaveHWP.UseToolTip = false;
            this.btnSaveHWP.WindowRateWidth = 1F;
            this.btnSaveHWP.Click += new System.EventHandler(this.btnSaveHWP_Click);
            // 
            // panelChart
            // 
            this.panelChart.Location = new System.Drawing.Point(54, 95);
            this.panelChart.Name = "panelChart";
            this.panelChart.Size = new System.Drawing.Size(1704, 368);
            this.panelChart.TabIndex = 67;
            this.panelChart.Paint += new System.Windows.Forms.PaintEventHandler(this.panelChart_Paint);
            this.panelChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // cboPageIndex
            // 
            this.cboPageIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPageIndex.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboPageIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageIndex.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboPageIndex.FormattingEnabled = true;
            this.cboPageIndex.Location = new System.Drawing.Point(1416, 59);
            this.cboPageIndex.MaxDropDownItems = 20;
            this.cboPageIndex.Name = "cboPageIndex";
            this.cboPageIndex.Size = new System.Drawing.Size(58, 25);
            this.cboPageIndex.TabIndex = 71;
            this.cboPageIndex.Visible = false;
            this.cboPageIndex.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cboPageIndex_DrawItem);
            this.cboPageIndex.SelectedIndexChanged += new System.EventHandler(this.cboPageIndex_SelectedIndexChanged);
            // 
            // lblTotalPage
            // 
            this.lblTotalPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalPage.AutoSize = true;
            this.lblTotalPage.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTotalPage.ForeColor = System.Drawing.Color.White;
            this.lblTotalPage.Location = new System.Drawing.Point(1707, 63);
            this.lblTotalPage.Name = "lblTotalPage";
            this.lblTotalPage.Size = new System.Drawing.Size(58, 15);
            this.lblTotalPage.TabIndex = 68;
            this.lblTotalPage.Text = "/ 10000";
            this.lblTotalPage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::SDMS.Properties.Resources.NotOperationIntrusionPageLegend;
            this.pictureBox1.Location = new System.Drawing.Point(597, 60);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(286, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 72;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // btnNextIndex
            // 
            this.btnNextIndex.ButtonText = "";
            this.btnNextIndex.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNextIndex.ImageClicked = global::SDMS.Properties.Resources.BtnRightArrow_Click;
            this.btnNextIndex.ImageDisabled = null;
            this.btnNextIndex.ImageMouseOver = global::SDMS.Properties.Resources.BtnRightArrow_Click;
            this.btnNextIndex.ImageNormal = global::SDMS.Properties.Resources.BtnRightArrow_Default;
            this.btnNextIndex.Location = new System.Drawing.Point(1775, 265);
            this.btnNextIndex.Name = "btnNextIndex";
            this.btnNextIndex.Owner = null;
            this.btnNextIndex.Size = new System.Drawing.Size(18, 30);
            this.btnNextIndex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnNextIndex.TabIndex = 74;
            this.btnNextIndex.TabStop = false;
            this.btnNextIndex.TextColor = System.Drawing.Color.Black;
            this.btnNextIndex.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNextIndex.ToolTipText = "";
            this.btnNextIndex.UseToolTip = false;
            this.btnNextIndex.WindowRateWidth = 1F;
            this.btnNextIndex.Click += new System.EventHandler(this.btnNextIndex_Click);
            // 
            // btnPreviousIndex
            // 
            this.btnPreviousIndex.ButtonText = "";
            this.btnPreviousIndex.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPreviousIndex.ImageClicked = global::SDMS.Properties.Resources.BtnLeftArrow_Click;
            this.btnPreviousIndex.ImageDisabled = null;
            this.btnPreviousIndex.ImageMouseOver = global::SDMS.Properties.Resources.BtnLeftArrow_Click;
            this.btnPreviousIndex.ImageNormal = global::SDMS.Properties.Resources.BtnLeftArrow_Default;
            this.btnPreviousIndex.Location = new System.Drawing.Point(23, 265);
            this.btnPreviousIndex.Name = "btnPreviousIndex";
            this.btnPreviousIndex.Owner = null;
            this.btnPreviousIndex.Size = new System.Drawing.Size(16, 30);
            this.btnPreviousIndex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnPreviousIndex.TabIndex = 73;
            this.btnPreviousIndex.TabStop = false;
            this.btnPreviousIndex.TextColor = System.Drawing.Color.Black;
            this.btnPreviousIndex.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPreviousIndex.ToolTipText = "";
            this.btnPreviousIndex.UseToolTip = false;
            this.btnPreviousIndex.WindowRateWidth = 1F;
            this.btnPreviousIndex.Click += new System.EventHandler(this.btnPreviousIndex_Click);
            // 
            // NotOperationPSMPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(45)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1834, 1005);
            this.Controls.Add(this.btnNextIndex);
            this.Controls.Add(this.btnPreviousIndex);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.cboPageIndex);
            this.Controls.Add(this.lblTotalPage);
            this.Controls.Add(this.panelChart);
            this.Controls.Add(this.btnSaveHWP);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblBuilding);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblMaxDate);
            this.Controls.Add(this.lblMinDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboChart);
            this.Controls.Add(this.gvMain);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.winChartViewer1);
            this.Name = "NotOperationPSMPage";
            this.Text = "NotOperationPSMPage";
            this.Load += new System.EventHandler(this.NotOperationPSMPage_Load);
            this.Resize += new System.EventHandler(this.NotOperationPSMPage_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.notOperationPSMPageGridDataBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveHWP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousIndex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblBuilding;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblMaxDate;
        private System.Windows.Forms.Label lblMinDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboChart;
        private System.Windows.Forms.DataGridView gvMain;
        private ChartDirector.WinChartViewer winChartViewer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.BindingSource notOperationPSMPageGridDataBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn noDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn materialDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn buildingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn locationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn detectDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn reportDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn systemRecoveryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fieldRecoveryDataGridViewTextBoxColumn;
        private System.Windows.Forms.Panel panel1;
        private UnE.GUI.ImageButton btnSaveHWP;
        private System.Windows.Forms.Panel panelChart;
        private System.Windows.Forms.ComboBox cboPageIndex;
        private System.Windows.Forms.Label lblTotalPage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private UnE.GUI.ImageButton btnNextIndex;
        private UnE.GUI.ImageButton btnPreviousIndex;
    }
}