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
			this.btnSelectZone = new System.Windows.Forms.Button();
			this.cboFloor = new System.Windows.Forms.ComboBox();
			this.cboBuilding = new System.Windows.Forms.ComboBox();
			this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
			this.proc_lblSelectZone = new System.Windows.Forms.Label();
			this.cboSensorType = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cboStatus = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.gridSensorList = new System.Windows.Forms.DataGridView();
			this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colBuilding = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colFloor = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colETC = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.gridSensorList)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnSelectZone
			// 
			this.btnSelectZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSelectZone.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnSelectZone.Location = new System.Drawing.Point(776, 16);
			this.btnSelectZone.Name = "btnSelectZone";
			this.btnSelectZone.Size = new System.Drawing.Size(67, 28);
			this.btnSelectZone.TabIndex = 9;
			this.btnSelectZone.Text = "선택";
			this.btnSelectZone.UseVisualStyleBackColor = true;
			this.btnSelectZone.Click += new System.EventHandler(this.btnSelectZone_Click);
			// 
			// cboFloor
			// 
			this.cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboFloor.FormattingEnabled = true;
			this.cboFloor.Location = new System.Drawing.Point(670, 18);
			this.cboFloor.Name = "cboFloor";
			this.cboFloor.Size = new System.Drawing.Size(95, 20);
			this.cboFloor.TabIndex = 8;
			// 
			// cboBuilding
			// 
			this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboBuilding.FormattingEnabled = true;
			this.cboBuilding.Location = new System.Drawing.Point(416, 18);
			this.cboBuilding.Name = "cboBuilding";
			this.cboBuilding.Size = new System.Drawing.Size(248, 20);
			this.cboBuilding.TabIndex = 7;
			this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
			// 
			// cboBuildingGroup
			// 
			this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboBuildingGroup.FormattingEnabled = true;
			this.cboBuildingGroup.Location = new System.Drawing.Point(271, 18);
			this.cboBuildingGroup.Name = "cboBuildingGroup";
			this.cboBuildingGroup.Size = new System.Drawing.Size(139, 20);
			this.cboBuildingGroup.TabIndex = 6;
			this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
			// 
			// proc_lblSelectZone
			// 
			this.proc_lblSelectZone.AutoSize = true;
			this.proc_lblSelectZone.Location = new System.Drawing.Point(212, 22);
			this.proc_lblSelectZone.Name = "proc_lblSelectZone";
			this.proc_lblSelectZone.Size = new System.Drawing.Size(53, 12);
			this.proc_lblSelectZone.TabIndex = 5;
			this.proc_lblSelectZone.Text = "범위선택";
			// 
			// cboSensorType
			// 
			this.cboSensorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboSensorType.FormattingEnabled = true;
			this.cboSensorType.Location = new System.Drawing.Point(423, 53);
			this.cboSensorType.Name = "cboSensorType";
			this.cboSensorType.Size = new System.Drawing.Size(139, 20);
			this.cboSensorType.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(364, 57);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 5;
			this.label1.Text = "유형선택";
			// 
			// cboStatus
			// 
			this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboStatus.FormattingEnabled = true;
			this.cboStatus.Location = new System.Drawing.Point(670, 52);
			this.cboStatus.Name = "cboStatus";
			this.cboStatus.Size = new System.Drawing.Size(96, 20);
			this.cboStatus.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(610, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "운영상태";
			// 
			// gridSensorList
			// 
			this.gridSensorList.AllowUserToAddRows = false;
			this.gridSensorList.AllowUserToDeleteRows = false;
			this.gridSensorList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridSensorList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colType,
            this.colStatus,
            this.colBuilding,
            this.colFloor,
            this.colETC});
			this.gridSensorList.Location = new System.Drawing.Point(11, 13);
			this.gridSensorList.Name = "gridSensorList";
			this.gridSensorList.RowHeadersVisible = false;
			this.gridSensorList.RowTemplate.Height = 23;
			this.gridSensorList.Size = new System.Drawing.Size(843, 362);
			this.gridSensorList.TabIndex = 10;
			// 
			// colNo
			// 
			this.colNo.HeaderText = "No";
			this.colNo.Name = "colNo";
			this.colNo.ReadOnly = true;
			this.colNo.Width = 30;
			// 
			// colType
			// 
			this.colType.HeaderText = "유형";
			this.colType.Name = "colType";
			this.colType.Width = 120;
			// 
			// colStatus
			// 
			this.colStatus.HeaderText = "운영상태";
			this.colStatus.Name = "colStatus";
			this.colStatus.ReadOnly = true;
			this.colStatus.Width = 150;
			// 
			// colBuilding
			// 
			this.colBuilding.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colBuilding.HeaderText = "건물";
			this.colBuilding.Name = "colBuilding";
			this.colBuilding.ReadOnly = true;
			// 
			// colFloor
			// 
			this.colFloor.HeaderText = "층";
			this.colFloor.Name = "colFloor";
			this.colFloor.ReadOnly = true;
			// 
			// colETC
			// 
			this.colETC.HeaderText = "비고";
			this.colETC.Name = "colETC";
			this.colETC.ReadOnly = true;
			this.colETC.Width = 200;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Controls.Add(this.btnSelectZone);
			this.panel1.Controls.Add(this.cboFloor);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.cboStatus);
			this.panel1.Controls.Add(this.proc_lblSelectZone);
			this.panel1.Controls.Add(this.cboSensorType);
			this.panel1.Controls.Add(this.cboBuilding);
			this.panel1.Controls.Add(this.cboBuildingGroup);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(865, 88);
			this.panel1.TabIndex = 11;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.White;
			this.panel2.Controls.Add(this.gridSensorList);
			this.panel2.Location = new System.Drawing.Point(12, 113);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(865, 385);
			this.panel2.TabIndex = 12;
			// 
			// FormSensorList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.LightGray;
			this.ClientSize = new System.Drawing.Size(889, 511);
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
			((System.ComponentModel.ISupportInitialize)(this.gridSensorList)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelectZone;
        private System.Windows.Forms.ComboBox cboFloor;
        private System.Windows.Forms.ComboBox cboBuilding;
        private System.Windows.Forms.ComboBox cboBuildingGroup;
        private System.Windows.Forms.Label proc_lblSelectZone;
        private System.Windows.Forms.ComboBox cboSensorType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView gridSensorList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBuilding;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFloor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colETC;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;

    }
}