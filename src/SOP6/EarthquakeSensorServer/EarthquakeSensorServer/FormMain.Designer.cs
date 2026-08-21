namespace EarthquakeSensorServer
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
            this.gridLog = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTimeSpan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMagnitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIntensity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTPGA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSimpleInput = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.labelErrorMessage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxIgnoreMinute = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClearAlarm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridLog)).BeginInit();
            this.SuspendLayout();
            // 
            // gridLog
            // 
            this.gridLog.AllowUserToAddRows = false;
            this.gridLog.AllowUserToDeleteRows = false;
            this.gridLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.colLocation,
            this.colTimeSpan,
            this.colMagnitude,
            this.colIntensity,
            this.colTPGA});
            this.gridLog.Location = new System.Drawing.Point(12, 12);
            this.gridLog.Name = "gridLog";
            this.gridLog.ReadOnly = true;
            this.gridLog.RowHeadersVisible = false;
            this.gridLog.RowTemplate.Height = 23;
            this.gridLog.Size = new System.Drawing.Size(636, 338);
            this.gridLog.TabIndex = 1;
            // 
            // colNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 60;
            // 
            // colLocation
            // 
            this.colLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colLocation.HeaderText = "위치";
            this.colLocation.Name = "colLocation";
            this.colLocation.ReadOnly = true;
            // 
            // colTimeSpan
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTimeSpan.DefaultCellStyle = dataGridViewCellStyle3;
            this.colTimeSpan.HeaderText = "시간";
            this.colTimeSpan.Name = "colTimeSpan";
            this.colTimeSpan.ReadOnly = true;
            this.colTimeSpan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTimeSpan.Width = 200;
            // 
            // colMagnitude
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colMagnitude.DefaultCellStyle = dataGridViewCellStyle4;
            this.colMagnitude.HeaderText = "규모";
            this.colMagnitude.Name = "colMagnitude";
            this.colMagnitude.ReadOnly = true;
            this.colMagnitude.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colMagnitude.Width = 70;
            // 
            // colIntensity
            // 
            this.colIntensity.HeaderText = "진도";
            this.colIntensity.Name = "colIntensity";
            this.colIntensity.ReadOnly = true;
            this.colIntensity.Width = 70;
            // 
            // colTPGA
            // 
            this.colTPGA.HeaderText = "TPGA";
            this.colTPGA.Name = "colTPGA";
            this.colTPGA.ReadOnly = true;
            // 
            // btnSimpleInput
            // 
            this.btnSimpleInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSimpleInput.Location = new System.Drawing.Point(504, 417);
            this.btnSimpleInput.Name = "btnSimpleInput";
            this.btnSimpleInput.Size = new System.Drawing.Size(63, 23);
            this.btnSimpleInput.TabIndex = 2;
            this.btnSimpleInput.Text = "간편입력";
            this.btnSimpleInput.UseVisualStyleBackColor = true;
            this.btnSimpleInput.Click += new System.EventHandler(this.btnSimpleInput_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(573, 418);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labelErrorMessage
            // 
            this.labelErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelErrorMessage.AutoSize = true;
            this.labelErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.labelErrorMessage.Location = new System.Drawing.Point(13, 423);
            this.labelErrorMessage.Name = "labelErrorMessage";
            this.labelErrorMessage.Size = new System.Drawing.Size(69, 12);
            this.labelErrorMessage.TabIndex = 5;
            this.labelErrorMessage.Text = "에러 메시지";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 386);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "지진신호 발생후 ";
            // 
            // textBoxIgnoreMinute
            // 
            this.textBoxIgnoreMinute.Location = new System.Drawing.Point(109, 383);
            this.textBoxIgnoreMinute.Name = "textBoxIgnoreMinute";
            this.textBoxIgnoreMinute.Size = new System.Drawing.Size(34, 21);
            this.textBoxIgnoreMinute.TabIndex = 7;
            this.textBoxIgnoreMinute.Text = "1";
            this.textBoxIgnoreMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxIgnoreMinute.TextChanged += new System.EventHandler(this.textBoxIgnoreMinute_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(148, 386);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(249, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "분 이내에 같거나 작은 신호가 오면 무시한다.";
            // 
            // btnClearAlarm
            // 
            this.btnClearAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearAlarm.Location = new System.Drawing.Point(429, 417);
            this.btnClearAlarm.Name = "btnClearAlarm";
            this.btnClearAlarm.Size = new System.Drawing.Size(69, 23);
            this.btnClearAlarm.TabIndex = 8;
            this.btnClearAlarm.Text = "신호해제";
            this.btnClearAlarm.UseVisualStyleBackColor = true;
            this.btnClearAlarm.Click += new System.EventHandler(this.btnClearAlarm_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 453);
            this.Controls.Add(this.btnClearAlarm);
            this.Controls.Add(this.textBoxIgnoreMinute);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelErrorMessage);
            this.Controls.Add(this.btnSimpleInput);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gridLog);
            this.Name = "FormMain";
            this.Text = "지진센서 서버";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gridLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridLog;
        private System.Windows.Forms.Button btnSimpleInput;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labelErrorMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxIgnoreMinute;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTimeSpan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMagnitude;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIntensity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTPGA;
        private System.Windows.Forms.Button btnClearAlarm;
    }
}

