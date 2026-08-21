namespace CrisisAlertManager.Popup_Dialog
{
    partial class FormManualMember
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cbMember = new System.Windows.Forms.CheckBox();
            this.lbSubTeam = new System.Windows.Forms.Label();
            this.gridMainTeam = new System.Windows.Forms.DataGridView();
            this.colTeamName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridSubTeam = new System.Windows.Forms.DataGridView();
            this.colSubTeamName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridMember = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.gridManager = new System.Windows.Forms.DataGridView();
            this.colInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colManagerCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbManager = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.plTop = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.ImageButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnRemoveManager = new UnE.GUI.ImageButton();
            this.btnAddManager = new UnE.GUI.ImageButton();
            this.btnConfirm = new UnE.GUI.ImageButton();
            this.btnCancle = new UnE.GUI.ImageButton();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMainTeam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSubTeam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
            this.panel1.SuspendLayout();
            this.plTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemoveManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(74)))), ((int)(((byte)(127)))));
            this.panel3.Controls.Add(this.cbMember);
            this.panel3.Controls.Add(this.lbSubTeam);
            this.panel3.Location = new System.Drawing.Point(260, 59);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(205, 32);
            this.panel3.TabIndex = 65;
            // 
            // cbMember
            // 
            this.cbMember.AutoSize = true;
            this.cbMember.Location = new System.Drawing.Point(176, 10);
            this.cbMember.Name = "cbMember";
            this.cbMember.Size = new System.Drawing.Size(15, 14);
            this.cbMember.TabIndex = 70;
            this.cbMember.UseVisualStyleBackColor = true;
            this.cbMember.CheckedChanged += new System.EventHandler(this.cbMember_CheckedChanged);
            // 
            // lbSubTeam
            // 
            this.lbSubTeam.AutoSize = true;
            this.lbSubTeam.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbSubTeam.ForeColor = System.Drawing.Color.White;
            this.lbSubTeam.Location = new System.Drawing.Point(21, 10);
            this.lbSubTeam.Name = "lbSubTeam";
            this.lbSubTeam.Size = new System.Drawing.Size(33, 13);
            this.lbSubTeam.TabIndex = 61;
            this.lbSubTeam.Text = "부서";
            // 
            // gridMainTeam
            // 
            this.gridMainTeam.AllowUserToAddRows = false;
            this.gridMainTeam.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(240)))));
            this.gridMainTeam.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridMainTeam.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridMainTeam.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridMainTeam.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle14;
            this.gridMainTeam.ColumnHeadersHeight = 40;
            this.gridMainTeam.ColumnHeadersVisible = false;
            this.gridMainTeam.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTeamName});
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridMainTeam.DefaultCellStyle = dataGridViewCellStyle15;
            this.gridMainTeam.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridMainTeam.Location = new System.Drawing.Point(10, 59);
            this.gridMainTeam.Name = "gridMainTeam";
            this.gridMainTeam.ReadOnly = true;
            this.gridMainTeam.RowHeadersVisible = false;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle16.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridMainTeam.RowsDefaultCellStyle = dataGridViewCellStyle16;
            this.gridMainTeam.RowTemplate.Height = 50;
            this.gridMainTeam.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridMainTeam.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMainTeam.Size = new System.Drawing.Size(126, 450);
            this.gridMainTeam.TabIndex = 64;
            this.gridMainTeam.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMainTeam_CellClick);
            // 
            // colTeamName
            // 
            this.colTeamName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTeamName.HeaderText = "소속";
            this.colTeamName.Name = "colTeamName";
            this.colTeamName.ReadOnly = true;
            this.colTeamName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // gridSubTeam
            // 
            this.gridSubTeam.AllowUserToAddRows = false;
            this.gridSubTeam.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(240)))));
            this.gridSubTeam.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridSubTeam.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridSubTeam.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSubTeam.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.gridSubTeam.ColumnHeadersHeight = 40;
            this.gridSubTeam.ColumnHeadersVisible = false;
            this.gridSubTeam.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSubTeamName});
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridSubTeam.DefaultCellStyle = dataGridViewCellStyle18;
            this.gridSubTeam.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridSubTeam.Location = new System.Drawing.Point(135, 59);
            this.gridSubTeam.Name = "gridSubTeam";
            this.gridSubTeam.RowHeadersVisible = false;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle19.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridSubTeam.RowsDefaultCellStyle = dataGridViewCellStyle19;
            this.gridSubTeam.RowTemplate.Height = 50;
            this.gridSubTeam.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridSubTeam.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridSubTeam.Size = new System.Drawing.Size(126, 450);
            this.gridSubTeam.TabIndex = 63;
            this.gridSubTeam.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridSubTeam_CellContentClick);
            // 
            // colSubTeamName
            // 
            this.colSubTeamName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colSubTeamName.HeaderText = "부서";
            this.colSubTeamName.Name = "colSubTeamName";
            this.colSubTeamName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // gridMember
            // 
            this.gridMember.AllowUserToAddRows = false;
            this.gridMember.BackgroundColor = System.Drawing.Color.White;
            this.gridMember.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridMember.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridMember.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle20.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridMember.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle20;
            this.gridMember.ColumnHeadersHeight = 40;
            this.gridMember.ColumnHeadersVisible = false;
            this.gridMember.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colCheck});
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle21.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle21.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridMember.DefaultCellStyle = dataGridViewCellStyle21;
            this.gridMember.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridMember.Location = new System.Drawing.Point(260, 90);
            this.gridMember.Name = "gridMember";
            this.gridMember.RowHeadersVisible = false;
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle22.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle22.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle22.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle22.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle22.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridMember.RowsDefaultCellStyle = dataGridViewCellStyle22;
            this.gridMember.RowTemplate.Height = 50;
            this.gridMember.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridMember.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMember.Size = new System.Drawing.Size(205, 384);
            this.gridMember.TabIndex = 61;
            this.gridMember.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMember_CellValueChanged);
            this.gridMember.CurrentCellDirtyStateChanged += new System.EventHandler(this.gridMember_CurrentCellDirtyStateChanged);
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.HeaderText = "팀원";
            this.colName.Name = "colName";
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCheck
            // 
            this.colCheck.FalseValue = "False";
            this.colCheck.HeaderText = "체크";
            this.colCheck.Name = "colCheck";
            this.colCheck.TrueValue = "True";
            this.colCheck.Width = 40;
            // 
            // gridManager
            // 
            this.gridManager.AllowUserToAddRows = false;
            this.gridManager.BackgroundColor = System.Drawing.Color.White;
            this.gridManager.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridManager.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridManager.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle23.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle23.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle23.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle23.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle23.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridManager.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle23;
            this.gridManager.ColumnHeadersHeight = 40;
            this.gridManager.ColumnHeadersVisible = false;
            this.gridManager.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colInfo,
            this.colManagerCheck});
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle25.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle25.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle25.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle25.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle25.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridManager.DefaultCellStyle = dataGridViewCellStyle25;
            this.gridManager.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridManager.Location = new System.Drawing.Point(464, 90);
            this.gridManager.Name = "gridManager";
            this.gridManager.RowHeadersVisible = false;
            dataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle26.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle26.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle26.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle26.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle26.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridManager.RowsDefaultCellStyle = dataGridViewCellStyle26;
            this.gridManager.RowTemplate.Height = 50;
            this.gridManager.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridManager.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridManager.Size = new System.Drawing.Size(205, 384);
            this.gridManager.TabIndex = 62;
            this.gridManager.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridManager_CellValueChanged);
            this.gridManager.CurrentCellDirtyStateChanged += new System.EventHandler(this.gridManager_CurrentCellDirtyStateChanged);
            // 
            // colInfo
            // 
            this.colInfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle24.BackColor = System.Drawing.Color.White;
            this.colInfo.DefaultCellStyle = dataGridViewCellStyle24;
            this.colInfo.HeaderText = "직급+이름";
            this.colInfo.Name = "colInfo";
            this.colInfo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colInfo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colManagerCheck
            // 
            this.colManagerCheck.FalseValue = "False";
            this.colManagerCheck.HeaderText = "비고";
            this.colManagerCheck.Name = "colManagerCheck";
            this.colManagerCheck.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colManagerCheck.TrueValue = "True";
            this.colManagerCheck.Width = 40;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cbManager);
            this.panel1.Controls.Add(this.label1);
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(464, 59);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(205, 32);
            this.panel1.TabIndex = 66;
            // 
            // cbManager
            // 
            this.cbManager.AutoSize = true;
            this.cbManager.Location = new System.Drawing.Point(175, 9);
            this.cbManager.Name = "cbManager";
            this.cbManager.Size = new System.Drawing.Size(15, 14);
            this.cbManager.TabIndex = 71;
            this.cbManager.UseVisualStyleBackColor = true;
            this.cbManager.CheckedChanged += new System.EventHandler(this.cbManager_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(74)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(21, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 61;
            this.label1.Text = "선택된 수신자";
            // 
            // plTop
            // 
            this.plTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.plTop.Controls.Add(this.pictureBox6);
            this.plTop.Controls.Add(this.btnClose);
            this.plTop.Location = new System.Drawing.Point(0, 0);
            this.plTop.Name = "plTop";
            this.plTop.Size = new System.Drawing.Size(680, 50);
            this.plTop.TabIndex = 71;
            this.plTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.plTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.plTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnClose_Selected;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnClose_MouseOver;
            this.btnClose.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnClose_Normal;
            this.btnClose.Location = new System.Drawing.Point(643, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(25, 25);
            this.btnClose.TabIndex = 56;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
            this.pictureBox1.Location = new System.Drawing.Point(414, 508);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 1);
            this.pictureBox1.TabIndex = 70;
            this.pictureBox1.TabStop = false;
            // 
            // btnRemoveManager
            // 
            this.btnRemoveManager.ButtonText = "";
            this.btnRemoveManager.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnRemoveManual_Click;
            this.btnRemoveManager.ImageDisabled = null;
            this.btnRemoveManager.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnRemoveManual_Hover;
            this.btnRemoveManager.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnRemoveManual_Normal;
            this.btnRemoveManager.Location = new System.Drawing.Point(339, 474);
            this.btnRemoveManager.Name = "btnRemoveManager";
            this.btnRemoveManager.Owner = null;
            this.btnRemoveManager.Size = new System.Drawing.Size(80, 35);
            this.btnRemoveManager.TabIndex = 69;
            this.btnRemoveManager.TabStop = false;
            this.btnRemoveManager.TextColor = System.Drawing.Color.Black;
            this.btnRemoveManager.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemoveManager.ToolTipText = "";
            this.btnRemoveManager.UseToolTip = false;
            this.btnRemoveManager.WindowRateWidth = 1F;
            this.btnRemoveManager.Click += new System.EventHandler(this.btnRemoveManager_Click);
            // 
            // btnAddManager
            // 
            this.btnAddManager.ButtonText = "";
            this.btnAddManager.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnAddManual_Click;
            this.btnAddManager.ImageDisabled = null;
            this.btnAddManager.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnAddManual_Hover;
            this.btnAddManager.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnAddManual_Normal;
            this.btnAddManager.Location = new System.Drawing.Point(260, 474);
            this.btnAddManager.Name = "btnAddManager";
            this.btnAddManager.Owner = null;
            this.btnAddManager.Size = new System.Drawing.Size(80, 35);
            this.btnAddManager.TabIndex = 68;
            this.btnAddManager.TabStop = false;
            this.btnAddManager.TextColor = System.Drawing.Color.Black;
            this.btnAddManager.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAddManager.ToolTipText = "";
            this.btnAddManager.UseToolTip = false;
            this.btnAddManager.WindowRateWidth = 1F;
            this.btnAddManager.Click += new System.EventHandler(this.btnAddManager_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.ButtonText = "";
            this.btnConfirm.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnConfirm_Click;
            this.btnConfirm.ImageDisabled = null;
            this.btnConfirm.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnConfirm_Hover;
            this.btnConfirm.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnConfirm_Normal;
            this.btnConfirm.Location = new System.Drawing.Point(510, 474);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(80, 35);
            this.btnConfirm.TabIndex = 67;
            this.btnConfirm.TabStop = false;
            this.btnConfirm.TextColor = System.Drawing.Color.Black;
            this.btnConfirm.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfirm.ToolTipText = "";
            this.btnConfirm.UseToolTip = false;
            this.btnConfirm.WindowRateWidth = 1F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.ButtonText = "";
            this.btnCancle.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnCancle_Click;
            this.btnCancle.ImageDisabled = null;
            this.btnCancle.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnCancle_Hover;
            this.btnCancle.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnCancle_Normal;
            this.btnCancle.Location = new System.Drawing.Point(589, 474);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Owner = null;
            this.btnCancle.Size = new System.Drawing.Size(80, 35);
            this.btnCancle.TabIndex = 66;
            this.btnCancle.TabStop = false;
            this.btnCancle.TextColor = System.Drawing.Color.Black;
            this.btnCancle.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancle.ToolTipText = "";
            this.btnCancle.UseToolTip = false;
            this.btnCancle.WindowRateWidth = 1F;
            this.btnCancle.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox6.Image = global::CrisisAlertManager.Properties.Resources.ManagerAdd;
            this.pictureBox6.Location = new System.Drawing.Point(274, 13);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(119, 28);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 78;
            this.pictureBox6.TabStop = false;
            // 
            // FormManualMember
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(680, 520);
            this.Controls.Add(this.plTop);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnRemoveManager);
            this.Controls.Add(this.btnAddManager);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.gridMainTeam);
            this.Controls.Add(this.gridSubTeam);
            this.Controls.Add(this.gridMember);
            this.Controls.Add(this.gridManager);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormManualMember";
            this.Text = "FormManualMember";
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMainTeam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSubTeam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.plTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemoveManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lbSubTeam;
        private System.Windows.Forms.DataGridView gridMainTeam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeamName;
        private System.Windows.Forms.DataGridView gridSubTeam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubTeamName;
        private System.Windows.Forms.DataGridView gridMember;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colCheck;
        private System.Windows.Forms.DataGridView gridManager;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInfo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colManagerCheck;
        private UnE.GUI.ImageButton btnConfirm;
        private UnE.GUI.ImageButton btnCancle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private UnE.GUI.ImageButton btnRemoveManager;
        private UnE.GUI.ImageButton btnAddManager;
        private System.Windows.Forms.CheckBox cbMember;
        private System.Windows.Forms.CheckBox cbManager;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel plTop;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.PictureBox pictureBox6;
    }
}