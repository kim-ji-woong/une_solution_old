namespace BIMViewer.uControl
{
    partial class uSpace
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
            this.cmbYN = new System.Windows.Forms.ComboBox();
            this.lbSafetyFire = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFloor = new System.Windows.Forms.Label();
            this.txtSpaceName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtObject = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtRoomType = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbStairType = new System.Windows.Forms.ComboBox();
            this.lbStairType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbYN
            // 
            this.cmbYN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYN.FormattingEnabled = true;
            this.cmbYN.Items.AddRange(new object[] {
            "아니오",
            "예"});
            this.cmbYN.Location = new System.Drawing.Point(720, 31);
            this.cmbYN.Name = "cmbYN";
            this.cmbYN.Size = new System.Drawing.Size(58, 20);
            this.cmbYN.TabIndex = 38;
            // 
            // lbSafetyFire
            // 
            this.lbSafetyFire.AutoSize = true;
            this.lbSafetyFire.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbSafetyFire.Location = new System.Drawing.Point(641, 34);
            this.lbSafetyFire.Name = "lbSafetyFire";
            this.lbSafetyFire.Size = new System.Drawing.Size(73, 13);
            this.lbSafetyFire.TabIndex = 37;
            this.lbSafetyFire.Text = "방화구역유무";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(445, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "공간명";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(85, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Object";
            // 
            // lblFloor
            // 
            this.lblFloor.AutoSize = true;
            this.lblFloor.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblFloor.Location = new System.Drawing.Point(30, 42);
            this.lblFloor.Name = "lblFloor";
            this.lblFloor.Size = new System.Drawing.Size(0, 17);
            this.lblFloor.TabIndex = 28;
            // 
            // txtSpaceName
            // 
            this.txtSpaceName.BackColor = System.Drawing.SystemColors.InfoText;
            this.txtSpaceName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSpaceName.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtSpaceName.ForeColor = System.Drawing.SystemColors.Menu;
            this.txtSpaceName.Location = new System.Drawing.Point(491, 51);
            this.txtSpaceName.Name = "txtSpaceName";
            this.txtSpaceName.Size = new System.Drawing.Size(128, 25);
            this.txtSpaceName.TabIndex = 27;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(28, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Floor";
            // 
            // txtObject
            // 
            this.txtObject.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtObject.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtObject.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtObject.ForeColor = System.Drawing.SystemColors.Window;
            this.txtObject.Location = new System.Drawing.Point(132, 32);
            this.txtObject.Name = "txtObject";
            this.txtObject.ReadOnly = true;
            this.txtObject.Size = new System.Drawing.Size(120, 18);
            this.txtObject.TabIndex = 39;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(264, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 13);
            this.label4.TabIndex = 41;
            this.label4.Text = "ID";
            // 
            // txtID
            // 
            this.txtID.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtID.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtID.ForeColor = System.Drawing.SystemColors.Window;
            this.txtID.Location = new System.Drawing.Point(289, 32);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(140, 18);
            this.txtID.TabIndex = 42;
            // 
            // txtRoomType
            // 
            this.txtRoomType.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtRoomType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRoomType.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtRoomType.ForeColor = System.Drawing.SystemColors.Window;
            this.txtRoomType.Location = new System.Drawing.Point(491, 19);
            this.txtRoomType.Name = "txtRoomType";
            this.txtRoomType.ReadOnly = true;
            this.txtRoomType.Size = new System.Drawing.Size(128, 18);
            this.txtRoomType.TabIndex = 44;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(445, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 43;
            this.label5.Text = "실종류";
            // 
            // cmbStairType
            // 
            this.cmbStairType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStairType.FormattingEnabled = true;
            this.cmbStairType.Items.AddRange(new object[] {
            "일반계단",
            "피난계단",
            "특별피난계단"});
            this.cmbStairType.Location = new System.Drawing.Point(720, 53);
            this.cmbStairType.Name = "cmbStairType";
            this.cmbStairType.Size = new System.Drawing.Size(100, 20);
            this.cmbStairType.TabIndex = 46;
            this.cmbStairType.Visible = false;
            // 
            // lbStairType
            // 
            this.lbStairType.AutoSize = true;
            this.lbStairType.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStairType.Location = new System.Drawing.Point(641, 56);
            this.lbStairType.Name = "lbStairType";
            this.lbStairType.Size = new System.Drawing.Size(66, 13);
            this.lbStairType.TabIndex = 45;
            this.lbStairType.Text = "계단실 종류";
            this.lbStairType.Visible = false;
            // 
            // uSpace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbStairType);
            this.Controls.Add(this.lbStairType);
            this.Controls.Add(this.txtRoomType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtObject);
            this.Controls.Add(this.cmbYN);
            this.Controls.Add(this.lbSafetyFire);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblFloor);
            this.Controls.Add(this.txtSpaceName);
            this.Controls.Add(this.label1);
            this.Name = "uSpace";
            this.Size = new System.Drawing.Size(840, 90);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbYN;
        private System.Windows.Forms.Label lbSafetyFire;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblFloor;
        private System.Windows.Forms.TextBox txtSpaceName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtObject;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtRoomType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbStairType;
        private System.Windows.Forms.Label lbStairType;
    }
}
