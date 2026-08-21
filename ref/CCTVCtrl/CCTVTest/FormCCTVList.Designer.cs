namespace UnECCTV
{
    partial class FormCCTVList
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
            this.gridCCTV = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChannel1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colChannel2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colChannel3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridCCTV)).BeginInit();
            this.SuspendLayout();
            // 
            // gridCCTV
            // 
            this.gridCCTV.AllowUserToAddRows = false;
            this.gridCCTV.AllowUserToDeleteRows = false;
            this.gridCCTV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridCCTV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridCCTV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCCTV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colName,
            this.colChannel1,
            this.colChannel2,
            this.colChannel3});
            this.gridCCTV.Location = new System.Drawing.Point(12, 12);
            this.gridCCTV.Name = "gridCCTV";
            this.gridCCTV.ReadOnly = true;
            this.gridCCTV.RowHeadersVisible = false;
            this.gridCCTV.RowTemplate.Height = 23;
            this.gridCCTV.Size = new System.Drawing.Size(776, 426);
            this.gridCCTV.TabIndex = 0;
            this.gridCCTV.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridCCTV_CellMouseUp);
            // 
            // colNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 60;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colName.DefaultCellStyle = dataGridViewCellStyle3;
            this.colName.HeaderText = "CCTV 이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colChannel1
            // 
            this.colChannel1.HeaderText = "1번 채널";
            this.colChannel1.Name = "colChannel1";
            this.colChannel1.ReadOnly = true;
            // 
            // colChannel2
            // 
            this.colChannel2.HeaderText = "2번 채널";
            this.colChannel2.Name = "colChannel2";
            this.colChannel2.ReadOnly = true;
            // 
            // colChannel3
            // 
            this.colChannel3.HeaderText = "3번 채널";
            this.colChannel3.Name = "colChannel3";
            this.colChannel3.ReadOnly = true;
            // 
            // FormCCTVList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gridCCTV);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormCCTVList";
            this.Text = "CCTV List";
            ((System.ComponentModel.ISupportInitialize)(this.gridCCTV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridCCTV;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colChannel1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colChannel2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colChannel3;
    }
}