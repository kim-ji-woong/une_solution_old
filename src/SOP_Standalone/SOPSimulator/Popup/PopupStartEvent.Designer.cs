namespace SOPMonitoringSystem.Popup
{
    partial class PopupStartEvent
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPosition = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.radioAuto = new System.Windows.Forms.RadioButton();
            this.radioManual = new System.Windows.Forms.RadioButton();
            this.labelManualTime = new System.Windows.Forms.Label();
            this.btnEditManualTime = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelUserDefinedParameters = new System.Windows.Forms.Label();
            this.cboPSMType = new System.Windows.Forms.ComboBox();
            this.cboPositionHistory = new System.Windows.Forms.ComboBox();
            this.checkBoxShelterUse = new System.Windows.Forms.CheckBox();
            this.gridUserDefinedParameters = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridShelter = new System.Windows.Forms.DataGridView();
            this.colShelterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUse = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.labelPSMType = new System.Windows.Forms.Label();
            this.labelPSMDistance = new System.Windows.Forms.Label();
            this.textBoxPSMDistance = new System.Windows.Forms.TextBox();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUserDefinedParameters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridShelter)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "선택된 재난위치";
            // 
            // textBoxPosition
            // 
            this.textBoxPosition.Location = new System.Drawing.Point(10, 86);
            this.textBoxPosition.Name = "textBoxPosition";
            this.textBoxPosition.Size = new System.Drawing.Size(213, 21);
            this.textBoxPosition.TabIndex = 2;
            this.textBoxPosition.TextChanged += new System.EventHandler(this.textBoxPosition_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "최근 재난위치";
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(12, 604);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(77, 29);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "시작";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRunClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(95, 604);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 29);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "시작취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancelClick);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(12, 133);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(202, 16);
            this.checkBox2.TabIndex = 7;
            this.checkBox2.Text = "상황 시작/종료 문자 메시지 사용";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // radioAuto
            // 
            this.radioAuto.AutoSize = true;
            this.radioAuto.Checked = true;
            this.radioAuto.Location = new System.Drawing.Point(12, 166);
            this.radioAuto.Name = "radioAuto";
            this.radioAuto.Size = new System.Drawing.Size(211, 16);
            this.radioAuto.TabIndex = 8;
            this.radioAuto.TabStop = true;
            this.radioAuto.Text = "현재시간을 재난발생시간으로 설정";
            this.radioAuto.UseVisualStyleBackColor = true;
            this.radioAuto.CheckedChanged += new System.EventHandler(this.radioAuto_CheckedChanged);
            // 
            // radioManual
            // 
            this.radioManual.AutoSize = true;
            this.radioManual.Location = new System.Drawing.Point(12, 188);
            this.radioManual.Name = "radioManual";
            this.radioManual.Size = new System.Drawing.Size(123, 16);
            this.radioManual.TabIndex = 8;
            this.radioManual.Text = "재난발생시간 입력";
            this.radioManual.UseVisualStyleBackColor = true;
            this.radioManual.CheckedChanged += new System.EventHandler(this.radioManual_CheckedChanged);
            // 
            // labelManualTime
            // 
            this.labelManualTime.AutoSize = true;
            this.labelManualTime.Location = new System.Drawing.Point(26, 207);
            this.labelManualTime.Name = "labelManualTime";
            this.labelManualTime.Size = new System.Drawing.Size(113, 12);
            this.labelManualTime.TabIndex = 9;
            this.labelManualTime.Text = "0000-00-00 00:00:00";
            this.labelManualTime.Visible = false;
            // 
            // btnEditManualTime
            // 
            this.btnEditManualTime.Location = new System.Drawing.Point(176, 200);
            this.btnEditManualTime.Name = "btnEditManualTime";
            this.btnEditManualTime.Size = new System.Drawing.Size(45, 23);
            this.btnEditManualTime.TabIndex = 10;
            this.btnEditManualTime.Text = "편집";
            this.btnEditManualTime.UseVisualStyleBackColor = true;
            this.btnEditManualTime.Visible = false;
            this.btnEditManualTime.Click += new System.EventHandler(this.btnEditManualTime_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.labelUserDefinedParameters);
            this.panel2.Controls.Add(this.cboPSMType);
            this.panel2.Controls.Add(this.cboPositionHistory);
            this.panel2.Controls.Add(this.btnEditManualTime);
            this.panel2.Controls.Add(this.checkBoxShelterUse);
            this.panel2.Controls.Add(this.labelManualTime);
            this.panel2.Controls.Add(this.gridUserDefinedParameters);
            this.panel2.Controls.Add(this.gridShelter);
            this.panel2.Controls.Add(this.radioManual);
            this.panel2.Controls.Add(this.labelPSMType);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.radioAuto);
            this.panel2.Controls.Add(this.btnRun);
            this.panel2.Controls.Add(this.checkBox2);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.labelPSMDistance);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.textBoxPSMDistance);
            this.panel2.Controls.Add(this.textBoxPosition);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(438, 645);
            this.panel2.TabIndex = 12;
            // 
            // labelUserDefinedParameters
            // 
            this.labelUserDefinedParameters.AutoSize = true;
            this.labelUserDefinedParameters.Location = new System.Drawing.Point(12, 421);
            this.labelUserDefinedParameters.Name = "labelUserDefinedParameters";
            this.labelUserDefinedParameters.Size = new System.Drawing.Size(216, 12);
            this.labelUserDefinedParameters.TabIndex = 20;
            this.labelUserDefinedParameters.Text = "■ SOP에서 사용중인 사용자 정의 변수";
            // 
            // cboPSMType
            // 
            this.cboPSMType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPSMType.FormattingEnabled = true;
            this.cboPSMType.Location = new System.Drawing.Point(230, 31);
            this.cboPSMType.Name = "cboPSMType";
            this.cboPSMType.Size = new System.Drawing.Size(196, 20);
            this.cboPSMType.TabIndex = 15;
            this.cboPSMType.Visible = false;
            this.cboPSMType.SelectedValueChanged += new System.EventHandler(this.cboPSMType_SelectedValueChanged);
            // 
            // cboPositionHistory
            // 
            this.cboPositionHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPositionHistory.FormattingEnabled = true;
            this.cboPositionHistory.Location = new System.Drawing.Point(10, 30);
            this.cboPositionHistory.Name = "cboPositionHistory";
            this.cboPositionHistory.Size = new System.Drawing.Size(213, 20);
            this.cboPositionHistory.TabIndex = 14;
            this.cboPositionHistory.SelectedIndexChanged += new System.EventHandler(this.cboPositionHistory_SelectedIndexChanged);
            // 
            // checkBoxShelterUse
            // 
            this.checkBoxShelterUse.AutoSize = true;
            this.checkBoxShelterUse.Location = new System.Drawing.Point(12, 233);
            this.checkBoxShelterUse.Name = "checkBoxShelterUse";
            this.checkBoxShelterUse.Size = new System.Drawing.Size(60, 16);
            this.checkBoxShelterUse.TabIndex = 6;
            this.checkBoxShelterUse.Text = "피난처";
            this.checkBoxShelterUse.UseVisualStyleBackColor = true;
            this.checkBoxShelterUse.CheckedChanged += new System.EventHandler(this.checkBoxShelterUse_CheckedChanged);
            // 
            // gridUserDefinedParameters
            // 
            this.gridUserDefinedParameters.AllowUserToAddRows = false;
            this.gridUserDefinedParameters.AllowUserToDeleteRows = false;
            this.gridUserDefinedParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUserDefinedParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewCheckBoxColumn1,
            this.Column1});
            this.gridUserDefinedParameters.Location = new System.Drawing.Point(12, 443);
            this.gridUserDefinedParameters.Name = "gridUserDefinedParameters";
            this.gridUserDefinedParameters.RowHeadersVisible = false;
            this.gridUserDefinedParameters.RowTemplate.Height = 23;
            this.gridUserDefinedParameters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridUserDefinedParameters.Size = new System.Drawing.Size(414, 155);
            this.gridUserDefinedParameters.TabIndex = 5;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTextBoxColumn1.HeaderText = "변수명";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn2.HeaderText = "타입";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 70;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewCheckBoxColumn1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewCheckBoxColumn1.HeaderText = "설명";
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.ReadOnly = true;
            this.dataGridViewCheckBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCheckBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewCheckBoxColumn1.Width = 110;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column1.HeaderText = "입력";
            this.Column1.Name = "Column1";
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // gridShelter
            // 
            this.gridShelter.AllowUserToAddRows = false;
            this.gridShelter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridShelter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colShelterName,
            this.colDesc,
            this.colUse});
            this.gridShelter.Location = new System.Drawing.Point(12, 255);
            this.gridShelter.Name = "gridShelter";
            this.gridShelter.RowHeadersVisible = false;
            this.gridShelter.RowTemplate.Height = 23;
            this.gridShelter.Size = new System.Drawing.Size(414, 155);
            this.gridShelter.TabIndex = 5;
            this.gridShelter.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridShelter_CellContentClick);
            this.gridShelter.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridShelter_CellValueChanged);
            // 
            // colShelterName
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colShelterName.DefaultCellStyle = dataGridViewCellStyle5;
            this.colShelterName.HeaderText = "피난처";
            this.colShelterName.Name = "colShelterName";
            this.colShelterName.Width = 200;
            // 
            // colDesc
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colDesc.DefaultCellStyle = dataGridViewCellStyle6;
            this.colDesc.HeaderText = "설명";
            this.colDesc.Name = "colDesc";
            this.colDesc.Width = 150;
            // 
            // colUse
            // 
            this.colUse.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colUse.HeaderText = "적용";
            this.colUse.Name = "colUse";
            this.colUse.ReadOnly = true;
            // 
            // labelPSMType
            // 
            this.labelPSMType.AutoSize = true;
            this.labelPSMType.Location = new System.Drawing.Point(232, 10);
            this.labelPSMType.Name = "labelPSMType";
            this.labelPSMType.Size = new System.Drawing.Size(81, 12);
            this.labelPSMType.TabIndex = 4;
            this.labelPSMType.Text = "오염물질 종류";
            this.labelPSMType.Visible = false;
            // 
            // labelPSMDistance
            // 
            this.labelPSMDistance.AutoSize = true;
            this.labelPSMDistance.Location = new System.Drawing.Point(232, 64);
            this.labelPSMDistance.Name = "labelPSMDistance";
            this.labelPSMDistance.Size = new System.Drawing.Size(87, 12);
            this.labelPSMDistance.TabIndex = 1;
            this.labelPSMDistance.Text = "대피거리(미터)";
            this.labelPSMDistance.Visible = false;
            // 
            // textBoxPSMDistance
            // 
            this.textBoxPSMDistance.Enabled = false;
            this.textBoxPSMDistance.Location = new System.Drawing.Point(230, 86);
            this.textBoxPSMDistance.Name = "textBoxPSMDistance";
            this.textBoxPSMDistance.Size = new System.Drawing.Size(196, 21);
            this.textBoxPSMDistance.TabIndex = 2;
            this.textBoxPSMDistance.Visible = false;
            this.textBoxPSMDistance.TextChanged += new System.EventHandler(this.textBoxPSMDistance_TextChanged);
            // 
            // PopupStartEvent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 645);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupStartEvent";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "시작 이벤트 옵션";
            this.Load += new System.EventHandler(this.PopupStartEvent_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUserDefinedParameters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridShelter)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPosition;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.RadioButton radioAuto;
        private System.Windows.Forms.RadioButton radioManual;
        private System.Windows.Forms.Label labelManualTime;
        private System.Windows.Forms.Button btnEditManualTime;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox checkBoxShelterUse;
        private System.Windows.Forms.DataGridView gridShelter;
        private System.Windows.Forms.DataGridViewTextBoxColumn colShelterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDesc;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colUse;
        private System.Windows.Forms.ComboBox cboPositionHistory;
        private System.Windows.Forms.Label labelPSMType;
        private System.Windows.Forms.Label labelPSMDistance;
        private System.Windows.Forms.TextBox textBoxPSMDistance;
        private System.Windows.Forms.ComboBox cboPSMType;
        private System.Windows.Forms.DataGridView gridUserDefinedParameters;
        private System.Windows.Forms.Label labelUserDefinedParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    }
}