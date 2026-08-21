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
            this.mTreeViewCCTV = new System.Windows.Forms.TreeView();
            this.textBoxDictionary = new System.Windows.Forms.TextBox();
            this.dataGridViewCCTVList = new System.Windows.Forms.DataGridView();
            this.ColID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCCTVName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnChangeView = new UnE.GUI.ImageButton();
            this.btnFind = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCCTVList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChangeView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFind)).BeginInit();
            this.SuspendLayout();
            // 
            // mTreeViewCCTV
            // 
            this.mTreeViewCCTV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mTreeViewCCTV.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mTreeViewCCTV.Location = new System.Drawing.Point(6, 43);
            this.mTreeViewCCTV.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.mTreeViewCCTV.Name = "mTreeViewCCTV";
            this.mTreeViewCCTV.Size = new System.Drawing.Size(610, 883);
            this.mTreeViewCCTV.TabIndex = 5;
            this.mTreeViewCCTV.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.mTreeViewCCTV_AfterSelect);
            // 
            // textBoxDictionary
            // 
            this.textBoxDictionary.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxDictionary.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBoxDictionary.Font = new System.Drawing.Font(Program.prgFont, 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDictionary.Location = new System.Drawing.Point(6, 9);
            this.textBoxDictionary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxDictionary.Name = "textBoxDictionary";
            this.textBoxDictionary.Size = new System.Drawing.Size(497, 29);
            this.textBoxDictionary.TabIndex = 7;
            this.textBoxDictionary.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxDictionary_KeyDown);
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
            this.dataGridViewCCTVList.Location = new System.Drawing.Point(6, 43);
            this.dataGridViewCCTVList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridViewCCTVList.MultiSelect = false;
            this.dataGridViewCCTVList.Name = "dataGridViewCCTVList";
            this.dataGridViewCCTVList.ReadOnly = true;
            this.dataGridViewCCTVList.RowHeadersVisible = false;
            this.dataGridViewCCTVList.RowTemplate.Height = 23;
            this.dataGridViewCCTVList.Size = new System.Drawing.Size(610, 883);
            this.dataGridViewCCTVList.StandardTab = true;
            this.dataGridViewCCTVList.TabIndex = 11;
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
            // btnChangeView
            // 
            this.btnChangeView.BackColor = System.Drawing.Color.Transparent;
            this.btnChangeView.ButtonText = "";
            this.btnChangeView.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChangeView.ImageClicked = global::SDMS.Properties.Resources.CCTVList_ChangeViewGroup_Click;
            this.btnChangeView.ImageDisabled = null;
            this.btnChangeView.ImageMouseOver = global::SDMS.Properties.Resources.CCTVList_ChangeViewGroup_Click;
            this.btnChangeView.ImageNormal = global::SDMS.Properties.Resources.CCTVList_ChangeViewGroup_Default;
            this.btnChangeView.Location = new System.Drawing.Point(537, 8);
            this.btnChangeView.Name = "btnChangeView";
            this.btnChangeView.Owner = null;
            this.btnChangeView.Size = new System.Drawing.Size(75, 32);
            this.btnChangeView.TabIndex = 13;
            this.btnChangeView.TabStop = false;
            this.btnChangeView.TextColor = System.Drawing.Color.Black;
            this.btnChangeView.TextFont = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChangeView.ToolTipText = "";
            this.btnChangeView.Click += new System.EventHandler(this.btnChangeView_Click);
            // 
            // btnFind
            // 
            this.btnFind.BackColor = System.Drawing.Color.Transparent;
            this.btnFind.ButtonText = "";
            this.btnFind.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFind.ImageClicked = global::SDMS.Properties.Resources.BtnSearch_Click;
            this.btnFind.ImageDisabled = null;
            this.btnFind.ImageMouseOver = global::SDMS.Properties.Resources.BtnSearch_Click;
            this.btnFind.ImageNormal = global::SDMS.Properties.Resources.BtnSearch_Default;
            this.btnFind.Location = new System.Drawing.Point(502, 9);
            this.btnFind.Name = "btnFind";
            this.btnFind.Owner = null;
            this.btnFind.Size = new System.Drawing.Size(29, 29);
            this.btnFind.TabIndex = 12;
            this.btnFind.TabStop = false;
            this.btnFind.TextColor = System.Drawing.Color.Black;
            this.btnFind.TextFont = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFind.ToolTipText = "";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // FormCCTVList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(626, 933);
            this.Controls.Add(this.btnChangeView);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.dataGridViewCCTVList);
            this.Controls.Add(this.mTreeViewCCTV);
            this.Controls.Add(this.textBoxDictionary);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            ((System.ComponentModel.ISupportInitialize)(this.btnChangeView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFind)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView mTreeViewCCTV;
        private System.Windows.Forms.TextBox textBoxDictionary;
        private System.Windows.Forms.DataGridView dataGridViewCCTVList;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCCTVName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColPosition;
        private UnE.GUI.ImageButton btnFind;
        private UnE.GUI.ImageButton btnChangeView;
    }
}