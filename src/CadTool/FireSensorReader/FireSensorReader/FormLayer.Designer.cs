namespace FireSensorReader
{
    partial class FormLayer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridLayer = new System.Windows.Forms.DataGridView();
            this.colLayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVisible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLayerColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxAllLayer = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridLayer)).BeginInit();
            this.SuspendLayout();
            // 
            // gridLayer
            // 
            this.gridLayer.AllowUserToAddRows = false;
            this.gridLayer.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridLayer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridLayer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLayer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLayerName,
            this.colVisible,
            this.colLayerColor});
            this.gridLayer.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridLayer.Location = new System.Drawing.Point(0, 0);
            this.gridLayer.Name = "gridLayer";
            this.gridLayer.RowHeadersVisible = false;
            this.gridLayer.RowTemplate.Height = 23;
            this.gridLayer.Size = new System.Drawing.Size(400, 367);
            this.gridLayer.TabIndex = 0;
            this.gridLayer.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridLayer_CellContentClick);
            this.gridLayer.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridLayer_CellValueChanged);
            // 
            // colLayerName
            // 
            this.colLayerName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colLayerName.HeaderText = "Layer 이름";
            this.colLayerName.Name = "colLayerName";
            this.colLayerName.ReadOnly = true;
            this.colLayerName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colVisible
            // 
            this.colVisible.HeaderText = "켜기";
            this.colVisible.Name = "colVisible";
            this.colVisible.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colLayerColor
            // 
            this.colLayerColor.HeaderText = "색상";
            this.colLayerColor.Name = "colLayerColor";
            this.colLayerColor.ReadOnly = true;
            this.colLayerColor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colLayerColor.Width = 80;
            // 
            // checkBoxAllLayer
            // 
            this.checkBoxAllLayer.AutoSize = true;
            this.checkBoxAllLayer.Checked = true;
            this.checkBoxAllLayer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAllLayer.Location = new System.Drawing.Point(12, 398);
            this.checkBoxAllLayer.Name = "checkBoxAllLayer";
            this.checkBoxAllLayer.Size = new System.Drawing.Size(164, 16);
            this.checkBoxAllLayer.TabIndex = 1;
            this.checkBoxAllLayer.Text = "Layer 한꺼번에 끄고 켜기";
            this.checkBoxAllLayer.UseVisualStyleBackColor = true;
            this.checkBoxAllLayer.CheckedChanged += new System.EventHandler(this.checkBoxAllLayer_CheckedChanged);
            // 
            // FormLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 450);
            this.Controls.Add(this.checkBoxAllLayer);
            this.Controls.Add(this.gridLayer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormLayer";
            this.Text = "FormLayer";
            ((System.ComponentModel.ISupportInitialize)(this.gridLayer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridLayer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLayerName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLayerColor;
        private System.Windows.Forms.CheckBox checkBoxAllLayer;
    }
}