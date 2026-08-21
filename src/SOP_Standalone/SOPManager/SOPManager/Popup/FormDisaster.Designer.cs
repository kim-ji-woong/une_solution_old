namespace SOPManager
{
	partial class FormDisaster
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDisaster));
            this.btnDelUserType = new System.Windows.Forms.Button();
            this.dataGridViewSubDisaster = new System.Windows.Forms.DataGridView();
            this.Column3 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewDisaster = new System.Windows.Forms.DataGridView();
            this.Column2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnAddSubCategory = new System.Windows.Forms.Button();
            this.btnChangeSubCateogryName = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCategoryNetural = new UnE.GUI.RibbonButton();
            this.btnCategoryFire = new UnE.GUI.RibbonButton();
            this.btnCategorySpill = new UnE.GUI.RibbonButton();
            this.btnCategoryTerror = new UnE.GUI.RibbonButton();
            this.btnCategoryEtc = new UnE.GUI.RibbonButton();
            this.btnCategroyExplosion = new UnE.GUI.RibbonButton();
            this.btnUserType = new System.Windows.Forms.Button();
            this.btnCategoryTypoon = new UnE.GUI.RibbonButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSubDisaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDisaster)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDelUserType
            // 
            this.btnDelUserType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelUserType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelUserType.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDelUserType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnDelUserType.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelUserType.Location = new System.Drawing.Point(737, 493);
            this.btnDelUserType.Name = "btnDelUserType";
            this.btnDelUserType.Size = new System.Drawing.Size(216, 40);
            this.btnDelUserType.TabIndex = 73;
            this.btnDelUserType.Text = "재난상황 삭제";
            this.btnDelUserType.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnDelUserType.UseVisualStyleBackColor = true;
            this.btnDelUserType.Click += new System.EventHandler(this.btnDelUserType_Click);
            // 
            // dataGridViewSubDisaster
            // 
            this.dataGridViewSubDisaster.AllowUserToAddRows = false;
            this.dataGridViewSubDisaster.AllowUserToDeleteRows = false;
            this.dataGridViewSubDisaster.AllowUserToResizeColumns = false;
            this.dataGridViewSubDisaster.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridViewSubDisaster.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewSubDisaster.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewSubDisaster.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewSubDisaster.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.dataGridViewSubDisaster.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewSubDisaster.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewSubDisaster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSubDisaster.ColumnHeadersVisible = false;
            this.dataGridViewSubDisaster.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.dataGridViewTextBoxColumn1});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewSubDisaster.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewSubDisaster.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.dataGridViewSubDisaster.Location = new System.Drawing.Point(518, 44);
            this.dataGridViewSubDisaster.MultiSelect = false;
            this.dataGridViewSubDisaster.Name = "dataGridViewSubDisaster";
            this.dataGridViewSubDisaster.ReadOnly = true;
            this.dataGridViewSubDisaster.RowHeadersVisible = false;
            this.dataGridViewSubDisaster.RowTemplate.Height = 23;
            this.dataGridViewSubDisaster.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewSubDisaster.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSubDisaster.ShowEditingIcon = false;
            this.dataGridViewSubDisaster.Size = new System.Drawing.Size(435, 441);
            this.dataGridViewSubDisaster.TabIndex = 70;
            this.dataGridViewSubDisaster.SelectionChanged += new System.EventHandler(this.dataGridViewSubDisaster_SelectionChanged);
            // 
            // Column3
            // 
            this.Column3.FillWeight = 25F;
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn1.FillWeight = 150.6294F;
            this.dataGridViewTextBoxColumn1.HeaderText = "재난 상세유형";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.ToolTipText = "재난 상세유형";
            // 
            // dataGridViewDisaster
            // 
            this.dataGridViewDisaster.AllowUserToAddRows = false;
            this.dataGridViewDisaster.AllowUserToDeleteRows = false;
            this.dataGridViewDisaster.AllowUserToResizeColumns = false;
            this.dataGridViewDisaster.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridViewDisaster.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewDisaster.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridViewDisaster.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewDisaster.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.dataGridViewDisaster.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewDisaster.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewDisaster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDisaster.ColumnHeadersVisible = false;
            this.dataGridViewDisaster.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column2,
            this.column1});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewDisaster.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewDisaster.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.dataGridViewDisaster.Location = new System.Drawing.Point(208, 44);
            this.dataGridViewDisaster.MultiSelect = false;
            this.dataGridViewDisaster.Name = "dataGridViewDisaster";
            this.dataGridViewDisaster.ReadOnly = true;
            this.dataGridViewDisaster.RowHeadersVisible = false;
            this.dataGridViewDisaster.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewDisaster.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.dataGridViewDisaster.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dataGridViewDisaster.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.dataGridViewDisaster.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dataGridViewDisaster.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridViewDisaster.RowTemplate.Height = 23;
            this.dataGridViewDisaster.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewDisaster.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewDisaster.ShowEditingIcon = false;
            this.dataGridViewDisaster.Size = new System.Drawing.Size(303, 441);
            this.dataGridViewDisaster.TabIndex = 69;
            this.dataGridViewDisaster.SelectionChanged += new System.EventHandler(this.dataGridViewDisaster_SelectionChanged);
            // 
            // Column2
            // 
            this.Column2.FillWeight = 50.76142F;
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // column1
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.column1.DefaultCellStyle = dataGridViewCellStyle7;
            this.column1.FillWeight = 149.2386F;
            this.column1.HeaderText = "재난유형";
            this.column1.Name = "column1";
            this.column1.ReadOnly = true;
            this.column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.column1.ToolTipText = "재난유형";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.label12.Location = new System.Drawing.Point(109, 310);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(34, 17);
            this.label12.TabIndex = 68;
            this.label12.Text = "폭발";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.label11.Location = new System.Drawing.Point(19, 310);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 17);
            this.label11.TabIndex = 67;
            this.label11.Text = "일반재해";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.label10.Location = new System.Drawing.Point(674, 236);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 17);
            this.label10.TabIndex = 66;
            this.label10.Text = "태풍";
            this.label10.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.label9.Location = new System.Drawing.Point(109, 199);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 17);
            this.label9.TabIndex = 65;
            this.label9.Text = "테러";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.label8.Location = new System.Drawing.Point(16, 199);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 17);
            this.label8.TabIndex = 64;
            this.label8.Text = "유출사고";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.label7.Location = new System.Drawing.Point(109, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 17);
            this.label7.TabIndex = 63;
            this.label7.Text = "화재";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.label6.Location = new System.Drawing.Point(19, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 17);
            this.label6.TabIndex = 62;
            this.label6.Text = "자연재해";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.label4.Location = new System.Drawing.Point(514, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 21);
            this.label4.TabIndex = 58;
            this.label4.Text = "재난상황";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.label3.Location = new System.Drawing.Point(204, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 21);
            this.label3.TabIndex = 57;
            this.label3.Text = "재난종류";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.label2.Location = new System.Drawing.Point(12, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 21);
            this.label2.TabIndex = 51;
            this.label2.Text = "재난분야";
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(371, 612);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(140, 41);
            this.button3.TabIndex = 76;
            this.button3.Text = "확인";
            this.button3.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(518, 612);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(143, 41);
            this.button1.TabIndex = 77;
            this.button1.Text = "취소";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(518, 493);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(213, 40);
            this.button2.TabIndex = 78;
            this.button2.Text = "재난상황 이름 변경";
            this.button2.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnChangeDisaster_Click);
            // 
            // btnAddSubCategory
            // 
            this.btnAddSubCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddSubCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSubCategory.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAddSubCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnAddSubCategory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddSubCategory.Location = new System.Drawing.Point(208, 493);
            this.btnAddSubCategory.Name = "btnAddSubCategory";
            this.btnAddSubCategory.Size = new System.Drawing.Size(155, 40);
            this.btnAddSubCategory.TabIndex = 79;
            this.btnAddSubCategory.Text = "재난종류 추가";
            this.btnAddSubCategory.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnAddSubCategory.UseVisualStyleBackColor = true;
            this.btnAddSubCategory.Click += new System.EventHandler(this.btnAddSubCategroy_Click);
            // 
            // btnChangeSubCateogryName
            // 
            this.btnChangeSubCateogryName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnChangeSubCateogryName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeSubCateogryName.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChangeSubCateogryName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnChangeSubCateogryName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnChangeSubCateogryName.Location = new System.Drawing.Point(369, 493);
            this.btnChangeSubCateogryName.Name = "btnChangeSubCateogryName";
            this.btnChangeSubCateogryName.Size = new System.Drawing.Size(142, 40);
            this.btnChangeSubCateogryName.TabIndex = 80;
            this.btnChangeSubCateogryName.Text = "재난종류 변경";
            this.btnChangeSubCateogryName.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnChangeSubCateogryName.UseVisualStyleBackColor = true;
            this.btnChangeSubCateogryName.Click += new System.EventHandler(this.btnChnageSubCateogryName_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.Location = new System.Drawing.Point(208, 539);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(155, 40);
            this.button4.TabIndex = 81;
            this.button4.Text = "재난종류 삭제";
            this.button4.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btnDeleteSubCategory_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnCategoryNetural);
            this.groupBox1.Controls.Add(this.btnCategoryFire);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.btnCategorySpill);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.btnCategoryTerror);
            this.groupBox1.Controls.Add(this.btnCategoryEtc);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.btnCategroyExplosion);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(10, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(178, 420);
            this.groupBox1.TabIndex = 82;
            this.groupBox1.TabStop = false;
            // 
            // btnCategoryNetural
            // 
            this.btnCategoryNetural.BackgroundImage = global::SOPManager.Properties.Resources.btnCategory_back;
            this.btnCategoryNetural.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCategoryNetural.CheckButton = false;
            this.btnCategoryNetural.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.btnCategoryNetural.CheckedImage = global::SOPManager.Properties.Resources.btnCategoryNatural;
            this.btnCategoryNetural.ClickedBackgroundImage = null;
            this.btnCategoryNetural.ClickedImage = null;
            this.btnCategoryNetural.CustomImageRect = new System.Drawing.Rectangle(5, 5, 50, 50);
            this.btnCategoryNetural.DisabledBkgndImage = null;
            this.btnCategoryNetural.DisabledImage = null;
            this.btnCategoryNetural.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnCategoryNetural.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnCategoryNetural.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCategoryNetural.ID = -1;
            this.btnCategoryNetural.InitButtonWidth = 60;
            this.btnCategoryNetural.IsChecked = false;
            this.btnCategoryNetural.Location = new System.Drawing.Point(16, 25);
            this.btnCategoryNetural.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.btnCategoryNetural.MouseOverImage = null;
            this.btnCategoryNetural.Name = "btnCategoryNetural";
            this.btnCategoryNetural.NormalImage = global::SOPManager.Properties.Resources.btnCategoryNatural;
            this.btnCategoryNetural.Owner = null;
            this.btnCategoryNetural.Size = new System.Drawing.Size(60, 60);
            this.btnCategoryNetural.TabIndex = 52;
            this.btnCategoryNetural.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCategoryNetural.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCategoryNetural.ToolTipText = "자연재해";
            this.btnCategoryNetural.UseCustomImageRect = true;
            this.btnCategoryNetural.UseTextLocation = false;
            this.btnCategoryNetural.UseVisualStyleBackColor = true;
            this.btnCategoryNetural.Click += new System.EventHandler(this.btnCategoryNetural_Click);
            // 
            // btnCategoryFire
            // 
            this.btnCategoryFire.BackgroundImage = global::SOPManager.Properties.Resources.btnCategory_back;
            this.btnCategoryFire.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCategoryFire.CheckButton = false;
            this.btnCategoryFire.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.btnCategoryFire.CheckedImage = global::SOPManager.Properties.Resources.btnCategoryFire;
            this.btnCategoryFire.ClickedBackgroundImage = null;
            this.btnCategoryFire.ClickedImage = null;
            this.btnCategoryFire.CustomImageRect = new System.Drawing.Rectangle(5, 5, 50, 50);
            this.btnCategoryFire.DisabledBkgndImage = null;
            this.btnCategoryFire.DisabledImage = null;
            this.btnCategoryFire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCategoryFire.ID = -1;
            this.btnCategoryFire.InitButtonWidth = 60;
            this.btnCategoryFire.IsChecked = false;
            this.btnCategoryFire.Location = new System.Drawing.Point(98, 25);
            this.btnCategoryFire.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.btnCategoryFire.MouseOverImage = null;
            this.btnCategoryFire.Name = "btnCategoryFire";
            this.btnCategoryFire.NormalImage = global::SOPManager.Properties.Resources.btnCategoryFire;
            this.btnCategoryFire.Owner = null;
            this.btnCategoryFire.Size = new System.Drawing.Size(60, 60);
            this.btnCategoryFire.TabIndex = 53;
            this.btnCategoryFire.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCategoryFire.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCategoryFire.ToolTipText = "화재";
            this.btnCategoryFire.UseCustomImageRect = true;
            this.btnCategoryFire.UseTextLocation = false;
            this.btnCategoryFire.UseVisualStyleBackColor = true;
            this.btnCategoryFire.Click += new System.EventHandler(this.btnCategoryFire_Click);
            // 
            // btnCategorySpill
            // 
            this.btnCategorySpill.BackgroundImage = global::SOPManager.Properties.Resources.btnCategory_back;
            this.btnCategorySpill.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCategorySpill.CheckButton = false;
            this.btnCategorySpill.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.btnCategorySpill.CheckedImage = global::SOPManager.Properties.Resources.btnCategorySpill;
            this.btnCategorySpill.ClickedBackgroundImage = null;
            this.btnCategorySpill.ClickedImage = null;
            this.btnCategorySpill.CustomImageRect = new System.Drawing.Rectangle(5, 5, 50, 50);
            this.btnCategorySpill.DisabledBkgndImage = null;
            this.btnCategorySpill.DisabledImage = null;
            this.btnCategorySpill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCategorySpill.ID = -1;
            this.btnCategorySpill.InitButtonWidth = 60;
            this.btnCategorySpill.IsChecked = false;
            this.btnCategorySpill.Location = new System.Drawing.Point(16, 136);
            this.btnCategorySpill.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.btnCategorySpill.MouseOverImage = null;
            this.btnCategorySpill.Name = "btnCategorySpill";
            this.btnCategorySpill.NormalImage = global::SOPManager.Properties.Resources.btnCategorySpill;
            this.btnCategorySpill.Owner = null;
            this.btnCategorySpill.Size = new System.Drawing.Size(60, 60);
            this.btnCategorySpill.TabIndex = 54;
            this.btnCategorySpill.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCategorySpill.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCategorySpill.ToolTipText = "유출사고";
            this.btnCategorySpill.UseCustomImageRect = true;
            this.btnCategorySpill.UseTextLocation = false;
            this.btnCategorySpill.UseVisualStyleBackColor = true;
            this.btnCategorySpill.Click += new System.EventHandler(this.btnCategorySpill_Click);
            // 
            // btnCategoryTerror
            // 
            this.btnCategoryTerror.BackgroundImage = global::SOPManager.Properties.Resources.btnCategory_back;
            this.btnCategoryTerror.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCategoryTerror.CheckButton = false;
            this.btnCategoryTerror.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.btnCategoryTerror.CheckedImage = global::SOPManager.Properties.Resources.btnCategoryTerror;
            this.btnCategoryTerror.ClickedBackgroundImage = null;
            this.btnCategoryTerror.ClickedImage = null;
            this.btnCategoryTerror.CustomImageRect = new System.Drawing.Rectangle(5, 5, 50, 50);
            this.btnCategoryTerror.DisabledBkgndImage = null;
            this.btnCategoryTerror.DisabledImage = null;
            this.btnCategoryTerror.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCategoryTerror.ID = -1;
            this.btnCategoryTerror.InitButtonWidth = 60;
            this.btnCategoryTerror.IsChecked = false;
            this.btnCategoryTerror.Location = new System.Drawing.Point(98, 136);
            this.btnCategoryTerror.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.btnCategoryTerror.MouseOverImage = null;
            this.btnCategoryTerror.Name = "btnCategoryTerror";
            this.btnCategoryTerror.NormalImage = global::SOPManager.Properties.Resources.btnCategoryTerror;
            this.btnCategoryTerror.Owner = null;
            this.btnCategoryTerror.Size = new System.Drawing.Size(60, 60);
            this.btnCategoryTerror.TabIndex = 55;
            this.btnCategoryTerror.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCategoryTerror.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCategoryTerror.ToolTipText = "테러";
            this.btnCategoryTerror.UseCustomImageRect = true;
            this.btnCategoryTerror.UseTextLocation = false;
            this.btnCategoryTerror.UseVisualStyleBackColor = true;
            this.btnCategoryTerror.Click += new System.EventHandler(this.btnCategoryTerror_Click);
            // 
            // btnCategoryEtc
            // 
            this.btnCategoryEtc.BackgroundImage = global::SOPManager.Properties.Resources.btnCategory_back;
            this.btnCategoryEtc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCategoryEtc.CheckButton = false;
            this.btnCategoryEtc.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.btnCategoryEtc.CheckedImage = global::SOPManager.Properties.Resources.General_Disaster_Normal;
            this.btnCategoryEtc.ClickedBackgroundImage = null;
            this.btnCategoryEtc.ClickedImage = null;
            this.btnCategoryEtc.CustomImageRect = new System.Drawing.Rectangle(5, 5, 50, 50);
            this.btnCategoryEtc.DisabledBkgndImage = null;
            this.btnCategoryEtc.DisabledImage = null;
            this.btnCategoryEtc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCategoryEtc.ID = -1;
            this.btnCategoryEtc.InitButtonWidth = 60;
            this.btnCategoryEtc.IsChecked = false;
            this.btnCategoryEtc.Location = new System.Drawing.Point(16, 247);
            this.btnCategoryEtc.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.btnCategoryEtc.MouseOverImage = null;
            this.btnCategoryEtc.Name = "btnCategoryEtc";
            this.btnCategoryEtc.NormalImage = global::SOPManager.Properties.Resources.General_Disaster_Normal;
            this.btnCategoryEtc.Owner = null;
            this.btnCategoryEtc.Size = new System.Drawing.Size(60, 60);
            this.btnCategoryEtc.TabIndex = 60;
            this.btnCategoryEtc.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCategoryEtc.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCategoryEtc.ToolTipText = "기타";
            this.btnCategoryEtc.UseCustomImageRect = true;
            this.btnCategoryEtc.UseTextLocation = false;
            this.btnCategoryEtc.UseVisualStyleBackColor = true;
            this.btnCategoryEtc.Click += new System.EventHandler(this.btnCategorySaving_Click);
            // 
            // btnCategroyExplosion
            // 
            this.btnCategroyExplosion.BackgroundImage = global::SOPManager.Properties.Resources.btnCategory_back;
            this.btnCategroyExplosion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCategroyExplosion.CheckButton = false;
            this.btnCategroyExplosion.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.btnCategroyExplosion.CheckedImage = global::SOPManager.Properties.Resources.btnCategoryExplosion;
            this.btnCategroyExplosion.ClickedBackgroundImage = null;
            this.btnCategroyExplosion.ClickedImage = null;
            this.btnCategroyExplosion.CustomImageRect = new System.Drawing.Rectangle(5, 5, 50, 50);
            this.btnCategroyExplosion.DisabledBkgndImage = null;
            this.btnCategroyExplosion.DisabledImage = null;
            this.btnCategroyExplosion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCategroyExplosion.ID = -1;
            this.btnCategroyExplosion.InitButtonWidth = 60;
            this.btnCategroyExplosion.IsChecked = false;
            this.btnCategroyExplosion.Location = new System.Drawing.Point(98, 247);
            this.btnCategroyExplosion.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.btnCategroyExplosion.MouseOverImage = null;
            this.btnCategroyExplosion.Name = "btnCategroyExplosion";
            this.btnCategroyExplosion.NormalImage = global::SOPManager.Properties.Resources.btnCategoryExplosion;
            this.btnCategroyExplosion.Owner = null;
            this.btnCategroyExplosion.Size = new System.Drawing.Size(60, 60);
            this.btnCategroyExplosion.TabIndex = 61;
            this.btnCategroyExplosion.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCategroyExplosion.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCategroyExplosion.ToolTipText = "폭발";
            this.btnCategroyExplosion.UseCustomImageRect = true;
            this.btnCategroyExplosion.UseTextLocation = false;
            this.btnCategroyExplosion.UseVisualStyleBackColor = true;
            this.btnCategroyExplosion.Click += new System.EventHandler(this.btnCategroyExplosion_Click);
            // 
            // btnUserType
            // 
            this.btnUserType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUserType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUserType.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnUserType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnUserType.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUserType.Location = new System.Drawing.Point(518, 539);
            this.btnUserType.Name = "btnUserType";
            this.btnUserType.Size = new System.Drawing.Size(213, 40);
            this.btnUserType.TabIndex = 72;
            this.btnUserType.Text = " 사용자 재난상황 추가";
            this.btnUserType.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUserType.UseVisualStyleBackColor = true;
            this.btnUserType.Click += new System.EventHandler(this.btnUserType_Click);
            // 
            // btnCategoryTypoon
            // 
            this.btnCategoryTypoon.BackgroundImage = global::SOPManager.Properties.Resources.btnCategory_back;
            this.btnCategoryTypoon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCategoryTypoon.CheckButton = false;
            this.btnCategoryTypoon.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.btnCategoryTypoon.CheckedImage = global::SOPManager.Properties.Resources.btnCategoryTypoon;
            this.btnCategoryTypoon.ClickedBackgroundImage = null;
            this.btnCategoryTypoon.ClickedImage = null;
            this.btnCategoryTypoon.CustomImageRect = new System.Drawing.Rectangle(5, 5, 50, 50);
            this.btnCategoryTypoon.DisabledBkgndImage = null;
            this.btnCategoryTypoon.DisabledImage = null;
            this.btnCategoryTypoon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCategoryTypoon.ID = -1;
            this.btnCategoryTypoon.InitButtonWidth = 60;
            this.btnCategoryTypoon.IsChecked = false;
            this.btnCategoryTypoon.Location = new System.Drawing.Point(663, 173);
            this.btnCategoryTypoon.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.btnCategoryTypoon.MouseOverImage = null;
            this.btnCategoryTypoon.Name = "btnCategoryTypoon";
            this.btnCategoryTypoon.NormalImage = global::SOPManager.Properties.Resources.btnCategoryTypoon;
            this.btnCategoryTypoon.Owner = null;
            this.btnCategoryTypoon.Size = new System.Drawing.Size(60, 60);
            this.btnCategoryTypoon.TabIndex = 56;
            this.btnCategoryTypoon.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCategoryTypoon.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCategoryTypoon.ToolTipText = "태풍";
            this.btnCategoryTypoon.UseCustomImageRect = true;
            this.btnCategoryTypoon.UseTextLocation = false;
            this.btnCategoryTypoon.UseVisualStyleBackColor = true;
            this.btnCategoryTypoon.Visible = false;
            this.btnCategoryTypoon.Click += new System.EventHandler(this.btnCategoryTypoon_Click);
            // 
            // FormDisaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(979, 665);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnChangeSubCateogryName);
            this.Controls.Add(this.btnAddSubCategory);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnDelUserType);
            this.Controls.Add(this.btnUserType);
            this.Controls.Add(this.dataGridViewSubDisaster);
            this.Controls.Add(this.dataGridViewDisaster);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCategoryTypoon);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(995, 592);
            this.Name = "FormDisaster";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "재난 관리";
            this.Load += new System.EventHandler(this.FormDisaster_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSubDisaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDisaster)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnDelUserType;
		private System.Windows.Forms.Button btnUserType;
		private System.Windows.Forms.DataGridView dataGridViewSubDisaster;
		private System.Windows.Forms.DataGridViewImageColumn Column3;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private System.Windows.Forms.DataGridView dataGridViewDisaster;
		private System.Windows.Forms.DataGridViewImageColumn Column2;
		private System.Windows.Forms.DataGridViewTextBoxColumn column1;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private UnE.GUI.RibbonButton btnCategroyExplosion;
		private UnE.GUI.RibbonButton btnCategoryEtc;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private UnE.GUI.RibbonButton btnCategoryTypoon;
		private UnE.GUI.RibbonButton btnCategoryTerror;
		private UnE.GUI.RibbonButton btnCategorySpill;
		private UnE.GUI.RibbonButton btnCategoryFire;
		private UnE.GUI.RibbonButton btnCategoryNetural;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button btnAddSubCategory;
		private System.Windows.Forms.Button btnChangeSubCateogryName;
		private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox1;
	}
}