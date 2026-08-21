namespace SOPMonitoringSystem.Popup
{
    partial class PopupSelectPSMSensorSOPLink
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.gridLocation = new System.Windows.Forms.DataGridView();
            this.gridMaterial = new System.Windows.Forms.DataGridView();
            this.colMaterial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.colLocationNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLinkedSOP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel7.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSOP)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageBuildingSignal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMaterial)).BeginInit();
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
            this.label15.Size = new System.Drawing.Size(322, 16);
            this.label15.TabIndex = 1;
            this.label15.Text = "유해화학물질 누출탐지 신호별 SOP 설정";
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle1;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.Width = 40;
            // 
            // colSOPFullPath
            // 
            this.colSOPFullPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colSOPFullPath.DefaultCellStyle = dataGridViewCellStyle2;
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
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(742, 394);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageBuildingSignal
            // 
            this.tabPageBuildingSignal.Controls.Add(this.gridLocation);
            this.tabPageBuildingSignal.Controls.Add(this.gridMaterial);
            this.tabPageBuildingSignal.Location = new System.Drawing.Point(4, 22);
            this.tabPageBuildingSignal.Name = "tabPageBuildingSignal";
            this.tabPageBuildingSignal.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBuildingSignal.Size = new System.Drawing.Size(734, 368);
            this.tabPageBuildingSignal.TabIndex = 1;
            this.tabPageBuildingSignal.Text = "누출신호(물질별)";
            this.tabPageBuildingSignal.UseVisualStyleBackColor = true;
            // 
            // gridLocation
            // 
            this.gridLocation.AllowUserToAddRows = false;
            this.gridLocation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLocation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLocationNo,
            this.colLocation,
            this.colLinkedSOP});
            this.gridLocation.Location = new System.Drawing.Point(162, 14);
            this.gridLocation.Name = "gridLocation";
            this.gridLocation.RowHeadersVisible = false;
            this.gridLocation.RowTemplate.Height = 23;
            this.gridLocation.Size = new System.Drawing.Size(562, 316);
            this.gridLocation.TabIndex = 0;
            this.gridLocation.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellEndEdit);
            this.gridLocation.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseClick);
            // 
            // gridMaterial
            // 
            this.gridMaterial.AllowUserToAddRows = false;
            this.gridMaterial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMaterial.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMaterial});
            this.gridMaterial.Location = new System.Drawing.Point(7, 14);
            this.gridMaterial.Name = "gridMaterial";
            this.gridMaterial.RowHeadersVisible = false;
            this.gridMaterial.RowTemplate.Height = 23;
            this.gridMaterial.Size = new System.Drawing.Size(149, 316);
            this.gridMaterial.TabIndex = 0;
            this.gridMaterial.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseClick);
            this.gridMaterial.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
            // 
            // colMaterial
            // 
            this.colMaterial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colMaterial.DefaultCellStyle = dataGridViewCellStyle6;
            this.colMaterial.HeaderText = "물질";
            this.colMaterial.Name = "colMaterial";
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
            // colLocationNo
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colLocationNo.DefaultCellStyle = dataGridViewCellStyle3;
            this.colLocationNo.HeaderText = "No";
            this.colLocationNo.Name = "colLocationNo";
            this.colLocationNo.Width = 40;
            // 
            // colLocation
            // 
            this.colLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colLocation.DefaultCellStyle = dataGridViewCellStyle4;
            this.colLocation.HeaderText = "위치";
            this.colLocation.Name = "colLocation";
            // 
            // colLinkedSOP
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colLinkedSOP.DefaultCellStyle = dataGridViewCellStyle5;
            this.colLinkedSOP.HeaderText = "SOP";
            this.colLinkedSOP.Name = "colLinkedSOP";
            this.colLinkedSOP.Width = 60;
            // 
            // PopupSelectPSMSensorSOPLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1147, 514);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupSelectPSMSensorSOPLink";
            this.Text = "PopupSelectPSMSensorSOPLink";
            this.Load += new System.EventHandler(this.PopupSelectPSMSensorSOPLink_Load);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSOP)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageBuildingSignal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMaterial)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView gridSOP;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSOPFullPath;
        private System.Windows.Forms.Button btnNewSOP;
        private System.Windows.Forms.Button btnDeleteSOP;
        private System.Windows.Forms.Button btnChangeSOP;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuSelectAll;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageBuildingSignal;
        private System.Windows.Forms.DataGridView gridLocation;
        private System.Windows.Forms.DataGridView gridMaterial;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaterial;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocationNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLinkedSOP;
    }
}