namespace SDMS
{
    partial class NotOperationPage
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.winChartViewer1 = new ChartDirector.WinChartViewer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cboChart = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblMinDate = new System.Windows.Forms.Label();
            this.lblMaxDate = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblBuilding = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveHWP = new System.Windows.Forms.Button();
            this.notOperationPageGridDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.noDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sensorTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildingGroupDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.floorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.detectDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fireDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.malfunctionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fieldRecoveryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.malfunctionRateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.managerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.notOperationPageGridDataBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(23, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(170, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "화재 처리 이력";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(417, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "화재 탐지 중 값들 중 센서 오류 및 특정 상황에 의한 오작동률을 표시합니다.";
            // 
            // winChartViewer1
            // 
            this.winChartViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.winChartViewer1.Location = new System.Drawing.Point(152, 101);
            this.winChartViewer1.Name = "winChartViewer1";
            this.winChartViewer1.Size = new System.Drawing.Size(731, 237);
            this.winChartViewer1.TabIndex = 9;
            this.winChartViewer1.TabStop = false;
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
            this.sensorTypeDataGridViewTextBoxColumn,
            this.buildingGroupDataGridViewTextBoxColumn,
            this.buildingDataGridViewTextBoxColumn,
            this.floorDataGridViewTextBoxColumn,
            this.detectDataGridViewTextBoxColumn,
            this.fireDataGridViewTextBoxColumn,
            this.malfunctionDataGridViewTextBoxColumn,
            this.fieldRecoveryDataGridViewTextBoxColumn,
            this.malfunctionRateDataGridViewTextBoxColumn,
            this.managerDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.notOperationPageGridDataBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(25, 413);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(858, 116);
            this.dataGridView1.TabIndex = 10;
            // 
            // cboChart
            // 
            this.cboChart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChart.FormattingEnabled = true;
            this.cboChart.Location = new System.Drawing.Point(25, 101);
            this.cboChart.Name = "cboChart";
            this.cboChart.Size = new System.Drawing.Size(121, 20);
            this.cboChart.TabIndex = 11;
            this.cboChart.SelectedIndexChanged += new System.EventHandler(this.cboChart_SelectedIndexChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(201, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "조회 기간";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(254, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 19);
            this.label4.TabIndex = 14;
            this.label4.Text = "l";
            // 
            // lblMinDate
            // 
            this.lblMinDate.AutoSize = true;
            this.lblMinDate.Location = new System.Drawing.Point(265, 31);
            this.lblMinDate.Name = "lblMinDate";
            this.lblMinDate.Size = new System.Drawing.Size(51, 12);
            this.lblMinDate.TabIndex = 15;
            this.lblMinDate.Text = "MinDate";
            // 
            // lblMaxDate
            // 
            this.lblMaxDate.AutoSize = true;
            this.lblMaxDate.Location = new System.Drawing.Point(390, 31);
            this.lblMaxDate.Name = "lblMaxDate";
            this.lblMaxDate.Size = new System.Drawing.Size(55, 12);
            this.lblMaxDate.TabIndex = 16;
            this.lblMaxDate.Text = "MaxDate";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(558, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 12);
            this.label7.TabIndex = 17;
            this.label7.Text = "조회 범위";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(612, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 19);
            this.label8.TabIndex = 18;
            this.label8.Text = "l";
            // 
            // lblBuilding
            // 
            this.lblBuilding.AutoSize = true;
            this.lblBuilding.Location = new System.Drawing.Point(625, 31);
            this.lblBuilding.Name = "lblBuilding";
            this.lblBuilding.Size = new System.Drawing.Size(57, 12);
            this.lblBuilding.TabIndex = 19;
            this.lblBuilding.Text = "모든 건물";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(12, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(884, 5);
            this.panel1.TabIndex = 22;
            // 
            // btnSaveHWP
            // 
            this.btnSaveHWP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveHWP.Location = new System.Drawing.Point(806, 12);
            this.btnSaveHWP.Name = "btnSaveHWP";
            this.btnSaveHWP.Size = new System.Drawing.Size(90, 39);
            this.btnSaveHWP.TabIndex = 35;
            this.btnSaveHWP.Text = "한글파일저장";
            this.btnSaveHWP.UseVisualStyleBackColor = true;
            this.btnSaveHWP.Click += new System.EventHandler(this.btnSaveHWP_Click);
            // 
            // notOperationPageGridDataBindingSource
            // 
            this.notOperationPageGridDataBindingSource.DataSource = typeof(SDMS.Report.NotOperationPageGridData);
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
            this.noDataGridViewTextBoxColumn.Width = 78;
            // 
            // sensorTypeDataGridViewTextBoxColumn
            // 
            this.sensorTypeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.sensorTypeDataGridViewTextBoxColumn.DataPropertyName = "SensorType";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.sensorTypeDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.sensorTypeDataGridViewTextBoxColumn.HeaderText = "유형";
            this.sensorTypeDataGridViewTextBoxColumn.Name = "sensorTypeDataGridViewTextBoxColumn";
            this.sensorTypeDataGridViewTextBoxColumn.ReadOnly = true;
            this.sensorTypeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.sensorTypeDataGridViewTextBoxColumn.Width = 77;
            // 
            // buildingGroupDataGridViewTextBoxColumn
            // 
            this.buildingGroupDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.buildingGroupDataGridViewTextBoxColumn.DataPropertyName = "BuildingGroup";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.buildingGroupDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.buildingGroupDataGridViewTextBoxColumn.HeaderText = "건물 그룹";
            this.buildingGroupDataGridViewTextBoxColumn.Name = "buildingGroupDataGridViewTextBoxColumn";
            this.buildingGroupDataGridViewTextBoxColumn.ReadOnly = true;
            this.buildingGroupDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.buildingGroupDataGridViewTextBoxColumn.Width = 78;
            // 
            // buildingDataGridViewTextBoxColumn
            // 
            this.buildingDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.buildingDataGridViewTextBoxColumn.DataPropertyName = "Building";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.buildingDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.buildingDataGridViewTextBoxColumn.HeaderText = "건물";
            this.buildingDataGridViewTextBoxColumn.Name = "buildingDataGridViewTextBoxColumn";
            this.buildingDataGridViewTextBoxColumn.ReadOnly = true;
            this.buildingDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // floorDataGridViewTextBoxColumn
            // 
            this.floorDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.floorDataGridViewTextBoxColumn.DataPropertyName = "Floor";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.floorDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.floorDataGridViewTextBoxColumn.HeaderText = "층";
            this.floorDataGridViewTextBoxColumn.Name = "floorDataGridViewTextBoxColumn";
            this.floorDataGridViewTextBoxColumn.ReadOnly = true;
            this.floorDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.floorDataGridViewTextBoxColumn.Width = 78;
            // 
            // detectDataGridViewTextBoxColumn
            // 
            this.detectDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.detectDataGridViewTextBoxColumn.DataPropertyName = "Detect";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.detectDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.detectDataGridViewTextBoxColumn.HeaderText = "탐지";
            this.detectDataGridViewTextBoxColumn.Name = "detectDataGridViewTextBoxColumn";
            this.detectDataGridViewTextBoxColumn.ReadOnly = true;
            this.detectDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.detectDataGridViewTextBoxColumn.Width = 77;
            // 
            // fireDataGridViewTextBoxColumn
            // 
            this.fireDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.fireDataGridViewTextBoxColumn.DataPropertyName = "Fire";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.fireDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle8;
            this.fireDataGridViewTextBoxColumn.HeaderText = "화재";
            this.fireDataGridViewTextBoxColumn.Name = "fireDataGridViewTextBoxColumn";
            this.fireDataGridViewTextBoxColumn.ReadOnly = true;
            this.fireDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.fireDataGridViewTextBoxColumn.Width = 78;
            // 
            // malfunctionDataGridViewTextBoxColumn
            // 
            this.malfunctionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.malfunctionDataGridViewTextBoxColumn.DataPropertyName = "Malfunction";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.malfunctionDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle9;
            this.malfunctionDataGridViewTextBoxColumn.HeaderText = "오작동";
            this.malfunctionDataGridViewTextBoxColumn.Name = "malfunctionDataGridViewTextBoxColumn";
            this.malfunctionDataGridViewTextBoxColumn.ReadOnly = true;
            this.malfunctionDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.malfunctionDataGridViewTextBoxColumn.Width = 78;
            // 
            // fieldRecoveryDataGridViewTextBoxColumn
            // 
            this.fieldRecoveryDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.fieldRecoveryDataGridViewTextBoxColumn.DataPropertyName = "FieldRecovery";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.fieldRecoveryDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle10;
            this.fieldRecoveryDataGridViewTextBoxColumn.HeaderText = "현장복구";
            this.fieldRecoveryDataGridViewTextBoxColumn.Name = "fieldRecoveryDataGridViewTextBoxColumn";
            this.fieldRecoveryDataGridViewTextBoxColumn.ReadOnly = true;
            this.fieldRecoveryDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.fieldRecoveryDataGridViewTextBoxColumn.Width = 78;
            // 
            // malfunctionRateDataGridViewTextBoxColumn
            // 
            this.malfunctionRateDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.malfunctionRateDataGridViewTextBoxColumn.DataPropertyName = "MalfunctionRate";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.malfunctionRateDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle11;
            this.malfunctionRateDataGridViewTextBoxColumn.HeaderText = "오작동률";
            this.malfunctionRateDataGridViewTextBoxColumn.Name = "malfunctionRateDataGridViewTextBoxColumn";
            this.malfunctionRateDataGridViewTextBoxColumn.ReadOnly = true;
            this.malfunctionRateDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.malfunctionRateDataGridViewTextBoxColumn.Width = 77;
            // 
            // managerDataGridViewTextBoxColumn
            // 
            this.managerDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.managerDataGridViewTextBoxColumn.DataPropertyName = "Manager";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.managerDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle12;
            this.managerDataGridViewTextBoxColumn.HeaderText = "담당자";
            this.managerDataGridViewTextBoxColumn.Name = "managerDataGridViewTextBoxColumn";
            this.managerDataGridViewTextBoxColumn.ReadOnly = true;
            this.managerDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.managerDataGridViewTextBoxColumn.Visible = false;
            this.managerDataGridViewTextBoxColumn.Width = 78;
            // 
            // NotOperationPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(908, 554);
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
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.winChartViewer1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NotOperationPage";
            this.Text = "NotOperationPage";
            this.Load += new System.EventHandler(this.NotOperationPage_Load);
            this.Resize += new System.EventHandler(this.NotOperationPage_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.notOperationPageGridDataBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private ChartDirector.WinChartViewer winChartViewer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox cboChart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblMinDate;
        private System.Windows.Forms.Label lblMaxDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblBuilding;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSaveHWP;
        private System.Windows.Forms.BindingSource notOperationPageGridDataBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn noDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sensorTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn buildingGroupDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn buildingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn floorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn detectDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fireDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn malfunctionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fieldRecoveryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn malfunctionRateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn managerDataGridViewTextBoxColumn;

    }
}