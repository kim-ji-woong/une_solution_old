namespace BIMViewer.uControl
{
    partial class uBuilding
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtBuilding = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbBuildingType = new System.Windows.Forms.ComboBox();
            this.lbSafetyFire = new System.Windows.Forms.Label();
            this.cmbPilotiType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtBuilding
            // 
            this.txtBuilding.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtBuilding.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBuilding.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtBuilding.ForeColor = System.Drawing.SystemColors.Window;
            this.txtBuilding.Location = new System.Drawing.Point(84, 21);
            this.txtBuilding.Name = "txtBuilding";
            this.txtBuilding.ReadOnly = true;
            this.txtBuilding.Size = new System.Drawing.Size(120, 18);
            this.txtBuilding.TabIndex = 41;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(30, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 40;
            this.label3.Text = "건물명";
            // 
            // cmbBuildingType
            // 
            this.cmbBuildingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildingType.FormattingEnabled = true;
            this.cmbBuildingType.Items.AddRange(new object[] {
            "철근콘크리트구조",
            "목구조",
            "철골구조",
            "조적구조"});
            this.cmbBuildingType.Location = new System.Drawing.Point(84, 52);
            this.cmbBuildingType.Name = "cmbBuildingType";
            this.cmbBuildingType.Size = new System.Drawing.Size(120, 20);
            this.cmbBuildingType.TabIndex = 43;
            // 
            // lbSafetyFire
            // 
            this.lbSafetyFire.AutoSize = true;
            this.lbSafetyFire.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbSafetyFire.Location = new System.Drawing.Point(30, 55);
            this.lbSafetyFire.Name = "lbSafetyFire";
            this.lbSafetyFire.Size = new System.Drawing.Size(51, 13);
            this.lbSafetyFire.TabIndex = 42;
            this.lbSafetyFire.Text = "건물구조";
            // 
            // cmbPilotiType
            // 
            this.cmbPilotiType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPilotiType.FormattingEnabled = true;
            this.cmbPilotiType.Items.AddRange(new object[] {
            "아니오",
            "예"});
            this.cmbPilotiType.Location = new System.Drawing.Point(313, 20);
            this.cmbPilotiType.Name = "cmbPilotiType";
            this.cmbPilotiType.Size = new System.Drawing.Size(58, 20);
            this.cmbPilotiType.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(234, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "필로티 유무";
            // 
            // uBuilding
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbPilotiType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbBuildingType);
            this.Controls.Add(this.lbSafetyFire);
            this.Controls.Add(this.txtBuilding);
            this.Controls.Add(this.label3);
            this.Name = "uBuilding";
            this.Size = new System.Drawing.Size(420, 90);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBuilding;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbBuildingType;
        private System.Windows.Forms.Label lbSafetyFire;
        private System.Windows.Forms.ComboBox cmbPilotiType;
        private System.Windows.Forms.Label label1;
    }
}
