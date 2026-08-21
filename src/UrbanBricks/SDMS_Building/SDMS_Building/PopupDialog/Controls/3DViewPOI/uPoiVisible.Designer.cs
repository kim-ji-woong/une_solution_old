namespace SDMS_Building.PopupDialog.Controls
{
    partial class uPoiVisible
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
            this.btnMain = new UnE.GUI.ImageButton();
            this.rbtnFire = new UnE.GUI.RibbonButton();
            this.rbtnCCTV = new UnE.GUI.RibbonButton();
            this.rbtnPSM = new UnE.GUI.RibbonButton();
            this.rbtnDoor = new UnE.GUI.RibbonButton();
            this.rbtnFireWall = new UnE.GUI.RibbonButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnMain)).BeginInit();
            this.SuspendLayout();
            // 
            // btnMain
            // 
            this.btnMain.BackColor = System.Drawing.Color.Transparent;
            this.btnMain.ButtonText = "";
            this.btnMain.ImageClicked = global::SDMS_Building.Properties.Resources.close2_click;
            this.btnMain.ImageDisabled = null;
            this.btnMain.ImageMouseOver = global::SDMS_Building.Properties.Resources.close2_hover;
            this.btnMain.ImageNormal = global::SDMS_Building.Properties.Resources.close2_normal;
            this.btnMain.Location = new System.Drawing.Point(3, 17);
            this.btnMain.Name = "btnMain";
            this.btnMain.Owner = null;
            this.btnMain.Size = new System.Drawing.Size(55, 55);
            this.btnMain.TabIndex = 24;
            this.btnMain.TabStop = false;
            this.btnMain.TextColor = System.Drawing.Color.Black;
            this.btnMain.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMain.ToolTipText = "";
            this.btnMain.UseToolTip = false;
            this.btnMain.WindowRateWidth = 1F;
            this.btnMain.Click += new System.EventHandler(this.btnMain_Click);
            // 
            // rbtnFire
            // 
            this.rbtnFire.CheckButton = false;
            this.rbtnFire.CheckedBkgndImage = null;
            this.rbtnFire.CheckedImage = global::SDMS_Building.Properties.Resources.poi_fire_click;
            this.rbtnFire.CheckedMouseOver = global::SDMS_Building.Properties.Resources.poi_fire_click;
            this.rbtnFire.ClickedBackgroundImage = null;
            this.rbtnFire.ClickedImage = global::SDMS_Building.Properties.Resources.poi_fire_click;
            this.rbtnFire.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnFire.DisabledBkgndImage = null;
            this.rbtnFire.DisabledImage = null;
            this.rbtnFire.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnFire.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnFire.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnFire.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnFire.ForeColorsByTypeUse = false;
            this.rbtnFire.ID = -1;
            this.rbtnFire.InitButtonWidth = 55;
            this.rbtnFire.IsChecked = false;
            this.rbtnFire.Location = new System.Drawing.Point(3, 78);
            this.rbtnFire.MouseOverBkgndImage = null;
            this.rbtnFire.MouseOverImage = global::SDMS_Building.Properties.Resources.poi_fire_normal;
            this.rbtnFire.Name = "rbtnFire";
            this.rbtnFire.NormalImage = global::SDMS_Building.Properties.Resources.poi_fire_normal;
            this.rbtnFire.Owner = null;
            this.rbtnFire.Size = new System.Drawing.Size(55, 60);
            this.rbtnFire.TabIndex = 25;
            this.rbtnFire.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnFire.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFire.ToolTipText = "";
            this.rbtnFire.UseCustomImageRect = false;
            this.rbtnFire.UseTextLocation = false;
            this.rbtnFire.UseVisualStyleBackColor = true;
            this.rbtnFire.Click += new System.EventHandler(this.rbtnFire_Click);
            // 
            // rbtnCCTV
            // 
            this.rbtnCCTV.CheckButton = false;
            this.rbtnCCTV.CheckedBkgndImage = null;
            this.rbtnCCTV.CheckedImage = global::SDMS_Building.Properties.Resources.poi_cctv_click;
            this.rbtnCCTV.CheckedMouseOver = global::SDMS_Building.Properties.Resources.poi_cctv_click;
            this.rbtnCCTV.ClickedBackgroundImage = null;
            this.rbtnCCTV.ClickedImage = global::SDMS_Building.Properties.Resources.poi_cctv_click;
            this.rbtnCCTV.CustomImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.rbtnCCTV.DisabledBkgndImage = null;
            this.rbtnCCTV.DisabledImage = null;
            this.rbtnCCTV.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnCCTV.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnCCTV.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnCCTV.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnCCTV.ForeColorsByTypeUse = false;
            this.rbtnCCTV.ID = -1;
            this.rbtnCCTV.InitButtonWidth = 55;
            this.rbtnCCTV.IsChecked = false;
            this.rbtnCCTV.Location = new System.Drawing.Point(3, 159);
            this.rbtnCCTV.MouseOverBkgndImage = null;
            this.rbtnCCTV.MouseOverImage = global::SDMS_Building.Properties.Resources.poi_cctv_normal;
            this.rbtnCCTV.Name = "rbtnCCTV";
            this.rbtnCCTV.NormalImage = global::SDMS_Building.Properties.Resources.poi_cctv_normal;
            this.rbtnCCTV.Owner = null;
            this.rbtnCCTV.Size = new System.Drawing.Size(55, 60);
            this.rbtnCCTV.TabIndex = 26;
            this.rbtnCCTV.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnCCTV.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCCTV.ToolTipText = "";
            this.rbtnCCTV.UseCustomImageRect = false;
            this.rbtnCCTV.UseTextLocation = false;
            this.rbtnCCTV.UseVisualStyleBackColor = true;
            this.rbtnCCTV.Click += new System.EventHandler(this.rbtnCCTV_Click);
            // 
            // rbtnPSM
            // 
            this.rbtnPSM.CheckButton = false;
            this.rbtnPSM.CheckedBkgndImage = null;
            this.rbtnPSM.CheckedImage = global::SDMS_Building.Properties.Resources.poi_psm_click;
            this.rbtnPSM.CheckedMouseOver = global::SDMS_Building.Properties.Resources.poi_psm_click;
            this.rbtnPSM.ClickedBackgroundImage = null;
            this.rbtnPSM.ClickedImage = global::SDMS_Building.Properties.Resources.poi_psm_click;
            this.rbtnPSM.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnPSM.DisabledBkgndImage = null;
            this.rbtnPSM.DisabledImage = null;
            this.rbtnPSM.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorsByTypeUse = false;
            this.rbtnPSM.ID = -1;
            this.rbtnPSM.InitButtonWidth = 60;
            this.rbtnPSM.IsChecked = false;
            this.rbtnPSM.Location = new System.Drawing.Point(3, 228);
            this.rbtnPSM.MouseOverBkgndImage = null;
            this.rbtnPSM.MouseOverImage = global::SDMS_Building.Properties.Resources.poi_psm_normal;
            this.rbtnPSM.Name = "rbtnPSM";
            this.rbtnPSM.NormalImage = global::SDMS_Building.Properties.Resources.poi_psm_normal;
            this.rbtnPSM.Owner = null;
            this.rbtnPSM.Size = new System.Drawing.Size(60, 60);
            this.rbtnPSM.TabIndex = 27;
            this.rbtnPSM.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnPSM.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnPSM.ToolTipText = "";
            this.rbtnPSM.UseCustomImageRect = false;
            this.rbtnPSM.UseTextLocation = false;
            this.rbtnPSM.UseVisualStyleBackColor = true;
            this.rbtnPSM.Visible = false;
            this.rbtnPSM.Click += new System.EventHandler(this.rbtnPSM_Click);
            // 
            // rbtnDoor
            // 
            this.rbtnDoor.CheckButton = false;
            this.rbtnDoor.CheckedBkgndImage = null;
            this.rbtnDoor.CheckedImage = global::SDMS_Building.Properties.Resources.poi_door_click;
            this.rbtnDoor.CheckedMouseOver = global::SDMS_Building.Properties.Resources.poi_door_click;
            this.rbtnDoor.ClickedBackgroundImage = null;
            this.rbtnDoor.ClickedImage = global::SDMS_Building.Properties.Resources.poi_door_click;
            this.rbtnDoor.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnDoor.DisabledBkgndImage = null;
            this.rbtnDoor.DisabledImage = null;
            this.rbtnDoor.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnDoor.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnDoor.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnDoor.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnDoor.ForeColorsByTypeUse = false;
            this.rbtnDoor.ID = -1;
            this.rbtnDoor.InitButtonWidth = 60;
            this.rbtnDoor.IsChecked = false;
            this.rbtnDoor.Location = new System.Drawing.Point(3, 294);
            this.rbtnDoor.MouseOverBkgndImage = null;
            this.rbtnDoor.MouseOverImage = global::SDMS_Building.Properties.Resources.poi_door_normal;
            this.rbtnDoor.Name = "rbtnDoor";
            this.rbtnDoor.NormalImage = global::SDMS_Building.Properties.Resources.poi_door_normal;
            this.rbtnDoor.Owner = null;
            this.rbtnDoor.Size = new System.Drawing.Size(60, 60);
            this.rbtnDoor.TabIndex = 28;
            this.rbtnDoor.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnDoor.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnDoor.ToolTipText = "";
            this.rbtnDoor.UseCustomImageRect = false;
            this.rbtnDoor.UseTextLocation = false;
            this.rbtnDoor.UseVisualStyleBackColor = true;
            this.rbtnDoor.Visible = false;
            this.rbtnDoor.Click += new System.EventHandler(this.rbtnDoor_Click);
            // 
            // rbtnFireWall
            // 
            this.rbtnFireWall.CheckButton = false;
            this.rbtnFireWall.CheckedBkgndImage = null;
            this.rbtnFireWall.CheckedImage = global::SDMS_Building.Properties.Resources.poi_firewall_click;
            this.rbtnFireWall.CheckedMouseOver = global::SDMS_Building.Properties.Resources.poi_firewall_click;
            this.rbtnFireWall.ClickedBackgroundImage = null;
            this.rbtnFireWall.ClickedImage = global::SDMS_Building.Properties.Resources.poi_firewall_click;
            this.rbtnFireWall.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnFireWall.DisabledBkgndImage = null;
            this.rbtnFireWall.DisabledImage = null;
            this.rbtnFireWall.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnFireWall.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnFireWall.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnFireWall.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnFireWall.ForeColorsByTypeUse = false;
            this.rbtnFireWall.ID = -1;
            this.rbtnFireWall.InitButtonWidth = 60;
            this.rbtnFireWall.IsChecked = false;
            this.rbtnFireWall.Location = new System.Drawing.Point(3, 360);
            this.rbtnFireWall.MouseOverBkgndImage = null;
            this.rbtnFireWall.MouseOverImage = global::SDMS_Building.Properties.Resources.poi_firewall_normal;
            this.rbtnFireWall.Name = "rbtnFireWall";
            this.rbtnFireWall.NormalImage = global::SDMS_Building.Properties.Resources.poi_firewall_normal;
            this.rbtnFireWall.Owner = null;
            this.rbtnFireWall.Size = new System.Drawing.Size(60, 60);
            this.rbtnFireWall.TabIndex = 29;
            this.rbtnFireWall.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnFireWall.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFireWall.ToolTipText = "";
            this.rbtnFireWall.UseCustomImageRect = false;
            this.rbtnFireWall.UseTextLocation = false;
            this.rbtnFireWall.UseVisualStyleBackColor = true;
            this.rbtnFireWall.Visible = false;
            this.rbtnFireWall.Click += new System.EventHandler(this.rbtnFireWall_Click);
            // 
            // uPoiVisible
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rbtnFireWall);
            this.Controls.Add(this.rbtnDoor);
            this.Controls.Add(this.rbtnPSM);
            this.Controls.Add(this.rbtnCCTV);
            this.Controls.Add(this.rbtnFire);
            this.Controls.Add(this.btnMain);
            this.Name = "uPoiVisible";
            this.Size = new System.Drawing.Size(60, 422);
            this.Load += new System.EventHandler(this.uPoiVisible_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.uPoiVisible_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.btnMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private UnE.GUI.ImageButton btnMain;
        private UnE.GUI.RibbonButton rbtnFire;
        private UnE.GUI.RibbonButton rbtnCCTV;
        private UnE.GUI.RibbonButton rbtnPSM;
        private UnE.GUI.RibbonButton rbtnDoor;
        private UnE.GUI.RibbonButton rbtnFireWall;
    }
}
