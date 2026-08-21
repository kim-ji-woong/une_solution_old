namespace FireManagement
{
    partial class PageBackstageNew
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageBackstageNew));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblCaption = new System.Windows.Forms.Label();
            this.lblBackstageSeparator1 = new AxXtremeCommandBars.AxBackstageSeparator();
            this.label1 = new System.Windows.Forms.Label();
            this.textFile = new System.Windows.Forms.TextBox();
            this.btnDlgOpen = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.axBackstageSeparator1 = new AxXtremeCommandBars.AxBackstageSeparator();
            this.dataGridEquipment = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCount = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridVersion = new System.Windows.Forms.DataGridView();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cboBuilding = new System.Windows.Forms.ComboBox();
            this.cboFloor = new System.Windows.Forms.ComboBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.lblBackstageSeparator1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axBackstageSeparator1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEquipment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridVersion)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.BackColor = System.Drawing.Color.White;
            this.lblCaption.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCaption.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.lblCaption.Location = new System.Drawing.Point(12, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCaption.Size = new System.Drawing.Size(500, 35);
            this.lblCaption.TabIndex = 20;
            this.lblCaption.Text = "NEW";
            // 
            // lblBackstageSeparator1
            // 
            this.lblBackstageSeparator1.Enabled = true;
            this.lblBackstageSeparator1.Location = new System.Drawing.Point(12, 47);
            this.lblBackstageSeparator1.Name = "lblBackstageSeparator1";
            this.lblBackstageSeparator1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("lblBackstageSeparator1.OcxState")));
            this.lblBackstageSeparator1.Size = new System.Drawing.Size(603, 18);
            this.lblBackstageSeparator1.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 12);
            this.label1.TabIndex = 22;
            this.label1.Text = "관리점검 대상 파일";
            this.label1.Visible = false;
            // 
            // textFile
            // 
            this.textFile.BackColor = System.Drawing.Color.White;
            this.textFile.Location = new System.Drawing.Point(130, 105);
            this.textFile.Name = "textFile";
            this.textFile.ReadOnly = true;
            this.textFile.Size = new System.Drawing.Size(300, 21);
            this.textFile.TabIndex = 23;
            this.textFile.Visible = false;
            // 
            // btnDlgOpen
            // 
            this.btnDlgOpen.Location = new System.Drawing.Point(436, 103);
            this.btnDlgOpen.Name = "btnDlgOpen";
            this.btnDlgOpen.Size = new System.Drawing.Size(36, 23);
            this.btnDlgOpen.TabIndex = 24;
            this.btnDlgOpen.Text = "...";
            this.btnDlgOpen.UseVisualStyleBackColor = true;
            this.btnDlgOpen.Visible = false;
            this.btnDlgOpen.Click += new System.EventHandler(this.btnDlgOpen_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(15, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(603, 33);
            this.label2.TabIndex = 25;
            this.label2.Text = "점검 관리 대상";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(15, 287);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(603, 33);
            this.label3.TabIndex = 25;
            this.label3.Text = "문서 버전";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // axBackstageSeparator1
            // 
            this.axBackstageSeparator1.Enabled = true;
            this.axBackstageSeparator1.Location = new System.Drawing.Point(12, 266);
            this.axBackstageSeparator1.Name = "axBackstageSeparator1";
            this.axBackstageSeparator1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axBackstageSeparator1.OcxState")));
            this.axBackstageSeparator1.Size = new System.Drawing.Size(603, 18);
            this.axBackstageSeparator1.TabIndex = 26;
            // 
            // dataGridEquipment
            // 
            this.dataGridEquipment.AllowUserToAddRows = false;
            this.dataGridEquipment.AllowUserToDeleteRows = false;
            this.dataGridEquipment.AllowUserToResizeColumns = false;
            this.dataGridEquipment.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridEquipment.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridEquipment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridEquipment.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.colCount});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridEquipment.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridEquipment.Location = new System.Drawing.Point(17, 140);
            this.dataGridEquipment.MultiSelect = false;
            this.dataGridEquipment.Name = "dataGridEquipment";
            this.dataGridEquipment.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridEquipment.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridEquipment.RowHeadersVisible = false;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridEquipment.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridEquipment.RowTemplate.Height = 23;
            this.dataGridEquipment.Size = new System.Drawing.Size(455, 115);
            this.dataGridEquipment.TabIndex = 28;
            // 
            // Column1
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column1.HeaderText = "소방설비 항목";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 120;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column2.HeaderText = "소방설비 레이어";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // colCount
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCount.DefaultCellStyle = dataGridViewCellStyle4;
            this.colCount.HeaderText = "개수";
            this.colCount.Name = "colCount";
            this.colCount.ReadOnly = true;
            this.colCount.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colCount.Visible = false;
            // 
            // dataGridVersion
            // 
            this.dataGridVersion.AllowUserToAddRows = false;
            this.dataGridVersion.AllowUserToDeleteRows = false;
            this.dataGridVersion.AllowUserToResizeColumns = false;
            this.dataGridVersion.AllowUserToResizeRows = false;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridVersion.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridVersion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridVersion.ColumnHeadersVisible = false;
            this.dataGridVersion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column4,
            this.Column5});
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridVersion.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridVersion.Location = new System.Drawing.Point(17, 323);
            this.dataGridVersion.MultiSelect = false;
            this.dataGridVersion.Name = "dataGridVersion";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridVersion.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridVersion.RowHeadersVisible = false;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridVersion.RowsDefaultCellStyle = dataGridViewCellStyle13;
            this.dataGridVersion.RowTemplate.Height = 23;
            this.dataGridVersion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridVersion.Size = new System.Drawing.Size(455, 95);
            this.dataGridVersion.TabIndex = 29;
            // 
            // Column4
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle9;
            this.Column4.HeaderText = "Column4";
            this.Column4.Name = "Column4";
            this.Column4.Width = 130;
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column5.DefaultCellStyle = dataGridViewCellStyle10;
            this.Column5.HeaderText = "Column5";
            this.Column5.Name = "Column5";
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.Location = new System.Drawing.Point(162, 105);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Size = new System.Drawing.Size(248, 20);
            this.cboBuilding.TabIndex = 30;
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
            // 
            // cboFloor
            // 
            this.cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloor.FormattingEnabled = true;
            this.cboFloor.Location = new System.Drawing.Point(416, 105);
            this.cboFloor.Name = "cboFloor";
            this.cboFloor.Size = new System.Drawing.Size(56, 20);
            this.cboFloor.TabIndex = 30;
            this.cboFloor.SelectedIndexChanged += new System.EventHandler(this.cboFloor_SelectedIndexChanged);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(397, 440);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 31;
            this.btnApply.Text = "도면열기";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.Location = new System.Drawing.Point(17, 105);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Size = new System.Drawing.Size(139, 20);
            this.cboBuildingGroup.TabIndex = 30;
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // PageBackstageNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(666, 530);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.cboFloor);
            this.Controls.Add(this.cboBuildingGroup);
            this.Controls.Add(this.cboBuilding);
            this.Controls.Add(this.dataGridVersion);
            this.Controls.Add(this.dataGridEquipment);
            this.Controls.Add(this.axBackstageSeparator1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnDlgOpen);
            this.Controls.Add(this.textFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblBackstageSeparator1);
            this.Controls.Add(this.lblCaption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageNew";
            this.Text = "PageBackstageNew";
            ((System.ComponentModel.ISupportInitialize)(this.lblBackstageSeparator1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axBackstageSeparator1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEquipment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridVersion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblCaption;
        public AxXtremeCommandBars.AxBackstageSeparator lblBackstageSeparator1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textFile;
        private System.Windows.Forms.Button btnDlgOpen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public AxXtremeCommandBars.AxBackstageSeparator axBackstageSeparator1;
        private System.Windows.Forms.DataGridView dataGridEquipment;
        private System.Windows.Forms.DataGridView dataGridVersion;
        private System.Windows.Forms.ComboBox cboBuilding;
        private System.Windows.Forms.ComboBox cboFloor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCount;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ComboBox cboBuildingGroup;
    }
}