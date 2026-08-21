namespace CrisisAlertTester
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbFacilityType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSensor = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbRiskLevel = new System.Windows.Forms.ComboBox();
            this.lbRiskLevel = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbFacilityType
            // 
            this.cmbFacilityType.BackColor = System.Drawing.SystemColors.Window;
            this.cmbFacilityType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFacilityType.FormattingEnabled = true;
            this.cmbFacilityType.Location = new System.Drawing.Point(23, 38);
            this.cmbFacilityType.Name = "cmbFacilityType";
            this.cmbFacilityType.Size = new System.Drawing.Size(121, 20);
            this.cmbFacilityType.TabIndex = 1;
            this.cmbFacilityType.SelectedValueChanged += new System.EventHandler(this.cmbFacilityType_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "신호 타입";
            // 
            // cmbSensor
            // 
            this.cmbSensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSensor.FormattingEnabled = true;
            this.cmbSensor.Location = new System.Drawing.Point(23, 90);
            this.cmbSensor.Name = "cmbSensor";
            this.cmbSensor.Size = new System.Drawing.Size(171, 20);
            this.cmbSensor.TabIndex = 3;
            this.cmbSensor.SelectedValueChanged += new System.EventHandler(this.cmbSensor_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "센서 종류";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "위기경보 단계 조정";
            // 
            // cmbRiskLevel
            // 
            this.cmbRiskLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRiskLevel.FormattingEnabled = true;
            this.cmbRiskLevel.Location = new System.Drawing.Point(23, 144);
            this.cmbRiskLevel.Name = "cmbRiskLevel";
            this.cmbRiskLevel.Size = new System.Drawing.Size(171, 20);
            this.cmbRiskLevel.TabIndex = 5;
            // 
            // lbRiskLevel
            // 
            this.lbRiskLevel.AutoSize = true;
            this.lbRiskLevel.Location = new System.Drawing.Point(213, 93);
            this.lbRiskLevel.Name = "lbRiskLevel";
            this.lbRiskLevel.Size = new System.Drawing.Size(57, 12);
            this.lbRiskLevel.TabIndex = 7;
            this.lbRiskLevel.Text = "센서 상태";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(23, 196);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(87, 22);
            this.btnApply.TabIndex = 8;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 236);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lbRiskLevel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbRiskLevel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSensor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbFacilityType);
            this.Name = "FormMain";
            this.Text = "센서 테스터";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbFacilityType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSensor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbRiskLevel;
        private System.Windows.Forms.Label lbRiskLevel;
        private System.Windows.Forms.Button btnApply;
    }
}

