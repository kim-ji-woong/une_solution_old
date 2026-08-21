namespace UnE.SenarioMaker
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.button1 = new UnE.GUI.RibbonButton();
            this.button2 = new UnE.GUI.RibbonButton();
            this.btnRegMember = new UnE.GUI.RibbonButton();
            this.btnSetup = new UnE.GUI.RibbonButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 17.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(211, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 31);
            this.label1.TabIndex = 20;
            this.label1.Text = "시나리오 생성기";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label2.Location = new System.Drawing.Point(156, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "아 이 디";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label3.Location = new System.Drawing.Point(152, 180);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 20);
            this.label3.TabIndex = 22;
            this.label3.Text = "비밀번호";
            // 
            // textBoxID
            // 
            this.textBoxID.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxID.Location = new System.Drawing.Point(231, 142);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(143, 27);
            this.textBoxID.TabIndex = 0;
            this.textBoxID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxID_KeyPress);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxPassword.Location = new System.Drawing.Point(231, 177);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(143, 27);
            this.textBoxPassword.TabIndex = 1;
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxID_KeyPress);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.CheckButton = false;
            this.button1.CheckedBkgndImage = null;
            this.button1.CheckedImage = null;
            this.button1.ClickedBackgroundImage = null;
            this.button1.ClickedImage = null;
            this.button1.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.button1.DisabledBkgndImage = null;
            this.button1.DisabledImage = null;
            this.button1.ID = -1;
            this.button1.InitButtonWidth = 75;
            this.button1.IsChecked = false;
            this.button1.Location = new System.Drawing.Point(380, 142);
            this.button1.MouseOverBkgndImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin_over;
            this.button1.MouseOverImage = null;
            this.button1.Name = "button1";
            this.button1.NormalImage = null;
            this.button1.Owner = null;
            this.button1.Size = new System.Drawing.Size(75, 62);
            this.button1.TabIndex = 2;
            this.button1.Text = "로그인";
            this.button1.TextLocation = new System.Drawing.Point(0, 23);
            this.button1.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.button1.ToolTipText = "로그인";
            this.button1.UseCustomImageRect = false;
            this.button1.UseTextLocation = true;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            this.button2.InitButtonWidth = 138;
            this.button2.IsChecked = false;
            this.button2.Location = new System.Drawing.Point(309, 227);
            this.button2.MouseOverBkgndImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin2_over;
            this.button2.MouseOverImage = null;
            this.button2.Name = "button2";
            this.button2.NormalImage = null;
            this.button2.Owner = null;
            this.button2.Size = new System.Drawing.Size(138, 28);
            this.button2.TabIndex = 5;
            this.button2.Text = "비밀번호 찾기";
            this.button2.TextLocation = new System.Drawing.Point(0, 5);
            this.button2.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.button2.ToolTipText = "비밀번호 찾기";
            this.button2.UseCustomImageRect = false;
            this.button2.UseTextLocation = true;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnRegMember
            // 
            this.btnRegMember.BackColor = System.Drawing.Color.Transparent;
            this.btnRegMember.BackgroundImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin2;
            this.btnRegMember.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRegMember.CheckButton = false;
            this.btnRegMember.CheckedBkgndImage = null;
            this.btnRegMember.CheckedImage = null;
            this.btnRegMember.ClickedBackgroundImage = null;
            this.btnRegMember.ClickedImage = null;
            this.btnRegMember.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnRegMember.DisabledBkgndImage = null;
            this.btnRegMember.DisabledImage = null;
            this.btnRegMember.ID = -1;
            this.btnRegMember.InitButtonWidth = 138;
            this.btnRegMember.IsChecked = false;
            this.btnRegMember.Location = new System.Drawing.Point(157, 227);
            this.btnRegMember.MouseOverBkgndImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin2_over;
            this.btnRegMember.MouseOverImage = null;
            this.btnRegMember.Name = "btnRegMember";
            this.btnRegMember.NormalImage = null;
            this.btnRegMember.Owner = null;
            this.btnRegMember.Size = new System.Drawing.Size(138, 28);
            this.btnRegMember.TabIndex = 4;
            this.btnRegMember.Text = "사용자 등록";
            this.btnRegMember.TextLocation = new System.Drawing.Point(0, 5);
            this.btnRegMember.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnRegMember.ToolTipText = "사용자 등록";
            this.btnRegMember.UseCustomImageRect = false;
            this.btnRegMember.UseTextLocation = true;
            this.btnRegMember.UseVisualStyleBackColor = false;
            this.btnRegMember.Click += new System.EventHandler(this.btnRegMember_Click);
            // 
            // btnSetup
            // 
            this.btnSetup.BackColor = System.Drawing.Color.Transparent;
            this.btnSetup.BackgroundImage = global::UnE.SenarioMaker.Properties.Resources.btnSetup;
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
            this.btnSetup.Location = new System.Drawing.Point(97, 227);
            this.btnSetup.MouseOverBkgndImage = global::UnE.SenarioMaker.Properties.Resources.btnSetting_over;
            this.btnSetup.MouseOverImage = null;
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.NormalImage = null;
            this.btnSetup.Owner = null;
            this.btnSetup.Size = new System.Drawing.Size(40, 27);
            this.btnSetup.TabIndex = 3;
            this.btnSetup.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSetup.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSetup.ToolTipText = "";
            this.btnSetup.UseCustomImageRect = false;
            this.btnSetup.UseTextLocation = false;
            this.btnSetup.UseVisualStyleBackColor = false;
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UnE.SenarioMaker.Properties.Resources.LoginMain_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(605, 335);
            this.Controls.Add(this.btnSetup);
            this.Controls.Add(this.btnRegMember);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLogin";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormLogin";
            this.Shown += new System.EventHandler(this.FormLogin_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.TextBox textBoxPassword;
        private UnE.GUI.RibbonButton button1;
        private UnE.GUI.RibbonButton button2;
        private UnE.GUI.RibbonButton btnRegMember;
        private UnE.GUI.RibbonButton btnSetup;
    }
}