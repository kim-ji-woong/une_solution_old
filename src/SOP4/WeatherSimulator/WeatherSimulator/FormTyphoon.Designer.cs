namespace WeatherSimulator
{
    partial class FormTyphoon
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCenterLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCenterPressure = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaxSpeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRadius = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWindDirection = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colMoveSpeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEtc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colTime,
            this.colCenterLocation,
            this.colCenterPressure,
            this.colMaxSpeed,
            this.colRadius,
            this.colWindDirection,
            this.colMoveSpeed,
            this.colEtc});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(844, 378);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // colNo
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle1;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 40;
            // 
            // colTime
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.colTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.colTime.HeaderText = "일시";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Width = 150;
            // 
            // colCenterLocation
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCenterLocation.DefaultCellStyle = dataGridViewCellStyle3;
            this.colCenterLocation.HeaderText = "중심위치";
            this.colCenterLocation.Name = "colCenterLocation";
            this.colCenterLocation.ReadOnly = true;
            this.colCenterLocation.Width = 120;
            // 
            // colCenterPressure
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.colCenterPressure.DefaultCellStyle = dataGridViewCellStyle4;
            this.colCenterPressure.HeaderText = "중심기압";
            this.colCenterPressure.Name = "colCenterPressure";
            this.colCenterPressure.ReadOnly = true;
            this.colCenterPressure.Width = 80;
            // 
            // colMaxSpeed
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.colMaxSpeed.DefaultCellStyle = dataGridViewCellStyle5;
            this.colMaxSpeed.HeaderText = "최대풍속";
            this.colMaxSpeed.Name = "colMaxSpeed";
            this.colMaxSpeed.ReadOnly = true;
            this.colMaxSpeed.Width = 80;
            // 
            // colRadius
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.colRadius.DefaultCellStyle = dataGridViewCellStyle6;
            this.colRadius.HeaderText = "강풍반경";
            this.colRadius.Name = "colRadius";
            this.colRadius.ReadOnly = true;
            this.colRadius.Width = 80;
            // 
            // colWindDirection
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.colWindDirection.DefaultCellStyle = dataGridViewCellStyle7;
            this.colWindDirection.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colWindDirection.HeaderText = "진행방향";
            this.colWindDirection.Items.AddRange(new object[] {
            "북",
            "북북동",
            "북동",
            "동북동",
            "동",
            "동남동",
            "남동",
            "남남동",
            "남",
            "남남서",
            "남서",
            "서남서",
            "서",
            "서북서",
            "북서",
            "북북서"});
            this.colWindDirection.Name = "colWindDirection";
            this.colWindDirection.ReadOnly = true;
            this.colWindDirection.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colWindDirection.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colWindDirection.Width = 80;
            // 
            // colMoveSpeed
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.colMoveSpeed.DefaultCellStyle = dataGridViewCellStyle8;
            this.colMoveSpeed.HeaderText = "진행속도";
            this.colMoveSpeed.Name = "colMoveSpeed";
            this.colMoveSpeed.ReadOnly = true;
            this.colMoveSpeed.Width = 90;
            // 
            // colEtc
            // 
            this.colEtc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colEtc.HeaderText = "기타";
            this.colEtc.Name = "colEtc";
            this.colEtc.ReadOnly = true;
            // 
            // FormTyphoon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 378);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormTyphoon";
            this.Text = "FormTyphoon";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCenterLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCenterPressure;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaxSpeed;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRadius;
        private System.Windows.Forms.DataGridViewComboBoxColumn colWindDirection;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMoveSpeed;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEtc;
    }
}