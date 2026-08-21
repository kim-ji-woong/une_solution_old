namespace SOPMonitoringSystem
{
    partial class PopupSOPList
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
            this.lbTitle = new System.Windows.Forms.Label();
            this.plTitleba = new System.Windows.Forms.Panel();
            this.plMainTree = new System.Windows.Forms.Panel();
            this.plSubTree = new System.Windows.Forms.Panel();
            this.plDisasterTree = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioRegular = new System.Windows.Forms.RadioButton();
            this.radioNonRegular = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.radioAbnormal = new UnE.GUI.RibbonButton();
            this.radioNormal = new UnE.GUI.RibbonButton();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pbTitle = new System.Windows.Forms.PictureBox();
            this.btnClose = new UnE.GUI.RibbonButton();
            this.btnLoadSOP = new UnE.GUI.RibbonButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.lbTitle.Font = new System.Drawing.Font("나눔바른고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(57, 27);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(147, 27);
            this.lbTitle.TabIndex = 38;
            this.lbTitle.Text = "SOP 불러오기";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseMove);
            this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseUp);
            // 
            // plTitleba
            // 
            this.plTitleba.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.plTitleba.Location = new System.Drawing.Point(0, 0);
            this.plTitleba.Name = "plTitleba";
            this.plTitleba.Size = new System.Drawing.Size(920, 80);
            this.plTitleba.TabIndex = 48;
            this.plTitleba.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plTitleba_MouseDown);
            this.plTitleba.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plTitleba_MouseMove);
            this.plTitleba.MouseUp += new System.Windows.Forms.MouseEventHandler(this.plTitleba_MouseUp);
            // 
            // plMainTree
            // 
            this.plMainTree.BackColor = System.Drawing.SystemColors.Control;
            this.plMainTree.Location = new System.Drawing.Point(20, 190);
            this.plMainTree.Name = "plMainTree";
            this.plMainTree.Size = new System.Drawing.Size(200, 471);
            this.plMainTree.TabIndex = 50;
            // 
            // plSubTree
            // 
            this.plSubTree.AutoScroll = true;
            this.plSubTree.Location = new System.Drawing.Point(260, 190);
            this.plSubTree.Name = "plSubTree";
            this.plSubTree.Size = new System.Drawing.Size(218, 471);
            this.plSubTree.TabIndex = 51;
            // 
            // plDisasterTree
            // 
            this.plDisasterTree.AutoScroll = true;
            this.plDisasterTree.Location = new System.Drawing.Point(500, 190);
            this.plDisasterTree.Name = "plDisasterTree";
            this.plDisasterTree.Size = new System.Drawing.Size(418, 471);
            this.plDisasterTree.TabIndex = 52;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioRegular);
            this.groupBox1.Controls.Add(this.radioNonRegular);
            this.groupBox1.Location = new System.Drawing.Point(606, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 39);
            this.groupBox1.TabIndex = 55;
            this.groupBox1.TabStop = false;
            this.groupBox1.Visible = false;
            // 
            // radioRegular
            // 
            this.radioRegular.AutoSize = true;
            this.radioRegular.Location = new System.Drawing.Point(13, 16);
            this.radioRegular.Name = "radioRegular";
            this.radioRegular.Size = new System.Drawing.Size(75, 16);
            this.radioRegular.TabIndex = 6;
            this.radioRegular.TabStop = true;
            this.radioRegular.Text = "등록 모드";
            this.radioRegular.UseVisualStyleBackColor = true;
            this.radioRegular.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioNonRegular
            // 
            this.radioNonRegular.AutoSize = true;
            this.radioNonRegular.Location = new System.Drawing.Point(118, 16);
            this.radioNonRegular.Name = "radioNonRegular";
            this.radioNonRegular.Size = new System.Drawing.Size(87, 16);
            this.radioNonRegular.TabIndex = 5;
            this.radioNonRegular.Text = "미등록 모드";
            this.radioNonRegular.UseVisualStyleBackColor = true;
            this.radioNonRegular.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(219)))), ((int)(((byte)(219)))));
            this.pictureBox1.Location = new System.Drawing.Point(20, 170);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(880, 2);
            this.pictureBox1.TabIndex = 119;
            this.pictureBox1.TabStop = false;
            // 
            // radioAbnormal
            // 
            this.radioAbnormal.BackColor = System.Drawing.Color.Transparent;
            this.radioAbnormal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListAbnormal_Normal;
            this.radioAbnormal.CheckButton = false;
            this.radioAbnormal.CheckedBkgndImage = null;
            this.radioAbnormal.CheckedImage = null;
            this.radioAbnormal.CheckedMouseOver = null;
            this.radioAbnormal.ClickedBackgroundImage = null;
            this.radioAbnormal.ClickedImage = null;
            this.radioAbnormal.CustomImageRect = new System.Drawing.Rectangle(0, 0, 150, 50);
            this.radioAbnormal.DisabledBkgndImage = null;
            this.radioAbnormal.DisabledImage = null;
            this.radioAbnormal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.radioAbnormal.ForeColor = System.Drawing.Color.Black;
            this.radioAbnormal.ForeColorChecked = System.Drawing.Color.White;
            this.radioAbnormal.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.radioAbnormal.ForeColorDisabled = System.Drawing.Color.White;
            this.radioAbnormal.ForeColorMouseOver = System.Drawing.Color.White;
            this.radioAbnormal.ForeColorsByTypeUse = false;
            this.radioAbnormal.ID = -1;
            this.radioAbnormal.InitButtonWidth = 150;
            this.radioAbnormal.IsChecked = false;
            this.radioAbnormal.Location = new System.Drawing.Point(146, 100);
            this.radioAbnormal.MouseOverBkgndImage = null;
            this.radioAbnormal.MouseOverImage = null;
            this.radioAbnormal.Name = "radioAbnormal";
            this.radioAbnormal.NormalImage = null;
            this.radioAbnormal.Owner = null;
            this.radioAbnormal.Size = new System.Drawing.Size(150, 50);
            this.radioAbnormal.TabIndex = 118;
            this.radioAbnormal.Text = "야간 및 휴일모드";
            this.radioAbnormal.TextLocation = new System.Drawing.Point(15, 13);
            this.radioAbnormal.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.radioAbnormal.ToolTipText = "야간 및 휴일모드";
            this.radioAbnormal.UseCustomImageRect = true;
            this.radioAbnormal.UseTextLocation = true;
            this.radioAbnormal.UseVisualStyleBackColor = false;
            this.radioAbnormal.Click += new System.EventHandler(this.radioAbnormal_Click);
            // 
            // radioNormal
            // 
            this.radioNormal.BackColor = System.Drawing.Color.Transparent;
            this.radioNormal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListNormal_Normal;
            this.radioNormal.CheckButton = false;
            this.radioNormal.CheckedBkgndImage = null;
            this.radioNormal.CheckedImage = null;
            this.radioNormal.CheckedMouseOver = null;
            this.radioNormal.ClickedBackgroundImage = null;
            this.radioNormal.ClickedImage = null;
            this.radioNormal.CustomImageRect = new System.Drawing.Rectangle(0, 0, 120, 50);
            this.radioNormal.DisabledBkgndImage = null;
            this.radioNormal.DisabledImage = null;
            this.radioNormal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.radioNormal.ForeColor = System.Drawing.Color.Black;
            this.radioNormal.ForeColorChecked = System.Drawing.Color.White;
            this.radioNormal.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.radioNormal.ForeColorDisabled = System.Drawing.Color.White;
            this.radioNormal.ForeColorMouseOver = System.Drawing.Color.White;
            this.radioNormal.ForeColorsByTypeUse = false;
            this.radioNormal.ID = -1;
            this.radioNormal.InitButtonWidth = 120;
            this.radioNormal.IsChecked = true;
            this.radioNormal.Location = new System.Drawing.Point(20, 100);
            this.radioNormal.MouseOverBkgndImage = null;
            this.radioNormal.MouseOverImage = null;
            this.radioNormal.Name = "radioNormal";
            this.radioNormal.NormalImage = null;
            this.radioNormal.Owner = null;
            this.radioNormal.Size = new System.Drawing.Size(120, 50);
            this.radioNormal.TabIndex = 117;
            this.radioNormal.Text = "평일모드";
            this.radioNormal.TextLocation = new System.Drawing.Point(28, 13);
            this.radioNormal.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.radioNormal.ToolTipText = "평일모드";
            this.radioNormal.UseCustomImageRect = true;
            this.radioNormal.UseTextLocation = true;
            this.radioNormal.UseVisualStyleBackColor = false;
            this.radioNormal.Click += new System.EventHandler(this.radioNormal_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(219)))), ((int)(((byte)(219)))));
            this.pictureBox3.Location = new System.Drawing.Point(480, 190);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(2, 471);
            this.pictureBox3.TabIndex = 53;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(219)))), ((int)(((byte)(219)))));
            this.pictureBox2.Location = new System.Drawing.Point(239, 190);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(2, 471);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // pbTitle
            // 
            this.pbTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.pbTitle.Location = new System.Drawing.Point(30, 38);
            this.pbTitle.Margin = new System.Windows.Forms.Padding(0);
            this.pbTitle.Name = "pbTitle";
            this.pbTitle.Size = new System.Drawing.Size(7, 7);
            this.pbTitle.TabIndex = 0;
            this.pbTitle.TabStop = false;
            this.pbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseDown);
            this.pbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseMove);
            this.pbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseUp);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.CheckButton = false;
            this.btnClose.CheckedBkgndImage = null;
            this.btnClose.CheckedImage = null;
            this.btnClose.CheckedMouseOver = null;
            this.btnClose.ClickedBackgroundImage = null;
            this.btnClose.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.btnClose_Selected;
            this.btnClose.CustomImageRect = new System.Drawing.Rectangle(0, 0, 30, 30);
            this.btnClose.DisabledBkgndImage = null;
            this.btnClose.DisabledImage = null;
            this.btnClose.ForeColorChecked = System.Drawing.Color.White;
            this.btnClose.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnClose.ForeColorDisabled = System.Drawing.Color.White;
            this.btnClose.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnClose.ForeColorsByTypeUse = false;
            this.btnClose.ID = -1;
            this.btnClose.InitButtonWidth = 30;
            this.btnClose.IsChecked = false;
            this.btnClose.Location = new System.Drawing.Point(871, 25);
            this.btnClose.MouseOverBkgndImage = null;
            this.btnClose.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.btnClose_MouseOver;
            this.btnClose.Name = "btnClose";
            this.btnClose.NormalImage = global::SOPMonitoringSystem.Properties.Resources.btnClose_Normal;
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(30, 30);
            this.btnClose.TabIndex = 49;
            this.btnClose.TextLocation = new System.Drawing.Point(0, 0);
            this.btnClose.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnClose.ToolTipText = "";
            this.btnClose.UseCustomImageRect = false;
            this.btnClose.UseTextLocation = false;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnLoadSOP
            // 
            this.btnLoadSOP.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadSOP.CheckButton = false;
            this.btnLoadSOP.CheckedBkgndImage = null;
            this.btnLoadSOP.CheckedImage = null;
            this.btnLoadSOP.CheckedMouseOver = null;
            this.btnLoadSOP.ClickedBackgroundImage = null;
            this.btnLoadSOP.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.btnLoadSOP_Click;
            this.btnLoadSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 195, 60);
            this.btnLoadSOP.DisabledBkgndImage = null;
            this.btnLoadSOP.DisabledImage = null;
            this.btnLoadSOP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLoadSOP.ForeColor = System.Drawing.Color.Black;
            this.btnLoadSOP.ForeColorChecked = System.Drawing.Color.White;
            this.btnLoadSOP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnLoadSOP.ForeColorDisabled = System.Drawing.Color.White;
            this.btnLoadSOP.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnLoadSOP.ForeColorsByTypeUse = false;
            this.btnLoadSOP.ID = -1;
            this.btnLoadSOP.InitButtonWidth = 195;
            this.btnLoadSOP.IsChecked = false;
            this.btnLoadSOP.Location = new System.Drawing.Point(695, 674);
            this.btnLoadSOP.MouseOverBkgndImage = null;
            this.btnLoadSOP.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.btnLoadSOP_Click;
            this.btnLoadSOP.Name = "btnLoadSOP";
            this.btnLoadSOP.NormalImage = global::SOPMonitoringSystem.Properties.Resources.btnLoadSOP;
            this.btnLoadSOP.Owner = null;
            this.btnLoadSOP.Size = new System.Drawing.Size(195, 60);
            this.btnLoadSOP.TabIndex = 128;
            this.btnLoadSOP.Text = "실행";
            this.btnLoadSOP.TextLocation = new System.Drawing.Point(77, 16);
            this.btnLoadSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnLoadSOP.ToolTipText = "실행";
            this.btnLoadSOP.UseCustomImageRect = true;
            this.btnLoadSOP.UseTextLocation = true;
            this.btnLoadSOP.UseVisualStyleBackColor = false;
            this.btnLoadSOP.Click += new System.EventHandler(this.btnLoadSOP_Click);
            // 
            // PopupSOPList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 759);
            this.Controls.Add(this.btnLoadSOP);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.radioAbnormal);
            this.Controls.Add(this.radioNormal);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.plDisasterTree);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.plSubTree);
            this.Controls.Add(this.plMainTree);
            this.Controls.Add(this.pbTitle);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.plTitleba);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupSOPList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SOP 불러오기";
            this.Load += new System.EventHandler(this.PopupSOPList_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Panel plTitleba;
        private UnE.GUI.RibbonButton btnClose;
        private System.Windows.Forms.PictureBox pbTitle;
        private System.Windows.Forms.Panel plMainTree;
        private System.Windows.Forms.Panel plSubTree;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel plDisasterTree;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioRegular;
        private System.Windows.Forms.RadioButton radioNonRegular;
        private UnE.GUI.RibbonButton radioNormal;
        private UnE.GUI.RibbonButton radioAbnormal;
        private System.Windows.Forms.PictureBox pictureBox1;
        private UnE.GUI.RibbonButton btnLoadSOP;
    }
}