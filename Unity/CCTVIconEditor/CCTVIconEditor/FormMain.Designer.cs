namespace CCTVIconEditor
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
            this.btnChangeScene = new System.Windows.Forms.Button();
            this.cboScenes = new System.Windows.Forms.ComboBox();
            this.btnRunUnity = new System.Windows.Forms.Button();
            this.gridCCTV = new System.Windows.Forms.DataGridView();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxEdit = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridCCTV)).BeginInit();
            this.SuspendLayout();
            // 
            // btnChangeScene
            // 
            this.btnChangeScene.Location = new System.Drawing.Point(176, 94);
            this.btnChangeScene.Name = "btnChangeScene";
            this.btnChangeScene.Size = new System.Drawing.Size(75, 23);
            this.btnChangeScene.TabIndex = 5;
            this.btnChangeScene.Text = "화면전환";
            this.btnChangeScene.UseVisualStyleBackColor = true;
            this.btnChangeScene.Click += new System.EventHandler(this.btnChangeScene_Click);
            // 
            // cboScenes
            // 
            this.cboScenes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboScenes.FormattingEnabled = true;
            this.cboScenes.Location = new System.Drawing.Point(33, 94);
            this.cboScenes.Name = "cboScenes";
            this.cboScenes.Size = new System.Drawing.Size(121, 20);
            this.cboScenes.TabIndex = 4;
            this.cboScenes.SelectedIndexChanged += new System.EventHandler(this.cboScenes_SelectedIndexChanged);
            // 
            // btnRunUnity
            // 
            this.btnRunUnity.Location = new System.Drawing.Point(33, 34);
            this.btnRunUnity.Name = "btnRunUnity";
            this.btnRunUnity.Size = new System.Drawing.Size(75, 23);
            this.btnRunUnity.TabIndex = 3;
            this.btnRunUnity.Text = "Unity";
            this.btnRunUnity.UseVisualStyleBackColor = true;
            this.btnRunUnity.Click += new System.EventHandler(this.btnRunUnity_Click);
            // 
            // gridCCTV
            // 
            this.gridCCTV.AllowUserToAddRows = false;
            this.gridCCTV.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridCCTV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridCCTV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCCTV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colID,
            this.colName,
            this.colPosition,
            this.colX,
            this.colY,
            this.colZ});
            this.gridCCTV.Location = new System.Drawing.Point(30, 140);
            this.gridCCTV.MultiSelect = false;
            this.gridCCTV.Name = "gridCCTV";
            this.gridCCTV.RowHeadersVisible = false;
            this.gridCCTV.RowTemplate.Height = 23;
            this.gridCCTV.Size = new System.Drawing.Size(758, 298);
            this.gridCCTV.TabIndex = 6;
            this.gridCCTV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCCTV_CellClick);
            // 
            // colID
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colID.DefaultCellStyle = dataGridViewCellStyle2;
            this.colID.HeaderText = "ID";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colID.Width = 60;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colName.DefaultCellStyle = dataGridViewCellStyle3;
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colPosition
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colPosition.DefaultCellStyle = dataGridViewCellStyle4;
            this.colPosition.HeaderText = "위치";
            this.colPosition.Name = "colPosition";
            this.colPosition.ReadOnly = true;
            this.colPosition.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colX
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colX.DefaultCellStyle = dataGridViewCellStyle5;
            this.colX.HeaderText = "X";
            this.colX.Name = "colX";
            this.colX.ReadOnly = true;
            this.colX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colX.Width = 80;
            // 
            // colY
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colY.DefaultCellStyle = dataGridViewCellStyle6;
            this.colY.HeaderText = "Y";
            this.colY.Name = "colY";
            this.colY.ReadOnly = true;
            this.colY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colY.Width = 80;
            // 
            // colZ
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colZ.DefaultCellStyle = dataGridViewCellStyle7;
            this.colZ.HeaderText = "Z";
            this.colZ.Name = "colZ";
            this.colZ.ReadOnly = true;
            this.colZ.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colZ.Width = 80;
            // 
            // checkBoxEdit
            // 
            this.checkBoxEdit.AutoSize = true;
            this.checkBoxEdit.Location = new System.Drawing.Point(417, 96);
            this.checkBoxEdit.Name = "checkBoxEdit";
            this.checkBoxEdit.Size = new System.Drawing.Size(48, 16);
            this.checkBoxEdit.TabIndex = 7;
            this.checkBoxEdit.Text = "편집";
            this.checkBoxEdit.UseVisualStyleBackColor = true;
            this.checkBoxEdit.CheckedChanged += new System.EventHandler(this.checkBoxEdit_CheckedChanged);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(471, 91);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "적용";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.checkBoxEdit);
            this.Controls.Add(this.gridCCTV);
            this.Controls.Add(this.btnChangeScene);
            this.Controls.Add(this.cboScenes);
            this.Controls.Add(this.btnRunUnity);
            this.Name = "FormMain";
            this.Text = "CCTV 편집기";
            ((System.ComponentModel.ISupportInitialize)(this.gridCCTV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnChangeScene;
        private System.Windows.Forms.ComboBox cboScenes;
        private System.Windows.Forms.Button btnRunUnity;
        private System.Windows.Forms.DataGridView gridCCTV;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn colX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colZ;
        private System.Windows.Forms.CheckBox checkBoxEdit;
        private System.Windows.Forms.Button btnSave;
    }
}

