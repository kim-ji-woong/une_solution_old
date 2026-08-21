namespace SOPMonitoringSystem
{
    partial class FormDisaster
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
            this.components = new System.ComponentModel.Container();
            this.timerAnimation = new System.Windows.Forms.Timer(this.components);
            this.panelDisaster = new SOPMonitoringSystem.PanelDoubleBuffered();
            this.panelSubCategory = new SOPMonitoringSystem.PanelDoubleBuffered();
            this.panelCategory = new SOPMonitoringSystem.PanelDoubleBuffered();
            this.btnFire = new UnE.GUI.RibbonButton();
            this.btnNaturalDisaster = new UnE.GUI.RibbonButton();
            this.btnPollution = new UnE.GUI.RibbonButton();
            this.btnETC = new UnE.GUI.RibbonButton();
            this.btnTerror = new UnE.GUI.RibbonButton();
            this.btnSecurity = new UnE.GUI.RibbonButton();
            this.btnExplosion = new UnE.GUI.RibbonButton();
            this.panelCategory.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerAnimation
            // 
            this.timerAnimation.Interval = 12;
            this.timerAnimation.Tick += new System.EventHandler(this.timerAnimation_Tick);
            // 
            // panelDisaster
            // 
            this.panelDisaster.AutoScroll = true;
            this.panelDisaster.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Left_BG;
            this.panelDisaster.Location = new System.Drawing.Point(552, 0);
            this.panelDisaster.Name = "panelDisaster";
            this.panelDisaster.Size = new System.Drawing.Size(276, 612);
            this.panelDisaster.TabIndex = 3;
            // 
            // panelSubCategory
            // 
            this.panelSubCategory.AutoScroll = true;
            this.panelSubCategory.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Left_BG;
            this.panelSubCategory.Location = new System.Drawing.Point(276, 0);
            this.panelSubCategory.Name = "panelSubCategory";
            this.panelSubCategory.Size = new System.Drawing.Size(276, 612);
            this.panelSubCategory.TabIndex = 3;
            // 
            // panelCategory
            // 
            this.panelCategory.AutoScroll = true;
            this.panelCategory.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Left_BG;
            this.panelCategory.Controls.Add(this.btnFire);
            this.panelCategory.Controls.Add(this.btnNaturalDisaster);
            this.panelCategory.Controls.Add(this.btnPollution);
            this.panelCategory.Controls.Add(this.btnETC);
            this.panelCategory.Controls.Add(this.btnTerror);
            this.panelCategory.Controls.Add(this.btnSecurity);
            this.panelCategory.Controls.Add(this.btnExplosion);
            this.panelCategory.Location = new System.Drawing.Point(0, 0);
            this.panelCategory.Name = "panelCategory";
            this.panelCategory.Size = new System.Drawing.Size(276, 686);
            this.panelCategory.TabIndex = 1;
            // 
            // btnFire
            // 
            this.btnFire.BackColor = System.Drawing.Color.Transparent;
            this.btnFire.CheckButton = false;
            this.btnFire.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnFire.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Fire;
            this.btnFire.ClickedBackgroundImage = null;
            this.btnFire.ClickedImage = null;
            this.btnFire.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnFire.DisabledBkgndImage = null;
            this.btnFire.DisabledImage = null;
            this.btnFire.ForeColor = System.Drawing.Color.Gold;
            this.btnFire.ID = -1;
            this.btnFire.InitButtonWidth = 284;
            this.btnFire.IsChecked = false;
            this.btnFire.Location = new System.Drawing.Point(0, 0);
            this.btnFire.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnFire.MouseOverImage = null;
            this.btnFire.Name = "btnFire";
            this.btnFire.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Fire;
            this.btnFire.Owner = null;
            this.btnFire.Size = new System.Drawing.Size(284, 100);
            this.btnFire.TabIndex = 0;
            this.btnFire.Text = "화재";
            this.btnFire.TextLocation = new System.Drawing.Point(150, 40);
            this.btnFire.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnFire.ToolTipText = "화재";
            this.btnFire.UseCustomImageRect = true;
            this.btnFire.UseTextLocation = true;
            this.btnFire.UseVisualStyleBackColor = false;
            // 
            // btnNaturalDisaster
            // 
            this.btnNaturalDisaster.BackColor = System.Drawing.Color.Transparent;
            this.btnNaturalDisaster.CheckButton = false;
            this.btnNaturalDisaster.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnNaturalDisaster.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.HeavySnow;
            this.btnNaturalDisaster.ClickedBackgroundImage = null;
            this.btnNaturalDisaster.ClickedImage = null;
            this.btnNaturalDisaster.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnNaturalDisaster.DisabledBkgndImage = null;
            this.btnNaturalDisaster.DisabledImage = null;
            this.btnNaturalDisaster.ForeColor = System.Drawing.Color.White;
            this.btnNaturalDisaster.ID = -1;
            this.btnNaturalDisaster.InitButtonWidth = 284;
            this.btnNaturalDisaster.IsChecked = false;
            this.btnNaturalDisaster.Location = new System.Drawing.Point(0, 100);
            this.btnNaturalDisaster.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnNaturalDisaster.MouseOverImage = null;
            this.btnNaturalDisaster.Name = "btnNaturalDisaster";
            this.btnNaturalDisaster.NormalImage = global::SOPMonitoringSystem.Properties.Resources.HeavySnow;
            this.btnNaturalDisaster.Owner = null;
            this.btnNaturalDisaster.Size = new System.Drawing.Size(284, 100);
            this.btnNaturalDisaster.TabIndex = 0;
            this.btnNaturalDisaster.Text = "자연재해";
            this.btnNaturalDisaster.TextLocation = new System.Drawing.Point(150, 40);
            this.btnNaturalDisaster.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnNaturalDisaster.ToolTipText = "자연재해";
            this.btnNaturalDisaster.UseCustomImageRect = true;
            this.btnNaturalDisaster.UseTextLocation = true;
            this.btnNaturalDisaster.UseVisualStyleBackColor = false;
            // 
            // btnPollution
            // 
            this.btnPollution.BackColor = System.Drawing.Color.Transparent;
            this.btnPollution.CheckButton = false;
            this.btnPollution.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnPollution.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Pollution;
            this.btnPollution.ClickedBackgroundImage = null;
            this.btnPollution.ClickedImage = null;
            this.btnPollution.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnPollution.DisabledBkgndImage = null;
            this.btnPollution.DisabledImage = null;
            this.btnPollution.ForeColor = System.Drawing.Color.White;
            this.btnPollution.ID = -1;
            this.btnPollution.InitButtonWidth = 284;
            this.btnPollution.IsChecked = false;
            this.btnPollution.Location = new System.Drawing.Point(0, 200);
            this.btnPollution.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnPollution.MouseOverImage = null;
            this.btnPollution.Name = "btnPollution";
            this.btnPollution.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Pollution;
            this.btnPollution.Owner = null;
            this.btnPollution.Size = new System.Drawing.Size(284, 100);
            this.btnPollution.TabIndex = 0;
            this.btnPollution.Text = "유출사고";
            this.btnPollution.TextLocation = new System.Drawing.Point(150, 38);
            this.btnPollution.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnPollution.ToolTipText = "유출사고";
            this.btnPollution.UseCustomImageRect = true;
            this.btnPollution.UseTextLocation = true;
            this.btnPollution.UseVisualStyleBackColor = false;
            // 
            // btnETC
            // 
            this.btnETC.BackColor = System.Drawing.Color.Transparent;
            this.btnETC.CheckButton = false;
            this.btnETC.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnETC.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.General_Disaster;
            this.btnETC.ClickedBackgroundImage = null;
            this.btnETC.ClickedImage = null;
            this.btnETC.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnETC.DisabledBkgndImage = null;
            this.btnETC.DisabledImage = null;
            this.btnETC.ForeColor = System.Drawing.Color.White;
            this.btnETC.ID = -1;
            this.btnETC.InitButtonWidth = 284;
            this.btnETC.IsChecked = false;
            this.btnETC.Location = new System.Drawing.Point(0, 400);
            this.btnETC.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnETC.MouseOverImage = null;
            this.btnETC.Name = "btnETC";
            this.btnETC.NormalImage = global::SOPMonitoringSystem.Properties.Resources.General_Disaster;
            this.btnETC.Owner = null;
            this.btnETC.Size = new System.Drawing.Size(284, 100);
            this.btnETC.TabIndex = 0;
            this.btnETC.Text = "기타";
            this.btnETC.TextLocation = new System.Drawing.Point(150, 38);
            this.btnETC.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnETC.ToolTipText = "기타";
            this.btnETC.UseCustomImageRect = true;
            this.btnETC.UseTextLocation = true;
            this.btnETC.UseVisualStyleBackColor = false;
            // 
            // btnTerror
            // 
            this.btnTerror.BackColor = System.Drawing.Color.Transparent;
            this.btnTerror.CheckButton = false;
            this.btnTerror.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnTerror.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Terror;
            this.btnTerror.ClickedBackgroundImage = null;
            this.btnTerror.ClickedImage = null;
            this.btnTerror.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnTerror.DisabledBkgndImage = null;
            this.btnTerror.DisabledImage = null;
            this.btnTerror.ForeColor = System.Drawing.Color.White;
            this.btnTerror.ID = -1;
            this.btnTerror.InitButtonWidth = 284;
            this.btnTerror.IsChecked = false;
            this.btnTerror.Location = new System.Drawing.Point(0, 300);
            this.btnTerror.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnTerror.MouseOverImage = null;
            this.btnTerror.Name = "btnTerror";
            this.btnTerror.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Terror;
            this.btnTerror.Owner = null;
            this.btnTerror.Size = new System.Drawing.Size(284, 100);
            this.btnTerror.TabIndex = 0;
            this.btnTerror.Text = "테러";
            this.btnTerror.TextLocation = new System.Drawing.Point(150, 38);
            this.btnTerror.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnTerror.ToolTipText = "테러";
            this.btnTerror.UseCustomImageRect = true;
            this.btnTerror.UseTextLocation = true;
            this.btnTerror.UseVisualStyleBackColor = false;
            // 
            // btnSecurity
            // 
            this.btnSecurity.BackColor = System.Drawing.Color.Transparent;
            this.btnSecurity.CheckButton = false;
            this.btnSecurity.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnSecurity.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.security;
            this.btnSecurity.ClickedBackgroundImage = null;
            this.btnSecurity.ClickedImage = null;
            this.btnSecurity.CustomImageRect = new System.Drawing.Rectangle(15, 13, 80, 80);
            this.btnSecurity.DisabledBkgndImage = null;
            this.btnSecurity.DisabledImage = null;
            this.btnSecurity.ForeColor = System.Drawing.Color.White;
            this.btnSecurity.ID = -1;
            this.btnSecurity.InitButtonWidth = 284;
            this.btnSecurity.IsChecked = false;
            this.btnSecurity.Location = new System.Drawing.Point(0, 600);
            this.btnSecurity.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnSecurity.MouseOverImage = null;
            this.btnSecurity.Name = "btnSecurity";
            this.btnSecurity.NormalImage = global::SOPMonitoringSystem.Properties.Resources.security;
            this.btnSecurity.Owner = null;
            this.btnSecurity.Size = new System.Drawing.Size(284, 100);
            this.btnSecurity.TabIndex = 0;
            this.btnSecurity.Text = "방범";
            this.btnSecurity.TextLocation = new System.Drawing.Point(150, 38);
            this.btnSecurity.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnSecurity.ToolTipText = "방범";
            this.btnSecurity.UseCustomImageRect = true;
            this.btnSecurity.UseTextLocation = true;
            this.btnSecurity.UseVisualStyleBackColor = false;
            // 
            // btnExplosion
            // 
            this.btnExplosion.BackColor = System.Drawing.Color.Transparent;
            this.btnExplosion.CheckButton = false;
            this.btnExplosion.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnExplosion.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Explosion;
            this.btnExplosion.ClickedBackgroundImage = null;
            this.btnExplosion.ClickedImage = null;
            this.btnExplosion.CustomImageRect = new System.Drawing.Rectangle(15, 13, 80, 80);
            this.btnExplosion.DisabledBkgndImage = null;
            this.btnExplosion.DisabledImage = null;
            this.btnExplosion.ForeColor = System.Drawing.Color.White;
            this.btnExplosion.ID = -1;
            this.btnExplosion.InitButtonWidth = 284;
            this.btnExplosion.IsChecked = false;
            this.btnExplosion.Location = new System.Drawing.Point(0, 500);
            this.btnExplosion.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnExplosion.MouseOverImage = null;
            this.btnExplosion.Name = "btnExplosion";
            this.btnExplosion.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Explosion;
            this.btnExplosion.Owner = null;
            this.btnExplosion.Size = new System.Drawing.Size(284, 100);
            this.btnExplosion.TabIndex = 0;
            this.btnExplosion.Text = "폭발";
            this.btnExplosion.TextLocation = new System.Drawing.Point(150, 38);
            this.btnExplosion.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnExplosion.ToolTipText = "폭발";
            this.btnExplosion.UseCustomImageRect = true;
            this.btnExplosion.UseTextLocation = true;
            this.btnExplosion.UseVisualStyleBackColor = false;
            // 
            // FormDisaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Left_BG;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(276, 698);
            this.Controls.Add(this.panelDisaster);
            this.Controls.Add(this.panelSubCategory);
            this.Controls.Add(this.panelCategory);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDisaster";
            this.Text = "FormDisaster";
            this.Resize += new System.EventHandler(this.FormDisaster_Resize);
            this.panelCategory.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.RibbonButton btnNaturalDisaster;
        private UnE.GUI.RibbonButton btnFire;
        private UnE.GUI.RibbonButton btnPollution;
        private UnE.GUI.RibbonButton btnTerror;
        private UnE.GUI.RibbonButton btnETC;
        private UnE.GUI.RibbonButton btnExplosion;
        private PanelDoubleBuffered panelCategory;
        private PanelDoubleBuffered panelSubCategory;
        private PanelDoubleBuffered panelDisaster;
        private System.Windows.Forms.Timer timerAnimation;
        private UnE.GUI.RibbonButton btnSecurity;
    }
}