namespace KeyValidatorSample
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCertCode = new System.Windows.Forms.TextBox();
            this.btnCheckValidation = new System.Windows.Forms.Button();
            this.labelResult = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.radioAdmin = new System.Windows.Forms.RadioButton();
            this.radioNormal = new System.Windows.Forms.RadioButton();
            this.labelCode = new System.Windows.Forms.Label();
            this.textBoxIDCode = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "인증코드";
            // 
            // textBoxCertCode
            // 
            this.textBoxCertCode.Location = new System.Drawing.Point(71, 6);
            this.textBoxCertCode.Name = "textBoxCertCode";
            this.textBoxCertCode.Size = new System.Drawing.Size(201, 21);
            this.textBoxCertCode.TabIndex = 1;
            // 
            // btnCheckValidation
            // 
            this.btnCheckValidation.Location = new System.Drawing.Point(14, 48);
            this.btnCheckValidation.Name = "btnCheckValidation";
            this.btnCheckValidation.Size = new System.Drawing.Size(51, 23);
            this.btnCheckValidation.TabIndex = 2;
            this.btnCheckValidation.Text = "검증";
            this.btnCheckValidation.UseVisualStyleBackColor = true;
            this.btnCheckValidation.Click += new System.EventHandler(this.btnCheckValidation_Click);
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Location = new System.Drawing.Point(12, 84);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(57, 12);
            this.labelResult.TabIndex = 3;
            this.labelResult.Text = "검증 결과";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "계정 등급";
            // 
            // radioAdmin
            // 
            this.radioAdmin.AutoSize = true;
            this.radioAdmin.Enabled = false;
            this.radioAdmin.Location = new System.Drawing.Point(75, 108);
            this.radioAdmin.Name = "radioAdmin";
            this.radioAdmin.Size = new System.Drawing.Size(87, 16);
            this.radioAdmin.TabIndex = 4;
            this.radioAdmin.TabStop = true;
            this.radioAdmin.Text = "관리자 계정";
            this.radioAdmin.UseVisualStyleBackColor = true;
            // 
            // radioNormal
            // 
            this.radioNormal.AutoSize = true;
            this.radioNormal.Enabled = false;
            this.radioNormal.Location = new System.Drawing.Point(75, 130);
            this.radioNormal.Name = "radioNormal";
            this.radioNormal.Size = new System.Drawing.Size(115, 16);
            this.radioNormal.TabIndex = 4;
            this.radioNormal.TabStop = true;
            this.radioNormal.Text = "일반 사용자 계정";
            this.radioNormal.UseVisualStyleBackColor = true;
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.Location = new System.Drawing.Point(12, 156);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(57, 12);
            this.labelCode.TabIndex = 3;
            this.labelCode.Text = "계정 코드";
            // 
            // textBoxIDCode
            // 
            this.textBoxIDCode.Location = new System.Drawing.Point(75, 153);
            this.textBoxIDCode.Name = "textBoxIDCode";
            this.textBoxIDCode.ReadOnly = true;
            this.textBoxIDCode.Size = new System.Drawing.Size(197, 21);
            this.textBoxIDCode.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.textBoxIDCode);
            this.Controls.Add(this.radioNormal);
            this.Controls.Add(this.radioAdmin);
            this.Controls.Add(this.labelCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.btnCheckValidation);
            this.Controls.Add(this.textBoxCertCode);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCertCode;
        private System.Windows.Forms.Button btnCheckValidation;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioAdmin;
        private System.Windows.Forms.RadioButton radioNormal;
        private System.Windows.Forms.Label labelCode;
        private System.Windows.Forms.TextBox textBoxIDCode;
    }
}

