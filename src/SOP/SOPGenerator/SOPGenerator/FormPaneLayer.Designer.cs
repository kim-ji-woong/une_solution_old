namespace SOPGen
{
    partial class FormPaneLayer
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSOPDel = new System.Windows.Forms.Button();
            this.btnSOPEdit = new System.Windows.Forms.Button();
            this.btnSOPAdd = new System.Windows.Forms.Button();
            this.treeViewSOP = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.textSearch = new System.Windows.Forms.TextBox();
            this.rdoBtnPerson = new System.Windows.Forms.RadioButton();
            this.rdoBtnTeam = new System.Windows.Forms.RadioButton();
            this.dataGridViewSearch = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.btnSOPDel);
            this.groupBox1.Controls.Add(this.btnSOPEdit);
            this.groupBox1.Controls.Add(this.btnSOPAdd);
            this.groupBox1.Controls.Add(this.treeViewSOP);
            this.groupBox1.Location = new System.Drawing.Point(10, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 350);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SOP";
            // 
            // btnSOPDel
            // 
            this.btnSOPDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSOPDel.Location = new System.Drawing.Point(164, 321);
            this.btnSOPDel.Name = "btnSOPDel";
            this.btnSOPDel.Size = new System.Drawing.Size(65, 23);
            this.btnSOPDel.TabIndex = 21;
            this.btnSOPDel.Text = "삭제";
            this.btnSOPDel.UseVisualStyleBackColor = true;
            this.btnSOPDel.Click += new System.EventHandler(this.btnSOPDel_Click);
            // 
            // btnSOPEdit
            // 
            this.btnSOPEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSOPEdit.Location = new System.Drawing.Point(88, 321);
            this.btnSOPEdit.Name = "btnSOPEdit";
            this.btnSOPEdit.Size = new System.Drawing.Size(65, 23);
            this.btnSOPEdit.TabIndex = 20;
            this.btnSOPEdit.Text = "수정";
            this.btnSOPEdit.UseVisualStyleBackColor = true;
            this.btnSOPEdit.Click += new System.EventHandler(this.btnSOPEdit_Click);
            // 
            // btnSOPAdd
            // 
            this.btnSOPAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSOPAdd.Location = new System.Drawing.Point(12, 321);
            this.btnSOPAdd.Name = "btnSOPAdd";
            this.btnSOPAdd.Size = new System.Drawing.Size(65, 23);
            this.btnSOPAdd.TabIndex = 19;
            this.btnSOPAdd.Text = "추가";
            this.btnSOPAdd.UseVisualStyleBackColor = true;
            this.btnSOPAdd.Click += new System.EventHandler(this.btnSOPAdd_Click);
            // 
            // treeViewSOP
            // 
            this.treeViewSOP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewSOP.LabelEdit = true;
            this.treeViewSOP.Location = new System.Drawing.Point(12, 20);
            this.treeViewSOP.Name = "treeViewSOP";
            this.treeViewSOP.Size = new System.Drawing.Size(217, 295);
            this.treeViewSOP.TabIndex = 18;
            this.treeViewSOP.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewSOP_AfterLabelEdit);
            this.treeViewSOP.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSOP_AfterSelect);
            this.treeViewSOP.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewSOP_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.btnSearch);
            this.groupBox2.Controls.Add(this.textSearch);
            this.groupBox2.Controls.Add(this.rdoBtnPerson);
            this.groupBox2.Controls.Add(this.rdoBtnTeam);
            this.groupBox2.Controls.Add(this.dataGridViewSearch);
            this.groupBox2.Location = new System.Drawing.Point(10, 368);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 260);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "부서/담당자 조회";
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSearch.Location = new System.Drawing.Point(164, 231);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(65, 23);
            this.btnSearch.TabIndex = 19;
            this.btnSearch.Text = "조회";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // textSearch
            // 
            this.textSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textSearch.Location = new System.Drawing.Point(12, 232);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(146, 21);
            this.textSearch.TabIndex = 18;
            this.textSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textSearch_KeyDown);
            // 
            // rdoBtnPerson
            // 
            this.rdoBtnPerson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoBtnPerson.AutoSize = true;
            this.rdoBtnPerson.Location = new System.Drawing.Point(122, 210);
            this.rdoBtnPerson.Name = "rdoBtnPerson";
            this.rdoBtnPerson.Size = new System.Drawing.Size(59, 16);
            this.rdoBtnPerson.TabIndex = 17;
            this.rdoBtnPerson.Text = "담당자";
            this.rdoBtnPerson.UseVisualStyleBackColor = true;
            // 
            // rdoBtnTeam
            // 
            this.rdoBtnTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoBtnTeam.AutoSize = true;
            this.rdoBtnTeam.Checked = true;
            this.rdoBtnTeam.Location = new System.Drawing.Point(12, 210);
            this.rdoBtnTeam.Name = "rdoBtnTeam";
            this.rdoBtnTeam.Size = new System.Drawing.Size(47, 16);
            this.rdoBtnTeam.TabIndex = 17;
            this.rdoBtnTeam.TabStop = true;
            this.rdoBtnTeam.Text = "부서";
            this.rdoBtnTeam.UseVisualStyleBackColor = true;
            // 
            // dataGridViewSearch
            // 
            this.dataGridViewSearch.AllowUserToAddRows = false;
            this.dataGridViewSearch.AllowUserToDeleteRows = false;
            this.dataGridViewSearch.AllowUserToResizeColumns = false;
            this.dataGridViewSearch.AllowUserToResizeRows = false;
            this.dataGridViewSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridViewSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSearch.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridViewSearch.Location = new System.Drawing.Point(12, 20);
            this.dataGridViewSearch.MultiSelect = false;
            this.dataGridViewSearch.Name = "dataGridViewSearch";
            this.dataGridViewSearch.ReadOnly = true;
            this.dataGridViewSearch.RowHeadersVisible = false;
            this.dataGridViewSearch.RowTemplate.Height = 23;
            this.dataGridViewSearch.Size = new System.Drawing.Size(217, 184);
            this.dataGridViewSearch.TabIndex = 16;
            this.dataGridViewSearch.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_CellMouseDown);
            this.dataGridViewSearch.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_CellMouseUp);
            this.dataGridViewSearch.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseMove);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "부서";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 107;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "담당자";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 107;
            // 
            // FormPaneLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 640);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormPaneLayer";
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSearch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeViewSOP;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox textSearch;
        private System.Windows.Forms.RadioButton rdoBtnPerson;
        private System.Windows.Forms.RadioButton rdoBtnTeam;
        private System.Windows.Forms.DataGridView dataGridViewSearch;
        private System.Windows.Forms.Button btnSOPDel;
        private System.Windows.Forms.Button btnSOPEdit;
        private System.Windows.Forms.Button btnSOPAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;



    }
}