namespace SOPMonitoringSystem.Popup
{
    partial class PopupWorkflowOption
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
            this.checkBoxUseSMS = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnEditManualTime = new System.Windows.Forms.Button();
            this.labelManualTime = new System.Windows.Forms.Label();
            this.radioManual = new System.Windows.Forms.RadioButton();
            this.radioAuto = new System.Windows.Forms.RadioButton();
            this.checkBoxShelterUse = new System.Windows.Forms.CheckBox();
            this.gridShelter = new System.Windows.Forms.DataGridView();
            this.colShelterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUse = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.panelOption = new System.Windows.Forms.Panel();
            this.gridUserDefinedParameters = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelUserDefinedParameters = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridShelter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUserDefinedParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxUseSMS
            // 
            this.checkBoxUseSMS.AutoSize = true;
            this.checkBoxUseSMS.Location = new System.Drawing.Point(22, 109);
            this.checkBoxUseSMS.Name = "checkBoxUseSMS";
            this.checkBoxUseSMS.Size = new System.Drawing.Size(202, 16);
            this.checkBoxUseSMS.TabIndex = 10;
            this.checkBoxUseSMS.Text = "상황 시작/종료 문자 메시지 사용";
            this.checkBoxUseSMS.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(105, 548);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 29);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "시작취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(22, 548);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(77, 29);
            this.btnRun.TabIndex = 8;
            this.btnRun.Text = "시작";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnEditManualTime
            // 
            this.btnEditManualTime.Location = new System.Drawing.Point(160, 49);
            this.btnEditManualTime.Name = "btnEditManualTime";
            this.btnEditManualTime.Size = new System.Drawing.Size(45, 23);
            this.btnEditManualTime.TabIndex = 14;
            this.btnEditManualTime.Text = "편집";
            this.btnEditManualTime.UseVisualStyleBackColor = true;
            this.btnEditManualTime.Visible = false;
            this.btnEditManualTime.Click += new System.EventHandler(this.btnEditManualTime_Click);
            // 
            // labelManualTime
            // 
            this.labelManualTime.AutoSize = true;
            this.labelManualTime.Location = new System.Drawing.Point(41, 56);
            this.labelManualTime.Name = "labelManualTime";
            this.labelManualTime.Size = new System.Drawing.Size(113, 12);
            this.labelManualTime.TabIndex = 13;
            this.labelManualTime.Text = "0000-00-00 00:00:00";
            this.labelManualTime.Visible = false;
            // 
            // radioManual
            // 
            this.radioManual.AutoSize = true;
            this.radioManual.Location = new System.Drawing.Point(22, 34);
            this.radioManual.Name = "radioManual";
            this.radioManual.Size = new System.Drawing.Size(123, 16);
            this.radioManual.TabIndex = 11;
            this.radioManual.Text = "재난발생시간 입력";
            this.radioManual.UseVisualStyleBackColor = true;
            this.radioManual.CheckedChanged += new System.EventHandler(this.radioManual_CheckedChanged);
            // 
            // radioAuto
            // 
            this.radioAuto.AutoSize = true;
            this.radioAuto.Checked = true;
            this.radioAuto.Location = new System.Drawing.Point(22, 12);
            this.radioAuto.Name = "radioAuto";
            this.radioAuto.Size = new System.Drawing.Size(211, 16);
            this.radioAuto.TabIndex = 12;
            this.radioAuto.TabStop = true;
            this.radioAuto.Text = "현재시간을 재난발생시간으로 설정";
            this.radioAuto.UseVisualStyleBackColor = true;
            this.radioAuto.CheckedChanged += new System.EventHandler(this.radioAuto_CheckedChanged);
            // 
            // checkBoxShelterUse
            // 
            this.checkBoxShelterUse.AutoSize = true;
            this.checkBoxShelterUse.Location = new System.Drawing.Point(22, 141);
            this.checkBoxShelterUse.Name = "checkBoxShelterUse";
            this.checkBoxShelterUse.Size = new System.Drawing.Size(60, 16);
            this.checkBoxShelterUse.TabIndex = 16;
            this.checkBoxShelterUse.Text = "피난처";
            this.checkBoxShelterUse.UseVisualStyleBackColor = true;
            this.checkBoxShelterUse.CheckedChanged += new System.EventHandler(this.checkBoxShelterUse_CheckedChanged);
            // 
            // gridShelter
            // 
            this.gridShelter.AllowUserToAddRows = false;
            this.gridShelter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridShelter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colShelterName,
            this.colDesc,
            this.colUse});
            this.gridShelter.Location = new System.Drawing.Point(22, 163);
            this.gridShelter.Name = "gridShelter";
            this.gridShelter.RowHeadersVisible = false;
            this.gridShelter.RowTemplate.Height = 23;
            this.gridShelter.Size = new System.Drawing.Size(413, 171);
            this.gridShelter.TabIndex = 15;
            // 
            // colShelterName
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colShelterName.DefaultCellStyle = dataGridViewCellStyle1;
            this.colShelterName.HeaderText = "피난처";
            this.colShelterName.Name = "colShelterName";
            this.colShelterName.Width = 200;
            // 
            // colDesc
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colDesc.DefaultCellStyle = dataGridViewCellStyle2;
            this.colDesc.HeaderText = "설명";
            this.colDesc.Name = "colDesc";
            this.colDesc.Width = 150;
            // 
            // colUse
            // 
            this.colUse.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colUse.HeaderText = "적용";
            this.colUse.Name = "colUse";
            // 
            // panelOption
            // 
            this.panelOption.Location = new System.Drawing.Point(261, 12);
            this.panelOption.Name = "panelOption";
            this.panelOption.Size = new System.Drawing.Size(174, 132);
            this.panelOption.TabIndex = 18;
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
            this.gridUserDefinedParameters.Location = new System.Drawing.Point(22, 369);
            this.gridUserDefinedParameters.Name = "gridUserDefinedParameters";
            this.gridUserDefinedParameters.RowHeadersVisible = false;
            this.gridUserDefinedParameters.RowTemplate.Height = 23;
            this.gridUserDefinedParameters.Size = new System.Drawing.Size(413, 171);
            this.gridUserDefinedParameters.TabIndex = 15;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn1.HeaderText = "변수명";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn2.HeaderText = "타입";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 70;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewCheckBoxColumn1.DefaultCellStyle = dataGridViewCellStyle5;
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
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle6;
            this.Column1.HeaderText = "입력";
            this.Column1.Name = "Column1";
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // labelUserDefinedParameters
            // 
            this.labelUserDefinedParameters.AutoSize = true;
            this.labelUserDefinedParameters.Location = new System.Drawing.Point(20, 347);
            this.labelUserDefinedParameters.Name = "labelUserDefinedParameters";
            this.labelUserDefinedParameters.Size = new System.Drawing.Size(216, 12);
            this.labelUserDefinedParameters.TabIndex = 19;
            this.labelUserDefinedParameters.Text = "■ SOP에서 사용중인 사용자 정의 변수";
            // 
            // PopupWorkflowOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 587);
            this.Controls.Add(this.labelUserDefinedParameters);
            this.Controls.Add(this.panelOption);
            this.Controls.Add(this.checkBoxShelterUse);
            this.Controls.Add(this.gridUserDefinedParameters);
            this.Controls.Add(this.gridShelter);
            this.Controls.Add(this.btnEditManualTime);
            this.Controls.Add(this.labelManualTime);
            this.Controls.Add(this.radioManual);
            this.Controls.Add(this.radioAuto);
            this.Controls.Add(this.checkBoxUseSMS);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRun);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupWorkflowOption";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "시작 이벤트 옵션";
            this.Load += new System.EventHandler(this.PopupWorkflowOption_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridShelter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUserDefinedParameters)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxUseSMS;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnEditManualTime;
        private System.Windows.Forms.Label labelManualTime;
        private System.Windows.Forms.RadioButton radioManual;
        private System.Windows.Forms.RadioButton radioAuto;
        private System.Windows.Forms.CheckBox checkBoxShelterUse;
        private System.Windows.Forms.DataGridView gridShelter;
        private System.Windows.Forms.DataGridViewTextBoxColumn colShelterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDesc;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colUse;
        private System.Windows.Forms.Panel panelOption;
        private System.Windows.Forms.DataGridView gridUserDefinedParameters;
        private System.Windows.Forms.Label labelUserDefinedParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    }
}