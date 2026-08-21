namespace BIMViewer
{
    partial class BasePlan
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridBackgroundDXF = new System.Windows.Forms.DataGridView();
            this.colVisible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Lock = new System.Windows.Forms.DataGridViewImageColumn();
            this.DeleteBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblAdd = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridBackgroundDXF)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridBackgroundDXF
            // 
            this.gridBackgroundDXF.AllowUserToAddRows = false;
            this.gridBackgroundDXF.AllowUserToDeleteRows = false;
            this.gridBackgroundDXF.AllowUserToResizeRows = false;
            this.gridBackgroundDXF.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridBackgroundDXF.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(54)))), ((int)(((byte)(84)))));
            this.gridBackgroundDXF.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridBackgroundDXF.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridBackgroundDXF.ColumnHeadersVisible = false;
            this.gridBackgroundDXF.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVisible,
            this.colName,
            this.Lock,
            this.DeleteBase});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(54)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(218)))), ((int)(((byte)(228)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(54)))));
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridBackgroundDXF.DefaultCellStyle = dataGridViewCellStyle9;
            this.gridBackgroundDXF.Location = new System.Drawing.Point(0, 23);
            this.gridBackgroundDXF.MultiSelect = false;
            this.gridBackgroundDXF.Name = "gridBackgroundDXF";
            this.gridBackgroundDXF.RowHeadersVisible = false;
            this.gridBackgroundDXF.RowTemplate.Height = 30;
            this.gridBackgroundDXF.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridBackgroundDXF.Size = new System.Drawing.Size(206, 302);
            this.gridBackgroundDXF.TabIndex = 2;
            this.gridBackgroundDXF.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridBackgroundDXF_CellContentClick);
            this.gridBackgroundDXF.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridBackgroundDXF_CellMouseLeave);
            this.gridBackgroundDXF.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.GridBackgroundDXF_CellMouseMove);
            this.gridBackgroundDXF.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridBackgroundDXF_CellValueChanged);
            // 
            // colVisible
            // 
            this.colVisible.HeaderText = "";
            this.colVisible.Name = "colVisible";
            this.colVisible.Width = 30;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colName.DefaultCellStyle = dataGridViewCellStyle7;
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            // 
            // Lock
            // 
            this.Lock.HeaderText = "Lock";
            this.Lock.Image = global::BIMViewer.Properties.Resources.Lock_Unlock_01;
            this.Lock.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.Lock.Name = "Lock";
            this.Lock.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Lock.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Lock.Width = 26;
            // 
            // DeleteBase
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("휴먼둥근헤드라인", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.DeleteBase.DefaultCellStyle = dataGridViewCellStyle8;
            this.DeleteBase.HeaderText = "-";
            this.DeleteBase.Name = "DeleteBase";
            this.DeleteBase.Width = 26;
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "Lock";
            this.dataGridViewImageColumn1.Image = global::BIMViewer.Properties.Resources.Lock_Unlock_01;
            this.dataGridViewImageColumn1.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewImageColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridViewImageColumn1.Width = 26;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::BIMViewer.Properties.Resources.green_gradation_01;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.lblAdd);
            this.panel1.Controls.Add(this.labelTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(206, 23);
            this.panel1.TabIndex = 3;
            // 
            // lblAdd
            // 
            this.lblAdd.AutoSize = true;
            this.lblAdd.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblAdd.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAdd.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblAdd.Location = new System.Drawing.Point(173, 5);
            this.lblAdd.Name = "lblAdd";
            this.lblAdd.Size = new System.Drawing.Size(17, 15);
            this.lblAdd.TabIndex = 5;
            this.lblAdd.Text = "+";
            this.lblAdd.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblAdd.Click += new System.EventHandler(this.LblAdd_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(5, 5);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(79, 16);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Base Plan";
            // 
            // BasePlan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridBackgroundDXF);
            this.Controls.Add(this.panel1);
            this.Name = "BasePlan";
            this.Size = new System.Drawing.Size(206, 325);
            this.Resize += new System.EventHandler(this.BasePlan_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.gridBackgroundDXF)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridBackgroundDXF;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewImageColumn Lock;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeleteBase;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.Label lblAdd;
    }
}