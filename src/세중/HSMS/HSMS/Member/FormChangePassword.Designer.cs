namespace HSMS
{
    partial class FormChangePassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChangePassword));
            this.textBoxConfirmChanging = new System.Windows.Forms.TextBox();
            this.textBoxChangingPassword = new System.Windows.Forms.TextBox();
            this.labelConfirmChanging = new System.Windows.Forms.Label();
            this.labelChangingPassword = new System.Windows.Forms.Label();
            this.textBoxCurrentID = new System.Windows.Forms.TextBox();
            this.labelCurrentID = new System.Windows.Forms.Label();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnConfirm = new UnE.GUI.RibbonButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPath = new System.Windows.Forms.Button();
            this.textBoxConfirmCode = new System.Windows.Forms.TextBox();
            this.labelConfirmCode = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxConfirmChanging
            // 
            this.textBoxConfirmChanging.Location = new System.Drawing.Point(310, 224);
            this.textBoxConfirmChanging.Name = "textBoxConfirmChanging";
            this.textBoxConfirmChanging.PasswordChar = '*';
            this.textBoxConfirmChanging.Size = new System.Drawing.Size(105, 21);
            this.textBoxConfirmChanging.TabIndex = 31;
            this.textBoxConfirmChanging.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxChangingPassword
            // 
            this.textBoxChangingPassword.Location = new System.Drawing.Point(310, 199);
            this.textBoxChangingPassword.Name = "textBoxChangingPassword";
            this.textBoxChangingPassword.PasswordChar = '*';
            this.textBoxChangingPassword.Size = new System.Drawing.Size(105, 21);
            this.textBoxChangingPassword.TabIndex = 30;
            this.textBoxChangingPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelConfirmChanging
            // 
            this.labelConfirmChanging.AutoSize = true;
            this.labelConfirmChanging.BackColor = System.Drawing.Color.Transparent;
            this.labelConfirmChanging.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelConfirmChanging.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelConfirmChanging.Location = new System.Drawing.Point(193, 221);
            this.labelConfirmChanging.Name = "labelConfirmChanging";
            this.labelConfirmChanging.Size = new System.Drawing.Size(104, 20);
            this.labelConfirmChanging.TabIndex = 26;
            this.labelConfirmChanging.Text = "비밀번호 확인";
            // 
            // labelChangingPassword
            // 
            this.labelChangingPassword.AutoSize = true;
            this.labelChangingPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelChangingPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelChangingPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelChangingPassword.Location = new System.Drawing.Point(193, 196);
            this.labelChangingPassword.Name = "labelChangingPassword";
            this.labelChangingPassword.Size = new System.Drawing.Size(89, 20);
            this.labelChangingPassword.TabIndex = 24;
            this.labelChangingPassword.Text = "새 비밀번호";
            // 
            // textBoxCurrentID
            // 
            this.textBoxCurrentID.Location = new System.Drawing.Point(310, 174);
            this.textBoxCurrentID.Name = "textBoxCurrentID";
            this.textBoxCurrentID.Size = new System.Drawing.Size(105, 21);
            this.textBoxCurrentID.TabIndex = 28;
            this.textBoxCurrentID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelCurrentID
            // 
            this.labelCurrentID.AutoSize = true;
            this.labelCurrentID.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrentID.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCurrentID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelCurrentID.Location = new System.Drawing.Point(193, 171);
            this.labelCurrentID.Name = "labelCurrentID";
            this.labelCurrentID.Size = new System.Drawing.Size(64, 20);
            this.labelCurrentID.TabIndex = 25;
            this.labelCurrentID.Text = "아 이 디";
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
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(214)))), ((int)(((byte)(214)))));
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 105;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(308, 261);
            this.btnCancel.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnCancel.MouseOverImage = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = null;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(105, 34);
            this.btnCancel.TabIndex = 33;
            this.btnCancel.Text = "취소";
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 8);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnCancel.ToolTipText = "취소";
            this.btnCancel.UseCustomImageRect = false;
            this.btnCancel.UseTextLocation = true;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.btnConfirm.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConfirm.BackgroundImage")));
            this.btnConfirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnConfirm.CheckButton = false;
            this.btnConfirm.CheckedBkgndImage = null;
            this.btnConfirm.CheckedImage = null;
            this.btnConfirm.ClickedBackgroundImage = null;
            this.btnConfirm.ClickedImage = null;
            this.btnConfirm.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnConfirm.DisabledBkgndImage = null;
            this.btnConfirm.DisabledImage = null;
            this.btnConfirm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(214)))), ((int)(((byte)(214)))));
            this.btnConfirm.ID = -1;
            this.btnConfirm.InitButtonWidth = 105;
            this.btnConfirm.IsChecked = false;
            this.btnConfirm.Location = new System.Drawing.Point(192, 261);
            this.btnConfirm.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnConfirm.MouseOverImage = null;
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.NormalImage = null;
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(105, 34);
            this.btnConfirm.TabIndex = 32;
            this.btnConfirm.Text = "확인";
            this.btnConfirm.TextLocation = new System.Drawing.Point(0, 8);
            this.btnConfirm.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnConfirm.ToolTipText = "확인";
            this.btnConfirm.UseCustomImageRect = false;
            this.btnConfirm.UseTextLocation = true;
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.label1.Location = new System.Drawing.Point(45, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(513, 20);
            this.label1.TabIndex = 34;
            this.label1.Text = "아이디 생성시 사용한 인증파일을 이용하여 새로운 비밀번호를 입력하세요.";
            // 
            // btnPath
            // 
            this.btnPath.Location = new System.Drawing.Point(417, 148);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(26, 23);
            this.btnPath.TabIndex = 38;
            this.btnPath.Text = "...";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // textBoxConfirmCode
            // 
            this.textBoxConfirmCode.Location = new System.Drawing.Point(310, 149);
            this.textBoxConfirmCode.Name = "textBoxConfirmCode";
            this.textBoxConfirmCode.Size = new System.Drawing.Size(105, 21);
            this.textBoxConfirmCode.TabIndex = 37;
            this.textBoxConfirmCode.TextChanged += new System.EventHandler(this.textBoxConfirmCode_TextChanged);
            // 
            // labelConfirmCode
            // 
            this.labelConfirmCode.AutoSize = true;
            this.labelConfirmCode.BackColor = System.Drawing.Color.Transparent;
            this.labelConfirmCode.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelConfirmCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelConfirmCode.Location = new System.Drawing.Point(193, 146);
            this.labelConfirmCode.Name = "labelConfirmCode";
            this.labelConfirmCode.Size = new System.Drawing.Size(69, 20);
            this.labelConfirmCode.TabIndex = 36;
            this.labelConfirmCode.Text = "인증파일";
            // 
            // FormChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::HSMS.Properties.Resources.LoginMain_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(602, 332);
            this.Controls.Add(this.btnPath);
            this.Controls.Add(this.textBoxConfirmCode);
            this.Controls.Add(this.labelConfirmCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.textBoxConfirmChanging);
            this.Controls.Add(this.textBoxChangingPassword);
            this.Controls.Add(this.labelConfirmChanging);
            this.Controls.Add(this.labelChangingPassword);
            this.Controls.Add(this.textBoxCurrentID);
            this.Controls.Add(this.labelCurrentID);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormChangePassword";
            this.Text = "FormChangePassword";
            this.Load += new System.EventHandler(this.FormChangePassword_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxConfirmChanging;
        private System.Windows.Forms.TextBox textBoxChangingPassword;
        private System.Windows.Forms.Label labelConfirmChanging;
        private System.Windows.Forms.Label labelChangingPassword;
        private System.Windows.Forms.TextBox textBoxCurrentID;
        private System.Windows.Forms.Label labelCurrentID;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnConfirm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.TextBox textBoxConfirmCode;
        private System.Windows.Forms.Label labelConfirmCode;
    }
}