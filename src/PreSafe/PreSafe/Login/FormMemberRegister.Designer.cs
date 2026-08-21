namespace PreSafe
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
            this.btnOK = new UnE.GUI.RibbonButton();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.SuspendLayout();
            // 
            // textBoxConfirmPassword
            // 
            this.textBoxConfirmPassword.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxConfirmPassword.Location = new System.Drawing.Point(271, 205);
            this.textBoxConfirmPassword.Name = "textBoxConfirmPassword";
            this.textBoxConfirmPassword.Size = new System.Drawing.Size(143, 23);
            this.textBoxConfirmPassword.TabIndex = 5;
            this.textBoxConfirmPassword.UseSystemPasswordChar = true;
            // 
            // textBoxMemberID
            // 
            this.textBoxMemberID.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemberID.Location = new System.Drawing.Point(271, 143);
            this.textBoxMemberID.Name = "textBoxMemberID";
            this.textBoxMemberID.Size = new System.Drawing.Size(143, 23);
            this.textBoxMemberID.TabIndex = 3;
            // 
            // textBoxMemberPassword
            // 
            this.textBoxMemberPassword.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemberPassword.Location = new System.Drawing.Point(271, 174);
            this.textBoxMemberPassword.Name = "textBoxMemberPassword";
            this.textBoxMemberPassword.Size = new System.Drawing.Size(143, 23);
            this.textBoxMemberPassword.TabIndex = 4;
            this.textBoxMemberPassword.UseSystemPasswordChar = true;
            // 
            // textBoxConfirmCode
            // 
            this.textBoxConfirmCode.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxConfirmCode.Location = new System.Drawing.Point(271, 111);
            this.textBoxConfirmCode.Name = "textBoxConfirmCode";
            this.textBoxConfirmCode.Size = new System.Drawing.Size(143, 23);
            this.textBoxConfirmCode.TabIndex = 1;
            this.textBoxConfirmCode.TextChanged += new System.EventHandler(this.textBoxConfirmCode_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label5.Location = new System.Drawing.Point(162, 204);
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
            this.label3.Location = new System.Drawing.Point(199, 143);
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
            this.label4.Location = new System.Drawing.Point(196, 173);
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
            this.label2.Location = new System.Drawing.Point(196, 112);
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
            this.label1.Location = new System.Drawing.Point(211, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 31);
            this.label1.TabIndex = 14;
            this.label1.Text = "시나리오 생성기";
            // 
            // btnPath
            // 
            this.btnPath.Location = new System.Drawing.Point(420, 113);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(47, 23);
            this.btnPath.TabIndex = 2;
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
            this.labelisAdmin.Location = new System.Drawing.Point(266, 234);
            this.labelisAdmin.Name = "labelisAdmin";
            this.labelisAdmin.Size = new System.Drawing.Size(74, 20);
            this.labelisAdmin.TabIndex = 17;
            this.labelisAdmin.Text = "일반 계정";
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
            this.btnOK.Location = new System.Drawing.Point(174, 259);
            this.btnOK.MouseOverBkgndImage = global::PreSafe.Properties.Resources.btnLogin2_over;
            this.btnOK.MouseOverImage = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = null;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(115, 33);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "확인";
            this.btnOK.TextLocation = new System.Drawing.Point(0, 6);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "확인";
            this.btnOK.UseCustomImageRect = false;
            this.btnOK.UseTextLocation = true;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCancel.BackgroundImage")));
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = null;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 115;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(299, 259);
            this.btnCancel.MouseOverBkgndImage = global::PreSafe.Properties.Resources.btnLogin2_over;
            this.btnCancel.MouseOverImage = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = null;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(115, 33);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "취소";
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 6);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancel.ToolTipText = "취소";
            this.btnCancel.UseCustomImageRect = false;
            this.btnCancel.UseTextLocation = true;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormMemberRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::PreSafe.Properties.Resources.LoginMain_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(603, 333);
            this.Controls.Add(this.btnPath);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.textBoxConfirmPassword);
            this.Controls.Add(this.textBoxMemberID);
            this.Controls.Add(this.textBoxMemberPassword);
            this.Controls.Add(this.textBoxConfirmCode);
            this.Controls.Add(this.labelisAdmin);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.RibbonButton btnCancel;
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
    }
}