namespace SOPManager.Popup
{
    partial class PopupSelectCommander
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupSelectCommander));
            this.labelTeamType = new System.Windows.Forms.Label();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.checkBoxDefault = new System.Windows.Forms.CheckBox();
            this.btnSelectTeam = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDisplay = new System.Windows.Forms.TextBox();
            this.labelFullPath = new System.Windows.Forms.Label();
            this.rbBtnExternal = new System.Windows.Forms.RadioButton();
            this.rbBtnUserDefine = new System.Windows.Forms.RadioButton();
            this.rbBtnEmergency = new System.Windows.Forms.RadioButton();
            this.rbBtnRegular = new System.Windows.Forms.RadioButton();
            this.dataGridViewExternal = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewUserDefined = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rbBtnControlRoom = new System.Windows.Forms.RadioButton();
            this.picRegular = new System.Windows.Forms.PictureBox();
            this.lblRegular = new System.Windows.Forms.Label();
            this.picEmergency = new System.Windows.Forms.PictureBox();
            this.lblEmergency = new System.Windows.Forms.Label();
            this.picExternal = new System.Windows.Forms.PictureBox();
            this.lblExternal = new System.Windows.Forms.Label();
            this.picUserDefine = new System.Windows.Forms.PictureBox();
            this.lblUserDefine = new System.Windows.Forms.Label();
            this.picControlRoom = new System.Windows.Forms.PictureBox();
            this.lblControlRoom = new System.Windows.Forms.Label();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.picDefault = new System.Windows.Forms.PictureBox();
            this.lblDefault = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewExternal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUserDefined)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRegular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEmergency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExternal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserDefine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picControlRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDefault)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTeamType
            // 
            this.labelTeamType.AutoSize = true;
            this.labelTeamType.BackColor = System.Drawing.Color.Transparent;
            this.labelTeamType.Font = new System.Drawing.Font(Program.prgFont, 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTeamType.ForeColor = System.Drawing.Color.White;
            this.labelTeamType.Location = new System.Drawing.Point(8, 56);
            this.labelTeamType.Name = "labelTeamType";
            this.labelTeamType.Size = new System.Drawing.Size(57, 18);
            this.labelTeamType.TabIndex = 8;
            this.labelTeamType.Text = "전체 팀";
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewTeam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            this.treeViewTeam.Location = new System.Drawing.Point(12, 83);
            this.treeViewTeam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(738, 380);
            this.treeViewTeam.TabIndex = 7;
            this.treeViewTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterSelect);
            // 
            // checkBoxDefault
            // 
            this.checkBoxDefault.AutoSize = true;
            this.checkBoxDefault.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxDefault.ForeColor = System.Drawing.Color.White;
            this.checkBoxDefault.Location = new System.Drawing.Point(349, 548);
            this.checkBoxDefault.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBoxDefault.Name = "checkBoxDefault";
            this.checkBoxDefault.Size = new System.Drawing.Size(223, 21);
            this.checkBoxDefault.TabIndex = 11;
            this.checkBoxDefault.Text = "SOP 제어권 가진곳의 책임자";
            this.checkBoxDefault.UseMnemonic = false;
            this.checkBoxDefault.UseVisualStyleBackColor = true;
            this.checkBoxDefault.Visible = false;
            this.checkBoxDefault.CheckedChanged += new System.EventHandler(this.checkBoxDefault_CheckedChanged);
            this.checkBoxDefault.VisibleChanged += new System.EventHandler(this.checkBoxDefault_VisibleChanged);
            // 
            // btnSelectTeam
            // 
            this.btnSelectTeam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectTeam.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectTeam.Location = new System.Drawing.Point(664, 45);
            this.btnSelectTeam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSelectTeam.Name = "btnSelectTeam";
            this.btnSelectTeam.Size = new System.Drawing.Size(86, 29);
            this.btnSelectTeam.TabIndex = 12;
            this.btnSelectTeam.Text = "조직변경";
            this.btnSelectTeam.UseVisualStyleBackColor = true;
            this.btnSelectTeam.Visible = false;
            this.btnSelectTeam.Click += new System.EventHandler(this.btnSelectTeam_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(16, 546);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 18);
            this.label1.TabIndex = 13;
            this.label1.Text = "표시이름 :";
            // 
            // textBoxDisplay
            // 
            this.textBoxDisplay.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDisplay.Location = new System.Drawing.Point(95, 542);
            this.textBoxDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxDisplay.Name = "textBoxDisplay";
            this.textBoxDisplay.Size = new System.Drawing.Size(256, 26);
            this.textBoxDisplay.TabIndex = 14;
            // 
            // labelFullPath
            // 
            this.labelFullPath.Font = new System.Drawing.Font(Program.prgFont, 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFullPath.ForeColor = System.Drawing.Color.White;
            this.labelFullPath.Location = new System.Drawing.Point(16, 515);
            this.labelFullPath.Name = "labelFullPath";
            this.labelFullPath.Size = new System.Drawing.Size(738, 20);
            this.labelFullPath.TabIndex = 15;
            this.labelFullPath.Text = "전체경로";
            this.labelFullPath.Visible = false;
            // 
            // rbBtnExternal
            // 
            this.rbBtnExternal.AutoSize = true;
            this.rbBtnExternal.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnExternal.ForeColor = System.Drawing.Color.White;
            this.rbBtnExternal.Location = new System.Drawing.Point(349, 548);
            this.rbBtnExternal.Name = "rbBtnExternal";
            this.rbBtnExternal.Size = new System.Drawing.Size(86, 21);
            this.rbBtnExternal.TabIndex = 21;
            this.rbBtnExternal.Text = "외부조직";
            this.rbBtnExternal.UseVisualStyleBackColor = true;
            this.rbBtnExternal.Visible = false;
            this.rbBtnExternal.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnUserDefine
            // 
            this.rbBtnUserDefine.AutoSize = true;
            this.rbBtnUserDefine.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnUserDefine.ForeColor = System.Drawing.Color.White;
            this.rbBtnUserDefine.Location = new System.Drawing.Point(443, 495);
            this.rbBtnUserDefine.Name = "rbBtnUserDefine";
            this.rbBtnUserDefine.Size = new System.Drawing.Size(131, 21);
            this.rbBtnUserDefine.TabIndex = 20;
            this.rbBtnUserDefine.Text = "사용자정의조직";
            this.rbBtnUserDefine.UseVisualStyleBackColor = true;
            this.rbBtnUserDefine.Visible = false;
            this.rbBtnUserDefine.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnEmergency
            // 
            this.rbBtnEmergency.AutoSize = true;
            this.rbBtnEmergency.Checked = true;
            this.rbBtnEmergency.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnEmergency.ForeColor = System.Drawing.Color.White;
            this.rbBtnEmergency.Location = new System.Drawing.Point(349, 524);
            this.rbBtnEmergency.Name = "rbBtnEmergency";
            this.rbBtnEmergency.Size = new System.Drawing.Size(86, 21);
            this.rbBtnEmergency.TabIndex = 19;
            this.rbBtnEmergency.TabStop = true;
            this.rbBtnEmergency.Text = "비상조직";
            this.rbBtnEmergency.UseVisualStyleBackColor = true;
            this.rbBtnEmergency.Visible = false;
            this.rbBtnEmergency.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnRegular
            // 
            this.rbBtnRegular.AutoSize = true;
            this.rbBtnRegular.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnRegular.ForeColor = System.Drawing.Color.White;
            this.rbBtnRegular.Location = new System.Drawing.Point(349, 495);
            this.rbBtnRegular.Name = "rbBtnRegular";
            this.rbBtnRegular.Size = new System.Drawing.Size(86, 21);
            this.rbBtnRegular.TabIndex = 18;
            this.rbBtnRegular.Text = "정규조직";
            this.rbBtnRegular.UseVisualStyleBackColor = true;
            this.rbBtnRegular.Visible = false;
            this.rbBtnRegular.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // dataGridViewExternal
            // 
            this.dataGridViewExternal.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewExternal.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewExternal.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewExternal.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewExternal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewExternal.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn4,
            this.Column2,
            this.Column4});
            this.dataGridViewExternal.GridColor = System.Drawing.Color.Black;
            this.dataGridViewExternal.Location = new System.Drawing.Point(12, 83);
            this.dataGridViewExternal.MultiSelect = false;
            this.dataGridViewExternal.Name = "dataGridViewExternal";
            this.dataGridViewExternal.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewExternal.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewExternal.RowTemplate.Height = 23;
            this.dataGridViewExternal.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewExternal.Size = new System.Drawing.Size(738, 380);
            this.dataGridViewExternal.TabIndex = 22;
            this.dataGridViewExternal.Visible = false;
            this.dataGridViewExternal.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewExternal_CellEndEdit);
            this.dataGridViewExternal.SelectionChanged += new System.EventHandler(this.dataGridViewExternal_SelectionChanged);
            this.dataGridViewExternal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewExternal_KeyDown);
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "수신처";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 300;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "전화번호";
            this.Column2.Name = "Column2";
            this.Column2.Width = 210;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "팩스번호";
            this.Column4.Name = "Column4";
            // 
            // dataGridViewUserDefined
            // 
            this.dataGridViewUserDefined.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewUserDefined.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewUserDefined.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewUserDefined.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewUserDefined.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUserDefined.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            this.dataGridViewUserDefined.GridColor = System.Drawing.Color.Black;
            this.dataGridViewUserDefined.Location = new System.Drawing.Point(12, 83);
            this.dataGridViewUserDefined.MultiSelect = false;
            this.dataGridViewUserDefined.Name = "dataGridViewUserDefined";
            this.dataGridViewUserDefined.RowHeadersVisible = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewUserDefined.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewUserDefined.RowTemplate.Height = 23;
            this.dataGridViewUserDefined.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewUserDefined.Size = new System.Drawing.Size(738, 397);
            this.dataGridViewUserDefined.TabIndex = 23;
            this.dataGridViewUserDefined.Visible = false;
            this.dataGridViewUserDefined.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUserDefined_CellEndEdit);
            this.dataGridViewUserDefined.SelectionChanged += new System.EventHandler(this.dataGridViewUserDefined_SelectionChanged);
            this.dataGridViewUserDefined.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewUserDefined_KeyDown);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "수신처";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 300;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "전화번호";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 210;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.HeaderText = "팩스번호";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // rbBtnControlRoom
            // 
            this.rbBtnControlRoom.AutoSize = true;
            this.rbBtnControlRoom.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnControlRoom.ForeColor = System.Drawing.Color.White;
            this.rbBtnControlRoom.Location = new System.Drawing.Point(443, 517);
            this.rbBtnControlRoom.Name = "rbBtnControlRoom";
            this.rbBtnControlRoom.Size = new System.Drawing.Size(101, 21);
            this.rbBtnControlRoom.TabIndex = 24;
            this.rbBtnControlRoom.Text = "교대근무자";
            this.rbBtnControlRoom.UseVisualStyleBackColor = true;
            this.rbBtnControlRoom.Visible = false;
            this.rbBtnControlRoom.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // picRegular
            // 
            this.picRegular.BackColor = System.Drawing.Color.Transparent;
            this.picRegular.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picRegular.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picRegular.Location = new System.Drawing.Point(3, 1);
            this.picRegular.Margin = new System.Windows.Forms.Padding(3, 1, 0, 3);
            this.picRegular.Name = "picRegular";
            this.picRegular.Size = new System.Drawing.Size(22, 22);
            this.picRegular.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picRegular.TabIndex = 84;
            this.picRegular.TabStop = false;
            this.picRegular.Click += new System.EventHandler(this.Regular_Click);
            // 
            // lblRegular
            // 
            this.lblRegular.AutoSize = true;
            this.lblRegular.BackColor = System.Drawing.Color.Transparent;
            this.lblRegular.Font = new System.Drawing.Font(Program.prgFont, 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRegular.ForeColor = System.Drawing.Color.White;
            this.lblRegular.Location = new System.Drawing.Point(25, 3);
            this.lblRegular.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.lblRegular.Name = "lblRegular";
            this.lblRegular.Size = new System.Drawing.Size(68, 18);
            this.lblRegular.TabIndex = 83;
            this.lblRegular.Text = "정규조직";
            this.lblRegular.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRegular.Click += new System.EventHandler(this.Regular_Click);
            // 
            // picEmergency
            // 
            this.picEmergency.BackColor = System.Drawing.Color.Transparent;
            this.picEmergency.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picEmergency.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picEmergency.Location = new System.Drawing.Point(99, 1);
            this.picEmergency.Margin = new System.Windows.Forms.Padding(3, 1, 0, 3);
            this.picEmergency.Name = "picEmergency";
            this.picEmergency.Size = new System.Drawing.Size(22, 22);
            this.picEmergency.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picEmergency.TabIndex = 86;
            this.picEmergency.TabStop = false;
            this.picEmergency.Click += new System.EventHandler(this.Emergency_Click);
            // 
            // lblEmergency
            // 
            this.lblEmergency.AutoSize = true;
            this.lblEmergency.BackColor = System.Drawing.Color.Transparent;
            this.lblEmergency.Font = new System.Drawing.Font(Program.prgFont, 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblEmergency.ForeColor = System.Drawing.Color.White;
            this.lblEmergency.Location = new System.Drawing.Point(121, 3);
            this.lblEmergency.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.lblEmergency.Name = "lblEmergency";
            this.lblEmergency.Size = new System.Drawing.Size(68, 18);
            this.lblEmergency.TabIndex = 85;
            this.lblEmergency.Text = "비상조직";
            this.lblEmergency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEmergency.Click += new System.EventHandler(this.Emergency_Click);
            // 
            // picExternal
            // 
            this.picExternal.BackColor = System.Drawing.Color.Transparent;
            this.picExternal.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picExternal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picExternal.Location = new System.Drawing.Point(195, 1);
            this.picExternal.Margin = new System.Windows.Forms.Padding(3, 1, 0, 3);
            this.picExternal.Name = "picExternal";
            this.picExternal.Size = new System.Drawing.Size(22, 22);
            this.picExternal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picExternal.TabIndex = 88;
            this.picExternal.TabStop = false;
            this.picExternal.Click += new System.EventHandler(this.External_Click);
            // 
            // lblExternal
            // 
            this.lblExternal.AutoSize = true;
            this.lblExternal.BackColor = System.Drawing.Color.Transparent;
            this.lblExternal.Font = new System.Drawing.Font(Program.prgFont, 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblExternal.ForeColor = System.Drawing.Color.White;
            this.lblExternal.Location = new System.Drawing.Point(217, 3);
            this.lblExternal.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.lblExternal.Name = "lblExternal";
            this.lblExternal.Size = new System.Drawing.Size(68, 18);
            this.lblExternal.TabIndex = 87;
            this.lblExternal.Text = "외부조직";
            this.lblExternal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblExternal.Click += new System.EventHandler(this.External_Click);
            // 
            // picUserDefine
            // 
            this.picUserDefine.BackColor = System.Drawing.Color.Transparent;
            this.picUserDefine.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picUserDefine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picUserDefine.Location = new System.Drawing.Point(291, 1);
            this.picUserDefine.Margin = new System.Windows.Forms.Padding(3, 1, 0, 3);
            this.picUserDefine.Name = "picUserDefine";
            this.picUserDefine.Size = new System.Drawing.Size(22, 22);
            this.picUserDefine.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picUserDefine.TabIndex = 90;
            this.picUserDefine.TabStop = false;
            this.picUserDefine.Click += new System.EventHandler(this.UserDefine_Click);
            // 
            // lblUserDefine
            // 
            this.lblUserDefine.AutoSize = true;
            this.lblUserDefine.BackColor = System.Drawing.Color.Transparent;
            this.lblUserDefine.Font = new System.Drawing.Font(Program.prgFont, 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblUserDefine.ForeColor = System.Drawing.Color.White;
            this.lblUserDefine.Location = new System.Drawing.Point(313, 3);
            this.lblUserDefine.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.lblUserDefine.Name = "lblUserDefine";
            this.lblUserDefine.Size = new System.Drawing.Size(113, 18);
            this.lblUserDefine.TabIndex = 89;
            this.lblUserDefine.Text = "사용자정의조직";
            this.lblUserDefine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblUserDefine.Click += new System.EventHandler(this.UserDefine_Click);
            // 
            // picControlRoom
            // 
            this.picControlRoom.BackColor = System.Drawing.Color.Transparent;
            this.picControlRoom.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picControlRoom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picControlRoom.Location = new System.Drawing.Point(432, 1);
            this.picControlRoom.Margin = new System.Windows.Forms.Padding(3, 1, 0, 3);
            this.picControlRoom.Name = "picControlRoom";
            this.picControlRoom.Size = new System.Drawing.Size(22, 22);
            this.picControlRoom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picControlRoom.TabIndex = 92;
            this.picControlRoom.TabStop = false;
            this.picControlRoom.Click += new System.EventHandler(this.ControlRoom_Click);
            // 
            // lblControlRoom
            // 
            this.lblControlRoom.AutoSize = true;
            this.lblControlRoom.BackColor = System.Drawing.Color.Transparent;
            this.lblControlRoom.Font = new System.Drawing.Font(Program.prgFont, 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblControlRoom.ForeColor = System.Drawing.Color.White;
            this.lblControlRoom.Location = new System.Drawing.Point(454, 3);
            this.lblControlRoom.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.lblControlRoom.Name = "lblControlRoom";
            this.lblControlRoom.Size = new System.Drawing.Size(83, 18);
            this.lblControlRoom.TabIndex = 91;
            this.lblControlRoom.Text = "교대근무자";
            this.lblControlRoom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblControlRoom.Click += new System.EventHandler(this.ControlRoom_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 69;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(685, 536);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(69, 37);
            this.btnCancel.TabIndex = 95;
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseCustomImageRect = true;
            this.btnCancel.UseTextLocation = false;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 69;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(617, 536);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 94;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // picDefault
            // 
            this.picDefault.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_disable;
            this.picDefault.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picDefault.Location = new System.Drawing.Point(17, 486);
            this.picDefault.Name = "picDefault";
            this.picDefault.Size = new System.Drawing.Size(18, 18);
            this.picDefault.TabIndex = 109;
            this.picDefault.TabStop = false;
            this.picDefault.Click += new System.EventHandler(this.Default_Click);
            // 
            // lblDefault
            // 
            this.lblDefault.AutoSize = true;
            this.lblDefault.Font = new System.Drawing.Font(Program.prgFont, 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDefault.ForeColor = System.Drawing.Color.White;
            this.lblDefault.Location = new System.Drawing.Point(41, 487);
            this.lblDefault.Name = "lblDefault";
            this.lblDefault.Size = new System.Drawing.Size(202, 18);
            this.lblDefault.TabIndex = 110;
            this.lblDefault.Text = "SOP 제어권 가진곳의 책임자";
            this.lblDefault.Click += new System.EventHandler(this.Default_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.picRegular);
            this.flowLayoutPanel1.Controls.Add(this.lblRegular);
            this.flowLayoutPanel1.Controls.Add(this.picEmergency);
            this.flowLayoutPanel1.Controls.Add(this.lblEmergency);
            this.flowLayoutPanel1.Controls.Add(this.picExternal);
            this.flowLayoutPanel1.Controls.Add(this.lblExternal);
            this.flowLayoutPanel1.Controls.Add(this.picUserDefine);
            this.flowLayoutPanel1.Controls.Add(this.lblUserDefine);
            this.flowLayoutPanel1.Controls.Add(this.picControlRoom);
            this.flowLayoutPanel1.Controls.Add(this.lblControlRoom);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(9, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(741, 33);
            this.flowLayoutPanel1.TabIndex = 111;
            // 
            // PopupSelectCommander
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(762, 577);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.picDefault);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rbBtnControlRoom);
            this.Controls.Add(this.rbBtnExternal);
            this.Controls.Add(this.rbBtnUserDefine);
            this.Controls.Add(this.rbBtnEmergency);
            this.Controls.Add(this.rbBtnRegular);
            this.Controls.Add(this.labelFullPath);
            this.Controls.Add(this.textBoxDisplay);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelectTeam);
            this.Controls.Add(this.labelTeamType);
            this.Controls.Add(this.lblDefault);
            this.Controls.Add(this.checkBoxDefault);
            this.Controls.Add(this.dataGridViewUserDefined);
            this.Controls.Add(this.dataGridViewExternal);
            this.Controls.Add(this.treeViewTeam);
            this.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(762, 500);
            this.Name = "PopupSelectCommander";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PopupSelectTeam4";
            this.Load += new System.EventHandler(this.PopupSelectTeam4_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewExternal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUserDefined)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRegular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEmergency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExternal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserDefine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picControlRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDefault)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTeamType;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.CheckBox checkBoxDefault;
        private System.Windows.Forms.Button btnSelectTeam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDisplay;
        private System.Windows.Forms.Label labelFullPath;
        private System.Windows.Forms.RadioButton rbBtnExternal;
        private System.Windows.Forms.RadioButton rbBtnUserDefine;
        private System.Windows.Forms.RadioButton rbBtnEmergency;
        private System.Windows.Forms.RadioButton rbBtnRegular;
        private System.Windows.Forms.DataGridView dataGridViewExternal;
        private System.Windows.Forms.DataGridView dataGridViewUserDefined;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.RadioButton rbBtnControlRoom;
        private System.Windows.Forms.PictureBox picRegular;
        private System.Windows.Forms.Label lblRegular;
        private System.Windows.Forms.PictureBox picEmergency;
        private System.Windows.Forms.Label lblEmergency;
        private System.Windows.Forms.PictureBox picExternal;
        private System.Windows.Forms.Label lblExternal;
        private System.Windows.Forms.PictureBox picUserDefine;
        private System.Windows.Forms.Label lblUserDefine;
        private System.Windows.Forms.PictureBox picControlRoom;
        private System.Windows.Forms.Label lblControlRoom;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnOK;
        private System.Windows.Forms.PictureBox picDefault;
        private System.Windows.Forms.Label lblDefault;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}