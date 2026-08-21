namespace ServerConverter
{
    partial class FormConfig
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.colWebserverURL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFile1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFile1Line = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFile2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFile2Line = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFile3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFile3Line = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colWebserverURL,
            this.colFile1,
            this.colFile1Line,
            this.colFile2,
            this.colFile2Line,
            this.colFile3,
            this.colFile3Line});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 100;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1189, 150);
            this.dataGridView1.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(1067, 156);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(55, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(1128, 156);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(55, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // colWebserverURL
            // 
            this.colWebserverURL.HeaderText = "웹서버 URL";
            this.colWebserverURL.Name = "colWebserverURL";
            this.colWebserverURL.Width = 200;
            // 
            // colFile1
            // 
            this.colFile1.HeaderText = "File1";
            this.colFile1.Name = "colFile1";
            this.colFile1.Width = 200;
            // 
            // colFile1Line
            // 
            this.colFile1Line.HeaderText = "선택Line";
            this.colFile1Line.Name = "colFile1Line";
            // 
            // colFile2
            // 
            this.colFile2.HeaderText = "File2";
            this.colFile2.Name = "colFile2";
            this.colFile2.Width = 200;
            // 
            // colFile2Line
            // 
            this.colFile2Line.HeaderText = "선택Line";
            this.colFile2Line.Name = "colFile2Line";
            // 
            // colFile3
            // 
            this.colFile3.HeaderText = "File3";
            this.colFile3.Name = "colFile3";
            this.colFile3.Width = 200;
            // 
            // colFile3Line
            // 
            this.colFile3Line.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFile3Line.HeaderText = "선택Line";
            this.colFile3Line.Name = "colFile3Line";
            // 
            // FormConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1189, 185);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FormConfig";
            this.Text = "설정";
            this.Load += new System.EventHandler(this.FormConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWebserverURL;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFile1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFile1Line;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFile2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFile2Line;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFile3;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFile3Line;
    }
}