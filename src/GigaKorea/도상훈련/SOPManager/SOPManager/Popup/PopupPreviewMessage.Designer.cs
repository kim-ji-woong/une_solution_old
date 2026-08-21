namespace SOPManager
{
    partial class PopupPreviewMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupPreviewMessage));
            this.textBoxPreview = new System.Windows.Forms.TextBox();
            this.panelBackground = new System.Windows.Forms.Panel();
            this.picNight = new System.Windows.Forms.PictureBox();
            this.lblNight = new System.Windows.Forms.Label();
            this.picDay = new System.Windows.Forms.PictureBox();
            this.lblDay = new System.Windows.Forms.Label();
            this.picIgnoreDay = new System.Windows.Forms.PictureBox();
            this.lblIgnoreDay = new System.Windows.Forms.Label();
            this.picReal = new System.Windows.Forms.PictureBox();
            this.lblReal = new System.Windows.Forms.Label();
            this.picVirtual = new System.Windows.Forms.PictureBox();
            this.lblVirtual = new System.Windows.Forms.Label();
            this.picIgnoreVirtual = new System.Windows.Forms.PictureBox();
            this.lblIgnoreVirtual = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioIgnoreDay = new System.Windows.Forms.RadioButton();
            this.radioDay = new System.Windows.Forms.RadioButton();
            this.radioNight = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioIgnoreVirtual = new System.Windows.Forms.RadioButton();
            this.radioReal = new System.Windows.Forms.RadioButton();
            this.radioVirtual = new System.Windows.Forms.RadioButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.panelBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIgnoreDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picReal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVirtual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIgnoreVirtual)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxPreview
            // 
            this.textBoxPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPreview.BackColor = System.Drawing.Color.White;
            this.textBoxPreview.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxPreview.Location = new System.Drawing.Point(3, 7);
            this.textBoxPreview.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxPreview.Multiline = true;
            this.textBoxPreview.Name = "textBoxPreview";
            this.textBoxPreview.ReadOnly = true;
            this.textBoxPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPreview.Size = new System.Drawing.Size(510, 302);
            this.textBoxPreview.TabIndex = 18;
            // 
            // panelBackground
            // 
            this.panelBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panelBackground.Controls.Add(this.btnOK);
            this.panelBackground.Controls.Add(this.picNight);
            this.panelBackground.Controls.Add(this.lblNight);
            this.panelBackground.Controls.Add(this.picDay);
            this.panelBackground.Controls.Add(this.lblDay);
            this.panelBackground.Controls.Add(this.picIgnoreDay);
            this.panelBackground.Controls.Add(this.lblIgnoreDay);
            this.panelBackground.Controls.Add(this.picReal);
            this.panelBackground.Controls.Add(this.lblReal);
            this.panelBackground.Controls.Add(this.picVirtual);
            this.panelBackground.Controls.Add(this.lblVirtual);
            this.panelBackground.Controls.Add(this.picIgnoreVirtual);
            this.panelBackground.Controls.Add(this.lblIgnoreVirtual);
            this.panelBackground.Controls.Add(this.panel2);
            this.panelBackground.Controls.Add(this.panel1);
            this.panelBackground.Controls.Add(this.textBoxPreview);
            this.panelBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBackground.Location = new System.Drawing.Point(0, 0);
            this.panelBackground.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(516, 416);
            this.panelBackground.TabIndex = 19;
            // 
            // picNight
            // 
            this.picNight.BackColor = System.Drawing.Color.Transparent;
            this.picNight.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picNight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picNight.Location = new System.Drawing.Point(140, 382);
            this.picNight.Name = "picNight";
            this.picNight.Size = new System.Drawing.Size(22, 22);
            this.picNight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picNight.TabIndex = 94;
            this.picNight.TabStop = false;
            this.picNight.Click += new System.EventHandler(this.Night_Click);
            // 
            // lblNight
            // 
            this.lblNight.AutoSize = true;
            this.lblNight.BackColor = System.Drawing.Color.Transparent;
            this.lblNight.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblNight.ForeColor = System.Drawing.Color.White;
            this.lblNight.Location = new System.Drawing.Point(168, 384);
            this.lblNight.Name = "lblNight";
            this.lblNight.Size = new System.Drawing.Size(130, 18);
            this.lblNight.TabIndex = 93;
            this.lblNight.Text = "야간 및 휴일모드";
            this.lblNight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNight.Click += new System.EventHandler(this.Night_Click);
            // 
            // picDay
            // 
            this.picDay.BackColor = System.Drawing.Color.Transparent;
            this.picDay.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picDay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picDay.Location = new System.Drawing.Point(140, 349);
            this.picDay.Name = "picDay";
            this.picDay.Size = new System.Drawing.Size(22, 22);
            this.picDay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picDay.TabIndex = 92;
            this.picDay.TabStop = false;
            this.picDay.Click += new System.EventHandler(this.Day_Click);
            // 
            // lblDay
            // 
            this.lblDay.AutoSize = true;
            this.lblDay.BackColor = System.Drawing.Color.Transparent;
            this.lblDay.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDay.ForeColor = System.Drawing.Color.White;
            this.lblDay.Location = new System.Drawing.Point(168, 352);
            this.lblDay.Name = "lblDay";
            this.lblDay.Size = new System.Drawing.Size(109, 18);
            this.lblDay.TabIndex = 91;
            this.lblDay.Text = "평일 주간모드";
            this.lblDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDay.Click += new System.EventHandler(this.Day_Click);
            // 
            // picIgnoreDay
            // 
            this.picIgnoreDay.BackColor = System.Drawing.Color.Transparent;
            this.picIgnoreDay.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picIgnoreDay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picIgnoreDay.Location = new System.Drawing.Point(140, 316);
            this.picIgnoreDay.Name = "picIgnoreDay";
            this.picIgnoreDay.Size = new System.Drawing.Size(22, 22);
            this.picIgnoreDay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picIgnoreDay.TabIndex = 90;
            this.picIgnoreDay.TabStop = false;
            this.picIgnoreDay.Click += new System.EventHandler(this.IgnoreDay_Click);
            // 
            // lblIgnoreDay
            // 
            this.lblIgnoreDay.AutoSize = true;
            this.lblIgnoreDay.BackColor = System.Drawing.Color.Transparent;
            this.lblIgnoreDay.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblIgnoreDay.ForeColor = System.Drawing.Color.White;
            this.lblIgnoreDay.Location = new System.Drawing.Point(168, 318);
            this.lblIgnoreDay.Name = "lblIgnoreDay";
            this.lblIgnoreDay.Size = new System.Drawing.Size(40, 18);
            this.lblIgnoreDay.TabIndex = 89;
            this.lblIgnoreDay.Text = "무시";
            this.lblIgnoreDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblIgnoreDay.Click += new System.EventHandler(this.IgnoreDay_Click);
            // 
            // picReal
            // 
            this.picReal.BackColor = System.Drawing.Color.Transparent;
            this.picReal.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picReal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picReal.Location = new System.Drawing.Point(12, 382);
            this.picReal.Name = "picReal";
            this.picReal.Size = new System.Drawing.Size(22, 22);
            this.picReal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picReal.TabIndex = 88;
            this.picReal.TabStop = false;
            this.picReal.Click += new System.EventHandler(this.Real_Click);
            // 
            // lblReal
            // 
            this.lblReal.AutoSize = true;
            this.lblReal.BackColor = System.Drawing.Color.Transparent;
            this.lblReal.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblReal.ForeColor = System.Drawing.Color.White;
            this.lblReal.Location = new System.Drawing.Point(40, 384);
            this.lblReal.Name = "lblReal";
            this.lblReal.Size = new System.Drawing.Size(72, 18);
            this.lblReal.TabIndex = 87;
            this.lblReal.Text = "실제모드";
            this.lblReal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblReal.Click += new System.EventHandler(this.Real_Click);
            // 
            // picVirtual
            // 
            this.picVirtual.BackColor = System.Drawing.Color.Transparent;
            this.picVirtual.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picVirtual.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picVirtual.Location = new System.Drawing.Point(12, 349);
            this.picVirtual.Name = "picVirtual";
            this.picVirtual.Size = new System.Drawing.Size(22, 22);
            this.picVirtual.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picVirtual.TabIndex = 86;
            this.picVirtual.TabStop = false;
            this.picVirtual.Click += new System.EventHandler(this.Virtual_Click);
            // 
            // lblVirtual
            // 
            this.lblVirtual.AutoSize = true;
            this.lblVirtual.BackColor = System.Drawing.Color.Transparent;
            this.lblVirtual.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblVirtual.ForeColor = System.Drawing.Color.White;
            this.lblVirtual.Location = new System.Drawing.Point(40, 351);
            this.lblVirtual.Name = "lblVirtual";
            this.lblVirtual.Size = new System.Drawing.Size(72, 18);
            this.lblVirtual.TabIndex = 85;
            this.lblVirtual.Text = "훈련모드";
            this.lblVirtual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblVirtual.Click += new System.EventHandler(this.Virtual_Click);
            // 
            // picIgnoreVirtual
            // 
            this.picIgnoreVirtual.BackColor = System.Drawing.Color.Transparent;
            this.picIgnoreVirtual.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picIgnoreVirtual.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picIgnoreVirtual.Location = new System.Drawing.Point(12, 316);
            this.picIgnoreVirtual.Name = "picIgnoreVirtual";
            this.picIgnoreVirtual.Size = new System.Drawing.Size(22, 22);
            this.picIgnoreVirtual.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picIgnoreVirtual.TabIndex = 84;
            this.picIgnoreVirtual.TabStop = false;
            this.picIgnoreVirtual.Click += new System.EventHandler(this.IgnoreVirtual_Click);
            // 
            // lblIgnoreVirtual
            // 
            this.lblIgnoreVirtual.AutoSize = true;
            this.lblIgnoreVirtual.BackColor = System.Drawing.Color.Transparent;
            this.lblIgnoreVirtual.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblIgnoreVirtual.ForeColor = System.Drawing.Color.White;
            this.lblIgnoreVirtual.Location = new System.Drawing.Point(40, 318);
            this.lblIgnoreVirtual.Name = "lblIgnoreVirtual";
            this.lblIgnoreVirtual.Size = new System.Drawing.Size(40, 18);
            this.lblIgnoreVirtual.TabIndex = 83;
            this.lblIgnoreVirtual.Text = "무시";
            this.lblIgnoreVirtual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblIgnoreVirtual.Click += new System.EventHandler(this.IgnoreVirtual_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel2.Controls.Add(this.radioIgnoreDay);
            this.panel2.Controls.Add(this.radioDay);
            this.panel2.Controls.Add(this.radioNight);
            this.panel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel2.Location = new System.Drawing.Point(356, 316);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(58, 90);
            this.panel2.TabIndex = 21;
            this.panel2.Visible = false;
            // 
            // radioIgnoreDay
            // 
            this.radioIgnoreDay.AutoSize = true;
            this.radioIgnoreDay.BackColor = System.Drawing.Color.Transparent;
            this.radioIgnoreDay.Checked = true;
            this.radioIgnoreDay.ForeColor = System.Drawing.Color.White;
            this.radioIgnoreDay.Location = new System.Drawing.Point(5, 8);
            this.radioIgnoreDay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioIgnoreDay.Name = "radioIgnoreDay";
            this.radioIgnoreDay.Size = new System.Drawing.Size(47, 17);
            this.radioIgnoreDay.TabIndex = 20;
            this.radioIgnoreDay.TabStop = true;
            this.radioIgnoreDay.Text = "무시";
            this.radioIgnoreDay.UseVisualStyleBackColor = false;
            this.radioIgnoreDay.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioDay
            // 
            this.radioDay.AutoSize = true;
            this.radioDay.BackColor = System.Drawing.Color.Transparent;
            this.radioDay.ForeColor = System.Drawing.Color.White;
            this.radioDay.Location = new System.Drawing.Point(5, 36);
            this.radioDay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioDay.Name = "radioDay";
            this.radioDay.Size = new System.Drawing.Size(94, 17);
            this.radioDay.TabIndex = 20;
            this.radioDay.Text = "평일 주간모드";
            this.radioDay.UseVisualStyleBackColor = false;
            this.radioDay.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioNight
            // 
            this.radioNight.AutoSize = true;
            this.radioNight.BackColor = System.Drawing.Color.Transparent;
            this.radioNight.ForeColor = System.Drawing.Color.White;
            this.radioNight.Location = new System.Drawing.Point(5, 64);
            this.radioNight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioNight.Name = "radioNight";
            this.radioNight.Size = new System.Drawing.Size(108, 17);
            this.radioNight.TabIndex = 20;
            this.radioNight.Text = "야간 및 휴일모드";
            this.radioNight.UseVisualStyleBackColor = false;
            this.radioNight.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.radioIgnoreVirtual);
            this.panel1.Controls.Add(this.radioReal);
            this.panel1.Controls.Add(this.radioVirtual);
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel1.Location = new System.Drawing.Point(303, 316);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(47, 90);
            this.panel1.TabIndex = 21;
            this.panel1.Visible = false;
            // 
            // radioIgnoreVirtual
            // 
            this.radioIgnoreVirtual.AutoSize = true;
            this.radioIgnoreVirtual.BackColor = System.Drawing.Color.Transparent;
            this.radioIgnoreVirtual.ForeColor = System.Drawing.Color.White;
            this.radioIgnoreVirtual.Location = new System.Drawing.Point(6, 8);
            this.radioIgnoreVirtual.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioIgnoreVirtual.Name = "radioIgnoreVirtual";
            this.radioIgnoreVirtual.Size = new System.Drawing.Size(47, 17);
            this.radioIgnoreVirtual.TabIndex = 20;
            this.radioIgnoreVirtual.Text = "무시";
            this.radioIgnoreVirtual.UseVisualStyleBackColor = false;
            this.radioIgnoreVirtual.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioReal
            // 
            this.radioReal.AutoSize = true;
            this.radioReal.BackColor = System.Drawing.Color.Transparent;
            this.radioReal.ForeColor = System.Drawing.Color.White;
            this.radioReal.Location = new System.Drawing.Point(6, 64);
            this.radioReal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioReal.Name = "radioReal";
            this.radioReal.Size = new System.Drawing.Size(69, 17);
            this.radioReal.TabIndex = 20;
            this.radioReal.Text = "실제모드";
            this.radioReal.UseVisualStyleBackColor = false;
            this.radioReal.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioVirtual
            // 
            this.radioVirtual.AutoSize = true;
            this.radioVirtual.BackColor = System.Drawing.Color.Transparent;
            this.radioVirtual.Checked = true;
            this.radioVirtual.ForeColor = System.Drawing.Color.White;
            this.radioVirtual.Location = new System.Drawing.Point(6, 36);
            this.radioVirtual.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioVirtual.Name = "radioVirtual";
            this.radioVirtual.Size = new System.Drawing.Size(69, 17);
            this.radioVirtual.TabIndex = 20;
            this.radioVirtual.TabStop = true;
            this.radioVirtual.Text = "훈련모드";
            this.radioVirtual.UseVisualStyleBackColor = false;
            this.radioVirtual.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 69;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(447, 376);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 96;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PopupPreviewMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(516, 416);
            this.Controls.Add(this.panelBackground);
            this.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(516, 345);
            this.Name = "PopupPreviewMessage";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "메시지 미리보기";
            this.panelBackground.ResumeLayout(false);
            this.panelBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIgnoreDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picReal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVirtual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIgnoreVirtual)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TextBox textBoxPreview;
        private System.Windows.Forms.Panel panelBackground;
        private System.Windows.Forms.RadioButton radioVirtual;
        private System.Windows.Forms.RadioButton radioIgnoreVirtual;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radioIgnoreDay;
        private System.Windows.Forms.RadioButton radioDay;
        private System.Windows.Forms.RadioButton radioNight;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioReal;
        private System.Windows.Forms.PictureBox picReal;
        private System.Windows.Forms.Label lblReal;
        private System.Windows.Forms.PictureBox picVirtual;
        private System.Windows.Forms.Label lblVirtual;
        private System.Windows.Forms.PictureBox picIgnoreVirtual;
        private System.Windows.Forms.Label lblIgnoreVirtual;
        private System.Windows.Forms.PictureBox picNight;
        private System.Windows.Forms.Label lblNight;
        private System.Windows.Forms.PictureBox picDay;
        private System.Windows.Forms.Label lblDay;
        private System.Windows.Forms.PictureBox picIgnoreDay;
        private System.Windows.Forms.Label lblIgnoreDay;
        private UnE.GUI.RibbonButton btnOK;
    }
}