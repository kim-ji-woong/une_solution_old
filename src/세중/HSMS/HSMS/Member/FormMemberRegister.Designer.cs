namespace HSMS
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
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.textBoxMemberPassword = new System.Windows.Forms.TextBox();
            this.textBoxConfirmPassword = new System.Windows.Forms.TextBox();
            this.textBoxMemberID = new System.Windows.Forms.TextBox();
            this.labelMemberPassword = new System.Windows.Forms.Label();
            this.labelisAdmin = new System.Windows.Forms.Label();
            this.labelConfirmPassword = new System.Windows.Forms.Label();
            this.labelMemberID = new System.Windows.Forms.Label();
            this.textBoxConfirmCode = new System.Windows.Forms.TextBox();
            this.labelConfirmCode = new System.Windows.Forms.Label();
            this.btnConfirm = new UnE.GUI.RibbonButton();
            this.btnPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCancel.BackgroundImage")));
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
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
            this.btnCancel.Location = new System.Drawing.Point(303, 276);
            this.btnCancel.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnCancel.MouseOverImage = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = null;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(105, 34);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "취소";
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 8);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnCancel.ToolTipText = "취소";
            this.btnCancel.UseCustomImageRect = false;
            this.btnCancel.UseTextLocation = true;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // textBoxMemberPassword
            // 
            this.textBoxMemberPassword.Location = new System.Drawing.Point(311, 194);
            this.textBoxMemberPassword.Name = "textBoxMemberPassword";
            this.textBoxMemberPassword.PasswordChar = '*';
            this.textBoxMemberPassword.Size = new System.Drawing.Size(105, 21);
            this.textBoxMemberPassword.TabIndex = 33;
            this.textBoxMemberPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxConfirmPassword
            // 
            this.textBoxConfirmPassword.Location = new System.Drawing.Point(311, 222);
            this.textBoxConfirmPassword.Name = "textBoxConfirmPassword";
            this.textBoxConfirmPassword.PasswordChar = '*';
            this.textBoxConfirmPassword.Size = new System.Drawing.Size(105, 21);
            this.textBoxConfirmPassword.TabIndex = 34;
            this.textBoxConfirmPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxMemberID
            // 
            this.textBoxMemberID.Location = new System.Drawing.Point(311, 168);
            this.textBoxMemberID.Name = "textBoxMemberID";
            this.textBoxMemberID.Size = new System.Drawing.Size(105, 21);
            this.textBoxMemberID.TabIndex = 32;
            this.textBoxMemberID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelMemberPassword
            // 
            this.labelMemberPassword.AutoSize = true;
            this.labelMemberPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelMemberPassword.Location = new System.Drawing.Point(194, 192);
            this.labelMemberPassword.Name = "labelMemberPassword";
            this.labelMemberPassword.Size = new System.Drawing.Size(69, 20);
            this.labelMemberPassword.TabIndex = 30;
            this.labelMemberPassword.Text = "비밀번호";
            // 
            // labelisAdmin
            // 
            this.labelisAdmin.AutoSize = true;
            this.labelisAdmin.BackColor = System.Drawing.Color.Transparent;
            this.labelisAdmin.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelisAdmin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelisAdmin.Location = new System.Drawing.Point(257, 246);
            this.labelisAdmin.Name = "labelisAdmin";
            this.labelisAdmin.Size = new System.Drawing.Size(89, 20);
            this.labelisAdmin.TabIndex = 26;
            this.labelisAdmin.Text = "관리자 계정";
            // 
            // labelConfirmPassword
            // 
            this.labelConfirmPassword.AutoSize = true;
            this.labelConfirmPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelConfirmPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelConfirmPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelConfirmPassword.Location = new System.Drawing.Point(194, 219);
            this.labelConfirmPassword.Name = "labelConfirmPassword";
            this.labelConfirmPassword.Size = new System.Drawing.Size(104, 20);
            this.labelConfirmPassword.TabIndex = 29;
            this.labelConfirmPassword.Text = "비밀번호 확인";
            // 
            // labelMemberID
            // 
            this.labelMemberID.AutoSize = true;
            this.labelMemberID.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberID.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelMemberID.Location = new System.Drawing.Point(194, 165);
            this.labelMemberID.Name = "labelMemberID";
            this.labelMemberID.Size = new System.Drawing.Size(64, 20);
            this.labelMemberID.TabIndex = 28;
            this.labelMemberID.Text = "아 이 디";
            // 
            // textBoxConfirmCode
            // 
            this.textBoxConfirmCode.Location = new System.Drawing.Point(311, 143);
            this.textBoxConfirmCode.Name = "textBoxConfirmCode";
            this.textBoxConfirmCode.Size = new System.Drawing.Size(105, 21);
            this.textBoxConfirmCode.TabIndex = 31;
            this.textBoxConfirmCode.TextChanged += new System.EventHandler(this.textBoxConfirmCode_TextChanged);
            this.textBoxConfirmCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelConfirmCode
            // 
            this.labelConfirmCode.AutoSize = true;
            this.labelConfirmCode.BackColor = System.Drawing.Color.Transparent;
            this.labelConfirmCode.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelConfirmCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelConfirmCode.Location = new System.Drawing.Point(194, 140);
            this.labelConfirmCode.Name = "labelConfirmCode";
            this.labelConfirmCode.Size = new System.Drawing.Size(69, 20);
            this.labelConfirmCode.TabIndex = 27;
            this.labelConfirmCode.Text = "인증파일";
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.btnConfirm.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConfirm.BackgroundImage")));
            this.btnConfirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
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
            this.btnConfirm.Location = new System.Drawing.Point(187, 276);
            this.btnConfirm.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnConfirm.MouseOverImage = null;
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.NormalImage = null;
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(105, 34);
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "확인";
            this.btnConfirm.TextLocation = new System.Drawing.Point(0, 8);
            this.btnConfirm.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnConfirm.ToolTipText = "확인";
            this.btnConfirm.UseCustomImageRect = false;
            this.btnConfirm.UseTextLocation = true;
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnPath
            // 
            this.btnPath.Location = new System.Drawing.Point(418, 142);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(26, 23);
            this.btnPath.TabIndex = 35;
            this.btnPath.Text = "...";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // FormMemberRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::HSMS.Properties.Resources.LoginMain_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(601, 332);
            this.Controls.Add(this.btnPath);
            this.Controls.Add(this.textBoxMemberPassword);
            this.Controls.Add(this.textBoxConfirmPassword);
            this.Controls.Add(this.textBoxMemberID);
            this.Controls.Add(this.labelMemberPassword);
            this.Controls.Add(this.labelisAdmin);
            this.Controls.Add(this.labelConfirmPassword);
            this.Controls.Add(this.labelMemberID);
            this.Controls.Add(this.textBoxConfirmCode);
            this.Controls.Add(this.labelConfirmCode);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMemberRegister";
            this.Text = "FormMemberRegister";
            this.Load += new System.EventHandler(this.FormMemberRegister_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMemberPassword;
        private System.Windows.Forms.TextBox textBoxConfirmPassword;
        private System.Windows.Forms.TextBox textBoxMemberID;
        private System.Windows.Forms.Label labelMemberPassword;
        private System.Windows.Forms.Label labelisAdmin;
        private System.Windows.Forms.Label labelConfirmPassword;
        private System.Windows.Forms.Label labelMemberID;
        private System.Windows.Forms.TextBox textBoxConfirmCode;
        private System.Windows.Forms.Label labelConfirmCode;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnConfirm;
        private System.Windows.Forms.Button btnPath;
    }
}