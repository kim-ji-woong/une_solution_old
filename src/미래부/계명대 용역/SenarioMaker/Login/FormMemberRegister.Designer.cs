namespace UnE.SenarioMaker
{
    partial class FormMemberRegister
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMemberRegister));
            this.btnOK = new UnE.GUI.RibbonButton();
            this.button2 = new UnE.GUI.RibbonButton();
            this.textBoxConfirmPassword = new System.Windows.Forms.TextBox();
            this.textBoxMemberID = new System.Windows.Forms.TextBox();
            this.textBoxMemberPassword = new System.Windows.Forms.TextBox();
            this.textBoxConfirmCode = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPath = new System.Windows.Forms.Button();
            this.labelisAdmin = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cboAsk = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxAnswer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnOK.BackgroundImage")));
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = null;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 115;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(174, 275);
            this.btnOK.MouseOverBkgndImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin2_over;
            this.btnOK.MouseOverImage = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = null;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(115, 33);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "확인";
            this.btnOK.TextLocation = new System.Drawing.Point(0, 6);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "확인";
            this.btnOK.UseCustomImageRect = false;
            this.btnOK.UseTextLocation = true;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.CheckButton = false;
            this.button2.CheckedBkgndImage = null;
            this.button2.CheckedImage = null;
            this.button2.ClickedBackgroundImage = null;
            this.button2.ClickedImage = null;
            this.button2.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.button2.DisabledBkgndImage = null;
            this.button2.DisabledImage = null;
            this.button2.ID = -1;
            this.button2.InitButtonWidth = 115;
            this.button2.IsChecked = false;
            this.button2.Location = new System.Drawing.Point(299, 275);
            this.button2.MouseOverBkgndImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin2_over;
            this.button2.MouseOverImage = null;
            this.button2.Name = "button2";
            this.button2.NormalImage = null;
            this.button2.Owner = null;
            this.button2.Size = new System.Drawing.Size(115, 33);
            this.button2.TabIndex = 8;
            this.button2.Text = "취소";
            this.button2.TextLocation = new System.Drawing.Point(0, 6);
            this.button2.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.button2.ToolTipText = "취소";
            this.button2.UseCustomImageRect = false;
            this.button2.UseTextLocation = true;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // textBoxConfirmPassword
            // 
            this.textBoxConfirmPassword.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxConfirmPassword.Location = new System.Drawing.Point(271, 167);
            this.textBoxConfirmPassword.Name = "textBoxConfirmPassword";
            this.textBoxConfirmPassword.Size = new System.Drawing.Size(143, 23);
            this.textBoxConfirmPassword.TabIndex = 4;
            this.textBoxConfirmPassword.UseSystemPasswordChar = true;
            this.textBoxConfirmPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxConfirmCode_KeyPress);
            // 
            // textBoxMemberID
            // 
            this.textBoxMemberID.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemberID.Location = new System.Drawing.Point(271, 105);
            this.textBoxMemberID.Name = "textBoxMemberID";
            this.textBoxMemberID.Size = new System.Drawing.Size(143, 23);
            this.textBoxMemberID.TabIndex = 2;
            this.textBoxMemberID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxConfirmCode_KeyPress);
            // 
            // textBoxMemberPassword
            // 
            this.textBoxMemberPassword.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemberPassword.Location = new System.Drawing.Point(271, 136);
            this.textBoxMemberPassword.Name = "textBoxMemberPassword";
            this.textBoxMemberPassword.Size = new System.Drawing.Size(143, 23);
            this.textBoxMemberPassword.TabIndex = 3;
            this.textBoxMemberPassword.UseSystemPasswordChar = true;
            this.textBoxMemberPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxConfirmCode_KeyPress);
            // 
            // textBoxConfirmCode
            // 
            this.textBoxConfirmCode.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxConfirmCode.Location = new System.Drawing.Point(271, 73);
            this.textBoxConfirmCode.Name = "textBoxConfirmCode";
            this.textBoxConfirmCode.Size = new System.Drawing.Size(143, 23);
            this.textBoxConfirmCode.TabIndex = 0;
            this.textBoxConfirmCode.TextChanged += new System.EventHandler(this.textBoxConfirmCode_TextChanged);
            this.textBoxConfirmCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxConfirmCode_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label5.Location = new System.Drawing.Point(162, 166);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 20);
            this.label5.TabIndex = 17;
            this.label5.Text = "비밀번호 확인";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label3.Location = new System.Drawing.Point(199, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 20);
            this.label3.TabIndex = 18;
            this.label3.Text = "아 이 디";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label4.Location = new System.Drawing.Point(196, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 20);
            this.label4.TabIndex = 15;
            this.label4.Text = "비밀번호";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label2.Location = new System.Drawing.Point(196, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 20);
            this.label2.TabIndex = 16;
            this.label2.Text = "인증코드";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 17.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(211, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 31);
            this.label1.TabIndex = 14;
            this.label1.Text = "시나리오 생성기";
            // 
            // btnPath
            // 
            this.btnPath.Location = new System.Drawing.Point(420, 73);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(51, 23);
            this.btnPath.TabIndex = 1;
            this.btnPath.Text = "...";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // labelisAdmin
            // 
            this.labelisAdmin.AutoSize = true;
            this.labelisAdmin.BackColor = System.Drawing.Color.Transparent;
            this.labelisAdmin.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelisAdmin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelisAdmin.Location = new System.Drawing.Point(253, 252);
            this.labelisAdmin.Name = "labelisAdmin";
            this.labelisAdmin.Size = new System.Drawing.Size(89, 20);
            this.labelisAdmin.TabIndex = 17;
            this.labelisAdmin.Text = "관리자 계정";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label6.Location = new System.Drawing.Point(224, 194);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 20);
            this.label6.TabIndex = 17;
            this.label6.Text = "질문";
            // 
            // cboAsk
            // 
            this.cboAsk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAsk.FormattingEnabled = true;
            this.cboAsk.Items.AddRange(new object[] {
            "내 보물 1호는? ",
            "내가 가장 좋아하는 캐릭터는?",
            "가장 감명 깊게 읽은 책은?",
            "초등학교 때 짝꿍 이름은?",
            "어머니의 고향은?",
            "가장 무서웠던 선생님 이름은?",
            "가장 기억에 남는 장소는?",
            "내가 존경하는 인물은?",
            "다시 태어나면 되고 싶은 것은?",
            "초등학교 시절 나의 꿈은?",
            "우리집 애완동물의 이름은?",
            "나의 출신 초등학교는?"});
            this.cboAsk.Location = new System.Drawing.Point(271, 197);
            this.cboAsk.Name = "cboAsk";
            this.cboAsk.Size = new System.Drawing.Size(200, 20);
            this.cboAsk.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label7.Location = new System.Drawing.Point(236, 223);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 20);
            this.label7.TabIndex = 17;
            this.label7.Text = "답";
            // 
            // textBoxAnswer
            // 
            this.textBoxAnswer.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxAnswer.Location = new System.Drawing.Point(271, 223);
            this.textBoxAnswer.Name = "textBoxAnswer";
            this.textBoxAnswer.Size = new System.Drawing.Size(143, 23);
            this.textBoxAnswer.TabIndex = 6;
            this.textBoxAnswer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxConfirmCode_KeyPress);
            // 
            // FormMemberRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UnE.SenarioMaker.Properties.Resources.LoginMain_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(603, 333);
            this.Controls.Add(this.cboAsk);
            this.Controls.Add(this.btnPath);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBoxAnswer);
            this.Controls.Add(this.textBoxConfirmPassword);
            this.Controls.Add(this.textBoxMemberID);
            this.Controls.Add(this.textBoxMemberPassword);
            this.Controls.Add(this.textBoxConfirmCode);
            this.Controls.Add(this.labelisAdmin);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMemberRegister";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormMemberRegister";
            this.Shown += new System.EventHandler(this.FormMemberRegister_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.RibbonButton button2;
        private System.Windows.Forms.TextBox textBoxConfirmPassword;
        private System.Windows.Forms.TextBox textBoxMemberID;
        private System.Windows.Forms.TextBox textBoxMemberPassword;
        private System.Windows.Forms.TextBox textBoxConfirmCode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.Label labelisAdmin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboAsk;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxAnswer;
    }
}