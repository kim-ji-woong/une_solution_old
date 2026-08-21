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
            this.dxfControl1 = new DXFViewer.DXFControl();
            this.panelRightBar = new System.Windows.Forms.Panel();
            this.timerLongTab = new System.Windows.Forms.Timer(this.components);
            this.panelLeft = new System.Windows.Forms.Panel();
            this.contextMenuStripEditEquipZoneText = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemSaveDB = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCreateText = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEquipDel = new UnE.GUI.RibbonButton();
            this.btnEquipAdd = new UnE.GUI.RibbonButton();
            this.btnShowEquipmentList = new UnE.GUI.RibbonButton();
            this.btnFireAlarm = new FireManagement.RibbonButtonFireManagement();
            this.btnFirePlug = new FireManagement.RibbonButtonFireManagement();
            this.btnHome = new FireManagement.RibbonButtonFireManagement();
            this.btnGroup = new FireManagement.RibbonButtonFireManagement();
            this.btnFireExtinguisher = new FireManagement.RibbonButtonFireManagement();
            this.panelLeft.SuspendLayout();
            this.contextMenuStripEditEquipZoneText.SuspendLayout();
            this.SuspendLayout();
            // 
            // dxfControl1
            // 
            this.dxfControl1.BackColor = System.Drawing.Color.Black;
            this.dxfControl1.GroupItemDistance = 30;
            this.dxfControl1.GroupItemMinCount = 3;
            this.dxfControl1.Location = new System.Drawing.Point(131, 98);
            this.dxfControl1.Name = "dxfControl1";
            this.dxfControl1.Panning = false;
            this.dxfControl1.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.dxfControl1.Size = new System.Drawing.Size(150, 150);
            this.dxfControl1.TabIndex = 1;
            this.dxfControl1.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.dxfControl1.UseGroupItem = false;
            this.dxfControl1.UseMouseWheel = true;
            this.dxfControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dxfControl1_KeyDown);
            this.dxfControl1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseDoubleClick);
            this.dxfControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseDown);
            this.dxfControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseMove);
            this.dxfControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseUp);
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
            this.panelLeft.Controls.Add(this.btnFirePlug);
            this.panelLeft.Controls.Add(this.btnHome);
            this.panelLeft.Controls.Add(this.btnGroup);
            this.panelLeft.Controls.Add(this.btnFireExtinguisher);
            this.panelLeft.Location = new System.Drawing.Point(27, 65);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(83, 410);
            this.panelLeft.TabIndex = 1;
            // 
            // contextMenuStripEditEquipZoneText
            // 
            this.contextMenuStripEditEquipZoneText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSaveDB,
            this.toolStripMenuItemCreateText});
            this.contextMenuStripEditEquipZoneText.Name = "contextMenuStripEditEquipZoneText";
            this.contextMenuStripEditEquipZoneText.Size = new System.Drawing.Size(119, 26);
            // 
            // toolStripMenuItemSaveDB
            // 
            this.toolStripMenuItemSaveDB.Name = "toolStripMenuItemSaveDB";
            this.toolStripMenuItemSaveDB.Size = new System.Drawing.Size(118, 22);
            this.toolStripMenuItemSaveDB.Text = "DB 저장";
            this.toolStripMenuItemSaveDB.ToolTipText = "DB 저장";
            this.toolStripMenuItemSaveDB.Click += new System.EventHandler(this.toolStripMenuItemSaveDB_Click);
            //
            // toolStripMenuItemCreateText
            //
            this.toolStripMenuItemCreateText.Name = "toolStripMenuItemCreateText";
            this.toolStripMenuItemCreateText.Size = new System.Drawing.Size(118, 22);
            this.toolStripMenuItemCreateText.Text = "Text 생성";
            this.toolStripMenuItemCreateText.ToolTipText = "현재 위치에 Text를 생성한다.";
            this.toolStripMenuItemCreateText.Click += new System.EventHandler(this.toolStripMenuItemCreateText_Click);
            // 
            // btnEquipDel
            // 
            this.btnEquipDel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEquipDel.CheckedBkgndImage = null;
            this.btnEquipDel.CheckedImage = global::FireManagement.Properties.Resources.Delete_Icon_Click;
            this.btnEquipDel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnEquipDel.DisabledBkgndImage = null;
            this.btnEquipDel.DisabledImage = null;
            this.btnEquipDel.ID = -1;
            this.btnEquipDel.InitButtonWidth = 80;
            this.btnEquipDel.IsChecked = false;
            this.btnEquipDel.Location = new System.Drawing.Point(555, 567);
            this.btnEquipDel.MouseOverBkgndImage = global::FireManagement.Properties.Resources.Delete_Icon_Click;
            this.btnEquipDel.Name = "btnEquipDel";
            this.btnEquipDel.NormalImage = global::FireManagement.Properties.Resources.Delete_Icon;
            this.btnEquipDel.Owner = null;
            this.btnEquipDel.Size = new System.Drawing.Size(80, 80);
            this.btnEquipDel.TabIndex = 1;
            this.btnEquipDel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnEquipDel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnEquipDel.UseCustomImageRect = false;
            this.btnEquipDel.UseTextLocation = false;
            this.btnEquipDel.UseVisualStyleBackColor = true;
            this.btnEquipDel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnEquipDel_MouseUp);
            // 
            // btnEquipAdd
            // 
            this.btnEquipAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEquipAdd.CheckedBkgndImage = null;
            this.btnEquipAdd.CheckedImage = global::FireManagement.Properties.Resources.EquipNew_Icon_Click;
            this.btnEquipAdd.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnEquipAdd.DisabledBkgndImage = null;
            this.btnEquipAdd.DisabledImage = null;
            this.btnEquipAdd.ID = -1;
            this.btnEquipAdd.InitButtonWidth = 80;
            this.btnEquipAdd.IsChecked = false;
            this.btnEquipAdd.Location = new System.Drawing.Point(450, 567);
            this.btnEquipAdd.MouseOverBkgndImage = global::FireManagement.Properties.Resources.EquipNew_Icon_Click;
            this.btnEquipAdd.Name = "btnEquipAdd";
            this.btnEquipAdd.NormalImage = global::FireManagement.Properties.Resources.EquipNew_Icon;
            this.btnEquipAdd.Owner = null;
            this.btnEquipAdd.Size = new System.Drawing.Size(80, 80);
            this.btnEquipAdd.TabIndex = 0;
            this.btnEquipAdd.TextLocation = new System.Drawing.Point(0, 0);
            this.btnEquipAdd.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnEquipAdd.UseCustomImageRect = false;
            this.btnEquipAdd.UseTextLocation = false;
            this.btnEquipAdd.UseVisualStyleBackColor = true;
            this.btnEquipAdd.Click += new System.EventHandler(this.btnEquipAdd_Click);
            // 
            // btnShowEquipmentList
            // 
            this.btnShowEquipmentList.BackgroundImage = global::FireManagement.Properties.Resources.Btn_close_Panel_;
            this.btnShowEquipmentList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnShowEquipmentList.CheckedBkgndImage = null;
            this.btnShowEquipmentList.CheckedImage = null;
            this.btnShowEquipmentList.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnShowEquipmentList.DisabledBkgndImage = null;
            this.btnShowEquipmentList.DisabledImage = null;
            this.btnShowEquipmentList.ID = -1;
            this.btnShowEquipmentList.InitButtonWidth = 60;
            this.btnShowEquipmentList.IsChecked = false;
            this.btnShowEquipmentList.Location = new System.Drawing.Point(596, 262);
            this.btnShowEquipmentList.MouseOverBkgndImage = null;
            this.btnShowEquipmentList.Name = "btnShowEquipmentList";
            this.btnShowEquipmentList.NormalImage = null;
            this.btnShowEquipmentList.Owner = null;
            this.btnShowEquipmentList.Size = new System.Drawing.Size(60, 92);
            this.btnShowEquipmentList.TabIndex = 2;
            this.btnShowEquipmentList.TextLocation = new System.Drawing.Point(0, 0);
            this.btnShowEquipmentList.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnShowEquipmentList.UseCustomImageRect = false;
            this.btnShowEquipmentList.UseTextLocation = false;
            this.btnShowEquipmentList.UseVisualStyleBackColor = true;
            this.btnShowEquipmentList.Click += new System.EventHandler(this.btnShowEquipmentList_Click);
            // 
            // btnFireAlarm
            // 
            this.btnFireAlarm.CheckedBkgndImage = null;
            this.btnFireAlarm.CheckedImage = null;
            this.btnFireAlarm.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireAlarm.DisabledBkgndImage = null;
            this.btnFireAlarm.DisabledImage = null;
            this.btnFireAlarm.ID = -1;
            this.btnFireAlarm.InitButtonWidth = 83;
            this.btnFireAlarm.IsChecked = false;
            this.btnFireAlarm.Location = new System.Drawing.Point(0, 327);
            this.btnFireAlarm.MouseOverBkgndImage = null;
            this.btnFireAlarm.Name = "btnFireAlarm";
            this.btnFireAlarm.NormalImage = null;
            this.btnFireAlarm.Owner = null;
            this.btnFireAlarm.Size = new System.Drawing.Size(83, 82);
            this.btnFireAlarm.TabIndex = 3;
            this.btnFireAlarm.Text = "발신기";
            this.btnFireAlarm.TextLocation = new System.Drawing.Point(20, 60);
            this.btnFireAlarm.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireAlarm.UseCustomImageRect = false;
            this.btnFireAlarm.UseTextLocation = true;
            this.btnFireAlarm.UseVisualStyleBackColor = true;
            // 
            // btnFirePlug
            // 
            this.btnFirePlug.CheckedBkgndImage = null;
            this.btnFirePlug.CheckedImage = null;
            this.btnFirePlug.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFirePlug.DisabledBkgndImage = null;
            this.btnFirePlug.DisabledImage = null;
            this.btnFirePlug.ForeColor = System.Drawing.Color.White;
            this.btnFirePlug.ID = -1;
            this.btnFirePlug.InitButtonWidth = 83;
            this.btnFirePlug.IsChecked = false;
            this.btnFirePlug.Location = new System.Drawing.Point(0, 244);
            this.btnFirePlug.MouseOverBkgndImage = null;
            this.btnFirePlug.Name = "btnFirePlug";
            this.btnFirePlug.NormalImage = null;
            this.btnFirePlug.Owner = null;
            this.btnFirePlug.Size = new System.Drawing.Size(83, 82);
            this.btnFirePlug.TabIndex = 2;
            this.btnFirePlug.Text = "소화전";
            this.btnFirePlug.TextLocation = new System.Drawing.Point(20, 60);
            this.btnFirePlug.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFirePlug.UseCustomImageRect = false;
            this.btnFirePlug.UseTextLocation = true;
            this.btnFirePlug.UseVisualStyleBackColor = true;
            // 
            // btnHome
            // 
            this.btnHome.CheckedBkgndImage = null;
            this.btnHome.CheckedImage = null;
            this.btnHome.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnHome.DisabledBkgndImage = null;
            this.btnHome.DisabledImage = global::FireManagement.Properties.Resources.home_disabled;
            this.btnHome.ID = -1;
            this.btnHome.InitButtonWidth = 83;
            this.btnHome.IsChecked = false;
            this.btnHome.Location = new System.Drawing.Point(0, -3);
            this.btnHome.MouseOverBkgndImage = global::FireManagement.Properties.Resources.mouse_over_home;
            this.btnHome.Name = "btnHome";
            this.btnHome.NormalImage = global::FireManagement.Properties.Resources.home_86_82;
            this.btnHome.Owner = null;
            this.btnHome.Size = new System.Drawing.Size(83, 82);
            this.btnHome.TabIndex = 0;
            this.btnHome.Text = "홈";
            this.btnHome.TextLocation = new System.Drawing.Point(32, 60);
            this.btnHome.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnHome.UseCustomImageRect = false;
            this.btnHome.UseTextLocation = true;
            this.btnHome.UseVisualStyleBackColor = true;
            // 
            // btnGroup
            // 
            this.btnGroup.CheckedBkgndImage = global::FireManagement.Properties.Resources.click_FireManagement;
            this.btnGroup.CheckedImage = global::FireManagement.Properties.Resources.click_groupimg;
            this.btnGroup.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnGroup.DisabledBkgndImage = null;
            this.btnGroup.DisabledImage = null;
            this.btnGroup.ID = -1;
            this.btnGroup.InitButtonWidth = 83;
            this.btnGroup.IsChecked = false;
            this.btnGroup.Location = new System.Drawing.Point(0, 81);
            this.btnGroup.MouseOverBkgndImage = global::FireManagement.Properties.Resources.group_Nomal;
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.NormalImage = global::FireManagement.Properties.Resources.group_Nomal;
            this.btnGroup.Owner = null;
            this.btnGroup.Size = new System.Drawing.Size(83, 82);
            this.btnGroup.TabIndex = 1;
            this.btnGroup.Text = "그룹";
            this.btnGroup.TextLocation = new System.Drawing.Point(28, 60);
            this.btnGroup.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnGroup.UseCustomImageRect = false;
            this.btnGroup.UseTextLocation = true;
            this.btnGroup.UseVisualStyleBackColor = true;
            // 
            // btnFireExtinguisher
            // 
            this.btnFireExtinguisher.CheckedBkgndImage = null;
            this.btnFireExtinguisher.CheckedImage = null;
            this.btnFireExtinguisher.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireExtinguisher.DisabledBkgndImage = null;
            this.btnFireExtinguisher.DisabledImage = null;
            this.btnFireExtinguisher.ID = -1;
            this.btnFireExtinguisher.InitButtonWidth = 83;
            this.btnFireExtinguisher.IsChecked = false;
            this.btnFireExtinguisher.Location = new System.Drawing.Point(0, 162);
            this.btnFireExtinguisher.MouseOverBkgndImage = null;
            this.btnFireExtinguisher.Name = "btnFireExtinguisher";
            this.btnFireExtinguisher.NormalImage = null;
            this.btnFireExtinguisher.Owner = null;
            this.btnFireExtinguisher.Size = new System.Drawing.Size(83, 82);
            this.btnFireExtinguisher.TabIndex = 1;
            this.btnFireExtinguisher.Text = "소화기";
            this.btnFireExtinguisher.TextLocation = new System.Drawing.Point(20, 60);
            this.btnFireExtinguisher.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireExtinguisher.UseCustomImageRect = false;
            this.btnFireExtinguisher.UseTextLocation = true;
            this.btnFireExtinguisher.UseVisualStyleBackColor = true;
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
            this.Controls.Add(this.dxfControl1);
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
        private RibbonButtonFireManagement btnFirePlug;
        private RibbonButtonFireManagement btnFireExtinguisher;
        private RibbonButtonFireManagement btnHome;
        private RibbonButtonFireManagement btnFireAlarm;
        private DXFViewer.DXFControl dxfControl1;
        private System.Windows.Forms.Panel panelRightBar;
        private UnE.GUI.RibbonButton btnShowEquipmentList;
        private UnE.GUI.RibbonButton btnEquipDel;
        private UnE.GUI.RibbonButton btnEquipAdd;
        private System.Windows.Forms.Timer timerLongTab;
        private RibbonButtonFireManagement btnGroup;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEditEquipZoneText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveDB;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCreateText;
    }
}