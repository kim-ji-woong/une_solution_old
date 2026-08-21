namespace SOPMonitoringSystem
{
    partial class FormRighSummary
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
            this.dataGridSummary = new System.Windows.Forms.DataGridView();
            this.colTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSummary)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridSummary
            // 
            this.dataGridSummary.AllowUserToAddRows = false;
            this.dataGridSummary.AllowUserToDeleteRows = false;
            this.dataGridSummary.AllowUserToResizeColumns = false;
            this.dataGridSummary.AllowUserToResizeRows = false;
            this.dataGridSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridSummary.ColumnHeadersVisible = false;
            this.dataGridSummary.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTitle,
            this.colData});
            this.dataGridSummary.Location = new System.Drawing.Point(12, 12);
            this.dataGridSummary.Name = "dataGridSummary";
            this.dataGridSummary.ReadOnly = true;
            this.dataGridSummary.RowHeadersVisible = false;
            this.dataGridSummary.RowTemplate.Height = 23;
            this.dataGridSummary.Size = new System.Drawing.Size(260, 95);
            this.dataGridSummary.TabIndex = 0;
            // 
            // colTitle
            // 
            this.colTitle.HeaderText = "항목";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            this.colTitle.Width = 60;
            // 
            // colData
            // 
            this.colData.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colData.HeaderText = "내용";
            this.colData.Name = "colData";
            this.colData.ReadOnly = true;
            // 
            // FormRighSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.dataGridSummary);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRighSummary";
            this.Text = "SOP 개요";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSummary)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridSummary;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colData;
    }
}