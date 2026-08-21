namespace SOPMonitoringSystem
{
    partial class FormBottomSOPLog
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
            this.gridLog = new System.Windows.Forms.DataGridView();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTask = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMember = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProcess = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridLog)).BeginInit();
            this.SuspendLayout();
            // 
            // gridLog
            // 
            this.gridLog.AllowUserToAddRows = false;
            this.gridLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime,
            this.colStep,
            this.colProcess,
            this.colMember,
            this.colTask,
            this.colStatus});
            this.gridLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLog.Location = new System.Drawing.Point(0, 0);
            this.gridLog.Name = "gridLog";
            this.gridLog.RowHeadersVisible = false;
            this.gridLog.RowTemplate.Height = 23;
            this.gridLog.Size = new System.Drawing.Size(284, 262);
            this.gridLog.TabIndex = 0;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "상태";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // colTask
            // 
            this.colTask.HeaderText = "임무";
            this.colTask.Name = "colTask";
            this.colTask.ReadOnly = true;
            this.colTask.Width = 300;
            // 
            // colMember
            // 
            this.colMember.HeaderText = "담당";
            this.colMember.Name = "colMember";
            this.colMember.ReadOnly = true;
            this.colMember.Width = 200;
            // 
            // colProcess
            // 
            this.colProcess.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colProcess.HeaderText = "프로세스";
            this.colProcess.MinimumWidth = 100;
            this.colProcess.Name = "colProcess";
            this.colProcess.ReadOnly = true;
            // 
            // colStep
            // 
            this.colStep.HeaderText = "단계";
            this.colStep.Name = "colStep";
            this.colStep.ReadOnly = true;
            // 
            // colTime
            // 
            this.colTime.HeaderText = "시간";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            // 
            // FormBottomSOPLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.gridLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormBottomSOPLog";
            this.Text = "SOP Log";
            ((System.ComponentModel.ISupportInitialize)(this.gridLog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStep;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProcess;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMember;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTask;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
    }
}