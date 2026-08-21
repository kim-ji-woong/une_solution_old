namespace SDMS
{
    partial class FormFacilityZone
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnEdit = new System.Windows.Forms.Button();
            this.gridSensorList = new System.Windows.Forms.DataGridView();
            this.checkBoxFA = new System.Windows.Forms.CheckBox();
            this.checkBoxSpringCooler = new System.Windows.Forms.CheckBox();
            this.checkBoxFireSensor = new System.Windows.Forms.CheckBox();
            this.cboEquipZone = new System.Windows.Forms.ComboBox();
            this.cboFloor = new System.Windows.Forms.ComboBox();
            this.cboBuilding = new System.Windows.Forms.ComboBox();
            this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSensorList)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(8, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(442, 47);
            this.panel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(20, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "설비 영역";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnEdit);
            this.panel2.Controls.Add(this.gridSensorList);
            this.panel2.Controls.Add(this.checkBoxFA);
            this.panel2.Controls.Add(this.checkBoxSpringCooler);
            this.panel2.Controls.Add(this.checkBoxFireSensor);
            this.panel2.Controls.Add(this.cboEquipZone);
            this.panel2.Controls.Add(this.cboFloor);
            this.panel2.Controls.Add(this.cboBuilding);
            this.panel2.Controls.Add(this.cboBuildingGroup);
            this.panel2.Location = new System.Drawing.Point(8, 68);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(442, 353);
            this.panel2.TabIndex = 3;
            // 
            // btnEdit
            // 
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Location = new System.Drawing.Point(360, 102);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(61, 23);
            this.btnEdit.TabIndex = 13;
            this.btnEdit.Text = "편집";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // gridSensorList
            // 
            this.gridSensorList.AllowUserToAddRows = false;
            this.gridSensorList.AllowUserToDeleteRows = false;
            this.gridSensorList.AllowUserToResizeColumns = false;
            this.gridSensorList.AllowUserToResizeRows = false;
            this.gridSensorList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSensorList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.Type,
            this.Column2,
            this.Column1});
            this.gridSensorList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gridSensorList.Location = new System.Drawing.Point(22, 137);
            this.gridSensorList.Name = "gridSensorList";
            this.gridSensorList.RowHeadersVisible = false;
            this.gridSensorList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridSensorList.RowTemplate.Height = 23;
            this.gridSensorList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridSensorList.Size = new System.Drawing.Size(399, 192);
            this.gridSensorList.TabIndex = 12;
            // 
            // checkBoxFA
            // 
            this.checkBoxFA.AutoSize = true;
            this.checkBoxFA.Enabled = false;
            this.checkBoxFA.Location = new System.Drawing.Point(29, 180);
            this.checkBoxFA.Name = "checkBoxFA";
            this.checkBoxFA.Size = new System.Drawing.Size(60, 16);
            this.checkBoxFA.TabIndex = 11;
            this.checkBoxFA.Text = "발신기";
            this.checkBoxFA.UseVisualStyleBackColor = true;
            // 
            // checkBoxSpringCooler
            // 
            this.checkBoxSpringCooler.AutoSize = true;
            this.checkBoxSpringCooler.Location = new System.Drawing.Point(159, 144);
            this.checkBoxSpringCooler.Name = "checkBoxSpringCooler";
            this.checkBoxSpringCooler.Size = new System.Drawing.Size(84, 16);
            this.checkBoxSpringCooler.TabIndex = 9;
            this.checkBoxSpringCooler.Text = "스프링쿨러";
            this.checkBoxSpringCooler.UseVisualStyleBackColor = true;
            this.checkBoxSpringCooler.CheckedChanged += new System.EventHandler(this.checkBoxSpringCooler_CheckedChanged);
            // 
            // checkBoxFireSensor
            // 
            this.checkBoxFireSensor.AutoSize = true;
            this.checkBoxFireSensor.Location = new System.Drawing.Point(29, 144);
            this.checkBoxFireSensor.Name = "checkBoxFireSensor";
            this.checkBoxFireSensor.Size = new System.Drawing.Size(104, 16);
            this.checkBoxFireSensor.TabIndex = 10;
            this.checkBoxFireSensor.Text = "화재 탐지 센서";
            this.checkBoxFireSensor.UseVisualStyleBackColor = true;
            this.checkBoxFireSensor.CheckedChanged += new System.EventHandler(this.checkBoxFireSensor_CheckedChanged);
            // 
            // cboEquipZone
            // 
            this.cboEquipZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEquipZone.FormattingEnabled = true;
            this.cboEquipZone.Location = new System.Drawing.Point(23, 105);
            this.cboEquipZone.Name = "cboEquipZone";
            this.cboEquipZone.Size = new System.Drawing.Size(300, 20);
            this.cboEquipZone.TabIndex = 8;
            this.cboEquipZone.SelectedIndexChanged += new System.EventHandler(this.cboEquipZone_SelectedIndexChanged);
            this.cboEquipZone.SelectionChangeCommitted += new System.EventHandler(this.cboEquipZone_SelectionChangeCommitted);
            // 
            // cboFloor
            // 
            this.cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloor.FormattingEnabled = true;
            this.cboFloor.Location = new System.Drawing.Point(23, 79);
            this.cboFloor.Name = "cboFloor";
            this.cboFloor.Size = new System.Drawing.Size(121, 20);
            this.cboFloor.TabIndex = 5;
            this.cboFloor.SelectedIndexChanged += new System.EventHandler(this.cboFloor_SelectedIndexChanged);
            this.cboFloor.SelectionChangeCommitted += new System.EventHandler(this.cboFloor_SelectionChangeCommitted);
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.Location = new System.Drawing.Point(23, 53);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Size = new System.Drawing.Size(300, 20);
            this.cboBuilding.TabIndex = 6;
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
            this.cboBuilding.SelectionChangeCommitted += new System.EventHandler(this.cboBuilding_SelectionChangeCommitted);
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.Location = new System.Drawing.Point(23, 27);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Size = new System.Drawing.Size(300, 20);
            this.cboBuildingGroup.TabIndex = 7;
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            this.cboBuildingGroup.SelectionChangeCommitted += new System.EventHandler(this.cboBuildingGroup_SelectionChangeCommitted);
            // 
            // No
            // 
            this.No.HeaderText = "순번";
            this.No.Name = "No";
            this.No.Width = 55;
            // 
            // Type
            // 
            this.Type.HeaderText = "종류";
            this.Type.Name = "Type";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "이름";
            this.Column2.Name = "Column2";
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "설치위치";
            this.Column1.Name = "Column1";
            // 
            // FormFacilityZone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 433);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormFacilityZone";
            this.Text = "FormFacilityZone";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSensorList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox checkBoxFA;
        private System.Windows.Forms.CheckBox checkBoxSpringCooler;
        private System.Windows.Forms.CheckBox checkBoxFireSensor;
        private System.Windows.Forms.ComboBox cboEquipZone;
        private System.Windows.Forms.ComboBox cboFloor;
        private System.Windows.Forms.ComboBox cboBuilding;
        private System.Windows.Forms.ComboBox cboBuildingGroup;
        private System.Windows.Forms.DataGridView gridSensorList;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    }
}