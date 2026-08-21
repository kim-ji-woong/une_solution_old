namespace SDMS_Building.PopupDialog.Config
{
    partial class FormManagement
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.ImageButton();
            this.btnManagerTab = new UnE.GUI.RibbonButton();
            this.btnConfirm = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.btnSensorListTab = new UnE.GUI.RibbonButton();
            this.btnDetectPolicyTab = new UnE.GUI.RibbonButton();
            this.btnEarthquakeTab = new UnE.GUI.RibbonButton();
            this.btnBroadcast = new UnE.GUI.RibbonButton();
            this.btnSMS = new UnE.GUI.RibbonButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1000, 80);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(57, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 27);
            this.label1.TabIndex = 17;
            this.label1.Text = "관리";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.panel3.Location = new System.Drawing.Point(30, 38);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(7, 7);
            this.panel3.TabIndex = 16;
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::SDMS_Building.Properties.Resources.close_Click;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SDMS_Building.Properties.Resources.close_Hover;
            this.btnClose.ImageNormal = global::SDMS_Building.Properties.Resources.close_Normal;
            this.btnClose.Location = new System.Drawing.Point(939, 26);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(30, 30);
            this.btnClose.TabIndex = 15;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnManagerTab
            // 
            this.btnManagerTab.CheckButton = false;
            this.btnManagerTab.CheckedBkgndImage = null;
            this.btnManagerTab.CheckedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnManagerTab.CheckedMouseOver = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnManagerTab.ClickedBackgroundImage = null;
            this.btnManagerTab.ClickedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnManagerTab.CustomImageRect = new System.Drawing.Rectangle(0, 0, 158, 50);
            this.btnManagerTab.DisabledBkgndImage = null;
            this.btnManagerTab.DisabledImage = null;
            this.btnManagerTab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(123)))), ((int)(((byte)(123)))));
            this.btnManagerTab.ForeColorChecked = System.Drawing.Color.White;
            this.btnManagerTab.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnManagerTab.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnManagerTab.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(131)))), ((int)(((byte)(131)))));
            this.btnManagerTab.ForeColorsByTypeUse = true;
            this.btnManagerTab.ID = -1;
            this.btnManagerTab.InitButtonWidth = 158;
            this.btnManagerTab.IsChecked = true;
            this.btnManagerTab.Location = new System.Drawing.Point(21, 100);
            this.btnManagerTab.MouseOverBkgndImage = null;
            this.btnManagerTab.MouseOverImage = global::SDMS_Building.Properties.Resources.mgr_hover;
            this.btnManagerTab.Name = "btnManagerTab";
            this.btnManagerTab.NormalImage = global::SDMS_Building.Properties.Resources.mgr_unchecked;
            this.btnManagerTab.Owner = null;
            this.btnManagerTab.Size = new System.Drawing.Size(158, 50);
            this.btnManagerTab.TabIndex = 23;
            this.btnManagerTab.Text = "담당자 관리";
            this.btnManagerTab.TextLocation = new System.Drawing.Point(0, 13);
            this.btnManagerTab.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnManagerTab.ToolTipText = "담당자 관리";
            this.btnManagerTab.UseCustomImageRect = true;
            this.btnManagerTab.UseTextLocation = true;
            this.btnManagerTab.UseVisualStyleBackColor = true;
            this.btnManagerTab.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.ButtonText = "";
            this.btnConfirm.ImageClicked = global::SDMS_Building.Properties.Resources.ok_click;
            this.btnConfirm.ImageDisabled = null;
            this.btnConfirm.ImageMouseOver = global::SDMS_Building.Properties.Resources.ok_hover;
            this.btnConfirm.ImageNormal = global::SDMS_Building.Properties.Resources.ok_normal;
            this.btnConfirm.Location = new System.Drawing.Point(300, 674);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(195, 60);
            this.btnConfirm.TabIndex = 22;
            this.btnConfirm.TabStop = false;
            this.btnConfirm.TextColor = System.Drawing.Color.White;
            this.btnConfirm.TextFont = new System.Drawing.Font("나눔바른고딕", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfirm.ToolTipText = "";
            this.btnConfirm.UseToolTip = false;
            this.btnConfirm.WindowRateWidth = 1F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonText = "";
            this.btnCancel.ImageClicked = global::SDMS_Building.Properties.Resources.cancel_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SDMS_Building.Properties.Resources.cancel_Hover;
            this.btnCancel.ImageNormal = global::SDMS_Building.Properties.Resources.cancel_Normal;
            this.btnCancel.Location = new System.Drawing.Point(505, 674);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(195, 60);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSensorListTab
            // 
            this.btnSensorListTab.CheckButton = false;
            this.btnSensorListTab.CheckedBkgndImage = null;
            this.btnSensorListTab.CheckedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnSensorListTab.CheckedMouseOver = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnSensorListTab.ClickedBackgroundImage = null;
            this.btnSensorListTab.ClickedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnSensorListTab.CustomImageRect = new System.Drawing.Rectangle(0, 0, 158, 50);
            this.btnSensorListTab.DisabledBkgndImage = null;
            this.btnSensorListTab.DisabledImage = null;
            this.btnSensorListTab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(123)))), ((int)(((byte)(123)))));
            this.btnSensorListTab.ForeColorChecked = System.Drawing.Color.White;
            this.btnSensorListTab.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSensorListTab.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnSensorListTab.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(131)))), ((int)(((byte)(131)))));
            this.btnSensorListTab.ForeColorsByTypeUse = true;
            this.btnSensorListTab.ID = -1;
            this.btnSensorListTab.InitButtonWidth = 158;
            this.btnSensorListTab.IsChecked = false;
            this.btnSensorListTab.Location = new System.Drawing.Point(181, 100);
            this.btnSensorListTab.MouseOverBkgndImage = null;
            this.btnSensorListTab.MouseOverImage = global::SDMS_Building.Properties.Resources.mgr_hover;
            this.btnSensorListTab.Name = "btnSensorListTab";
            this.btnSensorListTab.NormalImage = global::SDMS_Building.Properties.Resources.mgr_unchecked;
            this.btnSensorListTab.Owner = null;
            this.btnSensorListTab.Size = new System.Drawing.Size(158, 50);
            this.btnSensorListTab.TabIndex = 24;
            this.btnSensorListTab.Text = "모든 설비 목록";
            this.btnSensorListTab.TextLocation = new System.Drawing.Point(0, 13);
            this.btnSensorListTab.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSensorListTab.ToolTipText = "모든 설비 목록";
            this.btnSensorListTab.UseCustomImageRect = true;
            this.btnSensorListTab.UseTextLocation = true;
            this.btnSensorListTab.UseVisualStyleBackColor = true;
            this.btnSensorListTab.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // btnDetectPolicyTab
            // 
            this.btnDetectPolicyTab.CheckButton = false;
            this.btnDetectPolicyTab.CheckedBkgndImage = null;
            this.btnDetectPolicyTab.CheckedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnDetectPolicyTab.CheckedMouseOver = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnDetectPolicyTab.ClickedBackgroundImage = null;
            this.btnDetectPolicyTab.ClickedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnDetectPolicyTab.CustomImageRect = new System.Drawing.Rectangle(0, 0, 159, 50);
            this.btnDetectPolicyTab.DisabledBkgndImage = null;
            this.btnDetectPolicyTab.DisabledImage = null;
            this.btnDetectPolicyTab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(123)))), ((int)(((byte)(123)))));
            this.btnDetectPolicyTab.ForeColorChecked = System.Drawing.Color.White;
            this.btnDetectPolicyTab.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnDetectPolicyTab.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnDetectPolicyTab.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(131)))), ((int)(((byte)(131)))));
            this.btnDetectPolicyTab.ForeColorsByTypeUse = true;
            this.btnDetectPolicyTab.ID = -1;
            this.btnDetectPolicyTab.InitButtonWidth = 159;
            this.btnDetectPolicyTab.IsChecked = false;
            this.btnDetectPolicyTab.Location = new System.Drawing.Point(340, 100);
            this.btnDetectPolicyTab.MouseOverBkgndImage = null;
            this.btnDetectPolicyTab.MouseOverImage = global::SDMS_Building.Properties.Resources.mgr_hover;
            this.btnDetectPolicyTab.Name = "btnDetectPolicyTab";
            this.btnDetectPolicyTab.NormalImage = global::SDMS_Building.Properties.Resources.mgr_unchecked;
            this.btnDetectPolicyTab.Owner = null;
            this.btnDetectPolicyTab.Size = new System.Drawing.Size(159, 50);
            this.btnDetectPolicyTab.TabIndex = 25;
            this.btnDetectPolicyTab.Text = "탐지 관리";
            this.btnDetectPolicyTab.TextLocation = new System.Drawing.Point(0, 13);
            this.btnDetectPolicyTab.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDetectPolicyTab.ToolTipText = "탐지 관리";
            this.btnDetectPolicyTab.UseCustomImageRect = true;
            this.btnDetectPolicyTab.UseTextLocation = true;
            this.btnDetectPolicyTab.UseVisualStyleBackColor = true;
            this.btnDetectPolicyTab.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // btnEarthquakeTab
            // 
            this.btnEarthquakeTab.CheckButton = false;
            this.btnEarthquakeTab.CheckedBkgndImage = null;
            this.btnEarthquakeTab.CheckedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnEarthquakeTab.CheckedMouseOver = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnEarthquakeTab.ClickedBackgroundImage = null;
            this.btnEarthquakeTab.ClickedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnEarthquakeTab.CustomImageRect = new System.Drawing.Rectangle(0, 0, 159, 50);
            this.btnEarthquakeTab.DisabledBkgndImage = null;
            this.btnEarthquakeTab.DisabledImage = null;
            this.btnEarthquakeTab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(123)))), ((int)(((byte)(123)))));
            this.btnEarthquakeTab.ForeColorChecked = System.Drawing.Color.White;
            this.btnEarthquakeTab.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnEarthquakeTab.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnEarthquakeTab.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(131)))), ((int)(((byte)(131)))));
            this.btnEarthquakeTab.ForeColorsByTypeUse = true;
            this.btnEarthquakeTab.ID = -1;
            this.btnEarthquakeTab.InitButtonWidth = 159;
            this.btnEarthquakeTab.IsChecked = false;
            this.btnEarthquakeTab.Location = new System.Drawing.Point(500, 100);
            this.btnEarthquakeTab.MouseOverBkgndImage = null;
            this.btnEarthquakeTab.MouseOverImage = global::SDMS_Building.Properties.Resources.mgr_hover;
            this.btnEarthquakeTab.Name = "btnEarthquakeTab";
            this.btnEarthquakeTab.NormalImage = global::SDMS_Building.Properties.Resources.mgr_unchecked;
            this.btnEarthquakeTab.Owner = null;
            this.btnEarthquakeTab.Size = new System.Drawing.Size(159, 50);
            this.btnEarthquakeTab.TabIndex = 26;
            this.btnEarthquakeTab.Text = "지진 관리";
            this.btnEarthquakeTab.TextLocation = new System.Drawing.Point(0, 13);
            this.btnEarthquakeTab.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnEarthquakeTab.ToolTipText = "지진 관리";
            this.btnEarthquakeTab.UseCustomImageRect = true;
            this.btnEarthquakeTab.UseTextLocation = true;
            this.btnEarthquakeTab.UseVisualStyleBackColor = true;
            this.btnEarthquakeTab.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // btnBroadcast
            // 
            this.btnBroadcast.CheckButton = false;
            this.btnBroadcast.CheckedBkgndImage = null;
            this.btnBroadcast.CheckedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnBroadcast.CheckedMouseOver = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnBroadcast.ClickedBackgroundImage = null;
            this.btnBroadcast.ClickedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnBroadcast.CustomImageRect = new System.Drawing.Rectangle(0, 0, 159, 50);
            this.btnBroadcast.DisabledBkgndImage = null;
            this.btnBroadcast.DisabledImage = null;
            this.btnBroadcast.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(123)))), ((int)(((byte)(123)))));
            this.btnBroadcast.ForeColorChecked = System.Drawing.Color.White;
            this.btnBroadcast.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnBroadcast.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnBroadcast.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(131)))), ((int)(((byte)(131)))));
            this.btnBroadcast.ForeColorsByTypeUse = true;
            this.btnBroadcast.ID = -1;
            this.btnBroadcast.InitButtonWidth = 159;
            this.btnBroadcast.IsChecked = false;
            this.btnBroadcast.Location = new System.Drawing.Point(660, 100);
            this.btnBroadcast.MouseOverBkgndImage = null;
            this.btnBroadcast.MouseOverImage = global::SDMS_Building.Properties.Resources.mgr_hover;
            this.btnBroadcast.Name = "btnBroadcast";
            this.btnBroadcast.NormalImage = global::SDMS_Building.Properties.Resources.mgr_unchecked;
            this.btnBroadcast.Owner = null;
            this.btnBroadcast.Size = new System.Drawing.Size(159, 50);
            this.btnBroadcast.TabIndex = 27;
            this.btnBroadcast.Text = "방송 관리";
            this.btnBroadcast.TextLocation = new System.Drawing.Point(0, 13);
            this.btnBroadcast.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnBroadcast.ToolTipText = "방송 관리";
            this.btnBroadcast.UseCustomImageRect = true;
            this.btnBroadcast.UseTextLocation = true;
            this.btnBroadcast.UseVisualStyleBackColor = true;
            this.btnBroadcast.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // btnSMS
            // 
            this.btnSMS.CheckButton = false;
            this.btnSMS.CheckedBkgndImage = null;
            this.btnSMS.CheckedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnSMS.CheckedMouseOver = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnSMS.ClickedBackgroundImage = null;
            this.btnSMS.ClickedImage = global::SDMS_Building.Properties.Resources.mgr_checked;
            this.btnSMS.CustomImageRect = new System.Drawing.Rectangle(0, 0, 159, 50);
            this.btnSMS.DisabledBkgndImage = null;
            this.btnSMS.DisabledImage = null;
            this.btnSMS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(123)))), ((int)(((byte)(123)))));
            this.btnSMS.ForeColorChecked = System.Drawing.Color.White;
            this.btnSMS.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSMS.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.btnSMS.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(131)))), ((int)(((byte)(131)))));
            this.btnSMS.ForeColorsByTypeUse = true;
            this.btnSMS.ID = -1;
            this.btnSMS.InitButtonWidth = 159;
            this.btnSMS.IsChecked = false;
            this.btnSMS.Location = new System.Drawing.Point(820, 100);
            this.btnSMS.MouseOverBkgndImage = null;
            this.btnSMS.MouseOverImage = global::SDMS_Building.Properties.Resources.mgr_hover;
            this.btnSMS.Name = "btnSMS";
            this.btnSMS.NormalImage = global::SDMS_Building.Properties.Resources.mgr_unchecked;
            this.btnSMS.Owner = null;
            this.btnSMS.Size = new System.Drawing.Size(159, 50);
            this.btnSMS.TabIndex = 28;
            this.btnSMS.Text = "문자 관리";
            this.btnSMS.TextLocation = new System.Drawing.Point(0, 13);
            this.btnSMS.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSMS.ToolTipText = "문자 관리";
            this.btnSMS.UseCustomImageRect = true;
            this.btnSMS.UseTextLocation = true;
            this.btnSMS.UseVisualStyleBackColor = true;
            this.btnSMS.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // FormManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(1000, 759);
            this.Controls.Add(this.btnSMS);
            this.Controls.Add(this.btnBroadcast);
            this.Controls.Add(this.btnEarthquakeTab);
            this.Controls.Add(this.btnDetectPolicyTab);
            this.Controls.Add(this.btnSensorListTab);
            this.Controls.Add(this.btnManagerTab);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormManagement";
            this.ShowInTaskbar = false;
            this.Text = "FormManagerment";
            this.Load += new System.EventHandler(this.FormManagement_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private UnE.GUI.ImageButton btnClose;
        private UnE.GUI.ImageButton btnCancel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private UnE.GUI.ImageButton btnConfirm;
        private UnE.GUI.RibbonButton btnManagerTab;
        private UnE.GUI.RibbonButton btnSensorListTab;
        private UnE.GUI.RibbonButton btnDetectPolicyTab;
        private UnE.GUI.RibbonButton btnEarthquakeTab;
        private UnE.GUI.RibbonButton btnBroadcast;
        private UnE.GUI.RibbonButton btnSMS;
    }
}