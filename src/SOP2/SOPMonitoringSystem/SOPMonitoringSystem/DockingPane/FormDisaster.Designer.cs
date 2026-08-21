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
            this.btnTyphoon = new UnE.GUI.RibbonButton();
            this.btnPollution = new UnE.GUI.RibbonButton();
            this.btnETC = new UnE.GUI.RibbonButton();
            this.btnTerror = new UnE.GUI.RibbonButton();
            this.btnExplosion = new UnE.GUI.RibbonButton();
            this.btnSavingLife = new UnE.GUI.RibbonButton();
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
            this.panelCategory.Controls.Add(this.btnTyphoon);
            this.panelCategory.Controls.Add(this.btnPollution);
            this.panelCategory.Controls.Add(this.btnETC);
            this.panelCategory.Controls.Add(this.btnTerror);
            this.panelCategory.Controls.Add(this.btnExplosion);
            this.panelCategory.Controls.Add(this.btnSavingLife);
            this.panelCategory.Location = new System.Drawing.Point(0, 0);
            this.panelCategory.Name = "panelCategory";
            this.panelCategory.Size = new System.Drawing.Size(276, 612);
            this.panelCategory.TabIndex = 1;
            // 
            // btnFire
            // 
            this.btnFire.BackColor = System.Drawing.Color.Transparent;
            this.btnFire.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnFire.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Fire;
            this.btnFire.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnFire.DisabledBkgndImage = null;
            this.btnFire.DisabledImage = null;
            this.btnFire.ForeColor = System.Drawing.Color.Gold;
            this.btnFire.ID = -1;
            this.btnFire.InitButtonWidth = 284;
            this.btnFire.IsChecked = false;
            this.btnFire.Location = new System.Drawing.Point(0, 0);
            this.btnFire.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnFire.Name = "btnFire";
            this.btnFire.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Fire;
            this.btnFire.Owner = null;
            this.btnFire.Size = new System.Drawing.Size(284, 100);
            this.btnFire.TabIndex = 0;
            this.btnFire.Text = "화재";
            this.btnFire.TextLocation = new System.Drawing.Point(150, 40);
            this.btnFire.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnFire.UseCustomImageRect = true;
            this.btnFire.UseTextLocation = true;
            this.btnFire.UseVisualStyleBackColor = false;
            // 
            // btnNaturalDisaster
            // 
            this.btnNaturalDisaster.BackColor = System.Drawing.Color.Transparent;
            this.btnNaturalDisaster.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnNaturalDisaster.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.HeavySnow;
            this.btnNaturalDisaster.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnNaturalDisaster.DisabledBkgndImage = null;
            this.btnNaturalDisaster.DisabledImage = null;
            this.btnNaturalDisaster.ForeColor = System.Drawing.Color.White;
            this.btnNaturalDisaster.ID = -1;
            this.btnNaturalDisaster.InitButtonWidth = 284;
            this.btnNaturalDisaster.IsChecked = false;
            this.btnNaturalDisaster.Location = new System.Drawing.Point(0, 100);
            this.btnNaturalDisaster.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnNaturalDisaster.Name = "btnNaturalDisaster";
            this.btnNaturalDisaster.NormalImage = global::SOPMonitoringSystem.Properties.Resources.HeavySnow;
            this.btnNaturalDisaster.Owner = null;
            this.btnNaturalDisaster.Size = new System.Drawing.Size(284, 100);
            this.btnNaturalDisaster.TabIndex = 0;
            this.btnNaturalDisaster.Text = "자연재해";
            this.btnNaturalDisaster.TextLocation = new System.Drawing.Point(150, 40);
            this.btnNaturalDisaster.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnNaturalDisaster.UseCustomImageRect = true;
            this.btnNaturalDisaster.UseTextLocation = true;
            this.btnNaturalDisaster.UseVisualStyleBackColor = false;
            // 
            // btnTyphoon
            // 
            this.btnTyphoon.BackColor = System.Drawing.Color.Transparent;
            this.btnTyphoon.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnTyphoon.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Typhoon;
            this.btnTyphoon.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnTyphoon.DisabledBkgndImage = null;
            this.btnTyphoon.DisabledImage = null;
            this.btnTyphoon.ForeColor = System.Drawing.Color.White;
            this.btnTyphoon.ID = -1;
            this.btnTyphoon.InitButtonWidth = 284;
            this.btnTyphoon.IsChecked = false;
            this.btnTyphoon.Location = new System.Drawing.Point(0, 300);
            this.btnTyphoon.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnTyphoon.Name = "btnTyphoon";
            this.btnTyphoon.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Typhoon;
            this.btnTyphoon.Owner = null;
            this.btnTyphoon.Size = new System.Drawing.Size(284, 100);
            this.btnTyphoon.TabIndex = 0;
            this.btnTyphoon.Text = "태풍";
            this.btnTyphoon.TextLocation = new System.Drawing.Point(150, 38);
            this.btnTyphoon.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnTyphoon.UseCustomImageRect = true;
            this.btnTyphoon.UseTextLocation = true;
            this.btnTyphoon.UseVisualStyleBackColor = false;
            // 
            // btnPollution
            // 
            this.btnPollution.BackColor = System.Drawing.Color.Transparent;
            this.btnPollution.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnPollution.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Pollution;
            this.btnPollution.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnPollution.DisabledBkgndImage = null;
            this.btnPollution.DisabledImage = null;
            this.btnPollution.ForeColor = System.Drawing.Color.White;
            this.btnPollution.ID = -1;
            this.btnPollution.InitButtonWidth = 284;
            this.btnPollution.IsChecked = false;
            this.btnPollution.Location = new System.Drawing.Point(0, 200);
            this.btnPollution.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnPollution.Name = "btnPollution";
            this.btnPollution.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Pollution;
            this.btnPollution.Owner = null;
            this.btnPollution.Size = new System.Drawing.Size(284, 100);
            this.btnPollution.TabIndex = 0;
            this.btnPollution.Text = "유출사고";
            this.btnPollution.TextLocation = new System.Drawing.Point(150, 38);
            this.btnPollution.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnPollution.UseCustomImageRect = true;
            this.btnPollution.UseTextLocation = true;
            this.btnPollution.UseVisualStyleBackColor = false;
            // 
            // btnETC
            // 
            this.btnETC.BackColor = System.Drawing.Color.Transparent;
            this.btnETC.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnETC.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.General_Disaster;
            this.btnETC.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnETC.DisabledBkgndImage = null;
            this.btnETC.DisabledImage = null;
            this.btnETC.ForeColor = System.Drawing.Color.White;
            this.btnETC.ID = -1;
            this.btnETC.InitButtonWidth = 284;
            this.btnETC.IsChecked = false;
            this.btnETC.Location = new System.Drawing.Point(0, 600);
            this.btnETC.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnETC.Name = "btnETC";
            this.btnETC.NormalImage = global::SOPMonitoringSystem.Properties.Resources.General_Disaster;
            this.btnETC.Owner = null;
            this.btnETC.Size = new System.Drawing.Size(284, 100);
            this.btnETC.TabIndex = 0;
            this.btnETC.Text = "기타";
            this.btnETC.TextLocation = new System.Drawing.Point(150, 38);
            this.btnETC.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnETC.UseCustomImageRect = true;
            this.btnETC.UseTextLocation = true;
            this.btnETC.UseVisualStyleBackColor = false;
            // 
            // btnTerror
            // 
            this.btnTerror.BackColor = System.Drawing.Color.Transparent;
            this.btnTerror.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnTerror.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Terror;
            this.btnTerror.CustomImageRect = new System.Drawing.Rectangle(0, -3, 112, 106);
            this.btnTerror.DisabledBkgndImage = null;
            this.btnTerror.DisabledImage = null;
            this.btnTerror.ForeColor = System.Drawing.Color.White;
            this.btnTerror.ID = -1;
            this.btnTerror.InitButtonWidth = 284;
            this.btnTerror.IsChecked = false;
            this.btnTerror.Location = new System.Drawing.Point(0, 400);
            this.btnTerror.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnTerror.Name = "btnTerror";
            this.btnTerror.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Terror;
            this.btnTerror.Owner = null;
            this.btnTerror.Size = new System.Drawing.Size(284, 100);
            this.btnTerror.TabIndex = 0;
            this.btnTerror.Text = "테러";
            this.btnTerror.TextLocation = new System.Drawing.Point(150, 38);
            this.btnTerror.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnTerror.UseCustomImageRect = true;
            this.btnTerror.UseTextLocation = true;
            this.btnTerror.UseVisualStyleBackColor = false;
            // 
            // btnExplosion
            // 
            this.btnExplosion.BackColor = System.Drawing.Color.Transparent;
            this.btnExplosion.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnExplosion.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Explosion;
            this.btnExplosion.CustomImageRect = new System.Drawing.Rectangle(15, 13, 80, 80);
            this.btnExplosion.DisabledBkgndImage = null;
            this.btnExplosion.DisabledImage = null;
            this.btnExplosion.ForeColor = System.Drawing.Color.White;
            this.btnExplosion.ID = -1;
            this.btnExplosion.InitButtonWidth = 284;
            this.btnExplosion.IsChecked = false;
            this.btnExplosion.Location = new System.Drawing.Point(0, 700);
            this.btnExplosion.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnExplosion.Name = "btnExplosion";
            this.btnExplosion.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Explosion;
            this.btnExplosion.Owner = null;
            this.btnExplosion.Size = new System.Drawing.Size(284, 100);
            this.btnExplosion.TabIndex = 0;
            this.btnExplosion.Text = "폭발";
            this.btnExplosion.TextLocation = new System.Drawing.Point(150, 38);
            this.btnExplosion.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnExplosion.UseCustomImageRect = true;
            this.btnExplosion.UseTextLocation = true;
            this.btnExplosion.UseVisualStyleBackColor = false;
            // 
            // btnSavingLife
            // 
            this.btnSavingLife.BackColor = System.Drawing.Color.Transparent;
            this.btnSavingLife.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnSavingLife.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.SavingLife;
            this.btnSavingLife.CustomImageRect = new System.Drawing.Rectangle(15, 13, 80, 80);
            this.btnSavingLife.DisabledBkgndImage = null;
            this.btnSavingLife.DisabledImage = null;
            this.btnSavingLife.ForeColor = System.Drawing.Color.White;
            this.btnSavingLife.ID = -1;
            this.btnSavingLife.InitButtonWidth = 284;
            this.btnSavingLife.IsChecked = false;
            this.btnSavingLife.Location = new System.Drawing.Point(0, 500);
            this.btnSavingLife.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
            this.btnSavingLife.Name = "btnSavingLife";
            this.btnSavingLife.NormalImage = global::SOPMonitoringSystem.Properties.Resources.SavingLife;
            this.btnSavingLife.Owner = null;
            this.btnSavingLife.Size = new System.Drawing.Size(284, 100);
            this.btnSavingLife.TabIndex = 0;
            this.btnSavingLife.Text = "인명구조";
            this.btnSavingLife.TextLocation = new System.Drawing.Point(150, 38);
            this.btnSavingLife.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnSavingLife.UseCustomImageRect = true;
            this.btnSavingLife.UseTextLocation = true;
            this.btnSavingLife.UseVisualStyleBackColor = false;
            // 
            // FormDisaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Left_BG;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(276, 612);
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
        private UnE.GUI.RibbonButton btnTyphoon;
        private UnE.GUI.RibbonButton btnSavingLife;
        private UnE.GUI.RibbonButton btnETC;
        private UnE.GUI.RibbonButton btnExplosion;
        private PanelDoubleBuffered panelCategory;
        private PanelDoubleBuffered panelSubCategory;
        private PanelDoubleBuffered panelDisaster;
        private System.Windows.Forms.Timer timerAnimation;
    }
}