namespace IntegratedManagement4.PopupDialog
{
    partial class SetOption
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
            this.textBoxMemberName = new System.Windows.Forms.TextBox();
            this.labelMemberName = new System.Windows.Forms.Label();
            this.textBoxMemberID = new System.Windows.Forms.TextBox();
            this.labelMemberID = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ribbonButton2 = new UnE.GUI.RibbonButton();
            this.btn_ok = new UnE.GUI.RibbonButton();
            this.btn_cancel = new UnE.GUI.RibbonButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxMemberName
            // 
            this.textBoxMemberName.Font = new System.Drawing.Font(Program.prgFont, 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemberName.Location = new System.Drawing.Point(120, 80);
            this.textBoxMemberName.Name = "textBoxMemberName";
            this.textBoxMemberName.Size = new System.Drawing.Size(147, 29);
            this.textBoxMemberName.TabIndex = 9;
            // 
            // labelMemberName
            // 
            this.labelMemberName.AutoSize = true;
            this.labelMemberName.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberName.Font = new System.Drawing.Font(Program.prgFont, 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberName.ForeColor = System.Drawing.Color.White;
            this.labelMemberName.Location = new System.Drawing.Point(25, 79);
            this.labelMemberName.Name = "labelMemberName";
            this.labelMemberName.Size = new System.Drawing.Size(44, 21);
            this.labelMemberName.TabIndex = 6;
            this.labelMemberName.Text = "이름";
            // 
            // textBoxMemberID
            // 
            this.textBoxMemberID.Font = new System.Drawing.Font(Program.prgFont, 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemberID.Location = new System.Drawing.Point(120, 47);
            this.textBoxMemberID.Name = "textBoxMemberID";
            this.textBoxMemberID.Size = new System.Drawing.Size(147, 29);
            this.textBoxMemberID.TabIndex = 8;
            // 
            // labelMemberID
            // 
            this.labelMemberID.AutoSize = true;
            this.labelMemberID.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberID.Font = new System.Drawing.Font(Program.prgFont, 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberID.ForeColor = System.Drawing.Color.White;
            this.labelMemberID.Location = new System.Drawing.Point(11, 50);
            this.labelMemberID.Name = "labelMemberID";
            this.labelMemberID.Size = new System.Drawing.Size(78, 21);
            this.labelMemberID.TabIndex = 7;
            this.labelMemberID.Text = "사원번호";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 18);
            this.label1.TabIndex = 13;
            this.label1.Text = "선택사항";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SetOption_MouseDown);
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SetOption_MouseMove);
            this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SetOption_MouseUp);
            // 
            // ribbonButton2
            // 
            this.ribbonButton2.CheckButton = false;
            this.ribbonButton2.CheckedBkgndImage = null;
            this.ribbonButton2.CheckedImage = null;
            this.ribbonButton2.ClickedBackgroundImage = null;
            this.ribbonButton2.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnCancelClick;
            this.ribbonButton2.CustomImageRect = new System.Drawing.Rectangle(0, 0, 115, 45);
            this.ribbonButton2.DisabledBkgndImage = null;
            this.ribbonButton2.DisabledImage = null;
            this.ribbonButton2.ID = -1;
            this.ribbonButton2.InitButtonWidth = 120;
            this.ribbonButton2.IsChecked = false;
            this.ribbonButton2.Location = new System.Drawing.Point(140, 132);
            this.ribbonButton2.Margin = new System.Windows.Forms.Padding(0);
            this.ribbonButton2.MouseOverBkgndImage = null;
            this.ribbonButton2.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnCancelClick;
            this.ribbonButton2.Name = "ribbonButton2";
            this.ribbonButton2.NormalImage = global::IntegratedManagement4.Properties.Resources.btnCancel;
            this.ribbonButton2.Owner = null;
            this.ribbonButton2.Size = new System.Drawing.Size(115, 45);
            this.ribbonButton2.TabIndex = 17;
            this.ribbonButton2.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton2.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton2.ToolTipText = "";
            this.ribbonButton2.UseCustomImageRect = true;
            this.ribbonButton2.UseTextLocation = false;
            this.ribbonButton2.UseVisualStyleBackColor = true;
            this.ribbonButton2.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_ok
            // 
            this.btn_ok.CheckButton = false;
            this.btn_ok.CheckedBkgndImage = null;
            this.btn_ok.CheckedImage = null;
            this.btn_ok.ClickedBackgroundImage = null;
            this.btn_ok.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnSettingClick;
            this.btn_ok.CustomImageRect = new System.Drawing.Rectangle(0, 0, 115, 45);
            this.btn_ok.DisabledBkgndImage = null;
            this.btn_ok.DisabledImage = null;
            this.btn_ok.ID = -1;
            this.btn_ok.InitButtonWidth = 120;
            this.btn_ok.IsChecked = false;
            this.btn_ok.Location = new System.Drawing.Point(25, 132);
            this.btn_ok.Margin = new System.Windows.Forms.Padding(0);
            this.btn_ok.MouseOverBkgndImage = null;
            this.btn_ok.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnSettingClick;
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.NormalImage = global::IntegratedManagement4.Properties.Resources.btnSetting;
            this.btn_ok.Owner = null;
            this.btn_ok.Size = new System.Drawing.Size(115, 45);
            this.btn_ok.TabIndex = 16;
            this.btn_ok.TextLocation = new System.Drawing.Point(0, 0);
            this.btn_ok.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btn_ok.ToolTipText = "";
            this.btn_ok.UseCustomImageRect = true;
            this.btn_ok.UseTextLocation = false;
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.CheckButton = false;
            this.btn_cancel.CheckedBkgndImage = null;
            this.btn_cancel.CheckedImage = null;
            this.btn_cancel.ClickedBackgroundImage = null;
            this.btn_cancel.ClickedImage = global::IntegratedManagement4.Properties.Resources.Close_40_40_Click;
            this.btn_cancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 22, 22);
            this.btn_cancel.DisabledBkgndImage = null;
            this.btn_cancel.DisabledImage = null;
            this.btn_cancel.ID = -1;
            this.btn_cancel.InitButtonWidth = 22;
            this.btn_cancel.IsChecked = false;
            this.btn_cancel.Location = new System.Drawing.Point(254, 4);
            this.btn_cancel.Margin = new System.Windows.Forms.Padding(0);
            this.btn_cancel.MouseOverBkgndImage = null;
            this.btn_cancel.MouseOverImage = global::IntegratedManagement4.Properties.Resources.Close_40_40_Click;
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.NormalImage = global::IntegratedManagement4.Properties.Resources.Close_40_40_Default;
            this.btn_cancel.Owner = null;
            this.btn_cancel.Size = new System.Drawing.Size(22, 22);
            this.btn_cancel.TabIndex = 14;
            this.btn_cancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btn_cancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btn_cancel.ToolTipText = "";
            this.btn_cancel.UseCustomImageRect = true;
            this.btn_cancel.UseTextLocation = false;
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.panel1.Controls.Add(this.btn_cancel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(279, 31);
            this.panel1.TabIndex = 18;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SetOption_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SetOption_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SetOption_MouseUp);
            // 
            // SetOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.ClientSize = new System.Drawing.Size(279, 186);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ribbonButton2);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.textBoxMemberName);
            this.Controls.Add(this.labelMemberName);
            this.Controls.Add(this.textBoxMemberID);
            this.Controls.Add(this.labelMemberID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SetOption";
            this.Text = "선택사항";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SetOption_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SetOption_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SetOption_MouseUp);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMemberName;
        private System.Windows.Forms.Label labelMemberName;
        private System.Windows.Forms.TextBox textBoxMemberID;
        private System.Windows.Forms.Label labelMemberID;
        private System.Windows.Forms.Label label1;
        private UnE.GUI.RibbonButton btn_cancel;
        private UnE.GUI.RibbonButton btn_ok;
        private UnE.GUI.RibbonButton ribbonButton2;
        private System.Windows.Forms.Panel panel1;
    }
}