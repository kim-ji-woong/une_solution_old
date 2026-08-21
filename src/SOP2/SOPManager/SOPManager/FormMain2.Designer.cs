namespace SOPManager
{
	partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelForm = new System.Windows.Forms.Panel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelSection = new System.Windows.Forms.Panel();
            this.panelSectionContent = new System.Windows.Forms.Panel();
            this.panelRibbon = new System.Windows.Forms.Panel();
            this.m_tmrCmdUpdate = new System.Windows.Forms.Timer(this.components);
            this.panelStatus = new System.Windows.Forms.Panel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnMin = new System.Windows.Forms.Button();
            this.btnMax = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lbTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelGap = new System.Windows.Forms.Panel();
            this.ribbonButton8 = new UnE.GUI.RibbonButton();
            this.ribbonButton7 = new UnE.GUI.RibbonButton();
            this.ribbonButton6 = new UnE.GUI.RibbonButton();
            this.ribbonButton5 = new UnE.GUI.RibbonButton();
            this.ribbonButton4 = new UnE.GUI.RibbonButton();
            this.ribbonButton3 = new UnE.GUI.RibbonButton();
            this.ribbonButton2 = new UnE.GUI.RibbonButton();
            this.ribbonButton1 = new UnE.GUI.RibbonButton();
            this.btnNewSOP = new UnE.GUI.RibbonButton();
            this.btnDeleteSOP = new UnE.GUI.RibbonButton();
            this.btnOpenXML = new UnE.GUI.RibbonButton();
            this.btnSaveXML = new UnE.GUI.RibbonButton();
            this.btnOpen = new UnE.GUI.RibbonButton();
            this.btnSave = new UnE.GUI.RibbonButton();
            this.pictureBoxFile = new UnE.GUI.TextPictureBox();
            this.pictureBoxSOP = new UnE.GUI.TextPictureBox();
            this.panelContent.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelSection.SuspendLayout();
            this.panelRibbon.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSOP)).BeginInit();
            this.SuspendLayout();
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panelContent.Controls.Add(this.panelForm);
            this.panelContent.Controls.Add(this.panelLeft);
            this.panelContent.Location = new System.Drawing.Point(51, 148);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(694, 603);
            this.panelContent.TabIndex = 12;
            // 
            // panelForm
            // 
            this.panelForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelForm.Location = new System.Drawing.Point(226, 0);
            this.panelForm.Name = "panelForm";
            this.panelForm.Size = new System.Drawing.Size(468, 603);
            this.panelForm.TabIndex = 13;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.panelLeft.Controls.Add(this.btnNewSOP);
            this.panelLeft.Controls.Add(this.btnDeleteSOP);
            this.panelLeft.Controls.Add(this.btnOpenXML);
            this.panelLeft.Controls.Add(this.btnSaveXML);
            this.panelLeft.Controls.Add(this.btnOpen);
            this.panelLeft.Controls.Add(this.btnSave);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(226, 603);
            this.panelLeft.TabIndex = 12;
            // 
            // panelSection
            // 
            this.panelSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panelSection.Controls.Add(this.panelSectionContent);
            this.panelSection.Controls.Add(this.panelRibbon);
            this.panelSection.Location = new System.Drawing.Point(614, 124);
            this.panelSection.Name = "panelSection";
            this.panelSection.Size = new System.Drawing.Size(712, 609);
            this.panelSection.TabIndex = 13;
            // 
            // panelSectionContent
            // 
            this.panelSectionContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panelSectionContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSectionContent.Location = new System.Drawing.Point(0, 94);
            this.panelSectionContent.Name = "panelSectionContent";
            this.panelSectionContent.Size = new System.Drawing.Size(712, 515);
            this.panelSectionContent.TabIndex = 1;
            // 
            // panelRibbon
            // 
            this.panelRibbon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.panelRibbon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelRibbon.Controls.Add(this.ribbonButton8);
            this.panelRibbon.Controls.Add(this.ribbonButton7);
            this.panelRibbon.Controls.Add(this.ribbonButton6);
            this.panelRibbon.Controls.Add(this.ribbonButton5);
            this.panelRibbon.Controls.Add(this.ribbonButton4);
            this.panelRibbon.Controls.Add(this.ribbonButton3);
            this.panelRibbon.Controls.Add(this.ribbonButton2);
            this.panelRibbon.Controls.Add(this.ribbonButton1);
            this.panelRibbon.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRibbon.Location = new System.Drawing.Point(0, 0);
            this.panelRibbon.Name = "panelRibbon";
            this.panelRibbon.Size = new System.Drawing.Size(712, 94);
            this.panelRibbon.TabIndex = 0;
            // 
            // m_tmrCmdUpdate
            // 
            this.m_tmrCmdUpdate.Interval = 300;
            this.m_tmrCmdUpdate.Tick += new System.EventHandler(this.m_tmrCmdUpdate_Tick);
            // 
            // panelStatus
            // 
            this.panelStatus.BackgroundImage = global::SOPManager.Properties.Resources.black_Bottom_bar;
            this.panelStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatus.Location = new System.Drawing.Point(0, 759);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(1386, 29);
            this.panelStatus.TabIndex = 10;
            // 
            // panelTop
            // 
            this.panelTop.BackgroundImage = global::SOPManager.Properties.Resources.TitleBar_background;
            this.panelTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelTop.Controls.Add(this.btnMin);
            this.panelTop.Controls.Add(this.btnMax);
            this.panelTop.Controls.Add(this.btnClose);
            this.panelTop.Controls.Add(this.lbTitle);
            this.panelTop.Controls.Add(this.pictureBox1);
            this.panelTop.Controls.Add(this.panelGap);
            this.panelTop.Controls.Add(this.pictureBoxFile);
            this.panelTop.Controls.Add(this.pictureBoxSOP);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1386, 101);
            this.panelTop.TabIndex = 8;
            this.panelTop.DoubleClick += new System.EventHandler(this.panelTop_DoubleClick);
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            this.panelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseUp);
            // 
            // btnMin
            // 
            this.btnMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMin.BackgroundImage = global::SOPManager.Properties.Resources.HideWindow_Normal;
            this.btnMin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMin.Location = new System.Drawing.Point(1294, 0);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(32, 24);
            this.btnMin.TabIndex = 12;
            this.btnMin.UseVisualStyleBackColor = true;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // btnMax
            // 
            this.btnMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMax.BackgroundImage = global::SOPManager.Properties.Resources.NormalWindow_Normal;
            this.btnMax.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMax.Location = new System.Drawing.Point(1324, 0);
            this.btnMax.Name = "btnMax";
            this.btnMax.Size = new System.Drawing.Size(32, 24);
            this.btnMax.TabIndex = 11;
            this.btnMax.UseVisualStyleBackColor = true;
            this.btnMax.Click += new System.EventHandler(this.btnMax_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackgroundImage = global::SOPManager.Properties.Resources.CloseWindow_Normal;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.Location = new System.Drawing.Point(1353, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(32, 24);
            this.btnClose.TabIndex = 10;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.Black;
            this.lbTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(34, 6);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(126, 15);
            this.lbTitle.TabIndex = 9;
            this.lbTitle.Text = "SOP Manager   v 2.0";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SOPManager.Properties.Resources.App_Icon_Small;
            this.pictureBox1.ImageLocation = "";
            this.pictureBox1.InitialImage = global::SOPManager.Properties.Resources.App_Icon_Small;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // panelGap
            // 
            this.panelGap.BackgroundImage = global::SOPManager.Properties.Resources.top_graybar;
            this.panelGap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelGap.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelGap.Location = new System.Drawing.Point(0, 76);
            this.panelGap.Name = "panelGap";
            this.panelGap.Size = new System.Drawing.Size(1386, 25);
            this.panelGap.TabIndex = 0;
            // 
            // ribbonButton8
            // 
            this.ribbonButton8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.ribbonButton8.BackgroundImage = global::SOPManager.Properties.Resources.RibbonBar_Middle;
            this.ribbonButton8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ribbonButton8.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.ribbonButton8.CheckedImage = global::SOPManager.Properties.Resources.deletePane_checked;
            this.ribbonButton8.CustomImageRect = new System.Drawing.Rectangle(20, 10, 32, 32);
            this.ribbonButton8.DisabledBkgndImage = null;
            this.ribbonButton8.DisabledImage = global::SOPManager.Properties.Resources.deletePane_disabled;
            this.ribbonButton8.ID = -1;
            this.ribbonButton8.InitButtonWidth = 70;
            this.ribbonButton8.IsChecked = false;
            this.ribbonButton8.Location = new System.Drawing.Point(545, 2);
            this.ribbonButton8.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.ribbonButton8.Name = "ribbonButton8";
            this.ribbonButton8.NormalImage = global::SOPManager.Properties.Resources.deletePane_normal;
            this.ribbonButton8.Owner = null;
            this.ribbonButton8.Size = new System.Drawing.Size(70, 90);
            this.ribbonButton8.TabIndex = 8;
            this.ribbonButton8.Text = "패널 삭제";
            this.ribbonButton8.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton8.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton8.UseCustomImageRect = true;
            this.ribbonButton8.UseTextLocation = false;
            this.ribbonButton8.UseVisualStyleBackColor = false;
            // 
            // ribbonButton7
            // 
            this.ribbonButton7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.ribbonButton7.BackgroundImage = global::SOPManager.Properties.Resources.RibbonBar_Middle;
            this.ribbonButton7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ribbonButton7.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.ribbonButton7.CheckedImage = global::SOPManager.Properties.Resources.addPane_Checked;
            this.ribbonButton7.CustomImageRect = new System.Drawing.Rectangle(20, 10, 32, 32);
            this.ribbonButton7.DisabledBkgndImage = null;
            this.ribbonButton7.DisabledImage = global::SOPManager.Properties.Resources.addPane_disabled;
            this.ribbonButton7.ID = -1;
            this.ribbonButton7.InitButtonWidth = 70;
            this.ribbonButton7.IsChecked = false;
            this.ribbonButton7.Location = new System.Drawing.Point(475, 2);
            this.ribbonButton7.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.ribbonButton7.Name = "ribbonButton7";
            this.ribbonButton7.NormalImage = global::SOPManager.Properties.Resources.addPane_normal;
            this.ribbonButton7.Owner = null;
            this.ribbonButton7.Size = new System.Drawing.Size(70, 90);
            this.ribbonButton7.TabIndex = 7;
            this.ribbonButton7.Text = "패널 추가";
            this.ribbonButton7.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton7.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton7.UseCustomImageRect = true;
            this.ribbonButton7.UseTextLocation = false;
            this.ribbonButton7.UseVisualStyleBackColor = false;
            // 
            // ribbonButton6
            // 
            this.ribbonButton6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.ribbonButton6.BackgroundImage = global::SOPManager.Properties.Resources.RibbonBar_Middle;
            this.ribbonButton6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ribbonButton6.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.ribbonButton6.CheckedImage = global::SOPManager.Properties.Resources.addTab_checked;
            this.ribbonButton6.CustomImageRect = new System.Drawing.Rectangle(20, 10, 32, 32);
            this.ribbonButton6.DisabledBkgndImage = null;
            this.ribbonButton6.DisabledImage = global::SOPManager.Properties.Resources.addTab_disabled;
            this.ribbonButton6.ID = -1;
            this.ribbonButton6.InitButtonWidth = 70;
            this.ribbonButton6.IsChecked = false;
            this.ribbonButton6.Location = new System.Drawing.Point(400, 2);
            this.ribbonButton6.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.ribbonButton6.Name = "ribbonButton6";
            this.ribbonButton6.NormalImage = global::SOPManager.Properties.Resources.addTab_normal;
            this.ribbonButton6.Owner = null;
            this.ribbonButton6.Size = new System.Drawing.Size(70, 90);
            this.ribbonButton6.TabIndex = 6;
            this.ribbonButton6.Text = "단계 추가";
            this.ribbonButton6.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton6.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton6.UseCustomImageRect = true;
            this.ribbonButton6.UseTextLocation = false;
            this.ribbonButton6.UseVisualStyleBackColor = false;
            // 
            // ribbonButton5
            // 
            this.ribbonButton5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.ribbonButton5.BackgroundImage = global::SOPManager.Properties.Resources.RibbonBar_Middle;
            this.ribbonButton5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ribbonButton5.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.ribbonButton5.CheckedImage = global::SOPManager.Properties.Resources.deleteTab_checked;
            this.ribbonButton5.CustomImageRect = new System.Drawing.Rectangle(20, 10, 32, 32);
            this.ribbonButton5.DisabledBkgndImage = null;
            this.ribbonButton5.DisabledImage = global::SOPManager.Properties.Resources.deleteTab_disabled;
            this.ribbonButton5.ID = -1;
            this.ribbonButton5.InitButtonWidth = 70;
            this.ribbonButton5.IsChecked = false;
            this.ribbonButton5.Location = new System.Drawing.Point(330, 2);
            this.ribbonButton5.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.ribbonButton5.Name = "ribbonButton5";
            this.ribbonButton5.NormalImage = global::SOPManager.Properties.Resources.deleteTab_normal;
            this.ribbonButton5.Owner = null;
            this.ribbonButton5.Size = new System.Drawing.Size(70, 90);
            this.ribbonButton5.TabIndex = 5;
            this.ribbonButton5.Text = "단계 삭제";
            this.ribbonButton5.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton5.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton5.UseCustomImageRect = true;
            this.ribbonButton5.UseTextLocation = false;
            this.ribbonButton5.UseVisualStyleBackColor = false;
            // 
            // ribbonButton4
            // 
            this.ribbonButton4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.ribbonButton4.BackgroundImage = global::SOPManager.Properties.Resources.RibbonBar_Middle;
            this.ribbonButton4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ribbonButton4.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.ribbonButton4.CheckedImage = global::SOPManager.Properties.Resources.copytab_checked;
            this.ribbonButton4.CustomImageRect = new System.Drawing.Rectangle(20, 10, 32, 32);
            this.ribbonButton4.DisabledBkgndImage = null;
            this.ribbonButton4.DisabledImage = global::SOPManager.Properties.Resources.copytab_disabled;
            this.ribbonButton4.ID = -1;
            this.ribbonButton4.InitButtonWidth = 70;
            this.ribbonButton4.IsChecked = false;
            this.ribbonButton4.Location = new System.Drawing.Point(260, 2);
            this.ribbonButton4.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.ribbonButton4.Name = "ribbonButton4";
            this.ribbonButton4.NormalImage = global::SOPManager.Properties.Resources.copytab_normal;
            this.ribbonButton4.Owner = null;
            this.ribbonButton4.Size = new System.Drawing.Size(70, 90);
            this.ribbonButton4.TabIndex = 4;
            this.ribbonButton4.Text = "단계 복사";
            this.ribbonButton4.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton4.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton4.UseCustomImageRect = true;
            this.ribbonButton4.UseTextLocation = false;
            this.ribbonButton4.UseVisualStyleBackColor = false;
            // 
            // ribbonButton3
            // 
            this.ribbonButton3.BackColor = System.Drawing.Color.Transparent;
            this.ribbonButton3.BackgroundImage = global::SOPManager.Properties.Resources.RibbonBar_Middle;
            this.ribbonButton3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ribbonButton3.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.ribbonButton3.CheckedImage = global::SOPManager.Properties.Resources.pasteTab_Checked;
            this.ribbonButton3.CustomImageRect = new System.Drawing.Rectangle(30, 10, 32, 32);
            this.ribbonButton3.DisabledBkgndImage = null;
            this.ribbonButton3.DisabledImage = global::SOPManager.Properties.Resources.pasteTab_disabled;
            this.ribbonButton3.ID = -1;
            this.ribbonButton3.InitButtonWidth = 70;
            this.ribbonButton3.IsChecked = false;
            this.ribbonButton3.Location = new System.Drawing.Point(190, 2);
            this.ribbonButton3.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.ribbonButton3.Name = "ribbonButton3";
            this.ribbonButton3.NormalImage = global::SOPManager.Properties.Resources.pasteTab_normal;
            this.ribbonButton3.Owner = null;
            this.ribbonButton3.Size = new System.Drawing.Size(81, 90);
            this.ribbonButton3.TabIndex = 3;
            this.ribbonButton3.Text = "단계붙여넣기";
            this.ribbonButton3.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton3.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton3.UseCustomImageRect = true;
            this.ribbonButton3.UseTextLocation = false;
            this.ribbonButton3.UseVisualStyleBackColor = false;
            // 
            // ribbonButton2
            // 
            this.ribbonButton2.BackColor = System.Drawing.Color.Transparent;
            this.ribbonButton2.BackgroundImage = global::SOPManager.Properties.Resources.RibbonBar_Middle;
            this.ribbonButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ribbonButton2.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.ribbonButton2.CheckedImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton2.CheckedImage")));
            this.ribbonButton2.CustomImageRect = new System.Drawing.Rectangle(20, 10, 32, 32);
            this.ribbonButton2.DisabledBkgndImage = null;
            this.ribbonButton2.DisabledImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton2.DisabledImage")));
            this.ribbonButton2.ID = -1;
            this.ribbonButton2.InitButtonWidth = 70;
            this.ribbonButton2.IsChecked = false;
            this.ribbonButton2.Location = new System.Drawing.Point(115, 2);
            this.ribbonButton2.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.ribbonButton2.Name = "ribbonButton2";
            this.ribbonButton2.NormalImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton2.NormalImage")));
            this.ribbonButton2.Owner = null;
            this.ribbonButton2.Size = new System.Drawing.Size(70, 90);
            this.ribbonButton2.TabIndex = 2;
            this.ribbonButton2.Text = "저장";
            this.ribbonButton2.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton2.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton2.UseCustomImageRect = true;
            this.ribbonButton2.UseTextLocation = false;
            this.ribbonButton2.UseVisualStyleBackColor = false;
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.BackColor = System.Drawing.Color.Transparent;
            this.ribbonButton1.BackgroundImage = global::SOPManager.Properties.Resources.RibbonBar_Middle;
            this.ribbonButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ribbonButton1.CheckedBkgndImage = global::SOPManager.Properties.Resources.RibbonChecked_bkgnd;
            this.ribbonButton1.CheckedImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.CheckedImage")));
            this.ribbonButton1.CustomImageRect = new System.Drawing.Rectangle(20, 10, 32, 32);
            this.ribbonButton1.DisabledBkgndImage = null;
            this.ribbonButton1.DisabledImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.DisabledImage")));
            this.ribbonButton1.ID = -1;
            this.ribbonButton1.InitButtonWidth = 70;
            this.ribbonButton1.IsChecked = false;
            this.ribbonButton1.Location = new System.Drawing.Point(45, 2);
            this.ribbonButton1.MouseOverBkgndImage = global::SOPManager.Properties.Resources.RibbonMouseOver_bkgnd;
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.NormalImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.NormalImage")));
            this.ribbonButton1.Owner = null;
            this.ribbonButton1.Size = new System.Drawing.Size(70, 90);
            this.ribbonButton1.TabIndex = 1;
            this.ribbonButton1.Text = "되돌리기";
            this.ribbonButton1.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton1.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton1.UseCustomImageRect = true;
            this.ribbonButton1.UseTextLocation = false;
            this.ribbonButton1.UseVisualStyleBackColor = false;
            // 
            // btnNewSOP
            // 
            this.btnNewSOP.CheckedBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnNewSOP.CheckedImage = null;
            this.btnNewSOP.CustomImageRect = new System.Drawing.Rectangle(30, 20, 32, 32);
            this.btnNewSOP.DisabledBkgndImage = null;
            this.btnNewSOP.DisabledImage = null;
            this.btnNewSOP.ID = -1;
            this.btnNewSOP.InitButtonWidth = 224;
            this.btnNewSOP.IsChecked = false;
            this.btnNewSOP.Location = new System.Drawing.Point(1, 350);
            this.btnNewSOP.MouseOverBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnNewSOP.Name = "btnNewSOP";
            this.btnNewSOP.NormalImage = global::SOPManager.Properties.Resources.leftNewSOP;
            this.btnNewSOP.Owner = null;
            this.btnNewSOP.Size = new System.Drawing.Size(224, 70);
            this.btnNewSOP.TabIndex = 4;
            this.btnNewSOP.Text = "새 SOP";
            this.btnNewSOP.TextLocation = new System.Drawing.Point(0, 0);
            this.btnNewSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnNewSOP.UseCustomImageRect = true;
            this.btnNewSOP.UseTextLocation = false;
            this.btnNewSOP.UseVisualStyleBackColor = true;
            // 
            // btnDeleteSOP
            // 
            this.btnDeleteSOP.CheckedBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnDeleteSOP.CheckedImage = null;
            this.btnDeleteSOP.CustomImageRect = new System.Drawing.Rectangle(30, 20, 32, 32);
            this.btnDeleteSOP.DisabledBkgndImage = null;
            this.btnDeleteSOP.DisabledImage = null;
            this.btnDeleteSOP.ID = -1;
            this.btnDeleteSOP.InitButtonWidth = 224;
            this.btnDeleteSOP.IsChecked = false;
            this.btnDeleteSOP.Location = new System.Drawing.Point(2, 138);
            this.btnDeleteSOP.MouseOverBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnDeleteSOP.Name = "btnDeleteSOP";
            this.btnDeleteSOP.NormalImage = global::SOPManager.Properties.Resources.leftDeleteSOP;
            this.btnDeleteSOP.Owner = null;
            this.btnDeleteSOP.Size = new System.Drawing.Size(224, 70);
            this.btnDeleteSOP.TabIndex = 3;
            this.btnDeleteSOP.Text = "삭제";
            this.btnDeleteSOP.TextLocation = new System.Drawing.Point(0, 0);
            this.btnDeleteSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnDeleteSOP.UseCustomImageRect = true;
            this.btnDeleteSOP.UseTextLocation = false;
            this.btnDeleteSOP.UseVisualStyleBackColor = true;
            // 
            // btnOpenXML
            // 
            this.btnOpenXML.CheckedBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnOpenXML.CheckedImage = null;
            this.btnOpenXML.CustomImageRect = new System.Drawing.Rectangle(30, 20, 32, 32);
            this.btnOpenXML.DisabledBkgndImage = null;
            this.btnOpenXML.DisabledImage = null;
            this.btnOpenXML.ID = -1;
            this.btnOpenXML.InitButtonWidth = 224;
            this.btnOpenXML.IsChecked = false;
            this.btnOpenXML.Location = new System.Drawing.Point(1, 212);
            this.btnOpenXML.MouseOverBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnOpenXML.Name = "btnOpenXML";
            this.btnOpenXML.NormalImage = global::SOPManager.Properties.Resources.LeftOpenXML;
            this.btnOpenXML.Owner = null;
            this.btnOpenXML.Size = new System.Drawing.Size(224, 70);
            this.btnOpenXML.TabIndex = 3;
            this.btnOpenXML.Text = "XML 열기";
            this.btnOpenXML.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOpenXML.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnOpenXML.UseCustomImageRect = true;
            this.btnOpenXML.UseTextLocation = false;
            this.btnOpenXML.UseVisualStyleBackColor = true;
            // 
            // btnSaveXML
            // 
            this.btnSaveXML.CheckedBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnSaveXML.CheckedImage = null;
            this.btnSaveXML.CustomImageRect = new System.Drawing.Rectangle(30, 20, 32, 32);
            this.btnSaveXML.DisabledBkgndImage = null;
            this.btnSaveXML.DisabledImage = null;
            this.btnSaveXML.ID = -1;
            this.btnSaveXML.InitButtonWidth = 224;
            this.btnSaveXML.IsChecked = false;
            this.btnSaveXML.Location = new System.Drawing.Point(1, 278);
            this.btnSaveXML.MouseOverBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnSaveXML.Name = "btnSaveXML";
            this.btnSaveXML.NormalImage = global::SOPManager.Properties.Resources.LeftSave;
            this.btnSaveXML.Owner = null;
            this.btnSaveXML.Size = new System.Drawing.Size(224, 70);
            this.btnSaveXML.TabIndex = 2;
            this.btnSaveXML.Text = "XML 저장";
            this.btnSaveXML.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSaveXML.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSaveXML.UseCustomImageRect = true;
            this.btnSaveXML.UseTextLocation = false;
            this.btnSaveXML.UseVisualStyleBackColor = true;
            // 
            // btnOpen
            // 
            this.btnOpen.CheckedBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnOpen.CheckedImage = null;
            this.btnOpen.CustomImageRect = new System.Drawing.Rectangle(30, 20, 32, 32);
            this.btnOpen.DisabledBkgndImage = null;
            this.btnOpen.DisabledImage = null;
            this.btnOpen.ID = -1;
            this.btnOpen.InitButtonWidth = 224;
            this.btnOpen.IsChecked = true;
            this.btnOpen.Location = new System.Drawing.Point(1, 70);
            this.btnOpen.MouseOverBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.NormalImage = global::SOPManager.Properties.Resources.LeftOpen;
            this.btnOpen.Owner = null;
            this.btnOpen.Size = new System.Drawing.Size(224, 70);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "열기";
            this.btnOpen.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOpen.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOpen.UseCustomImageRect = true;
            this.btnOpen.UseTextLocation = false;
            this.btnOpen.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.CheckedBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnSave.CheckedImage = null;
            this.btnSave.CustomImageRect = new System.Drawing.Rectangle(30, 20, 32, 32);
            this.btnSave.DisabledBkgndImage = null;
            this.btnSave.DisabledImage = null;
            this.btnSave.ID = -1;
            this.btnSave.InitButtonWidth = 224;
            this.btnSave.IsChecked = false;
            this.btnSave.Location = new System.Drawing.Point(1, 2);
            this.btnSave.MouseOverBkgndImage = global::SOPManager.Properties.Resources.select_skyblue;
            this.btnSave.Name = "btnSave";
            this.btnSave.NormalImage = global::SOPManager.Properties.Resources.LeftSave;
            this.btnSave.Owner = null;
            this.btnSave.Size = new System.Drawing.Size(224, 70);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "저장";
            this.btnSave.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSave.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSave.UseCustomImageRect = true;
            this.btnSave.UseTextLocation = false;
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // pictureBoxFile
            // 
            this.pictureBoxFile.BackColor = System.Drawing.Color.Red;
            this.pictureBoxFile.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Normal;
            this.pictureBoxFile.Image = global::SOPManager.Properties.Resources.Selection;
            this.pictureBoxFile.Location = new System.Drawing.Point(0, 36);
            this.pictureBoxFile.Name = "pictureBoxFile";
            this.pictureBoxFile.Size = new System.Drawing.Size(120, 40);
            this.pictureBoxFile.TabIndex = 2;
            this.pictureBoxFile.TabStop = false;
            this.pictureBoxFile.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxSOP
            // 
            this.pictureBoxSOP.BackColor = System.Drawing.Color.DodgerBlue;
            this.pictureBoxSOP.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Normal;
            this.pictureBoxSOP.Location = new System.Drawing.Point(120, 36);
            this.pictureBoxSOP.Name = "pictureBoxSOP";
            this.pictureBoxSOP.Size = new System.Drawing.Size(120, 40);
            this.pictureBoxSOP.TabIndex = 1;
            this.pictureBoxSOP.TabStop = false;
            this.pictureBoxSOP.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(1386, 788);
            this.ControlBox = false;
            this.Controls.Add(this.panelSection);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.panelTop);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SOP Manager   v 2.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panelContent.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelSection.ResumeLayout(false);
            this.panelRibbon.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSOP)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private UnE.GUI.TextPictureBox pictureBoxSOP;
		private UnE.GUI.TextPictureBox pictureBoxFile;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelGap;
		private System.Windows.Forms.Label lbTitle;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button btnMin;
		private System.Windows.Forms.Button btnMax;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Panel panelStatus;
		private System.Windows.Forms.Panel panelContent;
		private System.Windows.Forms.Panel panelLeft;
		private System.Windows.Forms.Panel panelSection;
		private System.Windows.Forms.Panel panelForm;
		private UnE.GUI.RibbonButton btnOpenXML;
		private UnE.GUI.RibbonButton btnSaveXML;
		private UnE.GUI.RibbonButton btnOpen;
		private UnE.GUI.RibbonButton btnSave;
		private UnE.GUI.RibbonButton btnNewSOP;
		private System.Windows.Forms.Panel panelSectionContent;
		private System.Windows.Forms.Panel panelRibbon;
		private UnE.GUI.RibbonButton ribbonButton8;
		private UnE.GUI.RibbonButton ribbonButton7;
		private UnE.GUI.RibbonButton ribbonButton6;
		private UnE.GUI.RibbonButton ribbonButton5;
		private UnE.GUI.RibbonButton ribbonButton4;
		private UnE.GUI.RibbonButton ribbonButton3;
		private UnE.GUI.RibbonButton ribbonButton2;
		private UnE.GUI.RibbonButton ribbonButton1;
        private System.Windows.Forms.Timer m_tmrCmdUpdate;
        private UnE.GUI.RibbonButton btnDeleteSOP;
	}
}