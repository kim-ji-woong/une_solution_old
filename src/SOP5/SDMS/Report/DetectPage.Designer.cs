namespace SDMS
{
    partial class DetectPage
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblMinDate = new System.Windows.Forms.Label();
            this.lblMaxDate = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.noDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeStampDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sensorTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SensorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildingGroupDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.floorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.locationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMemo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.gridDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.lblBuilding = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotalPage = new System.Windows.Forms.Label();
            this.cboPageIndex = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.popupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuShowFireReaction = new System.Windows.Forms.ToolStripMenuItem();
            this.panelChart = new System.Windows.Forms.Panel();
            this.btnNextIndex = new UnE.GUI.ImageButton();
            this.btnPreviousIndex = new UnE.GUI.ImageButton();
            this.btnSaveHWP = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDataBindingSource)).BeginInit();
            this.popupMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveHWP)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(39, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(854, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "각 센서들이 탐지한 화재빈도를 표시합니다. 센서 오류 및 특정 상황에 의한 오작동을 포함한 빈도입니다.";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(37, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(178, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "화재 탐지 이력";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(213, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "조회 기간";
            this.label3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(296, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 18);
            this.label5.TabIndex = 10;
            this.label5.Text = "l";
            this.label5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblMinDate
            // 
            this.lblMinDate.AutoSize = true;
            this.lblMinDate.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMinDate.ForeColor = System.Drawing.Color.White;
            this.lblMinDate.Location = new System.Drawing.Point(306, 31);
            this.lblMinDate.Name = "lblMinDate";
            this.lblMinDate.Size = new System.Drawing.Size(72, 18);
            this.lblMinDate.TabIndex = 11;
            this.lblMinDate.Text = "MinDate";
            this.lblMinDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblMaxDate
            // 
            this.lblMaxDate.AutoSize = true;
            this.lblMaxDate.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMaxDate.ForeColor = System.Drawing.Color.White;
            this.lblMaxDate.Location = new System.Drawing.Point(377, 31);
            this.lblMaxDate.Name = "lblMaxDate";
            this.lblMaxDate.Size = new System.Drawing.Size(79, 18);
            this.lblMaxDate.TabIndex = 12;
            this.lblMaxDate.Text = "MaxDate";
            this.lblMaxDate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblMaxDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.noDataGridViewTextBoxColumn,
            this.timeStampDataGridViewTextBoxColumn,
            this.sensorTypeDataGridViewTextBoxColumn,
            this.SensorName,
            this.buildingGroupDataGridViewTextBoxColumn,
            this.buildingDataGridViewTextBoxColumn,
            this.floorDataGridViewTextBoxColumn,
            this.locationDataGridViewTextBoxColumn,
            this.colMemo,
            this.statusDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.gridDataBindingSource;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridView1.Location = new System.Drawing.Point(0, 495);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1834, 510);
            this.dataGridView1.TabIndex = 15;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            this.dataGridView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseUp);
            // 
            // noDataGridViewTextBoxColumn
            // 
            this.noDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.noDataGridViewTextBoxColumn.DataPropertyName = "No";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.noDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.noDataGridViewTextBoxColumn.FillWeight = 365.4822F;
            this.noDataGridViewTextBoxColumn.HeaderText = "No";
            this.noDataGridViewTextBoxColumn.Name = "noDataGridViewTextBoxColumn";
            this.noDataGridViewTextBoxColumn.ReadOnly = true;
            this.noDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.noDataGridViewTextBoxColumn.Width = 90;
            // 
            // timeStampDataGridViewTextBoxColumn
            // 
            this.timeStampDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.timeStampDataGridViewTextBoxColumn.DataPropertyName = "TimeStamp";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.timeStampDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.timeStampDataGridViewTextBoxColumn.FillWeight = 312.6904F;
            this.timeStampDataGridViewTextBoxColumn.HeaderText = "일시";
            this.timeStampDataGridViewTextBoxColumn.Name = "timeStampDataGridViewTextBoxColumn";
            this.timeStampDataGridViewTextBoxColumn.ReadOnly = true;
            this.timeStampDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.timeStampDataGridViewTextBoxColumn.Width = 77;
            // 
            // sensorTypeDataGridViewTextBoxColumn
            // 
            this.sensorTypeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.sensorTypeDataGridViewTextBoxColumn.DataPropertyName = "SensorType";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.sensorTypeDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.sensorTypeDataGridViewTextBoxColumn.FillWeight = 20.30456F;
            this.sensorTypeDataGridViewTextBoxColumn.HeaderText = "유형";
            this.sensorTypeDataGridViewTextBoxColumn.Name = "sensorTypeDataGridViewTextBoxColumn";
            this.sensorTypeDataGridViewTextBoxColumn.ReadOnly = true;
            this.sensorTypeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.sensorTypeDataGridViewTextBoxColumn.Width = 90;
            // 
            // SensorName
            // 
            this.SensorName.DataPropertyName = "SensorName";
            this.SensorName.HeaderText = "센서이름";
            this.SensorName.Name = "SensorName";
            this.SensorName.ReadOnly = true;
            // 
            // buildingGroupDataGridViewTextBoxColumn
            // 
            this.buildingGroupDataGridViewTextBoxColumn.DataPropertyName = "BuildingGroup";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.buildingGroupDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.buildingGroupDataGridViewTextBoxColumn.FillWeight = 20.30456F;
            this.buildingGroupDataGridViewTextBoxColumn.HeaderText = "건물 그룹";
            this.buildingGroupDataGridViewTextBoxColumn.Name = "buildingGroupDataGridViewTextBoxColumn";
            this.buildingGroupDataGridViewTextBoxColumn.ReadOnly = true;
            this.buildingGroupDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.buildingGroupDataGridViewTextBoxColumn.Visible = false;
            // 
            // buildingDataGridViewTextBoxColumn
            // 
            this.buildingDataGridViewTextBoxColumn.DataPropertyName = "Building";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.buildingDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.buildingDataGridViewTextBoxColumn.FillWeight = 20.30456F;
            this.buildingDataGridViewTextBoxColumn.HeaderText = "건물";
            this.buildingDataGridViewTextBoxColumn.Name = "buildingDataGridViewTextBoxColumn";
            this.buildingDataGridViewTextBoxColumn.ReadOnly = true;
            this.buildingDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // floorDataGridViewTextBoxColumn
            // 
            this.floorDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.floorDataGridViewTextBoxColumn.DataPropertyName = "Floor";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.floorDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.floorDataGridViewTextBoxColumn.FillWeight = 20.30456F;
            this.floorDataGridViewTextBoxColumn.HeaderText = "층";
            this.floorDataGridViewTextBoxColumn.Name = "floorDataGridViewTextBoxColumn";
            this.floorDataGridViewTextBoxColumn.ReadOnly = true;
            this.floorDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.floorDataGridViewTextBoxColumn.Width = 5;
            // 
            // locationDataGridViewTextBoxColumn
            // 
            this.locationDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.locationDataGridViewTextBoxColumn.DataPropertyName = "Location";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.locationDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle8;
            this.locationDataGridViewTextBoxColumn.FillWeight = 20.30456F;
            this.locationDataGridViewTextBoxColumn.HeaderText = "화재 발생장소";
            this.locationDataGridViewTextBoxColumn.Name = "locationDataGridViewTextBoxColumn";
            this.locationDataGridViewTextBoxColumn.ReadOnly = true;
            this.locationDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colMemo
            // 
            this.colMemo.DataPropertyName = "Memo";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colMemo.DefaultCellStyle = dataGridViewCellStyle9;
            this.colMemo.HeaderText = "메모";
            this.colMemo.Name = "colMemo";
            this.colMemo.ReadOnly = true;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.statusDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle10;
            this.statusDataGridViewTextBoxColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.statusDataGridViewTextBoxColumn.FillWeight = 20.30456F;
            this.statusDataGridViewTextBoxColumn.HeaderText = "상태";
            this.statusDataGridViewTextBoxColumn.Items.AddRange(new object[] {
            "실제",
            "오동작",
            "테스트"});
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.statusDataGridViewTextBoxColumn.Width = 5;
            // 
            // gridDataBindingSource
            // 
            this.gridDataBindingSource.DataSource = typeof(SDMS.Report.DetectPageGridData);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(537, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 18);
            this.label6.TabIndex = 10;
            this.label6.Text = "l";
            this.label6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblBuilding
            // 
            this.lblBuilding.AutoSize = true;
            this.lblBuilding.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblBuilding.ForeColor = System.Drawing.Color.White;
            this.lblBuilding.Location = new System.Drawing.Point(546, 31);
            this.lblBuilding.Name = "lblBuilding";
            this.lblBuilding.Size = new System.Drawing.Size(86, 18);
            this.lblBuilding.TabIndex = 13;
            this.lblBuilding.Text = "모든 건물";
            this.lblBuilding.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(456, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 18);
            this.label4.TabIndex = 7;
            this.label4.Text = "조회 범위";
            this.label4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblTotalPage
            // 
            this.lblTotalPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalPage.AutoSize = true;
            this.lblTotalPage.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTotalPage.ForeColor = System.Drawing.Color.White;
            this.lblTotalPage.Location = new System.Drawing.Point(1706, 60);
            this.lblTotalPage.Name = "lblTotalPage";
            this.lblTotalPage.Size = new System.Drawing.Size(58, 15);
            this.lblTotalPage.TabIndex = 17;
            this.lblTotalPage.Text = "/ 10000";
            this.lblTotalPage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // cboPageIndex
            // 
            this.cboPageIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPageIndex.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboPageIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageIndex.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboPageIndex.FormattingEnabled = true;
            this.cboPageIndex.Location = new System.Drawing.Point(1637, 56);
            this.cboPageIndex.MaxDropDownItems = 20;
            this.cboPageIndex.Name = "cboPageIndex";
            this.cboPageIndex.Size = new System.Drawing.Size(63, 25);
            this.cboPageIndex.TabIndex = 20;
            this.cboPageIndex.Visible = false;
            this.cboPageIndex.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cboPageIndex_DrawItem);
            this.cboPageIndex.SelectedIndexChanged += new System.EventHandler(this.cboPageIndex_SelectedIndexChanged);
            this.cboPageIndex.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(144)))), ((int)(((byte)(139)))));
            this.panel1.Location = new System.Drawing.Point(42, 53);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1750, 3);
            this.panel1.TabIndex = 21;
            // 
            // popupMenu
            // 
            this.popupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuShowFireReaction});
            this.popupMenu.Name = "popupMenu";
            this.popupMenu.Size = new System.Drawing.Size(151, 26);
            // 
            // menuShowFireReaction
            // 
            this.menuShowFireReaction.Name = "menuShowFireReaction";
            this.menuShowFireReaction.Size = new System.Drawing.Size(150, 22);
            this.menuShowFireReaction.Text = "대응이력 보기";
            this.menuShowFireReaction.Click += new System.EventHandler(this.menuShowFireReaction_Click);
            // 
            // panelChart
            // 
            this.panelChart.Location = new System.Drawing.Point(54, 83);
            this.panelChart.Name = "panelChart";
            this.panelChart.Size = new System.Drawing.Size(1724, 409);
            this.panelChart.TabIndex = 67;
            this.panelChart.Paint += new System.Windows.Forms.PaintEventHandler(this.panelChart_Paint);
            this.panelChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // btnNextIndex
            // 
            this.btnNextIndex.ButtonText = "";
            this.btnNextIndex.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNextIndex.ImageClicked = global::SDMS.Properties.Resources.BtnRightArrow_Click;
            this.btnNextIndex.ImageDisabled = null;
            this.btnNextIndex.ImageMouseOver = global::SDMS.Properties.Resources.BtnRightArrow_Click;
            this.btnNextIndex.ImageNormal = global::SDMS.Properties.Resources.BtnRightArrow_Default;
            this.btnNextIndex.Location = new System.Drawing.Point(1790, 276);
            this.btnNextIndex.Name = "btnNextIndex";
            this.btnNextIndex.Owner = null;
            this.btnNextIndex.Size = new System.Drawing.Size(18, 30);
            this.btnNextIndex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnNextIndex.TabIndex = 69;
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
            this.btnPreviousIndex.Location = new System.Drawing.Point(22, 276);
            this.btnPreviousIndex.Name = "btnPreviousIndex";
            this.btnPreviousIndex.Owner = null;
            this.btnPreviousIndex.Size = new System.Drawing.Size(16, 30);
            this.btnPreviousIndex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnPreviousIndex.TabIndex = 68;
            this.btnPreviousIndex.TabStop = false;
            this.btnPreviousIndex.TextColor = System.Drawing.Color.Black;
            this.btnPreviousIndex.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPreviousIndex.ToolTipText = "";
            this.btnPreviousIndex.UseToolTip = false;
            this.btnPreviousIndex.WindowRateWidth = 1F;
            this.btnPreviousIndex.Click += new System.EventHandler(this.btnPreviousIndex_Click);
            // 
            // btnSaveHWP
            // 
            this.btnSaveHWP.ButtonText = "";
            this.btnSaveHWP.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveHWP.ImageClicked = global::SDMS.Properties.Resources.BtnSaveHWP_Click;
            this.btnSaveHWP.ImageDisabled = null;
            this.btnSaveHWP.ImageMouseOver = global::SDMS.Properties.Resources.BtnSaveHWP_Click;
            this.btnSaveHWP.ImageNormal = global::SDMS.Properties.Resources.BtnSaveHWP_Default;
            this.btnSaveHWP.Location = new System.Drawing.Point(1709, 18);
            this.btnSaveHWP.Name = "btnSaveHWP";
            this.btnSaveHWP.Owner = null;
            this.btnSaveHWP.Size = new System.Drawing.Size(83, 29);
            this.btnSaveHWP.TabIndex = 36;
            this.btnSaveHWP.TabStop = false;
            this.btnSaveHWP.TextColor = System.Drawing.Color.Black;
            this.btnSaveHWP.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveHWP.ToolTipText = "";
            this.btnSaveHWP.UseToolTip = false;
            this.btnSaveHWP.WindowRateWidth = 1F;
            this.btnSaveHWP.Click += new System.EventHandler(this.btnSaveHWP_Click);
            // 
            // DetectPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(45)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1834, 1005);
            this.Controls.Add(this.btnNextIndex);
            this.Controls.Add(this.btnPreviousIndex);
            this.Controls.Add(this.lblTotalPage);
            this.Controls.Add(this.panelChart);
            this.Controls.Add(this.btnSaveHWP);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cboPageIndex);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lblBuilding);
            this.Controls.Add(this.lblMaxDate);
            this.Controls.Add(this.lblMinDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "DetectPage";
            this.Text = "DetectPage";
            this.Load += new System.EventHandler(this.DetectPage_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            this.Resize += new System.EventHandler(this.DetectPage_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDataBindingSource)).EndInit();
            this.popupMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnNextIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveHWP)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public ChartDirector.WinChartViewer WinChartViewer1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblMinDate;
        private System.Windows.Forms.Label lblMaxDate;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblBuilding;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTotalPage;
        private System.Windows.Forms.ComboBox cboPageIndex;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ContextMenuStrip popupMenu;
        private System.Windows.Forms.ToolStripMenuItem menuShowFireReaction;
        private System.Windows.Forms.BindingSource gridDataBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn noDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeStampDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sensorTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SensorName;
        private System.Windows.Forms.DataGridViewTextBoxColumn buildingGroupDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn buildingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn floorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn locationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMemo;
        private System.Windows.Forms.DataGridViewComboBoxColumn statusDataGridViewTextBoxColumn;
        private UnE.GUI.ImageButton btnSaveHWP;
        private System.Windows.Forms.Panel panelChart;
        private UnE.GUI.ImageButton btnPreviousIndex;
        private UnE.GUI.ImageButton btnNextIndex;


    }
}