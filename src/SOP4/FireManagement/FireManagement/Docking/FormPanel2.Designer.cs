namespace FireManagement.Docking
{
    partial class FormPanel2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPanel2));
            this.panelRightBar = new System.Windows.Forms.Panel();
            this.timerLongTab = new System.Windows.Forms.Timer(this.components);
            this.panelLeft = new System.Windows.Forms.Panel();
            this.btnFireAlarm = new FireManagement.RibbonButtonFireManagement();
            this.btnFireReciver = new FireManagement.RibbonButtonFireManagement();
            this.btnFirePlug = new FireManagement.RibbonButtonFireManagement();
            this.btnHome = new FireManagement.RibbonButtonFireManagement();
            this.btnGroup = new FireManagement.RibbonButtonFireManagement();
            this.btnFireExtinguisher = new FireManagement.RibbonButtonFireManagement();
            this.contextMenuStripEditEquipZoneText = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemSaveDB = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCreateText = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEquipDel = new UnE.GUI.RibbonButton();
            this.btnEquipAdd = new UnE.GUI.RibbonButton();
            this.btnShowEquipmentList = new UnE.GUI.RibbonButton();
            this.btnFireReceiver = new FireManagement.RibbonButtonFireManagement();
            this.panelLeft.SuspendLayout();
            this.contextMenuStripEditEquipZoneText.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelRightBar
            // 
            this.panelRightBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelRightBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelRightBar.Location = new System.Drawing.Point(643, 1);
            this.panelRightBar.Name = "panelRightBar";
            this.panelRightBar.Size = new System.Drawing.Size(426, 656);
            this.panelRightBar.TabIndex = 3;
            // 
            // timerLongTab
            // 
            this.timerLongTab.Tick += new System.EventHandler(this.timerLongTab_Tick);
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.Transparent;
            this.panelLeft.BackgroundImage = global::FireManagement.Properties.Resources.Left_BG;
            this.panelLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLeft.Controls.Add(this.btnFireAlarm);
            this.panelLeft.Controls.Add(this.btnFireReciver);
            this.panelLeft.Controls.Add(this.btnFirePlug);
            this.panelLeft.Controls.Add(this.btnHome);
            this.panelLeft.Controls.Add(this.btnGroup);
            this.panelLeft.Controls.Add(this.btnFireExtinguisher);
            this.panelLeft.Location = new System.Drawing.Point(27, 65);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(83, 492);
            this.panelLeft.TabIndex = 1;
            // 
            // btnFireAlarm
            // 
            this.btnFireAlarm.CheckButton = false;
            this.btnFireAlarm.CheckedBkgndImage = null;
            this.btnFireAlarm.CheckedImage = null;
            this.btnFireAlarm.ClickedBackgroundImage = null;
            this.btnFireAlarm.ClickedImage = null;
            this.btnFireAlarm.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireAlarm.DisabledBkgndImage = null;
            this.btnFireAlarm.DisabledImage = null;
            this.btnFireAlarm.ID = -1;
            this.btnFireAlarm.InitButtonWidth = 83;
            this.btnFireAlarm.IsChecked = false;
            this.btnFireAlarm.Location = new System.Drawing.Point(0, 326);
            this.btnFireAlarm.MouseOverBkgndImage = null;
            this.btnFireAlarm.MouseOverImage = null;
            this.btnFireAlarm.Name = "btnFireAlarm";
            this.btnFireAlarm.NormalImage = null;
            this.btnFireAlarm.Owner = null;
            this.btnFireAlarm.Size = new System.Drawing.Size(83, 82);
            this.btnFireAlarm.TabIndex = 3;
            this.btnFireAlarm.Text = "발신기";
            this.btnFireAlarm.TextLocation = new System.Drawing.Point(20, 60);
            this.btnFireAlarm.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireAlarm.ToolTipText = "발신기";
            this.btnFireAlarm.UseCustomImageRect = false;
            this.btnFireAlarm.UseTextLocation = true;
            this.btnFireAlarm.UseVisualStyleBackColor = true;
            // 
            // btnFireReciver
            // 
            this.btnFireReciver.CheckButton = false;
            this.btnFireReciver.CheckedBkgndImage = null;
            this.btnFireReciver.CheckedImage = null;
            this.btnFireReciver.ClickedBackgroundImage = null;
            this.btnFireReciver.ClickedImage = null;
            this.btnFireReciver.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireReciver.DisabledBkgndImage = null;
            this.btnFireReciver.DisabledImage = null;
            this.btnFireReciver.ID = -1;
            this.btnFireReciver.InitButtonWidth = 83;
            this.btnFireReciver.IsChecked = false;
            this.btnFireReciver.Location = new System.Drawing.Point(0, 408);
            this.btnFireReciver.MouseOverBkgndImage = null;
            this.btnFireReciver.MouseOverImage = null;
            this.btnFireReciver.Name = "btnFireReciver";
            this.btnFireReciver.NormalImage = null;
            this.btnFireReciver.Owner = null;
            this.btnFireReciver.Size = new System.Drawing.Size(83, 82);
            this.btnFireReciver.TabIndex = 3;
            this.btnFireReciver.Text = "수신반";
            this.btnFireReciver.TextLocation = new System.Drawing.Point(20, 60);
            this.btnFireReciver.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireReciver.ToolTipText = "수신반";
            this.btnFireReciver.UseCustomImageRect = false;
            this.btnFireReciver.UseTextLocation = true;
            this.btnFireReciver.UseVisualStyleBackColor = true;
            // 
            // btnFirePlug
            // 
            this.btnFirePlug.CheckButton = false;
            this.btnFirePlug.CheckedBkgndImage = null;
            this.btnFirePlug.CheckedImage = null;
            this.btnFirePlug.ClickedBackgroundImage = null;
            this.btnFirePlug.ClickedImage = null;
            this.btnFirePlug.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFirePlug.DisabledBkgndImage = null;
            this.btnFirePlug.DisabledImage = null;
            this.btnFirePlug.ForeColor = System.Drawing.Color.White;
            this.btnFirePlug.ID = -1;
            this.btnFirePlug.InitButtonWidth = 83;
            this.btnFirePlug.IsChecked = false;
            this.btnFirePlug.Location = new System.Drawing.Point(0, 244);
            this.btnFirePlug.MouseOverBkgndImage = null;
            this.btnFirePlug.MouseOverImage = null;
            this.btnFirePlug.Name = "btnFirePlug";
            this.btnFirePlug.NormalImage = null;
            this.btnFirePlug.Owner = null;
            this.btnFirePlug.Size = new System.Drawing.Size(83, 82);
            this.btnFirePlug.TabIndex = 2;
            this.btnFirePlug.Text = "소화전";
            this.btnFirePlug.TextLocation = new System.Drawing.Point(20, 60);
            this.btnFirePlug.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFirePlug.ToolTipText = "소화전";
            this.btnFirePlug.UseCustomImageRect = false;
            this.btnFirePlug.UseTextLocation = true;
            this.btnFirePlug.UseVisualStyleBackColor = true;
            // 
            // btnHome
            // 
            this.btnHome.CheckButton = false;
            this.btnHome.CheckedBkgndImage = null;
            this.btnHome.CheckedImage = null;
            this.btnHome.ClickedBackgroundImage = null;
            this.btnHome.ClickedImage = null;
            this.btnHome.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnHome.DisabledBkgndImage = null;
            this.btnHome.DisabledImage = global::FireManagement.Properties.Resources.home_disabled;
            this.btnHome.ID = -1;
            this.btnHome.InitButtonWidth = 83;
            this.btnHome.IsChecked = false;
            this.btnHome.Location = new System.Drawing.Point(0, -3);
            this.btnHome.MouseOverBkgndImage = global::FireManagement.Properties.Resources.mouse_over_home;
            this.btnHome.MouseOverImage = null;
            this.btnHome.Name = "btnHome";
            this.btnHome.NormalImage = global::FireManagement.Properties.Resources.home_86_82;
            this.btnHome.Owner = null;
            this.btnHome.Size = new System.Drawing.Size(83, 82);
            this.btnHome.TabIndex = 0;
            this.btnHome.Text = "홈";
            this.btnHome.TextLocation = new System.Drawing.Point(32, 60);
            this.btnHome.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnHome.ToolTipText = "홈";
            this.btnHome.UseCustomImageRect = false;
            this.btnHome.UseTextLocation = true;
            this.btnHome.UseVisualStyleBackColor = true;
            // 
            // btnGroup
            // 
            this.btnGroup.CheckButton = false;
            this.btnGroup.CheckedBkgndImage = global::FireManagement.Properties.Resources.click_FireManagement;
            this.btnGroup.CheckedImage = global::FireManagement.Properties.Resources.click_groupimg;
            this.btnGroup.ClickedBackgroundImage = null;
            this.btnGroup.ClickedImage = null;
            this.btnGroup.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnGroup.DisabledBkgndImage = null;
            this.btnGroup.DisabledImage = null;
            this.btnGroup.ID = -1;
            this.btnGroup.InitButtonWidth = 83;
            this.btnGroup.IsChecked = false;
            this.btnGroup.Location = new System.Drawing.Point(0, 81);
            this.btnGroup.MouseOverBkgndImage = global::FireManagement.Properties.Resources.group_Nomal;
            this.btnGroup.MouseOverImage = null;
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.NormalImage = global::FireManagement.Properties.Resources.group_Nomal;
            this.btnGroup.Owner = null;
            this.btnGroup.Size = new System.Drawing.Size(83, 82);
            this.btnGroup.TabIndex = 1;
            this.btnGroup.Text = "그룹";
            this.btnGroup.TextLocation = new System.Drawing.Point(28, 60);
            this.btnGroup.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnGroup.ToolTipText = "그룹";
            this.btnGroup.UseCustomImageRect = false;
            this.btnGroup.UseTextLocation = true;
            this.btnGroup.UseVisualStyleBackColor = true;
            // 
            // btnFireExtinguisher
            // 
            this.btnFireExtinguisher.CheckButton = false;
            this.btnFireExtinguisher.CheckedBkgndImage = null;
            this.btnFireExtinguisher.CheckedImage = null;
            this.btnFireExtinguisher.ClickedBackgroundImage = null;
            this.btnFireExtinguisher.ClickedImage = null;
            this.btnFireExtinguisher.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireExtinguisher.DisabledBkgndImage = null;
            this.btnFireExtinguisher.DisabledImage = null;
            this.btnFireExtinguisher.ID = -1;
            this.btnFireExtinguisher.InitButtonWidth = 83;
            this.btnFireExtinguisher.IsChecked = false;
            this.btnFireExtinguisher.Location = new System.Drawing.Point(0, 162);
            this.btnFireExtinguisher.MouseOverBkgndImage = null;
            this.btnFireExtinguisher.MouseOverImage = null;
            this.btnFireExtinguisher.Name = "btnFireExtinguisher";
            this.btnFireExtinguisher.NormalImage = null;
            this.btnFireExtinguisher.Owner = null;
            this.btnFireExtinguisher.Size = new System.Drawing.Size(83, 82);
            this.btnFireExtinguisher.TabIndex = 1;
            this.btnFireExtinguisher.Text = "소화기";
            this.btnFireExtinguisher.TextLocation = new System.Drawing.Point(20, 60);
            this.btnFireExtinguisher.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireExtinguisher.ToolTipText = "소화기";
            this.btnFireExtinguisher.UseCustomImageRect = false;
            this.btnFireExtinguisher.UseTextLocation = true;
            this.btnFireExtinguisher.UseVisualStyleBackColor = true;
            // 
            // contextMenuStripEditEquipZoneText
            // 
            this.contextMenuStripEditEquipZoneText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSaveDB,
            this.toolStripMenuItemCreateText});
            this.contextMenuStripEditEquipZoneText.Name = "contextMenuStripEditEquipZoneText";
            this.contextMenuStripEditEquipZoneText.Size = new System.Drawing.Size(125, 48);
            // 
            // toolStripMenuItemSaveDB
            // 
            this.toolStripMenuItemSaveDB.Name = "toolStripMenuItemSaveDB";
            this.toolStripMenuItemSaveDB.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItemSaveDB.Text = "DB 저장";
            this.toolStripMenuItemSaveDB.ToolTipText = "DB 저장";
            this.toolStripMenuItemSaveDB.Click += new System.EventHandler(this.toolStripMenuItemSaveDB_Click);
            // 
            // toolStripMenuItemCreateText
            // 
            this.toolStripMenuItemCreateText.Name = "toolStripMenuItemCreateText";
            this.toolStripMenuItemCreateText.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItemCreateText.Text = "Text 생성";
            this.toolStripMenuItemCreateText.ToolTipText = "현재 위치에 Text를 생성한다.";
            this.toolStripMenuItemCreateText.Click += new System.EventHandler(this.toolStripMenuItemCreateText_Click);
            // 
            // btnEquipDel
            // 
            this.btnEquipDel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEquipDel.CheckButton = false;
            this.btnEquipDel.CheckedBkgndImage = null;
            this.btnEquipDel.CheckedImage = global::FireManagement.Properties.Resources.Delete_Icon_Click;
            this.btnEquipDel.ClickedBackgroundImage = null;
            this.btnEquipDel.ClickedImage = null;
            this.btnEquipDel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnEquipDel.DisabledBkgndImage = null;
            this.btnEquipDel.DisabledImage = null;
            this.btnEquipDel.ID = -1;
            this.btnEquipDel.InitButtonWidth = 80;
            this.btnEquipDel.IsChecked = false;
            this.btnEquipDel.Location = new System.Drawing.Point(555, 567);
            this.btnEquipDel.MouseOverBkgndImage = global::FireManagement.Properties.Resources.Delete_Icon_Click;
            this.btnEquipDel.MouseOverImage = null;
            this.btnEquipDel.Name = "btnEquipDel";
            this.btnEquipDel.NormalImage = global::FireManagement.Properties.Resources.Delete_Icon;
            this.btnEquipDel.Owner = null;
            this.btnEquipDel.Size = new System.Drawing.Size(80, 80);
            this.btnEquipDel.TabIndex = 1;
            this.btnEquipDel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnEquipDel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnEquipDel.ToolTipText = "";
            this.btnEquipDel.UseCustomImageRect = false;
            this.btnEquipDel.UseTextLocation = false;
            this.btnEquipDel.UseVisualStyleBackColor = true;
            this.btnEquipDel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnEquipDel_MouseUp);
            // 
            // btnEquipAdd
            // 
            this.btnEquipAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEquipAdd.CheckButton = false;
            this.btnEquipAdd.CheckedBkgndImage = null;
            this.btnEquipAdd.CheckedImage = global::FireManagement.Properties.Resources.EquipNew_Icon_Click;
            this.btnEquipAdd.ClickedBackgroundImage = null;
            this.btnEquipAdd.ClickedImage = null;
            this.btnEquipAdd.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnEquipAdd.DisabledBkgndImage = null;
            this.btnEquipAdd.DisabledImage = null;
            this.btnEquipAdd.ID = -1;
            this.btnEquipAdd.InitButtonWidth = 80;
            this.btnEquipAdd.IsChecked = false;
            this.btnEquipAdd.Location = new System.Drawing.Point(450, 567);
            this.btnEquipAdd.MouseOverBkgndImage = global::FireManagement.Properties.Resources.EquipNew_Icon_Click;
            this.btnEquipAdd.MouseOverImage = null;
            this.btnEquipAdd.Name = "btnEquipAdd";
            this.btnEquipAdd.NormalImage = global::FireManagement.Properties.Resources.EquipNew_Icon;
            this.btnEquipAdd.Owner = null;
            this.btnEquipAdd.Size = new System.Drawing.Size(80, 80);
            this.btnEquipAdd.TabIndex = 0;
            this.btnEquipAdd.TextLocation = new System.Drawing.Point(0, 0);
            this.btnEquipAdd.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnEquipAdd.ToolTipText = "";
            this.btnEquipAdd.UseCustomImageRect = false;
            this.btnEquipAdd.UseTextLocation = false;
            this.btnEquipAdd.UseVisualStyleBackColor = true;
            this.btnEquipAdd.Click += new System.EventHandler(this.btnEquipAdd_Click);
            // 
            // btnShowEquipmentList
            // 
            this.btnShowEquipmentList.BackgroundImage = global::FireManagement.Properties.Resources.Btn_close_Panel_;
            this.btnShowEquipmentList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnShowEquipmentList.CheckButton = false;
            this.btnShowEquipmentList.CheckedBkgndImage = null;
            this.btnShowEquipmentList.CheckedImage = null;
            this.btnShowEquipmentList.ClickedBackgroundImage = null;
            this.btnShowEquipmentList.ClickedImage = null;
            this.btnShowEquipmentList.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnShowEquipmentList.DisabledBkgndImage = null;
            this.btnShowEquipmentList.DisabledImage = null;
            this.btnShowEquipmentList.ID = -1;
            this.btnShowEquipmentList.InitButtonWidth = 60;
            this.btnShowEquipmentList.IsChecked = false;
            this.btnShowEquipmentList.Location = new System.Drawing.Point(596, 262);
            this.btnShowEquipmentList.MouseOverBkgndImage = null;
            this.btnShowEquipmentList.MouseOverImage = null;
            this.btnShowEquipmentList.Name = "btnShowEquipmentList";
            this.btnShowEquipmentList.NormalImage = null;
            this.btnShowEquipmentList.Owner = null;
            this.btnShowEquipmentList.Size = new System.Drawing.Size(60, 92);
            this.btnShowEquipmentList.TabIndex = 2;
            this.btnShowEquipmentList.TextLocation = new System.Drawing.Point(0, 0);
            this.btnShowEquipmentList.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnShowEquipmentList.ToolTipText = "";
            this.btnShowEquipmentList.UseCustomImageRect = false;
            this.btnShowEquipmentList.UseTextLocation = false;
            this.btnShowEquipmentList.UseVisualStyleBackColor = true;
            this.btnShowEquipmentList.Click += new System.EventHandler(this.btnShowEquipmentList_Click);
            // 
            // btnFireReceiver
            // 
            this.btnFireReceiver.CheckButton = false;
            this.btnFireReceiver.CheckedBkgndImage = null;
            this.btnFireReceiver.CheckedImage = null;
            this.btnFireReceiver.ClickedBackgroundImage = null;
            this.btnFireReceiver.ClickedImage = null;
            this.btnFireReceiver.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireReceiver.DisabledBkgndImage = null;
            this.btnFireReceiver.DisabledImage = null;
            this.btnFireReceiver.ID = -1;
            this.btnFireReceiver.InitButtonWidth = 83;
            this.btnFireReceiver.IsChecked = false;
            this.btnFireReceiver.Location = new System.Drawing.Point(0, 408);
            this.btnFireReceiver.MouseOverBkgndImage = null;
            this.btnFireReceiver.MouseOverImage = null;
            this.btnFireReceiver.Name = "btnFireReceiver";
            this.btnFireReceiver.NormalImage = null;
            this.btnFireReceiver.Owner = null;
            this.btnFireReceiver.Size = new System.Drawing.Size(83, 82);
            this.btnFireReceiver.TabIndex = 3;
            this.btnFireReceiver.Text = "수신반";
            this.btnFireReceiver.TextLocation = new System.Drawing.Point(20, 60);
            this.btnFireReceiver.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireReceiver.ToolTipText = "수신반";
            this.btnFireReceiver.UseCustomImageRect = false;
            this.btnFireReceiver.UseTextLocation = true;
            this.btnFireReceiver.UseVisualStyleBackColor = true;
            // 
            // FormPanel2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1068, 653);
            this.Controls.Add(this.btnEquipDel);
            this.Controls.Add(this.btnEquipAdd);
            this.Controls.Add(this.btnShowEquipmentList);
            this.Controls.Add(this.panelRightBar);
            this.Controls.Add(this.panelLeft);
            //this.Controls.Add(this.dxfControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormPanel2";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormPanel2";
            this.Load += new System.EventHandler(this.FormPanel2_Load);
            this.panelLeft.ResumeLayout(false);
            this.contextMenuStripEditEquipZoneText.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelRightBar;
        private UnE.GUI.RibbonButton btnShowEquipmentList;
        private UnE.GUI.RibbonButton btnEquipDel;
        private UnE.GUI.RibbonButton btnEquipAdd;
        private System.Windows.Forms.Timer timerLongTab;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEditEquipZoneText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveDB;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCreateText;
        private UnE.GUI.RibbonButton btnFirePlug;
        private UnE.GUI.RibbonButton btnFireExtinguisher;
        private UnE.GUI.RibbonButton btnHome;
        private UnE.GUI.RibbonButton btnFireAlarm;
        private UnE.GUI.RibbonButton btnFireReciver;
        private UnE.GUI.RibbonButton btnGroup;
        private UnE.GUI.RibbonButton btnFireReceiver;
    }
}