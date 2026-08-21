namespace SDMS_Building.PopupDialog.Controls
{
    partial class uBroadcast
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
            this.rbtnPSM = new UnE.GUI.RibbonButton();
            this.rbtnBlackout = new UnE.GUI.RibbonButton();
            this.rbtnSubmergency = new UnE.GUI.RibbonButton();
            this.rbtnTerror = new UnE.GUI.RibbonButton();
            this.rbtnCorona = new UnE.GUI.RibbonButton();
            this.rbtnEarthquake = new UnE.GUI.RibbonButton();
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
            this.rbtnFire.CheckedImage = global::SDMS_Building.Properties.Resources.Fire_Quick_Selected;
            this.rbtnFire.CheckedMouseOver = global::SDMS_Building.Properties.Resources.Fire_Quick_Selected;
            this.rbtnFire.ClickedBackgroundImage = null;
            this.rbtnFire.ClickedImage = global::SDMS_Building.Properties.Resources.Fire_Quick_Selected;
            this.rbtnFire.CustomImageRect = new System.Drawing.Rectangle(0, 0, 55, 55);
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
            this.rbtnFire.Location = new System.Drawing.Point(2, 78);
            this.rbtnFire.MouseOverBkgndImage = null;
            this.rbtnFire.MouseOverImage = global::SDMS_Building.Properties.Resources.Fire_Quick_MouseOver;
            this.rbtnFire.Name = "rbtnFire";
            this.rbtnFire.NormalImage = global::SDMS_Building.Properties.Resources.Fire_Quick_Normal;
            this.rbtnFire.Owner = null;
            this.rbtnFire.Size = new System.Drawing.Size(55, 55);
            this.rbtnFire.TabIndex = 25;
            this.rbtnFire.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnFire.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFire.ToolTipText = "";
            this.rbtnFire.UseCustomImageRect = true;
            this.rbtnFire.UseTextLocation = false;
            this.rbtnFire.UseVisualStyleBackColor = true;
            this.rbtnFire.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnPSM
            // 
            this.rbtnPSM.CheckButton = false;
            this.rbtnPSM.CheckedBkgndImage = null;
            this.rbtnPSM.CheckedImage = global::SDMS_Building.Properties.Resources.PSM_Quick_Selected;
            this.rbtnPSM.CheckedMouseOver = global::SDMS_Building.Properties.Resources.PSM_Quick_Selected;
            this.rbtnPSM.ClickedBackgroundImage = null;
            this.rbtnPSM.ClickedImage = global::SDMS_Building.Properties.Resources.PSM_Quick_Selected;
            this.rbtnPSM.CustomImageRect = new System.Drawing.Rectangle(0, 0, 55, 55);
            this.rbtnPSM.DisabledBkgndImage = null;
            this.rbtnPSM.DisabledImage = null;
            this.rbtnPSM.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorsByTypeUse = false;
            this.rbtnPSM.ID = -1;
            this.rbtnPSM.InitButtonWidth = 55;
            this.rbtnPSM.IsChecked = false;
            this.rbtnPSM.Location = new System.Drawing.Point(3, 139);
            this.rbtnPSM.MouseOverBkgndImage = null;
            this.rbtnPSM.MouseOverImage = global::SDMS_Building.Properties.Resources.PSM_Quick_MouseOver;
            this.rbtnPSM.Name = "rbtnPSM";
            this.rbtnPSM.NormalImage = global::SDMS_Building.Properties.Resources.PSM_Quick_Normal;
            this.rbtnPSM.Owner = null;
            this.rbtnPSM.Size = new System.Drawing.Size(55, 55);
            this.rbtnPSM.TabIndex = 26;
            this.rbtnPSM.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnPSM.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnPSM.ToolTipText = "";
            this.rbtnPSM.UseCustomImageRect = true;
            this.rbtnPSM.UseTextLocation = false;
            this.rbtnPSM.UseVisualStyleBackColor = true;
            this.rbtnPSM.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnBlackout
            // 
            this.rbtnBlackout.CheckButton = false;
            this.rbtnBlackout.CheckedBkgndImage = null;
            this.rbtnBlackout.CheckedImage = global::SDMS_Building.Properties.Resources.PowerOff_Quick_Selected;
            this.rbtnBlackout.CheckedMouseOver = global::SDMS_Building.Properties.Resources.PowerOff_Quick_Selected;
            this.rbtnBlackout.ClickedBackgroundImage = null;
            this.rbtnBlackout.ClickedImage = global::SDMS_Building.Properties.Resources.PowerOff_Quick_Selected;
            this.rbtnBlackout.CustomImageRect = new System.Drawing.Rectangle(0, 0, 55, 55);
            this.rbtnBlackout.DisabledBkgndImage = null;
            this.rbtnBlackout.DisabledImage = null;
            this.rbtnBlackout.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnBlackout.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnBlackout.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnBlackout.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnBlackout.ForeColorsByTypeUse = false;
            this.rbtnBlackout.ID = -1;
            this.rbtnBlackout.InitButtonWidth = 55;
            this.rbtnBlackout.IsChecked = false;
            this.rbtnBlackout.Location = new System.Drawing.Point(3, 200);
            this.rbtnBlackout.MouseOverBkgndImage = null;
            this.rbtnBlackout.MouseOverImage = global::SDMS_Building.Properties.Resources.PowerOff_Quick_MouseOver;
            this.rbtnBlackout.Name = "rbtnBlackout";
            this.rbtnBlackout.NormalImage = global::SDMS_Building.Properties.Resources.PowerOff_Quick_Normal;
            this.rbtnBlackout.Owner = null;
            this.rbtnBlackout.Size = new System.Drawing.Size(55, 55);
            this.rbtnBlackout.TabIndex = 27;
            this.rbtnBlackout.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnBlackout.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnBlackout.ToolTipText = "";
            this.rbtnBlackout.UseCustomImageRect = true;
            this.rbtnBlackout.UseTextLocation = false;
            this.rbtnBlackout.UseVisualStyleBackColor = true;
            this.rbtnBlackout.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnSubmergency
            // 
            this.rbtnSubmergency.CheckButton = false;
            this.rbtnSubmergency.CheckedBkgndImage = null;
            this.rbtnSubmergency.CheckedImage = global::SDMS_Building.Properties.Resources.Flood_Quick_Selected;
            this.rbtnSubmergency.CheckedMouseOver = global::SDMS_Building.Properties.Resources.Flood_Quick_Selected;
            this.rbtnSubmergency.ClickedBackgroundImage = null;
            this.rbtnSubmergency.ClickedImage = global::SDMS_Building.Properties.Resources.Flood_Quick_Selected;
            this.rbtnSubmergency.CustomImageRect = new System.Drawing.Rectangle(0, 0, 55, 55);
            this.rbtnSubmergency.DisabledBkgndImage = null;
            this.rbtnSubmergency.DisabledImage = null;
            this.rbtnSubmergency.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnSubmergency.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnSubmergency.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnSubmergency.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnSubmergency.ForeColorsByTypeUse = false;
            this.rbtnSubmergency.ID = -1;
            this.rbtnSubmergency.InitButtonWidth = 55;
            this.rbtnSubmergency.IsChecked = false;
            this.rbtnSubmergency.Location = new System.Drawing.Point(2, 261);
            this.rbtnSubmergency.MouseOverBkgndImage = null;
            this.rbtnSubmergency.MouseOverImage = global::SDMS_Building.Properties.Resources.Flood_Quick_MouseOver;
            this.rbtnSubmergency.Name = "rbtnSubmergency";
            this.rbtnSubmergency.NormalImage = global::SDMS_Building.Properties.Resources.Flood_Quick_Normal;
            this.rbtnSubmergency.Owner = null;
            this.rbtnSubmergency.Size = new System.Drawing.Size(55, 55);
            this.rbtnSubmergency.TabIndex = 28;
            this.rbtnSubmergency.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSubmergency.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSubmergency.ToolTipText = "";
            this.rbtnSubmergency.UseCustomImageRect = true;
            this.rbtnSubmergency.UseTextLocation = false;
            this.rbtnSubmergency.UseVisualStyleBackColor = true;
            this.rbtnSubmergency.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnTerror
            // 
            this.rbtnTerror.CheckButton = false;
            this.rbtnTerror.CheckedBkgndImage = null;
            this.rbtnTerror.CheckedImage = global::SDMS_Building.Properties.Resources.Terror_Quick_Selected;
            this.rbtnTerror.CheckedMouseOver = global::SDMS_Building.Properties.Resources.Terror_Quick_Selected;
            this.rbtnTerror.ClickedBackgroundImage = null;
            this.rbtnTerror.ClickedImage = global::SDMS_Building.Properties.Resources.Terror_Quick_Selected;
            this.rbtnTerror.CustomImageRect = new System.Drawing.Rectangle(0, 0, 55, 55);
            this.rbtnTerror.DisabledBkgndImage = null;
            this.rbtnTerror.DisabledImage = null;
            this.rbtnTerror.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnTerror.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnTerror.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnTerror.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnTerror.ForeColorsByTypeUse = false;
            this.rbtnTerror.ID = -1;
            this.rbtnTerror.InitButtonWidth = 55;
            this.rbtnTerror.IsChecked = false;
            this.rbtnTerror.Location = new System.Drawing.Point(2, 322);
            this.rbtnTerror.MouseOverBkgndImage = null;
            this.rbtnTerror.MouseOverImage = global::SDMS_Building.Properties.Resources.Terror_Quick_MouseOver;
            this.rbtnTerror.Name = "rbtnTerror";
            this.rbtnTerror.NormalImage = global::SDMS_Building.Properties.Resources.Terror_Quick_Normal;
            this.rbtnTerror.Owner = null;
            this.rbtnTerror.Size = new System.Drawing.Size(55, 55);
            this.rbtnTerror.TabIndex = 29;
            this.rbtnTerror.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnTerror.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnTerror.ToolTipText = "";
            this.rbtnTerror.UseCustomImageRect = true;
            this.rbtnTerror.UseTextLocation = false;
            this.rbtnTerror.UseVisualStyleBackColor = true;
            this.rbtnTerror.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnCorona
            // 
            this.rbtnCorona.CheckButton = false;
            this.rbtnCorona.CheckedBkgndImage = null;
            this.rbtnCorona.CheckedImage = global::SDMS_Building.Properties.Resources.Corona_Quick_Selected;
            this.rbtnCorona.CheckedMouseOver = global::SDMS_Building.Properties.Resources.Corona_Quick_Selected;
            this.rbtnCorona.ClickedBackgroundImage = null;
            this.rbtnCorona.ClickedImage = global::SDMS_Building.Properties.Resources.Corona_Quick_Selected;
            this.rbtnCorona.CustomImageRect = new System.Drawing.Rectangle(0, 0, 55, 55);
            this.rbtnCorona.DisabledBkgndImage = null;
            this.rbtnCorona.DisabledImage = null;
            this.rbtnCorona.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnCorona.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnCorona.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnCorona.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnCorona.ForeColorsByTypeUse = false;
            this.rbtnCorona.ID = -1;
            this.rbtnCorona.InitButtonWidth = 55;
            this.rbtnCorona.IsChecked = false;
            this.rbtnCorona.Location = new System.Drawing.Point(2, 383);
            this.rbtnCorona.MouseOverBkgndImage = null;
            this.rbtnCorona.MouseOverImage = global::SDMS_Building.Properties.Resources.Corona_Quick_MouseOver;
            this.rbtnCorona.Name = "rbtnCorona";
            this.rbtnCorona.NormalImage = global::SDMS_Building.Properties.Resources.Corona_Quick_Normal;
            this.rbtnCorona.Owner = null;
            this.rbtnCorona.Size = new System.Drawing.Size(55, 55);
            this.rbtnCorona.TabIndex = 30;
            this.rbtnCorona.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnCorona.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCorona.ToolTipText = "";
            this.rbtnCorona.UseCustomImageRect = true;
            this.rbtnCorona.UseTextLocation = false;
            this.rbtnCorona.UseVisualStyleBackColor = true;
            this.rbtnCorona.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnEarthquake
            // 
            this.rbtnEarthquake.CheckButton = false;
            this.rbtnEarthquake.CheckedBkgndImage = null;
            this.rbtnEarthquake.CheckedImage = global::SDMS_Building.Properties.Resources.Earthquake_Quick_Selected;
            this.rbtnEarthquake.CheckedMouseOver = global::SDMS_Building.Properties.Resources.Earthquake_Quick_Selected;
            this.rbtnEarthquake.ClickedBackgroundImage = null;
            this.rbtnEarthquake.ClickedImage = global::SDMS_Building.Properties.Resources.Earthquake_Quick_Selected;
            this.rbtnEarthquake.CustomImageRect = new System.Drawing.Rectangle(0, 0, 55, 55);
            this.rbtnEarthquake.DisabledBkgndImage = null;
            this.rbtnEarthquake.DisabledImage = null;
            this.rbtnEarthquake.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnEarthquake.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnEarthquake.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnEarthquake.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnEarthquake.ForeColorsByTypeUse = false;
            this.rbtnEarthquake.ID = -1;
            this.rbtnEarthquake.InitButtonWidth = 55;
            this.rbtnEarthquake.IsChecked = false;
            this.rbtnEarthquake.Location = new System.Drawing.Point(3, 444);
            this.rbtnEarthquake.MouseOverBkgndImage = null;
            this.rbtnEarthquake.MouseOverImage = global::SDMS_Building.Properties.Resources.Earthquake_Quick_MouseOver;
            this.rbtnEarthquake.Name = "rbtnEarthquake";
            this.rbtnEarthquake.NormalImage = global::SDMS_Building.Properties.Resources.Earthquake_Quick_Normal;
            this.rbtnEarthquake.Owner = null;
            this.rbtnEarthquake.Size = new System.Drawing.Size(55, 55);
            this.rbtnEarthquake.TabIndex = 31;
            this.rbtnEarthquake.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnEarthquake.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnEarthquake.ToolTipText = "";
            this.rbtnEarthquake.UseCustomImageRect = true;
            this.rbtnEarthquake.UseTextLocation = false;
            this.rbtnEarthquake.UseVisualStyleBackColor = true;
            this.rbtnEarthquake.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // uBroadcast
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rbtnEarthquake);
            this.Controls.Add(this.rbtnCorona);
            this.Controls.Add(this.rbtnTerror);
            this.Controls.Add(this.rbtnSubmergency);
            this.Controls.Add(this.rbtnBlackout);
            this.Controls.Add(this.rbtnPSM);
            this.Controls.Add(this.rbtnFire);
            this.Controls.Add(this.btnMain);
            this.Name = "uBroadcast";
            this.Size = new System.Drawing.Size(60, 508);
            this.Load += new System.EventHandler(this.uBroadcast_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private UnE.GUI.ImageButton btnMain;
        private UnE.GUI.RibbonButton rbtnFire;
        private UnE.GUI.RibbonButton rbtnPSM;
        private UnE.GUI.RibbonButton rbtnBlackout;
        private UnE.GUI.RibbonButton rbtnSubmergency;
        private UnE.GUI.RibbonButton rbtnTerror;
        private UnE.GUI.RibbonButton rbtnCorona;
        private UnE.GUI.RibbonButton rbtnEarthquake;
    }
}
