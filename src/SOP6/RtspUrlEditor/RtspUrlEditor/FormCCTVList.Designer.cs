namespace RtspUrlEditor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridCCTV = new System.Windows.Forms.DataGridView();
            this.ColID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCCTVName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridCCTV)).BeginInit();
            this.SuspendLayout();
            // 
            // gridCCTV
            // 
            this.gridCCTV.AllowDrop = true;
            this.gridCCTV.AllowUserToAddRows = false;
            this.gridCCTV.AllowUserToDeleteRows = false;
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
            this.ColID,
            this.ColCCTVName,
            this.ColPosition});
            this.gridCCTV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCCTV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gridCCTV.Location = new System.Drawing.Point(0, 0);
            this.gridCCTV.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridCCTV.MultiSelect = false;
            this.gridCCTV.Name = "gridCCTV";
            this.gridCCTV.ReadOnly = true;
            this.gridCCTV.RowHeadersVisible = false;
            this.gridCCTV.RowTemplate.Height = 23;
            this.gridCCTV.Size = new System.Drawing.Size(800, 450);
            this.gridCCTV.TabIndex = 1;
            this.gridCCTV.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridCCTV_CellMouseDown);
            this.gridCCTV.DragOver += new System.Windows.Forms.DragEventHandler(this.gridCCTV_DragOver);
            // 
            // ColID
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColID.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColID.HeaderText = "ID";
            this.ColID.Name = "ColID";
            this.ColID.ReadOnly = true;
            this.ColID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColID.Width = 80;
            // 
            // ColCCTVName
            // 
            this.ColCCTVName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.ColCCTVName.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColCCTVName.HeaderText = "CCTV 이름";
            this.ColCCTVName.Name = "ColCCTVName";
            this.ColCCTVName.ReadOnly = true;
            this.ColCCTVName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColPosition
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.ColPosition.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColPosition.HeaderText = "위치";
            this.ColPosition.Name = "ColPosition";
            this.ColPosition.ReadOnly = true;
            this.ColPosition.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColPosition.Width = 200;
            // 
            // FormCCTVList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gridCCTV);
            this.Name = "FormCCTVList";
            this.Text = "CCTV List";
            this.Load += new System.EventHandler(this.FormCCTVList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridCCTV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridCCTV;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCCTVName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColPosition;
    }
}