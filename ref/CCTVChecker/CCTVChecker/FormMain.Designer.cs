namespace CCTVChecker
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbGroup = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.cmbEquipZone = new System.Windows.Forms.ComboBox();
            this.cmbFloor = new System.Windows.Forms.ComboBox();
            this.cmbBuilding = new System.Windows.Forms.ComboBox();
            this.btnCCTV = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelCCTV = new System.Windows.Forms.Panel();
            this.btnFill = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnFill);
            this.panel1.Controls.Add(this.cmbGroup);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.cmbEquipZone);
            this.panel1.Controls.Add(this.cmbFloor);
            this.panel1.Controls.Add(this.cmbBuilding);
            this.panel1.Controls.Add(this.btnCCTV);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1211, 37);
            this.panel1.TabIndex = 0;
            // 
            // cmbGroup
            // 
            this.cmbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGroup.FormattingEnabled = true;
            this.cmbGroup.Location = new System.Drawing.Point(12, 9);
            this.cmbGroup.Name = "cmbGroup";
            this.cmbGroup.Size = new System.Drawing.Size(141, 20);
            this.cmbGroup.TabIndex = 5;
            this.cmbGroup.SelectedIndexChanged += new System.EventHandler(this.cmbGroup_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(940, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(76, 34);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbEquipZone
            // 
            this.cmbEquipZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquipZone.FormattingEnabled = true;
            this.cmbEquipZone.Location = new System.Drawing.Point(527, 10);
            this.cmbEquipZone.Name = "cmbEquipZone";
            this.cmbEquipZone.Size = new System.Drawing.Size(287, 20);
            this.cmbEquipZone.TabIndex = 3;
            this.cmbEquipZone.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // cmbFloor
            // 
            this.cmbFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFloor.FormattingEnabled = true;
            this.cmbFloor.Location = new System.Drawing.Point(433, 10);
            this.cmbFloor.Name = "cmbFloor";
            this.cmbFloor.Size = new System.Drawing.Size(75, 20);
            this.cmbFloor.TabIndex = 2;
            this.cmbFloor.SelectedIndexChanged += new System.EventHandler(this.cmbFloor_SelectedIndexChanged);
            // 
            // cmbBuilding
            // 
            this.cmbBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuilding.FormattingEnabled = true;
            this.cmbBuilding.Location = new System.Drawing.Point(193, 10);
            this.cmbBuilding.Name = "cmbBuilding";
            this.cmbBuilding.Size = new System.Drawing.Size(217, 20);
            this.cmbBuilding.TabIndex = 1;
            this.cmbBuilding.SelectedIndexChanged += new System.EventHandler(this.cmbBuilding_SelectedIndexChanged);
            // 
            // btnCCTV
            // 
            this.btnCCTV.Location = new System.Drawing.Point(820, 4);
            this.btnCCTV.Name = "btnCCTV";
            this.btnCCTV.Size = new System.Drawing.Size(114, 28);
            this.btnCCTV.TabIndex = 0;
            this.btnCCTV.Text = "CCTV List 보기";
            this.btnCCTV.UseVisualStyleBackColor = true;
            this.btnCCTV.Click += new System.EventHandler(this.btnCCTV_Click);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 37);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(168, 677);
            this.panel2.TabIndex = 1;
            // 
            // panelCCTV
            // 
            this.panelCCTV.AutoScroll = true;
            this.panelCCTV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCCTV.Location = new System.Drawing.Point(168, 37);
            this.panelCCTV.Name = "panelCCTV";
            this.panelCCTV.Size = new System.Drawing.Size(1043, 677);
            this.panelCCTV.TabIndex = 2;
            // 
            // btnFill
            // 
            this.btnFill.Location = new System.Drawing.Point(1022, 2);
            this.btnFill.Name = "btnFill";
            this.btnFill.Size = new System.Drawing.Size(76, 34);
            this.btnFill.TabIndex = 6;
            this.btnFill.Text = "빈 CCTV 채워넣기";
            this.btnFill.UseVisualStyleBackColor = true;
            this.btnFill.Visible = false;
            this.btnFill.Click += new System.EventHandler(this.btnFill_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1211, 714);
            this.Controls.Add(this.panelCCTV);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FormMain";
            this.Text = "CCTV Checker";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panelCCTV;
        private System.Windows.Forms.Button btnCCTV;
        private System.Windows.Forms.ComboBox cmbBuilding;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbEquipZone;
        private System.Windows.Forms.ComboBox cmbFloor;
        private System.Windows.Forms.ComboBox cmbGroup;
        private System.Windows.Forms.Button btnFill;

    }
}

