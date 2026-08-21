namespace SDMS
{
    partial class FormSMSHistory
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
            this.btnReleaseAll = new System.Windows.Forms.Button();
            this.editOrgMsg = new System.Windows.Forms.TextBox();
            this.btnSendSMS = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.editNewMsg = new System.Windows.Forms.TextBox();
            this.lbSelectMember = new System.Windows.Forms.Label();
            this.lbTotalMember = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSelectReverse = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbEquipZone = new System.Windows.Forms.Label();
            this.lbFloor = new System.Windows.Forms.Label();
            this.lbBuilding = new System.Windows.Forms.Label();
            this.lbBuildingGroup = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnReleaseAll
            // 
            this.btnReleaseAll.Location = new System.Drawing.Point(234, 383);
            this.btnReleaseAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReleaseAll.Name = "btnReleaseAll";
            this.btnReleaseAll.Size = new System.Drawing.Size(94, 29);
            this.btnReleaseAll.TabIndex = 1;
            this.btnReleaseAll.Text = "모두 해제";
            this.btnReleaseAll.UseVisualStyleBackColor = true;
            this.btnReleaseAll.Click += new System.EventHandler(this.btnReleaseAll_Click);
            // 
            // editOrgMsg
            // 
            this.editOrgMsg.Location = new System.Drawing.Point(6, 39);
            this.editOrgMsg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editOrgMsg.Multiline = true;
            this.editOrgMsg.Name = "editOrgMsg";
            this.editOrgMsg.ReadOnly = true;
            this.editOrgMsg.Size = new System.Drawing.Size(211, 79);
            this.editOrgMsg.TabIndex = 2;
            // 
            // btnSendSMS
            // 
            this.btnSendSMS.Location = new System.Drawing.Point(236, 190);
            this.btnSendSMS.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSendSMS.Name = "btnSendSMS";
            this.btnSendSMS.Size = new System.Drawing.Size(94, 29);
            this.btnSendSMS.TabIndex = 5;
            this.btnSendSMS.Text = "전송하기";
            this.btnSendSMS.UseVisualStyleBackColor = true;
            this.btnSendSMS.Click += new System.EventHandler(this.btnSendSMS_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label7);
            this.panel1.Location = new System.Drawing.Point(12, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(758, 47);
            this.panel1.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(20, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(122, 16);
            this.label7.TabIndex = 1;
            this.label7.Text = "문자 전송 확인";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Location = new System.Drawing.Point(12, 62);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(758, 454);
            this.panel2.TabIndex = 11;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.editNewMsg);
            this.groupBox3.Controls.Add(this.lbSelectMember);
            this.groupBox3.Controls.Add(this.lbTotalMember);
            this.groupBox3.Controls.Add(this.editOrgMsg);
            this.groupBox3.Controls.Add(this.btnSendSMS);
            this.groupBox3.Location = new System.Drawing.Point(400, 200);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(346, 231);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "전송문자";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 15);
            this.label2.TabIndex = 16;
            this.label2.Text = "전송할 문자";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 15);
            this.label1.TabIndex = 15;
            this.label1.Text = "전송된 문자";
            // 
            // editNewMsg
            // 
            this.editNewMsg.Location = new System.Drawing.Point(6, 142);
            this.editNewMsg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editNewMsg.Multiline = true;
            this.editNewMsg.Name = "editNewMsg";
            this.editNewMsg.Size = new System.Drawing.Size(211, 77);
            this.editNewMsg.TabIndex = 14;
            // 
            // lbSelectMember
            // 
            this.lbSelectMember.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSelectMember.AutoSize = true;
            this.lbSelectMember.Location = new System.Drawing.Point(283, 65);
            this.lbSelectMember.Name = "lbSelectMember";
            this.lbSelectMember.Size = new System.Drawing.Size(47, 15);
            this.lbSelectMember.TabIndex = 13;
            this.lbSelectMember.Text = "명 선택";
            // 
            // lbTotalMember
            // 
            this.lbTotalMember.AutoSize = true;
            this.lbTotalMember.Location = new System.Drawing.Point(233, 39);
            this.lbTotalMember.Name = "lbTotalMember";
            this.lbTotalMember.Size = new System.Drawing.Size(51, 15);
            this.lbTotalMember.TabIndex = 13;
            this.lbTotalMember.Text = "총 명 중";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSelectReverse);
            this.groupBox2.Controls.Add(this.btnSelectAll);
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Controls.Add(this.btnReleaseAll);
            this.groupBox2.Location = new System.Drawing.Point(14, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(381, 424);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "문자 전송 내역";
            // 
            // btnSelectReverse
            // 
            this.btnSelectReverse.Location = new System.Drawing.Point(134, 383);
            this.btnSelectReverse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSelectReverse.Name = "btnSelectReverse";
            this.btnSelectReverse.Size = new System.Drawing.Size(94, 29);
            this.btnSelectReverse.TabIndex = 15;
            this.btnSelectReverse.Text = "체크 반전";
            this.btnSelectReverse.UseVisualStyleBackColor = true;
            this.btnSelectReverse.Click += new System.EventHandler(this.btnSelectReverse_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(34, 383);
            this.btnSelectAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(94, 29);
            this.btnSelectAll.TabIndex = 14;
            this.btnSelectAll.Text = "모두 선택";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeight = 25;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelect,
            this.colNo,
            this.colName,
            this.colTeam,
            this.colGrade});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(9, 23);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(358, 352);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
            // 
            // colSelect
            // 
            this.colSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colSelect.HeaderText = "선택";
            this.colSelect.Name = "colSelect";
            this.colSelect.Width = 40;
            // 
            // colNo
            // 
            this.colNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "순번";
            this.colNo.Name = "colNo";
            this.colNo.Width = 40;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colName.DefaultCellStyle = dataGridViewCellStyle3;
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            // 
            // colTeam
            // 
            this.colTeam.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colTeam.HeaderText = "팀";
            this.colTeam.Name = "colTeam";
            // 
            // colGrade
            // 
            this.colGrade.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colGrade.DefaultCellStyle = dataGridViewCellStyle4;
            this.colGrade.FillWeight = 25F;
            this.colGrade.HeaderText = "직급";
            this.colGrade.Name = "colGrade";
            this.colGrade.Width = 60;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbEquipZone);
            this.groupBox1.Controls.Add(this.lbFloor);
            this.groupBox1.Controls.Add(this.lbBuilding);
            this.groupBox1.Controls.Add(this.lbBuildingGroup);
            this.groupBox1.Location = new System.Drawing.Point(401, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(345, 187);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "센서 위치";
            // 
            // lbEquipZone
            // 
            this.lbEquipZone.AutoSize = true;
            this.lbEquipZone.Location = new System.Drawing.Point(50, 153);
            this.lbEquipZone.Name = "lbEquipZone";
            this.lbEquipZone.Size = new System.Drawing.Size(0, 15);
            this.lbEquipZone.TabIndex = 13;
            // 
            // lbFloor
            // 
            this.lbFloor.AutoSize = true;
            this.lbFloor.Location = new System.Drawing.Point(50, 114);
            this.lbFloor.Name = "lbFloor";
            this.lbFloor.Size = new System.Drawing.Size(0, 15);
            this.lbFloor.TabIndex = 12;
            // 
            // lbBuilding
            // 
            this.lbBuilding.AutoSize = true;
            this.lbBuilding.Location = new System.Drawing.Point(50, 74);
            this.lbBuilding.Name = "lbBuilding";
            this.lbBuilding.Size = new System.Drawing.Size(0, 15);
            this.lbBuilding.TabIndex = 11;
            // 
            // lbBuildingGroup
            // 
            this.lbBuildingGroup.AutoSize = true;
            this.lbBuildingGroup.Location = new System.Drawing.Point(50, 35);
            this.lbBuildingGroup.Name = "lbBuildingGroup";
            this.lbBuildingGroup.Size = new System.Drawing.Size(0, 15);
            this.lbBuildingGroup.TabIndex = 10;
            // 
            // FormSMSHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(782, 528);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormSMSHistory";
            this.Text = "문자 전송";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnReleaseAll;
        private System.Windows.Forms.TextBox editOrgMsg;
        private System.Windows.Forms.Button btnSendSMS;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbSelectMember;
        private System.Windows.Forms.Label lbTotalMember;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbFloor;
        private System.Windows.Forms.Label lbBuilding;
        private System.Windows.Forms.Label lbBuildingGroup;
        private System.Windows.Forms.Label lbEquipZone;
        private System.Windows.Forms.Button btnSelectReverse;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.TextBox editNewMsg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGrade;
    }
}