namespace BIMViewer
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtUserKey = new System.Windows.Forms.TextBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rbtnOK = new UnE.GUI.RibbonButton();
            this.label4 = new System.Windows.Forms.Label();
            this.rbtnCancel = new UnE.GUI.RibbonButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::BIMViewer.Properties.Resources.loginPanel;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.txtUserKey);
            this.panel1.Controls.Add(this.txtUserID);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(25, 86);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(439, 105);
            this.panel1.TabIndex = 2;
            // 
            // txtUserKey
            // 
            this.txtUserKey.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUserKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserKey.Location = new System.Drawing.Point(119, 70);
            this.txtUserKey.Name = "txtUserKey";
            this.txtUserKey.PasswordChar = '*';
            this.txtUserKey.Size = new System.Drawing.Size(289, 17);
            this.txtUserKey.TabIndex = 1;
            // 
            // txtUserID
            // 
            this.txtUserID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUserID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserID.Location = new System.Drawing.Point(119, 16);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(289, 17);
            this.txtUserID.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(19, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(19, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "User ID";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::BIMViewer.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(104, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(45, 46);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(161, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(208, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fire Safety Manager";
            // 
            // rbtnOK
            // 
            this.rbtnOK.BackColor = System.Drawing.Color.Transparent;
            this.rbtnOK.BackgroundImage = global::BIMViewer.Properties.Resources.btnBackground;
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
            this.rbtnOK.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnOK.ForeColorsByTypeUse = false;
            this.rbtnOK.ID = -1;
            this.rbtnOK.InitButtonWidth = 60;
            this.rbtnOK.IsChecked = false;
            this.rbtnOK.Location = new System.Drawing.Point(329, 199);
            this.rbtnOK.MouseOverBkgndImage = null;
            this.rbtnOK.MouseOverImage = null;
            this.rbtnOK.Name = "rbtnOK";
            this.rbtnOK.NormalImage = null;
            this.rbtnOK.Owner = null;
            this.rbtnOK.Size = new System.Drawing.Size(60, 24);
            this.rbtnOK.TabIndex = 0;
            this.rbtnOK.Text = "Login";
            this.rbtnOK.TextLocation = new System.Drawing.Point(0, 3);
            this.rbtnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnOK.ToolTipText = "Login";
            this.rbtnOK.UseCustomImageRect = false;
            this.rbtnOK.UseTextLocation = true;
            this.rbtnOK.UseVisualStyleBackColor = false;
            this.rbtnOK.Click += new System.EventHandler(this.RbtnOK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label4.Location = new System.Drawing.Point(24, 205);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "로그인 후 업로드 가능";
            // 
            // rbtnCancel
            // 
            this.rbtnCancel.BackColor = System.Drawing.Color.Transparent;
            this.rbtnCancel.BackgroundImage = global::BIMViewer.Properties.Resources.btnBackground;
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
            this.rbtnCancel.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnCancel.ForeColorsByTypeUse = false;
            this.rbtnCancel.ID = -1;
            this.rbtnCancel.InitButtonWidth = 60;
            this.rbtnCancel.IsChecked = false;
            this.rbtnCancel.Location = new System.Drawing.Point(404, 199);
            this.rbtnCancel.MouseOverBkgndImage = null;
            this.rbtnCancel.MouseOverImage = null;
            this.rbtnCancel.Name = "rbtnCancel";
            this.rbtnCancel.NormalImage = null;
            this.rbtnCancel.Owner = null;
            this.rbtnCancel.Size = new System.Drawing.Size(60, 24);
            this.rbtnCancel.TabIndex = 1;
            this.rbtnCancel.Text = "Cancel";
            this.rbtnCancel.TextLocation = new System.Drawing.Point(0, 3);
            this.rbtnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCancel.ToolTipText = "Cancel";
            this.rbtnCancel.UseCustomImageRect = false;
            this.rbtnCancel.UseTextLocation = true;
            this.rbtnCancel.UseVisualStyleBackColor = false;
            this.rbtnCancel.Click += new System.EventHandler(this.RbtnCancel_Click);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::BIMViewer.Properties.Resources.background;
            this.ClientSize = new System.Drawing.Size(495, 232);
            this.Controls.Add(this.rbtnCancel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rbtnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLogin";
            this.Text = "FormLogin";
            this.Load += new System.EventHandler(this.FormLoginLoad);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormLogin_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormLogin_MouseMove);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUserKey;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Label label3;
        private UnE.GUI.RibbonButton rbtnOK;
        private System.Windows.Forms.Label label4;
        private UnE.GUI.RibbonButton rbtnCancel;
    }
}