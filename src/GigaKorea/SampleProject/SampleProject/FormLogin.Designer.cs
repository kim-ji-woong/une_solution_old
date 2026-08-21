
namespace SampleProject
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
            this.txtUserKey = new System.Windows.Forms.TextBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rbtnOK = new UnE.GUI.RibbonButton();
            this.rbtnCancel = new UnE.GUI.RibbonButton();
            this.SuspendLayout();
            // 
            // txtUserKey
            // 
            this.txtUserKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserKey.Location = new System.Drawing.Point(104, 146);
            this.txtUserKey.Name = "txtUserKey";
            this.txtUserKey.PasswordChar = '*';
            this.txtUserKey.Size = new System.Drawing.Size(320, 24);
            this.txtUserKey.TabIndex = 3;
            this.txtUserKey.Text = "spatial1234";
            // 
            // txtUserID
            // 
            this.txtUserID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserID.Location = new System.Drawing.Point(104, 98);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(320, 24);
            this.txtUserID.TabIndex = 2;
            this.txtUserID.Text = "user_spatial";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(29, 205);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "로그인 후 이용가능";
            // 
            // rbtnOK
            // 
            this.rbtnOK.BackColor = System.Drawing.Color.Transparent;
            this.rbtnOK.BackgroundImage = global::SampleProject.Properties.Resources.btnLogin;
            this.rbtnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbtnOK.CheckButton = false;
            this.rbtnOK.CheckedBkgndImage = null;
            this.rbtnOK.CheckedImage = null;
            this.rbtnOK.CheckedMouseOver = null;
            this.rbtnOK.ClickedBackgroundImage = null;
            this.rbtnOK.ClickedImage = null;
            this.rbtnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbtnOK.DisabledBkgndImage = null;
            this.rbtnOK.DisabledImage = null;
            this.rbtnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbtnOK.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnOK.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnOK.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnOK.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnOK.ForeColorsByTypeUse = false;
            this.rbtnOK.ID = -1;
            this.rbtnOK.InitButtonWidth = 60;
            this.rbtnOK.IsChecked = false;
            this.rbtnOK.Location = new System.Drawing.Point(295, 193);
            this.rbtnOK.MouseOverBkgndImage = null;
            this.rbtnOK.MouseOverImage = null;
            this.rbtnOK.Name = "rbtnOK";
            this.rbtnOK.NormalImage = null;
            this.rbtnOK.Owner = null;
            this.rbtnOK.Size = new System.Drawing.Size(60, 37);
            this.rbtnOK.TabIndex = 9;
            this.rbtnOK.TextLocation = new System.Drawing.Point(0, 3);
            this.rbtnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnOK.ToolTipText = "";
            this.rbtnOK.UseCustomImageRect = false;
            this.rbtnOK.UseTextLocation = true;
            this.rbtnOK.UseVisualStyleBackColor = false;
            this.rbtnOK.Click += new System.EventHandler(this.rbtnOK_Click);
            // 
            // rbtnCancel
            // 
            this.rbtnCancel.BackColor = System.Drawing.Color.Transparent;
            this.rbtnCancel.BackgroundImage = global::SampleProject.Properties.Resources.btnCancle;
            this.rbtnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbtnCancel.CheckButton = false;
            this.rbtnCancel.CheckedBkgndImage = null;
            this.rbtnCancel.CheckedImage = null;
            this.rbtnCancel.CheckedMouseOver = null;
            this.rbtnCancel.ClickedBackgroundImage = null;
            this.rbtnCancel.ClickedImage = null;
            this.rbtnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rbtnCancel.DisabledBkgndImage = null;
            this.rbtnCancel.DisabledImage = null;
            this.rbtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbtnCancel.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnCancel.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnCancel.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnCancel.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnCancel.ForeColorsByTypeUse = false;
            this.rbtnCancel.ID = -1;
            this.rbtnCancel.InitButtonWidth = 60;
            this.rbtnCancel.IsChecked = false;
            this.rbtnCancel.Location = new System.Drawing.Point(364, 193);
            this.rbtnCancel.MouseOverBkgndImage = null;
            this.rbtnCancel.MouseOverImage = null;
            this.rbtnCancel.Name = "rbtnCancel";
            this.rbtnCancel.NormalImage = null;
            this.rbtnCancel.Owner = null;
            this.rbtnCancel.Size = new System.Drawing.Size(60, 37);
            this.rbtnCancel.TabIndex = 10;
            this.rbtnCancel.TextLocation = new System.Drawing.Point(0, 3);
            this.rbtnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCancel.ToolTipText = "";
            this.rbtnCancel.UseCustomImageRect = false;
            this.rbtnCancel.UseTextLocation = true;
            this.rbtnCancel.UseVisualStyleBackColor = false;
            this.rbtnCancel.Click += new System.EventHandler(this.rbtnCancel_Click);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SampleProject.Properties.Resources.loginbackground;
            this.ClientSize = new System.Drawing.Size(470, 244);
            this.Controls.Add(this.txtUserKey);
            this.Controls.Add(this.rbtnCancel);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.rbtnOK);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "로그인";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtUserKey;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Label label1;
        private UnE.GUI.RibbonButton rbtnOK;
        private UnE.GUI.RibbonButton rbtnCancel;
    }
}