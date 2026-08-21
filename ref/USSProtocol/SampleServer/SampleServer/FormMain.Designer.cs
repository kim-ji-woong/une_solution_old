namespace SampleServer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridClients = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSendFire = new System.Windows.Forms.Button();
            this.btnSendPowerOff = new System.Windows.Forms.Button();
            this.btnSendEarthquake = new System.Windows.Forms.Button();
            this.btnSendWind = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridClients)).BeginInit();
            this.SuspendLayout();
            // 
            // gridClients
            // 
            this.gridClients.AllowUserToAddRows = false;
            this.gridClients.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridClients.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colIP});
            this.gridClients.Location = new System.Drawing.Point(12, 12);
            this.gridClients.Name = "gridClients";
            this.gridClients.ReadOnly = true;
            this.gridClients.RowHeadersVisible = false;
            this.gridClients.RowTemplate.Height = 23;
            this.gridClients.Size = new System.Drawing.Size(327, 242);
            this.gridClients.TabIndex = 0;
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
            // colIP
            // 
            this.colIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colIP.DefaultCellStyle = dataGridViewCellStyle3;
            this.colIP.HeaderText = "IP";
            this.colIP.Name = "colIP";
            this.colIP.ReadOnly = true;
            this.colIP.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnSendFire
            // 
            this.btnSendFire.Location = new System.Drawing.Point(366, 43);
            this.btnSendFire.Name = "btnSendFire";
            this.btnSendFire.Size = new System.Drawing.Size(120, 23);
            this.btnSendFire.TabIndex = 1;
            this.btnSendFire.Text = "화재신호 전송";
            this.btnSendFire.UseVisualStyleBackColor = true;
            this.btnSendFire.Click += new System.EventHandler(this.btnSendSignal_Click);
            // 
            // btnSendPowerOff
            // 
            this.btnSendPowerOff.Location = new System.Drawing.Point(366, 72);
            this.btnSendPowerOff.Name = "btnSendPowerOff";
            this.btnSendPowerOff.Size = new System.Drawing.Size(120, 23);
            this.btnSendPowerOff.TabIndex = 1;
            this.btnSendPowerOff.Text = "정전신호 전송";
            this.btnSendPowerOff.UseVisualStyleBackColor = true;
            this.btnSendPowerOff.Click += new System.EventHandler(this.btnSendSignal_Click);
            // 
            // btnSendEarthquake
            // 
            this.btnSendEarthquake.Location = new System.Drawing.Point(366, 101);
            this.btnSendEarthquake.Name = "btnSendEarthquake";
            this.btnSendEarthquake.Size = new System.Drawing.Size(120, 23);
            this.btnSendEarthquake.TabIndex = 1;
            this.btnSendEarthquake.Text = "지진신호 전송";
            this.btnSendEarthquake.UseVisualStyleBackColor = true;
            this.btnSendEarthquake.Click += new System.EventHandler(this.btnSendSignal_Click);
            // 
            // btnSendWind
            // 
            this.btnSendWind.Location = new System.Drawing.Point(366, 130);
            this.btnSendWind.Name = "btnSendWind";
            this.btnSendWind.Size = new System.Drawing.Size(120, 23);
            this.btnSendWind.TabIndex = 1;
            this.btnSendWind.Text = "강풍신호 전송";
            this.btnSendWind.UseVisualStyleBackColor = true;
            this.btnSendWind.Click += new System.EventHandler(this.btnSendSignal_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 291);
            this.Controls.Add(this.btnSendWind);
            this.Controls.Add(this.btnSendEarthquake);
            this.Controls.Add(this.btnSendPowerOff);
            this.Controls.Add(this.btnSendFire);
            this.Controls.Add(this.gridClients);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "Sample Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gridClients)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridClients;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIP;
        private System.Windows.Forms.Button btnSendFire;
        private System.Windows.Forms.Button btnSendPowerOff;
        private System.Windows.Forms.Button btnSendEarthquake;
        private System.Windows.Forms.Button btnSendWind;
    }
}

