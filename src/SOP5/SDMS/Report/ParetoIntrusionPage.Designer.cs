namespace SDMS
{
    partial class ParetoIntrusionPage
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridViewSensor = new System.Windows.Forms.DataGridView();
            this.lblBuilding = new System.Windows.Forms.Label();
            this.lblMaxDate = new System.Windows.Forms.Label();
            this.lblMinDate = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.cboChart = new System.Windows.Forms.ComboBox();
            this.dataGridViewEquipZone = new System.Windows.Forms.DataGridView();
            this.winChartViewerEquipZone = new ChartDirector.WinChartViewer();
            this.winChartViewerSensor = new ChartDirector.WinChartViewer();
            this.cboPageIndexEquipZone = new System.Windows.Forms.ComboBox();
            this.cboPageIndexSensor = new System.Windows.Forms.ComboBox();
            this.lblTotalPageEquipZone = new System.Windows.Forms.Label();
            this.lblTotalPageSensor = new System.Windows.Forms.Label();
            this.btnSaveHWP = new UnE.GUI.ImageButton();
            this.btnNextIndexEquipZone = new UnE.GUI.ImageButton();
            this.btnNextIndexSensor = new UnE.GUI.ImageButton();
            this.btnPreviousIndexEquipZone = new UnE.GUI.ImageButton();
            this.btnPreviousIndexSensor = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSensor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEquipZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerEquipZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerSensor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveHWP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextIndexEquipZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextIndexSensor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousIndexEquipZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousIndexSensor)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(144)))), ((int)(((byte)(139)))));
            this.panel1.Location = new System.Drawing.Point(43, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1750, 3);
            this.panel1.TabIndex = 51;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // dataGridViewSensor
            // 
            this.dataGridViewSensor.AllowUserToAddRows = false;
            this.dataGridViewSensor.AllowUserToDeleteRows = false;
            this.dataGridViewSensor.AllowUserToResizeRows = false;
            this.dataGridViewSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewSensor.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewSensor.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewSensor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewSensor.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewSensor.Location = new System.Drawing.Point(0, 435);
            this.dataGridViewSensor.MultiSelect = false;
            this.dataGridViewSensor.Name = "dataGridViewSensor";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewSensor.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewSensor.RowHeadersVisible = false;
            this.dataGridViewSensor.RowTemplate.Height = 30;
            this.dataGridViewSensor.RowTemplate.ReadOnly = true;
            this.dataGridViewSensor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSensor.Size = new System.Drawing.Size(1834, 570);
            this.dataGridViewSensor.TabIndex = 46;
            this.dataGridViewSensor.Visible = false;
            this.dataGridViewSensor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblBuilding
            // 
            this.lblBuilding.AutoSize = true;
            this.lblBuilding.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblBuilding.ForeColor = System.Drawing.Color.White;
            this.lblBuilding.Location = new System.Drawing.Point(543, 28);
            this.lblBuilding.Name = "lblBuilding";
            this.lblBuilding.Size = new System.Drawing.Size(86, 18);
            this.lblBuilding.TabIndex = 45;
            this.lblBuilding.Text = "모든 건물";
            // 
            // lblMaxDate
            // 
            this.lblMaxDate.AutoSize = true;
            this.lblMaxDate.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMaxDate.ForeColor = System.Drawing.Color.White;
            this.lblMaxDate.Location = new System.Drawing.Point(376, 28);
            this.lblMaxDate.Name = "lblMaxDate";
            this.lblMaxDate.Size = new System.Drawing.Size(79, 18);
            this.lblMaxDate.TabIndex = 44;
            this.lblMaxDate.Text = "MaxDate";
            this.lblMaxDate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblMaxDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblMinDate
            // 
            this.lblMinDate.AutoSize = true;
            this.lblMinDate.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMinDate.ForeColor = System.Drawing.Color.White;
            this.lblMinDate.Location = new System.Drawing.Point(305, 28);
            this.lblMinDate.Name = "lblMinDate";
            this.lblMinDate.Size = new System.Drawing.Size(72, 18);
            this.lblMinDate.TabIndex = 43;
            this.lblMinDate.Text = "MinDate";
            this.lblMinDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(535, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 18);
            this.label6.TabIndex = 42;
            this.label6.Text = "l";
            this.label6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(296, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 18);
            this.label5.TabIndex = 41;
            this.label5.Text = "l";
            this.label5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(454, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 18);
            this.label4.TabIndex = 39;
            this.label4.Text = "조회 범위";
            this.label4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(214, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 18);
            this.label3.TabIndex = 38;
            this.label3.Text = "조회 기간";
            this.label3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(38, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(178, 24);
            this.lblTitle.TabIndex = 37;
            this.lblTitle.Text = "방범 탐지 분석";
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDescription.ForeColor = System.Drawing.Color.White;
            this.lblDescription.Location = new System.Drawing.Point(40, 66);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(560, 18);
            this.lblDescription.TabIndex = 36;
            this.lblDescription.Text = "작동 빈도가 높은 센서들부터 왼쪽에서 오른쪽 방향으로 표시합니다.";
            this.lblDescription.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // cboChart
            // 
            this.cboChart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboChart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChart.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboChart.FormattingEnabled = true;
            this.cboChart.Items.AddRange(new object[] {
            "센서별 보기",
            "위치별 보기"});
            this.cboChart.Location = new System.Drawing.Point(1609, 58);
            this.cboChart.Name = "cboChart";
            this.cboChart.Size = new System.Drawing.Size(121, 25);
            this.cboChart.TabIndex = 57;
            this.cboChart.SelectedIndexChanged += new System.EventHandler(this.cboChart_SelectedIndexChanged);
            this.cboChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // dataGridViewEquipZone
            // 
            this.dataGridViewEquipZone.AllowUserToAddRows = false;
            this.dataGridViewEquipZone.AllowUserToDeleteRows = false;
            this.dataGridViewEquipZone.AllowUserToResizeRows = false;
            this.dataGridViewEquipZone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewEquipZone.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewEquipZone.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridViewEquipZone.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewEquipZone.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridViewEquipZone.Location = new System.Drawing.Point(0, 435);
            this.dataGridViewEquipZone.MultiSelect = false;
            this.dataGridViewEquipZone.Name = "dataGridViewEquipZone";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewEquipZone.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridViewEquipZone.RowHeadersVisible = false;
            this.dataGridViewEquipZone.RowTemplate.Height = 30;
            this.dataGridViewEquipZone.RowTemplate.ReadOnly = true;
            this.dataGridViewEquipZone.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewEquipZone.Size = new System.Drawing.Size(1834, 570);
            this.dataGridViewEquipZone.TabIndex = 58;
            this.dataGridViewEquipZone.Visible = false;
            this.dataGridViewEquipZone.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // winChartViewerEquipZone
            // 
            this.winChartViewerEquipZone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.winChartViewerEquipZone.Location = new System.Drawing.Point(44, 90);
            this.winChartViewerEquipZone.Name = "winChartViewerEquipZone";
            this.winChartViewerEquipZone.Size = new System.Drawing.Size(1750, 330);
            this.winChartViewerEquipZone.TabIndex = 40;
            this.winChartViewerEquipZone.TabStop = false;
            this.winChartViewerEquipZone.Visible = false;
            this.winChartViewerEquipZone.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // winChartViewerSensor
            // 
            this.winChartViewerSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.winChartViewerSensor.Location = new System.Drawing.Point(44, 90);
            this.winChartViewerSensor.Name = "winChartViewerSensor";
            this.winChartViewerSensor.Size = new System.Drawing.Size(1750, 330);
            this.winChartViewerSensor.TabIndex = 40;
            this.winChartViewerSensor.TabStop = false;
            this.winChartViewerSensor.Visible = false;
            this.winChartViewerSensor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // cboPageIndexEquipZone
            // 
            this.cboPageIndexEquipZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPageIndexEquipZone.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboPageIndexEquipZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageIndexEquipZone.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboPageIndexEquipZone.FormattingEnabled = true;
            this.cboPageIndexEquipZone.Location = new System.Drawing.Point(1636, 20);
            this.cboPageIndexEquipZone.MaxDropDownItems = 20;
            this.cboPageIndexEquipZone.Name = "cboPageIndexEquipZone";
            this.cboPageIndexEquipZone.Size = new System.Drawing.Size(54, 28);
            this.cboPageIndexEquipZone.TabIndex = 65;
            this.cboPageIndexEquipZone.Visible = false;
            this.cboPageIndexEquipZone.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cboPageIndex_DrawItem);
            this.cboPageIndexEquipZone.SelectedIndexChanged += new System.EventHandler(this.cboPageIndex_SelectedIndexChanged);
            this.cboPageIndexEquipZone.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // cboPageIndexSensor
            // 
            this.cboPageIndexSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPageIndexSensor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboPageIndexSensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageIndexSensor.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboPageIndexSensor.FormattingEnabled = true;
            this.cboPageIndexSensor.Location = new System.Drawing.Point(1636, 21);
            this.cboPageIndexSensor.MaxDropDownItems = 20;
            this.cboPageIndexSensor.Name = "cboPageIndexSensor";
            this.cboPageIndexSensor.Size = new System.Drawing.Size(54, 25);
            this.cboPageIndexSensor.TabIndex = 66;
            this.cboPageIndexSensor.Visible = false;
            this.cboPageIndexSensor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cboPageIndex_DrawItem);
            this.cboPageIndexSensor.SelectedIndexChanged += new System.EventHandler(this.cboPageIndex_SelectedIndexChanged);
            // 
            // lblTotalPageEquipZone
            // 
            this.lblTotalPageEquipZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalPageEquipZone.AutoSize = true;
            this.lblTotalPageEquipZone.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTotalPageEquipZone.ForeColor = System.Drawing.Color.White;
            this.lblTotalPageEquipZone.Location = new System.Drawing.Point(1750, 61);
            this.lblTotalPageEquipZone.Name = "lblTotalPageEquipZone";
            this.lblTotalPageEquipZone.Size = new System.Drawing.Size(40, 17);
            this.lblTotalPageEquipZone.TabIndex = 59;
            this.lblTotalPageEquipZone.Text = "1 / 1";
            this.lblTotalPageEquipZone.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblTotalPageSensor
            // 
            this.lblTotalPageSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalPageSensor.AutoSize = true;
            this.lblTotalPageSensor.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTotalPageSensor.ForeColor = System.Drawing.Color.White;
            this.lblTotalPageSensor.Location = new System.Drawing.Point(1750, 62);
            this.lblTotalPageSensor.Name = "lblTotalPageSensor";
            this.lblTotalPageSensor.Size = new System.Drawing.Size(40, 17);
            this.lblTotalPageSensor.TabIndex = 60;
            this.lblTotalPageSensor.Text = "1 / 1";
            // 
            // btnSaveHWP
            // 
            this.btnSaveHWP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnSaveHWP.TabIndex = 67;
            this.btnSaveHWP.TabStop = false;
            this.btnSaveHWP.TextColor = System.Drawing.Color.Black;
            this.btnSaveHWP.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveHWP.ToolTipText = "";
            this.btnSaveHWP.UseToolTip = false;
            this.btnSaveHWP.WindowRateWidth = 1F;
            this.btnSaveHWP.Click += new System.EventHandler(this.btnSaveHWP_Click);
            // 
            // btnNextIndexEquipZone
            // 
            this.btnNextIndexEquipZone.ButtonText = "";
            this.btnNextIndexEquipZone.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNextIndexEquipZone.ImageClicked = global::SDMS.Properties.Resources.BtnRightArrow_Click;
            this.btnNextIndexEquipZone.ImageDisabled = null;
            this.btnNextIndexEquipZone.ImageMouseOver = global::SDMS.Properties.Resources.BtnRightArrow_Click;
            this.btnNextIndexEquipZone.ImageNormal = global::SDMS.Properties.Resources.BtnRightArrow_Default;
            this.btnNextIndexEquipZone.Location = new System.Drawing.Point(1799, 241);
            this.btnNextIndexEquipZone.Name = "btnNextIndexEquipZone";
            this.btnNextIndexEquipZone.Owner = null;
            this.btnNextIndexEquipZone.Size = new System.Drawing.Size(18, 30);
            this.btnNextIndexEquipZone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnNextIndexEquipZone.TabIndex = 75;
            this.btnNextIndexEquipZone.TabStop = false;
            this.btnNextIndexEquipZone.TextColor = System.Drawing.Color.Black;
            this.btnNextIndexEquipZone.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNextIndexEquipZone.ToolTipText = "";
            this.btnNextIndexEquipZone.UseToolTip = false;
            this.btnNextIndexEquipZone.WindowRateWidth = 1F;
            this.btnNextIndexEquipZone.Click += new System.EventHandler(this.btnNextIndex_Click);
            // 
            // btnNextIndexSensor
            // 
            this.btnNextIndexSensor.ButtonText = "";
            this.btnNextIndexSensor.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNextIndexSensor.ImageClicked = global::SDMS.Properties.Resources.BtnRightArrow_Click;
            this.btnNextIndexSensor.ImageDisabled = null;
            this.btnNextIndexSensor.ImageMouseOver = global::SDMS.Properties.Resources.BtnRightArrow_Click;
            this.btnNextIndexSensor.ImageNormal = global::SDMS.Properties.Resources.BtnRightArrow_Default;
            this.btnNextIndexSensor.Location = new System.Drawing.Point(1799, 241);
            this.btnNextIndexSensor.Name = "btnNextIndexSensor";
            this.btnNextIndexSensor.Owner = null;
            this.btnNextIndexSensor.Size = new System.Drawing.Size(18, 30);
            this.btnNextIndexSensor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnNextIndexSensor.TabIndex = 74;
            this.btnNextIndexSensor.TabStop = false;
            this.btnNextIndexSensor.TextColor = System.Drawing.Color.Black;
            this.btnNextIndexSensor.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNextIndexSensor.ToolTipText = "";
            this.btnNextIndexSensor.UseToolTip = false;
            this.btnNextIndexSensor.WindowRateWidth = 1F;
            this.btnNextIndexSensor.Click += new System.EventHandler(this.btnNextIndex_Click);
            // 
            // btnPreviousIndexEquipZone
            // 
            this.btnPreviousIndexEquipZone.ButtonText = "";
            this.btnPreviousIndexEquipZone.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPreviousIndexEquipZone.ImageClicked = global::SDMS.Properties.Resources.BtnLeftArrow_Click;
            this.btnPreviousIndexEquipZone.ImageDisabled = null;
            this.btnPreviousIndexEquipZone.ImageMouseOver = global::SDMS.Properties.Resources.BtnLeftArrow_Click;
            this.btnPreviousIndexEquipZone.ImageNormal = global::SDMS.Properties.Resources.BtnLeftArrow_Default;
            this.btnPreviousIndexEquipZone.Location = new System.Drawing.Point(22, 241);
            this.btnPreviousIndexEquipZone.Name = "btnPreviousIndexEquipZone";
            this.btnPreviousIndexEquipZone.Owner = null;
            this.btnPreviousIndexEquipZone.Size = new System.Drawing.Size(16, 30);
            this.btnPreviousIndexEquipZone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnPreviousIndexEquipZone.TabIndex = 77;
            this.btnPreviousIndexEquipZone.TabStop = false;
            this.btnPreviousIndexEquipZone.TextColor = System.Drawing.Color.Black;
            this.btnPreviousIndexEquipZone.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPreviousIndexEquipZone.ToolTipText = "";
            this.btnPreviousIndexEquipZone.UseToolTip = false;
            this.btnPreviousIndexEquipZone.WindowRateWidth = 1F;
            this.btnPreviousIndexEquipZone.Click += new System.EventHandler(this.btnPreviousIndex_Click);
            // 
            // btnPreviousIndexSensor
            // 
            this.btnPreviousIndexSensor.ButtonText = "";
            this.btnPreviousIndexSensor.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPreviousIndexSensor.ImageClicked = global::SDMS.Properties.Resources.BtnLeftArrow_Click;
            this.btnPreviousIndexSensor.ImageDisabled = null;
            this.btnPreviousIndexSensor.ImageMouseOver = global::SDMS.Properties.Resources.BtnLeftArrow_Click;
            this.btnPreviousIndexSensor.ImageNormal = global::SDMS.Properties.Resources.BtnLeftArrow_Default;
            this.btnPreviousIndexSensor.Location = new System.Drawing.Point(22, 241);
            this.btnPreviousIndexSensor.Name = "btnPreviousIndexSensor";
            this.btnPreviousIndexSensor.Owner = null;
            this.btnPreviousIndexSensor.Size = new System.Drawing.Size(16, 30);
            this.btnPreviousIndexSensor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnPreviousIndexSensor.TabIndex = 76;
            this.btnPreviousIndexSensor.TabStop = false;
            this.btnPreviousIndexSensor.TextColor = System.Drawing.Color.Black;
            this.btnPreviousIndexSensor.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPreviousIndexSensor.ToolTipText = "";
            this.btnPreviousIndexSensor.UseToolTip = false;
            this.btnPreviousIndexSensor.WindowRateWidth = 1F;
            this.btnPreviousIndexSensor.Click += new System.EventHandler(this.btnPreviousIndex_Click);
            // 
            // ParetoIntrusionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(45)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1834, 1005);
            this.Controls.Add(this.btnPreviousIndexEquipZone);
            this.Controls.Add(this.btnPreviousIndexSensor);
            this.Controls.Add(this.btnNextIndexEquipZone);
            this.Controls.Add(this.btnNextIndexSensor);
            this.Controls.Add(this.btnSaveHWP);
            this.Controls.Add(this.cboPageIndexEquipZone);
            this.Controls.Add(this.cboPageIndexSensor);
            this.Controls.Add(this.lblTotalPageEquipZone);
            this.Controls.Add(this.lblTotalPageSensor);
            this.Controls.Add(this.dataGridViewEquipZone);
            this.Controls.Add(this.cboChart);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridViewSensor);
            this.Controls.Add(this.lblBuilding);
            this.Controls.Add(this.lblMaxDate);
            this.Controls.Add(this.lblMinDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.winChartViewerEquipZone);
            this.Controls.Add(this.winChartViewerSensor);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblDescription);
            this.Name = "ParetoIntrusionPage";
            this.Text = "ParetoPage";
            this.Load += new System.EventHandler(this.ParetoIntrusionPage_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            this.Resize += new System.EventHandler(this.ParetoIntrusionPage_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSensor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEquipZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerEquipZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerSensor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveHWP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextIndexEquipZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextIndexSensor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousIndexEquipZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousIndexSensor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridViewSensor;
        private System.Windows.Forms.Label lblBuilding;
        private System.Windows.Forms.Label lblMaxDate;
        private System.Windows.Forms.Label lblMinDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private ChartDirector.WinChartViewer winChartViewerSensor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.ComboBox cboChart;
        private ChartDirector.WinChartViewer winChartViewerEquipZone;
        private System.Windows.Forms.DataGridView dataGridViewEquipZone;
        private System.Windows.Forms.ComboBox cboPageIndexEquipZone;
        private System.Windows.Forms.ComboBox cboPageIndexSensor;
        private System.Windows.Forms.Label lblTotalPageEquipZone;
        private System.Windows.Forms.Label lblTotalPageSensor;
        private UnE.GUI.ImageButton btnSaveHWP;
        private UnE.GUI.ImageButton btnNextIndexEquipZone;
        private UnE.GUI.ImageButton btnNextIndexSensor;
        private UnE.GUI.ImageButton btnPreviousIndexEquipZone;
        private UnE.GUI.ImageButton btnPreviousIndexSensor;
    }
}