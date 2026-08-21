namespace SOPMonitoringSystem
{
    partial class PageBackstageHome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageBackstageHome));
            this.axDockingPane = new AxXtremeDockingPane.AxDockingPane();
            this.panelBackImage = new SOPMonitoringSystem.PanelSOP();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
            this.tabControl = new SOPMonitoringSystem.SectionTabControl();
            this.tabPage1 = new Sections.SectionTabPage();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelScenarioName = new System.Windows.Forms.Panel();
            this.labelScenarioName = new System.Windows.Forms.Label();
            this.btnOpenSOP = new SOPMonitoringSystem.RibbonButtonSmallToolbar();
            this.btnPollution = new SOPMonitoringSystem.RibbonButtonSmallToolbar();
            this.btnTerror = new SOPMonitoringSystem.RibbonButtonSmallToolbar();
            this.btnHeavySnow = new SOPMonitoringSystem.RibbonButtonSmallToolbar();
            this.btnGeneralDisaster = new SOPMonitoringSystem.RibbonButtonSmallToolbar();
            this.btnSubmergence = new SOPMonitoringSystem.RibbonButtonSmallToolbar();
            this.btnTyphoon = new SOPMonitoringSystem.RibbonButtonSmallToolbar();
            this.btnEarthquake = new SOPMonitoringSystem.RibbonButtonSmallToolbar();
            this.btnFire = new SOPMonitoringSystem.RibbonButtonSmallToolbar();
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).BeginInit();
            this.panelBackImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).BeginInit();
            this.splitContainerVertical.Panel1.SuspendLayout();
            this.splitContainerVertical.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelScenarioName.SuspendLayout();
            this.SuspendLayout();
            // 
            // axDockingPane
            // 
            this.axDockingPane.Enabled = true;
            this.axDockingPane.Location = new System.Drawing.Point(23, 558);
            this.axDockingPane.Name = "axDockingPane";
            this.axDockingPane.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axDockingPane.OcxState")));
            this.axDockingPane.Size = new System.Drawing.Size(24, 24);
            this.axDockingPane.TabIndex = 2;
            // 
            // panelBackImage
            // 
            this.panelBackImage.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Background_Symbol;
            this.panelBackImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelBackImage.Controls.Add(this.splitContainerMain);
            this.panelBackImage.Location = new System.Drawing.Point(0, 0);
            this.panelBackImage.Name = "panelBackImage";
            this.panelBackImage.Size = new System.Drawing.Size(1093, 525);
            this.panelBackImage.TabIndex = 1;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerVertical);
            this.splitContainerMain.Panel1.Controls.Add(this.panelTop);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.AutoScroll = true;
            this.splitContainerMain.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainerMain.Panel2.Resize += new System.EventHandler(this.splitContainerMain_Panel2_Resize);
            this.splitContainerMain.Size = new System.Drawing.Size(1093, 525);
            this.splitContainerMain.SplitterDistance = 832;
            this.splitContainerMain.TabIndex = 0;
            // 
            // splitContainerVertical
            // 
            this.splitContainerVertical.Location = new System.Drawing.Point(3, 79);
            this.splitContainerVertical.Name = "splitContainerVertical";
            this.splitContainerVertical.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerVertical.Panel1
            // 
            this.splitContainerVertical.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.splitContainerVertical.Panel1.Controls.Add(this.tabControl);
            // 
            // splitContainerVertical.Panel2
            // 
            this.splitContainerVertical.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.splitContainerVertical.Size = new System.Drawing.Size(826, 443);
            this.splitContainerVertical.SplitterDistance = 346;
            this.splitContainerVertical.TabIndex = 1;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("맑은 고딕", 15.75F, System.Drawing.FontStyle.Bold);
            this.tabControl.ItemSize = new System.Drawing.Size(72, 35);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(826, 346);
            this.tabControl.TabIndex = 1;
            this.tabControl.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.ActionStepID = 0;
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPage1.CreateNew = true;
            this.tabPage1.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tabPage1.Location = new System.Drawing.Point(4, 39);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(818, 303);
            this.tabPage1.State = Sections.TabPageState.NOUSE;
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "대응";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.UseWaterMark = false;
            this.tabPage1.VirtualMode = false;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panelTop.Controls.Add(this.panelScenarioName);
            this.panelTop.Controls.Add(this.btnOpenSOP);
            this.panelTop.Controls.Add(this.btnPollution);
            this.panelTop.Controls.Add(this.btnTerror);
            this.panelTop.Controls.Add(this.btnHeavySnow);
            this.panelTop.Controls.Add(this.btnGeneralDisaster);
            this.panelTop.Controls.Add(this.btnSubmergence);
            this.panelTop.Controls.Add(this.btnTyphoon);
            this.panelTop.Controls.Add(this.btnEarthquake);
            this.panelTop.Controls.Add(this.btnFire);
            this.panelTop.Location = new System.Drawing.Point(3, 3);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(826, 30);
            this.panelTop.TabIndex = 0;
            // 
            // panelScenarioName
            // 
            this.panelScenarioName.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Background;
            this.panelScenarioName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelScenarioName.Controls.Add(this.labelScenarioName);
            this.panelScenarioName.Location = new System.Drawing.Point(623, 2);
            this.panelScenarioName.Name = "panelScenarioName";
            this.panelScenarioName.Size = new System.Drawing.Size(200, 26);
            this.panelScenarioName.TabIndex = 1;
            // 
            // labelScenarioName
            // 
            this.labelScenarioName.AutoSize = true;
            this.labelScenarioName.BackColor = System.Drawing.Color.Transparent;
            this.labelScenarioName.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelScenarioName.Location = new System.Drawing.Point(8, 5);
            this.labelScenarioName.Name = "labelScenarioName";
            this.labelScenarioName.Size = new System.Drawing.Size(95, 15);
            this.labelScenarioName.TabIndex = 0;
            this.labelScenarioName.Text = "Scenario Name";
            // 
            // btnOpenSOP
            // 
            this.btnOpenSOP.CheckedBkgndImage = null;
            this.btnOpenSOP.CheckedImage = null;
            this.btnOpenSOP.DisabledBkgndImage = null;
            this.btnOpenSOP.DisabledImage = null;
            this.btnOpenSOP.InitButtonWidth = 60;
            this.btnOpenSOP.IsChecked = false;
            this.btnOpenSOP.Location = new System.Drawing.Point(557, 0);
            this.btnOpenSOP.MouseOverBkgndImage = null;
            this.btnOpenSOP.Name = "btnOpenSOP";
            this.btnOpenSOP.NormalImage = null;
            this.btnOpenSOP.Owner = null;
            this.btnOpenSOP.Size = new System.Drawing.Size(60, 30);
            this.btnOpenSOP.TabIndex = 0;
            this.btnOpenSOP.Text = "불러오기";
            this.btnOpenSOP.UseVisualStyleBackColor = true;
            // 
            // btnPollution
            // 
            this.btnPollution.CheckedBkgndImage = null;
            this.btnPollution.CheckedImage = null;
            this.btnPollution.DisabledBkgndImage = null;
            this.btnPollution.DisabledImage = null;
            this.btnPollution.InitButtonWidth = 60;
            this.btnPollution.IsChecked = false;
            this.btnPollution.Location = new System.Drawing.Point(420, 0);
            this.btnPollution.MouseOverBkgndImage = null;
            this.btnPollution.Name = "btnPollution";
            this.btnPollution.NormalImage = null;
            this.btnPollution.Owner = null;
            this.btnPollution.Size = new System.Drawing.Size(60, 30);
            this.btnPollution.TabIndex = 0;
            this.btnPollution.Text = "오염";
            this.btnPollution.UseVisualStyleBackColor = true;
            // 
            // btnTerror
            // 
            this.btnTerror.CheckedBkgndImage = null;
            this.btnTerror.CheckedImage = null;
            this.btnTerror.DisabledBkgndImage = null;
            this.btnTerror.DisabledImage = null;
            this.btnTerror.InitButtonWidth = 60;
            this.btnTerror.IsChecked = false;
            this.btnTerror.Location = new System.Drawing.Point(360, 0);
            this.btnTerror.MouseOverBkgndImage = null;
            this.btnTerror.Name = "btnTerror";
            this.btnTerror.NormalImage = null;
            this.btnTerror.Owner = null;
            this.btnTerror.Size = new System.Drawing.Size(60, 30);
            this.btnTerror.TabIndex = 0;
            this.btnTerror.Text = "테러";
            this.btnTerror.UseVisualStyleBackColor = true;
            // 
            // btnHeavySnow
            // 
            this.btnHeavySnow.CheckedBkgndImage = null;
            this.btnHeavySnow.CheckedImage = null;
            this.btnHeavySnow.DisabledBkgndImage = null;
            this.btnHeavySnow.DisabledImage = null;
            this.btnHeavySnow.InitButtonWidth = 60;
            this.btnHeavySnow.IsChecked = false;
            this.btnHeavySnow.Location = new System.Drawing.Point(300, 0);
            this.btnHeavySnow.MouseOverBkgndImage = null;
            this.btnHeavySnow.Name = "btnHeavySnow";
            this.btnHeavySnow.NormalImage = null;
            this.btnHeavySnow.Owner = null;
            this.btnHeavySnow.Size = new System.Drawing.Size(60, 30);
            this.btnHeavySnow.TabIndex = 0;
            this.btnHeavySnow.Text = "폭설";
            this.btnHeavySnow.UseVisualStyleBackColor = true;
            // 
            // btnGeneralDisaster
            // 
            this.btnGeneralDisaster.CheckedBkgndImage = null;
            this.btnGeneralDisaster.CheckedImage = null;
            this.btnGeneralDisaster.DisabledBkgndImage = null;
            this.btnGeneralDisaster.DisabledImage = null;
            this.btnGeneralDisaster.InitButtonWidth = 60;
            this.btnGeneralDisaster.IsChecked = false;
            this.btnGeneralDisaster.Location = new System.Drawing.Point(240, 0);
            this.btnGeneralDisaster.MouseOverBkgndImage = null;
            this.btnGeneralDisaster.Name = "btnGeneralDisaster";
            this.btnGeneralDisaster.NormalImage = null;
            this.btnGeneralDisaster.Owner = null;
            this.btnGeneralDisaster.Size = new System.Drawing.Size(60, 30);
            this.btnGeneralDisaster.TabIndex = 0;
            this.btnGeneralDisaster.Text = "일반재해";
            this.btnGeneralDisaster.UseVisualStyleBackColor = true;
            // 
            // btnSubmergence
            // 
            this.btnSubmergence.CheckedBkgndImage = null;
            this.btnSubmergence.CheckedImage = null;
            this.btnSubmergence.DisabledBkgndImage = null;
            this.btnSubmergence.DisabledImage = null;
            this.btnSubmergence.InitButtonWidth = 60;
            this.btnSubmergence.IsChecked = false;
            this.btnSubmergence.Location = new System.Drawing.Point(180, 0);
            this.btnSubmergence.MouseOverBkgndImage = null;
            this.btnSubmergence.Name = "btnSubmergence";
            this.btnSubmergence.NormalImage = null;
            this.btnSubmergence.Owner = null;
            this.btnSubmergence.Size = new System.Drawing.Size(60, 30);
            this.btnSubmergence.TabIndex = 0;
            this.btnSubmergence.Text = "침수";
            this.btnSubmergence.UseVisualStyleBackColor = true;
            // 
            // btnTyphoon
            // 
            this.btnTyphoon.CheckedBkgndImage = null;
            this.btnTyphoon.CheckedImage = null;
            this.btnTyphoon.DisabledBkgndImage = null;
            this.btnTyphoon.DisabledImage = null;
            this.btnTyphoon.InitButtonWidth = 60;
            this.btnTyphoon.IsChecked = false;
            this.btnTyphoon.Location = new System.Drawing.Point(120, 0);
            this.btnTyphoon.MouseOverBkgndImage = null;
            this.btnTyphoon.Name = "btnTyphoon";
            this.btnTyphoon.NormalImage = null;
            this.btnTyphoon.Owner = null;
            this.btnTyphoon.Size = new System.Drawing.Size(60, 30);
            this.btnTyphoon.TabIndex = 0;
            this.btnTyphoon.Text = "태풍";
            this.btnTyphoon.UseVisualStyleBackColor = true;
            // 
            // btnEarthquake
            // 
            this.btnEarthquake.CheckedBkgndImage = null;
            this.btnEarthquake.CheckedImage = null;
            this.btnEarthquake.DisabledBkgndImage = null;
            this.btnEarthquake.DisabledImage = null;
            this.btnEarthquake.InitButtonWidth = 60;
            this.btnEarthquake.IsChecked = false;
            this.btnEarthquake.Location = new System.Drawing.Point(60, 0);
            this.btnEarthquake.MouseOverBkgndImage = null;
            this.btnEarthquake.Name = "btnEarthquake";
            this.btnEarthquake.NormalImage = null;
            this.btnEarthquake.Owner = null;
            this.btnEarthquake.Size = new System.Drawing.Size(60, 30);
            this.btnEarthquake.TabIndex = 0;
            this.btnEarthquake.Text = "지진";
            this.btnEarthquake.UseVisualStyleBackColor = true;
            // 
            // btnFire
            // 
            this.btnFire.CheckedBkgndImage = null;
            this.btnFire.CheckedImage = null;
            this.btnFire.DisabledBkgndImage = null;
            this.btnFire.DisabledImage = null;
            this.btnFire.InitButtonWidth = 60;
            this.btnFire.IsChecked = false;
            this.btnFire.Location = new System.Drawing.Point(0, 0);
            this.btnFire.MouseOverBkgndImage = null;
            this.btnFire.Name = "btnFire";
            this.btnFire.NormalImage = null;
            this.btnFire.Owner = null;
            this.btnFire.Size = new System.Drawing.Size(60, 30);
            this.btnFire.TabIndex = 0;
            this.btnFire.Text = "화재";
            this.btnFire.UseVisualStyleBackColor = true;
            // 
            // PageBackstageHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1093, 595);
            this.Controls.Add(this.axDockingPane);
            this.Controls.Add(this.panelBackImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageHome";
            this.Text = "PageBackstageHome";
            this.Load += new System.EventHandler(this.PageBackstageHome_Load);
            this.Resize += new System.EventHandler(this.PageBackstageHome_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).EndInit();
            this.panelBackImage.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerVertical.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).EndInit();
            this.splitContainerVertical.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelScenarioName.ResumeLayout(false);
            this.panelScenarioName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel panelTop;
        private RibbonButtonSmallToolbar btnFire;
        private System.Windows.Forms.Panel panelScenarioName;
        private RibbonButtonSmallToolbar btnPollution;
        private RibbonButtonSmallToolbar btnTerror;
        private RibbonButtonSmallToolbar btnHeavySnow;
        private RibbonButtonSmallToolbar btnGeneralDisaster;
        private RibbonButtonSmallToolbar btnSubmergence;
        private RibbonButtonSmallToolbar btnTyphoon;
        private RibbonButtonSmallToolbar btnEarthquake;
        private System.Windows.Forms.Label labelScenarioName;
        private RibbonButtonSmallToolbar btnOpenSOP;
        private PanelSOP panelBackImage;
        public Sections.SectionTabPage tabPage1;
        public SectionTabControl tabControl;
        private AxXtremeDockingPane.AxDockingPane axDockingPane;
        private System.Windows.Forms.SplitContainer splitContainerVertical;
    }
}