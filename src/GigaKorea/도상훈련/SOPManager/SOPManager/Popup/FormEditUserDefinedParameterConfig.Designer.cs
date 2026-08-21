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
            this.label1 = new System.Windows.Forms.Label();
            this.gridItems = new System.Windows.Forms.DataGridView();
            this.colItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRemove = new UnE.GUI.ImageButton();
            this.btnRename = new UnE.GUI.ImageButton();
            this.btnClose = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRename)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔스퀘어", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "이름";
            // 
            // gridItems
            // 
            this.gridItems.AllowUserToAddRows = false;
            this.gridItems.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.gridItems.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridItems.BackgroundColor = System.Drawing.Color.White;
            this.gridItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridItems.ColumnHeadersVisible = false;
            this.gridItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItem});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridItems.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridItems.Location = new System.Drawing.Point(14, 32);
            this.gridItems.MultiSelect = false;
            this.gridItems.Name = "gridItems";
            this.gridItems.RowHeadersVisible = false;
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
            // btnRemove
            // 
            this.btnRemove.ButtonText = "";
            this.btnRemove.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemove.ImageClicked = global::SOPManager.Properties.Resources.EditUserDefined_RemoveClick;
            this.btnRemove.ImageDisabled = null;
            this.btnRemove.ImageMouseOver = global::SOPManager.Properties.Resources.EditUserDefined_RemoveClick;
            this.btnRemove.ImageNormal = global::SOPManager.Properties.Resources.EditUserDefined_Remove;
            this.btnRemove.Location = new System.Drawing.Point(290, 32);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Owner = null;
            this.btnRemove.Size = new System.Drawing.Size(95, 34);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.TabStop = false;
            this.btnRemove.TextColor = System.Drawing.Color.Black;
            this.btnRemove.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemove.ToolTipText = "";
            this.btnRemove.UseToolTip = false;
            this.btnRemove.WindowRateWidth = 1F;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnRename
            // 
            this.btnRename.ButtonText = "";
            this.btnRename.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRename.ImageClicked = global::SOPManager.Properties.Resources.EditUserDefined_ChangeNameClick;
            this.btnRename.ImageDisabled = null;
            this.btnRename.ImageMouseOver = global::SOPManager.Properties.Resources.EditUserDefined_ChangeNameClick;
            this.btnRename.ImageNormal = global::SOPManager.Properties.Resources.EditUserDefined_ChangeName;
            this.btnRename.Location = new System.Drawing.Point(290, 72);
            this.btnRename.Name = "btnRename";
            this.btnRename.Owner = null;
            this.btnRename.Size = new System.Drawing.Size(95, 34);
            this.btnRename.TabIndex = 5;
            this.btnRename.TabStop = false;
            this.btnRename.TextColor = System.Drawing.Color.Black;
            this.btnRename.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRename.ToolTipText = "";
            this.btnRename.UseToolTip = false;
            this.btnRename.WindowRateWidth = 1F;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ImageClicked = global::SOPManager.Properties.Resources.EditUserDefined_CloseClick;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SOPManager.Properties.Resources.EditUserDefined_CloseClick;
            this.btnClose.ImageNormal = global::SOPManager.Properties.Resources.EditUserDefined_Close;
            this.btnClose.Location = new System.Drawing.Point(291, 196);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(95, 37);
            this.btnClose.TabIndex = 4;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormEditUserDefinedParameterConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(388, 242);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gridItems);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormEditUserDefinedParameterConfig";
            this.Text = "설정 편집";
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRename)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gridItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItem;
        private UnE.GUI.ImageButton btnClose;
        private UnE.GUI.ImageButton btnRename;
        private UnE.GUI.ImageButton btnRemove;
    }
}