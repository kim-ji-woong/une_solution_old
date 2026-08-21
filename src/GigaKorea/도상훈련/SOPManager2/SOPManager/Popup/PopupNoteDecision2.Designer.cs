namespace SOPManager.Popup
{
    partial class PopupNoteDecision2
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridSystemType = new System.Windows.Forms.DataGridView();
            this.colVariable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxExpression = new System.Windows.Forms.CheckBox();
            this.labelType = new System.Windows.Forms.Label();
            this.labelExpression = new System.Windows.Forms.Label();
            this.textBoxExpression = new System.Windows.Forms.TextBox();
            this.textBox = new System.Windows.Forms.TextBox();
            this.lblAutoRun = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gridUserType = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.picAutoRun = new System.Windows.Forms.PictureBox();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.btnShowSpecialMessage = new UnE.GUI.RibbonButton();
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
            dataGridViewCellStyle1.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridSystemType.DefaultCellStyle = dataGridViewCellStyle5;
            this.gridSystemType.Location = new System.Drawing.Point(0, 297);
            this.gridSystemType.Name = "gridSystemType";
            this.gridSystemType.RowHeadersVisible = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this.gridSystemType.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.gridSystemType.RowTemplate.Height = 23;
            this.gridSystemType.Size = new System.Drawing.Size(433, 120);
            this.gridSystemType.TabIndex = 106;
            this.gridSystemType.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridType_CellClick);
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
            this.checkBoxExpression.Font = new System.Drawing.Font(Program.prgFont, 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxExpression.ForeColor = System.Drawing.Color.White;
            this.checkBoxExpression.Location = new System.Drawing.Point(22, 604);
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
            this.labelType.Font = new System.Drawing.Font(Program.prgFont, 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelType.ForeColor = System.Drawing.Color.White;
            this.labelType.Location = new System.Drawing.Point(4, 270);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(88, 21);
            this.labelType.TabIndex = 103;
            this.labelType.Text = "기본 타입";
            // 
            // labelExpression
            // 
            this.labelExpression.AutoSize = true;
            this.labelExpression.Font = new System.Drawing.Font(Program.prgFont, 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelExpression.ForeColor = System.Drawing.Color.White;
            this.labelExpression.Location = new System.Drawing.Point(4, 124);
            this.labelExpression.Name = "labelExpression";
            this.labelExpression.Size = new System.Drawing.Size(46, 21);
            this.labelExpression.TabIndex = 104;
            this.labelExpression.Text = "수식";
            // 
            // textBoxExpression
            // 
            this.textBoxExpression.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExpression.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxExpression.Location = new System.Drawing.Point(0, 149);
            this.textBoxExpression.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxExpression.Multiline = true;
            this.textBoxExpression.Name = "textBoxExpression";
            this.textBoxExpression.Size = new System.Drawing.Size(433, 111);
            this.textBoxExpression.TabIndex = 101;
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox.Location = new System.Drawing.Point(0, 9);
            this.textBox.Margin = new System.Windows.Forms.Padding(0);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(433, 229);
            this.textBox.TabIndex = 102;
            // 
            // lblAutoRun
            // 
            this.lblAutoRun.AutoSize = true;
            this.lblAutoRun.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAutoRun.ForeColor = System.Drawing.Color.White;
            this.lblAutoRun.Location = new System.Drawing.Point(137, 595);
            this.lblAutoRun.Name = "lblAutoRun";
            this.lblAutoRun.Size = new System.Drawing.Size(72, 18);
            this.lblAutoRun.TabIndex = 111;
            this.lblAutoRun.Text = "수식 사용";
            this.lblAutoRun.Click += new System.EventHandler(this.Expression_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(4, 427);
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
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            this.gridUserType.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridUserType.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.gridUserType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUserType.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewComboBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridUserType.DefaultCellStyle = dataGridViewCellStyle11;
            this.gridUserType.Location = new System.Drawing.Point(0, 454);
            this.gridUserType.Name = "gridUserType";
            this.gridUserType.RowHeadersVisible = false;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle12.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.Black;
            this.gridUserType.RowsDefaultCellStyle = dataGridViewCellStyle12;
            this.gridUserType.RowTemplate.Height = 23;
            this.gridUserType.Size = new System.Drawing.Size(433, 120);
            this.gridUserType.TabIndex = 106;
            this.gridUserType.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridType_CellClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewTextBoxColumn1.HeaderText = "변수";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 130;
            // 
            // dataGridViewComboBoxColumn1
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewComboBoxColumn1.DefaultCellStyle = dataGridViewCellStyle10;
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
            // picAutoRun
            // 
            this.picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_enable;
            this.picAutoRun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picAutoRun.Location = new System.Drawing.Point(117, 594);
            this.picAutoRun.Name = "picAutoRun";
            this.picAutoRun.Size = new System.Drawing.Size(20, 20);
            this.picAutoRun.TabIndex = 110;
            this.picAutoRun.TabStop = false;
            this.picAutoRun.Click += new System.EventHandler(this.Expression_Click);
            // 
            // btnCancel
            // 
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
            this.btnCancel.Location = new System.Drawing.Point(362, 585);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(69, 37);
            this.btnCancel.TabIndex = 109;
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseCustomImageRect = true;
            this.btnCancel.UseTextLocation = false;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
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
            this.btnOK.Location = new System.Drawing.Point(292, 585);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 108;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnShowSpecialMessage
            // 
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
            this.btnShowSpecialMessage.Location = new System.Drawing.Point(3, 585);
            this.btnShowSpecialMessage.MouseOverBkgndImage = null;
            this.btnShowSpecialMessage.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_SpecialcharoptionClick;
            this.btnShowSpecialMessage.Name = "btnShowSpecialMessage";
            this.btnShowSpecialMessage.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Specialcharoption;
            this.btnShowSpecialMessage.Owner = null;
            this.btnShowSpecialMessage.Size = new System.Drawing.Size(110, 37);
            this.btnShowSpecialMessage.TabIndex = 107;
            this.btnShowSpecialMessage.TextLocation = new System.Drawing.Point(0, 0);
            this.btnShowSpecialMessage.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnShowSpecialMessage.ToolTipText = "";
            this.btnShowSpecialMessage.UseCustomImageRect = true;
            this.btnShowSpecialMessage.UseTextLocation = true;
            this.btnShowSpecialMessage.UseVisualStyleBackColor = true;
            this.btnShowSpecialMessage.Click += new System.EventHandler(this.btnShowSpecialMessage_Click);
            // 
            // PopupNoteDecision2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(433, 625);
            this.Controls.Add(this.gridUserType);
            this.Controls.Add(this.gridSystemType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.labelExpression);
            this.Controls.Add(this.textBoxExpression);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.picAutoRun);
            this.Controls.Add(this.lblAutoRun);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnShowSpecialMessage);
            this.Controls.Add(this.checkBoxExpression);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupNoteDecision2";
            this.Text = "판단문 작성";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PopupNoteDecision2_FormClosed);
            this.Load += new System.EventHandler(this.PopupNoteDecision2_Load);
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
        private System.Windows.Forms.Label labelExpression;
        private System.Windows.Forms.TextBox textBoxExpression;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.PictureBox picAutoRun;
        private System.Windows.Forms.Label lblAutoRun;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.RibbonButton btnShowSpecialMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gridUserType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVariable;
        private System.Windows.Forms.DataGridViewComboBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}