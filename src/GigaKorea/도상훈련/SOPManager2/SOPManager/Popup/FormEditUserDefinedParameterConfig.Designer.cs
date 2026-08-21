namespace SOPManager.Popup
{
    partial class FormEditUserDefinedParameterConfig
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.gridItems = new System.Windows.Forms.DataGridView();
            this.colItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRename = new UnE.GUI.RibbonButton();
            this.btnRemove = new UnE.GUI.RibbonButton();
            this.btnClose = new UnE.GUI.RibbonButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "이름";
            // 
            // gridItems
            // 
            this.gridItems.AllowUserToAddRows = false;
            this.gridItems.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gridItems.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridItems.BackgroundColor = System.Drawing.Color.White;
            this.gridItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridItems.ColumnHeadersVisible = false;
            this.gridItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItem});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridItems.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridItems.Location = new System.Drawing.Point(14, 29);
            this.gridItems.MultiSelect = false;
            this.gridItems.Name = "gridItems";
            this.gridItems.RowHeadersVisible = false;
            dataGridViewCellStyle3.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gridItems.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.gridItems.RowTemplate.Height = 23;
            this.gridItems.Size = new System.Drawing.Size(271, 201);
            this.gridItems.TabIndex = 3;
            this.gridItems.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.gridItems_CellBeginEdit);
            this.gridItems.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridItems_CellValueChanged);
            // 
            // colItem
            // 
            this.colItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colItem.HeaderText = "Item";
            this.colItem.Name = "colItem";
            // 
            // btnRename
            // 
            this.btnRename.CheckButton = false;
            this.btnRename.CheckedBkgndImage = null;
            this.btnRename.CheckedImage = null;
            this.btnRename.ClickedBackgroundImage = null;
            this.btnRename.ClickedImage = global::SOPManager.Properties.Resources.EditUserDefined_ChangeNameClick;
            this.btnRename.CustomImageRect = new System.Drawing.Rectangle(0, 0, 96, 40);
            this.btnRename.DisabledBkgndImage = null;
            this.btnRename.DisabledImage = null;
            this.btnRename.ID = -1;
            this.btnRename.InitButtonWidth = 96;
            this.btnRename.IsChecked = false;
            this.btnRename.Location = new System.Drawing.Point(291, 73);
            this.btnRename.MouseOverBkgndImage = null;
            this.btnRename.MouseOverImage = global::SOPManager.Properties.Resources.EditUserDefined_ChangeNameClick;
            this.btnRename.Name = "btnRename";
            this.btnRename.NormalImage = global::SOPManager.Properties.Resources.EditUserDefined_ChangeName;
            this.btnRename.Owner = null;
            this.btnRename.Size = new System.Drawing.Size(96, 40);
            this.btnRename.TabIndex = 4;
            this.btnRename.TextLocation = new System.Drawing.Point(0, 0);
            this.btnRename.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnRename.ToolTipText = "";
            this.btnRename.UseCustomImageRect = true;
            this.btnRename.UseTextLocation = false;
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.CheckButton = false;
            this.btnRemove.CheckedBkgndImage = null;
            this.btnRemove.CheckedImage = null;
            this.btnRemove.ClickedBackgroundImage = null;
            this.btnRemove.ClickedImage = global::SOPManager.Properties.Resources.EditUserDefined_RemoveClick;
            this.btnRemove.CustomImageRect = new System.Drawing.Rectangle(0, 0, 96, 40);
            this.btnRemove.DisabledBkgndImage = null;
            this.btnRemove.DisabledImage = null;
            this.btnRemove.ID = -1;
            this.btnRemove.InitButtonWidth = 96;
            this.btnRemove.IsChecked = false;
            this.btnRemove.Location = new System.Drawing.Point(291, 27);
            this.btnRemove.MouseOverBkgndImage = null;
            this.btnRemove.MouseOverImage = global::SOPManager.Properties.Resources.EditUserDefined_RemoveClick;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.NormalImage = global::SOPManager.Properties.Resources.EditUserDefined_Remove;
            this.btnRemove.Owner = null;
            this.btnRemove.Size = new System.Drawing.Size(96, 40);
            this.btnRemove.TabIndex = 5;
            this.btnRemove.TextLocation = new System.Drawing.Point(0, 0);
            this.btnRemove.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnRemove.ToolTipText = "";
            this.btnRemove.UseCustomImageRect = true;
            this.btnRemove.UseTextLocation = false;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnClose
            // 
            this.btnClose.CheckButton = false;
            this.btnClose.CheckedBkgndImage = null;
            this.btnClose.CheckedImage = null;
            this.btnClose.ClickedBackgroundImage = null;
            this.btnClose.ClickedImage = global::SOPManager.Properties.Resources.EditUserDefined_CloseClick;
            this.btnClose.CustomImageRect = new System.Drawing.Rectangle(0, 0, 96, 40);
            this.btnClose.DisabledBkgndImage = null;
            this.btnClose.DisabledImage = null;
            this.btnClose.ID = -1;
            this.btnClose.InitButtonWidth = 96;
            this.btnClose.IsChecked = false;
            this.btnClose.Location = new System.Drawing.Point(291, 196);
            this.btnClose.MouseOverBkgndImage = null;
            this.btnClose.MouseOverImage = global::SOPManager.Properties.Resources.EditUserDefined_CloseClick;
            this.btnClose.Name = "btnClose";
            this.btnClose.NormalImage = global::SOPManager.Properties.Resources.EditUserDefined_Close;
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(96, 40);
            this.btnClose.TabIndex = 6;
            this.btnClose.TextLocation = new System.Drawing.Point(0, 0);
            this.btnClose.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnClose.ToolTipText = "";
            this.btnClose.UseCustomImageRect = true;
            this.btnClose.UseTextLocation = false;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormEditUserDefinedParameterConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(395, 242);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.gridItems);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormEditUserDefinedParameterConfig";
            this.Text = "설정 편집";
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gridItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItem;
        private UnE.GUI.RibbonButton btnRename;
        private UnE.GUI.RibbonButton btnRemove;
        private UnE.GUI.RibbonButton btnClose;
    }
}