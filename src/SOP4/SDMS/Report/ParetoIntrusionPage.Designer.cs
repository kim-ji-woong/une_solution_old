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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSaveHWP = new System.Windows.Forms.Button();
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
            this.cboPageIndexSensor = new System.Windows.Forms.ComboBox();
            this.btnNextIndexSensor = new System.Windows.Forms.Button();
            this.btnPreviousIndexSensor = new System.Windows.Forms.Button();
            this.lblTotalPageSensor = new System.Windows.Forms.Label();
            this.cboChart = new System.Windows.Forms.ComboBox();
            this.dataGridViewEquipZone = new System.Windows.Forms.DataGridView();
            this.winChartViewerEquipZone = new ChartDirector.WinChartViewer();
            this.winChartViewerSensor = new ChartDirector.WinChartViewer();
            this.lblTotalPageEquipZone = new System.Windows.Forms.Label();
            this.btnPreviousIndexEquipZone = new System.Windows.Forms.Button();
            this.btnNextIndexEquipZone = new System.Windows.Forms.Button();
            this.cboPageIndexEquipZone = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSensor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEquipZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerEquipZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerSensor)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSaveHWP
            // 
            this.btnSaveHWP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveHWP.Location = new System.Drawing.Point(806, 12);
            this.btnSaveHWP.Name = "btnSaveHWP";
            this.btnSaveHWP.Size = new System.Drawing.Size(90, 39);
            this.btnSaveHWP.TabIndex = 52;
            this.btnSaveHWP.Text = "한글파일저장";
            this.btnSaveHWP.UseVisualStyleBackColor = true;
            this.btnSaveHWP.Click += new System.EventHandler(this.btnSaveHWP_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(12, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(884, 5);
            this.panel1.TabIndex = 51;
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewSensor.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewSensor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewSensor.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewSensor.Location = new System.Drawing.Point(25, 388);
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
            this.dataGridViewSensor.Size = new System.Drawing.Size(858, 115);
            this.dataGridViewSensor.TabIndex = 46;
            this.dataGridViewSensor.Visible = false;
            // 
            // lblBuilding
            // 
            this.lblBuilding.AutoSize = true;
            this.lblBuilding.Location = new System.Drawing.Point(625, 31);
            this.lblBuilding.Name = "lblBuilding";
            this.lblBuilding.Size = new System.Drawing.Size(57, 12);
            this.lblBuilding.TabIndex = 45;
            this.lblBuilding.Text = "모든 건물";
            // 
            // lblMaxDate
            // 
            this.lblMaxDate.AutoSize = true;
            this.lblMaxDate.Location = new System.Drawing.Point(390, 31);
            this.lblMaxDate.Name = "lblMaxDate";
            this.lblMaxDate.Size = new System.Drawing.Size(55, 12);
            this.lblMaxDate.TabIndex = 44;
            this.lblMaxDate.Text = "MaxDate";
            this.lblMaxDate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblMinDate
            // 
            this.lblMinDate.AutoSize = true;
            this.lblMinDate.Location = new System.Drawing.Point(265, 31);
            this.lblMinDate.Name = "lblMinDate";
            this.lblMinDate.Size = new System.Drawing.Size(51, 12);
            this.lblMinDate.TabIndex = 43;
            this.lblMinDate.Text = "MinDate";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(612, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(12, 19);
            this.label6.TabIndex = 42;
            this.label6.Text = "l";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(254, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(12, 19);
            this.label5.TabIndex = 41;
            this.label5.Text = "l";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(558, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 12);
            this.label4.TabIndex = 39;
            this.label4.Text = "조회 범위";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(201, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 38;
            this.label3.Text = "조회 기간";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.Location = new System.Drawing.Point(23, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(170, 24);
            this.lblTitle.TabIndex = 37;
            this.lblTitle.Text = "방범 탐지 분석";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(28, 73);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(373, 12);
            this.lblDescription.TabIndex = 36;
            this.lblDescription.Text = "작동 빈도가 높은 센서들부터 왼쪽에서 오른쪽 방향으로 표시합니다.";
            // 
            // cboPageIndexSensor
            // 
            this.cboPageIndexSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPageIndexSensor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboPageIndexSensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageIndexSensor.FormattingEnabled = true;
            this.cboPageIndexSensor.Location = new System.Drawing.Point(749, 69);
            this.cboPageIndexSensor.MaxDropDownItems = 20;
            this.cboPageIndexSensor.Name = "cboPageIndexSensor";
            this.cboPageIndexSensor.Size = new System.Drawing.Size(70, 22);
            this.cboPageIndexSensor.TabIndex = 56;
            this.cboPageIndexSensor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cboPageIndex_DrawItem);
            this.cboPageIndexSensor.SelectedIndexChanged += new System.EventHandler(this.cboPageIndex_SelectedIndexChanged);
            // 
            // btnNextIndexSensor
            // 
            this.btnNextIndexSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextIndexSensor.Location = new System.Drawing.Point(876, 69);
            this.btnNextIndexSensor.Name = "btnNextIndexSensor";
            this.btnNextIndexSensor.Size = new System.Drawing.Size(20, 20);
            this.btnNextIndexSensor.TabIndex = 55;
            this.btnNextIndexSensor.Text = "▶";
            this.btnNextIndexSensor.UseVisualStyleBackColor = true;
            this.btnNextIndexSensor.Click += new System.EventHandler(this.btnNextIndex_Click);
            // 
            // btnPreviousIndexSensor
            // 
            this.btnPreviousIndexSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreviousIndexSensor.Location = new System.Drawing.Point(723, 69);
            this.btnPreviousIndexSensor.Name = "btnPreviousIndexSensor";
            this.btnPreviousIndexSensor.Size = new System.Drawing.Size(20, 20);
            this.btnPreviousIndexSensor.TabIndex = 54;
            this.btnPreviousIndexSensor.Text = "◀";
            this.btnPreviousIndexSensor.UseVisualStyleBackColor = true;
            this.btnPreviousIndexSensor.Click += new System.EventHandler(this.btnPreviousIndex_Click);
            // 
            // lblTotalPageSensor
            // 
            this.lblTotalPageSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalPageSensor.AutoSize = true;
            this.lblTotalPageSensor.Location = new System.Drawing.Point(825, 73);
            this.lblTotalPageSensor.Name = "lblTotalPageSensor";
            this.lblTotalPageSensor.Size = new System.Drawing.Size(21, 12);
            this.lblTotalPageSensor.TabIndex = 53;
            this.lblTotalPageSensor.Text = "/ 1";
            // 
            // cboChart
            // 
            this.cboChart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChart.FormattingEnabled = true;
            this.cboChart.Items.AddRange(new object[] {
            "센서별 보기",
            "위치별 보기"});
            this.cboChart.Location = new System.Drawing.Point(25, 108);
            this.cboChart.Name = "cboChart";
            this.cboChart.Size = new System.Drawing.Size(121, 20);
            this.cboChart.TabIndex = 57;
            this.cboChart.SelectedIndexChanged += new System.EventHandler(this.cboChart_SelectedIndexChanged);
            // 
            // dataGridViewEquipZone
            // 
            this.dataGridViewEquipZone.AllowUserToAddRows = false;
            this.dataGridViewEquipZone.AllowUserToDeleteRows = false;
            this.dataGridViewEquipZone.AllowUserToResizeRows = false;
            this.dataGridViewEquipZone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewEquipZone.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewEquipZone.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewEquipZone.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewEquipZone.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewEquipZone.Location = new System.Drawing.Point(25, 388);
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
            this.dataGridViewEquipZone.Size = new System.Drawing.Size(858, 115);
            this.dataGridViewEquipZone.TabIndex = 58;
            this.dataGridViewEquipZone.Visible = false;
            // 
            // winChartViewerEquipZone
            // 
            this.winChartViewerEquipZone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.winChartViewerEquipZone.Location = new System.Drawing.Point(152, 108);
            this.winChartViewerEquipZone.Name = "winChartViewerEquipZone";
            this.winChartViewerEquipZone.Size = new System.Drawing.Size(731, 218);
            this.winChartViewerEquipZone.TabIndex = 40;
            this.winChartViewerEquipZone.TabStop = false;
            this.winChartViewerEquipZone.Visible = false;
            // 
            // winChartViewerSensor
            // 
            this.winChartViewerSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.winChartViewerSensor.Location = new System.Drawing.Point(152, 108);
            this.winChartViewerSensor.Name = "winChartViewerSensor";
            this.winChartViewerSensor.Size = new System.Drawing.Size(731, 218);
            this.winChartViewerSensor.TabIndex = 40;
            this.winChartViewerSensor.TabStop = false;
            this.winChartViewerSensor.Visible = false;
            // 
            // lblTotalPageEquipZone
            // 
            this.lblTotalPageEquipZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalPageEquipZone.AutoSize = true;
            this.lblTotalPageEquipZone.Location = new System.Drawing.Point(825, 92);
            this.lblTotalPageEquipZone.Name = "lblTotalPageEquipZone";
            this.lblTotalPageEquipZone.Size = new System.Drawing.Size(21, 12);
            this.lblTotalPageEquipZone.TabIndex = 53;
            this.lblTotalPageEquipZone.Text = "/ 1";
            // 
            // btnPreviousIndexEquipZone
            // 
            this.btnPreviousIndexEquipZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreviousIndexEquipZone.Location = new System.Drawing.Point(723, 88);
            this.btnPreviousIndexEquipZone.Name = "btnPreviousIndexEquipZone";
            this.btnPreviousIndexEquipZone.Size = new System.Drawing.Size(20, 20);
            this.btnPreviousIndexEquipZone.TabIndex = 54;
            this.btnPreviousIndexEquipZone.Text = "◀";
            this.btnPreviousIndexEquipZone.UseVisualStyleBackColor = true;
            this.btnPreviousIndexEquipZone.Click += new System.EventHandler(this.btnPreviousIndex_Click);
            // 
            // btnNextIndexEquipZone
            // 
            this.btnNextIndexEquipZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextIndexEquipZone.Location = new System.Drawing.Point(876, 88);
            this.btnNextIndexEquipZone.Name = "btnNextIndexEquipZone";
            this.btnNextIndexEquipZone.Size = new System.Drawing.Size(20, 20);
            this.btnNextIndexEquipZone.TabIndex = 55;
            this.btnNextIndexEquipZone.Text = "▶";
            this.btnNextIndexEquipZone.UseVisualStyleBackColor = true;
            this.btnNextIndexEquipZone.Click += new System.EventHandler(this.btnNextIndex_Click);
            // 
            // cboPageIndexEquipZone
            // 
            this.cboPageIndexEquipZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPageIndexEquipZone.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboPageIndexEquipZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageIndexEquipZone.FormattingEnabled = true;
            this.cboPageIndexEquipZone.Location = new System.Drawing.Point(749, 88);
            this.cboPageIndexEquipZone.MaxDropDownItems = 20;
            this.cboPageIndexEquipZone.Name = "cboPageIndexEquipZone";
            this.cboPageIndexEquipZone.Size = new System.Drawing.Size(70, 22);
            this.cboPageIndexEquipZone.TabIndex = 56;
            this.cboPageIndexEquipZone.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cboPageIndex_DrawItem);
            this.cboPageIndexEquipZone.SelectedIndexChanged += new System.EventHandler(this.cboPageIndex_SelectedIndexChanged);
            // 
            // ParetoIntrusionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 515);
            this.Controls.Add(this.dataGridViewEquipZone);
            this.Controls.Add(this.cboChart);
            this.Controls.Add(this.cboPageIndexEquipZone);
            this.Controls.Add(this.cboPageIndexSensor);
            this.Controls.Add(this.btnNextIndexEquipZone);
            this.Controls.Add(this.btnPreviousIndexEquipZone);
            this.Controls.Add(this.btnNextIndexSensor);
            this.Controls.Add(this.lblTotalPageEquipZone);
            this.Controls.Add(this.btnPreviousIndexSensor);
            this.Controls.Add(this.lblTotalPageSensor);
            this.Controls.Add(this.btnSaveHWP);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ParetoIntrusionPage";
            this.Text = "ParetoPage";
            this.Load += new System.EventHandler(this.ParetoIntrusionPage_Load);
            this.Resize += new System.EventHandler(this.ParetoIntrusionPage_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSensor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEquipZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerEquipZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.winChartViewerSensor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSaveHWP;
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
        private System.Windows.Forms.ComboBox cboPageIndexSensor;
        private System.Windows.Forms.Button btnNextIndexSensor;
        private System.Windows.Forms.Button btnPreviousIndexSensor;
        private System.Windows.Forms.Label lblTotalPageSensor;
        private System.Windows.Forms.ComboBox cboChart;
        private ChartDirector.WinChartViewer winChartViewerEquipZone;
        private System.Windows.Forms.DataGridView dataGridViewEquipZone;
        private System.Windows.Forms.Label lblTotalPageEquipZone;
        private System.Windows.Forms.Button btnPreviousIndexEquipZone;
        private System.Windows.Forms.Button btnNextIndexEquipZone;
        private System.Windows.Forms.ComboBox cboPageIndexEquipZone;
    }
}