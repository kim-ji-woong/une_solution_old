namespace RoadMan
{
    partial class FormLandNumber
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLandNumber));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVillage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNumber1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHyphen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNumber2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStreetArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOwnerType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNumber,
            this.colVillage,
            this.colNumber1,
            this.colHyphen,
            this.colNumber2,
            this.colTotalArea,
            this.colStreetArea,
            this.colOwnerType,
            this.colCost});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(807, 307);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // colNumber
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNumber.DefaultCellStyle = dataGridViewCellStyle1;
            this.colNumber.FillWeight = 76.14214F;
            this.colNumber.HeaderText = "No";
            this.colNumber.Name = "colNumber";
            this.colNumber.ReadOnly = true;
            this.colNumber.Width = 30;
            // 
            // colVillage
            // 
            this.colVillage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colVillage.DefaultCellStyle = dataGridViewCellStyle2;
            this.colVillage.FillWeight = 105.9645F;
            this.colVillage.HeaderText = "동리";
            this.colVillage.Name = "colVillage";
            // 
            // colNumber1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNumber1.DefaultCellStyle = dataGridViewCellStyle3;
            this.colNumber1.FillWeight = 105.9645F;
            this.colNumber1.HeaderText = "본번";
            this.colNumber1.Name = "colNumber1";
            this.colNumber1.Width = 99;
            // 
            // colHyphen
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colHyphen.DefaultCellStyle = dataGridViewCellStyle4;
            this.colHyphen.FillWeight = 105.9645F;
            this.colHyphen.HeaderText = "-";
            this.colHyphen.Name = "colHyphen";
            this.colHyphen.Width = 20;
            // 
            // colNumber2
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNumber2.DefaultCellStyle = dataGridViewCellStyle5;
            this.colNumber2.FillWeight = 105.9645F;
            this.colNumber2.HeaderText = "부번";
            this.colNumber2.Name = "colNumber2";
            this.colNumber2.Width = 99;
            // 
            // colTotalArea
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTotalArea.DefaultCellStyle = dataGridViewCellStyle6;
            this.colTotalArea.HeaderText = "지적면적(m²)";
            this.colTotalArea.Name = "colTotalArea";
            this.colTotalArea.Width = 110;
            // 
            // colStreetArea
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colStreetArea.DefaultCellStyle = dataGridViewCellStyle7;
            this.colStreetArea.HeaderText = "편입면적(m²)";
            this.colStreetArea.Name = "colStreetArea";
            this.colStreetArea.Width = 110;
            // 
            // colOwnerType
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colOwnerType.DefaultCellStyle = dataGridViewCellStyle8;
            this.colOwnerType.HeaderText = "소유구분";
            this.colOwnerType.Name = "colOwnerType";
            // 
            // colCost
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCost.DefaultCellStyle = dataGridViewCellStyle9;
            this.colCost.HeaderText = "공시지가(원)";
            this.colCost.Name = "colCost";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(738, 313);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(57, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(676, 313);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(57, 23);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FormLandNumber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 341);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormLandNumber";
            this.Text = "토지지번";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLandNumber_FormClosing);
            this.Load += new System.EventHandler(this.FormLandNumber_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVillage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHyphen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStreetArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOwnerType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCost;
    }
}