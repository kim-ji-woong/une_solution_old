
namespace SampleProject
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
            this.cmbSido = new System.Windows.Forms.ComboBox();
            this.lblSido = new System.Windows.Forms.Label();
            this.chkDong = new System.Windows.Forms.CheckBox();
            this.lblSigungu = new System.Windows.Forms.Label();
            this.cmbSigungu = new System.Windows.Forms.ComboBox();
            this.cmbDong = new System.Windows.Forms.ComboBox();
            this.txtBulidingNum = new System.Windows.Forms.TextBox();
            this.lblMain = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRoadName = new System.Windows.Forms.TextBox();
            this.gridAddress = new System.Windows.Forms.DataGridView();
            this.colAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMenu = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFloorNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUpdateInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblFloors = new System.Windows.Forms.Label();
            this.lblBuildMenu = new System.Windows.Forms.Label();
            this.lblBuildName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbSido
            // 
            this.cmbSido.BackColor = System.Drawing.SystemColors.Window;
            this.cmbSido.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSido.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbSido.FormattingEnabled = true;
            this.cmbSido.Location = new System.Drawing.Point(86, 26);
            this.cmbSido.Name = "cmbSido";
            this.cmbSido.Size = new System.Drawing.Size(139, 20);
            this.cmbSido.TabIndex = 0;
            this.cmbSido.SelectedIndexChanged += new System.EventHandler(this.cmbSido_SelectedIndexChanged);
            // 
            // lblSido
            // 
            this.lblSido.AutoSize = true;
            this.lblSido.BackColor = System.Drawing.Color.Transparent;
            this.lblSido.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSido.ForeColor = System.Drawing.Color.Black;
            this.lblSido.Location = new System.Drawing.Point(40, 30);
            this.lblSido.Name = "lblSido";
            this.lblSido.Size = new System.Drawing.Size(40, 15);
            this.lblSido.TabIndex = 16;
            this.lblSido.Text = "시 도 :";
            this.lblSido.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkDong
            // 
            this.chkDong.AutoSize = true;
            this.chkDong.BackColor = System.Drawing.Color.Transparent;
            this.chkDong.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDong.ForeColor = System.Drawing.Color.Black;
            this.chkDong.Location = new System.Drawing.Point(453, 29);
            this.chkDong.Name = "chkDong";
            this.chkDong.Size = new System.Drawing.Size(68, 19);
            this.chkDong.TabIndex = 38;
            this.chkDong.Text = "읍면동 :";
            this.chkDong.UseVisualStyleBackColor = false;
            this.chkDong.CheckedChanged += new System.EventHandler(this.chkDong_CheckedChanged);
            // 
            // lblSigungu
            // 
            this.lblSigungu.AutoSize = true;
            this.lblSigungu.BackColor = System.Drawing.Color.Transparent;
            this.lblSigungu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSigungu.ForeColor = System.Drawing.Color.Black;
            this.lblSigungu.Location = new System.Drawing.Point(253, 29);
            this.lblSigungu.Name = "lblSigungu";
            this.lblSigungu.Size = new System.Drawing.Size(49, 15);
            this.lblSigungu.TabIndex = 37;
            this.lblSigungu.Text = "시군구 :";
            this.lblSigungu.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbSigungu
            // 
            this.cmbSigungu.BackColor = System.Drawing.SystemColors.Window;
            this.cmbSigungu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSigungu.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbSigungu.FormattingEnabled = true;
            this.cmbSigungu.Location = new System.Drawing.Point(308, 25);
            this.cmbSigungu.Name = "cmbSigungu";
            this.cmbSigungu.Size = new System.Drawing.Size(114, 20);
            this.cmbSigungu.TabIndex = 36;
            this.cmbSigungu.SelectedIndexChanged += new System.EventHandler(this.cmbSigungu_SelectedIndexChanged);
            // 
            // cmbDong
            // 
            this.cmbDong.BackColor = System.Drawing.SystemColors.Window;
            this.cmbDong.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDong.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbDong.FormattingEnabled = true;
            this.cmbDong.Location = new System.Drawing.Point(528, 26);
            this.cmbDong.Name = "cmbDong";
            this.cmbDong.Size = new System.Drawing.Size(140, 20);
            this.cmbDong.TabIndex = 35;
            // 
            // txtBulidingNum
            // 
            this.txtBulidingNum.BackColor = System.Drawing.SystemColors.Window;
            this.txtBulidingNum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBulidingNum.ForeColor = System.Drawing.Color.Black;
            this.txtBulidingNum.ImeMode = System.Windows.Forms.ImeMode.Hangul;
            this.txtBulidingNum.Location = new System.Drawing.Point(308, 59);
            this.txtBulidingNum.Name = "txtBulidingNum";
            this.txtBulidingNum.Size = new System.Drawing.Size(117, 21);
            this.txtBulidingNum.TabIndex = 42;
            // 
            // lblMain
            // 
            this.lblMain.AutoSize = true;
            this.lblMain.BackColor = System.Drawing.Color.Transparent;
            this.lblMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMain.ForeColor = System.Drawing.Color.Black;
            this.lblMain.Location = new System.Drawing.Point(241, 60);
            this.lblMain.Name = "lblMain";
            this.lblMain.Size = new System.Drawing.Size(61, 15);
            this.lblMain.TabIndex = 41;
            this.lblMain.Text = "건물번호 :";
            this.lblMain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(32, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 40;
            this.label1.Text = "도로명 :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtRoadName
            // 
            this.txtRoadName.BackColor = System.Drawing.SystemColors.Window;
            this.txtRoadName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRoadName.ForeColor = System.Drawing.Color.Black;
            this.txtRoadName.ImeMode = System.Windows.Forms.ImeMode.Hangul;
            this.txtRoadName.Location = new System.Drawing.Point(86, 57);
            this.txtRoadName.Name = "txtRoadName";
            this.txtRoadName.Size = new System.Drawing.Size(139, 21);
            this.txtRoadName.TabIndex = 39;
            // 
            // gridAddress
            // 
            this.gridAddress.AllowUserToResizeColumns = false;
            this.gridAddress.AllowUserToResizeRows = false;
            this.gridAddress.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridAddress.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridAddress.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAddress.ColumnHeadersVisible = false;
            this.gridAddress.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAddress,
            this.colMenu,
            this.colName,
            this.colFloorNo,
            this.colUpdateInfo});
            this.gridAddress.Location = new System.Drawing.Point(57, 131);
            this.gridAddress.MultiSelect = false;
            this.gridAddress.Name = "gridAddress";
            this.gridAddress.ReadOnly = true;
            this.gridAddress.RowHeadersVisible = false;
            this.gridAddress.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridAddress.RowTemplate.Height = 23;
            this.gridAddress.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAddress.Size = new System.Drawing.Size(697, 229);
            this.gridAddress.TabIndex = 43;
            this.gridAddress.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridAddress_CellMouseClick);
            // 
            // colAddress
            // 
            this.colAddress.HeaderText = "주소";
            this.colAddress.Name = "colAddress";
            this.colAddress.ReadOnly = true;
            this.colAddress.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colAddress.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colAddress.Width = 190;
            // 
            // colMenu
            // 
            this.colMenu.HeaderText = "건물용도";
            this.colMenu.Name = "colMenu";
            this.colMenu.ReadOnly = true;
            this.colMenu.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colMenu.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colMenu.Width = 120;
            // 
            // colName
            // 
            this.colName.HeaderText = "건물명";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colName.Width = 140;
            // 
            // colFloorNo
            // 
            this.colFloorNo.HeaderText = "층수";
            this.colFloorNo.Name = "colFloorNo";
            this.colFloorNo.ReadOnly = true;
            this.colFloorNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colFloorNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colFloorNo.Width = 50;
            // 
            // colUpdateInfo
            // 
            this.colUpdateInfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colUpdateInfo.HeaderText = "작업일자";
            this.colUpdateInfo.Name = "colUpdateInfo";
            this.colUpdateInfo.ReadOnly = true;
            this.colUpdateInfo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colUpdateInfo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.Control;
            this.btnSearch.Location = new System.Drawing.Point(679, 25);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 44;
            this.btnSearch.Text = "찾기";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.BackColor = System.Drawing.SystemColors.Control;
            this.btnUpload.Location = new System.Drawing.Point(591, 72);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(75, 23);
            this.btnUpload.TabIndex = 46;
            this.btnUpload.Text = "업로드";
            this.btnUpload.UseVisualStyleBackColor = false;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.SystemColors.Control;
            this.btnDownload.Location = new System.Drawing.Point(679, 72);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 47;
            this.btnDownload.Text = "다운로드";
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(364, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 23);
            this.label3.TabIndex = 55;
            this.label3.Text = "건물명";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDate
            // 
            this.lblDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.Color.White;
            this.lblDate.Location = new System.Drawing.Point(554, 108);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(200, 23);
            this.lblDate.TabIndex = 54;
            this.lblDate.Text = "작업일자";
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFloors
            // 
            this.lblFloors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.lblFloors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFloors.ForeColor = System.Drawing.Color.White;
            this.lblFloors.Location = new System.Drawing.Point(504, 108);
            this.lblFloors.Name = "lblFloors";
            this.lblFloors.Size = new System.Drawing.Size(50, 23);
            this.lblFloors.TabIndex = 53;
            this.lblFloors.Text = "층수";
            this.lblFloors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBuildMenu
            // 
            this.lblBuildMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.lblBuildMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBuildMenu.ForeColor = System.Drawing.Color.White;
            this.lblBuildMenu.Location = new System.Drawing.Point(247, 108);
            this.lblBuildMenu.Name = "lblBuildMenu";
            this.lblBuildMenu.Size = new System.Drawing.Size(120, 23);
            this.lblBuildMenu.TabIndex = 52;
            this.lblBuildMenu.Text = "건물용도";
            this.lblBuildMenu.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBuildName
            // 
            this.lblBuildName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.lblBuildName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBuildName.ForeColor = System.Drawing.Color.White;
            this.lblBuildName.Location = new System.Drawing.Point(57, 108);
            this.lblBuildName.Name = "lblBuildName";
            this.lblBuildName.Size = new System.Drawing.Size(190, 23);
            this.lblBuildName.TabIndex = 51;
            this.lblBuildName.Text = "건물목록";
            this.lblBuildName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 385);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.lblFloors);
            this.Controls.Add(this.lblBuildMenu);
            this.Controls.Add(this.lblBuildName);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.gridAddress);
            this.Controls.Add(this.txtBulidingNum);
            this.Controls.Add(this.lblMain);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRoadName);
            this.Controls.Add(this.chkDong);
            this.Controls.Add(this.lblSigungu);
            this.Controls.Add(this.cmbSigungu);
            this.Controls.Add(this.cmbDong);
            this.Controls.Add(this.lblSido);
            this.Controls.Add(this.cmbSido);
            this.Name = "FormMain";
            this.Text = "건물 정보 검색";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbSido;
        private System.Windows.Forms.Label lblSido;
        private System.Windows.Forms.CheckBox chkDong;
        private System.Windows.Forms.Label lblSigungu;
        private System.Windows.Forms.ComboBox cmbSigungu;
        private System.Windows.Forms.ComboBox cmbDong;
        private System.Windows.Forms.TextBox txtBulidingNum;
        private System.Windows.Forms.Label lblMain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRoadName;
        private System.Windows.Forms.DataGridView gridAddress;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblFloors;
        private System.Windows.Forms.Label lblBuildMenu;
        private System.Windows.Forms.Label lblBuildName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMenu;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFloorNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUpdateInfo;
    }
}

