namespace AccessSecurityServer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridLog = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeviceID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colContent1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colContent2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colContent3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colContent4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.btnUpdateLocation = new System.Windows.Forms.Button();
            this.btnTestAlarm = new System.Windows.Forms.Button();
            this.btnClearTestAlarm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridLog)).BeginInit();
            this.SuspendLayout();
            // 
            // gridLog
            // 
            this.gridLog.AllowUserToAddRows = false;
            this.gridLog.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridLog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colTime,
            this.colDeviceID,
            this.colAlarmState,
            this.colLocation,
            this.colContent1,
            this.colContent2,
            this.colContent3,
            this.colContent4});
            this.gridLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridLog.Location = new System.Drawing.Point(0, 0);
            this.gridLog.MultiSelect = false;
            this.gridLog.Name = "gridLog";
            this.gridLog.ReadOnly = true;
            this.gridLog.RowHeadersVisible = false;
            this.gridLog.RowTemplate.Height = 23;
            this.gridLog.Size = new System.Drawing.Size(941, 338);
            this.gridLog.TabIndex = 0;
            // 
            // colNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 40;
            // 
            // colTime
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTime.DefaultCellStyle = dataGridViewCellStyle3;
            this.colTime.HeaderText = "발생시간";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTime.Width = 150;
            // 
            // colDeviceID
            // 
            this.colDeviceID.HeaderText = "DeviceID";
            this.colDeviceID.Name = "colDeviceID";
            this.colDeviceID.ReadOnly = true;
            // 
            // colAlarmState
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colAlarmState.DefaultCellStyle = dataGridViewCellStyle4;
            this.colAlarmState.HeaderText = "타입";
            this.colAlarmState.Name = "colAlarmState";
            this.colAlarmState.ReadOnly = true;
            this.colAlarmState.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colLocation
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colLocation.DefaultCellStyle = dataGridViewCellStyle5;
            this.colLocation.HeaderText = "발생위치";
            this.colLocation.Name = "colLocation";
            this.colLocation.ReadOnly = true;
            this.colLocation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colLocation.Width = 150;
            // 
            // colContent1
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colContent1.DefaultCellStyle = dataGridViewCellStyle6;
            this.colContent1.HeaderText = "부가정보1";
            this.colContent1.Name = "colContent1";
            this.colContent1.ReadOnly = true;
            this.colContent1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colContent2
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colContent2.DefaultCellStyle = dataGridViewCellStyle7;
            this.colContent2.HeaderText = "부가정보2";
            this.colContent2.Name = "colContent2";
            this.colContent2.ReadOnly = true;
            this.colContent2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colContent3
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colContent3.DefaultCellStyle = dataGridViewCellStyle8;
            this.colContent3.HeaderText = "부가정보3";
            this.colContent3.Name = "colContent3";
            this.colContent3.ReadOnly = true;
            this.colContent3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colContent4
            // 
            this.colContent4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colContent4.DefaultCellStyle = dataGridViewCellStyle9;
            this.colContent4.HeaderText = "부가정보4";
            this.colContent4.Name = "colContent4";
            this.colContent4.ReadOnly = true;
            this.colContent4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(869, 344);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(60, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 355);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(53, 12);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "상태정보";
            // 
            // btnUpdateLocation
            // 
            this.btnUpdateLocation.Location = new System.Drawing.Point(768, 344);
            this.btnUpdateLocation.Name = "btnUpdateLocation";
            this.btnUpdateLocation.Size = new System.Drawing.Size(95, 23);
            this.btnUpdateLocation.TabIndex = 1;
            this.btnUpdateLocation.Text = "영역정보 갱신";
            this.btnUpdateLocation.UseVisualStyleBackColor = true;
            this.btnUpdateLocation.Click += new System.EventHandler(this.btnUpdateLocation_Click);
            // 
            // btnTestAlarm
            // 
            this.btnTestAlarm.Location = new System.Drawing.Point(541, 344);
            this.btnTestAlarm.Name = "btnTestAlarm";
            this.btnTestAlarm.Size = new System.Drawing.Size(95, 23);
            this.btnTestAlarm.TabIndex = 1;
            this.btnTestAlarm.Text = "Test Alarm";
            this.btnTestAlarm.UseVisualStyleBackColor = true;
            this.btnTestAlarm.Click += new System.EventHandler(this.btnTestAlarm_Click);
            // 
            // btnClearTestAlarm
            // 
            this.btnClearTestAlarm.Location = new System.Drawing.Point(642, 344);
            this.btnClearTestAlarm.Name = "btnClearTestAlarm";
            this.btnClearTestAlarm.Size = new System.Drawing.Size(120, 23);
            this.btnClearTestAlarm.TabIndex = 1;
            this.btnClearTestAlarm.Text = "선택된 Alarm 해제";
            this.btnClearTestAlarm.UseVisualStyleBackColor = true;
            this.btnClearTestAlarm.Click += new System.EventHandler(this.btnClearTestAlarm_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 376);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnClearTestAlarm);
            this.Controls.Add(this.btnTestAlarm);
            this.Controls.Add(this.btnUpdateLocation);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gridLog);
            this.Name = "FormMain";
            this.Text = "액세스 방범 이벤트 감시";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridLog;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button btnUpdateLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeviceID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmState;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colContent1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colContent2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colContent3;
        private System.Windows.Forms.DataGridViewTextBoxColumn colContent4;
        private System.Windows.Forms.Button btnTestAlarm;
        private System.Windows.Forms.Button btnClearTestAlarm;
    }
}

