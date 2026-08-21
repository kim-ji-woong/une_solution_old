namespace SDMS
{
    partial class FormManager
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
			this.cboSensorType = new System.Windows.Forms.ComboBox();
			this.labelDescription = new System.Windows.Forms.Label();
			this.checkBoxBuilding = new System.Windows.Forms.CheckBox();
			this.cboBuilding = new System.Windows.Forms.ComboBox();
			this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
			this.gridManager = new System.Windows.Forms.DataGridView();
			this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.btnEdit = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.cmbEquipZone = new System.Windows.Forms.ComboBox();
			this.cmbFloor = new System.Windows.Forms.ComboBox();
			this.cmbType = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// cboSensorType
			// 
			this.cboSensorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboSensorType.FormattingEnabled = true;
			this.cboSensorType.Items.AddRange(new object[] {
            "자탐센서",
            "CCTV",
            "소방시설물"});
			this.cboSensorType.Location = new System.Drawing.Point(13, 14);
			this.cboSensorType.Name = "cboSensorType";
			this.cboSensorType.Size = new System.Drawing.Size(121, 20);
			this.cboSensorType.TabIndex = 0;
			this.cboSensorType.SelectedIndexChanged += new System.EventHandler(this.cboSensorType_SelectedIndexChanged);
			// 
			// labelDescription
			// 
			this.labelDescription.AutoSize = true;
			this.labelDescription.Location = new System.Drawing.Point(162, 18);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(29, 12);
			this.labelDescription.TabIndex = 1;
			this.labelDescription.Text = "설명";
			// 
			// checkBoxBuilding
			// 
			this.checkBoxBuilding.AutoSize = true;
			this.checkBoxBuilding.Location = new System.Drawing.Point(261, 14);
			this.checkBoxBuilding.Name = "checkBoxBuilding";
			this.checkBoxBuilding.Size = new System.Drawing.Size(88, 16);
			this.checkBoxBuilding.TabIndex = 2;
			this.checkBoxBuilding.Text = "건물별 보기";
			this.checkBoxBuilding.UseVisualStyleBackColor = true;
			this.checkBoxBuilding.Visible = false;
			// 
			// cboBuilding
			// 
			this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboBuilding.FormattingEnabled = true;
			this.cboBuilding.Location = new System.Drawing.Point(285, 45);
			this.cboBuilding.Name = "cboBuilding";
			this.cboBuilding.Size = new System.Drawing.Size(248, 20);
			this.cboBuilding.TabIndex = 9;
			this.cboBuilding.Visible = false;
			this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
			// 
			// cboBuildingGroup
			// 
			this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboBuildingGroup.FormattingEnabled = true;
			this.cboBuildingGroup.Location = new System.Drawing.Point(140, 45);
			this.cboBuildingGroup.Name = "cboBuildingGroup";
			this.cboBuildingGroup.Size = new System.Drawing.Size(139, 20);
			this.cboBuildingGroup.TabIndex = 8;
			this.cboBuildingGroup.Visible = false;
			this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
			// 
			// gridManager
			// 
			this.gridManager.AllowUserToAddRows = false;
			this.gridManager.AllowUserToDeleteRows = false;
			this.gridManager.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridManager.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colName,
            this.colTeam});
			this.gridManager.Location = new System.Drawing.Point(13, 76);
			this.gridManager.Name = "gridManager";
			this.gridManager.ReadOnly = true;
			this.gridManager.RowHeadersVisible = false;
			this.gridManager.RowTemplate.Height = 23;
			this.gridManager.Size = new System.Drawing.Size(520, 313);
			this.gridManager.TabIndex = 10;
			this.gridManager.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.gridManager_RowsAdded);
			// 
			// colNo
			// 
			this.colNo.HeaderText = "No";
			this.colNo.Name = "colNo";
			this.colNo.ReadOnly = true;
			this.colNo.Width = 30;
			// 
			// colName
			// 
			this.colName.HeaderText = "이름";
			this.colName.Name = "colName";
			this.colName.ReadOnly = true;
			this.colName.Width = 150;
			// 
			// colTeam
			// 
			this.colTeam.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colTeam.HeaderText = "비고";
			this.colTeam.Name = "colTeam";
			this.colTeam.ReadOnly = true;
			// 
			// btnEdit
			// 
			this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnEdit.Location = new System.Drawing.Point(472, 11);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(61, 23);
			this.btnEdit.TabIndex = 11;
			this.btnEdit.Text = "편집";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new System.Drawing.Point(10, 10);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(546, 47);
			this.panel1.TabIndex = 12;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label1.Location = new System.Drawing.Point(20, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "담당자 관리";
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.White;
			this.panel2.Controls.Add(this.cmbEquipZone);
			this.panel2.Controls.Add(this.cmbFloor);
			this.panel2.Controls.Add(this.cmbType);
			this.panel2.Controls.Add(this.cboSensorType);
			this.panel2.Controls.Add(this.labelDescription);
			this.panel2.Controls.Add(this.gridManager);
			this.panel2.Controls.Add(this.btnEdit);
			this.panel2.Controls.Add(this.cboBuilding);
			this.panel2.Controls.Add(this.checkBoxBuilding);
			this.panel2.Controls.Add(this.cboBuildingGroup);
			this.panel2.Location = new System.Drawing.Point(10, 70);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(546, 401);
			this.panel2.TabIndex = 12;
			// 
			// cmbEquipZone
			// 
			this.cmbEquipZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbEquipZone.FormattingEnabled = true;
			this.cmbEquipZone.Location = new System.Drawing.Point(140, 76);
			this.cmbEquipZone.Name = "cmbEquipZone";
			this.cmbEquipZone.Size = new System.Drawing.Size(393, 20);
			this.cmbEquipZone.TabIndex = 14;
			this.cmbEquipZone.Visible = false;
			this.cmbEquipZone.SelectedIndexChanged += new System.EventHandler(this.cmbEquipZone_SelectedIndexChanged);
			// 
			// cmbFloor
			// 
			this.cmbFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbFloor.FormattingEnabled = true;
			this.cmbFloor.Location = new System.Drawing.Point(13, 76);
			this.cmbFloor.Name = "cmbFloor";
			this.cmbFloor.Size = new System.Drawing.Size(121, 20);
			this.cmbFloor.TabIndex = 13;
			this.cmbFloor.Visible = false;
			this.cmbFloor.SelectedIndexChanged += new System.EventHandler(this.cmbFloor_SelectedIndexChanged);
			// 
			// cmbType
			// 
			this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbType.FormattingEnabled = true;
			this.cmbType.Items.AddRange(new object[] {
            "시설물별 보기",
            "건물별 보기",
            "화재구역별 보기"});
			this.cmbType.Location = new System.Drawing.Point(13, 45);
			this.cmbType.Name = "cmbType";
			this.cmbType.Size = new System.Drawing.Size(121, 20);
			this.cmbType.TabIndex = 12;
			this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
			// 
			// FormManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.LightGray;
			this.ClientSize = new System.Drawing.Size(566, 481);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormManager";
			this.Text = "담당자 관리";
			this.Load += new System.EventHandler(this.FormManager_Load);
			((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboSensorType;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.CheckBox checkBoxBuilding;
        private System.Windows.Forms.ComboBox cboBuilding;
        private System.Windows.Forms.ComboBox cboBuildingGroup;
        private System.Windows.Forms.DataGridView gridManager;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeam;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ComboBox cmbType;
		private System.Windows.Forms.ComboBox cmbEquipZone;
		private System.Windows.Forms.ComboBox cmbFloor;
    }
}