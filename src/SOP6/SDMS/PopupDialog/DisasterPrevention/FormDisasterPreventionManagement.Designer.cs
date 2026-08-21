namespace SDMS.PopupDialog.DisasterPrevention
{
    partial class FormDisasterPreventionManagement
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_edit = new System.Windows.Forms.Button();
            this.button_locationCfg = new System.Windows.Forms.Button();
            this.button_export = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ColNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDevName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColStandardQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColStatusQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCheckCycle = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColCheckWay = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColvalidityDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.button_edit);
            this.panel1.Controls.Add(this.button_locationCfg);
            this.panel1.Controls.Add(this.button_export);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(660, 637);
            this.panel1.TabIndex = 0;
            // 
            // button_edit
            // 
            this.button_edit.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_edit.Location = new System.Drawing.Point(420, 3);
            this.button_edit.Name = "button_edit";
            this.button_edit.Size = new System.Drawing.Size(65, 23);
            this.button_edit.TabIndex = 3;
            this.button_edit.Text = "편집";
            this.button_edit.UseVisualStyleBackColor = true;
            this.button_edit.Click += new System.EventHandler(this.button_edit_Click);
            // 
            // button_locationCfg
            // 
            this.button_locationCfg.Enabled = false;
            this.button_locationCfg.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_locationCfg.Location = new System.Drawing.Point(491, 3);
            this.button_locationCfg.Name = "button_locationCfg";
            this.button_locationCfg.Size = new System.Drawing.Size(79, 23);
            this.button_locationCfg.TabIndex = 2;
            this.button_locationCfg.Text = "위치설정";
            this.button_locationCfg.UseVisualStyleBackColor = true;
            this.button_locationCfg.Click += new System.EventHandler(this.button_locationCfg_Click);
            // 
            // button_export
            // 
            this.button_export.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_export.Location = new System.Drawing.Point(576, 3);
            this.button_export.Name = "button_export";
            this.button_export.Size = new System.Drawing.Size(81, 23);
            this.button_export.TabIndex = 1;
            this.button_export.Text = "내보내기";
            this.button_export.UseVisualStyleBackColor = true;
            this.button_export.Click += new System.EventHandler(this.button_export_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeight = 35;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColNum,
            this.ColDevName,
            this.ColLocation,
            this.ColStandardQuantity,
            this.ColStatusQuantity,
            this.ColCheckCycle,
            this.ColCheckWay,
            this.ColvalidityDate});
            this.dataGridView1.Location = new System.Drawing.Point(3, 32);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(654, 602);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellLeave);
            this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            // 
            // ColNum
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColNum.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColNum.HeaderText = "No";
            this.ColNum.Name = "ColNum";
            this.ColNum.ReadOnly = true;
            this.ColNum.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColNum.Width = 35;
            // 
            // ColDevName
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColDevName.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColDevName.HeaderText = "장비명";
            this.ColDevName.Name = "ColDevName";
            this.ColDevName.ReadOnly = true;
            this.ColDevName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColLocation
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColLocation.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColLocation.HeaderText = "위치";
            this.ColLocation.Name = "ColLocation";
            this.ColLocation.ReadOnly = true;
            this.ColLocation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColLocation.Width = 106;
            // 
            // ColStandardQuantity
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColStandardQuantity.DefaultCellStyle = dataGridViewCellStyle5;
            this.ColStandardQuantity.HeaderText = "기준";
            this.ColStandardQuantity.Name = "ColStandardQuantity";
            this.ColStandardQuantity.ReadOnly = true;
            this.ColStandardQuantity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColStandardQuantity.Width = 55;
            // 
            // ColStatusQuantity
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColStatusQuantity.DefaultCellStyle = dataGridViewCellStyle6;
            this.ColStatusQuantity.HeaderText = "현황";
            this.ColStatusQuantity.Name = "ColStatusQuantity";
            this.ColStatusQuantity.ReadOnly = true;
            this.ColStatusQuantity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColStatusQuantity.Width = 55;
            // 
            // ColCheckCycle
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColCheckCycle.DefaultCellStyle = dataGridViewCellStyle7;
            this.ColCheckCycle.HeaderText = "점검주기";
            this.ColCheckCycle.Name = "ColCheckCycle";
            this.ColCheckCycle.ReadOnly = true;
            this.ColCheckCycle.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColCheckWay
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColCheckWay.DefaultCellStyle = dataGridViewCellStyle8;
            this.ColCheckWay.HeaderText = "점검방법";
            this.ColCheckWay.Name = "ColCheckWay";
            this.ColCheckWay.ReadOnly = true;
            // 
            // ColvalidityDate
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColvalidityDate.DefaultCellStyle = dataGridViewCellStyle9;
            this.ColvalidityDate.HeaderText = "유효기간";
            this.ColvalidityDate.Name = "ColvalidityDate";
            this.ColvalidityDate.ReadOnly = true;
            this.ColvalidityDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FormDisasterPreventionManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 662);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDisasterPreventionManagement";
            this.Text = "FormDisasterPreventionManagement";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button_export;
        private System.Windows.Forms.Button button_edit;
        private System.Windows.Forms.Button button_locationCfg;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColNum;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColDevName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStandardQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStatusQuantity;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColCheckCycle;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColCheckWay;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColvalidityDate;
    }
}