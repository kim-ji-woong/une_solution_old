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
            this.btnDel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.checkBoxDefault = new System.Windows.Forms.CheckBox();
            this.labelFullPath = new System.Windows.Forms.Label();
            this.textBoxDisplay = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxIncludeChildTeams = new System.Windows.Forms.CheckBox();
            this.panelSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUserDefined)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbBtnControlRoom
            // 
            this.rbBtnControlRoom.AutoSize = true;
            this.rbBtnControlRoom.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnControlRoom.Location = new System.Drawing.Point(274, 27);
            this.rbBtnControlRoom.Name = "rbBtnControlRoom";
            this.rbBtnControlRoom.Size = new System.Drawing.Size(91, 21);
            this.rbBtnControlRoom.TabIndex = 23;
            this.rbBtnControlRoom.Text = "교대근무자";
            this.rbBtnControlRoom.UseVisualStyleBackColor = true;
            this.rbBtnControlRoom.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.Color.White;
            this.panelSearch.Controls.Add(this.rtextSearch);
            this.panelSearch.Controls.Add(this.pictureBoxSearch);
            this.panelSearch.Location = new System.Drawing.Point(12, 68);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(466, 26);
            this.panelSearch.TabIndex = 22;
            // 
            // rtextSearch
            // 
            this.rtextSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtextSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtextSearch.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            this.rbBtnExternal.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnExternal.Location = new System.Drawing.Point(3, 27);
            this.rbBtnExternal.Name = "rbBtnExternal";
            this.rbBtnExternal.Size = new System.Drawing.Size(78, 21);
            this.rbBtnExternal.TabIndex = 3;
            this.rbBtnExternal.Text = "외부조직";
            this.rbBtnExternal.UseVisualStyleBackColor = true;
            this.rbBtnExternal.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnUserDefine
            // 
            this.rbBtnUserDefine.AutoSize = true;
            this.rbBtnUserDefine.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnUserDefine.Location = new System.Drawing.Point(118, 27);
            this.rbBtnUserDefine.Name = "rbBtnUserDefine";
            this.rbBtnUserDefine.Size = new System.Drawing.Size(117, 21);
            this.rbBtnUserDefine.TabIndex = 2;
            this.rbBtnUserDefine.Text = "사용자정의조직";
            this.rbBtnUserDefine.UseVisualStyleBackColor = true;
            this.rbBtnUserDefine.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnEmergency
            // 
            this.rbBtnEmergency.AutoSize = true;
            this.rbBtnEmergency.Checked = true;
            this.rbBtnEmergency.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnEmergency.Location = new System.Drawing.Point(118, 3);
            this.rbBtnEmergency.Name = "rbBtnEmergency";
            this.rbBtnEmergency.Size = new System.Drawing.Size(78, 21);
            this.rbBtnEmergency.TabIndex = 1;
            this.rbBtnEmergency.TabStop = true;
            this.rbBtnEmergency.Text = "비상조직";
            this.rbBtnEmergency.UseVisualStyleBackColor = true;
            this.rbBtnEmergency.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnRegular
            // 
            this.rbBtnRegular.AutoSize = true;
            this.rbBtnRegular.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnRegular.Location = new System.Drawing.Point(3, 3);
            this.rbBtnRegular.Name = "rbBtnRegular";
            this.rbBtnRegular.Size = new System.Drawing.Size(78, 21);
            this.rbBtnRegular.TabIndex = 0;
            this.rbBtnRegular.Text = "정규조직";
            this.rbBtnRegular.UseVisualStyleBackColor = true;
            this.rbBtnRegular.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // btnDel
            // 
            this.btnDel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnDel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnDel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDel.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.btnDel.Location = new System.Drawing.Point(490, 272);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(110, 31);
            this.btnDel.TabIndex = 9;
            this.btnDel.Text = "<<삭제";
            this.btnDel.UseVisualStyleBackColor = false;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.btnAdd.Location = new System.Drawing.Point(490, 220);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(110, 31);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "추가>>";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnChangeTeam
            // 
            this.btnChangeTeam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnChangeTeam.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnChangeTeam.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnChangeTeam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeTeam.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChangeTeam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.btnChangeTeam.Location = new System.Drawing.Point(490, 146);
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
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            dataGridViewCellStyle3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.GridColor = System.Drawing.Color.Black;
            this.dataGridView.Location = new System.Drawing.Point(609, 120);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
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
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.label2.Location = new System.Drawing.Point(633, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "선택한 팀";
            // 
            // labelTeamType
            // 
            this.labelTeamType.AutoSize = true;
            this.labelTeamType.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTeamType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.labelTeamType.Location = new System.Drawing.Point(9, 100);
            this.labelTeamType.Name = "labelTeamType";
            this.labelTeamType.Size = new System.Drawing.Size(52, 17);
            this.labelTeamType.TabIndex = 7;
            this.labelTeamType.Text = "전체 팀";
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.BackColor = System.Drawing.Color.White;
            this.treeViewTeam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewTeam.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            dataGridViewCellStyle5.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridViewUserDefined.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewUserDefined.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            dataGridViewCellStyle7.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White;
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
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.btnCancel.Location = new System.Drawing.Point(793, 506);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.btnOK.Location = new System.Drawing.Point(687, 506);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // checkBoxDefault
            // 
            this.checkBoxDefault.AutoSize = true;
            this.checkBoxDefault.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxDefault.Location = new System.Drawing.Point(12, 453);
            this.checkBoxDefault.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBoxDefault.Name = "checkBoxDefault";
            this.checkBoxDefault.Size = new System.Drawing.Size(196, 21);
            this.checkBoxDefault.TabIndex = 12;
            this.checkBoxDefault.Text = "SOP 제어권 가진곳의 책임자";
            this.checkBoxDefault.UseVisualStyleBackColor = true;
            this.checkBoxDefault.CheckedChanged += new System.EventHandler(this.checkBoxDefault_CheckedChanged);
            // 
            // labelFullPath
            // 
            this.labelFullPath.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFullPath.Location = new System.Drawing.Point(9, 483);
            this.labelFullPath.Name = "labelFullPath";
            this.labelFullPath.Size = new System.Drawing.Size(177, 20);
            this.labelFullPath.TabIndex = 16;
            this.labelFullPath.Text = "전체경로";
            this.labelFullPath.Visible = false;
            // 
            // textBoxDisplay
            // 
            this.textBoxDisplay.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDisplay.Location = new System.Drawing.Point(79, 511);
            this.textBoxDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxDisplay.Name = "textBoxDisplay";
            this.textBoxDisplay.Size = new System.Drawing.Size(187, 25);
            this.textBoxDisplay.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(9, 514);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "표시이름 :";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbBtnControlRoom);
            this.panel1.Controls.Add(this.rbBtnEmergency);
            this.panel1.Controls.Add(this.rbBtnRegular);
            this.panel1.Controls.Add(this.rbBtnExternal);
            this.panel1.Controls.Add(this.rbBtnUserDefine);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(466, 50);
            this.panel1.TabIndex = 24;
            // 
            // checkBoxIncludeChildTeams
            // 
            this.checkBoxIncludeChildTeams.AutoSize = true;
            this.checkBoxIncludeChildTeams.Location = new System.Drawing.Point(625, 19);
            this.checkBoxIncludeChildTeams.Name = "checkBoxIncludeChildTeams";
            this.checkBoxIncludeChildTeams.Size = new System.Drawing.Size(88, 16);
            this.checkBoxIncludeChildTeams.TabIndex = 25;
            this.checkBoxIncludeChildTeams.Text = "하위팀 포함";
            this.checkBoxIncludeChildTeams.UseVisualStyleBackColor = true;
            this.checkBoxIncludeChildTeams.Visible = false;
            this.checkBoxIncludeChildTeams.CheckedChanged += new System.EventHandler(this.checkBoxIncludeChildTeams_CheckedChanged);
            // 
            // PopupSelectTeam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(905, 545);
            this.Controls.Add(this.checkBoxIncludeChildTeams);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelTeamType);
            this.Controls.Add(this.dataGridViewUserDefined);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.labelFullPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxDefault);
            this.Controls.Add(this.textBoxDisplay);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnChangeTeam);
            this.Controls.Add(this.treeViewTeam);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(490, 545);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelTeamType;
		private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnChangeTeam;
		private System.Windows.Forms.Button btnOK;
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

    }
}