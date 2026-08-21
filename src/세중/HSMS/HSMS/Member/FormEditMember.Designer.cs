namespace HSMS
{
    partial class FormEditMember
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditMember));
            this.btnMemberDelete = new UnE.GUI.RibbonButton();
            this.btnEditMember = new UnE.GUI.RibbonButton();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.SuspendLayout();
            // 
            // btnMemberDelete
            // 
            this.btnMemberDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnMemberDelete.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMemberDelete.BackgroundImage")));
            this.btnMemberDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMemberDelete.CheckedBkgndImage = null;
            this.btnMemberDelete.CheckedImage = null;
            this.btnMemberDelete.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnMemberDelete.DisabledBkgndImage = null;
            this.btnMemberDelete.DisabledImage = null;
            this.btnMemberDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(214)))), ((int)(((byte)(214)))));
            this.btnMemberDelete.ID = -1;
            this.btnMemberDelete.InitButtonWidth = 125;
            this.btnMemberDelete.IsChecked = false;
            this.btnMemberDelete.Location = new System.Drawing.Point(104, 169);
            this.btnMemberDelete.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnMemberDelete.Name = "btnMemberDelete";
            this.btnMemberDelete.NormalImage = null;
            this.btnMemberDelete.Owner = null;
            this.btnMemberDelete.Size = new System.Drawing.Size(125, 34);
            this.btnMemberDelete.TabIndex = 0;
            this.btnMemberDelete.Text = "계정삭제";
            this.btnMemberDelete.TextLocation = new System.Drawing.Point(0, 8);
            this.btnMemberDelete.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnMemberDelete.UseCustomImageRect = false;
            this.btnMemberDelete.UseTextLocation = true;
            this.btnMemberDelete.UseVisualStyleBackColor = false;
            this.btnMemberDelete.Click += new System.EventHandler(this.btnMemberDelete_Click);
            // 
            // btnEditMember
            // 
            this.btnEditMember.BackColor = System.Drawing.Color.Transparent;
            this.btnEditMember.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnEditMember.BackgroundImage")));
            this.btnEditMember.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEditMember.CheckedBkgndImage = null;
            this.btnEditMember.CheckedImage = null;
            this.btnEditMember.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnEditMember.DisabledBkgndImage = null;
            this.btnEditMember.DisabledImage = null;
            this.btnEditMember.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(214)))), ((int)(((byte)(214)))));
            this.btnEditMember.ID = -1;
            this.btnEditMember.InitButtonWidth = 125;
            this.btnEditMember.IsChecked = false;
            this.btnEditMember.Location = new System.Drawing.Point(242, 169);
            this.btnEditMember.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnEditMember.Name = "btnEditMember";
            this.btnEditMember.NormalImage = null;
            this.btnEditMember.Owner = null;
            this.btnEditMember.Size = new System.Drawing.Size(125, 34);
            this.btnEditMember.TabIndex = 1;
            this.btnEditMember.Text = "비밀번호 변경";
            this.btnEditMember.TextLocation = new System.Drawing.Point(0, 8);
            this.btnEditMember.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnEditMember.UseCustomImageRect = false;
            this.btnEditMember.UseTextLocation = true;
            this.btnEditMember.UseVisualStyleBackColor = false;
            this.btnEditMember.Click += new System.EventHandler(this.btnEditMember_Click);
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
            this.btnCancel.InitButtonWidth = 125;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(381, 169);
            this.btnCancel.MouseOverBkgndImage = global::HSMS.Properties.Resources.btn_over;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = null;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(125, 34);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "취소";
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 8);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnCancel.UseCustomImageRect = false;
            this.btnCancel.UseTextLocation = true;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormEditMember
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::HSMS.Properties.Resources.LoginMain_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(601, 332);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEditMember);
            this.Controls.Add(this.btnMemberDelete);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormEditMember";
            this.Text = "FormEditMember";
            this.Load += new System.EventHandler(this.FormEditMember_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.RibbonButton btnMemberDelete;
        private UnE.GUI.RibbonButton btnEditMember;
        private UnE.GUI.RibbonButton btnCancel;

    }
}