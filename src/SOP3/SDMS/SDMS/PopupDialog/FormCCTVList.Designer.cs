namespace SDMS
{
    partial class FormCCTVList
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
            this.dataGridViewCCTVList = new System.Windows.Forms.DataGridView();
            this.ColID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCCTVName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBoxDictionary = new System.Windows.Forms.TextBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.checkBoxShowOutdoor = new System.Windows.Forms.CheckBox();
            this.checkBoxShowIndoor = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCCTVList)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewCCTVList
            // 
            this.dataGridViewCCTVList.AllowUserToAddRows = false;
            this.dataGridViewCCTVList.AllowUserToDeleteRows = false;
            this.dataGridViewCCTVList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCCTVList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColID,
            this.ColCCTVName,
            this.ColPosition});
            this.dataGridViewCCTVList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridViewCCTVList.Location = new System.Drawing.Point(0, 41);
            this.dataGridViewCCTVList.MultiSelect = false;
            this.dataGridViewCCTVList.Name = "dataGridViewCCTVList";
            this.dataGridViewCCTVList.ReadOnly = true;
            this.dataGridViewCCTVList.RowHeadersVisible = false;
            this.dataGridViewCCTVList.RowTemplate.Height = 23;
            this.dataGridViewCCTVList.Size = new System.Drawing.Size(636, 453);
            this.dataGridViewCCTVList.TabIndex = 0;
            this.dataGridViewCCTVList.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewCCTVList_CellMouseUp);
            // 
            // ColID
            // 
            this.ColID.HeaderText = "ID";
            this.ColID.Name = "ColID";
            this.ColID.ReadOnly = true;
            this.ColID.Width = 80;
            // 
            // ColCCTVName
            // 
            this.ColCCTVName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColCCTVName.HeaderText = "CCTV 이름";
            this.ColCCTVName.Name = "ColCCTVName";
            this.ColCCTVName.ReadOnly = true;
            // 
            // ColPosition
            // 
            this.ColPosition.HeaderText = "위치";
            this.ColPosition.Name = "ColPosition";
            this.ColPosition.ReadOnly = true;
            this.ColPosition.Width = 200;
            // 
            // textBoxDictionary
            // 
            this.textBoxDictionary.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxDictionary.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBoxDictionary.Location = new System.Drawing.Point(12, 12);
            this.textBoxDictionary.Name = "textBoxDictionary";
            this.textBoxDictionary.Size = new System.Drawing.Size(210, 21);
            this.textBoxDictionary.TabIndex = 1;
            this.textBoxDictionary.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(228, 12);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(41, 23);
            this.btnFind.TabIndex = 2;
            this.btnFind.Text = "찾기";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // checkBoxShowOutdoor
            // 
            this.checkBoxShowOutdoor.AutoSize = true;
            this.checkBoxShowOutdoor.Location = new System.Drawing.Point(384, 16);
            this.checkBoxShowOutdoor.Name = "checkBoxShowOutdoor";
            this.checkBoxShowOutdoor.Size = new System.Drawing.Size(116, 16);
            this.checkBoxShowOutdoor.TabIndex = 3;
            this.checkBoxShowOutdoor.Text = "외부 카메라 보기";
            this.checkBoxShowOutdoor.UseVisualStyleBackColor = true;
            this.checkBoxShowOutdoor.CheckedChanged += new System.EventHandler(this.checkBoxShowOutdoor_CheckedChanged);
            // 
            // checkBoxShowIndoor
            // 
            this.checkBoxShowIndoor.AutoSize = true;
            this.checkBoxShowIndoor.Location = new System.Drawing.Point(508, 14);
            this.checkBoxShowIndoor.Name = "checkBoxShowIndoor";
            this.checkBoxShowIndoor.Size = new System.Drawing.Size(116, 16);
            this.checkBoxShowIndoor.TabIndex = 3;
            this.checkBoxShowIndoor.Text = "실내 카메라 보기";
            this.checkBoxShowIndoor.UseVisualStyleBackColor = true;
            this.checkBoxShowIndoor.CheckedChanged += new System.EventHandler(this.checkBoxShowIndoor_CheckedChanged);
            // 
            // FormCCTVList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 494);
            this.Controls.Add(this.checkBoxShowIndoor);
            this.Controls.Add(this.checkBoxShowOutdoor);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.textBoxDictionary);
            this.Controls.Add(this.dataGridViewCCTVList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormCCTVList";
            this.ShowInTaskbar = false;
            this.Text = "FormCCTVList";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCCTVList_FormClosing);
            this.Load += new System.EventHandler(this.FormCCTVList_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCCTVList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewCCTVList;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCCTVName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColPosition;
        private System.Windows.Forms.TextBox textBoxDictionary;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.CheckBox checkBoxShowOutdoor;
        private System.Windows.Forms.CheckBox checkBoxShowIndoor;
    }
}