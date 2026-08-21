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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.radioRegularTeam = new System.Windows.Forms.RadioButton();
            this.radioCompanyMember = new System.Windows.Forms.RadioButton();
            this.radioExternalCompanyTeam = new System.Windows.Forms.RadioButton();
            this.radioExternalCompanyMember = new System.Windows.Forms.RadioButton();
            this.radioLevelID = new System.Windows.Forms.RadioButton();
            this.radioUserDefinedTeam = new System.Windows.Forms.RadioButton();
            this.panelForms = new System.Windows.Forms.Panel();
            this.gridLevelID = new System.Windows.Forms.DataGridView();
            this.colLevelID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSelect = new UnE.GUI.RibbonButton();
            this.btnClose = new UnE.GUI.RibbonButton();
            this.picRegularTeam = new System.Windows.Forms.PictureBox();
            this.lblRegularTeam = new System.Windows.Forms.Label();
            this.picCompanyMember = new System.Windows.Forms.PictureBox();
            this.lblCompanyMember = new System.Windows.Forms.Label();
            this.picExternalCompanyTeam = new System.Windows.Forms.PictureBox();
            this.lblExternalCompanyTeam = new System.Windows.Forms.Label();
            this.picExternalCompanyMember = new System.Windows.Forms.PictureBox();
            this.lblExternalCompanyMember = new System.Windows.Forms.Label();
            this.picLevelID = new System.Windows.Forms.PictureBox();
            this.lblLevelID = new System.Windows.Forms.Label();
            this.picUserDefinedTeam = new System.Windows.Forms.PictureBox();
            this.lblUserDefinedTeam = new System.Windows.Forms.Label();
            this.panelForms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLevelID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRegularTeam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCompanyMember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExternalCompanyTeam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExternalCompanyMember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLevelID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserDefinedTeam)).BeginInit();
            this.SuspendLayout();
            // 
            // radioRegularTeam
            // 
            this.radioRegularTeam.AutoSize = true;
            this.radioRegularTeam.Checked = true;
            this.radioRegularTeam.Location = new System.Drawing.Point(21, 566);
            this.radioRegularTeam.Name = "radioRegularTeam";
            this.radioRegularTeam.Size = new System.Drawing.Size(83, 16);
            this.radioRegularTeam.TabIndex = 0;
            this.radioRegularTeam.TabStop = true;
            this.radioRegularTeam.Text = "정규조직도";
            this.radioRegularTeam.UseVisualStyleBackColor = true;
            this.radioRegularTeam.Visible = false;
            this.radioRegularTeam.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioCompanyMember
            // 
            this.radioCompanyMember.AutoSize = true;
            this.radioCompanyMember.Location = new System.Drawing.Point(110, 566);
            this.radioCompanyMember.Name = "radioCompanyMember";
            this.radioCompanyMember.Size = new System.Drawing.Size(75, 16);
            this.radioCompanyMember.TabIndex = 0;
            this.radioCompanyMember.Text = "정규 직원";
            this.radioCompanyMember.UseVisualStyleBackColor = true;
            this.radioCompanyMember.Visible = false;
            this.radioCompanyMember.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioExternalCompanyTeam
            // 
            this.radioExternalCompanyTeam.AutoSize = true;
            this.radioExternalCompanyTeam.Location = new System.Drawing.Point(191, 566);
            this.radioExternalCompanyTeam.Name = "radioExternalCompanyTeam";
            this.radioExternalCompanyTeam.Size = new System.Drawing.Size(71, 16);
            this.radioExternalCompanyTeam.TabIndex = 0;
            this.radioExternalCompanyTeam.Text = "협력업체";
            this.radioExternalCompanyTeam.UseVisualStyleBackColor = true;
            this.radioExternalCompanyTeam.Visible = false;
            this.radioExternalCompanyTeam.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioExternalCompanyMember
            // 
            this.radioExternalCompanyMember.AutoSize = true;
            this.radioExternalCompanyMember.Location = new System.Drawing.Point(268, 566);
            this.radioExternalCompanyMember.Name = "radioExternalCompanyMember";
            this.radioExternalCompanyMember.Size = new System.Drawing.Size(99, 16);
            this.radioExternalCompanyMember.TabIndex = 0;
            this.radioExternalCompanyMember.Text = "협력업체 직원";
            this.radioExternalCompanyMember.UseVisualStyleBackColor = true;
            this.radioExternalCompanyMember.Visible = false;
            this.radioExternalCompanyMember.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioLevelID
            // 
            this.radioLevelID.AutoSize = true;
            this.radioLevelID.Location = new System.Drawing.Point(373, 566);
            this.radioLevelID.Name = "radioLevelID";
            this.radioLevelID.Size = new System.Drawing.Size(87, 16);
            this.radioLevelID.TabIndex = 0;
            this.radioLevelID.Text = "직급별 전체";
            this.radioLevelID.UseVisualStyleBackColor = true;
            this.radioLevelID.Visible = false;
            this.radioLevelID.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioUserDefinedTeam
            // 
            this.radioUserDefinedTeam.AutoSize = true;
            this.radioUserDefinedTeam.Location = new System.Drawing.Point(466, 566);
            this.radioUserDefinedTeam.Name = "radioUserDefinedTeam";
            this.radioUserDefinedTeam.Size = new System.Drawing.Size(107, 16);
            this.radioUserDefinedTeam.TabIndex = 0;
            this.radioUserDefinedTeam.Text = "사용자정의조직";
            this.radioUserDefinedTeam.UseVisualStyleBackColor = true;
            this.radioUserDefinedTeam.Visible = false;
            this.radioUserDefinedTeam.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // panelForms
            // 
            this.panelForms.Controls.Add(this.gridLevelID);
            this.panelForms.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panelForms.Location = new System.Drawing.Point(12, 48);
            this.panelForms.Name = "panelForms";
            this.panelForms.Size = new System.Drawing.Size(906, 498);
            this.panelForms.TabIndex = 1;
            // 
            // gridLevelID
            // 
            this.gridLevelID.AllowUserToAddRows = false;
            this.gridLevelID.AllowUserToDeleteRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.gridLevelID.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.gridLevelID.BackgroundColor = System.Drawing.Color.White;
            this.gridLevelID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLevelID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLevelID});
            this.gridLevelID.Location = new System.Drawing.Point(3, 3);
            this.gridLevelID.MultiSelect = false;
            this.gridLevelID.Name = "gridLevelID";
            this.gridLevelID.ReadOnly = true;
            this.gridLevelID.RowHeadersVisible = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            this.gridLevelID.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.gridLevelID.RowTemplate.Height = 23;
            this.gridLevelID.Size = new System.Drawing.Size(305, 492);
            this.gridLevelID.TabIndex = 0;
            this.gridLevelID.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridLevelID_CellClick);
            this.gridLevelID.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gridLevelID_CellPainting);
            // 
            // colLevelID
            // 
            this.colLevelID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colLevelID.DefaultCellStyle = dataGridViewCellStyle6;
            this.colLevelID.HeaderText = "직급별 전체";
            this.colLevelID.Name = "colLevelID";
            this.colLevelID.ReadOnly = true;
            // 
            // btnSelect
            // 
            this.btnSelect.CheckButton = false;
            this.btnSelect.CheckedBkgndImage = null;
            this.btnSelect.CheckedImage = null;
            this.btnSelect.ClickedBackgroundImage = null;
            this.btnSelect.ClickedImage = global::TeamEditor.Properties.Resources.@__COMMON_SelectClick;
            this.btnSelect.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnSelect.DisabledBkgndImage = null;
            this.btnSelect.DisabledImage = null;
            this.btnSelect.ID = -1;
            this.btnSelect.InitButtonWidth = 69;
            this.btnSelect.IsChecked = false;
            this.btnSelect.Location = new System.Drawing.Point(786, 561);
            this.btnSelect.MouseOverBkgndImage = null;
            this.btnSelect.MouseOverImage = global::TeamEditor.Properties.Resources.@__COMMON_SelectClick;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.NormalImage = global::TeamEditor.Properties.Resources.@__COMMON_Select;
            this.btnSelect.Owner = null;
            this.btnSelect.Size = new System.Drawing.Size(69, 37);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSelect.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSelect.ToolTipText = "";
            this.btnSelect.UseCustomImageRect = true;
            this.btnSelect.UseTextLocation = false;
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnClose
            // 
            this.btnClose.CheckButton = false;
            this.btnClose.CheckedBkgndImage = null;
            this.btnClose.CheckedImage = null;
            this.btnClose.ClickedBackgroundImage = null;
            this.btnClose.ClickedImage = global::TeamEditor.Properties.Resources.@__COMMON_CancelClick;
            this.btnClose.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnClose.DisabledBkgndImage = null;
            this.btnClose.DisabledImage = null;
            this.btnClose.ID = -1;
            this.btnClose.InitButtonWidth = 69;
            this.btnClose.IsChecked = false;
            this.btnClose.Location = new System.Drawing.Point(856, 561);
            this.btnClose.MouseOverBkgndImage = null;
            this.btnClose.MouseOverImage = global::TeamEditor.Properties.Resources.@__COMMON_CancelClick;
            this.btnClose.Name = "btnClose";
            this.btnClose.NormalImage = global::TeamEditor.Properties.Resources.@__COMMON_Cancel;
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(69, 37);
            this.btnClose.TabIndex = 5;
            this.btnClose.TextLocation = new System.Drawing.Point(0, 0);
            this.btnClose.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnClose.ToolTipText = "";
            this.btnClose.UseCustomImageRect = true;
            this.btnClose.UseTextLocation = false;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // picRegularTeam
            // 
            this.picRegularTeam.BackgroundImage = global::TeamEditor.Properties.Resources.@__SOPEDIT_Enable2;
            this.picRegularTeam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picRegularTeam.Location = new System.Drawing.Point(15, 7);
            this.picRegularTeam.Name = "picRegularTeam";
            this.picRegularTeam.Size = new System.Drawing.Size(26, 31);
            this.picRegularTeam.TabIndex = 6;
            this.picRegularTeam.TabStop = false;
            this.picRegularTeam.Tag = "radioRegularTeam";
            this.picRegularTeam.Click += new System.EventHandler(this.Select_Menu);
            // 
            // lblRegularTeam
            // 
            this.lblRegularTeam.AutoSize = true;
            this.lblRegularTeam.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRegularTeam.ForeColor = System.Drawing.Color.White;
            this.lblRegularTeam.Location = new System.Drawing.Point(39, 13);
            this.lblRegularTeam.Name = "lblRegularTeam";
            this.lblRegularTeam.Size = new System.Drawing.Size(88, 18);
            this.lblRegularTeam.TabIndex = 7;
            this.lblRegularTeam.Tag = "radioRegularTeam";
            this.lblRegularTeam.Text = "정규조직도";
            this.lblRegularTeam.Click += new System.EventHandler(this.Select_Menu);
            // 
            // picCompanyMember
            // 
            this.picCompanyMember.BackgroundImage = global::TeamEditor.Properties.Resources.@__SOPEDIT_Disable2;
            this.picCompanyMember.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picCompanyMember.Location = new System.Drawing.Point(138, 7);
            this.picCompanyMember.Name = "picCompanyMember";
            this.picCompanyMember.Size = new System.Drawing.Size(26, 31);
            this.picCompanyMember.TabIndex = 8;
            this.picCompanyMember.TabStop = false;
            this.picCompanyMember.Tag = "radioCompanyMember";
            this.picCompanyMember.Click += new System.EventHandler(this.Select_Menu);
            // 
            // lblCompanyMember
            // 
            this.lblCompanyMember.AutoSize = true;
            this.lblCompanyMember.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCompanyMember.ForeColor = System.Drawing.Color.White;
            this.lblCompanyMember.Location = new System.Drawing.Point(162, 13);
            this.lblCompanyMember.Name = "lblCompanyMember";
            this.lblCompanyMember.Size = new System.Drawing.Size(77, 18);
            this.lblCompanyMember.TabIndex = 9;
            this.lblCompanyMember.Tag = "radioCompanyMember";
            this.lblCompanyMember.Text = "정규 직원";
            this.lblCompanyMember.Click += new System.EventHandler(this.Select_Menu);
            // 
            // picExternalCompanyTeam
            // 
            this.picExternalCompanyTeam.BackgroundImage = global::TeamEditor.Properties.Resources.@__SOPEDIT_Disable2;
            this.picExternalCompanyTeam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picExternalCompanyTeam.Location = new System.Drawing.Point(249, 7);
            this.picExternalCompanyTeam.Name = "picExternalCompanyTeam";
            this.picExternalCompanyTeam.Size = new System.Drawing.Size(26, 31);
            this.picExternalCompanyTeam.TabIndex = 10;
            this.picExternalCompanyTeam.TabStop = false;
            this.picExternalCompanyTeam.Tag = "radioExternalCompanyTeam";
            this.picExternalCompanyTeam.Click += new System.EventHandler(this.Select_Menu);
            // 
            // lblExternalCompanyTeam
            // 
            this.lblExternalCompanyTeam.AutoSize = true;
            this.lblExternalCompanyTeam.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblExternalCompanyTeam.ForeColor = System.Drawing.Color.White;
            this.lblExternalCompanyTeam.Location = new System.Drawing.Point(273, 13);
            this.lblExternalCompanyTeam.Name = "lblExternalCompanyTeam";
            this.lblExternalCompanyTeam.Size = new System.Drawing.Size(72, 18);
            this.lblExternalCompanyTeam.TabIndex = 11;
            this.lblExternalCompanyTeam.Tag = "radioExternalCompanyTeam";
            this.lblExternalCompanyTeam.Text = "협력업체";
            this.lblExternalCompanyTeam.Click += new System.EventHandler(this.Select_Menu);
            // 
            // picExternalCompanyMember
            // 
            this.picExternalCompanyMember.BackgroundImage = global::TeamEditor.Properties.Resources.@__SOPEDIT_Disable2;
            this.picExternalCompanyMember.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picExternalCompanyMember.Location = new System.Drawing.Point(355, 7);
            this.picExternalCompanyMember.Name = "picExternalCompanyMember";
            this.picExternalCompanyMember.Size = new System.Drawing.Size(26, 31);
            this.picExternalCompanyMember.TabIndex = 12;
            this.picExternalCompanyMember.TabStop = false;
            this.picExternalCompanyMember.Tag = "radioExternalCompanyMember";
            this.picExternalCompanyMember.Click += new System.EventHandler(this.Select_Menu);
            // 
            // lblExternalCompanyMember
            // 
            this.lblExternalCompanyMember.AutoSize = true;
            this.lblExternalCompanyMember.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblExternalCompanyMember.ForeColor = System.Drawing.Color.White;
            this.lblExternalCompanyMember.Location = new System.Drawing.Point(381, 13);
            this.lblExternalCompanyMember.Name = "lblExternalCompanyMember";
            this.lblExternalCompanyMember.Size = new System.Drawing.Size(109, 18);
            this.lblExternalCompanyMember.TabIndex = 13;
            this.lblExternalCompanyMember.Tag = "radioExternalCompanyMember";
            this.lblExternalCompanyMember.Text = "협력업체 직원";
            this.lblExternalCompanyMember.Click += new System.EventHandler(this.Select_Menu);
            // 
            // picLevelID
            // 
            this.picLevelID.BackgroundImage = global::TeamEditor.Properties.Resources.@__SOPEDIT_Disable2;
            this.picLevelID.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picLevelID.Location = new System.Drawing.Point(502, 7);
            this.picLevelID.Name = "picLevelID";
            this.picLevelID.Size = new System.Drawing.Size(26, 31);
            this.picLevelID.TabIndex = 14;
            this.picLevelID.TabStop = false;
            this.picLevelID.Tag = "radioLevelID";
            this.picLevelID.Click += new System.EventHandler(this.Select_Menu);
            // 
            // lblLevelID
            // 
            this.lblLevelID.AutoSize = true;
            this.lblLevelID.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLevelID.ForeColor = System.Drawing.Color.White;
            this.lblLevelID.Location = new System.Drawing.Point(526, 13);
            this.lblLevelID.Name = "lblLevelID";
            this.lblLevelID.Size = new System.Drawing.Size(93, 18);
            this.lblLevelID.TabIndex = 15;
            this.lblLevelID.Tag = "radioLevelID";
            this.lblLevelID.Text = "직급별 전체";
            this.lblLevelID.Click += new System.EventHandler(this.Select_Menu);
            // 
            // picUserDefinedTeam
            // 
            this.picUserDefinedTeam.BackgroundImage = global::TeamEditor.Properties.Resources.@__SOPEDIT_Disable2;
            this.picUserDefinedTeam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picUserDefinedTeam.Location = new System.Drawing.Point(630, 7);
            this.picUserDefinedTeam.Name = "picUserDefinedTeam";
            this.picUserDefinedTeam.Size = new System.Drawing.Size(26, 31);
            this.picUserDefinedTeam.TabIndex = 16;
            this.picUserDefinedTeam.TabStop = false;
            this.picUserDefinedTeam.Tag = "radioUserDefinedTeam";
            this.picUserDefinedTeam.Click += new System.EventHandler(this.Select_Menu);
            // 
            // lblUserDefinedTeam
            // 
            this.lblUserDefinedTeam.AutoSize = true;
            this.lblUserDefinedTeam.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblUserDefinedTeam.ForeColor = System.Drawing.Color.White;
            this.lblUserDefinedTeam.Location = new System.Drawing.Point(654, 13);
            this.lblUserDefinedTeam.Name = "lblUserDefinedTeam";
            this.lblUserDefinedTeam.Size = new System.Drawing.Size(120, 18);
            this.lblUserDefinedTeam.TabIndex = 17;
            this.lblUserDefinedTeam.Tag = "radioUserDefinedTeam";
            this.lblUserDefinedTeam.Text = "사용자정의조직";
            this.lblUserDefinedTeam.Click += new System.EventHandler(this.Select_Menu);
            // 
            // FormSelectTemporaryMember
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(930, 607);
            this.Controls.Add(this.picUserDefinedTeam);
            this.Controls.Add(this.radioRegularTeam);
            this.Controls.Add(this.radioCompanyMember);
            this.Controls.Add(this.lblUserDefinedTeam);
            this.Controls.Add(this.radioExternalCompanyTeam);
            this.Controls.Add(this.picLevelID);
            this.Controls.Add(this.radioExternalCompanyMember);
            this.Controls.Add(this.lblLevelID);
            this.Controls.Add(this.radioLevelID);
            this.Controls.Add(this.picExternalCompanyMember);
            this.Controls.Add(this.radioUserDefinedTeam);
            this.Controls.Add(this.lblExternalCompanyMember);
            this.Controls.Add(this.picExternalCompanyTeam);
            this.Controls.Add(this.lblExternalCompanyTeam);
            this.Controls.Add(this.picCompanyMember);
            this.Controls.Add(this.lblCompanyMember);
            this.Controls.Add(this.picRegularTeam);
            this.Controls.Add(this.lblRegularTeam);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.panelForms);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectTemporaryMember";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "비상조직 담당자 설정";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSelectTemporaryMember_FormClosing);
            this.panelForms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLevelID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRegularTeam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCompanyMember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExternalCompanyTeam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExternalCompanyMember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLevelID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserDefinedTeam)).EndInit();
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
        private System.Windows.Forms.DataGridViewTextBoxColumn colLevelID;
        private UnE.GUI.RibbonButton btnSelect;
        private UnE.GUI.RibbonButton btnClose;
        private System.Windows.Forms.PictureBox picRegularTeam;
        private System.Windows.Forms.Label lblRegularTeam;
        private System.Windows.Forms.PictureBox picCompanyMember;
        private System.Windows.Forms.Label lblCompanyMember;
        private System.Windows.Forms.PictureBox picExternalCompanyTeam;
        private System.Windows.Forms.Label lblExternalCompanyTeam;
        private System.Windows.Forms.PictureBox picExternalCompanyMember;
        private System.Windows.Forms.Label lblExternalCompanyMember;
        private System.Windows.Forms.PictureBox picLevelID;
        private System.Windows.Forms.Label lblLevelID;
        private System.Windows.Forms.PictureBox picUserDefinedTeam;
        private System.Windows.Forms.Label lblUserDefinedTeam;
    }
}