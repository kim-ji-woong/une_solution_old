namespace HSMS
{
    partial class FormDangerFacility
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridManager = new System.Windows.Forms.DataGridView();
            this.gridMember = new System.Windows.Forms.DataGridView();
            this.colEquipName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStandard = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaker = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTypeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSensor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmbEquipGroup = new System.Windows.Forms.ComboBox();
            this.btnAddGroup = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editEquipGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.colNumber2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEquipName2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEquipGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStandard2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaker2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMakeNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTypeName2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSensor2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridManager
            // 
            this.gridManager.AllowUserToAddRows = false;
            this.gridManager.AllowUserToDeleteRows = false;
            this.gridManager.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gridManager.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridManager.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(73)))), ((int)(((byte)(106)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridManager.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridManager.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridManager.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNumber2,
            this.colEquipName2,
            this.colEquipGroup,
            this.colStandard2,
            this.colMaker2,
            this.colMakeNumber,
            this.colTypeName2,
            this.colSensor2});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridManager.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridManager.Location = new System.Drawing.Point(19, 317);
            this.gridManager.Name = "gridManager";
            this.gridManager.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridManager.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.gridManager.RowHeadersVisible = false;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridManager.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.gridManager.RowTemplate.Height = 23;
            this.gridManager.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridManager.Size = new System.Drawing.Size(918, 185);
            this.gridManager.TabIndex = 41;
            this.gridManager.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridManager_CellMouseClick);
            // 
            // gridMember
            // 
            this.gridMember.AllowUserToAddRows = false;
            this.gridMember.AllowUserToDeleteRows = false;
            this.gridMember.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridMember.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.gridMember.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridMember.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.gridMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMember.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEquipName,
            this.colStandard,
            this.colMaker,
            this.colTypeName,
            this.colSensor});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridMember.DefaultCellStyle = dataGridViewCellStyle8;
            this.gridMember.Location = new System.Drawing.Point(350, 75);
            this.gridMember.Name = "gridMember";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridMember.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.gridMember.RowHeadersVisible = false;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridMember.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.gridMember.RowTemplate.Height = 23;
            this.gridMember.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMember.Size = new System.Drawing.Size(599, 229);
            this.gridMember.TabIndex = 40;
            // 
            // colEquipName
            // 
            this.colEquipName.HeaderText = "설비이름";
            this.colEquipName.Name = "colEquipName";
            this.colEquipName.ReadOnly = true;
            this.colEquipName.Width = 130;
            // 
            // colStandard
            // 
            this.colStandard.HeaderText = "설비규격";
            this.colStandard.Name = "colStandard";
            // 
            // colMaker
            // 
            this.colMaker.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMaker.HeaderText = "제작회사";
            this.colMaker.Name = "colMaker";
            this.colMaker.ReadOnly = true;
            // 
            // colTypeName
            // 
            this.colTypeName.HeaderText = "모델명";
            this.colTypeName.Name = "colTypeName";
            this.colTypeName.ReadOnly = true;
            this.colTypeName.Width = 110;
            // 
            // colSensor
            // 
            this.colSensor.HeaderText = "센서ID";
            this.colSensor.Name = "colSensor";
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewTeam.Location = new System.Drawing.Point(31, 75);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(313, 229);
            this.treeViewTeam.TabIndex = 39;
            this.treeViewTeam.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewTeam_BeforeSelect);
            this.treeViewTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(952, 46);
            this.panel1.TabIndex = 66;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "설비 생성";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.cmbEquipGroup);
            this.panel2.Controls.Add(this.btnAddGroup);
            this.panel2.Controls.Add(this.btnRemove);
            this.panel2.Controls.Add(this.btnAdd);
            this.panel2.Controls.Add(this.gridManager);
            this.panel2.Location = new System.Drawing.Point(12, 61);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(952, 521);
            this.panel2.TabIndex = 67;
            // 
            // cmbEquipGroup
            // 
            this.cmbEquipGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquipGroup.FormattingEnabled = true;
            this.cmbEquipGroup.Location = new System.Drawing.Point(338, 271);
            this.cmbEquipGroup.Name = "cmbEquipGroup";
            this.cmbEquipGroup.Size = new System.Drawing.Size(116, 20);
            this.cmbEquipGroup.TabIndex = 71;
            // 
            // btnAddGroup
            // 
            this.btnAddGroup.Location = new System.Drawing.Point(460, 270);
            this.btnAddGroup.Name = "btnAddGroup";
            this.btnAddGroup.Size = new System.Drawing.Size(66, 23);
            this.btnAddGroup.TabIndex = 70;
            this.btnAddGroup.Text = "그룹 추가";
            this.btnAddGroup.UseVisualStyleBackColor = true;
            this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.Location = new System.Drawing.Point(869, 604);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 28);
            this.btnCancel.TabIndex = 69;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.Location = new System.Drawing.Point(755, 604);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 28);
            this.btnOK.TabIndex = 68;
            this.btnOK.Text = "저장";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editEquipGroupToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(151, 26);
            // 
            // editEquipGroupToolStripMenuItem
            // 
            this.editEquipGroupToolStripMenuItem.Name = "editEquipGroupToolStripMenuItem";
            this.editEquipGroupToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.editEquipGroupToolStripMenuItem.Text = "설비그룹 수정";
            this.editEquipGroupToolStripMenuItem.Click += new System.EventHandler(this.editEquipGroupToolStripMenuItem_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.BackgroundImage = global::HSMS.Properties.Resources.Arrow_Up;
            this.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(588, 260);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(51, 43);
            this.btnRemove.TabIndex = 43;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackgroundImage = global::HSMS.Properties.Resources.Arrow_Down;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(661, 260);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(51, 43);
            this.btnAdd.TabIndex = 42;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // colNumber2
            // 
            this.colNumber2.HeaderText = "No";
            this.colNumber2.Name = "colNumber2";
            this.colNumber2.ReadOnly = true;
            this.colNumber2.Width = 55;
            // 
            // colEquipName2
            // 
            this.colEquipName2.HeaderText = "설비이름";
            this.colEquipName2.Name = "colEquipName2";
            this.colEquipName2.ReadOnly = true;
            this.colEquipName2.Width = 150;
            // 
            // colEquipGroup
            // 
            this.colEquipGroup.HeaderText = "설비그룹";
            this.colEquipGroup.Name = "colEquipGroup";
            this.colEquipGroup.ReadOnly = true;
            // 
            // colStandard2
            // 
            this.colStandard2.HeaderText = "설비규격";
            this.colStandard2.Name = "colStandard2";
            this.colStandard2.ReadOnly = true;
            this.colStandard2.Width = 150;
            // 
            // colMaker2
            // 
            this.colMaker2.HeaderText = "제작회사";
            this.colMaker2.Name = "colMaker2";
            this.colMaker2.ReadOnly = true;
            this.colMaker2.Width = 120;
            // 
            // colMakeNumber
            // 
            this.colMakeNumber.HeaderText = "제조번호";
            this.colMakeNumber.Name = "colMakeNumber";
            this.colMakeNumber.ReadOnly = true;
            this.colMakeNumber.Width = 120;
            // 
            // colTypeName2
            // 
            this.colTypeName2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTypeName2.HeaderText = "모델명";
            this.colTypeName2.Name = "colTypeName2";
            this.colTypeName2.ReadOnly = true;
            // 
            // colSensor2
            // 
            this.colSensor2.HeaderText = "센서ID";
            this.colSensor2.Name = "colSensor2";
            this.colSensor2.ReadOnly = true;
            this.colSensor2.Width = 120;
            // 
            // FormDangerFacility
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(976, 644);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gridMember);
            this.Controls.Add(this.treeViewTeam);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDangerFacility";
            this.Text = "FormDangerFacility";
            this.Load += new System.EventHandler(this.FormDangerFacility_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.DataGridView gridManager;
        private System.Windows.Forms.DataGridView gridMember;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStandard;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaker;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTypeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSensor;
        private System.Windows.Forms.ComboBox cmbEquipGroup;
        private System.Windows.Forms.Button btnAddGroup;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editEquipGroupToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipName2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStandard2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaker2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMakeNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTypeName2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSensor2;
    }
}