namespace EarthquakeSensorServer
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTimeSpan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMagnitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIntensity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHPGA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTPGA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPortNo = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.labelErrorMessage = new System.Windows.Forms.Label();
            this.btnSimpleInput = new System.Windows.Forms.Button();
            this.textBoxSMSTag = new System.Windows.Forms.TextBox();
            this.checkBoxInternalMessageSMSPopup = new System.Windows.Forms.CheckBox();
            this.labelSMSTime = new System.Windows.Forms.Label();
            this.checkBoxInternalMessageBroadcastPopup = new System.Windows.Forms.CheckBox();
            this.checkBoxAfter = new System.Windows.Forms.CheckBox();
            this.btnSirenOn = new System.Windows.Forms.Button();
            this.btnSirenOff = new System.Windows.Forms.Button();
            this.btnResetVIP = new System.Windows.Forms.Button();
            this.btnRunBroadcast = new System.Windows.Forms.Button();
            this.btnHomeView = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxBuildingName = new System.Windows.Forms.TextBox();
            this.btnCollapseBuilding = new System.Windows.Forms.Button();
            this.btnRecoverBuilding = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.colNo,
            this.colLocation,
            this.colTimeSpan,
            this.colMagnitude,
            this.colIntensity,
            this.colAlarmLevel,
            this.colHPGA,
            this.colTPGA});
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(810, 338);
            this.dataGridView1.TabIndex = 0;
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
            // colLocation
            // 
            this.colLocation.HeaderText = "위치";
            this.colLocation.Name = "colLocation";
            this.colLocation.ReadOnly = true;
            this.colLocation.Width = 165;
            // 
            // colTimeSpan
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTimeSpan.DefaultCellStyle = dataGridViewCellStyle3;
            this.colTimeSpan.HeaderText = "시간";
            this.colTimeSpan.Name = "colTimeSpan";
            this.colTimeSpan.ReadOnly = true;
            this.colTimeSpan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTimeSpan.Width = 165;
            // 
            // colMagnitude
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colMagnitude.DefaultCellStyle = dataGridViewCellStyle4;
            this.colMagnitude.HeaderText = "규모";
            this.colMagnitude.Name = "colMagnitude";
            this.colMagnitude.ReadOnly = true;
            this.colMagnitude.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colMagnitude.Visible = false;
            this.colMagnitude.Width = 70;
            // 
            // colIntensity
            // 
            this.colIntensity.HeaderText = "진도";
            this.colIntensity.Name = "colIntensity";
            this.colIntensity.ReadOnly = true;
            this.colIntensity.Width = 70;
            // 
            // colAlarmLevel
            // 
            this.colAlarmLevel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colAlarmLevel.DefaultCellStyle = dataGridViewCellStyle5;
            this.colAlarmLevel.HeaderText = "알람";
            this.colAlarmLevel.Name = "colAlarmLevel";
            this.colAlarmLevel.ReadOnly = true;
            this.colAlarmLevel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colHPGA
            // 
            this.colHPGA.HeaderText = "HPGA";
            this.colHPGA.Name = "colHPGA";
            this.colHPGA.ReadOnly = true;
            // 
            // colTPGA
            // 
            this.colTPGA.HeaderText = "TPGA";
            this.colTPGA.Name = "colTPGA";
            this.colTPGA.ReadOnly = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(747, 418);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 421);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port 번호";
            // 
            // textBoxPortNo
            // 
            this.textBoxPortNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPortNo.Location = new System.Drawing.Point(73, 416);
            this.textBoxPortNo.Name = "textBoxPortNo";
            this.textBoxPortNo.Size = new System.Drawing.Size(55, 21);
            this.textBoxPortNo.TabIndex = 3;
            this.textBoxPortNo.Text = "20000";
            this.textBoxPortNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(263, 418);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(48, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // labelErrorMessage
            // 
            this.labelErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelErrorMessage.AutoSize = true;
            this.labelErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.labelErrorMessage.Location = new System.Drawing.Point(188, 421);
            this.labelErrorMessage.Name = "labelErrorMessage";
            this.labelErrorMessage.Size = new System.Drawing.Size(69, 12);
            this.labelErrorMessage.TabIndex = 4;
            this.labelErrorMessage.Text = "에러 메시지";
            // 
            // btnSimpleInput
            // 
            this.btnSimpleInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSimpleInput.Location = new System.Drawing.Point(678, 417);
            this.btnSimpleInput.Name = "btnSimpleInput";
            this.btnSimpleInput.Size = new System.Drawing.Size(63, 23);
            this.btnSimpleInput.TabIndex = 1;
            this.btnSimpleInput.Text = "간편입력";
            this.btnSimpleInput.UseVisualStyleBackColor = true;
            this.btnSimpleInput.Click += new System.EventHandler(this.btnSimpleInput_Click);
            // 
            // textBoxSMSTag
            // 
            this.textBoxSMSTag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSMSTag.Location = new System.Drawing.Point(317, 418);
            this.textBoxSMSTag.Name = "textBoxSMSTag";
            this.textBoxSMSTag.Size = new System.Drawing.Size(100, 21);
            this.textBoxSMSTag.TabIndex = 5;
            // 
            // checkBoxInternalMessageSMSPopup
            // 
            this.checkBoxInternalMessageSMSPopup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxInternalMessageSMSPopup.AutoSize = true;
            this.checkBoxInternalMessageSMSPopup.Location = new System.Drawing.Point(560, 420);
            this.checkBoxInternalMessageSMSPopup.Name = "checkBoxInternalMessageSMSPopup";
            this.checkBoxInternalMessageSMSPopup.Size = new System.Drawing.Size(112, 16);
            this.checkBoxInternalMessageSMSPopup.TabIndex = 6;
            this.checkBoxInternalMessageSMSPopup.Text = "문자메시지 팝업";
            this.checkBoxInternalMessageSMSPopup.UseVisualStyleBackColor = true;
            this.checkBoxInternalMessageSMSPopup.CheckedChanged += new System.EventHandler(this.checkBoxInternalMessagePopup_CheckedChanged);
            // 
            // labelSMSTime
            // 
            this.labelSMSTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSMSTime.AutoSize = true;
            this.labelSMSTime.Location = new System.Drawing.Point(558, 395);
            this.labelSMSTime.Name = "labelSMSTime";
            this.labelSMSTime.Size = new System.Drawing.Size(89, 12);
            this.labelSMSTime.TabIndex = 7;
            this.labelSMSTime.Text = "문자 발송시간 :";
            // 
            // checkBoxInternalMessageBroadcastPopup
            // 
            this.checkBoxInternalMessageBroadcastPopup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxInternalMessageBroadcastPopup.AutoSize = true;
            this.checkBoxInternalMessageBroadcastPopup.Location = new System.Drawing.Point(468, 420);
            this.checkBoxInternalMessageBroadcastPopup.Name = "checkBoxInternalMessageBroadcastPopup";
            this.checkBoxInternalMessageBroadcastPopup.Size = new System.Drawing.Size(76, 16);
            this.checkBoxInternalMessageBroadcastPopup.TabIndex = 6;
            this.checkBoxInternalMessageBroadcastPopup.Text = "방송 팝업";
            this.checkBoxInternalMessageBroadcastPopup.UseVisualStyleBackColor = true;
            this.checkBoxInternalMessageBroadcastPopup.CheckedChanged += new System.EventHandler(this.checkBoxInternalMessagePopup_CheckedChanged);
            // 
            // checkBoxAfter
            // 
            this.checkBoxAfter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAfter.AutoSize = true;
            this.checkBoxAfter.Location = new System.Drawing.Point(468, 395);
            this.checkBoxAfter.Name = "checkBoxAfter";
            this.checkBoxAfter.Size = new System.Drawing.Size(48, 16);
            this.checkBoxAfter.TabIndex = 8;
            this.checkBoxAfter.Text = "여진";
            this.checkBoxAfter.UseVisualStyleBackColor = true;
            // 
            // btnSirenOn
            // 
            this.btnSirenOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSirenOn.Location = new System.Drawing.Point(136, 388);
            this.btnSirenOn.Name = "btnSirenOn";
            this.btnSirenOn.Size = new System.Drawing.Size(67, 26);
            this.btnSirenOn.TabIndex = 9;
            this.btnSirenOn.Text = "켜기";
            this.btnSirenOn.UseVisualStyleBackColor = true;
            this.btnSirenOn.Click += new System.EventHandler(this.btnSirenOn_Click);
            // 
            // btnSirenOff
            // 
            this.btnSirenOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSirenOff.Location = new System.Drawing.Point(209, 388);
            this.btnSirenOff.Name = "btnSirenOff";
            this.btnSirenOff.Size = new System.Drawing.Size(67, 26);
            this.btnSirenOff.TabIndex = 10;
            this.btnSirenOff.Text = "끄기";
            this.btnSirenOff.UseVisualStyleBackColor = true;
            this.btnSirenOff.Click += new System.EventHandler(this.btnSirenOff_Click);
            // 
            // btnResetVIP
            // 
            this.btnResetVIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetVIP.Location = new System.Drawing.Point(747, 388);
            this.btnResetVIP.Name = "btnResetVIP";
            this.btnResetVIP.Size = new System.Drawing.Size(75, 23);
            this.btnResetVIP.TabIndex = 11;
            this.btnResetVIP.Text = "Reset VIP";
            this.btnResetVIP.UseVisualStyleBackColor = true;
            this.btnResetVIP.Click += new System.EventHandler(this.btnResetVIP_Click);
            // 
            // btnRunBroadcast
            // 
            this.btnRunBroadcast.Enabled = false;
            this.btnRunBroadcast.Location = new System.Drawing.Point(678, 359);
            this.btnRunBroadcast.Name = "btnRunBroadcast";
            this.btnRunBroadcast.Size = new System.Drawing.Size(75, 23);
            this.btnRunBroadcast.TabIndex = 12;
            this.btnRunBroadcast.Text = "방송 실행";
            this.btnRunBroadcast.UseVisualStyleBackColor = true;
            this.btnRunBroadcast.Click += new System.EventHandler(this.btnRunBroadcast_Click);
            // 
            // btnHomeView
            // 
            this.btnHomeView.Location = new System.Drawing.Point(27, 369);
            this.btnHomeView.Name = "btnHomeView";
            this.btnHomeView.Size = new System.Drawing.Size(64, 19);
            this.btnHomeView.TabIndex = 13;
            this.btnHomeView.Text = "전체화면";
            this.btnHomeView.UseVisualStyleBackColor = true;
            this.btnHomeView.Click += new System.EventHandler(this.btnHomeView_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(315, 360);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "건물명    :";
            // 
            // textBoxBuildingName
            // 
            this.textBoxBuildingName.Location = new System.Drawing.Point(394, 356);
            this.textBoxBuildingName.Name = "textBoxBuildingName";
            this.textBoxBuildingName.Size = new System.Drawing.Size(63, 21);
            this.textBoxBuildingName.TabIndex = 15;
            // 
            // btnCollapseBuilding
            // 
            this.btnCollapseBuilding.Location = new System.Drawing.Point(317, 383);
            this.btnCollapseBuilding.Name = "btnCollapseBuilding";
            this.btnCollapseBuilding.Size = new System.Drawing.Size(67, 23);
            this.btnCollapseBuilding.TabIndex = 16;
            this.btnCollapseBuilding.Text = "건물붕괴";
            this.btnCollapseBuilding.UseVisualStyleBackColor = true;
            this.btnCollapseBuilding.Click += new System.EventHandler(this.btnCollapseBuilding_Click);
            // 
            // btnRecoverBuilding
            // 
            this.btnRecoverBuilding.Location = new System.Drawing.Point(390, 383);
            this.btnRecoverBuilding.Name = "btnRecoverBuilding";
            this.btnRecoverBuilding.Size = new System.Drawing.Size(67, 23);
            this.btnRecoverBuilding.TabIndex = 16;
            this.btnRecoverBuilding.Text = "붕괴종료";
            this.btnRecoverBuilding.UseVisualStyleBackColor = true;
            this.btnRecoverBuilding.Click += new System.EventHandler(this.btnRecoverBuilding_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 453);
            this.Controls.Add(this.btnRecoverBuilding);
            this.Controls.Add(this.btnCollapseBuilding);
            this.Controls.Add(this.textBoxBuildingName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnHomeView);
            this.Controls.Add(this.btnRunBroadcast);
            this.Controls.Add(this.btnResetVIP);
            this.Controls.Add(this.btnSirenOff);
            this.Controls.Add(this.btnSirenOn);
            this.Controls.Add(this.checkBoxAfter);
            this.Controls.Add(this.labelSMSTime);
            this.Controls.Add(this.checkBoxInternalMessageBroadcastPopup);
            this.Controls.Add(this.checkBoxInternalMessageSMSPopup);
            this.Controls.Add(this.textBoxSMSTag);
            this.Controls.Add(this.labelErrorMessage);
            this.Controls.Add(this.textBoxPortNo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSimpleInput);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FormMain";
            this.Text = "지진센서 서버";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPortNo;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label labelErrorMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTimeSpan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMagnitude;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIntensity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHPGA;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTPGA;
        private System.Windows.Forms.Button btnSimpleInput;
        private System.Windows.Forms.TextBox textBoxSMSTag;
        private System.Windows.Forms.CheckBox checkBoxInternalMessageSMSPopup;
        private System.Windows.Forms.Label labelSMSTime;
        private System.Windows.Forms.CheckBox checkBoxInternalMessageBroadcastPopup;
        private System.Windows.Forms.CheckBox checkBoxAfter;
        private System.Windows.Forms.Button btnSirenOn;
        private System.Windows.Forms.Button btnSirenOff;
        private System.Windows.Forms.Button btnResetVIP;
        private System.Windows.Forms.Button btnRunBroadcast;
        private System.Windows.Forms.Button btnHomeView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxBuildingName;
        private System.Windows.Forms.Button btnCollapseBuilding;
        private System.Windows.Forms.Button btnRecoverBuilding;
    }
}

