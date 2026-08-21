namespace SDMS
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnOK = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.labelMiddle = new System.Windows.Forms.Label();
            this.textBoxMiddle = new System.Windows.Forms.TextBox();
            this.labelLow = new System.Windows.Forms.Label();
            this.radioLevelMiddle = new System.Windows.Forms.RadioButton();
            this.textBoxLow = new System.Windows.Forms.TextBox();
            this.radioLevelLow = new System.Windows.Forms.RadioButton();
            this.labelMode = new System.Windows.Forms.Label();
            this.labelLevel = new System.Windows.Forms.Label();
            this.textBoxLevel = new System.Windows.Forms.TextBox();
            this.radioNoLimit = new System.Windows.Forms.RadioButton();
            this.radioLevelLimit = new System.Windows.Forms.RadioButton();
            this.gridManager = new System.Windows.Forms.DataGridView();
            this.gridMember = new System.Windows.Forms.DataGridView();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.btnRemove = new UnE.GUI.ImageButton();
            this.btnAdd = new UnE.GUI.ImageButton();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMember = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.ButtonText = "";
            this.btnOK.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ImageClicked = global::SDMS.Properties.Resources.BtnOk_Click;
            this.btnOK.ImageDisabled = null;
            this.btnOK.ImageMouseOver = global::SDMS.Properties.Resources.BtnOk_Click;
            this.btnOK.ImageNormal = global::SDMS.Properties.Resources.BtnOk_Default;
            this.btnOK.Location = new System.Drawing.Point(477, 609);
            this.btnOK.Name = "btnOK";
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(51, 29);
            this.btnOK.TabIndex = 22;
            this.btnOK.TabStop = false;
            this.btnOK.TextColor = System.Drawing.Color.Black;
            this.btnOK.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ToolTipText = "";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.ButtonText = "";
            this.btnCancel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ImageClicked = global::SDMS.Properties.Resources.BtnCancel_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SDMS.Properties.Resources.BtnCancel_Click;
            this.btnCancel.ImageNormal = global::SDMS.Properties.Resources.BtnCancel_Default;
            this.btnCancel.Location = new System.Drawing.Point(534, 609);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(51, 29);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // labelMiddle
            // 
            this.labelMiddle.AutoSize = true;
            this.labelMiddle.BackColor = System.Drawing.Color.Transparent;
            this.labelMiddle.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMiddle.ForeColor = System.Drawing.Color.White;
            this.labelMiddle.Location = new System.Drawing.Point(164, 347);
            this.labelMiddle.Name = "labelMiddle";
            this.labelMiddle.Size = new System.Drawing.Size(72, 18);
            this.labelMiddle.TabIndex = 20;
            this.labelMiddle.Text = "급만 해당";
            // 
            // textBoxMiddle
            // 
            this.textBoxMiddle.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMiddle.Location = new System.Drawing.Point(113, 344);
            this.textBoxMiddle.Name = "textBoxMiddle";
            this.textBoxMiddle.Size = new System.Drawing.Size(45, 26);
            this.textBoxMiddle.TabIndex = 19;
            this.textBoxMiddle.Text = "4";
            this.textBoxMiddle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelLow
            // 
            this.labelLow.AutoSize = true;
            this.labelLow.BackColor = System.Drawing.Color.Transparent;
            this.labelLow.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLow.ForeColor = System.Drawing.Color.White;
            this.labelLow.Location = new System.Drawing.Point(164, 320);
            this.labelLow.Name = "labelLow";
            this.labelLow.Size = new System.Drawing.Size(178, 18);
            this.labelLow.TabIndex = 20;
            this.labelLow.Text = "급 및 그 하위 직급만 해당";
            // 
            // radioLevelMiddle
            // 
            this.radioLevelMiddle.AutoSize = true;
            this.radioLevelMiddle.BackColor = System.Drawing.Color.Transparent;
            this.radioLevelMiddle.Font = new System.Drawing.Font(Program.prgFont, 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioLevelMiddle.Location = new System.Drawing.Point(91, 350);
            this.radioLevelMiddle.Name = "radioLevelMiddle";
            this.radioLevelMiddle.Size = new System.Drawing.Size(14, 13);
            this.radioLevelMiddle.TabIndex = 18;
            this.radioLevelMiddle.TabStop = true;
            this.radioLevelMiddle.UseVisualStyleBackColor = false;
            this.radioLevelMiddle.CheckedChanged += new System.EventHandler(this.rdLevelLow_CheckedChanged);
            // 
            // textBoxLow
            // 
            this.textBoxLow.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxLow.Location = new System.Drawing.Point(113, 317);
            this.textBoxLow.Name = "textBoxLow";
            this.textBoxLow.Size = new System.Drawing.Size(45, 26);
            this.textBoxLow.TabIndex = 19;
            this.textBoxLow.Text = "4";
            this.textBoxLow.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // radioLevelLow
            // 
            this.radioLevelLow.AutoSize = true;
            this.radioLevelLow.BackColor = System.Drawing.Color.Transparent;
            this.radioLevelLow.Font = new System.Drawing.Font(Program.prgFont, 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioLevelLow.Location = new System.Drawing.Point(91, 323);
            this.radioLevelLow.Name = "radioLevelLow";
            this.radioLevelLow.Size = new System.Drawing.Size(14, 13);
            this.radioLevelLow.TabIndex = 18;
            this.radioLevelLow.TabStop = true;
            this.radioLevelLow.UseVisualStyleBackColor = false;
            this.radioLevelLow.CheckedChanged += new System.EventHandler(this.rdLevelLow_CheckedChanged);
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.BackColor = System.Drawing.Color.Transparent;
            this.labelMode.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMode.ForeColor = System.Drawing.Color.White;
            this.labelMode.Location = new System.Drawing.Point(13, 296);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(61, 18);
            this.labelMode.TabIndex = 16;
            this.labelMode.Text = "팀 선택";
            // 
            // labelLevel
            // 
            this.labelLevel.AutoSize = true;
            this.labelLevel.BackColor = System.Drawing.Color.Transparent;
            this.labelLevel.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLevel.ForeColor = System.Drawing.Color.White;
            this.labelLevel.Location = new System.Drawing.Point(164, 296);
            this.labelLevel.Name = "labelLevel";
            this.labelLevel.Size = new System.Drawing.Size(178, 18);
            this.labelLevel.TabIndex = 15;
            this.labelLevel.Text = "급 및 그 상위 직급만 해당";
            // 
            // textBoxLevel
            // 
            this.textBoxLevel.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxLevel.Location = new System.Drawing.Point(113, 290);
            this.textBoxLevel.Name = "textBoxLevel";
            this.textBoxLevel.Size = new System.Drawing.Size(45, 26);
            this.textBoxLevel.TabIndex = 14;
            this.textBoxLevel.Text = "4";
            this.textBoxLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // radioNoLimit
            // 
            this.radioNoLimit.AutoSize = true;
            this.radioNoLimit.BackColor = System.Drawing.Color.Transparent;
            this.radioNoLimit.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioNoLimit.ForeColor = System.Drawing.Color.White;
            this.radioNoLimit.Location = new System.Drawing.Point(362, 292);
            this.radioNoLimit.Name = "radioNoLimit";
            this.radioNoLimit.Size = new System.Drawing.Size(90, 22);
            this.radioNoLimit.TabIndex = 13;
            this.radioNoLimit.TabStop = true;
            this.radioNoLimit.Text = "모든 팀원";
            this.radioNoLimit.UseVisualStyleBackColor = false;
            // 
            // radioLevelLimit
            // 
            this.radioLevelLimit.AutoSize = true;
            this.radioLevelLimit.BackColor = System.Drawing.Color.Transparent;
            this.radioLevelLimit.Font = new System.Drawing.Font(Program.prgFont, 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioLevelLimit.Location = new System.Drawing.Point(91, 297);
            this.radioLevelLimit.Name = "radioLevelLimit";
            this.radioLevelLimit.Size = new System.Drawing.Size(14, 13);
            this.radioLevelLimit.TabIndex = 13;
            this.radioLevelLimit.TabStop = true;
            this.radioLevelLimit.UseVisualStyleBackColor = false;
            // 
            // gridManager
            // 
            this.gridManager.AllowUserToAddRows = false;
            this.gridManager.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(73)))), ((int)(((byte)(106)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridManager.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridManager.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridManager.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colName,
            this.colTeam});
            this.gridManager.Location = new System.Drawing.Point(10, 372);
            this.gridManager.Name = "gridManager";
            this.gridManager.ReadOnly = true;
            this.gridManager.RowHeadersVisible = false;
            this.gridManager.RowTemplate.Height = 23;
            this.gridManager.Size = new System.Drawing.Size(574, 231);
            this.gridManager.TabIndex = 11;
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
            this.gridMember.Location = new System.Drawing.Point(362, 6);
            this.gridMember.Name = "gridMember";
            this.gridMember.RowHeadersVisible = false;
            this.gridMember.RowTemplate.Height = 23;
            this.gridMember.Size = new System.Drawing.Size(223, 280);
            this.gridMember.TabIndex = 1;
            this.gridMember.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMember_CellClick);
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Location = new System.Drawing.Point(10, 6);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(348, 280);
            this.treeViewTeam.TabIndex = 0;
            this.treeViewTeam.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterCollapse);
            this.treeViewTeam.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterExpand);
            this.treeViewTeam.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewTeam_NodeMouseClick);
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.Transparent;
            this.btnRemove.ButtonText = "";
            this.btnRemove.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemove.ImageClicked = global::SDMS.Properties.Resources.BtnDelete_Click;
            this.btnRemove.ImageDisabled = null;
            this.btnRemove.ImageMouseOver = global::SDMS.Properties.Resources.BtnDelete_Click;
            this.btnRemove.ImageNormal = global::SDMS.Properties.Resources.BtnDelete_Default;
            this.btnRemove.Location = new System.Drawing.Point(534, 327);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Owner = null;
            this.btnRemove.Size = new System.Drawing.Size(51, 29);
            this.btnRemove.TabIndex = 23;
            this.btnRemove.TabStop = false;
            this.btnRemove.TextColor = System.Drawing.Color.Black;
            this.btnRemove.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemove.ToolTipText = "";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.Transparent;
            this.btnAdd.ButtonText = "";
            this.btnAdd.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAdd.ImageClicked = global::SDMS.Properties.Resources.BtnAdd_Click;
            this.btnAdd.ImageDisabled = null;
            this.btnAdd.ImageMouseOver = global::SDMS.Properties.Resources.BtnAdd_Click;
            this.btnAdd.ImageNormal = global::SDMS.Properties.Resources.BtnAdd_Default;
            this.btnAdd.Location = new System.Drawing.Point(534, 292);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Owner = null;
            this.btnAdd.Size = new System.Drawing.Size(51, 29);
            this.btnAdd.TabIndex = 24;
            this.btnAdd.TabStop = false;
            this.btnAdd.TextColor = System.Drawing.Color.Black;
            this.btnAdd.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAdd.ToolTipText = "";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // colNo
            // 
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 50;
            // 
            // colName
            // 
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 400;
            // 
            // colTeam
            // 
            this.colTeam.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTeam.HeaderText = "비고";
            this.colTeam.Name = "colTeam";
            this.colTeam.ReadOnly = true;
            // 
            // colIndex
            // 
            this.colIndex.HeaderText = "No";
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            this.colIndex.Width = 50;
            // 
            // colMember
            // 
            this.colMember.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMember.HeaderText = "팀원";
            this.colMember.Name = "colMember";
            this.colMember.ReadOnly = true;
            // 
            // colLevel
            // 
            this.colLevel.HeaderText = "직급";
            this.colLevel.Name = "colLevel";
            this.colLevel.ReadOnly = true;
            // 
            // FormEditManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SDMS.Properties.Resources.EditManager_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(594, 650);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.labelMiddle);
            this.Controls.Add(this.textBoxMiddle);
            this.Controls.Add(this.labelLow);
            this.Controls.Add(this.radioLevelMiddle);
            this.Controls.Add(this.textBoxLow);
            this.Controls.Add(this.radioLevelLow);
            this.Controls.Add(this.labelMode);
            this.Controls.Add(this.labelLevel);
            this.Controls.Add(this.textBoxLevel);
            this.Controls.Add(this.radioNoLimit);
            this.Controls.Add(this.radioLevelLimit);
            this.Controls.Add(this.gridManager);
            this.Controls.Add(this.gridMember);
            this.Controls.Add(this.treeViewTeam);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "담당자 편집";
            this.Load += new System.EventHandler(this.FormEditManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.DataGridView gridMember;
        private System.Windows.Forms.DataGridView gridManager;
        private System.Windows.Forms.RadioButton radioLevelLimit;
        private System.Windows.Forms.RadioButton radioNoLimit;
        private System.Windows.Forms.TextBox textBoxLevel;
        private System.Windows.Forms.Label labelLevel;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label labelLow;
		private System.Windows.Forms.TextBox textBoxLow;
		private System.Windows.Forms.RadioButton radioLevelLow;
        private System.Windows.Forms.RadioButton radioLevelMiddle;
        private System.Windows.Forms.TextBox textBoxMiddle;
        private System.Windows.Forms.Label labelMiddle;
        private UnE.GUI.ImageButton btnCancel;
        private UnE.GUI.ImageButton btnOK;
        private UnE.GUI.ImageButton btnRemove;
        private UnE.GUI.ImageButton btnAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMember;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLevel;
    }
}