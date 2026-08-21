namespace SMSSender
{
    partial class FormReciver
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
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.rbBtnExternal = new System.Windows.Forms.RadioButton();
            this.rbBtnRegular = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.listReciver = new System.Windows.Forms.ListBox();
            this.btnAddManual = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.labelMemberPath = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.BackColor = System.Drawing.Color.White;
            this.treeViewTeam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewTeam.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewTeam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            this.treeViewTeam.HideSelection = false;
            this.treeViewTeam.Location = new System.Drawing.Point(12, 49);
            this.treeViewTeam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeViewTeam.MinimumSize = new System.Drawing.Size(378, 300);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(450, 380);
            this.treeViewTeam.TabIndex = 5;
            this.treeViewTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterSelect);
            this.treeViewTeam.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewTeam_NodeMouseDoubleClick);
            // 
            // rbBtnExternal
            // 
            this.rbBtnExternal.AutoSize = true;
            this.rbBtnExternal.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnExternal.Location = new System.Drawing.Point(106, 15);
            this.rbBtnExternal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbBtnExternal.Name = "rbBtnExternal";
            this.rbBtnExternal.Size = new System.Drawing.Size(78, 21);
            this.rbBtnExternal.TabIndex = 9;
            this.rbBtnExternal.Text = "외부조직";
            this.rbBtnExternal.UseVisualStyleBackColor = true;
            this.rbBtnExternal.CheckedChanged += new System.EventHandler(this.rbBtnExternal_CheckedChanged);
            // 
            // rbBtnRegular
            // 
            this.rbBtnRegular.AutoSize = true;
            this.rbBtnRegular.Checked = true;
            this.rbBtnRegular.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnRegular.Location = new System.Drawing.Point(22, 15);
            this.rbBtnRegular.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbBtnRegular.Name = "rbBtnRegular";
            this.rbBtnRegular.Size = new System.Drawing.Size(78, 21);
            this.rbBtnRegular.TabIndex = 6;
            this.rbBtnRegular.TabStop = true;
            this.rbBtnRegular.Text = "정규조직";
            this.rbBtnRegular.UseVisualStyleBackColor = true;
            this.rbBtnRegular.CheckedChanged += new System.EventHandler(this.rbBtnRegular_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(705, 442);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 29);
            this.btnCancel.TabIndex = 28;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(477, 131);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(94, 29);
            this.btnAdd.TabIndex = 27;
            this.btnAdd.Text = "추가하기";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(241, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(130, 23);
            this.txtSearch.TabIndex = 29;
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(383, 11);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 29);
            this.btnSearch.TabIndex = 30;
            this.btnSearch.Text = "찾기";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // listReciver
            // 
            this.listReciver.FormattingEnabled = true;
            this.listReciver.ItemHeight = 15;
            this.listReciver.Location = new System.Drawing.Point(586, 49);
            this.listReciver.Name = "listReciver";
            this.listReciver.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listReciver.Size = new System.Drawing.Size(231, 379);
            this.listReciver.TabIndex = 31;
            // 
            // btnAddManual
            // 
            this.btnAddManual.Location = new System.Drawing.Point(477, 287);
            this.btnAddManual.Name = "btnAddManual";
            this.btnAddManual.Size = new System.Drawing.Size(94, 29);
            this.btnAddManual.TabIndex = 35;
            this.btnAddManual.Text = "직접추가";
            this.btnAddManual.UseVisualStyleBackColor = true;
            this.btnAddManual.Click += new System.EventHandler(this.btnAddManual_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(477, 184);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(94, 29);
            this.btnSelectAll.TabIndex = 34;
            this.btnSelectAll.Text = "전체선택";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(477, 236);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(94, 29);
            this.btnRemove.TabIndex = 33;
            this.btnRemove.Text = "선택삭제";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(588, 443);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(95, 27);
            this.btnOK.TabIndex = 36;
            this.btnOK.Text = "적용";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // labelMemberPath
            // 
            this.labelMemberPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMemberPath.AutoSize = true;
            this.labelMemberPath.Location = new System.Drawing.Point(12, 438);
            this.labelMemberPath.Name = "labelMemberPath";
            this.labelMemberPath.Size = new System.Drawing.Size(115, 15);
            this.labelMemberPath.TabIndex = 37;
            this.labelMemberPath.Text = "선택 직원 부서 정보";
            // 
            // FormReciver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 485);
            this.Controls.Add(this.labelMemberPath);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnAddManual);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.listReciver);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.rbBtnExternal);
            this.Controls.Add(this.rbBtnRegular);
            this.Controls.Add(this.treeViewTeam);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReciver";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "수신자 설정";
            this.Load += new System.EventHandler(this.FormTeamTree_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.RadioButton rbBtnExternal;
        private System.Windows.Forms.RadioButton rbBtnRegular;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ListBox listReciver;
        private System.Windows.Forms.Button btnAddManual;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label labelMemberPath;
    }
}