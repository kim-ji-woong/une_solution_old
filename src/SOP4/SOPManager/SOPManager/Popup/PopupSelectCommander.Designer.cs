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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewExternal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUserDefined)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTeamType
            // 
            this.labelTeamType.AutoSize = true;
            this.labelTeamType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.labelTeamType.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTeamType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.labelTeamType.Location = new System.Drawing.Point(11, 57);
            this.labelTeamType.Name = "labelTeamType";
            this.labelTeamType.Size = new System.Drawing.Size(59, 20);
            this.labelTeamType.TabIndex = 8;
            this.labelTeamType.Text = "전체 팀";
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewTeam.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            this.checkBoxDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxDefault.AutoSize = true;
            this.checkBoxDefault.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxDefault.Location = new System.Drawing.Point(13, 470);
            this.checkBoxDefault.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBoxDefault.Name = "checkBoxDefault";
            this.checkBoxDefault.Size = new System.Drawing.Size(196, 21);
            this.checkBoxDefault.TabIndex = 11;
            this.checkBoxDefault.Text = "SOP 제어권 가진곳의 책임자";
            this.checkBoxDefault.UseVisualStyleBackColor = true;
            this.checkBoxDefault.CheckedChanged += new System.EventHandler(this.checkBoxDefault_CheckedChanged);
            // 
            // btnSelectTeam
            // 
            this.btnSelectTeam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectTeam.Location = new System.Drawing.Point(682, 17);
            this.btnSelectTeam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSelectTeam.Name = "btnSelectTeam";
            this.btnSelectTeam.Size = new System.Drawing.Size(64, 29);
            this.btnSelectTeam.TabIndex = 12;
            this.btnSelectTeam.Text = "조직변경";
            this.btnSelectTeam.UseVisualStyleBackColor = true;
            this.btnSelectTeam.Visible = false;
            this.btnSelectTeam.Click += new System.EventHandler(this.btnSelectTeam_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(12, 529);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "표시이름 :";
            // 
            // textBoxDisplay
            // 
            this.textBoxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxDisplay.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDisplay.Location = new System.Drawing.Point(86, 526);
            this.textBoxDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxDisplay.Name = "textBoxDisplay";
            this.textBoxDisplay.Size = new System.Drawing.Size(256, 25);
            this.textBoxDisplay.TabIndex = 14;
            // 
            // labelFullPath
            // 
            this.labelFullPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelFullPath.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFullPath.Location = new System.Drawing.Point(12, 499);
            this.labelFullPath.Name = "labelFullPath";
            this.labelFullPath.Size = new System.Drawing.Size(663, 20);
            this.labelFullPath.TabIndex = 15;
            this.labelFullPath.Text = "전체경로";
            this.labelFullPath.Visible = false;
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
            this.btnCancel.Location = new System.Drawing.Point(575, 523);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 31);
            this.btnCancel.TabIndex = 17;
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
            this.btnOK.Location = new System.Drawing.Point(459, 523);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(110, 31);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbBtnExternal
            // 
            this.rbBtnExternal.AutoSize = true;
            this.rbBtnExternal.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnExternal.Location = new System.Drawing.Point(267, 21);
            this.rbBtnExternal.Name = "rbBtnExternal";
            this.rbBtnExternal.Size = new System.Drawing.Size(78, 21);
            this.rbBtnExternal.TabIndex = 21;
            this.rbBtnExternal.Text = "외부조직";
            this.rbBtnExternal.UseVisualStyleBackColor = true;
            this.rbBtnExternal.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnUserDefine
            // 
            this.rbBtnUserDefine.AutoSize = true;
            this.rbBtnUserDefine.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnUserDefine.Location = new System.Drawing.Point(379, 21);
            this.rbBtnUserDefine.Name = "rbBtnUserDefine";
            this.rbBtnUserDefine.Size = new System.Drawing.Size(117, 21);
            this.rbBtnUserDefine.TabIndex = 20;
            this.rbBtnUserDefine.Text = "사용자정의조직";
            this.rbBtnUserDefine.UseVisualStyleBackColor = true;
            this.rbBtnUserDefine.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnEmergency
            // 
            this.rbBtnEmergency.AutoSize = true;
            this.rbBtnEmergency.Checked = true;
            this.rbBtnEmergency.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnEmergency.Location = new System.Drawing.Point(130, 21);
            this.rbBtnEmergency.Name = "rbBtnEmergency";
            this.rbBtnEmergency.Size = new System.Drawing.Size(78, 21);
            this.rbBtnEmergency.TabIndex = 19;
            this.rbBtnEmergency.TabStop = true;
            this.rbBtnEmergency.Text = "비상조직";
            this.rbBtnEmergency.UseVisualStyleBackColor = true;
            this.rbBtnEmergency.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // rbBtnRegular
            // 
            this.rbBtnRegular.AutoSize = true;
            this.rbBtnRegular.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnRegular.Location = new System.Drawing.Point(19, 21);
            this.rbBtnRegular.Name = "rbBtnRegular";
            this.rbBtnRegular.Size = new System.Drawing.Size(78, 21);
            this.rbBtnRegular.TabIndex = 18;
            this.rbBtnRegular.Text = "정규조직";
            this.rbBtnRegular.UseVisualStyleBackColor = true;
            this.rbBtnRegular.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // dataGridViewExternal
            // 
            this.dataGridViewExternal.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridViewExternal.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewExternal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewExternal.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
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
            dataGridViewCellStyle4.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridViewUserDefined.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewUserDefined.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserDefined.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridViewUserDefined.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewUserDefined.RowTemplate.Height = 23;
            this.dataGridViewUserDefined.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewUserDefined.Size = new System.Drawing.Size(738, 380);
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
            this.rbBtnControlRoom.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnControlRoom.Location = new System.Drawing.Point(521, 21);
            this.rbBtnControlRoom.Name = "rbBtnControlRoom";
            this.rbBtnControlRoom.Size = new System.Drawing.Size(91, 21);
            this.rbBtnControlRoom.TabIndex = 24;
            this.rbBtnControlRoom.Text = "교대근무자";
            this.rbBtnControlRoom.UseVisualStyleBackColor = true;
            this.rbBtnControlRoom.CheckedChanged += new System.EventHandler(this.radioTeam_CheckedChanged);
            // 
            // PopupSelectCommander
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(762, 577);
            this.Controls.Add(this.rbBtnControlRoom);
            this.Controls.Add(this.rbBtnExternal);
            this.Controls.Add(this.rbBtnUserDefine);
            this.Controls.Add(this.rbBtnEmergency);
            this.Controls.Add(this.rbBtnRegular);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labelFullPath);
            this.Controls.Add(this.textBoxDisplay);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelectTeam);
            this.Controls.Add(this.checkBoxDefault);
            this.Controls.Add(this.labelTeamType);
            this.Controls.Add(this.dataGridViewUserDefined);
            this.Controls.Add(this.dataGridViewExternal);
            this.Controls.Add(this.treeViewTeam);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
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
    }
}