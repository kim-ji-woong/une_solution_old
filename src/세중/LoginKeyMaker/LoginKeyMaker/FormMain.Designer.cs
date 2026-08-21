namespace LoginKeyMaker
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDBName = new System.Windows.Forms.TextBox();
            this.btnCreateCode = new System.Windows.Forms.Button();
            this.textBoxCode = new System.Windows.Forms.TextBox();
            this.radioAdmin = new System.Windows.Forms.RadioButton();
            this.radioUser = new System.Windows.Forms.RadioButton();
            this.textMacAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnMacAddrDetail = new System.Windows.Forms.Button();
            this.btnFilePath = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioNew = new System.Windows.Forms.RadioButton();
            this.radioUpdate = new System.Windows.Forms.RadioButton();
            this.radioInsert = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "DB 이름";
            // 
            // textBoxDBName
            // 
            this.textBoxDBName.Location = new System.Drawing.Point(88, 6);
            this.textBoxDBName.Name = "textBoxDBName";
            this.textBoxDBName.Size = new System.Drawing.Size(171, 21);
            this.textBoxDBName.TabIndex = 1;
            // 
            // btnCreateCode
            // 
            this.btnCreateCode.Location = new System.Drawing.Point(14, 129);
            this.btnCreateCode.Name = "btnCreateCode";
            this.btnCreateCode.Size = new System.Drawing.Size(62, 23);
            this.btnCreateCode.TabIndex = 2;
            this.btnCreateCode.Text = "코드생성";
            this.btnCreateCode.UseVisualStyleBackColor = true;
            this.btnCreateCode.Click += new System.EventHandler(this.btnCreateCode_Click);
            // 
            // textBoxCode
            // 
            this.textBoxCode.Location = new System.Drawing.Point(88, 129);
            this.textBoxCode.Name = "textBoxCode";
            this.textBoxCode.Size = new System.Drawing.Size(171, 21);
            this.textBoxCode.TabIndex = 1;
            // 
            // radioAdmin
            // 
            this.radioAdmin.AutoSize = true;
            this.radioAdmin.Location = new System.Drawing.Point(12, 7);
            this.radioAdmin.Name = "radioAdmin";
            this.radioAdmin.Size = new System.Drawing.Size(87, 16);
            this.radioAdmin.TabIndex = 3;
            this.radioAdmin.TabStop = true;
            this.radioAdmin.Text = "관리자 계정";
            this.radioAdmin.UseVisualStyleBackColor = true;
            // 
            // radioUser
            // 
            this.radioUser.AutoSize = true;
            this.radioUser.Location = new System.Drawing.Point(107, 7);
            this.radioUser.Name = "radioUser";
            this.radioUser.Size = new System.Drawing.Size(75, 16);
            this.radioUser.TabIndex = 3;
            this.radioUser.TabStop = true;
            this.radioUser.Text = "일반 계정";
            this.radioUser.UseVisualStyleBackColor = true;
            // 
            // textMacAddress
            // 
            this.textMacAddress.Location = new System.Drawing.Point(88, 65);
            this.textMacAddress.Name = "textMacAddress";
            this.textMacAddress.Size = new System.Drawing.Size(171, 21);
            this.textMacAddress.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "MacAdrs";
            // 
            // btnMacAddrDetail
            // 
            this.btnMacAddrDetail.Location = new System.Drawing.Point(265, 64);
            this.btnMacAddrDetail.Name = "btnMacAddrDetail";
            this.btnMacAddrDetail.Size = new System.Drawing.Size(26, 23);
            this.btnMacAddrDetail.TabIndex = 6;
            this.btnMacAddrDetail.Text = "...";
            this.btnMacAddrDetail.UseVisualStyleBackColor = true;
            this.btnMacAddrDetail.Click += new System.EventHandler(this.btnMacAddrDetail_Click);
            // 
            // btnFilePath
            // 
            this.btnFilePath.Location = new System.Drawing.Point(265, 128);
            this.btnFilePath.Name = "btnFilePath";
            this.btnFilePath.Size = new System.Drawing.Size(26, 23);
            this.btnFilePath.TabIndex = 6;
            this.btnFilePath.Text = "...";
            this.btnFilePath.UseVisualStyleBackColor = true;
            this.btnFilePath.Click += new System.EventHandler(this.btnFilePath_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioAdmin);
            this.panel1.Controls.Add(this.radioUser);
            this.panel1.Location = new System.Drawing.Point(2, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(229, 26);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radioNew);
            this.panel2.Controls.Add(this.radioInsert);
            this.panel2.Controls.Add(this.radioUpdate);
            this.panel2.Location = new System.Drawing.Point(2, 94);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(289, 26);
            this.panel2.TabIndex = 7;
            // 
            // radioNew
            // 
            this.radioNew.AutoSize = true;
            this.radioNew.Location = new System.Drawing.Point(12, 7);
            this.radioNew.Name = "radioNew";
            this.radioNew.Size = new System.Drawing.Size(75, 16);
            this.radioNew.TabIndex = 3;
            this.radioNew.TabStop = true;
            this.radioNew.Text = "새로 생성";
            this.radioNew.UseVisualStyleBackColor = true;
            this.radioNew.CheckedChanged += new System.EventHandler(this.radioNew_CheckedChanged);
            // 
            // radioUpdate
            // 
            this.radioUpdate.AutoSize = true;
            this.radioUpdate.Location = new System.Drawing.Point(107, 7);
            this.radioUpdate.Name = "radioUpdate";
            this.radioUpdate.Size = new System.Drawing.Size(75, 16);
            this.radioUpdate.TabIndex = 3;
            this.radioUpdate.TabStop = true;
            this.radioUpdate.Text = "계정 수정";
            this.radioUpdate.UseVisualStyleBackColor = true;
            this.radioUpdate.CheckedChanged += new System.EventHandler(this.radioUpdate_CheckedChanged);
            // 
            // radioInsert
            // 
            this.radioInsert.AutoSize = true;
            this.radioInsert.Location = new System.Drawing.Point(207, 7);
            this.radioInsert.Name = "radioInsert";
            this.radioInsert.Size = new System.Drawing.Size(75, 16);
            this.radioInsert.TabIndex = 3;
            this.radioInsert.TabStop = true;
            this.radioInsert.Text = "계정 추가";
            this.radioInsert.UseVisualStyleBackColor = true;
            this.radioInsert.CheckedChanged += new System.EventHandler(this.radioInsert_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 161);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnFilePath);
            this.Controls.Add(this.btnMacAddrDetail);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textMacAddress);
            this.Controls.Add(this.btnCreateCode);
            this.Controls.Add(this.textBoxCode);
            this.Controls.Add(this.textBoxDBName);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "인증코드 생성기";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDBName;
        private System.Windows.Forms.Button btnCreateCode;
        private System.Windows.Forms.TextBox textBoxCode;
        private System.Windows.Forms.RadioButton radioAdmin;
        private System.Windows.Forms.RadioButton radioUser;
        private System.Windows.Forms.TextBox textMacAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnMacAddrDetail;
        private System.Windows.Forms.Button btnFilePath;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radioNew;
        private System.Windows.Forms.RadioButton radioInsert;
        private System.Windows.Forms.RadioButton radioUpdate;
    }
}

