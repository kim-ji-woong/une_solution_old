namespace JubixSensorSimulator
{
    partial class FormMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridSensor = new System.Windows.Forms.DataGridView();
            this.btnApply = new System.Windows.Forms.Button();
            this.colSensorTagInfoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSSID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSensorLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDensity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelAlarm1 = new System.Windows.Forms.Label();
            this.labelAlarm2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelAlarm3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridSensor)).BeginInit();
            this.SuspendLayout();
            // 
            // gridSensor
            // 
            this.gridSensor.AllowUserToAddRows = false;
            this.gridSensor.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSensor.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridSensor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSensor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSensorTagInfoID,
            this.colSSID,
            this.colSensorLocation,
            this.colDensity,
            this.colStatus});
            this.gridSensor.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridSensor.Location = new System.Drawing.Point(0, 0);
            this.gridSensor.Name = "gridSensor";
            this.gridSensor.RowHeadersVisible = false;
            this.gridSensor.RowTemplate.Height = 23;
            this.gridSensor.Size = new System.Drawing.Size(488, 209);
            this.gridSensor.TabIndex = 0;
            this.gridSensor.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridSensor_CellValueChanged);
            this.gridSensor.SelectionChanged += new System.EventHandler(this.gridSensor_SelectionChanged);
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(415, 215);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(61, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // colSensorTagInfoID
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSensorTagInfoID.DefaultCellStyle = dataGridViewCellStyle2;
            this.colSensorTagInfoID.HeaderText = "ID";
            this.colSensorTagInfoID.Name = "colSensorTagInfoID";
            this.colSensorTagInfoID.ReadOnly = true;
            this.colSensorTagInfoID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colSensorTagInfoID.Width = 40;
            // 
            // colSSID
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSSID.DefaultCellStyle = dataGridViewCellStyle3;
            this.colSSID.HeaderText = "ssID";
            this.colSSID.Name = "colSSID";
            this.colSSID.ReadOnly = true;
            this.colSSID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colSSID.Width = 80;
            // 
            // colSensorLocation
            // 
            this.colSensorLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colSensorLocation.DefaultCellStyle = dataGridViewCellStyle4;
            this.colSensorLocation.HeaderText = "위치";
            this.colSensorLocation.Name = "colSensorLocation";
            this.colSensorLocation.ReadOnly = true;
            this.colSensorLocation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colDensity
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colDensity.DefaultCellStyle = dataGridViewCellStyle5;
            this.colDensity.HeaderText = "농도";
            this.colDensity.Name = "colDensity";
            this.colDensity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDensity.Width = 80;
            // 
            // colStatus
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colStatus.DefaultCellStyle = dataGridViewCellStyle6;
            this.colStatus.HeaderText = "상태";
            this.colStatus.Items.AddRange(new object[] {
            "정상",
            "Alarm1",
            "Alarm2",
            "Alarm3",
            "알람",
            "알람요청",
            "중지요청",
            "CCTV컷",
            "CCTV컷 요청",
            "실패"});
            this.colStatus.Name = "colStatus";
            this.colStatus.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colStatus.Width = 80;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 220);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "1단계 알람 임계치 :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 241);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "2단계 알람 임계치 :";
            // 
            // labelAlarm1
            // 
            this.labelAlarm1.AutoSize = true;
            this.labelAlarm1.Location = new System.Drawing.Point(126, 220);
            this.labelAlarm1.Name = "labelAlarm1";
            this.labelAlarm1.Size = new System.Drawing.Size(0, 12);
            this.labelAlarm1.TabIndex = 3;
            // 
            // labelAlarm2
            // 
            this.labelAlarm2.AutoSize = true;
            this.labelAlarm2.Location = new System.Drawing.Point(126, 241);
            this.labelAlarm2.Name = "labelAlarm2";
            this.labelAlarm2.Size = new System.Drawing.Size(0, 12);
            this.labelAlarm2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(189, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "3단계 알람 임계치 :";
            // 
            // labelAlarm3
            // 
            this.labelAlarm3.AutoSize = true;
            this.labelAlarm3.Location = new System.Drawing.Point(303, 241);
            this.labelAlarm3.Name = "labelAlarm3";
            this.labelAlarm3.Size = new System.Drawing.Size(0, 12);
            this.labelAlarm3.TabIndex = 4;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 261);
            this.Controls.Add(this.labelAlarm3);
            this.Controls.Add(this.labelAlarm2);
            this.Controls.Add(this.labelAlarm1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.gridSensor);
            this.Name = "FormMain";
            this.Text = "센서 시뮬레이터";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridSensor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridSensor;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSensorTagInfoID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSSID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSensorLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDensity;
        private System.Windows.Forms.DataGridViewComboBoxColumn colStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelAlarm1;
        private System.Windows.Forms.Label labelAlarm2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelAlarm3;
    }
}

