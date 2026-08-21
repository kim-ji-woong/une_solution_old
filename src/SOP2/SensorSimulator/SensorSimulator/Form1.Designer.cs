namespace SensorSimulator
{
    partial class Form1
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colSignal = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEquipType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Zone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSendSignal = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSignal,
            this.colEquipID,
            this.colEquipType,
            this.Zone});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(590, 227);
            this.dataGridView1.TabIndex = 0;
            // 
            // colSignal
            // 
            this.colSignal.HeaderText = "신호";
            this.colSignal.Name = "colSignal";
            this.colSignal.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSignal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colSignal.Width = 60;
            // 
            // colEquipID
            // 
            this.colEquipID.HeaderText = "설비 ID";
            this.colEquipID.Name = "colEquipID";
            // 
            // colEquipType
            // 
            this.colEquipType.HeaderText = "설비 Type";
            this.colEquipType.Name = "colEquipType";
            // 
            // Zone
            // 
            this.Zone.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Zone.HeaderText = "위치";
            this.Zone.Name = "Zone";
            // 
            // btnSendSignal
            // 
            this.btnSendSignal.Location = new System.Drawing.Point(472, 248);
            this.btnSendSignal.Name = "btnSendSignal";
            this.btnSendSignal.Size = new System.Drawing.Size(106, 30);
            this.btnSendSignal.TabIndex = 1;
            this.btnSendSignal.Text = "신호보내기";
            this.btnSendSignal.UseVisualStyleBackColor = true;
            this.btnSendSignal.Click += new System.EventHandler(this.btnSendSignal_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 290);
            this.Controls.Add(this.btnSendSignal);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Sensor Simulator";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSignal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Zone;
        private System.Windows.Forms.Button btnSendSignal;
    }
}

