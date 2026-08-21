namespace SOPManager
{
	partial class PopupNoteExpr
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelNote = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.mBtnInsertMacro = new UnE.GUI.RibbonButton();
            this.mBtnMacro = new UnE.GUI.RibbonButton();
            this.mVariableGrid = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mVariableGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // labelNote
            // 
            this.labelNote.AutoSize = true;
            this.labelNote.BackColor = System.Drawing.Color.Transparent;
            this.labelNote.Font = new System.Drawing.Font("나눔스퀘어", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNote.ForeColor = System.Drawing.Color.White;
            this.labelNote.Location = new System.Drawing.Point(12, 20);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(53, 17);
            this.labelNote.TabIndex = 0;
            this.labelNote.Text = "메시지";
            // 
            // textBox
            // 
            this.textBox.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox.Location = new System.Drawing.Point(14, 56);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(430, 122);
            this.textBox.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(464, 472);
            this.panel2.TabIndex = 23;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnOK);
            this.panel3.Controls.Add(this.mBtnInsertMacro);
            this.panel3.Controls.Add(this.mBtnMacro);
            this.panel3.Controls.Add(this.mVariableGrid);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(458, 466);
            this.panel3.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 88, 44);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 88;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(360, 419);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBack;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(88, 44);
            this.btnCancel.TabIndex = 23;
            this.btnCancel.Text = "취소";
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 13);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancel.ToolTipText = "취소";
            this.btnCancel.UseCustomImageRect = true;
            this.btnCancel.UseTextLocation = true;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 88, 44);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 88;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(276, 419);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBack;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(88, 44);
            this.btnOK.TabIndex = 22;
            this.btnOK.Text = "확인";
            this.btnOK.TextLocation = new System.Drawing.Point(0, 13);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "확인";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = true;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // mBtnInsertMacro
            // 
            this.mBtnInsertMacro.CheckButton = false;
            this.mBtnInsertMacro.CheckedBkgndImage = null;
            this.mBtnInsertMacro.CheckedImage = null;
            this.mBtnInsertMacro.ClickedBackgroundImage = null;
            this.mBtnInsertMacro.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.mBtnInsertMacro.CustomImageRect = new System.Drawing.Rectangle(0, 0, 88, 44);
            this.mBtnInsertMacro.DisabledBkgndImage = null;
            this.mBtnInsertMacro.DisabledImage = null;
            this.mBtnInsertMacro.ID = -1;
            this.mBtnInsertMacro.InitButtonWidth = 88;
            this.mBtnInsertMacro.IsChecked = false;
            this.mBtnInsertMacro.Location = new System.Drawing.Point(193, 419);
            this.mBtnInsertMacro.MouseOverBkgndImage = null;
            this.mBtnInsertMacro.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.mBtnInsertMacro.Name = "mBtnInsertMacro";
            this.mBtnInsertMacro.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBack;
            this.mBtnInsertMacro.Owner = null;
            this.mBtnInsertMacro.Size = new System.Drawing.Size(88, 44);
            this.mBtnInsertMacro.TabIndex = 21;
            this.mBtnInsertMacro.Text = "삽입";
            this.mBtnInsertMacro.TextLocation = new System.Drawing.Point(0, 13);
            this.mBtnInsertMacro.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.mBtnInsertMacro.ToolTipText = "삽입";
            this.mBtnInsertMacro.UseCustomImageRect = true;
            this.mBtnInsertMacro.UseTextLocation = true;
            this.mBtnInsertMacro.UseVisualStyleBackColor = true;
            this.mBtnInsertMacro.Click += new System.EventHandler(this.mBtnInsertMacro_Click);
            // 
            // mBtnMacro
            // 
            this.mBtnMacro.CheckButton = false;
            this.mBtnMacro.CheckedBkgndImage = null;
            this.mBtnMacro.CheckedImage = null;
            this.mBtnMacro.ClickedBackgroundImage = null;
            this.mBtnMacro.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.mBtnMacro.CustomImageRect = new System.Drawing.Rectangle(0, 0, 124, 44);
            this.mBtnMacro.DisabledBkgndImage = null;
            this.mBtnMacro.DisabledImage = null;
            this.mBtnMacro.ID = -1;
            this.mBtnMacro.InitButtonWidth = 124;
            this.mBtnMacro.IsChecked = false;
            this.mBtnMacro.Location = new System.Drawing.Point(328, 373);
            this.mBtnMacro.MouseOverBkgndImage = null;
            this.mBtnMacro.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.mBtnMacro.Name = "mBtnMacro";
            this.mBtnMacro.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBack;
            this.mBtnMacro.Owner = null;
            this.mBtnMacro.Size = new System.Drawing.Size(124, 44);
            this.mBtnMacro.TabIndex = 20;
            this.mBtnMacro.Text = "매크로(&M) >>";
            this.mBtnMacro.TextLocation = new System.Drawing.Point(0, 13);
            this.mBtnMacro.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.mBtnMacro.ToolTipText = "매크로(&M) >>";
            this.mBtnMacro.UseCustomImageRect = true;
            this.mBtnMacro.UseTextLocation = true;
            this.mBtnMacro.UseVisualStyleBackColor = true;
            this.mBtnMacro.Click += new System.EventHandler(this.mBtnMacro_Click);
            // 
            // mVariableGrid
            // 
            this.mVariableGrid.AllowUserToAddRows = false;
            this.mVariableGrid.AllowUserToDeleteRows = false;
            this.mVariableGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.mVariableGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.mVariableGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mVariableGrid.BackgroundColor = System.Drawing.Color.White;
            this.mVariableGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.mVariableGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.mVariableGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mVariableGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colValue,
            this.colAccount});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.mVariableGrid.DefaultCellStyle = dataGridViewCellStyle4;
            this.mVariableGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.mVariableGrid.EnableHeadersVisualStyles = false;
            this.mVariableGrid.GridColor = System.Drawing.Color.DarkGray;
            this.mVariableGrid.Location = new System.Drawing.Point(11, 179);
            this.mVariableGrid.Margin = new System.Windows.Forms.Padding(0);
            this.mVariableGrid.MultiSelect = false;
            this.mVariableGrid.Name = "mVariableGrid";
            this.mVariableGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.mVariableGrid.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.mVariableGrid.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.mVariableGrid.RowTemplate.Height = 23;
            this.mVariableGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.mVariableGrid.Size = new System.Drawing.Size(430, 190);
            this.mVariableGrid.TabIndex = 19;
            this.mVariableGrid.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.mVariableGrid_CellContentDoubleClick);
            this.mVariableGrid.DoubleClick += new System.EventHandler(this.mVariableGrid_DoubleClick);
            // 
            // colName
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            this.colName.DefaultCellStyle = dataGridViewCellStyle3;
            this.colName.HeaderText = "이름";
            this.colName.MinimumWidth = 100;
            this.colName.Name = "colName";
            this.colName.ToolTipText = "변수의 지정된 이름입니다.";
            // 
            // colValue
            // 
            this.colValue.HeaderText = "값";
            this.colValue.Name = "colValue";
            // 
            // colAccount
            // 
            this.colAccount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAccount.HeaderText = "설명";
            this.colAccount.Name = "colAccount";
            // 
            // PopupNoteExpr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(464, 472);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.labelNote);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupNoteExpr";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "메시지 작성";
            this.Load += new System.EventHandler(this.PopupNoteEx_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupNote_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupNote_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupNote_MouseUp);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mVariableGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        internal System.Windows.Forms.DataGridView mVariableGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccount;
        private UnE.GUI.RibbonButton mBtnMacro;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.RibbonButton mBtnInsertMacro;
    }
}