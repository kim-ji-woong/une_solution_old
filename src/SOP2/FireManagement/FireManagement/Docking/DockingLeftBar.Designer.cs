namespace FireManagement
{
    partial class DockingLeftBar
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
            this.labelFE = new System.Windows.Forms.Label();
            this.dataGridFE = new System.Windows.Forms.DataGridView();
            this.colRFIDTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelHD = new System.Windows.Forms.Label();
            this.dataGridHD = new System.Windows.Forms.DataGridView();
            this.colHDRFID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHDEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelFA = new System.Windows.Forms.Label();
            this.dataGridFA = new System.Windows.Forms.DataGridView();
            this.colFARFID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFAEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridHD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFA)).BeginInit();
            this.SuspendLayout();
            // 
            // labelFE
            // 
            this.labelFE.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelFE.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFE.Location = new System.Drawing.Point(9, 9);
            this.labelFE.Name = "labelFE";
            this.labelFE.Size = new System.Drawing.Size(221, 28);
            this.labelFE.TabIndex = 0;
            this.labelFE.Text = "  소화기";
            this.labelFE.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataGridFE
            // 
            this.dataGridFE.AllowUserToAddRows = false;
            this.dataGridFE.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFE.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRFIDTag,
            this.colEquipID});
            this.dataGridFE.Location = new System.Drawing.Point(9, 40);
            this.dataGridFE.Name = "dataGridFE";
            this.dataGridFE.RowHeadersVisible = false;
            this.dataGridFE.RowTemplate.Height = 23;
            this.dataGridFE.Size = new System.Drawing.Size(221, 115);
            this.dataGridFE.TabIndex = 1;
            this.dataGridFE.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellClick);
            this.dataGridFE.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellValueChanged);
            // 
            // colRFIDTag
            // 
            this.colRFIDTag.HeaderText = "RFID Tag";
            this.colRFIDTag.Name = "colRFIDTag";
            // 
            // colEquipID
            // 
            this.colEquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colEquipID.HeaderText = "설비관리번호";
            this.colEquipID.Name = "colEquipID";
            // 
            // labelHD
            // 
            this.labelHD.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelHD.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelHD.Location = new System.Drawing.Point(9, 162);
            this.labelHD.Name = "labelHD";
            this.labelHD.Size = new System.Drawing.Size(221, 28);
            this.labelHD.TabIndex = 0;
            this.labelHD.Text = "  소화전";
            this.labelHD.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataGridHD
            // 
            this.dataGridHD.AllowUserToAddRows = false;
            this.dataGridHD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridHD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHDRFID,
            this.colHDEquipID});
            this.dataGridHD.Location = new System.Drawing.Point(9, 193);
            this.dataGridHD.Name = "dataGridHD";
            this.dataGridHD.RowHeadersVisible = false;
            this.dataGridHD.RowTemplate.Height = 23;
            this.dataGridHD.Size = new System.Drawing.Size(221, 115);
            this.dataGridHD.TabIndex = 1;
            this.dataGridHD.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellClick);
            this.dataGridHD.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellValueChanged);
            // 
            // colHDRFID
            // 
            this.colHDRFID.HeaderText = "RFID Tag";
            this.colHDRFID.Name = "colHDRFID";
            // 
            // colHDEquipID
            // 
            this.colHDEquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHDEquipID.HeaderText = "설비관리번호";
            this.colHDEquipID.Name = "colHDEquipID";
            // 
            // labelFA
            // 
            this.labelFA.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelFA.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFA.Location = new System.Drawing.Point(9, 317);
            this.labelFA.Name = "labelFA";
            this.labelFA.Size = new System.Drawing.Size(221, 28);
            this.labelFA.TabIndex = 0;
            this.labelFA.Text = "  발신기";
            this.labelFA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataGridFA
            // 
            this.dataGridFA.AllowUserToAddRows = false;
            this.dataGridFA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFA.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFARFID,
            this.colFAEquipID});
            this.dataGridFA.Location = new System.Drawing.Point(9, 348);
            this.dataGridFA.Name = "dataGridFA";
            this.dataGridFA.RowHeadersVisible = false;
            this.dataGridFA.RowTemplate.Height = 23;
            this.dataGridFA.Size = new System.Drawing.Size(221, 115);
            this.dataGridFA.TabIndex = 1;
            this.dataGridFA.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellClick);
            this.dataGridFA.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellValueChanged);
            // 
            // colFARFID
            // 
            this.colFARFID.HeaderText = "RFID Tag";
            this.colFARFID.Name = "colFARFID";
            // 
            // colFAEquipID
            // 
            this.colFAEquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFAEquipID.HeaderText = "설비관리번호";
            this.colFAEquipID.Name = "colFAEquipID";
            // 
            // DockingLeftBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 486);
            this.Controls.Add(this.dataGridFA);
            this.Controls.Add(this.labelFA);
            this.Controls.Add(this.dataGridHD);
            this.Controls.Add(this.labelHD);
            this.Controls.Add(this.dataGridFE);
            this.Controls.Add(this.labelFE);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingLeftBar";
            this.Text = "DockingLeftBar";
            this.Resize += new System.EventHandler(this.DockingLeftBar_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridHD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFA)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelFE;
        private System.Windows.Forms.DataGridView dataGridFE;
        private System.Windows.Forms.Label labelHD;
        private System.Windows.Forms.DataGridView dataGridHD;
        private System.Windows.Forms.Label labelFA;
        private System.Windows.Forms.DataGridView dataGridFA;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRFIDTag;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDRFID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDEquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFARFID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFAEquipID;
    }
}