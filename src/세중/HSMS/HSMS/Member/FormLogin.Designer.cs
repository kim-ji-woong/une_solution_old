namespace HSMS
{
    partial class FormLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            this.btnChangePwd = new UnE.GUI.RibbonButton();
            this.btnLogin = new UnE.GUI.RibbonButton();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            this.btnRegMember = new UnE.GUI.RibbonButton();
            this.btnSetup = new UnE.GUI.RibbonButton();
            this.SuspendLayout();
            // 
            // btnChangePwd
            // 
            this.btnChangePwd.BackColor = System.Drawing.Color.Transparent;
            this.btnChangePwd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChangePwd.BackgroundImage")));
            this.btnChangePwd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnChangePwd.CheckButton = false;
            this.btnChangePwd.CheckedBkgndImage = null;
            this.btnChangePwd.CheckedImage = null;
            this.btnChangePwd.ClickedBackgroundImage = null;
            this.btnChangePwd.ClickedImage = null;
            this.btnChangePwd.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnChangePwd.DisabledBkgndImage = null;
            this.btnChangePwd.DisabledImage = null;
            this.btnChangePwd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(214)))), ((int)(((byte)(214)))));
            this.btnChangePwd.ID = -1;
            this.btnChangePwd.InitButtonWidth = 135;
            this.btnChangePwd.IsChecked = false;
            this.btnChangePwd.Location = new System.Drawing.Point(312, 243);
            this.btnChangePwd.Margin = new System.Windows.Forms.Padding(4);
            this.btnChangePwd.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnChangePwd.MouseOverImage = null;
            this.btnChangePwd.Name = "btnChangePwd";
            this.btnChangePwd.NormalImage = null;
            this.btnChangePwd.Owner = null;
            this.btnChangePwd.Size = new System.Drawing.Size(135, 27);
            this.btnChangePwd.TabIndex = 28;
            this.btnChangePwd.Text = "비밀번호 찾기";
            this.btnChangePwd.TextLocation = new System.Drawing.Point(0, 5);
            this.btnChangePwd.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnChangePwd.ToolTipText = "비밀번호 찾기";
            this.btnChangePwd.UseCustomImageRect = false;
            this.btnChangePwd.UseTextLocation = true;
            this.btnChangePwd.UseVisualStyleBackColor = false;
            this.btnChangePwd.Click += new System.EventHandler(this.btnChangePwd_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.Transparent;
            this.btnLogin.BackgroundImage = global::HSMS.Properties.Resources.btnLogin;
            this.btnLogin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLogin.CheckButton = false;
            this.btnLogin.CheckedBkgndImage = null;
            this.btnLogin.CheckedImage = null;
            this.btnLogin.ClickedBackgroundImage = null;
            this.btnLogin.ClickedImage = null;
            this.btnLogin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnLogin.DisabledBkgndImage = null;
            this.btnLogin.DisabledImage = null;
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.ID = -1;
            this.btnLogin.InitButtonWidth = 83;
            this.btnLogin.IsChecked = false;
            this.btnLogin.Location = new System.Drawing.Point(371, 156);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(4);
            this.btnLogin.MouseOverBkgndImage = global::HSMS.Properties.Resources.btnLogin_over;
            this.btnLogin.MouseOverImage = null;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.NormalImage = null;
            this.btnLogin.Owner = null;
            this.btnLogin.Size = new System.Drawing.Size(83, 65);
            this.btnLogin.TabIndex = 24;
            this.btnLogin.Text = "로그인";
            this.btnLogin.TextLocation = new System.Drawing.Point(0, 23);
            this.btnLogin.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnLogin.ToolTipText = "로그인";
            this.btnLogin.UseCustomImageRect = false;
            this.btnLogin.UseTextLocation = true;
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxPassword.Location = new System.Drawing.Point(231, 192);
            this.textBoxPassword.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(134, 27);
            this.textBoxPassword.TabIndex = 24;
            this.textBoxPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxID
            // 
            this.textBoxID.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxID.Location = new System.Drawing.Point(231, 159);
            this.textBoxID.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(134, 27);
            this.textBoxID.TabIndex = 23;
            this.textBoxID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelPassword.Location = new System.Drawing.Point(156, 194);
            this.labelPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(69, 20);
            this.labelPassword.TabIndex = 25;
            this.labelPassword.Text = "비밀번호";
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.BackColor = System.Drawing.Color.Transparent;
            this.labelID.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelID.Location = new System.Drawing.Point(160, 158);
            this.labelID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(64, 20);
            this.labelID.TabIndex = 24;
            this.labelID.Text = "아 이 디";
            // 
            // btnRegMember
            // 
            this.btnRegMember.BackColor = System.Drawing.Color.Transparent;
            this.btnRegMember.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRegMember.BackgroundImage")));
            this.btnRegMember.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRegMember.CheckButton = false;
            this.btnRegMember.CheckedBkgndImage = null;
            this.btnRegMember.CheckedImage = null;
            this.btnRegMember.ClickedBackgroundImage = null;
            this.btnRegMember.ClickedImage = null;
            this.btnRegMember.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnRegMember.DisabledBkgndImage = null;
            this.btnRegMember.DisabledImage = null;
            this.btnRegMember.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(214)))), ((int)(((byte)(214)))));
            this.btnRegMember.ID = -1;
            this.btnRegMember.InitButtonWidth = 135;
            this.btnRegMember.IsChecked = false;
            this.btnRegMember.Location = new System.Drawing.Point(165, 243);
            this.btnRegMember.Margin = new System.Windows.Forms.Padding(4);
            this.btnRegMember.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnRegMember.MouseOverImage = null;
            this.btnRegMember.Name = "btnRegMember";
            this.btnRegMember.NormalImage = null;
            this.btnRegMember.Owner = null;
            this.btnRegMember.Size = new System.Drawing.Size(135, 27);
            this.btnRegMember.TabIndex = 27;
            this.btnRegMember.Text = "회원가입";
            this.btnRegMember.TextLocation = new System.Drawing.Point(0, 5);
            this.btnRegMember.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnRegMember.ToolTipText = "회원가입";
            this.btnRegMember.UseCustomImageRect = false;
            this.btnRegMember.UseTextLocation = true;
            this.btnRegMember.UseVisualStyleBackColor = false;
            this.btnRegMember.Click += new System.EventHandler(this.btnRegMember_Click);
            // 
            // btnSetup
            // 
            this.btnSetup.BackColor = System.Drawing.Color.Transparent;
            this.btnSetup.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSetup.BackgroundImage")));
            this.btnSetup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSetup.CheckButton = false;
            this.btnSetup.CheckedBkgndImage = null;
            this.btnSetup.CheckedImage = null;
            this.btnSetup.ClickedBackgroundImage = null;
            this.btnSetup.ClickedImage = null;
            this.btnSetup.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnSetup.DisabledBkgndImage = null;
            this.btnSetup.DisabledImage = null;
            this.btnSetup.ID = -1;
            this.btnSetup.InitButtonWidth = 40;
            this.btnSetup.IsChecked = false;
            this.btnSetup.Location = new System.Drawing.Point(110, 243);
            this.btnSetup.MouseOverBkgndImage = global::HSMS.Properties.Resources.btnSetting_over;
            this.btnSetup.MouseOverImage = null;
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.NormalImage = null;
            this.btnSetup.Owner = null;
            this.btnSetup.Size = new System.Drawing.Size(40, 27);
            this.btnSetup.TabIndex = 26;
            this.btnSetup.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSetup.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSetup.ToolTipText = "";
            this.btnSetup.UseCustomImageRect = false;
            this.btnSetup.UseTextLocation = false;
            this.btnSetup.UseVisualStyleBackColor = false;
            this.btnSetup.Click += new System.EventHandler(this.btnSetup_Click);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(595, 324);
            this.Controls.Add(this.btnSetup);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelID);
            this.Controls.Add(this.btnRegMember);
            this.Controls.Add(this.btnChangePwd);
            this.Controls.Add(this.btnLogin);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormLogin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLogin_FormClosing);
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.Shown += new System.EventHandler(this.FormLogin_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UnE.GUI.RibbonButton btnChangePwd;
        private UnE.GUI.RibbonButton btnLogin;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelID;
        private UnE.GUI.RibbonButton btnRegMember;
        private UnE.GUI.RibbonButton btnSetup;

    }
}