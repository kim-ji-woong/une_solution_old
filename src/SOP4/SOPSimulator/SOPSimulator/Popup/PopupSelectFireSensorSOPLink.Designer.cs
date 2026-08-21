namespace SOPMonitoringSystem.Popup
{
    partial class PopupSelectFireSensorSOPLink
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gridSOP = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSOPFullPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnNewSOP = new System.Windows.Forms.Button();
            this.btnChangeSOP = new System.Windows.Forms.Button();
            this.btnDeleteSOP = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageBuildingSignal = new System.Windows.Forms.TabPage();
            this.gridBuilding = new System.Windows.Forms.DataGridView();
            this.colBuildingID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBuilding = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLinkedSOP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridBuildingGroup = new System.Windows.Forms.DataGridView();
            this.colBuildingGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageZoneSignal = new System.Windows.Forms.TabPage();
            this.gridZone = new System.Windows.Forms.DataGridView();
            this.colZoneID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZoneName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridBuilding2 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridBuildingGroup2 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.panel7.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSOP)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageBuildingSignal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridBuilding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBuildingGroup)).BeginInit();
            this.tabPageZoneSignal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBuilding2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBuildingGroup2)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.Controls.Add(this.label15);
            this.panel7.Location = new System.Drawing.Point(12, 12);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(1127, 47);
            this.panel7.TabIndex = 4;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label15.Location = new System.Drawing.Point(20, 15);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(214, 16);
            this.label15.TabIndex = 1;
            this.label15.Text = "화재탐지 신호별 SOP 설정";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.tabControl2);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Location = new System.Drawing.Point(12, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1127, 437);
            this.panel1.TabIndex = 4;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Location = new System.Drawing.Point(748, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(376, 394);
            this.tabControl2.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gridSOP);
            this.tabPage1.Controls.Add(this.btnNewSOP);
            this.tabPage1.Controls.Add(this.btnChangeSOP);
            this.tabPage1.Controls.Add(this.btnDeleteSOP);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(368, 368);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "SOP 목록";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gridSOP
            // 
            this.gridSOP.AllowUserToAddRows = false;
            this.gridSOP.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSOP.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colSOPFullPath});
            this.gridSOP.Location = new System.Drawing.Point(6, 14);
            this.gridSOP.MultiSelect = false;
            this.gridSOP.Name = "gridSOP";
            this.gridSOP.RowHeadersVisible = false;
            this.gridSOP.RowTemplate.Height = 23;
            this.gridSOP.Size = new System.Drawing.Size(356, 316);
            this.gridSOP.TabIndex = 0;
            this.gridSOP.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridSOP_CellMouseClick);
            // 
            // colNo
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle15;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.Width = 40;
            // 
            // colSOPFullPath
            // 
            this.colSOPFullPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colSOPFullPath.DefaultCellStyle = dataGridViewCellStyle16;
            this.colSOPFullPath.HeaderText = "SOP";
            this.colSOPFullPath.Name = "colSOPFullPath";
            // 
            // btnNewSOP
            // 
            this.btnNewSOP.Location = new System.Drawing.Point(6, 340);
            this.btnNewSOP.Name = "btnNewSOP";
            this.btnNewSOP.Size = new System.Drawing.Size(75, 23);
            this.btnNewSOP.TabIndex = 1;
            this.btnNewSOP.Text = "SOP 추가";
            this.btnNewSOP.UseVisualStyleBackColor = true;
            this.btnNewSOP.Click += new System.EventHandler(this.btnNewSOP_Click);
            // 
            // btnChangeSOP
            // 
            this.btnChangeSOP.Location = new System.Drawing.Point(87, 340);
            this.btnChangeSOP.Name = "btnChangeSOP";
            this.btnChangeSOP.Size = new System.Drawing.Size(75, 23);
            this.btnChangeSOP.TabIndex = 1;
            this.btnChangeSOP.Text = "SOP 변경";
            this.btnChangeSOP.UseVisualStyleBackColor = true;
            this.btnChangeSOP.Click += new System.EventHandler(this.btnChangeSOP_Click);
            // 
            // btnDeleteSOP
            // 
            this.btnDeleteSOP.Location = new System.Drawing.Point(168, 340);
            this.btnDeleteSOP.Name = "btnDeleteSOP";
            this.btnDeleteSOP.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteSOP.TabIndex = 1;
            this.btnDeleteSOP.Text = "SOP 삭제";
            this.btnDeleteSOP.UseVisualStyleBackColor = true;
            this.btnDeleteSOP.Click += new System.EventHandler(this.btnDeleteSOP_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(1045, 403);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(964, 403);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "저장";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageBuildingSignal);
            this.tabControl1.Controls.Add(this.tabPageZoneSignal);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(742, 394);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageBuildingSignal
            // 
            this.tabPageBuildingSignal.Controls.Add(this.gridBuilding);
            this.tabPageBuildingSignal.Controls.Add(this.gridBuildingGroup);
            this.tabPageBuildingSignal.Location = new System.Drawing.Point(4, 22);
            this.tabPageBuildingSignal.Name = "tabPageBuildingSignal";
            this.tabPageBuildingSignal.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBuildingSignal.Size = new System.Drawing.Size(734, 368);
            this.tabPageBuildingSignal.TabIndex = 1;
            this.tabPageBuildingSignal.Text = "화재신호(건물)";
            this.tabPageBuildingSignal.UseVisualStyleBackColor = true;
            // 
            // gridBuilding
            // 
            this.gridBuilding.AllowUserToAddRows = false;
            this.gridBuilding.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridBuilding.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBuildingID,
            this.colBuilding,
            this.colLinkedSOP});
            this.gridBuilding.Location = new System.Drawing.Point(162, 14);
            this.gridBuilding.Name = "gridBuilding";
            this.gridBuilding.RowHeadersVisible = false;
            this.gridBuilding.RowTemplate.Height = 23;
            this.gridBuilding.Size = new System.Drawing.Size(562, 316);
            this.gridBuilding.TabIndex = 0;
            this.gridBuilding.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellEndEdit);
            this.gridBuilding.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseClick);
            // 
            // colBuildingID
            // 
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colBuildingID.DefaultCellStyle = dataGridViewCellStyle17;
            this.colBuildingID.HeaderText = "ID";
            this.colBuildingID.Name = "colBuildingID";
            this.colBuildingID.Width = 40;
            // 
            // colBuilding
            // 
            this.colBuilding.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colBuilding.DefaultCellStyle = dataGridViewCellStyle18;
            this.colBuilding.HeaderText = "건물";
            this.colBuilding.Name = "colBuilding";
            // 
            // colLinkedSOP
            // 
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colLinkedSOP.DefaultCellStyle = dataGridViewCellStyle19;
            this.colLinkedSOP.HeaderText = "SOP";
            this.colLinkedSOP.Name = "colLinkedSOP";
            this.colLinkedSOP.Width = 60;
            // 
            // gridBuildingGroup
            // 
            this.gridBuildingGroup.AllowUserToAddRows = false;
            this.gridBuildingGroup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridBuildingGroup.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBuildingGroup});
            this.gridBuildingGroup.Location = new System.Drawing.Point(7, 14);
            this.gridBuildingGroup.Name = "gridBuildingGroup";
            this.gridBuildingGroup.RowHeadersVisible = false;
            this.gridBuildingGroup.RowTemplate.Height = 23;
            this.gridBuildingGroup.Size = new System.Drawing.Size(149, 316);
            this.gridBuildingGroup.TabIndex = 0;
            this.gridBuildingGroup.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseClick);
            this.gridBuildingGroup.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
            // 
            // colBuildingGroup
            // 
            this.colBuildingGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle12.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colBuildingGroup.DefaultCellStyle = dataGridViewCellStyle12;
            this.colBuildingGroup.HeaderText = "건물그룹";
            this.colBuildingGroup.Name = "colBuildingGroup";
            // 
            // tabPageZoneSignal
            // 
            this.tabPageZoneSignal.Controls.Add(this.gridZone);
            this.tabPageZoneSignal.Controls.Add(this.gridBuilding2);
            this.tabPageZoneSignal.Controls.Add(this.gridBuildingGroup2);
            this.tabPageZoneSignal.Location = new System.Drawing.Point(4, 22);
            this.tabPageZoneSignal.Name = "tabPageZoneSignal";
            this.tabPageZoneSignal.Size = new System.Drawing.Size(734, 368);
            this.tabPageZoneSignal.TabIndex = 2;
            this.tabPageZoneSignal.Text = "화재신호(영역)";
            this.tabPageZoneSignal.UseVisualStyleBackColor = true;
            // 
            // gridZone
            // 
            this.gridZone.AllowUserToAddRows = false;
            this.gridZone.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridZone.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colZoneID,
            this.colZoneName,
            this.dataGridViewTextBoxColumn1});
            this.gridZone.Location = new System.Drawing.Point(364, 14);
            this.gridZone.Name = "gridZone";
            this.gridZone.RowHeadersVisible = false;
            this.gridZone.RowTemplate.Height = 23;
            this.gridZone.Size = new System.Drawing.Size(363, 316);
            this.gridZone.TabIndex = 1;
            this.gridZone.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellEndEdit);
            this.gridZone.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseClick);
            // 
            // colZoneID
            // 
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colZoneID.DefaultCellStyle = dataGridViewCellStyle20;
            this.colZoneID.HeaderText = "ID";
            this.colZoneID.Name = "colZoneID";
            this.colZoneID.Width = 40;
            // 
            // colZoneName
            // 
            this.colZoneName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle21.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colZoneName.DefaultCellStyle = dataGridViewCellStyle21;
            this.colZoneName.HeaderText = "층 또는 영역";
            this.colZoneName.Name = "colZoneName";
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle22;
            this.dataGridViewTextBoxColumn1.HeaderText = "SOP";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 60;
            // 
            // gridBuilding2
            // 
            this.gridBuilding2.AllowUserToAddRows = false;
            this.gridBuilding2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridBuilding2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2});
            this.gridBuilding2.Location = new System.Drawing.Point(148, 14);
            this.gridBuilding2.Name = "gridBuilding2";
            this.gridBuilding2.RowHeadersVisible = false;
            this.gridBuilding2.RowTemplate.Height = 23;
            this.gridBuilding2.Size = new System.Drawing.Size(210, 316);
            this.gridBuilding2.TabIndex = 1;
            this.gridBuilding2.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseClick);
            this.gridBuilding2.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle13;
            this.dataGridViewTextBoxColumn2.HeaderText = "건물";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // gridBuildingGroup2
            // 
            this.gridBuildingGroup2.AllowUserToAddRows = false;
            this.gridBuildingGroup2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridBuildingGroup2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn4});
            this.gridBuildingGroup2.Location = new System.Drawing.Point(7, 14);
            this.gridBuildingGroup2.Name = "gridBuildingGroup2";
            this.gridBuildingGroup2.RowHeadersVisible = false;
            this.gridBuildingGroup2.RowTemplate.Height = 23;
            this.gridBuildingGroup2.Size = new System.Drawing.Size(134, 316);
            this.gridBuildingGroup2.TabIndex = 2;
            this.gridBuildingGroup2.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseClick);
            this.gridBuildingGroup2.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle14.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle14;
            this.dataGridViewTextBoxColumn4.HeaderText = "건물그룹";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuSelectAll});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(123, 26);
            // 
            // tsMenuSelectAll
            // 
            this.tsMenuSelectAll.Name = "tsMenuSelectAll";
            this.tsMenuSelectAll.Size = new System.Drawing.Size(122, 22);
            this.tsMenuSelectAll.Text = "전체선택";
            this.tsMenuSelectAll.Click += new System.EventHandler(this.tsMenuSelectAll_Click);
            // 
            // PopupSelectFireSensorSOPLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1147, 514);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupSelectFireSensorSOPLink";
            this.Text = "PopupSelectFireSensorSOPLink";
            this.Load += new System.EventHandler(this.PopupSelectFireSensorSOPLink_Load);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSOP)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageBuildingSignal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridBuilding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBuildingGroup)).EndInit();
            this.tabPageZoneSignal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBuilding2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBuildingGroup2)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView gridSOP;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSOPFullPath;
        private System.Windows.Forms.Button btnNewSOP;
        private System.Windows.Forms.Button btnDeleteSOP;
        private System.Windows.Forms.Button btnChangeSOP;
        private System.Windows.Forms.TabPage tabPageBuildingSignal;
        private System.Windows.Forms.DataGridView gridBuilding;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBuildingID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBuilding;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLinkedSOP;
        private System.Windows.Forms.DataGridView gridBuildingGroup;
        private System.Windows.Forms.TabPage tabPageZoneSignal;
        private System.Windows.Forms.DataGridView gridZone;
        private System.Windows.Forms.DataGridViewTextBoxColumn colZoneID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colZoneName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridView gridBuilding2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridView gridBuildingGroup2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBuildingGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuSelectAll;
    }
}