namespace CCTVAlarmWatcher
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridAlarmLog = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEventTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTVID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTVName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelCCTV1 = new CCTVAlarmWatcher.PanelCCTV();
            this.panelCCTV2 = new CCTVAlarmWatcher.PanelCCTV();
            this.panelCCTV3 = new CCTVAlarmWatcher.PanelCCTV();
            this.panelCCTV4 = new CCTVAlarmWatcher.PanelCCTV();
            this.panelCCTV5 = new CCTVAlarmWatcher.PanelCCTV();
            this.panelCCTV6 = new CCTVAlarmWatcher.PanelCCTV();
            this.panelCCTV7 = new CCTVAlarmWatcher.PanelCCTV();
            this.panelCCTV8 = new CCTVAlarmWatcher.PanelCCTV();
            this.btnAlarm = new System.Windows.Forms.Button();
            this.btnAlarmClear = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboCCTVIndex = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarmLog)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridAlarmLog
            // 
            this.gridAlarmLog.AllowUserToAddRows = false;
            this.gridAlarmLog.AllowUserToDeleteRows = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridAlarmLog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.gridAlarmLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAlarmLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colEventTime,
            this.colCCTVID,
            this.colCCTVName});
            this.gridAlarmLog.Location = new System.Drawing.Point(0, 508);
            this.gridAlarmLog.Name = "gridAlarmLog";
            this.gridAlarmLog.RowHeadersVisible = false;
            this.gridAlarmLog.RowTemplate.Height = 23;
            this.gridAlarmLog.Size = new System.Drawing.Size(602, 227);
            this.gridAlarmLog.TabIndex = 1;
            // 
            // colNo
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle7;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.Width = 80;
            // 
            // colEventTime
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colEventTime.DefaultCellStyle = dataGridViewCellStyle8;
            this.colEventTime.HeaderText = "발생시간";
            this.colEventTime.Name = "colEventTime";
            this.colEventTime.Width = 200;
            // 
            // colCCTVID
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTVID.DefaultCellStyle = dataGridViewCellStyle9;
            this.colCCTVID.HeaderText = "CCTV";
            this.colCCTVID.Name = "colCCTVID";
            // 
            // colCCTVName
            // 
            this.colCCTVName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colCCTVName.DefaultCellStyle = dataGridViewCellStyle10;
            this.colCCTVName.HeaderText = "위치";
            this.colCCTVName.Name = "colCCTVName";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1067, 705);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelCCTV1
            // 
            this.panelCCTV1.BackColor = System.Drawing.Color.Black;
            this.panelCCTV1.Location = new System.Drawing.Point(0, 0);
            this.panelCCTV1.Name = "panelCCTV1";
            this.panelCCTV1.Size = new System.Drawing.Size(284, 248);
            this.panelCCTV1.TabIndex = 3;
            // 
            // panelCCTV2
            // 
            this.panelCCTV2.BackColor = System.Drawing.Color.Black;
            this.panelCCTV2.Location = new System.Drawing.Point(290, 0);
            this.panelCCTV2.Name = "panelCCTV2";
            this.panelCCTV2.Size = new System.Drawing.Size(284, 248);
            this.panelCCTV2.TabIndex = 3;
            // 
            // panelCCTV3
            // 
            this.panelCCTV3.BackColor = System.Drawing.Color.Black;
            this.panelCCTV3.Location = new System.Drawing.Point(580, 0);
            this.panelCCTV3.Name = "panelCCTV3";
            this.panelCCTV3.Size = new System.Drawing.Size(284, 248);
            this.panelCCTV3.TabIndex = 3;
            // 
            // panelCCTV4
            // 
            this.panelCCTV4.BackColor = System.Drawing.Color.Black;
            this.panelCCTV4.Location = new System.Drawing.Point(870, 0);
            this.panelCCTV4.Name = "panelCCTV4";
            this.panelCCTV4.Size = new System.Drawing.Size(284, 248);
            this.panelCCTV4.TabIndex = 3;
            // 
            // panelCCTV5
            // 
            this.panelCCTV5.BackColor = System.Drawing.Color.Black;
            this.panelCCTV5.Location = new System.Drawing.Point(0, 254);
            this.panelCCTV5.Name = "panelCCTV5";
            this.panelCCTV5.Size = new System.Drawing.Size(284, 248);
            this.panelCCTV5.TabIndex = 3;
            // 
            // panelCCTV6
            // 
            this.panelCCTV6.BackColor = System.Drawing.Color.Black;
            this.panelCCTV6.Location = new System.Drawing.Point(290, 254);
            this.panelCCTV6.Name = "panelCCTV6";
            this.panelCCTV6.Size = new System.Drawing.Size(284, 248);
            this.panelCCTV6.TabIndex = 3;
            // 
            // panelCCTV7
            // 
            this.panelCCTV7.BackColor = System.Drawing.Color.Black;
            this.panelCCTV7.Location = new System.Drawing.Point(580, 254);
            this.panelCCTV7.Name = "panelCCTV7";
            this.panelCCTV7.Size = new System.Drawing.Size(284, 248);
            this.panelCCTV7.TabIndex = 3;
            // 
            // panelCCTV8
            // 
            this.panelCCTV8.BackColor = System.Drawing.Color.Black;
            this.panelCCTV8.Location = new System.Drawing.Point(870, 254);
            this.panelCCTV8.Name = "panelCCTV8";
            this.panelCCTV8.Size = new System.Drawing.Size(284, 248);
            this.panelCCTV8.TabIndex = 3;
            // 
            // btnAlarm
            // 
            this.btnAlarm.Location = new System.Drawing.Point(119, 20);
            this.btnAlarm.Name = "btnAlarm";
            this.btnAlarm.Size = new System.Drawing.Size(75, 23);
            this.btnAlarm.TabIndex = 4;
            this.btnAlarm.Text = "알람전송";
            this.btnAlarm.UseVisualStyleBackColor = true;
            this.btnAlarm.Click += new System.EventHandler(this.btnAlarm_Click);
            // 
            // btnAlarmClear
            // 
            this.btnAlarmClear.Location = new System.Drawing.Point(119, 49);
            this.btnAlarmClear.Name = "btnAlarmClear";
            this.btnAlarmClear.Size = new System.Drawing.Size(75, 23);
            this.btnAlarmClear.TabIndex = 4;
            this.btnAlarmClear.Text = "알람해제";
            this.btnAlarmClear.UseVisualStyleBackColor = true;
            this.btnAlarmClear.Click += new System.EventHandler(this.btnAlarmClear_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboCCTVIndex);
            this.groupBox1.Controls.Add(this.btnAlarmClear);
            this.groupBox1.Controls.Add(this.btnAlarm);
            this.groupBox1.Location = new System.Drawing.Point(942, 508);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 82);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Simulation";
            // 
            // cboCCTVIndex
            // 
            this.cboCCTVIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCCTVIndex.FormattingEnabled = true;
            this.cboCCTVIndex.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.cboCCTVIndex.Location = new System.Drawing.Point(6, 20);
            this.cboCCTVIndex.Name = "cboCCTVIndex";
            this.cboCCTVIndex.Size = new System.Drawing.Size(69, 20);
            this.cboCCTVIndex.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 740);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelCCTV4);
            this.Controls.Add(this.panelCCTV3);
            this.Controls.Add(this.panelCCTV2);
            this.Controls.Add(this.panelCCTV8);
            this.Controls.Add(this.panelCCTV7);
            this.Controls.Add(this.panelCCTV6);
            this.Controls.Add(this.panelCCTV5);
            this.Controls.Add(this.panelCCTV1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gridAlarmLog);
            this.Name = "FormMain";
            this.Text = "외부비상벨 알람 감시";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarmLog)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridAlarmLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEventTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTVID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTVName;
        private System.Windows.Forms.Button btnClose;
        private PanelCCTV panelCCTV1;
        private PanelCCTV panelCCTV2;
        private PanelCCTV panelCCTV3;
        private PanelCCTV panelCCTV4;
        private PanelCCTV panelCCTV5;
        private PanelCCTV panelCCTV6;
        private PanelCCTV panelCCTV7;
        private PanelCCTV panelCCTV8;
        private System.Windows.Forms.Button btnAlarm;
        private System.Windows.Forms.Button btnAlarmClear;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboCCTVIndex;
    }
}

