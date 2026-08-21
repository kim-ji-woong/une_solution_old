namespace USSFireSensorServer
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelStatusSOP = new System.Windows.Forms.Label();
            this.gridClients = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAddr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioOfficeB = new System.Windows.Forms.RadioButton();
            this.radioOfficeA = new System.Windows.Forms.RadioButton();
            this.btnSendWind = new System.Windows.Forms.Button();
            this.btnSendEarthquake = new System.Windows.Forms.Button();
            this.textBoxWindSpeed = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxIntensity = new System.Windows.Forms.TextBox();
            this.checkBoxStopReadEvent = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridClients)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(28, 38);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(137, 12);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "USS 서버와의 접속 상태";
            // 
            // labelStatusSOP
            // 
            this.labelStatusSOP.AutoSize = true;
            this.labelStatusSOP.Location = new System.Drawing.Point(28, 78);
            this.labelStatusSOP.Name = "labelStatusSOP";
            this.labelStatusSOP.Size = new System.Drawing.Size(138, 12);
            this.labelStatusSOP.TabIndex = 0;
            this.labelStatusSOP.Text = "SOP 서버와의 접속 상태";
            // 
            // gridClients
            // 
            this.gridClients.AllowUserToAddRows = false;
            this.gridClients.AllowUserToDeleteRows = false;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridClients.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.gridClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colAddr,
            this.colType});
            this.gridClients.Location = new System.Drawing.Point(30, 116);
            this.gridClients.Name = "gridClients";
            this.gridClients.ReadOnly = true;
            this.gridClients.RowHeadersVisible = false;
            this.gridClients.RowTemplate.Height = 23;
            this.gridClients.Size = new System.Drawing.Size(304, 264);
            this.gridClients.TabIndex = 1;
            // 
            // colNo
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle14;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 60;
            // 
            // colAddr
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colAddr.DefaultCellStyle = dataGridViewCellStyle15;
            this.colAddr.HeaderText = "접속정보";
            this.colAddr.Name = "colAddr";
            this.colAddr.ReadOnly = true;
            this.colAddr.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colAddr.Width = 120;
            // 
            // colType
            // 
            this.colType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colType.DefaultCellStyle = dataGridViewCellStyle16;
            this.colType.HeaderText = "타입";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "진도 :";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioOfficeB);
            this.groupBox1.Controls.Add(this.radioOfficeA);
            this.groupBox1.Controls.Add(this.btnSendWind);
            this.groupBox1.Controls.Add(this.btnSendEarthquake);
            this.groupBox1.Controls.Add(this.textBoxWindSpeed);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxIntensity);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(381, 116);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 121);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Simulation";
            // 
            // radioOfficeB
            // 
            this.radioOfficeB.AutoSize = true;
            this.radioOfficeB.Location = new System.Drawing.Point(126, 89);
            this.radioOfficeB.Name = "radioOfficeB";
            this.radioOfficeB.Size = new System.Drawing.Size(63, 16);
            this.radioOfficeB.TabIndex = 5;
            this.radioOfficeB.Text = "OfficeB";
            this.radioOfficeB.UseVisualStyleBackColor = true;
            // 
            // radioOfficeA
            // 
            this.radioOfficeA.AutoSize = true;
            this.radioOfficeA.Checked = true;
            this.radioOfficeA.Location = new System.Drawing.Point(57, 89);
            this.radioOfficeA.Name = "radioOfficeA";
            this.radioOfficeA.Size = new System.Drawing.Size(63, 16);
            this.radioOfficeA.TabIndex = 5;
            this.radioOfficeA.TabStop = true;
            this.radioOfficeA.Text = "OfficeA";
            this.radioOfficeA.UseVisualStyleBackColor = true;
            // 
            // btnSendWind
            // 
            this.btnSendWind.Location = new System.Drawing.Point(161, 50);
            this.btnSendWind.Name = "btnSendWind";
            this.btnSendWind.Size = new System.Drawing.Size(51, 23);
            this.btnSendWind.TabIndex = 4;
            this.btnSendWind.Text = "전송";
            this.btnSendWind.UseVisualStyleBackColor = true;
            this.btnSendWind.Click += new System.EventHandler(this.btnSendWind_Click);
            // 
            // btnSendEarthquake
            // 
            this.btnSendEarthquake.Location = new System.Drawing.Point(161, 25);
            this.btnSendEarthquake.Name = "btnSendEarthquake";
            this.btnSendEarthquake.Size = new System.Drawing.Size(51, 23);
            this.btnSendEarthquake.TabIndex = 4;
            this.btnSendEarthquake.Text = "전송";
            this.btnSendEarthquake.UseVisualStyleBackColor = true;
            this.btnSendEarthquake.Click += new System.EventHandler(this.btnSendEarthquake_Click);
            // 
            // textBoxWindSpeed
            // 
            this.textBoxWindSpeed.Location = new System.Drawing.Point(58, 52);
            this.textBoxWindSpeed.Name = "textBoxWindSpeed";
            this.textBoxWindSpeed.Size = new System.Drawing.Size(56, 21);
            this.textBoxWindSpeed.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "m/s";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "풍속 :";
            // 
            // textBoxIntensity
            // 
            this.textBoxIntensity.Location = new System.Drawing.Point(58, 25);
            this.textBoxIntensity.Name = "textBoxIntensity";
            this.textBoxIntensity.Size = new System.Drawing.Size(56, 21);
            this.textBoxIntensity.TabIndex = 3;
            // 
            // checkBoxStopReadEvent
            // 
            this.checkBoxStopReadEvent.AutoSize = true;
            this.checkBoxStopReadEvent.Location = new System.Drawing.Point(381, 52);
            this.checkBoxStopReadEvent.Name = "checkBoxStopReadEvent";
            this.checkBoxStopReadEvent.Size = new System.Drawing.Size(112, 16);
            this.checkBoxStopReadEvent.TabIndex = 4;
            this.checkBoxStopReadEvent.Text = "ReadEvent 중지";
            this.checkBoxStopReadEvent.UseVisualStyleBackColor = true;
            this.checkBoxStopReadEvent.CheckedChanged += new System.EventHandler(this.checkBoxStopReadEvent_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 450);
            this.Controls.Add(this.checkBoxStopReadEvent);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gridClients);
            this.Controls.Add(this.labelStatusSOP);
            this.Controls.Add(this.labelStatus);
            this.Name = "FormMain";
            this.Text = "화재센서 서버";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridClients)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelStatusSOP;
        private System.Windows.Forms.DataGridView gridClients;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAddr;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSendWind;
        private System.Windows.Forms.Button btnSendEarthquake;
        private System.Windows.Forms.TextBox textBoxWindSpeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxIntensity;
        private System.Windows.Forms.RadioButton radioOfficeB;
        private System.Windows.Forms.RadioButton radioOfficeA;
        private System.Windows.Forms.CheckBox checkBoxStopReadEvent;
    }
}

