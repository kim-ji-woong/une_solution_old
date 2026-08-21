namespace SoilMan.Popup
{
    partial class FormInputCondition
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
            this.gridCost = new UnE.Controls.MergedDataGridView();
            this.colCostType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridCondition = new UnE.Controls.MergedDataGridView();
            this.colItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCondition)).BeginInit();
            this.SuspendLayout();
            // 
            // gridCost
            // 
            this.gridCost.AllowUserToAddRows = false;
            this.gridCost.AllowUserToDeleteRows = false;
            this.gridCost.AllowUserToResizeRows = false;
            this.gridCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCost.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCost.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCostType,
            this.colCost,
            this.Column1});
            this.gridCost.Location = new System.Drawing.Point(17, 187);
            this.gridCost.Name = "gridCost";
            this.gridCost.RowHeadersVisible = false;
            this.gridCost.RowTemplate.Height = 23;
            this.gridCost.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gridCost.Size = new System.Drawing.Size(343, 114);
            this.gridCost.TabIndex = 5;
            this.gridCost.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCost_CellClick);
            this.gridCost.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCost_CellValueChanged);
            // 
            // colCostType
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCostType.DefaultCellStyle = dataGridViewCellStyle1;
            this.colCostType.HeaderText = "구 분";
            this.colCostType.Name = "colCostType";
            this.colCostType.Width = 125;
            // 
            // colCost
            // 
            this.colCost.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colCost.DefaultCellStyle = dataGridViewCellStyle2;
            this.colCost.HeaderText = "경제적가치(억원/년)";
            this.colCost.Name = "colCost";
            // 
            // Column1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            this.Column1.Width = 160;
            // 
            // gridCondition
            // 
            this.gridCondition.AllowUserToAddRows = false;
            this.gridCondition.AllowUserToDeleteRows = false;
            this.gridCondition.AllowUserToResizeRows = false;
            this.gridCondition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCondition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCondition.ColumnHeadersVisible = false;
            this.gridCondition.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItem,
            this.colData});
            this.gridCondition.Location = new System.Drawing.Point(17, 37);
            this.gridCondition.Name = "gridCondition";
            this.gridCondition.RowHeadersVisible = false;
            this.gridCondition.RowTemplate.Height = 23;
            this.gridCondition.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gridCondition.Size = new System.Drawing.Size(343, 114);
            this.gridCondition.TabIndex = 6;
            this.gridCondition.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCondition_CellClick);
            this.gridCondition.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCondition_CellValueChanged);
            // 
            // colItem
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colItem.DefaultCellStyle = dataGridViewCellStyle4;
            this.colItem.HeaderText = "항목";
            this.colItem.Name = "colItem";
            this.colItem.Width = 125;
            // 
            // colData
            // 
            this.colData.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colData.DefaultCellStyle = dataGridViewCellStyle5;
            this.colData.HeaderText = "데이터";
            this.colData.Name = "colData";
            this.colData.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(12, 158);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "비사용가치 입력";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "분석조건";
            // 
            // FormInputCondition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 386);
            this.Controls.Add(this.gridCost);
            this.Controls.Add(this.gridCondition);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormInputCondition";
            this.Text = "FormInputCondition";
            this.Load += new System.EventHandler(this.FormInputCondition_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCondition)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UnE.Controls.MergedDataGridView gridCost;
        private UnE.Controls.MergedDataGridView gridCondition;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCostType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colData;
    }
}