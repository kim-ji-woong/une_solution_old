namespace HSMS
{
    partial class FormWorker
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.gridManager = new System.Windows.Forms.DataGridView();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.gridMember = new System.Windows.Forms.DataGridView();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCompanyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColSensorID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cboWorkerLevel = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ColNumber2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeam2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCompanyName2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWorkerLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColSensorID2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMemberID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnOK.Location = new System.Drawing.Point(351, 666);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 28);
            this.btnOK.TabIndex = 49;
            this.btnOK.Text = "저장";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnCancel.Location = new System.Drawing.Point(468, 666);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 28);
            this.btnCancel.TabIndex = 50;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackgroundImage = global::HSMS.Properties.Resources.Arrow_Down;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(799, 293);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(57, 43);
            this.btnAdd.TabIndex = 42;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.BackgroundImage = global::HSMS.Properties.Resources.Arrow_Up;
            this.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(705, 293);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(57, 43);
            this.btnRemove.TabIndex = 43;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // gridManager
            // 
            this.gridManager.AllowUserToAddRows = false;
            this.gridManager.AllowUserToDeleteRows = false;
            this.gridManager.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9F);
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
            this.ColNumber2,
            this.colName2,
            this.colTeam2,
            this.ColCompanyName2,
            this.colWorkerLevel,
            this.ColSensorID2,
            this.colMemberID});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridManager.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridManager.Location = new System.Drawing.Point(30, 421);
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
            this.gridManager.Size = new System.Drawing.Size(885, 211);
            this.gridManager.TabIndex = 41;
            this.gridManager.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridManager_CellMouseDown);
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.treeViewTeam.Location = new System.Drawing.Point(30, 88);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(356, 261);
            this.treeViewTeam.TabIndex = 39;
            this.treeViewTeam.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewTeam_BeforeSelect);
            this.treeViewTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(922, 47);
            this.panel1.TabIndex = 57;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(20, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "작업자 생성";
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
            dataGridViewCellStyle7.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridMember.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.gridMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMember.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colID,
            this.colName,
            this.ColTeam,
            this.colCompanyName,
            this.ColSensorID});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridMember.DefaultCellStyle = dataGridViewCellStyle8;
            this.gridMember.Location = new System.Drawing.Point(400, 88);
            this.gridMember.MultiSelect = false;
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
            this.gridMember.Size = new System.Drawing.Size(515, 261);
            this.gridMember.TabIndex = 40;
            // 
            // colID
            // 
            this.colID.FillWeight = 20F;
            this.colID.HeaderText = "ID";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.FillWeight = 20F;
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // ColTeam
            // 
            this.ColTeam.HeaderText = "팀";
            this.ColTeam.Name = "ColTeam";
            // 
            // colCompanyName
            // 
            this.colCompanyName.FillWeight = 20F;
            this.colCompanyName.HeaderText = "회사명";
            this.colCompanyName.Name = "colCompanyName";
            this.colCompanyName.ReadOnly = true;
            // 
            // ColSensorID
            // 
            this.ColSensorID.FillWeight = 40F;
            this.ColSensorID.HeaderText = "센서 ID";
            this.ColSensorID.Name = "ColSensorID";
            // 
            // cboWorkerLevel
            // 
            this.cboWorkerLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWorkerLevel.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.cboWorkerLevel.FormattingEnabled = true;
            this.cboWorkerLevel.Items.AddRange(new object[] {
            "1등급",
            "2등급",
            "3등급",
            "4등급",
            "5등급"});
            this.cboWorkerLevel.Location = new System.Drawing.Point(517, 305);
            this.cboWorkerLevel.Name = "cboWorkerLevel";
            this.cboWorkerLevel.Size = new System.Drawing.Size(148, 23);
            this.cboWorkerLevel.TabIndex = 69;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.label2.Location = new System.Drawing.Point(426, 310);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 15);
            this.label2.TabIndex = 70;
            this.label2.Text = "출입등급 선택";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btnAdd);
            this.panel2.Controls.Add(this.cboWorkerLevel);
            this.panel2.Controls.Add(this.btnRemove);
            this.panel2.Location = new System.Drawing.Point(12, 68);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(922, 581);
            this.panel2.TabIndex = 71;
            // 
            // ColNumber2
            // 
            this.ColNumber2.HeaderText = "No";
            this.ColNumber2.Name = "ColNumber2";
            this.ColNumber2.ReadOnly = true;
            this.ColNumber2.Width = 55;
            // 
            // colName2
            // 
            this.colName2.FillWeight = 30F;
            this.colName2.HeaderText = "이름";
            this.colName2.Name = "colName2";
            this.colName2.ReadOnly = true;
            this.colName2.Width = 125;
            // 
            // colTeam2
            // 
            this.colTeam2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTeam2.FillWeight = 20F;
            this.colTeam2.HeaderText = "팀";
            this.colTeam2.Name = "colTeam2";
            this.colTeam2.ReadOnly = true;
            // 
            // ColCompanyName2
            // 
            this.ColCompanyName2.HeaderText = "회사명";
            this.ColCompanyName2.Name = "ColCompanyName2";
            this.ColCompanyName2.ReadOnly = true;
            this.ColCompanyName2.Width = 150;
            // 
            // colWorkerLevel
            // 
            this.colWorkerLevel.HeaderText = "출입등급";
            this.colWorkerLevel.Name = "colWorkerLevel";
            this.colWorkerLevel.ReadOnly = true;
            this.colWorkerLevel.Width = 120;
            // 
            // ColSensorID2
            // 
            this.ColSensorID2.FillWeight = 20F;
            this.ColSensorID2.HeaderText = "센서ID";
            this.ColSensorID2.Name = "ColSensorID2";
            this.ColSensorID2.ReadOnly = true;
            this.ColSensorID2.Width = 150;
            // 
            // colMemberID
            // 
            this.colMemberID.HeaderText = "사원번호";
            this.colMemberID.Name = "colMemberID";
            this.colMemberID.ReadOnly = true;
            this.colMemberID.Width = 150;
            // 
            // FormWorker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(946, 706);
            this.Controls.Add(this.treeViewTeam);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gridManager);
            this.Controls.Add(this.gridMember);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormWorker";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormWorker";
            this.Load += new System.EventHandler(this.FormWorker_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.DataGridView gridManager;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gridMember;
        private System.Windows.Forms.ComboBox cboWorkerLevel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColTeam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCompanyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSensorID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColNumber2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeam2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCompanyName2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWorkerLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSensorID2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMemberID;
    }
}