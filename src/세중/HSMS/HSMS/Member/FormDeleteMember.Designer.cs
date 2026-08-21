namespace HSMS
{
    partial class FormDeleteMember
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDeleteMember));
            this.textBoxCurrentID = new System.Windows.Forms.TextBox();
            this.textBoxCurrentPassword = new System.Windows.Forms.TextBox();
            this.labelCurrentID = new System.Windows.Forms.Label();
            this.labelCurrentPassword = new System.Windows.Forms.Label();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnConfirm = new UnE.GUI.RibbonButton();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxCurrentID
            // 
            this.textBoxCurrentID.Location = new System.Drawing.Point(304, 169);
            this.textBoxCurrentID.Name = "textBoxCurrentID";
            this.textBoxCurrentID.Size = new System.Drawing.Size(105, 21);
            this.textBoxCurrentID.TabIndex = 0;
            this.textBoxCurrentID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxCurrentPassword
            // 
            this.textBoxCurrentPassword.Location = new System.Drawing.Point(304, 194);
            this.textBoxCurrentPassword.Name = "textBoxCurrentPassword";
            this.textBoxCurrentPassword.PasswordChar = '*';
            this.textBoxCurrentPassword.Size = new System.Drawing.Size(105, 21);
            this.textBoxCurrentPassword.TabIndex = 1;
            this.textBoxCurrentPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelCurrentID
            // 
            this.labelCurrentID.AutoSize = true;
            this.labelCurrentID.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrentID.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCurrentID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelCurrentID.Location = new System.Drawing.Point(187, 166);
            this.labelCurrentID.Name = "labelCurrentID";
            this.labelCurrentID.Size = new System.Drawing.Size(64, 20);
            this.labelCurrentID.TabIndex = 24;
            this.labelCurrentID.Text = "아 이 디";
            // 
            // labelCurrentPassword
            // 
            this.labelCurrentPassword.AutoSize = true;
            this.labelCurrentPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrentPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCurrentPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(123)))), ((int)(((byte)(162)))));
            this.labelCurrentPassword.Location = new System.Drawing.Point(187, 191);
            this.labelCurrentPassword.Name = "labelCurrentPassword";
            this.labelCurrentPassword.Size = new System.Drawing.Size(104, 20);
            this.labelCurrentPassword.TabIndex = 23;
            this.labelCurrentPassword.Text = "현재 비밀번호";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCancel.BackgroundImage")));
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(214)))), ((int)(((byte)(214)))));
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 105;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(302, 227);
            this.btnCancel.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = null;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(105, 34);
            this.btnCancel.TabIndex = 30;
            this.btnCancel.Text = "취소";
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 8);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
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
            this.btnConfirm.CheckedBkgndImage = null;
            this.btnConfirm.CheckedImage = null;
            this.btnConfirm.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnConfirm.DisabledBkgndImage = null;
            this.btnConfirm.DisabledImage = null;
            this.btnConfirm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(214)))), ((int)(((byte)(214)))));
            this.btnConfirm.ID = -1;
            this.btnConfirm.InitButtonWidth = 105;
            this.btnConfirm.IsChecked = false;
            this.btnConfirm.Location = new System.Drawing.Point(186, 227);
            this.btnConfirm.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.NormalImage = null;
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(105, 34);
            this.btnConfirm.TabIndex = 29;
            this.btnConfirm.Text = "확인";
            this.btnConfirm.TextLocation = new System.Drawing.Point(0, 8);
            this.btnConfirm.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
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
            this.label1.Location = new System.Drawing.Point(161, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(283, 20);
            this.label1.TabIndex = 24;
            this.label1.Text = "삭제할 아이디와 비밀번호를 입력하세요.";
            // 
            // FormDeleteMember
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::HSMS.Properties.Resources.LoginMain_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(602, 333);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.textBoxCurrentID);
            this.Controls.Add(this.textBoxCurrentPassword);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelCurrentID);
            this.Controls.Add(this.labelCurrentPassword);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDeleteMember";
            this.Text = "FormDeleteMember";
            this.Load += new System.EventHandler(this.FormDeleteMember_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxCurrentID;
        private System.Windows.Forms.TextBox textBoxCurrentPassword;
        private System.Windows.Forms.Label labelCurrentID;
        private System.Windows.Forms.Label labelCurrentPassword;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnConfirm;
        private System.Windows.Forms.Label label1;
    }
}