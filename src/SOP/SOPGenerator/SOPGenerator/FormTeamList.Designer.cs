namespace SOPGen
{
    partial class FormTeamList
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.teamDataGrid = new System.Windows.Forms.DataGridView();
            this.Item = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new ZBobb.AlphaBlendTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.teamDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(210, 225);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(131, 225);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(73, 28);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = "선택";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // teamDataGrid
            // 
            this.teamDataGrid.AllowUserToAddRows = false;
            this.teamDataGrid.AllowUserToDeleteRows = false;
            this.teamDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.teamDataGrid.ColumnHeadersVisible = false;
            this.teamDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Item});
            this.teamDataGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.teamDataGrid.Location = new System.Drawing.Point(0, 0);
            this.teamDataGrid.MultiSelect = false;
            this.teamDataGrid.Name = "teamDataGrid";
            this.teamDataGrid.ReadOnly = true;
            this.teamDataGrid.RowHeadersVisible = false;
            this.teamDataGrid.RowTemplate.Height = 23;
            this.teamDataGrid.Size = new System.Drawing.Size(284, 176);
            this.teamDataGrid.TabIndex = 4;
            this.teamDataGrid.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.teamDataGrid_MouseDoubleClick);
            // 
            // Item
            // 
            this.Item.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Item.HeaderText = "Item";
            this.Item.Name = "Item";
            this.Item.ReadOnly = true;
            // 
            // textBox1
            // 
            this.textBox1.BackAlpha = 10;
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(0, 182);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(283, 37);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = "중복된 이름이 존재합니다.\r\n위 리스트에서 선택하고자 하는 행을 고르십시오.";
            // 
            // FormTeamList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 257);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.teamDataGrid);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.Name = "FormTeamList";
            this.ShowInTaskbar = false;
            this.Text = "중복된 아이템 리스트";
            this.Load += new System.EventHandler(this.FormTeamList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.teamDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.DataGridView teamDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item;
        //private System.Windows.Forms.TextBox textBox1;
        private ZBobb.AlphaBlendTextBox textBox1;
    }
}