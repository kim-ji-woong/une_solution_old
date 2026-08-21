namespace IntegratedManagement3
{
    partial class FormMonitorSettings
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnDelete = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnMission = new System.Windows.Forms.Panel();
            this.pnCCTV = new System.Windows.Forms.Panel();
            this.pnSOP = new System.Windows.Forms.Panel();
            this.pnSDMS = new System.Windows.Forms.Panel();
            this.rbClose = new UnE.GUI.RibbonButton();
            this.rbSave = new UnE.GUI.RibbonButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnDelete);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(520, 231);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "모니터 설정";
            // 
            // pnDelete
            // 
            this.pnDelete.AllowDrop = true;
            this.pnDelete.BackgroundImage = global::IntegratedManagement3.Properties.Resources.Monitor_Delete_Disable;
            this.pnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnDelete.Location = new System.Drawing.Point(473, 195);
            this.pnDelete.Name = "pnDelete";
            this.pnDelete.Size = new System.Drawing.Size(45, 34);
            this.pnDelete.TabIndex = 51;
            this.pnDelete.DragDrop += new System.Windows.Forms.DragEventHandler(this.pnDelete_DragDrop);
            this.pnDelete.DragEnter += new System.Windows.Forms.DragEventHandler(this.pnDelete_DragEnter);
            this.pnDelete.DragOver += new System.Windows.Forms.DragEventHandler(this.pnDelete_DragOver);
            this.pnDelete.DragLeave += new System.EventHandler(this.pnDelete_DragLeave);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(150, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 0;
            // 
            // pnMission
            // 
            this.pnMission.AllowDrop = true;
            this.pnMission.BackgroundImage = global::IntegratedManagement3.Properties.Resources.Monitor_Mission_Enable;
            this.pnMission.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnMission.Location = new System.Drawing.Point(538, 195);
            this.pnMission.Name = "pnMission";
            this.pnMission.Size = new System.Drawing.Size(45, 45);
            this.pnMission.TabIndex = 50;
            this.pnMission.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.panel_GiveFeedback);
            this.pnMission.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_MouseDown);
            // 
            // pnCCTV
            // 
            this.pnCCTV.AllowDrop = true;
            this.pnCCTV.BackgroundImage = global::IntegratedManagement3.Properties.Resources.Monitor_CCTV_Enable;
            this.pnCCTV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnCCTV.Location = new System.Drawing.Point(538, 144);
            this.pnCCTV.Name = "pnCCTV";
            this.pnCCTV.Size = new System.Drawing.Size(45, 45);
            this.pnCCTV.TabIndex = 50;
            this.pnCCTV.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.panel_GiveFeedback);
            this.pnCCTV.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_MouseDown);
            // 
            // pnSOP
            // 
            this.pnSOP.AllowDrop = true;
            this.pnSOP.BackgroundImage = global::IntegratedManagement3.Properties.Resources.Monitor_SOP_Enable;
            this.pnSOP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnSOP.Location = new System.Drawing.Point(538, 93);
            this.pnSOP.Name = "pnSOP";
            this.pnSOP.Size = new System.Drawing.Size(45, 45);
            this.pnSOP.TabIndex = 50;
            this.pnSOP.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.panel_GiveFeedback);
            this.pnSOP.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_MouseDown);
            // 
            // pnSDMS
            // 
            this.pnSDMS.AllowDrop = true;
            this.pnSDMS.BackgroundImage = global::IntegratedManagement3.Properties.Resources.Monitor_SDMS_Enable;
            this.pnSDMS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnSDMS.Location = new System.Drawing.Point(538, 42);
            this.pnSDMS.Name = "pnSDMS";
            this.pnSDMS.Size = new System.Drawing.Size(45, 45);
            this.pnSDMS.TabIndex = 49;
            this.pnSDMS.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.panel_GiveFeedback);
            this.pnSDMS.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_MouseDown);
            // 
            // rbClose
            // 
            this.rbClose.BackColor = System.Drawing.Color.Transparent;
            this.rbClose.CheckButton = false;
            this.rbClose.CheckedBkgndImage = null;
            this.rbClose.CheckedImage = null;
            this.rbClose.ClickedBackgroundImage = null;
            this.rbClose.ClickedImage = global::IntegratedManagement3.Properties.Resources.btnCloseClick;
            this.rbClose.CustomImageRect = new System.Drawing.Rectangle(0, 0, 115, 45);
            this.rbClose.DisabledBkgndImage = null;
            this.rbClose.DisabledImage = null;
            this.rbClose.ID = -1;
            this.rbClose.InitButtonWidth = 115;
            this.rbClose.IsChecked = false;
            this.rbClose.Location = new System.Drawing.Point(280, 252);
            this.rbClose.Margin = new System.Windows.Forms.Padding(0);
            this.rbClose.MouseOverBkgndImage = null;
            this.rbClose.MouseOverImage = global::IntegratedManagement3.Properties.Resources.btnCloseClick;
            this.rbClose.Name = "rbClose";
            this.rbClose.NormalImage = global::IntegratedManagement3.Properties.Resources.btnClose;
            this.rbClose.Owner = null;
            this.rbClose.Size = new System.Drawing.Size(115, 45);
            this.rbClose.TabIndex = 45;
            this.rbClose.TextLocation = new System.Drawing.Point(0, 0);
            this.rbClose.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbClose.ToolTipText = "";
            this.rbClose.UseCustomImageRect = true;
            this.rbClose.UseTextLocation = false;
            this.rbClose.UseVisualStyleBackColor = false;
            this.rbClose.Click += new System.EventHandler(this.rbClose_Click);
            // 
            // rbSave
            // 
            this.rbSave.BackColor = System.Drawing.Color.Transparent;
            this.rbSave.CheckButton = false;
            this.rbSave.CheckedBkgndImage = null;
            this.rbSave.CheckedImage = null;
            this.rbSave.ClickedBackgroundImage = null;
            this.rbSave.ClickedImage = global::IntegratedManagement3.Properties.Resources.btnSaveClick;
            this.rbSave.CustomImageRect = new System.Drawing.Rectangle(0, 0, 115, 45);
            this.rbSave.DisabledBkgndImage = null;
            this.rbSave.DisabledImage = null;
            this.rbSave.ID = -1;
            this.rbSave.InitButtonWidth = 115;
            this.rbSave.IsChecked = false;
            this.rbSave.Location = new System.Drawing.Point(162, 252);
            this.rbSave.Margin = new System.Windows.Forms.Padding(0);
            this.rbSave.MouseOverBkgndImage = null;
            this.rbSave.MouseOverImage = global::IntegratedManagement3.Properties.Resources.btnSaveClick;
            this.rbSave.Name = "rbSave";
            this.rbSave.NormalImage = global::IntegratedManagement3.Properties.Resources.btnSave;
            this.rbSave.Owner = null;
            this.rbSave.Size = new System.Drawing.Size(115, 45);
            this.rbSave.TabIndex = 44;
            this.rbSave.TextLocation = new System.Drawing.Point(0, 0);
            this.rbSave.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbSave.ToolTipText = "";
            this.rbSave.UseCustomImageRect = true;
            this.rbSave.UseTextLocation = false;
            this.rbSave.UseVisualStyleBackColor = false;
            this.rbSave.Click += new System.EventHandler(this.rbSave_Click);
            // 
            // FormMonitorSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.ClientSize = new System.Drawing.Size(600, 306);
            this.Controls.Add(this.pnMission);
            this.Controls.Add(this.pnCCTV);
            this.Controls.Add(this.pnSOP);
            this.Controls.Add(this.pnSDMS);
            this.Controls.Add(this.rbClose);
            this.Controls.Add(this.rbSave);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMonitorSettings";
            this.Text = "FormMonitorSettings";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private UnE.GUI.RibbonButton rbClose;
        private UnE.GUI.RibbonButton rbSave;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnSDMS;
        private System.Windows.Forms.Panel pnSOP;
        private System.Windows.Forms.Panel pnCCTV;
        private System.Windows.Forms.Panel pnMission;
        private System.Windows.Forms.Panel pnDelete;
    }
}