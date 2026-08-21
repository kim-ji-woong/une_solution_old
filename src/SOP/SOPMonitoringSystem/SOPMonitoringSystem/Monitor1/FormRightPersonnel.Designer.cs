namespace SOPMonitoringSystem
{
    partial class FormRightPersonnel
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
            this.dataGridPersonnel = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridNetwork = new System.Windows.Forms.DataGridView();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SMS = new System.Windows.Forms.DataGridViewImageColumn();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPersonnel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridNetwork)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridPersonnel
            // 
            this.dataGridPersonnel.AllowUserToAddRows = false;
            this.dataGridPersonnel.AllowUserToDeleteRows = false;
            this.dataGridPersonnel.AllowUserToResizeColumns = false;
            this.dataGridPersonnel.AllowUserToResizeRows = false;
            this.dataGridPersonnel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPersonnel.ColumnHeadersVisible = false;
            this.dataGridPersonnel.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridPersonnel.Location = new System.Drawing.Point(12, 12);
            this.dataGridPersonnel.MultiSelect = false;
            this.dataGridPersonnel.Name = "dataGridPersonnel";
            this.dataGridPersonnel.ReadOnly = true;
            this.dataGridPersonnel.RowHeadersVisible = false;
            this.dataGridPersonnel.RowTemplate.Height = 23;
            this.dataGridPersonnel.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridPersonnel.Size = new System.Drawing.Size(260, 95);
            this.dataGridPersonnel.TabIndex = 0;
            this.dataGridPersonnel.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridPersonnel_CellClick);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 70;
            // 
            // dataGridNetwork
            // 
            this.dataGridNetwork.AllowUserToAddRows = false;
            this.dataGridNetwork.AllowUserToDeleteRows = false;
            this.dataGridNetwork.AllowUserToResizeColumns = false;
            this.dataGridNetwork.AllowUserToResizeRows = false;
            this.dataGridNetwork.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridNetwork.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.Column4,
            this.SMS});
            this.dataGridNetwork.Location = new System.Drawing.Point(12, 129);
            this.dataGridNetwork.Name = "dataGridNetwork";
            this.dataGridNetwork.ReadOnly = true;
            this.dataGridNetwork.RowHeadersVisible = false;
            this.dataGridNetwork.RowTemplate.Height = 23;
            this.dataGridNetwork.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridNetwork.Size = new System.Drawing.Size(260, 121);
            this.dataGridNetwork.TabIndex = 1;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "업무수행자";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 90;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "CellPhone";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // SMS
            // 
            this.SMS.HeaderText = "";
            this.SMS.Name = "SMS";
            this.SMS.ReadOnly = true;
            this.SMS.Width = 50;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SOPMonitoringSystem.Properties.Resources.btn_arrow;
            this.pictureBox1.Location = new System.Drawing.Point(122, 109);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 17);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // FormRightPersonnel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(5, 5);
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.dataGridNetwork);
            this.Controls.Add(this.dataGridPersonnel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRightPersonnel";
            this.Text = "SOP 요원 현황";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPersonnel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridNetwork)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridPersonnel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridView dataGridNetwork;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewImageColumn SMS;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}