namespace EquipZoneCCTV
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridEquipZoneCCTV = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEquipZoneID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEquipZoneName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cboBuildings = new System.Windows.Forms.ComboBox();
            this.cboZones = new System.Windows.Forms.ComboBox();
            this.btnApplyDB = new System.Windows.Forms.Button();
            this.btnCCTVURL = new System.Windows.Forms.Button();
            this.btnDBBackup = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridEquipZoneCCTV)).BeginInit();
            this.SuspendLayout();
            // 
            // gridEquipZoneCCTV
            // 
            this.gridEquipZoneCCTV.AllowUserToAddRows = false;
            this.gridEquipZoneCCTV.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridEquipZoneCCTV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridEquipZoneCCTV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridEquipZoneCCTV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colEquipZoneID,
            this.colEquipZoneName,
            this.colCCTV1,
            this.colCCTV2,
            this.colCCTV3,
            this.colCCTV4,
            this.colCCTV5,
            this.colCCTV6});
            this.gridEquipZoneCCTV.Location = new System.Drawing.Point(30, 84);
            this.gridEquipZoneCCTV.Name = "gridEquipZoneCCTV";
            this.gridEquipZoneCCTV.RowHeadersVisible = false;
            this.gridEquipZoneCCTV.RowTemplate.Height = 23;
            this.gridEquipZoneCCTV.Size = new System.Drawing.Size(847, 586);
            this.gridEquipZoneCCTV.TabIndex = 0;
            // 
            // colNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 80;
            // 
            // colEquipZoneID
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colEquipZoneID.DefaultCellStyle = dataGridViewCellStyle3;
            this.colEquipZoneID.HeaderText = "영역ID";
            this.colEquipZoneID.Name = "colEquipZoneID";
            this.colEquipZoneID.ReadOnly = true;
            this.colEquipZoneID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colEquipZoneID.Width = 80;
            // 
            // colEquipZoneName
            // 
            this.colEquipZoneName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colEquipZoneName.HeaderText = "영역이름";
            this.colEquipZoneName.Name = "colEquipZoneName";
            this.colEquipZoneName.ReadOnly = true;
            this.colEquipZoneName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCCTV1
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV1.DefaultCellStyle = dataGridViewCellStyle4;
            this.colCCTV1.HeaderText = "CCTV1";
            this.colCCTV1.Name = "colCCTV1";
            this.colCCTV1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCCTV1.Width = 80;
            // 
            // colCCTV2
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV2.DefaultCellStyle = dataGridViewCellStyle5;
            this.colCCTV2.HeaderText = "CCTV2";
            this.colCCTV2.Name = "colCCTV2";
            this.colCCTV2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCCTV2.Width = 80;
            // 
            // colCCTV3
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV3.DefaultCellStyle = dataGridViewCellStyle6;
            this.colCCTV3.HeaderText = "CCTV3";
            this.colCCTV3.Name = "colCCTV3";
            this.colCCTV3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCCTV3.Width = 80;
            // 
            // colCCTV4
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV4.DefaultCellStyle = dataGridViewCellStyle7;
            this.colCCTV4.HeaderText = "CCTV4";
            this.colCCTV4.Name = "colCCTV4";
            this.colCCTV4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCCTV4.Width = 80;
            // 
            // colCCTV5
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV5.DefaultCellStyle = dataGridViewCellStyle8;
            this.colCCTV5.HeaderText = "CCTV5";
            this.colCCTV5.Name = "colCCTV5";
            // 
            // colCCTV6
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV6.DefaultCellStyle = dataGridViewCellStyle9;
            this.colCCTV6.HeaderText = "CCTV6";
            this.colCCTV6.Name = "colCCTV6";
            // 
            // cboBuildings
            // 
            this.cboBuildings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildings.FormattingEnabled = true;
            this.cboBuildings.Location = new System.Drawing.Point(30, 33);
            this.cboBuildings.Name = "cboBuildings";
            this.cboBuildings.Size = new System.Drawing.Size(121, 20);
            this.cboBuildings.TabIndex = 1;
            this.cboBuildings.SelectedIndexChanged += new System.EventHandler(this.cboBuildings_SelectedIndexChanged);
            // 
            // cboZones
            // 
            this.cboZones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboZones.FormattingEnabled = true;
            this.cboZones.Location = new System.Drawing.Point(183, 33);
            this.cboZones.Name = "cboZones";
            this.cboZones.Size = new System.Drawing.Size(121, 20);
            this.cboZones.TabIndex = 1;
            this.cboZones.SelectedIndexChanged += new System.EventHandler(this.cboZones_SelectedIndexChanged);
            // 
            // btnApplyDB
            // 
            this.btnApplyDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyDB.Location = new System.Drawing.Point(802, 33);
            this.btnApplyDB.Name = "btnApplyDB";
            this.btnApplyDB.Size = new System.Drawing.Size(75, 23);
            this.btnApplyDB.TabIndex = 2;
            this.btnApplyDB.Text = "DB에 적용";
            this.btnApplyDB.UseVisualStyleBackColor = true;
            this.btnApplyDB.Click += new System.EventHandler(this.btnApplyDB_Click);
            // 
            // btnCCTVURL
            // 
            this.btnCCTVURL.Location = new System.Drawing.Point(324, 33);
            this.btnCCTVURL.Name = "btnCCTVURL";
            this.btnCCTVURL.Size = new System.Drawing.Size(75, 23);
            this.btnCCTVURL.TabIndex = 3;
            this.btnCCTVURL.Text = "CCTV URL";
            this.btnCCTVURL.UseVisualStyleBackColor = true;
            this.btnCCTVURL.Click += new System.EventHandler(this.btnCCTVURL_Click);
            // 
            // btnDBBackup
            // 
            this.btnDBBackup.Location = new System.Drawing.Point(721, 33);
            this.btnDBBackup.Name = "btnDBBackup";
            this.btnDBBackup.Size = new System.Drawing.Size(75, 23);
            this.btnDBBackup.TabIndex = 4;
            this.btnDBBackup.Text = "DB 백업";
            this.btnDBBackup.UseVisualStyleBackColor = true;
            this.btnDBBackup.Click += new System.EventHandler(this.btnDBBackup_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 684);
            this.Controls.Add(this.btnDBBackup);
            this.Controls.Add(this.btnCCTVURL);
            this.Controls.Add(this.btnApplyDB);
            this.Controls.Add(this.cboZones);
            this.Controls.Add(this.cboBuildings);
            this.Controls.Add(this.gridEquipZoneCCTV);
            this.Name = "FormMain";
            this.Text = "화재영역별 CCTV 설정";
            ((System.ComponentModel.ISupportInitialize)(this.gridEquipZoneCCTV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridEquipZoneCCTV;
        private System.Windows.Forms.ComboBox cboBuildings;
        private System.Windows.Forms.ComboBox cboZones;
        private System.Windows.Forms.Button btnApplyDB;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipZoneID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipZoneName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV3;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV4;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV5;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV6;
        private System.Windows.Forms.Button btnCCTVURL;
        private System.Windows.Forms.Button btnDBBackup;
    }
}

