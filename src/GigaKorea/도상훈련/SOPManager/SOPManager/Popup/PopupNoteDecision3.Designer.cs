namespace SOPManager.Popup
{
    partial class PopupNoteDecision3
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridSystemType = new System.Windows.Forms.DataGridView();
            this.colVariable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxExpression = new System.Windows.Forms.CheckBox();
            this.labelType = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.lblAutoRun = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gridUserType = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnMain = new System.Windows.Forms.Panel();
            this.lblDesc = new System.Windows.Forms.Label();
            this.btnHighRank = new UnE.GUI.RibbonButton();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.btnShowSpecialMessage = new UnE.GUI.RibbonButton();
            this.picAutoRun = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridSystemType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUserType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAutoRun)).BeginInit();
            this.SuspendLayout();
            // 
            // gridSystemType
            // 
            this.gridSystemType.AllowUserToAddRows = false;
            this.gridSystemType.AllowUserToDeleteRows = false;
            this.gridSystemType.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.gridSystemType.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSystemType.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridSystemType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSystemType.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVariable,
            this.colType,
            this.colDescription});
            this.gridSystemType.Location = new System.Drawing.Point(0, 170);
            this.gridSystemType.Name = "gridSystemType";
            this.gridSystemType.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.gridSystemType.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.gridSystemType.RowTemplate.Height = 23;
            this.gridSystemType.Size = new System.Drawing.Size(433, 120);
            this.gridSystemType.TabIndex = 106;
            this.gridSystemType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grid_MouseDown);
            // 
            // colVariable
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colVariable.DefaultCellStyle = dataGridViewCellStyle3;
            this.colVariable.HeaderText = "변수";
            this.colVariable.Name = "colVariable";
            this.colVariable.ReadOnly = true;
            this.colVariable.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colVariable.Width = 130;
            // 
            // colType
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colType.DefaultCellStyle = dataGridViewCellStyle4;
            this.colType.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colType.HeaderText = "Type";
            this.colType.Items.AddRange(new object[] {
            "정수",
            "실수",
            "문자열",
            "참/거짓"});
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Width = 80;
            // 
            // colDescription
            // 
            this.colDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDescription.HeaderText = "설명";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            // 
            // checkBoxExpression
            // 
            this.checkBoxExpression.AutoSize = true;
            this.checkBoxExpression.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxExpression.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxExpression.ForeColor = System.Drawing.Color.White;
            this.checkBoxExpression.Location = new System.Drawing.Point(22, 623);
            this.checkBoxExpression.Name = "checkBoxExpression";
            this.checkBoxExpression.Size = new System.Drawing.Size(82, 18);
            this.checkBoxExpression.TabIndex = 105;
            this.checkBoxExpression.Text = "수식 사용";
            this.checkBoxExpression.UseVisualStyleBackColor = false;
            this.checkBoxExpression.Visible = false;
            this.checkBoxExpression.CheckedChanged += new System.EventHandler(this.checkBoxExpression_CheckedChanged);
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Font = new System.Drawing.Font("나눔스퀘어", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelType.ForeColor = System.Drawing.Color.White;
            this.labelType.Location = new System.Drawing.Point(7, 146);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(46, 21);
            this.labelType.TabIndex = 103;
            this.labelType.Text = "타입";
            // 
            // textBox
            // 
            this.textBox.Font = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox.Location = new System.Drawing.Point(0, 4);
            this.textBox.Margin = new System.Windows.Forms.Padding(0);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(432, 137);
            this.textBox.TabIndex = 102;
            this.textBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoveClickFormula_MouseDown);
            // 
            // lblAutoRun
            // 
            this.lblAutoRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAutoRun.AutoSize = true;
            this.lblAutoRun.Font = new System.Drawing.Font("나눔스퀘어 Bold", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAutoRun.ForeColor = System.Drawing.Color.White;
            this.lblAutoRun.Location = new System.Drawing.Point(136, 459);
            this.lblAutoRun.Name = "lblAutoRun";
            this.lblAutoRun.Size = new System.Drawing.Size(72, 18);
            this.lblAutoRun.TabIndex = 111;
            this.lblAutoRun.Text = "수식 사용";
            this.lblAutoRun.Click += new System.EventHandler(this.Expression_Click);
            this.lblAutoRun.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoveClickFormula_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔스퀘어", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 298);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 21);
            this.label1.TabIndex = 103;
            this.label1.Text = "사용자 정의 타입";
            // 
            // gridUserType
            // 
            this.gridUserType.AllowUserToAddRows = false;
            this.gridUserType.AllowUserToDeleteRows = false;
            this.gridUserType.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this.gridUserType.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridUserType.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.gridUserType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUserType.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewComboBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.gridUserType.Location = new System.Drawing.Point(0, 322);
            this.gridUserType.Name = "gridUserType";
            this.gridUserType.RowHeadersVisible = false;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Black;
            this.gridUserType.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.gridUserType.RowTemplate.Height = 23;
            this.gridUserType.Size = new System.Drawing.Size(433, 120);
            this.gridUserType.TabIndex = 106;
            this.gridUserType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grid_MouseDown);
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn1.HeaderText = "변수";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 130;
            // 
            // dataGridViewComboBoxColumn1
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewComboBoxColumn1.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewComboBoxColumn1.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.dataGridViewComboBoxColumn1.HeaderText = "Type";
            this.dataGridViewComboBoxColumn1.Items.AddRange(new object[] {
            "정수",
            "실수",
            "문자열",
            "참/거짓"});
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            this.dataGridViewComboBoxColumn1.ReadOnly = true;
            this.dataGridViewComboBoxColumn1.Width = 80;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "설명";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // pnMain
            // 
            this.pnMain.AllowDrop = true;
            this.pnMain.Location = new System.Drawing.Point(439, 3);
            this.pnMain.Name = "pnMain";
            this.pnMain.Size = new System.Drawing.Size(743, 397);
            this.pnMain.TabIndex = 116;
            this.pnMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.TrgDragDrop);
            this.pnMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.TrgDragEnter);
            this.pnMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoveClickFormula_MouseDown);
            // 
            // lblDesc
            // 
            this.lblDesc.AutoSize = true;
            this.lblDesc.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDesc.ForeColor = System.Drawing.Color.White;
            this.lblDesc.Location = new System.Drawing.Point(439, 412);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(107, 18);
            this.lblDesc.TabIndex = 117;
            this.lblDesc.Text = "Description";
            this.lblDesc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoveClickFormula_MouseDown);
            // 
            // btnHighRank
            // 
            this.btnHighRank.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHighRank.CheckButton = false;
            this.btnHighRank.CheckedBkgndImage = null;
            this.btnHighRank.CheckedImage = null;
            this.btnHighRank.ClickedBackgroundImage = null;
            this.btnHighRank.ClickedImage = global::SOPManager.Properties.Resources.BtnHighRank_Click;
            this.btnHighRank.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnHighRank.DisabledBkgndImage = null;
            this.btnHighRank.DisabledImage = null;
            this.btnHighRank.ID = -1;
            this.btnHighRank.InitButtonWidth = 69;
            this.btnHighRank.IsChecked = false;
            this.btnHighRank.Location = new System.Drawing.Point(1118, 449);
            this.btnHighRank.MouseOverBkgndImage = null;
            this.btnHighRank.MouseOverImage = global::SOPManager.Properties.Resources.BtnHighRank_Click;
            this.btnHighRank.Name = "btnHighRank";
            this.btnHighRank.NormalImage = global::SOPManager.Properties.Resources.BtnHighRank;
            this.btnHighRank.Owner = null;
            this.btnHighRank.Size = new System.Drawing.Size(69, 37);
            this.btnHighRank.TabIndex = 115;
            this.btnHighRank.TextLocation = new System.Drawing.Point(0, 0);
            this.btnHighRank.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnHighRank.ToolTipText = "";
            this.btnHighRank.UseCustomImageRect = true;
            this.btnHighRank.UseTextLocation = false;
            this.btnHighRank.UseVisualStyleBackColor = true;
            this.btnHighRank.Click += new System.EventHandler(this.btnHighRank_Click);
            this.btnHighRank.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoveClickFormula_MouseDown);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 69;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(367, 452);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(69, 37);
            this.btnCancel.TabIndex = 114;
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseCustomImageRect = true;
            this.btnCancel.UseTextLocation = false;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnCancel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoveClickFormula_MouseDown);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 69;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(300, 452);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 113;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            this.btnOK.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoveClickFormula_MouseDown);
            // 
            // btnShowSpecialMessage
            // 
            this.btnShowSpecialMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowSpecialMessage.CheckButton = false;
            this.btnShowSpecialMessage.CheckedBkgndImage = null;
            this.btnShowSpecialMessage.CheckedImage = null;
            this.btnShowSpecialMessage.ClickedBackgroundImage = null;
            this.btnShowSpecialMessage.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_SpecialcharoptionClick;
            this.btnShowSpecialMessage.CustomImageRect = new System.Drawing.Rectangle(0, 0, 110, 37);
            this.btnShowSpecialMessage.DisabledBkgndImage = null;
            this.btnShowSpecialMessage.DisabledImage = null;
            this.btnShowSpecialMessage.ID = -1;
            this.btnShowSpecialMessage.InitButtonWidth = 110;
            this.btnShowSpecialMessage.IsChecked = false;
            this.btnShowSpecialMessage.Location = new System.Drawing.Point(0, 452);
            this.btnShowSpecialMessage.MouseOverBkgndImage = null;
            this.btnShowSpecialMessage.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_SpecialcharoptionClick;
            this.btnShowSpecialMessage.Name = "btnShowSpecialMessage";
            this.btnShowSpecialMessage.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Specialcharoption;
            this.btnShowSpecialMessage.Owner = null;
            this.btnShowSpecialMessage.Size = new System.Drawing.Size(110, 37);
            this.btnShowSpecialMessage.TabIndex = 112;
            this.btnShowSpecialMessage.TextLocation = new System.Drawing.Point(0, 0);
            this.btnShowSpecialMessage.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnShowSpecialMessage.ToolTipText = "";
            this.btnShowSpecialMessage.UseCustomImageRect = true;
            this.btnShowSpecialMessage.UseTextLocation = true;
            this.btnShowSpecialMessage.UseVisualStyleBackColor = true;
            this.btnShowSpecialMessage.Click += new System.EventHandler(this.btnShowSpecialMessage_Click);
            this.btnShowSpecialMessage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoveClickFormula_MouseDown);
            // 
            // picAutoRun
            // 
            this.picAutoRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_enable;
            this.picAutoRun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picAutoRun.Location = new System.Drawing.Point(116, 458);
            this.picAutoRun.Name = "picAutoRun";
            this.picAutoRun.Size = new System.Drawing.Size(20, 20);
            this.picAutoRun.TabIndex = 110;
            this.picAutoRun.TabStop = false;
            this.picAutoRun.Click += new System.EventHandler(this.Expression_Click);
            this.picAutoRun.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoveClickFormula_MouseDown);
            // 
            // PopupNoteDecision3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(1186, 488);
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.pnMain);
            this.Controls.Add(this.btnHighRank);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnShowSpecialMessage);
            this.Controls.Add(this.gridUserType);
            this.Controls.Add(this.gridSystemType);
            this.Controls.Add(this.checkBoxExpression);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.picAutoRun);
            this.Controls.Add(this.lblAutoRun);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupNoteDecision3";
            this.Text = "판단문 작성";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PopupNoteDecision3_FormClosed);
            this.Load += new System.EventHandler(this.PopupNoteDecision3_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoveClickFormula_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.gridSystemType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUserType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAutoRun)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridSystemType;
        private System.Windows.Forms.CheckBox checkBoxExpression;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.PictureBox picAutoRun;
        private System.Windows.Forms.Label lblAutoRun;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gridUserType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVariable;
        private System.Windows.Forms.DataGridViewComboBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.RibbonButton btnShowSpecialMessage;
        private UnE.GUI.RibbonButton btnHighRank;
        private System.Windows.Forms.Panel pnMain;
        private System.Windows.Forms.Label lblDesc;
    }
}