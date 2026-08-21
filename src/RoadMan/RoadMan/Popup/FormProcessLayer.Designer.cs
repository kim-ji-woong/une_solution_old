namespace RoadMan
{
    partial class FormProcessLayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcessLayer));
            this.radioComplete = new System.Windows.Forms.RadioButton();
            this.radioIncomplete = new System.Windows.Forms.RadioButton();
            this.radioPartialComplete = new System.Windows.Forms.RadioButton();
            this.gridAll = new System.Windows.Forms.DataGridView();
            this.colVisibleAll = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLayerNameAll = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColorAll = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBoxInsert = new System.Windows.Forms.PictureBox();
            this.gridProcess = new System.Windows.Forms.DataGridView();
            this.colVisibleProcess = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLayerNameProcess = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColorProcess = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pictureBoxRemove = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInsert)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridProcess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemove)).BeginInit();
            this.SuspendLayout();
            // 
            // radioComplete
            // 
            this.radioComplete.AutoSize = true;
            this.radioComplete.Checked = true;
            this.radioComplete.Location = new System.Drawing.Point(12, 12);
            this.radioComplete.Name = "radioComplete";
            this.radioComplete.Size = new System.Drawing.Size(47, 16);
            this.radioComplete.TabIndex = 0;
            this.radioComplete.TabStop = true;
            this.radioComplete.Text = "개설";
            this.radioComplete.UseVisualStyleBackColor = true;
            this.radioComplete.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioIncomplete
            // 
            this.radioIncomplete.AutoSize = true;
            this.radioIncomplete.Location = new System.Drawing.Point(90, 12);
            this.radioIncomplete.Name = "radioIncomplete";
            this.radioIncomplete.Size = new System.Drawing.Size(59, 16);
            this.radioIncomplete.TabIndex = 0;
            this.radioIncomplete.Text = "미개설";
            this.radioIncomplete.UseVisualStyleBackColor = true;
            this.radioIncomplete.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioPartialComplete
            // 
            this.radioPartialComplete.AutoSize = true;
            this.radioPartialComplete.Location = new System.Drawing.Point(182, 12);
            this.radioPartialComplete.Name = "radioPartialComplete";
            this.radioPartialComplete.Size = new System.Drawing.Size(83, 16);
            this.radioPartialComplete.TabIndex = 0;
            this.radioPartialComplete.Text = "폭원미개설";
            this.radioPartialComplete.UseVisualStyleBackColor = true;
            this.radioPartialComplete.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
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
            this.gridAll.Location = new System.Drawing.Point(12, 54);
            this.gridAll.Name = "gridAll";
            this.gridAll.ReadOnly = true;
            this.gridAll.RowHeadersVisible = false;
            this.gridAll.RowTemplate.Height = 23;
            this.gridAll.Size = new System.Drawing.Size(253, 356);
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
            this.pictureBoxInsert.Location = new System.Drawing.Point(277, 177);
            this.pictureBoxInsert.Name = "pictureBoxInsert";
            this.pictureBoxInsert.Size = new System.Drawing.Size(51, 32);
            this.pictureBoxInsert.TabIndex = 2;
            this.pictureBoxInsert.TabStop = false;
            this.pictureBoxInsert.Click += new System.EventHandler(this.pictureBoxInsert_Click);
            // 
            // gridProcess
            // 
            this.gridProcess.AllowUserToAddRows = false;
            this.gridProcess.AllowUserToDeleteRows = false;
            this.gridProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridProcess.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridProcess.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVisibleProcess,
            this.colLayerNameProcess,
            this.colColorProcess});
            this.gridProcess.Location = new System.Drawing.Point(343, 54);
            this.gridProcess.Name = "gridProcess";
            this.gridProcess.ReadOnly = true;
            this.gridProcess.RowHeadersVisible = false;
            this.gridProcess.RowTemplate.Height = 23;
            this.gridProcess.Size = new System.Drawing.Size(253, 356);
            this.gridProcess.TabIndex = 1;
            this.gridProcess.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridProcess_CellContentClick);
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
            this.btnOK.Location = new System.Drawing.Point(474, 420);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(55, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(541, 420);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(55, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pictureBoxRemove
            // 
            this.pictureBoxRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBoxRemove.Image = global::RoadMan.Properties.Resources.left_arrow_normal;
            this.pictureBoxRemove.Location = new System.Drawing.Point(277, 233);
            this.pictureBoxRemove.Name = "pictureBoxRemove";
            this.pictureBoxRemove.Size = new System.Drawing.Size(51, 32);
            this.pictureBoxRemove.TabIndex = 2;
            this.pictureBoxRemove.TabStop = false;
            this.pictureBoxRemove.Click += new System.EventHandler(this.pictureBoxRemove_Click);
            // 
            // FormProcessLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 451);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pictureBoxRemove);
            this.Controls.Add(this.pictureBoxInsert);
            this.Controls.Add(this.gridProcess);
            this.Controls.Add(this.gridAll);
            this.Controls.Add(this.radioPartialComplete);
            this.Controls.Add(this.radioIncomplete);
            this.Controls.Add(this.radioComplete);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "FormProcessLayer";
            this.Text = "집행 도면층";
            this.Load += new System.EventHandler(this.FormProcessLayer_Load);
            this.Resize += new System.EventHandler(this.FormProcessLayer_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.gridAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInsert)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridProcess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemove)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioComplete;
        private System.Windows.Forms.RadioButton radioIncomplete;
        private System.Windows.Forms.RadioButton radioPartialComplete;
        private System.Windows.Forms.DataGridView gridAll;
        private System.Windows.Forms.PictureBox pictureBoxInsert;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisibleAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLayerNameAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColorAll;
        private System.Windows.Forms.DataGridView gridProcess;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pictureBoxRemove;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisibleProcess;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLayerNameProcess;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColorProcess;
    }
}