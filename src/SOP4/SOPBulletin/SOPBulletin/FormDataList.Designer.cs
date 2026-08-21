namespace SOPBulletin
{
    partial class FormDataList
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
            this.colNO1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVisible1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNo2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVisible2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNo3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVisible3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNO1,
            this.colVisible1,
            this.colNo2,
            this.colVisible2,
            this.colNo3,
            this.colVisible3});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(284, 261);
            this.dataGridView1.TabIndex = 0;
            // 
            // colNO1
            // 
            this.colNO1.HeaderText = "번호";
            this.colNO1.Name = "colNO1";
            this.colNO1.Width = 60;
            // 
            // colVisible1
            // 
            this.colVisible1.HeaderText = "";
            this.colVisible1.Name = "colVisible1";
            this.colVisible1.Width = 30;
            // 
            // colNo2
            // 
            this.colNo2.HeaderText = "번호";
            this.colNo2.Name = "colNo2";
            this.colNo2.Width = 60;
            // 
            // colVisible2
            // 
            this.colVisible2.HeaderText = "";
            this.colVisible2.Name = "colVisible2";
            this.colVisible2.Width = 30;
            // 
            // colNo3
            // 
            this.colNo3.HeaderText = "번호";
            this.colNo3.Name = "colNo3";
            this.colNo3.Width = 60;
            // 
            // colVisible3
            // 
            this.colVisible3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colVisible3.HeaderText = "";
            this.colVisible3.Name = "colVisible3";
            // 
            // FormDataList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FormDataList";
            this.Text = "데이터 리스트";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDataList_FormClosing);
            this.Load += new System.EventHandler(this.FormDataList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNO1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible3;
    }
}