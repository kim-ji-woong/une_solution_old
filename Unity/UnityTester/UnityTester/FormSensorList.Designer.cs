namespace UnityTester
{
    partial class FormSensorList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridSensors = new System.Windows.Forms.DataGridView();
            this.colTab = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRelay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSensorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBoxAlarmZones = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnSaveDB = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridSensors)).BeginInit();
            this.SuspendLayout();
            // 
            // gridSensors
            // 
            this.gridSensors.AllowUserToAddRows = false;
            this.gridSensors.AllowUserToDeleteRows = false;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSensors.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.gridSensors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSensors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTab,
            this.colRelay,
            this.colSensorName});
            this.gridSensors.Location = new System.Drawing.Point(12, 12);
            this.gridSensors.MultiSelect = false;
            this.gridSensors.Name = "gridSensors";
            this.gridSensors.ReadOnly = true;
            this.gridSensors.RowHeadersVisible = false;
            this.gridSensors.RowTemplate.Height = 23;
            this.gridSensors.Size = new System.Drawing.Size(494, 320);
            this.gridSensors.TabIndex = 0;
            this.gridSensors.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridSensors_CellClick);
            // 
            // colTab
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTab.DefaultCellStyle = dataGridViewCellStyle10;
            this.colTab.HeaderText = "탭";
            this.colTab.Name = "colTab";
            this.colTab.ReadOnly = true;
            this.colTab.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colRelay
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colRelay.DefaultCellStyle = dataGridViewCellStyle11;
            this.colRelay.HeaderText = "중계기";
            this.colRelay.Name = "colRelay";
            this.colRelay.ReadOnly = true;
            this.colRelay.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colSensorName
            // 
            this.colSensorName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colSensorName.DefaultCellStyle = dataGridViewCellStyle12;
            this.colSensorName.HeaderText = "센서 이름";
            this.colSensorName.Name = "colSensorName";
            this.colSensorName.ReadOnly = true;
            this.colSensorName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // textBoxAlarmZones
            // 
            this.textBoxAlarmZones.Location = new System.Drawing.Point(12, 357);
            this.textBoxAlarmZones.Multiline = true;
            this.textBoxAlarmZones.Name = "textBoxAlarmZones";
            this.textBoxAlarmZones.Size = new System.Drawing.Size(381, 97);
            this.textBoxAlarmZones.TabIndex = 1;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(399, 357);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(107, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnSaveDB
            // 
            this.btnSaveDB.Location = new System.Drawing.Point(399, 394);
            this.btnSaveDB.Name = "btnSaveDB";
            this.btnSaveDB.Size = new System.Drawing.Size(107, 23);
            this.btnSaveDB.TabIndex = 2;
            this.btnSaveDB.Text = "DB에 저장";
            this.btnSaveDB.UseVisualStyleBackColor = true;
            this.btnSaveDB.Click += new System.EventHandler(this.btnSaveDB_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(399, 431);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(107, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "종료";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormSensorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 466);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSaveDB);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.textBoxAlarmZones);
            this.Controls.Add(this.gridSensors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormSensorList";
            this.Text = "FormSensorList";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSensorList_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gridSensors)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridSensors;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTab;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRelay;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSensorName;
        private System.Windows.Forms.TextBox textBoxAlarmZones;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnSaveDB;
        private System.Windows.Forms.Button btnClose;
    }
}