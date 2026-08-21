namespace SOPManager
{
	partial class FormOpenDB
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
            this.panelTreeView = new System.Windows.Forms.Panel();
            this.treeViewSOP = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewVersion = new System.Windows.Forms.DataGridView();
            this.column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOpenSOP = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioRegular = new System.Windows.Forms.RadioButton();
            this.textSOPInfo = new System.Windows.Forms.RichTextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.radioNormal = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rdPictureBox1 = new System.Windows.Forms.PictureBox();
            this.rdLabel1 = new System.Windows.Forms.Label();
            this.rdPictureBox2 = new System.Windows.Forms.PictureBox();
            this.rdLabel2 = new System.Windows.Forms.Label();
            this.rdPictureBox3 = new System.Windows.Forms.PictureBox();
            this.rdLabel3 = new System.Windows.Forms.Label();
            this.rdPictureBox4 = new System.Windows.Forms.PictureBox();
            this.rdLabel4 = new System.Windows.Forms.Label();
            this.panelTreeView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVersion)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTreeView
            // 
            this.panelTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTreeView.Controls.Add(this.treeViewSOP);
            this.panelTreeView.Location = new System.Drawing.Point(50, 223);
            this.panelTreeView.Name = "panelTreeView";
            this.panelTreeView.Size = new System.Drawing.Size(694, 361);
            this.panelTreeView.TabIndex = 0;
            // 
            // treeViewSOP
            // 
            this.treeViewSOP.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewSOP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewSOP.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewSOP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            this.treeViewSOP.Location = new System.Drawing.Point(0, 0);
            this.treeViewSOP.Name = "treeViewSOP";
            this.treeViewSOP.Size = new System.Drawing.Size(692, 359);
            this.treeViewSOP.TabIndex = 7;
            this.treeViewSOP.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSOP_AfterSelect);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.label1.Location = new System.Drawing.Point(50, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 40);
            this.label1.TabIndex = 1;
            this.label1.Text = "최근 사용한 항목";
            // 
            // dataGridViewVersion
            // 
            this.dataGridViewVersion.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(84)))), ((int)(((byte)(105)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridViewVersion.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewVersion.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewVersion.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewVersion.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewVersion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewVersion.ColumnHeadersVisible = false;
            this.dataGridViewVersion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column1});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(84)))), ((int)(((byte)(105)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewVersion.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewVersion.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.dataGridViewVersion.Location = new System.Drawing.Point(783, 223);
            this.dataGridViewVersion.MultiSelect = false;
            this.dataGridViewVersion.Name = "dataGridViewVersion";
            this.dataGridViewVersion.ReadOnly = true;
            this.dataGridViewVersion.RowHeadersVisible = false;
            this.dataGridViewVersion.RowTemplate.Height = 23;
            this.dataGridViewVersion.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewVersion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewVersion.ShowEditingIcon = false;
            this.dataGridViewVersion.Size = new System.Drawing.Size(469, 277);
            this.dataGridViewVersion.TabIndex = 10;
            this.dataGridViewVersion.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewVersion_CellClick);
            // 
            // column1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(84)))), ((int)(((byte)(105)))));
            this.column1.DefaultCellStyle = dataGridViewCellStyle3;
            this.column1.HeaderText = "버전명";
            this.column1.Name = "column1";
            this.column1.ReadOnly = true;
            this.column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.column1.ToolTipText = "버전명";
            // 
            // btnOpenSOP
            // 
            this.btnOpenSOP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenSOP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnOpenSOP.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnOpenSOP.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnOpenSOP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenSOP.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOpenSOP.Location = new System.Drawing.Point(784, 604);
            this.btnOpenSOP.Name = "btnOpenSOP";
            this.btnOpenSOP.Size = new System.Drawing.Size(468, 53);
            this.btnOpenSOP.TabIndex = 12;
            this.btnOpenSOP.Text = "SOP 열기 >";
            this.btnOpenSOP.UseVisualStyleBackColor = false;
            this.btnOpenSOP.Click += new System.EventHandler(this.btnOpenSOP_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButton2);
            this.panel1.Controls.Add(this.radioRegular);
            this.panel1.Location = new System.Drawing.Point(615, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(392, 65);
            this.panel1.TabIndex = 19;
            this.panel1.Visible = false;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.radioButton2.Location = new System.Drawing.Point(206, 13);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(180, 41);
            this.radioButton2.TabIndex = 16;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "미등록 모드";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioRegular
            // 
            this.radioRegular.AutoSize = true;
            this.radioRegular.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioRegular.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.radioRegular.Location = new System.Drawing.Point(25, 13);
            this.radioRegular.Name = "radioRegular";
            this.radioRegular.Size = new System.Drawing.Size(153, 41);
            this.radioRegular.TabIndex = 15;
            this.radioRegular.TabStop = true;
            this.radioRegular.Text = "등록 모드";
            this.radioRegular.UseVisualStyleBackColor = true;
            this.radioRegular.CheckedChanged += new System.EventHandler(this.radioRegular_CheckedChanged);
            // 
            // textSOPInfo
            // 
            this.textSOPInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.textSOPInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textSOPInfo.Cursor = System.Windows.Forms.Cursors.Default;
            this.textSOPInfo.DetectUrls = false;
            this.textSOPInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textSOPInfo.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textSOPInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            this.textSOPInfo.Location = new System.Drawing.Point(0, 0);
            this.textSOPInfo.Name = "textSOPInfo";
            this.textSOPInfo.ReadOnly = true;
            this.textSOPInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textSOPInfo.Size = new System.Drawing.Size(467, 65);
            this.textSOPInfo.TabIndex = 12;
            this.textSOPInfo.TabStop = false;
            this.textSOPInfo.Text = "";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.textSOPInfo);
            this.panel3.Location = new System.Drawing.Point(783, 517);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(469, 67);
            this.panel3.TabIndex = 21;
            // 
            // radioNormal
            // 
            this.radioNormal.AutoSize = true;
            this.radioNormal.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioNormal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.radioNormal.Location = new System.Drawing.Point(25, 10);
            this.radioNormal.Name = "radioNormal";
            this.radioNormal.Size = new System.Drawing.Size(153, 41);
            this.radioNormal.TabIndex = 19;
            this.radioNormal.TabStop = true;
            this.radioNormal.Text = "평일 모드";
            this.radioNormal.UseVisualStyleBackColor = true;
            this.radioNormal.CheckedChanged += new System.EventHandler(this.radioNormal_CheckedChanged);
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.radioButton4.Location = new System.Drawing.Point(205, 10);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(254, 41);
            this.radioButton4.TabIndex = 20;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "야간 및 휴일 모드";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radioButton4);
            this.panel2.Controls.Add(this.radioNormal);
            this.panel2.Location = new System.Drawing.Point(615, 109);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(546, 60);
            this.panel2.TabIndex = 20;
            this.panel2.Visible = false;
            // 
            // rdPictureBox1
            // 
            this.rdPictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            this.rdPictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rdPictureBox1.Location = new System.Drawing.Point(62, 109);
            this.rdPictureBox1.Name = "rdPictureBox1";
            this.rdPictureBox1.Size = new System.Drawing.Size(18, 17);
            this.rdPictureBox1.TabIndex = 26;
            this.rdPictureBox1.TabStop = false;
            this.rdPictureBox1.Click += new System.EventHandler(this.rdPictureBox1_Click);
            // 
            // rdLabel1
            // 
            this.rdLabel1.AutoSize = true;
            this.rdLabel1.BackColor = System.Drawing.Color.Transparent;
            this.rdLabel1.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold);
            this.rdLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.rdLabel1.Location = new System.Drawing.Point(86, 98);
            this.rdLabel1.Name = "rdLabel1";
            this.rdLabel1.Size = new System.Drawing.Size(135, 37);
            this.rdLabel1.TabIndex = 25;
            this.rdLabel1.Text = "등록 모드";
            this.rdLabel1.Click += new System.EventHandler(this.rdLabel1_Click);
            // 
            // rdPictureBox2
            // 
            this.rdPictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            this.rdPictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rdPictureBox2.Location = new System.Drawing.Point(267, 109);
            this.rdPictureBox2.Name = "rdPictureBox2";
            this.rdPictureBox2.Size = new System.Drawing.Size(18, 17);
            this.rdPictureBox2.TabIndex = 28;
            this.rdPictureBox2.TabStop = false;
            this.rdPictureBox2.Click += new System.EventHandler(this.rdPictureBox2_Click);
            // 
            // rdLabel2
            // 
            this.rdLabel2.AutoSize = true;
            this.rdLabel2.BackColor = System.Drawing.Color.Transparent;
            this.rdLabel2.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold);
            this.rdLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.rdLabel2.Location = new System.Drawing.Point(291, 98);
            this.rdLabel2.Name = "rdLabel2";
            this.rdLabel2.Size = new System.Drawing.Size(162, 37);
            this.rdLabel2.TabIndex = 27;
            this.rdLabel2.Text = "미등록 모드";
            this.rdLabel2.Click += new System.EventHandler(this.rdLabel2_Click);
            // 
            // rdPictureBox3
            // 
            this.rdPictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            this.rdPictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rdPictureBox3.Location = new System.Drawing.Point(62, 161);
            this.rdPictureBox3.Name = "rdPictureBox3";
            this.rdPictureBox3.Size = new System.Drawing.Size(18, 17);
            this.rdPictureBox3.TabIndex = 30;
            this.rdPictureBox3.TabStop = false;
            this.rdPictureBox3.Click += new System.EventHandler(this.rdPictureBox3_Click);
            // 
            // rdLabel3
            // 
            this.rdLabel3.AutoSize = true;
            this.rdLabel3.BackColor = System.Drawing.Color.Transparent;
            this.rdLabel3.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold);
            this.rdLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.rdLabel3.Location = new System.Drawing.Point(86, 150);
            this.rdLabel3.Name = "rdLabel3";
            this.rdLabel3.Size = new System.Drawing.Size(135, 37);
            this.rdLabel3.TabIndex = 29;
            this.rdLabel3.Text = "평일 모드";
            this.rdLabel3.Click += new System.EventHandler(this.rdLabel3_Click);
            // 
            // rdPictureBox4
            // 
            this.rdPictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            this.rdPictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rdPictureBox4.Location = new System.Drawing.Point(267, 161);
            this.rdPictureBox4.Name = "rdPictureBox4";
            this.rdPictureBox4.Size = new System.Drawing.Size(18, 17);
            this.rdPictureBox4.TabIndex = 32;
            this.rdPictureBox4.TabStop = false;
            this.rdPictureBox4.Click += new System.EventHandler(this.rdPictureBox4_Click);
            // 
            // rdLabel4
            // 
            this.rdLabel4.AutoSize = true;
            this.rdLabel4.BackColor = System.Drawing.Color.Transparent;
            this.rdLabel4.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold);
            this.rdLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.rdLabel4.Location = new System.Drawing.Point(291, 150);
            this.rdLabel4.Name = "rdLabel4";
            this.rdLabel4.Size = new System.Drawing.Size(226, 37);
            this.rdLabel4.TabIndex = 31;
            this.rdLabel4.Text = "야간 및 휴일모드";
            this.rdLabel4.Click += new System.EventHandler(this.rdLabel4_Click);
            // 
            // FormOpenDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(1264, 679);
            this.Controls.Add(this.rdPictureBox4);
            this.Controls.Add(this.rdLabel4);
            this.Controls.Add(this.rdPictureBox3);
            this.Controls.Add(this.rdLabel3);
            this.Controls.Add(this.rdPictureBox2);
            this.Controls.Add(this.rdLabel2);
            this.Controls.Add(this.rdPictureBox1);
            this.Controls.Add(this.rdLabel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnOpenSOP);
            this.Controls.Add(this.dataGridViewVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelTreeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOpenDB";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormOpenDB";
            this.panelTreeView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVersion)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panelTreeView;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridView dataGridViewVersion;
		private System.Windows.Forms.Button btnOpenSOP;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioRegular;
		private System.Windows.Forms.RichTextBox textSOPInfo;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.RadioButton radioNormal;
		private System.Windows.Forms.RadioButton radioButton4;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.PictureBox rdPictureBox1;
		private System.Windows.Forms.Label rdLabel1;
		private System.Windows.Forms.PictureBox rdPictureBox2;
		private System.Windows.Forms.Label rdLabel2;
		private System.Windows.Forms.PictureBox rdPictureBox3;
		private System.Windows.Forms.Label rdLabel3;
		private System.Windows.Forms.PictureBox rdPictureBox4;
		private System.Windows.Forms.Label rdLabel4;
		private System.Windows.Forms.TreeView treeViewSOP;
		private System.Windows.Forms.DataGridViewTextBoxColumn column1;
	}
}