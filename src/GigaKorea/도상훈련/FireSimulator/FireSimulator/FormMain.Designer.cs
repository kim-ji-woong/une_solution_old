namespace FireSimulator
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelBuildingName = new System.Windows.Forms.Label();
            this.treeSpace = new System.Windows.Forms.TreeView();
            this.btnFire = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.gridFire = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOpenXML = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelClientCount = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.gridOutbreak = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.treeOutbreak = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnClearOutbreak = new System.Windows.Forms.Button();
            this.btnOutbreak = new System.Windows.Forms.Button();
            this.cmbOutbreak = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnDownXML = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridFire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridOutbreak)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // labelBuildingName
            // 
            this.labelBuildingName.AutoSize = true;
            this.labelBuildingName.Location = new System.Drawing.Point(258, 24);
            this.labelBuildingName.Name = "labelBuildingName";
            this.labelBuildingName.Size = new System.Drawing.Size(53, 12);
            this.labelBuildingName.TabIndex = 0;
            this.labelBuildingName.Text = "건물이름";
            this.labelBuildingName.Visible = false;
            // 
            // treeSpace
            // 
            this.treeSpace.Location = new System.Drawing.Point(12, 104);
            this.treeSpace.Name = "treeSpace";
            this.treeSpace.Size = new System.Drawing.Size(242, 210);
            this.treeSpace.TabIndex = 1;
            this.treeSpace.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSpace_AfterSelect);
            // 
            // btnFire
            // 
            this.btnFire.Enabled = false;
            this.btnFire.Location = new System.Drawing.Point(333, 320);
            this.btnFire.Name = "btnFire";
            this.btnFire.Size = new System.Drawing.Size(67, 26);
            this.btnFire.TabIndex = 2;
            this.btnFire.Text = "화재 발생";
            this.btnFire.UseVisualStyleBackColor = true;
            this.btnFire.Click += new System.EventHandler(this.btnFire_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(406, 320);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(67, 26);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "화재 꺼짐";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // gridFire
            // 
            this.gridFire.AllowUserToAddRows = false;
            this.gridFire.AllowUserToDeleteRows = false;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridFire.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.gridFire.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridFire.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colTime,
            this.colLocation});
            this.gridFire.Location = new System.Drawing.Point(260, 104);
            this.gridFire.MultiSelect = false;
            this.gridFire.Name = "gridFire";
            this.gridFire.ReadOnly = true;
            this.gridFire.RowHeadersVisible = false;
            this.gridFire.RowTemplate.Height = 23;
            this.gridFire.Size = new System.Drawing.Size(213, 210);
            this.gridFire.TabIndex = 3;
            // 
            // colNo
            // 
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle18;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 40;
            // 
            // colTime
            // 
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTime.DefaultCellStyle = dataGridViewCellStyle19;
            this.colTime.HeaderText = "시간";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTime.Width = 60;
            // 
            // colLocation
            // 
            this.colLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.colLocation.DefaultCellStyle = dataGridViewCellStyle20;
            this.colLocation.HeaderText = "장소";
            this.colLocation.Name = "colLocation";
            this.colLocation.ReadOnly = true;
            this.colLocation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnOpenXML
            // 
            this.btnOpenXML.Location = new System.Drawing.Point(113, 17);
            this.btnOpenXML.Name = "btnOpenXML";
            this.btnOpenXML.Size = new System.Drawing.Size(67, 26);
            this.btnOpenXML.TabIndex = 2;
            this.btnOpenXML.Text = "XML 열기";
            this.btnOpenXML.UseVisualStyleBackColor = true;
            this.btnOpenXML.Visible = false;
            this.btnOpenXML.Click += new System.EventHandler(this.btnOpenXML_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(258, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Client 접속 : ";
            // 
            // labelClientCount
            // 
            this.labelClientCount.AutoSize = true;
            this.labelClientCount.Location = new System.Drawing.Point(332, 42);
            this.labelClientCount.Name = "labelClientCount";
            this.labelClientCount.Size = new System.Drawing.Size(11, 12);
            this.labelClientCount.TabIndex = 4;
            this.labelClientCount.Text = "0";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pictureBox1.Location = new System.Drawing.Point(11, 369);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(462, 2);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // gridOutbreak
            // 
            this.gridOutbreak.AllowUserToAddRows = false;
            this.gridOutbreak.AllowUserToDeleteRows = false;
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle21.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle21.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle21.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridOutbreak.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle21;
            this.gridOutbreak.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridOutbreak.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            this.gridOutbreak.Location = new System.Drawing.Point(260, 417);
            this.gridOutbreak.MultiSelect = false;
            this.gridOutbreak.Name = "gridOutbreak";
            this.gridOutbreak.ReadOnly = true;
            this.gridOutbreak.RowHeadersVisible = false;
            this.gridOutbreak.RowTemplate.Height = 23;
            this.gridOutbreak.Size = new System.Drawing.Size(213, 210);
            this.gridOutbreak.TabIndex = 7;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle22;
            this.dataGridViewTextBoxColumn1.HeaderText = "번호";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 40;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle23;
            this.dataGridViewTextBoxColumn2.HeaderText = "시간";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 60;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle24;
            this.dataGridViewTextBoxColumn3.HeaderText = "장소";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // treeOutbreak
            // 
            this.treeOutbreak.Location = new System.Drawing.Point(12, 417);
            this.treeOutbreak.Name = "treeOutbreak";
            this.treeOutbreak.Size = new System.Drawing.Size(242, 210);
            this.treeOutbreak.TabIndex = 6;
            this.treeOutbreak.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeOutbreak_AfterSelect);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 387);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "돌발상황 발생";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pictureBox2.Location = new System.Drawing.Point(11, 66);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(462, 2);
            this.pictureBox2.TabIndex = 9;
            this.pictureBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "화재 발생";
            // 
            // btnClearOutbreak
            // 
            this.btnClearOutbreak.Location = new System.Drawing.Point(406, 633);
            this.btnClearOutbreak.Name = "btnClearOutbreak";
            this.btnClearOutbreak.Size = new System.Drawing.Size(67, 26);
            this.btnClearOutbreak.TabIndex = 11;
            this.btnClearOutbreak.Text = "돌발 꺼짐";
            this.btnClearOutbreak.UseVisualStyleBackColor = true;
            this.btnClearOutbreak.Click += new System.EventHandler(this.btnClearOutbreak_Click);
            // 
            // btnOutbreak
            // 
            this.btnOutbreak.Enabled = false;
            this.btnOutbreak.Location = new System.Drawing.Point(333, 633);
            this.btnOutbreak.Name = "btnOutbreak";
            this.btnOutbreak.Size = new System.Drawing.Size(67, 26);
            this.btnOutbreak.TabIndex = 12;
            this.btnOutbreak.Text = "돌발 발생";
            this.btnOutbreak.UseVisualStyleBackColor = true;
            this.btnOutbreak.Click += new System.EventHandler(this.btnOutbreak_Click);
            // 
            // cmbOutbreak
            // 
            this.cmbOutbreak.BackColor = System.Drawing.Color.White;
            this.cmbOutbreak.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutbreak.FormattingEnabled = true;
            this.cmbOutbreak.Location = new System.Drawing.Point(352, 382);
            this.cmbOutbreak.Name = "cmbOutbreak";
            this.cmbOutbreak.Size = new System.Drawing.Size(121, 20);
            this.cmbOutbreak.TabIndex = 13;
            this.cmbOutbreak.SelectedValueChanged += new System.EventHandler(this.cmbOutbreak_SelectedValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(283, 387);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "상황 선택";
            // 
            // btnDownXML
            // 
            this.btnDownXML.Location = new System.Drawing.Point(11, 17);
            this.btnDownXML.Name = "btnDownXML";
            this.btnDownXML.Size = new System.Drawing.Size(92, 26);
            this.btnDownXML.TabIndex = 15;
            this.btnDownXML.Text = "XML 다운로드";
            this.btnDownXML.UseVisualStyleBackColor = true;
            this.btnDownXML.Click += new System.EventHandler(this.btnDownXML_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(485, 697);
            this.Controls.Add(this.btnDownXML);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbOutbreak);
            this.Controls.Add(this.btnClearOutbreak);
            this.Controls.Add(this.btnOutbreak);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gridOutbreak);
            this.Controls.Add(this.treeOutbreak);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelClientCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gridFire);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnOpenXML);
            this.Controls.Add(this.btnFire);
            this.Controls.Add(this.treeSpace);
            this.Controls.Add(this.labelBuildingName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "가상 도상훈련 사고 발생 시뮬레이터";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridFire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridOutbreak)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelBuildingName;
        private System.Windows.Forms.TreeView treeSpace;
        private System.Windows.Forms.Button btnFire;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DataGridView gridFire;
        private System.Windows.Forms.Button btnOpenXML;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelClientCount;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridView gridOutbreak;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.TreeView treeOutbreak;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnClearOutbreak;
        private System.Windows.Forms.Button btnOutbreak;
        private System.Windows.Forms.ComboBox cmbOutbreak;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDownXML;
    }
}

