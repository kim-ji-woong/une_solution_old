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
            this.panel1 = new System.Windows.Forms.Panel();
            this.editCheck = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.storeBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cboBuilding = new System.Windows.Forms.ComboBox();
            this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
            this.cboSensorType = new System.Windows.Forms.ComboBox();
            this.searchBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gvSensorMgrList = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBuildingGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBuilding = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEZone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeActivated = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.sensorMgrBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvSensorMgrList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sensorMgrBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.editCheck);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.storeBtn);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.cboBuilding);
            this.panel1.Controls.Add(this.cboBuildingGroup);
            this.panel1.Controls.Add(this.cboSensorType);
            this.panel1.Controls.Add(this.searchBtn);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(906, 107);
            this.panel1.TabIndex = 0;
            // 
            // editCheck
            // 
            this.editCheck.AutoSize = true;
            this.editCheck.Location = new System.Drawing.Point(652, 30);
            this.editCheck.Name = "editCheck";
            this.editCheck.Size = new System.Drawing.Size(15, 14);
            this.editCheck.TabIndex = 11;
            this.editCheck.UseVisualStyleBackColor = true;
            this.editCheck.CheckedChanged += new System.EventHandler(this.editCheck_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(617, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "편집";
            // 
            // storeBtn
            // 
            this.storeBtn.Location = new System.Drawing.Point(816, 29);
            this.storeBtn.Name = "storeBtn";
            this.storeBtn.Size = new System.Drawing.Size(75, 57);
            this.storeBtn.TabIndex = 9;
            this.storeBtn.Text = "저장";
            this.storeBtn.UseVisualStyleBackColor = true;
            this.storeBtn.Click += new System.EventHandler(this.store_btnClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(353, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "건물";
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.Location = new System.Drawing.Point(355, 66);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Size = new System.Drawing.Size(250, 20);
            this.cboBuilding.TabIndex = 7;
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.Location = new System.Drawing.Point(145, 66);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Size = new System.Drawing.Size(194, 20);
            this.cboBuildingGroup.TabIndex = 6;
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // cboSensorType
            // 
            this.cboSensorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSensorType.FormattingEnabled = true;
            this.cboSensorType.Location = new System.Drawing.Point(16, 66);
            this.cboSensorType.Name = "cboSensorType";
            this.cboSensorType.Size = new System.Drawing.Size(114, 20);
            this.cboSensorType.TabIndex = 5;
            // 
            // searchBtn
            // 
            this.searchBtn.Location = new System.Drawing.Point(713, 29);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Size = new System.Drawing.Size(75, 57);
            this.searchBtn.TabIndex = 3;
            this.searchBtn.Text = "검색";
            this.searchBtn.UseVisualStyleBackColor = true;
            this.searchBtn.Click += new System.EventHandler(this.searchBtnClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(143, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "위치";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "센서종류";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.gvSensorMgrList);
            this.panel2.Location = new System.Drawing.Point(12, 125);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(906, 379);
            this.panel2.TabIndex = 1;
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
            this.gvSensorMgrList.Location = new System.Drawing.Point(16, 14);
            this.gvSensorMgrList.Name = "gvSensorMgrList";
            this.gvSensorMgrList.RowHeadersVisible = false;
            this.gvSensorMgrList.RowTemplate.Height = 23;
            this.gvSensorMgrList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gvSensorMgrList.Size = new System.Drawing.Size(875, 351);
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
            // FormSensorMgrList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(930, 516);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSensorMgrList";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormSensorMgrList";
            this.Load += new System.EventHandler(this.FormSensorMgrList_Load);
            this.VisibleChanged += new System.EventHandler(this.FormSensorMgrList_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvSensorMgrList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sensorMgrBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboBuildingGroup;
        private System.Windows.Forms.ComboBox cboSensorType;
        private System.Windows.Forms.DataGridView gvSensorMgrList;
        private System.Windows.Forms.BindingSource sensorMgrBindingSource;
        private System.Windows.Forms.Button searchBtn;
        private System.Windows.Forms.ComboBox cboBuilding;
        private System.Windows.Forms.Label label3;
        private Button storeBtn;
        private DataGridViewTextBoxColumn colNo;
        private DataGridViewTextBoxColumn colType;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colBuildingGroup;
        private DataGridViewTextBoxColumn colBuilding;
        private DataGridViewTextBoxColumn colEZone;
        private DataGridViewCheckBoxColumn colDeActivated;
        private CheckBox editCheck;
        private Label label4;
    }
}