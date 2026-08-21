namespace SDMS.PopupDialog
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
            this.btnChangeView = new System.Windows.Forms.Button();
            this.mTreeViewCCTV = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.paneSearch = new System.Windows.Forms.Panel();
            this.lblPSMMaterial = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCCTVList)).BeginInit();
            this.panel1.SuspendLayout();
            this.paneSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewCCTVList
            // 
            this.dataGridViewCCTVList.AllowUserToAddRows = false;
            this.dataGridViewCCTVList.AllowUserToDeleteRows = false;
            this.dataGridViewCCTVList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewCCTVList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCCTVList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColID,
            this.ColCCTVName,
            this.ColPosition});
            this.dataGridViewCCTVList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewCCTVList.Location = new System.Drawing.Point(3, 4);
            this.dataGridViewCCTVList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridViewCCTVList.MultiSelect = false;
            this.dataGridViewCCTVList.Name = "dataGridViewCCTVList";
            this.dataGridViewCCTVList.ReadOnly = true;
            this.dataGridViewCCTVList.RowHeadersVisible = false;
            this.dataGridViewCCTVList.RowTemplate.Height = 23;
            this.dataGridViewCCTVList.Size = new System.Drawing.Size(588, 516);
            this.dataGridViewCCTVList.StandardTab = true;
            this.dataGridViewCCTVList.TabIndex = 0;
            this.dataGridViewCCTVList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewCCTVList_CellMouseClick);
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
            this.textBoxDictionary.Location = new System.Drawing.Point(14, 33);
            this.textBoxDictionary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxDictionary.Name = "textBoxDictionary";
            this.textBoxDictionary.Size = new System.Drawing.Size(210, 23);
            this.textBoxDictionary.TabIndex = 1;
            this.textBoxDictionary.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(230, 31);
            this.btnFind.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(41, 25);
            this.btnFind.TabIndex = 2;
            this.btnFind.Text = "찾기";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnChangeView
            // 
            this.btnChangeView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeView.Location = new System.Drawing.Point(493, 31);
            this.btnChangeView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnChangeView.Name = "btnChangeView";
            this.btnChangeView.Size = new System.Drawing.Size(87, 25);
            this.btnChangeView.TabIndex = 4;
            this.btnChangeView.Text = "그룹별 보기";
            this.btnChangeView.UseVisualStyleBackColor = true;
            this.btnChangeView.Click += new System.EventHandler(this.btnChangeView_Click);
            // 
            // mTreeViewCCTV
            // 
            this.mTreeViewCCTV.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mTreeViewCCTV.Location = new System.Drawing.Point(134, 174);
            this.mTreeViewCCTV.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.mTreeViewCCTV.Name = "mTreeViewCCTV";
            this.mTreeViewCCTV.Size = new System.Drawing.Size(257, 190);
            this.mTreeViewCCTV.TabIndex = 5;
            this.mTreeViewCCTV.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.mTreeViewCCTV_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.mTreeViewCCTV);
            this.panel1.Controls.Add(this.dataGridViewCCTVList);
            this.panel1.Location = new System.Drawing.Point(13, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(594, 524);
            this.panel1.TabIndex = 6;
            // 
            // paneSearch
            // 
            this.paneSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.paneSearch.BackColor = System.Drawing.Color.White;
            this.paneSearch.Controls.Add(this.lblPSMMaterial);
            this.paneSearch.Controls.Add(this.textBoxDictionary);
            this.paneSearch.Controls.Add(this.btnChangeView);
            this.paneSearch.Controls.Add(this.btnFind);
            this.paneSearch.Location = new System.Drawing.Point(13, 6);
            this.paneSearch.Name = "paneSearch";
            this.paneSearch.Size = new System.Drawing.Size(594, 70);
            this.paneSearch.TabIndex = 1;
            this.paneSearch.MouseDown += new System.Windows.Forms.MouseEventHandler(this.paneSearch_MouseDown);
            this.paneSearch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.paneSearch_MouseUp);
            // 
            // lblPSMMaterial
            // 
            this.lblPSMMaterial.AutoSize = true;
            this.lblPSMMaterial.Location = new System.Drawing.Point(17, 10);
            this.lblPSMMaterial.Name = "lblPSMMaterial";
            this.lblPSMMaterial.Size = new System.Drawing.Size(65, 15);
            this.lblPSMMaterial.TabIndex = 5;
            this.lblPSMMaterial.Text = "CCTV 검색";
            // 
            // FormCCTVList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 618);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.paneSearch);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormCCTVList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormCCTVList";
            this.Activated += new System.EventHandler(this.FormCCTVList_Activated);
            this.Deactivate += new System.EventHandler(this.FormCCTVList_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCCTVList_FormClosing);
            this.Load += new System.EventHandler(this.FormCCTVList_Load);
            this.Enter += new System.EventHandler(this.FormCCTVList_Enter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.Leave += new System.EventHandler(this.FormCCTVList_Leave);
            this.MouseEnter += new System.EventHandler(this.FormCCTVList_MouseEnter);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCCTVList)).EndInit();
            this.panel1.ResumeLayout(false);
            this.paneSearch.ResumeLayout(false);
            this.paneSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewCCTVList;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCCTVName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColPosition;
        private System.Windows.Forms.TextBox textBoxDictionary;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnChangeView;
        private System.Windows.Forms.TreeView mTreeViewCCTV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel paneSearch;
        private System.Windows.Forms.Label lblPSMMaterial;
    }
}