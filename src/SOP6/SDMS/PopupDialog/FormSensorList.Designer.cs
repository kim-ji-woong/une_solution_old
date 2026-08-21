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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblSelectZone = new System.Windows.Forms.Label();
            this.lblFacilityType = new System.Windows.Forms.Label();
            this.lblOperationStatus = new System.Windows.Forms.Label();
            this.gvSensorList = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBuilding = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFloor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colETC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.noDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.floorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sensorTypeIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.equipmentZoneDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.zoneDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sensorListGridDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblFacilityName = new System.Windows.Forms.Label();
            this.gvDisasterPreventionEquipment = new System.Windows.Forms.DataGridView();
            this.colDisasterPreventionEquipmentNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisasterPreventionEquipmentType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colDisasterPreventionEquipmentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisasterPreventionEquipmentLocation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colDisasterPreventionEquipmentQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisasterPreventionEquipmentDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSelectZone = new UnE.GUI.ImageButton();
            this.cboSensorType = new UnE.GUI.ImageComboBox();
            this.cboStatus = new UnE.GUI.ImageComboBox();
            this.cboPSMSensorStatus = new UnE.GUI.ImageComboBox();
            this.cboBuildingGroup = new UnE.GUI.ImageComboBox();
            this.cboBuilding = new UnE.GUI.ImageComboBox();
            this.cboFloor = new UnE.GUI.ImageComboBox();
            this.cboPSMSensorLocations = new UnE.GUI.ImageComboBox();
            this.cboDisasterPreventionEquipmentLocation = new UnE.GUI.ImageComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.gvSensorList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sensorListGridDataBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDisasterPreventionEquipment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSelectZone)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSelectZone
            // 
            this.lblSelectZone.AutoSize = true;
            this.lblSelectZone.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectZone.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSelectZone.ForeColor = System.Drawing.Color.White;
            this.lblSelectZone.Location = new System.Drawing.Point(16, 105);
            this.lblSelectZone.Name = "lblSelectZone";
            this.lblSelectZone.Size = new System.Drawing.Size(84, 18);
            this.lblSelectZone.TabIndex = 5;
            this.lblSelectZone.Text = "범위선택";
            // 
            // lblFacilityType
            // 
            this.lblFacilityType.AutoSize = true;
            this.lblFacilityType.BackColor = System.Drawing.Color.Transparent;
            this.lblFacilityType.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblFacilityType.ForeColor = System.Drawing.Color.White;
            this.lblFacilityType.Location = new System.Drawing.Point(16, 46);
            this.lblFacilityType.Name = "lblFacilityType";
            this.lblFacilityType.Size = new System.Drawing.Size(84, 18);
            this.lblFacilityType.TabIndex = 5;
            this.lblFacilityType.Text = "유형선택";
            // 
            // lblOperationStatus
            // 
            this.lblOperationStatus.AutoSize = true;
            this.lblOperationStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblOperationStatus.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblOperationStatus.ForeColor = System.Drawing.Color.White;
            this.lblOperationStatus.Location = new System.Drawing.Point(172, 46);
            this.lblOperationStatus.Name = "lblOperationStatus";
            this.lblOperationStatus.Size = new System.Drawing.Size(84, 18);
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
            this.colETC,
            this.noDataGridViewTextBoxColumn,
            this.typeDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.statusDataGridViewTextBoxColumn,
            this.buildingDataGridViewTextBoxColumn,
            this.floorDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.sensorTypeIDDataGridViewTextBoxColumn,
            this.equipmentZoneDataGridViewTextBoxColumn,
            this.zoneDataGridViewTextBoxColumn});
            this.gvSensorList.DataSource = this.sensorListGridDataBindingSource;
            this.gvSensorList.Location = new System.Drawing.Point(19, 171);
            this.gvSensorList.Name = "gvSensorList";
            this.gvSensorList.RowHeadersVisible = false;
            this.gvSensorList.RowTemplate.Height = 23;
            this.gvSensorList.Size = new System.Drawing.Size(906, 432);
            this.gvSensorList.TabIndex = 0;
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
            // noDataGridViewTextBoxColumn
            // 
            this.noDataGridViewTextBoxColumn.DataPropertyName = "No";
            this.noDataGridViewTextBoxColumn.HeaderText = "No";
            this.noDataGridViewTextBoxColumn.Name = "noDataGridViewTextBoxColumn";
            this.noDataGridViewTextBoxColumn.Visible = false;
            // 
            // typeDataGridViewTextBoxColumn
            // 
            this.typeDataGridViewTextBoxColumn.DataPropertyName = "Type";
            this.typeDataGridViewTextBoxColumn.HeaderText = "Type";
            this.typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
            this.typeDataGridViewTextBoxColumn.Visible = false;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.Visible = false;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.Visible = false;
            // 
            // buildingDataGridViewTextBoxColumn
            // 
            this.buildingDataGridViewTextBoxColumn.DataPropertyName = "Building";
            this.buildingDataGridViewTextBoxColumn.HeaderText = "Building";
            this.buildingDataGridViewTextBoxColumn.Name = "buildingDataGridViewTextBoxColumn";
            this.buildingDataGridViewTextBoxColumn.Visible = false;
            // 
            // floorDataGridViewTextBoxColumn
            // 
            this.floorDataGridViewTextBoxColumn.DataPropertyName = "Floor";
            this.floorDataGridViewTextBoxColumn.HeaderText = "Floor";
            this.floorDataGridViewTextBoxColumn.Name = "floorDataGridViewTextBoxColumn";
            this.floorDataGridViewTextBoxColumn.Visible = false;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.Visible = false;
            // 
            // sensorTypeIDDataGridViewTextBoxColumn
            // 
            this.sensorTypeIDDataGridViewTextBoxColumn.DataPropertyName = "SensorTypeID";
            this.sensorTypeIDDataGridViewTextBoxColumn.HeaderText = "SensorTypeID";
            this.sensorTypeIDDataGridViewTextBoxColumn.Name = "sensorTypeIDDataGridViewTextBoxColumn";
            this.sensorTypeIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // equipmentZoneDataGridViewTextBoxColumn
            // 
            this.equipmentZoneDataGridViewTextBoxColumn.DataPropertyName = "EquipmentZone";
            this.equipmentZoneDataGridViewTextBoxColumn.HeaderText = "EquipmentZone";
            this.equipmentZoneDataGridViewTextBoxColumn.Name = "equipmentZoneDataGridViewTextBoxColumn";
            this.equipmentZoneDataGridViewTextBoxColumn.Visible = false;
            // 
            // zoneDataGridViewTextBoxColumn
            // 
            this.zoneDataGridViewTextBoxColumn.DataPropertyName = "Zone";
            this.zoneDataGridViewTextBoxColumn.HeaderText = "Zone";
            this.zoneDataGridViewTextBoxColumn.Name = "zoneDataGridViewTextBoxColumn";
            this.zoneDataGridViewTextBoxColumn.Visible = false;
            // 
            // sensorListGridDataBindingSource
            // 
            this.sensorListGridDataBindingSource.DataSource = typeof(SDMS.Admin.SensorListGridData);
            // 
            // lblFacilityName
            // 
            this.lblFacilityName.AutoSize = true;
            this.lblFacilityName.BackColor = System.Drawing.Color.Transparent;
            this.lblFacilityName.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblFacilityName.ForeColor = System.Drawing.Color.White;
            this.lblFacilityName.Location = new System.Drawing.Point(651, 71);
            this.lblFacilityName.Name = "lblFacilityName";
            this.lblFacilityName.Size = new System.Drawing.Size(212, 18);
            this.lblFacilityName.TabIndex = 10;
            this.lblFacilityName.Text = "유해화학물질 센서 목록";
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
            this.gvDisasterPreventionEquipment.Location = new System.Drawing.Point(19, 171);
            this.gvDisasterPreventionEquipment.Name = "gvDisasterPreventionEquipment";
            this.gvDisasterPreventionEquipment.RowHeadersVisible = false;
            this.gvDisasterPreventionEquipment.RowTemplate.Height = 23;
            this.gvDisasterPreventionEquipment.Size = new System.Drawing.Size(906, 432);
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
            // btnSelectZone
            // 
            this.btnSelectZone.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectZone.ButtonText = "";
            this.btnSelectZone.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectZone.ImageClicked = global::SDMS.Properties.Resources.Search_124_57_Click;
            this.btnSelectZone.ImageDisabled = null;
            this.btnSelectZone.ImageMouseOver = global::SDMS.Properties.Resources.Search_124_57_Click;
            this.btnSelectZone.ImageNormal = global::SDMS.Properties.Resources.Search_124_57_Default;
            this.btnSelectZone.Location = new System.Drawing.Point(863, 64);
            this.btnSelectZone.Name = "btnSelectZone";
            this.btnSelectZone.Owner = null;
            this.btnSelectZone.Size = new System.Drawing.Size(62, 29);
            this.btnSelectZone.TabIndex = 11;
            this.btnSelectZone.TabStop = false;
            this.btnSelectZone.TextColor = System.Drawing.Color.Black;
            this.btnSelectZone.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectZone.ToolTipText = "";
            this.btnSelectZone.UseToolTip = false;
            this.btnSelectZone.WindowRateWidth = 1F;
            this.btnSelectZone.Click += new System.EventHandler(this.btnSelectZone_Click);
            // 
            // cboSensorType
            // 
            this.cboSensorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSensorType.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboSensorType.FormattingEnabled = true;
            this.cboSensorType.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboSensorType.ImageDisabled = null;
            this.cboSensorType.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboSensorType.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboSensorType.Location = new System.Drawing.Point(19, 71);
            this.cboSensorType.Name = "cboSensorType";
            this.cboSensorType.Owner = null;
            this.cboSensorType.Size = new System.Drawing.Size(146, 25);
            this.cboSensorType.TabIndex = 14;
            this.cboSensorType.TextColor = System.Drawing.Color.Black;
            this.cboSensorType.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboSensorType.SelectedIndexChanged += new System.EventHandler(this.cboSensorType_SelectedIndexChanged);
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboStatus.ImageDisabled = null;
            this.cboStatus.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboStatus.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboStatus.Location = new System.Drawing.Point(175, 71);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Owner = null;
            this.cboStatus.Size = new System.Drawing.Size(133, 25);
            this.cboStatus.TabIndex = 15;
            this.cboStatus.TextColor = System.Drawing.Color.Black;
            this.cboStatus.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // cboPSMSensorStatus
            // 
            this.cboPSMSensorStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPSMSensorStatus.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboPSMSensorStatus.FormattingEnabled = true;
            this.cboPSMSensorStatus.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboPSMSensorStatus.ImageDisabled = null;
            this.cboPSMSensorStatus.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboPSMSensorStatus.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboPSMSensorStatus.Location = new System.Drawing.Point(175, 71);
            this.cboPSMSensorStatus.Name = "cboPSMSensorStatus";
            this.cboPSMSensorStatus.Owner = null;
            this.cboPSMSensorStatus.Size = new System.Drawing.Size(133, 25);
            this.cboPSMSensorStatus.TabIndex = 16;
            this.cboPSMSensorStatus.TextColor = System.Drawing.Color.Black;
            this.cboPSMSensorStatus.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboBuildingGroup.ImageDisabled = null;
            this.cboBuildingGroup.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboBuildingGroup.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboBuildingGroup.Location = new System.Drawing.Point(16, 130);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Owner = null;
            this.cboBuildingGroup.Size = new System.Drawing.Size(146, 25);
            this.cboBuildingGroup.TabIndex = 17;
            this.cboBuildingGroup.TextColor = System.Drawing.Color.Black;
            this.cboBuildingGroup.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboBuilding.ImageDisabled = null;
            this.cboBuilding.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboBuilding.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboBuilding.Location = new System.Drawing.Point(175, 130);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Owner = null;
            this.cboBuilding.Size = new System.Drawing.Size(609, 25);
            this.cboBuilding.TabIndex = 18;
            this.cboBuilding.TextColor = System.Drawing.Color.Black;
            this.cboBuilding.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
            // 
            // cboFloor
            // 
            this.cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloor.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboFloor.FormattingEnabled = true;
            this.cboFloor.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboFloor.ImageDisabled = null;
            this.cboFloor.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboFloor.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboFloor.Location = new System.Drawing.Point(792, 130);
            this.cboFloor.Name = "cboFloor";
            this.cboFloor.Owner = null;
            this.cboFloor.Size = new System.Drawing.Size(133, 25);
            this.cboFloor.TabIndex = 19;
            this.cboFloor.TextColor = System.Drawing.Color.Black;
            this.cboFloor.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // cboPSMSensorLocations
            // 
            this.cboPSMSensorLocations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPSMSensorLocations.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboPSMSensorLocations.FormattingEnabled = true;
            this.cboPSMSensorLocations.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboPSMSensorLocations.ImageDisabled = null;
            this.cboPSMSensorLocations.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboPSMSensorLocations.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboPSMSensorLocations.Location = new System.Drawing.Point(511, 46);
            this.cboPSMSensorLocations.Name = "cboPSMSensorLocations";
            this.cboPSMSensorLocations.Owner = null;
            this.cboPSMSensorLocations.Size = new System.Drawing.Size(133, 25);
            this.cboPSMSensorLocations.TabIndex = 20;
            this.cboPSMSensorLocations.TextColor = System.Drawing.Color.Black;
            this.cboPSMSensorLocations.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboPSMSensorLocations.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // cboDisasterPreventionEquipmentLocation
            // 
            this.cboDisasterPreventionEquipmentLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDisasterPreventionEquipmentLocation.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboDisasterPreventionEquipmentLocation.FormattingEnabled = true;
            this.cboDisasterPreventionEquipmentLocation.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboDisasterPreventionEquipmentLocation.ImageDisabled = null;
            this.cboDisasterPreventionEquipmentLocation.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboDisasterPreventionEquipmentLocation.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboDisasterPreventionEquipmentLocation.Location = new System.Drawing.Point(511, 57);
            this.cboDisasterPreventionEquipmentLocation.Name = "cboDisasterPreventionEquipmentLocation";
            this.cboDisasterPreventionEquipmentLocation.Owner = null;
            this.cboDisasterPreventionEquipmentLocation.Size = new System.Drawing.Size(133, 25);
            this.cboDisasterPreventionEquipmentLocation.TabIndex = 21;
            this.cboDisasterPreventionEquipmentLocation.TextColor = System.Drawing.Color.Black;
            this.cboDisasterPreventionEquipmentLocation.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // FormSensorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BackgroundImage = global::SDMS.Properties.Resources.SensorList_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(943, 620);
            this.Controls.Add(this.gvSensorList);
            this.Controls.Add(this.cboDisasterPreventionEquipmentLocation);
            this.Controls.Add(this.cboPSMSensorLocations);
            this.Controls.Add(this.cboFloor);
            this.Controls.Add(this.cboBuilding);
            this.Controls.Add(this.cboBuildingGroup);
            this.Controls.Add(this.cboPSMSensorStatus);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.cboSensorType);
            this.Controls.Add(this.btnSelectZone);
            this.Controls.Add(this.gvDisasterPreventionEquipment);
            this.Controls.Add(this.lblFacilityName);
            this.Controls.Add(this.lblOperationStatus);
            this.Controls.Add(this.lblFacilityType);
            this.Controls.Add(this.lblSelectZone);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSensorList";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "센서구역/CCTV/소방시설 리스트";
            this.Load += new System.EventHandler(this.FormSensorList_Load);
            this.VisibleChanged += new System.EventHandler(this.FormSensorList_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.gvSensorList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sensorListGridDataBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDisasterPreventionEquipment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSelectZone)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSelectZone;
        private System.Windows.Forms.Label lblFacilityType;
        private System.Windows.Forms.Label lblOperationStatus;
        private System.Windows.Forms.DataGridView gvSensorList;
        private System.Windows.Forms.Label lblFacilityName;
        private System.Windows.Forms.DataGridView gvDisasterPreventionEquipment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisasterPreventionEquipmentNo;
        private System.Windows.Forms.DataGridViewComboBoxColumn colDisasterPreventionEquipmentType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisasterPreventionEquipmentName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colDisasterPreventionEquipmentLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisasterPreventionEquipmentQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisasterPreventionEquipmentDescription;
        private System.Windows.Forms.BindingSource sensorListGridDataBindingSource;
        private UnE.GUI.ImageButton btnSelectZone;
        private UnE.GUI.ImageComboBox cboSensorType;
        private UnE.GUI.ImageComboBox cboStatus;
        private UnE.GUI.ImageComboBox cboPSMSensorStatus;
        private UnE.GUI.ImageComboBox cboBuildingGroup;
        private UnE.GUI.ImageComboBox cboBuilding;
        private UnE.GUI.ImageComboBox cboFloor;
        private UnE.GUI.ImageComboBox cboPSMSensorLocations;
        private UnE.GUI.ImageComboBox cboDisasterPreventionEquipmentLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBuilding;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFloor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colETC;
        private System.Windows.Forms.DataGridViewTextBoxColumn noDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn buildingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn floorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sensorTypeIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn equipmentZoneDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn zoneDataGridViewTextBoxColumn;

    }
}