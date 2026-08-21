namespace SOPMonitoringSystem
{
    partial class DockingRightProgress
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridProgress = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridSOPMission = new System.Windows.Forms.DataGridView();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colprogress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridProgress)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSOPMission)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridProgress);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 92);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SOP 진행 시간 정보";
            // 
            // dataGridProgress
            // 
            this.dataGridProgress.AllowUserToAddRows = false;
            this.dataGridProgress.AllowUserToDeleteRows = false;
            this.dataGridProgress.AllowUserToResizeColumns = false;
            this.dataGridProgress.AllowUserToResizeRows = false;
            this.dataGridProgress.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridProgress.ColumnHeadersVisible = false;
            this.dataGridProgress.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Time,
            this.TimeValue});
            this.dataGridProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridProgress.Location = new System.Drawing.Point(3, 17);
            this.dataGridProgress.Name = "dataGridProgress";
            this.dataGridProgress.ReadOnly = true;
            this.dataGridProgress.RowHeadersVisible = false;
            this.dataGridProgress.RowTemplate.Height = 23;
            this.dataGridProgress.Size = new System.Drawing.Size(254, 72);
            this.dataGridProgress.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridSOPMission);
            this.groupBox2.Location = new System.Drawing.Point(12, 110);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 161);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SOP 임무 수행 정보";
            // 
            // dataGridSOPMission
            // 
            this.dataGridSOPMission.AllowUserToAddRows = false;
            this.dataGridSOPMission.AllowUserToDeleteRows = false;
            this.dataGridSOPMission.AllowUserToResizeColumns = false;
            this.dataGridSOPMission.AllowUserToResizeRows = false;
            this.dataGridSOPMission.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridSOPMission.ColumnHeadersVisible = false;
            this.dataGridSOPMission.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colprogress,
            this.colValue});
            this.dataGridSOPMission.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridSOPMission.Location = new System.Drawing.Point(3, 17);
            this.dataGridSOPMission.Name = "dataGridSOPMission";
            this.dataGridSOPMission.ReadOnly = true;
            this.dataGridSOPMission.RowHeadersVisible = false;
            this.dataGridSOPMission.RowTemplate.Height = 23;
            this.dataGridSOPMission.Size = new System.Drawing.Size(254, 141);
            this.dataGridSOPMission.TabIndex = 0;
            // 
            // Time
            // 
            this.Time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Time.HeaderText = "진행시간";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            // 
            // TimeValue
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.TimeValue.DefaultCellStyle = dataGridViewCellStyle1;
            this.TimeValue.HeaderText = "시간정보";
            this.TimeValue.Name = "TimeValue";
            this.TimeValue.ReadOnly = true;
            this.TimeValue.Width = 150;
            // 
            // colprogress
            // 
            this.colprogress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colprogress.HeaderText = "임무";
            this.colprogress.Name = "colprogress";
            this.colprogress.ReadOnly = true;
            // 
            // colValue
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colValue.DefaultCellStyle = dataGridViewCellStyle2;
            this.colValue.HeaderText = "비고";
            this.colValue.Name = "colValue";
            this.colValue.ReadOnly = true;
            this.colValue.Width = 70;
            // 
            // DockingRightProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(5, 5);
            this.ClientSize = new System.Drawing.Size(284, 292);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingRightProgress";
            this.Text = "SOP 진행 현황";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridProgress)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSOPMission)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dataGridProgress;
        private System.Windows.Forms.DataGridView dataGridSOPMission;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colprogress;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
    }
}