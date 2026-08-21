namespace RoadMan
{
	partial class FormShowLayer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormShowLayer));
			this.gridAll = new System.Windows.Forms.DataGridView();
			this.colVisibleAll = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colLayerNameAll = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colColorAll = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.pictureBoxInsert = new System.Windows.Forms.PictureBox();
			this.gridShow = new System.Windows.Forms.DataGridView();
			this.colVisibleProcess = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colLayerNameProcess = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colColorProcess = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.btnOK = new System.Windows.Forms.Button();
			this.pictureBoxRemove = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.gridAll)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxInsert)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridShow)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemove)).BeginInit();
			this.SuspendLayout();
			// 
			// gridAll
			// 
			this.gridAll.AllowUserToAddRows = false;
			this.gridAll.AllowUserToDeleteRows = false;
			this.gridAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridAll.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridAll.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVisibleAll,
            this.colLayerNameAll,
            this.colColorAll});
			this.gridAll.Location = new System.Drawing.Point(12, 42);
			this.gridAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.gridAll.Name = "gridAll";
			this.gridAll.ReadOnly = true;
			this.gridAll.RowHeadersVisible = false;
			this.gridAll.RowTemplate.Height = 23;
			this.gridAll.Size = new System.Drawing.Size(285, 471);
			this.gridAll.TabIndex = 1;
			this.gridAll.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridAll_CellContentClick);
			// 
			// colVisibleAll
			// 
			this.colVisibleAll.HeaderText = "상태";
			this.colVisibleAll.Name = "colVisibleAll";
			this.colVisibleAll.ReadOnly = true;
			this.colVisibleAll.Width = 40;
			// 
			// colLayerNameAll
			// 
			this.colLayerNameAll.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colLayerNameAll.HeaderText = "도면층 이름";
			this.colLayerNameAll.Name = "colLayerNameAll";
			this.colLayerNameAll.ReadOnly = true;
			// 
			// colColorAll
			// 
			this.colColorAll.HeaderText = "색상";
			this.colColorAll.Name = "colColorAll";
			this.colColorAll.ReadOnly = true;
			this.colColorAll.Width = 60;
			// 
			// pictureBoxInsert
			// 
			this.pictureBoxInsert.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.pictureBoxInsert.Image = global::RoadMan.Properties.Resources.right_arrow_normal;
			this.pictureBoxInsert.Location = new System.Drawing.Point(327, 221);
			this.pictureBoxInsert.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.pictureBoxInsert.Name = "pictureBoxInsert";
			this.pictureBoxInsert.Size = new System.Drawing.Size(51, 32);
			this.pictureBoxInsert.TabIndex = 2;
			this.pictureBoxInsert.TabStop = false;
			this.pictureBoxInsert.Click += new System.EventHandler(this.pictureBoxInsert_Click);
			// 
			// gridShow
			// 
			this.gridShow.AllowUserToAddRows = false;
			this.gridShow.AllowUserToDeleteRows = false;
			this.gridShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridShow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridShow.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVisibleProcess,
            this.colLayerNameProcess,
            this.colColorProcess});
			this.gridShow.Location = new System.Drawing.Point(414, 42);
			this.gridShow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.gridShow.Name = "gridShow";
			this.gridShow.ReadOnly = true;
			this.gridShow.RowHeadersVisible = false;
			this.gridShow.RowTemplate.Height = 23;
			this.gridShow.Size = new System.Drawing.Size(283, 471);
			this.gridShow.TabIndex = 1;
			this.gridShow.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridProcess_CellContentClick);
			// 
			// colVisibleProcess
			// 
			this.colVisibleProcess.HeaderText = "상태";
			this.colVisibleProcess.Name = "colVisibleProcess";
			this.colVisibleProcess.ReadOnly = true;
			this.colVisibleProcess.Width = 40;
			// 
			// colLayerNameProcess
			// 
			this.colLayerNameProcess.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colLayerNameProcess.HeaderText = "도면층 이름";
			this.colLayerNameProcess.Name = "colLayerNameProcess";
			this.colLayerNameProcess.ReadOnly = true;
			// 
			// colColorProcess
			// 
			this.colColorProcess.HeaderText = "색상";
			this.colColorProcess.Name = "colColorProcess";
			this.colColorProcess.ReadOnly = true;
			this.colColorProcess.Width = 60;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(552, 525);
			this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 30);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "닫기";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// pictureBoxRemove
			// 
			this.pictureBoxRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.pictureBoxRemove.Image = global::RoadMan.Properties.Resources.left_arrow_normal;
			this.pictureBoxRemove.Location = new System.Drawing.Point(327, 291);
			this.pictureBoxRemove.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.pictureBoxRemove.Name = "pictureBoxRemove";
			this.pictureBoxRemove.Size = new System.Drawing.Size(51, 32);
			this.pictureBoxRemove.TabIndex = 2;
			this.pictureBoxRemove.TabStop = false;
			this.pictureBoxRemove.Click += new System.EventHandler(this.pictureBoxRemove_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(423, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 15);
			this.label1.TabIndex = 4;
			this.label1.Text = "표시할 도면층";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 15);
			this.label2.TabIndex = 5;
			this.label2.Text = "전체 도면층";
			// 
			// FormShowLayer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(709, 564);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.pictureBoxRemove);
			this.Controls.Add(this.pictureBoxInsert);
			this.Controls.Add(this.gridShow);
			this.Controls.Add(this.gridAll);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MinimumSize = new System.Drawing.Size(200, 242);
			this.Name = "FormShowLayer";
			this.Text = "표시 도면층";
			this.Load += new System.EventHandler(this.FormProcessLayer_Load);
			this.Resize += new System.EventHandler(this.FormProcessLayer_Resize);
			((System.ComponentModel.ISupportInitialize)(this.gridAll)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxInsert)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridShow)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemove)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.DataGridView gridAll;
        private System.Windows.Forms.PictureBox pictureBoxInsert;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisibleAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLayerNameAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColorAll;
        private System.Windows.Forms.DataGridView gridShow;
		private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PictureBox pictureBoxRemove;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisibleProcess;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLayerNameProcess;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColorProcess;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
    }
}