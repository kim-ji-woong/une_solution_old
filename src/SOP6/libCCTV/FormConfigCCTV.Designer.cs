namespace UnE.CCTV
{
    partial class FormConfigCCTV
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigCCTV));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbSituation = new System.Windows.Forms.RadioButton();
            this.rbNormal = new System.Windows.Forms.RadioButton();
            this.btnShowCCTVList = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioPSM = new System.Windows.Forms.RadioButton();
            this.radioFire = new System.Windows.Forms.RadioButton();
            this.labelEquipZone = new System.Windows.Forms.Label();
            this.labelFloor = new System.Windows.Forms.Label();
            this.labelBuilding = new System.Windows.Forms.Label();
            this.labelBuildingGroup = new System.Windows.Forms.Label();
            this.cmbEquipZone = new System.Windows.Forms.ComboBox();
            this.cmbFloor = new System.Windows.Forms.ComboBox();
            this.cmbBuilding = new System.Windows.Forms.ComboBox();
            this.cmbGroup = new System.Windows.Forms.ComboBox();
            this.chkConfigRegionCCTV = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbSituation);
            this.groupBox1.Controls.Add(this.rbNormal);
            this.groupBox1.Controls.Add(this.btnShowCCTVList);
            this.groupBox1.Location = new System.Drawing.Point(12, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(335, 78);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "보기";
            // 
            // rbSituation
            // 
            this.rbSituation.AutoSize = true;
            this.rbSituation.Location = new System.Drawing.Point(38, 111);
            this.rbSituation.Name = "rbSituation";
            this.rbSituation.Size = new System.Drawing.Size(135, 19);
            this.rbSituation.TabIndex = 2;
            this.rbSituation.Text = "화재상황 CCTV 보기";
            this.rbSituation.UseVisualStyleBackColor = true;
            this.rbSituation.Visible = false;
            this.rbSituation.CheckedChanged += new System.EventHandler(this.rbSituation_CheckedChanged);
            // 
            // rbNormal
            // 
            this.rbNormal.AutoSize = true;
            this.rbNormal.Checked = true;
            this.rbNormal.Location = new System.Drawing.Point(38, 80);
            this.rbNormal.Name = "rbNormal";
            this.rbNormal.Size = new System.Drawing.Size(123, 19);
            this.rbNormal.TabIndex = 1;
            this.rbNormal.TabStop = true;
            this.rbNormal.Text = "평상시 CCTV 보기";
            this.rbNormal.UseVisualStyleBackColor = true;
            this.rbNormal.Visible = false;
            this.rbNormal.CheckedChanged += new System.EventHandler(this.rbNormal_CheckedChanged);
            // 
            // btnShowCCTVList
            // 
            this.btnShowCCTVList.Location = new System.Drawing.Point(21, 24);
            this.btnShowCCTVList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnShowCCTVList.Name = "btnShowCCTVList";
            this.btnShowCCTVList.Size = new System.Drawing.Size(160, 32);
            this.btnShowCCTVList.TabIndex = 0;
            this.btnShowCCTVList.Text = "CCTV 전체 리스트 보기";
            this.btnShowCCTVList.UseVisualStyleBackColor = true;
            this.btnShowCCTVList.Click += new System.EventHandler(this.btnShowCCTVList_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.radioPSM);
            this.groupBox2.Controls.Add(this.radioFire);
            this.groupBox2.Controls.Add(this.labelEquipZone);
            this.groupBox2.Controls.Add(this.labelFloor);
            this.groupBox2.Controls.Add(this.labelBuilding);
            this.groupBox2.Controls.Add(this.labelBuildingGroup);
            this.groupBox2.Controls.Add(this.cmbEquipZone);
            this.groupBox2.Controls.Add(this.cmbFloor);
            this.groupBox2.Controls.Add(this.cmbBuilding);
            this.groupBox2.Controls.Add(this.cmbGroup);
            this.groupBox2.Controls.Add(this.chkConfigRegionCCTV);
            this.groupBox2.Location = new System.Drawing.Point(12, 101);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(335, 199);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "설정";
            // 
            // radioPSM
            // 
            this.radioPSM.AutoSize = true;
            this.radioPSM.Enabled = false;
            this.radioPSM.Location = new System.Drawing.Point(143, 45);
            this.radioPSM.Name = "radioPSM";
            this.radioPSM.Size = new System.Drawing.Size(97, 19);
            this.radioPSM.TabIndex = 6;
            this.radioPSM.Text = "유해화학물질";
            this.radioPSM.UseVisualStyleBackColor = true;
            this.radioPSM.Visible = false;
            this.radioPSM.CheckedChanged += new System.EventHandler(this.radioSensor_CheckedChanged);
            // 
            // radioFire
            // 
            this.radioFire.AutoSize = true;
            this.radioFire.Checked = true;
            this.radioFire.Enabled = false;
            this.radioFire.Location = new System.Drawing.Point(38, 45);
            this.radioFire.Name = "radioFire";
            this.radioFire.Size = new System.Drawing.Size(49, 19);
            this.radioFire.TabIndex = 6;
            this.radioFire.TabStop = true;
            this.radioFire.Text = "센서";
            this.radioFire.UseVisualStyleBackColor = true;
            this.radioFire.CheckedChanged += new System.EventHandler(this.radioSensor_CheckedChanged);
            // 
            // labelEquipZone
            // 
            this.labelEquipZone.AutoSize = true;
            this.labelEquipZone.Location = new System.Drawing.Point(18, 159);
            this.labelEquipZone.Name = "labelEquipZone";
            this.labelEquipZone.Size = new System.Drawing.Size(55, 15);
            this.labelEquipZone.TabIndex = 5;
            this.labelEquipZone.Text = "설비영역";
            this.labelEquipZone.Visible = false;
            // 
            // labelFloor
            // 
            this.labelFloor.AutoSize = true;
            this.labelFloor.Location = new System.Drawing.Point(18, 131);
            this.labelFloor.Name = "labelFloor";
            this.labelFloor.Size = new System.Drawing.Size(19, 15);
            this.labelFloor.TabIndex = 5;
            this.labelFloor.Text = "층";
            this.labelFloor.Visible = false;
            // 
            // labelBuilding
            // 
            this.labelBuilding.AutoSize = true;
            this.labelBuilding.Location = new System.Drawing.Point(18, 102);
            this.labelBuilding.Name = "labelBuilding";
            this.labelBuilding.Size = new System.Drawing.Size(31, 15);
            this.labelBuilding.TabIndex = 5;
            this.labelBuilding.Text = "건물";
            this.labelBuilding.Visible = false;
            // 
            // labelBuildingGroup
            // 
            this.labelBuildingGroup.AutoSize = true;
            this.labelBuildingGroup.Location = new System.Drawing.Point(18, 73);
            this.labelBuildingGroup.Name = "labelBuildingGroup";
            this.labelBuildingGroup.Size = new System.Drawing.Size(55, 15);
            this.labelBuildingGroup.TabIndex = 5;
            this.labelBuildingGroup.Text = "건물그룹";
            this.labelBuildingGroup.Visible = false;
            // 
            // cmbEquipZone
            // 
            this.cmbEquipZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquipZone.FormattingEnabled = true;
            this.cmbEquipZone.Location = new System.Drawing.Point(98, 156);
            this.cmbEquipZone.Name = "cmbEquipZone";
            this.cmbEquipZone.Size = new System.Drawing.Size(213, 23);
            this.cmbEquipZone.TabIndex = 4;
            this.cmbEquipZone.SelectedIndexChanged += new System.EventHandler(this.cmbEquipZone_SelectedIndexChanged);
            // 
            // cmbFloor
            // 
            this.cmbFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFloor.FormattingEnabled = true;
            this.cmbFloor.Location = new System.Drawing.Point(98, 128);
            this.cmbFloor.Name = "cmbFloor";
            this.cmbFloor.Size = new System.Drawing.Size(213, 23);
            this.cmbFloor.TabIndex = 3;
            this.cmbFloor.SelectedIndexChanged += new System.EventHandler(this.cmbFloor_SelectedIndexChanged);
            // 
            // cmbBuilding
            // 
            this.cmbBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuilding.FormattingEnabled = true;
            this.cmbBuilding.Location = new System.Drawing.Point(98, 99);
            this.cmbBuilding.Name = "cmbBuilding";
            this.cmbBuilding.Size = new System.Drawing.Size(213, 23);
            this.cmbBuilding.TabIndex = 2;
            this.cmbBuilding.SelectedIndexChanged += new System.EventHandler(this.cmbBuilding_SelectedIndexChanged);
            // 
            // cmbGroup
            // 
            this.cmbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGroup.FormattingEnabled = true;
            this.cmbGroup.Location = new System.Drawing.Point(98, 70);
            this.cmbGroup.Name = "cmbGroup";
            this.cmbGroup.Size = new System.Drawing.Size(213, 23);
            this.cmbGroup.TabIndex = 1;
            this.cmbGroup.SelectedIndexChanged += new System.EventHandler(this.cmbGroup_SelectedIndexChanged);
            // 
            // chkConfigRegionCCTV
            // 
            this.chkConfigRegionCCTV.AutoSize = true;
            this.chkConfigRegionCCTV.Location = new System.Drawing.Point(18, 24);
            this.chkConfigRegionCCTV.Name = "chkConfigRegionCCTV";
            this.chkConfigRegionCCTV.Size = new System.Drawing.Size(172, 19);
            this.chkConfigRegionCCTV.TabIndex = 0;
            this.chkConfigRegionCCTV.Text = "설비영역별 CCTV 설정하기";
            this.chkConfigRegionCCTV.UseVisualStyleBackColor = true;
            this.chkConfigRegionCCTV.CheckedChanged += new System.EventHandler(this.chkConfigRegionCCTV_CheckedChanged);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(260, 307);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(87, 29);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormConfigCCTV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 342);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigCCTV";
            this.Text = "CCTV 설정";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfigCCTV_FormClosing);
            this.Load += new System.EventHandler(this.FormConfigCCTV_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbSituation;
        private System.Windows.Forms.RadioButton rbNormal;
        private System.Windows.Forms.Button btnShowCCTVList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbFloor;
        private System.Windows.Forms.ComboBox cmbBuilding;
        private System.Windows.Forms.ComboBox cmbGroup;
        private System.Windows.Forms.CheckBox chkConfigRegionCCTV;
        private System.Windows.Forms.ComboBox cmbEquipZone;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labelEquipZone;
        private System.Windows.Forms.Label labelFloor;
        private System.Windows.Forms.Label labelBuilding;
        private System.Windows.Forms.Label labelBuildingGroup;
        private System.Windows.Forms.RadioButton radioPSM;
        private System.Windows.Forms.RadioButton radioFire;
    }
}