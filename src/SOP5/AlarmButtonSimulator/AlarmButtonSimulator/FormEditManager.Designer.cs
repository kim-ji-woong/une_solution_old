namespace AlarmButtonSimulator
{
    partial class FormEditManager
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelMiddle = new System.Windows.Forms.Label();
            this.labelLow = new System.Windows.Forms.Label();
            this.radioLevelMiddle = new System.Windows.Forms.RadioButton();
            this.textBoxLow = new System.Windows.Forms.TextBox();
            this.radioLevelLow = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBoxMiddle = new System.Windows.Forms.TextBox();
            this.labelMode = new System.Windows.Forms.Label();
            this.labelLevel = new System.Windows.Forms.Label();
            this.textBoxLevel = new System.Windows.Forms.TextBox();
            this.radioLevelLimit = new System.Windows.Forms.RadioButton();
            this.colTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.gridManager = new System.Windows.Forms.DataGridView();
            this.colLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMember = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.radioNoLimit = new System.Windows.Forms.RadioButton();
            this.gridMember = new System.Windows.Forms.DataGridView();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMiddle
            // 
            this.labelMiddle.AutoSize = true;
            this.labelMiddle.Location = new System.Drawing.Point(121, 336);
            this.labelMiddle.Name = "labelMiddle";
            this.labelMiddle.Size = new System.Drawing.Size(57, 12);
            this.labelMiddle.TabIndex = 38;
            this.labelMiddle.Text = "급만 해당";
            // 
            // labelLow
            // 
            this.labelLow.AutoSize = true;
            this.labelLow.Location = new System.Drawing.Point(121, 313);
            this.labelLow.Name = "labelLow";
            this.labelLow.Size = new System.Drawing.Size(145, 12);
            this.labelLow.TabIndex = 37;
            this.labelLow.Text = "급 및 그 하위 직급만 해당";
            // 
            // radioLevelMiddle
            // 
            this.radioLevelMiddle.AutoSize = true;
            this.radioLevelMiddle.Location = new System.Drawing.Point(74, 334);
            this.radioLevelMiddle.Name = "radioLevelMiddle";
            this.radioLevelMiddle.Size = new System.Drawing.Size(14, 13);
            this.radioLevelMiddle.TabIndex = 34;
            this.radioLevelMiddle.TabStop = true;
            this.radioLevelMiddle.UseVisualStyleBackColor = true;
            this.radioLevelMiddle.CheckedChanged += new System.EventHandler(this.rdLevelLow_CheckedChanged);
            // 
            // textBoxLow
            // 
            this.textBoxLow.Location = new System.Drawing.Point(94, 308);
            this.textBoxLow.Name = "textBoxLow";
            this.textBoxLow.Size = new System.Drawing.Size(24, 21);
            this.textBoxLow.TabIndex = 35;
            this.textBoxLow.Text = "4";
            this.textBoxLow.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // radioLevelLow
            // 
            this.radioLevelLow.AutoSize = true;
            this.radioLevelLow.Location = new System.Drawing.Point(74, 311);
            this.radioLevelLow.Name = "radioLevelLow";
            this.radioLevelLow.Size = new System.Drawing.Size(14, 13);
            this.radioLevelLow.TabIndex = 33;
            this.radioLevelLow.TabStop = true;
            this.radioLevelLow.UseVisualStyleBackColor = true;
            this.radioLevelLow.CheckedChanged += new System.EventHandler(this.rdLevelLow_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(423, 577);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(57, 23);
            this.btnOK.TabIndex = 32;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(486, 577);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(57, 23);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // textBoxMiddle
            // 
            this.textBoxMiddle.Location = new System.Drawing.Point(94, 331);
            this.textBoxMiddle.Name = "textBoxMiddle";
            this.textBoxMiddle.Size = new System.Drawing.Size(24, 21);
            this.textBoxMiddle.TabIndex = 36;
            this.textBoxMiddle.Text = "4";
            this.textBoxMiddle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.Location = new System.Drawing.Point(16, 288);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(45, 12);
            this.labelMode.TabIndex = 30;
            this.labelMode.Text = "팀 선택";
            // 
            // labelLevel
            // 
            this.labelLevel.AutoSize = true;
            this.labelLevel.Location = new System.Drawing.Point(121, 290);
            this.labelLevel.Name = "labelLevel";
            this.labelLevel.Size = new System.Drawing.Size(145, 12);
            this.labelLevel.TabIndex = 29;
            this.labelLevel.Text = "급 및 그 상위 직급만 해당";
            // 
            // textBoxLevel
            // 
            this.textBoxLevel.Location = new System.Drawing.Point(94, 285);
            this.textBoxLevel.Name = "textBoxLevel";
            this.textBoxLevel.Size = new System.Drawing.Size(24, 21);
            this.textBoxLevel.TabIndex = 28;
            this.textBoxLevel.Text = "4";
            this.textBoxLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // radioLevelLimit
            // 
            this.radioLevelLimit.AutoSize = true;
            this.radioLevelLimit.Location = new System.Drawing.Point(74, 288);
            this.radioLevelLimit.Name = "radioLevelLimit";
            this.radioLevelLimit.Size = new System.Drawing.Size(14, 13);
            this.radioLevelLimit.TabIndex = 27;
            this.radioLevelLimit.TabStop = true;
            this.radioLevelLimit.UseVisualStyleBackColor = true;
            // 
            // colTeam
            // 
            this.colTeam.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTeam.HeaderText = "비고";
            this.colTeam.Name = "colTeam";
            this.colTeam.ReadOnly = true;
            // 
            // colName
            // 
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 170;
            // 
            // colNo
            // 
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 30;
            // 
            // btnAdd
            // 
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(472, 277);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(71, 26);
            this.btnAdd.TabIndex = 25;
            this.btnAdd.Text = "추가";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(472, 307);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(71, 26);
            this.btnRemove.TabIndex = 24;
            this.btnRemove.Text = "삭제";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // gridManager
            // 
            this.gridManager.AllowUserToAddRows = false;
            this.gridManager.AllowUserToDeleteRows = false;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(73)))), ((int)(((byte)(106)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridManager.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.gridManager.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridManager.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colName,
            this.colTeam});
            this.gridManager.Location = new System.Drawing.Point(12, 355);
            this.gridManager.Name = "gridManager";
            this.gridManager.ReadOnly = true;
            this.gridManager.RowHeadersVisible = false;
            this.gridManager.RowTemplate.Height = 23;
            this.gridManager.Size = new System.Drawing.Size(531, 211);
            this.gridManager.TabIndex = 23;
            // 
            // colLevel
            // 
            this.colLevel.HeaderText = "직급";
            this.colLevel.Name = "colLevel";
            this.colLevel.ReadOnly = true;
            this.colLevel.Width = 60;
            // 
            // colMember
            // 
            this.colMember.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMember.HeaderText = "팀원";
            this.colMember.Name = "colMember";
            this.colMember.ReadOnly = true;
            // 
            // colIndex
            // 
            this.colIndex.HeaderText = "No";
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            this.colIndex.Width = 30;
            // 
            // radioNoLimit
            // 
            this.radioNoLimit.AutoSize = true;
            this.radioNoLimit.Checked = true;
            this.radioNoLimit.Location = new System.Drawing.Point(355, 288);
            this.radioNoLimit.Name = "radioNoLimit";
            this.radioNoLimit.Size = new System.Drawing.Size(75, 16);
            this.radioNoLimit.TabIndex = 26;
            this.radioNoLimit.TabStop = true;
            this.radioNoLimit.Text = "모든 팀원";
            this.radioNoLimit.UseVisualStyleBackColor = true;
            // 
            // gridMember
            // 
            this.gridMember.AllowUserToAddRows = false;
            this.gridMember.AllowUserToDeleteRows = false;
            this.gridMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMember.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIndex,
            this.colMember,
            this.colLevel});
            this.gridMember.Location = new System.Drawing.Point(333, 12);
            this.gridMember.Name = "gridMember";
            this.gridMember.RowHeadersVisible = false;
            this.gridMember.RowTemplate.Height = 23;
            this.gridMember.Size = new System.Drawing.Size(211, 261);
            this.gridMember.TabIndex = 22;
            this.gridMember.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMember_CellClick);
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Location = new System.Drawing.Point(12, 12);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(315, 261);
            this.treeViewTeam.TabIndex = 21;
            this.treeViewTeam.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterCollapse);
            this.treeViewTeam.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterExpand);
            this.treeViewTeam.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewTeam_NodeMouseClick);
            // 
            // FormEditManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 614);
            this.Controls.Add(this.labelMiddle);
            this.Controls.Add(this.labelLow);
            this.Controls.Add(this.radioLevelMiddle);
            this.Controls.Add(this.textBoxLow);
            this.Controls.Add(this.radioLevelLow);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.textBoxMiddle);
            this.Controls.Add(this.labelMode);
            this.Controls.Add(this.labelLevel);
            this.Controls.Add(this.textBoxLevel);
            this.Controls.Add(this.radioLevelLimit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.gridManager);
            this.Controls.Add(this.radioNoLimit);
            this.Controls.Add(this.gridMember);
            this.Controls.Add(this.treeViewTeam);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormEditManager";
            this.Text = "담당자 편집";
            this.Load += new System.EventHandler(this.FormEditManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMiddle;
        private System.Windows.Forms.Label labelLow;
        private System.Windows.Forms.RadioButton radioLevelMiddle;
        private System.Windows.Forms.TextBox textBoxLow;
        private System.Windows.Forms.RadioButton radioLevelLow;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBoxMiddle;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.Label labelLevel;
        private System.Windows.Forms.TextBox textBoxLevel;
        private System.Windows.Forms.RadioButton radioLevelLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.DataGridView gridManager;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMember;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.RadioButton radioNoLimit;
        private System.Windows.Forms.DataGridView gridMember;
        private System.Windows.Forms.TreeView treeViewTeam;
    }
}