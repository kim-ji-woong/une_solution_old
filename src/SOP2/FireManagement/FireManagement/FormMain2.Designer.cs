namespace FireManagement
{
    partial class FormMain2
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblStatusText = new System.Windows.Forms.Label();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.btnMin = new System.Windows.Forms.Button();
            this.btnMax = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.RibbonButton();
            this.btnSave = new UnE.GUI.RibbonButton();
            this.btnUpdate = new UnE.GUI.RibbonButton();
            this.btnLoad = new UnE.GUI.RibbonButton();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelinTop = new System.Windows.Forms.Panel();
            this.pictureBoxCheckEquip = new UnE.GUI.TextPictureBox();
            this.pictureBoxHistory = new UnE.GUI.TextPictureBox();
            this.pictureBoxEditMode = new UnE.GUI.TextPictureBox();
            this.pictureBoxNormalMode = new UnE.GUI.TextPictureBox();
            this.lblZoneName = new System.Windows.Forms.Label();
            this.lblMenuName = new System.Windows.Forms.Label();
            this.pictureBoxMgr = new UnE.GUI.TextPictureBox();
            this.pictureBoxFile = new UnE.GUI.TextPictureBox();
            this.pictureBoxFire = new UnE.GUI.TextPictureBox();
            this.panelBottom.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelinTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckEquip)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEditMode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNormalMode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMgr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFire)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.Location = new System.Drawing.Point(323, 148);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(564, 416);
            this.panelMain.TabIndex = 0;
            // 
            // panelBottom
            // 
            this.panelBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBottom.BackgroundImage = global::FireManagement.Properties.Resources.black_BottomBar;
            this.panelBottom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelBottom.Controls.Add(this.lblStatusText);
            this.panelBottom.Location = new System.Drawing.Point(0, 733);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1273, 25);
            this.panelBottom.TabIndex = 2;
            // 
            // lblStatusText
            // 
            this.lblStatusText.AutoSize = true;
            this.lblStatusText.BackColor = System.Drawing.Color.Black;
            this.lblStatusText.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblStatusText.ForeColor = System.Drawing.Color.White;
            this.lblStatusText.Location = new System.Drawing.Point(4, 12);
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(38, 11);
            this.lblStatusText.TabIndex = 3;
            this.lblStatusText.Text = "label2";
            // 
            // panelTitle
            // 
            this.panelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTitle.BackColor = System.Drawing.Color.Transparent;
            this.panelTitle.BackgroundImage = global::FireManagement.Properties.Resources.FireManageMent_System;
            this.panelTitle.Controls.Add(this.btnMin);
            this.panelTitle.Controls.Add(this.btnMax);
            this.panelTitle.Controls.Add(this.btnExit);
            this.panelTitle.Controls.Add(this.labelTitle);
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(1269, 0);
            this.panelTitle.TabIndex = 2;
            // 
            // btnMin
            // 
            this.btnMin.BackColor = System.Drawing.SystemColors.Control;
            this.btnMin.BackgroundImage = global::FireManagement.Properties.Resources.HideWindow_Normal;
            this.btnMin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMin.Location = new System.Drawing.Point(1152, 0);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(41, 30);
            this.btnMin.TabIndex = 3;
            this.btnMin.UseVisualStyleBackColor = false;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // btnMax
            // 
            this.btnMax.BackColor = System.Drawing.SystemColors.Control;
            this.btnMax.BackgroundImage = global::FireManagement.Properties.Resources.MaxWindow_Normal;
            this.btnMax.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMax.Location = new System.Drawing.Point(1191, 0);
            this.btnMax.Name = "btnMax";
            this.btnMax.Size = new System.Drawing.Size(41, 30);
            this.btnMax.TabIndex = 4;
            this.btnMax.UseVisualStyleBackColor = false;
            this.btnMax.Click += new System.EventHandler(this.btnMax_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.Control;
            this.btnExit.BackgroundImage = global::FireManagement.Properties.Resources.CloseWindow_Normal;
            this.btnExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExit.Location = new System.Drawing.Point(1230, 0);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(41, 30);
            this.btnExit.TabIndex = 5;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.labelTitle.Location = new System.Drawing.Point(527, -2);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(247, 32);
            this.labelTitle.TabIndex = 3;
            this.labelTitle.Text = "소방설비 관리 시스템";
            // 
            // panelLeft
            // 
            this.panelLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelLeft.BackgroundImage = global::FireManagement.Properties.Resources.Left_BG;
            this.panelLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLeft.Controls.Add(this.btnClose);
            this.panelLeft.Controls.Add(this.btnSave);
            this.panelLeft.Controls.Add(this.btnUpdate);
            this.panelLeft.Controls.Add(this.btnLoad);
            this.panelLeft.Location = new System.Drawing.Point(-1, 86);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(230, 647);
            this.panelLeft.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.CheckedBkgndImage = global::FireManagement.Properties.Resources.LeftBar_Click_Area;
            this.btnClose.CheckedImage = null;
            this.btnClose.CustomImageRect = new System.Drawing.Rectangle(80, 25, 70, 70);
            this.btnClose.DisabledBkgndImage = null;
            this.btnClose.DisabledImage = null;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.ID = -1;
            this.btnClose.InitButtonWidth = 230;
            this.btnClose.IsChecked = false;
            this.btnClose.Location = new System.Drawing.Point(0, 474);
            this.btnClose.MouseOverBkgndImage = global::FireManagement.Properties.Resources.LeftBar_Click_Area;
            this.btnClose.Name = "btnClose";
            this.btnClose.NormalImage = global::FireManagement.Properties.Resources.Close_icon;
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(230, 150);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "닫기";
            this.btnClose.TextLocation = new System.Drawing.Point(100, 100);
            this.btnClose.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnClose.UseCustomImageRect = true;
            this.btnClose.UseTextLocation = true;
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.CheckedBkgndImage = global::FireManagement.Properties.Resources.LeftBar_Click_Area;
            this.btnSave.CheckedImage = null;
            this.btnSave.CustomImageRect = new System.Drawing.Rectangle(80, 25, 70, 70);
            this.btnSave.DisabledBkgndImage = null;
            this.btnSave.DisabledImage = null;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.ID = -1;
            this.btnSave.InitButtonWidth = 230;
            this.btnSave.IsChecked = false;
            this.btnSave.Location = new System.Drawing.Point(0, 316);
            this.btnSave.MouseOverBkgndImage = global::FireManagement.Properties.Resources.LeftBar_Click_Area;
            this.btnSave.Name = "btnSave";
            this.btnSave.NormalImage = global::FireManagement.Properties.Resources.Save_icon;
            this.btnSave.Owner = null;
            this.btnSave.Size = new System.Drawing.Size(230, 150);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "저장";
            this.btnSave.TextLocation = new System.Drawing.Point(100, 100);
            this.btnSave.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSave.UseCustomImageRect = true;
            this.btnSave.UseTextLocation = true;
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.Color.Transparent;
            this.btnUpdate.CheckedBkgndImage = global::FireManagement.Properties.Resources.LeftBar_Click_Area;
            this.btnUpdate.CheckedImage = null;
            this.btnUpdate.CustomImageRect = new System.Drawing.Rectangle(80, 25, 70, 70);
            this.btnUpdate.DisabledBkgndImage = null;
            this.btnUpdate.DisabledImage = null;
            this.btnUpdate.ForeColor = System.Drawing.Color.White;
            this.btnUpdate.ID = -1;
            this.btnUpdate.InitButtonWidth = 230;
            this.btnUpdate.IsChecked = false;
            this.btnUpdate.Location = new System.Drawing.Point(0, 158);
            this.btnUpdate.MouseOverBkgndImage = global::FireManagement.Properties.Resources.LeftBar_Click_Area;
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.NormalImage = global::FireManagement.Properties.Resources.Update_Icon;
            this.btnUpdate.Owner = null;
            this.btnUpdate.Size = new System.Drawing.Size(230, 150);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Text = "업데이트";
            this.btnUpdate.TextLocation = new System.Drawing.Point(90, 100);
            this.btnUpdate.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnUpdate.UseCustomImageRect = true;
            this.btnUpdate.UseTextLocation = true;
            this.btnUpdate.UseVisualStyleBackColor = false;
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.Transparent;
            this.btnLoad.CheckedBkgndImage = global::FireManagement.Properties.Resources.LeftBar_Click_Area;
            this.btnLoad.CheckedImage = null;
            this.btnLoad.CustomImageRect = new System.Drawing.Rectangle(80, 25, 70, 70);
            this.btnLoad.DisabledBkgndImage = null;
            this.btnLoad.DisabledImage = null;
            this.btnLoad.ForeColor = System.Drawing.Color.White;
            this.btnLoad.ID = -1;
            this.btnLoad.InitButtonWidth = 230;
            this.btnLoad.IsChecked = true;
            this.btnLoad.Location = new System.Drawing.Point(0, 0);
            this.btnLoad.MouseOverBkgndImage = global::FireManagement.Properties.Resources.LeftBar_Click_Area;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.NormalImage = global::FireManagement.Properties.Resources.Load_Icon;
            this.btnLoad.Owner = null;
            this.btnLoad.Size = new System.Drawing.Size(230, 150);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "불러오기";
            this.btnLoad.TextLocation = new System.Drawing.Point(90, 100);
            this.btnLoad.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnLoad.UseCustomImageRect = true;
            this.btnLoad.UseTextLocation = true;
            this.btnLoad.UseVisualStyleBackColor = false;
            // 
            // panelTop
            // 
            this.panelTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTop.BackgroundImage = global::FireManagement.Properties.Resources.Top_Titlebar;
            this.panelTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelTop.Controls.Add(this.panelinTop);
            this.panelTop.Controls.Add(this.pictureBoxMgr);
            this.panelTop.Controls.Add(this.pictureBoxFile);
            this.panelTop.Controls.Add(this.pictureBoxFire);
            this.panelTop.Location = new System.Drawing.Point(-1, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1273, 92);
            this.panelTop.TabIndex = 0;
            // 
            // panelinTop
            // 
            this.panelinTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelinTop.BackColor = System.Drawing.Color.Transparent;
            this.panelinTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelinTop.Controls.Add(this.pictureBoxCheckEquip);
            this.panelinTop.Controls.Add(this.pictureBoxHistory);
            this.panelinTop.Controls.Add(this.pictureBoxEditMode);
            this.panelinTop.Controls.Add(this.pictureBoxNormalMode);
            this.panelinTop.Controls.Add(this.lblZoneName);
            this.panelinTop.Controls.Add(this.lblMenuName);
            this.panelinTop.Location = new System.Drawing.Point(243, 0);
            this.panelinTop.Name = "panelinTop";
            this.panelinTop.Size = new System.Drawing.Size(1031, 85);
            this.panelinTop.TabIndex = 4;
            // 
            // pictureBoxCheckEquip
            // 
            this.pictureBoxCheckEquip.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCheckEquip.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
            this.pictureBoxCheckEquip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxCheckEquip.Location = new System.Drawing.Point(928, 3);
            this.pictureBoxCheckEquip.Name = "pictureBoxCheckEquip";
            this.pictureBoxCheckEquip.Size = new System.Drawing.Size(100, 85);
            this.pictureBoxCheckEquip.TabIndex = 7;
            this.pictureBoxCheckEquip.TabStop = false;
            this.pictureBoxCheckEquip.Text = "설비점검";
            this.pictureBoxCheckEquip.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxHistory
            // 
            this.pictureBoxHistory.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxHistory.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
            this.pictureBoxHistory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxHistory.Location = new System.Drawing.Point(829, 3);
            this.pictureBoxHistory.Name = "pictureBoxHistory";
            this.pictureBoxHistory.Size = new System.Drawing.Size(100, 85);
            this.pictureBoxHistory.TabIndex = 6;
            this.pictureBoxHistory.TabStop = false;
            this.pictureBoxHistory.Text = "이력관리";
            this.pictureBoxHistory.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxEditMode
            // 
            this.pictureBoxEditMode.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxEditMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
            this.pictureBoxEditMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxEditMode.Location = new System.Drawing.Point(730, 3);
            this.pictureBoxEditMode.Name = "pictureBoxEditMode";
            this.pictureBoxEditMode.Size = new System.Drawing.Size(100, 85);
            this.pictureBoxEditMode.TabIndex = 5;
            this.pictureBoxEditMode.TabStop = false;
            this.pictureBoxEditMode.Text = "편집모드";
            this.pictureBoxEditMode.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxNormalMode
            // 
            this.pictureBoxNormalMode.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxNormalMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
            this.pictureBoxNormalMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxNormalMode.Location = new System.Drawing.Point(631, 3);
            this.pictureBoxNormalMode.Name = "pictureBoxNormalMode";
            this.pictureBoxNormalMode.Size = new System.Drawing.Size(100, 85);
            this.pictureBoxNormalMode.TabIndex = 4;
            this.pictureBoxNormalMode.TabStop = false;
            this.pictureBoxNormalMode.Text = "일반모드";
            this.pictureBoxNormalMode.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // lblZoneName
            // 
            this.lblZoneName.AutoSize = true;
            this.lblZoneName.BackColor = System.Drawing.Color.Transparent;
            this.lblZoneName.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblZoneName.ForeColor = System.Drawing.Color.White;
            this.lblZoneName.Location = new System.Drawing.Point(17, 30);
            this.lblZoneName.Name = "lblZoneName";
            this.lblZoneName.Size = new System.Drawing.Size(0, 32);
            this.lblZoneName.TabIndex = 0;
            // 
            // lblMenuName
            // 
            this.lblMenuName.AutoSize = true;
            this.lblMenuName.BackColor = System.Drawing.Color.Transparent;
            this.lblMenuName.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMenuName.ForeColor = System.Drawing.Color.White;
            this.lblMenuName.Location = new System.Drawing.Point(13, 30);
            this.lblMenuName.Name = "lblMenuName";
            this.lblMenuName.Size = new System.Drawing.Size(192, 32);
            this.lblMenuName.TabIndex = 3;
            this.lblMenuName.Text = "파일 > 불러오기";
            // 
            // pictureBoxMgr
            // 
            this.pictureBoxMgr.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxMgr.BackgroundImage = global::FireManagement.Properties.Resources.LeftTop_3_AreaBG;
            this.pictureBoxMgr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxMgr.Location = new System.Drawing.Point(156, -1);
            this.pictureBoxMgr.Name = "pictureBoxMgr";
            this.pictureBoxMgr.Size = new System.Drawing.Size(73, 86);
            this.pictureBoxMgr.TabIndex = 0;
            this.pictureBoxMgr.TabStop = false;
            this.pictureBoxMgr.Text = "관리";
            this.pictureBoxMgr.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxFile
            // 
            this.pictureBoxFile.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxFile.BackgroundImage = global::FireManagement.Properties.Resources.LeftTop_3_AreaBG;
            this.pictureBoxFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxFile.InitialImage = global::FireManagement.Properties.Resources.LeftTop_3_AreaBG;
            this.pictureBoxFile.Location = new System.Drawing.Point(2, -1);
            this.pictureBoxFile.Name = "pictureBoxFile";
            this.pictureBoxFile.Size = new System.Drawing.Size(73, 86);
            this.pictureBoxFile.TabIndex = 0;
            this.pictureBoxFile.TabStop = false;
            this.pictureBoxFile.Text = "파일";
            this.pictureBoxFile.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxFire
            // 
            this.pictureBoxFire.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxFire.BackgroundImage = global::FireManagement.Properties.Resources.LeftTop_3_AreaBG;
            this.pictureBoxFire.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxFire.Location = new System.Drawing.Point(79, -1);
            this.pictureBoxFire.Name = "pictureBoxFire";
            this.pictureBoxFire.Size = new System.Drawing.Size(73, 86);
            this.pictureBoxFire.TabIndex = 0;
            this.pictureBoxFire.TabStop = false;
            this.pictureBoxFire.Text = "소방설비";
            this.pictureBoxFire.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // FormMain2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.ClientSize = new System.Drawing.Size(1272, 758);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTitle);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMain2";
            this.Text = "FormMain2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain2_FormClosing);
            this.Load += new System.EventHandler(this.FormMain2_Load);
            this.Resize += new System.EventHandler(this.FormMain2_Resize);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.panelTitle.ResumeLayout(false);
            this.panelTitle.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelinTop.ResumeLayout(false);
            this.panelinTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckEquip)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEditMode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNormalMode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMgr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFire)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelTop;
        private UnE.GUI.TextPictureBox pictureBoxFile;
        private System.Windows.Forms.Panel panelLeft;
        private UnE.GUI.TextPictureBox pictureBoxMgr;
        private UnE.GUI.TextPictureBox pictureBoxFire;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblMenuName;
        private System.Windows.Forms.Panel panelinTop;
        private UnE.GUI.RibbonButton btnUpdate;
        private UnE.GUI.RibbonButton btnLoad;
        private UnE.GUI.RibbonButton btnClose;
        private UnE.GUI.RibbonButton btnSave;
        private System.Windows.Forms.Label lblStatusText;
        private System.Windows.Forms.Label lblZoneName;
        private UnE.GUI.TextPictureBox pictureBoxCheckEquip;
        private UnE.GUI.TextPictureBox pictureBoxHistory;
        private UnE.GUI.TextPictureBox pictureBoxEditMode;
        private UnE.GUI.TextPictureBox pictureBoxNormalMode;
        private System.Windows.Forms.Button btnMin;
        private System.Windows.Forms.Button btnMax;
        private System.Windows.Forms.Button btnExit;


        //private RibbonButtonEx button5;
        //private RibbonButtonEx button4;
    }
}