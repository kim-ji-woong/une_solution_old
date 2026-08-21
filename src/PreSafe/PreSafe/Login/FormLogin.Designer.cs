namespace PreSafe
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new UnE.GUI.RibbonButton();
            this.button2 = new UnE.GUI.RibbonButton();
            this.btnRegMember = new UnE.GUI.RibbonButton();
            this.button4 = new UnE.GUI.RibbonButton();
            this.SuspendLayout();
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
            this.label1.TabIndex = 0;
            this.label1.Text = "시나리오 생성기";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label2.Location = new System.Drawing.Point(156, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "아 이 디";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label3.Location = new System.Drawing.Point(152, 183);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "비밀번호";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox1.Location = new System.Drawing.Point(231, 145);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(143, 27);
            this.textBox1.TabIndex = 3;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox2.Location = new System.Drawing.Point(231, 180);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(143, 27);
            this.textBox2.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImage = global::PreSafe.Properties.Resources.btnLogin;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
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
            this.button1.Location = new System.Drawing.Point(380, 145);
            this.button1.MouseOverBkgndImage = global::PreSafe.Properties.Resources.btnLogin_over;
            this.button1.MouseOverImage = null;
            this.button1.Name = "button1";
            this.button1.NormalImage = null;
            this.button1.Owner = null;
            this.button1.Size = new System.Drawing.Size(75, 62);
            this.button1.TabIndex = 5;
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
            this.button2.Location = new System.Drawing.Point(309, 230);
            this.button2.MouseOverBkgndImage = global::PreSafe.Properties.Resources.btnLogin2_over;
            this.button2.MouseOverImage = null;
            this.button2.Name = "button2";
            this.button2.NormalImage = null;
            this.button2.Owner = null;
            this.button2.Size = new System.Drawing.Size(138, 28);
            this.button2.TabIndex = 6;
            this.button2.Text = "비밀번호 찾기";
            this.button2.TextLocation = new System.Drawing.Point(0, 5);
            this.button2.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.button2.ToolTipText = "비밀번호 찾기";
            this.button2.UseCustomImageRect = false;
            this.button2.UseTextLocation = true;
            this.button2.UseVisualStyleBackColor = false;
            // 
            // btnRegMember
            // 
            this.btnRegMember.BackColor = System.Drawing.Color.Transparent;
            this.btnRegMember.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRegMember.BackgroundImage")));
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
            this.btnRegMember.Location = new System.Drawing.Point(157, 230);
            this.btnRegMember.MouseOverBkgndImage = global::PreSafe.Properties.Resources.btnLogin2_over;
            this.btnRegMember.MouseOverImage = null;
            this.btnRegMember.Name = "btnRegMember";
            this.btnRegMember.NormalImage = null;
            this.btnRegMember.Owner = null;
            this.btnRegMember.Size = new System.Drawing.Size(138, 28);
            this.btnRegMember.TabIndex = 7;
            this.btnRegMember.Text = "회원가입";
            this.btnRegMember.TextLocation = new System.Drawing.Point(0, 5);
            this.btnRegMember.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnRegMember.ToolTipText = "회원가입";
            this.btnRegMember.UseCustomImageRect = false;
            this.btnRegMember.UseTextLocation = true;
            this.btnRegMember.UseVisualStyleBackColor = false;
            this.btnRegMember.Click += new System.EventHandler(this.btnRegMember_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Transparent;
            this.button4.BackgroundImage = global::PreSafe.Properties.Resources.btnSetup;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.CheckedBkgndImage = null;
            this.button4.CheckedImage = null;
            this.button4.ClickedBackgroundImage = null;
            this.button4.ClickedImage = null;
            this.button4.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.button4.DisabledBkgndImage = null;
            this.button4.DisabledImage = null;
            this.button4.ID = -1;
            this.button4.InitButtonWidth = 40;
            this.button4.IsChecked = false;
            this.button4.Location = new System.Drawing.Point(97, 230);
            this.button4.MouseOverBkgndImage = global::PreSafe.Properties.Resources.btnSetting_over;
            this.button4.MouseOverImage = null;
            this.button4.Name = "button4";
            this.button4.NormalImage = null;
            this.button4.Owner = null;
            this.button4.Size = new System.Drawing.Size(40, 27);
            this.button4.TabIndex = 8;
            this.button4.TextLocation = new System.Drawing.Point(0, 0);
            this.button4.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.button4.ToolTipText = "";
            this.button4.UseCustomImageRect = false;
            this.button4.UseTextLocation = false;
            this.button4.UseVisualStyleBackColor = false;
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::PreSafe.Properties.Resources.LoginMain_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(605, 335);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnRegMember);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLogin";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormLogin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private UnE.GUI.RibbonButton button1;
        private UnE.GUI.RibbonButton button2;
        private UnE.GUI.RibbonButton btnRegMember;
        private UnE.GUI.RibbonButton button4;
    }
}