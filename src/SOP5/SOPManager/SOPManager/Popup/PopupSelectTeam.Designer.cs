namespace SOPManager
{
    partial class PopupSelectTeam
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupSelectTeam));
            this.rbBtnControlRoom = new System.Windows.Forms.RadioButton();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.rtextSearch = new System.Windows.Forms.RichTextBox();
            this.pictureBoxSearch = new System.Windows.Forms.PictureBox();
            this.rbBtnExternal = new System.Windows.Forms.RadioButton();
            this.rbBtnUserDefine = new System.Windows.Forms.RadioButton();
            this.rbBtnEmergency = new System.Windows.Forms.RadioButton();
            this.rbBtnRegular = new System.Windows.Forms.RadioButton();
            this.btnChangeTeam = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.labelTeamType = new System.Windows.Forms.Label();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.dataGridViewUserDefined = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxDefault = new System.Windows.Forms.CheckBox();
            this.labelFullPath = new System.Windows.Forms.Label();
            this.textBoxDisplay = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.picRegular = new System.Windows.Forms.PictureBox();
            this.lblRegular = new System.Windows.Forms.Label();
            this.picEmergency = new System.Windows.Forms.PictureBox();
            this.lblEmergency = new System.Windows.Forms.Label();
            this.picExternal = new System.Windows.Forms.PictureBox();
            this.picControlRoom = new System.Windows.Forms.PictureBox();
            this.lblControlRoom = new System.Windows.Forms.Label();
            this.picUserDefine = new System.Windows.Forms.PictureBox();
            this.lblUserDefine = new System.Windows.Forms.Label();
            this.lblExternal = new System.Windows.Forms.Label();
            this.checkBoxIncludeChildTeams = new System.Windows.Forms.CheckBox();
            this.picChildTeams = new System.Windows.Forms.PictureBox();
            this.lblChildTeams = new System.Windows.Forms.Label();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.btnAdd = new UnE.GUI.RibbonButton();
            this.btnDel = new UnE.GUI.RibbonButton();
            this.picDefault = new System.Windows.Forms.PictureBox();
            this.lblDefault = new System.Windows.Forms.Label();
            this.panelSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUserDefined)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRegular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEmergency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExternal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picControlRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserDefine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picChildTeams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDefault)).BeginInit();
            this.SuspendLayout();
            // 
            // rbBtnControlRoom
            // 
            this.rbBtnControlRoom.AutoSize = true;
            this.rbBtnControlRoom.BackColor = System.Drawing.Color.Transparent;
            this.rbBtnControlRoom.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnControlRoom.ForeColor = System.Drawing.Color.White;
            this.rbBtnControlRoom.Location = new System.Drawing.Point(544, 515);
            this.rbBtnControlRoom.Name = "rbBtnControlRoom";
            this.rbBtnControlRoom.Size = new System.Drawing.Size(90, 17);
            this.rbBtnControlRoom.TabIndex = 23;
            this.rbBtnControlRoom.Text = "교대근무자";
            this.rbBtnControlRoom.UseVisualStyleBackColor = false;
            this.rbBtnControlRoom.Visible = false;
            this.rbBtnControlRoom.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.Color.White;
            this.panelSearch.Controls.Add(this.rtextSearch);
            this.panelSearch.Controls.Add(this.pictureBoxSearch);
            this.panelSearch.Location = new System.Drawing.Point(12, 65);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(466, 26);
            this.panelSearch.TabIndex = 22;
            // 
            // rtextSearch
            // 
            this.rtextSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtextSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtextSearch.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rtextSearch.Location = new System.Drawing.Point(31, 2);
            this.rtextSearch.Multiline = false;
            this.rtextSearch.Name = "rtextSearch";
            this.rtextSearch.Size = new System.Drawing.Size(432, 24);
            this.rtextSearch.TabIndex = 1;
            this.rtextSearch.Text = "";
            // 
            // pictureBoxSearch
            // 
            this.pictureBoxSearch.BackColor = System.Drawing.Color.White;
            this.pictureBoxSearch.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_Search;
            this.pictureBoxSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxSearch.Location = new System.Drawing.Point(1, 1);
            this.pictureBoxSearch.Name = "pictureBoxSearch";
            this.pictureBoxSearch.Size = new System.Drawing.Size(24, 24);
            this.pictureBoxSearch.TabIndex = 0;
            this.pictureBoxSearch.TabStop = false;
            // 
            // rbBtnExternal
            // 
            this.rbBtnExternal.AutoSize = true;
            this.rbBtnExternal.BackColor = System.Drawing.Color.Transparent;
            this.rbBtnExternal.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnExternal.ForeColor = System.Drawing.Color.White;
            this.rbBtnExternal.Location = new System.Drawing.Point(273, 515);
            this.rbBtnExternal.Name = "rbBtnExternal";
            this.rbBtnExternal.Size = new System.Drawing.Size(77, 17);
            this.rbBtnExternal.TabIndex = 3;
            this.rbBtnExternal.Text = "외부조직";
            this.rbBtnExternal.UseVisualStyleBackColor = false;
            this.rbBtnExternal.Visible = false;
            this.rbBtnExternal.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnUserDefine
            // 
            this.rbBtnUserDefine.AutoSize = true;
            this.rbBtnUserDefine.BackColor = System.Drawing.Color.Transparent;
            this.rbBtnUserDefine.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnUserDefine.ForeColor = System.Drawing.Color.White;
            this.rbBtnUserDefine.Location = new System.Drawing.Point(388, 515);
            this.rbBtnUserDefine.Name = "rbBtnUserDefine";
            this.rbBtnUserDefine.Size = new System.Drawing.Size(116, 17);
            this.rbBtnUserDefine.TabIndex = 2;
            this.rbBtnUserDefine.Text = "사용자정의조직";
            this.rbBtnUserDefine.UseVisualStyleBackColor = false;
            this.rbBtnUserDefine.Visible = false;
            this.rbBtnUserDefine.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnEmergency
            // 
            this.rbBtnEmergency.AutoSize = true;
            this.rbBtnEmergency.BackColor = System.Drawing.Color.Transparent;
            this.rbBtnEmergency.Checked = true;
            this.rbBtnEmergency.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnEmergency.ForeColor = System.Drawing.Color.White;
            this.rbBtnEmergency.Location = new System.Drawing.Point(388, 491);
            this.rbBtnEmergency.Name = "rbBtnEmergency";
            this.rbBtnEmergency.Size = new System.Drawing.Size(77, 17);
            this.rbBtnEmergency.TabIndex = 1;
            this.rbBtnEmergency.TabStop = true;
            this.rbBtnEmergency.Text = "비상조직";
            this.rbBtnEmergency.UseVisualStyleBackColor = false;
            this.rbBtnEmergency.Visible = false;
            this.rbBtnEmergency.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnRegular
            // 
            this.rbBtnRegular.AutoSize = true;
            this.rbBtnRegular.BackColor = System.Drawing.Color.Transparent;
            this.rbBtnRegular.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnRegular.ForeColor = System.Drawing.Color.White;
            this.rbBtnRegular.Location = new System.Drawing.Point(273, 491);
            this.rbBtnRegular.Name = "rbBtnRegular";
            this.rbBtnRegular.Size = new System.Drawing.Size(77, 17);
            this.rbBtnRegular.TabIndex = 0;
            this.rbBtnRegular.Text = "정규조직";
            this.rbBtnRegular.UseVisualStyleBackColor = false;
            this.rbBtnRegular.Visible = false;
            this.rbBtnRegular.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // btnChangeTeam
            // 
            this.btnChangeTeam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnChangeTeam.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnChangeTeam.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnChangeTeam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeTeam.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChangeTeam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.btnChangeTeam.Location = new System.Drawing.Point(755, 15);
            this.btnChangeTeam.Name = "btnChangeTeam";
            this.btnChangeTeam.Size = new System.Drawing.Size(110, 31);
            this.btnChangeTeam.TabIndex = 21;
            this.btnChangeTeam.Text = "조직변경";
            this.btnChangeTeam.UseVisualStyleBackColor = false;
            this.btnChangeTeam.Visible = false;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.ColumnHeadersVisible = false;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.GridColor = System.Drawing.Color.Black;
            this.dataGridView.Location = new System.Drawing.Point(587, 120);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(285, 326);
            this.dataGridView.TabIndex = 7;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(582, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "선택한 팀";
            // 
            // labelTeamType
            // 
            this.labelTeamType.AutoSize = true;
            this.labelTeamType.BackColor = System.Drawing.Color.Transparent;
            this.labelTeamType.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTeamType.ForeColor = System.Drawing.Color.White;
            this.labelTeamType.Location = new System.Drawing.Point(9, 100);
            this.labelTeamType.Name = "labelTeamType";
            this.labelTeamType.Size = new System.Drawing.Size(68, 17);
            this.labelTeamType.TabIndex = 7;
            this.labelTeamType.Text = "전체 팀";
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.BackColor = System.Drawing.Color.White;
            this.treeViewTeam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewTeam.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewTeam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            this.treeViewTeam.Location = new System.Drawing.Point(12, 120);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(466, 326);
            this.treeViewTeam.TabIndex = 4;
            // 
            // dataGridViewUserDefined
            // 
            this.dataGridViewUserDefined.AllowUserToDeleteRows = false;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewUserDefined.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewUserDefined.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewUserDefined.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewUserDefined.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUserDefined.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewUserDefined.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewUserDefined.GridColor = System.Drawing.Color.Black;
            this.dataGridViewUserDefined.Location = new System.Drawing.Point(12, 120);
            this.dataGridViewUserDefined.Name = "dataGridViewUserDefined";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewUserDefined.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewUserDefined.RowHeadersVisible = false;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewUserDefined.RowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewUserDefined.RowTemplate.Height = 23;
            this.dataGridViewUserDefined.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewUserDefined.Size = new System.Drawing.Size(466, 326);
            this.dataGridViewUserDefined.TabIndex = 6;
            this.dataGridViewUserDefined.Visible = false;
            this.dataGridViewUserDefined.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUserDefined_CellEndEdit);
            this.dataGridViewUserDefined.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewUserDefined_KeyDown);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "수신처";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 160;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "전화번호";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 140;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.HeaderText = "팩스번호";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // checkBoxDefault
            // 
            this.checkBoxDefault.AutoSize = true;
            this.checkBoxDefault.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDefault.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxDefault.ForeColor = System.Drawing.Color.White;
            this.checkBoxDefault.Location = new System.Drawing.Point(15, 453);
            this.checkBoxDefault.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBoxDefault.Name = "checkBoxDefault";
            this.checkBoxDefault.Size = new System.Drawing.Size(196, 17);
            this.checkBoxDefault.TabIndex = 12;
            this.checkBoxDefault.Text = "SOP 제어권 가진곳의 책임자";
            this.checkBoxDefault.UseVisualStyleBackColor = false;
            this.checkBoxDefault.Visible = false;
            this.checkBoxDefault.CheckedChanged += new System.EventHandler(this.checkBoxDefault_CheckedChanged);
            this.checkBoxDefault.VisibleChanged += new System.EventHandler(this.checkBoxDefault_VisibleChanged);
            // 
            // labelFullPath
            // 
            this.labelFullPath.BackColor = System.Drawing.Color.Transparent;
            this.labelFullPath.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFullPath.ForeColor = System.Drawing.Color.White;
            this.labelFullPath.Location = new System.Drawing.Point(9, 481);
            this.labelFullPath.Name = "labelFullPath";
            this.labelFullPath.Size = new System.Drawing.Size(718, 20);
            this.labelFullPath.TabIndex = 16;
            this.labelFullPath.Text = "전체경로";
            this.labelFullPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelFullPath.Visible = false;
            // 
            // textBoxDisplay
            // 
            this.textBoxDisplay.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDisplay.Location = new System.Drawing.Point(89, 513);
            this.textBoxDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxDisplay.Name = "textBoxDisplay";
            this.textBoxDisplay.Size = new System.Drawing.Size(187, 22);
            this.textBoxDisplay.TabIndex = 18;
            this.textBoxDisplay.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 515);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "표시이름 :";
            this.label1.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.picRegular);
            this.panel1.Controls.Add(this.lblRegular);
            this.panel1.Controls.Add(this.picEmergency);
            this.panel1.Controls.Add(this.lblEmergency);
            this.panel1.Controls.Add(this.picExternal);
            this.panel1.Controls.Add(this.picControlRoom);
            this.panel1.Controls.Add(this.lblControlRoom);
            this.panel1.Controls.Add(this.picUserDefine);
            this.panel1.Controls.Add(this.lblUserDefine);
            this.panel1.Controls.Add(this.lblExternal);
            this.panel1.Location = new System.Drawing.Point(12, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(466, 50);
            this.panel1.TabIndex = 24;
            // 
            // picRegular
            // 
            this.picRegular.BackColor = System.Drawing.Color.Transparent;
            this.picRegular.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picRegular.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picRegular.Location = new System.Drawing.Point(3, 0);
            this.picRegular.Margin = new System.Windows.Forms.Padding(0);
            this.picRegular.Name = "picRegular";
            this.picRegular.Size = new System.Drawing.Size(22, 22);
            this.picRegular.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picRegular.TabIndex = 94;
            this.picRegular.TabStop = false;
            this.picRegular.Click += new System.EventHandler(this.Regular_Click);
            // 
            // lblRegular
            // 
            this.lblRegular.AutoSize = true;
            this.lblRegular.BackColor = System.Drawing.Color.Transparent;
            this.lblRegular.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRegular.ForeColor = System.Drawing.Color.White;
            this.lblRegular.Location = new System.Drawing.Point(25, 3);
            this.lblRegular.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegular.Name = "lblRegular";
            this.lblRegular.Size = new System.Drawing.Size(80, 17);
            this.lblRegular.TabIndex = 93;
            this.lblRegular.Text = "정규조직";
            this.lblRegular.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRegular.Click += new System.EventHandler(this.Regular_Click);
            // 
            // picEmergency
            // 
            this.picEmergency.BackColor = System.Drawing.Color.Transparent;
            this.picEmergency.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picEmergency.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picEmergency.Location = new System.Drawing.Point(131, 0);
            this.picEmergency.Margin = new System.Windows.Forms.Padding(0);
            this.picEmergency.Name = "picEmergency";
            this.picEmergency.Size = new System.Drawing.Size(22, 22);
            this.picEmergency.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picEmergency.TabIndex = 96;
            this.picEmergency.TabStop = false;
            this.picEmergency.Click += new System.EventHandler(this.Emergency_Click);
            // 
            // lblEmergency
            // 
            this.lblEmergency.AutoSize = true;
            this.lblEmergency.BackColor = System.Drawing.Color.Transparent;
            this.lblEmergency.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblEmergency.ForeColor = System.Drawing.Color.White;
            this.lblEmergency.Location = new System.Drawing.Point(153, 3);
            this.lblEmergency.Margin = new System.Windows.Forms.Padding(0);
            this.lblEmergency.Name = "lblEmergency";
            this.lblEmergency.Size = new System.Drawing.Size(80, 17);
            this.lblEmergency.TabIndex = 95;
            this.lblEmergency.Text = "비상조직";
            this.lblEmergency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEmergency.Click += new System.EventHandler(this.Emergency_Click);
            // 
            // picExternal
            // 
            this.picExternal.BackColor = System.Drawing.Color.Transparent;
            this.picExternal.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picExternal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picExternal.Location = new System.Drawing.Point(3, 28);
            this.picExternal.Margin = new System.Windows.Forms.Padding(0);
            this.picExternal.Name = "picExternal";
            this.picExternal.Size = new System.Drawing.Size(22, 22);
            this.picExternal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picExternal.TabIndex = 98;
            this.picExternal.TabStop = false;
            this.picExternal.Click += new System.EventHandler(this.External_Click);
            // 
            // picControlRoom
            // 
            this.picControlRoom.BackColor = System.Drawing.Color.Transparent;
            this.picControlRoom.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picControlRoom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picControlRoom.Location = new System.Drawing.Point(305, 28);
            this.picControlRoom.Margin = new System.Windows.Forms.Padding(0);
            this.picControlRoom.Name = "picControlRoom";
            this.picControlRoom.Size = new System.Drawing.Size(22, 22);
            this.picControlRoom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picControlRoom.TabIndex = 102;
            this.picControlRoom.TabStop = false;
            this.picControlRoom.Click += new System.EventHandler(this.ControlRoom_Click);
            // 
            // lblControlRoom
            // 
            this.lblControlRoom.AutoSize = true;
            this.lblControlRoom.BackColor = System.Drawing.Color.Transparent;
            this.lblControlRoom.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblControlRoom.ForeColor = System.Drawing.Color.White;
            this.lblControlRoom.Location = new System.Drawing.Point(327, 31);
            this.lblControlRoom.Margin = new System.Windows.Forms.Padding(0);
            this.lblControlRoom.Name = "lblControlRoom";
            this.lblControlRoom.Size = new System.Drawing.Size(98, 17);
            this.lblControlRoom.TabIndex = 101;
            this.lblControlRoom.Text = "교대근무자";
            this.lblControlRoom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblControlRoom.Click += new System.EventHandler(this.ControlRoom_Click);
            // 
            // picUserDefine
            // 
            this.picUserDefine.BackColor = System.Drawing.Color.Transparent;
            this.picUserDefine.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picUserDefine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picUserDefine.Location = new System.Drawing.Point(131, 28);
            this.picUserDefine.Margin = new System.Windows.Forms.Padding(0);
            this.picUserDefine.Name = "picUserDefine";
            this.picUserDefine.Size = new System.Drawing.Size(22, 22);
            this.picUserDefine.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picUserDefine.TabIndex = 100;
            this.picUserDefine.TabStop = false;
            this.picUserDefine.Click += new System.EventHandler(this.UserDefine_Click);
            // 
            // lblUserDefine
            // 
            this.lblUserDefine.AutoSize = true;
            this.lblUserDefine.BackColor = System.Drawing.Color.Transparent;
            this.lblUserDefine.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblUserDefine.ForeColor = System.Drawing.Color.White;
            this.lblUserDefine.Location = new System.Drawing.Point(153, 31);
            this.lblUserDefine.Margin = new System.Windows.Forms.Padding(0);
            this.lblUserDefine.Name = "lblUserDefine";
            this.lblUserDefine.Size = new System.Drawing.Size(134, 17);
            this.lblUserDefine.TabIndex = 99;
            this.lblUserDefine.Text = "사용자정의조직";
            this.lblUserDefine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblUserDefine.Click += new System.EventHandler(this.UserDefine_Click);
            // 
            // lblExternal
            // 
            this.lblExternal.AutoSize = true;
            this.lblExternal.BackColor = System.Drawing.Color.Transparent;
            this.lblExternal.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblExternal.ForeColor = System.Drawing.Color.White;
            this.lblExternal.Location = new System.Drawing.Point(25, 31);
            this.lblExternal.Margin = new System.Windows.Forms.Padding(0);
            this.lblExternal.Name = "lblExternal";
            this.lblExternal.Size = new System.Drawing.Size(80, 17);
            this.lblExternal.TabIndex = 97;
            this.lblExternal.Text = "외부조직";
            this.lblExternal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblExternal.Click += new System.EventHandler(this.External_Click);
            // 
            // checkBoxIncludeChildTeams
            // 
            this.checkBoxIncludeChildTeams.AutoSize = true;
            this.checkBoxIncludeChildTeams.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxIncludeChildTeams.ForeColor = System.Drawing.Color.White;
            this.checkBoxIncludeChildTeams.Location = new System.Drawing.Point(375, 455);
            this.checkBoxIncludeChildTeams.Name = "checkBoxIncludeChildTeams";
            this.checkBoxIncludeChildTeams.Size = new System.Drawing.Size(88, 16);
            this.checkBoxIncludeChildTeams.TabIndex = 25;
            this.checkBoxIncludeChildTeams.Text = "하위팀 포함";
            this.checkBoxIncludeChildTeams.UseVisualStyleBackColor = false;
            this.checkBoxIncludeChildTeams.Visible = false;
            this.checkBoxIncludeChildTeams.CheckedChanged += new System.EventHandler(this.checkBoxIncludeChildTeams_CheckedChanged);
            this.checkBoxIncludeChildTeams.VisibleChanged += new System.EventHandler(this.checkBoxIncludeChildTeams_VisibleChanged);
            // 
            // picChildTeams
            // 
            this.picChildTeams.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_disable;
            this.picChildTeams.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picChildTeams.Location = new System.Drawing.Point(356, 452);
            this.picChildTeams.Name = "picChildTeams";
            this.picChildTeams.Size = new System.Drawing.Size(18, 18);
            this.picChildTeams.TabIndex = 101;
            this.picChildTeams.TabStop = false;
            this.picChildTeams.Click += new System.EventHandler(this.IncludeChildTeam_Click);
            // 
            // lblChildTeams
            // 
            this.lblChildTeams.AutoSize = true;
            this.lblChildTeams.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblChildTeams.ForeColor = System.Drawing.Color.White;
            this.lblChildTeams.Location = new System.Drawing.Point(375, 453);
            this.lblChildTeams.Name = "lblChildTeams";
            this.lblChildTeams.Size = new System.Drawing.Size(104, 17);
            this.lblChildTeams.TabIndex = 102;
            this.lblChildTeams.Text = "하위팀 포함";
            this.lblChildTeams.Click += new System.EventHandler(this.IncludeChildTeam_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnCancel.Location = new System.Drawing.Point(803, 472);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(69, 37);
            this.btnCancel.TabIndex = 104;
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
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnOK.Location = new System.Drawing.Point(733, 472);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 103;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.CheckButton = false;
            this.btnAdd.CheckedBkgndImage = null;
            this.btnAdd.CheckedImage = null;
            this.btnAdd.ClickedBackgroundImage = null;
            this.btnAdd.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_AddShiftClick;
            this.btnAdd.CustomImageRect = new System.Drawing.Rectangle(0, 0, 90, 37);
            this.btnAdd.DisabledBkgndImage = null;
            this.btnAdd.DisabledImage = null;
            this.btnAdd.ID = -1;
            this.btnAdd.InitButtonWidth = 90;
            this.btnAdd.IsChecked = false;
            this.btnAdd.Location = new System.Drawing.Point(489, 224);
            this.btnAdd.MouseOverBkgndImage = null;
            this.btnAdd.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_AddShiftClick;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_AddShift;
            this.btnAdd.Owner = null;
            this.btnAdd.Size = new System.Drawing.Size(90, 37);
            this.btnAdd.TabIndex = 105;
            this.btnAdd.TextLocation = new System.Drawing.Point(0, 0);
            this.btnAdd.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnAdd.ToolTipText = "";
            this.btnAdd.UseCustomImageRect = true;
            this.btnAdd.UseTextLocation = false;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDel
            // 
            this.btnDel.CheckButton = false;
            this.btnDel.CheckedBkgndImage = null;
            this.btnDel.CheckedImage = null;
            this.btnDel.ClickedBackgroundImage = null;
            this.btnDel.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_DeleteShiftClick;
            this.btnDel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 90, 37);
            this.btnDel.DisabledBkgndImage = null;
            this.btnDel.DisabledImage = null;
            this.btnDel.ID = -1;
            this.btnDel.InitButtonWidth = 90;
            this.btnDel.IsChecked = false;
            this.btnDel.Location = new System.Drawing.Point(489, 262);
            this.btnDel.MouseOverBkgndImage = null;
            this.btnDel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_DeleteShiftClick;
            this.btnDel.Name = "btnDel";
            this.btnDel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_DeleteShift;
            this.btnDel.Owner = null;
            this.btnDel.Size = new System.Drawing.Size(90, 37);
            this.btnDel.TabIndex = 106;
            this.btnDel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnDel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDel.ToolTipText = "";
            this.btnDel.UseCustomImageRect = true;
            this.btnDel.UseTextLocation = false;
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // picDefault
            // 
            this.picDefault.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_disable;
            this.picDefault.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picDefault.Location = new System.Drawing.Point(13, 452);
            this.picDefault.Name = "picDefault";
            this.picDefault.Size = new System.Drawing.Size(18, 18);
            this.picDefault.TabIndex = 107;
            this.picDefault.TabStop = false;
            this.picDefault.Click += new System.EventHandler(this.Default_Click);
            // 
            // lblDefault
            // 
            this.lblDefault.AutoSize = true;
            this.lblDefault.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDefault.ForeColor = System.Drawing.Color.White;
            this.lblDefault.Location = new System.Drawing.Point(30, 453);
            this.lblDefault.Name = "lblDefault";
            this.lblDefault.Size = new System.Drawing.Size(241, 17);
            this.lblDefault.TabIndex = 108;
            this.lblDefault.Text = "SOP 제어권 가진곳의 책임자";
            this.lblDefault.Click += new System.EventHandler(this.Default_Click);
            // 
            // PopupSelectTeam
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(880, 510);
            this.Controls.Add(this.textBoxDisplay);
            this.Controls.Add(this.picDefault);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.picChildTeams);
            this.Controls.Add(this.lblChildTeams);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelTeamType);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.labelFullPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnChangeTeam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxIncludeChildTeams);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.rbBtnControlRoom);
            this.Controls.Add(this.rbBtnEmergency);
            this.Controls.Add(this.rbBtnRegular);
            this.Controls.Add(this.rbBtnExternal);
            this.Controls.Add(this.rbBtnUserDefine);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.lblDefault);
            this.Controls.Add(this.checkBoxDefault);
            this.Controls.Add(this.dataGridViewUserDefined);
            this.Controls.Add(this.treeViewTeam);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(490, 510);
            this.Name = "PopupSelectTeam";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "팀 선택";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupSelectTeam_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupSelectTeam_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupSelectTeam_MouseUp);
            this.panelSearch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUserDefined)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRegular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEmergency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExternal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picControlRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserDefine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picChildTeams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDefault)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelTeamType;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.Button btnChangeTeam;
        private System.Windows.Forms.RadioButton rbBtnExternal;
        private System.Windows.Forms.RadioButton rbBtnUserDefine;
        private System.Windows.Forms.RadioButton rbBtnEmergency;
        private System.Windows.Forms.RadioButton rbBtnRegular;
        private System.Windows.Forms.DataGridView dataGridViewUserDefined;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.PictureBox pictureBoxSearch;
        private System.Windows.Forms.RadioButton rbBtnControlRoom;
        private System.Windows.Forms.RichTextBox rtextSearch;
        private System.Windows.Forms.CheckBox checkBoxDefault;
        private System.Windows.Forms.Label labelFullPath;
        private System.Windows.Forms.TextBox textBoxDisplay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBoxIncludeChildTeams;
        private System.Windows.Forms.PictureBox picRegular;
        private System.Windows.Forms.Label lblRegular;
        private System.Windows.Forms.PictureBox picEmergency;
        private System.Windows.Forms.Label lblEmergency;
        private System.Windows.Forms.PictureBox picExternal;
        private System.Windows.Forms.PictureBox picControlRoom;
        private System.Windows.Forms.Label lblControlRoom;
        private System.Windows.Forms.PictureBox picUserDefine;
        private System.Windows.Forms.Label lblUserDefine;
        private System.Windows.Forms.Label lblExternal;
        private System.Windows.Forms.PictureBox picChildTeams;
        private System.Windows.Forms.Label lblChildTeams;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.RibbonButton btnAdd;
        private UnE.GUI.RibbonButton btnDel;
        private System.Windows.Forms.PictureBox picDefault;
        private System.Windows.Forms.Label lblDefault;

    }
}