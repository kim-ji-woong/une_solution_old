namespace SDMS
{
    partial class FormSensorList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSelectZone = new System.Windows.Forms.Button();
            this.cboFloor = new System.Windows.Forms.ComboBox();
            this.cboBuilding = new System.Windows.Forms.ComboBox();
            this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
            this.lblSelectZone = new System.Windows.Forms.Label();
            this.cboSensorType = new System.Windows.Forms.ComboBox();
            this.lblFacilityType = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.lblOperationStatus = new System.Windows.Forms.Label();
            this.gvSensorList = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboDisasterPreventionEquipmentLocation = new System.Windows.Forms.ComboBox();
            this.lblFacilityName = new System.Windows.Forms.Label();
            this.cboPSMSensorStatus = new System.Windows.Forms.ComboBox();
            this.cboPSMSensorLocations = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gvDisasterPreventionEquipment = new System.Windows.Forms.DataGridView();
            this.colDisasterPreventionEquipmentNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisasterPreventionEquipmentType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colDisasterPreventionEquipmentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisasterPreventionEquipmentLocation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colDisasterPreventionEquipmentQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisasterPreventionEquipmentDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sensorListGridDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBuilding = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFloor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colETC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gvSensorList)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvDisasterPreventionEquipment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sensorListGridDataBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectZone
            // 
            this.btnSelectZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectZone.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectZone.Location = new System.Drawing.Point(841, 17);
            this.btnSelectZone.Name = "btnSelectZone";
            this.btnSelectZone.Size = new System.Drawing.Size(67, 55);
            this.btnSelectZone.TabIndex = 8;
            this.btnSelectZone.Text = "선택";
            this.btnSelectZone.UseVisualStyleBackColor = true;
            this.btnSelectZone.Click += new System.EventHandler(this.btnSelectZone_Click);
            // 
            // cboFloor
            // 
            this.cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloor.FormattingEnabled = true;
            this.cboFloor.Location = new System.Drawing.Point(751, 52);
            this.cboFloor.Name = "cboFloor";
            this.cboFloor.Size = new System.Drawing.Size(75, 20);
            this.cboFloor.TabIndex = 7;
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.Location = new System.Drawing.Point(435, 52);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Size = new System.Drawing.Size(309, 20);
            this.cboBuilding.TabIndex = 4;
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.Location = new System.Drawing.Point(291, 52);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Size = new System.Drawing.Size(139, 20);
            this.cboBuildingGroup.TabIndex = 3;
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // lblSelectZone
            // 
            this.lblSelectZone.AutoSize = true;
            this.lblSelectZone.Location = new System.Drawing.Point(232, 57);
            this.lblSelectZone.Name = "lblSelectZone";
            this.lblSelectZone.Size = new System.Drawing.Size(53, 12);
            this.lblSelectZone.TabIndex = 5;
            this.lblSelectZone.Text = "범위선택";
            // 
            // cboSensorType
            // 
            this.cboSensorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSensorType.FormattingEnabled = true;
            this.cboSensorType.Location = new System.Drawing.Point(291, 18);
            this.cboSensorType.Name = "cboSensorType";
            this.cboSensorType.Size = new System.Drawing.Size(139, 20);
            this.cboSensorType.TabIndex = 0;
            this.cboSensorType.SelectedIndexChanged += new System.EventHandler(this.cboSensorType_SelectedIndexChanged);
            // 
            // lblFacilityType
            // 
            this.lblFacilityType.AutoSize = true;
            this.lblFacilityType.Location = new System.Drawing.Point(232, 22);
            this.lblFacilityType.Name = "lblFacilityType";
            this.lblFacilityType.Size = new System.Drawing.Size(53, 12);
            this.lblFacilityType.TabIndex = 5;
            this.lblFacilityType.Text = "유형선택";
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Location = new System.Drawing.Point(751, 18);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(76, 20);
            this.cboStatus.TabIndex = 1;
            // 
            // lblOperationStatus
            // 
            this.lblOperationStatus.AutoSize = true;
            this.lblOperationStatus.Location = new System.Drawing.Point(691, 22);
            this.lblOperationStatus.Name = "lblOperationStatus";
            this.lblOperationStatus.Size = new System.Drawing.Size(53, 12);
            this.lblOperationStatus.TabIndex = 5;
            this.lblOperationStatus.Text = "운영상태";
            // 
            // gvSensorList
            // 
            this.gvSensorList.AllowUserToAddRows = false;
            this.gvSensorList.AllowUserToDeleteRows = false;
            this.gvSensorList.AutoGenerateColumns = false;
            this.gvSensorList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvSensorList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colType,
            this.colName,
            this.colStatus,
            this.colBuilding,
            this.colFloor,
            this.colETC});
            this.gvSensorList.DataSource = this.sensorListGridDataBindingSource;
            this.gvSensorList.Location = new System.Drawing.Point(11, 13);
            this.gvSensorList.Name = "gvSensorList";
            this.gvSensorList.RowHeadersVisible = false;
            this.gvSensorList.RowTemplate.Height = 23;
            this.gvSensorList.Size = new System.Drawing.Size(897, 404);
            this.gvSensorList.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.cboDisasterPreventionEquipmentLocation);
            this.panel1.Controls.Add(this.lblFacilityName);
            this.panel1.Controls.Add(this.btnSelectZone);
            this.panel1.Controls.Add(this.cboFloor);
            this.panel1.Controls.Add(this.lblOperationStatus);
            this.panel1.Controls.Add(this.lblFacilityType);
            this.panel1.Controls.Add(this.cboPSMSensorStatus);
            this.panel1.Controls.Add(this.cboStatus);
            this.panel1.Controls.Add(this.lblSelectZone);
            this.panel1.Controls.Add(this.cboSensorType);
            this.panel1.Controls.Add(this.cboBuilding);
            this.panel1.Controls.Add(this.cboPSMSensorLocations);
            this.panel1.Controls.Add(this.cboBuildingGroup);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(922, 88);
            this.panel1.TabIndex = 0;
            // 
            // cboDisasterPreventionEquipmentLocation
            // 
            this.cboDisasterPreventionEquipmentLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDisasterPreventionEquipmentLocation.FormattingEnabled = true;
            this.cboDisasterPreventionEquipmentLocation.Location = new System.Drawing.Point(459, 27);
            this.cboDisasterPreventionEquipmentLocation.Name = "cboDisasterPreventionEquipmentLocation";
            this.cboDisasterPreventionEquipmentLocation.Size = new System.Drawing.Size(203, 20);
            this.cboDisasterPreventionEquipmentLocation.TabIndex = 6;
            this.cboDisasterPreventionEquipmentLocation.Visible = false;
            // 
            // lblFacilityName
            // 
            this.lblFacilityName.AutoSize = true;
            this.lblFacilityName.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblFacilityName.Location = new System.Drawing.Point(20, 39);
            this.lblFacilityName.Name = "lblFacilityName";
            this.lblFacilityName.Size = new System.Drawing.Size(82, 16);
            this.lblFacilityName.TabIndex = 10;
            this.lblFacilityName.Text = "모든 설비";
            // 
            // cboPSMSensorStatus
            // 
            this.cboPSMSensorStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPSMSensorStatus.FormattingEnabled = true;
            this.cboPSMSensorStatus.Location = new System.Drawing.Point(751, 27);
            this.cboPSMSensorStatus.Name = "cboPSMSensorStatus";
            this.cboPSMSensorStatus.Size = new System.Drawing.Size(76, 20);
            this.cboPSMSensorStatus.TabIndex = 2;
            this.cboPSMSensorStatus.Visible = false;
            // 
            // cboPSMSensorLocations
            // 
            this.cboPSMSensorLocations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPSMSensorLocations.FormattingEnabled = true;
            this.cboPSMSensorLocations.Location = new System.Drawing.Point(459, 14);
            this.cboPSMSensorLocations.Name = "cboPSMSensorLocations";
            this.cboPSMSensorLocations.Size = new System.Drawing.Size(203, 20);
            this.cboPSMSensorLocations.TabIndex = 5;
            this.cboPSMSensorLocations.Visible = false;
            this.cboPSMSensorLocations.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.gvDisasterPreventionEquipment);
            this.panel2.Controls.Add(this.gvSensorList);
            this.panel2.Location = new System.Drawing.Point(12, 113);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(922, 430);
            this.panel2.TabIndex = 1;
            // 
            // gvDisasterPreventionEquipment
            // 
            this.gvDisasterPreventionEquipment.AllowUserToAddRows = false;
            this.gvDisasterPreventionEquipment.AllowUserToDeleteRows = false;
            this.gvDisasterPreventionEquipment.AllowUserToResizeRows = false;
            this.gvDisasterPreventionEquipment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvDisasterPreventionEquipment.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDisasterPreventionEquipmentNo,
            this.colDisasterPreventionEquipmentType,
            this.colDisasterPreventionEquipmentName,
            this.colDisasterPreventionEquipmentLocation,
            this.colDisasterPreventionEquipmentQuantity,
            this.colDisasterPreventionEquipmentDescription});
            this.gvDisasterPreventionEquipment.Location = new System.Drawing.Point(59, 202);
            this.gvDisasterPreventionEquipment.Name = "gvDisasterPreventionEquipment";
            this.gvDisasterPreventionEquipment.RowHeadersVisible = false;
            this.gvDisasterPreventionEquipment.RowTemplate.Height = 23;
            this.gvDisasterPreventionEquipment.Size = new System.Drawing.Size(803, 180);
            this.gvDisasterPreventionEquipment.TabIndex = 1;
            this.gvDisasterPreventionEquipment.Visible = false;
            this.gvDisasterPreventionEquipment.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvDisasterPreventionEquipment_CellClick);
            this.gvDisasterPreventionEquipment.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvDisasterPreventionEquipment_CellEndEdit);
            this.gvDisasterPreventionEquipment.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gvDisasterPreventionEquipment_CellFormatting);
            this.gvDisasterPreventionEquipment.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvDisasterPreventionEquipment_CellValidated);
            this.gvDisasterPreventionEquipment.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.gvDisasterPreventionEquipment_CellValidating);
            this.gvDisasterPreventionEquipment.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.gvDisasterPreventionEquipment_RowsAdded);
            this.gvDisasterPreventionEquipment.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gvDisasterPreventionEquipment_KeyDown);
            // 
            // colDisasterPreventionEquipmentNo
            // 
            this.colDisasterPreventionEquipmentNo.HeaderText = "No";
            this.colDisasterPreventionEquipmentNo.Name = "colDisasterPreventionEquipmentNo";
            this.colDisasterPreventionEquipmentNo.ReadOnly = true;
            this.colDisasterPreventionEquipmentNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDisasterPreventionEquipmentNo.Width = 30;
            // 
            // colDisasterPreventionEquipmentType
            // 
            this.colDisasterPreventionEquipmentType.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colDisasterPreventionEquipmentType.HeaderText = "유형";
            this.colDisasterPreventionEquipmentType.Name = "colDisasterPreventionEquipmentType";
            this.colDisasterPreventionEquipmentType.Width = 120;
            // 
            // colDisasterPreventionEquipmentName
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colDisasterPreventionEquipmentName.DefaultCellStyle = dataGridViewCellStyle2;
            this.colDisasterPreventionEquipmentName.HeaderText = "장비이름";
            this.colDisasterPreventionEquipmentName.Name = "colDisasterPreventionEquipmentName";
            this.colDisasterPreventionEquipmentName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDisasterPreventionEquipmentName.Width = 160;
            // 
            // colDisasterPreventionEquipmentLocation
            // 
            this.colDisasterPreventionEquipmentLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDisasterPreventionEquipmentLocation.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colDisasterPreventionEquipmentLocation.HeaderText = "위치";
            this.colDisasterPreventionEquipmentLocation.Name = "colDisasterPreventionEquipmentLocation";
            // 
            // colDisasterPreventionEquipmentQuantity
            // 
            dataGridViewCellStyle3.Format = "N0";
            dataGridViewCellStyle3.NullValue = "0";
            this.colDisasterPreventionEquipmentQuantity.DefaultCellStyle = dataGridViewCellStyle3;
            this.colDisasterPreventionEquipmentQuantity.HeaderText = "수량";
            this.colDisasterPreventionEquipmentQuantity.Name = "colDisasterPreventionEquipmentQuantity";
            this.colDisasterPreventionEquipmentQuantity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDisasterPreventionEquipmentQuantity.Width = 80;
            // 
            // colDisasterPreventionEquipmentDescription
            // 
            this.colDisasterPreventionEquipmentDescription.HeaderText = "비고";
            this.colDisasterPreventionEquipmentDescription.Name = "colDisasterPreventionEquipmentDescription";
            this.colDisasterPreventionEquipmentDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDisasterPreventionEquipmentDescription.Width = 180;
            // 
            // sensorListGridDataBindingSource
            // 
            this.sensorListGridDataBindingSource.DataSource = typeof(SDMS.Admin.SensorListGridData);
            // 
            // colNo
            // 
            this.colNo.DataPropertyName = "No";
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 30;
            // 
            // colType
            // 
            this.colType.DataPropertyName = "Type";
            this.colType.HeaderText = "유형";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // colName
            // 
            this.colName.DataPropertyName = "Name";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colName.DefaultCellStyle = dataGridViewCellStyle1;
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 200;
            // 
            // colStatus
            // 
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "운영상태";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // colBuilding
            // 
            this.colBuilding.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colBuilding.DataPropertyName = "Building";
            this.colBuilding.HeaderText = "건물";
            this.colBuilding.Name = "colBuilding";
            this.colBuilding.ReadOnly = true;
            // 
            // colFloor
            // 
            this.colFloor.DataPropertyName = "Floor";
            this.colFloor.HeaderText = "층";
            this.colFloor.Name = "colFloor";
            this.colFloor.ReadOnly = true;
            this.colFloor.Width = 60;
            // 
            // colETC
            // 
            this.colETC.DataPropertyName = "Description";
            this.colETC.HeaderText = "설치장소";
            this.colETC.Name = "colETC";
            this.colETC.ReadOnly = true;
            this.colETC.Width = 200;
            // 
            // FormSensorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(946, 555);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSensorList";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "센서구역/CCTV/소방시설 리스트";
            this.Load += new System.EventHandler(this.FormSensorList_Load);
            this.VisibleChanged += new System.EventHandler(this.FormSensorList_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.gvSensorList)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvDisasterPreventionEquipment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sensorListGridDataBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelectZone;
        private System.Windows.Forms.ComboBox cboFloor;
        private System.Windows.Forms.ComboBox cboBuilding;
        private System.Windows.Forms.ComboBox cboBuildingGroup;
        private System.Windows.Forms.Label lblSelectZone;
        private System.Windows.Forms.ComboBox cboSensorType;
        private System.Windows.Forms.Label lblFacilityType;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Label lblOperationStatus;
        private System.Windows.Forms.DataGridView gvSensorList;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cboPSMSensorLocations;
        private System.Windows.Forms.ComboBox cboPSMSensorStatus;
        private System.Windows.Forms.Label lblFacilityName;
        private System.Windows.Forms.ComboBox cboDisasterPreventionEquipmentLocation;
        private System.Windows.Forms.DataGridView gvDisasterPreventionEquipment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisasterPreventionEquipmentNo;
        private System.Windows.Forms.DataGridViewComboBoxColumn colDisasterPreventionEquipmentType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisasterPreventionEquipmentName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colDisasterPreventionEquipmentLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisasterPreventionEquipmentQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisasterPreventionEquipmentDescription;
        private System.Windows.Forms.BindingSource sensorListGridDataBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBuilding;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFloor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colETC;

    }
}