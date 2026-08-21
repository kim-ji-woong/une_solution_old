namespace TeamManagementSystem
{
    partial class FormLeftTeamState
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
            this.groupBoxVersion = new System.Windows.Forms.GroupBox();
            this.dataGridVersion = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxVersion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridVersion)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxVersion
            // 
            this.groupBoxVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxVersion.Controls.Add(this.dataGridVersion);
            this.groupBoxVersion.Location = new System.Drawing.Point(12, 12);
            this.groupBoxVersion.Name = "groupBoxVersion";
            this.groupBoxVersion.Padding = new System.Windows.Forms.Padding(5);
            this.groupBoxVersion.Size = new System.Drawing.Size(260, 142);
            this.groupBoxVersion.TabIndex = 0;
            this.groupBoxVersion.TabStop = false;
            this.groupBoxVersion.Text = "버전";
            // 
            // dataGridVersion
            // 
            this.dataGridVersion.AllowUserToAddRows = false;
            this.dataGridVersion.AllowUserToDeleteRows = false;
            this.dataGridVersion.AllowUserToResizeColumns = false;
            this.dataGridVersion.AllowUserToResizeRows = false;
            this.dataGridVersion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridVersion.ColumnHeadersVisible = false;
            this.dataGridVersion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridVersion.Location = new System.Drawing.Point(5, 19);
            this.dataGridVersion.MultiSelect = false;
            this.dataGridVersion.Name = "dataGridVersion";
            this.dataGridVersion.ReadOnly = true;
            this.dataGridVersion.RowHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridVersion.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridVersion.RowTemplate.Height = 23;
            this.dataGridVersion.Size = new System.Drawing.Size(250, 118);
            this.dataGridVersion.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.FillWeight = 85F;
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 85;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // FormLeftTeamState
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.groupBoxVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLeftTeamState";
            this.Text = "조직구성 현황";
            this.groupBoxVersion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridVersion)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxVersion;
        private System.Windows.Forms.DataGridView dataGridVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
    }
}