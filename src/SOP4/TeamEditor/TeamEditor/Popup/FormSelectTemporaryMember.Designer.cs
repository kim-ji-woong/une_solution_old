namespace TeamEditor.Popup
{
    partial class FormSelectTemporaryMember
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.radioRegularTeam = new System.Windows.Forms.RadioButton();
            this.radioCompanyMember = new System.Windows.Forms.RadioButton();
            this.radioExternalCompanyTeam = new System.Windows.Forms.RadioButton();
            this.radioExternalCompanyMember = new System.Windows.Forms.RadioButton();
            this.radioLevelID = new System.Windows.Forms.RadioButton();
            this.radioUserDefinedTeam = new System.Windows.Forms.RadioButton();
            this.panelForms = new System.Windows.Forms.Panel();
            this.gridLevelID = new System.Windows.Forms.DataGridView();
            this.colLevelID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelForms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLevelID)).BeginInit();
            this.SuspendLayout();
            // 
            // radioRegularTeam
            // 
            this.radioRegularTeam.AutoSize = true;
            this.radioRegularTeam.Checked = true;
            this.radioRegularTeam.Location = new System.Drawing.Point(12, 12);
            this.radioRegularTeam.Name = "radioRegularTeam";
            this.radioRegularTeam.Size = new System.Drawing.Size(83, 16);
            this.radioRegularTeam.TabIndex = 0;
            this.radioRegularTeam.TabStop = true;
            this.radioRegularTeam.Text = "정규조직도";
            this.radioRegularTeam.UseVisualStyleBackColor = true;
            this.radioRegularTeam.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioCompanyMember
            // 
            this.radioCompanyMember.AutoSize = true;
            this.radioCompanyMember.Location = new System.Drawing.Point(101, 12);
            this.radioCompanyMember.Name = "radioCompanyMember";
            this.radioCompanyMember.Size = new System.Drawing.Size(75, 16);
            this.radioCompanyMember.TabIndex = 0;
            this.radioCompanyMember.Text = "정규 직원";
            this.radioCompanyMember.UseVisualStyleBackColor = true;
            this.radioCompanyMember.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioExternalCompanyTeam
            // 
            this.radioExternalCompanyTeam.AutoSize = true;
            this.radioExternalCompanyTeam.Location = new System.Drawing.Point(182, 12);
            this.radioExternalCompanyTeam.Name = "radioExternalCompanyTeam";
            this.radioExternalCompanyTeam.Size = new System.Drawing.Size(71, 16);
            this.radioExternalCompanyTeam.TabIndex = 0;
            this.radioExternalCompanyTeam.Text = "협력업체";
            this.radioExternalCompanyTeam.UseVisualStyleBackColor = true;
            this.radioExternalCompanyTeam.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioExternalCompanyMember
            // 
            this.radioExternalCompanyMember.AutoSize = true;
            this.radioExternalCompanyMember.Location = new System.Drawing.Point(259, 12);
            this.radioExternalCompanyMember.Name = "radioExternalCompanyMember";
            this.radioExternalCompanyMember.Size = new System.Drawing.Size(99, 16);
            this.radioExternalCompanyMember.TabIndex = 0;
            this.radioExternalCompanyMember.Text = "협력업체 직원";
            this.radioExternalCompanyMember.UseVisualStyleBackColor = true;
            this.radioExternalCompanyMember.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioLevelID
            // 
            this.radioLevelID.AutoSize = true;
            this.radioLevelID.Location = new System.Drawing.Point(364, 12);
            this.radioLevelID.Name = "radioLevelID";
            this.radioLevelID.Size = new System.Drawing.Size(87, 16);
            this.radioLevelID.TabIndex = 0;
            this.radioLevelID.Text = "직급별 전체";
            this.radioLevelID.UseVisualStyleBackColor = true;
            this.radioLevelID.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioUserDefinedTeam
            // 
            this.radioUserDefinedTeam.AutoSize = true;
            this.radioUserDefinedTeam.Location = new System.Drawing.Point(457, 12);
            this.radioUserDefinedTeam.Name = "radioUserDefinedTeam";
            this.radioUserDefinedTeam.Size = new System.Drawing.Size(107, 16);
            this.radioUserDefinedTeam.TabIndex = 0;
            this.radioUserDefinedTeam.Text = "사용자정의조직";
            this.radioUserDefinedTeam.UseVisualStyleBackColor = true;
            this.radioUserDefinedTeam.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // panelForms
            // 
            this.panelForms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelForms.Controls.Add(this.gridLevelID);
            this.panelForms.Location = new System.Drawing.Point(12, 34);
            this.panelForms.Name = "panelForms";
            this.panelForms.Size = new System.Drawing.Size(906, 367);
            this.panelForms.TabIndex = 1;
            // 
            // gridLevelID
            // 
            this.gridLevelID.AllowUserToAddRows = false;
            this.gridLevelID.AllowUserToDeleteRows = false;
            this.gridLevelID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridLevelID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLevelID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLevelID});
            this.gridLevelID.Location = new System.Drawing.Point(3, 3);
            this.gridLevelID.MultiSelect = false;
            this.gridLevelID.Name = "gridLevelID";
            this.gridLevelID.ReadOnly = true;
            this.gridLevelID.RowHeadersVisible = false;
            this.gridLevelID.RowTemplate.Height = 23;
            this.gridLevelID.Size = new System.Drawing.Size(305, 361);
            this.gridLevelID.TabIndex = 0;
            this.gridLevelID.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridLevelID_CellClick);
            // 
            // colLevelID
            // 
            this.colLevelID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colLevelID.DefaultCellStyle = dataGridViewCellStyle2;
            this.colLevelID.HeaderText = "직급별 전체";
            this.colLevelID.Name = "colLevelID";
            this.colLevelID.ReadOnly = true;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(780, 407);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(66, 28);
            this.btnSelect.TabIndex = 3;
            this.btnSelect.Text = "선택";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(852, 407);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(66, 28);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormSelectTemporaryMember
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 449);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.panelForms);
            this.Controls.Add(this.radioUserDefinedTeam);
            this.Controls.Add(this.radioLevelID);
            this.Controls.Add(this.radioExternalCompanyMember);
            this.Controls.Add(this.radioExternalCompanyTeam);
            this.Controls.Add(this.radioCompanyMember);
            this.Controls.Add(this.radioRegularTeam);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectTemporaryMember";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "비상조직 담당자 설정";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSelectTemporaryMember_FormClosing);
            this.panelForms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLevelID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioRegularTeam;
        private System.Windows.Forms.RadioButton radioCompanyMember;
        private System.Windows.Forms.RadioButton radioExternalCompanyTeam;
        private System.Windows.Forms.RadioButton radioExternalCompanyMember;
        private System.Windows.Forms.RadioButton radioLevelID;
        private System.Windows.Forms.RadioButton radioUserDefinedTeam;
        private System.Windows.Forms.Panel panelForms;
        private System.Windows.Forms.DataGridView gridLevelID;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLevelID;
    }
}