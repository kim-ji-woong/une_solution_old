namespace LibraryReader
{
    partial class FormSelectLibrary2
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
            this.colAddr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCoord = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBoxAddr = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnGoBack = new System.Windows.Forms.Button();
            this.btnStopProgress = new System.Windows.Forms.Button();
            this.labelOldTypeAddress = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAddr,
            this.colCoord});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(668, 223);
            this.dataGridView1.TabIndex = 1;
            // 
            // colAddr
            // 
            this.colAddr.HeaderText = "주소";
            this.colAddr.Name = "colAddr";
            this.colAddr.ReadOnly = true;
            this.colAddr.Width = 400;
            // 
            // colCoord
            // 
            this.colCoord.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCoord.HeaderText = "좌표";
            this.colCoord.Name = "colCoord";
            this.colCoord.ReadOnly = true;
            // 
            // textBoxAddr
            // 
            this.textBoxAddr.Location = new System.Drawing.Point(12, 258);
            this.textBoxAddr.Name = "textBoxAddr";
            this.textBoxAddr.Size = new System.Drawing.Size(322, 21);
            this.textBoxAddr.TabIndex = 2;
            this.textBoxAddr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxAddr_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(353, 257);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "재검색";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(581, 256);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnGoBack
            // 
            this.btnGoBack.Enabled = false;
            this.btnGoBack.Location = new System.Drawing.Point(12, 229);
            this.btnGoBack.Name = "btnGoBack";
            this.btnGoBack.Size = new System.Drawing.Size(111, 23);
            this.btnGoBack.TabIndex = 5;
            this.btnGoBack.Text = "이전 데이터 보기";
            this.btnGoBack.UseVisualStyleBackColor = true;
            this.btnGoBack.Click += new System.EventHandler(this.btnGoBack_Click);
            // 
            // btnStopProgress
            // 
            this.btnStopProgress.Location = new System.Drawing.Point(129, 229);
            this.btnStopProgress.Name = "btnStopProgress";
            this.btnStopProgress.Size = new System.Drawing.Size(121, 23);
            this.btnStopProgress.TabIndex = 6;
            this.btnStopProgress.Text = "여기까지 저장하기";
            this.btnStopProgress.UseVisualStyleBackColor = true;
            this.btnStopProgress.Click += new System.EventHandler(this.btnStopProgress_Click);
            // 
            // labelOldTypeAddress
            // 
            this.labelOldTypeAddress.AutoSize = true;
            this.labelOldTypeAddress.Location = new System.Drawing.Point(12, 286);
            this.labelOldTypeAddress.Name = "labelOldTypeAddress";
            this.labelOldTypeAddress.Size = new System.Drawing.Size(57, 12);
            this.labelOldTypeAddress.TabIndex = 7;
            this.labelOldTypeAddress.Text = "지번 주소";
            // 
            // FormSelectLibrary2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 307);
            this.Controls.Add(this.labelOldTypeAddress);
            this.Controls.Add(this.btnStopProgress);
            this.Controls.Add(this.btnGoBack);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.textBoxAddr);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FormSelectLibrary2";
            this.Text = "FormSelectLibrary2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSelectLibrary2_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAddr;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCoord;
        private System.Windows.Forms.TextBox textBoxAddr;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnGoBack;
        private System.Windows.Forms.Button btnStopProgress;
        private System.Windows.Forms.Label labelOldTypeAddress;
    }
}