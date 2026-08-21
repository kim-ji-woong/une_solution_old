using System.Windows.Forms;
namespace SDMS
{
    partial class FormSensorMgrList
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
            this.gvSensorMgrList = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBuildingGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBuilding = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEZone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeActivated = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.sensorMgrBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cboSensorType = new UnE.GUI.ImageComboBox();
            this.cboBuildingGroup = new UnE.GUI.ImageComboBox();
            this.cboBuilding = new UnE.GUI.ImageComboBox();
            this.searchBtn = new UnE.GUI.ImageButton();
            this.storeBtn = new UnE.GUI.ImageButton();
            this.btnEditCheck = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.gvSensorMgrList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sensorMgrBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.storeBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEditCheck)).BeginInit();
            this.SuspendLayout();
            // 
            // gvSensorMgrList
            // 
            this.gvSensorMgrList.AllowUserToAddRows = false;
            this.gvSensorMgrList.AllowUserToDeleteRows = false;
            this.gvSensorMgrList.AutoGenerateColumns = false;
            this.gvSensorMgrList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvSensorMgrList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colType,
            this.colName,
            this.colBuildingGroup,
            this.colBuilding,
            this.colEZone,
            this.colDeActivated});
            this.gvSensorMgrList.DataSource = this.sensorMgrBindingSource;
            this.gvSensorMgrList.Location = new System.Drawing.Point(21, 83);
            this.gvSensorMgrList.Name = "gvSensorMgrList";
            this.gvSensorMgrList.RowHeadersVisible = false;
            this.gvSensorMgrList.RowTemplate.Height = 23;
            this.gvSensorMgrList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gvSensorMgrList.Size = new System.Drawing.Size(993, 392);
            this.gvSensorMgrList.TabIndex = 0;
            this.gvSensorMgrList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvSensorMgrList_CellContentClick);
            this.gvSensorMgrList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvSensorMgrList_CellValueChanged);
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
            this.colType.HeaderText = "센서 유형";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // colName
            // 
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "센서 이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 120;
            // 
            // colBuildingGroup
            // 
            this.colBuildingGroup.DataPropertyName = "BuildingGroupName";
            this.colBuildingGroup.HeaderText = "구역";
            this.colBuildingGroup.Name = "colBuildingGroup";
            this.colBuildingGroup.ReadOnly = true;
            // 
            // colBuilding
            // 
            this.colBuilding.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colBuilding.DataPropertyName = "BuildingName";
            this.colBuilding.HeaderText = "건물";
            this.colBuilding.Name = "colBuilding";
            this.colBuilding.ReadOnly = true;
            // 
            // colEZone
            // 
            this.colEZone.DataPropertyName = "EZoneName";
            this.colEZone.HeaderText = "센서 영역";
            this.colEZone.Name = "colEZone";
            this.colEZone.ReadOnly = true;
            // 
            // colDeActivated
            // 
            this.colDeActivated.DataPropertyName = "SensorDeActivated";
            this.colDeActivated.FalseValue = false;
            this.colDeActivated.HeaderText = "비활성화";
            this.colDeActivated.Name = "colDeActivated";
            this.colDeActivated.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colDeActivated.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colDeActivated.TrueValue = true;
            // 
            // sensorMgrBindingSource
            // 
            this.sensorMgrBindingSource.DataSource = typeof(SDMS.Admin.SensorMgrListGridData);
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
            this.cboSensorType.Location = new System.Drawing.Point(70, 46);
            this.cboSensorType.Name = "cboSensorType";
            this.cboSensorType.Owner = null;
            this.cboSensorType.Size = new System.Drawing.Size(108, 25);
            this.cboSensorType.TabIndex = 19;
            this.cboSensorType.TextColor = System.Drawing.Color.Black;
            this.cboSensorType.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            this.cboBuildingGroup.Location = new System.Drawing.Point(218, 46);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Owner = null;
            this.cboBuildingGroup.Size = new System.Drawing.Size(236, 25);
            this.cboBuildingGroup.TabIndex = 20;
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
            this.cboBuilding.Location = new System.Drawing.Point(494, 46);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Owner = null;
            this.cboBuilding.Size = new System.Drawing.Size(351, 25);
            this.cboBuilding.TabIndex = 21;
            this.cboBuilding.TextColor = System.Drawing.Color.Black;
            this.cboBuilding.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // searchBtn
            // 
            this.searchBtn.BackColor = System.Drawing.Color.Transparent;
            this.searchBtn.ButtonText = "";
            this.searchBtn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.searchBtn.ImageClicked = global::SDMS.Properties.Resources.Search_96_65_Click;
            this.searchBtn.ImageDisabled = null;
            this.searchBtn.ImageMouseOver = global::SDMS.Properties.Resources.Search_96_65_Click;
            this.searchBtn.ImageNormal = global::SDMS.Properties.Resources.Search_96_65_Default;
            this.searchBtn.Location = new System.Drawing.Point(914, 41);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Owner = null;
            this.searchBtn.Size = new System.Drawing.Size(48, 33);
            this.searchBtn.TabIndex = 22;
            this.searchBtn.TabStop = false;
            this.searchBtn.TextColor = System.Drawing.Color.Black;
            this.searchBtn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.searchBtn.ToolTipText = "";
            this.searchBtn.UseToolTip = false;
            this.searchBtn.WindowRateWidth = 1F;
            this.searchBtn.Click += new System.EventHandler(this.searchBtnClick);
            // 
            // storeBtn
            // 
            this.storeBtn.BackColor = System.Drawing.Color.Transparent;
            this.storeBtn.ButtonText = "";
            this.storeBtn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.storeBtn.ImageClicked = global::SDMS.Properties.Resources.Save_96_65_Click;
            this.storeBtn.ImageDisabled = null;
            this.storeBtn.ImageMouseOver = global::SDMS.Properties.Resources.Save_96_65_Click;
            this.storeBtn.ImageNormal = global::SDMS.Properties.Resources.Save_96_65_Default;
            this.storeBtn.Location = new System.Drawing.Point(965, 41);
            this.storeBtn.Name = "storeBtn";
            this.storeBtn.Owner = null;
            this.storeBtn.Size = new System.Drawing.Size(48, 33);
            this.storeBtn.TabIndex = 23;
            this.storeBtn.TabStop = false;
            this.storeBtn.TextColor = System.Drawing.Color.Black;
            this.storeBtn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.storeBtn.ToolTipText = "";
            this.storeBtn.UseToolTip = false;
            this.storeBtn.WindowRateWidth = 1F;
            this.storeBtn.Click += new System.EventHandler(this.store_btnClick);
            // 
            // btnEditCheck
            // 
            this.btnEditCheck.BackColor = System.Drawing.Color.Transparent;
            this.btnEditCheck.ButtonText = "";
            this.btnEditCheck.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnEditCheck.ImageClicked = global::SDMS.Properties.Resources.CheckBox_Click;
            this.btnEditCheck.ImageDisabled = null;
            this.btnEditCheck.ImageMouseOver = global::SDMS.Properties.Resources.CheckBox_Default;
            this.btnEditCheck.ImageNormal = global::SDMS.Properties.Resources.CheckBox_Default;
            this.btnEditCheck.Location = new System.Drawing.Point(860, 50);
            this.btnEditCheck.Name = "btnEditCheck";
            this.btnEditCheck.Owner = null;
            this.btnEditCheck.Size = new System.Drawing.Size(16, 16);
            this.btnEditCheck.TabIndex = 24;
            this.btnEditCheck.TabStop = false;
            this.btnEditCheck.TextColor = System.Drawing.Color.Black;
            this.btnEditCheck.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnEditCheck.ToolTipText = "";
            this.btnEditCheck.UseToolTip = false;
            this.btnEditCheck.WindowRateWidth = 1F;
            this.btnEditCheck.Click += new System.EventHandler(this.btnEditCheck_Click);
            // 
            // FormSensorMgrList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BackgroundImage = global::SDMS.Properties.Resources.SensorMgrList_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1038, 502);
            this.Controls.Add(this.btnEditCheck);
            this.Controls.Add(this.storeBtn);
            this.Controls.Add(this.searchBtn);
            this.Controls.Add(this.cboBuilding);
            this.Controls.Add(this.cboBuildingGroup);
            this.Controls.Add(this.cboSensorType);
            this.Controls.Add(this.gvSensorMgrList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSensorMgrList";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormSensorMgrList";
            this.Load += new System.EventHandler(this.FormSensorMgrList_Load);
            this.VisibleChanged += new System.EventHandler(this.FormSensorMgrList_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.gvSensorMgrList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sensorMgrBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.storeBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEditCheck)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gvSensorMgrList;
        private System.Windows.Forms.BindingSource sensorMgrBindingSource;
        private DataGridViewTextBoxColumn colNo;
        private DataGridViewTextBoxColumn colType;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colBuildingGroup;
        private DataGridViewTextBoxColumn colBuilding;
        private DataGridViewTextBoxColumn colEZone;
        private DataGridViewCheckBoxColumn colDeActivated;
        private UnE.GUI.ImageComboBox cboSensorType;
        private UnE.GUI.ImageComboBox cboBuildingGroup;
        private UnE.GUI.ImageComboBox cboBuilding;
        private UnE.GUI.ImageButton searchBtn;
        private UnE.GUI.ImageButton storeBtn;
        private UnE.GUI.ImageButton btnEditCheck;
    }
}