namespace FireManagement
{
    partial class Form1
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
            this.dataGridGroup = new System.Windows.Forms.DataGridView();
            this.colEquipType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridGroup)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridGroup
            // 
            this.dataGridGroup.AllowUserToAddRows = false;
            this.dataGridGroup.AllowUserToDeleteRows = false;
            this.dataGridGroup.AllowUserToResizeColumns = false;
            this.dataGridGroup.AllowUserToResizeRows = false;
            this.dataGridGroup.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridGroup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridGroup.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEquipType,
            this.colEquipID});
            this.dataGridGroup.Location = new System.Drawing.Point(12, 7);
            this.dataGridGroup.MultiSelect = false;
            this.dataGridGroup.Name = "dataGridGroup";
            this.dataGridGroup.ReadOnly = true;
            this.dataGridGroup.RowHeadersVisible = false;
            this.dataGridGroup.RowTemplate.Height = 23;
            this.dataGridGroup.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridGroup.Size = new System.Drawing.Size(149, 184);
            this.dataGridGroup.TabIndex = 1;
            this.dataGridGroup.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridGroup_CellClick);
            // 
            // colEquipType
            // 
            this.colEquipType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colEquipType.FillWeight = 40F;
            this.colEquipType.HeaderText = "유형";
            this.colEquipType.Name = "colEquipType";
            this.colEquipType.ReadOnly = true;
            // 
            // colEquipID
            // 
            this.colEquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colEquipID.FillWeight = 60F;
            this.colEquipID.HeaderText = "관리번호";
            this.colEquipID.Name = "colEquipID";
            this.colEquipID.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.BackgroundImage = global::FireManagement.Properties.Resources.popup_BG;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(173, 222);
            this.Controls.Add(this.dataGridGroup);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.SystemColors.HotTrack;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridGroup)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipID;
    }
}