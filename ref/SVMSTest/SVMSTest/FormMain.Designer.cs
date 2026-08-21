namespace SVMSTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.gridCameras = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGUID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.textBoxPW = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.axRTSPLiveScreen1 = new AxRTSPLiveScreenLib.AxRTSPLiveScreen();
            this.btnExpand = new System.Windows.Forms.Button();
            this.btnExportCCTVList = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridCameras)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axRTSPLiveScreen1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridCameras
            // 
            this.gridCameras.AllowUserToAddRows = false;
            this.gridCameras.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridCameras.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridCameras.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCameras.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colGUID,
            this.colName});
            this.gridCameras.Location = new System.Drawing.Point(12, 118);
            this.gridCameras.Name = "gridCameras";
            this.gridCameras.ReadOnly = true;
            this.gridCameras.RowHeadersVisible = false;
            this.gridCameras.RowTemplate.Height = 23;
            this.gridCameras.Size = new System.Drawing.Size(391, 259);
            this.gridCameras.TabIndex = 1;
            this.gridCameras.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCameras_CellDoubleClick);
            // 
            // colNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 60;
            // 
            // colGUID
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colGUID.DefaultCellStyle = dataGridViewCellStyle3;
            this.colGUID.HeaderText = "GUID";
            this.colGUID.Name = "colGUID";
            this.colGUID.ReadOnly = true;
            this.colGUID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colName.DefaultCellStyle = dataGridViewCellStyle4;
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnDisconnect);
            this.groupBox3.Controls.Add(this.textBoxPW);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.textBoxID);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.textBoxPort);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.textBoxIP);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.btnConnect);
            this.groupBox3.Location = new System.Drawing.Point(12, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 64);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "SVMS 정보";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(690, 23);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(80, 23);
            this.btnDisconnect.TabIndex = 8;
            this.btnDisconnect.Text = "접속해제";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // textBoxPW
            // 
            this.textBoxPW.Location = new System.Drawing.Point(482, 24);
            this.textBoxPW.Name = "textBoxPW";
            this.textBoxPW.PasswordChar = '*';
            this.textBoxPW.Size = new System.Drawing.Size(104, 21);
            this.textBoxPW.TabIndex = 7;
            this.textBoxPW.Text = "root@Passwd!";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(384, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "User Password";
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(274, 24);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(104, 21);
            this.textBoxID.TabIndex = 5;
            this.textBoxID.Text = "root";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(222, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "User ID";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(171, 24);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(41, 21);
            this.textBoxPort.TabIndex = 3;
            this.textBoxPort.Text = "8020";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(140, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Port";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(27, 24);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(104, 21);
            this.textBoxIP.TabIndex = 1;
            this.textBoxIP.Text = "192.168.0.250";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "IP";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(599, 23);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(80, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "접속";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // axRTSPLiveScreen1
            // 
            this.axRTSPLiveScreen1.Enabled = true;
            this.axRTSPLiveScreen1.Location = new System.Drawing.Point(428, 118);
            this.axRTSPLiveScreen1.Name = "axRTSPLiveScreen1";
            this.axRTSPLiveScreen1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axRTSPLiveScreen1.OcxState")));
            this.axRTSPLiveScreen1.Size = new System.Drawing.Size(360, 259);
            this.axRTSPLiveScreen1.TabIndex = 0;
            // 
            // btnExpand
            // 
            this.btnExpand.Enabled = false;
            this.btnExpand.Location = new System.Drawing.Point(724, 383);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(64, 23);
            this.btnExpand.TabIndex = 25;
            this.btnExpand.Text = "화면확대";
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnExportCCTVList
            // 
            this.btnExportCCTVList.Enabled = false;
            this.btnExportCCTVList.Location = new System.Drawing.Point(236, 383);
            this.btnExportCCTVList.Name = "btnExportCCTVList";
            this.btnExportCCTVList.Size = new System.Drawing.Size(167, 23);
            this.btnExportCCTVList.TabIndex = 25;
            this.btnExportCCTVList.Text = "CCTV 정보 파일로 내보내기";
            this.btnExportCCTVList.UseVisualStyleBackColor = true;
            this.btnExportCCTVList.Click += new System.EventHandler(this.btnExportCCTVList_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 450);
            this.Controls.Add(this.btnExportCCTVList);
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gridCameras);
            this.Controls.Add(this.axRTSPLiveScreen1);
            this.Name = "FormMain";
            this.Text = "SVMS 테스트";
            ((System.ComponentModel.ISupportInitialize)(this.gridCameras)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axRTSPLiveScreen1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxRTSPLiveScreenLib.AxRTSPLiveScreen axRTSPLiveScreen1;
        private System.Windows.Forms.DataGridView gridCameras;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.TextBox textBoxPW;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGUID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.Button btnExpand;
        private System.Windows.Forms.Button btnExportCCTVList;
    }
}

