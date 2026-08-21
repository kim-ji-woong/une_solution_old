namespace SOPDisasterSystem
{
    partial class FormBottomLog
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
            this.tabCtrlBottom = new System.Windows.Forms.TabControl();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.dataGridViewLog = new System.Windows.Forms.DataGridView();
            this.tabGraph = new System.Windows.Forms.TabPage();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabCtrlBottom.SuspendLayout();
            this.tabLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLog)).BeginInit();
            this.SuspendLayout();
            // 
            // tabCtrlBottom
            // 
            this.tabCtrlBottom.Controls.Add(this.tabLog);
            this.tabCtrlBottom.Controls.Add(this.tabGraph);
            this.tabCtrlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrlBottom.Location = new System.Drawing.Point(0, 0);
            this.tabCtrlBottom.Name = "tabCtrlBottom";
            this.tabCtrlBottom.SelectedIndex = 0;
            this.tabCtrlBottom.Size = new System.Drawing.Size(732, 207);
            this.tabCtrlBottom.TabIndex = 0;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.dataGridViewLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(724, 181);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = " 로그 ";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // dataGridViewLog
            // 
            this.dataGridViewLog.AllowUserToAddRows = false;
            this.dataGridViewLog.AllowUserToDeleteRows = false;
            this.dataGridViewLog.AllowUserToResizeColumns = false;
            this.dataGridViewLog.AllowUserToResizeRows = false;
            this.dataGridViewLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridViewLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewLog.GridColor = System.Drawing.SystemColors.ActiveBorder;
            this.dataGridViewLog.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewLog.Name = "dataGridViewLog";
            this.dataGridViewLog.ReadOnly = true;
            this.dataGridViewLog.RowHeadersVisible = false;
            this.dataGridViewLog.RowTemplate.Height = 23;
            this.dataGridViewLog.Size = new System.Drawing.Size(718, 175);
            this.dataGridViewLog.TabIndex = 0;
            // 
            // tabGraph
            // 
            this.tabGraph.Location = new System.Drawing.Point(4, 22);
            this.tabGraph.Name = "tabGraph";
            this.tabGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabGraph.Size = new System.Drawing.Size(724, 181);
            this.tabGraph.TabIndex = 1;
            this.tabGraph.Text = "그래프";
            this.tabGraph.UseVisualStyleBackColor = true;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "시간";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 150;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "이벤트";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // FormBottomLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 207);
            this.Controls.Add(this.tabCtrlBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormBottomLog";
            this.Text = "FormBottomLog";
            this.tabCtrlBottom.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabCtrlBottom;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TabPage tabGraph;
        private System.Windows.Forms.DataGridView dataGridViewLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
    }
}