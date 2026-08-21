namespace SoilMan.Popup
{
    partial class FormConfirmArea
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.gridArea = new UnE.Controls.MergedDataGridView();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPercentage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.gridCost = new UnE.Controls.MergedDataGridView();
            this.colCostType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCost)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "대상지 면적";
            // 
            // gridArea
            // 
            this.gridArea.AllowUserToAddRows = false;
            this.gridArea.AllowUserToDeleteRows = false;
            this.gridArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridArea.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridArea.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colType,
            this.colArea,
            this.colPercentage});
            this.gridArea.Location = new System.Drawing.Point(22, 37);
            this.gridArea.Name = "gridArea";
            this.gridArea.ReadOnly = true;
            this.gridArea.RowHeadersVisible = false;
            this.gridArea.RowTemplate.Height = 23;
            this.gridArea.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gridArea.Size = new System.Drawing.Size(250, 114);
            this.gridArea.TabIndex = 2;
            // 
            // colType
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colType.DefaultCellStyle = dataGridViewCellStyle1;
            this.colType.HeaderText = "구 분";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Width = 125;
            // 
            // colArea
            // 
            this.colArea.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colArea.DefaultCellStyle = dataGridViewCellStyle2;
            this.colArea.HeaderText = "면 적(ha)";
            this.colArea.Name = "colArea";
            this.colArea.ReadOnly = true;
            // 
            // colPercentage
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colPercentage.DefaultCellStyle = dataGridViewCellStyle3;
            this.colPercentage.HeaderText = "";
            this.colPercentage.Name = "colPercentage";
            this.colPercentage.ReadOnly = true;
            this.colPercentage.Width = 50;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(12, 167);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "대상지 공시지가";
            // 
            // gridCost
            // 
            this.gridCost.AllowUserToAddRows = false;
            this.gridCost.AllowUserToDeleteRows = false;
            this.gridCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCost.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCost.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCostType,
            this.colCost});
            this.gridCost.Location = new System.Drawing.Point(17, 195);
            this.gridCost.Name = "gridCost";
            this.gridCost.ReadOnly = true;
            this.gridCost.RowHeadersVisible = false;
            this.gridCost.RowTemplate.Height = 23;
            this.gridCost.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gridCost.Size = new System.Drawing.Size(250, 114);
            this.gridCost.TabIndex = 2;
            // 
            // colCostType
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCostType.DefaultCellStyle = dataGridViewCellStyle4;
            this.colCostType.HeaderText = "구 분";
            this.colCostType.Name = "colCostType";
            this.colCostType.ReadOnly = true;
            this.colCostType.Width = 125;
            // 
            // colCost
            // 
            this.colCost.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colCost.DefaultCellStyle = dataGridViewCellStyle5;
            this.colCost.HeaderText = "공시지가(억원)";
            this.colCost.Name = "colCost";
            this.colCost.ReadOnly = true;
            // 
            // FormConfirmArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 336);
            this.Controls.Add(this.gridCost);
            this.Controls.Add(this.gridArea);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormConfirmArea";
            this.Text = "FormConfirmArea";
            this.Load += new System.EventHandler(this.FormConfirmArea_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCost)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private UnE.Controls.MergedDataGridView gridArea;
        private System.Windows.Forms.Label label2;
        private UnE.Controls.MergedDataGridView gridCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCostType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPercentage;
    }
}