namespace RoadMan
{
    partial class FormResultPropertyHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormResultPropertyHistory));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBeginTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProjectCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAcumulatedLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProjectLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAcumulatedArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProjectArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStartDirection = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnSort = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colProjectName,
            this.colBeginTime,
            this.colEndTime,
            this.colProjectCost,
            this.colAcumulatedLength,
            this.colProjectLength,
            this.colAcumulatedArea,
            this.colProjectArea,
            this.colStartDirection});
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(984, 270);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView1_EditingControlShowing);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            this.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView1_Scroll);
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // colNo
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle1;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 30;
            // 
            // colProjectName
            // 
            this.colProjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colProjectName.HeaderText = "사업명";
            this.colProjectName.Name = "colProjectName";
            // 
            // colBeginTime
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colBeginTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.colBeginTime.HeaderText = "사업시작일";
            this.colBeginTime.Name = "colBeginTime";
            this.colBeginTime.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colEndTime
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colEndTime.DefaultCellStyle = dataGridViewCellStyle3;
            this.colEndTime.HeaderText = "사업종료일";
            this.colEndTime.Name = "colEndTime";
            this.colEndTime.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colProjectCost
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colProjectCost.DefaultCellStyle = dataGridViewCellStyle4;
            this.colProjectCost.HeaderText = "사업비(원)";
            this.colProjectCost.Name = "colProjectCost";
            this.colProjectCost.Width = 120;
            // 
            // colAcumulatedLength
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colAcumulatedLength.DefaultCellStyle = dataGridViewCellStyle5;
            this.colAcumulatedLength.HeaderText = "누적길이(m)";
            this.colAcumulatedLength.Name = "colAcumulatedLength";
            this.colAcumulatedLength.ReadOnly = true;
            // 
            // colProjectLength
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colProjectLength.DefaultCellStyle = dataGridViewCellStyle6;
            this.colProjectLength.HeaderText = "길이(m)";
            this.colProjectLength.Name = "colProjectLength";
            this.colProjectLength.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colProjectLength.Width = 80;
            // 
            // colAcumulatedArea
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colAcumulatedArea.DefaultCellStyle = dataGridViewCellStyle7;
            this.colAcumulatedArea.HeaderText = "누적면적(m²)";
            this.colAcumulatedArea.Name = "colAcumulatedArea";
            this.colAcumulatedArea.ReadOnly = true;
            this.colAcumulatedArea.Width = 120;
            // 
            // colProjectArea
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colProjectArea.DefaultCellStyle = dataGridViewCellStyle8;
            this.colProjectArea.HeaderText = "면적(m²)";
            this.colProjectArea.Name = "colProjectArea";
            this.colProjectArea.Width = 80;
            // 
            // colStartDirection
            // 
            this.colStartDirection.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colStartDirection.HeaderText = "공사방향";
            this.colStartDirection.Items.AddRange(new object[] {
            "시점으로부터",
            "종점으로부터"});
            this.colStartDirection.Name = "colStartDirection";
            this.colStartDirection.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colStartDirection.Width = 120;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(920, 278);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(59, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(855, 278);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(59, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnSort
            // 
            this.btnSort.Location = new System.Drawing.Point(12, 278);
            this.btnSort.Name = "btnSort";
            this.btnSort.Size = new System.Drawing.Size(75, 23);
            this.btnSort.TabIndex = 5;
            this.btnSort.Text = "정렬하기";
            this.btnSort.UseVisualStyleBackColor = true;
            this.btnSort.Click += new System.EventHandler(this.btnSort_Click);
            // 
            // FormResultPropertyHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 311);
            this.Controls.Add(this.btnSort);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormResultPropertyHistory";
            this.Text = "집행 진행상황 이력";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormResultPropertyHistory_FormClosing);
            this.Load += new System.EventHandler(this.FormResultPropertyHistory_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnSort;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBeginTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEndTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProjectCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAcumulatedLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProjectLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAcumulatedArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProjectArea;
        private System.Windows.Forms.DataGridViewComboBoxColumn colStartDirection;
    }
}