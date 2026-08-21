namespace SOPMonitoringSystem.Popup.Login
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
            this.panelID = new System.Windows.Forms.Panel();
            this.textBoxID = new SOPMonitoringSystem.Popup.Login.ImageTextBox();
            this.panelPW = new System.Windows.Forms.Panel();
            this.textBoxPW = new SOPMonitoringSystem.Popup.Login.ImageTextBox();
            this.btnLogin = new UnE.GUI.RibbonButton();
            this.btnKeepLogin = new UnE.GUI.RibbonButton();
            this.btnClose = new UnE.GUI.RibbonButton();
            this.panelID.SuspendLayout();
            this.panelPW.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelID
            // 
            this.panelID.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.ID_normal;
            this.panelID.Controls.Add(this.textBoxID);
            this.panelID.Location = new System.Drawing.Point(32, 63);
            this.panelID.Name = "panelID";
            this.panelID.Size = new System.Drawing.Size(241, 27);
            this.panelID.TabIndex = 0;
            this.panelID.Click += new System.EventHandler(this.panel_Click);
            // 
            // textBoxID
            // 
            this.textBoxID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxID.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxID.Location = new System.Drawing.Point(0, 0);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Owner = null;
            this.textBoxID.Size = new System.Drawing.Size(241, 25);
            this.textBoxID.TabIndex = 0;
            this.textBoxID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxID_KeyDown);
            this.textBoxID.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.textBox_PreviewKeyDown);
            // 
            // panelPW
            // 
            this.panelPW.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.PW_normal;
            this.panelPW.Controls.Add(this.textBoxPW);
            this.panelPW.Location = new System.Drawing.Point(32, 96);
            this.panelPW.Name = "panelPW";
            this.panelPW.Size = new System.Drawing.Size(241, 27);
            this.panelPW.TabIndex = 0;
            this.panelPW.Click += new System.EventHandler(this.panel_Click);
            // 
            // textBoxPW
            // 
            this.textBoxPW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPW.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxPW.Location = new System.Drawing.Point(0, 0);
            this.textBoxPW.Name = "textBoxPW";
            this.textBoxPW.Owner = null;
            this.textBoxPW.PasswordChar = '*';
            this.textBoxPW.Size = new System.Drawing.Size(241, 25);
            this.textBoxPW.TabIndex = 1;
            this.textBoxPW.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.textBox_PreviewKeyDown);
            // 
            // btnLogin
            // 
            this.btnLogin.CheckButton = false;
            this.btnLogin.CheckedBkgndImage = null;
            this.btnLogin.CheckedImage = null;
            this.btnLogin.CheckedMouseOver = null;
            this.btnLogin.ClickedBackgroundImage = null;
            this.btnLogin.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.login_click;
            this.btnLogin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 241, 29);
            this.btnLogin.DisabledBkgndImage = null;
            this.btnLogin.DisabledImage = null;
            this.btnLogin.ForeColorChecked = System.Drawing.Color.White;
            this.btnLogin.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnLogin.ForeColorDisabled = System.Drawing.Color.White;
            this.btnLogin.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnLogin.ForeColorsByTypeUse = false;
            this.btnLogin.ID = -1;
            this.btnLogin.InitButtonWidth = 241;
            this.btnLogin.IsChecked = false;
            this.btnLogin.Location = new System.Drawing.Point(32, 132);
            this.btnLogin.MouseOverBkgndImage = null;
            this.btnLogin.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.login_hover;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.NormalImage = global::SOPMonitoringSystem.Properties.Resources.login_normal;
            this.btnLogin.Owner = null;
            this.btnLogin.Size = new System.Drawing.Size(241, 29);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.TabStop = false;
            this.btnLogin.TextLocation = new System.Drawing.Point(0, 0);
            this.btnLogin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnLogin.ToolTipText = "";
            this.btnLogin.UseCustomImageRect = false;
            this.btnLogin.UseTextLocation = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnKeepLogin
            // 
            this.btnKeepLogin.CheckButton = false;
            this.btnKeepLogin.CheckedBkgndImage = null;
            this.btnKeepLogin.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.loginkeep_check;
            this.btnKeepLogin.CheckedMouseOver = global::SOPMonitoringSystem.Properties.Resources.loginkeep_check_hover;
            this.btnKeepLogin.ClickedBackgroundImage = null;
            this.btnKeepLogin.ClickedImage = null;
            this.btnKeepLogin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 107, 19);
            this.btnKeepLogin.DisabledBkgndImage = null;
            this.btnKeepLogin.DisabledImage = null;
            this.btnKeepLogin.ForeColorChecked = System.Drawing.Color.White;
            this.btnKeepLogin.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnKeepLogin.ForeColorDisabled = System.Drawing.Color.White;
            this.btnKeepLogin.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnKeepLogin.ForeColorsByTypeUse = false;
            this.btnKeepLogin.ID = -1;
            this.btnKeepLogin.InitButtonWidth = 107;
            this.btnKeepLogin.IsChecked = false;
            this.btnKeepLogin.Location = new System.Drawing.Point(32, 170);
            this.btnKeepLogin.MouseOverBkgndImage = null;
            this.btnKeepLogin.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.loginkeep_hover;
            this.btnKeepLogin.Name = "btnKeepLogin";
            this.btnKeepLogin.NormalImage = global::SOPMonitoringSystem.Properties.Resources.loginkeep_normal;
            this.btnKeepLogin.Owner = null;
            this.btnKeepLogin.Size = new System.Drawing.Size(107, 19);
            this.btnKeepLogin.TabIndex = 5;
            this.btnKeepLogin.TabStop = false;
            this.btnKeepLogin.TextLocation = new System.Drawing.Point(0, 0);
            this.btnKeepLogin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnKeepLogin.ToolTipText = "";
            this.btnKeepLogin.UseCustomImageRect = false;
            this.btnKeepLogin.UseTextLocation = false;
            this.btnKeepLogin.Click += new System.EventHandler(this.btnKeepLogin_Click);
            // 
            // btnClose
            // 
            this.btnClose.CheckButton = false;
            this.btnClose.CheckedBkgndImage = null;
            this.btnClose.CheckedImage = null;
            this.btnClose.CheckedMouseOver = null;
            this.btnClose.ClickedBackgroundImage = null;
            this.btnClose.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.close_click;
            this.btnClose.CustomImageRect = new System.Drawing.Rectangle(0, 0, 13, 14);
            this.btnClose.DisabledBkgndImage = null;
            this.btnClose.DisabledImage = null;
            this.btnClose.ForeColorChecked = System.Drawing.Color.White;
            this.btnClose.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnClose.ForeColorDisabled = System.Drawing.Color.White;
            this.btnClose.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnClose.ForeColorsByTypeUse = false;
            this.btnClose.ID = -1;
            this.btnClose.InitButtonWidth = 13;
            this.btnClose.IsChecked = false;
            this.btnClose.Location = new System.Drawing.Point(283, 12);
            this.btnClose.MouseOverBkgndImage = null;
            this.btnClose.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.close_hover;
            this.btnClose.Name = "btnClose";
            this.btnClose.NormalImage = global::SOPMonitoringSystem.Properties.Resources.close_normal;
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(13, 14);
            this.btnClose.TabIndex = 5;
            this.btnClose.TabStop = false;
            this.btnClose.TextLocation = new System.Drawing.Point(0, 0);
            this.btnClose.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnClose.ToolTipText = "";
            this.btnClose.UseCustomImageRect = false;
            this.btnClose.UseTextLocation = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.LoginBackground;
            this.ClientSize = new System.Drawing.Size(308, 202);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnKeepLogin);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.panelPW);
            this.Controls.Add(this.panelID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLogin";
            this.Text = "FormLogin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLogin_FormClosing);
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormLogin_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormLogin_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormLogin_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormLogin_MouseUp);
            this.Resize += new System.EventHandler(this.FormLogin_Resize);
            this.panelID.ResumeLayout(false);
            this.panelID.PerformLayout();
            this.panelPW.ResumeLayout(false);
            this.panelPW.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelID;
        private ImageTextBox textBoxID;
        private System.Windows.Forms.Panel panelPW;
        private ImageTextBox textBoxPW;
        private UnE.GUI.RibbonButton btnLogin;
        private UnE.GUI.RibbonButton btnKeepLogin;
        private UnE.GUI.RibbonButton btnClose;
    }
}