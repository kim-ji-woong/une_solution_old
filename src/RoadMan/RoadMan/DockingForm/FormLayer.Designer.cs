namespace RoadMan
{
    partial class FormLayer
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
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.colVisible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colLayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colLayerColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuLayerPriority = new System.Windows.Forms.ToolStripMenuItem();
			this.menuLayerPriorityOneByOne = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSortLayer = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSortLayerInverse = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVisible,
            this.colLayerName,
            this.colLayerColor});
			this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView1.Location = new System.Drawing.Point(0, 38);
			this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.RowTemplate.Height = 23;
			this.dataGridView1.Size = new System.Drawing.Size(350, 262);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
			this.dataGridView1.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
			this.dataGridView1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick);
			this.dataGridView1.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView1_CurrentCellDirtyStateChanged);
			this.dataGridView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragDrop);
			this.dataGridView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragEnter);
			this.dataGridView1.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragOver);
			this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
			this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
			this.dataGridView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseMove);
			this.dataGridView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseUp);
			// 
			// colVisible
			// 
			this.colVisible.HeaderText = "상태";
			this.colVisible.Name = "colVisible";
			this.colVisible.Width = 40;
			// 
			// colLayerName
			// 
			this.colLayerName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colLayerName.HeaderText = "도면층 이름";
			this.colLayerName.Name = "colLayerName";
			this.colLayerName.ReadOnly = true;
			// 
			// colLayerColor
			// 
			this.colLayerColor.HeaderText = "색상";
			this.colLayerColor.Name = "colLayerColor";
			this.colLayerColor.ReadOnly = true;
			this.colLayerColor.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colLayerColor.Width = 60;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLayerPriority,
            this.menuLayerPriorityOneByOne,
            this.menuSortLayer,
            this.menuSortLayerInverse});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(267, 92);
			// 
			// menuLayerPriority
			// 
			this.menuLayerPriority.Name = "menuLayerPriority";
			this.menuLayerPriority.Size = new System.Drawing.Size(266, 22);
			this.menuLayerPriority.Text = "도면층 우선순위 보기";
			this.menuLayerPriority.Click += new System.EventHandler(this.menuLayerPriority_Click);
			// 
			// menuLayerPriorityOneByOne
			// 
			this.menuLayerPriorityOneByOne.Name = "menuLayerPriorityOneByOne";
			this.menuLayerPriorityOneByOne.Size = new System.Drawing.Size(266, 22);
			this.menuLayerPriorityOneByOne.Text = "도면층 우선순위 매기기";
			this.menuLayerPriorityOneByOne.Click += new System.EventHandler(this.menuLayerPriorityOneByOne_Click);
			// 
			// menuSortLayer
			// 
			this.menuSortLayer.Enabled = false;
			this.menuSortLayer.Name = "menuSortLayer";
			this.menuSortLayer.Size = new System.Drawing.Size(266, 22);
			this.menuSortLayer.Text = "도면층 우선순위대로 정렬하기";
			this.menuSortLayer.Click += new System.EventHandler(this.menuSortLayer_Click);
			// 
			// menuSortLayerInverse
			// 
			this.menuSortLayerInverse.Enabled = false;
			this.menuSortLayerInverse.Name = "menuSortLayerInverse";
			this.menuSortLayerInverse.Size = new System.Drawing.Size(266, 22);
			this.menuSortLayerInverse.Text = "도면층 우선순위 역순으로 정렬하기";
			this.menuSortLayerInverse.Click += new System.EventHandler(this.menuSortLayerInverse_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(350, 38);
			this.panel1.TabIndex = 1;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.BackgroundImage = global::RoadMan.Properties.Resources.닫기_normal;
			this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.button1.FlatAppearance.BorderSize = 0;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(310, 1);
			this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(37, 35);
			this.button1.TabIndex = 4;
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label1.Location = new System.Drawing.Point(8, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 21);
			this.label1.TabIndex = 3;
			this.label1.Text = "활성화된 도면층";
			// 
			// FormLayer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(350, 300);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "FormLayer";
			this.ShowInTaskbar = false;
			this.Text = "FormLayer";
			this.Load += new System.EventHandler(this.FormLayer_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLayerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLayerColor;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuLayerPriority;
        private System.Windows.Forms.ToolStripMenuItem menuSortLayer;
        private System.Windows.Forms.ToolStripMenuItem menuSortLayerInverse;
        private System.Windows.Forms.ToolStripMenuItem menuLayerPriorityOneByOne;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
    }
}