namespace CrisisAlertManager.Manual
{
    partial class uFormManual
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.plCollapse = new System.Windows.Forms.Panel();
            this.plCollapseManual = new System.Windows.Forms.Panel();
            this.rbtnCollapseSerious = new UnE.GUI.RibbonButton();
            this.rbtnCollapseAlert = new UnE.GUI.RibbonButton();
            this.rbtnCollapseCaution = new UnE.GUI.RibbonButton();
            this.rbtnCollapseAttention = new UnE.GUI.RibbonButton();
            this.plHeat = new System.Windows.Forms.Panel();
            this.plHeatManual = new System.Windows.Forms.Panel();
            this.rbtnHeatSerious = new UnE.GUI.RibbonButton();
            this.rbtnHeatAlert = new UnE.GUI.RibbonButton();
            this.rbtnHeatCaution = new UnE.GUI.RibbonButton();
            this.rbtnHeatAttention = new UnE.GUI.RibbonButton();
            this.plFlood = new System.Windows.Forms.Panel();
            this.plFloodManual = new System.Windows.Forms.Panel();
            this.rbtnFloodSerious = new UnE.GUI.RibbonButton();
            this.rbtnFloodAlert = new UnE.GUI.RibbonButton();
            this.rbtnFloodCaution = new UnE.GUI.RibbonButton();
            this.rbtnFloodAttention = new UnE.GUI.RibbonButton();
            this.plFire = new System.Windows.Forms.Panel();
            this.plFireManual = new System.Windows.Forms.Panel();
            this.rbtnFireSerious = new UnE.GUI.RibbonButton();
            this.rbtnFireAlert = new UnE.GUI.RibbonButton();
            this.rbtnFireCaution = new UnE.GUI.RibbonButton();
            this.rbtnFireAttention = new UnE.GUI.RibbonButton();
            this.btnRemoveManual = new UnE.GUI.ImageButton();
            this.btnAddManual = new UnE.GUI.ImageButton();
            this.rbtnFire = new UnE.GUI.RibbonButton();
            this.rbtnFlood = new UnE.GUI.RibbonButton();
            this.rbtnHeat = new UnE.GUI.RibbonButton();
            this.rbtnCollapse = new UnE.GUI.RibbonButton();
            this.plCollapse.SuspendLayout();
            this.plHeat.SuspendLayout();
            this.plFlood.SuspendLayout();
            this.plFire.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemoveManual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddManual)).BeginInit();
            this.SuspendLayout();
            // 
            // plCollapse
            // 
            this.plCollapse.BackColor = System.Drawing.Color.White;
            this.plCollapse.Controls.Add(this.plCollapseManual);
            this.plCollapse.Controls.Add(this.rbtnCollapseSerious);
            this.plCollapse.Controls.Add(this.rbtnCollapseAlert);
            this.plCollapse.Controls.Add(this.rbtnCollapseCaution);
            this.plCollapse.Controls.Add(this.rbtnCollapseAttention);
            this.plCollapse.Location = new System.Drawing.Point(30, 90);
            this.plCollapse.Name = "plCollapse";
            this.plCollapse.Size = new System.Drawing.Size(1400, 705);
            this.plCollapse.TabIndex = 12;
            // 
            // plCollapseManual
            // 
            this.plCollapseManual.Location = new System.Drawing.Point(0, 50);
            this.plCollapseManual.Name = "plCollapseManual";
            this.plCollapseManual.Size = new System.Drawing.Size(1400, 655);
            this.plCollapseManual.TabIndex = 20;
            this.plCollapseManual.Click += new System.EventHandler(this.plCollapseManual_Click);
            // 
            // rbtnCollapseSerious
            // 
            this.rbtnCollapseSerious.CheckButton = false;
            this.rbtnCollapseSerious.CheckedBkgndImage = null;
            this.rbtnCollapseSerious.CheckedImage = global::CrisisAlertManager.Properties.Resources.CollapseTabSerious_Click;
            this.rbtnCollapseSerious.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.CollapseTabSerious_Click;
            this.rbtnCollapseSerious.ClickedBackgroundImage = null;
            this.rbtnCollapseSerious.ClickedImage = global::CrisisAlertManager.Properties.Resources.CollapseTabSerious_Click;
            this.rbtnCollapseSerious.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnCollapseSerious.DisabledBkgndImage = null;
            this.rbtnCollapseSerious.DisabledImage = null;
            this.rbtnCollapseSerious.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseSerious.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapseSerious.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapseSerious.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseSerious.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseSerious.ForeColorsByTypeUse = true;
            this.rbtnCollapseSerious.ID = -1;
            this.rbtnCollapseSerious.InitButtonWidth = 350;
            this.rbtnCollapseSerious.IsChecked = false;
            this.rbtnCollapseSerious.Location = new System.Drawing.Point(1050, 0);
            this.rbtnCollapseSerious.MouseOverBkgndImage = null;
            this.rbtnCollapseSerious.MouseOverImage = global::CrisisAlertManager.Properties.Resources.CollapseTabSerious_Normal;
            this.rbtnCollapseSerious.Name = "rbtnCollapseSerious";
            this.rbtnCollapseSerious.NormalImage = global::CrisisAlertManager.Properties.Resources.CollapseTabSerious_Normal;
            this.rbtnCollapseSerious.Owner = null;
            this.rbtnCollapseSerious.Size = new System.Drawing.Size(350, 50);
            this.rbtnCollapseSerious.TabIndex = 19;
            this.rbtnCollapseSerious.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnCollapseSerious.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCollapseSerious.ToolTipText = "";
            this.rbtnCollapseSerious.UseCustomImageRect = true;
            this.rbtnCollapseSerious.UseTextLocation = true;
            this.rbtnCollapseSerious.UseVisualStyleBackColor = true;
            this.rbtnCollapseSerious.Click += new System.EventHandler(this.rbtnCollapseSerious_Click);
            // 
            // rbtnCollapseAlert
            // 
            this.rbtnCollapseAlert.CheckButton = false;
            this.rbtnCollapseAlert.CheckedBkgndImage = null;
            this.rbtnCollapseAlert.CheckedImage = global::CrisisAlertManager.Properties.Resources.CollapseTabCaution_Click;
            this.rbtnCollapseAlert.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.CollapseTabCaution_Click;
            this.rbtnCollapseAlert.ClickedBackgroundImage = null;
            this.rbtnCollapseAlert.ClickedImage = global::CrisisAlertManager.Properties.Resources.CollapseTabCaution_Click;
            this.rbtnCollapseAlert.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnCollapseAlert.DisabledBkgndImage = null;
            this.rbtnCollapseAlert.DisabledImage = null;
            this.rbtnCollapseAlert.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseAlert.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapseAlert.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapseAlert.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseAlert.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseAlert.ForeColorsByTypeUse = true;
            this.rbtnCollapseAlert.ID = -1;
            this.rbtnCollapseAlert.InitButtonWidth = 350;
            this.rbtnCollapseAlert.IsChecked = false;
            this.rbtnCollapseAlert.Location = new System.Drawing.Point(700, 0);
            this.rbtnCollapseAlert.MouseOverBkgndImage = null;
            this.rbtnCollapseAlert.MouseOverImage = global::CrisisAlertManager.Properties.Resources.CollapseTabCaution_Normal;
            this.rbtnCollapseAlert.Name = "rbtnCollapseAlert";
            this.rbtnCollapseAlert.NormalImage = global::CrisisAlertManager.Properties.Resources.CollapseTabCaution_Normal;
            this.rbtnCollapseAlert.Owner = null;
            this.rbtnCollapseAlert.Size = new System.Drawing.Size(350, 50);
            this.rbtnCollapseAlert.TabIndex = 18;
            this.rbtnCollapseAlert.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnCollapseAlert.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCollapseAlert.ToolTipText = "";
            this.rbtnCollapseAlert.UseCustomImageRect = true;
            this.rbtnCollapseAlert.UseTextLocation = true;
            this.rbtnCollapseAlert.UseVisualStyleBackColor = true;
            this.rbtnCollapseAlert.Click += new System.EventHandler(this.rbtnCollapseAlert_Click);
            // 
            // rbtnCollapseCaution
            // 
            this.rbtnCollapseCaution.CheckButton = false;
            this.rbtnCollapseCaution.CheckedBkgndImage = null;
            this.rbtnCollapseCaution.CheckedImage = global::CrisisAlertManager.Properties.Resources.CollapseTabAlert_Click;
            this.rbtnCollapseCaution.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.CollapseTabAlert_Click;
            this.rbtnCollapseCaution.ClickedBackgroundImage = null;
            this.rbtnCollapseCaution.ClickedImage = global::CrisisAlertManager.Properties.Resources.CollapseTabAlert_Click;
            this.rbtnCollapseCaution.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnCollapseCaution.DisabledBkgndImage = null;
            this.rbtnCollapseCaution.DisabledImage = null;
            this.rbtnCollapseCaution.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseCaution.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapseCaution.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapseCaution.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseCaution.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseCaution.ForeColorsByTypeUse = true;
            this.rbtnCollapseCaution.ID = -1;
            this.rbtnCollapseCaution.InitButtonWidth = 350;
            this.rbtnCollapseCaution.IsChecked = false;
            this.rbtnCollapseCaution.Location = new System.Drawing.Point(350, 0);
            this.rbtnCollapseCaution.MouseOverBkgndImage = null;
            this.rbtnCollapseCaution.MouseOverImage = global::CrisisAlertManager.Properties.Resources.CollapseTabAlert_Normal;
            this.rbtnCollapseCaution.Name = "rbtnCollapseCaution";
            this.rbtnCollapseCaution.NormalImage = global::CrisisAlertManager.Properties.Resources.CollapseTabAlert_Normal;
            this.rbtnCollapseCaution.Owner = null;
            this.rbtnCollapseCaution.Size = new System.Drawing.Size(350, 50);
            this.rbtnCollapseCaution.TabIndex = 17;
            this.rbtnCollapseCaution.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnCollapseCaution.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCollapseCaution.ToolTipText = "";
            this.rbtnCollapseCaution.UseCustomImageRect = true;
            this.rbtnCollapseCaution.UseTextLocation = true;
            this.rbtnCollapseCaution.UseVisualStyleBackColor = true;
            this.rbtnCollapseCaution.Click += new System.EventHandler(this.rbtnCollapseCaution_Click);
            // 
            // rbtnCollapseAttention
            // 
            this.rbtnCollapseAttention.CheckButton = false;
            this.rbtnCollapseAttention.CheckedBkgndImage = null;
            this.rbtnCollapseAttention.CheckedImage = global::CrisisAlertManager.Properties.Resources.CollapseTabAttion_Click;
            this.rbtnCollapseAttention.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.CollapseTabAttion_Click;
            this.rbtnCollapseAttention.ClickedBackgroundImage = null;
            this.rbtnCollapseAttention.ClickedImage = global::CrisisAlertManager.Properties.Resources.CollapseTabAttion_Click;
            this.rbtnCollapseAttention.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnCollapseAttention.DisabledBkgndImage = null;
            this.rbtnCollapseAttention.DisabledImage = null;
            this.rbtnCollapseAttention.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseAttention.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapseAttention.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapseAttention.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseAttention.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapseAttention.ForeColorsByTypeUse = true;
            this.rbtnCollapseAttention.ID = -1;
            this.rbtnCollapseAttention.InitButtonWidth = 350;
            this.rbtnCollapseAttention.IsChecked = false;
            this.rbtnCollapseAttention.Location = new System.Drawing.Point(0, 0);
            this.rbtnCollapseAttention.MouseOverBkgndImage = null;
            this.rbtnCollapseAttention.MouseOverImage = global::CrisisAlertManager.Properties.Resources.CollapseTabAttion_Normal;
            this.rbtnCollapseAttention.Name = "rbtnCollapseAttention";
            this.rbtnCollapseAttention.NormalImage = global::CrisisAlertManager.Properties.Resources.CollapseTabAttion_Normal;
            this.rbtnCollapseAttention.Owner = null;
            this.rbtnCollapseAttention.Size = new System.Drawing.Size(350, 50);
            this.rbtnCollapseAttention.TabIndex = 16;
            this.rbtnCollapseAttention.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnCollapseAttention.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCollapseAttention.ToolTipText = "";
            this.rbtnCollapseAttention.UseCustomImageRect = true;
            this.rbtnCollapseAttention.UseTextLocation = true;
            this.rbtnCollapseAttention.UseVisualStyleBackColor = true;
            this.rbtnCollapseAttention.Click += new System.EventHandler(this.rbtnCollapseAttention_Click);
            // 
            // plHeat
            // 
            this.plHeat.BackColor = System.Drawing.Color.White;
            this.plHeat.Controls.Add(this.plHeatManual);
            this.plHeat.Controls.Add(this.rbtnHeatSerious);
            this.plHeat.Controls.Add(this.rbtnHeatAlert);
            this.plHeat.Controls.Add(this.rbtnHeatCaution);
            this.plHeat.Controls.Add(this.rbtnHeatAttention);
            this.plHeat.Location = new System.Drawing.Point(30, 90);
            this.plHeat.Name = "plHeat";
            this.plHeat.Size = new System.Drawing.Size(1400, 705);
            this.plHeat.TabIndex = 13;
            // 
            // plHeatManual
            // 
            this.plHeatManual.Location = new System.Drawing.Point(0, 50);
            this.plHeatManual.Name = "plHeatManual";
            this.plHeatManual.Size = new System.Drawing.Size(1400, 655);
            this.plHeatManual.TabIndex = 20;
            this.plHeatManual.Click += new System.EventHandler(this.plHeatManual_Click);
            // 
            // rbtnHeatSerious
            // 
            this.rbtnHeatSerious.CheckButton = false;
            this.rbtnHeatSerious.CheckedBkgndImage = null;
            this.rbtnHeatSerious.CheckedImage = global::CrisisAlertManager.Properties.Resources.HeatTabSerious_Click;
            this.rbtnHeatSerious.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.HeatTabSerious_Click;
            this.rbtnHeatSerious.ClickedBackgroundImage = null;
            this.rbtnHeatSerious.ClickedImage = global::CrisisAlertManager.Properties.Resources.HeatTabSerious_Click;
            this.rbtnHeatSerious.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnHeatSerious.DisabledBkgndImage = null;
            this.rbtnHeatSerious.DisabledImage = null;
            this.rbtnHeatSerious.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatSerious.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeatSerious.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeatSerious.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatSerious.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatSerious.ForeColorsByTypeUse = true;
            this.rbtnHeatSerious.ID = -1;
            this.rbtnHeatSerious.InitButtonWidth = 350;
            this.rbtnHeatSerious.IsChecked = false;
            this.rbtnHeatSerious.Location = new System.Drawing.Point(1050, 0);
            this.rbtnHeatSerious.MouseOverBkgndImage = null;
            this.rbtnHeatSerious.MouseOverImage = global::CrisisAlertManager.Properties.Resources.HeatTabSerious_Normal;
            this.rbtnHeatSerious.Name = "rbtnHeatSerious";
            this.rbtnHeatSerious.NormalImage = global::CrisisAlertManager.Properties.Resources.HeatTabSerious_Normal;
            this.rbtnHeatSerious.Owner = null;
            this.rbtnHeatSerious.Size = new System.Drawing.Size(350, 50);
            this.rbtnHeatSerious.TabIndex = 19;
            this.rbtnHeatSerious.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnHeatSerious.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnHeatSerious.ToolTipText = "";
            this.rbtnHeatSerious.UseCustomImageRect = true;
            this.rbtnHeatSerious.UseTextLocation = true;
            this.rbtnHeatSerious.UseVisualStyleBackColor = true;
            this.rbtnHeatSerious.Click += new System.EventHandler(this.rbtnHeatSerious_Click);
            // 
            // rbtnHeatAlert
            // 
            this.rbtnHeatAlert.CheckButton = false;
            this.rbtnHeatAlert.CheckedBkgndImage = null;
            this.rbtnHeatAlert.CheckedImage = global::CrisisAlertManager.Properties.Resources.HeatTabAlert_Click;
            this.rbtnHeatAlert.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.HeatTabAlert_Click;
            this.rbtnHeatAlert.ClickedBackgroundImage = null;
            this.rbtnHeatAlert.ClickedImage = global::CrisisAlertManager.Properties.Resources.HeatTabAlert_Click;
            this.rbtnHeatAlert.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnHeatAlert.DisabledBkgndImage = null;
            this.rbtnHeatAlert.DisabledImage = null;
            this.rbtnHeatAlert.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatAlert.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeatAlert.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeatAlert.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatAlert.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatAlert.ForeColorsByTypeUse = true;
            this.rbtnHeatAlert.ID = -1;
            this.rbtnHeatAlert.InitButtonWidth = 350;
            this.rbtnHeatAlert.IsChecked = false;
            this.rbtnHeatAlert.Location = new System.Drawing.Point(700, 0);
            this.rbtnHeatAlert.MouseOverBkgndImage = null;
            this.rbtnHeatAlert.MouseOverImage = global::CrisisAlertManager.Properties.Resources.HeatTabAlert_Normal;
            this.rbtnHeatAlert.Name = "rbtnHeatAlert";
            this.rbtnHeatAlert.NormalImage = global::CrisisAlertManager.Properties.Resources.HeatTabAlert_Normal;
            this.rbtnHeatAlert.Owner = null;
            this.rbtnHeatAlert.Size = new System.Drawing.Size(350, 50);
            this.rbtnHeatAlert.TabIndex = 18;
            this.rbtnHeatAlert.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnHeatAlert.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnHeatAlert.ToolTipText = "";
            this.rbtnHeatAlert.UseCustomImageRect = true;
            this.rbtnHeatAlert.UseTextLocation = true;
            this.rbtnHeatAlert.UseVisualStyleBackColor = true;
            this.rbtnHeatAlert.Click += new System.EventHandler(this.rbtnHeatAlert_Click);
            // 
            // rbtnHeatCaution
            // 
            this.rbtnHeatCaution.CheckButton = false;
            this.rbtnHeatCaution.CheckedBkgndImage = null;
            this.rbtnHeatCaution.CheckedImage = global::CrisisAlertManager.Properties.Resources.HeatTabCaution_Click;
            this.rbtnHeatCaution.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.HeatTabCaution_Click;
            this.rbtnHeatCaution.ClickedBackgroundImage = null;
            this.rbtnHeatCaution.ClickedImage = global::CrisisAlertManager.Properties.Resources.HeatTabCaution_Click;
            this.rbtnHeatCaution.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnHeatCaution.DisabledBkgndImage = null;
            this.rbtnHeatCaution.DisabledImage = null;
            this.rbtnHeatCaution.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatCaution.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeatCaution.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeatCaution.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatCaution.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatCaution.ForeColorsByTypeUse = true;
            this.rbtnHeatCaution.ID = -1;
            this.rbtnHeatCaution.InitButtonWidth = 350;
            this.rbtnHeatCaution.IsChecked = false;
            this.rbtnHeatCaution.Location = new System.Drawing.Point(350, 0);
            this.rbtnHeatCaution.MouseOverBkgndImage = null;
            this.rbtnHeatCaution.MouseOverImage = global::CrisisAlertManager.Properties.Resources.HeatTabCaution_Normal;
            this.rbtnHeatCaution.Name = "rbtnHeatCaution";
            this.rbtnHeatCaution.NormalImage = global::CrisisAlertManager.Properties.Resources.HeatTabCaution_Normal;
            this.rbtnHeatCaution.Owner = null;
            this.rbtnHeatCaution.Size = new System.Drawing.Size(350, 50);
            this.rbtnHeatCaution.TabIndex = 17;
            this.rbtnHeatCaution.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnHeatCaution.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnHeatCaution.ToolTipText = "";
            this.rbtnHeatCaution.UseCustomImageRect = true;
            this.rbtnHeatCaution.UseTextLocation = true;
            this.rbtnHeatCaution.UseVisualStyleBackColor = true;
            this.rbtnHeatCaution.Click += new System.EventHandler(this.rbtnHeatCaution_Click);
            // 
            // rbtnHeatAttention
            // 
            this.rbtnHeatAttention.CheckButton = false;
            this.rbtnHeatAttention.CheckedBkgndImage = null;
            this.rbtnHeatAttention.CheckedImage = global::CrisisAlertManager.Properties.Resources.HeatTabAttion_Click;
            this.rbtnHeatAttention.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.HeatTabAttion_Click;
            this.rbtnHeatAttention.ClickedBackgroundImage = null;
            this.rbtnHeatAttention.ClickedImage = global::CrisisAlertManager.Properties.Resources.HeatTabAttion_Click;
            this.rbtnHeatAttention.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnHeatAttention.DisabledBkgndImage = null;
            this.rbtnHeatAttention.DisabledImage = null;
            this.rbtnHeatAttention.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatAttention.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeatAttention.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeatAttention.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatAttention.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeatAttention.ForeColorsByTypeUse = true;
            this.rbtnHeatAttention.ID = -1;
            this.rbtnHeatAttention.InitButtonWidth = 350;
            this.rbtnHeatAttention.IsChecked = false;
            this.rbtnHeatAttention.Location = new System.Drawing.Point(0, 0);
            this.rbtnHeatAttention.MouseOverBkgndImage = null;
            this.rbtnHeatAttention.MouseOverImage = global::CrisisAlertManager.Properties.Resources.HeatTabAttion_Normal;
            this.rbtnHeatAttention.Name = "rbtnHeatAttention";
            this.rbtnHeatAttention.NormalImage = global::CrisisAlertManager.Properties.Resources.HeatTabAttion_Normal;
            this.rbtnHeatAttention.Owner = null;
            this.rbtnHeatAttention.Size = new System.Drawing.Size(350, 50);
            this.rbtnHeatAttention.TabIndex = 16;
            this.rbtnHeatAttention.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnHeatAttention.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnHeatAttention.ToolTipText = "";
            this.rbtnHeatAttention.UseCustomImageRect = true;
            this.rbtnHeatAttention.UseTextLocation = true;
            this.rbtnHeatAttention.UseVisualStyleBackColor = true;
            this.rbtnHeatAttention.Click += new System.EventHandler(this.rbtnHeatAttention_Click);
            // 
            // plFlood
            // 
            this.plFlood.BackColor = System.Drawing.Color.White;
            this.plFlood.Controls.Add(this.plFloodManual);
            this.plFlood.Controls.Add(this.rbtnFloodSerious);
            this.plFlood.Controls.Add(this.rbtnFloodAlert);
            this.plFlood.Controls.Add(this.rbtnFloodCaution);
            this.plFlood.Controls.Add(this.rbtnFloodAttention);
            this.plFlood.Location = new System.Drawing.Point(30, 90);
            this.plFlood.Name = "plFlood";
            this.plFlood.Size = new System.Drawing.Size(1400, 705);
            this.plFlood.TabIndex = 14;
            // 
            // plFloodManual
            // 
            this.plFloodManual.Location = new System.Drawing.Point(0, 50);
            this.plFloodManual.Name = "plFloodManual";
            this.plFloodManual.Size = new System.Drawing.Size(1400, 655);
            this.plFloodManual.TabIndex = 15;
            this.plFloodManual.Click += new System.EventHandler(this.plFloodManual_Click);
            // 
            // rbtnFloodSerious
            // 
            this.rbtnFloodSerious.CheckButton = false;
            this.rbtnFloodSerious.CheckedBkgndImage = null;
            this.rbtnFloodSerious.CheckedImage = global::CrisisAlertManager.Properties.Resources.FloodTabSerious_Click;
            this.rbtnFloodSerious.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.FloodTabSerious_Click;
            this.rbtnFloodSerious.ClickedBackgroundImage = null;
            this.rbtnFloodSerious.ClickedImage = global::CrisisAlertManager.Properties.Resources.FloodTabSerious_Click;
            this.rbtnFloodSerious.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnFloodSerious.DisabledBkgndImage = null;
            this.rbtnFloodSerious.DisabledImage = null;
            this.rbtnFloodSerious.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodSerious.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFloodSerious.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFloodSerious.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodSerious.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodSerious.ForeColorsByTypeUse = true;
            this.rbtnFloodSerious.ID = -1;
            this.rbtnFloodSerious.InitButtonWidth = 350;
            this.rbtnFloodSerious.IsChecked = false;
            this.rbtnFloodSerious.Location = new System.Drawing.Point(1050, 0);
            this.rbtnFloodSerious.MouseOverBkgndImage = null;
            this.rbtnFloodSerious.MouseOverImage = global::CrisisAlertManager.Properties.Resources.FloodTabSerious_Normal;
            this.rbtnFloodSerious.Name = "rbtnFloodSerious";
            this.rbtnFloodSerious.NormalImage = global::CrisisAlertManager.Properties.Resources.FloodTabSerious_Normal;
            this.rbtnFloodSerious.Owner = null;
            this.rbtnFloodSerious.Size = new System.Drawing.Size(350, 50);
            this.rbtnFloodSerious.TabIndex = 14;
            this.rbtnFloodSerious.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFloodSerious.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFloodSerious.ToolTipText = "";
            this.rbtnFloodSerious.UseCustomImageRect = true;
            this.rbtnFloodSerious.UseTextLocation = true;
            this.rbtnFloodSerious.UseVisualStyleBackColor = true;
            this.rbtnFloodSerious.Click += new System.EventHandler(this.rbtnFloodSerious_Click);
            // 
            // rbtnFloodAlert
            // 
            this.rbtnFloodAlert.CheckButton = false;
            this.rbtnFloodAlert.CheckedBkgndImage = null;
            this.rbtnFloodAlert.CheckedImage = global::CrisisAlertManager.Properties.Resources.FloodTabAlert_Click;
            this.rbtnFloodAlert.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.FloodTabAlert_Click;
            this.rbtnFloodAlert.ClickedBackgroundImage = null;
            this.rbtnFloodAlert.ClickedImage = global::CrisisAlertManager.Properties.Resources.FloodTabAlert_Click;
            this.rbtnFloodAlert.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnFloodAlert.DisabledBkgndImage = null;
            this.rbtnFloodAlert.DisabledImage = null;
            this.rbtnFloodAlert.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodAlert.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFloodAlert.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFloodAlert.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodAlert.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodAlert.ForeColorsByTypeUse = true;
            this.rbtnFloodAlert.ID = -1;
            this.rbtnFloodAlert.InitButtonWidth = 350;
            this.rbtnFloodAlert.IsChecked = false;
            this.rbtnFloodAlert.Location = new System.Drawing.Point(700, 0);
            this.rbtnFloodAlert.MouseOverBkgndImage = null;
            this.rbtnFloodAlert.MouseOverImage = global::CrisisAlertManager.Properties.Resources.FloodTabAlert_Normal;
            this.rbtnFloodAlert.Name = "rbtnFloodAlert";
            this.rbtnFloodAlert.NormalImage = global::CrisisAlertManager.Properties.Resources.FloodTabAlert_Normal;
            this.rbtnFloodAlert.Owner = null;
            this.rbtnFloodAlert.Size = new System.Drawing.Size(350, 50);
            this.rbtnFloodAlert.TabIndex = 13;
            this.rbtnFloodAlert.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFloodAlert.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFloodAlert.ToolTipText = "";
            this.rbtnFloodAlert.UseCustomImageRect = true;
            this.rbtnFloodAlert.UseTextLocation = true;
            this.rbtnFloodAlert.UseVisualStyleBackColor = true;
            this.rbtnFloodAlert.Click += new System.EventHandler(this.rbtnFloodAlert_Click);
            // 
            // rbtnFloodCaution
            // 
            this.rbtnFloodCaution.CheckButton = false;
            this.rbtnFloodCaution.CheckedBkgndImage = null;
            this.rbtnFloodCaution.CheckedImage = global::CrisisAlertManager.Properties.Resources.FloodTabCaution_Click;
            this.rbtnFloodCaution.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.FloodTabCaution_Click;
            this.rbtnFloodCaution.ClickedBackgroundImage = null;
            this.rbtnFloodCaution.ClickedImage = global::CrisisAlertManager.Properties.Resources.FloodTabCaution_Click;
            this.rbtnFloodCaution.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnFloodCaution.DisabledBkgndImage = null;
            this.rbtnFloodCaution.DisabledImage = null;
            this.rbtnFloodCaution.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodCaution.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFloodCaution.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFloodCaution.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodCaution.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodCaution.ForeColorsByTypeUse = true;
            this.rbtnFloodCaution.ID = -1;
            this.rbtnFloodCaution.InitButtonWidth = 350;
            this.rbtnFloodCaution.IsChecked = false;
            this.rbtnFloodCaution.Location = new System.Drawing.Point(350, 0);
            this.rbtnFloodCaution.MouseOverBkgndImage = null;
            this.rbtnFloodCaution.MouseOverImage = global::CrisisAlertManager.Properties.Resources.FloodTabCaution_Normal;
            this.rbtnFloodCaution.Name = "rbtnFloodCaution";
            this.rbtnFloodCaution.NormalImage = global::CrisisAlertManager.Properties.Resources.FloodTabCaution_Normal;
            this.rbtnFloodCaution.Owner = null;
            this.rbtnFloodCaution.Size = new System.Drawing.Size(350, 50);
            this.rbtnFloodCaution.TabIndex = 12;
            this.rbtnFloodCaution.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFloodCaution.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFloodCaution.ToolTipText = "";
            this.rbtnFloodCaution.UseCustomImageRect = true;
            this.rbtnFloodCaution.UseTextLocation = true;
            this.rbtnFloodCaution.UseVisualStyleBackColor = true;
            this.rbtnFloodCaution.Click += new System.EventHandler(this.rbtnFloodCaution_Click);
            // 
            // rbtnFloodAttention
            // 
            this.rbtnFloodAttention.CheckButton = false;
            this.rbtnFloodAttention.CheckedBkgndImage = null;
            this.rbtnFloodAttention.CheckedImage = global::CrisisAlertManager.Properties.Resources.FloodTabAttion_Click;
            this.rbtnFloodAttention.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.FloodTabAttion_Click;
            this.rbtnFloodAttention.ClickedBackgroundImage = null;
            this.rbtnFloodAttention.ClickedImage = global::CrisisAlertManager.Properties.Resources.FloodTabAttion_Click;
            this.rbtnFloodAttention.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnFloodAttention.DisabledBkgndImage = null;
            this.rbtnFloodAttention.DisabledImage = null;
            this.rbtnFloodAttention.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodAttention.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFloodAttention.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFloodAttention.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodAttention.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFloodAttention.ForeColorsByTypeUse = true;
            this.rbtnFloodAttention.ID = -1;
            this.rbtnFloodAttention.InitButtonWidth = 350;
            this.rbtnFloodAttention.IsChecked = false;
            this.rbtnFloodAttention.Location = new System.Drawing.Point(0, 0);
            this.rbtnFloodAttention.MouseOverBkgndImage = null;
            this.rbtnFloodAttention.MouseOverImage = global::CrisisAlertManager.Properties.Resources.FloodTabAttion_Normal;
            this.rbtnFloodAttention.Name = "rbtnFloodAttention";
            this.rbtnFloodAttention.NormalImage = global::CrisisAlertManager.Properties.Resources.FloodTabAttion_Normal;
            this.rbtnFloodAttention.Owner = null;
            this.rbtnFloodAttention.Size = new System.Drawing.Size(350, 50);
            this.rbtnFloodAttention.TabIndex = 11;
            this.rbtnFloodAttention.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFloodAttention.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFloodAttention.ToolTipText = "";
            this.rbtnFloodAttention.UseCustomImageRect = true;
            this.rbtnFloodAttention.UseTextLocation = true;
            this.rbtnFloodAttention.UseVisualStyleBackColor = true;
            this.rbtnFloodAttention.Click += new System.EventHandler(this.rbtnFloodAttention_Click);
            // 
            // plFire
            // 
            this.plFire.BackColor = System.Drawing.Color.White;
            this.plFire.Controls.Add(this.plFireManual);
            this.plFire.Controls.Add(this.rbtnFireSerious);
            this.plFire.Controls.Add(this.rbtnFireAlert);
            this.plFire.Controls.Add(this.rbtnFireCaution);
            this.plFire.Controls.Add(this.rbtnFireAttention);
            this.plFire.Location = new System.Drawing.Point(30, 90);
            this.plFire.Name = "plFire";
            this.plFire.Size = new System.Drawing.Size(1400, 705);
            this.plFire.TabIndex = 15;
            // 
            // plFireManual
            // 
            this.plFireManual.Location = new System.Drawing.Point(0, 50);
            this.plFireManual.Name = "plFireManual";
            this.plFireManual.Size = new System.Drawing.Size(1400, 655);
            this.plFireManual.TabIndex = 13;
            this.plFireManual.Click += new System.EventHandler(this.plFireManual_Click);
            // 
            // rbtnFireSerious
            // 
            this.rbtnFireSerious.CheckButton = false;
            this.rbtnFireSerious.CheckedBkgndImage = null;
            this.rbtnFireSerious.CheckedImage = global::CrisisAlertManager.Properties.Resources.FireTabSerious_Click;
            this.rbtnFireSerious.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.FireTabSerious_Click;
            this.rbtnFireSerious.ClickedBackgroundImage = null;
            this.rbtnFireSerious.ClickedImage = global::CrisisAlertManager.Properties.Resources.FireTabSerious_Click;
            this.rbtnFireSerious.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnFireSerious.DisabledBkgndImage = null;
            this.rbtnFireSerious.DisabledImage = null;
            this.rbtnFireSerious.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireSerious.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFireSerious.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFireSerious.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireSerious.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireSerious.ForeColorsByTypeUse = true;
            this.rbtnFireSerious.ID = -1;
            this.rbtnFireSerious.InitButtonWidth = 350;
            this.rbtnFireSerious.IsChecked = false;
            this.rbtnFireSerious.Location = new System.Drawing.Point(1050, 0);
            this.rbtnFireSerious.MouseOverBkgndImage = null;
            this.rbtnFireSerious.MouseOverImage = global::CrisisAlertManager.Properties.Resources.FireTabSerious_Normal;
            this.rbtnFireSerious.Name = "rbtnFireSerious";
            this.rbtnFireSerious.NormalImage = global::CrisisAlertManager.Properties.Resources.FireTabSerious_Normal;
            this.rbtnFireSerious.Owner = null;
            this.rbtnFireSerious.Size = new System.Drawing.Size(350, 50);
            this.rbtnFireSerious.TabIndex = 12;
            this.rbtnFireSerious.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFireSerious.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFireSerious.ToolTipText = "";
            this.rbtnFireSerious.UseCustomImageRect = true;
            this.rbtnFireSerious.UseTextLocation = true;
            this.rbtnFireSerious.UseVisualStyleBackColor = true;
            this.rbtnFireSerious.Click += new System.EventHandler(this.rbtnFireSerious_Click);
            // 
            // rbtnFireAlert
            // 
            this.rbtnFireAlert.CheckButton = false;
            this.rbtnFireAlert.CheckedBkgndImage = null;
            this.rbtnFireAlert.CheckedImage = global::CrisisAlertManager.Properties.Resources.FireTabAlert_Click;
            this.rbtnFireAlert.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.FireTabAlert_Click;
            this.rbtnFireAlert.ClickedBackgroundImage = null;
            this.rbtnFireAlert.ClickedImage = global::CrisisAlertManager.Properties.Resources.FireTabAlert_Click;
            this.rbtnFireAlert.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnFireAlert.DisabledBkgndImage = null;
            this.rbtnFireAlert.DisabledImage = null;
            this.rbtnFireAlert.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireAlert.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFireAlert.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFireAlert.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireAlert.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireAlert.ForeColorsByTypeUse = true;
            this.rbtnFireAlert.ID = -1;
            this.rbtnFireAlert.InitButtonWidth = 350;
            this.rbtnFireAlert.IsChecked = false;
            this.rbtnFireAlert.Location = new System.Drawing.Point(700, 0);
            this.rbtnFireAlert.MouseOverBkgndImage = null;
            this.rbtnFireAlert.MouseOverImage = global::CrisisAlertManager.Properties.Resources.FireTabAlert_Normal;
            this.rbtnFireAlert.Name = "rbtnFireAlert";
            this.rbtnFireAlert.NormalImage = global::CrisisAlertManager.Properties.Resources.FireTabAlert_Normal;
            this.rbtnFireAlert.Owner = null;
            this.rbtnFireAlert.Size = new System.Drawing.Size(350, 50);
            this.rbtnFireAlert.TabIndex = 11;
            this.rbtnFireAlert.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFireAlert.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFireAlert.ToolTipText = "";
            this.rbtnFireAlert.UseCustomImageRect = true;
            this.rbtnFireAlert.UseTextLocation = true;
            this.rbtnFireAlert.UseVisualStyleBackColor = true;
            this.rbtnFireAlert.Click += new System.EventHandler(this.rbtnFireAlert_Click);
            // 
            // rbtnFireCaution
            // 
            this.rbtnFireCaution.CheckButton = false;
            this.rbtnFireCaution.CheckedBkgndImage = null;
            this.rbtnFireCaution.CheckedImage = global::CrisisAlertManager.Properties.Resources.FireTabCaution_Click;
            this.rbtnFireCaution.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.FireTabCaution_Click;
            this.rbtnFireCaution.ClickedBackgroundImage = null;
            this.rbtnFireCaution.ClickedImage = global::CrisisAlertManager.Properties.Resources.FireTabCaution_Click;
            this.rbtnFireCaution.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnFireCaution.DisabledBkgndImage = null;
            this.rbtnFireCaution.DisabledImage = null;
            this.rbtnFireCaution.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireCaution.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFireCaution.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFireCaution.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireCaution.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireCaution.ForeColorsByTypeUse = true;
            this.rbtnFireCaution.ID = -1;
            this.rbtnFireCaution.InitButtonWidth = 350;
            this.rbtnFireCaution.IsChecked = false;
            this.rbtnFireCaution.Location = new System.Drawing.Point(350, 0);
            this.rbtnFireCaution.MouseOverBkgndImage = null;
            this.rbtnFireCaution.MouseOverImage = global::CrisisAlertManager.Properties.Resources.FireTabCaution_Normal;
            this.rbtnFireCaution.Name = "rbtnFireCaution";
            this.rbtnFireCaution.NormalImage = global::CrisisAlertManager.Properties.Resources.FireTabCaution_Normal;
            this.rbtnFireCaution.Owner = null;
            this.rbtnFireCaution.Size = new System.Drawing.Size(350, 50);
            this.rbtnFireCaution.TabIndex = 10;
            this.rbtnFireCaution.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFireCaution.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFireCaution.ToolTipText = "";
            this.rbtnFireCaution.UseCustomImageRect = true;
            this.rbtnFireCaution.UseTextLocation = true;
            this.rbtnFireCaution.UseVisualStyleBackColor = true;
            this.rbtnFireCaution.Click += new System.EventHandler(this.rbtnFireCaution_Click);
            // 
            // rbtnFireAttention
            // 
            this.rbtnFireAttention.CheckButton = false;
            this.rbtnFireAttention.CheckedBkgndImage = null;
            this.rbtnFireAttention.CheckedImage = global::CrisisAlertManager.Properties.Resources.FireTabAttion_Click;
            this.rbtnFireAttention.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.FireTabAttion_Click;
            this.rbtnFireAttention.ClickedBackgroundImage = null;
            this.rbtnFireAttention.ClickedImage = global::CrisisAlertManager.Properties.Resources.FireTabAttion_Click;
            this.rbtnFireAttention.CustomImageRect = new System.Drawing.Rectangle(0, 0, 350, 50);
            this.rbtnFireAttention.DisabledBkgndImage = null;
            this.rbtnFireAttention.DisabledImage = null;
            this.rbtnFireAttention.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireAttention.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFireAttention.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFireAttention.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireAttention.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFireAttention.ForeColorsByTypeUse = true;
            this.rbtnFireAttention.ID = -1;
            this.rbtnFireAttention.InitButtonWidth = 350;
            this.rbtnFireAttention.IsChecked = false;
            this.rbtnFireAttention.Location = new System.Drawing.Point(0, 0);
            this.rbtnFireAttention.MouseOverBkgndImage = null;
            this.rbtnFireAttention.MouseOverImage = global::CrisisAlertManager.Properties.Resources.FireTabAttion_Normal;
            this.rbtnFireAttention.Name = "rbtnFireAttention";
            this.rbtnFireAttention.NormalImage = global::CrisisAlertManager.Properties.Resources.FireTabAttion_Normal;
            this.rbtnFireAttention.Owner = null;
            this.rbtnFireAttention.Size = new System.Drawing.Size(350, 50);
            this.rbtnFireAttention.TabIndex = 9;
            this.rbtnFireAttention.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFireAttention.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFireAttention.ToolTipText = "";
            this.rbtnFireAttention.UseCustomImageRect = true;
            this.rbtnFireAttention.UseTextLocation = true;
            this.rbtnFireAttention.UseVisualStyleBackColor = true;
            this.rbtnFireAttention.Click += new System.EventHandler(this.rbtnFireAttention_Click);
            // 
            // btnRemoveManual
            // 
            this.btnRemoveManual.ButtonText = "";
            this.btnRemoveManual.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnRemoveManual_Click;
            this.btnRemoveManual.ImageDisabled = null;
            this.btnRemoveManual.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnRemoveManual_Hover;
            this.btnRemoveManual.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnRemoveManual_Normal;
            this.btnRemoveManual.Location = new System.Drawing.Point(1315, 811);
            this.btnRemoveManual.Name = "btnRemoveManual";
            this.btnRemoveManual.Owner = null;
            this.btnRemoveManual.Size = new System.Drawing.Size(115, 50);
            this.btnRemoveManual.TabIndex = 55;
            this.btnRemoveManual.TabStop = false;
            this.btnRemoveManual.TextColor = System.Drawing.Color.Black;
            this.btnRemoveManual.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemoveManual.ToolTipText = "";
            this.btnRemoveManual.UseToolTip = false;
            this.btnRemoveManual.WindowRateWidth = 1F;
            this.btnRemoveManual.Click += new System.EventHandler(this.btnRemoveManual_Click);
            // 
            // btnAddManual
            // 
            this.btnAddManual.ButtonText = "";
            this.btnAddManual.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnAddManual_Click;
            this.btnAddManual.ImageDisabled = null;
            this.btnAddManual.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnAddManual_Hover;
            this.btnAddManual.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnAddManual_Normal;
            this.btnAddManual.Location = new System.Drawing.Point(1180, 811);
            this.btnAddManual.Name = "btnAddManual";
            this.btnAddManual.Owner = null;
            this.btnAddManual.Size = new System.Drawing.Size(115, 50);
            this.btnAddManual.TabIndex = 54;
            this.btnAddManual.TabStop = false;
            this.btnAddManual.TextColor = System.Drawing.Color.Black;
            this.btnAddManual.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAddManual.ToolTipText = "";
            this.btnAddManual.UseToolTip = false;
            this.btnAddManual.WindowRateWidth = 1F;
            this.btnAddManual.Click += new System.EventHandler(this.btnAddManual_Click);
            // 
            // rbtnFire
            // 
            this.rbtnFire.CheckButton = false;
            this.rbtnFire.CheckedBkgndImage = null;
            this.rbtnFire.CheckedImage = global::CrisisAlertManager.Properties.Resources.ManualFireTab_Click;
            this.rbtnFire.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ManualFireTab_Click;
            this.rbtnFire.ClickedBackgroundImage = null;
            this.rbtnFire.ClickedImage = global::CrisisAlertManager.Properties.Resources.ManualFireTab_Click;
            this.rbtnFire.CustomImageRect = new System.Drawing.Rectangle(0, 0, 144, 36);
            this.rbtnFire.DisabledBkgndImage = null;
            this.rbtnFire.DisabledImage = null;
            this.rbtnFire.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFire.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFire.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFire.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFire.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFire.ForeColorsByTypeUse = true;
            this.rbtnFire.ID = -1;
            this.rbtnFire.InitButtonWidth = 144;
            this.rbtnFire.IsChecked = false;
            this.rbtnFire.Location = new System.Drawing.Point(30, 54);
            this.rbtnFire.MouseOverBkgndImage = null;
            this.rbtnFire.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ManualFireTab_Hover;
            this.rbtnFire.Name = "rbtnFire";
            this.rbtnFire.NormalImage = global::CrisisAlertManager.Properties.Resources.ManualFireTab_Normal;
            this.rbtnFire.Owner = null;
            this.rbtnFire.Size = new System.Drawing.Size(144, 36);
            this.rbtnFire.TabIndex = 8;
            this.rbtnFire.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFire.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFire.ToolTipText = "";
            this.rbtnFire.UseCustomImageRect = true;
            this.rbtnFire.UseTextLocation = true;
            this.rbtnFire.UseVisualStyleBackColor = true;
            this.rbtnFire.Click += new System.EventHandler(this.rbtnFire_Click);
            // 
            // rbtnFlood
            // 
            this.rbtnFlood.CheckButton = false;
            this.rbtnFlood.CheckedBkgndImage = null;
            this.rbtnFlood.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportFloodTab_Click;
            this.rbtnFlood.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportFloodTab_Click;
            this.rbtnFlood.ClickedBackgroundImage = null;
            this.rbtnFlood.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportFloodTab_Click;
            this.rbtnFlood.CustomImageRect = new System.Drawing.Rectangle(0, 0, 144, 36);
            this.rbtnFlood.DisabledBkgndImage = null;
            this.rbtnFlood.DisabledImage = null;
            this.rbtnFlood.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFlood.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFlood.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFlood.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFlood.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFlood.ForeColorsByTypeUse = true;
            this.rbtnFlood.ID = -1;
            this.rbtnFlood.InitButtonWidth = 144;
            this.rbtnFlood.IsChecked = false;
            this.rbtnFlood.Location = new System.Drawing.Point(180, 54);
            this.rbtnFlood.MouseOverBkgndImage = null;
            this.rbtnFlood.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportFloodTab_Hover;
            this.rbtnFlood.Name = "rbtnFlood";
            this.rbtnFlood.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportFloodTab_Normal;
            this.rbtnFlood.Owner = null;
            this.rbtnFlood.Size = new System.Drawing.Size(144, 36);
            this.rbtnFlood.TabIndex = 9;
            this.rbtnFlood.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFlood.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFlood.ToolTipText = "";
            this.rbtnFlood.UseCustomImageRect = true;
            this.rbtnFlood.UseTextLocation = true;
            this.rbtnFlood.UseVisualStyleBackColor = true;
            this.rbtnFlood.Click += new System.EventHandler(this.rbtnFlood_Click);
            // 
            // rbtnHeat
            // 
            this.rbtnHeat.CheckButton = false;
            this.rbtnHeat.CheckedBkgndImage = null;
            this.rbtnHeat.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportHeatTab_Click;
            this.rbtnHeat.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportHeatTab_Click;
            this.rbtnHeat.ClickedBackgroundImage = null;
            this.rbtnHeat.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportHeatTab_Click;
            this.rbtnHeat.CustomImageRect = new System.Drawing.Rectangle(0, 0, 144, 36);
            this.rbtnHeat.DisabledBkgndImage = null;
            this.rbtnHeat.DisabledImage = null;
            this.rbtnHeat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeat.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeat.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeat.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeat.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeat.ForeColorsByTypeUse = true;
            this.rbtnHeat.ID = -1;
            this.rbtnHeat.InitButtonWidth = 144;
            this.rbtnHeat.IsChecked = false;
            this.rbtnHeat.Location = new System.Drawing.Point(330, 54);
            this.rbtnHeat.MouseOverBkgndImage = null;
            this.rbtnHeat.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportHeatTab_Hover;
            this.rbtnHeat.Name = "rbtnHeat";
            this.rbtnHeat.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportHeatTab_Normal;
            this.rbtnHeat.Owner = null;
            this.rbtnHeat.Size = new System.Drawing.Size(144, 36);
            this.rbtnHeat.TabIndex = 10;
            this.rbtnHeat.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnHeat.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnHeat.ToolTipText = "";
            this.rbtnHeat.UseCustomImageRect = true;
            this.rbtnHeat.UseTextLocation = true;
            this.rbtnHeat.UseVisualStyleBackColor = true;
            this.rbtnHeat.Click += new System.EventHandler(this.rbtnHeat_Click);
            // 
            // rbtnCollapse
            // 
            this.rbtnCollapse.CheckButton = false;
            this.rbtnCollapse.CheckedBkgndImage = null;
            this.rbtnCollapse.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportCollapseTab_Click;
            this.rbtnCollapse.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportCollapseTab_Click;
            this.rbtnCollapse.ClickedBackgroundImage = null;
            this.rbtnCollapse.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportCollapseTab_Click;
            this.rbtnCollapse.CustomImageRect = new System.Drawing.Rectangle(0, 0, 144, 36);
            this.rbtnCollapse.DisabledBkgndImage = null;
            this.rbtnCollapse.DisabledImage = null;
            this.rbtnCollapse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapse.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapse.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapse.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapse.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapse.ForeColorsByTypeUse = true;
            this.rbtnCollapse.ID = -1;
            this.rbtnCollapse.InitButtonWidth = 144;
            this.rbtnCollapse.IsChecked = false;
            this.rbtnCollapse.Location = new System.Drawing.Point(480, 54);
            this.rbtnCollapse.MouseOverBkgndImage = null;
            this.rbtnCollapse.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportCollapseTab_Hover;
            this.rbtnCollapse.Name = "rbtnCollapse";
            this.rbtnCollapse.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportCollapseTab_Normal;
            this.rbtnCollapse.Owner = null;
            this.rbtnCollapse.Size = new System.Drawing.Size(144, 36);
            this.rbtnCollapse.TabIndex = 11;
            this.rbtnCollapse.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnCollapse.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCollapse.ToolTipText = "";
            this.rbtnCollapse.UseCustomImageRect = true;
            this.rbtnCollapse.UseTextLocation = true;
            this.rbtnCollapse.UseVisualStyleBackColor = true;
            this.rbtnCollapse.Click += new System.EventHandler(this.rbtnCollapse_Click);
            // 
            // uFormManual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnRemoveManual);
            this.Controls.Add(this.btnAddManual);
            this.Controls.Add(this.rbtnFire);
            this.Controls.Add(this.rbtnFlood);
            this.Controls.Add(this.rbtnHeat);
            this.Controls.Add(this.rbtnCollapse);
            this.Controls.Add(this.plHeat);
            this.Controls.Add(this.plCollapse);
            this.Controls.Add(this.plFire);
            this.Controls.Add(this.plFlood);
            this.Name = "uFormManual";
            this.Size = new System.Drawing.Size(1600, 970);
            this.plCollapse.ResumeLayout(false);
            this.plHeat.ResumeLayout(false);
            this.plFlood.ResumeLayout(false);
            this.plFire.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnRemoveManual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddManual)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.RibbonButton rbtnFire;
        private UnE.GUI.RibbonButton rbtnFlood;
        private UnE.GUI.RibbonButton rbtnHeat;
        private UnE.GUI.RibbonButton rbtnCollapse;
        private System.Windows.Forms.Panel plCollapse;
        private System.Windows.Forms.Panel plHeat;
        private System.Windows.Forms.Panel plFlood;
        private System.Windows.Forms.Panel plFire;
        private UnE.GUI.RibbonButton rbtnFireAttention;
        private UnE.GUI.RibbonButton rbtnFireSerious;
        private UnE.GUI.RibbonButton rbtnFireAlert;
        private UnE.GUI.RibbonButton rbtnFireCaution;
        private UnE.GUI.RibbonButton rbtnHeatSerious;
        private UnE.GUI.RibbonButton rbtnHeatAlert;
        private UnE.GUI.RibbonButton rbtnHeatCaution;
        private UnE.GUI.RibbonButton rbtnHeatAttention;
        private UnE.GUI.RibbonButton rbtnFloodSerious;
        private UnE.GUI.RibbonButton rbtnFloodAlert;
        private UnE.GUI.RibbonButton rbtnFloodCaution;
        private UnE.GUI.RibbonButton rbtnFloodAttention;
        private UnE.GUI.RibbonButton rbtnCollapseSerious;
        private UnE.GUI.RibbonButton rbtnCollapseAlert;
        private UnE.GUI.RibbonButton rbtnCollapseCaution;
        private UnE.GUI.RibbonButton rbtnCollapseAttention;
        private UnE.GUI.ImageButton btnRemoveManual;
        private UnE.GUI.ImageButton btnAddManual;
        private System.Windows.Forms.Panel plFireManual;
        private System.Windows.Forms.Panel plFloodManual;
        private System.Windows.Forms.Panel plHeatManual;
        private System.Windows.Forms.Panel plCollapseManual;
    }
}
