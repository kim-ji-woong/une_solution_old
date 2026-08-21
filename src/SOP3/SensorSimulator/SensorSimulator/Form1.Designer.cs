namespace SensorSimulator
{
    partial class Form1
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
            this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
            this.cboBuilding = new System.Windows.Forms.ComboBox();
            this.cboFloor = new System.Windows.Forms.ComboBox();
            this.cboSensorZone = new System.Windows.Forms.ComboBox();
            this.cboisSenser = new System.Windows.Forms.ComboBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.cboEquipZone = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.Location = new System.Drawing.Point(22, 35);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Size = new System.Drawing.Size(151, 20);
            this.cboBuildingGroup.TabIndex = 0;
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.Location = new System.Drawing.Point(189, 35);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Size = new System.Drawing.Size(228, 20);
            this.cboBuilding.TabIndex = 1;
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
            // 
            // cboFloor
            // 
            this.cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloor.FormattingEnabled = true;
            this.cboFloor.Location = new System.Drawing.Point(434, 35);
            this.cboFloor.Name = "cboFloor";
            this.cboFloor.Size = new System.Drawing.Size(78, 20);
            this.cboFloor.TabIndex = 2;
            this.cboFloor.SelectedIndexChanged += new System.EventHandler(this.cboFloor_SelectedIndexChanged);
            // 
            // cboSensorZone
            // 
            this.cboSensorZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSensorZone.FormattingEnabled = true;
            this.cboSensorZone.Location = new System.Drawing.Point(264, 166);
            this.cboSensorZone.Name = "cboSensorZone";
            this.cboSensorZone.Size = new System.Drawing.Size(121, 20);
            this.cboSensorZone.TabIndex = 3;
            this.cboSensorZone.SelectedIndexChanged += new System.EventHandler(this.cboSensorZone_SelectedIndexChanged);
            // 
            // cboisSenser
            // 
            this.cboisSenser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboisSenser.FormattingEnabled = true;
            this.cboisSenser.Location = new System.Drawing.Point(401, 166);
            this.cboisSenser.Name = "cboisSenser";
            this.cboisSenser.Size = new System.Drawing.Size(111, 20);
            this.cboisSenser.TabIndex = 4;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(401, 245);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(111, 23);
            this.btnSubmit.TabIndex = 5;
            this.btnSubmit.Text = "button1";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.button1_Click);
            // 
            // cboEquipZone
            // 
            this.cboEquipZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEquipZone.FormattingEnabled = true;
            this.cboEquipZone.Location = new System.Drawing.Point(189, 98);
            this.cboEquipZone.Name = "cboEquipZone";
            this.cboEquipZone.Size = new System.Drawing.Size(323, 20);
            this.cboEquipZone.TabIndex = 1;
            this.cboEquipZone.SelectedIndexChanged += new System.EventHandler(this.cboEquipZone_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 363);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.cboisSenser);
            this.Controls.Add(this.cboSensorZone);
            this.Controls.Add(this.cboFloor);
            this.Controls.Add(this.cboEquipZone);
            this.Controls.Add(this.cboBuilding);
            this.Controls.Add(this.cboBuildingGroup);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboBuildingGroup;
        private System.Windows.Forms.ComboBox cboBuilding;
        private System.Windows.Forms.ComboBox cboFloor;
        private System.Windows.Forms.ComboBox cboSensorZone;
        private System.Windows.Forms.ComboBox cboisSenser;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.ComboBox cboEquipZone;
    }
}

