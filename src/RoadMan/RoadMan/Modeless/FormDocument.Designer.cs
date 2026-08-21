namespace RoadMan
{
    partial class FormDocument
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewDate = new System.Windows.Forms.DataGridView();
            this.columnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewDecision = new System.Windows.Forms.DataGridView();
            this.dataGridViewResult = new System.Windows.Forms.DataGridView();
            this.dataGridViewArea = new System.Windows.Forms.DataGridView();
            this.dataGridViewIncomplete = new System.Windows.Forms.DataGridView();
            this.dataGridViewType = new System.Windows.Forms.DataGridView();
            this.dataGridViewOwner = new System.Windows.Forms.DataGridView();
            this.columnDecision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnIncomplete = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTypeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLastDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFirstDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colInsertArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComplete = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIncomplete = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPartialComplete = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRiceField = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colField = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colETC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNational = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPublic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrivate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAvgCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConstCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOutlineCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxComplete = new System.Windows.Forms.CheckBox();
            this.checkBoxIncomplete = new System.Windows.Forms.CheckBox();
            this.checkBoxPartialComplete = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDecision)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewIncomplete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwner)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colCity,
            this.colType,
            this.colSubType,
            this.colTypeName,
            this.colStatus,
            this.colLastDate,
            this.colFirstDate,
            this.colArea,
            this.colInsertArea,
            this.colComplete,
            this.colIncomplete,
            this.colPartialComplete,
            this.colRiceField,
            this.colField,
            this.colLand,
            this.colETC,
            this.colNational,
            this.colPublic,
            this.colPrivate,
            this.colAvgCost,
            this.colConstCost,
            this.colOutlineCost});
            this.dataGridView1.Location = new System.Drawing.Point(0, 34);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(746, 413);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridView1_ColumnWidthChanged);
            this.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView1_Scroll);
            this.dataGridView1.Resize += new System.EventHandler(this.dataGridView1_Resize);
            // 
            // dataGridViewDate
            // 
            this.dataGridViewDate.AllowUserToAddRows = false;
            this.dataGridViewDate.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewDate.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnDate});
            this.dataGridViewDate.Enabled = false;
            this.dataGridViewDate.Location = new System.Drawing.Point(39, 106);
            this.dataGridViewDate.Name = "dataGridViewDate";
            this.dataGridViewDate.RowHeadersVisible = false;
            this.dataGridViewDate.RowTemplate.Height = 23;
            this.dataGridViewDate.Size = new System.Drawing.Size(104, 57);
            this.dataGridViewDate.TabIndex = 1;
            // 
            // columnDate
            // 
            this.columnDate.HeaderText = "";
            this.columnDate.Name = "columnDate";
            // 
            // dataGridViewDecision
            // 
            this.dataGridViewDecision.AllowUserToAddRows = false;
            this.dataGridViewDecision.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDecision.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnDecision});
            this.dataGridViewDecision.Enabled = false;
            this.dataGridViewDecision.Location = new System.Drawing.Point(149, 106);
            this.dataGridViewDecision.Name = "dataGridViewDecision";
            this.dataGridViewDecision.RowHeadersVisible = false;
            this.dataGridViewDecision.RowTemplate.Height = 23;
            this.dataGridViewDecision.Size = new System.Drawing.Size(104, 57);
            this.dataGridViewDecision.TabIndex = 1;
            // 
            // dataGridViewResult
            // 
            this.dataGridViewResult.AllowUserToAddRows = false;
            this.dataGridViewResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnResult});
            this.dataGridViewResult.Enabled = false;
            this.dataGridViewResult.Location = new System.Drawing.Point(259, 106);
            this.dataGridViewResult.Name = "dataGridViewResult";
            this.dataGridViewResult.RowHeadersVisible = false;
            this.dataGridViewResult.RowTemplate.Height = 23;
            this.dataGridViewResult.Size = new System.Drawing.Size(104, 57);
            this.dataGridViewResult.TabIndex = 1;
            // 
            // dataGridViewArea
            // 
            this.dataGridViewArea.AllowUserToAddRows = false;
            this.dataGridViewArea.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewArea.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnArea});
            this.dataGridViewArea.Enabled = false;
            this.dataGridViewArea.Location = new System.Drawing.Point(369, 106);
            this.dataGridViewArea.Name = "dataGridViewArea";
            this.dataGridViewArea.RowHeadersVisible = false;
            this.dataGridViewArea.RowTemplate.Height = 23;
            this.dataGridViewArea.Size = new System.Drawing.Size(104, 57);
            this.dataGridViewArea.TabIndex = 1;
            // 
            // dataGridViewIncomplete
            // 
            this.dataGridViewIncomplete.AllowUserToAddRows = false;
            this.dataGridViewIncomplete.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewIncomplete.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnIncomplete});
            this.dataGridViewIncomplete.Enabled = false;
            this.dataGridViewIncomplete.Location = new System.Drawing.Point(479, 106);
            this.dataGridViewIncomplete.Name = "dataGridViewIncomplete";
            this.dataGridViewIncomplete.RowHeadersVisible = false;
            this.dataGridViewIncomplete.RowTemplate.Height = 23;
            this.dataGridViewIncomplete.Size = new System.Drawing.Size(104, 57);
            this.dataGridViewIncomplete.TabIndex = 1;
            // 
            // dataGridViewType
            // 
            this.dataGridViewType.AllowUserToAddRows = false;
            this.dataGridViewType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewType.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnType});
            this.dataGridViewType.Enabled = false;
            this.dataGridViewType.Location = new System.Drawing.Point(39, 181);
            this.dataGridViewType.Name = "dataGridViewType";
            this.dataGridViewType.RowHeadersVisible = false;
            this.dataGridViewType.RowTemplate.Height = 23;
            this.dataGridViewType.Size = new System.Drawing.Size(104, 57);
            this.dataGridViewType.TabIndex = 1;
            // 
            // dataGridViewOwner
            // 
            this.dataGridViewOwner.AllowUserToAddRows = false;
            this.dataGridViewOwner.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOwner.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnOwner});
            this.dataGridViewOwner.Enabled = false;
            this.dataGridViewOwner.Location = new System.Drawing.Point(149, 181);
            this.dataGridViewOwner.Name = "dataGridViewOwner";
            this.dataGridViewOwner.RowHeadersVisible = false;
            this.dataGridViewOwner.RowTemplate.Height = 23;
            this.dataGridViewOwner.Size = new System.Drawing.Size(104, 57);
            this.dataGridViewOwner.TabIndex = 1;
            // 
            // columnDecision
            // 
            this.columnDecision.HeaderText = "";
            this.columnDecision.Name = "columnDecision";
            // 
            // columnResult
            // 
            this.columnResult.HeaderText = "";
            this.columnResult.Name = "columnResult";
            // 
            // columnArea
            // 
            this.columnArea.HeaderText = "";
            this.columnArea.Name = "columnArea";
            // 
            // columnIncomplete
            // 
            this.columnIncomplete.HeaderText = "";
            this.columnIncomplete.Name = "columnIncomplete";
            // 
            // columnType
            // 
            this.columnType.HeaderText = "";
            this.columnType.Name = "columnType";
            // 
            // columnOwner
            // 
            this.columnOwner.HeaderText = "";
            this.columnOwner.Name = "columnOwner";
            // 
            // colNo
            // 
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.Width = 30;
            // 
            // colCity
            // 
            this.colCity.FillWeight = 282.638F;
            this.colCity.HeaderText = "도시구분";
            this.colCity.Name = "colCity";
            // 
            // colType
            // 
            this.colType.FillWeight = 245.573F;
            this.colType.HeaderText = "시설구분";
            this.colType.Name = "colType";
            // 
            // colSubType
            // 
            this.colSubType.FillWeight = 213.4965F;
            this.colSubType.HeaderText = "시설의 세분";
            this.colSubType.Name = "colSubType";
            // 
            // colTypeName
            // 
            this.colTypeName.FillWeight = 185.7372F;
            this.colTypeName.HeaderText = "시설명";
            this.colTypeName.Name = "colTypeName";
            // 
            // colStatus
            // 
            this.colStatus.FillWeight = 161.714F;
            this.colStatus.HeaderText = "상태";
            this.colStatus.Name = "colStatus";
            // 
            // colLastDate
            // 
            this.colLastDate.FillWeight = 140.9241F;
            this.colLastDate.HeaderText = "최종변경일";
            this.colLastDate.Name = "colLastDate";
            // 
            // colFirstDate
            // 
            this.colFirstDate.FillWeight = 122.9322F;
            this.colFirstDate.HeaderText = "최초결정일";
            this.colFirstDate.Name = "colFirstDate";
            // 
            // colArea
            // 
            this.colArea.FillWeight = 107.3619F;
            this.colArea.HeaderText = "결정면적";
            this.colArea.Name = "colArea";
            // 
            // colInsertArea
            // 
            this.colInsertArea.FillWeight = 93.88719F;
            this.colInsertArea.HeaderText = "편입면적";
            this.colInsertArea.Name = "colInsertArea";
            // 
            // colComplete
            // 
            this.colComplete.FillWeight = 82.22606F;
            this.colComplete.HeaderText = "개설";
            this.colComplete.Name = "colComplete";
            // 
            // colIncomplete
            // 
            this.colIncomplete.FillWeight = 72.13437F;
            this.colIncomplete.HeaderText = "미개설";
            this.colIncomplete.Name = "colIncomplete";
            // 
            // colPartialComplete
            // 
            this.colPartialComplete.FillWeight = 63.40092F;
            this.colPartialComplete.HeaderText = "폭원미개설";
            this.colPartialComplete.Name = "colPartialComplete";
            // 
            // colRiceField
            // 
            this.colRiceField.FillWeight = 55.8429F;
            this.colRiceField.HeaderText = "전";
            this.colRiceField.Name = "colRiceField";
            // 
            // colField
            // 
            this.colField.FillWeight = 49.30211F;
            this.colField.HeaderText = "답";
            this.colField.Name = "colField";
            // 
            // colLand
            // 
            this.colLand.FillWeight = 43.64164F;
            this.colLand.HeaderText = "대지";
            this.colLand.Name = "colLand";
            // 
            // colETC
            // 
            this.colETC.FillWeight = 38.743F;
            this.colETC.HeaderText = "기타";
            this.colETC.Name = "colETC";
            // 
            // colNational
            // 
            this.colNational.FillWeight = 34.50368F;
            this.colNational.HeaderText = "국유지";
            this.colNational.Name = "colNational";
            // 
            // colPublic
            // 
            this.colPublic.FillWeight = 30.83492F;
            this.colPublic.HeaderText = "공유지";
            this.colPublic.Name = "colPublic";
            // 
            // colPrivate
            // 
            this.colPrivate.FillWeight = 27.65993F;
            this.colPrivate.HeaderText = "사유지";
            this.colPrivate.Name = "colPrivate";
            // 
            // colAvgCost
            // 
            this.colAvgCost.FillWeight = 24.91228F;
            this.colAvgCost.HeaderText = "평균공시지가";
            this.colAvgCost.Name = "colAvgCost";
            this.colAvgCost.Width = 120;
            // 
            // colConstCost
            // 
            this.colConstCost.FillWeight = 22.53442F;
            this.colConstCost.HeaderText = "공사비단가";
            this.colConstCost.Name = "colConstCost";
            // 
            // colOutlineCost
            // 
            this.colOutlineCost.HeaderText = "개략공사비";
            this.colOutlineCost.Name = "colOutlineCost";
            // 
            // checkBoxComplete
            // 
            this.checkBoxComplete.AutoSize = true;
            this.checkBoxComplete.Checked = true;
            this.checkBoxComplete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxComplete.Location = new System.Drawing.Point(12, 12);
            this.checkBoxComplete.Name = "checkBoxComplete";
            this.checkBoxComplete.Size = new System.Drawing.Size(48, 16);
            this.checkBoxComplete.TabIndex = 2;
            this.checkBoxComplete.Text = "개설";
            this.checkBoxComplete.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncomplete
            // 
            this.checkBoxIncomplete.AutoSize = true;
            this.checkBoxIncomplete.Checked = true;
            this.checkBoxIncomplete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIncomplete.Location = new System.Drawing.Point(83, 12);
            this.checkBoxIncomplete.Name = "checkBoxIncomplete";
            this.checkBoxIncomplete.Size = new System.Drawing.Size(60, 16);
            this.checkBoxIncomplete.TabIndex = 2;
            this.checkBoxIncomplete.Text = "미개설";
            this.checkBoxIncomplete.UseVisualStyleBackColor = true;
            // 
            // checkBoxPartialComplete
            // 
            this.checkBoxPartialComplete.AutoSize = true;
            this.checkBoxPartialComplete.Checked = true;
            this.checkBoxPartialComplete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPartialComplete.Location = new System.Drawing.Point(173, 12);
            this.checkBoxPartialComplete.Name = "checkBoxPartialComplete";
            this.checkBoxPartialComplete.Size = new System.Drawing.Size(84, 16);
            this.checkBoxPartialComplete.TabIndex = 2;
            this.checkBoxPartialComplete.Text = "폭원미개설";
            this.checkBoxPartialComplete.UseVisualStyleBackColor = true;
            // 
            // FormDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 447);
            this.Controls.Add(this.checkBoxPartialComplete);
            this.Controls.Add(this.checkBoxIncomplete);
            this.Controls.Add(this.checkBoxComplete);
            this.Controls.Add(this.dataGridViewOwner);
            this.Controls.Add(this.dataGridViewType);
            this.Controls.Add(this.dataGridViewIncomplete);
            this.Controls.Add(this.dataGridViewArea);
            this.Controls.Add(this.dataGridViewResult);
            this.Controls.Add(this.dataGridViewDecision);
            this.Controls.Add(this.dataGridViewDate);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FormDocument";
            this.Text = "세부시설 집행여부 조서";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDecision)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewIncomplete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridViewDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDate;
        private System.Windows.Forms.DataGridView dataGridViewDecision;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDecision;
        private System.Windows.Forms.DataGridView dataGridViewResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnResult;
        private System.Windows.Forms.DataGridView dataGridViewArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnArea;
        private System.Windows.Forms.DataGridView dataGridViewIncomplete;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnIncomplete;
        private System.Windows.Forms.DataGridView dataGridViewType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnType;
        private System.Windows.Forms.DataGridView dataGridViewOwner;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnOwner;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTypeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLastDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFirstDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInsertArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComplete;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIncomplete;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPartialComplete;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRiceField;
        private System.Windows.Forms.DataGridViewTextBoxColumn colField;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLand;
        private System.Windows.Forms.DataGridViewTextBoxColumn colETC;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNational;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPublic;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrivate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAvgCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConstCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOutlineCost;
        private System.Windows.Forms.CheckBox checkBoxComplete;
        private System.Windows.Forms.CheckBox checkBoxIncomplete;
        private System.Windows.Forms.CheckBox checkBoxPartialComplete;
    }
}