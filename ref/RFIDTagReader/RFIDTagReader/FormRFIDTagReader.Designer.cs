namespace RFIDTagReader
{
    partial class FormRFIDTagReader
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEquipType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEquipName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRFIDTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.cboEquipType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.btnInput = new System.Windows.Forms.Button();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEquipID,
            this.colEquipType,
            this.colEquipName,
            this.colRFIDTag});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(862, 356);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            // 
            // colEquipID
            // 
            this.colEquipID.HeaderText = "설비번호";
            this.colEquipID.Name = "colEquipID";
            // 
            // colEquipType
            // 
            this.colEquipType.HeaderText = "설비타입";
            this.colEquipType.Name = "colEquipType";
            // 
            // colEquipName
            // 
            this.colEquipName.HeaderText = "설비이름";
            this.colEquipName.Name = "colEquipName";
            this.colEquipName.Width = 300;
            // 
            // colRFIDTag
            // 
            this.colRFIDTag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRFIDTag.HeaderText = "RFID Tag";
            this.colRFIDTag.Name = "colRFIDTag";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 391);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "설비 Type";
            // 
            // cboEquipType
            // 
            this.cboEquipType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEquipType.FormattingEnabled = true;
            this.cboEquipType.Items.AddRange(new object[] {
            "소화기",
            "소화전",
            "발신기",
            "기타"});
            this.cboEquipType.Location = new System.Drawing.Point(108, 387);
            this.cboEquipType.Name = "cboEquipType";
            this.cboEquipType.Size = new System.Drawing.Size(121, 20);
            this.cboEquipType.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 423);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Input File";
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(108, 420);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(390, 21);
            this.textBoxFilePath.TabIndex = 3;
            // 
            // btnInput
            // 
            this.btnInput.Location = new System.Drawing.Point(504, 418);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(30, 23);
            this.btnInput.TabIndex = 4;
            this.btnInput.Text = "...";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(540, 418);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 5;
            this.btnOpenFile.Text = "파일 열기";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(540, 380);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 6;
            this.btnSelectAll.Text = "전체 선택";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // FormRFIDTagReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 499);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.btnInput);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.cboEquipType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FormRFIDTagReader";
            this.Text = "FormRFIDTagReader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRFIDTagReader_FormClosing);
            this.Load += new System.EventHandler(this.FormRFIDTagReader_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRFIDTag;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboEquipType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnSelectAll;
    }
}