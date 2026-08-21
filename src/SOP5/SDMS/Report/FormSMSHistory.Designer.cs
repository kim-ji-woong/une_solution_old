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
            this.editOrgMsg = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.editNewMsg = new System.Windows.Forms.TextBox();
            this.lbSelectMember = new System.Windows.Forms.Label();
            this.lbTotalMember = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbEquipZone = new System.Windows.Forms.Label();
            this.lbFloor = new System.Windows.Forms.Label();
            this.lbBuilding = new System.Windows.Forms.Label();
            this.lbBuildingGroup = new System.Windows.Forms.Label();
            this.btnSelectAll = new UnE.GUI.ImageButton();
            this.btnSelectReverse = new UnE.GUI.ImageButton();
            this.btnReleaseAll = new UnE.GUI.ImageButton();
            this.btnSendSMS = new UnE.GUI.ImageButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSelectAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSelectReverse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnReleaseAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSendSMS)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // editOrgMsg
            // 
            this.editOrgMsg.Location = new System.Drawing.Point(11, 36);
            this.editOrgMsg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editOrgMsg.Multiline = true;
            this.editOrgMsg.Name = "editOrgMsg";
            this.editOrgMsg.ReadOnly = true;
            this.editOrgMsg.Size = new System.Drawing.Size(395, 90);
            this.editOrgMsg.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(7, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 16;
            this.label2.Text = "전송할 문자";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(7, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 15;
            this.label1.Text = "전송된 문자";
            // 
            // editNewMsg
            // 
            this.editNewMsg.Location = new System.Drawing.Point(11, 163);
            this.editNewMsg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editNewMsg.Multiline = true;
            this.editNewMsg.Name = "editNewMsg";
            this.editNewMsg.Size = new System.Drawing.Size(395, 149);
            this.editNewMsg.TabIndex = 14;
            // 
            // lbSelectMember
            // 
            this.lbSelectMember.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSelectMember.AutoSize = true;
            this.lbSelectMember.BackColor = System.Drawing.Color.Transparent;
            this.lbSelectMember.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbSelectMember.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.lbSelectMember.Location = new System.Drawing.Point(194, 11);
            this.lbSelectMember.Name = "lbSelectMember";
            this.lbSelectMember.Size = new System.Drawing.Size(61, 18);
            this.lbSelectMember.TabIndex = 13;
            this.lbSelectMember.Text = "명 선택";
            // 
            // lbTotalMember
            // 
            this.lbTotalMember.AutoSize = true;
            this.lbTotalMember.BackColor = System.Drawing.Color.Transparent;
            this.lbTotalMember.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTotalMember.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.lbTotalMember.Location = new System.Drawing.Point(122, 11);
            this.lbTotalMember.Name = "lbTotalMember";
            this.lbTotalMember.Size = new System.Drawing.Size(66, 18);
            this.lbTotalMember.TabIndex = 13;
            this.lbTotalMember.Text = "총 명 중";
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
            this.dataGridView1.Location = new System.Drawing.Point(12, 82);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(349, 455);
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
            // lbEquipZone
            // 
            this.lbEquipZone.AutoSize = true;
            this.lbEquipZone.BackColor = System.Drawing.Color.Transparent;
            this.lbEquipZone.Font = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbEquipZone.ForeColor = System.Drawing.Color.White;
            this.lbEquipZone.Location = new System.Drawing.Point(153, 12);
            this.lbEquipZone.Name = "lbEquipZone";
            this.lbEquipZone.Size = new System.Drawing.Size(45, 20);
            this.lbEquipZone.TabIndex = 13;
            this.lbEquipZone.Text = "Text";
            // 
            // lbFloor
            // 
            this.lbFloor.AutoSize = true;
            this.lbFloor.BackColor = System.Drawing.Color.Transparent;
            this.lbFloor.Font = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbFloor.ForeColor = System.Drawing.Color.White;
            this.lbFloor.Location = new System.Drawing.Point(102, 12);
            this.lbFloor.Name = "lbFloor";
            this.lbFloor.Size = new System.Drawing.Size(45, 20);
            this.lbFloor.TabIndex = 12;
            this.lbFloor.Text = "Text";
            // 
            // lbBuilding
            // 
            this.lbBuilding.AutoSize = true;
            this.lbBuilding.BackColor = System.Drawing.Color.Transparent;
            this.lbBuilding.Font = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbBuilding.ForeColor = System.Drawing.Color.White;
            this.lbBuilding.Location = new System.Drawing.Point(51, 12);
            this.lbBuilding.Name = "lbBuilding";
            this.lbBuilding.Size = new System.Drawing.Size(45, 20);
            this.lbBuilding.TabIndex = 11;
            this.lbBuilding.Text = "Text";
            // 
            // lbBuildingGroup
            // 
            this.lbBuildingGroup.AutoSize = true;
            this.lbBuildingGroup.BackColor = System.Drawing.Color.Transparent;
            this.lbBuildingGroup.Font = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbBuildingGroup.ForeColor = System.Drawing.Color.White;
            this.lbBuildingGroup.Location = new System.Drawing.Point(7, 12);
            this.lbBuildingGroup.Name = "lbBuildingGroup";
            this.lbBuildingGroup.Size = new System.Drawing.Size(45, 20);
            this.lbBuildingGroup.TabIndex = 10;
            this.lbBuildingGroup.Text = "Text";
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectAll.ButtonText = "";
            this.btnSelectAll.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectAll.ImageClicked = global::SDMS.Properties.Resources.BtnSelectAll_Click;
            this.btnSelectAll.ImageDisabled = null;
            this.btnSelectAll.ImageMouseOver = global::SDMS.Properties.Resources.BtnSelectAll_Click;
            this.btnSelectAll.ImageNormal = global::SDMS.Properties.Resources.BtnSelectAll_Default;
            this.btnSelectAll.Location = new System.Drawing.Point(375, 501);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Owner = null;
            this.btnSelectAll.Size = new System.Drawing.Size(70, 36);
            this.btnSelectAll.TabIndex = 15;
            this.btnSelectAll.TabStop = false;
            this.btnSelectAll.TextColor = System.Drawing.Color.Black;
            this.btnSelectAll.TextFont = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectAll.ToolTipText = "";
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnSelectReverse
            // 
            this.btnSelectReverse.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectReverse.ButtonText = "";
            this.btnSelectReverse.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectReverse.ImageClicked = global::SDMS.Properties.Resources.BtnSelectReverse_Click;
            this.btnSelectReverse.ImageDisabled = null;
            this.btnSelectReverse.ImageMouseOver = global::SDMS.Properties.Resources.BtnSelectReverse_Click;
            this.btnSelectReverse.ImageNormal = global::SDMS.Properties.Resources.BtnSelectReverse_Default;
            this.btnSelectReverse.Location = new System.Drawing.Point(451, 501);
            this.btnSelectReverse.Name = "btnSelectReverse";
            this.btnSelectReverse.Owner = null;
            this.btnSelectReverse.Size = new System.Drawing.Size(70, 36);
            this.btnSelectReverse.TabIndex = 16;
            this.btnSelectReverse.TabStop = false;
            this.btnSelectReverse.TextColor = System.Drawing.Color.Black;
            this.btnSelectReverse.TextFont = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectReverse.ToolTipText = "";
            this.btnSelectReverse.Click += new System.EventHandler(this.btnSelectReverse_Click);
            // 
            // btnReleaseAll
            // 
            this.btnReleaseAll.BackColor = System.Drawing.Color.Transparent;
            this.btnReleaseAll.ButtonText = "";
            this.btnReleaseAll.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnReleaseAll.ImageClicked = global::SDMS.Properties.Resources.BtnReleaseAll_Click;
            this.btnReleaseAll.ImageDisabled = null;
            this.btnReleaseAll.ImageMouseOver = global::SDMS.Properties.Resources.BtnReleaseAll_Click;
            this.btnReleaseAll.ImageNormal = global::SDMS.Properties.Resources.BtnReleaseAll_Default;
            this.btnReleaseAll.Location = new System.Drawing.Point(527, 501);
            this.btnReleaseAll.Name = "btnReleaseAll";
            this.btnReleaseAll.Owner = null;
            this.btnReleaseAll.Size = new System.Drawing.Size(70, 36);
            this.btnReleaseAll.TabIndex = 17;
            this.btnReleaseAll.TabStop = false;
            this.btnReleaseAll.TextColor = System.Drawing.Color.Black;
            this.btnReleaseAll.TextFont = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnReleaseAll.ToolTipText = "";
            this.btnReleaseAll.Click += new System.EventHandler(this.btnReleaseAll_Click);
            // 
            // btnSendSMS
            // 
            this.btnSendSMS.BackColor = System.Drawing.Color.Transparent;
            this.btnSendSMS.ButtonText = "";
            this.btnSendSMS.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSendSMS.ImageClicked = global::SDMS.Properties.Resources.BtnSendSMS_Click;
            this.btnSendSMS.ImageDisabled = null;
            this.btnSendSMS.ImageMouseOver = global::SDMS.Properties.Resources.BtnSendSMS_Click;
            this.btnSendSMS.ImageNormal = global::SDMS.Properties.Resources.BtnSendSMS_Default;
            this.btnSendSMS.Location = new System.Drawing.Point(728, 501);
            this.btnSendSMS.Name = "btnSendSMS";
            this.btnSendSMS.Owner = null;
            this.btnSendSMS.Size = new System.Drawing.Size(70, 36);
            this.btnSendSMS.TabIndex = 18;
            this.btnSendSMS.TabStop = false;
            this.btnSendSMS.TextColor = System.Drawing.Color.Black;
            this.btnSendSMS.TextFont = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSendSMS.ToolTipText = "";
            this.btnSendSMS.Click += new System.EventHandler(this.btnSendSMS_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(12, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.TabIndex = 19;
            this.label3.Text = "전송된 문자";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(373, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 20);
            this.label4.TabIndex = 20;
            this.label4.Text = "문자 전송 내역";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panel1.Controls.Add(this.lbEquipZone);
            this.panel1.Controls.Add(this.lbBuildingGroup);
            this.panel1.Controls.Add(this.lbBuilding);
            this.panel1.Controls.Add(this.lbFloor);
            this.panel1.Location = new System.Drawing.Point(377, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(421, 44);
            this.panel1.TabIndex = 21;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panel2.Controls.Add(this.editOrgMsg);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.lbTotalMember);
            this.panel2.Controls.Add(this.lbSelectMember);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.editNewMsg);
            this.panel2.Location = new System.Drawing.Point(377, 162);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(421, 327);
            this.panel2.TabIndex = 22;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(373, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 20);
            this.label5.TabIndex = 23;
            this.label5.Text = "전송 문자";
            // 
            // FormSMSHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BackgroundImage = global::SDMS.Properties.Resources.SMSHistory_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(810, 550);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnSendSMS);
            this.Controls.Add(this.btnReleaseAll);
            this.Controls.Add(this.btnSelectReverse);
            this.Controls.Add(this.btnSelectAll);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormSMSHistory";
            this.Text = "문자 전송";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSelectAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSelectReverse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnReleaseAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSendSMS)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox editOrgMsg;
        private System.Windows.Forms.Label lbSelectMember;
        private System.Windows.Forms.Label lbTotalMember;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lbFloor;
        private System.Windows.Forms.Label lbBuilding;
        private System.Windows.Forms.Label lbBuildingGroup;
        private System.Windows.Forms.Label lbEquipZone;
        private System.Windows.Forms.TextBox editNewMsg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGrade;
        private UnE.GUI.ImageButton btnSelectAll;
        private UnE.GUI.ImageButton btnSelectReverse;
        private UnE.GUI.ImageButton btnReleaseAll;
        private UnE.GUI.ImageButton btnSendSMS;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
    }
}