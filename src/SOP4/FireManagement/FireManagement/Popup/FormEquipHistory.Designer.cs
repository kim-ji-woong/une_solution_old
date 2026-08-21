namespace FireManagement
{
    partial class FormEquipHistory
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
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxRFIDTagID = new System.Windows.Forms.TextBox();
            this.textBoxEquipID = new System.Windows.Forms.TextBox();
            this.textBoxEquipType = new System.Windows.Forms.TextBox();
            this.dataGridViewHistory = new System.Windows.Forms.DataGridView();
            this.colChecker = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCheckTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOpinion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(12, 63);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 21);
            this.label8.TabIndex = 30;
            this.label8.Text = "Tag 이름";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(13, 92);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 21);
            this.label7.TabIndex = 28;
            this.label7.Text = "관리번호";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(358, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 21);
            this.label3.TabIndex = 29;
            this.label3.Text = "설비종류";
            // 
            // textBoxRFIDTagID
            // 
            this.textBoxRFIDTagID.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxRFIDTagID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxRFIDTagID.Location = new System.Drawing.Point(91, 63);
            this.textBoxRFIDTagID.Name = "textBoxRFIDTagID";
            this.textBoxRFIDTagID.ReadOnly = true;
            this.textBoxRFIDTagID.Size = new System.Drawing.Size(252, 21);
            this.textBoxRFIDTagID.TabIndex = 27;
            // 
            // textBoxEquipID
            // 
            this.textBoxEquipID.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxEquipID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxEquipID.Location = new System.Drawing.Point(90, 92);
            this.textBoxEquipID.Name = "textBoxEquipID";
            this.textBoxEquipID.ReadOnly = true;
            this.textBoxEquipID.Size = new System.Drawing.Size(252, 21);
            this.textBoxEquipID.TabIndex = 32;
            // 
            // textBoxEquipType
            // 
            this.textBoxEquipType.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxEquipType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxEquipType.Location = new System.Drawing.Point(434, 63);
            this.textBoxEquipType.Name = "textBoxEquipType";
            this.textBoxEquipType.ReadOnly = true;
            this.textBoxEquipType.Size = new System.Drawing.Size(216, 21);
            this.textBoxEquipType.TabIndex = 31;
            // 
            // dataGridViewHistory
            // 
            this.dataGridViewHistory.AllowUserToAddRows = false;
            this.dataGridViewHistory.AllowUserToDeleteRows = false;
            this.dataGridViewHistory.AllowUserToResizeColumns = false;
            this.dataGridViewHistory.AllowUserToResizeRows = false;
            this.dataGridViewHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colChecker,
            this.colStatus,
            this.colCheckTime,
            this.colOpinion});
            this.dataGridViewHistory.Location = new System.Drawing.Point(16, 125);
            this.dataGridViewHistory.Name = "dataGridViewHistory";
            this.dataGridViewHistory.ReadOnly = true;
            this.dataGridViewHistory.RowHeadersVisible = false;
            this.dataGridViewHistory.RowTemplate.Height = 23;
            this.dataGridViewHistory.Size = new System.Drawing.Size(634, 234);
            this.dataGridViewHistory.TabIndex = 33;
            // 
            // colChecker
            // 
            this.colChecker.HeaderText = "담당자";
            this.colChecker.Name = "colChecker";
            this.colChecker.ReadOnly = true;
            this.colChecker.Visible = false;
            this.colChecker.Width = 70;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "상태";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 70;
            // 
            // colCheckTime
            // 
            this.colCheckTime.HeaderText = "점검 시간";
            this.colCheckTime.Name = "colCheckTime";
            this.colCheckTime.ReadOnly = true;
            this.colCheckTime.Width = 130;
            // 
            // colOpinion
            // 
            this.colOpinion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colOpinion.HeaderText = "점검의견";
            this.colOpinion.Name = "colOpinion";
            this.colOpinion.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(8, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(211, 40);
            this.label1.TabIndex = 35;
            this.label1.Text = "시설 점검 이력";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDown);
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label1_MouseMove);
            this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label1_MouseUp);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.BackgroundImage = global::FireManagement.Properties.Resources.Docking_nomal_Button;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button2.Location = new System.Drawing.Point(492, 365);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(161, 42);
            this.button2.TabIndex = 36;
            this.button2.Text = "닫기";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormEquipHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.ClientSize = new System.Drawing.Size(671, 418);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewHistory);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxRFIDTagID);
            this.Controls.Add(this.textBoxEquipID);
            this.Controls.Add(this.textBoxEquipType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "FormEquipHistory";
            this.Text = "설비 점검 이력";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormEquipHistory_FormClosed);
            this.Load += new System.EventHandler(this.FormEquipHistory_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormEquipHistory_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormEquipHistory_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormEquipHistory_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxRFIDTagID;
        private System.Windows.Forms.TextBox textBoxEquipID;
        private System.Windows.Forms.TextBox textBoxEquipType;
        private System.Windows.Forms.DataGridView dataGridViewHistory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChecker;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCheckTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOpinion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
    }
}