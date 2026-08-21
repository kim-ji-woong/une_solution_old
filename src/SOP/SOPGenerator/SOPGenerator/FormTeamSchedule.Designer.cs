namespace SOPGen
{
    partial class FormTeamSchedule
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
            this.teamScheduleDataGrid = new System.Windows.Forms.DataGridView();
            this.colBeginTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFullPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new ZBobb.AlphaBlendTextBox();
            this.hourTextBox = new System.Windows.Forms.TextBox();
            this.hourLabel = new System.Windows.Forms.Label();
            this.minuteTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.teamScheduleDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // teamScheduleDataGrid
            // 
            this.teamScheduleDataGrid.AllowUserToAddRows = false;
            this.teamScheduleDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.teamScheduleDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBeginTime,
            this.colFullPath});
            this.teamScheduleDataGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.teamScheduleDataGrid.Location = new System.Drawing.Point(0, 0);
            this.teamScheduleDataGrid.Name = "teamScheduleDataGrid";
            this.teamScheduleDataGrid.ReadOnly = true;
            this.teamScheduleDataGrid.RowHeadersVisible = false;
            this.teamScheduleDataGrid.RowTemplate.Height = 23;
            this.teamScheduleDataGrid.Size = new System.Drawing.Size(432, 174);
            this.teamScheduleDataGrid.TabIndex = 0;
            // 
            // colBeginTime
            // 
            this.colBeginTime.HeaderText = "시작시간(시간:분)";
            this.colBeginTime.Name = "colBeginTime";
            this.colBeginTime.ReadOnly = true;
            this.colBeginTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colBeginTime.Width = 130;
            // 
            // colFullPath
            // 
            this.colFullPath.HeaderText = "팀 이름";
            this.colFullPath.Name = "colFullPath";
            this.colFullPath.ReadOnly = true;
            this.colFullPath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colFullPath.Width = 300;
            // 
            // textBox1
            // 
            this.textBox1.BackAlpha = 10;
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(0, 180);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(283, 31);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = "이미 같은 이름의 팀/팀원이 존재합니다.\r\n기존에 존재하는 것과 다른 시작 시간을 지정하세요.";
            // 
            // hourTextBox
            // 
            this.hourTextBox.Location = new System.Drawing.Point(12, 217);
            this.hourTextBox.Name = "hourTextBox";
            this.hourTextBox.Size = new System.Drawing.Size(29, 21);
            this.hourTextBox.TabIndex = 7;
            this.hourTextBox.Text = "0";
            this.hourTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.hourTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hourTextBox_KeyDown);
            this.hourTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.hourTextBox_KeyPress);
            // 
            // hourLabel
            // 
            this.hourLabel.AutoSize = true;
            this.hourLabel.Location = new System.Drawing.Point(44, 220);
            this.hourLabel.Name = "hourLabel";
            this.hourLabel.Size = new System.Drawing.Size(12, 12);
            this.hourLabel.TabIndex = 8;
            this.hourLabel.Text = "h";
            // 
            // minuteTextBox
            // 
            this.minuteTextBox.Location = new System.Drawing.Point(62, 217);
            this.minuteTextBox.Name = "minuteTextBox";
            this.minuteTextBox.Size = new System.Drawing.Size(29, 21);
            this.minuteTextBox.TabIndex = 7;
            this.minuteTextBox.Text = "0";
            this.minuteTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.minuteTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.minuteTextBox_KeyDown);
            this.minuteTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.minuteTextBox_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 220);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "m";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(347, 212);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 28);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(268, 212);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(73, 28);
            this.btnSelect.TabIndex = 9;
            this.btnSelect.Text = "선택";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // FormTeamSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 250);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hourLabel);
            this.Controls.Add(this.minuteTextBox);
            this.Controls.Add(this.hourTextBox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.teamScheduleDataGrid);
            this.Name = "FormTeamSchedule";
            this.ShowInTaskbar = false;
            this.Text = "중복된 팀의 스케쥴 설정";
            this.Load += new System.EventHandler(this.FormTeamSchedule_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormTeamSchedule_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.teamScheduleDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView teamScheduleDataGrid;
        private ZBobb.AlphaBlendTextBox textBox1;
        private System.Windows.Forms.TextBox hourTextBox;
        private System.Windows.Forms.Label hourLabel;
        private System.Windows.Forms.TextBox minuteTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBeginTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFullPath;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSelect;
    }
}