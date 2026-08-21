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
            this.cboBuilding = new UnE.GUI.ImageComboBox();
            this.cboBuildingGroup = new UnE.GUI.ImageComboBox();
            this.cboAdditional = new UnE.GUI.ImageComboBox();
            this.cmbEquipZone = new UnE.GUI.ImageComboBox();
            this.cmbFloor = new UnE.GUI.ImageComboBox();
            this.cmbType = new UnE.GUI.ImageComboBox();
            this.cboSensorType = new UnE.GUI.ImageComboBox();
            this.btnEdit = new UnE.GUI.ImageButton();
            this.radioReport = new System.Windows.Forms.RadioButton();
            this.radioDetect = new System.Windows.Forms.RadioButton();
            this.btnShowSimulationModeManager = new System.Windows.Forms.Button();
            this.checkBoxBuilding = new System.Windows.Forms.CheckBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.gridManager = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.btnEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
            this.SuspendLayout();
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
            this.cboBuilding.Location = new System.Drawing.Point(418, 77);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Owner = null;
            this.cboBuilding.Size = new System.Drawing.Size(304, 25);
            this.cboBuilding.TabIndex = 24;
            this.cboBuilding.TextColor = System.Drawing.Color.Black;
            this.cboBuilding.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
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
            this.cboBuildingGroup.Location = new System.Drawing.Point(204, 77);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Owner = null;
            this.cboBuildingGroup.Size = new System.Drawing.Size(208, 25);
            this.cboBuildingGroup.TabIndex = 23;
            this.cboBuildingGroup.TextColor = System.Drawing.Color.Black;
            this.cboBuildingGroup.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // cboAdditional
            // 
            this.cboAdditional.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAdditional.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboAdditional.FormattingEnabled = true;
            this.cboAdditional.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboAdditional.ImageDisabled = null;
            this.cboAdditional.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cboAdditional.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboAdditional.Location = new System.Drawing.Point(204, 77);
            this.cboAdditional.Name = "cboAdditional";
            this.cboAdditional.Owner = null;
            this.cboAdditional.Size = new System.Drawing.Size(208, 25);
            this.cboAdditional.TabIndex = 22;
            this.cboAdditional.TextColor = System.Drawing.Color.Black;
            this.cboAdditional.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // cmbEquipZone
            // 
            this.cmbEquipZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquipZone.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbEquipZone.FormattingEnabled = true;
            this.cmbEquipZone.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbEquipZone.ImageDisabled = null;
            this.cmbEquipZone.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbEquipZone.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cmbEquipZone.Location = new System.Drawing.Point(204, 109);
            this.cmbEquipZone.Name = "cmbEquipZone";
            this.cmbEquipZone.Owner = null;
            this.cmbEquipZone.Size = new System.Drawing.Size(518, 25);
            this.cmbEquipZone.TabIndex = 21;
            this.cmbEquipZone.TextColor = System.Drawing.Color.Black;
            this.cmbEquipZone.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbEquipZone.SelectedIndexChanged += new System.EventHandler(this.cmbEquipZone_SelectedIndexChanged);
            // 
            // cmbFloor
            // 
            this.cmbFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFloor.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbFloor.FormattingEnabled = true;
            this.cmbFloor.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbFloor.ImageDisabled = null;
            this.cmbFloor.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbFloor.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cmbFloor.Location = new System.Drawing.Point(12, 109);
            this.cmbFloor.Name = "cmbFloor";
            this.cmbFloor.Owner = null;
            this.cmbFloor.Size = new System.Drawing.Size(186, 25);
            this.cmbFloor.TabIndex = 20;
            this.cmbFloor.TextColor = System.Drawing.Color.Black;
            this.cmbFloor.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbFloor.SelectedIndexChanged += new System.EventHandler(this.cmbFloor_SelectedIndexChanged);
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbType.FormattingEnabled = true;
            this.cmbType.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbType.ImageDisabled = null;
            this.cmbType.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbType.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cmbType.Location = new System.Drawing.Point(12, 77);
            this.cmbType.Name = "cmbType";
            this.cmbType.Owner = null;
            this.cmbType.Size = new System.Drawing.Size(186, 25);
            this.cmbType.TabIndex = 19;
            this.cmbType.TextColor = System.Drawing.Color.Black;
            this.cmbType.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
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
            this.cboSensorType.Location = new System.Drawing.Point(12, 45);
            this.cboSensorType.Name = "cboSensorType";
            this.cboSensorType.Owner = null;
            this.cboSensorType.Size = new System.Drawing.Size(186, 25);
            this.cboSensorType.TabIndex = 18;
            this.cboSensorType.TextColor = System.Drawing.Color.Black;
            this.cboSensorType.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboSensorType.SelectedIndexChanged += new System.EventHandler(this.cboSensorType_SelectedIndexChanged);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.Transparent;
            this.btnEdit.ButtonText = "";
            this.btnEdit.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnEdit.ImageClicked = global::SDMS.Properties.Resources.BtnEdit_Click;
            this.btnEdit.ImageDisabled = null;
            this.btnEdit.ImageMouseOver = global::SDMS.Properties.Resources.BtnEdit_Click;
            this.btnEdit.ImageNormal = global::SDMS.Properties.Resources.BtnEdit_Default;
            this.btnEdit.Location = new System.Drawing.Point(660, 43);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Owner = null;
            this.btnEdit.Size = new System.Drawing.Size(62, 29);
            this.btnEdit.TabIndex = 17;
            this.btnEdit.TabStop = false;
            this.btnEdit.TextColor = System.Drawing.Color.Black;
            this.btnEdit.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnEdit.ToolTipText = "";
            this.btnEdit.UseToolTip = false;
            this.btnEdit.WindowRateWidth = 1F;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // radioReport
            // 
            this.radioReport.AutoSize = true;
            this.radioReport.BackColor = System.Drawing.Color.Transparent;
            this.radioReport.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioReport.ForeColor = System.Drawing.Color.White;
            this.radioReport.Location = new System.Drawing.Point(338, 47);
            this.radioReport.Name = "radioReport";
            this.radioReport.Size = new System.Drawing.Size(127, 20);
            this.radioReport.TabIndex = 16;
            this.radioReport.Text = "전파시 담당자";
            this.radioReport.UseVisualStyleBackColor = false;
            this.radioReport.Visible = false;
            this.radioReport.CheckedChanged += new System.EventHandler(this.radioFacilityType_CheckedChanged);
            // 
            // radioDetect
            // 
            this.radioDetect.AutoSize = true;
            this.radioDetect.BackColor = System.Drawing.Color.Transparent;
            this.radioDetect.Checked = true;
            this.radioDetect.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioDetect.ForeColor = System.Drawing.Color.White;
            this.radioDetect.Location = new System.Drawing.Point(204, 47);
            this.radioDetect.Name = "radioDetect";
            this.radioDetect.Size = new System.Drawing.Size(127, 20);
            this.radioDetect.TabIndex = 16;
            this.radioDetect.TabStop = true;
            this.radioDetect.Text = "탐지시 담당자";
            this.radioDetect.UseVisualStyleBackColor = false;
            this.radioDetect.Visible = false;
            this.radioDetect.CheckedChanged += new System.EventHandler(this.radioFacilityType_CheckedChanged);
            // 
            // btnShowSimulationModeManager
            // 
            this.btnShowSimulationModeManager.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowSimulationModeManager.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnShowSimulationModeManager.Location = new System.Drawing.Point(592, 2);
            this.btnShowSimulationModeManager.Name = "btnShowSimulationModeManager";
            this.btnShowSimulationModeManager.Size = new System.Drawing.Size(130, 29);
            this.btnShowSimulationModeManager.TabIndex = 11;
            this.btnShowSimulationModeManager.Text = "연습용 담당자보기";
            this.btnShowSimulationModeManager.UseVisualStyleBackColor = true;
            this.btnShowSimulationModeManager.Visible = false;
            this.btnShowSimulationModeManager.Click += new System.EventHandler(this.btnShowSimulationModeManager_Click);
            // 
            // checkBoxBuilding
            // 
            this.checkBoxBuilding.AutoSize = true;
            this.checkBoxBuilding.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxBuilding.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxBuilding.ForeColor = System.Drawing.Color.White;
            this.checkBoxBuilding.Location = new System.Drawing.Point(472, 47);
            this.checkBoxBuilding.Name = "checkBoxBuilding";
            this.checkBoxBuilding.Size = new System.Drawing.Size(112, 20);
            this.checkBoxBuilding.TabIndex = 2;
            this.checkBoxBuilding.Text = "건물별 보기";
            this.checkBoxBuilding.UseVisualStyleBackColor = false;
            this.checkBoxBuilding.Visible = false;
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.BackColor = System.Drawing.Color.Transparent;
            this.labelDescription.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDescription.ForeColor = System.Drawing.Color.White;
            this.labelDescription.Location = new System.Drawing.Point(591, 47);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(40, 16);
            this.labelDescription.TabIndex = 1;
            this.labelDescription.Text = "설명";
            this.labelDescription.Visible = false;
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
            this.gridManager.Location = new System.Drawing.Point(12, 141);
            this.gridManager.Name = "gridManager";
            this.gridManager.ReadOnly = true;
            this.gridManager.RowHeadersVisible = false;
            this.gridManager.RowTemplate.Height = 23;
            this.gridManager.Size = new System.Drawing.Size(710, 401);
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
            // FormManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BackgroundImage = global::SDMS.Properties.Resources.Manager_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(732, 554);
            this.Controls.Add(this.cboBuilding);
            this.Controls.Add(this.cboBuildingGroup);
            this.Controls.Add(this.cboAdditional);
            this.Controls.Add(this.cmbEquipZone);
            this.Controls.Add(this.cmbFloor);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.cboSensorType);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.radioReport);
            this.Controls.Add(this.radioDetect);
            this.Controls.Add(this.btnShowSimulationModeManager);
            this.Controls.Add(this.checkBoxBuilding);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.gridManager);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormManager";
            this.Text = "z";
            this.Load += new System.EventHandler(this.FormManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.CheckBox checkBoxBuilding;
        private System.Windows.Forms.DataGridView gridManager;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeam;
        private System.Windows.Forms.Button btnShowSimulationModeManager;
        private System.Windows.Forms.RadioButton radioReport;
        private System.Windows.Forms.RadioButton radioDetect;
        private UnE.GUI.ImageButton btnEdit;
        private UnE.GUI.ImageComboBox cboSensorType;
        private UnE.GUI.ImageComboBox cmbType;
        private UnE.GUI.ImageComboBox cmbFloor;
        private UnE.GUI.ImageComboBox cmbEquipZone;
        private UnE.GUI.ImageComboBox cboAdditional;
        private UnE.GUI.ImageComboBox cboBuildingGroup;
        private UnE.GUI.ImageComboBox cboBuilding;
    }
}