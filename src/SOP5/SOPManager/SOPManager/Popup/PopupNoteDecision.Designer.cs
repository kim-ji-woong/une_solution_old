namespace SOPManager
{
    partial class PopupNoteDecision
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupNoteDecision));
            this.textBox = new System.Windows.Forms.TextBox();
            this.labelExpression = new System.Windows.Forms.Label();
            this.textBoxExpression = new System.Windows.Forms.TextBox();
            this.labelType = new System.Windows.Forms.Label();
            this.checkBoxExpression = new System.Windows.Forms.CheckBox();
            this.gridType = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVariable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnShowSpecialMessage = new UnE.GUI.RibbonButton();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.picAutoRun = new System.Windows.Forms.PictureBox();
            this.lblAutoRun = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAutoRun)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox.Location = new System.Drawing.Point(0, 4);
            this.textBox.Margin = new System.Windows.Forms.Padding(0);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(433, 111);
            this.textBox.TabIndex = 20;
            // 
            // labelExpression
            // 
            this.labelExpression.AutoSize = true;
            this.labelExpression.Font = new System.Drawing.Font(Program.prgFont, 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelExpression.ForeColor = System.Drawing.Color.White;
            this.labelExpression.Location = new System.Drawing.Point(4, 130);
            this.labelExpression.Name = "labelExpression";
            this.labelExpression.Size = new System.Drawing.Size(46, 21);
            this.labelExpression.TabIndex = 21;
            this.labelExpression.Text = "수식";
            // 
            // textBoxExpression
            // 
            this.textBoxExpression.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxExpression.Location = new System.Drawing.Point(0, 155);
            this.textBoxExpression.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxExpression.Multiline = true;
            this.textBoxExpression.Name = "textBoxExpression";
            this.textBoxExpression.Size = new System.Drawing.Size(433, 111);
            this.textBoxExpression.TabIndex = 20;
            this.textBoxExpression.TextChanged += new System.EventHandler(this.textBoxExpression_TextChanged);
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Font = new System.Drawing.Font(Program.prgFont, 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelType.ForeColor = System.Drawing.Color.White;
            this.labelType.Location = new System.Drawing.Point(4, 288);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(82, 21);
            this.labelType.TabIndex = 21;
            this.labelType.Text = "타입정의";
            // 
            // checkBoxExpression
            // 
            this.checkBoxExpression.AutoSize = true;
            this.checkBoxExpression.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxExpression.Font = new System.Drawing.Font(Program.prgFont, 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxExpression.ForeColor = System.Drawing.Color.White;
            this.checkBoxExpression.Location = new System.Drawing.Point(120, 454);
            this.checkBoxExpression.Name = "checkBoxExpression";
            this.checkBoxExpression.Size = new System.Drawing.Size(82, 18);
            this.checkBoxExpression.TabIndex = 26;
            this.checkBoxExpression.Text = "수식 사용";
            this.checkBoxExpression.UseVisualStyleBackColor = false;
            this.checkBoxExpression.Visible = false;
            this.checkBoxExpression.CheckedChanged += new System.EventHandler(this.checkBoxExpression_CheckedChanged);
            // 
            // gridType
            // 
            this.gridType.AllowUserToAddRows = false;
            this.gridType.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.gridType.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridType.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridType.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colVariable,
            this.colType});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridType.DefaultCellStyle = dataGridViewCellStyle6;
            this.gridType.Location = new System.Drawing.Point(0, 315);
            this.gridType.Name = "gridType";
            this.gridType.RowHeadersVisible = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(168)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            this.gridType.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.gridType.RowTemplate.Height = 23;
            this.gridType.Size = new System.Drawing.Size(433, 120);
            this.gridType.TabIndex = 27;
            // 
            // colNo
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle3;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 60;
            // 
            // colVariable
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colVariable.DefaultCellStyle = dataGridViewCellStyle4;
            this.colVariable.HeaderText = "변수";
            this.colVariable.Name = "colVariable";
            this.colVariable.ReadOnly = true;
            this.colVariable.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colVariable.Width = 250;
            // 
            // colType
            // 
            this.colType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colType.DefaultCellStyle = dataGridViewCellStyle5;
            this.colType.HeaderText = "Type";
            this.colType.Items.AddRange(new object[] {
            "unknown",
            "boolean",
            "double",
            "integer",
            "string"});
            this.colType.Name = "colType";
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
            this.btnShowSpecialMessage.Location = new System.Drawing.Point(0, 264);
            this.btnShowSpecialMessage.MouseOverBkgndImage = null;
            this.btnShowSpecialMessage.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_SpecialcharoptionClick;
            this.btnShowSpecialMessage.Name = "btnShowSpecialMessage";
            this.btnShowSpecialMessage.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Specialcharoption;
            this.btnShowSpecialMessage.Owner = null;
            this.btnShowSpecialMessage.Size = new System.Drawing.Size(110, 37);
            this.btnShowSpecialMessage.TabIndex = 42;
            this.btnShowSpecialMessage.TextLocation = new System.Drawing.Point(0, 0);
            this.btnShowSpecialMessage.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnShowSpecialMessage.ToolTipText = "";
            this.btnShowSpecialMessage.UseCustomImageRect = true;
            this.btnShowSpecialMessage.UseTextLocation = true;
            this.btnShowSpecialMessage.UseVisualStyleBackColor = true;
            this.btnShowSpecialMessage.Click += new System.EventHandler(this.btnShowSpecialMessage_Click);
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
            this.btnCancel.Location = new System.Drawing.Point(368, 264);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(69, 37);
            this.btnCancel.TabIndex = 44;
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
            this.btnOK.Location = new System.Drawing.Point(301, 264);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 43;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // picAutoRun
            // 
            this.picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_enable;
            this.picAutoRun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picAutoRun.Location = new System.Drawing.Point(116, 273);
            this.picAutoRun.Name = "picAutoRun";
            this.picAutoRun.Size = new System.Drawing.Size(20, 20);
            this.picAutoRun.TabIndex = 99;
            this.picAutoRun.TabStop = false;
            this.picAutoRun.Click += new System.EventHandler(this.Expression_Click);
            // 
            // lblAutoRun
            // 
            this.lblAutoRun.AutoSize = true;
            this.lblAutoRun.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAutoRun.ForeColor = System.Drawing.Color.White;
            this.lblAutoRun.Location = new System.Drawing.Point(136, 275);
            this.lblAutoRun.Name = "lblAutoRun";
            this.lblAutoRun.Size = new System.Drawing.Size(72, 18);
            this.lblAutoRun.TabIndex = 100;
            this.lblAutoRun.Text = "수식 사용";
            this.lblAutoRun.Click += new System.EventHandler(this.Expression_Click);
            // 
            // PopupNoteDecision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(433, 493);
            this.Controls.Add(this.picAutoRun);
            this.Controls.Add(this.lblAutoRun);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnShowSpecialMessage);
            this.Controls.Add(this.gridType);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.labelExpression);
            this.Controls.Add(this.textBoxExpression);
            this.Controls.Add(this.checkBoxExpression);
            this.Controls.Add(this.textBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PopupNoteDecision";
            this.Text = "판단문 작성";
            ((System.ComponentModel.ISupportInitialize)(this.gridType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAutoRun)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label labelExpression;
        private System.Windows.Forms.TextBox textBoxExpression;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.CheckBox checkBoxExpression;
        private System.Windows.Forms.DataGridView gridType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVariable;
        private System.Windows.Forms.DataGridViewComboBoxColumn colType;
        private UnE.GUI.RibbonButton btnShowSpecialMessage;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnOK;
        private System.Windows.Forms.PictureBox picAutoRun;
        private System.Windows.Forms.Label lblAutoRun;
    }
}