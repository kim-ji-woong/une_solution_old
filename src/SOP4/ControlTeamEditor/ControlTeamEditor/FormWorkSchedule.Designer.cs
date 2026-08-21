namespace ControlTeamEditor
{
    partial class FormWorkSchedule
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.radioScheduleA = new System.Windows.Forms.RadioButton();
            this.groupBoxControlRoom = new System.Windows.Forms.GroupBox();
            this.labelMemberInfo = new System.Windows.Forms.Label();
            this.dataGridControlRoomSchedule = new ControlTeamEditor.ComboBoxDataGridView();
            this.colNo1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNo2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNo3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNo4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNo5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNo6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxFireRoom = new System.Windows.Forms.GroupBox();
            this.labelExternalMemberInfo = new System.Windows.Forms.Label();
            this.dataGridFireCenterRoomSchedule = new ControlTeamEditor.ComboBoxDataGridView();
            this.colExternalMemberName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.radioFireScheduleA = new System.Windows.Forms.RadioButton();
            this.groupBoxDutyRoom = new System.Windows.Forms.GroupBox();
            this.labelDutyInfo = new System.Windows.Forms.Label();
            this.dataGridDuty = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBoxControlRoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridControlRoomSchedule)).BeginInit();
            this.groupBoxFireRoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFireCenterRoomSchedule)).BeginInit();
            this.groupBoxDutyRoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDuty)).BeginInit();
            this.SuspendLayout();
            // 
            // radioScheduleA
            // 
            this.radioScheduleA.AutoSize = true;
            this.radioScheduleA.Location = new System.Drawing.Point(6, 20);
            this.radioScheduleA.Name = "radioScheduleA";
            this.radioScheduleA.Size = new System.Drawing.Size(43, 16);
            this.radioScheduleA.TabIndex = 0;
            this.radioScheduleA.TabStop = true;
            this.radioScheduleA.Text = "A조";
            this.radioScheduleA.UseVisualStyleBackColor = true;
            this.radioScheduleA.CheckedChanged += new System.EventHandler(this.radioControlRoom_CheckedChanged);
            // 
            // groupBoxControlRoom
            // 
            this.groupBoxControlRoom.Controls.Add(this.labelMemberInfo);
            this.groupBoxControlRoom.Controls.Add(this.radioScheduleA);
            this.groupBoxControlRoom.Controls.Add(this.dataGridControlRoomSchedule);
            this.groupBoxControlRoom.Location = new System.Drawing.Point(12, 12);
            this.groupBoxControlRoom.Name = "groupBoxControlRoom";
            this.groupBoxControlRoom.Size = new System.Drawing.Size(732, 315);
            this.groupBoxControlRoom.TabIndex = 2;
            this.groupBoxControlRoom.TabStop = false;
            this.groupBoxControlRoom.Text = "제어실 근무표";
            // 
            // labelMemberInfo
            // 
            this.labelMemberInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMemberInfo.AutoSize = true;
            this.labelMemberInfo.Location = new System.Drawing.Point(7, 288);
            this.labelMemberInfo.Name = "labelMemberInfo";
            this.labelMemberInfo.Size = new System.Drawing.Size(53, 12);
            this.labelMemberInfo.TabIndex = 2;
            this.labelMemberInfo.Text = "직원정보";
            // 
            // dataGridControlRoomSchedule
            // 
            this.dataGridControlRoomSchedule.AllowUserToAddRows = false;
            this.dataGridControlRoomSchedule.AllowUserToDeleteRows = false;
            this.dataGridControlRoomSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridControlRoomSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridControlRoomSchedule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo1,
            this.colNo2,
            this.colNo3,
            this.colNo4,
            this.colNo5,
            this.colNo6});
            this.dataGridControlRoomSchedule.Location = new System.Drawing.Point(6, 47);
            this.dataGridControlRoomSchedule.MultiSelect = false;
            this.dataGridControlRoomSchedule.Name = "dataGridControlRoomSchedule";
            this.dataGridControlRoomSchedule.ReadOnly = true;
            this.dataGridControlRoomSchedule.RowTemplate.Height = 23;
            this.dataGridControlRoomSchedule.Size = new System.Drawing.Size(719, 230);
            this.dataGridControlRoomSchedule.TabIndex = 1;
            this.dataGridControlRoomSchedule.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridControlRoomSchedule_CellClick);
            this.dataGridControlRoomSchedule.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridControlRoomSchedule_CellDoubleClick);
            this.dataGridControlRoomSchedule.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridControlRoomSchedule_CellEnter);
            this.dataGridControlRoomSchedule.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGrid_KeyDown);
            // 
            // colNo1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo1.DefaultCellStyle = dataGridViewCellStyle1;
            this.colNo1.HeaderText = "1호기";
            this.colNo1.Name = "colNo1";
            this.colNo1.ReadOnly = true;
            // 
            // colNo2
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo2.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo2.HeaderText = "2호기";
            this.colNo2.Name = "colNo2";
            this.colNo2.ReadOnly = true;
            // 
            // colNo3
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo3.DefaultCellStyle = dataGridViewCellStyle3;
            this.colNo3.HeaderText = "3호기";
            this.colNo3.Name = "colNo3";
            this.colNo3.ReadOnly = true;
            // 
            // colNo4
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo4.DefaultCellStyle = dataGridViewCellStyle4;
            this.colNo4.HeaderText = "4호기";
            this.colNo4.Name = "colNo4";
            this.colNo4.ReadOnly = true;
            // 
            // colNo5
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo5.DefaultCellStyle = dataGridViewCellStyle5;
            this.colNo5.HeaderText = "5호기";
            this.colNo5.Name = "colNo5";
            this.colNo5.ReadOnly = true;
            // 
            // colNo6
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo6.DefaultCellStyle = dataGridViewCellStyle6;
            this.colNo6.HeaderText = "6호기";
            this.colNo6.Name = "colNo6";
            this.colNo6.ReadOnly = true;
            this.colNo6.Width = 176;
            // 
            // groupBoxFireRoom
            // 
            this.groupBoxFireRoom.Controls.Add(this.labelExternalMemberInfo);
            this.groupBoxFireRoom.Controls.Add(this.dataGridFireCenterRoomSchedule);
            this.groupBoxFireRoom.Controls.Add(this.radioFireScheduleA);
            this.groupBoxFireRoom.Location = new System.Drawing.Point(750, 12);
            this.groupBoxFireRoom.Name = "groupBoxFireRoom";
            this.groupBoxFireRoom.Size = new System.Drawing.Size(314, 315);
            this.groupBoxFireRoom.TabIndex = 3;
            this.groupBoxFireRoom.TabStop = false;
            this.groupBoxFireRoom.Text = "통합방재센터 근무표";
            // 
            // labelExternalMemberInfo
            // 
            this.labelExternalMemberInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelExternalMemberInfo.AutoSize = true;
            this.labelExternalMemberInfo.Location = new System.Drawing.Point(7, 288);
            this.labelExternalMemberInfo.Name = "labelExternalMemberInfo";
            this.labelExternalMemberInfo.Size = new System.Drawing.Size(53, 12);
            this.labelExternalMemberInfo.TabIndex = 2;
            this.labelExternalMemberInfo.Text = "직원정보";
            // 
            // dataGridFireCenterRoomSchedule
            // 
            this.dataGridFireCenterRoomSchedule.AllowUserToAddRows = false;
            this.dataGridFireCenterRoomSchedule.AllowUserToDeleteRows = false;
            this.dataGridFireCenterRoomSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridFireCenterRoomSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFireCenterRoomSchedule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colExternalMemberName});
            this.dataGridFireCenterRoomSchedule.Location = new System.Drawing.Point(6, 47);
            this.dataGridFireCenterRoomSchedule.MultiSelect = false;
            this.dataGridFireCenterRoomSchedule.Name = "dataGridFireCenterRoomSchedule";
            this.dataGridFireCenterRoomSchedule.ReadOnly = true;
            this.dataGridFireCenterRoomSchedule.RowTemplate.Height = 23;
            this.dataGridFireCenterRoomSchedule.Size = new System.Drawing.Size(300, 230);
            this.dataGridFireCenterRoomSchedule.TabIndex = 1;
            this.dataGridFireCenterRoomSchedule.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridFireCenterRoomSchedule_CellClick);
            this.dataGridFireCenterRoomSchedule.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridFireCenterRoomSchedule_CellDoubleClick);
            this.dataGridFireCenterRoomSchedule.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridFireCenterRoomSchedule_CellEnter);
            this.dataGridFireCenterRoomSchedule.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGrid_KeyDown);
            // 
            // colExternalMemberName
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colExternalMemberName.DefaultCellStyle = dataGridViewCellStyle7;
            this.colExternalMemberName.HeaderText = "이름";
            this.colExternalMemberName.Name = "colExternalMemberName";
            this.colExternalMemberName.ReadOnly = true;
            this.colExternalMemberName.Width = 257;
            // 
            // radioFireScheduleA
            // 
            this.radioFireScheduleA.AutoSize = true;
            this.radioFireScheduleA.Location = new System.Drawing.Point(6, 20);
            this.radioFireScheduleA.Name = "radioFireScheduleA";
            this.radioFireScheduleA.Size = new System.Drawing.Size(43, 16);
            this.radioFireScheduleA.TabIndex = 0;
            this.radioFireScheduleA.TabStop = true;
            this.radioFireScheduleA.Text = "A조";
            this.radioFireScheduleA.UseVisualStyleBackColor = true;
            this.radioFireScheduleA.CheckedChanged += new System.EventHandler(this.radioFireCenterRoom_CheckedChanged);
            // 
            // groupBoxDutyRoom
            // 
            this.groupBoxDutyRoom.Controls.Add(this.labelDutyInfo);
            this.groupBoxDutyRoom.Controls.Add(this.dataGridDuty);
            this.groupBoxDutyRoom.Location = new System.Drawing.Point(12, 342);
            this.groupBoxDutyRoom.Name = "groupBoxDutyRoom";
            this.groupBoxDutyRoom.Size = new System.Drawing.Size(314, 128);
            this.groupBoxDutyRoom.TabIndex = 3;
            this.groupBoxDutyRoom.TabStop = false;
            this.groupBoxDutyRoom.Text = "당직실 근무표";
            // 
            // labelDutyInfo
            // 
            this.labelDutyInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDutyInfo.AutoSize = true;
            this.labelDutyInfo.Location = new System.Drawing.Point(7, 101);
            this.labelDutyInfo.Name = "labelDutyInfo";
            this.labelDutyInfo.Size = new System.Drawing.Size(53, 12);
            this.labelDutyInfo.TabIndex = 2;
            this.labelDutyInfo.Text = "직원정보";
            // 
            // dataGridDuty
            // 
            this.dataGridDuty.AllowUserToAddRows = false;
            this.dataGridDuty.AllowUserToDeleteRows = false;
            this.dataGridDuty.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridDuty.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridDuty.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            this.dataGridDuty.Location = new System.Drawing.Point(6, 20);
            this.dataGridDuty.MultiSelect = false;
            this.dataGridDuty.Name = "dataGridDuty";
            this.dataGridDuty.ReadOnly = true;
            this.dataGridDuty.RowTemplate.Height = 23;
            this.dataGridDuty.Size = new System.Drawing.Size(300, 70);
            this.dataGridDuty.TabIndex = 1;
            this.dataGridDuty.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridDuty_CellClick);
            this.dataGridDuty.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridDuty_CellDoubleClick);
            this.dataGridDuty.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridDuty_CellEnter);
            this.dataGridDuty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGrid_KeyDown);
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn1.HeaderText = "이름";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 257;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(1003, 447);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(61, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(936, 447);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(61, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FormWorkSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 499);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBoxDutyRoom);
            this.Controls.Add(this.groupBoxFireRoom);
            this.Controls.Add(this.groupBoxControlRoom);
            this.Name = "FormWorkSchedule";
            this.Text = "근무표";
            this.Load += new System.EventHandler(this.FormWorkSchedule_Load);
            this.groupBoxControlRoom.ResumeLayout(false);
            this.groupBoxControlRoom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridControlRoomSchedule)).EndInit();
            this.groupBoxFireRoom.ResumeLayout(false);
            this.groupBoxFireRoom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFireCenterRoomSchedule)).EndInit();
            this.groupBoxDutyRoom.ResumeLayout(false);
            this.groupBoxDutyRoom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDuty)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioScheduleA;
        private ComboBoxDataGridView dataGridControlRoomSchedule;
        private System.Windows.Forms.GroupBox groupBoxControlRoom;
        private System.Windows.Forms.Label labelMemberInfo;
        private System.Windows.Forms.GroupBox groupBoxFireRoom;
        private System.Windows.Forms.Label labelExternalMemberInfo;
        private ComboBoxDataGridView dataGridFireCenterRoomSchedule;
        private System.Windows.Forms.RadioButton radioFireScheduleA;
        private System.Windows.Forms.GroupBox groupBoxDutyRoom;
        private System.Windows.Forms.Label labelDutyInfo;
        private System.Windows.Forms.DataGridView dataGridDuty;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo3;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo4;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo5;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo6;
        private System.Windows.Forms.DataGridViewTextBoxColumn colExternalMemberName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    }
}